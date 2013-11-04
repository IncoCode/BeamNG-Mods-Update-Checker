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
        bool isUpdating = false;
        Size mainFormSize = new Size( 748, 456 );

        public frmMain()
        {
            this.loadSettings();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo( lang );
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            this.Size = this.mainFormSize;
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

        private void printAllThreads( List<Topic> threads = null )
        {
            lvThreads.Items.Clear();
            if ( threads == null )
            {
                threads = this.upd.Threads;
            }
            for ( int i = 0; i < threads.Count; i++ )
            {
                ListViewItem lvi = new ListViewItem( threads[ i ].Title );
                lvi.UseItemStyleForSubItems = false;
                ListViewItem.ListViewSubItem lvisi = new ListViewItem.ListViewSubItem();
                lvisi.Text = threads[ i ].Link;
                lvisi.ForeColor = Color.Blue;
                if ( !threads[ i ].Read )
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
            try
            {
                IniFile ini = new IniFile( Application.StartupPath + @"\Settings.ini" );
                ini.Write( "Login", this.login, "Auth" );
                ini.Write( "Password", this.password, "Auth" );
                ini.Write( "Lang", lang, "Options" );
                ini.Write( "UpdInterval", this.updInterval.ToString(), "Options" );
                ini.Write( "MinimizeWhenStart", this.minimizeWhenStart.ToString(), "Options" );
                ini.Write( "MainFormWidth", this.Size.Width.ToString(), "Options" );
                ini.Write( "MainFormHeight", this.Size.Height.ToString(), "Options" );
            }
            catch
            {
                return;
            }
        }

        private void loadSettings()
        {
            try
            {
                IniFile ini = new IniFile( Application.StartupPath + @"\Settings.ini" );
                this.login = ini.Read( "Login", "Auth", "" );
                this.password = ini.Read( "Password", "Auth", "" );
                this.lang = ini.Read( "Lang", "Options", lang );
                this.updInterval = int.Parse( ini.Read( "UpdInterval", "Options", this.updInterval.ToString() ) );
                this.minimizeWhenStart = bool.Parse( ini.Read( "MinimizeWhenStart", "Options", this.minimizeWhenStart.ToString() ) );
                int mainFormWidth = int.Parse( ini.Read( "MainFormWidth", "Options", this.mainFormSize.Width.ToString() ) );
                int mainFormHeight = int.Parse( ini.Read( "MainFormHeight", "Options", this.mainFormSize.Height.ToString() ) );
                Size mainFormSize = new Size( mainFormWidth, mainFormHeight );
                this.mainFormSize = mainFormSize;
            }
            catch
            {
                return;
            }
        }

        private void saveThreads()
        {
            try
            {
                this.upd.saveThreads();
            }
            catch
            {
                MessageBox.Show( "Ошибка сохранения!", strings.error, MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }
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
            if ( this.isUpdating )
            {
                return;
            }
            this.isUpdating = true;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo( lang );
            this.Invoke( new MethodInvoker( delegate()
                {
                    pbCheckUpd.Visible = true;
                    pbCheckUpd.Value = 0;
                } ) );
            lvThreads.Enabled = false;
            tbKeyword.Enabled = false;
            ssStatus.Items[ 0 ].Text = strings.checkingForUpdates;
            try
            {
                int updatesCount = this.upd.checkUpdates();
                this.showUpdNot( updatesCount );
                this.saveThreads();
            }
            finally
            {
                lvThreads.Enabled = true;
                tbKeyword.Enabled = true;
                this.isUpdating = false;
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
                DialogResult dr = frm.ShowDialog();
                if ( dr == DialogResult.Cancel )
                {
                    Environment.Exit( 0 );
                    return;
                }
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
            try
            {
                this.upd.loadThreads();
            }
            catch
            {
                MessageBox.Show( strings.loadThreadsError, strings.error, MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
            this.printAllThreads();
            tmrUpd.Start();
            if ( this.minimizeWhenStart )
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void frmMain_FormClosing( object sender, FormClosingEventArgs e )
        {
            this.saveThreads();
            this.saveSettings();
        }

        private void tsmiAddThreads_Click( object sender, EventArgs e )
        {
            frmAddLinks frm = new frmAddLinks( this.upd, this );
            frm.ShowDialog();
            this.printAllThreads();
            this.saveThreads();
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
                this.showUpdNot( this.upd.getUnreadThreads(), false );
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
                }
                this.showUpdNot( this.upd.getUnreadThreads(), false );
                this.saveThreads();
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
            this.saveThreads();
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
                }
                this.showUpdNot( this.upd.getUnreadThreads(), false );
                this.saveThreads();
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

        private void tbKeyword_TextChanged( object sender, EventArgs e )
        {
            string keyword = tbKeyword.Text;
            this.printAllThreads( this.upd.searchThreads( keyword ) );
        }

        private void tsmiOfficialThread_Click( object sender, EventArgs e )
        {
            Process.Start( "http://www.beamng.com/threads/4920-Mods-Update-Checker?p=54900#post54900" );
        }
    }
}
