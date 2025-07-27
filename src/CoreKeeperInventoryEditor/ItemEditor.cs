using CoreKeepersWorkshop.Properties;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using CoreKeeperInventoryEditor;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using System.Linq;
using System.IO;
using System;

namespace CoreKeepersWorkshop
{
    public partial class ItemEditor : Form
    {
        // Form initialization.
        private CustomFormStyler _formThemeStyler;
        public ItemEditor()
        {
            InitializeComponent();
            Load += (_, __) => _formThemeStyler = this.ApplyTheme(); // Load the forms theme.
        }

        #region Fields

        // Define texture data.
        private static string InventoryDir => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "Inventory");
        public IEnumerable<string> ImageFiles =>
            Directory.Exists(InventoryDir)
                ? Directory.EnumerateFiles(InventoryDir, "*.png", SearchOption.AllDirectories)
                    .DefaultIfEmpty(string.Empty)
                : new[] { string.Empty };

        // Maps an item/variation id (string) -> absolute PNG path (string).
        // Built once when the type is first touched, then reused by every lookup.
        private static readonly Dictionary<string, string> _imagePathById =
            Directory.Exists(InventoryDir)
                ? Directory
                    .EnumerateFiles(InventoryDir, "*.png", SearchOption.AllDirectories)
                    .Where(f => {
                        var name = Path.GetFileName(f);
                        return !name.Equals("desktop.ini", StringComparison.OrdinalIgnoreCase)
                            && !name.Equals("Thumbs.db", StringComparison.OrdinalIgnoreCase);
                    })
                    .Select(f => new {
                        parts = Path.GetFileNameWithoutExtension(f).Split(','),
                        file = f
                    })
                    .Where(x => x.parts.Length >= 2)
                    .GroupBy(x => x.parts[1])
                    .ToDictionary(
                        g => g.Key,
                        g => g.First().file
                    )
                : new Dictionary<string, string>();

        #endregion

        #region Closing Varibles

        // Form closing saving.
        int selectedItemType      = 0;
        int  selectedItemAmount    = 0;
        int  selectedItemVariation = 0;
        int  selectedItemSkillset  = 0;
        bool userCanceldTask       = false;

        // Define closing variables.
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
        public int GetItemSkillsetFromList()
        {
            return selectedItemSkillset;
        }
        public bool GetUserCanceldTask()
        {
            return userCanceldTask;
        }
        #endregion

        #region Reload Pictureboxes & Labels

        
        // Helper to set one slot.
        private void SetSlot(PictureBox box, Label label, string id)
        {
            if (string.IsNullOrEmpty(id) || id == "0")
            {
                box.Image    = null;
                label.Text   = "";
            }
            else if (_imagePathById.TryGetValue(id, out var path))
            {
                var name     = Path.GetFileNameWithoutExtension(path).Split(',')[0];
                label.Text   = name;
                box.Image    = new Bitmap(ImageFast.FromFile(path));
                box.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                label.Text   = "UnknownItem";
                box.Image    = Resources.UnknownItem;
                box.SizeMode = PictureBoxSizeMode.Zoom;
            }
        }

        // Reload pictureboxes and labels.
        public void ReloadPictureBoxes(bool useTextboxData = false)
        {
            // base IDs
            string id1 = Settings.Default.InfoID.ToString();
            string id2 = "0";
            string id3 = "0";

            if (useTextboxData)
            {
                id1 = ((int)ItemID_NumericUpDown.Value).ToString();

                if (VariationNumerical_NumericUpDown.Visible)
                {
                    // Simple variant.
                    id2 = VariationNumerical_NumericUpDown.Value.ToString();
                }
                else
                {
                    id2 = Variation1_NumericUpDown.Value.ToString();
                    id3 = Variation2_NumericUpDown.Value.ToString();
                }
            }
            else
            {
                // Settings-based "food" variant IDs.
                int variation = Settings.Default.InfoVariation;
                if (variation >= 1)
                {
                    id2 = VariationHelper.GetIngredient1FromFoodVariation(variation).ToString();
                    if (!VariationNumerical_NumericUpDown.Visible)
                        id3 = VariationHelper.GetIngredient2FromFoodVariation(variation).ToString();
                }
            }

            // Apply to each slot.
            SetSlot(Slot1_PictureBox, Item1_Label, id1);
            SetSlot(Slot2_PictureBox, Item2_Label, id2);
            SetSlot(Slot3_PictureBox, Item3_Label, id3);

            // Refresh.
            Slot1_PictureBox.Invalidate();
            Slot2_PictureBox.Invalidate();
            Slot3_PictureBox.Invalidate();
        }
        #endregion

        #region Form Load And Closing Events

