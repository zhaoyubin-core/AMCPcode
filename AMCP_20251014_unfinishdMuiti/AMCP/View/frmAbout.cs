using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace AMCP
{
    partial class frmAbout : Form
    {
        public frmAbout()
        {
            InitializeComponent();
            this.Text = String.Format("关于 {0}", GV.AssemblyTitle);
            this.labelProductName.Text = GV.AssemblyProduct;
            this.labelVersion.Text = String.Format("版本 {0}", GV.AssemblyVersion);
            this.labelCopyright.Text = GV.AssemblyCopyright;
            this.labelCompanyName.Text = GV.AssemblyCompany;
            this.textBoxDescription.Text = GV.AssemblyDescription;
        }

        //#region 程序集特性访问器

        //public string AssemblyTitle
        //{
        //    get
        //    {
        //        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
        //        if (attributes.Length > 0)
        //        {
        //            AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
        //            if (titleAttribute.Title != "")
        //            {
        //                return titleAttribute.Title;
        //            }
        //        }
        //        return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
        //    }
        //}

        //public string AssemblyVersion
        //{
        //    get
        //    {
        //        return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        //    }
        //}

        //public string AssemblyDescription
        //{
        //    get
        //    {
        //        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
        //        if (attributes.Length == 0)
        //        {
        //            return "";
        //        }
        //        return ((AssemblyDescriptionAttribute)attributes[0]).Description;
        //    }
        //}

        //public string AssemblyProduct
        //{
        //    get
        //    {
        //        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
        //        if (attributes.Length == 0)
        //        {
        //            return "";
        //        }
        //        return ((AssemblyProductAttribute)attributes[0]).Product;
        //    }
        //}

        //public string AssemblyCopyright
        //{
        //    get
        //    {
        //        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
        //        if (attributes.Length == 0)
        //        {
        //            return "";
        //        }
        //        return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
        //    }
        //}

        //public string AssemblyCompany
        //{
        //    get
        //    {
        //        object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
        //        if (attributes.Length == 0)
        //        {
        //            return "";
        //        }
        //        return ((AssemblyCompanyAttribute)attributes[0]).Company;
        //    }
        //}
        //#endregion

        private void okButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
