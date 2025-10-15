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
    public partial class FrmDCPower : Form
    {
        public FrmDCPower()
        {
            InitializeComponent();
            //m_VisaOpt = new CVisaOpt();
        }

        
        CVisaOpt m_VisaOpt;

        public void Connect()
        {
            label1.Text = "连接中...";
            label1.Refresh();
            string m_strResourceName = null; //仪器资源名
            string[] InstrResourceArray = m_VisaOpt.FindResource("?*INSTR"); //查找资源
            if (InstrResourceArray[0] == "未能找到可用资源!")
            {
                return;
            }
            else
            {
                //示例，选取Chroma编号为L01000000163仪器作为选中仪器
                for (int i = 0; i < InstrResourceArray.Length; i++)
                {
                    if (InstrResourceArray[i].Contains("USB0::0x0A69::0x087F"))
                    {
                        m_strResourceName = InstrResourceArray[i];
                    }
                }

            }
            //如果没有找到指定仪器直接退出
            if (m_strResourceName == null)
            {
                label1.Text = "连接失败";
                panel1.Enabled = false;
                return;
            }
            //打开指定资源
            m_VisaOpt.OpenResource(m_strResourceName);
            label1.Text = "已连接到 " + m_strResourceName;
            button1.Text = "断开连接";
            panel1.Enabled = true;
        }

        public void Disconnect()
        {
            m_VisaOpt.Release();
            button1.Text = "连接";
            label1.Text = "连接已断开";
            panel1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "连接")
            {
                Connect();
                SetVoltage(0);
                timer1.Start();
            }
            else
            {
                SetVoltage(0);
                ClosePower();
                Disconnect();
                timer1.Stop();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SendSCPI(textBox1.Text);
        }

        private void SendSCPI(string cmd)
        {
            //textBox1.Text = cmd;
            //m_VisaOpt.Write("");
            m_VisaOpt.Write(cmd);
            Thread.Sleep(50);
            if (m_VisaOpt.IsMessageAvailable())
            {
                Thread.Sleep(1);
                //读取命令
                string strback = m_VisaOpt.Read();
                textBox2.Text = strback;
            }
        }

        /// <summary>
        /// 打开程控电源
        /// </summary>
        public void OpenPower()
        {
            SendSCPI("OUTP ON");
        }

        /// <summary>
        /// 关闭程控电源
        /// </summary>
        public void ClosePower()
        {
            SendSCPI("OUTP OFF");
            ConnOn.Hide();
        }

        private double QueryValue(string strSCPI)
        {
            m_VisaOpt.Write(strSCPI);
            Thread.Sleep(10);
            if (m_VisaOpt.IsMessageAvailable())
            {
                //读取命令
                string strback = m_VisaOpt.Read();
                return Convert.ToDouble(strback);
            }
            return 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox1.Text = comboBox1.SelectedItem.ToString();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SendSCPI("OUTP ON");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SendSCPI("OUTP OFF");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SetVoltage(0);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SetVoltage(5);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SetVoltage(10);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            SetVoltage(24);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            SendSCPI("SYST:ERR?");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SendSCPI("*CLS");
        }

        private void button10_Click(object sender, EventArgs e)
        {
            try
            {
                SetVoltage(Convert.ToDouble(numericUpDown1.Value));
            }
            catch (Exception ex)
            {
            }
        }

        private void SetVoltage(double voltage)
        {
            if (voltage < 0)
            {
                voltage = 0;
            }
            else if (voltage > 24)
            {
                voltage = 24;
            }
            SendSCPI("VOLT " + voltage .ToString("0.000"));
            numericUpDown1.Value = (decimal)voltage;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            SendSCPI("CURR " + textBox4.Text);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            SendSCPI("APPL " + textBox3.Text + "," + textBox4.Text);
        }

        private void label2_Click(object sender, EventArgs e)
        {
            textBox3.Text = QueryValue("VOLT?").ToString();
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((char)Keys.Enter == e.KeyChar) 
            {
                
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            SendSCPI("VOLT:PROT:CLE");
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            try
            {
                string cmd = "VOLT?";
                m_VisaOpt.Write(cmd);
                Thread.Sleep(50);
                if (m_VisaOpt.IsMessageAvailable())
                {
                    Thread.Sleep(1);
                    //读取命令
                    string strback = m_VisaOpt.Read();
                    numericUpDown1.Value = (decimal)(Convert.ToDouble(strback));
                }
                else
                {
                    numericUpDown1.Value = 0;
                }

            }
            catch (Exception ex)
            {
            }
        }

        private void FrmDCPower_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = (e.CloseReason == CloseReason.UserClosing);
            this.Hide();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            SendSCPI("VOLT:PROT:CLE");
        }

        private void button17_Click(object sender, EventArgs e)
        {
            SetVoltage(15);
        }

        private void button18_Click(object sender, EventArgs e)
        {
            SetVoltage(20);
        }

        private void numericUpDown1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                SetVoltage((double)numericUpDown1.Value);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                string cmd = "OUTP?";
                m_VisaOpt.Write(cmd);
                Thread.Sleep(50);
                if (m_VisaOpt.IsMessageAvailable())
                {
                    Thread.Sleep(1);
                    //读取命令
                    string strback = m_VisaOpt.Read();
                    ConnOn.Visible = (strback == "1");
                }
                else
                {
                    //ConnOn.Visible = false;
                }

            }
            catch (Exception ex)
            {
            }
        }

    }
}
