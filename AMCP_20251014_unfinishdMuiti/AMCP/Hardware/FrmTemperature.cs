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
    public partial class FrmTemperature : Form
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

        public FrmTemperature()
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
                    serialPort.BaudRate = 9600; //19200; //115200;
                    serialPort.Parity = Parity.Even;

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
            byte[] crc = GetCRC16ByPoly(sendBuffer, 0xA001, true);
            textBox5.Text = Convert.ToString(crc[crc.Length - 1], 16).ToUpper().PadLeft(2, '0') + " " + Convert.ToString(crc[crc.Length - 2], 16).ToUpper().PadLeft(2, '0');
            txtCommand.Text = textBox4.Text + " " + textBox5.Text;
            txtCommand.SelectAll();
            txtCommand.Copy();
        }


        //用于计算校验码
        public static byte[] GetCRC16ByPoly(byte[] Cmd, ushort Poly, bool IsHighBefore)
        {
            byte[] CRC = new byte[2];
            ushort CRCValue = 0xFFFF;
            byte[] crcbyte = new byte[Cmd.Length + 2];

            for (int i = 0; i < Cmd.Length; i++)
            {
                CRCValue = (ushort)(CRCValue ^ Cmd[i]);
                for (int j = 0; j < 8; j++)
                {
                    if ((CRCValue & 0x0001) != 0)
                    {
                        CRCValue = (ushort)((CRCValue >> 1) ^ Poly);
                    }
                    else
                    {
                        CRCValue = (ushort)(CRCValue >> 1);
                    }
                }
            }
            byte[] Check = BitConverter.GetBytes(CRCValue);
            if (IsHighBefore == true)
            {
                for (int j = 0; j < Cmd.Length; j++)
                    crcbyte[j] = Cmd[j];
                crcbyte[crcbyte.Length - 1] = Check[0];
                crcbyte[crcbyte.Length - 2] = Check[1];

                return crcbyte;
            }
            else
            {
                for (int j = 0; j < Cmd.Length; j++)
                    crcbyte[j] = Cmd[j];
                crcbyte[crcbyte.Length - 2] = Check[0];
                crcbyte[crcbyte.Length - 1] = Check[1];

                return crcbyte;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtReceived.Text = "";
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            int value;
            // 读取高温喷头温度
            value = ReadValue(0x2000, 1);
            if (value != ERR_VALUE)
            {
                if (value != ERR_DISCONNECT)
                {
                    txtPV1.Text = value.ToString();
                }
                else
                {
                    txtPV1.Text = "未接入";
                }
            }
            else
            {
                txtPV1.Text = "ERR";
            }

            // 读取低温喷头温度
            value = ReadValue(0x2000, 2);
            if (value != ERR_VALUE)
            {
                if (value != ERR_DISCONNECT)
                {
                    txtPV2.Text = value.ToString();
                }
                else
                {
                    txtPV2.Text = "未接入";
                }
            }
            else
            {
                txtPV2.Text = "ERR";
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            int value;
            // 读取高温喷头设置温度
            value = ReadValue(0x2103, 1);
            if (value != ERR_VALUE)
            {
                txtSV1.Text = value.ToString();
            }
            else
            {
                txtSV1.Text = "ERR";
            }

            // 读取低温喷头设置温度
            value = ReadValue(0x2103, 2);
            if (value != ERR_VALUE)
            {
                txtSV2.Text = value.ToString();
            }
            else
            {
                txtSV2.Text = "ERR";
            }
        }

        private void btnReadPV_Click(object sender, EventArgs e)
        {
            int value = ReadValue(0x2000);
            if (value != ERR_VALUE)
            {
                txtPV1.Text = value.ToString();
            }
            else
            {
                txtPV1.Text = "ERR";
            }

            value = ReadValue(0x2103);
            if (value != ERR_VALUE)
            {
                txtSV1.Text = value.ToString();
            }
            else
            {
                txtSV1.Text = "ERR";
            }
        }

        const int ERR_VALUE = -999;
        const int ERR_DISCONNECT = 1320;

        private int ReadPV()
        {
            string sendData = "01 03 20 00 00 01";
            byte[] sendBuffer = GetHexData4Sending(sendData);
            byte[] sendBufferCRC = GetCRC16ByPoly(sendBuffer, 0xA001, false);
            ExcuteCommand(serialPort1, sendBufferCRC);
            txtCommand.Text = Buf2HexString(sendBufferCRC);

            Thread.Sleep(100);
            int nByte;
            nByte = serialPort1.BytesToRead;
            byte[] recBuffer = new byte[serialPort1.BytesToRead];//接收数据缓存大小
            int value = 0;
            serialPort1.Read(recBuffer, 0, recBuffer.Length);//读取数据
            if (recBuffer[0] == 0x01 && recBuffer[1] == 0x03)
            {
                value = recBuffer[3] * 256 + recBuffer[4];
                return value;
            }
            return ERR_VALUE;
        }

        private int ReadValue(short addr, byte unitNo = 1)
        {
            try
            {
                // 发送读取变量指令
                byte funcCode = 0x03; // 功能代码（03：读取变量）
                string sendData = unitNo.ToString("X2") + funcCode.ToString("X2") + addr.ToString("X4") + " 00 01";
                byte[] sendBuffer = GetHexData4Sending(sendData);
                byte[] sendBufferCRC = GetCRC16ByPoly(sendBuffer, 0xA001, false);
                ExcuteCommand(serialPort1, sendBufferCRC);
                txtCommand.Text = Buf2HexString(sendBufferCRC);

                Thread.Sleep(100);
                int nByte;
                nByte = serialPort1.BytesToRead;
                byte[] recBuffer = new byte[serialPort1.BytesToRead];//接收数据缓存大小
                int value = 0;
                serialPort1.Read(recBuffer, 0, recBuffer.Length);//读取数据
                if (recBuffer[0] == unitNo && recBuffer[1] == funcCode)
                {
                    value = BitConverter.ToInt16(new byte[] { recBuffer[4], recBuffer[3] }, 0);
                    //value = recBuffer[3] * 256 + recBuffer[4]; //Convert.ToInt16(Convert.ToString(recBuffer[3] * 256 + recBuffer[4], 16), 16);
                    return value;
                }
            }
            catch (Exception ex)
            {
            }
            return ERR_VALUE;
        }

        private bool WriteValue(Int16 addr, Int16 value, byte unitNo = 1)
        {
            try
            {
                // 发送写入变量指令
                byte funcCode = 0x06; // 功能代码（06：写入变量）
                string sendData = unitNo.ToString("X2") + funcCode.ToString("X2") + addr.ToString("X4") + value.ToString("X4");
                byte[] sendBuffer = GetHexData4Sending(sendData);
                byte[] sendBufferCRC = GetCRC16ByPoly(sendBuffer, 0xA001, false);
                ExcuteCommand(serialPort1, sendBufferCRC);
                txtCommand.Text = Buf2HexString(sendBufferCRC);
                Thread.Sleep(100);
                int nByte;
                nByte = serialPort1.BytesToRead;
                byte[] recBuffer = new byte[serialPort1.BytesToRead];//接收数据缓存大小
                serialPort1.Read(recBuffer, 0, recBuffer.Length);//读取数据
                if (recBuffer[0] == unitNo && recBuffer[1] == funcCode && recBuffer[2] * 256 + recBuffer[3] == addr)
                {
                    return true;
                }
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

        private void btnWriteSV1_Click(object sender, EventArgs e)
        {
            try
            {
                short value = Convert.ToInt16(txtSV1.Text);
                WriteValue(0x2103, value, 1);
            }
            catch (Exception ex)
            {
            }
        }

        private void btnWriteSV2_Click(object sender, EventArgs e)
        {
            try
            {
                short value = Convert.ToInt16(txtSV2.Text);
                WriteValue(0x2103, value, 2);
            }
            catch (Exception ex)
            {
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
                btnWriteSV1_Click(sender, e);
            }
        }

        private void btnSet1_Click(object sender, EventArgs e)
        {
            Int16 value = 0;
            WriteValue(0x2103, value, 1);
        }

        private void btnSet2_Click(object sender, EventArgs e)
        {
            Int16 value = 100;
            WriteValue(0x2103, value, 2);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                short addr = Convert.ToInt16(textBox1.Text, 16);
                int value = Convert.ToInt32(textBox2.Text, 16);
                // 发送写入变量指令
                byte funcCode = 0x06; // 功能代码（06：写入变量）
                string sendData = (1).ToString("X2") + funcCode.ToString("X2") + addr.ToString("X4") + value.ToString("X4");
                byte[] sendBuffer = GetHexData4Sending(sendData);
                byte[] sendBufferCRC = GetCRC16ByPoly(sendBuffer, 0xA001, false);
                ExcuteCommand(serialPort1, sendBufferCRC);
                txtCommand.Text = Buf2HexString(sendBufferCRC);
                Thread.Sleep(100);
                int nByte;
                nByte = serialPort1.BytesToRead;
                byte[] recBuffer = new byte[serialPort1.BytesToRead];//接收数据缓存大小
                serialPort1.Read(recBuffer, 0, recBuffer.Length);//读取数据
                textBox3.Text = Buf2HexString(recBuffer);
            }
            catch (Exception ex)
            {
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                short addr = Convert.ToInt16(textBox1.Text, 16);
                // 发送读取变量指令
                byte funcCode = 0x03; // 功能代码（03：读取变量）
                string sendData = (1).ToString("X2") + funcCode.ToString("X2") + addr.ToString("X4") + " 00 02";
                byte[] sendBuffer = GetHexData4Sending(sendData);
                byte[] sendBufferCRC = GetCRC16ByPoly(sendBuffer, 0xA001, false);
                ExcuteCommand(serialPort1, sendBufferCRC);
                Thread.Sleep(100);
                int nByte;
                nByte = serialPort1.BytesToRead;
                byte[] recBuffer = new byte[serialPort1.BytesToRead];//接收数据缓存大小
                serialPort1.Read(recBuffer, 0, recBuffer.Length);//读取数据
                textBox3.Text = Buf2HexString(recBuffer);
            }
            catch (Exception ex)
            {
            }
        }

        private void FrmTemperature_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = (e.CloseReason == CloseReason.UserClosing);
            this.Hide();
        }

        private void label7_Click(object sender, EventArgs e)
        {
            RefreshComList();
        }

    }
}
    

