namespace AMCP
{
    partial class FrmTemptTableCtrl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmTemptTableCtrl));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.txtCommand = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnOpenCOM1 = new System.Windows.Forms.Button();
            this.cmbComPort1 = new System.Windows.Forms.ComboBox();
            this.btnRefresh = new System.Windows.Forms.Button();
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
            this.txtPV = new System.Windows.Forms.TextBox();
            this.btnOpen = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.ConnOn = new System.Windows.Forms.PictureBox();
            this.ConnOff = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nmudSV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ConnOn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ConnOff)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // txtCommand
            // 
            this.txtCommand.Location = new System.Drawing.Point(97, 429);
            this.txtCommand.Multiline = true;
            this.txtCommand.Name = "txtCommand";
            this.txtCommand.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCommand.Size = new System.Drawing.Size(238, 56);
            this.txtCommand.TabIndex = 74;
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(397, 432);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(79, 56);
            this.button1.TabIndex = 75;
            this.button1.Text = "发送指令";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnOpenCOM1
            // 
            this.btnOpenCOM1.BackColor = System.Drawing.Color.White;
            this.btnOpenCOM1.Font = new System.Drawing.Font("宋体", 12F);
            this.btnOpenCOM1.Location = new System.Drawing.Point(245, 32);
            this.btnOpenCOM1.Name = "btnOpenCOM1";
            this.btnOpenCOM1.Size = new System.Drawing.Size(98, 37);
            this.btnOpenCOM1.TabIndex = 76;
            this.btnOpenCOM1.Text = "打开串口";
            this.btnOpenCOM1.UseVisualStyleBackColor = false;
            this.btnOpenCOM1.Click += new System.EventHandler(this.OpenSerialPort);
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
            this.cmbComPort1.Text = "COM6";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Font = new System.Drawing.Font("宋体", 12F);
            this.btnRefresh.Location = new System.Drawing.Point(475, 32);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(98, 37);
            this.btnRefresh.TabIndex = 82;
            this.btnRefresh.Text = "刷新";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Visible = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(25, 432);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 36);
            this.label4.TabIndex = 83;
            this.label4.Text = "发送指令：\r\n\r\n(HEX)";
            // 
            // txtReceived
            // 
            this.txtReceived.Location = new System.Drawing.Point(97, 504);
            this.txtReceived.Multiline = true;
            this.txtReceived.Name = "txtReceived";
            this.txtReceived.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtReceived.Size = new System.Drawing.Size(238, 56);
            this.txtReceived.TabIndex = 74;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 507);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 36);
            this.label1.TabIndex = 83;
            this.label1.Text = "解析指令：\r\n\r\n(ASC)";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(43, 372);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(255, 21);
            this.textBox4.TabIndex = 118;
            // 
            // txtCalcuCRC16
            // 
            this.txtCalcuCRC16.Location = new System.Drawing.Point(304, 365);
            this.txtCalcuCRC16.Name = "txtCalcuCRC16";
            this.txtCalcuCRC16.Size = new System.Drawing.Size(75, 32);
            this.txtCalcuCRC16.TabIndex = 119;
            this.txtCalcuCRC16.Text = "计算校验码";
            this.txtCalcuCRC16.UseVisualStyleBackColor = true;
            this.txtCalcuCRC16.Click += new System.EventHandler(this.button4_Click);
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(391, 372);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(100, 21);
            this.textBox5.TabIndex = 118;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(397, 520);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 40);
            this.btnClear.TabIndex = 120;
            this.btnClear.Text = "清除";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnReadPV
            // 
            this.btnReadPV.BackColor = System.Drawing.Color.White;
            this.btnReadPV.Font = new System.Drawing.Font("宋体", 12F);
            this.btnReadPV.Location = new System.Drawing.Point(230, 92);
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
            this.label2.Location = new System.Drawing.Point(9, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 16);
            this.label2.TabIndex = 122;
            this.label2.Text = "当前温度：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 12F);
            this.label3.Location = new System.Drawing.Point(177, 98);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 16);
            this.label3.TabIndex = 122;
            this.label3.Text = "℃";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 12F);
            this.label5.Location = new System.Drawing.Point(7, 163);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 16);
            this.label5.TabIndex = 122;
            this.label5.Text = "目标温度：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 12F);
            this.label6.Location = new System.Drawing.Point(177, 163);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 16);
            this.label6.TabIndex = 122;
            this.label6.Text = "℃";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnWriteSV
            // 
            this.btnWriteSV.BackColor = System.Drawing.Color.White;
            this.btnWriteSV.Font = new System.Drawing.Font("宋体", 12F);
            this.btnWriteSV.Location = new System.Drawing.Point(230, 155);
            this.btnWriteSV.Name = "btnWriteSV";
            this.btnWriteSV.Size = new System.Drawing.Size(98, 33);
            this.btnWriteSV.TabIndex = 121;
            this.btnWriteSV.Text = "设置";
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
            this.nmudSV.Font = new System.Drawing.Font("Arial", 16F);
            this.nmudSV.Location = new System.Drawing.Point(99, 156);
            this.nmudSV.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.nmudSV.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            -2147483648});
            this.nmudSV.Name = "nmudSV";
            this.nmudSV.Size = new System.Drawing.Size(72, 32);
            this.nmudSV.TabIndex = 123;
            // 
            // txtPV
            // 
            this.txtPV.Font = new System.Drawing.Font("Arial", 16F);
            this.txtPV.Location = new System.Drawing.Point(99, 92);
            this.txtPV.Name = "txtPV";
            this.txtPV.Size = new System.Drawing.Size(72, 32);
            this.txtPV.TabIndex = 124;
            // 
            // btnOpen
            // 
            this.btnOpen.BackColor = System.Drawing.Color.White;
            this.btnOpen.Font = new System.Drawing.Font("宋体", 12F);
            this.btnOpen.Location = new System.Drawing.Point(99, 20);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(98, 32);
            this.btnOpen.TabIndex = 121;
            this.btnOpen.Text = "开机";
            this.btnOpen.UseVisualStyleBackColor = false;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.White;
            this.btnClose.Font = new System.Drawing.Font("宋体", 12F);
            this.btnClose.Location = new System.Drawing.Point(230, 20);
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
            this.ConnOn.Location = new System.Drawing.Point(12, 20);
            this.ConnOn.Name = "ConnOn";
            this.ConnOn.Size = new System.Drawing.Size(29, 30);
            this.ConnOn.TabIndex = 290;
            this.ConnOn.TabStop = false;
            this.ConnOn.Visible = false;
            // 
            // ConnOff
            // 
            this.ConnOff.Image = ((System.Drawing.Image)(resources.GetObject("ConnOff.Image")));
            this.ConnOff.Location = new System.Drawing.Point(12, 20);
            this.ConnOff.Name = "ConnOff";
            this.ConnOff.Size = new System.Drawing.Size(29, 30);
            this.ConnOff.TabIndex = 289;
            this.ConnOff.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ConnOn);
            this.panel1.Controls.Add(this.ConnOff);
            this.panel1.Controls.Add(this.txtPV);
            this.panel1.Controls.Add(this.nmudSV);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.btnWriteSV);
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.btnOpen);
            this.panel1.Controls.Add(this.btnReadPV);
            this.panel1.Enabled = false;
            this.panel1.Location = new System.Drawing.Point(15, 83);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(357, 227);
            this.panel1.TabIndex = 291;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 12F);
            this.label7.Location = new System.Drawing.Point(24, 42);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 16);
            this.label7.TabIndex = 292;
            this.label7.Text = "选择串口：";
            this.label7.Click += new System.EventHandler(this.label7_Click);
            // 
            // FrmTemptTableCtrl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(388, 319);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.txtCalcuCRC16);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.cmbComPort1);
            this.Controls.Add(this.txtReceived);
            this.Controls.Add(this.txtCommand);
            this.Controls.Add(this.btnOpenCOM1);
            this.Controls.Add(this.button1);
            this.Name = "FrmTemptTableCtrl";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "恒温平台温度控制";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmTemptTableCtrl_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.nmudSV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ConnOn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ConnOff)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.TextBox txtCommand;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnOpenCOM1;
        private System.Windows.Forms.ComboBox cmbComPort1;
        private System.Windows.Forms.Button btnRefresh;
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
        private System.Windows.Forms.TextBox txtPV;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.PictureBox ConnOn;
        private System.Windows.Forms.PictureBox ConnOff;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label7;
    }
}

