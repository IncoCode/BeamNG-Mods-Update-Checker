using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Ini;

namespace BeamNGModsUpdateChecker
{
    public partial class frmMain : Form
    {
        public string lang = "en-GB";

        UpdateChecker upd;
        string login = "";
        string password = "";
        double[] lvColProp = { 0.6, 0.4 };

        public frmMain()
        {
            this.loadSettings();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo( lang );
            InitializeComponent();
        }

        public void changeLanguage( string lang )
        {
            this.lang = lang;
            this.saveSettings();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo( this.lang );
            DialogResult res = MessageBox.Show( strings.restartApp, strings.warning, MessageBoxButtons.YesNo, MessageBoxIcon.Warning );
            if ( res == DialogResult.Yes )
            {
                Application.Restart();
            }
        }

        public void setLoginPassword( string login, string password )
        {
            this.login = Crypto.EncryptPassword( login );
            this.password = Crypto.EncryptPassword( password );
        }

        private void printAllThreads()
        {
            lvThreads.Items.Clear();
            for ( int i = 0; i < this.upd.Threads.Count; i++ )
            {
                ListViewItem lvi = new ListViewItem( this.upd.Threads[ i ].Title );
                lvi.UseItemStyleForSubItems = false;
                ListViewItem.ListViewSubItem lvisi = new ListViewItem.ListViewSubItem();
                lvisi.Text = this.upd.Threads[ i ].Link;
                lvisi.ForeColor = Color.Blue;
                if ( !this.upd.Threads[ i ].Read )
                {
                    lvi.BackColor = Color.GreenYellow;
                    lvisi.BackColor = Color.GreenYellow;
                }
                lvi.SubItems.Add( lvisi );
                lvThreads.Items.Add( lvi );
            }
        }

        private void saveSettings()
        {
            IniFile ini = new IniFile( Application.StartupPath + @"\Settings.ini" );
            ini.Write( "Login", this.login, "Auth" );
            ini.Write( "Password", this.password, "Auth" );
            ini.Write( "Lang", lang, "Options" );
        }

        private void loadSettings()
        {
            IniFile ini = new IniFile( Application.StartupPath + @"\Settings.ini" );
            this.login = ini.Read( "Login", "Auth", "" );
            this.password = ini.Read( "Password", "Auth", "" );
            this.lang = ini.Read( "Lang", "Options", lang );
        }

        private void updProgress( object sender, UpdEventArgs e )
        {
            var lvi = lvThreads.FindItemWithText( e.thread.Link );
            if ( lvi != null )
            {
                lvi.Text = e.thread.Title;
                lvi.BackColor = Color.GreenYellow;
                lvi.SubItems[ 1 ].BackColor = Color.GreenYellow;
            }
        }

        private void checkUpdates()
        {
            int updatesCount = this.upd.checkUpdates();
            if ( updatesCount > 0 )
            {
                this.showUpdNot( updatesCount );
                this.upd.saveThreads();
            }
        }

        private void showUpdNot( int updatesCount )
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo( this.lang );
            niTray.Text = string.Format( strings.updatesCount, updatesCount );
            niTray.BalloonTipIcon = ToolTipIcon.Info;
            niTray.BalloonTipTitle = strings.updatesFound;
            niTray.BalloonTipText = string.Format( strings.updatesCount, updatesCount );
            niTray.ShowBalloonTip( 2500 );
        }

        private void lvColAutosize()
        {
            int allWidth = lvThreads.Width - 37;
            for ( int i = 0; i < this.lvColProp.Length; i++ )
            {
                int width = (int)( allWidth * this.lvColProp[ i ] );
                lvThreads.Columns[ i ].Width = width;
            }
        }

        private void frmMain_Load( object sender, EventArgs e )
        {
            if ( string.IsNullOrEmpty( this.login ) || string.IsNullOrEmpty( this.password ) )
            {
                frmEnterPassword frm = new frmEnterPassword( this );
                frm.ShowDialog();
            }
            this.upd = new UpdateChecker( this.login, this.password, Application.StartupPath );
            upd.updEvent += new EventHandler<UpdEventArgs>( updProgress );
            this.upd.loadThreads();
            this.printAllThreads();
            this.checkUpdates();
        }

        private void frmMain_FormClosing( object sender, FormClosingEventArgs e )
        {
            this.upd.saveThreads();
            this.saveSettings();
        }

        private void tsmiAddThreads_Click( object sender, EventArgs e )
        {
            frmAddLinks frm = new frmAddLinks( this.upd, this );
            frm.ShowDialog();
            this.printAllThreads();
            this.upd.saveThreads();
        }

        private void tsmiLAddThread_Click( object sender, EventArgs e )
        {
            tsmiAddThreads.PerformClick();
        }

        private void tsmiRemove_Click( object sender, EventArgs e )
        {
            if ( lvThreads.SelectedItems.Count > 0 )
            {
                for ( int i = 0; i < lvThreads.SelectedItems.Count; i++ )
                {
                    string link = lvThreads.SelectedItems[ i ].SubItems[ 1 ].Text;
                    this.upd.removeThread( link );
                }
                this.printAllThreads();
            }
        }

        private void lvThreads_DoubleClick( object sender, EventArgs e )
        {
            if ( lvThreads.SelectedItems.Count == 1 )
            {
                string link = lvThreads.SelectedItems[ 0 ].SubItems[ 1 ].Text;
                Process.Start( link );
                lvThreads.SelectedItems[ 0 ].BackColor = Color.White;
                lvThreads.SelectedItems[ 0 ].SubItems[ 1 ].BackColor = Color.White;
                this.upd.makeRead( link );
            }
        }

        private void tmrUpd_Tick( object sender, EventArgs e )
        {
            this.checkUpdates();
        }

        private void tsmiExit_Click( object sender, EventArgs e )
        {
            this.Close();
        }

        private void tsmiExpand_Click( object sender, EventArgs e )
        {
            this.Visible = true;
            this.ShowInTaskbar = true;
            this.WindowState = FormWindowState.Normal;
        }

        private void frmMain_Resize( object sender, EventArgs e )
        {
            this.lvColAutosize();
            if ( WindowState == FormWindowState.Minimized )
            {
                this.Visible = false;
                this.ShowInTaskbar = false;
            }
        }

        private void tsmiMakeRead_Click( object sender, EventArgs e )
        {
            if ( lvThreads.SelectedItems.Count > 0 )
            {
                for ( int i = 0; i < lvThreads.SelectedItems.Count; i++ )
                {
                    string link = lvThreads.SelectedItems[ i ].SubItems[ 1 ].Text;
                    lvThreads.SelectedItems[ i ].BackColor = Color.White;
                    lvThreads.SelectedItems[ i ].SubItems[ 1 ].BackColor = Color.White;
                    this.upd.makeRead( link );
                }                
            }
        }

        private void niTray_DoubleClick( object sender, EventArgs e )
        {
            tsmiExpand.PerformClick();
        }

        private void englishToolStripMenuItem_Click( object sender, EventArgs e )
        {
            this.changeLanguage( "en-GB" );
        }

        private void русскийToolStripMenuItem_Click( object sender, EventArgs e )
        {
            this.changeLanguage( "ru-RU" );
        }
    }
}
