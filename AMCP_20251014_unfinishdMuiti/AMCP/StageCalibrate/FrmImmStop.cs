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
    public partial class FrmImmStop : Form
    {
        public FrmImmStop()
        {
            InitializeComponent();
            //this.Left = GV.frmMain.Left + GV.frmMain.Width - this.Width - 50;
            //this.Top = GV.frmMain.Top + GV.frmMain.Height - this.Height - 200;
        }

        private void stopImmediately_Load(object sender, EventArgs e)
        {
            //GV = this.MdiParent as FrmMain;
            this.ShowInTaskbar = false;
        }

        private void btnStopImmediately_Click(object sender, EventArgs e)
        {
            //this.Text = this.Location.ToString() + ", " + this.Size.ToString();
            GV.StopImmediately();
            TwinkleButton(2);
        }

        Color color1 = Color.FromArgb(192, 0, 0);
        Color color2 = Color.FromArgb(255, 255, 255);
        int twinkle = 6;
        private void timer1_Tick(object sender, EventArgs e)
        {
            twinkle--;
            if (twinkle > 0)
            {
                if (twinkle % 2 == 0)
                {
                    // 反色显示
                    btnStopImmediately.ForeColor = color1;
                    btnStopImmediately.BackColor = color2;
                }
                else
                {
                    // 正常颜色
                    btnStopImmediately.ForeColor = color2;
                    btnStopImmediately.BackColor = color1;
                }
            }
            else
            {
                timer1.Stop();
                bStopImmediatelyPressedRightnow = false;

                this.Text = "紧急停止 (Space)";
           }            
        }

        static bool bStopImmediatelyPressedRightnow = false;

        public void TwinkleButton(int times)
        {
            if (bStopImmediatelyPressedRightnow)
            {
                GV.PrintingObj.Ch.DisableAll();
            }
            else
            {
                bStopImmediatelyPressedRightnow = true;
            }
            twinkle = times * 2;
            btnStopImmediately.ForeColor = color1;
            btnStopImmediately.BackColor = color2;
            this.Text = "已急停！";
            timer1.Start();
        }

    }
}
