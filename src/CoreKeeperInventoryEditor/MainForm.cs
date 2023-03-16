using CoreKeepersWorkshop;
using Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace CoreKeeperInventoryEditor
{
    public partial class MainForm : Form
    {
        // Form initialization.
        public MainForm()
        {
            InitializeComponent();
        }

        #region Variables

        // Setup some variables.
        public Mem MemLib = new Mem();
        public IEnumerable<long> AoBScanResultsInventory;
        public IEnumerable<long> AoBScanResultsPlayerName;
        public IEnumerable<long> AoBScanResultsChat;
        public IEnumerable<long> AoBScanResultsGroundItems;
        public IEnumerable<long> AoBScanResultsPlayerTools;
        public IEnumerable<long> AoBScanResultsPlayerLocation;
        public IEnumerable<long> AoBScanResultsPlayerBuffs;
        public IEnumerable<long> AoBScanResultsTeleportData;
        public IEnumerable<long> AoBScanResultsFishingData;
        public IEnumerable<long> AoBScanResultsDevMapReveal;
        public IEnumerable<long> AoBScanResultsRevealMapRange;
        public List<string> LastChatCommand = new List<string>() { "" };
        public Dictionary<string, int> ExportPlayerItems = new Dictionary<string, int> { };
        public string ExportPlayerName = "";
        public bool isMinimized = false;
        public int useAddress = 1;

        // Define texture data.
        public IEnumerable<string> ImageFiles1 = Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"assets\Inventory\") && Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"assets\Inventory\", "*.png", SearchOption.AllDirectories) != null ? Directory.GetFileSystemEntries(AppDomain.CurrentDomain.BaseDirectory + @"assets\Inventory\", "*.png", SearchOption.AllDirectories) : new String[] { "" }; // Ensure directory exists and images exist. Fix: v1.2.9.
        public IEnumerable<string> InventorySkins = Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"assets\backgrounds\Inventory") && Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"assets\backgrounds\Inventory", "*.png", SearchOption.AllDirectories) != null ? Directory.GetFileSystemEntries(AppDomain.CurrentDomain.BaseDirectory + @"assets\backgrounds\Inventory", "*.png", SearchOption.AllDirectories) : new String[] { "" }; // Ensure directory exists and images exist. Fix: v1.2.9.
        public IEnumerable<string> PlayerSkins = Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"assets\backgrounds\Player") && Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"assets\backgrounds\Player", "*.png", SearchOption.AllDirectories) != null ? Directory.GetFileSystemEntries(AppDomain.CurrentDomain.BaseDirectory + @"assets\backgrounds\Player", "*.png", SearchOption.AllDirectories) : new String[] { "" }; // Ensure directory exists and images exist. Fix: v1.2.9.
        public IEnumerable<string> WorldSkins = Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"assets\backgrounds\World") && Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"assets\backgrounds\World", "*.png", SearchOption.AllDirectories) != null ? Directory.GetFileSystemEntries(AppDomain.CurrentDomain.BaseDirectory + @"assets\backgrounds\World", "*.png", SearchOption.AllDirectories) : new String[] { "" }; // Ensure directory exists and images exist. Fix: v1.2.9.
        public IEnumerable<string> ChatSkins = Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"assets\backgrounds\Chat") && Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"assets\backgrounds\Chat", "*.png", SearchOption.AllDirectories) != null ? Directory.GetFileSystemEntries(AppDomain.CurrentDomain.BaseDirectory + @"assets\backgrounds\Chat", "*.png", SearchOption.AllDirectories) : new String[] { "" }; // Ensure directory exists and images exist. Fix: v1.2.9.

        // Define skin counters.
        public int inventorySkinCounter = CoreKeepersWorkshop.Properties.Settings.Default.InventoryBackgroundCount;
        public int playerSkinCounter = CoreKeepersWorkshop.Properties.Settings.Default.PlayerBackgroundCount;
        public int worldSkinCounter = CoreKeepersWorkshop.Properties.Settings.Default.WorldBackgroundCount;
        public int chatSkinCounter = CoreKeepersWorkshop.Properties.Settings.Default.ChatBackgroundCount;

        // Set the mouse event class.
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        private const int MOUSEEVENT_LEFTDOWN = 0x02;
        private const int MOUSEEVENT_LEFTUP = 0x04;
        private const int MOUSEEVENT_MIDDLEDOWN = 0x20;
        private const int MOUSEEVENT_MIDDLEUP = 0x40;
        private const int MOUSEEVENT_RIGHTDOWN = 0x08;
        private const int MOUSEEVENT_RIGHTUP = 0x10;

        #region Proccess Handle Classes

        // Set the process handle resize class.
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, int uFlags);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner
        }

        private const int SWP_NOSIZE = 0x0001;
        private const int SWP_NOZORDER = 0x0004;
        private const int SWP_SHOWWINDOW = 0x0040;

        #endregion

        #endregion // End variables.

        #region Form Controls

        // Do form loading events.
        private void MainForm_Load(object sender, EventArgs e)
        {
            try // Further catch possible errors.
            {
                #region Set Custom Cusror

                // Set the applications cursor.
                Cursor = new Cursor(CoreKeepersWorkshop.Properties.Resources.UICursor.GetHicon());
                #endregion

                #region Set Controls

                // Set the about tabs content.
                // About.
                richTextBox2.Text = String.Concat(new string[] {
                @"// CoreKeepersWorkshop v" + FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion + " - Written kindly by: D.RUSS#2430" + Environment.NewLine,
                @"-------------------------------------------------------------------------------------------------------------------" + Environment.NewLine,
                @"This tool was created with future content and modded content in mind. It currently supports manual item additions by naming images using the following format: ItemName,ItemID,ItemVariation.png - You can add these assets to the ""\assets\inventory\"" directory. For future requests or any issues, please contact me under my discord handle above, thanks!" + Environment.NewLine,
                @"-------------------------------------------------------------------------------------------------------------------" + Environment.NewLine,
                @"Project source: https://github.com/RussDev7/CoreKeepersWorkshop"
                });

                // Honorable mentions.
                richTextBox8.Text = String.Concat(new string[] {
                @"// Here we give thanks to those who have helped the project grow!" + Environment.NewLine,
                @"// This project would never have grown if not for the following:" + Environment.NewLine + Environment.NewLine,

                @"1) ultimaton2   - Most helpful debugger in the projects lifetime." + Environment.NewLine,
                @"2) ZeroGravitas - Helped get food tested!" + Environment.NewLine,
                @"3) Roupiks      - Created assets for all the tabs!" + Environment.NewLine + Environment.NewLine,

                @"Honorable Mentions:" + Environment.NewLine,
                @"BourbonCrow, puxxy5layer, Flux, pharuxtan, Iskrownik, Yumiko Abe, Ice, Kremnev8",
                });
                #endregion

                #region Set Dev-Tool / Main Control Contents

                // Main controls.
                numericUpDown14.Value = CoreKeepersWorkshop.Properties.Settings.Default.MapRenderingMax; // Map rendering max radius.
                numericUpDown16.Value = CoreKeepersWorkshop.Properties.Settings.Default.MapRenderingStart; // Map rendering start radius.
                numericUpDown19.Value = CoreKeepersWorkshop.Properties.Settings.Default.FishingCast; // Fishing bot casting delay.
                numericUpDown20.Value = CoreKeepersWorkshop.Properties.Settings.Default.FishingPadding; // Fishing bot padding delay.

                // Dev tools.
                numericUpDown2.Value = (decimal)CoreKeepersWorkshop.Properties.Settings.Default.DevToolDelay; // Dev tool operation delay.
                numericUpDown18.Value = CoreKeepersWorkshop.Properties.Settings.Default.RadialMoveScale; // Auto render maps radialMoveScale.
                checkBox2.Checked = CoreKeepersWorkshop.Properties.Settings.Default.TopMost; // Set as top most.
                #endregion

                #region Set Form Locations

                // Set the forms active location based on previous save.
                MainForm.ActiveForm.Location = CoreKeepersWorkshop.Properties.Settings.Default.MainFormLocation;
                #endregion

                #region Set Background

                // Get background from saved settings.
                if (CoreKeepersWorkshop.Properties.Settings.Default.InventoryBackground != "") // Ensure background is not null.
                {
                    // Catch image missing / renamed errors.
                    try
                    {
                        tabControl1.TabPages[0].BackgroundImage = ImageFast.FromFile(CoreKeepersWorkshop.Properties.Settings.Default.InventoryBackground);
                    }
                    catch (Exception)
                    {
                        CoreKeepersWorkshop.Properties.Settings.Default.InventoryBackground = "";
                    }
                }
                if (CoreKeepersWorkshop.Properties.Settings.Default.PlayerBackground != "") // Ensure background is not null.
                {
                    // Catch image missing / renamed errors.
                    try
                    {
                        tabControl1.TabPages[1].BackgroundImage = ImageFast.FromFile(CoreKeepersWorkshop.Properties.Settings.Default.PlayerBackground);
                    }
                    catch (Exception)
                    {
                        CoreKeepersWorkshop.Properties.Settings.Default.PlayerBackground = "";
                    }
                }
                if (CoreKeepersWorkshop.Properties.Settings.Default.WorldBackground != "") // Ensure background is not null.
                {
                    // Catch image missing / renamed errors.
                    try
                    {
                        tabControl1.TabPages[2].BackgroundImage = ImageFast.FromFile(CoreKeepersWorkshop.Properties.Settings.Default.WorldBackground);
                    }
                    catch (Exception)
                    {
                        CoreKeepersWorkshop.Properties.Settings.Default.WorldBackground = "";
                    }
                }
                if (CoreKeepersWorkshop.Properties.Settings.Default.ChatBackground != "") // Ensure background is not null.
                {
                    // Catch image missing / renamed errors.
                    try
                    {
                        tabControl1.TabPages[3].BackgroundImage = ImageFast.FromFile(CoreKeepersWorkshop.Properties.Settings.Default.ChatBackground);
                    }
                    catch (Exception)
                    {
                        CoreKeepersWorkshop.Properties.Settings.Default.ChatBackground = "";
                    }
                }
                #endregion

                #region Tooltips

                // Create a new tooltip.
                ToolTip toolTip = new ToolTip()
                {
                    AutoPopDelay = 5000,
                    InitialDelay = 750
                };

                // Set tool texts.
                toolTip.SetToolTip(textBox1, "Enter the existing loaded player's name.");
                toolTip.SetToolTip(textBox2, "Enter a custom name. Must match current player's name length.");
                toolTip.SetToolTip(textBox3, "Enter the name of the world you wish to load.");

                toolTip.SetToolTip(button1, "Get the required addresses for editing the inventory.");
                toolTip.SetToolTip(button2, "Reload loads the GUI with updated inventory items.");
                toolTip.SetToolTip(button3, "Remove all items from the inventory.");
                toolTip.SetToolTip(button4, "Change your existing name.");
                toolTip.SetToolTip(button5, "Import a player file to overwrite items.");
                toolTip.SetToolTip(button6, "Export a player file to overwrite items.");
                toolTip.SetToolTip(button7, "Enable / disable in-game chat commands.");
                toolTip.SetToolTip(button8, "Removes all ground items not picked up by the player.");
                toolTip.SetToolTip(button9, "Teleport the player to a desired world position.");
                toolTip.SetToolTip(button10, "Get the required addresses for using player tools.");
                toolTip.SetToolTip(button11, "Get the required addresses for using world tools.");
                toolTip.SetToolTip(button12, "Replaces the glow tulip buff with a desired buff.");
                toolTip.SetToolTip(button16, "Fills the datagridview with the world header information.");
                toolTip.SetToolTip(button17, "Change the difficutly of the current world.");
                toolTip.SetToolTip(button15, "Change the date created of the current world.");
                toolTip.SetToolTip(button18, "Change the activated crystals of the current world.");
                toolTip.SetToolTip(button19, "Automatically fishes for you. First throw reel into water.");
                toolTip.SetToolTip(button20, "Switch to the previous found inventory.");
                toolTip.SetToolTip(button21, "Switch to the next found inventory.");
                toolTip.SetToolTip(button22, "Automatically render very large areas around the player.");
                toolTip.SetToolTip(button25, "Restore the default range and disable full map brightness.");
                toolTip.SetToolTip(button27, "Turn on custom map render distance with full map brightness.");
                toolTip.SetToolTip(button28, "Cancel the map rendering operation.");
                toolTip.SetToolTip(button30, "Get the required addresses for custom map rendering.");
                toolTip.SetToolTip(button31, "Pause or resume the auto map rendering operation.");

                toolTip.SetToolTip(comboBox1, "Open a list of all ingame buffs and debuffs.");

                toolTip.SetToolTip(checkBox1, "Save the map to file after each completed ring.");
                toolTip.SetToolTip(checkBox2, "Keep this application always on top of other applications.");
                toolTip.SetToolTip(checkBox3, "Brute force the address searching for the teleport address.");

                toolTip.SetToolTip(richTextBox1, "A list of all found addresses. Used mostly for debugging.");
                toolTip.SetToolTip(richTextBox6, "A list of all found addresses. Used mostly for debugging.");

                toolTip.SetToolTip(siticoneWinToggleSwith1, "Gets the players XY coordinates and displays it.");
                toolTip.SetToolTip(siticoneWinToggleSwith2, "Enabling will prevent the player from being killed.");
                toolTip.SetToolTip(siticoneWinToggleSwith3, "Set a custom run speed for the player.");
                toolTip.SetToolTip(siticoneWinToggleSwith4, "Spacebar will allow the player to pass through walls.");
                toolTip.SetToolTip(siticoneWinToggleSwith5, "Enabling will keep the players food replenished.");
                toolTip.SetToolTip(siticoneWinToggleSwith6, "Enabling this will instantly kill the player.");
                toolTip.SetToolTip(siticoneWinToggleSwith7, "Prevents the diminishing of inventory items.");
                toolTip.SetToolTip(siticoneWinToggleSwith8, "Prevents being killed or teleported while stuck in walls. Use the t-key to toggle.");
                toolTip.SetToolTip(siticoneWinToggleSwith9, "Recalls the player to spawn immediately.");
                toolTip.SetToolTip(siticoneWinToggleSwith10, "Enables the ability to craft without consuming recourses.");

                toolTip.SetToolTip(radioButton1, "Overwrite item slot one.");
                toolTip.SetToolTip(radioButton2, "Add item to an empty inventory slot.");
                toolTip.SetToolTip(radioButton3, "Add items to a custom inventory slot.");
                toolTip.SetToolTip(radioButton4, "Normal world difficutly.");
                toolTip.SetToolTip(radioButton5, "Hard world difficutly.");

                toolTip.SetToolTip(numericUpDown1, "Change what item slot to send items too.");
                toolTip.SetToolTip(numericUpDown2, "Change the interval of dev-tools that use delays. (defualt: 80)");
                toolTip.SetToolTip(numericUpDown3, "Change the base speed the player will walk at.");
                toolTip.SetToolTip(numericUpDown4, "Change the x-axis world position to be teleported on.");
                toolTip.SetToolTip(numericUpDown5, "Change the y-axis world position to be teleported on.");
                toolTip.SetToolTip(numericUpDown6, "Change the amount of power the buff will contain.");
                toolTip.SetToolTip(numericUpDown7, "Change the amount of time the buff will be active for.");
                toolTip.SetToolTip(numericUpDown14, "The (radius x range) of tiles to render around the player.");
                toolTip.SetToolTip(numericUpDown15, "Change the cooldown time (milliseconds) before the next teleport.");
                toolTip.SetToolTip(numericUpDown16, "Set the mininum range in tiles away from the player to start the map render.");
                toolTip.SetToolTip(numericUpDown17, "Set the maximum range in tiles to render the map by.");
                toolTip.SetToolTip(numericUpDown18, "Set a custom radialMoveScale for auto map rendering. (defualt: 0.1)");
                toolTip.SetToolTip(numericUpDown19, "Set the delay for re-casting the fishing pole.");
                toolTip.SetToolTip(numericUpDown20, "Set the delay for loop operations. Ex: Cought fish checking.");

                toolTip.SetToolTip(label7, "Create an ID list from all installed assets.");
                toolTip.SetToolTip(label4, "Sets the variant of item slot2 based on a file list.");
                toolTip.SetToolTip(label8, "Change item slot2s variant based on the left/right arrow keys.");
                toolTip.SetToolTip(label30, "Grabs the application and sends it to the center of the screen.");
                toolTip.SetToolTip(label31, "Stores the games private bytes with timestamps for each completed rotation.");

                // toolTip.SetToolTip(dataGridView1, "Prints all the world header information.");

                #endregion
            }
            catch (Exception)
            {
            }
        }

        // Change the top most varible.
        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            // Check if the application is already set to top most or not.
            if (!checkBox2.Checked)
            {
                // Turn top most off.
                this.TopMost = false;

                // Save the property.
                CoreKeepersWorkshop.Properties.Settings.Default.TopMost = false;
            }
            else
            {
                // Turn top most on.
                this.TopMost = true;

                // Save the property.
                CoreKeepersWorkshop.Properties.Settings.Default.TopMost = true;
            }
        }

        // Populate combobox upon dropdown.
        private void ComboBox1_DropDown(object sender, EventArgs e)
        {
            if (comboBox1.Items.Count == 0)
            {
                // Get json file from resources.
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CoreKeepersWorkshop.Resources.BuffIDs.json"))
                using (StreamReader reader = new StreamReader(stream))
                {
                    // Convert stream into string.
                    var jsonFileContent = reader.ReadToEnd();
                    dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonFileContent);

                    // Load each object from json to a string array.
                    foreach (var file in result)
                    {
                        // Remove spaces from food names.
                        string buffName = (string)file.name;

                        // Add the values to the combobox if it's not empty.
                        if (buffName != "")
                        {
                            comboBox1.Items.Add((string)buffName);
                        }
                    }
                }
            }
        }

        // Launch the link in the browser.
        private void RichTextBox2_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/RussDev7/CoreKeepersWorkshop");
        }

        // Reset inventory stats back to defualts.
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Ensure we catch all closing exceptions. // Fix v1.3.3.
            try
            {
                // Save the previous form location before closing if it's not minimized.
                Rectangle activeScreenDimensions = Screen.FromControl(this).Bounds;
                if (WindowState == FormWindowState.Normal && this.Location != new Point(0, activeScreenDimensions.Height - 40) && !isMinimized) // isMinimized fix 1.3.1.
                {
                    CoreKeepersWorkshop.Properties.Settings.Default.MainFormLocation = this.Location;
                }

                // Save some form settings.
                CoreKeepersWorkshop.Properties.Settings.Default.ItemAmount = 50;
                CoreKeepersWorkshop.Properties.Settings.Default.ItemID = 110;
                CoreKeepersWorkshop.Properties.Settings.Default.CurrentItemTab = "TabPage1";
                CoreKeepersWorkshop.Properties.Settings.Default.ItemVariation = 0;

                // Save UI form settings.
                CoreKeepersWorkshop.Properties.Settings.Default.InventoryBackgroundCount = inventorySkinCounter;
                CoreKeepersWorkshop.Properties.Settings.Default.PlayerBackgroundCount = playerSkinCounter;
                CoreKeepersWorkshop.Properties.Settings.Default.WorldBackgroundCount = worldSkinCounter;
                CoreKeepersWorkshop.Properties.Settings.Default.ChatBackgroundCount = chatSkinCounter;

                // Save some form controls.
                CoreKeepersWorkshop.Properties.Settings.Default.DevToolDelay = (int)numericUpDown2.Value; // Dev tool operation delay.
                CoreKeepersWorkshop.Properties.Settings.Default.RadialMoveScale = numericUpDown18.Value; // Auto render maps radialMoveScale.
                CoreKeepersWorkshop.Properties.Settings.Default.MapRenderingMax = numericUpDown14.Value; // Map rendering max radius.
                CoreKeepersWorkshop.Properties.Settings.Default.MapRenderingStart = numericUpDown16.Value; // Map rendering start radius.
                CoreKeepersWorkshop.Properties.Settings.Default.FishingCast = numericUpDown19.Value; // Fishing bot casting delay.
                CoreKeepersWorkshop.Properties.Settings.Default.FishingPadding = numericUpDown20.Value; // Fishing bot padding delay.
                CoreKeepersWorkshop.Properties.Settings.Default.Save();
            }
            catch (Exception)
            { } // Do nothing.
        }

        // Move window to the bottom left.
        private void Form1_Resize(object sender, EventArgs e)
        {
            // Get height for both types of taskbar modes.
            Rectangle activeScreenDimensions = Screen.FromControl(this).Bounds;

            // Save the previous form location before minimizing it.
            if (WindowState == FormWindowState.Normal && this.Location != new Point(0, activeScreenDimensions.Height - 40) && !isMinimized) // isMinimized fix 1.3.1.
            {
                CoreKeepersWorkshop.Properties.Settings.Default.MainFormLocation = this.Location;
            }

            // Get window states.
            if (WindowState == FormWindowState.Minimized && checkBox2.Checked)
            {
                // Adjust window properties
                this.WindowState = FormWindowState.Normal;
                this.Size = new Size(320, 37);

                // Adjust the form location.
                this.Location = new Point(0, activeScreenDimensions.Height - 40);

                // Adjust window properties
                this.Opacity = 0.8;
                this.MaximizeBox = true;
                this.MinimizeBox = false;

                // Adjust minimized bool.
                isMinimized = true;
            }
            else if (WindowState == FormWindowState.Maximized)
            {
                // Adjust window properties
                this.WindowState = FormWindowState.Normal;
                this.MaximizeBox = false;
                this.MinimizeBox = true;

                // Ensure we got the correct tab size to maximize back too.
                if (tabControl1.SelectedTab == tabPage1) // Inventory.
                {
                    this.Size = new Size(756, 494);
                }
                else if (tabControl1.SelectedTab == tabPage2) // Player.
                {
                    this.Size = new Size(756, 360);
                }
                else if (tabControl1.SelectedTab == tabPage8) // World.
                {
                    this.Size = new Size(756, 494);
                }
                else if (tabControl1.SelectedTab == tabPage5) // Chat.
                {
                    this.Size = new Size(410, 360);
                }

                // Adjust some final form settings.
                this.Opacity = 100;
                this.Location = CoreKeepersWorkshop.Properties.Settings.Default.MainFormLocation;

                // Adjust minimized bool.
                isMinimized = false;
            }
        }

        // Control switching tabs.
        int previousTab = 0;
        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage1) // Inventory.
            {
                this.Size = new Size(756, 494);
            }
            else if (tabControl1.SelectedTab == tabPage2) // Player.
            {
                this.Size = new Size(756, 360);
            }
            else if (tabControl1.SelectedTab == tabPage8) // World.
            {
                this.Size = new Size(756, 494);
            }
            else if (tabControl1.SelectedTab == tabPage5) // Chat.
            {
                this.Size = new Size(410, 360);
            }

            // Change skin
            if (tabControl1.SelectedTab == tabPage6)
            {
                // Get the tab we are changing.
                switch (previousTab)
                {

                    case 0: // Inventory
                        // Reset tab page back to one.
                        tabControl1.SelectedTab = tabPage1;

                        // Prevent overflow from add or removal of images.
                        if (inventorySkinCounter >= InventorySkins.Count()) { inventorySkinCounter = 0; }

                        // Ensure the skin exists. Fix: v1.2.9.
                        if (InventorySkins.Count() < 1 || !File.Exists(InventorySkins.ToArray()[inventorySkinCounter])) // Check if folder is empty. Fix: v1.3.4
                        {
                            // Display an error.
                            MessageBox.Show("No skins exist within the asset folder!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Change the background.
                        tabControl1.TabPages[0].BackgroundImage = ImageFast.FromFile(InventorySkins.ToArray()[inventorySkinCounter].ToString());

                        // Save the property in the settings.
                        CoreKeepersWorkshop.Properties.Settings.Default.InventoryBackground = InventorySkins.ToArray()[inventorySkinCounter].ToString();

                        // Add to the counter.
                        inventorySkinCounter++;
                        if (inventorySkinCounter == InventorySkins.Count()) { inventorySkinCounter = 0; }
                        break;
                    case 1: // Player
                        // Reset tab page back to two.
                        tabControl1.SelectedTab = tabPage2;

                        // Prevent overflow from add or removal of images.
                        if (playerSkinCounter >= PlayerSkins.Count()) { playerSkinCounter = 0; }

                        // Ensure the skin exists. Fix: v1.2.9.
                        if (PlayerSkins.Count() < 1 || !File.Exists(PlayerSkins.ToArray()[playerSkinCounter])) // Check if folder is empty. Fix: v1.3.4
                        {
                            // Display an error.
                            MessageBox.Show("No skins exist within the asset folder!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Change the background.
                        tabControl1.TabPages[1].BackgroundImage = ImageFast.FromFile(PlayerSkins.ToArray()[playerSkinCounter].ToString());

                        // Save the property in the settings.
                        CoreKeepersWorkshop.Properties.Settings.Default.PlayerBackground = PlayerSkins.ToArray()[playerSkinCounter].ToString();

                        // Add to the counter.
                        playerSkinCounter++;
                        if (playerSkinCounter == PlayerSkins.Count()) { playerSkinCounter = 0; }
                        break;
                    case 2: // World
                        // Reset tab page back to two.
                        tabControl1.SelectedTab = tabPage8;

                        // Prevent overflow from add or removal of images.
                        if (worldSkinCounter >= WorldSkins.Count()) { worldSkinCounter = 0; }

                        // Ensure the skin exists. Fix: v1.2.9.
                        if (WorldSkins.Count() < 1 || !File.Exists(WorldSkins.ToArray()[worldSkinCounter])) // Check if folder is empty. Fix: v1.3.4
                        {
                            // Display an error.
                            MessageBox.Show("No skins exist within the asset folder!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Change the background.
                        tabControl1.TabPages[2].BackgroundImage = ImageFast.FromFile(WorldSkins.ToArray()[worldSkinCounter].ToString());

                        // Save the property in the settings.
                        CoreKeepersWorkshop.Properties.Settings.Default.WorldBackground = WorldSkins.ToArray()[worldSkinCounter].ToString();

                        // Add to the counter.
                        worldSkinCounter++;
                        if (worldSkinCounter == WorldSkins.Count()) { worldSkinCounter = 0; }
                        break;
                    case 3: // Chat
                        // Reset tab page back to three.
                        tabControl1.SelectedTab = tabPage5;

                        // Prevent overflow from add or removal of images.
                        if (chatSkinCounter >= ChatSkins.Count()) { chatSkinCounter = 0; }

                        // Ensure the skin exists. Fix: v1.2.9.
                        if (ChatSkins.Count() < 1 || !File.Exists(ChatSkins.ToArray()[chatSkinCounter])) // Check if folder is empty. Fix: v1.3.4
                        {
                            // Display an error.
                            MessageBox.Show("No skins exist within the asset folder!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Change the background.
                        tabControl1.TabPages[3].BackgroundImage = ImageFast.FromFile(ChatSkins.ToArray()[chatSkinCounter].ToString());

                        // Save the property in the settings.
                        CoreKeepersWorkshop.Properties.Settings.Default.ChatBackground = ChatSkins.ToArray()[chatSkinCounter].ToString();

                        // Add to the counter.
                        chatSkinCounter++;
                        if (chatSkinCounter == ChatSkins.Count()) { chatSkinCounter = 0; }
                        break;
                }
            }

            // Update the previous tab value.
            previousTab = tabControl1.SelectedIndex;
        }

        #endregion // End form controls.

        #region Inventory Editor

        #region Image Tools

        // Function for combining bitmaps.
        public static Bitmap CombineBitmaps(params Bitmap[] sources)
        {
            List<int> imageHeights = new List<int>();
            List<int> imageWidths = new List<int>();
            foreach (Bitmap img in sources)
            {
                imageHeights.Add(img.Height);
                imageWidths.Add(img.Width);
            }
            Bitmap result = new Bitmap(imageWidths.Max(), imageHeights.Max());
            using (Graphics g = Graphics.FromImage(result))
            {
                foreach (Bitmap img in sources)
                    g.DrawImage(img, Point.Empty);
            }
            return result;
        }

        // Adjust the transparency of an image.
        private const int bytesPerPixel = 4;

        /// <summary>
        /// Change the opacity of an image
        /// </summary>
        /// <param name="originalImage">The original image</param>
        /// <param name="opacity">Opacity, where 1.0 is no opacity, 0.0 is full transparency</param>
        /// <returns>The changed image</returns>
        public static Bitmap ChangeImageOpacity(Bitmap originalImage, double opacity)
        {
            if ((originalImage.PixelFormat & PixelFormat.Indexed) == PixelFormat.Indexed)
            {
                // Cannot modify an image with indexed colors
                return originalImage;
            }

            Bitmap bmp = (Bitmap)originalImage.Clone();

            // Specify a pixel format.
            PixelFormat pxf = PixelFormat.Format32bppArgb;

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, pxf);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            // This code is specific to a bitmap with 32 bits per pixels 
            // (32 bits = 4 bytes, 3 for RGB and 1 byte for alpha).
            int numBytes = bmp.Width * bmp.Height * bytesPerPixel;
            byte[] argbValues = new byte[numBytes];

            // Copy the ARGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, argbValues, 0, numBytes);

            // Manipulate the bitmap, such as changing the
            // RGB values for all pixels in the the bitmap.
            for (int counter = 0; counter < argbValues.Length; counter += bytesPerPixel)
            {
                // argbValues is in format BGRA (Blue, Green, Red, Alpha)

                // If 100% transparent, skip pixel
                if (argbValues[counter + bytesPerPixel - 1] == 0)
                    continue;

                int pos = 0;
                pos++; // B value
                pos++; // G value
                pos++; // R value

                argbValues[counter + pos] = (byte)(argbValues[counter + pos] * opacity);
            }

            // Copy the ARGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(argbValues, 0, ptr, numBytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            return bmp;
        }

        // Convert image to grayscale.
        public static Bitmap MakeGrayscale3(Bitmap original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                //create the grayscale ColorMatrix
                ColorMatrix colorMatrix = new ColorMatrix(
                   new float[][]
                   {
                        new float[] {.3f, .3f, .3f, 0, 0},
                        new float[] {.59f, .59f, .59f, 0, 0},
                        new float[] {.11f, .11f, .11f, 0, 0},
                        new float[] {0, 0, 0, 1, 0},
                        new float[] {0, 0, 0, 0, 1}
                   });

                //create some image attributes
                using (ImageAttributes attributes = new ImageAttributes())
                {

                    //set the color matrix attribute
                    attributes.SetColorMatrix(colorMatrix);

                    //draw the original image on the new image
                    //using the grayscale color matrix
                    g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                                0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
                }
            }
            return newBitmap;
        }
        #endregion // End Image Tools

        // Get Inventory addresses.
        private void Button1_Click(object sender, EventArgs e)
        {
            // Reset progress bar.
            progressBar2.Value = 0;

            // Reset the useaddress.
            useAddress = 1;

            // Load addresses.
            GetInventoryAddresses();
        }

        // Scan for the inventory addresses.
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

            // Disable button to prevent spamming.
            button1.Enabled = false;

            // Reset textbox.
            richTextBox1.Text = "Addresses Loaded: 0";

            // Offset the progress bar to show it's working.
            progressBar2.Visible = true;
            progressBar2.Maximum = 100;
            progressBar2.Value = 10;

            // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
            // AoB scan is offset +1 bit to increase loading times.
            AoBScanResultsInventory = await MemLib.AoBScan("08 00 00 00 00 00 00 6E 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 6E 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00", true, true);

            // Get the progress bar maximum.
            progressBar2.Maximum = AoBScanResultsInventory.Count() * 50;

            // If the count is zero, the scan had an error.
            if (AoBScanResultsInventory.Count() == 0)
            {
                // Reset textbox.
                richTextBox1.Text = "Addresses Loaded: 0";

                // Reset progress bar.
                progressBar2.Value = 0;
                progressBar2.Visible = false;

                // Rename button back to defualt.
                button1.Text = "Get Inventory Addresses";

                // Re-enable button.
                button1.Enabled = true;

                // Reset aob scan results
                AoBScanResultsInventory = null;

                // Display error message.
                MessageBox.Show("You need to have torches in the first and last Inventory slots!!\n\nPlease ignore added inventory rows.\n\nNOTE: This tool is host only!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Update richtextbox with found addresses..
            foreach (long res in AoBScanResultsInventory)
            {
                if (richTextBox1.Text == "Addresses Loaded: 0")
                {
                    richTextBox1.Text = "Addresses Loaded: " + AoBScanResultsInventory.Count().ToString() + ", Selected: " + useAddress + ", [" + res.ToString("X").ToString();
                }
                else
                {
                    richTextBox1.Text += ", " + res.ToString("X").ToString();
                }
            }
            richTextBox1.Text += "]";

            // Enable controls if addresses where found or not.
            if (AoBScanResultsInventory.Count() > 0)
            {
                // Enable controls.
                button2.Enabled = true; // Reload.
                button3.Enabled = true; // Remove all.

                // If scan is larger then 1 result, enable arrow controls.
                if (AoBScanResultsInventory.Count() > 1)
                {
                    // Enable arrow buttons.
                    button20.Enabled = true; // Previous.
                    button21.Enabled = true; // Next.
                }
                else
                {
                    // Disable arrow buttons.
                    button20.Enabled = false; // Previous.
                    button21.Enabled = false; // Next.
                }
            }
            else
            {
                // Disable controls.
                button2.Enabled = false; // Reload.
                button3.Enabled = false; // Remove all.
                button20.Enabled = false; // Previous.
                button21.Enabled = false; // Next.
            }

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

                if (!loadInventory) // Prevent double error messages. // Fix: v1.3.4.7.
                    MessageBox.Show("You need to first scan for the Inventory addresses!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            // Set playername in jason array.
            if (ExportInventory)
            {
                SaveFileDialog sfd = new SaveFileDialog()
                {
                    Filter = "Player File|*.ckplayer"
                };

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

            // Select the inventory to use.
            var res = AoBScanResultsInventory.ElementAt(useAddress - 1);

            // Get address from loop.
            // Base address was moved 9 bits.
            string baseAddress = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("7", NumberStyles.Integer)).ToString("X");

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
                pictureBox31.Image = null;
                pictureBox32.Image = null;
                pictureBox33.Image = null;
                pictureBox34.Image = null;
                pictureBox35.Image = null;
                pictureBox36.Image = null;
                pictureBox37.Image = null;
                pictureBox38.Image = null;
                pictureBox39.Image = null;
                pictureBox40.Image = null;
                pictureBox41.Image = null;
                pictureBox42.Image = null;
                pictureBox43.Image = null;
                pictureBox44.Image = null;
                pictureBox45.Image = null;
                pictureBox46.Image = null;
                pictureBox47.Image = null;
                pictureBox48.Image = null;
                pictureBox49.Image = null;
                pictureBox50.Image = null;
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
                                finalItemAmount = (int)MemLib.ReadUInt(slot1Amount);
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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox1.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox1.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox1.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 1 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot1Amount) + " | Variation: " + (MemLib.ReadInt(slot1Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 1 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot1Amount) + " | Variation: " + (MemLib.ReadInt(slot1Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox2.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox2.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox2.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 2 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot2Amount) + " | Variation: " + (MemLib.ReadInt(slot2Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 2 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot2Amount) + " | Variation: " + (MemLib.ReadInt(slot2Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                finalItemAmount = (int)MemLib.ReadUInt(slot3Amount); // Update slots amount. // Lost and found fix v1.3.3.1.
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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox3.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox3.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox3.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 3 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot3Amount) + " | Variation: " + (MemLib.ReadInt(slot3Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 3 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot3Amount) + " | Variation: " + (MemLib.ReadInt(slot3Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                }
                                catch (Exception)
                                {
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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox4.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox4.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox4.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 4 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot4Amount) + " | Variation: " + (MemLib.ReadInt(slot4Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 4 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot4Amount) + " | Variation: " + (MemLib.ReadInt(slot4Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox5.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox5.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox5.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox5.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox5.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox5.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 5 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot5Amount) + " | Variation: " + (MemLib.ReadInt(slot5Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 5 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot5Amount) + " | Variation: " + (MemLib.ReadInt(slot5Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox6.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox6.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox6.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox6.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox6.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox6.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 6 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot6Amount) + " | Variation: " + (MemLib.ReadInt(slot6Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 6 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot6Amount) + " | Variation: " + (MemLib.ReadInt(slot6Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox7.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox7.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox7.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox7.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox7.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox7.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 7 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot7Amount) + " | Variation: " + (MemLib.ReadInt(slot7Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 7 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot7Amount) + " | Variation: " + (MemLib.ReadInt(slot7Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox8.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox8.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox8.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox8.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox8.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox8.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 8 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot8Amount) + " | Variation: " + (MemLib.ReadInt(slot8Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 8 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot8Amount) + " | Variation: " + (MemLib.ReadInt(slot8Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox9.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox9.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox9.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox9.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox9.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox9.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 9 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot9Amount) + " | Variation: " + (MemLib.ReadInt(slot9Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 9 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot9Amount) + " | Variation: " + (MemLib.ReadInt(slot9Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox10.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox10.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox10.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox10.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox10.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox10.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 10 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot10Amount) + " | Variation: " + (MemLib.ReadInt(slot10Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 10 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot10Amount) + " | Variation: " + (MemLib.ReadInt(slot10Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox11.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox11.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox11.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox11.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox11.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox11.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 11 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot11Amount) + " | Variation: " + (MemLib.ReadInt(slot11Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 11 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot11Amount) + " | Variation: " + (MemLib.ReadInt(slot11Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox12.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox12.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox12.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox12.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox12.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox12.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 12 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot12Amount) + " | Variation: " + (MemLib.ReadInt(slot12Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 12 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot12Amount) + " | Variation: " + (MemLib.ReadInt(slot12Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox13.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox13.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox13.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox13.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox13.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox13.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 13 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot13Amount) + " | Variation: " + (MemLib.ReadInt(slot13Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 13 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot13Amount) + " | Variation: " + (MemLib.ReadInt(slot13Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox14.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox14.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox14.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox14.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox14.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox14.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 14 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot14Amount) + " | Variation: " + (MemLib.ReadInt(slot14Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 14 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot14Amount) + " | Variation: " + (MemLib.ReadInt(slot14Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox15.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox15.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox15.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox15.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox15.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox15.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 15 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot15Amount) + " | Variation: " + (MemLib.ReadInt(slot15Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 15 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot15Amount) + " | Variation: " + (MemLib.ReadInt(slot15Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox16.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox16.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox16.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox16.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox16.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox16.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 16 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot16Amount) + " | Variation: " + (MemLib.ReadInt(slot16Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 16 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot16Amount) + " | Variation: " + (MemLib.ReadInt(slot16Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox17.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox17.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox17.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox17.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox17.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox17.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 17 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot17Amount) + " | Variation: " + (MemLib.ReadInt(slot17Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 17 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot17Amount) + " | Variation: " + (MemLib.ReadInt(slot17Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox18.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox18.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox18.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox18.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox18.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox18.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 18 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot18Amount) + " | Variation: " + (MemLib.ReadInt(slot18Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 18 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot18Amount) + " | Variation: " + (MemLib.ReadInt(slot18Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox19.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox19.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox19.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox19.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox19.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox19.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 19 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot19Amount) + " | Variation: " + (MemLib.ReadInt(slot19Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 19 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot19Amount) + " | Variation: " + (MemLib.ReadInt(slot19Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox20.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox20.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox20.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox20.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox20.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox20.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 20 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot20Amount) + " | Variation: " + (MemLib.ReadInt(slot20Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 20 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot20Amount) + " | Variation: " + (MemLib.ReadInt(slot20Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox21.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox21.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox21.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox21.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox21.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox21.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 21 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot21Amount) + " | Variation: " + (MemLib.ReadInt(slot21Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 21 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot21Amount) + " | Variation: " + (MemLib.ReadInt(slot21Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox22.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox22.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox22.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox22.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox22.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox22.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 22 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot22Amount) + " | Variation: " + (MemLib.ReadInt(slot22Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 22 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot22Amount) + " | Variation: " + (MemLib.ReadInt(slot22Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox23.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox23.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox23.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox23.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox23.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox23.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 23 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot23Amount) + " | Variation: " + (MemLib.ReadInt(slot23Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 23 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot23Amount) + " | Variation: " + (MemLib.ReadInt(slot23Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox24.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox24.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox24.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox24.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox24.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox24.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 24 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot24Amount) + " | Variation: " + (MemLib.ReadInt(slot24Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 24 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot24Amount) + " | Variation: " + (MemLib.ReadInt(slot24Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox25.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox25.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox25.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox25.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox25.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox25.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 25 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot25Amount) + " | Variation: " + (MemLib.ReadInt(slot25Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 25 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot25Amount) + " | Variation: " + (MemLib.ReadInt(slot25Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox26.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox26.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox26.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox26.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox26.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox26.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 26 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot26Amount) + " | Variation: " + (MemLib.ReadInt(slot26Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 26 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot26Amount) + " | Variation: " + (MemLib.ReadInt(slot26Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox27.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox27.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox27.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox27.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox27.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox27.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 27 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot27Amount) + " | Variation: " + (MemLib.ReadInt(slot27Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 27 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot27Amount) + " | Variation: " + (MemLib.ReadInt(slot27Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox28.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox28.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox28.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox28.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox28.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox28.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 28 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot28Amount) + " | Variation: " + (MemLib.ReadInt(slot28Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 28 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot28Amount) + " | Variation: " + (MemLib.ReadInt(slot28Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox29.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox29.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox29.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox29.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox29.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox29.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 29 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot29Amount) + " | Variation: " + (MemLib.ReadInt(slot29Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 29 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot29Amount) + " | Variation: " + (MemLib.ReadInt(slot29Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox30.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox30.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox30.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox30.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox30.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox30.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 30 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot30Amount) + " | Variation: " + (MemLib.ReadInt(slot30Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 30 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot30Amount) + " | Variation: " + (MemLib.ReadInt(slot30Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

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
                if (itemSlot == 31 || loadInventory || CycleAll || ExportInventory)
                {
                    string slot31Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("480", NumberStyles.Integer)).ToString("X");
                    string slot31Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("484", NumberStyles.Integer)).ToString("X");
                    string slot31Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("488", NumberStyles.Integer)).ToString("X");

                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set Values
                    if (!loadInventory && !ExportInventory && !GetItemInfo)
                    {
                        // Add New Item
                        MemLib.WriteMemory(slot31Item, "int", type.ToString()); // Write item type
                        if (type == 0)
                        {
                            MemLib.WriteMemory(slot31Amount, "int", "0"); // Write item amount
                            MemLib.WriteMemory(slot31Variation, "int", "0"); // Write item variation
                            finalItemAmount = 0;
                        }
                        else
                        {
                            if (Overwrite)
                            {
                                MemLib.WriteMemory(slot31Amount, "int", amount.ToString()); // Write item amount
                                MemLib.WriteMemory(slot31Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = amount;
                            }
                            else
                            {
                                MemLib.WriteMemory(slot31Amount, "int", (MemLib.ReadUInt(slot31Amount) + amount).ToString()); // Write item amount
                                MemLib.WriteMemory(slot31Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = (int)MemLib.ReadUInt(slot31Amount);
                            }
                        }
                    }
                    else
                    {
                        // Export inventory to list.
                        if (ExportInventory)
                        {
                            if (!ExportPlayerItems.ContainsKey("itemSlot31"))
                            {
                                ExportPlayerItems.Add("itemSlot31-ID", MemLib.ReadInt(slot31Item));
                                ExportPlayerItems.Add("itemSlot31-Amount", MemLib.ReadInt(slot31Amount));
                                ExportPlayerItems.Add("itemSlot31-Variation", MemLib.ReadInt(slot31Variation));
                            }
                        }
                        else
                        {
                            // First Load
                            type = MemLib.ReadInt(slot31Item);
                            variation = MemLib.ReadInt(slot31Variation);

                            // Load Picture
                            // Set image to null if type is zero.
                            if (type.ToString() == "0")
                            {
                                pictureBox31.Image = null;
                            }
                            else if (pictureBox31.Image == null)
                            {
                                // Get Picture
                                try
                                {
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox31.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox31.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox31.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox31.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox31.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox31.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 31 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot31Amount) + " | Variation: " + (MemLib.ReadInt(slot31Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 31 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot31Amount) + " | Variation: " + (MemLib.ReadInt(slot31Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

                                    // Draw item amount.
                                    using (Font font = new Font("Arial", 24f))
                                    using (Graphics G = Graphics.FromImage(pictureBox31.Image))
                                    using (GraphicsPath gp = new GraphicsPath())
                                    {
                                        // Do drawling actions.
                                        gp.AddString(MemLib.ReadInt(slot31Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                        G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                        G.FillPath(new SolidBrush(Color.White), gp);
                                    }
                                    pictureBox31.Invalidate(); // Reload picturebox.
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }

                    // Do some textbox scrolling.
                    richTextBox3.ScrollToCaret();

                    // Do some information stuff.
                    if (GetItemInfo)
                    {
                        infoType = MemLib.ReadInt(slot31Item);
                        infoAmount = MemLib.ReadInt(slot31Amount);
                        infoVariant = MemLib.ReadInt(slot31Variation);
                    }
                }
                if (itemSlot == 32 || loadInventory || CycleAll || ExportInventory)
                {
                    string slot32Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("496", NumberStyles.Integer)).ToString("X");
                    string slot32Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("500", NumberStyles.Integer)).ToString("X");
                    string slot32Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("504", NumberStyles.Integer)).ToString("X");

                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set Values
                    if (!loadInventory && !ExportInventory && !GetItemInfo)
                    {
                        // Add New Item
                        MemLib.WriteMemory(slot32Item, "int", type.ToString()); // Write item type
                        if (type == 0)
                        {
                            MemLib.WriteMemory(slot32Amount, "int", "0"); // Write item amount
                            MemLib.WriteMemory(slot32Variation, "int", "0"); // Write item variation
                            finalItemAmount = 0;
                        }
                        else
                        {
                            if (Overwrite)
                            {
                                MemLib.WriteMemory(slot32Amount, "int", amount.ToString()); // Write item amount
                                MemLib.WriteMemory(slot32Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = amount;
                            }
                            else
                            {
                                MemLib.WriteMemory(slot32Amount, "int", (MemLib.ReadUInt(slot32Amount) + amount).ToString()); // Write item amount
                                MemLib.WriteMemory(slot32Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = (int)MemLib.ReadUInt(slot32Amount);
                            }
                        }
                    }
                    else
                    {
                        // Export inventory to list.
                        if (ExportInventory)
                        {
                            if (!ExportPlayerItems.ContainsKey("itemSlot32"))
                            {
                                ExportPlayerItems.Add("itemSlot32-ID", MemLib.ReadInt(slot32Item));
                                ExportPlayerItems.Add("itemSlot32-Amount", MemLib.ReadInt(slot32Amount));
                                ExportPlayerItems.Add("itemSlot32-Variation", MemLib.ReadInt(slot32Variation));
                            }
                        }
                        else
                        {
                            // First Load
                            type = MemLib.ReadInt(slot32Item);
                            variation = MemLib.ReadInt(slot32Variation);

                            // Load Picture
                            // Set image to null if type is zero.
                            if (type.ToString() == "0")
                            {
                                pictureBox32.Image = null;
                            }
                            else if (pictureBox32.Image == null)
                            {
                                // Get Picture
                                try
                                {
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox32.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox32.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox32.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox32.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox32.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox32.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 32 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot32Amount) + " | Variation: " + (MemLib.ReadInt(slot32Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 32 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot32Amount) + " | Variation: " + (MemLib.ReadInt(slot32Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

                                    // Draw item amount.
                                    using (Font font = new Font("Arial", 24f))
                                    using (Graphics G = Graphics.FromImage(pictureBox32.Image))
                                    using (GraphicsPath gp = new GraphicsPath())
                                    {
                                        // Do drawling actions.
                                        gp.AddString(MemLib.ReadInt(slot32Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                        G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                        G.FillPath(new SolidBrush(Color.White), gp);
                                    }
                                    pictureBox32.Invalidate(); // Reload picturebox.
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }

                    // Do some textbox scrolling.
                    richTextBox3.ScrollToCaret();

                    // Do some information stuff.
                    if (GetItemInfo)
                    {
                        infoType = MemLib.ReadInt(slot32Item);
                        infoAmount = MemLib.ReadInt(slot32Amount);
                        infoVariant = MemLib.ReadInt(slot32Variation);
                    }
                }
                if (itemSlot == 33 || loadInventory || CycleAll || ExportInventory)
                {
                    string slot33Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("512", NumberStyles.Integer)).ToString("X");
                    string slot33Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("516", NumberStyles.Integer)).ToString("X");
                    string slot33Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("520", NumberStyles.Integer)).ToString("X");

                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set Values
                    if (!loadInventory && !ExportInventory && !GetItemInfo)
                    {
                        // Add New Item
                        MemLib.WriteMemory(slot33Item, "int", type.ToString()); // Write item type
                        if (type == 0)
                        {
                            MemLib.WriteMemory(slot33Amount, "int", "0"); // Write item amount
                            MemLib.WriteMemory(slot33Variation, "int", "0"); // Write item variation
                            finalItemAmount = 0;
                        }
                        else
                        {
                            if (Overwrite)
                            {
                                MemLib.WriteMemory(slot33Amount, "int", amount.ToString()); // Write item amount
                                MemLib.WriteMemory(slot33Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = amount;
                            }
                            else
                            {
                                MemLib.WriteMemory(slot33Amount, "int", (MemLib.ReadUInt(slot33Amount) + amount).ToString()); // Write item amount
                                MemLib.WriteMemory(slot33Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = (int)MemLib.ReadUInt(slot33Amount);
                            }
                        }
                    }
                    else
                    {
                        // Export inventory to list.
                        if (ExportInventory)
                        {
                            if (!ExportPlayerItems.ContainsKey("itemSlot33"))
                            {
                                ExportPlayerItems.Add("itemSlot33-ID", MemLib.ReadInt(slot33Item));
                                ExportPlayerItems.Add("itemSlot33-Amount", MemLib.ReadInt(slot33Amount));
                                ExportPlayerItems.Add("itemSlot33-Variation", MemLib.ReadInt(slot33Variation));
                            }
                        }
                        else
                        {
                            // First Load
                            type = MemLib.ReadInt(slot33Item);
                            variation = MemLib.ReadInt(slot33Variation);

                            // Load Picture
                            // Set image to null if type is zero.
                            if (type.ToString() == "0")
                            {
                                pictureBox33.Image = null;
                            }
                            else if (pictureBox33.Image == null)
                            {
                                // Get Picture
                                try
                                {
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox33.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox33.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox33.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox33.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox33.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox33.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 33 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot33Amount) + " | Variation: " + (MemLib.ReadInt(slot33Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 33 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot33Amount) + " | Variation: " + (MemLib.ReadInt(slot33Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

                                    // Draw item amount.
                                    using (Font font = new Font("Arial", 24f))
                                    using (Graphics G = Graphics.FromImage(pictureBox33.Image))
                                    using (GraphicsPath gp = new GraphicsPath())
                                    {
                                        // Do drawling actions.
                                        gp.AddString(MemLib.ReadInt(slot33Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                        G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                        G.FillPath(new SolidBrush(Color.White), gp);
                                    }
                                    pictureBox33.Invalidate(); // Reload picturebox.
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }

                    // Do some textbox scrolling.
                    richTextBox3.ScrollToCaret();

                    // Do some information stuff.
                    if (GetItemInfo)
                    {
                        infoType = MemLib.ReadInt(slot33Item);
                        infoAmount = MemLib.ReadInt(slot33Amount);
                        infoVariant = MemLib.ReadInt(slot33Variation);
                    }
                }
                if (itemSlot == 34 || loadInventory || CycleAll || ExportInventory)
                {
                    string slot34Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("528", NumberStyles.Integer)).ToString("X");
                    string slot34Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("532", NumberStyles.Integer)).ToString("X");
                    string slot34Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("536", NumberStyles.Integer)).ToString("X");

                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set Values
                    if (!loadInventory && !ExportInventory && !GetItemInfo)
                    {
                        // Add New Item
                        MemLib.WriteMemory(slot34Item, "int", type.ToString()); // Write item type
                        if (type == 0)
                        {
                            MemLib.WriteMemory(slot34Amount, "int", "0"); // Write item amount
                            MemLib.WriteMemory(slot34Variation, "int", "0"); // Write item variation
                            finalItemAmount = 0;
                        }
                        else
                        {
                            if (Overwrite)
                            {
                                MemLib.WriteMemory(slot34Amount, "int", amount.ToString()); // Write item amount
                                MemLib.WriteMemory(slot34Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = amount;
                            }
                            else
                            {
                                MemLib.WriteMemory(slot34Amount, "int", (MemLib.ReadUInt(slot34Amount) + amount).ToString()); // Write item amount
                                MemLib.WriteMemory(slot34Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = (int)MemLib.ReadUInt(slot34Amount);
                            }
                        }
                    }
                    else
                    {
                        // Export inventory to list.
                        if (ExportInventory)
                        {
                            if (!ExportPlayerItems.ContainsKey("itemSlot34"))
                            {
                                ExportPlayerItems.Add("itemSlot34-ID", MemLib.ReadInt(slot34Item));
                                ExportPlayerItems.Add("itemSlot34-Amount", MemLib.ReadInt(slot34Amount));
                                ExportPlayerItems.Add("itemSlot34-Variation", MemLib.ReadInt(slot34Variation));
                            }
                        }
                        else
                        {
                            // First Load
                            type = MemLib.ReadInt(slot34Item);
                            variation = MemLib.ReadInt(slot34Variation);

                            // Load Picture
                            // Set image to null if type is zero.
                            if (type.ToString() == "0")
                            {
                                pictureBox34.Image = null;
                            }
                            else if (pictureBox34.Image == null)
                            {
                                // Get Picture
                                try
                                {
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox34.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox34.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox34.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox34.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox34.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox34.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 34 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot34Amount) + " | Variation: " + (MemLib.ReadInt(slot34Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 34 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot34Amount) + " | Variation: " + (MemLib.ReadInt(slot34Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

                                    // Draw item amount.
                                    using (Font font = new Font("Arial", 24f))
                                    using (Graphics G = Graphics.FromImage(pictureBox34.Image))
                                    using (GraphicsPath gp = new GraphicsPath())
                                    {
                                        // Do drawling actions.
                                        gp.AddString(MemLib.ReadInt(slot34Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                        G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                        G.FillPath(new SolidBrush(Color.White), gp);
                                    }
                                    pictureBox34.Invalidate(); // Reload picturebox.
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }

                    // Do some textbox scrolling.
                    richTextBox3.ScrollToCaret();

                    // Do some information stuff.
                    if (GetItemInfo)
                    {
                        infoType = MemLib.ReadInt(slot34Item);
                        infoAmount = MemLib.ReadInt(slot34Amount);
                        infoVariant = MemLib.ReadInt(slot34Variation);
                    }
                }
                if (itemSlot == 35 || loadInventory || CycleAll || ExportInventory)
                {
                    string slot35Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("544", NumberStyles.Integer)).ToString("X");
                    string slot35Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("548", NumberStyles.Integer)).ToString("X");
                    string slot35Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("552", NumberStyles.Integer)).ToString("X");

                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set Values
                    if (!loadInventory && !ExportInventory && !GetItemInfo)
                    {
                        // Add New Item
                        MemLib.WriteMemory(slot35Item, "int", type.ToString()); // Write item type
                        if (type == 0)
                        {
                            MemLib.WriteMemory(slot35Amount, "int", "0"); // Write item amount
                            MemLib.WriteMemory(slot35Variation, "int", "0"); // Write item variation
                            finalItemAmount = 0;
                        }
                        else
                        {
                            if (Overwrite)
                            {
                                MemLib.WriteMemory(slot35Amount, "int", amount.ToString()); // Write item amount
                                MemLib.WriteMemory(slot35Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = amount;
                            }
                            else
                            {
                                MemLib.WriteMemory(slot35Amount, "int", (MemLib.ReadUInt(slot35Amount) + amount).ToString()); // Write item amount
                                MemLib.WriteMemory(slot35Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = (int)MemLib.ReadUInt(slot35Amount);
                            }
                        }
                    }
                    else
                    {
                        // Export inventory to list.
                        if (ExportInventory)
                        {
                            if (!ExportPlayerItems.ContainsKey("itemSlot35"))
                            {
                                ExportPlayerItems.Add("itemSlot35-ID", MemLib.ReadInt(slot35Item));
                                ExportPlayerItems.Add("itemSlot35-Amount", MemLib.ReadInt(slot35Amount));
                                ExportPlayerItems.Add("itemSlot35-Variation", MemLib.ReadInt(slot35Variation));
                            }
                        }
                        else
                        {
                            // First Load
                            type = MemLib.ReadInt(slot35Item);
                            variation = MemLib.ReadInt(slot35Variation);

                            // Load Picture
                            // Set image to null if type is zero.
                            if (type.ToString() == "0")
                            {
                                pictureBox35.Image = null;
                            }
                            else if (pictureBox35.Image == null)
                            {
                                // Get Picture
                                try
                                {
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox35.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox35.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox35.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox35.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox35.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox35.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 35 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot35Amount) + " | Variation: " + (MemLib.ReadInt(slot35Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 35 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot35Amount) + " | Variation: " + (MemLib.ReadInt(slot35Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

                                    // Draw item amount.
                                    using (Font font = new Font("Arial", 24f))
                                    using (Graphics G = Graphics.FromImage(pictureBox35.Image))
                                    using (GraphicsPath gp = new GraphicsPath())
                                    {
                                        // Do drawling actions.
                                        gp.AddString(MemLib.ReadInt(slot35Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                        G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                        G.FillPath(new SolidBrush(Color.White), gp);
                                    }
                                    pictureBox35.Invalidate(); // Reload picturebox.
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }

                    // Do some textbox scrolling.
                    richTextBox3.ScrollToCaret();

                    // Do some information stuff.
                    if (GetItemInfo)
                    {
                        infoType = MemLib.ReadInt(slot35Item);
                        infoAmount = MemLib.ReadInt(slot35Amount);
                        infoVariant = MemLib.ReadInt(slot35Variation);
                    }
                }
                if (itemSlot == 36 || loadInventory || CycleAll || ExportInventory)
                {
                    string slot36Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("560", NumberStyles.Integer)).ToString("X");
                    string slot36Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("564", NumberStyles.Integer)).ToString("X");
                    string slot36Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("568", NumberStyles.Integer)).ToString("X");

                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set Values
                    if (!loadInventory && !ExportInventory && !GetItemInfo)
                    {
                        // Add New Item
                        MemLib.WriteMemory(slot36Item, "int", type.ToString()); // Write item type
                        if (type == 0)
                        {
                            MemLib.WriteMemory(slot36Amount, "int", "0"); // Write item amount
                            MemLib.WriteMemory(slot36Variation, "int", "0"); // Write item variation
                            finalItemAmount = 0;
                        }
                        else
                        {
                            if (Overwrite)
                            {
                                MemLib.WriteMemory(slot36Amount, "int", amount.ToString()); // Write item amount
                                MemLib.WriteMemory(slot36Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = amount;
                            }
                            else
                            {
                                MemLib.WriteMemory(slot36Amount, "int", (MemLib.ReadUInt(slot36Amount) + amount).ToString()); // Write item amount
                                MemLib.WriteMemory(slot36Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = (int)MemLib.ReadUInt(slot36Amount);
                            }
                        }
                    }
                    else
                    {
                        // Export inventory to list.
                        if (ExportInventory)
                        {
                            if (!ExportPlayerItems.ContainsKey("itemSlot36"))
                            {
                                ExportPlayerItems.Add("itemSlot36-ID", MemLib.ReadInt(slot36Item));
                                ExportPlayerItems.Add("itemSlot36-Amount", MemLib.ReadInt(slot36Amount));
                                ExportPlayerItems.Add("itemSlot36-Variation", MemLib.ReadInt(slot36Variation));
                            }
                        }
                        else
                        {
                            // First Load
                            type = MemLib.ReadInt(slot36Item);
                            variation = MemLib.ReadInt(slot36Variation);

                            // Load Picture
                            // Set image to null if type is zero.
                            if (type.ToString() == "0")
                            {
                                pictureBox36.Image = null;
                            }
                            else if (pictureBox36.Image == null)
                            {
                                // Get Picture
                                try
                                {
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox36.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox36.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox36.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox36.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox36.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox36.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 36 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot36Amount) + " | Variation: " + (MemLib.ReadInt(slot36Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 36 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot36Amount) + " | Variation: " + (MemLib.ReadInt(slot36Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

                                    // Draw item amount.
                                    using (Font font = new Font("Arial", 24f))
                                    using (Graphics G = Graphics.FromImage(pictureBox36.Image))
                                    using (GraphicsPath gp = new GraphicsPath())
                                    {
                                        // Do drawling actions.
                                        gp.AddString(MemLib.ReadInt(slot36Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                        G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                        G.FillPath(new SolidBrush(Color.White), gp);
                                    }
                                    pictureBox36.Invalidate(); // Reload picturebox.
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }

                    // Do some textbox scrolling.
                    richTextBox3.ScrollToCaret();

                    // Do some information stuff.
                    if (GetItemInfo)
                    {
                        infoType = MemLib.ReadInt(slot36Item);
                        infoAmount = MemLib.ReadInt(slot36Amount);
                        infoVariant = MemLib.ReadInt(slot36Variation);
                    }
                }
                if (itemSlot == 37 || loadInventory || CycleAll || ExportInventory)
                {
                    string slot37Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("576", NumberStyles.Integer)).ToString("X");
                    string slot37Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("580", NumberStyles.Integer)).ToString("X");
                    string slot37Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("584", NumberStyles.Integer)).ToString("X");

                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set Values
                    if (!loadInventory && !ExportInventory && !GetItemInfo)
                    {
                        // Add New Item
                        MemLib.WriteMemory(slot37Item, "int", type.ToString()); // Write item type
                        if (type == 0)
                        {
                            MemLib.WriteMemory(slot37Amount, "int", "0"); // Write item amount
                            MemLib.WriteMemory(slot37Variation, "int", "0"); // Write item variation
                            finalItemAmount = 0;
                        }
                        else
                        {
                            if (Overwrite)
                            {
                                MemLib.WriteMemory(slot37Amount, "int", amount.ToString()); // Write item amount
                                MemLib.WriteMemory(slot37Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = amount;
                            }
                            else
                            {
                                MemLib.WriteMemory(slot37Amount, "int", (MemLib.ReadUInt(slot37Amount) + amount).ToString()); // Write item amount
                                MemLib.WriteMemory(slot37Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = (int)MemLib.ReadUInt(slot37Amount);
                            }
                        }
                    }
                    else
                    {
                        // Export inventory to list.
                        if (ExportInventory)
                        {
                            if (!ExportPlayerItems.ContainsKey("itemSlot37"))
                            {
                                ExportPlayerItems.Add("itemSlot37-ID", MemLib.ReadInt(slot37Item));
                                ExportPlayerItems.Add("itemSlot37-Amount", MemLib.ReadInt(slot37Amount));
                                ExportPlayerItems.Add("itemSlot37-Variation", MemLib.ReadInt(slot37Variation));
                            }
                        }
                        else
                        {
                            // First Load
                            type = MemLib.ReadInt(slot37Item);
                            variation = MemLib.ReadInt(slot37Variation);

                            // Load Picture
                            // Set image to null if type is zero.
                            if (type.ToString() == "0")
                            {
                                pictureBox37.Image = null;
                            }
                            else if (pictureBox37.Image == null)
                            {
                                // Get Picture
                                try
                                {
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox37.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox37.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox37.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox37.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox37.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox37.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 37 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot37Amount) + " | Variation: " + (MemLib.ReadInt(slot37Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 37 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot37Amount) + " | Variation: " + (MemLib.ReadInt(slot37Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

                                    // Draw item amount.
                                    using (Font font = new Font("Arial", 24f))
                                    using (Graphics G = Graphics.FromImage(pictureBox37.Image))
                                    using (GraphicsPath gp = new GraphicsPath())
                                    {
                                        // Do drawling actions.
                                        gp.AddString(MemLib.ReadInt(slot37Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                        G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                        G.FillPath(new SolidBrush(Color.White), gp);
                                    }
                                    pictureBox37.Invalidate(); // Reload picturebox.
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }

                    // Do some textbox scrolling.
                    richTextBox3.ScrollToCaret();

                    // Do some information stuff.
                    if (GetItemInfo)
                    {
                        infoType = MemLib.ReadInt(slot37Item);
                        infoAmount = MemLib.ReadInt(slot37Amount);
                        infoVariant = MemLib.ReadInt(slot37Variation);
                    }
                }
                if (itemSlot == 38 || loadInventory || CycleAll || ExportInventory)
                {
                    string slot38Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("592", NumberStyles.Integer)).ToString("X");
                    string slot38Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("596", NumberStyles.Integer)).ToString("X");
                    string slot38Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("600", NumberStyles.Integer)).ToString("X");

                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set Values
                    if (!loadInventory && !ExportInventory && !GetItemInfo)
                    {
                        // Add New Item
                        MemLib.WriteMemory(slot38Item, "int", type.ToString()); // Write item type
                        if (type == 0)
                        {
                            MemLib.WriteMemory(slot38Amount, "int", "0"); // Write item amount
                            MemLib.WriteMemory(slot38Variation, "int", "0"); // Write item variation
                            finalItemAmount = 0;
                        }
                        else
                        {
                            if (Overwrite)
                            {
                                MemLib.WriteMemory(slot38Amount, "int", amount.ToString()); // Write item amount
                                MemLib.WriteMemory(slot38Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = amount;
                            }
                            else
                            {
                                MemLib.WriteMemory(slot38Amount, "int", (MemLib.ReadUInt(slot38Amount) + amount).ToString()); // Write item amount
                                MemLib.WriteMemory(slot38Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = (int)MemLib.ReadUInt(slot38Amount);
                            }
                        }
                    }
                    else
                    {
                        // Export inventory to list.
                        if (ExportInventory)
                        {
                            if (!ExportPlayerItems.ContainsKey("itemSlot38"))
                            {
                                ExportPlayerItems.Add("itemSlot38-ID", MemLib.ReadInt(slot38Item));
                                ExportPlayerItems.Add("itemSlot38-Amount", MemLib.ReadInt(slot38Amount));
                                ExportPlayerItems.Add("itemSlot38-Variation", MemLib.ReadInt(slot38Variation));
                            }
                        }
                        else
                        {
                            // First Load
                            type = MemLib.ReadInt(slot38Item);
                            variation = MemLib.ReadInt(slot38Variation);

                            // Load Picture
                            // Set image to null if type is zero.
                            if (type.ToString() == "0")
                            {
                                pictureBox38.Image = null;
                            }
                            else if (pictureBox38.Image == null)
                            {
                                // Get Picture
                                try
                                {
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox38.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox38.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox38.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox38.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox38.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox38.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 38 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot38Amount) + " | Variation: " + (MemLib.ReadInt(slot38Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 38 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot38Amount) + " | Variation: " + (MemLib.ReadInt(slot38Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

                                    // Draw item amount.
                                    using (Font font = new Font("Arial", 24f))
                                    using (Graphics G = Graphics.FromImage(pictureBox38.Image))
                                    using (GraphicsPath gp = new GraphicsPath())
                                    {
                                        // Do drawling actions.
                                        gp.AddString(MemLib.ReadInt(slot38Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                        G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                        G.FillPath(new SolidBrush(Color.White), gp);
                                    }
                                    pictureBox38.Invalidate(); // Reload picturebox.
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }

                    // Do some textbox scrolling.
                    richTextBox3.ScrollToCaret();

                    // Do some information stuff.
                    if (GetItemInfo)
                    {
                        infoType = MemLib.ReadInt(slot38Item);
                        infoAmount = MemLib.ReadInt(slot38Amount);
                        infoVariant = MemLib.ReadInt(slot38Variation);
                    }
                }
                if (itemSlot == 39 || loadInventory || CycleAll || ExportInventory)
                {
                    string slot39Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("608", NumberStyles.Integer)).ToString("X");
                    string slot39Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("612", NumberStyles.Integer)).ToString("X");
                    string slot39Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("616", NumberStyles.Integer)).ToString("X");

                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set Values
                    if (!loadInventory && !ExportInventory && !GetItemInfo)
                    {
                        // Add New Item
                        MemLib.WriteMemory(slot39Item, "int", type.ToString()); // Write item type
                        if (type == 0)
                        {
                            MemLib.WriteMemory(slot39Amount, "int", "0"); // Write item amount
                            MemLib.WriteMemory(slot39Variation, "int", "0"); // Write item variation
                            finalItemAmount = 0;
                        }
                        else
                        {
                            if (Overwrite)
                            {
                                MemLib.WriteMemory(slot39Amount, "int", amount.ToString()); // Write item amount
                                MemLib.WriteMemory(slot39Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = amount;
                            }
                            else
                            {
                                MemLib.WriteMemory(slot39Amount, "int", (MemLib.ReadUInt(slot39Amount) + amount).ToString()); // Write item amount
                                MemLib.WriteMemory(slot39Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = (int)MemLib.ReadUInt(slot39Amount);
                            }
                        }
                    }
                    else
                    {
                        // Export inventory to list.
                        if (ExportInventory)
                        {
                            if (!ExportPlayerItems.ContainsKey("itemSlot39"))
                            {
                                ExportPlayerItems.Add("itemSlot39-ID", MemLib.ReadInt(slot39Item));
                                ExportPlayerItems.Add("itemSlot39-Amount", MemLib.ReadInt(slot39Amount));
                                ExportPlayerItems.Add("itemSlot39-Variation", MemLib.ReadInt(slot39Variation));
                            }
                        }
                        else
                        {
                            // First Load
                            type = MemLib.ReadInt(slot39Item);
                            variation = MemLib.ReadInt(slot39Variation);

                            // Load Picture
                            // Set image to null if type is zero.
                            if (type.ToString() == "0")
                            {
                                pictureBox39.Image = null;
                            }
                            else if (pictureBox39.Image == null)
                            {
                                // Get Picture
                                try
                                {
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox39.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox39.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox39.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox39.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox39.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox39.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 39 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot39Amount) + " | Variation: " + (MemLib.ReadInt(slot39Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 39 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot39Amount) + " | Variation: " + (MemLib.ReadInt(slot39Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

                                    // Draw item amount.
                                    using (Font font = new Font("Arial", 24f))
                                    using (Graphics G = Graphics.FromImage(pictureBox39.Image))
                                    using (GraphicsPath gp = new GraphicsPath())
                                    {
                                        // Do drawling actions.
                                        gp.AddString(MemLib.ReadInt(slot39Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                        G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                        G.FillPath(new SolidBrush(Color.White), gp);
                                    }
                                    pictureBox39.Invalidate(); // Reload picturebox.
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }

                    // Do some textbox scrolling.
                    richTextBox3.ScrollToCaret();

                    // Do some information stuff.
                    if (GetItemInfo)
                    {
                        infoType = MemLib.ReadInt(slot39Item);
                        infoAmount = MemLib.ReadInt(slot39Amount);
                        infoVariant = MemLib.ReadInt(slot39Variation);
                    }
                }
                if (itemSlot == 40 || loadInventory || CycleAll || ExportInventory)
                {
                    string slot40Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("624", NumberStyles.Integer)).ToString("X");
                    string slot40Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("628", NumberStyles.Integer)).ToString("X");
                    string slot40Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("632", NumberStyles.Integer)).ToString("X");

                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set Values
                    if (!loadInventory && !ExportInventory && !GetItemInfo)
                    {
                        // Add New Item
                        MemLib.WriteMemory(slot40Item, "int", type.ToString()); // Write item type
                        if (type == 0)
                        {
                            MemLib.WriteMemory(slot40Amount, "int", "0"); // Write item amount
                            MemLib.WriteMemory(slot40Variation, "int", "0"); // Write item variation
                            finalItemAmount = 0;
                        }
                        else
                        {
                            if (Overwrite)
                            {
                                MemLib.WriteMemory(slot40Amount, "int", amount.ToString()); // Write item amount
                                MemLib.WriteMemory(slot40Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = amount;
                            }
                            else
                            {
                                MemLib.WriteMemory(slot40Amount, "int", (MemLib.ReadUInt(slot40Amount) + amount).ToString()); // Write item amount
                                MemLib.WriteMemory(slot40Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = (int)MemLib.ReadUInt(slot40Amount);
                            }
                        }
                    }
                    else
                    {
                        // Export inventory to list.
                        if (ExportInventory)
                        {
                            if (!ExportPlayerItems.ContainsKey("itemSlot40"))
                            {
                                ExportPlayerItems.Add("itemSlot40-ID", MemLib.ReadInt(slot40Item));
                                ExportPlayerItems.Add("itemSlot40-Amount", MemLib.ReadInt(slot40Amount));
                                ExportPlayerItems.Add("itemSlot40-Variation", MemLib.ReadInt(slot40Variation));
                            }
                        }
                        else
                        {
                            // First Load
                            type = MemLib.ReadInt(slot40Item);
                            variation = MemLib.ReadInt(slot40Variation);

                            // Load Picture
                            // Set image to null if type is zero.
                            if (type.ToString() == "0")
                            {
                                pictureBox40.Image = null;
                            }
                            else if (pictureBox40.Image == null)
                            {
                                // Get Picture
                                try
                                {
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox40.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox40.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox40.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox40.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox40.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox40.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 40 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot40Amount) + " | Variation: " + (MemLib.ReadInt(slot40Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 40 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot40Amount) + " | Variation: " + (MemLib.ReadInt(slot40Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

                                    // Draw item amount.
                                    using (Font font = new Font("Arial", 24f))
                                    using (Graphics G = Graphics.FromImage(pictureBox40.Image))
                                    using (GraphicsPath gp = new GraphicsPath())
                                    {
                                        // Do drawling actions.
                                        gp.AddString(MemLib.ReadInt(slot40Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                        G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                        G.FillPath(new SolidBrush(Color.White), gp);
                                    }
                                    pictureBox40.Invalidate(); // Reload picturebox.
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }

                    // Do some textbox scrolling.
                    richTextBox3.ScrollToCaret();

                    // Do some information stuff.
                    if (GetItemInfo)
                    {
                        infoType = MemLib.ReadInt(slot40Item);
                        infoAmount = MemLib.ReadInt(slot40Amount);
                        infoVariant = MemLib.ReadInt(slot40Variation);
                    }
                }
                if (itemSlot == 41 || loadInventory || CycleAll || ExportInventory)
                {
                    string slot41Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("640", NumberStyles.Integer)).ToString("X");
                    string slot41Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("644", NumberStyles.Integer)).ToString("X");
                    string slot41Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("648", NumberStyles.Integer)).ToString("X");

                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set Values
                    if (!loadInventory && !ExportInventory && !GetItemInfo)
                    {
                        // Add New Item
                        MemLib.WriteMemory(slot41Item, "int", type.ToString()); // Write item type
                        if (type == 0)
                        {
                            MemLib.WriteMemory(slot41Amount, "int", "0"); // Write item amount
                            MemLib.WriteMemory(slot41Variation, "int", "0"); // Write item variation
                            finalItemAmount = 0;
                        }
                        else
                        {
                            if (Overwrite)
                            {
                                MemLib.WriteMemory(slot41Amount, "int", amount.ToString()); // Write item amount
                                MemLib.WriteMemory(slot41Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = amount;
                            }
                            else
                            {
                                MemLib.WriteMemory(slot41Amount, "int", (MemLib.ReadUInt(slot41Amount) + amount).ToString()); // Write item amount
                                MemLib.WriteMemory(slot41Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = (int)MemLib.ReadUInt(slot41Amount);
                            }
                        }
                    }
                    else
                    {
                        // Export inventory to list.
                        if (ExportInventory)
                        {
                            if (!ExportPlayerItems.ContainsKey("itemSlot41"))
                            {
                                ExportPlayerItems.Add("itemSlot41-ID", MemLib.ReadInt(slot41Item));
                                ExportPlayerItems.Add("itemSlot41-Amount", MemLib.ReadInt(slot41Amount));
                                ExportPlayerItems.Add("itemSlot41-Variation", MemLib.ReadInt(slot41Variation));
                            }
                        }
                        else
                        {
                            // First Load
                            type = MemLib.ReadInt(slot41Item);
                            variation = MemLib.ReadInt(slot41Variation);

                            // Load Picture
                            // Set image to null if type is zero.
                            if (type.ToString() == "0")
                            {
                                pictureBox41.Image = null;
                            }
                            else if (pictureBox41.Image == null)
                            {
                                // Get Picture
                                try
                                {
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox41.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox41.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox41.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox41.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox41.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox41.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 41 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot41Amount) + " | Variation: " + (MemLib.ReadInt(slot41Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 41 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot41Amount) + " | Variation: " + (MemLib.ReadInt(slot41Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

                                    // Draw item amount.
                                    using (Font font = new Font("Arial", 24f))
                                    using (Graphics G = Graphics.FromImage(pictureBox41.Image))
                                    using (GraphicsPath gp = new GraphicsPath())
                                    {
                                        // Do drawling actions.
                                        gp.AddString(MemLib.ReadInt(slot41Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                        G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                        G.FillPath(new SolidBrush(Color.White), gp);
                                    }
                                    pictureBox41.Invalidate(); // Reload picturebox.
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }

                    // Do some textbox scrolling.
                    richTextBox3.ScrollToCaret();

                    // Do some information stuff.
                    if (GetItemInfo)
                    {
                        infoType = MemLib.ReadInt(slot41Item);
                        infoAmount = MemLib.ReadInt(slot41Amount);
                        infoVariant = MemLib.ReadInt(slot41Variation);
                    }
                }
                if (itemSlot == 42 || loadInventory || CycleAll || ExportInventory)
                {
                    string slot42Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("656", NumberStyles.Integer)).ToString("X");
                    string slot42Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("660", NumberStyles.Integer)).ToString("X");
                    string slot42Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("664", NumberStyles.Integer)).ToString("X");

                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set Values
                    if (!loadInventory && !ExportInventory && !GetItemInfo)
                    {
                        // Add New Item
                        MemLib.WriteMemory(slot42Item, "int", type.ToString()); // Write item type
                        if (type == 0)
                        {
                            MemLib.WriteMemory(slot42Amount, "int", "0"); // Write item amount
                            MemLib.WriteMemory(slot42Variation, "int", "0"); // Write item variation
                            finalItemAmount = 0;
                        }
                        else
                        {
                            if (Overwrite)
                            {
                                MemLib.WriteMemory(slot42Amount, "int", amount.ToString()); // Write item amount
                                MemLib.WriteMemory(slot42Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = amount;
                            }
                            else
                            {
                                MemLib.WriteMemory(slot42Amount, "int", (MemLib.ReadUInt(slot42Amount) + amount).ToString()); // Write item amount
                                MemLib.WriteMemory(slot42Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = (int)MemLib.ReadUInt(slot42Amount);
                            }
                        }
                    }
                    else
                    {
                        // Export inventory to list.
                        if (ExportInventory)
                        {
                            if (!ExportPlayerItems.ContainsKey("itemSlot42"))
                            {
                                ExportPlayerItems.Add("itemSlot42-ID", MemLib.ReadInt(slot42Item));
                                ExportPlayerItems.Add("itemSlot42-Amount", MemLib.ReadInt(slot42Amount));
                                ExportPlayerItems.Add("itemSlot42-Variation", MemLib.ReadInt(slot42Variation));
                            }
                        }
                        else
                        {
                            // First Load
                            type = MemLib.ReadInt(slot42Item);
                            variation = MemLib.ReadInt(slot42Variation);

                            // Load Picture
                            // Set image to null if type is zero.
                            if (type.ToString() == "0")
                            {
                                pictureBox42.Image = null;
                            }
                            else if (pictureBox42.Image == null)
                            {
                                // Get Picture
                                try
                                {
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox42.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox42.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox42.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox42.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox42.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox42.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 42 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot42Amount) + " | Variation: " + (MemLib.ReadInt(slot42Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 42 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot42Amount) + " | Variation: " + (MemLib.ReadInt(slot42Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

                                    // Draw item amount.
                                    using (Font font = new Font("Arial", 24f))
                                    using (Graphics G = Graphics.FromImage(pictureBox42.Image))
                                    using (GraphicsPath gp = new GraphicsPath())
                                    {
                                        // Do drawling actions.
                                        gp.AddString(MemLib.ReadInt(slot42Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                        G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                        G.FillPath(new SolidBrush(Color.White), gp);
                                    }
                                    pictureBox42.Invalidate(); // Reload picturebox.
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }

                    // Do some textbox scrolling.
                    richTextBox3.ScrollToCaret();

                    // Do some information stuff.
                    if (GetItemInfo)
                    {
                        infoType = MemLib.ReadInt(slot42Item);
                        infoAmount = MemLib.ReadInt(slot42Amount);
                        infoVariant = MemLib.ReadInt(slot42Variation);
                    }
                }
                if (itemSlot == 43 || loadInventory || CycleAll || ExportInventory)
                {
                    string slot43Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("672", NumberStyles.Integer)).ToString("X");
                    string slot43Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("676", NumberStyles.Integer)).ToString("X");
                    string slot43Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("680", NumberStyles.Integer)).ToString("X");

                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set Values
                    if (!loadInventory && !ExportInventory && !GetItemInfo)
                    {
                        // Add New Item
                        MemLib.WriteMemory(slot43Item, "int", type.ToString()); // Write item type
                        if (type == 0)
                        {
                            MemLib.WriteMemory(slot43Amount, "int", "0"); // Write item amount
                            MemLib.WriteMemory(slot43Variation, "int", "0"); // Write item variation
                            finalItemAmount = 0;
                        }
                        else
                        {
                            if (Overwrite)
                            {
                                MemLib.WriteMemory(slot43Amount, "int", amount.ToString()); // Write item amount
                                MemLib.WriteMemory(slot43Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = amount;
                            }
                            else
                            {
                                MemLib.WriteMemory(slot43Amount, "int", (MemLib.ReadUInt(slot43Amount) + amount).ToString()); // Write item amount
                                MemLib.WriteMemory(slot43Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = (int)MemLib.ReadUInt(slot43Amount);
                            }
                        }
                    }
                    else
                    {
                        // Export inventory to list.
                        if (ExportInventory)
                        {
                            if (!ExportPlayerItems.ContainsKey("itemSlot43"))
                            {
                                ExportPlayerItems.Add("itemSlot43-ID", MemLib.ReadInt(slot43Item));
                                ExportPlayerItems.Add("itemSlot43-Amount", MemLib.ReadInt(slot43Amount));
                                ExportPlayerItems.Add("itemSlot43-Variation", MemLib.ReadInt(slot43Variation));
                            }
                        }
                        else
                        {
                            // First Load
                            type = MemLib.ReadInt(slot43Item);
                            variation = MemLib.ReadInt(slot43Variation);

                            // Load Picture
                            // Set image to null if type is zero.
                            if (type.ToString() == "0")
                            {
                                pictureBox43.Image = null;
                            }
                            else if (pictureBox43.Image == null)
                            {
                                // Get Picture
                                try
                                {
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox43.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox43.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox43.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox43.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox43.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox43.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 43 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot43Amount) + " | Variation: " + (MemLib.ReadInt(slot43Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 43 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot43Amount) + " | Variation: " + (MemLib.ReadInt(slot43Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

                                    // Draw item amount.
                                    using (Font font = new Font("Arial", 24f))
                                    using (Graphics G = Graphics.FromImage(pictureBox43.Image))
                                    using (GraphicsPath gp = new GraphicsPath())
                                    {
                                        // Do drawling actions.
                                        gp.AddString(MemLib.ReadInt(slot43Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                        G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                        G.FillPath(new SolidBrush(Color.White), gp);
                                    }
                                    pictureBox43.Invalidate(); // Reload picturebox.
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }

                    // Do some textbox scrolling.
                    richTextBox3.ScrollToCaret();

                    // Do some information stuff.
                    if (GetItemInfo)
                    {
                        infoType = MemLib.ReadInt(slot43Item);
                        infoAmount = MemLib.ReadInt(slot43Amount);
                        infoVariant = MemLib.ReadInt(slot43Variation);
                    }
                }
                if (itemSlot == 44 || loadInventory || CycleAll || ExportInventory)
                {
                    string slot44Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("688", NumberStyles.Integer)).ToString("X");
                    string slot44Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("692", NumberStyles.Integer)).ToString("X");
                    string slot44Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("696", NumberStyles.Integer)).ToString("X");

                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set Values
                    if (!loadInventory && !ExportInventory && !GetItemInfo)
                    {
                        // Add New Item
                        MemLib.WriteMemory(slot44Item, "int", type.ToString()); // Write item type
                        if (type == 0)
                        {
                            MemLib.WriteMemory(slot44Amount, "int", "0"); // Write item amount
                            MemLib.WriteMemory(slot44Variation, "int", "0"); // Write item variation
                            finalItemAmount = 0;
                        }
                        else
                        {
                            if (Overwrite)
                            {
                                MemLib.WriteMemory(slot44Amount, "int", amount.ToString()); // Write item amount
                                MemLib.WriteMemory(slot44Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = amount;
                            }
                            else
                            {
                                MemLib.WriteMemory(slot44Amount, "int", (MemLib.ReadUInt(slot44Amount) + amount).ToString()); // Write item amount
                                MemLib.WriteMemory(slot44Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = (int)MemLib.ReadUInt(slot44Amount);
                            }
                        }
                    }
                    else
                    {
                        // Export inventory to list.
                        if (ExportInventory)
                        {
                            if (!ExportPlayerItems.ContainsKey("itemSlot44"))
                            {
                                ExportPlayerItems.Add("itemSlot44-ID", MemLib.ReadInt(slot44Item));
                                ExportPlayerItems.Add("itemSlot44-Amount", MemLib.ReadInt(slot44Amount));
                                ExportPlayerItems.Add("itemSlot44-Variation", MemLib.ReadInt(slot44Variation));
                            }
                        }
                        else
                        {
                            // First Load
                            type = MemLib.ReadInt(slot44Item);
                            variation = MemLib.ReadInt(slot44Variation);

                            // Load Picture
                            // Set image to null if type is zero.
                            if (type.ToString() == "0")
                            {
                                pictureBox44.Image = null;
                            }
                            else if (pictureBox44.Image == null)
                            {
                                // Get Picture
                                try
                                {
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox44.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox44.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox44.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox44.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox44.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox44.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 44 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot44Amount) + " | Variation: " + (MemLib.ReadInt(slot44Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 44 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot44Amount) + " | Variation: " + (MemLib.ReadInt(slot44Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

                                    // Draw item amount.
                                    using (Font font = new Font("Arial", 24f))
                                    using (Graphics G = Graphics.FromImage(pictureBox44.Image))
                                    using (GraphicsPath gp = new GraphicsPath())
                                    {
                                        // Do drawling actions.
                                        gp.AddString(MemLib.ReadInt(slot44Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                        G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                        G.FillPath(new SolidBrush(Color.White), gp);
                                    }
                                    pictureBox44.Invalidate(); // Reload picturebox.
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }

                    // Do some textbox scrolling.
                    richTextBox3.ScrollToCaret();

                    // Do some information stuff.
                    if (GetItemInfo)
                    {
                        infoType = MemLib.ReadInt(slot44Item);
                        infoAmount = MemLib.ReadInt(slot44Amount);
                        infoVariant = MemLib.ReadInt(slot44Variation);
                    }
                }
                if (itemSlot == 45 || loadInventory || CycleAll || ExportInventory)
                {
                    string slot45Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("704", NumberStyles.Integer)).ToString("X");
                    string slot45Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("708", NumberStyles.Integer)).ToString("X");
                    string slot45Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("712", NumberStyles.Integer)).ToString("X");

                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set Values
                    if (!loadInventory && !ExportInventory && !GetItemInfo)
                    {
                        // Add New Item
                        MemLib.WriteMemory(slot45Item, "int", type.ToString()); // Write item type
                        if (type == 0)
                        {
                            MemLib.WriteMemory(slot45Amount, "int", "0"); // Write item amount
                            MemLib.WriteMemory(slot45Variation, "int", "0"); // Write item variation
                            finalItemAmount = 0;
                        }
                        else
                        {
                            if (Overwrite)
                            {
                                MemLib.WriteMemory(slot45Amount, "int", amount.ToString()); // Write item amount
                                MemLib.WriteMemory(slot45Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = amount;
                            }
                            else
                            {
                                MemLib.WriteMemory(slot45Amount, "int", (MemLib.ReadUInt(slot45Amount) + amount).ToString()); // Write item amount
                                MemLib.WriteMemory(slot45Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = (int)MemLib.ReadUInt(slot45Amount);
                            }
                        }
                    }
                    else
                    {
                        // Export inventory to list.
                        if (ExportInventory)
                        {
                            if (!ExportPlayerItems.ContainsKey("itemSlot45"))
                            {
                                ExportPlayerItems.Add("itemSlot45-ID", MemLib.ReadInt(slot45Item));
                                ExportPlayerItems.Add("itemSlot45-Amount", MemLib.ReadInt(slot45Amount));
                                ExportPlayerItems.Add("itemSlot45-Variation", MemLib.ReadInt(slot45Variation));
                            }
                        }
                        else
                        {
                            // First Load
                            type = MemLib.ReadInt(slot45Item);
                            variation = MemLib.ReadInt(slot45Variation);

                            // Load Picture
                            // Set image to null if type is zero.
                            if (type.ToString() == "0")
                            {
                                pictureBox45.Image = null;
                            }
                            else if (pictureBox45.Image == null)
                            {
                                // Get Picture
                                try
                                {
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox45.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox45.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox45.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox45.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox45.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox45.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 45 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot45Amount) + " | Variation: " + (MemLib.ReadInt(slot45Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 45 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot45Amount) + " | Variation: " + (MemLib.ReadInt(slot45Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

                                    // Draw item amount.
                                    using (Font font = new Font("Arial", 24f))
                                    using (Graphics G = Graphics.FromImage(pictureBox45.Image))
                                    using (GraphicsPath gp = new GraphicsPath())
                                    {
                                        // Do drawling actions.
                                        gp.AddString(MemLib.ReadInt(slot45Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                        G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                        G.FillPath(new SolidBrush(Color.White), gp);
                                    }
                                    pictureBox45.Invalidate(); // Reload picturebox.
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }

                    // Do some textbox scrolling.
                    richTextBox3.ScrollToCaret();

                    // Do some information stuff.
                    if (GetItemInfo)
                    {
                        infoType = MemLib.ReadInt(slot45Item);
                        infoAmount = MemLib.ReadInt(slot45Amount);
                        infoVariant = MemLib.ReadInt(slot45Variation);
                    }
                }
                if (itemSlot == 46 || loadInventory || CycleAll || ExportInventory)
                {
                    string slot46Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("720", NumberStyles.Integer)).ToString("X");
                    string slot46Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("724", NumberStyles.Integer)).ToString("X");
                    string slot46Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("728", NumberStyles.Integer)).ToString("X");

                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set Values
                    if (!loadInventory && !ExportInventory && !GetItemInfo)
                    {
                        // Add New Item
                        MemLib.WriteMemory(slot46Item, "int", type.ToString()); // Write item type
                        if (type == 0)
                        {
                            MemLib.WriteMemory(slot46Amount, "int", "0"); // Write item amount
                            MemLib.WriteMemory(slot46Variation, "int", "0"); // Write item variation
                            finalItemAmount = 0;
                        }
                        else
                        {
                            if (Overwrite)
                            {
                                MemLib.WriteMemory(slot46Amount, "int", amount.ToString()); // Write item amount
                                MemLib.WriteMemory(slot46Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = amount;
                            }
                            else
                            {
                                MemLib.WriteMemory(slot46Amount, "int", (MemLib.ReadUInt(slot46Amount) + amount).ToString()); // Write item amount
                                MemLib.WriteMemory(slot46Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = (int)MemLib.ReadUInt(slot46Amount);
                            }
                        }
                    }
                    else
                    {
                        // Export inventory to list.
                        if (ExportInventory)
                        {
                            if (!ExportPlayerItems.ContainsKey("itemSlot46"))
                            {
                                ExportPlayerItems.Add("itemSlot46-ID", MemLib.ReadInt(slot46Item));
                                ExportPlayerItems.Add("itemSlot46-Amount", MemLib.ReadInt(slot46Amount));
                                ExportPlayerItems.Add("itemSlot46-Variation", MemLib.ReadInt(slot46Variation));
                            }
                        }
                        else
                        {
                            // First Load
                            type = MemLib.ReadInt(slot46Item);
                            variation = MemLib.ReadInt(slot46Variation);

                            // Load Picture
                            // Set image to null if type is zero.
                            if (type.ToString() == "0")
                            {
                                pictureBox46.Image = null;
                            }
                            else if (pictureBox46.Image == null)
                            {
                                // Get Picture
                                try
                                {
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox46.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox46.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox46.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox46.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox46.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox46.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 46 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot46Amount) + " | Variation: " + (MemLib.ReadInt(slot46Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 46 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot46Amount) + " | Variation: " + (MemLib.ReadInt(slot46Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

                                    // Draw item amount.
                                    using (Font font = new Font("Arial", 24f))
                                    using (Graphics G = Graphics.FromImage(pictureBox46.Image))
                                    using (GraphicsPath gp = new GraphicsPath())
                                    {
                                        // Do drawling actions.
                                        gp.AddString(MemLib.ReadInt(slot46Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                        G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                        G.FillPath(new SolidBrush(Color.White), gp);
                                    }
                                    pictureBox46.Invalidate(); // Reload picturebox.
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }

                    // Do some textbox scrolling.
                    richTextBox3.ScrollToCaret();

                    // Do some information stuff.
                    if (GetItemInfo)
                    {
                        infoType = MemLib.ReadInt(slot46Item);
                        infoAmount = MemLib.ReadInt(slot46Amount);
                        infoVariant = MemLib.ReadInt(slot46Variation);
                    }
                }
                if (itemSlot == 47 || loadInventory || CycleAll || ExportInventory)
                {
                    string slot47Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("736", NumberStyles.Integer)).ToString("X");
                    string slot47Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("740", NumberStyles.Integer)).ToString("X");
                    string slot47Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("744", NumberStyles.Integer)).ToString("X");

                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set Values
                    if (!loadInventory && !ExportInventory && !GetItemInfo)
                    {
                        // Add New Item
                        MemLib.WriteMemory(slot47Item, "int", type.ToString()); // Write item type
                        if (type == 0)
                        {
                            MemLib.WriteMemory(slot47Amount, "int", "0"); // Write item amount
                            MemLib.WriteMemory(slot47Variation, "int", "0"); // Write item variation
                            finalItemAmount = 0;
                        }
                        else
                        {
                            if (Overwrite)
                            {
                                MemLib.WriteMemory(slot47Amount, "int", amount.ToString()); // Write item amount
                                MemLib.WriteMemory(slot47Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = amount;
                            }
                            else
                            {
                                MemLib.WriteMemory(slot47Amount, "int", (MemLib.ReadUInt(slot47Amount) + amount).ToString()); // Write item amount
                                MemLib.WriteMemory(slot47Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = (int)MemLib.ReadUInt(slot47Amount);
                            }
                        }
                    }
                    else
                    {
                        // Export inventory to list.
                        if (ExportInventory)
                        {
                            if (!ExportPlayerItems.ContainsKey("itemSlot47"))
                            {
                                ExportPlayerItems.Add("itemSlot47-ID", MemLib.ReadInt(slot47Item));
                                ExportPlayerItems.Add("itemSlot47-Amount", MemLib.ReadInt(slot47Amount));
                                ExportPlayerItems.Add("itemSlot47-Variation", MemLib.ReadInt(slot47Variation));
                            }
                        }
                        else
                        {
                            // First Load
                            type = MemLib.ReadInt(slot47Item);
                            variation = MemLib.ReadInt(slot47Variation);

                            // Load Picture
                            // Set image to null if type is zero.
                            if (type.ToString() == "0")
                            {
                                pictureBox47.Image = null;
                            }
                            else if (pictureBox47.Image == null)
                            {
                                // Get Picture
                                try
                                {
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox47.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox47.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox47.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox47.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox47.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox47.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 47 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot47Amount) + " | Variation: " + (MemLib.ReadInt(slot47Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 47 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot47Amount) + " | Variation: " + (MemLib.ReadInt(slot47Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

                                    // Draw item amount.
                                    using (Font font = new Font("Arial", 24f))
                                    using (Graphics G = Graphics.FromImage(pictureBox47.Image))
                                    using (GraphicsPath gp = new GraphicsPath())
                                    {
                                        // Do drawling actions.
                                        gp.AddString(MemLib.ReadInt(slot47Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                        G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                        G.FillPath(new SolidBrush(Color.White), gp);
                                    }
                                    pictureBox47.Invalidate(); // Reload picturebox.
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }

                    // Do some textbox scrolling.
                    richTextBox3.ScrollToCaret();

                    // Do some information stuff.
                    if (GetItemInfo)
                    {
                        infoType = MemLib.ReadInt(slot47Item);
                        infoAmount = MemLib.ReadInt(slot47Amount);
                        infoVariant = MemLib.ReadInt(slot47Variation);
                    }
                }
                if (itemSlot == 48 || loadInventory || CycleAll || ExportInventory)
                {
                    string slot48Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("752", NumberStyles.Integer)).ToString("X");
                    string slot48Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("756", NumberStyles.Integer)).ToString("X");
                    string slot48Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("760", NumberStyles.Integer)).ToString("X");

                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set Values
                    if (!loadInventory && !ExportInventory && !GetItemInfo)
                    {
                        // Add New Item
                        MemLib.WriteMemory(slot48Item, "int", type.ToString()); // Write item type
                        if (type == 0)
                        {
                            MemLib.WriteMemory(slot48Amount, "int", "0"); // Write item amount
                            MemLib.WriteMemory(slot48Variation, "int", "0"); // Write item variation
                            finalItemAmount = 0;
                        }
                        else
                        {
                            if (Overwrite)
                            {
                                MemLib.WriteMemory(slot48Amount, "int", amount.ToString()); // Write item amount
                                MemLib.WriteMemory(slot48Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = amount;
                            }
                            else
                            {
                                MemLib.WriteMemory(slot48Amount, "int", (MemLib.ReadUInt(slot48Amount) + amount).ToString()); // Write item amount
                                MemLib.WriteMemory(slot48Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = (int)MemLib.ReadUInt(slot48Amount);
                            }
                        }
                    }
                    else
                    {
                        // Export inventory to list.
                        if (ExportInventory)
                        {
                            if (!ExportPlayerItems.ContainsKey("itemSlot48"))
                            {
                                ExportPlayerItems.Add("itemSlot48-ID", MemLib.ReadInt(slot48Item));
                                ExportPlayerItems.Add("itemSlot48-Amount", MemLib.ReadInt(slot48Amount));
                                ExportPlayerItems.Add("itemSlot48-Variation", MemLib.ReadInt(slot48Variation));
                            }
                        }
                        else
                        {
                            // First Load
                            type = MemLib.ReadInt(slot48Item);
                            variation = MemLib.ReadInt(slot48Variation);

                            // Load Picture
                            // Set image to null if type is zero.
                            if (type.ToString() == "0")
                            {
                                pictureBox48.Image = null;
                            }
                            else if (pictureBox48.Image == null)
                            {
                                // Get Picture
                                try
                                {
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox48.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox48.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox48.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox48.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox48.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox48.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 48 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot48Amount) + " | Variation: " + (MemLib.ReadInt(slot48Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 48 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot48Amount) + " | Variation: " + (MemLib.ReadInt(slot48Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

                                    // Draw item amount.
                                    using (Font font = new Font("Arial", 24f))
                                    using (Graphics G = Graphics.FromImage(pictureBox48.Image))
                                    using (GraphicsPath gp = new GraphicsPath())
                                    {
                                        // Do drawling actions.
                                        gp.AddString(MemLib.ReadInt(slot48Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                        G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                        G.FillPath(new SolidBrush(Color.White), gp);
                                    }
                                    pictureBox48.Invalidate(); // Reload picturebox.
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }

                    // Do some textbox scrolling.
                    richTextBox3.ScrollToCaret();

                    // Do some information stuff.
                    if (GetItemInfo)
                    {
                        infoType = MemLib.ReadInt(slot48Item);
                        infoAmount = MemLib.ReadInt(slot48Amount);
                        infoVariant = MemLib.ReadInt(slot48Variation);
                    }
                }
                if (itemSlot == 49 || loadInventory || CycleAll || ExportInventory)
                {
                    string slot49Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("768", NumberStyles.Integer)).ToString("X");
                    string slot49Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("772", NumberStyles.Integer)).ToString("X");
                    string slot49Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("776", NumberStyles.Integer)).ToString("X");

                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set Values
                    if (!loadInventory && !ExportInventory && !GetItemInfo)
                    {
                        // Add New Item
                        MemLib.WriteMemory(slot49Item, "int", type.ToString()); // Write item type
                        if (type == 0)
                        {
                            MemLib.WriteMemory(slot49Amount, "int", "0"); // Write item amount
                            MemLib.WriteMemory(slot49Variation, "int", "0"); // Write item variation
                            finalItemAmount = 0;
                        }
                        else
                        {
                            if (Overwrite)
                            {
                                MemLib.WriteMemory(slot49Amount, "int", amount.ToString()); // Write item amount
                                MemLib.WriteMemory(slot49Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = amount;
                            }
                            else
                            {
                                MemLib.WriteMemory(slot49Amount, "int", (MemLib.ReadUInt(slot49Amount) + amount).ToString()); // Write item amount
                                MemLib.WriteMemory(slot49Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = (int)MemLib.ReadUInt(slot49Amount);
                            }
                        }
                    }
                    else
                    {
                        // Export inventory to list.
                        if (ExportInventory)
                        {
                            if (!ExportPlayerItems.ContainsKey("itemSlot49"))
                            {
                                ExportPlayerItems.Add("itemSlot49-ID", MemLib.ReadInt(slot49Item));
                                ExportPlayerItems.Add("itemSlot49-Amount", MemLib.ReadInt(slot49Amount));
                                ExportPlayerItems.Add("itemSlot49-Variation", MemLib.ReadInt(slot49Variation));
                            }
                        }
                        else
                        {
                            // First Load
                            type = MemLib.ReadInt(slot49Item);
                            variation = MemLib.ReadInt(slot49Variation);

                            // Load Picture
                            // Set image to null if type is zero.
                            if (type.ToString() == "0")
                            {
                                pictureBox49.Image = null;
                            }
                            else if (pictureBox49.Image == null)
                            {
                                // Get Picture
                                try
                                {
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox49.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox49.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox49.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox49.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox49.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox49.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 49 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot49Amount) + " | Variation: " + (MemLib.ReadInt(slot49Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 49 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot49Amount) + " | Variation: " + (MemLib.ReadInt(slot49Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

                                    // Draw item amount.
                                    using (Font font = new Font("Arial", 24f))
                                    using (Graphics G = Graphics.FromImage(pictureBox49.Image))
                                    using (GraphicsPath gp = new GraphicsPath())
                                    {
                                        // Do drawling actions.
                                        gp.AddString(MemLib.ReadInt(slot49Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                        G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                        G.FillPath(new SolidBrush(Color.White), gp);
                                    }
                                    pictureBox49.Invalidate(); // Reload picturebox.
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }

                    // Do some textbox scrolling.
                    richTextBox3.ScrollToCaret();

                    // Do some information stuff.
                    if (GetItemInfo)
                    {
                        infoType = MemLib.ReadInt(slot49Item);
                        infoAmount = MemLib.ReadInt(slot49Amount);
                        infoVariant = MemLib.ReadInt(slot49Variation);
                    }
                }
                if (itemSlot == 50 || loadInventory || CycleAll || ExportInventory)
                {
                    string slot50Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("784", NumberStyles.Integer)).ToString("X");
                    string slot50Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("788", NumberStyles.Integer)).ToString("X");
                    string slot50Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("792", NumberStyles.Integer)).ToString("X");

                    // Perform progress step.
                    progressBar2.PerformStep();

                    // Set Values
                    if (!loadInventory && !ExportInventory && !GetItemInfo)
                    {
                        // Add New Item
                        MemLib.WriteMemory(slot50Item, "int", type.ToString()); // Write item type
                        if (type == 0)
                        {
                            MemLib.WriteMemory(slot50Amount, "int", "0"); // Write item amount
                            MemLib.WriteMemory(slot50Variation, "int", "0"); // Write item variation
                            finalItemAmount = 0;
                        }
                        else
                        {
                            if (Overwrite)
                            {
                                MemLib.WriteMemory(slot50Amount, "int", amount.ToString()); // Write item amount
                                MemLib.WriteMemory(slot50Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = amount;
                            }
                            else
                            {
                                MemLib.WriteMemory(slot50Amount, "int", (MemLib.ReadUInt(slot50Amount) + amount).ToString()); // Write item amount
                                MemLib.WriteMemory(slot50Variation, "int", variation.ToString()); // Write item variation
                                finalItemAmount = (int)MemLib.ReadUInt(slot50Amount);
                            }
                        }
                    }
                    else
                    {
                        // Export inventory to list.
                        if (ExportInventory)
                        {
                            if (!ExportPlayerItems.ContainsKey("itemSlot50"))
                            {
                                ExportPlayerItems.Add("itemSlot50-ID", MemLib.ReadInt(slot50Item));
                                ExportPlayerItems.Add("itemSlot50-Amount", MemLib.ReadInt(slot50Amount));
                                ExportPlayerItems.Add("itemSlot50-Variation", MemLib.ReadInt(slot50Variation));
                            }
                        }
                        else
                        {
                            // First Load
                            type = MemLib.ReadInt(slot50Item);
                            variation = MemLib.ReadInt(slot50Variation);

                            // Load Picture
                            // Set image to null if type is zero.
                            if (type.ToString() == "0")
                            {
                                pictureBox50.Image = null;
                            }
                            else if (pictureBox50.Image == null)
                            {
                                // Get Picture
                                try
                                {
                                    // Check if image plus variation exists.
                                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                                    {
                                        pictureBox50.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                        pictureBox50.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                                    {
                                        // Image without variation exists.
                                        pictureBox50.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                        pictureBox50.SizeMode = PictureBoxSizeMode.Zoom;
                                    }
                                    else
                                    {
                                        // No image found.
                                        pictureBox50.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                        pictureBox50.SizeMode = PictureBoxSizeMode.Zoom;

                                        // Do debug information.
                                        if (Array.Exists(richTextBox3.Lines, element => element == ("ItemSlot: 50 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot50Amount) + " | Variation: " + (MemLib.ReadInt(slot50Variation)))) == false) // Check if entree exists already.
                                        {
                                            richTextBox3.AppendText("ItemSlot: 50 | ItemID: " + type + " | Amount: " + MemLib.ReadInt(slot50Amount) + " | Variation: " + (MemLib.ReadInt(slot50Variation)) + Environment.NewLine); // Record the midding values.
                                        }
                                    }

                                    // Draw item amount.
                                    using (Font font = new Font("Arial", 24f))
                                    using (Graphics G = Graphics.FromImage(pictureBox50.Image))
                                    using (GraphicsPath gp = new GraphicsPath())
                                    {
                                        // Do drawling actions.
                                        gp.AddString(MemLib.ReadInt(slot50Amount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                        G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                        G.FillPath(new SolidBrush(Color.White), gp);
                                    }
                                    pictureBox50.Invalidate(); // Reload picturebox.
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }

                    // Do some textbox scrolling.
                    richTextBox3.ScrollToCaret();

                    // Do some information stuff.
                    if (GetItemInfo)
                    {
                        infoType = MemLib.ReadInt(slot50Item);
                        infoAmount = MemLib.ReadInt(slot50Amount);
                        infoVariant = MemLib.ReadInt(slot50Variation);
                    }
                }
            }
            catch (Exception)
            {
                // Do nothing.
            }

            #endregion

            // Save the player Json.
            if (ExportInventory)
            {
                string playerItems = JsonConvert.SerializeObject(ExportPlayerItems, Formatting.Indented);
                System.IO.File.WriteAllText(ExportPlayerName, playerItems);
            }

            // Gather item info and announce.
            #region Announce Item Info

            // Ammounce the information.
            if (GetItemInfo)
            {
                // Get the name of the item.
                string baseItemName = "";
                string baseIngrdient1Name = "";
                string baseIngrdient2Name = "";

                // Get base item name.
                if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == infoType.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (infoVariant == 0 ? 0 : infoVariant).ToString()) != null)
                {
                    baseItemName = Path.GetFileName(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == infoType.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (infoVariant == 0 ? 0 : infoVariant).ToString())).Split(',')[0];
                }
                else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == infoType.ToString()) != null)
                {
                    baseItemName = Path.GetFileName(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == infoType.ToString())).Split(',')[0];
                }
                else
                {
                    // Check if the item id is 0.
                    if (infoType == 0)
                    {
                        baseItemName = "Empty";
                    }
                    else
                    {
                        baseItemName = "UnkownItem";
                    }
                }
                // Check if the items variant is an lengh of 8. 
                if (infoVariant.ToString().Length == 8)
                {
                    // Get base item ingrdient 1 name.
                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == infoVariant.ToString().Substring(0, infoVariant.ToString().Length / 2).ToString()) != null)
                    {
                        baseIngrdient1Name = Path.GetFileName(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == infoVariant.ToString().Substring(0, infoVariant.ToString().Length / 2).ToString())).Split(',')[0];
                    }
                    else
                    {
                        // Check if the item id is 0.
                        if (infoType == 0)
                        {
                            baseIngrdient1Name = "Empty";
                        }
                        else
                        {
                            baseIngrdient1Name = "UnkownItem";
                        }
                    }
                    // Get base item ingrdient 2 name.
                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == infoVariant.ToString().Substring(infoVariant.ToString().Length / 2).ToString()) != null)
                    {
                        baseIngrdient2Name = Path.GetFileName(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == infoVariant.ToString().Substring(infoVariant.ToString().Length / 2).ToString())).Split(',')[0];
                    }
                    else
                    {
                        // Check if the item id is 0.
                        if (infoType == 0)
                        {
                            baseIngrdient2Name = "Empty";
                        }
                        else
                        {
                            baseIngrdient2Name = "UnkownItem";
                        }
                    }
                }

                // Check if the items variant is an lengh of 8. 
                string itemMessage = "";
                if (infoVariant.ToString().Length == 8)
                {
                    itemMessage = "Inventory Slot " + itemSlot + "'s Item Info: " + Environment.NewLine + Environment.NewLine + "Name: " + baseItemName + Environment.NewLine + "Base ID: " + infoType + Environment.NewLine + "Amount: " + infoAmount + Environment.NewLine + Environment.NewLine + "Variant IDs:" + Environment.NewLine + "- Ingrdient1: " + baseIngrdient1Name + " [" + infoVariant.ToString().Substring(0, infoVariant.ToString().Length / 2) + "]" + Environment.NewLine + "- Ingrdient2: " + baseIngrdient2Name + " [" + infoVariant.ToString().Substring(infoVariant.ToString().Length / 2) + "]";
                }
                else
                {
                    itemMessage = "Inventory Slot " + itemSlot + "'s Item Info: " + Environment.NewLine + Environment.NewLine + "Name: " + baseItemName + Environment.NewLine + "ID: " + infoType + Environment.NewLine + "Amount: " + infoAmount + Environment.NewLine + Environment.NewLine + "Variant ID: " + infoVariant;
                }

                // Display informational messagebox.
                MessageBox.Show(itemMessage, "Item Information:", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            #endregion // End announce item info.

            #endregion // End adding items upon editing.

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox1.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox1.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox1.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox2.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox2.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox2.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox3.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox3.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox3.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox4.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox4.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox4.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox5.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox5.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox5.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox5.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox5.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox5.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox6.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox6.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox6.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox6.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox6.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox6.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox7.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox7.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox7.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox7.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox7.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox7.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox8.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox8.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox8.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox8.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox8.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox8.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox9.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox9.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox9.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox9.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox9.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox9.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox10.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox10.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox10.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox10.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox10.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox10.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox11.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox11.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox11.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox11.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox11.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox11.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox12.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox12.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox12.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox12.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox12.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox12.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox13.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox13.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox13.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox13.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox13.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox13.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox14.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox14.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox14.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox14.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox14.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox14.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox15.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox15.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox15.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox15.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox15.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox15.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox16.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox16.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox16.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox16.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox16.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox16.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox17.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox17.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox17.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox17.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox17.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox17.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox18.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox18.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox18.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox18.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox18.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox18.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox19.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox19.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox19.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox19.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox19.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox19.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox20.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox20.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox20.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox20.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox20.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox20.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox21.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox21.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox21.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox21.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox21.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox21.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox22.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox22.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox22.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox22.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox22.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox22.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox23.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox23.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox23.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox23.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox23.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox23.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox24.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox24.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox24.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox24.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox24.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox24.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox25.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox25.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox25.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox25.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox25.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox25.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox26.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox26.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox26.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox26.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox26.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox26.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox27.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox27.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox27.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox27.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox27.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox27.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox28.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox28.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox28.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox28.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox28.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox28.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox29.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox29.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox29.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox29.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox29.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox29.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox30.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox30.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox30.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox30.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox30.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox30.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 31 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox31.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 31, type: type, amount: amount, variation: variation);

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
                        pictureBox31.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox31.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox31.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox31.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox31.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox31.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox31.SizeMode = PictureBoxSizeMode.Zoom;
                            }

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox31.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox31.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 32 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox32.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 32, type: type, amount: amount, variation: variation);

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
                        pictureBox32.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox32.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox32.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox32.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox32.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox32.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox32.SizeMode = PictureBoxSizeMode.Zoom;
                            }

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox32.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox32.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 33 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox33.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 33, type: type, amount: amount, variation: variation);

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
                        pictureBox33.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox33.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox33.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox33.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox33.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox33.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox33.SizeMode = PictureBoxSizeMode.Zoom;
                            }

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox33.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox33.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 34 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox34.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 34, type: type, amount: amount, variation: variation);

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
                        pictureBox34.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox34.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox34.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox34.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox34.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox34.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox34.SizeMode = PictureBoxSizeMode.Zoom;
                            }

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox34.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox34.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 35 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox35.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 35, type: type, amount: amount, variation: variation);

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
                        pictureBox35.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox35.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox35.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox35.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox35.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox35.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox35.SizeMode = PictureBoxSizeMode.Zoom;
                            }

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox35.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox35.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 36 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox36.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 36, type: type, amount: amount, variation: variation);

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
                        pictureBox36.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox36.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox36.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox36.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox36.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox36.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox36.SizeMode = PictureBoxSizeMode.Zoom;
                            }

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox36.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox36.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 37 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox37.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 37, type: type, amount: amount, variation: variation);

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
                        pictureBox37.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox37.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox37.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox37.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox37.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox37.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox37.SizeMode = PictureBoxSizeMode.Zoom;
                            }

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox37.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox37.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 38 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox38.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 38, type: type, amount: amount, variation: variation);

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
                        pictureBox38.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox38.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox38.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox38.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox38.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox38.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox38.SizeMode = PictureBoxSizeMode.Zoom;
                            }

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox38.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox38.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 39 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox39.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 39, type: type, amount: amount, variation: variation);

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
                        pictureBox39.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox39.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox39.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox39.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox39.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox39.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox39.SizeMode = PictureBoxSizeMode.Zoom;
                            }

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox39.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox39.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 40 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox40.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 40, type: type, amount: amount, variation: variation);

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
                        pictureBox40.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox40.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox40.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox40.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox40.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox40.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox40.SizeMode = PictureBoxSizeMode.Zoom;
                            }

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox40.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox40.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 41 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox41.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 41, type: type, amount: amount, variation: variation);

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
                        pictureBox41.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox41.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox41.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox41.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox41.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox41.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox41.SizeMode = PictureBoxSizeMode.Zoom;
                            }

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox41.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox41.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 42 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox42.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 42, type: type, amount: amount, variation: variation);

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
                        pictureBox42.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox42.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox42.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox42.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox42.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox42.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox42.SizeMode = PictureBoxSizeMode.Zoom;
                            }

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox42.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox42.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 43 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox43.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 43, type: type, amount: amount, variation: variation);

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
                        pictureBox43.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox43.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox43.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox43.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox43.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox43.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox43.SizeMode = PictureBoxSizeMode.Zoom;
                            }

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox43.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox43.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 44 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox44.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 44, type: type, amount: amount, variation: variation);

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
                        pictureBox44.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox44.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox44.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox44.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox44.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox44.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox44.SizeMode = PictureBoxSizeMode.Zoom;
                            }

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox44.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox44.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 45 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox45.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 45, type: type, amount: amount, variation: variation);

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
                        pictureBox45.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox45.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox45.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox45.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox45.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox45.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox45.SizeMode = PictureBoxSizeMode.Zoom;
                            }

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox45.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox45.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 46 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox46.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 46, type: type, amount: amount, variation: variation);

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
                        pictureBox46.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox46.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox46.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox46.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox46.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox46.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox46.SizeMode = PictureBoxSizeMode.Zoom;
                            }

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
                            pictureBox46.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 47 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox4.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 47, type: type, amount: amount, variation: variation);

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
                        pictureBox47.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox47.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox47.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox47.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox47.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox47.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox47.SizeMode = PictureBoxSizeMode.Zoom;
                            }

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox47.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox47.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 48 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox48.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 48, type: type, amount: amount, variation: variation);

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
                        pictureBox48.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox48.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox48.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox48.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox48.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox48.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox48.SizeMode = PictureBoxSizeMode.Zoom;
                            }

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox48.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox48.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 49 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox49.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 49, type: type, amount: amount, variation: variation);

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
                        pictureBox49.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox49.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox49.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox49.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox49.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox49.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox49.SizeMode = PictureBoxSizeMode.Zoom;
                            }

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox49.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox49.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
            if (!loadInventory && !GetItemInfo && (itemSlot == 50 || CycleAll || AddToEmpty))
            {
                // If slot is empty, add item to invintory.
                if (AddToEmpty && pictureBox50.Image == null && !emptySlotFound)
                {
                    // Add item to inventory.
                    AddItemToInv(itemSlot: 50, type: type, amount: amount, variation: variation);

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
                        pictureBox50.Image = null;
                    }
                    else
                    {
                        // Get Picture
                        try
                        {
                            // Check if image plus variation exists.
                            if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()) != null)
                            {
                                pictureBox50.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (variation == 0 ? 0 : variation).ToString()))); // Check if file matches current type, set it.
                                pictureBox50.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString()) != null)
                            {
                                // Image without variation exists.
                                pictureBox50.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current type, set it.
                                pictureBox50.SizeMode = PictureBoxSizeMode.Zoom;
                            }
                            else
                            {
                                // No image found.
                                pictureBox50.Image = CoreKeepersWorkshop.Properties.Resources.UnknownItem;
                                pictureBox50.SizeMode = PictureBoxSizeMode.Zoom;
                            }

                            // Draw item amount.
                            using (Font font = new Font("Arial", 24f))
                            using (Graphics G = Graphics.FromImage(pictureBox50.Image))
                            using (GraphicsPath gp = new GraphicsPath())
                            {
                                // Do drawling actions.
                                gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                G.FillPath(new SolidBrush(Color.White), gp);
                            }
                            pictureBox50.Invalidate(); // Reload picturebox.
                        }
                        catch (Exception)
                        {
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

            // Re-enable button.
            button1.Enabled = true;

            // Rehide the progressbar.
            progressBar2.Visible = false;
        }

        // Previous address button.
        private void Button20_Click(object sender, EventArgs e)
        {
            // Reset progress bar.
            progressBar2.Value = 0;

            // Subtract from the use address if its not one.
            useAddress = (useAddress == 1) ? 1 : useAddress - 1;

            // Update the rich textbox.
            richTextBox1.Text = "Addresses Loaded: 0";
            foreach (long res in AoBScanResultsInventory)
            {
                if (richTextBox1.Text == "Addresses Loaded: 0")
                {
                    richTextBox1.Text = "Addresses Loaded: " + AoBScanResultsInventory.Count().ToString() + ", Selected: " + useAddress + ", [" + res.ToString("X").ToString();
                }
                else
                {
                    richTextBox1.Text += ", " + res.ToString("X").ToString();
                }
            }
            richTextBox1.Text += "]";

            // Load addresses.
            AddItemToInv(loadInventory: true);
        }

        // Next address button.
        private void Button21_Click(object sender, EventArgs e)
        {
            // Reset progress bar.
            progressBar2.Value = 0;

            // Add to the use address if its not the max.
            useAddress = (AoBScanResultsInventory != null && useAddress == AoBScanResultsInventory.Count()) ? AoBScanResultsInventory.Count() : useAddress + 1;

            // Update the rich textbox.
            richTextBox1.Text = "Addresses Loaded: 0";
            foreach (long res in AoBScanResultsInventory)
            {
                if (richTextBox1.Text == "Addresses Loaded: 0")
                {
                    richTextBox1.Text = "Addresses Loaded: " + AoBScanResultsInventory.Count().ToString() + ", Selected: " + useAddress + ", [" + res.ToString("X").ToString();
                }
                else
                {
                    richTextBox1.Text += ", " + res.ToString("X").ToString();
                }
            }
            richTextBox1.Text += "]";

            // Load addresses.
            AddItemToInv(loadInventory: true);
        }

        // Reload Inventory.
        private void Button2_Click(object sender, EventArgs e)
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
        private void Button3_Click(object sender, EventArgs e)
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
        private void PictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            // Stop infinite recourses if enabled.
            if (playersInfiniteResourcesTimer.Enabled)
            {
                // Stop timer.
                playersInfiniteResourcesTimer.Stop();

                // Clear the list of items.
                inventoryInformation = null;

                // Toggle slider.
                siticoneWinToggleSwith7.CheckedChanged -= SiticoneWinToggleSwith7_CheckedChanged;
                siticoneWinToggleSwith7.Checked = false;
                siticoneWinToggleSwith7.CheckedChanged += SiticoneWinToggleSwith7_CheckedChanged;
            }

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
                    int itemVariation = frm2.GetItemVeriationFromList() == 0 ? 0 : (frm2.GetItemVeriationFromList()); // If variation is not zero, add offset.
                    bool wasAborted = frm2.GetUserCancledTask();
                    bool itemOverwrite = frm2.GetSelectedOverwriteTask();
                    frm2.Close();

                    // Check if user closed the form
                    if (wasAborted) { return; };

                    // Spawn the item.
                    AddItemToInv(slotNumber, type: itemType, amount: itemAmount, variation: itemVariation, Overwrite: itemOverwrite);

                    // Reload Inventory. Added: v1.3.4.5.
                    AddItemToInv(loadInventory: true);
                }
                else if (e.Button == MouseButtons.Middle) // Get item stats.
                {
                    // Get the picturebox selected number.
                    int slotNumber = int.Parse(pic.Name.Replace("pictureBox", ""));

                    // Get item stats.
                    AddItemToInv(slotNumber, GetItemInfo: true);

                    // Reload Inventory. Added: v1.3.4.5.
                    AddItemToInv(loadInventory: true);
                }
                else if (e.Button == MouseButtons.Right) // Change item amount value.
                {
                    // Open the process and check if it was successful before the AoB scan.
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

                    // Get the picturebox selected number.
                    int slotNumber = int.Parse(pic.Name.Replace("pictureBox", ""));

                    // Get item slot values.
                    int[] itemInfo = GetSlotInfo(slotNumber);

                    // Save some form settings.
                    CoreKeepersWorkshop.Properties.Settings.Default.InfoID = itemInfo[0];
                    CoreKeepersWorkshop.Properties.Settings.Default.InfoAmount = Math.Abs(itemInfo[1]); // Fix negitive numbers throwing an exception. // Fix v1.3.4.4.
                    CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation = itemInfo[2] == 0 ? 0 : (itemInfo[2]); // Ensure variant gets translated correctly.

                    // Spawn item picker window.
                    ItemEditor frm3 = new ItemEditor();
                    DialogResult dr = frm3.ShowDialog(this);

                    // Get returned item from picker.
                    int itemType = frm3.GetItemTypeFromList();
                    int itemAmount = frm3.GetItemAmountFromList();
                    int itemVariation = frm3.GetItemVeriationFromList() == 0 ? 0 : (frm3.GetItemVeriationFromList()); // If variation is not zero, add offset.
                    bool wasAborted = frm3.GetUserCancledTask();
                    // bool itemOverwrite = frm3.GetSelectedOverwriteTask();
                    frm3.Close();

                    // Check if user closed the form
                    if (wasAborted) { return; };

                    // Edit the item.
                    AddItemToInv(slotNumber, type: itemType, amount: itemAmount, variation: itemVariation, Overwrite: true);

                    // Reload Inventory. Added: v1.3.4.5.
                    AddItemToInv(loadInventory: true);
                }
            }
        }

        #endregion // Click Events

        #region Get itemSlot Values

        // Get item amount.
        public int[] GetSlotInfo(int itemSlot)
        {
            // Define main string.
            int[] itemInfo = new int[3];

            // Define some varibles for item info.
            int infoType;
            int infoAmount;
            int infoVariant;

            // Select the inventory to use.
            var res = AoBScanResultsInventory.ElementAt(useAddress - 1);

            // Get address from loop.
            // Base address was moved 9 bits.
            string baseAddress = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("7", NumberStyles.Integer)).ToString("X");

            if (itemSlot == 1)
            {
                infoType = (int)MemLib.ReadUInt(baseAddress);
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("4", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 2)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("16", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("20", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("24", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 3)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("32", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("36", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("40", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 4)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("48", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("52", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("56", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 5)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("64", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("68", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("72", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 6)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("80", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("84", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("88", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 7)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("96", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("100", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("104", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 8)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("112", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("116", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("120", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 9)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("128", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("132", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("136", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 10)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("144", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("148", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("152", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 11)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("160", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("164", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("168", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 12)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("176", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("180", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("184", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 13)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("192", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("196", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("200", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 14)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("208", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("212", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("216", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 15)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("224", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("228", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("232", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 16)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("240", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("244", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("248", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 17)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("256", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("260", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("264", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 18)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("272", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("276", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("280", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 19)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("288", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("292", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("296", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 20)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("304", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("308", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("312", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 21)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("320", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("324", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("328", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 22)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("336", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("340", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("344", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 23)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("352", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("356", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("360", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 24)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("368", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("372", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("376", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 25)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("384", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("388", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("392", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 26)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("400", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("404", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("408", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 27)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("416", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("420", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("424", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 28)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("432", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("436", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("440", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 29)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("448", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("452", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("456", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 30)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("464", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("468", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("472", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 31)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("480", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("484", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("488", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 32)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("496", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("500", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("504", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 33)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("512", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("516", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("520", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 34)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("528", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("532", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("536", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 35)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("544", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("548", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("552", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 36)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("560", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("564", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("568", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 37)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("576", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("580", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("584", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 38)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("592", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("596", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("600", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 39)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("608", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("612", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("616", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 40)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("624", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("628", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("632", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 41)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("640", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("644", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("648", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 42)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("656", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("660", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("664", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 43)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("672", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("676", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("680", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 44)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("688", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("692", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("696", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 45)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("704", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("708", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("712", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 46)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("720", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("724", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("728", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 47)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("736", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("740", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("744", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 48)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("752", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("756", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("760", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 49)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("768", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("772", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("776", NumberStyles.Integer)).ToString("X"));
            }
            else if (itemSlot == 50)
            {
                infoType = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("784", NumberStyles.Integer)).ToString("X"));
                infoAmount = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("788", NumberStyles.Integer)).ToString("X"));
                infoVariant = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("792", NumberStyles.Integer)).ToString("X"));
            }
            else // Prevent out of range errors.
            {
                infoType = 0;
                infoAmount = 0;
                infoVariant = 0;
            }

            // Define item info string.
            itemInfo[0] = infoType;
            itemInfo[1] = infoAmount;
            itemInfo[2] = infoVariant;

            // Return value.
            return itemInfo;
        }

        #endregion // Get itemSlot Values

        #endregion // End Inventory Region

        #region Player Tab

        #region Change Player Name

        // Change player name.
        private async void Button4_Click(object sender, EventArgs e)
        {
            // Ensure properties are filled.
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                // Display error message.
                MessageBox.Show("You must type your playername and a new name!!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
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
                builder.Append(Convert.ToInt64(c).ToString("X"));
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
            ChangePlayersName(textBox2.Text, textBox2.Text.Length);
        }

        // this function is async, which means it does not block other code.
        public void ChangePlayersName(string NewName, int NewLengh)
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

        #endregion // End change playername.

        #region Import / Export

        // Import a player file.
        private void Button5_Click(object sender, EventArgs e)
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
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Player File|*.ckplayer"
            };

            // Ensure the user chose a file.
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                // Reset progress bar.
                progressBar1.Step = 90;
                progressBar1.Value = 0;

                // Define playername
                // string playerName = ofd.SafeFileName;

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
                                            AddItemToInv(itemSlot: ItemSlotCount, type: itemID, amount: itemAmount, variation: itemVariation, Overwrite: true);
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
        private void Button6_Click(object sender, EventArgs e)
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

        #endregion // End import & export.

        #region Player Tool Addresses

        // Get player address.
        private void Button10_Click(object sender, EventArgs e)
        {
            // Reset progress bar.
            progressBar5.Value = 0;

            // Load addresses.
            GetPlayerToolsAddresses();
        }

        public async void GetPlayerToolsAddresses()
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Name button to indicate loading.
            button10.Text = "Loading...";

            // Disable button to prevent spamming.
            // button10.Enabled = false;
            groupBox7.Enabled = false;

            // Reset textbox.
            richTextBox6.Text = "Addresses Loaded: 0";

            // Offset the progress bar to show it's working.
            progressBar5.Visible = true;
            progressBar5.Maximum = 100;
            progressBar5.Step = 45;
            progressBar5.Value = 10;

            // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
            // Depreciated Address 08Feb23: ?? 99 D9 3F ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 99 D9 3F
            AoBScanResultsPlayerTools = await MemLib.AoBScan("?? 99 D9 3F ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 99 D9 3F", true, true);

            // If the count is zero, the scan had an error.
            if (AoBScanResultsPlayerTools.Count() < 1)
            {
                // Reset textbox.
                richTextBox6.Text = "Addresses Loaded: 0";

                // Reset progress bar.
                progressBar5.Value = 0;
                progressBar5.Visible = false;

                // Rename button back to defualt.
                button10.Text = "Get Addresses";

                // Re-enable button.
                //button10.Enabled = true;
                groupBox7.Enabled = true;

                // Reset aob scan results
                AoBScanResultsPlayerTools = null;

                // Display error message.
                MessageBox.Show("You must be standing at the core's entrance!!\r\rTIP: Press 'W' & 'D' keys when at the core's entrance.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Update richtextbox with found addresses.
            foreach (long res in AoBScanResultsPlayerTools)
            {
                if (richTextBox6.Text == "Addresses Loaded: 0")
                {
                    richTextBox6.Text = "Player Addresses Loaded: " + AoBScanResultsPlayerTools.Count().ToString() + " [" + res.ToString("X").ToString();
                }
                else
                {
                    richTextBox6.Text += ", " + res.ToString("X").ToString();
                }
            }
            richTextBox6.Text += "]";

            // Re-enable button.
            // button10.Enabled = true;
            groupBox7.Enabled = true;

            // Rename button back to defualt.
            button10.Text = "Get Addresses";

            // Complete progress bar.
            progressBar5.Value = 100;

            // Hide progressbar.
            progressBar5.Visible = false;
        }
        #endregion // End get player addresses region.

        #region Player Position

        // Enable player xy tool.
        readonly System.Timers.Timer playersPositionTimer = new System.Timers.Timer();
        private void SiticoneWinToggleSwith1_CheckedChanged(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                siticoneWinToggleSwith1.CheckedChanged -= SiticoneWinToggleSwith1_CheckedChanged;
                siticoneWinToggleSwith1.Checked = false;
                siticoneWinToggleSwith1.CheckedChanged += SiticoneWinToggleSwith1_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsPlayerTools == null)
            {
                // Toggle slider.
                siticoneWinToggleSwith1.CheckedChanged -= SiticoneWinToggleSwith1_CheckedChanged;
                siticoneWinToggleSwith1.Checked = false;
                siticoneWinToggleSwith1.CheckedChanged += SiticoneWinToggleSwith1_CheckedChanged;

                MessageBox.Show("You need to first scan for the Player addresses!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if the slider was not yet checked.
            if (siticoneWinToggleSwith1.Checked)
            {
                // Start the timed events.
                playersPositionTimer.Interval = 100; // Custom intervals.
                playersPositionTimer.Elapsed += new ElapsedEventHandler(PlayersPositionTimedEvent);
                playersPositionTimer.Start();

                // Update consoile with the status.
                richTextBox5.AppendText("[PlayerPosition] Player position has been enabled." + Environment.NewLine);
                richTextBox5.ScrollToCaret();
            }
            else
            {
                // Disable player position.
                // Stop the timers.
                playersPositionTimer.Stop();

                // Change appllication text back to defualt.
                this.Text = "CoreKeeper's Workshop";

                // Update consoile with the status.
                richTextBox5.AppendText("[PlayerPosition] Player position has been disabled." + Environment.NewLine);
                richTextBox5.ScrollToCaret();
            }
        }

        // Players position timer.
        private void PlayersPositionTimedEvent(Object source, ElapsedEventArgs e)
        {
            // Get the addresses.
            string positionX = BigInteger.Subtract(BigInteger.Parse(AoBScanResultsPlayerTools.Last().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");
            string positionY = AoBScanResultsPlayerTools.Last().ToString("X");

            // Convert values to number.
            string playerPositionX = MemLib.ReadFloat(positionX).ToString();
            string playerPositionY = (MemLib.ReadFloat(positionY) - 1).ToString(); // Correct the offset. 

            // Change the applications tittle based on minimization and tab pages. 
            if (isMinimized || tabControl1.SelectedTab == tabPage5) // Tab five is smaller.
            {
                // Change text based on minimized window.
                this.Text = "Pos [X: " + playerPositionX + " Y: " + playerPositionY + "]";
            }
            else
            {
                // Change text based on maximized window.
                this.Text = "CoreKeeper's Workshop | PlayersPos [X: " + playerPositionX + " Y: " + playerPositionY + "]";
            }
        }
        #endregion // End player positon.

        #region Godmode

        // Toggle godmode.
        readonly System.Timers.Timer playersGodmodeTimer = new System.Timers.Timer();
        string godmodeAddress = "0";
        private void SiticoneWinToggleSwith2_CheckedChanged(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                siticoneWinToggleSwith2.CheckedChanged -= SiticoneWinToggleSwith2_CheckedChanged;
                siticoneWinToggleSwith2.Checked = false;
                siticoneWinToggleSwith2.CheckedChanged += SiticoneWinToggleSwith2_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsPlayerTools == null)
            {
                // Toggle slider.
                siticoneWinToggleSwith2.CheckedChanged -= SiticoneWinToggleSwith2_CheckedChanged;
                siticoneWinToggleSwith2.Checked = false;
                siticoneWinToggleSwith2.CheckedChanged += SiticoneWinToggleSwith2_CheckedChanged;

                MessageBox.Show("You need to first scan for the Player addresses!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get the addresses.
            godmodeAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.Last().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("2184", NumberStyles.Integer)).ToString("X");

            // Check if the slider was not yet checked.
            if (siticoneWinToggleSwith2.Checked)
            {
                // Slider is being toggled on.
                // Start the timed events.
                playersGodmodeTimer.Interval = 1; // Custom intervals.
                playersGodmodeTimer.Elapsed += new ElapsedEventHandler(PlayersGodmodeTimedEvent);
                playersGodmodeTimer.Start();
            }
            else
            {
                // Slider is being toggled off.
                // Stop the timers.
                playersGodmodeTimer.Stop();
            }
        }

        // Players position timer.
        private void PlayersGodmodeTimedEvent(Object source, ElapsedEventArgs e)
        {
            // Write value.
            MemLib.WriteMemory(godmodeAddress, "int", "100000"); // Overwrite new value.
        }
        #endregion // End godmode.

        #region Player Speed

        // Change player speed.
        string originalSpeed = "336"; // Original max speed.
        private void SiticoneWinToggleSwith3_CheckedChanged(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                siticoneWinToggleSwith3.CheckedChanged -= SiticoneWinToggleSwith3_CheckedChanged;
                siticoneWinToggleSwith3.Checked = false;
                siticoneWinToggleSwith3.CheckedChanged += SiticoneWinToggleSwith3_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsPlayerTools == null)
            {
                // Toggle slider.
                siticoneWinToggleSwith3.CheckedChanged -= SiticoneWinToggleSwith3_CheckedChanged;
                siticoneWinToggleSwith3.Checked = false;
                siticoneWinToggleSwith3.CheckedChanged += SiticoneWinToggleSwith3_CheckedChanged;

                MessageBox.Show("You need to first scan for the Player addresses!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get the addresses.
            string playerSpeedAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.Last().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("728", NumberStyles.Integer)).ToString("X");

            // Check if the slider was not yet checked.
            if (siticoneWinToggleSwith3.Checked)
            {
                // Disable numericupdown.
                numericUpDown3.Enabled = false;

                // Slider is being toggled on.
                // Read current value.
                originalSpeed = MemLib.ReadFloat(playerSpeedAddress).ToString();

                // Write new value.
                MemLib.WriteMemory(playerSpeedAddress, "float", (numericUpDown3.Value + ".0").ToString()); // Overwrite new value.
            }
            else
            {
                // Slider is being toggled off.
                // Disable numericupdown.
                numericUpDown3.Enabled = true;

                // Write value back to original.
                // Write value.
                MemLib.WriteMemory(playerSpeedAddress, "float", originalSpeed); // Overwrite new value.
            }
        }
        #endregion // End player speed.

        #region Noclip

        // Toggle godmode.
        readonly System.Timers.Timer playersNoclipTimer = new System.Timers.Timer();
        string noclipAddress = "0";
        string noclipOriginalValue = "2";
        private void SiticoneWinToggleSwith4_CheckedChanged(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                siticoneWinToggleSwith4.CheckedChanged -= SiticoneWinToggleSwith4_CheckedChanged;
                siticoneWinToggleSwith4.Checked = false;
                siticoneWinToggleSwith4.CheckedChanged += SiticoneWinToggleSwith4_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsPlayerTools == null)
            {
                // Toggle slider.
                siticoneWinToggleSwith4.CheckedChanged -= SiticoneWinToggleSwith4_CheckedChanged;
                siticoneWinToggleSwith4.Checked = false;
                siticoneWinToggleSwith4.CheckedChanged += SiticoneWinToggleSwith4_CheckedChanged;

                MessageBox.Show("You need to first scan for the Player addresses!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get the addresses.
            // Old alternitive address: 124 // Fix 1.3.4.6 15Jan23. // Reverted 1.3.4.9 09Feb23 - old address: 116.
            noclipAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.Last().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("124", NumberStyles.Integer)).ToString("X");

            // Check if the slider was not yet checked.
            if (siticoneWinToggleSwith4.Checked)
            {
                // Slider is being toggled on.
                // Get original value.
                noclipOriginalValue = MemLib.ReadUInt(noclipAddress).ToString();

                // Start the timed events.
                playersNoclipTimer.Interval = 100;
                playersNoclipTimer.Elapsed += new ElapsedEventHandler(PlayersNoclipTimedEvent);
                playersNoclipTimer.Start();
            }
            else
            {
                // Slider is being toggled off.
                // Stop the timers.
                playersNoclipTimer.Stop();

                // Write value.
                MemLib.WriteMemory(noclipAddress, "int", noclipOriginalValue); // Overwrite new value.
            }
        }

        // Players position timer.
        private void PlayersNoclipTimedEvent(Object source, ElapsedEventArgs e)
        {
            // Check if noclip is activated or not.
            if (!IsKeyPressed(0x20))
            {
                // Write value.
                MemLib.WriteMemory(noclipAddress, "int", noclipOriginalValue); // Overwrite new value.
            }
            else
            {
                // Write value.
                MemLib.WriteMemory(noclipAddress, "int", "0"); // Overwrite new value.
            }
        }
        #endregion // End noclip.

        #region No Hunger

        // Toggle godmode.
        readonly System.Timers.Timer playersNoHungerTimer = new System.Timers.Timer();
        public IEnumerable<long> AoBScanResultsNoHunger1Tools = null;
        private async void SiticoneWinToggleSwith5_CheckedChanged(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                siticoneWinToggleSwith5.CheckedChanged -= SiticoneWinToggleSwith5_CheckedChanged;
                siticoneWinToggleSwith5.Checked = false;
                siticoneWinToggleSwith5.CheckedChanged += SiticoneWinToggleSwith5_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if the slider was not yet checked.
            if (siticoneWinToggleSwith5.Checked)
            {
                // Slider is being toggled on.
                // Name button to indicate loading.
                label14.Text = "- Loading..";

                // Offset the progress bar to show it's working.
                progressBar5.Visible = true;
                progressBar5.Maximum = 100;
                progressBar5.Value = 10;

                // Check if we need to rescan the addresses or not.
                if (AoBScanResultsNoHunger1Tools != null)
                {
                    string foodAddress0 = BigInteger.Subtract(BigInteger.Parse(AoBScanResultsNoHunger1Tools.Last().ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("14", NumberStyles.HexNumber)).ToString("X");
                    int currentFood = MemLib.ReadInt(foodAddress0);

                    // Check if we need to rescan food or not.
                    if (currentFood < 1 || currentFood > 100)
                    {
                        // Rescan food address.
                        AoBScanResultsNoHunger1Tools = await MemLib.AoBScan("01 00 00 00 ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 4? 44 44 3F ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 08 00 00 00", true, true);
                    }
                }
                else
                {
                    // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
                    // Depreciated Address 17Dec22: 01 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 48 44 44 3F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 AC 00 00 00 01 00 00 00 01 00 00 00
                    // Depreciated Address 09Jan23: 01 00 00 00 ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 4? 44 44 3F
                    // Depreciated Address 08Feb23: 01 00 00 00 ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 4? 44 44 3F ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 08 00 00 00
                    AoBScanResultsNoHunger1Tools = await MemLib.AoBScan("01 00 00 00 ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 4? 44 44 3F ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 08 00 00 00", true, true);
                }

                // If the count is zero, the scan had an error.
                if (AoBScanResultsNoHunger1Tools.Count() == 0)
                {
                    // Name label to indicate loading.
                    label14.Text = "- Infinite Food";

                    // Reset progress bar.
                    progressBar5.Value = 0;
                    progressBar5.Visible = false;

                    // Reset aob scan results
                    AoBScanResultsNoHunger1Tools = null;

                    // Toggle slider.
                    siticoneWinToggleSwith5.CheckedChanged -= SiticoneWinToggleSwith5_CheckedChanged;
                    siticoneWinToggleSwith5.Checked = false;
                    siticoneWinToggleSwith5.CheckedChanged += SiticoneWinToggleSwith5_CheckedChanged;

                    // Display error message.
                    MessageBox.Show("There was an issue trying to fetch the hunger addresses." + Environment.NewLine + "Try reloading the game!!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Update richtextbox with found addresses.
                richTextBox6.Text = "Addresses Loaded: 0"; // Reset textbox.
                foreach (long res in AoBScanResultsNoHunger1Tools)
                {
                    if (richTextBox6.Text == "Addresses Loaded: 0")
                    {
                        richTextBox6.Text = "Hunger Addresses Loaded: " + AoBScanResultsNoHunger1Tools.Count() + " [" + res.ToString("X").ToString();
                    }
                    else
                    {
                        richTextBox6.Text += ", " + res.ToString("X").ToString();
                    }
                }
                richTextBox6.Text += "]";

                // Complete progress bar.
                progressBar5.Value = 100;
                progressBar5.Visible = false;

                // Rename label to defualt text.
                label14.Text = "- Infinite Food";

                // Start the timed events.
                playersNoHungerTimer.Interval = 1; // Custom intervals.
                playersNoHungerTimer.Elapsed += new ElapsedEventHandler(PlayersNoHungerTimedEvent);
                playersNoHungerTimer.Start();
            }
            else
            {
                // Slider is being toggled off.
                // Reset label name.
                label14.Text = "- Infinite Food";

                // Complete progress bar.
                progressBar5.Value = 100;
                progressBar5.Visible = false;

                // Stop the timers.
                playersNoHungerTimer.Stop();
            }
        }

        // Players position timer.
        private void PlayersNoHungerTimedEvent(Object source, ElapsedEventArgs e)
        {
            // Update richtextbox with found addresses.
            foreach (long res in AoBScanResultsNoHunger1Tools)
            {
                // Get the hunger addresses.
                string foodAddress1 = BigInteger.Subtract(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("14", NumberStyles.HexNumber)).ToString("X");

                // Write value.
                MemLib.WriteMemory(foodAddress1, "int", "100"); // Overwrite new value.
            }
        }
        #endregion // End no hunger.

        #region Buff Editor

        // Change the players buff.
        private async void Button12_Click(object sender, EventArgs e)
        {
            // Check if the combobox has a value and is not null.
            if (comboBox1.Text == "")
            {
                MessageBox.Show("The buff type cannot be null!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Change button to indicate loading.
            button12.Text = "Working";
            button12.Enabled = false;

            // Find the memory addresses.
            // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
            AoBScanResultsPlayerBuffs = await MemLib.AoBScan("01 00 00 00 00 00 70 42 04 00 00 00 00 00 00 00 ?? ?? ?? ?? 00 00 00 00", true, true);

            // If the count is zero, the scan had an error.
            if (AoBScanResultsPlayerBuffs.Count() < 1)
            {
                // Rename button back to defualt.
                button12.Text = "Apply";

                // Re-enable button.;
                button12.Enabled = true;

                // Reset aob scan results
                AoBScanResultsPlayerBuffs = null;

                // Display error message.
                MessageBox.Show("You must first consume a glow tulip!!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get json file from resources.
            string buffOffset = "00";
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CoreKeepersWorkshop.Resources.BuffIDs.json"))
            using (StreamReader reader = new StreamReader(stream))
            {
                // Convert stream into string.
                var jsonFileContent = reader.ReadToEnd();
                dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonFileContent);

                // Load each object from json to a string array.
                foreach (var file in result)
                {
                    // Remove spaces from food names.
                    string buffName = (string)file.name;

                    // Add the values to the combobox if it's not empty.
                    if (buffName == comboBox1.Text)
                    {
                        // Update the buffoffset.
                        buffOffset = (string)file.offset;

                        // End the loop.
                        break;
                    }
                }
            }

            // Change the buff values.
            foreach (long res in AoBScanResultsPlayerBuffs)
            {
                // Get byte offsets.
                string buffID = res.ToString("X").ToString();
                string buffPower = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");
                string buffTime = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("16", NumberStyles.Integer)).ToString("X");

                MemLib.WriteMemory(buffID, "byte", "0x" + buffOffset); // Write buff id.
                MemLib.WriteMemory(buffPower, "int", numericUpDown6.Value.ToString()); // Write buff power.
                MemLib.WriteMemory(buffTime, "float", numericUpDown7.Value.ToString()); // Write buff time.
            }

            // Process completed, run finishing tasks.
            // Rename button back to defualt.
            button12.Text = "Apply";
            button12.Enabled = true;
        }
        #endregion // End buff editor.

        #region Suicide

        // Kill the player via suicide.
        private void SiticoneWinToggleSwith6_CheckedChanged(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                siticoneWinToggleSwith6.CheckedChanged -= SiticoneWinToggleSwith6_CheckedChanged;
                siticoneWinToggleSwith6.Checked = false;
                siticoneWinToggleSwith6.CheckedChanged += SiticoneWinToggleSwith6_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsPlayerTools == null)
            {
                // Toggle slider.
                siticoneWinToggleSwith6.CheckedChanged -= SiticoneWinToggleSwith6_CheckedChanged;
                siticoneWinToggleSwith6.Checked = false;
                siticoneWinToggleSwith6.CheckedChanged += SiticoneWinToggleSwith6_CheckedChanged;

                MessageBox.Show("You need to first scan for the Player addresses!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get the addresses.
            godmodeAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.Last().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("2184", NumberStyles.Integer)).ToString("X");

            // Write value.
            MemLib.WriteMemory(godmodeAddress, "int", "0"); // Overwrite new value.

            // Toggle slider.
            siticoneWinToggleSwith6.CheckedChanged -= SiticoneWinToggleSwith6_CheckedChanged;
            siticoneWinToggleSwith6.Checked = false;
            siticoneWinToggleSwith6.CheckedChanged += SiticoneWinToggleSwith6_CheckedChanged;
        }
        #endregion

        #region Infinite Resources

        // Toggle infinite resources.
        readonly System.Timers.Timer playersInfiniteResourcesTimer = new System.Timers.Timer();
        List<Tuple<int, int[]>> inventoryInformation = new List<Tuple<int, int[]>>();
        private void SiticoneWinToggleSwith7_CheckedChanged(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                siticoneWinToggleSwith7.CheckedChanged -= SiticoneWinToggleSwith7_CheckedChanged;
                siticoneWinToggleSwith7.Checked = false;
                siticoneWinToggleSwith7.CheckedChanged += SiticoneWinToggleSwith7_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure the inventory was loaded.
            if (AoBScanResultsInventory == null)
            {
                // Toggle slider.
                siticoneWinToggleSwith7.CheckedChanged -= SiticoneWinToggleSwith7_CheckedChanged;
                siticoneWinToggleSwith7.Checked = false;
                siticoneWinToggleSwith7.CheckedChanged += SiticoneWinToggleSwith7_CheckedChanged;

                MessageBox.Show("You need to first scan for the Inventory addresses!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if the slider was not yet checked.
            if (siticoneWinToggleSwith7.Checked)
            {
                // Slider is being toggled on.
                // Name button to indicate loading.
                label27.Text = "- Loading..";

                // Offset the progress bar to show it's working.
                progressBar5.Visible = true;
                progressBar5.Maximum = 100;
                progressBar5.Step = 100 / 50;

                // Loop through each slot.
                for (int a = 1; a < 50 + 1; a++)
                {
                    // Get information from the item slot.
                    int[] itemInfo = new int[3];
                    itemInfo = GetSlotInfo(a);

                    // Update the inventory info list.
                    inventoryInformation.Add(new Tuple<int, int[]>(a, itemInfo));

                    // Advance the progress bar.
                    progressBar5.PerformStep();
                }

                // Complete progress bar.
                progressBar5.Value = 100;
                progressBar5.Visible = false;

                // Rename label to defualt text.
                label27.Text = "- Infinite Resources";

                // Start the timed events.
                playersInfiniteResourcesTimer.Interval = 100; // Custom intervals.
                playersInfiniteResourcesTimer.Elapsed += new ElapsedEventHandler(PlayersInfiniteResourcesTimedEvent);
                playersInfiniteResourcesTimer.Start();
            }
            else
            {
                // Slider is being toggled off.
                // Reset label name.
                label27.Text = "- Infinite Resources";

                // Complete progress bar.
                progressBar5.Value = 100;
                progressBar5.Visible = false;

                // Stop the timers.
                playersInfiniteResourcesTimer.Stop();

                // Clear the list of items.
                inventoryInformation = null;
            }
        }

        // Infinite recourses timer.
        private void PlayersInfiniteResourcesTimedEvent(Object source, ElapsedEventArgs e)
        {
            // Iterate through each inventory item.
            foreach (var inventorySlot in inventoryInformation)
            {
                // Ensure the item is still the same as saved. 
                int slotNumber = inventorySlot.Item1;
                int savedItemType = inventorySlot.Item2[0];
                int savedItemAmount = inventorySlot.Item2[1];
                int savedItemVariation = inventorySlot.Item2[2];

                // Get the updated slot's item type.
                int currentItemType = GetSlotInfo(slotNumber)[0];

                // Check if the current slots item was changed or is nothing.
                if (currentItemType == savedItemType)
                {
                    // Ensure the item is not null.
                    if (currentItemType == 0)
                    {
                        // Skip entree.
                        continue;
                    }
                    else
                    {
                        // Proper item was found within the inventory slot.
                        // Change the existing items durability to it's original.
                        AddItemToInv(itemSlot: slotNumber, type: savedItemType, amount: savedItemAmount, variation: savedItemVariation, Overwrite: true);
                    }
                }
                else
                {
                    // Item was changed, update it.
                    inventoryInformation[slotNumber - 1] = new Tuple<int, int[]>(slotNumber, GetSlotInfo(slotNumber));
                }
            }
        }
        #endregion // End infinite resources.

        #region Force Recall

        // Kill the player via suicide.
        private void SiticoneWinToggleSwith9_CheckedChanged(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                siticoneWinToggleSwith9.CheckedChanged -= SiticoneWinToggleSwith9_CheckedChanged;
                siticoneWinToggleSwith9.Checked = false;
                siticoneWinToggleSwith9.CheckedChanged += SiticoneWinToggleSwith9_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsPlayerTools == null)
            {
                // Toggle slider.
                siticoneWinToggleSwith9.CheckedChanged -= SiticoneWinToggleSwith9_CheckedChanged;
                siticoneWinToggleSwith9.Checked = false;
                siticoneWinToggleSwith9.CheckedChanged += SiticoneWinToggleSwith9_CheckedChanged;

                MessageBox.Show("You need to first scan for the Player addresses!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Fetch some addresses.
            string playerStateAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.Last().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("344", NumberStyles.Integer)).ToString("X");
            string playerStateSpawnAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.Last().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("480", NumberStyles.Integer)).ToString("X");

            // Write value.
            MemLib.WriteMemory(playerStateAddress, "int", MemLib.ReadInt(playerStateSpawnAddress).ToString());

            // Toggle slider.
            siticoneWinToggleSwith9.CheckedChanged -= SiticoneWinToggleSwith9_CheckedChanged;
            siticoneWinToggleSwith9.Checked = false;
            siticoneWinToggleSwith9.CheckedChanged += SiticoneWinToggleSwith9_CheckedChanged;
        }
        #endregion // End goto spawn.

        #region Anti Collision

        // Toggle anti collision.
        string playerStateOriginalValue;
        string playerStateAddress;
        string playerStateNoClipAddress;
        readonly System.Timers.Timer playersAntiCollisionTimer = new System.Timers.Timer();
        private void SiticoneWinToggleSwith8_CheckedChanged(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                siticoneWinToggleSwith8.CheckedChanged -= SiticoneWinToggleSwith8_CheckedChanged;
                siticoneWinToggleSwith8.Checked = false;
                siticoneWinToggleSwith8.CheckedChanged += SiticoneWinToggleSwith8_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsPlayerTools == null)
            {
                // Toggle slider.
                siticoneWinToggleSwith8.CheckedChanged -= SiticoneWinToggleSwith8_CheckedChanged;
                siticoneWinToggleSwith8.Checked = false;
                siticoneWinToggleSwith8.CheckedChanged += SiticoneWinToggleSwith8_CheckedChanged;

                MessageBox.Show("You need to first scan for the Player addresses!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get the addresses.
            // Get the noclip addresses.
            playerStateAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.Last().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("344", NumberStyles.Integer)).ToString("X");
            playerStateNoClipAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.Last().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("416", NumberStyles.Integer)).ToString("X");

            // Check if the slider was not yet checked.
            if (siticoneWinToggleSwith8.Checked)
            {
                // Slider is being toggled on.

                // Reset the toggle bool.
                antiCollisionToggleHold = false;

                // Read current value.
                playerStateOriginalValue = MemLib.ReadInt(playerStateAddress).ToString();

                // Start the timed events.
                playersAntiCollisionTimer.Interval = 1; // Custom intervals.
                playersAntiCollisionTimer.Elapsed += new ElapsedEventHandler(PlayersAntiCollisionTimedEvent);
                playersAntiCollisionTimer.Start();
            }
            else
            {
                // Slider is being toggled off.

                // Stop the timers.
                playersAntiCollisionTimer.Stop();

                // Write value back to original.
                // Write new value.
                MemLib.WriteMemory(playerStateAddress, "int", playerStateOriginalValue); // Overwrite new value.

                // Reset the toggle bool.
                antiCollisionToggleHold = false;
            }
        }

        // Players anti collision timer.
        bool antiCollisionToggleHold = false;
        private async void PlayersAntiCollisionTimedEvent(Object source, ElapsedEventArgs e)
        {
            // Get keydown events.
            if (IsKeyPressed(0x54))  // Get t-key press.
            {
                // Toggle bool value.
                if (antiCollisionToggleHold)
                {
                    antiCollisionToggleHold = false;
                }
                else
                {
                    antiCollisionToggleHold = true;
                }

                // Add await time for release.
                await Task.Delay(300);
            }

            // Check to run or not using the toggle.
            if (!antiCollisionToggleHold)
            {
                // Write new value.
                MemLib.WriteMemory(playerStateAddress, "int", MemLib.ReadInt(playerStateNoClipAddress).ToString()); // Overwrite new value.
            }
        }
        #endregion // End anti collision.

        #region Free Crafting

        // Toggle godmode.
        public IEnumerable<long> AoBScanResultsFreeCraftingTools = null;
        private async void SiticoneWinToggleSwith10_CheckedChanged(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                siticoneWinToggleSwith10.CheckedChanged -= SiticoneWinToggleSwith10_CheckedChanged;
                siticoneWinToggleSwith10.Checked = false;
                siticoneWinToggleSwith10.CheckedChanged += SiticoneWinToggleSwith10_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if the slider was not yet checked.
            if (siticoneWinToggleSwith10.Checked)
            {
                // Slider is being toggled on.
                // Name button to indicate loading.
                label32.Text = "- Loading..";

                // Offset the progress bar to show it's working.
                progressBar5.Visible = true;
                progressBar5.Maximum = 100;
                progressBar5.Value = 10;

                // Check if we need to rescan the addresses or not.
                if (AoBScanResultsFreeCraftingTools != null)
                {
                    string freeCraftingAddress = BigInteger.Subtract(BigInteger.Parse(AoBScanResultsFreeCraftingTools.Last().ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("10", NumberStyles.HexNumber)).ToString("X");
                    int freeCrafting = MemLib.ReadInt(freeCraftingAddress);

                    // Check if we need to rescan food or not.
                    if (freeCrafting != 0 && freeCrafting != 1)
                    {
                        // Rescan food address.
                        AoBScanResultsFreeCraftingTools = await MemLib.AoBScan("00 00 80 3F D4 04 63 3E D4 04 63 3E 00 00 80 3F E9 DD 25 3E 79 2B 7B 3F C6 AF 56 3E 00 00 80 3F", true, true);
                    }
                }
                else
                {
                    // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
                    AoBScanResultsFreeCraftingTools = await MemLib.AoBScan("00 00 80 3F D4 04 63 3E D4 04 63 3E 00 00 80 3F E9 DD 25 3E 79 2B 7B 3F C6 AF 56 3E 00 00 80 3F", true, true);
                }

                // If the count is zero, the scan had an error.
                if (AoBScanResultsFreeCraftingTools.Count() == 0)
                {
                    // Name label to indicate loading.
                    label32.Text = "- Free Crafting";

                    // Reset progress bar.
                    progressBar5.Value = 0;
                    progressBar5.Visible = false;

                    // Reset aob scan results
                    AoBScanResultsFreeCraftingTools = null;

                    // Toggle slider.
                    siticoneWinToggleSwith10.CheckedChanged -= SiticoneWinToggleSwith10_CheckedChanged;
                    siticoneWinToggleSwith10.Checked = false;
                    siticoneWinToggleSwith10.CheckedChanged += SiticoneWinToggleSwith10_CheckedChanged;

                    // Display error message.
                    MessageBox.Show("There was an issue trying to fetch the free crafting addresses." + Environment.NewLine + "Try reloading the game!!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Update richtextbox with found addresses.
                richTextBox6.Text = "Addresses Loaded: 0"; // Reset textbox.
                foreach (long res in AoBScanResultsFreeCraftingTools)
                {
                    if (richTextBox6.Text == "Addresses Loaded: 0")
                    {
                        richTextBox6.Text = "Free Crafting Addresses Loaded: " + AoBScanResultsFreeCraftingTools.Count() + " [" + res.ToString("X").ToString();
                    }
                    else
                    {
                        richTextBox6.Text += ", " + res.ToString("X").ToString();
                    }
                }
                richTextBox6.Text += "]";

                // Complete progress bar.
                progressBar5.Value = 100;
                progressBar5.Visible = false;

                // Rename label to defualt text.
                label32.Text = "- Free Crafting";

                // Toggle on free item crafting.
                foreach (long res in AoBScanResultsFreeCraftingTools)
                {
                    // Get the free crafting addresses.
                    string freeCraftingAddress = BigInteger.Subtract(BigInteger.Parse(AoBScanResultsFreeCraftingTools.Last().ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("10", NumberStyles.HexNumber)).ToString("X");

                    // Write value.
                    MemLib.WriteMemory(freeCraftingAddress, "int", "1"); // Overwrite new value.
                }
            }
            else
            {
                // Slider is being toggled off.
                // Reset label name.
                label32.Text = "- Free Crafting";

                // Complete progress bar.
                progressBar5.Value = 100;
                progressBar5.Visible = false;

                // Toggle off free item crafting.
                foreach (long res in AoBScanResultsFreeCraftingTools)
                {
                    // Get the free crafting addresses.
                    string freeCraftingAddress = BigInteger.Subtract(BigInteger.Parse(AoBScanResultsFreeCraftingTools.Last().ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("10", NumberStyles.HexNumber)).ToString("X");

                    // Write value.
                    MemLib.WriteMemory(freeCraftingAddress, "int", "0"); // Overwrite new value.
                }
            }
        }
        #endregion // End free crafting.

        #endregion // End player tab.

        #region World Tab

        #region Teleport Tool Addresses

        // Get Teleport Player addresses.
        private void Button11_Click(object sender, EventArgs e)
        {
            // Reset progress bar.
            progressBar4.Value = 0;

            // Load addresses.
            GetPlayerLocationAddresses();
        }

        // Toggle brute force teleport player addresses.
        private void CheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            // Double check if the player wishes to enable this.
            if (checkBox3.Checked && MessageBox.Show("This option should only be used if normal scaning brings no results.\n\nThis could crash your game in the process -\nSaving prior is recommended!\n\nAre you sure you wish to brute force the address searching?", "Brute Force Teleport Address Search", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                // Disable the checkbox.
                checkBox3.Checked = false;
            }
        }

        public IEnumerable<long> AoBScanResultsPlayerLocationScanner;
        public async void GetPlayerLocationAddresses()
        {
            // Amount of times to rescan the address.
            int scanTimes = 20;

            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Name button to indicate loading.
            button11.Text = "Loading...";

            // Disable button to prevent spamming.
            // button11.Enabled = false;
            groupBox11.Enabled = false;

            // Reset textbox.
            richTextBox7.Text = "Addresses Loaded: 0";

            // Offset the progress bar to show it's working.
            progressBar4.Visible = true;
            progressBar4.Maximum = 100;
            progressBar4.Step = 70 / scanTimes;
            progressBar4.Value = 10;

            // Select an address based on brute force mode.
            string AoBPlayerLocationArray = "C? CC CC 3D 00 00 00 00 ?? 99 D9 3F ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 ?? ?? ?? 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00";
            if (checkBox3.Checked)
            {
                // Brute force mode is enabled, switch array.
                AoBPlayerLocationArray = "C? CC CC 3D 00 00 00 00 ?? 99 D9 3F";
            }

            // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
            // Depreciated Address 08Feb23: C? CC CC 3D 00 00 00 00 ?? 99 D9 3F ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 ?0 ?? 00 00
            // Depreciated Address 11Mar23: C? CC CC 3D 00 00 00 00 ?? 99 D9 3F ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 ?? ?? ?? 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00
            AoBScanResultsPlayerLocationScanner = await MemLib.AoBScan(AoBPlayerLocationArray, true, true);

            // If the count is zero, the scan had an error.
            if (AoBScanResultsPlayerLocationScanner.Count() < 1)
            {
                // Reset textbox.
                richTextBox7.Text = "Addresses Loaded: 0";

                // Reset progress bar.
                progressBar4.Value = 0;
                progressBar4.Visible = false;

                // Rename button back to defualt.
                button11.Text = "Get Addresses";

                // Re-enable button.
                button11.Enabled = true;
                groupBox11.Enabled = true;

                // Reset aob scan results
                AoBScanResultsPlayerLocation = null;
                AoBScanResultsPlayerLocationScanner = null;

                // Display error message.
                MessageBox.Show("You must be standing at the core's entrance!!\r\rTIP: Press 'W' & 'D' keys when at the core's entrance.\r\rCommunity Feedback Support:\r(1) Hold [W] into The Core entrance alcove.\r(2) Tap [D].\r(3) Release [W].", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Update the progress bar.
            progressBar4.Value = 20;

            // Display info message.
            MessageBox.Show("Now stand in the Glurch (slime boss) statue entrance.\rHold W and then tap D for precise positioning.\r\rPress 'ok' when ready!", "SUCCESS - STEP 2 OF 2", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Re-scan results x times to clear invalid addresses.
            bool firstRun = true;
            List<long> resultLocations = new List<long>(AoBScanResultsPlayerLocationScanner);
            List<long> resultLocationsTemp = new List<long>(AoBScanResultsPlayerLocationScanner);
            for (int a = 0; a < scanTimes; a++)
            {
                // Skip the first loop.
                if (!firstRun)
                {
                    // Wait for 0.25 seconds.
                    await System.Threading.Tasks.Task.Delay(250);
                }
                else
                {
                    // Update bool.
                    firstRun = false;
                }

                // Get byte offsets.
                foreach (long res in resultLocationsTemp)
                {
                    try
                    {
                        string byte1 = res.ToString("X"); // C? CC CC 3D 00 00 00 00 CD CC 0C 41
                        string byte2 = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("1", NumberStyles.Integer)).ToString("X");
                        string byte3 = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("2", NumberStyles.Integer)).ToString("X");
                        string byte4 = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("3", NumberStyles.Integer)).ToString("X");
                        string byte5 = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("4", NumberStyles.Integer)).ToString("X");
                        string byte6 = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("5", NumberStyles.Integer)).ToString("X");
                        string byte7 = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("6", NumberStyles.Integer)).ToString("X");
                        string byte8 = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("7", NumberStyles.Integer)).ToString("X");
                        string byte9 = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");
                        string byte10 = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("9", NumberStyles.Integer)).ToString("X");

                        // Check if value does not exist.
                        if (
                            MemLib.ReadByte(byte1).ToString("X").ToCharArray()[0].ToString() != "C" || // C? CC CC 3D 00 00 00 00 CD CC 0C 41
                            MemLib.ReadByte(byte2).ToString("X").ToCharArray()[0].ToString() != "C" ||
                            MemLib.ReadByte(byte2).ToString("X").ToCharArray()[1].ToString() != "C" ||
                            MemLib.ReadByte(byte3).ToString("X").ToCharArray()[0].ToString() != "C" ||
                            MemLib.ReadByte(byte3).ToString("X").ToCharArray()[1].ToString() != "C" ||
                            MemLib.ReadByte(byte4).ToString("X").ToCharArray()[0].ToString() != "3" ||
                            MemLib.ReadByte(byte4).ToString("X").ToCharArray()[1].ToString() != "D" ||

                            MemLib.ReadByte(byte5).ToString("X").ToString() != "0" ||
                            MemLib.ReadByte(byte6).ToString("X").ToString() != "0" ||
                            MemLib.ReadByte(byte7).ToString("X").ToString() != "0" ||
                            MemLib.ReadByte(byte8).ToString("X").ToString() != "0" ||

                            MemLib.ReadByte(byte9).ToString("X").ToCharArray()[0].ToString() != "C" ||
                            MemLib.ReadByte(byte9).ToString("X").ToCharArray()[1].ToString() != "D" ||
                            MemLib.ReadByte(byte10).ToString("X").ToCharArray()[0].ToString() != "C" ||
                            MemLib.ReadByte(byte10).ToString("X").ToCharArray()[1].ToString() != "C"
                        )
                        {
                            // Result does not match the value it needs to be, remove it.
                            try
                            {
                                resultLocations.Remove(res);
                            }
                            catch (Exception) { }
                        }
                    }
                    catch (Exception) { }
                }

                // Progress the progress.
                progressBar4.PerformStep();
            }

            // Update the progressbar step.
            progressBar4.Step = (resultLocations.Count() == 0) ? 1 : 10 / resultLocations.Count();

            // Test the remaining values.
            resultLocationsTemp = new List<long>(resultLocations);
            foreach (long res in resultLocationsTemp)
            {
                // Get address from loop.
                string playerX = res.ToString("X").ToString();
                string playerY = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");

                // Store original values.
                float playerXOriginal = MemLib.ReadFloat(playerX);
                float playerYOriginal = MemLib.ReadFloat(playerY);

                // Write zeros to the players position.
                MemLib.WriteMemory(playerX, "float", "0");
                MemLib.WriteMemory(playerY, "float", "0");

                // Wait for 0.30 seconds.
                await System.Threading.Tasks.Task.Delay(300);

                // Check to see if the remaining values returned 0.
                if (MemLib.ReadFloat(playerX).ToString() != "0" || MemLib.ReadFloat(playerY).ToString() != "0")
                {
                    // Result does not match the value it needs to be, remove it.
                    try
                    {
                        // Further remove addresses.
                        resultLocations.Remove(res);

                        // Write the original values back.
                        MemLib.WriteMemory(playerX, "float", playerXOriginal.ToString());
                        MemLib.WriteMemory(playerY, "float", playerYOriginal.ToString());

                        // Wait for one second.
                        await System.Threading.Tasks.Task.Delay(300);
                    }
                    catch (Exception) { }
                }
                else
                {
                    // Write the original values back.
                    MemLib.WriteMemory(playerX, "float", playerXOriginal.ToString());
                    MemLib.WriteMemory(playerY, "float", playerYOriginal.ToString());

                    // Wait for 0.30 seconds.
                    await System.Threading.Tasks.Task.Delay(300);
                }

                // Progress the progress.
                progressBar4.PerformStep();
            }

            // Update the IEnumerable.
            AoBScanResultsPlayerLocation = resultLocations;

            // If the count is less then five, the scan had an error.
            if (AoBScanResultsPlayerLocation.Count() < 1)
            {
                // Reset textbox.
                richTextBox7.Text = "Addresses Loaded: 0";

                // Reset progress bar.
                progressBar4.Value = 0;
                progressBar4.Visible = false;

                // Rename button back to defualt.
                button11.Text = "Get Addresses";

                // Re-enable button.
                button11.Enabled = true;
                groupBox11.Enabled = true;

                // Reset aob scan results
                AoBScanResultsPlayerLocation = null;
                AoBScanResultsPlayerLocationScanner = null;

                // Display error message.
                MessageBox.Show("There was an issue finding the address!\rTry leaving the world or restarting the game!\r\rINFORMATION: You must be standing at the 'Glurch the Abominous Mass's entrance!!\r\rTIP: Press 'W' & 'D' keys when at the 'Glurch the Abominous Mass's entrance.\r\rCommunity Feedback Support:\r(1) Hold [W] into The Glurch the Abominous Mass's entrance alcove.\r(2) Tap [D].\r(3) Release [W].", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (AoBScanResultsPlayerLocation.Count() > 1 && AoBScanResultsPlayerLocation.Count() < 10) // Check if or between 1 & 9.
            {
                // Display error message.
                MessageBox.Show("WARNING! There is more than a single address found! While this mod may still work, long term use may cause crashes.\r\rIt's recommended to reload the world or restart the game and scan again.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // return; No return is needed.
            }
            else if (AoBScanResultsPlayerLocation.Count() > 10)
            {
                // Reset textbox.
                richTextBox7.Text = "Addresses Loaded: 0";

                // Reset progress bar.
                progressBar4.Value = 0;
                progressBar4.Visible = false;

                // Rename button back to defualt.
                button11.Text = "Get Addresses";

                // Re-enable button.
                button11.Enabled = true;
                groupBox11.Enabled = true;

                // Reset aob scan results
                AoBScanResultsPlayerLocation = null;
                AoBScanResultsPlayerLocationScanner = null;

                // Display error message.
                MessageBox.Show("Whoa there! We found too many addresses!\r\rPlease try launching the game as vanilla via steam!\rTIP: Some mod managers do not launch it as true vanilla.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Update richtextbox with found addresses.
            foreach (long res in AoBScanResultsPlayerLocation)
            {
                if (richTextBox7.Text == "Addresses Loaded: 0")
                {
                    richTextBox7.Text = "Teleport Addresses Loaded: " + AoBScanResultsPlayerLocation.Count().ToString() + " [" + res.ToString("X").ToString();
                }
                else
                {
                    richTextBox7.Text += ", " + res.ToString("X").ToString();
                }
            }
            richTextBox7.Text += "]";

            // Re-enable button.
            // button11.Enabled = true;
            groupBox11.Enabled = true;

            // Rename button back to defualt.
            button11.Text = "Get Addresses";

            // Complete progress bar.
            progressBar4.Value = 100;

            // Hide progressbar.
            progressBar4.Visible = false;
        }
        #endregion // End world tool addresses.

        #region Map Rendering Addresses

        // Get map rendering addresses.
        private void Button30_Click(object sender, EventArgs e)
        {
            // Reset progress bar.
            progressBar6.Value = 0;

            // Load addresses.
            GetMapRevealAddresses();
        }

        // Depreciated Address 10Feb23: GameAssembly.dll+381D950
        // Depreciated Address 15Feb23: GameAssembly.dll+3877D1C
        // Depreciated Address 19Feb23: GameAssembly.dll+387DDAC
        public async void GetMapRevealAddresses()
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Name button to indicate loading.
            button30.Text = "Loading...";

            // Disable button to prevent spamming.
            // button11.Enabled = false;
            groupBox8.Enabled = false;

            // Reset textbox.
            richTextBox9.Text = "Addresses Loaded: 0";

            // Offset the progress bar to show it's working.
            progressBar6.Visible = true;
            progressBar6.Maximum = 100;
            progressBar6.Step = 40;
            progressBar6.Value = 10;

            // Find the GameAssembly.dll module start and end region within memory.
            // Get a collection of all modules within a procces.
            ProcessModuleCollection modules = Process.GetProcessesByName("CoreKeeper")[0].Modules;

            // Loop through each of the modules.
            ProcessModule dllBaseAdressIWant = null;
            foreach (ProcessModule i in modules)
            {
                // Check if the module name matches.
                if (i.ModuleName == "GameAssembly.dll")
                {
                    // Record the modules address.
                    dllBaseAdressIWant = i;
                    break;
                }
            }

            // Display the collected address.
            long moduleStart = Convert.ToInt64(dllBaseAdressIWant.BaseAddress.ToString("X"), 16);
            long moduleEnd = Convert.ToInt64(BigInteger.Add(BigInteger.Parse(dllBaseAdressIWant.BaseAddress.ToString("X"), NumberStyles.HexNumber), BigInteger.Parse(dllBaseAdressIWant.ModuleMemorySize.ToString("X"), NumberStyles.HexNumber)).ToString("X"), 16);

            // Define reveal range address varible.
            AoBScanResultsRevealMapRange = await MemLib.AoBScan(moduleStart, moduleEnd, "41 00 00 40 41", true, true, false);

            // Adjust the offset of the address.
            List<long> AoBScanResultsRevealMapRangeTemp = new List<long>();
            foreach (long res in AoBScanResultsRevealMapRange)
            {
                // Add the new offset to the list.
                long revealRange = (long)BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("1", NumberStyles.Integer));

                // Ensure the defualt value is 12.
                if (MemLib.ReadFloat(revealRange.ToString("X")).ToString() == "12")
                {
                    AoBScanResultsRevealMapRangeTemp.Add(revealRange);
                }
            }

            // Build the completed list.
            AoBScanResultsRevealMapRange = AoBScanResultsRevealMapRangeTemp;

            // Check for the reveal range addresses.
            if (AoBScanResultsRevealMapRange.Count() < 1 || AoBScanResultsRevealMapRange.Count() > 1)
            {
                // Reset textbox.
                richTextBox7.Text = "Addresses Loaded: 0";

                // Reset progress bar.
                progressBar6.Value = 0;
                progressBar6.Visible = false;

                // Rename button back to defualt.
                button30.Text = "Get Addresses";

                // Re-enable button.
                button30.Enabled = true;
                groupBox8.Enabled = true;

                // Reset aob scan results
                AoBScanResultsDevMapReveal = null;

                MessageBox.Show("There was an issue gathing the reveal range addresses! Found: " + AoBScanResultsRevealMapRange.Count()  + "\r\rTry restarting your game!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Progress progress bar.
            progressBar6.PerformStep();

            // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
            AoBScanResultsDevMapReveal = await MemLib.AoBScan("04 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 01 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 04 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 04 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00", true, true);

            // Perform progressbar step.
            progressBar6.PerformStep();

            // If the count is zero, the scan had an error.
            if (AoBScanResultsDevMapReveal.Count() < 1)
            {
                // Reset textbox.
                richTextBox7.Text = "Addresses Loaded: 0";

                // Reset progress bar.
                progressBar6.Value = 0;
                progressBar6.Visible = false;

                // Rename button back to defualt.
                button30.Text = "Get Addresses";

                // Re-enable button.
                button30.Enabled = true;
                groupBox8.Enabled = true;

                // Reset aob scan results
                AoBScanResultsDevMapReveal = null;

                // Display error message.
                MessageBox.Show("Could not find the reveal map addresses!!\r\rTry restarting your game.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Update richtextbox with found addresses.
            foreach (long res in AoBScanResultsDevMapReveal)
            {
                if (richTextBox9.Text == "Addresses Loaded: 0")
                {
                    richTextBox9.Text = "Render Addresses Loaded: " + (AoBScanResultsDevMapReveal.Count() + 1).ToString() + " [" + AoBScanResultsRevealMapRange.Last().ToString("X") + ", " + res.ToString("X").ToString();
                }
                else
                {
                    richTextBox9.Text += ", " + res.ToString("X").ToString();
                }
            }
            richTextBox9.Text += "]";

            // Re-enable button.
            // button11.Enabled = true;
            groupBox8.Enabled = true;

            // Rename button back to defualt.
            button30.Text = "Get Addresses";

            // Complete progress bar.
            progressBar6.Value = 100;

            // Hide progressbar.
            progressBar6.Visible = false;
        }
        #endregion // End map rendering addresses.

        #region Set Render Distance

        // Set custom render distaance.
        private async void Button27_Click(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsDevMapReveal == null || AoBScanResultsRevealMapRange == null)
            {
                MessageBox.Show("You need to first scan for the map rendering addresses!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Reset progress bar.
            progressBar6.Visible = true;
            progressBar6.Value = 0;

            // Enable custom render.
            foreach (long res in AoBScanResultsDevMapReveal)
            {
                // Get the offset.
                string enableAddress = BigInteger.Subtract(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("104", NumberStyles.Integer)).ToString("X");
                MemLib.WriteMemory(enableAddress, "int", "1");
            }

            // Set the custom render.
            foreach (long res in AoBScanResultsRevealMapRange)
            {
                // Set the new value within memory.
                MemLib.WriteMemory(res.ToString("X"), "float", numericUpDown17.Value.ToString());
            }

            // Update the progress bar.
            if (progressBar6.Maximum >= 100)
                progressBar6.Value = 100;
            await Task.Delay(1000);
            progressBar6.Visible = false;
        }
        #endregion // End set render distance.

        #region Set Defualt Render Distance

        // Restore defualt render.
        private async void Button25_Click(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsDevMapReveal == null || AoBScanResultsRevealMapRange == null)
            {
                MessageBox.Show("You need to first scan for the map rendering addresses!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Reset progress bar.
            progressBar6.Visible = true;
            progressBar6.Value = 0;

            // Enable custom render.
            foreach (long res in AoBScanResultsDevMapReveal)
            {
                // Get the offset.
                string enableAddress = BigInteger.Subtract(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("104", NumberStyles.Integer)).ToString("X");
                MemLib.WriteMemory(enableAddress, "int", "0");
            }

            // Set the custom render.
            foreach (long res in AoBScanResultsRevealMapRange)
            {
                // Set the new value within memory.
                MemLib.WriteMemory(res.ToString("X"), "float", "12");
            }

            // Update the progress bar.
            if (progressBar6.Maximum >= 100)
                progressBar6.Value = 100;
            await Task.Delay(1000);
            progressBar6.Visible = false;
        }
        #endregion // End set defualt render distance.

        #region Auto Render Map

        #region Controls

        // Pause operations.
        private void Button31_Click(object sender, EventArgs e)
        {
            // Ensure the button is enabled first.
            if (button31.Enabled)
            {
                // Get the button state.
                if (button31.Text == "Pause Operation")
                {
                    pauseRenderingOperation = true;

                    // Update the buttons text.
                    button31.Text = "Resume Operation";

                    // Disable some controls.
                    button28.Enabled = false;
                }
                else if (button31.Text == "Resume Operation")
                {
                    pauseRenderingOperation = false;

                    // Update the buttons text.
                    button31.Text = "Pause Operation";

                    // Enable some controls.
                    button28.Enabled = true;
                }
            }
        }

        // Cancle auto renderer.
        private void Button28_Click(object sender, EventArgs e)
        {
            // Disable the groupbox.
            groupBox8.Enabled = false;

            // Ensure the button is visable first.
            if (button28.Visible)
            {
                cancleRenderingOperation = true;
            }
        }

        // Prevent value from being larger then the max radius.
        private void NumericUpDown16_ValueChanged(object sender, EventArgs e)
        {
            // Check value.
            if (numericUpDown16.Value > numericUpDown14.Value)
                numericUpDown16.Value = numericUpDown14.Value;
        }
        #endregion

        // Set anti collision and godmode timer variables.
        string renderMapPlayerStateAddress;
        string renderMapPlayerStateOriginalValue;
        string renderMapPlayerStateNoClipAddress;
        string renderMapGodmodeAddress;

        // Render map anti collision timer.
        readonly System.Timers.Timer renderMapOperationsTimer = new System.Timers.Timer();
        private void RenderMapOperationsTimedEvent(Object source, ElapsedEventArgs e)
        {
            // Write new values.
            MemLib.WriteMemory(renderMapPlayerStateAddress, "int", MemLib.ReadInt(renderMapPlayerStateNoClipAddress).ToString()); // Anti collision.
            MemLib.WriteMemory(renderMapGodmodeAddress, "int", "100000"); // Godmode.
        }

        // Auto rebnder the map.
        public bool cancleRenderingOperation = false;
        public bool pauseRenderingOperation = false;
        private async void Button22_Click(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsPlayerLocation == null)
            {
                MessageBox.Show("You need to first scan for the Teleport Player addresses!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsPlayerTools == null)
            {
                MessageBox.Show("You need to first scan for the Player addresses!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsDevMapReveal == null)
            {
                MessageBox.Show("You need to first scan for the Player addresses!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure the min radius is not larger then the max.
            if (numericUpDown16.Value > numericUpDown14.Value)
            {
                MessageBox.Show("The minimum radius cannot be larger then the max radius!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                numericUpDown16.Value = numericUpDown14.Value; // Reset the min to the max value.
                return;
            }

            // Do initial varible reset.
            renderMapPlayerStateAddress = "";
            renderMapPlayerStateOriginalValue = "";
            renderMapPlayerStateNoClipAddress = "";
            renderMapGodmodeAddress = "";

            // Define players initial position.
            var initialres = AoBScanResultsPlayerTools.Last();
            float xlocres = MemLib.ReadFloat(BigInteger.Add(BigInteger.Parse(initialres.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("144", NumberStyles.Integer)).ToString("X"));
            float ylocres = MemLib.ReadFloat(BigInteger.Add(BigInteger.Parse(initialres.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("148", NumberStyles.Integer)).ToString("X"));
            Vector2 initialPosition = new Vector2(xlocres, ylocres);

            // Define entree values.
            Vector2 localPosition = initialPosition;
            int maxRadius = (int)numericUpDown14.Value; // Max radius.
            int minRadius = (int)numericUpDown16.Value; // Min radius.
            int stepSize = (int)numericUpDown17.Value; // Range.
            double radialMoveScale = (double)numericUpDown18.Value; // radialMoveScale.
            int stepsCompleted = 0;
            int count = 0;

            // Get each XY value within x radius of player.
            int xoffset = (int)localPosition.X;
            int yoffset = (int)localPosition.Y;

            // Define starting vars.
            int x = xoffset;
            int y;
            int r;
            int rPrevious = minRadius;

            // Calculate time and primpt user.
            int calculateCount = 0;

            #region Calculate Render Time

            // Calculate the total time required.
            if ((int)numericUpDown16.Value > 0)
            {
                calculateCount++;
            }
            for (r = minRadius; r <= maxRadius; r += stepSize) //Loop through each circle radius within ranges
            {
                x = xoffset;
                for (y = rPrevious; y < r; y += (int)((double)stepSize * radialMoveScale)) //Move upwards between successive circles
                {
                    calculateCount++;
                }
                double delta = (double)((double)stepSize / (double)r);
                double theta;
                for (theta = 0; theta < 2 * Math.PI; theta += (delta * radialMoveScale)) //Move around current radius circle
                {
                    x = (int)(Math.Sin(theta) * r) + xoffset;
                    calculateCount++;
                }
                rPrevious = r;
            }
            string time = (((calculateCount * (int)numericUpDown15.Value) / 60000) >= 60) ? ((calculateCount * (int)numericUpDown15.Value) / 60000 / 60) + " hours." : (((calculateCount * (int)numericUpDown15.Value) / 1000) >= 60) ? ((calculateCount * (int)numericUpDown15.Value) / 60000) + " minutes." : ((calculateCount * (int)numericUpDown15.Value) / 1000) + " seconds";
            time = (((calculateCount * (int)numericUpDown15.Value) / 60000 / 60) >= 24) ? (((calculateCount * (int)numericUpDown15.Value) / 60000 / 60) / 24) + " days." : time;
            if (MessageBox.Show("This operaration will take ~" + time + "\n\nContinue?", "Attention!!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                // User cancled, exit void.
                return;
            }
            #endregion

            // Reset the progress bar.
            textProgressBar1.Visible = true;
            textProgressBar1.Maximum = calculateCount; // Set the progress bar total to the total required points to complete.
            textProgressBar1.Step = 1;
            textProgressBar1.Value = 0;
            textProgressBar1.CustomText = "0.00% | Current Radius: 0";

            // Change button to indicate loading.
            button22.Text = "Loading...";
            button22.Enabled = false;
            button22.Visible = false;
            button28.Visible = true;
            button31.Enabled = true;
            cancleRenderingOperation = false;

            // Enable custom render.
            foreach (long res in AoBScanResultsDevMapReveal)
            {
                // Get the offset.
                string enableAddress = BigInteger.Subtract(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("104", NumberStyles.Integer)).ToString("X");
                MemLib.WriteMemory(enableAddress, "int", "1");
            }

            // Set the custom render.
            foreach (long res in AoBScanResultsRevealMapRange)
            {
                // Set the new value within memory.
                MemLib.WriteMemory(res.ToString("X"), "float", numericUpDown17.Value.ToString());
            }

            // Reset variable.
            rPrevious = minRadius;

            // Get the noclip addresses.
            renderMapPlayerStateAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.Last().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("344", NumberStyles.Integer)).ToString("X");
            renderMapPlayerStateOriginalValue = MemLib.ReadInt(renderMapPlayerStateAddress).ToString(); // Save state for returning later.
            renderMapPlayerStateNoClipAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.Last().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("416", NumberStyles.Integer)).ToString("X");

            // Get the godmode address.
            renderMapGodmodeAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.Last().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("2184", NumberStyles.Integer)).ToString("X");

            // Declare the current start time.
            DateTime startTime = DateTime.Now;

            // Enable noclip, godmode, and start the timed events.
            renderMapOperationsTimer.Interval = 1; // Custom intervals.
            renderMapOperationsTimer.Elapsed += new ElapsedEventHandler(RenderMapOperationsTimedEvent);
            renderMapOperationsTimer.Start();

            #region Do Rendering

            // Math for creating a filled / hollow circle.
            #region Initial Y Offset
            if ((int)numericUpDown16.Value > 0)
            {
                y = minRadius + yoffset;

                // Define current position.
                Vector2 newPosition = new Vector2(x, y);

                // Iterate through each found address and update the players position.
                foreach (long res in AoBScanResultsPlayerLocation)
                {
                    // Get address from loop.
                    string playerX = res.ToString("X").ToString();
                    string playerY = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");

                    // Send player to X.
                    MemLib.WriteMemory(playerX, "float", newPosition.X.ToString());

                    // Send player to Y.
                    MemLib.WriteMemory(playerY, "float", newPosition.Y.ToString());
                }

                // Add to steps completed.
                stepsCompleted++;

                // Progress the progress bar.
                textProgressBar1.PerformStep();
                textProgressBar1.CustomText = decimal.Parse((stepsCompleted / (decimal)((decimal)calculateCount / 100)).ToString("0.00")).ToString() + "% | Current Radius: " + (int)numericUpDown16.Value;

                // Add a long cooldown.
                await Task.Delay(10000);

                // Pause the rendering operation.
                while (pauseRenderingOperation)
                {
                    try
                    {
                        // Keep the thread busy.
                        await Task.Delay(10);
                    }
                    catch (TaskCanceledException)
                    {
                        pauseRenderingOperation = false;
                    }
                }

                // Cancle the rendering operation.
                if (cancleRenderingOperation)
                {
                    // Reenable controls.
                    cancleRenderingOperation = false;
                    button22.Enabled = true;
                    button22.Visible = true;
                    button22.Text = "Auto Map Renderer";
                    button28.Visible = false; // Hide cancle button.
                    button31.Enabled = false;
                    groupBox8.Enabled = true;

                    // End look.
                    goto exitLoop;
                }
            }
            #endregion

            for (r = minRadius; r <= maxRadius; r += stepSize) //Loop through each circle radius within ranges
            {
                x = xoffset;

                #region Moving Between Circles
                for (y = rPrevious; y < r; y += (int)((double)stepSize * radialMoveScale)) //Move upwards between successive circles
                {
                    // Force enable noclip to prevent unclipping.
                    MemLib.WriteMemory(playerStateAddress, "int", MemLib.ReadInt(playerStateNoClipAddress).ToString());

                    // Define current position.
                    Vector2 newPosition = new Vector2(x, y + yoffset);

                    // Iterate through each found address and update the players position.
                    foreach (long res in AoBScanResultsPlayerLocation)
                    {
                        // Get address from loop.
                        string playerX = res.ToString("X").ToString();
                        string playerY = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");

                        // Send player to X.
                        MemLib.WriteMemory(playerX, "float", newPosition.X.ToString());

                        // Send player to Y.
                        MemLib.WriteMemory(playerY, "float", newPosition.Y.ToString());
                    }

                    // Add to steps completed.
                    stepsCompleted++;

                    // Progress the progress bar.
                    textProgressBar1.PerformStep();
                    textProgressBar1.CustomText = decimal.Parse((stepsCompleted / (decimal)((decimal)calculateCount / 100)).ToString("0.00")).ToString() + "% | Current Radius: " + r;

                    // Add a cooldown.
                    await Task.Delay((int)numericUpDown15.Value);

                    // Pause the rendering operation.
                    while (pauseRenderingOperation)
                    {
                        try
                        {
                            // Keep the thread busy.
                            await Task.Delay(10);
                        }
                        catch (TaskCanceledException)
                        {
                            pauseRenderingOperation = false;
                        }
                    }

                    // Cancle the rendering operation.
                    if (cancleRenderingOperation)
                    {
                        // Reenable controls.
                        cancleRenderingOperation = false;
                        button22.Enabled = true;
                        button22.Visible = true;
                        button22.Text = "Auto Map Renderer";
                        button28.Visible = false; // Hide cancle button.
                        groupBox8.Enabled = true;

                        // End look.
                        goto exitLoop;
                    }
                }
                #endregion

                #region Move Around Circle
                double delta = (double)((double)stepSize / (double)r);
                double theta;
                for (theta = 0; theta < 2 * Math.PI; theta += (delta * radialMoveScale)) //Move around current radius circle
                {
                    // Force enable noclip to prevent unclipping.
                    MemLib.WriteMemory(playerStateAddress, "int", MemLib.ReadInt(playerStateNoClipAddress).ToString());

                    x = (int)(Math.Sin(theta) * r) + xoffset;
                    y = (int)(Math.Cos(theta) * r) + yoffset;

                    // Define current position.
                    Vector2 newPosition = new Vector2(x, y);

                    // Iterate through each found address and update the players position.
                    foreach (long res in AoBScanResultsPlayerLocation)
                    {
                        // Get address from loop.
                        string playerX = res.ToString("X").ToString();
                        string playerY = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");

                        // Send player to X.
                        MemLib.WriteMemory(playerX, "float", newPosition.X.ToString());

                        // Send player to Y.
                        MemLib.WriteMemory(playerY, "float", newPosition.Y.ToString());
                    }

                    // Add to steps completed.
                    stepsCompleted++;

                    // Progress the progress bar.
                    textProgressBar1.PerformStep();
                    textProgressBar1.CustomText = decimal.Parse((stepsCompleted / (decimal)((decimal)calculateCount / 100)).ToString("0.00")).ToString() + "% | Current Radius: " + r;

                    // Add a cooldown.
                    await Task.Delay((int)numericUpDown15.Value);

                    // Pause the rendering operation.
                    while (pauseRenderingOperation)
                    {
                        try
                        {
                            // Keep the thread busy.
                            await Task.Delay(10);
                        }
                        catch (TaskCanceledException)
                        {
                            pauseRenderingOperation = false;
                        }
                    }

                    // Cancle the rendering operation.
                    if (cancleRenderingOperation)
                    {
                        // Reenable controls.
                        cancleRenderingOperation = false;
                        button22.Enabled = true;
                        button22.Visible = true;
                        button22.Text = "Auto Map Renderer";
                        button28.Visible = false; // Hide cancle button.
                        groupBox8.Enabled = true;

                        // End look.
                        goto exitLoop;
                    }
                }
                #endregion

                rPrevious = r;

                #region After Completed Ring Operations

                // Save the maps progress before starting next ring.
                // Skip first circle check: r != (int)numericUpDown16.Value
                if (checkBox1.Checked && r != 0)
                {
                    // Add a cooldown.
                    await Task.Delay(100);

                    // Press the "M" key to open the map.
                    if (Process.GetProcessesByName("CoreKeeper").FirstOrDefault() != null)
                    {
                        SetForegroundWindow(FindWindow(null, "Core Keeper"));
                        keybd_event((byte)0x4D, 0, 0x0001 | 0, 0);
                        keybd_event((byte)0x4D, 0, 0x0001 | 2, 0);
                    }

                    // Add a long cooldown.
                    await Task.Delay(10000);

                    // Press the "M" key to close the map.
                    if (Process.GetProcessesByName("CoreKeeper").FirstOrDefault() != null)
                    {
                        SetForegroundWindow(FindWindow(null, "Core Keeper"));
                        keybd_event((byte)0x4D, 0, 0x0001 | 0, 0);
                        keybd_event((byte)0x4D, 0, 0x0001 | 2, 0);
                    }

                    // Add a cooldown.
                    await Task.Delay(100);
                }

                // Ensure the game process still exists.
                if (!MemLib.OpenProcess("CoreKeeper"))
                {
                    // Declare the finish time and get difference of two dates.
                    DateTime finishTimeCrashed = DateTime.Now;
                    TimeSpan timeDifferenceCrashed = finishTimeCrashed - startTime;

                    // Show error message.
                    MessageBox.Show("The Core Keeper proccess was no longer found!\rRecord your progress!\r\rTask ran for " + timeDifferenceCrashed.Days + " day(s), " + timeDifferenceCrashed.Hours + " hour(s), " + timeDifferenceCrashed.Minutes + " minute(s), " + timeDifferenceCrashed.Seconds + " seconds.\r\r~" + (stepSize * stepSize) * count + " tiles have been rendered.", "Render Map", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    // Reenable controls.
                    groupBox8.Enabled = true;
                    cancleRenderingOperation = false;
                    button22.Enabled = true;
                    button22.Visible = true;
                    button22.Text = "Auto Map Renderer";
                    button28.Visible = false; // Hide cancle button.
                    groupBox8.Enabled = true;

                    // End look.
                    goto exitLoop;
                }

                // Check if memory logging is enabled.
                if (memoryLoggerActive)
                {
                    MemoryLogger(); // Do logging.
                    await Task.Delay(1000); // Add a cooldown.
                }
                #endregion
            }
            #endregion

            // Leave the loop and put the player to spawn.
            exitLoop:;

            // Reenable controls.
            groupBox8.Enabled = true;
            cancleRenderingOperation = false;
            button22.Enabled = true;
            button22.Visible = true;
            button22.Text = "Auto Map Renderer";
            button28.Visible = false; // Hide cancle button.
            button31.Enabled = false;
            textProgressBar1.Visible = false;
            textProgressBar1.Maximum = 100;
            textProgressBar1.CustomText = "";

            // Send the player back to the starting position.
            foreach (long res in AoBScanResultsPlayerLocation)
            {
                // Get address from loop.
                string playerX = res.ToString("X").ToString();
                string playerY = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");

                // Send player to X.
                MemLib.WriteMemory(playerX, "float", initialPosition.X.ToString());

                // Send player to Y.
                MemLib.WriteMemory(playerY, "float", initialPosition.Y.ToString());
            }

            // Stop timed events, disable noclip, disable godmode.
            renderMapOperationsTimer.Stop();
            MemLib.WriteMemory(renderMapPlayerStateAddress, "int", renderMapPlayerStateOriginalValue);

            // Reset variables.
            rPrevious = minRadius;

            #region Calculate Total Tiles Rendered

            // Calculate the total tiles and display result.
            if ((int)numericUpDown16.Value > 0)
            {
                count++;
                if (count >= stepsCompleted)
                {
                    goto FinishCounting;
                }
            }
            for (r = minRadius; r <= maxRadius; r += stepSize) //Loop through each circle radius within ranges
            {
                for (y = rPrevious; y < r; y += (int)((double)stepSize * radialMoveScale)) //Move upwards between successive circles
                {
                    count++;
                    if (count >= stepsCompleted)
                    {
                        goto FinishCounting;
                    }
                }
                double delta = (double)((double)stepSize / (double)r);
                double theta;
                for (theta = 0; theta < 2 * Math.PI; theta += (delta * radialMoveScale)) //Move around current radius circle
                {
                    count++;
                    if (count >= stepsCompleted)
                    {
                        goto FinishCounting;
                    }
                }
                rPrevious = r;
            }
        #endregion

            // Leave counting loop.
            FinishCounting:;

            // Declare the finish time and get difference of two dates.
            DateTime finishTime = DateTime.Now;
            TimeSpan timeDifference = finishTime - startTime;

            // Display results based on if the game is running or not.
            if (MemLib.OpenProcess("CoreKeeper"))
            {
                // Game is still running.
                MessageBox.Show("Task ran for " + timeDifference.Days + " day(s), " + timeDifference.Hours + " hour(s), " + timeDifference.Minutes + " minute(s), " + timeDifference.Seconds + " seconds.\r\r~" + (stepSize * stepSize) * count + " tiles have been rendered.", "Render Map", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                // No game found.
                // MessageBox.Show("~" + (stepSize * stepSize) * count + " tiles have been rendered.", "Render Map", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        #endregion // End render map.

        #region Remove Ground Items

        // Delete all ground items.
        private async void Button8_Click(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Reset progress bar.
            progressBar5.Step = 10;
            progressBar5.Value = 0;
            progressBar5.PerformStep(); // Progress 10%.

            // Name button to indicate loading.
            button8.Text = "Removing Items..";

            // Disable button to prevent spamming.
            button8.Enabled = false;

            // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
            AoBScanResultsGroundItems = await MemLib.AoBScan("6E 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 01 00 00 00 ?? ?? ?? ?? 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 01 00 00 00", true, true);

            // Adjust the max value of the progress bar.
            progressBar5.Step = AoBScanResultsGroundItems.Count() == 0 ? 0 : AoBScanResultsGroundItems.Count();

            // If the count is zero, the scan had an error.
            if (AoBScanResultsGroundItems.Count() == 0)
            {
                // Rename button back to defualt.
                button8.Text = "Remove Ground Items";

                // Enable button.
                button8.Enabled = true;

                // Ensure progressbar is at 100.
                progressBar5.Value = 100;

                // Update consoile with the status.
                richTextBox5.AppendText("[RemoveGroundItems] You must throw at least one torch on the ground!!" + Environment.NewLine);
                richTextBox5.ScrollToCaret();

                // Display error message.
                MessageBox.Show("You must throw at least one torch on the ground!!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Perform step.
            progressBar5.PerformStep();

            // Remove ground items.
            RemoveGroundItems();

            // Process completed, run finishing tasks.
            // Rename button back to defualt.
            button8.Text = "Remove Ground Items";

            // Enable button.
            button8.Enabled = true;
        }

        // Remove items function.
        public void RemoveGroundItems()
        {
            // Reset progress bar.
            progressBar5.Step = 10;
            progressBar5.Value = 0;
            progressBar5.PerformStep(); // Progress 10%.

            // Iterate through each found address.
            foreach (long res in AoBScanResultsGroundItems)
            {
                // Get base addresses.
                string ItemType = res.ToString("X").ToString();
                string ItemAmount = BigInteger.Add(BigInteger.Parse(ItemType, NumberStyles.HexNumber), BigInteger.Parse("4", NumberStyles.Integer)).ToString("X");
                string ItemVariant = BigInteger.Add(BigInteger.Parse(ItemType, NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");
                string ItemFooter = BigInteger.Add(BigInteger.Parse(ItemType, NumberStyles.HexNumber), BigInteger.Parse("24", NumberStyles.Integer)).ToString("X");

                // Climb down the address for each item.
                bool endAddressFound = false;
                while (!endAddressFound)
                {
                    // Find the next footer value.
                    ItemFooter = BigInteger.Subtract(BigInteger.Parse(ItemFooter, NumberStyles.HexNumber), BigInteger.Parse("32", NumberStyles.Integer)).ToString("X");

                    // Check if this is the last value.
                    if (MemLib.ReadUInt(ItemFooter).ToString() == "1")
                    {
                        // The final value to write too.
                        // Log the items removed.
                        if (MemLib.ReadUInt(ItemType).ToString() != "0")
                        {
                            richTextBox5.AppendText("Item Removed: " + "ItemID: " + MemLib.ReadInt(ItemType) + " | Amount: " + MemLib.ReadInt(ItemAmount) + " | Variation: " + MemLib.ReadInt(ItemVariant) + Environment.NewLine);
                            richTextBox5.ScrollToCaret();
                        }

                        // Use the previous values to wrtite.
                        MemLib.WriteMemory(ItemType, "int", "0");
                        MemLib.WriteMemory(ItemAmount, "int", "0");
                        MemLib.WriteMemory(ItemVariant, "int", "0");
                    }
                    else
                    {
                        // End the loop.
                        endAddressFound = true;
                    }

                    // Get the next footer values.
                    ItemType = BigInteger.Subtract(BigInteger.Parse(ItemType, NumberStyles.HexNumber), BigInteger.Parse("32", NumberStyles.Integer)).ToString("X");
                    ItemAmount = BigInteger.Subtract(BigInteger.Parse(ItemAmount, NumberStyles.HexNumber), BigInteger.Parse("32", NumberStyles.Integer)).ToString("X");
                    ItemVariant = BigInteger.Subtract(BigInteger.Parse(ItemVariant, NumberStyles.HexNumber), BigInteger.Parse("32", NumberStyles.Integer)).ToString("X");
                }

                // Progress the progress bar.
                progressBar5.PerformStep();
            }

            // Ensure progressbar is at 100.
            progressBar5.Value = 100;
        }
        #endregion // End world tools.

        #region Teleport Player

        // Teleport the player to a world position.
        private void Button9_Click(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsPlayerLocation == null)
            {
                MessageBox.Show("You need to first scan for the Teleport Player addresses!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Change button to indicate loading.
            button9.Text = "Teleporting...";
            button9.Enabled = false;

            // Iterate through each found address.
            foreach (long res in AoBScanResultsPlayerLocation)
            {
                // Get address from loop.
                string playerX = res.ToString("X").ToString();
                string playerY = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");

                // Send player to X.
                MemLib.WriteMemory(playerX, "float", numericUpDown4.Value.ToString());

                // Send player to Y.
                MemLib.WriteMemory(playerY, "float", numericUpDown5.Value.ToString());
            }

            // Process completed, run finishing tasks.
            // Rename button back to defualt.
            button9.Text = "Teleport Player To XY";
            button9.Enabled = true;
        }

        // Numericupdown key down teleport player.
        private void NumericUpDown4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Open the process and check if it was successful before the AoB scan.
                if (!MemLib.OpenProcess("CoreKeeper"))
                {
                    MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Ensure pointers are found.
                if (AoBScanResultsPlayerLocation == null)
                {
                    MessageBox.Show("You need to first scan for the Teleport Player addresses!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Change button to indicate loading.
                button9.Text = "Teleporting...";
                button9.Enabled = false;

                // Iterate through each found address.
                foreach (long res in AoBScanResultsPlayerLocation)
                {
                    // Get address from loop.
                    string playerX = res.ToString("X").ToString();
                    string playerY = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");

                    // Send player to X.
                    MemLib.WriteMemory(playerX, "float", numericUpDown4.Value.ToString());

                    // Send player to Y.
                    MemLib.WriteMemory(playerY, "float", numericUpDown5.Value.ToString());
                }

                // Process completed, run finishing tasks.
                // Rename button back to defualt.
                button9.Text = "Teleport Player To XY";
                button9.Enabled = true;
            }
        }

        // Numericupdown key down teleport player.
        private void NumericUpDown5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Open the process and check if it was successful before the AoB scan.
                if (!MemLib.OpenProcess("CoreKeeper"))
                {
                    MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Ensure pointers are found.
                if (AoBScanResultsPlayerLocation == null)
                {
                    MessageBox.Show("You need to first scan for the Teleport Player addresses!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Change button to indicate loading.
                button9.Text = "Teleporting...";
                button9.Enabled = false;

                // Iterate through each found address.
                foreach (long res in AoBScanResultsPlayerLocation)
                {
                    // Get address from loop.
                    string playerX = res.ToString("X").ToString();
                    string playerY = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");

                    // Send player to X.
                    MemLib.WriteMemory(playerX, "float", numericUpDown4.Value.ToString());

                    // Send player to Y.
                    MemLib.WriteMemory(playerY, "float", numericUpDown5.Value.ToString());
                }

                // Process completed, run finishing tasks.
                // Rename button back to defualt.
                button9.Text = "Teleport Player To XY";
                button9.Enabled = true;

            }
        }
        #endregion // End teleport player.

        #region Get World Information

        // Get world information.
        private async void Button16_Click(object sender, EventArgs e)
        {
            // Ensure properties are filled.
            if (textBox3.Text == "")
            {
                // Display error message.
                MessageBox.Show("You must type the world name you wish to use!!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Load world information.
            await LoadWorldInformation();
        }

        // Get world information keydown.
        private async void TextBox3_KeyDown(object sender, KeyEventArgs e)
        {
            // Get enter key.
            if (e.KeyCode == Keys.Enter)
            {
                // Ensure properties are filled.
                if (textBox3.Text == "")
                {
                    // Display error message.
                    MessageBox.Show("You must type the world name you wish to use!!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Open the process and check if it was successful before the AoB scan.
                if (!MemLib.OpenProcess("CoreKeeper"))
                {
                    MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Load world information.
                await LoadWorldInformation();
            }
        }

        // Function to load world information.
        public async Task LoadWorldInformation(string worldName = "")
        {
            // Ensure properties are filled.
            if (textBox3.Text == "" && worldName == "")
            {
                // Display error message.
                MessageBox.Show("You must type the world name you wish to use!!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Offset the progress bar to show it's working.
            progressBar7.Visible = true;
            progressBar7.Maximum = 100;
            progressBar7.Step = 50;
            progressBar7.Value = 10;

            // Change button to indicate loading.
            button16.Text = "Loading...";
            groupBox12.Enabled = false;

            // Clear the datagridview.
            dataGridView1.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();

            // Get current player name.
            string world = (worldName != "") ? worldName : textBox3.Text; // Check if world name override is active.
            string searchString = "{\"name\":\"" + world + "\"";
            StringBuilder builder = new StringBuilder();
            foreach (char c in searchString)
            {
                builder.Append(Convert.ToInt64(c).ToString("X"));
            }

            // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
            AoBScanResultsTeleportData = await MemLib.AoBScan(string.Join(string.Empty, builder.ToString().Select((x, i) => i > 0 && i % 2 == 0 ? string.Format(" {0}", x) : x.ToString())), true, true);

            // If the count is zero, the scan had an error.
            if (AoBScanResultsTeleportData.Count() < 1)
            {
                // Reset progress bar.
                progressBar7.Value = 0;
                progressBar7.Visible = false;

                // Rename button back to defualt.
                button16.Text = "Get World Information";

                // Re-enable button.
                groupBox12.Enabled = true;

                // Reset aob scan results
                AoBScanResultsTeleportData = null;

                // Display error message.
                // MessageBox.Show("Unable to find the world information!!\rTry playing within the world for a few minuites.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dataGridView1.DefaultCellStyle.SelectionBackColor = Color.Red;
                dataGridView1.Invoke((MethodInvoker)(() => dataGridView1.Rows.Add("ERROR:", "No information was found!!")));
                dataGridView1.Invoke((MethodInvoker)(() => dataGridView1.Rows.Add("", "")));
                dataGridView1.Invoke((MethodInvoker)(() => dataGridView1.Rows.Add("Tips:", "1) Load the world and play for a few minutes.")));
                dataGridView1.Invoke((MethodInvoker)(() => dataGridView1.Rows.Add("", "2) Ensure the spelling of your world is correct.")));
                return;
            }

            // Update the progressbar step.
            progressBar7.Step = 100 / AoBScanResultsTeleportData.Count();

            // Iterate through each found address.
            string getJsonData = "";
            bool foundData = false;
            foreach (long res in AoBScanResultsTeleportData)
            {
                // Reset found json data.
                getJsonData = "";

                // Get the cirrent base address.
                string baseJsonAddress = res.ToString("X");

                // Search result and add it to the string.
                getJsonData = MemLib.ReadString(baseJsonAddress.ToString(), length: 300);

                // Add a catch to prevent exceptions to bad addresses.
                try
                {
                    // Trim the world name to remove special characters.
                    StringBuilder sb = new StringBuilder();
                    foreach (char c in getJsonData)
                    {
                        // Define chars to include.
                        if ((c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '"' || c == '{' || c == '}' || c == ':')
                        {
                            // Build the string.
                            sb.Append(c);
                        }
                    }

                    // Check if json is completed.
                    string name = Regex.Match(sb.ToString(), "\\\"name\":\"(?<Data>\\w+)\\\"").Groups[1].Value.ToString();
                    if ((getJsonData.IndexOf('}') != getJsonData.LastIndexOf('}')) && name != "")
                    {
                        // Extract the data from the string.
                        string guid = Regex.Match(getJsonData, "\\\"guid\":\"(?<Data>\\w+)\\\"").Groups[1].Value.ToString();
                        string seed = Regex.Match(getJsonData, "\\\"seed\":(?<Data>\\w+)\\,").Groups[1].Value.ToString();
                        string activatedCrystals = Regex.Match(getJsonData, "\\\"activatedCrystals\":\\[(?<TextInsideBrackets>[a-z A-Z 0-9 ,]*\\w+)]").Groups[1].Value.ToString();
                        string year = Regex.Match(getJsonData, "\\\"year\":(?<Data>\\w+)\\,").Groups[1].Value.ToString();
                        string month = Regex.Match(getJsonData, "\\\"month\":(?<Data>\\w+)\\,").Groups[1].Value.ToString();
                        string day = Regex.Match(getJsonData, "\\\"day\":(?<Data>\\w+)\\}").Groups[1].Value.ToString();
                        string iconIndex = Regex.Match(getJsonData, "\\\"iconIndex\":(?<Data>\\w+)\\,").Groups[1].Value.ToString();
                        string mode = Regex.Match(getJsonData, "\\\"mode\":(?<Data>\\w+)\\}").Groups[1].Value.ToString();

                        // Add the information to the datagridview.
                        dataGridView1.Invoke((MethodInvoker)(() => dataGridView1.Rows.Add("Address:", baseJsonAddress)));
                        dataGridView1.Invoke((MethodInvoker)(() => dataGridView1.Rows.Add("Name:", name)));
                        dataGridView1.Invoke((MethodInvoker)(() => dataGridView1.Rows.Add("GUID:", guid)));
                        dataGridView1.Invoke((MethodInvoker)(() => dataGridView1.Rows.Add("Seed:", seed)));
                        dataGridView1.Invoke((MethodInvoker)(() => dataGridView1.Rows.Add("Crystals:", (activatedCrystals != "") ? activatedCrystals : "0,0,0")));
                        dataGridView1.Invoke((MethodInvoker)(() => dataGridView1.Rows.Add("Year:", year)));
                        dataGridView1.Invoke((MethodInvoker)(() => dataGridView1.Rows.Add("Month:", CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(int.Parse(month) + 1))));
                        dataGridView1.Invoke((MethodInvoker)(() => dataGridView1.Rows.Add("Day:", day)));
                        dataGridView1.Invoke((MethodInvoker)(() => dataGridView1.Rows.Add("iconIndex:", iconIndex)));
                        dataGridView1.Invoke((MethodInvoker)(() => dataGridView1.Rows.Add("Mode:", (mode == "0") ? "Normal" : "Hard")));

                        #region Adjust Controls

                        // Toggle controls based on world difficutly.
                        radioButton4.Checked = (mode == "0");
                        radioButton5.Checked = (mode == "1");

                        // Set world creation.
                        numericUpDown8.Value = int.Parse(year);
                        numericUpDown9.Value = int.Parse(month) + 1;
                        numericUpDown10.Value = int.Parse(day);

                        // Set activated crystals.
                        numericUpDown11.Value = (activatedCrystals != "") ? (activatedCrystals.Split(',')[0] != "") ? int.Parse(activatedCrystals.Split(',')[0]) : 0 : 0;
                        numericUpDown12.Value = (activatedCrystals != "") ? (activatedCrystals.Split(',')[1] != "") ? int.Parse(activatedCrystals.Split(',')[1]) : 0 : 0;
                        numericUpDown13.Value = (activatedCrystals != "") ? (activatedCrystals.Split(',')[2] != "") ? int.Parse(activatedCrystals.Split(',')[2]) : 0 : 0;
                        #endregion

                        // Update data found bool.
                        foundData = true;

                        // Completed, end loop.
                        break;
                    }
                    else
                    {
                        // Unfinished, reset.
                        getJsonData = "";
                    }
                }
                catch (Exception)
                {
                    // Ignore the exception as it's probably just a bad address.
                    continue;
                }

                // Perform progress step.
                progressBar7.PerformStep();
            }

            // Check if any data was found, do action if not.
            if (!foundData)
            {
                dataGridView1.DefaultCellStyle.SelectionBackColor = Color.Red;
                dataGridView1.Invoke((MethodInvoker)(() => dataGridView1.Rows.Add("ERROR:", "No information was found!!")));
                dataGridView1.Invoke((MethodInvoker)(() => dataGridView1.Rows.Add("", "")));
                dataGridView1.Invoke((MethodInvoker)(() => dataGridView1.Rows.Add("Tips:", "1) Load the world and play for a few minutes.")));
                dataGridView1.Invoke((MethodInvoker)(() => dataGridView1.Rows.Add("", "2) Ensure the spelling of your world is correct.")));
            }

            // Process completed, run finishing tasks.
            progressBar7.Value = 100;
            progressBar7.Visible = false;

            // Rename button back to defualt.
            button16.Text = "Get World Information";
            groupBox12.Enabled = true;
        }
        #endregion // End get world information.

        #region Copy Cell Text

        // Copy the value to the clipboard.
        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Define cell text.
            var cellText = dataGridView1.Rows[e.RowIndex].Cells[1].Value.ToString();

            // Ensure text is not blank.
            if (cellText != "")
            {
                // Set the clipboard.
                Clipboard.SetText(cellText);
            }
        }
        #endregion // End copy cell text.

        #region Change Difficutly

        // Change world difficulty.
        private void Button17_Click(object sender, EventArgs e)
        {
            // Ensure the datagridview is populated.
            if (dataGridView1 == null || dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("You first need to get the world information!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsTeleportData == null)
            {
                MessageBox.Show("You need to first scan for the Teleport Player addresses!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Change difficutky.
            ChangeWorldDifficulty();
        }

        // Change world difficutly.
        public IEnumerable<long> AoBScanResultsWorldMode;
        public async void ChangeWorldDifficulty(int difficutly = -1)
        {
            // Ensure the datagridview is populated.
            if (dataGridView1 == null || dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("You first need to get the world information!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsTeleportData == null)
            {
                MessageBox.Show("You need to first scan for the Teleport Player addresses!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Offset the progress bar to show it's working.
            progressBar4.Visible = true;
            progressBar4.Maximum = 100;
            progressBar4.Step = 50;
            progressBar4.Value = 10;

            // Change button to indicate loading.
            button17.Text = "Loading...";
            button17.Enabled = false;

            // Get the seed value.
            int rowIndex = -1;
            DataGridViewRow row = dataGridView1.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value.ToString().Equals("Seed:"))
                .First();
            rowIndex = row.Index;

            // Define uint.
            uint a = uint.Parse(dataGridView1.Rows[rowIndex].Cells[1].Value.ToString());

            // Convert uInt to hex 4 bytes.
            // Credits to Matthew Watson on stackoverflow: https://stackoverflow.com/a/58708490/8667430
            string result = string.Join(" ", BitConverter.GetBytes(a).Select(b => b.ToString("X2"))) + " 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00";

            // Scan for the addresses.
            AoBScanResultsWorldMode = await MemLib.AoBScan(result, true, true);

            // If the count is zero, the scan had an error.
            if (AoBScanResultsWorldMode.Count() < 1)
            {
                // Reset progress bar.
                progressBar4.Value = 0;
                progressBar4.Visible = false;

                // Rename button back to defualt.
                button17.Text = "Change Difficutly";

                // Re-enable button.
                button17.Enabled = true;

                // Reset aob scan results
                AoBScanResultsWorldMode = null;

                // Display error message.
                MessageBox.Show("Unable to find the correct addresses!!/RLoad the world and play for a few minuites.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Update the progress bar.
            progressBar4.Step = 100 / AoBScanResultsWorldMode.Count();

            // Iterate through each found address.
            foreach (long res in AoBScanResultsWorldMode)
            {
                // Get address from loop.
                string mode = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("28", NumberStyles.Integer)).ToString("X");

                // Get the new mode.
                string modeType = (radioButton4.Checked) ? "0" : (radioButton5.Checked) ? "1" : "0";
                modeType = (difficutly != -1) ? difficutly.ToString() : modeType; // Check if mode override was selected.

                // Set the new mode value.
                MemLib.WriteMemory(mode, "int", modeType);

                // Perform progress step.
                progressBar4.PerformStep();
            }

            // Update datagridview.
            int rowIndex2 = -1;
            DataGridViewRow row2 = dataGridView1.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value.ToString().Equals("Mode:"))
                .First();
            rowIndex2 = row2.Index;
            dataGridView1.Rows[rowIndex2].Cells[1].Value = (difficutly != -1) ? ((difficutly == 0) ? "Normal" : (difficutly == 1) ? "Hard" : "Normal") : (radioButton4.Checked) ? "Normal" : (radioButton5.Checked) ? "Hard" : "Normal";

            // Update the progress bar.
            progressBar4.Value = 100;
            progressBar4.Visible = false;

            // Rename button back to defualt.
            button17.Text = "Change Difficutly";
            button17.Enabled = true;
        }
        #endregion // End change world difficutly.

        #region Change Creation Date.

        // Change world date.
        private async void Button15_Click(object sender, EventArgs e)
        {
            // Ensure the datagridview is populated.
            if (dataGridView1 == null || dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("You first need to get the world information!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Offset the progress bar to show it's working.
            progressBar4.Visible = true;
            progressBar4.Maximum = 100;
            progressBar4.Step = 50;
            progressBar4.Value = 10;

            // Change button to indicate loading.
            button15.Text = "Loading...";
            button15.Enabled = false;
            numericUpDown8.Enabled = false;
            numericUpDown9.Enabled = false;
            numericUpDown10.Enabled = false;

            // Get year, month, day from datagridview.
            DataGridViewRow yearRow = dataGridView1.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value.ToString().Equals("Year:"))
                .First();
            string year = string.Join(" ", BitConverter.GetBytes(uint.Parse(dataGridView1.Rows[yearRow.Index].Cells[1].Value.ToString())).Select(b => b.ToString("X2")));
            DataGridViewRow monthRow = dataGridView1.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value.ToString().Equals("Month:"))
                .First();
            string month = string.Join(" ", BitConverter.GetBytes(uint.Parse((DateTime.ParseExact(dataGridView1.Rows[monthRow.Index].Cells[1].Value.ToString(), "MMMM", CultureInfo.CurrentCulture).Month - 1).ToString())).Select(b => b.ToString("X2")));
            DataGridViewRow dayRow = dataGridView1.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value.ToString().Equals("Day:"))
                .First();
            string day = string.Join(" ", BitConverter.GetBytes(uint.Parse(dataGridView1.Rows[dayRow.Index].Cells[1].Value.ToString())).Select(b => b.ToString("X2")));

            // Get current date string.
            string searchString = year + " " + month + " " + day;

            // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
            AoBScanResultsTeleportData = await MemLib.AoBScan(searchString, true, true);

            // If the count is zero, the scan had an error.
            if (AoBScanResultsTeleportData.Count() < 1)
            {
                // Reset progress bar.
                progressBar4.Value = 0;
                progressBar4.Visible = false;

                // Rename button back to defualt.
                button15.Text = "Change World Date";

                // Re-enable button.
                button15.Enabled = true;
                numericUpDown8.Enabled = true;
                numericUpDown9.Enabled = true;
                numericUpDown10.Enabled = true;

                // Reset aob scan results
                AoBScanResultsTeleportData = null;
                return;
            }

            // Update the progressbar step.
            progressBar4.Step = 100 / AoBScanResultsTeleportData.Count();

            // Iterate through each found address.
            foreach (long res in AoBScanResultsTeleportData)
            {
                // Get the cirrent base address.
                string yearAddress = res.ToString("X");
                string MonthAddress = BigInteger.Add(BigInteger.Parse(res.ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("4", NumberStyles.Integer)).ToString("X");
                string DayAddress = BigInteger.Add(BigInteger.Parse(res.ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");

                MemLib.WriteMemory(yearAddress, "int", numericUpDown8.Value.ToString()); // Write year address.
                MemLib.WriteMemory(MonthAddress, "int", (numericUpDown9.Value - 1).ToString()); // Write month address.
                MemLib.WriteMemory(DayAddress, "int", numericUpDown10.Value.ToString()); // Write day address.

                // Perform progress step.
                progressBar4.PerformStep();
            }

            // Update datagridview.
            dataGridView1.Rows[yearRow.Index].Cells[1].Value = numericUpDown8.Value.ToString();
            dataGridView1.Rows[monthRow.Index].Cells[1].Value = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(int.Parse(numericUpDown9.Value.ToString()));
            dataGridView1.Rows[dayRow.Index].Cells[1].Value = numericUpDown10.Value.ToString();

            // Process completed, run finishing tasks.
            progressBar4.Value = 100;
            progressBar4.Visible = false;

            // Rename button back to defualt.
            button15.Text = "Change World Date";
            button15.Enabled = true;
            numericUpDown8.Enabled = true;
            numericUpDown9.Enabled = true;
            numericUpDown10.Enabled = true;
        }
        #endregion // End world creation date.

        #region Activated Crystals

        // Activated crystals.
        private async void Button18_Click(object sender, EventArgs e)
        {
            // Ensure the datagridview is populated.
            if (dataGridView1 == null || dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("You first need to get the world information!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Offset the progress bar to show it's working.
            progressBar4.Visible = true;
            progressBar4.Maximum = 100;
            progressBar4.Step = 50;
            progressBar4.Value = 10;

            // Change button to indicate loading.
            button18.Text = "Loading...";
            button18.Enabled = false;
            numericUpDown11.Enabled = false;
            numericUpDown12.Enabled = false;
            numericUpDown13.Enabled = false;

            // Get year, month, day from datagridview.
            DataGridViewRow yearRow = dataGridView1.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value.ToString().Equals("Year:"))
                .First();
            string year = string.Join(" ", BitConverter.GetBytes(uint.Parse(dataGridView1.Rows[yearRow.Index].Cells[1].Value.ToString())).Select(b => b.ToString("X2")));
            DataGridViewRow monthRow = dataGridView1.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value.ToString().Equals("Month:"))
                .First();
            string month = string.Join(" ", BitConverter.GetBytes(uint.Parse((DateTime.ParseExact(dataGridView1.Rows[monthRow.Index].Cells[1].Value.ToString(), "MMMM", CultureInfo.CurrentCulture).Month - 1).ToString())).Select(b => b.ToString("X2")));
            DataGridViewRow dayRow = dataGridView1.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value.ToString().Equals("Day:"))
                .First();
            string day = string.Join(" ", BitConverter.GetBytes(uint.Parse(dataGridView1.Rows[dayRow.Index].Cells[1].Value.ToString())).Select(b => b.ToString("X2")));

            // Get year, month, day from datagridview.
            DataGridViewRow crystalOneRow = dataGridView1.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value.ToString().Equals("Crystals:"))
                .First();
            string crystalOne = string.Join(" ", BitConverter.GetBytes(uint.Parse((dataGridView1.Rows[crystalOneRow.Index].Cells[1].Value.ToString().Split(',')[0] != "") ? dataGridView1.Rows[crystalOneRow.Index].Cells[1].Value.ToString().Split(',')[0] : "0")).Select(b => b.ToString("X2")));
            DataGridViewRow crystalTwoRow = dataGridView1.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value.ToString().Equals("Crystals:"))
                .First();
            string crystalTwo = string.Join(" ", BitConverter.GetBytes(uint.Parse((dataGridView1.Rows[crystalTwoRow.Index].Cells[1].Value.ToString().Split(',')[1] != "") ? dataGridView1.Rows[crystalTwoRow.Index].Cells[1].Value.ToString().Split(',')[1] : "0")).Select(b => b.ToString("X2")));
            DataGridViewRow crystalThreeRow = dataGridView1.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value.ToString().Equals("Crystals:"))
                .First();
            string crystalThree = string.Join(" ", BitConverter.GetBytes(uint.Parse((dataGridView1.Rows[crystalThreeRow.Index].Cells[1].Value.ToString().Split(',')[2] != "") ? dataGridView1.Rows[crystalThreeRow.Index].Cells[1].Value.ToString().Split(',')[2] : "0")).Select(b => b.ToString("X2")));

            // Get current date string.
            string searchString = year + " " + month + " " + day;

            // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
            AoBScanResultsTeleportData = await MemLib.AoBScan(searchString, true, true);

            // If the count is zero, the scan had an error.
            if (AoBScanResultsTeleportData.Count() < 1)
            {
                // Reset progress bar.
                progressBar4.Value = 0;
                progressBar4.Visible = false;

                // Rename button back to defualt.
                button18.Text = "Activated Crystals";

                // Re-enable button.
                button18.Enabled = true;
                numericUpDown11.Enabled = true;
                numericUpDown12.Enabled = true;
                numericUpDown13.Enabled = true;

                // Reset aob scan results
                AoBScanResultsTeleportData = null;
                return;
            }

            // Update the progressbar step.
            progressBar4.Step = 100 / AoBScanResultsTeleportData.Count();

            // Iterate through each found address.
            foreach (long res in AoBScanResultsTeleportData)
            {
                // Get the cirrent base address.
                string CrystalOneAddress = BigInteger.Add(BigInteger.Parse(res.ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("64", NumberStyles.Integer)).ToString("X");
                string CrystalTwoAddress = BigInteger.Add(BigInteger.Parse(res.ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("68", NumberStyles.Integer)).ToString("X");
                string CrystalThreeAddress = BigInteger.Add(BigInteger.Parse(res.ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("72", NumberStyles.Integer)).ToString("X");

                MemLib.WriteMemory(CrystalOneAddress, "int", numericUpDown11.Value.ToString()); // Write crystal one address.
                MemLib.WriteMemory(CrystalTwoAddress, "int", numericUpDown12.Value.ToString()); // Write crystal two address.
                MemLib.WriteMemory(CrystalThreeAddress, "int", numericUpDown13.Value.ToString()); // Write crystal three address.

                // Perform progress step.
                progressBar4.PerformStep();
            }

            // Update datagridview.
            dataGridView1.Rows[crystalOneRow.Index].Cells[1].Value = numericUpDown11.Value.ToString() + "," + numericUpDown12.Value.ToString() + "," + numericUpDown13.Value.ToString();

            // Process completed, run finishing tasks.
            progressBar4.Value = 100;
            progressBar4.Visible = false;

            // Rename button back to defualt.
            button18.Text = "Activated Crystals";
            button18.Enabled = true;
            numericUpDown11.Enabled = true;
            numericUpDown12.Enabled = true;
            numericUpDown13.Enabled = true;
        }
        #endregion // End activated crystals.

        #region Auto Fishing Bot

        // Toggle auto fishing.
        readonly System.Timers.Timer autoFishingTimer = new System.Timers.Timer();
        string baseFishingAddress = "0";
        string fishTypeAddress = "0";
        string fishFightAddress = "0";
        bool autoFishingChecked = false;
        private async void Button19_Click(object sender, EventArgs e)
        {
            // Check if button was toggled.
            if (!autoFishingChecked)
            {
                // Open the process and check if it was successful before the AoB scan.
                if (!MemLib.OpenProcess("CoreKeeper"))
                {
                    MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Open the process and check if it was successful before the AoB scan.
                if (!MemLib.OpenProcess("CoreKeeper"))
                {
                    MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Toggle fishing bool.
                autoFishingChecked = true;

                // Name button to indicate loading.
                button19.Text = "Loading...";
                button19.Enabled = true;

                // Disable button to prevent spamming.
                // button11.Enabled = false;
                groupBox12.Enabled = false;

                // Reset textbox.
                richTextBox7.Text = "Addresses Loaded: 0";

                // Offset the progress bar to show it's working.
                progressBar4.Visible = true;
                progressBar4.Maximum = 100;
                progressBar4.Step = 100;
                progressBar4.Value = 10;

                // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
                // Depreciated Address 08Feb23: 3? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 01 00 00 00 ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? 01 00 00 00
                //
                AoBScanResultsFishingData = await MemLib.AoBScan("3? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 01 00 00 00 ?? ?? ?? ?? FF FF FF FF", true, true);

                // If the count is zero, the scan had an error.
                if (AoBScanResultsFishingData.Count() < 1)
                {
                    // Reset textbox.
                    richTextBox7.Text = "Addresses Loaded: 0";

                    // Reset progress bar.
                    progressBar4.Value = 0;
                    progressBar4.Visible = false;

                    // Rename button back to defualt.
                    button19.Text = "Automatic Fishing";

                    // Re-enable button.
                    button19.Enabled = true;
                    groupBox12.Enabled = true;

                    // Toggle fishing bool.
                    autoFishingChecked = false;

                    // Reset aob scan results
                    AoBScanResultsFishingData = null;

                    // Display error message.
                    MessageBox.Show("Try throwing a fishing rod's line into water first!!\rTIP: This can be of any variation.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Update richtextbox with found addresses.
                foreach (long res in AoBScanResultsFishingData)
                {
                    if (richTextBox7.Text == "Addresses Loaded: 0")
                    {
                        richTextBox7.Text = "Fishing Addresses Loaded: " + AoBScanResultsFishingData.Count().ToString() + " [" + res.ToString("X").ToString();
                    }
                    else
                    {
                        richTextBox7.Text += ", " + res.ToString("X").ToString();
                    }
                }
                richTextBox7.Text += "]";

                // Reset progress bar.
                progressBar4.Value = 0;
                progressBar4.Visible = false;

                // Get the addresses.
                baseFishingAddress = AoBScanResultsFishingData.Last().ToString("X");
                fishTypeAddress = BigInteger.Add(BigInteger.Parse(baseFishingAddress, NumberStyles.HexNumber), BigInteger.Parse("944", NumberStyles.Integer)).ToString("X");
                fishFightAddress = BigInteger.Add(BigInteger.Parse(baseFishingAddress, NumberStyles.HexNumber), BigInteger.Parse("964", NumberStyles.Integer)).ToString("X");

                // Rename button back to defualt.
                button19.Text = "Disable Fishing Bot";

                // Re-enable button.
                button19.Enabled = true;
                groupBox12.Enabled = true;

                // Slider is being toggled on.
                // Start the timed events.
                autoFishingTimer.Interval = 1; // Custom intervals.
                autoFishingTimer.Elapsed += new ElapsedEventHandler(AutoFishingTimedEvent);
                autoFishingTimer.Start();
            }
            else
            {
                // Slider is being toggled off.
                // Stop the timers.
                autoFishingTimer.Stop();

                // Rename button back to defualt.
                button19.Text = "Automatic Fishing";

                // Toggle bool.
                autoFishingChecked = false;
            }
        }

        // Auto fishing timer.
        int fishType = 0;
        bool fishFighting = false;
        bool reelingActive = false;
        private async void AutoFishingTimedEvent(Object source, ElapsedEventArgs e)
        {
            // Fetch current addresses.
            fishType = MemLib.ReadInt(fishTypeAddress);
            fishFighting = (MemLib.ReadInt(fishFightAddress) == 1);

            // Fish is on the hook, attempt to reel in. //
            // Check if a fish was caught.
            if (fishType != 0)
            {
                // Check if fish is currently fighting.
                if (!fishFighting)
                {
                    // Pull fish in.
                    mouse_event(MOUSEEVENT_RIGHTDOWN, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENT_RIGHTUP, 0, 0, 0, 0);

                    // Activate bool.
                    reelingActive = true;
                }
                else
                {
                    // Do nothing.
                    return;
                }
            }

            // Throw rod back into the water. //
            // Check if reeling.
            else if (reelingActive && fishType == 0)
            {
                // Add some delay.
                await Task.Delay((int)numericUpDown19.Value);

                // Cought finished.
                mouse_event(MOUSEEVENT_RIGHTDOWN, 0, 0, 0, 0);
                mouse_event(MOUSEEVENT_RIGHTUP, 0, 0, 0, 0);

                // Reset bool.
                reelingActive = false;
            }

            // Add some delay.
            await Task.Delay((int)numericUpDown20.Value);
        }
        #endregion // End auto fishing bot.

        #endregion // End player tools.

        #region Chat Tab

        // Enable the numericupdown based on if selected radiobutton is checked.
        private void RadioButton3_CheckedChanged(object sender, EventArgs e)
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

        #region Toggle Chat Commands

        // Toggle chat commands.
        bool chatEnabled = false;
        public System.Timers.Timer chatTimer = new System.Timers.Timer(500);
        private async void Button7_Click(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                // groupBox6.Enabled = true;
            }
            else
            {
                // Enable some controls.
                chatEnabled = true;

                // Disable some controls.
                // groupBox6.Enabled = false;

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
                    // groupBox6.Enabled = true;

                    // Display error message.
                    MessageBox.Show("You must type \"/item\" in the player chat first!!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Name button to indicate loading finished.
                button7.Text = "Disable";
                button7.Enabled = true;

                // Reset richtextbox.
                richTextBox4.Text = "Welcome to the chat commands! Available CMDS are below." + Environment.NewLine + Environment.NewLine +
                    "/item [type] [amount] [variation] - Give the player an item." + Environment.NewLine +
                    "/clearground - Remove ground items." + Environment.NewLine +
                    "/cls - Clear the console." + Environment.NewLine +
                    "/mode [worldName] [difficutly] - Change the world difficutly." + Environment.NewLine +
                    "------------------------------------------------------------------------------------------------------------" + Environment.NewLine;
                richTextBox4.AppendText("Any captured chat messages will appear here." + Environment.NewLine +
                    "------------------------------------------------------------------------------------------------------------" + Environment.NewLine);

                // Advance progress bar.
                progressBar3.Value = 100;

                // Enable a timer.
                chatTimer.AutoReset = true;
                chatTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
                chatTimer.Start();
            }
        }
        #endregion // End toggle chat commands.

        #region Chat Events

        // Do events for the chat.
        bool firstRun = true; // Do text reset bool.
        bool firstItem = true; // Ensure we only add to one slot.
        private async void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            // Iterate through each found address.
            foreach (long res in AoBScanResultsChat)
            {
                // Get address from loop.
                string baseAddress = res.ToString("X").ToString();

                // Get address value.
                string currentCommand = MemLib.ReadString(baseAddress);

                // Do chat actions.
                try
                {
                    #region Give Item

                    // Check if current value is valid command and it's unique.
                    if (currentCommand.Split(' ')[0] == "/item")
                    {
                        // Erase current chat values.
                        MemLib.WriteMemory(baseAddress, "string", "                                ");

                        // Update last chat command
                        LastChatCommand.Add(currentCommand);

                        string itemName = currentCommand.Split(' ')[1];
                        string itemAmount = currentCommand.Split(' ')[2];
                        string itemVariation = "";

                        // Log command if it does not exist.
                        if (currentCommand != richTextBox4.Lines[richTextBox4.Lines.Length - 1] && firstRun)
                        {
                            // Prevent further entries this loop.
                            firstRun = false;

                            // Display the chat command.
                            if (itemName != "")
                            {
                                // Display message.
                                richTextBox4.AppendText(currentCommand + Environment.NewLine);
                                richTextBox4.ScrollToCaret();
                            }
                            else
                            {
                                // End loop.
                                break;
                            }

                            // Ensure pointers are found.
                            if (AoBScanResultsInventory == null)
                            {
                                richTextBox4.AppendText("ERROR: You need to first scan for the Inventory addresses!" + Environment.NewLine);
                                richTextBox4.ScrollToCaret();
                                break;
                            }

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
                                            if (filenameData[0].ToLower().Contains(itemName.Replace(" ", "")) || filenameData[1] == itemName.Replace(" ", "")) // Name or ID.
                                            {
                                                // Check if to overwrite or to add to empty slots.
                                                if (radioButton1.Checked) // Overwrite slot1.
                                                {
                                                    AddItemToInv(itemSlot: 1, type: int.Parse(filenameData[1]), amount: int.Parse(itemAmount), variation: int.Parse(itemVariation) == 0 ? 0 : (int.Parse(itemVariation)), Overwrite: true);
                                                }
                                                else if (radioButton2.Checked) // Add item to an empty slot.
                                                {
                                                    // Reload inventory if add to empty is checked.
                                                    if (radioButton2.Checked && firstItem)
                                                    {
                                                        // Mark item as first.
                                                        firstItem = false;

                                                        AddItemToInv(AddToEmpty: true, type: int.Parse(filenameData[1]), amount: int.Parse(itemAmount), variation: int.Parse(itemVariation) == 0 ? 0 : (int.Parse(itemVariation)), Overwrite: true);
                                                    }
                                                }
                                                else if (radioButton3.Checked) // Custom slot.
                                                {
                                                    AddItemToInv(itemSlot: (int)numericUpDown1.Value, type: int.Parse(filenameData[1]), amount: int.Parse(itemAmount), variation: int.Parse(itemVariation) == 0 ? 0 : (int.Parse(itemVariation)), Overwrite: true);
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
                                            if (filenameData[0].ToLower().Contains(itemName.Replace(" ", "")) || filenameData[1] == itemName.Replace(" ", "")) // Name or ID.
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

                        // End chat command.
                        break;
                    }
                    #endregion // End give item.

                    #region Clear Ground Items

                    if (currentCommand.Split(' ')[0] == "/clearground")
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

                            richTextBox4.AppendText(currentCommand + " - Loading please wait.." + Environment.NewLine);
                            richTextBox4.ScrollToCaret();

                            // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
                            AoBScanResultsGroundItems = await MemLib.AoBScan("6E 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 01 00 00 00 ?? ?? ?? ?? 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 01 00 00 00", true, true);

                            // If the count is zero, the scan had an error.
                            if (AoBScanResultsGroundItems.Count() == 0)
                            {
                                // Display error message.
                                richTextBox4.AppendText("[ClearGround] You must throw at least one torch on the ground!!" + Environment.NewLine);
                                richTextBox4.ScrollToCaret();
                                break;
                            }

                            // Remove ground items.
                            RemoveGroundItems();

                            // Log evensts.
                            richTextBox4.AppendText("[ClearGround] Ground items cleared!" + Environment.NewLine);
                            richTextBox4.ScrollToCaret();

                            // End chat command.
                            break;
                        }
                    }
                    #endregion // End clear ground items.

                    #region Clear CMD

                    if (currentCommand.Split(' ')[0] == "/cls")
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

                            // Reset richtextbox.
                            richTextBox4.Text = "Any captured chat messages will appear here." + Environment.NewLine + "------------------------------------------------------------------------------------------------------------" + Environment.NewLine;

                            // End chat command.
                            break;
                        }
                    }
                    #endregion // End clear CMD.

                    #region Change World Difficulty

                    if (currentCommand.Split(' ')[0] == "/mode")
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

                            // Get strings.
                            string worldName = currentCommand.Split(' ')[1].Split(' ')[0];
                            string mode = currentCommand.Split(' ')[2];

                            // Ensure command is fully populated.
                            if (worldName == "" || mode == "")
                            {
                                // Log evensts.
                                richTextBox4.AppendText("[WorldMode] CMD ERROR: /mode [worldName] [Difficutly = 'normal' or 'hard']" + Environment.NewLine);
                                richTextBox4.ScrollToCaret();
                                break;
                            }

                            // Open the process and check if it was successful before the AoB scan.
                            if (!MemLib.OpenProcess("CoreKeeper"))
                            {
                                // Log evensts.
                                richTextBox4.AppendText("[WorldMode] ERROR: Process Is Not Found or Open!" + Environment.NewLine);
                                richTextBox4.ScrollToCaret();
                                break;
                            }

                            // Load world information.
                            Task.Run(async () => { await LoadWorldInformation(worldName); }).Wait();

                            // Change world difficutly.
                            if (mode.ToLower() == "normal")
                            {
                                // Change world difficutly to normal.
                                ChangeWorldDifficulty(0);

                                // Log evensts.
                                richTextBox4.AppendText("[WorldMode] Difficulty set to normal!" + Environment.NewLine);
                                richTextBox4.ScrollToCaret();
                            }
                            else if (mode.ToLower() == "hard")
                            {
                                // Change world difficutly to hard.
                                ChangeWorldDifficulty(1);

                                // Log evensts.
                                richTextBox4.AppendText("[WorldMode] Difficulty set to hard!" + Environment.NewLine);
                                richTextBox4.ScrollToCaret();
                            }

                            // End chat command.
                            break;
                        }
                    }
                    #endregion // End change world difficutly.
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
        #endregion // End chat events.

        #endregion // End toggle chat commands

        #region Admin Tools

        #region Item ID List Builder

        // Create an ID list from all installed assets.
        private void Label7_Click(object sender, EventArgs e)
        {
            // Recolor label.
            label7.ForeColor = Color.Lime;

            // Delete files in out if they exist.
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"ItemIDList.txt"))
            {
                File.Delete(AppDomain.CurrentDomain.BaseDirectory + @"ItemIDList.txt");
            }

            // Define counter for total items.
            int renamedImagesCount = 0;

            // Check if images exist within the directory.
            try
            {
                if (Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"assets\Inventory\", "*.png", SearchOption.AllDirectories) == null)
                {
                    // Recolor label.
                    label7.ForeColor = Color.Red;

                    MessageBox.Show("No assets found within the inventory directory.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    // Get each file in the directory.
                    string[] Files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"assets\Inventory\", "*.png", SearchOption.AllDirectories);

                    // Sort array based on item ids.
                    Array.Sort(Files, (a, b) => int.Parse(Regex.Replace(Path.GetFileName(a).Split(',')[1], "[^0-9]", "")) - int.Parse(Regex.Replace(Path.GetFileName(b).Split(',')[1], "[^0-9]", "")));

                    // Append first lineset.
                    File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"ItemIDList.txt", "public enum ObjectID\r{\r  None = 0,\r");

                    // Get each image file in directory.
                    foreach (string file in Files)
                    {
                        try
                        {
                            // Define image variant.
                            string imageName = Path.GetFileName(file).Split(',')[0];
                            string imageID = Path.GetFileName(file).Split(',')[1];
                            string imageVariant = Path.GetFileName(file).Split(',')[2].Split('.')[0];

                            // Check if variant is not zero and not the last entree.
                            if (int.Parse(imageVariant) == 0 && Path.GetFileName(Files.Last()).Split(',')[0] != imageName)
                            {
                                // Define new filename.
                                File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"ItemIDList.txt", "  " + imageName + " = " + imageID + "," + Environment.NewLine);

                                // Add to count.
                                renamedImagesCount++;
                            }
                            else if (int.Parse(imageVariant) == 0 && Path.GetFileName(Files.Last()).Split(',')[0] == imageName) // Last entree.
                            {
                                // Define new filename.
                                File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"ItemIDList.txt", "  " + imageName + " = " + imageID + Environment.NewLine);

                                // Add to count.
                                renamedImagesCount++;
                            }
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }

                    // Append first lineset.
                    File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"ItemIDList.txt", "}");

                    // Display results.
                    MessageBox.Show(renamedImagesCount.ToString() + " IDs where found and recorded.\r\rOutput: \r" + AppDomain.CurrentDomain.BaseDirectory + @"ItemIDList.txt", "ItemID List Builder", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception a)
            {
                // Recolor label.
                label7.ForeColor = Color.Red;

                // Display error.
                MessageBox.Show(a.Message.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Recolor label.
            label7.ForeColor = Color.Red;
        }
        #endregion // End upgrade legacy items.

        #region Random Food ID

        // Get a random food ID.
        private void Label4_Click(object sender, EventArgs e)
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

            // Recolor label.
            label4.ForeColor = Color.Lime;

            // Create directories if they do not exist.
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\"))
            {
                // Create directory.
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\");
            }

            // Check if images exist within the directory.
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\data.txt"))
            {
                // Recolor label.
                label4.ForeColor = Color.Red;

                MessageBox.Show(@"The file '\assets\debug\data.txt' does not exist!" + Environment.NewLine + "Create one with random IDs per line.", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                // Define new random item variation.
                Random randomItem = new Random();
                int item1 = randomItem.Next(0, File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\data.txt").Length);
                int item2 = randomItem.Next(0, File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\data.txt").Length);
                string itemVariation = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\data.txt")[item1].ToString() + File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\data.txt")[item2].ToString();

                // Add new item to slot2.
                // Iterate through each found address.
                foreach (long res in AoBScanResultsInventory)
                {
                    try
                    {
                        // Get address from loop.
                        string baseAddress = res.ToString("X").ToString();

                        string slot2Item = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("16", NumberStyles.Integer)).ToString("X");
                        string slot2Amount = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("20", NumberStyles.Integer)).ToString("X");
                        string slot2Variation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("24", NumberStyles.Integer)).ToString("X");

                        // Add New Item
                        MemLib.WriteMemory(slot2Item, "int", MemLib.ReadInt(slot2Item).ToString()); // Write item type 
                        MemLib.WriteMemory(slot2Amount, "int", MemLib.ReadInt(slot2Amount).ToString()); // Write item amount
                        MemLib.WriteMemory(slot2Variation, "int", itemVariation.ToString()); // Write item variation
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }

            // Recolor label.
            label4.ForeColor = Color.Lime;
        }
        #endregion // End random food id.

        #region Quick Edit Slot2

        // Quick edit Slot2s item using arrow keys.
        [DllImport("user32.dll")]
        static extern short GetKeyState(int nVirtKey);
        public static bool IsKeyPressed(int testKey)
        {
            // Barrowed From: http://pinvoke.net/default.aspx/user32/GetKeyboardState.html
            // Virtual Key Values: https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes

            short result = GetKeyState(testKey);
            bool keyPressed;
            switch (result)
            {
                case 0:
                    // Not pressed and not toggled on.
                    keyPressed = false;
                    break;

                case 1:
                    // Not pressed, but toggled on
                    keyPressed = false;
                    break;

                default:
                    // Pressed (and may be toggled on)
                    keyPressed = true;
                    break;
            }
            return keyPressed;
        }

        // Define current item values.
        int currentSwapItem = 0;
        int currentSwapAmount = 50;
        int currentSwapVariation = 0;
        bool itemSwapActive = false; // Define toggle for on / off.
        readonly System.Timers.Timer itemSwapTimer = new System.Timers.Timer();
        private void Label8_Click(object sender, EventArgs e)
        {
            // Check if item swap is active or not.
            if (!itemSwapActive)
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

                // Show message box.
                MessageBox.Show("Key Mapping:" + Environment.NewLine + Environment.NewLine + "Left & Right Arrow Keys: +/- ID" + Environment.NewLine + "Up & Down Arrow Keys: +/- Variation" + Environment.NewLine + "Add & Subtract Buttons: +/- Amount", "INFO", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Recolor label.
                label8.ForeColor = Color.Lime;

                // Get existing slot two values.
                // Iterate through each found address.
                foreach (long res in AoBScanResultsInventory)
                {
                    // Get address for the second item slot from loop.
                    string baseAddress = res.ToString("X").ToString();
                    currentSwapItem = MemLib.ReadInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("16", NumberStyles.Integer)).ToString("X"));
                    currentSwapAmount = MemLib.ReadInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("20", NumberStyles.Integer)).ToString("X"));
                    currentSwapVariation = MemLib.ReadInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("24", NumberStyles.Integer)).ToString("X"));
                }

                // Enable bool.
                itemSwapActive = true;

                // Start the timed events.
                itemSwapTimer.Interval = (double)numericUpDown2.Value; // Custom intervals.
                itemSwapTimer.Elapsed += new ElapsedEventHandler(ItemSwapTimedEvent);
                itemSwapTimer.Start();
            }
            else
            {
                // Stop the timer.
                itemSwapTimer.Stop();

                // Recolor label.
                label8.ForeColor = Color.Red;

                // Disable bool.
                itemSwapActive = false;
            }
        }

        // Item swap timer.
        private void ItemSwapTimedEvent(Object source, ElapsedEventArgs e)
        {
            // Get keydown events.
            if (IsKeyPressed(0x25))  // Get left arrow press; subtract id.
            {
                AddItemToInv(itemSlot: 2, type: currentSwapItem -= 1, amount: currentSwapAmount, variation: currentSwapVariation, Overwrite: true);
            }
            else if (IsKeyPressed(0x27)) // Get right arrow press; add id.
            {
                AddItemToInv(itemSlot: 2, type: currentSwapItem += 1, amount: currentSwapAmount, variation: currentSwapVariation, Overwrite: true);
            }
            else if (IsKeyPressed(0x26)) // Get up arrow press; subtract variant.
            {
                AddItemToInv(itemSlot: 2, type: currentSwapItem, amount: currentSwapAmount, variation: currentSwapVariation -= 1, Overwrite: true);
            }
            else if (IsKeyPressed(0x28)) // Get down arrow press; add variant.
            {
                AddItemToInv(itemSlot: 2, type: currentSwapItem, amount: currentSwapAmount, variation: currentSwapVariation += 1, Overwrite: true);
            }
            else if (IsKeyPressed(0xBB)) // Get plus button; add amount.
            {
                AddItemToInv(itemSlot: 2, type: currentSwapItem, amount: currentSwapAmount += 1, variation: currentSwapVariation, Overwrite: true);
            }
            else if (IsKeyPressed(0xBD)) // Get minus button; subtract amount.
            {
                AddItemToInv(itemSlot: 2, type: currentSwapItem, amount: currentSwapAmount -= 1, variation: currentSwapVariation, Overwrite: true);
            }
        }
        #endregion // End quick edit slot2.

        #region Recenter Game

        // Bring game window back to the center and resize.
        private void Label30_Click(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get the process of the game.
            Process process = Process.GetProcessesByName("CoreKeeper").First();

            while (process.MainWindowHandle == IntPtr.Zero)
                process.Refresh();

            // Define the handle.
            IntPtr handle = process.MainWindowHandle;
            Rectangle screen = Screen.FromHandle(handle).Bounds;

            // Define the new rectangle size and find the screen center.
            Rectangle newSize = new Rectangle(0, 0, 600, 400);
            Point pt = new Point(screen.Left + screen.Width / 2 - (newSize.Right - newSize.Left) / 2, screen.Top + screen.Height / 2 - (newSize.Bottom - newSize.Top) / 2);

            // Send the process to a new location.
            SetWindowPos(handle, IntPtr.Zero, pt.X, pt.Y, 600, 400, SWP_SHOWWINDOW);
        }
        #endregion // End recenter game.

        #region Memory Logger

        // Auto render map memory logger.
        bool memoryLoggerActive = false; // Define toggle for on / off.
        private void Label31_Click(object sender, EventArgs e)
        {
            // Create directories if they do not exist.
            if (!Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\"))
            {
                // Create directory.
                Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\");
            }

            // Check if item swap is active or not.
            if (!memoryLoggerActive)
            {
                // Recolor label.
                label31.ForeColor = Color.Lime;

                // Enable bool.
                memoryLoggerActive = true;
            }
            else
            {
                // Recolor label.
                label31.ForeColor = Color.Red;

                // Disable bool.
                memoryLoggerActive = false;
            }
        }

        // Callable Memory logger.
        private void MemoryLogger()
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                return;
            }

            // Record information.
            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + @"\assets\debug\MemoryLogger.txt", DateTime.Now + " -> " + Math.Round(new PerformanceCounter("Process", "Private Bytes", "CoreKeeper", true).NextValue() / 1024 / 1024 / 1000, 2).ToString() + " GBs" + Environment.NewLine);
        }
        #endregion

        #region Reset All Controls

        // Reset all controls.
        private void Button34_Click(object sender, EventArgs e)
        {
            // Ask user if they are sure to reset all controls.
            if (MessageBox.Show("Are you sure you wish to reset all form controls?", "Reset All Controls", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // Main controls.
                numericUpDown14.Value = decimal.Parse(CoreKeepersWorkshop.Properties.Settings.Default.GetType().GetProperty(nameof(CoreKeepersWorkshop.Properties.Settings.Default.MapRenderingMax)).GetCustomAttribute<System.Configuration.DefaultSettingValueAttribute>().Value); // Map rendering max radius.
                numericUpDown16.Value = decimal.Parse(CoreKeepersWorkshop.Properties.Settings.Default.GetType().GetProperty(nameof(CoreKeepersWorkshop.Properties.Settings.Default.MapRenderingStart)).GetCustomAttribute<System.Configuration.DefaultSettingValueAttribute>().Value); // Map rendering start radius.
                numericUpDown19.Value = decimal.Parse(CoreKeepersWorkshop.Properties.Settings.Default.GetType().GetProperty(nameof(CoreKeepersWorkshop.Properties.Settings.Default.FishingCast)).GetCustomAttribute<System.Configuration.DefaultSettingValueAttribute>().Value); // Fishing bot casting delay.
                numericUpDown20.Value = decimal.Parse(CoreKeepersWorkshop.Properties.Settings.Default.GetType().GetProperty(nameof(CoreKeepersWorkshop.Properties.Settings.Default.FishingPadding)).GetCustomAttribute<System.Configuration.DefaultSettingValueAttribute>().Value); // Fishing bot padding delay.

                // Dev tools.
                numericUpDown2.Value = decimal.Parse(CoreKeepersWorkshop.Properties.Settings.Default.GetType().GetProperty(nameof(CoreKeepersWorkshop.Properties.Settings.Default.DevToolDelay)).GetCustomAttribute<System.Configuration.DefaultSettingValueAttribute>().Value); // Dev tool operation delay.
                numericUpDown18.Value = decimal.Parse(CoreKeepersWorkshop.Properties.Settings.Default.GetType().GetProperty(nameof(CoreKeepersWorkshop.Properties.Settings.Default.RadialMoveScale)).GetCustomAttribute<System.Configuration.DefaultSettingValueAttribute>().Value); // Auto render maps radialMoveScale.
                checkBox2.Checked = bool.Parse(CoreKeepersWorkshop.Properties.Settings.Default.GetType().GetProperty(nameof(CoreKeepersWorkshop.Properties.Settings.Default.TopMost)).GetCustomAttribute<System.Configuration.DefaultSettingValueAttribute>().Value); // Set as top most.

                // Display completed message.
                MessageBox.Show("All controls have been reset!", "Reset All Controls", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        #endregion

        #endregion // End admin tools.
    }

    public class Nameof<T>
    {
        public static string Property<TProp>(Expression<Func<T, TProp>> expression)
        {
            var body = expression.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException("'expression' should be a member expression");
            return body.Member.Name;
        }
    }
}
