using System.Collections.Generic;
using CoreKeeperInventoryEditor;
using System.Windows.Forms;
using System.Reflection;
using Newtonsoft.Json;
using System.IO;
using System;

namespace CoreKeepersWorkshop
{
    public partial class FoodCookbook : Form
    {
        // Form initialization.
        private CustomFormStyler _formThemeStyler;
        public FoodCookbook()
        {
            InitializeComponent();
            Load += (_, __) => _formThemeStyler = this.ApplyTheme(); // Load the forms theme.
        }

        #region Closing Varibles

        // Form closing saving.
        int selectedItemType      = 0;
        int selectedItemAmount    = 0;
        int selectedItemVariation = 0;
        int selectedItemSkillset  = 0;
        bool userCanceldTask      = false;

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
        #endregion

        #region Form Load And Closing Events

        // Do loading events for the form.
        private void FoodCookbook_Load(object sender, EventArgs e)
        {
            #region Set Custom Cusror

            // Set the applications cursor.
            Cursor = new Cursor(CoreKeepersWorkshop.Properties.Resources.UICursor.GetHicon());
            #endregion

            #region Set Form Locations

            // Set the forms active location based on previous save.
            this.Location = CoreKeepersWorkshop.Properties.Settings.Default.CookbookLocation;
            #endregion

            #region Set Form Opacity

            // Set form opacity based on trackbars value saved setting (1 to 100 -> 0.01 to 1.0).
            this.Opacity = Properties.Settings.Default.FormOpacity / 100.0;
            #endregion

            #region Tooltips

            // Create a new tooltip.
            ToolTip toolTip = new ToolTip()
            {
                AutoPopDelay = 5000,
                InitialDelay = 750
            };

            // Set tool texts.
            toolTip.SetToolTip(ItemSearch_TextBox,       "Search for an item by name, id, or variant. Press enter when done.");
            toolTip.SetToolTip(SearchForItem_Button,     "Search the indexes for a desired item.");

            toolTip.SetToolTip(ItemAmount_NumericUpDown, "Enter the amount of items to add.");
            #endregion

            #region Populate Datagridview

            // Get json file from resources.
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CoreKeepersWorkshop.Resources.Cookbook.json"))
            using (StreamReader reader = new StreamReader(stream))
            {
                // Convert stream into string.
                var jsonFileContent = reader.ReadToEnd();
                var records = JsonConvert.DeserializeObject<List<FoodRecord>>(jsonFileContent);

                foreach (var r in records)
                {
                    FoodCookbook_DataGridView.Rows.Add(
                        r.Name,
                        r.Stats,
                        r.Id,
                        r.Variation,
                        r.Skillset ?? 0
                    );
                }
            }

            // Sort datagridview by ascending.
            FoodCookbook_DataGridView.Sort(FoodCookbook_DataGridView.Columns[0], System.ComponentModel.ListSortDirection.Ascending);
            #endregion
        }

        #region Form Closing

