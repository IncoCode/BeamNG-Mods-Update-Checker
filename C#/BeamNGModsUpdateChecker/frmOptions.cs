﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace BeamNGModsUpdateChecker
{
    public partial class frmOptions : Form
    {
        frmMain MainForm;

        public frmOptions( frmMain MainForm )
        {
            this.MainForm = MainForm;
            InitializeComponent();
        }

        private void addRemoveStartup( bool add )
        {
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey( "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true );
            if ( add )
            {
                rkApp.SetValue( "BeamNGModsUpdateChecker", Application.ExecutablePath.ToString() );
            }
            else
            {
                rkApp.DeleteValue( "BeamNGModsUpdateChecker", false );
            }
        }

        private bool IsStartupItem()
        {
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey( "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true );
            if ( rkApp.GetValue( "BeamNGModsUpdateChecker" ) == null )
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void btnOk_Click( object sender, EventArgs e )
        {
            this.MainForm.minimizeWhenStart = cbMinimizeToTray.Checked;
            this.MainForm.updInterval = (int)nudUpdInterval.Value;
            this.addRemoveStartup( cbAutorun.Checked );
            this.Close();
        }

        private void frmOptions_Load( object sender, EventArgs e )
        {
            cbAutorun.Checked = this.IsStartupItem();
        }
    }
}