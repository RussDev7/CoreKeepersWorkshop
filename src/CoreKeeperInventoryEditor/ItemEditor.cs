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
                pictureBox1.Image = new Bitmap(Image.FromFile(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseItemID.ToString())));
                pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
            }
            else
            {
                // Check if the first item is not empty.
                if (numericUpDown1.Value != 0)
                {
                    label3.Text = "UnkownItem";

                    // Load image.
                    pictureBox1.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                    pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;
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
                if (baseItemVariation.ToString().Length == 8)
                {
                    // Get base item ingrdient 1 name.
                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseItemVariation.ToString().Substring(0, baseItemVariation.ToString().Length / 2).ToString()) != null)
                    {
                        label4.Text = Path.GetFileName(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseItemVariation.ToString().Substring(0, baseItemVariation.ToString().Length / 2).ToString())).Split(',')[0];

                        // Load image.
                        pictureBox2.Image = new Bitmap(Image.FromFile(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseItemVariation.ToString().Substring(0, baseItemVariation.ToString().Length / 2).ToString())));
                        pictureBox2.SizeMode = PictureBoxSizeMode.CenterImage;
                    }
                    else
                    {
                        // Check if the variant item two is not empty.
                        if (numericUpDown3.Value != 0 || numericUpDown4.Value != 0)
                        {
                            label4.Text = "UnkownItem";

                            // Load image.
                            pictureBox2.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox2.SizeMode = PictureBoxSizeMode.CenterImage;
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
                if (baseItemVariation.ToString().Length == 8 && !numericUpDown3.Visible)
                {
                    // Get base item ingrdient 2 name.
                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseItemVariation.ToString().Substring(baseItemVariation.ToString().Length / 2).ToString()) != null)
                    {
                        label5.Text = Path.GetFileName(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseItemVariation.ToString().Substring(baseItemVariation.ToString().Length / 2).ToString())).Split(',')[0];

                        // Load image.
                        pictureBox3.Image = new Bitmap(Image.FromFile(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseItemVariation.ToString().Substring(baseItemVariation.ToString().Length / 2).ToString())));
                        pictureBox3.SizeMode = PictureBoxSizeMode.CenterImage;
                    }
                    else
                    {
                        // Check if the third item is not empty.
                        if (numericUpDown5.Value != 0)
                        {
                            label5.Text = "UnkownItem";

                            // Load image.
                            pictureBox3.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox3.SizeMode = PictureBoxSizeMode.CenterImage;
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
                    // Get base item ingrdient 1 name.
                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseIngredient1ID) != null)
                    {
                        label4.Text = Path.GetFileName(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseIngredient1ID)).Split(',')[0];

                        // Load image.
                        pictureBox2.Image = new Bitmap(Image.FromFile(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseIngredient1ID)));
                        pictureBox2.SizeMode = PictureBoxSizeMode.CenterImage;
                    }
                    else
                    {
                        // Check if the variant item two is not empty.
                        if (numericUpDown3.Value != 0 || numericUpDown4.Value != 0)
                        {
                            label4.Text = "UnkownItem";

                            // Load image.
                            pictureBox2.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox2.SizeMode = PictureBoxSizeMode.CenterImage;
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
                if (baseIngredient2ID.ToString().Length > 0 && !numericUpDown3.Visible) // Make sure duel texbox mode is enabled.
                {
                    // Get base item ingrdient 2 name.
                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseIngredient2ID) != null)
                    {
                        label5.Text = Path.GetFileName(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseIngredient2ID)).Split(',')[0];

                        // Load image.
                        pictureBox3.Image = new Bitmap(Image.FromFile(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == baseIngredient2ID)));
                        pictureBox3.SizeMode = PictureBoxSizeMode.CenterImage;
                    }
                    else
                    {
                        // Check if the third item is not empty.
                        if (numericUpDown5.Value != 0)
                        {
                            label5.Text = "UnkownItem";

                            // Load image.
                            pictureBox3.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox3.SizeMode = PictureBoxSizeMode.CenterImage;
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
            #region Set Form Locations

            // Set the forms active location based on previous save.
            this.Location = CoreKeepersWorkshop.Properties.Settings.Default.ItemEditorLocation;
            #endregion

            #region Tooltips

            // Create a new tooltip.
            ToolTip toolTip = new ToolTip()
            {
                AutoPopDelay = 3000,
                InitialDelay = 1000
            };

            // Set tool texts.
            toolTip.SetToolTip(numericUpDown1, "Enter the ID you wish to add. Press enter when done.");
            toolTip.SetToolTip(numericUpDown2, "Enter a custom item quantity. Press enter when done.");
            toolTip.SetToolTip(numericUpDown3, "Enter a custom variant ID. Press enter when done.");
            toolTip.SetToolTip(numericUpDown4, "Enter an ingredient one ID. Press enter when done.");
            toolTip.SetToolTip(numericUpDown5, "Enter an ingredient two ID. Press enter when done.");

            toolTip.SetToolTip(label2, "Toggle the GUI between food / item variaty.");
            toolTip.SetToolTip(button1, "Change your food rarity. Press enter when done.");
            toolTip.SetToolTip(button5, "Remove the item from this inventory slot.");
            toolTip.SetToolTip(button6, "Open the food cookbook to easily search for food items.");

            toolTip.SetToolTip(pictureBox1, "Click to open the item explorer.");
            toolTip.SetToolTip(pictureBox2, "Click to open the item explorer.");
            toolTip.SetToolTip(pictureBox3, "Click to open the item explorer.");
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
                label2.Text = "Variation [Food Ingredients]";
            }
            else
            {
                // None food variant, keep normal settings.
                numericUpDown1.Value = CoreKeepersWorkshop.Properties.Settings.Default.InfoID;
                numericUpDown2.Value = CoreKeepersWorkshop.Properties.Settings.Default.InfoAmount;
                numericUpDown3.Value = CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation;
            }

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
        private void NumericUpDown2_KeyDown(object sender, KeyEventArgs e)
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
        private void NumericUpDown3_KeyDown(object sender, KeyEventArgs e)
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
        private void NumericUpDown4_KeyDown(object sender, KeyEventArgs e)
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
        private void NumericUpDown5_KeyDown(object sender, KeyEventArgs e)
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
        private void Button5_Click(object sender, EventArgs e)
        {
            selectedItemType = 0;
            selectedItemAmount = 1;
            selectedItemVariation = 0;
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
                label2.Text = "Variation [Items]";

                // Update item variant
                numericUpDown3.Value = numericUpDown4.Value;

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
                numericUpDown4.Value = numericUpDown3.Value;

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
                    originalVariation = int.Parse(numericUpDown4.Value.ToString() + numericUpDown5.Value.ToString());
                }
                else
                {
                    // Only single value exists, treat as a unique variant value.
                    originalVariation = (int)numericUpDown4.Value;
                }
            }
            else
            {
                // Normal item variant.
                originalVariation = (int)numericUpDown3.Value;
            }

            // Ensure original variation is 8 in lengh.
            if (originalVariation.ToString().Length == 8)
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
                                    numericUpDown4.Value = int.Parse(foodVariation.Substring(0, foodVariation.Length / 2));
                                    numericUpDown5.Value = int.Parse(foodVariation.Substring(foodVariation.ToString().Length / 2));
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
        #endregion // End form controls.

        #region Click Events

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
            if (numericUpDown3.Visible)
            {
                numericUpDown3.Value = itemType;
            }
            else
            {
                numericUpDown4.Value = itemType;
            }

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
        #endregion


    }
}
