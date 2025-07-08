using CoreKeepersWorkshop.Properties;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Security.Principal;
using System.Drawing.Drawing2D;
using System.Linq.Expressions;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Globalization;
using System.Windows.Forms;
using CoreKeepersWorkshop;
using System.Diagnostics;
using System.Reflection;
using Newtonsoft.Json;
using System.Numerics;
using System.Drawing;
using System.Timers;
using System.Linq;
using System.Text;
using System.IO;
using Memory;
using System;

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
        public Mem MemLib = null;                                     // Do not define memory.dll yet.
        public IEnumerable<long> AoBScanResultsInventory;
        public IEnumerable<long> AoBScanResultsPlayerName;
        public IEnumerable<long> AoBScanResultsChat;
        public IEnumerable<long> AoBScanResultsGroundItems;
        public IEnumerable<long> AoBScanResultsPlayerTools;
        public IEnumerable<long> AoBScanResultsPlayerLocation;
        public IEnumerable<long> AoBScanResultsPlayerMapLocation;
        public IEnumerable<long> AoBScanResultsPlayerBuffs;
        public IEnumerable<long> AoBScanResultsWorldData;
        public IEnumerable<long> AoBScanResultsFishingData;
        public IEnumerable<long> AoBScanResultsDevMapReveal;
        public IEnumerable<long> AoBScanResultsRevealMapRange;
        public IEnumerable<long> AoBScanResultsWorldName;
        public static List<long> AoBScanResultsSkills;                // SkillEditor.
        public static IEnumerable<long> AoBScanResultsSkillLoadout;   // SkillEditor.
        // public static List<Tuple<long, int>> AoBScanResultsSkills; // SkillEditor.
        // public static IEnumerable<long> AoBScanResultsSkills;      // SkillEditor.
        // public List<string> LastChatCommand = new List<string>() { "" };
        public Dictionary<string, int> ExportPlayerItems = new Dictionary<string, int> { };
        public string ExportPlayerName = "";
        public bool isMinimized = false;
        public int useAddress = 1;
        public bool placeholdersActive = false;

        // Define texture data.
        public IEnumerable<string> ImageFiles1 = Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"assets\Inventory\") && Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"assets\Inventory\", "*.png", SearchOption.AllDirectories) != null ? Directory.GetFileSystemEntries(AppDomain.CurrentDomain.BaseDirectory + @"assets\Inventory\", "*.png", SearchOption.AllDirectories) : new String[] { "" }; // Ensure directory exists and images exist. Fix: v1.2.9.
        public IEnumerable<string> InventorySkins = Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"assets\backgrounds\Inventory") && Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"assets\backgrounds\Inventory", "*.png", SearchOption.AllDirectories) != null ? Directory.GetFileSystemEntries(AppDomain.CurrentDomain.BaseDirectory + @"assets\backgrounds\Inventory", "*.png", SearchOption.AllDirectories) : new String[] { "" }; // Ensure directory exists and images exist. Fix: v1.2.9.
        public IEnumerable<string> PlayerSkins = Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"assets\backgrounds\Player") && Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"assets\backgrounds\Player", "*.png", SearchOption.AllDirectories) != null ? Directory.GetFileSystemEntries(AppDomain.CurrentDomain.BaseDirectory + @"assets\backgrounds\Player", "*.png", SearchOption.AllDirectories) : new String[] { "" }; // Ensure directory exists and images exist. Fix: v1.2.9.
        public IEnumerable<string> WorldSkins = Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"assets\backgrounds\World") && Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"assets\backgrounds\World", "*.png", SearchOption.AllDirectories) != null ? Directory.GetFileSystemEntries(AppDomain.CurrentDomain.BaseDirectory + @"assets\backgrounds\World", "*.png", SearchOption.AllDirectories) : new String[] { "" }; // Ensure directory exists and images exist. Fix: v1.2.9.
        public IEnumerable<string> ChatSkins = Directory.Exists(AppDomain.CurrentDomain.BaseDirectory + @"assets\backgrounds\Chat") && Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory + @"assets\backgrounds\Chat", "*.png", SearchOption.AllDirectories) != null ? Directory.GetFileSystemEntries(AppDomain.CurrentDomain.BaseDirectory + @"assets\backgrounds\Chat", "*.png", SearchOption.AllDirectories) : new String[] { "" }; // Ensure directory exists and images exist. Fix: v1.2.9.

        // Define skin counters.
        public int inventorySkinCounter = Settings.Default.InventoryBackgroundCount;
        public int playerSkinCounter = Settings.Default.PlayerBackgroundCount;
        public int worldSkinCounter = Settings.Default.WorldBackgroundCount;
        public int chatSkinCounter = Settings.Default.ChatBackgroundCount;

        // Define warning and error titles.
        public static readonly string warningTitle = $"WARNING: {FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).ProductName} v{FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion}";
        public static readonly string errorTitle   = $"ERROR: {FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).ProductName} v{FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion}";

        // Set the mouse event class.
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        private const int MOUSEEVENT_LEFTDOWN   = 0x02;
        private const int MOUSEEVENT_LEFTUP     = 0x04;
        private const int MOUSEEVENT_RIGHTDOWN  = 0x08;
        private const int MOUSEEVENT_RIGHTUP    = 0x10;
        private const int MOUSEEVENT_MIDDLEDOWN = 0x20;
        private const int MOUSEEVENT_MIDDLEUP   = 0x40;

        #region Process Handle Classes

        // Set the process overlay class.
        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("User32.dll")]
        public static extern void ReleaseDC(IntPtr hwnd, IntPtr dc);

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
        private static IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex)
        {
            if (IntPtr.Size == 8)
                return GetWindowLongPtr64(hWnd, nIndex);
            else
                return new IntPtr(GetWindowLong32(hWnd, nIndex));
        }

        [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
        private static extern int GetWindowLong32(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;   // x position of upper-left corner.
            public int Top;    // y position of upper-left corner.
            public int Right;  // x position of lower-right corner.
            public int Bottom; // y position of lower-right corner.
        }

        private const int SWP_NOSIZE      = 0x0001;
        private const int SWP_NOZORDER    = 0x0004;
        private const int SWP_SHOWWINDOW  = 0x0040;

        private const int GWL_STYLE       = -16;
        private const int GWL_EXSTYLE     = -20;

        private const long WS_BORDER      = 0x00800000L;
        private const long WS_CAPTION     = 0x00C00000L; // WS_BORDER | WS_DLGFRAME .
        private const long WS_DLGFRAME    = 0x00400000L;
        private const long WS_OVERLAPPED  = 0x00000000L;
        private const long WS_POPUP       = unchecked((long)0x80000000L);
        private const long WS_SYSMENU     = 0x00080000L;
        private const long WS_THICKFRAME  = 0x00040000L; // resizable border.
        private const long WS_MINIMIZEBOX = 0x00020000L;
        private const long WS_MAXIMIZEBOX = 0x00010000L;

        #endregion

        #endregion // End variables.

        #region Form Controls

        // Do form loading events.
        private void MainForm_Load(object sender, EventArgs e)
        {
            try // Further catch possible errors.
            {
                #region Are We Admin?

                // Check if the application was started in administrator mode.
                if (!IsAdministrator() && MessageBox.Show("The mods in this application require administrator privileges.\n\nWould you like to proceed anyways? (not recommended)", "WARNING: LAUNCHED WITHOUT ADMINISTRATIVE RIGHTS", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
                {
                    // Close the application.
                    this.Close();
                }
                #endregion

                #region Does Memory.dll Exist?

                // Check if the users AV detected memory.dll as a false positive.
                string startupPath = AppDomain.CurrentDomain.BaseDirectory;
                string dllPath = Path.Combine(startupPath, "Memory.dll");

                if (!File.Exists(dllPath))
                {
                    // If the DLL doesn't exist, show an error and close the app.
                    MessageBox.Show("The required Memory.dll is missing from the application directory.\n\n" +
                                    "This may have occurred because your antivirus software mistakenly flagged it as a false positive.\n\n" +
                                    "To resolve this, please follow these steps to add an exception in your antivirus:\n\n" +
                                    "1) Open your antivirus software.\n" +
                                    "2) Go to the 'Settings' or 'Exclusions' section.\n" +
                                    "3) Add an exclusion for 'Memory.dll' located in: " + dllPath + ".\n\n" +
                                    "Once the exception is added, and Memory.dll has been restored, restart the application.\n\n" +
                                    "Exiting the application now.",
                                    errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }

                // Checks passed, initiate memory.dll.
                InitiateMemoryDLL();
                #endregion

                #region Set Custom Cusror

                // Set the applications cursor.
                Cursor = new Cursor(Resources.UICursor.GetHicon());
                #endregion

                #region Set Controls

                // Set the about tabs content.
                // About.
                About_RichTextBox.Text = String.Concat(new string[] {
                @"// CoreKeepersWorkshop v" + FileVersionInfo.GetVersionInfo(Path.GetFileName(System.Windows.Forms.Application.ExecutablePath)).FileVersion + " - Written kindly by: dannyruss" + Environment.NewLine,
                @"-------------------------------------------------------------------------------------------------------------------" + Environment.NewLine,
                @"This tool was created with future content and modded content in mind. It currently supports manual item additions by naming images using the following format: ItemName,ItemID,ItemVariation.png - You can add these assets to the ""\assets\inventory\"" directory. For future requests or any issues, please contact me under my discord handle above, thanks!" + Environment.NewLine,
                @"-------------------------------------------------------------------------------------------------------------------" + Environment.NewLine,
                @"Project source: https://github.com/RussDev7/CoreKeepersWorkshop"
                });

                // Honorable mentions.
                SpecialThanks_RichTextBox.Text = String.Concat(new string[] {
                @"// Here we give thanks to those who have helped the project grow!" + Environment.NewLine,
                @"// This project would never have grown if not for the following:" + Environment.NewLine + Environment.NewLine,

                @"1) Ultimaton2   - Most helpful debugger in the projects lifetime." + Environment.NewLine,
                @"2) Pharuxtan    - Helped get the new variation system working." + Environment.NewLine,
                @"3) Roupiks       - Created assets for all the tabs!" + Environment.NewLine + Environment.NewLine,

                @"Honorable Mentions:" + Environment.NewLine,
                @"BourbonCrow, puxxy5layer, Flux, ZeroGravitas, kremnev8, Norois, Ice, Yumiko Abe, TheBgNaz, Arne, Bosh, Smuke.",
                });
                #endregion

                #region Set Dev-Tool / Main Control Contents

                // Main controls.
                MaxRadius_NumericUpDown.Value = Settings.Default.MapRenderingMax;     // Map rendering max radius.
                StartRadius_NumericUpDown.Value = Settings.Default.MapRenderingStart; // Map rendering start radius.
                CastDelay_NumericUpDown.Value = Settings.Default.FishingCast;         // Fishing bot casting delay.
                FishingPadding_NumericUpDown.Value = Settings.Default.FishingPadding; // Fishing bot padding delay.

                // Console color.
                WorldInformation_DataGridView.RowsDefaultCellStyle.ForeColor = Settings.Default.ConsoleForeColor;
                WorldInformation_DataGridView.AlternatingRowsDefaultCellStyle.ForeColor = Settings.Default.ConsoleForeColor;

                // Console color indicator.
                ColorSample_Button.ForeColor = Settings.Default.ConsoleForeColor;
                ColorSample_Button.BackColor = Settings.Default.ConsoleForeColor;

                // Dev tools.
                DevToolsDelay_NumericUpDown.Value = (decimal)Settings.Default.DevToolDelay; // Dev tool operation delay.
                RadialMoveScale_NumericUpDown.Value = Settings.Default.RadialMoveScale;     // Auto render maps radialMoveScale.
                AlwaysOnTop_CheckBox.Checked = Settings.Default.TopMost;                    // Set as top most.
                AppPriority_ComboBox.SelectedIndex = Settings.Default.ProcessPriorityIndex; // Set the process priority.
                FormOpacity_TrackBar.Value = Settings.Default.FormOpacity;                  // Set the form opacity trackbar value.
                #endregion

                #region Set Form Opacity

                // Set form opacity based on trackbars value saved setting (1 to 100 -> 0.01 to 1.0).
                this.Opacity = Settings.Default.FormOpacity / 100.0;
                #endregion

                #region Set Form Locations

                // Set the forms active location based on previous save.
                MainForm.ActiveForm.Location = Settings.Default.MainFormLocation;
                #endregion

                #region Set Background

                // Get background from saved settings.
                if (Settings.Default.InventoryBackground != "") // Ensure background is not null.
                {
                    // Catch image missing / renamed errors.
                    try
                    {
                        Main_TabControl.TabPages[0].BackgroundImage = ImageFast.FromFile(Settings.Default.InventoryBackground);
                    }
                    catch (Exception)
                    {
                        Settings.Default.InventoryBackground = "";
                    }
                }
                if (Settings.Default.PlayerBackground != "") // Ensure background is not null.
                {
                    // Catch image missing / renamed errors.
                    try
                    {
                        Main_TabControl.TabPages[1].BackgroundImage = ImageFast.FromFile(Settings.Default.PlayerBackground);
                    }
                    catch (Exception)
                    {
                        Settings.Default.PlayerBackground = "";
                    }
                }
                if (Settings.Default.WorldBackground != "") // Ensure background is not null.
                {
                    // Catch image missing / renamed errors.
                    try
                    {
                        Main_TabControl.TabPages[2].BackgroundImage = ImageFast.FromFile(Settings.Default.WorldBackground);
                    }
                    catch (Exception)
                    {
                        Settings.Default.WorldBackground = "";
                    }
                }
                if (Settings.Default.ChatBackground != "") // Ensure background is not null.
                {
                    // Catch image missing / renamed errors.
                    try
                    {
                        Main_TabControl.TabPages[3].BackgroundImage = ImageFast.FromFile(Settings.Default.ChatBackground);
                    }
                    catch (Exception)
                    {
                        Settings.Default.ChatBackground = "";
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
                toolTip.SetToolTip(CurrentName_TextBox, "Enter the existing loaded player's name.");
                toolTip.SetToolTip(NewName_TextBox, "Enter a custom name. Must match current player's name length.");
                toolTip.SetToolTip(WorldInformation_TextBox, "Enter the name of the world you wish to load.");

                toolTip.SetToolTip(GetInventoryAddresses_Button, "Get the required addresses for editing the inventory.");
                toolTip.SetToolTip(ReloadInventory_Button, "Reload loads the GUI with updated inventory items.");
                toolTip.SetToolTip(RemoveAll_Button, "Remove all items from the inventory.");
                toolTip.SetToolTip(ChanngeName_Button, "Change your existing name.");
                toolTip.SetToolTip(ImportPlayer_Button, "Import a player file to overwrite items.");
                toolTip.SetToolTip(ExportPlayer_Button, "Export a player file to overwrite items.");
                toolTip.SetToolTip(EnableChatCommands_Button, "Enable / disable in-game chat commands.");
                toolTip.SetToolTip(TrashGroundItems_Button, "Removes all ground items not picked up by the player.");
                toolTip.SetToolTip(TeleportXY_Button, "Teleport the player to a desired world position.");
                toolTip.SetToolTip(GetAddresses_Button, "Get the required addresses for using player tools.");
                toolTip.SetToolTip(GetTeleportAddresses_Button, "Get the required addresses for using world tools.");
                toolTip.SetToolTip(ApplyBuff_Button, "Replaces the glow tulip buff with a desired buff.");
                toolTip.SetToolTip(ChangeDate_Button, "Change the date created of the current world.");
                toolTip.SetToolTip(GetWorldInformation_Button, "Fills the datagridview with the world header information.");
                toolTip.SetToolTip(ChangeDifficulty_Button, "Change the difficulty of the current world.");
                toolTip.SetToolTip(ChangeCrystals_Button, "Change the activated crystals of the current world.");
                toolTip.SetToolTip(AutomaticFishing_Button, "Automatically fishes for you. First throw reel into water.");
                toolTip.SetToolTip(RandomTeleport_Button, "Teleport the player to random positions around the map.");
                toolTip.SetToolTip(PreviousInvAddress_Button, "Switch to the previous found inventory.");
                toolTip.SetToolTip(NextInvAddress_Button, "Switch to the next found inventory.");
                toolTip.SetToolTip(AutoMapRenderer_Button, "Automatically render very large areas around the player.");
                toolTip.SetToolTip(RestoreDefaultRange_Button, "Restore the default range and disable full map brightness.");
                toolTip.SetToolTip(SetRevealRange_Button, "Turn on custom map render distance with full map brightness.");
                toolTip.SetToolTip(CancelOperation_Button, "Cancel the map rendering operation.");
                toolTip.SetToolTip(GetMapRenderingAddresses_Button, "Get the required addresses for custom map rendering.");
                toolTip.SetToolTip(PauseOperation_Button, "Pause or resume the auto map rendering operation.");
                toolTip.SetToolTip(OpenChunkVisualizer_Button, "Open a chunk viewer to display real-time chunk position tracking.");
                toolTip.SetToolTip(TeleportPlayerHelp_Button, "Launch a visualization guide on how to set your teleport addresses.");
                toolTip.SetToolTip(ChangeSeed_Button, "Change the seed of the current world.");
                toolTip.SetToolTip(ChangeIcon_Button, "Change the icon of the current world.");
                toolTip.SetToolTip(ChangeConsoleForeColor_Button, "Change the world property editors console color.\nCurrent Color: " + Settings.Default.ConsoleForeColor.Name.ToString());
                toolTip.SetToolTip(OpenSkillEditor_Button, "Launch the player skill editor.");
                // toolTip.SetToolTip(button41, "This is the console color visualizer.");
                toolTip.SetToolTip(ClearDebugLog_Button, "Clear the debug console.");
                toolTip.SetToolTip(ClearWorldToolsLog_Button, "Clear the world tools console.");
                toolTip.SetToolTip(ResetControls_Button, "Used to reset (defaults) all saved control settings across all forms.");

                toolTip.SetToolTip(BuffType_ComboBox, "Open a list of all ingame buffs and debuffs.");
                toolTip.SetToolTip(AppPriority_ComboBox, "Set this applications process priority.");

                toolTip.SetToolTip(SaveEachRing_CheckBox, "Save the map to file after each completed ring.");
                toolTip.SetToolTip(AlwaysOnTop_CheckBox, "Keep this application always on top of other applications.");
                toolTip.SetToolTip(BruteForceTP_CheckBox, "Brute force the address searching for the teleport address.");
                toolTip.SetToolTip(BruteForceTrash_CheckBox, "Brute force the trashing of items by singling out each item.");
                toolTip.SetToolTip(ForceNoclip_Checkbox, "Force noclip to always be on.");
                toolTip.SetToolTip(MapTeleport_CheckBox, "Use the overhead map to left-click teleport to any position.");

                toolTip.SetToolTip(Inventory_RichTextBox, "A list of all found addresses. Used mostly for debugging.");
                toolTip.SetToolTip(PlayerTools_RichTextBox, "A list of all found addresses. Used mostly for debugging.");

                toolTip.SetToolTip(DisplayLocation_ToggleSwitch, "Gets the players XY coordinates and displays it.");
                toolTip.SetToolTip(Godmode_ToggleSwitch, "Enabling will prevent the player from being killed.");
                toolTip.SetToolTip(Speed_ToggleSwitch, "Set a custom run speed for the player.");
                toolTip.SetToolTip(Noclip_ToggleSwitch, "Spacebar will allow the player to pass through walls.");
                toolTip.SetToolTip(InfiniteFood_ToggleSwitch, "Enabling will keep the players food replenished.");
                toolTip.SetToolTip(Suicide_ToggleSwitch, "Enabling this will instantly kill the player.");
                toolTip.SetToolTip(InfiniteResources_ToggleSwitch, "Prevents the diminishing of inventory items.");
                // OBSOLETE: toolTip.SetToolTip(siticoneWinToggleSwith8, "Prevents being killed or teleported while stuck in walls. Use the t-key to toggle.");
                toolTip.SetToolTip(InfiniteMana_ToggleSwitch, "Enabling will keep the players mana replenished.");
                toolTip.SetToolTip(ForceRecall_ToggleSwitch, "Recalls the player to spawn immediately.");
                toolTip.SetToolTip(FreeCrafting_ToggleSwitch, "Enables the ability to craft without consuming resources.");
                toolTip.SetToolTip(PassiveAI_ToggleSwitch, "Toggles enemies aggression towards the player.");
                toolTip.SetToolTip(PlaceAnywhere_ToggleSwitch, "Enabling will allow the player to place on invalid tiles.");
                // BROKEN: toolTip.SetToolTip(siticoneWinToggleSwith13, "Enabling will allow the player to adjust the placement range.");
                toolTip.SetToolTip(Range_ToggleSwitch, "Coming back soon...");
                toolTip.SetToolTip(KeepInventory_ToggleSwitch, "Prevents losing inventory items upon death.");
                toolTip.SetToolTip(TrashInventory_ToggleSwitch, "Enabling will continuously remove all items from the inventory. Items will be logged.");
                // BROKEN: toolTip.SetToolTip(siticoneWinToggleSwith16, "Set a custom max speed for minecarts.");
                toolTip.SetToolTip(MaxMinecartSpeed_ToggleSwitch, "Coming back soon...");
                toolTip.SetToolTip(FreezeItemSlots_ToggleSwitch, "Adds checkboxes to the inventory editor for freezing an items property(s).");

                toolTip.SetToolTip(OverwriteSlotOne_RadioButton, "Overwrite item slot one.");
                toolTip.SetToolTip(AddToEmptySlots_RadioButton, "Add item to an empty inventory slot.");
                toolTip.SetToolTip(Custom_RadioButton, "Add items to a custom inventory slot.");
                toolTip.SetToolTip(WorldDifficulty_ComboBox, "Change the world difficulty (mode).");

                toolTip.SetToolTip(CustomAmount_NumericUpDown, "Change what item slot to send items too.");
                toolTip.SetToolTip(DevToolsDelay_NumericUpDown, "Change the interval of dev-tools that use delays. (default: 80)");
                toolTip.SetToolTip(SpeedAmount_NumericUpDown, "Change the base speed the player will walk at.");
                toolTip.SetToolTip(TeleportX_NumericUpDown, "Change the x-axis world position to be teleported on.");
                toolTip.SetToolTip(TeleportY_NumericUpDown, "Change the y-axis world position to be teleported on.");
                toolTip.SetToolTip(Power_NumericUpDown, "Change the amount of power the buff will contain.");
                toolTip.SetToolTip(TimeS_NumericUpDown, "Change the amount of time the buff will be active for.");
                toolTip.SetToolTip(MaxRadius_NumericUpDown, "The (radius x range) of tiles to render around the player.");
                toolTip.SetToolTip(NextRingDelay_NumericUpDown, "Change the cooldown time (milliseconds) before the next teleport.");
                toolTip.SetToolTip(StartRadius_NumericUpDown, "Set the minimum range in tiles away from the player to start the map render.");
                toolTip.SetToolTip(RenderRange_NumericUpDown, "Set the maximum range in tiles to render the map by.");
                toolTip.SetToolTip(RadialMoveScale_NumericUpDown, "Set a custom radialMoveScale for auto map rendering. (default: 0.1)");
                toolTip.SetToolTip(CastDelay_NumericUpDown, "Set the delay for re-casting the fishing pole.");
                toolTip.SetToolTip(FishingPadding_NumericUpDown, "Set the delay for loop operations. Ex: Caught fish checking.");
                toolTip.SetToolTip(RTDelay_NumericUpDown, "Set the seconds delay for re-teleporting the player.");
                toolTip.SetToolTip(RTRange_NumericUpDown, "Set the max range (radius) to teleport the player within.");

                toolTip.SetToolTip(DevTools3_Label, "Create an ID list from all installed assets.");
                toolTip.SetToolTip(DevTools2_Label, "Sets the variant of item slot2 based on a file list.");
                toolTip.SetToolTip(DevTools1_Label, "Change item slot2s variant based on the left/right arrow keys.");
                toolTip.SetToolTip(DevTools5_Label, "Grabs the application and sends it to the center of the screen.");
                toolTip.SetToolTip(DevTools4_Label, "Stores the games private bytes with timestamps for each completed rotation.");
                toolTip.SetToolTip(MoreMobs_Label, "Use the slider below for more mods!");

                toolTip.SetToolTip(Mods_TrackBar, "Used to scroll to other player mods.");
                toolTip.SetToolTip(MaxMinecartSpeed_TrackBar, "Used to set a custom max speed for minecarts.");
                toolTip.SetToolTip(FormOpacity_TrackBar, "Used to set a custom opacity that applies to all forms.");

                // toolTip.SetToolTip(dataGridView1, "Prints all the world header information.");

                #endregion
            }
            catch (Exception)
            {
            }
        }

        #region Memory Dll Loader

        // Had to make this it's own void because winforms tries to initiate it regardless of checks.
        public void InitiateMemoryDLL()
        {
            try
            {
                // Try creating the Mem object only after the DLL is confirmed to exist.
                MemLib = new Mem();
            }
            catch (FileNotFoundException ex)
            {
                // If the DLL can't be found despite the check, show an error and close.
                MessageBox.Show("Memory.dll could not be loaded: " + ex.Message, errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            catch (Exception ex)
            {
                // Catch all other exceptions and close.
                MessageBox.Show("An unexpected error occurred: " + ex.Message, errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }
        #endregion

        #region Control Logic

        // Checks for administrator.
        public static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent()))
                      .IsInRole(WindowsBuiltInRole.Administrator);
        }

        #region TopMost Checkbox Logic

        // Change the top most variable.
        private void AlwaysOnTop_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Check if the application is already set to top most or not.
            if (!AlwaysOnTop_CheckBox.Checked)
            {
                // Turn top most off.
                this.TopMost = false;

                // Save the property.
                Settings.Default.TopMost = false;
            }
            else
            {
                // Turn top most on.
                this.TopMost = true;

                // Save the property.
                Settings.Default.TopMost = true;
            }
        }
        #endregion

        #region Initialize Buff Editor Dropdown Content

        // Populate combobox upon dropdown.
        private void BuffType_ComboBox_DropDown(object sender, EventArgs e)
        {
            if (BuffType_ComboBox.Items.Count == 0)
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
                            BuffType_ComboBox.Items.Add((string)buffName);
                        }
                    }
                }
            }
        }
        #endregion

        #region Clicked About Link Logic

        // Launch the link in the (users default) browser.
        private void About_RichTextBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                // Open the clicked URL in the default browser.
                Process.Start(new ProcessStartInfo(e.LinkText) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open link: " + ex.Message, errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region Form Closing

        // Reset inventory stats back to defaults.
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Ensure we catch all closing exceptions. // Fix v1.3.3.
            try
            {
                // Save the previous form location before closing if it's not minimized.
                Rectangle activeScreenDimensions = Screen.FromControl(this).Bounds;
                if (WindowState == FormWindowState.Normal && this.Location != new Point(0, activeScreenDimensions.Height - 40) && !isMinimized) // isMinimized fix 1.3.1.
                {
                    Settings.Default.MainFormLocation = this.Location;
                }

                // Save some form settings.
                Settings.Default.ItemAmount = 50;
                Settings.Default.ItemID = 110;
                Settings.Default.CurrentItemTab = "Tab1_TabPage";
                Settings.Default.ItemVariation = 0;

                // Save UI form settings.
                Settings.Default.InventoryBackgroundCount = inventorySkinCounter;
                Settings.Default.PlayerBackgroundCount = playerSkinCounter;
                Settings.Default.WorldBackgroundCount = worldSkinCounter;
                Settings.Default.ChatBackgroundCount = chatSkinCounter;

                // Save some form controls.
                Settings.Default.DevToolDelay = (int)DevToolsDelay_NumericUpDown.Value; // Dev tool operation delay.
                Settings.Default.RadialMoveScale = RadialMoveScale_NumericUpDown.Value; // Auto render maps radialMoveScale.
                Settings.Default.MapRenderingMax = MaxRadius_NumericUpDown.Value;       // Map rendering max radius.
                Settings.Default.MapRenderingStart = StartRadius_NumericUpDown.Value;   // Map rendering start radius.
                Settings.Default.FishingCast = CastDelay_NumericUpDown.Value;           // Fishing bot casting delay.
                Settings.Default.FishingPadding = FishingPadding_NumericUpDown.Value;   // Fishing bot padding delay.
                Settings.Default.FormOpacity = FormOpacity_TrackBar.Value;              // Dev tool form opacity.
                Settings.Default.Save();
            }
            catch (Exception)
            { } // Do nothing.
        }
        #endregion

        #region Form Resize

        // Move window to the bottom left.
        private void MainForm_Resize(object sender, EventArgs e)
        {
            // Get height for both types of taskbar modes.
            Rectangle activeScreenDimensions = Screen.FromControl(this).Bounds;

            // Save the previous form location before minimizing it.
            if (WindowState == FormWindowState.Normal && this.Location != new Point(0, activeScreenDimensions.Height - 40) && !isMinimized) // isMinimized fix 1.3.1.
            {
                Settings.Default.MainFormLocation = this.Location;
            }

            // Get window states.
            if (WindowState == FormWindowState.Minimized && AlwaysOnTop_CheckBox.Checked)
            {
                // Adjust window properties
                this.WindowState = FormWindowState.Normal;
                this.Size = new Size(320, 37);

                // Adjust the form location.
                // Try to place this window top left of the games window.
                Process[] procs = Process.GetProcessesByName("corekeeper");
                if (procs.Length != 0 && !IsIconic(procs[0].MainWindowHandle) && GetWindowRect(procs[0].MainWindowHandle, out RECT rect))
                {
                    // Check if the game is fullscreen or borderless fullscreen. If windowed add a +6 x-buffer.
                    bool hasCaption = (GetWindowLongPtr(procs[0].MainWindowHandle, GWL_STYLE).ToInt64() & WS_CAPTION) != 0;
                    int leftOffset = (hasCaption) ? 6 : 0;
                    this.Location = new Point(rect.Left + leftOffset, rect.Top + 6);
                }
                else
                    this.Location = new Point(0, 6);

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
                if (Main_TabControl.SelectedTab == Inventory_TabPage)   // Inventory.
                {
                    this.Size = new Size(756, 494);
                }
                else if (Main_TabControl.SelectedTab == Player_TabPage) // Player.
                {
                    this.Size = new Size(756, 360);
                }
                else if (Main_TabControl.SelectedTab == World_TabPage)  // World.
                {
                    this.Size = new Size(756, 494);
                }
                else if (Main_TabControl.SelectedTab == Chat_TabPage)   // Chat.
                {
                    this.Size = new Size(410, 360);
                }

                // Adjust some final form settings.
                this.Opacity = Settings.Default.FormOpacity / 100.0;
                this.Location = Settings.Default.MainFormLocation;

                // Adjust minimized bool.
                isMinimized = false;
            }
        }
        #endregion

        #region Switching Tabs

        // Control switching tabs.
        int previousTab = 0;
        private void Main_TabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Main_TabControl.SelectedTab == Inventory_TabPage) // Inventory.
            {
                this.Size = new Size(756, 494);
            }
            else if (Main_TabControl.SelectedTab == Player_TabPage) // Player.
            {
                this.Size = new Size(756, 360);
            }
            else if (Main_TabControl.SelectedTab == World_TabPage) // World.
            {
                this.Size = new Size(756, 494);
            }
            else if (Main_TabControl.SelectedTab == Chat_TabPage) // Chat.
            {
                this.Size = new Size(410, 360);
            }

            // Change skin
            if (Main_TabControl.SelectedTab == ChangeSkin_TabPage)
            {
                // Get the tab we are changing.
                switch (previousTab)
                {

                    case 0: // Inventory
                        // Reset tab page back to one.
                        Main_TabControl.SelectedTab = Inventory_TabPage;

                        // Prevent overflow from add or removal of images.
                        if (inventorySkinCounter >= InventorySkins.Count()) { inventorySkinCounter = 0; }

                        // Ensure the skin exists. Fix: v1.2.9.
                        if (InventorySkins.Count() < 1 || !File.Exists(InventorySkins.ToArray()[inventorySkinCounter])) // Check if folder is empty. Fix: v1.3.4
                        {
                            // Display an error.
                            MessageBox.Show("No skins exist within the asset folder!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Change the background.
                        Main_TabControl.TabPages[0].BackgroundImage = ImageFast.FromFile(InventorySkins.ToArray()[inventorySkinCounter].ToString());

                        // Save the property in the settings.
                        Settings.Default.InventoryBackground = InventorySkins.ToArray()[inventorySkinCounter].ToString();

                        // Add to the counter.
                        inventorySkinCounter++;
                        if (inventorySkinCounter == InventorySkins.Count()) { inventorySkinCounter = 0; }
                        break;
                    case 1: // Player
                        // Reset tab page back to two.
                        Main_TabControl.SelectedTab = Player_TabPage;

                        // Prevent overflow from add or removal of images.
                        if (playerSkinCounter >= PlayerSkins.Count()) { playerSkinCounter = 0; }

                        // Ensure the skin exists. Fix: v1.2.9.
                        if (PlayerSkins.Count() < 1 || !File.Exists(PlayerSkins.ToArray()[playerSkinCounter])) // Check if folder is empty. Fix: v1.3.4
                        {
                            // Display an error.
                            MessageBox.Show("No skins exist within the asset folder!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Change the background.
                        Main_TabControl.TabPages[1].BackgroundImage = ImageFast.FromFile(PlayerSkins.ToArray()[playerSkinCounter].ToString());

                        // Save the property in the settings.
                        Settings.Default.PlayerBackground = PlayerSkins.ToArray()[playerSkinCounter].ToString();

                        // Add to the counter.
                        playerSkinCounter++;
                        if (playerSkinCounter == PlayerSkins.Count()) { playerSkinCounter = 0; }
                        break;
                    case 2: // World
                        // Reset tab page back to two.
                        Main_TabControl.SelectedTab = World_TabPage;

                        // Prevent overflow from add or removal of images.
                        if (worldSkinCounter >= WorldSkins.Count()) { worldSkinCounter = 0; }

                        // Ensure the skin exists. Fix: v1.2.9.
                        if (WorldSkins.Count() < 1 || !File.Exists(WorldSkins.ToArray()[worldSkinCounter])) // Check if folder is empty. Fix: v1.3.4
                        {
                            // Display an error.
                            MessageBox.Show("No skins exist within the asset folder!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Change the background.
                        Main_TabControl.TabPages[2].BackgroundImage = ImageFast.FromFile(WorldSkins.ToArray()[worldSkinCounter].ToString());

                        // Save the property in the settings.
                        Settings.Default.WorldBackground = WorldSkins.ToArray()[worldSkinCounter].ToString();

                        // Add to the counter.
                        worldSkinCounter++;
                        if (worldSkinCounter == WorldSkins.Count()) { worldSkinCounter = 0; }
                        break;
                    case 3: // Chat
                        // Reset tab page back to three.
                        Main_TabControl.SelectedTab = Chat_TabPage;

                        // Prevent overflow from add or removal of images.
                        if (chatSkinCounter >= ChatSkins.Count()) { chatSkinCounter = 0; }

                        // Ensure the skin exists. Fix: v1.2.9.
                        if (ChatSkins.Count() < 1 || !File.Exists(ChatSkins.ToArray()[chatSkinCounter])) // Check if folder is empty. Fix: v1.3.4
                        {
                            // Display an error.
                            MessageBox.Show("No skins exist within the asset folder!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Change the background.
                        Main_TabControl.TabPages[3].BackgroundImage = ImageFast.FromFile(ChatSkins.ToArray()[chatSkinCounter].ToString());

                        // Save the property in the settings.
                        Settings.Default.ChatBackground = ChatSkins.ToArray()[chatSkinCounter].ToString();

                        // Add to the counter.
                        chatSkinCounter++;
                        if (chatSkinCounter == ChatSkins.Count()) { chatSkinCounter = 0; }
                        break;
                }
            }

            // Update the previous tab value.
            previousTab = Main_TabControl.SelectedIndex;
        }
        #endregion

        #endregion // End control logic.

        #endregion // End form controls.

        #region Form Helpers

        /// <summary>
        /// Generates a random integer between the specified minimum (inclusive) and maximum (exclusive) values.
        /// This function uses 'RNGCryptoServiceProvider' over 'Random' to generate close to true random values.
        /// </summary>
        public static int GenerateRandomNumber(int min, int max)
        {
            // Ensure that the input values are valid.
            if (min > max)
                return min; // If min is greater than max, return min.
            if (min == max)
                return min; // If min is equal to max, return min.

            // Use RNGCryptoServiceProvider to generate random bytes.
            using (var rng = new RNGCryptoServiceProvider())
            {
                var data = new byte[8];
                rng.GetBytes(data);

                // Convert the generated bytes to an integer (use startIndex: 0 for the entire byte array).
                int generatedValue = Math.Abs(BitConverter.ToInt32(data, startIndex: 0));

                // Calculate the difference between max and min.
                int diff = max - min;

                // Use modulo to ensure the generated value is within the specified range.
                int mod = generatedValue % diff;

                // Shift the normalized value to be within the specified range.
                int normalizedNumber = min + mod;
                return normalizedNumber;
            }
        }
        #endregion // End form helpers.

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
        private void GetInventoryAddresses_Button_Click(object sender, EventArgs e)
        {
            // Reset progress bar.
            Inventory_ProgressBar.Value = 0;

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
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Name button to indicate loading.
            GetInventoryAddresses_Button.Text = "Loading Addresses...";

            // Disable button to prevent spamming.
            GetInventoryAddresses_Button.Enabled = false;

            // Reset textbox.
            Inventory_RichTextBox.Text = "Addresses Loaded: 0";

            // Offset the progress bar to show it's working.
            Inventory_ProgressBar.Visible = true;
            Inventory_ProgressBar.Maximum = 100;
            Inventory_ProgressBar.Value = 10;

            // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
            // AoB scan is offset +1 bit to increase loading times.
            // Creative update 10May23 added a fifth byte.
            // Depreciated header 10May23: 08 00 00 00 00 00 00.
            // Depreciated header 10Mar25: 0A 00 00 00 00 00 00.
            AoBScanResultsInventory = await MemLib.AoBScan("14 00 00 00 00 00 00 6E 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? 6E 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00", true, true);

            // Get the progress bar maximum.
            Inventory_ProgressBar.Maximum = AoBScanResultsInventory.Count() * 50;

            // If the count is zero, the scan had an error.
            if (AoBScanResultsInventory.Count() == 0)
            {
                // Reset textbox.
                Inventory_RichTextBox.Text = "Addresses Loaded: 0";

                // Reset progress bar.
                Inventory_ProgressBar.Value = 0;
                Inventory_ProgressBar.Visible = false;

                // Rename button back to default.
                GetInventoryAddresses_Button.Text = "Get Inventory Addresses";

                // Re-enable button.
                GetInventoryAddresses_Button.Enabled = true;

                // Reset aob scan results.
                AoBScanResultsInventory = null;

                // Toggle placeholder torches on.
                TogglePlaceholderTorches(true);

                // Display error message.
                MessageBox.Show("You need to have torches in the first and last Inventory slots!!\n\nPlease ignore added inventory rows.\n\nNOTE: This tool is host only! **\n\n\n(** Works on multiplayer but be careful!)", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Update richtextbox with found addresses..
            foreach (long res in AoBScanResultsInventory)
            {
                // Get display address.
                string displayAddress = BigInteger.Subtract(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("1", NumberStyles.Integer)).ToString("X");

                if (Inventory_RichTextBox.Text == "Addresses Loaded: 0")
                {
                    Inventory_RichTextBox.Text = "Addresses Loaded: " + AoBScanResultsInventory.Count().ToString() + ", Selected: " + useAddress + ", [" + displayAddress.ToString();
                }
                else
                {
                    Inventory_RichTextBox.Text += ", " + displayAddress.ToString();
                }
            }
            Inventory_RichTextBox.Text += "]";

            // Enable controls if addresses where found or not.
            if (AoBScanResultsInventory.Count() > 0)
            {
                // Enable controls.
                ReloadInventory_Button.Enabled = true; // Reload.
                RemoveAll_Button.Enabled = true;       // Remove all.

                // If scan is larger then 1 result, enable arrow controls.
                if (AoBScanResultsInventory.Count() > 1)
                {
                    // Enable arrow buttons.
                    PreviousInvAddress_Button.Enabled = true; // Previous.
                    NextInvAddress_Button.Enabled = true;     // Next.
                }
                else
                {
                    // Disable arrow buttons.
                    PreviousInvAddress_Button.Enabled = false; // Previous.
                    NextInvAddress_Button.Enabled = false;     // Next.
                }
            }
            else
            {
                // Disable controls.
                ReloadInventory_Button.Enabled = false;    // Reload.
                RemoveAll_Button.Enabled = false;          // Remove all.
                PreviousInvAddress_Button.Enabled = false; // Previous.
                NextInvAddress_Button.Enabled = false;     // Next.
            }

            // Toggle placeholder torches off.
            TogglePlaceholderTorches(false);

            // Reset item id richtextbox.
            Debug_RichTextBox.Text = "If any unknown items are found, their ID's will appear here!\n------------------------------------------------------------------------------------------------------------\n";

            // Name button to indicate loading.
            GetInventoryAddresses_Button.Text = "Loading Assets...";

            // Load Inventory.
            AddItemToInv(LoadInventory: true);
        }

        // Function for adding items to the players inventory.
        public void AddItemToInv(
            int ItemSlot         = 1,
            int Type             = 1,
            int Variation        = 0,
            int Amount           = 1,
            int Skillset         = 0,
            bool LoadInventory   = false,
            bool CycleAll        = false,
            bool ExportInventory = false,
            bool Overwrite       = false,
            bool GetItemInfo     = false,
            bool AddToEmpty      = false
        )
        {
            #region Add Items Upon Editing

            if (AoBScanResultsInventory == null)
            {
                // Rename button back to default.
                GetInventoryAddresses_Button.Text = "Get Inventory Addresses";
                ReloadInventory_Button.Text = "Reload Inventory";
                RemoveAll_Button.Text = "Remove All";

                // Reset progress bar.
                Inventory_ProgressBar.Value = 0;
                Inventory_ProgressBar.Visible = false;

                if (!LoadInventory) // Prevent double error messages. // Fix: v1.3.4.7.
                    MessageBox.Show("You need to first scan for the Inventory addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);

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
                    ImportExport_ProgressBar.Value = 0;
                }
                else
                {
                    return;
                }
            }

            // Define some variables for item info.
            int infoType = 0;
            int infoAmount = 0;
            int infoVariant = 0;
            int infoSkillset = 0;

            // Define a variable to hold the new item Amount information.
            int finalItemAmount = 0;

            // Select the inventory to use.
            var res = AoBScanResultsInventory.ElementAt(useAddress - 1);

            // Get address from loop.
            // Base address was moved 9 bits.
            string baseAddress = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("7", NumberStyles.Integer)).ToString("X");

            // Count the total amount of pictureboxes on the inventory tab.
            int slotCount = Main_TabControl.TabPages["Inventory_TabPage"].Controls.OfType<PictureBox>().Count();

            #region Set Inventory Items

            // Remove existing images.
            if (LoadInventory || CycleAll)
            {
                for (int i = 1; i <= slotCount; i++)
                {
                    if (this.Controls.Find($"Slot{i}_PictureBox", searchAllChildren: true).FirstOrDefault() is PictureBox ctrl) ctrl.Image = null;
                }
            }

            // Make some exception catches.
            try
            {
                // Dynamically populate each item slot.
                for (int i = 1; i <= slotCount; i++)
                {
                    // Define the picturebox control.
                    var slotPictureBoxControl = this.Controls.Find($"Slot{i}_PictureBox", searchAllChildren: true).FirstOrDefault() as PictureBox;

                    // Get Offsets for Inventory.
                    if (!AddToEmpty && (ItemSlot == i || LoadInventory || CycleAll || ExportInventory))
                    {
                        // Dynamically get the addresses for each slot property.
                        string slotItem      = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse((0  + (20 * (i - 1))).ToString(), NumberStyles.Integer)).ToString("X");
                        string slotAmount    = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse((4  + (20 * (i - 1))).ToString(), NumberStyles.Integer)).ToString("X");
                        string slotVariation = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse((8  + (20 * (i - 1))).ToString(), NumberStyles.Integer)).ToString("X");
                        string slotSkillset  = BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse((16 + (20 * (i - 1))).ToString(), NumberStyles.Integer)).ToString("X");

                        // Perform progress step.
                        Inventory_ProgressBar.PerformStep();

                        // Set Values
                        if (!LoadInventory && !ExportInventory && !GetItemInfo)
                        {
                            // Add New Item
                            MemLib.WriteMemory(slotItem, "int", Type.ToString()); // Write item Type
                            if (Type == 0)
                            {
                                MemLib.WriteMemory(slotAmount, "int", "0"); // Write item Amount
                                MemLib.WriteMemory(slotVariation, "int", "0"); // Write item Variation
                                MemLib.WriteMemory(slotSkillset, "int", "0"); // Write item Skillset
                                finalItemAmount = 0;
                            }
                            else
                            {
                                if (Overwrite)
                                {
                                    MemLib.WriteMemory(slotAmount, "int", Amount.ToString()); // Write item Amount
                                    MemLib.WriteMemory(slotVariation, "int", Variation.ToString()); // Write item Variation
                                    MemLib.WriteMemory(slotSkillset, "int", Skillset.ToString()); // Write item Skillset
                                    finalItemAmount = Amount;
                                }
                                else
                                {
                                    MemLib.WriteMemory(slotAmount, "int", (MemLib.ReadUInt(slotAmount) + Amount).ToString()); // Write item Amount
                                    MemLib.WriteMemory(slotVariation, "int", Variation.ToString()); // Write item Variation
                                    MemLib.WriteMemory(slotSkillset, "int", Skillset.ToString()); // Write item Skillset
                                    finalItemAmount = (int)MemLib.ReadUInt(slotAmount);
                                }
                            }
                        }
                        else
                        {
                            // Export inventory to list.
                            if (ExportInventory)
                            {
                                if (!ExportPlayerItems.ContainsKey($"itemSlot{i}"))
                                {
                                    ExportPlayerItems.Add($"itemSlot{i}-ID", MemLib.ReadInt(slotItem));
                                    ExportPlayerItems.Add($"itemSlot{i}-Amount", MemLib.ReadInt(slotAmount));
                                    ExportPlayerItems.Add($"itemSlot{i}-Variation", MemLib.ReadInt(slotVariation));
                                    ExportPlayerItems.Add($"itemSlot{i}-Skillset", MemLib.ReadInt(slotSkillset));
                                }
                            }
                            else
                            {
                                // First Load
                                Type = MemLib.ReadInt(slotItem);
                                Variation = MemLib.ReadInt(slotVariation);
                                Skillset = MemLib.ReadInt(slotSkillset);

                                // Load Picture
                                // Set image to null if Type is zero.
                                if (Type.ToString() == "0")
                                {
                                    slotPictureBoxControl.Image = null;
                                }
                                else if (slotPictureBoxControl.Image == null)
                                {
                                    // Get Picture
                                    try
                                    {
                                        // Check if image plus Variation exists.
                                        if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == Type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (Variation == 0 ? 0 : Variation).ToString()) != null)
                                        {
                                            slotPictureBoxControl.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == Type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (Variation == 0 ? 0 : Variation).ToString()))); // Check if file matches current Type, set it.
                                            slotPictureBoxControl.SizeMode = PictureBoxSizeMode.Zoom;
                                        }
                                        else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == Type.ToString()) != null)
                                        {
                                            // Image without Variation exists.
                                            slotPictureBoxControl.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == Type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current Type, set it.
                                            slotPictureBoxControl.SizeMode = PictureBoxSizeMode.Zoom;
                                        }
                                        else
                                        {
                                            // No image found.
                                            slotPictureBoxControl.Image = Resources.UnknownItem;
                                            slotPictureBoxControl.SizeMode = PictureBoxSizeMode.Zoom;

                                            // Do debug information.
                                            if (Array.Exists(Debug_RichTextBox.Lines, element => element == $"ItemSlot: {i} | ItemID: {Type} | Amount: {MemLib.ReadInt(slotAmount)} | Variation: {MemLib.ReadInt(slotVariation)} | Skillset: {MemLib.ReadInt(slotSkillset)}") == false) // Check if entree exists already.
                                            {
                                                Debug_RichTextBox.AppendText($"ItemSlot: {i} | ItemID: {Type} | Amount: {MemLib.ReadInt(slotAmount)} | Variation: {MemLib.ReadInt(slotVariation)} | Skillset: {MemLib.ReadInt(slotSkillset)}" + Environment.NewLine); // Record the missing values.
                                            }
                                        }

                                        // Draw item Amount.
                                        using (Font font = new Font("Arial", 24f))
                                        using (Graphics G = Graphics.FromImage(slotPictureBoxControl?.Image))
                                        using (GraphicsPath gp = new GraphicsPath())
                                        {
                                            // Do drawling actions.
                                            gp.AddString(MemLib.ReadInt(slotAmount).ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                            G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                            G.FillPath(new SolidBrush(Color.White), gp);
                                        }
                                        slotPictureBoxControl?.Invalidate(); // Reload picturebox.
                                    }
                                    catch (Exception) { } // Swallow safely.
                                }
                            }
                        }

                        // Do some information stuff.
                        if (GetItemInfo)
                        {
                            infoType = MemLib.ReadInt(slotItem);
                            infoAmount = MemLib.ReadInt(slotAmount);
                            infoVariant = MemLib.ReadInt(slotVariation);
                            infoSkillset = MemLib.ReadInt(slotSkillset);
                        }
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

            // Announce the information.
            if (GetItemInfo)
            {
                // Get the name of the item.
                string baseItemName = "";
                string baseingredient1Name = "";
                string baseingredient2Name = "";

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
                // Check if the items variant is an length of 1. 
                if (infoVariant >= 1)
                {
                    // Get base item ingredient 1 name.
                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == VariationHelper.GetIngredient1FromFoodVariation(infoVariant).ToString()) != null)
                    {
                        baseingredient1Name = Path.GetFileName(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == VariationHelper.GetIngredient1FromFoodVariation(infoVariant).ToString())).Split(',')[0];
                    }
                    else
                    {
                        // Check if the item id is 0.
                        if (infoType == 0)
                        {
                            baseingredient1Name = "Empty";
                        }
                        else
                        {
                            baseingredient1Name = "UnkownItem";
                        }
                    }
                    // Get base item ingredient 2 name.
                    if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == VariationHelper.GetIngredient2FromFoodVariation(infoVariant).ToString()) != null)
                    {
                        baseingredient2Name = Path.GetFileName(ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == VariationHelper.GetIngredient2FromFoodVariation(infoVariant).ToString())).Split(',')[0];
                    }
                    else
                    {
                        // Check if the item id is 0.
                        if (infoType == 0)
                        {
                            baseingredient2Name = "Empty";
                        }
                        else
                        {
                            baseingredient2Name = "UnkownItem";
                        }
                    }
                }

                // Check if the items variant is an length of 1. 
                string itemMessage = "";
                if (infoVariant >= 1)
                {
                    itemMessage = $"Inventory Slot {ItemSlot}'s Item Info:\n\nName: {baseItemName}\nBase ID: {infoType}\nAmount: {infoAmount}\nSkillset: {infoSkillset}\n\nVariant IDs: ({infoVariant})\n- ingredient1: {baseingredient1Name} [{VariationHelper.GetIngredient1FromFoodVariation(infoVariant)}]\n- ingredient2: {baseingredient2Name} [{VariationHelper.GetIngredient2FromFoodVariation(infoVariant)}]";
                }
                else
                {
                    itemMessage = $"Inventory Slot {ItemSlot}'s Item Info:\n\nName: {baseItemName}\nBase ID: {infoType}\nAmount: {infoAmount}\nSkillset: {infoSkillset}\n\nVariant ID: {infoVariant}";
                }

                // Display informational messagebox.
                MessageBox.Show(itemMessage, "Item Information:", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            #endregion // End announce item info.

            #endregion // End adding items upon editing.

            #region Load Pictures Upon Editing

            // Define variable for addtoempty.
            bool emptySlotFound = false;

            // Dynamically populate each item slot.
            for (int i = 1; i <= slotCount; i++)
            {
                // Define the picturebox control.
                var slotPictureBoxControl = this.Controls.Find($"Slot{i}_PictureBox", searchAllChildren: true).FirstOrDefault() as PictureBox;

                // Load Picture
                if (!LoadInventory && !GetItemInfo && (ItemSlot == i || CycleAll || AddToEmpty))
                {
                    // If slot is empty, add item to inventory.
                    if (AddToEmpty && slotPictureBoxControl.Image == null && !emptySlotFound)
                    {
                        // Add item to inventory.
                        AddItemToInv(ItemSlot: i, Type: Type, Amount: Amount, Variation: Variation);

                        // Update bool.
                        emptySlotFound = true;
                    }
                    else if (!AddToEmpty)
                    {
                        // Perform progress step.
                        Inventory_ProgressBar.PerformStep();

                        // Set image to null if Type is zero.
                        if (Type.ToString() == "0")
                        {
                            slotPictureBoxControl.Image = null;
                        }
                        else
                        {
                            // Get Picture
                            try
                            {
                                // Check if image plus Variation exists.
                                if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == Type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (Variation == 0 ? 0 : Variation).ToString()) != null)
                                {
                                    slotPictureBoxControl.Image = new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == Type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == (Variation == 0 ? 0 : Variation).ToString()))); // Check if file matches current Type, set it.
                                    slotPictureBoxControl.SizeMode = PictureBoxSizeMode.Zoom;
                                }
                                else if (ImageFiles1.FirstOrDefault(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == Type.ToString()) != null)
                                {
                                    // Image without Variation exists.
                                    slotPictureBoxControl.Image = MakeGrayscale3(new Bitmap(ImageFast.FromFile(ImageFiles1.First(x => new FileInfo(x).Name.Split(',')[0] != "desktop.ini" && new FileInfo(x).Name.Split(',')[0] != "Thumbs.db" && new FileInfo(x).Name.Split(',')[1] == Type.ToString() && new FileInfo(x).Name.Split(',')[2].Split('.')[0] == "0")))); // Check if file matches current Type, set it.
                                    slotPictureBoxControl.SizeMode = PictureBoxSizeMode.Zoom;
                                }
                                else
                                {
                                    // No image found.
                                    slotPictureBoxControl.Image = Resources.UnknownItem;
                                    slotPictureBoxControl.SizeMode = PictureBoxSizeMode.Zoom;
                                }

                                // Draw item Amount.
                                using (Font font = new Font("Arial", 24f))
                                using (Graphics G = Graphics.FromImage(slotPictureBoxControl?.Image))
                                using (GraphicsPath gp = new GraphicsPath())
                                {
                                    // Do drawling actions.
                                    gp.AddString(finalItemAmount.ToString(), font.FontFamily, (int)font.Style, font.Size, ClientRectangle, new StringFormat());
                                    G.DrawPath(new Pen(Color.Black, 4) { LineJoin = LineJoin.Round }, gp);
                                    G.FillPath(new SolidBrush(Color.White), gp);
                                }
                                slotPictureBoxControl?.Invalidate(); // Reload picturebox.
                            }
                            catch (Exception) { } // Swallow safely.
                        }
                    }
                }
            }
            #endregion

            // Check if there was no slots empty.
            if (AddToEmpty && !emptySlotFound)
            {
                MessageBox.Show("Your inventory is full!" + Environment.NewLine + Environment.NewLine + "Try using the reload inventory button.", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Rename button back to default.
            GetInventoryAddresses_Button.Text = "Get Inventory Addresses";
            ReloadInventory_Button.Text = "Reload Inventory";
            RemoveAll_Button.Text = "Remove All";

            // Re-enable button.
            GetInventoryAddresses_Button.Enabled = true;

            // Rehide the progressbar.
            Inventory_ProgressBar.Visible = false;
        }

        // Previous address button.
        private void PreviousInvAddress_Button_Click(object sender, EventArgs e)
        {
            // Reset progress bar.
            Inventory_ProgressBar.Value = 0;

            // Subtract from the use address if its not one.
            useAddress = (useAddress == 1) ? 1 : useAddress - 1;

            // Update the rich textbox.
            Inventory_RichTextBox.Text = "Addresses Loaded: 0";
            foreach (long res in AoBScanResultsInventory)
            {
                if (Inventory_RichTextBox.Text == "Addresses Loaded: 0")
                {
                    Inventory_RichTextBox.Text = "Addresses Loaded: " + AoBScanResultsInventory.Count().ToString() + ", Selected: " + useAddress + ", [" + res.ToString("X").ToString();
                }
                else
                {
                    Inventory_RichTextBox.Text += ", " + res.ToString("X").ToString();
                }
            }
            Inventory_RichTextBox.Text += "]";

            // Load addresses.
            AddItemToInv(LoadInventory: true);
        }

        // Toggle on and off the placeholder torches.
        public void TogglePlaceholderTorches(bool Enabled)
        {
            // Check to enable or disable the placeholder torches.
            if (Enabled)
            {
                // Enable placeholder torches.

                Slot1_PictureBox.Image = MakeGrayscale3(new Bitmap(Resources.TorchPlaceholder));
                Slot1_PictureBox.SizeMode = PictureBoxSizeMode.Zoom;

                Slot30_PictureBox.Image = MakeGrayscale3(new Bitmap(Resources.TorchPlaceholder));
                Slot30_PictureBox.SizeMode = PictureBoxSizeMode.Zoom;

                Slot1_PictureBox.Invalidate(); // Reload picturebox.
                Slot30_PictureBox.Invalidate(); // Reload picturebox.
            }
            else
            {
                // Disable placeholder torches.

                Slot1_PictureBox.Image = null;
                Slot30_PictureBox.Image = null;
            }
        }

        // Next address button.
        private void NextInvAddress_Button_Click(object sender, EventArgs e)
        {
            // Reset progress bar.
            Inventory_ProgressBar.Value = 0;

            // Add to the use address if its not the max.
            useAddress = (AoBScanResultsInventory != null && useAddress == AoBScanResultsInventory.Count()) ? AoBScanResultsInventory.Count() : useAddress + 1;

            // Update the rich textbox.
            Inventory_RichTextBox.Text = "Addresses Loaded: 0";
            foreach (long res in AoBScanResultsInventory)
            {
                if (Inventory_RichTextBox.Text == "Addresses Loaded: 0")
                {
                    Inventory_RichTextBox.Text = "Addresses Loaded: " + AoBScanResultsInventory.Count().ToString() + ", Selected: " + useAddress + ", [" + res.ToString("X").ToString();
                }
                else
                {
                    Inventory_RichTextBox.Text += ", " + res.ToString("X").ToString();
                }
            }
            Inventory_RichTextBox.Text += "]";

            // Load addresses.
            AddItemToInv(LoadInventory: true);
        }

        // Reload Inventory.
        private void ReloadInventory_Button_Click(object sender, EventArgs e)
        {
            // Change button labels.
            ReloadInventory_Button.Text = "Loading..";

            // Reset progress bar.
            Inventory_ProgressBar.Value = 0;
            Inventory_ProgressBar.Visible = true;

            // Reload Inventory.
            AddItemToInv(LoadInventory: true);
        }

        // Remove entire Inventory.
        private void RemoveAll_Button_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Delete ALL items? Are you sure?", "Remove All", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                // Change button labels.
                RemoveAll_Button.Text = "Clearing..";

                // Reload Inventory.
                AddItemToInv(Type: 0, Amount: 1, Variation: 0, CycleAll: true);
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
                InfiniteResources_ToggleSwitch.CheckedChanged -= InfiniteResources_ToggleSwitch_CheckedChanged;
                InfiniteResources_ToggleSwitch.Checked = false;
                InfiniteResources_ToggleSwitch.CheckedChanged += InfiniteResources_ToggleSwitch_CheckedChanged;
            }

            PictureBox pic = sender as PictureBox;

            // Ensure picturebox control exists.
            if (pic != null)
            {
                if (e.Button == MouseButtons.Left) // Load inventory editor.
                {
                    // Before showing the InventoryEditor popup form, check if the images are already loaded in memory cache.
                    // If not, load them now (this only happens once).
                    if (!InventoryImageCache.IsLoaded)
                    {
                        // Loads all images from disk into memory (static cache).
                        // Pass the FolderNames to tell it what folders to load.
                        InventoryImageCache.LoadAllImages(ItemSelectionMenu.FolderNames);
                    }

                    // Get the picturebox selected number.
                    int slotNumber = int.Parse(pic.Name.Replace("Slot", "").Replace("_PictureBox", ""));

                    // Spawn item picker window.
                    ItemSelectionMenu itemSelectionMenu = new ItemSelectionMenu();
                    DialogResult dr = itemSelectionMenu.ShowDialog(this);

                    // Get returned item from picker.
                    int itemType       = itemSelectionMenu.GetItemTypeFromList();
                    int itemAmount     = itemSelectionMenu.GetItemAmountFromList();
                    int itemVariation  = itemSelectionMenu.GetItemVeriationFromList() == 0 ? 0 : (itemSelectionMenu.GetItemVeriationFromList()); // If variation is not zero, add offset.
                    int itemSkillset   = itemSelectionMenu.GetItemSkillsetFromList()  == 0 ? 0 : (itemSelectionMenu.GetItemSkillsetFromList());  // If skillset is not zero, add offset.
                    bool wasAborted    = itemSelectionMenu.GetUserCanceldTask();
                    bool itemOverwrite = itemSelectionMenu.GetSelectedOverwriteTask();
                    itemSelectionMenu.Close();

                    // Check if user closed the form
                    if (wasAborted) { return; };

                    // Spawn the item.
                    AddItemToInv(slotNumber, Type: itemType, Amount: itemAmount, Variation: itemVariation, Skillset: itemSkillset, Overwrite: itemOverwrite);

                    // Reload Inventory. Added: v1.3.4.5.
                    AddItemToInv(LoadInventory: true);
                }
                else if (e.Button == MouseButtons.Middle) // Get item stats.
                {
                    // Get the picturebox selected number.
                    int slotNumber = int.Parse(pic.Name.Replace("Slot", "").Replace("_PictureBox", ""));

                    // Get item stats.
                    AddItemToInv(slotNumber, GetItemInfo: true);

                    // Reload Inventory. Added: v1.3.4.5.
                    AddItemToInv(LoadInventory: true);
                }
                else if (e.Button == MouseButtons.Right) // Change item amount value.
                {
                    // Open the process and check if it was successful before the AoB scan.
                    if (AoBScanResultsInventory == null)
                    {
                        // Rename button back to default.
                        GetInventoryAddresses_Button.Text = "Get Inventory Addresses";
                        ReloadInventory_Button.Text = "Reload Inventory";
                        RemoveAll_Button.Text = "Remove All";

                        // Reset progress bar.
                        Inventory_ProgressBar.Value = 0;
                        Inventory_ProgressBar.Visible = false;

                        MessageBox.Show("You need to first scan for the Inventory addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Get the picturebox selected number.
                    int slotNumber = int.Parse(pic.Name.Replace("Slot", "").Replace("_PictureBox", ""));

                    // Get item slot values.
                    int[] itemInfo = GetSlotInfo(slotNumber);

                    // Save some form settings.
                    Settings.Default.InfoID = itemInfo[0];
                    Settings.Default.InfoAmount = Math.Abs(itemInfo[1]); // Fix negative numbers throwing an exception. // Fix v1.3.4.4.
                    Settings.Default.InfoVariation = itemInfo[2] == 0 ? 0 : (itemInfo[2]); // Ensure variant gets translated correctly.
                    Settings.Default.InfoSkillset = itemInfo[3];

                    // Spawn item picker window.
                    ItemEditor itemEditor = new ItemEditor();
                    DialogResult dr = itemEditor.ShowDialog(this);

                    // Get returned item from picker.
                    int itemType      = itemEditor.GetItemTypeFromList();
                    int itemAmount    = itemEditor.GetItemAmountFromList();
                    int itemVariation = itemEditor.GetItemVeriationFromList() == 0 ? 0 : (itemEditor.GetItemVeriationFromList()); // If variation is not zero, add offset.
                    int itemSkillset  = itemEditor.GetItemSkillsetFromList()  == 0 ? 0 : (itemEditor.GetItemSkillsetFromList());  // If skillset is not zero, add offset.
                    bool wasAborted   = itemEditor.GetUserCanceldTask();
                    // bool itemOverwrite = itemEditor.GetSelectedOverwriteTask();
                    itemEditor.Close();

                    // Check if user closed the form
                    if (wasAborted) { return; };

                    // Edit the item.
                    AddItemToInv(slotNumber, Type: itemType, Amount: itemAmount, Variation: itemVariation, Skillset: itemSkillset, Overwrite: true);

                    // Reload Inventory. Added: v1.3.4.5.
                    AddItemToInv(LoadInventory: true);
                }
            }
        }

        #endregion // Click Events

        #region Get itemSlot Values

        // Get item amount.
        public int[] GetSlotInfo(int itemSlot)
        {
            // Define main string.
            int[] itemInfo = new int[4];

            // Define some variables for item info.
            int infoType     = 0;
            int infoAmount   = 0;
            int infoVariant  = 0;
            int infoSkillset = 0;

            // Select the inventory to use.
            var res = AoBScanResultsInventory.ElementAt(useAddress - 1);

            // Get address from loop.
            // Base address was moved 9 bits.
            string baseAddress = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("7", NumberStyles.Integer)).ToString("X");

            // Count the total amount of pictureboxes on the inventory tab.
            int slotCount = Main_TabControl.TabPages["Inventory_TabPage"].Controls.OfType<PictureBox>().Count();

            // Make some exception catches.
            try
            {
                // Dynamically populate each item slot.
                for (int i = 1; i <= slotCount; i++)
                {
                    if (itemSlot == i)
                    {
                        // Dynamically get the addresses for each slot property.
                        infoType     = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse((0  + (20 * (i - 1))).ToString(), NumberStyles.Integer)).ToString("X"));
                        infoAmount   = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse((4  + (20 * (i - 1))).ToString(), NumberStyles.Integer)).ToString("X"));
                        infoVariant  = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse((8  + (20 * (i - 1))).ToString(), NumberStyles.Integer)).ToString("X"));
                        infoSkillset = (int)MemLib.ReadUInt(BigInteger.Add(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse((16 + (20 * (i - 1))).ToString(), NumberStyles.Integer)).ToString("X"));
                        break; // Break loop once found.
                    }
                    else if (i < slotCount || i > slotCount) // Prevent out of range errors.
                    {
                        infoType     = 0;
                        infoAmount   = 0;
                        infoVariant  = 0;
                        infoSkillset = 0;
                    }
                }
            }
            catch (Exception)
            {
                // Do nothing.
            }

            // Define item info string.
            itemInfo[0] = infoType;
            itemInfo[1] = infoAmount;
            itemInfo[2] = infoVariant;
            itemInfo[3] = infoSkillset;

            // Return value.
            return itemInfo;
        }

        #endregion // Get itemSlot Values

        #endregion // End Inventory Region

        #region Player Tab

        #region Player Mod Offsets

        // Below contains all the offsets for the player mods.
        // These values are all added to the players base address. // Base + Offset.
        readonly string positionX_Offset = "0";                    // Player position X.
        readonly string positionY_Offset = "4";                    // Player position Y.
        readonly string godmode_Offset   = "1248";                 // Godmode.
        readonly string speed_Offset     = "2808";                 // Player speed.  // 336 default.
        readonly string hunger_Offset    = "2912";                 // Player hunger. // What even is this game anymore LOL.
        readonly string mana_Offset      = "3808";                 // Player mana.
        readonly string noclip_Offset    = "6624";                 // Noclip.        // 4 = off, 32 = on, 524288 = Recall (wait 5s).
        readonly string passiveAI_Offset = "7716";                 // Passive AI.    // 5 = off, 8  = on.

        // Obsolete Offsets 05Feb25:
        // readonly string playerStateBaseOffset = "248";          // For player effects like recall and anti collision.
        // readonly string playerStateAntiCollisionOffset = "312"; // Anti collision effect.
        // readonly string playerStateRecallOffset = "384";        // Recall effect.

        #endregion

        #region Move Players Tab

        // Move the player panel horizontally.
        private void Mods_TrackBar_ValueChanged(object sender, EventArgs e)
        {
            PlayerTools_Panel.AutoScrollMargin = new Size(500, 0); // Define amount of space to scroll over too.
            PlayerTools_Panel.AutoScrollPosition = new Point((PlayerTools_Panel.AutoScrollMargin.Width / 100) * Mods_TrackBar.Value, 0);
            PlayerTools_Panel.AutoScroll = false; // Prevent the scrollbars.

            // Force refresh controls.
            PlayerTools_Panel.Refresh();
            Mods_TrackBar.Refresh();
        }
        #endregion

        #region Change Player Name

        // Change player name.
        private async void ChanngeName_Button_Click(object sender, EventArgs e)
        {
            // Ensure properties are filled.
            if (CurrentName_TextBox.Text == "" || NewName_TextBox.Text == "")
            {
                // Display error message.
                MessageBox.Show("You must type your player name and a new name!!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Name button to indicate loading.
            ChanngeName_Button.Text = "Changing Name..";

            // Get current player name.
            StringBuilder builder = new StringBuilder();
            foreach (char c in CurrentName_TextBox.Text)
            {
                builder.Append(Convert.ToInt64(c).ToString("X"));
            }

            // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
            AoBScanResultsPlayerName = await MemLib.AoBScan(string.Join(string.Empty, builder.ToString().Select((x, i) => i > 0 && i % 2 == 0 ? string.Format(" {0}", x) : x.ToString())), true, true);

            // If the count is zero, the scan had an error.
            if (AoBScanResultsPlayerName.Count() == 0 | AoBScanResultsPlayerName.Count() < 10)
            {
                // Rename button back to default.
                ChanngeName_Button.Text = "Change Name";

                // Display error message.
                MessageBox.Show("Your name must be of your current in-game player!!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Change players name.
            ChangePlayersName(NewName_TextBox.Text, NewName_TextBox.Text.Length);
        }

        // this function is async, which means it does not block other code.
        public void ChangePlayersName(string NewName, int NewLength)
        {
            // Iterate through each found address.
            foreach (long res in AoBScanResultsPlayerName)
            {
                // Get address from loop.
                string baseAddress = res.ToString("X").ToString();
                string nameLengthAddress = BigInteger.Subtract(BigInteger.Parse(baseAddress, NumberStyles.HexNumber), BigInteger.Parse("2", NumberStyles.Integer)).ToString("X");

                // Write new name length.
                MemLib.WriteMemory(nameLengthAddress, "int", NewLength.ToString());

                // Write new name to addresses.
                MemLib.WriteMemory(baseAddress, "string", NewName);
            }

            // Process completed, run finishing tasks.
            // Rename button back to default.
            ChanngeName_Button.Text = "Change Name";
            CurrentName_TextBox.Text = NewName;
            NewName_TextBox.Text = "";
        }

        #endregion // End change playername.

        #region Import / Export

        // Import a player file.
        private void ImportPlayer_Button_Click(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsInventory == null)
            {
                // Rename button back to default.
                GetInventoryAddresses_Button.Text = "Get Inventory Addresses";
                ReloadInventory_Button.Text = "Reload Inventory";
                RemoveAll_Button.Text = "Remove All";

                MessageBox.Show("You need to first scan for the Inventory addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                ImportExport_ProgressBar.Step = 25;
                ImportExport_ProgressBar.Value = 0;

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
                                int itemSkillset = 0;

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
                                        ImportExport_ProgressBar.PerformStep();
                                    }
                                    else if (ex.Key.Contains("-Amount"))
                                    {
                                        // Get item amount.
                                        string slotNumberAmount = "itemSlot" + ItemSlotCount.ToString() + "-Amount";
                                        itemAmount = int.Parse(playerData[slotNumberAmount].ToString().Replace("itemSlot", "").Replace("-Amount", ""));

                                        // Advance the protgress bar.
                                        ImportExport_ProgressBar.PerformStep();
                                    }
                                    else if (ex.Key.Contains("-Variation"))
                                    {
                                        // Get item variation.
                                        string slotNumberVariation = "itemSlot" + ItemSlotCount.ToString() + "-Variation";
                                        itemVariation = int.Parse(playerData[slotNumberVariation].ToString().Replace("itemSlot", "").Replace("-Variation", ""));

                                        // Advance the protgress bar.
                                        ImportExport_ProgressBar.PerformStep();
                                    }
                                    else if (ex.Key.Contains("-Skillset"))
                                    {
                                        // Get item skillset.
                                        string slotNumberSkillset = "itemSlot" + ItemSlotCount.ToString() + "-Skillset";
                                        itemSkillset = int.Parse(playerData[slotNumberSkillset].ToString().Replace("itemSlot", "").Replace("-Skillset", ""));

                                        // Advance the protgress bar.
                                        ImportExport_ProgressBar.PerformStep();

                                        // Add the item to the inventory.
                                        if (itemVariation == 0)
                                        {
                                            AddItemToInv(ItemSlot: ItemSlotCount, Type: itemID, Amount: itemAmount, Variation: itemVariation, Skillset: itemSkillset, Overwrite: true);
                                        }
                                        else
                                        {
                                            AddItemToInv(ItemSlot: ItemSlotCount, Type: itemID, Amount: itemAmount, Variation: itemVariation, Skillset: itemSkillset, Overwrite: true);
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
                    MessageBox.Show("There was an error reading this file!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // Ensure progressbar is at 100.
                ImportExport_ProgressBar.Value = 100;
            }
        }

        // Export a player file.
        private void ExportPlayer_Button_Click(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsInventory == null)
            {
                // Rename button back to default.
                GetInventoryAddresses_Button.Text = "Get Inventory Addresses";
                ReloadInventory_Button.Text = "Reload Inventory";
                RemoveAll_Button.Text = "Remove All";

                MessageBox.Show("You need to first scan for the Inventory addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Export the inventory.
            AddItemToInv(ExportInventory: true);

            // Advance progress bar.
            ImportExport_ProgressBar.Value = 100;
        }

        #endregion // End import & export.

        #region Player Tool Addresses

        // Get player address.
        private void GetAddresses_Button_Click(object sender, EventArgs e)
        {
            // Reset progress bar.
            PlayerTools_ProgressBar.Value = 0;

            // Load addresses.
            GetPlayerToolsAddresses();
        }

        public async void GetPlayerToolsAddresses()
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Name button to indicate loading.
            GetAddresses_Button.Text = "Loading...";

            // Disable button to prevent spamming.
            // button10.Enabled = false;
            PlayerTools_GroupBox.Enabled = false;

            // Reset textbox.
            PlayerTools_RichTextBox.Text = "Addresses Loaded: 0";

            // Offset the progress bar to show it's working.
            PlayerTools_ProgressBar.Visible = true;
            PlayerTools_ProgressBar.Maximum = 100;
            PlayerTools_ProgressBar.Step = 45;
            PlayerTools_ProgressBar.Value = 10;

            // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
            // Depreciated Address 08Feb23: ?? 99 D9 3F ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 99 D9 3F
            // Depreciated Address 10May23: ?? 99 D9 3F ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 99 D9 3F
            // Depreciated Address 04Oct23: ?? 99 D9 3F ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 99 D9 3F
            // Depreciated Address 05Feb25: ?? 00 00 3E ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 99 D9 3F
            // Depreciated Address 10Mar25: ?? 99 19 3E ?? 99 D9 3F 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? 3C 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? 99 19 3E 00 00 00 00 ?? 99 D9 3F
            // Depreciated Address 28May25: ?? 99 19 3E 00 00 00 00 ?? 99 D9 3F ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 99 19 3E 00 00 00 00 ?? 99 D9 3F
            AoBScanResultsPlayerTools = await MemLib.AoBScan("?? 99 19 3E ?? 99 D9 3F ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 99 19 3E ?? 99 D9 3F", true, true);

            // If the count is zero, the scan had an error.
            if (AoBScanResultsPlayerTools.Count() < 1)
            {
                // Reset textbox.
                PlayerTools_RichTextBox.Text = "Addresses Loaded: 0";

                // Reset progress bar.
                PlayerTools_ProgressBar.Value = 0;
                PlayerTools_ProgressBar.Visible = false;

                // Rename button back to default.
                GetAddresses_Button.Text = "Get Addresses";

                // Re-enable button.
                //button10.Enabled = true;
                PlayerTools_GroupBox.Enabled = true;

                // Reset aob scan results
                AoBScanResultsPlayerTools = null;

                // Display error message.
                MessageBox.Show("You must be standing at the core's entrance!!\r\rTIP: Press 'W' & 'D' keys when at the core's entrance.", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Update richtextbox with found addresses.
            foreach (long res in AoBScanResultsPlayerTools)
            {
                if (PlayerTools_RichTextBox.Text == "Addresses Loaded: 0")
                {
                    PlayerTools_RichTextBox.Text = "Player Addresses Loaded: " + AoBScanResultsPlayerTools.Count().ToString() + " [" + res.ToString("X").ToString();
                }
                else
                {
                    PlayerTools_RichTextBox.Text += ", " + res.ToString("X").ToString();
                }
            }
            PlayerTools_RichTextBox.Text += "]";

            // Re-enable button.
            // button10.Enabled = true;
            PlayerTools_GroupBox.Enabled = true;

            // Rename button back to default.
            GetAddresses_Button.Text = "Get Addresses";

            // Complete progress bar.
            PlayerTools_ProgressBar.Value = 100;

            // Hide progressbar.
            PlayerTools_ProgressBar.Visible = false;
        }
        #endregion // End get player addresses region.

        #region Free Crafting

        // Toggle free crafting.
        public IEnumerable<long> AoBScanResultsFreeCraftingTools = null;
        private async void FreeCrafting_ToggleSwitch_CheckedChanged(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                FreeCrafting_ToggleSwitch.CheckedChanged -= FreeCrafting_ToggleSwitch_CheckedChanged;
                FreeCrafting_ToggleSwitch.Checked = false;
                FreeCrafting_ToggleSwitch.CheckedChanged += FreeCrafting_ToggleSwitch_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if the slider was not yet checked.
            if (FreeCrafting_ToggleSwitch.Checked)
            {
                // Slider is being toggled on.
                // Name button to indicate loading.
                FreeCrafting_Label.Text = "- Loading..";

                // Offset the progress bar to show it's working.
                PlayerTools_ProgressBar.Visible = true;
                PlayerTools_ProgressBar.Maximum = 100;
                PlayerTools_ProgressBar.Value = 10;

                // Check if we need to rescan the addresses or not.
                // Depreciated Address 04Oct23: 00 00 80 3F D4 04 63 3E D4 04 63 3E 00 00 80 3F E9 DD 25 3E 79 2B 7B 3F C6 AF 56 3E 00 00 80 3F
                if (AoBScanResultsFreeCraftingTools != null)
                {
                    string freeCraftingAddress = BigInteger.Subtract(BigInteger.Parse(AoBScanResultsFreeCraftingTools.Last().ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("4", NumberStyles.HexNumber)).ToString("X");
                    int freeCrafting = MemLib.ReadInt(freeCraftingAddress);

                    // Check if we need to rescan crafting or not.
                    if (freeCrafting != 0 && freeCrafting != 1)
                    {
                        // Rescan food address.
                        AoBScanResultsFreeCraftingTools = await MemLib.AoBScan("00 00 80 3F D4 04 63 3E D4 04 63 3E 00 00 80 3F", true, true);
                    }
                }
                else
                {
                    // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
                    AoBScanResultsFreeCraftingTools = await MemLib.AoBScan("00 00 80 3F D4 04 63 3E D4 04 63 3E 00 00 80 3F", true, true);
                }

                // If the count is zero, the scan had an error.
                if (AoBScanResultsFreeCraftingTools.Count() == 0)
                {
                    // Name label to indicate loading.
                    FreeCrafting_Label.Text = "- Free Crafting";

                    // Reset progress bar.
                    PlayerTools_ProgressBar.Value = 0;
                    PlayerTools_ProgressBar.Visible = false;

                    // Reset aob scan results
                    AoBScanResultsFreeCraftingTools = null;

                    // Toggle slider.
                    FreeCrafting_ToggleSwitch.CheckedChanged -= FreeCrafting_ToggleSwitch_CheckedChanged;
                    FreeCrafting_ToggleSwitch.Checked = false;
                    FreeCrafting_ToggleSwitch.CheckedChanged += FreeCrafting_ToggleSwitch_CheckedChanged;

                    // Display error message.
                    MessageBox.Show("There was an issue trying to fetch the free crafting addresses." + Environment.NewLine + "Try reloading the game!!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Update richtextbox with found addresses.
                PlayerTools_RichTextBox.Text = "Addresses Loaded: 0"; // Reset textbox.
                foreach (long res in AoBScanResultsFreeCraftingTools)
                {
                    if (PlayerTools_RichTextBox.Text == "Addresses Loaded: 0")
                    {
                        PlayerTools_RichTextBox.Text = "Free Crafting Addresses Loaded: " + AoBScanResultsFreeCraftingTools.Count() + " [" + res.ToString("X").ToString();
                    }
                    else
                    {
                        PlayerTools_RichTextBox.Text += ", " + res.ToString("X").ToString();
                    }
                }
                PlayerTools_RichTextBox.Text += "]";

                // Complete progress bar.
                PlayerTools_ProgressBar.Value = 100;
                PlayerTools_ProgressBar.Visible = false;

                // Rename label to default text.
                FreeCrafting_Label.Text = "- Free Crafting";

                // Toggle on free item crafting.
                foreach (long res in AoBScanResultsFreeCraftingTools)
                {
                    // Get the free crafting addresses.
                    // OLD: -10
                    string freeCraftingAddress = BigInteger.Subtract(BigInteger.Parse(AoBScanResultsFreeCraftingTools.Last().ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("4", NumberStyles.HexNumber)).ToString("X");

                    // Write value.
                    MemLib.WriteMemory(freeCraftingAddress, "int", "1"); // Overwrite new value.
                }
            }
            else
            {
                // Slider is being toggled off.
                // Reset label name.
                FreeCrafting_Label.Text = "- Free Crafting";

                // Complete progress bar.
                PlayerTools_ProgressBar.Value = 100;
                PlayerTools_ProgressBar.Visible = false;

                // Toggle off free item crafting.
                foreach (long res in AoBScanResultsFreeCraftingTools)
                {
                    // Get the free crafting addresses.
                    string freeCraftingAddress = BigInteger.Subtract(BigInteger.Parse(AoBScanResultsFreeCraftingTools.Last().ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("4", NumberStyles.HexNumber)).ToString("X");

                    // Write value.
                    MemLib.WriteMemory(freeCraftingAddress, "int", "0"); // Overwrite new value.
                }
            }
        }
        #endregion // End free crafting.

        #region Keep Inventory

        // Toggle keep inventory.
        readonly System.Timers.Timer playersKeepInventoryTimer = new System.Timers.Timer();
        List<Tuple<int, int[]>> keepInventoryInformation = new List<Tuple<int, int[]>>();
        string playerHealthAddress = "0";
        private void KeepInventory_ToggleSwitch_CheckedChanged(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                KeepInventory_ToggleSwitch.CheckedChanged -= KeepInventory_ToggleSwitch_CheckedChanged;
                KeepInventory_ToggleSwitch.Checked = false;
                KeepInventory_ToggleSwitch.CheckedChanged += KeepInventory_ToggleSwitch_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure the inventory was loaded.
            if (AoBScanResultsInventory == null)
            {
                // Toggle slider.
                KeepInventory_ToggleSwitch.CheckedChanged -= KeepInventory_ToggleSwitch_CheckedChanged;
                KeepInventory_ToggleSwitch.Checked = false;
                KeepInventory_ToggleSwitch.CheckedChanged += KeepInventory_ToggleSwitch_CheckedChanged;

                MessageBox.Show("You need to first scan for the Inventory addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsPlayerTools == null)
            {
                // Toggle slider.
                KeepInventory_ToggleSwitch.CheckedChanged -= KeepInventory_ToggleSwitch_CheckedChanged;
                KeepInventory_ToggleSwitch.Checked = false;
                KeepInventory_ToggleSwitch.CheckedChanged += KeepInventory_ToggleSwitch_CheckedChanged;

                MessageBox.Show("You need to first scan for the Player addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if the slider was not yet checked.
            if (KeepInventory_ToggleSwitch.Checked)
            {
                // Slider is being toggled on.
                // Name button to indicate loading.
                KeepInventory_Label.Text = "- Loading..";

                // Offset the progress bar to show it's working.
                PlayerTools_ProgressBar.Visible = true;
                PlayerTools_ProgressBar.Maximum = 100;
                PlayerTools_ProgressBar.Step = 100 / 51;
                PlayerTools_ProgressBar.Value = 0;

                // Get the players health address.
                playerHealthAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.First().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse(godmode_Offset, NumberStyles.Integer)).ToString("X");

                // Advance the progress bar.
                PlayerTools_ProgressBar.PerformStep();

                // Loop through each slot.
                for (int a = 1; a < 50 + 1; a++)
                {
                    // Get information from the item slot.
                    int[] itemInfo = new int[3];
                    itemInfo = GetSlotInfo(a);

                    // Update the inventory info list.
                    keepInventoryInformation.Add(new Tuple<int, int[]>(a, itemInfo));

                    // Advance the progress bar.
                    PlayerTools_ProgressBar.PerformStep();
                }

                // Complete progress bar.
                PlayerTools_ProgressBar.Value = 100;
                PlayerTools_ProgressBar.Visible = false;

                // Rename label to default text.
                KeepInventory_Label.Text = "- Keep Inventory";

                // Start the timed events.
                playersKeepInventoryTimer.Interval = 100; // Custom intervals.
                playersKeepInventoryTimer.Elapsed += new ElapsedEventHandler(PlayersKeepInventoryTimedEvent);
                playersKeepInventoryTimer.Start();
            }
            else
            {
                // Slider is being toggled off.
                // Reset label name.
                KeepInventory_Label.Text = "- Keep Inventory";

                // Complete progress bar.
                PlayerTools_ProgressBar.Value = 100;
                PlayerTools_ProgressBar.Visible = false;

                // Stop the timers.
                playersKeepInventoryTimer.Stop();

                // Clear the list of items.
                keepInventoryInformation = new List<Tuple<int, int[]>>();
            }
        }

        // Keep inventory timer.
        bool playerDied = false;
        private void PlayersKeepInventoryTimedEvent(Object source, ElapsedEventArgs e) // Godforbid, do not use an async timer thread!
        {
            // Check if the player is still alive.
            if (MemLib.ReadInt(playerHealthAddress) > 0 && !playerDied)
            {
                // Iterate through each inventory item.
                foreach (var inventorySlot in keepInventoryInformation)
                {
                    // Check if the player is still alive.
                    if (MemLib.ReadInt(playerHealthAddress) > 0)
                    {
                        // Ensure the item is still the same as saved. 
                        int slotNumber = inventorySlot.Item1;
                        int savedItemType = inventorySlot.Item2[0];
                        int savedItemAmount = inventorySlot.Item2[1];
                        // int savedItemVariation = inventorySlot.Item2[2];

                        // Get the updated slot's item type.
                        int[] currentItemInfo = GetSlotInfo(slotNumber);
                        int currentItemType = currentItemInfo[0];
                        int currentItemAmount = currentItemInfo[1];

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
                                //
                                // Check if the current item amount was increased.
                                if (currentItemAmount > savedItemAmount)
                                {
                                    // Amount was found to be more, update array.
                                    keepInventoryInformation[slotNumber - 1] = new Tuple<int, int[]>(slotNumber, GetSlotInfo(slotNumber));
                                }
                                else if (currentItemAmount < savedItemAmount)
                                {
                                    // Amount was found to be less, update array.
                                    keepInventoryInformation[slotNumber - 1] = new Tuple<int, int[]>(slotNumber, GetSlotInfo(slotNumber));
                                }
                            }
                        }
                        else
                        {
                            // Item was changed, update it.
                            keepInventoryInformation[slotNumber - 1] = new Tuple<int, int[]>(slotNumber, GetSlotInfo(slotNumber));
                        }
                    }
                    else
                    {
                        // Check if the player died awhile updating the inventory, break.
                        playerDied = true;
                        break;
                    }
                }
            }
            else
            {
                // Player has died. Restore inventory.
                //
                // Lets wait for the player to respond.
                while (MemLib.ReadInt(playerHealthAddress) <= 0)
                {
                    // Enable player died.
                    playerDied = true;

                    // Stop the timer.
                    playersKeepInventoryTimer.Stop();

                    // Wait 100 milliseconds.
                    //await Task.Delay(100);

                    // Continue.
                    continue;
                }

                // Iterate through each inventory item.
                foreach (var inventorySlot in keepInventoryInformation)
                {
                    // Ensure the item is still the same as saved. 
                    int slotNumber = inventorySlot.Item1;
                    int savedItemType = inventorySlot.Item2[0];
                    int savedItemAmount = inventorySlot.Item2[1];
                    int savedItemVariation = inventorySlot.Item2[2];

                    // Change the existing items durability to it's original.
                    AddItemToInv(ItemSlot: slotNumber, Type: savedItemType, Amount: savedItemAmount, Variation: savedItemVariation, Overwrite: true);
                }

                // Reset player died flag.
                playerDied = false;

                // Restart the timer.
                playersKeepInventoryTimer.Start();
            }
        }
        #endregion

        #region Infinite Resources

        // Toggle infinite resources.
        readonly System.Timers.Timer playersInfiniteResourcesTimer = new System.Timers.Timer();
        List<Tuple<int, int[]>> inventoryInformation = new List<Tuple<int, int[]>>();
        private void InfiniteResources_ToggleSwitch_CheckedChanged(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                InfiniteResources_ToggleSwitch.CheckedChanged -= InfiniteResources_ToggleSwitch_CheckedChanged;
                InfiniteResources_ToggleSwitch.Checked = false;
                InfiniteResources_ToggleSwitch.CheckedChanged += InfiniteResources_ToggleSwitch_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure the inventory was loaded.
            if (AoBScanResultsInventory == null)
            {
                // Toggle slider.
                InfiniteResources_ToggleSwitch.CheckedChanged -= InfiniteResources_ToggleSwitch_CheckedChanged;
                InfiniteResources_ToggleSwitch.Checked = false;
                InfiniteResources_ToggleSwitch.CheckedChanged += InfiniteResources_ToggleSwitch_CheckedChanged;

                MessageBox.Show("You need to first scan for the Inventory addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if the slider was not yet checked.
            if (InfiniteResources_ToggleSwitch.Checked)
            {
                // Slider is being toggled on.
                // Name button to indicate loading.
                InfiniteResources_Label.Text = "- Loading..";

                // Offset the progress bar to show it's working.
                PlayerTools_ProgressBar.Visible = true;
                PlayerTools_ProgressBar.Maximum = 100;
                PlayerTools_ProgressBar.Step = 100 / 50;

                // Loop through each slot.
                for (int a = 1; a < 50 + 1; a++)
                {
                    // Get information from the item slot.
                    int[] itemInfo = new int[3];
                    itemInfo = GetSlotInfo(a);

                    // Update the inventory info list.
                    inventoryInformation.Add(new Tuple<int, int[]>(a, itemInfo));

                    // Advance the progress bar.
                    PlayerTools_ProgressBar.PerformStep();
                }

                // Complete progress bar.
                PlayerTools_ProgressBar.Value = 100;
                PlayerTools_ProgressBar.Visible = false;

                // Rename label to default text.
                InfiniteResources_Label.Text = "- Infinite Resources";

                // Start the timed events.
                playersInfiniteResourcesTimer.Interval = 100; // Custom intervals.
                playersInfiniteResourcesTimer.Elapsed += new ElapsedEventHandler(PlayersInfiniteResourcesTimedEvent);
                playersInfiniteResourcesTimer.Start();
            }
            else
            {
                // Slider is being toggled off.
                // Reset label name.
                InfiniteResources_Label.Text = "- Infinite Resources";

                // Complete progress bar.
                PlayerTools_ProgressBar.Value = 100;
                PlayerTools_ProgressBar.Visible = false;

                // Stop the timers.
                playersInfiniteResourcesTimer.Stop();

                // Clear the list of items.
                inventoryInformation = new List<Tuple<int, int[]>>();
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
                int[] currentItemInfo = GetSlotInfo(slotNumber);
                int currentItemType = currentItemInfo[0];
                int currentItemAmount = currentItemInfo[1];

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
                        //
                        // Check if the current item amount was increased.
                        if (currentItemAmount > savedItemAmount)
                        {
                            // Amount was found to be more, update array.
                            inventoryInformation[slotNumber - 1] = new Tuple<int, int[]>(slotNumber, GetSlotInfo(slotNumber));
                        }
                        else if (currentItemAmount < savedItemAmount)
                        {
                            // Change the existing items durability to it's original.
                            AddItemToInv(ItemSlot: slotNumber, Type: savedItemType, Amount: savedItemAmount, Variation: savedItemVariation, Overwrite: true);
                        }
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

        #region Buff Editor

        // Change the players buff.
        private async void ApplyBuff_Button_Click(object sender, EventArgs e)
        {
            // Check if the combobox has a value and is not null.
            if (BuffType_ComboBox.Text == "")
            {
                MessageBox.Show("The buff type cannot be null!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Change button to indicate loading.
            ApplyBuff_Button.Text = "Working";
            ApplyBuff_Button.Enabled = false;

            // Find the memory addresses.
            // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
            // Depreciated Address 15Feb25: 01 00 00 00 00 00 70 42 04 00 00 00 00 00 00 00 ?? ?? ?? ?? 00 00 00 00
            AoBScanResultsPlayerBuffs = await MemLib.AoBScan("01 00 00 00 00 00 70 42 04 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00", true, true);

            // If the count is zero, the scan had an error.
            if (AoBScanResultsPlayerBuffs.Count() < 1)
            {
                // Rename button back to default.
                ApplyBuff_Button.Text = "Apply";

                // Re-enable button.;
                ApplyBuff_Button.Enabled = true;

                // Reset aob scan results
                AoBScanResultsPlayerBuffs = null;

                // Display error message.
                MessageBox.Show("You must first consume a glow tulip!!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get json file from resources.
            string buffIDValue = "1";
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
                    if (buffName == BuffType_ComboBox.Text)
                    {
                        // Update the buffidvalue.
                        buffIDValue = int.Parse((string)file.id).ToString();

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
                string buffTime = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("4", NumberStyles.Integer)).ToString("X");
                string buffPower = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");

                MemLib.WriteMemory(buffID, "int", buffIDValue);                              // Write buff id.
                MemLib.WriteMemory(buffTime, "float", TimeS_NumericUpDown.Value.ToString()); // Write buff time.
                MemLib.WriteMemory(buffPower, "int", Power_NumericUpDown.Value.ToString());  // Write buff power.
            }

            // Process completed, run finishing tasks.
            // Rename button back to default.
            ApplyBuff_Button.Text = "Apply";
            ApplyBuff_Button.Enabled = true;
        }
        #endregion // End buff editor.

        #region Place Anywhere

        // PlacementHandler.allowPlacingAnywhere;
        // Place anywhere.
        public IEnumerable<long> AoBScanResultsPlaceAnywhereTools = null;
        private async void PlaceAnywhere_ToggleSwitch_CheckedChanged(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                PlaceAnywhere_ToggleSwitch.CheckedChanged -= PlaceAnywhere_ToggleSwitch_CheckedChanged;
                PlaceAnywhere_ToggleSwitch.Checked = false;
                PlaceAnywhere_ToggleSwitch.CheckedChanged += PlaceAnywhere_ToggleSwitch_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if the slider was not yet checked.
            if (PlaceAnywhere_ToggleSwitch.Checked)
            {
                // Slider is being toggled on.
                // Name button to indicate loading.
                PlaceAnywhere_Label.Text = "- Loading..";

                // Offset the progress bar to show it's working.
                PlayerTools_ProgressBar.Visible = true;
                PlayerTools_ProgressBar.Maximum = 100;
                PlayerTools_ProgressBar.Value = 10;

                // Check if we need to rescan the addresses or not.
                // Depreciated Address 21Feb24: 04 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 07 00 00 00 21 00 00 00 00 00 04 40 00 00 00 00 00 00 00 00 00 00 00 00
                // Depreciated Address 21Feb24: 00 01 02 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 01 00 00 00 00 00 00 28 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 3B 4D 00 00 00 01 02 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                // Depreciated Address 26Apr24: 01 00 00 00 00 00 00 28 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 3B 4D 00 00 00 01 02 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                // Depreciated Address 23Aug24: 01 00 00 00 00 00 00 28 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 01 02 00 04 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                // Depreciated Address 05Feb25: 32 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 02 01 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                // Depreciated Address 22Mar25: 0? 8D 84 F0 35 AC F0 76 00 00 00 00 00 00 00 00
                // Depreciated Address 13Apr25: 3F 2A 40 ED A5 87 85 B3 0? 3A F5 ?0
                // Depreciated Address 29May25: 04 00 00 00 10 00 00 00 0? BE F8 ??
                if (AoBScanResultsPlaceAnywhereTools != null)
                {
                    // string placeAnywhereAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlaceAnywhereTools.Last().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");
                    string placeAnywhereAddress = AoBScanResultsPlaceAnywhereTools.Last().ToString("X");
                    int placeAnywhere = MemLib.ReadByte(placeAnywhereAddress);

                    // Check if we need to rescan place anywhere or not.
                    if (placeAnywhere != 0 && placeAnywhere != 1)
                    {
                        // Rescan place anywhere address.
                        AoBScanResultsPlaceAnywhereTools = await MemLib.AoBScan("0? 9E F5 ?? ?? ?? 00 00 08 00 00 00 10 00 00 00 9? 0C 00 00 00 00 00 00", true, true);
                    }
                }
                else
                {
                    // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
                    AoBScanResultsPlaceAnywhereTools = await MemLib.AoBScan("0? 9E F5 ?? ?? ?? 00 00 08 00 00 00 10 00 00 00 9? 0C 00 00 00 00 00 00", true, true);
                }

                // If the count is zero, the scan had an error.
                if (AoBScanResultsPlaceAnywhereTools.Count() == 0)
                {
                    // Name label to indicate loading.
                    PlaceAnywhere_Label.Text = "- Place Anywhere";

                    // Reset progress bar.
                    PlayerTools_ProgressBar.Value = 0;
                    PlayerTools_ProgressBar.Visible = false;

                    // Reset aob scan results
                    AoBScanResultsPlaceAnywhereTools = null;

                    // Toggle slider.
                    PlaceAnywhere_ToggleSwitch.CheckedChanged -= PlaceAnywhere_ToggleSwitch_CheckedChanged;
                    PlaceAnywhere_ToggleSwitch.Checked = false;
                    PlaceAnywhere_ToggleSwitch.CheckedChanged += PlaceAnywhere_ToggleSwitch_CheckedChanged;

                    // Display error message.
                    MessageBox.Show("There was an issue trying to fetch the place anywhere addresses." + Environment.NewLine + "Try reloading the game!!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Update richtextbox with found addresses.
                PlayerTools_RichTextBox.Text = "Addresses Loaded: 0"; // Reset textbox.
                foreach (long res in AoBScanResultsPlaceAnywhereTools)
                {
                    if (PlayerTools_RichTextBox.Text == "Addresses Loaded: 0")
                    {
                        PlayerTools_RichTextBox.Text = "Place Anywhere Addresses Loaded: " + AoBScanResultsPlaceAnywhereTools.Count() + " [" + res.ToString("X").ToString();
                    }
                    else
                    {
                        PlayerTools_RichTextBox.Text += ", " + res.ToString("X").ToString();
                    }
                }
                PlayerTools_RichTextBox.Text += "]";

                // Complete progress bar.
                PlayerTools_ProgressBar.Value = 100;
                PlayerTools_ProgressBar.Visible = false;

                // Rename label to default text.
                PlaceAnywhere_Label.Text = "- Place Anywhere";

                // Toggle on place anywhere.
                foreach (long res in AoBScanResultsPlaceAnywhereTools)
                {
                    // Get the place anywhere addresses.
                    // string placeAnywhereAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlaceAnywhereTools.Last().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");
                    string placeAnywhereAddress = AoBScanResultsPlaceAnywhereTools.Last().ToString("X");

                    // Write value.
                    MemLib.WriteMemory(placeAnywhereAddress, "byte", "1"); // Overwrite new value.
                }
            }
            else
            {
                // Slider is being toggled off.
                // Reset label name.
                PlaceAnywhere_Label.Text = "- Place Anywhere";

                // Complete progress bar.
                PlayerTools_ProgressBar.Value = 100;
                PlayerTools_ProgressBar.Visible = false;

                // Toggle off place anywhere.
                foreach (long res in AoBScanResultsPlaceAnywhereTools)
                {
                    // Get the place anywhere addresses.
                    // string placeAnywhereAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlaceAnywhereTools.Last().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");
                    string placeAnywhereAddress = AoBScanResultsPlaceAnywhereTools.Last().ToString("X");

                    // Write value.
                    MemLib.WriteMemory(placeAnywhereAddress, "byte", "0"); // Overwrite new value.
                }
            }
        }
        #endregion // End place anywhere.

        #region Placement Range

        // PlayerController player = Manager.main.player;
        // Placement Range.
        public IEnumerable<long> AoBScanResultsPlacementRangeTools = null;
        private async void Range_ToggleSwitch_CheckedChanged(object sender, EventArgs e)
        {
            // Mod is currently broken - toggle slider off.
            Range_ToggleSwitch.CheckedChanged -= Range_ToggleSwitch_CheckedChanged;
            Range_ToggleSwitch.Checked = false;
            Range_ToggleSwitch.CheckedChanged += Range_ToggleSwitch_CheckedChanged;
            return;

            #pragma warning disable // Suppress Unreachable code.
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                Range_ToggleSwitch.CheckedChanged -= Range_ToggleSwitch_CheckedChanged;
                Range_ToggleSwitch.Checked = false;
                Range_ToggleSwitch.CheckedChanged += Range_ToggleSwitch_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if the slider was not yet checked.
            if (Range_ToggleSwitch.Checked)
            {
                // Slider is being toggled on.
                // Name button to indicate loading.
                Range_Label.Text = "- Loading..";

                // Offset the progress bar to show it's working.
                PlayerTools_ProgressBar.Visible = true;
                PlayerTools_ProgressBar.Maximum = 100;
                PlayerTools_ProgressBar.Value = 10;

                // Disable numericupdown.
                Range_NumericUpDown.Enabled = false;

                // Check if we need to rescan the addresses or not.
                if (AoBScanResultsPlacementRangeTools != null)
                {
                    string placementRangeAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlacementRangeTools.Last().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("16", NumberStyles.Integer)).ToString("X");
                    float placementRange = MemLib.ReadFloat(placementRangeAddress);

                    // Check if we need to rescan placement range or not.
                    // default 1.5.
                    if (placementRange > (float)Range_NumericUpDown.Maximum && placementRange < (float)CustomAmount_NumericUpDown.Minimum)
                    {
                        // Rescan placement range addresses.
                        // Depreciated address 30Nov23: 00 00 80 3F ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 01 04 02 05 04 03 01 50 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 49 BB ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 55 48 8B EC 48 83 EC 50 48 89 75 E8 48 89 7D F0
                        AoBScanResultsPlacementRangeTools = await MemLib.AoBScan("01 04 02 05 04 03 01 50 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 49 BB ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 55 48 8B EC 48 83 EC 50 48 89 75 E8 48 89 7D F0 4C 89 7D F8 48 8B F1 48", true, true);
                    }
                }
                else
                {
                    // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
                    AoBScanResultsPlacementRangeTools = await MemLib.AoBScan("01 04 02 05 04 03 01 50 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 49 BB ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 55 48 8B EC 48 83 EC 50 48 89 75 E8 48 89 7D F0 4C 89 7D F8 48 8B F1 48", true, true);
                }

                // If the count is zero, the scan had an error.
                if (AoBScanResultsPlacementRangeTools.Count() == 0)
                {
                    // Name label to indicate loading.
                    Range_Label.Text = "- Range:";

                    // Reset progress bar.
                    PlayerTools_ProgressBar.Value = 0;
                    PlayerTools_ProgressBar.Visible = false;

                    // Enable numericupdown.
                    Range_NumericUpDown.Enabled = true;

                    // Reset aob scan results
                    AoBScanResultsPlacementRangeTools = null;

                    // Toggle slider.
                    Range_ToggleSwitch.CheckedChanged -= Range_ToggleSwitch_CheckedChanged;
                    Range_ToggleSwitch.Checked = false;
                    Range_ToggleSwitch.CheckedChanged += Range_ToggleSwitch_CheckedChanged;

                    // Display error message.
                    MessageBox.Show("There was an issue trying to fetch the placement range addresses." + Environment.NewLine + "Try reloading the game!!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Update richtextbox with found addresses.
                PlayerTools_RichTextBox.Text = "Addresses Loaded: 0"; // Reset textbox.
                foreach (long res in AoBScanResultsPlacementRangeTools)
                {
                    if (PlayerTools_RichTextBox.Text == "Addresses Loaded: 0")
                    {
                        PlayerTools_RichTextBox.Text = "Placement Range Addresses Loaded: " + AoBScanResultsPlacementRangeTools.Count() + " [" + res.ToString("X").ToString();
                    }
                    else
                    {
                        PlayerTools_RichTextBox.Text += ", " + BigInteger.Subtract(BigInteger.Parse(res.ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X"); // +16 Old.
                    }
                }
                PlayerTools_RichTextBox.Text += "]";

                // Complete progress bar.
                PlayerTools_ProgressBar.Value = 100;
                PlayerTools_ProgressBar.Visible = false;

                // Rename label to default text.
                Range_Label.Text = "- Range:";

                // Toggle on placement range.
                foreach (long res in AoBScanResultsPlacementRangeTools)
                {
                    // Get the placement range addresses.
                    // OLD: 124
                    string passiveAIAddress = BigInteger.Subtract(BigInteger.Parse(res.ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");

                    // Write value.
                    MemLib.WriteMemory(passiveAIAddress, "float", Range_NumericUpDown.Value.ToString()); // Overwrite new value.
                }
            }
            else
            {
                // Slider is being toggled off.
                // Reset label name.
                Range_Label.Text = "- Range:";

                // Complete progress bar.
                PlayerTools_ProgressBar.Value = 100;
                PlayerTools_ProgressBar.Visible = false;

                // Enable numericupdown.
                Range_NumericUpDown.Enabled = true;

                // Toggle off placement range.
                foreach (long res in AoBScanResultsPlacementRangeTools)
                {
                    // Get the placement range addresses.
                    string passiveAIAddress = BigInteger.Subtract(BigInteger.Parse(res.ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");

                    // Write value.
                    MemLib.WriteMemory(passiveAIAddress, "float", "1.5"); // Overwrite new value.
                }
            }
            #pragma warning restore // Suppress Unreachable code.
        }
        #endregion // End place anywhere.

        #region Skill Editor

        // Launch skill editor.
        private void OpenSkillEditor_Button_Click(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                DisplayLocation_ToggleSwitch.CheckedChanged -= DisplayLocation_ToggleSwitch_CheckedChanged;
                DisplayLocation_ToggleSwitch.Checked = false;
                DisplayLocation_ToggleSwitch.CheckedChanged += DisplayLocation_ToggleSwitch_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            // if (AoBScanResultsPlayerTools == null)
            // {
            //     MessageBox.Show("You need to first scan for the Player addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //     return;
            // }

            // Save some form settings.
            // Settings.Default.SkillEditorAddress = AoBScanResultsPlayerTools.First().ToString("X");

            // Spawn item picker window.
            try
            {
                SkillEditor skillEditor = new SkillEditor();
                DialogResult dr = skillEditor.ShowDialog();

                // Get returned item from chunk viewer.
                skillEditor.Close();
            }
            catch
            { }
        }
        #endregion

        #region Trash Inventory

        // Toggle force keep the inventory empty.
        readonly System.Timers.Timer playersKeepInventoryEmptyTimer = new System.Timers.Timer();
        private void TrashInventory_ToggleSwitch_CheckedChanged(object sender, EventArgs e)
        {
            if (TrashInventory_ToggleSwitch.Checked && MessageBox.Show("This mod will delete ALL inventory items and ALL picked up ground items. Are you sure?", "Trash Inventory", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                // Toggle slider.
                TrashInventory_ToggleSwitch.CheckedChanged -= TrashInventory_ToggleSwitch_CheckedChanged;
                TrashInventory_ToggleSwitch.Checked = false;
                TrashInventory_ToggleSwitch.CheckedChanged += TrashInventory_ToggleSwitch_CheckedChanged;

                // User declined, return.
                return;
            }

            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                TrashInventory_ToggleSwitch.CheckedChanged -= TrashInventory_ToggleSwitch_CheckedChanged;
                TrashInventory_ToggleSwitch.Checked = false;
                TrashInventory_ToggleSwitch.CheckedChanged += TrashInventory_ToggleSwitch_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure the inventory was loaded.
            if (AoBScanResultsInventory == null)
            {
                // Toggle slider.
                TrashInventory_ToggleSwitch.CheckedChanged -= TrashInventory_ToggleSwitch_CheckedChanged;
                TrashInventory_ToggleSwitch.Checked = false;
                TrashInventory_ToggleSwitch.CheckedChanged += TrashInventory_ToggleSwitch_CheckedChanged;

                MessageBox.Show("You need to first scan for the Inventory addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if the slider was not yet checked.
            if (TrashInventory_ToggleSwitch.Checked)
            {
                // Slider is being toggled on.
                // Name button to indicate loading.
                TrashInventory_Label.Text = "- Loading..";

                // Offset the progress bar to show it's working.
                PlayerTools_ProgressBar.Visible = true;
                PlayerTools_ProgressBar.Maximum = 100;
                PlayerTools_ProgressBar.Value = 0;

                // Complete progress bar.
                PlayerTools_ProgressBar.Value = 100;
                PlayerTools_ProgressBar.Visible = false;

                // Rename label to default text.
                TrashInventory_Label.Text = "- Trash Inventory";

                // Append mod header to world tools.
                WorldTools_RichTextBox.AppendText("Keep Inventory Empty Mod Initiated:" + Environment.NewLine);

                // Start the timed events.
                playersKeepInventoryEmptyTimer.Interval = 100; // Custom intervals.
                playersKeepInventoryEmptyTimer.Elapsed += new ElapsedEventHandler(PlayersKeepInventoryEmptyTimedEvent);
                playersKeepInventoryEmptyTimer.Start();
            }
            else
            {
                // Slider is being toggled off.
                // Reset label name.
                TrashInventory_Label.Text = "- Trash Inventory";

                // Complete progress bar.
                PlayerTools_ProgressBar.Value = 100;
                PlayerTools_ProgressBar.Visible = false;

                // Stop the timers.
                playersKeepInventoryEmptyTimer.Stop();

                // Append mod header to world tools.
                WorldTools_RichTextBox.AppendText("Keep Inventory Empty Mod Terminated." + Environment.NewLine);
            }
        }

        // Keep inventory empty timer.
        private void PlayersKeepInventoryEmptyTimedEvent(Object source, ElapsedEventArgs e)
        {
            // Loop through each slot.
            for (int a = 1; a < 50 + 1; a++)
            {
                // Ensure the item is still the same as saved. 
                int slotNumber = a;

                // Get the updated slot's item type.
                int[] currentItemInfo = GetSlotInfo(slotNumber);
                int currentItemType = currentItemInfo[0];
                int currentItemAmount = currentItemInfo[1];
                int currentItemVariation = currentItemInfo[2];
                int currentItemSkillset = currentItemInfo[3];

                // Check if item is not empty.
                if (currentItemType != 0 || currentItemAmount != 0 || currentItemVariation != 0 || currentItemSkillset != 0)
                {
                    // Add item to debug info.
                    WorldTools_RichTextBox.AppendText("ItemSlot: " + slotNumber + " | ItemID: " + currentItemType + " | Amount: " + currentItemAmount + " | Variation: " + currentItemVariation + " | Skillset: " + currentItemSkillset + Environment.NewLine);
                }
            }

            // Reload Inventory.
            AddItemToInv(Type: 0, Amount: 1, Variation: 0, CycleAll: true);
        }
        #endregion

        #region Minecart Max Speed

        // Toggle minecart max speed.
        readonly System.Timers.Timer minecartMaxSpeedTimer = new System.Timers.Timer();
        public IEnumerable<long> AoBScanResultsMaxMinecartSpeed = null;
        private async void MaxMinecartSpeed_ToggleSwitch_CheckedChanged(object sender, EventArgs e)
        {
            // Mod is currently broken - toggle slider off.
            MaxMinecartSpeed_ToggleSwitch.CheckedChanged -= MaxMinecartSpeed_ToggleSwitch_CheckedChanged;
            MaxMinecartSpeed_ToggleSwitch.Checked = false;
            MaxMinecartSpeed_ToggleSwitch.CheckedChanged += MaxMinecartSpeed_ToggleSwitch_CheckedChanged;
            return;

            #pragma warning disable // Suppress Unreachable code.
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                MaxMinecartSpeed_ToggleSwitch.CheckedChanged -= MaxMinecartSpeed_ToggleSwitch_CheckedChanged;
                MaxMinecartSpeed_ToggleSwitch.Checked = false;
                MaxMinecartSpeed_ToggleSwitch.CheckedChanged += MaxMinecartSpeed_ToggleSwitch_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if the slider was not yet checked.
            if (MaxMinecartSpeed_ToggleSwitch.Checked)
            {
                // Slider is being toggled on.
                // Name button to indicate loading.
                MinecartSpeed_Label.Text = "- Loading..";

                // Offset the progress bar to show it's working.
                PlayerTools_ProgressBar.Visible = true;
                PlayerTools_ProgressBar.Maximum = 100;
                PlayerTools_ProgressBar.Value = 10;

                // Check if we need to rescan the addresses or not.
                if (AoBScanResultsMaxMinecartSpeed != null)
                {
                    string maxMinecartSpeedAddress = BigInteger.Subtract(BigInteger.Parse(AoBScanResultsMaxMinecartSpeed.Last().ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("4", NumberStyles.Integer)).ToString("X");
                    float maxSpeed = MemLib.ReadFloat(maxMinecartSpeedAddress);

                    // Check if we need to rescan food or not.
                    if (maxSpeed < 0 || maxSpeed > 9999)
                    {
                        // Rescan food address.
                        AoBScanResultsMaxMinecartSpeed = await MemLib.AoBScan("CD CC 4C 3E ?? ?? ?? ?? ?? ?? ?? ?? 01 00 00 00 ?? ?? ?? ?? 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00", true, true);
                    }
                }
                else
                {
                    // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
                    // Depreciated Address 28Jun24: ?? FF FF FF ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? CD CC 4C 3E ?? ?? ?? ?? ?? ?? ?? ?? 01 00 00 00 ?? ?? ?? ?? 01 00 00 00 00 00 00 00
                    AoBScanResultsMaxMinecartSpeed = await MemLib.AoBScan("CD CC 4C 3E ?? ?? ?? ?? ?? ?? ?? ?? 01 00 00 00 ?? ?? ?? ?? 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00", true, true);
                }

                // If the count is zero, the scan had an error.
                if (AoBScanResultsMaxMinecartSpeed.Count() == 0)
                {
                    // Name label to indicate loading.
                    MinecartSpeed_Label.Text = "- Minecart Speed";

                    // Reset progress bar.
                    PlayerTools_ProgressBar.Value = 0;
                    PlayerTools_ProgressBar.Visible = false;

                    // Reset aob scan results
                    AoBScanResultsMaxMinecartSpeed = null;

                    // Toggle slider.
                    MaxMinecartSpeed_ToggleSwitch.CheckedChanged -= MaxMinecartSpeed_ToggleSwitch_CheckedChanged;
                    MaxMinecartSpeed_ToggleSwitch.Checked = false;
                    MaxMinecartSpeed_ToggleSwitch.CheckedChanged += MaxMinecartSpeed_ToggleSwitch_CheckedChanged;

                    // Display error message.
                    MessageBox.Show("There was an issue trying to fetch the minecart addresses." + Environment.NewLine + "Try reloading the game!!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Update richtextbox with found addresses.
                PlayerTools_RichTextBox.Text = "Addresses Loaded: 0"; // Reset textbox.
                foreach (long res in AoBScanResultsMaxMinecartSpeed)
                {
                    if (PlayerTools_RichTextBox.Text == "Addresses Loaded: 0")
                    {
                        PlayerTools_RichTextBox.Text = "Minecart Addresses Loaded: " + AoBScanResultsMaxMinecartSpeed.Count() + " [" + res.ToString("X").ToString();
                    }
                    else
                    {
                        PlayerTools_RichTextBox.Text += ", " + res.ToString("X").ToString();
                    }
                }
                PlayerTools_RichTextBox.Text += "]";

                // Complete progress bar.
                PlayerTools_ProgressBar.Value = 100;
                PlayerTools_ProgressBar.Visible = false;

                // Rename label to default text.
                MinecartSpeed_Label.Text = "- Minecart Speed";

                // Enable the slider.
                MaxMinecartSpeed_TrackBar.Enabled = true;

                // Start the timed events.
                minecartMaxSpeedTimer.Interval = 1; // Custom intervals.
                minecartMaxSpeedTimer.Elapsed += new ElapsedEventHandler(MinecartMaxSpeedTimedEvent);
                minecartMaxSpeedTimer.Start();
            }
            else
            {
                // Slider is being toggled off.
                // Stop the timers.
                minecartMaxSpeedTimer.Stop();

                // Reset the sliders value.
                // label45.Text = "- MaxSpeed: 800";
                // siticoneMetroTrackBar1.Value = 800;

                // Enable the slider.
                MaxMinecartSpeed_TrackBar.Enabled = false;
            }
            #pragma warning restore // Suppress Unreachable code.
        }

        // Minecart max speed timer.
        private void MinecartMaxSpeedTimedEvent(Object source, ElapsedEventArgs e)
        {
            // Write value.
            string maxMinecartSpeedAddress = BigInteger.Subtract(BigInteger.Parse(AoBScanResultsMaxMinecartSpeed.Last().ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("4", NumberStyles.Integer)).ToString("X");
            MemLib.WriteMemory(maxMinecartSpeedAddress, "float", MaxMinecartSpeed_TrackBar.Value.ToString()); // Overwrite new value.
        }

        // Show the slider value text.
        private void MaxMinecartSpeed_MetroTrackBar_MouseHover(object sender, EventArgs e)
        {
            MinecartSpeed_Label.Visible = false;
            MaxMinecartSpeed_Label.Visible = true;
        }

        // Hide the slider value text.
        private void MaxMinecartSpeed_MetroTrackBar_MouseLeave(object sender, EventArgs e)
        {
            MinecartSpeed_Label.Visible = true;
            MaxMinecartSpeed_Label.Visible = false;
        }

        // Change the labels text to the new sliders value.
        private void MaxMinecartSpeed_MetroTrackBar_ValueChanged(object sender, EventArgs e)
        {
            MaxMinecartSpeed_Label.Text = "- MaxSpeed: " + MaxMinecartSpeed_TrackBar.Value.ToString();
        }
        #endregion

        #region Freeze Slot Stats

        private PictureBox[] _slotPictures;             // Holds references to each SlotN_PictureBox on the Inventory tab.
        private Dictionary<int, CheckBox> _freezeBoxes; // Maps a slot index to its dynamically created “freeze” CheckBox.
        private Dictionary<int, int[]> _frozenSlotInfo; // Stores the original [type, amount, variation, skillset] info for each slot the user has frozen.
        readonly System.Timers.Timer playersFreezeItemSlotsTimer = new System.Timers.Timer();
        private void FreezeItemSlots_ToggleSwitch_CheckedChanged(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                FreezeItemSlots_ToggleSwitch.CheckedChanged -= FreezeItemSlots_ToggleSwitch_CheckedChanged;
                FreezeItemSlots_ToggleSwitch.Checked = false;
                FreezeItemSlots_ToggleSwitch.CheckedChanged += FreezeItemSlots_ToggleSwitch_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure the inventory was loaded.
            if (AoBScanResultsInventory == null)
            {
                // Toggle slider.
                FreezeItemSlots_ToggleSwitch.CheckedChanged -= FreezeItemSlots_ToggleSwitch_CheckedChanged;
                FreezeItemSlots_ToggleSwitch.Checked = false;
                FreezeItemSlots_ToggleSwitch.CheckedChanged += FreezeItemSlots_ToggleSwitch_CheckedChanged;

                MessageBox.Show("You need to first scan for the Inventory addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (FreezeItemSlots_ToggleSwitch.Checked)
            {
                // Show the checkboxes.
                BuildSlotArrays();
                CreateFreezeCheckboxes();

                // Snapshot every checked slot.
                _frozenSlotInfo = new Dictionary<int, int[]>();
                foreach (var kv in _freezeBoxes)
                    if (kv.Value.Checked)
                        _frozenSlotInfo[kv.Key] = GetSlotInfo(kv.Key);

                // Start the timed events.
                playersFreezeItemSlotsTimer.Interval = 100; // Custom intervals.
                playersFreezeItemSlotsTimer.Elapsed += new ElapsedEventHandler(PlayersFreezeItemSlotsTimedEvent);
                playersFreezeItemSlotsTimer.Start();
            }
            else
            {
                // Stop freezing.
                playersFreezeItemSlotsTimer.Stop();
                _frozenSlotInfo.Clear();

                // Remove the checkboxes. We dont just want them to sit hidden or linger invisibly.
                foreach (var cb in _freezeBoxes.Values)
                {
                    cb.Parent?.Controls.Remove(cb);
                    cb.Dispose();
                }
                _freezeBoxes.Clear();
            }
        }

        private void PlayersFreezeItemSlotsTimedEvent(object sender, ElapsedEventArgs e)
        {
            // Run on the UI thread for calling UI methods.
            this.Invoke((MethodInvoker)(() =>
            {
                foreach (var kv in _frozenSlotInfo.ToList())
                {
                    int slot = kv.Key;
                    int[] original = kv.Value;
                    int[] current = GetSlotInfo(slot);

                    // If item type changed, re-snapshot.
                    if (current[0] != original[0])
                    {
                        _frozenSlotInfo[slot] = current;
                        continue;
                    }

                    // If amount decreased, write it back.
                    if (current[1] != original[1])
                    {
                        AddItemToInv(
                            ItemSlot:  slot,
                            Type:      original[0],
                            Amount:    original[1],
                            Variation: original[2],
                            Skillset:  original[3],
                            Overwrite: true
                        );
                    }

                    #region Disabled: Allow Value Increases

                    /*
                    // If amount decreased, write it back.
                    if (current[1] < original[1])
                    {
                        AddItemToInv(
                            ItemSlot:  slot,
                            Type:      original[0],
                            Amount:    original[1],
                            Variation: original[2],
                            Skillset:  original[3],
                            Overwrite: true
                        );
                    }
                    else if (current[1] > original[1])
                    {
                        // If amount increased, update baseline.
                        _frozenSlotInfo[slot] = current;
                    }
                    */
                    #endregion
                }
            }));
        }

        #region Checkbox Helpers

        private void BuildSlotArrays()
        {
            // Count the total amount of pictureboxes on the inventory tab.
            int slotCount = Main_TabControl.TabPages["Inventory_TabPage"].Controls.OfType<PictureBox>().Count();

            var tab = Main_TabControl.TabPages["Inventory_TabPage"];
            _slotPictures = Enumerable.Range(1, slotCount)
                .Select(i => tab.Controls.Find($"Slot{i}_PictureBox", true)
                .FirstOrDefault() as PictureBox)
                .ToArray();
        }

        private void CreateFreezeCheckboxes()
        {
            var tab = Main_TabControl.TabPages["Inventory_TabPage"];
            _freezeBoxes = new Dictionary<int, CheckBox>();

            for (int i = 1; i <= _slotPictures.Length; i++)
            {
                var pic = _slotPictures[i - 1];
                if (pic == null) continue;

                var cb = new CheckBox
                {
                    Name = $"FreezeSlot{i}_CheckBox",
                    Tag = i,                                      // Store the slot.
                    Size = new Size(16, 16),
                    Location = new Point(pic.Width - 18, 2),      // Relative to pic.ClientRectangle.
                    BackColor = Color.FromArgb(150, Color.Black),
                    ForeColor = Color.White,
                    Parent = pic,                                 // Make it a child.
                    AutoSize = false
                };
                cb.CheckedChanged += FreezeSlotCheckBox_CheckedChanged;

                pic.Controls.Add(cb);                             // Add to pic rather than tab.
                _freezeBoxes[i] = cb;
            }
        }

        private void FreezeSlotCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // If user checks it while the feature is on, immediately capture its current info.
            if (FreezeItemSlots_ToggleSwitch.Checked)
            {
                var cb = (CheckBox)sender;
                int slot = (int)cb.Tag; // Get it back safely.
                if (cb.Checked)
                    _frozenSlotInfo[slot] = GetSlotInfo(slot);
                else
                    _frozenSlotInfo.Remove(slot);
            }
        }
        #endregion

        #endregion // End freeze item slots.

        // Mods below use the "Player Mod Offsets".

        #region Player Position

        // Enable player xy tool.
        readonly System.Timers.Timer playersPositionTimer = new System.Timers.Timer();
        private void DisplayLocation_ToggleSwitch_CheckedChanged(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                DisplayLocation_ToggleSwitch.CheckedChanged -= DisplayLocation_ToggleSwitch_CheckedChanged;
                DisplayLocation_ToggleSwitch.Checked = false;
                DisplayLocation_ToggleSwitch.CheckedChanged += DisplayLocation_ToggleSwitch_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsPlayerTools == null)
            {
                // Toggle slider.
                DisplayLocation_ToggleSwitch.CheckedChanged -= DisplayLocation_ToggleSwitch_CheckedChanged;
                DisplayLocation_ToggleSwitch.Checked = false;
                DisplayLocation_ToggleSwitch.CheckedChanged += DisplayLocation_ToggleSwitch_CheckedChanged;

                MessageBox.Show("You need to first scan for the Player addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if the slider was not yet checked.
            if (DisplayLocation_ToggleSwitch.Checked)
            {
                // Start the timed events.
                playersPositionTimer.Interval = 100; // Custom intervals.
                playersPositionTimer.Elapsed += new ElapsedEventHandler(PlayersPositionTimedEvent);
                playersPositionTimer.Start();

                // Update consoile with the status.
                WorldTools_RichTextBox.AppendText("[PlayerPosition] Player position has been enabled." + Environment.NewLine);
                WorldTools_RichTextBox.ScrollToCaret();
            }
            else
            {
                // Disable player position.
                // Stop the timers.
                playersPositionTimer.Stop();

                // Change appllication text back to default.
                this.Text = "CoreKeeper's Workshop";

                // Update consoile with the status.
                WorldTools_RichTextBox.AppendText("[PlayerPosition] Player position has been disabled." + Environment.NewLine);
                WorldTools_RichTextBox.ScrollToCaret();
            }
        }

        // Players position timer.
        private void PlayersPositionTimedEvent(Object source, ElapsedEventArgs e)
        {
            // Get the addresses.
            // Old 04Oct23: BigInteger.Subtract(BigInteger.Parse(AoBScanResultsPlayerTools.First().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");
            string positionX = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.First().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse(positionX_Offset, NumberStyles.Integer)).ToString("X");
            string positionY = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.First().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse(positionY_Offset, NumberStyles.Integer)).ToString("X");

            // Convert values to number.
            string playerPositionX = MemLib.ReadFloat(positionX).ToString();
            string playerPositionY = MemLib.ReadFloat(positionY).ToString(); // OLD: -1 Correct the offset. 

            // Change the applications tittle based on minimization and tab pages. 
            if (isMinimized || Main_TabControl.SelectedTab == Chat_TabPage) // Tab five is smaller.
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
        #endregion // End player position.

        #region Player Position Chunk

        #region Chunk Math Functions

        // Get the nearest XY position of a chunk based on position.
        public Vector2 GetChunk(Vector2 Position)
        {
            return new Vector2(IRoundTo((int)((Vector2)Position).X, 64), IRoundTo((int)((Vector2)Position).Y, 64));
        }

        // Algorithm for comparing two integers.
        public static int IRoundTo(int inval, int nearest)
        {
            inval /= nearest;
            return (int)((float)Math.Round((double)inval) * (float)nearest);
        }
        #endregion // End player math.

        // Get chunk information.
        private void OpenChunkVisualizer_Button_Click(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                DisplayLocation_ToggleSwitch.CheckedChanged -= DisplayLocation_ToggleSwitch_CheckedChanged;
                DisplayLocation_ToggleSwitch.Checked = false;
                DisplayLocation_ToggleSwitch.CheckedChanged += DisplayLocation_ToggleSwitch_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if the slider was not yet checked.
            if (DisplayLocation_ToggleSwitch.Checked)
            {
                // Ensure pointers are found.
                if (AoBScanResultsPlayerTools == null)
                {
                    MessageBox.Show("You need to first scan for the Player addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Save some form settings.
                Settings.Default.ChunkViewerAddress = AoBScanResultsPlayerTools.First().ToString("X");

                // Spawn item picker window.
                try
                {
                    ChunkViewer chunkViewer = new ChunkViewer(this);
                    DialogResult dr = chunkViewer.ShowDialog(this);

                    // Get returned item from chunk viewer.
                    chunkViewer.Close();
                }
                catch
                { }
            }
            else
            {
                // Display position is not enabled.
                MessageBox.Show("Display position is not enabled!\nEnable this feature first!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion // End player position chunk.

        #region Godmode

        // Toggle godmode.
        readonly System.Timers.Timer playersGodmodeTimer = new System.Timers.Timer();
        string godmodeAddress = "0";
        private void Godmode_ToggleSwitch_CheckedChanged(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                Godmode_ToggleSwitch.CheckedChanged -= Godmode_ToggleSwitch_CheckedChanged;
                Godmode_ToggleSwitch.Checked = false;
                Godmode_ToggleSwitch.CheckedChanged += Godmode_ToggleSwitch_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsPlayerTools == null)
            {
                // Toggle slider.
                Godmode_ToggleSwitch.CheckedChanged -= Godmode_ToggleSwitch_CheckedChanged;
                Godmode_ToggleSwitch.Checked = false;
                Godmode_ToggleSwitch.CheckedChanged += Godmode_ToggleSwitch_CheckedChanged;

                MessageBox.Show("You need to first scan for the Player addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get the addresses.
            godmodeAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.First().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse(godmode_Offset, NumberStyles.Integer)).ToString("X");

            // Check if the slider was not yet checked.
            if (Godmode_ToggleSwitch.Checked)
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
        private void Speed_ToggleSwitch_CheckedChanged(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                Speed_ToggleSwitch.CheckedChanged -= Speed_ToggleSwitch_CheckedChanged;
                Speed_ToggleSwitch.Checked = false;
                Speed_ToggleSwitch.CheckedChanged += Speed_ToggleSwitch_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsPlayerTools == null)
            {
                // Toggle slider.
                Speed_ToggleSwitch.CheckedChanged -= Speed_ToggleSwitch_CheckedChanged;
                Speed_ToggleSwitch.Checked = false;
                Speed_ToggleSwitch.CheckedChanged += Speed_ToggleSwitch_CheckedChanged;

                MessageBox.Show("You need to first scan for the Player addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get the addresses.
            string playerSpeedAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.First().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse(speed_Offset, NumberStyles.Integer)).ToString("X");

            // Check if the slider was not yet checked.
            if (Speed_ToggleSwitch.Checked)
            {
                // Disable numericupdown.
                SpeedAmount_NumericUpDown.Enabled = false;

                // Slider is being toggled on.
                // Read current value.
                originalSpeed = MemLib.ReadFloat(playerSpeedAddress).ToString();

                // Write new value.
                MemLib.WriteMemory(playerSpeedAddress, "float", (SpeedAmount_NumericUpDown.Value + ".0").ToString()); // Overwrite new value.
            }
            else
            {
                // Slider is being toggled off.
                // Disable numericupdown.
                SpeedAmount_NumericUpDown.Enabled = true;

                // Write value back to original.
                // Write value.
                MemLib.WriteMemory(playerSpeedAddress, "float", originalSpeed); // Overwrite new value.
            }
        }
        #endregion // End player speed.

        #region No Hunger

        // Toggle no hunger.
        readonly System.Timers.Timer playersNoHungerTimer = new System.Timers.Timer();
        string hungerAddress = "0";
        // public IEnumerable<long> AoBScanResultsNoHunger1Tools = null;
        private void InfiniteFood_ToggleSwitch_CheckedChanged(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                InfiniteFood_ToggleSwitch.CheckedChanged -= InfiniteFood_ToggleSwitch_CheckedChanged;
                InfiniteFood_ToggleSwitch.Checked = false;
                InfiniteFood_ToggleSwitch.CheckedChanged += InfiniteFood_ToggleSwitch_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsPlayerTools == null)
            {
                // Toggle slider.
                InfiniteFood_ToggleSwitch.CheckedChanged -= InfiniteFood_ToggleSwitch_CheckedChanged;
                InfiniteFood_ToggleSwitch.Checked = false;
                InfiniteFood_ToggleSwitch.CheckedChanged += InfiniteFood_ToggleSwitch_CheckedChanged;

                MessageBox.Show("You need to first scan for the Player addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get the addresses.
            hungerAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.First().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse(hunger_Offset, NumberStyles.Integer)).ToString("X");

            // Check if the slider was not yet checked.
            if (InfiniteFood_ToggleSwitch.Checked)
            {
                // Slider is being toggled on.
                // Start the timed events.
                playersNoHungerTimer.Interval = 1; // Custom intervals.
                playersNoHungerTimer.Elapsed += new ElapsedEventHandler(PlayersNoHungerTimedEvent);
                playersNoHungerTimer.Start();
            }
            else
            {
                // Slider is being toggled off.
                // Stop the timers.
                playersNoHungerTimer.Stop();
            }

            #region Obsolete Code Backup

            // !! Obsolete Mod 07Feb25 !!
            /*
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
                            AoBScanResultsNoHunger1Tools = await MemLib.AoBScan("", true, true);
                        }
                    }
                    else
                    {
                        // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
                        // Depreciated Address 17Dec22: 01 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 48 44 44 3F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 AC 00 00 00 01 00 00 00 01 00 00 00
                        // Depreciated Address 09Jan23: 01 00 00 00 ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 4? 44 44 3F
                        // Depreciated Address 08Feb23: 01 00 00 00 ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 4? 44 44 3F ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 08 00 00 00
                        // Depreciated Address 10May23: 01 00 00 00 ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 4? 44 44 3F ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 08 00 00 00
                        // Depreciated Address 17Jun24: 01 00 00 00 ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 4? 44 44 3F
                        // Depreciated Address 07Feb25: 01 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 02 00 40 3F
                        AoBScanResultsNoHunger1Tools = await MemLib.AoBScan("", true, true);
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
                        MessageBox.Show("There was an issue trying to fetch the hunger addresses." + Environment.NewLine + "Try reloading the game!!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
    
                    // Rename label to default text.
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
            */
            #endregion
        }

        // Players no hunger timer.
        private void PlayersNoHungerTimedEvent(Object source, ElapsedEventArgs e)
        {
            // Write value.
            MemLib.WriteMemory(hungerAddress, "int", "100"); // Overwrite new value.

            #region Obsolete Code Backup

            // !! Obsolete Code 07Feb25 !!
            /*
                // Update richtextbox with found addresses.
                foreach (long res in AoBScanResultsNoHunger1Tools)
                {
                     // Get the hunger addresses.
                     // Depreciated Offset 10May23: 14
                     // Depreciated Offset 04Oct23: 20
                     string foodAddress1 = BigInteger.Subtract(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("14", NumberStyles.HexNumber)).ToString("X");

                     // Write value.
                     MemLib.WriteMemory(foodAddress1, "int", "100"); // Overwrite new value.
                 }
            */
            #endregion
        }
        #endregion // End no hunger.

        #region Infinite Mana

        // Toggle infinite mana.
        readonly System.Timers.Timer playersInfiniteManaTimer = new System.Timers.Timer();
        string manaAddress = "0";
        private void InfiniteMana_ToggleSwitch_CheckedChanged(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                InfiniteMana_ToggleSwitch.CheckedChanged -= InfiniteMana_ToggleSwitch_CheckedChanged;
                InfiniteMana_ToggleSwitch.Checked = false;
                InfiniteMana_ToggleSwitch.CheckedChanged += InfiniteMana_ToggleSwitch_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsPlayerTools == null)
            {
                // Toggle slider.
                InfiniteMana_ToggleSwitch.CheckedChanged -= InfiniteMana_ToggleSwitch_CheckedChanged;
                InfiniteMana_ToggleSwitch.Checked = false;
                InfiniteMana_ToggleSwitch.CheckedChanged += InfiniteMana_ToggleSwitch_CheckedChanged;

                MessageBox.Show("You need to first scan for the Player addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get the addresses.
            manaAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.First().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse(mana_Offset, NumberStyles.Integer)).ToString("X");

            // Check if the slider was not yet checked.
            if (InfiniteMana_ToggleSwitch.Checked)
            {
                // Slider is being toggled on.
                // Start the timed events.
                playersInfiniteManaTimer.Interval = 1; // Custom intervals.
                playersInfiniteManaTimer.Elapsed += new ElapsedEventHandler(PlayersInfiniteManaTimedEvent);
                playersInfiniteManaTimer.Start();
            }
            else
            {
                // Slider is being toggled off.
                // Stop the timers.
                playersInfiniteManaTimer.Stop();
            }
        }

        // Players position timer.
        private void PlayersInfiniteManaTimedEvent(Object source, ElapsedEventArgs e)
        {
            // Write value.
            MemLib.WriteMemory(manaAddress, "int", "100000"); // Overwrite new value.
        }
        #endregion // End infinite mana.

        #region Noclip

        // Toggle noclip.
        readonly System.Timers.Timer playersNoclipTimer = new System.Timers.Timer();
        string noclipAddress = "0";
        string noclipOriginalValue = "4";
        private void Noclip_ToggleSwitch_CheckedChanged(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                Noclip_ToggleSwitch.CheckedChanged -= Noclip_ToggleSwitch_CheckedChanged;
                Noclip_ToggleSwitch.Checked = false;
                Noclip_ToggleSwitch.CheckedChanged += Noclip_ToggleSwitch_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsPlayerTools == null)
            {
                // Toggle slider.
                Noclip_ToggleSwitch.CheckedChanged -= Noclip_ToggleSwitch_CheckedChanged;
                Noclip_ToggleSwitch.Checked = false;
                Noclip_ToggleSwitch.CheckedChanged += Noclip_ToggleSwitch_CheckedChanged;

                MessageBox.Show("You need to first scan for the Player addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get the addresses.
            // Old alternitive address: 124 // Fix 1.3.4.6 15Jan23. // Reverted 1.3.4.9 09Feb23 - old address: 116.
            // POINTER MAP: 03 ?? (02) - Use the second one.
            noclipAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.First().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse(noclip_Offset, NumberStyles.Integer)).ToString("X");

            // Check if the slider was not yet checked.
            if (Noclip_ToggleSwitch.Checked)
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

        // Players noclip timer.
        private void PlayersNoclipTimedEvent(Object source, ElapsedEventArgs e)
        {
            // Check if noclip is activated or not.
            // Obsolete Note 07Feb25: Using the second noclip address.
            if (IsKeyPressed(0x20) || ForceNoclip_Checkbox.Checked) // Check if space is pressed or if override is on.
            {
                // Write value.
                // Depreciated Value 07Feb25: 0
                MemLib.WriteMemory(noclipAddress, "int", "32"); // Overwrite new value.
            }
            else
            {
                // Write value.
                MemLib.WriteMemory(noclipAddress, "int", noclipOriginalValue); // Overwrite new value.
            }
        }
        #endregion // End noclip.

        #region Suicide

        // Kill the player via suicide.
        private void Suicide_ToggleSwitch_CheckedChanged(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                Suicide_ToggleSwitch.CheckedChanged -= Suicide_ToggleSwitch_CheckedChanged;
                Suicide_ToggleSwitch.Checked = false;
                Suicide_ToggleSwitch.CheckedChanged += Suicide_ToggleSwitch_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsPlayerTools == null)
            {
                // Toggle slider.
                Suicide_ToggleSwitch.CheckedChanged -= Suicide_ToggleSwitch_CheckedChanged;
                Suicide_ToggleSwitch.Checked = false;
                Suicide_ToggleSwitch.CheckedChanged += Suicide_ToggleSwitch_CheckedChanged;

                MessageBox.Show("You need to first scan for the Player addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get the addresses.
            godmodeAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.First().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse(godmode_Offset, NumberStyles.Integer)).ToString("X");

            // Write value.
            MemLib.WriteMemory(godmodeAddress, "int", "0"); // Overwrite new value.

            // Toggle slider.
            Suicide_ToggleSwitch.CheckedChanged -= Suicide_ToggleSwitch_CheckedChanged;
            Suicide_ToggleSwitch.Checked = false;
            Suicide_ToggleSwitch.CheckedChanged += Suicide_ToggleSwitch_CheckedChanged;
        }
        #endregion

        #region Force Recall

        // Kill the player via suicide.
        private async void ForceRecall_ToggleSwitch_CheckedChanged(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                ForceRecall_ToggleSwitch.CheckedChanged -= ForceRecall_ToggleSwitch_CheckedChanged;
                ForceRecall_ToggleSwitch.Checked = false;
                ForceRecall_ToggleSwitch.CheckedChanged += ForceRecall_ToggleSwitch_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsPlayerTools == null)
            {
                // Toggle slider.
                ForceRecall_ToggleSwitch.CheckedChanged -= ForceRecall_ToggleSwitch_CheckedChanged;
                ForceRecall_ToggleSwitch.Checked = false;
                ForceRecall_ToggleSwitch.CheckedChanged += ForceRecall_ToggleSwitch_CheckedChanged;

                MessageBox.Show("You need to first scan for the Player addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Fetch some addresses. // Using noclipOffset as the player state. // 4 = off, 524288 = on.
            string playerStateAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.First().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse(noclip_Offset, NumberStyles.Integer)).ToString("X");

            // Disable slider.
            ForceRecall_ToggleSwitch.Enabled = false;

            // Enable recall sequence value.
            MemLib.WriteMemory(playerStateAddress, "int", "524288");

            // Wait 5 seconds.
            await Task.Delay(5000);

            // Reset the player state.
            MemLib.WriteMemory(playerStateAddress, "int", "4");

            // Enable slider.
            ForceRecall_ToggleSwitch.Enabled = true;

            // Toggle slider.
            ForceRecall_ToggleSwitch.CheckedChanged -= ForceRecall_ToggleSwitch_CheckedChanged;
            ForceRecall_ToggleSwitch.Checked = false;
            ForceRecall_ToggleSwitch.CheckedChanged += ForceRecall_ToggleSwitch_CheckedChanged;
        }
        #endregion // End goto spawn.

        #region Passive AI

        // Passive AI.
        // public IEnumerable<long> AoBScanResultsPassiveAITools = null;
        private void PassiveAI_ToggleSwitch_CheckedChanged(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                // Toggle slider.
                PassiveAI_ToggleSwitch.CheckedChanged -= PassiveAI_ToggleSwitch_CheckedChanged;
                PassiveAI_ToggleSwitch.Checked = false;
                PassiveAI_ToggleSwitch.CheckedChanged += PassiveAI_ToggleSwitch_CheckedChanged;

                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsPlayerTools == null)
            {
                // Toggle slider.
                PassiveAI_ToggleSwitch.CheckedChanged -= PassiveAI_ToggleSwitch_CheckedChanged;
                PassiveAI_ToggleSwitch.Checked = false;
                PassiveAI_ToggleSwitch.CheckedChanged += PassiveAI_ToggleSwitch_CheckedChanged;

                MessageBox.Show("You need to first scan for the Player addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get the addresses.
            string passiveAIAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.First().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse(passiveAI_Offset, NumberStyles.Integer)).ToString("X");

            // Toggle on passive AI.
            if (PassiveAI_ToggleSwitch.Checked)
            {
                // On.
                // Write value.
                MemLib.WriteMemory(passiveAIAddress, "int", "8"); // Overwrite new value.
            }
            else
            {
                // Off.
                // Write value.
                MemLib.WriteMemory(passiveAIAddress, "int", "5"); // Overwrite new value.
            }

            #region Obsolete Code Backup

            // !! Obsolete Mod 07Feb25 !!
            /*
                // Check if the slider was not yet checked.
                if (siticoneWinToggleSwith11.Checked)
                {
                    // Slider is being toggled on.
                    // Name button to indicate loading.
                    label33.Text = "- Loading..";
    
                    // Offset the progress bar to show it's working.
                    progressBar5.Visible = true;
                    progressBar5.Maximum = 100;
                    progressBar5.Value = 10;
    
                    // Check if we need to rescan the addresses or not.
                    if (AoBScanResultsPassiveAITools != null)
                    {
                        string passiveAIAddress = BigInteger.Subtract(BigInteger.Parse(AoBScanResultsPassiveAITools.Last().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("700", NumberStyles.Integer)).ToString("X");
                        int passiveAI = MemLib.ReadInt(passiveAIAddress);
    
                        // Check if we need to rescan food or not.
                        if (passiveAI != 5 && passiveAI != 8)
                        {
                            // Rescan food address.
                            // Depreciated Address 23Mar23: 05 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 6F 50 45 62 84 D3 69 3D 8F C8 4E 5B 20 AA C8 38 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 42 00 00 00 43 00 00 00 44 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 33 00 00 00 34 00 00 00 35 00 00 00 36 00 00 00 37 00 00 00 38 00 00 00 39 00 00 00 3A 00 00 00
                            // Depreciated Address 04Oct23: 6F 50 45 62 84 D3 69 3D 8F C8 4E 5B 20 AA C8 38 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 42 00 00 00 43 00 00 00 44 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 33 00 00 00 34 00 00 00 35 00 00 00 36 00 00 00 37 00 00 00 38 00 00 00 39 00 00 00 3A 00 00 00
                            // Depreciated Address 30Nov23: 6F 50 45 62 84 D3 69 3D 8F C8 4E 5B 20 AA C8 38 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 42 00 00 00 43 00 00 00 44 00 00 00
                            // Depreciated Address 26Apr24: 3B 01 00 00 64 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 03 00 00 00
                            // Depreciated Address 07Feb25: 70 17 00 00 00 00 00 00 ?? ?? ?? ?? 00 00 80 3F
                            AoBScanResultsPassiveAITools = await MemLib.AoBScan("", true, true);
                        }
                    }
                    else
                    {
                        // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
                        AoBScanResultsPassiveAITools = await MemLib.AoBScan("", true, true);
                    }
    
                    // If the count is zero, the scan had an error.
                    if (AoBScanResultsPassiveAITools.Count() == 0)
                    {
                        // Name label to indicate loading.
                        label33.Text = "- Passive AI";
    
                        // Reset progress bar.
                        progressBar5.Value = 0;
                        progressBar5.Visible = false;
    
                        // Reset aob scan results
                        AoBScanResultsPassiveAITools = null;
    
                        // Toggle slider.
                        siticoneWinToggleSwith11.CheckedChanged -= SiticoneWinToggleSwith11_CheckedChanged;
                        siticoneWinToggleSwith11.Checked = false;
                        siticoneWinToggleSwith11.CheckedChanged += SiticoneWinToggleSwith11_CheckedChanged;
    
                        // Display error message.
                        MessageBox.Show("There was an issue trying to fetch the passive AI addresses." + Environment.NewLine + "Try reloading the game!!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
    
                    // Update richtextbox with found addresses.
                    richTextBox6.Text = "Addresses Loaded: 0"; // Reset textbox.
                    foreach (long res in AoBScanResultsPassiveAITools)
                    {
                        if (richTextBox6.Text == "Addresses Loaded: 0")
                        {
                            richTextBox6.Text = "Passive AI Addresses Loaded: " + AoBScanResultsPassiveAITools.Count() + " [" + res.ToString("X").ToString();
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
    
                    // Rename label to default text.
                    label33.Text = "- Passive AI";
    
                    // Toggle on passive AI.
                    foreach (long res in AoBScanResultsPassiveAITools)
                    {
                        // Get the passive AI addresses.
                        // OLD: 124
                        string passiveAIAddress = BigInteger.Subtract(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("700", NumberStyles.Integer)).ToString("X");
    
                        // Write value.
                        MemLib.WriteMemory(passiveAIAddress, "int", "8"); // Overwrite new value.
                    }
                }
                else
                {
                    // Slider is being toggled off.
                    // Reset label name.
                    label33.Text = "- Passive AI";
    
                    // Complete progress bar.
                    progressBar5.Value = 100;
                    progressBar5.Visible = false;
    
                    // Toggle off passive AI.
                    foreach (long res in AoBScanResultsPassiveAITools)
                    {
                        // Get the passive AI addresses.
                        string passiveAIAddress = BigInteger.Subtract(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("700", NumberStyles.Integer)).ToString("X");
    
                        // Write value.
                        MemLib.WriteMemory(passiveAIAddress, "int", "5"); // Overwrite new value.
                    }
                }
            */
            #endregion
        }
        #endregion // End passive AI.

        #region Decommissioned: Anti Collision

        // !! Obsolete Mod 07Feb25 !!
        /*
            // Toggle anti collision.
            // string playerStateOriginalValue;
            // string playerStateAddress;
            // string playerStateNoClipAddress;
            // readonly System.Timers.Timer playersAntiCollisionTimer = new System.Timers.Timer();
            private void SiticoneWinToggleSwith8_CheckedChanged(object sender, EventArgs e)
            {
                // Open the process and check if it was successful before the AoB scan.
                if (!MemLib.OpenProcess("CoreKeeper"))
                {
                    // Toggle slider.
                    siticoneWinToggleSwith8.CheckedChanged -= SiticoneWinToggleSwith8_CheckedChanged;
                    siticoneWinToggleSwith8.Checked = false;
                    siticoneWinToggleSwith8.CheckedChanged += SiticoneWinToggleSwith8_CheckedChanged;

                    MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Ensure pointers are found.
                if (AoBScanResultsPlayerTools == null)
                {
                    // Toggle slider.
                    siticoneWinToggleSwith8.CheckedChanged -= SiticoneWinToggleSwith8_CheckedChanged;
                    siticoneWinToggleSwith8.Checked = false;
                    siticoneWinToggleSwith8.CheckedChanged += SiticoneWinToggleSwith8_CheckedChanged;

                    MessageBox.Show("You need to first scan for the Player addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Display warning message.
                if (siticoneWinToggleSwith8.Checked)
                {
                    MessageBox.Show("This mod is now obsolete!\n\nEnabling this will now force noclip to be continuous.", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                #region Obsolete Code Backup

                // !! Obsolete Mod 07Feb25 !!
                    // Get the addresses.
                    // Get the noclip addresses.
                    playerStateAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.First().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse(playerStateBaseOffset, NumberStyles.Integer)).ToString("X");
                    playerStateNoClipAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.First().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse(playerStateAntiCollisionOffset, NumberStyles.Integer)).ToString("X");

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
                #endregion
            }

            region Obsolete Code Backup

            // !! Obsolete Mod Timer 07Feb25 !!
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
            #endregion
        */

        #endregion // End anti collision.

        #endregion // End player tab.

        #region World Tab

        #region Teleport Tool Addresses

        // Get Teleport Player addresses.
        private void GetTeleportAddresses_Button_Click(object sender, EventArgs e)
        {
            // Reset progress bar.
            TeleportPlayer_ProgressBar.Value = 0;

            // Load addresses.
            GetPlayerLocationAddresses();
        }

        // Launch the teleport address guide.
        private void TeleportPlayerHelp_Button_Click(object sender, EventArgs e)
        {
            // De-select button.
            TeleportPlayerHelp_Button.Enabled = false;
            TeleportPlayerHelp_Button.Enabled = true;

            // Spawn teleport address guide window.
            TeleportAddressGuide teleportAddressGuide = new TeleportAddressGuide();
            DialogResult dr = teleportAddressGuide.ShowDialog(this);

            // Close returning form.
            teleportAddressGuide.Close();
        }

        // Toggle brute force teleport player addresses.
        private void BruteForceTP_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Double check if the player wishes to enable this.
            if (BruteForceTP_CheckBox.Checked && MessageBox.Show("This option should only be used if normal scaning brings no results.\n\nThis could crash your game in the process -\nSaving prior is recommended!\n\nAre you sure you wish to brute force the address searching?", "Brute Force Teleport Address Search", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                // Disable the checkbox.
                BruteForceTP_CheckBox.Checked = false;
            }
        }

        public IEnumerable<long> AoBScanResultsPlayerLocationScanner = null;
        public async void GetPlayerLocationAddresses()
        {
            // Amount of times to rescan the address.
            int scanTimes = 20;

            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Name button to indicate loading.
            GetTeleportAddresses_Button.Text = "Loading...";

            // Disable button to prevent spamming.
            // button11.Enabled = false;
            TeleportPlayer_GroupBox.Enabled = false;

            // Reset textbox.
            TeleportPlayerAddresses_RichTextBox.Text = "Addresses Loaded: 0";

            // Offset the progress bar to show it's working.
            TeleportPlayer_ProgressBar.Visible = true;
            TeleportPlayer_ProgressBar.Maximum = 100;
            TeleportPlayer_ProgressBar.Step = 70 / scanTimes;
            TeleportPlayer_ProgressBar.Value = 10;

            // Select an address based on brute force mode.
            string AoBPlayerLocationArray = "9? 99 19 3E 00 00 00 00 ?? 99 D9 3F 00 00 80 3F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 80 3F";
            if (BruteForceTP_CheckBox.Checked)
            {
                // Brute force mode is enabled, switch array.
                AoBPlayerLocationArray = "9? 99 19 3E 00 00 00 00 ?? 99 D9 3F";
            }

            // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
            // Depreciated Address 08Feb23: C? CC CC 3D 00 00 00 00 ?? 99 D9 3F ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 ?0 ?? 00 00
            // Depreciated Address 11Mar23: C? CC CC 3D 00 00 00 00 ?? 99 D9 3F ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 ?? ?? ?? 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00
            // Depreciated Address 23Jun23: C? CC CC 3D 00 00 00 00 ?? 99 D9 3F ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 ?? ?? ?? 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00
            // Depreciated Address 26Oct23: C? CC CC 3D 00 00 00 00 ?? 99 D9 3F ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 ?? ?? ?? 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 ?? ?? 00 00
            // Depreciated Address 21Feb24: C? CC CC 3D 00 00 00 00 ?? 99 D9 3F ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00
            // Depreciated Address 21Aug24: C? CC CC 3D 00 00 00 00 ?? 99 D9 3F 00 00 80 3F 00 00 00 00 00 00 00 00 00 00 00 00 00 00 80 3F // Version 0.7.5, the world 0,0 base position was moved on X-Axis: 0.05 to the left.
            AoBScanResultsPlayerLocationScanner = await MemLib.AoBScan(AoBPlayerLocationArray, true, true);

            // If the count is zero, the scan had an error.
            if (AoBScanResultsPlayerLocationScanner.Count() < 1)
            {
                // Reset textbox.
                TeleportPlayerAddresses_RichTextBox.Text = "Addresses Loaded: 0";

                // Reset progress bar.
                TeleportPlayer_ProgressBar.Value = 0;
                TeleportPlayer_ProgressBar.Visible = false;

                // Rename button back to default.
                GetTeleportAddresses_Button.Text = "Get Addresses";

                // Re-enable button.
                GetTeleportAddresses_Button.Enabled = true;
                TeleportPlayer_GroupBox.Enabled = true;

                // Reset aob scan results.
                AoBScanResultsPlayerLocation = null;
                AoBScanResultsPlayerLocationScanner = null;

                // Display error message.
                MessageBox.Show("You must be standing at the core's entrance!!\r\rTIP: Press 'W' & 'D' keys when at the core's entrance.\r\rCommunity Feedback Support:\r(1) Hold [W] into The Core entrance alcove.\r(2) Tap [D].\r(3) Release [W].", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Update the progress bar.
            TeleportPlayer_ProgressBar.Value = 20;

            // Display info message.
            MessageBox.Show("Now stand in the Glurch (slime boss) statue entrance.\rHold W and then tap D for precise positioning.\r\rPress 'ok' when ready!", "SUCCESS - STEP 2 OF 2", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Update the progressbar step.
            TeleportPlayer_ProgressBar.Step = 35 / scanTimes;

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
                        // Depreciated Address 21Aug24: C? CC CC 3D 00 00 00 00 CD CC 0C 41

                        string byte1 = res.ToString("X"); // 9? 99 19 3E 00 00 00 00 CD CC 0C 41
                        string byte2 = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("1", NumberStyles.Integer)).ToString("X");
                        string byte3 = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("2", NumberStyles.Integer)).ToString("X");
                        string byte4 = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("3", NumberStyles.Integer)).ToString("X");
                        string byte5 = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("4", NumberStyles.Integer)).ToString("X");
                        string byte6 = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("5", NumberStyles.Integer)).ToString("X");
                        string byte7 = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("6", NumberStyles.Integer)).ToString("X");
                        string byte8 = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("7", NumberStyles.Integer)).ToString("X");
                        string byte9 = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");
                        string byte10 = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("9", NumberStyles.Integer)).ToString("X");
                        // string byte11 = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("10", NumberStyles.Integer)).ToString("X");
                        // string byte12 = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("11", NumberStyles.Integer)).ToString("X");

                        // Check if value does not exist.
                        if (
                            MemLib.ReadByte(byte1).ToString("X").ToCharArray()[0].ToString() != "9" || // 9? 99 19 3E 00 00 00 00 CD CC 0C 41
                            // Skip: '?'
                            MemLib.ReadByte(byte2).ToString("X").ToCharArray()[0].ToString() != "9" ||
                            MemLib.ReadByte(byte2).ToString("X").ToCharArray()[1].ToString() != "9" ||
                            MemLib.ReadByte(byte3).ToString("X").ToCharArray()[0].ToString() != "1" ||
                            MemLib.ReadByte(byte3).ToString("X").ToCharArray()[1].ToString() != "9" ||
                            MemLib.ReadByte(byte4).ToString("X").ToCharArray()[0].ToString() != "3" ||
                            MemLib.ReadByte(byte4).ToString("X").ToCharArray()[1].ToString() != "E" ||

                            MemLib.ReadByte(byte5).ToString("X").ToString() != "0" ||
                            MemLib.ReadByte(byte6).ToString("X").ToString() != "0" ||
                            MemLib.ReadByte(byte7).ToString("X").ToString() != "0" ||
                            MemLib.ReadByte(byte8).ToString("X").ToString() != "0" ||

                            MemLib.ReadByte(byte9).ToString("X").ToCharArray()[0].ToString() != "C" ||
                            MemLib.ReadByte(byte9).ToString("X").ToCharArray()[1].ToString() != "D" ||
                            MemLib.ReadByte(byte10).ToString("X").ToCharArray()[0].ToString() != "C" ||
                            MemLib.ReadByte(byte10).ToString("X").ToCharArray()[1].ToString() != "C"
                            // Skip: '0'
                            // Skip: 'C'
                            // Skip: '4'
                            // Skip: '1'
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
                TeleportPlayer_ProgressBar.PerformStep();
            }

            // Update the progressbar step.
            TeleportPlayer_ProgressBar.Step = (resultLocations.Count() == 0) ? 1 : 35 / resultLocations.Count();

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

                // Wait for 0.35 seconds.
                // Fix v1.3.6.2: Changed delay to 0.50 seconds.
                await System.Threading.Tasks.Task.Delay(500);

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

                        // Wait for 0.35 seconds.
                        await System.Threading.Tasks.Task.Delay(350);
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
                TeleportPlayer_ProgressBar.PerformStep();
            }

            // Update the IEnumerable.
            AoBScanResultsPlayerLocation = resultLocations;

            // If the count is less then five, the scan had an error.
            if (AoBScanResultsPlayerLocation.Count() < 1)
            {
                // Reset textbox.
                TeleportPlayerAddresses_RichTextBox.Text = "Addresses Loaded: 0";

                // Reset progress bar.
                TeleportPlayer_ProgressBar.Value = 0;
                TeleportPlayer_ProgressBar.Visible = false;

                // Rename button back to default.
                GetTeleportAddresses_Button.Text = "Get Addresses";

                // Re-enable button.
                GetTeleportAddresses_Button.Enabled = true;
                TeleportPlayer_GroupBox.Enabled = true;

                // Reset aob scan results.
                AoBScanResultsPlayerLocation = null;
                AoBScanResultsPlayerLocationScanner = null;

                // Display error message.
                MessageBox.Show("There was an issue finding the address!\rTry leaving the world or restarting the game!\r\rINFORMATION: You must be standing at the 'Glurch the Abominous Mass's entrance!!\r\rTIP: Press 'W' & 'D' keys when at the 'Glurch the Abominous Mass's entrance.\r\rCommunity Feedback Support:\r(1) Hold [W] into The Glurch the Abominous Mass's entrance alcove.\r(2) Tap [D].\r(3) Release [W].", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (AoBScanResultsPlayerLocation.Count() > 1 && AoBScanResultsPlayerLocation.Count() < 10) // Check if or between 1 & 9.
            {
                // Display error message.
                MessageBox.Show($"WARNING! There is more than a single address found! ({AoBScanResultsPlayerLocation.Count()})\nWhile this mod may still work, long term use may cause crashes.\r\rIt's recommended to reload the world or restart the game and scan again.", warningTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // return; No return is needed.
            }
            else if (AoBScanResultsPlayerLocation.Count() > 10 && !BruteForceTP_CheckBox.Checked) // Override check if brute force is on.
            {
                // Reset textbox.
                TeleportPlayerAddresses_RichTextBox.Text = "Addresses Loaded: 0";

                // Reset progress bar.
                TeleportPlayer_ProgressBar.Value = 0;
                TeleportPlayer_ProgressBar.Visible = false;

                // Rename button back to default.
                GetTeleportAddresses_Button.Text = "Get Addresses";

                // Re-enable button.
                GetTeleportAddresses_Button.Enabled = true;
                TeleportPlayer_GroupBox.Enabled = true;

                // Reset aob scan results.
                AoBScanResultsPlayerLocation = null;
                AoBScanResultsPlayerLocationScanner = null;

                // Display error message.
                MessageBox.Show("Whoa there! We found too many addresses!\r\rPlease try launching the game as vanilla via steam!\rTIP: Some mod managers do not launch it as true vanilla.", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Update richtextbox with found addresses.
            foreach (long res in AoBScanResultsPlayerLocation)
            {
                if (TeleportPlayerAddresses_RichTextBox.Text == "Addresses Loaded: 0")
                {
                    TeleportPlayerAddresses_RichTextBox.Text = "Teleport Addresses Loaded: " + AoBScanResultsPlayerLocation.Count().ToString() + " [" + res.ToString("X").ToString();
                }
                else
                {
                    TeleportPlayerAddresses_RichTextBox.Text += ", " + res.ToString("X").ToString();
                }
            }
            TeleportPlayerAddresses_RichTextBox.Text += "]";

            // Re-enable button.
            // button11.Enabled = true;
            TeleportPlayer_GroupBox.Enabled = true;

            // Rename button back to default.
            GetTeleportAddresses_Button.Text = "Get Addresses";

            // Complete progress bar.
            TeleportPlayer_ProgressBar.Value = 100;

            // Hide progressbar.
            TeleportPlayer_ProgressBar.Visible = false;
        }
        #endregion // End world tool addresses.

        #region Map Teleport Addresses

        // Using a 'Task' instead of a 'void' to allow for 'await'ing.
        public async Task GetPlayerMapLocationAddresses()
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Name button to indicate loading.
            GetTeleportAddresses_Button.Text = "Loading...";

            // Disable button to prevent spamming.
            TeleportPlayer_GroupBox.Enabled = false;

            // Reset textbox.
            TeleportPlayerAddresses_RichTextBox.Text = "Addresses Loaded: 0";

            // Offset the progress bar to show it's working.
            TeleportPlayer_ProgressBar.Visible = true;
            TeleportPlayer_ProgressBar.Maximum = 100;
            TeleportPlayer_ProgressBar.Step = 25;
            TeleportPlayer_ProgressBar.Value = 10;

            // Reference: [MapUI]
            // Select an address based on brute force mode.
            //                                                                                                            [           ] - mapOpenedAtLeastOnce.
            string AoBPlayerMapLocationArrayOne = "?? 02 00 00 00 00 00 00 04 00 00 00 CD CC CC 3D 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 80 3F 00 00 00 00";
            string AoBPlayerMapLocationArrayTwo = "?? 02 00 00 00 00 00 00 04 00 00 00 CD CC CC 3D 00 00 00 00 ?? ?? ?? ?? 01 00 00 00 ?? ?? ?? ?? 00 00 80 3F 00 00 00 00";

            // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
            AoBScanResultsPlayerMapLocation = await MemLib.AoBScan(AoBPlayerMapLocationArrayOne, true, true);

            // Update the progress bar.
            TeleportPlayer_ProgressBar.Value = 50;

            // If the count is zero, the scan had an error.
            if (AoBScanResultsPlayerMapLocation.Count() < 1)
            {
                // Reset textbox.
                TeleportPlayerAddresses_RichTextBox.Text = "Addresses Loaded: 0";

                // Reset progress bar.
                TeleportPlayer_ProgressBar.Value = 0;
                TeleportPlayer_ProgressBar.Visible = false;

                // Rename button back to default.
                GetTeleportAddresses_Button.Text = "Get Addresses";

                // Re-enable button.
                GetTeleportAddresses_Button.Enabled = true;
                TeleportPlayer_GroupBox.Enabled = true;

                // Reset aob scan results.
                AoBScanResultsPlayerMapLocation = null;

                // Display error message.
                MessageBox.Show("There was an issue trying to fetch the map teleport addresses." + Environment.NewLine + "Try reloading the game!!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (AoBScanResultsPlayerMapLocation.Count() > 1)
            {
                // If the count is more then one, try using array two.
                // Display info message.
                MessageBox.Show("Multiple map teleport addresses where found. This is normal and will require an additional step.\r\r1) Open the overhead map.\r2) Press 'ok' when ready!", "Map Teleport Helper", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Update the progress bar.
                TeleportPlayer_ProgressBar.Value = 75;

                // Rescan the AoB using the second array.
                AoBScanResultsPlayerMapLocation = await MemLib.AoBScan(AoBPlayerMapLocationArrayTwo, true, true);
            }

            // If the count is not one, the scan had an error.
            if (AoBScanResultsPlayerMapLocation.Count() != 1)
            {
                // Reset textbox.
                TeleportPlayerAddresses_RichTextBox.Text = "Addresses Loaded: 0";

                // Reset progress bar.
                TeleportPlayer_ProgressBar.Value = 0;
                TeleportPlayer_ProgressBar.Visible = false;

                // Rename button back to default.
                GetTeleportAddresses_Button.Text = "Get Addresses";

                // Re-enable button.
                GetTeleportAddresses_Button.Enabled = true;
                TeleportPlayer_GroupBox.Enabled = true;

                // Reset aob scan results.
                AoBScanResultsPlayerMapLocation = null;

                // Display error message.
                MessageBox.Show($"There was an issue trying to fetch the map teleport addresses. Count: [{AoBScanResultsPlayerMapLocation.Count()}]" + Environment.NewLine + "Try reloading the game!!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Update richtextbox with found addresses.
            foreach (long res in AoBScanResultsPlayerMapLocation)
            {
                if (TeleportPlayerAddresses_RichTextBox.Text == "Addresses Loaded: 0")
                {
                    TeleportPlayerAddresses_RichTextBox.Text = "Teleport Map Addresses Loaded: " + AoBScanResultsPlayerMapLocation.Count().ToString() + " [" + res.ToString("X").ToString();
                }
                else
                {
                    TeleportPlayerAddresses_RichTextBox.Text += ", " + res.ToString("X").ToString();
                }
            }
            TeleportPlayerAddresses_RichTextBox.Text += "]";

            // Re-enable button.
            // button11.Enabled = true;
            TeleportPlayer_GroupBox.Enabled = true;

            // Rename button back to default.
            GetTeleportAddresses_Button.Text = "Get Addresses";

            // Complete progress bar.
            TeleportPlayer_ProgressBar.Value = 100;

            // Hide progressbar.
            TeleportPlayer_ProgressBar.Visible = false;
        }
        #endregion

        #region Map Rendering Addresses

        // Get map rendering addresses.
        private void GetMapRenderingAddresses_Button_Click(object sender, EventArgs e)
        {
            // Reset progress bar.
            MapRendering_ProgressBar.Value = 0;

            // Load addresses.
            GetMapRevealAddresses();
        }

        // This bool is for separating the vanilla & modded reveal ranges.
        // Depreciated 28May25: public static bool mapRevealIsVanilla = false;
        public async void GetMapRevealAddresses()
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Name button to indicate loading.
            GetMapRenderingAddresses_Button.Text = "Loading...";

            // Disable button to prevent spamming.
            // button11.Enabled = false;
            MapRendering_GroupBox.Enabled = false;

            // Reset textbox.
            MapRenderingAddresses_RichTextBox.Text = "Addresses Loaded: 0";

            // Offset the progress bar to show it's working.
            MapRendering_ProgressBar.Visible = true;
            MapRendering_ProgressBar.Maximum = 100;
            MapRendering_ProgressBar.Step = 40;
            MapRendering_ProgressBar.Value = 10;

            #region Depreciated: Scan GameAssembly.dll Module

            /*
            // Find the GameAssembly.dll module start and end region within memory.
            // Get a collection of all modules within a process.
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

            // Define reveal range address variable.
            AoBScanResultsRevealMapRange = await MemLib.AoBScan(moduleStart, moduleEnd, "00 00 40 41 00 00 10 42", true, true, false);

            // Adjust the offset of the address.
            List<long> AoBScanResultsRevealMapRangeTemp = new List<long>();
            foreach (long res in AoBScanResultsRevealMapRange)
            {
                // Add the new offset to the list.
                // long revealRange = (long)BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("4", NumberStyles.Integer));

                // Ensure the default value is 12.
                if (MemLib.ReadFloat(res.ToString("X")).ToString() == "12")
                {
                    AoBScanResultsRevealMapRangeTemp.Add(res);
                }
            }

            AoBScanResultsRevealMapRange = AoBScanResultsRevealMapRangeTemp;
            */
            #endregion

            #region Depreciated: Scan Modded vs Vanilla

            /*
            // Build the completed list.
            // Vanilla: C3 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 E0 40 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
            // Modded:  00 00 40 41 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 0C 00 00 00 34 00 00 00 00 00 04 40

            // Scan the modded address first.
            mapRevealIsVanilla = false; // Change flag to default.
            AoBScanResultsRevealMapRange = await MemLib.AoBScan("00 00 40 41 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 0C 00 00 00 34 00 00 00 00 00 04 40", true, true);

            // Check if the modded AOB address was found.
            if (AoBScanResultsRevealMapRange.Count() < 1)
            {
                // Address not found. Lets try scanning for modded.
                AoBScanResultsRevealMapRange = await MemLib.AoBScan("C3 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 E0 40 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00", true, true);

                // Change the modded flag.
                mapRevealIsVanilla = true;
            }
            */
            #endregion

            // Reference: [MapUpdateSystem] float num = (this._largeRevealDistance ? 12f : 7f);
            // Depreciated Address 10Feb23: GameAssembly.dll+381D950
            // Depreciated Address 15Feb23: GameAssembly.dll+3877D1C
            // Depreciated Address 19Feb23: GameAssembly.dll+387DDAC
            // Depreciated Address 05Apr23: 41 00 00 40 41
            // Depreciated Address 10May23: 00 00 30 41 00 00 40 41
            // Depreciated Address 05Oct23: 00 00 40 41 00 00 10 42
            // Depreciated Address 28May25: 00 00 40 41 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 0C 00 00 00 34 00 00 00 00 00 04 40
            // Depreciated Address 24Jun25: 51 01 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 E0 40 00 00 00 00 01 04 02 05 04 03 01 50 00 00 00 00
            // Future: [51 01 00 00 EB C4 BA C9 01 00 00 EB BD BA E1 01 00 00 EB B6] or [00 00 E0 40 00 00 00 00 01 04 02 05 04 03 01 50 00 00 00 00]
            // Scan the reveal range address first.
            AoBScanResultsRevealMapRange = await MemLib.AoBScan("51 01 00 00 EB C4 BA C9 01 00 00 EB BD BA E1 01 00 00 EB B6", true, true);

            // Check for the reveal range addresses.
            if (AoBScanResultsRevealMapRange.Count() < 1 || AoBScanResultsRevealMapRange.Count() > 2) // Using 2 as a cap.
            {
                // Reset textbox.
                TeleportPlayerAddresses_RichTextBox.Text = "Addresses Loaded: 0";

                // Reset progress bar.
                MapRendering_ProgressBar.Value = 0;
                MapRendering_ProgressBar.Visible = false;

                // Rename button back to default.
                GetMapRenderingAddresses_Button.Text = "Get Addresses";

                // Re-enable button.
                GetMapRenderingAddresses_Button.Enabled = true;
                MapRendering_GroupBox.Enabled = true;

                // Reset aob scan results
                AoBScanResultsDevMapReveal = null;

                MessageBox.Show("There was an issue gathering the reveal range addresses! Found: " + AoBScanResultsRevealMapRange.Count() + "\r\rTry restarting your game!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Progress progress bar.
            MapRendering_ProgressBar.PerformStep();

            // Reference: [MapUpdateSystem] OnlyRevealLitTiles = !this._largeRevealDistance,
            // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
            // Depreciated Address: 04 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 01 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 04 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 04 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00
            // Depreciated Address 31May23: 04 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 01 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 04 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 04 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 34 00 00 00 A0 01 00 00 00 00 00 00 20 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00
            // Depreciated Address 05Oct23: 04 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 01 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 04 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 04 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 3F 00 00 00 00 00 00 00 3F
            // Depreciated Address 23Aug24: ?? 00 00 00 A6 9B C4 3A ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? 00 00 A0 40 00 00 00 00
            // Depreciated Address 28May25: ?? 00 00 00 A6 9B C4 3A ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? 00 00 A0 40 00 00 00 00
            // Depreciated Address 12Jun25: 30 0D 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? 01 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? FB 7F 00 00 34 00 00 00
            // Depreciated Address 24Jun25: FF FF FF FF 0A 00 00 00 ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00
            AoBScanResultsDevMapReveal = await MemLib.AoBScan("34 00 00 00 00 00 60 41 00 00 60 41", true, true);

            // Perform progressbar step.
            MapRendering_ProgressBar.PerformStep();

            // If the count is zero, the scan had an error.
            if (AoBScanResultsDevMapReveal.Count() < 1)
            {
                // Reset textbox.
                TeleportPlayerAddresses_RichTextBox.Text = "Addresses Loaded: 0";

                // Reset progress bar.
                MapRendering_ProgressBar.Value = 0;
                MapRendering_ProgressBar.Visible = false;

                // Rename button back to default.
                GetMapRenderingAddresses_Button.Text = "Get Addresses";

                // Re-enable button.
                GetMapRenderingAddresses_Button.Enabled = true;
                MapRendering_GroupBox.Enabled = true;

                // Reset aob scan results
                AoBScanResultsDevMapReveal = null;

                // Display error message.
                MessageBox.Show("There was an issue gathering the map reveal dev addresses! Found: " + AoBScanResultsDevMapReveal.Count() + "\r\rTry restarting your game!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Update richtextbox with found addresses.
            string revealMapRangeAddresses = string.Join(", ", AoBScanResultsRevealMapRange.Select(val => val.ToString("X")));
            string devMapRevealAddresses   = string.Join(", ", AoBScanResultsDevMapReveal.Select(val => val.ToString("X")));
            int addressCount               = AoBScanResultsRevealMapRange.Count() + AoBScanResultsDevMapReveal.Count();
            MapRenderingAddresses_RichTextBox.Text = $"Addresses Loaded ({addressCount}): R:[{revealMapRangeAddresses}], D:[{devMapRevealAddresses}]";

            // Re-enable button.
            // button11.Enabled = true;
            MapRendering_GroupBox.Enabled = true;

            // Rename button back to default.
            GetMapRenderingAddresses_Button.Text = "Get Addresses";

            // Complete progress bar.
            MapRendering_ProgressBar.Value = 100;

            // Hide progressbar.
            MapRendering_ProgressBar.Visible = false;
        }
        #endregion // End map rendering addresses.

        #region Set Render Distance

        // Set custom render distance.
        private async void SetRevealRange_Button_Click(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsDevMapReveal == null || AoBScanResultsRevealMapRange == null)
            {
                MessageBox.Show("You need to first scan for the map rendering addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Reset progress bar.
            MapRendering_ProgressBar.Visible = true;
            MapRendering_ProgressBar.Value = 0;

            // Enable custom render.
            foreach (long res in AoBScanResultsDevMapReveal)
            {
                // Get the offset.
                string devAddress = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("105", NumberStyles.Integer)).ToString("X");
                MemLib.WriteMemory(devAddress, "byte", "1");
            }

            // Set the custom render.
            foreach (long res in AoBScanResultsRevealMapRange)
            {
                // Set the new value within memory.
                // string rangeAddress = mapRevealIsVanilla ? BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("16", NumberStyles.Integer)).ToString("X") : BigInteger.Subtract(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");
                string rangeAddress = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("32", NumberStyles.Integer)).ToString("X");
                MemLib.WriteMemory(rangeAddress, "float", RenderRange_NumericUpDown.Value.ToString());
            }

            // Update the progress bar.
            if (MapRendering_ProgressBar.Maximum >= 100)
                MapRendering_ProgressBar.Value = 100;
            await Task.Delay(1000);
            MapRendering_ProgressBar.Visible = false;
        }
        #endregion // End set render distance.

        #region Set default Render Distance

        // Restore default render.
        private async void RestoreDefaultRange_Button_Click(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsDevMapReveal == null || AoBScanResultsRevealMapRange == null)
            {
                MessageBox.Show("You need to first scan for the map rendering addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Reset progress bar.
            MapRendering_ProgressBar.Visible = true;
            MapRendering_ProgressBar.Value = 0;

            // Disable custom render.
            foreach (long res in AoBScanResultsDevMapReveal)
            {
                // Get the offset.
                string devAddress = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("105", NumberStyles.Integer)).ToString("X");
                MemLib.WriteMemory(devAddress, "byte", "0");
            }

            // Set the custom render.
            foreach (long res in AoBScanResultsRevealMapRange)
            {
                // Set the new value within memory.
                // string rangeAddress = mapRevealIsVanilla ? BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("16", NumberStyles.Integer)).ToString("X") : BigInteger.Subtract(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");
                string rangeAddress = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("32", NumberStyles.Integer)).ToString("X");
                MemLib.WriteMemory(rangeAddress, "float", "12");
            }

            // Update the render range numericupdown.
            RenderRange_NumericUpDown.Value = 12;

            // Update the progress bar.
            if (MapRendering_ProgressBar.Maximum >= 100)
                MapRendering_ProgressBar.Value = 100;
            await Task.Delay(1000);
            MapRendering_ProgressBar.Visible = false;
        }
        #endregion // End set default render distance.

        #region Auto Render Map

        #region Controls

        // Pause operations.
        private void PauseOperation_Button_Click(object sender, EventArgs e)
        {
            // Ensure the button is enabled first.
            if (PauseOperation_Button.Enabled)
            {
                // Get the button state.
                if (PauseOperation_Button.Text == "Pause Operation")
                {
                    pauseRenderingOperation = true;

                    // Update the buttons text.
                    PauseOperation_Button.Text = "Resume Operation";

                    // Disable some controls.
                    CancelOperation_Button.Enabled = false;
                }
                else if (PauseOperation_Button.Text == "Resume Operation")
                {
                    pauseRenderingOperation = false;

                    // Update the buttons text.
                    PauseOperation_Button.Text = "Pause Operation";

                    // Enable some controls.
                    CancelOperation_Button.Enabled = true;
                }
            }
        }

        // Cancel auto renderer.
        private void CancelOperation_Button_Click(object sender, EventArgs e)
        {
            // Disable the groupbox.
            MapRendering_GroupBox.Enabled = false;

            // Ensure the button is visable first.
            if (CancelOperation_Button.Visible)
            {
                cancelRenderingOperation = true;
            }
        }

        // Prevent value from being larger then the max radius.
        private void StartRadius_NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            // Check value.
            if (StartRadius_NumericUpDown.Value > MaxRadius_NumericUpDown.Value)
                StartRadius_NumericUpDown.Value = MaxRadius_NumericUpDown.Value;
        }
        #endregion

        // Set anti collision and godmode timer variables.
        // string renderMapPlayerStateAddress;
        // string renderMapPlayerStateOriginalValue;
        string renderMapNoClipAddress;
        string renderMapNoClipOriginalValue; // Should be 4.
        string renderMapGodmodeAddress;

        // Render map anti collision timer.
        readonly System.Timers.Timer renderMapOperationsTimer = new System.Timers.Timer();
        private void RenderMapOperationsTimedEvent(Object source, ElapsedEventArgs e)
        {
            // Write new values.
            MemLib.WriteMemory(renderMapNoClipAddress, "int", "32");      // Noclip. - Obsolete: Anti collision.
            MemLib.WriteMemory(renderMapGodmodeAddress, "int", "100000"); // Godmode.
        }

        // Auto render the map.
        public bool cancelRenderingOperation = false;
        public bool pauseRenderingOperation = false;
        private async void AutoMapRenderer_Button_Click(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsPlayerLocation == null)
            {
                MessageBox.Show("You need to first scan for the Teleport Player addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsPlayerTools == null)
            {
                MessageBox.Show("You need to first scan for the Player addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsDevMapReveal == null)
            {
                MessageBox.Show("You need to first scan for the Map Rendering addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure the min radius is not larger then the max.
            if (StartRadius_NumericUpDown.Value > MaxRadius_NumericUpDown.Value)
            {
                MessageBox.Show("The minimum radius cannot be larger then the max radius!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                StartRadius_NumericUpDown.Value = MaxRadius_NumericUpDown.Value; // Reset the min to the max value.
                return;
            }

            // Do a try statement.
            try
            {
                // Do initial variable reset.
                // renderMapPlayerStateAddress = "";
                renderMapNoClipAddress = "";
                renderMapNoClipOriginalValue = "";
                renderMapGodmodeAddress = "";

                // Define players initial position.
                var initialres = AoBScanResultsPlayerTools.Last();
                float xlocres = MemLib.ReadFloat(BigInteger.Add(BigInteger.Parse(initialres.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse(positionX_Offset, NumberStyles.Integer)).ToString("X"));
                float ylocres = MemLib.ReadFloat(BigInteger.Add(BigInteger.Parse(initialres.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse(positionY_Offset, NumberStyles.Integer)).ToString("X"));
                Vector2 initialPosition = new Vector2(xlocres, ylocres);

                // Define entree values.
                Vector2 localPosition = initialPosition;
                int maxRadius = (int)MaxRadius_NumericUpDown.Value; // Max radius.
                int minRadius = (int)StartRadius_NumericUpDown.Value; // Min radius.
                int stepSize = (int)RenderRange_NumericUpDown.Value; // Range.
                double radialMoveScale = (double)RadialMoveScale_NumericUpDown.Value; // radialMoveScale.
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

                // Calculate time and prompt user.
                int calculateCount = 0;

                #region Calculate Render Time

                // Calculate the total time required.
                if ((int)StartRadius_NumericUpDown.Value > 0)
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
                string time = (((calculateCount * (int)NextRingDelay_NumericUpDown.Value) / 60000) >= 60) ? ((calculateCount * (int)NextRingDelay_NumericUpDown.Value) / 60000 / 60) + " hours." : (((calculateCount * (int)NextRingDelay_NumericUpDown.Value) / 1000) >= 60) ? ((calculateCount * (int)NextRingDelay_NumericUpDown.Value) / 60000) + " minutes." : ((calculateCount * (int)NextRingDelay_NumericUpDown.Value) / 1000) + " seconds";
                time = (((calculateCount * (int)NextRingDelay_NumericUpDown.Value) / 60000 / 60) >= 24) ? (((calculateCount * (int)NextRingDelay_NumericUpDown.Value) / 60000 / 60) / 24) + " days." : time;
                if (MessageBox.Show("This operaration will take ~" + time + "\n\nContinue?", "Attention!!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    // User canceld, exit void.
                    return;
                }
                #endregion

                // Reset the progress bar.
                MapRendering_TextProgressBar.Visible = true;
                MapRendering_TextProgressBar.Maximum = calculateCount; // Set the progress bar total to the total required points to complete.
                MapRendering_TextProgressBar.Step = 1;
                MapRendering_TextProgressBar.Value = 0;
                MapRendering_TextProgressBar.CustomText = "0.00% | Current Radius: 0";

                // Change button to indicate loading.
                AutoMapRenderer_Button.Text = "Loading...";
                AutoMapRenderer_Button.Enabled = false;
                AutoMapRenderer_Button.Visible = false;
                CancelOperation_Button.Visible = true;
                PauseOperation_Button.Enabled = true;
                cancelRenderingOperation = false;

                // Enable custom render.
                foreach (long res in AoBScanResultsDevMapReveal)
                {
                    // Get the offset.
                    string devAddress = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("105", NumberStyles.Integer)).ToString("X");
                    MemLib.WriteMemory(devAddress, "byte", "1");
                }

                // Set the custom render.
                foreach (long res in AoBScanResultsRevealMapRange)
                {
                    // Set the new value within memory.
                    // string rangeAddress = mapRevealIsVanilla ? BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("16", NumberStyles.Integer)).ToString("X") : BigInteger.Subtract(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");
                    string rangeAddress = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("32", NumberStyles.Integer)).ToString("X");
                    MemLib.WriteMemory(rangeAddress, "float", RenderRange_NumericUpDown.Value.ToString());
                }

                // Reset variable.
                rPrevious = minRadius;

                // Get the anti collision addresses.
                // renderMapPlayerStateAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.First().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse(playerStateBaseOffset, NumberStyles.Integer)).ToString("X");
                // renderMapPlayerStateOriginalValue = MemLib.ReadInt(renderMapPlayerStateAddress).ToString(); // Save state for returning later.
                renderMapNoClipAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.First().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse(noclip_Offset, NumberStyles.Integer)).ToString("X");
                renderMapNoClipOriginalValue = MemLib.ReadInt(renderMapNoClipAddress).ToString(); // Save state for returning later.

                // Get the godmode address.
                renderMapGodmodeAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.First().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse(godmode_Offset, NumberStyles.Integer)).ToString("X");

                // Declare the current start time.
                DateTime startTime = DateTime.Now;

                // Enable noclip, godmode, and start the timed events.
                renderMapOperationsTimer.Interval = 1; // Custom intervals.
                renderMapOperationsTimer.Elapsed += new ElapsedEventHandler(RenderMapOperationsTimedEvent);
                renderMapOperationsTimer.Start();

                #region Do Rendering

                // Math for creating a filled / hollow circle.
                #region Initial Y Offset

                if ((int)StartRadius_NumericUpDown.Value > 0)
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
                    MapRendering_TextProgressBar.PerformStep();
                    MapRendering_TextProgressBar.CustomText = decimal.Parse((stepsCompleted / (decimal)((decimal)calculateCount / 100)).ToString("0.00")).ToString() + "% | Current Radius: " + (int)StartRadius_NumericUpDown.Value;

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

                    // Cancel the rendering operation.
                    if (cancelRenderingOperation)
                    {
                        // Reenable controls.
                        cancelRenderingOperation = false;
                        AutoMapRenderer_Button.Enabled = true;
                        AutoMapRenderer_Button.Visible = true;
                        AutoMapRenderer_Button.Text = "Auto Map Renderer";
                        CancelOperation_Button.Visible = false; // Hide cancel button.
                        PauseOperation_Button.Enabled = false;
                        MapRendering_GroupBox.Enabled = true;

                        // End look.
                        goto exitLoop;
                    }
                }
                #endregion

                for (r = minRadius; r <= maxRadius; r += stepSize) // Loop through each circle radius within ranges
                {
                    x = xoffset;

                    #region Moving Between Circles

                    for (y = rPrevious; y < r; y += (int)((double)stepSize * radialMoveScale)) //Move upwards between successive circles
                    {
                        // Obsolete 07Feb25: Noclip & Anti-collision are the same now!
                        // Force enable noclip to prevent unclipping.
                        // MemLib.WriteMemory(playerStateAddress, "int", MemLib.ReadInt(playerStateNoClipAddress).ToString());

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
                        MapRendering_TextProgressBar.PerformStep();
                        MapRendering_TextProgressBar.CustomText = decimal.Parse((stepsCompleted / (decimal)((decimal)calculateCount / 100)).ToString("0.00")).ToString() + "% | Current Radius: " + r;

                        // Add a cooldown.
                        await Task.Delay((int)NextRingDelay_NumericUpDown.Value);

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

                        // Cancel the rendering operation.
                        if (cancelRenderingOperation)
                        {
                            // Reenable controls.
                            cancelRenderingOperation = false;
                            AutoMapRenderer_Button.Enabled = true;
                            AutoMapRenderer_Button.Visible = true;
                            AutoMapRenderer_Button.Text = "Auto Map Renderer";
                            CancelOperation_Button.Visible = false; // Hide cancel button.
                            MapRendering_GroupBox.Enabled = true;

                            // End look.
                            goto exitLoop;
                        }
                    }
                    #endregion

                    #region Move Around Circle

                    double delta = (double)((double)stepSize / (double)r);
                    double theta;
                    for (theta = 0; theta < 2 * Math.PI; theta += (delta * radialMoveScale)) // Move around current radius circle
                    {
                        // Obsolete 07Feb25: Noclip & Anti-collision are the same now!
                        // Force enable noclip to prevent unclipping.
                        // MemLib.WriteMemory(playerStateAddress, "int", MemLib.ReadInt(playerStateNoClipAddress).ToString());

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
                        MapRendering_TextProgressBar.PerformStep();
                        MapRendering_TextProgressBar.CustomText = decimal.Parse((stepsCompleted / (decimal)((decimal)calculateCount / 100)).ToString("0.00")).ToString() + "% | Current Radius: " + r;

                        // Add a cooldown.
                        await Task.Delay((int)NextRingDelay_NumericUpDown.Value);

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

                        // Cancel the rendering operation.
                        if (cancelRenderingOperation)
                        {
                            // Reenable controls.
                            cancelRenderingOperation = false;
                            AutoMapRenderer_Button.Enabled = true;
                            AutoMapRenderer_Button.Visible = true;
                            AutoMapRenderer_Button.Text = "Auto Map Renderer";
                            CancelOperation_Button.Visible = false; // Hide cancel button.
                            MapRendering_GroupBox.Enabled = true;

                            // End look.
                            goto exitLoop;
                        }
                    }
                    #endregion

                    rPrevious = r;

                    #region After Completed Ring Operations

                    // Save the maps progress before starting next ring.
                    // Skip first circle check: r != (int)numericUpDown16.Value
                    if (SaveEachRing_CheckBox.Checked && r != 0)
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
                        MessageBox.Show("The Core Keeper process was no longer found!\rRecord your progress!\r\rTask ran for " + timeDifferenceCrashed.Days + " day(s), " + timeDifferenceCrashed.Hours + " hour(s), " + timeDifferenceCrashed.Minutes + " minute(s), " + timeDifferenceCrashed.Seconds + " seconds.\r\r~" + (stepSize * stepSize) * count + " tiles have been rendered.", "Render Map", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                        // Reenable controls.
                        MapRendering_GroupBox.Enabled = true;
                        cancelRenderingOperation = false;
                        AutoMapRenderer_Button.Enabled = true;
                        AutoMapRenderer_Button.Visible = true;
                        AutoMapRenderer_Button.Text = "Auto Map Renderer";
                        CancelOperation_Button.Visible = false; // Hide cancel button.
                        MapRendering_GroupBox.Enabled = true;

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
                MapRendering_GroupBox.Enabled = true;
                cancelRenderingOperation = false;
                AutoMapRenderer_Button.Enabled = true;
                AutoMapRenderer_Button.Visible = true;
                AutoMapRenderer_Button.Text = "Auto Map Renderer";
                CancelOperation_Button.Visible = false; // Hide cancel button.
                PauseOperation_Button.Enabled = false;
                MapRendering_TextProgressBar.Visible = false;
                MapRendering_TextProgressBar.Maximum = 100;
                MapRendering_TextProgressBar.CustomText = "";

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
                // Obsolete 07Feb25: MemLib.WriteMemory(renderMapPlayerStateAddress, "int", renderMapPlayerStateOriginalValue);

                // Reset noclip.
                MemLib.WriteMemory(renderMapNoClipAddress, "int", renderMapNoClipOriginalValue);

                // Reset variables.
                rPrevious = minRadius;

                #region Calculate Total Tiles Rendered

                // Calculate the total tiles and display result.
                if ((int)StartRadius_NumericUpDown.Value > 0)
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
            catch (Exception p)
            {
                MessageBox.Show("An error occurred and was caught!\n\n" + p.ToString(), "Render Map", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion // End render map.

        #region Remove Ground Items

        // Delete all ground items.
        private async void TrashGroundItems_Button_Click(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Reset progress bar.
            TeleportPlayer_ProgressBar.Step = 1;
            TeleportPlayer_ProgressBar.Value = 5;
            TeleportPlayer_ProgressBar.Visible = true;
            TeleportPlayer_ProgressBar.PerformStep(); // Progress 10%.

            // Name button to indicate loading.
            TrashGroundItems_Button.Text = "Removing Items..";

            // Disable button to prevent spamming.
            TrashGroundItems_Button.Enabled = false;
            BruteForceTrash_CheckBox.Enabled = false;

            // Check if brute force mode is enabled.
            // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
            if (BruteForceTrash_CheckBox.Checked)
            {
                // Do brute force scanning.
                AoBScanResultsGroundItems = await MemLib.AoBScan("01 00 00 00 01 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 01 00 00 00", true, true);
            }
            else
            {
                // Do normal scanning.
                // Depreciated Address 23Oct23: 6E 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 01 00 00 00 ?? ?? ?? ?? 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 01 00 00 00
                // Depreciated Address 04May24: 01 00 00 00 01 00 00 00 6E 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ??
                // Depreciated Address 23Feb25: 01 00 00 00 01 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 01 00 00 00 01 00 00 00 6E 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                AoBScanResultsGroundItems = await MemLib.AoBScan("01 00 00 00 01 00 00 00 6E 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00", true, true);
            }

            // Adjust the max value of the progress bar.
            // PlayerTools_ProgressBar.Step = AoBScanResultsGroundItems.Count() == 0 ? 0 : 90.0 / Math.Max(1, AoBScanResultsGroundItems.Count());
            PlayerTools_ProgressBar.Maximum = AoBScanResultsGroundItems.Count();

            // If the count is zero, the scan had an error.
            if (AoBScanResultsGroundItems.Count() == 0)
            {
                // Rename button back to default.
                TrashGroundItems_Button.Text = "Remove Ground Items";

                // Enable controls.
                TrashGroundItems_Button.Enabled = true;
                BruteForceTrash_CheckBox.Enabled = true;

                // Ensure progressbar is at 100.
                TeleportPlayer_ProgressBar.Value = 100;
                TeleportPlayer_ProgressBar.Visible = false;

                // Update console with the status.
                if (BruteForceTrash_CheckBox.Checked)
                    WorldTools_RichTextBox.AppendText("[RemoveGroundItems_BruteForce] Failed to find any addresses. No ground items found!!" + Environment.NewLine);
                else
                    WorldTools_RichTextBox.AppendText("[RemoveGroundItems] Throw a torch on the ground, and walk away from it!!" + Environment.NewLine);
                WorldTools_RichTextBox.ScrollToCaret();

                // Display error message.
                if (BruteForceTrash_CheckBox.Checked)
                    MessageBox.Show("Failed to find any addresses. No ground items found!!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                else
                    MessageBox.Show("You must throw a torch on the ground, and walk away from it!!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Perform step.
            // TeleportPlayer_ProgressBar.PerformStep();

            // Remove ground items.
            await RemoveGroundItemsAsync();

            // Process completed, run finishing tasks.
            // Rename button back to default.
            TrashGroundItems_Button.Text = "Trash Ground Items";
            TeleportPlayer_ProgressBar.Visible = false;

            // Enable controls.
            TrashGroundItems_Button.Enabled = true;
            BruteForceTrash_CheckBox.Enabled = true;
        }

        // Toggle brute force trash ground items addresses.
        private void BruteForceTrash_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Double check if the player wishes to enable this.
            if (BruteForceTrash_CheckBox.Checked && MessageBox.Show("This option should only be used if normal item removal brings no results.\n\nThis could crash your game in the process -\nSaving prior is recommended!\n\nAre you sure you wish to brute force the removal of ground items?", "Brute Force Trash Ground Items", MessageBoxButtons.YesNo) == DialogResult.No)
            {
                // Disable the checkbox.
                BruteForceTrash_CheckBox.Checked = false;
            }
        }

        // Remove items function.
        public async Task RemoveGroundItemsAsync()
        {
            await Task.Run(() =>
            {
                // Reset progress bar on UI thread.
                TeleportPlayer_ProgressBar.Invoke((MethodInvoker)(() =>
                {
                    TeleportPlayer_ProgressBar.Maximum = AoBScanResultsGroundItems.Count();
                    TeleportPlayer_ProgressBar.Step = 1;
                    TeleportPlayer_ProgressBar.Value = 0;
                    TeleportPlayer_ProgressBar.Visible = true;
                }));

                // Iterate through each found address.
                foreach (long res in AoBScanResultsGroundItems)
                {
                    // Ensure the process is still alive.
                    if (!MemLib.OpenProcess("CoreKeeper"))
                    {
                        MessageBox.Show("The process appears to have died! Canceling task.", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Check if brute force mode is enabled.
                    if (BruteForceTrash_CheckBox.Checked)
                    {
                        // Gather record debug data pointers.
                        string ItemHeader1 = res.ToString("X").ToString();
                        // string ItemHeader2 = BigInteger.Add(BigInteger.Parse(ItemHeader1, NumberStyles.HexNumber), BigInteger.Parse("4", NumberStyles.Integer)).ToString("X");
                        string ItemType = BigInteger.Add(BigInteger.Parse(ItemHeader1, NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");
                        string ItemAmount = BigInteger.Add(BigInteger.Parse(ItemHeader1, NumberStyles.HexNumber), BigInteger.Parse("12", NumberStyles.Integer)).ToString("X");
                        string ItemVariant = BigInteger.Add(BigInteger.Parse(ItemHeader1, NumberStyles.HexNumber), BigInteger.Parse("16", NumberStyles.Integer)).ToString("X");
                        // string UnknownVariable = BigInteger.Add(BigInteger.Parse(ItemHeader1, NumberStyles.HexNumber), BigInteger.Parse("20", NumberStyles.Integer)).ToString("X");
                        string ItemSkillset = BigInteger.Add(BigInteger.Parse(ItemHeader1, NumberStyles.HexNumber), BigInteger.Parse("24", NumberStyles.Integer)).ToString("X");
                        // string ItemFooter1 = BigInteger.Add(BigInteger.Parse(ItemHeader1, NumberStyles.HexNumber), BigInteger.Parse("28", NumberStyles.Integer)).ToString("X");
                        // string ItemFooter2 = BigInteger.Add(BigInteger.Parse(ItemHeader1, NumberStyles.HexNumber), BigInteger.Parse("32", NumberStyles.Integer)).ToString("X");

                        // Log the items removed.
                        int ItemTypeValue = MemLib.ReadInt(ItemType);
                        int ItemAmountValue = MemLib.ReadInt(ItemAmount);
                        string logMessage = "Item Removed BruteForce: " + "ItemID: " + ItemTypeValue + " | Amount: " + ItemAmountValue + Environment.NewLine;

                        // Update UI with removed item details.
                        WorldTools_RichTextBox.Invoke((MethodInvoker)(() =>
                        {
                            // Ensure that the item is valid.
                            if (ItemTypeValue != 0)
                            {
                                WorldTools_RichTextBox.AppendText(logMessage);
                                WorldTools_RichTextBox.ScrollToCaret();
                                WorldTools_RichTextBox.Refresh();
                            }
                        }));

                        // Ensure that the item is valid.
                        if (ItemTypeValue != 0)
                        {
                            // Write all ground item values to zero.
                            MemLib.WriteMemory(ItemType, "int", "0");
                            MemLib.WriteMemory(ItemAmount, "int", "0");
                            // MemLib.WriteMemory(ItemVariant, "int", "0");
                            // MemLib.WriteMemory(ItemSkillset, "int", "0");
                        }
                    }
                    else
                    {
                        // Start at the last byte of the memory address.
                        string currentByte = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("32", NumberStyles.Integer)).ToString("X");
                        bool foundPattern = true;

                        // Climb down the address for each item.
                        while (foundPattern)
                        {
                            // Subtract current byte by 8 and 7 to get the double one values.
                            int byteValueOne = MemLib.ReadInt(BigInteger.Subtract(BigInteger.Parse(currentByte, NumberStyles.HexNumber), BigInteger.Parse("32", NumberStyles.Integer)).ToString("X"));
                            int byteValueTwo = MemLib.ReadInt(BigInteger.Subtract(BigInteger.Parse(currentByte, NumberStyles.HexNumber), BigInteger.Parse("28", NumberStyles.Integer)).ToString("X"));

                            // Check if the current 8th and 9th bytes are both "1".
                            if (byteValueOne == 1 && byteValueTwo == 1)
                            {
                                // Gather record debug data pointers.
                                string ItemHeader1 = BigInteger.Subtract(BigInteger.Parse(currentByte, NumberStyles.HexNumber), BigInteger.Parse("32", NumberStyles.Integer)).ToString("X");
                                // string ItemHeader2 = BigInteger.Add(BigInteger.Parse(ItemHeader1, NumberStyles.HexNumber), BigInteger.Parse("4", NumberStyles.Integer)).ToString("X");
                                string ItemType = BigInteger.Add(BigInteger.Parse(ItemHeader1, NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");
                                string ItemAmount = BigInteger.Add(BigInteger.Parse(ItemHeader1, NumberStyles.HexNumber), BigInteger.Parse("12", NumberStyles.Integer)).ToString("X");
                                string ItemVariant = BigInteger.Add(BigInteger.Parse(ItemHeader1, NumberStyles.HexNumber), BigInteger.Parse("16", NumberStyles.Integer)).ToString("X");
                                // string UnknownVariable = BigInteger.Add(BigInteger.Parse(ItemHeader1, NumberStyles.HexNumber), BigInteger.Parse("20", NumberStyles.Integer)).ToString("X");
                                string ItemSkillset = BigInteger.Add(BigInteger.Parse(ItemHeader1, NumberStyles.HexNumber), BigInteger.Parse("24", NumberStyles.Integer)).ToString("X");
                                // string ItemFooter1 = BigInteger.Add(BigInteger.Parse(ItemHeader1, NumberStyles.HexNumber), BigInteger.Parse("28", NumberStyles.Integer)).ToString("X");
                                // string ItemFooter2 = BigInteger.Add(BigInteger.Parse(ItemHeader1, NumberStyles.HexNumber), BigInteger.Parse("32", NumberStyles.Integer)).ToString("X");

                                // Log the items removed.
                                int ItemTypeValue = MemLib.ReadInt(ItemType);
                                int ItemAmountValue = MemLib.ReadInt(ItemAmount);
                                int ItemVariantValue = MemLib.ReadInt(ItemVariant);
                                int ItemSkillsetValue = MemLib.ReadInt(ItemSkillset);
                                string logMessage = "Item Removed: " + "ItemID: " + ItemTypeValue + " | Amount: " + ItemAmountValue + " | Variation: " + ItemVariantValue + " | Skillset: " + ItemSkillsetValue + Environment.NewLine;

                                // Update UI with removed item details.
                                WorldTools_RichTextBox.Invoke((MethodInvoker)(() =>
                                {
                                    // Ensure that the item is valid.
                                    if (ItemTypeValue != 0)
                                    {
                                        WorldTools_RichTextBox.AppendText(logMessage);
                                        WorldTools_RichTextBox.ScrollToCaret();
                                        WorldTools_RichTextBox.Refresh();
                                    }
                                }));

                                // Ensure that the item is valid.
                                if (ItemTypeValue != 0)
                                {
                                    // Write all ground item values to zero.
                                    MemLib.WriteMemory(ItemType, "int", "0");
                                    MemLib.WriteMemory(ItemAmount, "int", "0");
                                    MemLib.WriteMemory(ItemVariant, "int", "0");
                                    MemLib.WriteMemory(ItemSkillset, "int", "0");
                                }

                                // !! Obsolete Code 23Feb25 !!
                                /*
                                // If "1 1" is found, set the previous 9 bytes to 0.
                                string byteSets = currentByte;
                                for (int x = 1; x < 10; x++)
                                {
                                    // Write current value to zero. Subtract to next stepback byte.
                                    MemLib.WriteMemory(byteSets, "int", "0");
                                    byteSets = BigInteger.Subtract(BigInteger.Parse(byteSets, NumberStyles.HexNumber), BigInteger.Parse("4", NumberStyles.Integer)).ToString("X");
                                }
                                */

                                // Move the index 9 bytes back.
                                currentByte = BigInteger.Subtract(BigInteger.Parse(currentByte, NumberStyles.HexNumber), BigInteger.Parse("36", NumberStyles.Integer)).ToString("X");
                            }
                            else
                            {
                                // If "1 1" is not found, break the loop.
                                foundPattern = false;
                            }
                        }
                    }

                    // Progress the progress bar.
                    TeleportPlayer_ProgressBar.Invoke((MethodInvoker)(() =>
                    {
                        TeleportPlayer_ProgressBar.PerformStep();
                    }));
                }

                // Ensure progress bar is at 100 and hide it.
                TeleportPlayer_ProgressBar.Invoke((MethodInvoker)(() =>
                {
                    TeleportPlayer_ProgressBar.Value = TeleportPlayer_ProgressBar.Maximum;
                    TeleportPlayer_ProgressBar.Visible = false;
                }));
            });
        }
        #endregion // End trash ground items.

        #region Teleport Player

        // Teleport the player to a world position.
        private void TeleportXY_Button_Click(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsPlayerLocation == null)
            {
                MessageBox.Show("You need to first scan for the Teleport Player addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Change button to indicate loading.
            TeleportXY_Button.Text = "Teleporting...";
            TeleportXY_Button.Enabled = false;

            // Iterate through each found address.
            foreach (long res in AoBScanResultsPlayerLocation)
            {
                // Get address from loop.
                string playerX = res.ToString("X").ToString();
                string playerY = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");

                // Send player to X.
                MemLib.WriteMemory(playerX, "float", TeleportX_NumericUpDown.Value.ToString());

                // Send player to Y.
                MemLib.WriteMemory(playerY, "float", TeleportY_NumericUpDown.Value.ToString());
            }

            // Process completed, run finishing tasks.
            // Rename button back to default.
            TeleportXY_Button.Text = "Teleport XY";
            TeleportXY_Button.Enabled = true;
        }

        // Numericupdown key down teleport player.
        private void TeleportX_NumericUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Open the process and check if it was successful before the AoB scan.
                if (!MemLib.OpenProcess("CoreKeeper"))
                {
                    MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Ensure pointers are found.
                if (AoBScanResultsPlayerLocation == null)
                {
                    MessageBox.Show("You need to first scan for the Teleport Player addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Change button to indicate loading.
                TeleportXY_Button.Text = "Teleporting...";
                TeleportXY_Button.Enabled = false;

                // Iterate through each found address.
                foreach (long res in AoBScanResultsPlayerLocation)
                {
                    // Get address from loop.
                    string playerX = res.ToString("X").ToString();
                    string playerY = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");

                    // Send player to X.
                    MemLib.WriteMemory(playerX, "float", TeleportX_NumericUpDown.Value.ToString());

                    // Send player to Y.
                    MemLib.WriteMemory(playerY, "float", TeleportY_NumericUpDown.Value.ToString());
                }

                // Process completed, run finishing tasks.
                // Rename button back to default.
                TeleportXY_Button.Text = "Teleport XY";
                TeleportXY_Button.Enabled = true;
            }
        }

        // Numericupdown key down teleport player.
        private void TeleportY_NumericUpDown_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Open the process and check if it was successful before the AoB scan.
                if (!MemLib.OpenProcess("CoreKeeper"))
                {
                    MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Ensure pointers are found.
                if (AoBScanResultsPlayerLocation == null)
                {
                    MessageBox.Show("You need to first scan for the Teleport Player addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Change button to indicate loading.
                TeleportXY_Button.Text = "Teleporting...";
                TeleportXY_Button.Enabled = false;

                // Iterate through each found address.
                foreach (long res in AoBScanResultsPlayerLocation)
                {
                    // Get address from loop.
                    string playerX = res.ToString("X").ToString();
                    string playerY = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");

                    // Send player to X.
                    MemLib.WriteMemory(playerX, "float", TeleportX_NumericUpDown.Value.ToString());

                    // Send player to Y.
                    MemLib.WriteMemory(playerY, "float", TeleportY_NumericUpDown.Value.ToString());
                }

                // Process completed, run finishing tasks.
                // Rename button back to default.
                TeleportXY_Button.Text = "Teleport XY";
                TeleportXY_Button.Enabled = true;

            }
        }
        #endregion // End teleport player.

        #region Map Teleport

        #region Map Teleport Mod Offsets

        // Suppress CS0414 (field is assigned but its value is never used).
        // Suppressing since these are modding offsets meant for future use or documentation.
        #pragma warning disable CS0414

        // Below contains all the offsets for the map teleport mod.
        // These values are all added to the map teleports base address.
                                                             // Base + Offset.
        readonly string mapMiddleClickX_Offset      = "20";  // (this.mapUI.GetCursorWorldPosition().X).
        readonly string mapOpenedAtLeastOnce_Offset = "24";  // (0=false, 1=true).
        readonly string mapMiddleClickY_Offset      = "28";  // (this.mapUI.GetCursorWorldPosition().Y).
        readonly string mapOpen_Offset              = "92";  // (0=close, 1=open).
        readonly string mapLeftClickX_Offset        = "136"; // (this.mapUI.GetCursorWorldPosition().X).
        readonly string mapLeftClickY_Offset        = "140"; // (this.mapUI.GetCursorWorldPosition().Y).

        #pragma warning restore CS0414

        #endregion

        // Toggle map teleport.
        readonly System.Timers.Timer mapTeleportTimer = new System.Timers.Timer();
        string mapOpen_Address        = "0";
        string mapLeftClickX_Address  = "0";
        string mapLeftClickY_Address  = "0";
        private async void MapTeleport_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Check if the checkbox's state is checked to ensure the checks are only ran upon checked.
            if (MapTeleport_CheckBox.Checked)
            {
                // Open the process and check if it was successful before the AoB scan.
                if (!MemLib.OpenProcess("CoreKeeper"))
                {
                    // Toggle checkbox.
                    MapTeleport_CheckBox.Checked = false;

                    MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Ensure the teleport player pointers are found.
                if (AoBScanResultsPlayerLocation == null)
                {
                    // Toggle checkbox.
                    MapTeleport_CheckBox.Checked = false;

                    MessageBox.Show("You need to first scan for the Teleport Player addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // STEP1: Ensure pointers are found.
                if (AoBScanResultsPlayerMapLocation == null)
                {
                    // First scan for the map location address.
                    await GetPlayerMapLocationAddresses();
                }
                else
                {
                    // Pointers are found, check if they are still valid.
                    //
                    string mapLocationCheckAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerMapLocation.First().ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("12", NumberStyles.Integer)).ToString("X");
                    float mapLocationCheck = MemLib.ReadFloat(mapLocationCheckAddress, round: false); // _pingCooldown - Should be 0.1f.

                    // Check if we need to rescan crafting or not.
                    if (mapLocationCheck != 0.1f)
                    {
                        // Scan for the map location address.
                        await GetPlayerMapLocationAddresses();
                    }
                }

                // STEP2: Ensure the address fetching was successful.
                if (AoBScanResultsPlayerMapLocation == null)
                {
                    // Toggle checkbox; No need for error message, this was handled by the fetch.
                    MapTeleport_CheckBox.Checked = false;
                    return;
                }

                // Calculate the address offsets outside of the timed loop to save perfomance.
                mapOpen_Address = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerMapLocation.First().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse(mapOpen_Offset, NumberStyles.Integer)).ToString("X");
                mapLeftClickX_Address = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerMapLocation.First().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse(mapLeftClickX_Offset, NumberStyles.Integer)).ToString("X");
                mapLeftClickY_Address = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerMapLocation.First().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse(mapLeftClickY_Offset, NumberStyles.Integer)).ToString("X");

                // Checkbox is being toggled on.
                // Start the timed events.
                mapTeleportTimer.Interval = 1; // Custom intervals.
                mapTeleportTimer.Elapsed += new ElapsedEventHandler(MapTeleportTimedEvent);
                mapTeleportTimer.Start();
            }
            else
            {
                // Checkbox is being toggled off.
                // Stop the timers.
                mapTeleportTimer.Stop();
            }
        }

        // Map teleport timer.
        float oldmapToWorldPosX = 0;
        float oldmapToWorldPosY = 0;
        private void MapTeleportTimedEvent(Object source, ElapsedEventArgs e)
        {
            // Check if the overhead map is open.
            bool mapIsOpen = MemLib.ReadInt(mapOpen_Address) == 1;

            // Ensure the map is currently opened.
            if (mapIsOpen)
            {
                // Gather the existing clicked locations.
                float mapToWorldPosX = MemLib.ReadFloat(mapLeftClickX_Address);
                float mapToWorldPosY = MemLib.ReadFloat(mapLeftClickY_Address);
                bool clickPositionChanged = mapToWorldPosX != oldmapToWorldPosX || mapToWorldPosY != oldmapToWorldPosY;

                // Update the old click positions reguardless of 0,0 checks.
                if (clickPositionChanged)
                {
                    oldmapToWorldPosX = mapToWorldPosX;
                    oldmapToWorldPosY = mapToWorldPosY;
                }

                // Check if the existing click position changed and the initial positions are not 0,0.
                if (clickPositionChanged && mapToWorldPosX != 0 && mapToWorldPosY != 0)
                {
                    // Iterate through each found address.
                    foreach (long res in AoBScanResultsPlayerLocation)
                    {
                        // Get address from loop.
                        string playerX = res.ToString("X").ToString();
                        string playerY = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");

                        // Send player to X.
                        MemLib.WriteMemory(playerX, "float", mapToWorldPosX.ToString());

                        // Send player to Y.
                        MemLib.WriteMemory(playerY, "float", mapToWorldPosY.ToString());
                    }
                }
            }
        }
        #endregion

        #region Get World Information

        // Get world information.
        private async void GetWorldInformation_Button_Click(object sender, EventArgs e)
        {
            // Ensure properties are filled.
            if (WorldInformation_TextBox.Text == "")
            {
                // Display error message.
                MessageBox.Show("You must type the world name you wish to use!!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Load world information.
            await LoadWorldInformation();
        }

        // Get world information keydown.
        private async void WorldInformation_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Get enter key.
            if (e.KeyCode == Keys.Enter)
            {
                // Ensure properties are filled.
                if (WorldInformation_TextBox.Text == "")
                {
                    // Display error message.
                    MessageBox.Show("You must type the world name you wish to use!!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Open the process and check if it was successful before the AoB scan.
                if (!MemLib.OpenProcess("CoreKeeper"))
                {
                    MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Load world information.
                await LoadWorldInformation();
            }
        }

        #region Copy Cell Text

        // Copy the value to the clipboard.
        private void WorldInformation_DataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Catch any errors.
            try
            {
                // Define cell text.
                var cellText = WorldInformation_DataGridView.Rows[e.RowIndex].Cells[1].Value.ToString();

                // Ensure text is not blank.
                if (cellText != "")
                {
                    // Set the clipboard.
                    Clipboard.SetText(cellText);
                }
            }
            catch (Exception) { };
        }
        #endregion // End copy cell text.

        #region Color Dropdown Choices

        private void WorldDifficulty_ComboBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            // Draw the background.
            e.DrawBackground();

            // Unselect combobox text.
            this.BeginInvoke(new Action(() => { WorldDifficulty_ComboBox.Select(0, 0); }));

            // Get the item text.
            string text = ((ComboBox)sender).Items[e.Index].ToString();

            // Determine the forecolor based on whether or not the item is selected.
            Brush brush;
            if (e.Index == 0) // Standard.
            {
                brush = new SolidBrush(ColorTranslator.FromHtml("#000000")); // White.
            }
            else if (e.Index == 1) // Hard.
            {
                brush = new SolidBrush(ColorTranslator.FromHtml("#F93A3B")); // Red.
            }
            else if (e.Index == 2) // Creative.
            {
                brush = new SolidBrush(ColorTranslator.FromHtml("#01ADF9")); // Blue.
            }
            else if (e.Index == 3) // Hard, Creative.
            {
                brush = new SolidBrush(ColorTranslator.FromHtml("#F93A3B")); // Error color.
            }
            else if (e.Index == 4) // Casual.
            {
                brush = new SolidBrush(ColorTranslator.FromHtml("#21DE5F")); // Green.
            }
            else if (e.Index == 5) // Hard, Casual.
            {
                brush = new SolidBrush(ColorTranslator.FromHtml("#F93A3B")); // Error color.
            }
            else if (e.Index == 6) // Creative, Casual.
            {
                brush = new SolidBrush(ColorTranslator.FromHtml("#01ADF9")); // Blue.
            }
            else if (e.Index == 7) // Hard, Creative, Casual.
            {
                brush = new SolidBrush(ColorTranslator.FromHtml("#F93A3B")); // Error color.
            }
            else // Other.
            {
                brush = new SolidBrush(ColorTranslator.FromHtml("#F93A3B")); // Error color.
            }

            // Draw the text.
            e.Graphics.DrawString(text, ((Control)sender).Font, brush, e.Bounds.X, e.Bounds.Y);
        }
        #endregion

        #region Deselect Combobox

        // Unselect the combobox text after the dropdown closes.
        private void WorldDifficulty_ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            // Unselect combobox text.
            this.BeginInvoke(new Action(() => { WorldDifficulty_ComboBox.Select(0, 0); }));
        }
        #endregion

        #region Change World Icon Label

        // Change the icon text based on the selection.
        private void Icon_NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            // Set the labels text based on numeric updowns value.
            if (Icon_NumericUpDown.Value == 0)
                CurrentIcon_Label.Text = "Glow Tulup";
            else if (Icon_NumericUpDown.Value == 1)
                CurrentIcon_Label.Text = "Gold Ore";
            else if (Icon_NumericUpDown.Value == 2)
                CurrentIcon_Label.Text = "Recall Idol";
            else if (Icon_NumericUpDown.Value == 3)
                CurrentIcon_Label.Text = "Bomb";
            else if (Icon_NumericUpDown.Value == 4)
                CurrentIcon_Label.Text = "Lantern";
            else if (Icon_NumericUpDown.Value == 5)
                CurrentIcon_Label.Text = "Mushroom";
            else if (Icon_NumericUpDown.Value == 6)
                CurrentIcon_Label.Text = "Gemstone";
            else if (Icon_NumericUpDown.Value == 7)
                CurrentIcon_Label.Text = "Ammonite";
            else if (Icon_NumericUpDown.Value == 8)
                CurrentIcon_Label.Text = "Cavelink Skull";
            else if (Icon_NumericUpDown.Value == 9)
                CurrentIcon_Label.Text = "Ocarina";
            else if (Icon_NumericUpDown.Value == 10)
                CurrentIcon_Label.Text = "Golden Feather";
            else if (Icon_NumericUpDown.Value == 11)
                CurrentIcon_Label.Text = "Cavelink Doll";
            else if (Icon_NumericUpDown.Value == 12)
                CurrentIcon_Label.Text = "Old Journal";
            else if (Icon_NumericUpDown.Value == 13)
                CurrentIcon_Label.Text = "Pudding";
            else if (Icon_NumericUpDown.Value == 14)
                CurrentIcon_Label.Text = "Steak";
            else if (Icon_NumericUpDown.Value == 15)
                CurrentIcon_Label.Text = "Large Shiny Glimstone";
            else if (Icon_NumericUpDown.Value == 16)
                CurrentIcon_Label.Text = "Desert Chest";
            else if (Icon_NumericUpDown.Value == 17)
                CurrentIcon_Label.Text = "Campfire";
            else if (Icon_NumericUpDown.Value == 18)
                CurrentIcon_Label.Text = "Ancient Coin";
            else if (Icon_NumericUpDown.Value == 19)
                CurrentIcon_Label.Text = "Bed";
            else if (Icon_NumericUpDown.Value == 20)
                CurrentIcon_Label.Text = "Moon Pincher";
            else if (Icon_NumericUpDown.Value == 21)
                CurrentIcon_Label.Text = "Critter Larva";
            else if (Icon_NumericUpDown.Value == 22)
                CurrentIcon_Label.Text = "Mysterious Idol";
            else if (Icon_NumericUpDown.Value == 23)
                CurrentIcon_Label.Text = "Explorer Backpack";
            else if (Icon_NumericUpDown.Value == 24)
                CurrentIcon_Label.Text = "Tombstone";
            else if (Icon_NumericUpDown.Value == 25)
                CurrentIcon_Label.Text = "Old Spore Mask";
            else if (Icon_NumericUpDown.Value == 26)
                CurrentIcon_Label.Text = "Earth Worm";
            else if (Icon_NumericUpDown.Value == 27)
                CurrentIcon_Label.Text = "Dusk Fairy";
            else if (Icon_NumericUpDown.Value == 28)
                CurrentIcon_Label.Text = "King Slime Crown";
            else if (Icon_NumericUpDown.Value == 29)
                CurrentIcon_Label.Text = "Iron Helm";
            else if (Icon_NumericUpDown.Value == 30)
                CurrentIcon_Label.Text = "Radical Rabbit Ears";
            else if (Icon_NumericUpDown.Value == 31)
                CurrentIcon_Label.Text = "Sushi";
            else if (Icon_NumericUpDown.Value == 32)
                CurrentIcon_Label.Text = "Core Figurune";
            else if (Icon_NumericUpDown.Value == 33)
                CurrentIcon_Label.Text = "Rune Parchment";
            else if (Icon_NumericUpDown.Value == 34)
                CurrentIcon_Label.Text = "Caveling Medal";
            else if (Icon_NumericUpDown.Value == 35)
                CurrentIcon_Label.Text = "Green Painting";
            else if (Icon_NumericUpDown.Value == 36)
                CurrentIcon_Label.Text = "Bubble Pearl";
            else if (Icon_NumericUpDown.Value == 37)
                CurrentIcon_Label.Text = "Oracle Card Entity";
            else if (Icon_NumericUpDown.Value == 38)
                CurrentIcon_Label.Text = "Ocean Heart Necklace";
            else if (Icon_NumericUpDown.Value == 39)
                CurrentIcon_Label.Text = "Nobile Ring";
            else if (Icon_NumericUpDown.Value > 39)
                CurrentIcon_Label.Text = "Nobile Ring";
        }
        #endregion

        // Function to load world information.
        public IEnumerable<long> AoBScanResultsWorldSeedIconMode;
        public IEnumerable<long> AoBScanResultsWorldCreationDate;
        public IEnumerable<long> AoBScanResultsWorldActivatedCrystals;
        public async Task LoadWorldInformation(string worldName = "")
        {
            // Ensure properties are filled.
            if (WorldInformation_TextBox.Text == "" && worldName == "")
            {
                // Display error message.
                MessageBox.Show("You must type the world name you wish to use!!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Offset the progress bar to show it's working.
            WorldInformation_ProgressBar.Visible = true;
            WorldInformation_ProgressBar.Maximum = 100;
            WorldInformation_ProgressBar.Step = 50;
            WorldInformation_ProgressBar.Value = 10;

            // Change button to indicate loading.
            GetWorldInformation_Button.Text = "Loading...";
            WorldInformation_GroupBox.Enabled = false;

            // Reset world properties.
            // This is so each property rescans on each new world.
            AoBScanResultsWorldData = null;
            AoBScanResultsWorldSeedIconMode = null;
            AoBScanResultsWorldCreationDate = null;
            AoBScanResultsWorldActivatedCrystals = null;

            // Clear the datagridview.
            WorldInformation_DataGridView.DefaultCellStyle.SelectionBackColor = SystemColors.Highlight;
            WorldInformation_DataGridView.DataSource = null;
            WorldInformation_DataGridView.Rows.Clear();
            WorldInformation_DataGridView.Refresh();

            // Get current world name.
            string world = (worldName != "") ? worldName : WorldInformation_TextBox.Text; // Check if world name override is active.
            string searchString = "{\"name\":\"" + world + "\"";
            StringBuilder builder = new StringBuilder();
            foreach (char c in searchString)
            {
                builder.Append(Convert.ToInt64(c).ToString("X"));
            }

            // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
            AoBScanResultsWorldData = await MemLib.AoBScan(string.Join(string.Empty, builder.ToString().Select((x, i) => i > 0 && i % 2 == 0 ? string.Format(" {0}", x) : x.ToString())), true, true);

            // If the count is zero, the scan had an error.
            if (AoBScanResultsWorldData.Count() < 1)
            {
                // Reset progress bar.
                WorldInformation_ProgressBar.Value = 0;
                WorldInformation_ProgressBar.Visible = false;

                // Rename button back to default.
                GetWorldInformation_Button.Text = "Get Information";

                // Re-enable button.
                WorldInformation_GroupBox.Enabled = true;

                // Reset aob scan results.
                AoBScanResultsWorldData = null;
                AoBScanResultsWorldSeedIconMode = null;
                AoBScanResultsWorldCreationDate = null;
                AoBScanResultsWorldActivatedCrystals = null;

                // Display error message.
                // MessageBox.Show("Unable to find the world information!!\rTry playing within the world for a few minuites.", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                WorldInformation_DataGridView.DefaultCellStyle.SelectionBackColor = Color.Red;
                WorldInformation_DataGridView.Invoke((MethodInvoker)(() => WorldInformation_DataGridView.Rows.Add("ERROR:", "No information was found!!")));
                WorldInformation_DataGridView.Invoke((MethodInvoker)(() => WorldInformation_DataGridView.Rows.Add("", "")));
                WorldInformation_DataGridView.Invoke((MethodInvoker)(() => WorldInformation_DataGridView.Rows.Add("Tips:", "1) Load the world and play for a few minutes.")));
                WorldInformation_DataGridView.Invoke((MethodInvoker)(() => WorldInformation_DataGridView.Rows.Add("", "2) Ensure the spelling of your world is correct.")));
                return;
            }

            // Update the progressbar step.
            WorldInformation_ProgressBar.Step = 100 / AoBScanResultsWorldData.Count();

            // Iterate through each found address.
            string getJsonData = "";
            bool foundData = false;
            foreach (long res in AoBScanResultsWorldData)
            {
                // Reset found json data.
                getJsonData = "";

                // Get the current base address.
                string baseJsonAddress = res.ToString("X");

                // Search result and add it to the string.
                getJsonData = MemLib.ReadString(baseJsonAddress.ToString(), length: 600); // Double length.

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
                    string name = Regex.Match(getJsonData, "\\\"name\":\"(?<Data>[^\"]+)\\\"").Groups["Data"].Value;
                    if ((getJsonData.IndexOf('}') != getJsonData.LastIndexOf('}')) && name != "")
                    {
                        // Update data found bool.
                        foundData = true;

                        // Extract the data from the string.
                        string guid = Regex.Match(getJsonData, "\\\"guid\":\"(?<Data>[^\"]*)\\\"").Groups["Data"].Value;
                        string seedString = Regex.Match(getJsonData, "\\\"seedString\":\"(?<Data>[^\"]*)\\\"").Groups["Data"].Value;                                    // New 1.0+.
                        string seed = Regex.Match(getJsonData, "\\\"seed\":(?<Data>\\d+)").Groups["Data"].Value;
                        string activatedCrystals = Regex.Match(getJsonData, "\\\"activatedCrystals\":\\[(?<Data>[0-9, ]*)\\]").Groups["Data"].Value.Trim();
                        string year = Regex.Match(getJsonData, "\\\"year\":(?<Data>\\d+)").Groups["Data"].Value;
                        string month = Regex.Match(getJsonData, "\\\"month\":(?<Data>\\d+)").Groups["Data"].Value;
                        string day = Regex.Match(getJsonData, "\\\"day\":(?<Data>\\d+)").Groups["Data"].Value;
                        string iconIndex = Regex.Match(getJsonData, "\\\"iconIndex\":(?<Data>\\d+)").Groups["Data"].Value;
                        string mode = Regex.Match(getJsonData, "\\\"mode\":(?<Data>\\d+)").Groups["Data"].Value;
                        string bossesKilled = Regex.Match(getJsonData, "\\\"bossesKilled\":(?<Data>\\d+)").Groups["Data"].Value;                                        // New 1.0+.
                        string worldGenerationType = Regex.Match(getJsonData, "\\\"worldGenerationType\":(?<Data>\\d+)").Groups["Data"].Value;                          // New 1.0+.
                        string nextNewContentBundle = Regex.Match(getJsonData, "\\\"nextNewContentBundle\":(?<Data>\\d+)").Groups["Data"].Value;                        // New 1.1+.
                        string activatedContentBundles = Regex.Match(getJsonData, "\\\"activatedContentBundles\":\\[(?<Data>[0-9, ]*)\\]").Groups["Data"].Value.Trim(); // New 1.1+.

                        // Extract world generation settings as JSON array of objects
                        MatchCollection worldGenMatches = Regex.Matches(getJsonData, "\\{\"type\":(?<Type>\\d+),\"level\":(?<Level>\\d+)\\}");                          // New 1.0+.
                        string worldGenerationSettings = "";
                        foreach (Match match in worldGenMatches)
                        {
                            // C# 8.0+: worldGenerationSettings += $"Type {match.Groups["Type"].Value} - Level {match.Groups["Level"].Value}, ";
                            worldGenerationSettings += "Type " + match.Groups["Type"].Value + " - Level " + match.Groups["Level"].Value + ", ";
                        }
                        if (worldGenerationSettings.Length > 0)
                        {
                            worldGenerationSettings = worldGenerationSettings.TrimEnd(',', ' '); // Remove trailing comma and space
                        }

                        // Add the information to the datagridview.
                        WorldInformation_DataGridView.Invoke((MethodInvoker)(() => WorldInformation_DataGridView.Rows.Add("Address:", baseJsonAddress)));
                        WorldInformation_DataGridView.Invoke((MethodInvoker)(() => WorldInformation_DataGridView.Rows.Add("Name:", name)));
                        WorldInformation_DataGridView.Invoke((MethodInvoker)(() => WorldInformation_DataGridView.Rows.Add("GUID:", string.IsNullOrEmpty(guid) ? "N/A" : guid)));
                        WorldInformation_DataGridView.Invoke((MethodInvoker)(() => WorldInformation_DataGridView.Rows.Add("seedString:", string.IsNullOrEmpty(seedString) ? "N/A" : seedString)));
                        WorldInformation_DataGridView.Invoke((MethodInvoker)(() => WorldInformation_DataGridView.Rows.Add("Seed:", seed == "0" ? "None (new world? reload!)" : seed)));
                        WorldInformation_DataGridView.Invoke((MethodInvoker)(() => WorldInformation_DataGridView.Rows.Add("Crystals:", string.IsNullOrWhiteSpace(activatedCrystals) ? "0,0,0" : activatedCrystals)));
                        WorldInformation_DataGridView.Invoke((MethodInvoker)(() => WorldInformation_DataGridView.Rows.Add("Year:", year)));
                        WorldInformation_DataGridView.Invoke((MethodInvoker)(() => WorldInformation_DataGridView.Rows.Add("Month:", CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(int.Parse(month) + 1))));
                        WorldInformation_DataGridView.Invoke((MethodInvoker)(() => WorldInformation_DataGridView.Rows.Add("Day:", day)));
                        WorldInformation_DataGridView.Invoke((MethodInvoker)(() => WorldInformation_DataGridView.Rows.Add("iconIndex:", iconIndex)));

                        // Get the mode text.
                        string modeString = "Standard";
                        if (mode == "0") modeString = "Standard";
                        else if (mode == "1") modeString = "Hard";
                        else if (mode == "2") modeString = "Creative";
                        else if (mode == "3") modeString = "Hard Creative";
                        else if (mode == "4") modeString = "Casual";
                        else if (mode == "5") modeString = "Hard Casual";
                        else if (mode == "6") modeString = "Creative Casual";
                        else if (mode == "7") modeString = "Hard Creative Casual";
                        else modeString = "Unknown";

                        // Add the information to the datagridview.
                        WorldInformation_DataGridView.Invoke((MethodInvoker)(() => WorldInformation_DataGridView.Rows.Add("Mode:", modeString)));
                        WorldInformation_DataGridView.Invoke((MethodInvoker)(() => WorldInformation_DataGridView.Rows.Add("bossesKilled:", string.IsNullOrEmpty(bossesKilled) ? "N/A" : bossesKilled)));
                        WorldInformation_DataGridView.Invoke((MethodInvoker)(() => WorldInformation_DataGridView.Rows.Add("generationType:", string.IsNullOrEmpty(worldGenerationType) ? "N/A" : worldGenerationType)));
                        WorldInformation_DataGridView.Invoke((MethodInvoker)(() => WorldInformation_DataGridView.Rows.Add("generationSettings:", string.IsNullOrEmpty(worldGenerationSettings) ? "Default" : worldGenerationSettings)));
                        WorldInformation_DataGridView.Invoke((MethodInvoker)(() => WorldInformation_DataGridView.Rows.Add("nextNewContentBundle:", string.IsNullOrEmpty(nextNewContentBundle) ? "N/A" : nextNewContentBundle)));
                        WorldInformation_DataGridView.Invoke((MethodInvoker)(() => WorldInformation_DataGridView.Rows.Add("activatedContentBundles:", string.IsNullOrEmpty(activatedContentBundles) ? "N/A" : activatedContentBundles)));

                        #region Adjust Controls

                        // Toggle controls based on world difficulty.
                        WorldDifficulty_ComboBox.SelectedIndex = int.Parse(mode);

                        // Set seed.
                        // numericUpDown22.Value = (string.IsNullOrEmpty(seed) ? (string.IsNullOrEmpty(seedString) ? 0 : int.Parse(seedString)) : (seed == "0") ? (string.IsNullOrEmpty(seedString) ? 0 : int.Parse(seedString)) : int.Parse(seed));
                        Seed_NumericUpDown.Value = uint.Parse(seed);

                        // Set icon.
                        Icon_NumericUpDown.Value = int.Parse(iconIndex);

                        // Set world creation.
                        Year_NumericUpDown.Value = int.Parse(year);
                        Month_NumericUpDown.Value = int.Parse(month) + 1; // Convert to 1-based indexing.
                        Day_NumericUpDown.Value = int.Parse(day);

                        // Set activated crystals.
                        // If not null, and desired element exists, and if ,1,2,3 text not null, use ,1,2,3, else 0.
                        CrystalOne_NumericUpDown.Value = (activatedCrystals != "") ? (activatedCrystals.Split(',').ElementAtOrDefault(0) != null) ? (activatedCrystals.Split(',')[0] != "") ? int.Parse(activatedCrystals.Split(',')[0]) : 0 : 0 : 0;
                        CrystalTwo_NumericUpDown.Value = (activatedCrystals != "") ? (activatedCrystals.Split(',').ElementAtOrDefault(1) != null) ? (activatedCrystals.Split(',')[1] != "") ? int.Parse(activatedCrystals.Split(',')[1]) : 0 : 0 : 0;
                        CrystalThree_NumericUpDown.Value = (activatedCrystals != "") ? (activatedCrystals.Split(',').ElementAtOrDefault(2) != null) ? (activatedCrystals.Split(',')[2] != "") ? int.Parse(activatedCrystals.Split(',')[2]) : 0 : 0 : 0;

                        // Deactivate controls based on activated crystals.
                        CrystalOne_NumericUpDown.Enabled = (CrystalOne_NumericUpDown.Value > 0);
                        CrystalTwo_NumericUpDown.Enabled = (CrystalTwo_NumericUpDown.Value > 0);
                        CrystalThree_NumericUpDown.Enabled = (CrystalThree_NumericUpDown.Value > 0);
                        #endregion

                        // Completed, end loop.
                        break;
                    }
                    else
                    {
                        // Unfinished, reset.
                        getJsonData = "";
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                    // Ignore the exception as it's probably just a bad address.
                    continue;
                }

                // Perform progress step.
                WorldInformation_ProgressBar.PerformStep();
            }

            // Check if any data was found, do action if not.
            if (!foundData)
            {
                WorldInformation_DataGridView.DefaultCellStyle.SelectionBackColor = Color.Red;
                WorldInformation_DataGridView.Invoke((MethodInvoker)(() => WorldInformation_DataGridView.Rows.Add("ERROR:", "No information was found!!")));
                WorldInformation_DataGridView.Invoke((MethodInvoker)(() => WorldInformation_DataGridView.Rows.Add("", "")));
                WorldInformation_DataGridView.Invoke((MethodInvoker)(() => WorldInformation_DataGridView.Rows.Add("Tips:", "1) Load the world and play for a few minutes.")));
                WorldInformation_DataGridView.Invoke((MethodInvoker)(() => WorldInformation_DataGridView.Rows.Add("", "2) Ensure the spelling of your world is correct.")));
            }

            // Process completed, run finishing tasks.
            WorldInformation_ProgressBar.Value = 100;
            WorldInformation_ProgressBar.Visible = false;

            // Rename button back to default.
            GetWorldInformation_Button.Text = "Get Information";
            WorldInformation_GroupBox.Enabled = true;
        }
        #endregion // End get world information.

        #region Change difficulty

        // Change world difficulty.
        private void ChangeDifficulty_Button_Click(object sender, EventArgs e)
        {
            // Ensure the datagridview is populated.
            if (WorldInformation_DataGridView == null || WorldInformation_DataGridView.Rows.Count == 0)
            {
                MessageBox.Show("You first need to get the world information!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsWorldData == null)
            {
                MessageBox.Show("You need to first scan for the World Information addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Change difficutky.
            ChangeWorldDifficulty();
        }

        // Change world difficulty.
        public async void ChangeWorldDifficulty(int difficulty = -1)
        {
            // Ensure the datagridview is populated.
            if (WorldInformation_DataGridView == null || WorldInformation_DataGridView.Rows.Count == 0)
            {
                MessageBox.Show("You first need to get the world information!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsWorldData == null)
            {
                MessageBox.Show("You need to first scan for the World Information addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Offset the progress bar to show it's working.
            WorldInformation_ProgressBar.Visible = true;
            WorldInformation_ProgressBar.Maximum = 100;
            WorldInformation_ProgressBar.Step = 50;
            WorldInformation_ProgressBar.Value = 10;

            // Change button to indicate loading.
            ChangeDifficulty_Button.Text = "Loading...";
            if (AoBScanResultsWorldSeedIconMode == null) // Only hide the groupbox if address is not null.
                WorldInformation_GroupBox.Enabled = false;

            // Get the seed, icon, and mode values from datagrid.
            int seedRowIndex = -1;
            DataGridViewRow seedRow = WorldInformation_DataGridView.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value.ToString().Equals("Seed:"))
                .First();
            seedRowIndex = seedRow.Index;
            int iconRowIndex = -1;
            DataGridViewRow iconRow = WorldInformation_DataGridView.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value.ToString().Equals("iconIndex:"))
                .First();
            iconRowIndex = iconRow.Index;
            int modeRowIndex = -1;
            DataGridViewRow modeRow = WorldInformation_DataGridView.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value.ToString().Equals("Mode:"))
                .First();
            modeRowIndex = modeRow.Index;

            // Define uint from world variables.
            uint worldSeed = uint.Parse(WorldInformation_DataGridView.Rows[seedRowIndex].Cells[1].Value.ToString());
            uint worldIcon = uint.Parse(WorldInformation_DataGridView.Rows[iconRowIndex].Cells[1].Value.ToString());
            uint worldMode = (uint)WorldDifficulty_ComboBox.FindString(WorldInformation_DataGridView.Rows[modeRowIndex].Cells[1].Value.ToString());

            // Convert uInt to hex 4 bytes.
            // Credits to Matthew Watson on stackoverflow: https://stackoverflow.com/a/58708490/8667430
            string result = string.Join(" ", BitConverter.GetBytes(worldSeed).Select(b => b.ToString("X2"))) + " " + string.Join(" ", BitConverter.GetBytes(worldIcon).Select(b => b.ToString("X2"))) + " " + string.Join(" ", BitConverter.GetBytes(worldMode).Select(b => b.ToString("X2")));

            // Scan for the addresses. // Only re-scan address if address is null.
            if (AoBScanResultsWorldSeedIconMode == null)
                AoBScanResultsWorldSeedIconMode = await MemLib.AoBScan(result, true, true);

            // If the count is zero, the scan had an error.
            if (AoBScanResultsWorldSeedIconMode.Count() < 1) // No results found.
            {
                // Reset progress bar.
                WorldInformation_ProgressBar.Value = 0;
                WorldInformation_ProgressBar.Visible = false;

                // Rename button back to default.
                ChangeDifficulty_Button.Text = "Change difficulty";

                // Re-enable button.
                WorldInformation_GroupBox.Enabled = true;

                // Reset aob scan results
                AoBScanResultsWorldSeedIconMode = null;

                // Display error message.
                MessageBox.Show("Unable to find the correct addresses!!/RLoad the world and play for a few minuites.", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (AoBScanResultsWorldSeedIconMode.Count() > 1 && MessageBox.Show(AoBScanResultsWorldSeedIconMode.Count().ToString() + " addresses have been found!\n\nThis typically happens when another world contains the same information.\nContinuing will overwrite both worlds' data.\n\nWould you like to proceed anyways?", "World Properties Editor", MessageBoxButtons.YesNo) == DialogResult.No) // More then one found.
            {
                // User wishes not to continue.
                // Reset progress bar.
                WorldInformation_ProgressBar.Value = 0;
                WorldInformation_ProgressBar.Visible = false;

                // Rename button back to default.
                ChangeDifficulty_Button.Text = "Change difficulty";

                // Re-enable button.
                WorldInformation_GroupBox.Enabled = true;

                // Reset aob scan results
                // AoBScanResultsWorldSeedIconMode = null;
                return;
            }

            // Update the progress bar. // Use 90 as we already progressed to 10.
            WorldInformation_ProgressBar.Step = 90 / AoBScanResultsWorldSeedIconMode.Count();

            // Iterate through each found address.
            foreach (long res in AoBScanResultsWorldSeedIconMode)
            {
                // Get address from loop.
                // Seed, icon, mode.
                string modeAddress = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");

                // Get the new mode.
                string modeType = WorldDifficulty_ComboBox.SelectedIndex.ToString();
                modeType = (difficulty == -1) ? modeType : difficulty.ToString(); // Check if mode override was selected.

                // Set the new mode value. // Convert ASCII text to hex.

                MemLib.WriteMemory(modeAddress, "int", modeType);

                // Perform progress step.
                WorldInformation_ProgressBar.PerformStep();
            }

            // Update datagridview.
            // Get the mode text.
            string modeString = "Standard";
            if (WorldDifficulty_ComboBox.SelectedIndex.ToString() == "0") modeString = "Standard";
            else if (WorldDifficulty_ComboBox.SelectedIndex.ToString() == "1") modeString = "Hard";
            else if (WorldDifficulty_ComboBox.SelectedIndex.ToString() == "2") modeString = "Creative";
            else if (WorldDifficulty_ComboBox.SelectedIndex.ToString() == "3") modeString = "Hard Creative";
            else if (WorldDifficulty_ComboBox.SelectedIndex.ToString() == "4") modeString = "Casual";
            else if (WorldDifficulty_ComboBox.SelectedIndex.ToString() == "5") modeString = "Hard Casual";
            else if (WorldDifficulty_ComboBox.SelectedIndex.ToString() == "6") modeString = "Creative Casual";
            else if (WorldDifficulty_ComboBox.SelectedIndex.ToString() == "7") modeString = "Hard Creative Casual";
            else modeString = "Unknown";

            // Change the datagridviews mode data.
            WorldInformation_DataGridView.Rows[modeRowIndex].Cells[1].Value = (difficulty == -1) ? modeString : modeString + " - " + difficulty.ToString(); // Mod string will auto detect the correct string.

            // Update the progress bar.
            WorldInformation_ProgressBar.Value = 100;
            WorldInformation_ProgressBar.Visible = false;

            // Rename button back to default.
            ChangeDifficulty_Button.Text = "Change difficulty";
            WorldInformation_GroupBox.Enabled = true;

            // Refresh address.
            // await LoadWorldInformation();
        }
        #endregion // End change world difficulty.

        #region Change Seed

        // Change world icon.
        private void ChangeSeed_Button_Click(object sender, EventArgs e)
        {
            // Ensure the datagridview is populated.
            if (WorldInformation_DataGridView == null || WorldInformation_DataGridView.Rows.Count == 0)
            {
                MessageBox.Show("You first need to get the world information!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsWorldData == null)
            {
                MessageBox.Show("You need to first scan for the World Information addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Change difficulty.
            ChangeWorldSeed();
        }

        // Change world seed.
        public async void ChangeWorldSeed(uint seed = 0)
        {
            // Ensure the datagridview is populated.
            if (WorldInformation_DataGridView == null || WorldInformation_DataGridView.Rows.Count == 0)
            {
                MessageBox.Show("You first need to get the world information!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsWorldData == null)
            {
                MessageBox.Show("You need to first scan for the World Information addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Offset the progress bar to show it's working.
            WorldInformation_ProgressBar.Visible = true;
            WorldInformation_ProgressBar.Maximum = 100;
            WorldInformation_ProgressBar.Step = 50;
            WorldInformation_ProgressBar.Value = 10;

            // Change button to indicate loading.
            ChangeSeed_Button.Text = "Loading...";
            if (AoBScanResultsWorldSeedIconMode == null) // Only hide the groupbox if address is not null.
                WorldInformation_GroupBox.Enabled = false;

            // Get the seed, icon, and mode values from datagrid.
            int seedRowIndex = -1;
            DataGridViewRow seedRow = WorldInformation_DataGridView.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value.ToString().Equals("Seed:"))
                .First();
            seedRowIndex = seedRow.Index;
            int iconRowIndex = -1;
            DataGridViewRow iconRow = WorldInformation_DataGridView.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value.ToString().Equals("iconIndex:"))
                .First();
            iconRowIndex = iconRow.Index;
            int modeRowIndex = -1;
            DataGridViewRow modeRow = WorldInformation_DataGridView.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value.ToString().Equals("Mode:"))
                .First();
            modeRowIndex = modeRow.Index;

            // Define uint from world variables.
            uint worldSeed = uint.Parse(WorldInformation_DataGridView.Rows[seedRowIndex].Cells[1].Value.ToString());
            uint worldIcon = uint.Parse(WorldInformation_DataGridView.Rows[iconRowIndex].Cells[1].Value.ToString());
            uint worldMode = (uint)WorldDifficulty_ComboBox.FindString(WorldInformation_DataGridView.Rows[modeRowIndex].Cells[1].Value.ToString());

            // Convert uInt to hex 4 bytes.
            // Credits to Matthew Watson on stackoverflow: https://stackoverflow.com/a/58708490/8667430
            string result = string.Join(" ", BitConverter.GetBytes(worldSeed).Select(b => b.ToString("X2"))) + " " + string.Join(" ", BitConverter.GetBytes(worldIcon).Select(b => b.ToString("X2"))) + " " + string.Join(" ", BitConverter.GetBytes(worldMode).Select(b => b.ToString("X2")));

            // Scan for the addresses. // Only re-scan address if address is null.
            AoBScanResultsWorldSeedIconMode ??= await MemLib.AoBScan(result, true, true);

            // If the count is zero, the scan had an error.
            if (AoBScanResultsWorldSeedIconMode.Count() < 1) // No results found.
            {
                // Reset progress bar.
                WorldInformation_ProgressBar.Value = 0;
                WorldInformation_ProgressBar.Visible = false;

                // Rename button back to default.
                ChangeSeed_Button.Text = "Change Seed";

                // Re-enable button.
                WorldInformation_GroupBox.Enabled = true;

                // Reset aob scan results
                AoBScanResultsWorldSeedIconMode = null;

                // Display error message.
                MessageBox.Show("Unable to find the correct addresses!!/RLoad the world and play for a few minuites.", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (AoBScanResultsWorldSeedIconMode.Count() > 1 && MessageBox.Show(AoBScanResultsWorldSeedIconMode.Count().ToString() + " addresses have been found!\n\nThis typically happens when another world contains the same information.\nContinuing will overwrite both worlds' data.\n\nWould you like to proceed anyways?", "World Properties Editor", MessageBoxButtons.YesNo) == DialogResult.No) // More then one found.
            {
                // User wishes not to continue.
                // Reset progress bar.
                WorldInformation_ProgressBar.Value = 0;
                WorldInformation_ProgressBar.Visible = false;

                // Rename button back to default.
                ChangeSeed_Button.Text = "Change Seed";

                // Re-enable button.
                WorldInformation_GroupBox.Enabled = true;

                // Reset aob scan results
                // AoBScanResultsWorldSeedIconMode = null;
                return;
            }

            // Update the progress bar. // Use 90 as we already progressed to 10.
            WorldInformation_ProgressBar.Step = 90 / AoBScanResultsWorldSeedIconMode.Count();

            // Iterate through each found address.
            foreach (long res in AoBScanResultsWorldSeedIconMode)
            {
                // Get address from loop.
                // Seed, icon, mode.
                string seedAddress = res.ToString("X").ToString();

                // Get the new seed.
                string seedValue = Seed_NumericUpDown.Value.ToString();
                seedValue = (seed == 0) ? seedValue : seed.ToString(); // Check if seed override was selected.

                // Set the new mode value. // Convert ASCII text to hex.

                // Memory.dll does not have a direct "uint" type. So lets use "bytes".
                // MemLib.WriteMemory(seedAddress, "int", seedValue);

                #region Write UInt Value

                // Convert the uint into bytes (little-endian):
                byte[] bytes = BitConverter.GetBytes(uint.Parse(seedValue));

                // Convert byte array to hex-string representation:
                string byteString = BitConverter.ToString(bytes).Replace("-", " ");

                // Write the bytes to memory.
                MemLib.WriteMemory(seedAddress, "bytes", byteString);

                #endregion

                // Perform progress step.
                WorldInformation_ProgressBar.PerformStep();
            }

            // Update datagridview.
            WorldInformation_DataGridView.Rows[seedRowIndex].Cells[1].Value = (seed == 0) ? Seed_NumericUpDown.Value.ToString() : seed.ToString();

            // Update the progress bar.
            WorldInformation_ProgressBar.Value = 100;
            WorldInformation_ProgressBar.Visible = false;

            // Rename button back to default.
            ChangeSeed_Button.Text = "Change Seed";
            WorldInformation_GroupBox.Enabled = true;

            // Refresh address.
            // await LoadWorldInformation();
        }
        #endregion // End change world seed.

        #region Change Icon

        // Change world icon.
        private void ChangeIcon_Button_Click(object sender, EventArgs e)
        {
            // Ensure the datagridview is populated.
            if (WorldInformation_DataGridView == null || WorldInformation_DataGridView.Rows.Count == 0)
            {
                MessageBox.Show("You first need to get the world information!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsWorldData == null)
            {
                MessageBox.Show("You need to first scan for the World Information addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Change difficulty.
            ChangeWorldIcon();
        }

        // Change world icon.
        public async void ChangeWorldIcon(int icon = -1)
        {
            // Ensure the datagridview is populated.
            if (WorldInformation_DataGridView == null || WorldInformation_DataGridView.Rows.Count == 0)
            {
                MessageBox.Show("You first need to get the world information!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsWorldData == null)
            {
                MessageBox.Show("You need to first scan for the World Information addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Offset the progress bar to show it's working.
            WorldInformation_ProgressBar.Visible = true;
            WorldInformation_ProgressBar.Maximum = 100;
            WorldInformation_ProgressBar.Step = 50;
            WorldInformation_ProgressBar.Value = 10;

            // Change button to indicate loading.
            ChangeIcon_Button.Text = "Loading...";
            if (AoBScanResultsWorldSeedIconMode == null) // Only hide the groupbox if address is not null.
                WorldInformation_GroupBox.Enabled = false;

            // Get the seed, icon, and mode values from datagrid.
            int seedRowIndex = -1;
            DataGridViewRow seedRow = WorldInformation_DataGridView.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value.ToString().Equals("Seed:"))
                .First();
            seedRowIndex = seedRow.Index;
            int iconRowIndex = -1;
            DataGridViewRow iconRow = WorldInformation_DataGridView.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value.ToString().Equals("iconIndex:"))
                .First();
            iconRowIndex = iconRow.Index;
            int modeRowIndex = -1;
            DataGridViewRow modeRow = WorldInformation_DataGridView.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value.ToString().Equals("Mode:"))
                .First();
            modeRowIndex = modeRow.Index;

            // Define uint from world variables.
            uint worldSeed = uint.Parse(WorldInformation_DataGridView.Rows[seedRowIndex].Cells[1].Value.ToString());
            uint worldIcon = uint.Parse(WorldInformation_DataGridView.Rows[iconRowIndex].Cells[1].Value.ToString());
            uint worldMode = (uint)WorldDifficulty_ComboBox.FindString(WorldInformation_DataGridView.Rows[modeRowIndex].Cells[1].Value.ToString());

            // Convert uInt to hex 4 bytes.
            // Credits to Matthew Watson on stackoverflow: https://stackoverflow.com/a/58708490/8667430
            string result = string.Join(" ", BitConverter.GetBytes(worldSeed).Select(b => b.ToString("X2"))) + " " + string.Join(" ", BitConverter.GetBytes(worldIcon).Select(b => b.ToString("X2"))) + " " + string.Join(" ", BitConverter.GetBytes(worldMode).Select(b => b.ToString("X2")));

            // Scan for the addresses. // Only re-scan address if address is null.
            if (AoBScanResultsWorldSeedIconMode == null)
                AoBScanResultsWorldSeedIconMode = await MemLib.AoBScan(result, true, true);

            // If the count is zero, the scan had an error.
            if (AoBScanResultsWorldSeedIconMode.Count() < 1) // No results found.
            {
                // Reset progress bar.
                WorldInformation_ProgressBar.Value = 0;
                WorldInformation_ProgressBar.Visible = false;

                // Rename button back to default.
                ChangeIcon_Button.Text = "Change Icon";

                // Re-enable button.
                WorldInformation_GroupBox.Enabled = true;

                // Reset aob scan results
                AoBScanResultsWorldSeedIconMode = null;

                // Display error message.
                MessageBox.Show("Unable to find the correct addresses!!/RLoad the world and play for a few minutes.", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (AoBScanResultsWorldSeedIconMode.Count() > 1 && MessageBox.Show(AoBScanResultsWorldSeedIconMode.Count().ToString() + " addresses have been found!\n\nThis typically happens when another world contains the same information.\nContinuing will overwrite both worlds' data.\n\nWould you like to proceed anyways?", "World Properties Editor", MessageBoxButtons.YesNo) == DialogResult.No) // More then one found.
            {
                // User wishes not to continue.
                // Reset progress bar.
                WorldInformation_ProgressBar.Value = 0;
                WorldInformation_ProgressBar.Visible = false;

                // Rename button back to default.
                ChangeIcon_Button.Text = "Change Icon";

                // Re-enable button.
                WorldInformation_GroupBox.Enabled = true;

                // Reset aob scan results
                // AoBScanResultsWorldSeedIconMode = null;
                return;
            }

            // Update the progress bar. // Use 90 as we already progressed to 10.
            WorldInformation_ProgressBar.Step = 90 / AoBScanResultsWorldSeedIconMode.Count();

            // Iterate through each found address.
            foreach (long res in AoBScanResultsWorldSeedIconMode)
            {
                // Get address from loop.
                // Seed, icon, mode.
                string iconAddress = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("4", NumberStyles.Integer)).ToString("X");

                // Get the new icon.
                string iconValue = Icon_NumericUpDown.Value.ToString();
                iconValue = (icon == -1) ? iconValue : icon.ToString(); // Check if seed override was selected.

                // Set the new mode value. // Convert ASCII text to hex.

                MemLib.WriteMemory(iconAddress, "int", iconValue);

                // Perform progress step.
                WorldInformation_ProgressBar.PerformStep();
            }

            // Update datagridview.
            WorldInformation_DataGridView.Rows[iconRowIndex].Cells[1].Value = (icon == -1) ? Icon_NumericUpDown.Value.ToString() : icon.ToString();

            // Update the progress bar.
            WorldInformation_ProgressBar.Value = 100;
            WorldInformation_ProgressBar.Visible = false;

            // Rename button back to default.
            ChangeIcon_Button.Text = "Change Icon";
            WorldInformation_GroupBox.Enabled = true;

            // Refresh address.
            // await LoadWorldInformation();
        }
        #endregion // End change world icon.

        #region Change Creation Date

        // Change world date.
        private void ChangeDate_Button_Click(object sender, EventArgs e)
        {
            // Ensure the datagridview is populated.
            if (WorldInformation_DataGridView == null || WorldInformation_DataGridView.Rows.Count == 0)
            {
                MessageBox.Show("You first need to get the world information!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsWorldData == null)
            {
                MessageBox.Show("You need to first scan for the World Information addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Change difficulty.
            ChangeWorldCreationDate();
        }

        // Change world date.
        public async void ChangeWorldCreationDate(int year = -1, int month = -1, int day = -1)
        {
            // Ensure the datagridview is populated.
            if (WorldInformation_DataGridView == null || WorldInformation_DataGridView.Rows.Count == 0)
            {
                MessageBox.Show("You first need to get the world information!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsWorldData == null)
            {
                MessageBox.Show("You need to first scan for the World Information addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Offset the progress bar to show it's working.
            WorldInformation_ProgressBar.Visible = true;
            WorldInformation_ProgressBar.Maximum = 100;
            WorldInformation_ProgressBar.Step = 50;
            WorldInformation_ProgressBar.Value = 10;

            // Change button to indicate loading.
            ChangeDate_Button.Text = "Loading...";
            if (AoBScanResultsWorldCreationDate == null) // Only hide the groupbox if address is not null.
                WorldInformation_GroupBox.Enabled = false;

            // Get the seed, icon, and mode values from datagrid.
            int yearRowIndex = -1;
            DataGridViewRow yearRow = WorldInformation_DataGridView.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value.ToString().Equals("Year:"))
                .First();
            yearRowIndex = yearRow.Index;
            int monthRowIndex = -1;
            DataGridViewRow monthRow = WorldInformation_DataGridView.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value.ToString().Equals("Month:"))
                .First();
            monthRowIndex = monthRow.Index;
            int dayRowIndex = -1;
            DataGridViewRow dayRow = WorldInformation_DataGridView.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value.ToString().Equals("Day:"))
                .First();
            dayRowIndex = dayRow.Index;

            // Define uint from world variables.
            uint worldYear = uint.Parse(WorldInformation_DataGridView.Rows[yearRowIndex].Cells[1].Value.ToString());
            uint worldMonth = (uint)DateTime.ParseExact(WorldInformation_DataGridView.Rows[monthRowIndex].Cells[1].Value.ToString(), "MMMM", CultureInfo.CurrentCulture).Month - 1; // Convert to 0-based indexing.
            uint worldDay = uint.Parse(WorldInformation_DataGridView.Rows[dayRowIndex].Cells[1].Value.ToString());

            // Convert uInt to hex 4 bytes.
            // Credits to Matthew Watson on stackoverflow: https://stackoverflow.com/a/58708490/8667430
            string result = string.Join(" ", BitConverter.GetBytes(worldYear).Select(b => b.ToString("X2"))) + " " + string.Join(" ", BitConverter.GetBytes(worldMonth).Select(b => b.ToString("X2"))) + " " + string.Join(" ", BitConverter.GetBytes(worldDay).Select(b => b.ToString("X2")));

            // Scan for the addresses. // Only re-scan address if address is null.
            if (AoBScanResultsWorldCreationDate == null)
                AoBScanResultsWorldCreationDate = await MemLib.AoBScan(result, true, true);

            // If the count is zero, the scan had an error.
            if (AoBScanResultsWorldCreationDate.Count() < 1) // No results found.
            {
                // Reset progress bar.
                WorldInformation_ProgressBar.Value = 0;
                WorldInformation_ProgressBar.Visible = false;

                // Rename button back to default.
                ChangeDate_Button.Text = "Change Date";

                // Re-enable button.
                WorldInformation_GroupBox.Enabled = true;

                // Reset aob scan results
                AoBScanResultsWorldCreationDate = null;

                // Display error message.
                MessageBox.Show("Unable to find the correct addresses!!\n\nLoad the world and play for a few minutes.", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (AoBScanResultsWorldCreationDate.Count() > 1 && MessageBox.Show(AoBScanResultsWorldCreationDate.Count().ToString() + " addresses have been found!\n\nThis typically happens when another world contains the same information.\nContinuing will overwrite both worlds' data.\n\nWould you like to proceed anyways?", "World Properties Editor", MessageBoxButtons.YesNo) == DialogResult.No) // More then one found.
            {
                // User wishes not to continue.
                // Reset progress bar.
                WorldInformation_ProgressBar.Value = 0;
                WorldInformation_ProgressBar.Visible = false;

                // Rename button back to default.
                ChangeDate_Button.Text = "Change Date";

                // Re-enable button.
                WorldInformation_GroupBox.Enabled = true;

                // Reset aob scan results
                // AoBScanResultsWorldCreationDate = null;
                return;
            }

            // Update the progress bar. // Use 90 as we already progressed to 10.
            WorldInformation_ProgressBar.Step = 90 / AoBScanResultsWorldCreationDate.Count();

            // Iterate through each found address.
            foreach (long res in AoBScanResultsWorldCreationDate)
            {
                // Get address from loop.
                // Year, month, day.
                string yearAddress = res.ToString("X");
                string monthAddress = BigInteger.Add(BigInteger.Parse(res.ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("4", NumberStyles.Integer)).ToString("X");
                string dayAddress = BigInteger.Add(BigInteger.Parse(res.ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");

                // Get the new times.
                string yearValue = Year_NumericUpDown.Value.ToString();
                yearValue = (year == -1) ? yearValue : year.ToString();           // Check if seed override was selected.
                string monthValue = (Month_NumericUpDown.Value - 1).ToString();        // Convert to 0-based indexing.
                monthValue = (month == -1) ? monthValue : (month - 1).ToString(); // Check if seed override was selected.
                string dayValue = Day_NumericUpDown.Value.ToString();
                dayValue = (day == -1) ? dayValue : day.ToString();               // Check if seed override was selected.

                // Set the new mode value. // Convert ASCII text to hex.

                MemLib.WriteMemory(yearAddress, "int", yearValue); // Write year address.
                MemLib.WriteMemory(monthAddress, "int", monthValue); // Write month address.
                MemLib.WriteMemory(dayAddress, "int", dayValue); // Write day address.

                // Perform progress step.
                WorldInformation_ProgressBar.PerformStep();
            }

            // Update datagridview.
            WorldInformation_DataGridView.Rows[yearRow.Index].Cells[1].Value = (year == -1) ? Year_NumericUpDown.Value.ToString() : year.ToString();
            WorldInformation_DataGridView.Rows[monthRow.Index].Cells[1].Value = (month == -1) ? CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(int.Parse(Month_NumericUpDown.Value.ToString())) : CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month); // NA: Convert to 1-based indexing.
            WorldInformation_DataGridView.Rows[dayRow.Index].Cells[1].Value = (day == -1) ? Day_NumericUpDown.Value.ToString() : day.ToString();

            // Update the progress bar.
            WorldInformation_ProgressBar.Value = 100;
            WorldInformation_ProgressBar.Visible = false;

            // Rename button back to default.
            ChangeDate_Button.Text = "Change Date";
            WorldInformation_GroupBox.Enabled = true;

            // Refresh address.
            // await LoadWorldInformation();
        }
        #endregion // End world creation date.

        #region Change Activated Crystals

        // Change activated crystals.
        private void ChangeCrystals_Button_Click(object sender, EventArgs e)
        {
            // Ensure the datagridview is populated.
            if (WorldInformation_DataGridView == null || WorldInformation_DataGridView.Rows.Count == 0)
            {
                MessageBox.Show("You first need to get the world information!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsWorldData == null)
            {
                MessageBox.Show("You need to first scan for the World Information addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Change difficutky.
            ChangeWorldActivatedCrystals();
        }

        // Change world date.
        public async void ChangeWorldActivatedCrystals(int crystal1 = -1, int crystal2 = -1, int crystal3 = -1)
        {
            // Ensure the datagridview is populated.
            if (WorldInformation_DataGridView == null || WorldInformation_DataGridView.Rows.Count == 0)
            {
                MessageBox.Show("You first need to get the world information!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsWorldData == null)
            {
                MessageBox.Show("You need to first scan for the World Information addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Offset the progress bar to show it's working.
            WorldInformation_ProgressBar.Visible = true;
            WorldInformation_ProgressBar.Maximum = 100;
            WorldInformation_ProgressBar.Step = 50;
            WorldInformation_ProgressBar.Value = 10;

            // Change button to indicate loading.
            ChangeCrystals_Button.Text = "Loading...";
            if (AoBScanResultsWorldActivatedCrystals == null) // Only hide the groupbox if address is not null.
                WorldInformation_GroupBox.Enabled = false;

            // Get the crystal values from datagrid.
            int crystalRowIndex = -1;
            DataGridViewRow crystalOneRow = WorldInformation_DataGridView.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value.ToString().Equals("Crystals:"))
                .First();
            string crystalOne = string.Join(" ", BitConverter.GetBytes(uint.Parse((WorldInformation_DataGridView.Rows[crystalOneRow.Index].Cells[1].Value.ToString().Split(',').ElementAtOrDefault(0) != null) ? (WorldInformation_DataGridView.Rows[crystalOneRow.Index].Cells[1].Value.ToString().Split(',')[0] != "") ? WorldInformation_DataGridView.Rows[crystalOneRow.Index].Cells[1].Value.ToString().Split(',')[0] : "0" : "0")).Select(b => b.ToString("X2")));
            DataGridViewRow crystalTwoRow = WorldInformation_DataGridView.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value.ToString().Equals("Crystals:"))
                .First();
            string crystalTwo = string.Join(" ", BitConverter.GetBytes(uint.Parse((WorldInformation_DataGridView.Rows[crystalTwoRow.Index].Cells[1].Value.ToString().Split(',').ElementAtOrDefault(1) != null) ? (WorldInformation_DataGridView.Rows[crystalTwoRow.Index].Cells[1].Value.ToString().Split(',')[1] != "") ? WorldInformation_DataGridView.Rows[crystalTwoRow.Index].Cells[1].Value.ToString().Split(',')[1] : "0" : "0")).Select(b => b.ToString("X2")));
            DataGridViewRow crystalThreeRow = WorldInformation_DataGridView.Rows
                .Cast<DataGridViewRow>()
                .Where(r => r.Cells[0].Value.ToString().Equals("Crystals:"))
                .First();
            string crystalThree = string.Join(" ", BitConverter.GetBytes(uint.Parse((WorldInformation_DataGridView.Rows[crystalThreeRow.Index].Cells[1].Value.ToString().Split(',').ElementAtOrDefault(2) != null) ? (WorldInformation_DataGridView.Rows[crystalThreeRow.Index].Cells[1].Value.ToString().Split(',')[2] != "") ? WorldInformation_DataGridView.Rows[crystalThreeRow.Index].Cells[1].Value.ToString().Split(',')[2] : "0" : "0")).Select(b => b.ToString("X2")));

            // Save the row index.
            crystalRowIndex = crystalOneRow.Index;

            // If byte returned a zero change it to a wildcard.
            crystalOne = (crystalOne == "00 00 00 00") ? "?? ?? ?? ??" : crystalOne;
            crystalTwo = (crystalTwo == "00 00 00 00") ? "?? ?? ?? ??" : crystalTwo;
            crystalThree = (crystalThree == "00 00 00 00") ? "?? ?? ?? ??" : crystalThree;

            // Convert uInt to hex 4 bytes.
            // Credits to Matthew Watson on stackoverflow: https://stackoverflow.com/a/58708490/8667430
            string result = crystalOne + " " + crystalTwo + " " + crystalThree + " ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00";

            // Scan for the addresses. // Only re-scan address if address is null.
            if (AoBScanResultsWorldActivatedCrystals == null)
                AoBScanResultsWorldActivatedCrystals = await MemLib.AoBScan(result, true, true);

            // If the count is zero, the scan had an error.
            if (AoBScanResultsWorldActivatedCrystals.Count() < 1) // No results found.
            {
                // Reset progress bar.
                WorldInformation_ProgressBar.Value = 0;
                WorldInformation_ProgressBar.Visible = false;

                // Rename button back to default.
                ChangeCrystals_Button.Text = "Change Crystals";

                // Re-enable button.
                WorldInformation_GroupBox.Enabled = true;

                // Reset aob scan results
                AoBScanResultsWorldActivatedCrystals = null;

                // Display error message.
                MessageBox.Show("Unable to find the correct addresses!!/RLoad the world and play for a few minutes.", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (AoBScanResultsWorldActivatedCrystals.Count() > 1 && MessageBox.Show(AoBScanResultsWorldActivatedCrystals.Count().ToString() + " addresses have been found!\n\nThis typically happens when another world contains the same information.\nContinuing will overwrite both worlds' data.\n\nWould you like to proceed anyways?", "World Properties Editor", MessageBoxButtons.YesNo) == DialogResult.No) // More then one found.
            {
                // User wishes not to continue.
                // Reset progress bar.
                WorldInformation_ProgressBar.Value = 0;
                WorldInformation_ProgressBar.Visible = false;

                // Rename button back to default.
                ChangeCrystals_Button.Text = "Change Crystals";

                // Re-enable button.
                WorldInformation_GroupBox.Enabled = true;

                // Reset aob scan results
                // AoBScanResultsWorldActivatedCrystals = null;
                return;
            }

            // Update the progress bar. // Use 90 as we already progressed to 10.
            WorldInformation_ProgressBar.Step = 90 / AoBScanResultsWorldActivatedCrystals.Count();

            // Iterate through each found address.
            foreach (long res in AoBScanResultsWorldActivatedCrystals)
            {
                // Get address from loop.
                // Crystal1, crystal2, crystal3, ?, ?, 0, 0.
                string crystalOneAddress = res.ToString("X");
                string crystalTwoAddress = BigInteger.Add(BigInteger.Parse(res.ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("4", NumberStyles.Integer)).ToString("X");
                string crystalThreeAddress = BigInteger.Add(BigInteger.Parse(res.ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");

                // Get the new crystals.
                string crystalOneValue = CrystalOne_NumericUpDown.Value.ToString();
                crystalOneValue = (crystal1 == -1) ? crystalOneValue : crystal1.ToString(); // Check if seed override was selected.
                string crystalTwoValue = CrystalTwo_NumericUpDown.Value.ToString();
                crystalTwoValue = (crystal2 == -1) ? crystalTwoValue : crystal2.ToString(); // Check if seed override was selected.
                string crystalThreeValue = CrystalThree_NumericUpDown.Value.ToString();
                crystalThreeValue = (crystal3 == -1) ? crystalThreeValue : crystal3.ToString(); // Check if seed override was selected.

                // Set the new mode value. // Convert ASCII text to hex.

                MemLib.WriteMemory(crystalOneAddress, "int", crystalOneValue); // Write year address.
                MemLib.WriteMemory(crystalTwoAddress, "int", crystalTwoValue); // Write month address.
                MemLib.WriteMemory(crystalThreeAddress, "int", crystalThreeValue); // Write day address.

                // Perform progress step.
                WorldInformation_ProgressBar.PerformStep();
            }

            // Update datagridview.
            string crystalPartOne = (crystal1 == -1) ? CrystalOne_NumericUpDown.Value.ToString() : crystal1.ToString();
            string crystalPartTwo = (crystal1 == -1) ? CrystalTwo_NumericUpDown.Value.ToString() : crystal2.ToString();
            string crystalPartThree = (crystal1 == -1) ? CrystalThree_NumericUpDown.Value.ToString() : crystal3.ToString();

            // Build the active crystal string for the datagridview.
            string crystalBuilder = "";
            if (crystalPartOne != "0") crystalBuilder += crystalPartOne;
            else crystalBuilder = "0,0,0";
            if (crystalPartTwo != "0") crystalBuilder += "," + crystalPartTwo;
            if (crystalPartThree != "0") crystalBuilder += "," + crystalPartThree;

            // Update the activate crystals text within the datagridview.
            WorldInformation_DataGridView.Rows[crystalRowIndex].Cells[1].Value = crystalBuilder;

            // Update the progress bar.
            WorldInformation_ProgressBar.Value = 100;
            WorldInformation_ProgressBar.Visible = false;

            // Rename button back to default.
            ChangeCrystals_Button.Text = "Change Crystals";
            WorldInformation_GroupBox.Enabled = true;

            // Refresh address.
            // await LoadWorldInformation();
        }
        #endregion // End world activated crystals.

        #region Change Console ForeColor

        // Change console fore color.
        private void ChangeConsoleForeColor_Button_Click(object sender, EventArgs e)
        {
            // Update button text.
            ChangeConsoleForeColor_Button.Text = "Pick New Color";

            ColorDialog clrDialog = new ColorDialog();

            // Show the color dialog and check that user clicked ok.
            if (clrDialog.ShowDialog() == DialogResult.OK)
            {
                // Save the color that the user chose.
                Color consoleColor = clrDialog.Color;

                // Update the color for future rows.
                WorldInformation_DataGridView.RowsDefaultCellStyle.ForeColor = consoleColor;
                WorldInformation_DataGridView.AlternatingRowsDefaultCellStyle.ForeColor = consoleColor;

                // Update the color of the color indicator button.
                ColorSample_Button.ForeColor = consoleColor;
                ColorSample_Button.BackColor = consoleColor;

                // Update the color of existing rows.
                foreach (DataGridViewRow row in WorldInformation_DataGridView.Rows)
                {
                    row.DefaultCellStyle.ForeColor = consoleColor;
                }

                // Save the color for future loads.
                Settings.Default.ConsoleForeColor = consoleColor;

                // Update the tooltip.
                ToolTip toolTip = new ToolTip()
                {
                    AutoPopDelay = 5000,
                    InitialDelay = 750
                };
                toolTip.SetToolTip(ChangeConsoleForeColor_Button, "Change the world property editors console color.\nCurrent Color: " + Settings.Default.ConsoleForeColor.Name.ToString());
            }

            // Update button text.
            ChangeConsoleForeColor_Button.Text = "Change Console ForeColor";
        }
        #endregion // End change console fore color.

        #region Auto Fishing Bot

        #region Auto Fishing Bot Offsets

        // Suppress CS0414 (field is assigned but its value is never used).
        // Suppressing since these are modding offsets meant for future use or documentation.
        #pragma warning disable CS0414

        // Below contains all the offsets for the auto fishing bot.
        // These values are all added to the auto fishing bots base address.
                                                    // Base + Offset.
        readonly string baseFishing_Offset = "0";   // Base address.
        readonly string fishType_Offset    = "872"; // ID of the fish currently on the line. Will be '0' with no fish on the line.
        readonly string fishFight_Offset   = "N/A"; // FISH MINI-GAME: Fish is currently fighting, do not pull in line during this.
                                                    // As of version 0.9.9.9, they removed the fighting minigame.

        #pragma warning restore CS0414

        #endregion

        // Toggle auto fishing.
        readonly System.Timers.Timer autoFishingTimer = new System.Timers.Timer();
        string baseFishing_Address  = "0";
        string fishType_Address     = "0";
        // string fishFight_Address = "0";
        bool autoFishingChecked = false;
        private async void AutomaticFishing_Button_Click(object sender, EventArgs e)
        {
            // Check if button was toggled.
            if (!autoFishingChecked)
            {
                // Open the process and check if it was successful before the AoB scan.
                if (!MemLib.OpenProcess("CoreKeeper"))
                {
                    MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Toggle fishing bool.
                autoFishingChecked = true;

                // Name button to indicate loading.
                AutomaticFishing_Button.Text = "Loading...";
                AutomaticFishing_Button.Enabled = true;

                // Disable button to prevent spamming.
                // button11.Enabled = false;
                WorldInformation_GroupBox.Enabled = false;

                // Reset textbox.
                TeleportPlayerAddresses_RichTextBox.Text = "Addresses Loaded: 0";

                // Offset the progress bar to show it's working.
                TeleportPlayer_ProgressBar.Visible = true;
                TeleportPlayer_ProgressBar.Maximum = 100;
                TeleportPlayer_ProgressBar.Step = 100;
                TeleportPlayer_ProgressBar.Value = 10;

                // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
                // Depreciated Address 08Feb23: 3? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 01 00 00 00 ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? 01 00 00 00
                // Depreciated Address 18May23: 3? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 01 00 00 00 ?? ?? ?? ?? FF FF FF FF
                // Depreciated Address 19Jan24: 3? 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 01 00 00 00 ?? ?? ?? ?? FF FF FF FF 00 08 00 00 00 00 00 00 FF FF FF FF
                // Depreciated Address 27Apr24: 3? 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? 09 00 00 00 04 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00
                // Depreciated Address 23Aug24: 3? 00 00 00 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? 09 00 00 00 04 00 00 00 00 00 00 00 00 00 00 00
                // Depreciated Address 10Feb25: 9D 9C 64 3E CD CC 4C BE ?? ?? ?? ?? 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 92 9D 52 8D
                // Depreciated Address 29May25: CD CC 4C BE ?? ?? ?? ?? 00 00 00 00 00 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 0A 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 80 3F ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 0? 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 3? 00 00 00
                AoBScanResultsFishingData = await MemLib.AoBScan("3? 00 00 00 3? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? 43 00 00 01 00 00 00 00 00 00 00", true, true);

                // If the count is zero, the scan had an error.
                if (AoBScanResultsFishingData.Count() < 1)
                {
                    // Reset textbox.
                    TeleportPlayerAddresses_RichTextBox.Text = "Addresses Loaded: 0";

                    // Reset progress bar.
                    TeleportPlayer_ProgressBar.Value = 0;
                    TeleportPlayer_ProgressBar.Visible = false;

                    // Rename button back to default.
                    AutomaticFishing_Button.Text = "Automatic Fishing";

                    // Re-enable button.
                    AutomaticFishing_Button.Enabled = true;
                    WorldInformation_GroupBox.Enabled = true;

                    // Toggle fishing bool.
                    autoFishingChecked = false;

                    // Reset aob scan results
                    AoBScanResultsFishingData = null;

                    // Display error message.
                    MessageBox.Show("Try throwing a fishing rod's line into water first!!\rTIP: This can be of any variation.", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Update richtextbox with found addresses.
                foreach (long res in AoBScanResultsFishingData)
                {
                    if (TeleportPlayerAddresses_RichTextBox.Text == "Addresses Loaded: 0")
                    {
                        TeleportPlayerAddresses_RichTextBox.Text = "Fishing Addresses Loaded: " + AoBScanResultsFishingData.Count().ToString() + " [" + res.ToString("X").ToString();
                    }
                    else
                    {
                        TeleportPlayerAddresses_RichTextBox.Text += ", " + res.ToString("X").ToString();
                    }
                }
                TeleportPlayerAddresses_RichTextBox.Text += "]";

                // Reset progress bar.
                TeleportPlayer_ProgressBar.Value = 0;
                TeleportPlayer_ProgressBar.Visible = false;

                // Get the addresses.
                baseFishing_Address = AoBScanResultsFishingData.Last().ToString("X"); // Rod type. // Always use the last address!
                fishType_Address = BigInteger.Subtract(BigInteger.Parse(baseFishing_Address, NumberStyles.HexNumber), BigInteger.Parse(fishType_Offset, NumberStyles.Integer)).ToString("X");
                // fishType_Address = BigInteger.Add(BigInteger.Parse(baseFishingAddress, NumberStyles.HexNumber), BigInteger.Parse("784", NumberStyles.Integer)).ToString("X");
                // fishFight_Address = BigInteger.Add(BigInteger.Parse(baseFishingAddress, NumberStyles.HexNumber), BigInteger.Parse("804", NumberStyles.Integer)).ToString("X");

                // Rename button back to default.
                AutomaticFishing_Button.Text = "Disable Fishing Bot";

                // Re-enable button.
                AutomaticFishing_Button.Enabled = true;
                WorldInformation_GroupBox.Enabled = true;

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

                // Rename button back to default.
                AutomaticFishing_Button.Text = "Automatic Fishing";

                // Toggle bool.
                autoFishingChecked = false;
            }
        }

        // Auto fishing timer.
        int fishType = 0;
        readonly bool fishFighting = false; // Now readonly g.v: 0.9.9.9
        bool reelingActive = false;
        private async void AutoFishingTimedEvent(Object source, ElapsedEventArgs e)
        {
            // Fetch current addresses.
            fishType = MemLib.ReadInt(fishType_Address);
            // fishFighting = (MemLib.ReadInt(fishFightAddress) == 1);

            // Fish is on the hook, attempt to reel in. //
            // Check if a fish was caught.
            if (fishType != 0)
            {
                // Check if fish is currently fighting.
                if (!fishFighting)
                {
                    // Add some cast delay.
                    await Task.Delay((int)CastDelay_NumericUpDown.Value);

                    // Pull fish in.
                    mouse_event(MOUSEEVENT_RIGHTDOWN, 0, 0, 0, 0);
                    mouse_event(MOUSEEVENT_RIGHTUP, 0, 0, 0, 0);

                    // Activate bool.
                    reelingActive = true;
                }
                else
                {
                    // As of version 0.9.9.9, they removed the fighting minigame.
                    // This function while still here, should never be used.
                    //
                    // Do nothing.
                    return;
                }
            }

            // Throw rod back into the water. //
            // Check if reeling.
            else if (reelingActive && fishType == 0)
            {
                // Add some cast delay.
                await Task.Delay((int)CastDelay_NumericUpDown.Value);

                // Caught finished.
                mouse_event(MOUSEEVENT_RIGHTDOWN, 0, 0, 0, 0);
                mouse_event(MOUSEEVENT_RIGHTUP, 0, 0, 0, 0);

                // Reset bool.
                reelingActive = false;
            }

            // Add some padding delay.
            await Task.Delay((int)FishingPadding_NumericUpDown.Value);
        }
        #endregion // End auto fishing bot.

        #region Random Teleport

        // Toggle random teleport.
        readonly System.Timers.Timer randomTeleportTimer = new System.Timers.Timer();
        Vector2 initialRTPosition = new Vector2(0, 0);
        bool randomTeleportChecked = false;
        private void RandomTeleport_Button_Click(object sender, EventArgs e)
        {
            // Check if button was toggled.
            if (!randomTeleportChecked)
            {
                // Open the process and check if it was successful before the AoB scan.
                if (!MemLib.OpenProcess("CoreKeeper"))
                {
                    MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Ensure pointers are found.
                if (AoBScanResultsPlayerTools == null)
                {
                    MessageBox.Show("You need to first scan for the Player addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Ensure pointers are found.
                if (AoBScanResultsPlayerLocation == null)
                {
                    MessageBox.Show("You need to first scan for the Teleport Player addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Toggle random teleport bool.
                randomTeleportChecked = true;

                // Name button to indicate loading.
                RandomTeleport_Button.Text = "Disable Random TP";
                RandomTeleport_Button.Enabled = true;

                // Define players initial position.
                var initialres = AoBScanResultsPlayerTools.Last();
                float xPos = MemLib.ReadFloat(BigInteger.Add(BigInteger.Parse(initialres.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse(positionX_Offset, NumberStyles.Integer)).ToString("X"));
                float yPos = MemLib.ReadFloat(BigInteger.Add(BigInteger.Parse(initialres.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse(positionY_Offset, NumberStyles.Integer)).ToString("X"));
                initialRTPosition = new Vector2(xPos, yPos);

                // Slider is being toggled on.
                // Start the timed events.
                randomTeleportTimer.Interval = 1; // Custom intervals.
                randomTeleportTimer.Elapsed += new ElapsedEventHandler(RandomTeleportTimedEvent);
                randomTeleportTimer.Start();
            }
            else
            {
                // Slider is being toggled off.
                // Stop the timers.
                randomTeleportTimer.Stop();

                // Rename button back to default.
                RandomTeleport_Button.Text = "Random Teleport";

                // Toggle bool.
                randomTeleportChecked = false;
            }
        }

        private bool _isTeleporting = false;
        private async void RandomTeleportTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (_isTeleporting) return; // Exit if an event is already being processed.
            _isTeleporting = true;

            // Define the radius size to get the random points within.
            int radius_size = (int)RTRange_NumericUpDown.Value;

            // Generate random positions within the given radius size.
            // Notice the call uses (radius_size * 2) + 1 to include the upper bound.
            float RandomX = (float)(GenerateRandomNumber(0, (radius_size * 2) + 1) - radius_size) + (float)initialRTPosition.X;
            float RandomY = (float)(GenerateRandomNumber(0, (radius_size * 2) + 1) - radius_size) + (float)initialRTPosition.Y;

            // Iterate through each found address.
            foreach (long res in AoBScanResultsPlayerLocation)
            {
                // Get address from loop.
                string playerX = res.ToString("X").ToString();
                string playerY = BigInteger.Add(BigInteger.Parse(res.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");

                // Send player to X.
                MemLib.WriteMemory(playerX, "float", RandomX.ToString());

                // Send player to Y.
                MemLib.WriteMemory(playerY, "float", RandomY.ToString());
            }

            // Add delay before re-teleporting the player.
            int decimalToMillisecond = (int)(RTDelay_NumericUpDown.Value * 1000);
            await Task.Delay(decimalToMillisecond);

            _isTeleporting = false;
        }
        #endregion

        #endregion // End player tools.

        #region Chat Tab

        #region Toggle Controls

        // Enable the numericupdown based on if selected radiobutton is checked.
        private void Custom_RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (Custom_RadioButton.Checked == true)
            {
                // Disable
                CustomAmount_NumericUpDown.Enabled = true;
            }
            else
            {
                // Enable
                CustomAmount_NumericUpDown.Enabled = false;
            }
        }
        #endregion

        #region Available Commands List

        public class CommandReader
        {
            #region Future Code Implementations

            // .NET 4.7.2 does not support tuple array declarations
            /*
            private static readonly (string command, string description)[] commands = new (string, string)[]
            {
                // ~~Inventory~~
                ("item [type] (amount) (variation)",    "Give the player an item."),

                // ~~Player~~
                ("godmode",                             "Toggle invincibility."),
                ("noclip",                              "Toggle no clipping."),
                ("tp [x] [y]",                          "Teleport to a location."),

                // ~~World~~
                ("clearground",                         "Remove ground items."),
                ("mode [worldName] [difficulty]",       "Change the world difficulty."),

                // ~~Misc~~
                ("help (page)",                         "Display a list of commands."),
                ("cls",                                 "Clear the console.")
            };
            */
            #endregion

            private static readonly Tuple<string, string>[] commands = new Tuple<string, string>[]
            {
                // ~~Inventory~~
                Tuple.Create("item [type] (amount) (variation)",    "Give the player an item."),

                // ~~Player~~
                Tuple.Create("godmode",                             "Toggle invincibility."),
                Tuple.Create("noclip",                              "Toggle no clipping."),
                Tuple.Create("tp [x] [y]",                          "Teleport to a location."),

                // ~~World~~
                Tuple.Create("clearground",                         "Remove ground items."),
                Tuple.Create("mode [worldName] [difficulty]",       "Change the world difficulty."),

                // ~~Misc~~
                Tuple.Create("help (page)",                         "Display a list of commands."),
                Tuple.Create("cls",                                 "Clear the console.")
             };

            #region Formatting Logic

            private static string FormatCommand(string command, string description, int maxLength)
            {
                int spacesToAdd = maxLength - command.Length;

                int offset = (spacesToAdd != 0) ? -1 : 0; // Apply a -1 offset if the spaces to add is not zero.
                return command + new string(' ', spacesToAdd + offset) + "\t - " + description;
            }

            public static string ReadCommands(int maxLines, int page, bool ingameText = false)
            {
                int totalPages = (int)Math.Ceiling((double)commands.Length / maxLines);
                if (page < 1 || page > totalPages)
                {
                    return "Invalid page number. Total pages: " + totalPages;
                }

                int maxCommandLength = commands.Max(cmd => cmd.Item1.Length);

                StringBuilder sb = new StringBuilder();

                if (ingameText)
                    sb.AppendLine("=============================================");
                else
                    sb.AppendLine("========================================================");
                sb.AppendLine("[] ---- Required Argument");
                sb.AppendLine("() ---- Optional Argument");
                sb.AppendLine("==============CONSOLE COMMANDS [PAGE " + page.ToString("D2") + "]==============");
                sb.AppendLine("");

                int startIndex = (page - 1) * maxLines;
                int endIndex = Math.Min(startIndex + maxLines, commands.Length);

                for (int i = startIndex; i < endIndex; i++)
                {
                    // Get the format based on where to display.
                    if (!ingameText)
                    {
                        // Format for console.

                        // Get string for richtextbox.
                        sb.AppendLine(FormatCommand(commands[i].Item1, commands[i].Item2, maxCommandLength));
                    }
                    else
                    {
                        // Format for ingame.

                        // Get string for richtextbox.
                        sb.AppendLine(commands[i].Item1 + "\t - " + commands[i].Item2);
                    }
                }

                // Finish the ingame builder.
                if (ingameText)
                {
                    sb.AppendLine("");
                    sb.AppendLine("Use '/help " + (page + 1) + "' for more commands.");
                }

                return sb.ToString();
            }
            #endregion
        }
        #endregion

        #region Toggle Chat Commands

        // Toggle chat commands.
        bool chatEnabled = false;
        public System.Timers.Timer chatTimer = new System.Timers.Timer(500);
        private async void EnableChatCommands_Button_Click(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Check if the chat button is enabled or disabled.
            if (chatEnabled)
            {
                // Disable / reset some controls.
                chatEnabled = false;
                EnableChatCommands_Button.Text = "Enable";
                EnableChatCommands_Button.Enabled = true;
                ChatCommands_ProgressBar.Value = 0;
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
                ChatCommands_ProgressBar.Value = 10;
                EnableChatCommands_Button.Text = "Loading..";
                EnableChatCommands_Button.Enabled = false;

                // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
                // 2F 69 74 65 6D = /item
                // 2F 63 68 61 74 = /chat
                AoBScanResultsChat = await MemLib.AoBScan("2F 63 68 61 74", true, true);

                // If the count is zero, the scan had an error.
                if (AoBScanResultsChat.Count() == 0 | AoBScanResultsChat.Count() < 1)
                {
                    // Disable / reset some controls.
                    chatEnabled = false;
                    EnableChatCommands_Button.Text = "Enable";
                    EnableChatCommands_Button.Enabled = true;
                    ChatCommands_ProgressBar.Value = 0;
                    chatTimer.Stop();
                    chatTimer.Enabled = false;
                    // groupBox6.Enabled = true;

                    // Display error message.
                    MessageBox.Show("You must type \"/chat\" in the player chat first!!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Name button to indicate loading finished.
                EnableChatCommands_Button.Text = "Disable";
                EnableChatCommands_Button.Enabled = true;

                // Reset richtextbox.
                string stringBuilder = "Welcome to the chat commands! Available CMDS are below.\n\n" +
                    CommandReader.ReadCommands(15, 1) +
                    "-----------------------------------------------------------------------------------------------------------------\n";

                ChatCommands_RichTextBox.Text = stringBuilder;
                ChatCommands_RichTextBox.AppendText("Any captured chat messages will appear here.\n" +
                    "-----------------------------------------------------------------------------------------------------------------\n");
                ChatCommands_RichTextBox.ScrollToCaret();

                // Advance progress bar.
                ChatCommands_ProgressBar.Value = 100;

                // Enable a timer.
                chatTimer.AutoReset = true;
                chatTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
                chatTimer.Start();
            }
        }
        #endregion // End toggle chat commands.

        #region Chat Events

        // Do events for the chat.
        bool firstRun = true;        // Do text reset bool.
        bool firstItem = true;       // Ensure we only add to one slot.
        bool godmodeEnabled = false; // Holder for godmode toggle.
        bool noclipEnabled = false;  // Holder for noclip toggle.
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
                /* More coming soon! */
                try
                {
                    // ~~Inventory~~
                    //
                    #region Give Item

                    // Check if current value is valid command and it's unique.
                    if (currentCommand.Split(' ')[0] == "/item")
                    {
                        // Erase current chat values.
                        MemLib.WriteMemory(baseAddress, "string", new string(' ', 100));

                        // Make a list of each command part.
                        string[] cmdParts = currentCommand.Split(' ');

                        // Item name, amount, and variation (from the command)
                        string itemName = cmdParts.Length > 1 ? cmdParts[1] : "";                                                  // Keep blank to indicate null items.
                        string itemAmount = cmdParts.Length > 2 && !string.IsNullOrWhiteSpace(cmdParts[2]) ? cmdParts[2] : "1";    // Ensure the default is 1.
                        string itemVariation = cmdParts.Length > 3 && !string.IsNullOrWhiteSpace(cmdParts[3]) ? cmdParts[3] : "0"; // Ensure the default is 0.

                        // Do first run.
                        // INFO: Loop runs multiple times, run once via checking cmd execution in log.
                        // Ensure the previous ran command was not the same. (prevents duplicate runs per address)
                        if (!(currentCommand == ChatCommands_RichTextBox.Lines[ChatCommands_RichTextBox.Lines.Length - 1]
                             || currentCommand == ChatCommands_RichTextBox.Lines[ChatCommands_RichTextBox.Lines.Length - 2]
                             || ChatCommands_RichTextBox.Lines[ChatCommands_RichTextBox.Lines.Length - 1] == "ERROR: You need to first scan for the Inventory addresses!"
                             || ChatCommands_RichTextBox.Lines[ChatCommands_RichTextBox.Lines.Length - 2] == "ERROR: You need to first scan for the Inventory addresses!") && firstRun)
                        {
                            // Prevent further entries this loop.
                            firstRun = false;

                            // Display the chat command.
                            if (itemName != "")
                            {
                                // Display message.
                                RichTextBoxHelper.AppendUniqueText(ChatCommands_RichTextBox, "[GiveItem] " + currentCommand, UseOverlay_CheckBox.Checked);
                                ChatCommands_RichTextBox.ScrollToCaret();
                            }
                            else
                            {
                                // End loop.
                                break;
                            }

                            // Ensure pointers are found.
                            if (AoBScanResultsInventory == null)
                            {
                                // Display message.
                                RichTextBoxHelper.AppendUniqueText(ChatCommands_RichTextBox, "ERROR: You need to first scan for the Inventory addresses!", UseOverlay_CheckBox.Checked);
                                ChatCommands_RichTextBox.ScrollToCaret();
                                break;
                            }

                            try
                            {
                                // With item variation.
                                // itemVariation = currentCommand.Split(' ')[3] != "" ? currentCommand.Split(' ')[3] : "0"; // Ensure the default is 0.

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
                                            if (filenameData[0].ToLower().Contains(itemName.Replace(" ", "")) || filenameData[1] == itemName.Replace(" ", "")) // Name or ID.
                                            {
                                                // Check if to overwrite or to add to empty slots.
                                                if (OverwriteSlotOne_RadioButton.Checked) // Overwrite slot1.
                                                {
                                                    AddItemToInv(ItemSlot: 1, Type: int.Parse(filenameData[1]), Amount: int.Parse(itemAmount), Variation: int.Parse(itemVariation) == 0 ? 0 : (int.Parse(itemVariation)), Overwrite: true);
                                                }
                                                else if (AddToEmptySlots_RadioButton.Checked) // Add item to an empty slot.
                                                {
                                                    // Reload inventory if add to empty is checked.
                                                    if (AddToEmptySlots_RadioButton.Checked && firstItem)
                                                    {
                                                        // Mark item as first.
                                                        firstItem = false;

                                                        AddItemToInv(AddToEmpty: true, Type: int.Parse(filenameData[1]), Amount: int.Parse(itemAmount), Variation: int.Parse(itemVariation) == 0 ? 0 : (int.Parse(itemVariation)), Overwrite: true);
                                                    }
                                                }
                                                else if (Custom_RadioButton.Checked) // Custom slot.
                                                {
                                                    AddItemToInv(ItemSlot: (int)CustomAmount_NumericUpDown.Value, Type: int.Parse(filenameData[1]), Amount: int.Parse(itemAmount), Variation: int.Parse(itemVariation) == 0 ? 0 : (int.Parse(itemVariation)), Overwrite: true);
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
                                            // Get file information.
                                            var fi = new FileInfo(file);
                                            string[] filenameData = fi.Name.Split(',');

                                            // Catch desktop.ini from throwing errors.
                                            if (filenameData[0] == "desktop.ini") continue;

                                            // Get all matches.
                                            if (filenameData[0].ToLower().Contains(itemName.Replace(" ", "")) || filenameData[1] == itemName.Replace(" ", "")) // Name or ID.
                                            {
                                                // Check if to overwrite or to add to empty slots.
                                                if (OverwriteSlotOne_RadioButton.Checked) // Overwrite slot1.
                                                {
                                                    AddItemToInv(ItemSlot: 1, Type: int.Parse(filenameData[1]), Amount: int.Parse(itemAmount), Overwrite: true);
                                                }
                                                else if (AddToEmptySlots_RadioButton.Checked) // Add item to an empty slot.
                                                {
                                                    // Reload inventory if add to empty is checked.
                                                    if (AddToEmptySlots_RadioButton.Checked && firstItem)
                                                    {
                                                        // Mark item as first.
                                                        firstItem = false;

                                                        AddItemToInv(AddToEmpty: true, Type: int.Parse(filenameData[1]), Amount: int.Parse(itemAmount), Overwrite: true);
                                                    }
                                                }
                                                else if (Custom_RadioButton.Checked) // Custom slot.
                                                {
                                                    AddItemToInv(ItemSlot: (int)CustomAmount_NumericUpDown.Value, Type: int.Parse(filenameData[1]), Amount: int.Parse(itemAmount), Overwrite: true);
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


                    // ~~Player~~
                    //
                    #region Godmode

                    if (currentCommand.Split(' ')[0] == "/godmode")
                    {
                        // Erase current chat values.
                        MemLib.WriteMemory(baseAddress, "string", new string(' ', 100));

                        // Log command if it does not exist.
                        // INFO: Loop runs multiple times, run once via checking cmd execution in log.
                        if (!(ChatCommands_RichTextBox.Lines[ChatCommands_RichTextBox.Lines.Length - 1].Split(' ')[0] == "[Godmode]"
                            || ChatCommands_RichTextBox.Lines[ChatCommands_RichTextBox.Lines.Length - 2].Split(' ')[0] == "[Godmode]") && firstRun)
                        {
                            // Prevent further entries this loop.
                            firstRun = false;

                            // Make a list of each command part.
                            // string[] cmdParts = currentCommand.Split(' ');

                            // Get strings.
                            // string worldName = cmdParts.Length > 1 ? cmdParts[1] : "";                                       // Keep blank to indicate null name.
                            // string mode = cmdParts.Length > 2 && !string.IsNullOrWhiteSpace(cmdParts[2]) ? cmdParts[2] : ""; // Keep blank to indicate null mode.

                            // Open the process and check if it was successful before the AoB scan.
                            if (!MemLib.OpenProcess("CoreKeeper"))
                            {
                                // Log events.
                                RichTextBoxHelper.AppendUniqueText(ChatCommands_RichTextBox, "[Godmode] ERROR: Process Is Not Found or Open!", UseOverlay_CheckBox.Checked);
                                ChatCommands_RichTextBox.ScrollToCaret();
                                break;
                            }

                            // Ensure pointers are found.
                            if (AoBScanResultsPlayerTools == null)
                            {
                                // Display message.
                                RichTextBoxHelper.AppendUniqueText(ChatCommands_RichTextBox, "ERROR: You need to first scan for the Player addresses!", UseOverlay_CheckBox.Checked);
                                ChatCommands_RichTextBox.ScrollToCaret();
                                break;
                            }

                            // Toggle the godmode bool.
                            godmodeEnabled = !godmodeEnabled;

                            // Get the addresses.
                            godmodeAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.First().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse(godmode_Offset, NumberStyles.Integer)).ToString("X");

                            // Check if the slider was not yet checked.
                            if (godmodeEnabled)
                            {
                                // Toggle slider.
                                Godmode_ToggleSwitch.CheckedChanged -= Godmode_ToggleSwitch_CheckedChanged;
                                Godmode_ToggleSwitch.Checked = true;
                                Godmode_ToggleSwitch.CheckedChanged += Godmode_ToggleSwitch_CheckedChanged;

                                // Slider is being toggled on.
                                // Start the timed events.
                                playersGodmodeTimer.Interval = 1; // Custom intervals.
                                playersGodmodeTimer.Elapsed += new ElapsedEventHandler(PlayersGodmodeTimedEvent);
                                playersGodmodeTimer.Start();

                                // Display message to ingame console before clearing.
                                RichTextBoxHelper.AppendUniqueText(ChatCommands_RichTextBox, "[Godmode] Enabled!", UseOverlay_CheckBox.Checked);
                                ChatCommands_RichTextBox.ScrollToCaret();

                                // End chat command.
                                break;
                            }
                            else
                            {
                                // Toggle slider.
                                Godmode_ToggleSwitch.CheckedChanged -= Godmode_ToggleSwitch_CheckedChanged;
                                Godmode_ToggleSwitch.Checked = false;
                                Godmode_ToggleSwitch.CheckedChanged += Godmode_ToggleSwitch_CheckedChanged;

                                // Slider is being toggled off.
                                // Stop the timers.
                                playersGodmodeTimer.Stop();

                                // Display message to ingame console before clearing.
                                RichTextBoxHelper.AppendUniqueText(ChatCommands_RichTextBox, "[Godmode] Disabled!", UseOverlay_CheckBox.Checked);
                                ChatCommands_RichTextBox.ScrollToCaret();

                                // End chat command.
                                break;
                            }
                        }

                        // End chat command.
                        break;
                    }
                    #endregion // End godmode.

                    #region Noclip

                    if (currentCommand.Split(' ')[0] == "/noclip")
                    {
                        // Erase current chat values.
                        MemLib.WriteMemory(baseAddress, "string", new string(' ', 100));

                        // Log command if it does not exist.
                        // INFO: Loop runs multiple times, run once via checking cmd execution in log.
                        if (!(ChatCommands_RichTextBox.Lines[ChatCommands_RichTextBox.Lines.Length - 1].Split(' ')[0] == "[Noclip]"
                            || ChatCommands_RichTextBox.Lines[ChatCommands_RichTextBox.Lines.Length - 2].Split(' ')[0] == "[Noclip]") && firstRun)
                        {
                            // Prevent further entries this loop.
                            firstRun = false;

                            // Make a list of each command part.
                            // string[] cmdParts = currentCommand.Split(' ');

                            // Get strings.
                            // string worldName = cmdParts.Length > 1 ? cmdParts[1] : "";                                       // Keep blank to indicate null name.
                            // string mode = cmdParts.Length > 2 && !string.IsNullOrWhiteSpace(cmdParts[2]) ? cmdParts[2] : ""; // Keep blank to indicate null mode.

                            // Open the process and check if it was successful before the AoB scan.
                            if (!MemLib.OpenProcess("CoreKeeper"))
                            {
                                // Log events.
                                RichTextBoxHelper.AppendUniqueText(ChatCommands_RichTextBox, "[Noclip] ERROR: Process Is Not Found or Open!", UseOverlay_CheckBox.Checked);
                                ChatCommands_RichTextBox.ScrollToCaret();
                                break;
                            }

                            // Ensure pointers are found.
                            if (AoBScanResultsPlayerTools == null)
                            {
                                // Display message.
                                RichTextBoxHelper.AppendUniqueText(ChatCommands_RichTextBox, "ERROR: You need to first scan for the Player addresses!", UseOverlay_CheckBox.Checked);
                                ChatCommands_RichTextBox.ScrollToCaret();
                                break;
                            }

                            // Toggle the godmode bool.
                            noclipEnabled = !noclipEnabled;

                            // Get the addresses.
                            // Old alternative address: 124 // Fix 1.3.4.6 15Jan23. // Reverted 1.3.4.9 09Feb23 - old address: 116.
                            // POINTER MAP: 03 ?? (02) - Use the second one.
                            noclipAddress = BigInteger.Add(BigInteger.Parse(AoBScanResultsPlayerTools.First().ToString("X"), NumberStyles.HexNumber), BigInteger.Parse(noclip_Offset, NumberStyles.Integer)).ToString("X");

                            // Check if the slider was not yet checked.
                            if (noclipEnabled)
                            {
                                // Toggle slider.
                                Noclip_ToggleSwitch.CheckedChanged -= Noclip_ToggleSwitch_CheckedChanged;
                                Noclip_ToggleSwitch.Checked = true;
                                Noclip_ToggleSwitch.CheckedChanged += Noclip_ToggleSwitch_CheckedChanged;

                                // Toggle force on checkbox.
                                ForceNoclip_Checkbox.Checked = true;

                                // Slider is being toggled on.
                                // Get original value.
                                noclipOriginalValue = MemLib.ReadUInt(noclipAddress).ToString();

                                // Start the timed events.
                                playersNoclipTimer.Interval = 100;
                                playersNoclipTimer.Elapsed += new ElapsedEventHandler(PlayersNoclipTimedEvent);
                                playersNoclipTimer.Start();

                                // Display message to ingame console before clearing.
                                RichTextBoxHelper.AppendUniqueText(ChatCommands_RichTextBox, "[Noclip] Enabled!", UseOverlay_CheckBox.Checked);
                                ChatCommands_RichTextBox.ScrollToCaret();

                                // End chat command.
                                break;
                            }
                            else
                            {
                                // Toggle slider.
                                Noclip_ToggleSwitch.CheckedChanged -= Noclip_ToggleSwitch_CheckedChanged;
                                Noclip_ToggleSwitch.Checked = false;
                                Noclip_ToggleSwitch.CheckedChanged += Noclip_ToggleSwitch_CheckedChanged;

                                // Toggle force on checkbox.
                                ForceNoclip_Checkbox.Checked = false;

                                // Slider is being toggled off.
                                // Stop the timers.
                                playersNoclipTimer.Stop();

                                // Write value.
                                MemLib.WriteMemory(noclipAddress, "int", noclipOriginalValue); // Overwrite new value.

                                // Display message to ingame console before clearing.
                                RichTextBoxHelper.AppendUniqueText(ChatCommands_RichTextBox, "[Noclip] Disabled!", UseOverlay_CheckBox.Checked);
                                ChatCommands_RichTextBox.ScrollToCaret();

                                // End chat command.
                                break;
                            }
                        }

                        // End chat command.
                        break;
                    }
                    #endregion // End noclip.

                    #region Teleport Player

                    if (currentCommand.Split(' ')[0] == "/tp")
                    {
                        // Erase current chat values.
                        MemLib.WriteMemory(baseAddress, "string", new string(' ', 100));

                        // Log command if it does not exist.
                        // INFO: Loop runs multiple times, run once via checking cmd execution in log.
                        if (!(ChatCommands_RichTextBox.Lines[ChatCommands_RichTextBox.Lines.Length - 1].Split(' ')[0] == "[TP]"
                            || ChatCommands_RichTextBox.Lines[ChatCommands_RichTextBox.Lines.Length - 2].Split(' ')[0] == "[TP]") && firstRun)
                        {
                            // Prevent further entries this loop.
                            firstRun = false;

                            // Make a list of each command part.
                            string[] cmdParts = currentCommand.Split(' ');

                            // Get strings.
                            string xLocation = cmdParts.Length > 1 ? cmdParts[1] : "";                                            // Keep blank to indicate null name.
                            string yLocation = cmdParts.Length > 2 && !string.IsNullOrWhiteSpace(cmdParts[2]) ? cmdParts[2] : ""; // Keep blank to indicate null mode.

                            // Ensure command is fully populated.
                            if (xLocation == "" || yLocation == "")
                            {
                                // Log events.
                                RichTextBoxHelper.AppendUniqueText(ChatCommands_RichTextBox, "[TP] CMD ERROR: You need to specify both X & Y coordinates!", UseOverlay_CheckBox.Checked);
                                ChatCommands_RichTextBox.ScrollToCaret();
                                break;
                            }

                            // Open the process and check if it was successful before the AoB scan.
                            if (!MemLib.OpenProcess("CoreKeeper"))
                            {
                                // Log events.
                                RichTextBoxHelper.AppendUniqueText(ChatCommands_RichTextBox, "[TP] ERROR: Process Is Not Found or Open!", UseOverlay_CheckBox.Checked);
                                ChatCommands_RichTextBox.ScrollToCaret();
                                break;
                            }

                            // Ensure pointers are found.
                            if (AoBScanResultsPlayerLocation == null)
                            {
                                // Display message.
                                RichTextBoxHelper.AppendUniqueText(ChatCommands_RichTextBox, "ERROR: You need to first scan for the Teleport Player addresses!", UseOverlay_CheckBox.Checked);
                                ChatCommands_RichTextBox.ScrollToCaret();
                                break;
                            }

                            // Iterate through each found address.
                            foreach (long res2 in AoBScanResultsPlayerLocation)
                            {
                                // Get address from loop.
                                string playerX = res2.ToString("X").ToString();
                                string playerY = BigInteger.Add(BigInteger.Parse(res2.ToString("X").ToString(), NumberStyles.HexNumber), BigInteger.Parse("8", NumberStyles.Integer)).ToString("X");

                                // Send player to X.
                                MemLib.WriteMemory(playerX, "float", xLocation);

                                // Send player to Y.
                                MemLib.WriteMemory(playerY, "float", yLocation);
                            }

                            // Display message to ingame console before clearing.
                            RichTextBoxHelper.AppendUniqueText(ChatCommands_RichTextBox, "[TP] Teleported Player to: {X: " + xLocation + ", Y: " + yLocation + "}.", UseOverlay_CheckBox.Checked);
                            ChatCommands_RichTextBox.ScrollToCaret();

                            // End chat command.
                            break;
                        }

                        // End chat command.
                        break;
                    }
                    #endregion // End noclip.


                    // ~~World~~
                    //
                    #region Clear Ground Items

                    if (currentCommand.Split(' ')[0] == "/clearground")
                    {
                        // Erase current chat values.
                        MemLib.WriteMemory(baseAddress, "string", new string(' ', 100));

                        // Log command if it does not exist.
                        // INFO: Loop runs multiple times, run once via checking cmd execution in log.
                        if (!(ChatCommands_RichTextBox.Lines[ChatCommands_RichTextBox.Lines.Length - 1].Split(' ')[0] == "[ClearGround]"
                            || ChatCommands_RichTextBox.Lines[ChatCommands_RichTextBox.Lines.Length - 2].Split(' ')[0] == "[ClearGround]") && firstRun)
                        {
                            // Prevent further entries this loop.
                            firstRun = false;

                            ChatCommands_RichTextBox.AppendText(currentCommand + " - Loading please wait.." + Environment.NewLine);
                            ChatCommands_RichTextBox.ScrollToCaret();

                            // Check if brute force mode is enabled.
                            // AoB scan and store it in AoBScanResults. We specify our start and end address regions to decrease scan time.
                            if (BruteForceTrash_CheckBox.Checked)
                            {
                                // Do brute force scanning.
                                AoBScanResultsGroundItems = await MemLib.AoBScan("01 00 00 00 01 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 01 00 00 00", true, true);
                            }
                            else
                            {
                                // Do normal scanning.
                                // Depreciated Address 23Oct23: 6E 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 01 00 00 00 ?? ?? ?? ?? 01 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 01 00 00 00 01 00 00 00
                                // Depreciated Address 04May24: 01 00 00 00 01 00 00 00 6E 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ??
                                // Depreciated Address 23Feb25: 01 00 00 00 01 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 01 00 00 00 01 00 00 00 6E 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                                AoBScanResultsGroundItems = await MemLib.AoBScan("01 00 00 00 01 00 00 00 6E 00 00 00 ?? ?? ?? ?? 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00", true, true);
                            }

                            // If the count is zero, the scan had an error.
                            if (AoBScanResultsGroundItems.Count() == 0)
                            {
                                // Display error message.
                                if (BruteForceTrash_CheckBox.Checked)
                                    RichTextBoxHelper.AppendUniqueText(ChatCommands_RichTextBox, "[ClearGround_BruteForce] Failed to find any addresses. No ground items found!!", UseOverlay_CheckBox.Checked);
                                else
                                    RichTextBoxHelper.AppendUniqueText(ChatCommands_RichTextBox, "[ClearGround] You must throw at least one torch on the ground!!", UseOverlay_CheckBox.Checked);
                                ChatCommands_RichTextBox.ScrollToCaret();
                                break;
                            }

                            // Remove ground items.
                            await RemoveGroundItemsAsync();

                            // Log events.
                            RichTextBoxHelper.AppendUniqueText(ChatCommands_RichTextBox, "[ClearGround] Ground items cleared!", UseOverlay_CheckBox.Checked);
                            ChatCommands_RichTextBox.ScrollToCaret();

                            // End chat command.
                            break;
                        }

                        // End chat command.
                        break;
                    }
                    #endregion // End clear ground items.

                    #region Change World Difficulty

                    if (currentCommand.Split(' ')[0] == "/mode")
                    {
                        // Erase current chat values.
                        MemLib.WriteMemory(baseAddress, "string", new string(' ', 100));

                        // Log command if it does not exist.
                        // INFO: Loop runs multiple times, run once via checking cmd execution in log.
                        if (!(ChatCommands_RichTextBox.Lines[ChatCommands_RichTextBox.Lines.Length - 1].Split(' ')[0] == "[WorldMode]"
                            || ChatCommands_RichTextBox.Lines[ChatCommands_RichTextBox.Lines.Length - 2].Split(' ')[0] == "[WorldMode]") && firstRun)
                        {
                            // Prevent further entries this loop.
                            firstRun = false;

                            // Make a list of each command part.
                            string[] cmdParts = currentCommand.Split(' ');

                            // Get strings.
                            string worldName = cmdParts.Length > 1 ? cmdParts[1] : "";                                       // Keep blank to indicate null name.
                            string mode = cmdParts.Length > 2 && !string.IsNullOrWhiteSpace(cmdParts[2]) ? cmdParts[2] : ""; // Keep blank to indicate null mode.

                            // Ensure command is fully populated.
                            if (worldName == "" || mode == "")
                            {
                                // Log events.
                                RichTextBoxHelper.AppendUniqueText(ChatCommands_RichTextBox, "[WorldMode] CMD ERROR: /mode [worldName] [difficulty = 'normal' or 'hard']", UseOverlay_CheckBox.Checked);
                                ChatCommands_RichTextBox.ScrollToCaret();
                                break;
                            }

                            // Open the process and check if it was successful before the AoB scan.
                            if (!MemLib.OpenProcess("CoreKeeper"))
                            {
                                // Log events.
                                RichTextBoxHelper.AppendUniqueText(ChatCommands_RichTextBox, "[WorldMode] ERROR: Process Is Not Found or Open!", UseOverlay_CheckBox.Checked);
                                ChatCommands_RichTextBox.ScrollToCaret();
                                break;
                            }

                            // Ensure pointers are found.
                            if (AoBScanResultsWorldData == null)
                            {
                                // Display message.
                                RichTextBoxHelper.AppendUniqueText(ChatCommands_RichTextBox, "ERROR: You need to first scan for the World Information addresses!", UseOverlay_CheckBox.Checked);
                                ChatCommands_RichTextBox.ScrollToCaret();
                                break;
                            }

                            // Load world information.
                            Task.Run(async () => { await LoadWorldInformation(worldName); }).Wait();

                            // Change world difficulty.
                            if (mode.ToLower() == "normal")
                            {
                                // Change world difficulty to normal.
                                ChangeWorldDifficulty(0);

                                // Log events.
                                RichTextBoxHelper.AppendUniqueText(ChatCommands_RichTextBox, "[WorldMode] Difficulty set to normal!", UseOverlay_CheckBox.Checked);
                                ChatCommands_RichTextBox.ScrollToCaret();
                            }
                            else if (mode.ToLower() == "hard")
                            {
                                // Change world difficulty to hard.
                                ChangeWorldDifficulty(1);

                                // Log events.
                                RichTextBoxHelper.AppendUniqueText(ChatCommands_RichTextBox, "[WorldMode] Difficulty set to hard!", UseOverlay_CheckBox.Checked);
                                ChatCommands_RichTextBox.ScrollToCaret();
                            }
                        }

                        // End chat command.
                        break;
                    }
                    #endregion // End change world difficulty.


                    // ~~Misc~~
                    //
                    #region Get Help

                    if (currentCommand.Split(' ')[0] == "/help")
                    {
                        // Erase current chat values.
                        // "                                "
                        MemLib.WriteMemory(baseAddress, "string", new string(' ', 100));

                        // Log command if it does not exist.
                        // INFO: Loop runs multiple times, run once via checking cmd execution in log.
                        if (!(ChatCommands_RichTextBox.Lines[ChatCommands_RichTextBox.Lines.Length - 1].Split(' ')[0] == "[Help]"
                            || ChatCommands_RichTextBox.Lines[ChatCommands_RichTextBox.Lines.Length - 2].Split(' ')[0] == "[Help]") && firstRun)
                        {
                            // Prevent further entries this loop.
                            firstRun = false;

                            // Display message to ingame console before clearing.
                            RichTextBoxHelper.AppendUniqueText(ChatCommands_RichTextBox, "[Help] Help command was issued!", false);
                            ChatCommands_RichTextBox.ScrollToCaret();

                            // Handle overlay differently.
                            if (UseOverlay_CheckBox.Checked)
                            {
                                // Get page number. - ensure a valid suffix exists or just use the first page.
                                // Make a list of each command part.
                                string[] cmdParts = currentCommand.Split(' ');

                                int testValue;
                                int pageNumber = (cmdParts.Length > 1 && !string.IsNullOrWhiteSpace(cmdParts[1]))
                                    ? (int.TryParse(cmdParts[1], out testValue) ? testValue : 1)
                                    : 1;

                                OverlayHelper.ShowOverlay(CommandReader.ReadCommands(5, pageNumber, true), 10); // Use longer display time.
                            }

                            // End chat command.
                            break;
                        }

                        // End chat command.
                        break;
                    }
                    #endregion // End clear CMD.

                    #region Clear CMD

                    if (currentCommand.Split(' ')[0] == "/cls")
                    {
                        // Erase current chat values.
                        MemLib.WriteMemory(baseAddress, "string", new string(' ', 100));

                        // Log command if it does not exist.
                        // INFO: Loop runs multiple times, run once via checking cmd execution in log.
                        if (!(ChatCommands_RichTextBox.Lines[ChatCommands_RichTextBox.Lines.Length - 1].Split(' ')[0] == "[ClearConsole]"
                            || ChatCommands_RichTextBox.Lines[ChatCommands_RichTextBox.Lines.Length - 2].Split(' ')[0] == "[ClearConsole]") && firstRun)
                        {
                            // Prevent further entries this loop.
                            firstRun = false;

                            // Display message to ingame console before clearing.
                            RichTextBoxHelper.AppendUniqueText(ChatCommands_RichTextBox, "[ClearConsole] Console log was cleared!", UseOverlay_CheckBox.Checked);
                            ChatCommands_RichTextBox.ScrollToCaret();

                            // Reset richtextbox.
                            ChatCommands_RichTextBox.Text = "Any captured chat messages will appear here.\n" +
                                "-----------------------------------------------------------------------------------------------------------------\n";
                        }

                        // End chat command.
                        break;
                    }
                    #endregion // End clear CMD.


                    // CMD not found.
                    //
                    #region Command Not Found

                    // Check if this is the initial launch message or not.
                    if (currentCommand.Split(' ')[0] == "/chat"
                        || currentCommand.StartsWith("/chat")) // Fix for loops. - EX: Finds /chat in "'/chatX3 [".
                    {
                        // Erase current chat values.
                        MemLib.WriteMemory(baseAddress, "string", new string(' ', 100));

                        // End loop.
                        break;
                    }
                    else if (currentCommand.Split(' ')[0].Contains('/'))
                    {
                        // Unknown command.

                        // Erase current chat values.
                        MemLib.WriteMemory(baseAddress, "string", new string(' ', 100));

                        // Display error message.
                        RichTextBoxHelper.AppendUniqueText(ChatCommands_RichTextBox, "'" + currentCommand.Split(' ')[0] + "' is not recognized as an command.", UseOverlay_CheckBox.Checked);
                        ChatCommands_RichTextBox.ScrollToCaret();

                        // End loop.
                        break;
                    }
                    #endregion
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

        #region Append Unique Text Helper

        public static class RichTextBoxHelper
        {
            private static string _lastMessage = string.Empty;

            public static void AppendUniqueText(RichTextBox richTextBox, string message, bool useOverlay)
            {
                if (_lastMessage != message)
                {
                    // Display console in-game.
                    if (useOverlay)
                    {
                        OverlayHelper.ShowOverlay(message, 5);
                    }

                    richTextBox.AppendText(message + Environment.NewLine);
                    _lastMessage = message;
                }
            }
        }
        #endregion

        #endregion // End toggle chat commands

        #region Admin Tools

        #region Clear Logs

        // Clear the debug log.
        private void ClearDebugLog_Button_Click(object sender, EventArgs e)
        {
            // Set the debug logs content.
            Debug_RichTextBox.Text = String.Concat(new string[] {
                @"If any unknown items are found, their ID's will appear here!" + Environment.NewLine,
                @"------------------------------------------------------------------------------------------------------------"
            });
        }

        // Clear the world tools log.
        private void ClearWorldToolsLog_Button_Click(object sender, EventArgs e)
        {
            // Set the debug logs content.
            WorldTools_RichTextBox.Text = String.Concat(new string[] {
                @"Information regarding the world tools will appear here." + Environment.NewLine,
                @"------------------------------------------------------------------------------------------------------------"
            });
        }
        #endregion // End clear logs.

        #region Item ID List Builder

        // Create an ID list from all installed assets.
        private void DevTools3_Label_Click(object sender, EventArgs e)
        {
            // Recolor label.
            DevTools3_Label.ForeColor = Color.Lime;

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
                    DevTools3_Label.ForeColor = Color.Red;

                    MessageBox.Show("No assets found within the inventory directory.", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                DevTools3_Label.ForeColor = Color.Red;

                // Display error.
                MessageBox.Show(a.Message.ToString(), errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Recolor label.
            DevTools3_Label.ForeColor = Color.Red;
        }
        #endregion // End upgrade legacy items.

        #region Random Food ID

        // Get a random food ID.
        private void DevTools2_Label_Click(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Ensure pointers are found.
            if (AoBScanResultsInventory == null)
            {
                MessageBox.Show("You need to first scan for the Inventory addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Recolor label.
            DevTools2_Label.ForeColor = Color.Lime;

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
                DevTools2_Label.ForeColor = Color.Red;

                MessageBox.Show(@"The file '\assets\debug\data.txt' does not exist!" + Environment.NewLine + "Create one with random IDs per line.", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            DevTools2_Label.ForeColor = Color.Lime;
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
        private void DevTools1_Label_Click(object sender, EventArgs e)
        {
            // Check if item swap is active or not.
            if (!itemSwapActive)
            {
                // Open the process and check if it was successful before the AoB scan.
                if (!MemLib.OpenProcess("CoreKeeper"))
                {
                    MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Ensure pointers are found.
                if (AoBScanResultsInventory == null)
                {
                    MessageBox.Show("You need to first scan for the Inventory addresses!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Show message box.
                MessageBox.Show("Key Mapping:" + Environment.NewLine + Environment.NewLine + "Left & Right Arrow Keys: +/- ID" + Environment.NewLine + "Up & Down Arrow Keys: +/- Variation" + Environment.NewLine + "Add & Subtract Buttons: +/- Amount", "INFO", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Recolor label.
                DevTools1_Label.ForeColor = Color.Lime;

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
                itemSwapTimer.Interval = (double)DevToolsDelay_NumericUpDown.Value; // Custom intervals.
                itemSwapTimer.Elapsed += new ElapsedEventHandler(ItemSwapTimedEvent);
                itemSwapTimer.Start();
            }
            else
            {
                // Stop the timer.
                itemSwapTimer.Stop();

                // Recolor label.
                DevTools1_Label.ForeColor = Color.Red;

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
                AddItemToInv(ItemSlot: 2, Type: currentSwapItem -= 1, Amount: currentSwapAmount, Variation: currentSwapVariation, Overwrite: true);
            }
            else if (IsKeyPressed(0x27)) // Get right arrow press; add id.
            {
                AddItemToInv(ItemSlot: 2, Type: currentSwapItem += 1, Amount: currentSwapAmount, Variation: currentSwapVariation, Overwrite: true);
            }
            else if (IsKeyPressed(0x26)) // Get up arrow press; subtract variant.
            {
                AddItemToInv(ItemSlot: 2, Type: currentSwapItem, Amount: currentSwapAmount, Variation: currentSwapVariation -= 1, Overwrite: true);
            }
            else if (IsKeyPressed(0x28)) // Get down arrow press; add variant.
            {
                AddItemToInv(ItemSlot: 2, Type: currentSwapItem, Amount: currentSwapAmount, Variation: currentSwapVariation += 1, Overwrite: true);
            }
            else if (IsKeyPressed(0xBB)) // Get plus button; add amount.
            {
                AddItemToInv(ItemSlot: 2, Type: currentSwapItem, Amount: currentSwapAmount += 1, Variation: currentSwapVariation, Overwrite: true);
            }
            else if (IsKeyPressed(0xBD)) // Get minus button; subtract amount.
            {
                AddItemToInv(ItemSlot: 2, Type: currentSwapItem, Amount: currentSwapAmount -= 1, Variation: currentSwapVariation, Overwrite: true);
            }
        }
        #endregion // End quick edit slot2.

        #region Recenter Game

        // Bring game window back to the center and resize.
        private void DevTools5_Label_Click(object sender, EventArgs e)
        {
            // Open the process and check if it was successful before the AoB scan.
            if (!MemLib.OpenProcess("CoreKeeper"))
            {
                MessageBox.Show("Process Is Not Found or Open!", errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        private void DevTools4_Label_Click(object sender, EventArgs e)
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
                DevTools4_Label.ForeColor = Color.Lime;

                // Enable bool.
                memoryLoggerActive = true;
            }
            else
            {
                // Recolor label.
                DevTools4_Label.ForeColor = Color.Red;

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
        private void ResetControls_Button_Click(object sender, EventArgs e)
        {
            // Ask user if they are sure to reset all controls.
            if (MessageBox.Show("Are you sure you wish to reset all form controls?", "Reset All Controls", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // Backgrounds.
                Settings.Default.InventoryBackground = "";
                Settings.Default.InventoryBackgroundCount = 0;
                Settings.Default.ChatBackground = "";
                Settings.Default.ChatBackgroundCount = 0;
                Settings.Default.PlayerBackground = "";
                Settings.Default.PlayerBackgroundCount = 0;
                Settings.Default.WorldBackground = "";
                Settings.Default.WorldBackgroundCount = 0;
                if (InventorySkins.Count() < 1 || !File.Exists(InventorySkins.ToArray()[Settings.Default.InventoryBackgroundCount])) // Check if folder is empty. Fix: v1.3.4
                    Main_TabControl.TabPages[0].BackgroundImage = null;
                else
                    Main_TabControl.TabPages[0].BackgroundImage = ImageFast.FromFile(InventorySkins.ToArray()[Settings.Default.InventoryBackgroundCount].ToString());
                Main_TabControl.TabPages[1].BackgroundImage = null;
                Main_TabControl.TabPages[2].BackgroundImage = null;
                Main_TabControl.TabPages[3].BackgroundImage = null;

                // Main controls.
                MaxRadius_NumericUpDown.Value = decimal.Parse(Settings.Default.GetType().GetProperty(GetNameOf(() => Settings.Default.MapRenderingMax)).GetCustomAttribute<System.Configuration.DefaultSettingValueAttribute>().Value);     // Map rendering max radius.
                StartRadius_NumericUpDown.Value = decimal.Parse(Settings.Default.GetType().GetProperty(GetNameOf(() => Settings.Default.MapRenderingStart)).GetCustomAttribute<System.Configuration.DefaultSettingValueAttribute>().Value); // Map rendering start radius.
                CastDelay_NumericUpDown.Value = decimal.Parse(Settings.Default.GetType().GetProperty(GetNameOf(() => Settings.Default.FishingCast)).GetCustomAttribute<System.Configuration.DefaultSettingValueAttribute>().Value);         // Fishing bot casting delay.
                FishingPadding_NumericUpDown.Value = decimal.Parse(Settings.Default.GetType().GetProperty(GetNameOf(() => Settings.Default.FishingPadding)).GetCustomAttribute<System.Configuration.DefaultSettingValueAttribute>().Value); // Fishing bot padding delay.

                // World properties console.
                WorldInformation_DataGridView.RowsDefaultCellStyle.ForeColor = Color.Snow;
                WorldInformation_DataGridView.AlternatingRowsDefaultCellStyle.ForeColor = Color.Snow;
                foreach (DataGridViewRow row in WorldInformation_DataGridView.Rows)
                {
                    row.DefaultCellStyle.ForeColor = Color.Snow;
                }
                Settings.Default.ConsoleForeColor = Color.Snow;
                ColorSample_Button.ForeColor = Color.Snow;
                ColorSample_Button.BackColor = Color.Snow;

                // Dev tools.
                DevToolsDelay_NumericUpDown.Value = decimal.Parse(Settings.Default.GetType().GetProperty(GetNameOf(() => Settings.Default.DevToolDelay)).GetCustomAttribute<System.Configuration.DefaultSettingValueAttribute>().Value);      // Dev tool operation delay.
                RadialMoveScale_NumericUpDown.Value = decimal.Parse(Settings.Default.GetType().GetProperty(GetNameOf(() => Settings.Default.RadialMoveScale)).GetCustomAttribute<System.Configuration.DefaultSettingValueAttribute>().Value); // Auto render maps radialMoveScale.
                AlwaysOnTop_CheckBox.Checked = bool.Parse(Settings.Default.GetType().GetProperty(GetNameOf(() => Settings.Default.TopMost)).GetCustomAttribute<System.Configuration.DefaultSettingValueAttribute>().Value);                   // Set as top most.
                AppPriority_ComboBox.SelectedIndex = int.Parse(Settings.Default.GetType().GetProperty(GetNameOf(() => Settings.Default.ProcessPriorityIndex)).GetCustomAttribute<System.Configuration.DefaultSettingValueAttribute>().Value); // Set the priority.
                FormOpacity_TrackBar.Value = int.Parse(Settings.Default.GetType().GetProperty(GetNameOf(() => Settings.Default.FormOpacity)).GetCustomAttribute<System.Configuration.DefaultSettingValueAttribute>().Value);                  // Set the form opacity.

                // Display completed message.
                MessageBox.Show("All controls have been reset!", "Reset All Controls", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        // Added support for earlier dotnet.
        public static string GetNameOf<T>(Expression<Func<T>> expression)
        {
            var body = (MemberExpression)expression.Body;

            return body.Member.Name;
        }
        #endregion

        #region Set Process Priority

        // Save the previous priority value.
        public string originalPriorityValue = "Normal";
        private void AppPriority_ComboBox_Enter(object sender, EventArgs e)
        {
            // Update the global string with the new value.
            originalPriorityValue = AppPriority_ComboBox.Text;
        }

        // Change process priority.
        private void AppPriority_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Try to get the priority based on a string.
            ProcessPriorityClass priority = ProcessPriorityClass.Normal; // Define default priority.
            if (Enum.TryParse<ProcessPriorityClass>(AppPriority_ComboBox.SelectedItem.ToString().Replace(" ", ""), out priority))
            {
                // Double check if the player wishes to enable this.
                if (Settings.Default.ProcessPriorityIndex != 0 && priority == ProcessPriorityClass.RealTime && MessageBox.Show("Are you sure you wish to enable real time priority?\n\nThis setting may cause your PC to freeze while memory scanning or performing some operations.", "Enable Real Time Priority:", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    // User canceld, revert changes.
                    AppPriority_ComboBox.SelectedIndex = Settings.Default.ProcessPriorityIndex;

                    // Close function.
                    return;
                }

                // Set a new priority for all iterations of the core keeper process.
                Process[] processes = Process.GetProcessesByName("CoreKeepersWorkshop");
                foreach (Process proc in processes)
                {
                    // Set the priority of the found process.
                    proc.PriorityClass = priority;
                }

                // Save the new index value.
                Settings.Default.ProcessPriorityIndex = AppPriority_ComboBox.SelectedIndex;
            }
        }

        #endregion // End set process priority.

        #region Set Form Transparency

        private void FormOpacity_TrackBar_ValueChanged(object sender, EventArgs e)
        {
            // Set form opacity based on trackbars value (1 to 100 -> 0.01 to 1.0).
            this.Opacity = FormOpacity_TrackBar.Value / 100.0;

            // Update the trackbars label.
            FormOpacity_Label.Text = $"Form Opacity: [{FormOpacity_TrackBar.Value}%]";

            // Save the changed opacity value to the settings to be used elsewhere.
            Settings.Default.FormOpacity = FormOpacity_TrackBar.Value;
        }
        #endregion

        #endregion // End admin tools.
    }
}
