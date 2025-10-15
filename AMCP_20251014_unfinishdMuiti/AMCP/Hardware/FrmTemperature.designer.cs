namespace AMCP
{
    partial class FrmTemperature
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
            this.txtPV1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnWriteSV = new System.Windows.Forms.Button();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.label7 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtPV2 = new System.Windows.Forms.TextBox();
            this.txtSV1 = new System.Windows.Forms.NumericUpDown();
            this.txtSV2 = new System.Windows.Forms.NumericUpDown();
            this.btnWriteSV2 = new System.Windows.Forms.Button();
            this.btnSet1 = new System.Windows.Forms.Button();
            this.btnSet2 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.txtSV1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSV2)).BeginInit();
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
            this.txtCommand.Location = new System.Drawing.Point(143, 494);
            this.txtCommand.Multiline = true;
            this.txtCommand.Name = "txtCommand";
            this.txtCommand.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtCommand.Size = new System.Drawing.Size(238, 56);
            this.txtCommand.TabIndex = 74;
            this.txtCommand.Visible = false;
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(443, 497);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(79, 56);
            this.button1.TabIndex = 75;
            this.button1.Text = "发送指令";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
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
            "COM1"});
            this.cmbComPort1.Location = new System.Drawing.Point(114, 42);
            this.cmbComPort1.Name = "cmbComPort1";
            this.cmbComPort1.Size = new System.Drawing.Size(102, 24);
            this.cmbComPort1.TabIndex = 81;
            this.cmbComPort1.Text = "COM1";
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.White;
            this.btnRefresh.Font = new System.Drawing.Font("宋体", 12F);
            this.btnRefresh.Location = new System.Drawing.Point(531, 32);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(85, 37);
            this.btnRefresh.TabIndex = 82;
            this.btnRefresh.Text = "刷新";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(71, 497);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 36);
            this.label4.TabIndex = 83;
            this.label4.Text = "发送指令：\r\n\r\n(HEX)";
            this.label4.Visible = false;
            // 
            // txtReceived
            // 
            this.txtReceived.Location = new System.Drawing.Point(143, 569);
            this.txtReceived.Multiline = true;
            this.txtReceived.Name = "txtReceived";
            this.txtReceived.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtReceived.Size = new System.Drawing.Size(238, 56);
            this.txtReceived.TabIndex = 74;
            this.txtReceived.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(72, 572);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 36);
            this.label1.TabIndex = 83;
            this.label1.Text = "解析指令：\r\n\r\n(ASC)";
            this.label1.Visible = false;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(89, 437);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(255, 21);
            this.textBox4.TabIndex = 118;
            this.textBox4.Visible = false;
            // 
            // txtCalcuCRC16
            // 
            this.txtCalcuCRC16.Location = new System.Drawing.Point(350, 430);
            this.txtCalcuCRC16.Name = "txtCalcuCRC16";
            this.txtCalcuCRC16.Size = new System.Drawing.Size(75, 32);
            this.txtCalcuCRC16.TabIndex = 119;
            this.txtCalcuCRC16.Text = "计算校验码";
            this.txtCalcuCRC16.UseVisualStyleBackColor = true;
            this.txtCalcuCRC16.Visible = false;
            this.txtCalcuCRC16.Click += new System.EventHandler(this.button4_Click);
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(437, 437);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(100, 21);
            this.textBox5.TabIndex = 118;
            this.textBox5.Visible = false;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(443, 585);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 40);
            this.btnClear.TabIndex = 120;
            this.btnClear.Text = "清除";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Visible = false;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnReadPV
            // 
            this.btnReadPV.BackColor = System.Drawing.Color.White;
            this.btnReadPV.Font = new System.Drawing.Font("宋体", 12F);
            this.btnReadPV.Location = new System.Drawing.Point(531, 93);
            this.btnReadPV.Name = "btnReadPV";
            this.btnReadPV.Size = new System.Drawing.Size(98, 28);
            this.btnReadPV.TabIndex = 121;
            this.btnReadPV.Text = "立即读取";
            this.btnReadPV.UseVisualStyleBackColor = false;
            this.btnReadPV.Click += new System.EventHandler(this.btnReadPV_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F);
            this.label2.Location = new System.Drawing.Point(9, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 16);
            this.label2.TabIndex = 122;
            this.label2.Text = "当前值：";
            // 
            // txtPV1
            // 
            this.txtPV1.BackColor = System.Drawing.Color.White;
            this.txtPV1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPV1.Font = new System.Drawing.Font("Arial", 16F);
            this.txtPV1.Location = new System.Drawing.Point(78, 62);
            this.txtPV1.Name = "txtPV1";
            this.txtPV1.Size = new System.Drawing.Size(72, 32);
            this.txtPV1.TabIndex = 118;
            this.txtPV1.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 12F);
            this.label3.Location = new System.Drawing.Point(156, 70);
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
            this.label5.Location = new System.Drawing.Point(7, 136);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 16);
            this.label5.TabIndex = 122;
            this.label5.Text = "目标值：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 12F);
            this.label6.Location = new System.Drawing.Point(156, 136);
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
            this.btnWriteSV.Location = new System.Drawing.Point(78, 176);
            this.btnWriteSV.Name = "btnWriteSV";
            this.btnWriteSV.Size = new System.Drawing.Size(72, 33);
            this.btnWriteSV.TabIndex = 121;
            this.btnWriteSV.Text = "设置";
            this.btnWriteSV.UseVisualStyleBackColor = false;
            this.btnWriteSV.Click += new System.EventHandler(this.btnWriteSV1_Click);
            // 
            // timer2
            // 
            this.timer2.Interval = 1000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("宋体", 12F);
            this.label7.Location = new System.Drawing.Point(22, 45);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 16);
            this.label7.TabIndex = 122;
            this.label7.Text = "选择串口：";
            this.label7.Click += new System.EventHandler(this.label7_Click);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label17.Location = new System.Drawing.Point(257, 11);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(110, 16);
            this.label17.TabIndex = 124;
            this.label17.Text = "低温喷头温度";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label16.Location = new System.Drawing.Point(9, 11);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(110, 16);
            this.label16.TabIndex = 123;
            this.label16.Text = "高温喷头温度";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("宋体", 12F);
            this.label8.Location = new System.Drawing.Point(257, 70);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(72, 16);
            this.label8.TabIndex = 122;
            this.label8.Text = "当前值：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("宋体", 12F);
            this.label9.Location = new System.Drawing.Point(255, 136);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 16);
            this.label9.TabIndex = 122;
            this.label9.Text = "目标值：";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("宋体", 12F);
            this.label10.Location = new System.Drawing.Point(404, 70);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(24, 16);
            this.label10.TabIndex = 122;
            this.label10.Text = "℃";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("宋体", 12F);
            this.label11.Location = new System.Drawing.Point(404, 136);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(24, 16);
            this.label11.TabIndex = 122;
            this.label11.Text = "℃";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtPV2
            // 
            this.txtPV2.BackColor = System.Drawing.Color.White;
            this.txtPV2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPV2.Font = new System.Drawing.Font("Arial", 16F);
            this.txtPV2.Location = new System.Drawing.Point(326, 62);
            this.txtPV2.Name = "txtPV2";
            this.txtPV2.Size = new System.Drawing.Size(72, 32);
            this.txtPV2.TabIndex = 118;
            this.txtPV2.Text = "0";
            // 
            // txtSV1
            // 
            this.txtSV1.Font = new System.Drawing.Font("Arial", 16F);
            this.txtSV1.Location = new System.Drawing.Point(78, 128);
            this.txtSV1.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.txtSV1.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            -2147483648});
            this.txtSV1.Name = "txtSV1";
            this.txtSV1.Size = new System.Drawing.Size(72, 32);
            this.txtSV1.TabIndex = 125;
            this.txtSV1.Enter += new System.EventHandler(this.txtSV_Enter);
            this.txtSV1.Leave += new System.EventHandler(this.txtSV_Leave);
            // 
            // txtSV2
            // 
            this.txtSV2.Font = new System.Drawing.Font("Arial", 16F);
            this.txtSV2.Location = new System.Drawing.Point(326, 128);
            this.txtSV2.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.txtSV2.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            -2147483648});
            this.txtSV2.Name = "txtSV2";
            this.txtSV2.Size = new System.Drawing.Size(72, 32);
            this.txtSV2.TabIndex = 125;
            this.txtSV2.Enter += new System.EventHandler(this.txtSV_Enter);
            this.txtSV2.Leave += new System.EventHandler(this.txtSV_Leave);
            // 
            // btnWriteSV2
            // 
            this.btnWriteSV2.BackColor = System.Drawing.Color.White;
            this.btnWriteSV2.Font = new System.Drawing.Font("宋体", 12F);
            this.btnWriteSV2.Location = new System.Drawing.Point(326, 176);
            this.btnWriteSV2.Name = "btnWriteSV2";
            this.btnWriteSV2.Size = new System.Drawing.Size(72, 33);
            this.btnWriteSV2.TabIndex = 121;
            this.btnWriteSV2.Text = "设置";
            this.btnWriteSV2.UseVisualStyleBackColor = false;
            this.btnWriteSV2.Click += new System.EventHandler(this.btnWriteSV2_Click);
            // 
            // btnSet1
            // 
            this.btnSet1.BackColor = System.Drawing.Color.White;
            this.btnSet1.Font = new System.Drawing.Font("宋体", 12F);
            this.btnSet1.Location = new System.Drawing.Point(12, 176);
            this.btnSet1.Name = "btnSet1";
            this.btnSet1.Size = new System.Drawing.Size(42, 33);
            this.btnSet1.TabIndex = 121;
            this.btnSet1.Text = "0";
            this.btnSet1.UseVisualStyleBackColor = false;
            this.btnSet1.Click += new System.EventHandler(this.btnSet1_Click);
            // 
            // btnSet2
            // 
            this.btnSet2.BackColor = System.Drawing.Color.White;
            this.btnSet2.Font = new System.Drawing.Font("宋体", 12F);
            this.btnSet2.Location = new System.Drawing.Point(269, 176);
            this.btnSet2.Name = "btnSet2";
            this.btnSet2.Size = new System.Drawing.Size(42, 33);
            this.btnSet2.TabIndex = 121;
            this.btnSet2.Text = "100";
            this.btnSet2.UseVisualStyleBackColor = false;
            this.btnSet2.Click += new System.EventHandler(this.btnSet2_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtSV2);
            this.panel1.Controls.Add(this.txtSV1);
            this.panel1.Controls.Add(this.txtPV2);
            this.panel1.Controls.Add(this.txtPV1);
            this.panel1.Controls.Add(this.label17);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.label16);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.btnWriteSV2);
            this.panel1.Controls.Add(this.btnSet2);
            this.panel1.Controls.Add(this.btnSet1);
            this.panel1.Controls.Add(this.btnWriteSV);
            this.panel1.Enabled = false;
            this.panel1.Location = new System.Drawing.Point(15, 88);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(447, 225);
            this.panel1.TabIndex = 126;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.White;
            this.button2.Location = new System.Drawing.Point(350, 369);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 127;
            this.button2.Text = "写入";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.White;
            this.button3.Location = new System.Drawing.Point(143, 371);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 127;
            this.button3.Text = "读取";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(49, 371);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(75, 21);
            this.textBox1.TabIndex = 128;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(269, 369);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(75, 21);
            this.textBox2.TabIndex = 128;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(49, 410);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(376, 21);
            this.textBox3.TabIndex = 128;
            // 
            // FrmTemperature
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(474, 322);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnReadPV);
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
            this.Name = "FrmTemperature";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "高低温喷头温度控制";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmTemperature_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.txtSV1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtSV2)).EndInit();
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
        private System.Windows.Forms.TextBox txtPV1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnWriteSV;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtPV2;
        private System.Windows.Forms.NumericUpDown txtSV1;
        private System.Windows.Forms.NumericUpDown txtSV2;
        private System.Windows.Forms.Button btnWriteSV2;
        private System.Windows.Forms.Button btnSet1;
        private System.Windows.Forms.Button btnSet2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
    }
}

