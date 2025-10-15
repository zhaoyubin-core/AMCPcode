using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AMCP
{
    /// <summary>
    /// 打印参数设置（打印机相关）
    /// </summary>
    public class ParaSetting
    {
        public MachineParaStruct machinePara;
        public HomeParaStruct homePara;
        public JogParaStruct jogPara;
        public SpeedParaStruct speedPara;
        public PrintParaStruct printPara;
        public AdjustNeedleStruct adjustNeedle;


        /// <summary>
        /// 读取INI文件，并保存数据到各参数结构体。
        /// </summary>
        /// <param name="fileName">文件名（含路径）</param>
        public void ReadINIFile(string fileName)
        {

        }

        /// <summary>
        /// 根据各参数结构体的数据，写入INI文件。
        /// </summary>
        /// <param name="fileName">文件名（含路径）</param>
        public void WriteINIFile(string fileName)
        {

        }

        public struct MachineParaStruct // 设备极限参数
        {
            // 打印工作区域
            public double xMax;
            public double xMin;
            public double yMin;
            public double yMax;
            public double zMin;
            public double zMax;

            // 最大速度限制
            public double vMax;
            // 最大加速度限制
            public double aMax;
            // 最大加加速度限制
            public double jMax;
        }

        public struct HomeParaStruct  // 回零参数
        {
            public int xDirection;
            public int yDirection;
            public int zDirection;

            public double xOffset;
            public double yOffset;
            public double zOffset;

            public double xSpeed;
            public double ySpeed;
            public double zSpeed;
        }

        public struct JogParaStruct  // 点动参数
        {
            public bool jogMode; // 是否为点动模式
            public double lowSpeed;
            public double midSpeed;
            public double hiSpeed;

            public double stepDistance;
            public int speedMode;
        }

        public struct SpeedParaStruct
        {
            public double xSpeed;
            public double ySpeed;
            public double zSpeed;

            public double xAcc;
            public double yAcc;
            public double zAcc;

            public double xJerk;
            public double yJerk;
            public double zJerk;
        }

        public struct PrintParaStruct
        {
            public double speed;
            public double acc;
            public double thickness;
        }

        public struct AdjustNeedleStruct
        {
            public double xAdjust;
            public double yAdjust;
            public double zAdjust;
            public int bufferID;
        }

    }
}
