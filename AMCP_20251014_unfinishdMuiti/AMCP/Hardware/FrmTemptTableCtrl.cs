using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

namespace AMCP
{
    public partial class FrmTemptTableCtrl : Form
    {
        private System.IO.Ports.SerialPort serialPort1;

        private string[] ports;     //可用串口数组
        private bool recStaus = true;//接收状态字
        private bool sConnected = false;//COM口开启状态字，在打开/关闭串口中使用，这里没有使用自带的ComPort.IsOpen，因为在串口突然丢失的时候，ComPort.IsOpen会自动false，逻辑混乱
        private bool Listening = false;//用于检测是否没有执行完invoke相关操作，仅在单线程收发使用，但是在公共代码区有相关设置，所以未用#define隔离
        private bool WaitClose = false;//invoke里判断是否正在关闭串口是否正在关闭串口，执行Application.DoEvents，并阻止再次invoke ,解决关闭串口时，程序假死，具体参见http://news.ccidnet.com/art/32859/20100524/2067861_4.html 仅在单线程收发使用，但是在公共代码区有相关设置，所以未用#define隔离//可用串口集合

        const string strSTX = "02";
        const string strETX = "03";
        const string strEOT = "04";
        const string strENQ = "05";
        const string strACK = "06";
        const char STX = (char)(2);
        const char ETX = (char)(3);
        const char EOT = (char)(4);
        const char ENQ = (char)(5);
        const char ACK = (char)(6);

        double Pc = 0, Pp = 0;

        struct SeriCommand
        {
            public SerialPort seriPort;
            public string strCommand;
        }

        public FrmTemptTableCtrl()
        {
            InitializeComponent();

            serialPort1 = new SerialPort();
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(serialPort1_DataReceived);
            //RefreshComList();
        }

        Queue recQueue = new Queue();//接收数据过程中，接收数据线程与数据处理线程直接传递的队列，先进先出

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

        }


