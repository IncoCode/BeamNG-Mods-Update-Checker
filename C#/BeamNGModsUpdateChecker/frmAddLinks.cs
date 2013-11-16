using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace BeamNGModsUpdateChecker
{
    public partial class frmAddLinks : Form
    {
        private UpdateChecker upd;
        private bool addingLinks = false;

        public frmAddLinks( UpdateChecker upd, frmMain MainForm )
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo( MainForm.lang );
            InitializeComponent();
            this.upd = upd;
        }

        private void addLinks()
        {
            string[] links = tbLinks.Lines;
            pb1.Maximum = links.Length;
            for ( int i = 0; i < links.Length; i++ )
            {
                string link = links[ i ];
                this.upd.addThread( link );
                Thread.Sleep( 50 );
                pb1.PerformStep();
            }
            this.addingLinks = false;
            this.Close();
        }

        private void btnAdd_Click( object sender, EventArgs e )
        {
            this.addingLinks = true;
            pb1.Value = 0;
            tbLinks.Enabled = false;
            btnAdd.Enabled = false;
            lblStatus.Show();
            Thread thr = new Thread( this.addLinks );
            thr.Start();
        }

        private void frmAddLinks_FormClosing( object sender, FormClosingEventArgs e )
        {
            if ( this.addingLinks )
            {
                e.Cancel = true;
            }
        }
    }
}