using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMCP
{
    /// <summary>
    /// 打印模型数据管理类（打印对象相关，包括STL、Gcode、CMD格式文件及其内存数据的管理）
    /// </summary>
    public class DataManagement
    {
        public CmdDataStruct[] CmdData;
        public int iCmdFront;
        public int iCmdTail;
        public int countCmdTotal = 0;           // 自开始打印以来载入的所有指令数量
        public int countCmdExecuted = 0;        // 自开始打印以来已执行的指令数量
        public double totalEstimateTime = 0;    // 估计打印需要的总时间（s） - 理论值（t_eA）
        public double segStartEstimateTime = 0; // 已打印部分需要的总时间（s） - 理论值（t_e0）
        public double segEndEstimateTime = 0;   // 已发送指令执行需要的总时间（s） - 理论值（t_e1）
        public double segStartRealTime = 0;     // 指令发送时的实际时间 - 实际值（t_r0）
        public double k_Estimate_Real = 1;      // 实际时间和估计时间的比值(k)
        public double totalRealTimeEst = 0;     // 打印实际时间估计值（s） - 实际值
        public double realUsedTime = 0;         // 实际已打印时间 - 实际值（t_r0）
        public double realEstimateLeftTime = 0; // 估计的实际剩余打印时间 - 实际值（t_r1)
        public Queue<CmdDataStruct> CmdQueue;
        public System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        public DataManagement()
        {
            //CmdData = new DataManagement.CmdDataStruct[51];
            CmdQueue = new Queue<CmdDataStruct>(500); // 定义一个CmdDataStruct队列

            //iCmdFront = 0;
            //iCmdTail = 0;
        }

        /// <summary>
        /// 读取STL文件，并保存数据到数组NormalSTL, VertexsSTL
        /// </summary>
        /// <param name="fileName">文件名（含路径）</param>
        public void ReadSTLFile(string fileName)
        {

        }

        /// <summary>
        /// 根据数组数组NormalSTL, VertexsSTL的数据，写入STL文件。
        /// </summary>
        /// <param name="fileName">文件名（含路径）</param>
        public void WriteSTLFile(string fileName)
        {

        }

        /// <summary>
        /// 根据数组GcodeData的数据，写入Gcode文件。
        /// </summary>
        /// <param name="fileName">文件名（含路径）</param>
        public void WriteGcodeFile(string fileName)
        {

        }

        /// <summary>
        /// 读取CMD文件，并保存数据到数组CmdData
        /// </summary>
        /// <param name="fileName">文件名（含路径）</param>
        public void ReadCmdFile(string fileName)
        {

        }

        /// <summary>
        /// 根据数组CmdData的数据，写入文件。
        /// </summary>
        /// <param name="fileName">文件名（含路径）</param>
        public void WriteCmdFile(string fileName)
        {

        }
        public long PrintingElapsedMilliseconds()
        {
            return stopwatch.ElapsedMilliseconds;
        }
        public void ResetCmdCounter()
        {
            countCmdTotal = GV.dataManagementObj.CmdQueue.Count;
            countCmdExecuted = 0;
            totalEstimateTime = 0;
            segEndEstimateTime = 0;
            realEstimateLeftTime = 0;
            stopwatch.Restart();
        }

        /// <summary>
        /// 从队首读取指令
        /// </summary>
        /// <returns></returns>
        public CmdDataStruct GetCmdData()
        {
            CmdDataStruct cds = CmdQueue.Dequeue();
            // 读取的指令马上发送给控制器，执行马上开始：
            countCmdExecuted++;
            segStartEstimateTime = segEndEstimateTime;
            segEndEstimateTime += cds.EstimateTime;
            //if (countCmdTotal>0)
            //{
            //    GV.bgWorker2.ReportProgress(100 * countCmdExecuted / countCmdTotal);
            //}
            return cds;
        }

        /// <summary>
        /// 获取当前队列第一个指令但不移除
        /// </summary>
        /// <returns></returns>
        public CmdDataStruct ReadCmdData()
        {
            CmdDataStruct cds;
            if(CmdQueue.Count > 0)
            {
                cds = CmdQueue.Peek();
            }
            else
            {
                cds = new DataManagement.CmdDataStruct();
            }
            return cds;
        }

        /// <summary>
        /// 从队尾插入新指令
        /// </summary>
        /// <param name="cds"></param>
        public void InsertCmdData(CmdDataStruct cds)
        {
            countCmdTotal++;
            totalEstimateTime += cds.EstimateTime;
            CmdQueue.Enqueue(cds);
        }

        /// <summary>
        /// 从队尾插入新指令
        /// </summary>
        /// <param name="cds"></param>
        public void InsertCmdDataWithEstimateTime(CmdDataStruct cds, double EstimatedTime)
        {
            cds.EstimateTime = EstimatedTime;
            InsertCmdData(cds);
        }

        public int GetCmdCount()
        {
            if (iCmdFront <= iCmdTail)
                return iCmdTail - iCmdFront;
            else
                return iCmdTail + CmdData.Length - iCmdFront;
        }

        /// <summary>
        /// 将当前数组GcodeData的数据转换为CMD格式，并保存到数组GcodeData
        /// </summary>
        public void Gcode2Cmd()
        {

        }

        private static int _totalValidCommands = 0;
        private static readonly Dictionary<OptType, int> _commandStatistics = new Dictionary<OptType, int>();
        //显示cmdstruct
        public string PeekNextCmdDataAsString(int offset = 0)
        {
            if (CmdQueue.Count == 0 || offset >= CmdQueue.Count)
            {
                return "未发送指令";
            }

            var cmd = CmdQueue.ElementAt(offset);
            var sb = new StringBuilder();

            // 更新统计信息
            // 判断是否为有效指令
            bool isValidCommand = !string.IsNullOrEmpty(cmd.Para1) && (cmd.CmdName == OptType.SegmentLine
                ||cmd.CmdName == OptType.SegmentedMotion || cmd.CmdName == OptType.MoveAxisTo);
           
            if (isValidCommand)
            {
                _totalValidCommands += 1;

                if (_commandStatistics.ContainsKey(cmd.CmdName))
                {
                    _commandStatistics[cmd.CmdName]++;
                }
                else
                {
                    _commandStatistics.Add(cmd.CmdName, 1);
                }
            }

            sb.AppendLine($"指令类型: {cmd.CmdName}");
            //sb.AppendLine($"预计时间: {cmd.EstimateTime}秒");

            // 添加非空参数
            string[] parameters = { cmd.Para1, cmd.Para2, cmd.Para3, cmd.Para4, cmd.Para5,
                          cmd.Para6, cmd.Para7, cmd.Para8, cmd.Para9, cmd.Para10 };

            for (int i = 0; i < parameters.Length; i++)
            {
                if (!string.IsNullOrEmpty(parameters[i]))
                {
                    sb.AppendLine($"参数{i + 1}: {parameters[i]}");
                }
            }

            // 添加统计信息
            sb.AppendLine("\n--- 指令计数 ---");
            sb.AppendLine($"有效指令数: {_totalValidCommands}");
            //sb.AppendLine("各类指令数量:");
            //foreach (var stat in _commandStatistics.OrderByDescending(x => x.Value))
            //{
            //    sb.AppendLine($"  {stat.Key}: {stat.Value}");
            //}

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public struct CmdDataStruct
        {
            public OptType CmdName;
            public string Para1;
            public string Para2;
            public string Para3;
            public string Para4;
            public string Para5;
            public string Para6;
            public string Para7;
            public string Para8;
            public string Para9;
            public string Para10;
            public double EstimateTime;
        };
        //打印工作状态类型
        public enum OptType
        {
            NoOpt = 0,
            Home = 1,
            Jog = 2,
            EnableMotor = 3,
            DisableMotor = 4,
            MoveXYZTo = 5,
            MoveXYTo = 6,
            MoveAxisTo = 7,
            MoveAxisRelative = 8,
            Extrude = 9,
            AdjustNeedle = 10,
            Pause = 11,
            WaitMoveEnd = 12,
            SegmentedMotion = 13,
            EndSequenceM = 14,
            SegmentLine = 15,
            SegmentArc1 = 16,
            SegmentArc2 = 17,
            SegmentStopper = 18,
            DisplayInfo = 19,
            EndPrinting = 20,
            HighVoltage = 21,
            SetHighVoltageValue = 22,
            ExtSegmentedMotion = 23,
            TakePhoto = 24,
            SwitchNozzle = 25,
            CleanNozzle = 26,

            GetLaserDistance = 27,
            StartDectecting = 28,
            EndDectecting = 29,
            SetStatus = 30,
            RecordDisplacement = 31,//测量距离
            SetPressure = 32,
            CheckPenStatus = 34,
            SetRotarySpeed = 35,
            SetPrintPosStartEnd = 36,
            BasalTransPort = 37, //基底转运模块
            AdjustMicroMotor = 38, //微动平台调节
            AdjustPMmotor = 39     //平面电机调节
        }



        private string[] GcodeData;

        public double[, ,] VertexsSTL;

        public double[,] NormalSTL;

    }
}