        // Do form closing events.
        private void FoodCookbook_FormClosing(object sender, FormClosingEventArgs e)
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
                CoreKeepersWorkshop.Properties.Settings.Default.CookbookLocation = this.Location;
            }
            catch (Exception)
            { } // Do nothing.
        }
        #endregion

        #endregion

        #region Form Controls - Search For Items

        #region Search

        // Search the datagridview.
        private void SearchForItem_Button_Click(object sender, EventArgs e)
        {
            string searchValue = ItemSearch_TextBox.Text;
            FoodCookbook_DataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            try
            {
                // Get each row within datagridview.
                bool valueFound = false;

                // Check is a row is selected or not.
                if (FoodCookbook_DataGridView.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow row in FoodCookbook_DataGridView.Rows)
                    {
                        for (int i = 0; i < row.Cells.Count; i++)
                        {
                            // Search rows only rows after the selected row.
                            if (FoodCookbook_DataGridView.SelectedRows[0].Index < row.Index)
                            {
                                // Check if rows current cell contains the value.
                                if (row.Cells[i].Value != null && row.Cells[i].Value.ToString().ToLower().Contains(searchValue))
                                {
                                    // Search match found.
                                    int rowIndex = row.Index;
                                    FoodCookbook_DataGridView.Rows[rowIndex].Selected = true;
                                    FoodCookbook_DataGridView.CurrentCell = FoodCookbook_DataGridView.Rows[0].Cells[0];        // Unselect row.
                                    FoodCookbook_DataGridView.CurrentCell = FoodCookbook_DataGridView.Rows[rowIndex].Cells[0]; // Select found row.
                                    valueFound = true;
                                    break;
                                }
                            }
                        }

                        // Break out of loop if a value was found.
                        if (valueFound)
                            break;
                    }
                }
                else
                {
                    foreach (DataGridViewRow row in FoodCookbook_DataGridView.Rows)
                    {
                        for (int i = 0; i < row.Cells.Count; i++)
                        {
                            // Check if rows current cell contains the value.
                            if (row.Cells[i].Value != null && row.Cells[i].Value.ToString().ToLower().Contains(searchValue))
                            {
                                // Search match found.
                                int rowIndex = row.Index;
                                FoodCookbook_DataGridView.Rows[rowIndex].Selected = true;
                                FoodCookbook_DataGridView.CurrentCell = FoodCookbook_DataGridView.Rows[0].Cells[0]; // Unselect row.
                                FoodCookbook_DataGridView.CurrentCell = FoodCookbook_DataGridView.Rows[rowIndex].Cells[0]; // Select found row.
                                valueFound = true;
                                break;
                            }
                        }

                        // Break out of loop if a value was found.
                        if (valueFound)
                            break;
                    }
                }
                if (!valueFound)
                {
                    // Check is a row is selected or not.
                    if (FoodCookbook_DataGridView.SelectedRows.Count > 0)
                    {
                        // Display message.
                        MessageBox.Show("No further search results for \"" + ItemSearch_TextBox.Text + "\".", "Cookbook Search", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        // Display message.
                        MessageBox.Show("No search results for \"" + ItemSearch_TextBox.Text + "\".", "Cookbook Search", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    // Reset row back to zero.
                    FoodCookbook_DataGridView.CurrentCell = FoodCookbook_DataGridView.Rows[0].Cells[0];
                    FoodCookbook_DataGridView.Rows[0].Selected = false;
                    return;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }
        #endregion

        #region Enter Down

        // Do enter down events.        
        private void ItemSearch_TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // Filter only the enter key.
            if (e.KeyCode == Keys.Enter)
            {
                string searchValue = ItemSearch_TextBox.Text;
                FoodCookbook_DataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                try
                {
                    // Get each row within datagridview.
                    bool valueFound = false;

                    // Check is a row is selected or not.
                    if (FoodCookbook_DataGridView.SelectedRows.Count > 0)
                    {
                        foreach (DataGridViewRow row in FoodCookbook_DataGridView.Rows)
                        {
                            for (int i = 0; i < row.Cells.Count; i++)
                            {
                                // Search rows only rows after the selected row.
                                if (FoodCookbook_DataGridView.SelectedRows[0].Index < row.Index)
                                {
                                    // Check if rows current cell contains the value.
                                    if (row.Cells[i].Value != null && row.Cells[i].Value.ToString().ToLower().Contains(searchValue))
                                    {
                                        // Search match found.
                                        int rowIndex = row.Index;
                                        FoodCookbook_DataGridView.Rows[rowIndex].Selected = true;
                                        FoodCookbook_DataGridView.CurrentCell = FoodCookbook_DataGridView.Rows[0].Cells[0]; // Unselect row.
                                        FoodCookbook_DataGridView.CurrentCell = FoodCookbook_DataGridView.Rows[rowIndex].Cells[0]; // Select found row.
                                        valueFound = true;
                                        break;
                                    }
                                }
                            }

                            // Break out of loop if a value was found.
                            if (valueFound)
                                break;
                        }
                    }
                    else
                    {
                        foreach (DataGridViewRow row in FoodCookbook_DataGridView.Rows)
                        {
                            for (int i = 0; i < row.Cells.Count; i++)
                            {
                                // Check if rows current cell contains the value.
                                if (row.Cells[i].Value != null && row.Cells[i].Value.ToString().ToLower().Contains(searchValue))
                                {
                                    // Search match found.
                                    int rowIndex = row.Index;
                                    FoodCookbook_DataGridView.Rows[rowIndex].Selected = true;
                                    FoodCookbook_DataGridView.CurrentCell = FoodCookbook_DataGridView.Rows[0].Cells[0]; // Unselect row.
                                    FoodCookbook_DataGridView.CurrentCell = FoodCookbook_DataGridView.Rows[rowIndex].Cells[0]; // Select found row.
                                    valueFound = true;
                                    break;
                                }
                            }

                            // Break out of loop if a value was found.
                            if (valueFound)
                                break;
                        }
                    }
                    if (!valueFound)
                    {
                        // Check is a row is selected or not.
                        if (FoodCookbook_DataGridView.SelectedRows.Count > 0)
                        {
                            // Display message.
                            MessageBox.Show("No further search results for \"" + ItemSearch_TextBox.Text + "\".", "Cookbook Search", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            // Display message.
                            MessageBox.Show("No search results for \"" + ItemSearch_TextBox.Text + "\".", "Cookbook Search", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        // Reset row back to zero.
                        FoodCookbook_DataGridView.CurrentCell = FoodCookbook_DataGridView.Rows[0].Cells[0];
                        FoodCookbook_DataGridView.Rows[0].Selected = false;
                        return;
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message);
                }
            }
        }
        #endregion

        #endregion

        #region Keydown Events

        // Select the row.
        private void FoodCookbook_DataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Get values from selected row.
                int itemAmount        = (int)ItemAmount_NumericUpDown.Value;
                int itemID            = int.Parse(FoodCookbook_DataGridView.Rows[e.RowIndex].Cells[2].Value.ToString());
                int itemVariation     = int.Parse(FoodCookbook_DataGridView.Rows[e.RowIndex].Cells[3].Value.ToString());
                int itemSkillset      = int.Parse(FoodCookbook_DataGridView.Rows[e.RowIndex].Cells[4].Value.ToString());

                // Set values for closing.
                selectedItemAmount    = itemAmount;
                selectedItemType      = itemID;
                selectedItemVariation = itemVariation;
                selectedItemSkillset  = itemSkillset;
                this.Close();
            }
            catch (Exception) { }
        }
        #endregion
    }
}