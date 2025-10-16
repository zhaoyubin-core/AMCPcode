using Emgu.CV.Dnn;
using PMCLIB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static Emgu.Util.Platform;
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace AMCP
{
    /// <summary>
    /// 喷头控制端口类型定义
    /// </summary>
    public enum ExtrudePortType
    {
        AllNozzles = -2,    // 全部喷头
        NozzleA = 0,        // 喷头1出丝
        NozzleB = 1,        // 喷头2出丝
        SwitchNozzle = 2,   // 喷头切换
        CleanNozzle = 3,    // 喷头清洗
        DisplacePen = 4,    // 位移笔伸出
        RotaryValve = 5     // 螺杆阀
    }

    public enum AlarmType
    {
        Alarm_Standby = 0,              // 待机状态
        Alarm_Normal = 1,               // 开始正常工作
        Alarm_Printing_Completed = 2,   // 打印完成报警
        Alarm_Printing_Error = 3,       // 打印出错报警
        Alarm_Operate_Succuss = 4,      // 操作成功
        Alarm_Operate_Fail = 5,         // 操作失败
        Alarm_Operate_Warning = 6       // 存在问题，发出警示
    }
    public class ACSMotionControl : PrintingControl
    {
        public SPIIPLUSCOM660Lib.Channel Ch;
        public bool bConnected;//连接ACS
        public bool IsSimulator;
        public bool IsCancelled;
        public bool IsPrinting = false;
        public bool bPhotoTaken = true;
        public string photoName;
        public string NoticeInfo = "";//提示信息，进度
        public string InfoColor = "TransParent";
        public bool bPenInPos = true;
        public int detectPercent = 0; // 已检测百分比
        //Engine5

        public bool bPMConnected = false;//PMC连接状态
        public bool isTransPorting = false;//机械臂转运

        public int workMode { get; set; }
        public ParaSetting paraSettingObj;
        public DataManagement DataObj;
        public StageStatus Status;

        public int[] extruderOutports = new int[] { 0 };//存储出气端口号
        public int[] cureOutports = new int[] { 0 };
        public List<string> Controlcommand = new List<string>();//存储写入指令信息
        public List<string> Monitordata = new List<string>();//存储写入的日志信息

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="paraSetObj"></param>
        /// <param name="DataManagementObj"></param>
        public ACSMotionControl(ParaSetting paraSetObj, DataManagement DataManagementObj)
        {
            Ch = new SPIIPLUSCOM660Lib.Channel();
            paraSettingObj = paraSetObj;
            DataObj = DataManagementObj;
            Status = new StageStatus();
            Status.statusMsg = new List<string>();
        }

        /// <summary>
        /// 查询ACS控制器是否连接成功
        /// </summary>
        /// <returns></returns>
        public bool IfConnected()
        {
            SPIIPLUSCOM660Lib.CONNECTION_INFO connInfo;
            Ch.GetConnectionInfo(out connInfo);
            bConnected = connInfo.Type == SPIIPLUSCOM660Lib.CONNECTION_TYPE.ETHERNET;
            return bConnected;
        }

        /// <summary>
        /// 连接到ACS控制器
        /// </summary>
        /// <param name="Address">IP地址</param>
        /// <returns>返回是否连接成功</returns>
        public bool OpenCommEthernet(string Address)
        {
            try
            {
                if (IfConnected())
                {
                    CloseComm();
                }
                Ch.OpenCommEthernet(Address, Ch.ACSC_SOCKET_STREAM_PORT);
                bConnected = true;
                IsSimulator = Address.StartsWith("127.0.0.1");
            }
            catch (Exception ex)
            {
                bConnected = false;
                PushMessage(ex.Message);
            }
            return bConnected;
        }

        /// <summary>
        /// 断开ACS控制器连接
        /// </summary>
        public void CloseComm()
        {
            try
            {
                Ch.CloseComm();
                bConnected = false;
            }
            catch (Exception ex)
            {
                Status.statusMsg.Add(ex.Message);
            }
        }
        public void ClosePMComm()
        {
            try
            {
                _systemCommand.DisconnectFromPMC();//连接到指定IP
                bPMConnected = false;
            }
            catch(Exception ex)
            {
                Status.statusMsg.Add(ex.Message);
            }
        }
        /// <summary>
        /// 执行打印过程
        /// </summary>
        /// <param name="DataObj">模型数据类对象</param>
        public void Run()
        {
            int isXSEG = 1;
            DataManagement.CmdDataStruct cds;
            try
            {
                //如果还有待发指令，则继续添加指令：
                if (DataObj.CmdQueue.Count > 0 && !GV.bPausePrint)
                {
                    cds = DataObj.GetCmdData();
                    double realTime = DataObj.stopwatch.ElapsedMilliseconds * 0.001;//实时时间戳
                    //PushMessage(cds.CmdName + " " + cds.Para1 + " " + cds.Para2 + " " + cds.Para3 + "\r\n");
                    //cds = DataObj.CmdQueue.Dequeue();
                    switch (cds.CmdName)
                    {
                        case DataManagement.OptType.NoOpt:
                            {
                                break;
                            }
                        case DataManagement.OptType.Home:
                            {
                                int axis = Convert.ToInt16(cds.Para1);
                                Ch.RunBuffer(axis); //执行控制器回零
                                break;
                            }
                        case DataManagement.OptType.Jog:
                            {
                                int axis = Convert.ToInt32(cds.Para1);
                                double speed = Convert.ToDouble(cds.Para2);
                                Ch.Jog(Ch.ACSC_AMF_VELOCITY, axis, speed);
                                break;
                            }
                        case DataManagement.OptType.Pause:
                            {
                                int stoptime = Convert.ToInt16(cds.Para1);
                                Ch.WaitMotionEnd(0, 3000000);
                                Thread.Sleep(stoptime);
                                //Ch.SetVelocityImm(0, 0);
                                //Ch.SetVelocityImm(1, 0);
                                //Ch.SetVelocityImm(2, 0);
                                break;
                            }
                        case DataManagement.OptType.EnableMotor:
                            {
                                int axis = Convert.ToInt32(cds.Para1);
                                if (bConnected)
                                    Ch.Enable(axis);
                                break;
                            }
                        case DataManagement.OptType.DisableMotor:
                            {
                                int axis = Convert.ToInt32(cds.Para1);
                                if (bConnected)
                                    Ch.Disable(axis);
                                break;
                            }

                        case DataManagement.OptType.MoveXYTo:
                            {
                                double xPos = Convert.ToDouble(cds.Para1);
                                double yPos = Convert.ToDouble(cds.Para2);
                                double speed = Convert.ToDouble(cds.Para3);

                                int[] Axes = new int[3];
                                Axes[0] = GV.X;
                                Axes[1] = GV.Y;
                                Axes[2] = -1;
                                double[] Points = new double[2];
                                Points[0] = xPos + nozzleDX; // 双喷头偏移量
                                Points[1] = yPos + nozzleDY; // 双喷头偏移量

                                Controlcommand.Add(realTime.ToString("F3") + "," + cds.Para10);
                                Ch.ExtToPointM(Ch.ACSC_AMF_VELOCITY, Axes, Points, speed, speed);
                                break;
                            }
                        case DataManagement.OptType.MoveAxisTo:
                            {
                                int axis = Convert.ToInt32(cds.Para1);
                                double pos = Convert.ToDouble(cds.Para2);
                                double speed = Convert.ToDouble(cds.Para3);
                                Controlcommand.Add(realTime.ToString("F3") + "," + cds.Para10);
                                switch (axis)
                                {
                                    case GV.X:
                                        pos += nozzleDX; // 双喷头偏移量
                                        break;
                                    case GV.Y:
                                        pos += nozzleDY; // 双喷头偏移量
                                        break;
                                    case GV.Z:
                                        pos += nozzleDZ; // 双喷头偏移量
                                        break;
                                }
                                Ch.ExtToPoint(Ch.ACSC_AMF_VELOCITY, axis, pos, speed, 0);
                                break;
                            }
                        case DataManagement.OptType.MoveAxisRelative:
                            {
                                int axis = Convert.ToInt32(cds.Para1);
                                double distance = Convert.ToDouble(cds.Para2); // 双喷头相对位移无需偏移
                                double speed = Convert.ToDouble(cds.Para3);
                                double layer = Convert.ToDouble(cds.Para9);
                                string strGcode = cds.Para10;

                                MoveAxisRelative(axis, distance, speed, layer, strGcode);
                                //Ch.ExtToPoint(Ch.ACSC_AMF_VELOCITY + Ch.ACSC_AMF_RELATIVE, axis, distance, speed, 0);
                                break;
                            }
                        case DataManagement.OptType.AdjustMicroMotor:
                            {
                                int axis = Convert.ToInt32(cds.Para1);
                                double pos = Convert.ToDouble(cds.Para2);
                                string strGcode = cds.Para10;
                                Ch.ExtToPoint(Ch.ACSC_AMF_VELOCITY, axis, pos, 3, 0);
                                break;
                            }
                        case DataManagement.OptType.Extrude:
                            {
                                int port = Convert.ToInt16(cds.Para1);
                                int value = Convert.ToInt16(cds.Para2);
                                if (value != 0 && cds.Para9 != null)
                                {
                                    double rot = Convert.ToDouble(cds.Para9);
                                    GV.speedRotaryValueA = rot;
                                    GV.frmRotaryValveCtrl.SetPV(rot);
                                }
                                //GV.Isextrude = Convert.ToBoolean(value);
                                Controlcommand.Add(realTime.ToString("F3") + "," + cds.Para10);//日志
                                SetExtrudePorts(port, value);
                                //GV.frmRotaryValveCtrl.SendSpeedSetCmd();
                                break;
                            }
                        case DataManagement.OptType.HighVoltage:
                            {
                                int value = Convert.ToInt16(cds.Para1);
                                Ch.SetOutput(IsSimulator ? 0 : 1, 1, value);
                                break;
                            }

                        case DataManagement.OptType.SetHighVoltageValue:
                            {
                                int valueOfVoltage = Convert.ToInt16(cds.Para1);
                                Ch.SetAnalogOutput(0, valueOfVoltage);
                                break;
                            }

                        case DataManagement.OptType.AdjustNeedle:
                            {
                                break;
                            }

                        case DataManagement.OptType.WaitMoveEnd:
                            {
                                double realTime1 = DataObj.stopwatch.ElapsedMilliseconds * 0.001;
                                WaitMoveEnd();
                                double realTime2 = DataObj.stopwatch.ElapsedMilliseconds * 0.001;
                                double k_Estimate_Real = realTime2 / DataObj.segEndEstimateTime;
                                double time = realTime2 - realTime1;
                                DataObj.totalRealTimeEst = k_Estimate_Real * DataObj.totalEstimateTime;
                                Controlcommand.Add(realTime.ToString("F3") + "," + cds.Para10 + "," + time.ToString("F3"));
                                break;
                            }

                        case DataManagement.OptType.TakePhoto:
                            {
                                double xNew, yNew, dz;
                                xNew = Convert.ToDouble(cds.Para1);
                                yNew = Convert.ToDouble(cds.Para2);
                                dz = Convert.ToDouble(cds.Para3);

                                // 提升Z轴（摄像头）
                                Ch.ExtToPoint(Ch.ACSC_AMF_VELOCITY + Ch.ACSC_AMF_RELATIVE, GV.Z, -dz, 20, 0);
                                // 等待停稳
                                Ch.WaitMotionEnd(GV.Z, 3000000);
                                // 记录当前位置
                                double xOld = GV.PrintingObj.Ch.GetFPosition(GV.X);
                                double yOld = GV.PrintingObj.Ch.GetFPosition(GV.Y);

                                int[] Axes = new int[3];
                                Axes[0] = GV.X;
                                Axes[1] = GV.Y;
                                Axes[2] = -1;

                                double[] Points = new double[2];
                                Points[0] = xNew;
                                Points[1] = yNew;

                                // 平移到拍照位置（样品中间）
                                Ch.ExtToPointM(Ch.ACSC_AMF_VELOCITY, Axes, Points, 20, 20);
                                // 等待停稳
                                Ch.WaitMotionEnd(GV.X, 3000000);
                                Ch.WaitMotionEnd(GV.Y, 3000000);
                                Thread.Sleep(1000);

                                // 启动拍照
                                bPhotoTaken = false;
                                photoName = cds.Para4;
                                //开始出丝
                                SetExtrudePorts(0, 1);
                                //固定出丝60s
                                Thread.Sleep(6 * 1000);
                                SetExtrudePorts(0, 0);
                                // 拍完后等待,10秒手抄称重
                                Thread.Sleep(10 * 1000);

                                Points[0] = xOld;
                                Points[1] = yOld;
                                // 平移到拍照位置（样品中间）
                                Ch.ExtToPointM(Ch.ACSC_AMF_VELOCITY, Axes, Points, 20, 20);
                                // 等待停稳
                                Ch.WaitMotionEnd(GV.X, 3000000);
                                Ch.WaitMotionEnd(GV.Y, 3000000);
                                // 降低Z轴（摄像头）
                                Ch.ExtToPoint(Ch.ACSC_AMF_VELOCITY + Ch.ACSC_AMF_RELATIVE, GV.Z, dz, 10, 0);
                                Ch.WaitMotionEnd(GV.Z, 3000000);
                                break;
                            }
                        case DataManagement.OptType.SwitchNozzle:
                            int nozzleID = Convert.ToInt32(cds.Para1);
                            SwitchNozzle(nozzleID);
                            break;
                        case DataManagement.OptType.CleanNozzle:
                            {
                                double xNew, yNew, zNew;
                                double xOld, yOld, zOld;
                                // 清洁位置
                                xNew = Convert.ToDouble(cds.Para1);
                                yNew = Convert.ToDouble(cds.Para2);
                                zNew = Convert.ToDouble(cds.Para3);
                                // 等待停稳
                                Ch.WaitMotionEnd(GV.Z, 3000000);
                                // 记录当前位置，记录为打印完一个样品/某几层的结束点
                                xOld = GV.PrintingObj.Ch.GetFPosition(GV.X);
                                yOld = GV.PrintingObj.Ch.GetFPosition(GV.Y);
                                zOld = GV.PrintingObj.Ch.GetFPosition(GV.Z);
                                // 提升主Z轴
                                Ch.ExtToPoint(Ch.ACSC_AMF_VELOCITY, GV.Z, 0, 20, 0);
                                // 提升小Z轴

                                int[] Axes = new int[3];
                                Axes[0] = GV.X;
                                Axes[1] = GV.Y;
                                Axes[2] = -1;

                                double[] Points = new double[2];
                                Points[0] = xNew;
                                Points[1] = yNew;

                                // 平移到清洁位置上方
                                Ch.ExtToPointM(Ch.ACSC_AMF_VELOCITY, Axes, Points, 20, 20);
                                // 等待停稳
                                Ch.WaitMotionEnd(GV.X, 3000000);
                                Ch.WaitMotionEnd(GV.Y, 3000000);

                                // 下降到清洁位置位置
                                Ch.ExtToPoint(Ch.ACSC_AMF_VELOCITY, GV.Z, zNew, 20, 20);

                                // 启动清洁
                                //Extrude(3, 1);
                                //Thread.Sleep(5000);
                                //Extrude(3, 0);

                                //来回运动，进行毛刷清扫
                                for (int i = 0; i < 6; i++)
                                {
                                    double dis = 5;
                                    Points[0] = xNew + (i % 2 == 0 ? -1 : 1) * dis;
                                    Points[1] = yNew;
                                    Ch.ExtToPointM(Ch.ACSC_AMF_VELOCITY, Axes, Points, 10, 10);
                                }
                                // 等待停稳
                                Ch.WaitMotionEnd(GV.X, 3000000);
                                Ch.WaitMotionEnd(GV.Y, 3000000);

                                // 移动回初始原点
                                Points[0] = xOld;
                                Points[1] = yOld;
                                Ch.ExtToPointM(Ch.ACSC_AMF_VELOCITY, Axes, Points, 20, 20);

                                // 等待停稳
                                Ch.WaitMotionEnd(GV.X, 3000000);
                                Ch.WaitMotionEnd(GV.Y, 3000000);
                                // 降低Z轴，准备继续打印
                                Ch.ExtToPoint(Ch.ACSC_AMF_VELOCITY + Ch.ACSC_AMF_RELATIVE, GV.Z, zOld, 10, 0);
                                Ch.WaitMotionEnd(GV.Z, 3000000);
                            }
                            break;
                        case DataManagement.OptType.BasalTransPort:
                            {
                                //动作规划：z轴回零，小z回零，y移动到最远端
                                double xNew, yNew, zNew;
                                double xOld, yOld, zOld;

                                // 等待停稳
                                Ch.WaitMotionEnd(GV.Z, 3000000);
                                // 记录当前位置，记录为打印完一个样品/某几层的结束点
                                xOld = GV.PrintingObj.Ch.GetFPosition(GV.X);
                                yOld = GV.PrintingObj.Ch.GetFPosition(GV.Y);
                                zOld = GV.PrintingObj.Ch.GetFPosition(GV.Z);
                                // 提升主Z轴、小Z轴
                                Ch.ExtToPoint(Ch.ACSC_AMF_VELOCITY, GV.Z, 0, 20, 0);

                                // 等待停稳
                                Ch.WaitMotionEnd(GV.Z, 3000000);
                                // 腾空龙门
                                xNew = Convert.ToDouble(cds.Para1);
                                yNew = Convert.ToDouble(cds.Para2);
                                zNew = Convert.ToDouble(cds.Para3);
                                int waitingTime = Convert.ToInt32(cds.Para10);

                                int[] Axes = new int[3];
                                Axes[0] = GV.X;
                                Axes[1] = GV.Y;
                                Axes[2] = -1;

                                double[] Points = new double[2];
                                Points[0] = xNew;
                                Points[1] = yNew;
                                // 平移
                                Ch.ExtToPointM(Ch.ACSC_AMF_VELOCITY, Axes, Points, 20, 20);

                                // 等待停稳
                                Ch.WaitMotionEnd(GV.X, 3000000);
                                Ch.WaitMotionEnd(GV.Y, 3000000);

                                //等待转运结束信号
                                //while( isTransPorting)
                                {
                                    Thread.Sleep(waitingTime);
                                }
                                //蜂鸣器提醒
                                Ch.SetOutput(0, 11, 1);
                                Thread.Sleep(500);
                                Ch.SetOutput(0, 11, 0);

                                //// 移动回初始原点
                                //Points[0] = xOld;
                                //Points[1] = yOld;
                                //Ch.ExtToPointM(Ch.ACSC_AMF_VELOCITY, Axes, Points, 20, 20);

                                //// 等待停稳
                                //Ch.WaitMotionEnd(GV.X, 3000000);
                                //Ch.WaitMotionEnd(GV.Y, 3000000);
                                //// 降低Z轴，准备继续打印
                                //Ch.ExtToPoint(Ch.ACSC_AMF_VELOCITY + Ch.ACSC_AMF_RELATIVE, GV.Z, zOld, 10, 0);
                                //Ch.WaitMotionEnd(GV.Z, 3000000);
                            }
                            break;

                        case DataManagement.OptType.SegmentArc1:
                            {
                                CheckGSFree();
                                int[] Axes = new int[3];//Para1
                                double[] Center = new double[2];//Para2
                                double[] FinalPoint = new double[2];//Para3

                                string[] strAxes = cds.Para1.Split(); //Para1: Axes
                                Axes[0] = Convert.ToInt32(strAxes[0]); //Axes[0]
                                Axes[1] = Convert.ToInt32(strAxes[1]); //Axes[1]
                                Axes[2] = Convert.ToInt32(strAxes[2]); //Axes[2]

                                string[] strCenter = cds.Para2.Split(); //Para2: Center
                                Center[0] = Convert.ToDouble(strCenter[0]); //Center[0]
                                Center[1] = Convert.ToDouble(strCenter[1]); //Center[1]

                                string[] strFinalPoint = cds.Para3.Split(); //Para3: FinalPoint
                                FinalPoint[0] = Convert.ToDouble(strFinalPoint[0]); //FinalPoint[0]
                                FinalPoint[1] = Convert.ToDouble(strFinalPoint[1]); //FinalPoint[1]

                                int Rotation = Convert.ToInt32(cds.Para4);  //Para4: Rotation
                                double Velocity = Convert.ToDouble(cds.Para5);//Para5: Velocity
                                double EndVelocity = Convert.ToDouble(cds.Para6);//Para6: EndVelocity

                                Controlcommand.Add(realTime.ToString("F3") + "," + cds.Para10);

                                // 双喷头偏移量处理：
                                for (int i = 0; i < Axes.Length - 1; i++)
                                {
                                    switch (Axes[i])
                                    {
                                        case GV.X:
                                            Center[i] += nozzleDX;
                                            FinalPoint[i] += nozzleDX;
                                            break;
                                        case GV.Y:
                                            Center[i] += nozzleDY;
                                            FinalPoint[i] += nozzleDY;
                                            break;
                                        case GV.Z:
                                            Center[i] += nozzleDZ;
                                            FinalPoint[i] += nozzleDZ;
                                            break;
                                    }
                                }
                                Ch.SegmentArc1(0, Axes, Center, FinalPoint, Rotation, Velocity, EndVelocity, null, null, Ch.ACSC_NONE, null);
                                break;
                            }

                        case DataManagement.OptType.SegmentArc2:
                            {
                                CheckGSFree();

                                string[] strAxes = cds.Para1.Split();  //Para1: Axes
                                int[] Axes = new int[strAxes.Length];
                                for (int i = 0; i < strAxes.Length; i++)
                                {
                                    Axes[i] = Convert.ToInt32(strAxes[i]);
                                }

                                string[] strFinalPoint = cds.Para2.Split(); //Para2: FinalPoint
                                double[] centerPoint = new double[strFinalPoint.Length];
                                for (int i = 0; i < centerPoint.Length; i++)
                                {
                                    centerPoint[i] = Convert.ToDouble(strFinalPoint[i]); //FinalPoint[0]
                                }

                                double Angle = Convert.ToDouble(cds.Para3); //Para3: Angle
                                double Velocity = Convert.ToDouble(cds.Para4); //Para4: Velocity
                                double EndVelocity = Convert.ToDouble(cds.Para5); //Para5: EndVelocity

                                Controlcommand.Add(realTime.ToString("F3") + "," + cds.Para10);

                                // 双喷头偏移量处理：
                                for (int i = 0; i < Axes.Length - 1; i++)
                                {
                                    switch (Axes[i])
                                    {
                                        case GV.X:
                                            centerPoint[i] += nozzleDX;
                                            break;
                                        case GV.Y:
                                            centerPoint[i] += nozzleDY;
                                            break;
                                        case GV.Z:
                                            centerPoint[i] += nozzleDZ;
                                            break;
                                    }
                                }
                                Ch.SegmentArc2(0, Axes, centerPoint, Angle, Velocity, EndVelocity, null, null, Ch.ACSC_NONE, null);
                                break;
                            }

                        case DataManagement.OptType.SegmentLine:
                            {
                                CheckGSFree();

                                string[] strAxes = cds.Para1.Split();  //Para1: Axes
                                int[] Axes = new int[strAxes.Length];
                                for (int i = 0; i < strAxes.Length; i++)
                                {
                                    Axes[i] = Convert.ToInt32(strAxes[i]);
                                }

                                string[] strFinalPoint = cds.Para2.Split(); //Para2: FinalPoint
                                double[] FinalPoint = new double[strFinalPoint.Length];
                                for (int i = 0; i < FinalPoint.Length; i++)
                                {
                                    FinalPoint[i] = Convert.ToDouble(strFinalPoint[i]); //FinalPoint[0]
                                }

                                double Velocity = Convert.ToDouble(cds.Para3);  //Para3: Velocity
                                double EndVelocity = Convert.ToDouble(cds.Para4);  //Para4: EndVelocity

                                Controlcommand.Add(realTime.ToString("F3") + "," + cds.Para10);

                                // 双喷头偏移量处理：
                                for (int i = 0; i < Axes.Length - 1; i++)
                                {
                                    switch (Axes[i])
                                    {
                                        case GV.X:
                                            FinalPoint[i] += nozzleDX;
                                            break;
                                        case GV.Y:
                                            FinalPoint[i] += nozzleDY;
                                            break;
                                        case GV.Z:
                                            FinalPoint[i] += nozzleDZ;
                                            break;
                                    }
                                }

                                //changed
                                //20180717 XJW
                                //在SEG命令时候不能使用SEGLine中，Flag应设置为0
                                //而在XSEG命令中，FLag才能被设置
                                if (isXSEG == -1)
                                {
                                    Ch.SegmentLine(Ch.ACSC_AMF_VELOCITY + Ch.ACSC_AMF_ENDVELOCITY, Axes, FinalPoint, Velocity, EndVelocity, null, null, Ch.ACSC_NONE, null);
                                }
                                else
                                {
                                    Ch.SegmentLine(0, Axes, FinalPoint, Velocity, EndVelocity, null, null, Ch.ACSC_NONE, null);
                                }
                                //end

                                break;
                            }
                        case DataManagement.OptType.SegmentedMotion:
                            {
                                isXSEG = 1;
                                string[] strAxes = cds.Para1.Split();  //Para1: Axes
                                int[] Axes = new int[strAxes.Length];
                                for (int i = 0; i < strAxes.Length; i++)
                                {
                                    Axes[i] = Convert.ToInt32(strAxes[i]);
                                }

                                string[] strCenter = cds.Para2.Split(); //Para2: Center
                                double[] Center = new double[strCenter.Length];
                                for (int i = 0; i < strCenter.Length; i++)
                                {
                                    Center[i] = Convert.ToDouble(strCenter[i]);
                                }

                                Controlcommand.Add(realTime.ToString("F3") + "," + cds.Para10);//时间戳

                                //Ch.SegmentedMotion(0, Axes, Center);
                                int optNow = Ch.ACSC_AMF_VELOCITY;

                                // 双喷头偏移量处理：
                                for (int i = 0; i < Axes.Length - 1; i++)
                                {
                                    switch (Axes[i])
                                    {
                                        case GV.X:
                                            Center[i] += nozzleDX;
                                            break;
                                        case GV.Y:
                                            Center[i] += nozzleDY;
                                            break;
                                        case GV.Z:
                                            Center[i] += nozzleDZ;
                                            break;
                                    }
                                }
                                // 在当前位置建立一段SegmentedMotion
                                Ch.SegmentedMotion(optNow, Axes, Center);
                                IsCancelled = false;
                                break;
                            }

                        case DataManagement.OptType.ExtSegmentedMotion:
                            {
                                isXSEG = -1;
                                string[] strAxes = cds.Para1.Split();  //Para1: Axes
                                int[] Axes = new int[strAxes.Length];
                                for (int i = 0; i < strAxes.Length; i++)
                                {
                                    Axes[i] = Convert.ToInt32(strAxes[i]);
                                }

                                string[] strStartPoint = cds.Para2.Split(); //Para2: Center
                                double[] startPoint = new double[strStartPoint.Length];
                                for (int i = 0; i < strStartPoint.Length; i++)
                                {
                                    startPoint[i] = Convert.ToDouble(strStartPoint[i]);
                                }

                                Controlcommand.Add(realTime.ToString("F3") + "," + cds.Para10);

                                double velocity = Convert.ToDouble(cds.Para3);
                                double endVelocity = Convert.ToDouble(cds.Para4);
                                double juncVelocity = Convert.ToDouble(cds.Para5);
                                double Angle = Convert.ToDouble(cds.Para6);

                                // 双喷头偏移量处理：
                                for (int i = 0; i < Axes.Length - 1; i++)
                                {
                                    switch (Axes[i])
                                    {
                                        case GV.X:
                                            startPoint[i] += nozzleDX;
                                            break;
                                        case GV.Y:
                                            startPoint[i] += nozzleDY;
                                            break;
                                        case GV.Z:
                                            startPoint[i] += nozzleDZ;
                                            break;
                                    }
                                }

                                Ch.ExtendedSegmentedMotion(Ch.ACSC_AMF_VELOCITY + Ch.ACSC_AMF_ENDVELOCITY, Axes, startPoint, velocity, endVelocity, juncVelocity, Angle, 0, null);
                                // 在当前位置建立一段ExtendedSegmentedMotion

                                IsCancelled = false;
                                break;
                            }
                        case DataManagement.OptType.EndSequenceM:
                            {
                                string[] strAxes = cds.Para1.Split();  //Para1: Axes
                                int[] Axes = new int[strAxes.Length];
                                for (int i = 0; i < strAxes.Length; i++)
                                {
                                    Axes[i] = Convert.ToInt32(strAxes[i]);
                                }

                                Controlcommand.Add(realTime.ToString("F3") + "," + cds.Para10);

                                Ch.EndSequenceM(Axes);

                                break;
                            }

                        case DataManagement.OptType.SegmentStopper:
                            {
                                CheckGSFree();
                                int[] Axes = new int[3];

                                string[] strAxes = cds.Para1.Split();  //Para1: Axes
                                Axes[0] = Convert.ToInt32(strAxes[0]); //Axes[0]
                                Axes[1] = Convert.ToInt32(strAxes[1]); //Axes[1]
                                Axes[2] = Convert.ToInt32(strAxes[2]); //Axes[2]

                                Ch.Stopper(Axes);

                                break;
                            }

                        case DataManagement.OptType.DisplayInfo:
                            string strInfoName = cds.Para1;
                            string strInfoText = cds.Para2;
                            string strInfoColor = cds.Para3;
                            double Nowlayer = Convert.ToInt32(cds.Para10);
                            if (Nowlayer != -1)
                            {
                                Controlcommand.Add(";LAYER:" + cds.Para10);
                            }
                            if (strInfoName == "layerCurrent")
                            {
                                Status.layerCurrent = strInfoText;
                            }
                            else if (strInfoName == "filamentNumber")
                            {
                                Status.filamentNumber = strInfoText;
                            }
                            else if (strInfoName == "Notice")
                            {
                                NoticeInfo = strInfoText;    
                                InfoColor = strInfoColor;//文字颜色
                            }
                            else if (strInfoName == "Gcode")
                            {
                                Status.gcodeCommands = strInfoText;
                            }
                            else if (strInfoName == "DetectPercent")
                            {
                                detectPercent = Convert.ToInt32(strInfoText);
                            }
                            break;

                        case DataManagement.OptType.EndPrinting:
                            try
                            {
                                DataObj.ResetCmdCounter();
                                DataObj.stopwatch.Stop();

                                Controlcommand.Add(realTime.ToString("F3") + "," + cds.Para10);
                                IsPrinting = false;
                                GV.bgWorker2.ReportProgress(100);//结束打印，进度记录100
                            }
                            catch (Exception ex)
                            {
                                PushMessage(ex.Message);
                            }
                            break;
                        case DataManagement.OptType.SetPrintPosStartEnd://计数样品序号，A-B
                            {
                                bool bStartA = Convert.ToBoolean(cds.Para1);
                                bool bStartB = Convert.ToBoolean(cds.Para2);
                                int iPrintPos = Convert.ToInt32(cds.Para3);//正在打印的序号
                                if (bStartA || bStartB)
                                {
                                    GV.indexPrintingPos = iPrintPos;
                                    GV.arrStartTime[iPrintPos] = GV.dataManagementObj.PrintingElapsedMilliseconds(); //已打印用时
                                    GV.arrStartTimeStamp[iPrintPos] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    GV.strPrintPosStatus = "A: ";
                                    // 1√,2○,3,4
                                    for (int i = 0; i < GV.listPrintPosA.Count; i++)
                                    {
                                        if (GV.listPrintPosA[i])
                                        {
                                            if (i < iPrintPos) // 已打印完成样品序号
                                            {
                                                GV.strPrintPosStatus += (i + 1) + "√,";
                                            }
                                            else if (i == iPrintPos) // 正在打印序号
                                            {
                                                GV.strPrintPosStatus += (i + 1) + "○,";
                                            }
                                            else
                                            {
                                                GV.strPrintPosStatus += (i + 1) + ",";
                                            }
                                        }
                                    }
                                    GV.strPrintPosStatus += "\r\n  B: ";
                                    for (int j = 0; j < GV.listPrintPosB.Count; j++)
                                    {
                                        if (GV.listPrintPosB[j])
                                        {
                                            if (j < iPrintPos) // 已打印完成样品序号
                                            {
                                                GV.strPrintPosStatus += (j + 1) + "√,";
                                            }
                                            else if (j == iPrintPos) // 正在打印序号
                                            {
                                                GV.strPrintPosStatus += (j + 1) + "○,";
                                            }
                                            else
                                            {
                                                GV.strPrintPosStatus += (j + 1) + ",";
                                            }
                                        }
                                    }
                                }
                                else // 索引为iPrintPos的工位打印结束
                                {
                                    GV.arrEndTime[iPrintPos] = GV.dataManagementObj.PrintingElapsedMilliseconds(); //已打印用时
                                    //GV.frmPrintStep4.WriteTempLog(iPrintPos);
                                }
                                break;

                            }
                        case DataManagement.OptType.RecordDisplacement://31，记录测量数据
                            Ch.WaitMotionEnd(GV.Z, 3000);
                            if (!GV.PrintingObj.IsSimulator) Thread.Sleep(6000);// CYQ 20240607
                            double x1 = Status.fPosX;
                            double y1 = Status.fPosY;
                            double z1 = Status.fPosZ;
                            double valuePen = 0;
                            int countFail = 0;
                            while (countFail < 3 && (valuePen <= 0 || valuePen > 2.5))
                            {
                                valuePen = GV.valueDisplacementSensor_rA; //GV.frmSetPressure.GetPenDisplacement();
                                if (GV.PrintingObj.IsSimulator) valuePen = 0.85 + 0.1 * GV.rand.NextDouble();// CYQ 20240504
                                Thread.Sleep(500);
                                countFail++;
                            }
                            Ch.WaitMotionEnd(GV.Z, 3000);
                            if (countFail >= 3)
                            {
                                DataObj.CmdQueue.Clear();
                            }
                            else
                            {
                                string strLine = x1.ToString() + "," + y1.ToString() + "," + z1.ToString() + "," + valuePen.ToString("0.0000");
                                listDisplacementLiDanRecord.Add(strLine);
                                listDistanceRecordValue.Add(valuePen);
                                //NoticeInfo = "No." + listDisplacementPenRecord.Count + ": " + valuePen.ToString("0.0000");
                            }
                            break;
                        case DataManagement.OptType.AdjustPMmotor:
                            int xbotId = Convert.ToInt32(cds.Para1.ToString());
                            int movetype = Convert.ToInt32(cds.Para2.ToString());
                            int moveAxis = Convert.ToInt32(cds.Para3.ToString());
                            double movePara = Convert.ToDouble(cds.Para4.ToString());
                            switch (movetype)
                            {
                                case 1://z调节                                 
                                    double pos = movePara;
                                    double speed = 3;
                                    MoveAxisRelative(moveAxis, pos, speed);
                                    break;
                                case 2://平面电机转动

                                    MoveXbotXYrotary(xbotId, moveAxis, movePara, 5);
                                    break;
                            }

                            break;
                    }

                    //Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                PushMessage(ex.Message);
            }
        }

        public void WaitMoveEnd()
        {
            Ch.WaitMotionEnd(GV.X, 3000000);
            Ch.WaitMotionEnd(GV.Y, 3000000);
            Ch.WaitMotionEnd(GV.Z, 3000000);
        }
        //检查缓冲区余量
        public void CheckGSFree()
        {
            int gsfreeCount = GetGSFree();
            //如果控制器缓冲指令空间不足，则暂停添加指令：
            while (gsfreeCount > 0 && gsfreeCount <= 10 && !IsCancelled)
            {
                Thread.Sleep(GV.Command_Block);
                gsfreeCount = GetGSFree();
            }
        }
        //检查结束
        public void CheckGRtime()
        {
            double grTime = Convert.ToDouble(GetGRTime());
            while (grTime > 0 && grTime <= 500 && !IsCancelled)
            {
                //小于设定值则关闭出气

            }
        }
        /// <summary>
        /// 设置出丝IO端口开关
        /// </summary>
        /// <param name="port">IO端口号：大于等于0表示设定对应的某个IO端口，小于0表示设置由outPort指定的全部端口</param>
        /// <param name="value">0：关闭；1：打开</param>
        public void SetExtrudePorts(int port, int value)
        {
            if (port >= 0 && port <= 1)
            {
                SetAlarmPort(2, value);    // 2#三色灯端口，用于显示出丝状态                
            }
            if (port >= 0)
            {
                Extrude(port, value);
            }
            else if (port < 0) // 当port<0 表示使用outPorts数组列明的端口号控制开关
            {
                for (int i = 0; i < extruderOutports.Length; i++)
                {
                    if (extruderOutports[i] >= 0)
                    {
                        Extrude(extruderOutports[i], value);
                        // Ch.SetOutput(0, extruderOutports[i], value);
                    }
                }
            }
        }

        public double nozzleDX = 0, nozzleDY = 0, nozzleDZ = 0;

        public void ClearNozzleDXYZ_AB()
        {
            nozzleDX = 0;
            nozzleDY = 0;
            nozzleDZ = 0;
        }

        public void SwitchNozzle(int nozzleNum)
        {
            switch (nozzleNum)
            {
                case 0:
                case -2: // 所有喷头同时启动时使用A喷头
                    if (Status.nozzleID == 1) // 当前为喷头B，将切换为喷头A
                    {
                        Extrude(2, 0); // 喷头B气动提升
                        MoveAxisRelative(GV.X, -GV.dX_AB, 30);
                        MoveAxisRelative(GV.Y, -GV.dY_AB, 30);// 双喷头相对位移无需偏移
                        WaitMoveEnd();
                        MoveAxisRelative(GV.Z, -GV.dZ_AB, 10);
                        WaitMoveEnd();
                        nozzleDX = 0;
                        nozzleDY = 0;
                        nozzleDZ = 0;
                    }
                    break;
                case 1:
                    if (Status.nozzleID == 0) // 当前为喷头A，将切换为喷头B
                    {
                        MoveAxisRelative(GV.Z, GV.dZ_AB - 5, 10); // Z轴留5mm余量，给喷头B气动下降留缓冲余地。
                        WaitMoveEnd();
                        MoveAxisRelative(GV.X, GV.dX_AB, 30);
                        MoveAxisRelative(GV.Y, GV.dY_AB, 30);
                        WaitMoveEnd();
                        Extrude(2, 1);  // 喷头B气动下降
                        Pause(1000);    // 等待喷头气动降下停稳
                        MoveAxisRelative(GV.Z, 5, 10); // Z轴在缓冲区继续下降
                        WaitMoveEnd();
                        nozzleDX = GV.dX_AB;
                        nozzleDY = GV.dY_AB;
                        nozzleDZ = GV.dZ_AB;
                    }
                    break;
            }
        }

        /// <summary>
        /// 暂停打印
        /// </summary>
        public void Pause(int stoptime)
        {
            Thread.Sleep(stoptime);
        }

        public void MoveAxisRelative(int axis, double distance, double speed, double layer = -1, string strGcode = "")
        {
            Ch.ExtToPoint(Ch.ACSC_AMF_VELOCITY + Ch.ACSC_AMF_RELATIVE, axis, distance, speed, 0);
        }
        public void AdjustMicroMotor(int axis, double pos)
        {

        }

        enum ExtrudePort
        {
            NozzleA = 0,
            NozzleB = 1,
            NozzleExchange = 2,
            NozzleClean = 3
        }

        public void SetAlarmPort(int port, int value)
        {
            Ch.SetOutput(0, port + 8, value);
        }

        /// <summary>
        /// 设定出丝组数字输出端口状态
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="value">开关状态: 1-开启; 0-关闭</param>
        public void Extrude(int port, int value)
        {
            switch (port)
            {
                case 0: // 喷头1出丝
                    GV.Isextrude = Convert.ToBoolean(value);//挤出out口状态赋值
                    // Ch.SetOutput(1, 11, value);
                    //xlm
                    //Ch.SetOutput(0, 10, value);//气压
                    //dlm
                    Ch.SetOutput(1, 1, value);
                    GV.frmRotaryValveCtrl.MotorRun(value);
                    break;
                case 1: // 喷头2出丝
                    //xlm
                    //Ch.SetOutput(0, 9, value);                  
                    //dlm
                    Ch.SetOutput(1, 2, value);
                    break;
                case 2: // 喷头切换
                    Ch.SetOutput(1, 8, value);
                    break;
                case 3: // 喷头清洗
                    Ch.SetOutput(1, 9, value);
                    break;
                case 4://螺杆打印，气压持续供给，螺杆控制开关
                    if (value == 0 && !IsPrinting)
                    {
                        GV.Isextrude = false;
                        Ch.SetOutput(0, 8, 1);
                        GV.frmRotaryValveCtrl.SendZeroSpeed();
                    }
                    else if (value == 0 && IsPrinting)
                    {
                        GV.Isextrude = true;
                        Ch.SetOutput(0, 8, 0);
                        GV.frmRotaryValveCtrl.SendZeroSpeed();
                    }
                    else if (value == 1)
                    {
                        GV.Isextrude = true;
                        Ch.SetOutput(0, 8, 1);
                        GV.frmRotaryValveCtrl.MotorRun(value);
                    }
                    break;
                case 5://蜂鸣器
                    Ch.SetOutput(0, 11, value);//noise
                    Ch.SetOutput(0, 8, value);//red light
                    break;
                default:
                    break;
            }
        }

        public int GetExtrudePort(ExtrudePortType portType)
        {
            return GetExtrudePort((int)portType);
        }
        /// <summary>
        /// 获取出丝组数字输出端口状态
        /// </summary>
        /// <param name="port">端口号</param>
        /// <returns>开关状态: 1-开启; 0-关闭</returns>
        public int GetExtrudePort(int port)
        {
            int value;
            switch (port)
            {
                case 0: // 喷头1出丝
                    value = Ch.GetOutput(1, 1);
                    break;
                case 1: // 喷头2出丝
                    value = Ch.GetOutput(1, 2);
                    break;
                case 2: // 喷头切换
                    value = Ch.GetOutput(0, 8);
                    break;
                case 3: // 喷头清洗
                    value = Ch.GetOutput(1, 9);
                    break;
                default:
                    value = -1;
                    break;
            }
            return value;
        }

        public void RunNozzleCalibrate(int times = 1)
        {
            if (times == 1)
            {
                RunBuffer(6, "FirstCalibrate");
                //Thread.Sleep(500);
                //RunBuffer(6, "NozzleCalibrate");
            }
            else if (times == 2)
            {
                RunBuffer(6, "SecondCalibrate");
                //Thread.Sleep(500);
                //RunBuffer(6, "NozzleCalibrate");
            }
        }

        /// <summary>
        /// 获取针头激光对准中心临界点
        /// </summary>
        /// <returns></returns>
        public double[] GetLaserCenter()
        {
            try
            {
                dynamic AdjustPercent = Ch.ReadVariable("AdjustPercent", 6);
                if (AdjustPercent >= 100)
                {
                    dynamic LaserCenterPos = Ch.ReadVariable("LaserCenterPos", 6);
                    double[] centerPoint = new double[3];
                    centerPoint[0] = (double)LaserCenterPos[0];
                    centerPoint[1] = (double)LaserCenterPos[1];
                    centerPoint[2] = (double)LaserCenterPos[2];
                    return centerPoint;
                }
            }
            catch (Exception ex)
            {
            }
            return null;
        }


        /// <summary>
        /// 查询喷头校准进度
        /// </summary>
        /// <returns></returns>
        public int GetAdjustStatus()
        {
            dynamic adjPercent = GV.PrintingObj.Ch.ReadVariable("AdjustPercent", 6);
            return adjPercent;
        }

        /// <summary>
        /// 设置固化IO端口开关
        /// </summary>
        /// <param name="port">IO端口号：大于等于0表示设定对应的某个IO端口，小于0表示设置由outPort指定的全部端口</param>
        /// <param name="value">0：关闭；1：打开</param>
        public void SetCurePorts(int port, int value)
        {
            if (port > 0)
            {
                Ch.SetOutput(0, port, value);
            }
            else if (port < 0) // 当port<0 表示使用outPorts数组列明的端口号控制开关
            {
                for (int i = 0; i < cureOutports.Length; i++)
                {
                    if (cureOutports[i] >= 0)
                    {
                        Ch.SetOutput(0, cureOutports[i], value);
                    }
                }
            }
        }

        /// <summary>
        /// 推出错误消息
        /// </summary>
        /// <param name="strMsg"></param>
        public void PushMessage(string strMsg, bool withTime = true)
        {
            if (withTime)
                strMsg = DateTime.Now.Minute + "'" + DateTime.Now.Second + "\": "
                         + strMsg;
            Status.statusMsg.Add(strMsg);
        }


        public void WaitMoveEnd(int Axis, int Timeout)
        {
            Ch.WaitMotionEnd(Axis, Timeout);
        }

        /// <summary>
        /// 暂停打印
        /// </summary>
        public void Pause()
        {
            Ch.SetVelocityImm(0, 0);
            Ch.SetVelocityImm(1, 0);
            Ch.SetVelocityImm(2, 0);
        }



        /// <summary>
        /// 恢复打印
        /// </summary>
        public void Resume()
        {

        }

        /// <summary>
        /// 中止打印
        /// </summary>
        public void Stop()
        {
            //Ch.KillAll();
            try
            {
                IsCancelled = true;
                Ch.Halt(GV.X);
                Ch.Halt(GV.Y);
                Ch.Halt(GV.Z);
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 终止指定轴打印
        /// </summary>
        public void Stop(int iAxis)
        {
            try
            {
                //Ch.KillAll();
                IsCancelled = true;
                Ch.Halt(iAxis);
                //Ch.Break(iAxis);
            }
            catch (Exception ex)
            {
            }
        }

        public void Jog(int iAxis, double vSet)
        {
            try
            {
                Ch.Jog(Ch.ACSC_AMF_VELOCITY, iAxis, vSet);
            }
            catch (Exception ex)
            {
            }
        }

        public void StepMove(int axis, int direction, double speed, double distance)
        {
            try
            {
                Ch.WaitMotionEnd(axis, 60000);
                Ch.ExtToPoint(Ch.ACSC_AMF_VELOCITY + Ch.ACSC_AMF_RELATIVE, axis, direction * distance, speed, 0);
            }
            catch (Exception ex)
            {
            }
        }

        //*********************************************************************************//
        //                           排队控制指令 （以q开头）                              //
        //*********************************************************************************//

        /// <summary>
        /// 暂停打印
        /// </summary>
        public void qPause(int stoptime)
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.Pause;
            cds.Para1 = stoptime.ToString();
            cds.EstimateTime = stoptime * 0.001;
            DataObj.InsertCmdData(cds);
        }

        /// <summary>
        /// 按指定速度往指定方向移动
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="direction"></param>
        /// <param name="speed"></param>
        public void qJog(int axis, double speed)
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.Jog;
            cds.Para1 = axis.ToString();
            cds.Para2 = speed.ToString();
            cds.EstimateTime = 0;
            DataObj.InsertCmdData(cds);
        }

        public void qEndPrinting(string str = "")
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.EndPrinting;
            cds.Para10 = str;
            cds.EstimateTime = 0;
            DataObj.InsertCmdData(cds);
        }

        /// <summary>
        /// 按指定速度往指定方向移动指定距离
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="direction"></param>
        /// <param name="speed"></param>
        /// <param name="distance"></param>
        public void qStep(int axis, int direction, double speed, double distance)
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.MoveAxisRelative;
            cds.Para1 = axis.ToString();
            cds.Para2 = (direction * distance).ToString();
            cds.Para3 = speed.ToString();
            cds.EstimateTime = distance / speed;
            DataObj.InsertCmdData(cds);
        }

        public void qWaitMoveEnd(string str = "")
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.WaitMoveEnd;
            cds.Para10 = str;
            cds.EstimateTime = 0;
            DataObj.InsertCmdData(cds);
        }

        public void qWaitMoveEnd(int output, double[] FinalPoint, string str = "")
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.WaitMoveEnd;
            cds.Para1 = "";
            cds.Para10 = str;
            cds.EstimateTime = 0;
            DataObj.InsertCmdData(cds);

            //DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            //cds.CmdName = DataManagement.OptType.WaitMoveEnd;
            //if (output == 0) // 提前停止出丝
            //{
            //    cds.Para1 = "0";
            //}
            //else // 延时开启出丝
            //{
            //    cds.Para1 = "1";
            //}

            //cds.Para2 = FinalPoint[0].ToString() + " " + FinalPoint[1].ToString();
            //for (int i = 2; i < FinalPoint.Length; i++)
            //{
            //    cds.Para2 += (" " + FinalPoint[i].ToString());
            //}

            //cds.EstimateTime = 0;
            //DataObj.InsertCmdData(cds);
        }


        public void qTakePhoto(double x, double y, double z, string photoName)
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.TakePhoto;
            cds.Para1 = x.ToString();
            cds.Para2 = y.ToString();
            cds.Para3 = z.ToString();
            cds.Para4 = photoName;
            cds.EstimateTime = 0;
            DataObj.InsertCmdData(cds);
        }

        public void qRecordDisplacement(int index)
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.RecordDisplacement;
            cds.Para1 = index.ToString();
            DataObj.InsertCmdData(cds);
        }
        public void qAdjustPMmotor(int xbotId, int moveType, int moveAxis, double movePara, string str = "")
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.AdjustPMmotor;
            cds.Para1 = xbotId.ToString();
            cds.Para2 = moveType.ToString();
            cds.Para3 = moveAxis.ToString();
            cds.Para4 = movePara.ToString();
            cds.Para10 = str;
            cds.EstimateTime = 0;
            DataObj.InsertCmdData(cds);
        }
        /* 针头擦针，前往毛刷位置进行擦针*/
        public void qCleanNozzle(double x, double y, double z)
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.CleanNozzle;
            cds.Para1 = x.ToString();
            cds.Para2 = y.ToString();
            cds.Para3 = z.ToString();
            cds.EstimateTime = 0;
            DataObj.InsertCmdData(cds);
        }
        /*基底转运，停止出丝，龙门移动，腾出空位，等待时间*/
        public void qBasalTransPort(double x, double y, double z, int delay)
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.BasalTransPort;
            cds.Para1 = x.ToString();
            cds.Para2 = y.ToString();
            cds.Para3 = z.ToString();
            cds.Para10 = delay.ToString();
            cds.EstimateTime = 0;
            DataObj.InsertCmdData(cds);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expectStatus"></param>
        public void qSetPrintPosStartEnd(bool bStartA, bool bStartB, int iPrintPos)
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.SetPrintPosStartEnd;
            cds.Para1 = bStartA.ToString();
            cds.Para2 = bStartB.ToString();
            cds.Para3 = iPrintPos.ToString();
            cds.EstimateTime = 0;
            DataObj.InsertCmdData(cds);
        }
        public void qSegmentedMotion(int[] Axes, double[] Center, string str = "")
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.SegmentedMotion;
            cds.Para1 = Axes[0].ToString() + " " + Axes[1].ToString() + " " + Axes[2].ToString();
            cds.Para2 = Center[0].ToString() + " " + Center[1].ToString();
            cds.Para10 = str;
            cds.EstimateTime = 0;
            DataObj.InsertCmdData(cds);
        }

        public void qSegmentStopper(int[] Axes)
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.SegmentStopper;
            cds.Para1 = Axes[0].ToString() + " " + Axes[1].ToString() + " " + Axes[2].ToString();
            cds.EstimateTime = 0;
            DataObj.InsertCmdData(cds);
        }

        public void qExtSegmentedMotion(int[] Axes, double[] startPoint, double velocity, double endVelocity, double juncVelocity, double angle, string str = "")
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.ExtSegmentedMotion;
            cds.Para1 = Axes[0].ToString() + " " + Axes[1].ToString() + " " + Axes[2].ToString();
            for (int i = 3; i < Axes.Length; i++)
            {
                cds.Para1 += (" " + Axes[i].ToString());
            }

            cds.Para2 = startPoint[0].ToString() + " " + startPoint[1].ToString();
            for (int i = 2; i < startPoint.Length; i++)
            {
                cds.Para2 += (" " + startPoint[i].ToString());
            }

            cds.Para3 = velocity.ToString();
            cds.Para4 = endVelocity.ToString();
            cds.Para5 = juncVelocity.ToString();
            cds.Para6 = angle.ToString();
            cds.Para10 = str;

            cds.EstimateTime = 0;
            DataObj.InsertCmdData(cds);
        }

        public void qSegmentLine(int[] Axes, double[] StartPoint, double[] FinalPoint, double Velocity, double EndVelocity, double layer = -1, string strGcode = "", int index = 0, int printPos = 0)
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.SegmentLine;
            cds.Para1 = Axes[0].ToString();
            for (int i = 1; i < Axes.Length; i++)
            {
                cds.Para1 += (" " + Axes[i].ToString());
            }
            cds.Para2 = FinalPoint[0].ToString();
            for (int i = 1; i < FinalPoint.Length; i++)
            {
                cds.Para2 += (" " + FinalPoint[i].ToString());
            }
            cds.Para3 = Velocity.ToString();
            cds.Para4 = EndVelocity.ToString();
            cds.Para9 = layer.ToString();
            cds.Para10 = strGcode;
            cds.Para11 = index.ToString();
            cds.Para12 = printPos.ToString();

            double distance = Math.Sqrt(Math.Pow(StartPoint[0] - FinalPoint[0], 2) + Math.Pow(StartPoint[1] - FinalPoint[1], 2));
            cds.EstimateTime = distance / Velocity;
            DataObj.InsertCmdData(cds);
        }

        public void qSegmentArc1(int[] Axes, double[] Center, double[] FinalPoint, int Rotation, double Velocity, double EndVelocity, double layer = -1, string strGcode = "", int index = 0, int printPos = 0)
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.SegmentArc1;
            //cds.Para1 = Axes[0].ToString() + " " + Axes[1].ToString() + " " + Axes[2].ToString();
            //cds.Para2 = Center[0].ToString() + " " + Center[1].ToString();
            cds.Para1 = Axes[0].ToString();
            for (int i = 1; i < Axes.Length; i++)
            {
                cds.Para1 += (" " + Axes[i].ToString());
            }

            cds.Para2 = Center[0].ToString();
            for (int i = 1; i < Center.Length; i++)
            {
                cds.Para2 += (" " + Center[i].ToString());
            }
            cds.Para3 = FinalPoint[0].ToString() + " " + FinalPoint[1].ToString();
            cds.Para4 = Rotation.ToString();
            cds.Para5 = Velocity.ToString();
            cds.Para6 = EndVelocity.ToString();

            cds.Para9 = layer.ToString();
            cds.Para10 = strGcode;
            cds.Para11 = index.ToString();
            cds.Para12 = printPos.ToString();
            cds.EstimateTime = 0;
            DataObj.InsertCmdData(cds);
        }

        public void qSegmentArc1(int[] Axes, double[] Center, double[] FinalPoint, bool isClockwise, double Velocity, double EndVelocity, double layer = -1, string strGcode = "")
        {
            int Rotation = isClockwise ? -1 : 1;
            qSegmentArc1(Axes, Center, FinalPoint, Rotation, Velocity, EndVelocity, layer, strGcode);
        }


        public void qSegmentArc2(int[] Axes, double[] Center, double Angle, double Velocity, double EndVelocity, double layer = -1, string strGcode = "")
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.SegmentArc2;
            cds.Para1 = Axes[0].ToString();
            for (int i = 1; i < Axes.Length; i++)
            {
                cds.Para1 += (" " + Axes[i].ToString());
            }

            cds.Para2 = Center[0].ToString();
            for (int i = 1; i < Center.Length; i++)
            {
                cds.Para2 += (" " + Center[i].ToString());
            }

            cds.Para3 = Angle.ToString();
            cds.Para4 = Velocity.ToString();
            cds.Para5 = EndVelocity.ToString();
            cds.Para9 = layer.ToString();
            cds.Para10 = strGcode;
            cds.EstimateTime = 0;
            DataObj.InsertCmdData(cds);
        }

        public void qSegmentArc2(int[] Axes, double[] Center, double[] endPoint, double Angle, double Velocity, double EndVelocity, double layer = -1, string strGcode = "", int index = 0, int printPos = 0)
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.SegmentArc2;
            double radius, s;
            cds.Para1 = Axes[0].ToString();
            for (int i = 1; i < Axes.Length; i++)
            {
                cds.Para1 += (" " + Axes[i].ToString());
            }

            cds.Para2 = Center[0].ToString();
            for (int i = 1; i < Center.Length; i++)
            {
                cds.Para2 += (" " + Center[i].ToString());
            }

            cds.Para3 = Angle.ToString();
            cds.Para4 = Velocity.ToString();
            cds.Para5 = EndVelocity.ToString();
            cds.Para9 = layer.ToString();
            cds.Para10 = strGcode;
            cds.Para11 = index.ToString();
            cds.Para12 = printPos.ToString();

            if (Axes.Length == 3)
            {
                radius = Math.Sqrt(Math.Pow(endPoint[0] - Center[0], 2) + Math.Pow(endPoint[1] - Center[1], 2));
            }
            else
            {
                radius = Math.Sqrt(Math.Pow(endPoint[0] - Center[0], 2) + Math.Pow(endPoint[1] - Center[1], 2) + Math.Pow(endPoint[2] - Center[2], 2));
            }

            s = Angle * radius;
            cds.EstimateTime = s / Velocity;
            DataObj.InsertCmdData(cds);
        }

        public void qEndSequenceM(int[] Axes, string str = "")
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.EndSequenceM;
            cds.Para1 = Axes[0].ToString();
            for (int i = 1; i < Axes.Length; i++)
            {
                cds.Para1 += (" " + Axes[i].ToString());
            }
            cds.Para10 = str;
            cds.EstimateTime = 0;
            DataObj.InsertCmdData(cds);
        }
        public void qExtrude(ExtrudePortType port, int value)
        {
            qExtrude((int)port, value);
        }
        /// <summary>
        /// 控制挤出装置
        /// </summary>
        /// <param name="port">I/O端口号</param>
        /// <param name="value">0:关; 1: 开</param>
        public void qExtrude(int port, int value, string str = "", double speed = 0)
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.Extrude;
            cds.Para1 = port.ToString();
            cds.Para2 = value.ToString();
            //新增，设置不同的转速
            cds.Para9 = double.IsNaN(speed) ? null : speed.ToString();
            cds.Para10 = str;
            cds.EstimateTime = 0;
            DataObj.InsertCmdData(cds);
        }

        /// <summary>
        /// 设置螺杆转速
        /// </summary>
        /// <param name="port">螺杆的端口号</param>
        /// <param name="value">转速值</param>
        public void qSetRotarySpeed(int port, double speed)
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.SetRotarySpeed;
            cds.Para1 = port.ToString();
            cds.Para2 = speed.ToString();
            cds.EstimateTime = 0;
            DataObj.InsertCmdData(cds);
        }

        public void qSwitchNozzle(int nozzleNum)
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.SwitchNozzle;
            cds.Para1 = nozzleNum.ToString();
            cds.EstimateTime = 0;
            DataObj.InsertCmdData(cds);
        }
        public void qSetPressure(int iExtruder, double pressure)
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.SetPressure;
            cds.Para1 = iExtruder.ToString();
            cds.Para2 = pressure.ToString();
            cds.EstimateTime = 0;
            DataObj.InsertCmdData(cds);

        }

        /// <summary>
        /// 检测位移笔的位置状态是否符合预期
        /// </summary>
        /// <param name="expectStatus">-1: 通信失败, 0: 伸出, 1: 缩回, 2: 伸出且缩回, 3: 未伸出且未缩回</param>
        public void qCheckPenStatus(int expectStatus)
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.CheckPenStatus;
            cds.Para1 = expectStatus.ToString();
            cds.EstimateTime = 0;
            DataObj.InsertCmdData(cds);
        }
        public void qHighVolControl(int value)
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.HighVoltage;
            cds.Para1 = value.ToString();
            cds.EstimateTime = 0;
            DataObj.InsertCmdData(cds);
        }

        public void qSetHighVoltageValue(int value)
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.SetHighVoltageValue;
            cds.Para1 = value.ToString();
            cds.EstimateTime = 0;
            DataObj.InsertCmdData(cds);
        }

        /// <summary>
        /// 移动到指定的X、Y坐标位置
        /// </summary>
        /// <param name="xPos">目标位置x</param>
        /// <param name="yPos">目标位置y</param>
        /// <param name="speed">移动速度</param>
        /// <param name="xStart">开始位置x</param>
        /// <param name="yStart">开始位置y</param>
        public void qMoveXYTo(double xPos, double yPos, double speed, double xStart, double yStart, double layer = -1, string strGcode = "", int index = 0, int printPos = 0)
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.MoveXYTo;
            cds.Para1 = xPos.ToString();
            cds.Para2 = yPos.ToString();
            cds.Para3 = speed.ToString();
            cds.Para9 = layer.ToString();
            cds.Para10 = strGcode;
            cds.Para11 = index.ToString();
            cds.Para12 = printPos.ToString();

            double s = Math.Sqrt(Math.Pow(xPos - xStart, 2) + Math.Pow(yPos - yStart, 2));
            cds.EstimateTime = s / speed;
            cds.EstimateTime += 0.2;
            DataObj.InsertCmdData(cds);
        }

        public void qMoveXYZTo(double xPos, double yPos, double zPos, double speed, double xStart, double yStart, double zStart, double layer = -1, string strGcode = "")
        {
            DataManagement.CmdDataStruct cds;
            double estimateTime, t1, t2, t3;
            t1 = Math.Abs(xPos - xStart) / speed;
            t2 = Math.Abs(yPos - yStart) / speed;
            t3 = Math.Abs(zPos - zStart) / speed;
            estimateTime = t1 > t2 ? (t1 > t3 ? t1 : t3) : (t2 > t3 ? t2 : t3); // max(t1, t2, t3)

            cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.MoveAxisTo;
            cds.Para1 = GV.Z.ToString();
            cds.Para2 = zPos.ToString();
            cds.Para3 = speed.ToString();
            cds.Para9 = layer.ToString();
            cds.Para10 = strGcode;
            cds.EstimateTime = estimateTime;
            cds.EstimateTime += 0.2; // 加减速多消耗的时间
            DataObj.InsertCmdData(cds);

            cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.MoveAxisTo;
            cds.Para1 = GV.X.ToString();
            cds.Para2 = xPos.ToString();
            cds.Para3 = speed.ToString();
            cds.Para9 = layer.ToString();
            cds.Para10 = strGcode;
            cds.EstimateTime = 0;
            DataObj.InsertCmdData(cds);

            cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.MoveAxisTo;
            cds.Para1 = GV.Y.ToString();
            cds.Para2 = yPos.ToString();
            cds.Para3 = speed.ToString();
            cds.Para9 = layer.ToString();
            cds.Para10 = strGcode;
            cds.EstimateTime = 0;
            DataObj.InsertCmdData(cds);
        }

        public void qMoveAxisTo(int axis, double pos, double speed, double posStart, double layer = -1, string strGcode = "", int index = 0, int printPos = 0)
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.MoveAxisTo;
            cds.Para1 = axis.ToString();
            cds.Para2 = pos.ToString();
            cds.Para3 = speed.ToString();

            cds.Para9 = layer.ToString();
            cds.Para10 = strGcode;
            cds.Para11 = index.ToString();
            cds.Para12 = printPos.ToString();

            cds.EstimateTime = Math.Abs(posStart - pos) / speed;
            cds.EstimateTime += 0.2; // 加减速多消耗的时间
            DataObj.InsertCmdData(cds);
        }
        public void qAdjustMicroMotor(int axis, double pos, double posStart, string strGcode = "")
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.AdjustMicroMotor;
            cds.Para1 = axis.ToString();
            cds.Para2 = pos.ToString();
            cds.Para10 = strGcode;
            cds.EstimateTime = Math.Abs(posStart - pos) / 10;
            cds.EstimateTime += 0.2; // 加减速多消耗的时间
            DataObj.InsertCmdData(cds);
        }

        public void qMoveAxisRelative(int axis, double distance, double speed, double layer = -1, string strGcode = "", int index = 0, int printPos = 0)
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.MoveAxisRelative;
            cds.Para1 = axis.ToString();
            cds.Para2 = distance.ToString();
            cds.Para3 = speed.ToString();

            cds.Para9 = layer.ToString();
            cds.Para10 = strGcode;
            cds.Para11 = index.ToString();
            cds.Para12 = printPos.ToString();

            cds.EstimateTime = Math.Abs(distance) / speed;
            cds.EstimateTime += 0.2; // 加减速多消耗的时间
            DataObj.InsertCmdData(cds);
        }

        public void qDisplayInfo(string InfoName, string InfoText, string InfoColor = "TransParent", int layer = -1)
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.DisplayInfo;
            cds.Para1 = InfoName;
            cds.Para2 = InfoText;
            cds.Para3 = InfoColor;
            cds.Para10 = layer.ToString();
            cds.EstimateTime = 0;
            DataObj.InsertCmdData(cds);
        }
        public void GetNoticeInfo(out string notice, out string color)
        {
            notice = this.NoticeInfo;
            color = this.InfoColor;
        }
        //*********************************************************************************//


        public bool IsHomedAxis(int axis)
        {
            int MFLAGS = Ch.ReadVariable("MFLAGS", Ch.ACSC_NONE, axis, 0);
            int BRUSHL = (int)Math.Pow(2, 8);   // if the motor is a DC brushless motor.
            int BRUSHOK = (int)Math.Pow(2, 9);
            if ((MFLAGS & BRUSHL) != 0 && (MFLAGS & BRUSHOK) == 0)
            {
                return (false);
            }
            else
            {
                return (true);
            }
        }

        public void Home(int axis)
        {
            try
            {
                //Ch.ToPoint(0, axis, 0);
                Ch.ExtToPoint(Ch.ACSC_AMF_VELOCITY, axis, 0, 5, 0);
            }
            catch (Exception ex)
            {
                PushMessage(ex.Message);
            }
        }

        public void HomeAll()
        {
            try
            {
                qMoveAxisTo(GV.Z, 0, 30, Status.fPosX);
                qWaitMoveEnd();
                qMoveXYTo(0, 0, 30, Status.fPosX, Status.fPosY);
            }
            catch (Exception ex)
            {
                PushMessage(ex.Message);
            }
        }

        public void ResetXYZ()
        {
            try
            {
                // 如果不是仿真器
                if (!IsSimulator)
                {
                    //执行buffer进行所有轴回零
                    Ch.RunBuffer(0);
                }
                else // 如果是仿真器
                {
                    Ch.RunBuffer(0, "SIMULATOR");
                }
            }
            catch (Exception ex)
            {
                PushMessage(ex.Message);
            }
        }

        public void RunBuffer(int buffID)
        {
            Ch.RunBuffer(buffID);
        }

        public void RunBuffer(int buffID, string buffLabel)
        {
            Ch.RunBuffer(buffID, buffLabel);
        }

        public void StopBuffer(int buffID)
        {
            Ch.StopBuffer(buffID);
        }

        /// <summary>
        /// 对ACS系统变量值按位赋值为0或1
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="Mask"></param>
        /// <param name="bitValue"></param>
        public void SetVariableByMask(int axis, string VarName, int Mask, int bitValue)
        {
            int Value;  //原来的状态变量值
            if (bitValue == 0)
            {
                Value = Ch.ReadVariable(VarName, Ch.ACSC_NONE, axis, axis);
                Value = SetBit0(Value, Mask); // 左限位开关使能置0
                Ch.WriteVariable(Value, VarName, Ch.ACSC_NONE, axis, axis);
            }
            else if (bitValue == 1)
            {
                Value = Ch.ReadVariable(VarName, Ch.ACSC_NONE, axis, axis);
                Value = SetBit1(Value, Mask); // 左限位开关使能置1
                Ch.WriteVariable(Value, VarName, Ch.ACSC_NONE, axis, axis);
            }
            else
            {
                return;
            }
            //FDEF = Ch.ReadVariable("FDEF", Ch.ACSC_NONE, 0, 0);
            //FDEF = SetBit1(FDEF, Ch.ACSC_SAFETY_LL); // 左限位开关使能置零
            //Ch.WriteVariable(FDEF, "FDEF", Ch.ACSC_NONE, 0, 0);

            //FDEF0 = SetBit0(FDEF0, Ch.ACSC_SAFETY_LL); // 左限位开关使能置零
            //Ch.WriteVariable(FDEF0, "FDEF", Ch.ACSC_NONE, 0, 0);
            // Ch.WriteVariable(11, "VEL", Ch.ACSC_NONE, 2, 2);
        }

        private int SetBit0(int oldValue, int Mask)
        {
            int newValue = oldValue & (~Mask);
            return newValue;
        }

        private int SetBit1(int oldValue, int Mask)
        {
            int newValue = oldValue | Mask;
            return newValue;
        }

        public void EnableMotor(int axis)
        {
            if (bConnected)
                Ch.Enable(axis);
        }

        public void DisableMotor(int axis)
        {
            if (bConnected)
                Ch.Disable(axis);
        }

        public double GetFPosition(int axis)
        {
            return Ch.GetFPosition(axis);
        }

        public double GetFVelocity(int axis)
        {
            return Ch.GetFVelocity(axis);
        }

        public double GetAcceleration(int axis)
        {
            return Ch.GetAcceleration(axis);
        }

        public double GetJerk(int axis)
        {
            return Ch.GetJerk(axis);
        }

        public double GetGlobalVelocity()
        {
            return Convert.ToDouble(Ch.ReadVariable("GVEL", Ch.ACSC_NONE)[0]);
        }

        public double GetLeftTime()
        {
            return Convert.ToDouble(Ch.ReadVariable("GRTIME", Ch.ACSC_NONE)[0]);

        }

        public int GetOutput()
        {
            return Ch.GetOutput(IsSimulator ? 0 : 1, 0);
        }
        //读取指令空余量
        public int GetGSFree()
        {
            return Ch.ReadVariable("GSFREE", 1, GV.X, GV.X);
        }
        //获取当前执行的指令数
        public int GetGSeg()
        {
            return Ch.ReadVariable("GSEG", 1, GV.X, GV.X);
        }

        public object GetGRTime()
        {
            return Ch.ReadVariable("GRTIME", Ch.ACSC_NONE);
        }

        public bool IsMotorMoving(int axis)
        {
            try
            {
                return Convert.ToBoolean(Ch.GetMotorState(axis) & Ch.ACSC_MST_MOVE);
                int iINPOS = 4;
                return Convert.ToBoolean(Ch.ReadVariable("MST", Ch.ACSC_NONE, axis, iINPOS));
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public bool IsMotorEnabled(int axis)
        {
            return Convert.ToBoolean(Ch.GetMotorState(axis) & Ch.ACSC_MST_ENABLE);
            int iENABLED = 0;
            return Convert.ToBoolean(Ch.ReadVariable("MST", Ch.ACSC_NONE, axis, iENABLED));
        }

        public void ClearCommand()
        {
            Controlcommand.Clear();
            Monitordata.Clear();
        }

        /// <summary>
        /// 伸出位移笔传感器
        /// </summary>
        public void PushDownPenSensor()
        {
            GV.PrintingObj.bPenInPos = false;
            int valuePortPen = GV.PrintingObj.GetExtrudePort(ExtrudePortType.DisplacePen);
            if (valuePortPen != 1)
            {
                //MessageBox.Show("检测到位移笔未伸出，即将自动伸出。", "提示");
                // 如果位移笔传感器未伸出，首先提升至安全高度后伸出
                if (!GV.PrintingObj.Status.isEnabledZ)
                {
                    return;
                }
                if (GV.PrintingObj.Status.fPosZ > GV.Z_TOP)
                {
                    GV.PrintingObj.qMoveAxisTo(GV.Z, GV.Z_TOP, 30, 0);
                }
                GV.PrintingObj.qWaitMoveEnd();
                GV.PrintingObj.qExtrude(ExtrudePortType.RotaryValve, 0);      // 关闭螺杆阀
                GV.PrintingObj.qSetPressure(1, 50);     // 设置一个较小的气压
                GV.PrintingObj.qPause(2000);            // 等待气压达到设定值
                //GV.PrintingObj.qExtrude(0, 1);        // 打开气压开关
                GV.PrintingObj.qExtrude(ExtrudePortType.DisplacePen, 1);      // 伸出位移笔
                GV.PrintingObj.qCheckPenStatus(0);      // 检查是否伸出到位
                GV.PrintingObj.qPause(2000);            // 等待位移笔伸出到位
                GV.PrintingObj.qSetPressure(1, 300);    // 设置一个较高气压推住位移笔
            }
        }

        /// <summary>
        /// 收回位移笔传感器
        /// </summary>
        public void PullUpPenSensor()
        {
            GV.PrintingObj.bPenInPos = false;
            int valuePortPen = GV.PrintingObj.GetExtrudePort(ExtrudePortType.DisplacePen);
            if (valuePortPen == 1)
            {
                GV.PrintingObj.qWaitMoveEnd();
                GV.PrintingObj.qSetPressure(1, 80);    // 设置一个的气压
                //GV.PrintingObj.qExtrude(0, 1);          // 打开气压开关
                GV.PrintingObj.qExtrude(ExtrudePortType.RotaryValve, 0);          // 关闭螺杆阀
                GV.PrintingObj.qPause(500);             // 等待气压达到设定值
                GV.PrintingObj.qExtrude(ExtrudePortType.DisplacePen, 0);          // 缩回位移笔
                //GV.PrintingObj.qPause(200);           // 等待响应
                //GV.PrintingObj.qSetPressure(0, 0);    // 设置气压为0
                GV.PrintingObj.qPause(2000);            // 等待位移笔回缩到位
                GV.PrintingObj.qCheckPenStatus(1);      // 检查是否缩回
                GV.PrintingObj.qSetPressure(1, 100);     // 设置一个能维持位移笔位置的较小气压
            }
        }

        /// <summary>
        /// 查询基底测距进度
        /// </summary>
        /// <returns>完成度百分比：0~100</returns>
        public void GetBaseDetectPercent(out int percent, out string info)
        {
            percent = this.detectPercent;
            info = this.NoticeInfo;
        }

        public List<string> listDisplacementLiDanRecord = new List<string>();
        public List<double> listDistanceRecordValue = new List<double>();
        /// <summary>
        /// 收回微型滑台
        /// </summary>

        public struct StatusStruct
        {
            public double fPosX;
            public double fPosY;
            public double fPosZ;
            public double fVelX;
            public double fVelY;
            public double fVelZ;
            public double gVel;
            public double fAccX;
            public double fAccY;
            public double fAccZ;
            public bool isEnabledX;
            public bool isEnabledY;
            public bool isEnabledZ;
            public bool isMovingX;
            public bool isMovingY;
            public bool isMovingZ;
            public bool isInPosX;
            public bool isInPosY;
            public bool isInPosZ;
            public double leftTime;
            public bool isExtruding;
            public string layerCurrent;
            public string filamentNumber;
            public List<string> statusMsg;

        }
        internal int targetLayer = 0;
        //查看打印机状态
        public string UpdateStatus(long ElapsedMilliseconds)
        {
            try
            {
                dynamic[] fpos = Ch.ReadVariable("FPOS", Ch.ACSC_NONE, 0, 6);
                dynamic[] apos = Ch.ReadVariable("APOS", Ch.ACSC_NONE, 0, 6);
                dynamic[] rpos = Ch.ReadVariable("RPOS", Ch.ACSC_NONE, 0, 6);
                dynamic[] velo = Ch.ReadVariable("FVEL", Ch.ACSC_NONE, 0, 6);
                dynamic[] acce = Ch.ReadVariable("FACC", Ch.ACSC_NONE, 0, 6);
                //食品机
                //dynamic[] fpos = Ch.ReadVariable("FPOS", Ch.ACSC_NONE, 0, 3);
                //dynamic[] apos = Ch.ReadVariable("APOS", Ch.ACSC_NONE, 0, 3);
                //dynamic[] rpos = Ch.ReadVariable("RPOS", Ch.ACSC_NONE, 0, 3);
                //dynamic[] velo = Ch.ReadVariable("FVEL", Ch.ACSC_NONE, 0, 3);
                //dynamic[] acce = Ch.ReadVariable("FACC", Ch.ACSC_NONE, 0, 3);
                //平面电机
                //显示PMC状态
                PMCStatusResult statusResult = CheckPMCStatus();
                string currentStatus = statusResult.CurrentPMCStatus;
                Status.PMCStatus = currentStatus;

                Status.fXbotPosXa = GetXbotPosition(GV.PMC.arrXBotIds[0], GV.PMC.X) * GV.PMC.M2MM;
                Status.fXbotPosYa = GetXbotPosition(GV.PMC.arrXBotIds[0], GV.PMC.Y) * GV.PMC.M2MM;
                Status.fXbotPosZa = GetXbotPosition(GV.PMC.arrXBotIds[0], GV.PMC.Z) * GV.PMC.M2MM;
                Status.fXbotRxA = GetXbotPosition(GV.PMC.arrXBotIds[0], GV.PMC.Rx) * GV.PMC.Rad2Degree;
                Status.fXbotRyA = GetXbotPosition(GV.PMC.arrXBotIds[0], GV.PMC.Ry) * GV.PMC.Rad2Degree;
                Status.fXbotRzA = GetXbotPosition(GV.PMC.arrXBotIds[0], GV.PMC.Rz) * GV.PMC.Rad2Degree;

                Status.isActiveA = GV.PrintingObj.CheckXbotState(GV.PMC.arrXBotIds[0], 1);//检查漂浮状态

                //GV.PrintingObj.CheckXbotState(GV.PMC.XbotId, 3);//检查移动状态

                Status.time = ElapsedMilliseconds;//时间

                Status.fPosX = apos[GV.X];     //Ch.GetFPosition(0);
                Status.fPosY = apos[GV.Y];     //Ch.GetFPosition(1);
                Status.fPosZ = apos[GV.Z];     //Ch.GetFPosition(2);
                Status.fPosZ1 = apos[GV.Z1];
                Status.fPosZ2 = apos[GV.Z2];

                Status.fVelX = velo[GV.X];
                Status.fVelY = velo[GV.Y];
                Status.fVelZ = velo[GV.Z];
                Status.fVelZ1 = velo[GV.Z1];
                Status.fVelZ2 = velo[GV.Z2];

                Status.fAccX = acce[GV.X];
                Status.fAccY = acce[GV.Y];
                Status.fAccZ = acce[GV.Z];

                //Status.fVelX = Ch.GetFVelocity(GV.X);
                //Status.fVelY = Ch.GetFVelocity(GV.Y);
                //Status.fVelZ = Ch.GetFVelocity(GV.Z);

                //Status.fAccX = Ch.GetAcceleration(GV.X);
                //Status.fAccY = Ch.GetAcceleration(GV.Y);
                //Status.fAccZ = Ch.GetAcceleration(GV.Z);

                Status.gVel = Math.Sqrt(Status.fVelX * Status.fVelX + Status.fVelY * Status.fVelY + Status.fVelZ * Status.fVelZ);
                dynamic dy = Ch.ReadVariable("GSFREE", Ch.ACSC_NONE);
                Status.GSFREE[0] = dy[GV.X];
                Status.GSFREE[1] = dy[GV.Y];
                Status.GSFREE[2] = dy[GV.Z];

                //dynamic dx = Ch.ReadVariable("GSEG", 1, GV.X, GV.X);
                //Status.GSEG = dx;               

                //Convert.ToDouble(Ch.ReadVariable("GVEL", Ch.ACSC_NONE)[GV.X]);

                object[] grTime = Ch.ReadVariable("GRTIME", Ch.ACSC_NONE);
                double maxLeftTime = Convert.ToDouble(grTime[GV.X]);
                if (Convert.ToDouble(grTime[1]) > maxLeftTime)
                    maxLeftTime = Convert.ToDouble(grTime[GV.Y]);
                if (Convert.ToDouble(grTime[2]) > maxLeftTime)
                    maxLeftTime = Convert.ToDouble(grTime[GV.Z]);

                //Status.GRTIME[0] = Convert.ToDouble(grTime[GV.X]);
                //Status.GRTIME[1] = Convert.ToDouble(grTime[GV.Y]);
                //Status.GRTIME[2] = Convert.ToDouble(grTime[GV.Z]);

                Status.leftTime = maxLeftTime;

                Status.isExtruding = GetExtrudePort(0) == 1;
                Status.nozzleID = GetExtrudePort(2); // 0: 左喷头；1：右喷头
                Status.isExtruding2 = GetExtrudePort(1) == 1;

                // Get motor state of the Axis
                int MotorState = Ch.GetMotorState(GV.X);
                Status.isEnabledX = Convert.ToBoolean(MotorState & Ch.ACSC_MST_ENABLE);
                Status.isMovingX = Convert.ToBoolean(MotorState & Ch.ACSC_MST_MOVE);
                Status.isInPosX = Convert.ToBoolean(MotorState & Ch.ACSC_MST_INPOS);

                MotorState = Ch.GetMotorState(GV.Y);
                Status.isEnabledY = Convert.ToBoolean(MotorState & Ch.ACSC_MST_ENABLE);
                Status.isMovingY = Convert.ToBoolean(MotorState & Ch.ACSC_MST_MOVE);
                Status.isInPosY = Convert.ToBoolean(MotorState & Ch.ACSC_MST_INPOS);

                MotorState = Ch.GetMotorState(GV.Z);
                Status.isEnabledZ = Convert.ToBoolean(MotorState & Ch.ACSC_MST_ENABLE);
                Status.isMovingZ = Convert.ToBoolean(MotorState & Ch.ACSC_MST_MOVE);
                Status.isInPosZ = Convert.ToBoolean(MotorState & Ch.ACSC_MST_INPOS);

                //微动滑台
                MotorState = Ch.GetMotorState(GV.Z1);
                Status.isEnabledZ1 = Convert.ToBoolean(MotorState & Ch.ACSC_MST_ENABLE);
                Status.isMovingZ1 = Convert.ToBoolean(MotorState & Ch.ACSC_MST_MOVE);
                //Status.isInPosZ = Convert.ToBoolean(MotorState & Ch.ACSC_MST_INPOS);
                MotorState = Ch.GetMotorState(GV.Z2);
                Status.isEnabledZ2 = Convert.ToBoolean(MotorState & Ch.ACSC_MST_ENABLE);
                Status.isMovingZ2 = Convert.ToBoolean(MotorState & Ch.ACSC_MST_MOVE);
                return "Normal";
            }
            catch (Exception ex)
            {
                if (bConnected)
                    return "Normal";
                else
                {
                    PushMessage(ex.Message);
                    return ex.Message;
                }
            }
        }
        /// <summary>
        /// 移动到目标位置（先提针、平移、再下针）
        /// </summary>
        /// <param name="x">目标X</param>
        /// <param name="y">目标Y</param>
        /// <param name="z">目标Z</param>
        /// <param name="NoticeInfo">达到目标位置的通知消息</param>
        /// <param name="zTop">提针到水平移动的高度</param>
        /// <returns>估计所需运动时间</returns>
        public int Move2ReadyPos(double x, double y, double z, string NoticeInfo, double zTop = 0)
        {
            if (GV.CheckClearCommands() != ClearResult.Needless) return 0;

            double vZup = 20;       // Z轴提针速度(mm/s)
            double vZdown1 = 10;    // Z轴第一阶段下针速度(mm/s)
            double vZdown2 = 2;     // Z轴第二阶段下针速度(mm/s)
            double vXY = 50;        // XY轴移动速度(mm/s)
            double dNear = 10;       // 接近减速距离(mm)
            // 估计运动至起始点的时间：
            double timeEstimate = 0;

            GV.PrintingObj.qDisplayInfo("Notice", "Moving");

            // 分4步移动到目标位置：
            // 第1步：快速将Z轴升至提针位置
            if (zTop > z) // 防止提针位置比目标位置还低发生意外
            {
                MessageBox.Show("提针位置比目标位置还低，请检查", "问题提示");
            }
            GV.PrintingObj.qMoveAxisTo(GV.Z, zTop, vZup, 0);
            GV.PrintingObj.qWaitMoveEnd();
            timeEstimate += (Math.Abs(GV.PrintingObj.Status.fPosZ - zTop) / vZup);  // 耗时估算

            // 第2步：将XY轴移动到目标位置
            GV.PrintingObj.qMoveXYTo(x, y, vXY, 0, 0);
            GV.PrintingObj.qWaitMoveEnd();
            timeEstimate += (Math.Sqrt(Math.Pow(GV.PrintingObj.Status.fPosX - x, 2) + Math.Pow(GV.PrintingObj.Status.fPosY - y, 2)) / vXY);  // 耗时估算
            // 第3步：较慢速将Z轴降至开始减速的接近目标位置
            if (zTop < z - dNear) // 开始减速的接近目标位置比当前位置低
            {
                GV.PrintingObj.qMoveAxisTo(GV.Z, z - dNear, vZdown1, 0);
                GV.PrintingObj.qWaitMoveEnd();
                timeEstimate += (Math.Abs(z - dNear) / vZdown1);  // 耗时估算                
            }
            // 第4步：非常慢速将Z轴降至目标位置
            GV.PrintingObj.qMoveAxisTo(GV.Z, z, vZdown2, 0);
            GV.PrintingObj.qWaitMoveEnd();
            GV.PrintingObj.qDisplayInfo("Notice", NoticeInfo);
            timeEstimate += (5 / vZdown2);  // 耗时估算

            timeEstimate += 2;
            return (int)(timeEstimate * 1000);
        }

        ///PMC

        //this class contains a collection of system commands such as connecting to the PMC, gain mastership, etc.
        public static SystemCommands _systemCommand = new SystemCommands();
        //this class contains a collection of xbot commands, such as activate xbots, levitation control, linear motion, etc.
        private static XBotCommands _xbotCommand = new XBotCommands();
        //PMC is the the operation state, we can proceed to check the xbot count
        XBotIDs xBot_IDs = new XBotIDs(); //this command retrieves the XBOT IDs from the PMC. The return value contains the xbot count, and an array with all the XBOT
        public bool isConnectedToPMC = _systemCommand.ConnectToSpecificPMC(GV.PMC.IpAddress);

        MotionRtn motionRtn = new MotionRtn();
        //get the current xbot position
        XBotStatus xBotStatus = new XBotStatus();
        public bool OpenPMCommEthernet(string Address)
        {
            try
            {
                //if (IfConnected())
                //{
                //    CloseComm();
                //}
                //Ch.OpenCommEthernet(Address, Ch.ACSC_SOCKET_STREAM_PORT);
                //bConnected = true;
                //IsSimulator = Address.StartsWith("127.0.0.1");
                bPMConnected = _systemCommand.ConnectToSpecificPMC(Address);//连接到指定IP
            }
            catch (Exception ex)
            {
                bPMConnected = false;
                PushMessage(ex.Message);
            }
            return bPMConnected;
        }

        //获取主机，待完善，双设备时一个主机，一个从机
        PMCRTN rtnVal = _systemCommand.GainMastership();    //sends the gain mastership command
        public void GainMastership()
        {
            rtnVal = _systemCommand.GainMastership();
            if (rtnVal == PMCRTN.ALLOK)
            {

            }
        }

        //check if the PMC is in operation
        PMCSTATUS pmcStat = new PMCSTATUS(); //send the get PMC status command
                                             // 定义返回的状态结果类
        public class PMCStatusResult
        {
            public bool IsInOperation { get; set; }
            public PMCSTATUS CurrentStatus { get; set; }
            public bool ActivationAttempted { get; set; }
            public string ErrorMessage { get; set; }

            // 详细状态标志
            public bool IsActivating { get; set; }
            public bool IsBooting { get; set; }
            public bool IsDeactivating { get; set; }
            public bool HasError { get; set; }
            public bool IsFullControl { get; set; }
            public bool IsIntelligentControl { get; set; }
            public string CurrentPMCStatus { get; set; }
        }
        //if the PMC is not in the Operation State, then we run the following code to bring it into the operation state
        public PMCStatusResult CheckPMCStatus()
        {
            var result = new PMCStatusResult
            {
                IsInOperation = false,
                CurrentStatus = pmcStat,
                ActivationAttempted = false,
                ErrorMessage = null,
                CurrentPMCStatus = null,

            };

            if (pmcStat != PMCSTATUS.PMC_FULLCTRL && pmcStat != PMCSTATUS.PMC_INTELLIGENTCTRL)
            {
                bool attemptedActivation = false;
                while (!result.IsInOperation)
                {
                    pmcStat = _systemCommand.GetPMCStatus();

                    result.CurrentStatus = pmcStat;

                    switch (pmcStat)
                    {
                        case PMCSTATUS.PMC_ACTIVATING:
                            result.IsActivating = true;
                            GV.PMC.pmc_activating = true;
                            result.CurrentPMCStatus = "PMC_ACTIVATING";
                            break;

                        case PMCSTATUS.PMC_BOOTING:
                            result.IsBooting = true;
                            result.CurrentPMCStatus = "PMC_BOOTING";
                            break;

                        case PMCSTATUS.PMC_DEACTIVATING:
                            result.IsDeactivating = true;
                            result.CurrentPMCStatus = "PMC_DEACTIVATING";
                            break;

                        case PMCSTATUS.PMC_ERRORHANDLING:
                            System.Threading.Thread.Sleep(1000);
                            result.CurrentPMCStatus = "PMC_ERRORHANDLING";
                            break;

                        case PMCSTATUS.PMC_ERROR:
                            result.HasError = true;
                            //result.ErrorMessage = "PMC is in error state";
                            result.CurrentPMCStatus = "PMC_ERROR";
                            return result;

                        case PMCSTATUS.PMC_INACTIVE:
                            result.CurrentPMCStatus = "PMC_INACTIVE";
                            if (!attemptedActivation)
                            {
                                Console.WriteLine("Activate all xbots");
                                result.ActivationAttempted = true;
                                attemptedActivation = true;

                                //if (rtnVal != PMCRTN.ALLOK)
                                //{
                                //    result.ErrorMessage = $"Failed to Activate XBOTs. Error: {rtnVal}";
                                //    return result;
                                //}
                                return result;
                            }
                            else
                            {
                                result.ErrorMessage = "Attempted but failed to Activate XBOTs";
                                return result;
                            }
                            break;

                        case PMCSTATUS.PMC_FULLCTRL:
                            result.IsInOperation = true;
                            result.IsFullControl = true;
                            GV.PMC.pmc_fullcontrol = true;
                            result.CurrentPMCStatus = "PMC_FULLCTRL";
                            break;

                        case PMCSTATUS.PMC_INTELLIGENTCTRL:
                            result.IsInOperation = true;
                            result.IsIntelligentControl = true;
                            result.CurrentPMCStatus = "PMC_INTELLIGENTCTRL";
                            break;

                        default:
                            result.ErrorMessage = $"Unexpected PMC State: {pmcStat}";
                            return result;
                    }
                }
            }
            else
            {
                result.IsInOperation = true;
                result.IsFullControl = pmcStat == PMCSTATUS.PMC_FULLCTRL;
                result.IsIntelligentControl = pmcStat == PMCSTATUS.PMC_INTELLIGENTCTRL;
            }

            return result;
        }

        //检查动子数量
        public int expectedXbotCount = 0;
        public int CheckXbotCount()
        {
            // 初始化返回值为0（默认无动子）
            int detectedCount = -1;
            try
            {
                xBot_IDs = _xbotCommand.GetXBotIDS();
                if (xBot_IDs.PmcRtn == PMCRTN.ALLOK)
                {
                    // 仅当检测到动子时才更新期望数量
                    if (xBot_IDs.XBotCount > 0)
                    {
                        detectedCount = xBot_IDs.XBotCount;
                    }
                }
                else
                {
                    detectedCount = 0;
                }
            }
            catch (Exception ex)
            {
            }
            return detectedCount;
        }

        public void NamingXbotIDs()
        {
            try
            {
                xBot_IDs = _xbotCommand.GetXBotIDS();
                if (xBot_IDs.PmcRtn == PMCRTN.ALLOK)
                {
                    for (int i = 0; i < xBot_IDs.XBotIDsArray.Length; i++)
                    {
                        GV.PMC.arrXBotIds[i] = xBot_IDs.XBotIDsArray[i];
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        //获取动子状态
        XBotStatus xbotStatus;
        XBOTSTATE xbotstate;
        //读取动子实际位置信息
        public double GetXbotPosition(int xbotId, int positionType, ALLXBOTSFEEDBACKOPTION feedbackOption = ALLXBOTSFEEDBACKOPTION.POSITION)
        {
            AllXBotInfo allXbotInfo = _xbotCommand.GetAllXbotInfo();
            //XBotInfo item = new XBotInfo();
            // 查找特定ID的XBot
            XBotInfo item = allXbotInfo.AllXbotInfoList.FirstOrDefault(x => x.XbotID == xbotId);

            switch (positionType)
            {
                case 1:
                    return item.XPos;
                case 2:
                    return item.YPos;
                case 3:
                    return item.ZPos;
                case 4:
                    return item.RxPos;
                case 5:
                    return item.RyPos;
                case 6:
                    return item.RzPos;
                default:
                    throw new ArgumentException($"Invalid position type: {positionType}");
            }
        }
        public bool isXbotMoving(int axis)
        {
            try
            {
                //先得到所有状态
                xbotStatus = _xbotCommand.GetXbotStatus(axis);

                xbotstate = xbotStatus.XBOTState;
                if (xbotstate == XBOTSTATE.XBOT_MOTION)//反应慢，不应用
                    return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }
        public bool CheckXbotState(int xbotId, int stateTye)
        {
            try
            {
                //先得到所有状态
                xbotStatus = _xbotCommand.GetXbotStatus(xbotId);
                xbotstate = xbotStatus.XBOTState;
                switch (stateTye)
                {
                    case 1://空闲状态
                        //int count1 = 0;
                        //while (xbotstate != XBOTSTATE.XBOT_IDLE)
                        //{
                        //    Thread.Sleep(100);
                        //    xbotStatus = _xbotCommand.GetXbotStatus(xbotId);

                        //    count1++;
                        //    if (count1 > 100)
                        //    {
                        //        Status.isActiveA = false;
                        //        _xbotCommand.StopMotion(xbotId);
                        //        return false; // 超时未进入空闲状态
                        //    }
                        //}
                        if (xbotstate == XBOTSTATE.XBOT_IDLE)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                            // MessageBox.Show("xbot stopped");
                            return true;
                    case 2: //等待触发                     
                        if (xbotstate == XBOTSTATE.XBOT_WAIT)
                        {
                            StopXbotMotion(GV.PMC.XbotId);
                            MessageBox.Show("正在等待超过阈值");
                            return true;
                        }
                        return false;
                    case 3:
                        int count2 = 0;
                        if (xbotstate != XBOTSTATE.XBOT_MOTION)
                        {
                            Status.isMovingX = false;
                            return true;
                        }
                        else
                        {
                            Status.isMovingX = true;
                            return false;
                        }

                    case 4:
                        int count3 = 0;
                        while (xbotstate != XBOTSTATE.XBOT_STOPPED)
                        {
                            Thread.Sleep(50);
                            xbotStatus = _xbotCommand.GetXbotStatus(xbotId);

                            count3++;
                            if (count3 > 100)
                            {
                                Status.isEnabledX = false;
                                _xbotCommand.StopMotion(xbotId);
                                return false; // 超时未停止
                            }
                        }
                        MessageBox.Show("xbot stopped");
                        return true; // 出现故障停止
                    default:
                        return false; // 未知状态类型
                }
            }
            catch (Exception ex)
            {

                return false; // 发生异常时返回 false
            }
        }
        //激活
        public void ActiveXbot()
        {
            if (bPMConnected == true)
            {
                //Activate XBOTs everywhere (in all zones) on the flyway
                Console.WriteLine("Activate all xbots");
                rtnVal = _xbotCommand.ActivateXBOTS();
                // attemptedActivation = true;  //only attempt to send the Activate xbots command once
                //check the return value to see if the command was accepted
                if (rtnVal != PMCRTN.ALLOK)
                {
                    MessageBox.Show("Failed to Activate XBOTs. Error: " + rtnVal.ToString());
                    //return false;
                }
            }
        }

        //关闭
        public void DeActiveXbots()
        {
            if (bPMConnected == true)
            {
                //Activate XBOTs everywhere (in all zones) on the flyway
                Console.WriteLine("Activate all xbots");
                rtnVal = _xbotCommand.DeactivateXBOTS();
                // attemptedActivation = true;  //only attempt to send the Activate xbots command once
                //check the return value to see if the command was accepted
                if (rtnVal != PMCRTN.ALLOK)
                {
                    Console.WriteLine("Failed to Activate XBOTs. Error: " + rtnVal.ToString());
                    //return false;
                }
            }
        }
        //停止
        public void StopXbotMotion(int xBotId)
        {
            //停止运动
            //we will run the stop xbot motion command first, to bring any xbots that are in motion to a stop. 
            //the stop motion command will also clear any remaining commands in the XBOT's motion buffer
            IsCancelled = true;
            _xbotCommand.StopMotion(0); //sending 0 for XBOT id here means all XBOTs will be stopped
        }


        //线性移动
        //Linear motion to starting position of X=120mm, Y=180mm
        //breaking down the parameters of this command:
        //command label = 100. It will be possible to an XBOT which command it is currently executing, the xbot will be reply with the command label of the command it is executing.
        //the command label does not need to be unique, but the user can choose to make it unique during programming for debugging purposes
        //XBOT ID = xbotID. The ID of the XBOT, determined in a previous step
        //Position mode = POSITIONMODE.ABSOLUTE. The coordinates provided in this command are absolute coordinates. The other option is relative positioning
        //Path Type = LINEARPATHTYTPE.DIRECT. The XBOT will go in a straight line towards its destination. It is also possible to go along X first then Y, or go along Y first then X
        //The final destination x position is 120mm or 0.120m 
        //The final destination y position is 180mm or 0.180m 
        //the final speed is 0 m/s
        //the maximum speed during travel is 1m/s
        //the maximum acceleration is 10m/s^2
        public void LinearMotion2Center()//XY联动到平台中心
        {
            xBot_IDs = _xbotCommand.GetXBotIDS();
            int xbotId = xBot_IDs.XBotCount;
            GV.PMC.XbotId = xbotId;
            _xbotCommand.LinearMotionSI(100, xbotId, POSITIONMODE.ABSOLUTE, LINEARPATHTYPE.DIRECT, 0.120, 0.120, 0, 0.1, 1);//
        }

        //圆弧移动
        //arc motion from (120mm, 180mm) to (120mm, 60mm). The arc is centered around (120, 120), and is 180 degrees counter-clockwise
        //command label = 101
        //XBOT ID = xbotID
        //Arc Mode = ARCMODE.CENTERANGLE. In this mode, we define the arc by specifying the arc center location, and the total degree of rotation
        //Arc Type = ARCTYPE.MAJORARC. This is not a useful parameter for the center+angle arc mode, it's value is ignored
        //Arc Direction = ARCDIRECTION.COUNTERCLOCKWISE. The XBOT is moving along the arc in the counter-clockwise direction
        //Position Mode = POSITIONMODE.ABSOLUTE. The Arc center coordinate is specified in absolute coordinates
        //Arc Center X position = 0.120m
        //Arc Center Y position = 0.120m
        //final speed = 0 m/s
        //max speed = 1 m/s
        //max acceleration = 10 m/s^2
        //Arc Radius = 0m. This is not a useful parameter for the center+angle arc mode, it's value is ignored
        //Rotation angle = 180degrees or pi

        //单轴相对运动，步进
        public void MoveXbotRelative(int xbotID, int axis, double distance, double speed, double accleration, double layer = -1, string strGcode = "")
        {
            //Ch.ExtToPoint(Ch.ACSC_AMF_VELOCITY + Ch.ACSC_AMF_RELATIVE, axis, distance, speed, 0);
            //moveSingle
            // 调用LinearSingleAxisMotionSI方法
            double convertedDis = distance / GV.PMC.M2MM;
            double convertedVel = speed / GV.PMC.M2MM;
            double convertedAcc = accleration / GV.PMC.M2MM;
            AXISNAMES axisName = (AXISNAMES)axis;
            _xbotCommand.LinearSingleAxisMotionSI(cmdLabel: 100, xbotID: xbotID, positionMode: POSITIONMODE.RELATIVE, axis: axisName, targetPos: convertedDis, finalSpeed: 0, maxSpeed: convertedVel, maxAcceleration: convertedAcc, hasPriority: false
            );
        }

        //相对运动，点动
        //点动
        //Z，Rx，Ry,Rz点动
        public void xBotShortJog(int xbotID, int iAxis, double vSet, int operation)
        {
            double convertedVel = vSet / GV.PMC.M2MM;
            SHORTAXIS axisName = (SHORTAXIS)iAxis;
            JOGGINGOPERATION jogoperation = (JOGGINGOPERATION)operation;
            _xbotCommand.ShortAxisJoggingControl(xbotID, axisName, convertedVel, jogoperation);//1:move,0:stop
        }
        //X,Y点动
        public void xBotLongJog(int XbotID, double XSpeed, double YSpeed, int operation)
        {
            //double convertedTarget = target / GV.PMC.M2MM;
            double convertedVelX = XSpeed / GV.PMC.M2MM;
            double convertedVelY = YSpeed / GV.PMC.M2MM;
            JOGGINGOPERATION jogoperation = (JOGGINGOPERATION)operation;
            _xbotCommand.LongAxisJoggingControl(XbotID, convertedVelX, convertedVelY, jogoperation);
        }

        //单轴绝对运动
        public void MoveXbotAbsolute(int xbotID, int axis, double target, double speed, double accleration, double layer = -1, string strGcode = "")
        {
            //Ch.ExtToPoint(Ch.ACSC_AMF_VELOCITY + Ch.ACSC_AMF_RELATIVE, axis, distance, speed, 0);
            //moveSingle]
            double convertedTarget = target / GV.PMC.M2MM;
            double convertedVel = speed / GV.PMC.M2MM;
            double convertedAcc = accleration / GV.PMC.M2MM;

            // 调用LinearSingleAxisMotionSI方法
            AXISNAMES axisName = (AXISNAMES)axis;
            _xbotCommand.LinearSingleAxisMotionSI(cmdLabel: 100, xbotID: xbotID, positionMode: POSITIONMODE.ABSOLUTE, axis: axisName, targetPos: convertedTarget, finalSpeed: 0, maxSpeed: convertedVel, maxAcceleration: convertedAcc, hasPriority: false
            );
        }
        //两轴线段
        public void XbotSegmentLine(int xbotid, double[] FinalPoint, double Velocity, double EndVelocity, double layer = -1, string strGcode = "")
        {
            double[] convertedPoint = new double[2];
            for (int i = 0; i < FinalPoint.Length; i++)
            {
                convertedPoint[i] = FinalPoint[i] / GV.PMC.M2MM; //CenterPoint[0]
            }

            double convertedVel = Velocity / GV.PMC.M2MM;
            double convertedEndVel = EndVelocity / GV.PMC.M2MM;
            _xbotCommand.LinearMotionSI(cmdLabel: 100, xbotID: xbotid, positionMode: POSITIONMODE.ABSOLUTE, LINEARPATHTYPE.DIRECT, targetXmeters: convertedPoint[0], convertedPoint[1], convertedEndVel, convertedVel, convertedVel * 10);

        }
        //圆弧逆时针
        public void XbotSgementArc2(int xbotId, double[] Center, double Angle, double Velocity, double EndVelocity)
        {
            double[] convertedCenter = new double[2];
            for (int i = 0; i < Center.Length; i++)
            {
                convertedCenter[i] = Center[i] / GV.PMC.M2MM; //CenterPoint[0]
            }

            double convertedVel = Velocity / GV.PMC.M2MM;
            double convertedEndVel = EndVelocity / GV.PMC.M2MM;
            //ushort cmdLabel, int xbotID, ARCMODE arcMode, ARCTYPE arcType, ARCDIRECTION arcDir, POSITIONMODE positionMode, double XMeters, double YMeters, double finalSpeedMetersPs, double maxSpeedMetersPs, double maxAccelerationMetersPs2, double radiusMeters, double angleRadians
            _xbotCommand.ArcMotionMetersRadians(cmdLabel: 101, xbotID: xbotId, arcMode: ARCMODE.CENTERANGLE, arcType: ARCTYPE.MAJORARC, positionMode: POSITIONMODE.ABSOLUTE, arcDir: ARCDIRECTION.COUNTERCLOCKWISE, XMeters: convertedCenter[0], YMeters: convertedCenter[1], finalSpeedMetersPs: convertedEndVel, maxSpeedMetersPs: convertedVel, maxAccelerationMetersPs2: convertedVel * 10, radiusMeters: 0, angleRadians: Angle);
        }
        //圆弧顺时针
        public void XbotSgementArc1(int xbotId, double[] Center, double Angle, double Velocity, double EndVelocity)
        {
            double[] convertedCenter = new double[2];
            for (int i = 0; i < Center.Length; i++)
            {
                convertedCenter[i] = Center[i] / GV.PMC.M2MM; //CenterPoint[0]
            }

            double convertedVel = Velocity / GV.PMC.M2MM;
            double convertedEndVel = EndVelocity / GV.PMC.M2MM;
            //ushort cmdLabel, int xbotID, ARCMODE arcMode, ARCTYPE arcType, ARCDIRECTION arcDir, POSITIONMODE positionMode, double XMeters, double YMeters, double finalSpeedMetersPs, double maxSpeedMetersPs, double maxAccelerationMetersPs2, double radiusMeters, double angleRadians
            _xbotCommand.ArcMotionMetersRadians(cmdLabel: 102, xbotID: xbotId, arcMode: ARCMODE.CENTERANGLE, arcType: ARCTYPE.MAJORARC, positionMode: POSITIONMODE.ABSOLUTE, arcDir: ARCDIRECTION.CLOCKWISE, XMeters: convertedCenter[0], YMeters: convertedCenter[1], finalSpeedMetersPs: convertedEndVel, maxSpeedMetersPs: convertedVel, maxAccelerationMetersPs2: convertedVel * 10, radiusMeters: 0, angleRadians: Angle);
        }

        //绝对运动XY联动
        public void MoveXbotXYAbsolute(int xbotID, double targetX, double targetY, double speed, double endSpeed, double accleration, double layer = -1, string strGcode = "")
        {
            double convertedTargetX = targetX / GV.PMC.M2MM;
            double convertedTargetY = targetY / GV.PMC.M2MM;
            double convertedVel = speed / GV.PMC.M2MM;
            double convertedAcc = accleration / GV.PMC.M2MM;
            _xbotCommand.LinearMotionSI(cmdLabel: 103, xbotID: xbotID, positionMode: POSITIONMODE.ABSOLUTE, LINEARPATHTYPE.DIRECT, targetXmeters: convertedTargetX, targetYmeters: convertedTargetY, finalSpeedMetersPs: 0, maxSpeedMetersPs: convertedVel, maxAccelerationMetersPs2: convertedAcc);
        }
        //旋转
        public void MoveXbotXYrotary(int xboID, int axis, double targetRads, double speed)
        {
            double currentXpos = Status.fXbotPosXa / GV.PMC.M2MM;
            double currentYpos = Status.fXbotPosYa / GV.PMC.M2MM;
            double currentZpos = Status.fXbotPosZa / GV.PMC.M2MM;
            double Rx = GetXbotPosition(GV.PMC.arrXBotIds[0], GV.PMC.Rx);
            double Ry = GetXbotPosition(GV.PMC.arrXBotIds[0], GV.PMC.Ry);
            double currentRz = Status.fXbotRzA;
            speed = speed / GV.PMC.M2MM;
            if (axis == GV.PMC.Rx)
            {
                Rx = targetRads;             
            }
            else if (axis == GV.PMC.Ry)
            {             
                Ry = targetRads;
            }

            _xbotCommand.SixDofMotionSI(cmdLabel: 104, xbotID: xboID, currentXpos, currentYpos, currentZpos, Rx, Ry, 0, speed, speed * 10, zMaxSpeed: speed, rxMaxSpeed: speed, ryMaxSpeed: speed, rzMaxSpeed: speed);
        }
        //六轴
        public void MoveXbotSixDof(int xbotId, double posX, double posY, double posZ, double RotX, double RotY, double RotZ)
        {
            double targetX = posX / GV.PMC.M2MM;
            double targetY = posY / GV.PMC.M2MM;
            double taegetZ = posZ / GV.PMC.M2MM;
            double targetRx = RotX / GV.PMC.Rad2Degree;
            double targetRy = RotY / GV.PMC.Rad2Degree;
            double targetRz = RotZ / GV.PMC.Rad2Degree;
            _xbotCommand.SixDofMotionSI(cmdLabel: 105, xbotID: xbotId, targetX, targetY, taegetZ, targetRx, targetRy, targetRz, 5, 5 * 10, zMaxSpeed: 5, rxMaxSpeed: 5, ryMaxSpeed: 5, rzMaxSpeed: 5);
        }
        //距离触发，满足条件后再进行下一个指令,多动子
        WaitUntilTriggerParams waitUntilStartTrigger = new WaitUntilTriggerParams();
        public void WaitXbotMoveBeyond(int xbotId, double threshold)
        {
            waitUntilStartTrigger.triggerXbotID = xbotId;
            waitUntilStartTrigger.displacementTriggerType = TRIGGERDISPLACEMENTTYPE.GREATER_THAN;
            waitUntilStartTrigger.displacementTriggerMode = TRIGGERDISPLACEMENTMODE.AX_BY;
            waitUntilStartTrigger.displacementThresholdMeters = threshold;

            _xbotCommand.WaitUntil(0, xbotId, TRIGGERSOURCE.DISPLACEMENT, waitUntilStartTrigger);
        }

        //时间触发，暂停时间，继续下一个运动
        public void WaitXbotPause(int xbotId, double threshold)
        {
            waitUntilStartTrigger.triggerXbotID = xbotId;
            waitUntilStartTrigger.delaySecs = threshold;
            PMCRTN rtnVal = _xbotCommand.WaitUntil(0, xbotId, TRIGGERSOURCE.TIME_DELAY, waitUntilStartTrigger);
            if (rtnVal != PMCRTN.ALLOK)
            {
                MessageBox.Show(" Error: " + rtnVal.ToString());
            }
        }
        //等待指令完成
        public void WaitXbotCmdFinish(ushort cmdLabel, int xbotId)
        {
            waitUntilStartTrigger.triggerXbotID = xbotId;
            waitUntilStartTrigger.triggerCmdLabel = cmdLabel;
            // triggerParams.CmdLabelTriggerType = TRIGGERCMDLABELTYPE.CMD_EXECUTING;
            waitUntilStartTrigger.CmdLabelTriggerType = TRIGGERCMDLABELTYPE.CMD_FINISH;
            waitUntilStartTrigger.triggerCmdType = TRIGGERCMDTYPE.MOTION_COMMAND;
            PMCRTN rtnVal = _xbotCommand.WaitUntil(0, xbotId, TRIGGERSOURCE.CMD_LABEL, waitUntilStartTrigger);
            if (rtnVal != PMCRTN.ALLOK)
            {
                MessageBox.Show("Failed to run the wait until command. Error: " + rtnVal.ToString());
            }
        }
        //waitMoveEnd
        public async Task WaitXbotMoveEndAsync(int xbotId)
        {
            bool isMoving = true;
            int timeoutCounter = 0;
            const int maxTimeoutAttempts = 100;
            const int checkIntervalMs = 50;

            while (isMoving && timeoutCounter < maxTimeoutAttempts)
            {
                isMoving = CheckXbotState(xbotId, 3);
                if (isMoving)
                {
                    await Task.Delay(checkIntervalMs);
                    timeoutCounter++;
                }
            }

            if (timeoutCounter >= maxTimeoutAttempts)
            {
                _xbotCommand.StopMotion(xbotId);
                MessageBox.Show("XBot 运动超时未停止，已强制停止！");
            }
            Status.isMovingX = false;
        }

        //缓冲区操作
        public void MotionBufferOperate(int xbotid, int option)
        {
            switch (option)
            {
                case 0://锁定buffer，可以接受命令但不执行
                    _xbotCommand.MotionBufferControl(xbotid, MOTIONBUFFEROPTIONS.BLOCKBUFFER);

                    break;
                case 1:
                    //release the motion buffer so the XBOTs can actually run the commands
                    _xbotCommand.MotionBufferControl(xbotid, MOTIONBUFFEROPTIONS.RELEASEBUFFER);
                    break;
                case 2:
                    _xbotCommand.MotionBufferControl(xbotid, MOTIONBUFFEROPTIONS.CLEARBUFFER);
                    break;

            }
        }
        public void sixTry()
        {
            _xbotCommand.SixDofMotionSI((ushort)(1), GV.PMC.XbotId, 330 / GV.PMC.M2MM, 150 / GV.PMC.M2MM, 1 / GV.PMC.M2MM, 0, 0, 0);
            int[] xbotIds = { 0 };
            int[] currentTrajectoryId = { 1 };
            _xbotCommand.TrajectoryActivation(1, 1, xbotIds, currentTrajectoryId);
        }
        // GV.PrintingObj._xbotCommand.SixDofMotionSI((ushort)(trajectoryIndex), xbotId, targetX / GV.PMC.M2MM, targetY / GV.PMC.M2MM, targetZ / GV.PMC.M2MM, 0, 0, 0);

        //横向运动为一组，纵向运动为一组，运行轨迹ID,设置线间距，固定范围是180x180，每12mm一个group
        public void ActivateTrajectory_old(int xbotId, int[] trajectoryIDs, double interval, bool runY, double[] zMove)
        {

            ushort cmdLabel = 1;
            int xbotCount = 1;                            //动子数量
            int[] xbotIds = new int[1];
            int[] currentTrajectoryId = new int[1];
            bool isMovingForward = true;

            double xStart = 150.0, yStart = 150.0, zStart = 1;//测量基点的起始位置
            double xRange = 180.0, yRange = 180.0;            //路线范围固定为180x180
            double targetX, targetY, targetZ;
            double targetRx = 0, targetRy = 0, targetRz = 0;

            int totalGroups = (int)Math.Ceiling(yRange / 12);        //固定12mm间隔为一个来回组
            int segmentsPerGroup = (int)Math.Round(12 / interval); //每一组内的线段数
            int trajectoryIndex = 0;                     //从组内第一个ID开始运动
            xbotIds[0] = GV.PMC.XbotId;
            for (int i = 0; i < totalGroups; i++)        //每组占用一段插补路径
            {
                //trajectoryIndex = i * 2 ;//运行完一组，切换下一段插补路径,
                for (int j = 0; j < segmentsPerGroup; j++)//在组内来回运动,不足处需补齐下一段的
                {
                    //计算折线处目标点
                    if (runY)//在y方向上来回运动，x方向间隔步进
                    {
                        //targetX = xStart + (i * 12) + ((j + 1) * interval);//不能被12整除会使得下一组线间距不同
                        targetX = xStart + (segmentsPerGroup * i + j + 1) * interval;//当恰好落在边界会有bug                       
                        targetY = isMovingForward ? yStart + yRange : yStart;

                        double groupIndexDouble = ((targetX - 150.0) / 12.0);         //ID号会在被整除的部分乱序
                        int groupIndex = (int)Math.Floor(groupIndexDouble);         // 记录在哪个组

                        // 处理正好落在组边界的情况（被12整除）
                        if (Math.Abs(groupIndexDouble - groupIndex) < double.Epsilon && groupIndex > 0)
                        {
                            groupIndex--; // 取前一个组的序号
                        }
                        trajectoryIndex = groupIndex * 2;
                    }
                    else
                    {
                        targetX = isMovingForward ? xStart + xRange : xStart;
                        //targetY = yStart + (i * 12) + ((j + 1) * interval);
                        targetY = yStart + (segmentsPerGroup * i + j + 1) * interval;
                        double groupIndexDouble = ((targetY - 150.0) / 12.0);         //ID号会在被整除的部分乱序
                        int groupIndex = (int)Math.Floor(groupIndexDouble);

                        // 处理正好落在组边界的情况（被12整除）
                        if (Math.Abs(groupIndexDouble - groupIndex) < double.Epsilon && groupIndex > 0)
                        {
                            groupIndex--; // 取前一个组的序号
                        }
                        trajectoryIndex = groupIndex * 2;
                    }
                    // 去或回来回走线           
                    if (isMovingForward)
                    {
                        currentTrajectoryId[0] = trajectoryIDs[trajectoryIndex];//id 1
                        targetZ = zMove[trajectoryIndex];
                        _xbotCommand.TrajectoryActivation((ushort)(trajectoryIndex), xbotCount, xbotIds, currentTrajectoryId);
                        //CheckXbotState(GV.PMC.XbotId, 1);
                        _xbotCommand.SixDofMotionSI((ushort)(trajectoryIndex + 1), xbotId, targetX / GV.PMC.M2MM, targetY / GV.PMC.M2MM, targetZ / GV.PMC.M2MM, 0, 0, 0, 0.1, 1, 0.05, 0, 0, 0);
                        //CheckXbotState(GV.PMC.XbotId, 1);
                    }
                    else
                    {
                        currentTrajectoryId[0] = trajectoryIDs[trajectoryIndex + 1];//id 2
                        targetZ = zMove[trajectoryIndex + 1];
                        _xbotCommand.TrajectoryActivation((ushort)(trajectoryIndex + 2), xbotCount, xbotIds, currentTrajectoryId);
                        //CheckXbotState(GV.PMC.XbotId, 1);
                        _xbotCommand.SixDofMotionSI((ushort)(trajectoryIndex + 3), xbotId, targetX / GV.PMC.M2MM, targetY / GV.PMC.M2MM, targetZ / GV.PMC.M2MM, 0, 0, 0, 0.1, 1, 0.05, 0, 0, 0);
                        //CheckXbotState(GV.PMC.XbotId, 1);
                    }
                    isMovingForward = !isMovingForward;
                    //检查buffer状态
                    //CheckMotionBufferCount(GV.PMC.XbotId);
                }
                //一组结束检查buffer状态
                CheckMotionBufferCount(GV.PMC.XbotId);
            }
        }
        public void ActivateTrajectory(int xbotId, int[] trajectoryIDs, double interval, bool runY, double[] zMove)
        {
            // 参数验证
            if (interval <= 0.001) // 最小间隔限制
                throw new ArgumentException("Interval must be greater than 0.001mm");

            const double xStart = 150.0, yStart = 150.0;
            const double xRange = 180.0, yRange = 180.0;
            const double groupLength = 12.0; // 每组固定长度12mm
            const double epsilon = 0.0001;   // 浮点比较精度

            // 计算总线段数（使用Math.Ceiling确保完全覆盖）
            int totalSegments = (int)Math.Ceiling(yRange / interval);

            // 初始化运动参数
            int[] xbotIds = { xbotId };
            int[] currentTrajectoryId = new int[1];
            bool isMovingForward = true;
            double accumulatedPos = 0; // 累计位置跟踪

            try
            {
                for (int segIdx = 0; segIdx < totalSegments; segIdx++)//
                {
                    // 计算当前绝对位置（避免浮点累积误差）
                    accumulatedPos = (segIdx + 1) * interval;
                    double currentPos = xStart + accumulatedPos;

                    // 计算目标位置
                    double targetX, targetY;
                    double targetZ;
                    if (runY)
                    {
                        targetX = currentPos;
                        targetY = isMovingForward ? yStart + yRange : yStart;
                    }
                    else
                    {
                        targetY = currentPos;
                        targetX = isMovingForward ? xStart + xRange : xStart;
                    }

                    // 精确计算组索引（处理小间隔情况）
                    double groupPos = accumulatedPos - epsilon; // 防止边界误差
                    int groupIndex = (int)(groupPos / groupLength);

                    // 轨迹索引计算（每组2个ID）
                    int trajectoryIndex = Math.Min(groupIndex * 2, trajectoryIDs.Length - 2);

                    // 执行运动控制
                    int activeIndex = isMovingForward ? trajectoryIndex : trajectoryIndex + 1;
                    currentTrajectoryId[0] = trajectoryIDs[activeIndex];
                    targetZ = zMove[activeIndex];

                    // 运动指令（添加了运动参数约束）
                    _xbotCommand.TrajectoryActivation(
                        (ushort)(trajectoryIndex + (isMovingForward ? 0 : 2)),
                        1, xbotIds, currentTrajectoryId);

                    _xbotCommand.SixDofMotionSI(
                        (ushort)(trajectoryIndex + (isMovingForward ? 1 : 3)),
                        xbotId,
                        Math.Round(targetX / GV.PMC.M2MM, 6), // 位置精度控制
                        Math.Round(targetY / GV.PMC.M2MM, 6),
                        Math.Round(targetZ / GV.PMC.M2MM, 6),
                        0, 0, 0, 0.1, 1, 0.05, 0, 0, 0);

                    // 切换方向并检查缓冲区
                    isMovingForward = !isMovingForward;
                    if (segIdx % 25 == 0) CheckMotionBufferCount(xbotId); // 每25个线段检查一次
                }
            }
            catch (Exception ex)
            {
                // 错误处理（建议添加日志记录）
                Debug.WriteLine($"Motion error: {ex.Message}");
                throw new ApplicationException("Trajectory activation failed", ex);
            }
            finally
            {
                // 最终缓冲区检查
                CheckMotionBufferCount(xbotId);
            }
        }
        public class TrajectoryAxesParams
        {
            public bool EnableX { get; set; } = true;
            public bool EnableY { get; set; } = true;
            public bool EnableZ { get; set; } = true;
            public bool EnableRx { get; set; } = true;
            public bool EnableRy { get; set; } = true;
            public bool EnableRz { get; set; } = true;
            public bool EnableDigitalInput { get; set; } = false;
        }
        //更新采集到的数据
        public void SetTrajectories(int trajectoryID, double interval, string trajectory, TrajectoryAxesParams axesParams = null)
        {
            //TrajectoryAxesDefinition axesDefinition = new TrajectoryAxesDefinition();
            //bool[] axisDef = new bool[7] { axesDefinition.XAxis, axesDefinition.YAxis, axesDefinition.ZAxis, axesDefinition.RxAxis, axesDefinition.RyAxis, axesDefinition.RzAxis, axesDefinition.DigitalInput };
            axesParams = axesParams ?? new TrajectoryAxesParams();
            TrajectoryAxesDefinition axesDefinition = new TrajectoryAxesDefinition
            {
                XAxis = axesParams.EnableX,
                YAxis = axesParams.EnableY,
                ZAxis = axesParams.EnableZ,
                RxAxis = axesParams.EnableRx,
                RyAxis = axesParams.EnableRy,
                RzAxis = axesParams.EnableRz,
                DigitalInput = axesParams.EnableDigitalInput,
            };
            //bool[] axesDefinition = { true, true, true, true, true, true, false };
            _systemCommand.SetTrajectory(trajectoryID, interval, axesDefinition, trajectory);
        }
        /*按照设置点位进行依次写入补偿数据，最多可写入450x450个点位，覆盖打印范围
         * 当动子移动到某个设置好的点位后，会主动代入补偿数据，而不会在反馈数据中体现*/
        public void SetGridTable(double xBottom, double yBottom, double xStep, double yStep, int xCount, int yCount, double[,,] CompensateData)
        {

            try
            {
                PMCRTN rtn = _xbotCommand.DeactivateXBOTS();
                if (rtn != PMCRTN.ALLOK)
                {
                    MessageBox.Show("Failed to Deactive the xBot!");
                    return;
                }
                // 验证输入参数有效性
                if (CompensateData == null || xCount <= 0 || yCount <= 0)
                {
                    MessageBox.Show("Invalid input parameters!");
                    return;
                }

                // 将位置和步长从mm转换为m (除以1000)
                double xBottom_m = xBottom / GV.PMC.M2MM;
                double yBottom_m = yBottom / GV.PMC.M2MM;
                double xStep_m = xStep / GV.PMC.M2MM;
                double yStep_m = yStep / GV.PMC.M2MM;

                // 创建新的补偿数据数组并将值从mm转换为m
                double[,,] compensateData_m = new double[6, CompensateData.GetLength(1), CompensateData.GetLength(2)];

                for (int i = 0; i < CompensateData.GetLength(1); i++)
                {
                    for (int j = 0; j < CompensateData.GetLength(2); j++)
                    {
                        compensateData_m[2, i, j] = CompensateData[2, i, j] / GV.PMC.M2MM;
                    }
                }

                // 调用系统命令，使用转换后的m单位参数
                rtn = _systemCommand.SetGridTable(
                   xBottom_m, yBottom_m, xStep_m, yStep_m,
                   xCount, yCount, COMPENSATIONTYPE.SENSING_COMP, compensateData_m);

                if (rtn != PMCRTN.ALLOK)
                {
                    MessageBox.Show("Failed to write the grid compensation table!");
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in SetGridTable: {ex.Message}");
            }
        }
        //记录相对运动参数
        public void qMoveXbotRelative(int xbotID, int axis, double distance, double speed, double accleration, double layer = -1, string strGcode = "")
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            //cds.CmdName = DataManagement.OptType.MoveAxisRelative;

            //cds.Para1 = xbotID.ToString();
            //cds.Para2 = axis.ToString();
            //cds.Para3 = distance.ToString();
            //cds.Para4 = speed.ToString();
            //cds.Para5 = accleration.ToString();

            //cds.Para9 = layer.ToString();
            //cds.Para10 = strGcode;
            //cds.EstimateTime = Math.Abs(distance) / speed;
            //cds.EstimateTime += 0.2; // 加减速多消耗的时间
            //DataObj.InsertCmdData(cds);
        }
        //单轴绝对移动移动
        public void qMovexBotTo(int xbotID, int axis, double target, double speed, double accleration, double layer = -1, string strGcode = "")
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.MoveAxisTo;

            cds.Para1 = xbotID.ToString();
            cds.Para2 = axis.ToString();
            cds.Para3 = target.ToString();
            cds.Para4 = speed.ToString();
            cds.Para5 = accleration.ToString();

            cds.Para9 = layer.ToString();
            cds.Para10 = strGcode;

            cds.EstimateTime = Math.Abs(target) / speed;//PMC---target-pos
            cds.EstimateTime += 0.2; // 加减速多消耗的时间
            DataObj.InsertCmdData(cds);
        }

        //终点不为0
        public void qMoveXbotXYTo(int xbotID, double xTarget, double yTarget, double speed, double accleration, double xStart = 60, double yStart = 60, double layer = -1, string strGcode = "")
        {
            DataManagement.CmdDataStruct cds = new DataManagement.CmdDataStruct();
            cds.CmdName = DataManagement.OptType.MoveXYTo;
            cds.Para1 = xTarget.ToString();
            cds.Para2 = yTarget.ToString();
            cds.Para3 = speed.ToString();
            cds.Para4 = xbotID.ToString();
            cds.Para9 = layer.ToString();
            cds.Para10 = strGcode;


            double s = Math.Sqrt(Math.Pow(xTarget - xStart, 2) + Math.Pow(yTarget - yStart, 2));
            cds.EstimateTime = s / speed;
            cds.EstimateTime += 0.2;
            DataObj.InsertCmdData(cds);
        }
        //获取buffer中指令的数量
        internal int GetMotionBufferStatus(int xbotId)
        {
            int numCmd;
            xbotStatus = _xbotCommand.GetXbotStatus(xbotId);
            numCmd = xbotStatus.bufferedMotionCount;
            return numCmd;
        }
        public void CheckMotionBufferCount(int xbotId)
        {
            int motionBufferrCmd;
            motionBufferrCmd = GetMotionBufferStatus(xbotId);
            //如果控制器缓冲指令空间不足，则暂停添加指令：
            while (motionBufferrCmd > 490 && motionBufferrCmd <= 500 && !IsCancelled)
            {
                //MotionBufferOperate(xbotId , 1);//锁定buffer不再添加
                Thread.Sleep(GV.Command_Block);
                motionBufferrCmd = GetMotionBufferStatus(xbotId);
            }
        }


    }
}
