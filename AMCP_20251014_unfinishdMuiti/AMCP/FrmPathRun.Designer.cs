namespace AMCP
{
    partial class FrmPathRun
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnExcute = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.lblValue = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lblNotice1 = new System.Windows.Forms.Label();
            this.timerWarn = new System.Windows.Forms.Timer(this.components);
            this.lblNotice2 = new System.Windows.Forms.Label();
            this.button9 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.button10 = new System.Windows.Forms.Button();
            this.label21 = new System.Windows.Forms.Label();
            this.txtGcodeFileName = new System.Windows.Forms.TextBox();
            this.btnBrowsePathFile = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.numPreTimeStart = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numPreTimeEnd = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnpathpointPreview = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPreTimeStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPreTimeEnd)).BeginInit();
            this.SuspendLayout();
            // 
            // btnExcute
            // 
            this.btnExcute.BackColor = System.Drawing.Color.White;
            this.btnExcute.Location = new System.Drawing.Point(129, 46);
            this.btnExcute.Margin = new System.Windows.Forms.Padding(4);
            this.btnExcute.Name = "btnExcute";
            this.btnExcute.Size = new System.Drawing.Size(104, 31);
            this.btnExcute.TabIndex = 0;
            this.btnExcute.Text = "执行打印";
            this.btnExcute.UseVisualStyleBackColor = false;
            this.btnExcute.Click += new System.EventHandler(this.btnExcute_Click);
            // 
            // textBox1
            // 
            this.textBox1.ForeColor = System.Drawing.SystemColors.WindowText;
            this.textBox1.Location = new System.Drawing.Point(26, 93);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(323, 195);
            this.textBox1.TabIndex = 2;
            this.textBox1.Text = "\r\n";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(385, 93);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox2.Size = new System.Drawing.Size(351, 195);
            this.textBox2.TabIndex = 2;
            this.textBox2.Text = "\r\n";
            // 
            // lblValue
            // 
            this.lblValue.AutoSize = true;
            this.lblValue.Location = new System.Drawing.Point(32, 9);
            this.lblValue.Name = "lblValue";
            this.lblValue.Size = new System.Drawing.Size(0, 16);
            this.lblValue.TabIndex = 4;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lblNotice1
            // 
            this.lblNotice1.AutoSize = true;
            this.lblNotice1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.lblNotice1.Location = new System.Drawing.Point(32, 304);
            this.lblNotice1.Name = "lblNotice1";
            this.lblNotice1.Size = new System.Drawing.Size(0, 16);
            this.lblNotice1.TabIndex = 11;
            this.lblNotice1.Visible = false;
            // 
            // timerWarn
            // 
            this.timerWarn.Tick += new System.EventHandler(this.timerWarn_Tick);
            // 
            // lblNotice2
            // 
            this.lblNotice2.AutoSize = true;
            this.lblNotice2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.lblNotice2.Location = new System.Drawing.Point(486, 304);
            this.lblNotice2.Name = "lblNotice2";
            this.lblNotice2.Size = new System.Drawing.Size(0, 16);
            this.lblNotice2.TabIndex = 11;
            this.lblNotice2.Visible = false;
            // 
            // button9
            // 
            this.button9.BackColor = System.Drawing.Color.White;
            this.button9.Location = new System.Drawing.Point(31, 46);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(75, 31);
            this.button9.TabIndex = 14;
            this.button9.Text = "复位";
            this.button9.UseVisualStyleBackColor = false;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(32, 363);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 16);
            this.label1.TabIndex = 15;
            this.label1.Text = "         ";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown1.Location = new System.Drawing.Point(732, 353);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(120, 26);
            this.numericUpDown1.TabIndex = 16;
            this.numericUpDown1.Visible = false;
            // 
            // button10
            // 
            this.button10.BackColor = System.Drawing.Color.White;
            this.button10.Location = new System.Drawing.Point(255, 46);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(75, 31);
            this.button10.TabIndex = 14;
            this.button10.Text = "停止";
            this.button10.UseVisualStyleBackColor = false;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("宋体", 12F);
            this.label21.Location = new System.Drawing.Point(24, 406);
            this.label21.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(87, 16);
            this.label21.TabIndex = 157;
            this.label21.Text = "路径文件：";
            // 
            // txtGcodeFileName
            // 
            this.txtGcodeFileName.Location = new System.Drawing.Point(26, 437);
            this.txtGcodeFileName.Multiline = true;
            this.txtGcodeFileName.Name = "txtGcodeFileName";
            this.txtGcodeFileName.ReadOnly = true;
            this.txtGcodeFileName.Size = new System.Drawing.Size(536, 61);
            this.txtGcodeFileName.TabIndex = 158;
            // 
            // btnBrowsePathFile
            // 
            this.btnBrowsePathFile.BackColor = System.Drawing.Color.White;
            this.btnBrowsePathFile.Font = new System.Drawing.Font("宋体", 12F);
            this.btnBrowsePathFile.Location = new System.Drawing.Point(637, 433);
            this.btnBrowsePathFile.Name = "btnBrowsePathFile";
            this.btnBrowsePathFile.Size = new System.Drawing.Size(88, 31);
            this.btnBrowsePathFile.TabIndex = 159;
            this.btnBrowsePathFile.Text = "浏览...";
            this.btnBrowsePathFile.UseVisualStyleBackColor = false;
            this.btnBrowsePathFile.Click += new System.EventHandler(this.btnBrowsePathFile_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // numPreTimeStart
            // 
            this.numPreTimeStart.Increment = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numPreTimeStart.Location = new System.Drawing.Point(459, 48);
            this.numPreTimeStart.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numPreTimeStart.Name = "numPreTimeStart";
            this.numPreTimeStart.Size = new System.Drawing.Size(62, 26);
            this.numPreTimeStart.TabIndex = 160;
            this.numPreTimeStart.ValueChanged += new System.EventHandler(this.numPreTimeStart_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(384, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 16);
            this.label2.TabIndex = 161;
            this.label2.Text = "出丝提前";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(566, 53);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 16);
            this.label3.TabIndex = 161;
            this.label3.Text = "停丝提前";
            // 
            // numPreTimeEnd
            // 
            this.numPreTimeEnd.Increment = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numPreTimeEnd.Location = new System.Drawing.Point(644, 48);
            this.numPreTimeEnd.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numPreTimeEnd.Name = "numPreTimeEnd";
            this.numPreTimeEnd.Size = new System.Drawing.Size(62, 26);
            this.numPreTimeEnd.TabIndex = 160;
            this.numPreTimeEnd.ValueChanged += new System.EventHandler(this.numPreTimeEnd_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(527, 53);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 16);
            this.label4.TabIndex = 161;
            this.label4.Text = "ms";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(710, 53);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 16);
            this.label5.TabIndex = 161;
            this.label5.Text = "ms";
            // 
            // btnpathpointPreview
            // 
            this.btnpathpointPreview.Location = new System.Drawing.Point(637, 470);
            this.btnpathpointPreview.Name = "btnpathpointPreview";
            this.btnpathpointPreview.Size = new System.Drawing.Size(88, 37);
            this.btnpathpointPreview.TabIndex = 162;
            this.btnpathpointPreview.Text = "预览路径";
            this.btnpathpointPreview.UseVisualStyleBackColor = true;
            this.btnpathpointPreview.Visible = false;
            this.btnpathpointPreview.Click += new System.EventHandler(this.btnpathpointPreview_Click);
            // 
            // FrmPathRun
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(745, 549);
            this.Controls.Add(this.btnpathpointPreview);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numPreTimeEnd);
            this.Controls.Add(this.numPreTimeStart);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.txtGcodeFileName);
            this.Controls.Add(this.btnBrowsePathFile);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.lblNotice2);
            this.Controls.Add(this.lblNotice1);
            this.Controls.Add(this.lblValue);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnExcute);
            this.Font = new System.Drawing.Font("宋体", 12F);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "FrmPathRun";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "曲面打印";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmBasicTest_FormClosing);
            this.Load += new System.EventHandler(this.FrmPathRun_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPreTimeStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPreTimeEnd)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExcute;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label lblValue;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lblNotice1;
        private System.Windows.Forms.Timer timerWarn;
        private System.Windows.Forms.Label lblNotice2;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Label label21;
        public System.Windows.Forms.TextBox txtGcodeFileName;
        private System.Windows.Forms.Button btnBrowsePathFile;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.NumericUpDown numPreTimeStart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numPreTimeEnd;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnpathpointPreview;
    }
}