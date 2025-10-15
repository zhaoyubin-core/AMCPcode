using MySql.Data.MySqlClient.Authentication;
using System;//引入基本命名空间
using System.Collections;//引入集合接口和类的非泛型版本，String,
using System.Collections.Generic;//引入接口和类的泛型版本
using System.Drawing;//图形处理的类
using System.IO.Ports;//引入串口通信
using System.Linq;//语言集成查询LINQ，对数组、集合等进行查询
using System.Text;//文本处理相关的类
using System.Text.RegularExpressions;//正则表达式处理
using System.Threading;//多线程处理
using System.Windows.Forms;//创建图形用户界面GUI
using static System.Windows.Forms.VisualStyles.VisualStyleElement;//VisualStyleElement静态成员静态类成员
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;//Button类静态成员

namespace AMCP
{
    public partial class FrmWeightCtrl : Form
    {
        private System.IO.Ports.SerialPort serialPort1;
        private string[] ports;     //可用串口数组
        private bool sConnected = false;//COM口开启状态字，在打开/关闭串口中使用，这里没有使用自带的ComPort.IsOpen，因为在串口突然丢失的时候，ComPort.IsOpen会自动false，逻辑混乱

        public FrmWeightCtrl()
        {
            InitializeComponent();

            serialPort1 = new SerialPort();
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
            //RefreshComList();
            nmudSV_ValueChanged(null, null);
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

        }


