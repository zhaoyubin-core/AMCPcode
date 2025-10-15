using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;

namespace AMCP
{
    public partial class FrmDiagnose : Form
    {
        public FrmDiagnose()
        {
            InitializeComponent();
        }

        private void btnCheck1_Click(object sender, EventArgs e)
        {
            lblCheck1.Text = "正在检测...";
            lblCheck1.ForeColor = Color.FromArgb(64, 64, 64);
            picCheck1.Hide();
            lblCheck1.Refresh();
            try
            {
                if (!GV.seriPort1.IsOpen)
                {
                    GV.seriPort1.Open();
                    System.Threading.Thread.Sleep(30);
                }
                QueryTemperature(GV.seriPort1);
            }
            catch (Exception ex)
            {
                lblCheck1.Text = "温度传感器检测失败！原因：" + ex.Message;
                lblCheck1.ForeColor = Color.Red;
                picCheck1.Hide();
            }
            btnCheck1.Enabled = true;
        }

        private void btnCheck2_Click(object sender, EventArgs e)
        {
            lblCheck2.Text = "正在检测...";
            lblCheck2.ForeColor = Color.FromArgb(64, 64, 64);
            lblCheck2.Refresh();
            picCheck2.Hide(); 
            try
            {
                if (!GV.seriPort2.IsOpen)
                {
                    GV.seriPort2.Open();
                }
                QueryPressure(GV.seriPort2);
                GV.seriPort2.Close();
            }
            catch (Exception ex)
            {
                lblCheck2.Text = "压力传感器检测失败！原因：" + ex.Message;
                lblCheck2.ForeColor = Color.Red;
                picCheck2.Hide();
            }
            btnCheck2.Enabled = true;
        }

        private void btnCheck3_Click(object sender, EventArgs e)
        {
            lblCheck3.Text = "正在检测...";
            lblCheck3.ForeColor = Color.FromArgb(64, 64, 64);
            picCheck3.Hide();
            lblCheck3.Refresh();
            try
            {
                if (!GV.seriPort3.IsOpen)
                {
                    GV.seriPort3.Open();
                }
                QueryHighVoltagePower(GV.seriPort3);
                GV.seriPort3.Close();
            }
            catch (Exception ex)
            {
                lblCheck3.Text = "高压电源检测失败！原因：" + ex.Message;
                lblCheck3.ForeColor = Color.Red;
                picCheck3.Hide();
            }
            btnCheck3.Enabled = true;
        }

        private void btnCheck4_Click(object sender, EventArgs e)
        {
            lblCheck4.Text = "正在检测...";
            lblCheck4.ForeColor = Color.FromArgb(64, 64, 64);
            picCheck4.Hide();
            lblCheck4.Refresh();
            try
            {
                if (!GV.seriPort4.IsOpen)
                {
                    GV.seriPort4.Open();
                }
                QueryPumpController(GV.seriPort4);
                btnCheck4.Enabled = true;
                GV.seriPort4.Close();
            }
            catch (Exception ex)
            {
                lblCheck4.Text = "注射泵检测失败！原因：" + ex.Message;
                lblCheck4.ForeColor = Color.Red;
                picCheck4.Hide();
            }
            btnCheck4.Enabled = true;
        }

