using Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows.Forms;

namespace CoreKeeperInventoryEditor
{
    public partial class MainForm : Form
    {
        // Setup some varibles.
        public Mem MemLib = new Mem();
        public IEnumerable<long> AoBScanResultsInventory;
        public IEnumerable<long> AoBScanResultsPlayerName;
        public IEnumerable<long> AoBScanResultsChat;
        public List<string> LastChatCommand = new List<string>() { "" };
        public Dictionary<string, int> ExportPlayerItems = new Dictionary<string, int> { };
        public string ExportPlayerName = "";
        public int skinCounter = CoreKeepersWorkshop.Properties.Settings.Default.UIBackgroundCount;

        // Define texture data.
        public IEnumerable<string> ImageFiles1 = Directory.GetFileSystemEntries(AppDomain.CurrentDomain.BaseDirectory + @"assets\Inventory\", "*.png", SearchOption.AllDirectories);
        public IEnumerable<string> InventorySkins = Directory.GetFileSystemEntries(AppDomain.CurrentDomain.BaseDirectory + @"assets\UI\", "*.png", SearchOption.AllDirectories);

        // Form initialization.
        public MainForm()
        {
            InitializeComponent();
        }

        #region Form Controls

        // Do form loading events.
        private void MainForm_Load(object sender, EventArgs e)
        {
            #region Set Background

            // Get background from saved settings.
            // Ensure background is not null.
            if (CoreKeepersWorkshop.Properties.Settings.Default.UIBackground != "")
            {
                // Catch image missing / renamed errors.
                try
                {
                    tabControl1.TabPages[0].BackgroundImage = Image.FromFile(CoreKeepersWorkshop.Properties.Settings.Default.UIBackground);
                }
                catch (Exception)
                {
                    CoreKeepersWorkshop.Properties.Settings.Default.UIBackground = "";
                }
            }
            #endregion

            #region Tooltips

            // Create a new tooltip.
            ToolTip toolTip = new ToolTip();
            toolTip.AutoPopDelay = 3000;
            toolTip.InitialDelay = 1000;

            // Set tool texts.
            toolTip.SetToolTip(textBox1, "Enter the existing loaded player's name.");
            toolTip.SetToolTip(textBox2, "Enter a custom name. Must match current player's name length.");

            toolTip.SetToolTip(button1, "Get the required addresses for editing the inventory.");
            toolTip.SetToolTip(button2, "Reload loads the GUI with updated inventory items.");
            toolTip.SetToolTip(button3, "Remove all items from the inventory.");
            toolTip.SetToolTip(button4, "Change your existing name.");
            toolTip.SetToolTip(button5, "Import a player file to overwrite items.");
            toolTip.SetToolTip(button6, "Export a player file to overwrite items.");
            toolTip.SetToolTip(button7, "Enable / disable in-game chat commands.");

            toolTip.SetToolTip(richTextBox1, "A list of all found addresses. Used mostly for debugging.");

            toolTip.SetToolTip(radioButton1, "Overwrite item slot one.");
            toolTip.SetToolTip(radioButton2, "Add item to an empty inventory slot.");
            toolTip.SetToolTip(radioButton2, "Add items to a custom inventory slot.");

            toolTip.SetToolTip(numericUpDown1, "Change what item slot to send items too.");

            #endregion
        }

        // Launch the link in the browser.
        private void richTextBox2_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/RussDev7/CoreKeepersWorkshop");
        }

        // Reset inventory stats back to defualts.
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Save some form settings.
            CoreKeepersWorkshop.Properties.Settings.Default.ItemAmount = 50;
            CoreKeepersWorkshop.Properties.Settings.Default.ItemID = 110;
            CoreKeepersWorkshop.Properties.Settings.Default.CurrentItemTab = "TabPage1";
            CoreKeepersWorkshop.Properties.Settings.Default.ItemVeriation = 0;

            // Save UI form settings.
            CoreKeepersWorkshop.Properties.Settings.Default.UIBackgroundCount = skinCounter;
            CoreKeepersWorkshop.Properties.Settings.Default.Save();
        }

        // Move window to the bottom left.
        private void Form1_Resize(object sender, EventArgs e)
        {
            // Get window states.
            if (WindowState == FormWindowState.Minimized)
            {
                // Adjust window properties
                this.WindowState = FormWindowState.Normal;
                MainForm.ActiveForm.Size = new Size(320, 37);

                // Get height for both types of taskbar modes.
                Rectangle activeScreenDimensions = Screen.FromControl(this).Bounds;
                MainForm.ActiveForm.Location = new Point(0, activeScreenDimensions.Height - 40);

                // Adjust window properties
                this.Opacity = 0.8;
                MainForm.ActiveForm.MaximizeBox = true;
                MainForm.ActiveForm.MinimizeBox = false;
            }
            else if (WindowState == FormWindowState.Maximized)
            {
                // Adjust window properties
                this.WindowState = FormWindowState.Normal;
                MainForm.ActiveForm.MaximizeBox = false;
                MainForm.ActiveForm.MinimizeBox = true;

                // Ensure we got the correct tab size to maximize back too.
                if (tabControl1.SelectedTab == tabPage1)
                {
                    MainForm.ActiveForm.Size = new Size(804, 371);
                }
                else if (tabControl1.SelectedTab == tabPage2)
                {
                    MainForm.ActiveForm.Size = new Size(410, 360);
                }
                else if (tabControl1.SelectedTab == tabPage5)
                {
                    MainForm.ActiveForm.Size = new Size(410, 360);
                }

                MainForm.ActiveForm.Opacity = 100;
                this.CenterToScreen();
            }
        }

        // Control switching tabs.

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage1)
            {
                MainForm.ActiveForm.Size = new Size(804, 371);
            }
            else if (tabControl1.SelectedTab == tabPage2)
            {
                MainForm.ActiveForm.Size = new Size(410, 360);
            }
            else if (tabControl1.SelectedTab == tabPage5)
            {
                MainForm.ActiveForm.Size = new Size(410, 360);
            }

