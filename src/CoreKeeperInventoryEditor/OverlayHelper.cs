using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Drawing;
using System.Linq;
using System;

namespace CoreKeepersWorkshop
{
    internal class OverlayHelper
    {
        private static OverlayForm _overlayForm;
        private static Thread _overlayThread;
        private static readonly object _lock = new object();

        public static void ShowOverlay(string message, int durationSeconds = 3)
        {
            if (Process.GetProcessesByName("CoreKeeper").Length == 0)
                return;

            Process coreKeeperProcess = Process.GetProcessesByName("CoreKeeper").Last();
            RECT gameRect;
            GetWindowRect(coreKeeperProcess.MainWindowHandle, out gameRect);

            lock (_lock)
            {
                if (_overlayForm == null || _overlayForm.IsDisposed)
                {
                    _overlayThread = new Thread(() =>
                    {
                        _overlayForm = new OverlayForm(gameRect);
                        Application.Run(_overlayForm);
                    });

                    _overlayThread.SetApartmentState(ApartmentState.STA);
                    _overlayThread.IsBackground = true;
                    _overlayThread.Start();

                    // Wait for form to initialize
                    while (_overlayForm == null || !_overlayForm.IsHandleCreated)
                        Thread.Sleep(10);
                }
            }

            // _overlayForm?.AddMessage(message, durationSeconds);
            if (_overlayForm != null)
                _overlayForm.AddMessage(message, durationSeconds);
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        private struct RECT
        {
            public int Left, Top, Right, Bottom;
        }

        private class OverlayForm : Form
        {
            private const int MaxMessages = 5;
            // private readonly List<(Label label, System.Windows.Forms.Timer timer)> _messages = new();
            private readonly List<Tuple<Label, System.Windows.Forms.Timer>> _messages = new List<Tuple<Label, System.Windows.Forms.Timer>>(); // Downgraded to 4.7.2.
            private readonly Font _messageFont = new Font(FontFamily.GenericSansSerif, 12, FontStyle.Bold);
            private readonly int _lineHeight;

            public OverlayForm(RECT gameRect)
            {
                FormBorderStyle = FormBorderStyle.None;
                BackColor = Color.Black;
                TransparencyKey = Color.Black;
                TopMost = true;
                ShowInTaskbar = false;

                StartPosition = FormStartPosition.Manual;
                Location = new Point(gameRect.Left + 10, gameRect.Top + 30);
                Size = new Size(gameRect.Right - (gameRect.Left + 10), 250);

                _lineHeight = _messageFont.Height + 5; // Add some padding
                Load += (s, e) => AttachToGame();
            }

            public void AddMessage(string message, int durationSeconds)
            {
                Invoke((MethodInvoker)(() =>
                {
                    Label newLabel = new Label
                    {
                        Text = "CKWorkshop: " + message,
                        Font = _messageFont,
                        ForeColor = Color.Lime,
                        BackColor = Color.Transparent,
                        AutoSize = true,
                        Location = new Point(10, 10)
                    };

                    Controls.Add(newLabel);
                    // _messages.Insert(0, (newLabel, StartRemoveTimer(newLabel, durationSeconds)));
                    _messages.Insert(0, new Tuple<Label, System.Windows.Forms.Timer>(newLabel, StartRemoveTimer(newLabel, durationSeconds)));

                    UpdateMessagePositions();

                    if (_messages.Count > MaxMessages)
                    {
                        RemoveOldestMessage();
                    }
                }));
            }

            private void UpdateMessagePositions()
            {
                for (int i = 0; i < _messages.Count; i++)
                {
                    _messages[i].Item1.Location = new Point(10, 10 + (i * _lineHeight));
                }
            }

            private System.Windows.Forms.Timer StartRemoveTimer(Label label, int durationSeconds)
            {
                System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer { Interval = durationSeconds * 1000 };
                timer.Tick += (s, e) =>
                {
                    timer.Stop();
                    timer.Dispose();
                    RemoveMessage(label);
                };
                timer.Start();
                return timer;
            }

            private void RemoveMessage(Label label)
            {
                Invoke((MethodInvoker)(() =>
                {
                    var item = _messages.FirstOrDefault(x => x.Item1 == label);
                    if (item.Item1 != null)
                    {
                        _messages.Remove(item);
                        Controls.Remove(label);
                        label.Dispose();
                        UpdateMessagePositions();
                    }

                    if (_messages.Count == 0)
                    {
                        Close();
                    }
                }));
            }

            private void RemoveOldestMessage()
            {
                var oldest = _messages.Last();
                _messages.Remove(oldest);
                Controls.Remove(oldest.Item1);
                oldest.Item1.Dispose();
                oldest.Item2.Stop();
                oldest.Item2.Dispose();
                UpdateMessagePositions();
            }

            private void AttachToGame()
            {
                IntPtr handle = Handle;
                SetWindowLong(handle, GWL_EXSTYLE, GetWindowLong(handle, GWL_EXSTYLE) | WS_EX_LAYERED | WS_EX_TRANSPARENT | WS_EX_TOPMOST);
                SetWindowPos(handle, HWND_TOPMOST, Left, Top, Width, Height, SWP_NOACTIVATE | SWP_SHOWWINDOW);
            }

            [DllImport("user32.dll")]
            private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

            [DllImport("user32.dll")]
            private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

            [DllImport("user32.dll")]
            private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

            private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
            private const int GWL_EXSTYLE = -20;
            private const int WS_EX_TOPMOST = 0x00000008;
            private const int WS_EX_LAYERED = 0x00080000;
            private const int WS_EX_TRANSPARENT = 0x00000020;
            private const uint SWP_NOACTIVATE = 0x0010;
            private const uint SWP_SHOWWINDOW = 0x0040;
        }
    }
}
