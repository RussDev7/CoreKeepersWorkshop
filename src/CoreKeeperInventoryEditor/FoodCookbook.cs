using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CoreKeepersWorkshop
{
    public partial class FoodCookbook : Form
    {
        public FoodCookbook()
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

        // Do loading events for the form.
        private void FoodCookbook_Load(object sender, EventArgs e)
        {
            #region Set Form Locations

            // Set the forms active location based on previous save.
            this.Location = CoreKeepersWorkshop.Properties.Settings.Default.CookbookLocation;
            #endregion

            #region Tooltips

            // Create a new tooltip.
            ToolTip toolTip = new ToolTip()
            {
                AutoPopDelay = 3000,
                InitialDelay = 1000
            };

            // Set tool texts.
            toolTip.SetToolTip(textBox1, "Search for an item by name, id, or variant. Press enter when done.");
            toolTip.SetToolTip(button1, "Search the indexes for a desired item.");

            toolTip.SetToolTip(numericUpDown1, "Enter the amount of items to add.");
            #endregion

            #region Populate Datagridview
            // Get json file from resources.
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CoreKeepersWorkshop.Resources.Cookbook.json"))
            using (StreamReader reader = new StreamReader(stream))
            {
                // Convert stream into string.
                var jsonFileContent = reader.ReadToEnd();
                dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonFileContent);

                // Load each object from json to a string array.
                foreach (var file in result)
                {
                    // Remove spaces from food names.
                    string foodName = (string)file.name;
                    foodName = new Regex("[ ]{2,}", RegexOptions.None).Replace(foodName, " ");

                    // Add the values to the datagridview.
                    dataGridView1.Invoke((MethodInvoker)(() => dataGridView1.Rows.Add((string)foodName, (string)file.stats, (string)file.id, (string)file.variation)));
                }
            }

            // Sort datagridview by ascending.
            dataGridView1.Sort(dataGridView1.Columns[0], System.ComponentModel.ListSortDirection.Ascending);
            #endregion
        }

        // Do form closing events.
        private void FoodCookbook_FormClosing(object sender, FormClosingEventArgs e)
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
                CoreKeepersWorkshop.Properties.Settings.Default.CookbookLocation = this.Location;
            }
            catch (Exception)
            { } // Do nothing.
        }
        #endregion

        #region Form Controls

        // Search the datagridview.
        private void Button2_Click(object sender, EventArgs e)
        {
            string searchValue = textBox1.Text;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            try
            {
                // Get each row within datagridview.
                bool valueFound = false;

                // Check is a row is selected or not.
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        for (int i = 0; i < row.Cells.Count; i++)
                        {
                            // Search rows only rows after the selected row.
                            if (dataGridView1.SelectedRows[0].Index < row.Index)
                            {
                                // Check if rows cirst cell contains the value.
                                if (row.Cells[i].Value != null && row.Cells[i].Value.ToString().ToLower().Contains(searchValue))
                                {
                                    // Search match found.
                                    int rowIndex = row.Index;
                                    dataGridView1.Rows[rowIndex].Selected = true;
                                    dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0]; // Unselect row.
                                    dataGridView1.CurrentCell = dataGridView1.Rows[rowIndex].Cells[0]; // Select found row.
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
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        for (int i = 0; i < row.Cells.Count; i++)
                        {
                            // Check if rows cirst cell contains the value.
                            if (row.Cells[i].Value != null && row.Cells[i].Value.ToString().ToLower().Contains(searchValue))
                            {
                                // Search match found.
                                int rowIndex = row.Index;
                                dataGridView1.Rows[rowIndex].Selected = true;
                                dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0]; // Unselect row.
                                dataGridView1.CurrentCell = dataGridView1.Rows[rowIndex].Cells[0]; // Select found row.
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
                    if (dataGridView1.SelectedRows.Count > 0)
                    {
                        // Display message.
                        MessageBox.Show("No further search results for \"" + textBox1.Text + "\".", "Cookbook Search", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        // Display message.
                        MessageBox.Show("No search results for \"" + textBox1.Text + "\".", "Cookbook Search", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    // Reset row back to zero.
                    dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];
                    dataGridView1.Rows[0].Selected = false;
                    return;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message);
            }
        }

        // Do enter events.        
        private void TextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            // Filter only the enter key.
            if (e.KeyCode == Keys.Enter)
            {
                string searchValue = textBox1.Text;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                try
                {
                    // Get each row within datagridview.
                    bool valueFound = false;

                    // Check is a row is selected or not.
                    if (dataGridView1.SelectedRows.Count > 0)
                    {
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            for (int i = 0; i < row.Cells.Count; i++)
                            {
                                // Search rows only rows after the selected row.
                                if (dataGridView1.SelectedRows[0].Index < row.Index)
                                {
                                    // Check if rows cirst cell contains the value.
                                    if (row.Cells[i].Value != null && row.Cells[i].Value.ToString().ToLower().Contains(searchValue))
                                    {
                                        // Search match found.
                                        int rowIndex = row.Index;
                                        dataGridView1.Rows[rowIndex].Selected = true;
                                        dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0]; // Unselect row.
                                        dataGridView1.CurrentCell = dataGridView1.Rows[rowIndex].Cells[0]; // Select found row.
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
                        foreach (DataGridViewRow row in dataGridView1.Rows)
                        {
                            for (int i = 0; i < row.Cells.Count; i++)
                            {
                                // Check if rows cirst cell contains the value.
                                if (row.Cells[i].Value != null && row.Cells[i].Value.ToString().ToLower().Contains(searchValue))
                                {
                                    // Search match found.
                                    int rowIndex = row.Index;
                                    dataGridView1.Rows[rowIndex].Selected = true;
                                    dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0]; // Unselect row.
                                    dataGridView1.CurrentCell = dataGridView1.Rows[rowIndex].Cells[0]; // Select found row.
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
                        if (dataGridView1.SelectedRows.Count > 0)
                        {
                            // Display message.
                            MessageBox.Show("No further search results for \"" + textBox1.Text + "\".", "Cookbook Search", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            // Display message.
                            MessageBox.Show("No search results for \"" + textBox1.Text + "\".", "Cookbook Search", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        // Reset row back to zero.
                        dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];
                        dataGridView1.Rows[0].Selected = false;
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

        #region Keydown Events

        // Select the row.
        private void DataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            // Get values from selected row.
            int itemAmount = (int)numericUpDown1.Value;
            int itemID = int.Parse(dataGridView1.Rows[e.RowIndex].Cells[2].Value.ToString());
            int itemVariation = int.Parse(dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString());

            // Set values for closing.
            selectedItemAmount = itemAmount;
            selectedItemType = itemID;
            selectedItemVariation = itemVariation;
            this.Close();
        }
        #endregion
    }
}
