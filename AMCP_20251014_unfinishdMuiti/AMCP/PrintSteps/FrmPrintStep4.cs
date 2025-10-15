using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace AMCP
{
    public partial class FrmPrintStep4 : Form
    {
        //FrmMain GV;
        bool bExtrudeTestCompleted = false;
        bool bNozzleAdjustCompleted = false;
        bool bPrintPreviewed = false;
        Label[] arr_nozzleAdjust;
        Label[] arr_stageManage;
        TextBox[] arr_txt2PosHeight;
        TextBox[] arr_txt2RotateSpeed;
        TextBox[] arr_txt2SetPressure;
        TextBox[] arr_txtPrintSpeed;
        public FrmPrintStep4()
        {
            InitializeComponent();
            arr_nozzleAdjust = new Label[] { lblNozzleAdjustA, lblNozzleAdjustB};
            arr_stageManage = new Label[] { lblPrintStageA, lblPrintStageB};
            arr_txt2PosHeight = new TextBox[] { txtPosHeightA, txtPosHeightB};
            arr_txt2RotateSpeed = new TextBox[] { txtRotateSpeedA, txtRotateSpeedB};
            arr_txt2SetPressure = new TextBox[] { txtSetPressureA, txtSetPressureB};
            arr_txtPrintSpeed = new TextBox[] { txtPrintSpeed1, txtPrintSpeed2, txtPrintSpeed3, txtPrintSpeed4 };
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            //GV = this.MdiParent as FrmMain;
            txtPathFileLoad.Text = GV.PathFileName;
            txtTechParaFileLoad.Text = GV.TechParaFileName;
            txtConLogFileName.Text = GV.PathFileName.ToString() + "ControlLog";
            txtMonLogFileName.Text = GV.PathFileName.ToString() + "MonitorLog";
        }

        void ReadLayerData(double[][] matrixP, double xStart, double yStart, int layer, out double dz, out double liftHeight, out int axis, out double[] m, out double[] p)
        {
            int count = matrixP[layer].Length;
            p = new double[count - 5];
            m = new double[2];
            dz = matrixP[layer][0];
            liftHeight = matrixP[layer][1];
            axis = (int)matrixP[layer][2];
            double offset_m = (axis == 0 ? xStart : yStart);
            double offset_p = (axis == 0 ? yStart : xStart);
            m[0] = matrixP[layer][3] + offset_m;
            m[1] = matrixP[layer][4] + offset_m;

            for (int i = 5; i < count; i++)
            {
                p[i - 5] = matrixP[layer][i] + offset_p;
            }
        }

        private string findAttributeInXML(string argu)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(txtTechParaFileLoad.Text);
            XmlElement root = doc.DocumentElement;
            XmlNodeList nodeList = root.ChildNodes;
            for (int i = 0; i < nodeList.Count; i++)
            {
                if (nodeList[i].Name == argu)
                {
                    return nodeList[i].InnerText;
                }
            }
            return null;
        }

        private void FrmPrintStep4_Enter(object sender, EventArgs e)
        {
            txtPathFileLoad.Text = GV.PathFileName;
            txtTechParaFileLoad.Text = GV.TechParaFileName;
        }

        private void btnExtrudeTest_Click(object sender, EventArgs e)
        {
            panel1.Visible = !panel1.Visible;
            panel2.Hide();
            panel3.Hide();
        }

        private void btnNozzleCalibrate_Click(object sender, EventArgs e)
        {
            panel2.Visible = !panel2.Visible;
            panel1.Hide();
            panel3.Hide();
        }
        //打印预览
        private void btnPrintPreview_Click(object sender, EventArgs e)
        {  

            if (!panel3.Visible)
            {
                panel3.Visible = true;
                panel1.Hide();
                panel2.Hide();

            }
            else
            {
                panel3.Visible = false;
                return;
            }

            GV.StopImmediately();
            GV.ExecuteCustomedMultiCubes(GV.listPathFileNameA, GV.listPathFileNameB, GV.PrintMode);
            string extName = System.IO.Path.GetExtension(GV.PathFileName);
            if (extName == ".csv")
            {
                 GV. ExecuteCustomedCube(GV.PathFileName, GV.PrintMode);
                //GV.ExecuteCustomedMultiCubes(GV.PathFileName);
            }
            else if (extName == ".gcode")
            {
                GV.ExecuteGcode(GV.PathFileName, GV.PrintMode);
            }

            GV.frmPathPreview.LoadPlannedPath();
        }

        public void SetProgressbarVisible(bool bShow)
        {         
            lblShowPathLoading.Visible = bShow;
            lblShowPathLoading.Refresh();
            progressPathloading.Visible = bShow;
            progressPathloading.Refresh();
        }

        public void UpdatePrintPercent(int percent)
        {
            progressPathloading.Value = percent;
        }

        private void btnExtrudersOn_Click(object sender, EventArgs e)
        {
            //根据设置值出丝测试
            double pressValue0 = Convert.ToDouble(txtSetPressureA.Text);
            double pressValue1 = Convert.ToDouble(txtSetPressureB.Text);
            double rotaryValue0 = Convert.ToDouble(txtRotateSpeedA.Text);
            double rotaryValue1 = Convert.ToDouble(txtRotateSpeedB.Text);

            if (pressValue0 != GV.extrudePressValueA || pressValue1 != GV.extrudePressValueB)
            {
                GV.PrintingObj.SetExtrudePorts(-2, 1);
            }
            else
            {
                GV.PrintingObj.SetExtrudePorts(-2, 1);
            }
                //GV.PrintingObj.SetExtrudePorts(-2, 1);
        }

        private void btnExtrudersOff_Click(object sender, EventArgs e)
        {
            GV.PrintingObj.SetExtrudePorts(-2, 0);
        }

        private void btnReturnStep3_Click(object sender, EventArgs e)
        {
            //GV.frmPrintGuide.SetCurrentStep(GV.frmPrintStep3);
            GV.frmMotionAdjust.Down2MaxZ();
        }

        private void btnCompleteExtrudeTest_Click(object sender, EventArgs e)
        {
            bExtrudeTestCompleted = true;
            panel1.Hide();
            btnExtrudeTest.BackColor = Color.LightGreen;
            CheckReady();
        }

        public void CheckReady()
        {
            if (bExtrudeTestCompleted && bNozzleAdjustCompleted && bPrintPreviewed)
            {
                SetStatus(1);
            }
            else
            {
                SetStatus(0);
            }
        }

        public void SetNotPreviewed()
        {
            bPrintPreviewed = false;
            btnPrintPreview.BackColor = Color.White;
            //GV.frmPrintGuide.btnNextStep.Enabled = false;
            GV.frmPrintGuide.btnNextStep.BackColor = Color.White;
        }

        public int printStatus = 0; // 打印状态: 0. 未准备好; 1. 已准备好但未开始打印; 2. 打印中

        /// <summary>
        /// 设置当前状态
        /// </summary>
        /// <param name="status">打印状态</param>
        public void SetStatus(int status)
        {
            switch (status)
            {
                case 0: // 未准备好
                    //GV.frmPrintGuide.btnNextStep.Enabled = false;
                    GV.frmPrintGuide.btnNextStep.BackColor = Color.White;
                    //btnExtrudeTest.Enabled = true;
                    //btnNozzleCalibrate.Enabled = true;
                    btnPrintPreview.BackColor = Color.White;
                    
                    break;
                case 1: // 已准备好但未开始打印
                    //GV.frmPrintGuide.btnNextStep.Enabled = true;
                    GV.frmPrintGuide.btnNextStep.BackColor = Color.LightGreen;
                    //btnExtrudeTest.Enabled = true;
                    //btnNozzleCalibrate.Enabled = true;
                    //btnPrintPreview.Enabled = true;
                    break; 
                case 2: // 打印中
                    //GV.frmPrintGuide.btnNextStep.Enabled = true;
                    GV.frmPrintGuide.btnNextStep.BackColor = Color.LightGreen;
                    //btnExtrudeTest.Enabled = false;
                    //btnNozzleCalibrate.Enabled = false;
                    //btnPrintPreview.Enabled = false;
                    break;
                default:
                    break;
            }
            printStatus = status;
        }
        //界面更新
        public void SetCheckState(int index, bool bChecked)
        {
            if (bChecked)
            {
                arr_nozzleAdjust[index].ForeColor = Color.White;
                arr_stageManage[index].ForeColor = Color.White;
            }
            else
            {
                arr_nozzleAdjust[index].ForeColor = Color.DarkGray;
                arr_nozzleAdjust[index].ForeColor = Color.DarkGray;
            }
            arr_txt2PosHeight[index].Enabled = bChecked;
            arr_txt2RotateSpeed[index].Enabled = bChecked;
            arr_txt2SetPressure[index].Enabled = bChecked;
            arr_txtPrintSpeed[index].Enabled = bChecked;
        }
        private void btnUp2Top_Click(object sender, EventArgs e)
        {
            GV.frmMotionAdjust.Up2Top();
        }

        private void btnDown2MaxZ_Click(object sender, EventArgs e)
        {
            GV.frmMotionAdjust.Down2MaxZ();
        }

        private void btnCompleteNozzleAdjust_Click(object sender, EventArgs e)
        {
            bNozzleAdjustCompleted = true;
            panel2.Hide();
            CheckReady();
            btnNozzleCalibrate.BackColor = Color.LightGreen;
        }


        private void btnAutoNozzleAdjust_Click(object sender, EventArgs e)
        {
            GV.frmNozzleCalibrate.Show();
            GV.frmNozzleCalibrate.Activate();
        }
        //预览窗口
        private void btnOpenPreview_Click(object sender, EventArgs e)
        {
            GV.frmPathPreview.EnableConfirmPrinting(true);
            GV.frmPathPreview.Show();
            GV.frmPathPreview.Activate();
        }

        private void btnCompletePreview_Click(object sender, EventArgs e)
        {
            bPrintPreviewed = true;
            panel3.Hide();
            CheckReady();
            btnPrintPreview.BackColor = Color.LightGreen;

        }

        public void ResetForm()
        {
            bExtrudeTestCompleted = false;
            bNozzleAdjustCompleted = false;
            bPrintPreviewed = false;
            btnExtrudeTest.BackColor = Color.White;
            btnNozzleCalibrate.BackColor = Color.White;
            btnPrintPreview.BackColor = Color.White;
        }

        private void btnDown2MaxZ_MouseMove(object sender, MouseEventArgs e)
        {
            lblInitPos.Text = "下针到：" + GV.Z_BOTTOM.ToString("0.000");
        }

        private void btnDown2MaxZ_MouseLeave(object sender, EventArgs e)
        {
            lblInitPos.Text = "";
        }


        private void chkManualAdjust_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkManualAdjust.Checked && chkCleaned.Checked)
            {
                btnAutoNozzleAdjust.Enabled = true;
                GV.frmMain.EnableNozzleCalibrate();
            }
            else
            {
                btnAutoNozzleAdjust.Enabled = false;
                GV.frmMain.DisableNozzleCalibrate();
            }
            //btnCompleteNozzleAdjust.Enabled = chkCleaned.Checked;
        }

        private void chkCleaned_CheckedChanged(object sender, EventArgs e)
        {
            //btnCompleteNozzleAdjust.Enabled = chkCleaned.Checked;

            if (!chkManualAdjust.Checked && chkCleaned.Checked)
            {
                btnAutoNozzleAdjust.Enabled = true;
                GV.frmMain.EnableNozzleCalibrate();
            }
            else
            {
                btnAutoNozzleAdjust.Enabled = false;
                GV.frmMain.DisableNozzleCalibrate();
            }
        }
        //将文件记录，两个文件
        public void btnSaveLogfile_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "log files (*.txt)|*.txt";
            saveFileDialog1.DefaultExt = "txt";
            //保存控制指令日志文件
            saveFileDialog1.FileName = GV.PathFileName.ToString() + "ControlLog_" + DateTime.Now.ToString("y.M.d hhmmss");
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string FileName = saveFileDialog1.FileName;
                if (!GV.WriteLineTextFile(FileName, GV.PrintingObj.Controlcommand.ToArray()))
                {
                    MessageBox.Show("保存失败！");
                }
            }
            //保存监测信息日志文件
            saveFileDialog1.FileName = GV.PathFileName.ToString() + "MonitorLog_" + DateTime.Now.ToString("y.M.d hhmmss");
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string FileName = saveFileDialog1.FileName;
                if (!GV.WriteLineTextFile(FileName, GV.PrintingObj.Monitordata.ToArray()))
                {
                    MessageBox.Show("保存失败！");
                }
            }
        }

        /// <summary>
        /// 设置指定工位的下针位置,双工位，用b去匹配
        /// </summary>
        /// <param name="index">工位序号索引</param>
        /// <param name="value">下针位置</param>       
        public void SetTargetZ(int index, int printPos, double value)
        {
            double adjustZa = 5;//微调平台调节的距离
            double adjustZb = 5;
            double targetZ = value - 5;
                     
            if (index >= 0 )//&& index < GV.listTargetZ.Count)
            {
                if (printPos == 0)//A工位
                {
                    // 确保GV.listTargetZ有足够的长度
                    while (GV.listTargetZ.Count < index)
                    {
                        GV.listTargetZ.Add(0.0); // 添加默认值
                    }
                    GV.listTargetZ[index] = value;
                    while (GV.listSlightZa.Count < index)
                    {
                        GV.listSlightZa.Add(0.0);
                    }
                    GV.listSlightZa[index] = adjustZa;
                }
                else if (printPos == 1)
                {
                    adjustZb = value - GV.listTargetZ[index];//补充A工位的差值
                    //待修改
                    //// 确保GV.listTargetZ有足够的长度
                    //while (GV.listTargetZ.Count < index)
                    //{
                    //    GV.listTargetZ.Add(0.0); // 添加默认值
                    //}
                    //GV.listTargetZ[index] = value;

                    //while (GV.listSlightZb.Count < index)
                    //{
                    //    GV.listSlightZb.Add(0.0); 
                    //}
                    //GV.listSlightZb[index] = adjustZb;
                }
            }
            else
            {
                MessageBox.Show("数组索引越界，请检查！", "错误");
            }
        }
        /// <summary>
        /// 设置指定工位的螺杆转速
        /// </summary>
        /// <param name="index">工位索引</param>
        /// <param name="value">转速值</param>
        public void SetRotateSpeed(int index, int printPos, double value)
        {
            if (index >= 0 && (index < GV.listRotateSpeedA.Count || index < GV.listRotateSpeedB.Count))
            {
                if (printPos == 0)
                {
                    GV.listRotateSpeedA[index] = value;
                }
                else if (printPos == 1)
                {
                    GV.listRotateSpeedB[index] = value;
                }             
            }
            else
            {
                MessageBox.Show("数组索引越界，请检查！", "错误");
            }
        }

        /// <summary>
        /// 设置指定工位的螺杆气压
        /// </summary>
        /// <param name="index">工位索引</param>
        /// <param name="value">气压值</param>
        public void SetAirPressure(int index, int printPos, double value)
        {
            if (index >= 0 && (index < GV.listAirPressureA.Count || index < GV.listRotateSpeedB.Count))
            {
                if (printPos == 0)
                {
                    GV.listAirPressureA[index] = value;
                }
                else if (printPos == 1)
                {
                    GV.listAirPressureB[index] = value;
                }
                  
            }
            else
            {
                MessageBox.Show("数组索引越界，请检查！", "错误");
            }
        }

        /// <summary>
        /// 设置指定工位的打印速度
        /// </summary>
        /// <param name="index">工位索引</param>
        /// <param name="value">速度值</param>
        public void SetPrintSpeed(int index, double value)
        {
            if (index >= 0 && index < GV.listPrintSpeed.Count)
            {
                GV.listPrintSpeed[index] = value;
            }
            else
            {
                MessageBox.Show("数组索引越界，请检查！", "错误");
            }
        }
        //人工配置下针高度
        private void txtPosHeight_Leave(object sender, EventArgs e)
        {
            try
            {
                Control ctrl = sender as Control;
                string str = ctrl.Name;
                int index = 0;
                int posA = 0, posB = 1;
                if (str == "txtPosHeightA")
                {
                    double value = Convert.ToDouble(arr_txt2PosHeight[0].Text);
                    SetTargetZ(index, posA, value);//初始时，可人工配置下针高度
                }
                else if (str == "txtPosHeightB")
                {
                    double value = Convert.ToDouble(arr_txt2PosHeight[1].Text);
                    SetTargetZ(index, posB,  value);//初始时，可人工配置下针高度
                }                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //人工配置螺杆转速
        private void txtRotateSpeed_Leave(object sender, EventArgs e)
        {
            try
            {
                Control ctrl = sender as Control;
                string str = ctrl.Name;
                int index = 0;
                int posA = 0, posB = 1;
                if (str == "txtRotateSpeedA")
                {
                    double value = Convert.ToDouble(arr_txt2RotateSpeed[0].Text);
                    SetRotateSpeed(index, posA, value);
                }
                else if (str == "txtRotateSpeedB")
                {
                    double value = Convert.ToDouble(arr_txt2RotateSpeed[1].Text);
                    SetRotateSpeed(index, posB, value);
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtSetPressure_Leave(object sender, EventArgs e)
        {
            try
            {
                Control ctrl = sender as Control;
                string str = ctrl.Name;
                int index = 0;
                int posA = 0, posB = 1;
                if (str == "txtRotateSpeedA")
                {
                    double value = Convert.ToDouble(arr_txt2SetPressure[0].Text);
                    SetAirPressure(index, posA, value);
                }
                else if (str == "txtRotateSpeedB")
                {
                    double value = Convert.ToDouble(arr_txt2SetPressure[1].Text);
                    SetAirPressure(index, posB, value);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void txtPrintSpeed_Leave(object sender, EventArgs e)
        {
            try
            {
                Control ctrl = sender as Control;
                string str = ctrl.Name;
                int index = Convert.ToInt32(str.Substring(str.Length - 1, 1)) - 1;
                double value = Convert.ToDouble(arr_txtPrintSpeed[index].Text);
                SetPrintSpeed(index, value);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
