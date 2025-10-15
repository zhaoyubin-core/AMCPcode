using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.IO.Ports;


namespace AMCP
{
    public partial class FrmPrintStep3 : Form
    {
        //FrmMain GV;
        //IntPtr m_handle;  //雷塞控制器连接标识
        //bool pumpConnect = false;
        const int axisPump = 3;   // 注射泵的轴号

        public FrmPrintStep3()
        {
            InitializeComponent();
            txtTechParaFileName.Text = GV.TechParaFileName;
            InitParaControls();//初始化装置
            //UpdateTableControls();
            ReadAmtpFile(txtTechParaFileName.Text);
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            //GV = this.MdiParent as FrmMain;
        }

        private void btnNextStep3_Click(object sender, EventArgs e)
        {
            //XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes");
            //XmlElement root = xmlDoc.CreateElement("arguments");
            //xmlDoc.AppendChild(root);
          
            ////if (flag == "一般气源")
            //XmlElement element1 = xmlDoc.CreateElement("Type");
            //XmlAttribute newValue1 = xmlDoc.CreateAttribute("Name");
            //newValue1.Value = "挤出系统选择";
            //element1.Attributes.Append(newValue1);
            //element1.InnerText = "pushModeChange.Text";
            //root.AppendChild(element1);

            //XmlElement element2 = xmlDoc.CreateElement("VelocityOfPrinting");
            //XmlAttribute newValue2 = xmlDoc.CreateAttribute("Name");
            //newValue2.Value = "打印速度设置";
            //element2.Attributes.Append(newValue2);
            //root.AppendChild(element2);

            //XmlElement element3 = xmlDoc.CreateElement("Stablized");
            //XmlAttribute newValue3 = xmlDoc.CreateAttribute("Name");
            //newValue3.Value = "固化方式设置";
            //element3.Attributes.Append(newValue3);
            //if (checkBox1.Checked)
            //{
            //    element3.InnerText = "双材料混料";
            //}
            //else if (checkBox2.Checked)
            //{
            //    element3.InnerText = "UV光固化";
            //}
            //else {
            //    element3.InnerText = "None";
            //}
            //root.AppendChild(element3);

            //XmlElement element4 = xmlDoc.CreateElement("Adjustingheigth");
            //XmlAttribute newValue4 = xmlDoc.CreateAttribute("Name");
            //newValue4.Value = "红外测距";
            //element4.Attributes.Append(newValue4);
            //if (checkBox4.Checked) 
            //{
            //    element4.InnerText = "红外测距";
            //} else {
            //    element4.InnerText = "None";
            //}
            //root.AppendChild(element4);


            //XmlElement element5 = xmlDoc.CreateElement("Port");
            //XmlAttribute newValue5 = xmlDoc.CreateAttribute("Name");
            //newValue5.Value = "通讯接口";
            //element5.Attributes.Append(newValue5);
            //element5.InnerText=setPort.Text;
            //root.AppendChild(element5);

            //if (checkBox8.Checked)
            //{
            //    XmlElement element6 = xmlDoc.CreateElement("settingPressure1");
            //    XmlAttribute newValue6 = xmlDoc.CreateAttribute("Name");
            //    newValue6.Value = "气压设置1";
            //    element6.Attributes.Append(newValue6) ;
            //    element6.InnerText = Convert.ToString(nmbSetPressure1.Value);
            //    root.AppendChild(element6);
            //}

            //if (checkBox7.Checked)
            //{
            //    XmlElement element7 = xmlDoc.CreateElement("settingPressure2");
            //    XmlAttribute newValue7 = xmlDoc.CreateAttribute("Name");
            //    newValue7.Value = "气压设置2";
            //    element7.Attributes.Append(newValue7);
            //    element7.InnerText = Convert.ToString(nmbSetPressure2.Value);
            //    root.AppendChild(element7);
            //}
            
            //xmlDoc.Save(GV.TechParaFileName);

            //this.Hide();
            //GV.frmPrintStep4.Show();
        }

        private void btnPreStep2_Click(object sender, EventArgs e)
        {
            this.Hide();
            GV.frmPrintStep2.Show();
        }


