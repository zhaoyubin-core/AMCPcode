using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace AMCP
{       
    public partial class FrmPathTrace : Form
    {
        public static int lastX = 0; // 最后一次鼠标事件的e.X值
        public static int lastY = 0; // 最后一次鼠标事件的e.Y值
        public static double kx;
        public static double ky;

        bool followCurPosition = false;
        public int refreshInterval = 50;
        public int keepPoints = 10000;

        public FrmPathTrace()
        {
            InitializeComponent();
            this.positionChart.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.positionChart_MouseWheel);
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

        private void FrmImmStop_Load(object sender, EventArgs e)
        {
            ResetCharts();
        }

        public Chart getPositionChart()
        {
            return positionChart;
        }

        public Chart getPositionChart2()
        {
            return positionChart2;
        }

        /// <summary>
        /// 重置当前坐标系
        /// </summary>
        public void ResetCharts()
        {
            switch (GV.axesType)
            {
                case AxesType.XY_Z:
                    positionChart.ChartAreas[0].AxisX.Title = "X";
                    positionChart.ChartAreas[0].AxisY.Title = "Y";
                    positionChart2.ChartAreas[0].AxisY.Title = "Z";

                    positionChart.ChartAreas[0].AxisX.Maximum = GV.X_MAX;
                    positionChart.ChartAreas[0].AxisX.Minimum = 0;

                    positionChart.ChartAreas[0].AxisY.Maximum = GV.Y_MAX;
                    positionChart.ChartAreas[0].AxisY.Minimum = 0;

                    positionChart2.ChartAreas[0].AxisY.Maximum = GV.Z_MAX;
                    positionChart2.ChartAreas[0].AxisY.Minimum = 0;

                    positionChart.Series[0] = GV.seriXY;
                    positionChart.Series[1] = GV.seriXY1;
                    positionChart2.Series[0] = GV.seriZ0;
                    positionChart2.Series[1] = GV.seriZ01;

                    break;

                case AxesType.XZ_Y:
                    positionChart.ChartAreas[0].AxisX.Title = "X";
                    positionChart.ChartAreas[0].AxisY.Title = "Z";
                    positionChart2.ChartAreas[0].AxisY.Title = "Y";

                    positionChart.ChartAreas[0].AxisX.Maximum = GV.X_MAX;
                    positionChart.ChartAreas[0].AxisX.Minimum = 0;

                    positionChart.ChartAreas[0].AxisY.Maximum = GV.Z_MAX;
                    positionChart.ChartAreas[0].AxisY.Minimum = 0;

                    positionChart2.ChartAreas[0].AxisY.Maximum = GV.Y_MAX;
                    positionChart2.ChartAreas[0].AxisY.Minimum = 0;

                    positionChart.Series[0] = GV.seriXZ;
                    positionChart2.Series[0] = GV.seriY0;
                    positionChart.Series[1] = GV.seriXZ1;
                    positionChart2.Series[1] = GV.seriY01;
                    break;

                case AxesType.YZ_X:
                    positionChart.ChartAreas[0].AxisX.Title = "Y";
                    positionChart.ChartAreas[0].AxisY.Title = "Z";
                    positionChart2.ChartAreas[0].AxisY.Title = "X";

                    positionChart.ChartAreas[0].AxisX.Maximum = GV.Y_MAX;
                    positionChart.ChartAreas[0].AxisX.Minimum = 0;

                    positionChart.ChartAreas[0].AxisY.Maximum = GV.Z_MAX;
                    positionChart.ChartAreas[0].AxisY.Minimum = 0;

                    positionChart2.ChartAreas[0].AxisY.Maximum = GV.X_MAX;
                    positionChart2.ChartAreas[0].AxisY.Minimum = 0;

                    positionChart.Series[0] = GV.seriYZ;
                    positionChart2.Series[0] = GV.seriX0;
                    positionChart.Series[1] = GV.seriYZ1;
                    positionChart2.Series[1] = GV.seriX01;

                    break;
            }

            positionChart2.ChartAreas[0].AxisX.Maximum = 10;
            positionChart2.ChartAreas[0].AxisX.Minimum = 0;
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
                //chart.ChartAreas[0].AxisX.Maximum -= dX;
                //chart.ChartAreas[0].AxisX.Minimum -= dX;
                //chart.ChartAreas[0].AxisY.Maximum -= dY;
                //chart.ChartAreas[0].AxisY.Minimum -= dY;

                lastX = e.X;
                lastY = e.Y;
                if (GV.followCurPosition)
                {
                    btnLocate_Click(sender, e);
                }
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

        private void btnLocate_Click(object sender, EventArgs e)
        {
            GV.followCurPosition = !GV.followCurPosition;
            if (GV.followCurPosition)
            {
                btnLocate.BackColor = Color.LightGreen;
            }
            else
            {
                btnLocate.BackColor = SystemColors.Control;
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetCharts();
            if (GV.followCurPosition)
                btnLocate_Click(sender, e);
        }

        private void btnClearChart_Click(object sender, EventArgs e)
        {
            positionChart.Series[0].Points.Clear();
            positionChart2.Series[0].Points.Clear();
            GV.seriXY.Points.Clear();
            GV.seriXZ.Points.Clear();
            GV.seriYZ.Points.Clear();
            GV.seriX0.Points.Clear();
            GV.seriY0.Points.Clear();
            GV.seriZ0.Points.Clear();
        }

        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            GV.axesType = AxesType.XY_Z;
            ToolStripMenuItem1.Checked = true;
            ToolStripMenuItem2.Checked = false;
            ToolStripMenuItem3.Checked = false;
            ResetCharts();
        }

        private void ToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            GV.axesType = AxesType.XZ_Y;
            ToolStripMenuItem1.Checked = false;
            ToolStripMenuItem2.Checked = true;
            ToolStripMenuItem3.Checked = false;
            ResetCharts();
        }

        private void ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            GV.axesType = AxesType.YZ_X;
            ToolStripMenuItem1.Checked = false;
            ToolStripMenuItem2.Checked = false;
            ToolStripMenuItem3.Checked = true;
            ResetCharts();
        }

        private void pathTrace_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = (e.CloseReason == CloseReason.UserClosing);
            this.Hide();
        }

        private void FrmPathTrace_Resize(object sender, EventArgs e)
        {
            try
            {
                splitContainer1.SplitterDistance = this.Height - 100;
                splitContainer2.SplitterDistance = this.Width - 150;
            }
            catch (Exception ex)
            {
            }
        }

        private void FrmPathTrace_KeyDown(object sender, KeyEventArgs e)
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
        public void ShowLayer()
        {
            lblCurrentLayer.Text = GV.PrintingObj.Status.layerCurrent;
        }
        public void ShowMoveCmd()
        {
            lblCurrentCmd.Text = GV.PrintingObj.Status.gcodeCommands ?? GV.PrintingObj.Status.filamentNumber ?? string.Empty;        
        }
    }
}