            // Change skin
            if (tabControl1.SelectedTab == tabPage6)
            {
                // Reset tab page back to one.
                tabControl1.SelectedTab = tabPage1;

                // Prevent overflow from add or removal of images.
                if (skinCounter >= InventorySkins.Count()) { skinCounter = 0; }

                // Change the background.
                tabControl1.TabPages[0].BackgroundImage = Image.FromFile(InventorySkins.ToArray()[skinCounter].ToString());

                // Save the property in the settings.
                CoreKeepersWorkshop.Properties.Settings.Default.UIBackground = InventorySkins.ToArray()[skinCounter].ToString();

                // Add to the counter.
                skinCounter++;
                if (skinCounter == InventorySkins.Count()) { skinCounter = 0; }
            }
        }

        #endregion // End form controls.

        #region Inventory Editor

        // Get Inventory addresses.
        private void button1_Click(object sender, EventArgs e)
        {
            // Reset progress bar.
            progressBar2.Value = 0;

            // Load addresses.
            GetInventoryAddresses();
        }

        public async void GetInventoryAddresses()
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Name button to indicate loading.
            button1.Text = "Loading Addresses...";

            // Reset textbox.
            richTextBox1.Text = "Addresses Loaded: 0";

            // Offset the progress bar to show it's working.
            progressBar2.Visible = true;
            progressBar2.Maximum = 100;
            progressBar2.Value = 10;

            // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
            AoBScanResultsInventory = await MemLib.AoBScan("6E 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 6E 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00", true, true);

            // Get the progress bar maximum.
            progressBar2.Maximum = AoBScanResultsInventory.Count() * 30;

            // If the count is zero, the scan had an error.
            if (AoBScanResultsInventory.Count() == 0 | AoBScanResultsInventory.Count() < 10)
            {
                // Reset textbox.
                richTextBox1.Text = "Addresses Loaded: 0";

                // Reset progress bar.
                progressBar2.Value = 0;
                progressBar2.Visible = false;

                // Rename button back to defualt.
                button1.Text = "Get Inventory Addresses";

                // Reset aob scan results
                AoBScanResultsInventory = null;

                // Display error message.
                MessageBox.Show("You need to have torches in the first and last Inventory slots!!\n\nPlease ignore added inventory rows.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Update richtextbox with found addresses.
            foreach (long res in AoBScanResultsInventory)
            {
                if (richTextBox1.Text == "Addresses Loaded: 0")
                {
                    richTextBox1.Text = "Addresses Loaded: " + AoBScanResultsInventory.Count().ToString() + " [" + res.ToString("X").ToString();
                }
                else
                {
                    richTextBox1.Text += ", " + res.ToString("X").ToString();
                }
            }
            richTextBox1.Text += "]";

            // Reset item id richtextbox.
            richTextBox3.Text = "If any unknown items are found, their ID's will appear here!" + Environment.NewLine + "------------------------------------------------------------------------------------------------------------" + Environment.NewLine;

            // Name button to indicate loading.
            button1.Text = "Loading Assets...";

            // Load Inventory.
            AddItemToInv(loadInventory: true);
        }

        // this function is async, which means it does not block other code
        public void AddItemToInv(int itemSlot = 1, int type = 1, int variation = 0, int amount = 1, bool loadInventory = false, bool CycleAll = false, bool ExportInventory = false, bool Overwrite = false, bool GetItemInfo = false, bool AddToEmpty = false)
        {
            #region Add Items Upon Editing

            if (AoBScanResultsInventory == null)
            {
                // Rename button back to defualt.
                button1.Text = "Get Inventory Addresses";
                button2.Text = "Reload Inventory";
                button3.Text = "Remove All";

                // Reset progress bar.
                progressBar2.Value = 0;
                progressBar2.Visible = false;

                MessageBox.Show("You need to first scan for the Inventory addresses!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Set playername in jason array.
            if (ExportInventory)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Player File|*.ckplayer";

                // Ensure the user chose a file.
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ExportPlayerName = sfd.FileName;

                    // Reset export item names.
                    ExportPlayerItems.Clear();

                    // Reset progress bar.
                    progressBar1.Value = 0;
                }
                else
                {
                    return;
                }
            }

            // Define some varibles for item info.
            int infoType = 0;
            int infoAmount = 0;
            int infoVariant = 0;

            // Define a varible to hold the new item amount information.
            int finalItemAmount = 0;

            // Iterate through each found address.
            foreach (long res in AoBScanResultsInventory)
            {
                // Get address from loop.
                string baseAddress = res.ToString("X").ToString();

                #region Set Inventory Items

                // Remove Existing Images
                if (loadInventory || CycleAll)
                {
                    pictureBox1.Image = null;
                    pictureBox2.Image = null;
                    pictureBox3.Image = null;
                    pictureBox4.Image = null;
                    pictureBox5.Image = null;
                    pictureBox6.Image = null;
                    pictureBox7.Image = null;
                    pictureBox8.Image = null;
                    pictureBox9.Image = null;
                    pictureBox10.Image = null;
                    pictureBox11.Image = null;
                    pictureBox12.Image = null;
                    pictureBox13.Image = null;
                    pictureBox14.Image = null;
                    pictureBox14.Image = null;
                    pictureBox15.Image = null;
                    pictureBox16.Image = null;
                    pictureBox17.Image = null;
                    pictureBox18.Image = null;
                    pictureBox19.Image = null;
                    pictureBox20.Image = null;
                    pictureBox21.Image = null;
                    pictureBox22.Image = null;
                    pictureBox23.Image = null;
                    pictureBox24.Image = null;
                    pictureBox25.Image = null;
                    pictureBox26.Image = null;
                    pictureBox27.Image = null;
                    pictureBox28.Image = null;
                    pictureBox29.Image = null;
                    pictureBox30.Image = null;
                }

                // Make some exception catches
                try
                {
                    // Get Offsets for Inventory.
                    if (!AddToEmpty && (itemSlot == 1 || loadInventory || CycleAll || ExportInventory))
                    {
                        string slot1Item = baseAddress;
                        string slot1Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("4", NumberStyles.Integer)).ToString("X");
                        string slot1Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot1Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot1Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot1Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot1Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot1Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot1Amount, "int", (MemLib.ReadUInt(slot1Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot1Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)(MemLib.ReadUInt(slot1Amount) + amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventeory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot1"))
                                {
                                    ExportPlayerItems.Add("itemSlot1-ID", MemLib.ReadInt(slot1Item));
                                    ExportPlayerItems.Add("itemSlot1-Amount", MemLib.ReadInt(slot1Amount));
                                    ExportPlayerItems.Add("itemSlot1-Variation", MemLib.ReadInt(slot1Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot1Item);
                                variation = MemLib.ReadInt(slot1Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox1.Image = null;
                                }
                                else if (pictureBox1.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox1.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox1.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot1Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox1.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox1.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox1.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot1Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox1.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 1 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot1Amount) + " | Variation: " + (MemLib.ReadInt(slot1Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 1 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot1Amount) + " | Variation: " + (MemLib.ReadInt(slot1Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot1Item);
                            infoAmount = MemLib.ReadInt(slot1Amount);
                            infoVariant = MemLib.ReadInt(slot1Variation);
                        }
                    }
                    if (itemSlot == 2 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot2Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("16", NumberStyles.Integer)).ToString("X");
                        string slot2Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("20", NumberStyles.Integer)).ToString("X");
                        string slot2Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("24", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot2Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot2Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot2Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot2Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot2Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot2Amount, "int", (MemLib.ReadUInt(slot2Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot2Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot2Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot2"))
                                {
                                    ExportPlayerItems.Add("itemSlot2-ID", MemLib.ReadInt(slot2Item));
                                    ExportPlayerItems.Add("itemSlot2-Amount", MemLib.ReadInt(slot2Amount));
                                    ExportPlayerItems.Add("itemSlot2-Variation", MemLib.ReadInt(slot2Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot2Item);
                                variation = MemLib.ReadInt(slot2Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox2.Image = null;
                                }
                                else if (pictureBox2.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox2.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox2.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox2.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot2Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox2.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox2.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox2.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox2.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot2Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox2.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 2 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot2Amount) + " | Variation: " + (MemLib.ReadInt(slot2Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 2 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot2Amount) + " | Variation: " + (MemLib.ReadInt(slot2Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot2Item);
                            infoAmount = MemLib.ReadInt(slot2Amount);
                            infoVariant = MemLib.ReadInt(slot2Variation);
                        }
                    }
                    if (itemSlot == 3 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot3Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("32", NumberStyles.Integer)).ToString("X");
                        string slot3Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("36", NumberStyles.Integer)).ToString("X");
                        string slot3Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("40", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot3Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot3Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot3Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot3Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot3Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot3Amount, "int", (MemLib.ReadUInt(slot3Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot3Variation, "int", variation.ToString()); // Write item variation
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot3"))
                                {
                                    ExportPlayerItems.Add("itemSlot3-ID", MemLib.ReadInt(slot3Item));
                                    ExportPlayerItems.Add("itemSlot3-Amount", MemLib.ReadInt(slot3Amount));
                                    ExportPlayerItems.Add("itemSlot3-Variation", MemLib.ReadInt(slot3Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot3Item);
                                variation = MemLib.ReadInt(slot3Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox3.Image = null;
                                }
                                else if (pictureBox3.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox3.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox3.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox3.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot3Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox3.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox3.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox3.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox3.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot3Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox3.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 3 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot3Amount) + " | Variation: " + (MemLib.ReadInt(slot3Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 3 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot3Amount) + " | Variation: " + (MemLib.ReadInt(slot3Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot3Item);
                            infoAmount = MemLib.ReadInt(slot3Amount);
                            infoVariant = MemLib.ReadInt(slot3Variation);
                        }
                    }
                    if (itemSlot == 4 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot4Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("48", NumberStyles.Integer)).ToString("X");
                        string slot4Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("52", NumberStyles.Integer)).ToString("X");
                        string slot4Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("56", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot4Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot4Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot4Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot4Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot4Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot4Amount, "int", (MemLib.ReadUInt(slot4Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot4Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot4Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot4"))
                                {
                                    ExportPlayerItems.Add("itemSlot4-ID", MemLib.ReadInt(slot4Item));
                                    ExportPlayerItems.Add("itemSlot4-Amount", MemLib.ReadInt(slot4Amount));
                                    ExportPlayerItems.Add("itemSlot4-Variation", MemLib.ReadInt(slot4Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot4Item);
                                variation = MemLib.ReadInt(slot4Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox4.Image = null;
                                }
                                else if (pictureBox4.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox4.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox4.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox4.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot4Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox4.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox4.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox4.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox4.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot4Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox4.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 4 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot4Amount) + " | Variation: " + (MemLib.ReadInt(slot4Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 4 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot4Amount) + " | Variation: " + (MemLib.ReadInt(slot4Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot4Item);
                            infoAmount = MemLib.ReadInt(slot4Amount);
                            infoVariant = MemLib.ReadInt(slot4Variation);
                        }
                    }
                    if (itemSlot == 5 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot5Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("64", NumberStyles.Integer)).ToString("X");
                        string slot5Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("68", NumberStyles.Integer)).ToString("X");
                        string slot5Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("72", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot5Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot5Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot5Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot5Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot5Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot5Amount, "int", (MemLib.ReadUInt(slot5Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot5Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot5Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot5"))
                                {
                                    ExportPlayerItems.Add("itemSlot5-ID", MemLib.ReadInt(slot5Item));
                                    ExportPlayerItems.Add("itemSlot5-Amount", MemLib.ReadInt(slot5Amount));
                                    ExportPlayerItems.Add("itemSlot5-Variation", MemLib.ReadInt(slot5Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot5Item);
                                variation = MemLib.ReadInt(slot5Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox5.Image = null;
                                }
                                else if (pictureBox5.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox5.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox5.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox5.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot5Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox5.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox5.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox5.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox5.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot5Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox5.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 5 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot5Amount) + " | Variation: " + (MemLib.ReadInt(slot5Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 5 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot5Amount) + " | Variation: " + (MemLib.ReadInt(slot5Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot5Item);
                            infoAmount = MemLib.ReadInt(slot5Amount);
                            infoVariant = MemLib.ReadInt(slot5Variation);
                        }
                    }
                    if (itemSlot == 6 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot6Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("80", NumberStyles.Integer)).ToString("X");
                        string slot6Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("84", NumberStyles.Integer)).ToString("X");
                        string slot6Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("88", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot6Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot6Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot6Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot6Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot6Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot6Amount, "int", (MemLib.ReadUInt(slot6Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot6Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot6Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot6"))
                                {
                                    ExportPlayerItems.Add("itemSlot6-ID", MemLib.ReadInt(slot6Item));
                                    ExportPlayerItems.Add("itemSlot6-Amount", MemLib.ReadInt(slot6Amount));
                                    ExportPlayerItems.Add("itemSlot6-Variation", MemLib.ReadInt(slot6Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot6Item);
                                variation = MemLib.ReadInt(slot6Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox6.Image = null;
                                }
                                else if (pictureBox6.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox6.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox6.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox6.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot6Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox6.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox6.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox6.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox6.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot6Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox6.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 6 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot6Amount) + " | Variation: " + (MemLib.ReadInt(slot6Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 6 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot6Amount) + " | Variation: " + (MemLib.ReadInt(slot6Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot6Item);
                            infoAmount = MemLib.ReadInt(slot6Amount);
                            infoVariant = MemLib.ReadInt(slot6Variation);
                        }
                    }
                    if (itemSlot == 7 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot7Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("96", NumberStyles.Integer)).ToString("X");
                        string slot7Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("100", NumberStyles.Integer)).ToString("X");
                        string slot7Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("104", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot7Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot7Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot7Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot7Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot7Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot7Amount, "int", (MemLib.ReadUInt(slot7Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot7Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot7Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot7"))
                                {
                                    ExportPlayerItems.Add("itemSlot7-ID", MemLib.ReadInt(slot7Item));
                                    ExportPlayerItems.Add("itemSlot7-Amount", MemLib.ReadInt(slot7Amount));
                                    ExportPlayerItems.Add("itemSlot7-Variation", MemLib.ReadInt(slot7Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot7Item);
                                variation = MemLib.ReadInt(slot7Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox7.Image = null;
                                }
                                else if (pictureBox7.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox7.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox7.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox7.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot7Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox7.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox7.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox7.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox7.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot7Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox7.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 7 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot7Amount) + " | Variation: " + (MemLib.ReadInt(slot7Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 7 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot7Amount) + " | Variation: " + (MemLib.ReadInt(slot7Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot7Item);
                            infoAmount = MemLib.ReadInt(slot7Amount);
                            infoVariant = MemLib.ReadInt(slot7Variation);
                        }
                    }
                    if (itemSlot == 8 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot8Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("112", NumberStyles.Integer)).ToString("X");
                        string slot8Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("116", NumberStyles.Integer)).ToString("X");
                        string slot8Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("120", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot8Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot8Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot8Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot8Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot8Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot8Amount, "int", (MemLib.ReadUInt(slot8Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot8Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot8Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot8"))
                                {
                                    ExportPlayerItems.Add("itemSlot8-ID", MemLib.ReadInt(slot8Item));
                                    ExportPlayerItems.Add("itemSlot8-Amount", MemLib.ReadInt(slot8Amount));
                                    ExportPlayerItems.Add("itemSlot8-Variation", MemLib.ReadInt(slot8Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot8Item);
                                variation = MemLib.ReadInt(slot8Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox8.Image = null;
                                }
                                else if (pictureBox8.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox8.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox8.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox8.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot8Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox8.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox8.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox8.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox8.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot8Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox8.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 8 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot8Amount) + " | Variation: " + (MemLib.ReadInt(slot8Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 8 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot8Amount) + " | Variation: " + (MemLib.ReadInt(slot8Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot8Item);
                            infoAmount = MemLib.ReadInt(slot8Amount);
                            infoVariant = MemLib.ReadInt(slot8Variation);
                        }
                    }
                    if (itemSlot == 9 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot9Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("128", NumberStyles.Integer)).ToString("X");
                        string slot9Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("132", NumberStyles.Integer)).ToString("X");
                        string slot9Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("136", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot9Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot9Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot9Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot9Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot9Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot9Amount, "int", (MemLib.ReadUInt(slot9Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot9Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot9Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot9"))
                                {
                                    ExportPlayerItems.Add("itemSlot9-ID", MemLib.ReadInt(slot9Item));
                                    ExportPlayerItems.Add("itemSlot9-Amount", MemLib.ReadInt(slot9Amount));
                                    ExportPlayerItems.Add("itemSlot9-Variation", MemLib.ReadInt(slot9Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot9Item);
                                variation = MemLib.ReadInt(slot9Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox9.Image = null;
                                }
                                else if (pictureBox9.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox9.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox9.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox9.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot9Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox9.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox9.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox9.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox9.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot9Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox9.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 9 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot9Amount) + " | Variation: " + (MemLib.ReadInt(slot9Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 9 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot9Amount) + " | Variation: " + (MemLib.ReadInt(slot9Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot9Item);
                            infoAmount = MemLib.ReadInt(slot9Amount);
                            infoVariant = MemLib.ReadInt(slot9Variation);
                        }
                    }
                    if (itemSlot == 10 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot10Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("144", NumberStyles.Integer)).ToString("X");
                        string slot10Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("148", NumberStyles.Integer)).ToString("X");
                        string slot10Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("152", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot10Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot10Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot10Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot10Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot10Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot10Amount, "int", (MemLib.ReadUInt(slot10Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot10Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot10Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot10"))
                                {
                                    ExportPlayerItems.Add("itemSlot10-ID", MemLib.ReadInt(slot10Item));
                                    ExportPlayerItems.Add("itemSlot10-Amount", MemLib.ReadInt(slot10Amount));
                                    ExportPlayerItems.Add("itemSlot10-Variation", MemLib.ReadInt(slot10Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot10Item);
                                variation = MemLib.ReadInt(slot10Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox10.Image = null;
                                }
                                else if (pictureBox10.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox10.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox10.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox10.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot10Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox10.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox10.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox10.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox10.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot10Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox10.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 10 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot10Amount) + " | Variation: " + (MemLib.ReadInt(slot10Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 10 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot10Amount) + " | Variation: " + (MemLib.ReadInt(slot10Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot10Item);
                            infoAmount = MemLib.ReadInt(slot10Amount);
                            infoVariant = MemLib.ReadInt(slot10Variation);
                        }
                    }
                    if (itemSlot == 11 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot11Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("160", NumberStyles.Integer)).ToString("X");
                        string slot11Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("164", NumberStyles.Integer)).ToString("X");
                        string slot11Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("168", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot11Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot11Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot11Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot11Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot11Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot11Amount, "int", (MemLib.ReadUInt(slot11Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot11Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot11Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot11"))
                                {
                                    ExportPlayerItems.Add("itemSlot11-ID", MemLib.ReadInt(slot11Item));
                                    ExportPlayerItems.Add("itemSlot11-Amount", MemLib.ReadInt(slot11Amount));
                                    ExportPlayerItems.Add("itemSlot11-Variation", MemLib.ReadInt(slot11Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot11Item);
                                variation = MemLib.ReadInt(slot11Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox11.Image = null;
                                }
                                else if (pictureBox11.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox11.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox11.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox11.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot11Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox11.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox11.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox11.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox11.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot11Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox11.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 11 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot11Amount) + " | Variation: " + (MemLib.ReadInt(slot11Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 11 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot11Amount) + " | Variation: " + (MemLib.ReadInt(slot11Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot11Item);
                            infoAmount = MemLib.ReadInt(slot11Amount);
                            infoVariant = MemLib.ReadInt(slot11Variation);
                        }
                    }
                    if (itemSlot == 12 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot12Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("176", NumberStyles.Integer)).ToString("X");
                        string slot12Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("180", NumberStyles.Integer)).ToString("X");
                        string slot12Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("184", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot12Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot12Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot12Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot12Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot12Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot12Amount, "int", (MemLib.ReadUInt(slot12Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot12Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot12Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot12"))
                                {
                                    ExportPlayerItems.Add("itemSlot12-ID", MemLib.ReadInt(slot12Item));
                                    ExportPlayerItems.Add("itemSlot12-Amount", MemLib.ReadInt(slot12Amount));
                                    ExportPlayerItems.Add("itemSlot12-Variation", MemLib.ReadInt(slot12Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot12Item);
                                variation = MemLib.ReadInt(slot12Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox12.Image = null;
                                }
                                else if (pictureBox12.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox12.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox12.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox12.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot12Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox12.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox12.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox12.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox12.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot12Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox12.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 12 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot12Amount) + " | Variation: " + (MemLib.ReadInt(slot12Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 12 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot12Amount) + " | Variation: " + (MemLib.ReadInt(slot12Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot12Item);
                            infoAmount = MemLib.ReadInt(slot12Amount);
                            infoVariant = MemLib.ReadInt(slot12Variation);
                        }
                    }
                    if (itemSlot == 13 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot13Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("192", NumberStyles.Integer)).ToString("X");
                        string slot13Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("196", NumberStyles.Integer)).ToString("X");
                        string slot13Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("200", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot13Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot13Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot13Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot13Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot13Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot13Amount, "int", (MemLib.ReadUInt(slot13Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot13Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot13Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot13"))
                                {
                                    ExportPlayerItems.Add("itemSlot13-ID", MemLib.ReadInt(slot13Item));
                                    ExportPlayerItems.Add("itemSlot13-Amount", MemLib.ReadInt(slot13Amount));
                                    ExportPlayerItems.Add("itemSlot13-Variation", MemLib.ReadInt(slot13Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot13Item);
                                variation = MemLib.ReadInt(slot13Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox13.Image = null;
                                }
                                else if (pictureBox13.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox13.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox13.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox13.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot13Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox13.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox13.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox13.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox13.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot13Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox13.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 13 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot13Amount) + " | Variation: " + (MemLib.ReadInt(slot13Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 13 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot13Amount) + " | Variation: " + (MemLib.ReadInt(slot13Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot13Item);
                            infoAmount = MemLib.ReadInt(slot13Amount);
                            infoVariant = MemLib.ReadInt(slot13Variation);
                        }
                    }
                    if (itemSlot == 14 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot14Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("208", NumberStyles.Integer)).ToString("X");
                        string slot14Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("212", NumberStyles.Integer)).ToString("X");
                        string slot14Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("216", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot14Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot14Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot14Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot14Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot14Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot14Amount, "int", (MemLib.ReadUInt(slot14Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot14Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot14Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot14"))
                                {
                                    ExportPlayerItems.Add("itemSlot14-ID", MemLib.ReadInt(slot14Item));
                                    ExportPlayerItems.Add("itemSlot14-Amount", MemLib.ReadInt(slot14Amount));
                                    ExportPlayerItems.Add("itemSlot14-Variation", MemLib.ReadInt(slot14Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot14Item);
                                variation = MemLib.ReadInt(slot14Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox14.Image = null;
                                }
                                else if (pictureBox14.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox14.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox14.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox14.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot14Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox14.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox14.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox14.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox14.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot14Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox14.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 14 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot14Amount) + " | Variation: " + (MemLib.ReadInt(slot14Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 14 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot14Amount) + " | Variation: " + (MemLib.ReadInt(slot14Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot14Item);
                            infoAmount = MemLib.ReadInt(slot14Amount);
                            infoVariant = MemLib.ReadInt(slot14Variation);
                        }
                    }
                    if (itemSlot == 15 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot15Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("224", NumberStyles.Integer)).ToString("X");
                        string slot15Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("228", NumberStyles.Integer)).ToString("X");
                        string slot15Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("232", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot15Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot15Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot15Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot15Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot15Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot15Amount, "int", (MemLib.ReadUInt(slot15Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot15Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot15Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot15"))
                                {
                                    ExportPlayerItems.Add("itemSlot15-ID", MemLib.ReadInt(slot15Item));
                                    ExportPlayerItems.Add("itemSlot15-Amount", MemLib.ReadInt(slot15Amount));
                                    ExportPlayerItems.Add("itemSlot15-Variation", MemLib.ReadInt(slot15Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot15Item);
                                variation = MemLib.ReadInt(slot15Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox15.Image = null;
                                }
                                else if (pictureBox15.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox15.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox15.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox15.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot15Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox15.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox15.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox15.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox15.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot15Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox15.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 15 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot15Amount) + " | Variation: " + (MemLib.ReadInt(slot15Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 15 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot15Amount) + " | Variation: " + (MemLib.ReadInt(slot15Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }

                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot15Item);
                            infoAmount = MemLib.ReadInt(slot15Amount);
                            infoVariant = MemLib.ReadInt(slot15Variation);
                        }
                    }
                    if (itemSlot == 16 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot16Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("240", NumberStyles.Integer)).ToString("X");
                        string slot16Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("244", NumberStyles.Integer)).ToString("X");
                        string slot16Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("248", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot16Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot16Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot16Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot16Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot16Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot16Amount, "int", (MemLib.ReadUInt(slot16Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot16Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot16Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot16"))
                                {
                                    ExportPlayerItems.Add("itemSlot16-ID", MemLib.ReadInt(slot16Item));
                                    ExportPlayerItems.Add("itemSlot16-Amount", MemLib.ReadInt(slot16Amount));
                                    ExportPlayerItems.Add("itemSlot16-Variation", MemLib.ReadInt(slot16Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot16Item);
                                variation = MemLib.ReadInt(slot16Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox16.Image = null;
                                }
                                else if (pictureBox16.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox16.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox16.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox16.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot16Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox16.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox16.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox16.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox16.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot16Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox16.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 16 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot16Amount) + " | Variation: " + (MemLib.ReadInt(slot16Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 16 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot16Amount) + " | Variation: " + (MemLib.ReadInt(slot16Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot16Item);
                            infoAmount = MemLib.ReadInt(slot16Amount);
                            infoVariant = MemLib.ReadInt(slot16Variation);
                        }
                    }
                    if (itemSlot == 17 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot17Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("256", NumberStyles.Integer)).ToString("X");
                        string slot17Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("260", NumberStyles.Integer)).ToString("X");
                        string slot17Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("264", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot17Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot17Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot17Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot17Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot17Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot17Amount, "int", (MemLib.ReadUInt(slot17Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot17Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot17Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot17"))
                                {
                                    ExportPlayerItems.Add("itemSlot17-ID", MemLib.ReadInt(slot17Item));
                                    ExportPlayerItems.Add("itemSlot17-Amount", MemLib.ReadInt(slot17Amount));
                                    ExportPlayerItems.Add("itemSlot17-Variation", MemLib.ReadInt(slot17Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot17Item);
                                variation = MemLib.ReadInt(slot17Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox17.Image = null;
                                }
                                else if (pictureBox17.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox17.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox17.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox17.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot17Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox17.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox17.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox17.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox17.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot17Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox17.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 17 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot17Amount) + " | Variation: " + (MemLib.ReadInt(slot17Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 17 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot17Amount) + " | Variation: " + (MemLib.ReadInt(slot17Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot17Item);
                            infoAmount = MemLib.ReadInt(slot17Amount);
                            infoVariant = MemLib.ReadInt(slot17Variation);
                        }
                    }
                    if (itemSlot == 18 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot18Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("272", NumberStyles.Integer)).ToString("X");
                        string slot18Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("276", NumberStyles.Integer)).ToString("X");
                        string slot18Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("280", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot18Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot18Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot18Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot18Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot18Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot18Amount, "int", (MemLib.ReadUInt(slot18Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot18Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot18Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot18"))
                                {
                                    ExportPlayerItems.Add("itemSlot18-ID", MemLib.ReadInt(slot18Item));
                                    ExportPlayerItems.Add("itemSlot18-Amount", MemLib.ReadInt(slot18Amount));
                                    ExportPlayerItems.Add("itemSlot18-Variation", MemLib.ReadInt(slot18Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot18Item);
                                variation = MemLib.ReadInt(slot18Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox18.Image = null;
                                }
                                else if (pictureBox18.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox18.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox18.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox18.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot18Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox18.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox18.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox18.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox18.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot18Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox18.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 18 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot18Amount) + " | Variation: " + (MemLib.ReadInt(slot18Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 18 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot18Amount) + " | Variation: " + (MemLib.ReadInt(slot18Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot18Item);
                            infoAmount = MemLib.ReadInt(slot18Amount);
                            infoVariant = MemLib.ReadInt(slot18Variation);
                        }
                    }
                    if (itemSlot == 19 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot19Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("288", NumberStyles.Integer)).ToString("X");
                        string slot19Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("292", NumberStyles.Integer)).ToString("X");
                        string slot19Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("296", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot19Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot19Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot19Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot19Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot19Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot19Amount, "int", (MemLib.ReadUInt(slot19Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot19Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot19Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot19"))
                                {
                                    ExportPlayerItems.Add("itemSlot19-ID", MemLib.ReadInt(slot19Item));
                                    ExportPlayerItems.Add("itemSlot19-Amount", MemLib.ReadInt(slot19Amount));
                                    ExportPlayerItems.Add("itemSlot19-Variation", MemLib.ReadInt(slot19Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot19Item);
                                variation = MemLib.ReadInt(slot19Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox19.Image = null;
                                }
                                else if (pictureBox19.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox19.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox19.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox19.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot19Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox19.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox19.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox19.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox19.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot19Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox19.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 19 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot19Amount) + " | Variation: " + (MemLib.ReadInt(slot19Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 19 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot19Amount) + " | Variation: " + (MemLib.ReadInt(slot19Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot19Item);
                            infoAmount = MemLib.ReadInt(slot19Amount);
                            infoVariant = MemLib.ReadInt(slot19Variation);
                        }
                    }
                    if (itemSlot == 20 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot20Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("304", NumberStyles.Integer)).ToString("X");
                        string slot20Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("308", NumberStyles.Integer)).ToString("X");
                        string slot20Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("312", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot20Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot20Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot20Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot20Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot20Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot20Amount, "int", (MemLib.ReadUInt(slot20Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot20Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot20Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot20"))
                                {
                                    ExportPlayerItems.Add("itemSlot20-ID", MemLib.ReadInt(slot20Item));
                                    ExportPlayerItems.Add("itemSlot20-Amount", MemLib.ReadInt(slot20Amount));
                                    ExportPlayerItems.Add("itemSlot20-Variation", MemLib.ReadInt(slot20Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot20Item);
                                variation = MemLib.ReadInt(slot20Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox20.Image = null;
                                }
                                else if (pictureBox20.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox20.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox20.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox20.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot20Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox20.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox20.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox20.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox20.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot20Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox20.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 20 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot20Amount) + " | Variation: " + (MemLib.ReadInt(slot20Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 20 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot20Amount) + " | Variation: " + (MemLib.ReadInt(slot20Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot20Item);
                            infoAmount = MemLib.ReadInt(slot20Amount);
                            infoVariant = MemLib.ReadInt(slot20Variation);
                        }
                    }
                    if (itemSlot == 21 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot21Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("320", NumberStyles.Integer)).ToString("X");
                        string slot21Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("324", NumberStyles.Integer)).ToString("X");
                        string slot21Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("328", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot21Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot21Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot21Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot21Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot21Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot21Amount, "int", (MemLib.ReadUInt(slot21Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot21Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot21Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot21"))
                                {
                                    ExportPlayerItems.Add("itemSlot21-ID", MemLib.ReadInt(slot21Item));
                                    ExportPlayerItems.Add("itemSlot21-Amount", MemLib.ReadInt(slot21Amount));
                                    ExportPlayerItems.Add("itemSlot21-Variation", MemLib.ReadInt(slot21Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot21Item);
                                variation = MemLib.ReadInt(slot21Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox21.Image = null;
                                }
                                else if (pictureBox21.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox21.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox21.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox21.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot21Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox21.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox21.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox21.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox21.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot21Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox21.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 21 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot21Amount) + " | Variation: " + (MemLib.ReadInt(slot21Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 21 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot21Amount) + " | Variation: " + (MemLib.ReadInt(slot21Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot21Item);
                            infoAmount = MemLib.ReadInt(slot21Amount);
                            infoVariant = MemLib.ReadInt(slot21Variation);
                        }
                    }
                    if (itemSlot == 22 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot22Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("336", NumberStyles.Integer)).ToString("X");
                        string slot22Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("340", NumberStyles.Integer)).ToString("X");
                        string slot22Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("344", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot22Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot22Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot22Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot22Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot22Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot22Amount, "int", (MemLib.ReadUInt(slot22Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot22Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot22Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot22"))
                                {
                                    ExportPlayerItems.Add("itemSlot22-ID", MemLib.ReadInt(slot22Item));
                                    ExportPlayerItems.Add("itemSlot22-Amount", MemLib.ReadInt(slot22Amount));
                                    ExportPlayerItems.Add("itemSlot22-Variation", MemLib.ReadInt(slot22Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot22Item);
                                variation = MemLib.ReadInt(slot22Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox22.Image = null;
                                }
                                else if (pictureBox22.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox22.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox22.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox22.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot22Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox22.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox22.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox22.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox22.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot22Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox22.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 22 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot22Amount) + " | Variation: " + (MemLib.ReadInt(slot22Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 22 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot22Amount) + " | Variation: " + (MemLib.ReadInt(slot22Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot22Item);
                            infoAmount = MemLib.ReadInt(slot22Amount);
                            infoVariant = MemLib.ReadInt(slot22Variation);
                        }
                    }
                    if (itemSlot == 23 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot23Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("352", NumberStyles.Integer)).ToString("X");
                        string slot23Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("356", NumberStyles.Integer)).ToString("X");
                        string slot23Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("360", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot23Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot23Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot23Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot23Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot23Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot23Amount, "int", (MemLib.ReadUInt(slot23Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot23Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot23Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot23"))
                                {
                                    ExportPlayerItems.Add("itemSlot23-ID", MemLib.ReadInt(slot23Item));
                                    ExportPlayerItems.Add("itemSlot23-Amount", MemLib.ReadInt(slot23Amount));
                                    ExportPlayerItems.Add("itemSlot23-Variation", MemLib.ReadInt(slot23Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot23Item);
                                variation = MemLib.ReadInt(slot23Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox23.Image = null;
                                }
                                else if (pictureBox23.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox23.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox23.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox23.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot23Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox23.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox23.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox23.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox23.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot23Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox23.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 23 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot23Amount) + " | Variation: " + (MemLib.ReadInt(slot23Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 23 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot23Amount) + " | Variation: " + (MemLib.ReadInt(slot23Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot23Item);
                            infoAmount = MemLib.ReadInt(slot23Amount);
                            infoVariant = MemLib.ReadInt(slot23Variation);
                        }
                    }
                    if (itemSlot == 24 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot24Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("368", NumberStyles.Integer)).ToString("X");
                        string slot24Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("372", NumberStyles.Integer)).ToString("X");
                        string slot24Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("376", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot24Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot24Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot24Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot24Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot24Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot24Amount, "int", (MemLib.ReadUInt(slot24Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot24Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot24Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot24"))
                                {
                                    ExportPlayerItems.Add("itemSlot24-ID", MemLib.ReadInt(slot24Item));
                                    ExportPlayerItems.Add("itemSlot24-Amount", MemLib.ReadInt(slot24Amount));
                                    ExportPlayerItems.Add("itemSlot24-Variation", MemLib.ReadInt(slot24Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot24Item);
                                variation = MemLib.ReadInt(slot24Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox24.Image = null;
                                }
                                else if (pictureBox24.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox24.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox24.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox24.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot24Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox24.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox24.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox24.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox24.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot24Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox24.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 24 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot24Amount) + " | Variation: " + (MemLib.ReadInt(slot24Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 24 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot24Amount) + " | Variation: " + (MemLib.ReadInt(slot24Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot24Item);
                            infoAmount = MemLib.ReadInt(slot24Amount);
                            infoVariant = MemLib.ReadInt(slot24Variation);
                        }
                    }
                    if (itemSlot == 25 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot25Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("384", NumberStyles.Integer)).ToString("X");
                        string slot25Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("388", NumberStyles.Integer)).ToString("X");
                        string slot25Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("392", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot25Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot25Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot25Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot25Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot25Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot25Amount, "int", (MemLib.ReadUInt(slot25Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot25Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot25Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot25"))
                                {
                                    ExportPlayerItems.Add("itemSlot25-ID", MemLib.ReadInt(slot25Item));
                                    ExportPlayerItems.Add("itemSlot25-Amount", MemLib.ReadInt(slot25Amount));
                                    ExportPlayerItems.Add("itemSlot25-Variation", MemLib.ReadInt(slot25Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot25Item);
                                variation = MemLib.ReadInt(slot25Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox25.Image = null;
                                }
                                else if (pictureBox25.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox25.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox25.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox25.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot25Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox25.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox25.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox25.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox25.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot25Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox25.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 25 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot25Amount) + " | Variation: " + (MemLib.ReadInt(slot25Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 25 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot25Amount) + " | Variation: " + (MemLib.ReadInt(slot25Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot25Item);
                            infoAmount = MemLib.ReadInt(slot25Amount);
                            infoVariant = MemLib.ReadInt(slot25Variation);
                        }
                    }
                    if (itemSlot == 26 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot26Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("400", NumberStyles.Integer)).ToString("X");
                        string slot26Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("404", NumberStyles.Integer)).ToString("X");
                        string slot26Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("408", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot26Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot26Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot26Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot26Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot26Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot26Amount, "int", (MemLib.ReadUInt(slot26Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot26Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot26Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot26"))
                                {
                                    ExportPlayerItems.Add("itemSlot26-ID", MemLib.ReadInt(slot26Item));
                                    ExportPlayerItems.Add("itemSlot26-Amount", MemLib.ReadInt(slot26Amount));
                                    ExportPlayerItems.Add("itemSlot26-Variation", MemLib.ReadInt(slot26Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot26Item);
                                variation = MemLib.ReadInt(slot26Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox26.Image = null;
                                }
                                else if (pictureBox26.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox26.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox26.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox26.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot26Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox26.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox26.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox26.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox26.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot26Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox26.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 26 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot26Amount) + " | Variation: " + (MemLib.ReadInt(slot26Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 26 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot26Amount) + " | Variation: " + (MemLib.ReadInt(slot26Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot26Item);
                            infoAmount = MemLib.ReadInt(slot26Amount);
                            infoVariant = MemLib.ReadInt(slot26Variation);
                        }
                    }
                    if (itemSlot == 27 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot27Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("416", NumberStyles.Integer)).ToString("X");
                        string slot27Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("420", NumberStyles.Integer)).ToString("X");
                        string slot27Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("424", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot27Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot27Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot27Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot27Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot27Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot27Amount, "int", (MemLib.ReadUInt(slot27Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot27Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot27Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot27"))
                                {
                                    ExportPlayerItems.Add("itemSlot27-ID", MemLib.ReadInt(slot27Item));
                                    ExportPlayerItems.Add("itemSlot27-Amount", MemLib.ReadInt(slot27Amount));
                                    ExportPlayerItems.Add("itemSlot27-Variation", MemLib.ReadInt(slot27Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot27Item);
                                variation = MemLib.ReadInt(slot27Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox27.Image = null;
                                }
                                else if (pictureBox27.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox27.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox27.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox27.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot27Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox27.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox27.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox27.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox27.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot27Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox27.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 27 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot27Amount) + " | Variation: " + (MemLib.ReadInt(slot27Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 27 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot27Amount) + " | Variation: " + (MemLib.ReadInt(slot27Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot27Item);
                            infoAmount = MemLib.ReadInt(slot27Amount);
                            infoVariant = MemLib.ReadInt(slot27Variation);
                        }
                    }
                    if (itemSlot == 28 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot28Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("432", NumberStyles.Integer)).ToString("X");
                        string slot28Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("436", NumberStyles.Integer)).ToString("X");
                        string slot28Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("440", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot28Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot28Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot28Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot28Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot28Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot28Amount, "int", (MemLib.ReadUInt(slot28Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot28Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot28Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot28"))
                                {
                                    ExportPlayerItems.Add("itemSlot28-ID", MemLib.ReadInt(slot28Item));
                                    ExportPlayerItems.Add("itemSlot28-Amount", MemLib.ReadInt(slot28Amount));
                                    ExportPlayerItems.Add("itemSlot28-Variation", MemLib.ReadInt(slot28Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot28Item);
                                variation = MemLib.ReadInt(slot28Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox28.Image = null;
                                }
                                else if (pictureBox28.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox28.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox28.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox28.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot28Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox28.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox28.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox28.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox28.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot28Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox28.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 28 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot28Amount) + " | Variation: " + (MemLib.ReadInt(slot28Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 28 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot28Amount) + " | Variation: " + (MemLib.ReadInt(slot28Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot28Item);
                            infoAmount = MemLib.ReadInt(slot28Amount);
                            infoVariant = MemLib.ReadInt(slot28Variation);
                        }
                    }
                    if (itemSlot == 29 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot29Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("448", NumberStyles.Integer)).ToString("X");
                        string slot29Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("452", NumberStyles.Integer)).ToString("X");
                        string slot29Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("456", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot29Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot29Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot29Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot29Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot29Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot29Amount, "int", (MemLib.ReadUInt(slot29Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot29Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot29Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot29"))
                                {
                                    ExportPlayerItems.Add("itemSlot29-ID", MemLib.ReadInt(slot29Item));
                                    ExportPlayerItems.Add("itemSlot29-Amount", MemLib.ReadInt(slot29Amount));
                                    ExportPlayerItems.Add("itemSlot29-Variation", MemLib.ReadInt(slot29Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot29Item);
                                variation = MemLib.ReadInt(slot29Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox29.Image = null;
                                }
                                else if (pictureBox29.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox29.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox29.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox29.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot29Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox29.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox29.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox29.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox29.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot29Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox29.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 29 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot29Amount) + " | Variation: " + (MemLib.ReadInt(slot29Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 29 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot29Amount) + " | Variation: " + (MemLib.ReadInt(slot29Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot29Item);
                            infoAmount = MemLib.ReadInt(slot29Amount);
                            infoVariant = MemLib.ReadInt(slot29Variation);
                        }
                    }
                    if (itemSlot == 30 || loadInventory || CycleAll || ExportInventory)
                    {
                        string slot30Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("464", NumberStyles.Integer)).ToString("X");
                        string slot30Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("468", NumberStyles.Integer)).ToString("X");
                        string slot30Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("472", NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        progressBar2.PerformStep();

                        // Set Values
                        if (!loadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slot30Item, "int", type.ToString()); // Write item type
                            if (type == 0)
                            {
                                MemLib.WriteMemory(slot30Amount, "int", "0"); // Write item amount
                                MemLib.WriteMemory(slot30Variation, "int", "0"); // Write item variation
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slot30Amount, "int", amount.ToString()); // Write item amount
                                    MemLib.WriteMemory(slot30Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slot30Amount, "int", (MemLib.ReadUInt(slot30Amount) + amount).ToString()); // Write item amount
                                    MemLib.WriteMemory(slot30Variation, "int", variation.ToString()); // Write item variation
                                    finalItemAmount = (int)MemLib.ReadUInt(slot30Amount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey("itemSlot30"))
                                {
                                    ExportPlayerItems.Add("itemSlot30-ID", MemLib.ReadInt(slot30Item));
                                    ExportPlayerItems.Add("itemSlot30-Amount", MemLib.ReadInt(slot30Amount));
                                    ExportPlayerItems.Add("itemSlot30-Variation", MemLib.ReadInt(slot30Variation));
                                }
                            }
                            else
                            {
                                // First Load
                                type = MemLib.ReadInt(slot30Item);
                                variation = MemLib.ReadInt(slot30Variation);

                                // Load Picture
                                // Set image to null if type is zero.
                                if (type.ToString() == "0")
                                {
                                    pictureBox30.Image = null;
                                }
                                else if (pictureBox30.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        pictureBox30.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation - 80068096).ToString()))); // Check if file matches current type, set it.
                                        pictureBox30.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox30.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot30Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox30.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception)
                                    {
                                        pictureBox30.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox30.SizeMode = PictureBoxSizeMode.CenterImage;

                                        // Draw item amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(pictureBox30.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slot30Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        pictureBox30.Invalidate(); // Reload picturebox.

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 30 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot30Amount) + " | Variation: " + (MemLib.ReadInt(slot30Variation) - 80068096))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 30 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot30Amount) + " | Variation: " + (MemLib.ReadInt(slot30Variation) - 80068096) + Environment.NewLine); // Record the midding values.
                                        }
                                    }
                                }
                            }
                        }

                        // Do some textbox scrolling.
                        richTextBox3.ScrollToCaret();

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slot30Item);
                            infoAmount = MemLib.ReadInt(slot30Amount);
                            infoVariant = MemLib.ReadInt(slot30Variation);
                        }
                    }
                }
                catch (Exception)
                {
                    continue;
                }

                #endregion
            }

            // Save the player Json.
            if (ExportInventory)
            {
                string playerItems = JsonConvert.SerializeObject(ExportPlayerItems, Formatting.Indented);
                System.IO.File.WriteAllText(ExportPlayerName, playerItems);
            }

            // Ammounce the information.
            if (GetItemInfo)
            {
                // Tweak amount for item variant;
                if (infoVariant != 0)
                {
                    infoVariant -= 80068096;
                }

                // Display messagebox.
                MessageBox.Show("Inventory Slot " + itemSlot + "'s Item Info: " + Environment.NewLine + Environment.NewLine + "ID: " + infoType + Environment.NewLine + "Amount: " + infoAmount + Environment.NewLine + "Variant: " + infoVariant, "Item Information:", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            #endregion

            #region Load Pictures Upon Editing

            // Define varible for addtoempty.
            bool emptySlotFound = false;

            // Load Picture
            if (!loadInventory && !GetItemInfo && (itemSlot == 1 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox1.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 1, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox1.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox1.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox1.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox1.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox1.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox1.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox1.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox1.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 2 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox2.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 2, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox2.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox2.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox2.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox2.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox2.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox2.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox2.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox2.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox2.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 3 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox3.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 3, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox3.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox3.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox3.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox3.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox3.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox3.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox3.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox3.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox3.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 4 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox4.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 4, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox4.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox4.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox4.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox4.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox4.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox4.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox4.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox4.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox4.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 5 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox5.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 5, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox5.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox5.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox5.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox5.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox5.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox5.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox5.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox5.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox5.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 6 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox6.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 6, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox6.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox6.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox6.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox6.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox6.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox6.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox6.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox6.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox6.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 7 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox7.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 7, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox7.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox7.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox7.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox7.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox7.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox7.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox7.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox7.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox7.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 8 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox8.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 8, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox8.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox8.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox8.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox8.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox8.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox8.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox8.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox8.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox8.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 9 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox9.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 9, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox9.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox9.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox9.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox9.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox9.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox9.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox9.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox9.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox9.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 10 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox10.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 10, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox10.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox10.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox10.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox10.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox10.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox10.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox10.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox10.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox10.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 11 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox11.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 11, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox11.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox11.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox11.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox11.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox11.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox11.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox11.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox11.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox11.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 12 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox12.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 12, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox12.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox12.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox12.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox12.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox12.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox12.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox12.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox12.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox12.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 13 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox13.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 13, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox13.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox13.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox13.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox13.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox13.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox13.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox13.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox13.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox13.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 14 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox14.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 14, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox14.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox14.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox14.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox14.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox14.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox14.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox14.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox14.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox14.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 15 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox15.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 15, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox15.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox15.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox15.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox15.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox15.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox15.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox15.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox15.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox15.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 16 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox16.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 16, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox16.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox16.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox16.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox16.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox16.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox16.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox16.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox16.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox16.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 17 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox17.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 17, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox17.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox17.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox17.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox17.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox17.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox17.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox17.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox17.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox17.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 18 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox18.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 18, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox18.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox18.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox18.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox18.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox18.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox18.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox18.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox18.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox18.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 19 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox19.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 19, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox19.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox19.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox19.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox19.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox19.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox19.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox19.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox19.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox19.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 20 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox20.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 20, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox20.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox20.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox20.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox20.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox20.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox20.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox20.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox20.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox20.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 21 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox21.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 21, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox21.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox21.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox21.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox21.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox21.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox21.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox21.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox21.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox21.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 22 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox22.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 22, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox22.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox22.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox22.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox22.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox22.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox22.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox22.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox22.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox22.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 23 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox23.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 23, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox23.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox23.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox23.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox23.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox23.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox23.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox23.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox23.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox23.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 24 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox24.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 24, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox24.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox24.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox24.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox24.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox24.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox24.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox24.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox24.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox24.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 25 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox25.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 25, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox25.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox25.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox25.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox25.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox25.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox25.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox25.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox25.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox25.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 26 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox26.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 26, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox26.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox26.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox26.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox26.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox26.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox26.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox26.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox26.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox26.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 27 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox27.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 27, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox27.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox27.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox27.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox27.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox27.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox27.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox27.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox27.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox27.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 28 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox28.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 28, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox28.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox28.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox28.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox28.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox28.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox28.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox28.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox28.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox28.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 29 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox29.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 29, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox29.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox29.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox29.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox29.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox29.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox29.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox29.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox29.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox29.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 30 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox30.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 30, type: type, amount: amount, variation: variation);

                    // Update bool.
                    emptySlotFound = true;
                }
                else if (!AddToEmpty)
                {
                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set image to null if type is zero.
                    if (type.ToString() == "0")
                    {
                        pictureBox30.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            pictureBox30.Image = new Bitmap(Image.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()))); // Check if file matches current type, set it.
                            pictureBox30.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox30.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox30.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                            pictureBox30.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                            pictureBox30.SizeMode = PictureBoxSizeMode.CenterImage;

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox30.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox30.Invalidate(); // Reload picturebox.
                        }
                    }
                }
            }

            #endregion

            // Check if there was no slots empty.
            if (AddToEmpty && !emptySlotFound)
            {
                MessageBox.Show("Your inventoy is full!" + Environment.NewLine + Environment.NewLine + "Try using the reload inventory button.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Rename button back to defualt.
            button1.Text = "Get Inventory Addresses";
            button2.Text = "Reload Inventory";
            button3.Text = "Remove All";

            // Rehide the progressbar.
            progressBar2.Visible = false;
        }

        // Reload Inventory.
        private void button2_Click(object sender, EventArgs e)
        {
            // Change button labels.
            button2.Text = "Loading..";

            // Reset progress bar.
            progressBar2.Value = 0;
            progressBar2.Visible = true;

            // Reload Inventory.
            AddItemToInv(loadInventory: true);
        }

        // Remove entire Inventory.
        private void button3_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete ALL items? Are you sure?", "Remove All", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                // Change button labels.
                button3.Text = "Clearing..";

                // Reload Inventory.
                AddItemToInv(type: 0, amount: 1, variation: 0, CycleAll: true);
            }
        }

        #region Click Events

        // Click Events
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            PictureBox pic = sender as PictureBox;

            // Ensure picturebox control exists.
            if (pic != null)
            {
                if (e.Button == MouseButtons.Left) // Load inventory editor.
                {
                    // Get the picturebox selected number.
                    int slotNumber = int.Parse(pic.Name.Replace("pictureBox", ""));

                    // Spawn item picker window.
                    InventoryEditor frm2 = new InventoryEditor();
                    DialogResult dr = frm2.ShowDialog(this);

                    // Get returned item from picker.
                    int itemType = frm2.GetItemTypeFromList();
                    int itemAmount = frm2.GetItemAmountFromList();
                    int itemVariation = frm2.GetItemVeriationFromList() == 0 ? 0 : (frm2.GetItemVeriationFromList() + 80068096); // If variation is not zero, add offset.
                    bool wasAborted = frm2.GetUserCancledTask();
                    bool itemOverwrite = frm2.GetSelectedOverwriteTask();
                    frm2.Close();

                    // Check if user closed the form
                    if (wasAborted) { return; };

                    // Spawn the item.
                    AddItemToInv(slotNumber, type: itemType, amount: itemAmount, variation: itemVariation, Overwrite: itemOverwrite);
                }
                else if (e.Button == MouseButtons.Right) // Get item stats.
                {
                    // Get the picturebox selected number.
                    int slotNumber = int.Parse(pic.Name.Replace("pictureBox", ""));

                    // Get item stats.
                    AddItemToInv(slotNumber, GetItemInfo: true);
                }
            }
        }

        #endregion // Click Events

        #endregion // End Inventory Region

        #region Misc

        // Change player name.
        private async void button4_Click(object sender, EventArgs e)
        {
            // Ensure properties are filled.
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                // Display error message.
                MessageBox.Show("Your must type your playername and a new name!!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //return;
            }

            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Name button to indicate loading.
            button4.Text = "Changing Name..";

            // Get current player name.
            StringBuilder builder = new StringBuilder();
            foreach (char c in textBox1.Text)
            {
                builder.Append(Convert.ToInt32(c).ToString("X"));
            }

            // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
            AoBScanResultsPlayerName = await MemLib.AoBScan(string.Join(string.Empty, builder.ToString().Select((x, i) => i > 0 && i % 2 == 0 ? string.Format(" {0}", x) : x.ToString())), true, true);

            // If the count is zero, the scan had an error.
            if (AoBScanResultsPlayerName.Count() == 0 | AoBScanResultsPlayerName.Count() < 10)
            {
                // Rename button back to defualt.
                button4.Text = "Change Name";

                // Display error message.
                MessageBox.Show("Your name must be of your current in-game player!!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Change players name.
            ChangePlayersName(textBox2.Text, textBox2.Text.Length, textBox2.Text.Length);
        }

        // this function is async, which means it does not block other code.
        public void ChangePlayersName(string NewName, int OldLengh, int NewLengh)
        {
            // Iterate through each found address.
            foreach (long res in AoBScanResultsPlayerName)
            {
                // Get address from loop.
                string baseAddress = res.ToString("X").ToString();
                string nameLenghAddress = BigInteger.Subtract(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("2", NumberStyles.Integer)).ToString("X");

                // Write new name lengh.
                MemLib.WriteMemory(nameLenghAddress, "int", NewLengh.ToString());

                // Write new name to addresses.
                MemLib.WriteMemory(baseAddress, "string", NewName);
            }

            // Process completed, run finishing tasks.
            // Rename button back to defualt.
            button4.Text = "Change Name";
            textBox1.Text = NewName;
            textBox2.Text = "";
        }

        // Import a player file.
        private void button5_Click(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsInventory == null)
            {
                // Rename button back to defualt.
                button1.Text = "Get Inventory Addresses";
                button2.Text = "Reload Inventory";
                button3.Text = "Remove All";

                MessageBox.Show("You need to first scan for the Inventory addresses!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get file from browser.
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Player File|*.ckplayer";

            // Ensure the user chose a file.
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // Reset progress bar.
                progressBar1.Step = 90;
                progressBar1.Value = 0;

                // Define playername
                string playerName = ofd.SafeFileName;

                // Define count for inventory slots.
                int ItemSlotCount = 1;

                try
                {
                    // Read the json file.
                    using (FileStream fileStream = new FileStream(ofd.FileName, FileMode.Open, FileAccess.Read))
                    using (StreamReader fileReader = new StreamReader(fileStream))
                    using (JsonTextReader reader = new JsonTextReader(fileReader))
                    {
                        while (reader.Read())
                        {
                            if (reader.TokenType == JsonToken.StartObject)
                            {
                                // Load each object from the stream.
                                JObject playerData = JObject.Load(reader);

                                // Define some vars.
                                int itemID = 0;
                                int itemAmount = 0;
                                int itemVariation = 0;

                                // Loop through each item in object.
                                foreach (var ex in playerData)
                                {
                                    // Convert json items to vars.
                                    // Get item id.
                                    if (ex.Key.Contains("-ID"))
                                    {
                                        // Get item id.
                                        string slotNumberID = "itemSlot" + ItemSlotCount.ToString() + "-ID";
                                        itemID = int.Parse(playerData[slotNumberID].ToString().Replace("itemSlot", "").Replace("-ID", ""));

                                        // Advance the protgress bar.
                                        progressBar1.PerformStep();
                                    }
                                    else if (ex.Key.Contains("-Amount"))
                                    {
                                        // Get item amount.
                                        string slotNumberAmount = "itemSlot" + ItemSlotCount.ToString() + "-Amount";
                                        itemAmount = int.Parse(playerData[slotNumberAmount].ToString().Replace("itemSlot", "").Replace("-Amount", ""));

                                        // Advance the protgress bar.
                                        progressBar1.PerformStep();
                                    }
                                    else if (ex.Key.Contains("-Variation"))
                                    {
                                        // Get item amount.
                                        string slotNumberAmount = "itemSlot" + ItemSlotCount.ToString() + "-Variation";
                                        itemVariation = int.Parse(playerData[slotNumberAmount].ToString().Replace("itemSlot", "").Replace("-Variation", ""));

                                        // Advance the protgress bar.
                                        progressBar1.PerformStep();

                                        // Add the item to the inventory.
                                        if (itemVariation == 0)
                                        {
                                            AddItemToInv(itemSlot: ItemSlotCount, type: itemID, amount: itemAmount, variation: itemVariation, Overwrite: true);
                                        }
                                        else
                                        {
                                            AddItemToInv(itemSlot: ItemSlotCount, type: itemID, amount: itemAmount, variation: itemVariation + 80068096, Overwrite: true);
                                        }

                                        // Add one to the loopcount.
                                        ItemSlotCount++;
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show("There was an error reading this file!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Ensure progressbar is at 100.
                progressBar1.Value = 100;
            }
        }

        // Export a player file.
        private void button6_Click(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsInventory == null)
            {
                // Rename button back to defualt.
                button1.Text = "Get Inventory Addresses";
                button2.Text = "Reload Inventory";
                button3.Text = "Remove All";

                MessageBox.Show("You need to first scan for the Inventory addresses!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Export the inventory.
            AddItemToInv(ExportInventory: true);

            // Advance progress bar.
            progressBar1.Value = 100;
        }

        #endregion // End misc tab.

        #region ItemChatCommands

        // Enable the numericupdown based on if selected radiobutton is checked.
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked == true)
            {
                // Disable
                numericUpDown1.Enabled = true;
            }
            else
            {
                // Enable
                numericUpDown1.Enabled = false;
            }
        }

        // Toggle chat commands.
        bool chatEnabled = false;
        public System.Timers.Timer chatTimer = new System.Timers.Timer(500);
        private async void button7_Click(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsInventory == null)
            {
                MessageBox.Show("You need to first scan for the Inventory addresses!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if the chat button is enabled or disabled.
            if (chatEnabled)
            {
                // Disable / reset some controls.
                chatEnabled = false;
                button7.Text = "Enable";
                button7.Enabled = true;
                progressBar3.Value = 0;
                chatTimer.Stop();
                chatTimer.Enabled = false;
            }
            else
            {
                // Enable some controls.
                chatEnabled = true;

                // Name button to indicate loading.
                progressBar3.Value = 10;
                button7.Text = "Loading..";
                button7.Enabled = false;

                // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
                AoBScanResultsChat = await MemLib.AoBScan("2F 69 74 65 6D", true, true);

                // If the count is zero, the scan had an error.
                if (AoBScanResultsChat.Count() == 0 | AoBScanResultsChat.Count() < 1)
                {
                    // Disable / reset some controls.
                    chatEnabled = false;
                    button7.Text = "Enable";
                    button7.Enabled = true;
                    progressBar3.Value = 0;
                    chatTimer.Stop();
                    chatTimer.Enabled = false;

                    // Display error message.
                    MessageBox.Show("You must type \"/item\" in the player chat first!!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Name button to indicate loading finished.
                button7.Text = "Disable";
                button7.Enabled = true;

                // Reset richtextbox.
                richTextBox4.Text = "Any captured chat messages will appear here." + Environment.NewLine + "------------------------------------------------------------------------------------------------------------" + Environment.NewLine;

                // Advance progress bar.
                progressBar3.Value = 100;

                // Enable a timer.
                chatTimer.AutoReset = true;
                chatTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
                chatTimer.Start();
            }
        }

        // Do events for the chat.
        bool firstRun = true; // Do text reset bool.
        bool firstItem = true; // Ensure we only add to one slot.
        private void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            // Iterate through each found address.
            foreach (long res in AoBScanResultsChat)
            {
                // Get address from loop.
                string baseAddress = res.ToString("X").ToString();

                // Get address value.
                string currentCommand = MemLib.ReadString(baseAddress);

                try
                {
                    // Check if current value is valid command and it's unique.
                    if (currentCommand.Split(' ')[0] == "/item")
                    {
                        // Erase current chat values.
                        MemLib.WriteMemory(baseAddress, "string", "                                ");

                        // Update last chat command
                        LastChatCommand.Add(currentCommand);

                        // Log command if it does not exist.
                        if (currentCommand != richTextBox4.Lines[richTextBox4.Lines.Length - 1] && firstRun)
                        {
                            // Prevent further entries this loop.
                            firstRun = false;

                            richTextBox4.AppendText(currentCommand + Environment.NewLine);
                            richTextBox4.ScrollToCaret();
                        }

                        string itemName = currentCommand.Split(' ')[1];
                        string itemAmount = currentCommand.Split(' ')[2];
                        string itemVariation = "";

                        try
                        {
                            // With item variation.
                            itemVariation = currentCommand.Split(' ')[3];

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
                                        if (filenameData[0].ToLower().Contains(itemName.Replace(" ", "")))
                                        {
                                            // Check if to overwrite or to add to empty slots.
                                            if (radioButton1.Checked) // Overwrite slot1.
                                            {
                                                AddItemToInv(itemSlot: 1, type: int.Parse(filenameData[1]), amount: int.Parse(itemAmount), variation: int.Parse(itemVariation) == 0 ? 0 : (int.Parse(itemVariation) + 80068096), Overwrite: true);
                                            }
                                            else if (radioButton2.Checked) // Add item to an empty slot.
                                            {
                                                // Reload inventory if add to empty is checked.
                                                if (radioButton2.Checked && firstItem)
                                                {
                                                    // Mark item as first.
                                                    firstItem = false;

                                                    AddItemToInv(AddToEmpty: true, type: int.Parse(filenameData[1]), amount: int.Parse(itemAmount), variation: int.Parse(itemVariation) == 0 ? 0 : (int.Parse(itemVariation) + 80068096), Overwrite: true);
                                                }
                                            }
                                            else if (radioButton3.Checked) // Custom slot.
                                            {
                                                AddItemToInv(itemSlot: (int)numericUpDown1.Value, type: int.Parse(filenameData[1]), amount: int.Parse(itemAmount), variation: int.Parse(itemVariation) == 0 ? 0 : (int.Parse(itemVariation) + 80068096), Overwrite: true);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            // Without item variation.
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
                                        if (filenameData[0].ToLower().Contains(itemName.Replace(" ", "")))
                                        {
                                            // Check if to overwrite or to add to empty slots.
                                            if (radioButton1.Checked) // Overwrite slot1.
                                            {
                                                AddItemToInv(itemSlot: 1, type: int.Parse(filenameData[1]), amount: int.Parse(itemAmount), Overwrite: true);
                                            }
                                            else if (radioButton2.Checked) // Add item to an empty slot.
                                            {
                                                // Reload inventory if add to empty is checked.
                                                if (radioButton2.Checked && firstItem)
                                                {
                                                    // Mark item as first.
                                                    firstItem = false;

                                                    AddItemToInv(AddToEmpty: true, type: int.Parse(filenameData[1]), amount: int.Parse(itemAmount), Overwrite: true);
                                                }
                                            }
                                            else if (radioButton3.Checked) // Custom slot.
                                            {
                                                AddItemToInv(itemSlot: (int)numericUpDown1.Value, type: int.Parse(filenameData[1]), amount: int.Parse(itemAmount), Overwrite: true);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }

            // Reset the loop checks.
            firstRun = true;
            firstItem = true;
        }

        #endregion

        #region Admin Tools

        // Remove duplicates.
        private void button10_Click(object sender, EventArgs e)
        {
            // Create directories if they do not exist.
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\"))
            {
                // Create directory.
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\");
            }
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\images"))
            {
                // Create directory.
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\images");
            }
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\out"))
            {
                // Create directory.
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\out");
            }

            // Delete files in out if they exist.
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\log.txt"))
            {
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\log.txt");
            }

            // Check if images exist within the directory.
            if (Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\images", "*.png").Length == 0 || !File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\data.txt"))
            {
                MessageBox.Show("Missing images or data files within the debug directory.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                // Define count for file deletion.
                int imageRemovalCount = 0;

                // Get each file in the directory.
                FileInfo[] Files = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\images").GetFiles("*.png");

                // Get each image file in directory.
                foreach (FileInfo file in Files)
                {
                    // Get each png file from the inventory subdirectories.
                    var file123 = Directory.GetFileSystemEntries(AppDomain.CurrentDomain.BaseDirectory + @"assets\Inventory\", "*.png", SearchOption.AllDirectories);
                    foreach (string image in file123)
                    {
                        // Check if image from debug exists within inventory.
                        if (file.FullName.Substring(file.FullName.LastIndexOf('\\') + 1).Replace("_", "").Split('.')[0].ToLower() == image.Substring(image.LastIndexOf('\\') + 1).Split(',')[0].ToLower())
                        {
                            try
                            {
                                // Match was found, log it.
                                string imageName = file.FullName.Substring(file.FullName.LastIndexOf('\\') + 1).Replace("_", "").Split('.')[0];
                                File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\log.txt", "Removed File: " + imageName + ".png" + Environment.NewLine);

                                // Remove image from debug.
                                File.Delete(file.FullName);

                                // Advance removal counter.
                                imageRemovalCount++;
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }

                // Display results.
                MessageBox.Show(imageRemovalCount.ToString() + " images where found and deleted.", "Removed Dupes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Rename images based off data.txt.
        private void button8_Click(object sender, EventArgs e)
        {
            // Create directories if they do not exist.
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\"))
            {
                // Create directory.
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\");
            }
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\images"))
            {
                // Create directory.
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\images");
            }
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\out"))
            {
                // Create directory.
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\out");
            }

            // Delete files in out if they exist.
            if (Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\out", "*.png").Length != 0)
            {
                Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\out").ToList().ForEach(File.Delete);
            }
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\log.txt"))
            {
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\log.txt");
            }

            // Define counter for total items.
            int renamedImagesCount = 0;

            // Check if images exist within the directory.
            if (Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\images", "*.png").Length == 0 || !File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\data.txt"))
            {
                MessageBox.Show("Missing images or data files within the debug directory.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                // Get each file in the directory.
                FileInfo[] Files = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\images").GetFiles("*.png");

                // Read each line in the text file.
                foreach (var line in File.ReadLines(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\data.txt"))
                {
                    // Define image name from file.
                    string imageNameFromFile = line.Split(',')[0].Replace(" ", "");

                    // Get each image file in directory.
                    foreach (FileInfo file in Files)
                    {
                        // Define image name.
                        string imageName = file.Name.Split('.')[0].Replace("_", "");

                        // Check if file contains the recorded data.
                        if (imageName.ToLower() == imageNameFromFile.ToLower())
                        {
                            try
                            {
                                // Define new filename.
                                string newImageName = imageNameFromFile + "," + line.Split(',')[1] + ",0.png";

                                // Match was found, log it.
                                File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\log.txt", file.Name + " -> " + newImageName + Environment.NewLine);

                                // Copy and save the rename image file.
                                File.Copy(file.FullName, AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\out\" + newImageName);

                                // Add to count.
                                renamedImagesCount++;
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                    }
                }

                // Display results.
                MessageBox.Show(renamedImagesCount.ToString() + " images where found and renamed.", "Rename Images", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Cycle Items
        private async void button9_Click(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsInventory == null)
            {
                MessageBox.Show("You need to first scan for the Inventory addresses!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Delete files in out if they exist.
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\log.txt"))
            {
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\log.txt");
            }

            // Get address from loop.
            string baseAddress = AoBScanResultsInventory.Last().ToString("X").ToString();
            string slot1Item = baseAddress;

            // Test 
            for (int testValue = 0; testValue < 32767 + 1; testValue++)
            {
                // Add New Item
                MemLib.WriteMemory(slot1Item, "int", testValue.ToString()); // Write item type

                // Add Delay
                await System.Threading.Tasks.Task.Delay(100);

                // Check if new item reads negitive.
                int itemByteValue = (int)MemLib.ReadByte(slot1Item);
                if (itemByteValue < 0)
                {
                    // Log items.
                    File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\log.txt", "Invalid: ItemID: " + testValue + " Result: " + itemByteValue + Environment.NewLine);

                    // MessageBox.Show("Invalid: ItemID: " + testValue + " Result: " + itemByteValue, "Cycle Items", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    // Log items.
                    File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\log.txt", itemByteValue.ToString() + Environment.NewLine);

                    // MessageBox.Show(itemByteValue.ToString(), "Cycle Items", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            // Finished
            MessageBox.Show("Finished cycling items.", "Cycle Items", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion
    }
}