        //刷新串口列表
        private void RefreshComList(string defaultPortName)
        {
            int defaultPortNo = 0;
            //↓↓↓↓↓↓↓↓↓可用串口下拉控件↓↓↓↓↓↓↓↓↓
            cmbComPort1.Items.Clear();
            ports = System.IO.Ports.SerialPort.GetPortNames();//获取可用串口
            if (ports.Length > 0)//ports.Length > 0说明有串口可用
            {
                btnOpenCOM1.Enabled = true;
                for (int i = 0; i < ports.Length; i++)
                {
                    cmbComPort1.Items.Add(ports[i]);
                    if (ports[i] == defaultPortName)
                    {
                        defaultPortNo = i;
                    }
                }
                cmbComPort1.SelectedIndex = defaultPortNo;//默认选第1个串口
                btnOpenCOM1.Enabled = true;
                OpenSerialPort();
            }
            else//未检测到串口
            {
                btnOpenCOM1.Enabled = false;
                //MessageBox.Show("无可用串口");
            }
            //↑↑↑↑↑↑↑↑↑可用串口下拉控件↑↑↑↑↑↑↑↑↑
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!cmbComPort1.Enabled)
            {
                string sendData = txtCommand.Text;//复制发送数据，以免发送过程中数据被手动改变
                byte[] sendBuffer = GetHexData4Sending(sendData);
                ExcuteCommand(serialPort1, sendBuffer);
                txtReceived.Text += "[T] " + sendData + "\r\n";//加显到接收区
                Thread.Sleep(500);
                byte[] recBuffer;//接收缓冲区
                //txtReceived.Text = "";
                while (serialPort1.BytesToRead > 0)
                {
                    try
                    {
                        recBuffer = new byte[serialPort1.BytesToRead];//接收数据缓存大小
                        serialPort1.Read(recBuffer, 0, recBuffer.Length);//读取数据

                        StringBuilder recBuffer16 = new StringBuilder();//定义16进制接收缓存
                        for (int i = 0; i < recBuffer.Length; i++)
                        {
                            recBuffer16.AppendFormat("{0:X2}" + " ", recBuffer[i]);//X2表示十六进制格式（大写），域宽2位，不足的左边填0。
                        }
                        txtReceived.Text += "[R] " + recBuffer16.ToString() + "\r\n";//加显到接收区
                        //recCount.Text = (Convert.ToInt32(recCount.Text) + recBuffer.Length).ToString();//接收数据字节数
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
        }

        private byte[] GetHexData4Sending(string sendData)
        {
            byte[] sendBuffer = null;//发送数据缓冲区

            //16进制发送
            try //尝试将发送的数据转为16进制Hex
            {
                sendData = sendData.Replace(" ", "");//去除16进制数据中所有空格
                sendData = sendData.Replace("\r", "");//去除16进制数据中所有换行
                sendData = sendData.Replace("\n", "");//去除16进制数据中所有换行
                if (sendData.Length == 1)//数据长度为1的时候，在数据前补0
                {
                    sendData = "0" + sendData;
                }
                else if (sendData.Length % 2 != 0)//数据长度为奇数位时，去除最后一位数据
                {
                    sendData = sendData.Remove(sendData.Length - 1, 1);
                }

                List<string> sendData16 = new List<string>();//将发送的数据，2个合为1个，然后放在该缓存里 如：123456→12,34,56
                for (int i = 0; i < sendData.Length; i += 2)
                {
                    sendData16.Add(sendData.Substring(i, 2));
                }
                sendBuffer = new byte[sendData16.Count];//sendBuffer的长度设置为：发送的数据2合1后的字节数
                for (int i = 0; i < sendData16.Count; i++)
                {
                    sendBuffer[i] = (byte)(Convert.ToInt32(sendData16[i], 16));//发送数据改为16进制
                }
            }
            catch //无法转为16进制时，出现异常
            {
                MessageBox.Show("请输入正确的16进制数据");
            }
            return sendBuffer;
        }


        private void ExcuteCommand(SerialPort seriPort, string sendcommand)
        {
            seriPort.Write(sendcommand);//发送sendBuffer
        }
        private void ExcuteCommand(SerialPort seriPort, byte[] sendBuffer)
        {
            seriPort.Write(sendBuffer, 0, sendBuffer.Length);//发送sendBuffer
        }


        private void btnClear_Click(object sender, EventArgs e)
        {
            txtReceived.Text = "";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            double pv = ReadWKCMass();
            picReading.Visible = !picReading.Visible;
            if (pv != ERR_VALUE)
            {
                ShowWeightData(pv);
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            double pv = ReadAllData();
            if (pv != ERR_VALUE)
            {
                ShowAllData(pv);
            }
        }


        public bool SetSpeedValue(string txtSpeed)
        {
            try
            {
                nmudMassSet_g.Value = (decimal)(Convert.ToDouble(txtSpeed));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SetSpeedValue(decimal speed)
        {
            try
            {
                nmudMassSet_g.Value = speed;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

 
        /// <summary>
        /// 发送速度设置指令
        /// </summary>
        /// <returns>是否设置成功</returns>
        public bool SendZeroMass()
        {
            try
            {
                string strSend = "01 06 00 20 00 01 49 C0";
                byte[] hexSend = GetHexData4Sending(strSend);
                ExcuteCommand(serialPort1, hexSend);
                txtReceived.Text += "[T] " + strSend + "\r\n";//加显到接收区
                Thread.Sleep(500);
                int nByte = serialPort1.BytesToRead;
                if (nByte > 0)
                {
                    byte[] recBuffer = new byte[nByte];
                    serialPort1.Read(recBuffer, 0, nByte);//读取数据
                    StringBuilder recBuffer16 = new StringBuilder();//定义16进制接收缓存
                    for (int i = 0; i < recBuffer.Length; i++)
                    {
                        recBuffer16.AppendFormat("{0:X2}" + " ", recBuffer[i]);
                        //X2表示十六进制格式（大写），域宽2位，不足的左边填0。
                    }
                    txtReceived.Text += "[R] " + (recBuffer16.ToString()) + "\r\n";//加显到接收区
                }
                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }
        //seriport发送strsend，接受recbuffer
        public void SendWKCZeroMass()
        {
    
            string strSend = "T\r\n";
            ExcuteCommand(serialPort1, strSend);//发送
            Thread.Sleep(300);
            int nByte;//字节数
            nByte = serialPort1.BytesToRead;//获取串口缓冲区中当前可读取的字节数
            byte[] recBuffer = new byte[nByte];//接收数据缓存大小
            serialPort1.Read(recBuffer, 0, nByte);//读取数据
            string recbuffer = Encoding.UTF8.GetString(recBuffer);
            while (!recbuffer.Contains("S"))
            {
                strSend = "T\r\n";
                ExcuteCommand(serialPort1, strSend);
                Thread.Sleep(300);
                nByte = serialPort1.BytesToRead;
                recBuffer = new byte[nByte];//接收数据缓存大小
                serialPort1.Read(recBuffer, 0, nByte);//读取数据
                recbuffer = Encoding.UTF8.GetString(recBuffer);
            }

        }
        private string Buf2HexString(byte[] sendBuffer)
        {
            string str = "";
            for (int i = 0; i < sendBuffer.Length; i++)
            {
                str += Convert.ToString(sendBuffer[i], 16).ToUpper().PadLeft(2, '0') + " ";
            }
            return str;
        }



        void SetZero()
        {
            // AA AA 01 02 35 01 02 93
            string strSend = "AA AA 01 00 02 04 00 00 00 00 01";
            byte[] hexSend = GetHexData4Sending(strSend);
            ExcuteCommand(serialPort1, hexSend);
        }

        const int ERR_VALUE = -999;
        double lastMass = 0;
        double stable = 0; // 09 10
        double linecalib = 0; // 线性标定状态 11 12
        double calibrstatus = 0; // 校准状态 13 14
        double exterset = 0; // 校准砝码设置 15 16
        double zerotrack = 0; // 零点追踪设置 17 18
        double stableset = 0; // 稳定参数设置 19 20
        double underload = 0; // 是否欠载 21 22
        double overload = 0; // 是否过载 23 24

        /// <summary>
        /// 读取称重模块全部寄存器值
        /// </summary>
        /// <returns></returns>
        private double ReadAllData()
        {
            string strSend = "01 03 00 01 00 0B 55 CD"; // 读取全部寄存器数据
            //string strSend = "01 03 00 01 00 03 54 0B";     // 读取从地址01到03的寄存器值(含重量及符号）
            // 返回值示例：   01 03 06 00 00 00 00 00 00 21 75
            int recLengthNormal = 27; // 5+11*2=27
            byte[] hexSend = GetHexData4Sending(strSend);
            ExcuteCommand(serialPort1, hexSend);
            txtReceived.Text += "[T] " + strSend + "\r\n";//加显到接收区
            Thread.Sleep(300);
            int nByte;
            nByte = serialPort1.BytesToRead;
            byte[] recBuffer = new byte[nByte];//接收数据缓存大小
            if (nByte >= recLengthNormal)
            {
                serialPort1.Read(recBuffer, 0, nByte);//读取数据
                StringBuilder recBuffer16 = new StringBuilder();//定义16进制接收缓存
                for (int i = 0; i < recBuffer.Length; i++)
                {
                    recBuffer16.AppendFormat("{0:X2}" + " ", recBuffer[i]);//X2表示十六进制格式（大写），域宽2位，不足的左边填0。
                }
                txtReceived.Text += "[R] " + (recBuffer16.ToString()) + "\r\n";//加显到接收区
                if (recBuffer[0] == 0x01 && recBuffer[1] == 0x03 && recBuffer[2] == 0x16) //以01 03 16开头
                {
                    double mass_abs = (Convert.ToInt16(recBuffer[3]) * 16777216 + Convert.ToInt16(recBuffer[4]) * 65536 + Convert.ToInt16(recBuffer[5]) * 256 + Convert.ToInt16(recBuffer[6])) * 0.0001;
                    int sign = recBuffer[8];
                    stable = Convert.ToInt16(recBuffer[9]) * 256 + Convert.ToInt16(recBuffer[10]);
                    linecalib = Convert.ToInt16(recBuffer[11]) * 256 + Convert.ToInt16(recBuffer[12]);
                    calibrstatus = Convert.ToInt16(recBuffer[13]) * 256 + Convert.ToInt16(recBuffer[14]);
                    exterset = Convert.ToInt16(recBuffer[15]) * 256 + Convert.ToInt16(recBuffer[16]);
                    zerotrack = Convert.ToInt16(recBuffer[17]) * 256 + Convert.ToInt16(recBuffer[18]);
                    stableset = Convert.ToInt16(recBuffer[19]) * 256 + Convert.ToInt16(recBuffer[20]);
                    underload = Convert.ToInt16(recBuffer[21]) * 256 + Convert.ToInt16(recBuffer[22]);
                    overload = Convert.ToInt16(recBuffer[23]) * 256 + Convert.ToInt16(recBuffer[24]);

                    if (sign == 0)
                    {
                        lastMass = mass_abs;
                        return lastMass;
                    }
                    else
                    {
                        if (sign == 1 && mass_abs < 100)
                        {
                            lastMass = -mass_abs;
                            return lastMass;
                        }
                        else
                        {
                            return lastMass;
                        }
                    }
                }
                else if (recBuffer[0] == 0x01 && recBuffer[1] == 0x83 && recBuffer[2] == 0x04)
                {
                    return lastMass;
                }
                else
                {
                    return lastMass;
                }
            }
            return lastMass;
        }

        private double ReadWKCMass()
        {
            string strSend = "SI\r\n";
            ExcuteCommand(serialPort1, strSend);//发送指令给串口
            Thread.Sleep(300);
            int nByte;
            nByte = serialPort1.BytesToRead;
            try
            {
                if (nByte > 0)
                {
                    byte[] recBuffer = new byte[nByte];//接收数据缓存大小
                    serialPort1.Read(recBuffer, 0, nByte);//读取数据
                    string recbuffer = System.Text.Encoding.UTF8.GetString(recBuffer);
                    recbuffer = recbuffer.Replace(" ", "");//去除数据中所有空格
                    if (recbuffer.StartsWith("SD") || recbuffer.StartsWith("SS"))//判断称重模块是否报错
                    {
                        recbuffer = recbuffer.Replace("S", "");//去除数据中所有“S”
                        recbuffer = recbuffer.Replace("D", "");//去除数据中所有空格 
                        recbuffer = recbuffer.Replace("\r", "");//去除数据中所有换行
                        recbuffer = recbuffer.Replace("\n", "");//去除数据中所有换行
                        recbuffer = recbuffer.Replace("g", "");//去除数据中所有“G”
                        recbuffer = recbuffer.Replace("T", "");//去除数据中所有“T”
                        double mass_abs = double.Parse(recbuffer);
                        lastMass = mass_abs;
                        return lastMass;
                    }
                }
            }
            catch (Exception)
            {
                return lastMass;
            }
            return lastMass;
        }
        private double ReadMass()
        {
            string strSend = "01 03 00 01 00 04 15 C9"; // 读取地址01到04的寄存器值:
            int recLengthNormal = 13; // 5+2*4=13
            byte[] hexSend = GetHexData4Sending(strSend);
            ExcuteCommand(serialPort1, hexSend);
            txtReceived.Text += "[T] " + strSend + "\r\n";//加显到接收区
            Thread.Sleep(200);
            int nByte;
            nByte = serialPort1.BytesToRead;
            byte[] recBuffer = new byte[nByte];//接收数据缓存大小
            if (nByte >= recLengthNormal)
            {
                serialPort1.Read(recBuffer, 0, nByte);//读取数据
                StringBuilder recBuffer16 = new StringBuilder();//定义16进制接收缓存
                for (int i = 0; i < recBuffer.Length; i++)
                {
                    recBuffer16.AppendFormat("{0:X2}" + " ", recBuffer[i]);//X2表示十六进制格式（大写），域宽2位，不足的左边填0。
                }
                txtReceived.Text += "[R] " + (recBuffer16.ToString()) + "\r\n";//加显到接收区
                if (recBuffer[0] == 0x01 && recBuffer[1] == 0x03 && recBuffer[2] == 0x08) //以01 03 08开头
                {
                    double mass_abs = (Convert.ToInt16(recBuffer[3]) * 16777216 + Convert.ToInt16(recBuffer[4]) * 65536 + Convert.ToInt16(recBuffer[5]) * 256 + Convert.ToInt16(recBuffer[6])) * 0.0001;
                    int sign = recBuffer[8];
                    stable = Convert.ToInt16(recBuffer[9]) * 256 + Convert.ToInt16(recBuffer[10]);
                    if (sign == 0)
                    {
                        lastMass = mass_abs;
                        return lastMass;
                    }
                    else
                    {
                        if (sign == 1 && mass_abs < 100)
                        {
                            lastMass = -mass_abs;
                            return lastMass;
                        }
                        else
                        {
                            return lastMass;
                        }
                    }
                }
                else if (recBuffer[0] == 0x01 && recBuffer[1] == 0x83 && recBuffer[2] == 0x04)
                {
                    return lastMass;
                }
                else
                {
                    return lastMass;
                }
            }
            return lastMass;
        }


        private void FrmWeight_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = (e.CloseReason == CloseReason.UserClosing);
            this.Hide();
            if (btnOpenCOM1.Text == "断开称重模块")
            {
                OpenSerialPort();
            }
        }


        private void label7_Click(object sender, EventArgs e)
        {
            RefreshComList("COM1");
        }

        private void nmudSV_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                double mass_g = Convert.ToDouble(nmudMassSet_g.Value);
                double time_min = (double)numericUpDown1.Value / 60.0;
                double massFlowRate = mass_g / time_min;
                for (int i = 0; i < GV.arrPrintPosSelected.Length; i++)
                {
                    GV.arrMassFlowRate[i] = massFlowRate;
                }
            }
            catch (Exception ex)
            {
            }
        }
        //去皮
        private void btnSetZero_Click(object sender, EventArgs e)
        {
            checkBox1.Checked = false;//连续读取
            SendWKCZeroMass();
            countZeroOK = 0;
            lblZeroing.Show();
            timer5.Start();
            checkBox1.Checked = true;
        }

        private void FrmWeightCtrl_Load(object sender, EventArgs e)
        {
            //RefreshComList(GV.ComWeightMeter);
            button3_Click(sender, e);
        }

        private void btnOpenCOM1_Click(object sender, EventArgs e)
        {
            OpenSerialPort();
        }

        /// <summary>
        /// 打开称重模块串口
        /// </summary>
        public void OpenSerialPort()
        {
            System.Windows.Forms.Button btnConnectSerialPort = btnOpenCOM1;
            System.Windows.Forms.ComboBox cmbComPort;
            SerialPort serialPort;

            cmbComPort = cmbComPort1;
            serialPort = serialPort1;

            try
            {
                if (btnConnectSerialPort.Text == "连接称重模块")
                {
                    serialPort.PortName = cmbComPort.Text;
                    serialPort.BaudRate = 9600;
                    serialPort.Parity = Parity.None;

                    serialPort.Open();

                    btnConnectSerialPort.Text = "断开称重模块";
                    button1.Enabled = true;
                    cmbComPort.Enabled = false;
                    sConnected = true;
                    //panel1.Enabled = true;
                    GV.frmMain.SetWeightControlConnected(true);//通知主界面，称重模块已连接
                    checkBox1.Checked = true;   
                }
                else
                {
                    serialPort.Close();
                    btnConnectSerialPort.Text = "连接称重模块";
                    button1.Enabled = false;
                    cmbComPort.Enabled = true;
                    sConnected = false;
                    timer1.Stop();
                    timer2.Stop();
                    //panel1.Enabled = false;
                    GV.frmMain.SetWeightControlConnected(false);
                    checkBox1.Checked = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                sConnected = false;
                GV.frmMain.SetWeightControlConnected(false);
                return;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                timer1.Start();
                btnReadMass.Hide();
            }
            else
            {
                timer1.Stop();
                picReading.Hide();
                picIsStable.Hide();
                btnReadMass.Show();
            }
        }

        private void btnReadMass_Click(object sender, EventArgs e)
        {
            timer1_Tick(sender, e);
        }

        private void ShowWeightData(double mass)
        {
            lblMassReal_g.Text = mass.ToString("0.0000");
            picIsStable.Visible = (stable == 0); //0：为稳定 ，1：为不稳定
            if (IsWeightOK(mass))
            {
                lblMassReal_g.ForeColor = Color.Lime;
            }
            else
            {
                lblMassReal_g.ForeColor = Color.White;
            }
        }

        private bool IsWeightOK(double mass)
        {
            return (stable == 0) && Math.Abs((double)nmudMassSet_g.Value - mass) <= (double)(nmudMassErr.Value);
        }

        private void ShowAllData(double mass)
        {
            lblMassReal_g.Text = mass.ToString("0.0000");

            picIsStable.Visible = (stable == 0); //0：为稳定 ，1：为不稳定
            //label11.Text = "线性标定状态：" + linecalib.ToString();
            //label12.Text = "校准状态：" + calibrstatus.ToString();
            //label13.Text = "校准砝码设置：" + exterset.ToString();
            //label14.Text = "零点追踪设置：" + zerotrack.ToString(); ;
            //label15.Text = "稳定参数设置：" + stableset.ToString();
            //label16.Text = "是否欠载：" + underload.ToString();
            //label17.Text = "是否过载：" + overload.ToString();

            string str = "";
            // 线性标定状态 linecalib
            str = linecalib.ToString(); 
            switch (linecalib)
            {
                case 0x01:
                    //01: 标定中
                    str = "标定中...";
                    break;
                case 0x00:
                    //00: 加载0g砝码
                    str = "加载0g砝码";
                    break;
                case 0x05:
                    //05: 加载500g砝码
                    str = "加载500g砝码";
                    break;
                case 0x0A:
                    //0A: 加载1000g砝码
                    str = "加载1000g砝码";
                    break;
                case 0x14:
                    //14: 加载2000g砝码
                    str = "加载2000g砝码";
                    break;
                case 0x1E:
                    //1E: 加载3000g砝码
                    str = "加载3000g砝码";
                    break;
                case 0x0E:
                    //0E: 标定完成
                    str = "标定完成";
                    break;
            }
            label11.Text = "线性标定状态：" + str;

            // 校准状态 calibrstatus
            str = calibrstatus.ToString(); 
            switch (calibrstatus)
            {
                case 0x00:
                    //00: 非校准状态
                    str = calibrstatus.ToString();
                    break;
                case 0x01:
                    //01: 校准中
                    str = "01.校准中...";
                    break;
                case 0x0A:
                    //0A: 校准完成
                    str = "0A.校准完成";
                    break;
                case 0xE1:
                    //E1: 校准错误
                    str = "E1.校准错误";
                    break;
            }
            label12.Text = "校准状态：" + str;

            //校准砝码设置 exterset 
            str = exterset.ToString();
            switch (exterset)
            {
                case 0x01:
                    //120g量程：默认校准砝码为100g
                    str = "1. 100g";
                    break;
                case 0x02:
                    //0A: 校准完成
                    str = "2. undefined";
                    break;
            }
            label13.Text = "校准砝码设置：" + str + "";

            //零点追踪设置 zerotrack
            str = zerotrack.ToString();
            //01 03 02 00 02 39 85：表示自动回零D - 2
            switch (zerotrack)
            {
                case 0x00:
                    //00: 关闭
                    str = "0 关闭";
                    break;
                case 0x01:
                case 0x02:
                case 0x03:
                case 0x04:
                case 0x05:
                case 0x06:
                    // 1–6为设置值
                    str = zerotrack.ToString() + " （1–6）";
                    break;
            }
            label14.Text = "零点追踪设置：" + str;

            //稳定参数设置 stableset
            // 0 - 3，滤波强度低 - 高
            str = stableset.ToString() + " （0–3）";
            label15.Text = "稳定参数设置：" + str;

            //是否欠载 underload
            str = underload.ToString();
            switch (underload)
            {
                case 0x00:
                    // 0.正常
                    str = "正常";
                    break;
                case 0x01:
                    // 1.欠载
                    str = "欠载";
                    break;
            }
            label16.Text = "是否欠载：" + str;

            //是否过载 overload 
            switch (overload)
            {
                case 0x00:
                    // 0.正常
                    str = "正常";
                    break;
                case 0x01:
                    // 1.过载
                    str = "过载";
                    break;
            }
            label17.Text = "是否过载：" + str;
        }

        private void btnCalcuCRC16_Click(object sender, EventArgs e)
        {
            txtCommand.Text = CRC16.GetCommandStringCRC16(textBox4.Text);
        }

        private void btnReadAllData_Click(object sender, EventArgs e)
        {
            timer2_Tick(sender, e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                double n_d = 0;                                             // 建议转速
                double n_now = (double)nmudSVa.Value;                        // 当前转速
                double m_now = Convert.ToDouble(lblMassReal_g.Text);        // 当前质量
                double m_d = (double)nmudMassSet_g.Value;                  // 目标质量 
                n_d = m_d * n_now / m_now;
                txtNewSpeed.Text = n_d.ToString("0.0");
                textBox2.Text += n_now.ToString("0.0") + "\t" + m_d.ToString("0.0000") + "\t" + m_now.ToString("0.0000") + "\t" + n_d.ToString("0.0") + "\r\n";
                if (IsWeightOK(m_now))
                {
                    if (DialogResult.OK == MessageBox.Show("当前出丝流量已达到设定目标，确定结束测试并关闭称重模块？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                    {
                        OpenSerialPort();
                        //this.Hide();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void label20_Click(object sender, EventArgs e)
        {
            nmudSVa.Value = numericUpDown1.Value;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox2.Text = "设定转速\t目标质量\t实测质量\t建议转速\r\n";
        }

        private void btnMove2Weight_Click(object sender, EventArgs e)
        {
            GV.PrintingObj.Move2ReadyPos(200, 20, 85, "移动到称重位置", 0);
        }

        private void btnSetNewSpeed_Click(object sender, EventArgs e)
        {
            try
            {
                nmudSVa.Value = Convert.ToDecimal(txtNewSpeed.Text);
                //GV.frmPrintStep2.SetDefaultRotateSpeed((double)nmudSV.Value);
            }
            catch (Exception ex)
            {
            }
        }

        private void btnStartTest_Click(object sender, EventArgs e)
        {
            if (countZeroOK > 10)
            {
                //timer5.Stop();
                StartExtruding();
            }
            else
            {
                
            }
        }

        private void StartExtruding()
        {
            GV.PrintingObj.Extrude(0, 1);
            GV.PrintingObj.Extrude(5, 1);   //开气开螺杆阀
            GV.MsgShow(lblExtrudeTips, "正在出丝...\r\n开始" + (timer3.Interval / 1000).ToString("0") + "秒后自动停止。", timer4, timer3.Interval, Color.Lime);
            timer3.Stop();
            timer3.Start();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            timer3.Interval = (int)(numericUpDown1.Value * 1000);
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            GV.PrintingObj.Extrude(5, 0);   //关闭螺杆阀
           // GV.PrintingObj.PushAlarm(AlarmType.Alarm_Operate_Succuss);
            timer3.Stop();
        }

        private void nmudSV_ValueChanged_1(object sender, EventArgs e)
        {
            GV.frmRotaryValveCtrl.SetSpeedValue(nmudSVa.Value);
            //GV.frmPrintStep4.SetDefaultRotateSpeed();//同步转速显示
        }
        private void nmudSVb_ValueChanged(object sender, EventArgs e)
        {
            //GV.frmRotaryValveCtrl.SetSpeedValue(nmudSVb.Value);
        }

        int countZeroOK = 0;//去皮计数500ms间隔
        private void timer5_Tick(object sender, EventArgs e)
        {
            if (Math.Abs(lastMass) < 0.0003 && stable == 0) // 当前称重模块测量值小于0.0003g 且 读数稳定
            {
                countZeroOK++;
                if (countZeroOK > 10)
                {
                    lblZeroing.Text = "去皮成功";
                    countZeroOK = 11;
                    btnStartTest.Enabled = true;
                    timer5.Stop();
                }
                else
                {
                    lblZeroing.Text = "去皮中..." + countZeroOK.ToString();
                    btnStartTest.Enabled = false;
                }
            }
            else
            {
                countZeroOK = 0;
                lblZeroing.Text = "去皮中..." + countZeroOK.ToString();
                btnStartTest.Enabled = false;
            }
        }

        private void btnStopTest_Click(object sender, EventArgs e)
        {
            GV.PrintingObj.Extrude(5, 0);   //关闭螺杆阀
            lblExtrudeTips.Hide();
            timer3.Stop();
        }

        private void timer4_Tick(object sender, EventArgs e)
        {

        }

        private void FrmWeightCtrl_Enter(object sender, EventArgs e)
        {
            nmudSVa.Value = GV.frmRotaryValveCtrl.GetSpeedValue();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            textBox2.SelectAll();
            textBox2.Copy();
        }

       
    }

    #region 16位CRC校验
    public static class CRC16
    {
        /// <summary>
        /// 将十六进制字符串加CRC16/Modbus校验位
        /// </summary>
        /// <param name="orignCommandString">要转换的十六进制字符串</param>
        /// <returns>添加CRC16/modbus的十六进制字符串</returns>
        public static string GetCommandStringCRC16(string orignCommandString)
        {
            byte[] bdata = CRC16.HexStringToByteArray(orignCommandString);

            //计算CRC值
            byte[] crc16 = CRC16.CRCCalc(bdata);
            //CRC校验码，crc16[0]为高位，crc16[1]为低位，所以不用再Reverse()
            byte[] command = new byte[bdata.Length + crc16.Length];
            Array.Copy(bdata, 0, command, 0, bdata.Length);

            //将CRC校验码写入指令command
            Array.Copy(crc16, 0, command, bdata.Length, crc16.Length);

            //输出16进制结果
            return CRC16.ByteArrayToHexString(command);
        }

        /// <summary>
        /// CRC校验，参数data为byte数组
        /// </summary>
        /// <param name="data">校验数据，字节数组</param>
        /// <returns>字节0是高8位，字节1是低8位</returns>
        public static byte[] CRCCalc(byte[] data)
        {
            //crc计算赋初始值
            int crc = 0xffff;
            for (int i = 0; i < data.Length; i++)
            {
                crc = crc ^ data[i];
                for (int j = 0; j < 8; j++)
                {
                    int temp;
                    temp = crc & 1;
                    crc = crc >> 1;
                    crc = crc & 0x7fff;
                    if (temp == 1)
                    {
                        crc = crc ^ 0xa001;
                    }
                    crc = crc & 0xffff;
                }
            }
            //CRC寄存器的高低位进行互换
            byte[] crc16 = new byte[2];
            //CRC寄存器的高8位变成低8位，
            crc16[1] = (byte)((crc >> 8) & 0xff);
            //CRC寄存器的低8位变成高8位
            crc16[0] = (byte)(crc & 0xff);
            return crc16;
        }

        /// <summary>
        /// CRC校验，参数为空格或逗号间隔的字符串
        /// </summary>
        /// <param name="data">校验数据，逗号或空格间隔的16进制字符串(带有0x或0X也可以),逗号与空格不能混用</param>
        /// <returns>字节0是高8位，字节1是低8位</returns>
        public static byte[] CRCCalc(string data)
        {
            //分隔符是空格还是逗号进行分类，并去除输入字符串中的多余空格
            IEnumerable<string> datac = data.Contains(",") ? data.Replace(" ", "").Replace("0x", "").Replace("0X", "").Trim().Split(',') : data.Replace("0x", "").Replace("0X", "").Split(' ').ToList().Where(u => u != "");
            List<byte> bytedata = new List<byte>();
            foreach (string str in datac)
            {
                bytedata.Add(byte.Parse(str, System.Globalization.NumberStyles.AllowHexSpecifier));
            }
            byte[] crcbuf = bytedata.ToArray();
            //crc计算赋初始值
            return CRCCalc(crcbuf);
        }

        /// <summary>
        ///  CRC校验，截取data中的一段进行CRC16校验
        /// </summary>
        /// <param name="data">校验数据，字节数组</param>
        /// <param name="offset">从头开始偏移几个byte</param>
        /// <param name="length">偏移后取几个字节byte</param>
        /// <returns>字节0是高8位，字节1是低8位</returns>
        public static byte[] CRCCalc(byte[] data, int offset, int length)
        {
            byte[] Tdata = data.Skip(offset).Take(length).ToArray();
            return CRCCalc(Tdata);
        }

        /// <summary>
        /// Hex字符串转16进制字节数组 
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] HexStringToByteArray(string hexString)
        {
            hexString = hexString.Trim();     //去除前后空字符
            hexString = hexString.Replace(',', ' ');  //去掉英文逗号
            hexString = hexString.Replace('，', ' '); //去掉中文逗号
            hexString = hexString.Replace('\t', ' '); //去掉制表符
            hexString = hexString.Replace("0x", "");  //去掉0x
            hexString = hexString.Replace("0X", "");  //去掉0X
            hexString = Regex.Replace(Regex.Replace(hexString, @"(?i)[^a-f\d\s]+", ""), "\\w{3,}", m => string.Join(" ", Regex.Split(m.Value, @"(?<=\G\w{2})(?!$)").Select(x => x.PadLeft(2, '0')).ToArray())).ToUpper();

            hexString = hexString.Replace(" ", "");
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        /// <summary>
        /// 16进制字节数组转Hex字符串 
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static string ByteArrayToHexString(byte[] command)
        {
            StringBuilder resultCmd = new StringBuilder();
            foreach (var one in command)
            {
                resultCmd.Append(string.Format("{0:X2}", one));
                resultCmd.Append(" ");
            }
            return resultCmd.ToString().Trim();
        }
    }
    #endregion

}


