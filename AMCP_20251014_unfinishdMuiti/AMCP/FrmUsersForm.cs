using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AMCP
{
    public partial class FrmUsersForm : Form
    {
        public FrmUsersForm()
        {
            InitializeComponent();
        }

        private void lblStartMain_Click(object sender, EventArgs e)
        {
            // 隐藏或关闭当前窗体
            this.Hide();
            // 启动主窗体
            FrmMain mainForm = new FrmMain();
            mainForm.ShowDialog();
            // 关闭当前窗体
            this.Close();
        }
    }
}
