using CoreKeeperInventoryEditor;
using System.Windows.Forms;
using System;

namespace CoreKeepersWorkshop
{
    public partial class TeleportAddressGuide : Form
    {
        // Form initialization.
        private CustomFormStyler _formThemeStyler;
        public TeleportAddressGuide()
        {
            InitializeComponent();
            Load += (_, __) => _formThemeStyler = this.ApplyTheme(); // Load the forms theme.
        }

        #region Form Load And Closing Events

        // Do loadng events.
        private void TeleportAddressExplanation_Load(object sender, EventArgs e)
        {
            #region Set Custom Cusror

            // Set the applications cursor.
            Cursor = new Cursor(CoreKeepersWorkshop.Properties.Resources.UICursor.GetHicon());
            #endregion

            #region Set Form Locations

            // Set the forms active location based on previous save.
            this.Location = CoreKeepersWorkshop.Properties.Settings.Default.TeleportAddressGuideLocation;
            #endregion

            #region Set Form Opacity

            // Set form opacity based on trackbars value saved setting (1 to 100 -> 0.01 to 1.0).
            this.Opacity = CoreKeepersWorkshop.Properties.Settings.Default.FormOpacity / 100.0;
            #endregion
        }

        #region Form Closing Events

        private void TeleportAddressGuide_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check if the "X" button was pressed to close form.
            // if (!new StackTrace().GetFrames().Any(x => x.GetMethod().Name == "Close"))
            if (_formThemeStyler.CloseButtonPressed) // Now capture the custom titlebar.
            {
                // User pressed the "X" button cancle task.
                // this.Close();
            }

            // Ensure we catch all closing exceptions. // Fix v1.3.3.
            try
            {
                // Save some form settings.
                CoreKeepersWorkshop.Properties.Settings.Default.TeleportAddressGuideLocation = this.Location;
            }
            catch (Exception)
            { } // Do nothing.
        }
        #endregion

        #endregion
    }
}
