using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Threading;

namespace AMCP
{
    public partial class FrmSyringePump : Form
    {
        private SerialPort serialPortE;

        private string strReceived = "";
        //变量的定义
        private double pres = 0.0;

        private int chanel = 1;//0表示电压, 1表示电流，2表示容量，3表示温度，4表示内阻
        private double data_v;//接收的数据（电压，V）
        private double data_c;//接收的数据（电流，A）

        private long nCycle;  //循环次数
        private long nReceived; //最后一次收到数据的循环次数


        SerialPort ComPort = new SerialPort();//声明一个串口      
        private string[] ports;//可用串口数组
        private bool recStaus = true;//接收状态字
        private bool ComPortIsOpen = false;//COM口开启状态字，在打开/关闭串口中使用，这里没有使用自带的ComPort.IsOpen，因为在串口突然丢失的时候，ComPort.IsOpen会自动false，逻辑混乱
        private bool Listening = false;//用于检测是否没有执行完invoke相关操作，仅在单线程收发使用，但是在公共代码区有相关设置，所以未用#define隔离
        private bool WaitClose = false;//invoke里判断是否正在关闭串口是否正在关闭串口，执行Application.DoEvents，并阻止再次invoke ,解决关闭串口时，程序假死，具体参见http://news.ccidnet.com/art/32859/20100524/2067861_4.html 仅在单线程收发使用，但是在公共代码区有相关设置，所以未用#define隔离//可用串口集合
        IList<customer> comList = new List<customer>();//可用串口集合

        string strSTX = "02";
        string strETX = "03";
        string strEOT = "04";
        string strENQ = "05";
        string strACK = "06";
        char STX = (char)(2);
        char ETX = (char)(3);
        char EOT = (char)(4);
        char ENQ = (char)(5);
        char ACK = (char)(6);

        double Pc = 0, Pp = 0;

        bool bWorking = false;

        public FrmSyringePump()
        {
            InitializeComponent();
            serialPortE = serialPort1;

            nCycle = 0;
            nReceived = -1;
            //RefreshComList();
        }

        //刷新串口列表
        private void RefreshComList()
        {
            //↓↓↓↓↓↓↓↓↓可用串口下拉控件↓↓↓↓↓↓↓↓↓
            AvailableComCbobox.Items.Clear();
            ports = System.IO.Ports.SerialPort.GetPortNames();//获取可用串口
            if (ports.Length > 0)//ports.Length > 0说明有串口可用
            {
                btnOpenCOM.Enabled = true;
                for (int i = 0; i < ports.Length; i++)
                {
                    //comList.Add(new customer() { com = ports[i] });//下拉控件里添加可用串口
                    //AvailableComCbobox.Items.Add(comList[i].com);
                    AvailableComCbobox.Items.Add(ports[i]);
                }
                AvailableComCbobox.SelectedIndex = 0;//默认选第1个串口
            }
            else//未检测到串口
            {
                btnOpenCOM.Enabled = false;
                //MessageBox.Show("无可用串口");
            }
            //↑↑↑↑↑↑↑↑↑可用串口下拉控件↑↑↑↑↑↑↑↑↑
        }


        private void button1_Click(object sender, EventArgs e)
        {
            SendHexData(textBox1.Text);  //发送请求通信指令
            Thread.Sleep(300);
            //char incoming = (char)serialPortE.ReadChar();
        }

