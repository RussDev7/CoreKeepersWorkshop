using System.Collections.Generic;
using CoreKeeperInventoryEditor;
using System.Globalization;
using System.Windows.Forms;
using System.Diagnostics;
using System.Numerics;
using System.Drawing;
using System.Linq;
using Memory;
using System;
using CoreKeepersWorkshop.Properties;

namespace CoreKeepersWorkshop
{
    public partial class SkillEditor : Form
    {
        public SkillEditor()
        {
            InitializeComponent();
        }

        #region Variables

        // Define some variables.
        public Mem MemLib = new Mem();
        public int useAddress = 1;

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

            #region Set Form Transparency

            // Set form opacity based on trackbars value saved setting (1 to 100 -> 0.01 to 1.0).
            this.Opacity = Settings.Default.FormOpacity / 100.0;
            #endregion

            #region Set Form Controls

            // Set controls based on saved settings.
            UseEXPValues_CheckBox.Checked = CoreKeepersWorkshop.Properties.Settings.Default.SkillEditorUseEXP;
            #endregion

            #region Tooltips

            // Create a new tooltip.
            ToolTip toolTip = new ToolTip()
            {
                AutoPopDelay = 5000,
                InitialDelay = 750
            };

            // Set tool texts.
            toolTip.SetToolTip(SkillID0_NumericUpDown, "Set the current skilltype to desired a ID.");
            toolTip.SetToolTip(SkillID1_NumericUpDown, "Set the current skilltype to desired a ID.");
            toolTip.SetToolTip(SkillID2_NumericUpDown, "Set the current skilltype to desired a ID.");
            toolTip.SetToolTip(SkillID3_NumericUpDown, "Set the current skilltype to desired a ID.");
            toolTip.SetToolTip(SkillID4_NumericUpDown, "Set the current skilltype to desired a ID.");
            toolTip.SetToolTip(SkillID5_NumericUpDown, "Set the current skilltype to desired a ID.");
            toolTip.SetToolTip(SkillID6_NumericUpDown, "Set the current skilltype to desired a ID.");
            toolTip.SetToolTip(SkillID7_NumericUpDown, "Set the current skilltype to desired a ID.");
            toolTip.SetToolTip(SkillID8_NumericUpDown, "Set the current skilltype to desired a ID.");
            toolTip.SetToolTip(SkillID9_NumericUpDown, "Set the current skilltype to desired a ID.");
            toolTip.SetToolTip(SkillID10_NumericUpDown, "Set the current skilltype to desired a ID.");
            toolTip.SetToolTip(SkillID11_NumericUpDown, "Set the current skilltype to desired a ID.");

            toolTip.SetToolTip(SkillILvL0_NumericUpDown, "Set the EXP amount for the desired skill.");
            toolTip.SetToolTip(SkillILvL1_NumericUpDown, "Set the EXP amount for the desired skill.");
            toolTip.SetToolTip(SkillILvL2_NumericUpDown, "Set the EXP amount for the desired skill.");
            toolTip.SetToolTip(SkillILvL3_NumericUpDown, "Set the EXP amount for the desired skill.");
            toolTip.SetToolTip(SkillILvL4_NumericUpDown, "Set the EXP amount for the desired skill.");
            toolTip.SetToolTip(SkillILvL5_NumericUpDown, "Set the EXP amount for the desired skill.");
            toolTip.SetToolTip(SkillILvL6_NumericUpDown, "Set the EXP amount for the desired skill.");
            toolTip.SetToolTip(SkillILvL7_NumericUpDown, "Set the EXP amount for the desired skill.");
            toolTip.SetToolTip(SkillILvL8_NumericUpDown, "Set the EXP amount for the desired skill.");
            toolTip.SetToolTip(SkillILvL9_NumericUpDown, "Set the EXP amount for the desired skill.");
            toolTip.SetToolTip(SkillILvL10_NumericUpDown, "Set the EXP amount for the desired skill.");
            toolTip.SetToolTip(SkillILvL11_NumericUpDown, "Set the EXP amount for the desired skill.");

            // toolTip.SetToolTip(numericUpDown19, "Define how many skills the player has discovered over 1 EXP.");
            // toolTip.SetToolTip(numericUpDown20, "DEBUG: Define the header1 offset.");
            // toolTip.SetToolTip(numericUpDown21, "DEBUG: Define the header2 offset.");

            toolTip.SetToolTip(ChangeSkills_Button, "Change your players skills to custom values!");
            toolTip.SetToolTip(MaxAllSkills_Button, "Change your players skills to max values!");
            toolTip.SetToolTip(ResetAllSkills_Button, "Resets all player skills to 0.");
            toolTip.SetToolTip(MaxLevelsHelp_Button, "Show a list of all skill names, IDs, and max values.");
            toolTip.SetToolTip(GetPlayerSkillAddresses_Button, "Scan for the skill addresses.");
            toolTip.SetToolTip(UseSkillAddress_Button, "Select the current loadout for scanning.");
            toolTip.SetToolTip(PreviousSkillAddress_Button, "Switch skill loadout to the previous address.");
            toolTip.SetToolTip(NextSkillAddress_Button, "Switch skill loadout to the next address.");

            // toolTip.SetToolTip(checkBox1, "This is used to help find the correct addresses.");
            toolTip.SetToolTip(UseEXPValues_CheckBox, "Switch the display format to use EXP vs Levels.");
            #endregion
        }

        #region Form Closing

        // Do form closing events.
        private void SkillEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Check if the "X" button was pressed to close form.
            if (!new StackTrace().GetFrames().Any(x => x.GetMethod().Name == "Close"))
            {
                // User pressed the "X" button cancel task.
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

        #endregion

        #region Form Controls

        #region Display Levels or EXP

        // Change the format the skill values are displayed in. EXP or Levels.
        private void UseEXPValues_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Get the current state.
            if (UseEXPValues_CheckBox.Checked)
            {
                // Use exp.

                // Change the numerics max value.
                SkillILvL0_NumericUpDown.Maximum = GetNumericMaximum((int)SkillID0_NumericUpDown.Value);
                SkillILvL1_NumericUpDown.Maximum = GetNumericMaximum((int)SkillID1_NumericUpDown.Value);
                SkillILvL2_NumericUpDown.Maximum = GetNumericMaximum((int)SkillID2_NumericUpDown.Value);
                SkillILvL3_NumericUpDown.Maximum = GetNumericMaximum((int)SkillID3_NumericUpDown.Value);
                SkillILvL4_NumericUpDown.Maximum = GetNumericMaximum((int)SkillID4_NumericUpDown.Value);
                SkillILvL5_NumericUpDown.Maximum = GetNumericMaximum((int)SkillID5_NumericUpDown.Value);
                SkillILvL6_NumericUpDown.Maximum = GetNumericMaximum((int)SkillID6_NumericUpDown.Value);
                SkillILvL7_NumericUpDown.Maximum = GetNumericMaximum((int)SkillID7_NumericUpDown.Value);
                SkillILvL8_NumericUpDown.Maximum = GetNumericMaximum((int)SkillID8_NumericUpDown.Value);
                SkillILvL9_NumericUpDown.Maximum = GetNumericMaximum((int)SkillID9_NumericUpDown.Value);
                SkillILvL10_NumericUpDown.Maximum = GetNumericMaximum((int)SkillID10_NumericUpDown.Value);
                SkillILvL11_NumericUpDown.Maximum = GetNumericMaximum((int)SkillID11_NumericUpDown.Value);

                // Since increasing, translate the numerics to a higher value.
                SkillILvL0_NumericUpDown.Value = GetConvertedValues((int)SkillID0_NumericUpDown.Value, (int)SkillILvL0_NumericUpDown.Value, true, true);
                SkillILvL1_NumericUpDown.Value = GetConvertedValues((int)SkillID1_NumericUpDown.Value, (int)SkillILvL1_NumericUpDown.Value, true, true);
                SkillILvL2_NumericUpDown.Value = GetConvertedValues((int)SkillID2_NumericUpDown.Value, (int)SkillILvL2_NumericUpDown.Value, true, true);
                SkillILvL3_NumericUpDown.Value = GetConvertedValues((int)SkillID3_NumericUpDown.Value, (int)SkillILvL3_NumericUpDown.Value, true, true);
                SkillILvL4_NumericUpDown.Value = GetConvertedValues((int)SkillID4_NumericUpDown.Value, (int)SkillILvL4_NumericUpDown.Value, true, true);
                SkillILvL5_NumericUpDown.Value = GetConvertedValues((int)SkillID5_NumericUpDown.Value, (int)SkillILvL5_NumericUpDown.Value, true, true);
                SkillILvL6_NumericUpDown.Value = GetConvertedValues((int)SkillID6_NumericUpDown.Value, (int)SkillILvL6_NumericUpDown.Value, true, true);
                SkillILvL7_NumericUpDown.Value = GetConvertedValues((int)SkillID7_NumericUpDown.Value, (int)SkillILvL7_NumericUpDown.Value, true, true);
                SkillILvL8_NumericUpDown.Value = GetConvertedValues((int)SkillID8_NumericUpDown.Value, (int)SkillILvL8_NumericUpDown.Value, true, true);
                SkillILvL9_NumericUpDown.Value = GetConvertedValues((int)SkillID9_NumericUpDown.Value, (int)SkillILvL9_NumericUpDown.Value, true, true);
                SkillILvL10_NumericUpDown.Value = GetConvertedValues((int)SkillID10_NumericUpDown.Value, (int)SkillILvL10_NumericUpDown.Value, true, true);
                SkillILvL11_NumericUpDown.Value = GetConvertedValues((int)SkillID11_NumericUpDown.Value, (int)SkillILvL11_NumericUpDown.Value, true, true);

                // Change the labels.
                LvLRow1_Label.Text = "EXP";
                LvLRow2_Label.Text = "EXP";
                LvLRow3_Label.Text = "EXP";
                LvLRow4_Label.Text = "EXP";
            }
            else
            {
                // Use levels.

                // Since decreasing, translate the numerics to a lower value.
                SkillILvL0_NumericUpDown.Value = GetConvertedValues((int)SkillID0_NumericUpDown.Value, (int)SkillILvL0_NumericUpDown.Value, true, false);
                SkillILvL1_NumericUpDown.Value = GetConvertedValues((int)SkillID1_NumericUpDown.Value, (int)SkillILvL1_NumericUpDown.Value, true, false);
                SkillILvL2_NumericUpDown.Value = GetConvertedValues((int)SkillID2_NumericUpDown.Value, (int)SkillILvL2_NumericUpDown.Value, true, false);
                SkillILvL3_NumericUpDown.Value = GetConvertedValues((int)SkillID3_NumericUpDown.Value, (int)SkillILvL3_NumericUpDown.Value, true, false);
                SkillILvL4_NumericUpDown.Value = GetConvertedValues((int)SkillID4_NumericUpDown.Value, (int)SkillILvL4_NumericUpDown.Value, true, false);
                SkillILvL5_NumericUpDown.Value = GetConvertedValues((int)SkillID5_NumericUpDown.Value, (int)SkillILvL5_NumericUpDown.Value, true, false);
                SkillILvL6_NumericUpDown.Value = GetConvertedValues((int)SkillID6_NumericUpDown.Value, (int)SkillILvL6_NumericUpDown.Value, true, false);
                SkillILvL7_NumericUpDown.Value = GetConvertedValues((int)SkillID7_NumericUpDown.Value, (int)SkillILvL7_NumericUpDown.Value, true, false);
                SkillILvL8_NumericUpDown.Value = GetConvertedValues((int)SkillID8_NumericUpDown.Value, (int)SkillILvL8_NumericUpDown.Value, true, false);
                SkillILvL9_NumericUpDown.Value = GetConvertedValues((int)SkillID9_NumericUpDown.Value, (int)SkillILvL9_NumericUpDown.Value, true, false);
                SkillILvL10_NumericUpDown.Value = GetConvertedValues((int)SkillID10_NumericUpDown.Value, (int)SkillILvL10_NumericUpDown.Value, true, false);
                SkillILvL11_NumericUpDown.Value = GetConvertedValues((int)SkillID11_NumericUpDown.Value, (int)SkillILvL11_NumericUpDown.Value, true, false);

                // Change the numerics max value.
                SkillILvL0_NumericUpDown.Maximum = 100;
                SkillILvL1_NumericUpDown.Maximum = 100;
                SkillILvL2_NumericUpDown.Maximum = 100;
                SkillILvL3_NumericUpDown.Maximum = 100;
                SkillILvL4_NumericUpDown.Maximum = 100;
                SkillILvL5_NumericUpDown.Maximum = 100;
                SkillILvL6_NumericUpDown.Maximum = 100;
                SkillILvL7_NumericUpDown.Maximum = 100;
                SkillILvL8_NumericUpDown.Maximum = 100;
                SkillILvL9_NumericUpDown.Maximum = 100;
                SkillILvL10_NumericUpDown.Maximum = 100;
                SkillILvL11_NumericUpDown.Maximum = 100;

                // Change the labels.
                LvLRow1_Label.Text = "LVL";
                LvLRow2_Label.Text = "LVL";
                LvLRow3_Label.Text = "LVL";
                LvLRow4_Label.Text = "LVL";
            }
        }
        #endregion

