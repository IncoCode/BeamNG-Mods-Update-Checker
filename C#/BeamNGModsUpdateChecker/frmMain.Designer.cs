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
            this.tsmiMakeRead = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.msMenu = new System.Windows.Forms.MenuStrip();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAddThreads = new System.Windows.Forms.ToolStripMenuItem();
            this.языкLanguageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.englishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.русскийToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tmrUpd = new System.Windows.Forms.Timer(this.components);
            this.niTray = new System.Windows.Forms.NotifyIcon(this.components);
            this.cmsTray = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiExpand = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.ssStatus = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
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
            resources.ApplyResources(this.cmsThreadsMenu, "cmsThreadsMenu");
            this.cmsThreadsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiLAddThread,
            this.tsmiMakeRead,
            this.toolStripMenuItem1,
            this.tsmiRemove});
            this.cmsThreadsMenu.Name = "cmsThreadsMenu";
            // 
            // tsmiLAddThread
            // 
            resources.ApplyResources(this.tsmiLAddThread, "tsmiLAddThread");
            this.tsmiLAddThread.Name = "tsmiLAddThread";
            this.tsmiLAddThread.Click += new System.EventHandler(this.tsmiLAddThread_Click);
            // 
            // tsmiMakeRead
            // 
            resources.ApplyResources(this.tsmiMakeRead, "tsmiMakeRead");
            this.tsmiMakeRead.Name = "tsmiMakeRead";
            this.tsmiMakeRead.Click += new System.EventHandler(this.tsmiMakeRead_Click);
            // 
            // toolStripMenuItem1
            // 
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            // 
            // tsmiRemove
            // 
            resources.ApplyResources(this.tsmiRemove, "tsmiRemove");
            this.tsmiRemove.Name = "tsmiRemove";
            this.tsmiRemove.Click += new System.EventHandler(this.tsmiRemove_Click);
            // 
            // msMenu
            // 
            resources.ApplyResources(this.msMenu, "msMenu");
            this.msMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem,
            this.языкLanguageToolStripMenuItem});
            this.msMenu.Name = "msMenu";
            // 
            // файлToolStripMenuItem
            // 
            resources.ApplyResources(this.файлToolStripMenuItem, "файлToolStripMenuItem");
            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAddThreads});
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            // 
            // tsmiAddThreads
            // 
            resources.ApplyResources(this.tsmiAddThreads, "tsmiAddThreads");
            this.tsmiAddThreads.Name = "tsmiAddThreads";
            this.tsmiAddThreads.Click += new System.EventHandler(this.tsmiAddThreads_Click);
            // 
            // языкLanguageToolStripMenuItem
            // 
            resources.ApplyResources(this.языкLanguageToolStripMenuItem, "языкLanguageToolStripMenuItem");
            this.языкLanguageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.englishToolStripMenuItem,
            this.русскийToolStripMenuItem});
            this.языкLanguageToolStripMenuItem.Name = "языкLanguageToolStripMenuItem";
            // 
            // englishToolStripMenuItem
            // 
            resources.ApplyResources(this.englishToolStripMenuItem, "englishToolStripMenuItem");
            this.englishToolStripMenuItem.Name = "englishToolStripMenuItem";
            this.englishToolStripMenuItem.Click += new System.EventHandler(this.englishToolStripMenuItem_Click);
            // 
            // русскийToolStripMenuItem
            // 
            resources.ApplyResources(this.русскийToolStripMenuItem, "русскийToolStripMenuItem");
            this.русскийToolStripMenuItem.Name = "русскийToolStripMenuItem";
            this.русскийToolStripMenuItem.Click += new System.EventHandler(this.русскийToolStripMenuItem_Click);
            // 
            // tmrUpd
            // 
            this.tmrUpd.Enabled = true;
            this.tmrUpd.Interval = 1800000;
            this.tmrUpd.Tick += new System.EventHandler(this.tmrUpd_Tick);
            // 
            // niTray
            // 
            resources.ApplyResources(this.niTray, "niTray");
            this.niTray.ContextMenuStrip = this.cmsTray;
            this.niTray.DoubleClick += new System.EventHandler(this.niTray_DoubleClick);
            // 
            // cmsTray
            // 
            resources.ApplyResources(this.cmsTray, "cmsTray");
            this.cmsTray.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiExpand,
            this.toolStripMenuItem2,
            this.tsmiExit});
            this.cmsTray.Name = "cmsTray";
            // 
            // tsmiExpand
            // 
            resources.ApplyResources(this.tsmiExpand, "tsmiExpand");
            this.tsmiExpand.Name = "tsmiExpand";
            this.tsmiExpand.Click += new System.EventHandler(this.tsmiExpand_Click);
            // 
            // toolStripMenuItem2
            // 
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            // 
            // tsmiExit
            // 
            resources.ApplyResources(this.tsmiExit, "tsmiExit");
            this.tsmiExit.Name = "tsmiExit";
            this.tsmiExit.Click += new System.EventHandler(this.tsmiExit_Click);
            // 
            // ssStatus
            // 
            resources.ApplyResources(this.ssStatus, "ssStatus");
            this.ssStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.ssStatus.Name = "ssStatus";
            // 
            // toolStripStatusLabel1
            // 
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            // 
            // frmMain
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
    }
}

