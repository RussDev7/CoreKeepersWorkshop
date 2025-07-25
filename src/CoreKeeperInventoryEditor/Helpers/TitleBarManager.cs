/*
===========================================================================
 File          : TitleBarManager.cs
 Author        : RussDev7
 Last modified : 2025‑07‑17

 Summary:
     A helper that gives any WinForms Form a flat‑style title‑bar with
     rounded corners, dark/light themes, and custom min/max/close buttons.

     Typical use:
         var styler = new CustomFormStyler(this); // Inside Form ctor.
         styler.Enable();                         // Apply styling.

     Call styler.UpdateStyle(...) to tweak options at runtime, or
     styler.Disable() to restore the native window chrome.

 License:
     This file is part of CoreKeepersWorkshop and is distributed under the
     terms of the GNU General Public License v3. See the LICENSE file
     in the project root for the full text.
===========================================================================
*/

using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System;

// Reminder: Update namespace when re‑using this class in a different project.
namespace CoreKeeperInventoryEditor
{
    #region Support Types

    /// <summary>
    /// Which window corners should be rounded.
    /// </summary>
    [Flags]
    public enum Corner
    {
        None        = 0,
        TopLeft     = 1,
        TopRight    = 2,
        BottomRight = 4,
        BottomLeft  = 8,
        All         = TopLeft | TopRight | BottomRight | BottomLeft
    }

    /// <summary>
    /// Color set to apply to the title‑bar and buttons.
    /// </summary>
    public enum ThemeMode
    {
        Dark,
        Light
    }

    #endregion

    /// <summary>
    /// Gives any WinForms <see cref="Form"/> a custom flat title‑bar with
    /// rounded corners, theme support, and fully‑custom buttons.
    /// </summary>
    public sealed class CustomFormStyler
    {
        #region Win32 Interop

