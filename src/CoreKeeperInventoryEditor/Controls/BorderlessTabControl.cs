/*
===========================================================================
 File          : BorderlessTabControl.cs
 Author        : RussDev7
 Last modified : 2025‑07‑17

 Summary:
     A TabControl that:

     • Removes the OS‑drawn borders / client‑edge.
     • Paints its own tabs (owner‑draw).
     • Keeps the page‑area transparent so the parent can shine through.
     • Draws custom bevels, separators, and pressed / selected states.
     • Shrinks "Normal" tabs so their height matches FlatButtons / Buttons.

     Typical use:
         Simply drop it on a Form. You can optionally set each TabPage.Tag
         to a Color to give that tab its own background color.

 License:
     This file is part of CoreKeepersWorkshop and is distributed under the
     terms of the GNU General Public License v3. See the LICENSE file
     in the project root for the full text.
===========================================================================
*/

using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System;

// Reminder: Update namespace when re‑using this class in a different project.
namespace CoreKeepersWorkshop
{
    /// <summary>
    /// Border‑free TabControl with owner‑drawn buttons and optional per‑tab
    /// background colors (store a <see cref="Color"/> in <c>TabPage.Tag</c>).
    /// </summary>
    public sealed class BorderlessTabControl : TabControl
    {
        #region Constants & Interop

        // Window‑styles to strip from CreateParams.
        private const int WS_BORDER        = 0x00000008;
        private const int WS_EX_CLIENTEDGE = 0x00000200;

        // TabControl messages.
        private const int TCM_FIRST        = 0x1300;
        private const int TCM_ADJUSTRECT   = TCM_FIRST + 40;

        // Misc.
        private const int SeparatorXOffset = 1; // Horizontal shift for the vertical bar.

        // Colors.
        private static readonly Color BevelColor = SystemColors.ControlDarkDark;

        #endregion

        #region Fields

        private bool _heightQueued;     // Stops queueing multiple delegates when the handle is destroyed/re‑created by layout changes.
        private int _pressedIndex = -1; // –1 ⇢ no tab is being clicked.

        #endregion

        #region Ctor & CreateParams

        /// <summary>
        /// Constructor – sets owner‑draw, double‑buffering, transparency, etc.
        /// </summary>
        public BorderlessTabControl()
        {
            base.OnCreateControl();

            // Paint everything ourselves.
            DrawMode = TabDrawMode.OwnerDrawFixed;
            /// Appearance = TabAppearance.FlatButtons; // Optional – set in Designer.

            SetStyle(ControlStyles.UserPaint             |
                     ControlStyles.AllPaintingInWmPaint  |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw          |
                     ControlStyles.SupportsTransparentBackColor,
                     true);
        }

