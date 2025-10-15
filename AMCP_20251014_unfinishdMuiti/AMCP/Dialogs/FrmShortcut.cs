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
    public partial class FrmShortcut : Form
    {
        public FrmShortcut()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label1.Visible = true;
            label1.Text = (sender as Control).Tag.ToString();
        }
    }
}
