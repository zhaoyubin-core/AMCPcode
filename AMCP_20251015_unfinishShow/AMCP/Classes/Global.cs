#define EngineFive
//#define Yunco
//#define QiSUO
//#define GLM221
//#define Yunco
//#define Sansuo
//#define SansuoHP
//#define SansuoHSHS
//#define Qisuo

using Advantech.Adam;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static OpenCvSharp.Stitcher;
using static System.Runtime.CompilerServices.RuntimeHelpers;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
//using System.Diagnostics;//maybe useless
//using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
//using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AMCP
{
    public static class GV
    {
        public static bool bPausePrint = false;
        public static bool bAllowedClose = false;
        public static FrmMain frmMain = null;
        //print steps forms
        public static FrmPrintStep1 frmPrintStep1 = null;  // 系统初始化
        public static FrmPrintStep2 frmPrintStep2 = null;  // 路径文件生成/加载
        public static FrmPrintStep3 frmPrintStep3 = null;  // 工艺文件生成/加载      
        public static FrmPrintStep4 frmPrintStep4 = null;  // 打印准备/预览
        public static FrmPrintGuide frmPrintGuide = null;  // 打印向导

        public static FrmPathTrace frmPathTrace = null;    // 路径跟踪
        public static FrmMotionAdjust frmMotionAdjust = null;
        public static FrmNozzleCalibrate frmNozzleCalibrate = null;
        public static FrmStatusMonitor frmStatusMonitor = null;
        public static FrmImmStop frmImmStop = null; //紧急停止开关界面
        public static FrmPathRun frmBasicTest = null;
        public static FrmPathPreview frmPathPreview = null;
        public static FrmDiagnose frmDiagnose = null;

        //Hardware control forms
        public static FrmDCPower frmDCPower = null;
        public static FrmSetPressure frmSetPressure = null;
        public static FrmSyringePump frmSyringePump = null;
        public static FrmTemperature frmTemperature = null;
        public static FrmTemptTableCtrl frmTemptTableCtrl = null;   
        public static FrmRotaryValveCtrl frmRotaryValveCtrl = null;   //螺杆阀
        public static FrmWeightCtrl frmWeightCtrl = null;

        public static ParaSetting paraSettingObj = null;         // 参数设置类对象
        public static DataManagement dataManagementObj = null;   // 数据管理类对象
        public static ACSMotionControl PrintingObj = null; //PrintingControl PrintingControlObj;

        public static ConnectMode connMode = ConnectMode.Disconnect;

        public static BackgroundWorker bgWorker = new BackgroundWorker();  // 后台线程：状态监控
        public static BackgroundWorker bgWorker2 = new BackgroundWorker(); // 后台线程：控制指令执行
        //新增后台
        public static BackgroundWorker monitorWorker = new BackgroundWorker();//新增：线程指令监控

        public static SerialPort seriPort1 = new SerialPort("COM16"); // 温度传感器
        public static SerialPort seriPort2 = new SerialPort("COM2");  // 压力传感器
        public static SerialPort seriPort3 = new SerialPort("COM14"); // 高压电源

        public static SerialPort seriPort4 = new SerialPort("COM4", 1200, Parity.Even);

        //serialPortE.PortName = AvailableComCbobox.SelectedItem.ToString();
        //serialPortE.BaudRate = 1200;            //设置当前波特率
        //serialPortE.Parity = Parity.Even;       //设置当前校验位
        //serialPortE.DataBits = 8;               //设置当前数据位
        //serialPortE.StopBits = StopBits.One;    //设置当前停止位   

        //传感器的测量值
        public static double valueOutPressure1;          // 气压阀1的测量值(左）
        public static double valueOutPressure2;          // 气压阀2的测量值（左位移笔）
        public static double valueOutPressure3;          // 气压阀3的测量值（右）
        public static double valueOutPressure4;          // 气压阀4的测量值（右位移笔）

        public static double valueSetPressure1;          // 气压阀1的设定值
        public static double valueSetPressure2;          // 气压阀2的设定值
        public static double valueSetPressure3;          // 气压阀3的设定值
        public static double valueSetPressure4;          // 气压阀4的设定值

        public static double valueDisplacementSensor;    // 笔式位移传感器测量值（左）
        public static double valueDisplacementSensor_ACS;    // 笔式位移传感器测量值(ACS)（左）
        public static double valueDisplacementSensorB;    // 笔式位移传感器测量值(右）
        public static double valueDisplacementSensor_ACSB;    // 笔式位移传感器测量值(ACS)（右）

        public static double valueDisplacementSensor_rA;  // 光谱共焦位移传感器测量值记录
        public static double valueDisplacementSensor_rB;  // 光谱共焦位移传感器测量值记录

        public static string strTemperature = "";


        public static double xFed;  // 当前位置
        public static double yFed;  // 当前位置
        public static double zFed;  // 当前位置
        public static double z1Fed;
        public static double z2Fed;

        public static int layerCurrent; // 当前打印层数

        public static double xLast;  // 上一次位置
        public static double yLast;  // 上一次位置
        public static double zLast;  // 上一次位置
        public static double z1Last;
        public static double z2Last;

        public static double printTimeLast = 0;    // 上次打印耗时

        public static double xSet;          // 设定位置
        public static double ySet;          // 设定位置
        public static double zSet;          // 设定位置
        public static double z1Set;
        public static double z2Set;
        public static double vSet = 100;          // 设定度速

        //龙门主轴
        public static double X_MIN = 0;     // X轴下限
        public static double Y_MIN = 0;     // Y轴下限
        public static double Z_MIN = 0;     // Z轴下限

        

        public static double Z_TOP = 0;     // 提针位置
        public static double Z_BOTTOM = 50; // 下针位置
        public static double Za_BOTTOM = 0; // 滑台a下针位置
        public static double Zb_BOTTOM = 0; // 滑台b下针位置 

        public static double X_INIT = 100;  // 初始位置X坐标
        public static double Y_INIT = 100;  // 初始位置Y坐标
        public static double Z_INIT = 10;   // 初始位置Z坐标

        public static double xStart = 0;
        public static double yStart = 0;
        public static double zStart = 0;

        public static double Z_INTERVAL = 0.1; // 初始针头高度间隙

        public static double Z_OFFSET = 0; // 打印基底与对针传感器之间的高度差

        public static double X_ADJUST = 70;  // 针尖对针位置X坐标
        public static double Y_ADJUST = 150; // 针尖对针位置Y坐标
        public static double Z_ADJUST = 50;  // 针尖对针位置Z坐标

        public static double X_ADJUST_B = 70;  // 针尖对针位置X坐标
        public static double Y_ADJUST_B = 150; // 针尖对针位置Y坐标
        public static double Z_ADJUST_B = 50;  // 针尖对针位置Z坐标

        public static double OP1_X = 0;     // 光谱共焦位移传感器对到动子中心的X坐标
        public static double OP1_Y = 0;     // 光谱共焦位移传感器对到动子中心的Y坐标
        public static double OP1_Z = 0;     // 光谱共焦位移传感器对到动子中心的Z坐标
        public static double OP1_dX = 0;    //调平时的左右范围
        public static double OP1_dY = 0;    //调平时的前后测量范围
        public static double adjustRxA = 0; //调平应用参数
        public static double adjustRxB = 0;
        public static double adjustRyA = 0;
        public static double adjustRyB = 0;
        public static double D_OP1 = 0;     // 光谱共焦传感器调平后距离动子的距离

        public static double OP2_X = 0;     // 相机对焦到针尖的X坐标
        public static double OP2_Y = 0;     // 相机对焦到针尖的Y坐标
        public static double OP2_Z = 0;     // 相机对焦到针尖的Z坐标

        public static double D_OP2 = 0;     // 相机测量后针尖到基底的距离

        //public static double OP3_X = 0;     // 位移笔碰打印基底测量点（1号工位中心位置）的X坐标
        //public static double OP3_Y = 0;     // 位移笔碰打印基底测量点（1号工位中心位置）的Y坐标
        //public static double OP3_Z = 0;     // 位移笔碰打印基底测量点（1号工位中心位置）的Z坐标

        public static double D_INIT = 0.1;     // 初始间距d*（针头高于基底的初始距离）

        public static double dX_AB = 0;     // AB两个喷头针尖对针位置X坐标差值
        public static double dY_AB = 0;     // AB两个喷头针尖对针位置Y坐标差值
        public static double dZ_AB = 0;     // AB两个喷头针尖对针位置Z坐标差值

        public static double dX_Camera = 0;     // 针头与摄像头的X坐标差值
        public static double dY_Camera = 0;     // 针头与摄像头的Y坐标差值
        public static double dZ_Camera = 0;     // 针头与摄像头的Z坐标差值

        public static double dX_Clean = 0;   // 清洁装置与对针位置的X坐标差值
        public static double dY_Clean = 0;  // 清洁装置与对针位置的Y坐标差值
        public static double dZ_Clean = 0;  // 清洁装置与对针位置的Z坐标差值

        public static double dX_Trans = 0;  //机械臂转运龙门需要腾出的X位置
        public static double dY_Trans = 0;  //机械臂转运龙门需要腾出的Y位置
        public static double dZ_Trans = 0;  //机械臂转运龙门需要腾出的Z位置

        public static double X_NOZZLE_CHANGE = 10;  // 针尖对针位置X坐标
        public static double Y_NOZZLE_CHANGE = 150; // 针尖对针位置Y坐标
        public static double Z_NOZZLE_CHANGE = 5;  // 针尖对针位置Z坐标


        public static bool followCurPosition = false;
        public static bool drawSwitch = false;
        public static int refreshInterval = 40;//指令输入间隔
        public static int keepPoints = 5000;
        public static AxesType axesType = AxesType.XY_Z;

        public static Series seriXY, seriXY1, seriZ0, seriZ01;
        public static Series seriXZ, seriXZ1, seriY0, seriY01;
        public static Series seriYZ, seriX0, seriYZ1, seriX01;

        //public static Series seriTvelX, seriTvelY, seriTvelZ, seriTvelG;

        public static string TechParaFileName;
        public static string PathFileName; //单工件时的打印文件名
        //public static string PathFileNamesA;
        //public static string PathFileNamesB;
        public static string ControlLogFileName;
        public static string MonitorLogFileName;
        public static int PrintMode = 0;                //喷头打印模式0:单喷头，1多喷头
        // 打印工位相关设置
        public static int ContiniousPrintCount = 4;          // 最大打印工位数量

        public static int indexPrintingPos = -1;
        public static string strPrintPosStatus = "";//显示打印状态
        public static bool[] arrPrintPosSelected = new bool[] { true, true };//两个工位是否选择打印
        public static List<bool> listPrintPosA = new List<bool> { false, false, false, false, false };//stageA print
        public static List<bool> listPrintPosB = new List<bool> { false, false, false, false, false };//  
        public static List<bool> listPrintStageSelected = new List<bool>();//选择的工位
        //public static string[] arrPathFileName = new string[] { "", "", "", "", "", "" };
        public static List<string> listPathFileNameA = new List<string>();//存储路径文件名
        public static List<string> listPathFileNameB = new List<string>();
        public static List<double> listTargetZ = new List<double> ();//目标下针位置,主Z轴
        public static List<double> listSlightZa = new List<double> ();//目标下针位置,小Z1轴
        public static List<double> listSlightZb = new List<double> ();//目标下针位置,小Z2轴
        public static List<double> listRotateSpeedA = new List<double>();//配置序号的螺杆转速1
        public static List<double> listRotateSpeedB = new List<double>();//配置序号的螺杆转速2
        public static List<double> listAirPressureA = new List<double>();//配置挤出气压1
        public static List<double> listAirPressureB = new List<double>();//2
        public static List<double> listPrintSpeed = new List<double>();//打印速度

        public static long[] arrStartTime = new long[] { 0, 0, 0, 0, 0, 0 };
        public static long[] arrEndTime = new long[] { 0, 0, 0, 0, 0, 0 };
        public static string[] arrStartTimeStamp = new string[] { "", "", "", "", "", "" };
        public static double[] arrMassFlowRate = new double[] { 0, 0, 0, 0, 0, 0 };
        public static string[] arrInkCode = new string[] { "", "", "", "", "", "" };       // 墨水编号
        public static string[] arrSampleCode = new string[] { "", "", "", "", "", "" };     // 片材编号
        public static string[] arrDebubbling = new string[] { "", "", "", "", "", "" };     // 脱泡确认 
        public static string[] arrPrintBaseNum = new string[] { "", "", "", "", "", "" };     // 基底编号
        public static string[] arrOperator = new string[] { "", "", "", "", "", "" };     // 操作
        public static string[] arrReviewer = new string[] { "", "", "", "", "", "" };     // 复核 

        public static double printingSpeedSet = 10.0;   //打印速度设定
        public static double jumpSpeedSet = 20;         //空跳速度设定

        public static byte axisPump = 0;                // 注射泵的控制轴
        public static int portExtrude = -2;//4             // 出丝的端口号数据
        public static int timeExtrudeInAdvance0 = 0;     // 提前打开点胶机A的时间(ms)（当前为在出丝前停顿）
        public static int timeExtrudeInAdvance1 = 0;     // 提前打开点胶机B的时间(ms)（AB耦合，未使用）

        public static int timeCloseExtrudeInAdvance = 0; //提前关点胶机时间（ms）（开发中）

        public static double speedRotaryValueA = 60;      //螺杆阀转速a
        public static double speedRotaryValueB = 60;      //螺杆阀转速b
        public static double extrudePressValueA = 600;         //气压打开数值1
        public static double extrudePressValueB = 600;         //气压打开数值2

        public static int Command_Num = 20;             //单次刷新指令数量
        public static int Command_Block = 50;           //插补指令缓冲区线程阻塞时间
        public static int ConRecord_interval = 50;      //控制指令记录至日志间隔数量
        public static int Monitor_interval = 5;         //监测间隔时间，x40ms
        public static int MonRecord_interval = 200;     //监测打印信息记录到日志间隔时间

        public static bool bInitialized = false;

        public static SerialPort serialPortE;
        public static bool photoEnabled = false;//层间拍照
        public static bool noStopExtrude = false;//层间不停丝
        public static bool timeRotation = false;//时间转速控制
        public static bool layerRotation = false;//层号转速控制
        public static bool setLayerRotation = false;//是否使用表格最后一列的转速
        public static int lenAdvanced = 0;
        public static bool Isextrude = false;
        public static bool nozzleClean = false;//针头擦拭
        public static bool basalTransport = false;//机械臂转运
        public static bool nozzleUp = false;//打印完抬针

        public static bool pathprinting = false;//曲面打印
        public static bool Scilence = false;//是否启用蜂鸣器

        public static string Commands = "";//发送的指令
        public static List<List<int>> allG1Counts = new List<List<int>>();//存储g1数量
        public static string ComRotaryValve = "COM9";   // 螺杆阀模块COM口：转485接口，用于控制和反馈螺杆阀的转速

        public static Random rand = new Random();

#if (EngineFive)
        public const int X = 3;     // X轴轴号
        public const int Y = 0;     // Y轴轴号
        public const int Z = 6;     // Z轴轴号
     
        public const string IpAddr = "192.168.1.100";
        public static double X_MAX = 580;       // X轴上限
        public static double Y_MAX = 300;       // Y轴上限
        public static double Z_MAX = 100;       // Z轴上限

        //平面电机
        public static double stageXa = 120;//A平台中心
        public static double stageXb = 460;//B平台中心
        public static double stageY = 120;//定子平台的中心位置
        public static double sizeFlyWayS3 = 240;//定子尺寸;240*240*70
        public static double sizeM3 = 240; //动子M3-13-SD 240*240*10
        public static double sizePrint = 170;//标准打印文件半径170*170

        //微调平台
        public const int Z1 = 4;
        public const int Z2 = 5;
        public static double Z1_Max = 10;
        public static double Z2_Max = 10;

        //配置模块串口
        public static string ComADAM = "COM3";          // 亚当模块COM口：转485，含模拟输入（可获取SMC调压阀的气压反馈值、笔式位移传感器电压值）、模拟输出（控制调压阀的气压大小）
        public static string ComNozzleSensor = "COM2";   // 对针位移传感器COM口
        public static string ComWeightMeter = "COM12";   // 称重模块通信COM口：转232/485接口

#elif (Yunco)
        public const int X = 2;     // X轴轴号
        public const int Y = 0;     // Y轴轴号
        public const int Z = 6;     // Z轴轴号
        public const string IpAddr = "192.168.1.100";
        public static double X_MAX = 400;    // X轴上限
        public static double Y_MAX = 400;    // Y轴上限
        public static double Z_MAX = 150;    // Z轴上限
        public static string output = "1,1" + "1,2";

#elif (QiSUO)
        public const int X = 2;     // X轴轴号
        public const int Y = 0;     // Y轴轴号
        public const int Z = 3;     // Z轴轴号
        public const string IpAddr = "10.0.0.100";
        public static double X_MAX = 300;       // X轴上限
        public static double Y_MAX = 245;       // Y轴上限
        public static double Z_MAX = 100;       // Z轴上限
        
#else
            public const int X = 2;     // X轴轴号
            public const int Y = 0;     // Y轴轴号
            public const int Z = 3;     // Z轴轴号
            public static double X_MAX = 200;       // X轴上限
            public static double Y_MAX = 200;       // Y轴上限
            public static double Z_MAX = 80;        // Z轴上限


#endif
        public static class PMC
        {
            public const int X = 1;     // X轴序号
            public const int Y = 2;     // Y轴序号
            public const int Z = 3;     // Z轴序号
            public const int Rx = 4;
            public const int Ry = 5;
            public const int Rz = 6;
            //控制器
            public const string IpAddress = "192.168.10.101";
            public static int XbotId;
            public static int[] arrXBotIds = new int[2] {1, 2};//多动子工况
            public static bool pmc_activating;
            public static string pmc_booting;
            public static string pmc_deactivating;
            public static string pmc_errorHandling;
            public static string pmc_error;
            public static bool pmc_inactive = false;
            public static bool pmc_fullcontrol = false;
            //单位转换
            public static double M2MM = 1000;//m转mm
            public static double Rad2Degree = 180 / Math.PI;//弧度转角度
        };


        /// <summary>
        /// 紧急停止
        /// </summary>
        public static void StopImmediately()
        {
            try
            {
                GV.bPausePrint = false;
                StartEndPrinting(false);
                if (GV.frmPrintGuide.frmCurrStep.Name == "FrmPrintStep4")
                {
                    GV.frmPrintStep4.SetNotPreviewed();
                }
            }
            catch (Exception ex)
            {
            }
        }
        //打印开始
        public static void ConfirmPrinting()
        {
            bPausePrint = false;
            string str1 = Application.StartupPath + "\\Log\\" + "Conlog_" + Path.GetFileName(GV.PathFileName) + "_" + DateTime.Now.ToString("y.M.d hhmmss") + ".txt";//创建日志文件
            string str2 = Application.StartupPath + "\\Log\\" + "Monlog_" + Path.GetFileName(GV.PathFileName) + "_" + DateTime.Now.ToString("y.M.d hhmmss") + ".txt";
            GV.ControlLogFileName = str1;//控制指令信息文件
            GV.MonitorLogFileName = str2;//检测运动信息文件
            GV.PrintingObj.ClearCommand();
            StartEndPrinting(true);
            frmPathTrace.Show();
            frmPathTrace.Activate();
            //if (!GV.monitorWorker.IsBusy)
            {
                //  GV.monitorWorker.RunWorkerAsync();
            }
        }

        //开始打印
        public static void StartEndPrinting(bool IsStarting)
        {
            PrintingObj.IsPrinting = IsStarting;//打印状态
            GV.frmMain.SetPrintStatus(IsStarting);//设置开始打印
            //转速序列
            GV.frmRotaryValveCtrl.SetStartPrint(IsStarting);//开始计时，改变转速
            if (IsStarting)
            {
                dataManagementObj.ResetCmdCounter();
                GV.frmPrintStep4.SetStatus(2);

            }
            else
            {
                dataManagementObj.CmdQueue.Clear();
                dataManagementObj.ResetCmdCounter();
                dataManagementObj.stopwatch.Stop();
                frmPathPreview.EnableConfirmPrinting(false);
                PrintingObj.Extrude(0, 0);//关OUT口
                PrintingObj.Extrude(1, 0);
                PrintingObj.Extrude(2, 0);
                PrintingObj.Extrude(3, 0);
                PrintingObj.Extrude(4, 0);
                PrintingObj.Extrude(5, 0);
                PrintingObj.SetAlarmPort(0, 0);
                PrintingObj.SetAlarmPort(1, 0);
                PrintingObj.SetAlarmPort(2, 0);
                PrintingObj.SetAlarmPort(3, 0);
                PrintingObj.Stop();
                PrintingObj.ClearNozzleDXYZ_AB();
                GV.frmPrintStep4.SetStatus(0);
                pathprinting = false;//曲面打印停止写入
                                     // PrintingObj.IsPrinting = false;
                                     //double hour2 = Math.Floor(printTimeLast / 3600);
                                     //double minute2 = Math.Floor(printTimeLast / 60 % 60);
                                     //double seconde2 = Math.Floor(printTimeLast % 60);
                                     //string str =  "打印完成, 共计耗时: " + (hour2 > 0 ? hour2.ToString() + "时" : "") + (minute2 > 0 ? minute2.ToString() + "分" : "") + seconde2.ToString() + "秒";
                                     // GV.monitorWorker.CancelAsync();//取消指令执行监控线程
            }
        }

        public static void ReadLayerData(double[][] matrixP, double xStart, double yStart, int layer, bool setRotation, bool sendRotation, out double dz, out double liftHeight, out int axis, out double[] m, out double[] p, out double rot)
        {
            //int colIndex = setRotation ? 6 : 5;//决定是否含有转速列
            int count = matrixP[layer].Length;
            //p = new double[count - colIndex];
            m = new double[2];
            dz = matrixP[layer][0];//读取层高
            liftHeight = matrixP[layer][1];
            axis = (int)matrixP[layer][2];//x||y轴
            double offset_m = (axis == 0 ? xStart : yStart);//偏移
            double offset_p = (axis == 0 ? yStart : xStart);
            m[0] = matrixP[layer][3] + offset_m;//min
            m[1] = matrixP[layer][4] + offset_m;//max
                                                // 读取 p 数据（从第5列开始）
            int pStartIndex = 5;
            int pLength = matrixP[layer].Length - (setRotation ? 6 : 5); // 剩余列数
            p = new double[pLength];
            for (int i = 0; i < pLength; i++)
            {
                p[i] = matrixP[layer][pStartIndex + i] + offset_p;
            }

            if (setRotation && sendRotation)
            {
                rot = matrixP[layer][count - 1];//最后一列
            }
            else
            {
                rot = double.NaN;
            }
        }


        public static void InitObject()
        {
            GV.paraSettingObj = new ParaSetting();
            GV.dataManagementObj = new DataManagement();
            GV.PrintingObj = new ACSMotionControl(GV.paraSettingObj, GV.dataManagementObj);

            //dataManagementObj.CmdData = new DataManagement.CmdDataStruct[50];

            // 设置Backgoundworker属性
            GV.bgWorker.WorkerReportsProgress = true;
            GV.bgWorker.WorkerSupportsCancellation = true;

            // 把处理程序连接到BackgroundWorker对象
            GV.bgWorker.DoWork += DoWork_Handler;
            GV.bgWorker.ProgressChanged += ProgressChanged_Handler;
            GV.bgWorker.RunWorkerCompleted += RunWorkerCompleted_Handler;
            //
            // 设置Backgoundworker属性
            GV.bgWorker2.WorkerReportsProgress = true;
            GV.bgWorker2.WorkerSupportsCancellation = true;

            // 把处理程序连接到BackgroundWorker对象
            GV.bgWorker2.DoWork += DoWork2_Handler;
            GV.bgWorker2.ProgressChanged += ProgressChanged2_Handler;
            GV.bgWorker2.RunWorkerCompleted += RunWorkerCompleted2_Handler;

            //新建线程读取指令执行状态
            //GV.monitorWorker.WorkerReportsProgress = true;
            //GV.monitorWorker.WorkerSupportsCancellation = true;
            //GV.monitorWorker.DoWork += MonitorGRTime;

            //if (!ReadConfigFile()) return; //读取配置文件

            GV.PathFileName = Application.StartupPath + "\\PrintFiles\\default.csv";
            GV.TechParaFileName = Application.StartupPath + "\\PrintFiles\\default.amtp";
        }

        public static void InitForms(FrmMain frm)
        {
            GV.InitAllSeries();

            GV.frmMain = frm;
            //GV.frmPrintSteps.Show();

            //GV.frmPrintStep1 = new FrmPrintStep1();
            //GV.frmPrintStep2 = new FrmPrintStep2();
            //GV.frmPrintStep3 = new FrmPrintStep3();
            //GV.frmPrintStep4 = new FrmPrintStep4();

            GV.frmPathPreview = new FrmPathPreview();
            GV.frmPathPreview.MdiParent = frm;

            GV.frmPrintGuide = new FrmPrintGuide();
            GV.frmPrintGuide.MdiParent = frm;

            GV.frmMotionAdjust = new FrmMotionAdjust();
            GV.frmMotionAdjust.MdiParent = frm;
            //GV.frmMotionAdjust.Show();

            GV.frmNozzleCalibrate = new FrmNozzleCalibrate();
            GV.frmNozzleCalibrate.MdiParent = frm;

            GV.frmStatusMonitor = new FrmStatusMonitor();
            GV.frmStatusMonitor.MdiParent = frm;

            //GV.frmStatusMonitor.Show();

            GV.frmImmStop = new FrmImmStop();
            GV.frmImmStop.TopMost = true;
            //GV.frmImmStop.Show();

            GV.frmPathTrace = new FrmPathTrace();
            GV.frmPathTrace.MdiParent = frm;

            //GV.frmPathTrace.Hide();
            GV.frmBasicTest = new FrmPathRun();
            GV.frmBasicTest.MdiParent = frm;

            GV.frmDiagnose = new FrmDiagnose();
            GV.frmDiagnose.MdiParent = frm;

            //GV.frmCamera = new FrmCamera();
            //GV.frmCamera.MdiParent = frm;

            //FrmDCPower
            //FrmSetPressure
            //FrmSyringePump
            //FrmTemperature
            GV.frmDCPower = new FrmDCPower();
            GV.frmDCPower.MdiParent = frm;

            GV.frmSetPressure = new FrmSetPressure();
            GV.frmSetPressure.MdiParent = frm;

            GV.frmSyringePump = new FrmSyringePump();
            GV.frmSyringePump.MdiParent = frm;

            GV.frmTemperature = new FrmTemperature();
            GV.frmTemperature.MdiParent = frm;

            GV.frmTemptTableCtrl = new FrmTemptTableCtrl();
            GV.frmTemptTableCtrl.MdiParent = frm;
            //曲面打印

            GV.frmRotaryValveCtrl = new FrmRotaryValveCtrl();
            GV.frmRotaryValveCtrl.MdiParent = frm;

            GV.frmWeightCtrl = new FrmWeightCtrl();
            GV.frmWeightCtrl.MdiParent = frm;
        }

        /// <summary>
        /// 后台线程：状态更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void DoWork_Handler(object sender, DoWorkEventArgs args)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            //System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            //stopwatch.Start();
            int count = 0;
            // 进行后台处理
            for (byte i = 0; ; i++)
            {
                // 每次都检查是否取消
                if (worker.CancellationPending)
                {
                    worker.ReportProgress(-1);//取消线程任务
                    args.Cancel = true;
                    break;
                }
                else
                {
                    // 如果没有取消则继续处理
                    string msg = GV.PrintingObj.UpdateStatus(GV.PrintingObj.DataObj.stopwatch.ElapsedMilliseconds);// 查询当前打印状态
                    //挤出模式
                    if (PrintingObj.IsPrinting)
                    {
                        if ((PrintingObj.Status.isExtruding ^ Isextrude) && PrintingObj.Status.nozzleID == 0)//挤出状态变化，且1号口出丝
                        {
                            if (count < 10)//挤出失败则立即停止
                            {
                                PrintingObj.Extrude(0, Convert.ToInt32(Isextrude));
                                count++;
                            }
                            else
                            {
                                //处理出丝异常情况
                                StopImmediately();
                            }
                        }
                    }
                    else
                    {
                        count = 0;
                    }

                    if (i % 128 == 0)//128次循环查询一次温度
                    {
                        strTemperature = QueryTemperature(seriPort1);
                    }
                    if (msg != "Normal")//打印异常取消
                    {
                        GV.bgWorker.CancelAsync();
                        GV.bgWorker2.CancelAsync();
                    }
                    worker.ReportProgress(0);   // 触发ProgressChanged事件，以便向主线程进行通信。
                    Thread.Sleep(GV.refreshInterval);//指令输入刷新间隔
                }
            }

            // 处理完毕，存储结果并输出。
            args.Result = -1;
        }

        static int counter1 = 0;

        // 主线程处理后台线程的输入
        private static void ProgressChanged_Handler(object sender, ProgressChangedEventArgs args)
        {
            try
            {
                if (args.ProgressPercentage == -1)
                {
                    frmStatusMonitor.ResetMonitorChart();
                    return;
                }
                counter1++;
                if (counter1 % 10 == 0)
                {
                    if (!PrintingObj.bPhotoTaken)
                    {
                        //frmCamera.TakePhoto(PrintingObj.photoName);
                        PrintingObj.bPhotoTaken = true;
                    }
                    frmPrintStep1.SetAxesStatus(PrintingObj.Status);//检查轴状态
                    frmMain.UpdatePMCStatus(PrintingObj.Status);
                    if (followCurPosition) LocateCurPosition();

                    //if (PrintingObj.IsPrinting)
                    //{
                    //    if (dataManagementObj.countCmdTotal > 0)
                    //    {
                    //        double percent = 100 * dataManagementObj.countCmdExecuted / dataManagementObj.countCmdTotal;
                    //        GV.frmMain.UpdatePrintPercent((int)percent);
                    //    }
                    //}
                    //else if (args.ProgressPercentage == 100)
                    //{
                    //    StartEndPrinting(false);
                    //}
                }

                if (PrintingObj.IsPrinting && counter1 % GV.Monitor_interval == 0)//正在打印且是监控间隔的整数倍5*40ms刷新一次
                {
                    double time = PrintingObj.Status.time * 0.001;//时间毫秒到秒
                    string str = time.ToString("F3") + "," + PrintingObj.Status.fPosX.ToString("F3") + "," + PrintingObj.Status.fPosY.ToString("F3")
                        + "," + PrintingObj.Status.fPosZ.ToString("F3") + "," + PrintingObj.Status.isExtruding.ToString();//日志信息
                    PrintingObj.Monitordata.Add(str);
                    if (counter1 % GV.MonRecord_interval == 0)//200*40写入日志一次
                    {
                        GV.AppendLineTextFile(GV.MonitorLogFileName, PrintingObj.Monitordata.ToArray());//写入
                        PrintingObj.Monitordata.Clear();//更新记录
                    }
                }
                frmStatusMonitor.UpdateMotionStatus(PrintingObj.Status);//实时更新显示运动状态
                SetSeriesValue();
                //显示层
                frmPathTrace.ShowLayer();
                frmPathTrace.ShowMoveCmd();

            }
            catch (Exception ex)
            {
            }
        }

        // 后台线程完成之后，保存结果。
        private static void RunWorkerCompleted_Handler(object sender, RunWorkerCompletedEventArgs args)
        {
        }

        /// <summary>
        /// 后台线程：执行控制指令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private static void DoWork2_Handler(object sender, DoWorkEventArgs args)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            // 进行后台处理
            for (; ; )//一直执行
            {
                // 每次都检查是否取消
                if (worker.CancellationPending)
                {
                    //worker.ReportProgress(-1);
                    args.Cancel = true;
                    break;
                }
                else
                {
                    // 如果没有取消则继续处理
                    for (int i = 0; i < GV.Command_Num; i++)//单次刷新20条指令
                    {
                        if (GV.dataManagementObj.ReadCmdData().CmdName == DataManagement.OptType.SegmentLine || GV.dataManagementObj.ReadCmdData().CmdName == DataManagement.OptType.DisplayInfo)//获取队列第一条指令
                        {
                            GV.PrintingObj.Run();
                        }
                        else
                        {
                            GV.PrintingObj.Run();
                            break;
                        }
                    }
                    //GV.PrintingObj.Run();
                    Thread.Sleep(GV.refreshInterval);
                    worker.ReportProgress(0);
                }
            }

            // 处理完毕，存储结果并输出。
            args.Result = -1;
        }

        static int counter2 = 0;//调节时间的计数器
        // 主线程处理后台线程的输入
        private static void ProgressChanged2_Handler(object sender, ProgressChangedEventArgs args)
        {
            try
            {
                //double t_e = dataManagementObj.segStartEstimateTime;
                //double t_eA = dataManagementObj.totalEstimateTime;
                ////double t_rA = dataManagementObj.totalRealTimeEst;
                ////double t_r = dataManagementObj.stopwatch.ElapsedMilliseconds * 0.001;
                ////double t_rLeft = t_rA - t_r;
                ////double d_t_rLeft = Math.Abs(t_rLeft - dataManagementObj.realEstimateLeftTime);

                int percent;
                // percent = args.ProgressPercentage;

                counter2++;

                if (PrintingObj.IsPrinting && counter2 % GV.ConRecord_interval == 0)
                {
                    GV.AppendLineTextFile(GV.ControlLogFileName, GV.PrintingObj.Controlcommand.ToArray());//指令写入
                    GV.PrintingObj.Controlcommand.Clear();
                }

                if (PrintingObj.IsPrinting)
                {
                    percent = 100 * dataManagementObj.countCmdExecuted / dataManagementObj.countCmdTotal;
                    if (percent > 100) percent = 100;
                    else if (percent < 0) percent = 0;
                    GV.frmMain.UpdatePrintPercent(percent);//显示百分比
                    if (percent >= 100)
                    {
                        //  GV.monitorWorker.CancelAsync();//打印完成取消
                    }
                }
                else if (args.ProgressPercentage == 100)
                {
                    StartEndPrinting(false);

                    GV.AppendLineTextFile(GV.ControlLogFileName, GV.PrintingObj.Controlcommand.ToArray());
                    GV.PrintingObj.Controlcommand.Clear();

                    //GV.frmPrintStep4.btnSaveLogfile_Click(null, null);
                }
            }
            catch (Exception ex)
            {
            }
        }

        // 后台线程完成之后，保存结果。
        private static void RunWorkerCompleted2_Handler(object sender, RunWorkerCompletedEventArgs args)
        {
        }
        //预先读取每层G1数量

        //新增监控指令执行线程
        private static void MonitorGRTime(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            Queue<string> preProcessQueue = new Queue<string>(); // 用于存储预处理的GCode指令
            const int PREPROCESS_WINDOW_SIZE = 10; // 预处理窗口大小
            int gseg = -1;//记录gseg值
            int currentLayer = 0;//记录当前打印的层
            int lastGSEG = -1;//初始gseg值-1
            int consecutiveG1Count = 0;//读取一层中连续G1的个数
            bool isNewSegment = true;//是否新的segmengt
            int maxGESGInsegment = 0;//记录当前段最大的GSEG值            
            int previousSegmentMaxGSEG = 0;//记录上一段打印时所用segment的指令数量
            List<int> segmentG1History = new List<int>();//存储记录
            bool isExtrudingSegment = false;

            double GRTIME_THRESHOLD; // GRTIME阈值
            const int GSEG_WARNING_MARGIN = 50;   // GSEG接近G1数量的预警余量类似50*20=1000

            for (int i = 0; ; i++)
            {
                try
                {
                    if (worker.CancellationPending)
                    {
                        e.Cancel = true;
                        worker.ReportProgress(-1);
                        //重置
                        currentLayer = 0;
                        previousSegmentMaxGSEG = 0;
                        isExtrudingSegment = false;
                        gseg = -1;
                        return;
                    }
                    else
                    {
                        // 获取当前正在执行的指令
                        string currentExecutingCmd = GV.dataManagementObj.ReadCmdData().CmdName.ToString();

                        //// 获取已发送指令
                        //string nextCmd = dataManagementObj.PeekNextCmdDataAsString(1);//读取后续指令                       

                        //if (!string.IsNullOrEmpty(nextCmd))
                        //{
                        //    // 预处理(未添加）
                        //    PreprocessGCode(nextCmd);
                        //};
                        GRTIME_THRESHOLD = GV.timeCloseExtrudeInAdvance;//提前关气时间

                        // 读取当前GSEG值
                        gseg = GV.PrintingObj.Ch.ReadVariable("GSEG", 1, GV.X, GV.X);
                        object[] grTime = GV.PrintingObj.Ch.ReadVariable("GRTIME", GV.PrintingObj.Ch.ACSC_NONE);

                        int currentGSEG = (int)gseg;
                        PrintingObj.Status.GSEG = (int)gseg;//显示Gseg

                        PrintingObj.Status.GRTIME[0] = Convert.ToDouble(grTime[GV.X]);
                        PrintingObj.Status.GRTIME[1] = Convert.ToDouble(grTime[GV.Y]);
                        PrintingObj.Status.GRTIME[2] = Convert.ToDouble(grTime[GV.Z]);
                        double currentGRTime = PrintingObj.Status.GRTIME[0];//当前运动的grtime

                        bool isG1Extrude = GV.Isextrude;
                        // 更新挤出段状态
                        if (isG1Extrude)
                        {
                            isExtrudingSegment = true;
                        }
                        // 更新最大GSEG值（只在G1挤出段记录）
                        if (isG1Extrude && currentGSEG > maxGESGInsegment)
                        {
                            maxGESGInsegment = currentGSEG;
                        }
                        // 当GSEG变化时处理
                        if (currentGSEG != lastGSEG)
                        {
                            if (currentGSEG == -1 && isExtrudingSegment)
                            {
                                // 记录前一段的最大GSEG值
                                previousSegmentMaxGSEG = maxGESGInsegment;
                                GV.PrintingObj.Status.previousMaxCountGseg = previousSegmentMaxGSEG;

                                // 层切换逻辑
                                string layerCurrent = GV.PrintingObj.Status.layerCurrent; // "第 层, 共 层"
                                // 使用正则表达式提取数字
                                Match match = Regex.Match(layerCurrent, @"第(\d+)层");
                                if (match.Success)
                                {
                                    currentLayer = int.Parse(match.Groups[1].Value);
                                }
                                consecutiveG1Count = 0;
                                isNewSegment = true;
                            }
                            else if (currentGSEG > 0)
                            {
                                // 重置段状态
                                isExtrudingSegment = false;
                                maxGESGInsegment = 0;
                            }
                            lastGSEG = currentGSEG;
                        }
                        // 统计连续G1指令
                        if (currentExecutingCmd == "Extrude")
                        {
                            if (isNewSegment)
                            {
                                // 获取当前层的连续G1数量
                                // consecutiveG1Count = CountG1Commands(GV.PathFileName, currentLayer);
                                //consecutiveG1Count = GV.allG1Counts.Count > currentLayer ? allG1Counts[currentLayer] : 0;
                                // 获取第 层的各连续段G1数量
                                if (GV.allG1Counts.Count > currentLayer)
                                {
                                    var layerSegments = allG1Counts[currentLayer];
                                    Console.WriteLine($"第5层有{layerSegments.Count}段连续G1:");
                                    foreach (int count in layerSegments)
                                    {
                                        Console.WriteLine($"- 连续{count}条G1指令");
                                        consecutiveG1Count = count;
                                    }
                                }
                                GV.PrintingObj.Status.countG1 = consecutiveG1Count;//显示G1数量
                                isNewSegment = false;
                            }

                            // 实时减少剩余G1计数
                            if (currentGSEG > 0 && consecutiveG1Count > 0)
                            {
                                //GV.PrintingObj.Status.remainingG1 = consecutiveG1Count - currentGSEG;
                            }
                        }
                        else
                        {
                            isNewSegment = true; // 遇到非G1指令，重置段标记
                        }
                        //检查GSEG接近G1数量且GRTIME小于阈值的情况
                        if (isExtrudingSegment && currentGSEG > 0 //在出丝的segment
                            && (consecutiveG1Count - currentGSEG) < GSEG_WARNING_MARGIN //gseg的数量接近当前层G1的数量
                            && currentGRTime < GRTIME_THRESHOLD)//GRTime<设置的提前关气时间
                        {
                            PrintingObj.SetExtrudePorts(-2, 0);
                        }
                        else
                        {
                            //待添加标志
                        }
                        // 等待下一次检查
                        Thread.Sleep(10); //设置间隔
                    }
                }
                catch (Exception ex)
                {
                    // 记录错误并终止工作线程
                    worker.ReportProgress(-1, $"监控出错: {ex.Message}");
                    e.Cancel = true;
                    return;
                }
                // 正常退出
                e.Cancel = true;
                worker.ReportProgress(100, "监控正常结束");
            }
        }

        // 统计指定层中连续G1指令的数量
        private static int CountConsecutiveG1Commands(string fileName, int layer)
        {
            int maxConsecutive = 0;
            int currentStreak = 0;
            bool isInTargetLayer = false;

            try
            {
                foreach (string line in File.ReadLines(fileName))
                {
                    string trimmedLine = line.Trim();

                    // 检查层标记
                    if (trimmedLine.StartsWith(";LAYER:"))
                    {
                        int currentLineLayer = int.Parse(trimmedLine.Substring(7));
                        isInTargetLayer = (currentLineLayer == layer);
                        currentStreak = 0; // 层切换时重置计数
                        continue;
                    }

                    // 只在目标层统计
                    if (!isInTargetLayer)
                        continue;

                    // 统计连续G1指令
                    if (trimmedLine.StartsWith("G1"))
                    {
                        currentStreak++;
                        maxConsecutive = Math.Max(maxConsecutive, currentStreak);
                    }
                    else if (!trimmedLine.StartsWith(";")) // 忽略注释
                    {
                        currentStreak = 0; // 非G1指令重置连续计数
                    }
                }
            }
            catch
            {
                // 错误处理
            }

            return maxConsecutive;
        }
        //获取整个文件
        public static List<List<int>> CountG1Commands(string fileName)
        {
            // 返回值结构: List<层号, List<该层各连续G1段的指令数量>>
            var layerConsecutiveG1s = new List<List<int>>();
            int currentLayer = -1;
            bool isInFillSection = false;
            List<int> currentLayerSegments = new List<int>();
            int currentConsecutiveCount = 0;

            try
            {
                foreach (string line in File.ReadLines(fileName))
                {
                    string trimmedLine = line.Trim().ToUpper();

                    // 检测层变化
                    if (trimmedLine.StartsWith(";LAYER:"))
                    {
                        // 保存上一层的计数
                        if (currentLayer >= 0)
                        {
                            // 完成最后一个连续段
                            if (currentConsecutiveCount > 0)
                            {
                                currentLayerSegments.Add(currentConsecutiveCount);
                                currentConsecutiveCount = 0;
                            }

                            // 确保列表足够大以容纳当前层
                            while (layerConsecutiveG1s.Count <= currentLayer)
                            {
                                layerConsecutiveG1s.Add(new List<int>());
                            }
                            layerConsecutiveG1s[currentLayer] = currentLayerSegments;
                        }

                        // 初始化新层计数
                        if (int.TryParse(trimmedLine.Substring(7), out int layerNum))
                        {
                            currentLayer = layerNum;
                            currentLayerSegments = new List<int>();
                            currentConsecutiveCount = 0;
                            isInFillSection = false;
                        }
                        continue;
                    }

                    // 当前未在任何层中（首行注释等情况）
                    if (currentLayer < 0)
                        continue;

                    // 检测填充区域开始
                    if (trimmedLine.StartsWith(";TYPE:FILL"))
                    {
                        isInFillSection = true;
                        continue;
                    }

                    // 检测填充区域结束
                    if (trimmedLine.StartsWith(";TYPE:WALL-INNER") ||
                       (trimmedLine.StartsWith(";TYPE:") && !trimmedLine.StartsWith(";TYPE:FILL")))
                    {
                        isInFillSection = false;
                        // 结束当前连续段
                        if (currentConsecutiveCount > 0)
                        {
                            currentLayerSegments.Add(currentConsecutiveCount);
                            currentConsecutiveCount = 0;
                        }
                        continue;
                    }

                    // 在填充区域内统计连续G1指令
                    if (isInFillSection)
                    {
                        if (trimmedLine.StartsWith("G1"))
                        {
                            currentConsecutiveCount++;
                        }
                        else if (currentConsecutiveCount > 0) // 遇到非G1指令结束当前连续段
                        {
                            currentLayerSegments.Add(currentConsecutiveCount);
                            currentConsecutiveCount = 0;
                        }
                    }
                }

                // 保存最后一层的计数
                if (currentLayer >= 0)
                {
                    // 完成最后一个连续段
                    if (currentConsecutiveCount > 0)
                    {
                        currentLayerSegments.Add(currentConsecutiveCount);
                    }

                    // 确保列表足够大以容纳当前层
                    while (layerConsecutiveG1s.Count <= currentLayer)
                    {
                        layerConsecutiveG1s.Add(new List<int>());
                    }
                    layerConsecutiveG1s[currentLayer] = currentLayerSegments;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"分析Gcode文件出错: {ex.Message}");
                return new List<List<int>>(); // 返回空列表表示出错
            }

            return layerConsecutiveG1s;
        }
        // GCode预处理
        private static void PreprocessGCode(string parameters)
        {
            try
            {
                // 处理显示信息指令
                GV.Commands = parameters;
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// 初始化各坐标系的点序列
        /// </summary>
        public static void InitAllSeries()
        {
            InitSeries(ref seriXY, 0);
            InitSeries(ref seriXY1, 1);
            seriXY1.Points.AddXY(0, 0);

            InitSeries(ref seriZ0, 0);
            InitSeries(ref seriZ01, 1);
            seriZ01.Points.AddXY(5, 0);

            InitSeries(ref seriXZ, 0);
            InitSeries(ref seriXZ1, 1);
            seriXZ1.Points.AddXY(0, 0);

            InitSeries(ref seriY0, 0);
            InitSeries(ref seriY01, 1);
            seriY01.Points.AddXY(5, 0);

            InitSeries(ref seriYZ, 0);
            InitSeries(ref seriYZ1, 1);
            seriYZ1.Points.AddXY(0, 0);

            InitSeries(ref seriX0, 0);
            InitSeries(ref seriX01, 1);
            seriX01.Points.AddXY(5, 0);

            //InitSeries(ref seriTvelX, 2, 0);
            //InitSeries(ref seriTvelY, 2, 1);
            //InitSeries(ref seriTvelZ, 2, 2);
            //InitSeries(ref seriTvelG, 2, 3);
        }

        /// <summary>
        /// 初始化指定序列
        /// </summary>
        /// <param name="seri">序列名</param>
        /// <param name="seriType">序列类型</param>
        public static void InitSeries(ref Series seri, int seriType, int colorType = 2)
        {
            seri = new Series();
            Color color;
            switch (colorType)
            {
                case 0:
                    color = Color.Red;
                    break;
                case 1:
                    color = Color.Green;
                    break;
                case 2:
                    color = Color.Blue;
                    break;
                case 3:
                    color = Color.Gold;
                    break;
                case 10:
                    color = Color.Black; // 障碍物
                    break;
                default:
                    color = Color.Blue;
                    break;
            }
            switch (seriType)
            {
                case 0: // Draw lines
                    seri.ChartArea = "ChartArea1";
                    seri.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
                    seri.Name = "Series0";
                    seri.Color = color;
                    break;
                case 1: // Draw Point
                    seri.ChartArea = "ChartArea1";
                    seri.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastPoint;
                    seri.Color = color;
                    seri.MarkerBorderColor = System.Drawing.Color.Lime;
                    seri.MarkerBorderWidth = 0;
                    seri.MarkerColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
                    seri.MarkerSize = 10;
                    seri.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
                    seri.Name = "Series1";
                    seri.YValuesPerPoint = 1;
                    break;
                case 2:
                    seri.ChartArea = "ChartArea1";
                    seri.ChartType = SeriesChartType.Line;
                    seri.Name = "Series2";
                    seri.Color = color;
                    break;
            }
        }


        /// <summary>
        /// 定位当前位置，将之显示到坐标系中间。
        /// </summary>
        public static void LocateCurPosition()
        {
            Chart chart;
            double midAxisX = 0;
            double midAxisY = 0;
            double halfLenAxisX;
            double halfLenAxisY;

            // Locate positionChart:
            chart = GV.frmPathTrace.getPositionChart();

            midAxisX = chart.Series[1].Points[0].XValue; //PrintingObj.Status.fPosX;
            midAxisY = chart.Series[1].Points[0].YValues[0]; //PrintingObj.Status.fPosY;

            halfLenAxisX = 0.5 * (chart.ChartAreas[0].AxisX.Maximum - chart.ChartAreas[0].AxisX.Minimum);
            chart.ChartAreas[0].AxisX.Minimum = midAxisX - halfLenAxisX;
            chart.ChartAreas[0].AxisX.Maximum = midAxisX + halfLenAxisX;

            halfLenAxisY = 0.5 * (chart.ChartAreas[0].AxisY.Maximum - chart.ChartAreas[0].AxisY.Minimum);
            chart.ChartAreas[0].AxisY.Minimum = midAxisY - halfLenAxisY;
            chart.ChartAreas[0].AxisY.Maximum = midAxisY + halfLenAxisY;

            // Locate positionChart2:
            chart = GV.frmPathTrace.getPositionChart2();
            midAxisX = chart.Series[1].Points[0].XValue; //PrintingObj.Status.fPosX;
            midAxisY = chart.Series[1].Points[0].YValues[0]; //PrintingObj.Status.fPosY;

            halfLenAxisX = 0.5 * (chart.ChartAreas[0].AxisX.Maximum - chart.ChartAreas[0].AxisX.Minimum);
            chart.ChartAreas[0].AxisX.Minimum = midAxisX - halfLenAxisX;
            chart.ChartAreas[0].AxisX.Maximum = midAxisX + halfLenAxisX;

            halfLenAxisY = 0.5 * (chart.ChartAreas[0].AxisY.Maximum - chart.ChartAreas[0].AxisY.Minimum);
            chart.ChartAreas[0].AxisY.Minimum = midAxisY - halfLenAxisY;
            chart.ChartAreas[0].AxisY.Maximum = midAxisY + halfLenAxisY;
        }

        /// <summary>
        /// 更新曲线序列数据值
        /// </summary>
        /// <param name="axes"></param>
        public static void SetSeriesValue()
        {
            StageStatus status = GV.PrintingObj.Status;
            double threshold = 0.1;
            double x = status.fPosX - PrintingObj.nozzleDX;
            double y = status.fPosY - PrintingObj.nozzleDY;
            double z = status.fPosZ - PrintingObj.nozzleDZ;

            double xDisp = x + dX_AB;

            if (Math.Abs(xLast - x) > threshold || Math.Abs(yLast - y) > threshold || Math.Abs(zLast - z) > threshold)
            {
                seriXY.Points.AddXY(x, y);
                seriXZ.Points.AddXY(x, z);
                seriYZ.Points.AddXY(y, z);
                int iMax = seriXY.Points.Count - 1;

                if (status.isExtruding)
                {
                    seriXY.Points[iMax].BorderWidth = 3;
                    seriXZ.Points[iMax].BorderWidth = 3;
                    seriYZ.Points[iMax].BorderWidth = 3;
                    if (status.nozzleID == 1)
                    {
                        seriXY.Points[iMax].Color = Color.DarkGreen;
                        seriXZ.Points[iMax].Color = Color.DarkGreen;
                        seriYZ.Points[iMax].Color = Color.DarkGreen;
                    }
                    else
                    {
                        seriXY.Points[iMax].Color = Color.Blue;
                        seriXZ.Points[iMax].Color = Color.Blue;
                        seriYZ.Points[iMax].Color = Color.Blue;
                    }
                }
                else
                {
                    seriXY.Points[iMax].BorderWidth = 1;
                    seriXZ.Points[iMax].BorderWidth = 1;
                    seriYZ.Points[iMax].BorderWidth = 1;
                    seriXY.Points[iMax].Color = Color.Gray;
                    seriXZ.Points[iMax].Color = Color.Gray;
                    seriYZ.Points[iMax].Color = Color.Gray;
                }

                seriX0.Points.AddXY(5, x);
                seriY0.Points.AddXY(5, y);
                seriZ0.Points.AddXY(5, z);

                xLast = x;
                yLast = y;
                zLast = z;
            }

            //seriX01.Points[0].XValue = 5;
            seriX01.Points[0].YValues[0] = x;
            //seriY01.Points[0].XValue = 5;
            seriY01.Points[0].YValues[0] = y;
            //seriZ01.Points[0].XValue = 5;
            seriZ01.Points[0].YValues[0] = z;

            seriXY1.Points[0].XValue = x;
            seriXY1.Points[0].YValues[0] = y;
            seriXZ1.Points[0].XValue = x;
            seriXZ1.Points[0].YValues[0] = z;
            seriYZ1.Points[0].XValue = y;
            seriYZ1.Points[0].YValues[0] = z;

            //lblCurPoint.Text = seriXY1.Points[0].XValue.ToString("0.00") + "," + seriXY1.Points[0].YValues[0].ToString("0.00");
            //lblLastPoint.Text = seriXY.Points[seriXY.Points.Count - 1].XValue.ToString("0.00") + "," + seriXY.Points[seriXY.Points.Count - 1].YValues[0].ToString("0.00");

            //seriTvelX.Points.AddXY(GV.PrintingObj.Status.time, GV.PrintingObj.Status.fVelX);
            //seriTvelY.Points.AddXY(GV.PrintingObj.Status.time, GV.PrintingObj.Status.fVelY);
            //seriTvelZ.Points.AddXY(GV.PrintingObj.Status.time, GV.PrintingObj.Status.fVelZ);
            //while (seriTvelX.Points.Count > 200)
            //{
            //    seriTvelX.Points.RemoveAt(0);
            //    seriTvelY.Points.RemoveAt(0);
            //    seriTvelZ.Points.RemoveAt(0);
            //    seriTvelG.Points.RemoveAt(0);
            //}
            while (seriXY.Points.Count > keepPoints)
            {
                seriXY.Points.RemoveAt(0);
                seriXZ.Points.RemoveAt(0);
                seriYZ.Points.RemoveAt(0);
                seriX0.Points.RemoveAt(0);
                seriY0.Points.RemoveAt(0);
                seriZ0.Points.RemoveAt(0);
            }
        }


        /// <summary>
        /// 生成自定义网格打印指令
        /// </summary>
        /// <param name="fileName">自定义网格文件（.csv）</param>
        public static void ExecuteCustomedCube(string fileName, int printMode)
        {
            if (PrintingObj.IsPrinting)
            {
                return;
            }
            bPausePrint = true; // 暂停打印

            string[][] strCSV;
            double[][] matrixP;
            try
            {
                strCSV = CsvFileReader.Read(fileName);
                matrixP = new double[strCSV.Length][];
                for (int i = 0; i < strCSV.Length; i++)
                {
                    List<double> listP = new List<double>();
                    for (int j = 0; j < strCSV[i].Length; j++)
                    {
                        if (strCSV[i][j].Trim().Length > 0)
                        {
                            listP.Add(Convert.ToDouble(strCSV[i][j]));
                        }
                    }
                    matrixP[i] = listP.ToArray();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            double xCenter = 0, yCenter = 0; // 样品几何中心坐标
            for (int i = 0; i < matrixP.Length; i++)
            {
                if (matrixP[i][2] == 0) // X轴方向
                {
                    xCenter = (matrixP[i][3] + matrixP[i][4]) / 2;
                }
                else // Y轴方向
                {
                    yCenter = (matrixP[i][3] + matrixP[i][4]) / 2;
                }
            }

            double[] p;
            double[] m;
            int axis;
            double vprint = printingSpeedSet;
            double vmove = jumpSpeedSet;
            double zprint, zmove;
            double liftHeight = 2;      // 移动时提针高度
            double layerHeight = 10;    // 打印层高/层厚
            double rotSpeed = 60;          //新增螺杆阀转速设置 
            int sleepTime = GV.timeExtrudeInAdvance0;
            int advanceTime = timeCloseExtrudeInAdvance;
            int im = 0, im0 = 0; // im: index of max/min value (0: min; 1: max)
            int ip = 0, ip0 = 0, ip_next;    // ip: index of line postion
            double xStart, yStart;
            double xLast, yLast, zLast;
            VertexPos lastPos = VertexPos.LeftBottom;
            bool isClockwise = false;
            if (connMode == ConnectMode.ConnectSimulator)
            {
                //设置仿真时的起点位置为动子的左上角
                xStart = stageXa - sizePrint/2.0;
                yStart = stageY - sizePrint/2.0;
                zprint = 30;
            }
            else
            {
                // 连接实际设备时，可以直接从当前位置开始打印
                xStart = PrintingObj.Status.fPosX;
                yStart = PrintingObj.Status.fPosY;
                zprint = PrintingObj.Status.fPosZ;
            }           
            xLast = xStart; yLast = yStart; zLast = zprint;

            int[] Axes = new int[3];
            Axes[0] = X;
            Axes[1] = Y;
            Axes[2] = -1;
            double[] centerPoint = new double[] { 0.0, 0.0 };
            double[] startPoint = new double[] { 0.0, 0.0 };
            double[] endPoint = new double[] { 0.0, 0.0 };
            string strGcode = "";
            int nozzleID = 0;
            bool LaterRotate = false;
            bool TimeRotate = false;

            for (int layer = 0; layer < matrixP.Length; layer++)
            {
                PrintingObj.qDisplayInfo("layerCurrent", "第" + (layer + 1).ToString() + "层, 共" + matrixP.Length + "层", "TransParent", layer);

                //读取单层的数据
                ReadLayerData(matrixP, xStart, yStart, layer, GV.setLayerRotation, GV.layerRotation, out layerHeight, out liftHeight, out axis, out m, out p, out rotSpeed);

                if (axis == 0) // 本层线条沿X轴方向
                {
                    switch (lastPos)
                    {
                        case VertexPos.LeftBottom:
                            im0 = 0; ip0 = 0; isClockwise = false;
                            break;
                        case VertexPos.LeftTop:
                            im0 = 0; ip0 = p.Length - 1; isClockwise = true;
                            break;
                        case VertexPos.RightBottom:
                            im0 = 1; ip0 = 0; isClockwise = true;
                            break;
                        case VertexPos.RightTop:
                            im0 = 1; ip0 = p.Length - 1; isClockwise = false;
                            break;
                        default:
                            break;
                    }
                    // move to start point of this layer
                    if (layer == 0) // 第一层增加一段延长线
                    {
                        double xx = m[im0] - GV.lenAdvanced;
                        strGcode = "G0 X" + xx.ToString() + " Y" + p[ip0].ToString();   // G0 X93.875 Y93.875
                        PrintingObj.qMoveXYTo(xx, p[ip0], vmove, xLast, yLast, layer, strGcode);
                        xLast = xx;
                    }
                    else
                    {
                        strGcode = "G0 X" + m[im0].ToString() + " Y" + p[ip0].ToString();   // G0 X93.875 Y93.875
                        PrintingObj.qMoveXYTo(m[im0], p[ip0], vmove, xLast, yLast, layer, strGcode);
                        xLast = m[im0];
                    }
                    yLast = p[ip0];
                }
                else // 本层线条沿Y轴方向
                {
                    switch (lastPos)
                    {
                        case VertexPos.LeftBottom:
                            im0 = 0; ip0 = 0; isClockwise = true;
                            break;
                        case VertexPos.LeftTop:
                            im0 = 1; ip0 = 0; isClockwise = false;
                            break;
                        case VertexPos.RightBottom:
                            im0 = 0; ip0 = p.Length - 1; isClockwise = false;
                            break;
                        case VertexPos.RightTop:
                            im0 = 1; ip0 = p.Length - 1; isClockwise = true;
                            break;
                        default:
                            break;
                    }
                    // move to start point of this layer
                    strGcode = "G0 X" + p[ip0].ToString() + " Y" + m[im0].ToString();   // G0 X93.875 Y93.875
                    PrintingObj.qMoveXYTo(p[ip0], m[im0], vmove, xLast, yLast, layer, strGcode);
                    xLast = p[ip0]; yLast = m[im0];
                }
                PrintingObj.qWaitMoveEnd("WaitMoveEnd"); // wait until print-head moved to line start
                strGcode = "G0 Z" + zprint.ToString();
                PrintingObj.qMoveAxisTo(Z, zprint, vmove, zLast, layer, strGcode);
                zLast = zprint;
                PrintingObj.qWaitMoveEnd("WaitMoveEnd");

                if (printMode == 0)
                {
                    nozzleID = 0;// layer % 2;  // 单喷头                 
                }
                else if (printMode == 1)
                {
                    nozzleID = layer % 2;  // 选择该层使用的喷头：奇数层用左喷头，偶数层用右喷头                        
                }
                else
                {
                    nozzleID = -2;  // 所有喷头同时打开                        
                }
                PrintingObj.qSwitchNozzle(nozzleID); // 切换喷头
                PrintingObj.qExtrude(nozzleID, 1, "Extrude:true", rotSpeed); // open dispenser
                if (sleepTime > 0) PrintingObj.qPause(sleepTime); // open dispenser in advance
                //sleepTime = 0;
                // 在当前位置建立一段SegmentedMotion
                startPoint = new double[] { xLast, yLast };
                //PrintingObj.qSegmentedMotion(Axes, startPoint, "SegmentedMotion");
                PrintingObj.qExtSegmentedMotion(Axes, startPoint, vprint, vprint, 0, 0);//202503,转角处减速，缓冲震动

                im = im0;
                // 本层除了最后一条线，都需在最后一条线加一个半圆弧。
                for (int i = 0; i < p.Length - 1; i++)
                {
                    if (ip0 == 0)    // draw lines from min to max
                    {
                        ip = i;
                        ip_next = ip + 1;
                    }
                    else                // draw lines from max to min
                    {
                        ip = ip0 - i;
                        ip_next = ip - 1;
                    }
                    PrintingObj.qDisplayInfo("filamentNumber", "本层第" + (i + 1).ToString() + "条, 共" + p.Length + "条");

                    im = 1 - im; // swap im (min->max or max->min) 相对本层第一条线的起点位置进行反向
                    if (axis == 0)
                    {
                        endPoint = new double[2] { m[im], p[ip] };
                        strGcode = "G1 X" + endPoint[0].ToString() + " Y" + endPoint[1].ToString();
                        PrintingObj.qSegmentLine(Axes, startPoint, endPoint, vprint, vprint, layer, strGcode);
                        startPoint = endPoint;
                        endPoint = new double[2] { m[im], p[ip_next] };
                        centerPoint = new double[2] { m[im], (p[ip] + p[ip_next]) * 0.5 };

                        if (isClockwise)
                        {
                            strGcode = "G2 XC" + centerPoint[0].ToString() + " YC" + centerPoint[1].ToString() + " X" + endPoint[0].ToString() + " Y" + endPoint[1].ToString() + " R" + Math.PI.ToString();
                        }
                        else
                        {
                            strGcode = "G3 XC" + centerPoint[0].ToString() + " YC" + centerPoint[1].ToString() + " X" + endPoint[0].ToString() + " Y" + endPoint[1].ToString() + " R" + Math.PI.ToString();
                        }
                        PrintingObj.qSegmentArc2(Axes, centerPoint, endPoint, (isClockwise ? -1 : 1) * Math.PI, vprint, vprint, layer, strGcode);
                        //PrintingObj.qMoveAxisTo(axis, m[im], vprint, xLast); PrintingObj.qWaitMoveEnd(); // move to line end                           
                        xLast = m[im];
                    }
                    else
                    {
                        endPoint = new double[2] { p[ip], m[im] };
                        strGcode = "G1 X" + endPoint[0].ToString() + " Y" + endPoint[1].ToString();
                        PrintingObj.qSegmentLine(Axes, startPoint, endPoint, vprint, vprint, layer, strGcode);
                        startPoint = endPoint;
                        endPoint = new double[2] { p[ip_next], m[im] };
                        centerPoint = new double[2] { (p[ip] + p[ip_next]) * 0.5, m[im] };
                        if (isClockwise)
                        {
                            strGcode = "G2 XC" + centerPoint[0].ToString() + " YC" + centerPoint[1].ToString() + " X" + endPoint[0].ToString() + " Y" + endPoint[1].ToString() + " R" + Math.PI.ToString();
                        }
                        else
                        {
                            strGcode = "G3 XC" + centerPoint[0].ToString() + " YC" + centerPoint[1].ToString() + " X" + endPoint[0].ToString() + " Y" + endPoint[1].ToString() + " R" + Math.PI.ToString();
                        }
                        PrintingObj.qSegmentArc2(Axes, centerPoint, endPoint, (isClockwise ? -1 : 1) * Math.PI, vprint, vprint, layer, strGcode);
                        //PrintingObj.qMoveAxisTo(axis, m[im], vprint, yLast); PrintingObj.qWaitMoveEnd(); // move to line end                           
                        yLast = m[im];
                    }
                    isClockwise = !isClockwise; // 反向
                }
                // 最后一条线不需要画圆弧
                if (ip0 == 0)    // draw lines from min to max
                {
                    ip = p.Length - 1; // index of the last line
                }
                else                // draw lines from max to min
                {
                    ip = 0;         // index of the last line
                }
                im = 1 - im; // swap im (min->max or max->min) 相对本层最后一条线的起点位置进行反向
                if (axis == 0) // 沿X轴方向画最后一条线
                {
                    endPoint = new double[2] { m[im], p[ip] };
                    strGcode = "G1 X" + endPoint[0].ToString() + " Y" + endPoint[1].ToString();
                    PrintingObj.qSegmentLine(Axes, startPoint, endPoint, vprint, 0, layer, strGcode);
                    startPoint = endPoint;
                    xLast = m[im];
                }
                else  // 沿Y轴方向画最后一条线
                {
                    endPoint = new double[2] { p[ip], m[im] };
                    strGcode = "G1 X" + endPoint[0].ToString() + " Y" + endPoint[1].ToString();
                    PrintingObj.qSegmentLine(Axes, startPoint, endPoint, vprint, 0, layer, strGcode);
                    startPoint = endPoint;
                    yLast = m[im];
                }
                // 记录本层终点位置
                if (axis == 0) // 线条沿X轴方向
                {
                    if (im == 0) // Left
                    {
                        if (ip == 0) // Bottom
                        {
                            lastPos = VertexPos.LeftBottom;
                        }
                        else // Top
                        {
                            lastPos = VertexPos.LeftTop;
                        }
                    }
                    else // Right
                    {
                        if (ip == 0) // Bottom
                        {
                            lastPos = VertexPos.RightBottom;
                        }
                        else // Top
                        {
                            lastPos = VertexPos.RightTop;
                        }
                    }
                }
                else // 线条沿Y轴方向
                {
                    if (im == 0) // Bottom
                    {
                        if (ip == 0) // Left
                        {
                            lastPos = VertexPos.LeftBottom;
                        }
                        else // Right
                        {
                            lastPos = VertexPos.RightBottom;
                        }
                    }
                    else // Top
                    {
                        if (ip == 0) // Left
                        {
                            lastPos = VertexPos.LeftTop;
                        }
                        else // Right
                        {
                            lastPos = VertexPos.RightTop;
                        }
                    }
                }
                PrintingObj.qEndSequenceM(Axes, "EndSequence");
                PrintingObj.qWaitMoveEnd("WaitMoveEnd");
                zprint -= layerHeight;
                if (photoEnabled) //如果需要逐层拍照
                {
                    PrintingObj.qExtrude(nozzleID, 0, "Extrude:false", 0); // close dispenser
                    PrintingObj.qTakePhoto(xStart + xCenter - GV.dX_Camera, yStart + yCenter - GV.dY_Camera, GV.dZ_Camera >= 0 ? GV.dZ_Camera : 0, DateTime.Now.ToString("yyyyMMddhhmm") + "-Layer-" + layer.ToString());
                }
                //else if (!GV.noStopExtrude) //如果层间需要停止出丝
                //{
                //    PrintingObj.qExtrude(nozzleID, 0, "Extrude:false", 0); // close dispenser
                //}
                else if (nozzleClean)//如果需要擦针
                {
                    PrintingObj.qExtrude(nozzleID, 0, "Extrude:false", 0); // close dispenser
                    PrintingObj.qCleanNozzle(xStart + xCenter - GV.dX_Clean, yStart + yCenter - GV.dY_Clean, GV.dZ_Clean >= 0 ? GV.dZ_Clean : 0);
                }
            }
            PrintingObj.qWaitMoveEnd("WaitMoveEnd");
            PrintingObj.qExtrude(portExtrude, 0, "Extrude:false", 0); // close dispenser
            PrintingObj.qSwitchNozzle(0); // 切换喷头
            //打印完提针
            strGcode = "G0 Z" + (0).ToString();
            PrintingObj.qMoveAxisTo(Z, 0, vmove, zLast, 0, strGcode); zLast = 0;
            PrintingObj.qWaitMoveEnd("WaitMoveEnd"); // nozzle up
            //打印模拟转运
            if (basalTransport)
            {
                int delay = 1000 * 60;//延时时间
                PrintingObj.qExtrude(nozzleID, 0, "Extrude:false", 0); // close dispenser
                PrintingObj.qBasalTransPort(xStart + xCenter - GV.dX_Trans, yStart + yCenter - GV.dY_Trans, GV.dZ_Trans >= 0 ? GV.dZ_Trans : 0, delay);
            }
            PrintingObj.qEndPrinting("END");
        }

        /// <summary>
        /// 生成Gcode打印指令
        /// </summary>
        /// <param name="fileName">Gcode文件（.gcode）</param>
        public static void ExecuteGcode(string fileName, int prinMode)
        {
            if (PrintingObj.IsPrinting)
            {
                return;
            }
            bPausePrint = true; // 暂停打印

            allG1Counts.Clear();
            // 获取所有层G1数量
            allG1Counts = CountG1Commands(fileName);

            double xMin, xMax, yMin, yMax, zMin, zMax;
            double xFirst, yFirst, zFirst;//匹配第一个打印点
            // ParseMinMaxGcode(fileName, out xMin, out xMax, out yMin, out yMax, out zMin, out zMax);
            //只看G0G1的打印范围获取第一个打印点的坐标
            ParseMinMaxG0G1(fileName, out xMin, out xMax, out yMin, out yMax, out zMin, out zMax, out xFirst, out yFirst);

            double vprint = printingSpeedSet;
            double vmove = jumpSpeedSet;

            double zoominXY = 1.0;//1：1缩放
            double zoominZ = 1.0;
            int[] Axes = new int[4];
            Axes[0] = GV.X;
            Axes[1] = GV.Y;
            Axes[2] = GV.Z;
            Axes[3] = -1;

            int[] AxesXY = new int[3];
            AxesXY[0] = GV.X;
            AxesXY[1] = GV.Y;
            AxesXY[2] = -1;

            double[] centerPoint = new double[] { 0, 0 };
            double[] startPointXY = new double[] { 0, 0 };
            double[] endPointXY = new double[] { 0, 0 };
            double[] startPointXYZ = new double[] { 0, 0, 0 };
            double[] targetPointXYZ = new double[] { 0, 0, 0 };
            double xLast, yLast, zLast;
            double xLastLast, yLastLast, zLastLast;
            double xOffset, yOffset, zOffset;
            double xOffset1, yOffset1, zOffset1;
            string cmdNameLast = "";
            string[] strLines;
            string str;
            Regex rgx, rgxCmdName;
            Match match;
            int layer = 1, layerCount = 1;
            double valueX = 0, valueY = 0, valueZ = 0, valueF = 0, valueE = 0;
            int valueP = 0;//pauseTime
            int valueW = 0;
            bool flagX = false, flagY = false, flagZ = false, flagF = false, flagE = false, flagP = false, flagW = false;
            bool extruding = false;
            int insegment = 0;
            bool _firstXYPairFound = false;
            try
            {
                //string strText = GV.ReadTextFile(fileName); //GV.Read(txtGcodeFileName.Text);
                //strLines = strText.Split(new char[2] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                //for (int i = 0; i < strLines.Length; i++)
                //{
                //    strLines[i] = strLines[i].TrimStart();
                //}

                strLines = GV.ReadTextFileLines(fileName);//按行读取

                string cmdName = "";
                string strCmd;
                rgxCmdName = new Regex(@"^(?<CmdName>\w+)\s+");

                //StartEndPrinting(true); // 开始打印

                double xStart = PrintingObj.Status.fPosX;
                double yStart = PrintingObj.Status.fPosY;

                xLast = xStart;
                yLast = yStart;
                zLast = PrintingObj.Status.fPosZ;

                xLastLast = xLast;
                yLastLast = yLast;
                zLastLast = zLast;

                xOffset = xLast - xMin;
                yOffset = yLast - yMin;
                zOffset = zLast + zMin;

                //相对点
                xOffset1 = xLast - xFirst;//当前位置-匹配的第一个打印点
                yOffset1 = yLast - yFirst;

                //计算打印区域
                double xLL, xRL, yLL, yRL, zLL, zRL;
                xLL = xOffset + xMin;
                xRL = xOffset + xMax;
                yLL = yOffset + yMin;
                yRL = yOffset + yMax;
                zLL = zOffset - zMax;
                zRL = zOffset - zMin;

                string errMsg = "";
                if (xLL < X_MIN)
                {
                    errMsg += "X左边界为" + xLL.ToString("0.0") + "，已越界。";
                }
                if (xRL > X_MAX)
                {
                    errMsg += "X右边界为" + xRL.ToString("0.0") + "，已越界。";
                }
                if (yLL < Y_MIN)
                {
                    errMsg += "Y里边界为" + yLL.ToString("0.0") + "，已越界。";
                }
                if (yRL > Y_MAX)
                {
                    errMsg += "Y外边界为" + yRL.ToString("0.0") + "，已越界。";
                }
                if (zLL < Z_MIN)
                {
                    errMsg += "Z上边界为" + zLL.ToString("0.0") + "，已越界。";
                }
                if (zRL > Z_MAX)
                {
                    errMsg += "Z下边界为" + zRL.ToString("0.0") + "，已越界。";
                }
                if (errMsg.Length > 0)
                {
                    MessageBox.Show(errMsg + "请调整打印初始位置或修订路径文件！", "异常提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                int sleepTime = GV.timeExtrudeInAdvance0;
                int advanceTime = timeCloseExtrudeInAdvance;
                targetPointXYZ[0] = xLast; targetPointXYZ[1] = yLast; targetPointXYZ[2] = zLast; //
                GV.frmPrintStep4.SetProgressbarVisible(true);
                for (int i = 0; i < strLines.Length; i++)
                {
                    // 更新本次运动的起点
                    startPointXYZ[0] = targetPointXYZ[0]; startPointXYZ[1] = targetPointXYZ[1]; startPointXYZ[2] = targetPointXYZ[2];

                    // 解析Gcode并更新本次运动的终点
                    strCmd = strLines[i].ToUpper();
                    //if (strCmd.StartsWith("G0  Z0.2"))
                    //{
                    //    Thread.Sleep(10);、                    //}
                    PrintingObj.qDisplayInfo("Gcode", strCmd);//显示gcode指令

                    if (strCmd.StartsWith(";LAYER"))//识别字符串，层数层号
                    {
                        //PrintingObj.PushDispMessage(strCmd);
                        try
                        {
                            rgx = new Regex(@"\;LAYER:\s*(?<LAYER>[-\+]{0,1}\d+)");
                            match = rgx.Match(strCmd);
                            if (match.Success)
                            {
                                str = match.Result("${LAYER}");
                                layer = Convert.ToInt32(str);
                                PrintingObj.qDisplayInfo("layerCurrent", "第" + layer.ToString() + "层, 共" + layerCount + "层", "TransParent", layer);

                                if (photoEnabled)
                                {
                                    PrintingObj.qTakePhoto((xLL + xRL) * 0.5 - GV.dX_Camera, (yLL + yRL) * 0.5 - GV.dY_Camera, GV.dZ_Camera >= 0 ? GV.dZ_Camera : 0, DateTime.Now.ToString("yyyyMMddhhmm") + "-Layer-" + layer.ToString());
                                }
                            }
                            //rgx = new Regex(@"\;LAYER COUNT:\s*(?<LAYERCOUNT>[-\+]{0,1}\d+)");
                            rgx = new Regex(@"\;LAYER[\s_]*COUNT:\s*(?<LAYERCOUNT>[-\+]{0,1}\d+)");
                            match = rgx.Match(strCmd);
                            if (match.Success)
                            {
                                str = match.Result("${LAYERCOUNT}");
                                layerCount = Convert.ToInt32(str);
                            }
                        }
                        catch (Exception ex)
                        {
                            //PrintingObj.PushErrorMessage(ex.Message);
                        }
                        cmdName = "";
                        continue;
                    }
                    // 获取命令名称（G0, G1...)
                    rgx = new Regex(@"^(?<CmdName>\w+)\s+");
                    match = rgx.Match(strCmd);
                    // 匹配失败则跳过此指令
                    if (!match.Success) continue;

                    // 匹配成功则进行解析
                    cmdName = match.Result("${CmdName}");

                    if (cmdName.StartsWith("G") || cmdName.StartsWith("M300"))
                    {
                        //// 匹配X值
                        //rgx = new Regex(@"X(?<Value>[-\+]{0,1}\d*\.{0,1}\d+)");
                        //match = rgx.Match(strCmd);
                        //if (match.Success)
                        //{
                        //    str = match.Result("${Value}");
                        //    valueX = zoominXY * Convert.ToDouble(str) + xOffset;//打印点=匹配值+当前下针位置-xmin
                        //    flagX = true;
                        //    targetPointXYZ[0] = valueX;
                        //}
                        //else
                        //{
                        //    valueX = xLast;
                        //    flagX = false;
                        //}
                        //// 匹配Y值
                        //rgx = new Regex(@"Y(?<Value>[-\+]{0,1}\d*\.{0,1}\d+)");
                        //match = rgx.Match(strCmd);
                        //if (match.Success)
                        //{
                        //    str = match.Result("${Value}");
                        //    valueY = zoominXY * Convert.ToDouble(str) + yOffset;
                        //    flagY = true;
                        //    targetPointXYZ[1] = valueY;
                        //}
                        //else
                        //{
                        //    valueY = yLast;
                        //    flagY = false;
                        //}

                        // 同时匹配X和Y
                        var xyMatch = Regex.Match(strCmd, @"X(?<XValue>[-\+]?\d*\.?\d+)\s+Y(?<YValue>[-\+]?\d*\.?\d+)");
                        if (xyMatch.Success)
                        {
                            // 处理X值
                            string xStr = xyMatch.Groups["XValue"].Value;
                            valueX = zoominXY * Convert.ToDouble(xStr) + xOffset1;
                            flagX = true;
                            targetPointXYZ[0] = valueX;

                            // 处理Y值
                            string yStr = xyMatch.Groups["YValue"].Value;
                            valueY = zoominXY * Convert.ToDouble(yStr) + yOffset1;
                            flagY = true;
                            targetPointXYZ[1] = valueY;

                            _firstXYPairFound = true;
                        }
                        else if (_firstXYPairFound)
                        {
                            // 如果已经找到第一个XY对，则允许单独匹配X或Y
                            // 单独匹配X值
                            var xMatch = Regex.Match(strCmd, @"X(?<Value>[-\+]?\d*\.?\d+)");
                            if (xMatch.Success)
                            {
                                str = xMatch.Groups["Value"].Value;
                                valueX = zoominXY * Convert.ToDouble(str) + xOffset1;
                                flagX = true;
                                targetPointXYZ[0] = valueX;
                            }
                            else
                            {
                                valueX = xLast;
                                flagX = false;
                            }

                            // 单独匹配Y值
                            var yMatch = Regex.Match(strCmd, @"Y(?<Value>[-\+]?\d*\.?\d+)");
                            if (yMatch.Success)
                            {
                                str = yMatch.Groups["Value"].Value;
                                valueY = zoominXY * Convert.ToDouble(str) + yOffset1;
                                flagY = true;
                                targetPointXYZ[1] = valueY;
                            }
                            else
                            {
                                valueY = yLast;
                                flagY = false;
                            }
                        }
                        else
                        {
                            // 首次匹配且没有同时找到X和Y，忽略单独出现的X或Y
                            valueX = xLast;
                            valueY = yLast;
                            flagX = false;
                            flagY = false;
                        }

                        // 匹配Z值
                        rgx = new Regex(@"Z(?<Value>[-\+]{0,1}\d*\.{0,1}\d+)");
                        match = rgx.Match(strCmd);
                        if (match.Success)
                        {
                            str = match.Result("${Value}");
                            valueZ = zOffset - zoominZ * Convert.ToDouble(str);
                            flagZ = true;
                            targetPointXYZ[2] = valueZ;
                        }
                        else
                        {
                            valueZ = zLast;
                            flagZ = false;
                        }
                        // 匹配F值
                        rgx = new Regex(@"F(?<Value>[-\+]{0,1}\d*\.{0,1}\d+)");
                        match = rgx.Match(strCmd);
                        if (match.Success)
                        {
                            str = match.Result("${Value}");
                            valueF = Convert.ToDouble(str);
                            flagF = true;
                        }
                        else
                        {
                            valueF = 0;
                            flagF = false;
                        }
                        // 匹配E值
                        rgx = new Regex(@"E(?<Value>[-\+]{0,1}\d*\.{0,1}\d+)");
                        match = rgx.Match(strCmd);
                        if (match.Success)
                        {
                            str = match.Result("${Value}");
                            valueE = Convert.ToDouble(str);
                            flagE = true;
                        }
                        else
                        {
                            valueE = 0;
                            flagE = false;
                        }
                        // 匹配P值
                        rgx = new Regex(@"P(?<Value>[-\+]{0,1}\d*\.{0,1}\d+)");
                        match = rgx.Match(strCmd);
                        if (match.Success)
                        {
                            str = match.Result("${Value}");
                            valueP = Convert.ToInt32(str);
                            flagP = true;
                        }
                        else
                        {
                            valueP = 0;
                            flagP = false;
                        }
                        //匹配M300
                        if (cmdName.StartsWith("M300"))//提示指令
                        {
                            //M300 P100
                            // rgx = new Regex(@"P(?<Value>[-\+]{0,1}\d*\.{0,1}\d+)");
                            rgx = new Regex(@"M300\s+P(?<Value>\d+)"); // 匹配 M300 P<数字>
                            match = rgx.Match(strCmd);
                            if (match.Success)
                            {
                                str = match.Result("${Value}");
                                valueW = Convert.ToInt32(str);
                                flagW = true;
                            }
                            else
                            {
                                valueW = 0;
                                flagW = false;
                            }
                        }
                    }
                    xLast = startPointXYZ[0]; yLast = startPointXYZ[1]; zLast = startPointXYZ[2];
                    startPointXY[0] = xLast; startPointXY[1] = yLast;
                    endPointXY[0] = targetPointXYZ[0]; endPointXY[1] = targetPointXYZ[1];
                    //每次运动，写入相应的字符串指令
                    switch (cmdName)
                    {
                        // 根据命令不同分别进行解析
                        case "G0":
                            extruding = false;
                            if ((flagX || flagY) && !flagZ) // X-Y联动
                            {
                                switch (insegment)//insegment值为0：不在Segment中；1：在不出丝的Segment中；2：在出丝的Segment中
                                {
                                    case 0://不在Segment中，
                                        // 确认不再出丝
                                        PrintingObj.qWaitMoveEnd(0, startPointXYZ, "WaitMoveEnd");
                                        PrintingObj.qExtrude(portExtrude, 0, "Extrude:false");
                                        // 进入不出丝的Segment
                                        //double juncVelocity = vmove > 3 ? 3 : vmove;
                                        PrintingObj.qWaitMoveEnd(0, startPointXYZ, "WaitMoveEnd");
                                        PrintingObj.qSegmentedMotion(AxesXY, startPointXY, "SegmentedMotion");
                                        insegment = 1;
                                        break;
                                    case 2://在出丝的Segment中，先结束之
                                        PrintingObj.qEndSequenceM(AxesXY, "EndSequence");
                                        // 确认不再出丝
                                        PrintingObj.qWaitMoveEnd(0, startPointXYZ, "WaitMoveEnd");
                                        PrintingObj.qExtrude(portExtrude, 0, "Extrude:false");
                                        // 进入不出丝的Segment
                                        //double juncVelocity = vmove > 3 ? 3 : vmove;
                                        PrintingObj.qWaitMoveEnd(0, startPointXYZ, "WaitMoveEnd");
                                        PrintingObj.qSegmentedMotion(AxesXY, startPointXY, "SegmentedMotion");
                                        insegment = 1;
                                        break;
                                        //case 1:
                                        //    break;
                                }
                                // 在Segment中添加线段
                                PrintingObj.qSegmentLine(AxesXY, startPointXY, endPointXY, vmove, vmove, layer, strCmd);
                                xLastLast = xLast; yLastLast = yLast; zLastLast = zLast;
                                xLast = valueX; yLast = valueY; zLast = valueZ;
                            }
                            else if (!flagX && !flagY && flagZ) // 仅Z轴运动
                            {
                                // 退出Segments且停止出丝
                                CheckEndSegment(ref insegment, AxesXY, startPointXYZ, extruding);
                                // 单轴运动到目标位置
                                PrintingObj.qMoveAxisTo(GV.Z, valueZ, vmove, zLast, layer, strCmd);
                                PrintingObj.qWaitMoveEnd(0, startPointXYZ, "WaitMoveEnd");
                            }
                            else if (!flagX && flagY && flagZ) // Y-Z联动
                            {
                                // 退出Segments且停止出丝
                                CheckEndSegment(ref insegment, AxesXY, startPointXYZ, extruding);
                                PrintingObj.qMoveAxisTo(GV.Z, valueZ, vmove, zLast, layer, strCmd);
                                PrintingObj.qMoveAxisTo(GV.Y, valueY, vmove, yLast, layer, strCmd);
                                PrintingObj.qWaitMoveEnd(0, targetPointXYZ, "WaitMoveEnd");
                            }
                            else if (flagX && !flagY && flagZ) // X-Z联动
                            {
                                // 退出Segments且停止出丝
                                CheckEndSegment(ref insegment, AxesXY, startPointXYZ, extruding);
                                PrintingObj.qMoveAxisTo(GV.Z, valueZ, vmove, zLast, layer, strCmd);
                                PrintingObj.qMoveAxisTo(GV.X, valueX, vmove, xLast, layer, strCmd);
                                PrintingObj.qWaitMoveEnd(0, targetPointXYZ, "WaitMoveEnd");
                            }
                            else if (flagX && flagY & flagZ) //X-Y-Z联动
                            {
                                // 退出Segments且停止出丝
                                CheckEndSegment(ref insegment, AxesXY, startPointXYZ, extruding);
                                //PrintingObj.qMoveXYZTo(valueX, valueY, valueZ, vprint, xLast, yLast, zLast, layer, strCmd);
                                PrintingObj.qMoveAxisTo(GV.Z, valueZ, vmove, zLast, layer, strCmd);
                                PrintingObj.qWaitMoveEnd(0, targetPointXYZ, "WaitMoveEnd");
                                PrintingObj.qMoveAxisTo(GV.X, valueX, vmove, xLast, layer, strCmd);
                                PrintingObj.qMoveAxisTo(GV.Y, valueY, vmove, yLast, layer, strCmd);
                                PrintingObj.qWaitMoveEnd(0, targetPointXYZ, "WaitMoveEnd");
                            }
                            else
                            {
                                // 忽略这种情况
                            }
                            break;
                        case "G1":
                            extruding = true;
                            if ((flagX || flagY) && !flagZ)  // X-Y联动
                            {
                                switch (insegment)
                                {
                                    case 0:
                                        // 确保开始出丝
                                        PrintingObj.qWaitMoveEnd(1, startPointXYZ, "WaitMoveEnd");
                                        PrintingObj.qExtrude(portExtrude, 1, "Extrude:true");
                                        if (sleepTime > 0) PrintingObj.qPause(sleepTime); // open dispenser in advance
                                        // 进入出丝的Segment
                                        //double juncVelocity = vprint > 3 ? 3 : vprint;
                                        PrintingObj.qWaitMoveEnd(1, startPointXYZ, "WaitMoveEnd");
                                        PrintingObj.qSegmentedMotion(AxesXY, startPointXY, "SegmentedMotion");
                                        insegment = 2;
                                        break;
                                    case 1://不出丝要先开始出丝
                                        PrintingObj.qEndSequenceM(AxesXY, "EndSequence"); //在不出丝的Segment中，先结束之
                                        // 确保开始出丝
                                        PrintingObj.qWaitMoveEnd(1, startPointXYZ, "WaitMoveEnd");
                                        PrintingObj.qExtrude(portExtrude, 1, "Extrude:true");
                                        if (sleepTime > 0) PrintingObj.qPause(sleepTime); // open dispenser in advance
                                        // 进入出丝的Segment
                                        //double juncVelocity = vprint > 3 ? 3 : vprint;
                                        PrintingObj.qWaitMoveEnd(1, startPointXYZ, "WaitMoveEnd");
                                        PrintingObj.qSegmentedMotion(AxesXY, startPointXY, "SegmentedMotion");
                                        insegment = 2;
                                        break;
                                        //case 2:
                                        //    break;
                                }
                                // 在Segment中添加线段
                                PrintingObj.qSegmentLine(AxesXY, startPointXY, endPointXY, vprint, vprint, layer, strCmd);
                                xLastLast = xLast; yLastLast = yLast; zLastLast = zLast;
                                xLast = valueX; yLast = valueY; zLast = valueZ;
                            }
                            else if (!flagX && !flagY && flagZ) // 仅Z轴运动
                            {
                                // 退出Segments且确保在出丝
                                CheckEndSegment(ref insegment, AxesXY, startPointXYZ, extruding);
                                PrintingObj.qMoveAxisTo(GV.Z, valueZ, vprint, zLast, layer, strCmd);
                                PrintingObj.qWaitMoveEnd(1, targetPointXYZ, "WaitMoveEnd");
                            }
                            else if (!flagX && flagY && flagZ) // Y-Z联动
                            {
                                // 退出Segments且确保在出丝
                                CheckEndSegment(ref insegment, AxesXY, startPointXYZ, extruding);
                                PrintingObj.qMoveAxisTo(GV.Z, valueZ, vprint, zLast, layer, strCmd);
                                PrintingObj.qMoveAxisTo(GV.Y, valueY, vprint, yLast, layer, strCmd);
                                PrintingObj.qWaitMoveEnd(0, targetPointXYZ, "WaitMoveEnd");
                            }
                            else if (flagX && !flagY && flagZ) // X-Z联动
                            {
                                // 退出Segments且确保在出丝
                                CheckEndSegment(ref insegment, AxesXY, startPointXYZ, extruding);
                                PrintingObj.qMoveAxisTo(GV.Z, valueZ, vprint, zLast, layer, strCmd);
                                PrintingObj.qMoveAxisTo(GV.X, valueX, vprint, xLast, layer, strCmd);
                                PrintingObj.qWaitMoveEnd(0, targetPointXYZ, "WaitMoveEnd");
                            }
                            else if (flagX && flagY & flagZ) // X-Y-Z联动
                            {
                                // 退出Segments且确保在出丝
                                CheckEndSegment(ref insegment, AxesXY, startPointXYZ, extruding);
                                //PrintingObj.qMoveXYZTo(valueX, valueY, valueZ, vprint, xLast, yLast, zLast, layer, strCmd);
                                PrintingObj.qMoveAxisTo(GV.Z, valueZ, vprint, zLast, layer, strCmd);
                                PrintingObj.qWaitMoveEnd(0, targetPointXYZ, "WaitMoveEnd");
                                PrintingObj.qMoveAxisTo(GV.X, valueX, vprint, xLast, layer, strCmd);
                                PrintingObj.qMoveAxisTo(GV.Y, valueY, vprint, yLast, layer, strCmd);
                                PrintingObj.qWaitMoveEnd(0, targetPointXYZ, "WaitMoveEnd");
                            }
                            else
                            {
                                // 忽略这种情况
                            }
                            break;
                        case "G4"://G4指令为等待延时指令
                            // 退出Segments且确保在出丝
                            if (flagP)
                            {
                                CheckEndSegment(ref insegment, AxesXY, startPointXYZ, extruding);
                                //蜂鸣器
                                //PrintingObj.qExtrude(5, 1,"Attention:true");
                                //PrintingObj.qPause(100);//100ms                           
                                //PrintingObj.qExtrude(5, 0, "Attention:false");
                                //valueP延时
                                PrintingObj.qPause(valueP);

                            }
                            break;
                        case "M300":
                            if (flagW && !Scilence)
                            {
                                CheckEndSegment(ref insegment, AxesXY, startPointXYZ, extruding);
                                //蜂鸣器
                                //添加静音
                                PrintingObj.qExtrude(5, 1, "Attention:true");
                                PrintingObj.qPause(valueW);
                                PrintingObj.qExtrude(5, 0, "Attention:false");
                            }
                            break;
                        default:
                            // 退出Segments
                            //if (insegment != 0)
                            //{
                            //    PrintingObj.qEndSequenceM(AxesXY);
                            //    PrintingObj.qWaitMoveEnd();
                            //    insegment = 0;
                            //}
                            //// 停止出丝
                            //if (extruding)
                            //{
                            //    PrintingObj.qWaitMoveEnd();
                            //    PrintingObj.qExtrude(0);
                            //    extruding = false;
                            //}
                            break;
                    }

                    cmdNameLast = cmdName; // 记录上一次命令
                    if (i % 1000 == 0)
                    {
                        int percent = 100 * i / strLines.Length;
                        GV.frmPrintStep4.UpdatePrintPercent(percent);//显示百分比
                    }
                }
                GV.frmPrintStep4.SetProgressbarVisible(false);
                // 退出Segments且确保停止出丝
                extruding = false;
                CheckEndSegment(ref insegment, AxesXY, targetPointXYZ, extruding);
                if (photoEnabled)
                {
                    PrintingObj.qTakePhoto((xLL + xRL) * 0.5 - GV.dX_Camera, (yLL + yRL) * 0.5 - GV.dY_Camera, GV.dZ_Camera >= 0 ? GV.dZ_Camera : 0, DateTime.Now.ToString("yyyyMMddhhmm") + "-Layer-" + layer.ToString());
                }

                // //打印结束提针
                string strGcode1 = "G0 Z" + (0).ToString();
                PrintingObj.qMoveAxisTo(Z, Z_TOP, vmove, zLast, 0, strGcode1); zLast = 0;
                PrintingObj.qWaitMoveEnd();
                PrintingObj.qEndPrinting("END");
            }
            catch (Exception ex)
            {
                //PrintingObj.PushErrorMessage(ex.Message);
                PrintingObj.qEndPrinting("Error END");
                return;
            }
        }

        /// <summary>
        /// 新增自定义连续多文件打印指令，A为主工位，B为辅助工位，耦合在同一个XYZ主轴上
        /// </summary>
        /// <param name="fileNames">多个打印路径文件</param>
        /// <param name="printMode">打印模式</param>
        /// <param name="fileCounts">最多文件数量的工位</param>
        /// <param name="printStage">打印工位，0：A工位1：B工位，2：双工位</param>
        public static void ExecuteCustomedMultiCubes(List<string> fileNamesA, List<string> fileNamesB, int printMode)
        {
            if (PrintingObj.IsPrinting)
            {
                return;
            }
            bPausePrint = true; // 暂停打印
            PrintingObj.DataObj.CmdQueue.Clear();//清除队列
            //GV.PrintingObj.PullUpPenSensor();//抬起位移笔

            //读取自定义网格文件
            string[][] strCSV;
            double[][] matrixP;
            List<double[][]> list_matrixP = new List<double[][]>();//存储所有的点

            int nozzleID = 0;

            //导入路径
            int countA = fileNamesA.Count;
            int countB = fileNamesB.Count;
            int stage = 0;
            if (countA != 0 && countB != 0) stage = 2;//双工位
            else if (countA != 0 && countB == 0) stage = 0;//A工位
            else stage = 1;//B工位
            int fileCounts = Math.Max(countA, countB);

            int portExtrudeA = 0, portExtrudeB = 1;//喷头出丝
            bool printA = false, printB = false;//该工位是否打印

            //读取所有的运动点
            try
            {
                switch (stage)//查看打印工位
                {
                    case 0://只有A工位打印
                        printA = true;
                        printB = false;
                        for (int fileIndex = 0; fileIndex < countA; fileIndex++)
                        {
                            if (!File.Exists(fileNamesA[fileIndex]))
                            {
                                MessageBox.Show("路径文件不存在！" + fileNamesA[fileIndex], "异常提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                            strCSV = CsvFileReader.Read(fileNamesA[fileIndex]);//选择文件序列
                            matrixP = new double[strCSV.Length][];
                            for (int i = 0; i < strCSV.Length; i++)
                            {
                                List<double> listP = new List<double>();
                                for (int j = 0; j < strCSV[i].Length; j++)
                                {
                                    if (strCSV[i][j].Trim().Length > 0)
                                    {
                                        listP.Add(Convert.ToDouble(strCSV[i][j]));
                                    }
                                }
                                matrixP[i] = listP.ToArray();
                            }
                            list_matrixP.Add(matrixP);
                        }
                        break;
                    case 1://只有B工位打印
                        printA = false;
                        printB = true;
                        for (int fileIndex = 0; fileIndex < countB; fileIndex++)
                        {
                            if (!File.Exists(fileNamesB[fileIndex]))
                            {
                                MessageBox.Show("路径文件不存在！" + fileNamesB[fileIndex], "异常提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                            strCSV = CsvFileReader.Read(fileNamesB[fileIndex]);//选择文件序列
                            matrixP = new double[strCSV.Length][];
                            for (int i = 0; i < strCSV.Length; i++)
                            {
                                List<double> listP = new List<double>();
                                for (int j = 0; j < strCSV[i].Length; j++)
                                {
                                    if (strCSV[i][j].Trim().Length > 0)
                                    {
                                        listP.Add(Convert.ToDouble(strCSV[i][j]));
                                    }
                                }
                                matrixP[i] = listP.ToArray();
                            }
                            list_matrixP.Add(matrixP);
                        }
                        break;
                    case 2://双工位打印,存在一个工位文件数量大于另一个工位的情况（未解决）
                        printA = true;
                        printB = true;
                        for (int fileIndex = 0; fileIndex < fileCounts; fileIndex++)
                        {
                            if (!File.Exists(fileNamesA[fileIndex]))
                            {
                                MessageBox.Show("路径文件不存在！" + fileNamesA[fileIndex], "异常提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                            if (countA >= countB)//同步的，优先应用文件数多的序列
                            {
                                strCSV = CsvFileReader.Read(fileNamesA[fileIndex]);//选择文件序列
                            }
                            else
                            {
                                strCSV = CsvFileReader.Read(fileNamesB[fileIndex]);//选择文件序列
                            }
                            matrixP = new double[strCSV.Length][];
                            for (int i = 0; i < strCSV.Length; i++)
                            {
                                List<double> listP = new List<double>();
                                for (int j = 0; j < strCSV[i].Length; j++)
                                {
                                    if (strCSV[i][j].Trim().Length > 0)
                                    {
                                        listP.Add(Convert.ToDouble(strCSV[i][j]));
                                    }
                                }
                                matrixP[i] = listP.ToArray();
                            }
                            list_matrixP.Add(matrixP);
                        }
                        break;
                }

                matrixP = list_matrixP[0];//初始数据集
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }


            double[] p;
            double[] m;
            int axis;
            double vprint = printingSpeedSet;
            double vmoveXY = jumpSpeedSet;
            double vmoveZ = vmoveXY > 30 ? 30 : vmoveXY; ;
            double zprint,slightMoveZa,slightMoveZb, zmove;
            double liftHeight = 2;      // 移动时提针高度
            double layerHeight = 10;    // 打印层高/层
            double speedRotaryValveA, speedRotaryValveB;//螺杆转速
            double rotSpeed;//螺杆转速
            int sleepTime = GV.timeExtrudeInAdvance0;
            int advanceTime = timeCloseExtrudeInAdvance;
            int im = 0, im0 = 0; // im: index of max/min value (0: min; 1: max)
            int ip = 0, ip0 = 0, ip_next;    // ip: index of line postion

            double xLast, yLast, zLast;//主轴上次位置
            double z1Last = 0, z2Last = 0;//小Z轴上次位置

            double xEnd, yEnd, zEnd;//记录上一个工件结束时的位置
            double xNext, yNext, zNext;//计算下一个工件开始时的位置

            double[] xCenter = new double[fileCounts]; //样品中心
            double[] yCenter = new double[fileCounts];
            VertexPos lastPos = VertexPos.LeftBottom;
            bool isClockwise = false;

            xStart = GV.X_INIT;// printingObj.Status.fPosX;固定工位位置
            yStart = GV.Y_INIT;// printingObj.Status.fPosY;
            zStart = GV.Z_BOTTOM;// printingObj.Status.fPosZ;

            GV.listTargetZ[0] = zStart;//主Z下针高度


            int[] Axes = new int[3];
            Axes[0] = X;
            Axes[1] = Y;
            Axes[2] = -1;
            double[] centerPoint = new double[] { 0.0, 0.0 };
            double[] startPoint = new double[] { 0.0, 0.0 };
            double[] endPoint = new double[] { 0.0, 0.0 };
            string strGcodeA = "";//主工位显示路径
            string strGcodeB = "";//B工位显示路径

            int countSample = 0;//记录打印样品数量
            // xNext = xStart + xCenter * 2.5; yNext = yStart + yCenter * 2.5; zNext = zprint;
            zprint = zStart;
            xLast = xStart; yLast = yStart; zLast = zprint;//初始化起点
            for (int iPrint = 0; iPrint < fileCounts; iPrint++)
            {
                //移动数据
                matrixP = list_matrixP[iPrint];

                if (listPrintPosA[iPrint]) // 如果该工位不处于打印状态
                {
                    printA = true;
                }
                else if (listPrintPosB[iPrint])
                {
                    printB = true;
                }

                //计算样品几何中心坐标A,用于针头切换
                double xc = 0, yc = 0;
                for (int i = 0; i < matrixP.Length; i++)
                {
                    if (matrixP[i][2] == 0) // X轴方向
                    {
                        xc = (matrixP[i][3] + matrixP[i][4]) / 2;
                    }
                    else // Y轴方向
                    {
                        yc = (matrixP[i][3] + matrixP[i][4]) / 2;
                    }
                }
                xCenter[iPrint] = xc; yCenter[iPrint] = yc;

                //根据编号配置相关参数
                GV.PrintingObj.qSetRotarySpeed(0, GV.listRotateSpeedA[iPrint]);//确定工位螺杆转速
                GV.PrintingObj.qSetPressure(0, GV.listRotateSpeedA[iPrint]);//确定工位供料气压，默认为port0
                vprint = GV.listPrintSpeed[iPrint] > 0 ? GV.listPrintSpeed[iPrint] : vprint;//默认或选定打印速度

                if (GV.listTargetZ[iPrint] <= 0 || GV.listTargetZ == null)//若未使用自动对针
                {
                    // 尚未对针，直接使用默认下针位置来预览路径
                    zprint = zStart;
                }
                else
                {
                    zprint = GV.listTargetZ[iPrint];    //读取工位的下针高度 主z + 小z

                    speedRotaryValveA = GV.listRotateSpeedA[iPrint];  // 读取工位的螺杆转速
                    speedRotaryValveB = GV.listRotateSpeedB[iPrint];                 
                }
                PrintingObj.qWaitMoveEnd(); // wait until print-head moved to line start
                //提针，计数打印工位
                if (countSample > 0) // 如果不是第一个样品
                {
                    strGcodeA = "G0 Z" + zprint.ToString();
                    // 先提针20mm再平移
                    PrintingObj.qMoveAxisTo(Z, zprint - 20, vmoveZ, zLast, 0, strGcodeA);
                    PrintingObj.qAdjustMicroMotor(Z1, 0, z1Last);//微调平台
                    PrintingObj.qAdjustMicroMotor(Z2, 0, z2Last);
                    zLast = zprint;
                    PrintingObj.qWaitMoveEnd();
                    // 暂停等待上一个样品出丝结束
                    PrintingObj.qExtrude(portExtrude, 0); // close dispenser
                    PrintingObj.qPause(5000);
                    //PrintingObj.qExtrude(portExtrude, 1); // close dispenser
                }
                else // 第一个样品
                {
                    // 先提针至安全高度
                    PrintingObj.qMoveAxisTo(Z, GV.Z_TOP, vmoveZ, zLast, 0, strGcodeA);
                    zLast = GV.Z_TOP;
                    PrintingObj.qWaitMoveEnd();
                }
                countSample++;//样品计数
                GV.PrintingObj.qSetPrintPosStartEnd(printA, printB, iPrint);//更新打印工位状态

                lastPos = VertexPos.LeftBottom;//初始化起始位置，左上角开始打印

                matrixP = list_matrixP[iPrint];//工位样品解析    
               
                //逐层生成路径
                for (int layer = 0; layer < matrixP.Length; layer++)
                {
                    PrintingObj.qDisplayInfo("layerCurrent", "第" + (layer + 1).ToString() + "层, 共" + matrixP.Length + "层", "TransParent", layer);

                    //读取单层的数据
                    ReadLayerData(matrixP, xStart, yStart, layer, GV.setLayerRotation, GV.layerRotation, out layerHeight, out liftHeight, out axis, out m, out p, out rotSpeed);

                    if (axis == 0) // 本层线条沿X轴方向
                    {
                        switch (lastPos)
                        {
                            case VertexPos.LeftBottom:
                                im0 = 0; ip0 = 0; isClockwise = false;
                                break;
                            case VertexPos.LeftTop:
                                im0 = 0; ip0 = p.Length - 1; isClockwise = true;
                                break;
                            case VertexPos.RightBottom:
                                im0 = 1; ip0 = 0; isClockwise = true;
                                break;
                            case VertexPos.RightTop:
                                im0 = 1; ip0 = p.Length - 1; isClockwise = false;
                                break;
                            default:
                                break;
                        }
                        // move to start point of this layer
                        if (layer == 0) // 第一层增加一段延长线
                        {
                            double xx = m[im0] - GV.lenAdvanced;
                            strGcodeA = "G0 X" + xx.ToString() + " Y" + p[ip0].ToString();   // G0 X93.875 Y93.875
                            PrintingObj.qMoveXYTo(xx, p[ip0], vmoveXY, xLast, yLast, layer, strGcodeA);
                            xLast = xx;
                        }
                        else
                        {
                            strGcodeA = "G0 X" + m[im0].ToString() + " Y" + p[ip0].ToString();   // G0 X93.875 Y93.875
                            PrintingObj.qMoveXYTo(m[im0], p[ip0], vmoveXY, xLast, yLast, layer, strGcodeA);
                            xLast = m[im0];
                        }
                        yLast = p[ip0];
                    }
                    else // 本层线条沿Y轴方向
                    {
                        switch (lastPos)
                        {
                            case VertexPos.LeftBottom:
                                im0 = 0; ip0 = 0; isClockwise = true;
                                break;
                            case VertexPos.LeftTop:
                                im0 = 1; ip0 = 0; isClockwise = false;
                                break;
                            case VertexPos.RightBottom:
                                im0 = 0; ip0 = p.Length - 1; isClockwise = false;
                                break;
                            case VertexPos.RightTop:
                                im0 = 1; ip0 = p.Length - 1; isClockwise = true;
                                break;
                            default:
                                break;
                        }
                        // move to start point of this layer
                        strGcodeA = "G0 X" + p[ip0].ToString() + " Y" + m[im0].ToString();   // G0 X93.875 Y93.875
                        PrintingObj.qMoveXYTo(p[ip0], m[im0], vmoveXY, xLast, yLast, layer, strGcodeA);
                        xLast = p[ip0]; yLast = m[im0];
                    }
                    PrintingObj.qWaitMoveEnd("WaitMoveEnd"); // wait until print-head moved to line start
                    strGcodeA = "G0 Z" + zprint.ToString();
                    PrintingObj.qMoveAxisTo(Z, zprint, vmoveZ, zLast, layer, strGcodeA);//移动到打印z高度
                    zLast = zprint;
                    PrintingObj.qWaitMoveEnd("WaitMoveEnd");

                    if (printMode == 0)
                    {
                        nozzleID = 0;// layer % 2;  // 单喷头                 
                    }
                    else if (printMode == 1)
                    {
                        nozzleID = layer % 2;  // 选择该层使用的喷头：奇数层用左喷头，偶数层用右喷头                        
                    }
                    else
                    {
                        nozzleID = -2;  // 所有喷头同时打开                        
                    }
                    PrintingObj.qSwitchNozzle(nozzleID); // 切换喷头
                    PrintingObj.qExtrude(nozzleID, 1, "Extrude:true"); // open dispenser
                    if (sleepTime > 0) PrintingObj.qPause(sleepTime); // open dispenser in advance
                    //sleepTime = 0;
                    // 在当前位置建立一段SegmentedMotion
                    startPoint = new double[] { xLast, yLast };
                    //PrintingObj.qSegmentedMotion(Axes, startPoint, "SegmentedMotion");
                    PrintingObj.qExtSegmentedMotion(Axes, startPoint, vprint, vprint, 0, 0);//202503,转角处减速，缓冲震动

                    im = im0;
                    // 本层除了最后一条线，都需在最后一条线加一个半圆弧。
                    for (int i = 0; i < p.Length - 1; i++)
                    {
                        if (ip0 == 0)    // draw lines from min to max
                        {
                            ip = i;
                            ip_next = ip + 1;
                        }
                        else                // draw lines from max to min
                        {
                            ip = ip0 - i;
                            ip_next = ip - 1;
                        }
                        PrintingObj.qDisplayInfo("filamentNumber", "本层第" + (i + 1).ToString() + "条, 共" + p.Length + "条");

                        im = 1 - im; // swap im (min->max or max->min) 相对本层第一条线的起点位置进行反向
                        if (axis == 0)
                        {
                            endPoint = new double[2] { m[im], p[ip] };
                            strGcodeA = "G1 X" + endPoint[0].ToString() + " Y" + endPoint[1].ToString();
                            PrintingObj.qSegmentLine(Axes, startPoint, endPoint, vprint, vprint, layer, strGcodeA);
                            startPoint = endPoint;
                            endPoint = new double[2] { m[im], p[ip_next] };
                            centerPoint = new double[2] { m[im], (p[ip] + p[ip_next]) * 0.5 };

                            if (isClockwise)
                            {
                                strGcodeA = "G2 XC" + centerPoint[0].ToString() + " YC" + centerPoint[1].ToString() + " X" + endPoint[0].ToString() + " Y" + endPoint[1].ToString() + " R" + Math.PI.ToString();
                            }
                            else
                            {
                                strGcodeA = "G3 XC" + centerPoint[0].ToString() + " YC" + centerPoint[1].ToString() + " X" + endPoint[0].ToString() + " Y" + endPoint[1].ToString() + " R" + Math.PI.ToString();
                            }
                            PrintingObj.qSegmentArc2(Axes, centerPoint, endPoint, (isClockwise ? -1 : 1) * Math.PI, vprint, vprint, layer, strGcodeA);
                            //PrintingObj.qMoveAxisTo(axis, m[im], vprint, xLast); PrintingObj.qWaitMoveEnd(); // move to line end                           
                            xLast = m[im];
                        }
                        else
                        {
                            endPoint = new double[2] { p[ip], m[im] };
                            strGcodeA = "G1 X" + endPoint[0].ToString() + " Y" + endPoint[1].ToString();
                            PrintingObj.qSegmentLine(Axes, startPoint, endPoint, vprint, vprint, layer, strGcodeA);
                            startPoint = endPoint;
                            endPoint = new double[2] { p[ip_next], m[im] };
                            centerPoint = new double[2] { (p[ip] + p[ip_next]) * 0.5, m[im] };
                            if (isClockwise)
                            {
                                strGcodeA = "G2 XC" + centerPoint[0].ToString() + " YC" + centerPoint[1].ToString() + " X" + endPoint[0].ToString() + " Y" + endPoint[1].ToString() + " R" + Math.PI.ToString();
                            }
                            else
                            {
                                strGcodeA = "G3 XC" + centerPoint[0].ToString() + " YC" + centerPoint[1].ToString() + " X" + endPoint[0].ToString() + " Y" + endPoint[1].ToString() + " R" + Math.PI.ToString();
                            }
                            PrintingObj.qSegmentArc2(Axes, centerPoint, endPoint, (isClockwise ? -1 : 1) * Math.PI, vprint, vprint, layer, strGcodeA);
                            //PrintingObj.qMoveAxisTo(axis, m[im], vprint, yLast); PrintingObj.qWaitMoveEnd(); // move to line end                           
                            yLast = m[im];
                        }
                        isClockwise = !isClockwise; // 反向
                    }
                    // 最后一条线不需要画圆弧
                    if (ip0 == 0)    // draw lines from min to max
                    {
                        ip = p.Length - 1; // index of the last line
                    }
                    else                // draw lines from max to min
                    {
                        ip = 0;         // index of the last line
                    }
                    im = 1 - im; // swap im (min->max or max->min) 相对本层最后一条线的起点位置进行反向
                    if (axis == 0) // 沿X轴方向画最后一条线
                    {
                        endPoint = new double[2] { m[im], p[ip] };
                        strGcodeA = "G1 X" + endPoint[0].ToString() + " Y" + endPoint[1].ToString();
                        PrintingObj.qSegmentLine(Axes, startPoint, endPoint, vprint, 0, layer, strGcodeA);
                        startPoint = endPoint;
                        xLast = m[im];
                    }
                    else  // 沿Y轴方向画最后一条线
                    {
                        endPoint = new double[2] { p[ip], m[im] };
                        strGcodeA = "G1 X" + endPoint[0].ToString() + " Y" + endPoint[1].ToString();
                        PrintingObj.qSegmentLine(Axes, startPoint, endPoint, vprint, 0, layer, strGcodeA);
                        startPoint = endPoint;
                        yLast = m[im];
                    }
                    // 记录本层终点位置
                    if (axis == 0) // 线条沿X轴方向
                    {
                        if (im == 0) // Left
                        {
                            if (ip == 0) // Bottom
                            {
                                lastPos = VertexPos.LeftBottom;
                            }
                            else // Top
                            {
                                lastPos = VertexPos.LeftTop;
                            }
                        }
                        else // Right
                        {
                            if (ip == 0) // Bottom
                            {
                                lastPos = VertexPos.RightBottom;
                            }
                            else // Top
                            {
                                lastPos = VertexPos.RightTop;
                            }
                        }
                    }
                    else // 线条沿Y轴方向
                    {
                        if (im == 0) // Bottom
                        {
                            if (ip == 0) // Left
                            {
                                lastPos = VertexPos.LeftBottom;
                            }
                            else // Right
                            {
                                lastPos = VertexPos.RightBottom;
                            }
                        }
                        else // Top
                        {
                            if (ip == 0) // Left
                            {
                                lastPos = VertexPos.LeftTop;
                            }
                            else // Right
                            {
                                lastPos = VertexPos.RightTop;
                            }
                        }
                    }
                    PrintingObj.qEndSequenceM(Axes, "EndSequence");
                    PrintingObj.qWaitMoveEnd("WaitMoveEnd");
                    zprint -= layerHeight;
                    //if (photoEnabled) //如果需要逐层拍照
                    //{
                    //    PrintingObj.qExtrude(nozzleID, 0, "Extrude:false", 0); // close dispenser
                    //    PrintingObj.qTakePhoto(xStart + xCenter - GV.dX_Camera, yStart + yCenter - GV.dY_Camera, GV.dZ_Camera >= 0 ? GV.dZ_Camera : 0, DateTime.Now.ToString("yyyyMMddhhmm") + "-Layer-" + layer.ToString());
                    //}
                    if (!GV.noStopExtrude) //如果层间需要停止出丝
                    {
                        PrintingObj.qExtrude(nozzleID, 0, "Extrude:false", 0); // close dispenser
                    }
                    else if (nozzleClean)//如果需要擦针
                    {
                        //擦针层数确定
                        int cleanLayer = 0;
                        PrintingObj.qExtrude(nozzleID, 0, "Extrude:false", 0); // close dispenser
                        PrintingObj.qCleanNozzle(xStart + xCenter[iPrint] - GV.dX_Clean, yStart + yCenter[iPrint] - GV.dY_Clean, GV.dZ_Clean >= 0 ? GV.dZ_Clean : 0);
                    }
                }
            }


            PrintingObj.qWaitMoveEnd("WaitMoveEnd");
            PrintingObj.qExtrude(portExtrude, 0, "Extrude:false", 0); // close dispenser
            PrintingObj.qSwitchNozzle(0); // 切换喷头

            //打印完提针
            if (nozzleUp)
            {
                strGcodeA = "G0 Z" + (0).ToString();
                PrintingObj.qMoveAxisTo(Z, 0, vmoveXY, zLast, 0, strGcodeA); zLast = 0;
                PrintingObj.qWaitMoveEnd("WaitMoveEnd"); // nozzle up
            }
            //打印模拟转运
            if (basalTransport)
            {
                int delay = 1000 * 60;//延时时间
                PrintingObj.qExtrude(nozzleID, 0, "Extrude:false", 0); // close dispenser
                PrintingObj.qBasalTransPort(xStart + xCenter[fileCounts] - GV.dX_Trans, yStart + yCenter[fileCounts] - GV.dY_Trans, GV.dZ_Trans >= 0 ? GV.dZ_Trans : 0, delay);
            }
            PrintingObj.qEndPrinting("END");

        }

        public static void ParseMinMaxGcode(string fileName, out double xMin, out double xMax, out double yMin, out double yMax, out double zMin, out double zMax)
        {
            // 从Gcode文件中读取坐标信息
            string strGcode;
            string[] strLines;
            try
            {
                strGcode = GV.ReadTextFile(fileName);
                strLines = strGcode.Split(new char[2] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                xMin = 0; yMin = 0; zMin = 0; xMax = 0; yMax = 0; zMax = 0;
                return;
            }

            string[] strGLines = strGcode.Split(new char[2] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            double x, y, z, f, e, v;
            const int EMPTY = 99999;
            xMin = EMPTY; yMin = EMPTY; zMin = EMPTY;
            xMax = -EMPTY; yMax = -EMPTY; zMax = -EMPTY;
            string strCurLine;
            Match match, matchX, matchY, matchZ;
            string gStr, fStr, xStr, yStr, zStr, eStr;
            v = 10;
            Regex rgx = new Regex(@"^G(?<G>\d+)\s*(F(?<F>-{0,1}([1-9]\d*|0)\.{0,1}\d*)){0,1}\s*(X(?<X>-{0,1}([1-9]\d*|0)\.{0,1}\d*)){0,1}\s*(Y(?<Y>-{0,1}([1-9]\d*|0)\.{0,1}\d*)){0,1}\s*(Z(?<Z>-{0,1}([1-9]\d*|0)\.{0,1}\d*)){0,1}\s*(E(?<E>-{0,1}([1-9]\d*|0)\.{0,1}\d*)){0,1}");
            Regex rgxX = new Regex(@"G\d+(\s*\w*)*X(?<X>(-{0,1}([1-9]\d*|0)\.{0,1}\d*))");
            Regex rgxY = new Regex(@"G\d+(\s*\w*)*Y(?<Y>(-{0,1}([1-9]\d*|0)\.{0,1}\d*))");
            Regex rgxZ = new Regex(@"G\d+(\s*\w*)*Z(?<Z>(-{0,1}([1-9]\d*|0)\.{0,1}\d*))");
            byte iG = 0;

            // 匹配所有G代码，并提取关键数据；得到X、Y、Z的最值，确定运动范围。
            for (int i = 0; i < strGLines.Length; i++)
            {
                strCurLine = strGLines[i].Trim();
                // 匹配当前行
                match = rgx.Match(strCurLine);

                // 匹配成功
                if (match.Success)
                {
                    gStr = match.Result("${G}");
                    fStr = match.Result("${F}");
                    xStr = match.Result("${X}");
                    yStr = match.Result("${Y}");
                    zStr = match.Result("${Z}");
                    eStr = match.Result("${E}");
                    try
                    {
                        iG = Convert.ToByte(gStr);
                        if (xStr.Length == 0)
                        {
                            x = EMPTY;
                        }
                        else
                        {
                            x = Convert.ToDouble(xStr);
                            if (x < xMin) xMin = x;
                            if (x > xMax) xMax = x;
                        }
                        if (yStr.Length == 0)
                        {
                            y = EMPTY;
                        }
                        else
                        {
                            y = Convert.ToDouble(yStr);
                            if (y < yMin) yMin = y;
                            if (y > yMax) yMax = y;
                        }
                        if (zStr.Length == 0)
                        {
                            z = EMPTY;
                        }
                        else
                        {
                            z = Convert.ToDouble(zStr);
                            if (z < zMin) zMin = z;
                            if (z > zMax) zMax = z;
                        }
                        f = fStr.Length == 0 ? 0 : Convert.ToDouble(fStr);
                        e = eStr.Length == 0 ? 0 : Convert.ToDouble(eStr);
                    }
                    catch (Exception ex)
                    {
                        continue; // GCode解析出错，跳到下一行代码
                    }
                }
            }
        }

        public static void ParseMinMaxG0G1(string fileName, out double xMin, out double xMax, out double yMin, out double yMax, out double zMin, out double zMax, out double xFirst, out double yFirst)
        {
            // 初始化默认值
            const double EMPTY = 99999;
            xMin = EMPTY; yMin = EMPTY; zMin = EMPTY;
            xMax = -EMPTY; yMax = -EMPTY; zMax = -EMPTY;
            xFirst = 0; yFirst = 0;
            bool firstXYFound = false; // 标记是否已找到第一个XY值
            try
            {
                foreach (string line in File.ReadLines(fileName))
                {
                    string trimmedLine = line.Trim();

                    // 只处理G0/G1开头的行
                    if (!trimmedLine.StartsWith("G0") && !trimmedLine.StartsWith("G1"))
                        continue;

                    // 分割行中的各个参数
                    string[] parts = trimmedLine.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                    double? x = null, y = null, z = null;

                    foreach (string part in parts)
                    {
                        if (part.Length < 2) continue;

                        char axis = char.ToUpper(part[0]);
                        string valueStr = part.Substring(1);

                        if (!double.TryParse(valueStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                            continue;

                        switch (axis)
                        {
                            case 'X': x = value; break;
                            case 'Y': y = value; break;
                            case 'Z': z = value; break;
                        }
                    }

                    // 记录第一个有效的XY值（仅在首次找到时）
                    if (!firstXYFound && x.HasValue && y.HasValue)
                    {
                        xFirst = x.Value;
                        yFirst = y.Value;
                        firstXYFound = true;
                    }

                    // 更新最小最大值
                    if (x.HasValue)
                    {
                        xMin = Math.Min(xMin, x.Value);
                        xMax = Math.Max(xMax, x.Value);
                    }
                    if (y.HasValue)
                    {
                        yMin = Math.Min(yMin, y.Value);
                        yMax = Math.Max(yMax, y.Value);
                    }
                    if (z.HasValue)
                    {
                        zMin = Math.Min(zMin, z.Value);
                        zMax = Math.Max(zMax, z.Value);
                    }
                }

                // 如果没有找到有效坐标，设置为0
                if (xMin == EMPTY) xMin = xMax = 0;
                if (yMin == EMPTY) yMin = yMax = 0;
                if (zMin == EMPTY) zMin = zMax = 0;

                // 如果没有找到有效的第一个XY值，使用最小XY值
                if (!firstXYFound)
                {
                    xFirst = xMin != EMPTY ? xMin : 0;
                    yFirst = yMin != EMPTY ? yMin : 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"解析Gcode文件出错: {ex.Message}");
                xMin = xMax = yMin = yMax = zMin = zMax = 0;
            }
        }

        private static void CheckEndSegment(ref int insegment, int[] AxesXY, double[] targetPointXYZ, bool extruding)
        {
            // 退出Segments
            if (insegment != 0)
            {
                PrintingObj.qEndSequenceM(AxesXY, "EndSequence");
                insegment = 0;
            }
            // 确保开始或停止出丝
            PrintingObj.qWaitMoveEnd("WaitMoveEnd"); //(0, targetPointXYZ);
            if (extruding)
            {
                PrintingObj.qExtrude(portExtrude, 1, "Extrude:true");
            }
            else
            {
                PrintingObj.qExtrude(portExtrude, 0, "Extrude:false");
            }
        }

        /// <summary>
        /// a为LastLast, b为Last, c为Now, b为转角点
        /// </summary>
        /// <param name="velocity"></param>
        /// <param name="xa"></param>
        /// <param name="ya"></param>
        /// <param name="za"></param>
        /// <param name="xb"></param>
        /// <param name="yb"></param>
        /// <param name="zb"></param>
        /// <param name="xc"></param>
        /// <param name="yc"></param>
        /// <param name="zc"></param>
        /// <param name="safeAngel"></param>
        /// <param name="safeVelocity"></param>
        /// <returns></returns>
        public static bool shouldAddStopper(double velocity, double xa, double ya, double za, double xb, double yb, double zb,
            double xc, double yc, double zc, double safeAngel = 0.2, double safeVelocity = 3)
        {

            // 向量1
            double x1 = xa - xb;
            double y1 = ya - yb;
            double z1 = za - zb;

            // 向量2
            double x2 = xb - xc;
            double y2 = yb - yc;
            double z2 = zb - zc;

            // 
            double x3 = xc - xa;
            double y3 = yc - ya;
            double z3 = zc - za;

            // 求内积的模
            double innerProduct = (x1 * x2) + (y1 * y2) + (z1 * z2);
            //double innerProduct = (x1 * x2) * (x1 * x2) + (y1 * y2) * (y1 * y2) + (z1 * z2) * (z1 * z2);

            // 求模
            double mod_ab = Math.Sqrt(x1 * x1 + y1 * y1 + z1 * z1);
            double mod_bc = Math.Sqrt(x2 * x2 + y2 * y2 + z2 * z2);
            double mod_ac = Math.Sqrt(x3 * x3 + y3 * y3 + z3 * z3);
            double cosB = 0;

            if (mod_ab.Equals(0))
            {
                return true;
            }
            if (mod_ab.Equals(0) || mod_bc.Equals(0))
            {
                return false;
            }

            // 求余弦
            cosB = Math.Round(innerProduct / (mod_ab * mod_bc), 5);
            double B = Math.Acos(cosB);
            return (cosB < Math.Cos(safeAngel)) && (velocity > safeVelocity);
        }


        /// <summary>
        /// 读取文本文件
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static string ReadTextFile(string FileName)
        {
            try
            {
                string TextData;
                FileStream fs;
                StreamReader sr;
                //m = 0;
                fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                sr = new StreamReader(fs, Encoding.Default);
                //sr = new StreamReader(fs, Encoding.UTF8);
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                TextData = sr.ReadToEnd();
                fs.Close();
                sr.Close();
                return TextData;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "";
            }
        }

        /// <summary>
        /// 按行读取文本文件
        /// </summary>
        /// <param name="FileName"></param>
        /// <returns></returns>
        public static string[] ReadTextFileLines(string FileName)
        {
            try
            {
                string line;
                List<string> TextData = new List<string>();
                FileStream fs;
                StreamReader sr;
                fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                sr = new StreamReader(fs, Encoding.UTF8);
                while ((line = sr.ReadLine()) != null)
                {
                    if (line != "")
                    {
                        TextData.Add(line);
                    }
                }
                fs.Close();
                sr.Close();
                return TextData.ToArray();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 写入文本文件
        /// </summary>
        /// <param name="FileName">写入文件全名（含路径）</param>
        /// <param name="TextData">文本数据</param>
        public static bool WriteTextFile(string FileName, string TextData)
        {
            try
            {
                FileStream fs;
                StreamWriter sw;
                //m = 0;
                fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write);
                sw = new StreamWriter(fs, Encoding.Default);
                //sr = new StreamReader(fs, Encoding.UTF8);
                sw.BaseStream.Seek(0, SeekOrigin.Begin);
                sw.Write(TextData);
                sw.Close();
                fs.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 按行写入文本文件
        /// </summary>
        /// <param name="FileName">写入文件全名（含路径）</param>
        /// <param name="TextData">文本数据</param>
        public static bool WriteLineTextFile(string FileName, string[] TextData)
        {
            try
            {
                FileStream fs;
                StreamWriter sw;
                //m = 0;
                fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write);
                sw = new StreamWriter(fs, Encoding.Default);
                foreach (string s in TextData)
                {
                    sw.WriteLine(s);
                }
                sw.Close();
                fs.Close();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 将数据追加到文件的末尾，而不覆盖原有内容,文件不存在则创建文件 
        /// </summary>
        /// <param name="FileName">写入文件全名（含路径）</param>
        /// <param name="TextData">文本数据</param>
        /// <returns></returns>
        //主动写入日志文件
        public static bool AppendLineTextFile(string FileName, string[] TextData)
        {
            try
            {
                //执行完毕后自动调用 Dispose() 方法，从而关闭文件并释放相关资源
                using (StreamWriter writer = new StreamWriter(FileName, append: true))
                {
                    foreach (string s in TextData)
                    {
                        writer.WriteLine(s);  //追加一行文本,向下写一行
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 在指定的控件（如Label、Text）显示提示消息
        /// </summary>
        /// <param name="control">显示消息的控件</param>
        /// <param name="strShowText">提示信息的内容</param>
        /// <param name="timer">用于设置显示时长的Timer控件</param>
        /// <param name="showTime">显示时长（ms）</param>
        /// <param name="color">字体颜色</param>
        public static void MsgShow(Control control, string strShowText, System.Windows.Forms.Timer timer, int showTime, Color foreColor)
        {
            if (timer.Tag != null)
            {
                (timer.Tag as Control).Visible = false;
            }
            control.Text = strShowText;
            control.ForeColor = foreColor;
            control.Visible = true;
            timer.Interval = showTime;
            timer.Tag = control;
            timer.Start();
            timer.Tick += new EventHandler(tmr_Tick);
        }


        /// <summary>
        /// 在指定的控件（如Label、Text）显示提示消息
        /// </summary>
        /// <param name="control">显示消息的控件</param>
        /// <param name="strShowText">提示信息的内容</param>
        /// <param name="timer">用于设置显示时长的Timer控件</param>
        /// <param name="showTime">显示时长（ms）</param>
        public static void MsgShow(Control control, string strShowText, System.Windows.Forms.Timer timer, int showTime)
        {
            MsgShow(control, strShowText, timer, showTime, control.ForeColor);
        }

        /// <summary>
        /// 在给定时间后让指定的控件消失
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void tmr_Tick(object sender, EventArgs e)
        {
            System.Windows.Forms.Timer tmr = (sender as System.Windows.Forms.Timer);
            (tmr.Tag as Control).Visible = false;
            tmr.Stop();
            tmr.Tick -= new EventHandler(tmr_Tick);
        }


        /// <summary>
        /// 检查是否需要清空指令
        /// </summary>
        /// <returns>需要清空返回true; 不需要清空返回false</returns>
        public static ClearResult CheckClearCommands()
        {
            if (PrintingObj.IsPrinting)
            {
                if (DialogResult.Yes == MessageBox.Show("当前正在打印，是否终止打印进行运动调校？", "中止打印提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2))
                {
                    StopImmediately();
                    return ClearResult.Cleared;
                }
                else
                {
                    return ClearResult.DonotClear;
                }
            }
            if (bPausePrint)
            {
                if (DialogResult.Yes == MessageBox.Show("缓冲中已有指令加载，是否清空所有指令进行运动调校？", "清空指令提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2))
                {
                    StopImmediately();
                    return ClearResult.Cleared;
                }
                else
                {
                    return ClearResult.DonotClear;
                }
            }
            GV.PrintingObj.ClearNozzleDXYZ_AB();
            return ClearResult.Needless;
        }


        //用于计算校验码
        public static byte[] GetCRC16ByPoly(byte[] Cmd, ushort Poly, bool IsHighBefore)
        {
            byte[] CRC = new byte[2];
            ushort CRCValue = 0xFFFF;
            byte[] crcbyte = new byte[Cmd.Length + 2];

            for (int i = 0; i < Cmd.Length; i++)
            {
                CRCValue = (ushort)(CRCValue ^ Cmd[i]);
                for (int j = 0; j < 8; j++)
                {
                    if ((CRCValue & 0x0001) != 0)
                    {
                        CRCValue = (ushort)((CRCValue >> 1) ^ Poly);
                    }
                    else
                    {
                        CRCValue = (ushort)(CRCValue >> 1);
                    }
                }
            }
            byte[] Check = BitConverter.GetBytes(CRCValue);
            if (IsHighBefore == true)
            {
                for (int j = 0; j < Cmd.Length; j++)
                    crcbyte[j] = Cmd[j];
                crcbyte[crcbyte.Length - 2] = Check[0];
                crcbyte[crcbyte.Length - 1] = Check[1];

                return crcbyte;
            }
            else
            {
                return Check;
            }
        }


        /// <summary>
        /// 查询当前温度
        /// </summary>
        /// <param name="seriPort">温度传感器连接的COM口</param>
        /// <returns>温度值（摄氏度）</returns>
        public static string QueryTemperature(SerialPort seriPort)
        {
            if (!seriPort.IsOpen)
            {
                return "ERR";
            }
            // 03 04 00 00 00 06 71 EA 
            byte[] sendBuffer = new byte[8];
            sendBuffer[0] = 0x03;
            sendBuffer[1] = 0x04;
            sendBuffer[2] = 0x00;
            sendBuffer[3] = 0x00;
            sendBuffer[4] = 0x00;
            sendBuffer[5] = 0x06;
            sendBuffer[6] = 0x71;
            sendBuffer[7] = 0xEA;
            seriPort.Write(sendBuffer, 0, sendBuffer.Length);

            Thread.Sleep(150);
            int countBytesToRead = seriPort.BytesToRead;
            int temp = 0;
            string strTemp = "";
            if (countBytesToRead >= 17)
            {
                byte[] readBuff = new byte[17];
                seriPort.Read(readBuff, 0, readBuff.Length);
                seriPort.ReadExisting();
                // 03 04 0C 00 C2 00 60 00 9C 00 00 00 00 00 01 D5 C6
                if (readBuff[0] == 0x03 && readBuff[1] == 0x04 && readBuff[2] == 0x0C)
                {
                    temp = readBuff[3] * 256 + readBuff[4];
                    temp = Convert.ToInt16(temp.ToString("X4"), 16);
                    strTemp = (temp * 0.1).ToString("0.0");
                    return strTemp;
                }
            }
            return "ERR";
        }


        #region 程序集特性访问器

        public static string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public static string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public static string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public static string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public static string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public static string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

    }
}

public enum AxesType  // 显示坐标轴类型枚举型变量
{
    XY_Z = 1,
    XZ_Y = 2,
    YZ_X = 3
}

public enum ConnectMode
{
    Disconnect = 0,
    ConnectController = 1,
    ConnectSimulator = 2,
    ConnectControllerWithLS = 3,
    ConnectPMController = 4,
    DisconnectPMController = 5
}

public enum VertexPos  // 顶点位置
{
    LeftBottom = 0,
    LeftTop = 1,
    RightBottom = 2,
    RightTop = 3
}

public enum ClearResult
{
    Needless = 0,
    Cleared = 1,
    DonotClear = 2
}


public class StageStatus
{
    public long time;
    public double fPosX;
    public double fPosY;
    public double fPosZ;
    public double fPosZ1;
    public double fPosZ2;
    public double fVelX;
    public double fVelY;
    public double fVelZ;
    public double fVelZ1;
    public double fVelZ2;
    public double gVel;
    public double fAccX;
    public double fAccY;
    public double fAccZ;
    public bool isEnabledX;
    public bool isEnabledY;
    public bool isEnabledZ;
    public bool isEnabledZ1;
    public bool isEnabledZ2;
    public bool isMovingX;
    public bool isMovingY;
    public bool isMovingZ;
    public bool isMovingZ1;
    public bool isMovingZ2;
    public bool isInPosX;
    public bool isInPosY;
    public bool isInPosZ;
    //Xbot
    public string PMCStatus;//PMC状态
    public double fXbotPosXa;
    public double fXbotPosYa;
    public double fXbotPosZa;
    public double fXbotPosXb;
    public double fXbotPosYb;
    public double fXbotPosZb;
    public double fXbotRxA;
    public double fXbotRyA;
    public double fXbotRzA;
    public double fXbotRxB;
    public double fXbotRyB;
    public double fXbotRzB;
    public bool isActiveA;
    public bool isActiveB;

    public double leftTime;
    public bool isExtruding;
    //新增喷头2显示
    public bool isExtruding2;
    public int nozzleID;
    public string layerCurrent;
    public string filamentNumber;
    public List<string> statusMsg;
    public int[] GSFREE = new int[3];
    public double[] GRTIME = new double[3];
    public int GSEG;
    public string gcodeCommands;//发送的gcode指令
    public int countG1;
    public int countGseg;
    public int previousMaxCountGseg;
}

public class LineInfo
{
    public int layer;
    public string gCode;

    public LineInfo(int layer, string gCode)
    {
        this.layer = layer;
        this.gCode = gCode;
    }
}

public static class CsvFileReader
{
    public static string[][] Read(string fullname)
    {
        if (!File.Exists(fullname)) return new string[][] { };
        var lines = File.ReadAllLines(fullname).Skip(1);//跳过标题行
        var list = new List<string[]>();
        var builder = new StringBuilder();
        foreach (var line in lines)
        {
            builder.Clear();
            var comma = false;
            var array = line.ToCharArray();
            var values = new List<string>();
            var length = array.Length;
            var index = 0;
            while (index < length)
            {
                var item = array[index++];
                switch (item)
                {
                    case ',':
                        if (comma)
                        {
                            builder.Append(item);
                        }
                        else
                        {
                            values.Add(builder.ToString());
                            builder.Clear();
                        }
                        break;
                    case '"':
                        comma = !comma;
                        break;
                    default:
                        builder.Append(item);
                        break;
                }
            }
            var count = values.Count;
            if (count == 0) continue;
            list.Add(values.ToArray());
        }
        return list.ToArray();
    }
}