        //刷新串口列表
        private void RefreshComList()
        {
            //↓↓↓↓↓↓↓↓↓可用串口下拉控件↓↓↓↓↓↓↓↓↓
            cmbComPort1.Items.Clear();
            ports = System.IO.Ports.SerialPort.GetPortNames();//获取可用串口
            if (ports.Length > 0)//ports.Length > 0说明有串口可用
            {
                btnOpenCOM1.Enabled = true;
                for (int i = 0; i < ports.Length; i++)
                {
                    cmbComPort1.Items.Add(ports[i]);
                }
                cmbComPort1.SelectedIndex = 0;//默认选第1个串口
                btnOpenCOM1.Enabled = true;
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
                //txtReceived.Text = "";
                string sendData = txtCommand.Text;//复制发送数据，以免发送过程中数据被手动改变
                byte[] sendBuffer = GetHexData4Sending(sendData);
                ExcuteCommand(serialPort1, sendBuffer);

                Thread.Sleep(100);
                if (serialPort1.BytesToRead == 0)
                {
                    return;
                }
                //Thread.Sleep(10);//发送和接收均为文本时，接收中为加入判断是否为文字的算法，发送（C4E3），接收可能识别为C4,E3，可用在这里加延时解决

                byte[] recBuffer;//接收缓冲区
                txtReceived.Text = "";

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
                        txtReceived.Text += recBuffer16.ToString();//加显到接收区
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

        private void OpenSerialPort(object sender, EventArgs e)
        {
            Button btnConnectSerialPort = sender as Button;
            ComboBox cmbComPort;
            SerialPort serialPort;

            cmbComPort = cmbComPort1;
            serialPort = serialPort1;

            try
            {
                if (btnConnectSerialPort.Text == "打开串口")
                {
                    serialPort.PortName = cmbComPort.SelectedItem.ToString();
                    serialPort.BaudRate = 115200;
                    serialPort.Parity = Parity.None;

                    serialPort.Open();

                    btnConnectSerialPort.Text = "关闭串口";
                    button1.Enabled = true;
                    cmbComPort.Enabled = false;
                    sConnected = true;
                    timer1.Start();
                    timer2.Start();
                    btnReadPV_Click(sender, e);
                    panel1.Enabled = true;
                }
                else
                {
                    serialPort.Close();
                    btnConnectSerialPort.Text = "打开串口";
                    button1.Enabled = false;
                    cmbComPort.Enabled = true;
                    sConnected = false;
                    timer1.Stop();
                    timer2.Stop();
                    panel1.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                sConnected = false;
                return;
            }
        }


        public class customer//各下拉控件访问接口
        {

            public string com { get; set; }//可用串口
            public string com1 { get; set; }//可用串口
            public string BaudRate { get; set; }//波特率
            public string Parity { get; set; }//校验位
            public string ParityValue { get; set; }//校验位对应值
            public string Dbits { get; set; }//数据位
            public string Sbits { get; set; }//停止位
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshComList();
        }

        private string getASCII(byte[] bytes)
        {
            // ASCII Table:
            string[] tabASCII =
                {   /*   0~  7 */     "[NUL]", "[SOH]", "[STX]", "[ETX]", "[EOT]", "[ENQ]", "[ACK]", "[BEL]",
                    /*   1~ 15 */     "[BS]",  "[TAB]", "[LF]",  "[VT]",  "[FF]",  "[CR]",  "[S0]",  "[SI]",
                    /*  16~ 23 */     "[DLE]", "[DC1]",	"[DC2]", "[DC3]", "[DC4]", "[NAK]", "[SYN]", "[ETB]",
                    /*  24~ 31 */     "[CAN]", "[EM]",  "[SUB]", "[ESC]", "[FS]",  "[GS]",  "[RS]",  "[US]",
                    /*  32~ 39 */     " ",	"!",	"\"",	 "#",     "$",	   "%",	     "&",    "\' ",
                    /*  40~ 47 */     "(",	    ")",    "*",     "+",     ",",     "-",      ".",    "/", 
                    /*  48~ 55 */     "0",	    "1",    "2",     "3",     "4",     "5",      "6",    "7",
                    /*  56~ 63 */     "8",      "9",    ":",     ";",     "<",     "=",      ">",    "?",
                    /*  64~ 71 */     "@",      "A",    "B",     "C",     "D",     "E",      "F",    "G",
                    /*  72~ 79 */     "H",      "I",    "J",     "K",     "L",     "M",      "N",    "O",
                    /*  80~ 87 */     "P",      "Q",    "R",     "S",     "T",     "U",      "V",    "W",
                    /*  88~ 95 */     "X",      "Y",    "Z",     "[",     "\\",    "]",      "^",    "-",
                    /*  96~103 */     "`",      "a",    "b",     "c",     "d",     "e",      "f",    "g",
                    /* 104~111 */     "h",      "i",    "j",     "k",     "l",     "m",      "n",    "o",
                    /* 112~119 */     "p",      "q",    "r",     "s",     "t",     "u",      "v",    "w",
                    /* 120~127 */     "x",      "y",    "z",     "{",     "|",     "}",      "~",    "[DEL]"
                };

            string strASCII = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                strASCII += tabASCII[bytes[i]];
            }
            return strASCII;
        }

        private void ExcuteCommand(SerialPort seriPort, byte[] sendBuffer)
        {
            seriPort.Write(sendBuffer, 0, sendBuffer.Length);//发送sendBuffer
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string sendData = textBox4.Text;    //复制发送数据
            byte[] sendBuffer = GetHexData4Sending(sendData);
            byte crc = GetBCC(sendBuffer);
            //textBox5.Text = Convert.ToString(crc[crc.Length - 1], 16).ToUpper().PadLeft(2, '0') + " " + Convert.ToString(crc[crc.Length - 2], 16).ToUpper().PadLeft(2, '0');
            //txtCommand.Text = textBox4.Text + " " + textBox5.Text;
            //txtCommand.SelectAll();
            //txtCommand.Copy();
        }


        //用于计算BCC异或校验码
        public static byte GetBCC(byte[] Cmd)
        {
            byte BCCValue = Cmd[0];
            byte[] crcbyte = new byte[Cmd.Length + 1]; 
            for (int i = 1; i < Cmd.Length; i++)
            {
                BCCValue = (byte)(BCCValue ^ Cmd[i]);
            }
            return BCCValue;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtReceived.Text = "";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            double value = ReadPV();
            if (value != ERR_VALUE)
            {
                txtPV.Text = value.ToString("0.00");
            }
            else
            {
                txtPV.Text = "ERR";
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            //int value = ReadValue(0x2103);
            //if (value != ERR_VALUE)
            //{
            //    txtSV.Text = value.ToString();
            //}
            //else
            //{
            //    txtSV.Text = "ERR";
            //}
        }

        private void btnReadPV_Click(object sender, EventArgs e)
        {
            double value = ReadPV();
            if (value != ERR_VALUE)
            {
                txtPV.Text = value.ToString("0.0");
            }
            else
            {
                txtPV.Text = "ERR";
            }
        }

        const int ERR_VALUE = -999;

        private double ReadPV()
        {
            try
            {
                // AA BB 00 05 A1 00 00 09 C4 6C EF F0
                string strHead = "AA BB 00 ";
                string strData = "B0 01 ";
                string strTail = " EF F0";
                byte[] hexData = GetHexData4Sending(strData);
                string strLen = hexData.Length.ToString("X2").PadRight(3); // "02 ";
                byte BCC = GetBCC(hexData);
                string strSend = strHead + strLen + strData + BCC.ToString("X2") + strTail;
                //strSend = "AA BB 00 05 A1 00 00 09 C4 6C EF F0";
                byte[] hexSend = GetHexData4Sending(strSend);
                ExcuteCommand(serialPort1, hexSend);
                txtCommand.Text = Buf2HexString(hexSend);

                Thread.Sleep(100);
                int nByte;
                nByte = serialPort1.BytesToRead;
                byte[] recBuffer = new byte[serialPort1.BytesToRead];//接收数据缓存大小
                int value = 0;
                if (recBuffer.Length > 0)
                {
                    serialPort1.Read(recBuffer, 0, recBuffer.Length);//读取数据
                    // CC DD 00 05 B0 01 00 09 B5 0D EF F0 
                    if (recBuffer[0] == 0xCC && recBuffer[1] == 0xDD && recBuffer[2] == 0x00 && recBuffer[3] == 0x05)
                    {
                        value = recBuffer[7] * 256 + recBuffer[8];
                        return value * 0.01;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return ERR_VALUE;
        }

        private bool Open()
        {
            try
            {
                // AA BB 00 02 A9 01 A8 EF F0
                string strSend = "AA BB 00 02 A9 01 A8 EF F0";
                //string strHead = "AA BB 00 ";
                //string strData = "B0 01 ";
                //string strTail = " EF F0";
                //byte[] hexData = GetHexData4Sending(strData);
                //string strLen = hexData.Length.ToString("X2").PadRight(3); // "02 ";
                //byte BCC = GetBCC(hexData);
                //string strSend = strHead + strLen + strData + BCC.ToString("X2") + strTail;
                //strSend = "AA BB 00 05 A1 00 00 09 C4 6C EF F0";
                byte[] hexSend = GetHexData4Sending(strSend);
                ExcuteCommand(serialPort1, hexSend);
                txtCommand.Text = Buf2HexString(hexSend);

                Thread.Sleep(100);
                int nByte;
                nByte = serialPort1.BytesToRead;
                byte[] recBuffer = new byte[serialPort1.BytesToRead];//接收数据缓存大小
                int value = 0;
                if (recBuffer.Length > 0)
                {
                    serialPort1.Read(recBuffer, 0, recBuffer.Length);//读取数据
                    // CC DD 00 05 B0 01 00 09 B5 0D EF F0 
                    if (recBuffer[0] == 0xCC && recBuffer[1] == 0xDD && recBuffer[2] == hexSend[2] && recBuffer[3] == hexSend[3] && recBuffer[4] == hexSend[4] && recBuffer[5] == hexSend[5] && recBuffer[6] == hexSend[6] && recBuffer[7] == hexSend[7] && recBuffer[8] == hexSend[8])
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        private bool Close()
        {
            try
            {
                // AA BB 00 02 A9 01 A8 EF F0
                string strSend = "AA BB 00 02 A9 00 A9 EF F0";
                //string strHead = "AA BB 00 ";
                //string strData = "B0 01 ";
                //string strTail = " EF F0";
                //byte[] hexData = GetHexData4Sending(strData);
                //string strLen = hexData.Length.ToString("X2").PadRight(3); // "02 ";
                //byte BCC = GetBCC(hexData);
                //string strSend = strHead + strLen + strData + BCC.ToString("X2") + strTail;
                //strSend = "AA BB 00 05 A1 00 00 09 C4 6C EF F0";
                byte[] hexSend = GetHexData4Sending(strSend);
                ExcuteCommand(serialPort1, hexSend);
                txtCommand.Text = Buf2HexString(hexSend);

                Thread.Sleep(100);
                int nByte;
                nByte = serialPort1.BytesToRead;
                byte[] recBuffer = new byte[serialPort1.BytesToRead];//接收数据缓存大小
                int value = 0;
                if (recBuffer.Length > 0)
                {
                    serialPort1.Read(recBuffer, 0, recBuffer.Length);//读取数据
                    // CC DD 00 05 B0 01 00 09 B5 0D EF F0 
                    if (recBuffer[0] == 0xCC && recBuffer[1] == 0xDD && recBuffer[2] == hexSend[2] && recBuffer[3] == hexSend[3] && recBuffer[4] == hexSend[4] && recBuffer[5] == hexSend[5] && recBuffer[6] == hexSend[6] && recBuffer[7] == hexSend[7] && recBuffer[8] == hexSend[8])
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        private bool WriteValue(Int16 addr, Int16 value)
        {
            try
            {
                byte[] addrBuf = new byte[2];
                addrBuf[1] = (byte)(addr % 256);
                addrBuf[0] = (byte)(addr / 256);
                string sendData = "01 06 " + Convert.ToString(addr, 16).ToUpper().PadLeft(4, '0') + Convert.ToString(value, 16).ToUpper().PadLeft(4, '0');
                //string sendData = "01 03 " + Convert.ToString(addrBuf[1], 16).ToUpper().PadLeft(2, '0') + " " + Convert.ToString(addrBuf[0], 16).ToUpper().PadLeft(2, '0') + " 00 01";
                //string sendData = "01 03 20 00 00 01";01 06 21 03 00 1E F3 FE
                byte[] sendBuffer = GetHexData4Sending(sendData);
                //byte[] sendBufferCRC = GetCRCC(sendBuffer, 0xA001, false);
                //ExcuteCommand(serialPort1, sendBufferCRC);
                //txtCommand.Text = Buf2HexString(sendBufferCRC);

                //Thread.Sleep(100);
                //int nByte;
                //nByte = serialPort1.BytesToRead;
                //byte[] recBuffer = new byte[serialPort1.BytesToRead];//接收数据缓存大小
                //serialPort1.Read(recBuffer, 0, recBuffer.Length);//读取数据
                //if (recBuffer[0] == 0x01 && recBuffer[1] == 0x06 && recBuffer[2] * 256 + recBuffer[3] == addr)
                //{
                //    return true;
                //}
            }
            catch (Exception ex)
            {
            }
            return false;
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

        private void btnReadSP_Click(object sender, EventArgs e)
        {
            //ReadSP();
        }

        private void btnWriteSV_Click(object sender, EventArgs e)
        {
            try
            {
                SetPV(Convert.ToDouble(nmudSV.Value));
            }
            catch (Exception ex)
            {
            }
        }

        void SetPV(double value)
        {
            // AA BB 00 05 A1 00 00 09 C4 6C EF F0
            string strHead = "AA BB 00 ";
            string strData1 = "A1 01 ";
            int data = Convert.ToInt32(value * 100);
            int sgn = (value >= 0 ? 0 : 1);
            string strData2 = sgn.ToString("X2") + Math.Abs(data).ToString("X4"); // "00 07 08 "
            byte[] hexData = GetHexData4Sending(strData1 + strData2);
            string strLen = hexData.Length.ToString("X2").PadRight(3); // "05 ";
            byte BCC = GetBCC(hexData);
            string strTail = " EF F0";
            string strSend = strHead + strLen + strData1 + strData2 + BCC.ToString("X2") + strTail;
            //strSend = "AA BB 00 05 A1 00 00 09 C4 6C EF F0";
            byte[] hexSend = GetHexData4Sending(strSend);
            ExcuteCommand(serialPort1, hexSend);
            txtCommand.Text = Buf2HexString(hexSend);

            Thread.Sleep(100);
            int nByte;
            nByte = serialPort1.BytesToRead;
            byte[] recBuffer = new byte[serialPort1.BytesToRead];//接收数据缓存大小
            if (recBuffer.Length > 0 )
            {
                serialPort1.Read(recBuffer, 0, recBuffer.Length);//读取数据
                // CC DD 00 05 B0 01 00 09 B5 0D EF F0 

            }
        }

        private void txtSV_Enter(object sender, EventArgs e)
        {
            timer2.Stop();
        }

        private void txtSV_Leave(object sender, EventArgs e)
        {
            timer2.Start();
        }

        private void txtSV_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnWriteSV_Click(sender, e);
            }
        }

        private void FrmTemptTableCtrl_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = (e.CloseReason == CloseReason.UserClosing);
            this.Hide();
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            if (Open())
            {
                ConnOn.Show();
            }
            else
            {
                MessageBox.Show("开机指令发送失败，请检查是否恒温平台电源是否打开，通信线缆是否连通正常。", "错误提示");
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (Close())
            {
                ConnOn.Hide();
            }
            else
            {
                MessageBox.Show("关机指令发送失败，请检查是否恒温平台电源是否打开，通信线缆是否连通正常。", "错误提示");
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {
            RefreshComList();
        }

    }
}
    

