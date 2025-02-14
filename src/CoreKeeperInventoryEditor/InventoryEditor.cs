using System.Collections.Generic;
using System.Windows.Forms;
using CoreKeepersWorkshop;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.IO;
using System;

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
        public IEnumerable<string> FolderNames = Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"assets\Inventory\") ? Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory + @"assets\Inventory\") : new String[] { "Null", "Null", "Null", "Null", "Null", "Null", "Null", "Null", "Null", "Null", "Null", "Null", "Null", "Null", "Null", "Null", "Null" }; // Ensure directory exists. Fix: v1.2.9.

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
        readonly ImageList ImagelistUnobtainable = new ImageList();
        readonly ImageList ImagelistLargeUnobtainable = new ImageList();
        readonly ImageList ImagelistUnused = new ImageList();
        readonly ImageList ImagelistLargeUnused = new ImageList();
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
                CoreKeepersWorkshop.Properties.Settings.Default.ItemAmount = (int)CustomAmount_NumericUpDown.Value;
                CoreKeepersWorkshop.Properties.Settings.Default.ItemID = (int)CustomID_NumericUpDown.Value;
                CoreKeepersWorkshop.Properties.Settings.Default.ItemVariation = (int)ItemVariant_NumericUpDown.Value;
                CoreKeepersWorkshop.Properties.Settings.Default.ItemSkillset = (int)SkillType_NumericUpDown.Value;
                CoreKeepersWorkshop.Properties.Settings.Default.InventoryEditorLocation = this.Location;

                // Ensure current tab is not search, if so, reset.
                if (Main_TabControl.SelectedTab == Search)
                {
                    // Set value to tools.
                    CoreKeepersWorkshop.Properties.Settings.Default.CurrentItemTab = "Tab1_TabPage";
                }
                else
                {
                    // Save current tab.
                    CoreKeepersWorkshop.Properties.Settings.Default.CurrentItemTab = Main_TabControl.SelectedTab.Name;
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
            toolTip.SetToolTip(CustomAmount_NumericUpDown, "Enter the amount of items to add.");
            toolTip.SetToolTip(CustomID_NumericUpDown, "Enter a custom ID. Either press enter when done or use the button.");
            toolTip.SetToolTip(ItemVariant_NumericUpDown, "Enter a custom variant ID. Either press enter when done or use the button.");
            toolTip.SetToolTip(SkillType_NumericUpDown, "Enter a custom skillset ID. Either press enter when done or use the button.");

            toolTip.SetToolTip(CustomAmount_Button, "Remove the item from this inventory slot.");
            toolTip.SetToolTip(CustomID_Button, "Spawn in custom item amount + ID.");
            toolTip.SetToolTip(Search_Button, "Start the search for a desired item.");
            toolTip.SetToolTip(ItemVariant_Button, "Spawn in custom item with variation.");
            toolTip.SetToolTip(OpenCookedFoodList_Button, "Open the food cookbook to easily search for food items.");
            toolTip.SetToolTip(SkillTypeInfo_Button, "Launch a guide on how to find skillset IDs.");

            toolTip.SetToolTip(Search_TextBox, "Enter a name to search for.");

            #endregion

            #region Do Loading Events

            // Set tab text based on the folder names.
            Main_TabControl.TabPages[0].Text = new DirectoryInfo(FolderNames.ElementAt(0)).Name;
            Main_TabControl.TabPages[1].Text = new DirectoryInfo(FolderNames.ElementAt(1)).Name;
            Main_TabControl.TabPages[2].Text = new DirectoryInfo(FolderNames.ElementAt(2)).Name;
            Main_TabControl.TabPages[3].Text = new DirectoryInfo(FolderNames.ElementAt(3)).Name;
            Main_TabControl.TabPages[4].Text = new DirectoryInfo(FolderNames.ElementAt(4)).Name;
            Main_TabControl.TabPages[5].Text = new DirectoryInfo(FolderNames.ElementAt(5)).Name;
            Main_TabControl.TabPages[6].Text = new DirectoryInfo(FolderNames.ElementAt(6)).Name;
            Main_TabControl.TabPages[7].Text = new DirectoryInfo(FolderNames.ElementAt(7)).Name;
            Main_TabControl.TabPages[8].Text = new DirectoryInfo(FolderNames.ElementAt(8)).Name;
            Main_TabControl.TabPages[9].Text = new DirectoryInfo(FolderNames.ElementAt(9)).Name;
            Main_TabControl.TabPages[10].Text = new DirectoryInfo(FolderNames.ElementAt(10)).Name;
            Main_TabControl.TabPages[11].Text = new DirectoryInfo(FolderNames.ElementAt(11)).Name;
            Main_TabControl.TabPages[12].Text = new DirectoryInfo(FolderNames.ElementAt(12)).Name;
            Main_TabControl.TabPages[13].Text = new DirectoryInfo(FolderNames.ElementAt(13)).Name;
            Main_TabControl.TabPages[14].Text = new DirectoryInfo(FolderNames.ElementAt(14)).Name;
            Main_TabControl.TabPages[15].Text = new DirectoryInfo(FolderNames.ElementAt(15)).Name;
            Main_TabControl.TabPages[16].Text = new DirectoryInfo(FolderNames.ElementAt(16)).Name;

            // Ensure the skillset is more then -1.
            if (CoreKeepersWorkshop.Properties.Settings.Default.ItemSkillset < 0)
            {
                CoreKeepersWorkshop.Properties.Settings.Default.ItemSkillset = 0;
            }

            // Load some settings.
            CustomAmount_NumericUpDown.Value = CoreKeepersWorkshop.Properties.Settings.Default.ItemAmount;
            CustomID_NumericUpDown.Value = CoreKeepersWorkshop.Properties.Settings.Default.ItemID;
            ItemVariant_NumericUpDown.Value = CoreKeepersWorkshop.Properties.Settings.Default.ItemVariation;
            SkillType_NumericUpDown.Value = CoreKeepersWorkshop.Properties.Settings.Default.ItemSkillset;
            Main_TabControl.SelectedTab = Main_TabControl.TabPages[CoreKeepersWorkshop.Properties.Settings.Default.CurrentItemTab];

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
            ImagelistLargeUnobtainable.ImageSize = new Size(50, 60);
            ImagelistLargeUnused.ImageSize = new Size(50, 60);

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
            int countUnobtainable = 0;
            int countUnused = 0;

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

                                Tab1_ListView.LargeImageList = ImagelistTools;

                                // Save filename information.
                                Tab1_ListView.Items.Add(new ListViewItem { ImageIndex = countTools, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countTools++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(1)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistPlaceableItems.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargePlaceableItems.Images.Add(ImageFast.FromFile(file));

                                Tab2_ListView.LargeImageList = ImagelistPlaceableItems;

                                // Save filename information.
                                Tab2_ListView.Items.Add(new ListViewItem { ImageIndex = countPlaceableItems, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countPlaceableItems++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(2)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistNature.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeNature.Images.Add(ImageFast.FromFile(file));

                                Tab3_ListView.LargeImageList = ImagelistNature;

                                // Save filename information.
                                Tab3_ListView.Items.Add(new ListViewItem { ImageIndex = countNature, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countNature++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(3)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistMaterials.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeMaterials.Images.Add(ImageFast.FromFile(file));

                                Tab4_ListView.LargeImageList = ImagelistMaterials;

                                // Save filename information.
                                Tab4_ListView.Items.Add(new ListViewItem { ImageIndex = countMaterials, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countMaterials++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(4)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistSpecial.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeSpecial.Images.Add(ImageFast.FromFile(file));

                                Tab5_ListView.LargeImageList = ImagelistSpecial;

                                // Save filename information.
                                Tab5_ListView.Items.Add(new ListViewItem { ImageIndex = countSpecial, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countSpecial++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(5)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistMobItems.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeMobItems.Images.Add(ImageFast.FromFile(file));

                                Tab6_ListView.LargeImageList = ImagelistMobItems;

                                // Save filename information.
                                Tab6_ListView.Items.Add(new ListViewItem { ImageIndex = countMobItems, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countMobItems++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(6)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistBaseBuilding.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeBaseBuilding.Images.Add(ImageFast.FromFile(file));

                                Tab7_ListView.LargeImageList = ImagelistBaseBuilding;

                                // Save filename information.
                                Tab7_ListView.Items.Add(new ListViewItem { ImageIndex = countBaseBuilding, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countBaseBuilding++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(7)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistTreasures.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeTreasures.Images.Add(ImageFast.FromFile(file));

                                Tab8_ListView.LargeImageList = ImagelistTreasures;

                                // Save filename information.
                                Tab8_ListView.Items.Add(new ListViewItem { ImageIndex = countTreasures, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countTreasures++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(8)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistWiring.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeWiring.Images.Add(ImageFast.FromFile(file));

                                Tab9_ListView.LargeImageList = ImagelistWiring;

                                // Save filename information.
                                Tab9_ListView.Items.Add(new ListViewItem { ImageIndex = countWiring, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countWiring++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(9)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistPlants.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargePlants.Images.Add(ImageFast.FromFile(file));

                                Tab10_ListView.LargeImageList = ImagelistPlants;

                                // Save filename information.
                                Tab10_ListView.Items.Add(new ListViewItem { ImageIndex = countPlants, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countPlants++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(10)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistArmors.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeArmors.Images.Add(ImageFast.FromFile(file));

                                Tab11_ListView.LargeImageList = ImagelistArmors;

                                // Save filename information.
                                Tab11_ListView.Items.Add(new ListViewItem { ImageIndex = countArmors, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countArmors++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(11)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistAccessories.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeAccessories.Images.Add(ImageFast.FromFile(file));

                                Tab12_ListView.LargeImageList = ImagelistAccessories;

                                // Save filename information.
                                Tab12_ListView.Items.Add(new ListViewItem { ImageIndex = countAccessories, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countAccessories++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(12)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistWeapons.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeWeapons.Images.Add(ImageFast.FromFile(file));

                                Tab13_ListView.LargeImageList = ImagelistWeapons;

                                // Save filename information.
                                Tab13_ListView.Items.Add(new ListViewItem { ImageIndex = countWeapons, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countWeapons++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(13)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistConsumables.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeConsumables.Images.Add(ImageFast.FromFile(file));

                                Tab14_ListView.LargeImageList = ImagelistConsumables;

                                // Save filename information.
                                Tab14_ListView.Items.Add(new ListViewItem { ImageIndex = countConsumables, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countConsumables++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(14)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistSeasonal.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeSeasonal.Images.Add(ImageFast.FromFile(file));

                                Tab15_ListView.LargeImageList = ImagelistSeasonal;

                                // Save filename information.
                                Tab15_ListView.Items.Add(new ListViewItem { ImageIndex = countSeasonal, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countSeasonal++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(15)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistUnobtainable.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeUnobtainable.Images.Add(ImageFast.FromFile(file));

                                Tab16_ListView.LargeImageList = ImagelistUnobtainable;

                                // Save filename information.
                                Tab16_ListView.Items.Add(new ListViewItem { ImageIndex = countUnobtainable, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countUnobtainable++; // Add one to index count.
                            }
                            else if (catergoryName == new DirectoryInfo(FolderNames.ElementAt(16)).Name)
                            {
                                //Add images to Imagelist
                                ImagelistUnused.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeUnused.Images.Add(ImageFast.FromFile(file));

                                Tab17_ListView.LargeImageList = ImagelistUnused;

                                // Save filename information.
                                Tab17_ListView.Items.Add(new ListViewItem { ImageIndex = countUnused, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countUnused++; // Add one to index count.
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
                Tab1_ListView.LargeImageList = ImagelistLargeTools;
                Tab1_ListView.View = View.LargeIcon;
                Tab2_ListView.LargeImageList = ImagelistPlaceableItems;
                Tab2_ListView.View = View.LargeIcon;
                Tab3_ListView.LargeImageList = ImagelistLargeNature;
                Tab3_ListView.View = View.LargeIcon;
                Tab4_ListView.LargeImageList = ImagelistLargeMaterials;
                Tab4_ListView.View = View.LargeIcon;
                Tab5_ListView.LargeImageList = ImagelistLargeSpecial;
                Tab5_ListView.View = View.LargeIcon;
                Tab6_ListView.LargeImageList = ImagelistLargeMobItems;
                Tab6_ListView.View = View.LargeIcon;
                Tab7_ListView.LargeImageList = ImagelistLargeBaseBuilding;
                Tab7_ListView.View = View.LargeIcon;
                Tab8_ListView.LargeImageList = ImagelistLargeTreasures;
                Tab8_ListView.View = View.LargeIcon;
                Tab9_ListView.LargeImageList = ImagelistLargeWiring;
                Tab9_ListView.View = View.LargeIcon;
                Tab10_ListView.LargeImageList = ImagelistLargePlants;
                Tab10_ListView.View = View.LargeIcon;
                Tab11_ListView.LargeImageList = ImagelistLargeArmors;
                Tab11_ListView.View = View.LargeIcon;
                Tab12_ListView.LargeImageList = ImagelistLargeAccessories;
                Tab12_ListView.View = View.LargeIcon;
                Tab13_ListView.LargeImageList = ImagelistLargeWeapons;
                Tab13_ListView.View = View.LargeIcon;
                Tab14_ListView.LargeImageList = ImagelistLargeConsumables;
                Tab14_ListView.View = View.LargeIcon;
                Tab15_ListView.LargeImageList = ImagelistLargeSeasonal;
                Tab15_ListView.View = View.LargeIcon;
                Tab16_ListView.LargeImageList = ImagelistLargeUnobtainable;
                Tab16_ListView.View = View.LargeIcon;
                Tab17_ListView.LargeImageList = ImagelistLargeUnused;
                Tab17_ListView.View = View.LargeIcon;
            }

            #endregion
        }

        #region Main Form Controls

        // Add custom ID.
        private void Button3_Click(object sender, EventArgs e)
        {
            selectedItemType = (int)CustomID_NumericUpDown.Value;
            selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
            selectedItemVariation = (int)ItemVariant_NumericUpDown.Value;
            selectedItemSkillset = (int)SkillType_NumericUpDown.Value;
            selectedOverwrite = true;
            this.Close();
        }

        // Add custom veriation.
        private void Button5_Click(object sender, EventArgs e)
        {
            selectedItemType = (int)CustomID_NumericUpDown.Value;
            selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
            selectedItemVariation = (int)ItemVariant_NumericUpDown.Value;
            selectedItemSkillset = (int)SkillType_NumericUpDown.Value;
            selectedOverwrite = true;
            this.Close();
        }

        // Add custom skillset.
        private void Button7_Click(object sender, EventArgs e)
        {
            selectedItemType = (int)CustomID_NumericUpDown.Value;
            selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
            selectedItemVariation = (int)ItemVariant_NumericUpDown.Value;
            selectedItemSkillset = (int)SkillType_NumericUpDown.Value;
            selectedOverwrite = true;
            this.Close();
        }

        // Add custom veriation - Enter shortcut.
        private void NumericUpDown3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectedItemType = (int)CustomID_NumericUpDown.Value;
                selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
                selectedItemVariation = (int)ItemVariant_NumericUpDown.Value;
                selectedItemSkillset = (int)SkillType_NumericUpDown.Value;
                selectedOverwrite = true;
                this.Close();
            }
        }

        // Add custom ID - Enter shortcut.
        private void NumericUpDown2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectedItemType = (int)CustomID_NumericUpDown.Value;
                selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
                selectedItemVariation = (int)ItemVariant_NumericUpDown.Value;
                selectedItemSkillset = (int)SkillType_NumericUpDown.Value;
                selectedOverwrite = true;
                this.Close();
            }
        }

        // Add custom skillset - Enter shortcut.
        private void NumericUpDown4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectedItemType = (int)CustomID_NumericUpDown.Value;
                selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
                selectedItemVariation = (int)ItemVariant_NumericUpDown.Value;
                selectedItemSkillset = (int)SkillType_NumericUpDown.Value;
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
            listViewSearch.Clear();
            listViewSearch.Items.Clear();
            listViewSearch.Refresh();

            // Clear previous image data.
            ImagelistSearch.Images.Clear();
            ImagelistLargeSearch.Images.Clear();

            // Tab over to search.
            Main_TabControl.SelectedTab = Search;

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
                        if (filenameData[0].ToLower().Contains(Search_TextBox.Text.ToLower()))
                        {
                            //Add images to Imagelist
                            ImagelistSearch.Images.Add(ImageFast.FromFile(file));
                            ImagelistLargeSearch.Images.Add(ImageFast.FromFile(file));

                            listViewSearch.LargeImageList = ImagelistSearch;

                            // Save filename information.
                            listViewSearch.Items.Add(new ListViewItem { ImageIndex = countSearch, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                            countSearch++; // Add one to index count.
                        }
                    }
                }
            }

            //set the amall and large ImageList properties of listview
            listViewSearch.LargeImageList = ImagelistLargeSearch;
            listViewSearch.View = View.LargeIcon;
        }

        // Search for item via enter support.
        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Clear previous results.
                listViewSearch.Clear();
                listViewSearch.Items.Clear();
                listViewSearch.Refresh();

                // Clear previous image data.
                ImagelistSearch.Images.Clear();
                ImagelistLargeSearch.Images.Clear();

                // Tab over to search.
                Main_TabControl.SelectedTab = Search;

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
                            if (filenameData[0].ToLower().Contains(Search_TextBox.Text.ToLower()))
                            {
                                //Add images to Imagelist
                                ImagelistSearch.Images.Add(ImageFast.FromFile(file));
                                ImagelistLargeSearch.Images.Add(ImageFast.FromFile(file));

                                listViewSearch.LargeImageList = ImagelistSearch;

                                // Save filename information.
                                listViewSearch.Items.Add(new ListViewItem { ImageIndex = countSearch, Text = filenameData[0], Tag = filenameData[1] + "," + filenameData[2].Split('.')[0] }); // Using object initializer to add the text
                                countSearch++; // Add one to index count.
                            }
                        }
                    }
                }

                //set the amall and large ImageList properties of listview
                listViewSearch.LargeImageList = ImagelistLargeSearch;
                listViewSearch.View = View.LargeIcon;
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
            for (int i = 0; i < Tab1_ListView.SelectedItems.Count; i++)
            {
                string[] PostNumber = Tab1_ListView.Items[Tab1_ListView.SelectedIndices[i]].Tag.ToString().Split(',');
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
                selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView2_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < Tab2_ListView.SelectedItems.Count; i++)
            {
                string[] PostNumber = Tab2_ListView.Items[Tab2_ListView.SelectedIndices[i]].Tag.ToString().Split(',');
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
                selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView3_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < Tab3_ListView.SelectedItems.Count; i++)
            {
                string[] PostNumber = Tab3_ListView.Items[Tab3_ListView.SelectedIndices[i]].Tag.ToString().Split(',');
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
                selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView4_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < Tab4_ListView.SelectedItems.Count; i++)
            {
                string[] PostNumber = Tab4_ListView.Items[Tab4_ListView.SelectedIndices[i]].Tag.ToString().Split(',');
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
                selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView5_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < Tab5_ListView.SelectedItems.Count; i++)
            {
                string[] PostNumber = Tab5_ListView.Items[Tab5_ListView.SelectedIndices[i]].Tag.ToString().Split(',');
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
                selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView6_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < Tab6_ListView.SelectedItems.Count; i++)
            {
                string[] PostNumber = Tab6_ListView.Items[Tab6_ListView.SelectedIndices[i]].Tag.ToString().Split(',');
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
                selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView7_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < Tab7_ListView.SelectedItems.Count; i++)
            {
                string[] PostNumber = Tab7_ListView.Items[Tab7_ListView.SelectedIndices[i]].Tag.ToString().Split(',');
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
                selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView8_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < Tab8_ListView.SelectedItems.Count; i++)
            {
                string[] PostNumber = Tab8_ListView.Items[Tab8_ListView.SelectedIndices[i]].Tag.ToString().Split(',');
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
                selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView9_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < Tab9_ListView.SelectedItems.Count; i++)
            {
                string[] PostNumber = Tab9_ListView.Items[Tab9_ListView.SelectedIndices[i]].Tag.ToString().Split(',');
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
                selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView10_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < Tab10_ListView.SelectedItems.Count; i++)
            {
                string[] PostNumber = Tab10_ListView.Items[Tab10_ListView.SelectedIndices[i]].Tag.ToString().Split(',');
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
                selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView11_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < Tab11_ListView.SelectedItems.Count; i++)
            {
                string[] PostNumber = Tab11_ListView.Items[Tab11_ListView.SelectedIndices[i]].Tag.ToString().Split(',');
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
                selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView12_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < Tab12_ListView.SelectedItems.Count; i++)
            {
                string[] PostNumber = Tab12_ListView.Items[Tab12_ListView.SelectedIndices[i]].Tag.ToString().Split(',');
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
                selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView13_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < Tab13_ListView.SelectedItems.Count; i++)
            {
                string[] PostNumber = Tab13_ListView.Items[Tab13_ListView.SelectedIndices[i]].Tag.ToString().Split(',');
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
                selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView14_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < Tab14_ListView.SelectedItems.Count; i++)
            {
                string[] PostNumber = Tab14_ListView.Items[Tab14_ListView.SelectedIndices[i]].Tag.ToString().Split(',');
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
                selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView15_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < Tab15_ListView.SelectedItems.Count; i++)
            {
                string[] PostNumber = Tab15_ListView.Items[Tab15_ListView.SelectedIndices[i]].Tag.ToString().Split(',');
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
                selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView16_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < Tab16_ListView.SelectedItems.Count; i++)
            {
                string[] PostNumber = Tab16_ListView.Items[Tab16_ListView.SelectedIndices[i]].Tag.ToString().Split(',');
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
                selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListView17_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < Tab17_ListView.SelectedItems.Count; i++)
            {
                string[] PostNumber = Tab17_ListView.Items[Tab17_ListView.SelectedIndices[i]].Tag.ToString().Split(',');
                selectedItemType = int.Parse(PostNumber[0]);
                selectedItemVariation = int.Parse(PostNumber[1]);
                // selectedItemSkillset = int.Parse(PostNumber[2]); // Not implimented yet;
                this.Close();
            }
        }

        // Get the amount to add to the game.
        private void ListView17_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Left is add one.
            {
                selectedItemAmount = 1;
            }
            else if (e.Button == MouseButtons.Right) // Right is custom.
            {
                selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
            }
        }

        // Get Value From Clicked Item
        private void ListViewSearch_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < listViewSearch.SelectedItems.Count; i++)
            {
                string[] PostNumber = listViewSearch.Items[listViewSearch.SelectedIndices[i]].Tag.ToString().Split(',');
                selectedItemType = int.Parse(PostNumber[0]);
                selectedItemVariation = int.Parse(PostNumber[1]);
                // selectedItemSkillset = int.Parse(PostNumber[2]); // Not implimented yet;
                this.Close();
            }
        }

        // Get the amount to add to the game.
        private void ListViewSearch_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) // Left is add one.
            {
                selectedItemAmount = 1;
            }
            else if (e.Button == MouseButtons.Right) // Right is custom.
            {
                selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
            }
        }
        #endregion
    }
}
