using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AMCP
{
    public partial class FrmSetPressure : Form
    {
        ADAM adam;

        public FrmSetPressure()
        {
            InitializeComponent();
            adam = new ADAM();
        }

        public void StartStop()
        {
            if (timer2.Enabled)
            {
                timer2.Stop();
                adam.Disconnect();

                buttonStart.Text = "打开串口";
                label13.Text = "连接断开";
                comboBox2.Enabled = true;
                GV.frmMain.SetPressureConnected(false);
                panel1.Enabled = false;
            }
            else
            {
                label13.Text = "正在连接，请稍候...";
                label13.Refresh();
                buttonStart.Enabled = false;
                if (adam.Connect())
                {
                    timer2.Start();
                    timer1.Stop();
                    buttonStart.Text = "关闭串口";
                    label13.Text = "连接成功!";
                    comboBox2.Enabled = false;
                    panel1.Enabled = true;
                    GV.frmMain.SetPressureConnected(true);
                }
                else 
                {
                    label13.Text = "连接失败!";
                }
                buttonStart.Enabled = true;
            }
        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            StartStop();   
        }
        //读取亚当模块通道的值
        private void timer2_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!adam.IsOpen())
                {
                    label13.Text = "连接断开";
                    buttonStart.Text = "开始";
                    timer1.Start();
                    return;
                }
                double a = Convert.ToDouble(textBox2.Text);
                double b = Convert.ToDouble(textBox3.Text);
                double c = Convert.ToDouble(textBox5.Text);
                double d = Convert.ToDouble(textBox6.Text);
                double value = 0, valuePhysical;

                // 获取模拟输入值0
                //value = adam.GetAIValue(0);
                if (value != -1)
                {           
                    txtAIValue0.Text = value.ToString("0.000");
                    valuePhysical = value * a + b;
                    textBox1.Text = valuePhysical.ToString("0.0");
                }
                

                // 获取模拟输入值1
                value = adam.GetAIValue(1);
                valuePhysical = value * a + b;
                textBox8.Text = valuePhysical.ToString("0.0");


                //获取模拟输入值2 位移笔
                value = adam.GetAIValue(0);
                if (value != -1)
                {
                    txtVolt.Text = value.ToString("0.000");         
                    double a1 = Convert.ToDouble(textBox11.Text);
                    double b1 = Convert.ToDouble(textBox12.Text);
                    if (value < b1)
                    {
                        valuePhysical = 0;
                    }
                    else if (value != -1)
                    {
                        valuePhysical = value * a1 + b1;
                    }
                    else
                    {
                        //GV.PrintingObj.Ch.SetOutput(0, 11, 1);
                        valuePhysical = 0;
                    }
                    GV.valueDisplacementSensor = valuePhysical;
                    GV.valueDisplacementSensor_ACS = GV.PrintingObj.Ch.GetAnalogInput(0) * 0.0245;
                    txtDis.Text = valuePhysical.ToString("0.000");
                    //textBox8.Text = valuePhysical.ToString("0.0");
                    //textBox8.Text = value.ToString("0.000");

                }
                // 获取模拟输出值0
                if (enableAout0Update)
                {
                    value = adam.GetAOValue(0);
                    double y = (value - d) / c;  // value = c * y + d;
                    numericUpDown1.Value = (decimal)y;
                }

                // 获取模拟输出值1
                if (enableAout1Update)
                {
                    value = adam.GetAOValue(1);
                    double y = (value - d) / c;  // value = c * y + d;
                    numericUpDown2.Value = (decimal)y;
                }
            }
            catch (Exception ex)
            {
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;
            //直接刷新串口，进行连接气压
            //RefreshComList();
            //timer4.Start();//等确认串口号后自动连接
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            double y = Convert.ToDouble(numericUpDown1.Value);
            SetPressure(0, y);
            GV.extrudePressValueA = y;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            double y = Convert.ToDouble(numericUpDown2.Value);
            SetPressure(1, y);
            GV.extrudePressValueB = y;
        }

        public void SetPressure(int iExtruder, double pressure)
        {
            double y = Math.Round(pressure, 0);
            double c = Convert.ToDouble(textBox5.Text);
            double d = Convert.ToDouble(textBox6.Text);
            double aout = c * y + d;
            aout = Math.Round(aout, 2);
            adam.SetAOValue(iExtruder, aout);
        }

        private void comboBox2_Enter(object sender, EventArgs e)
        {
            //RefreshComList();
        }

        //刷新串口列表
        private void RefreshComList(string defaultName = "")
        {
            string strOld = comboBox2.Text;
            int indexNew = 0;
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            comboBox2.Items.Clear();
            //if (ports.Length > 0)
            {
                for (int i = 0; i < ports.Length; i++)
                {
                    comboBox2.Items.Add(ports[i]);
                    if (strOld == ports[i])
                    {
                        indexNew = i;
                    }
                }
                comboBox2.SelectedIndex = indexNew;
                StartStop();
            }      
            //else
            //{

            //}
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int portNum = Convert.ToInt32(comboBox2.SelectedItem.ToString().Substring(3));
            adam.SetPortNum(portNum);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            StartStop();
        }

        private void comboBox2_Click(object sender, EventArgs e)
        {
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void FrmSetPressure_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = (e.CloseReason == CloseReason.UserClosing);
            this.Hide();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }


        private void label12_Click(object sender, EventArgs e)
        {
            RefreshComList();
        }


        bool enableAout0Update = true, enableAout1Update = true;

        private void textBox4_Enter(object sender, EventArgs e)
        {
            enableAout0Update = false;
        }

        private void textBox9_Enter(object sender, EventArgs e)
        {
            enableAout1Update = false;
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            enableAout0Update = true;
        }

        private void textBox9_Leave(object sender, EventArgs e)
        {
            enableAout1Update = true;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            double y = Convert.ToDouble(numericUpDown1.Value);
            textBox4.Text = (y * 0.14503).ToString("0.0");
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            double y = Convert.ToDouble(numericUpDown2.Value);
            textBox9.Text = (y * 0.14503).ToString("0.0");
        }

        private void numericUpDown1_Enter(object sender, EventArgs e)
        {
            enableAout0Update = false;
        }

        private void numericUpDown1_Leave(object sender, EventArgs e)
        {
            enableAout0Update = true;
        }

        private void numericUpDown2_Enter(object sender, EventArgs e)
        {
            enableAout1Update = false;
        }

        private void numericUpDown2_Leave(object sender, EventArgs e)
        {
            enableAout1Update = true;
        }

        private void numericUpDown1_FontChanged(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_Leave_1(object sender, EventArgs e)
        {
            timer3.Start();
        }

        private void numericUpDown1_Enter_1(object sender, EventArgs e)
        {
            enableAout0Update = false;
            enableAout1Update = false;
        }

        private void numericUpDown1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            SetPressure(0, 0);
            numericUpDown1.Value = 0;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SetPressure(1, 0);
            numericUpDown2.Value = 0;
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            enableAout0Update = true;
            enableAout1Update = true;
            timer3.Stop();
        }

        private void numericUpDown1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                double y = Convert.ToDouble(numericUpDown1.Value);
                SetPressure(0, y);
            }
        }

        private void numericUpDown2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                double y = Convert.ToDouble(numericUpDown2.Value);
                SetPressure(1, y);
            }
        }

        private void numericUpDown1_Click(object sender, EventArgs e)
        {
            enableAout0Update = false;
        }

        private void btnPort2Open_Click(object sender, EventArgs e)
        {
            if (btnPort2Open.Text == "气压2打开")
            {
                GV.PrintingObj.Extrude(1, 1);
                btnPort2Open.Text = "气压2关闭";
            }
            else if (btnPort2Open.Text == "气压2关闭")
            {
                GV.PrintingObj.Extrude(1, 0);
                btnPort2Open.Text = "气压2打开";
            }
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            RefreshComList(GV.ComADAM);
            timer4.Stop();
        }

        private void numericUpDown2_Click(object sender, EventArgs e)
        {
            enableAout1Update = false;
        }

    }
}
