using CoreKeeperInventoryEditor;
using Memory;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;

namespace CoreKeepersWorkshop
{
    public partial class SkillEditor : Form
    {
        public SkillEditor()
        {
            InitializeComponent();
        }

        #region Variables

        // Define some varibles.
        public Mem MemLib = new Mem();

        // Define total enabled skills.
        public int totalEnabledSkills = (int)CoreKeepersWorkshop.Properties.Settings.Default.ActiveSkillAmount;

        // Define string from host form.
        public string playerToolAddress = CoreKeepersWorkshop.Properties.Settings.Default.SkillEditorAddress;
        #endregion // End variables.

        #region Form Load And Closing Events

        // Do loading events for the form.
        private void SkillEditor_Load(object sender, EventArgs e)
        {
            #region Set Custom Cusror

            // Set the applications cursor.
            Cursor = new Cursor(CoreKeepersWorkshop.Properties.Resources.UICursor.GetHicon());
            #endregion

            #region Set Form Locations

            // Set the forms active location based on previous save.
            this.Location = CoreKeepersWorkshop.Properties.Settings.Default.SkillEditorLocation;
            #endregion

            #region Set Form Controls

            // Set controls based on saved settings.
            numericUpDown19.Value = totalEnabledSkills;
            #endregion

            #region Tooltips

            // Create a new tooltip.
            ToolTip toolTip = new ToolTip()
            {
                AutoPopDelay = 5000,
                InitialDelay = 750
            };

            // Set tool texts.
            toolTip.SetToolTip(numericUpDown19, "Define how many skils the player has discovered over 1 EXP.");
            toolTip.SetToolTip(numericUpDown20, "DEBUG: Define the header1 offset.");
            toolTip.SetToolTip(numericUpDown21, "DEBUG: Define the header2 offset.");

            toolTip.SetToolTip(button1, "Change your players skills to custom values!");
            toolTip.SetToolTip(button2, "Change your players skills to max values!");
            toolTip.SetToolTip(button3, "Resets all player skills to 0.");

            toolTip.SetToolTip(checkBox1, "This is used to help find the correct addresses.");
            #endregion
        }

        // Do form closing events.
        private void SkillEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            #region Set Form Controls

            // Set controls based on saved settings.
            CoreKeepersWorkshop.Properties.Settings.Default.ActiveSkillAmount = numericUpDown19.Value;
            #endregion

            // Check if the "X" button was pressed to close form.
            if (!new StackTrace().GetFrames().Any(x => x.GetMethod().Name == "Close"))
            {
                // User pressed the "X" button cancle task.
                this.Close();
            }

            // Ensure we catch all closing exceptions. // Fix v1.3.3.
            try
            {
                // Save some form settings.
                CoreKeepersWorkshop.Properties.Settings.Default.SkillEditorLocation = this.Location;
            }
            catch (Exception)
            { } // Do nothing.
        }
        #endregion

        #region Form Controls

        // Adjust how many skills are present.
        private void NumericUpDown19_ValueChanged(object sender, EventArgs e)
        {
            // Reset player address to null each change.
            MainForm.AoBScanResultsSkills = null;

            totalEnabledSkills = (int)numericUpDown19.Value;

            // Endable or disable controls depending on amount.
            panel1.Visible = (totalEnabledSkills >= 1);
            panel2.Visible = (totalEnabledSkills >= 2);
            panel3.Visible = (totalEnabledSkills >= 3);
            panel4.Visible = (totalEnabledSkills >= 4);
            panel5.Visible = (totalEnabledSkills >= 5);
            panel6.Visible = (totalEnabledSkills >= 6);
            panel7.Visible = (totalEnabledSkills >= 7);
            panel8.Visible = (totalEnabledSkills >= 8);
            panel9.Visible = (totalEnabledSkills >= 9);
        }

