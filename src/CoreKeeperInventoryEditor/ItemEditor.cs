using System.Text.RegularExpressions;
using System.Collections.Generic;
using CoreKeeperInventoryEditor;
using System.Windows.Forms;
using System.Diagnostics;
using System.Reflection;
using System.Drawing;
using System.Linq;
using System.IO;
using System;

namespace CoreKeepersWorkshop
{
    public partial class ItemEditor : Form
    {
        public ItemEditor()
        {
            InitializeComponent();
        }

        // Define texture data.
        public IEnumerable<string> ImageFiles1 = Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"assets\Inventory\") && Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"assets\Inventory\", "*.png", SearchOption.AllDirectories) != null ? Directory.GetFileSystemEntries(AppDomain.CurrentDomain.BaseDirectory + @"assets\Inventory\", "*.png", SearchOption.AllDirectories) : new String[] { "" }; // Ensure directory exists and images exist. Fix: v1.2.9.

        // Form closing saving.
        int selectedItemType = 0;
        int selectedItemAmount = 0;
        int selectedItemVariation = 0;
        int selectedItemSkillset = 0;
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
        public int GetItemSkillsetFromList()
        {
            return selectedItemSkillset;
        }
        public bool GetUserCancledTask()
        {
            return userCancledTask;
        }

        #region Reload Pictureboxes & Labels

        // Reload pictureboxes and labels.
        public void ReloadPictureBoxes(bool useTextboxeData = false)
        {
            // Define base settings.
            int baseItemID = CoreKeepersWorkshop.Properties.Settings.Default.InfoID;
            int baseItemAmount = CoreKeepersWorkshop.Properties.Settings.Default.InfoAmount;
            int baseItemVariation = CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation;

            // Get the ingredient ids of the item.
            string baseIngredient1ID = "0";
            string baseIngredient2ID = "0";

            // Use defined form data.
            if (useTextboxeData)
            {
                baseItemID = (int)ItemID_NumericUpDown.Value;
                baseItemAmount = (int)Quantity_NumericUpDown.Value;
                if (!VariationNumerical_NumericUpDown.Visible) // Check if item is a food variant.
                {
                    baseIngredient1ID = Variation1_NumericUpDown.Value.ToString();
                    baseIngredient2ID = Variation2_NumericUpDown.Value.ToString();
                }
                else
                {
                    // Normal item variant.
                    baseIngredient1ID = VariationNumerical_NumericUpDown.Value.ToString();
                }
            }

            // Get base item name.
            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseItemID.ToString()) != null)
            {
                Item1_Label.Text = Path.GetFileName(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseItemID.ToString())).Split(',')[0];

