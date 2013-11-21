#region Using

using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

#endregion

namespace BeamNGModsUpdateChecker
{
    public partial class FrmEnterPassword : Form
    {
        private FrmMain _mainForm;

        public FrmEnterPassword( FrmMain mainForm )
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo( mainForm.Lang );
            InitializeComponent();
            this._mainForm = mainForm;
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOk_Click( object sender, EventArgs e )
        {
            this._mainForm.SetLoginPassword( tbLogin.Text, tbPassword.Text );
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}