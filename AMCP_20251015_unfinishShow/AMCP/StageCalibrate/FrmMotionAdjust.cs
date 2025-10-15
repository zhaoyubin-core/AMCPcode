using OpenCvSharp.Dnn;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace AMCP
{
    public partial class FrmMotionAdjust : Form
    {
        double xSet;  // 设定位置
        double ySet;  // 设定位置
        double zSet;  // 设定位置
        double xFed;  // 当前位置
        double yFed;  // 当前位置
        double zFed;  // 当前位置
        double jogSpeed;    // 点动运动时的速度
        double moveStep;    // 步进运动时的步
        double vSet;  // 设定速度

        double HIGHSPEED = 30;
        double MIDSPEED = 10;
        double LOWSPEED = 1;

        double LONGSTEP = 10;
        double MIDSTEP = 1;
        double SHORTSTEP = 0.1;


        double HOMESPEED = 15;

        string[] paraArr = new string[]{"HIGHSPEED", "MIDSPEED","LOWSPEED","LONGSTEP", "MIDSTEP","SHORTSTEP","HOMESPEED",
                                        "X_MIN", "Y_MIN", "Z_MIN", "X_MAX", "Y_MAX", "Z_MAX", "X_INIT", "Y_INIT", "Z_INIT",
                                        "X_ADJUST", "Y_ADJUST", "Z_ADJUST", "Z_TOP", "Z_BOTTOM"};


        //FrmMain GV;
        public FrmMotionAdjust()
        {
            InitializeComponent();
            ReadAdvanConfigFile();
            moveStep = MIDSTEP;
            jogSpeed = MIDSPEED;
            if (chkStepMove.Checked)
            {
                lblCurrentSpeed.Text = "当前步进距离：" + moveStep.ToString() + "mm";
            }
            else
            {
                lblCurrentSpeed.Text = "当前速度：" + jogSpeed.ToString() + "mm/s";
            }
        }

        private void FrmMotionAdjust_Load(object sender, EventArgs e)
        {
            //GV = this.MdiParent as FrmMain;
        }
        //移动到设定位置
        private void btnToSetPosition_Click(object sender, EventArgs e)
        {
            if (CheckClearCommands() == ClearResult.DonotClear) return;

            try
            {
                // 从文本框读取设定位置数据
                GV.xSet = Convert.ToDouble(txtSetX.Text);
                GV.ySet = Convert.ToDouble(txtSetY.Text);
                GV.zSet = Convert.ToDouble(txtSetZ.Text);
                GV.z1Set = Convert.ToDouble(txtSetZ1.Text);
                GV.z2Set = Convert.ToDouble(txtSetZ2.Text);
                // 从文本框读取设定速度数据
                double vSet = Convert.ToDouble(txtSetVelocity.Text);

                // 获取当前位置数据
                GV.xFed = GV.PrintingObj.Status.fPosX; //Convert.ToDouble(txtFeedbackPos0.Text);
                GV.yFed = GV.PrintingObj.Status.fPosY; //Convert.ToDouble(txtFeedbackPos1.Text);
                GV.zFed = GV.PrintingObj.Status.fPosZ; //Convert.ToDouble(txtFeedbackPos2.Text);
                GV.z1Fed = GV.PrintingObj.Status.fPosZ1;
                GV.z2Fed = GV.PrintingObj.Status.fPosZ2;

                if (chkAxis0.Checked && GV.xSet != GV.xFed)
                {
                    GV.PrintingObj.qMoveAxisTo(GV.X, GV.xSet, vSet, GV.xFed);
                }

                if (chkAxis1.Checked && GV.ySet != GV.yFed)
                {
                    GV.PrintingObj.qMoveAxisTo(GV.Y, GV.ySet, vSet, GV.yFed);
                }

                if (chkAxis2.Checked && GV.zSet != GV.zFed)
                {
                    GV.PrintingObj.qMoveAxisTo(GV.Z, GV.zSet, vSet, GV.zFed);
                }
                if (chkAxis3.Checked && GV.z1Set != GV.z1Fed)
                {
                    GV.PrintingObj.qMoveAxisTo(GV.Z1, GV.z1Set, vSet, GV.z1Fed);
                }
                if (chkAxis4.Checked && GV.z2Set != GV.z2Fed)
                {
                    GV.PrintingObj.qMoveAxisTo(GV.Z2, GV.z2Set, vSet, GV.z2Fed);
                }
            }
            catch (System.Exception ex)
            {
                //PrintingObj.PushMessage(ex.Message);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            GV.PrintingObj.Stop();
        }

        private void StepMove(int axis, int direction, object sender)
        {
            double step;
            double vSet;
            if (chkStepMove.Checked && !GV.PrintingObj.IsMotorMoving(axis))
            {
                vSet = Convert.ToDouble(txtSetVelocity.Text);
                Control ctrl = sender as Control;
                step = Convert.ToDouble(ctrl.Text);
                GV.PrintingObj.qMoveAxisRelative(axis, direction * step, vSet);
            }
        }

        private void StepMoveX_Positive_Click(object sender, EventArgs e)
        {
            StepMove(GV.X, 1, sender);
        }

        private void StepMoveX_Negative_Click(object sender, EventArgs e)
        {
            StepMove(GV.X, -1, sender);
        }

        private void StepMoveY_Positive_Click(object sender, EventArgs e)
        {
            StepMove(GV.Y, 1, sender);
        }

        private void StepMoveY_Negative_Click(object sender, EventArgs e)
        {
            StepMove(GV.Y, -1, sender);
        }

        private void StepMoveZ_Positive_Click(object sender, EventArgs e)
        {
            StepMove(GV.Z, 1, sender);
        }

        private void StepMoveZ_Negative_Click(object sender, EventArgs e)
        {
            StepMove(GV.Z, -1, sender);
        }

        private void JogMove(int axis, int direction, object sender)
        {
            double vSet;
            if (!chkStepMove.Checked && !GV.PrintingObj.IsMotorMoving(axis))
            {
                Control ctrl = sender as Control;
                vSet = Convert.ToDouble(ctrl.Text);
                GV.PrintingObj.qJog(axis, direction * vSet);
            }
        }

        private void JogMove_Stop(int axis, int direction, object sender)
        {
            double step, dx;
            double curPos, stopPos;
            double adjPos;
            double V, A, J, am;
            if (!chkStepMove.Checked)
            {
                Control ctrl = sender as Control;
                V = Convert.ToDouble(ctrl.Text);
                step = V * 0.05;
                curPos = GV.PrintingObj.GetFPosition(axis);
                A = GV.PrintingObj.GetAcceleration(axis);
                J = GV.PrintingObj.GetJerk(axis);
                GV.PrintingObj.Stop(axis);

                return;
                am = Math.Sqrt(J * V);
                if (A < am)
                    dx = 0.5 * V * (A / J + V / A);
                else
                    dx = Math.Pow(V, 1.5) / Math.Sqrt(J);

                if (direction > 0)
                {
                    stopPos = curPos + dx;
                    adjPos = Math.Ceiling(stopPos / step) * step;
                }
                else
                {
                    stopPos = curPos - dx;
                    adjPos = Math.Floor(stopPos / step) * step;
                }
                GV.PrintingObj.qMoveAxisTo(axis, adjPos, V, curPos);
            }
        }

        private void JogMoveX_Positive_MouseDown(object sender, MouseEventArgs e)
        {
            JogMove(GV.X, 1, sender);
        }

        private void JogMoveX_Negative_MouseDown(object sender, MouseEventArgs e)
        {
            JogMove(GV.X, -1, sender);
        }

        private void JogMoveY_Positive_MouseDown(object sender, MouseEventArgs e)
        {
            JogMove(GV.Y, 1, sender);
        }

        private void JogMoveY_Negative_MouseDown(object sender, MouseEventArgs e)
        {
            JogMove(GV.Y, -1, sender);
        }

        private void JogMoveZ_Positive_MouseDown(object sender, MouseEventArgs e)
        {
            JogMove(GV.Z, 1, sender);
        }

        private void JogMoveZ_Negative_MouseDown(object sender, MouseEventArgs e)
        {
            JogMove(GV.Z, -1, sender);
        }

        private void JogMoveX_Positive_MouseUp(object sender, MouseEventArgs e)
        {
            JogMove_Stop(GV.X, 1, sender);
        }

        private void JogMoveX_Negative_MouseUp(object sender, MouseEventArgs e)
        {
            JogMove_Stop(GV.X, -1, sender);
        }

        private void JogMoveY_Positive_MouseUp(object sender, MouseEventArgs e)
        {
            JogMove_Stop(GV.Y, 1, sender);
        }

        private void JogMoveY_Negative_MouseUp(object sender, MouseEventArgs e)
        {
            JogMove_Stop(GV.Y, -1, sender);
        }

        private void JogMoveZ_Positive_MouseUp(object sender, MouseEventArgs e)
        {
            JogMove_Stop(GV.Z, 1, sender);
        }

        private void JogMoveZ_Negative_MouseUp(object sender, MouseEventArgs e)
        {
            JogMove_Stop(GV.Z, -1, sender);
        }

        /// <summary>
        /// 解析该行字符串中有用的信息，以数组形式返回。
        /// </summary>
        /// <param name="strRow"></param>
        /// <returns></returns>
        public string[] getStrValue(string strRow)
        {
            string[] arr = strRow.Split('=', ';', ' ');
            List<string> list = arr.ToList(); // 转为链表

            int j = 0;
            for (int i = 0; i < list.Count;)
            {
                if (list[i].Trim().Length == 0)
                {
                    list.RemoveAt(i);
                }
                else
                    i++;
            }

            string[] newarr = list.ToArray();
            return newarr;
        }

        private void btnConfig_Click(object sender, EventArgs e)
        {
            string strLink1 = Application.StartupPath + "\\Config.ini";
            System.Diagnostics.Process.Start(strLink1);
        }

        private void setMaxZ_Click(object sender, EventArgs e)
        {
            try
            {
                // 检查是否填写正确
                double setZBottom = Convert.ToDouble(txtZBottom.Text);
                double setZaBottom = Convert.ToDouble(txtZaBottom.Text);
                double setZbBottom = Convert.ToDouble(txtZbBottom.Text);
                if (setZBottom < GV.Z_MIN || setZBottom > GV.Z_MAX)
                {
                    MessageBox.Show("请填写 " + GV.Z_MIN.ToString() + " 到 " + GV.Z_MAX + " 之间的数字。");
                    txtZBottom.Focus();
                    return;
                }
                GV.Z_BOTTOM = setZBottom;
                GV.Za_BOTTOM = setZaBottom;
                GV.Zb_BOTTOM = setZbBottom;
                double targetA = GV.Z_BOTTOM + GV.Za_BOTTOM;
                double targetB = GV.Z_BOTTOM + GV.Zb_BOTTOM;
                if (WriteAdvanConfigFile())//写入后不修改
                {
                    setMaxZ.Enabled = false;
                    lblShowTargetZ.Text = $"下针位置= A: {targetA} B: {targetB}";               
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void btnSetAsMaxZ_Click(object sender, EventArgs e)
        {
            //txtZBottom.Text = txtFeedbackPos2.Text;
            txtZBottom.Text = GV.PrintingObj.Status.fPosZ.ToString("0.000");
            txtZaBottom.Text = GV.PrintingObj.Status.fPosZ1.ToString("0.000");
            txtZbBottom.Text = GV.PrintingObj.Status.fPosZ2.ToString("0.000");
            setMaxZ_Click(sender, e);
        }

        public void SetTxtZBottom(string str)
        {
            txtZBottom.Text = str;
        }

        private void FrmMotionAdjust_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = (e.CloseReason == CloseReason.UserClosing);
            this.Hide();
        }

        private void btnHomeX_Click(object sender, EventArgs e)
        {
            if (CheckClearCommands() == ClearResult.DonotClear) return;
            if (GV.PrintingObj.Status.fPosZ > 0)
            {
                if (DialogResult.OK == MessageBox.Show("当前Z轴尚未回零，请务必检查直接进行X轴回零是否存在安全风险？\r\n\r\n若没有问题，请点击确认；若有安全风险，请点击取消。", "安全提示",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2))
                {
                    GV.PrintingObj.Home(GV.X);
                }
            }
            else
            {
                GV.PrintingObj.Home(GV.X);
            }
        }

        private void btnHomeY_Click(object sender, EventArgs e)
        {
            if (CheckClearCommands() == ClearResult.DonotClear) return;
            if (GV.PrintingObj.Status.fPosZ > 0)
            {
                if (DialogResult.OK == MessageBox.Show("当前Z轴尚未回零，请务必检查直接进行Y轴回零是否存在安全风险？\r\n\r\n若没有问题，请点击确认；若有安全风险，请点击取消。", "安全提示",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2))
                {
                    GV.PrintingObj.Home(GV.Y);
                }
            }
            else
            {
                GV.PrintingObj.Home(GV.Y);
            }
        }

        private void btnHomeZ_Click(object sender, EventArgs e)
        {
            if (CheckClearCommands() == ClearResult.DonotClear) return;
            GV.PrintingObj.Home(GV.Z);
        }

        public void btnHomeZ1_Click(object sender, EventArgs e)
        {
            if (CheckClearCommands() == ClearResult.DonotClear) return;
            GV.PrintingObj.Home(GV.Z1);
        }
        public void btnHomeZ2_Click(object sender, EventArgs e)
        {
            if (CheckClearCommands() == ClearResult.DonotClear) return;
            GV.PrintingObj.Home(GV.Z2);
        }

        private void btnHomeXYZ_Click(object sender, EventArgs e)
        {
            if (CheckClearCommands() == ClearResult.DonotClear) return;
            GV.PrintingObj.HomeAll();
        }


        private void DisplayText(string strText, double ResetTime)
        {
            timer1.Stop();
            timer1.Interval = (int)(ResetTime * 1000);
            if (strText != null)
            {
                this.Text = strText;
            }
            timer1.Start();
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                SelectRadio1();
            }
        }


        Color highlightColor = Color.LightGreen;

        public void SelectRadio1()
        {
            string strSTEP;
            if (chkStepMove.Checked) // 步进模式
            {
                moveStep = LONGSTEP;
                strSTEP = moveStep.ToString();
                DisplayText("已切换至 大步进模式(" + moveStep.ToString() + "mm/步)", 10);
                lblCurrentSpeed.Text = "当前步进距离：" + moveStep.ToString() + "mm";
            }
            else // 点动模式
            {
                jogSpeed = HIGHSPEED;
                txtSetVelocity.Text = jogSpeed.ToString();
                strSTEP = "";//不显示
                DisplayText("已切换至 高速点动模式(" + jogSpeed.ToString() + "mm/s)", 10);
                highlightColor = Color.LightGreen;
                lblCurrentSpeed.Text = "当前速度：" + jogSpeed.ToString() + "mm/s";
            }

            btnMoveRight.Text = (strSTEP + " →").Trim();
            btnMoveLeft.Text = ("← " + strSTEP).Trim();
            btnMoveIn.Text = (strSTEP + " ↗").Trim();
            btnMoveOut.Text = ("↙ " + strSTEP).Trim();
            btnMoveUp.Text = ("↑ " + strSTEP).Trim();
            btnMoveDown.Text = ("↓ " + strSTEP).Trim();
            //btnMoveUpZ1.Text = ("↑ " + strSTEP).Trim();
            //btnMoveDownZ1.Text = ("↓ " + strSTEP).Trim();
            //btnMoveUpZ2.Text = ("↑ " + strSTEP).Trim();
            //btnMoveDownZ2.Text = ("↓ " + strSTEP).Trim();

            radioButton1.BackColor = highlightColor;
            radioButton2.BackColor = Color.White;
            radioButton3.BackColor = Color.White;

            radioButton1.Checked = true;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                SelectRadio2();
            }
        }

        public void SelectRadio2()
        {
            string strSTEP;
            if (chkStepMove.Checked) // 步进模式
            {
                moveStep = MIDSTEP;
                strSTEP = moveStep.ToString();
                DisplayText("已切换至 中步进模式(" + moveStep.ToString() + "mm/步)", 10);
                lblCurrentSpeed.Text = "当前步进距离：" + moveStep.ToString() + "mm";
            }
            else // 点动模式
            {
                jogSpeed = MIDSPEED;
                txtSetVelocity.Text = jogSpeed.ToString();
                strSTEP = "";
                DisplayText("已切换至 中速点动模式(" + jogSpeed.ToString() + "mm/s)", 10);
                lblCurrentSpeed.Text = "当前速度：" + jogSpeed.ToString() + "mm/s";
            }

            btnMoveRight.Text = (strSTEP + " →").Trim();
            btnMoveLeft.Text = ("← " + strSTEP).Trim();
            btnMoveIn.Text = (strSTEP + " ↗").Trim();
            btnMoveOut.Text = ("↙ " + strSTEP).Trim();
            btnMoveUp.Text = ("↑ " + strSTEP).Trim();
            btnMoveDown.Text = ("↓ " + strSTEP).Trim();
            //btnMoveUpZ1.Text = ("↑ " + strSTEP).Trim();
            //btnMoveDownZ1.Text = ("↓ " + strSTEP).Trim();
            //btnMoveUpZ2.Text = ("↑ " + strSTEP).Trim();
            //btnMoveDownZ2.Text = ("↓ " + strSTEP).Trim();

            radioButton1.BackColor = Color.White;
            radioButton2.BackColor = highlightColor;
            radioButton3.BackColor = Color.White;

            radioButton2.Checked = true;
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked)
            {
                SelectRadio3();
            }
        }

        public void SelectRadio3()
        {
            string strSTEP;
            if (chkStepMove.Checked) // 步进模式
            {
                moveStep = SHORTSTEP;
                strSTEP = moveStep.ToString();
                DisplayText("已切换至 小步进模式(" + moveStep.ToString() + "mm/步)", 10);
                lblCurrentSpeed.Text = "当前步进距离：" + moveStep.ToString() + "mm";
            }
            else // 点动模式
            {
                jogSpeed = LOWSPEED;
                txtSetVelocity.Text = jogSpeed.ToString();
                strSTEP = "";
                DisplayText("已切换至 慢速点动模式(" + jogSpeed.ToString() + "mm/s)", 10);
                lblCurrentSpeed.Text = "当前速度：" + jogSpeed.ToString() + "mm/s";
            }
            btnMoveRight.Text = (strSTEP + " →").Trim();
            btnMoveLeft.Text = ("← " + strSTEP).Trim();
            btnMoveIn.Text = (strSTEP + " ↗").Trim();
            btnMoveOut.Text = ("↙ " + strSTEP).Trim();
            btnMoveUp.Text = ("↑ " + strSTEP).Trim();
            btnMoveDown.Text = ("↓ " + strSTEP).Trim();
            //btnMoveUpZ1.Text = ("↑ " + strSTEP).Trim();
            //btnMoveDownZ1.Text = ("↓ " + strSTEP).Trim();
            //btnMoveUpZ2.Text = ("↑ " + strSTEP).Trim();
            //btnMoveDownZ2.Text = ("↓ " + strSTEP).Trim();

            radioButton1.BackColor = Color.White;
            radioButton2.BackColor = Color.White;
            radioButton3.BackColor = highlightColor;

            radioButton3.Checked = true;
        }

        private void btnMoveIn_Click(object sender, EventArgs e)
        {
            chkKeyControl.Checked = false;
            StepMove(GV.Y, -1);
        }

        private void btnMoveOut_Click(object sender, EventArgs e)
        {
            chkKeyControl.Checked = false;
            StepMove(GV.Y, 1);
        }

        private void btnMoveLeft_Click(object sender, EventArgs e)
        {
            chkKeyControl.Checked = false;
            StepMove(GV.X, -1);
        }

        private void btnMoveRight_Click(object sender, EventArgs e)
        {
            chkKeyControl.Checked = false;
            StepMove(GV.X, 1);
        }

        private void btnMoveUp_Click(object sender, EventArgs e)
        {
            chkKeyControl.Checked = false;
            StepMove(GV.Z, -1);
        }

        private void btnMoveDown_Click(object sender, EventArgs e)
        {
            chkKeyControl.Checked = false;
            StepMove(GV.Z, 1);
        }

        private void chkKeyControl_CheckedChanged(object sender, EventArgs e)
        {
            if (chkKeyControl.Checked)
            {
                lblHotkeyTip1.Visible = true;
                lblHotkeyTip2.Visible = true;
                lblHotkeyTip3.Visible = true;
                lblHotkeyTip4.Visible = true;
                lblHotkeyTip5.Visible = true;
                lblHotkeyTip6.Visible = true;
                DisplayText("已解除键盘锁定，请尽快操作。", 30);
            }
            else
            {
                lblHotkeyTip1.Visible = false;
                lblHotkeyTip2.Visible = false;
                lblHotkeyTip3.Visible = false;
                lblHotkeyTip4.Visible = false;
                lblHotkeyTip5.Visible = false;
                lblHotkeyTip6.Visible = false;
                bAllowKeyCtrl1 = false;
                chkKeyControl.Checked = false;
                this.Text = "运动调校";
            }
        }

        private void txtZBottom_TextChanged(object sender, EventArgs e)
        {
            setMaxZ.Enabled = true;
        }

        private void chkStepMove_CheckedChanged(object sender, EventArgs e)
        {
            //chkKeyControl.Checked = false;
            if (chkStepMove.Checked) // 步进模式
            {
                highlightColor = Color.Yellow;
                radioButton1.Text = "大步";
                radioButton2.Text = "中步";
                radioButton3.Text = "小步";
                if (moveStep == LONGSTEP)
                {
                    //radioButton1.Checked = true;
                    SelectRadio1();
                    DisplayText("已切换至 大步进模式(" + moveStep.ToString() + "mm/步)", 10);
                }
                else if (moveStep == MIDSTEP)
                {
                    //radioButton2.Checked = true;
                    SelectRadio2();
                    DisplayText("已切换至 中步进模式(" + moveStep.ToString() + "mm/步)", 10);
                }
                else if (moveStep == SHORTSTEP)
                {
                    //radioButton3.Checked = true;
                    SelectRadio3();
                    DisplayText("已切换至 小步进模式(" + moveStep.ToString() + "mm/步)", 10);
                }
            }
            else // 点动模式
            {
                highlightColor = Color.LightGreen;
                radioButton1.Text = "高速";
                radioButton2.Text = "中速";
                radioButton3.Text = "低速";
                if (jogSpeed == HIGHSPEED)
                {
                    //radioButton1.Checked = true;
                    SelectRadio1();
                    DisplayText("已切换至 高速点动模式(" + jogSpeed.ToString() + "mm/s)", 10);
                }
                else if (jogSpeed == MIDSPEED)
                {
                    //radioButton2.Checked = true;
                    SelectRadio2();
                    DisplayText("已切换至 中速点动模式(" + jogSpeed.ToString() + "mm/s)", 10);
                }
                else if (jogSpeed == LOWSPEED)
                {
                    //radioButton3.Checked = true;
                    SelectRadio3();
                    DisplayText("已切换至 慢速点动模式(" + jogSpeed.ToString() + "mm/s)", 10);
                }
            }
        }

        // X负方向点动/停止：

        private void btnMoveLeft_MouseDown(object sender, MouseEventArgs e)
        {
            JogMove(GV.X, -1);
        }

        private void btnMoveLeft_MouseUp(object sender, MouseEventArgs e)
        {
            JogMove_Stop(GV.X, -1);
        }

        // X正方向点动/停止：

        private void btnMoveRight_MouseDown(object sender, MouseEventArgs e)
        {
            JogMove(GV.X, 1);
        }

        private void btnMoveRight_MouseUp(object sender, MouseEventArgs e)
        {
            JogMove_Stop(GV.X, 1);
        }

        // Y负方向点动/停止：

        private void btnMoveIn_MouseDown(object sender, MouseEventArgs e)
        {
            JogMove(GV.Y, -1);
        }

        private void btnMoveIn_MouseUp(object sender, MouseEventArgs e)
        {
            JogMove_Stop(GV.Y, -1);
        }

        // Y正方向点动/停止：

        private void btnMoveOut_MouseDown(object sender, MouseEventArgs e)
        {
            JogMove(GV.Y, 1);
        }

        private void btnMoveOut_MouseUp(object sender, MouseEventArgs e)
        {
            JogMove_Stop(GV.Y, 1);
        }

        // Z负方向点动/停止：

        private void btnMoveUp_MouseDown(object sender, MouseEventArgs e)
        {
            JogMove(GV.Z, -1);
        }

        private void btnMoveUp_MouseUp(object sender, MouseEventArgs e)
        {
            JogMove_Stop(GV.Z, -1);
        }

        // Z正方向点动/停止：

        private void btnMoveDown_MouseDown(object sender, MouseEventArgs e)
        {
            JogMove(GV.Z, 1);
        }

        private void btnMoveDown_MouseUp(object sender, MouseEventArgs e)
        {
            JogMove_Stop(GV.Z, 1);
        }

        bool JogMovingX = false, JogMovingY = false, JogMovingZ = false;
        internal bool JogMovingZ1 = false, JogMovingZ2 = false;

        //小Z轴运动
        private void btnMoveUpZ1_Click(object sender, EventArgs e)
        {
            chkKeyControl.Checked = false;
            StepMove(GV.Z1, -1);
        }
        private void btnMoveUpZ1_MouseDown(object sender, MouseEventArgs e)
        {
            JogMove(GV.Z1, -1);
        }
        private void btnMoveUpZ1_MouseUp(object sender, MouseEventArgs e)
        {
            JogMove_Stop(GV.Z1, -1);
        }

        private void btnMoveDownZ1_Click(object sender, EventArgs e)
        {
            chkKeyControl.Checked = false;
            StepMove(GV.Z1, 1);
        }
        private void btnMoveDownZ1_MouseDown(object sender, MouseEventArgs e)
        {
            JogMove(GV.Z1, 1);
        }
        private void btnMoveDownZ1_MouseUp(object sender, MouseEventArgs e)
        {
            JogMove_Stop(GV.Z1, 1);
        }
        private void btnMoveUpZ2_Click(object sender, EventArgs e)
        {
            chkKeyControl.Checked = false;
            StepMove(GV.Z2, -1);
        }
        private void btnMoveUpZ2_MouseDown(object sender, MouseEventArgs e)
        {
            JogMove(GV.Z2, -1);
        }
        private void btnMoveUpZ2_MouseUp(object sender, MouseEventArgs e)
        {
            JogMove_Stop(GV.Z2, -1);
        }
        private void btnMoveDownZ2_Click(object sender, EventArgs e)
        {
            chkKeyControl.Checked = false;
            StepMove(GV.Z2, 1);
        }
        private void btnMoveDownZ2_MouseDown(object sender, MouseEventArgs e)
        {
            JogMove(GV.Z2, 1);
        }
        private void btnMoveDownZ2_MouseUp(object sender, MouseEventArgs e)
        {
            JogMove_Stop(GV.Z2, 1);
        }
        private void JogMove(int axis, int direction)
        {
            if (CheckClearCommands() != ClearResult.Needless) return;
            if (!chkStepMove.Checked && !GV.PrintingObj.IsMotorMoving(axis))
            {
                if (axis == GV.X || axis == GV.Y || axis == GV.Z)
                {
                    vSet = jogSpeed;
                }
                else
                {
                    vSet = 1;
                }
                   // vSet = jogSpeed;

                //PrintingObj.qJog(axis, direction * vSet);
                GV.PrintingObj.Jog(axis, direction * vSet);
                switch (axis)
                {
                    case GV.X:
                        JogMovingX = true;
                        break;
                    case GV.Y:
                        JogMovingY = true;
                        break;
                    case GV.Z:
                        JogMovingZ = true;
                        break;
                    case GV.Z1:
                        JogMovingZ1 = true;
                        break;
                    case GV.Z2:
                        JogMovingZ2 = true;
                        break;
                    default:
                        break;
                }
            }
        }

        private void JogMove_Stop(int axis, int direction)
        {
            if (CheckClearCommands() != ClearResult.Needless) return;
            double step, dx;
            double curPos, stopPos;
            double adjPos;
            double V, A, J, am;
            if (!chkStepMove.Checked)
            {

                GV.PrintingObj.Stop(axis);
                switch (axis)
                {
                    case GV.X:
                        JogMovingX = false;
                        break;
                    case GV.Y:
                        JogMovingY = false;
                        break;
                    case GV.Z:
                        JogMovingZ = false;
                        break;
                    case GV.Z1:
                        JogMovingZ1 = false;
                        break;
                    case GV.Z2:
                        JogMovingZ2 = false;
                        break;
                    default:
                        break;
                }
                return;
                V = jogSpeed;
                step = V * 0.05;
                curPos = GV.PrintingObj.GetFPosition(axis);
                A = GV.PrintingObj.GetAcceleration(axis);
                J = GV.PrintingObj.GetJerk(axis);

                am = Math.Sqrt(J * V);
                if (A < am)
                    dx = 0.5 * V * (A / J + V / A);
                else
                    dx = Math.Pow(V, 1.5) / Math.Sqrt(J);

                if (direction > 0)
                {
                    stopPos = curPos + dx;
                    adjPos = Math.Ceiling(stopPos / step) * step;
                }
                else
                {
                    stopPos = curPos - dx;
                    adjPos = Math.Floor(stopPos / step) * step;
                }
                GV.PrintingObj.qMoveAxisTo(axis, adjPos, V, curPos);
            }
        }

        //步进运动
        private void StepMove(int axis, int direction)
        {
            if (CheckClearCommands() != ClearResult.Needless) return;
            if (chkStepMove.Checked && !GV.PrintingObj.IsMotorMoving(axis))
            {
                if (axis == GV.X || axis == GV.Y || axis == GV.Z)
                {
                    vSet = moveStep * 2;// Convert.ToDouble(txtSetVelocity.Text);
                    GV.PrintingObj.qMoveAxisRelative(axis, direction * moveStep, vSet);
                }
                else//控制小Z轴的速度
                {
                    vSet = 3;// Convert.ToDouble(txtSetVelocity.Text);
                    GV.PrintingObj.qMoveAxisRelative(axis, direction * moveStep, vSet);
                }           
            }
        }

        bool bAllowKeyCtrl1 = false;
        //bool bAllowKeyCtrl2 = false;

        /// <summary>
        /// 重写ProcessDialogKey，来允许监听方向键
        /// </summary>
        /// <param name="keycode"></param>
        /// <returns></returns>
        //protected override bool ProcessDialogKey(Keys keycode)
        //{
        //    //switch (keycode)
        //    //{
        //    //    case Keys.Left:
        //    //    case Keys.Up:
        //    //    case Keys.Right:
        //    //    case Keys.Down:
        //    //    case Keys.Tab:

        //    return false;
        //    //}
        //    //return true;
        //}

        public void FrmMotionAdjust_KeyDown(object sender, KeyEventArgs e)
        {
            if (GV.PrintingObj.IsPrinting)
            {
                return;
            }
            //this.Text = e.KeyValue.ToString() + "  " + e.KeyData.ToString();
            switch (e.KeyCode)
            {
                case Keys.F1:
                    radioButton1.Checked = true;
                    break;
                case Keys.F2:
                    radioButton2.Checked = true;
                    break;
                case Keys.F3:
                    radioButton3.Checked = true;
                    break;
                case Keys.F4:
                    chkStepMove.Checked = !chkStepMove.Checked;
                    break;
                case Keys.F5:
                    GV.PrintingObj.qExtrude(0, 1);
                    break;
                case Keys.F6:
                    GV.PrintingObj.qExtrude(0, 0);
                    break;
                case Keys.K:
                    if (e.Control)
                    {
                        chkKeyControl.Checked = !chkKeyControl.Checked;
                        e.SuppressKeyPress = true;
                    }
                    break;
            }

            // 小键盘控制：
            if (e.Control || chkKeyControl.Checked)
            {
                e.SuppressKeyPress = true;
                switch (e.KeyValue)
                {
                    case 111: // F1
                        SelectRadio1();
                        break;
                    case 106: // F2
                        SelectRadio2();
                        break;
                    case 8:   // F3
                        SelectRadio3();
                        break;
                    case 105:
                        chkStepMove.Checked = !chkStepMove.Checked;
                        break;
                    case 100: // Left
                              //case 37:
                        if (chkStepMove.Checked)
                            StepMove(GV.X, -1);
                        else
                            JogMove(GV.X, -1);
                        break;
                    case 102: // Right
                              //case 39:
                        if (chkStepMove.Checked)
                            StepMove(GV.X, 1);
                        else
                            JogMove(GV.X, 1);
                        break;
                    case 104: // Up
                              //case 38:
                        if (chkStepMove.Checked)
                            StepMove(GV.Y, -1);
                        else
                            JogMove(GV.Y, -1);
                        break;
                    case 98:  // Down
                              //case 40:
                        if (chkStepMove.Checked)
                            StepMove(GV.Y, 1);
                        else
                            JogMove(GV.Y, 1);
                        break;
                    case 109: // -: Nozzle Up
                              //case 33:
                        if (chkStepMove.Checked)
                            StepMove(GV.Z, -1);
                        else
                            JogMove(GV.Z, -1);
                        break;
                    case 107:  // +: Nozzle Down
                               //case 34:
                        if (chkStepMove.Checked)
                            StepMove(GV.Z, 1);
                        else
                            JogMove(GV.Z, 1);
                        break;

                        //case 97:
                        //case 99:
                        //case 101:
                        //case 105:
                }
            }
            else // 小键盘未被激活
            {
                switch (e.KeyValue)
                {
                    //case 97:
                    case 98:
                    //case 99:
                    case 100:
                    //case 101:
                    case 102:
                    //case 103:
                    case 104:
                    case 105:
                    case 106:
                    case 107:
                    //case 108:
                    case 109:
                    case 111:
                        //case 37:
                        //case 38:
                        //case 39:
                        //case 40:
                        //case 33:
                        //case 34:
                        this.Text = "！如需使用小键盘控制运动，请先依次按7、5键解除锁定！";
                        //GV.PrintingObj.SetAlarmPort(3, 1);
                        //System.Threading.Thread.Sleep(50);
                        //GV.PrintingObj.SetAlarmPort(3, 0);
                        //e.SuppressKeyPress = true;
                        if (bAllowKeyCtrl1 && !chkKeyControl.Checked)
                        {
                            bAllowKeyCtrl1 = false;
                            timer1.Stop();
                        }
                        return;
                    case 103:   // 7: Lock off - Step 1
                        bAllowKeyCtrl1 = true;
                        DisplayText("已按小键盘7，再按小键盘5解除锁定", 0.5);
                        timer1.Start();
                        //e.SuppressKeyPress = true;
                        break;
                    case 101:   // 5: Lock off - Step 2
                        if (bAllowKeyCtrl1)
                        {
                            chkKeyControl.Checked = true;
                            DisplayText("已解除键盘锁定，请尽快操作。", 30);
                            e.SuppressKeyPress = true;
                        }
                        break;
                    default:
                        if (bAllowKeyCtrl1 && !chkKeyControl.Checked)
                        {
                            bAllowKeyCtrl1 = false;
                            DisplayText("依次按7、5键解除锁定。", 5);
                        }
                        break;
                }
            }

            if (e.Control) // 如果按下Control键，则检测运动控制指令
            {
                if (!chkStepMove.Checked) // 点动模式
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Left:
                            JogMove(GV.X, -1);
                            e.SuppressKeyPress = true;
                            break;
                        case Keys.Right:
                            if (!e.Control) break;
                            JogMove(GV.X, 1);
                            e.SuppressKeyPress = true;
                            break;
                        case Keys.Down:
                            if (!e.Control) break;
                            JogMove(GV.Y, 1);
                            e.SuppressKeyPress = true;
                            break;
                        case Keys.Up:
                            if (!e.Control) break;
                            JogMove(GV.Y, -1);
                            e.SuppressKeyPress = true;
                            break;
                        case Keys.PageUp:
                            if (!e.Control) break;
                            JogMove(GV.Z, -1);
                            e.SuppressKeyPress = true;
                            break;
                        case Keys.PageDown:
                            if (!e.Control) break;
                            JogMove(GV.Z, 1);
                            e.SuppressKeyPress = true;
                            break;
                    }
                }
                else // 步进模式
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Left:
                            StepMove(GV.X, -1);
                            e.SuppressKeyPress = true;
                            break;
                        case Keys.Right:
                            StepMove(GV.X, 1);
                            e.SuppressKeyPress = true;
                            break;
                        case Keys.Down:
                            StepMove(GV.Y, 1);
                            e.SuppressKeyPress = true;
                            break;
                        case Keys.Up:
                            StepMove(GV.Y, -1);
                            e.SuppressKeyPress = true;
                            break;
                        case Keys.PageUp:
                            StepMove(GV.Z, -1);
                            e.SuppressKeyPress = true;
                            break;
                        case Keys.PageDown:
                            StepMove(GV.Z, 1);
                            e.SuppressKeyPress = true;
                            break;
                    }
                }
            }

            if (!chkKeyControl.Checked) return;

            if (!chkStepMove.Checked) // 点动模式
            {
                switch (e.KeyCode)
                {
                    case Keys.A:
                        JogMove(GV.X, -1);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.D:
                        JogMove(GV.X, 1);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.S:
                        JogMove(GV.Y, 1);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.W:
                        JogMove(GV.Y, -1);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.R:
                        JogMove(GV.Z, -1);
                        e.SuppressKeyPress = true;
                        break;
                    case Keys.F:
                        JogMove(GV.Z, 1);
                        e.SuppressKeyPress = true;
                        break;
                }
            }
            else // 步进模式
            {
                {
                    switch (e.KeyCode)
                    {
                        case Keys.A:
                            StepMove(GV.X, -1);
                            e.SuppressKeyPress = true;
                            break;
                        case Keys.D:
                            StepMove(GV.X, 1);
                            e.SuppressKeyPress = true;
                            break;
                        case Keys.S:
                            StepMove(GV.Y, 1);
                            e.SuppressKeyPress = true;
                            break;
                        case Keys.W:
                            StepMove(GV.Y, -1);
                            e.SuppressKeyPress = true;
                            break;
                        case Keys.R:
                            StepMove(GV.Z, -1);
                            e.SuppressKeyPress = true;
                            break;
                        case Keys.F:
                            StepMove(GV.Z, 1);
                            e.SuppressKeyPress = true;
                            break;
                    }
                }
            }
        }

        public void FrmMotionAdjust_KeyUp(object sender, KeyEventArgs e)
        {
            //this.Text = e.KeyValue.ToString() + "  " + e.KeyData.ToString();
            if (!chkStepMove.Checked && !GV.PrintingObj.IsPrinting) // 点动模式
            {
                switch (e.KeyCode)
                {
                    case Keys.Left:
                    case Keys.A:
                        JogMove_Stop(GV.X, -1);
                        break;
                    case Keys.Right:
                    case Keys.D:
                        JogMove_Stop(GV.X, 1);
                        break;
                    case Keys.Down:
                    case Keys.S:
                        JogMove_Stop(GV.Y, 1);
                        break;
                    case Keys.Up:
                    case Keys.W:
                        JogMove_Stop(GV.Y, -1);
                        break;
                    case Keys.PageUp:
                    case Keys.R:
                        JogMove_Stop(GV.Z, -1);
                        break;
                    case Keys.PageDown:
                    case Keys.F:
                        JogMove_Stop(GV.Z, 1);
                        break;
                    default:
                        //JogMove_Stop(GV.X, -1);
                        //JogMove_Stop(GV.Y, -1);
                        //JogMove_Stop(GV.Z, -1);
                        break;
                }
                switch (e.KeyValue)
                {
                    case 100: // Left
                              //case 37:  // Left
                        JogMove_Stop(GV.X, -1);
                        e.SuppressKeyPress = true;
                        DisplayText(null, 10);
                        break;
                    case 102: // Right
                              //case 39:  // Right
                        JogMove_Stop(GV.X, 1);
                        e.SuppressKeyPress = true;
                        DisplayText(null, 10);
                        break;
                    case 104: // Up: Nozzle In
                              //case 38:  // Up
                        JogMove_Stop(GV.Y, -1);
                        e.SuppressKeyPress = true;
                        DisplayText(null, 10);
                        break;
                    case 98:  // Down: Nozzle Out
                              //case 40:  // Down
                        JogMove_Stop(GV.Y, 1);
                        e.SuppressKeyPress = true;
                        DisplayText(null, 10);
                        break;
                    case 109: // -: Nozzle Up
                              //case 33:  // PgUp
                        JogMove_Stop(GV.Z, -1);
                        e.SuppressKeyPress = true;
                        DisplayText(null, 10);
                        break;
                    case 107:  // +: Nozzle Down
                               //case 34:   // PgDn
                        JogMove_Stop(GV.Z, 1);
                        e.SuppressKeyPress = true;
                        DisplayText(null, 10);
                        break;
                }
            }
        }

        private void FrmMotionAdjust_Leave(object sender, EventArgs e)
        {
            if (JogMovingX)
            {
                GV.PrintingObj.Stop(GV.X);
            }
            if (JogMovingY)
            {
                GV.PrintingObj.Stop(GV.Y);
            }
            if (JogMovingZ)
            {
                GV.PrintingObj.Stop(GV.Z);
            }
            if (JogMovingZ1 || JogMovingZ2)
            {
                GV.PrintingObj.Stop(GV.Z1);
                GV.PrintingObj.Stop(GV.Z2);
            }
        }

        private void btnReadConfig_Click(object sender, EventArgs e)
        {
            ReadAdvanConfigFile();
            GV.frmNozzleCalibrate.ReadOPXYZ();
        }


        string[] controlArr = new string[] { "txtSetX", "txtSetY", "txtSetZ","txtSetZ1","txtSetZ2","txtSetVelocity" };
        // string[] paraArr = new string[] { "设置X", "设置Y", "设置Z", "设置速度", "Z下针位置"};


        public bool ReadAdvanConfigFile()
        {
            string FileName = Application.StartupPath + "\\Config.ini";
            bool ReadSuccess = false;
            try
            {
                FileStream fs;
                StreamReader sr;
                //m = 0;
                fs = new FileStream(FileName, FileMode.Open, FileAccess.Read);
                sr = new StreamReader(fs, Encoding.Unicode);
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                string str;
                string[] item;
                string paraName;
                string paraText;
                double paraValue;
                Control[] ctrls;

                while ((str = sr.ReadLine()) != null)
                {
                    item = getStrValue(str);
                    if (item.Length < 2)	//说明性文本行，无有效数据。
                        continue;
                    paraName = item[0].Trim();
                    paraText = item[1].Trim();
                    //paraValue = Convert.ToDouble(paraText);
                    //解析对应的参数值：
                    switch (paraName)
                    {
                        case "HIGHSPEED":
                            HIGHSPEED = Convert.ToDouble(paraText);
                            break;
                        case "MIDSPEED":
                            MIDSPEED = Convert.ToDouble(paraText);
                            break;
                        case "LOWSPEED":
                            LOWSPEED = Convert.ToDouble(paraText);
                            break;
                        case "LONGSTEP":
                            LONGSTEP = Convert.ToDouble(paraText);
                            break;
                        case "MIDSTEP":
                            MIDSTEP = Convert.ToDouble(paraText);
                            break;
                        case "SHORTSTEP":
                            SHORTSTEP = Convert.ToDouble(paraText);
                            break;
                        case "HOMESPEED":
                            HOMESPEED = Convert.ToDouble(paraText);
                            break;
                        case "X_MIN":
                            GV.X_MIN = Convert.ToDouble(paraText);
                            break;
                        case "Y_MIN":
                            GV.Y_MIN = Convert.ToDouble(paraText);
                            break;
                        case "Z_MIN":
                            GV.Z_MIN = Convert.ToDouble(paraText);
                            break;
                        case "X_MAX":
                            GV.X_MAX = Convert.ToDouble(paraText);
                            break;
                        case "Y_MAX":
                            GV.Y_MAX = Convert.ToDouble(paraText);
                            break;
                        case "Z_MAX":
                            GV.Z_MAX = Convert.ToDouble(paraText);
                            break;
                        case "X_INIT":
                            GV.X_INIT = Convert.ToDouble(paraText);
                            break;
                        case "Y_INIT":
                            GV.Y_INIT = Convert.ToDouble(paraText);
                            break;
                        case "Z_INIT":
                            GV.Z_INIT = Convert.ToDouble(paraText);
                            break;
                        case "Z_INTERVAL":
                            GV.Z_INTERVAL = Convert.ToDouble(paraText);
                            break;
                        case "X_ADJUST":
                            GV.X_ADJUST = Convert.ToDouble(paraText);
                            break;
                        case "Y_ADJUST":
                            GV.Y_ADJUST = Convert.ToDouble(paraText);
                            break;
                        case "Z_ADJUST":
                            GV.Z_ADJUST = Convert.ToDouble(paraText);
                            break;
                        case "X_ADJUST_B":
                            GV.X_ADJUST_B = Convert.ToDouble(paraText);
                            break;
                        case "Y_ADJUST_B":
                            GV.Y_ADJUST_B = Convert.ToDouble(paraText);
                            break;
                        case "Z_ADJUST_B":
                            GV.Z_ADJUST_B = Convert.ToDouble(paraText);
                            break;
                        case "Z_TOP":
                            GV.Z_TOP = Convert.ToDouble(paraText);
                            break;
                        case "Z_BOTTOM":
                            GV.Z_BOTTOM = Convert.ToDouble(paraText);
                            txtZBottom.Text = GV.Z_BOTTOM.ToString();
                            break;
                            //对针调节参数
                        case "OP1_X":
                            GV.OP1_X = Convert.ToDouble(paraText);
                            break;
                        case "OP1_Y":
                            GV.OP1_Y = Convert.ToDouble(paraText);
                            break;
                        case "OP1_Z":
                            GV.OP1_Z = Convert.ToDouble(paraText);
                            break;
                        case "OP1_dX":
                            GV.OP1_dX = Convert.ToDouble(paraText);
                            break;
                        case "OP1_dY":
                            GV.OP1_dY = Convert.ToDouble(paraText);
                            break;
                        case "OP2_X":
                            GV.OP2_X = Convert.ToDouble(paraText);
                            break;
                        case "OP2_Y":
                            GV.OP2_Y = Convert.ToDouble(paraText);
                            break;
                        case "OP2_Z":
                            GV.OP2_Z = Convert.ToDouble(paraText);
                            break;
                        case "D_INIT":
                            GV.D_INIT = Convert.ToDouble(paraText);
                            break;
                        //新增
                        case "Za_BOTTOM":
                            GV.Za_BOTTOM = Convert.ToDouble(paraText);
                            txtZaBottom.Text = GV.Za_BOTTOM.ToString();
                            break;
                        case "Zb_BOTTOM":
                            GV.Zb_BOTTOM = Convert.ToDouble(paraText);
                            txtZbBottom.Text = GV.Zb_BOTTOM.ToString();
                            break;
                        case "Z_OFFSET":
                            GV.Z_OFFSET = Convert.ToDouble(paraText);
                            break;
                        case "dX_Camera":
                            GV.dX_Camera = Convert.ToDouble(paraText);
                            break;
                        case "dY_Camera":
                            GV.dY_Camera = Convert.ToDouble(paraText);
                            break;
                        case "dZ_Camera":
                            GV.dZ_Camera = Convert.ToDouble(paraText);
                            break;
                        case "dX_Clean":
                            GV.dX_Clean = Convert.ToDouble(paraText);
                            break;
                        case "dY_Clean":
                            GV.dY_Clean = Convert.ToDouble(paraText);
                            break;
                        case "dZ_Clean":
                            GV.dZ_Clean = Convert.ToDouble(paraText);
                            break;
                        case "dX_Trans":
                            GV.dX_Trans = Convert.ToDouble(paraText);
                            break;
                        case "dY_Trans":
                            GV.dY_Trans = Convert.ToDouble(paraText);
                            break;
                        case "dZ_Trans":
                            GV.dZ_Trans = Convert.ToDouble(paraText);
                            break;
                        case "Command_Num":
                            GV.Command_Num = Convert.ToInt32(paraText);
                            break;
                        case "Command_Block":
                            GV.Command_Block = Convert.ToInt32(paraText);
                            break;
                        case "ConRecord_interval":
                            GV.ConRecord_interval = Convert.ToInt32(paraText);
                            break;
                        case "Monitor_interval":
                            GV.Monitor_interval = Convert.ToInt32(paraText);
                            break;
                        case "MonRecord_interval":
                            GV.MonRecord_interval = Convert.ToInt32(paraText);
                            break;
                        default:
                            //解析对应的参数值并赋值到相应控件：
                            for (int i = 0; i < controlArr.Length; i++)
                            {
                                if (paraName == controlArr[i])
                                {
                                    ctrls = this.Controls.Find(controlArr[i], true);
                                    ctrls[0].Text = paraText;
                                }
                            }
                            break;
                    }
                }
                GV.dX_AB = GV.X_ADJUST_B - GV.X_ADJUST;
                GV.dY_AB = GV.Y_ADJUST_B - GV.Y_ADJUST;
                GV.dZ_AB = GV.Z_ADJUST_B - GV.Z_ADJUST;
                lblShowTargetZ.Text = $"下针位置= A: {GV.Zb_BOTTOM} B: {GV.Zb_BOTTOM}";
                sr.Close();
                fs.Close();
                ReadSuccess = true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                ReadSuccess = false;
            }
            return ReadSuccess;
        }

        public bool WriteAdvanConfigFile()
        {
            string FileName = Application.StartupPath + "\\Config.ini";
            bool WriteSuccess = false;
            try
            {
                Control[] ctrls;
                FileStream fs;
                StreamWriter sw;

                if (File.Exists(FileName))
                {
                    fs = new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write);
                    sw = new StreamWriter(fs, Encoding.Unicode); //Encoding.Default

                    string[] paraNameArr = new string[] { "HIGHSPEED", "MIDSPEED", "LOWSPEED", "LONGSTEP", "MIDSTEP", "SHORTSTEP", "HOMESPEED", "X_MIN", "Y_MIN", "Z_MIN",
                                                        "X_MAX", "Y_MAX","Z_MAX","X_INIT", "Y_INIT","Z_INIT", "Z_INTERVAL", "X_ADJUST","Y_ADJUST","Z_ADJUST","X_ADJUST_B","Y_ADJUST_B",
                                                         "OP1_X",  "OP1_Y",  "OP1_Z","OP1_dX", "OP1_dY" , "OP2_X",  "OP2_Y",  "OP2_Z","D_INIT",
                                                        "Z_ADJUST_B",  "Z_BOTTOM", "Za_BOTTOM", "Zb_BOTTOM","Z_TOP", "Z_OFFSET", "dX_Camera", "dY_Camera", "dZ_Camera", "dX_Clean", "dY_Clean", "dZ_Clean"};
                    string paraName;
                    double paraValue;
                    for (int i = 0; i < paraNameArr.Length; i++)
                    {
                        paraName = paraNameArr[i];
                        switch (paraName)
                        {
                            case "HIGHSPEED":
                                paraValue = HIGHSPEED;
                                break;
                            case "MIDSPEED":
                                paraValue = MIDSPEED;
                                break;
                            case "LOWSPEED":
                                paraValue = LOWSPEED;
                                break;
                            case "LONGSTEP":
                                paraValue = LONGSTEP;
                                break;
                            case "MIDSTEP":
                                paraValue = MIDSTEP;
                                break;
                            case "SHORTSTEP":
                                paraValue = SHORTSTEP;
                                break;
                            case "HOMESPEED":
                                paraValue = HOMESPEED;
                                break;
                            case "X_MIN":
                                paraValue = GV.X_MIN;
                                break;
                            case "Y_MIN":
                                paraValue = GV.Y_MIN;
                                break;
                            case "Z_MIN":
                                paraValue = GV.Z_MIN;
                                break;
                            case "X_MAX":
                                paraValue = GV.X_MAX;
                                break;
                            case "Y_MAX":
                                paraValue = GV.Y_MAX;
                                break;
                            case "Z_MAX":
                                paraValue = GV.Z_MAX;
                                break;
                            case "X_INIT":
                                paraValue = GV.X_INIT;
                                break;
                            case "Y_INIT":
                                paraValue = GV.Y_INIT;
                                break;
                            case "Z_INIT":
                                paraValue = GV.Z_INIT;
                                break;
                            case "Z_INTERVAL":
                                paraValue = GV.Z_INTERVAL;
                                break;
                            case "X_ADJUST":
                                paraValue = GV.X_ADJUST;
                                break;
                            case "Y_ADJUST":
                                paraValue = GV.Y_ADJUST;
                                break;
                            case "Z_ADJUST":
                                paraValue = GV.Z_ADJUST;
                                break;
                            case "X_ADJUST_B":
                                paraValue = GV.X_ADJUST_B;
                                break;
                            case "Y_ADJUST_B":
                                paraValue = GV.Y_ADJUST_B;
                                break;
                            case "Z_ADJUST_B":
                                paraValue = GV.Z_ADJUST_B;
                                break;
                            case "Z_BOTTOM":
                                paraValue = GV.Z_BOTTOM;
                                break;
                            case "OP1_X":
                                paraValue = GV.OP1_X;
                                break;
                            case "OP1_Y":
                                paraValue = GV.OP1_Y;
                                break;
                            case "OP1_Z":
                                paraValue = GV.OP1_Z;
                                break;
                            case "OP1_dX":
                                paraValue = GV.OP1_dX;
                                break;
                            case "OP1_dY":
                                paraValue = GV.OP1_dY;
                                break;
                            case "OP2_X":
                                paraValue = GV.OP2_X;
                                break;
                            case "OP2_Y":
                                paraValue = GV.OP2_Y;
                                break;
                            case "OP2_Z":
                                paraValue = GV.OP2_Z;
                                break;

                            case "D_INIT":
                                paraValue = GV.D_INIT;
                                break;
                            //新增
                            case "Za_BOTTOM":
                                paraValue = GV.Za_BOTTOM;
                                break;
                            case "Zb_BOTTOM":
                                paraValue = GV.Zb_BOTTOM;
                                break;
                            case "Z_TOP":
                                paraValue = GV.Z_TOP;
                                break;
                            case "Z_OFFSET":
                                paraValue = GV.Z_OFFSET;
                                break;
                            case "dX_Camera":
                                paraValue = GV.dX_Camera;
                                break;
                            case "dY_Camera":
                                paraValue = GV.dY_Camera;
                                break;
                            case "dZ_Camera":
                                paraValue = GV.dZ_Camera;
                                break;
                            case "dX_Clean":
                                paraValue = GV.dX_Clean;
                                break;
                            case "dY_Clean":
                                paraValue = GV.dY_Clean;
                                break;
                            case "dZ_Clean":
                                paraValue = GV.dZ_Clean;
                                break;
                            case "dX_Trans":
                                paraValue = GV.dX_Trans;
                                break;
                            case "dY_Trans":
                                paraValue = GV.dY_Trans;
                                break;
                            case "dZ_Trans":
                                paraValue = GV.dZ_Trans;
                                break;
                            default:
                                paraValue = 0;
                                break;
                        }
                        sw.WriteLine(paraName + " = " + paraValue);

                    }

                    for (int i = 0; i < controlArr.Length; i++)
                    {
                        ctrls = this.Controls.Find(controlArr[i], true);
                        //sw.WriteLine(controlArr[i] + " = " + ctrls[0].Text + ";  // " + paraArr[i]);
                        sw.WriteLine(controlArr[i] + " = " + ctrls[0].Text);
                    }

                    sw.Close();
                    fs.Close();
                    WriteSuccess = true;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                WriteSuccess = false;
            }
            return WriteSuccess;
        }

        private void label11_MouseMove(object sender, MouseEventArgs e)
        {
            label11.ForeColor = Color.FromArgb(0, 0, 255);
        }

        private void label11_MouseEnter(object sender, EventArgs e)
        {
            label11.ForeColor = Color.FromArgb(0, 0, 255);
        }

        private void label11_MouseLeave(object sender, EventArgs e)
        {
            label11.ForeColor = Color.Black;
        }

        private void label11_Click(object sender, EventArgs e)
        {
            txtSetX.Text = GV.PrintingObj.Status.fPosX.ToString("0.0000");
            txtSetY.Text = GV.PrintingObj.Status.fPosY.ToString("0.0000");
            txtSetZ.Text = GV.PrintingObj.Status.fPosZ.ToString("0.0000");
        }

        /// <summary>
        /// 检查是否需要清空指令
        /// </summary>
        /// <returns>需要清空返回true; 不需要清空返回false</returns>
        public ClearResult CheckClearCommands()
        {
            return GV.CheckClearCommands();
            //if (GV.PrintingObj.IsPrinting)
            //{
            //    if (DialogResult.Yes == MessageBox.Show("当前正在打印，是否终止打印进行运动调校？", "中止打印提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2))
            //    {
            //        GV.StopImmediately();
            //        return ClearResult.Cleared;
            //    }
            //    else
            //    {
            //        return ClearResult.DonotClear;
            //    }
            //}
            //if (GV.bPausePrint)
            //{
            //    if (DialogResult.Yes == MessageBox.Show("缓冲中已有指令加载，是否清空所有指令进行运动调校？", "清空指令提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2))
            //    {
            //        GV.StopImmediately();
            //        return ClearResult.Cleared;
            //    }
            //    else
            //    {
            //        return ClearResult.DonotClear;
            //    }
            //}
            //return ClearResult.Needless;
        }

        private void btnUp2Top_Click(object sender, EventArgs e)
        {
            Up2Top();
        }

        private void btnDown2MaxZ_Click(object sender, EventArgs e)
        {
            Down2MaxZ();
        }

        public void Up2Top()
        {
            if (CheckClearCommands() != ClearResult.Needless) return;
            double curPos = GV.PrintingObj.GetFPosition(GV.Z);
            double curZa = GV.PrintingObj.GetFPosition(GV.Z1);
            double curZb = GV.PrintingObj.GetFPosition(GV.Z2);
            GV.PrintingObj.qMoveAxisTo(GV.Z, GV.Z_TOP, 20, curPos);
            //新增，xiaoZ
            //GV.PrintingObj.qMoveAxisTo(GV.Z, 0, 5, curZa);
            //GV.PrintingObj.qMoveAxisTo(GV.Z, 0, 5, curZb);
        }

        public void Down2MaxZ()
        {
            if (CheckClearCommands() != ClearResult.Needless) return;
            double curPos = GV.PrintingObj.GetFPosition(GV.Z);

            double vZdown1 = 20;    // Z轴第一阶段下针速度(mm/s)
            double vZdown2 = 2;     // Z轴第二阶段下针速度(mm/s)
            double vZdown3 = 1;     //小Z轴的下针速度
            double dNear = 10;      // 接近减速距离(mm)
            double zTarget = GV.Z_BOTTOM;
            double targetZa = GV.Za_BOTTOM;
            double targetZb = GV.Zb_BOTTOM;
            try
            {
                double setValue = Convert.ToDouble(txtZBottom.Text);
                if (zTarget > setValue)
                {
                    DialogResult dr = MessageBox.Show("当前下针位置超过设置位置！！！\r\n点击确定下针到: " + setValue.ToString("0.000"), "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                    if (DialogResult.OK == dr)
                    {
                        if (Math.Abs(GV.Zb_BOTTOM - setValue) > 0.0001)
                        {
                            GV.Z_BOTTOM = setValue;//以设定值为准
                        }
                        zTarget = setValue;
                    }
                    else if (DialogResult.Cancel == dr)
                    {
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            // 较慢速将Z轴降至接近目标位置
            GV.PrintingObj.qMoveAxisTo(GV.Z, zTarget - dNear, vZdown1, curPos);
            GV.PrintingObj.qWaitMoveEnd();
            // 非常慢速将Z轴降至目标位置
            GV.PrintingObj.qMoveAxisTo(GV.Z, zTarget, vZdown2, 0);
            GV.PrintingObj.qWaitMoveEnd();
            //下降小Z
            GV.PrintingObj.qMoveAxisTo(GV.Z1, targetZa, vZdown2, 0);
            GV.PrintingObj.qMoveAxisTo(GV.Z2, targetZb, vZdown2, 0);
            GV.PrintingObj.qWaitMoveEnd();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            bAllowKeyCtrl1 = false;
            chkKeyControl.Checked = false;
            this.Text = "运动调校";
        }

        private void rdbNozzle1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void rdbNozzle2_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void btnNozzle1_Click(object sender, EventArgs e)
        {
            //GV.PrintingObj.qExtrude(1, 0);
            GV.PrintingObj.qSwitchNozzle(0);
            //GV.PrintingObj.qExtrude(0, 1);
        }

        private void btnNozzle2_Click(object sender, EventArgs e)
        {
            //GV.PrintingObj.qExtrude(0, 0);
            GV.PrintingObj.qSwitchNozzle(1);
            //GV.PrintingObj.qExtrude(1, 1);
        }

        private void btnDown2MaxZ_MouseMove(object sender, MouseEventArgs e)
        {
            lblInitPos.Text = "下针到：" + GV.Z_BOTTOM.ToString("0.000");
        }

        private void btnDown2MaxZ_MouseLeave(object sender, EventArgs e)
        {
            lblInitPos.Text = "";
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            GV.PrintingObj.qSwitchNozzle(trackBar1.Value);
        }

        private void FrmMotionAdjust_Enter(object sender, EventArgs e)
        {
            UpdateNozzleStatus();
        }
        public void UpdateNozzleStatus()
        {
            trackBar1.Value = GV.PrintingObj.Status.nozzleID;
        }

        private void radioButton1_Click(object sender, EventArgs e)
        {
            SelectRadio1();
        }
        //临时用
        private void button1_Click(object sender, EventArgs e)
        {
            //GV.PrintingObj.MoveXbotXYAbsolute(GV.PMC.arrXBotIds[0], 360, 120, 20, 0, 20 * 100);
            //
            //GV.PrintingObj.MoveXbotAbsolute(GV.PMC.arrXBotIds[0], GV.PMC.Z, 1, 1, 10);
            GV.PrintingObj.MoveXbotSixDof(GV.PMC.arrXBotIds[0], 360, 120, 1, 0, 0, 0);
        }

        private void txtZbBottom_TextChanged(object sender, EventArgs e)
        {

        }



        private void radioButton2_Click(object sender, EventArgs e)
        {
            SelectRadio2();
        }

        private void radioButton3_Click(object sender, EventArgs e)
        {
            SelectRadio3();
        }
    }
}
