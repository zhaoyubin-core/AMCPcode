namespace AMCP
{
    partial class FrmImmStop
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
            this.btnStopImmediately = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // btnStopImmediately
            // 
            this.btnStopImmediately.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnStopImmediately.Font = new System.Drawing.Font("微软雅黑", 24F);
            this.btnStopImmediately.ForeColor = System.Drawing.Color.White;
            this.btnStopImmediately.Location = new System.Drawing.Point(-1, 0);
            this.btnStopImmediately.Margin = new System.Windows.Forms.Padding(2);
            this.btnStopImmediately.Name = "btnStopImmediately";
            this.btnStopImmediately.Size = new System.Drawing.Size(129, 87);
            this.btnStopImmediately.TabIndex = 0;
            this.btnStopImmediately.Text = "STOP";
            this.btnStopImmediately.UseVisualStyleBackColor = false;
            this.btnStopImmediately.Click += new System.EventHandler(this.btnStopImmediately_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FrmImmStop
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(129, 88);
            this.ControlBox = false;
            this.Controls.Add(this.btnStopImmediately);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Location = new System.Drawing.Point(0, 850);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmImmStop";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "  紧急停止 (Space)";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.stopImmediately_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStopImmediately;
        private System.Windows.Forms.Timer timer1;
    }
}