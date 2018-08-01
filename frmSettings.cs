using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageConverter
{
    public partial class frmSettings : Form
    {
        public string FtpPath
        {
            get;
            private set;
        }

        public string UserName
        {
            get;
            private set;
        }

        public string Password
        {
            get;
            private set;
        }

        public frmSettings()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            bool isModified = false;

            if (!String.IsNullOrEmpty(txtName.Text))
            {
                UserName = txtName.Text;
                Properties.Settings.Default.FtpName = txtName.Text;
                isModified = true;
            }

            if (!String.IsNullOrEmpty(txtPassword.Text))
            {
                Password = txtPassword.Text;
                Properties.Settings.Default.FtpPassword = txtPassword.Text;
                isModified = true;
            }

            if (!String.IsNullOrEmpty(txtPath.Text))
            {
                FtpPath = txtPath.Text;
                Properties.Settings.Default.FtpPath = txtPath.Text;
                isModified = true;
            }

            if (isModified)
            {
                Properties.Settings.Default.Save();
                MessageBox.Show("Настройки сохранены.");
            }
        }


        private void frmSettings_Shown(object sender, EventArgs e)
        {
            txtName.Text = Properties.Settings.Default.FtpName;
            txtPassword.Text = Properties.Settings.Default.FtpPassword;
            txtPath.Text = Properties.Settings.Default.FtpPath;
        }
    }
}
