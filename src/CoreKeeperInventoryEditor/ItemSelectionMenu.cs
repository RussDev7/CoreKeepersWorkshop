using System.Collections.Generic;
using System.Windows.Forms;
using CoreKeepersWorkshop;
using System.Drawing;
using System.Linq;
using System.IO;
using System;

namespace CoreKeeperInventoryEditor
{
    public partial class ItemSelectionMenu : Form
    {
        // Form closing saving.
        int selectedItemType = 0;
        int selectedItemAmount = 0;
        int selectedItemVariation = 0;
        int selectedItemSkillset = 0;
        bool selectedOverwrite = false;
        bool userCanceldTask = false;

        // Define texture data.
        private static string InventoryDir => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "Inventory");
        public IEnumerable<string> ImageFiles =>
            Directory.Exists(InventoryDir)
                ? Directory.EnumerateFiles(InventoryDir, "*.png", SearchOption.AllDirectories)
                    .DefaultIfEmpty(string.Empty)
                : new[] { string.Empty };

        // Define folder names.
        public static IEnumerable<string> FolderNames =>
            Directory.Exists(InventoryDir)
                ? Directory.EnumerateDirectories(InventoryDir)
                : Enumerable.Repeat("Null", 17);

        // Define imagelist.
        private ListView[]  TabListViews;
        private ImageList[] Imagelists;
        private ImageList[] ImagelistsLarge;
        readonly ImageList ImagelistTools               = new ImageList();
        readonly ImageList ImagelistLargeTools          = new ImageList();
        readonly ImageList ImagelistPlaceableItems      = new ImageList();
        readonly ImageList ImagelistLargePlaceableItems = new ImageList();
        readonly ImageList ImagelistNature              = new ImageList();
        readonly ImageList ImagelistLargeNature         = new ImageList();
        readonly ImageList ImagelistMaterials           = new ImageList();
        readonly ImageList ImagelistLargeMaterials      = new ImageList();
        readonly ImageList ImagelistSpecial             = new ImageList();
        readonly ImageList ImagelistLargeSpecial        = new ImageList();
        readonly ImageList ImagelistMobItems            = new ImageList();
        readonly ImageList ImagelistLargeMobItems       = new ImageList();
        readonly ImageList ImagelistBaseBuilding        = new ImageList();
        readonly ImageList ImagelistLargeBaseBuilding   = new ImageList();
        readonly ImageList ImagelistTreasures           = new ImageList();
        readonly ImageList ImagelistLargeTreasures      = new ImageList();
        readonly ImageList ImagelistWiring              = new ImageList();
        readonly ImageList ImagelistLargeWiring         = new ImageList();
        readonly ImageList ImagelistPlants              = new ImageList();
        readonly ImageList ImagelistLargePlants         = new ImageList();
        readonly ImageList ImagelistArmors              = new ImageList();
        readonly ImageList ImagelistLargeArmors         = new ImageList();
        readonly ImageList ImagelistAccessories         = new ImageList();
        readonly ImageList ImagelistLargeAccessories    = new ImageList();
        readonly ImageList ImagelistWeapons             = new ImageList();
        readonly ImageList ImagelistLargeWeapons        = new ImageList();
        readonly ImageList ImagelistConsumables         = new ImageList();
        readonly ImageList ImagelistLargeConsumables    = new ImageList();
        readonly ImageList ImagelistSeasonal            = new ImageList();
        readonly ImageList ImagelistLargeSeasonal       = new ImageList();
        readonly ImageList ImagelistUnobtainable        = new ImageList();
        readonly ImageList ImagelistLargeUnobtainable   = new ImageList();
        readonly ImageList ImagelistUnused              = new ImageList();
        readonly ImageList ImagelistLargeUnused         = new ImageList();
        readonly ImageList ImagelistSearch              = new ImageList();
        readonly ImageList ImagelistLargeSearch         = new ImageList();

