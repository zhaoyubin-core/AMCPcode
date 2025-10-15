using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Collections;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.IO;
//using System.Collections.Generic;

namespace AMCP
{
    public partial class FrmRotaryValveCtrl : Form
    {
        private System.IO.Ports.SerialPort serialPort1;

        private string[] ports;     //可用串口数组
        private bool recStaus = true;//接收状态字
        private bool sConnected = false;//COM口开启状态字，在打开/关闭串口中使用，这里没有使用自带的ComPort.IsOpen，因为在串口突然丢失的时候，ComPort.IsOpen会自动false，逻辑混乱
        private bool Listening = false;//用于检测是否没有执行完invoke相关操作，仅在单线程收发使用，但是在公共代码区有相关设置，所以未用#define隔离
        private bool WaitClose = false;//invoke里判断是否正在关闭串口是否正在关闭串口，执行Application.DoEvents，并阻止再次invoke ,解决关闭串口时，程序假死，具体参见http://news.ccidnet.com/art/32859/20100524/2067861_4.html 仅在单线程收发使用，但是在公共代码区有相关设置，所以未用#define隔离//可用串口集合

        public string strCmdStartRotaryValve = "AA AA 01 00 02 04 00 00 00 00 01";
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

        public FrmRotaryValveCtrl()
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
        private void RefreshComList(string defaultPortName)
        {
            int defaultPortNo = -1;
            //↓↓↓↓↓↓↓↓↓可用串口下拉控件↓↓↓↓↓↓↓↓↓
            cmbComPort1.Items.Clear();
            ports = System.IO.Ports.SerialPort.GetPortNames();//获取可用串口
            if (ports.Length > 0)//ports.Length > 0说明有串口可用
            {
                //btnSetTimeRotation.Enabled = true;
                for (int i = 0; i < ports.Length; i++)
                {
                    cmbComPort1.Items.Add(ports[i]);
                    if (ports[i] == defaultPortName)
                    {
                        defaultPortNo = i;
                    }
                }
                cmbComPort1.SelectedIndex = defaultPortNo;//默认选第1个串口
                btnSetTimeRotation.Enabled = true;
                // OpenSerialPort();
            }
            else//未检测到串口
            {
                btnSetTimeRotation.Enabled = false;
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
        //输入的字符串（16进制格式）转换成字节数组（byte[]）
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
            //RefreshComList("COM1");
        }

        private string getASCII(byte[] bytes)
        {
            // ASCII Table:
            string[] tabASCII =
                {   /*   0~  7 */     "[NUL]", "[SOH]", "[STX]", "[ETX]", "[EOT]", "[ENQ]", "[ACK]", "[BEL]",
                    /*   1~ 15 */     "[BS]",  "[TAB]", "[LF]",  "[VT]",  "[FF]",  "[CR]",  "[S0]",  "[SI]",
                    /*  16~ 23 */     "[DLE]", "[DC1]", "[DC2]", "[DC3]", "[DC4]", "[NAK]", "[SYN]", "[ETB]",
                    /*  24~ 31 */     "[CAN]", "[EM]",  "[SUB]", "[ESC]", "[FS]",  "[GS]",  "[RS]",  "[US]",
                    /*  32~ 39 */     " ",  "!",    "\"",    "#",     "$",     "%",      "&",    "\' ",
                    /*  40~ 47 */     "(",      ")",    "*",     "+",     ",",     "-",      ".",    "/", 
                    /*  48~ 55 */     "0",      "1",    "2",     "3",     "4",     "5",      "6",    "7",
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
            try
            {
                string[] sendData = textBox4.Text.Split('\n');    //复制发送数据
                for (int i = 0; i < sendData.Length; i++)
                {
                    string str = sendData[i].Substring(6).Trim();
                    if (str.Length > 10)
                    {
                        byte[] sendBuffer = GetHexData4Sending(str.Substring(0, str.Length - 2));
                        byte crc = GetCRC(sendBuffer);
                        textBox5.Text += crc.ToString("X2") + "\r\n";

                    }
                }
            }
            catch (Exception ex)
            {
            }


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

        bool bOnOff = false;
        private void timer1_Tick(object sender, EventArgs e)
        {
            txtPV_real.Text = lastRotarySpeedSet.ToString("0.0");
            //if (bOnOff)
            //{

            //}
            //else
            //{
            //    txtPV_real.Text = "0.0";
            //}
            //double value = ReadPV();
            //if (value != ERR_VALUE)
            //{
            //    txtPV_real.Text = value.ToString("0.00");
            //}
            //else
            //{
            //    txtPV_real.Text = "ERR";
            //}
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
                txtPV_real.Text = value.ToString("0.0");
            }
            else
            {
                txtPV_real.Text = "ERR";
            }
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
        //设置
        private bool SetSpeedMode()
        {
            try
            {
                //AA AA 01 02 35 01 02 93
                //01 06 00 01 00 01 19 CA 
                string strSend = "AA AA 01 02 35 01 02 93";/*"01 06 00 01 00 01 19 CA";*///正向
                byte[] hexSend = GetHexData4Sending(strSend);
                ExcuteCommand(serialPort1, hexSend);
                txtCommand.Text = Buf2HexString(hexSend);

                Thread.Sleep(100);
                int nByte;
                nByte = serialPort1.BytesToRead;
                byte[] recBuffer = new byte[serialPort1.BytesToRead];//接收数据缓存大小
                if (recBuffer.Length > 0)
                {
                    serialPort1.Read(recBuffer, 0, recBuffer.Length);//读取数据
                    // CC DD 00 05 B0 01 00 09 B5 0D EF F0 

                }

            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public bool SetSpeedValue(string txtSpeed)
        {
            try
            {
                nmudSV.Value = (decimal)(Convert.ToDouble(txtSpeed));
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
                nmudSV.Value = speed;
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public decimal GetSpeedValue()
        {
            return nmudSV.Value;
        }

        /// <summary>
        /// 启停螺杆阀转动
        /// </summary>
        /// <param name="onOff">1：启动；2：停止</param>
        /// <param name="speed">转速（rpm）：默认为0</param>
        /// <returns>是否执行成功</returns>
        public bool OpenClose(int onOff, double speed = 0)
        {
            try
            {
                if (onOff == 1) //螺杆阀开启
                {
                    if (speed == 0)
                    {
                        return GV.frmRotaryValveCtrl.SendSpeedSetCmd();
                    }
                    else
                    {
                        return GV.frmRotaryValveCtrl.SendSpeedSetCmd(speed);
                    }
                }
                else //螺杆阀关闭
                {
                    // GV.frmDCPower.ClosePower();
                    return GV.frmRotaryValveCtrl.SendZeroSpeed();
                }
            }
            catch (Exception)
            {
            }
            return false;
        }

        /// <summary>
        /// 发送速度设置指令
        /// </summary>
        /// <returns>是否设置成功</returns>
        public bool SendSpeedSetCmd()
        {
            try
            {
                string strSend = strCmdStartRotaryValve;
                byte[] hexSend = GetHexData4Sending(strSend);
                ExcuteCommand(serialPort1, hexSend);
                txtCommand.Text = Buf2HexString(hexSend);
                bOnOff = true;

                //txtPV_real.Text = lastRotarySpeedSet.ToString("0.0");
                //Thread.Sleep(100);
                //int nByte;
                //nByte = serialPort1.BytesToRead;
                //byte[] recBuffer = new byte[serialPort1.BytesToRead];//接收数据缓存大小
                //if (recBuffer.Length > 0)
                //{
                //    serialPort1.Read(recBuffer, 0, recBuffer.Length);//读取数据

                //    StringBuilder recBuffer16 = new StringBuilder();//定义16进制接收缓存
                //    for (int i = 0; i < recBuffer.Length; i++)
                //    {
                //        recBuffer16.AppendFormat("{0:X2}" + " ", recBuffer[i]);//X2表示十六进制格式（大写），域宽2位，不足的左边填0。
                //    }
                //    txtReceived.Text += recBuffer16.ToString();//加显到接收区

                //}
                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        /// <summary>
        /// 发送螺杆转速设置指令
        /// </summary>
        /// <param name="value">待设置的转速（rpm）</param>
        /// <returns>是否执行成功</returns>
        public bool SendSpeedSetCmd(double value)
        {
            try
            {
                string strHead = "AA AA ";
                string strCmd = "01 00 02 04 ";
                int data = Convert.ToInt32(value * 10);
                string strData = data.ToString("X8");
                string strData1 = strData.Substring(6, 2).PadRight(3);
                string strData2 = strData.Substring(4, 2).PadRight(3);
                string strData3 = strData.Substring(2, 2).PadRight(3);
                string strData4 = strData.Substring(0, 2).PadRight(3);
                string message = strCmd + strData1 + strData2 + strData3 + strData4;
                byte[] hexData = GetHexData4Sending(message);
                byte CRC = GetCRC(hexData);
                string strTail = CRC.ToString("X2");
                string strSend = strHead + message + strTail;
                byte[] hexSend = GetHexData4Sending(strSend);
                ExcuteCommand(serialPort1, hexSend);
                bOnOff = true;
                return true;
            }
            catch (Exception ex)
            {
            }
            return false;
        }


        /// <summary>
        /// 发送速度设置指令
        /// </summary>
        /// <returns>是否设置成功</returns>
        public bool SendZeroSpeed()
        {
            try
            {
                //01 06 00 03 00 01 B8 0A 停止

                string strSend = /*"01 06 00 03 00 01 B8 0A";*/"AA AA 01 00 02 04 00 00 00 00 01";
                byte[] hexSend = GetHexData4Sending(strSend);
                ExcuteCommand(serialPort1, hexSend);
                txtCommand.Text = Buf2HexString(hexSend);
                bOnOff = false;
                //txtPV_real.Text = "0.0";
                //Thread.Sleep(100);
                //int nByte;
                //nByte = serialPort1.BytesToRead;
                //byte[] recBuffer = new byte[serialPort1.BytesToRead];//接收数据缓存大小
                //if (recBuffer.Length > 0)
                //{
                //    serialPort1.Read(recBuffer, 0, recBuffer.Length);//读取数据

                //    StringBuilder recBuffer16 = new StringBuilder();//定义16进制接收缓存
                //    for (int i = 0; i < recBuffer.Length; i++)
                //    {
                //        recBuffer16.AppendFormat("{0:X2}" + " ", recBuffer[i]);//X2表示十六进制格式（大写），域宽2位，不足的左边填0。
                //    }
                //    txtReceived.Text += recBuffer16.ToString();//加显到接收区

                //}
                return true;
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

        private async void btnWriteSV_Click(object sender, EventArgs e)
        {
            SendSpeedSetCmd();
            //GV.PrintingObj.SetExtrudePorts(-2, 1);
            ////延时
            //await Task.Delay(Convert.ToInt32(numTimeDelay.Value));
            //SendZeroSpeed();
            //GV.PrintingObj.SetExtrudePorts(-2, 0);
        }

        void SetZero()
        {
            // AA AA 01 02 35 01 02 93
            string strSend = "AA AA 01 00 02 04 00 00 00 00 01";
            byte[] hexSend = GetHexData4Sending(strSend);
            ExcuteCommand(serialPort1, hexSend);
        }

        const int ERR_VALUE = -999;

        private double ReadPV()
        {
            string strReceived = "";
            string errMessage = "";
            try
            {
                // AA BB 00 05 A1 00 00 09 C4 6C EF F0
                string strHead = "AA AA ";
                string strCmd = "01 03 06 00 ";
                string message = strCmd;
                byte[] hexData = GetHexData4Sending(message);
                byte CRC = GetCRC(hexData);
                string strTail = CRC.ToString("X2");
                string strSend = strHead + message + strTail;
                txtCommand.Text = strSend;

                byte[] hexSend = GetHexData4Sending(strSend);
                ExcuteCommand(serialPort1, hexSend);
                txtCommand.Text = Buf2HexString(hexSend);

                Thread.Sleep(100);
                int nByte;
                nByte = serialPort1.BytesToRead;
                byte[] recBuffer = new byte[serialPort1.BytesToRead];//接收数据缓存大小
                double value = 0;
                if (recBuffer.Length > 0)
                {
                    serialPort1.Read(recBuffer, 0, recBuffer.Length);//读取数据
                    strReceived = Buf2HexString(recBuffer);
                    // CC DD 00 05 B0 01 00 09 B5 0D EF F0 
                    if (recBuffer[0] == 0xAA && recBuffer[1] == 0xAA)
                    {
                        byte[] buffer = new byte[recBuffer.Length - 3];
                        for (int i = 0; i < buffer.Length; i++)
                        {
                            buffer[i] = recBuffer[i + 2];
                        }
                        byte crc = GetCRC(buffer);
                        if (crc != recBuffer[recBuffer.Length - 1]) // 校验不通过
                        {
                            txtReceived.Text = strReceived + "\t" + "CRC_ERR" + "\r\n" + txtReceived.Text;
                            return ERR_VALUE;
                        }
                        else if (recBuffer[2] == 0x01 && recBuffer[3] == 0x03 && recBuffer[4] == 0x06)
                        {
                            string strValue = recBuffer[9].ToString("X2") + recBuffer[8].ToString("X2") + recBuffer[7].ToString("X2") + recBuffer[6].ToString("X2");
                            value = -0.1 * Convert.ToInt32(strValue, 16);
                            txtReceived.Text = strReceived + "\t" + value.ToString() + "\r\n" + txtReceived.Text;
                            return value;
                        }
                        else
                        {
                            errMessage = "CMD_ERR";
                        }
                    }
                    else
                    {
                        errMessage = "HEAD_ERR";
                    }
                }
                else
                {
                    errMessage = "BUF_LEN_ERR";
                }
            }
            catch (Exception ex)
            {
                errMessage = ex.Message;
            }
            txtReceived.Text = strReceived + "\t" + errMessage + "\r\n" + txtReceived.Text;
            return ERR_VALUE;
        }

        double lastRotarySpeedSet = 0;
        public string SetPV(double value)
        {
            // AA AA 01 02 35 01 02 93
            //01 06 00 02 00 01 E9 CA //运行
            // 01 06 00 04 00 05 08 08  //速度设置

            //string strHead = "01 06 00 04";//AA AA
            //string strCmd = "00 02 00 01 ";//01 00 02 04 
            string strHead = "AA AA ";
            string strCmd = "01 00 02 04 ";
            int data = Convert.ToInt32(value);// * 10);
            string strData = data.ToString("X8");// data.ToString("X8");//十六进制，至少8位，不足补0
            string strData1 = strData.Substring(6, 2).PadRight(3);
            string strData2 = strData.Substring(4, 2).PadRight(3);
            string strData3 = strData.Substring(2, 2).PadRight(3);
            string strData4 = strData.Substring(0, 2).PadRight(3);
            string message = /*strHead + strData;*/strCmd + strData1 + strData2 + strData3 + strData4;
            byte[] hexData = GetHexData4Sending(message);//字符串转字节

            // string speed = data.ToString("X4"); 
            byte CRC = GetCRC(hexData);
            //获取CRC16
            byte[] crcBytes = CalculateModbusCrc(hexData);
            string strTail = CRC.ToString("X2")/*($"{crcBytes[0]:X2} {crcBytes[1]:X2}")*/;//校验码
            string strSend = strHead + message + strTail;//发送
            //string strSend = strHead + speed + strTail;
            strCmdStartRotaryValve = strSend;
            lastRotarySpeedSet = value;

            lblCmdStartRotaryValve.Text = strSend;
            //txtCommand.Text = strSend;
            return strSend;
        }

        byte[] crc8Table =
                {0x00, 0xd5,0x7f,0xaa,0xfe,0x2b,0x81,0x54,0x29,0xfc,0x56,0x83,0xd7,0x02,0xa8,0x7d,0x52,0x87,0x2d,0xf8,0xac,
                0x79,0xd3,0x06,0x7b,0xae,0x04,0xd1,0x85,0x50,0xfa,0x2f,0xa4,0x71,0xdb,0x0e,0x5a,0x8f,0x25,0xf0,0x8d,0x58,
                0xf2,0x27,0x73,0xa6,0x0c,0xd9,0xf6,0x23,0x89,0x5c,0x08,0xdd,0x77,0xa2,0xdf,0x0a,0xa0,0x75,0x21,0xf4,0x5e,
                0x8b,0x9d,0x48,0xe2,0x37,0x63,0xb6,0x1c,0xc9,0xb4,0x61,0xcb,0x1e,0x4a,0x9f,0x35,0xe0,0xcf,0x1a,0xb0,0x65,
                0x31,0xe4,0x4e,0x9b,0xe6,0x33,0x99,0x4c,0x18,0xcd,0x67,0xb2,0x39,0xec,0x46,0x93,0xc7,0x12,0xb8,0x6d,0x10,
                0xc5,0x6f,0xba,0xee,0x3b,0x91,0x44,0x6b,0xbe,0x14,0xc1,0x95,0x40,0xea,0x3f,0x42,0x97,0x3d,0xe8,0xbc,0x69,
                0xc3,0x16,0xef,0x3a,0x90,0x45,0x11,0xc4,0x6e,0xbb,0xc6,0x13,0xb9,0x6c,0x38,0xed,0x47,0x92,0xbd,0x68,0xc2,
                0x17,0x43,0x96,0x3c,0xe9,0x94,0x41,0xeb,0x3e,0x6a,0xbf,0x15,0xc0,0x4b,0x9e,0x34,0xe1,0xb5,0x60,0xca,0x1f,
                0x62,0xb7,0x1d,0xc8,0x9c,0x49,0xe3,0x36,0x19,0xcc,0x66,0xb3,0xe7,0x32,0x98,0x4d,0x30,0xe5,0x4f,0x9a,0xce,
                0x1b,0xb1,0x64,0x72,0xa7,0x0d,0xd8,0x8c,0x59,0xf3,0x26,0x5b,0x8e,0x24,0xf1,0xa5,0x70,0xda,0x0f,0x20,0xf5,
                0x5f,0x8a,0xde,0x0b,0xa1,0x74,0x09,0xdc,0x76,0xa3,0xf7,0x22,0x88,0x5d,0xd6,0x03,0xa9,0x7c,0x28,0xfd,0x57,
                0x82,0xff,0x2a,0x80,0x55,0x01,0xd4,0x7e,0xab,0x84,0x51,0xfb,0x2e,0x7a,0xaf,0x05,0xd0,0xad,0x78,0xd2,0x07,
                0x53,0x86,0x2c,0xf9};


        byte GetCRC(byte[] message)
        {
            byte remainder = 0x00;
            byte i;
            int data;
            for (i = 0; i < message.Length; i++)
            {
                data = message[i] ^ remainder;
                remainder = crc8Table[data];
            }
            return remainder;

        }
        /// <summary>
        /// 计算 Modbus RTU 协议的 CRC16 校验码（字节数组版本）
        /// </summary>
        /// <param name="bytes">待校验的字节数组</param>
        /// <returns>2字节的 CRC16 校验码</returns>
        public static byte[] CalculateModbusCrc(byte[] bytes)
        {
            ushort crc = 0xFFFF;
            foreach (byte b in bytes)
            {
                crc ^= b;
                for (int i = 0; i < 8; i++)
                {
                    bool lsb = (crc & 1) == 1;
                    crc >>= 1;
                    if (lsb)
                        crc ^= 0xA001;
                }
            }
            return new byte[] { (byte)(crc & 0xFF), (byte)(crc >> 8) };
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
            RefreshComList(GV.ComRotaryValve);
        }

        private void nmudSV_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                GV.speedRotaryValueA = Convert.ToDouble(nmudSV.Value);
                SetPV(GV.speedRotaryValueA);//设置速度
                                           // txtSV_real.Text = ((double)nmudSV.Value * 10).ToString("0");
                if (!GV.PrintingObj.IsPrinting) return;
                // GV.frmPrintStep3.SetRotateSpeed(GV.speedRotaryValve);
                // GV.frmPrintStep2.SetDefaultRotateSpeed(GV.speedRotaryValve);
            }
            catch (Exception ex)
            {
            }
        }

        private void btnSetZero_Click(object sender, EventArgs e)
        {
            SendZeroSpeed();
        }

        private void FrmRotaryValveCtrl_Load(object sender, EventArgs e)
        {
            //  RefreshComList(GV.ComRotaryValve);
        }

        private void btnOpenCOM1_Click(object sender, EventArgs e)
        {
            OpenSerialPort();
        }

        public void OpenSerialPort()
        {
            System.Windows.Forms.Button btnConnectSerialPort = btnOpenCOM1;
            System.Windows.Forms.ComboBox cmbComPort;
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
                    SetSpeedMode();

                    btnConnectSerialPort.Text = "关闭串口";
                    button1.Enabled = true;
                    cmbComPort.Enabled = false;
                    sConnected = true;
                    panel1.Enabled = true;
                    //GV.SetRotaryValveConnected(true);
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
                    timerLoadRotation.Stop();
                    panel1.Enabled = false;
                    //GV.SetRotaryValveConnected(false);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                sConnected = false;
                //GV.SetRotaryValveConnected(false);
                return;
            }
        }

        private void btnReadData_Click(object sender, EventArgs e)
        {
            double pv = ReadPV();
            if (pv != ERR_VALUE)
            {
                txtPV_real.Text = pv.ToString();
            }
        }

        private void FrmRotaryValveCtrl_Click(object sender, EventArgs e)
        {
            //this.Text = this.Location.ToString();   
        }

        public void btnRotate_Click(object sender, EventArgs e)
        {
            MotorRun(1);
        }
        public void MotorRun(int valve)
        {
            //01 06 00 03 00 01 B8 0A 停止
            //01 06 00 01 00 03 98 0B  正转          
            if (valve == 1)
            {
                SendSpeedSetCmd();
                //strSend = strCmdStartRotaryValve; /*"01 06 00 01 00 03 98 0B";*//*"AA AA 01 00 02 04 00 00 00 00 00";*/
                //hexSend = GetHexData4Sending(strSend);
                //ExcuteCommand(serialPort1, hexSend);
                //txtCommand.Text = Buf2HexString(hexSend);
                //bOnOff = false;
            }
            else
            {
                SendZeroSpeed();
            }
            //string strSend = strCmdStartRotaryValve; /*"01 06 00 01 00 03 98 0B";*//*"AA AA 01 00 02 04 00 00 00 00 00";*/
            //byte[] hexSend = GetHexData4Sending(strSend);
            //ExcuteCommand(serialPort1, hexSend);
            //txtCommand.Text = Buf2HexString(hexSend);
            //bOnOff = false;
        }

        private void txtReceived_TextChanged(object sender, EventArgs e)
        {

        }
        //读取时间序列的转速
        private DateTime startTime;
        private int timeIndex = 0;//时间序列的索引
        private double lastRotValue = 0; // 记录上一次设置的转速
        private void timerLoadRotation_Tick(object sender, EventArgs e)
        {
            // 计算经过的时间（min）
            double elapsedTime = (DateTime.Now - startTime).TotalMinutes;
            // 更新界面显示的时间（每秒刷新）
            lblTimeRotation.Text = $"{elapsedTime:F2} 分钟 | 当前转速: {lastRotValue} RPM";

            // 检查是否所有数据已读完
            if (timeIndex >= dGVTimeRotation.Rows.Count)
            {
                timerLoadRotation.Stop();
                lblTimeRotation.Text += " (已完成)";
                return;
            }
            // 获取当前行的时间值，单位为分钟
            double currentRowTime = Convert.ToDouble(dGVTimeRotation.Rows[timeIndex].Cells[0].Value);
            double rotValue = Convert.ToDouble(dGVTimeRotation.Rows[timeIndex].Cells[1].Value);
            bool extrude = GV.Isextrude;

            // 如果经过的时间大于或等于当前行的时间，则读取该行的值
            if (elapsedTime >= currentRowTime)
            {
                //设置转速
                SetPV(rotValue);
                lastRotValue = rotValue;
                SendSpeedSetCmd();//发送转速
                // 移动到下一行
                timeIndex++;
            }
            if (extrude)
            {
                SendSpeedSetCmd();//发送转速
            }
        }
        public void SetStartPrint(bool isStarting)
        {
            if (isStarting && GV.timeRotation == true)
            {
                startTime = DateTime.Now; // 记录开始时间
                timeIndex = 0; // 重置索引
                timerLoadRotation.Enabled = true;
                timerLoadRotation.Start();
            }
            else
            {
                timerLoadRotation.Stop();
            }
        }

        private void dGVTimeRotation_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void btnSetTimeRotation_Click(object sender, EventArgs e)
        {
            dGVTimeRotation.Rows.Add();
        }

        private void btnDeleteRow_Click(object sender, EventArgs e)
        {
            if (dGVTimeRotation.CurrentRow != null)
            {
                dGVTimeRotation.Rows.Remove(dGVTimeRotation.CurrentRow);
            }
            else
            {
                MessageBox.Show("请先选择一行要删除的数据", "提示");
            }
        }
        //加载时间序列
        private void btnLoadSequence_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "CSV files (*.csv)|*.csv";
            openFileDialog1.DefaultExt = "csv";
            openFileDialog1.FileName = "";
            DialogResult dlgRslt;
            dlgRslt = openFileDialog1.ShowDialog();
            if (dlgRslt == DialogResult.OK)
            {
                string filepath = openFileDialog1.FileName;
                try
                {
                    dGVTimeRotation.Rows.Clear();
                    string[] lines = File.ReadAllLines(filepath);

                    for (int i = 1; i < lines.Length; i++)
                    {
                        string line = lines[i];
                        // 分割CSV行（考虑逗号分隔和引号包裹的情况）
                        string[] columns = line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        // 确保至少有前两列数据
                        if (columns.Length >= 2)
                        {
                            // 添加新行并填充前两列数据
                            int rowIndex = dGVTimeRotation.Rows.Add();
                            dGVTimeRotation.Rows[rowIndex].Cells[0].Value = columns[0].Trim('"', ' ');
                            dGVTimeRotation.Rows[rowIndex].Cells[1].Value = columns[1].Trim('"', ' ');
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"加载文件失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSaveSequence_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "CSV files (*.csv)|*.csv";
            saveFileDialog1.DefaultExt = "csv";
            saveFileDialog1.FileName = "转速序列";
            DialogResult dlgRslt;
            dlgRslt = saveFileDialog1.ShowDialog();
            if (dlgRslt == DialogResult.OK)
            {
                string fileName = saveFileDialog1.FileName;
                try
                {
                    if (File.Exists(fileName))
                    {
                        File.Delete(fileName);
                    }
                    // 创建StreamWriter写入文件
                    using (StreamWriter csvContent = new StreamWriter(fileName, false, Encoding.UTF8))
                    {
                        csvContent.WriteLine("时间(分钟),转速(RPM)");
                        // 遍历DataGridView的所有行
                        foreach (DataGridViewRow row in dGVTimeRotation.Rows)
                        {
                            // 跳过新行（未完成编辑的行）
                            if (!row.IsNewRow)
                            {
                                // 获取两列的值
                                string time = row.Cells[0].Value != null ? row.Cells[0].Value.ToString() : "";
                                string speed = row.Cells[1].Value != null ? row.Cells[1].Value.ToString() : "";

                                // 添加到CSV内容中
                                csvContent.WriteLine(string.Format("{0},{1}",time,speed));
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"保存文件失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void lblTimeRotation_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            timer1.Enabled = checkBox1.Checked;
        }
    }
}