        /// <summary>
        /// Strip the OS border and client‑edge so the tabs look flush.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.Style   &= ~WS_BORDER;
                cp.ExStyle &= ~WS_EX_CLIENTEDGE;
                return cp;
            }
        }
        #endregion

        #region Win32 Adjustments

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int Left, Top, Right, Bottom; }

        /// <summary>
        /// Intercepts <c>TCM_ADJUSTRECT</c> to expand the client rectangle and
        /// suppresses <c>WM_ERASEBKGND</c> so the background stays transparent.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == TCM_ADJUSTRECT && !DesignMode)
            {
                base.WndProc(ref m);     // Ask Windows first.

                // Enlarge slightly so the thin border disappears.
                var rc = Marshal.PtrToStructure<RECT>(m.LParam);
                rc.Left   -= 4;
                rc.Top    -= 1;          // Hides the separator line.
                rc.Right  += 4;
                rc.Bottom += 4;
                Marshal.StructureToPtr(rc, m.LParam, true);
                return;                  // We handled it.
            }

            if (m.Msg == 0x0014) return; // WM_ERASEBKGND → ignore.

            base.WndProc(ref m);
        }
        #endregion

        #region Custom Property Options

        /// <summary>
        /// When <c>true</c> and a <see cref="TabPage"/> has a
        /// <see cref="TabPage.BackgroundImage"/>, the image is stretched to fill the
        /// button using <b>nearest‑neighbor</b> interpolation (crisp pixel‑art
        /// look).  
        /// When <c>false</c> the image is stretched with the normal
        /// HighQualityBicubic interpolation.
        /// </summary>
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Stretch tab background images with nearest‑neighbor " +
                     "interpolation instead of the default smoothing.")]
        public bool NearestNeighborStretch
        {
            get => _nearestNeighborStretch;
            set { _nearestNeighborStretch = value; Invalidate(); }
        }
        private bool _nearestNeighborStretch = false;

        #endregion

        #region Painting

        /// <summary>
        /// Custom painting for every tab button, plus separators.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            for (int i = 0; i < TabCount; i++)
            {
                Rectangle outer = GetTabRect(i);

                bool isPressed  = (i == _pressedIndex);
                bool isSelected = (i == SelectedIndex);

                // Background.
                // Now handled in OnPaintBackground().

                // Button background colors.
                //  Fill the button with the per‑tab color (if any).
                Color back = GetButtonBackColor(TabPages[i]);
                if (!back.IsEmpty)
                {
                    using var br = new SolidBrush(back);
                    e.Graphics.FillRectangle(br, outer);
                }

                // Caption.
                Rectangle textRect = outer;
                if (isSelected) textRect.Offset(1, 1); // Pressed look.
                if (Appearance == TabAppearance.Normal) textRect.Offset(0, -2);

                Font tabCaptionFont       = GetCaptionFont(TabPages[i]);
                Color tabCaptionForeColor = GetCaptionColor(TabPages[i]);
                TextRenderer.DrawText(e.Graphics, TabPages[i].Text, tabCaptionFont,
                                      textRect, tabCaptionForeColor,
                                      TextFormatFlags.HorizontalCenter |
                                      TextFormatFlags.VerticalCenter);

                // Bevel outline.
                switch (Appearance)
                {
                    case TabAppearance.FlatButtons:
                        if (isPressed && !isSelected)   DrawBevel(e.Graphics, outer, 0, false);
                        else if (isSelected)          { DrawBevel(e.Graphics, outer, 0, true);  DrawBevel(e.Graphics, outer, 1, true);  }
                        break;

                    case TabAppearance.Buttons:
                        if (isPressed && !isSelected) { DrawBevel(e.Graphics, outer, 0, true);  DrawBevel(e.Graphics, outer, 1, true);  }
                        else if (isSelected)          { DrawBevel(e.Graphics, outer, 0, true);  DrawBevel(e.Graphics, outer, 1, true);  }
                        else                          { DrawBevel(e.Graphics, outer, 0, false); DrawBevel(e.Graphics, outer, 1, false); }
                        break;

                    default: // Normal.
                        DrawBevel(e.Graphics, outer, 0, isRightOnly: true);
                        break;
                }

                // Vertical separator.
                if (Appearance   == TabAppearance.FlatButtons &&
                    i < TabCount - 1 &&
                    outer.Bottom == GetTabRect(i + 1).Bottom) // Same row?
                {
                    int yPad     = 2;
                    int x        = outer.Right + SeparatorXOffset;
                    e.Graphics.DrawLine(SystemPens.ControlDark,
                                        x,     outer.Top    + yPad + 1,
                                        x,     outer.Bottom - yPad + 1);
                    e.Graphics.DrawLine(SystemPens.ControlLight,
                                        x + 1, outer.Top    + yPad + 1,
                                        x + 1, outer.Bottom - yPad + 1);
                }
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            var page = SelectedTab;

            // If there’s a bitmap, draw it on top.
            if (page?.BackgroundImage is Bitmap bmp)
            {
                var oldMode = e.Graphics.InterpolationMode;
                e.Graphics.InterpolationMode = NearestNeighborStretch
                                               ? InterpolationMode.NearestNeighbor
                                               : InterpolationMode.HighQualityBicubic;

                e.Graphics.DrawImage(bmp, DisplayRectangle);
                e.Graphics.InterpolationMode = oldMode;
            }
        }
        #endregion

        #region Caption Style Helpers

        /// <summary>
        /// Returns the font that should be used when drawing a tab caption.
        /// Priority: <see cref="TabPage.Font"/> → control‑wide <see cref="Control.Font"/>.
        /// </summary>
        private Font GetCaptionFont(TabPage page)
        {
            // TabPage inherits its parent's Font unless explicitly overridden.
            // If it differs from the control's, assume the user set it.
            return !ReferenceEquals(page.Font, Font) ? page.Font : Font;
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            Invalidate(); // Repaint captions with the new font.
        }

        /// <summary>
        /// Returns the color that should be used when drawing a tab caption.
        /// Priority: <see cref="TabPage.ForeColor"/> → control‑wide <see cref="Control.ForeColor"/>.
        /// </summary>
        private Color GetCaptionColor(TabPage page)
        {
            if (page.ForeColor != Color.Empty &&
                page.ForeColor != SystemColors.ControlText)
                return page.ForeColor;

            return ForeColor;
        }

        protected override void OnForeColorChanged(EventArgs e)
        {
            base.OnForeColorChanged(e);
            Invalidate(); // Repaint captions with the new color.
        }
        #endregion

        #region Bevel Helpers

        /// <summary>
        /// Color overrides via TabPage.Tag.
        /// The null‑safe check keeps the helper designer safe.
        /// </summary>
        private static Color GetButtonBackColor(TabPage page)
        {
            if (page?.Tag == null) return Color.Empty;

            if (page.Tag is Color c) return c;
            if (page.Tag is string s)
            {
                // #RRGGBB or color names
                try { return ColorTranslator.FromHtml(s); } catch { }
                try { return Color.FromName(s);           } catch { }
            }
            return Color.Empty;
        }

        /* Draws one “ring” of the bevel.
           Parameters adjust which edges to draw and how far inside. */
        private void DrawBevel(Graphics g, Rectangle outer, int ring,
                               bool isTopLeft = false,
                               bool isRightOnly = false)
        {
            Rectangle r = outer;     // Copy.
            r.Inflate(-ring, -ring); // Step in for each ring.

            // Offsets vary by Appearance & pressed/selected state.
            int TOP_OFF, BOTTOM_OFF, LEFT_OFF, RIGHT_OFF, H_SHIFT;
            if      (Appearance == TabAppearance.FlatButtons)
            {
                TOP_OFF    = isTopLeft ? 2 : 3;
                BOTTOM_OFF = isTopLeft ? 0 : 2;
                LEFT_OFF   = 0;
                RIGHT_OFF  = -4;
                H_SHIFT    = 3;
            }
            else if (Appearance == TabAppearance.Buttons)
            {
                TOP_OFF    = isTopLeft ? 2 : 3;
                BOTTOM_OFF = isTopLeft ? 0 : 2;
                LEFT_OFF   = 0;
                RIGHT_OFF  = -1;
                H_SHIFT    = 0;
            }
            else    // Normal.
            {
                TOP_OFF   = BOTTOM_OFF = LEFT_OFF = 0;
                RIGHT_OFF = -1;
                H_SHIFT   = 0;
            }

            // Apply offsets.
            r.Y      += TOP_OFF;
            r.Height -= BOTTOM_OFF * 2;
            r.X      += H_SHIFT    + LEFT_OFF;
            r.Width  -= H_SHIFT    - RIGHT_OFF;

            using var pen = new Pen(BevelColor);
            if (isRightOnly)
            {
                g.DrawLine(pen, r.Right, r.Top,    r.Right, r.Bottom); // Right.
                return;
            }

            if (isTopLeft)
            {
                g.DrawLine(pen, r.Left,  r.Top,    r.Right, r.Top);    // Top.
                g.DrawLine(pen, r.Left,  r.Top,    r.Left,  r.Bottom); // Left.
            }
            else
            {
                g.DrawLine(pen, r.Left,  r.Bottom, r.Right, r.Bottom); // Bottom.
                g.DrawLine(pen, r.Right, r.Top,    r.Right, r.Bottom); // Right.
            }
        }
        #endregion

        #region Height Fix For Normal Tabs

        /// <summary>
        /// When the native handle is first created (or re‑created after DPI/theme
        /// changes) we queue two layout‑fix delegates:
        /// • <see cref="ReclaimNormalTabHeight"/> – normalizes tab‑strip height.
        /// • <see cref="AdjustControlSize"/>      – compensates for WndProc inset.
        /// </summary>
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            // Make sure we queue the delegates only once per handle build.
            if (!_heightQueued)
            {
                _heightQueued = true;

                // Fix the height of TabAppearance.Normal after the handle exists.
                BeginInvoke((MethodInvoker)ReclaimNormalTabHeight);

                // Shrinks this control—or, when Dock = Fill, its parent—to offset the
                // extra margin added in WndProc.
                //
                // Passing 'false' here keeps the control at the exact size defined in the
                // designer. The host application can later call AdjustControlSize(true)
                // (or set the Size of this control / its parent directly) once it really
                // wants the inset applied.
                BeginInvoke((MethodInvoker)(() => AdjustControlSize(false)));
            }
        }

        /// <summary>
        /// Compensates for the extra margin we add back in <c>WndProc</c>
        /// (<c>TCM_ADJUSTRECT</c>) by shrinking either this control
        /// or—when docked <see cref="DockStyle.Fill"/>—its parent’s client
        /// rectangle.  
        ///
        /// | Why two code paths? |
        /// ‑‑---------------------
        /// • Normal / Anchored / Dock‑Top/Left/Right/Bottom
        ///   The layout engine respects the <c>Size</c> you set, so we can
        ///   simply subtract the margin from our own width/height.
        ///
        /// • Dock = Fill
        ///   The layout engine *immediately overwrites* any size we assign to
        ///   a filled control. To keep the visual inset we instead make the
        ///   parent’s client area smaller **once** per handle‑creation cycle.
        /// </summary>
        /// <param name="enabled">
        /// Pass <c>true</c> when you want the adjustment to run (first handle
        /// build); pass <c>false</c> when queueing a placeholder during
        /// design‑time instantiation.
        /// </param>
        private bool _parentShrunk;     // Keeps us from nibbling multiple times.
        private void AdjustControlSize(bool enabled = false)
        {
            if (!enabled) return;       // No‑op when queued as a design‑time placeholder.

            const int shrinkWidth  = 8; // Horizontal inset added in WndProc.
            const int shrinkHeight = 0; // Vertical inset.

            /* --------------------------------------------------------------
               1. Dock ≠ Fill -> we can safely resize this control.
               -------------------------------------------------------------- */
            if (Dock != DockStyle.Fill)
            {
                // Guard against over‑shrinking on handle re‑creation.
                if (Width  <= shrinkWidth ||
                    Height <= shrinkHeight)
                    return;

                Size = new Size(Width  - shrinkWidth,
                                Height - shrinkHeight);
                return;
            }

            /* --------------------------------------------------------------
               2. Dock = Fill -> layout always stretches us to the parent's
                  client area, so we shrink the parent once instead.
               -------------------------------------------------------------- */
            if (_parentShrunk) return;  // Already handled for this handle build.
            _parentShrunk = true;

            Control p = Parent;
            if (p == null) return;      // Unlikely, but protects against odd cases.

            // Prevent collapse below the desired inset.
            if (p.ClientSize.Width  > shrinkWidth &&
                p.ClientSize.Height > shrinkHeight)
            {
                p.ClientSize = new Size(p.ClientSize.Width  - shrinkWidth,
                                        p.ClientSize.Height - shrinkHeight);
            }
        }

        /// <summary>
        /// Forces TabAppearance.Normal to use the same 24‑px height as Buttons +
        /// FlatButtons, while keeping natural width.
        /// </summary>
        private void ReclaimNormalTabHeight()
        {
            // Never run in the designer or after disposal.
            if (!IsHandleCreated || IsDisposed) return; // Handle was just torn down.
            if (TabPages.Count == 0) return;            // No pages yet (opening in designer).

            if (Appearance != TabAppearance.Normal) { SizeMode = TabSizeMode.Normal; return; }

            SizeMode = TabSizeMode.Fixed; // So TCM_SETITEMSIZE is sent.

            // Measure the widest caption.
            int widest = 0;
            using (var g = CreateGraphics())
                foreach (TabPage p in TabPages)
                    widest = Math.Max(widest, TextRenderer.MeasureText(g, p.Text, Font).Width);

            const int Pad = 16;
            ItemSize = new Size(widest + Pad, 24); // 24 px tall like the others.

            UpdateStripRegion();
            Invalidate();
        }
        #endregion

        #region Region Sync

        // Keep the Region updated whenever pages or size change.
        protected override void OnControlAdded   (ControlEventArgs e) { base.OnControlAdded(e);    UpdateStripRegion(); }
        protected override void OnControlRemoved (ControlEventArgs e) { base.OnControlRemoved(e);  UpdateStripRegion(); }
        protected override void OnSizeChanged    (EventArgs e)        { base.OnSizeChanged(e);     UpdateStripRegion(); }
        protected override void OnLayout         (LayoutEventArgs e)  { base.OnLayout(e);          UpdateStripRegion(); }

        // Pressed‑state logic.
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _pressedIndex = GetIndexAtPoint(e.Location);
            if (_pressedIndex >= 0) Invalidate(GetTabRect(_pressedIndex));
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_pressedIndex >= 0)
            {
                Invalidate(GetTabRect(_pressedIndex));
                _pressedIndex = -1;
            }
        }
        private int GetIndexAtPoint(Point p)
        {
            for (int i = 0; i < TabCount; i++)
                if (GetTabRect(i).Contains(p)) return i;
            return -1;
        }
        #endregion

        #region Region Calculation

        /// <summary>
        /// Builds a <see cref="Region"/> that exactly matches the union of:
        /// • All tab buttons (with small adjustments per style)  
        /// • The page area (DisplayRectangle)
        /// This lets the control respect transparency and proper hit‑testing.
        /// </summary>
        private void UpdateStripRegion()
        {
            if (!IsHandleCreated || IsDisposed) return;

            using var path = new GraphicsPath();

            // Tab buttons — shrink slightly to pull them inside.
            for (int i = 0; i < TabCount; i++)
            {
                Rectangle r = GetTabRect(i);
                if      (Appearance == TabAppearance.FlatButtons) path.AddRectangle(Shrink(r, 3, 3, 2, 0));
                else if (Appearance == TabAppearance.Buttons)     path.AddRectangle(Shrink(r, 0, 0, 2, 0));
                else                                              path.AddRectangle(Shrink(r, 0, 0, 0, 2));
            }

            // Separators (FlatButtons only).
            if (Appearance == TabAppearance.FlatButtons)
            {
                for (int i = 0; i < TabCount - 1; i++)
                {
                    Rectangle r = GetTabRect(i);
                    if (r.Bottom == GetTabRect(i + 1).Bottom) // Same row.
                    {
                        const int yShift = 3;
                        r.Y      += yShift;
                        r.Height -= yShift;

                        var stripe = new Rectangle(r.Right + SeparatorXOffset,
                                                   r.Top, 1, r.Height);
                        path.AddRectangle(stripe);
                    }
                }
            }

            // Page surface.
            path.AddRectangle(DisplayRectangle);

            Region = new Region(path);
        }

        private static Rectangle Shrink(Rectangle rc,
                                        int left, int right, int top, int bottom)
        {
            rc.X      += left;
            rc.Y      += top;
            rc.Width  -= left + right;
            rc.Height -= top  + bottom;
            return rc;
        }
        #endregion
    }

    #region Theme Helpers (Optional)

    /// <summary>
    /// Extension helpers that add theming support to any
    /// <see cref="BorderlessTabControl"/> without touching its source file.
    /// </summary>
    public static class BorderlessTabControlExtensions
    {
        /* -------------------------------------------------------------
           1. A tiny palette enum so callers can say:
              myTabControl.RecolorAllTabs(ThemeMode.Dark);
           ------------------------------------------------------------ */
        public enum ThemeMode
        {
            Dark,
            Light
        }

        /* -------------------------------------------------------------
           2. Extension method (note the this BorderlessTabControl).
              Because of the “this” keyword the compiler treats it as if
              it were an *instance* method on the control:
                  ctl.RecolorAllTabs(ThemeMode.Light);
           ------------------------------------------------------------ */
        public static void RecolorAllTabs(this BorderlessTabControl ctl,
                                          ThemeMode themeMode)
        {
            if (ctl == null) return; // Safety‑net for null refs.

            // Freeze layout to avoid flicker while we update every page.
            ctl.SuspendLayout();

            foreach (TabPage page in ctl.TabPages)
            {
                // Store the background color in Tag (read by OnPaint) and
                // update the caption color so text stays readable.
                if (themeMode == ThemeMode.Dark)
                {
                    page.Tag = Color.FromArgb(50, 50, 50); // Slate‑grey.
                    page.ForeColor = Color.White;          // White text.
                }
                else
                {
                    page.Tag = SystemColors.Control;       // Standard light grey.
                    page.ForeColor = Color.Black;          // Black text.
                }
            }

            ctl.ResumeLayout(); // Re‑enable layout engine.
            ctl.Invalidate();   // Single repaint for the whole strip.
        }
    }
    #endregion
}
