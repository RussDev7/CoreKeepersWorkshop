using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace CoreKeepersWorkshop
{
    public partial class ItemEditor : Form
    {
        public ItemEditor()
        {
            InitializeComponent();
        }

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

        #region Form Load And Closing Events

        // Do loading events.
        private void ItemEditor_Load(object sender, EventArgs e)
        {
            #region Tooltips

            // Create a new tooltip.
            ToolTip toolTip = new ToolTip();
            toolTip.AutoPopDelay = 5000;
            toolTip.InitialDelay = 1500;

            // Set tool texts.
            toolTip.SetToolTip(numericUpDown1, "Enter the amount of items to add.");
            toolTip.SetToolTip(numericUpDown2, "Enter a custom ID. Either press enter when done or use the button.");
            toolTip.SetToolTip(numericUpDown3, "Enter a custom variant ID. Either press enter when done or use the button.");

            toolTip.SetToolTip(button1, "Remove the item from this inventory slot.");
            toolTip.SetToolTip(button3, "Spawn in custom item amount + ID.");

            #endregion

            // Load some settings.
            numericUpDown1.Value = CoreKeepersWorkshop.Properties.Settings.Default.InfoID;
            numericUpDown2.Value = CoreKeepersWorkshop.Properties.Settings.Default.InfoAmount;
            numericUpDown3.Value = CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation;
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

            // Save some form settings.
            CoreKeepersWorkshop.Properties.Settings.Default.InfoID = (int)numericUpDown1.Value;
            CoreKeepersWorkshop.Properties.Settings.Default.InfoAmount = (int)numericUpDown2.Value;
            CoreKeepersWorkshop.Properties.Settings.Default.InfoVariation = (int)numericUpDown3.Value;
        }
        #endregion // Form loading and closing events.

        #region Keydown Events

        // Do enter events.
        private void numericUpDown1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectedItemType = (int)numericUpDown1.Value;
                selectedItemAmount = (int)numericUpDown2.Value;
                selectedItemVariation = (int)numericUpDown3.Value;
                this.Close();
            }
        }
        private void numericUpDown2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectedItemType = (int)numericUpDown1.Value;
                selectedItemAmount = (int)numericUpDown2.Value;
                selectedItemVariation = (int)numericUpDown3.Value;
                this.Close();
            }
        }
        private void numericUpDown3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                selectedItemType = (int)numericUpDown1.Value;
                selectedItemAmount = (int)numericUpDown2.Value;
                selectedItemVariation = (int)numericUpDown3.Value;
                this.Close();
            }
        }
        #endregion // Keydown Events.

    }
}