                // Load image.
                Slot1_PictureBox.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseItemID.ToString())));
                Slot1_PictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                // Check if the first item is not empty.
                if (ItemID_NumericUpDown.Value != 0)
                {
                    Item1_Label.Text = "UnkownItem";

                    // Load image.
                    Slot1_PictureBox.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                    Slot1_PictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    Slot1_PictureBox.Image = null;
                    Item1_Label.Text = "";
                }
            }

            // Check if usetextbox mode is enabled.
            if (!useTextboxeData)
            {
                // Check if the items variant is populated.
                if (baseItemVariation >= 1)
                {
                    // Get base item ingrdient 1 name.
                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == VariationHelper.GetIngredient1FromFoodVariation(baseItemVariation).ToString()) != null)
                    {
                        Item2_Label.Text = Path.GetFileName(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == VariationHelper.GetIngredient1FromFoodVariation(baseItemVariation).ToString())).Split(',')[0];

                        // Load image.
                        Slot2_PictureBox.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == VariationHelper.GetIngredient1FromFoodVariation(baseItemVariation).ToString())));
                        Slot2_PictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    else
                    {
                        // Check if the variant item two is not empty.
                        if (VariationNumerical_NumericUpDown.Value != 0 || Variation1_NumericUpDown.Value != 0)
                        {
                            Item2_Label.Text = "UnkownItem";

                            // Load image.
                            Slot2_PictureBox.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            Slot2_PictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                        }
                        else
                        {
                            Slot2_PictureBox.Image = null;
                            Item2_Label.Text = "";
                        }
                    }
                }
                else
                {
                    Slot2_PictureBox.Image = null;
                    Item2_Label.Text = "";
                }
                // Check if the items variant is populated.
                if (baseItemVariation >= 1 && !VariationNumerical_NumericUpDown.Visible)
                {
                    // Get base item ingrdient 2 name.
                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == VariationHelper.GetIngredient2FromFoodVariation(baseItemVariation).ToString()) != null)
                    {
                        Item3_Label.Text = Path.GetFileName(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == VariationHelper.GetIngredient2FromFoodVariation(baseItemVariation).ToString())).Split(',')[0];

                        // Load image.
                        Slot3_PictureBox.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == VariationHelper.GetIngredient2FromFoodVariation(baseItemVariation).ToString())));
                        Slot3_PictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    else
                    {
                        // Check if the third item is not empty.
                        if (Variation2_NumericUpDown.Value != 0)
                        {
                            Item3_Label.Text = "UnkownItem";

                            // Load image.
                            Slot3_PictureBox.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            Slot3_PictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                        }
                        else
                        {
                            Slot3_PictureBox.Image = null;
                            Item3_Label.Text = "";
                        }
                    }
                }
                else
                {
                    
                    Slot3_PictureBox.Image = null;
                    Item3_Label.Text = "";
                }
            }
            else
            {
                // Use texbox data.
                // Check if the items variant is populated.
                if (baseIngredient1ID.ToString().Length > 0 && int.Parse(baseIngredient1ID.ToString()) > 0)
                {
                    // Check if target is item mode or not. // Fix v1.3.5.6.
                    if (!VariationNumerical_NumericUpDown.Visible)
                    {
                        // Get base item ingrdient 1 name.
                        if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseIngredient1ID) != null)
                        {
                            Item2_Label.Text = Path.GetFileName(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseIngredient1ID)).Split(',')[0];

                            // Load image.
                            Slot2_PictureBox.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseIngredient1ID)));
                            Slot2_PictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                        }
                        else
                        {
                            // Check if the variant item two is not empty.
                            if (VariationNumerical_NumericUpDown.Value != 0 || Variation1_NumericUpDown.Value != 0)
                            {
                                Item2_Label.Text = "UnkownItem";

                                // Load image.
                                Slot2_PictureBox.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                Slot2_PictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                Slot2_PictureBox.Image = null;
                                Item2_Label.Text = "";
                            }
                        }
                    }
                    else
                    {
                        Slot2_PictureBox.Image = null;
                        Item2_Label.Text = "";
                    }
                }
                else
                {
                    Slot2_PictureBox.Image = null;
                    Item2_Label.Text = "";
                }
                // Check if the items variant is populated.
                if (baseIngredient2ID.ToString().Length > 0 && !VariationNumerical_NumericUpDown.Visible) // Make sure duel texbox mode is enabled.
                {
                    // Get base item ingrdient 2 name.
                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseIngredient2ID) != null)
                    {
                        Item3_Label.Text = Path.GetFileName(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseIngredient2ID)).Split(',')[0];

                        // Load image.
                        Slot3_PictureBox.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseIngredient2ID)));
                        Slot3_PictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    else
                    {
                        // Check if the third item is not empty.
                        if (Variation2_NumericUpDown.Value != 0)
                        {
                            Item3_Label.Text = "UnkownItem";

                            // Load image.
                            Slot3_PictureBox.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            Slot3_PictureBox.SizeMode = PictureBoxSizeMode.Zoom;
                        }
                        else
                        {
                            Slot3_PictureBox.Image = null;
                            Item3_Label.Text = "";
                        }
                    }
                }
                else
                {
                    Slot3_PictureBox.Image = null;
                    Item3_Label.Text = "";
                }
            }

            // Reload pictureboxs.
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

            #region Set Form Locations

            // Set the forms active location based on previous save.
            this.Location = CoreKeepersWorkshop.Properties.Settings.Default.ItemEditorLocation;
            #endregion

            #region Tooltips

            // Create a new tooltip.
            ToolTip toolTip = new ToolTip()
            {
                AutoPopDelay = 5000,
                InitialDelay = 750
            };

            // Set tool texts.
            toolTip.SetToolTip(ItemID_NumericUpDown, "Enter the ID you wish to add. Press enter when done.");
            toolTip.SetToolTip(Quantity_NumericUpDown, "Enter a custom item quantity. Press enter when done.");
            toolTip.SetToolTip(VariationNumerical_NumericUpDown, "Enter a custom variant ID. Press enter when done.");
            toolTip.SetToolTip(Variation1_NumericUpDown, "Enter an ingredient one ID. Press enter when done.");
            toolTip.SetToolTip(Variation2_NumericUpDown, "Enter an ingredient two ID. Press enter when done.");
            toolTip.SetToolTip(CustomQuantity1_NumericUpDown, "Press the enter key when finished.");
            toolTip.SetToolTip(CustomQuantity2_NumericUpDown, "Press the enter key when finished.");
            toolTip.SetToolTip(CustomQuantity3_NumericUpDown, "Press the enter key when finished.");
            toolTip.SetToolTip(CustomQuantity4_NumericUpDown, "Press the enter key when finished.");
            toolTip.SetToolTip(CustomQuantity5_NumericUpDown, "Press the enter key when finished.");
            toolTip.SetToolTip(Skillset_NumericUpDown, "Enter a custom skillset ID. Either press enter when done or use the botton.");

            toolTip.SetToolTip(Variation_Label, "Toggle the GUI between food / item variaty.");

            toolTip.SetToolTip(ChangeRarity_Button, "Change your food rarity. Press enter when done.");
            toolTip.SetToolTip(Done_Button, "Finish editing the item.");
            toolTip.SetToolTip(RemoveItem_Button, "Remove the item from this inventory slot.");
            toolTip.SetToolTip(CookedFoodList_Button, "Open the food cookbook to easily search for food items.");
            toolTip.SetToolTip(CustomQuantity1_Button, "Quick change the items quanitity. Right click to edit.");
            toolTip.SetToolTip(CustomQuantity2_Button, "Quick change the items quanitity. Right click to edit.");
            toolTip.SetToolTip(CustomQuantity3_Button, "Quick change the items quanitity. Right click to edit.");
            toolTip.SetToolTip(CustomQuantity4_Button, "Quick change the items quanitity. Right click to edit.");
            toolTip.SetToolTip(CustomQuantity5_Button, "Quick change the items quanitity. Right click to edit.");
            toolTip.SetToolTip(SkillsetHelp_Button, "Launch a guide on how to find skillset IDs.");

            toolTip.SetToolTip(Slot1_PictureBox, "Click to open the item explorer.");
            toolTip.SetToolTip(Slot2_PictureBox, "Click to open the item explorer.");
            toolTip.SetToolTip(Slot3_PictureBox, "Click to open the item explorer.");
            #endregion

            #region Load Form Settings

            // Load quantity select numerics and buttons.
            CustomQuantity1_Button.Text = CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton1.ToString();
            CustomQuantity1_NumericUpDown.Value = CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton1;
            CustomQuantity2_Button.Text = CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton2.ToString();
            CustomQuantity2_NumericUpDown.Value = CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton2;
            CustomQuantity3_Button.Text = CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton3.ToString();
            CustomQuantity3_NumericUpDown.Value = CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton3;
            CustomQuantity4_Button.Text = CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton4.ToString();
            CustomQuantity4_NumericUpDown.Value = CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton4;
            CustomQuantity5_Button.Text = CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton5.ToString();
            CustomQuantity5_NumericUpDown.Value = CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton5;

            // Ensure the quantity is more then zero. // Fix v1.3.5.4.
            if (CoreKeepersWorkshop.Properties.Settings.Default.InfoAmount < 1)
            {
                CoreKeepersWorkshop.Properties.Settings.Default.InfoAmount = 1;
            }
            // Ensure the skillset is more then -1.
            if (CoreKeepersWorkshop.Properties.Settings.Default.InfoSkillset < 0)
            {
                MessageBox.Show("The skillset was lower then 0! -> Current value: " + CoreKeepersWorkshop.Properties.Settings.Default.InfoSkillset + "\n\nValue will be set to 0.", "WARNING", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CoreKeepersWorkshop.Properties.Settings.Default.InfoSkillset = 0;
            }

            // Load some settings.
            if (CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation >= 1) // Check if item is a food variant.
            {
                // Change some form items.
                VariationNumerical_NumericUpDown.Visible = false;
                Variation1_NumericUpDown.Visible = true;
                Variation2_NumericUpDown.Visible = true;
                VariationAnd_Button.Visible = true;

                // Update settings.
                ItemID_NumericUpDown.Value = CoreKeepersWorkshop.Properties.Settings.Default.InfoID;
                Quantity_NumericUpDown.Value = CoreKeepersWorkshop.Properties.Settings.Default.InfoAmount;

                // New format. // Fix v1.3.5.6.
                Variation1_NumericUpDown.Value = VariationHelper.GetIngredient1FromFoodVariation(CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation);
                Variation2_NumericUpDown.Value = VariationHelper.GetIngredient2FromFoodVariation(CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation);

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
                // numericUpDown4.Value = decimal.Parse(CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation.ToString().Substring(0, CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation.ToString().Length / 2));
                // numericUpDown5.Value = decimal.Parse(CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation.ToString().Substring(CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation.ToString().Length / 2));

                Skillset_NumericUpDown.Value = CoreKeepersWorkshop.Properties.Settings.Default.InfoSkillset;

                // Rename button label.
                Variation_Label.Text = "Variation [Food Ingredients]";
            }
            else
            {
                // None food variant, keep normal settings.
                ItemID_NumericUpDown.Value = CoreKeepersWorkshop.Properties.Settings.Default.InfoID;
                Quantity_NumericUpDown.Value = CoreKeepersWorkshop.Properties.Settings.Default.InfoAmount;
                VariationNumerical_NumericUpDown.Value = CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation;
                Skillset_NumericUpDown.Value = CoreKeepersWorkshop.Properties.Settings.Default.InfoSkillset;
            }
            #endregion

            #region Load Pictures & Names

            // Reload all pictureboxes and labels from the defualt load data.
            ReloadPictureBoxes(useTextboxeData: true);
            #endregion
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
                        CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation = VariationHelper.GetFoodVariation((int)Variation1_NumericUpDown.Value, (int)Variation2_NumericUpDown.Value);

                        // Legacy format.
                        // CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation = int.Parse(numericUpDown4.Value.ToString() + numericUpDown5.Value.ToString());
                    }
                    else
                    {
                        // Only single value exists, treat as a unique variant value.
                        CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation = (int)Variation1_NumericUpDown.Value;
                    }
                }
                else
                {
                    // Normal item variant.
                    CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation = (int)VariationNumerical_NumericUpDown.Value;
                }
                CoreKeepersWorkshop.Properties.Settings.Default.InfoID = (int)ItemID_NumericUpDown.Value;
                CoreKeepersWorkshop.Properties.Settings.Default.InfoAmount = (int)Quantity_NumericUpDown.Value;
                CoreKeepersWorkshop.Properties.Settings.Default.InfoSkillset = (int)Skillset_NumericUpDown.Value;
                CoreKeepersWorkshop.Properties.Settings.Default.ItemEditorLocation = this.Location;
            }
            catch (Exception)
            { } // Do nothing.
        }
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
                ReloadPictureBoxes(useTextboxeData: true);
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
                ReloadPictureBoxes(useTextboxeData: true);
            }
        }

        // Open food cookbook.
        private void CookedFoodList_Button_Click(object sender, EventArgs e)
        {
            // Spawn food cookbook window.
            FoodCookbook frm4 = new FoodCookbook();
            DialogResult dr = frm4.ShowDialog(this);

            // Get returned item from picker.
            int itemType = frm4.GetItemTypeFromList();
            int itemAmount = frm4.GetItemAmountFromList();
            int itemVariation = frm4.GetItemVeriationFromList() == 0 ? 0 : (frm4.GetItemVeriationFromList()); // If variation is not zero, add offset.
            bool wasAborted = frm4.GetUserCancledTask();
            // bool itemOverwrite = frm3.GetSelectedOverwriteTask();
            frm4.Close();

            // Check if user closed the form
            if (wasAborted) { return; };

            // Set the values from returning form.
            selectedItemType = itemType;
            selectedItemAmount = itemAmount;
            selectedItemVariation = itemVariation;
            this.Close();
        }

        // Value had changed, reload images and labels.
        private void VariationNumerical_NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            // Reload all pictureboxes and labels from the textboxe data.
            ReloadPictureBoxes(useTextboxeData: true);
        }

        // Value had changed, reload images and labels.
        private void Variation1_NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            // Reload all pictureboxes and labels from the textboxe data.
            ReloadPictureBoxes(useTextboxeData: true);
        }

        // Value had changed, reload images and labels.
        private void Variation2_NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            // Reload all pictureboxes and labels from the textboxe data.
            ReloadPictureBoxes(useTextboxeData: true);
        }

        // Value had changed, reload images and labels.
        private void ItemID_NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            // Reload all pictureboxes and labels from the textboxe data.
            ReloadPictureBoxes(useTextboxeData: true);
        }

        // Change item rarity.
        private void ChangeRarity_Button_Click(object sender, EventArgs e)
        {
            string originalName = "Unkown";
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

            // Ensure original variation is 8 in lengh.
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

                // Determin the next prefix we need to match.
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
                                ReloadPictureBoxes(useTextboxeData: true);

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

        // Lanch about skillset message.
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
            // Change the existing items quanitity. 
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
        private void Button8_Click(object sender, EventArgs e)
        {
            // Change the existing items quanitity. 
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
            // Change the existing items quanitity. 
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
            // Change the existing items quanitity. 
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
            // Change the existing items quanitity. 
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
            // Detect if enter was pressesed.
            if (e.KeyCode == Keys.Enter)
            {
                // Save settings.
                CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton1 = (int)CustomQuantity1_NumericUpDown.Value;

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
            // Detect if enter was pressesed.
            if (e.KeyCode == Keys.Enter)
            {
                // Save settings.
                CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton2 = (int)CustomQuantity2_NumericUpDown.Value;

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
            // Detect if enter was pressesed.
            if (e.KeyCode == Keys.Enter)
            {
                // Save settings.
                CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton3 = (int)CustomQuantity3_NumericUpDown.Value;

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
            // Detect if enter was pressesed.
            if (e.KeyCode == Keys.Enter)
            {
                // Save settings.
                CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton4 = (int)CustomQuantity4_NumericUpDown.Value;

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
            // Detect if enter was pressesed.
            if (e.KeyCode == Keys.Enter)
            {
                // Save settings.
                CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton5 = (int)CustomQuantity5_NumericUpDown.Value;

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
            // Spawn food cookbook window.
            InventoryEditor frm3 = new InventoryEditor();
            DialogResult dr = frm3.ShowDialog(this);

            // Get returned item from picker.
            int itemType = frm3.GetItemTypeFromList();
            bool wasAborted = frm3.GetUserCancledTask();
            // bool itemOverwrite = frm3.GetSelectedOverwriteTask();
            frm3.Close();

            // Check if user closed the form
            if (wasAborted) { return; };

            // Set the values from returning form.
            ItemID_NumericUpDown.Value = itemType;

            // Reload pictureboxes and labels.
            ReloadPictureBoxes(useTextboxeData: true);
        }

        // Launch item explorer.
        private void Slot2_PictureBox_Click(object sender, EventArgs e)
        {
            // Spawn food cookbook window.
            InventoryEditor frm3 = new InventoryEditor();
            DialogResult dr = frm3.ShowDialog(this);

            // Get returned item from picker.
            int itemType = frm3.GetItemTypeFromList();
            bool wasAborted = frm3.GetUserCancledTask();
            // bool itemOverwrite = frm3.GetSelectedOverwriteTask();
            frm3.Close();

            // Check if user closed the form
            if (wasAborted) { return; };

            // Set the values from returning form.
            // Check if food edit mode is enabled or not.
            // if (numericUpDown3.Visible)
            // {
            //     numericUpDown3.Value = itemType;
            // }
            // else
            // {
            //     numericUpDown4.Value = itemType;
            // }

            // Reload pictureboxes and labels.
            ReloadPictureBoxes(useTextboxeData: true);
        }

        // Launch item explorer.
        private void Slot3_PictureBox_Click(object sender, EventArgs e)
        {
            // Spawn food cookbook window.
            InventoryEditor frm3 = new InventoryEditor();
            DialogResult dr = frm3.ShowDialog(this);

            // Get returned item from picker.
            int itemType = frm3.GetItemTypeFromList();
            bool wasAborted = frm3.GetUserCancledTask();
            // bool itemOverwrite = frm3.GetSelectedOverwriteTask();
            frm3.Close();

            // Check if user closed the form
            if (wasAborted) { return; };

            // Set the values from returning form.
            Variation2_NumericUpDown.Value = itemType;

            // Reload pictureboxes and labels.
            ReloadPictureBoxes(useTextboxeData: true);
        }

        // User clicked done, save and close form.
        private void Done_Button_Click(object sender, EventArgs e)
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