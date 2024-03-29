﻿using CoreKeepersWorkshop;
using Siticone.UI.WinForms.Suite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CoreKeeperInventoryEditor
{
    public partial class InventoryEditor : Form
    {
        // Form closing saving.
        int selectedItemType = 0;
        int selectedItemAmount = 0;
        int selectedItemVariation = 0;
        int selectedItemSkillset = 0;
        bool selectedOverwrite = false;
        bool userCancledTask = false;

        // Define texture data.
        public IEnumerable<string> ImageFiles = Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"assets\Inventory\") && Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"assets\Inventory\", "*.png", SearchOption.AllDirectories) != null ? Directory.GetFileSystemEntries(AppDomain.CurrentDomain.BaseDirectory + @"assets\Inventory\", "*.png", SearchOption.AllDirectories) : new String[] { "" }; // Ensure directory exists and images exist. Fix: v1.2.9.

        // Define folder names.
        public IEnumerable<string> FolderNames = Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"assets\Inventory\") ? Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + @"assets\Inventory\") : new String[] { "Null", "Null", "Null", "Null", "Null", "Null", "Null", "Null", "Null", "Null", "Null", "Null", "Null", "Null", "Null" }; // Ensure directory exists. Fix: v1.2.9.

        // Define imagelist.
        readonly ImageList ImagelistTools = new ImageList();
        readonly ImageList ImagelistLargeTools = new ImageList();
        readonly ImageList ImagelistPlaceableItems = new ImageList();
        readonly ImageList ImagelistLargePlaceableItems = new ImageList();
        readonly ImageList ImagelistNature = new ImageList();
        readonly ImageList ImagelistLargeNature = new ImageList();
        readonly ImageList ImagelistMaterials = new ImageList();
        readonly ImageList ImagelistLargeMaterials = new ImageList();
        readonly ImageList ImagelistSpecial = new ImageList();
        readonly ImageList ImagelistLargeSpecial = new ImageList();
        readonly ImageList ImagelistMobItems = new ImageList();
        readonly ImageList ImagelistLargeMobItems = new ImageList();
        readonly ImageList ImagelistBaseBuilding = new ImageList();
        readonly ImageList ImagelistLargeBaseBuilding = new ImageList();
        readonly ImageList ImagelistTreasures = new ImageList();
        readonly ImageList ImagelistLargeTreasures = new ImageList();
        readonly ImageList ImagelistWiring = new ImageList();
        readonly ImageList ImagelistLargeWiring = new ImageList();
        readonly ImageList ImagelistPlants = new ImageList();
        readonly ImageList ImagelistLargePlants = new ImageList();
        readonly ImageList ImagelistArmors = new ImageList();
        readonly ImageList ImagelistLargeArmors = new ImageList();
        readonly ImageList ImagelistAccessories = new ImageList();
        readonly ImageList ImagelistLargeAccessories = new ImageList();
        readonly ImageList ImagelistWeapons = new ImageList();
        readonly ImageList ImagelistLargeWeapons = new ImageList();
        readonly ImageList ImagelistConsumables = new ImageList();
        readonly ImageList ImagelistLargeConsumables = new ImageList();
        readonly ImageList ImagelistSeasonal = new ImageList();
        readonly ImageList ImagelistLargeSeasonal = new ImageList();
        readonly ImageList ImagelistSearch = new ImageList();
        readonly ImageList ImagelistLargeSearch = new ImageList();

        public InventoryEditor()
        {
            InitializeComponent();
        }

        #region Form Controls

        // Do form closing events.
        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
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
                CoreKeepersWorkshop.Properties.Settings.Default.ItemAmount = (int)numericUpDown1.Value;
                CoreKeepersWorkshop.Properties.Settings.Default.ItemID = (int)numericUpDown2.Value;
                CoreKeepersWorkshop.Properties.Settings.Default.ItemVariation = (int)numericUpDown3.Value;
                CoreKeepersWorkshop.Properties.Settings.Default.ItemSkillset = (int)numericUpDown4.Value;
                CoreKeepersWorkshop.Properties.Settings.Default.InventoryEditorLocation = this.Location;

                // Ensure current tab is not search, if so, reset.
                if (tabControl1.SelectedTab == tabPage16)
                {
                    // Set value to tools.
                    CoreKeepersWorkshop.Properties.Settings.Default.CurrentItemTab = "tabPage1";
                }
                else
                {
                    // Save current tab.
                    CoreKeepersWorkshop.Properties.Settings.Default.CurrentItemTab = tabControl1.SelectedTab.Name;
                }
            }
            catch (Exception)
            { } // Do nothing.
        }

        // Lanch about skillset message.
        private void Button8_Click(object sender, EventArgs e)
        {
            MessageBox.Show("How to find my pet's skillset ID?\r\n    - Each pet only has 4 skillsets and are typically +4/-4 the current pets skillset. For example, if your pet has an assigned skillset of 150 then the 4 skillset IDs will be within the 146-154 range. For fresh pets first name the pet and it will be assigned a skillset id.\r\n\r\nWhy is it like this?\r\n    - The game generates progressive \"skill tables\" rather than using a static ID for a pets skills. Bad system I know, nothing I can do about it.\r\n", "How To Use --> Get Skillset ID", MessageBoxButtons.OK, MessageBoxIcon.Question);
            return;
        }
        #endregion

        #region Closing Varibles

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
        public bool GetSelectedOverwriteTask()
        {
            return selectedOverwrite;
        }
        #endregion

        // Do loading events.
        private void Form2_Load(object sender, EventArgs e)
        {
            #region Set Custom Cusror

            // Set the applications cursor.
            Cursor = new Cursor(CoreKeepersWorkshop.Properties.Resources.UICursor.GetHicon());
            #endregion

            #region Set Form Locations

            // Set the forms active location based on previous save.
            this.Location = CoreKeepersWorkshop.Properties.Settings.Default.InventoryEditorLocation;
            #endregion

            #region Tooltips

            // Create a new tooltip.
            ToolTip toolTip = new ToolTip()
            {
                AutoPopDelay = 5000,
                InitialDelay = 750
            };

            // Set tool texts.
            toolTip.SetToolTip(numericUpDown1, "Enter the amount of items to add.");
            toolTip.SetToolTip(numericUpDown2, "Enter a custom ID. Either press enter when done or use the button.");
            toolTip.SetToolTip(numericUpDown3, "Enter a custom variant ID. Either press enter when done or use the button.");
            toolTip.SetToolTip(numericUpDown4, "Enter a custom skillset ID. Either press enter when done or use the button.");

            toolTip.SetToolTip(button1, "Remove the item from this inventory slot.");
            toolTip.SetToolTip(button3, "Spawn in custom item amount + ID.");
            toolTip.SetToolTip(button4, "Start the search for a desired item.");
            toolTip.SetToolTip(button5, "Spawn in custom item with variation.");
            toolTip.SetToolTip(button6, "Open the food cookbook to easily search for food items.");
            toolTip.SetToolTip(button8, "Launch a guide on how to find skillset IDs.");

            toolTip.SetToolTip(textBox1, "Enter a name to search for.");

            #endregion

            #region Do Loading Events

            // Set tab text based on the folder names.
            tabControl1.TabPages[0].Text = new DirectoryInfo(FolderNames.ElementAt(0)).Name;
            tabControl1.TabPages[1].Text = new DirectoryInfo(FolderNames.ElementAt(1)).Name;
            tabControl1.TabPages[2].Text = new DirectoryInfo(FolderNames.ElementAt(2)).Name;
            tabControl1.TabPages[3].Text = new DirectoryInfo(FolderNames.ElementAt(3)).Name;
            tabControl1.TabPages[4].Text = new DirectoryInfo(FolderNames.ElementAt(4)).Name;
            tabControl1.TabPages[5].Text = new DirectoryInfo(FolderNames.ElementAt(5)).Name;
            tabControl1.TabPages[6].Text = new DirectoryInfo(FolderNames.ElementAt(6)).Name;
            tabControl1.TabPages[7].Text = new DirectoryInfo(FolderNames.ElementAt(7)).Name;
            tabControl1.TabPages[8].Text = new DirectoryInfo(FolderNames.ElementAt(8)).Name;
            tabControl1.TabPages[9].Text = new DirectoryInfo(FolderNames.ElementAt(9)).Name;
            tabControl1.TabPages[10].Text = new DirectoryInfo(FolderNames.ElementAt(10)).Name;
            tabControl1.TabPages[11].Text = new DirectoryInfo(FolderNames.ElementAt(11)).Name;
            tabControl1.TabPages[12].Text = new DirectoryInfo(FolderNames.ElementAt(12)).Name;
            tabControl1.TabPages[13].Text = new DirectoryInfo(FolderNames.ElementAt(13)).Name;
            tabControl1.TabPages[14].Text = new DirectoryInfo(FolderNames.ElementAt(14)).Name;

            // Ensure the skillset is more then -1.
            if (CoreKeepersWorkshop.Properties.Settings.Default.ItemSkillset < 0)
            {
                CoreKeepersWorkshop.Properties.Settings.Default.ItemSkillset = 0;
            }

            // Load some settings.
            numericUpDown1.Value = CoreKeepersWorkshop.Properties.Settings.Default.ItemAmount;
            numericUpDown2.Value = CoreKeepersWorkshop.Properties.Settings.Default.ItemID;
            numericUpDown3.Value = CoreKeepersWorkshop.Properties.Settings.Default.ItemVariation;
            numericUpDown4.Value = CoreKeepersWorkshop.Properties.Settings.Default.ItemSkillset;
            tabControl1.SelectedTab = tabControl1.TabPages[CoreKeepersWorkshop.Properties.Settings.Default.CurrentItemTab];

            // Set Image Size
            ImagelistLargeTools.ImageSize = new Size(50, 60);
            ImagelistPlaceableItems.ImageSize = new Size(50, 60);
            ImagelistLargeNature.ImageSize = new Size(50, 60);
            ImagelistLargeMaterials.ImageSize = new Size(50, 60);
            ImagelistLargeSpecial.ImageSize = new Size(50, 60);
            ImagelistLargeMobItems.ImageSize = new Size(50, 60);
            ImagelistLargeBaseBuilding.ImageSize = new Size(50, 60);
            ImagelistLargeTreasures.ImageSize = new Size(50, 60);
            ImagelistLargeWiring.ImageSize = new Size(50, 60);
            ImagelistLargePlants.ImageSize = new Size(50, 60);
            ImagelistLargeArmors.ImageSize = new Size(50, 60);
            ImagelistLargeAccessories.ImageSize = new Size(50, 60);
            ImagelistLargeWeapons.ImageSize = new Size(50, 60);
            ImagelistLargeConsumables.ImageSize = new Size(50, 60);
            ImagelistLargeSeasonal.ImageSize = new Size(50, 60);

            // Define seperate counts.
            int countTools = 0;
            int countPlaceableItems = 0;
            int countNature = 0;
            int countMaterials = 0;
            int countSpecial = 0;
            int countMobItems = 0;
            int countBaseBuilding = 0;
            int countTreasures = 0;
            int countWiring = 0;
            int countPlants = 0;
            int countArmors = 0;
            int countAccessories = 0;
            int countWeapons = 0;
            int countConsumables = 0;
            int countSeasonal = 0;

            // Make sure assets exist.
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\assets\Inventory\"))
            {
                // Get each folder in inventory.
                foreach (var catergoryFolder in Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + @"\assets\Inventory\"))
                {
                    // Get current folder name.
                    var catergoryName = new DirectoryInfo(catergoryFolder).Name;

                    // Retrieve all image files
                    foreach (var file in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"\assets\Inventory\" + catergoryName))
                    {
                        // Get file infomration.
                        var fi = new FileInfo(file);
                        string[] filenameData = fi.Name.Split(',');

                        // Catch desktop.ini from throwing errors.
                        if (filenameData[0] == "desktop.ini") continue;
                        if (filenameData[0] == "Thumbs.db") continue;

                        // Place items in listview.
                        try
                        {
                            if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(0)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistTools.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeTools.Images.Add(ImageFast.FromFile(file));

                                listView1.LargeImageList = ImagelistTools;

                                // Save filename information.
                                listView1.Items.Add(new ListViewItem { ImageIndex = countTools, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countTools++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(1)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistPlaceableItems.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargePlaceableItems.Images.Add(ImageFast.FromFile(file));

                                listView2.LargeImageList = ImagelistPlaceableItems;

                                // Save filename information.
                                listView2.Items.Add(new ListViewItem { ImageIndex = countPlaceableItems, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countPlaceableItems++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(2)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistNature.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeNature.Images.Add(ImageFast.FromFile(file));

                                listView3.LargeImageList = ImagelistNature;

                                // Save filename information.
                                listView3.Items.Add(new ListViewItem { ImageIndex = countNature, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countNature++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(3)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistMaterials.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeMaterials.Images.Add(ImageFast.FromFile(file));

                                listView4.LargeImageList = ImagelistMaterials;

                                // Save filename information.
                                listView4.Items.Add(new ListViewItem { ImageIndex = countMaterials, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countMaterials++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(4)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistSpecial.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeSpecial.Images.Add(ImageFast.FromFile(file));

                                listView5.LargeImageList = ImagelistSpecial;

                                // Save filename information.
                                listView5.Items.Add(new ListViewItem { ImageIndex = countSpecial, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countSpecial++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(5)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistMobItems.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeMobItems.Images.Add(ImageFast.FromFile(file));

                                listView6.LargeImageList = ImagelistMobItems;

                                // Save filename information.
                                listView6.Items.Add(new ListViewItem { ImageIndex = countMobItems, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countMobItems++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(6)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistBaseBuilding.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeBaseBuilding.Images.Add(ImageFast.FromFile(file));

                                listView7.LargeImageList = ImagelistBaseBuilding;

                                // Save filename information.
                                listView7.Items.Add(new ListViewItem { ImageIndex = countBaseBuilding, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countBaseBuilding++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(7)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistTreasures.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeTreasures.Images.Add(ImageFast.FromFile(file));

                                listView8.LargeImageList = ImagelistTreasures;

                                // Save filename information.
                                listView8.Items.Add(new ListViewItem { ImageIndex = countTreasures, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countTreasures++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(8)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistWiring.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeWiring.Images.Add(ImageFast.FromFile(file));

                                listView9.LargeImageList = ImagelistWiring;

                                // Save filename information.
                                listView9.Items.Add(new ListViewItem { ImageIndex = countWiring, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countWiring++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(9)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistPlants.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargePlants.Images.Add(ImageFast.FromFile(file));

                                listView10.LargeImageList = ImagelistPlants;

                                // Save filename information.
                                listView10.Items.Add(new ListViewItem { ImageIndex = countPlants, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countPlants++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(10)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistArmors.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeArmors.Images.Add(ImageFast.FromFile(file));

                                listView11.LargeImageList = ImagelistArmors;

                                // Save filename information.
                                listView11.Items.Add(new ListViewItem { ImageIndex = countArmors, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countArmors++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(11)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistAccessories.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeAccessories.Images.Add(ImageFast.FromFile(file));

                                listView12.LargeImageList = ImagelistAccessories;

                                // Save filename information.
                                listView12.Items.Add(new ListViewItem { ImageIndex = countAccessories, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countAccessories++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(12)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistWeapons.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeWeapons.Images.Add(ImageFast.FromFile(file));

                                listView13.LargeImageList = ImagelistWeapons;

                                // Save filename information.
                                listView13.Items.Add(new ListViewItem { ImageIndex = countWeapons, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countWeapons++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(13)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistConsumables.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeConsumables.Images.Add(ImageFast.FromFile(file));

                                listView14.LargeImageList = ImagelistConsumables;

                                // Save filename information.
                                listView14.Items.Add(new ListViewItem { ImageIndex = countConsumables, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countConsumables++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(14)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistSeasonal.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeSeasonal.Images.Add(ImageFast.FromFile(file));

                                listView15.LargeImageList = ImagelistSeasonal;

                                // Save filename information.
                                listView15.Items.Add(new ListViewItem { ImageIndex = countSeasonal, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countSeasonal++; // Add one to index count.
                            }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("The filename: \"" + filenameData[0] + "\" has thrown an error while loading!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }
                    }
                }

                //set the amall and large ImageList properties of listview
                listView1.LargeImageList = ImagelistLargeTools;
                listView1.View = View.LargeIcon;
                listView2.LargeImageList = ImagelistPlaceableItems;
                listView2.View = View.LargeIcon;
                listView3.LargeImageList = ImagelistLargeNature;
                listView3.View = View.LargeIcon;
                listView4.LargeImageList = ImagelistLargeMaterials;
                listView4.View = View.LargeIcon;
                listView5.LargeImageList = ImagelistLargeSpecial;
                listView5.View = View.LargeIcon;
                listView6.LargeImageList = ImagelistLargeMobItems;
                listView6.View = View.LargeIcon;
                listView7.LargeImageList = ImagelistLargeBaseBuilding;
                listView7.View = View.LargeIcon;
                listView8.LargeImageList = ImagelistLargeTreasures;
                listView8.View = View.LargeIcon;
                listView9.LargeImageList = ImagelistLargeWiring;
                listView9.View = View.LargeIcon;
                listView10.LargeImageList = ImagelistLargePlants;
                listView10.View = View.LargeIcon;
                listView11.LargeImageList = ImagelistLargeArmors;
                listView11.View = View.LargeIcon;
                listView12.LargeImageList = ImagelistLargeAccessories;
                listView12.View = View.LargeIcon;
                listView13.LargeImageList = ImagelistLargeWeapons;
                listView13.View = View.LargeIcon;
                listView14.LargeImageList = ImagelistLargeConsumables;
                listView14.View = View.LargeIcon;
                listView15.LargeImageList = ImagelistLargeSeasonal;
                listView15.View = View.LargeIcon;
            }

            #endregion
        }

        #region Main Form Controls

        // Add custom ID.
        private void Button3_Click(object sender, EventArgs e)
        {
            selectedItemType = (int)numericUpDown2.Value;
            selectedItemAmount = (int)numericUpDown1.Value;
            selectedItemVariation = (int)numericUpDown3.Value;
            selectedItemSkillset = (int)numericUpDown4.Value;
            selectedOverwrite = true;
            this.Close();
        }

        // Add custom veriation.
        private void Button5_Click(object sender, EventArgs e)
        {
            selectedItemType = (int)numericUpDown2.Value;
            selectedItemAmount = (int)numericUpDown1.Value;
            selectedItemVariation = (int)numericUpDown3.Value;
            selectedItemSkillset = (int)numericUpDown4.Value;
            selectedOverwrite = true;
            this.Close();
        }

        // Add custom skillset.
        private void Button7_Click(object sender, EventArgs e)
        {
            selectedItemType = (int)numericUpDown2.Value;
            selectedItemAmount = (int)numericUpDown1.Value;
            selectedItemVariation = (int)numericUpDown3.Value;
            selectedItemSkillset = (int)numericUpDown4.Value;
            selectedOverwrite = true;
            this.Close();
        }

        // Add custom veriation - Enter shortcut.
        private void NumericUpDown3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectedItemType = (int)numericUpDown2.Value;
                selectedItemAmount = (int)numericUpDown1.Value;
                selectedItemVariation = (int)numericUpDown3.Value;
                selectedItemSkillset = (int)numericUpDown4.Value;
                selectedOverwrite = true;
                this.Close();
            }
        }

        // Add custom ID - Enter shortcut.
        private void NumericUpDown2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectedItemType = (int)numericUpDown2.Value;
                selectedItemAmount = (int)numericUpDown1.Value;
                selectedItemVariation = (int)numericUpDown3.Value;
                selectedItemSkillset = (int)numericUpDown4.Value;
                selectedOverwrite = true;
                this.Close();
            }
        }

        // Add custom skillset - Enter shortcut.
        private void NumericUpDown4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectedItemType = (int)numericUpDown2.Value;
                selectedItemAmount = (int)numericUpDown1.Value;
                selectedItemVariation = (int)numericUpDown3.Value;
                selectedItemSkillset = (int)numericUpDown4.Value;
                selectedOverwrite = true;
                this.Close();
            }
        }

        // Remove item.
        private void Button2_Click(object sender, EventArgs e)
        {
            selectedItemType = 0;
            selectedItemAmount = 1;
            selectedItemVariation = 0;
            selectedItemSkillset = 0;
            this.Close();
        }

        // Search for item.
        private void Button4_Click(object sender, EventArgs e)
        {
            // Clear previous results.
            listView16.Clear();
            listView16.Items.Clear();
            listView16.Refresh();

            // Clear previous image data.
            ImagelistSearch.Images.Clear();
            ImagelistLargeSearch.Images.Clear();

            // Tab over to search.
            tabControl1.SelectedTab = tabPage16;

            // Set Image Size
            ImagelistLargeSearch.ImageSize = new Size(50, 60);
            ImagelistSearch.ImageSize = new Size(50, 60);

            // Define seperate counts.
            int countSearch = 0;

            // Make sure assets exist.
            if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\assets\Inventory\"))
            {
                // Get each folder in inventory.
                foreach (var catergoryFolder in Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + @"\assets\Inventory\"))
                {
                    // Get current folder name.
                    var catergoryName = new DirectoryInfo(catergoryFolder).Name;

                    // Retrieve all image files
                    foreach (var file in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"\assets\Inventory\" + catergoryName))
                    {
                        // Get file infomration.
                        var fi = new FileInfo(file);
                        string[] filenameData = fi.Name.Split(',');

                        // Catch desktop.ini from throwing errors.
                        if (filenameData[0] == "desktop.ini") continue;

                        // Get all matches.
                        if (filenameData[0].ToLower().Contains(textBox1.Text.ToLower()))
                        {
                            //Add images to Imagelist
                            ImagelistSearch.Images.Add(ImageFast.FromFile(file));
                            ImagelistLargeSearch.Images.Add(ImageFast.FromFile(file));

                            listView16.LargeImageList = ImagelistSearch;

                            // Save filename information.
                            listView16.Items.Add(new ListViewItem { ImageIndex = countSearch, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                            countSearch++; // Add one to index count.
                        }
                    }
                }
            }

            //set the amall and large ImageList properties of listview
            listView16.LargeImageList = ImagelistLargeSearch;
            listView16.View = View.LargeIcon;
        }

        // Search for item via enter support.
        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Clear previous results.
                listView16.Clear();
                listView16.Items.Clear();
                listView16.Refresh();

                // Clear previous image data.
                ImagelistSearch.Images.Clear();
                ImagelistLargeSearch.Images.Clear();

                // Tab over to search.
                tabControl1.SelectedTab = tabPage16;

                // Set Image Size
                ImagelistLargeSearch.ImageSize = new Size(50, 60);
                ImagelistSearch.ImageSize = new Size(50, 60);

                // Define seperate counts.

                int countSearch = 0;
                // Make sure assets exist.
                if (Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\assets\Inventory\"))
                {
                    // Get each folder in inventory.
                    foreach (var catergoryFolder in Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + @"\assets\Inventory\"))
                    {
                        // Get current folder name.
                        var catergoryName = new DirectoryInfo(catergoryFolder).Name;

                        // Retrieve all image files
                        foreach (var file in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"\assets\Inventory\" + catergoryName))
                        {
                            // Get file infomration.
                            var fi = new FileInfo(file);
                            string[] filenameData = fi.Name.Split(',');

                            // Catch desktop.ini from throwing errors.
                            if (filenameData[0] == "desktop.ini") continue;

                            // Get all matches.
                            if (filenameData[0].ToLower().Contains(textBox1.Text.ToLower()))
                            {
                                //Add images to Imagelist
                                ImagelistSearch.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeSearch.Images.Add(ImageFast.FromFile(file));

                                listView16.LargeImageList = ImagelistSearch;

                                // Save filename information.
                                listView16.Items.Add(new ListViewItem { ImageIndex = countSearch, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countSearch++; // Add one to index count.
                            }
                        }
                    }
                }

                //set the amall and large ImageList properties of listview
                listView16.LargeImageList = ImagelistLargeSearch;
                listView16.View = View.LargeIcon;
            }
        }

        // Launch the food cookbook.
        private void Button6_Click(object sender, EventArgs e)
        {
            // Spawn food cookbook window.
            FoodCookbook frm4 = new FoodCookbook();
            DialogResult dr = frm4.ShowDialog(this);

            // Get returned item from picker.
            int itemType = frm4.GetItemTypeFromList();
            int itemAmount = frm4.GetItemAmountFromList();
            int itemVariation = frm4.GetItemVeriationFromList() == 0 ? 0 : (frm4.GetItemVeriationFromList()); // If variation is not zero, add offset.
            // int itemSkillset = frm4.GetItemSkillsetFromList(); // Not implimented yet;
            bool wasAborted = frm4.GetUserCancledTask();
            // bool itemOverwrite = frm3.GetSelectedOverwriteTask();
            frm4.Close();

            // Check if user closed the form
            if (wasAborted) { return; };

            // Set the values from returning form.
            selectedItemType = itemType;
            selectedItemAmount = itemAmount;
            selectedItemVariation = itemVariation;
            // selectedItemSkillset = itemSkillset; // Not implimented yet;
            this.Close();
        }
        #endregion

        #region Selection Indexes

        // Get Value From Clicked Item
        private void ListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listView1.SelectedItems.Count; i++)
            {
                string[] PostNumber = listView1.Items[listView1.SelectedIndices[i]].Tag.ToString().Split(',');
                selectedItemType = int.Parse(PostNumber[0]);
                selectedItemVariation = int.Parse(PostNumber[1]);
                // selectedItemSkillset = int.Parse(PostNumber[2]); // Not implimented yet;
                this.Close();
            }
        }

        // Get the amount to add to the game.
        private void ListView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Left is add one.
            {
                selectedItemAmount = 1;
            }
            else if (e.Button == MouseButtons.Right) // Right is custom.
            {
                selectedItemAmount = (int)numericUpDown1.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listView2.SelectedItems.Count; i++)
            {
                string[] PostNumber = listView2.Items[listView2.SelectedIndices[i]].Tag.ToString().Split(',');
                selectedItemType = int.Parse(PostNumber[0]);
                selectedItemVariation = int.Parse(PostNumber[1]);
                // selectedItemSkillset = int.Parse(PostNumber[2]); // Not implimented yet;
                this.Close();
            }
        }

        // Get the amount to add to the game.
        private void ListView2MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Left is add one.
            {
                selectedItemAmount = 1;
            }
            else if (e.Button == MouseButtons.Right) // Right is custom.
            {
                selectedItemAmount = (int)numericUpDown1.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView3_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listView3.SelectedItems.Count; i++)
            {
                string[] PostNumber = listView3.Items[listView3.SelectedIndices[i]].Tag.ToString().Split(',');
                selectedItemType = int.Parse(PostNumber[0]);
                selectedItemVariation = int.Parse(PostNumber[1]);
                // selectedItemSkillset = int.Parse(PostNumber[2]); // Not implimented yet;
                this.Close();
            }
        }

        // Get the amount to add to the game.
        private void ListView3_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Left is add one.
            {
                selectedItemAmount = 1;
            }
            else if (e.Button == MouseButtons.Right) // Right is custom.
            {
                selectedItemAmount = (int)numericUpDown1.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView4_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listView4.SelectedItems.Count; i++)
            {
                string[] PostNumber = listView4.Items[listView4.SelectedIndices[i]].Tag.ToString().Split(',');
                selectedItemType = int.Parse(PostNumber[0]);
                selectedItemVariation = int.Parse(PostNumber[1]);
                // selectedItemSkillset = int.Parse(PostNumber[2]); // Not implimented yet;
                this.Close();
            }
        }

        // Get the amount to add to the game.
        private void ListView4_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Left is add one.
            {
                selectedItemAmount = 1;
            }
            else if (e.Button == MouseButtons.Right) // Right is custom.
            {
                selectedItemAmount = (int)numericUpDown1.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView5_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listView5.SelectedItems.Count; i++)
            {
                string[] PostNumber = listView5.Items[listView5.SelectedIndices[i]].Tag.ToString().Split(',');
                selectedItemType = int.Parse(PostNumber[0]);
                selectedItemVariation = int.Parse(PostNumber[1]);
                // selectedItemSkillset = int.Parse(PostNumber[2]); // Not implimented yet;
                this.Close();
            }
        }

        // Get the amount to add to the game.
        private void ListView5_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Left is add one.
            {
                selectedItemAmount = 1;
            }
            else if (e.Button == MouseButtons.Right) // Right is custom.
            {
                selectedItemAmount = (int)numericUpDown1.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView6_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listView6.SelectedItems.Count; i++)
            {
                string[] PostNumber = listView6.Items[listView6.SelectedIndices[i]].Tag.ToString().Split(',');
                selectedItemType = int.Parse(PostNumber[0]);
                selectedItemVariation = int.Parse(PostNumber[1]);
                // selectedItemSkillset = int.Parse(PostNumber[2]); // Not implimented yet;
                this.Close();
            }
        }

        // Get the amount to add to the game.
        private void ListView6_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Left is add one.
            {
                selectedItemAmount = 1;
            }
            else if (e.Button == MouseButtons.Right) // Right is custom.
            {
                selectedItemAmount = (int)numericUpDown1.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView7_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listView7.SelectedItems.Count; i++)
            {
                string[] PostNumber = listView7.Items[listView7.SelectedIndices[i]].Tag.ToString().Split(',');
                selectedItemType = int.Parse(PostNumber[0]);
                selectedItemVariation = int.Parse(PostNumber[1]);
                // selectedItemSkillset = int.Parse(PostNumber[2]); // Not implimented yet;
                this.Close();
            }
        }

        // Get the amount to add to the game.
        private void ListView7_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Left is add one.
            {
                selectedItemAmount = 1;
            }
            else if (e.Button == MouseButtons.Right) // Right is custom.
            {
                selectedItemAmount = (int)numericUpDown1.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView8_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listView8.SelectedItems.Count; i++)
            {
                string[] PostNumber = listView8.Items[listView8.SelectedIndices[i]].Tag.ToString().Split(',');
                selectedItemType = int.Parse(PostNumber[0]);
                selectedItemVariation = int.Parse(PostNumber[1]);
                // selectedItemSkillset = int.Parse(PostNumber[2]); // Not implimented yet;
                this.Close();
            }
        }

        // Get the amount to add to the game.
        private void ListView8_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Left is add one.
            {
                selectedItemAmount = 1;
            }
            else if (e.Button == MouseButtons.Right) // Right is custom.
            {
                selectedItemAmount = (int)numericUpDown1.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView9_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listView9.SelectedItems.Count; i++)
            {
                string[] PostNumber = listView9.Items[listView9.SelectedIndices[i]].Tag.ToString().Split(',');
                selectedItemType = int.Parse(PostNumber[0]);
                selectedItemVariation = int.Parse(PostNumber[1]);
                // selectedItemSkillset = int.Parse(PostNumber[2]); // Not implimented yet;
                this.Close();
            }
        }

        // Get the amount to add to the game.
        private void ListView9_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Left is add one.
            {
                selectedItemAmount = 1;
            }
            else if (e.Button == MouseButtons.Right) // Right is custom.
            {
                selectedItemAmount = (int)numericUpDown1.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView10_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listView10.SelectedItems.Count; i++)
            {
                string[] PostNumber = listView10.Items[listView10.SelectedIndices[i]].Tag.ToString().Split(',');
                selectedItemType = int.Parse(PostNumber[0]);
                selectedItemVariation = int.Parse(PostNumber[1]);
                // selectedItemSkillset = int.Parse(PostNumber[2]); // Not implimented yet;
                this.Close();
            }
        }

        // Get the amount to add to the game.
        private void ListView10_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Left is add one.
            {
                selectedItemAmount = 1;
            }
            else if (e.Button == MouseButtons.Right) // Right is custom.
            {
                selectedItemAmount = (int)numericUpDown1.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView11_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listView11.SelectedItems.Count; i++)
            {
                string[] PostNumber = listView11.Items[listView11.SelectedIndices[i]].Tag.ToString().Split(',');
                selectedItemType = int.Parse(PostNumber[0]);
                selectedItemVariation = int.Parse(PostNumber[1]);
                // selectedItemSkillset = int.Parse(PostNumber[2]); // Not implimented yet;
                this.Close();
            }
        }

        // Get the amount to add to the game.
        private void ListView11_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Left is add one.
            {
                selectedItemAmount = 1;
            }
            else if (e.Button == MouseButtons.Right) // Right is custom.
            {
                selectedItemAmount = (int)numericUpDown1.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView12_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listView12.SelectedItems.Count; i++)
            {
                string[] PostNumber = listView12.Items[listView12.SelectedIndices[i]].Tag.ToString().Split(',');
                selectedItemType = int.Parse(PostNumber[0]);
                selectedItemVariation = int.Parse(PostNumber[1]);
                // selectedItemSkillset = int.Parse(PostNumber[2]); // Not implimented yet;
                this.Close();
            }
        }

        // Get the amount to add to the game.
        private void ListView12_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Left is add one.
            {
                selectedItemAmount = 1;
            }
            else if (e.Button == MouseButtons.Right) // Right is custom.
            {
                selectedItemAmount = (int)numericUpDown1.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView13_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listView13.SelectedItems.Count; i++)
            {
                string[] PostNumber = listView13.Items[listView13.SelectedIndices[i]].Tag.ToString().Split(',');
                selectedItemType = int.Parse(PostNumber[0]);
                selectedItemVariation = int.Parse(PostNumber[1]);
                // selectedItemSkillset = int.Parse(PostNumber[2]); // Not implimented yet;
                this.Close();
            }
        }

        // Get the amount to add to the game.
        private void ListView13_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Left is add one.
            {
                selectedItemAmount = 1;
            }
            else if (e.Button == MouseButtons.Right) // Right is custom.
            {
                selectedItemAmount = (int)numericUpDown1.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView14_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listView14.SelectedItems.Count; i++)
            {
                string[] PostNumber = listView14.Items[listView14.SelectedIndices[i]].Tag.ToString().Split(',');
                selectedItemType = int.Parse(PostNumber[0]);
                selectedItemVariation = int.Parse(PostNumber[1]);
                // selectedItemSkillset = int.Parse(PostNumber[2]); // Not implimented yet;
                this.Close();
            }
        }

        // Get the amount to add to the game.
        private void ListView14_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Left is add one.
            {
                selectedItemAmount = 1;
            }
            else if (e.Button == MouseButtons.Right) // Right is custom.
            {
                selectedItemAmount = (int)numericUpDown1.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView15_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listView15.SelectedItems.Count; i++)
            {
                string[] PostNumber = listView15.Items[listView15.SelectedIndices[i]].Tag.ToString().Split(',');
                selectedItemType = int.Parse(PostNumber[0]);
                selectedItemVariation = int.Parse(PostNumber[1]);
                // selectedItemSkillset = int.Parse(PostNumber[2]); // Not implimented yet;
                this.Close();
            }
        }

        // Get the amount to add to the game.
        private void ListView15_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Left is add one.
            {
                selectedItemAmount = 1;
            }
            else if (e.Button == MouseButtons.Right) // Right is custom.
            {
                selectedItemAmount = (int)numericUpDown1.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView16_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listView16.SelectedItems.Count; i++)
            {
                string[] PostNumber = listView16.Items[listView16.SelectedIndices[i]].Tag.ToString().Split(',');
                selectedItemType = int.Parse(PostNumber[0]);
                selectedItemVariation = int.Parse(PostNumber[1]);
                // selectedItemSkillset = int.Parse(PostNumber[2]); // Not implimented yet;
                this.Close();
            }
        }

        // Get the amount to add to the game.
        private void ListView16_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Left is add one.
            {
                selectedItemAmount = 1;
            }
            else if (e.Button == MouseButtons.Right) // Right is custom.
            {
                selectedItemAmount = (int)numericUpDown1.Value;
            }
        }
        #endregion
    }
}