        // Do loading events.
        private void ItemEditor_Load(object sender, EventArgs e)
        {
            #region Set Custom Cusror

            // Set the applications cursor.
            Cursor = new Cursor(CoreKeepersWorkshop.Properties.Resources.UICursor.GetHicon());
            #endregion

            #region Set Form Opacity

            // Set form opacity based on trackbars value saved setting (1 to 100 -> 0.01 to 1.0).
            this.Opacity = Settings.Default.FormOpacity / 100.0;
            #endregion

            #region Tooltips

            // Create a new tooltip.
            ToolTip toolTip = new ToolTip()
            {
                AutoPopDelay = 5000,
                InitialDelay = 750
            };

            // Set tool texts.
            toolTip.SetToolTip(ItemID_NumericUpDown,             "Enter the ID you wish to add. Press enter when done.");
            toolTip.SetToolTip(Quantity_NumericUpDown,           "Enter a custom item quantity. Press enter when done.");
            toolTip.SetToolTip(VariationNumerical_NumericUpDown, "Enter a custom variant ID. Press enter when done.");
            toolTip.SetToolTip(Variation1_NumericUpDown,         "Enter an ingredient one ID. Press enter when done.");
            toolTip.SetToolTip(Variation2_NumericUpDown,         "Enter an ingredient two ID. Press enter when done.");
            toolTip.SetToolTip(CustomQuantity1_NumericUpDown,    "Press the enter key when finished.");
            toolTip.SetToolTip(CustomQuantity2_NumericUpDown,    "Press the enter key when finished.");
            toolTip.SetToolTip(CustomQuantity3_NumericUpDown,    "Press the enter key when finished.");
            toolTip.SetToolTip(CustomQuantity4_NumericUpDown,    "Press the enter key when finished.");
            toolTip.SetToolTip(CustomQuantity5_NumericUpDown,    "Press the enter key when finished.");
            toolTip.SetToolTip(Skillset_NumericUpDown,           "Enter a custom skillset ID. Either press enter when done or use the button.");

            toolTip.SetToolTip(Variation_Label,                  "Toggle the GUI between food / item variety.");

            toolTip.SetToolTip(ChangeRarity_Button,              "Change your food rarity. Press enter when done.");
            toolTip.SetToolTip(Done_Button,                      "Finish editing the item.");
            toolTip.SetToolTip(RemoveItem_Button,                "Remove the item from this inventory slot.");
            toolTip.SetToolTip(CookedFoodList_Button,            "Open the food cookbook to easily search for food items.");
            toolTip.SetToolTip(CustomQuantity1_Button,           "Quick change the items quantity. Right click to edit.");
            toolTip.SetToolTip(CustomQuantity2_Button,           "Quick change the items quantity. Right click to edit.");
            toolTip.SetToolTip(CustomQuantity3_Button,           "Quick change the items quantity. Right click to edit.");
            toolTip.SetToolTip(CustomQuantity4_Button,           "Quick change the items quantity. Right click to edit.");
            toolTip.SetToolTip(CustomQuantity5_Button,           "Quick change the items quantity. Right click to edit.");
            toolTip.SetToolTip(SkillsetHelp_Button,              "Launch a guide on how to find skillset IDs.");

            toolTip.SetToolTip(Slot1_PictureBox,                 "Click to open the item explorer.");
            toolTip.SetToolTip(Slot2_PictureBox,                 "Click to open the item explorer.");
            toolTip.SetToolTip(Slot3_PictureBox,                 "Click to open the item explorer.");
            #endregion

            #region Load Form Settings

            // Load quantity select numerics and buttons.
            CustomQuantity1_Button.Text         = Settings.Default.QuantitySelectGetInventoryAddresses_Button.ToString();
            CustomQuantity1_NumericUpDown.Value = Settings.Default.QuantitySelectGetInventoryAddresses_Button;
            CustomQuantity2_Button.Text         = Settings.Default.QuantitySelectButton2.ToString();
            CustomQuantity2_NumericUpDown.Value = Settings.Default.QuantitySelectButton2;
            CustomQuantity3_Button.Text         = Settings.Default.QuantitySelectButton3.ToString();
            CustomQuantity3_NumericUpDown.Value = Settings.Default.QuantitySelectButton3;
            CustomQuantity4_Button.Text         = Settings.Default.QuantitySelectButton4.ToString();
            CustomQuantity4_NumericUpDown.Value = Settings.Default.QuantitySelectButton4;
            CustomQuantity5_Button.Text         = Settings.Default.QuantitySelectButton5.ToString();
            CustomQuantity5_NumericUpDown.Value = Settings.Default.QuantitySelectButton5;

            // Ensure the quantity is more then zero. // Fix v1.3.5.4.
            if (Settings.Default.InfoAmount   < 1)
            {
                Settings.Default.InfoAmount   = 1;
            }
            // Ensure the skillset is more then -1.
            if (Settings.Default.InfoSkillset < 0)
            {
                MessageBox.Show("The skillset was lower then 0! -> Current value: " + Settings.Default.InfoSkillset + "\n\nValue will be set to 0.", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Settings.Default.InfoSkillset = 0;
            }

            // Load some settings.
            if (Settings.Default.InfoVariation >= 1) // Check if item is a food variant.
            {
                // Change some form items.
                VariationNumerical_NumericUpDown.Visible = false;
                Variation1_NumericUpDown.Visible         = true;
                Variation2_NumericUpDown.Visible         = true;
                VariationAnd_Button.Visible              = true;

                // Update settings.
                ItemID_NumericUpDown.Value   = Settings.Default.InfoID;
                Quantity_NumericUpDown.Value = Settings.Default.InfoAmount;

                // New format. // Fix v1.3.5.6.
                Variation1_NumericUpDown.Value = VariationHelper.GetIngredient1FromFoodVariation(Settings.Default.InfoVariation);
                Variation2_NumericUpDown.Value = VariationHelper.GetIngredient2FromFoodVariation(Settings.Default.InfoVariation);

                // Ensure the largest value loads in the front.
                // if (num5 > num4)
                // {
                //     // Flip values.
                //     numericUpDown5.Value = num4;
                //     numericUpDown4.Value = num5;
                // }
                // else
                // {
                //     numericUpDown4.Value = num4;
                //     numericUpDown5.Value = num5;
                // }

                // Legacy format.
                // numericUpDown4.Value = decimal.Parse(Settings.Default.InfoVariation.ToString().Substring(0, Settings.Default.InfoVariation.ToString().Length / 2));
                // numericUpDown5.Value = decimal.Parse(Settings.Default.InfoVariation.ToString().Substring(Settings.Default.InfoVariation.ToString().Length / 2));

                Skillset_NumericUpDown.Value = Settings.Default.InfoSkillset;

                // Rename button label.
                Variation_Label.Text = "Variation [Food Ingredients]";
            }
            else
            {
                // None food variant, keep normal settings.
                ItemID_NumericUpDown.Value             = Settings.Default.InfoID;
                Quantity_NumericUpDown.Value           = Settings.Default.InfoAmount;
                VariationNumerical_NumericUpDown.Value = Settings.Default.InfoVariation;
                Skillset_NumericUpDown.Value           = Settings.Default.InfoSkillset;
            }
            #endregion

            #region Load Pictures & Names

            // Reload all pictureboxes and labels from the default load data.
            ReloadPictureBoxes(useTextboxData: true);
            #endregion

            #region Set Form Locations

            // Set the forms active location based on previous save.
            if (ActiveForm != null) this.Location = Settings.Default.ItemEditorLocation;
            #endregion
        }

        #region Form Closing

        // Do closing events.
        private void ItemEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check if the "X" button was pressed to close form.
            // if (!new StackTrace().GetFrames().Any(x => x.GetMethod().Name == "Close"))
            if (_formThemeStyler.CloseButtonPressed) // Now capture the custom titlebar.
            {
                // User pressed the "X" button cancel task.
                userCanceldTask = true;
                // this.Close();
            }

            // Ensure we catch all closing exceptions. // Fix v1.3.3.
            try
            {
                // Save some form settings.
                if (!VariationNumerical_NumericUpDown.Visible) // Check if item is a food variant.
                {
                    // Check if both entrees are populated.
                    if (Variation2_NumericUpDown.Value != 0)
                    {
                        // Do some checks and corrections for values over 8. // Fix v1.3.5.1.
                        // if (numericUpDown4.Value > numericUpDown5.Value)
                        // {
                        //     // Flip values.
                        //     decimal item2 = numericUpDown4.Value;
                        //     decimal item3 = numericUpDown5.Value;
                        // 
                        //     numericUpDown4.Value = item3;
                        //     numericUpDown5.Value = item2;
                        // }

                        // Combine strings into int. // Fix v1.3.5.6.
                        Settings.Default.InfoVariation = VariationHelper.GetFoodVariation((int)Variation1_NumericUpDown.Value, (int)Variation2_NumericUpDown.Value);

                        // Legacy format.
                        // Settings.Default.InfoVariation = int.Parse(numericUpDown4.Value.ToString() + numericUpDown5.Value.ToString());
                    }
                    else
                    {
                        // Only single value exists, treat as a unique variant value.
                        Settings.Default.InfoVariation = (int)Variation1_NumericUpDown.Value;
                    }
                }
                else
                {
                    // Normal item variant.
                    Settings.Default.InfoVariation  = (int)VariationNumerical_NumericUpDown.Value;
                }
                Settings.Default.InfoID             = (int)ItemID_NumericUpDown.Value;
                Settings.Default.InfoAmount         = (int)Quantity_NumericUpDown.Value;
                Settings.Default.InfoSkillset       = (int)Skillset_NumericUpDown.Value;
                Settings.Default.ItemEditorLocation = this.Location;
            }
            catch (Exception)
            { } // Do nothing.
        }
        #endregion

