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
    public partial class frmAddLinks : Form
    {
        UpdateChecker upd;        

        public frmAddLinks( UpdateChecker upd, frmMain MainForm )
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo( MainForm.lang );
            InitializeComponent();
            this.upd = upd;
        }

        private void btnAdd_Click( object sender, EventArgs e )
        {
            string[] links = tbLinks.Lines;
            for ( int i = 0; i < links.Length; i++ )
            {
                string link = links[ i ];
                this.upd.addThread( link );
                Thread.Sleep( 50 );
            }
            this.Close();
        }
    }
}
