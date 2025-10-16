using AForge.Controls;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using GxIAPINET;
using OpenCvSharp;
using OpenCvSharp.Flann;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using tscmcnet;
using static AMCP.FrmNozzleCalibrate;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using CheckBox = System.Windows.Forms.CheckBox;
using TextBox = System.Windows.Forms.TextBox;

namespace AMCP
{
    public partial class FrmNozzleCalibrate : Form
    {

        double z2_connact = 0;    // 碰针位置（压力传感器接触临界位置：记录压力值在1~5之间时的Z坐标）
        double[] centerPoint = new double[3] { 0, 0, 0 };

        TextBox[] arr_txtPosHeight;
        CheckBox[] arr_chkPrintPos;

        public FrmNozzleCalibrate()
        {
            InitializeComponent();
            arr_txtPosHeight = new TextBox[] { txtPosHeightA, txtPosHeightB };
            arr_chkPrintPos = new CheckBox[] { chkPrintPosA, chkPrintPosB };
        }

        private void FrmNozzleCalibrate_Enter(object sender, EventArgs e)
        {
            //// 更新打印开始位置
            //GetInitPos();
            //// 更新对针准备位置
            //GetAdjustPos();
            //// 根据当前喷头获取对针Z坐标
            //GetAdjustZ();
            //// 启动状态监测循环
            //timer2.Start();
        }

        public int step = 0;


