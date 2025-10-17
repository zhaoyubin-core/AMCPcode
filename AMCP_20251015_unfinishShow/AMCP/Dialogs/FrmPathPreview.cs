using OpenCvSharp;
using PMCLIB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace AMCP
{
    public partial class FrmPathPreview : Form
    {
        public int lastX = 0; // 最后一次鼠标事件的e.X值
        public int lastY = 0; // 最后一次鼠标事件的e.Y值
        public double kx;
        public double ky;

        public static AxesType axesType = AxesType.XY_Z;//默认显示XY平面
        public static Series seriXY, seriXY1, seriZ0, seriZ01;//显示路径的序列0：A工位，1：显示框架        
        public static Series seriXZ, seriXZ1, seriY0, seriY01;
        public static Series seriYZ, seriX0, seriYZ1, seriX01;
        public static Series seriXY2, seriZ02;//B工位序列点
        public static Series seriXZ2, seriY2;//XZ平面
        public static Series seriYZ2, seriX2;//YZ平面

        public static Series seriHide;
        bool followCurPosition = false;
        public int refreshInterval = 50;
        public int keepPoints = 10000;

        public FrmPathPreview()
        {
            InitializeComponent();
            InitAllSeries();
            this.positionChart.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.positionChart_MouseWheel);
        }

        /// <summary>
        /// 初始化各坐标系的点序列
        /// </summary>
        public static void InitAllSeries()
        {
            GV.InitSeries(ref seriXY, 0, 1);//初始化序列
            GV.InitSeries(ref seriXY2, 3, 1);//初始化B工位序列
            //GV.InitSeries(ref seriXY1, 1, 0);
            //seriXY1.Points.AddXY(0, 0);

            //GV.InitSeries(ref seriZ0, 0);
            //GV.InitSeries(ref seriZ01, 1);
            //seriZ01.Points.AddXY(5, 0);

            GV.InitSeries(ref seriXZ, 0);
            GV.InitSeries(ref seriXZ2, 3);

            //GV.InitSeries(ref seriXZ1, 1);
            //seriXZ1.Points.AddXY(0, 0);

            //GV.InitSeries(ref seriY0, 0);
            //GV.InitSeries(ref seriY01, 1);
            //seriY01.Points.AddXY(5, 0);

            GV.InitSeries(ref seriYZ, 0);
            GV.InitSeries(ref seriYZ2, 3);
            //GV.InitSeries(ref seriYZ1, 1);
            //seriYZ1.Points.AddXY(0, 0);

            //GV.InitSeries(ref seriX0, 0);
            //GV.InitSeries(ref seriX01, 1);
            //GV.seriX01.Points.AddXY(5, 0);

            //初始化平台
            GV.InitSeries(ref seriXY1, 2, 10);
            GV.InitSeries(ref seriXZ1, 2, 10);
            GV.InitSeries(ref seriYZ1, 2, 10);

            GV.InitSeries(ref seriHide, 2, 10);

            LoadDoubleStageFrame();
        }

        /// <summary>
        /// 加载障碍物区域序列数据CYQ 20240504
        /// 加载双工位区域序列数据202510
        /// </summary>
        private static void LoadDoubleStageFrame()
        {
            //REAL xD = 392, zD_up = 282.5, zD_down = 263;
            //REAL yD1 = 105, yD2 = 305, yD3 = 505, yD4 = 705;

            double xN = 2;              // 左导柱中心X坐标
            double xD = 392;            // 右导柱位移笔碰撞中心X坐标
            double xOffset = 50;        // 位移笔相对针头的X偏移量
            double yOffset = 15;        // 位移笔相对针头的Y偏移量
            double zN = 269;            // 导柱最高点碰撞针头Z坐标
            double zD = 282.5;          // 导柱最高点碰撞位移笔Z坐标
            double zB = 276;            // 压条上表面碰撞针头Z坐标
            double rB = 7.5;            // 压条宽度的一半
            double rN = 4;              // 导柱与针头碰撞危险半径
            double rD = 8;              // 导柱与位移笔碰撞危险半径

            double xL = xN - 10;        // 压条左边界
            double xN2 = xD + xOffset;  // 右导柱中心X坐标
            double xR = xN2 + 10;       // 压条右边界
            double yN1 = 121, yN2 = 321, yN3 = 521, yN4 = 721; // 4组导柱中心的Y坐标
            double yD1 = yN1 - yOffset, yD2 = yN2 - yOffset, yD3 = yN3 - yOffset, yD4 = yN4 - yOffset; // 4组导柱位移笔碰撞中心的Y坐标

            // 定子左上角，定子尺寸，定子距离
            //定子240*240*70，动子M3-13240*240*10
            double stageXa = 120, stageXb = 460, stageY = 120;//定子的中心位置
            double rStage = 120;//定子半径;
            double rXBot = 120;//动子半径
            double rPrint = 85;//标准打印文件半径170*170
            double[] xx;
            double[] yy;
            int[] ww;

            //
            // XY平面：===================================================================================
            //
            // 打印区域在XY平面的投影
            xx = new double[] { stageXa - rPrint, stageXa + rPrint, stageXa + rPrint, stageXa - rPrint, stageXa - rPrint,
                                stageXb - rPrint, stageXb + rPrint, stageXb + rPrint, stageXb - rPrint, stageXb - rPrint, };
            yy = new double[] { stageY - rPrint, stageY - rPrint, stageY + rPrint, stageY + rPrint, stageY - rPrint,
                                stageY - rPrint, stageY - rPrint, stageY + rPrint, stageY + rPrint, stageY - rPrint, };
            ww = new int[] { 0, 4, 4, 4, 4, 0, 4, 4, 4, 4 };
            //ww = new int[] { 0, 4, 4, 4, 4 };//边框宽度
            for (int i = 0; i < yy.Length; i++)
            {
                DataPoint dp = new DataPoint(xx[i], yy[i]);
                dp.BorderWidth = ww[i];
                dp.Color = Color.Pink;
                seriXY1.Points.Add(dp);
            }

            //// 压条在XY平面的投影
            //xx = new double[] { xL, xR, xR, xL, xL, xL, xR, xR, xL, xL, xL, xR, xR, xL, xL, xL, xR, xR, xL, xL };
            //yy = new double[] { yN1 - rB, yN1 - rB, yN1 + rB, yN1 + rB, yN1 - rB, yN2 - rB, yN2 - rB, yN2 + rB, yN2 + rB, yN2 - rB, yN3 - rB, yN3 - rB, yN3 + rB, yN3 + rB, yN3 - rB, yN4 - rB, yN4 - rB, yN4 + rB, yN4 + rB, yN4 - rB };
            //ww = new int[] { 0, 2, 2, 2, 2, 0, 2, 2, 2, 2, 0, 2, 2, 2, 2, 0, 2, 2, 2, 2 };
            //for (int i = 0; i < yy.Length; i++)
            //{
            //    DataPoint dp = new DataPoint(xx[i], yy[i]);
            //    dp.BorderWidth = ww[i];
            //    seriXY1.Points.Add(dp);
            //}

            //// 左导柱在XY平面的投影
            //xx = new double[] { xN - rN, xN + rN, xN + rN, xN - rN, xN - rN, xN - rN, xN + rN, xN + rN, xN - rN, xN - rN, xN - rN, xN + rN, xN + rN, xN - rN, xN - rN, xN - rN, xN + rN, xN + rN, xN - rN, xN - rN };
            //yy = new double[] { yN1 - rN, yN1 - rN, yN1 + rN, yN1 + rN, yN1 - rN, yN2 - rN, yN2 - rN, yN2 + rN, yN2 + rN, yN2 - rN, yN3 - rN, yN3 - rN, yN3 + rN, yN3 + rN, yN3 - rN, yN4 - rN, yN4 - rN, yN4 + rN, yN4 + rN, yN4 - rN };
            //ww = new int[] { 0, 2, 2, 2, 2, 0, 2, 2, 2, 2, 0, 2, 2, 2, 2, 0, 2, 2, 2, 2 };
            //for (int i = 0; i < yy.Length; i++)
            //{
            //    DataPoint dp = new DataPoint(xx[i], yy[i]);
            //    dp.BorderWidth = ww[i];
            //    seriXY1.Points.Add(dp);
            //}

            //// 右导柱在XY平面的投影
            //xx = new double[] { xN2 - rN, xN2 + rN, xN2 + rN, xN2 - rN, xN2 - rN, xN2 - rN, xN2 + rN, xN2 + rN, xN2 - rN, xN2 - rN, xN2 - rN, xN2 + rN, xN2 + rN, xN2 - rN, xN2 - rN, xN2 - rN, xN2 + rN, xN2 + rN, xN2 - rN, xN2 - rN };
            //yy = new double[] { yN1 - rN, yN1 - rN, yN1 + rN, yN1 + rN, yN1 - rN, yN2 - rN, yN2 - rN, yN2 + rN, yN2 + rN, yN2 - rN, yN3 - rN, yN3 - rN, yN3 + rN, yN3 + rN, yN3 - rN, yN4 - rN, yN4 - rN, yN4 + rN, yN4 + rN, yN4 - rN };
            //ww = new int[] { 0, 2, 2, 2, 2, 0, 2, 2, 2, 2, 0, 2, 2, 2, 2, 0, 2, 2, 2, 2 };
            //for (int i = 0; i < yy.Length; i++)
            //{
            //    DataPoint dp = new DataPoint(xx[i], yy[i]);
            //    dp.BorderWidth = ww[i];
            //    seriXY1.Points.Add(dp);
            //}

            //// 右导柱对应的位移笔危险区在XY平面的投影
            //xx = new double[] { xD - rD, xD + rD, xD + rD, xD - rD, xD - rD, xD - rD, xD + rD, xD + rD, xD - rD, xD - rD, xD - rD, xD + rD, xD + rD, xD - rD, xD - rD, xD - rD, xD + rD, xD + rD, xD - rD, xD - rD };
            //yy = new double[] { yD1 - rD, yD1 - rD, yD1 + rD, yD1 + rD, yD1 - rD, yD2 - rD, yD2 - rD, yD2 + rD, yD2 + rD, yD2 - rD, yD3 - rD, yD3 - rD, yD3 + rD, yD3 + rD, yD3 - rD, yD4 - rD, yD4 - rD, yD4 + rD, yD4 + rD, yD4 - rD };
            //ww = new int[] { 0, 2, 2, 2, 2, 0, 2, 2, 2, 2, 0, 2, 2, 2, 2, 0, 2, 2, 2, 2 };
            //for (int i = 0; i < yy.Length; i++)
            //{
            //    DataPoint dp = new DataPoint(xx[i], yy[i]);
            //    dp.BorderWidth = ww[i];
            //    dp.Color = Color.Gray;
            //    seriXY1.Points.Add(dp);
            //}

            //新增，平面定子在XY平面的投影
            xx = new double[] { stageXa - rStage, stageXa + rStage, stageXa + rStage, stageXa - rStage, stageXa - rStage,
                                stageXb - rStage, stageXb + rStage, stageXb + rStage, stageXb - rStage, stageXb - rStage, };
            yy = new double[] { stageY - rStage, stageY - rStage, stageY + rStage, stageY + rStage, stageY - rStage,
                                stageY - rStage, stageY - rStage, stageY + rStage, stageY + rStage, stageY - rStage, };
            ww = new int[] { 0, 4, 4, 4, 4, 0, 4, 4, 4, 4 };
            for (int i = 0; i < yy.Length; i++)
            {
                DataPoint dp = new DataPoint(xx[i], yy[i]);
                dp.BorderWidth = ww[i];
                dp.Color = Color.Black;
                seriXY1.Points.Add(dp);
            }
            ////新增，动子在xy平面的投影
            //xx = new double[] { stageA - rXBot, stageA + rXBot, stageA + rXBot, stageA - rXBot, stageA - rXBot };
            //yy = new double[] { stageA - rXBot, stageA - rXBot, stageA + rXBot, stageA + rXBot, stageA - rXBot };
            //ww = new int[] { 0, 4, 4, 4, 4 };
            //for (int i = 0; i < yy.Length; i++)
            //{
            //    DataPoint dp = new DataPoint(xx[i], yy[i]);
            //    dp.BorderWidth = ww[i];
            //    dp.Color = Color.DarkGreen;
            //    seriXY1.Points.Add(dp);
            //}
            // XZ平面：===================================================================================
            //
            // 打印区域在XZ平面的投影
            xx = new double[] { 0, GV.X_MAX, GV.X_MAX, 0, 0 };
            yy = new double[] { 0, 0, GV.Z_MAX, GV.Z_MAX, 0 };
            ww = new int[] { 0, 2, 2, 2, 2 };
            for (int i = 0; i < yy.Length; i++)
            {
                DataPoint dp = new DataPoint(xx[i], yy[i]);
                dp.BorderWidth = ww[i];
                dp.Color = Color.Pink;
                seriXZ1.Points.Add(dp);
            }

            // 压条在XZ平面的投影
            xx = new double[] { xL, xR, xR, xL, xL };
            yy = new double[] { 300, 300, zB, zB, 300 };
            ww = new int[] { 0, 2, 2, 2, 2 };
            for (int i = 0; i < yy.Length; i++)
            {
                DataPoint dp = new DataPoint(xx[i], yy[i]);
                dp.BorderWidth = ww[i];
                seriXZ1.Points.Add(dp);
            }

            // 左导柱在XZ平面的投影
            xx = new double[] { xN - rN, xN + rN, xN + rN, xN - rN, xN - rN };
            yy = new double[] { 300, 300, zN, zN, 300 };
            ww = new int[] { 0, 2, 2, 2, 2 };
            for (int i = 0; i < yy.Length; i++)
            {
                DataPoint dp = new DataPoint(xx[i], yy[i]);
                dp.BorderWidth = ww[i];
                seriXZ1.Points.Add(dp);
            }

            // 右导柱在XZ平面的投影
            xx = new double[] { xN2 - rN, xN2 + rN, xN2 + rN, xN2 - rN, xN2 - rN };
            yy = new double[] { 300, 300, zN, zN, 300 };
            ww = new int[] { 0, 2, 2, 2, 2 };
            for (int i = 0; i < yy.Length; i++)
            {
                DataPoint dp = new DataPoint(xx[i], yy[i]);
                dp.BorderWidth = ww[i];
                seriXZ1.Points.Add(dp);
            }

            // 右导柱对应的位移笔危险区在XZ平面的投影
            xx = new double[] { xD - rD, xD + rD, xD + rD, xD - rD, xD - rD };
            yy = new double[] { 300, 300, zD, zD, 300 };
            ww = new int[] { 0, 2, 2, 2, 2 };
            for (int i = 0; i < yy.Length; i++)
            {
                DataPoint dp = new DataPoint(xx[i], yy[i]);
                dp.BorderWidth = ww[i];
                dp.Color = Color.Gray;
                seriXZ1.Points.Add(dp);
            }

            //// 障碍物在XZ平面的投影
            //xx = new double[] { xD - rD, xD + rD, xD + rD, xD - rD, xD - rD, xN - rN, xN + rN, xN + rN, xN - rN, xN - rN, xL, xR, xR, xL, xL };
            //yy = new double[] { 300, 300, zD, zD, 300, 300, 300, zN, zN, 300, 300, 300, zB, zB, 300 };
            //ww = new int[] { 0, 2, 2, 2, 2, 0, 2, 2, 2, 2, 0, 2, 2, 2, 2 };
            //for (int i = 0; i < yy.Length; i++)
            //{
            //    DataPoint dp = new DataPoint(xx[i], yy[i]);
            //    dp.BorderWidth = ww[i];
            //    seriXZ1.Points.Add(dp);
            //}

            //
            // YZ平面：===================================================================================
            //
            // 打印区域在YZ平面的投影
            xx = new double[] { 0, GV.Y_MAX, GV.Y_MAX, 0, 0 };
            yy = new double[] { 0, 0, GV.Z_MAX, GV.Z_MAX, 0 };
            ww = new int[] { 0, 2, 2, 2, 2 };
            for (int i = 0; i < yy.Length; i++)
            {
                DataPoint dp = new DataPoint(xx[i], yy[i]);
                dp.BorderWidth = ww[i];
                dp.Color = Color.Pink;
                seriYZ1.Points.Add(dp);
            }

            // 障碍物在YZ平面的投影
            xx = new double[] { yD1 - rD, yD1 + rD, yD1 + rD, yD1 - rD, yD1 - rD, yD2 - rD, yD2 + rD, yD2 + rD, yD2 - rD, yD2 - rD, yD3 - rD, yD3 + rD, yD3 + rD, yD3 - rD, yD3 - rD, yD4 - rD, yD4 + rD, yD4 + rD, yD4 - rD, yD4 - rD, yN1 - rN, yN1 + rN, yN1 + rN, yN1 - rN, yN1 - rN, yN2 - rN, yN2 + rN, yN2 + rN, yN2 - rN, yN2 - rN, yN3 - rN, yN3 + rN, yN3 + rN, yN3 - rN, yN3 - rN, yN4 - rN, yN4 + rN, yN4 + rN, yN4 - rN, yN4 - rN };
            yy = new double[] { 300, 300, zD, zD, 300, 300, 300, zD, zD, 300, 300, 300, zD, zD, 300, 300, 300, zD, zD, 300, 300, 300, zN, zN, 300, 300, 300, zN, zN, 300, 300, 300, zN, zN, 300, 300, 300, zN, zN, 300 };
            ww = new int[] { 0, 2, 2, 2, 2, 0, 2, 2, 2, 2, 0, 2, 2, 2, 2, 0, 2, 2, 2, 2, 0, 2, 2, 2, 2, 0, 2, 2, 2, 2, 0, 2, 2, 2, 2, 0, 2, 2, 2, 2 };
            for (int i = 0; i < yy.Length; i++)
            {
                DataPoint dp = new DataPoint(xx[i], yy[i]);
                dp.BorderWidth = ww[i];
                seriYZ1.Points.Add(dp);
            }

        }
        private void positionChart_MouseWheel(object sender, MouseEventArgs e)
        {
            Chart chart = sender as Chart;
            double LenAxisX = chart.ChartAreas[0].AxisX.Maximum - chart.ChartAreas[0].AxisX.Minimum;
            double kZoom;  // 放缩倍数
            if (e.Delta > 0) // 向上滚动鼠标滚轮
            {
                if (LenAxisX < 0.1) return;
                kZoom = 0.8;
            }
            else
            {
                if (LenAxisX > 1000) return;
                kZoom = 1.25;
            }
            double x0 = GetXFromChart(e.X, sender);
            double y0 = GetYFromChart(e.Y, sender);
            ZoomChart(sender as Chart, x0, y0, kZoom);
        }

        private double GetXFromChart(int eX, object chart)
        {
            Chart theChart = chart as Chart;
            double CW = theChart.Width;
            double PX = CW * theChart.ChartAreas[0].Position.X * 0.01;                  // ChartArea左边界占据整个Chart宽度的百分比
            double PW = CW * theChart.ChartAreas[0].Position.Width * 0.01;              // ChartArea宽度占据整个Chart宽度的百分比
            double IPX = PW * theChart.ChartAreas[0].InnerPlotPosition.X * 0.01;        // 绘图区域左边界占据整个ChartArea宽度的百分比
            double IPW = PW * theChart.ChartAreas[0].InnerPlotPosition.Width * 0.01;    // 绘图区域宽度占据整个ChartArea宽度的百分比
            double xmin = theChart.ChartAreas[0].AxisX.Minimum;
            double xmax = theChart.ChartAreas[0].AxisX.Maximum;
            double x = (eX - PX - IPX) * (xmax - xmin) / IPW + xmin;
            return x;
        }

        private double GetYFromChart(int eY, object chart)
        {
            Chart theChart = chart as Chart;
            double CH = theChart.Height;
            double PY = CH * theChart.ChartAreas[0].Position.Y * 0.01;                  // ChartAreas上边界占据整个Chart高度的百分比
            double PH = CH * theChart.ChartAreas[0].Position.Height * 0.01;             // ChartArea高度占据整个Chart高度的百分比
            double IPY = PH * theChart.ChartAreas[0].InnerPlotPosition.Y * 0.01;        // 绘图区域上边界占据整个ChartArea高度的百分比
            double IPH = PH * theChart.ChartAreas[0].InnerPlotPosition.Height * 0.01;   // 绘图区域高度占据整个ChartArea高度的百分比
            double ymin = theChart.ChartAreas[0].AxisY.Minimum;
            double ymax = theChart.ChartAreas[0].AxisY.Maximum;
            double y = (eY - PY - IPY) * (ymax - ymin) / IPH + ymin;
            return y;
        }

        private void ZoomChart(Chart chart, double x0, double y0, double kZoom)
        {
            double xmin = chart.ChartAreas[0].AxisX.Minimum;
            double xmax = chart.ChartAreas[0].AxisX.Maximum;

            double xmin2 = x0 - kZoom * (x0 - xmin);
            double xmax2 = x0 + kZoom * (xmax - x0);

            double ymin = chart.ChartAreas[0].AxisY.Minimum;
            double ymax = chart.ChartAreas[0].AxisY.Maximum;

            double ymin2 = y0 - kZoom * (y0 - ymin);
            double ymax2 = y0 + kZoom * (ymax - y0);

            chart.ChartAreas[0].AxisX.Minimum = xmin2;
            chart.ChartAreas[0].AxisX.Maximum = xmax2;
            chart.ChartAreas[0].AxisY.Minimum = ymin2;
            chart.ChartAreas[0].AxisY.Maximum = ymax2;
        }

        /// <summary>
        /// 重置当前坐标系
        /// </summary>
        public void ResetCharts()
        {
            switch (axesType)
            {
                case AxesType.XY_Z:
                    positionChart.ChartAreas[0].AxisX.Title = "X";
                    positionChart.ChartAreas[0].AxisY.Title = "Y";

                    positionChart.ChartAreas[0].AxisX.Maximum = GV.X_MAX;
                    positionChart.ChartAreas[0].AxisX.Minimum = 0;

                    positionChart.ChartAreas[0].AxisY.Maximum = GV.Y_MAX;
                    positionChart.ChartAreas[0].AxisY.Minimum = 0;


                    positionChart.Series[0] = seriXY;
                    //positionChart.Series[1] = seriXY2;
                    ShowDoubleFrame();
                    ShowPos2();

                    //if (showDoubleFrame)
                    //{
                    //    positionChart.Series[1] = seriXY1;
                    //}
                    //else
                    //{
                    //    positionChart.Series[1] = seriHide;
                    //}
                    break;

                case AxesType.XZ_Y:
                    positionChart.ChartAreas[0].AxisX.Title = "X";
                    positionChart.ChartAreas[0].AxisY.Title = "Z";

                    positionChart.ChartAreas[0].AxisX.Maximum = GV.X_MAX;
                    positionChart.ChartAreas[0].AxisX.Minimum = 0;

                    positionChart.ChartAreas[0].AxisY.Maximum = GV.Z_MAX;
                    positionChart.ChartAreas[0].AxisY.Minimum = 0;

                    positionChart.Series[0] = seriXZ;
                    //positionChart.Series[1] = seriXZ1;
                    //positionChart.Series[2] = seriXZ2;
                    ShowDoubleFrame();
                    ShowPos2();
                    break;

                case AxesType.YZ_X:
                    positionChart.ChartAreas[0].AxisX.Title = "Y";
                    positionChart.ChartAreas[0].AxisY.Title = "Z";

                    positionChart.ChartAreas[0].AxisX.Maximum = GV.Y_MAX;
                    positionChart.ChartAreas[0].AxisX.Minimum = 0;

                    positionChart.ChartAreas[0].AxisY.Maximum = GV.Z_MAX;
                    positionChart.ChartAreas[0].AxisY.Minimum = 0;

                    positionChart.Series[0] = seriYZ;
     
                    ShowDoubleFrame();
                    ShowPos2();
                    break;
            }
        }
      
        private void MoveChartAxisX(double dX, object chart)
        {
            Chart theChart = chart as Chart;
            theChart.ChartAreas[0].AxisX.Maximum -= dX;
            theChart.ChartAreas[0].AxisX.Minimum -= dX;
        }

        private void MoveChartAxisY(double dY, object chart)
        {
            Chart theChart = chart as Chart;
            theChart.ChartAreas[0].AxisY.Maximum -= dY;
            theChart.ChartAreas[0].AxisY.Minimum -= dY;
        }

        private void ZoomWithMousewheel(Chart chart, int eX, int eY, int eDelta)
        {
            //Chart chart = sender as Chart;
            double lx, ly; // 点击点的x、y坐标分别距离坐标轴的位置比例
            double LenAxisX = chart.ChartAreas[0].AxisX.Maximum - chart.ChartAreas[0].AxisX.Minimum;

            double kZoom;  // 放缩倍数
            if (eDelta > 0) // 向上滚动鼠标滚轮
            {
                if (LenAxisX < 0.1) return;
                kZoom = 0.8;
            }
            else
            {
                if (LenAxisX > 1000) return;
                kZoom = 1.25;
            }
            double top = positionChart.Height * positionChart.ChartAreas[0].Position.Y * 0.01; // 10
            double bottom = top + positionChart.Height * (chart.ChartAreas[0].Position.Height) / 100 * chart.ChartAreas[0].InnerPlotPosition.Bottom / 100; //300;
            double left = positionChart.Width * (1 - positionChart.ChartAreas[0].Position.X * 0.01 - positionChart.ChartAreas[0].Position.Width * 0.01 * chart.ChartAreas[0].InnerPlotPosition.Width * 0.01); //48;
            double right = positionChart.Width * (1 - positionChart.ChartAreas[0].Position.X * 0.01);//340;

            lx = (eX - left) / (right - left);

            chart.ChartAreas[0].AxisX.Minimum += lx * LenAxisX * (1 - kZoom);
            chart.ChartAreas[0].AxisX.Maximum -= (1 - lx) * LenAxisX * (1 - kZoom);

            double LenAxisY = chart.ChartAreas[0].AxisY.Maximum - chart.ChartAreas[0].AxisY.Minimum;
            ly = (bottom - eY) / (bottom - top);

            chart.ChartAreas[0].AxisY.Minimum += ly * LenAxisY * (1 - kZoom);
            chart.ChartAreas[0].AxisY.Maximum -= (1 - ly) * LenAxisY * (1 - kZoom);
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            Chart chart = positionChart;
            //ZoomWithMousewheel(chart, chart.Width / 2 + 18, chart.Height / 2 - 20, 60);
            double x0 = GV.PrintingObj.Status.fPosX;
            double y0 = GV.PrintingObj.Status.fPosY;
            ZoomChartMid(chart, 0.8);
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            Chart chart = positionChart;
            //ZoomWithMousewheel(chart, chart.Width / 2 + 18, chart.Height / 2 - 20, -60);
            double x0 = GV.PrintingObj.Status.fPosX;
            double y0 = GV.PrintingObj.Status.fPosY;
            ZoomChartMid(chart, 1.25);
        }

        private void ZoomChartMid(Chart chart, double kZoom)
        {
            double x0;
            double y0;
            double xmin = chart.ChartAreas[0].AxisX.Minimum;
            double xmax = chart.ChartAreas[0].AxisX.Maximum;
            x0 = (xmax + xmin) * 0.5;

            double xmin2 = x0 - kZoom * (x0 - xmin);
            double xmax2 = x0 + kZoom * (xmax - x0);

            double ymin = chart.ChartAreas[0].AxisY.Minimum;
            double ymax = chart.ChartAreas[0].AxisY.Maximum;
            y0 = (ymax + ymin) * 0.5;
            double ymin2 = y0 - kZoom * (y0 - ymin);
            double ymax2 = y0 + kZoom * (ymax - y0);

            chart.ChartAreas[0].AxisX.Minimum = xmin2;
            chart.ChartAreas[0].AxisX.Maximum = xmax2;
            chart.ChartAreas[0].AxisY.Minimum = ymin2;
            chart.ChartAreas[0].AxisY.Maximum = ymax2;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetCharts();
        }

        private void btnClearChart_Click(object sender, EventArgs e)
        {
            ClearChart();
        }

        private void ClearChart()
        {
            positionChart.Series[0].Points.Clear();
            positionChart.Series[2].Points.Clear();

            seriXY.Points.Clear();//清空序列点
            seriXZ.Points.Clear();
            seriYZ.Points.Clear();

            seriXY2.Points.Clear();
            seriXZ2.Points.Clear();
            seriYZ2.Points.Clear();

            //seriX0.Points.Clear();
            //seriY0.Points.Clear();
            //seriZ0.Points.Clear();
        }

        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            axesType = AxesType.XY_Z;
            ToolStripMenuItem1.Checked = true;
            ToolStripMenuItem2.Checked = false;
            ToolStripMenuItem3.Checked = false;
            ResetCharts();
        }

        private void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            axesType = AxesType.XZ_Y;
            ToolStripMenuItem1.Checked = false;
            ToolStripMenuItem2.Checked = true;
            ToolStripMenuItem3.Checked = false;
            ResetCharts();
        }

        private void ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            axesType = AxesType.YZ_X;
            ToolStripMenuItem1.Checked = false;
            ToolStripMenuItem2.Checked = false;
            ToolStripMenuItem3.Checked = true;
            ResetCharts();
        }

        private void positionChart_MouseMove(object sender, MouseEventArgs e)
        {
            double x = GetXFromChart(e.X, sender);
            double y = GetYFromChart(e.Y, sender);

            lblMouseAxisXY.Text = x.ToString("0.0") + ", " + y.ToString("0.0");

            double dX = 0, dY = 0;
            if (e.Button == MouseButtons.Left)  // 拖动平移
            {
                dX = (e.X - lastX) * kx;
                dY = (e.Y - lastY) * ky;

                MoveChartAxisX(dX, sender);
                MoveChartAxisY(dY, sender);

                lastX = e.X;
                lastY = e.Y;
            }

        }

        private void positionChart_MouseDown(object sender, MouseEventArgs e)
        {
            Chart chart = sender as Chart;
            lastX = e.X;
            lastY = e.Y;
            kx = (chart.ChartAreas[0].AxisX.Maximum - chart.ChartAreas[0].AxisX.Minimum) / (chart.Width * chart.ChartAreas[0].Position.Width * 0.01 * chart.ChartAreas[0].InnerPlotPosition.Width * 0.01);
            ky = (chart.ChartAreas[0].AxisY.Maximum - chart.ChartAreas[0].AxisY.Minimum) / (chart.Height * chart.ChartAreas[0].Position.Height * 0.01 * chart.ChartAreas[0].InnerPlotPosition.Height * 0.01);
        }

        DataManagement.CmdDataStruct[] arrCds;//获取整个的导入的指令信息
        DataManagement.CmdDataStruct cds;
        double x;
        double y;
        double z;
        int typeLine = 0;
        int i = 0;

        private void btnLoadPath_Click(object sender, EventArgs e)
        {
            LoadPlannedPath();
        }
        //加载路径
        public void LoadPlannedPath(bool bPausePrinting = true)
        {
            //if (bPausePrinting)
            //{
            //    GV.bPausePrint = true;
            //    GV.PrintingObj.IsPrinting = false;
            //}
            listBox1.Items.Clear();
            lastIndex = 0;

            ClearChart();
            arrCds = GV.PrintingObj.DataObj.CmdQueue.ToArray();//加载信息

            // 添加第1层第1个点（取当前位置）:
            i = 0;

            if (GV.connMode == ConnectMode.ConnectSimulator)
            {
                //设置仿真时的起点位置为动子的左上角
                x = GV.stageXa - GV.sizePrint / 2.0;
                y = GV.stageY - GV.sizePrint / 2.0;
                z = 30;
            }
            else
            {             
                x = GV.PrintingObj.Status.fPosX;
                y = GV.PrintingObj.Status.fPosY;
                z = GV.PrintingObj.Status.fPosZ; 
            }
           

            LineInfo info = new LineInfo(-1, "", 0, 2);
            AddSeriesPoint(x, y, z, info); 
            i++;
            //GV.seriXY1.Points[0].XValue = x;
            //GV.seriXY1.Points[0].YValues[0] = y;
            //GV.seriXZ1.Points[0].XValue = x;
            //GV.seriXZ1.Points[0].YValues[0] = z;
            //GV.seriYZ1.Points[0].XValue = y;
            //GV.seriYZ1.Points[0].YValues[0] = z;
            timer1.Interval = 1;
            timer1.Start();
            timer2.Stop();
            lblNotice.Text = "路径加载中：" + GV.PathFileName;
            lblNotice.Show();
            btnPauseLoad.Text = "暂停加载";
            //lblNotice.Show();
            //lblNotice.ForeColor = Color.Black;
        }
        //显示路径
        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int k  = 0; k < 100; k++)//每次刷新100个点
            {
                if (i >= arrCds.Length)
                {
                    timer1.Stop();
                    GV.MsgShow(lblNotice, "路径加载完毕！", timer2, 30000, Color.FromArgb(0, 192, 0));
                    this.Text = "打印预览";
                    i = 0;
                    return;
                }
                switch ((i / 20) % 4)
                {
                    case 0:
                        this.Text = "打印预览—路径加载中";
                        break;
                    case 1:
                        this.Text = "打印预览—路径加载中.";
                        break;
                    case 2:
                        this.Text = "打印预览—路径加载中..";
                        break;
                    case 3:
                        this.Text = "打印预览—路径加载中...";
                        break;
                    default:
                        break;
                }

                cds = arrCds[i];//
                int layer = -1;
                string gCode = "";
                int index = 0;
                int printPos = 2;
                try
                {
                    layer = Convert.ToInt32(cds.Para9);
                    gCode = cds.Para10;
                    index = Convert.ToInt32(cds.Para11);
                    printPos = Convert.ToInt32(cds.Para12);
                }
                catch (Exception ex)
                {
                }
                //根据指令解析路径
                switch (cds.CmdName)
                {
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
                            Points[0] = xPos;
                            Points[1] = yPos;

                            //Ch.ExtToPointM(Ch.ACSC_AMF_VELOCITY, Axes, Points, speed, speed);
                            x = xPos;
                            y = yPos;

                            LineInfo info = new LineInfo(layer, gCode, index, printPos);
                            AddSeriesPoint(x, y, z, info);
                            break;
                        }
                    case DataManagement.OptType.MoveAxisTo:
                        {
                            int axis = Convert.ToInt32(cds.Para1);
                            double pos = Convert.ToDouble(cds.Para2);
                            double speed = Convert.ToDouble(cds.Para3);
                            //Ch.ExtToPoint(Ch.ACSC_AMF_VELOCITY, axis, pos, speed, 0);
                            //x = xPos;
                            //y = yPos;
                            switch (axis)
                            {
                                case GV.X:
                                    x = pos;
                                    break;
                                case GV.Y:
                                    y = pos;
                                    break;
                                case GV.Z:
                                    z = pos;
                                    break;
                                default:
                                    break;
                            }
                            LineInfo info = new LineInfo(layer, gCode, index, printPos);
                            AddSeriesPoint(x, y, z, info);
                            break;
                        }
                    case DataManagement.OptType.MoveAxisRelative:
                        {
                            int axis = Convert.ToInt32(cds.Para1);
                            double distance = Convert.ToDouble(cds.Para2);
                            double speed = Convert.ToDouble(cds.Para3);
                            //Ch.ExtToPoint(Ch.ACSC_AMF_VELOCITY + Ch.ACSC_AMF_RELATIVE, axis, distance, speed, 0);
                            switch (axis)
                            {
                                case GV.X:
                                    x += distance;
                                    break;
                                case GV.Y:
                                    y += distance;
                                    break;
                                case GV.Z:
                                    z += distance;
                                    break;
                                default:
                                    break;
                            }
                            LineInfo info = new LineInfo(layer, gCode, index, printPos);
                            AddSeriesPoint(x, y, z, info);
                            break;
                        }
                    case DataManagement.OptType.Extrude:
                        {
                            int port = Convert.ToInt16(cds.Para1);
                            int value = Convert.ToInt16(cds.Para2);
                            typeLine = value;
                            break;
                        }

                    case DataManagement.OptType.SegmentArc1:
                        {
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

                            //Ch.SegmentArc1(0, Axes, Center, FinalPoint, Rotation, Velocity, EndVelocity, null, null, Ch.ACSC_NONE, null);
                            double xc = Center[0];
                            double yc = Center[1];

                            LineInfo info = new LineInfo(layer, gCode, index, printPos);

                            double x1, y1, z1, x2, y2, z2; // 圆弧的起点和终点坐标
                            AxesType axesType = AxesType.XY_Z;

                            if (Axes[0] == GV.X)
                            {
                                if (Axes[1] == GV.Y)
                                {
                                    axesType = AxesType.XY_Z;
                                }
                                else if (Axes[1] == GV.Z)
                                {
                                    axesType = AxesType.XZ_Y;
                                }
                            }
                            else if (Axes[0] == GV.Y)
                            {
                                if (Axes[1] == GV.Z)
                                {
                                    axesType = AxesType.YZ_X;
                                }
                            }
                            switch (axesType)
                            {
                                case AxesType.XY_Z:
                                    x1 = x; y1 = y; z1 = z;
                                    break;
                                case AxesType.XZ_Y:
                                    x1 = x; y1 = z; z1 = y;
                                    break;
                                case AxesType.YZ_X:
                                    x1 = y; y1 = z; z1 = x;
                                    break;
                                default:
                                    x1 = x; y1 = y; z1 = z;
                                    break;
                            }

                            x2 = FinalPoint[0];
                            y2 = FinalPoint[1];

                            double dx, dy, r, theta0, theta1, theta2;

                            // 求圆的半径
                            r = Math.Sqrt((x2 - xc) * (x2 - xc) + (y2 - yc) * (y2 - yc));

                            // 求圆心(xc, yc)到起点(x1, y1)连线与X轴夹角的弧度
                            dx = x1 - xc;
                            dy = y1 - yc;
                            r = Math.Sqrt((x1 - xc) * (x1 - xc) + (y1 - yc) * (y1 - yc));
                            theta0 = Math.Asin(dy / r);
                            if (dx > 0)
                            {
                                theta1 = theta0;
                            }
                            else
                            {
                                theta1 = Math.PI - theta0;
                            }

                            // 求圆心(xc, yc)到终点(x2, y2)连线与X轴夹角的弧度
                            dx = x2 - xc;
                            dy = y2 - yc;
                            theta0 = Math.Asin(dy / r);
                            if (dx > 0)
                            {
                                theta2 = theta0;
                            }
                            else
                            {
                                theta2 = Math.PI - theta0;
                            }

                            double dtheta = 0.1 * Math.Sign(theta2 - theta1);
                            double xtheta, ytheta;
                            for (double theta = theta1; (dtheta > 0 && theta < theta2) || (dtheta < 0 && theta > theta2); theta++)
                            {
                                xtheta = xc + r * Math.Cos(theta);
                                ytheta = yc + r * Math.Sin(theta);
                                switch (axesType)
                                {
                                    case AxesType.XY_Z:
                                        x = xtheta;
                                        y = ytheta;
                                        break;
                                    case AxesType.XZ_Y:
                                        x = xtheta;
                                        z = ytheta;
                                        break;
                                    case AxesType.YZ_X:
                                        y = xtheta;
                                        z = ytheta;
                                        break;
                                    default:
                                        x = xtheta;
                                        y = ytheta;
                                        break;
                                }
                                AddSeriesPoint(x, y, z, info);
                            }
                            switch (axesType)
                            {
                                case AxesType.XY_Z:
                                    x = x2;
                                    y = y2;
                                    break;
                                case AxesType.XZ_Y:
                                    x = x2;
                                    z = y2;
                                    break;
                                case AxesType.YZ_X:
                                    y = x2;
                                    z = y2;
                                    break;
                                default:
                                    x = x2;
                                    y = y2;
                                    break;
                            }
                            AddSeriesPoint(x, y, z, info);
                            break;
                        }

                    case DataManagement.OptType.SegmentArc2:
                        {
                            string[] strAxes = cds.Para1.Split();  //Para1: Axes
                            int[] Axes = new int[strAxes.Length];
                            for (int j = 0; j < strAxes.Length; j++)
                            {
                                Axes[j] = Convert.ToInt32(strAxes[j]);
                            }

                            string[] strCenterPoint = cds.Para2.Split(); //Para2: CenterPoint
                            double[] centerPoint = new double[strCenterPoint.Length];
                            for (int j = 0; j < centerPoint.Length; j++)
                            {
                                centerPoint[j] = Convert.ToDouble(strCenterPoint[j]); //CenterPoint[0]
                            }

                            double Angle = Convert.ToDouble(cds.Para3); //Para3: Angle
                            double Velocity = Convert.ToDouble(cds.Para4); //Para4: Velocity
                            double EndVelocity = Convert.ToDouble(cds.Para5); //Para5: EndVelocity

                            // Ch.SegmentArc2(0, Axes, FinalPoint, Angle, Velocity, EndVelocity, null, null, Ch.ACSC_NONE, null);

                            double xc = centerPoint[0];
                            double yc = centerPoint[1];

                            LineInfo info = new LineInfo(layer, gCode, index, printPos);

                            double x1, y1, z1; // 圆弧的起点
                            AxesType axesType = AxesType.XY_Z;

                            if (Axes[0] == GV.X)
                            {
                                if (Axes[1] == GV.Y)
                                {
                                    axesType = AxesType.XY_Z;
                                }
                                else if (Axes[1] == GV.Z)
                                {
                                    axesType = AxesType.XZ_Y;
                                }
                            }
                            else if (Axes[0] == GV.Y)
                            {
                                if (Axes[1] == GV.Z)
                                {
                                    axesType = AxesType.YZ_X;
                                }
                            }
                            switch (axesType)
                            {
                                case AxesType.XY_Z:
                                    x1 = x; y1 = y; z1 = z;
                                    break;
                                case AxesType.XZ_Y:
                                    x1 = x; y1 = z; z1 = y;
                                    break;
                                case AxesType.YZ_X:
                                    x1 = y; y1 = z; z1 = x;
                                    break;
                                default:
                                    x1 = x; y1 = y; z1 = z;
                                    break;
                            }

                            double dx, dy, r, theta0, theta1, theta2;

                            // 求圆的半径
                            r = Math.Sqrt((x1 - xc) * (x1 - xc) + (y1 - yc) * (y1 - yc));

                            // 求圆心(xc, yc)到起点(x1, y1)连线与X轴夹角的弧度
                            dx = x1 - xc;
                            dy = y1 - yc;
                            r = Math.Sqrt((x1 - xc) * (x1 - xc) + (y1 - yc) * (y1 - yc));
                            theta0 = Math.Asin(dy / r);
                            if (dx > 0)
                            {
                                theta1 = theta0;
                            }
                            else
                            {
                                theta1 = Math.PI - theta0;
                            }

                            // 求圆心(xc, yc)到终点(x2, y2)连线与X轴夹角的弧度
                            theta2 = theta1 + Angle;

                            double dtheta = 0.1 * Math.Sign(Angle);
                            double xtheta, ytheta;
                            for (double theta = theta1; (dtheta > 0 && theta < theta2) || (dtheta < 0 && theta > theta2); theta += dtheta)
                            {
                                xtheta = xc + r * Math.Cos(theta);
                                ytheta = yc + r * Math.Sin(theta);
                                switch (axesType)
                                {
                                    case AxesType.XY_Z:
                                        x = xtheta;
                                        y = ytheta;
                                        break;
                                    case AxesType.XZ_Y:
                                        x = xtheta;
                                        z = ytheta;
                                        break;
                                    case AxesType.YZ_X:
                                        y = xtheta;
                                        z = ytheta;
                                        break;
                                    default:
                                        x = xtheta;
                                        y = ytheta;
                                        break;
                                }
                                AddSeriesPoint(x, y, z, info);
                            }
                            break;
                        }

                    case DataManagement.OptType.SegmentLine:
                        {
                            string[] strAxes = cds.Para1.Split();  //Para1: Axes
                            int[] Axes = new int[strAxes.Length];
                            for (int j = 0; j < strAxes.Length; j++)
                            {
                                Axes[j] = Convert.ToInt32(strAxes[j]);
                            }

                            string[] strFinalPoint = cds.Para2.Split(); //Para2: FinalPoint
                            double[] FinalPoint = new double[strFinalPoint.Length];
                            for (int j = 0; j < FinalPoint.Length; j++)
                            {
                                FinalPoint[j] = Convert.ToDouble(strFinalPoint[j]); //FinalPoint[0]
                            }

                            double Velocity = Convert.ToDouble(cds.Para3);  //Para3: Velocity
                            double EndVelocity = Convert.ToDouble(cds.Para4);  //Para4: EndVelocity

                            //if (isXSEG == -1)
                            //{
                            //    Ch.SegmentLine(Ch.ACSC_AMF_VELOCITY + Ch.ACSC_AMF_ENDVELOCITY, Axes, FinalPoint, Velocity, EndVelocity, null, null, Ch.ACSC_NONE, null);
                            //}
                            //else
                            //{
                            //    Ch.SegmentLine(0, Axes, FinalPoint, Velocity, EndVelocity, null, null, Ch.ACSC_NONE, null);
                            //}
                            //end
                            for (int j = 0; j < strAxes.Length; j++)
                            {
                                switch (Axes[j])
                                {
                                    case GV.X:
                                        x = FinalPoint[j];
                                        break;
                                    case GV.Y:
                                        y = FinalPoint[j];
                                        break;
                                    case GV.Z:
                                        z = FinalPoint[j];
                                        break;
                                }
                            }
                            LineInfo info = new LineInfo(layer, gCode, index, printPos);
                            AddSeriesPoint(x, y, z, info);
                            break;
                        }
                    case DataManagement.OptType.SegmentedMotion:
                        {
                            break;
                        }

                    case DataManagement.OptType.ExtSegmentedMotion:
                        {
                            break;
                        }
                    case DataManagement.OptType.EndSequenceM:
                        {
                            break;
                        }


                    case DataManagement.OptType.EndPrinting:
                        try
                        {
                        }
                        catch (Exception ex)
                        {
                        }
                        break;
                    default:
                        break;
                }

                i++;
                
            }
        }

        private void FrmPathPreview_Load(object sender, EventArgs e)
        {
            ResetCharts();
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            nmudSelectLayer.UpButton();
            ShowLayer((int)nmudSelectLayer.Value - 1);
        }

        //添加路径点
        private void AddSeriesPoint(double x, double y, double z, LineInfo info)
        {
            double offsetX = x + GV.stageXb - GV.stageXa;
            double offsetY = y;
            double offsetZ = z;
            switch (info.printPos)
            {
                case 0:
                    seriXY.Points.AddXY(x, y);//添加系列点
                    seriXZ.Points.AddXY(x, z);
                    seriYZ.Points.AddXY(y, z);
                    break;
                case 1:
                    seriXY2.Points.AddXY(offsetX, offsetY);
                    seriXZ2.Points.AddXY(offsetX, offsetZ); 
                    seriYZ2.Points.AddXY(offsetY, offsetZ);
                    break;
                case 2:
                    seriXY.Points.AddXY(x, y);//添加系列点
                    seriXZ.Points.AddXY(x, z);
                    seriYZ.Points.AddXY(y, z);
                    seriXY2.Points.AddXY(offsetX, offsetY);
                    seriXZ2.Points.AddXY(offsetX, offsetZ);
                    seriYZ2.Points.AddXY(offsetY, offsetZ);
                    break;
            }
            //seriXY.Points.AddXY(x, y);//添加系列点
            //seriXZ.Points.AddXY(x, z);
            //seriYZ.Points.AddXY(y, z);

            int iMax = seriXY.Points.Count - 1;
            int iMaxB = seriXY2.Points.Count - 1;
            seriXY.Points[iMax].Tag = info;
            seriXY2.Points[iMaxB].Tag = info;

            //seriXZ.Points[iMax].Tag = info;
            //seriYZ.Points[iMax].Tag = info;
            
            SetPointFormat(seriXY.Points[iMax], info);
            SetPointFormat(seriXZ.Points[iMax], info);
            SetPointFormat(seriYZ.Points[iMax], info);

            SetPointFormat(seriXY2.Points[iMaxB], info);
            SetPointFormat(seriXZ2.Points[iMaxB], info);
            SetPointFormat(seriYZ2.Points[iMaxB], info);
            if (info.gCode != "" && (listBox1.Items.Count < 1 || info.gCode != listBox1.Items[0]))
            {
                listBox1.Items.Insert(0, iMax.ToString() + ". " + info.gCode);             
            }
        }

        enum LayerShowMode
        {
            Normal = 0,
            Shadow = 1,
            Hide = 2,
            Highlight = 3
        }

        private void SetSeriesPointFormat(int iPoint, LayerShowMode mode)
        {
            try
            {
                if (iPoint < seriXY.Points.Count)
                {
                    LineInfo info = seriXY.Points[iPoint].Tag as LineInfo;
                    SetPointFormat(seriXY.Points[iPoint], info, mode);
                    SetPointFormat(seriXZ.Points[iPoint], info, mode);
                    SetPointFormat(seriYZ.Points[iPoint], info, mode);                    
                }
            }
            catch (Exception ex)
            {
            }
        }
        
        private void SetPointFormat(DataPoint dp, LineInfo info, LayerShowMode mode = LayerShowMode.Normal)
        {
            int width = 0;
            Color color = Color.LightGray;
            int type = info.layer % 4;//每一层显示不同颜色
            switch (mode)
            {
                case LayerShowMode.Normal:
                    if (info.gCode.StartsWith("G0"))
                    {
                        width = 1;
                        color = Color.Gray;
                    }
                    else if (info.gCode.StartsWith("G1") || info.gCode.StartsWith("G2") || info.gCode.StartsWith("G3"))
                    {
                        width = 3;
                        switch (type)
                        {
                            case 0:
                                color = Color.Red;
                                break;
                            case 1:
                                color = Color.Blue;
                                break;
                            case 2:
                                color = Color.FromArgb(0, 192, 0);
                                break;
                            case 3:
                                color = Color.DarkOrange;
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case LayerShowMode.Shadow:
                    if (info.gCode.StartsWith("G0"))
                    {
                        width = 1;
                        color = Color.LightGray;
                    }
                    else if (info.gCode.StartsWith("G1") || info.gCode.StartsWith("G2") || info.gCode.StartsWith("G3"))
                    {
                        width = 3;
                        switch (type)
                        {
                            case 0:
                                color = Color.FromArgb(255, 200, 200);//红
                                break;
                            case 1:
                                color = Color.FromArgb(220, 220, 255); ;//绿
                                break;
                            case 2:
                                color = Color.FromArgb(200, 255, 200);//蓝
                                break;
                            case 3:
                                color = Color.FromArgb(255, 224, 200);//橙
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                case LayerShowMode.Hide:
                    width = 0;
                    color = Color.LightGray;
                    break;
                case  LayerShowMode.Highlight:
                    width = 4;
                    color = Color.FromArgb(0, 0, 0); //192, 0, 192
                    break;
                default:
                    break;
            }

            dp.BorderWidth = width;
            dp.Color = color;
        }


        /// <summary>
        /// 显示指定的层
        /// </summary>
        /// <param name="layer">层编号。默认为-2，表示显示所有层</param>
        private void ShowLayer(int layer = -2)
        {
            // 显示所有层：
            if (layer <= -2)
            {
                for (int i = 0; i < seriXY.Points.Count; i++)
                {
                    if (seriXY.Points[i].Tag.GetType().Name == "LineInfo")
                    {
                        LineInfo info = seriXY.Points[i].Tag as LineInfo;
                        SetPointFormat(seriXY.Points[i], info);
                        SetPointFormat(seriYZ.Points[i], info);
                        SetPointFormat(seriXZ.Points[i], info);
                    }
                }          
                    return;
            } 
            // 单独显示指定层：指定层显示正常颜色，其下方的各层用浅灰色显示，其上方的各层全部隐藏。
            DataPoint dp = seriXY.Points[0];
            for (int i = 0; i < seriXY.Points.Count; i++)
            {
                dp = seriXY.Points[i];
                if (dp.Tag.GetType().Name == "LineInfo")
                {
                    LineInfo info = dp.Tag as LineInfo;

                    if (info.layer < layer) // 在指定层下方的层
                    {
                        SetPointFormat(seriXY.Points[i], info, LayerShowMode.Shadow);
                        SetPointFormat(seriYZ.Points[i], info, LayerShowMode.Shadow);
                        SetPointFormat(seriXZ.Points[i], info, LayerShowMode.Shadow);
                    }
                    else if (info.layer == layer) // 指定层
                    {
                        SetPointFormat(seriXY.Points[i], info, LayerShowMode.Normal);
                        SetPointFormat(seriYZ.Points[i], info, LayerShowMode.Normal);
                        SetPointFormat(seriXZ.Points[i], info, LayerShowMode.Normal);
                    }
                    else // 在指定层上方（隐藏显示）
                    {
                        SetPointFormat(seriXY.Points[i], info, LayerShowMode.Hide);
                        SetPointFormat(seriYZ.Points[i], info, LayerShowMode.Hide);
                        SetPointFormat(seriXZ.Points[i], info, LayerShowMode.Hide);
                    }

                }
            } 
        }


        /// <summary>
        //显示当前文件路径
        /// </summary>
        /// <param name="layer">层编号。默认为-2，表示显示所有层</param>
        private void ShowPrintIndex(int index = 0)
        {
            //// 显示所有层：
            //if (layer <= -2)
            //{
            //    for (int i = 0; i < seriXY.Points.Count; i++)
            //    {
            //        if (seriXY.Points[i].Tag.GetType().Name == "LineInfo")
            //        {
            //            LineInfo info = seriXY.Points[i].Tag as LineInfo;
            //            SetPointFormat(seriXY.Points[i], info);
            //            SetPointFormat(seriYZ.Points[i], info);
            //            SetPointFormat(seriXZ.Points[i], info);
            //        }
            //    }
            //    return;
            //}

            // 显示指定序号：指定层显示正常颜色，其下方的各层用浅灰色显示，其上方的各层全部隐藏。
            DataPoint dp = seriXY.Points[0];
            for (int i = 0; i < seriXY.Points.Count; i++)
            {
                dp = seriXY.Points[i];
                if (dp.Tag.GetType().Name == "LineInfo")
                {
                    LineInfo info = dp.Tag as LineInfo;

                    //if (info.layer < layer) // 在指定层下方的层
                    //{
                    //    SetPointFormat(seriXY.Points[i], info, LayerShowMode.Shadow);
                    //    SetPointFormat(seriYZ.Points[i], info, LayerShowMode.Shadow);
                    //    SetPointFormat(seriXZ.Points[i], info, LayerShowMode.Shadow);
                    //}
                    //else if (info.layer == layer) // 指定层
                    //{
                    //    SetPointFormat(seriXY.Points[i], info, LayerShowMode.Normal);
                    //    SetPointFormat(seriYZ.Points[i], info, LayerShowMode.Normal);
                    //    SetPointFormat(seriXZ.Points[i], info, LayerShowMode.Normal);
                    //}
                    //else // 在指定层上方（隐藏显示）
                    {
                        SetPointFormat(seriXY.Points[i], info, LayerShowMode.Hide);
                        SetPointFormat(seriYZ.Points[i], info, LayerShowMode.Hide);
                        SetPointFormat(seriXZ.Points[i], info, LayerShowMode.Hide);
                    }

                }
            }
        }

        private void btnConfirmPrinting_Click(object sender, EventArgs e)
        {
            GV.ConfirmPrinting();
        }

        public void EnableConfirmPrinting(bool enabled)
        {
            btnConfirmPrinting.Visible = enabled;
        }

        private void FrmPathPreview_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = (e.CloseReason == CloseReason.UserClosing);
            this.Hide();
        }

        private void nmudSelectLayer_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            nmudSelectLayer.DownButton();
            ShowLayer((int)nmudSelectLayer.Value - 1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ShowLayer((int)nmudSelectLayer.Value-1);
        }

        private void timer2_Tick(object sender, EventArgs e)
        {

        }

        private void FrmPathPreview_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case (Keys.LButton | Keys.RButton | Keys.Back | Keys.ShiftKey | Keys.Space | Keys.F17):
                    btnZoomIn_Click(sender, e);
                    break;
                case (Keys.LButton | Keys.MButton | Keys.Back | Keys.ShiftKey | Keys.Space | Keys.F17):
                    btnZoomOut_Click(sender, e);
                    break;
            }
        }

        private void btnExportGcode_Click(object sender, EventArgs e)
        {
            try
            {
                string strText = "";
                for (int i = listBox1.Items.Count - 1; i >= 0; i--)
                {
                    string str = listBox1.Items[i].ToString();
                    int k = str.IndexOf(".");
                    strText += str.Substring(k + 2);
                    strText += "\r\n";
                }
                saveFileDialog1.Filter = "gcode files (*.gcode)|*.gcode";
                saveFileDialog1.DefaultExt = "gcode";
                saveFileDialog1.FileName = "Gcode_" + DateTime.Now.ToString("y.M.d hhmmss");
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string FileName = saveFileDialog1.FileName;
                    if (GV.WriteTextFile(FileName, strText))
                    {
                        if (DialogResult.OK == MessageBox.Show("数据已保存到：" + FileName + "\n\n点击确定打开文件。点击取消关闭此对话。", "提示", MessageBoxButtons.OKCancel))
                        {
                            try
                            {
                                System.Diagnostics.Process.Start(@FileName);
                            }
                            catch (Exception ee)
                            {
                                MessageBox.Show("打开失败！");
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }

            
        }

        int lastIndex = 0;
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string str = listBox1.SelectedItem.ToString();
                string[] strArr = str.Split('.');
                int index = Convert.ToInt32(strArr[0]);
                SetSeriesPointFormat(lastIndex, LayerShowMode.Normal);
                SetSeriesPointFormat(index, LayerShowMode.Highlight);
                lastIndex = index;

            }
            catch (Exception ex)
            {
                
            }
        }

        private void btnPreviewAll_Click(object sender, EventArgs e)
        {
            ShowLayer();
        }

        private void btnPauseLoad_Click(object sender, EventArgs e)
        {
            if (timer1.Enabled)
            {
                timer1.Stop();
                GV.MsgShow(lblNotice, "路径加载暂停...", timer2, 30000, Color.FromArgb(0, 192, 0));
                this.Text = "打印预览";
                btnPauseLoad.Text = "继续加载";
            }
            else
            {
                timer1.Start();
                btnPauseLoad.Text = "暂停加载";
                lblNotice.Text = "路径加载中：" + GV.PathFileName;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            positionChart.ChartAreas[0].AxisX.MajorGrid.Enabled = checkBox1.Checked;
            positionChart.ChartAreas[0].AxisY.MajorGrid.Enabled = checkBox1.Checked;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox1.Items[listBox1.SelectedIndex] = listBox1.SelectedItem.ToString().Replace("G1", "G0");
             listBox1.Refresh();
       }

        private void button5_Click(object sender, EventArgs e)
        {
            listBox1.Items[listBox1.SelectedIndex] = listBox1.SelectedItem.ToString().Replace("G0", "G1");
             listBox1.Refresh();
       }

        private void button6_Click(object sender, EventArgs e)
        {
            //listBox1.SelectedItem = textBox1.Text;
            listBox1.Items[0] = textBox1.Text;
            listBox1.Refresh();
        }

        //显示双工位框架
        bool showDoubleFrame = true;
        private void ShowDoubleFrame(bool show = true)
        {          
            if (show)
            {
                //if (positionChart.Series.Count > 1)
                //{
                //    positionChart.Series.RemoveAt(1); // Remove the existing series at index 1
                //}
                switch (axesType)
                {
                    case AxesType.XY_Z:
                        positionChart.Series[1] = seriXY1;
                        break;

                    case AxesType.XZ_Y:
                        positionChart.Series[1] = seriXZ1;
                        break;

                    case AxesType.YZ_X:
                        positionChart.Series[1] = seriYZ1;
                        break;
                }
            }
            else
            {
                positionChart.Series[1] = seriHide;
            }
        }
        private void ShowPos2()
        {
            switch (axesType)
            {
                case AxesType.XY_Z:
                    positionChart.Series[2] = seriXY2;
                    break;

                case AxesType.XZ_Y:
                    positionChart.Series[2] = seriXZ2;
                    break;

                case AxesType.YZ_X:
                    positionChart.Series[2] = seriYZ2;
                    break;
            }
        }
    }
}
