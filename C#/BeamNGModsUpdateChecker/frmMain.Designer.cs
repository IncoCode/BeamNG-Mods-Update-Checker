namespace BeamNGModsUpdateChecker
{
    partial class frmMain
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.label1 = new System.Windows.Forms.Label();
            this.lvThreads = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cmsThreadsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiLAddThread = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiMakeRead = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMakeUnread = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.msMenu = new System.Windows.Forms.MenuStrip();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAddThreads = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiMarkAllRead = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.языкLanguageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.englishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.русскийToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.помощьToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOfficialThread = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRepository = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tmrUpd = new System.Windows.Forms.Timer(this.components);
            this.niTray = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmsTray = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiExpand = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.добавитьТемыToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.ssStatus = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.pbCheckUpd = new System.Windows.Forms.ToolStripProgressBar();
            this.tbKeyword = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tmrUpdProgress = new System.Windows.Forms.Timer(this.components);
            this.tsmiRemoveDuplicates = new System.Windows.Forms.ToolStripMenuItem();
            this.cmsThreadsMenu.SuspendLayout();
            this.msMenu.SuspendLayout();
            this.cmsTray.SuspendLayout();
            this.ssStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // lvThreads
            // 
            resources.ApplyResources(this.lvThreads, "lvThreads");
            this.lvThreads.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvThreads.ContextMenuStrip = this.cmsThreadsMenu;
            this.lvThreads.FullRowSelect = true;
            this.lvThreads.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvThreads.Name = "lvThreads";
            this.lvThreads.ShowItemToolTips = true;
            this.lvThreads.UseCompatibleStateImageBehavior = false;
            this.lvThreads.View = System.Windows.Forms.View.Details;
            this.lvThreads.DoubleClick += new System.EventHandler(this.lvThreads_DoubleClick);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // cmsThreadsMenu
            // 
            this.cmsThreadsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiLAddThread,
            this.toolStripMenuItem3,
            this.tsmiMakeRead,
            this.tsmiMakeUnread,
            this.toolStripMenuItem1,
            this.tsmiRemove});
            this.cmsThreadsMenu.Name = "cmsThreadsMenu";
            resources.ApplyResources(this.cmsThreadsMenu, "cmsThreadsMenu");
            // 
            // tsmiLAddThread
            // 
            this.tsmiLAddThread.Name = "tsmiLAddThread";
            resources.ApplyResources(this.tsmiLAddThread, "tsmiLAddThread");
            this.tsmiLAddThread.Click += new System.EventHandler(this.tsmiLAddThread_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");
            // 
            // tsmiMakeRead
            // 
            this.tsmiMakeRead.Name = "tsmiMakeRead";
            resources.ApplyResources(this.tsmiMakeRead, "tsmiMakeRead");
            this.tsmiMakeRead.Click += new System.EventHandler(this.tsmiMakeRead_Click);
            // 
            // tsmiMakeUnread
            // 
            this.tsmiMakeUnread.Name = "tsmiMakeUnread";
            resources.ApplyResources(this.tsmiMakeUnread, "tsmiMakeUnread");
            this.tsmiMakeUnread.Click += new System.EventHandler(this.tsmiMakeUnread_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            // 
            // tsmiRemove
            // 
            this.tsmiRemove.Name = "tsmiRemove";
            resources.ApplyResources(this.tsmiRemove, "tsmiRemove");
            this.tsmiRemove.Click += new System.EventHandler(this.tsmiRemove_Click);
            // 
            // msMenu
            // 
            this.msMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem,
            this.tsmiOptions,
            this.языкLanguageToolStripMenuItem,
            this.помощьToolStripMenuItem});
            resources.ApplyResources(this.msMenu, "msMenu");
            this.msMenu.Name = "msMenu";
            // 
            // файлToolStripMenuItem
            // 
            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAddThreads,
            this.tsmiMarkAllRead,
            this.tsmiRemoveDuplicates,
            this.toolStripMenuItem6,
            this.tsmiRefresh});
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            resources.ApplyResources(this.файлToolStripMenuItem, "файлToolStripMenuItem");
            // 
            // tsmiAddThreads
            // 
            this.tsmiAddThreads.Name = "tsmiAddThreads";
            resources.ApplyResources(this.tsmiAddThreads, "tsmiAddThreads");
            this.tsmiAddThreads.Click += new System.EventHandler(this.tsmiAddThreads_Click);
            // 
            // tsmiMarkAllRead
            // 
            this.tsmiMarkAllRead.Name = "tsmiMarkAllRead";
            resources.ApplyResources(this.tsmiMarkAllRead, "tsmiMarkAllRead");
            this.tsmiMarkAllRead.Click += new System.EventHandler(this.tsmiMarkAllRead_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            resources.ApplyResources(this.toolStripMenuItem6, "toolStripMenuItem6");
            // 
            // tsmiRefresh
            // 
            this.tsmiRefresh.Name = "tsmiRefresh";
            resources.ApplyResources(this.tsmiRefresh, "tsmiRefresh");
            this.tsmiRefresh.Click += new System.EventHandler(this.tsmiRefresh_Click);
            // 
            // tsmiOptions
            // 
            this.tsmiOptions.Name = "tsmiOptions";
            resources.ApplyResources(this.tsmiOptions, "tsmiOptions");
            this.tsmiOptions.Click += new System.EventHandler(this.tsmiOptions_Click);
            // 
            // языкLanguageToolStripMenuItem
            // 
            this.языкLanguageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.englishToolStripMenuItem,
            this.русскийToolStripMenuItem});
            this.языкLanguageToolStripMenuItem.Name = "языкLanguageToolStripMenuItem";
            resources.ApplyResources(this.языкLanguageToolStripMenuItem, "языкLanguageToolStripMenuItem");
            // 
            // englishToolStripMenuItem
            // 
            this.englishToolStripMenuItem.Name = "englishToolStripMenuItem";
            resources.ApplyResources(this.englishToolStripMenuItem, "englishToolStripMenuItem");
            this.englishToolStripMenuItem.Click += new System.EventHandler(this.tsmiEnglish_Click);
            // 
            // русскийToolStripMenuItem
            // 
            this.русскийToolStripMenuItem.Name = "русскийToolStripMenuItem";
            resources.ApplyResources(this.русскийToolStripMenuItem, "русскийToolStripMenuItem");
            this.русскийToolStripMenuItem.Click += new System.EventHandler(this.tsmiRussian_Click);
            // 
            // помощьToolStripMenuItem
            // 
            this.помощьToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiOfficialThread,
            this.tsmiRepository,
            this.toolStripMenuItem5,
            this.tsmiAbout});
            this.помощьToolStripMenuItem.Name = "помощьToolStripMenuItem";
            resources.ApplyResources(this.помощьToolStripMenuItem, "помощьToolStripMenuItem");
            // 
            // tsmiOfficialThread
            // 
            this.tsmiOfficialThread.Name = "tsmiOfficialThread";
            resources.ApplyResources(this.tsmiOfficialThread, "tsmiOfficialThread");
            this.tsmiOfficialThread.Click += new System.EventHandler(this.tsmiOfficialThread_Click);
            // 
            // tsmiRepository
            // 
            this.tsmiRepository.Name = "tsmiRepository";
            resources.ApplyResources(this.tsmiRepository, "tsmiRepository");
            this.tsmiRepository.Click += new System.EventHandler(this.tsmiRepository_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            resources.ApplyResources(this.toolStripMenuItem5, "toolStripMenuItem5");
            // 
            // tsmiAbout
            // 
            this.tsmiAbout.Name = "tsmiAbout";
            resources.ApplyResources(this.tsmiAbout, "tsmiAbout");
            this.tsmiAbout.Click += new System.EventHandler(this.tsmiAbout_Click);
            // 
            // tmrUpd
            // 
            this.tmrUpd.Interval = 1000;
            this.tmrUpd.Tick += new System.EventHandler(this.tmrUpd_Tick);
            // 
            // niTray
            // 
            this.niTray.ContextMenuStrip = this.cmsTray;
            resources.ApplyResources(this.niTray, "niTray");
            this.niTray.DoubleClick += new System.EventHandler(this.niTray_DoubleClick);
            // 
            // cmsTray
            // 
            this.cmsTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiExpand,
            this.toolStripMenuItem4,
            this.добавитьТемыToolStripMenuItem,
            this.toolStripMenuItem2,
            this.tsmiExit});
            this.cmsTray.Name = "cmsTray";
            resources.ApplyResources(this.cmsTray, "cmsTray");
            // 
            // tsmiExpand
            // 
            this.tsmiExpand.Name = "tsmiExpand";
            resources.ApplyResources(this.tsmiExpand, "tsmiExpand");
            this.tsmiExpand.Click += new System.EventHandler(this.tsmiExpand_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            resources.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");
            // 
            // добавитьТемыToolStripMenuItem
            // 
            this.добавитьТемыToolStripMenuItem.Name = "добавитьТемыToolStripMenuItem";
            resources.ApplyResources(this.добавитьТемыToolStripMenuItem, "добавитьТемыToolStripMenuItem");
            this.добавитьТемыToolStripMenuItem.Click += new System.EventHandler(this.tsmiAddT_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            // 
            // tsmiExit
            // 
            this.tsmiExit.Name = "tsmiExit";
            resources.ApplyResources(this.tsmiExit, "tsmiExit");
            this.tsmiExit.Click += new System.EventHandler(this.tsmiExit_Click);
            // 
            // ssStatus
            // 
            this.ssStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.pbCheckUpd});
            resources.ApplyResources(this.ssStatus, "ssStatus");
            this.ssStatus.Name = "ssStatus";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            // 
            // pbCheckUpd
            // 
            this.pbCheckUpd.Name = "pbCheckUpd";
            resources.ApplyResources(this.pbCheckUpd, "pbCheckUpd");
            // 
            // tbKeyword
            // 
            resources.ApplyResources(this.tbKeyword, "tbKeyword");
            this.tbKeyword.Name = "tbKeyword";
            this.tbKeyword.TextChanged += new System.EventHandler(this.tbKeyword_TextChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // tmrUpdProgress
            // 
            this.tmrUpdProgress.Tick += new System.EventHandler(this.tmrUpdProgress_Tick);
            // 
            // tsmiRemoveDuplicates
            // 
            this.tsmiRemoveDuplicates.Name = "tsmiRemoveDuplicates";
            resources.ApplyResources(this.tsmiRemoveDuplicates, "tsmiRemoveDuplicates");
            this.tsmiRemoveDuplicates.Click += new System.EventHandler(this.tsmiRemoveDuplicates_Click);
            // 
            // frmMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbKeyword);
            this.Controls.Add(this.ssStatus);
            this.Controls.Add(this.lvThreads);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.msMenu);
            this.MainMenuStrip = this.msMenu;
            this.Name = "frmMain";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            this.cmsThreadsMenu.ResumeLayout(false);
            this.msMenu.ResumeLayout(false);
            this.msMenu.PerformLayout();
            this.cmsTray.ResumeLayout(false);
            this.ssStatus.ResumeLayout(false);
            this.ssStatus.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView lvThreads;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.MenuStrip msMenu;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiAddThreads;
        private System.Windows.Forms.ContextMenuStrip cmsThreadsMenu;
        private System.Windows.Forms.ToolStripMenuItem tsmiLAddThread;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem tsmiRemove;
        private System.Windows.Forms.Timer tmrUpd;
        private System.Windows.Forms.NotifyIcon niTray;
        private System.Windows.Forms.ToolStripMenuItem tsmiMakeRead;
        private System.Windows.Forms.ContextMenuStrip cmsTray;
        private System.Windows.Forms.ToolStripMenuItem tsmiExpand;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem tsmiExit;
        private System.Windows.Forms.ToolStripMenuItem языкLanguageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem englishToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem русскийToolStripMenuItem;
        private System.Windows.Forms.StatusStrip ssStatus;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem tsmiOptions;
        private System.Windows.Forms.ToolStripMenuItem tsmiMarkAllRead;
        private System.Windows.Forms.ToolStripMenuItem tsmiRefresh;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem tsmiMakeUnread;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem добавитьТемыToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem помощьToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tsmiOfficialThread;
        private System.Windows.Forms.ToolStripMenuItem tsmiRepository;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem tsmiAbout;
        private System.Windows.Forms.ToolStripProgressBar pbCheckUpd;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem6;
        private System.Windows.Forms.TextBox tbKeyword;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Timer tmrUpdProgress;
        private System.Windows.Forms.ToolStripMenuItem tsmiRemoveDuplicates;
    }
}