        private void UpdateNoticeInfo()
        {
            // 更新通知信息
            if (GV.PrintingObj.NoticeInfo == "PositionReady")
            {
                lblNotice.Text = "已到达准备位置";
                lblNotice.BackColor = Color.Green;
                GV.PrintingObj.NoticeInfo = "";
                //btnNozzleCalibrate.Enabled = true;
                //btnNozzleCalibrate.BackColor = Color.LightGreen;
            }
            else if (GV.PrintingObj.NoticeInfo != "")
            {
                //btnNozzleCalibrate.Enabled = false;
                //btnNozzleCalibrate.BackColor = Color.White;
                if (GV.PrintingObj.NoticeInfo == "PrintReady")
                {
                    lblNotice.Text = "已到达打印开始位置";
                    lblNotice.BackColor = Color.Green;
                    GV.PrintingObj.NoticeInfo = "";
                }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (GV.PrintingObj.Status.nozzleID == 1)
            {
                rdbNozzle1.Checked = false;
                rdbNozzle2.Checked = true;
            }
            else
            {
                rdbNozzle1.Checked = true;
                rdbNozzle2.Checked = false;
            }
            GV.frmMotionAdjust.UpdateNozzleStatus();
        }



        private void btnMove2SetP_Click(object sender, EventArgs e)
        {
            if (GV.CheckClearCommands() == ClearResult.DonotClear) return;

            try
            {
                // 从文本框读取设定位置数据
                GV.xSet = Convert.ToDouble(txtAdjustX_A.Text);
                GV.ySet = Convert.ToDouble(txtAdjustY_A.Text);
                GV.zSet = Convert.ToDouble(txtAdjustZ_A.Text);
                // 从文本框读取设定速度数据
                double vSetXY = 30;
                double vSetZ = 20;

                // 获取当前位置数据
                GV.xFed = GV.PrintingObj.Status.fPosX; //Convert.ToDouble(txtFeedbackPos0.Text);
                GV.yFed = GV.PrintingObj.Status.fPosY; //Convert.ToDouble(txtFeedbackPos1.Text);
                GV.zFed = GV.PrintingObj.Status.fPosZ; //Convert.ToDouble(txtFeedbackPos2.Text);
                GV.PrintingObj.qMoveAxisTo(GV.Z, 0, vSetZ, GV.zFed);
                GV.PrintingObj.qWaitMoveEnd();
                GV.PrintingObj.qMoveAxisTo(GV.X, GV.xSet, vSetXY, GV.xFed);
                GV.PrintingObj.qMoveAxisTo(GV.Y, GV.ySet, vSetXY, GV.yFed);
                GV.PrintingObj.qWaitMoveEnd();
                GV.PrintingObj.qMoveAxisTo(GV.Z, GV.zSet, vSetZ / 5, GV.zFed);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSetAdjustPosAsCurrent_Click(object sender, EventArgs e)
        {
            try
            {
                GV.X_ADJUST = Convert.ToDouble(txtAdjustX_A.Text);
                GV.Y_ADJUST = Convert.ToDouble(txtAdjustY_A.Text);
                GV.Z_ADJUST = Convert.ToDouble(txtAdjustZ_A.Text);
                GV.X_ADJUST_B = Convert.ToDouble(txtAdjustX_B.Text);
                GV.Y_ADJUST_B = Convert.ToDouble(txtAdjustY_B.Text);
                GV.Z_ADJUST_B = Convert.ToDouble(txtAdjustZ_B.Text);
                GV.dX_AB = GV.X_ADJUST_B - GV.X_ADJUST;
                GV.dY_AB = GV.Y_ADJUST_B - GV.Y_ADJUST;
                GV.dZ_AB = GV.Z_ADJUST_B - GV.Z_ADJUST;
                GV.X_INIT = Convert.ToDouble(txtInitX.Text);
                GV.Y_INIT = Convert.ToDouble(txtInitY.Text);
                GV.Z_INIT = Convert.ToDouble(txtInitZ.Text);
                GV.Z_INTERVAL = Convert.ToDouble(nmud_d2.Value);
                SaveParameters();
            }
            catch (Exception)
            {
                txt_z0.Focus();
            }
        }

        public void GetInitPos()
        {
            txtInitX.Text = GV.X_INIT.ToString("0.000");
            txtInitY.Text = GV.Y_INIT.ToString("0.000");
            txt_z0.Text = GV.Z_INIT.ToString("0.000");
            txt_d3.Text = GV.Z_OFFSET.ToString("0.000");
            txt_z0.Text = GV.Z_BOTTOM.ToString("0.000");
            nmud_d2.Value = (decimal)GV.Z_INTERVAL;
        }

        public void SetInitPos()
        {
            try
            {
                GV.X_INIT = Convert.ToDouble(txtInitX.Text);
                GV.Y_INIT = Convert.ToDouble(txtInitY.Text);
                GV.Z_INIT = Convert.ToDouble(txt_z0.Text);
                GV.Z_OFFSET = Convert.ToDouble(txt_d3.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void GetAdjustPos()
        {
            // 双喷头校准位置
            txtAdjustX_A.Text = GV.X_ADJUST.ToString("0.000");
            txtAdjustY_A.Text = GV.Y_ADJUST.ToString("0.000");
            txtAdjustZ_A.Text = GV.Z_ADJUST.ToString("0.000");
            txtAdjustX_B.Text = GV.X_ADJUST_B.ToString("0.000");
            txtAdjustY_B.Text = GV.Y_ADJUST_B.ToString("0.000");
            txtAdjustZ_B.Text = GV.Z_ADJUST_B.ToString("0.000");
            // 摄像头校准位置差
            txtCameradX.Text = GV.dX_Camera.ToString("0.000");
            txtCameradY.Text = GV.dY_Camera.ToString("0.000");
            txtCameradZ.Text = GV.dZ_Camera.ToString("0.000");
            // 清洁装置校准位置差
            txtCleandX.Text = GV.dX_Clean.ToString("0.000");
            txtCleandY.Text = GV.dY_Clean.ToString("0.000");
            txtCleandZ.Text = GV.dZ_Clean.ToString("0.000");
        }

        public void SetAdjustPos()
        {
            try
            {
                GV.X_ADJUST = Convert.ToDouble(txtAdjustX_A.Text);
                GV.Y_ADJUST = Convert.ToDouble(txtAdjustY_A.Text);
                GV.Z_ADJUST = Convert.ToDouble(txtAdjustZ_A.Text);
                GV.X_ADJUST_B = Convert.ToDouble(txtAdjustX_B.Text);
                GV.Y_ADJUST_B = Convert.ToDouble(txtAdjustY_B.Text);
                GV.Z_ADJUST_B = Convert.ToDouble(txtAdjustZ_B.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void GetAdjustZ()
        {
            if (GV.PrintingObj.Status.nozzleID == 0) // 左喷头
            {
                txtVar_z2.Text = txtAdjustZ_A.Text;
            }
            else  // 右喷头
            {
                txtVar_z2.Text = txtAdjustZ_B.Text;
            }
        }

        private void SaveParameters()
        {
            try
            {
                GV.frmMotionAdjust.WriteAdvanConfigFile();
            }
            catch (Exception)
            {
            }
        }

        private void ReadParameters()
        {
            try
            {
                GV.frmMotionAdjust.ReadAdvanConfigFile();
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 获取压力值
        /// </summary>
        /// <returns></returns>
        private int GetPressure()
        {
            int value = 0;
            try
            {
                byte[] sendBuffer = new byte[8];
                sendBuffer[0] = 170;
                sendBuffer[1] = 170;
                sendBuffer[2] = 170;
                sendBuffer[3] = 1;
                sendBuffer[4] = 161;
                sendBuffer[5] = 0;
                sendBuffer[6] = 0;
                sendBuffer[7] = 10;

                seriPortSensor1.Write(sendBuffer, 0, sendBuffer.Length);
                nNotReplay++;
                Thread.Sleep(50);

                return value;

            }
            catch (Exception ex)
            {
                return PRESSURE_ERR;
            }
        }

        private bool ZeroPressure()
        {
            try
            {
                step = 12;

                byte[] sendBuffer = new byte[8];
                sendBuffer[0] = 170;
                sendBuffer[1] = 170;
                sendBuffer[2] = 170;
                sendBuffer[3] = 1;
                sendBuffer[4] = 162;
                sendBuffer[5] = 0;
                sendBuffer[6] = 0;
                sendBuffer[7] = 9;
                Thread.Sleep(100);
                step = 13;
                seriPortSensor1.Write(sendBuffer, 0, sendBuffer.Length);
                step = 14;
                nNotReplay++;
                return true;
                //Thread.Sleep(100);

                //byte[] readBuff = new byte[10];
                //seriPortSensor1.Read(readBuff, 0, 10);


            }
            catch (Exception)
            {
                return false;
            }
        }

        private void btnOpenCOM2_Click(object sender, EventArgs e)
        {
            if (btnOpenCOM2.Text == "连接压力传感器")
            {
                try
                {
                    seriPortSensor1.Open();                     //打开串口
                    btnOpenCOM2.Text = "关闭连接";
                    panel2.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "请对通信串口进行配置！");
                    panel2.Enabled = false;
                    return;
                }
            }
            else
            {
                seriPortSensor1.Close();
                btnOpenCOM2.Text = "连接压力传感器";
                panel2.Enabled = false;
            }

        }

        double zStep = 0.05;
        double vStep = 1;
        int direction = 1;

        private void SetZStep(int stepMicroMeter)
        {
            zStep = stepMicroMeter * 0.001;
            lblZStep.Text = (zStep * 1000).ToString("0");
        }

        private void HalfZStep()
        {
            zStep = zStep * 0.5;
            if (zStep < 0.0005)
            {
                zStep = 0.001;
            }
            else if (zStep > 0.01)
            {
                zStep = 0.01;
            }
            lblZStep.Text = (zStep * 1000).ToString("0");
        }

        const int PRESSURE_ERR = 30000;




        System.Windows.Forms.Timer tmrDelay;
        delegate void FuncHandler(object sender, EventArgs e);

        private void DelayCall(FuncHandler f, int delayMsec)//, object tag = null)
        {
            tmrDelay = new System.Windows.Forms.Timer();
            tmrDelay.Tick += new EventHandler(f);
            tmrDelay.Interval = delayMsec > 0 ? delayMsec : 1;
            //tmr.Tag = tag;
            tmrDelay.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                int percent = GV.PrintingObj.GetAdjustStatus();
                progressBar1.Value = percent;
                if (percent <= 0)
                {
                    lblNotice.Text = "尚未执行喷头校准程序";
                }
                if (percent > 0 && percent < 50)
                {
                    lblNotice.Text = "正在对喷头X/Z进行校准";
                    return;
                }
                else if (percent >= 50 && percent < 99)
                {
                    lblNotice.Text = "喷头X/Z校准完成，正在校准Y/Z";
                    return;
                }
                else if (percent == 100)
                {
                    centerPoint = GV.PrintingObj.GetLaserCenter();
                    if (centerPoint != null)
                    {
                        if (centerPoint.Length == 3 && centerPoint[0] == GV.PrintingObj.Status.fPosX && centerPoint[1] == GV.PrintingObj.Status.fPosY && centerPoint[2] == GV.PrintingObj.Status.fPosZ)
                        {
                            lblNotice.Text = "喷头对准成功！";
                            lblNotice.BackColor = Color.Green;
                            btnNozzleCalibrate.Text = "开始校准";
                            timer1.Stop();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void FrmNozzleCalibrate_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = (e.CloseReason == CloseReason.UserClosing);
            this.Hide();
        }

        private void btnResetStep_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            SetZStep(50);
            ZeroPressure();
            lblNotice.Text = "喷头待对准";
            lblNotice.BackColor = Color.FromKnownColor(KnownColor.Control);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            ZeroPressure();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!seriPortSensor1.IsOpen)
            {
                try
                {
                    seriPortSensor1.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            timer1.Interval = (int)numericUpDown1.Value;
            vStep = (int)numericUpDown2.Value;
            direction = 1;
            timer1.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }

        private void btnNozzleCalibrate_Click(object sender, EventArgs e)
        {
            try
            {
                if (timer1.Enabled)
                {
                    GV.PrintingObj.StopBuffer(6);
                    lblNotice.Text = "";
                    progressBar1.Hide();
                    lblNotice.BackColor = Color.Transparent;
                    timer1.Stop();
                    btnNozzleCalibrate.Text = "开始校准";
                }
                else
                {
                    DelayCall(StartAdjustNozzle_Tick, 100);
                    btnNozzleCalibrate.Text = "停止校准";
                }
            }
            catch (Exception ex)
            {
            }
        }


        /// <summary>
        /// 移动喷头到打印初始位置
        /// </summary>
        private void Move2InitPos()
        {
            if (GV.CheckClearCommands() == ClearResult.DonotClear) return;

            double vZup = 30;       // Z轴提针速度
            double vZdown1 = 10;     // Z轴第一阶段下针速度
            double vZdown2 = 2;     // Z轴第二阶段下针速度
            double vXY = 30;        // XY轴移动速度

            // 分4步移动到初始位置：
            // 第1步：快速将Z轴提高至最上面
            GV.PrintingObj.qMoveAxisTo(GV.Z, 0, vZup, 0);
            GV.PrintingObj.qWaitMoveEnd();

            // 第2步：将XY轴移动到初始位置
            GV.PrintingObj.qMoveXYTo(GV.X_INIT, GV.Y_INIT, vXY, 0, 0);
            GV.PrintingObj.qWaitMoveEnd();

            // 第3步：较慢速将Z轴降至接近初始位置
            GV.PrintingObj.qMoveAxisTo(GV.Z, GV.Z_INIT - 5, vZdown1, 0);  // 将Z轴降低至初始调针位置
            GV.PrintingObj.qWaitMoveEnd();

            // 第4步：非常慢速将Z轴降至初始位置
            GV.PrintingObj.qMoveAxisTo(GV.Z, GV.Z_INIT, vZdown2, 0);  // 将Z轴降低至初始调针位置
            GV.PrintingObj.qWaitMoveEnd();

            // 通知准备就绪
            GV.PrintingObj.qDisplayInfo("Notice", "PositionReady");
        }

        private void StartAdjustNozzle_Tick(object sender, EventArgs e)
        {
            try
            {
                progressBar1.Show();
                lblNotice.Text = "喷头正在对准中..";
                lblNotice.BackColor = Color.Red;
                GV.PrintingObj.RunNozzleCalibrate(checkBox1.Checked ? 2 : 1);
                timer1.Start();
                tmrDelay.Stop();
            }
            catch (Exception ex)
            {
            }
        }

        private void btnSetFPosAsAdjust_Click(object sender, EventArgs e)
        {

        }

        private void btnSetFPosAsInit_Click(object sender, EventArgs e)
        {

        }


        private void btnResetAdjust_Click(object sender, EventArgs e)
        {
            ReadParameters();
            GetAdjustPos();
            GetInitPos();
        }

        private void btnCancelAdjust_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            SetZStep(50);
            ZeroPressure();
            lblNotice.BackColor = Color.FromKnownColor(KnownColor.Transparent);
            lblNotice.Text = "";
        }

        private void ResetFormStatus()
        {
            timer1.Stop();
            lblNotice.BackColor = Color.FromKnownColor(KnownColor.Transparent);
            btnNozzleCalibrate.Enabled = false;
            btnNozzleCalibrate.BackColor = Color.White;
            btnNozzleCalibrate.Text = "启动压感对针";
        }

        private void txtInit_Leave(object sender, EventArgs e)
        {
            SetInitPos();
        }

        private void btnMove2InitPos_Click(object sender, EventArgs e)
        {
            //Move2ReadyPos(GV.X_INIT, GV.Y_INIT, 5, "PrintReady");

            double vZup = 30;       // Z轴提针速度(mm/s)
            double vXY = 30;        // XY轴移动速度(mm/s)

            // 第1步：快速将Z轴升至提针位置（若当前位置高于目标位置则Z保持不动）
            if (GV.PrintingObj.Status.fPosZ > GV.Z_TOP)
            {
                GV.PrintingObj.qMoveAxisTo(GV.Z, GV.Z_TOP, vZup, 0);
                GV.PrintingObj.qWaitMoveEnd();
            }
            // 第2步：将XY轴移动到目标位置
            GV.PrintingObj.qMoveXYTo(GV.X_INIT, GV.Y_INIT, vXY, 0, 0);
            GV.PrintingObj.qWaitMoveEnd();
        }

        private void txtPos_Leave(object sender, EventArgs e)
        {
            try
            {
                TextBox txtPos = sender as TextBox;
                txtPos.Text = Convert.ToDouble(txtPos.Text).ToString("0.000");
                SetInitPos();
                SetAdjustPos();
            }
            catch (Exception)
            {
                (sender as Control).Focus();
            }
        }


        private void btnMove2ReadyPos_Click(object sender, EventArgs e)
        {
            double x, y, z;
            if (rdbNozzle2.Checked)
            {
                x = GV.X_ADJUST_B;
                y = GV.Y_ADJUST_B;
                z = GV.Z_ADJUST_B - 5; // 预留5mm安全距离
            }
            else
            {
                x = GV.X_ADJUST;
                y = GV.Y_ADJUST;
                z = GV.Z_ADJUST - 5; // 预留5mm安全距离
            }
            if (DialogResult.OK == MessageBox.Show("确定移动到坐标: (" + x.ToString("0") + ", "
                + y.ToString("0") + "," + z.ToString("0") + ") 吗？\r\n\r\n"
                + "如果您之前没有配置过左右喷头校准位置，请点击“取消”按钮，"
                + "并手动移动到喷头校准装置的十字正上方后，再点击“开始校准”。", "请注意", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation))
            {
                Move2ReadyPos(x, y, z, "PositionReady");
            }
        }

        /// <summary>
        /// 移动到目标位置（先提针、平移、再下针）
        /// </summary>
        /// <param name="x">目标X</param>
        /// <param name="y">目标Y</param>
        /// <param name="z">目标Z</param>
        /// <param name="NoticeInfo">达到目标位置的通知消息</param>
        /// <returns>估计所需运动时间</returns>
        private int Move2ReadyPos(double x, double y, double z, string NoticeInfo)
        {
            if (GV.CheckClearCommands() != ClearResult.Needless) return 0;

            double vZup = 30;       // Z轴提针速度(mm/s)
            double vZdown1 = 10;    // Z轴第一阶段下针速度(mm/s)
            double vZdown2 = 2;     // Z轴第二阶段下针速度(mm/s)
            double vXY = 30;        // XY轴移动速度(mm/s)
            double dNear = 10;       // 接近减速距离(mm)
            // 估计运动至起始点的时间：
            double timeEstimate = 0;

            GV.PrintingObj.qDisplayInfo("Notice", "Moving");

            // 分4步移动到目标位置：
            // 第1步：快速将Z轴升至提针位置
            GV.PrintingObj.qMoveAxisTo(GV.Z, GV.Z_TOP, vZup, 0);
            GV.PrintingObj.qWaitMoveEnd();
            timeEstimate += (Math.Abs(GV.PrintingObj.Status.fPosZ - GV.Z_TOP) / vZup);  // 耗时估算

            // 第2步：将XY轴移动到目标位置
            GV.PrintingObj.qMoveXYTo(x, y, vXY, 0, 0);
            GV.PrintingObj.qWaitMoveEnd();
            timeEstimate += (Math.Sqrt(Math.Pow(GV.PrintingObj.Status.fPosX - x, 2) + Math.Pow(GV.PrintingObj.Status.fPosY - y, 2)) / vXY);  // 耗时估算
            // 第3步：较慢速将Z轴降至接近目标位置
            GV.PrintingObj.qMoveAxisTo(GV.Z, z - dNear, vZdown1, 0);
            GV.PrintingObj.qWaitMoveEnd();
            timeEstimate += (Math.Abs(z - dNear) / vZdown1);  // 耗时估算
            // 第4步：非常慢速将Z轴降至目标位置
            GV.PrintingObj.qMoveAxisTo(GV.Z, z, vZdown2, 0);
            GV.PrintingObj.qWaitMoveEnd();
            GV.PrintingObj.qDisplayInfo("Notice", NoticeInfo);
            timeEstimate += (5 / vZdown2);  // 耗时估算

            timeEstimate += 2;
            return (int)(timeEstimate * 1000);
        }




        bool JogMovingZ = false;

        private void JogMove(int direction)
        {
            if (GV.CheckClearCommands() != ClearResult.Needless) return;
            if (direction == 1)
            {
                GV.PrintingObj.Jog(GV.Z, 1);
            }
            else
            {
                GV.PrintingObj.Jog(GV.Z, -10);
            }
            JogMovingZ = true;
        }

        private void JogMove_Stop()
        {
            if (JogMovingZ)
            {
                GV.PrintingObj.Stop(GV.Z);
                JogMovingZ = false;
            }
        }

        private void btnNozzleDown_MouseDown(object sender, MouseEventArgs e)
        {
            JogMove(1);

        }

        private void btnNozzleDown_MouseUp(object sender, MouseEventArgs e)
        {
            JogMove_Stop();
        }

        private void btnNozzleUp_Click(object sender, EventArgs e)
        {
            if (GV.CheckClearCommands() != ClearResult.Needless) return;
            double curPos = GV.PrintingObj.GetFPosition(GV.Z);
            GV.PrintingObj.qMoveAxisTo(GV.Z, GV.Z_TOP, 30, curPos);
        }

        private void btnSensorPosCalibrate_Click(object sender, EventArgs e)
        {
            if (DialogResult.Cancel == MessageBox.Show("校准前请首先将喷头移动至打印开始位置，并用塞尺精确测量间隙。如果已完成此步骤，点击“确定”立即标定；点击“取消”重新准备", "标定确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2))
            {
                return;
            }
            double z1, z2, d1;
            double d3;
            z1 = Convert.ToDouble(txt_z1.Text);
            z2 = Convert.ToDouble(txt_z2.Text);
            d1 = Convert.ToDouble(nmud_d1.Value);
            d3 = z2 - z1 - d1;
            GV.Z_OFFSET = d3;
            txt_d3.Text = d3.ToString("0.000");


            //if (DialogResult.Cancel == MessageBox.Show("现在喷头正在移动至对针准备位置，喷头已经到达准备位置，点击“确定”启动压感对针程序；点击“取消”重新准备", "喷头是否已经到达对针准备位置？，", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2))
            //{
            //    return;
            //}

        }

        private void nmudHeight_ValueChanged(object sender, EventArgs e)
        {
            btnSensorPosCalibrate.Enabled = false;
            btnSensorPosCalibrate.BackColor = Color.White;
        }

        private void CalcuZ_BOTTOM()
        {
            try
            {
                double z0, z2, d2, d3;
                z2 = Convert.ToDouble(txtVar_z2.Text);
                d2 = (double)nmud_d2.Value;
                d3 = Convert.ToDouble(txtCon_d3.Text);
                GV.Z_OFFSET = d3;

                z0 = z2 - d3 - d2;
                GV.Z_BOTTOM = z0;
                txt_z0.Text = z0.ToString("0.000");
            }
            catch (Exception ex)
            {
            }
        }

        private void label21_Click(object sender, EventArgs e)
        {
            txt_z1.Text = GV.PrintingObj.Status.fPosZ.ToString("0.000");
        }

        private void label21_MouseMove(object sender, MouseEventArgs e)
        {
            label21.ForeColor = Color.Blue;
        }

        private void label21_MouseLeave(object sender, EventArgs e)
        {
            label21.ForeColor = Color.Black;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveParameters();
        }

        private void txt_d3_Leave(object sender, EventArgs e)
        {
            try
            {
                TextBox txt = sender as TextBox;
                GV.Z_OFFSET = Convert.ToDouble(txt.Text);
                txt.Text = GV.Z_OFFSET.ToString("0.000");
                txtCon_d3.Text = txt_d3.Text;
            }
            catch (Exception)
            {
                (sender as Control).Focus();
            }

        }

        private void txt_d3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TextBox txt = sender as TextBox;
                GV.Z_OFFSET = Convert.ToDouble(txt.Text);
                txt.Text = GV.Z_OFFSET.ToString("0.000");
                txtCon_d3.Text = txt_d3.Text;
                CalcuZ_BOTTOM();
            }
            catch (Exception)
            {
                (sender as Control).Focus();
            }

        }

        private void nmud_d2_ValueChanged(object sender, EventArgs e)
        {
            txt_d2.Text = nmud_d2.Value.ToString();
        }

        private void btnChangeNozzle_Click(object sender, EventArgs e)
        {
            Move2ReadyPos(GV.X_NOZZLE_CHANGE, GV.Y_NOZZLE_CHANGE, GV.Z_NOZZLE_CHANGE, "NOZZLE_CHANGE");
        }

        private void label20_MouseLeave(object sender, EventArgs e)
        {
            label20.ForeColor = Color.Black;
        }

        private void label20_MouseMove(object sender, MouseEventArgs e)
        {
            label20.ForeColor = Color.Blue;

        }

        private void label20_Click(object sender, EventArgs e)
        {
            txt_z2.Text = GV.PrintingObj.Status.fPosZ.ToString("0.000");
        }

        int valueP = PRESSURE_ERR;
        int nNotReplay = 0;

        private void seriPortSensor1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                nNotReplay = 0;

                // 监测压力值
                byte[] readBuff = new byte[10];
                seriPortSensor1.Read(readBuff, 0, 10);

                // BB BB BB 01 A1 XX XX 01 FF E5
                if (readBuff[0] == 187 && readBuff[1] == 187 && readBuff[2] == 187 && readBuff[3] == 1 && readBuff[4] == 161)
                {
                    valueP = readBuff[5] * 256 + readBuff[6];
                    valueP = Convert.ToInt16(valueP.ToString("X4"), 16);
                }
                // BB BB BB 01 A2 00 00 XX XX XX
                else if (readBuff[0] == 187 && readBuff[1] == 187 && readBuff[2] == 187 && readBuff[3] == 1 && readBuff[4] == 162)
                {
                    valueP = 0;
                }
            }
            catch (Exception ex)
            {
                valueP = PRESSURE_ERR;
            }
        }

        private void btnMove2ReadyPos_MouseMove(object sender, MouseEventArgs e)
        {
            double x, y, z;
            if (rdbNozzle2.Checked)
            {
                x = GV.X_ADJUST_B;
                y = GV.Y_ADJUST_B;
                z = GV.Z_ADJUST_B - 5; // 预留5mm安全距离
            }
            else
            {
                x = GV.X_ADJUST;
                y = GV.Y_ADJUST;
                z = GV.Z_ADJUST - 5; // 预留5mm安全距离
            }
            lblReadyPos.Text = "(" + x.ToString("0.000") + ", " + y.ToString("0.000") + ", " + z.ToString("0.000") + ")";
            lblReadyPos.Show();
        }

        private void btnMove2ReadyPos_MouseLeave(object sender, EventArgs e)
        {
            lblReadyPos.Text = "";
            lblReadyPos.Hide();
        }

        private void btnMove2InitPos_MouseMove(object sender, MouseEventArgs e)
        {
            CalcuZ_BOTTOM();
            double zPos;
            if (GV.PrintingObj.Status.fPosZ > GV.Z_TOP)
            {
                zPos = GV.Z_TOP;
            }
            else
            {
                zPos = GV.PrintingObj.Status.fPosZ;
            }
            lblInitPos.Text = "(" + GV.X_INIT.ToString("0.000") + ", " + GV.Y_INIT.ToString("0.000") + ", " + zPos.ToString("0.000") + ")";
            lblInitPos.Show();
        }

        private void btnMove2InitPos_MouseLeave(object sender, EventArgs e)
        {
            lblInitPos.Text = "";
            lblInitPos.Hide();
        }

        private void txt_z0_TextChanged(object sender, EventArgs e)
        {
            GV.Z_BOTTOM = Convert.ToDouble(txt_z0.Text);
        }

        private void btnMove2ReadyPos_MouseDown(object sender, MouseEventArgs e)
        {
            //if (GV.PrintingObj.NoticeInfo == "PositionReady" && GV.PrintingObj.Status.fPosZ >= GV.Z_ADJUST)
            //{
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                JogMove(1);
            }

            //}
        }

        private void btnMove2ReadyPos_MouseUp(object sender, MouseEventArgs e)
        {
            JogMove_Stop();
        }

        private void btnGetCenterPoint_Click(object sender, EventArgs e)
        {

        }

        private void lblAdjust_Click(object sender, EventArgs e)
        {
            Control ctrl = sender as Control;
            switch (ctrl.Name.Substring(ctrl.Name.Length - 1, 1))
            {
                case "A":
                    txtAdjustX_A.Text = GV.PrintingObj.Status.fPosX.ToString("0.000");
                    txtAdjustY_A.Text = GV.PrintingObj.Status.fPosY.ToString("0.000");
                    txtAdjustZ_A.Text = GV.PrintingObj.Status.fPosZ.ToString("0.000");
                    break;
                case "B":
                    txtAdjustX_B.Text = GV.PrintingObj.Status.fPosX.ToString("0.000");
                    txtAdjustY_B.Text = GV.PrintingObj.Status.fPosY.ToString("0.000");
                    txtAdjustZ_B.Text = GV.PrintingObj.Status.fPosZ.ToString("0.000");
                    break;
                default:
                    txtAdjustX_A.Text = GV.PrintingObj.Status.fPosX.ToString("0.000");
                    txtAdjustY_A.Text = GV.PrintingObj.Status.fPosY.ToString("0.000");
                    txtAdjustZ_A.Text = GV.PrintingObj.Status.fPosZ.ToString("0.000");
                    break;
            }
            SetAdjustPos();
        }

        private void txtAdjust_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            try
            {
                double Xa = Convert.ToDouble(txtAdjustX_A.Text);
                double Ya = Convert.ToDouble(txtAdjustY_A.Text);
                double Za = Convert.ToDouble(txtAdjustZ_A.Text);
                double Xb = Convert.ToDouble(txtAdjustX_B.Text);
                double Yb = Convert.ToDouble(txtAdjustY_B.Text);
                double Zb = Convert.ToDouble(txtAdjustZ_B.Text);
                txtDX_ab.Text = (Xb - Xa).ToString("0.000");
                txtDY_ab.Text = (Yb - Ya).ToString("0.000");
                txtDZ_ab.Text = (Zb - Za).ToString("0.000");
            }
            catch (Exception ex)
            {
                lblNotice.Text = ex.Message;
                txt.Focus();
            }
        }

        private void label29_Click(object sender, EventArgs e)
        {
            txtVar_z2.Text = GV.PrintingObj.Status.fPosZ.ToString("0.000");
        }

        public void SetNozzle(int nozzleID)
        {
            if (nozzleID == 0)
            {
                rdbNozzle1.Checked = true;
            }
            else
            {
                rdbNozzle2.Checked = true;
            }
        }

        private void rdbNozzle1_CheckedChanged(object sender, EventArgs e)
        {
            GetAdjustZ();
        }

        private void rdbNozzle2_CheckedChanged(object sender, EventArgs e)
        {
            GetAdjustZ();
        }

        private void txtVar_z2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                TextBox txtPos = sender as TextBox;
                double value = Convert.ToDouble(txtPos.Text);
                CalcuZ_BOTTOM();
            }
            catch (Exception)
            {
                (sender as Control).Focus();
            }
        }

        private void btnGetCenterPoint_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (rdbNozzle1.Checked)
                {
                    txtAdjustX_A.Text = centerPoint[0].ToString("0.000");
                    txtAdjustY_A.Text = centerPoint[1].ToString("0.000");
                    txtAdjustZ_A.Text = centerPoint[2].ToString("0.000");
                    txtVar_z2.Text = txtAdjustZ_A.Text;
                }
                else
                {
                    txtAdjustX_B.Text = centerPoint[0].ToString("0.000");
                    txtAdjustY_B.Text = centerPoint[1].ToString("0.000");
                    txtAdjustZ_B.Text = centerPoint[2].ToString("0.000");
                    txtVar_z2.Text = txtAdjustZ_B.Text;
                }
                GV.X_ADJUST = Convert.ToDouble(txtAdjustX_A.Text);
                GV.Y_ADJUST = Convert.ToDouble(txtAdjustY_A.Text);
                GV.Z_ADJUST = Convert.ToDouble(txtAdjustZ_A.Text);
                GV.X_ADJUST_B = Convert.ToDouble(txtAdjustX_B.Text);
                GV.Y_ADJUST_B = Convert.ToDouble(txtAdjustY_B.Text);
                GV.Z_ADJUST_B = Convert.ToDouble(txtAdjustZ_B.Text);
                SaveParameters();
            }
            catch (Exception ex)
            {
            }
        }

        private void btnGetCenterPoint_MouseMove(object sender, MouseEventArgs e)
        {
            centerPoint = GV.PrintingObj.GetLaserCenter();
            if (centerPoint != null)
            {
                if (centerPoint.Length == 3)
                {
                    lblCenterPoint.Text = "(" + centerPoint[0].ToString("0.000") + ", " + centerPoint[1].ToString("0.000") + ", " + centerPoint[2].ToString("0.000") + ")";
                    lblCenterPoint.Show();
                }
            }
        }

        private void btnGetCenterPoint_MouseLeave(object sender, EventArgs e)
        {
            lblCenterPoint.Text = "";
            lblCenterPoint.Hide();
        }

        private void rdbNozzle1_Click(object sender, EventArgs e)
        {
            if (GV.PrintingObj.Status.nozzleID == 1) // 当前为喷头B，将切换为喷头A
            {
                if (DialogResult.OK == MessageBox.Show("当前为右喷头，确定切换到左喷头吗？", "提示", MessageBoxButtons.OKCancel))
                {
                    GV.PrintingObj.Status.nozzleID = 0;
                    GV.PrintingObj.qExtrude(2, 0); // 喷头B气动提升
                    GV.PrintingObj.qPause(1000);    // 等待喷头气动停稳
                    GV.PrintingObj.qMoveAxisRelative(GV.Z, -GV.dZ_AB, 5); // Z轴缓慢下降至预设位置。
                    //GetAdjustZ();
                }
            }
        }

        private void rdbNozzle2_Click(object sender, EventArgs e)
        {
            if (GV.PrintingObj.Status.nozzleID == 0)
            {
                if (DialogResult.OK == MessageBox.Show("当前为左喷头，确定切换到右喷头吗？", "提示", MessageBoxButtons.OKCancel))
                {
                    GV.PrintingObj.Status.nozzleID = 1;
                    GV.PrintingObj.qMoveAxisRelative(GV.Z, GV.dZ_AB - 5, 10); // Z轴留5mm余量，给喷头B气动下降留缓冲余地。
                    GV.PrintingObj.qWaitMoveEnd();
                    GV.PrintingObj.qExtrude(2, 1);  // 喷头B气动下降
                    GV.PrintingObj.qPause(1000);    // 等待喷头气动降下停稳
                    GV.PrintingObj.qMoveAxisRelative(GV.Z, 5, 10); // Z轴在缓冲区继续下降
                    //rdbNozzle2.Checked = true;
                    GetAdjustZ();
                }
            }
        }

        private void label11_Click(object sender, EventArgs e)
        {
            txtInitX.Text = GV.PrintingObj.Status.fPosX.ToString("0.000");
            txtInitY.Text = GV.PrintingObj.Status.fPosY.ToString("0.000");
            SetInitPos();
        }

        private void label11_MouseMove(object sender, MouseEventArgs e)
        {
            label11.ForeColor = Color.Blue;
        }

        private void label11_MouseLeave(object sender, EventArgs e)
        {
            label11.ForeColor = Color.Black;
        }

        private void txt_z0_TextChanged_1(object sender, EventArgs e)
        {
            txtInitZ.Text = txt_z0.Text;
            GV.frmMotionAdjust.SetTxtZBottom(txt_z0.Text);
        }

        private void btnCleanNozzle_Click(object sender, EventArgs e)
        {
            int IsCleanning = GV.PrintingObj.GetExtrudePort(3);
            GV.PrintingObj.Extrude(3, 1 - IsCleanning);
        }

        private void label45_Click(object sender, EventArgs e)
        {
            txtCleanX.Text = GV.PrintingObj.Status.fPosX.ToString("0.000");
            txtCleanY.Text = GV.PrintingObj.Status.fPosY.ToString("0.000");
            txtCleanZ.Text = GV.PrintingObj.Status.fPosZ.ToString("0.000");
        }

        private void label28_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Move2ReadyPos(xClean, yClean, zClean, "PrintReady");
            //// 开始出丝
            GV.PrintingObj.qWaitMoveEnd();
            GV.PrintingObj.Extrude(3, 1);
            GV.PrintingObj.qPause(5000);
            Move2ReadyPos(xClean, yClean, zClean, "PrintReady");
        }

        private void label57_Click(object sender, EventArgs e)
        {
            txtCameraX.Text = GV.PrintingObj.Status.fPosX.ToString("0.000");
            txtCameraY.Text = GV.PrintingObj.Status.fPosY.ToString("0.000");
            txtCameraZ.Text = GV.PrintingObj.Status.fPosZ.ToString("0.000");
        }

        private void label53_Click(object sender, EventArgs e)
        {
            txtNozzleX.Text = GV.PrintingObj.Status.fPosX.ToString("0.000");
            txtNozzleY.Text = GV.PrintingObj.Status.fPosY.ToString("0.000");
            txtNozzleZ.Text = GV.PrintingObj.Status.fPosZ.ToString("0.000");
        }

        private void label61_Click(object sender, EventArgs e)
        {

        }

        private void txtCamera_Leave(object sender, EventArgs e)
        {
            try
            {
                double dx = Convert.ToDouble(txtNozzleX.Text) - Convert.ToDouble(txtCameraX.Text);
                double dy = Convert.ToDouble(txtNozzleY.Text) - Convert.ToDouble(txtCameraY.Text);
                double dz = Convert.ToDouble(txtNozzleZ.Text) - Convert.ToDouble(txtCameraZ.Text);
            }
            catch (Exception ex)
            {
                TextBox txt = (sender as TextBox);
                txt.Focus();
                //txt.SelectAll();
            }
        }

        private void btnSavePara_Click(object sender, EventArgs e)
        {
            SaveParameters();
        }

        private void btnCameraPosCalibrate_Click(object sender, EventArgs e)
        {
            try
            {
                double dx = Convert.ToDouble(txtNozzleX.Text) - Convert.ToDouble(txtCameraX.Text);
                double dy = Convert.ToDouble(txtNozzleY.Text) - Convert.ToDouble(txtCameraY.Text);
                double dz = Convert.ToDouble(txtNozzleZ.Text) - Convert.ToDouble(txtCameraZ.Text);
                if (dz >= 0)
                {
                    txtCameradX.Text = dx.ToString("0.000");
                    txtCameradY.Text = dy.ToString("0.000");
                    txtCameradZ.Text = dz.ToString("0.000");
                    GV.dX_Camera = dx;
                    GV.dY_Camera = dy;
                    GV.dZ_Camera = dz;
                }
                else
                {
                    MessageBox.Show("摄像头位置必须必针头位置高，否则拍照时有可能撞击打印基底，或损坏已打印样品", "警告");
                    txtCameraZ.Focus();
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void btnCalibrateClean_Click(object sender, EventArgs e)
        {
            double xClean = 0, yClean = 0, zClean = 0;
            double xAdjust, yAdjust, zAdjust;
            try
            {
                if (rdbNozzle1.Checked)  //喷头1启用状态
                {
                    xAdjust = GV.X_ADJUST;
                    yAdjust = GV.Y_ADJUST;
                    zAdjust = GV.Z_ADJUST;
                }
                else
                {
                    xAdjust = GV.X_ADJUST_B;
                    yAdjust = GV.Y_ADJUST_B;
                    zAdjust = GV.Z_ADJUST_B;
                }
                xClean = Convert.ToDouble(txtCleanX.Text);
                yClean = Convert.ToDouble(txtCleanY.Text);
                zClean = Convert.ToDouble(txtCleanZ.Text);

                GV.dX_Clean = xClean - xAdjust;
                GV.dY_Clean = yClean - yAdjust;
                GV.dZ_Clean = zClean - zAdjust;

                txtCleandX.Text = GV.dX_Clean.ToString("0.000");
                txtCleandY.Text = GV.dY_Clean.ToString("0.000");
                txtCleandZ.Text = GV.dZ_Clean.ToString("0.000");


            }
            catch (Exception ex)
            {
            }

        }

        double xClean = 0, yClean = 0, zClean = 0;
        private void button3_MouseMove(object sender, MouseEventArgs e)
        {

            double xAdjust, yAdjust, zAdjust;
            try
            {
                if (rdbNozzle1.Checked)  //喷头1启用状态
                {
                    xAdjust = GV.X_ADJUST;
                    yAdjust = GV.Y_ADJUST;
                    zAdjust = GV.Z_ADJUST;
                }
                else
                {
                    xAdjust = GV.X_ADJUST_B;
                    yAdjust = GV.Y_ADJUST_B;
                    zAdjust = GV.Z_ADJUST_B;
                }
                xClean = xAdjust + GV.dX_Clean;
                yClean = yAdjust + GV.dY_Clean;
                zClean = zAdjust + GV.dZ_Clean;

                label63.Text = "(" + xClean.ToString("0.000") + ", " + yClean.ToString("0.000") + ", " + zClean.ToString("0.000") + ")";
                label63.Show();
            }
            catch (Exception ex)
            {
            }

        }


        private void button3_MouseLeave(object sender, EventArgs e)
        {
            label63.Text = "";
            label63.Hide();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                GV.dX_Clean = Convert.ToDouble(txtCleandX.Text);
                GV.dY_Clean = Convert.ToDouble(txtCleandY.Text);
                GV.dZ_Clean = Convert.ToDouble(txtCleandZ.Text);
                SaveParameters();
            }
            catch (Exception ex)
            {
            }
        }

        //******************************************************************************************************************//
        //相机对针

        /// <summary>
        /// 大恒相机
        /// </summary>     

        bool m_bIsOpen = false;                           ///<设备打开状态
        bool m_bIsSnap = false;                           ///<发送开采命令标识
        bool m_bTriggerMode = false;                           ///<是否支持触发模式
        bool m_bTriggerActive = false;                           ///<是否支持触发极性
        bool m_bTriggerSource = false;                           ///<是否支持触发源 
        bool m_bWhiteAuto = false;                           ///<标识是否支持白平衡
        bool m_bBalanceRatioSelector = false;                           ///<标识是否支持白平衡通道
        bool m_bWhiteAutoSelectedIndex = true;                            ///<白平衡列表框转换标志
        IGXFactory m_objIGXFactory = null;                            ///<Factory对像
        IGXDevice m_objIGXDevice = null;                            ///<设备对像
        IGXStream m_objIGXStream = null;                            ///<流对像
        IGXFeatureControl m_objIGXFeatureControl = null;                            ///<远端设备属性控制器对像
        IGXFeatureControl m_objIGXStreamFeatureControl = null;                            ///<流层属性控制器对象
        string m_strBalanceWhiteAutoValue = "Off";                           ///<自动白平衡当前的值
        GxBitmap m_objGxBitmap = null;                            ///<图像显示类对象
        string m_strFilePath = "";                              ///<应用程序当前路径

        private bool singleSnap = false;//单次采集
        List<IGXDeviceInfo> listGXDeviceInfo = new List<IGXDeviceInfo>();
        private void UpdateUI()
        {

        }
        /// <summary>
        /// 相机初始化
        /// </summary>
        void InitDevice()
        {
            if (null != m_objIGXFeatureControl)
            {
                //设置采集模式连续采集
                m_objIGXFeatureControl.GetEnumFeature("AcquisitionMode").SetValue("Continuous");
            }
        }

        /// <summary>
        /// 关闭流
        /// </summary>
        private void CloseStream()
        {
            try
            {
                //关闭流
                if (null != m_objIGXStream)
                {
                    m_objIGXStream.Close();
                    m_objIGXStream = null;
                    m_objIGXStreamFeatureControl = null;
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 关闭设备
        /// </summary>
        private void CloseDevice()
        {
            try
            {
                //关闭设备
                if (null != m_objIGXDevice)
                {
                    m_objIGXDevice.Close();
                    m_objIGXDevice = null;
                }
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// 设备打开后初始化界面，初始配置参数
        /// </summary>
        private void InitUI()
        {
            InitEnumComBoxUI(m_cb_TriggerMode, "TriggerMode", m_objIGXFeatureControl, ref m_bTriggerMode);                      //触发模式初始化
            InitEnumComBoxUI(m_cb_TriggerSource, "TriggerSource", m_objIGXFeatureControl, ref m_bTriggerSource);                //触发源初始化
            InitEnumComBoxUI(m_cb_TriggerActivation, "TriggerActivation", m_objIGXFeatureControl, ref m_bTriggerActive);        //触发极性初始化
            InitShutterUI();                                                                                                    //曝光初始化
            InitGainUI();                                                                                                       //增益的初始化
            CleanBalanceUI();                                                                                                   //清除白平衡相关控件
            InitWhiteRatioUI();                                                                                                 //初始化白平衡系数相关控件
            InitEnumComBoxUI(m_cb_AutoWhite, "BalanceWhiteAuto", m_objIGXFeatureControl, ref m_bWhiteAuto);                     //自动白平衡的初始化
            InitEnumComBoxUI(m_cb_RatioSelector, "BalanceRatioSelector", m_objIGXFeatureControl, ref m_bBalanceRatioSelector);  //白平衡通道选择


            //获取白平衡当前的值
            bool bIsImplemented = false;             //是否支持
            bool bIsReadable = false;                //是否可读
            // 获取是否支持
            if (null != m_objIGXFeatureControl)
            {
                bIsImplemented = m_objIGXFeatureControl.IsImplemented("BalanceWhiteAuto");
                bIsReadable = m_objIGXFeatureControl.IsReadable("BalanceWhiteAuto");
                if (bIsImplemented)
                {
                    if (bIsReadable)
                    {
                        //获取当前功能值
                        m_strBalanceWhiteAutoValue = m_objIGXFeatureControl.GetEnumFeature("BalanceWhiteAuto").GetValue();
                    }
                }
            }
        }
        //打开相机，并开始采集图像
        private void btnConnect2Camera_Click(object sender, EventArgs e)
        {
            if (btnConnect2Camera.Text == "连接至相机")
            {
                try
                {
                    //打开设备
                    if (openDevice(listGXDeviceInfo[cmbEnumDevice.SelectedIndex])) ;
                    {
                        btnConnect2Camera.Text = "关闭相机";
                        if (null != m_objIGXStreamFeatureControl)
                        {
                            try
                            {
                                //设置流层Buffer处理模式为OldestFirst
                                m_objIGXStreamFeatureControl.GetEnumFeature("StreamBufferHandlingMode").SetValue("OldestFirst");
                            }
                            catch (Exception)
                            {
                            }
                        }
                        //开启采集流通道
                        if (null != m_objIGXStream)
                        {
                            //RegisterCaptureCallback第一个参数属于用户自定参数(类型必须为引用
                            //类型)，若用户想用这个参数可以在委托函数中进行使用
                            m_objIGXStream.RegisterCaptureCallback(this, __CaptureCallbackPro);
                            m_objIGXStream.StartGrab();
                        }

                        //发送开采命令
                        if (null != m_objIGXFeatureControl)
                        {
                            m_objIGXFeatureControl.GetCommandFeature("AcquisitionStart").Execute();
                        }
                        m_bIsSnap = true;

                    }
                    //UpdateUI();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else if (m_bIsOpen || m_bIsSnap && btnConnect2Camera.Text == "关闭相机")
            {
                try
                {
                    //发送停采命令
                    if (null != m_objIGXFeatureControl)
                    {
                        m_objIGXFeatureControl.GetCommandFeature("AcquisitionStop").Execute();
                    }

                    //关闭采集流通道
                    if (null != m_objIGXStream)
                    {
                        m_objIGXStream.StopGrab();
                        //注销采集回调函数
                        m_objIGXStream.UnregisterCaptureCallback();
                    }
                    m_objIGXStream.Close();
                    m_objIGXStream = null;
                    m_objIGXStreamFeatureControl = null;

                    m_bIsSnap = false;

                    // 更新界面UI
                    //UpdateUI();
                    try
                    {
                        //关闭设备
                        if (null != m_objIGXDevice)
                        {
                            m_objIGXDevice.Close();
                            m_objIGXDevice = null;
                        }
                    }
                    catch (Exception)
                    {

                    }
                    m_bIsOpen = false;
                    btnConnect2Camera.Text = "连接至相机";

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        //搜索设备
        private void lblSearchDevice_Click(object sender, EventArgs e)
        {
            try
            {
                //刷新界面
                UpdateUI();

                m_objIGXFactory = IGXFactory.GetInstance();
                m_objIGXFactory.Init();
                m_objIGXFactory.UpdateAllDeviceList(500, listGXDeviceInfo);

                // 判断当前连接设备个数
                if (listGXDeviceInfo.Count <= 0)
                {
                    // MessageBox.Show("未发现设备!");
                    return;
                }

                //更新在combox
                cmbEnumDevice.Items.Clear();
                for (int i = 0; i < listGXDeviceInfo.Count; i++)
                {
                    cmbEnumDevice.Items.Add(listGXDeviceInfo[i].GetDisplayName());
                }
                cmbEnumDevice.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private bool openDevice(IGXDeviceInfo targetCam)
        {
            try
            {
                List<IGXDeviceInfo> listGXDeviceInfo = new List<IGXDeviceInfo>();

                //关闭流
                CloseStream();
                // 如果设备已经打开则关闭，保证相机在初始化出错情况下能再次打开
                CloseDevice();

                //// 如果设备已经打开则关闭，保证相机在初始化出错情况下能再次打开
                //if (null != m_objIGXDevice)
                //{
                //    m_objIGXDevice.Close();
                //    m_objIGXDevice = null;
                //}

                //打开设备
                m_objIGXDevice = m_objIGXFactory.OpenDeviceBySN(targetCam.GetSN(), GX_ACCESS_MODE.GX_ACCESS_EXCLUSIVE);
                m_objIGXFeatureControl = m_objIGXDevice.GetRemoteFeatureControl();

                //打开流
                if (null != m_objIGXDevice)
                {
                    m_objIGXStream = m_objIGXDevice.OpenStream(0);
                    m_objIGXStreamFeatureControl = m_objIGXStream.GetFeatureControl();
                }

                // 建议用户在打开网络相机之后，根据当前网络环境设置相机的流通道包长值，
                // 以提高网络相机的采集性能,设置方法参考以下代码。
                GX_DEVICE_CLASS_LIST objDeviceClass = m_objIGXDevice.GetDeviceInfo().GetDeviceClass();
                if (GX_DEVICE_CLASS_LIST.GX_DEVICE_CLASS_GEV == objDeviceClass)
                {
                    // 判断设备是否支持流通道数据包功能
                    if (true == m_objIGXFeatureControl.IsImplemented("GevSCPSPacketSize"))
                    {
                        // 获取当前网络环境的最优包长值
                        uint nPacketSize = m_objIGXStream.GetOptimalPacketSize();
                        // 将最优包长值设置为当前设备的流通道包长值
                        m_objIGXFeatureControl.GetIntFeature("GevSCPSPacketSize").SetValue(nPacketSize);
                    }
                }

                //初始化相机参数
                InitDevice();

                // 获取相机参数,初始化界面控件
                InitUI();

                if (null != m_objGxBitmap)
                {
                    m_objGxBitmap.ReleaseBuffer();
                    m_objGxBitmap = null;
                }
                m_objGxBitmap = new GxBitmap(m_objIGXDevice, pic_ShowImage, m_objIGXStream, m_objIGXFactory);

                // 更新设备打开标识
                m_bIsOpen = true;

            }
            catch (Exception ex)
            {
                // 发生异常时，清理所有相关对象，防止残留状态
                m_objIGXDevice = null;
                m_objIGXFeatureControl = null;
                m_objIGXStream = null;
                m_objIGXStreamFeatureControl = null;
                if (m_objGxBitmap != null)
                {
                    m_objGxBitmap.ReleaseBuffer();
                    m_objGxBitmap = null;
                }
                m_bIsOpen = false;

                MessageBox.Show(ex.Message);
            }
            return m_bIsOpen;
        }

        /// <summary>
        /// 枚举型功能ComBox界面初始化
        /// </summary>
        /// <param name="cbEnum">ComboBox控件名称</param>
        /// <param name="strFeatureName">枚举型功能名称</param>
        /// <param name="objIGXFeatureControl">属性控制器对像</param>
        /// <param name="bIsImplemented">是否支持</param>
        private void InitEnumComBoxUI(System.Windows.Forms.ComboBox cbEnum, string strFeatureName, IGXFeatureControl objIGXFeatureControl, ref bool bIsImplemented)
        {
            string strTriggerValue = "";                   //当前选择项
            List<string> list = new List<string>();   //Combox将要填入的列表
            bool bIsReadable = false;                //是否可读
            bool bIsWrite = false;
            // 获取是否支持
            if (null != objIGXFeatureControl)
            {
                bIsImplemented = objIGXFeatureControl.IsImplemented(strFeatureName);
                // 如果不支持则直接返回
                if (!bIsImplemented)
                {
                    return;
                }

                bIsReadable = objIGXFeatureControl.IsReadable(strFeatureName);

                if (bIsReadable)
                {
                    list.AddRange(objIGXFeatureControl.GetEnumFeature(strFeatureName).GetEnumEntryList());
                    //获取当前功能值
                    strTriggerValue = objIGXFeatureControl.GetEnumFeature(strFeatureName).GetValue();
                }

                bIsWrite = objIGXFeatureControl.IsWritable(strFeatureName);
                // 如果不可写则直接返回
                if (!bIsWrite)
                {
                    bIsImplemented = false;
                    return;
                }
            }

            //清空组合框并更新数据到窗体
            cbEnum.Items.Clear();
            foreach (string str in list)
            {
                cbEnum.Items.Add(str);
            }

            //获得相机值和枚举到值进行比较，刷新对话框
            for (int i = 0; i < cbEnum.Items.Count; i++)
            {
                string strTemp = cbEnum.Items[i].ToString();
                if (strTemp == strTriggerValue)
                {
                    cbEnum.SelectedIndex = i;
                    break;
                }
            }
        }

        /// <summary>
        /// 曝光控制界面初始化
        /// </summary>
        private void InitShutterUI()
        {
            double dCurShuter = 0.0;                       //当前曝光值
            double dMin = 0.0;                       //最小值
            double dMax = 0.0;                       //最大值
            string strUnit = "";                        //单位
            string strText = "";                        //显示内容

            //获取当前相机的曝光值、最小值、最大值和单位
            if (null != m_objIGXFeatureControl)
            {
                dCurShuter = m_objIGXFeatureControl.GetFloatFeature("ExposureTime").GetValue();
                dMin = m_objIGXFeatureControl.GetFloatFeature("ExposureTime").GetMin();
                dMax = m_objIGXFeatureControl.GetFloatFeature("ExposureTime").GetMax();
                strUnit = m_objIGXFeatureControl.GetFloatFeature("ExposureTime").GetUnit();
            }

            //刷新曝光范围及单位到界面上
            strText = string.Format("曝光时间({0}~{1}){2}", dMin.ToString("0.00"), dMax.ToString("0.00"), strUnit);
            m_lbl_Shutter.Text = strText;

            //当前的曝光值刷新到曝光的编辑框
            m_txt_Shutter.Text = dCurShuter.ToString("0.00");
        }

        /// <summary>
        /// 增益控制界面初始化
        /// </summary>
        private void InitGainUI()
        {
            double dCurGain = 0;             //当前增益值
            double dMin = 0.0;           //最小值
            double dMax = 0.0;           //最大值
            string strUnit = "";            //单位
            string strText = "";            //显示内容

            //获取当前相机的增益值、最小值、最大值和单位
            if (null != m_objIGXFeatureControl)
            {
                dCurGain = m_objIGXFeatureControl.GetFloatFeature("Gain").GetValue();
                dMin = m_objIGXFeatureControl.GetFloatFeature("Gain").GetMin();
                dMax = m_objIGXFeatureControl.GetFloatFeature("Gain").GetMax();
                strUnit = m_objIGXFeatureControl.GetFloatFeature("Gain").GetUnit();
            }

            //更新增益值范围到界面
            strText = string.Format("增益({0}~{1}){2}", dMin.ToString("0.00"), dMax.ToString("0.00"), strUnit);
            m_lbl_Gain.Text = strText;

            //当前的增益值刷新到增益的编辑框
            string strCurGain = dCurGain.ToString("0.00");
            m_txt_Gain.Text = strCurGain;
        }
        //开始采集
        private void m_btn_StartDevice_Click(object sender, EventArgs e)
        {
            try
            {
                if (null != m_objIGXStreamFeatureControl)//
                {
                    try
                    {
                        //设置流层Buffer处理模式为OldestFirst,先进先出模式
                        m_objIGXStreamFeatureControl.GetEnumFeature("StreamBufferHandlingMode").SetValue("OldestFirst");
                    }
                    catch (Exception)
                    {
                    }
                }

                //开启采集流通道
                if (null != m_objIGXStream)
                {
                    //RegisterCaptureCallback第一个参数属于用户自定参数(类型必须为引用
                    //类型)，若用户想用这个参数可以在委托函数中进行使用
                    m_objIGXStream.RegisterCaptureCallback(this, __CaptureCallbackPro);
                    m_objIGXStream.StartGrab();
                }

                //发送开采命令
                if (null != m_objIGXFeatureControl)
                {
                    m_objIGXFeatureControl.GetCommandFeature("AcquisitionStart").Execute();
                }
                m_bIsSnap = true;

                // 更新界面UI
                UpdateUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 清空白平衡相关控件
        /// </summary>
        private void CleanBalanceUI()
        {
            //清空自动白平衡

            m_cb_AutoWhite.Items.Clear();
            m_cb_AutoWhite.Text = "";

            //清空自动白平衡通道选择
            m_cb_RatioSelector.Items.Clear();
            m_cb_RatioSelector.Text = "";

            //当前的白平衡文本
            m_lbl_WhiteRatio.Text = "";

            //当前的白平衡系数的编辑框
            m_txt_BalanceRatio.Text = "";
        }
        /// <summary>
        /// 初始化白平衡系数相关控件
        /// </summary>
        private void InitWhiteRatioUI()
        {
            double dWhiteRatio = 0.0;                       //当前曝光值
            double dMin = 0.0;                       //最小值
            double dMax = 0.0;                       //最大值
            string strUnit = "";                        //单位
            string strText = "";                        //显示内容
            bool bIsBalanceRatio = false;                   //是否白平衡是否支持
            //获取当前相机的白平衡系数、最小值、最大值和单位
            if (null != m_objIGXFeatureControl)
            {
                bIsBalanceRatio = m_objIGXFeatureControl.IsImplemented("BalanceRatio");
                bool bBalanceRatioReadable = m_objIGXFeatureControl.IsReadable("BalanceRatio");
                if (!bIsBalanceRatio || !bBalanceRatioReadable)
                {
                    m_txt_BalanceRatio.Enabled = false; ;
                    return;
                }
                dWhiteRatio = m_objIGXFeatureControl.GetFloatFeature("BalanceRatio").GetValue();
                dMin = m_objIGXFeatureControl.GetFloatFeature("BalanceRatio").GetMin();
                dMax = m_objIGXFeatureControl.GetFloatFeature("BalanceRatio").GetMax();
                strUnit = m_objIGXFeatureControl.GetFloatFeature("BalanceRatio").GetUnit();
            }

            //刷新获取白平衡系数范围及单位到界面上
            strText = string.Format("白平衡系数({0}~{1}){2}", dMin.ToString("0.00"), dMax.ToString("0.00"), strUnit);
            m_lbl_WhiteRatio.Text = strText;

            //当前的白平衡系数的编辑框
            m_txt_BalanceRatio.Text = dWhiteRatio.ToString("0.00");
        }
        //停止采集
        private void m_btn_StopDevice_Click(object sender, EventArgs e)
        {
            try
            {

                //发送停采命令
                if (null != m_objIGXFeatureControl)
                {
                    m_objIGXFeatureControl.GetCommandFeature("AcquisitionStop").Execute();
                }

                //关闭采集流通道
                if (null != m_objIGXStream)
                {
                    m_objIGXStream.StopGrab();
                    //注销采集回调函数
                    m_objIGXStream.UnregisterCaptureCallback();
                }

                m_bIsSnap = false;

                // 更新界面UI
                UpdateUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //校验距离，保存照片
        private void btnImageSave_Click(object sender, EventArgs e)
        {


            //进行图像保存       
            try
            {
                if (btnImageSave.Text == "校验距离")
                {
                    //tmrOP2_CalibrateNozzle.Start();
                    //打开触发模式
                    string strTriggerMode = "On";
                    SetEnumValue("TriggerMode", strTriggerMode, m_objIGXFeatureControl);

                    //选择软触发
                    string strTriggerSource = "Software";
                    SetEnumValue("TriggerSource", strTriggerSource, m_objIGXFeatureControl);

                    //设置极性
                    string strTriggerActivation = "RisingEdge";
                    SetEnumValue("TriggerActivation", strTriggerActivation, m_objIGXFeatureControl);

                    //发送软触发命令
                    if (null != m_objIGXFeatureControl)
                    {
                        m_objIGXFeatureControl.GetCommandFeature("TriggerSoftware").Execute();
                    }

                    //采集并保存图像
                    singleSnap = true;//单次采集，采集后便保存
                                      //封装一下

                    //关闭触发模式
                    strTriggerMode = "Off";
                    SetEnumValue("TriggerMode", strTriggerMode, m_objIGXFeatureControl);
                    btnImageSave.Text = "结束校验";
                }
                else if (btnImageSave.Text == "结束校验")
                {
                    tmrOP2_CalibrateNozzle.Stop();
                    btnImageSave.Text = "校验距离";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 图像的显示
        /// </summary
        /// <param name="objIFrameData">图像信息对象</param>
        private void ImageShow(IFrameData objIFrameData)
        {
            try
            {
                m_objGxBitmap.Show(objIFrameData);
            }
            catch (Exception)
            {
            }

            // 是否需要进行图像保存
            // if (m_bSaveBmpImg.Checked)
            if (false)
            {
                DateTime dtNow = System.DateTime.Now;  // 获取系统当前时间
                string strDateTime = dtNow.Year.ToString() + "_"
                                   + dtNow.Month.ToString() + "_"
                                   + dtNow.Day.ToString() + "_"
                                   + dtNow.Hour.ToString() + "_"
                                   + dtNow.Minute.ToString() + "_"
                                   + dtNow.Second.ToString() + "_"
                                   + dtNow.Millisecond.ToString();

                string stfFileName = m_strFilePath + "\\" + strDateTime + ".bmp";  // 默认的图像保存名称
                m_objGxBitmap.SaveBmp(objIFrameData, stfFileName);
            }
        }
        /// <summary>
        /// 对枚举型变量按照功能名称设置值
        /// </summary>
        /// <param name="strFeatureName">枚举功能名称</param>
        /// <param name="strValue">功能的值</param>
        /// <param name="objIGXFeatureControl">属性控制器对像</param>
        private void SetEnumValue(string strFeatureName, string strValue, IGXFeatureControl objIGXFeatureControl)
        {
            if (null != objIGXFeatureControl)
            {
                //设置当前功能值
                objIGXFeatureControl.GetEnumFeature(strFeatureName).SetValue(strValue);
            }
        }
        private void m_cb_TriggerMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            //切换"触发模式"combox框响应函数    
            try
            {
                string strValue = m_cb_TriggerMode.Text;
                SetEnumValue("TriggerMode", strValue, m_objIGXFeatureControl);

                // 更新界面UI
                UpdateUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void m_btn_SoftTriggerCommand_Click(object sender, EventArgs e)
        {
            try
            {
                //发送软触发命令
                if (null != m_objIGXFeatureControl)
                {
                    m_objIGXFeatureControl.GetCommandFeature("TriggerSoftware").Execute();
                }

                // 更新界面UI
                UpdateUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        /// <summary>
        /*-----------------------------------------------------------------------
         * 当切换自动白平衡模式为Once时,设备内部在设置完Once模式后会自动更新为off,
         * 为了与设备状态保持一致,程序以代码模拟该过程：判断当前设置模式为Once后,
         * 将界面随即更新为off(由UpdateWhiteAutoUI()函数实现),但此过程会导致函数
         * m_cb_AutoWhite_SelectedIndexChanged()执行两次,第二次执行时自动白平衡
         * 选项已经更新为off,若重新执行可能会打断Once的设置过程,引起白平衡不起作用,
         * 参数m_bWhiteAutoSelectedIndex即是为了解决函数重入问题而引入的变量
         ------------------------------------------------------------------------*/
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void m_cb_AutoWhite_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (!m_bWhiteAutoSelectedIndex)
                {
                    return;
                }
                string strValue = m_cb_AutoWhite.Text;
                SetEnumValue("BalanceWhiteAuto", strValue, m_objIGXFeatureControl);
                m_strBalanceWhiteAutoValue = strValue;
                // 获取白平衡系数更新界面
                InitWhiteRatioUI();
                // 更新界面UI
                UpdateUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //******************************************************************************************************************//
        //对针操作

        //相机对针所需参数
        double xNozzle, yNozzle, zNozzle, hNozzle;
        double z_OP1 = 0, z_OP2 = 0, z1_OP2, z2_OP2, d_OP1 = 0, d_OP2 = 0, d_target = 0, z1_target = 0, z2_target = 0;
        //z1_target = Z + z1 ,z2_target = Z + z2 = z_OP2 + z1_OP2 + d_OP2 - d_target
        //z_target = (z_OP1 - z_OP2) + (z_OP3 - d_target) + (d_OP2 - d_OP1) + (p_OP2 - p_OP3);
        double[][] p_OP1_matrix = new double[2][];//调平记录数据


        private void ResetMeasureData()
        {
            z_OP1 = 0;
            z_OP2 = 0;
            d_OP1 = 0;
            d_OP2 = 0;
            //p_OP2 = 0;
            d_target = (double)num_dNozzle.Value;

            for (int i = 0; i < 2; i++)
            {
                p_OP1_matrix[i] = new double[] { 0, 0, 0, 0, 0 };
            }
            //GV.arrTargetZ = new double[] { 0, 0, 0, 0, 0, 0 };
            lblNoticeCamOP2.Text = "";
            //lblNoticeOP2.Text = "";
            //lblNoticeOP3.Text = "";
        }


        //移动到基底测量位置(动子中心）
        private void btnGoPrintPos_Click(object sender, EventArgs e)
        {
            try
            {
                if (tmrOP1_MeasureBase.Enabled)//进入调平流程，点击则停止
                {
                    //lblNoticeProgress.BackColor = Color.Transparent;
                    tmrOP1_MeasureBase.Stop();
                    GV.StopImmediately();
                    // btnNozzleCali.Text = "开始校针";
                    //lblNoticeProgress.Text = "";
                    return;
                }
                else
                {
                    tmrOP1_MeasureBase.Start();//进入调平流程
                }

                double x0, y0, z0, z0A, z0B;
                x0 = (double)nmud_OP1_X.Value;//1号工位的位置
                y0 = (double)nmud_OP1_Y.Value;
                z0 = (double)nmud_OP1_Z.Value;

                double xTarget = x0, yTarget = y0, zTarget = z0;

                if (DialogResult.OK == MessageBox.Show("确定移动到工位的打印起始坐标 (" + xTarget.ToString("0.0") + ", "
                    + yTarget.ToString("0.0") + "," + zTarget.ToString("0.000") + ") 吗？", "请注意", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation))
                {
                    Move2ReadyPos(xTarget, yTarget, zTarget, "正在前往共焦点", GV.Z_TOP);
                }
                GV.PrintingObj.qWaitMoveEnd();
                GV.PrintingObj.qDisplayInfo("Notice", "已到达打印起始位置", "Orange");
                ////精细对焦
                //double originalH = valueDistanceA;//当前测量数值
                //if (Math.Abs(originalH) > 0.1 && !double.IsInfinity(originalH))
                //{
                //    double zLiDan = GV.PrintingObj.Status.fPosZ + originalH;
                //    GV.PrintingObj.qMoveAxisTo(GV.Z, zLiDan, 3, GV.PrintingObj.Status.fPosZ);
                //}

                //GV.PrintingObj.qWaitMoveEnd();

            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 移动到目标位置（先提针、平移、再下针）
        /// </summary>
        /// <param name="xt">目标X</param>
        /// <param name="yt">目标Y</param>
        /// <param name="zt">目标Z</param>
        /// <param name="NoticeInfo">达到目标位置的通知消息</param>
        /// <returns>估计所需运动时间</returns>
        private int Move2ReadyPos(double xt, double yt, double zt, string NoticeInfo, double zTop = 0)
        {
            if (GV.CheckClearCommands() != ClearResult.Needless) return 0;

            double x0 = GV.PrintingObj.Status.fPosX;
            double y0 = GV.PrintingObj.Status.fPosY;
            double z0 = GV.PrintingObj.Status.fPosZ;
            double vZup = 20;       // Z轴提针速度(mm/s)
            double vZdown1 = 10;    // Z轴第一阶段下针速度(mm/s)
            double vZdown2 = 2;     // Z轴第二阶段下针速度(mm/s)
            double vXY = 50;        // XY轴移动速度(mm/s)
            double dNear = 10;       // 接近减速距离(mm)
            double zNear;           // 接近位置
            // 估计运动至起始点的时间：
            double timeEstimate = 0;

            GV.PrintingObj.qDisplayInfo("Notice", NoticeInfo, "Orange");
            //检查目标对针距离是否合理
            if (zTop > zt) // 防止提针位置比目标位置还低发生意外
            {
                MessageBox.Show("提针位置比目标位置还低，请检查", "问题提示");
                zTop = zt; // 保持现有高度不动
            }
            if (zTop > z0) // 防止提针位置比当前位置还低，就没必要先提针（实际为下降）
            {
                zTop = z0; // 保持现有高度不动
            }
            // 分步骤移动到目标位置：
            if (x0 != xt || y0 != yt) // 如果当前不在目标位置的X、Y坐标上了
            {
                // 第1步：快速将Z轴升至提针位置
                GV.PrintingObj.qMoveAxisTo(GV.Z, zTop, vZup, 0);
                GV.PrintingObj.qWaitMoveEnd();
                timeEstimate += (Math.Abs(GV.PrintingObj.Status.fPosZ - zTop) / vZup);  // 耗时估算

                // 第2步：将XY轴移动到目标位置
                GV.PrintingObj.qMoveXYTo(xt, yt, vXY, 0, 0);
                GV.PrintingObj.qWaitMoveEnd();
                timeEstimate += (Math.Sqrt(Math.Pow(GV.PrintingObj.Status.fPosX - xt, 2) + Math.Pow(GV.PrintingObj.Status.fPosY - yt, 2)) / vXY);  // 耗时估算
            }
            else // 如果当前已经在目标位置的X、Y坐标上了，那么无需提针，直接移动Z轴到目标位置即可。
            {
                zTop = z0;
            }
            zNear = zt - dNear;
            if (zTop < zNear) // 开始减速的接近目标位置比当前位置低
            {
                // 第3步：较慢速将Z轴降至开始减速的接近目标位置
                GV.PrintingObj.qMoveAxisTo(GV.Z, zNear, vZdown1, 0);
                GV.PrintingObj.qWaitMoveEnd();
                timeEstimate += (Math.Abs(zt - dNear) / vZdown1);  // 耗时估算                
            }

            // 第4步：非常慢速将Z轴降至目标位置
            GV.PrintingObj.qMoveAxisTo(GV.Z, zt, vZdown2, 0);
            GV.PrintingObj.qWaitMoveEnd();
           
            timeEstimate += (dNear / vZdown2);  // 耗时估算

            timeEstimate += 2;
            return (int)(timeEstimate * 1000);
        }
        MeasurePoint[] measurePoints = null;//测量点数组，中间，左边，右边，上边，下边
        string strDisplace;
        double[] arrPointA = new double[5];
        private void btnMeasureBase_Click(object sender, EventArgs e)//位移笔/光谱共焦测基底
        {
            int tsetType = 0;
            try
            {
                switch (tsetType)
                {
                    case 0://非接触式
                        {                      
                            if (listBox1.Items.Count > 0 && measurePoints.Length > 0)
                            {
                                ShowMeasurePoints();//将测量点展示到listbox1
                                listBox1.SelectedIndex = 0;
                                double x, y, z;
                                x = measurePoints[0].X;
                                y = measurePoints[0].Y;
                                z = measurePoints[0].Z;

                                if (DialogResult.OK == MessageBox.Show("确定移动到第一个测量点上方位置: (" + x.ToString("0") + ", "
                                    + y.ToString("0") + "," + z.ToString("0") + ") 开始整个测试过程吗？\r\n\r\n"
                                    + "如果您之前没有把握测量点位置安全，请点击“取消”按钮。", "请注意", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation))
                                {
                                    Move2ReadyPos(x, y, z, "移动中...", GV.Z_TOP);
                                }
                                else
                                {
                                    return;
                                }
                            }
                            else
                            {
                                return;
                            }
                            double[] xx = null, yy = null, zz = null, zzUp = null;
                            xx = new double[measurePoints.Length];
                            yy = new double[measurePoints.Length];
                            //zzUp = new double[measurePoints.Length];
                            zz = new double[measurePoints.Length];

                            for (int i = 0; i < measurePoints.Length; i++)
                            {
                                xx[i] = measurePoints[i].X;
                                yy[i] = measurePoints[i].Y;
                                zz[i] = measurePoints[i].Z;
                                //zzUp[i] = measurePoints[i].ZUp;
                            }
                            GV.valueDisplacementSensor_rA = 0;
                            GV.PrintingObj.qDisplayInfo("DetectPercent", "0");

                            string strMsg = "下面将依次测量列表中的点：";
                            bool bOverLimit = false; // 记录是否有测量点越界
                            for (int i = 0; i < xx.Length; i++)
                            {
                                strMsg += "(" + xx[i].ToString() + ", " + yy[i].ToString() + "), ";
                                if (xx[i] < 0 || xx[i] > 480 || yy[i] < 0 || yy[i] > 750)
                                {
                                    bOverLimit = true;
                                }
                            }
                            if (bOverLimit)
                            {
                                strMsg += "\r\n\r\n注意：存在超越边界的测量点，请调整测量点坐标。";
                                MessageBox.Show(strMsg, "警示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                            else
                            {
                                strMsg += "\r\n\r\n确认测量这些点吗？";
                            }
                            if (DialogResult.OK != MessageBox.Show(strMsg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
                            {
                                return;
                            }
                            

                            double vXY = 40;
                            GV.PrintingObj.qDisplayInfo("DetectPercent", "0");
                            //GV.PrintingObj.qDisplayInfo("Notice", "正在测量");
                            GV.PrintingObj.qDisplayInfo("Notice", "测量中...", "Green");
                            GV.PrintingObj.listDisplacementLiDanRecord.Clear();//记录数据
                            GV.PrintingObj.listDistanceRecordValue.Clear();
                            for (int i = 0; i < xx.Length; i++)
                            {
                                GV.PrintingObj.qMoveXYTo(xx[i], yy[i], vXY, 0, 0); // 平移
                                GV.PrintingObj.qWaitMoveEnd();
                                GV.PrintingObj.qRecordDisplacement(i);
                            }
                            GV.PrintingObj.qDisplayInfo("Notice", "测量完成", "Green");

                            GV.PrintingObj.qMoveXYTo(xx[0], yy[0], vXY, 0, 0); // 平移
                            GV.PrintingObj.qWaitMoveEnd();
                            
                            break;                     
                        }
                    case 1://接触式对针
                        {
                            GV.PrintingObj.PushDownPenSensor();//伸出位移笔
                            if (listBox1.Items.Count > 0 && measurePoints.Length > 0)
                            {
                                ShowMeasurePoints();//将测量点展示到listbox1
                                listBox1.SelectedIndex = 0;
                                GV.PrintingObj.PushDownPenSensor();
                                double x, y, z;
                                x = measurePoints[0].X;
                                y = measurePoints[0].Y;
                                z = Math.Round(measurePoints[0].ZUp);   // 预留安全距离

                                if (DialogResult.OK == MessageBox.Show("确定移动到第一个测量点上方位置: (" + x.ToString("0") + ", "
                                    + y.ToString("0") + "," + z.ToString("0") + ") 开始整个测试过程吗？\r\n\r\n"
                                    + "如果您之前没有把握测量点位置安全，请点击“取消”按钮。", "请注意", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation))
                                {
                                    Move2ReadyPos(x, y, GV.Z_TOP, "PositionReady", GV.Z_TOP);
                                }
                                else
                                {
                                    return;
                                }
                            }
                            else
                            {
                                return;
                            }
                            double zTarget = (double)nmud_OP1_Z.Value;
                            if (measurePoints == null)
                            {
                                return;
                            }
                            else if (measurePoints.Length <= 0)
                            {
                                return;
                            }
                            double[] xx = null, yy = null, zz = null, zzUp = null;
                            xx = new double[measurePoints.Length];
                            yy = new double[measurePoints.Length];
                            zzUp = new double[measurePoints.Length];
                            zz = new double[measurePoints.Length];
                            for (int i = 0; i < measurePoints.Length; i++)
                            {
                                xx[i] = measurePoints[i].X;
                                yy[i] = measurePoints[i].Y;
                                zz[i] = measurePoints[i].Z;
                                zzUp[i] = measurePoints[i].ZUp;
                            }

                            //GV.valueDisplacementSensor_rA[0] = 0;
                            //SetAllowPenDown_Tick(sender, e);//电子式传感器，检测滑台是否到位
                            GV.PrintingObj.qDisplayInfo("DetectPercent", "0");

                            string strMsg = "下面位移笔将依次测量列表中的点：";
                            bool bOverLimit = false; // 记录是否有测量点越界
                            for (int i = 0; i < xx.Length; i++)
                            {
                                strMsg += "(" + xx[i].ToString() + ", " + yy[i].ToString() + "), ";
                                if (xx[i] < 0 || xx[i] > 400 || yy[i] < 100 || yy[i] > 750)
                                {
                                    bOverLimit = true;
                                }
                            }
                            if (bOverLimit)
                            {
                                strMsg += "\r\n\r\n注意：存在超越边界的测量点，请调整测量点坐标。";
                                MessageBox.Show(strMsg, "警示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                                return;
                            }
                            else
                            {
                                strMsg += "\r\n\r\n确认测量这些点吗？";
                            }
                            if (DialogResult.OK != MessageBox.Show(strMsg, "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
                            {
                                return;
                            }
                            int count = xx.Length;
                            double zPress = zTarget;
                            double zUp = zPress - 5;
                            double vXY = 80;
                            double vZdown1 = 2, vZdown2 = 10, vZup = 30;
                            GV.PrintingObj.qDisplayInfo("DetectPercent", "0");
                            GV.PrintingObj.qDisplayInfo("Notice", "正在测量");
                            GV.PrintingObj.listDisplacementLiDanRecord.Clear();//记录数据
                            GV.PrintingObj.listDistanceRecordValue.Clear();
                            for (int i = 0; i < count; i++)
                            {
                                zUp = zzUp[i];
                                if (zUp > zPress) zUp = zPress; // 防止zUp数据错误，比测量位置更低发生危险
                                GV.PrintingObj.qMoveAxisTo(GV.Z, zUp, vZup, 0); // 提针
                                GV.PrintingObj.qWaitMoveEnd();
                                GV.PrintingObj.qMoveXYTo(xx[i], yy[i], vXY, 0, 0); // 平移
                                GV.PrintingObj.qWaitMoveEnd();
                                GV.PrintingObj.qMoveAxisTo(GV.Z, zPress, vZdown1, 0); // 下压
                                GV.PrintingObj.qWaitMoveEnd();
                                GV.PrintingObj.qRecordDisplacement(i);//
                                GV.PrintingObj.qDisplayInfo("DetectPercent", (100 * (i + 1) / count).ToString("0"));
                                GV.PrintingObj.qDisplayInfo("Notice", "正在测量，已完成 " + (i + 1).ToString() + "/" + count.ToString());
                            }
                            GV.PrintingObj.qMoveAxisTo(GV.Z, GV.Z_TOP, vZup, 0);
                            strDisplace = "";
                            tmrOP1_MeasureBase.Start();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void ShowMeasurePoints(int stage = 1)
        {
            listBox1.Items.Clear();
            if (stage == 1)
            {
                if (measurePoints != null)
                {
                    for (int i = 0; i < measurePoints.Length; i++)
                    {
                        measurePoints[i].D1 = -1;
                        measurePoints[i].D2 = -1;
                        listBox1.Items.Add(measurePoints[i].ToListText((i + 1)));
                    }
                }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        string[] strLines;
        string[] strSymbols = { "", ".", "..", "..." };
        int iSymbol = 0;
        bool bAlarmPushed = false;
        int iPosStart = 0, iPosEnd = 0;


        private void button14_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "图像文件|*.jpg;*.jpeg;*.png;*.bmp|所有文件|*.*";
            // openFileDialog1.DefaultExt = "csv";
            openFileDialog1.FileName = "";
            openFileDialog1.Title = "选择针头图像文件";
            DialogResult dlgRslt;
            dlgRslt = openFileDialog1.ShowDialog();
            if (dlgRslt == DialogResult.OK)
            {
                //处理图像，计算结果
                ProcessImage(openFileDialog1.FileName);
            }
        }
        private double _imageScale = 0.2; // 图像缩放比例
        private double _micronsPerPixel = 0.5; // 每像素对应的微米数
        double distanceBase = 0;
        double targetDistance = 0;
        Emgu.CV.Mat img = new Emgu.CV.Mat();
        Emgu.CV.Mat resizedImg = new Emgu.CV.Mat();
        Emgu.CV.Mat grayImg = new Emgu.CV.Mat();
        Emgu.CV.Mat blurredImg = new Emgu.CV.Mat();
        Emgu.CV.Mat binaryImg = new Emgu.CV.Mat();
        Emgu.CV.Mat InputImg = new Emgu.CV.Mat();
        Emgu.CV.Mat resultImg = new Emgu.CV.Mat();


        private void SafeUpdateControl(Control control, Action updateAction)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(updateAction);
            }
            else
            {
                updateAction();
            }
        }

        private void ProcessImage(string imgPath)
        {
            try
            {
                // 如果之前有显示的图像，先释放
                if (pic_ShowImage.Image != null)
                {
                    pic_ShowImage.Image.Dispose();
                    pic_ShowImage.Image = null;
                }

                using (InputImg = CvInvoke.Imread(imgPath, Emgu.CV.CvEnum.ImreadModes.AnyColor))
                {
                    if (InputImg.IsEmpty)
                    {
                        MessageBox.Show("无法读取图像！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    img = InputImg.Clone();
                }
                System.Drawing.Size newSize = System.Drawing.Size.Empty;
                // 缩放图像
                CvInvoke.Resize(img, resizedImg, newSize, _imageScale, _imageScale, Inter.Linear);

                // 预处理
                CvInvoke.CvtColor(resizedImg, grayImg, ColorConversion.Bgr2Gray);

                CvInvoke.GaussianBlur(grayImg, blurredImg, new System.Drawing.Size(5, 5), 0);

                CvInvoke.Threshold(blurredImg, binaryImg, 0, 255, ThresholdType.Binary | ThresholdType.Otsu);

                // 垂直投影+边缘检测（返回 upperY 和 lowerY）
                int[] verticalProj = CalculateVerticalProjection(binaryImg);
                (int upperY, int lowerY) = DetectEdges(verticalProj, binaryImg.Cols);

                // 修正：upperType → upperY
                if (upperY == -1 || lowerY == -1)
                {
                    MessageBox.Show("未检测到边缘！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 计算实际尺寸
                double pixelDist = lowerY - upperY;
                double originalPixelDist = pixelDist / _imageScale;
                double actualUm = originalPixelDist * _micronsPerPixel;

                // 计算 Distance Base（间距测量值的一半）
                distanceBase = actualUm / 2;
                targetDistance = (double)num_dNozzle.Value * 1000;


                // 绘制结果
                resultImg = DrawMeasurementResult(resizedImg, upperY, lowerY, actualUm, distanceBase);

                // 转换图像
                Bitmap resultBmp = MatToBitmapManual(resultImg);




                //保存回原路径
                if (resultBmp != null)
                {
                    // 获取原文件的目录和文件名（不含扩展名）
                    string directory = Path.GetDirectoryName(imgPath);
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(imgPath);
                    string extension = Path.GetExtension(imgPath);

                    // 生成新的文件名（在原文件名后添加 "_processed"）
                    string processedFileName = $"{fileNameWithoutExtension}_processed{extension}";
                    string processedFilePath = Path.Combine(directory, processedFileName);

                    // 保存处理后的图像 - 使用克隆避免资源冲突
                    using (Bitmap saveBmp = (Bitmap)resultBmp.Clone())
                    {
                        saveBmp.Save(processedFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("处理图像时出错: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        private void ShowProgressedImg(Bitmap bmp)
        {
            if (pic_ShowImage.InvokeRequired)
            {
                pic_ShowImage.Invoke(new Action<Bitmap>(ShowProgressedImg), bmp);
            }
            else
            {
                // 先释放旧的图像
                if (pic_ShowImage.Image != null)
                {
                    var oldImage = pic_ShowImage.Image;
                    pic_ShowImage.Image = null;
                    oldImage.Dispose();
                }

                // 显示新的图像
                pic_ShowImage.Image = bmp;

                // 刷新显示
                pic_ShowImage.Refresh();
            }
        }
        private int[] CalculateVerticalProjection(Emgu.CV.Mat binaryImg)
        {
            int rows = binaryImg.Rows;
            int cols = binaryImg.Cols;
            int[] projection = new int[rows];
            byte[] pixelData = new byte[rows * cols];

            GCHandle handle = GCHandle.Alloc(pixelData, GCHandleType.Pinned);
            try
            {
                Marshal.Copy(binaryImg.DataPointer, pixelData, 0, pixelData.Length);
                for (int y = 0; y < rows; y++)
                {
                    int whiteCount = 0;
                    for (int x = 0; x < cols; x++)
                    {
                        if (pixelData[y * cols + x] == 255)
                            whiteCount++;
                    }
                    projection[y] = whiteCount;
                }
            }
            finally
            {
                handle.Free();
            }
            return projection;
        }

        private (int upperY, int lowerY) DetectEdges(int[] projection, int imgWidth)
        {
            int threshold = (int)(imgWidth * 0.85);
            int upperY = -1, lowerY = -1;

            // 找上边缘
            for (int y = 0; y < projection.Length; y++)
            {
                if (projection[y] >= threshold)
                {
                    upperY = y;
                    break;
                }
            }

            // 找下边缘
            for (int y = projection.Length - 1; y >= 0; y--)
            {
                if (projection[y] >= threshold)
                {
                    lowerY = y;
                    break;
                }
            }

            // 修正：upperType → upperY（返回正确的变量名）
            return (upperY, lowerY);
        }
        System.Drawing.Point Point = new System.Drawing.Point();
        private Emgu.CV.Mat DrawMeasurementResult(Emgu.CV.Mat img, int upperY, int lowerY, double actualUm, double distanceBase)
        {
            Emgu.CV.Mat result = img.Clone();
            int midX = result.Cols / 2;


            // 绘制边缘线
            CvInvoke.Line(result, new System.Drawing.Point(0, upperY), new System.Drawing.Point(result.Cols, upperY),
                         new Bgr(Color.Green).MCvScalar, 2);
            CvInvoke.Line(result, new System.Drawing.Point(0, lowerY), new System.Drawing.Point(result.Cols, lowerY),
                         new Bgr(Color.Red).MCvScalar, 2);

            // 绘制测量线
            CvInvoke.Line(result, new System.Drawing.Point(midX, upperY), new System.Drawing.Point(midX, lowerY),
                         new Bgr(Color.Blue).MCvScalar, 2);

            // 1. 标注原始尺寸（黄色）- 修正：upperType → upperY
            CvInvoke.PutText(result, $"length:{actualUm:F2} um",
                             new System.Drawing.Point(midX - 100, (upperY + lowerY) / 2 + 20),
                             FontFace.HersheySimplex, 1.4, new Bgr(Color.Yellow).MCvScalar, 2);

            // 2. 标注 Distance Base（红色）- 修正：upperType → upperY
            CvInvoke.PutText(result, $"Distance Base:{distanceBase:F2} um",
                             new System.Drawing.Point(midX - 120, (upperY + lowerY) / 2 + 45),
                             FontFace.HersheySimplex, 1.4, new Bgr(Color.Red).MCvScalar, 2);

            // 绘制图例
            CvInvoke.PutText(result, "Upper Edge | Lower Edge | Measurement",
                             new System.Drawing.Point(10, 30), FontFace.HersheySimplex, 1.2,
                             new Bgr(Color.Black).MCvScalar, 2);

            return result;
        }

        /// <summary>
        /// 手动实现Mat转Bitmap
        /// </summary>
        private Bitmap MatToBitmapManual(Emgu.CV.Mat mat)
        {
            int width = mat.Cols;
            int height = mat.Rows;
            int channels = mat.NumberOfChannels;

            PixelFormat pixelFormat;
            if (channels == 3)
            {
                pixelFormat = PixelFormat.Format24bppRgb;
            }
            else if (channels == 1)
            {
                pixelFormat = PixelFormat.Format8bppIndexed;
            }
            else
            {
                throw new ArgumentException($"不支持的通道数：{channels}");
            }

            Bitmap bmp = new Bitmap(width, height, pixelFormat);
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, pixelFormat);
            IntPtr bmpPtr = bmpData.Scan0;
            int stride = bmpData.Stride;

            if (channels == 3)
            {
                byte[] matData = new byte[width * height * 3];
                Marshal.Copy(mat.DataPointer, matData, 0, matData.Length);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int matIdx = y * width * 3 + x * 3;
                        int bmpIdx = y * stride + x * 3;

                        // BGR转RGB
                        Marshal.WriteByte(bmpPtr, bmpIdx + 0, matData[matIdx + 2]);
                        Marshal.WriteByte(bmpPtr, bmpIdx + 1, matData[matIdx + 1]);
                        Marshal.WriteByte(bmpPtr, bmpIdx + 2, matData[matIdx + 0]);
                    }
                }
            }
            else if (channels == 1)
            {
                byte[] matData = new byte[width * height];
                Marshal.Copy(mat.DataPointer, matData, 0, matData.Length);

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int matIdx = y * width + x;
                        int bmpIdx = y * stride + x;
                        Marshal.WriteByte(bmpPtr, bmpIdx, matData[matIdx]);
                    }
                }

                // 设置灰度调色板
                ColorPalette palette = bmp.Palette;
                for (int i = 0; i < 256; i++)
                {
                    palette.Entries[i] = Color.FromArgb(i, i, i);
                }
                bmp.Palette = palette;
            }

            bmp.UnlockBits(bmpData);
            return bmp;
        }
        //*************************************************************************************************//
        //调平
        TSCMCAPINET protocol = new TSCMCAPINET();
        public static int controller_index = 0;
        int portCOM = 3;
        //控制器编号 
        int controller_idx = 0;
        bool ret = false;//连接状态
        private ERRCODE err;
        List<double> dataList = new List<double>(); // 用于存储实时数据
        //打开A光谱共焦传感器
        private dynamic _gxApiObject;


        private void btnConnectDistanceSensorA_Click(object sender, EventArgs e)
        {
            protocol.SetConnectionType(CONNECTION_TYPE.USB);
            protocol.SetUSBPort(portCOM);
            ret = protocol.OpenConnectionPort();
            string USBport = Convert.ToString(portCOM);


            if (btnConnectDistanceSensorA.Text == "连接至传感器" && ret == true)
            {
                //USB.Text = "设备端口为:com" + USBport;              
                btnConnectDistanceSensorA.Text = "断开传感器连接";
                GV.frmMain.SetNozzleSensorConnected(true);
                grabTimer.Start();//启动读取光谱共焦传感器数据
                err = protocol.SetConnectionOn(controller_idx);
            }
            else if (err != ERRCODE.OK || btnConnectDistanceSensorA.Text == "断开传感器连接")
            {
                //err = protocol.SetConnectionOff(controller_idx);
                protocol.CloseConnectionPort();
                btnConnectDistanceSensorA.Text = "连接至传感器";
                GV.frmMain.SetNozzleSensorConnected(false);
                grabTimer.Stop();//停止读取光谱共焦传感器数据
            }
        }

        private void lblConnectDisSensorA_Click(object sender, EventArgs e)
        {
            string strOld = cmbDistanceSensorA.Text;
            int indexNew = 0;
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            cmbDistanceSensorA.Items.Clear();
            if (ports.Length > 0)
            {
                for (int i = 0; i < ports.Length; i++)
                {
                    cmbDistanceSensorA.Items.Add(ports[i]);
                    if (strOld == ports[i])
                    {
                        indexNew = i;
                    }
                }
                cmbDistanceSensorA.SelectedIndex = indexNew;
                // 获取端口号（如 COM3 → 3）
                string selectedPort = cmbDistanceSensorA.Text;
                int portNumber = 0;
                if (selectedPort.StartsWith("COM", StringComparison.OrdinalIgnoreCase))
                {
                    int.TryParse(selectedPort.Substring(3), out portNumber);
                }
                portCOM = portNumber;

                // 获取下拉框选中的序号
                int selectedIndex = cmbDistanceSensorA.SelectedIndex;
                portCOM = Convert.ToInt16(cmbDistanceSensorA.Text.Replace("COM", ""));
            }
        }
        private void cmbDistanceSensorA_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 获取下拉框选中的序号
            int selectedIndex = cmbDistanceSensorA.SelectedIndex;
            portCOM = Convert.ToInt16(cmbDistanceSensorA.Text.Replace("COM", ""));
        }
        //测量传感器数据
        private double CatchDistanceSensorValue()
        {
            double[] measurement_data = null;
            double lianxudata = double.NaN; // 默认返回NaN表示无效值

            // 1) 读取一次数据
            err = protocol.GetSingleData(controller_idx, ref measurement_data);
            if (err == ERRCODE.OK && measurement_data != null && measurement_data.Length > 1)
            {
                lianxudata = measurement_data[1];

                // 2) 合法性检查
                if (!double.IsNaN(lianxudata) &&
                    !double.IsInfinity(lianxudata))
                //lianxudata > -10 && lianxudata < 10)
                {
                    // 3) 线程安全地写入数据列表
                    lock (dataList)
                    {
                        dataList.Add(lianxudata);
                    }
                }
            }
            // 返回本次测得的数值
            return lianxudata;
        }

        double valueDistanceA, valueDistanceB;//左右工位读数值

        private void FrmNozzleCalibrate_Load(object sender, EventArgs e)
        {
            DelayCall(FirstConnect_Tick, 1000);
            ReadOPXYZ();
        }

        public void ReadOPXYZ()//读取坐标
        {
            nmud_OP1_X.Value = (decimal)GV.OP1_X;
            nmud_OP1_Y.Value = (decimal)GV.OP1_Y;
            nmud_OP1_Z.Value = (decimal)GV.OP1_Z;
            num_OP2_X.Value = (decimal)GV.OP2_X;
            num_OP2_Y.Value = (decimal)GV.OP2_Y;
            num_OP2_Z.Value = (decimal)GV.OP2_Z;
            num_RangeX.Value = (decimal)(GV.OP1_dX / 2.0);
            num_RangeY.Value = (decimal)(GV.OP1_dY / 2.0);

            //nmud_OP3_X.Value = (decimal)GV.OP3_X;
            //nmud_OP3_Y.Value = (decimal)GV.OP3_Y;
            //nmud_OP3_Z.Value = (decimal)GV.OP3_Z;
            num_dNozzle.Value = (decimal)GV.D_INIT;
        }

        public void WriteOPXYZ()
        {
            GV.OP1_X = (double)nmud_OP1_X.Value;
            GV.OP1_Y = (double)nmud_OP1_Y.Value;
            GV.OP1_Z = (double)nmud_OP1_Z.Value;
            GV.OP2_X = (double)num_OP2_X.Value;
            GV.OP2_Y = (double)num_OP2_Y.Value;
            GV.OP2_Z = (double)num_OP2_Z.Value;
            GV.OP1_dX = (double)num_RangeX.Value * 2;
            GV.OP1_dY = (double)num_RangeY.Value * 2;
            //GV.OP3_X = (double)nmud_OP3_X.Value;
            //GV.OP3_Y = (double)nmud_OP3_Y.Value;
            //GV.OP3_Z = (double)nmud_OP3_Z.Value;
            GV.D_INIT = (double)num_dNozzle.Value;
        }

        private void FirstConnect_Tick(object sender, EventArgs e)
        {
            //btnConnectNozzleSensor(sender, e);
            //RefreshComList(GV.ComNozzleSensor);

            //GetMeasurePoint();
            //ShowMeasurePoints();
            tmrDelay.Stop();
        }

        private void btnSaveOPXYZ_Click(object sender, EventArgs e)
        {
            WriteOPXYZ();
        }

        private void btnAdjustXbotRot_Click(object sender, EventArgs e)
        {
            if (tmrOP1_MeasureBase.Enabled)
            {
                tmrOP1_MeasureBase.Stop();
            }

            double rxA = Convert.ToDouble(txtRxA.Text) / GV.PMC.Rad2Degree, ryA = Convert.ToDouble(txtRyA.Text) / GV.PMC.Rad2Degree,
                   rxB = Convert.ToDouble(txtRxB.Text) / GV.PMC.Rad2Degree, ryB = Convert.ToDouble(txtRyB.Text) / GV.PMC.Rad2Degree;
           
            double tolerance = 0.000001;
            bool isRxAEqual = Math.Abs(rxA - GV.adjustRxA) < tolerance;
            bool isRyAEqual = Math.Abs(ryA - GV.adjustRyA) < tolerance;

            //rxA = GV.adjustRxA; ryA = GV.adjustRyA ; rxB = GV.adjustRyB ; ryB = GV.adjustRyB ;
            if (chkPrintPosA.Checked && (!isRxAEqual || !isRyAEqual))
            {
                 GV.adjustRxA = rxA;
                 GV.adjustRyA = ryA;

                if (Math.Abs(rxA) * 1000 > 0.002)//mRad
                {
                    GV.PrintingObj.qAdjustPMmotor(GV.PMC.arrXBotIds[0], 2, GV.PMC.Rx, rxA, "AdjustRotX");//  
                                                                                                         //GV.PrintingObj.MoveXbotXYrotary(GV.PMC.arrXBotIds[0], GV.PMC.Y, RotX, 3);
                    GV.PrintingObj.qPause(500);
                }
                // 调节Y方向旋转（绕Y轴）
                if (Math.Abs(ryA) * 1000 > 0.002)
                {
                    GV.PrintingObj.qAdjustPMmotor(GV.PMC.arrXBotIds[0], 2, GV.PMC.Ry, ryA, "AdjustRotY");
                    GV.PrintingObj.qPause(500);
                }
            }
        }

        private void num_dNozzle_ValueChanged(object sender, EventArgs e)
        {
            //同步到motionAdjust

        }
        string notice = "";
        string color = "";
       

        //读取传感器数据定时器
        private void grabTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                valueDistanceA = CatchDistanceSensorValue();

                if (Math.Abs(valueDistanceA ) <= 2.5)
                {
                    GV.valueDisplacementSensor_rA = valueDistanceA;
                    lblLidanValueA.Text = valueDistanceA.ToString("0.0000");//显示精度0.1微
                }
                else
                {
                    lblLidanValueA.Text ="超出量程";//显示精度0.1微
                }
                GV.PrintingObj.GetNoticeInfo(out notice, out color);
                lblNoticeProgress.Text = "当前状态：" + notice;
                lblNoticeProgress.BackColor = ColorTranslator.FromHtml(color);
            }
            catch (Exception ex)
            {

            }


            //添加保护措施（大于显示值就抬针，蜂鸣器提醒）//待完善阈值
            //double referenceValue = GV.valueDisplacementSensor;//位移笔读数
            //if (referenceValue > sensorThreashold)
            //{
            //    GV.PrintingObj.AdjustMicroMotor(GV.Z1, 0);//收回滑台

            //    GV.StopImmediately();
            //    //发出警报
            //    //GV.printingObj.PushAlarm(AlarmType.Alarm_Operate_Warning);
            //    bool isLiftNozzle = MessageBox.Show("当前测距仪数值已超过设定阈值：" + sensorThreashold.ToString() + "\n" +
            //        "建议立即抬针并调整下针距离，是否接受？",
            //        "提示",
            //         MessageBoxButtons.OKCancel,  // 确定和取消按钮
            //         MessageBoxIcon.Warning       // 警告图标
            //        ) == DialogResult.OK;
            //    if (isLiftNozzle) GV.PrintingObj.qMoveAxisRelative(GV.Z, 20, 10);
            //}
        }

        //string strDisplace = "";
        System.Windows.Forms.ToolTip toolTip = new System.Windows.Forms.ToolTip();
        int percent;
        public bool[] arrSelectMeasurePos = new bool[2] { false, false };
        //展示测量点
        private void chkPrintPos_CheckedChanged(object sender, EventArgs e)
        {
            GV.frmPrintStep2.SetCheckState(sender as CheckBox);//同步到工位选择
            CheckBox checkBox = sender as CheckBox;
            // 根据CheckBox的Name属性确定索引
            if (checkBox.Name == "chkPrintPosA")
            {
                arrSelectMeasurePos[0] = checkBox.Checked;
            }
            else if (checkBox.Name == "chkPrintPosB")
            {
                arrSelectMeasurePos[1] = checkBox.Checked;
            }
            GetMeasurePoint();
            ShowMeasurePoints();
        }
        private void GetMeasurePoint()
        {
            double xTarget0 = Convert.ToDouble(nmud_OP1_X.Text);
            double yTarget0 = Convert.ToDouble(nmud_OP1_Y.Text);
            double zTarget0 = (double)nmud_OP1_Z.Value;
            double rx = (double)num_RangeX.Value, ry = (double)num_RangeY.Value;
            measurePoints = GetMeasurePoints(arrSelectMeasurePos, xTarget0, yTarget0, zTarget0, rx, ry, 5, 15);
        }

        /// <summary>
        /// 获取测量点坐标序列
        /// </summary>
        /// <param name="bPrintPosSelected">选择需要测量的工位序列</param>
        /// <param name="xTarget0">1#工位的X坐标中心位置</param>
        /// <param name="yTarget0">1#工位的Y坐标中心位置</param>
        /// <param name="zTarget">测量点Z坐标位置</param>
        /// <param name="rx">X方向半径</param>
        /// <param name="ry">Y方向半径</param>
        /// <param name="hSafe1">一级安全高度</param>
        /// <param name="hSafe2">二级安全高度</param>
        private MeasurePoint[] GetMeasurePoints(bool[] bPrintPosSelected, double xTarget0, double yTarget0, double zTarget, double rx, double ry, double hSafe1 = 10, double hSafe2 = 20)
        {
            List<MeasurePoint> listMeasurePoint = new List<MeasurePoint>();
            //List<MeasurePoint> listMeasurePointB = new List<MeasurePoint>();
            int countPrintPos = bPrintPosSelected.Length;

            double xTarget = xTarget0, yTarget = yTarget0;
            //for (int j = 0; j < countPrintPos; j++)
            //{
            //    if (bPrintPosSelected[j])
            //    {
                   
            //    }
            //}
            // GetTargetXYZPrintPos(j, xTarget0, yTarget0, out xTarget, out yTarget);
           
            listMeasurePoint.Add(new MeasurePoint(xTarget, yTarget, zTarget, Math.Floor(zTarget - hSafe2),bPrintPosSelected[0],bPrintPosSelected[1]));
            listMeasurePoint.Add(new MeasurePoint(xTarget - rx, yTarget - ry, zTarget, Math.Floor(zTarget - hSafe1), bPrintPosSelected[0], bPrintPosSelected[1]));//左右
            listMeasurePoint.Add(new MeasurePoint(xTarget + rx, yTarget - ry, zTarget, Math.Floor(zTarget - hSafe1), bPrintPosSelected[0], bPrintPosSelected[1]));

            listMeasurePoint.Add(new MeasurePoint(xTarget + rx, yTarget + ry, zTarget, Math.Floor(zTarget - hSafe1), bPrintPosSelected[0], bPrintPosSelected[1]));//前后
            listMeasurePoint.Add(new MeasurePoint(xTarget - rx, yTarget + ry, zTarget, Math.Floor(zTarget - hSafe1), bPrintPosSelected[0], bPrintPosSelected[1]));
            if (listMeasurePoint.Count == 0)
            {
                listMeasurePoint.Add(new MeasurePoint(xTarget, yTarget, zTarget, zTarget - 30));
            }
            return listMeasurePoint.ToArray();
        }
        //计算目标位置
        internal void GetTargetXYZPrintPos(int indexPrintPos, double xTarget0, double yTarget0, out double xTarget, out double yTarget)
        {
            xTarget = xTarget0;
            yTarget = yTarget0;
            switch (indexPrintPos)
            {
                case 0: // 1#打印工位
                    xTarget = xTarget0 + 0;
                    yTarget = yTarget0 + 0;
                    break;
                case 1: // 2#打印工位
                    xTarget = xTarget0 + 0;
                    yTarget = yTarget0 + 0;
                    break;
            }
        }

        private void tmrOP1_MeasureBase_Tick(object sender, EventArgs e)
        {
            try
            {
          

                //获取点，测量组调节一次
                //GV.PrintingObj.GetBaseDetectPercent(out percent, out strNotice);
                int countMeasuresd = GV.PrintingObj.listDisplacementLiDanRecord.Count;
                if (countMeasuresd < measurePoints.Length)
                {
                    listBox1.SelectedIndex = countMeasuresd;
                }
                for (int iPoint = 0; iPoint < countMeasuresd; iPoint++)
                {
                    strLines = GV.PrintingObj.listDisplacementLiDanRecord[iPoint].Split(',');
                    if (strLines != null)
                    {
                        if (strLines.Length == 4)
                        {
                            if (-1 == measurePoints[iPoint].D1) // 得到一个新的测量数据
                            {
                                measurePoints[iPoint].X1 = Convert.ToDouble(strLines[0]);
                                measurePoints[iPoint].Y1 = Convert.ToDouble(strLines[1]);
                                measurePoints[iPoint].Z1 = Convert.ToDouble(strLines[2]);
                                measurePoints[iPoint].D1 = Convert.ToDouble(strLines[3]);
                                listBox1.Items[iPoint] = measurePoints[iPoint].ToListText(iPoint + 1);
                            }
                        }
                    }
                }
                if (countMeasuresd == 5)
                {
                    double iDistance = 0;
                    double iAdjust = GV.PrintingObj.listDistanceRecordValue[0];//测量值为正值应将z向下调节
                    GV.PrintingObj.qAdjustPMmotor(GV.PMC.arrXBotIds[0], 1, GV.Z, iAdjust, "AdjustToPeakValue");//调节到峰值
                    GV.PrintingObj.qWaitMoveEnd();

                    double topLeftA = GV.PrintingObj.listDistanceRecordValue[1];    // 左上角
                    double topRightA = GV.PrintingObj.listDistanceRecordValue[2];   // 右上角  
                    double bottomRightA = GV.PrintingObj.listDistanceRecordValue[3]; // 右下角
                    double bottomLeftA = GV.PrintingObj.listDistanceRecordValue[4];// 左下角

                    double leftAvg = (topLeftA + bottomLeftA) / 2;
                    double rightAvg = (topRightA + bottomRightA) / 2;
                    double RotY = Math.Atan((leftAvg - rightAvg) / GV.OP1_dX);

                    double topAvg = (topLeftA + topRightA) / 2;
                    double bottomAvg = (bottomLeftA + bottomRightA) / 2;
                    double RotX = Math.Atan((topAvg - bottomAvg) / GV.OP1_dY);

                    //GV.adjustRxA = RotX;
                    //GV.adjustRyA = RotY;

                    //显示测量计算值
                    txtRxA.Text = (RotX * GV.PMC.Rad2Degree).ToString("0.000");
                    txtRyA.Text = (RotY * GV.PMC.Rad2Degree).ToString("0.000");

                    //if (Math.Abs(RotX) * 1000 > 0.002)
                    //{
                    //    GV.PrintingObj.qAdjustPMmotor(GV.PMC.arrXBotIds[0], 2, GV.PMC.Rx, RotX, "AdjustRotX");//  
                    //                                                                                          //GV.PrintingObj.MoveXbotXYrotary(GV.PMC.arrXBotIds[0], GV.PMC.Y, RotX, 3);
                    //    GV.PrintingObj.qPause(500);
                    //}
                    //// 调节Y方向旋转（绕Y轴）
                    //if (Math.Abs(RotY) * 1000 > 0.002)
                    //{
                    //    GV.PrintingObj.qAdjustPMmotor(GV.PMC.arrXBotIds[0], 2, GV.PMC.Ry, RotY, "AdjustRotY");
                    //    GV.PrintingObj.qPause(500);
                    //}
                    //测量完成，不再
                    tmrOP1_MeasureBase.Stop();
                }
            }
            catch (Exception ex)
            {

            }
        }
        //对针
        private void btnNozzleCali_Click(object sender, EventArgs e)
        {
            try
            {
                if (tmrOP2_CalibrateNozzle.Enabled)
                {
                    lblNoticeCamOP2.BackColor = Color.Transparent;
                    tmrOP2_CalibrateNozzle.Stop();
                    GV.StopImmediately();
                    // btnNozzleCali.Text = "开始校针";
                    lblNoticeCamOP2.Text = "";
                }
                else
                {
                    tmrOP2_CalibrateNozzle.Start();//进度
                    //小Z轴回零
                    // GV.frmMotionAdjust.btnHomeZ1_Click(sender, e);
                    // GV.frmMotionAdjust.btnHomeZ2_Click(sender, e);

                    double x, y, z, Za, Zb;
                    x = (double)num_OP2_X.Value;
                    y = (double)num_OP2_Y.Value;
                    z = (double)num_OP2_Z.Value;   // 预留5mm安全距离//直接移到对焦点

                    if (DialogResult.OK == MessageBox.Show("确定移动到坐标: (" + x.ToString("0") + ", "
                        + y.ToString("0") + "," + z.ToString("0") + ") 吗？\r\n\r\n"
                        + "如果您之前没有配置过相机对针位置，请点击“取消”按钮。", "请注意", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation))
                    {
                        GV.PrintingObj.qDisplayInfo("Notice", "移动中...", "Red");
                        //lblNoticeCamOP2.Text = "喷头正在移动中..";
                        //lblNoticeCamOP2.BackColor = Color.Red;
                        double vPress = 2; // 下压速度
                        Move2ReadyPos(x, y, z, "NozzleMoving", GV.Z_TOP);//z
                        //GV.printingObj.qMoveAxisRelative(GV.Z, 8, vPress, 0);

                        GV.PrintingObj.qWaitMoveEnd();
                        GV.PrintingObj.qDisplayInfo("Notice", "已到达对焦位置", "Green");
                                       
                        btnNozzleCalibrate.Text = "停止校针";
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        double posNozzle = 10;//测量值
        double sensorThreashold = 2.5;
        //显示进度
        private void tmrOP2_CalibrateNozzle_Tick(object sender, EventArgs e)
        {
            //long tUsed = sw.ElapsedMilliseconds;
            //long tRemain = GV.printingObj.GetLeftTimeZ();
            //int percent = (int)(100.0 * tUsed / (tUsed + tRemain));

            //string notice;
            //string color;
            //GV.PrintingObj.GetNoticeInfo(out notice, out color);
            //if (notice != "NozzleReady")
            //{
            //    lblNoticeCamOP2.Text = notice;
            //    lblNoticeCamOP2.BackColor = Color.Orange;
            //    return;
            //}
            //else if (notice == "NozzleReady")
            {
                //lblNoticeCamOP2.Text = $"测量结果: {distanceBase:F2} um, 需要调整: {distanceBase - targetDistance:F2} um";
                //lblNoticeCamOP2.BackColor = Color.Green;
                //lblNoticeProgress.Text = "当前状态：" + notice;
                //lblNoticeProgress.BackColor = ColorTranslator.FromHtml(color);

                double iTargetZ;
                double iNowZ = GV.PrintingObj.GetFPosition(GV.Z);
                iTargetZ = iNowZ + (distanceBase - targetDistance) / 1000;
                txtTargetZ.Text = iTargetZ.ToString("0.000");//建议下针距离

                //lblNoticeCamOP2.Text = $"测量结果: {distanceBase:F2} um, 需要调整: {distanceBase - targetDistance:F2} um";
                //lblNoticeCamOP2.BackColor = Color.Green;
                string measureResult = $"测量结果: {distanceBase / 1000.0:F4} mm, 需要调整: {(distanceBase - targetDistance) / 1000.0:F4} mm";
                GV.PrintingObj.qDisplayInfo("Notice", measureResult, "TransParent");
                GV.D_OP2 = distanceBase;
                GV.Z_BOTTOM = iTargetZ;


               // tmrOP2_CalibrateNozzle.Stop();
            }
        }

        string strNotice;//提示信息      
        /// <summary>
        /// 图像的存储
        /// </summary>
        /// <param name="objIFrameData">图像信息对象</param>

        private void ImageSave(IFrameData objIFrameData)
        {
            try
            {
                // Create a directory in the debug folder of your project
                string debugFolderPath = Path.Combine(Application.StartupPath, "pic_Save"); // Or any subfolder name you want
                if (!Directory.Exists(debugFolderPath))
                {
                    Directory.CreateDirectory(debugFolderPath);
                }
                DateTime dtNow = System.DateTime.Now;  // 获取系统当前时间
                string strDateTime = dtNow.Year.ToString() + "_"
                                   + dtNow.Month.ToString() + "_"
                                   + dtNow.Day.ToString() + "_"
                                   + dtNow.Hour.ToString() + "_"
                                   + dtNow.Minute.ToString() + "_"
                                   + dtNow.Second.ToString() + "_"
                                   + dtNow.Millisecond.ToString();

                string stfFileName = strDateTime + ".bmp";  // 默认的图像保存名称
                string fileSavePath = Path.Combine(debugFolderPath, stfFileName); // Replace with your actual filename
                m_objGxBitmap.SaveBmp(objIFrameData, fileSavePath);

               // GV.PrintingObj.qDisplayInfo("Notice", "图片处理中...", "Orange");
               //// Color.Orange;
               // GV.PrintingObj.qPause(1000);//预埋sleep
                //保存完后处理
                ProcessImage(fileSavePath);
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// 回调函数,用于获取图像信息和显示图像
        /// </summary>
        /// <param name="obj">用户自定义传入参数</param>
        /// <param name="objIFrameData">图像信息对象</param>
        private void __CaptureCallbackPro(object objUserParam, IFrameData objIFrameData)
        {
            try
            {
                FrmNozzleCalibrate objGxSingleCam = objUserParam as FrmNozzleCalibrate;
                if (objGxSingleCam == null) return;
                try
                {
                    objGxSingleCam.ImageShow(objIFrameData);//显示图像
                }
                catch (Exception ex)
                {
                    // 采集显示异常处理
                    objGxSingleCam?.Invoke((MethodInvoker)delegate
                    {
                        MessageBox.Show("图像显示异常: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    });
                    //重置相机流
                    objGxSingleCam?.m_objIGXStream?.StopGrab();
                    objGxSingleCam?.m_objIGXStream?.StartGrab();
                    return;
                }

                if (singleSnap)
                {
                    objGxSingleCam.ImageSave(objIFrameData);//保存图像
                    singleSnap = false;

                    //GV.MsgShow(lblSave, "保存成功", grabTimer, 1000);
                    //lblSave.Text = "保存成功";

                    //// 最好通过Invoke方式更新UI
                    //objGxSingleCam.Invoke((MethodInvoker)delegate
                    //{
                    //    MessageBox.Show("照片保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //});

                }
            }
            catch (Exception)
            {
                singleSnap = false;
            }
        }

    }
    public class MeasurePoint
    {
        public double X;        // 测量点X
        public double Y;        // 测量点Y
        public double Z;        // 测量点Z

        public double ZUp;      // 测量点下针安全位置ZUp
        public bool MeasurePosA = false;//双喷头双工位
        public bool MeasurePosB = false;
        public int numPrintPos;   // 对应的打印工位索引，单喷头6工位
        public double D1 = -1;   // 测量值A
        public double D2 = -1;   // 测量值B
        public double X1 = -1;        // 实际测量点X1
        public double Y1 = -1;        // 实际测量点Y1
        public double Z1 = -1;        // 实际测量点Z1


        public MeasurePoint(double X, double Y, double Z, double ZUp, bool iPrintPosA = false, bool iPrintPosB = false, int iPrintPos = 0)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
            this.ZUp = ZUp;
            this.MeasurePosA = iPrintPosA;//选择工位
            this.MeasurePosB = iPrintPosB;
            this.numPrintPos = iPrintPos;//工位号
        }

        public string ToListText(int index, double measureValueA = -1, double measureValueB = -1)
        {
            string str = "";
            string strMeasureA = "?";
            string strMeasureB = "?";
            string strHeadA = "A :";
            string strHeadB = "B :";
            if (measureValueA > -1 || measureValueB > -1)
            {
                this.D1 = measureValueA;
                this.D2 = measureValueB;
            }
            if (this.MeasurePosA && !this.MeasurePosB)
            {
                if (D1 > -1) // measureValue <= -1
                {
                    // str = stage + index.ToString("00") + ". P" + (numPrintPos) + " (" + this.X.ToString("000") + "," + this.Y.ToString("000") + "," + this.Z1.ToString("000.0") + "): ";
                    str = "A; (" + this.X.ToString("000") + "," + this.Y.ToString("000") + "," + this.Z1.ToString("000.0") + "): ";
                    str += this.D1.ToString("0.0000");
                }
                else
                {
                    //strA = stage + index.ToString("00") + ". P" + (numPrintPos) + " (" + this.X.ToString("000") + "," + this.Y.ToString("000") + "," + this.Z.ToString("000.0") + "): ";
                    str = "A; (" + this.X.ToString("000") + "," + this.Y.ToString("000") + "," + this.Z.ToString("000.0") + "): ";
                    str += "?";
                }
            }
            else if (!this.MeasurePosA && this.MeasurePosB)
            {
                if (D2 > -1) // measureValue <= -1
                {
                    // str = stage + index.ToString("00") + ". P" + (numPrintPos) + " (" + this.X.ToString("000") + "," + this.Y.ToString("000") + "," + this.Z1.ToString("000.0") + "): ";
                    str = "B; (" + this.X.ToString("000") + "," + this.Y.ToString("000") + "," + this.Z1.ToString("000.0") + "): ";
                    str += this.D2.ToString("0.0000");
                }
                else
                {
                    //strA = stage + index.ToString("00") + ". P" + (numPrintPos) + " (" + this.X.ToString("000") + "," + this.Y.ToString("000") + "," + this.Z.ToString("000.0") + "): ";
                    str = "B; (" + this.X.ToString("000") + "," + this.Y.ToString("000") + "," + this.Z.ToString("000.0") + "): ";
                    str += "?";
                }
            }
            else if (this.MeasurePosA && this.MeasurePosB)
            {
                if (D1 > -1 && D2 > -1) // measureValue <= -1
                {
                    // str = stage + index.ToString("00") + ". P" + (numPrintPos) + " (" + this.X.ToString("000") + "," + this.Y.ToString("000") + "," + this.Z1.ToString("000.0") + "): ";
                    str = "A; (" + this.X.ToString("000") + "," + this.Y.ToString("000") + "," + this.Z1.ToString("000.0") + "): ";
                    str += this.D1.ToString("0.0000");
                    str += " ,B: " + this.D2.ToString("0.0000");
                }
                else
                {
                    //strA = stage + index.ToString("00") + ". P" + (numPrintPos) + " (" + this.X.ToString("000") + "," + this.Y.ToString("000") + "," + this.Z.ToString("000.0") + "): ";
                    str = "A; (" + this.X.ToString("000") + "," + this.Y.ToString("000") + "," + this.Z.ToString("000.0") + "): ";
                    str += "?";
                    str += " ,B: ?"; 
                }
            }                   
            //string str;
            //if (measureValue > -1)
            //{
            //    this.D = measureValue;
            //}
            //if (D > -1) // measureValue <= -1
            //{
            //    str = index.ToString("00") + ". P" + (numPrintPos) + " (" + this.X.ToString("000") + "," + this.Y.ToString("000") + "," + this.Z1.ToString("000.0") + "): ";
            //    str += this.D.ToString("0.0000");
            //}
            //else
            //{
            //    str = index.ToString("00") + ". P" + (numPrintPos) + " (" + this.X.ToString("000") + "," + this.Y.ToString("000") + "," + this.Z.ToString("000.0") + "): ";
            //    str += "?";
            //}
            return str;                    
        }

        public string ToPoint()
        {
            return this.X + "," + this.Y + "," + Math.Abs(this.Z - this.ZUp);
        }

        public string ToData(int index)
        {
            return index + "," + this.numPrintPos + "," + this.X + "," + this.Y + "," + this.Z + "," + this.D1;
        }
    }
}