        // Form initialization.
        private CustomFormStyler _formThemeStyler;
        private readonly int     _tabControlWidthOffset  = 8; // BorderlessTabControl custom parent offsets.
        private readonly int     _tabControlHeightOffset = 0;
        public ItemSelectionMenu()
        {
            InitializeComponent();
            Size = new Size(Width - _tabControlWidthOffset, Height - _tabControlHeightOffset); // Offset the form by the pixel amount 'WndProc' stole.
            Load += (_, __) => _formThemeStyler = this.ApplyTheme();                           // Load the forms theme.
        }

        #region Form Controls

        // Do form closing events.
        private void InventoryEditor_FormClosing(object sender, FormClosingEventArgs e)
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

        // Launch about skillset message.
        private void SkillTypeInfo_Button_Click(object sender, EventArgs e)
        {
            MessageBox.Show("How to find my pet's skillset ID?\r\n    - Each pet only has 4 skillsets and are typically +4/-4 the current pets skillset. For example, if your pet has an assigned skillset of 150 then the 4 skillset IDs will be within the 146-154 range. For fresh pets first name the pet and it will be assigned a skillset id.\r\n\r\nWhy is it like this?\r\n    - The game generates progressive \"skill tables\" rather than using a static ID for a pets skills. Bad system I know, nothing I can do about it.\r\n", "How To Use --> Get Skillset ID", MessageBoxButtons.OK, MessageBoxIcon.Question);
            return;
        }
        #endregion

        #region Closing Varibles

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
        public bool GetSelectedOverwriteTask()
        {
            return selectedOverwrite;
        }
        #endregion

