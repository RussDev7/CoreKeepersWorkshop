using System;
using System.Threading;
using System.Windows.Forms;

namespace CoreKeeperInventoryEditor
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Check if application is already open.
            bool result;
            var mutex = new Mutex(true, Application.ProductName, out result);

            // If application is already open, close thread.
            if (!result)
            {
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

            // Keep the mulex from releasing.
            GC.KeepAlive(mutex);
        }
    }
}