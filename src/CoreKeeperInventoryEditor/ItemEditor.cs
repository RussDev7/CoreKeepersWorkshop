using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace CoreKeepersWorkshop
{
    public partial class ItemEditor : Form
    {
        public ItemEditor()
        {
            InitializeComponent();
        }

        // Form closing saving.
        int selectedItemType = 0;
        int selectedItemAmount = 0;
        int selectedItemVariation = 0;
        bool userCancledTask = false;

        // Define closing varibles.
        public int GetItemTypeFromList()
        {
            return selectedItemType;
        }
        public int GetItemAmountFromList()
        {
            return selectedItemAmount;
        }
        public int GetItemVeriationFromList()
        {
            return selectedItemVariation;
        }
        public bool GetUserCancledTask()
        {
            return userCancledTask;
        }

        #region Form Load And Closing Events

        // Do loading events.
        private void ItemEditor_Load(object sender, EventArgs e)
        {
            #region Set Form Locations

            // Set the forms active location based on previous save.
            this.Location = CoreKeepersWorkshop.Properties.Settings.Default.ItemEditorLocation;
            #endregion

            #region Tooltips

            // Create a new tooltip.
            ToolTip toolTip = new ToolTip();
            toolTip.AutoPopDelay = 5000;
            toolTip.InitialDelay = 1500;

            // Set tool texts.
            toolTip.SetToolTip(numericUpDown1, "Enter the amount of items to add.");
            toolTip.SetToolTip(numericUpDown2, "Enter a custom ID. Press enter when done.");
            toolTip.SetToolTip(numericUpDown3, "Enter a custom variant ID. Press enter when done.");
            toolTip.SetToolTip(numericUpDown4, "Enter an ingredient ID. Press enter when done.");

            toolTip.SetToolTip(button3, "Toggle the GUI between food / item variaty.");
            toolTip.SetToolTip(button5, "Remove the item from this inventory slot.");

            #endregion

            // Load some settings.
            if (CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation.ToString().Length == 8) // Check if item is a food variant.
            {
                // Change some form items.
                numericUpDown3.Visible = false;
                numericUpDown4.Visible = true;
                numericUpDown5.Visible = true;
                button4.Visible = true;

                // Update settings.
                numericUpDown1.Value = CoreKeepersWorkshop.Properties.Settings.Default.InfoID;
                numericUpDown2.Value = CoreKeepersWorkshop.Properties.Settings.Default.InfoAmount;
                numericUpDown4.Value = decimal.Parse(CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation.ToString().Substring(0, CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation.ToString().Length / 2));
                numericUpDown5.Value = decimal.Parse(CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation.ToString().Substring(CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation.ToString().Length / 2));

                // Rename button label.
                button3.Text = "Food Variant [or]";

                // Update tooltips.
                toolTip.SetToolTip(numericUpDown3, "Enter an ingredient ID or full variant ID on this side. Press enter when done.");
            }
            else
            {
                // None food variant, keep normal settings.
                numericUpDown1.Value = CoreKeepersWorkshop.Properties.Settings.Default.InfoID;
                numericUpDown2.Value = CoreKeepersWorkshop.Properties.Settings.Default.InfoAmount;
                numericUpDown3.Value = CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation;

                // Update tooltips.
                toolTip.SetToolTip(numericUpDown3, "Enter a custom variant ID. Press enter when done.");
            }
        }

        // Do closing events.
        private void ItemEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check if the "X" button was pressed to close form.
            if (!new StackTrace().GetFrames().Any(x => x.GetMethod().Name == "Close"))
            {
                // User pressed the "X" button cancle task.
                userCancledTask = true;
                this.Close();
            }

            // Save some form settings.
            if (!numericUpDown3.Visible) // Check if item is a food variant.
            {
                // Check if both entrees are populated.
                if (numericUpDown5.Value != 0)
                {
                    // Combine strings into int.
                    CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation = int.Parse(numericUpDown4.Value.ToString() + numericUpDown5.Value.ToString());
                }
                else
                {
                    // Only single value exists, treat as a unique variant value.
                    CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation = (int)numericUpDown4.Value;
                }
            }
            else
            {
                // Normal item variant.
                CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation = (int)numericUpDown3.Value;
            }
            CoreKeepersWorkshop.Properties.Settings.Default.InfoID = (int)numericUpDown1.Value;
            CoreKeepersWorkshop.Properties.Settings.Default.InfoAmount = (int)numericUpDown2.Value;
            CoreKeepersWorkshop.Properties.Settings.Default.ItemEditorLocation = this.Location;
        }
        #endregion // Form loading and closing events.

        #region Keydown Events

        // Do enter events.
        private void numericUpDown1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectedItemType = (int)numericUpDown1.Value;
                selectedItemAmount = (int)numericUpDown2.Value;
                if (!numericUpDown3.Visible) // Check if item is a food variant.
                {
                    // Check if both entrees are populated.
                    if (numericUpDown5.Value != 0)
                    {
                        // Combine strings into int.
                        selectedItemVariation = int.Parse(numericUpDown4.Value.ToString() + numericUpDown5.Value.ToString());
                    }
                    else
                    {
                        // Only single value exists, treat as a unique variant value.
                        selectedItemVariation = (int)numericUpDown4.Value;
                    }
                }
                else
                {
                    // Normal item variant.
                    selectedItemVariation = (int)numericUpDown3.Value;
                }
                this.Close();
            }
        }
        private void numericUpDown2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectedItemType = (int)numericUpDown1.Value;
                selectedItemAmount = (int)numericUpDown2.Value;
                if (!numericUpDown3.Visible) // Check if item is a food variant.
                {
                    // Check if both entrees are populated.
                    if (numericUpDown5.Value != 0)
                    {
                        // Combine strings into int.
                        selectedItemVariation = int.Parse(numericUpDown4.Value.ToString() + numericUpDown5.Value.ToString());
                    }
                    else
                    {
                        // Only single value exists, treat as a unique variant value.
                        selectedItemVariation = (int)numericUpDown4.Value;
                    }
                }
                else
                {
                    // Normal item variant.
                    selectedItemVariation = (int)numericUpDown3.Value;
                }
                this.Close();
            }
        }
        private void numericUpDown3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectedItemType = (int)numericUpDown1.Value;
                selectedItemAmount = (int)numericUpDown2.Value;
                if (!numericUpDown3.Visible) // Check if item is a food variant.
                {
                    // Check if both entrees are populated.
                    if (numericUpDown5.Value != 0)
                    {
                        // Combine strings into int.
                        selectedItemVariation = int.Parse(numericUpDown4.Value.ToString() + numericUpDown5.Value.ToString());
                    }
                    else
                    {
                        // Only single value exists, treat as a unique variant value.
                        selectedItemVariation = (int)numericUpDown4.Value;
                    }
                }
                else
                {
                    // Normal item variant.
                    selectedItemVariation = (int)numericUpDown3.Value;
                }
                this.Close();
            }
        }
        private void numericUpDown4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectedItemType = (int)numericUpDown1.Value;
                selectedItemAmount = (int)numericUpDown2.Value;
                if (!numericUpDown3.Visible) // Check if item is a food variant.
                {
                    // Check if both entrees are populated.
                    if (numericUpDown5.Value != 0)
                    {
                        // Combine strings into int.
                        selectedItemVariation = int.Parse(numericUpDown4.Value.ToString() + numericUpDown5.Value.ToString());
                    }
                    else
                    {
                        // Only single value exists, treat as a unique variant value.
                        selectedItemVariation = (int)numericUpDown4.Value;
                    }
                }
                else
                {
                    // Normal item variant.
                    selectedItemVariation = (int)numericUpDown3.Value;
                }
                this.Close();
            }
        }
        private void numericUpDown5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectedItemType = (int)numericUpDown1.Value;
                selectedItemAmount = (int)numericUpDown2.Value;
                if (!numericUpDown3.Visible) // Check if item is a food variant.
                {
                    // Check if both entrees are populated.
                    if (numericUpDown5.Value != 0)
                    {
                        // Combine strings into int.
                        selectedItemVariation = int.Parse(numericUpDown4.Value.ToString() + numericUpDown5.Value.ToString());
                    }
                    else
                    {
                        // Only single value exists, treat as a unique variant value.
                        selectedItemVariation = (int)numericUpDown4.Value;
                    }
                }
                else
                {
                    // Normal item variant.
                    selectedItemVariation = (int)numericUpDown3.Value;
                }
                this.Close();
            }
        }
        // Remove item.
        private void button5_Click(object sender, EventArgs e)
        {
            selectedItemType = 0;
            selectedItemAmount = 1;
            selectedItemVariation = 0;
            this.Close();
        }
        #endregion // Keydown Events.

        #region Form Controls
        // Toggle variant settings.
        private void button3_Click(object sender, EventArgs e)
        {
            // Check if item or food mode is enabled.
            if (!numericUpDown3.Visible)
            {
                // Enabled controls.
                numericUpDown3.Visible = true;

                // Disable controls.
                numericUpDown4.Visible = false;
                numericUpDown5.Visible = false;
                button4.Visible = false;

                // Rename button label.
                button3.Text = "Item Variant [or]";

                // Update item variant
                numericUpDown3.Value = numericUpDown4.Value;
            }
            else
            {
                // Enabled controls.
                numericUpDown4.Visible = true;
                numericUpDown5.Visible = true;
                button4.Visible = true;

                // Disable controls.

                numericUpDown3.Visible = false;

                // Rename button label.
                button3.Text = "Food Variant [or]";

                // Update food variant
                numericUpDown4.Value = numericUpDown3.Value;
            }
        }
        #endregion // End form controls.
    }
}
