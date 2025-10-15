using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AMCP
{
    public partial class FrmPrintGuide : Form
    {
        public Form frmCurrStep;

        public FrmPrintGuide()
        {
            InitializeComponent();
            panel1.Hide();
            InitSteps();
            SetCurrentStep(GV.frmPrintStep1);
        }

        public void InitSteps()
        {
            if (GV.frmPrintStep1 == null) GV.frmPrintStep1 = new FrmPrintStep1();
            GV.frmPrintStep1.FormBorderStyle = FormBorderStyle.None;
            GV.frmPrintStep1.TopLevel = false;
            GV.frmPrintStep1.Visible = false;
            this.panel1.Controls.Add(GV.frmPrintStep1);
            
            if (GV.frmPrintStep2 == null) GV.frmPrintStep2 = new FrmPrintStep2();
            GV.frmPrintStep2.FormBorderStyle = FormBorderStyle.None;
            GV.frmPrintStep2.TopLevel = false;
            GV.frmPrintStep2.Visible = false;
            this.panel1.Controls.Add(GV.frmPrintStep2);

            if (GV.frmPrintStep3 == null) GV.frmPrintStep3 = new FrmPrintStep3();
            GV.frmPrintStep3.FormBorderStyle = FormBorderStyle.None;
            GV.frmPrintStep3.TopLevel = false;
            GV.frmPrintStep3.Visible = false;
            this.panel1.Controls.Add(GV.frmPrintStep3);

            if (GV.frmPrintStep4 == null) GV.frmPrintStep4 = new FrmPrintStep4();
            GV.frmPrintStep4.FormBorderStyle = FormBorderStyle.None;
            GV.frmPrintStep4.TopLevel = false;
            GV.frmPrintStep4.Visible = false;
            this.panel1.Controls.Add(GV.frmPrintStep4);

            this.Text = GV.frmPrintStep1.Text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currStep"></param>
        public void SetCurrentStep(Form currStep)
        {
            if (frmCurrStep != null)
            {
                frmCurrStep.Hide();
            }
            switch (currStep.Name)
            {
                case "FrmPrintStep1":
                    btnPreStep.Visible = false;
                    btnNextStep.Visible = true;
                    btnNextStep.Text = "下一步(&N)";
                    btnNextStep.Enabled = true;
                    btnNextStep.BackColor = Color.White;
                    break;
                case "FrmPrintStep2":
                    if (CheckConnected())
                    {
                        btnPreStep.Visible = true;
                        btnNextStep.Visible = true;
                        btnNextStep.Text = "下一步(&N)";
                        btnNextStep.Enabled = true;
                        btnNextStep.BackColor = Color.White;
                    }
                    else
                    {
                        return;
                    }
                    break;
                case "FrmPrintStep3":
                    if (CheckConnected())
                    {
                        btnPreStep.Visible = true;
                        btnNextStep.Visible = true;
                        btnNextStep.Text = "下一步(&N)";
                        btnNextStep.Enabled = true;
                        btnNextStep.BackColor = Color.White;                        
                    }
                    break;
                case "FrmPrintStep4":
                    btnPreStep.Visible = true;
                    btnNextStep.Visible = true;
                    btnNextStep.Text = "开始打印(&P)";
                    //btnNextStep.Enabled = false;
                    btnNextStep.BackColor = Color.White;
                    GV.frmPrintStep4.ResetForm();
                    CheckConnected();
                    break;
                default:
                    break;
            }
            frmCurrStep = currStep;
            this.Text = frmCurrStep.Text;
            frmCurrStep.Show();
            timer1.Start();
        }

        private bool CheckConnected()
        {
            bool IsConnected = true;
            if (GV.connMode == ConnectMode.Disconnect)  // 如果系统尚未连接
            {
                SetCurrentStep(GV.frmPrintStep1);
                IsConnected = false;
                MessageBox.Show("控制器尚未连接，请先连接控制器！", "控制器未连接");
            }
            return IsConnected;
        }

        private void btnPreStep_Click(object sender, EventArgs e)
        {
            switch (frmCurrStep.Name)
            {
                case "FrmPrintStep1":
                    break;
                case "FrmPrintStep2":
                    SetCurrentStep(GV.frmPrintStep1);
                    break;
                case "FrmPrintStep3":
                    SetCurrentStep(GV.frmPrintStep2);
                    break;
                case "FrmPrintStep4":
                    SetCurrentStep(GV.frmPrintStep3);
                    break;
                default:
                    break;
            }
        }

        private void btnNextStep_Click(object sender, EventArgs e)
        {
            switch (frmCurrStep.Name)
            {
                case "FrmPrintStep1":
                    SetCurrentStep(GV.frmPrintStep2);
                    break;
                case "FrmPrintStep2":
                    if (GV.frmPrintStep2.ValidityCheck())
                    {
                        SetCurrentStep(GV.frmPrintStep3);
                        GV.PathFileName = GV.frmPrintStep2.txtGcodeFileName.Text;//更新选择路径
                        //string str1 = Application.StartupPath + "\\Log\\" + "Conlog_" + Path.GetFileName(GV.PathFileName);
                        //string str2 = Application.StartupPath + "\\Log\\" + "Monlog_" + Path.GetFileName(GV.PathFileName);
                        //GV.ControlLogFileName = str1;
                        //GV.MonitorLogFileName = str2;
                    }
                    break;
                case "FrmPrintStep3":
                    if (GV.frmPrintStep3.ValidityCheck())
                    {
                        SetCurrentStep(GV.frmPrintStep4);
                        GV.TechParaFileName = GV.frmPrintStep3.txtTechParaFileName.Text;
                        GV.frmPrintStep3.UpdateOutports();
                    }
                    break;
                case "FrmPrintStep4":
                    switch (GV.frmPrintStep4.printStatus)
	                {
                        case 0:
                            MessageBox.Show("请完成所有准备工作后开始打印。", "提示");
                            break;
                        case 1:
                            GV.ConfirmPrinting();
                            
                            break;
	                }
                    break;
                default:
                    break;
            }
        }

        private void FrmPrintSteps_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = (e.CloseReason == CloseReason.UserClosing);
            this.Hide();
            this.panel1.Hide();
        }

        private void FrmPrintSteps_Load(object sender, EventArgs e)
        {
            //timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Stop();
            panel1.Show();
        }

        private void FrmPrintSteps_Activated(object sender, EventArgs e)
        {
           timer1.Start();
        }

        private void FrmPrintSteps_Enter(object sender, EventArgs e)
        {
            //timer1.Start();
        }

    }
}