        public void QueryTemperature(SerialPort seriPort)
        {
            // 03 04 00 00 00 06 71 EA 
            byte[] sendBuffer = new byte[8];
            sendBuffer[0] = 0x03;
            sendBuffer[1] = 0x04;
            sendBuffer[2] = 0x00;
            sendBuffer[3] = 0x00;
            sendBuffer[4] = 0x00;
            sendBuffer[5] = 0x06;
            sendBuffer[6] = 0x71;
            sendBuffer[7] = 0xEA;
            seriPort.Write(sendBuffer, 0, sendBuffer.Length);

            Thread.Sleep(700);
            int countBytesToRead = seriPort.BytesToRead;
            int temp = 0;
            string strTemp = "";
            if (countBytesToRead >= 17)
            {
                byte[] readBuff = new byte[17];
                seriPort.Read(readBuff, 0, readBuff.Length);
                seriPort.ReadExisting();
                // 03 04 0C 00 C2 00 60 00 9C 00 00 00 00 00 01 D5 C6
                if (readBuff[0] == 0x03 && readBuff[1] == 0x04 && readBuff[2] == 0x0C)
                {
                    temp = readBuff[3] * 256 + readBuff[4];
                    temp = Convert.ToInt16(temp.ToString("X4"), 16);
                    strTemp = (temp * 0.1).ToString("0.0");
                    lblCheck1.Text = "检测到温度传感器工作正常, 成功读取到温度值：" + strTemp + " ℃";
                    lblCheck1.ForeColor = Color.DarkGreen;
                    picCheck1.Show();
                    return;
                }
            }
            lblCheck1.Text = "温度传感器检测失败！请检查相关设备是否连通电源，串口转USB连线是否接触良好。";
            lblCheck1.ForeColor = Color.Red;
            picCheck1.Hide();
        }


        public void QueryPressure(SerialPort seriPort)
        {
            // AA AA AA 01 A1 00 00 0A 
            byte[] sendBuffer = new byte[8];
            sendBuffer[0] = 0xAA;
            sendBuffer[1] = 0xAA;
            sendBuffer[2] = 0xAA;
            sendBuffer[3] = 0x01;
            sendBuffer[4] = 0xA1;
            sendBuffer[5] = 0x00;
            sendBuffer[6] = 0x00;
            sendBuffer[7] = 0x0A;

            seriPort.Write(sendBuffer, 0, sendBuffer.Length);

            Thread.Sleep(700);
            int countBytesToRead = seriPort.BytesToRead;
            int valueP = 0;
            string strTemp = "";
            if (seriPort.BytesToRead >= 10)
            {
                // 监测压力值
                byte[] readBuff = new byte[10];
                seriPort.Read(readBuff, 0, readBuff.Length);
                seriPort.ReadExisting();
                // BB BB BB 01 A1 XX XX 01 FF E5
                if (readBuff[0] == 0xBB && readBuff[1] == 0xBB && readBuff[2] == 0xBB && readBuff[3] == 0x01 && readBuff[4] == 0xA1)
                {
                    valueP = readBuff[5] * 256 + readBuff[6];
                    valueP = Convert.ToInt16(valueP.ToString("X4"), 16);
                    strTemp = valueP.ToString("0");
                    lblCheck2.Text = "检测到压力传感器工作正常, 成功读取到压力值：" + strTemp + " g";
                    lblCheck2.ForeColor = Color.DarkGreen;
                    picCheck2.Show();
                    return;
                }
                // BB BB BB 01 A2 00 00 XX XX XX
                else if (readBuff[0] == 0xBB && readBuff[1] == 0xBB && readBuff[2] == 0xBB && readBuff[3] == 0x01 && readBuff[4] == 0xA2)
                {
                    valueP = 0;
                }
            }
            lblCheck2.Text = "压力传感器检测失败！请检查相关设备是否连通电源，串口连线是否接触良好。";
            lblCheck2.ForeColor = Color.Red;
            picCheck2.Hide();
        }

        public void QueryHighVoltagePower(SerialPort seriPort)
        {
            // Tx:01 05 00 0A FF 00 AC 38 
            byte[] sendBuffer = new byte[8];
            sendBuffer[0] = 0x01;
            sendBuffer[1] = 0x05;
            sendBuffer[2] = 0x00;
            sendBuffer[3] = 0x0A;
            sendBuffer[4] = 0xFF;
            sendBuffer[5] = 0x00;
            sendBuffer[6] = 0xAC;
            sendBuffer[7] = 0x38;

            seriPort.Write(sendBuffer, 0, sendBuffer.Length);

            Thread.Sleep(700);
            int countBytesToRead = seriPort.BytesToRead;
            int valueP = 0;
            string strTemp = "";
            if (seriPort.BytesToRead >= 8)
            {
                // 监测电压值
                byte[] readBuff = new byte[8];
                seriPort.Read(readBuff, 0, readBuff.Length);
                seriPort.ReadExisting();
                // Rx:01 05 00 0A FF 00 AC 38 
                if (readBuff[0] == 0x01 && readBuff[1] == 0x05 && readBuff[2] == 0x00 && readBuff[3] == 0x0A && readBuff[4] == 0xFF
                     && readBuff[5] == 0x00 && readBuff[6] == 0xAC && readBuff[7] == 0x38)
                {
                    lblCheck3.Text = "检测到高压电源工作正常";
                    lblCheck3.ForeColor = Color.DarkGreen;
                    picCheck3.Show();
                    return;
                }
            }
            lblCheck3.Text = "高压电源检测失败！请检查设备是否开机，USB连线是否接触良好。";
            lblCheck3.ForeColor = Color.Red;
            picCheck3.Hide();
        }

