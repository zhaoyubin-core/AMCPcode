using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AMCP
{
    public partial class FrmStatusMonitor : Form
    {
        public FrmStatusMonitor()
        {
            InitializeComponent();         
        }

        private void FrmStatusMonitor_Load(object sender, EventArgs e)
        {
        }

        double lastRescaleTime = 0;
        double posX, posY, posZ, velX, velY, velZ, velG, accX, accY, accZ;
        double posZ1, posZ2, velZ1, velZ2;//小z轴
        double xbotPosXa, xbotPosYa, xbotPosZa, xbotPosXb, xbotPosYb, xbotPosZb;//动子状态
        double xbotRxA, xbotRyA, xbotRzA, xbotRxB, xbotRyB, xbotRzB;
       
        public void UpdateMotionStatus(StageStatus Status)   
        {         
            double time;
            posX = Status.fPosX;
            posY = Status.fPosY;
            posZ = Status.fPosZ;
            velX = Status.fVelX;
            velY = Status.fVelY;
            velZ = Status.fVelZ;
            velG = Status.gVel;
            accX = Status.fAccX;
            accY = Status.fAccY;
            accZ = Status.fAccZ;
            time = Status.time * 0.001;
            posZ1 = Status.fPosZ1;
            posZ2 = Status.fPosZ2;
            velZ1 = Status.fVelZ1;
            velZ2 = Status.fVelZ2;
            //动子状态
            xbotPosXa = Status.fXbotPosXa;
            xbotPosYa = Status.fXbotPosYa;
            xbotPosZa = Status.fXbotPosZa;
            xbotPosXb = Status.fXbotPosXb;
            xbotPosYb = Status.fXbotPosYb;
            xbotPosZb = Status.fXbotPosZb;
            xbotRxA = Status.fXbotRxA;
            xbotRyA = Status.fXbotRyA;
            xbotRzA = Status.fXbotRzA;
            xbotRxB = Status.fXbotRxB;
            xbotRyB = Status.fXbotRyB;
            xbotRzB = Status.fXbotRzB;

            //界面显示
            lblFeedbackPos0.Text = posX.ToString("0.0000");
            lblFeedbackPos1.Text = posY.ToString("0.0000");
            lblFeedbackPos2.Text = posZ.ToString("0.0000");
            lblVelo0.Text = Math.Abs(velX).ToString("0.000");
            lblVelo1.Text = Math.Abs(velY).ToString("0.000");
            lblVelo2.Text = Math.Abs(velZ).ToString("0.000");
            lblFeedbackPos3.Text = posZ1.ToString("0.0000");
            lblFeedbackPos4.Text = posZ2.ToString("0.0000");
            lblVel3.Text = Math.Abs(velZ1).ToString("0.0000");
            lblVel4.Text = Math.Abs(velZ2).ToString("0.0000");
            lblSpeed.Text = Math.Sqrt(velX * velX + velY * velY + velZ * velZ + velZ1 * velZ1 + velZ2 * velZ2).ToString("0.000");
            lblTemp.Text = GV.strTemperature;
            txtAcc0.Text = accX.ToString("0.00");
            txtAcc1.Text = accY.ToString("0.00");
            txtAcc2.Text = accZ.ToString("0.00");

            picExtruding.Visible = Status.isExtruding;
            picExtruding2.Visible = Status.isExtruding2;

            lblPosZa.Text = xbotPosZa.ToString("0.00");
            lblPosXa.Text = xbotPosXa.ToString("0.00");
            lblPosYa.Text = xbotPosYa.ToString("0.00");
            lblRotXa.Text = xbotRxA.ToString("0.00");
            lblRotYa.Text = xbotRyA.ToString("0.00");
            lblPosZb.Text = xbotPosZb.ToString("0.00");
            lblPosXb.Text = xbotPosXb.ToString("0.00");
            lblPosYb.Text = xbotPosYb.ToString("0.00");
            lblRotXb.Text = xbotRxB.ToString("0.00");
            lblRotYb.Text = xbotRyB.ToString("0.00");
            panelIsActivateA.BackColor = Status.isActiveA ? Color.White : Color.Gray;
            panelIsActivateB.BackColor = Status.isActiveB ? Color.White : Color.Gray;
            //指令状态
            lblSegCount.Text = Status.GSEG.ToString();
            lblGseg.Text = "GESG:" + Status.GSEG.ToString() + "  G1数量：" + Status.countG1 + "上一段gseg：" + Status.previousMaxCountGseg;
            lblStatus1.Text = "GSFREE: " + Status.GSFREE[0].ToString() + "/" + Status.GSFREE[1].ToString() + "/" + Status.GSFREE[2].ToString();
            lblStatus2.Text = "GRTIME: " + Status.GRTIME[0].ToString("F3") + "/" + Status.GRTIME[1].ToString("F3") + "/" + Status.GRTIME[2].ToString("F3");
            lblCmdSend.Text = GV.Commands;

            //this.Refresh();
            lblFeedbackPos0.Invalidate();
            lblFeedbackPos1.Invalidate();
            lblFeedbackPos2.Invalidate();
            lblVelo0.Invalidate();
            lblVelo1.Invalidate();
            lblVelo2.Invalidate();
            lblVel3.Invalidate();
            lblVel4.Invalidate();
            lblSpeed.Invalidate();
            txtAcc0.Invalidate();
            txtAcc1.Invalidate();
            txtAcc2.Invalidate();
            picExtruding.Invalidate();

            try
            {
                chart1.Series[0].Points.AddXY(time, velX);
                chart1.Series[1].Points.AddXY(time, velY);
                chart1.Series[2].Points.AddXY(time, velZ);
                chart1.Series[3].Points.AddXY(time, velG);
                pointIndex++;

                // Define some variables
                int numberOfPointsInChart = 1000; // 100, 200, 300
                //int numberOfPointsAfterRemoval = numberOfPointsInChart - 1; // 1, 25, 50, 75


                // Keep a constant number of points by removing them from the left
                while (chart1.Series[0].Points.Count > numberOfPointsInChart)
                {
                    //// Remove data points on the left side
                    //while (chart1.Series[0].Points.Count > numberOfPointsAfterRemoval)
                    //{
                    chart1.Series[0].Points.RemoveAt(0);
                    chart1.Series[1].Points.RemoveAt(0);
                    chart1.Series[2].Points.RemoveAt(0);
                    chart1.Series[3].Points.RemoveAt(0);
                    //}

                }
                if (Math.Abs(time - lastRescaleTime) > 0.5)
                {
                    lastRescaleTime = time;
                    // Adjust X axis scale
                    double duration = (double)nmudDuration.Value;
                    double xmax = chart1.Series[0].Points[chart1.Series[0].Points.Count - 1].XValue;
                    double xmin = xmax - duration;
                    if (xmin < 0)
                    {
                        xmin = 0;
                        xmax = duration;
                    }
                    chart1.ChartAreas["Default"].AxisX.Maximum = xmax;
                    chart1.ChartAreas["Default"].AxisX.Minimum = xmin;
                }
                // Redraw chart
                chart1.Invalidate();
            }
            catch (Exception ex)
            {
            }
        }

        public void ResetMonitorChart()
        {
            //chart1.Series[0].Points.Clear();
            //chart1.Series[1].Points.Clear();
            //chart1.Series[2].Points.Clear();
            //chart1.Series[3].Points.Clear();
            //chart1.ChartAreas["Default"].RecalculateAxesScale();
        }

        private Random random = new Random();
        private int pointIndex = 0;

        private void FrmStatusMonitor_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void btnAutoScale_Click(object sender, EventArgs e)
        {
            double minX;
            double maxX;
            double minY;
            double maxY;
            minY = 100;   // chart1.ChartAreas[0].AxisY.Minimum;
            maxY = -100;   // chart1.ChartAreas[0].AxisY.Maximum;
            minX = chart1.ChartAreas[0].AxisX.Minimum;
            maxX = chart1.ChartAreas[0].AxisX.Maximum;

            for (int j = 0; j < 4; j++)
            {
                if (chart1.Series[j].ChartArea == "Default") // 如果该Series被选中，则予以统计Y轴最值，以便设置Y轴显示范围。
                {

                    for (int i = 0; i < chart1.Series[j].Points.Count; i++)
                    {
                        double x = chart1.Series[j].Points[i].XValue;
                        if (x >= minX && x <= maxX)
                        {
                            double y = chart1.Series[j].Points[i].YValues[0];
                            if (y > maxY)
                            {
                                maxY = y;
                            }
                            else if (y < minY)
                            {
                                minY = y;
                            }
                        }
                    }
                }

            }
            double minY2 = minY;
            double maxY2 = maxY;
            minY2 = Math.Floor((minY-1) / 5) * 5;
            maxY2 = Math.Ceiling((maxY +1) / 5) * 5;
            if (minY2 < maxY2)
            {
                chart1.ChartAreas[0].AxisY.Minimum = minY2;
                chart1.ChartAreas[0].AxisY.Maximum = maxY2;                
            }
            label15.Text = "minX= " + minX.ToString("0.0") + " maxX= " + maxX.ToString("0.0") + " minY= " + minY.ToString("0.0") + " maxY= " + maxY.ToString("0.0") + " minY2= " + minY2.ToString("0.0") + " maxY2= " + maxY2.ToString("0.0");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            chart1.Series[0].ChartArea = checkBox1.Checked ? "Default" : "";
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            chart1.Series[1].ChartArea = checkBox2.Checked ? "Default" : "";
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            chart1.Series[2].ChartArea = checkBox3.Checked ? "Default" : "";
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            chart1.Series[3].ChartArea = checkBox4.Checked ? "Default" : "";
        }
        //单独打开1气压
        private void picExtruding_DoubleClick(object sender, EventArgs e)
        {
            GV.PrintingObj.SetExtrudePorts(0, 0);
        }

        private void picNotExtruding_DoubleClick(object sender, EventArgs e)
        {
            GV.PrintingObj.SetExtrudePorts(0, 1);
        }
        //单独打开2气压
        private void picExtruding2_DoubleClick(object sender, EventArgs e)
        {
            GV.PrintingObj.Extrude(1, 0);
        }

        private void picNotExtruding2_DoubleClick(object sender, EventArgs e)
        {
            GV.PrintingObj.Extrude(1, 1);
        }
    }
}