        private void btnOutPutThings_Click(object sender, EventArgs e)
        {
            //int port = Convert.ToInt32(setPort.Text);
            //GV.PrintingObj.qExtrude(port,1);
            //if (!GV.PrintingObj.Status.isExtruding)
            //{
            //    picExtruding.Visible = true;
            //    picNonExtruding.Visible = false;
            //}
        }

        private void btnStopOutPutThings_Click(object sender, EventArgs e)
        {
            //int port = Convert.ToInt32(setPort.Text);
            //GV.PrintingObj.qExtrude(port,0);
            //if (GV.PrintingObj.Status.isExtruding)
            //{
            //    picExtruding.Visible = false;
            //    picNonExtruding.Visible = true;
            //}
        }

        private void btnPumpLink_Click(object sender, EventArgs e)
        {
            if (cmbConnMode.SelectedIndex == 1)
            {
                ConnectPumpBy485();
            }
        }

        private void ConnectPumpBy485()
        {
            if (btnPumpLink.Text == "连接")
            {
                try
                {
                    GV.seriPort4.PortName = txtConnAddr.Text;
                    GV.seriPort4.BaudRate = 1200;            //设置当前波特率
                    GV.seriPort4.Parity = Parity.Even;       //设置当前校验位
                    GV.seriPort4.DataBits = 8;               //设置当前数据位
                    GV.seriPort4.StopBits = StopBits.One;    //设置当前停止位                    

                    GV.seriPort4.Open();                     //打开串口
                    btnPumpLink.Text = "断开";
                    picPumpOn.Show();
                }
                catch (Exception ex)
                {
                    picPumpOn.Hide();
                    MessageBox.Show(ex.Message);
                    return;
                }
            }
            else
            {
                GV.seriPort4.Close();
                picPumpOn.Hide();
                btnPumpLink.Text = "连接";
            }
        }

        private void btnPumpBreakLink_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Pump Off");
            //int iret = -1;
            //iret = SMC6X.SMCClose(m_handle);
            //if (0 != iret)
            //{
            //    MessageBox.Show("注射泵关闭失败");
            //    return;
            //}else{
            //    picPumpOff.Visible = true;
            //    picPumpOn.Visible = false;
            //}
        }

        private void btnPumpPulse_Click(object sender, EventArgs e)
        {
            //int p = Convert.ToInt32(txtPumpPulse.Text);
            //SMC6X.SMCPMovePluses(m_handle, axisPump, p, 0);
        }

        private void btnPumpStep_Click(object sender, EventArgs e)
        {
            //double y = Convert.ToDouble(txtPumpStep.Text);
            //SMC6X.MoveStep(axisPump, y);
        }

        private void btnPumpVelo_Click(object sender, EventArgs e)
        {
            //double velo = Convert.ToDouble(txtPumpVelo.Text);
            //SMC6X.SetSpeed(m_handle, axisPump, velo); //设置运动速度，mm/s;
            //SMC6X.SetAcceleration(m_handle, axisPump, velo * 10); //设置运动加速度，mm/s;
            //SMC6X.SetDeceleration(m_handle, axisPump, velo * 10); //设置运动减速度，mm/s;  
        }

        private void btnPumpJogPos_MouseDown(object sender, MouseEventArgs e)
        {
            //SMC6X.SMCVMove(m_handle, axisPump, 1);
        }

        private void btnPumpJogNeg_MouseDown(object sender, MouseEventArgs e)
        {
            //SMC6X.SMCVMove(m_handle, axisPump, 0);
        }

        private void btnPumpJog_MouseUp(object sender, MouseEventArgs e)
        {
            //SMC6X.SMCDecelStop(m_handle, axisPump);
        }

        private void btnFlowrate_Click(object sender, EventArgs e)
        {
            //double f = Convert.ToDouble(txtFlowrate.Text); // 体积流量（uL/s = (mm)^3/s )
            //double d = Convert.ToDouble(txtSyringeDiameter.Text); // 针筒内径（mm）
            //// velo = 4/pi *f /(d^2)
            //double velo = 1.2732395 * f / Math.Pow(d, 2);   // 计算得到推进速度（mm/s)

            //SMC6X.SetSpeed(m_handle, axisPump, velo); //设置运动速度，mm/s;
            //SMC6X.SetAcceleration(m_handle, axisPump, velo * 10); //设置运动加速度，mm/s;
            //SMC6X.SetDeceleration(m_handle, axisPump, velo * 10); //设置运动减速度，mm/s;  
        }

