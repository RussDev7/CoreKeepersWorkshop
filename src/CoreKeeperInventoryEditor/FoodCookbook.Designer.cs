
namespace CoreKeepersWorkshop
{
    partial class FoodCookbook
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FoodCookbook));
            this.FoodCookbook_DataGridView = new System.Windows.Forms.DataGridView();
            this.itemName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.itemStats = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.itemID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.itemVariation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.itemSkillset = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SearchForItem_Button = new System.Windows.Forms.Button();
            this.ItemSearch_TextBox = new System.Windows.Forms.TextBox();
            this.ItemAmount_NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.ItemAmount_Button = new System.Windows.Forms.Button();
            this.Main_GroupBox = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.FoodCookbook_DataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemAmount_NumericUpDown)).BeginInit();
            this.Main_GroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // FoodCookbook_DataGridView
            // 
            this.FoodCookbook_DataGridView.AllowUserToAddRows = false;
            this.FoodCookbook_DataGridView.AllowUserToDeleteRows = false;
            this.FoodCookbook_DataGridView.AllowUserToResizeColumns = false;
            this.FoodCookbook_DataGridView.AllowUserToResizeRows = false;
            this.FoodCookbook_DataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.FoodCookbook_DataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.FoodCookbook_DataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.itemName,
            this.itemStats,
            this.itemID,
            this.itemVariation,
            this.itemSkillset});
            this.FoodCookbook_DataGridView.Location = new System.Drawing.Point(1, 7);
            this.FoodCookbook_DataGridView.Name = "FoodCookbook_DataGridView";
            this.FoodCookbook_DataGridView.ReadOnly = true;
            this.FoodCookbook_DataGridView.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.FoodCookbook_DataGridView.RowHeadersVisible = false;
            this.FoodCookbook_DataGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.FoodCookbook_DataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.FoodCookbook_DataGridView.ShowCellErrors = false;
            this.FoodCookbook_DataGridView.ShowCellToolTips = false;
            this.FoodCookbook_DataGridView.ShowEditingIcon = false;
            this.FoodCookbook_DataGridView.ShowRowErrors = false;
            this.FoodCookbook_DataGridView.Size = new System.Drawing.Size(690, 623);
            this.FoodCookbook_DataGridView.TabIndex = 3;
            this.FoodCookbook_DataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.FoodCookbook_DataGridView_CellDoubleClick);
            // 
            // itemName
            // 
            this.itemName.FillWeight = 130F;
            this.itemName.HeaderText = "Name";
            this.itemName.Name = "itemName";
            this.itemName.ReadOnly = true;
            // 
            // itemStats
            // 
            this.itemStats.FillWeight = 280F;
            this.itemStats.HeaderText = "Stats";
            this.itemStats.Name = "itemStats";
            this.itemStats.ReadOnly = true;
            // 
            // itemID
            // 
            this.itemID.FillWeight = 35F;
            this.itemID.HeaderText = "ID";
            this.itemID.Name = "itemID";
            this.itemID.ReadOnly = true;
            // 
            // itemVariation
            // 
            this.itemVariation.FillWeight = 50F;
            this.itemVariation.HeaderText = "Variation";
            this.itemVariation.Name = "itemVariation";
            this.itemVariation.ReadOnly = true;
            // 
            // itemSkillset
            // 
            this.itemSkillset.FillWeight = 40F;
            this.itemSkillset.HeaderText = "Skillset";
            this.itemSkillset.Name = "itemSkillset";
            this.itemSkillset.ReadOnly = true;
            // 
            // SearchForItem_Button
            // 
            this.SearchForItem_Button.Location = new System.Drawing.Point(397, 11);
            this.SearchForItem_Button.Name = "SearchForItem_Button";
            this.SearchForItem_Button.Size = new System.Drawing.Size(103, 22);
            this.SearchForItem_Button.TabIndex = 1;
            this.SearchForItem_Button.Text = "Search For Item";
            this.SearchForItem_Button.UseVisualStyleBackColor = true;
            this.SearchForItem_Button.Click += new System.EventHandler(this.SearchForItem_Button_Click);
            // 
            // ItemSearch_TextBox
            // 
            this.ItemSearch_TextBox.Location = new System.Drawing.Point(12, 12);
            this.ItemSearch_TextBox.Name = "ItemSearch_TextBox";
            this.ItemSearch_TextBox.Size = new System.Drawing.Size(386, 20);
            this.ItemSearch_TextBox.TabIndex = 0;
            this.ItemSearch_TextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ItemSearch_TextBox_KeyDown);
            // 
            // ItemAmount_NumericUpDown
            // 
            this.ItemAmount_NumericUpDown.Location = new System.Drawing.Point(506, 12);
            this.ItemAmount_NumericUpDown.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.ItemAmount_NumericUpDown.Name = "ItemAmount_NumericUpDown";
            this.ItemAmount_NumericUpDown.Size = new System.Drawing.Size(198, 20);
            this.ItemAmount_NumericUpDown.TabIndex = 2;
            this.ItemAmount_NumericUpDown.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            // 
            // ItemAmount_Button
            // 
            this.ItemAmount_Button.Enabled = false;
            this.ItemAmount_Button.Location = new System.Drawing.Point(629, 11);
            this.ItemAmount_Button.Name = "ItemAmount_Button";
            this.ItemAmount_Button.Size = new System.Drawing.Size(75, 22);
            this.ItemAmount_Button.TabIndex = 0;
            this.ItemAmount_Button.Text = "Amount";
            this.ItemAmount_Button.UseVisualStyleBackColor = true;
            // 
            // Main_GroupBox
            // 
            this.Main_GroupBox.Controls.Add(this.FoodCookbook_DataGridView);
            this.Main_GroupBox.Location = new System.Drawing.Point(12, 37);
            this.Main_GroupBox.Name = "Main_GroupBox";
            this.Main_GroupBox.Size = new System.Drawing.Size(692, 632);
            this.Main_GroupBox.TabIndex = 6;
            this.Main_GroupBox.TabStop = false;
            // 
            // FoodCookbook
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(716, 681);
            this.Controls.Add(this.ItemAmount_Button);
            this.Controls.Add(this.Main_GroupBox);
            this.Controls.Add(this.SearchForItem_Button);
            this.Controls.Add(this.ItemSearch_TextBox);
            this.Controls.Add(this.ItemAmount_NumericUpDown);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(12)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FoodCookbook";
            this.Text = "Food Cookbook --> Double Click: Selects Item";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FoodCookbook_FormClosing);
            this.Load += new System.EventHandler(this.FoodCookbook_Load);
            ((System.ComponentModel.ISupportInitialize)(this.FoodCookbook_DataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ItemAmount_NumericUpDown)).EndInit();
            this.Main_GroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView FoodCookbook_DataGridView;
        private System.Windows.Forms.Button SearchForItem_Button;
        private System.Windows.Forms.TextBox ItemSearch_TextBox;
        private System.Windows.Forms.NumericUpDown ItemAmount_NumericUpDown;
        private System.Windows.Forms.Button ItemAmount_Button;
        private System.Windows.Forms.GroupBox Main_GroupBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn itemName;
        private System.Windows.Forms.DataGridViewTextBoxColumn itemStats;
        private System.Windows.Forms.DataGridViewTextBoxColumn itemID;
        private System.Windows.Forms.DataGridViewTextBoxColumn itemVariation;
        private System.Windows.Forms.DataGridViewTextBoxColumn itemSkillset;
    }
}