        public void QueryPumpController(SerialPort seriPort)
        {
            byte[] bufferSend = GetBuffer(PDU_RS());
            SendPumpBuffer(bufferSend, seriPort);

            Thread.Sleep(700);
            int countBytesToRead = seriPort.BytesToRead;
            if (countBytesToRead > 2)
            {
                byte[] readBuff = new byte[countBytesToRead];
                seriPort.Read(readBuff, 0, readBuff.Length);
                if (readBuff[0] == 0xE9 && readBuff[2] == 0x03 && readBuff[3] == 82 && readBuff[4] == 83)
                {
                    lblCheck4.Text = "检测到注射泵工作正常";
                    lblCheck4.ForeColor = Color.DarkGreen;
                    picCheck4.Show();
                    return;
                }
            }
            lblCheck4.Text = "注射泵检测失败！请检查设备是否开机，USB连线是否接触良好。";
            lblCheck4.ForeColor = Color.Red;
            picCheck4.Hide();
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
        

        private void SendPumpBuffer(byte[] buffer, SerialPort seriPort)
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
            seriPort.Write(buffer, 0, buffer.Length);//发送sendBuffer
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
                buf[3 + i] = arrPdu[i];
                fcs = fcs ^ arrPdu[i];
            }

            // 添加校验码到buffer
            buf[3 + lenPdu] = (byte)fcs;
            return buf;
        }


        int indexChecking = 0;

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Interval = 1500;
            switch (indexChecking)
            {
                case 0:
                    btnCheck1_Click(sender, e);
                    indexChecking++;
                    break;
                case 1:
                    btnCheck2_Click(sender, e);
                    indexChecking++;
                    break;
                case 2:
                    btnCheck3_Click(sender, e);
                    indexChecking++;
                    break;
                case 3:
                    btnCheck4_Click(sender, e);
                    indexChecking++;
                    break;
                default:
                    indexChecking = 0;
                    btnComplete.Enabled = true;
                    btnRestart.Enabled = true;  
                    timer1.Stop();
                    break;
            }
        }

        private void FrmDiagnose_Enter(object sender, EventArgs e)
        {
            timer1.Interval = 500;
            lblCheck1.Text = "待检测";
            lblCheck1.ForeColor = Color.FromArgb(64, 64, 64);
            picCheck1.Hide();
            btnCheck1.Enabled = false;
            lblCheck2.Text = "待检测";
            lblCheck2.ForeColor = Color.FromArgb(64, 64, 64);
            picCheck2.Hide();
            btnCheck2.Enabled = false;
            lblCheck3.Text = "待检测";
            lblCheck3.ForeColor = Color.FromArgb(64, 64, 64);
            picCheck3.Hide();
            btnCheck3.Enabled = false;
            lblCheck4.Text = "待检测";
            lblCheck4.ForeColor = Color.FromArgb(64, 64, 64);
            picCheck4.Hide();
            btnCheck4.Enabled = false;
            btnComplete.Enabled = false;
            timer1.Start();
        }

        private void FrmDiagnose_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = (e.CloseReason == CloseReason.UserClosing);
            this.Hide();
        }

        private void btnComplete_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            FrmDiagnose_Enter(sender, e);
        }
    }
}
