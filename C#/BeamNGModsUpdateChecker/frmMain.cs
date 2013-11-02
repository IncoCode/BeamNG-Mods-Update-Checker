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
        public int updInterval = 30;
        public bool minimizeWhenStart = false;

        UpdateChecker upd;
        string login = "";
        string password = "";
        double[] lvColProp = { 0.6, 0.4 };

        public frmMain()
        {
            this.loadSettings();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo( lang );
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
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
            ini.Write( "UpdInterval", this.updInterval.ToString(), "Options" );
            ini.Write( "MinimizeWhenStart", this.minimizeWhenStart.ToString(), "Options" );
        }

        private void loadSettings()
        {
            IniFile ini = new IniFile( Application.StartupPath + @"\Settings.ini" );
            this.login = ini.Read( "Login", "Auth", "" );
            this.password = ini.Read( "Password", "Auth", "" );
            this.lang = ini.Read( "Lang", "Options", lang );
            this.updInterval = int.Parse( ini.Read( "UpdInterval", "Options", this.updInterval.ToString() ) );
            this.minimizeWhenStart = bool.Parse( ini.Read( "MinimizeWhenStart", "Options", this.minimizeWhenStart.ToString() ) );
        }

        private void updProgress( object sender, UpdEventArgs e )
        {
            ListViewItem lvi = lvThreads.FindItemWithText( e.thread.Link );
            if ( lvi != null )
            {
                lvi.Text = e.thread.Title;
                lvi.BackColor = Color.GreenYellow;
                lvi.SubItems[ 1 ].BackColor = Color.GreenYellow;
            }
        }

        private void checkUpdProgress( object sender, CheckUpdEventArgs e )
        {
            pbCheckUpd.Maximum = e.maxProgress;
            pbCheckUpd.Value = e.progress;
        }

        private void checkUpdates()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo( lang );
            this.Invoke( new MethodInvoker( delegate()
                {
                    pbCheckUpd.Visible = true;
                } ) );
            ssStatus.Items[ 0 ].Text = strings.checkingForUpdates;
            Application.DoEvents();
            try
            {
                int updatesCount = this.upd.checkUpdates();
                this.showUpdNot( updatesCount );
                this.upd.saveThreads();
            }
            finally
            {
                lvThreads.Enabled = true;
                this.Invoke( new MethodInvoker( delegate()
                {
                    pbCheckUpd.Visible = false;
                } ) );
            }
        }

        private void showUpdNot( int updatesCount, bool showBalloon = true )
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo( this.lang );
            niTray.Text = string.Format( strings.updatesCount, updatesCount );
            ssStatus.Items[ 0 ].Text = string.Format( strings.updatesCount, updatesCount );
            if ( updatesCount > 0 && showBalloon )
            {
                niTray.BalloonTipIcon = ToolTipIcon.Info;
                niTray.BalloonTipTitle = strings.updatesFound;
                niTray.BalloonTipText = string.Format( strings.updatesCount, updatesCount );
                niTray.ShowBalloonTip( 2500 );
            }
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
            frmEnterPassword frm = null;
            if ( string.IsNullOrEmpty( this.login ) || string.IsNullOrEmpty( this.password ) )
            {
                frm = new frmEnterPassword( this );
                frm.ShowDialog();
            }
            this.upd = new UpdateChecker( this.login, this.password, Application.StartupPath );
            try
            {
                bool isAuth = this.upd.auth();
                while ( !isAuth )
                {
                    frm = new frmEnterPassword( this );
                    frm.ShowDialog();
                    isAuth = this.upd.auth( this.login, this.password );
                }
            }
            catch
            {
                MessageBox.Show( "Unable to send a request!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                Environment.Exit( 0 );
            }
            upd.updEvent += new EventHandler<UpdEventArgs>( updProgress );
            upd.checkUpdEvent += new EventHandler<CheckUpdEventArgs>( checkUpdProgress );
            this.upd.loadThreads();
            this.printAllThreads();
            tmrUpd.Start();
            if ( this.minimizeWhenStart )
            {
                this.WindowState = FormWindowState.Minimized;
            }
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
                this.upd.changeReadStatus( link, true );
                this.showUpdNot( this.upd.getUnreadThreads(), false );
            }
        }

        private void tmrUpd_Tick( object sender, EventArgs e )
        {
            tmrUpd.Interval = this.updInterval * 60 * 1000;
            lvThreads.Enabled = false;
            Thread thr = new Thread( this.checkUpdates );
            thr.Start();
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
                    this.upd.changeReadStatus( link, true );
                    this.showUpdNot( this.upd.getUnreadThreads(), false );
                }
                this.upd.saveThreads();
            }
        }

        private void niTray_DoubleClick( object sender, EventArgs e )
        {
            tsmiExpand.PerformClick();
        }

        private void tsmiEnglish_Click( object sender, EventArgs e )
        {
            this.changeLanguage( "en-GB" );
        }

        private void tsmiRussian_Click( object sender, EventArgs e )
        {
            this.changeLanguage( "ru-RU" );
        }

        private void tsmiOptions_Click( object sender, EventArgs e )
        {
            frmOptions frm = new frmOptions( this );
            frm.ShowDialog();
        }

        private void tsmiMarkAllRead_Click( object sender, EventArgs e )
        {
            for ( int i = 0; i < this.upd.Threads.Count; i++ )
            {
                string link = this.upd.Threads[ i ].Link;
                lvThreads.Items[ i ].BackColor = Color.White;
                lvThreads.Items[ i ].SubItems[ 1 ].BackColor = Color.White;
                this.upd.changeReadStatus( link, true );
            }
            this.upd.saveThreads();
            this.showUpdNot( this.upd.getUnreadThreads(), false );
        }

        private void tsmiMakeUnread_Click( object sender, EventArgs e )
        {
            if ( lvThreads.SelectedItems.Count > 0 )
            {
                for ( int i = 0; i < lvThreads.SelectedItems.Count; i++ )
                {
                    string link = lvThreads.SelectedItems[ i ].SubItems[ 1 ].Text;
                    lvThreads.SelectedItems[ i ].BackColor = Color.GreenYellow;
                    lvThreads.SelectedItems[ i ].SubItems[ 1 ].BackColor = Color.GreenYellow;
                    this.upd.changeReadStatus( link, false );
                    this.showUpdNot( this.upd.getUnreadThreads(), false );
                }
                this.upd.saveThreads();
            }
        }

        private void tsmiRefresh_Click( object sender, EventArgs e )
        {
            tmrUpd.Stop();
            Thread thr = new Thread( this.checkUpdates );
            thr.Start();
            tmrUpd.Start();
        }

        private void tsmiAddT_Click( object sender, EventArgs e )
        {
            tsmiAddThreads.PerformClick();
        }

        private void tsmiRepository_Click( object sender, EventArgs e )
        {
            Process.Start( "https://bitbucket.org/IncoCode/beamng-mods-update-checker/overview" );
        }
    }
}