        [DllImport("user32.dll")] private static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(
            IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION        = 0x02;

        #endregion

        #region Fields

        public bool CloseButtonPressed { get; private set; }

        private readonly Form _form; // Target window.
        private Panel         _titleBar;
        private Panel         _contentPanel;
        private Label         _titleLabel;
        private Button        _btnMin, _btnMax, _btnClose;
        private PictureBox    _iconBox;

        // Style options (can be changed later via UpdateStyle).
        private int           _titleBarHeight;
        private int           _cornerRadius;
        private int           _borderSize;
        private bool          _showIcon;
        private Corner        _roundedCorners;
        private ThemeMode     _theme;

        // State flags.
        private bool          _enabled;

        #endregion

        #region Ctor

        /// <summary>
        /// Create a new styler, but do not enable it yet.
        /// </summary>
        public CustomFormStyler(
            Form form,
            ThemeMode theme       = ThemeMode.Dark,
            Corner roundedCorners = Corner.All,
            int titleBarHeight    = 30,
            int cornerRadius      = 15,
            int borderSize        = 0,
            bool showIcon         = true)
        {
            _form                 = form ?? throw new ArgumentNullException(nameof(form));
            _theme                = theme;
            _roundedCorners       = roundedCorners;
            _titleBarHeight       = titleBarHeight;
            _cornerRadius         = cornerRadius;
            _borderSize           = borderSize;
            _showIcon             = showIcon;
        }
        #endregion

        #region Public API

        /// <summary>
        /// Replace native chrome with the custom one.
        /// </summary>
        public void Enable()
        {
            if (_enabled) return;
            _enabled = true;

            // Remember the old client size.
            var oldClient = _form.ClientSize;

            // Kill system frame & add padding for our border.
            _form.FormBorderStyle = FormBorderStyle.None;
            _form.Padding         = new Padding(_borderSize);
            SetDoubleBuffered(_form);

            // Enlarge client area so content gets all of its original space back.
            _form.ClientSize = new Size(
                oldClient.Width,
                oldClient.Height + _titleBarHeight
            );

            WrapExistingControls();            // Wrap all existing controls.
            BuildTitleBar();                   // Build and add titlebar.

            // Add controls in the correct order.
            _form.Controls.Clear();
            _form.Controls.Add(_contentPanel); // Fill.
            _form.Controls.Add(_titleBar);     // Top.

            WireFormEvents();
            UpdateLayout();                    // Apply colors, text, button placement.
        }

        /// <summary>
        /// Return the window to its normal style.
        /// </summary>
        public void Disable()
        {
            if (!_enabled) return;
            _enabled = false;

            // Remove custom bar & handlers.
            if (_titleBar != null)
            {
                _titleBar.MouseDown -= TitleBar_MouseDown;
                _titleBar.Controls.Clear();
                _form.Controls.Remove(_titleBar);
                _titleBar.Dispose();
                _titleBar = null;
            }

            // Tell a possible MainForm to restore its own native style.
            if (_form is MainForm mf)
            {
                mf.OnRestoreNativeRequested();
            }
        }

        /// <summary>
        /// Toggle custom chrome on/off.
        /// </summary>
        public void Toggle()  => (_enabled ? (Action)Disable : Enable)();

        /// <summary>
        /// Gets a value indicating whether this styler is currently enabled.
        /// </summary>
        public bool IsEnabled => _enabled;

        /// <summary>
        /// Change any subset of style options at runtime.
        /// Only the values you supply are modified.
        /// </summary>
        public void UpdateStyle(
            ThemeMode? theme          = null,
            Corner?    roundedCorners = null,
            int?       titleBarHeight = null,
            int?       cornerRadius   = null,
            int?       borderSize     = null,
            bool?      showIcon       = null)
        {
            if (theme != null)          _theme          = theme.Value;
            if (roundedCorners != null) _roundedCorners = roundedCorners.Value;
            if (titleBarHeight != null) _titleBarHeight = titleBarHeight.Value;
            if (cornerRadius != null)   _cornerRadius   = cornerRadius.Value;
            if (borderSize != null)     _borderSize     = borderSize.Value;
            if (showIcon != null)       _showIcon       = showIcon.Value;

            UpdateLayout();
        }
        #endregion

        #region Initial Construction Helpers

        /// <summary>
        /// Create title‑bar and its child controls (buttons etc.).
        /// </summary>
        private void BuildTitleBar()
        {
            _titleBar = new Panel
            {
                Dock   = DockStyle.Top,
                Height = _titleBarHeight
            };
            _titleBar.MouseDown += TitleBar_MouseDown;
            _form.Controls.Add(_titleBar);
            _form.Controls.SetChildIndex(_titleBar, 0);
            _titleBar.BringToFront();

            // Icon (optional).
            if (_showIcon)
            {
                _iconBox = new PictureBox
                {
                    Size      = new Size(_titleBarHeight - 16, _titleBarHeight - 16),
                    Location  = new Point(9, 7),
                    BackColor = Color.Transparent,
                    SizeMode  = PictureBoxSizeMode.StretchImage
                };
                _titleBar.Controls.Add(_iconBox);
            }

            // Window title.
            _titleLabel           = new Label { AutoSize = true };
            _titleLabel.MouseDown += TitleBar_MouseDown;
            _titleBar.Controls.Add(_titleLabel);

            // Buttons.
            _btnMin   = MakeButton("—", Minimize);
            _btnMax   = MakeButton("☐", ToggleMaximise);
            _btnClose = MakeButton("✕", () =>
            {
                CloseButtonPressed = true;
                _form.Close();
            });
            _titleBar.Controls.AddRange(new[] { _btnMin, _btnMax, _btnClose });
        }

        /// <summary>
        /// Create a panel that will own every control already placed on the Form.
        /// This lets us freely resize the title‑bar without touching user content.
        /// </summary>
        private void WrapExistingControls()
        {
            _contentPanel = new Panel
            {
                Dock      = DockStyle.Fill,
                Padding   = _form.Padding,
                BackColor = _form.BackColor
            };

            foreach (Control c in _form.Controls.Cast<Control>().ToArray())
            {
                _contentPanel.Controls.Add(c);
                _contentPanel.Controls.SetChildIndex(c, 1);
            }
        }

        /// <summary>
        /// Hook form‑level events that need live layout refresh.
        /// </summary>
        private void WireFormEvents()
        {
            _form.Load        += (_, __) => UpdateLayout();
            _form.SizeChanged += (_, __) => _form.BeginInvoke((Action)UpdateLayout);
            _form.ResizeEnd   += (_, __) => UpdateLayout();
            _form.TextChanged += (_, __) => UpdateLayout();
        }
        #endregion

        #region Title‑bar Button Helpers

        private Button MakeButton(string text, Action onClick)
        {
            var btn = new Button
            {
                Text      = text,
                FlatStyle = FlatStyle.Flat,
                Size      = new Size(30, _titleBarHeight),
                Anchor    = AnchorStyles.Top | AnchorStyles.Right,
                TabStop   = false
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (_, __) => onClick();
            return btn;
        }

        private void Minimize() => _form.WindowState = FormWindowState.Minimized;
        private void ToggleMaximise()
            => _form.WindowState = _form.WindowState == FormWindowState.Maximized
                                 ? FormWindowState.Normal
                                 : FormWindowState.Maximized;
        #endregion

        #region Mouse Drag

        /// <summary>
        /// Let the user drag the window by holding the custom title‑bar.
        /// </summary>
        private void TitleBar_MouseDown(object _, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            ReleaseCapture();
            SendMessage(_form.Handle, WM_NCLBUTTONDOWN, (IntPtr)HTCAPTION, IntPtr.Zero);
        }
        #endregion

        #region Layout & Drawing

        /// <summary>
        /// Re‑evaluate titles, colors, button positions, rounded region, etc.
        /// Called on Load, Resize, TextChanged and whenever style settings change.
        /// </summary>
        private void UpdateLayout()
        {
            // Padding / border.
            if (_form.Padding.All != _borderSize)
                _form.Padding = new Padding(_borderSize);

            // Theme colors.
            Color formBack, titleBack, titleFore;
            if (_theme == ThemeMode.Dark)
            {
                // Color.FromArgb(50, 50, 50); // Slate‑grey.
                // Color.FromArgb(28, 28, 28); // Dark gray.

                formBack  = Color.FromArgb(28, 28, 28);
                titleBack = Color.Black;
                titleFore = Color.White;
            }
            else
            {
                formBack  = Color.FromArgb(230, 230, 230); // Light white.
                titleBack = Color.White;
                titleFore = Color.Black;
            }

            // Titlebar colors.
            _titleBar.BackColor   = titleBack;
            _titleLabel.ForeColor = titleFore;
            foreach (var b in new[] { _btnMin, _btnMax, _btnClose })
            {
                b.BackColor = titleBack;
                b.ForeColor = titleFore;
            }

            // Form backcolor.
            _contentPanel.BackColor = formBack;

            // Icon.
            if (_showIcon)
            {
                if (_iconBox != null)
                {
                    _iconBox.Visible = true;
                    _iconBox.Image   = _form.Icon?.ToBitmap();
                }
            }
            else if (_iconBox != null)
            {
                _iconBox.Visible = false;
            }

            // Title text & position.
            int labelLeft = _showIcon && _iconBox?.Visible == true
                            ? _iconBox.Right + 6
                            : 10;
            _titleLabel.Location = new Point(labelLeft, 7);
            _titleLabel.Text = _form.Text;

            // Buttons – right‑align in order Close | Max | Min.
            int x = _form.ClientSize.Width;
            PositionButton(_btnClose, _form.ControlBox);
            PositionButton(_btnMax,   _form.MaximizeBox);
            PositionButton(_btnMin,   _form.MinimizeBox);

            void PositionButton(Button btn, bool visible)
            {
                btn.Visible = visible;
                if (!visible) return;
                x -= btn.Width;
                btn.Location = new Point(x, 0);
            }

            // Apply rounded region (maximized windows skip this).
            if (_form.WindowState == FormWindowState.Normal)
                ApplyRoundedRegion();
            else
                _form.Region = null;
        }

        /// <summary>
        /// Create a rounded GraphicsPath according to _roundedCorners.
        /// </summary>
        private void ApplyRoundedRegion()
        {
            var r = _form.ClientRectangle;
            if (r.Width <= 2 || r.Height <= 2) return; // Ignore minimized size.

            int d = _cornerRadius * 2;
            var gp = new GraphicsPath();

            // TL.
            if (_roundedCorners.HasFlag(Corner.TopLeft))
                gp.AddArc(r.X, r.Y, d, d, 180, 90);
            else
                gp.AddLine(r.X, r.Y, r.X, r.Y);

            // TR.
            if (_roundedCorners.HasFlag(Corner.TopRight))
                gp.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            else
                gp.AddLine(r.Right, r.Y, r.Right, r.Y);

            // BR.
            if (_roundedCorners.HasFlag(Corner.BottomRight))
                gp.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            else
                gp.AddLine(r.Right, r.Bottom, r.Right, r.Bottom);

            // BL.
            if (_roundedCorners.HasFlag(Corner.BottomLeft))
                gp.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            else
                gp.AddLine(r.X, r.Bottom, r.X, r.Bottom);

            gp.CloseFigure();
            _form.Region?.Dispose();
            _form.Region = new Region(gp);
        }
        #endregion

        #region Misc Utilities

        /// <summary>
        /// Enable true double‑buffering via reflection.
        /// </summary>
        private static void SetDoubleBuffered(Control c) =>
            typeof(Control).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance)
            ?.SetValue(c, true);
        #endregion
    }
}
