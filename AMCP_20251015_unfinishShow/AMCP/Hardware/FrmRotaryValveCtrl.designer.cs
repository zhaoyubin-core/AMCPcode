namespace AMCP
{
    partial class FrmRotaryValveCtrl
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmRotaryValveCtrl));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.txtCommand = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnSetTimeRotation = new System.Windows.Forms.Button();
            this.cmbComPort1 = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.txtReceived = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.txtCalcuCRC16 = new System.Windows.Forms.Button();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnReadPV = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnWriteSV = new System.Windows.Forms.Button();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.nmudSV = new System.Windows.Forms.NumericUpDown();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.ConnOn = new System.Windows.Forms.PictureBox();
            this.ConnOff = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.numTimeDelay = new System.Windows.Forms.NumericUpDown();
            this.txtPV_real = new System.Windows.Forms.TextBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.lblCmdStartRotaryValve = new System.Windows.Forms.Label();
            this.txtSV_real = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.btnReadData = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.btnSetZero = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.serialPort2 = new System.IO.Ports.SerialPort(this.components);
            this.btnRotate = new System.Windows.Forms.Button();
            this.dGVTimeRotation = new System.Windows.Forms.DataGridView();
            this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Rotation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblTimeRotation = new System.Windows.Forms.Label();
            this.timerLoadRotation = new System.Windows.Forms.Timer(this.components);
            this.btnOpenCOM1 = new System.Windows.Forms.Button();
            this.btnDeleteRow = new System.Windows.Forms.Button();
            this.btnSaveSequence = new System.Windows.Forms.Button();
            this.btnLoadSequence = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.label11 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.label18 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nmudSV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ConnOn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ConnOff)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dGVTimeRotation)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // txtCommand
            // 
            this.txtCommand.Location = new System.Drawing.Point(524, 552);
            this.txtCommand.Multiline = true;
            this.txtCommand.Name = "txtCommand";
            this.txtCommand.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCommand.Size = new System.Drawing.Size(238, 56);
            this.txtCommand.TabIndex = 74;
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(400, 581);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(79, 56);
            this.button1.TabIndex = 75;
            this.button1.Text = "发送指令";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnSetTimeRotation
            // 
            this.btnSetTimeRotation.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnSetTimeRotation.Font = new System.Drawing.Font("宋体", 12F);
            this.btnSetTimeRotation.ForeColor = System.Drawing.Color.Black;
            this.btnSetTimeRotation.Location = new System.Drawing.Point(292, 316);
            this.btnSetTimeRotation.Name = "btnSetTimeRotation";
            this.btnSetTimeRotation.Size = new System.Drawing.Size(90, 30);
            this.btnSetTimeRotation.TabIndex = 76;
            this.btnSetTimeRotation.Text = "添加";
            this.btnSetTimeRotation.UseVisualStyleBackColor = false;
            this.btnSetTimeRotation.Click += new System.EventHandler(this.btnSetTimeRotation_Click);
            // 
            // cmbComPort1
            // 
            this.cmbComPort1.Font = new System.Drawing.Font("宋体", 12F);
            this.cmbComPort1.FormattingEnabled = true;
            this.cmbComPort1.Items.AddRange(new object[] {
            "COM6"});
            this.cmbComPort1.Location = new System.Drawing.Point(126, 39);
            this.cmbComPort1.Name = "cmbComPort1";
            this.cmbComPort1.Size = new System.Drawing.Size(98, 24);
            this.cmbComPort1.TabIndex = 81;
            this.cmbComPort1.Text = "COM7";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(23, 525);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 36);
            this.label4.TabIndex = 83;
            this.label4.Text = "发送指令：\r\n\r\n(HEX)";
            // 
            // txtReceived
            // 
            this.txtReceived.Location = new System.Drawing.Point(855, 43);
            this.txtReceived.Multiline = true;
            this.txtReceived.Name = "txtReceived";
            this.txtReceived.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtReceived.Size = new System.Drawing.Size(342, 333);
            this.txtReceived.TabIndex = 74;
            this.txtReceived.TextChanged += new System.EventHandler(this.txtReceived_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 656);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 36);
            this.label1.TabIndex = 83;
            this.label1.Text = "解析指令：\r\n\r\n(ASC)";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(142, 706);
            this.textBox4.Multiline = true;
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(255, 107);
            this.textBox4.TabIndex = 118;
            // 
            // txtCalcuCRC16
            // 
            this.txtCalcuCRC16.Location = new System.Drawing.Point(116, 631);
            this.txtCalcuCRC16.Name = "txtCalcuCRC16";
            this.txtCalcuCRC16.Size = new System.Drawing.Size(75, 32);
            this.txtCalcuCRC16.TabIndex = 119;
            this.txtCalcuCRC16.Text = "计算校验码";
            this.txtCalcuCRC16.UseVisualStyleBackColor = true;
            this.txtCalcuCRC16.Click += new System.EventHandler(this.button4_Click);
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(254, 566);
            this.textBox5.Multiline = true;
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(100, 107);
            this.textBox5.TabIndex = 118;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(400, 669);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 40);
            this.btnClear.TabIndex = 120;
            this.btnClear.Text = "清除";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnReadPV
            // 
            this.btnReadPV.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.btnReadPV.Font = new System.Drawing.Font("宋体", 12F);
            this.btnReadPV.Location = new System.Drawing.Point(868, 523);
            this.btnReadPV.Name = "btnReadPV";
            this.btnReadPV.Size = new System.Drawing.Size(98, 32);
            this.btnReadPV.TabIndex = 121;
            this.btnReadPV.Text = "立即读取";
            this.btnReadPV.UseVisualStyleBackColor = false;
            this.btnReadPV.Click += new System.EventHandler(this.btnReadPV_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F);
            this.label2.Location = new System.Drawing.Point(512, 599);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 16);
            this.label2.TabIndex = 122;
            this.label2.Text = "当前温度：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 12F);
            this.label3.Location = new System.Drawing.Point(815, 529);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 16);
            this.label3.TabIndex = 122;
            this.label3.Text = "℃";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 12F);
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(7, 30);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(87, 16);
            this.label5.TabIndex = 122;
            this.label5.Text = "目标转速：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 12F);
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(202, 30);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 16);
            this.label6.TabIndex = 122;
            this.label6.Text = "RPM";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnWriteSV
            // 
            this.btnWriteSV.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnWriteSV.Font = new System.Drawing.Font("宋体", 12F);
            this.btnWriteSV.ForeColor = System.Drawing.Color.Black;
            this.btnWriteSV.Location = new System.Drawing.Point(249, 23);
            this.btnWriteSV.Name = "btnWriteSV";
            this.btnWriteSV.Size = new System.Drawing.Size(98, 33);
            this.btnWriteSV.TabIndex = 121;
            this.btnWriteSV.Text = "启动";
            this.btnWriteSV.UseVisualStyleBackColor = false;
            this.btnWriteSV.Click += new System.EventHandler(this.btnWriteSV_Click);
            // 
            // timer2
            // 
            this.timer2.Interval = 1000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // nmudSV
            // 
            this.nmudSV.DecimalPlaces = 1;
            this.nmudSV.Font = new System.Drawing.Font("Arial", 16F);
            this.nmudSV.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.nmudSV.Location = new System.Drawing.Point(99, 23);
            this.nmudSV.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.nmudSV.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            -2147483648});
            this.nmudSV.Name = "nmudSV";
            this.nmudSV.Size = new System.Drawing.Size(97, 32);
            this.nmudSV.TabIndex = 123;
            this.nmudSV.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nmudSV.ValueChanged += new System.EventHandler(this.nmudSV_ValueChanged);
            // 
            // btnOpen
            // 
            this.btnOpen.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.btnOpen.Font = new System.Drawing.Font("宋体", 12F);
            this.btnOpen.Location = new System.Drawing.Point(502, 660);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(98, 32);
            this.btnOpen.TabIndex = 121;
            this.btnOpen.Text = "开机";
            this.btnOpen.UseVisualStyleBackColor = false;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.btnClose.Font = new System.Drawing.Font("宋体", 12F);
            this.btnClose.Location = new System.Drawing.Point(768, 590);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(98, 32);
            this.btnClose.TabIndex = 121;
            this.btnClose.Text = "关机";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // ConnOn
            // 
            this.ConnOn.Image = ((System.Drawing.Image)(resources.GetObject("ConnOn.Image")));
            this.ConnOn.Location = new System.Drawing.Point(725, 516);
            this.ConnOn.Name = "ConnOn";
            this.ConnOn.Size = new System.Drawing.Size(29, 30);
            this.ConnOn.TabIndex = 290;
            this.ConnOn.TabStop = false;
            this.ConnOn.Visible = false;
            // 
            // ConnOff
            // 
            this.ConnOff.Image = ((System.Drawing.Image)(resources.GetObject("ConnOff.Image")));
            this.ConnOff.Location = new System.Drawing.Point(725, 516);
            this.ConnOff.Name = "ConnOff";
            this.ConnOff.Size = new System.Drawing.Size(29, 30);
            this.ConnOff.TabIndex = 289;
            this.ConnOff.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lblTimeRotation);
            this.panel1.Controls.Add(this.numTimeDelay);
            this.panel1.Controls.Add(this.txtPV_real);
            this.panel1.Controls.Add(this.checkBox1);
            this.panel1.Controls.Add(this.lblCmdStartRotaryValve);
            this.panel1.Controls.Add(this.txtSV_real);
            this.panel1.Controls.Add(this.nmudSV);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.btnReadData);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.btnSetZero);
            this.panel1.Controls.Add(this.btnWriteSV);
            this.panel1.Enabled = false;
            this.panel1.Location = new System.Drawing.Point(25, 97);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(357, 193);
            this.panel1.TabIndex = 291;
            // 
            // numTimeDelay
            // 
            this.numTimeDelay.Location = new System.Drawing.Point(10, 76);
            this.numTimeDelay.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numTimeDelay.Name = "numTimeDelay";
            this.numTimeDelay.Size = new System.Drawing.Size(73, 21);
            this.numTimeDelay.TabIndex = 293;
            // 
            // txtPV_real
            // 
            this.txtPV_real.Font = new System.Drawing.Font("Arial", 16F);
            this.txtPV_real.Location = new System.Drawing.Point(99, 105);
            this.txtPV_real.Name = "txtPV_real";
            this.txtPV_real.ReadOnly = true;
            this.txtPV_real.Size = new System.Drawing.Size(97, 32);
            this.txtPV_real.TabIndex = 291;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Font = new System.Drawing.Font("宋体", 12F);
            this.checkBox1.ForeColor = System.Drawing.Color.Black;
            this.checkBox1.Location = new System.Drawing.Point(256, 156);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(90, 20);
            this.checkBox1.TabIndex = 293;
            this.checkBox1.Text = "连续读取";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.Visible = false;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // lblCmdStartRotaryValve
            // 
            this.lblCmdStartRotaryValve.Font = new System.Drawing.Font("宋体", 12F);
            this.lblCmdStartRotaryValve.ForeColor = System.Drawing.Color.Black;
            this.lblCmdStartRotaryValve.Location = new System.Drawing.Point(17, 138);
            this.lblCmdStartRotaryValve.Name = "lblCmdStartRotaryValve";
            this.lblCmdStartRotaryValve.Size = new System.Drawing.Size(227, 45);
            this.lblCmdStartRotaryValve.TabIndex = 292;
            this.lblCmdStartRotaryValve.Visible = false;
            // 
            // txtSV_real
            // 
            this.txtSV_real.Font = new System.Drawing.Font("Arial", 16F);
            this.txtSV_real.Location = new System.Drawing.Point(99, 65);
            this.txtSV_real.Name = "txtSV_real";
            this.txtSV_real.ReadOnly = true;
            this.txtSV_real.Size = new System.Drawing.Size(62, 32);
            this.txtSV_real.TabIndex = 291;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("宋体", 12F);
            this.label10.ForeColor = System.Drawing.Color.Black;
            this.label10.Location = new System.Drawing.Point(202, 111);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(31, 16);
            this.label10.TabIndex = 122;
            this.label10.Text = "RPM";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 12F);
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(162, 73);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 16);
            this.label8.TabIndex = 122;
            this.label8.Text = "1/10 RPM";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnReadData
            // 
            this.btnReadData.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnReadData.Font = new System.Drawing.Font("宋体", 12F);
            this.btnReadData.ForeColor = System.Drawing.Color.Black;
            this.btnReadData.Location = new System.Drawing.Point(249, 105);
            this.btnReadData.Name = "btnReadData";
            this.btnReadData.Size = new System.Drawing.Size(98, 33);
            this.btnReadData.TabIndex = 121;
            this.btnReadData.Text = "读取";
            this.btnReadData.UseVisualStyleBackColor = false;
            this.btnReadData.Click += new System.EventHandler(this.btnReadData_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 12F);
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Location = new System.Drawing.Point(5, 113);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(87, 16);
            this.label9.TabIndex = 122;
            this.label9.Text = "实际转速：";
            // 
            // btnSetZero
            // 
            this.btnSetZero.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnSetZero.Font = new System.Drawing.Font("宋体", 12F);
            this.btnSetZero.ForeColor = System.Drawing.Color.Black;
            this.btnSetZero.Location = new System.Drawing.Point(249, 64);
            this.btnSetZero.Name = "btnSetZero";
            this.btnSetZero.Size = new System.Drawing.Size(98, 33);
            this.btnSetZero.TabIndex = 121;
            this.btnSetZero.Text = "停止";
            this.btnSetZero.UseVisualStyleBackColor = false;
            this.btnSetZero.Click += new System.EventHandler(this.btnSetZero_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 12F);
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(24, 42);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(87, 16);
            this.label7.TabIndex = 292;
            this.label7.Text = "选择串口：";
            this.label7.Click += new System.EventHandler(this.label7_Click);
            // 
            // btnRotate
            // 
            this.btnRotate.Enabled = false;
            this.btnRotate.ForeColor = System.Drawing.Color.Black;
            this.btnRotate.Location = new System.Drawing.Point(28, 564);
            this.btnRotate.Name = "btnRotate";
            this.btnRotate.Size = new System.Drawing.Size(75, 23);
            this.btnRotate.TabIndex = 293;
            this.btnRotate.Text = "正转";
            this.btnRotate.UseVisualStyleBackColor = true;
            this.btnRotate.Click += new System.EventHandler(this.btnRotate_Click);
            // 
            // dGVTimeRotation
            // 
            this.dGVTimeRotation.AllowUserToAddRows = false;
            this.dGVTimeRotation.BackgroundColor = System.Drawing.SystemColors.ControlDark;
            this.dGVTimeRotation.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dGVTimeRotation.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Time,
            this.Rotation});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Format = "N2";
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dGVTimeRotation.DefaultCellStyle = dataGridViewCellStyle3;
            this.dGVTimeRotation.Location = new System.Drawing.Point(25, 305);
            this.dGVTimeRotation.Name = "dGVTimeRotation";
            this.dGVTimeRotation.RowTemplate.Height = 23;
            this.dGVTimeRotation.Size = new System.Drawing.Size(251, 193);
            this.dGVTimeRotation.TabIndex = 294;
            // 
            // Time
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
            this.Time.DefaultCellStyle = dataGridViewCellStyle1;
            this.Time.HeaderText = "时间";
            this.Time.Name = "Time";
            // 
            // Rotation
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Black;
            this.Rotation.DefaultCellStyle = dataGridViewCellStyle2;
            this.Rotation.HeaderText = "转速";
            this.Rotation.Name = "Rotation";
            // 
            // lblTimeRotation
            // 
            this.lblTimeRotation.AutoSize = true;
            this.lblTimeRotation.Font = new System.Drawing.Font("宋体", 12F);
            this.lblTimeRotation.ForeColor = System.Drawing.Color.Black;
            this.lblTimeRotation.Location = new System.Drawing.Point(3, 167);
            this.lblTimeRotation.Name = "lblTimeRotation";
            this.lblTimeRotation.Size = new System.Drawing.Size(31, 16);
            this.lblTimeRotation.TabIndex = 295;
            this.lblTimeRotation.Text = "   ";
            this.lblTimeRotation.Click += new System.EventHandler(this.lblTimeRotation_Click);
            // 
            // timerLoadRotation
            // 
            this.timerLoadRotation.Tick += new System.EventHandler(this.timerLoadRotation_Tick);
            // 
            // btnOpenCOM1
            // 
            this.btnOpenCOM1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnOpenCOM1.Font = new System.Drawing.Font("宋体", 12F);
            this.btnOpenCOM1.ForeColor = System.Drawing.Color.Black;
            this.btnOpenCOM1.Location = new System.Drawing.Point(253, 32);
            this.btnOpenCOM1.Name = "btnOpenCOM1";
            this.btnOpenCOM1.Size = new System.Drawing.Size(98, 37);
            this.btnOpenCOM1.TabIndex = 76;
            this.btnOpenCOM1.Text = "打开串口";
            this.btnOpenCOM1.UseVisualStyleBackColor = false;
            this.btnOpenCOM1.Click += new System.EventHandler(this.btnOpenCOM1_Click);
            // 
            // btnDeleteRow
            // 
            this.btnDeleteRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnDeleteRow.Font = new System.Drawing.Font("宋体", 12F);
            this.btnDeleteRow.ForeColor = System.Drawing.Color.Black;
            this.btnDeleteRow.Location = new System.Drawing.Point(292, 352);
            this.btnDeleteRow.Name = "btnDeleteRow";
            this.btnDeleteRow.Size = new System.Drawing.Size(90, 30);
            this.btnDeleteRow.TabIndex = 76;
            this.btnDeleteRow.Text = "删去";
            this.btnDeleteRow.UseVisualStyleBackColor = false;
            this.btnDeleteRow.Click += new System.EventHandler(this.btnDeleteRow_Click);
            // 
            // btnSaveSequence
            // 
            this.btnSaveSequence.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnSaveSequence.Font = new System.Drawing.Font("宋体", 12F);
            this.btnSaveSequence.ForeColor = System.Drawing.Color.Black;
            this.btnSaveSequence.Location = new System.Drawing.Point(292, 408);
            this.btnSaveSequence.Name = "btnSaveSequence";
            this.btnSaveSequence.Size = new System.Drawing.Size(90, 30);
            this.btnSaveSequence.TabIndex = 76;
            this.btnSaveSequence.Text = "保存";
            this.btnSaveSequence.UseVisualStyleBackColor = false;
            this.btnSaveSequence.Click += new System.EventHandler(this.btnSaveSequence_Click);
            // 
            // btnLoadSequence
            // 
            this.btnLoadSequence.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnLoadSequence.Font = new System.Drawing.Font("宋体", 12F);
            this.btnLoadSequence.ForeColor = System.Drawing.Color.Black;
            this.btnLoadSequence.Location = new System.Drawing.Point(292, 444);
            this.btnLoadSequence.Name = "btnLoadSequence";
            this.btnLoadSequence.Size = new System.Drawing.Size(90, 30);
            this.btnLoadSequence.TabIndex = 76;
            this.btnLoadSequence.Text = "加载";
            this.btnLoadSequence.UseVisualStyleBackColor = false;
            this.btnLoadSequence.Click += new System.EventHandler(this.btnLoadSequence_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("宋体", 12F);
            this.label11.ForeColor = System.Drawing.Color.Black;
            this.label11.Location = new System.Drawing.Point(24, 76);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(79, 16);
            this.label11.TabIndex = 122;
            this.label11.Text = "螺杆A转速";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.numericUpDown1);
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Controls.Add(this.checkBox2);
            this.panel2.Controls.Add(this.label12);
            this.panel2.Controls.Add(this.textBox2);
            this.panel2.Controls.Add(this.numericUpDown2);
            this.panel2.Controls.Add(this.label13);
            this.panel2.Controls.Add(this.label14);
            this.panel2.Controls.Add(this.label15);
            this.panel2.Controls.Add(this.button2);
            this.panel2.Controls.Add(this.label16);
            this.panel2.Controls.Add(this.label17);
            this.panel2.Controls.Add(this.button3);
            this.panel2.Controls.Add(this.button4);
            this.panel2.Enabled = false;
            this.panel2.Location = new System.Drawing.Point(397, 97);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(357, 193);
            this.panel2.TabIndex = 291;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(10, 76);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(73, 21);
            this.numericUpDown1.TabIndex = 293;
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Arial", 16F);
            this.textBox1.Location = new System.Drawing.Point(99, 105);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(97, 32);
            this.textBox1.TabIndex = 291;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Checked = true;
            this.checkBox2.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox2.Font = new System.Drawing.Font("宋体", 12F);
            this.checkBox2.ForeColor = System.Drawing.Color.Black;
            this.checkBox2.Location = new System.Drawing.Point(256, 156);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(90, 20);
            this.checkBox2.TabIndex = 293;
            this.checkBox2.Text = "连续读取";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.Visible = false;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label12
            // 
            this.label12.Font = new System.Drawing.Font("宋体", 12F);
            this.label12.ForeColor = System.Drawing.Color.Black;
            this.label12.Location = new System.Drawing.Point(17, 138);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(227, 45);
            this.label12.TabIndex = 292;
            this.label12.Visible = false;
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("Arial", 16F);
            this.textBox2.Location = new System.Drawing.Point(99, 65);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(62, 32);
            this.textBox2.TabIndex = 291;
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.DecimalPlaces = 1;
            this.numericUpDown2.Font = new System.Drawing.Font("Arial", 16F);
            this.numericUpDown2.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.numericUpDown2.Location = new System.Drawing.Point(99, 23);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.numericUpDown2.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            -2147483648});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(97, 32);
            this.numericUpDown2.TabIndex = 123;
            this.numericUpDown2.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown2.ValueChanged += new System.EventHandler(this.nmudSV_ValueChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("宋体", 12F);
            this.label13.ForeColor = System.Drawing.Color.Black;
            this.label13.Location = new System.Drawing.Point(202, 111);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(31, 16);
            this.label13.TabIndex = 122;
            this.label13.Text = "RPM";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("宋体", 12F);
            this.label14.ForeColor = System.Drawing.Color.Black;
            this.label14.Location = new System.Drawing.Point(162, 73);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(71, 16);
            this.label14.TabIndex = 122;
            this.label14.Text = "1/10 RPM";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("宋体", 12F);
            this.label15.ForeColor = System.Drawing.Color.Black;
            this.label15.Location = new System.Drawing.Point(202, 30);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(31, 16);
            this.label15.TabIndex = 122;
            this.label15.Text = "RPM";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.button2.Font = new System.Drawing.Font("宋体", 12F);
            this.button2.ForeColor = System.Drawing.Color.Black;
            this.button2.Location = new System.Drawing.Point(249, 105);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(98, 33);
            this.button2.TabIndex = 121;
            this.button2.Text = "读取";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.btnReadData_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("宋体", 12F);
            this.label16.ForeColor = System.Drawing.Color.Black;
            this.label16.Location = new System.Drawing.Point(5, 113);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(87, 16);
            this.label16.TabIndex = 122;
            this.label16.Text = "实际转速：";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("宋体", 12F);
            this.label17.ForeColor = System.Drawing.Color.Black;
            this.label17.Location = new System.Drawing.Point(7, 30);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(87, 16);
            this.label17.TabIndex = 122;
            this.label17.Text = "目标转速：";
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.button3.Font = new System.Drawing.Font("宋体", 12F);
            this.button3.ForeColor = System.Drawing.Color.Black;
            this.button3.Location = new System.Drawing.Point(249, 64);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(98, 33);
            this.button3.TabIndex = 121;
            this.button3.Text = "停止";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.btnSetZero_Click);
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.button4.Font = new System.Drawing.Font("宋体", 12F);
            this.button4.ForeColor = System.Drawing.Color.Black;
            this.button4.Location = new System.Drawing.Point(249, 23);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(98, 33);
            this.button4.TabIndex = 121;
            this.button4.Text = "启动";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.btnWriteSV_Click);
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("宋体", 12F);
            this.label18.ForeColor = System.Drawing.Color.Black;
            this.label18.Location = new System.Drawing.Point(397, 77);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(79, 16);
            this.label18.TabIndex = 122;
            this.label18.Text = "螺杆B转速";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ControlDark;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle6.Format = "N2";
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle6;
            this.dataGridView1.Location = new System.Drawing.Point(400, 305);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(251, 193);
            this.dataGridView1.TabIndex = 299;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Black;
            this.dataGridViewTextBoxColumn1.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewTextBoxColumn1.HeaderText = "时间";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.Black;
            this.dataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle5;
            this.dataGridViewTextBoxColumn2.HeaderText = "转速";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // button5
            // 
            this.button5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.button5.Font = new System.Drawing.Font("宋体", 12F);
            this.button5.ForeColor = System.Drawing.Color.Black;
            this.button5.Location = new System.Drawing.Point(667, 444);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(90, 30);
            this.button5.TabIndex = 295;
            this.button5.Text = "加载";
            this.button5.UseVisualStyleBackColor = false;
            // 
            // button6
            // 
            this.button6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.button6.Font = new System.Drawing.Font("宋体", 12F);
            this.button6.ForeColor = System.Drawing.Color.Black;
            this.button6.Location = new System.Drawing.Point(667, 408);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(90, 30);
            this.button6.TabIndex = 296;
            this.button6.Text = "保存";
            this.button6.UseVisualStyleBackColor = false;
            // 
            // button7
            // 
            this.button7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.button7.Font = new System.Drawing.Font("宋体", 12F);
            this.button7.ForeColor = System.Drawing.Color.Black;
            this.button7.Location = new System.Drawing.Point(667, 352);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(90, 30);
            this.button7.TabIndex = 297;
            this.button7.Text = "删去";
            this.button7.UseVisualStyleBackColor = false;
            // 
            // button8
            // 
            this.button8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.button8.Font = new System.Drawing.Font("宋体", 12F);
            this.button8.ForeColor = System.Drawing.Color.Black;
            this.button8.Location = new System.Drawing.Point(667, 316);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(90, 30);
            this.button8.TabIndex = 298;
            this.button8.Text = "添加";
            this.button8.UseVisualStyleBackColor = false;
            // 
            // FrmRotaryValveCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(821, 510);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.dGVTimeRotation);
            this.Controls.Add(this.btnRotate);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.ConnOn);
            this.Controls.Add(this.ConnOff);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.txtCalcuCRC16);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.cmbComPort1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtReceived);
            this.Controls.Add(this.txtCommand);
            this.Controls.Add(this.btnOpenCOM1);
            this.Controls.Add(this.btnLoadSequence);
            this.Controls.Add(this.btnSaveSequence);
            this.Controls.Add(this.btnDeleteRow);
            this.Controls.Add(this.btnSetTimeRotation);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnReadPV);
            this.ForeColor = System.Drawing.Color.White;
            this.Location = new System.Drawing.Point(38, 280);
            this.Name = "FrmRotaryValveCtrl";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "螺杆阀控制";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmTemptTableCtrl_FormClosing);
            this.Click += new System.EventHandler(this.FrmRotaryValveCtrl_Click);
            ((System.ComponentModel.ISupportInitialize)(this.nmudSV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ConnOn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ConnOff)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTimeDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dGVTimeRotation)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox txtCommand;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnSetTimeRotation;
        private System.Windows.Forms.ComboBox cmbComPort1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.TextBox txtReceived;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Button txtCalcuCRC16;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnReadPV;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnWriteSV;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.NumericUpDown nmudSV;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.PictureBox ConnOn;
        private System.Windows.Forms.PictureBox ConnOff;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtSV_real;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnSetZero;
        private System.Windows.Forms.Button btnReadData;
        private System.Windows.Forms.TextBox txtPV_real;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label lblCmdStartRotaryValve;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.IO.Ports.SerialPort serialPort2;
        private System.Windows.Forms.NumericUpDown numTimeDelay;
        private System.Windows.Forms.Button btnRotate;
        private System.Windows.Forms.DataGridView dGVTimeRotation;
        private System.Windows.Forms.Label lblTimeRotation;
        private System.Windows.Forms.Timer timerLoadRotation;
        private System.Windows.Forms.Button btnOpenCOM1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Time;
        private System.Windows.Forms.DataGridViewTextBoxColumn Rotation;
        private System.Windows.Forms.Button btnDeleteRow;
        private System.Windows.Forms.Button btnSaveSequence;
        private System.Windows.Forms.Button btnLoadSequence;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button8;
    }
}

