using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace AMCP
{
    public partial class FrmPathRun : Form
    {
        public FrmPathRun()
        {
            InitializeComponent();
            //InitArray();
            //ResetData();
        }

        string fileName = Application.StartupPath + @"\pathPoints.csv";

        private void btnGetStatus_Click(object sender, EventArgs e)
        {
            index = 0;
            timer1.Start();
        }

        double[] xx;
        double[] yy;
        double[] zz;
        double[] ee;

        //添加蜂鸣器
        int[] ww;
        //添加气压
        double[] pp;
        int pointCount = 11943;
        int lenArr = 1000;


        bool isLoading = false;
        int iFinish = 1;    // 最近一次写入完成的数组（1或2）
        int index = 0;      // 已写入的数据序号
        int iBuf = 3;       // iBuf = 9;曲面打印变量存储buffer


        private void ResetData()
        {
            try
            {
                GV.PrintingObj.Ch.StopBuffer(iBuf);
                WriteVar("sArr1", 0);
                WriteVar("sArr2", 0);
                WriteVar("countPoints", 0);
                WriteVar("dt", 20);//20ms执行一条
                WriteVar("begin", 0);
                index = 0;
                WriteData(1);
                GV.PrintingObj.Ch.RunBuffer(iBuf, "RUN");
                timer1.Start();
                //监测线程
                backgroundWorker1.RunWorkerAsync();
                //监测执行指令
                //GV.monitorWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
            }
        }
        public void RestartTimer1()
        {
            timer1.Stop();
            backgroundWorker1.CancelAsync();
            //timer2.Stop();
            // System.Threading.Thread.Sleep(100);
            timer1.Start();
            backgroundWorker1.RunWorkerAsync();
            //timer2.Start();
        }

        private void btnExcute_Click(object sender, EventArgs e)
        {
            WriteVar("begin", 1);
            wlast = -1;
            GV.PrintingObj.IsPrinting = true;//
            // GV.monitorWorker.RunWorkerAsync();//开始监控
        }

        //运行
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                //按下空格停止
                if (!GV.pathprinting)
                {
                    button10_Click(sender, e);
                    GV.PrintingObj.SetExtrudePorts(-2, 0);
                    return;
                }
                int offset = Convert.ToInt32(numericUpDown1.Value);//往前数的点
                int gseg = ReadVar("GSEG", GV.X, GV.X);//当前缓冲区执行到的指令数
                if (gseg >= pointCount - 1)//最后一条停止
                {
                    WriteVar("begin", 0);
                    timer1.Stop();
                    return;
                }

                //int iSegCur = gseg - 4;//当前指令
                //if (iSegCur < 0) iSegCur = 0;
                //int iSegOff = gseg + offset;
                //if (iSegOff >= pointCount)
                //{
                //    iSegOff = pointCount - 1;
                //}
                //if (ee[iSegCur] == 0 && ee[iSegOff] == 0)
                //{
                //    GV.PrintingObj.SetExtrudePorts(-2, 0);
                //}
                //else if (ee[iSegCur] == 0 && ee[iSegOff] == 1)
                //{
                //    GV.PrintingObj.SetExtrudePorts(-2, 1);
                //}
                //else if (ee[iSegCur] == 1 && ee[iSegOff] == 1)
                //{
                //    GV.PrintingObj.SetExtrudePorts(-2, 1);
                //}
                //else if (ee[iSegCur] == 1 && ee[iSegOff] == 0)
                //{
                //    //GV.PrintingObj.SetExtrudePorts(-2, 0);
                //}

                int countPoints = ReadVar("countPoints");
                int sArr1 = ReadVar("sArr1");
                int sArr2 = ReadVar("sArr2");
                int begin = ReadVar("begin");
                int iPoint = ReadVar("iPoint");
                label1.Text = "GSEG = " + gseg.ToString() + ";  countPoint = " + countPoints.ToString() + ";  sArr1 = " + sArr1.ToString() + ";  sArr2 = " + sArr2.ToString()
                    + ";  begin = " + begin.ToString() + ";  iPoint = " + iPoint.ToString(); //显示线程

                // Path开始前准备阶段
                if (sArr1 == 0 && sArr2 == 0 && begin == 0)
                {
                    lblValue.Text = "请先复位";
                    textBox1.ForeColor = Color.Black;
                    textBox2.ForeColor = Color.Black;
                }

                // Arr1准备完毕
                if (sArr1 == 1 && sArr2 == 0 && begin == 0)
                {
                    lblValue.Text = "Arr1准备完毕，请点击“执行打印”开始";
                    textBox1.ForeColor = Color.Black;
                    textBox2.ForeColor = Color.Black;
                }

                // Path正在执行中，目前正在使用Arr1的数据
                if (sArr1 == 2 && sArr2 == 0 && begin == 1)
                {
                    lblValue.Text = "Path正在执行中，目前正在使用Arr1的数据；同时正在为Arr2写入数据";
                    textBox1.ForeColor = Color.Green;
                    textBox2.ForeColor = Color.Black;
                    if (index < pointCount - 1)
                    {
                        WriteData(2);
                        sArr2 = 1;
                    }
                    else
                    {
                        sArr1 = 0;
                        sArr2 = 0;
                        WriteVar("sArr1", 0);
                        WriteVar("sArr2", 0);
                    }
                }

                // Path正在执行中，目前正在使用Arr2的数据
                if (sArr1 == 0 && sArr2 == 2 && begin == 1)
                {
                    lblValue.Text = "Path正在执行中，目前正在使用Arr2的数据；同时正在为Arr1写入数据";
                    textBox1.ForeColor = Color.Black;
                    textBox2.ForeColor = Color.Green;
                    if (index < pointCount - 1)
                    {
                        WriteData(1);
                        sArr1 = 1;
                    }
                    else
                    {
                        sArr1 = 0;
                        sArr2 = 0;
                        WriteVar("sArr1", 0);
                        WriteVar("sArr2", 0);
                    }
                }

                // Path已全部执行完毕
                if (sArr1 == 0 && sArr2 == 0 && begin != 0)
                {
                    if (begin == 1)
                    {
                        lblValue.Text = "全部数据已加载完成。";
                    }
                    else if (begin == 2)
                    {
                        lblValue.Text = "执行完毕。";
                        timer1.Stop();
                        backgroundWorker1.CancelAsync();
                        //监测指令执行
                       // GV.monitorWorker.CancelAsync();

                    }
                }
            }
            catch (Exception ex)
            {
                lblValue.Text = "";
            }

        }

        private void WriteVar(string varName, double value)
        {
            GV.PrintingObj.Ch.WriteVariable(value, varName, iBuf);
        }

        private dynamic ReadVar(string varName, int From1 = -1, int To1 = -1, int From2 = -1, int To2 = -1)
        {
            return GV.PrintingObj.Ch.ReadVariable(varName, iBuf, From1, To1, From2, To2);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            WriteData(1);
        }


        private void button2_Click(object sender, EventArgs e)
        {
            WriteData(2);
        }
        //分两个数据
        private void WriteData(int iArr)
        {
            try
            {
                double[] x;
                double[] y;
                double[] z;
                double[] e;
                string str = "";
                if (pointCount - index >= lenArr)
                {
                    x = new double[lenArr];
                    y = new double[lenArr];
                    z = new double[lenArr];
                    e = new double[lenArr];
                }
                else if (pointCount - index > 0)
                {
                    x = new double[pointCount - index];
                    y = new double[pointCount - index];
                    z = new double[pointCount - index];
                    e = new double[pointCount - index];
                }
                else
                {
                    if (iArr == 1)
                    {
                        WriteVar("sArr1", 0);
                        GV.MsgShow(lblNotice1, "Arr1数据已全部加载完毕。", timer2, 3000);
                    }
                    else if (iArr == 2)
                    {
                        WriteVar("sArr2", 0);
                        GV.MsgShow(lblNotice1, "Arr2数据已全部加载完毕。", timer2, 3000);
                    }
                    return;
                }

                for (int i = 0; i < x.Length; i++)
                {
                    x[i] = xx[index];
                    y[i] = yy[index];
                    z[i] = zz[index];
                    e[i] = ee[index];
                    str += "Point " + index.ToString() + ": (" + x[i].ToString("0.000") + ", " + y[i].ToString("0.000") + ", " + z[i].ToString("0.000") + ")\r\n";
                    index++;
                }
                if (iArr == 1)
                {
                    GV.PrintingObj.Ch.WriteVariable(x, "xx", iBuf);
                    GV.PrintingObj.Ch.WriteVariable(y, "yy", iBuf);
                    GV.PrintingObj.Ch.WriteVariable(z, "zz", iBuf);
                    GV.PrintingObj.Ch.WriteVariable(e, "ee", iBuf);
                    GV.PrintingObj.Ch.WriteVariable(x.Length, "lenArr1", iBuf);
                    textBox1.Text = str;
                    GV.MsgShow(lblNotice1, "Arr1数据已准备就绪。", timer2, 3000);
                    WriteVar("sArr1", 1);
                }
                else if (iArr == 2)
                {
                    GV.PrintingObj.Ch.WriteVariable(x, "xx2", iBuf);
                    GV.PrintingObj.Ch.WriteVariable(y, "yy2", iBuf);
                    GV.PrintingObj.Ch.WriteVariable(z, "zz2", iBuf);
                    GV.PrintingObj.Ch.WriteVariable(e, "ee2", iBuf);
                    GV.PrintingObj.Ch.WriteVariable(x.Length, "lenArr2", iBuf);
                    textBox2.Text = str;
                    GV.MsgShow(lblNotice2, "Arr2数据已准备就绪。", timer2, 3000);
                    WriteVar("sArr2", 1);
                }
                if (index >= pointCount)
                {
                    //WriteVar("begin", 0);
                    //GV.PrintingObj.Ch.WriteVariable(1, "finished", iBuf);
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        double xTop = 72.9, yTop = 42.2, zTop = 35.45;
        double xFirst = 0, yFirst = 0, zFirst = 0;
        private void InitArray()
        {
            //更新顶点坐标
            double xCur = GV.PrintingObj.Status.fPosX;
            double yCur = GV.PrintingObj.Status.fPosY;
            double zCur = GV.PrintingObj.Status.fPosZ;
            if (xTop != xCur || yTop != yCur || zTop != zCur)
            {
                //点击确定
                if (DialogResult.Yes == MessageBox.Show("是否将当前位置(" + xCur.ToString("0.0000") + ", " + yCur.ToString("0.0000") + ", " + zCur.ToString("0.0000") + ")设为新的顶点位置？\r\n"
                    + "现在的顶点坐标为(" + xTop.ToString("0.0000") + ", " + yTop.ToString("0.0000") + ", " + zTop.ToString("0.0000") + ").", "提示", MessageBoxButtons.YesNo))
                {
                    xTop = xCur;
                    yTop = yCur;
                    zTop = zCur;
                }
            }

            string strCSV;//存储文件内容
            string[] strLines;//存储分隔后的每行数组
            try
            {
                strCSV = GV.ReadTextFile(fileName);
                strLines = strCSV.Split(new char[2] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);//分割后的数据

                pointCount = strLines.Length;
                //分别存储每列读取到的数据
                xx = new double[pointCount];
                yy = new double[pointCount];
                zz = new double[pointCount];
                ee = new double[pointCount];
                //（新增）工艺参数
                ww = new int[pointCount];
                pp = new double[pointCount];
                string[] strPoint;
                //遍历每一列
                for (int i = 0; i < pointCount; i++)
                {
                    strPoint = strLines[i].Split(',');//分隔数据
                    //if( i == 0 )
                    //{
                    //    xFirst = Convert.ToDouble(strPoint[0]);yFirst = Convert.ToDouble(strPoint[1]);zFirst = Convert.ToDouble(strPoint[2]);           
                    //}
                    xx[i] = Convert.ToDouble(strPoint[0]) + xTop;// 点+当前位置
                    yy[i] = Convert.ToDouble(strPoint[1]) + yTop;// 
                    zz[i] = Convert.ToDouble(strPoint[2]) + zTop;
                    ee[i] = Convert.ToDouble(strPoint[3]);//出气关气
                    // 如果列数大于 4，则继续读取
                    if (strPoint.Length >= 5)
                    {
                        // 新增工艺参数
                        ww[i] = Convert.ToInt32(strPoint[4]);
                        if (strPoint.Length >= 6)//再加一列
                        {
                            pp[i] = Convert.ToDouble(strPoint[5]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
        }
        //复位
        private void button9_Click(object sender, EventArgs e)
        {
            InitArray();//确认起点导入pathpoints文件
            GV.pathprinting = true;//曲面打印
            ResetData();//向下位机写入
            wlast = -1;
        }
        //停止
        public void button10_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            backgroundWorker1.CancelAsync();
            //监测指令
            //GV.monitorWorker.CancelAsync();
            wlast = -1;
            try
            {
                GV.PrintingObj.Ch.StopBuffer(iBuf);
                WriteVar("sArr1", 0);
                WriteVar("sArr2", 0);
                WriteVar("countPoints", 0);
                WriteVar("dt", 20);
                WriteVar("begin", 0);
                index = 0;
                //停气
                GV.StopImmediately();
            }
            catch (Exception ex)
            {
                GV.PrintingObj.Ch.StopBuffer(iBuf);
                GV.StopImmediately();
            }
        }

        private void btnBrowsePathFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Path files (*.path)|*.path|CSV files (*.csv)|*.csv"; //"CSV files (*.csv)|*.csv|GCode files (*.gcode)|*.gcode";
            openFileDialog1.DefaultExt = "path";
            openFileDialog1.FileName = "";
            DialogResult dlgRslt;
            dlgRslt = openFileDialog1.ShowDialog();
            if (dlgRslt == DialogResult.OK)
            {
                txtGcodeFileName.Text = openFileDialog1.FileName;
                fileName = openFileDialog1.FileName;
            }
        }

        private void FrmPathRun_Load(object sender, EventArgs e)
        {
            txtGcodeFileName.Text = fileName;
        }

        //检测gseg执行到的位置，向亚当模块修改气压
        private void timpress_Tick(object sender, EventArgs e)
        {
            try
            {
                //按下空格停止
                if (!GV.pathprinting)
                {
                    button10_Click(sender, e);
                    GV.PrintingObj.SetExtrudePorts(-2, 0);
                    return;
                }
                int offset = Convert.ToInt32(numericUpDown1.Value);
                int gseg = ReadVar("GSEG", GV.X, GV.X);//当前缓冲区执行到的指令数
                if (gseg >= pointCount - 1)//最后一条停止
                {
                    WriteVar("begin", 0);
                    timer1.Stop();
                    return;
                }
                int iSegCur = gseg - 4;//当前指令
            }
            catch (Exception ex)
            {

            }
        }

        private void timerWarn_Tick(object sender, EventArgs e)
        {
            int gseg = ReadVar("GSEG", GV.X, GV.X);//已经执行的点数
            if (gseg >= pointCount - 1)
            {
                WriteVar("begin", 0);
                timer1.Stop();
                timerWarn.Stop();
                return;
            }

            int countPoints = ReadVar("countPoints");//上位机发送的点数
            int iPoint = ReadVar("iPoint");//下位机插补进的点数
            int begin = ReadVar("begin");

            //label2.Text = "GSEG:" + gseg.ToString() + " countPoint:" + countPoints.ToString() + " iPoint:" + iPoint.ToString();

            //int gsegCur = gsegCur + 1;
            //指定OUT口启闭
            if (begin == 1 && gseg > -1)
            {
                if (ww[gseg] == 0)
                {
                    GV.PrintingObj.SetExtrudePorts(5, 0);
                }
                else
                {
                    GV.PrintingObj.SetExtrudePorts(5, 1);
                }
                //  GV.PrintingObj.SetExtrudePorts(5, ww[gseg]);
            }
        }
        //监测线程
        int Gseg = 0;
        int preGsegStart = 0;
        int preGsegEnd = 0;
        int CountPoints = 0;
        int IPoint = 0;
        int Begin = 0;
        int prePointsStart = 0;
        int prePointsEnd = 0;
        int wlast = -1;//记录蜂鸣器的标志符
        int elast = -1;//记录出丝的标识符

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            // 使用更合理的间隔时间
            const int MonitorInterval = 500;
            const int ProcessingInterval = 20; // 适当增加处理间隔减少CPU负载
            DateTime lasMonitorTime = DateTime.Now;
            DateTime lastProcessingTime = DateTime.Now;
            try
            {
                while (!backgroundWorker1.CancellationPending)
                {
                    //每次都检查是否取消
                    if (backgroundWorker1.CancellationPending)
                    {
                        //worker.ReportProgress(-1);
                        e.Cancel = true;
                        break;
                    }
                    else
                    {
                        // 控制处理频率
                        if ((DateTime.Now - lastProcessingTime).TotalMilliseconds < ProcessingInterval)
                        {
                            Thread.Sleep(1); // 更短的睡眠时间，响应更快
                            continue;
                        }
                        lastProcessingTime = DateTime.Now;

                        Gseg = ReadVar("GSEG", GV.X, GV.X);//已经执行的点数
                        Begin = ReadVar("begin");

                        //出丝往前计数
                        preGsegStart = Gseg + prePointsStart;
                        preGsegEnd = Gseg + prePointsEnd;
                        //gseg = GV.PrintingObj.Status.GSEG;
                        //是否结束
                        if (preGsegEnd >= pointCount - 1)
                        {
                            WriteVar("begin", 0);
                            StopPathPrint();
                            return;
                        }
                        //出丝
                        if (Begin == 1 && preGsegEnd > -1 && preGsegStart > -1)//开始读取
                        {
                            ProcessExtrude(preGsegEnd, preGsegStart, Gseg);
                        }
                    }
                    //清理空间
                    if ((DateTime.Now - lasMonitorTime).TotalMilliseconds > MonitorInterval)
                    {
                        long memoryUsed = GC.GetTotalMemory(false);
                        if (memoryUsed > 25 * 1024 * 1024)
                        {
                            GC.Collect();
                            GC.WaitForPendingFinalizers();
                        }
                        lasMonitorTime = DateTime.Now;
                    }
                    Thread.Sleep(50);
                }
            }
            catch (Exception ex)
            {
                // 记录错误日志
                Debug.WriteLine($"Background worker error: {ex.Message}");
                e.Cancel = true;
            }
        }

        //出丝
        private void ProcessExtrude(int OffGseg, int OnGseg, int CurrentGseg)
        {
            if (ee[OffGseg] == 0)
            {
                ProcessWarn(OffGseg);
                GV.PrintingObj.SetExtrudePorts(GV.portExtrude, 0);
                elast = 0;
            }
            else if (ee[OnGseg] != elast)//出丝阶段
            {
                GV.PrintingObj.SetExtrudePorts(GV.portExtrude, 1);
                //  GV.PrintingObj.SetExtrudePorts( 5, 0);               
                wlast = 0;
                elast = 1;
            }
            else if (elast == 1 && pp[preGsegStart] != pp[preGsegStart - 1])//变气压
            {
                GV.frmSetPressure.SetPressure(0, pp[preGsegStart]);
            }
        }
        // 蜂鸣
        private void ProcessWarn(int offGseg)
        {
            if (wlast == -1 || ww[offGseg] != wlast)
            {
                int buzzerState = ww[offGseg] == 1 ? 1 : 0;
                if (!GV.Scilence)
                {
                    GV.PrintingObj.SetExtrudePorts(5, buzzerState);
                }
                GV.PrintingObj.SetExtrudePorts(5, buzzerState);
                wlast = buzzerState;
            }
        }

        //停止
        private void StopPathPrint()
        {
            preGsegEnd = pointCount - 1;
            //停气停螺杆阀
            WriteVar("begin", 0);
            timer1.Stop();
            backgroundWorker1.CancelAsync();
            //GV.monitorWorker.CancelAsync();
            GV.PrintingObj.IsPrinting = false;
            GV.PrintingObj.SetExtrudePorts(GV.portExtrude, 0);//停气或停转
            return;
        }

        private void numPreTimeStart_ValueChanged(object sender, EventArgs e)
        {
            prePointsStart = (int)(numPreTimeStart.Value / 20);
        }

        private void btnpathpointPreview_Click(object sender, EventArgs e)
        {

        }

        private void numPreTimeEnd_ValueChanged(object sender, EventArgs e)
        {
            prePointsEnd = (int)(numPreTimeEnd.Value / 20);
        }
        private void button11_Click(object sender, EventArgs e)
        {
            ResetData();
        }
        private void FrmBasicTest_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = (e.CloseReason == CloseReason.UserClosing);
            this.Hide();
        }
        private void timer2_Tick(object sender, EventArgs e)
        {

        }
    }
}


