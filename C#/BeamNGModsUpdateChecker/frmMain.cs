#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Ini;

#endregion

namespace BeamNGModsUpdateChecker
{
    public partial class FrmMain : Form
    {
        public string Lang = "en-GB";
        public int UpdInterval = 30;
        public bool MinimizeWhenStart = false;

        private UpdateChecker _upd;
        private string _login = "";
        private string _password = "";
        private readonly double[] _lvColProp = { 0.6, 0.4 };
        private bool _isUpdating = false;
        private Size _mainFormSize = new Size( 748, 456 );
        private Thread _updThread = null;
        private bool _showOnlyUpdated = false;

        public FrmMain()
        {
            this.LoadSettings();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo( this.Lang );
            InitializeComponent();
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            this.Size = this._mainFormSize;
        }

        public void ChangeLanguage( string lang )
        {
            this.Lang = lang;
            this.SaveSettings();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo( this.Lang );
            DialogResult res = MessageBox.Show( strings.restartApp, strings.warning, MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning );
            if ( res == DialogResult.Yes )
            {
                Application.Restart();
            }
        }

        public void SetLoginPassword( string login, string password )
        {
            this._login = Crypto.EncryptPassword( login );
            this._password = Crypto.EncryptPassword( password );
        }

        private void PrintAllThreads( List<Topic> threads = null )
        {
            lvThreads.Items.Clear();
            if ( threads == null )
            {
                threads = this._upd.Threads;
                this._showOnlyUpdated = false;
            }
            for ( int i = 0; i < threads.Count; i++ )
            {
                var lvi = new ListViewItem( threads[ i ].Title );
                lvi.UseItemStyleForSubItems = false;
                var lvisi = new ListViewItem.ListViewSubItem();
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

        private void SaveSettings()
        {
            try
            {
                var ini = new IniFile( Application.StartupPath + @"\Settings.ini" );
                ini.Write( "Login", this._login, "Auth" );
                ini.Write( "Password", this._password, "Auth" );
                ini.Write( "Lang", this.Lang, "Options" );
                ini.Write( "UpdInterval", this.UpdInterval.ToString(), "Options" );
                ini.Write( "MinimizeWhenStart", this.MinimizeWhenStart.ToString(), "Options" );
                if ( WindowState != FormWindowState.Minimized )
                {
                    ini.Write( "MainFormWidth", this.Size.Width.ToString(), "Options" );
                    ini.Write( "MainFormHeight", this.Size.Height.ToString(), "Options" );
                }
            }
            catch
            {
            }
        }

        private void LoadSettings()
        {
            try
            {
                var ini = new IniFile( Application.StartupPath + @"\Settings.ini" );
                this._login = ini.Read( "Login", "Auth", "" );
                this._password = ini.Read( "Password", "Auth", "" );
                this.Lang = ini.Read( "Lang", "Options", this.Lang );
                this.UpdInterval = int.Parse( ini.Read( "UpdInterval", "Options", this.UpdInterval.ToString() ) );
                this.MinimizeWhenStart =
                    bool.Parse( ini.Read( "MinimizeWhenStart", "Options", this.MinimizeWhenStart.ToString() ) );
                int mainFormWidth =
                    int.Parse( ini.Read( "MainFormWidth", "Options", this._mainFormSize.Width.ToString() ) );
                int mainFormHeight =
                    int.Parse( ini.Read( "MainFormHeight", "Options", this._mainFormSize.Height.ToString() ) );
                var mainFormSize = new Size( mainFormWidth, mainFormHeight );
                this._mainFormSize = mainFormSize;
            }
            catch
            {
                return;
            }
        }

        private void SaveThreads()
        {
            try
            {
                this._upd.SaveThreads();
            }
            catch
            {
                MessageBox.Show( strings.saveThreadsError, strings.error, MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }

        private void UpdProgress( object sender, UpdEventArgs e )
        {
            ListViewItem lvi = lvThreads.FindItemWithText( e.Thread.Link );
            if ( lvi != null )
            {
                lvi.Text = e.Thread.Title;
                lvi.BackColor = Color.GreenYellow;
                lvi.SubItems[ 1 ].BackColor = Color.GreenYellow;
            }
        }

        private void CheckUpdates()
        {
            if ( this._isUpdating )
            {
                return;
            }
            this._isUpdating = true;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo( this.Lang );
            this.Invoke( new MethodInvoker( delegate()
            {
                pbCheckUpd.Visible = true;
                pbCheckUpd.Value = 0;
            } ) );
            lvThreads.Enabled = false;
            tbKeyword.Enabled = false;
            ssStatus.Items[ 0 ].Text = strings.checkingForUpdates;
            niTray.Text = strings.checkingForUpdates;
            try
            {
                int updatesCount = this._upd.CheckUpdates();
                this.ShowUpdNot( updatesCount );
                this.SaveThreads();
            }
            finally
            {
                lvThreads.Enabled = true;
                tbKeyword.Enabled = true;
                this._isUpdating = false;
                try
                {
                    this.Invoke( new MethodInvoker( delegate()
                    {
                        pbCheckUpd.Visible = false;
                        tmrUpdProgress.Stop();
                    } ) );
                }
                catch
                {
                }
            }
        }

        private void ShowUpdNot( int updatesCount, bool showBalloon = true )
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo( this.Lang );
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

        private void LvColAutosize()
        {
            int allWidth = lvThreads.Width - 37;
            for ( int i = 0; i < this._lvColProp.Length; i++ )
            {
                int width = (int)( allWidth * this._lvColProp[ i ] );
                lvThreads.Columns[ i ].Width = width;
            }
        }

        private void frmMain_Load( object sender, EventArgs e )
        {
            FrmEnterPassword frm = null;
            if ( string.IsNullOrEmpty( this._login ) || string.IsNullOrEmpty( this._password ) )
            {
                frm = new FrmEnterPassword( this );
                DialogResult dr = frm.ShowDialog();
                if ( dr == DialogResult.Cancel )
                {
                    niTray.Dispose();
                    Environment.Exit( 0 );
                    return;
                }
            }
            this._upd = new UpdateChecker( this._login, this._password, Application.StartupPath );
            try
            {
                bool isAuth = this._upd.Auth();
                while ( !isAuth )
                {
                    frm = new FrmEnterPassword( this );
                    DialogResult dr = frm.ShowDialog();
                    if ( dr == DialogResult.Cancel )
                    {
                        niTray.Dispose();
                        Environment.Exit( 0 );
                        return;
                    }
                    isAuth = this._upd.Auth( this._login, this._password );
                }
            }
            catch
            {
                MessageBox.Show( strings.unableSendRequest, strings.error, MessageBoxButtons.OK, MessageBoxIcon.Error );
                Environment.Exit( 0 );
            }
            this._upd.UpdEvent += new EventHandler<UpdEventArgs>( this.UpdProgress );
            try
            {
                this._upd.LoadThreads();
            }
            catch
            {
                MessageBox.Show( strings.loadThreadsError, strings.error, MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
            this.PrintAllThreads();
            tmrUpd.Start();
            if ( this.MinimizeWhenStart )
            {
                this.WindowState = FormWindowState.Minimized;
            }
        }

        private void frmMain_FormClosing( object sender, FormClosingEventArgs e )
        {
            this.SaveThreads();
            this.SaveSettings();
            niTray.Dispose();
            if ( this._updThread != null )
            {
                try
                {
                    this._updThread.Abort();
                }
                catch
                {
                    Environment.Exit( 0 );
                }
            }
        }

        private void tsmiAddThreads_Click( object sender, EventArgs e )
        {
            if ( this._isUpdating )
            {
                return;
            }
            var frm = new FrmAddLinks( this._upd, this );
            frm.ShowDialog();
            this.PrintAllThreads();
            this.SaveThreads();
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
                    this._upd.RemoveThread( link );
                }
                this.PrintAllThreads();
                this.ShowUpdNot( this._upd.GetUnreadThreads(), false );
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
                this._upd.ChangeReadStatus( link, true );
                this.ShowUpdNot( this._upd.GetUnreadThreads(), false );
            }
        }

        private void tmrUpd_Tick( object sender, EventArgs e )
        {
            tmrUpdProgress.Start();
            tmrUpd.Interval = this.UpdInterval * 60 * 1000;
            this._updThread = new Thread( this.CheckUpdates );
            this._updThread.Start();
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
            this.Size = this._mainFormSize;
        }

        private void frmMain_Resize( object sender, EventArgs e )
        {
            this.LvColAutosize();
            this._mainFormSize = this.Size;
            this.SaveSettings();
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
                    this._upd.ChangeReadStatus( link, true );
                }
                this.ShowUpdNot( this._upd.GetUnreadThreads(), false );
                this.SaveThreads();
            }
        }

        private void niTray_DoubleClick( object sender, EventArgs e )
        {
            tsmiExpand.PerformClick();
        }

        private void tsmiEnglish_Click( object sender, EventArgs e )
        {
            this.ChangeLanguage( "en-GB" );
        }

        private void tsmiRussian_Click( object sender, EventArgs e )
        {
            this.ChangeLanguage( "ru-RU" );
        }

        private void tsmiOptions_Click( object sender, EventArgs e )
        {
            var frm = new FrmOptions( this );
            frm.ShowDialog();
        }

        private void tsmiMarkAllRead_Click( object sender, EventArgs e )
        {
            if ( this._isUpdating )
            {
                return;
            }
            for ( int i = 0; i < this._upd.Threads.Count; i++ )
            {
                string link = this._upd.Threads[ i ].Link;
                lvThreads.Items[ i ].BackColor = Color.White;
                lvThreads.Items[ i ].SubItems[ 1 ].BackColor = Color.White;
                this._upd.ChangeReadStatus( link, true );
            }
            this.SaveThreads();
            this.ShowUpdNot( this._upd.GetUnreadThreads(), false );
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
                    this._upd.ChangeReadStatus( link, false );
                }
                this.ShowUpdNot( this._upd.GetUnreadThreads(), false );
                this.SaveThreads();
            }
        }

