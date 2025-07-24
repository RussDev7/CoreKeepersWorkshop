using CoreKeepersWorkshop.Properties;
using System.Collections.Generic;
using CoreKeeperInventoryEditor;
using System.Windows.Forms;

namespace CoreKeepersWorkshop
{
    public static class FormStylingExtensions
    {
        // Global registry of active stylers.
        private static readonly List<CustomFormStyler> _stylers = new();

        /// <summary>
        /// Attach a new CustomFormStyler to the form and remember it.
        /// </summary>
        public static CustomFormStyler ApplyTheme(this Form form)
        {
            /// <summary>
            /// Reads corner check‑boxes from MainForm, returns Corner mask.
            /// (Returns Corner.All for any other window.)
            /// </summary>
            Corner corners = form is MainForm m
                     ? m.SelectedCorners
                     : Corner.All;

            var styler = new CustomFormStyler(
                form,
                theme:          Settings.Default.UITheme,
                roundedCorners: Settings.Default.UICorners,
                cornerRadius:   Settings.Default.UICornerRadius,
                titleBarHeight: Settings.Default.UITitleBarHeight,
                borderSize:     Settings.Default.UIBorderSize,
                showIcon:       Settings.Default.UIShowIcon);

            styler.Enable();
            _stylers.Add(styler);

            // Remove it from the list when the form dies so we don't leak.
            form.FormClosed += (_, __) => _stylers.Remove(styler);

            return styler;
        }

        /// <summary>
        /// Re‑apply the *current* global settings to every open form.
        /// Call this after the user changes Dark/Light, corner radius, etc.
        /// </summary>
        public static void RefreshAllThemes()
        {
            foreach (var s in _stylers.ToArray()) // ToArray - Safe if list mutates.
            {
                s.UpdateStyle(
                    theme:          Settings.Default.UITheme,
                    roundedCorners: Settings.Default.UICorners,
                    cornerRadius:   Settings.Default.UICornerRadius,
                    titleBarHeight: Settings.Default.UITitleBarHeight,
                    borderSize:     Settings.Default.UIBorderSize,
                    showIcon:       Settings.Default.UIShowIcon);
            }
        }
    }
}