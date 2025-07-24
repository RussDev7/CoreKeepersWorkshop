#nullable enable // Silence CS8632.
using System.Windows.Forms;
using System.Threading;
using System;

namespace CoreKeeperInventoryEditor
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Main method – guards against multiple instances, enables WinForms
        /// visual styles, and starts the UI via <see cref="AppContextManager" />.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Unique name for the mutex that enforces a single instance.
            string SingleInstanceMutexName = Application.ProductName;

            // Try to acquire the global mutex.
            bool isFirstInstance;
            using var mutex = new Mutex(initiallyOwned: true,
                                        name: SingleInstanceMutexName,
                                        createdNew: out isFirstInstance);

            if (!isFirstInstance) return; // Another copy is already running – quietly exit.

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AppContextManager());

            // GC.KeepAlive(mutex);       // Now unnecessary with the using‑statement.
        }

        #region Nested ApplicationContext

        /// <summary>
        /// Custom context lets us close and reopen <see cref="MainForm"/> without
        /// killing the message loop or losing the single‑instance mutex.
        /// </summary>
        private sealed class AppContextManager : ApplicationContext
        {
            public AppContextManager() => ShowMain();

            /// <summary>
            /// Instantiates and shows a new <see cref="MainForm"/>.
            /// </summary>
            private void ShowMain()
            {
                var main = new MainForm();
                this.MainForm = main; // Let ApplicationContext watch it.

                // Event handler that restores native chrome and restarts form.
                void RestoreHandler(object? sender, EventArgs e)
                {
                    main.RestoreNativeRequested -= RestoreHandler;
                    main.FormClosed             -= Main_FormClosed;

                    main.Close(); // Dispose current form.
                    ShowMain();   // Open a fresh one.
                }

                // Wire up events.
                main.RestoreNativeRequested += RestoreHandler;
                main.FormClosed             += (s, e) => ExitThread(); // Keep handler.

                // Show the window (non‑modal).
                main.Show();
            }

            /// <summary>
            /// Ends the message loop when the main form closes.
            /// </summary>
            private void Main_FormClosed(object? sender, FormClosedEventArgs e) =>
                ExitThread(); // Base class method – stops Application.Run.
        }
        #endregion
    }
}