        private void tsmiRefresh_Click( object sender, EventArgs e )
        {
            tmrUpd.Stop();
            tmrUpdProgress.Start();
            this._updThread = new Thread( this.CheckUpdates );
            this._updThread.Start();
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
            this.PrintAllThreads( this._upd.SearchThreads( keyword ) );
        }

        private void tsmiOfficialThread_Click( object sender, EventArgs e )
        {
            Process.Start( "http://www.beamng.com/threads/4920-Mods-Update-Checker?p=54900#post54900" );
        }

        private void tsmiAbout_Click( object sender, EventArgs e )
        {
            MessageBox.Show( strings.copyright.FixNewLines(), strings.aboutProg, MessageBoxButtons.OK,
                MessageBoxIcon.Information );
        }

        private void tmrUpdProgress_Tick( object sender, EventArgs e )
        {
            pbCheckUpd.Maximum = this._upd.UpdMaxProgress;
            pbCheckUpd.Value = this._upd.UpdProgress;
        }

        private void tsmiRemoveDuplicates_Click( object sender, EventArgs e )
        {
            this._upd.RemoveDuplicates();
            this.PrintAllThreads();
        }

        private void tsmiRefreshT_Click( object sender, EventArgs e )
        {
            tsmiRefresh.PerformClick();
        }

        private void lblOnlyUnread_Click( object sender, EventArgs e )
        {
            if ( this._isUpdating )
            {
                return;
            }
            this._showOnlyUpdated = !this._showOnlyUpdated;
            if ( this._showOnlyUpdated )
            {
                this.PrintAllThreads( this._upd.GetOnlyUpdatedThreads() );
            }
            else
            {
                this.PrintAllThreads();
            }
        }
    }

    public static class ExMethods
    {
        public static string FixNewLines( this string str )
        {
            return str.Replace( @"\n", Environment.NewLine );
        }
    }
}