        private void SendHexData(string sendData)
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
                return;//输入的16进制数据错误，无法发送，提示后返回  
            }
            serialPortE.Write(sendBuffer, 0, sendBuffer.Length);//发送sendBuffer
        }

        private void SendBuffer(byte[] buffer)
        {
            List<byte> listBuf = new List<byte>();
            listBuf.Add(buffer[0]);
            for (int i = 1; i < buffer.Length; i++)
            {
                // 若出现E8H，则以E8H、00H代替。若出现E9H，则以E8H、01H代替
                if (buffer[i] == 0xe8)
                {
                    listBuf.Add(0xe8);
                    listBuf.Add(0x00);
                }
                else if (buffer[i] == 0xe9)
                {
                    listBuf.Add(0xe8);
                    listBuf.Add(0x01);
                }
                else
                {
                    listBuf.Add(buffer[i]);
                }
            }
            buffer = listBuf.ToArray();
            //serialPortE.ReadExisting();
            //Thread.Sleep(100);
            serialPortE.Write(buffer, 0, buffer.Length);//发送sendBuffer
            //Thread.Sleep(100);
        }

        private void OpenSerialPort(object sender, EventArgs e)
        {
            if (btnOpenCOM.Text == "连接注射泵")
            {
                try
                {
                    serialPortE.PortName = AvailableComCbobox.SelectedItem.ToString();
                    serialPortE.BaudRate = 1200;            //设置当前波特率
                    serialPortE.Parity = Parity.Even;       //设置当前校验位
                    serialPortE.DataBits = 8;               //设置当前数据位
                    serialPortE.StopBits = StopBits.One;    //设置当前停止位                    

                    serialPortE.Open();                     //打开串口
                    btnOpenCOM.Text = "关闭连接";
                    button1.Enabled = true;

                    panel1.Enabled = true;
                    //timer1.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "请对通信串口进行配置！");
                    panel1.Enabled = false;
                    return;
                }
            }
            else
            {
                serialPortE.Close();
                btnOpenCOM.Text = "连接注射泵";
                panel1.Enabled = false;
                timer1.Stop();
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

        private string getCheckSum(string strOri)
        {
            //计算校验和
            int sum = 0;
            for (int i = 0; i < strOri.Length; i++)
            {
                sum += (int)(strOri[i]);
            }
            sum = ((~sum) + 1) & 0xff;
            return Chr2Hex(sum);// Convert.ToString(sum, 16).ToUpper().PadLeft(2, '0');
        }

        private string Chr2Hex(char chr)
        {
            return (Convert.ToString(chr, 16).ToUpper().PadLeft(2, '0'));
        }

        private string Chr2Hex(int chr)
        {
            return (Convert.ToString(chr, 16).ToUpper().PadLeft(2, '0'));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(txtCommand.Text);
            }
            catch (System.Exception)
            {
            	
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //string str = serialPortE.ReadExisting();
            //if (str.Length > 0)
            //{
            //    strReceived = "";
            //    for (int i = 0; i < str.Length; i++)
            //    {
            //        if (str[i] >= 'A' && str[i] <= 'Z' || str[i] >= 'a' && str[i] <= 'z' || str[i] >= '0' && str[i] <= '9')
            //        {
            //            strReceived += "[" + str[i] + "] ";
            //        }
            //        else
            //        {
            //            strReceived += ((byte)str[i]).ToString("X2") + " ";
            //        }
            //    }
            //}
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            btnPause(sender, e);

            //textBox1.Text = strReceived;
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

        private void button3_Click(object sender, EventArgs e)
        {
            txtCommand.Text = Clipboard.GetText();
        }



        private void txtConstantPresure_TextChanged(object sender, EventArgs e)
        {
            SetChanged();
        }

        private void txtPulsePresure_TextChanged(object sender, EventArgs e)
        {
            SetChanged();
        }

        private void txtPulseTime_TextChanged(object sender, EventArgs e)
        {
            SetChanged();
        }

        void SetChanged()
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            double t = 1.234;
            string strCmdData = t.ToString("0.0000");
            string str1 = strCmdData.Substring(0,1);
            string str2 = strCmdData.Substring(2);
        }

        private void btnOpen(object sender, EventArgs e)
        {
            OpenPump(1);
        }

        private void OpenPump(byte addr)
        {
            SendBuffer(GetBuffer(PDU_WS(1, 0, 0), addr));
            bWorking = true;
            panel2.Enabled = true;
        }

        private void ClosePump(byte addr)
        {
            
            SendBuffer(GetBuffer(PDU_WS(0, 0, 0), addr));
            bWorking = false;
            panel2.Enabled = false;
        }

        private void btnClose(object sender, EventArgs e)
        {
            ClosePump(1);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ClosePump(1);
            Thread.Sleep(150);
            ClosePump(2);

        }

        private void btnPause(object sender, EventArgs e)
        {
            OpenPump(1);
            Thread.Sleep(150);
            OpenPump(2);

            //int countBytesToRead = serialPort1.BytesToRead;
            //if (countBytesToRead > 8)
            //{
            //    byte[] readBuff = new byte[countBytesToRead];
            //    serialPort1.Read(readBuff, 0, readBuff.Length);
            //    if (readBuff[0] == 0xE9 && readBuff[2] == 0x03 && readBuff[3] == 82 && readBuff[4] == 83) 
            //    {
            //        if (readBuff[5] == 0)
            //        {
            //            lblRunStatus.Text = "停止";
            //        }
            //        else if (readBuff[5] == 1)
            //        {
            //            lblRunStatus.Text = "运行中";
            //        }
            //    }
            //}

            //byte[] bufferSend = GetBuffer(PDU_RS());
            //SendBuffer(bufferSend);
        }

        private string GenCmd_CWT1(int volume, byte volume_unit, int flowrate, byte fr_unit)
        {
            // 命令头
            string strFlag = "E9";
            int addr = 1;

            // 液量设置
            byte volume_low = (byte)(volume % 256);
            byte volume_high = (byte)(volume >> 8);

            // 流量设置
            byte fr_low = (byte)(flowrate % 256);
            byte fr_high = (byte)(flowrate >> 8);

            byte[] arrPdu = new byte[10];
            // 命令(灌注模式):
            arrPdu[0] = (byte)'C';
            arrPdu[1] = (byte)'W';
            arrPdu[2] = (byte)'T';
            arrPdu[3] = 1;
            // 液量
            arrPdu[4] = volume_low;
            arrPdu[5] = volume_high;
            arrPdu[6] = volume_unit;
            // 流量
            arrPdu[7] = fr_low;
            arrPdu[8] = fr_high;
            arrPdu[9] = fr_unit;

            // pdu的长度
            int len = arrPdu.Length;

            string strCmd = strFlag + " " + addr.ToString("X2") + " " + len.ToString("X2") + " ";
            int xorResult = addr ^ len;
            for (int i = 0; i < arrPdu.Length; i++)
            {
                strCmd += arrPdu[i].ToString("X2") + " ";
                xorResult = xorResult ^ arrPdu[i];
            }
            // 添加校验信息
            strCmd += xorResult.ToString("X2");
            return strCmd;
        }

        /// <summary>
        /// 1.5	发送启动、暂停和停止命令
        /// </summary>
        /// <param name="mode">0-停止; 1-启动; 2-暂停</param>
        /// <returns></returns>
        private string GenCmd_CWX(byte mode)
        {
            // 命令头
            string strFlag = "E9";
            int addr = 1;

            byte[] arrPdu = new byte[4];
            // 命令(灌注模式):
            arrPdu[0] = (byte)'C';
            arrPdu[1] = (byte)'W';
            arrPdu[2] = (byte)'X';
            arrPdu[3] = mode;

            // pdu的长度
            int len = arrPdu.Length;

            string strCmd = strFlag + " " + addr.ToString("X2") + " " + len.ToString("X2") + " ";
            int xorResult = addr ^ len;
            for (int i = 0; i < arrPdu.Length; i++)
            {
                strCmd += arrPdu[i].ToString("X2") + " ";
                xorResult = xorResult ^ arrPdu[i];
            }
            // 添加校验信息
            strCmd += xorResult.ToString("X2");
            return strCmd;
        }

        /// <summary>
        /// 读取运行状态信息 RX 0~2: 0-停止; 1-启动; 2-暂停
        /// </summary>
        /// <returns></returns>
        private string GenCmd_CRX()
        {
            // 命令头
            string strFlag = "E9";
            int addr = 1;

            byte[] arrPdu = new byte[3];
            // 命令(灌注模式):
            arrPdu[0] = (byte)'C';
            arrPdu[1] = (byte)'R';
            arrPdu[2] = (byte)'X';

            // pdu的长度
            int len = arrPdu.Length;

            string strCmd = strFlag + " " + addr.ToString("X2") + " " + len.ToString("X2") + " ";
            int xorResult = addr ^ len;
            for (int i = 0; i < arrPdu.Length; i++)
            {
                strCmd += arrPdu[i].ToString("X2") + " ";
                xorResult = xorResult ^ arrPdu[i];
            }
            // 添加校验信息
            strCmd += xorResult.ToString("X2");
            return strCmd;
        }

        private string GenHexCmd(string str)
        {
            // 命令头
            string strFlag = "E9";
            int addr = 1;

            byte[] arrPdu = new byte[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                arrPdu[i] = (byte)str[i];
            }

            // pdu的长度
            int len = arrPdu.Length;

            string strCmd = strFlag + " " + addr.ToString("X2") + " " + len.ToString("X2") + " ";
            int xorResult = addr ^ len;
            for (int i = 0; i < arrPdu.Length; i++)
            {
                strCmd += arrPdu[i].ToString("X2") + " ";
                xorResult = xorResult ^ arrPdu[i];
            }
            // 添加校验信息
            strCmd += xorResult.ToString("X2");
            return strCmd;
        }


        private void frmMain_Load(object sender, EventArgs e)
        {
            comboBox2.SelectedIndex = 1;
            comboBox4.SelectedIndex = 7;
            comboBox5.SelectedIndex = 1;
            comboBox6.SelectedIndex = 4;
            comboBox7.SelectedIndex = 1;
            comboBox8.SelectedIndex = 1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnSet_Click(object sender, EventArgs e)
        {
            if (bWorking)
            {
                int waitTime1 = (int)numericUpDown6.Value;
                int waitTime2 = (int)numericUpDown7.Value;
                ClosePump(1);
                Thread.Sleep(waitTime1);
                SetPara(1);
                Thread.Sleep(waitTime2);
                OpenPump(1);
            }
            else
            {
                SetPara(1);
            }
        }

        private void SetPara(byte addr)
        {
            byte mode = (byte)(comboBox2.SelectedIndex);
            byte rowPump = (byte)(comboBox4.SelectedIndex);
            byte colPump = (byte)(comboBox5.SelectedIndex);
            int volume = (int)(numericUpDown1.Value);
            byte volUnit = (byte)(comboBox6.SelectedIndex + 1);
            int timePumpOut = (int)(numericUpDown2.Value);
            byte pumpOutTimeUnit = (byte)(comboBox7.SelectedIndex + 1);
            int timePumpIn = (int)(numericUpDown3.Value);
            byte pumpInTimeUnit = (byte)(comboBox8.SelectedIndex + 1);
            int cycle = (int)nmbudCycle.Value;
            int timePause = (int)(numericUpDown4.Value * 10);
            byte[] pdu = PDU_WP(mode, rowPump, colPump, volume, volUnit, timePumpOut, pumpOutTimeUnit, timePumpIn, pumpInTimeUnit, cycle, timePause);
            SendBuffer(GetBuffer(pdu, addr));
            textBox1.Text = Buf2Hex(GetBuffer(pdu, 1));

        }

        private string Buf2Hex(byte[] pdu)
        {
            string str = "";
            for (int i = 0; i < pdu.Length; i++)
            {
                str += pdu[i].ToString("X2") + " ";
            }
            return str;
        }


        private string GenCmd_WP(byte mode = 1)
        {
            // 命令头
            string strFlag = "E9";
            int addr = 2;

            byte[] arrPdu = new byte[17];
            // 命令(设置运行参数):
            arrPdu[0] = (byte)'W';
            arrPdu[1] = (byte)'P';
            arrPdu[2] = mode;
            arrPdu[3] = 17; // 注射器编号：0x11 = 17第1行第1列
            arrPdu[4] = 1;  // 分配液量值1
            arrPdu[5] = 2;  // 分配液量值2
            arrPdu[6] = 3;  // 分配液量单位：0.1uL
            arrPdu[7] = 1;  // 灌注时间值1
            arrPdu[8] = 1;  // 灌注时间值2
            arrPdu[9] = 1;  // 灌注时间单位：0.1sec
            arrPdu[10] = 1; // 抽取时间值1
            arrPdu[11] = 1; // 抽取时间值2
            arrPdu[12] = 1; // 抽取时间单位：0.1sec
            arrPdu[13] = 0; // 分配次数值1
            arrPdu[14] = 1; // 分配次数值2
            arrPdu[15] = 0; // 间隔时间值1
            arrPdu[16] = 1; // 间隔时间值2


            // pdu的长度
            int len = arrPdu.Length;

            string strCmd = strFlag + " " + addr.ToString("X2") + " " + len.ToString("X2") + " ";
            int xorResult = addr ^ len;
            for (int i = 0; i < arrPdu.Length; i++)
            {
                strCmd += arrPdu[i].ToString("X2") + " ";
                xorResult = xorResult ^ arrPdu[i];
            }
            // 添加校验信息
            strCmd += xorResult.ToString("X2");
            return strCmd;
        }

        private string GenCmd_RP(byte mode = 1)
        {
            // 命令头
            string strFlag = "E9";
            int addr = 2;

            byte[] arrPdu = new byte[16];
            // 命令（读取运行参数）:
            arrPdu[0] = (byte)'R';
            arrPdu[1] = (byte)'P';
            arrPdu[2] = mode;
            arrPdu[3] = 17; // 注射器编号：0x11 = 17第1行第1列
            
            arrPdu[4] = 1;  // 分配液量值1
            arrPdu[5] = 1;  // 分配液量值2
            arrPdu[6] = 3;  // 分配液量单位：0.1uL
            
            arrPdu[7] = 1;  // 灌注时间值1
            arrPdu[8] = 1;  // 灌注时间值2
            arrPdu[9] = 1;  // 灌注时间单位：0.1sec
            
            arrPdu[10] = 1; // 抽取时间值1
            arrPdu[11] = 1; // 抽取时间值2
            arrPdu[12] = 1; // 抽取时间单位：0.1sec
            
            arrPdu[13] = 0; // 分配次数值1
            arrPdu[14] = 1; // 分配次数值2
            
            arrPdu[15] = 0; // 间隔时间值1
            arrPdu[15] = 1; // 间隔时间值2


            // pdu的长度
            int len = arrPdu.Length;

            string strCmd = strFlag + " " + addr.ToString("X2") + " " + len.ToString("X2") + " ";
            int xorResult = addr ^ len;
            for (int i = 0; i < arrPdu.Length; i++)
            {
                strCmd += arrPdu[i].ToString("X2") + " ";
                xorResult = xorResult ^ arrPdu[i];
            }
            // 添加校验信息
            strCmd += xorResult.ToString("X2");
            return strCmd;
        }

        /// <summary>
        /// 设置运行状态
        /// </summary>
        /// <param name="mode">1: 启动; 2: 停止</param>
        /// <returns></returns>
        private string GenCmd_WS(byte mode = 0)
        {
            // 命令头
            string strFlag = "E9";
            int addr = 2;

            byte[] arrPdu = new byte[3];
            // 命令(设置运行参数):
            arrPdu[0] = (byte)'W';
            arrPdu[1] = (byte)'S';
            arrPdu[2] = mode;

            // pdu的长度
            int len = arrPdu.Length;

            string strCmd = strFlag + " " + addr.ToString("X2") + " " + len.ToString("X2") + " ";
            int xorResult = addr ^ len;
            for (int i = 0; i < arrPdu.Length; i++)
            {
                strCmd += arrPdu[i].ToString("X2") + " ";
                xorResult = xorResult ^ arrPdu[i];
            }
            // 添加校验信息
            strCmd += xorResult.ToString("X2");
            return strCmd;
        }


        /// <summary>
        /// 设置运行状态
        /// </summary>
        /// <param name="mode">0: 停止; 1: 启动(反转); 2: 停止; 3: 启动(正转)</param>
        /// <returns></returns>
        private string GenCmd_IC(byte mode = 0)
        {
            // 命令头
            string strFlag = "E9";
            int addr = 2;

            byte[] arrPdu = new byte[3];
            // 命令(设置运行参数):
            arrPdu[0] = mode;
            arrPdu[1] = 1;
            arrPdu[2] = 1;

            // pdu的长度
            int len = arrPdu.Length;

            string strCmd = strFlag + " " + addr.ToString("X2") + " " + len.ToString("X2") + " ";
            int xorResult = addr ^ len;
            for (int i = 0; i < arrPdu.Length; i++)
            {
                strCmd += arrPdu[i].ToString("X2") + " ";
                xorResult = xorResult ^ arrPdu[i];
            }
            // 添加校验信息
            strCmd += xorResult.ToString("X2");
            return strCmd;
        }

        /// <summary>
        /// 根据PDU和地址生成待发送的buffer数据（byte数组）
        /// </summary>
        /// <param name="arrPdu"></param>
        /// <param name="addr"></param>
        /// <returns></returns>
        private byte[] GetBuffer(byte[] arrPdu, byte addr = 1)
        {
            // 命令头
            byte flag = 233; // "E9";
            
            // 检查pdu数据有效性
            if (arrPdu == null)
            {
                return null;
            }

            // pdu的长度
            int lenPdu = arrPdu.Length;

            // 帧格式: flag(1) + addr(1) + len(1) + pdu(len) + fcs(1)
            byte[] buf = new byte[3 + lenPdu + 1];
            buf[0] = flag;
            buf[1] = addr;
            buf[2] = (byte)lenPdu;
            
            // 计算校验码fcs（=addr、lenPdu、pdu的异或）
            int fcs = addr ^ lenPdu;
            for (int i = 0; i < lenPdu; i++)
            {
                buf[3+i] = arrPdu[i];
                fcs = fcs ^ arrPdu[i];
            }

            // 添加校验码到buffer
            buf[3 + lenPdu] = (byte)fcs;
            return buf;
        }

        /// <summary>
        /// 生成WS指令的PDU
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        private byte[] PDU_WS(byte mode = 0)
        {
            byte[] arrPdu = new byte[3];
            // 命令(设置运行参数):
            arrPdu[0] = (byte)'W';
            arrPdu[1] = (byte)'S';
            arrPdu[2] = mode;
            return arrPdu;
        }


        enum VolumeUnit
        {
            _nL = 1,
            _10nL = 2,
            _100nL = 3,
            _uL = 4,
            _10uL = 5
        }

        enum PumpTimeUnit
        {
            _0_1sec = 1,
            _0_1min = 2,
            _0_1hour = 3
        }

        /// <summary>
        /// 设置注射泵运行参数
        /// </summary>
        /// <param name="mode">工作模式: 1-灌注; 2-抽取; 3-先灌注后抽取; 4-先抽取后灌注; 5-连续</param>
        /// <param name="rowPump">注射器行号, 取值范围1-4</param>
        /// <param name="colPump">注射器列号, 取值范围1-8</param>
        /// <param name="volume">分配液量, 取值范围1-9999</param>
        /// <param name="volUnit">分配液量单位</param>
        /// <param name="timePumpOut">灌注时间, 取值范围1-9999</param>
        /// <param name="pumpOutTimeUnit">灌注时间单位</param>
        /// <param name="timePumpIn">抽取时间, 取值范围1-9999</param>
        /// <param name="pumpInTimeUnit">抽取时间单位</param>
        /// <param name="cycle">分配次数（循环次数）, 取值范围1-999</param>
        /// <param name="timePause">间隔时间（暂停时间）, 单位为0.1min, 取值范围1-9999</param>
        /// <returns>PDU数组</returns>
        private byte[] PDU_WP(byte mode, byte rowPump, byte colPump, int volume, byte volUnit, int timePumpOut, byte pumpOutTimeUnit,
            int timePumpIn, byte pumpInTimeUnit, int cycle, int timePause)
        { 
            byte[] arrPdu = new byte[17];
            // 命令(设置运行参数):
            arrPdu[0] = (byte)'W';
            arrPdu[1] = (byte)'P';
            arrPdu[2] = mode;
            arrPdu[3] = (byte)((rowPump << 4) + colPump); // 注射器编号：0x11 = 17第1行第1列
            arrPdu[4] = (byte)(volume >> 8);            // 分配液量值1
            arrPdu[5] = (byte)(volume % 256);           // 分配液量值2
            arrPdu[6] = (byte)(volUnit);                // 分配液量单位：0.1uL
            arrPdu[7] = (byte)(timePumpOut >> 8);       // 灌注时间值1
            arrPdu[8] = (byte)(timePumpOut % 256);      // 灌注时间值2
            arrPdu[9] = (byte)(pumpOutTimeUnit);        // 灌注时间单位：0.1sec
            arrPdu[10] = (byte)(timePumpIn >> 8);       // 抽取时间值1
            arrPdu[11] = (byte)(timePumpIn % 256);      // 抽取时间值2
            arrPdu[12] = (byte)(pumpInTimeUnit);        // 抽取时间单位：0.1sec
            arrPdu[13] = (byte)(cycle >> 8);            // 分配次数值1
            arrPdu[14] = (byte)(cycle % 256);           // 分配次数值2
            arrPdu[15] = (byte)(timePause >> 8);        // 间隔时间值1
            arrPdu[16] = (byte)(timePause % 256);       // 间隔时间值2        
            return arrPdu;
        }


        /// <summary>
        /// 设置运行状态
        /// </summary>
        /// <param name="bit0">启停状态位: 1-启动; 0-停止</param>
        /// <param name="bit1">快推状态位: 1-快推; 0-恢复</param>
        /// <param name="bit2">快拉状态位: 1-快拉; 0-恢复</param>
        /// <returns>PDU数组</returns>
        private byte[] PDU_WS(byte bit0, byte bit1, byte bit2)
        {
            byte[] arrPdu = new byte[3];
            // 命令(设置运行参数):
            arrPdu[0] = (byte)'W';
            arrPdu[1] = (byte)'S';
            arrPdu[2] = (byte)(bit0 + bit1 * 2 + bit2 * 4);
            return arrPdu;
        }

        /// <summary>
        /// 读取运行状态
        /// </summary>
        /// <returns>PDU数组</returns>
        private byte[] PDU_RS()
        {
            byte[] arrPdu = new byte[2];
            // 命令(设置运行参数):
            arrPdu[0] = (byte)'R';
            arrPdu[1] = (byte)'S';
            return arrPdu;
        }   
        
        private string GenCmdByPDU(string strPDU)
        {
            // 命令头
            string strFlag = "E9";
            int addr = 2;

            byte[] arrPdu = Hex2Buffer(strPDU);

            if (arrPdu == null)
            {
                return "";
            }

            // pdu的长度
            int len = arrPdu.Length;

            string strCmd = strFlag + " " + addr.ToString("X2") + " " + len.ToString("X2") + " ";
            int xorResult = addr ^ len;
            for (int i = 0; i < arrPdu.Length; i++)
            {
                strCmd += arrPdu[i].ToString("X2") + " ";
                xorResult = xorResult ^ arrPdu[i];
            }
            // 添加校验信息
            strCmd += xorResult.ToString("X2");
            return strCmd;
        }

        /// <summary>
        /// 将16进制字符串转换为可用于串口发送的byte数组
        /// </summary>
        /// <param name="hexData">Hex字符串</param>
        /// <returns>可用于串口发送的byte数组(</returns>
        private byte[] Hex2Buffer(string hexData)
        {
            byte[] sendBuffer = null;   //数据缓冲区

            try //尝试将16进制字符串转换为可用于串口发送的byte数组
            {
                hexData = hexData.Replace(" ", "");//去除16进制数据中所有空格
                hexData = hexData.Replace("\r", "");//去除16进制数据中所有换行
                hexData = hexData.Replace("\n", "");//去除16进制数据中所有换行
                if (hexData.Length == 1)//数据长度为1的时候，在数据前补0
                {
                    hexData = "0" + hexData;
                }
                else if (hexData.Length % 2 != 0)//数据长度为奇数位时，去除最后一位数据
                {
                    hexData = hexData.Remove(hexData.Length - 1, 1);
                }

                List<string> sendData16 = new List<string>();//将发送的数据，2个合为1个，然后放在该缓存里 如：123456→12,34,56
                for (int i = 0; i < hexData.Length; i += 2)
                {
                    sendData16.Add(hexData.Substring(i, 2));
                }
                sendBuffer = new byte[sendData16.Count];//sendBuffer的长度设置为：发送的数据2合1后的字节数
                for (int i = 0; i < sendData16.Count; i++)
                {
                    sendBuffer[i] = (byte)(Convert.ToInt32(sendData16[i], 16));//发送数据改为16进制
                }
                return sendBuffer;
            }
            catch //无法转为16进制时，出现异常
            {
                return null;
            }
        }

        private void btnOpenCOM2_Click(object sender, EventArgs e)
        {
            if (btnOpenCOM2.Text == "连接压力传感器")
            {
                try
                {
                    serialPortE.PortName = AvailableComCbobox.SelectedItem.ToString();
                    serialPortE.BaudRate = 9600;            //设置当前波特率
                    serialPortE.Parity = Parity.None;       //设置当前校验位
                    serialPortE.DataBits = 8;               //设置当前数据位
                    serialPortE.StopBits = StopBits.One;    //设置当前停止位                    

                    serialPortE.Open();                     //打开串口
                    btnOpenCOM2.Text = "关闭连接";
                    button1.Enabled = true;

                    panel2.Enabled = true;
                    timer2.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "请对通信串口进行配置！");
                    panel2.Enabled = false;
                    return;
                }
            }
            else
            {
                serialPortE.Close();
                btnOpenCOM2.Text = "连接压力传感器";
                panel2.Enabled = false;
                timer2.Stop();
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            if (comboBox5.SelectedIndex < comboBox5.Items.Count - 1)
            {
                comboBox5.SelectedIndex++;
                numericUpDown1.Value = rd.Next(10, 99);
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (comboBox5.SelectedIndex > 0)
            {
                comboBox5.SelectedIndex--;
            }
        }

        Random rd = new Random();

        private void label14_Click(object sender, EventArgs e)
        {
            numericUpDown1.Value = rd.Next(10, 99);
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            //if (textBox7.Text.StartsWith("BB BB BB 01 A1") && textBox7.Text.Substring(21, 5) == "01 FF") 
            //{
            //    string strHightByte = textBox7.Text.Substring(15, 2);
            //    string strLowByte = textBox7.Text.Substring(18, 2);
            //    int value = 0;
            //    value = Convert.ToByte(strHightByte, 16) * 256 + Convert.ToByte(strLowByte, 16);
            //    textBox8.Text = value.ToString();
            //}
        }

        /// <summary>
        /// 获取压力值
        /// </summary>
        /// <returns></returns>
        private int GetPressure()
        {
            SendHexData("AA AA AA 01 A1 00 00 0A");
            Thread.Sleep(30);
            byte[] arrBuff = new byte[10];
            serialPortE.Read(arrBuff, 0, 10);
            int value = 30000;

            // BB BB BB 01 A1 XX XX 01 FF E5
            if (arrBuff[0] == 187 && arrBuff[1] == 187 && arrBuff[2] == 187 && arrBuff[3] == 1 && arrBuff[4] == 161 &&
                arrBuff[7] == 1 && arrBuff[8] == 255) 
            {
                value = arrBuff[5] * 256 + arrBuff[6];
            }
            return value;
        }

        private void button13_Click(object sender, EventArgs e)
        {
            int p = GetPressure();
            textBox8.Text = p.ToString();

        }

        private void button11_Click(object sender, EventArgs e)
        {
            SendHexData(textBox3.Text);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            SendHexData(textBox5.Text);
        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                double rv1, rv2;
                double v0, v1, v2;
                v0 = (double)numericUpDown5.Value;
                rv1 = (double)numericUpDown8.Value;
                rv2 = (double)numericUpDown9.Value;
                v1 = rv1 / (rv1 + rv2) * v0;
                v2 = rv2 / (rv1 + rv2) * v0;
                numericUpDown10.Value = (decimal)v1;
                numericUpDown11.Value = (decimal)v2;

                UpdatePara(0);
            }
            catch (Exception ex)
            {
            }
        }

        private void UpdatePara(byte addr)
        {
            try
            {
                int volume = 1000;
                switch (comboBox5.SelectedIndex)
                {
                    case 1:
                        volume = 2000;
                        break;
                    case 2:
                        volume = 500;
                        break;
                    case 3:
                        volume = 1000;
                        break;
                    case 4:
                        volume = 3000;
                        break;
                    default:
                        break;
                }
                numericUpDown1.Value = (decimal)volume; // 分配液量 = volume * 0.01mL
                double flowrate;
                switch (addr)
	            {
                    case 1:
                        flowrate = (double)numericUpDown10.Value; // 流量（μL/min）
                        break;
                    case 2:
                        flowrate = (double)numericUpDown11.Value; // 流量（μL/min）
                        break;
                    default:
                        flowrate = (double)numericUpDown5.Value; // 流量（μL/min）
                        break;
	            }
                double time = volume * 10 / flowrate * 10; // 时间（0.1min）
                if (time > 9999)
                {
                    comboBox7.SelectedIndex = 2; // 0.1hour
                    time = time / 60;
                }
                else if (time >= 100)
                {
                    comboBox7.SelectedIndex = 1; // 0.1min
                }
                else
                {
                    comboBox7.SelectedIndex = 0; // 0.1sec
                    time = time * 60;
                }
                numericUpDown2.Value = (decimal)(Math.Round(time));
            }
            catch (Exception ex)
            {
            }
        }


        private void timer2_Tick(object sender, EventArgs e)
        {
        }

        private void btnOpen2_Click(object sender, EventArgs e)
        {
            OpenPump(2);
        }

        private void btnClose2_Click(object sender, EventArgs e)
        {
            ClosePump(2);
        }


        private void btnSet2_Click(object sender, EventArgs e)
        {
            if (bWorking)
            {
                ClosePump(2);
                Thread.Sleep(500);
                SetPara(2);
                OpenPump(2);
            }
            else
            {
                SetPara(2);
            }
            //byte mode = (byte)(comboBox2.SelectedIndex);
            //byte rowPump = (byte)(comboBox4.SelectedIndex);
            //byte colPump = (byte)(comboBox5.SelectedIndex);
            //int volume = (int)(numericUpDown1.Value);
            //byte volUnit = (byte)(comboBox6.SelectedIndex + 1);
            //int timePumpOut = (int)(numericUpDown2.Value);
            //byte pumpOutTimeUnit = (byte)(comboBox7.SelectedIndex + 1);
            //int timePumpIn = (int)(numericUpDown3.Value);
            //byte pumpInTimeUnit = (byte)(comboBox8.SelectedIndex + 1);
            //int cycle = (int)nmbudCycle.Value;
            //int timePause = (int)(numericUpDown4.Value * 10);
            //byte[] pdu = PDU_WP(mode, rowPump, colPump, volume, volUnit, timePumpOut, pumpOutTimeUnit, timePumpIn, pumpInTimeUnit, cycle, timePause);
            //SendBuffer(GetBuffer(pdu, 2));
            //textBox1.Text = Buf2Hex(GetBuffer(pdu,2));
        }

        private void label23_Click(object sender, EventArgs e)
        {

        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            int waitTime1 = 120;    // (int)numericUpDown6.Value;
            int waitTime2 = 0;      // (int)numericUpDown7.Value;
            int waitTime3 = 350;    // (int)numericUpDown12.Value;
            int waitTime4 = 550;    //

            if (bWorking)
            {
                ClosePump(1);
                UpdatePara(1);
                Thread.Sleep(waitTime1);
                SetPara(1);
                Thread.Sleep(waitTime2);
                OpenPump(1);

                Thread.Sleep(waitTime3);
                ClosePump(2);
                UpdatePara(2);
                Thread.Sleep(waitTime1);
                SetPara(2);
                Thread.Sleep(waitTime2);
                OpenPump(2);
            }
            else
            {
                if (numericUpDown10.Value > 0)
                {
                    UpdatePara(1);
                    SetPara(1);                    
                }
                if (numericUpDown11.Value > 0)
                {
                    Thread.Sleep(waitTime4);
                    UpdatePara(2);
                    SetPara(2);
                }

            }

        }

        private void FrmSyringePump_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = (e.CloseReason == CloseReason.UserClosing);
            this.Hide();
        }    
        

    }
}
    

