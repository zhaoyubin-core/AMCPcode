namespace AMCP
{
    partial class FrmPrintGuide
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnPreStep = new System.Windows.Forms.Button();
            this.btnNextStep = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(859, 858);
            this.panel1.TabIndex = 1;
            this.panel1.Visible = false;
            // 
            // btnPreStep
            // 
            this.btnPreStep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPreStep.BackColor = System.Drawing.Color.White;
            this.btnPreStep.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnPreStep.Location = new System.Drawing.Point(433, 798);
            this.btnPreStep.Margin = new System.Windows.Forms.Padding(2);
            this.btnPreStep.Name = "btnPreStep";
            this.btnPreStep.Size = new System.Drawing.Size(112, 37);
            this.btnPreStep.TabIndex = 2;
            this.btnPreStep.Text = "上一步(&B)";
            this.btnPreStep.UseVisualStyleBackColor = false;
            this.btnPreStep.Click += new System.EventHandler(this.btnPreStep_Click);
            // 
            // btnNextStep
            // 
            this.btnNextStep.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNextStep.BackColor = System.Drawing.Color.White;
            this.btnNextStep.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnNextStep.Location = new System.Drawing.Point(613, 798);
            this.btnNextStep.Margin = new System.Windows.Forms.Padding(2);
            this.btnNextStep.Name = "btnNextStep";
            this.btnNextStep.Size = new System.Drawing.Size(112, 37);
            this.btnNextStep.TabIndex = 221;
            this.btnNextStep.Text = "下一步(&N)";
            this.btnNextStep.UseVisualStyleBackColor = false;
            this.btnNextStep.Click += new System.EventHandler(this.btnNextStep_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 50;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FrmPrintGuide
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(859, 858);
            this.Controls.Add(this.btnPreStep);
            this.Controls.Add(this.btnNextStep);
            this.Controls.Add(this.panel1);
            this.MinimizeBox = false;
            this.Name = "FrmPrintGuide";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "打印向导";
            this.Activated += new System.EventHandler(this.FrmPrintSteps_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmPrintSteps_FormClosing);
            this.Load += new System.EventHandler(this.FrmPrintSteps_Load);
            this.Enter += new System.EventHandler(this.FrmPrintSteps_Enter);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnPreStep;
        public System.Windows.Forms.Button btnNextStep;
        private System.Windows.Forms.Timer timer1;

    }
}