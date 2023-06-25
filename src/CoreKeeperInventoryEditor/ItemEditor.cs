using CoreKeeperInventoryEditor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

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
                baseItemID = (int)numericUpDown1.Value;
                baseItemAmount = (int)numericUpDown2.Value;
                if (!numericUpDown3.Visible) // Check if item is a food variant.
                {
                    baseIngredient1ID = numericUpDown4.Value.ToString();
                    baseIngredient2ID = numericUpDown5.Value.ToString();
                }
                else
                {
                    // Normal item variant.
                    baseIngredient1ID = numericUpDown3.Value.ToString();
                }
            }

            // Get base item name.
            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseItemID.ToString()) != null)
            {
                label3.Text = Path.GetFileName(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseItemID.ToString())).Split(',')[0];

                // Load image.
                pictureBox1.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseItemID.ToString())));
                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            }
            else
            {
                // Check if the first item is not empty.
                if (numericUpDown1.Value != 0)
                {
                    label3.Text = "UnkownItem";

                    // Load image.
                    pictureBox1.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                    pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    pictureBox1.Image = null;
                    label3.Text = "";
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
                        label4.Text = Path.GetFileName(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == VariationHelper.GetIngredient1FromFoodVariation(baseItemVariation).ToString())).Split(',')[0];

                        // Load image.
                        pictureBox2.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == VariationHelper.GetIngredient1FromFoodVariation(baseItemVariation).ToString())));
                        pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    else
                    {
                        // Check if the variant item two is not empty.
                        if (numericUpDown3.Value != 0 || numericUpDown4.Value != 0)
                        {
                            label4.Text = "UnkownItem";

                            // Load image.
                            pictureBox2.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                        }
                        else
                        {
                            pictureBox2.Image = null;
                            label4.Text = "";
                        }
                    }
                }
                else
                {
                    pictureBox2.Image = null;
                    label4.Text = "";
                }
                // Check if the items variant is populated.
                if (baseItemVariation >= 1 && !numericUpDown3.Visible)
                {
                    // Get base item ingrdient 2 name.
                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == VariationHelper.GetIngredient2FromFoodVariation(baseItemVariation).ToString()) != null)
                    {
                        label5.Text = Path.GetFileName(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == VariationHelper.GetIngredient2FromFoodVariation(baseItemVariation).ToString())).Split(',')[0];

                        // Load image.
                        pictureBox3.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == VariationHelper.GetIngredient2FromFoodVariation(baseItemVariation).ToString())));
                        pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    else
                    {
                        // Check if the third item is not empty.
                        if (numericUpDown5.Value != 0)
                        {
                            label5.Text = "UnkownItem";

                            // Load image.
                            pictureBox3.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
                        }
                        else
                        {
                            pictureBox3.Image = null;
                            label5.Text = "";
                        }
                    }
                }
                else
                {
                    
                    pictureBox3.Image = null;
                    label5.Text = "";
                }
            }
            else
            {
                // Use texbox data.
                // Check if the items variant is populated.
                if (baseIngredient1ID.ToString().Length > 0 && int.Parse(baseIngredient1ID.ToString()) > 0)
                {
                    // Check if target is item mode or not. // Fix v1.3.5.6.
                    if (!numericUpDown3.Visible)
                    {
                        // Get base item ingrdient 1 name.
                        if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseIngredient1ID) != null)
                        {
                            label4.Text = Path.GetFileName(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseIngredient1ID)).Split(',')[0];

                            // Load image.
                            pictureBox2.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseIngredient1ID)));
                            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                        }
                        else
                        {
                            // Check if the variant item two is not empty.
                            if (numericUpDown3.Value != 0 || numericUpDown4.Value != 0)
                            {
                                label4.Text = "UnkownItem";

                                // Load image.
                                pictureBox2.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                pictureBox2.Image = null;
                                label4.Text = "";
                            }
                        }
                    }
                    else
                    {
                        pictureBox2.Image = null;
                        label4.Text = "";
                    }
                }
                else
                {
                    pictureBox2.Image = null;
                    label4.Text = "";
                }
                // Check if the items variant is populated.
                if (baseIngredient2ID.ToString().Length > 0 && !numericUpDown3.Visible) // Make sure duel texbox mode is enabled.
                {
                    // Get base item ingrdient 2 name.
                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseIngredient2ID) != null)
                    {
                        label5.Text = Path.GetFileName(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseIngredient2ID)).Split(',')[0];

                        // Load image.
                        pictureBox3.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseIngredient2ID)));
                        pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
                    }
                    else
                    {
                        // Check if the third item is not empty.
                        if (numericUpDown5.Value != 0)
                        {
                            label5.Text = "UnkownItem";

                            // Load image.
                            pictureBox3.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
                        }
                        else
                        {
                            pictureBox3.Image = null;
                            label5.Text = "";
                        }
                    }
                }
                else
                {
                    pictureBox3.Image = null;
                    label5.Text = "";
                }
            }

            // Reload pictureboxs.
            pictureBox1.Invalidate();
            pictureBox2.Invalidate();
            pictureBox3.Invalidate();
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
            toolTip.SetToolTip(numericUpDown1, "Enter the ID you wish to add. Press enter when done.");
            toolTip.SetToolTip(numericUpDown2, "Enter a custom item quantity. Press enter when done.");
            toolTip.SetToolTip(numericUpDown3, "Enter a custom variant ID. Press enter when done.");
            toolTip.SetToolTip(numericUpDown4, "Enter an ingredient one ID. Press enter when done.");
            toolTip.SetToolTip(numericUpDown5, "Enter an ingredient two ID. Press enter when done.");
            toolTip.SetToolTip(numericUpDown6, "Press the enter key when finished.");
            toolTip.SetToolTip(numericUpDown7, "Press the enter key when finished.");
            toolTip.SetToolTip(numericUpDown8, "Press the enter key when finished.");
            toolTip.SetToolTip(numericUpDown9, "Press the enter key when finished.");
            toolTip.SetToolTip(numericUpDown10, "Press the enter key when finished.");
            toolTip.SetToolTip(numericUpDown11, "Enter a custom skillset ID. Either press enter when done or use the botton.");

            toolTip.SetToolTip(label2, "Toggle the GUI between food / item variaty.");

            toolTip.SetToolTip(button1, "Change your food rarity. Press enter when done.");
            toolTip.SetToolTip(button3, "Finish editing the item.");
            toolTip.SetToolTip(button5, "Remove the item from this inventory slot.");
            toolTip.SetToolTip(button6, "Open the food cookbook to easily search for food items.");
            toolTip.SetToolTip(button7, "Quick change the items quanitity. Right click to edit.");
            toolTip.SetToolTip(button8, "Quick change the items quanitity. Right click to edit.");
            toolTip.SetToolTip(button9, "Quick change the items quanitity. Right click to edit.");
            toolTip.SetToolTip(button10, "Quick change the items quanitity. Right click to edit.");
            toolTip.SetToolTip(button11, "Quick change the items quanitity. Right click to edit.");
            toolTip.SetToolTip(button13, "Launch a guide on how to find skillset IDs.");

            toolTip.SetToolTip(pictureBox1, "Click to open the item explorer.");
            toolTip.SetToolTip(pictureBox2, "Click to open the item explorer.");
            toolTip.SetToolTip(pictureBox3, "Click to open the item explorer.");
            #endregion

            #region Load Form Settings

            // Load quantity select numerics and buttons.
            button7.Text = CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton1.ToString();
            numericUpDown6.Value = CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton1;
            button8.Text = CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton2.ToString();
            numericUpDown7.Value = CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton2;
            button9.Text = CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton3.ToString();
            numericUpDown8.Value = CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton3;
            button10.Text = CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton4.ToString();
            numericUpDown9.Value = CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton4;
            button11.Text = CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton5.ToString();
            numericUpDown10.Value = CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton5;

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
                numericUpDown3.Visible = false;
                numericUpDown4.Visible = true;
                numericUpDown5.Visible = true;
                button4.Visible = true;

                // Update settings.
                numericUpDown1.Value = CoreKeepersWorkshop.Properties.Settings.Default.InfoID;
                numericUpDown2.Value = CoreKeepersWorkshop.Properties.Settings.Default.InfoAmount;

                // New format. // Fix v1.3.5.6.
                int num4 = VariationHelper.GetIngredient1FromFoodVariation(CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation);
                int num5 = VariationHelper.GetIngredient2FromFoodVariation(CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation);

                // Ensure the largest value loads in the front.
                if (num5 > num4)
                {
                    // Flip values.
                    numericUpDown5.Value = num4;
                    numericUpDown4.Value = num5;
                }
                else
                {
                    numericUpDown4.Value = num4;
                    numericUpDown5.Value = num5;
                }

                // Legacy format.
                // numericUpDown4.Value = decimal.Parse(CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation.ToString().Substring(0, CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation.ToString().Length / 2));
                // numericUpDown5.Value = decimal.Parse(CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation.ToString().Substring(CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation.ToString().Length / 2));

                numericUpDown11.Value = CoreKeepersWorkshop.Properties.Settings.Default.InfoSkillset;

                // Rename button label.
                label2.Text = "Variation [Food Ingredients]";
            }
            else
            {
                // None food variant, keep normal settings.
                numericUpDown1.Value = CoreKeepersWorkshop.Properties.Settings.Default.InfoID;
                numericUpDown2.Value = CoreKeepersWorkshop.Properties.Settings.Default.InfoAmount;
                numericUpDown3.Value = CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation;
                numericUpDown11.Value = CoreKeepersWorkshop.Properties.Settings.Default.InfoSkillset;
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
                if (!numericUpDown3.Visible) // Check if item is a food variant.
                {
                    // Check if both entrees are populated.
                    if (numericUpDown5.Value != 0)
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
                        CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation = VariationHelper.GetFoodVariation((int)numericUpDown4.Value, (int)numericUpDown5.Value);

                        // Legacy format.
                        // CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation = int.Parse(numericUpDown4.Value.ToString() + numericUpDown5.Value.ToString());
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
                CoreKeepersWorkshop.Properties.Settings.Default.InfoSkillset = (int)numericUpDown11.Value;
                CoreKeepersWorkshop.Properties.Settings.Default.ItemEditorLocation = this.Location;
            }
            catch (Exception)
            { } // Do nothing.
        }
        #endregion // Form loading and closing events.

        #region Keydown Events

        // Do enter events.
        private void NumericUpDown1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectedItemType = (int)numericUpDown1.Value;
                selectedItemAmount = (int)numericUpDown2.Value;
                selectedItemSkillset = (int)numericUpDown11.Value;
                if (!numericUpDown3.Visible) // Check if item is a food variant.
                {
                    // Check if both entrees are populated.
                    if (numericUpDown5.Value != 0)
                    {
                        // Do some checks and corrections for values over 8. // Fix v1.3.5.1.
                        if (numericUpDown4.Value > numericUpDown5.Value)
                        {
                            // Flip values.
                            decimal item2 = numericUpDown4.Value;
                            decimal item3 = numericUpDown5.Value;

                            numericUpDown4.Value = item3;
                            numericUpDown5.Value = item2;
                        }

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
        private void NumericUpDown2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectedItemType = (int)numericUpDown1.Value;
                selectedItemAmount = (int)numericUpDown2.Value;
                selectedItemSkillset = (int)numericUpDown11.Value;
                if (!numericUpDown3.Visible) // Check if item is a food variant.
                {
                    // Check if both entrees are populated.
                    if (numericUpDown5.Value != 0)
                    {
                        // Do some checks and corrections for values over 8. // Fix v1.3.5.1.
                        if (numericUpDown4.Value > numericUpDown5.Value)
                        {
                            // Flip values.
                            decimal item2 = numericUpDown4.Value;
                            decimal item3 = numericUpDown5.Value;

                            numericUpDown4.Value = item3;
                            numericUpDown5.Value = item2;
                        }

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
        private void NumericUpDown3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectedItemType = (int)numericUpDown1.Value;
                selectedItemAmount = (int)numericUpDown2.Value;
                selectedItemSkillset = (int)numericUpDown11.Value;
                if (!numericUpDown3.Visible) // Check if item is a food variant.
                {
                    // Check if both entrees are populated.
                    if (numericUpDown5.Value != 0)
                    {
                        // Do some checks and corrections for values over 8. // Fix v1.3.5.1.
                        if (numericUpDown4.Value > numericUpDown5.Value)
                        {
                            // Flip values.
                            decimal item2 = numericUpDown4.Value;
                            decimal item3 = numericUpDown5.Value;

                            numericUpDown4.Value = item3;
                            numericUpDown5.Value = item2;
                        }

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
        private void NumericUpDown4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectedItemType = (int)numericUpDown1.Value;
                selectedItemAmount = (int)numericUpDown2.Value;
                selectedItemSkillset = (int)numericUpDown11.Value;
                if (!numericUpDown3.Visible) // Check if item is a food variant.
                {
                    // Check if both entrees are populated.
                    if (numericUpDown5.Value != 0)
                    {
                        // Do some checks and corrections for values over 8. // Fix v1.3.5.1.
                        if (numericUpDown4.Value > numericUpDown5.Value)
                        {
                            // Flip values.
                            decimal item2 = numericUpDown4.Value;
                            decimal item3 = numericUpDown5.Value;

                            numericUpDown4.Value = item3;
                            numericUpDown5.Value = item2;
                        }

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
        private void NumericUpDown5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectedItemType = (int)numericUpDown1.Value;
                selectedItemAmount = (int)numericUpDown2.Value;
                selectedItemSkillset = (int)numericUpDown11.Value;
                if (!numericUpDown3.Visible) // Check if item is a food variant.
                {
                    // Check if both entrees are populated.
                    if (numericUpDown5.Value != 0)
                    {
                        // Do some checks and corrections for values over 8. // Fix v1.3.5.1.
                        if (numericUpDown4.Value > numericUpDown5.Value)
                        {
                            // Flip values.
                            decimal item2 = numericUpDown4.Value;
                            decimal item3 = numericUpDown5.Value;

                            numericUpDown4.Value = item3;
                            numericUpDown5.Value = item2;
                        }

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
        private void NumericUpDown11_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectedItemType = (int)numericUpDown1.Value;
                selectedItemAmount = (int)numericUpDown2.Value;
                selectedItemSkillset = (int)numericUpDown11.Value;
                if (!numericUpDown3.Visible) // Check if item is a food variant.
                {
                    // Check if both entrees are populated.
                    if (numericUpDown5.Value != 0)
                    {
                        // Do some checks and corrections for values over 8. // Fix v1.3.5.1.
                        if (numericUpDown4.Value > numericUpDown5.Value)
                        {
                            // Flip values.
                            decimal item2 = numericUpDown4.Value;
                            decimal item3 = numericUpDown5.Value;

                            numericUpDown4.Value = item3;
                            numericUpDown5.Value = item2;
                        }

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
        private void Button5_Click(object sender, EventArgs e)
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
        private void Label2_Click(object sender, EventArgs e)
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
                label2.Text = "Variation [Custom]";

                // Update item variant
                numericUpDown3.Value = VariationHelper.GetFoodVariation((int)numericUpDown4.Value, (int)numericUpDown5.Value);

                // Reload all pictureboxes and labels.
                ReloadPictureBoxes(useTextboxeData: true);
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
                label2.Text = "Variation [Food Ingredients]";

                // Update food variant
                numericUpDown4.Value = VariationHelper.GetIngredient1FromFoodVariation((int)numericUpDown3.Value);
                numericUpDown5.Value = VariationHelper.GetIngredient2FromFoodVariation((int)numericUpDown3.Value);

                // Reload all pictureboxes and labels.
                ReloadPictureBoxes(useTextboxeData: true);
            }
        }

        // Open food cookbook.
        private void Button6_Click(object sender, EventArgs e)
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
        private void NumericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            // Reload all pictureboxes and labels from the textboxe data.
            ReloadPictureBoxes(useTextboxeData: true);
        }

        // Value had changed, reload images and labels.
        private void NumericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            // Reload all pictureboxes and labels from the textboxe data.
            ReloadPictureBoxes(useTextboxeData: true);
        }

        // Value had changed, reload images and labels.
        private void NumericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            // Reload all pictureboxes and labels from the textboxe data.
            ReloadPictureBoxes(useTextboxeData: true);
        }

        // Value had changed, reload images and labels.
        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            // Reload all pictureboxes and labels from the textboxe data.
            ReloadPictureBoxes(useTextboxeData: true);
        }

        // Change item rarity.
        private void Button1_Click(object sender, EventArgs e)
        {
            string originalName = "Unkown";
            string originalRarity = "Uncommon";
            int originalBase = (int)numericUpDown1.Value;
            bool foundNewRarity = false;

            int originalVariation;
            if (!numericUpDown3.Visible) // Check if item is a food variant.
            {
                // Check if both entrees are populated.
                if (numericUpDown5.Value != 0)
                {
                    // Combine strings into int.
                    originalVariation = VariationHelper.GetFoodVariation((int)numericUpDown4.Value, (int)numericUpDown5.Value);
                }
                else
                {
                    // Only single value exists, treat as a unique variant value.
                    originalVariation = VariationHelper.GetFoodVariation((int)numericUpDown4.Value, 0);
                }
            }
            else
            {
                // Normal item variant.
                originalVariation = (int)numericUpDown3.Value;
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
                                numericUpDown1.Value = int.Parse(foodID);

                                // Update variation.
                                if (!numericUpDown3.Visible) // Check if item is a food variant.
                                {
                                    // Populate both textboxes.
                                    numericUpDown4.Value = VariationHelper.GetIngredient1FromFoodVariation(int.Parse(foodVariation));
                                    numericUpDown5.Value = VariationHelper.GetIngredient2FromFoodVariation(int.Parse(foodVariation));

                                    // Legacy format.
                                    // numericUpDown4.Value = int.Parse(foodVariation.Substring(0, foodVariation.Length / 2));
                                    // numericUpDown5.Value = int.Parse(foodVariation.Substring(foodVariation.ToString().Length / 2));
                                }
                                else
                                {
                                    // Normal item variant.
                                    numericUpDown3.Value = int.Parse(foodVariation);
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
            this.ActiveControl = numericUpDown1;

            // Check if a new item rarity was found or not.
            if (!foundNewRarity)
            {
                MessageBox.Show("No alternative rarity found for this item.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Lanch about skillset message.
        private void Button13_Click(object sender, EventArgs e)
        {
            MessageBox.Show("How to find my pet's skillset ID?\r\n    - Each pet only has 4 skillsets and are typically +4/-4 the current pets skillset. For example, if your pet has an assigned skillset of 150 then the 4 skillset IDs will be within the 146-154 range. For fresh pets first name the pet and it will be assigned a skillset id.\r\n\r\nWhy is it like this?\r\n    - The game generates progressive \"skill tables\" rather than using a static ID for a pets skills. Bad system I know, nothing I can do about it.\r\n", "How To Use --> Get Skillset ID", MessageBoxButtons.OK, MessageBoxIcon.Question);
            return;
        }
        #endregion // End form controls.

        #region Click Events

        #region Quick Quantity Selector.

        // Quick change the items quantity to 1.
        private void Button7_Click(object sender, EventArgs e)
        {
            // Change the existing items quanitity. 
            selectedItemType = (int)numericUpDown1.Value;
            selectedItemAmount = (int)numericUpDown6.Value; // QQS Button 1.
            selectedItemSkillset = (int)numericUpDown11.Value;

            // Reserve the items variation.
            if (!numericUpDown3.Visible) // Check if item is a food variant.
            {
                // Check if both entrees are populated.
                if (numericUpDown5.Value != 0)
                {
                    // Do some checks and corrections for values over 8. // Fix v1.3.5.1.
                    if (numericUpDown4.Value > numericUpDown5.Value)
                    {
                        // Flip values.
                        decimal item2 = numericUpDown4.Value;
                        decimal item3 = numericUpDown5.Value;

                        numericUpDown4.Value = item3;
                        numericUpDown5.Value = item2;
                    }

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

        // Quick change the items quantity to 50.
        private void Button8_Click(object sender, EventArgs e)
        {
            // Change the existing items quanitity. 
            selectedItemType = (int)numericUpDown1.Value;
            selectedItemAmount = (int)numericUpDown7.Value; // QQS Button 2.
            selectedItemSkillset = (int)numericUpDown11.Value;

            // Reserve the items variation.
            if (!numericUpDown3.Visible) // Check if item is a food variant.
            {
                // Check if both entrees are populated.
                if (numericUpDown5.Value != 0)
                {
                    // Do some checks and corrections for values over 8. // Fix v1.3.5.1.
                    if (numericUpDown4.Value > numericUpDown5.Value)
                    {
                        // Flip values.
                        decimal item2 = numericUpDown4.Value;
                        decimal item3 = numericUpDown5.Value;

                        numericUpDown4.Value = item3;
                        numericUpDown5.Value = item2;
                    }

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

        // Quick change the items quantity to 500.
        private void Button9_Click(object sender, EventArgs e)
        {
            // Change the existing items quanitity. 
            selectedItemType = (int)numericUpDown1.Value;
            selectedItemAmount = (int)numericUpDown8.Value; // QQS Button 3.
            selectedItemSkillset = (int)numericUpDown11.Value;

            // Reserve the items variation.
            if (!numericUpDown3.Visible) // Check if item is a food variant.
            {
                // Check if both entrees are populated.
                if (numericUpDown5.Value != 0)
                {
                    // Do some checks and corrections for values over 8. // Fix v1.3.5.1.
                    if (numericUpDown4.Value > numericUpDown5.Value)
                    {
                        // Flip values.
                        decimal item2 = numericUpDown4.Value;
                        decimal item3 = numericUpDown5.Value;

                        numericUpDown4.Value = item3;
                        numericUpDown5.Value = item2;
                    }

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

        // Quick change the items quantity to 5000.
        private void Button10_Click(object sender, EventArgs e)
        {
            // Change the existing items quanitity. 
            selectedItemType = (int)numericUpDown1.Value;
            selectedItemAmount = (int)numericUpDown9.Value; // QQS Button 4.
            selectedItemSkillset = (int)numericUpDown11.Value;

            // Reserve the items variation.
            if (!numericUpDown3.Visible) // Check if item is a food variant.
            {
                // Check if both entrees are populated.
                if (numericUpDown5.Value != 0)
                {
                    // Do some checks and corrections for values over 8. // Fix v1.3.5.1.
                    if (numericUpDown4.Value > numericUpDown5.Value)
                    {
                        // Flip values.
                        decimal item2 = numericUpDown4.Value;
                        decimal item3 = numericUpDown5.Value;

                        numericUpDown4.Value = item3;
                        numericUpDown5.Value = item2;
                    }

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

        // Quick change the items quantity to 9999.
        private void Button11_Click(object sender, EventArgs e)
        {
            // Change the existing items quanitity. 
            selectedItemType = (int)numericUpDown1.Value;
            selectedItemAmount = (int)numericUpDown10.Value; // QQS Button 5.
            selectedItemSkillset = (int)numericUpDown11.Value;

            // Reserve the items variation.
            if (!numericUpDown3.Visible) // Check if item is a food variant.
            {
                // Check if both entrees are populated.
                if (numericUpDown5.Value != 0)
                {
                    // Do some checks and corrections for values over 8. // Fix v1.3.5.1.
                    if (numericUpDown4.Value > numericUpDown5.Value)
                    {
                        // Flip values.
                        decimal item2 = numericUpDown4.Value;
                        decimal item3 = numericUpDown5.Value;

                        numericUpDown4.Value = item3;
                        numericUpDown5.Value = item2;
                    }

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
        #endregion // End quick quantity selector.

        #region Set Custom Quick Quantity Selector Values

        // QQS Button 1.
        private void Button7_MouseDown(object sender, MouseEventArgs e)
        {
            // Detect if the right mouse was pressed.
            if (e.Button == MouseButtons.Right)
            {
                // Unhide the numericupdown and hide button.
                button7.Visible = false;
                numericUpDown6.Visible = true;
            }
        }
        private void NumericUpDown6_KeyDown(object sender, KeyEventArgs e)
        {
            // Detect if enter was pressesed.
            if (e.KeyCode == Keys.Enter)
            {
                // Save settings.
                CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton1 = (int)numericUpDown6.Value;

                // Change button text.
                button7.Text = ((int)numericUpDown6.Value).ToString();

                // Hide the numericupdown and unhide button.
                button7.Visible = true;
                numericUpDown6.Visible = false;
            }
        }

        // QQS Button 2.
        private void Button8_MouseDown(object sender, MouseEventArgs e)
        {
            // Detect if the right mouse was pressed.
            if (e.Button == MouseButtons.Right)
            {
                // Unhide the numericupdown and hide button.
                button8.Visible = false;
                numericUpDown7.Visible = true;
            }
        }
        private void NumericUpDown7_KeyDown(object sender, KeyEventArgs e)
        {
            // Detect if enter was pressesed.
            if (e.KeyCode == Keys.Enter)
            {
                // Save settings.
                CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton2 = (int)numericUpDown7.Value;

                // Change button text.
                button8.Text = ((int)numericUpDown7.Value).ToString();

                // Hide the numericupdown and unhide button.
                button8.Visible = true;
                numericUpDown7.Visible = false;
            }
        }

        // QQS Button 3.
        private void Button9_MouseDown(object sender, MouseEventArgs e)
        {
            // Detect if the right mouse was pressed.
            if (e.Button == MouseButtons.Right)
            {
                // Unhide the numericupdown and hide button.
                button9.Visible = false;
                numericUpDown8.Visible = true;
            }
        }
        private void NumericUpDown8_KeyDown(object sender, KeyEventArgs e)
        {
            // Detect if enter was pressesed.
            if (e.KeyCode == Keys.Enter)
            {
                // Save settings.
                CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton3 = (int)numericUpDown8.Value;

                // Change button text.
                button9.Text = ((int)numericUpDown8.Value).ToString();

                // Hide the numericupdown and unhide button.
                button9.Visible = true;
                numericUpDown8.Visible = false;
            }
        }

        // QQS Button 4.
        private void Button10_MouseDown(object sender, MouseEventArgs e)
        {
            // Detect if the right mouse was pressed.
            if (e.Button == MouseButtons.Right)
            {
                // Unhide the numericupdown and hide button.
                button10.Visible = false;
                numericUpDown9.Visible = true;
            }
        }
        private void NumericUpDown9_KeyDown(object sender, KeyEventArgs e)
        {
            // Detect if enter was pressesed.
            if (e.KeyCode == Keys.Enter)
            {
                // Save settings.
                CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton4 = (int)numericUpDown9.Value;

                // Change button text.
                button10.Text = ((int)numericUpDown9.Value).ToString();

                // Hide the numericupdown and unhide button.
                button10.Visible = true;
                numericUpDown9.Visible = false;
            }
        }

        // QQS Button 5.
        private void Button11_MouseDown(object sender, MouseEventArgs e)
        {
            // Detect if the right mouse was pressed.
            if (e.Button == MouseButtons.Right)
            {
                // Unhide the numericupdown and hide button.
                button11.Visible = false;
                numericUpDown10.Visible = true;
            }
        }
        private void NumericUpDown10_KeyDown(object sender, KeyEventArgs e)
        {
            // Detect if enter was pressesed.
            if (e.KeyCode == Keys.Enter)
            {
                // Save settings.
                CoreKeepersWorkshop.Properties.Settings.Default.QuantitySelectButton5 = (int)numericUpDown10.Value;

                // Change button text.
                button11.Text = ((int)numericUpDown10.Value).ToString();

                // Hide the numericupdown and unhide button.
                button11.Visible = true;
                numericUpDown10.Visible = false;
            }
        }
        #endregion // End set quick quantity selector values.

        // Launch item explorer.
        private void PictureBox1_Click(object sender, EventArgs e)
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
            numericUpDown1.Value = itemType;

            // Reload pictureboxes and labels.
            ReloadPictureBoxes(useTextboxeData: true);
        }

        // Launch item explorer.
        private void PictureBox2_Click(object sender, EventArgs e)
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
        private void PictureBox3_Click(object sender, EventArgs e)
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
            numericUpDown5.Value = itemType;

            // Reload pictureboxes and labels.
            ReloadPictureBoxes(useTextboxeData: true);
        }

        // User clicked done, save and close form.
        private void Button3_Click(object sender, EventArgs e)
        {
            selectedItemType = (int)numericUpDown1.Value;
            selectedItemAmount = (int)numericUpDown2.Value;
            selectedItemSkillset = (int)numericUpDown11.Value;
            if (!numericUpDown3.Visible) // Check if item is a food variant.
            {
                // Check if both entrees are populated.
                if (numericUpDown5.Value != 0)
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
                    selectedItemVariation = VariationHelper.GetFoodVariation((int)numericUpDown4.Value, (int)numericUpDown5.Value);

                    // Legacy format.
                    // selectedItemVariation = int.Parse(numericUpDown4.Value.ToString() + numericUpDown5.Value.ToString());
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
        #endregion
    }
}