        private void btnSetSyringDia_Click(object sender, EventArgs e)
        {
            //// f = v * pi * d ^ 2 / 4 = 0.78539816 * v * (d * d)
            //double vSet = 0;
            //SMC6X.GetSpeedSet(axisPump, ref vSet);
            //double d = Convert.ToDouble(txtSyringeDiameter.Text); // 针筒内径（mm）
            //double f = 0.78539816 * vSet * (d * d);
            //txtFlowrate.Text = f.ToString();
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "amtp files (*.amtp)|*.amtp";
            saveFileDialog1.DefaultExt = "amtp";
            saveFileDialog1.FileName = "自定义工艺文件" + DateTime.Now.ToString("y.M.d hhmmss");
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                WriteAmtpFile(saveFileDialog1.FileName);                    
            }
        }

        private void btnBrowseAmtpFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "AMTP files (*.amtp)|*.amtp";
            openFileDialog1.DefaultExt = "amtp";
            openFileDialog1.FileName = "";
            DialogResult dlgRslt;
            dlgRslt = openFileDialog1.ShowDialog();
            if (dlgRslt == DialogResult.OK)
            {
                if (ValidityCheck())
                {
                    txtTechParaFileName.Text = openFileDialog1.FileName;
                    ReadAmtpFile(openFileDialog1.FileName);
                }
            }
        }

        public bool ValidityCheck()
        {
            try
            {
                if (File.Exists(txtTechParaFileName.Text))
                {
                    lblFileError.Hide();
                }
            }
            catch (Exception)
            {
                GV.MsgShow(lblFileError, "工艺文件加载失败，请检查确认！", timer1, 5000);
                return false;
            }
            return true;
        }


        /// <summary>
        /// 根据复选框的选择结果，更新各参数设置选项卡是否启用
        /// </summary>
        private void UpdateTableControls()
        {
            tbctrlGroups.Visible = false;
            tbctrlGroups.TabPages.Clear();

            for (int i = 0; i < arrExtruderChk.Length ; i++)
            {
                if (arrExtruderChk[i] != null && arrExtruderChk[i].Checked)
                {
                    tbctrlGroups.TabPages.Add(arrTabPage[i]);
                }
            }

            for (int i = 0; i < arrCureChk.Length; i++)
            {
                if (arrCureChk[i] != null && arrCureChk[i].Checked)
                {
                    tbctrlGroups.TabPages.Add(arrTabPage[i + arrExtruderChk.Length]);
                }
            }

            tbctrlGroups.Visible = true;
            UpdateOutports();//更新出口号
        }

        private void chkExtruder1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTableControls();
            if (chkExtruder1.Checked)
            {
                tbctrlGroups.SelectTab(tabPage1);
                tabPage1.Focus();
            }
        }
        //螺杆勾选
        private void chkExtruder2_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTableControls();
            if (chkExtruder2.Checked)
            {
                tbctrlGroups.SelectTab(tabPage2);
                tabPage2.Focus();
            }
        }

        private void chkExtruder3_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTableControls();
            if (chkExtruder3.Checked)
            {
                tbctrlGroups.SelectTab(tabPage3);
                tabPage3.Focus();
            }
        }

        private void chkExtruder4_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTableControls();
            if (chkExtruder4.Checked)
            {
                tbctrlGroups.SelectTab(tabPage4);
                tabPage4.Focus();
            }
        }

        private void chkExtruder5_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTableControls();
            if (chkExtruder5.Checked)
            {
                tbctrlGroups.SelectTab(tabPage5);
                tabPage5.Focus();
            }
        }


        private void chkCure1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTableControls();
            if (chkCure1.Checked)
            {
                tbctrlGroups.SelectTab(tabPage6);
                tabPage6.Focus();
            }
        }

        private void chkCure2_CheckedChanged(object sender, EventArgs e)
        {
            UpdateTableControls();
            if (chkCure2.Checked)
            {
                tbctrlGroups.SelectTab(tabPage7);
                tabPage7.Focus();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            WriteAmtpFile(txtTechParaFileName.Text);
        }
        
        string[] arrGroupName;//存储设备名的数组
        CheckBox[] arrExtruderChk;//出口号checkbox
        CheckBox[] arrCureChk;
        TabPage[] arrTabPage;
        Control[][] matControl;
        //string[][] matParaName;

        /// <summary>
        /// 更新当前选中的端口号
        /// </summary>
        public void UpdateOutports()
        {
            try
            {
                List<int> listOutPorts = new List<int>();
                for (int i = 0; i < arrExtruderChk.Length; i++)
                {
                    if (arrExtruderChk[i] != null && arrExtruderChk[i].Checked && matControl[i] != null && matControl[i].Length >= 3)
                    {
                        if (matControl[i][2].GetType().Name == "NumericUpDown")
                        {
                            listOutPorts.Add((int)(matControl[i][2] as NumericUpDown).Value);
                        }
                    }
                }
                GV.PrintingObj.extruderOutports = listOutPorts.ToArray();//添加端口号

                listOutPorts.Clear();
                int j = 0;
                for (int i = 0; i < arrCureChk.Length; i++)
                {
                    j = i + arrExtruderChk.Length;
                    if (arrCureChk[i] != null && arrCureChk[i].Checked && matControl[j] != null && matControl[j].Length >= 3)
                    {
                        if (matControl[j][2].GetType().Name == "NumericUpDown")
                        {
                            listOutPorts.Add((int)(matControl[j][2] as NumericUpDown).Value);
                        }
                    }
                }
                GV.PrintingObj.cureOutports = listOutPorts.ToArray();

                if (chkExtruder1.Checked)
                {
                    GV.timeExtrudeInAdvance0 = Convert.ToInt32(txtTimeExtrudeInAdvance1.Text);//赋值提前出丝，打印开始前停顿设定时间

                    GV.timeCloseExtrudeInAdvance = Convert.ToInt32(txtTimeExtrudeInAdvance2.Text);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 初始化装置
        /// </summary>
        private void InitParaControls()
        {
            // 分组名称
            arrGroupName = new string[] { "DISPENSER", "SCREW_VALVE", "INJECTION_PUMP", "ELECTRO_HYDRO_DYNAMICS", "AUXILIARY_DEVICES", "THERMO_CURING", "UV_CURING", "PUBLIC" };
            // 挤出方式复选框
            arrExtruderChk = new CheckBox[] { chkExtruder1, chkExtruder2, chkExtruder3, chkExtruder4, chkExtruder5 };
            // 固化方式复选框
            arrCureChk = new CheckBox[] { chkCure1, chkCure2 };
            // 分组选项卡
            arrTabPage = new TabPage[] { tabPage1, tabPage2, tabPage3, tabPage4, tabPage5, tabPage6, tabPage7, null };
            // 控件矩阵（每行对应一个分组，每列对应不同的参数）
            matControl = new Control[arrGroupName.Length][];

            //matParaName = new string[arrExtruder.Length][];
            // 气压模块
            matControl[0] = new Control[] { arrExtruderChk[0], arrTabPage[0], nmudOutport1, nmbSetPressure1, txtTimeExtrudeInAdvance1, txtTimeExtrudeInAdvance2, cmbHP };
            //matParaName[0] = new string[] { arrExtruder[0], "气压模块", "输出端口", "设置气压", "启动提前时间", "关停提前时间", "增压器" };

            // 螺杆模块
            matControl[1] = new Control[] { arrExtruderChk[1], arrTabPage[1], nmudOutport2, txtSetVoltage1, txtSetRotateSpeed };
            //matParaName[1] = new string[] { arrExtruder[1], "螺杆模块", "输出端口", "设置电源电压", "设置转速" };

            // 注射泵
            matControl[2] = new Control[] { arrExtruderChk[2], arrTabPage[2], cmbConnMode, txtConnAddr, cmbSyringeType, txtFlowrate, txtPumpVelo, txtPumpStep };
            //matParaName[2] = new string[] { arrExtruder[2], "注射泵", "连接方式", "连接地址", "针筒内径", "设置流量", "设定速度", "脉冲数", "推进速度" };

            // 双料混合
            matControl[3] = new Control[] { arrExtruderChk[3], arrTabPage[3], nmudOutport3, nmudSetVoltage2, tkbrVoltageStep, cmbSerialPort, numericUpDown2 };
            //matParaName[3] = new string[] { arrExtruder[3], "双料混合", "输出端口", "nmudSetVoltage2", "tkbrVoltageStep" };

            // 高低温喷头
            matControl[4] = new Control[] { arrExtruderChk[4], arrTabPage[4] };
            //matParaName[4] = new string[] { arrExtruder[4], "高低温喷头" };

            // 喷头校准/清洁
            matControl[5] = new Control[] { arrCureChk[0], arrTabPage[5], nmudOutport4, nmudSetHeatPower, nmudSetHeatTemp, nmudSetHeatTime };
            //matParaName[5] = new string[] { arrExtruder[5], "喷头校准/清洁", "输出端口", "X", "Y", "Z" };

            // 光固化
            matControl[6] = new Control[] { arrCureChk[1], arrTabPage[6], nmudOutport5, nmudUvModel, nmudUvWaveLength };
            //matParaName[6] = new string[] { arrExtruder[6], "UV光固化", "输出端口", "UV光源型号", "UV光波长" };

            // 公共参数
            matControl[7] = new Control[] { null, arrTabPage[7], cmbPrintMaterial, nmbSetPrintingSpeed, nmbSetJumpSpeed };
        }

        /// <summary>
        /// 写入Amtp文件
        /// </summary>
        /// <param name="fileName">完整文件名（含路径）</param>
        /// <returns></returns>
        private bool WriteAmtpFile(string fileName)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "yes");
                XmlElement root = xmlDoc.CreateElement("AMTP_File");
                xmlDoc.AppendChild(root);
                XmlElement element;
                XmlAttribute newNode;

                for (int i = 0; i < arrGroupName.Length; i++)
                {
                    element = xmlDoc.CreateElement(arrGroupName[i]);
                    element.InnerText = "";

                    for (int j = 0; j < matControl[i].Length; j++)
                    {
                        Control ctrl = matControl[i][j];
                        if (ctrl == null)
                        {
                            continue;
                        }
                        switch (ctrl.GetType().Name)
                        {
                            case "CheckBox":
                                // 添加装置的使能状态
                                newNode = xmlDoc.CreateAttribute("Enabled");
                                newNode.Value = (matControl[i][j] as CheckBox).Checked ? "1" : "0";
                                element.Attributes.Append(newNode);
                                newNode = xmlDoc.CreateAttribute(matControl[i][j].Name);
                                newNode.Value = matControl[i][j].Text;
                                break;
                            case "TrackBar":
                                newNode = xmlDoc.CreateAttribute(matControl[i][j].Name);
                                newNode.Value = (matControl[i][j] as TrackBar).Value.ToString();
                                break;
                            case "NumericUpDown":
                                newNode = xmlDoc.CreateAttribute(matControl[i][j].Name);
                                newNode.Value = (matControl[i][j] as NumericUpDown).Value.ToString();
                                break;
                            default:
                                //case "ComboBox":
                                //case "TextBox":
                                //case "TabPage":
                                newNode = xmlDoc.CreateAttribute(matControl[i][j].Name);
                                newNode.Value = matControl[i][j].Text;
                                break;
                        }
                        element.Attributes.Append(newNode);
                        //element.InnerText += (matControl[i][j].Name + ": " + matParaName[i][j] + ", ");
                    }
                    root.AppendChild(element);
                }
                xmlDoc.Save(fileName);
                lblNotice.ForeColor = Color.FromArgb(0, 192, 0);
                GV.MsgShow(lblNotice, "参数保存成功。", timer1, 5000);
                UpdateOutports();
            }
            catch (Exception ex)
            {
                lblNotice.ForeColor = Color.Red;
                GV.MsgShow(lblNotice, "参数保存失败。" + ex.Message, timer1, 5000);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 读取Amtp文件
        /// </summary>
        /// <param name="fileName">完整文件名（含路径）</param>
        /// <returns></returns>
        private bool ReadAmtpFile(string fileName)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();//加载XML文件
                xmlDoc.Load(fileName);
                XmlElement root = xmlDoc.DocumentElement;
                XmlElement element;
                XmlAttribute attribute;
                int iDevice;
                Control[] arrControl;
                Control chkDevice;
                for (int i = 0; i < root.ChildNodes .Count; i++)//遍历节点
                {
                    element = root.ChildNodes[i] as XmlElement;
                    iDevice = -1;
                    for (int j = 0; j < arrGroupName.Length; j++)//匹配模块/设备
                    {
                        if (element.Name == arrGroupName[j])
                        {
                            // 装置英文名称匹配成功
                            iDevice = j;//设备索引
                            break;
                        }
                    }
                    // 装置英文名称匹配失败
                    if (iDevice == -1)
                    {
                        continue;
                    }
                    // 读取装置对应的Element成功
                    arrControl = matControl[iDevice];   // 装置参数的控件数组
                    // chkDevice = arrExtruderChk[iDevice];  // 装置的使能复选框
                    chkDevice = matControl[iDevice][0];
                    // 读取Element的第一个属性，判定装置是否使能
                    attribute = element.Attributes[0];
                    //int j0 = 0;
                    if (attribute.Name == "Enabled")//模块使能
                    {
                        //chkDevice.Checked = (attribute.Value == "1");
                        if (chkDevice.GetType().Name == "CheckBox")
                        {
                            (chkDevice as CheckBox).Checked = (attribute.Value == "1");
                        }   
                    }
                    // 读取装置参数的控件数组，并依次对控件赋值
                    Control ctrl;
                    for (int j = 0; j < element.Attributes.Count; j++)
                    {
                        attribute = element.Attributes[j];
                        ctrl = null;
                        // 搜索与attribute名称相同的控件
                        for (int k = 0; k < arrControl.Length; k++)
                        {
                            ctrl = arrControl[k];
                            if (ctrl != null && ctrl.Name == attribute.Name) // 搜索成功
                            {
                                ctrl = arrControl[k];
                                break;
                            }
                        }
                        // 对控件赋值
                        if (ctrl != null)
                        {
                            switch (ctrl.GetType().Name)//根据控件类型赋值
                            {
                                case "TrackBar":
                                    (ctrl as TrackBar).Value = Convert.ToInt32(attribute.Value);
                                    break;
                                case "NumericUpDown":
                                    //(ctrl as NumericUpDown).Value = Convert.ToDecimal(attribute.Value);
                                    ctrl.Text = attribute.Value;
                                    break;
                                default:
                                    ctrl.Text = attribute.Value;
                                    break;
                            }
                        }
                    }
                }
                GV.MsgShow(lblFileError, "参数加载成功。", timer1, 5000, Color.FromArgb(0, 192, 0));
                UpdateOutports();//读取工艺文件后更新串口号
            }
            catch (Exception ex)
            {
                GV.MsgShow(lblFileError, "参数加载失败。" + ex.Message, timer1, 5000, Color.Red);
                return false;
            }
            return true;
        }

        private void btnReadAmtpFile_Click(object sender, EventArgs e)
        {
            ReadAmtpFile(txtTechParaFileName.Text);
        }

        private void tkbrVoltageStep_ValueChanged(object sender, EventArgs e)
        {
            switch (tkbrVoltageStep.Value)
            {
                case 0:
                    nmudSetVoltage2.Increment = 0.1M;
                    break;
                case 1:
                    nmudSetVoltage2.Increment = 0.5M;
                    break;
                case 2:
                    nmudSetVoltage2.Increment = 1;
                    break;
                case 3:
                    nmudSetVoltage2.Increment = 2;
                    break;
                default:
                    break;
            }
            lblVoltageStep.Text = nmudSetVoltage2.Increment.ToString() + " kV";
        }

        private void btnExtrudersOn_Click(object sender, EventArgs e)
        {
            UpdateOutports();
            GV.PrintingObj.SetExtrudePorts(-2, 1);
        }

        private void btnExtrudersOff_Click(object sender, EventArgs e)
        {
            UpdateOutports();
            GV.PrintingObj.SetExtrudePorts(-2, 0);
        }

        private void btnCuresOn_Click(object sender, EventArgs e)
        {
            UpdateOutports();
            GV.PrintingObj.SetCurePorts(-2, 1);
        }

        private void btnCuresOff_Click(object sender, EventArgs e)
        {
            UpdateOutports();
            GV.PrintingObj.SetCurePorts(-2, 0);
        }

        private void nmbSetPrintingSpeed_ValueChanged(object sender, EventArgs e)
        {
            GV.printingSpeedSet = (double)nmbSetPrintingSpeed.Value;
            GV.frmPrintStep2.UpdatePrintingSpeed();
        }

        public void UpdatePrintingSpeed()
        {
            decimal newSpeed = (decimal)GV.printingSpeedSet;
            if (nmbSetPrintingSpeed.Value != newSpeed)
            {
                nmbSetPrintingSpeed.Value = newSpeed;
            }
        }

        private void nmbSetJumpSpeed_ValueChanged(object sender, EventArgs e)
        {
            GV.jumpSpeedSet = (double)nmbSetJumpSpeed.Value;
        }

        private void label7_DoubleClick(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(txtTechParaFileName.Text);
        }

        private void btnHVPOn_Click(object sender, EventArgs e)
        {
            HVPOn();
        }

        private void btnHVPOff_Click(object sender, EventArgs e)
        {
            HVPOff();
        }


        public void ControlOn()
        {
            byte[] command;
            byte[] commandCrc;
            //上位机控制
            //Tx:01 05 00 0A FF 00 AC 38
            //Rx:01 05 00 0A FF 00 AC 38
            command = new byte[] { 0x01, 0x05, 0x00, 0x0A, 0xFF, 0x00 }; //, 0xAC, 0x38 };

            commandCrc = GV.GetCRC16ByPoly(command, 0xA001, true);
            GV.seriPort3.Write(commandCrc, 0, commandCrc.Length);
            System.Threading.Thread.Sleep(100);

        }

        public void ControlOff()
        {
            byte[] command;
            byte[] commandCrc;

            //上位机控制关
            //Tx:01 05 00 0A 00 00 ED C8
            //Rx:01 05 00 0A 00 00 ED C8
            command = new byte[] { 0x01, 0x05, 0x00, 0x0A, 0x00, 0x00 }; //, 0xED, 0xC8 };

            commandCrc = GV.GetCRC16ByPoly(command, 0xA001, true);
            GV.seriPort3.Write(commandCrc, 0, commandCrc.Length); // 设置电子负载的电流值为Current的值（A）
            System.Threading.Thread.Sleep(100);
        }

        /// <summary>
        /// 打开高压电源
        /// </summary>
        public void HVPOn()
        {
            try
            {
                byte[] command;
                byte[] commandCrc;

                //高压开
                //Tx:01 05 00 00 FF 00 8C 3A
                command = new byte[] { 0x01, 0x05, 0x00, 0x00, 0xFF, 0x00 }; //, 0x8C, 0x3A };
                //Rx:01 05 00 00 FF 00 8C 3A

                commandCrc = GV.GetCRC16ByPoly(command, 0xA001, true);
                GV.seriPort3.Write(commandCrc, 0, commandCrc.Length);
                System.Threading.Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 关闭高压电源
        /// </summary>
        public void HVPOff()
        {
            try
            {
                byte[] command;
                byte[] commandCrc;

                //高压关
                //Tx:01 05 00 00 00 00 CD CA
                command = new byte[] { 0x01, 0x05, 0x00, 0x00, 0x00, 0x00 }; //, 0xCD, 0xCA };
                //Rx:01 05 00 00 00 00 CD CA

                commandCrc = GV.GetCRC16ByPoly(command, 0xA001, true);
                GV.seriPort3.Write(commandCrc, 0, commandCrc.Length);
                System.Threading.Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 设置高压电源电压电压
        /// </summary>
        /// <param name="voltage"></param>
        /// <param name="current"></param>
        public void SetHVPVoltageCurrent(double voltage, double current)
        {
            byte[] command;
            byte[] commandCrc;

            int vBytes = 0, iBytes = 0;
            byte[] bytesVI = new byte[4];
            try
            {
                vBytes = (int)(voltage * 4095 / 29);
                iBytes = (int)(current * 4095 / 1);

                bytesVI[0] = (byte)(vBytes >> 8);
                bytesVI[1] = (byte)(vBytes % 256);
                bytesVI[2] = (byte)(vBytes >> 8);
                bytesVI[3] = (byte)(iBytes % 256);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            //设定为额定电压+额定电流
            //Tx:01 10 00 00 00 02 04 0F FF 0F FF 85 3B
            command = new byte[] { 0x01, 0x10, 0x00, 0x00, 0x00, 0x02, 0x04, bytesVI[0], bytesVI[1], bytesVI[2], bytesVI[3] };
            //Rx:01 10 00 00 00 02 41 C8

            commandCrc = GV.GetCRC16ByPoly(command, 0xA001, true);
            GV.seriPort3.Write(commandCrc, 0, commandCrc.Length);
            System.Threading.Thread.Sleep(100);
        }

        private void btnConnectHVP_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnConnectHVP.Text == "连接")
                {
                    if (!GV.seriPort3.IsOpen)
                    {
                        GV.seriPort3.PortName = cmbSerialPort.Text;
                        GV.seriPort3.Open();
                        System.Threading.Thread.Sleep(100);
                        ControlOn();
                    }
                    btnConnectHVP.Text = "断开";
                }
                else
                {
                    if (GV.seriPort3.IsOpen)
                    {
                        ControlOff();
                        System.Threading.Thread.Sleep(100);
                        GV.seriPort3.Close();
                    }
                    btnConnectHVP.Text = "连接";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSetHVPVoltageCurrent_Click(object sender, EventArgs e)
        {
            SetHVPVoltageCurrent((double)nmudSetVoltage2.Value, (double)numericUpDown2.Value);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void btnSetPump_Click(object sender, EventArgs e)
        {
            byte mode = 1;
            byte rowPump = 7;
            byte colPump = 1;
            int volume = 300;
            byte volUnit = 5;
            int timePumpOut = 500;
            byte pumpOutTimeUnit = 3;
            //int timePumpIn = timePumpOut;
            //byte pumpInTimeUnit = pumpOutTimeUnit;
            int cycle = 1;
            int timePause = 0;

            switch (cmbSyringeType.SelectedIndex)
            {
                case 0:
                    colPump = 1;
                    volume = 300;
                    break;
                case 1:
                    colPump = 2;
                    volume = 500;
                    break;
                case 2:
                    colPump = 3;
                    volume = 1000;
                    break;
                case 3:
                    colPump = 4;
                    volume = 3000;
                    break;
                default:
                    colPump = 1;
                    volume = 0;
                    return;
            }

            try
            {
                double flowrate = Convert.ToDouble(txtFlowrate.Text); // 流量（μL/min）
                double time = volume * 10 / flowrate * 10; // 时间（0.1min）
                if (time > 9999)
                {
                    pumpOutTimeUnit = 3; // 0.1hour
                    time = time / 60;
                }
                else if (time >= 100)
                {
                    pumpOutTimeUnit = 2; // 0.1min
                }
                else
                {
                    pumpOutTimeUnit = 1; // 0.1sec
                    time = time * 60;
                }
                timePumpOut = (int)(Math.Round(time));
                byte[] pdu = PDU_WP(mode, rowPump, colPump, volume, volUnit, timePumpOut, pumpOutTimeUnit, timePumpOut, pumpOutTimeUnit, cycle, timePause);
                SendBufferPump485(GetPumpCommand(pdu), GV.seriPort4);
            }
            catch (Exception ex)
            {
            }

        }


        /// <summary>
        /// 根据PDU和地址生成待发送的buffer数据（byte数组）
        /// </summary>
        /// <param name="arrPdu">指令PDU</param>
        /// <param name="addr">注射泵通讯地址</param>
        /// <returns></returns>
        private byte[] GetPumpCommand(byte[] arrPdu, byte addr = 1)
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

        /// <summary>
        /// 发送注射泵485通信指令
        /// </summary>
        /// <param name="buffer">指令</param>
        /// <param name="seriPort">COM口</param>
        private void SendBufferPump485(byte[] buffer, SerialPort seriPort)
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

        private void btnPumpOn_Click(object sender, EventArgs e)
        {
            SendBufferPump485(GetPumpCommand(PDU_WS(1, 0, 0)), GV.seriPort4);
        }

        private void btnPumpOff_Click(object sender, EventArgs e)
        {
            SendBufferPump485(GetPumpCommand(PDU_WS(0, 0, 0)), GV.seriPort4);
        }

        private void btnSetCurrent_Click(object sender, EventArgs e)
        {

        }

        private void btn_Click(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 设置螺杆阀的转速（RPM）
        /// </summary>
        /// <param name="speed"></param>
        public void SetRotateSpeed(double speed)
        {
            txtSetRotateSpeed.Text = speed.ToString();
        }

        private void btnOpenRotateSpeed_Click(object sender, EventArgs e)
        {
            try
            {
                GV.frmRotaryValveCtrl.Show();
                GV.frmRotaryValveCtrl.Activate();
            }
            catch(Exception ex)
            { }                    
        }
    }
}
