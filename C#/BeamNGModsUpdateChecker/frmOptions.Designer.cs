namespace BeamNGModsUpdateChecker
{
    partial class FrmOptions
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if ( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmOptions));
            this.cbAutorun = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.nudUpdInterval = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.cbMinimizeToTray = new System.Windows.Forms.CheckBox();
            this.btnOk = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudUpdInterval)).BeginInit();
            this.SuspendLayout();
            // 
            // cbAutorun
            // 
            resources.ApplyResources(this.cbAutorun, "cbAutorun");
            this.cbAutorun.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cbAutorun.Name = "cbAutorun";
            this.cbAutorun.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // nudUpdInterval
            // 
            resources.ApplyResources(this.nudUpdInterval, "nudUpdInterval");
            this.nudUpdInterval.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nudUpdInterval.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudUpdInterval.Name = "nudUpdInterval";
            this.nudUpdInterval.Value = new decimal(new int[] {
            120,
            0,
            0,
            0});
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // cbMinimizeToTray
            // 
            resources.ApplyResources(this.cbMinimizeToTray, "cbMinimizeToTray");
            this.cbMinimizeToTray.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cbMinimizeToTray.Name = "cbMinimizeToTray";
            this.cbMinimizeToTray.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.Cursor = System.Windows.Forms.Cursors.Hand;
            resources.ApplyResources(this.btnOk, "btnOk");
            this.btnOk.Name = "btnOk";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // FrmOptions
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.cbMinimizeToTray);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nudUpdInterval);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbAutorun);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmOptions";
            this.Load += new System.EventHandler(this.frmOptions_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudUpdInterval)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbAutorun;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nudUpdInterval;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbMinimizeToTray;
        private System.Windows.Forms.Button btnOk;
    }
}