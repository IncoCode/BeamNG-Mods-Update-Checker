using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace BeamNGModsUpdateChecker
{
    public partial class frmOptions : Form
    {
        private frmMain MainForm;

        public frmOptions( frmMain MainForm )
        {
            this.MainForm = MainForm;
            InitializeComponent();
        }

        private void addRemoveStartup( bool add )
        {
            try
            {
                RegistryKey rkApp = Registry.CurrentUser.OpenSubKey( "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true );
                if ( add && !this.IsStartupItem() )
                {
                    rkApp.SetValue( "BeamNGModsUpdateChecker", Application.ExecutablePath.ToString() );
                }
                else if ( !add )
                {
                    rkApp.DeleteValue( "BeamNGModsUpdateChecker", false );
                }
            }
            catch
            {
                return;
            }
        }

        private bool IsStartupItem()
        {
            try
            {
                RegistryKey rkApp = Registry.CurrentUser.OpenSubKey( "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true );
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
            this.MainForm.minimizeWhenStart = cbMinimizeToTray.Checked;
            this.MainForm.updInterval = (int)nudUpdInterval.Value;
            this.addRemoveStartup( cbAutorun.Checked );
            this.Close();
        }

        private void frmOptions_Load( object sender, EventArgs e )
        {
            cbAutorun.Checked = this.IsStartupItem();
            cbMinimizeToTray.Checked = this.MainForm.minimizeWhenStart;
            nudUpdInterval.Value = this.MainForm.updInterval;
        }
    }
}