        #region Change Images

        // Picturebox 1.
        // Warn users about changing this value.
        // bool warnUsers = true;
        private void SkillID0_NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            // if (warnUsers && MessageBox.Show("Are you sure you want to change the value of the first skill?\nChanging this will break any further discoveries of this players skills!\n\nContinue?", "Change the ID of first skill?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            // {
            //     // Default the up down.
            //     warnUsers = false; // Disable temporarily to disable further notifications.
            //     numericUpDown1.Value = 1;
            //     warnUsers = true; // Re-enable.
            // }
            // else
            // {
            //     // User accepted their fate.
            //     warnUsers = false;
            // }

            // Adjust the image.
            Skill0_PictureBox.BackgroundImage = GetBitmap((int)SkillID0_NumericUpDown.Value);

            // Set the new maximum.
            SkillSelection_ValueChanged();
        }

        // Picturebox 2.
        private void SkillID1_NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Skill1_PictureBox.BackgroundImage = GetBitmap((int)SkillID1_NumericUpDown.Value);

            // Set the new maximum.
            SkillSelection_ValueChanged();
        }

        // Picturebox 3.
        private void SkillID2_NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Skill2_PictureBox.BackgroundImage = GetBitmap((int)SkillID2_NumericUpDown.Value);

            // Set the new maximum.
            SkillSelection_ValueChanged();
        }

        // Picturebox 4.
        private void SkillID3_NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Skill3_PictureBox.BackgroundImage = GetBitmap((int)SkillID3_NumericUpDown.Value);

            // Set the new maximum.
            SkillSelection_ValueChanged();
        }

        // Picturebox 5.
        private void SkillID4_NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Skill4_PictureBox.BackgroundImage = GetBitmap((int)SkillID4_NumericUpDown.Value);

            // Set the new maximum.
            SkillSelection_ValueChanged();
        }

        // Picturebox 6.
        private void SkillID5_NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Skill5_PictureBox.BackgroundImage = GetBitmap((int)SkillID5_NumericUpDown.Value);

            // Set the new maximum.
            SkillSelection_ValueChanged();
        }

        // Picturebox 7.
        private void SkillID6_NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Skill6_PictureBox.BackgroundImage = GetBitmap((int)SkillID6_NumericUpDown.Value);

            // Set the new maximum.
            SkillSelection_ValueChanged();
        }

        // Picturebox 8.
        private void SkillID7_NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Skill7_PictureBox.BackgroundImage = GetBitmap((int)SkillID7_NumericUpDown.Value);

            // Set the new maximum.
            SkillSelection_ValueChanged();
        }

        // Picturebox 9.
        private void SkillID8_NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Skill8_PictureBox.BackgroundImage = GetBitmap((int)SkillID8_NumericUpDown.Value);

            // Set the new maximum.
            SkillSelection_ValueChanged();
        }

        // Picturebox 10.
        private void SkillID9_NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Skill9_PictureBox.BackgroundImage = GetBitmap((int)SkillID9_NumericUpDown.Value);

            // Set the new maximum.
            SkillSelection_ValueChanged();
        }

        // Picturebox 11.
        private void SkillID10_NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Skill10_PictureBox.BackgroundImage = GetBitmap((int)SkillID10_NumericUpDown.Value);

            // Set the new maximum.
            SkillSelection_ValueChanged();
        }

        // Picturebox 12.
        private void SkillID11_NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            Skill11_PictureBox.BackgroundImage = GetBitmap((int)SkillID11_NumericUpDown.Value);

            // Set the new maximum.
            SkillSelection_ValueChanged();
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
            if (value == 9)
                return Properties.Resources.Skill10;
            if (value == 10)
                return Properties.Resources.Skill11;
            if (value == 11)
                return Properties.Resources.Skill12;

            return Properties.Resources.UnknownItem;
        }
        #endregion // End change images.

        #endregion // End form controls.

        #region Event Handeler & Form Helpers

        // Event handler for skill selection change.
        private void SkillSelection_ValueChanged()
        {
            // Check if 
            if (UseEXPValues_CheckBox.Checked)
            {
                // Change the numerics max value.
                SkillILvL0_NumericUpDown.Maximum = GetNumericMaximum((int)SkillID0_NumericUpDown.Value);
                SkillILvL1_NumericUpDown.Maximum = GetNumericMaximum((int)SkillID1_NumericUpDown.Value);
                SkillILvL2_NumericUpDown.Maximum = GetNumericMaximum((int)SkillID2_NumericUpDown.Value);
                SkillILvL3_NumericUpDown.Maximum = GetNumericMaximum((int)SkillID3_NumericUpDown.Value);
                SkillILvL4_NumericUpDown.Maximum = GetNumericMaximum((int)SkillID4_NumericUpDown.Value);
                SkillILvL5_NumericUpDown.Maximum = GetNumericMaximum((int)SkillID5_NumericUpDown.Value);
                SkillILvL6_NumericUpDown.Maximum = GetNumericMaximum((int)SkillID6_NumericUpDown.Value);
                SkillILvL7_NumericUpDown.Maximum = GetNumericMaximum((int)SkillID7_NumericUpDown.Value);
                SkillILvL8_NumericUpDown.Maximum = GetNumericMaximum((int)SkillID8_NumericUpDown.Value);
                SkillILvL9_NumericUpDown.Maximum = GetNumericMaximum((int)SkillID9_NumericUpDown.Value);
                SkillILvL10_NumericUpDown.Maximum = GetNumericMaximum((int)SkillID10_NumericUpDown.Value);
                SkillILvL11_NumericUpDown.Maximum = GetNumericMaximum((int)SkillID11_NumericUpDown.Value);
            }
            else
            {
                // Change the numerics max value.
                SkillILvL0_NumericUpDown.Maximum = 100;
                SkillILvL1_NumericUpDown.Maximum = 100;
                SkillILvL2_NumericUpDown.Maximum = 100;
                SkillILvL3_NumericUpDown.Maximum = 100;
                SkillILvL4_NumericUpDown.Maximum = 100;
                SkillILvL5_NumericUpDown.Maximum = 100;
                SkillILvL6_NumericUpDown.Maximum = 100;
                SkillILvL7_NumericUpDown.Maximum = 100;
                SkillILvL8_NumericUpDown.Maximum = 100;
                SkillILvL9_NumericUpDown.Maximum = 100;
                SkillILvL10_NumericUpDown.Maximum = 100;
                SkillILvL11_NumericUpDown.Maximum = 100;
            }
        }

        private void SetReadOnly(Control parent, bool readOnly)
        {
            foreach(Control control in parent.Controls)
            {
                // C# 7.0+: if (control is NumericUpDown numericUpDown)
                NumericUpDown numericUpDown = control as NumericUpDown;
                if (numericUpDown != null)
                {
                    numericUpDown.ReadOnly = readOnly;                                                   // Set the readonly attributes.
                    numericUpDown.Controls[0].Enabled = !readOnly;                                       // Enable/disable the up/down buttons.
                    numericUpDown.KeyDown += (s, e) => e.SuppressKeyPress = readOnly;                    // Block/unblock the mouse and keyboard input.
                    numericUpDown.MouseWheel += (s, e) => ((HandledMouseEventArgs)e).Handled = readOnly;
                }
            }
        }
        #endregion

        #region Help Buttons

        // Show skill help button.
        private void MaxLevelsHelp_Button_Click(object sender, EventArgs e)
        {
            MessageBox.Show("SKILL-NAME\tID\tMAX-LEVEL\r\n" +
                            "=========================\r\n" +
                            "Mining\t\t| 0 |\t59978\r\n" +
                            "Running\t\t| 1 |\t498767\r\n" +
                            "Melee Combat\t| 2 |\t20001\r\n" +
                            "Vitality\t\t| 3 |\t4999038\r\n" +
                            "Crafting\t\t| 4 |\t29995\r\n" +
                            "Range Combat\t| 5 |\t20001\r\n" +
                            "Gardening\t| 6 |\t6602\r\n" +
                            "Fishing\t\t| 7 |\t1494\r\n" +
                            "Cooking\t\t| 8 |\t5000\r\n" +
                            "Magic\t\t| 9 |\t20001\r\n" +
                            "Summoning\t| 10 |\t59663\r\n" +
                            "Explosives\t| 11 |\t2006", "Player Skill Editor", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

        #region Get Addresses

        #region Scan For Initial Address

        // Get valid skill addresses.
        private async void GetPlayerSkillAddresses_Button_Click(object sender, EventArgs e)
        {
            #region Toggle Controls

            // Reset player address to null each change.
            MainForm.AoBScanResultsSkills = null;

            // Unhide progressbar.
            PlayerSkill_ProgressBar.Visible = true;

            // Disable controls.
            GetPlayerSkillAddresses_Button.Enabled = false;

            ChangeSkills_Button.Enabled = false;
            MaxAllSkills_Button.Enabled = false;
            ResetAllSkills_Button.Enabled = false;
            UseSkillAddress_Button.Enabled = false;
            PreviousSkillAddress_Button.Enabled = false;
            NextSkillAddress_Button.Enabled = false;

            Skill0_Panel.Enabled = false;
            Skill1_Panel.Enabled = false;
            Skill2_Panel.Enabled = false;
            Skill3_Panel.Enabled = false;
            Skill4_Panel.Enabled = false;
            Skill5_Panel.Enabled = false;
            Skill6_Panel.Enabled = false;
            Skill7_Panel.Enabled = false;
            Skill8_Panel.Enabled = false;
            Skill9_Panel.Enabled = false;
            Skill10_Panel.Enabled = false;
            Skill11_Panel.Enabled = false;

            SetReadOnly(Skill0_Panel, true);
            SetReadOnly(Skill1_Panel, true);
            SetReadOnly(Skill2_Panel, true);
            SetReadOnly(Skill3_Panel, true);
            SetReadOnly(Skill4_Panel, true);
            SetReadOnly(Skill5_Panel, true);
            SetReadOnly(Skill6_Panel, true);
            SetReadOnly(Skill7_Panel, true);
            SetReadOnly(Skill8_Panel, true);
            SetReadOnly(Skill9_Panel, true);
            SetReadOnly(Skill10_Panel, true);
            SetReadOnly(Skill11_Panel, true);

            UseEXPValues_CheckBox.Enabled = false;

            #endregion

            // Scan for address is needed.
            if (MemLib.OpenProcess("CoreKeeper") && MainForm.AoBScanResultsSkills == null)
            {
                // Adjust progressbar.
                PlayerSkill_ProgressBar.Value = 10;
                PlayerSkill_ProgressBar.Visible = true;

                // Helper for progressbar.
                int scanAmounts = 2;

                // Ex: 1 0 2 3 4 5 6 7 8 9 10
                #region Scan Run #1

                // Define the AoB and scan it into an array list.
                // Depreciated Address 11Mar25: 02 00 00 00 ?? ?? ?? ?? 03 00 00 00 ?? ?? ?? ?? 04 00 00 00 ?? ?? ?? ?? 05 00 00 00 ?? ?? ?? ?? 06 00 00 00 ?? ?? ?? ?? 07 00 00 00 ?? ?? ?? ?? 08 00 00 00 ?? ?? ?? ?? ?? 00 00 00 ?? ?? ?? ?? ?? 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00
                string AoB = "02 00 00 00 ?? ?? ?? ?? 03 00 00 00 ?? ?? ?? ?? 04 00 00 00 ?? ?? ?? ?? ?? 00 00 00 ?? ?? ?? ?? ?? 00 00 00 ?? ?? ?? ?? ?? 00 00 00 ?? ?? ?? ?? ?? 00 00 00 ?? ?? ?? ?? ?? 00 00 00 ?? ?? ?? ?? ?? 00 00 00 ?? ?? ?? ?? ?? 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00";

                // Define new array for holding the scan results.
                List<long> AoBScanResultsTemp = (await MemLib.AoBScan(AoB, true, true)).ToList();

                if (AoBScanResultsTemp.Count() == 0)
                {
                    MessageBox.Show("Could not find any skill addresses!\nTry restarting your game!", "Player Skill Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Enable controls.
                    GetPlayerSkillAddresses_Button.Enabled = true;
                    UseEXPValues_CheckBox.Enabled = true;

                    PlayerSkill_ProgressBar.Value = 0;
                    return;
                }

                // Adjust progressbar.
                PlayerSkill_ProgressBar.Value = 20;
                PlayerSkill_ProgressBar.Step = (70 / scanAmounts) / AoBScanResultsTemp.Count();

                // Ensure we load the list var.
                // C# 8.0+: MainForm.AoBScanResultsSkills ??= new List<long>();
                if (MainForm.AoBScanResultsSkills == null)
                    MainForm.AoBScanResultsSkills = new List<long>();

                // Iterate through all addresses and check for a valid byte. Filter out non-valid addresses.
                foreach (long res in new List<long>(AoBScanResultsTemp))
                {
                    // Perform step.
                    PlayerSkill_ProgressBar.PerformStep();

                    // Backup 6 & 8 bytes [24, 32] and look for either 10, 11 or 12.
                    string skillAddress = BigInteger.Subtract(BigInteger.Parse(res.ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("32", NumberStyles.Integer)).ToString("X"); // 8 bytes.
                    int skillAddressValue = MemLib.ReadInt(skillAddress);

                    // Check for correct values.
                    if (skillAddressValue < 10 || skillAddressValue > 16)
                    {
                        skillAddress = BigInteger.Subtract(BigInteger.Parse(res.ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("24", NumberStyles.Integer)).ToString("X");    // 6 bytes.
                        skillAddressValue = MemLib.ReadInt(skillAddress);

                        if (skillAddressValue < 10 || skillAddressValue > 16)
                        {
                            // Value not found, remove this from the list.
                            AoBScanResultsTemp.Remove(res);

                            // No value found. Skip.
                            continue;
                        }
                    }

                    // Debug
                    // MessageBox.Show(Convert.ToInt64(skillAddress, 16).ToString());

                    // Add the valid address to the list (convert hex string to long properly)
                    MainForm.AoBScanResultsSkills.Add(Convert.ToInt64(skillAddress, 16));
                }
                #endregion

                // Ex: 9 1 0 2 3 4 5 6 7 8 10
                #region Scan Run #2

                // Define the AoB and scan it into an array list.
                // Depreciated Address 11Mar25: 02 00 00 00 ?? ?? ?? ?? 03 00 00 00 ?? ?? ?? ?? 04 00 00 00 ?? ?? ?? ?? 05 00 00 00 ?? ?? ?? ?? 06 00 00 00 ?? ?? ?? ?? 07 00 00 00 ?? ?? ?? ?? 08 00 00 00 ?? ?? ?? ?? ?? 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00
                AoB = "02 00 00 00 ?? ?? ?? ?? 03 00 00 00 ?? ?? ?? ?? 04 00 00 00 ?? ?? ?? ?? ?? 00 00 00 ?? ?? ?? ?? ?? 00 00 00 ?? ?? ?? ?? ?? 00 00 00 ?? ?? ?? ?? ?? 00 00 00 ?? ?? ?? ?? ?? 00 00 00 ?? ?? ?? ?? ?? 00 00 00 ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? ?? 00 00 00 00 00 00 00 00";

                // Define new array for holding the scan results.
                AoBScanResultsTemp.Clear();
                AoBScanResultsTemp = (await MemLib.AoBScan(AoB, true, true)).ToList();

                if (AoBScanResultsTemp.Count() == 0)
                {
                    MessageBox.Show("Could not find any skill addresses!\nTry restarting your game!", "Player Skill Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Enable controls.
                    GetPlayerSkillAddresses_Button.Enabled = true;
                    UseEXPValues_CheckBox.Enabled = true;

                    PlayerSkill_ProgressBar.Value = 0;
                    return;
                }

                // Adjust progressbar.
                // progressBar2.Value = 20;
                PlayerSkill_ProgressBar.Step = (70 / scanAmounts) / AoBScanResultsTemp.Count();

                // Ensure we load the list var.
                // MainForm.AoBScanResultsSkills ??= new List<long>();

                // Iterate through all addresses and check for a valid byte. Filter out non-valid addresses.
                foreach (long res in new List<long>(AoBScanResultsTemp))
                {
                    // Perform step.
                    PlayerSkill_ProgressBar.PerformStep();

                    // Backup 6 & 8 bytes [24, 32] and look for either 10, 11 or 12.
                    string skillAddress = BigInteger.Subtract(BigInteger.Parse(res.ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("32", NumberStyles.Integer)).ToString("X"); // 8 bytes.
                    int skillAddressValue = MemLib.ReadInt(skillAddress);

                    // Check for correct values.
                    if (skillAddressValue < 10 || skillAddressValue > 16)
                    {
                        skillAddress = BigInteger.Subtract(BigInteger.Parse(res.ToString("X"), NumberStyles.HexNumber), BigInteger.Parse("24", NumberStyles.Integer)).ToString("X");    // 6 bytes.
                        skillAddressValue = MemLib.ReadInt(skillAddress);

                        if (skillAddressValue < 10 || skillAddressValue > 16)
                        {
                            // Value not found, remove this from the list.
                            AoBScanResultsTemp.Remove(res);

                            // No value found. Skip.
                            continue;
                        }
                    }

                    // Debug
                    // MessageBox.Show(Convert.ToInt64(skillAddress, 16).ToString());

                    // Add the valid address to the list (convert hex string to long properly)
                    MainForm.AoBScanResultsSkills.Add(Convert.ToInt64(skillAddress, 16));
                }
                #endregion

                // Check if any results where found.
                if (AoBScanResultsTemp.Count() == 0)
                {
                    MessageBox.Show("Of the found skill addresses, none where valid!\nTry restarting your game!", "Player Skill Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Enable controls.
                    GetPlayerSkillAddresses_Button.Enabled = true;

                    PlayerSkill_ProgressBar.Value = 0;
                    return;
                }

                // Adjust progressbar.
                PlayerSkill_ProgressBar.Value = 100;

                // Update richtextbox with found addresses.
                foreach (long res in MainForm.AoBScanResultsSkills)
                {
                    if (PlayerSkill_RichTextBox.Text == "Addresses Loaded: 0")
                    {
                        PlayerSkill_RichTextBox.Text = "Player Skills Loaded: " + MainForm.AoBScanResultsSkills.Count().ToString() + " [" + res.ToString("X").ToString();
                    }
                    else
                    {
                        PlayerSkill_RichTextBox.Text += ", " + res.ToString("X").ToString();
                    }
                }
                PlayerSkill_RichTextBox.Text += "]";

                // Hide progressbar.
                PlayerSkill_ProgressBar.Visible = false;

                // Enable controls.
                GetPlayerSkillAddresses_Button.Enabled = true;
                UseSkillAddress_Button.Enabled = true;

                Skill0_Panel.Enabled = true;
                Skill1_Panel.Enabled = true;
                Skill2_Panel.Enabled = true;
                Skill3_Panel.Enabled = true;
                Skill4_Panel.Enabled = true;
                Skill5_Panel.Enabled = true;
                Skill6_Panel.Enabled = true;
                Skill7_Panel.Enabled = true;
                Skill8_Panel.Enabled = true;
                Skill9_Panel.Enabled = true;
                Skill10_Panel.Enabled = true;
                Skill11_Panel.Enabled = true;

                UseEXPValues_CheckBox.Enabled = true;

                // Check if results are greater then one.
                if (MainForm.AoBScanResultsSkills.Count() > 1)
                {
                    // Enable controls.
                    PreviousSkillAddress_Button.Enabled = true;
                    NextSkillAddress_Button.Enabled = true;
                }
                else
                {
                    // Disable controls.
                    PreviousSkillAddress_Button.Enabled = false;
                    NextSkillAddress_Button.Enabled = false;
                }

                // Load skills.
                UpdateSkills();
            }
            else
            {
                // Adjust progressbar.
                PlayerSkill_ProgressBar.Step = 0;
                PlayerSkill_ProgressBar.Value = 0;
            }
        }
        #endregion

        #region Forward & Backward Buttons

        // Previous address button.
        private void PreviousSkillAddress_Button_Click(object sender, EventArgs e)
        {
            // Reset progress bar.
            PlayerSkill_ProgressBar.Value = 0;

            // Subtract from the use address if its not one.
            useAddress = (useAddress == 1) ? 1 : useAddress - 1;

            // Update the rich textbox.
            PlayerSkill_RichTextBox.Text = "Addresses Loaded: 0";
            foreach (long res in MainForm.AoBScanResultsSkills)
            {
                if (PlayerSkill_RichTextBox.Text == "Addresses Loaded: 0")
                {
                    PlayerSkill_RichTextBox.Text = "Addresses Loaded: " + MainForm.AoBScanResultsSkills.Count().ToString() + ", Selected: " + useAddress + ", [" + res.ToString("X").ToString();
                }
                else
                {
                    PlayerSkill_RichTextBox.Text += ", " + res.ToString("X").ToString();
                }
            }
            PlayerSkill_RichTextBox.Text += "]";

            // Load skills.
            UpdateSkills();
        }

        // Next address button.
        private void NextSkillAddress_Button_Click(object sender, EventArgs e)
        {
            // Reset progress bar.
            PlayerSkill_ProgressBar.Value = 0;

            // Add to the use address if its not the max.
            useAddress = (MainForm.AoBScanResultsSkills != null && useAddress == MainForm.AoBScanResultsSkills.Count()) ? MainForm.AoBScanResultsSkills.Count() : useAddress + 1;

            // Update the rich textbox.
            PlayerSkill_RichTextBox.Text = "Addresses Loaded: 0";
            foreach (long res in MainForm.AoBScanResultsSkills)
            {
                if (PlayerSkill_RichTextBox.Text == "Addresses Loaded: 0")
                {
                    PlayerSkill_RichTextBox.Text = "Addresses Loaded: " + MainForm.AoBScanResultsSkills.Count().ToString() + ", Selected: " + useAddress + ", [" + res.ToString("X").ToString();
                }
                else
                {
                    PlayerSkill_RichTextBox.Text += ", " + res.ToString("X").ToString();
                }
            }
            PlayerSkill_RichTextBox.Text += "]";

            // Load skills.
            UpdateSkills();
        }
        #endregion

        #region Scan Address For Current Skill Layout

        // Get all addresses for the selected skill layout.
        private async void UseSkillAddress_Button_Click(object sender, EventArgs e)
        {
            // Reset player address to null each change.
            MainForm.AoBScanResultsSkillLoadout = null;

            // Unhide progressbar.
            PlayerSkill_ProgressBar.Visible = true;

            // Disable controls.
            UseSkillAddress_Button.Enabled = false;

            ChangeSkills_Button.Enabled = false;
            MaxAllSkills_Button.Enabled = false;
            ResetAllSkills_Button.Enabled = false;
            GetPlayerSkillAddresses_Button.Enabled = false;
            PreviousSkillAddress_Button.Enabled = false;
            NextSkillAddress_Button.Enabled = false;

            Skill0_Panel.Enabled = false;
            Skill1_Panel.Enabled = false;
            Skill2_Panel.Enabled = false;
            Skill3_Panel.Enabled = false;
            Skill4_Panel.Enabled = false;
            Skill5_Panel.Enabled = false;
            Skill6_Panel.Enabled = false;
            Skill7_Panel.Enabled = false;
            Skill8_Panel.Enabled = false;
            Skill9_Panel.Enabled = false;
            Skill10_Panel.Enabled = false;
            Skill11_Panel.Enabled = false;

            UseEXPValues_CheckBox.Enabled = false;

            // Scan for address is needed.
            if (MemLib.OpenProcess("CoreKeeper") && MainForm.AoBScanResultsSkillLoadout == null)
            {
                // Ensure selected skill levels are not all level 0.
                if ((int)SkillILvL0_NumericUpDown.Value == 0 && (int)SkillILvL1_NumericUpDown.Value == 0 && (int)SkillILvL2_NumericUpDown.Value == 0 && (int)SkillILvL3_NumericUpDown.Value == 0 && (int)SkillILvL4_NumericUpDown.Value == 0 && (int)SkillILvL5_NumericUpDown.Value == 0 &&
                    (int)SkillILvL6_NumericUpDown.Value == 0 && (int)SkillILvL7_NumericUpDown.Value == 0 && (int)SkillILvL8_NumericUpDown.Value == 0 && (int)SkillILvL9_NumericUpDown.Value == 0 && (int)SkillILvL10_NumericUpDown.Value == 0 && (int)SkillILvL11_NumericUpDown.Value == 0 &&
                    MessageBox.Show("It's not advised to 'use' a skill loadout that is all level 0.\nDoing this will lead to very long scan times.\n\nIt's recommended to get at least one or more skills to level 1 or higher.\n\nContinue?", "Player Skill Editor - Use selected skill loadout?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    #region Toggle Controls

                    // Hide progressbar.
                    PlayerSkill_ProgressBar.Visible = false;

                    // Enable controls.
                    PreviousSkillAddress_Button.Enabled = true;
                    NextSkillAddress_Button.Enabled = true;

                    // Enable controls.
                    GetPlayerSkillAddresses_Button.Enabled = true;
                    UseSkillAddress_Button.Enabled = true;

                    Skill0_Panel.Enabled = true;
                    Skill1_Panel.Enabled = true;
                    Skill2_Panel.Enabled = true;
                    Skill3_Panel.Enabled = true;
                    Skill4_Panel.Enabled = true;
                    Skill5_Panel.Enabled = true;
                    Skill6_Panel.Enabled = true;
                    Skill7_Panel.Enabled = true;
                    Skill8_Panel.Enabled = true;
                    Skill9_Panel.Enabled = true;
                    Skill10_Panel.Enabled = true;
                    Skill11_Panel.Enabled = true;

                    // Enable controls.
                    UseEXPValues_CheckBox.Enabled = true;

					// Return.
					return;

                    #endregion
                }

                // Adjust progressbar.
                PlayerSkill_ProgressBar.Value = 30;
                PlayerSkill_ProgressBar.Visible = true;

                #region Extract The Skill ID & Value From Stored Address

                // Select the inventory to use.
                var res = MainForm.AoBScanResultsSkills.ElementAt(useAddress - 1);

                string headerBase = res.ToString("X");
                // int addressOffset = res.Item2;

                // Parse header base once.
                BigInteger baseAddress = BigInteger.Parse(headerBase, NumberStyles.HexNumber);

                int skillCount = 12;
                int[] skillIDs = new int[skillCount];
                int[] skillValues = new int[skillCount];

                // string test = "";

                // Iterate through all the skills.
                for (int i = 0; i < skillCount; i++)
                {
                    int idOffset = 8 + (i * 8);     // ID is at base offset + (index * 8)
                    int valueOffset = idOffset + 4; // Value is always 4 bytes after ID

                    skillIDs[i] = MemLib.ReadInt(BigInteger.Add(baseAddress, idOffset).ToString("X"));
                    skillValues[i] = MemLib.ReadInt(BigInteger.Add(baseAddress, valueOffset).ToString("X"));

                    // test += " " + string.Join(" ", BitConverter.GetBytes(skillIDs[i]).Select(b => b.ToString("X2"))) + " " + string.Join(" ", BitConverter.GetBytes(skillValues[i]).Select(b => b.ToString("X2")));
                }

                // Debug.
                // Clipboard.SetText(test);
                #endregion

                // Adjust progressbar.
                PlayerSkill_ProgressBar.Value = 60;

                #region Rebuild Array Based On Ascending Skill IDs

                // Build array.
                // Array to hold skill values in correct order.
                int[] orderedValues = new int[skillCount];

                // Populate orderedValues using skillIDs as the index.
                for (int i = 0; i < skillCount; i++)
                {
                    int skillID = skillIDs[i];
                    if (skillID >= 0 && skillID < skillCount) // Ensure it's within range.
                    {
                        orderedValues[skillID] = skillValues[i];
                    }
                }

                // Convert to 4-byte hex representation.
                string AoB = string.Join(" ", orderedValues.Select(v =>
                    string.Join(" ", BitConverter.GetBytes(v).Select(b => b.ToString("X2")))
                ));

                // Debug.
                // Clipboard.SetText(AoB);
                #endregion

                // Define new array for holding the scan results.
                MainForm.AoBScanResultsSkillLoadout = (await MemLib.AoBScan(AoB, true, true)).ToList();

                // Check if any results where found.
                if (MainForm.AoBScanResultsSkillLoadout.Count() == 0)
                {
                    MessageBox.Show("Of the loaded skill addresses, no skill loadouts where found!\nTry a different skill address or try restarting your game!", "Player Skill Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    // Enable controls.
                    GetPlayerSkillAddresses_Button.Enabled = true;

                    PlayerSkill_ProgressBar.Value = 0;
                    return;
                }

                // Adjust progressbar.
                PlayerSkill_ProgressBar.Value = 100;

                // Update richtextbox with found addresses.
                PlayerSkill_RichTextBox.Text = "Addresses Loaded: 0";
                foreach (long res2 in MainForm.AoBScanResultsSkillLoadout)
                {
                    if (PlayerSkill_RichTextBox.Text == "Addresses Loaded: 0")
                    {
                        PlayerSkill_RichTextBox.Text = "Skill Loadouts Loaded: " + MainForm.AoBScanResultsSkillLoadout.Count().ToString() + " [" + res2.ToString("X").ToString();
                    }
                    else
                    {
                        PlayerSkill_RichTextBox.Text += ", " + res2.ToString("X").ToString();
                    }
                }
                PlayerSkill_RichTextBox.Text += "]";

                #region Toggle Controls

                // Hide progressbar.
                PlayerSkill_ProgressBar.Visible = false;

                // Enable controls.
                // Check if results are greater then one.
                if (MainForm.AoBScanResultsSkills.Count() > 1)
                {
                    // Enable controls.
                    PreviousSkillAddress_Button.Enabled = true;
                    NextSkillAddress_Button.Enabled = true;
                }
                else
                {
                    // Disable controls.
                    PreviousSkillAddress_Button.Enabled = false;
                    NextSkillAddress_Button.Enabled = false;
                }

                // Enable controls.
                GetPlayerSkillAddresses_Button.Enabled = true;
                UseSkillAddress_Button.Enabled = true;

                Skill0_Panel.Enabled = true;
                Skill1_Panel.Enabled = true;
                Skill2_Panel.Enabled = true;
                Skill3_Panel.Enabled = true;
                Skill4_Panel.Enabled = true;
                Skill5_Panel.Enabled = true;
                Skill6_Panel.Enabled = true;
                Skill7_Panel.Enabled = true;
                Skill8_Panel.Enabled = true;
                Skill9_Panel.Enabled = true;
                Skill10_Panel.Enabled = true;
                Skill11_Panel.Enabled = true;

                SetReadOnly(Skill0_Panel, false);
                SetReadOnly(Skill1_Panel, false);
                SetReadOnly(Skill2_Panel, false);
                SetReadOnly(Skill3_Panel, false);
                SetReadOnly(Skill4_Panel, false);
                SetReadOnly(Skill5_Panel, false);
                SetReadOnly(Skill6_Panel, false);
                SetReadOnly(Skill7_Panel, false);
                SetReadOnly(Skill8_Panel, false);
                SetReadOnly(Skill9_Panel, false);
                SetReadOnly(Skill10_Panel, false);
                SetReadOnly(Skill11_Panel, false);

                // Enable controls.
                ChangeSkills_Button.Enabled = true;
                MaxAllSkills_Button.Enabled = true;
                ResetAllSkills_Button.Enabled = true;

                UseEXPValues_CheckBox.Enabled = true;

                #endregion

                #region Congrats

                // Display message to the user to head back to the main menu.
                MessageBox.Show("Congratulations. Your skills where loaded successfully!\n\n" +
                    "Please save and exit to the main menu before editing any skills!\n\n" +
                    "(Not exiting to the main menu will cause skills not to stick and will revert back to their original values!)\n\n" +
                    "Once completed, load back into a world and save.", 
                    "Player Skill Editor", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                #endregion
            }
            else
            {
                // Adjust progressbar.
                PlayerSkill_ProgressBar.Step = 0;
                PlayerSkill_ProgressBar.Value = 0;
            }
        }
        #endregion

        #endregion

        #region Helpers

        #region Get Numeric Maximum

        // Helper for getting the raw skill EXP regardless of format.
        public int GetNumericMaximum(int skillId)
        {
            // Check if "Use EXP values" is enabled.
            if (!UseEXPValues_CheckBox.Checked)
            {
                return 100; // Default max level if EXP values are not used.
            }

            // Validate that the skill ID is within the SkillID enum.
            if (Enum.IsDefined(typeof(SkillID), skillId))
            {
                SkillID skillIDEnum = (SkillID)skillId;

                // Get the official max level for this skill.
                int maxLevel = GetMaxSkillLevel(skillIDEnum);

                // Convert the max level to the raw EXP value.
                return GetSkillFromLevel(skillIDEnum, maxLevel);
            }

            return 100; // Default value if skill ID is invalid.
        }
        #endregion

        #region Get Converted Values

        // Helper for getting the raw skill exp regardless of format.
        // COMPLETED: Change the rounding functions to represent the actual in-game values.
        public int GetConvertedValues(int skillId, int currentValue, bool forceConvert = false, bool upConvert = false)
        {
            // If using raw EXP values, just return the current value.
            if (UseEXPValues_CheckBox.Checked && !forceConvert)
            {
                return currentValue;
            }

            // Convert using the official conversion methods.
            SkillID officialSkillId = (SkillID)skillId;
            if (upConvert)
            {
                // Convert from in-game level to raw EXP.
                return GetSkillFromLevel(officialSkillId, currentValue);
            }
            else
            {
                // Convert from raw EXP to in-game level.
                return GetLevelFromSkill(officialSkillId, currentValue);
            }
        }
        #endregion

        #region Update Skills

        // Function for updating the GUI with the skill data.
        public bool warnUser = false;
        public int warnUserCount = 0;
        public void UpdateSkills()
        {
            // Ensure the scan results are populated. (errors should never happen)
            if (MainForm.AoBScanResultsSkills == null)
            {
                // Display error.
                MessageBox.Show("You need to first scan for the Skills addresses!", "Player Skill Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Since we're changing addresses, make sure we require a rescan!
            if (MainForm.AoBScanResultsSkillLoadout != null)
            {
                MainForm.AoBScanResultsSkillLoadout = null;

                // Disable some controls.
                ChangeSkills_Button.Enabled = false;
                MaxAllSkills_Button.Enabled = false;
                ResetAllSkills_Button.Enabled = false;

                // checkBox1.Enabled = false;
            }

            // Disable controls.
            GetPlayerSkillAddresses_Button.Enabled = false;
            UseSkillAddress_Button.Enabled = false;
            PreviousSkillAddress_Button.Enabled = false;
            NextSkillAddress_Button.Enabled = false;

            // Select the inventory to use.
            var res = MainForm.AoBScanResultsSkills.ElementAt(useAddress - 1);

            string headerBase = res.ToString("X");
            // int addressOffset = res.Item2;

            // Parse header base once.
            BigInteger baseAddress = BigInteger.Parse(headerBase, NumberStyles.HexNumber);

            int skillCount = 12;
            int[] skillIDs = new int[skillCount];
            int[] skillValues = new int[skillCount];

            // Iterate through all the skills.
            for (int i = 0; i < skillCount; i++)
            {
                int idOffset = 8 + (i * 8);     // ID is at base offset + (index * 8)
                int valueOffset = idOffset + 4; // Value is always 4 bytes after ID

                skillIDs[i] = MemLib.ReadInt(BigInteger.Add(baseAddress, idOffset).ToString("X"));
                skillValues[i] = MemLib.ReadInt(BigInteger.Add(baseAddress, valueOffset).ToString("X"));

                // Debug
                // MessageBox.Show("RAW: ID: " + skillIDs[i].ToString() + ", VAL: " + skillValues[i] + " | MOD: " + GetConvertedValues(skillIDs[i], skillValues[i]).ToString());
            }

            // Filter out invalid addresses.
            // This may cause a higher performance impact.
            #region Filter Invalid Addresses

            // Iterate through all the addresses.
            foreach (long res2 in MainForm.AoBScanResultsSkills)
            {
                string headerBaseTest = res2.ToString("X");
                // int addressOffset = res.Item2;

                // Parse header base once.
                BigInteger baseAddressTest = BigInteger.Parse(headerBaseTest, NumberStyles.HexNumber);

                int[] skillIDsTest = new int[skillCount];

                // Iterate through all the skills.
                for (int i = 0; i < skillCount; i++)
                {
                    int idOffsetTest = 8 + (i * 8);         // ID is at base offset + (index * 8)

                    skillIDsTest[i] = MemLib.ReadInt(BigInteger.Add(baseAddressTest, idOffsetTest).ToString("X"));
                }

                HashSet<int> seenIDs = new HashSet<int>();
                bool hasDuplicates = false;
                bool hasOutOfRange = false;

                for (int i = 0; i < skillCount; i++)
                {
                    int skillID = skillIDsTest[i];

                    // Check for duplicates
                    if (!seenIDs.Add(skillID))
                    {
                        hasDuplicates = true;
                    }

                    // Check if out of range
                    if (skillID < 0 || skillID >= skillCount)
                    {
                        hasOutOfRange = true;
                    }
                }

                // Check for errors.
                if (hasDuplicates || hasOutOfRange)
                {
                    // Change the warn user bool.
                    warnUserCount++;
                    warnUser = true;

                    // Check if this is the only address.
                    if (MainForm.AoBScanResultsSkills.Count() > 1)
                    {
                        // Other addresses exist, remove this one & switch to the next one.

                        // Remove address.
                        MainForm.AoBScanResultsSkills.Remove(res2);

                        // Reset the use address.
                        useAddress = 1;

                        // Update the richtextbox.
                        // Update richtextbox with found addresses.
                        PlayerSkill_RichTextBox.Text = "Addresses Loaded: 0";
                        foreach (long res3 in MainForm.AoBScanResultsSkills)
                        {
                            if (PlayerSkill_RichTextBox.Text == "Addresses Loaded: 0")
                            {
                                PlayerSkill_RichTextBox.Text = "Skill Loadouts Loaded: " + MainForm.AoBScanResultsSkills.Count().ToString() + ", Selected: " + useAddress + ", [" + res3.ToString("X").ToString();
                            }
                            else
                            {
                                PlayerSkill_RichTextBox.Text += ", " + res3.ToString("X").ToString();
                            }
                        }
                        PlayerSkill_RichTextBox.Text += "]";

                        // Warn user.
                        // MessageBox.Show("Found skill values out of range or duplicated. This typically means it was a bad address.\n\n" +
                        //    "- These addresses where automatically removed!", "Player Skill Editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        // Reload the next address.
                        UpdateSkills();

                        return;
                    }
                    else
                    {
                        // This was the only address! Not good.

                        // Warn user.
                        // MessageBox.Show("Found skill values out of range or duplicated. This typically means it was a bad address.\n\n" +
                        //    "- These addresses where automatically removed!", "Player Skill Editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        MessageBox.Show("Something went wrong. No valid addresses remaining!\n\n" +
                            "Please try reloading the game.", "Player Skill Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        #region Toggle Controls

                        // Reset player address to null each change.
                        MainForm.AoBScanResultsSkills = null;
                        MainForm.AoBScanResultsSkillLoadout = null;

                        // Unhide progressbar.
                        PlayerSkill_ProgressBar.Value = 0;
                        PlayerSkill_ProgressBar.Visible = true;

                        // Disable controls.
                        GetPlayerSkillAddresses_Button.Enabled = true; // Enable this control.

                        ChangeSkills_Button.Enabled = false;
                        MaxAllSkills_Button.Enabled = false;
                        ResetAllSkills_Button.Enabled = false;
                        UseSkillAddress_Button.Enabled = false;
                        PreviousSkillAddress_Button.Enabled = false;
                        NextSkillAddress_Button.Enabled = false;

                        Skill0_Panel.Enabled = false;
                        Skill1_Panel.Enabled = false;
                        Skill2_Panel.Enabled = false;
                        Skill3_Panel.Enabled = false;
                        Skill4_Panel.Enabled = false;
                        Skill5_Panel.Enabled = false;
                        Skill6_Panel.Enabled = false;
                        Skill7_Panel.Enabled = false;
                        Skill8_Panel.Enabled = false;
                        Skill9_Panel.Enabled = false;
                        Skill10_Panel.Enabled = false;

                        UseEXPValues_CheckBox.Enabled = false;

                        #endregion

                        return;
                    }
                }
            }

            // Warn user about any removed addresses.
            if (warnUser)
            {
                // Warn user.
                MessageBox.Show("Found [" + warnUserCount + "] skill values out of range or duplicated.\n" +
                    "(This typically means it was a bad address).\n\n" +
                    "- These addresses where automatically removed!", "Player Skill Editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                // Reset vars.
                warnUserCount = 0;
                warnUser = false;
            }
            #endregion

            // Catch any bad addresses.
            try
            {
                // Set the form control data.
                #region Controls

                SkillID0_NumericUpDown.Value = (decimal)skillIDs[0];
                SkillILvL0_NumericUpDown.Value = (decimal)GetConvertedValues((int)SkillID0_NumericUpDown.Value, skillValues[0]);

                SkillID1_NumericUpDown.Value = (decimal)skillIDs[1];
                SkillILvL1_NumericUpDown.Value = (decimal)GetConvertedValues((int)SkillID1_NumericUpDown.Value, skillValues[1]);

                SkillID2_NumericUpDown.Value = (decimal)skillIDs[2];
                SkillILvL2_NumericUpDown.Value = (decimal)GetConvertedValues((int)SkillID2_NumericUpDown.Value, skillValues[2]);

                SkillID3_NumericUpDown.Value = (decimal)skillIDs[3];
                SkillILvL3_NumericUpDown.Value = (decimal)GetConvertedValues((int)SkillID3_NumericUpDown.Value, skillValues[3]);

                SkillID4_NumericUpDown.Value = (decimal)skillIDs[4];
                SkillILvL4_NumericUpDown.Value = (decimal)GetConvertedValues((int)SkillID4_NumericUpDown.Value, skillValues[4]);

                SkillID5_NumericUpDown.Value = (decimal)skillIDs[5];
                SkillILvL5_NumericUpDown.Value = (decimal)GetConvertedValues((int)SkillID5_NumericUpDown.Value, skillValues[5]);

                SkillID6_NumericUpDown.Value = (decimal)skillIDs[6];
                SkillILvL6_NumericUpDown.Value = (decimal)GetConvertedValues((int)SkillID6_NumericUpDown.Value, skillValues[6]);

                SkillID7_NumericUpDown.Value = (decimal)skillIDs[7];
                SkillILvL7_NumericUpDown.Value = (decimal)GetConvertedValues((int)SkillID7_NumericUpDown.Value, skillValues[7]);

                SkillID8_NumericUpDown.Value = (decimal)skillIDs[8];
                SkillILvL8_NumericUpDown.Value = (decimal)GetConvertedValues((int)SkillID8_NumericUpDown.Value, skillValues[8]);

                SkillID9_NumericUpDown.Value = (decimal)skillIDs[9];
                SkillILvL9_NumericUpDown.Value = (decimal)GetConvertedValues((int)SkillID9_NumericUpDown.Value, skillValues[9]);

                SkillID10_NumericUpDown.Value = (decimal)skillIDs[10];
                SkillILvL10_NumericUpDown.Value = (decimal)GetConvertedValues((int)SkillID10_NumericUpDown.Value, skillValues[10]);

                SkillID11_NumericUpDown.Value = (decimal)skillIDs[11];
                SkillILvL11_NumericUpDown.Value = (decimal)GetConvertedValues((int)SkillID11_NumericUpDown.Value, skillValues[11]);

                #endregion
            }
            catch (Exception)
            {
                #region Remove Bad Addresses

                // Remove bad address.
                //
                // Check if this is the only address.
                if (MainForm.AoBScanResultsSkills.Count() > 1)
                {
                    // Other addresses exist, remove this one & switch to the next one.

                    // Remove address.
                    MainForm.AoBScanResultsSkills.Remove(res);

                    // Reset the use address.
                    useAddress = 1;

                    // Update the richtextbox.
                    // Update richtextbox with found addresses.
                    PlayerSkill_RichTextBox.Text = "Addresses Loaded: 0";
                    foreach (long res2 in MainForm.AoBScanResultsSkills)
                    {
                        if (PlayerSkill_RichTextBox.Text == "Addresses Loaded: 0")
                        {
                            PlayerSkill_RichTextBox.Text = "Skill Loadouts Loaded: " + MainForm.AoBScanResultsSkills.Count().ToString() + ", Selected: " + useAddress + ", [" + res2.ToString("X").ToString();
                        }
                        else
                        {
                            PlayerSkill_RichTextBox.Text += ", " + res2.ToString("X").ToString();
                        }
                    }
                    PlayerSkill_RichTextBox.Text += "]";

                    // Warn user.
                    MessageBox.Show("A skill value was out of range. This typically means it was a bad address.\n\n" +
                        "- The address was automatically removed!", "Player Skill Editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    // Reload the next address.
                    UpdateSkills();

                    return;
                }
                else
                {
                    // This was the only address! Not good.

                    // Warn user.
                    MessageBox.Show("A skill value was out of range. This typically means it was a bad address.\n\n" +
                        "- The address was automatically removed!", "Player Skill Editor", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    MessageBox.Show("Something went wrong. No valid addresses remaining!\n\n" +
                        "Please try reloading the game.", "Player Skill Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    #region Toggle Controls

                    // Reset player address to null each change.
                    MainForm.AoBScanResultsSkills = null;
                    MainForm.AoBScanResultsSkillLoadout = null;

                    // Unhide progressbar.
                    PlayerSkill_ProgressBar.Value = 0;
                    PlayerSkill_ProgressBar.Visible = true;

                    // Disable controls.
                    GetPlayerSkillAddresses_Button.Enabled = true; // Enable this control.

                    ChangeSkills_Button.Enabled = false;
                    MaxAllSkills_Button.Enabled = false;
                    ResetAllSkills_Button.Enabled = false;
                    UseSkillAddress_Button.Enabled = false;
                    PreviousSkillAddress_Button.Enabled = false;
                    NextSkillAddress_Button.Enabled = false;

                    Skill0_Panel.Enabled = false;
                    Skill1_Panel.Enabled = false;
                    Skill2_Panel.Enabled = false;
                    Skill3_Panel.Enabled = false;
                    Skill4_Panel.Enabled = false;
                    Skill5_Panel.Enabled = false;
                    Skill6_Panel.Enabled = false;
                    Skill7_Panel.Enabled = false;
                    Skill8_Panel.Enabled = false;
                    Skill9_Panel.Enabled = false;
                    Skill10_Panel.Enabled = false;
                    Skill11_Panel.Enabled = false;

                    UseEXPValues_CheckBox.Enabled = false;

                    #endregion

                    return;
                }
                #endregion
            }

            // Enable controls.
            GetPlayerSkillAddresses_Button.Enabled = true;
            UseSkillAddress_Button.Enabled = true;

            // Enable controls.
            // Check if results are greater then one.
            if (MainForm.AoBScanResultsSkills.Count() > 1)
            {
                // Enable controls.
                PreviousSkillAddress_Button.Enabled = true;
                NextSkillAddress_Button.Enabled = true;
            }
            else
            {
                // Disable controls.
                PreviousSkillAddress_Button.Enabled = false;
                NextSkillAddress_Button.Enabled = false;
            }
        }
        #endregion

        #region Skill Extensions

        private readonly float MiningMulFactor = 1.039572f;

        private readonly int MiningBase = 50;

        private readonly float runningMulFactor = 1.0494f;

        private readonly int runningBase = 200;

        private readonly float meleeMulFactor = 1.02382f;

        private readonly int meleeBase = 50;

        private readonly float vitalityMulFactor = 1.04943f;

        private readonly int vitalityBase = 2000;

        private readonly float craftingMulFactor = 1.03706f;

        private readonly int craftingBase = 30;

        private readonly float rangeMulFactor = 1.02382f;

        private readonly int rangeBase = 50;

        private readonly float gardeningMulFactor = 1.02526f;

        private readonly int gardeningBase = 15;

        private readonly float fishingMulFactor = 1.0193f;

        private readonly int fishingBase = 5;

        private readonly float cookingMulFactor = 1.03706f;

        private readonly int cookingBase = 5;

        private readonly float magicMulFactor = 1.02382f;

        private readonly int magicBase = 50;

        private readonly float summoningMulFactor = 1.0395f;

        private readonly int summoningBase = 50;

        private readonly float explosivesMulFactor = 1.0128f;

        private readonly int explosivesBase = 10;

        public enum SkillID
        {
            Mining,
            Running,
            Melee,
            Vitality,
            Crafting,
            Range,
            Gardening,
            Fishing,
            Cooking,
            Magic,
            Summoning,
            Explosives
        }

        private int GetSkillBase(SkillID skillID)
        {
            switch (skillID)
            {
                case SkillID.Mining:
                    return MiningBase;
                case SkillID.Running:
                    return runningBase;
                case SkillID.Melee:
                    return meleeBase;
                case SkillID.Vitality:
                    return vitalityBase;
                case SkillID.Crafting:
                    return craftingBase;
                case SkillID.Range:
                    return rangeBase;
                case SkillID.Gardening:
                    return gardeningBase;
                case SkillID.Fishing:
                    return fishingBase;
                case SkillID.Cooking:
                    return cookingBase;
                case SkillID.Magic:
                    return magicBase;
                case SkillID.Summoning:
                    return summoningBase;
                case SkillID.Explosives:
                    return explosivesBase;
                default:
                    return 1;
            }
        }

        private float GetSkillMulFactor(SkillID skillID)
        {
            switch (skillID)
            {
                case SkillID.Mining:
                    return MiningMulFactor;
                case SkillID.Running:
                    return runningMulFactor;
                case SkillID.Melee:
                    return meleeMulFactor;
                case SkillID.Vitality:
                    return vitalityMulFactor;
                case SkillID.Crafting:
                    return craftingMulFactor;
                case SkillID.Range:
                    return rangeMulFactor;
                case SkillID.Gardening:
                    return gardeningMulFactor;
                case SkillID.Fishing:
                    return fishingMulFactor;
                case SkillID.Cooking:
                    return cookingMulFactor;
                case SkillID.Magic:
                    return magicMulFactor;
                case SkillID.Summoning:
                    return summoningMulFactor;
                case SkillID.Explosives:
                    return explosivesMulFactor;
                default:
                    return 1f;
            }
        }

        public int GetMaxSkillLevel(SkillID skillID)
        {
            switch (skillID)
            {
                case SkillID.Mining:
                    return 100;
                case SkillID.Running:
                    return 100;
                case SkillID.Melee:
                    return 100;
                case SkillID.Vitality:
                    return 100;
                case SkillID.Crafting:
                    return 100;
                case SkillID.Range:
                    return 100;
                case SkillID.Gardening:
                    return 100;
                case SkillID.Fishing:
                    return 100;
                case SkillID.Cooking:
                    return 100;
                case SkillID.Magic:
                    return 100;
                case SkillID.Summoning:
                    return 100;
                case SkillID.Explosives:
                    return 100;
                default:
                    return 100;
            }
        }

        public int GetSkillFromLevel(SkillID skillID, int level)
        {
            float skillMulFactor = GetSkillMulFactor(skillID);
            int num = (int)Math.Round((float)GetSkillBase(skillID) * (1f - Math.Pow(skillMulFactor, (float)level)) / (1f - skillMulFactor));
            for (int i = GetLevelFromSkill(skillID, num); i < Math.Min(level, 100); i = GetLevelFromSkill(skillID, num))
            {
                num++;
            }
            return num;
        }

        public int GetLevelFromSkill(SkillID skillID, int skillValue)
        {
            float skillMulFactor = GetSkillMulFactor(skillID);
            int skillBase = GetSkillBase(skillID);
            return Math.Min((int)(Math.Log(1f - (float)skillValue * (1f - skillMulFactor) / (float)skillBase) / Math.Log(skillMulFactor)), GetMaxSkillLevel(skillID));
        }
        #endregion

        #endregion

        #region Reset Stats

        // Reset skills.
        private void ResetAllSkills_Button_Click(object sender, EventArgs e)
        {
            // Ensure the scan results are populated.
            if (MainForm.AoBScanResultsSkills == null)
            {
                // Display error.
                MessageBox.Show("You need to first scan for the Skills addresses!", "Player Skill Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Disable controls.
                ChangeSkills_Button.Enabled = false;
                MaxAllSkills_Button.Enabled = false;
                ResetAllSkills_Button.Enabled = false;
                GetPlayerSkillAddresses_Button.Enabled = true;
                PreviousSkillAddress_Button.Enabled = false;
                NextSkillAddress_Button.Enabled = false;

                Skill0_Panel.Enabled = false;
                Skill1_Panel.Enabled = false;
                Skill2_Panel.Enabled = false;
                Skill3_Panel.Enabled = false;
                Skill4_Panel.Enabled = false;
                Skill5_Panel.Enabled = false;
                Skill6_Panel.Enabled = false;
                Skill7_Panel.Enabled = false;
                Skill8_Panel.Enabled = false;
                Skill9_Panel.Enabled = false;
                Skill10_Panel.Enabled = false;
                Skill11_Panel.Enabled = false;

                ChangeSkills_ProgressBar.Value = 0;
                return;
            }

            // Adjust progressbar.
            ChangeSkills_ProgressBar.Value = 10;

            // Select the inventory to use.
            var res = MainForm.AoBScanResultsSkills.ElementAt(useAddress - 1);

            string headerBase = res.ToString("X");
            // int addressOffset = res.Item2;

            // Parse header base once.
            BigInteger baseAddress = BigInteger.Parse(headerBase, NumberStyles.HexNumber);

            int skillCount = 12;
            string[] skillIDs = new string[skillCount];
            string[] skillValues = new string[skillCount];

            // Adjust progressbar.
            ChangeSkills_ProgressBar.Value = 20;
            ChangeSkills_ProgressBar.Step = 80 / skillCount;

            // Iterate through all the skills.
            for (int i = 0; i < skillCount; i++)
            {
                int idOffset = 8 + (i * 8);     // ID is at base offset + (index * 8)
                int valueOffset = idOffset + 4; // Value is always 4 bytes after ID

                skillIDs[i] = BigInteger.Add(baseAddress, idOffset).ToString("X");
                skillValues[i] = BigInteger.Add(baseAddress, valueOffset).ToString("X");

                // Adjust progressbar.
                ChangeSkills_ProgressBar.PerformStep();
            }

            // Write values to the game.
            #region Logic

            MemLib.WriteMemory(skillIDs[0], "int", ((int)SkillID0_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillValues[0], "int", "0");

            MemLib.WriteMemory(skillIDs[1], "int", ((int)SkillID1_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillValues[1], "int", "0");

            MemLib.WriteMemory(skillIDs[2], "int", ((int)SkillID2_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillValues[2], "int", "0");

            MemLib.WriteMemory(skillIDs[3], "int", ((int)SkillID3_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillValues[3], "int", "0");

            MemLib.WriteMemory(skillIDs[4], "int", ((int)SkillID4_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillValues[4], "int", "0");

            MemLib.WriteMemory(skillIDs[5], "int", ((int)SkillID5_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillValues[5], "int", "0");

            MemLib.WriteMemory(skillIDs[6], "int", ((int)SkillID6_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillValues[6], "int", "0");

            MemLib.WriteMemory(skillIDs[7], "int", ((int)SkillID7_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillValues[7], "int", "0");

            MemLib.WriteMemory(skillIDs[8], "int", ((int)SkillID8_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillValues[8], "int", "0");

            MemLib.WriteMemory(skillIDs[9], "int", ((int)SkillID9_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillValues[9], "int", "0");

            MemLib.WriteMemory(skillIDs[10], "int", ((int)SkillID10_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillValues[10], "int", "0");

            MemLib.WriteMemory(skillIDs[11], "int", ((int)SkillID11_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillValues[11], "int", "0");

            #endregion

            // Adjust progressbar.
            ChangeSkills_ProgressBar.Value = 100;

            // Change numeric values.
            SkillILvL0_NumericUpDown.Value = 0;
            SkillILvL1_NumericUpDown.Value = 0;
            SkillILvL2_NumericUpDown.Value = 0;
            SkillILvL3_NumericUpDown.Value = 0;
            SkillILvL4_NumericUpDown.Value = 0;
            SkillILvL5_NumericUpDown.Value = 0;
            SkillILvL6_NumericUpDown.Value = 0;
            SkillILvL7_NumericUpDown.Value = 0;
            SkillILvL8_NumericUpDown.Value = 0;
            SkillILvL9_NumericUpDown.Value = 0;
            SkillILvL10_NumericUpDown.Value = 0;
            SkillILvL11_NumericUpDown.Value = 0;
        }
        #endregion

        #region Manual Stats

        // Manual set skils.
        private void ChangeSkills_Button_Click(object sender, EventArgs e)
        {
            // Ensure the scan results are populated.
            if (MainForm.AoBScanResultsSkills == null)
            {
                // Display error.
                MessageBox.Show("You need to first scan for the Skills addresses!", "Player Skill Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Disable controls.
                ChangeSkills_Button.Enabled = false;
                MaxAllSkills_Button.Enabled = false;
                ResetAllSkills_Button.Enabled = false;
                GetPlayerSkillAddresses_Button.Enabled = true;
                PreviousSkillAddress_Button.Enabled = false;
                NextSkillAddress_Button.Enabled = false;

                Skill0_Panel.Enabled = false;
                Skill1_Panel.Enabled = false;
                Skill2_Panel.Enabled = false;
                Skill3_Panel.Enabled = false;
                Skill4_Panel.Enabled = false;
                Skill5_Panel.Enabled = false;
                Skill6_Panel.Enabled = false;
                Skill7_Panel.Enabled = false;
                Skill8_Panel.Enabled = false;
                Skill9_Panel.Enabled = false;
                Skill10_Panel.Enabled = false;
                Skill11_Panel.Enabled = false;

                ChangeSkills_ProgressBar.Value = 0;
                return;
            }

            // Adjust progressbar.
            ChangeSkills_ProgressBar.Value = 10;

            // Select the inventory to use.
            var res = MainForm.AoBScanResultsSkills.ElementAt(useAddress - 1);

            string headerBase = res.ToString("X");
            // int addressOffset = res.Item2;

            // Parse header base once.
            BigInteger baseAddress = BigInteger.Parse(headerBase, NumberStyles.HexNumber);

            int skillCount = 12;
            string[] skillIDs = new string[skillCount];
            string[] skillValues = new string[skillCount];

            // Adjust progressbar.
            ChangeSkills_ProgressBar.Value = 20;
            ChangeSkills_ProgressBar.Step = 80 / skillCount;

            // Iterate through all the skills.
            for (int i = 0; i < skillCount; i++)
            {
                int idOffset = 8 + (i * 8);     // ID is at base offset + (index * 8)
                int valueOffset = idOffset + 4; // Value is always 4 bytes after ID

                skillIDs[i] = BigInteger.Add(baseAddress, idOffset).ToString("X");
                skillValues[i] = BigInteger.Add(baseAddress, valueOffset).ToString("X");

                // Adjust progressbar.
                ChangeSkills_ProgressBar.PerformStep();
            }

            // Write values to the game.
            #region Logic

            // Debug
            // MessageBox.Show(GetConvertedValues((int)numericUpDown1.Value, (int)numericUpDown2.Value, upConvert: true).ToString().ToString());

            MemLib.WriteMemory(skillIDs[0], "int", ((int)SkillID0_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillValues[0], "int", GetConvertedValues((int)SkillID0_NumericUpDown.Value, (int)SkillILvL0_NumericUpDown.Value, upConvert: true).ToString()); // Get numerical value reguardless of format!

            MemLib.WriteMemory(skillIDs[1], "int", ((int)SkillID1_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillValues[1], "int", GetConvertedValues((int)SkillID1_NumericUpDown.Value, (int)SkillILvL1_NumericUpDown.Value, upConvert: true).ToString());

            MemLib.WriteMemory(skillIDs[2], "int", ((int)SkillID2_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillValues[2], "int", GetConvertedValues((int)SkillID2_NumericUpDown.Value, (int)SkillILvL2_NumericUpDown.Value, upConvert: true).ToString());

            MemLib.WriteMemory(skillIDs[3], "int", ((int)SkillID3_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillValues[3], "int", GetConvertedValues((int)SkillID3_NumericUpDown.Value, (int)SkillILvL3_NumericUpDown.Value, upConvert: true).ToString());

            MemLib.WriteMemory(skillIDs[4], "int", ((int)SkillID4_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillValues[4], "int", GetConvertedValues((int)SkillID4_NumericUpDown.Value, (int)SkillILvL4_NumericUpDown.Value, upConvert: true).ToString());

            MemLib.WriteMemory(skillIDs[5], "int", ((int)SkillID5_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillValues[5], "int", GetConvertedValues((int)SkillID5_NumericUpDown.Value, (int)SkillILvL5_NumericUpDown.Value, upConvert: true).ToString());

            MemLib.WriteMemory(skillIDs[6], "int", ((int)SkillID6_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillValues[6], "int", GetConvertedValues((int)SkillID6_NumericUpDown.Value, (int)SkillILvL6_NumericUpDown.Value, upConvert: true).ToString());

            MemLib.WriteMemory(skillIDs[7], "int", ((int)SkillID7_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillValues[7], "int", GetConvertedValues((int)SkillID7_NumericUpDown.Value, (int)SkillILvL7_NumericUpDown.Value, upConvert: true).ToString());

            MemLib.WriteMemory(skillIDs[8], "int", ((int)SkillID8_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillValues[8], "int", GetConvertedValues((int)SkillID8_NumericUpDown.Value, (int)SkillILvL8_NumericUpDown.Value, upConvert: true).ToString());

            MemLib.WriteMemory(skillIDs[9], "int", ((int)SkillID9_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillValues[9], "int", GetConvertedValues((int)SkillID9_NumericUpDown.Value, (int)SkillILvL9_NumericUpDown.Value, upConvert: true).ToString());

            MemLib.WriteMemory(skillIDs[10], "int", ((int)SkillID10_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillValues[10], "int", GetConvertedValues((int)SkillID10_NumericUpDown.Value, (int)SkillILvL10_NumericUpDown.Value, upConvert: true).ToString());

            MemLib.WriteMemory(skillIDs[11], "int", ((int)SkillID11_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillValues[11], "int", GetConvertedValues((int)SkillID11_NumericUpDown.Value, (int)SkillILvL11_NumericUpDown.Value, upConvert: true).ToString());

            #endregion

            // Adjust progressbar.
            ChangeSkills_ProgressBar.Value = 100;
        }
        #endregion

        #region Max Stats

        // Max skills. 
        private void MaxAllSkills_Button_Click(object sender, EventArgs e)
        {
            // Ensure the scan results are populated.
            if (MainForm.AoBScanResultsSkills == null)
            {
                // Display error.
                MessageBox.Show("You need to first scan for the Skills addresses!", "Player Skill Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // Disable controls.
                ChangeSkills_Button.Enabled = false;
                MaxAllSkills_Button.Enabled = false;
                ResetAllSkills_Button.Enabled = false;
                GetPlayerSkillAddresses_Button.Enabled = true;
                PreviousSkillAddress_Button.Enabled = false;
                NextSkillAddress_Button.Enabled = false;

                Skill0_Panel.Enabled = false;
                Skill1_Panel.Enabled = false;
                Skill2_Panel.Enabled = false;
                Skill3_Panel.Enabled = false;
                Skill4_Panel.Enabled = false;
                Skill5_Panel.Enabled = false;
                Skill6_Panel.Enabled = false;
                Skill7_Panel.Enabled = false;
                Skill8_Panel.Enabled = false;
                Skill9_Panel.Enabled = false;
                Skill10_Panel.Enabled = false;
                Skill11_Panel.Enabled = false;

                ChangeSkills_ProgressBar.Value = 0;
                return;
            }

            // Adjust progressbar.
            ChangeSkills_ProgressBar.Value = 10;

            // Select the inventory to use.
            var res = MainForm.AoBScanResultsSkills.ElementAt(useAddress - 1);

            string headerBase = res.ToString("X");
            // int addressOffset = res.Item2;

            // Parse header base once.
            BigInteger baseAddress = BigInteger.Parse(headerBase, NumberStyles.HexNumber);

            int skillCount = 12;
            string[] skillIDs = new string[skillCount];
            string[] skillValues = new string[skillCount];

            // Adjust progressbar.
            ChangeSkills_ProgressBar.Value = 20;
            ChangeSkills_ProgressBar.Step = 80 / skillCount;

            // Iterate through all the skills.
            for (int i = 0; i < skillCount; i++)
            {
                int idOffset = 8 + (i * 8);     // ID is at base offset + (index * 8)
                int valueOffset = idOffset + 4; // Value is always 4 bytes after ID

                skillIDs[i] = BigInteger.Add(baseAddress, idOffset).ToString("X");
                skillValues[i] = BigInteger.Add(baseAddress, valueOffset).ToString("X");

                // Adjust progressbar.
                ChangeSkills_ProgressBar.PerformStep();
            }

            // Write values to the game.
            #region Set Max Levels

            // Iterate through all skills and assign their official max EXP values.
            for (int i = 0; i < skillCount; i++)
            {
                // Read the skill ID from memory.
                int skillIDInt = MemLib.ReadInt(skillIDs[i]);

                // Check if the skill ID is valid by testing if it exists in the SkillID enum.
                if (Enum.IsDefined(typeof(SkillID), skillIDInt))
                {
                    SkillID skillID = (SkillID)skillIDInt;

                    // Get the official max skill level (this is typically 100, but can change if modified).
                    int maxLevel = GetMaxSkillLevel(skillID);

                    // Convert the max level to the raw EXP value using the official conversion.
                    int maxExpValue = GetSkillFromLevel(skillID, maxLevel);

                    // Write the official max EXP value to memory.
                    MemLib.WriteMemory(skillValues[i], "int", maxExpValue.ToString());
                }
            }

            #endregion

            #region Set Custom IDs

            MemLib.WriteMemory(skillIDs[0], "int", ((int)SkillID0_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillIDs[1], "int", ((int)SkillID1_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillIDs[2], "int", ((int)SkillID2_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillIDs[3], "int", ((int)SkillID3_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillIDs[4], "int", ((int)SkillID4_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillIDs[5], "int", ((int)SkillID5_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillIDs[6], "int", ((int)SkillID6_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillIDs[7], "int", ((int)SkillID7_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillIDs[8], "int", ((int)SkillID8_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillIDs[9], "int", ((int)SkillID9_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillIDs[10], "int", ((int)SkillID10_NumericUpDown.Value).ToString());
            MemLib.WriteMemory(skillIDs[11], "int", ((int)SkillID11_NumericUpDown.Value).ToString());

            #endregion

            // Adjust progressbar.
            ChangeSkills_ProgressBar.Value = 100;

            // Change numeric values.
            SkillILvL0_NumericUpDown.Value = SkillILvL0_NumericUpDown.Maximum;
            SkillILvL1_NumericUpDown.Value = SkillILvL1_NumericUpDown.Maximum;
            SkillILvL2_NumericUpDown.Value = SkillILvL2_NumericUpDown.Maximum;
            SkillILvL3_NumericUpDown.Value = SkillILvL3_NumericUpDown.Maximum;
            SkillILvL4_NumericUpDown.Value = SkillILvL4_NumericUpDown.Maximum;
            SkillILvL5_NumericUpDown.Value = SkillILvL5_NumericUpDown.Maximum;
            SkillILvL6_NumericUpDown.Value = SkillILvL6_NumericUpDown.Maximum;
            SkillILvL7_NumericUpDown.Value = SkillILvL7_NumericUpDown.Maximum;
            SkillILvL8_NumericUpDown.Value = SkillILvL8_NumericUpDown.Maximum;
            SkillILvL9_NumericUpDown.Value = SkillILvL9_NumericUpDown.Maximum;
            SkillILvL10_NumericUpDown.Value = SkillILvL10_NumericUpDown.Maximum;
            SkillILvL11_NumericUpDown.Value = SkillILvL11_NumericUpDown.Maximum;
        }
        #endregion
    }
}