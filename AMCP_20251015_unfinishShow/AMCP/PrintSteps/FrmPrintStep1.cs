using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace AMCP
{
    public partial class FrmPrintStep1 : Form
    {
        //FrmMain GV;

        public void SetAxesStatus(StageStatus status)
        {
            EnabledOn0.Visible = status.isEnabledX;
            EnabledOn1.Visible = status.isEnabledY;
            EnabledOn2.Visible = status.isEnabledZ;
            EnableOn3.Visible = status.isEnabledZ1;
            EnableOn4.Visible = status.isEnabledZ2;
            //新增平面电机
            ActivateOnA.Visible = status.isActiveA;
            ActivateOnB.Visible = status.isActiveB;

            MovingOn0.Visible = status.isMovingX;
            MovingOn1.Visible = status.isMovingY;
            MovingOn2.Visible = status.isMovingZ;
            MovingOn3.Visible = status.isMovingZ1;
            MovingOn4.Visible = status.isMovingZ2;
        }

        public void setIsEnabledXFlag(bool flag)
        {
            EnabledOn0.Visible = flag;
            EnabledOff0.Visible = !flag;
        }

        public void setIsEnabledYFlag(bool flag)
        {
            EnabledOn1.Visible = flag;
            EnabledOff1.Visible = !flag;
        }

        public void setIsEnabledZFlag(bool flag)
        {
            EnabledOn2.Visible = flag;
            EnabledOff2.Visible = !flag;
        }

        public void setMovingOnXFlag(bool flag)
        {
            MovingOn0.Visible = flag;
            MovingPB.Visible = !flag;
        }

        public void setMovingOnYFlag(bool flag)
        {
            MovingOn1.Visible = flag;
            MovingPB1.Visible = !flag;
        }

        public void setMovingOnZFlag(bool flag)
        {
            MovingOn2.Visible = flag;
            MovingPB2.Visible = !flag;
        }

        //flag:判断对话框进行上一步或下一步的操作标志，false：即将进入上一步操作，true：即将进入下一步

        //FrmPrintGuide parent;
        public FrmPrintStep1()
        {
            InitializeComponent();
        }

        private void btnHomeAll_Click(object sender, EventArgs e)
        {
            GV.PrintingObj.HomeAll();
        }

        private void btnHomeX_Click(object sender, EventArgs e)
        {
            GV.PrintingObj.Home(0);
        }

        private void btnHomeY_Click(object sender, EventArgs e)
        {
            GV.PrintingObj.Home(1);
        }

        private void btnHomeZ_Click(object sender, EventArgs e)
        {
            GV.PrintingObj.Home(2);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            timer1.Tag = "Connecting";
            timer1.Start();
        }

        private void rdoConnect_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                if (GV.connMode != ConnectMode.Disconnect)
                {
                    GV.PrintingObj.CloseComm();
                }
                if (rdoDisConnect.Checked)
                {
                    MakeConnectedState(ConnectMode.Disconnect);
                }

                if (rdoConnectController.Checked)
                {
                    // 连接到控制器
                    //if (GV.PrintingObj.OpenCommEthernet("192.168.1.100"))
                    if (GV.PrintingObj.OpenCommEthernet(GV.IpAddr) && GV.PrintingObj.OpenPMCommEthernet(GV.PMC.IpAddress))
                    {
                        MakeConnectedState(ConnectMode.ConnectController);
                    }
                    else
                    {
                        MakeConnectedState(ConnectMode.Disconnect);
                        MessageBox.Show("连接失败！");
                    }
                }

                if (rdoConnectSimulator.Checked)
                {
                    // 连接到仿真器
                    if (GV.PrintingObj.OpenCommEthernet("127.0.0.1"))
                    {
                        MakeConnectedState(ConnectMode.ConnectSimulator);
                    }
                    else
                    {
                        MakeConnectedState(ConnectMode.Disconnect);
                        MessageBox.Show("连接失败！");
                    }
                }
            }
            catch (Exception ex)
            {
                //PrintingObj.PushMessage(ex.Message);
                MessageBox.Show(ex.Message);
            }


        }


        //设置连接状态,待完善，ACS，PMC，机械臂
        private void MakeConnectedState(ConnectMode mode)
        {
            switch (mode)
            {
                case ConnectMode.Disconnect:
                    ConnOn.Visible = false;
                    //ConnPMon.Visible = false;
                    GV.bgWorker.CancelAsync();
                    GV.bgWorker2.CancelAsync();
                    //新增
                    // GV.monitorWorker.CancelAsync();//断开连接 任务取消
                    EnableBtn.Enabled = false;
                    DisableBtn.Enabled = false;
                    btnInitSys.Enabled = false;
                    //GV.frmPrintGuide.btnNextStep.Enabled = false;
                    GV.connMode = mode;
                    GV.frmImmStop.Hide();
                    GV.frmMotionAdjust.Hide();
                    //GV.frmNozzleCalibrate.Hide();
                    GV.frmPathPreview.Hide();
                    GV.frmPathTrace.Hide();
                    GV.frmStatusMonitor.Hide();
                    //GV.frmRotaryValveCtrl.Hide();
                    lblConnectStatus.Text = "连接断开";
                    break;
                case ConnectMode.ConnectController:
                    ConnOn.Visible = true;
                    //ConnPMon.Visible = GV.PrintingObj.bPMConnected;

                    if (!GV.bgWorker.IsBusy)
                    {
                        GV.bgWorker.RunWorkerAsync();
                    }

                    if (!GV.bgWorker2.IsBusy)
                    {
                        GV.bgWorker2.RunWorkerAsync();
                    }
                    //if (!GV.monitorWorker.IsBusy)
                    //{
                    //    GV.monitorWorker.RunWorkerAsync();//连接到控制器，开启后台任务
                    //}
                    EnableBtn.Enabled = true;
                    DisableBtn.Enabled = true;
                    btnInitSys.Enabled = true;
                    GV.frmPrintGuide.btnNextStep.Enabled = true;
                    GV.frmImmStop.Show();
                    GV.frmMain.Activate();
                    lblConnectStatus.Text = "成功连接到控制器";
                    CheckIfInitialized();//检查是否初始化
                    //新增，将动子移动到各自的中心
                    //CheckIfInMultiOperations();//检查动子数量
                    break;
                case ConnectMode.ConnectSimulator://平面电机暂未开发仿真模式
                    ConnOn.Visible = true;
                    
                    if (!GV.bgWorker.IsBusy)
                    {
                        GV.bgWorker.RunWorkerAsync();
                    }

                    if (!GV.bgWorker2.IsBusy)
                    {
                        GV.bgWorker2.RunWorkerAsync();
                    }
                    //if (!GV.monitorWorker.IsBusy)
                    //{
                    //    GV.monitorWorker.RunWorkerAsync();//连接到控制器，开启后台任务
                    //}
                    EnableBtn.Enabled = true;
                    DisableBtn.Enabled = true;
                    btnInitSys.Enabled = true;
                    GV.frmPrintGuide.btnNextStep.Enabled = true;

                    GV.frmImmStop.Show();
                    GV.frmMain.Activate();
                    lblConnectStatus.Text = "成功连接到仿真器";
                    CheckIfInitialized();

                    break;
                case ConnectMode.ConnectControllerWithLS:
                    // 20180505 连接雷塞控制器 CYQ 
                    //int iret = -1;
                    //m_handle = (IntPtr)(0);
                    //string ipStr = "192.168.1.11";
                    //iret = SMC6X.SMCOpenEth(ipStr, ref m_handle);
                    //if (0 != iret)
                    //{
                    //    MessageBox.Show("注射泵连接失败");
                    //    return;
                    //}
                    //else
                    //{
                    //    SMC6X.phandle = m_handle;
                    //}
                    //SMC6X.phandle = m_handle;
                    CheckIfInitialized();
                    break;
                case ConnectMode.ConnectPMController:
                    //20251013连接平面电机控制器
                    //新增，将动子移动到各自的中心
                    CheckIfInMultiOperations();//检查动子数量
                    ConnPMon.Visible = GV.PrintingObj.bPMConnected;
                    break;
                    case ConnectMode.DisconnectPMController:
                    ConnPMon.Visible = false;
                    break;
                default:
                    break;
            }
            GV.frmMain.SetConnectStatus(mode);
        }

        private void EnableBtn_Click(object sender, EventArgs e)
        {
            bool[] chkAxis = new bool[7];  //坐标轴被选中情况
            chkAxis[0] = chkAxis0.Checked;
            chkAxis[1] = chkAxis1.Checked;
            chkAxis[2] = chkAxis2.Checked;
            chkAxis[3] = chkAxis3.Checked;
            chkAxis[4] = chkAxis4.Checked;
            chkAxis[5] = chkXbotA.Checked;
            chkAxis[6] = chkXbotB.Checked;

            // if no axis is selected
            if (!chkAxis[0] && !chkAxis[1] && !chkAxis[2])
            {
                return;
            }

            // if communication is open
            int iAxis = 0;
            for (int i = 0; i < 5; i++)
            {
                if (chkAxis[i])
                {
                    switch (i)
                    {
                        case 0:
                            iAxis = GV.X;
                            break;
                        case 1:
                            iAxis = GV.Y;
                            break;
                        case 2:
                            iAxis = GV.Z;
                            break;
                        case 3:
                            iAxis = GV.Z1;
                            break;
                        case 4:
                            iAxis = GV.Z2;
                            break;
                        default:
                            break;
                    }
                    GV.PrintingObj.EnableMotor(iAxis);
                }
            }
            if (chkAxis[5] || chkAxis[6]) GV.PrintingObj.ActiveXbot();//激活所有动子
            //GV.PrintingObj.ActiveXbot();//激活所有动子
        }

        private void DisableBtn_Click(object sender, EventArgs e)
        {
            bool[] chkAxis = new bool[5];  //坐标轴被选中情况
            chkAxis[0] = chkAxis0.Checked;
            chkAxis[1] = chkAxis1.Checked;
            chkAxis[2] = chkAxis2.Checked;
            chkAxis[3] = chkAxis3.Checked;
            chkAxis[4] = chkAxis4.Checked;
            // if no axis is selected
            if (!chkAxis[0] && !chkAxis[1] && !chkAxis[2])
            {
                return;
            }

            // if communication is open

            for (int i = 0; i < 5; i++)
            {
                if (chkAxis[i])
                {
                    switch (i)
                    {
                        case 0:
                            GV.PrintingObj.DisableMotor(GV.X);
                            break;
                        case 1:
                            GV.PrintingObj.DisableMotor(GV.Y);
                            break;
                        case 2:
                            GV.PrintingObj.DisableMotor(GV.Z);
                            break;
                        case 3:
                            GV.PrintingObj.DisableMotor(GV.Z1);
                            break;
                        case 4:
                            GV.PrintingObj.DisableMotor(GV.Z2);
                            break;
                        default:
                            break;
                    }
                }
            }
            GV.PrintingObj.DeActiveXbots();//取消所有动子激活
        }


        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

        private void btnInitSys_Click(object sender, EventArgs e)
        {
            if (btnInitSys.Text == "初始化") // 启动初始化buffer
            {
                InitMotionSys();
                //新增，将动子移动到各自的中心
            }
            else            // 终止初始化buffer
            {
                CancelInitMotionSys(false);
            }
        }

        /// <summary>
        /// 检查系统是否已经初始化
        /// </summary>
        public void CheckIfInitialized()
        {
            bool bInitialized = false;

            try
            {
                double percent = GV.PrintingObj.Ch.ReadVariable("Initialized", 0);
                if (percent >= 100)
                {
                    bInitialized = true;
                }
            }
            catch (Exception ex)
            {
                bInitialized = false;
            }

            if (!bInitialized)
            {
                DialogResult dlgRslt = MessageBox.Show("检测到系统尚未初始化，是否现在进行?", "初始化提示", MessageBoxButtons.YesNo);
                if (dlgRslt == System.Windows.Forms.DialogResult.Yes)
                {
                    InitMotionSys();
                }
            }
            GV.bInitialized = bInitialized;
        }
        //检查单动子平面电机状态
        public void CheckIfInOperation()
        {
            int xbotCount;
            bool bInitialized = false;
            xbotCount = GV.PrintingObj.CheckXbotCount();
            try
            {
                if (xbotCount > 0)
                {
                    bInitialized = true;
                    GV.bInitialized = bInitialized;

                    DialogResult dlgRslt = MessageBox.Show("检测到" + xbotCount.ToString() + " 个动子，是否激活", "激活提示", MessageBoxButtons.YesNo);
                    if (dlgRslt == System.Windows.Forms.DialogResult.Yes)
                    {
                        //激活
                        GV.PrintingObj.ActiveXbot();
                        //获取控制器状态
                        GV.PrintingObj.CheckPMCStatus();
                        if (GV.PMC.pmc_activating == true || GV.PMC.pmc_fullcontrol == true)
                        {
                            GV.PrintingObj.LinearMotion2Center();//移动到中心
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                bInitialized = false;
            }
        }
        public void CheckIfInMultiOperations()
        {
            int xbotCount;
            int[] xbotIDs;
            xbotCount = GV.PrintingObj.CheckXbotCount();
            try
            {
                if (xbotCount > 0)
                {            
                    //激活
                    GV.PrintingObj.ActiveXbot();
                    //获取控制器状态
                    GV.PrintingObj.CheckPMCStatus();
                    
                    if (GV.PMC.pmc_activating == true || GV.PMC.pmc_fullcontrol == true)
                    {
                        //GV.PrintingObj.LinearMotion2Center();//移动到中心
                        //MessageBox.Show("检测到" + xbotCount.ToString() + " 个动子");
                        //动子号赋值
                        GV.PrintingObj.NamingXbotIDs();
                    }             
                }
            }
            catch (Exception ex)
            {
                
            }
        }
        /// <summary>
        /// 初始化三维运动平台
        /// </summary>
        public void InitMotionSys()
        {
            try
            {
                if (!GV.PrintingObj.IsSimulator)
                {
                    //执行buffer进行所有轴回零
                    GV.PrintingObj.Ch.RunBuffer(0);
                }
                else // 如果是仿真器
                {
                    GV.PrintingObj.Ch.RunBuffer(0, "SIMULATOR");
                    //GV.PrintingObj.Ch.RunBuffer(5);
                }
                lblConnectStatus.Text = "正在初始化，请稍候...";
                lblConnectStatus.Refresh();
                StartInitialize();
                btnInitSys.Text = "取消";
            }
            catch (Exception ex)
            {
                lblConnectStatus.Text = "初始化失败";
                StopInitialize(false);
            }
        }

        /// <summary>
        /// 终止初始化buffer
        /// </summary>
        public void CancelInitMotionSys(bool withDialog)
        {
            if (withDialog)
            {
                GV.bInitialized = true;
                lblConnectStatus.Text = "初始化已忽略";
                StopInitialize(true);
            }
            else
            {
                lblConnectStatus.Text = "初始化取消";
                GV.bInitialized = false;
                StopInitialize(false);
            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            switch (timer1.Tag.ToString())
            {
                case "Connecting":
                    timer1.Stop();
                    lblConnectStatus.Text = "正在连接，请稍候...";
                    //btnConnectController_Click(sender, e);
                    // 自动启动连接控制器：
                    rdoConnectController.Checked = true;
                    // 自动启动连接仿真器
                    //rdoConnectSimulator.Checked = true;
                    break;
                case "Initializing":
                    //GV.PrintingObj.Ch.WaitProgramEnd(5, 10000);
                    double percent = GV.PrintingObj.Ch.ReadVariable("Initialized", 0);
                    if (percent >= 100)
                    {
                        lblConnectStatus.Text = "初始化完成 √";
                        StopInitialize(true);
                    }
                    else
                    {
                        int usedTime = (int)(stopwatch.ElapsedMilliseconds * 0.001);
                        if (usedTime > 150)
                        {
                            lblConnectStatus.Text = "初始化超时！请重新尝试初始化。";
                            StopInitialize(false);
                            return;
                        }
                        lblConnectStatus.Text = "正在初始化，请稍候... " + usedTime.ToString();
                        progressBar1.Value = (int)percent;
                    }
                    lblConnectStatus.Refresh();
                    break;
                default:
                    timer1.Stop();
                    break;
            }
        }

        private void StartInitialize()
        {
            timer1.Tag = "Initializing";
            stopwatch.Restart();
            progressBar1.Value = 0;
            progressBar1.Show();
            GV.bInitialized = false;
            GV.frmMain.SetInitStatus(GV.bInitialized);
            timer1.Start();
        }

        private void StopInitialize(bool ifSuccess)
        {
            timer1.Tag = "";
            stopwatch.Stop();
            progressBar1.Hide();
            GV.PrintingObj.StopBuffer(0);
            GV.PrintingObj.Stop();
            GV.bInitialized = ifSuccess;
            GV.frmMain.SetInitStatus(ifSuccess);
            timer1.Stop();
            btnInitSys.Text = "初始化";
        }

        private void lblhulve_DoubleClick(object sender, EventArgs e)
        {
            CancelInitMotionSys(true);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            GV.frmBasicTest.Show();
            GV.frmBasicTest.Activate();
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            GV.PrintingObj.CloseComm();
            MakeConnectedState(ConnectMode.Disconnect);
        }
        private void btnDisConnectPMC_Click(object sender, EventArgs e)
        {
            GV.PrintingObj.ClosePMComm();
            MakeConnectedState(ConnectMode.DisconnectPMController);
        }
        private void btnConnectController_Click(object sender, EventArgs e)
        {
            //if (GV.PrintingObj.IfConnected()) return;
            // 连接到控制器（龙门+平面电机）
            if (GV.PrintingObj.OpenCommEthernet(GV.IpAddr) )//&& GV.PrintingObj.OpenPMCommEthernet(GV.PMC.IpAddress))
            {
                MakeConnectedState(ConnectMode.ConnectController);
            }
            else
            {
                MakeConnectedState(ConnectMode.Disconnect);
                MessageBox.Show("连接ACS失败！");
            }


            //// 连接到仿真器
            //if (GV.PrintingObj.OpenCommEthernet("127.0.0.1"))
            //{
            //    MakeConnectedState(ConnectMode.ConnectSimulator);
            //}
            //else
            //{
            //    MakeConnectedState(ConnectMode.Disconnect);
            //    MessageBox.Show("连接失败！");
            //}
        }
        private void btnConnectPMC_Click(object sender, EventArgs e)
        {
            if ( GV.PrintingObj.OpenPMCommEthernet(GV.PMC.IpAddress))
            {
                MakeConnectedState(ConnectMode.ConnectPMController);
            }
            else
            {
                MakeConnectedState(ConnectMode.Disconnect);
                MessageBox.Show("连接PMC失败！");
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            GV.frmBasicTest.Show();
            GV.frmBasicTest.Activate();
        }

        private void btnSimulator_Click(object sender, EventArgs e)
        {
            // 连接到仿真器
            if (GV.PrintingObj.OpenCommEthernet("127.0.0.1"))
            {
                MakeConnectedState(ConnectMode.ConnectSimulator);
            }
            else
            {
                MakeConnectedState(ConnectMode.Disconnect);
                MessageBox.Show("连接失败！");
            }
        }

      
    }
}
