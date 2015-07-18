#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using BeamNGModsUpdateChecker.Properties;

#endregion

namespace BeamNGModsUpdateChecker
{
    public partial class FrmMain : Form
    {
        private readonly UpdateChecker _updateChecker;
        private readonly double[] _lvColProp = { 0.6, 0.4 };
        private bool _isUpdating;
        private Thread _updThread;
        private readonly Settings _settings = Settings.Default;

        [System.Runtime.InteropServices.DllImport( "user32.dll" )]
        private static extern IntPtr SendMessage( IntPtr hWnd, int msg, IntPtr wp, IntPtr lp );

        public FrmMain()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo( this._settings.Lang );
            this.InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            this.Size = new Size( this._settings.MainFormWidth, this._settings.MainFormHeight );
            this._updateChecker = new UpdateChecker( Application.StartupPath );
        }

        private void ChangeLanguage( string lang )
        {
            this._settings.Lang = lang;
            this.SaveSettings();
            Thread.CurrentThread.CurrentUICulture = new CultureInfo( this._settings.Lang );
            DialogResult res = MessageBox.Show( strings.restartApp, strings.warning, MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning );
            if ( res == DialogResult.Yes )
            {
                Application.Restart();
            }
        }

        private void PrintAllThreads( List<Topic> threads = null )
        {
            this.lvThreads.Items.Clear();
            if ( threads == null )
            {
                threads = this._updateChecker.Threads;
            }
            foreach ( Topic thread in threads )
            {
                var lvi = new ListViewItem( thread.Title );
                lvi.UseItemStyleForSubItems = false;
                var lvisi = new ListViewItem.ListViewSubItem();
                lvisi.Text = thread.Link;
                lvisi.ForeColor = Color.Blue;
                if ( !thread.Read )
                {
                    lvi.BackColor = Color.GreenYellow;
                    lvisi.BackColor = Color.GreenYellow;
                }
                lvi.SubItems.Add( lvisi );
                this.lvThreads.Items.Add( lvi );
            }
        }

        private void SaveSettings()
        {
            this._settings.MainFormWidth = this.Size.Width;
            this._settings.MainFormHeight = this.Size.Height;
            this._settings.Save();
        }

        private void SaveThreads()
        {
            try
            {
                this._updateChecker.SaveThreads();
            }
            catch
            {
                MessageBox.Show( strings.saveThreadsError, strings.error, MessageBoxButtons.OK, MessageBoxIcon.Error );
            }
        }

        private void CheckUpdatesProgramRun()
        {
            this.CheckUpdates( true );
        }

        private void CheckUpdates()
        {
            this.CheckUpdates( false );
        }

        private void CheckUpdates( bool programRun )
        {
            if ( this._isUpdating )
            {
                return;
            }
            this._isUpdating = true;
            Thread.CurrentThread.CurrentUICulture = new CultureInfo( this._settings.Lang );
            this.Invoke( new MethodInvoker( delegate()
            {
                this.pbCheckUpd.Visible = true;
                this.pbCheckUpd.Value = 0;
            } ) );
            this.lvThreads.Enabled = false;
            this.tbKeyword.Enabled = false;
            this.ssStatus.Items[ 0 ].Text = strings.checkingForUpdates;
            this.niTray.Text = strings.checkingForUpdates;
            if ( programRun )
            {
                Thread.Sleep( this._settings.StartupDelayBeforeCheck * 1000 );
            }
            try
            {
                int updatesCount = this._updateChecker.CheckUpdates();
                this.ShowUpdNot( updatesCount );
                this.SaveThreads();
            }
            finally
            {
                this.lvThreads.Enabled = true;
                this.tbKeyword.Enabled = true;
                this._isUpdating = false;
                try
                {
                    this.Invoke( new MethodInvoker( delegate()
                    {
                        this.pbCheckUpd.Visible = false;
                        this.tmrUpdProgress.Stop();
                        this.PrintAllThreads();
                    } ) );
                }
                catch
                {
                }
                GC.Collect();
            }
        }

