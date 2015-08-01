#region Using

using System;
using System.Windows.Forms;
using BeamNGModsUpdateChecker.Properties;
using Microsoft.Win32;

#endregion

namespace BeamNGModsUpdateChecker
{
    public partial class FrmOptions : Form
    {
        private readonly Settings _settings = Settings.Default;

        public FrmOptions()
        {
            this.InitializeComponent();
        }

        private void AddRemoveStartup( bool add )
        {
            try
            {
                RegistryKey rkApp = Registry.CurrentUser.OpenSubKey(
                    "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true );
                if ( add && !this.IsStartupItem() )
                {
                    rkApp?.SetValue( "BeamNGModsUpdateChecker", Application.ExecutablePath );
                }
                else if ( !add )
                {
                    rkApp?.DeleteValue( "BeamNGModsUpdateChecker", false );
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
                return rkApp?.GetValue( "BeamNGModsUpdateChecker" ) != null;
            }
            catch
            {
                return false;
            }
        }

        private void btnOk_Click( object sender, EventArgs e )
        {
            this._settings.MinimizeOnStart = this.cbMinimizeToTray.Checked;
            this._settings.UpdInterval = (int)this.nudUpdInterval.Value;
            this._settings.AutomaticallyCheckForUpdates = this.cbAutomaticallyCheckForUpdates.Checked;
            this.AddRemoveStartup( this.cbAutorun.Checked );
            this.Close();
        }

        private void frmOptions_Load( object sender, EventArgs e )
        {
            this.cbAutorun.Checked = this.IsStartupItem();
            this.cbMinimizeToTray.Checked = this._settings.MinimizeOnStart;
            this.nudUpdInterval.Value = this._settings.UpdInterval;
            this.cbAutomaticallyCheckForUpdates.Checked = this._settings.AutomaticallyCheckForUpdates;
        }
    }
}