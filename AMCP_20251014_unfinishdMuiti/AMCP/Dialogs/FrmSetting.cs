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
    public partial class FrmSetting : Form
    {
        public FrmSetting()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                int udiX = (int)numericUpDown1.Value;
                int portX = (int)numericUpDown2.Value;
                int udiY = (int)numericUpDown3.Value;
                int portY = (int)numericUpDown4.Value;
                portX = GV.PrintingObj.Ch.ReadVariable("portX", 6);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