        // Change skils.
        private async void Button1_Click(object sender, EventArgs e)
        {
            // Ensure that at least one perk is selected.
            if (totalEnabledSkills == 0)
            {
                MessageBox.Show("You need to select at least one skill!", "Player Skill Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Disable controls.
            numericUpDown19.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;

            // Scan for address is needed.
            if (MemLib.OpenProcess("CoreKeeper") && MainForm.AoBScanResultsSkills == null)
            {
                // Adjust progressbar.
                progressBar1.Value = 10;

                // Define the headers.
                uint header1Addpress = (uint)((int)Math.Abs(int.Parse(MemLib.ReadInt(BigInteger.Add(BigInteger.Parse(playerToolAddress, NumberStyles.HexNumber), BigInteger.Parse("168", NumberStyles.Integer)).ToString("X")).ToString())) + (int)numericUpDown20.Value);
                uint footer2Addpress = (uint)((int)header1Addpress + (int)numericUpDown21.Value); // Header 2 is typically minus two.

                // DEBUG: Convert to visual.
                // string AoBString = header1Addpress + " 0 0 0 0 ? 0 " + String.Concat(Enumerable.Repeat("ID VALUE ", (int)numericUpDown19.Value)) + ((int)numericUpDown19.Value == 9 ? "?? ?? ?? ?? ?? ?? ?? ?? " : "") + "? " + footer2Addpress + " 0 0";

                // Define the AoB and scan it into an array list. // Fix: If all 9 skills then add an extra ? ?.
                string AoB = string.Join(" ", BitConverter.GetBytes(header1Addpress).Select(b => b.ToString("X2"))) + " 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 " + "01 00 00 00 ?? ?? ?? ?? " + String.Concat(Enumerable.Repeat("?? ?? ?? ?? ?? ?? ?? ?? ", (int)numericUpDown19.Value - 1)) + ((int)numericUpDown19.Value == 9 ? "?? ?? ?? ?? ?? ?? ?? ?? " : "") + "?? ?? ?? ?? " + string.Join(" ", BitConverter.GetBytes(footer2Addpress).Select(b => b.ToString("X2"))) + " 00 00 00 00 00 00 00 00";

                MainForm.AoBScanResultsSkills = await MemLib.AoBScan(AoB, true, true);

                if (MainForm.AoBScanResultsSkills.Count() == 0)
                {
                    MessageBox.Show("Could not find any skill addresses!\nTry restarting your game!", "Player Skill Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Enable controls.
                    numericUpDown19.Enabled = true;
                    button1.Enabled = true;
                    button2.Enabled = true;
                    button3.Enabled = true;

                    progressBar1.Value = 0;
                    return;
                }

                // Adjust progressbar.
                progressBar1.Step = 90 / MainForm.AoBScanResultsSkills.Count();
            }
            else
            {
                // Adjust progressbar.
                progressBar1.Step = 90 / MainForm.AoBScanResultsSkills.Count();
                progressBar1.Value = 10;
            }

            // Update richtextbox with found addresses.
            foreach (long res in MainForm.AoBScanResultsSkills)
            {
                string headerBase = res.ToString("X");
                string skill1ID = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("28", NumberStyles.Integer)).ToString("X");
                string skill1Value = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("32", NumberStyles.Integer)).ToString("X");
                string skill2ID = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("36", NumberStyles.Integer)).ToString("X");
                string skill2Value = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("40", NumberStyles.Integer)).ToString("X");
                string skill3ID = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("44", NumberStyles.Integer)).ToString("X");
                string skill3Value = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("48", NumberStyles.Integer)).ToString("X");
                string skill4ID = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("52", NumberStyles.Integer)).ToString("X");
                string skill4Value = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("56", NumberStyles.Integer)).ToString("X");
                string skill5ID = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("60", NumberStyles.Integer)).ToString("X");
                string skill5Value = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("64", NumberStyles.Integer)).ToString("X");
                string skill6ID = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("68", NumberStyles.Integer)).ToString("X");
                string skill6Value = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("72", NumberStyles.Integer)).ToString("X");
                string skill7ID = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("76", NumberStyles.Integer)).ToString("X");
                string skill7Value = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("80", NumberStyles.Integer)).ToString("X");
                string skill8ID = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("84", NumberStyles.Integer)).ToString("X");
                string skill8Value = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("88", NumberStyles.Integer)).ToString("X");
                string skill9ID = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("92", NumberStyles.Integer)).ToString("X");
                string skill9Value = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("96", NumberStyles.Integer)).ToString("X");

                #region Set Values

                // Set values.
                if (totalEnabledSkills >= 1)
                {
                    MemLib.WriteMemory(skill1ID, "int", ((int)numericUpDown1.Value).ToString());
                    MemLib.WriteMemory(skill1Value, "int", ((int)numericUpDown2.Value).ToString());
                }
                if (totalEnabledSkills >= 2)
                {
                    MemLib.WriteMemory(skill2ID, "int", ((int)numericUpDown3.Value).ToString());
                    MemLib.WriteMemory(skill2Value, "int", ((int)numericUpDown4.Value).ToString());
                }
                if (totalEnabledSkills >= 3)
                {
                    MemLib.WriteMemory(skill3ID, "int", ((int)numericUpDown5.Value).ToString());
                    MemLib.WriteMemory(skill3Value, "int", ((int)numericUpDown6.Value).ToString());
                }
                if (totalEnabledSkills >= 4)
                {
                    MemLib.WriteMemory(skill4ID, "int", ((int)numericUpDown7.Value).ToString());
                    MemLib.WriteMemory(skill4Value, "int", ((int)numericUpDown8.Value).ToString());
                }
                if (totalEnabledSkills >= 5)
                {
                    MemLib.WriteMemory(skill5ID, "int", ((int)numericUpDown9.Value).ToString());
                    MemLib.WriteMemory(skill5Value, "int", ((int)numericUpDown10.Value).ToString());
                }
                if (totalEnabledSkills >= 6)
                {
                    MemLib.WriteMemory(skill6ID, "int", ((int)numericUpDown11.Value).ToString());
                    MemLib.WriteMemory(skill6Value, "int", ((int)numericUpDown12.Value).ToString());
                }
                if (totalEnabledSkills >= 7)
                {
                    MemLib.WriteMemory(skill7ID, "int", ((int)numericUpDown13.Value).ToString());
                    MemLib.WriteMemory(skill7Value, "int", ((int)numericUpDown14.Value).ToString());
                }
                if (totalEnabledSkills >= 8)
                {
                    MemLib.WriteMemory(skill8ID, "int", ((int)numericUpDown15.Value).ToString());
                    MemLib.WriteMemory(skill8Value, "int", ((int)numericUpDown16.Value).ToString());
                }
                if (totalEnabledSkills >= 9)
                {
                    MemLib.WriteMemory(skill9ID, "int", ((int)numericUpDown17.Value).ToString());
                    MemLib.WriteMemory(skill9Value, "int", ((int)numericUpDown18.Value).ToString());
                }
                #endregion

                // Step progress bar.
                progressBar1.PerformStep();
            }

            // Complete progress bar.
            progressBar1.Value = 100;

            // Enable controls.
            numericUpDown19.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
        }

        // Max skills. 
        private async void Button2_Click(object sender, EventArgs e)
        {
            // Ensure that at least one perk is selected.
            if (totalEnabledSkills == 0)
            {
                MessageBox.Show("You need to select at least one skill!", "Player Skill Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Disable controls.
            numericUpDown19.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;

            // Scan for address is needed.
            if (MemLib.OpenProcess("CoreKeeper") && MainForm.AoBScanResultsSkills == null)
            {
                // Adjust progressbar.
                progressBar1.Value = 10;

                // Define the headers.
                uint header1Addpress = (uint)((int)Math.Abs(int.Parse(MemLib.ReadInt(BigInteger.Add(BigInteger.Parse(playerToolAddress, NumberStyles.HexNumber), BigInteger.Parse("168", NumberStyles.Integer)).ToString("X")).ToString())) + (int)numericUpDown20.Value);
                uint footer2Addpress = (uint)((int)header1Addpress + (int)numericUpDown21.Value); // Header 2 is typically minus two.

                // DEBUG: Convert to visual.
                // string AoBString = header1Addpress + " 0 0 0 0 ? 0 " + String.Concat(Enumerable.Repeat("ID VALUE ", (int)numericUpDown19.Value)) + ((int)numericUpDown19.Value == 9 ? "?? ?? ?? ?? ?? ?? ?? ?? " : "") + "? " + footer2Addpress + " 0 0";

                // Define the AoB and scan it into an array list. // Fix: If all 9 skills then add an extra ? ?.
                string AoB = string.Join(" ", BitConverter.GetBytes(header1Addpress).Select(b => b.ToString("X2"))) + " 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 " + "01 00 00 00 ?? ?? ?? ?? " + String.Concat(Enumerable.Repeat("?? ?? ?? ?? ?? ?? ?? ?? ", (int)numericUpDown19.Value - 1)) + ((int)numericUpDown19.Value == 9 ? "?? ?? ?? ?? ?? ?? ?? ?? " : "") + "?? ?? ?? ?? " + string.Join(" ", BitConverter.GetBytes(footer2Addpress).Select(b => b.ToString("X2"))) + " 00 00 00 00 00 00 00 00";

                MainForm.AoBScanResultsSkills = await MemLib.AoBScan(AoB, true, true);

                if (MainForm.AoBScanResultsSkills.Count() == 0)
                {
                    MessageBox.Show("Could not find any skill addresses!\nTry restarting your game!", "Player Skill Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Enable controls.
                    numericUpDown19.Enabled = true;
                    button1.Enabled = true;
                    button2.Enabled = true;
                    button3.Enabled = true;

                    progressBar1.Value = 0;
                    return;
                }

                // Adjust progressbar.
                progressBar1.Step = 90 / MainForm.AoBScanResultsSkills.Count();
            }
            else
            {
                // Adjust progressbar.
                progressBar1.Step = 90 / MainForm.AoBScanResultsSkills.Count();
                progressBar1.Value = 10;
            }

            // Update richtextbox with found addresses.
            foreach (long res in MainForm.AoBScanResultsSkills)
            {
                string headerBase = res.ToString("X");
                string skill1ID = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("28", NumberStyles.Integer)).ToString("X");
                string skill1Value = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("32", NumberStyles.Integer)).ToString("X");
                string skill2ID = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("36", NumberStyles.Integer)).ToString("X");
                string skill2Value = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("40", NumberStyles.Integer)).ToString("X");
                string skill3ID = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("44", NumberStyles.Integer)).ToString("X");
                string skill3Value = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("48", NumberStyles.Integer)).ToString("X");
                string skill4ID = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("52", NumberStyles.Integer)).ToString("X");
                string skill4Value = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("56", NumberStyles.Integer)).ToString("X");
                string skill5ID = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("60", NumberStyles.Integer)).ToString("X");
                string skill5Value = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("64", NumberStyles.Integer)).ToString("X");
                string skill6ID = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("68", NumberStyles.Integer)).ToString("X");
                string skill6Value = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("72", NumberStyles.Integer)).ToString("X");
                string skill7ID = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("76", NumberStyles.Integer)).ToString("X");
                string skill7Value = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("80", NumberStyles.Integer)).ToString("X");
                string skill8ID = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("84", NumberStyles.Integer)).ToString("X");
                string skill8Value = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("88", NumberStyles.Integer)).ToString("X");
                string skill9ID = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("92", NumberStyles.Integer)).ToString("X");
                string skill9Value = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("96", NumberStyles.Integer)).ToString("X");

                #region Set Values

                // Set values.
                if (totalEnabledSkills >= 1)
                {
                    MemLib.WriteMemory(skill1ID, "int", ((int)numericUpDown1.Value).ToString());
                    MemLib.WriteMemory(skill1Value, "int", "9999999");
                }
                if (totalEnabledSkills >= 2)
                {
                    MemLib.WriteMemory(skill2ID, "int", ((int)numericUpDown3.Value).ToString());
                    MemLib.WriteMemory(skill2Value, "int", "9999999");
                }
                if (totalEnabledSkills >= 3)
                {
                    MemLib.WriteMemory(skill3ID, "int", ((int)numericUpDown5.Value).ToString());
                    MemLib.WriteMemory(skill3Value, "int", "9999999");
                }
                if (totalEnabledSkills >= 4)
                {
                    MemLib.WriteMemory(skill4ID, "int", ((int)numericUpDown7.Value).ToString());
                    MemLib.WriteMemory(skill4Value, "int", "9999999");
                }
                if (totalEnabledSkills >= 5)
                {
                    MemLib.WriteMemory(skill5ID, "int", ((int)numericUpDown9.Value).ToString());
                    MemLib.WriteMemory(skill5Value, "int", "9999999");
                }
                if (totalEnabledSkills >= 6)
                {
                    MemLib.WriteMemory(skill6ID, "int", ((int)numericUpDown11.Value).ToString());
                    MemLib.WriteMemory(skill6Value, "int", "9999999");
                }
                if (totalEnabledSkills >= 7)
                {
                    MemLib.WriteMemory(skill7ID, "int", ((int)numericUpDown13.Value).ToString());
                    MemLib.WriteMemory(skill7Value, "int", "9999999");
                }
                if (totalEnabledSkills >= 8)
                {
                    MemLib.WriteMemory(skill8ID, "int", ((int)numericUpDown15.Value).ToString());
                    MemLib.WriteMemory(skill8Value, "int", "9999999");
                }
                if (totalEnabledSkills >= 9)
                {
                    MemLib.WriteMemory(skill9ID, "int", ((int)numericUpDown17.Value).ToString());
                    MemLib.WriteMemory(skill9Value, "int", "9999999");
                }
                #endregion

                // Step progress bar.
                progressBar1.PerformStep();
            }

            // Complete progress bar.
            progressBar1.Value = 100;

            // Enable controls.
            numericUpDown19.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
        }

        // Reset skills.
        private async void Button3_Click(object sender, EventArgs e)
        {
            // Ensure that at least one perk is selected.
            if (totalEnabledSkills == 0)
            {
                MessageBox.Show("You need to select at least one skill!", "Player Skill Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Disable controls.
            numericUpDown19.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;

            // Scan for address is needed.
            if (MemLib.OpenProcess("CoreKeeper") && MainForm.AoBScanResultsSkills == null)
            {
                // Adjust progressbar.
                progressBar1.Value = 10;

                // Define the headers.
                uint header1Addpress = (uint)((int)Math.Abs(int.Parse(MemLib.ReadInt(BigInteger.Add(BigInteger.Parse(playerToolAddress, NumberStyles.HexNumber), BigInteger.Parse("168", NumberStyles.Integer)).ToString("X")).ToString())) + (int)numericUpDown20.Value);
                uint footer2Addpress = (uint)((int)header1Addpress + (int)numericUpDown21.Value); // Header 2 is typically minus two.

                // DEBUG: Convert to visual.
                // string AoBString = header1Addpress + " 0 0 0 0 ? 0 " + String.Concat(Enumerable.Repeat("ID VALUE ", (int)numericUpDown19.Value)) + ((int)numericUpDown19.Value == 9 ? "?? ?? ?? ?? ?? ?? ?? ?? " : "") + "? " + footer2Addpress + " 0 0";

                // Define the AoB and scan it into an array list. // Fix: If all 9 skills then add an extra ? ?.
                string AoB = string.Join(" ", BitConverter.GetBytes(header1Addpress).Select(b => b.ToString("X2"))) + " 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 ?? ?? ?? ?? 00 00 00 00 " + "01 00 00 00 ?? ?? ?? ?? " + String.Concat(Enumerable.Repeat("?? ?? ?? ?? ?? ?? ?? ?? ", (int)numericUpDown19.Value - 1)) + ((int)numericUpDown19.Value == 9 ? "?? ?? ?? ?? ?? ?? ?? ?? " : "") + "?? ?? ?? ?? " + string.Join(" ", BitConverter.GetBytes(footer2Addpress).Select(b => b.ToString("X2"))) + " 00 00 00 00 00 00 00 00";

                MainForm.AoBScanResultsSkills = await MemLib.AoBScan(AoB, true, true);

                if (MainForm.AoBScanResultsSkills.Count() == 0)
                {
                    MessageBox.Show("Could not find any skill addresses!\nTry restarting your game!", "Player Skill Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Enable controls.
                    numericUpDown19.Enabled = true;
                    button1.Enabled = true;
                    button2.Enabled = true;
                    button3.Enabled = true;

                    progressBar1.Value = 0;
                    return;
                }

                // Adjust progressbar.
                progressBar1.Step = 90 / MainForm.AoBScanResultsSkills.Count();
            }
            else
            {
                // Adjust progressbar.
                progressBar1.Step = 90 / MainForm.AoBScanResultsSkills.Count();
                progressBar1.Value = 10;
            }

            // Update richtextbox with found addresses.
            foreach (long res in MainForm.AoBScanResultsSkills)
            {
                string headerBase = res.ToString("X");
                string skill1ID = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("28", NumberStyles.Integer)).ToString("X");
                string skill1Value = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("32", NumberStyles.Integer)).ToString("X");
                string skill2ID = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("36", NumberStyles.Integer)).ToString("X");
                string skill2Value = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("40", NumberStyles.Integer)).ToString("X");
                string skill3ID = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("44", NumberStyles.Integer)).ToString("X");
                string skill3Value = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("48", NumberStyles.Integer)).ToString("X");
                string skill4ID = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("52", NumberStyles.Integer)).ToString("X");
                string skill4Value = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("56", NumberStyles.Integer)).ToString("X");
                string skill5ID = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("60", NumberStyles.Integer)).ToString("X");
                string skill5Value = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("64", NumberStyles.Integer)).ToString("X");
                string skill6ID = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("68", NumberStyles.Integer)).ToString("X");
                string skill6Value = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("72", NumberStyles.Integer)).ToString("X");
                string skill7ID = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("76", NumberStyles.Integer)).ToString("X");
                string skill7Value = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("80", NumberStyles.Integer)).ToString("X");
                string skill8ID = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("84", NumberStyles.Integer)).ToString("X");
                string skill8Value = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("88", NumberStyles.Integer)).ToString("X");
                string skill9ID = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("92", NumberStyles.Integer)).ToString("X");
                string skill9Value = BigInteger.Add(BigInteger.Parse(headerBase, NumberStyles.HexNumber), BigInteger.Parse("96", NumberStyles.Integer)).ToString("X");

                #region Set Values

                // Set values.
                if (totalEnabledSkills >= 1)
                {
                    MemLib.WriteMemory(skill1ID, "int", ((int)numericUpDown1.Value).ToString());
                    MemLib.WriteMemory(skill1Value, "int", "0");
                }
                if (totalEnabledSkills >= 2)
                {
                    MemLib.WriteMemory(skill2ID, "int", ((int)numericUpDown3.Value).ToString());
                    MemLib.WriteMemory(skill2Value, "int", "0");
                }
                if (totalEnabledSkills >= 3)
                {
                    MemLib.WriteMemory(skill3ID, "int", ((int)numericUpDown5.Value).ToString());
                    MemLib.WriteMemory(skill3Value, "int", "0");
                }
                if (totalEnabledSkills >= 4)
                {
                    MemLib.WriteMemory(skill4ID, "int", ((int)numericUpDown7.Value).ToString());
                    MemLib.WriteMemory(skill4Value, "int", "0");
                }
                if (totalEnabledSkills >= 5)
                {
                    MemLib.WriteMemory(skill5ID, "int", ((int)numericUpDown9.Value).ToString());
                    MemLib.WriteMemory(skill5Value, "int", "0");
                }
                if (totalEnabledSkills >= 6)
                {
                    MemLib.WriteMemory(skill6ID, "int", ((int)numericUpDown11.Value).ToString());
                    MemLib.WriteMemory(skill6Value, "int", "0");
                }
                if (totalEnabledSkills >= 7)
                {
                    MemLib.WriteMemory(skill7ID, "int", ((int)numericUpDown13.Value).ToString());
                    MemLib.WriteMemory(skill7Value, "int", "0");
                }
                if (totalEnabledSkills >= 8)
                {
                    MemLib.WriteMemory(skill8ID, "int", ((int)numericUpDown15.Value).ToString());
                    MemLib.WriteMemory(skill8Value, "int", "0");
                }
                if (totalEnabledSkills >= 9)
                {
                    MemLib.WriteMemory(skill9ID, "int", ((int)numericUpDown17.Value).ToString());
                    MemLib.WriteMemory(skill9Value, "int", "0");
                }
                #endregion

                // Step progress bar.
                progressBar1.PerformStep();
            }

            // Complete progress bar.
            progressBar1.Value = 100;

            // Enable controls.
            numericUpDown19.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
        }

        // Reset player address to null each change.
        private void NumericUpDown20_ValueChanged(object sender, EventArgs e)
        {
            MainForm.AoBScanResultsSkills = null;
        }

        // Reset player address to null each change.
        private void NumericUpDown21_ValueChanged(object sender, EventArgs e)
        {
            MainForm.AoBScanResultsSkills = null;
        }

        // Enable or disable debug mode.
        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            // Check if checkbox was checked.
            if (checkBox1.Checked)
            {
                // Make numericupdowns readonly.
                numericUpDown20.Enabled = true;
                numericUpDown21.Enabled = true;
            }
            else
            {
                // Make numericupdowns editable.
                numericUpDown20.Enabled = false;
                numericUpDown21.Enabled = false;
            }
        }

        #region Change Images

        // Picturebox 1.
        // Warn users about changing this value.
        bool warnUsers = true;
        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (warnUsers && MessageBox.Show("Are you sure you want to change the value of the first skill?\nChanging this will break any further descoveries of this players skills!\n\nContinue?", "Change the ID of first skill?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                // Defualt the up down.
                warnUsers = false; // Disable temporarly to disable further notifications.
                numericUpDown1.Value = 1;
                warnUsers = true; // Re-enable.
            }
            else
            {
                // User accepted their fate.
                warnUsers = false;
            }

            // Adjust the image.
            pictureBox1.BackgroundImage = GetBitmap((int)numericUpDown1.Value);
        }

        // Picturebox 2.
        private void NumericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            pictureBox2.BackgroundImage = GetBitmap((int)numericUpDown3.Value);
        }

        // Picturebox 3.
        private void NumericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            pictureBox3.BackgroundImage = GetBitmap((int)numericUpDown5.Value);
        }

        // Picturebox 4.
        private void NumericUpDown7_ValueChanged(object sender, EventArgs e)
        {
            pictureBox4.BackgroundImage = GetBitmap((int)numericUpDown7.Value);
        }

        // Picturebox 5.
        private void NumericUpDown9_ValueChanged(object sender, EventArgs e)
        {
            pictureBox5.BackgroundImage = GetBitmap((int)numericUpDown9.Value);
        }

        // Picturebox 6.
        private void NumericUpDown11_ValueChanged(object sender, EventArgs e)
        {
            pictureBox6.BackgroundImage = GetBitmap((int)numericUpDown11.Value);
        }

        // Picturebox 7.
        private void NumericUpDown13_ValueChanged(object sender, EventArgs e)
        {
            pictureBox7.BackgroundImage = GetBitmap((int)numericUpDown13.Value);
        }

        // Picturebox 8.
        private void NumericUpDown15_ValueChanged(object sender, EventArgs e)
        {
            pictureBox8.BackgroundImage = GetBitmap((int)numericUpDown15.Value);
        }

        // Picturebox 9.
        private void NumericUpDown17_ValueChanged(object sender, EventArgs e)
        {
            pictureBox9.BackgroundImage = GetBitmap((int)numericUpDown17.Value);
        }

        // Get image per numericupdown value.
        Bitmap GetBitmap(int value)
        {
            if (value == 0)
                return Properties.Resources.Skill1;
            if (value == 1)
                return Properties.Resources.Skill2;
            if (value == 2)
                return Properties.Resources.Skill3;
            if (value == 3)
                return Properties.Resources.Skill4;
            if (value == 4)
                return Properties.Resources.Skill5;
            if (value == 5)
                return Properties.Resources.Skill6;
            if (value == 6)
                return Properties.Resources.Skill7;
            if (value == 7)
                return Properties.Resources.Skill8;
            if (value == 8)
                return Properties.Resources.Skill9;

            return Properties.Resources.UnknownItem;
        }
        #endregion // End change images.

        #endregion // End form controls.
    }
}