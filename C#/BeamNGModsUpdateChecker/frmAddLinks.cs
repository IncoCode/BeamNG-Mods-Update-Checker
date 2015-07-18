#region Using

using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using BeamNGModsUpdateChecker.Properties;

#endregion

namespace BeamNGModsUpdateChecker
{
    public partial class FrmAddLinks : Form
    {
        private readonly UpdateChecker _upd;
        private bool _addingLinks;
        private readonly Settings _settings = Settings.Default;

        public FrmAddLinks( UpdateChecker upd, FrmMain mainForm )
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo( this._settings.Lang );
            this.InitializeComponent();
            this._upd = upd;
        }

        private void AddLinks()
        {
            string[] links = this.tbLinks.Lines;
            this.pb1.Maximum = links.Length;
            foreach ( string link in links )
            {
                this._upd.AddThread( link );
                Thread.Sleep( 50 );
                this.pb1.PerformStep();
            }
            this._addingLinks = false;
            this.Close();
        }

        private void btnAdd_Click( object sender, EventArgs e )
        {
            this._addingLinks = true;
            this.pb1.Value = 0;
            this.tbLinks.Enabled = false;
            this.btnAdd.Enabled = false;
            this.lblStatus.Show();
            var thr = new Thread( this.AddLinks );
            thr.Start();
        }

        private void frmAddLinks_FormClosing( object sender, FormClosingEventArgs e )
        {
            if ( this._addingLinks )
            {
                e.Cancel = true;
            }
        }
    }
}