        #endregion // Form loading and closing events.

        #region Keydown Events

        // Do enter events.
        private void ItemID_NumericUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectedItemType = (int)ItemID_NumericUpDown.Value;
                selectedItemAmount = (int)Quantity_NumericUpDown.Value;
                selectedItemSkillset = (int)Skillset_NumericUpDown.Value;
                if (!VariationNumerical_NumericUpDown.Visible) // Check if item is a food variant.
                {
                    // Check if both entrees are populated.
                    if (Variation2_NumericUpDown.Value != 0)
                    {
                        // Do some checks and corrections for values over 8. // Fix v1.3.5.1.
                        // if (numericUpDown4.Value > numericUpDown5.Value)
                        // {
                        //     // Flip values.
                        //     decimal item2 = numericUpDown4.Value;
                        //     decimal item3 = numericUpDown5.Value;
                        // 
                        //     numericUpDown4.Value = item3;
                        //     numericUpDown5.Value = item2;
                        // }

                        // Combine strings into int. // Fix v1.3.5.6. // Fix v1.3.5.7 - Added the math to this overlooked function.
                        selectedItemVariation = VariationHelper.GetFoodVariation((int)Variation1_NumericUpDown.Value, (int)Variation2_NumericUpDown.Value);

                        // Legacy format.
                        // selectedItemVariation = int.Parse(numericUpDown4.Value.ToString() + numericUpDown5.Value.ToString());
                    }
                    else
                    {
                        // Only single value exists, treat as a unique variant value.
                        selectedItemVariation = (int)Variation1_NumericUpDown.Value;
                    }
                }
                else
                {
                    // Normal item variant.
                    selectedItemVariation = (int)VariationNumerical_NumericUpDown.Value;
                }
                this.Close();
            }
        }
        private void Quantity_NumericUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectedItemType = (int)ItemID_NumericUpDown.Value;
                selectedItemAmount = (int)Quantity_NumericUpDown.Value;
                selectedItemSkillset = (int)Skillset_NumericUpDown.Value;
                if (!VariationNumerical_NumericUpDown.Visible) // Check if item is a food variant.
                {
                    // Check if both entrees are populated.
                    if (Variation2_NumericUpDown.Value != 0)
                    {
                        // Do some checks and corrections for values over 8. // Fix v1.3.5.1.
                        // if (numericUpDown4.Value > numericUpDown5.Value)
                        // {
                        //     // Flip values.
                        //     decimal item2 = numericUpDown4.Value;
                        //     decimal item3 = numericUpDown5.Value;
                        // 
                        //     numericUpDown4.Value = item3;
                        //     numericUpDown5.Value = item2;
                        // }

                        // Combine strings into int. // Fix v1.3.5.6. // Fix v1.3.5.7 - Added the math to this overlooked function.
                        selectedItemVariation = VariationHelper.GetFoodVariation((int)Variation1_NumericUpDown.Value, (int)Variation2_NumericUpDown.Value);

                        // Legacy format.
                        // selectedItemVariation = int.Parse(numericUpDown4.Value.ToString() + numericUpDown5.Value.ToString());
                    }
                    else
                    {
                        // Only single value exists, treat as a unique variant value.
                        selectedItemVariation = (int)Variation1_NumericUpDown.Value;
                    }
                }
                else
                {
                    // Normal item variant.
                    selectedItemVariation = (int)VariationNumerical_NumericUpDown.Value;
                }
                this.Close();
            }
        }
        private void VariationNumerical_NumericUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectedItemType = (int)ItemID_NumericUpDown.Value;
                selectedItemAmount = (int)Quantity_NumericUpDown.Value;
                selectedItemSkillset = (int)Skillset_NumericUpDown.Value;
                if (!VariationNumerical_NumericUpDown.Visible) // Check if item is a food variant.
                {
                    // Check if both entrees are populated.
                    if (Variation2_NumericUpDown.Value != 0)
                    {
                        // Do some checks and corrections for values over 8. // Fix v1.3.5.1.
                        if (Variation1_NumericUpDown.Value > Variation2_NumericUpDown.Value)
                        {
                            // Flip values.
                            decimal item2 = Variation1_NumericUpDown.Value;
                            decimal item3 = Variation2_NumericUpDown.Value;

                            Variation1_NumericUpDown.Value = item3;
                            Variation2_NumericUpDown.Value = item2;
                        }

                        // Combine strings into int. // Fix v1.3.5.6. // Fix v1.3.5.7 - Added the math to this overlooked function.
                        selectedItemVariation = VariationHelper.GetFoodVariation((int)Variation1_NumericUpDown.Value, (int)Variation2_NumericUpDown.Value);

                        // Legacy format.
                        // selectedItemVariation = int.Parse(numericUpDown4.Value.ToString() + numericUpDown5.Value.ToString());
                    }
                    else
                    {
                        // Only single value exists, treat as a unique variant value.
                        selectedItemVariation = (int)Variation1_NumericUpDown.Value;
                    }
                }
                else
                {
                    // Normal item variant.
                    selectedItemVariation = (int)VariationNumerical_NumericUpDown.Value;
                }
                this.Close();
            }
        }
        private void Variation1_NumericUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectedItemType = (int)ItemID_NumericUpDown.Value;
                selectedItemAmount = (int)Quantity_NumericUpDown.Value;
                selectedItemSkillset = (int)Skillset_NumericUpDown.Value;
                if (!VariationNumerical_NumericUpDown.Visible) // Check if item is a food variant.
                {
                    // Check if both entrees are populated.
                    if (Variation2_NumericUpDown.Value != 0)
                    {
                        // Do some checks and corrections for values over 8. // Fix v1.3.5.1.
                        // if (numericUpDown4.Value > numericUpDown5.Value)
                        // {
                        //     // Flip values.
                        //     decimal item2 = numericUpDown4.Value;
                        //     decimal item3 = numericUpDown5.Value;
                        // 
                        //     numericUpDown4.Value = item3;
                        //     numericUpDown5.Value = item2;
                        // }

                        // Combine strings into int. // Fix v1.3.5.6. // Fix v1.3.5.7 - Added the math to this overlooked function.
                        selectedItemVariation = VariationHelper.GetFoodVariation((int)Variation1_NumericUpDown.Value, (int)Variation2_NumericUpDown.Value);

                        // Legacy format.
                        // selectedItemVariation = int.Parse(numericUpDown4.Value.ToString() + numericUpDown5.Value.ToString());
                    }
                    else
                    {
                        // Only single value exists, treat as a unique variant value.
                        selectedItemVariation = (int)Variation1_NumericUpDown.Value;
                    }
                }
                else
                {
                    // Normal item variant.
                    selectedItemVariation = (int)VariationNumerical_NumericUpDown.Value;
                }
                this.Close();
            }
        }
        private void Variation2_NumericUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectedItemType = (int)ItemID_NumericUpDown.Value;
                selectedItemAmount = (int)Quantity_NumericUpDown.Value;
                selectedItemSkillset = (int)Skillset_NumericUpDown.Value;
                if (!VariationNumerical_NumericUpDown.Visible) // Check if item is a food variant.
                {
                    // Check if both entrees are populated.
                    if (Variation2_NumericUpDown.Value != 0)
                    {
                        // Do some checks and corrections for values over 8. // Fix v1.3.5.1.
                        // if (numericUpDown4.Value > numericUpDown5.Value)
                        // {
                        //     // Flip values.
                        //     decimal item2 = numericUpDown4.Value;
                        //     decimal item3 = numericUpDown5.Value;
                        // 
                        //     numericUpDown4.Value = item3;
                        //     numericUpDown5.Value = item2;
                        // }

                        // Combine strings into int. // Fix v1.3.5.6. // Fix v1.3.5.7 - Added the math to this overlooked function.
                        selectedItemVariation = VariationHelper.GetFoodVariation((int)Variation1_NumericUpDown.Value, (int)Variation2_NumericUpDown.Value);

                        // Legacy format.
                        // selectedItemVariation = int.Parse(numericUpDown4.Value.ToString() + numericUpDown5.Value.ToString());
                    }
                    else
                    {
                        // Only single value exists, treat as a unique variant value.
                        selectedItemVariation = (int)Variation1_NumericUpDown.Value;
                    }
                }
                else
                {
                    // Normal item variant.
                    selectedItemVariation = (int)VariationNumerical_NumericUpDown.Value;
                }
                this.Close();
            }
        }
        private void Skillset_Button_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectedItemType = (int)ItemID_NumericUpDown.Value;
                selectedItemAmount = (int)Quantity_NumericUpDown.Value;
                selectedItemSkillset = (int)Skillset_NumericUpDown.Value;
                if (!VariationNumerical_NumericUpDown.Visible) // Check if item is a food variant.
                {
                    // Check if both entrees are populated.
                    if (Variation2_NumericUpDown.Value != 0)
                    {
                        // Do some checks and corrections for values over 8. // Fix v1.3.5.1.
                        // if (numericUpDown4.Value > numericUpDown5.Value)
                        // {
                        //     // Flip values.
                        //     decimal item2 = numericUpDown4.Value;
                        //     decimal item3 = numericUpDown5.Value;
                        // 
                        //     numericUpDown4.Value = item3;
                        //     numericUpDown5.Value = item2;
                        // }

                        // Combine strings into int. // Fix v1.3.5.6. // Fix v1.3.5.7 - Added the math to this overlooked function.
                        selectedItemVariation = VariationHelper.GetFoodVariation((int)Variation1_NumericUpDown.Value, (int)Variation2_NumericUpDown.Value);

                        // Legacy format.
                        // selectedItemVariation = int.Parse(numericUpDown4.Value.ToString() + numericUpDown5.Value.ToString());
                    }
                    else
                    {
                        // Only single value exists, treat as a unique variant value.
                        selectedItemVariation = (int)Variation1_NumericUpDown.Value;
                    }
                }
                else
                {
                    // Normal item variant.
                    selectedItemVariation = (int)VariationNumerical_NumericUpDown.Value;
                }
                this.Close();
            }
        }

        // Remove item.
        private void RemoveItem_Button_Click(object sender, EventArgs e)
        {
            selectedItemType = 0;
            selectedItemAmount = 1;
            selectedItemVariation = 0;
            selectedItemSkillset = 0;
            this.Close();
        }
        #endregion // Keydown Events.

        #region Form Controls
        // Toggle variant settings.
        private void Variation_Label_Click(object sender, EventArgs e)
        {
            // Check if item or food mode is enabled.
            if (!VariationNumerical_NumericUpDown.Visible)
            {
                // Enabled controls.
                VariationNumerical_NumericUpDown.Visible = true;

                // Disable controls.
                Variation1_NumericUpDown.Visible = false;
                Variation2_NumericUpDown.Visible = false;
                VariationAnd_Button.Visible = false;

                // Rename button label.
                Variation_Label.Text = "Variation [Raw]";

                // Update item variant
                VariationNumerical_NumericUpDown.Value = VariationHelper.GetFoodVariation((int)Variation1_NumericUpDown.Value, (int)Variation2_NumericUpDown.Value);

                // Reload all pictureboxes and labels.
                ReloadPictureBoxes(useTextboxData: true);
            }
            else
            {
                // Enabled controls.
                Variation1_NumericUpDown.Visible = true;
                Variation2_NumericUpDown.Visible = true;
                VariationAnd_Button.Visible = true;

                // Disable controls.

                VariationNumerical_NumericUpDown.Visible = false;

                // Rename button label.
                Variation_Label.Text = "Variation [Food Ingredients]";

                // Update food variant
                Variation1_NumericUpDown.Value = VariationHelper.GetIngredient1FromFoodVariation((int)VariationNumerical_NumericUpDown.Value);
                Variation2_NumericUpDown.Value = VariationHelper.GetIngredient2FromFoodVariation((int)VariationNumerical_NumericUpDown.Value);

                // Reload all pictureboxes and labels.
                ReloadPictureBoxes(useTextboxData: true);
            }
        }

        // Open food cookbook.
        private void CookedFoodList_Button_Click(object sender, EventArgs e)
        {
            // Spawn food cookbook window.
            FoodCookbook foodCookbook = new FoodCookbook();
            DialogResult dr = foodCookbook.ShowDialog(this);

            // Get returned item from picker.
            int itemType      = foodCookbook.GetItemTypeFromList();
            int itemAmount    = foodCookbook.GetItemAmountFromList();
            int itemVariation = foodCookbook.GetItemVeriationFromList() == 0 ? 0 : (foodCookbook.GetItemVeriationFromList()); // If variation is not zero, add offset.
            int itemSkillset  = foodCookbook.GetItemSkillsetFromList()  == 0 ? 0 : (foodCookbook.GetItemSkillsetFromList());  // If skillset is not zero, add offset.
            bool wasAborted   = foodCookbook.GetUserCanceldTask();
            // bool itemOverwrite = foodCookbook.GetSelectedOverwriteTask();
            foodCookbook.Close();

            // Check if user closed the form
            if (wasAborted) { return; };

            // Set the values from returning form.
            selectedItemType      = itemType;
            selectedItemAmount    = itemAmount;
            selectedItemVariation = itemVariation;
            selectedItemSkillset  = itemSkillset;
            this.Close();
        }

        // Value had changed, reload images and labels.
        private void VariationNumerical_NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            // Reload all pictureboxes and labels from the textboxe data.
            ReloadPictureBoxes(useTextboxData: true);
        }

        // Value had changed, reload images and labels.
        private void Variation1_NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            // Reload all pictureboxes and labels from the textboxe data.
            ReloadPictureBoxes(useTextboxData: true);
        }

        // Value had changed, reload images and labels.
        private void Variation2_NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            // Reload all pictureboxes and labels from the textboxe data.
            ReloadPictureBoxes(useTextboxData: true);
        }

        // Value had changed, reload images and labels.
        private void ItemID_NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            // Reload all pictureboxes and labels from the textboxe data.
            ReloadPictureBoxes(useTextboxData: true);
        }

        // Change item rarity.
        private void ChangeRarity_Button_Click(object sender, EventArgs e)
        {
            string originalName   = "Unknown";
            string originalRarity = "Uncommon";
            int originalBase = (int)ItemID_NumericUpDown.Value;
            bool foundNewRarity = false;

            int originalVariation;
            if (!VariationNumerical_NumericUpDown.Visible) // Check if item is a food variant.
            {
                // Check if both entrees are populated.
                if (Variation2_NumericUpDown.Value != 0)
                {
                    // Combine strings into int.
                    originalVariation = VariationHelper.GetFoodVariation((int)Variation1_NumericUpDown.Value, (int)Variation2_NumericUpDown.Value);
                }
                else
                {
                    // Only single value exists, treat as a unique variant value.
                    originalVariation = VariationHelper.GetFoodVariation((int)Variation1_NumericUpDown.Value, 0);
                }
            }
            else
            {
                // Normal item variant.
                originalVariation = (int)VariationNumerical_NumericUpDown.Value;
            }

            // Ensure original variation is 8 in length.
            if (originalVariation >= 1)
            {
                // Get the original items prefix.
                // Get json file from resources.
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CoreKeepersWorkshop.Resources.Cookbook.json"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    // Convert stream into string.
                    var jsonFileContent = reader.ReadToEnd();
                    dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonFileContent);

                    // Load each object from json to a string array.
                    foreach (var file in result)
                    {
                        // Remove spaces from food names.
                        string foodVariation = (string)file.variation;
                        string foodID = (string)file.id;
                        string foodName = (string)file.name;
                        foodName = new Regex("[ ]{2,}", RegexOptions.None).Replace(foodName, " ");

                        // Check if the foodname matches.
                        if (foodID == originalBase.ToString() && foodVariation == originalVariation.ToString())
                        {
                            // Split the name based on properties.
                            string foodRarity = (foodName.Split(' ')[0].ToString() == "Rare" || foodName.Split(' ')[0].ToString() == "Epic") ? foodName.Split(' ')[0].ToString() : "Uncommon";
                            string splitFoodName = foodName.Replace("Epic ", "").Replace("Rare ", "");

                            // Set the prefix on the found item.
                            originalRarity = foodRarity;

                            // Set the food name of the found item.
                            originalName = splitFoodName;

                            // End loop.
                            break;
                        }
                    }
                }

                // Determine the next prefix we need to match.
                string nextRarity;
                if (originalRarity == "Uncommon")
                {
                    nextRarity = "Rare";
                }
                else if (originalRarity == "Rare")
                {
                    nextRarity = "Epic";
                }
                else if (originalRarity == "Epic")
                {
                    nextRarity = "Uncommon";
                }
                else
                {
                    // This should not happen.
                    nextRarity = "Uncommon";
                }

                // Change the items prefix.
                // Get json file from resources.
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CoreKeepersWorkshop.Resources.Cookbook.json"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    // Convert stream into string.
                    var jsonFileContent = reader.ReadToEnd();
                    dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonFileContent);

                    // Load each object from json to a string array.
                    foreach (var file in result)
                    {
                        // Remove spaces from food names.
                        string foodVariation = (string)file.variation;
                        string foodID = (string)file.id;
                        string foodName = (string)file.name;
                        foodName = new Regex("[ ]{2,}", RegexOptions.None).Replace(foodName, " ");

                        // Split the name based on properties.
                        string splitFoodName = foodName.Replace("Epic ", "").Replace("Rare ", "");

                        // Check if the foodname and variation matches.
                        if (splitFoodName == originalName && foodVariation == originalVariation.ToString())
                        {
                            // Get the food rarity.
                            string foodRarity = (foodName.Split(' ')[0].ToString() == "Rare" || foodName.Split(' ')[0].ToString() == "Epic") ? foodName.Split(' ')[0].ToString() : "Uncommon";

                            // Check the current prefix matches.
                            if (foodRarity == nextRarity)
                            {
                                // Set the values from the next found food.
                                // Update found bool.
                                foundNewRarity = true;

                                // Update base id.
                                ItemID_NumericUpDown.Value = int.Parse(foodID);

                                // Update variation.
                                if (!VariationNumerical_NumericUpDown.Visible) // Check if item is a food variant.
                                {
                                    // Populate both textboxes.
                                    Variation1_NumericUpDown.Value = VariationHelper.GetIngredient1FromFoodVariation(int.Parse(foodVariation));
                                    Variation2_NumericUpDown.Value = VariationHelper.GetIngredient2FromFoodVariation(int.Parse(foodVariation));

                                    // Legacy format.
                                    // numericUpDown4.Value = int.Parse(foodVariation.Substring(0, foodVariation.Length / 2));
                                    // numericUpDown5.Value = int.Parse(foodVariation.Substring(foodVariation.ToString().Length / 2));
                                }
                                else
                                {
                                    // Normal item variant.
                                    VariationNumerical_NumericUpDown.Value = int.Parse(foodVariation);
                                }

                                // Reload pictureboxes and labels.
                                ReloadPictureBoxes(useTextboxData: true);

                                // End loop.
                                break;
                            }
                        }
                    }
                }
            }

            // Unfocus the button.
            this.ActiveControl = ItemID_NumericUpDown;

            // Check if a new item rarity was found or not.
            if (!foundNewRarity)
            {
                MessageBox.Show("No alternative rarity found for this item.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Launch about skillset message.
        private void SkillsetHelp_Button_Click(object sender, EventArgs e)
        {
            MessageBox.Show("How to find my pet's skillset ID?\r\n    - Each pet only has 4 skillsets and are typically +4/-4 the current pets skillset. For example, if your pet has an assigned skillset of 150 then the 4 skillset IDs will be within the 146-154 range. For fresh pets first name the pet and it will be assigned a skillset id.\r\n\r\nWhy is it like this?\r\n    - The game generates progressive \"skill tables\" rather than using a static ID for a pets skills. Bad system I know, nothing I can do about it.\r\n", "How To Use --> Get Skillset ID", MessageBoxButtons.OK, MessageBoxIcon.Question);
            return;
        }
        #endregion // End form controls.

        #region Click Events

        #region Quick Quantity Selector.

        // Quick change the items quantity to 1.
        private void CustomQuantity1_Button_Click(object sender, EventArgs e)
        {
            // Change the existing items quantity. 
            selectedItemType = (int)ItemID_NumericUpDown.Value;
            selectedItemAmount = (int)CustomQuantity1_NumericUpDown.Value; // QQS Button 1.
            selectedItemSkillset = (int)Skillset_NumericUpDown.Value;

            // Reserve the items variation.
            if (!VariationNumerical_NumericUpDown.Visible) // Check if item is a food variant.
            {
                // Check if both entrees are populated.
                if (Variation2_NumericUpDown.Value != 0)
                {
                    // Do some checks and corrections for values over 8. // Fix v1.3.5.1.
                    // if (numericUpDown4.Value > numericUpDown5.Value)
                    // {
                    //     // Flip values.
                    //     decimal item2 = numericUpDown4.Value;
                    //     decimal item3 = numericUpDown5.Value;
                    // 
                    //     numericUpDown4.Value = item3;
                    //     numericUpDown5.Value = item2;
                    // }

                    // Combine strings into int. // Fix v1.3.5.6. // Fix v1.3.5.7 - Added the math to this overlooked function.
                    selectedItemVariation = VariationHelper.GetFoodVariation((int)Variation1_NumericUpDown.Value, (int)Variation2_NumericUpDown.Value);

                    // Legacy format.
                    // selectedItemVariation = int.Parse(numericUpDown4.Value.ToString() + numericUpDown5.Value.ToString());
                }
                else
                {
                    // Only single value exists, treat as a unique variant value.
                    selectedItemVariation = (int)Variation1_NumericUpDown.Value;
                }
            }
            else
            {
                // Normal item variant.
                selectedItemVariation = (int)VariationNumerical_NumericUpDown.Value;
            }
            this.Close();
        }

        // Quick change the items quantity to 50.
        private void CustomQuantity2_Button_Click(object sender, EventArgs e)
        {
            // Change the existing items quantity. 
            selectedItemType = (int)ItemID_NumericUpDown.Value;
            selectedItemAmount = (int)CustomQuantity2_NumericUpDown.Value; // QQS Button 2.
            selectedItemSkillset = (int)Skillset_NumericUpDown.Value;

            // Reserve the items variation.
            if (!VariationNumerical_NumericUpDown.Visible) // Check if item is a food variant.
            {
                // Check if both entrees are populated.
                if (Variation2_NumericUpDown.Value != 0)
                {
                    // Do some checks and corrections for values over 8. // Fix v1.3.5.1.
                    // if (numericUpDown4.Value > numericUpDown5.Value)
                    // {
                    //     // Flip values.
                    //     decimal item2 = numericUpDown4.Value;
                    //     decimal item3 = numericUpDown5.Value;
                    // 
                    //     numericUpDown4.Value = item3;
                    //     numericUpDown5.Value = item2;
                    // }

                    // Combine strings into int. // Fix v1.3.5.6. // Fix v1.3.5.7 - Added the math to this overlooked function.
                    selectedItemVariation = VariationHelper.GetFoodVariation((int)Variation1_NumericUpDown.Value, (int)Variation2_NumericUpDown.Value);

                    // Legacy format.
                    // selectedItemVariation = int.Parse(numericUpDown4.Value.ToString() + numericUpDown5.Value.ToString());
                }
                else
                {
                    // Only single value exists, treat as a unique variant value.
                    selectedItemVariation = (int)Variation1_NumericUpDown.Value;
                }
            }
            else
            {
                // Normal item variant.
                selectedItemVariation = (int)VariationNumerical_NumericUpDown.Value;
            }
            this.Close();
        }

        // Quick change the items quantity to 500.
        private void CustomQuantity3_Button_Click(object sender, EventArgs e)
        {
            // Change the existing items quantity. 
            selectedItemType = (int)ItemID_NumericUpDown.Value;
            selectedItemAmount = (int)CustomQuantity3_NumericUpDown.Value; // QQS Button 3.
            selectedItemSkillset = (int)Skillset_NumericUpDown.Value;

            // Reserve the items variation.
            if (!VariationNumerical_NumericUpDown.Visible) // Check if item is a food variant.
            {
                // Check if both entrees are populated.
                if (Variation2_NumericUpDown.Value != 0)
                {
                    // Do some checks and corrections for values over 8. // Fix v1.3.5.1.
                    // if (numericUpDown4.Value > numericUpDown5.Value)
                    // {
                    //     // Flip values.
                    //     decimal item2 = numericUpDown4.Value;
                    //     decimal item3 = numericUpDown5.Value;
                    // 
                    //     numericUpDown4.Value = item3;
                    //     numericUpDown5.Value = item2;
                    // }

                    // Combine strings into int. // Fix v1.3.5.6. // Fix v1.3.5.7 - Added the math to this overlooked function.
                    selectedItemVariation = VariationHelper.GetFoodVariation((int)Variation1_NumericUpDown.Value, (int)Variation2_NumericUpDown.Value);

                    // Legacy format.
                    // selectedItemVariation = int.Parse(numericUpDown4.Value.ToString() + numericUpDown5.Value.ToString());
                }
                else
                {
                    // Only single value exists, treat as a unique variant value.
                    selectedItemVariation = (int)Variation1_NumericUpDown.Value;
                }
            }
            else
            {
                // Normal item variant.
                selectedItemVariation = (int)VariationNumerical_NumericUpDown.Value;
            }
            this.Close();
        }

        // Quick change the items quantity to 5000.
        private void CustomQuantity4_Button_Click(object sender, EventArgs e)
        {
            // Change the existing items quantity. 
            selectedItemType = (int)ItemID_NumericUpDown.Value;
            selectedItemAmount = (int)CustomQuantity4_NumericUpDown.Value; // QQS Button 4.
            selectedItemSkillset = (int)Skillset_NumericUpDown.Value;

            // Reserve the items variation.
            if (!VariationNumerical_NumericUpDown.Visible) // Check if item is a food variant.
            {
                // Check if both entrees are populated.
                if (Variation2_NumericUpDown.Value != 0)
                {
                    // Do some checks and corrections for values over 8. // Fix v1.3.5.1.
                    // if (numericUpDown4.Value > numericUpDown5.Value)
                    // {
                    //     // Flip values.
                    //     decimal item2 = numericUpDown4.Value;
                    //     decimal item3 = numericUpDown5.Value;
                    // 
                    //     numericUpDown4.Value = item3;
                    //     numericUpDown5.Value = item2;
                    // }

                    // Combine strings into int. // Fix v1.3.5.6. // Fix v1.3.5.7 - Added the math to this overlooked function.
                    selectedItemVariation = VariationHelper.GetFoodVariation((int)Variation1_NumericUpDown.Value, (int)Variation2_NumericUpDown.Value);

                    // Legacy format.
                    // selectedItemVariation = int.Parse(numericUpDown4.Value.ToString() + numericUpDown5.Value.ToString());
                }
                else
                {
                    // Only single value exists, treat as a unique variant value.
                    selectedItemVariation = (int)Variation1_NumericUpDown.Value;
                }
            }
            else
            {
                // Normal item variant.
                selectedItemVariation = (int)VariationNumerical_NumericUpDown.Value;
            }
            this.Close();
        }

        // Quick change the items quantity to 9999.
        private void CustomQuantity5_Button_Click(object sender, EventArgs e)
        {
            // Change the existing items quantity. 
            selectedItemType = (int)ItemID_NumericUpDown.Value;
            selectedItemAmount = (int)CustomQuantity5_NumericUpDown.Value; // QQS Button 5.
            selectedItemSkillset = (int)Skillset_NumericUpDown.Value;

            // Reserve the items variation.
            if (!VariationNumerical_NumericUpDown.Visible) // Check if item is a food variant.
            {
                // Check if both entrees are populated.
                if (Variation2_NumericUpDown.Value != 0)
                {
                    // Do some checks and corrections for values over 8. // Fix v1.3.5.1.
                    // if (numericUpDown4.Value > numericUpDown5.Value)
                    // {
                    //     // Flip values.
                    //     decimal item2 = numericUpDown4.Value;
                    //     decimal item3 = numericUpDown5.Value;
                    // 
                    //     numericUpDown4.Value = item3;
                    //     numericUpDown5.Value = item2;
                    // }

                    // Combine strings into int. // Fix v1.3.5.6. // Fix v1.3.5.7 - Added the math to this overlooked function.
                    selectedItemVariation = VariationHelper.GetFoodVariation((int)Variation1_NumericUpDown.Value, (int)Variation2_NumericUpDown.Value);

                    // Legacy format.
                    // selectedItemVariation = int.Parse(numericUpDown4.Value.ToString() + numericUpDown5.Value.ToString());
                }
                else
                {
                    // Only single value exists, treat as a unique variant value.
                    selectedItemVariation = (int)Variation1_NumericUpDown.Value;
                }
            }
            else
            {
                // Normal item variant.
                selectedItemVariation = (int)VariationNumerical_NumericUpDown.Value;
            }
            this.Close();
        }
        #endregion // End quick quantity selector.

        #region Set Custom Quick Quantity Selector Values

        // QQS Button 1.
        private void CustomQuantity1_Button_MouseDown(object sender, MouseEventArgs e)
        {
            // Detect if the right mouse was pressed.
            if (e.Button == MouseButtons.Right)
            {
                // Unhide the numericupdown and hide button.
                CustomQuantity1_Button.Visible = false;
                CustomQuantity1_NumericUpDown.Visible = true;
            }
        }
        private void CustomQuantity1_NumericUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            // Detect if enter was pressed.
            if (e.KeyCode == Keys.Enter)
            {
                // Save settings.
                Settings.Default.QuantitySelectGetInventoryAddresses_Button = (int)CustomQuantity1_NumericUpDown.Value;

                // Change button text.
                CustomQuantity1_Button.Text = ((int)CustomQuantity1_NumericUpDown.Value).ToString();

                // Hide the numericupdown and unhide button.
                CustomQuantity1_Button.Visible = true;
                CustomQuantity1_NumericUpDown.Visible = false;
            }
        }

        // QQS Button 2.
        private void CustomQuantity2_Button_MouseDown(object sender, MouseEventArgs e)
        {
            // Detect if the right mouse was pressed.
            if (e.Button == MouseButtons.Right)
            {
                // Unhide the numericupdown and hide button.
                CustomQuantity2_Button.Visible = false;
                CustomQuantity2_NumericUpDown.Visible = true;
            }
        }
        private void CustomQuantity2_NumericUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            // Detect if enter was pressed.
            if (e.KeyCode == Keys.Enter)
            {
                // Save settings.
                Settings.Default.QuantitySelectButton2 = (int)CustomQuantity2_NumericUpDown.Value;

                // Change button text.
                CustomQuantity2_Button.Text = ((int)CustomQuantity2_NumericUpDown.Value).ToString();

                // Hide the numericupdown and unhide button.
                CustomQuantity2_Button.Visible = true;
                CustomQuantity2_NumericUpDown.Visible = false;
            }
        }

        // QQS Button 3.
        private void CustomQuantity3_Button_MouseDown(object sender, MouseEventArgs e)
        {
            // Detect if the right mouse was pressed.
            if (e.Button == MouseButtons.Right)
            {
                // Unhide the numericupdown and hide button.
                CustomQuantity3_Button.Visible = false;
                CustomQuantity3_NumericUpDown.Visible = true;
            }
        }
        private void CustomQuantity3_NumericUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            // Detect if enter was pressed.
            if (e.KeyCode == Keys.Enter)
            {
                // Save settings.
                Settings.Default.QuantitySelectButton3 = (int)CustomQuantity3_NumericUpDown.Value;

                // Change button text.
                CustomQuantity3_Button.Text = ((int)CustomQuantity3_NumericUpDown.Value).ToString();

                // Hide the numericupdown and unhide button.
                CustomQuantity3_Button.Visible = true;
                CustomQuantity3_NumericUpDown.Visible = false;
            }
        }

        // QQS Button 4.
        private void CustomQuantity4_Button_MouseDown(object sender, MouseEventArgs e)
        {
            // Detect if the right mouse was pressed.
            if (e.Button == MouseButtons.Right)
            {
                // Unhide the numericupdown and hide button.
                CustomQuantity4_Button.Visible = false;
                CustomQuantity4_NumericUpDown.Visible = true;
            }
        }
        private void CustomQuantity4_NumericUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            // Detect if enter was pressed.
            if (e.KeyCode == Keys.Enter)
            {
                // Save settings.
                Settings.Default.QuantitySelectButton4 = (int)CustomQuantity4_NumericUpDown.Value;

                // Change button text.
                CustomQuantity4_Button.Text = ((int)CustomQuantity4_NumericUpDown.Value).ToString();

                // Hide the numericupdown and unhide button.
                CustomQuantity4_Button.Visible = true;
                CustomQuantity4_NumericUpDown.Visible = false;
            }
        }

        // QQS Button 5.
        private void CustomQuantity5_Button_MouseDown(object sender, MouseEventArgs e)
        {
            // Detect if the right mouse was pressed.
            if (e.Button == MouseButtons.Right)
            {
                // Unhide the numericupdown and hide button.
                CustomQuantity5_Button.Visible = false;
                CustomQuantity5_NumericUpDown.Visible = true;
            }
        }
        private void CustomQuantity5_NumericUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            // Detect if enter was pressed.
            if (e.KeyCode == Keys.Enter)
            {
                // Save settings.
                Settings.Default.QuantitySelectButton5 = (int)CustomQuantity5_NumericUpDown.Value;

                // Change button text.
                CustomQuantity5_Button.Text = ((int)CustomQuantity5_NumericUpDown.Value).ToString();

                // Hide the numericupdown and unhide button.
                CustomQuantity5_Button.Visible = true;
                CustomQuantity5_NumericUpDown.Visible = false;
            }
        }
        #endregion // End set quick quantity selector values.

        // Launch item explorer.
        private void Slot1_PictureBox_Click(object sender, EventArgs e)
        {
            // Before showing the InventoryEditor popup form, check if the images are already loaded in memory cache.
            // If not, load them now (this only happens once).
            if (!InventoryImageCache.IsLoaded)
            {
                // Loads all images from disk into memory (static cache).
                // Pass the FolderNames to tell it what folders to load.
                InventoryImageCache.LoadAllImages(ItemSelectionMenu.FolderNames);
            }

            // Spawn item selection menu window.
            ItemSelectionMenu itemSelectionMenu1 = new ItemSelectionMenu();
            DialogResult dr = itemSelectionMenu1.ShowDialog(this);

            // Get returned item from picker.
            int itemType    = itemSelectionMenu1.GetItemTypeFromList();
            bool wasAborted = itemSelectionMenu1.GetUserCanceldTask();
            // bool itemOverwrite = itemSelectionMenu1.GetSelectedOverwriteTask();
            itemSelectionMenu1.Close();

            // Check if user closed the form
            if (wasAborted) { return; };

            // Set the values from returning form.
            ItemID_NumericUpDown.Value = itemType;

            // Reload pictureboxes and labels.
            ReloadPictureBoxes(useTextboxData: true);
        }

        // Launch item explorer.
        private void Slot2_PictureBox_Click(object sender, EventArgs e)
        {
            // Before showing the InventoryEditor popup form, check if the images are already loaded in memory cache.
            // If not, load them now (this only happens once).
            if (!InventoryImageCache.IsLoaded)
            {
                // Loads all images from disk into memory (static cache).
                // Pass the FolderNames to tell it what folders to load.
                InventoryImageCache.LoadAllImages(ItemSelectionMenu.FolderNames);
            }

            // Spawn item selection menu window.
            ItemSelectionMenu itemSelectionMenu2 = new ItemSelectionMenu();
            DialogResult dr = itemSelectionMenu2.ShowDialog(this);

            // Get returned item from picker.
            int itemType    = itemSelectionMenu2.GetItemTypeFromList();
            bool wasAborted = itemSelectionMenu2.GetUserCanceldTask();
            // bool itemOverwrite = itemSelectionMenu2.GetSelectedOverwriteTask();
            itemSelectionMenu2.Close();

            // Check if user closed the form
            if (wasAborted) { return; };

            // Set the values from returning form.
            if (VariationNumerical_NumericUpDown.Visible)
                VariationNumerical_NumericUpDown.Value = itemType;
            else
                Variation1_NumericUpDown.Value         = itemType;

            // Reload pictureboxes and labels.
            ReloadPictureBoxes(useTextboxData: true);
        }

        // Launch item explorer.
        private void Slot3_PictureBox_Click(object sender, EventArgs e)
        {
            // Before showing the InventoryEditor popup form, check if the images are already loaded in memory cache.
            // If not, load them now (this only happens once).
            if (!InventoryImageCache.IsLoaded)
            {
                // Loads all images from disk into memory (static cache).
                // Pass the FolderNames to tell it what folders to load.
                InventoryImageCache.LoadAllImages(ItemSelectionMenu.FolderNames);
            }

            // Do not launch the second variation item selection menu if item mode is enabled.
            if (VariationNumerical_NumericUpDown.Visible)
                return;

            // Spawn item selection menu window.
            ItemSelectionMenu itemSelectionMenu3 = new ItemSelectionMenu();
            DialogResult dr = itemSelectionMenu3.ShowDialog(this);

            // Get returned item from picker.
            int itemType    = itemSelectionMenu3.GetItemTypeFromList();
            bool wasAborted = itemSelectionMenu3.GetUserCanceldTask();
            // bool itemOverwrite = itemSelectionMenu3.GetSelectedOverwriteTask();
            itemSelectionMenu3.Close();

            // Check if user closed the form
            if (wasAborted) { return; };

            // Set the values from returning form.
            Variation2_NumericUpDown.Value = itemType;

            // Reload pictureboxes and labels.
            ReloadPictureBoxes(useTextboxData: true);
        }

        // User clicked done, save and close form.
        private void Done_Button_Click(object sender, EventArgs e)
        {
            selectedItemType     = (int)ItemID_NumericUpDown.Value;
            selectedItemAmount   = (int)Quantity_NumericUpDown.Value;
            selectedItemSkillset = (int)Skillset_NumericUpDown.Value;
            if (!VariationNumerical_NumericUpDown.Visible) // Check if item is a food variant.
            {
                // Check if both entrees are populated.
                if (Variation2_NumericUpDown.Value != 0)
                {
                    // Do some checks and corrections for values over 8. // Fix v1.3.5.1.
                    // if (numericUpDown4.Value > numericUpDown5.Value)
                    // {
                    //     // Flip values.
                    //     decimal item2 = numericUpDown4.Value;
                    //     decimal item3 = numericUpDown5.Value;
                    // 
                    //     numericUpDown4.Value = item3;
                    //     numericUpDown5.Value = item2;
                    // }

                    // Combine strings into int. // Fix v1.3.5.6.
                    selectedItemVariation = VariationHelper.GetFoodVariation((int)Variation1_NumericUpDown.Value, (int)Variation2_NumericUpDown.Value);

                    // Legacy format.
                    // selectedItemVariation = int.Parse(numericUpDown4.Value.ToString() + numericUpDown5.Value.ToString());
                }
                else
                {
                    // Only single value exists, treat as a unique variant value.
                    selectedItemVariation = (int)Variation1_NumericUpDown.Value;
                }
            }
            else
            {
                // Normal item variant.
                selectedItemVariation = (int)VariationNumerical_NumericUpDown.Value;
            }
            this.Close();
        }
        #endregion
    }
}