using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace BeamNGModsUpdateChecker
{
    public partial class frmEnterPassword : Form
    {
        frmMain MainForm;

        public frmEnterPassword( frmMain MainForm )
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo( MainForm.lang );
            InitializeComponent();
            this.MainForm = MainForm;
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOk_Click( object sender, EventArgs e )
        {
            this.MainForm.setLoginPassword( tbLogin.Text, tbPassword.Text );
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
