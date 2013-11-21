#region Using

using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

#endregion

namespace BeamNGModsUpdateChecker
{
    public partial class FrmAddLinks : Form
    {
        private UpdateChecker upd;
        private bool _addingLinks = false;

        public FrmAddLinks( UpdateChecker upd, FrmMain mainForm )
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo( mainForm.Lang );
            InitializeComponent();
            this.upd = upd;
        }

        private void AddLinks()
        {
            string[] links = tbLinks.Lines;
            pb1.Maximum = links.Length;
            for ( int i = 0; i < links.Length; i++ )
            {
                string link = links[ i ];
                this.upd.AddThread( link );
                Thread.Sleep( 50 );
                pb1.PerformStep();
            }
            this._addingLinks = false;
            this.Close();
        }

        private void btnAdd_Click( object sender, EventArgs e )
        {
            this._addingLinks = true;
            pb1.Value = 0;
            tbLinks.Enabled = false;
            btnAdd.Enabled = false;
            lblStatus.Show();
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