using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;

namespace AMCP
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
            GV.InitObject();
            GV.InitForms(this);
       
        }

        // ****************************************************************** //
        //   窗体事情响应
        // ****************************************************************** //

        private void mainFormPage_Load(object sender, EventArgs e)
        {
            //GV.frmDiagnose.Show();
            //GV.frmDiagnose.Activate();

            //using (Graphics g = this.CreateGraphics())
            //{
            //    float dpiX = g.DpiX;
            //    float dpiY = g.DpiY;
            //    MessageBox.Show($"当前DPI: {dpiX}x{dpiY}\n缩放因子: {dpiX / 96}");
            //}
        }

        private void mainFormPage_FormClosing(object sender, FormClosingEventArgs e)
        {
            GV.bAllowedClose = true;
            GV.bgWorker.CancelAsync();
            GV.bgWorker2.CancelAsync();
           // GV.monitorWorker.CancelAsync();//主界面关闭，结束任务
            GV.PrintingObj.CloseComm();
           // GV.frmCamera.CloseCamera();
            //Application.Exit();
        }

        // ****************************************************************** //
        //   菜单（ToolStripMenuItem）事件响应
        // ****************************************************************** //
        
        //
        // 一级菜单：打印操作
        // 

        /// <summary>
        /// 平台初始化
        /// </summary>
        private void tsmSysInitialize_Click(object sender, EventArgs e)
        {
            GV.frmPrintGuide.SetCurrentStep(GV.frmPrintStep1);
            GV.frmPrintGuide.Show();
            GV.frmPrintGuide.Activate();
        }

        /// <summary>
        /// 加载路径文件
        /// </summary>
        private void tsmLoadPathFile_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 加载工艺文件
        /// </summary>
        private void tsmLoadTechParaFile_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 打印预览
        /// </summary>
        private void tsmPrintPreview_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 系统设置
        /// </summary>
        private void tsmSysSetting_Click(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 退出平台
        /// </summary>
        private void tsmExit_Click(object sender, EventArgs e)
        {

        }

        //
        // 一级菜单：平台调控
        // 

        /// <summary>
        /// 运动调校
        /// </summary>
        private void tsmMotionCalibrate_Click(object sender, EventArgs e)
        {
            GV.frmMotionAdjust.Show();
            GV.frmMotionAdjust.Activate();
        }

        /// <summary>
        /// 喷头对准
        /// </summary>
        private void tsmNozzleCalibrate_Click(object sender, EventArgs e)
        {
            GV.frmNozzleCalibrate.Show();
            GV.frmNozzleCalibrate.Activate();
        }

        /// <summary>
        /// 基本测试
        /// </summary>
        private void tsmBasicTest_Click(object sender, EventArgs e)
        {

        }

        //
        // 一级菜单：状态查看
        // 

        /// <summary>
        /// 运动监测
        /// </summary>
        private void tsmMotionMonitor_Click(object sender, EventArgs e)
        {

            GV.frmStatusMonitor.Show();
            GV.frmStatusMonitor.Activate();
        }

        /// <summary>
        /// 路径跟踪
        /// </summary>
        private void tsmPathDisplay_Click(object sender, EventArgs e)
        {
            GV.frmPathTrace.Show();
            GV.frmPathTrace.Activate();
        }


        /// <summary>
        /// 打印状态
        /// </summary>
        private void tsmPrintStatus_Click(object sender, EventArgs e)
        {

        }

        //
        //  一级菜单：系统帮助
        //

        /// <summary>
        /// 用户手册
        /// </summary>
        private void tsmUserManual_Click(object sender, EventArgs e)
        {
            OpenUserManuel();
        }

        private void OpenUserManuel()
        {
            try
            {
                System.Diagnostics.Process.Start(Application.StartupPath + "\\Documents\\使用手册.pdf");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            } 
        }

        /// <summary>
        /// 技术支持
        /// </summary>
        private void tsmTechSupport_Click(object sender, EventArgs e)
        {
            MessageBox.Show("如有任何技术问题，欢迎联系江南大学机械工程学院iPISE团队。\r\n\r\n电话: 0510-85916656\r\n\r\n手机: 18021567640\r\n\r\nEmail: cyqleaf@qq.com", "技术支持", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 关于软件
        /// </summary>
        private void tsmAbout_Click(object sender, EventArgs e)
        {
            new frmAbout().ShowDialog();
        }

        // ****************************************************************** //
        //   工具按钮事件响应
        // ****************************************************************** //

        /// <summary>
        /// 打印向导
        /// </summary>
        private void tsbPrintGuide_Click(object sender, EventArgs e)
        {
            DelayShowForm(GV.frmPrintGuide, 0);
            //Point loc = GV.frmPrintGuide.Location;
            //GV.frmPrintGuide.Show();
            //GV.frmPrintGuide.Activate();
            //GV.frmPrintGuide.Location = loc;
        }

        private void OpenMotionForms()
        {
            tsbMotionCalibrate_Click(this, null);
        }

        /// <summary>
        /// 基本测试
        /// </summary>
        private void tsbBasicTests_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(@"D:\UG10.0\UGII\ugraf.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //GV.frmBasicTest.Show();
            //GV.frmBasicTest.Activate();
        }

        /// <summary>
        /// 样品资料
        /// </summary>
        private void tsbSampleReview_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Application.StartupPath + "\\Photos");
        }

        // ****************************************************************** //
        //   自定义方法
        // ****************************************************************** //

        /// <summary>
        /// 获取已执行指令条数
        /// </summary>
        /// <returns></returns>
        public int GetCountCmdExcuted()
        {
            int countCmdSent = GV.dataManagementObj.countCmdExecuted;
            int countCmdInBuff;                     // 缓冲区剩余指令条数
            int countCmdExcuted;                    // 已执行指令条数
            int gSeg = GV.PrintingObj.GetGSeg();       // 当前已执行的Segment分段数（-1表示不在Segment中）
            int gFree = GV.PrintingObj.GetGSFree();    // 缓冲区空闲指令条数 = 48 - 缓冲区剩余指令条数
            if (gSeg != -1)
            {
                countCmdInBuff = 48 - gFree;
            }
            else
            {
                countCmdInBuff = 0;
            }
            countCmdExcuted = countCmdSent - countCmdInBuff;
            return countCmdExcuted;
        }



        public void SetConnectStatus(ConnectMode mode)
        {
            GV.connMode = mode;
            switch (mode)
            {
                case ConnectMode.Disconnect:
                    tsslControllerConnected.Image = imageList1.Images[1];   //状态栏控制器是否连接显示
                    tsslControllerConnected.Text = " 控制器未连接";
                    tsslControllerConnected.ForeColor = Color.FromArgb(0xff, 0x0, 0x0); //0xff0000;
                    SetInitStatus(false);
                    break;                                                
                case ConnectMode.ConnectController:
                    tsslControllerConnected.Image = imageList1.Images[0];   //状态栏控制器是否连接显示
                    tsslControllerConnected.Text = " 已连接ACS控制器";
                    tsslControllerConnected.ForeColor = Color.FromArgb(0, 65, 152); //0x004198;
                    SetInitStatus(GV.bInitialized);
                    break;
                case ConnectMode.ConnectSimulator:
                    tsslControllerConnected.Image = imageList1.Images[0];
                    tsslControllerConnected.Text = " 已连接仿真器";
                    tsslControllerConnected.ForeColor = Color.FromArgb(0, 65, 152); //0x004198;
                    SetInitStatus(GV.bInitialized);
                    break;
                case ConnectMode.ConnectControllerWithLS:
                    tsslControllerConnected.Image = imageList1.Images[0];
                    tsslControllerConnected.Text = " 已连接雷赛器";
                    tsslControllerConnected.ForeColor =  Color.FromArgb(0, 65, 152); //0x004198;
                    SetInitStatus(GV.bInitialized);
                    break;
                default:
                    break;
            }
        }

        public void UpdatePrintPercent(int percent)
        {
            tsslPrintingPercent.Value = percent;
        }
        //开始打印
        public void SetPrintStatus(bool isPrinting, string strShowText = "正在打印...")
        {
            if (isPrinting)
            {
                tsslPrintingPercent.Visible = true;
                tsslPrinting.Visible = true;
            }
            else
            {
                tsslPrintingPercent.Visible = false;
                tsslPrinting.Visible = false;
            }
            tsslPrinting.Text = strShowText;
        }
        public void UpdatePMCStatus(StageStatus status)
        {
            toolStripStatusLabelPMC.Text = status.PMCStatus;
        }
        public void SetInitStatus(bool isInitialized)
        {
            if (isInitialized)
            {
                toolStripStatusLabel2.Text = "初始化完成";
                //toolStripStatusLabel2.ForeColor = Color.FromArgb(0, 65, 152);
                toolStripStatusLabel2.ForeColor = Color.Black;
                //tsbBasicTests.Enabled = true;           // 三轴回零按钮使能开启
                //tsbShortcuts.Enabled = true;            // 快捷打印按钮使能开启
                tsbLoadPathFile.Enabled = true;         // 加载路径按钮使能开启
                tsbLoadTechParaFile.Enabled = true;     // 加载工艺按钮使能开启
                tsbHomeAllAxes.Enabled = true;          // 三轴回零按钮使能开启
                tsbMotionCalibrate.Enabled = true;      // 运动调校按钮使能开启
                tsbNozzleCalibrate.Enabled = true;      // 喷头对准按钮使能开启
                tsbPathDisplay.Enabled = true;          // 运动轨迹按钮使能开启
                tsbPrintStatus.Enabled = true;          // 状态监测按钮使能开启
                //tsbBasicTests.Enabled = true;           // 基本测试按钮使能开启
                //tsbShortcuts.Enabled = true;            // 快捷打印按钮使能开启
                //DelayExcute(new MsgHandler(OpenMotionForms));
                DelayCall(OpenMotionForms_Tick, 1000);
            }
            else
            {
                toolStripStatusLabel2.Text = "尚未初始化";
                toolStripStatusLabel2.ForeColor = Color.Red;
                //tsbBasicTests.Enabled = false;          // 三轴回零按钮使能关闭
                //tsbShortcuts.Enabled = false;           // 快捷打印按钮使能关闭
                tsbLoadPathFile.Enabled = false;        // 加载路径按钮使能关闭
                tsbLoadTechParaFile.Enabled = false;    // 加载工艺按钮使能关闭
                tsbHomeAllAxes.Enabled = false;         // 三轴回零按钮使能关闭
                tsbMotionCalibrate.Enabled = false;     // 运动调校按钮使能关闭
                tsbNozzleCalibrate.Enabled = false;     // 喷头对准按钮使能关闭
                tsbPathDisplay.Enabled = false;         // 运动轨迹按钮使能关闭
                tsbPrintStatus.Enabled = false;         // 状态监测按钮使能关闭
                //tsbBasicTests.Enabled = false;          // 基本测试按钮使能关闭
                //tsbShortcuts.Enabled = false;           // 快捷打印按钮使能关闭
            }
        }

        System.Windows.Forms.Timer tmr;
        delegate void FuncHandler(object sender, EventArgs e);

        public void SetPressureConnected(bool isConnected)
        {
            if (isConnected)
            {
                toolStripStatusLabel3.Text = "点胶机连接成功";
                toolStripStatusLabel3.ForeColor = Color.Black;
            }
            else
            {
                toolStripStatusLabel3.Text = "点胶机未连接";
                toolStripStatusLabel3.ForeColor = Color.Red;
            }
        }

        public void SetRotaryValveConnected(bool isConnected)
        {
            if (isConnected)
            {
                toolStripStatusLabel4.Text = "螺杆阀连接成功";
                toolStripStatusLabel4.ForeColor = Color.Black;
            }
            else
            {
                toolStripStatusLabel4.Text = "螺杆阀未连接";
                toolStripStatusLabel4.ForeColor = Color.Red;
            }
        }

        public void SetNozzleSensorConnected(bool isConnected)
        {
            if (isConnected)
            {
                toolStripStatusLabel5.Text = "激光测距连接成功";
                toolStripStatusLabel5.ForeColor = Color.Black;
            }
            else
            {
                toolStripStatusLabel5.Text = "激光测距未连接";
                toolStripStatusLabel5.ForeColor = Color.Red;
            }
        }
        public void SetWeightControlConnected(bool isConnected)
        {
            if (isConnected)
            {
                toolStripStatusLabel6.Text = "称重模块连接成功";
                toolStripStatusLabel6.ForeColor = Color.Black;
            }
            else
            {
                toolStripStatusLabel6.Text = "称重模块未连接";
                toolStripStatusLabel6.ForeColor = Color.Red;
            }
        }
        private void DelayCall(FuncHandler f, int delayMsec)//, object tag = null)
        {
            tmr = new System.Windows.Forms.Timer();
            tmr.Tick += new EventHandler(f);
            tmr.Interval = delayMsec > 0 ? delayMsec : 1;
            //tmr.Tag = tag;
            tmr.Start();
        }

        private void DelayShowForm(Form form, int delayMsec)
        {
            tmr = new System.Windows.Forms.Timer();
            tmr.Tick += new EventHandler(ShowForm_Tick);
            tmr.Interval = delayMsec > 0 ? delayMsec : 1;
            tmr.Tag = form;
            tmr.Start();
        }

        private void OpenMotionForms_Tick(object sender, EventArgs e)
        {
            tsbMotionCalibrate_Click(this, null);
            tsbRotaryValve_Click(this, null);
            //tsbWeightCtrl_Click(this, null);//无称重模块
            tmr.Stop();
        }

        private void ShowForm_Tick(object sender, EventArgs e)
        {
            Form frm = (sender as System.Windows.Forms.Timer).Tag as Form;
            //Point loc = frm.Location;
            frm.Show();
            frm.Activate();
            //frm.Location = loc;
            tmr.Stop();
        }

        private void tsbSysHelp_Click(object sender, EventArgs e)
        {
            OpenUserManuel();
        }

        private void tsbExperimentsLog_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(@"D:\Program Files\Ultimaker Cura 4.8.0\Cura.exe");
                //System.Diagnostics.Process.Start(@"D:\Program Files\Ultimaker Cura 3.4\Cura.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            } 
            //(new FrmReviewExpLog()).Show();
        }
        //打开曲面打印exe
        private void tsbShortcuts_Click(object sender, EventArgs e)
        {
            try
            {
                //System.Diagnostics.Process.Start(Application.StartupPath + "\\PathEditor" + "\\PathEditor.exe");
                //xlm
                System.Diagnostics.Process.Start(Application.StartupPath + "\\PathEditor" + "\\PathEditor.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            //(new FrmShortcut()).Show();
        }

        private void tsbLoadPathFile_Click(object sender, EventArgs e)
        {
            GV.frmPrintGuide.Show();
            GV.frmPrintGuide.Activate();
            GV.frmPrintGuide.SetCurrentStep(GV.frmPrintStep2);
        }

        private void tsbLoadTechParaFile_Click(object sender, EventArgs e)
        {
            GV.frmPrintGuide.Show();
            GV.frmPrintGuide.Activate();
            GV.frmPrintGuide.SetCurrentStep(GV.frmPrintStep3);
        }

        private void tsbHomeAllAxes_Click(object sender, EventArgs e)
        {
            GV.frmPrintGuide.Show();
            GV.frmPrintGuide.Activate();            
            GV.frmPrintGuide.SetCurrentStep(GV.frmPrintStep1);
        }

        private void tsbMotionCalibrate_Click(object sender, EventArgs e)
        {
            GV.frmStatusMonitor.Show();
            GV.frmStatusMonitor.Activate();
            GV.frmMotionAdjust.Show();
            GV.frmMotionAdjust.Activate();
            GV.frmSetPressure.Show();
            GV.frmSetPressure.Activate();

            //DelayShowForm(GV.frmStatusMonitor, 0);
            //DelayShowForm(GV.frmMotionAdjust, 0);
        }

        private void tsbNozzleCalibrate_Click(object sender, EventArgs e)
        {
            GV.frmNozzleCalibrate.Show();
            GV.frmNozzleCalibrate.Activate();
        }

        public void EnableNozzleCalibrate()
        {
            tsbNozzleCalibrate.Enabled = true;
        }

        public void DisableNozzleCalibrate()
        {
            GV.frmNozzleCalibrate.Hide();
            tsbNozzleCalibrate.Enabled = false;
        }

        private void tsbPathDisplay_Click(object sender, EventArgs e)
        {
            GV.frmPathTrace.Show();
            GV.frmPathTrace.Activate();

        }

        private void tsbPrintStatus_Click(object sender, EventArgs e)
        {
            GV.frmStatusMonitor.Show();
            GV.frmStatusMonitor.Activate();
        }
        //按空格
        private void FrmMain_KeyDown(object sender, KeyEventArgs e)
        {
            //this.Text = e.KeyCode.ToString() + "  " + e.KeyValue.ToString();
            try
            {
                switch (e.KeyCode)
                {
                    case Keys.Space:
                        //case Keys.Escape:
                        e.SuppressKeyPress = false;
                        GV.StopImmediately();
                        GV.frmImmStop.Activate();
                        GV.frmImmStop.TwinkleButton(3);
                        return;
                }
                if (GV.PrintingObj.bConnected && !GV.PrintingObj.IsPrinting)
                {
                    GV.frmMotionAdjust.FrmMotionAdjust_KeyDown(sender, e);
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void FrmMain_KeyUp(object sender, KeyEventArgs e)
        {
            if (GV.PrintingObj.bConnected && !GV.PrintingObj.IsPrinting)
            {
                GV.frmMotionAdjust.FrmMotionAdjust_KeyUp(sender, e);
            }
        }

        private void tsmSysDiagnose_Click(object sender, EventArgs e)
        {
            GV.frmDiagnose.Show();
            GV.frmDiagnose.Activate();
        }

        private void tsmOpenCamera_Click(object sender, EventArgs e)
        {
            //GV.frmCamera.Show();
            //GV.frmCamera.Activate();、
            //打开大恒相机
        }

        private void tsmPressureSet_Click(object sender, EventArgs e)
        {
            GV.frmSetPressure.Show();
            GV.frmSetPressure.Activate();
        }

        private void tsmTemperature_Click(object sender, EventArgs e)
        {
            GV.frmTemperature.Show();
            GV.frmTemperature.Activate();
        }

        private void tsmDCPower_Click(object sender, EventArgs e)
        {
            GV.frmDCPower.Show();
            GV.frmDCPower.Activate();
        }

        private void tsmSyringePump_Click(object sender, EventArgs e)
        {
            GV.frmSyringePump.Show();
            GV.frmSyringePump.Activate();
        }

        private void tsmBasicTest_Click_1(object sender, EventArgs e)
        {
            GV.frmBasicTest.Show();
            GV.frmBasicTest.Activate();
        }

        private void FrmtsmTemptTableCtrl_Click(object sender, EventArgs e)
        {
            GV.frmTemptTableCtrl.Show();
            GV.frmTemptTableCtrl.Activate();
        }

        private void tsbPrintPathPoints_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(@"D:\AMCP\AMCP\AMCP\bin\Debug\JumpG0\JumpG0.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tsmRotSet_Click(object sender, EventArgs e)
        {
            GV.frmRotaryValveCtrl.Show();
            GV.frmRotaryValveCtrl.Activate();
        }

        private void tsbSetPressure_Click(object sender, EventArgs e)
        {
            GV.frmSetPressure.Show();
            GV.frmSetPressure.Activate();
        }

        private void tsbRotaryValve_Click(object sender, EventArgs e)
        {
            GV.frmRotaryValveCtrl.Show();
            GV.frmRotaryValveCtrl.Activate();
        }

        private void tsbWeightCtrl_Click(object sender, EventArgs e)
        {
            GV.frmWeightCtrl.Show();
            GV.frmWeightCtrl.Activate();
        }
    }
}
