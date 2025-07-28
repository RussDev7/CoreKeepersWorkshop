#nullable enable // Silence CS8632.
using System.Windows.Forms;
using System.Reflection;
using System.Threading;
using System.Linq;
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
            #region Embedded-DLL Resolver

            //  When the CLR can’t find a referenced assembly on disk, this handler
            //  looks inside the executable’s resources and loads it from there.
            //  (Handy for "single-file" deployments.)
            AppDomain.CurrentDomain.AssemblyResolve += (s, args) =>
            {
                // The DLL the CLR is looking for (e.g. “Foo.dll”).
                var asmName = new AssemblyName(args.Name).Name + ".dll";

                // Our own executable.
                var me = Assembly.GetExecutingAssembly();

                // Find a resource that ends with “…Foo.dll”.
                var resource = me.GetManifestResourceNames()
                                 .FirstOrDefault(r => r.EndsWith(asmName));

                if (resource == null) return null; // Not embedded → bail out.

                // Read the embedded DLL bytes and load them.
                using (var str = me.GetManifestResourceStream(resource))
                {
                    var data = new byte[str.Length];
                    str.Read(data, 0, data.Length);
                    return Assembly.Load(data);
                }
            };
            #endregion

            // Unique name for the mutex that enforces a single instance.
            string SingleInstanceMutexName = Application.ProductName;

            // Try to acquire the global mutex.
            bool isFirstInstance;
            using var mutex = new Mutex(initiallyOwned: true,
                                        name: SingleInstanceMutexName,
                                        createdNew: out isFirstInstance);

            if (!isFirstInstance) return; // Another copy is already running – quietly exit.

            // WinForms boilerplate.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Run AppContextManager w/ global exception trap.
            try
            {
                Application.Run(new AppContextManager());
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "Unhandled exception",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                Clipboard.SetText(ex.ToString());
            }

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