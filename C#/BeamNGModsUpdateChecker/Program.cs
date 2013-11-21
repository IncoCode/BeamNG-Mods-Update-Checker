using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace BeamNGModsUpdateChecker
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using ( Mutex mutex = new Mutex( false, "Global\\" + appGuid ) )
            {
                if ( !mutex.WaitOne( 0, false ) )
                {
                    MessageBox.Show( "Program already running!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault( false );
                Application.Run( new FrmMain() );
            }
        }

        private static string appGuid = "6759aa94-1382-4375-a900-b19e146a6010";
    }
}
