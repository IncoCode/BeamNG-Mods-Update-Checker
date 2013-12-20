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
        private readonly FrmMain _mainForm;

        public FrmEnterPassword( FrmMain mainForm )
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo( mainForm.Lang );
            InitializeComponent();
            this._mainForm = mainForm;
            this.DialogResult = DialogResult.Cancel;
        }

        private void btnOk_Click( object sender, EventArgs e )
        {
            if ( string.IsNullOrEmpty( tbLogin.Text ) || string.IsNullOrEmpty( tbPassword.Text ) )
            {
                MessageBox.Show( strings.incorrectFilledFields, strings.error, MessageBoxButtons.OK,
                    MessageBoxIcon.Error );
                return;
            }
            this._mainForm.SetLoginPassword( tbLogin.Text, tbPassword.Text );
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void FrmEnterPassword_KeyDown( object sender, KeyEventArgs e )
        {
            if ( e.KeyData == Keys.Enter )
            {
                btnOk.PerformClick();
            }
        }
    }
}