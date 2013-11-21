#region Using

using System;
using System.Windows.Forms;
using Microsoft.Win32;

#endregion

namespace BeamNGModsUpdateChecker
{
    public partial class FrmOptions : Form
    {
        private FrmMain _mainForm;

        public FrmOptions( FrmMain mainForm )
        {
            this._mainForm = mainForm;
            InitializeComponent();
        }

        private void AddRemoveStartup( bool add )
        {
            try
            {
                RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(
                    "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true );
                if ( add && !this.IsStartupItem() )
                {
                    if ( rkApp != null )
                    {
                        rkApp.SetValue( "BeamNGModsUpdateChecker", Application.ExecutablePath );
                    }
                }
                else if ( !add )
                {
                    if ( rkApp != null )
                    {
                        rkApp.DeleteValue( "BeamNGModsUpdateChecker", false );
                    }
                }
            }
            catch
            {
            }
        }

        private bool IsStartupItem()
        {
            try
            {
                RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(
                    "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true );
                if ( rkApp.GetValue( "BeamNGModsUpdateChecker" ) == null )
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private void btnOk_Click( object sender, EventArgs e )
        {
            this._mainForm.MinimizeWhenStart = cbMinimizeToTray.Checked;
            this._mainForm.UpdInterval = (int)nudUpdInterval.Value;
            this.AddRemoveStartup( cbAutorun.Checked );
            this.Close();
        }

        private void frmOptions_Load( object sender, EventArgs e )
        {
            cbAutorun.Checked = this.IsStartupItem();
            cbMinimizeToTray.Checked = this._mainForm.MinimizeWhenStart;
            nudUpdInterval.Value = this._mainForm.UpdInterval;
        }
    }
}