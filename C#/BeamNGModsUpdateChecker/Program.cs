﻿#region Using

using System;
using System.Threading;
using System.Windows.Forms;

#endregion

namespace BeamNGModsUpdateChecker
{
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );
            Application.Run( new FrmMain() );
        }
    }
}