        // Do loading events.
        private void InventoryEditor_Load(object sender, EventArgs e)
        {
            #region Set Custom Cusror

            // Set the applications cursor.
            Cursor = new Cursor(CoreKeepersWorkshop.Properties.Resources.UICursor.GetHicon());
            #endregion

            #region Set Form Locations

            // Set the forms active location based on previous save.
            this.Location = CoreKeepersWorkshop.Properties.Settings.Default.InventoryEditorLocation;
            #endregion

            #region Set Form Theme

            // Change the tab-control color settings based on the theme.
            if (CoreKeepersWorkshop.Properties.Settings.Default.UITheme == ThemeMode.Dark)
                Main_TabControl.RecolorAllTabs(BorderlessTabControlExtensions.ThemeMode.Dark);
            else
                Main_TabControl.RecolorAllTabs(BorderlessTabControlExtensions.ThemeMode.Light);
            MainContentPanel1_Panel.BackColor = Color.FromArgb(12, 12, 12); // Near black.
            #endregion

            #region Set Form Opacity

            // Set form opacity based on trackbars value saved setting (1 to 100 -> 0.01 to 1.0).
            this.Opacity = CoreKeepersWorkshop.Properties.Settings.Default.FormOpacity / 100.0;
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
            toolTip.SetToolTip(CustomID_NumericUpDown,     "Enter a custom ID. Either press enter when done or use the button.");
            toolTip.SetToolTip(ItemVariant_NumericUpDown,  "Enter a custom variant ID. Either press enter when done or use the button.");
            toolTip.SetToolTip(SkillType_NumericUpDown,    "Enter a custom skillset ID. Either press enter when done or use the button.");

            toolTip.SetToolTip(CustomAmount_Button,        "Remove the item from this inventory slot.");
            toolTip.SetToolTip(CustomID_Button,            "Spawn in custom item amount + ID.");
            toolTip.SetToolTip(Search_Button,              "Start the search for a desired item.");
            toolTip.SetToolTip(ItemVariant_Button,         "Spawn in custom item with variation.");
            toolTip.SetToolTip(OpenCookedFoodList_Button,  "Open the food cookbook to easily search for food items.");
            toolTip.SetToolTip(SkillTypeInfo_Button,       "Launch a guide on how to find skillset IDs.");

            toolTip.SetToolTip(Search_TextBox,             "Enter a name to search for.");

            #endregion

            #region Setup Image Lists

            TabListViews = new ListView[] {
                Tab1_ListView, Tab2_ListView, Tab3_ListView, Tab4_ListView, Tab5_ListView, Tab6_ListView, Tab7_ListView,
                Tab8_ListView, Tab9_ListView, Tab10_ListView, Tab11_ListView, Tab12_ListView, Tab13_ListView, Tab14_ListView,
                Tab15_ListView, Tab16_ListView, Tab17_ListView
            };

            Imagelists = new ImageList[] {
                ImagelistTools, ImagelistPlaceableItems, ImagelistNature, ImagelistMaterials, ImagelistSpecial,
                ImagelistMobItems, ImagelistBaseBuilding, ImagelistTreasures, ImagelistWiring, ImagelistPlants,
                ImagelistArmors, ImagelistAccessories, ImagelistWeapons, ImagelistConsumables, ImagelistSeasonal,
                ImagelistUnobtainable, ImagelistUnused
            };

            ImagelistsLarge = new ImageList[] {
                ImagelistLargeTools, ImagelistLargePlaceableItems, ImagelistLargeNature, ImagelistLargeMaterials, ImagelistLargeSpecial,
                ImagelistLargeMobItems, ImagelistLargeBaseBuilding, ImagelistLargeTreasures, ImagelistLargeWiring, ImagelistLargePlants,
                ImagelistLargeArmors, ImagelistLargeAccessories, ImagelistLargeWeapons, ImagelistLargeConsumables, ImagelistLargeSeasonal,
                ImagelistLargeUnobtainable, ImagelistLargeUnused
            };

            #endregion

            #region Do Loading Events

            // Set tab text based on the folder names.
            for (int i = 0; i < TabListViews.Length; i++)
                Main_TabControl.TabPages[i].Text = new DirectoryInfo(FolderNames.ElementAt(i)).Name;

            // Set image sizes.
            foreach (var imgList in ImagelistsLarge)
                imgList.ImageSize = new Size(50, 60);

            // Set the small and large ImageList properties of listview.
            for (int i = 0; i < TabListViews.Length; i++)
            {
                TabListViews[i].LargeImageList = ImagelistsLarge[i];
                TabListViews[i].View = View.LargeIcon;
            }

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

            // Assign images/items.
            // Loop through all folders/categories (each tab/category of items).
            for (int i = 0; i < FolderNames.Count(); i++)
            {
                // --- Get the cached images for this category ---
                // If there are no cached images for this category, just skip (empty list).
                var cachedItems = InventoryImageCache.CategoryImages.ContainsKey(i)
                    ? InventoryImageCache.CategoryImages[i]
                    : new List<InventoryImageData>();

                // Clear any previous images in the imagelists for this category.
                Imagelists[i].Images.Clear();
                ImagelistsLarge[i].Images.Clear();
                TabListViews[i].Items.Clear();

                // --- Loop over each cached image for this category ---
                int count = 0;
                foreach (var item in cachedItems)
                {
                    // Add the image to the imagelists (small/large).
                    Imagelists[i].Images.Add(item.Image);
                    ImagelistsLarge[i].Images.Add(item.Image);

                    // Store id, variation, (optional) skillset in the Tag for use on click/select
                    string tag = item.SkillSet == null
                        ? $"{item.Id},{item.Variation}"
                        : $"{item.Id},{item.Variation},{item.SkillSet}";

                    TabListViews[i].Items.Add(new ListViewItem // Add item to the ListView with appropriate image and tag (id, variation).
                    {
                        ImageIndex = count,                    // Index of image just added.
                        Text = item.Name,                      // The display name for this item.
                        Tag = tag                              // Store ID and variation in Tag.
                    });
                    count++;
                }

                // --- Assign the large imagelist and set view mode ---
                TabListViews[i].LargeImageList = ImagelistsLarge[i];
                TabListViews[i].View = View.LargeIcon;
            }

            #endregion
        }

        #region Main Form Controls