        private void ShowUpdNot( int updatesCount, bool showBalloon = true )
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo( this._settings.Lang );
            this.niTray.Text = string.Format( strings.updatesCount, updatesCount );
            this.ssStatus.Items[ 0 ].Text = string.Format( strings.updatesCount, updatesCount );
            if ( updatesCount > 0 && showBalloon )
            {
                this.niTray.BalloonTipIcon = ToolTipIcon.Info;
                this.niTray.BalloonTipTitle = strings.updatesFound;
                this.niTray.BalloonTipText = string.Format( strings.updatesCount, updatesCount );
                this.niTray.ShowBalloonTip( 2500 );
            }
        }

        private void LvColAutosize()
        {
            int allWidth = this.lvThreads.Width - 37;
            for ( int i = 0; i < this._lvColProp.Length; i++ )
            {
                int width = (int)( allWidth * this._lvColProp[ i ] );
                this.lvThreads.Columns[ i ].Width = width;
            }
        }

        private void frmMain_Load( object sender, EventArgs e )
        {
            this._updateChecker.LoadThreads();

            //try
            //{
            //    this._upd.LoadThreads();
            //}
            //catch
            //{
            //    MessageBox.Show( strings.loadThreadsError, strings.error, MessageBoxButtons.OK, MessageBoxIcon.Error );
            //}
            if ( this._settings.ShowOnlyUnread )
            {
                this.lblOnlyUnread_Click( this.lblOnlyUnread, EventArgs.Empty );
            }
            this.PrintAllThreads();
            this.tmrUpd.Start();
            if ( this._settings.MinimizeOnStart )
            {
                this.WindowState = FormWindowState.Minimized;
            }

            var btn = new Button { Size = new Size( 25, this.tbKeyword.ClientSize.Height + 2 ) };
            btn.Location = new Point( this.tbKeyword.ClientSize.Width - btn.Width, -1 );
            btn.Cursor = Cursors.Hand;
            btn.BackColor = this.BackColor;
            btn.Text = "X";
            btn.Click += this.btnClear_Click;
            this.tbKeyword.Controls.Add( btn );
            SendMessage( this.tbKeyword.Handle, 0xd3, (IntPtr)2, (IntPtr)( btn.Width << 16 ) );
        }

        private void btnClear_Click( object sender, EventArgs e )
        {
            this.tbKeyword.Text = "";
        }

        private void frmMain_FormClosing( object sender, FormClosingEventArgs e )
        {
            this.SaveThreads();
            this.SaveSettings();
            this.niTray.Dispose();
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
            var frm = new FrmAddLinks( this._updateChecker, this );
            frm.ShowDialog();
            this.PrintAllThreads();
            this.SaveThreads();
        }

        private void tsmiLAddThread_Click( object sender, EventArgs e )
        {
            this.tsmiAddThreads.PerformClick();
        }

        private void tsmiRemove_Click( object sender, EventArgs e )
        {
            if ( this.lvThreads.SelectedItems.Count > 0 )
            {
                for ( int i = 0; i < this.lvThreads.SelectedItems.Count; i++ )
                {
                    string link = this.lvThreads.SelectedItems[ i ].SubItems[ 1 ].Text;
                    this._updateChecker.RemoveThread( link );
                }
                this.PrintAllThreads();
                this.ShowUpdNot( this._updateChecker.UnreadThreadsCount, false );
            }
        }

        private void lvThreads_DoubleClick( object sender, EventArgs e )
        {
            if ( this.lvThreads.SelectedItems.Count == 1 )
            {
                string link = this.lvThreads.SelectedItems[ 0 ].SubItems[ 1 ].Text;
                Process.Start( link );
                this.lvThreads.SelectedItems[ 0 ].BackColor = Color.White;
                this.lvThreads.SelectedItems[ 0 ].SubItems[ 1 ].BackColor = Color.White;
                this._updateChecker.ChangeReadStatus( link, true );
                if ( this._updateChecker.ThreadFilter.ShowOnlyUnread )
                {
                    this.lvThreads.Items.Remove( this.lvThreads.SelectedItems[ 0 ] );
                }
                this.lvThreads.Refresh();
                this.ShowUpdNot( this._updateChecker.UnreadThreadsCount, false );
            }
        }

        private void tmrUpd_Tick( object sender, EventArgs e )
        {
            bool programRun = this.tmrUpd.Interval == 1000;
            this.tmrUpdProgress.Start();
            this.tmrUpd.Interval = this._settings.UpdInterval * 60 * 1000;
            this._updThread = programRun ? new Thread( this.CheckUpdatesProgramRun ) : new Thread( this.CheckUpdates );
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
            this.Size = new Size( this._settings.MainFormWidth, this._settings.MainFormHeight );
        }

        private void frmMain_Resize( object sender, EventArgs e )
        {
            this.LvColAutosize();
            this._settings.MainFormWidth = this.Size.Width;
            this._settings.MainFormHeight = this.Size.Height;
            this.SaveSettings();
            this.SaveSettings();
            if ( this.WindowState == FormWindowState.Minimized )
            {
                this.Visible = false;
                this.ShowInTaskbar = false;
            }
        }

        private void tsmiMakeRead_Click( object sender, EventArgs e )
        {
            if ( this.lvThreads.SelectedItems.Count > 0 )
            {
                for ( int i = 0; i < this.lvThreads.SelectedItems.Count; i++ )
                {
                    string link = this.lvThreads.SelectedItems[ i ].SubItems[ 1 ].Text;
                    this._updateChecker.ChangeReadStatus( link, true );
                }
                this.PrintAllThreads();
                this.ShowUpdNot( this._updateChecker.UnreadThreadsCount, false );
                this.SaveThreads();
            }
        }

        private void niTray_DoubleClick( object sender, EventArgs e )
        {
            this.tsmiExpand.PerformClick();
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
            var frm = new FrmOptions();
            frm.ShowDialog();
        }

        private void tsmiMarkAllRead_Click( object sender, EventArgs e )
        {
            if ( this._isUpdating )
            {
                return;
            }
            foreach ( Topic t in this._updateChecker.Threads )
            {
                string link = t.Link;
                this._updateChecker.ChangeReadStatus( link, true );
            }
            this.PrintAllThreads();
            this.SaveThreads();
            this.ShowUpdNot( this._updateChecker.UnreadThreadsCount, false );
        }

        private void tsmiMakeUnread_Click( object sender, EventArgs e )
        {
            if ( this.lvThreads.SelectedItems.Count > 0 )
            {
                for ( int i = 0; i < this.lvThreads.SelectedItems.Count; i++ )
                {
                    string link = this.lvThreads.SelectedItems[ i ].SubItems[ 1 ].Text;
                    this.lvThreads.SelectedItems[ i ].BackColor = Color.GreenYellow;
                    this.lvThreads.SelectedItems[ i ].SubItems[ 1 ].BackColor = Color.GreenYellow;
                    this._updateChecker.ChangeReadStatus( link, false );
                }
                this.ShowUpdNot( this._updateChecker.UnreadThreadsCount, false );
                this.SaveThreads();
            }
        }

        private void tsmiRefresh_Click( object sender, EventArgs e )
        {
            this.tmrUpd.Stop();
            this.tmrUpdProgress.Start();
            this._updThread = new Thread( this.CheckUpdates );
            this._updThread.Start();
            this.tmrUpd.Start();
        }

        private void tsmiAddT_Click( object sender, EventArgs e )
        {
            this.tsmiAddThreads.PerformClick();
        }

        private void tsmiRepository_Click( object sender, EventArgs e )
        {
            Process.Start( "https://bitbucket.org/IncoCode/beamng-mods-update-checker/overview" );
        }

        private void tbKeyword_TextChanged( object sender, EventArgs e )
        {
            string keyword = this.tbKeyword.Text;
            this._updateChecker.ThreadFilter.SearchKeyword = keyword;
            this.PrintAllThreads();
        }

        private void tsmiAbout_Click( object sender, EventArgs e )
        {
            MessageBox.Show( strings.copyright.FixNewLines(), strings.aboutProg, MessageBoxButtons.OK,
                MessageBoxIcon.Information );
        }

        private void tmrUpdProgress_Tick( object sender, EventArgs e )
        {
            this.pbCheckUpd.Maximum = this._updateChecker.UpdMaxProgress;
            this.pbCheckUpd.Value = this._updateChecker.UpdProgress;
        }

        private void tsmiRemoveDuplicates_Click( object sender, EventArgs e )
        {
            this._updateChecker.RemoveDuplicates();
            this.PrintAllThreads();
        }

        private void tsmiRefreshT_Click( object sender, EventArgs e )
        {
            this.tsmiRefresh.PerformClick();
        }

        private void lblOnlyUnread_Click( object sender, EventArgs e )
        {
            if ( this._isUpdating )
            {
                return;
            }
            this._updateChecker.ThreadFilter.ShowOnlyUnread = !this._updateChecker.ThreadFilter.ShowOnlyUnread;
            this.PrintAllThreads();
            this.lblOnlyUnread.Text = this._updateChecker.ThreadFilter.ShowOnlyUnread
                ? strings.showAll
                : strings.showOnlyUnread;
        }

        private void tsmiOpenAllUnread_Click( object sender, EventArgs e )
        {
            if ( this._isUpdating )
            {
                return;
            }
            List<Topic> threads = this._updateChecker.UnreadThreads;
            foreach ( var thread in threads )
            {
                Process.Start( thread.Link );
                thread.Read = true;
                Thread.Sleep( 150 );
            }
            this.PrintAllThreads();
            this.ShowUpdNot( this._updateChecker.UnreadThreadsCount, false );
        }

        private void tsmiOpenAllUnreadTray_Click( object sender, EventArgs e )
        {
            this.tsmiOpenAllUnread.PerformClick();
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