        // Add custom ID.
        private void CustomID_Button_Click(object sender, EventArgs e)
        {
            selectedItemType = (int)CustomID_NumericUpDown.Value;
            selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
            selectedItemVariation = (int)ItemVariant_NumericUpDown.Value;
            selectedItemSkillset = (int)SkillType_NumericUpDown.Value;
            selectedOverwrite = true;
            this.Close();
        }

        // Add custom variation.
        private void ItemVariant_Button_Click(object sender, EventArgs e)
        {
            selectedItemType = (int)CustomID_NumericUpDown.Value;
            selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
            selectedItemVariation = (int)ItemVariant_NumericUpDown.Value;
            selectedItemSkillset = (int)SkillType_NumericUpDown.Value;
            selectedOverwrite = true;
            this.Close();
        }

        // Add custom skillset.
        private void SkillType_Button_Click(object sender, EventArgs e)
        {
            selectedItemType = (int)CustomID_NumericUpDown.Value;
            selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
            selectedItemVariation = (int)ItemVariant_NumericUpDown.Value;
            selectedItemSkillset = (int)SkillType_NumericUpDown.Value;
            selectedOverwrite = true;
            this.Close();
        }

        // Add custom variation - Enter shortcut.
        private void ItemVariant_NumericUpDown_KeyDown(object sender, KeyEventArgs e)
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
        private void CustomID_NumericUpDown_KeyDown(object sender, KeyEventArgs e)
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
        private void SkillType_NumericUpDown_KeyDown(object sender, KeyEventArgs e)
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
        private void RemoveItem_Button_Click(object sender, EventArgs e)
        {
            selectedItemType = 0;
            selectedItemAmount = 1;
            selectedItemVariation = 0;
            selectedItemSkillset = 0;
            this.Close();
        }

        // Search for item.
        private void Search_Button_Click(object sender, EventArgs e)
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

            // Define separate counts.
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
                        // Get file information.
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

            //set the small and large ImageList properties of listview
            listViewSearch.LargeImageList = ImagelistLargeSearch;
            listViewSearch.View = View.LargeIcon;
        }

        // Search for item via enter support.
        private void Search_TextBox_KeyDown(object sender, KeyEventArgs e)
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

                // Define separate counts.

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
                            // Get file information.
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

                //set the small and large ImageList properties of listview
                listViewSearch.LargeImageList = ImagelistLargeSearch;
                listViewSearch.View = View.LargeIcon;
            }
        }

        // Launch the food cookbook.
        private void OpenCookedFoodList_Button_Click(object sender, EventArgs e)
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
            selectedItemType        = itemType;
            selectedItemAmount      = itemAmount;
            selectedItemVariation   = itemVariation;
            selectedItemSkillset    = itemSkillset;
            this.Close();
        }
        #endregion

        #region Selection Indexes

        // This method will be called for every ListView selection change event.
        private void TabListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            var listView = sender as ListView;     // Get which ListView triggered this event.
            if (listView?.SelectedItems.Count > 0) // Make sure there’s actually a selected item.
            {
                // Get the first selected item's Tag and split it.
                // Tag can be "id,variation" or "id,variation,skillset".
                string[] tagParts = listView.SelectedItems[0].Tag.ToString().Split(',');

                // Set your variables to what the user selected.
                selectedItemType = int.Parse(tagParts[0]);
                selectedItemVariation = int.Parse(tagParts[1]);

                // Skillset is optional; if present, parse it, else set to 0.
                if (tagParts.Length > 2)
                    selectedItemSkillset = int.Parse(tagParts[2]);
                else
                    selectedItemSkillset = 0;

                // Close the editor, returning the selection.
                this.Close();
            }
        }

        // This method is called whenever the mouse is pressed on any ListView.
        private void TabListView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)       // If left-click, set the item amount to 1.
                selectedItemAmount = 1;
            else if (e.Button == MouseButtons.Right) // If right-click, set the item amount to what's in the custom amount box.
                selectedItemAmount = (int)CustomAmount_NumericUpDown.Value;
        }
        #endregion
    }
}
