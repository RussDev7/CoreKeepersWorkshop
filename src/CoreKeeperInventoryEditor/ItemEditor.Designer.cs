
namespace CoreKeepersWorkshop
{
    partial class ItemEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ItemEditor));
            this.ItemID_NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.VariationNumerical_NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.Quantity_NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.Quantity_Button = new System.Windows.Forms.Button();
            this.Variation1_NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.VariationAnd_Button = new System.Windows.Forms.Button();
            this.Variation2_NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.RemoveItem_Button = new System.Windows.Forms.Button();
            this.CookedFoodList_Button = new System.Windows.Forms.Button();
            this.ItemID_Label = new System.Windows.Forms.Label();
            this.Variation_Label = new System.Windows.Forms.Label();
            this.Slot1_PictureBox = new System.Windows.Forms.PictureBox();
            this.Slot2_PictureBox = new System.Windows.Forms.PictureBox();
            this.Slot3_PictureBox = new System.Windows.Forms.PictureBox();
            this.Item1_Label = new System.Windows.Forms.Label();
            this.Item2_Label = new System.Windows.Forms.Label();
            this.Item3_Label = new System.Windows.Forms.Label();
            this.ChangeRarity_Button = new System.Windows.Forms.Button();
            this.Done_Button = new System.Windows.Forms.Button();
            this.QuickQuantitySelector_GroupBox = new System.Windows.Forms.GroupBox();
            this.CustomQuantity1_Button = new System.Windows.Forms.Button();
            this.CustomQuantity3_Button = new System.Windows.Forms.Button();
            this.CustomQuantity4_Button = new System.Windows.Forms.Button();
            this.CustomQuantity5_Button = new System.Windows.Forms.Button();
            this.CustomQuantity1_NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.CustomQuantity3_NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.CustomQuantity4_NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.CustomQuantity5_NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.CustomQuantity2_Button = new System.Windows.Forms.Button();
            this.CustomQuantity2_NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.Skillset_Button = new System.Windows.Forms.Button();
            this.Skillset_NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.SkillsetHelp_Button = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ItemID_NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.VariationNumerical_NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Quantity_NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Variation1_NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Variation2_NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Slot1_PictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Slot2_PictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Slot3_PictureBox)).BeginInit();
            this.QuickQuantitySelector_GroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CustomQuantity1_NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CustomQuantity3_NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CustomQuantity4_NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CustomQuantity5_NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CustomQuantity2_NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Skillset_NumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // ItemID_NumericUpDown
            // 
            this.ItemID_NumericUpDown.Location = new System.Drawing.Point(12, 29);
            this.ItemID_NumericUpDown.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.ItemID_NumericUpDown.Name = "ItemID_NumericUpDown";
            this.ItemID_NumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.ItemID_NumericUpDown.TabIndex = 0;
            this.ItemID_NumericUpDown.Value = new decimal(new int[] {
            110,
            0,
            0,
            0});
            this.ItemID_NumericUpDown.ValueChanged += new System.EventHandler(this.ItemID_NumericUpDown_ValueChanged);
            this.ItemID_NumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ItemID_NumericUpDown_KeyDown);
            // 
            // VariationNumerical_NumericUpDown
            // 
            this.VariationNumerical_NumericUpDown.Location = new System.Drawing.Point(91, 29);
            this.VariationNumerical_NumericUpDown.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.VariationNumerical_NumericUpDown.Name = "VariationNumerical_NumericUpDown";
            this.VariationNumerical_NumericUpDown.Size = new System.Drawing.Size(145, 20);
            this.VariationNumerical_NumericUpDown.TabIndex = 2;
            this.VariationNumerical_NumericUpDown.ValueChanged += new System.EventHandler(this.VariationNumerical_NumericUpDown_ValueChanged);
            this.VariationNumerical_NumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.VariationNumerical_NumericUpDown_KeyDown);
            // 
            // Quantity_NumericUpDown
            // 
            this.Quantity_NumericUpDown.Location = new System.Drawing.Point(72, 143);
            this.Quantity_NumericUpDown.Maximum = new decimal(new int[] {
            2000000000,
            0,
            0,
            0});
            this.Quantity_NumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.Quantity_NumericUpDown.Name = "Quantity_NumericUpDown";
            this.Quantity_NumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.Quantity_NumericUpDown.TabIndex = 4;
            this.Quantity_NumericUpDown.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.Quantity_NumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Quantity_NumericUpDown_KeyDown);
            // 
            // Quantity_Button
            // 
            this.Quantity_Button.Enabled = false;
            this.Quantity_Button.Location = new System.Drawing.Point(12, 142);
            this.Quantity_Button.Name = "Quantity_Button";
            this.Quantity_Button.Size = new System.Drawing.Size(60, 22);
            this.Quantity_Button.TabIndex = 0;
            this.Quantity_Button.Text = "Quantity";
            this.Quantity_Button.UseVisualStyleBackColor = true;
            // 
            // Variation1_NumericUpDown
            // 
            this.Variation1_NumericUpDown.Location = new System.Drawing.Point(91, 29);
            this.Variation1_NumericUpDown.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.Variation1_NumericUpDown.Name = "Variation1_NumericUpDown";
            this.Variation1_NumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.Variation1_NumericUpDown.TabIndex = 3;
            this.Variation1_NumericUpDown.Visible = false;
            this.Variation1_NumericUpDown.ValueChanged += new System.EventHandler(this.Variation1_NumericUpDown_ValueChanged);
            this.Variation1_NumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Variation1_NumericUpDown_KeyDown);
            // 
            // VariationAnd_Button
            // 
            this.VariationAnd_Button.Enabled = false;
            this.VariationAnd_Button.Location = new System.Drawing.Point(153, 28);
            this.VariationAnd_Button.Name = "VariationAnd_Button";
            this.VariationAnd_Button.Size = new System.Drawing.Size(21, 22);
            this.VariationAnd_Button.TabIndex = 0;
            this.VariationAnd_Button.Text = "&&";
            this.VariationAnd_Button.UseVisualStyleBackColor = true;
            this.VariationAnd_Button.Visible = false;
            // 
            // Variation2_NumericUpDown
            // 
            this.Variation2_NumericUpDown.Location = new System.Drawing.Point(176, 29);
            this.Variation2_NumericUpDown.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.Variation2_NumericUpDown.Name = "Variation2_NumericUpDown";
            this.Variation2_NumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.Variation2_NumericUpDown.TabIndex = 4;
            this.Variation2_NumericUpDown.Visible = false;
            this.Variation2_NumericUpDown.ValueChanged += new System.EventHandler(this.Variation2_NumericUpDown_ValueChanged);
            this.Variation2_NumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Variation2_NumericUpDown_KeyDown);
            // 
            // RemoveItem_Button
            // 
            this.RemoveItem_Button.Location = new System.Drawing.Point(139, 170);
            this.RemoveItem_Button.Name = "RemoveItem_Button";
            this.RemoveItem_Button.Size = new System.Drawing.Size(104, 22);
            this.RemoveItem_Button.TabIndex = 7;
            this.RemoveItem_Button.Text = "Remove Item";
            this.RemoveItem_Button.UseVisualStyleBackColor = true;
            this.RemoveItem_Button.Click += new System.EventHandler(this.RemoveItem_Button_Click);
            // 
            // CookedFoodList_Button
            // 
            this.CookedFoodList_Button.Location = new System.Drawing.Point(12, 198);
            this.CookedFoodList_Button.Name = "CookedFoodList_Button";
            this.CookedFoodList_Button.Size = new System.Drawing.Size(230, 22);
            this.CookedFoodList_Button.TabIndex = 6;
            this.CookedFoodList_Button.Text = "Cooked Food List";
            this.CookedFoodList_Button.UseVisualStyleBackColor = true;
            this.CookedFoodList_Button.Click += new System.EventHandler(this.CookedFoodList_Button_Click);
            // 
            // ItemID_Label
            // 
            this.ItemID_Label.AutoSize = true;
            this.ItemID_Label.Font = new System.Drawing.Font("Franklin Gothic Medium", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ItemID_Label.ForeColor = System.Drawing.Color.Snow;
            this.ItemID_Label.Location = new System.Drawing.Point(9, 9);
            this.ItemID_Label.Name = "ItemID_Label";
            this.ItemID_Label.Size = new System.Drawing.Size(48, 17);
            this.ItemID_Label.TabIndex = 8;
            this.ItemID_Label.Text = "Item ID";
            // 
            // Variation_Label
            // 
            this.Variation_Label.AutoSize = true;
            this.Variation_Label.Font = new System.Drawing.Font("Franklin Gothic Medium", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Variation_Label.ForeColor = System.Drawing.Color.Snow;
            this.Variation_Label.Location = new System.Drawing.Point(88, 9);
            this.Variation_Label.Name = "Variation_Label";
            this.Variation_Label.Size = new System.Drawing.Size(130, 17);
            this.Variation_Label.TabIndex = 9;
            this.Variation_Label.Text = "Variation [Ingredients]";
            this.Variation_Label.Click += new System.EventHandler(this.Variation_Label_Click);
            // 
            // Slot1_PictureBox
            // 
            this.Slot1_PictureBox.BackgroundImage = global::CoreKeepersWorkshop.Properties.Resources.UIBackgroundSingle;
            this.Slot1_PictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Slot1_PictureBox.Location = new System.Drawing.Point(12, 55);
            this.Slot1_PictureBox.Name = "Slot1_PictureBox";
            this.Slot1_PictureBox.Size = new System.Drawing.Size(64, 64);
            this.Slot1_PictureBox.TabIndex = 10;
            this.Slot1_PictureBox.TabStop = false;
            this.Slot1_PictureBox.Click += new System.EventHandler(this.Slot1_PictureBox_Click);
            // 
            // Slot2_PictureBox
            // 
            this.Slot2_PictureBox.BackgroundImage = global::CoreKeepersWorkshop.Properties.Resources.UIBackgroundSingle;
            this.Slot2_PictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Slot2_PictureBox.Location = new System.Drawing.Point(91, 55);
            this.Slot2_PictureBox.Name = "Slot2_PictureBox";
            this.Slot2_PictureBox.Size = new System.Drawing.Size(64, 64);
            this.Slot2_PictureBox.TabIndex = 11;
            this.Slot2_PictureBox.TabStop = false;
            this.Slot2_PictureBox.Click += new System.EventHandler(this.Slot2_PictureBox_Click);
            // 
            // Slot3_PictureBox
            // 
            this.Slot3_PictureBox.BackgroundImage = global::CoreKeepersWorkshop.Properties.Resources.UIBackgroundSingle;
            this.Slot3_PictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Slot3_PictureBox.Location = new System.Drawing.Point(172, 55);
            this.Slot3_PictureBox.Name = "Slot3_PictureBox";
            this.Slot3_PictureBox.Size = new System.Drawing.Size(64, 64);
            this.Slot3_PictureBox.TabIndex = 12;
            this.Slot3_PictureBox.TabStop = false;
            this.Slot3_PictureBox.Click += new System.EventHandler(this.Slot3_PictureBox_Click);
            // 
            // Item1_Label
            // 
            this.Item1_Label.AutoSize = true;
            this.Item1_Label.Font = new System.Drawing.Font("Franklin Gothic Medium", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Item1_Label.ForeColor = System.Drawing.Color.Snow;
            this.Item1_Label.Location = new System.Drawing.Point(12, 122);
            this.Item1_Label.Name = "Item1_Label";
            this.Item1_Label.Size = new System.Drawing.Size(35, 15);
            this.Item1_Label.TabIndex = 13;
            this.Item1_Label.Text = "Item1";
            // 
            // Item2_Label
            // 
            this.Item2_Label.AutoSize = true;
            this.Item2_Label.Font = new System.Drawing.Font("Franklin Gothic Medium", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Item2_Label.ForeColor = System.Drawing.Color.Snow;
            this.Item2_Label.Location = new System.Drawing.Point(88, 122);
            this.Item2_Label.Name = "Item2_Label";
            this.Item2_Label.Size = new System.Drawing.Size(35, 15);
            this.Item2_Label.TabIndex = 14;
            this.Item2_Label.Text = "Item2";
            // 
            // Item3_Label
            // 
            this.Item3_Label.AutoSize = true;
            this.Item3_Label.Font = new System.Drawing.Font("Franklin Gothic Medium", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Item3_Label.ForeColor = System.Drawing.Color.Snow;
            this.Item3_Label.Location = new System.Drawing.Point(169, 122);
            this.Item3_Label.Name = "Item3_Label";
            this.Item3_Label.Size = new System.Drawing.Size(35, 15);
            this.Item3_Label.TabIndex = 15;
            this.Item3_Label.Text = "Item3";
            // 
            // ChangeRarity_Button
            // 
            this.ChangeRarity_Button.Location = new System.Drawing.Point(139, 142);
            this.ChangeRarity_Button.Name = "ChangeRarity_Button";
            this.ChangeRarity_Button.Size = new System.Drawing.Size(104, 22);
            this.ChangeRarity_Button.TabIndex = 5;
            this.ChangeRarity_Button.Text = "Change Rarity";
            this.ChangeRarity_Button.UseVisualStyleBackColor = true;
            this.ChangeRarity_Button.Click += new System.EventHandler(this.ChangeRarity_Button_Click);
            // 
            // Done_Button
            // 
            this.Done_Button.Location = new System.Drawing.Point(12, 226);
            this.Done_Button.Name = "Done_Button";
            this.Done_Button.Size = new System.Drawing.Size(230, 22);
            this.Done_Button.TabIndex = 8;
            this.Done_Button.Text = "Done";
            this.Done_Button.UseVisualStyleBackColor = true;
            this.Done_Button.Click += new System.EventHandler(this.Done_Button_Click);
            // 
            // QuickQuantitySelector_GroupBox
            // 
            this.QuickQuantitySelector_GroupBox.Controls.Add(this.CustomQuantity1_Button);
            this.QuickQuantitySelector_GroupBox.Controls.Add(this.CustomQuantity3_Button);
            this.QuickQuantitySelector_GroupBox.Controls.Add(this.CustomQuantity4_Button);
            this.QuickQuantitySelector_GroupBox.Controls.Add(this.CustomQuantity5_Button);
            this.QuickQuantitySelector_GroupBox.Controls.Add(this.CustomQuantity1_NumericUpDown);
            this.QuickQuantitySelector_GroupBox.Controls.Add(this.CustomQuantity3_NumericUpDown);
            this.QuickQuantitySelector_GroupBox.Controls.Add(this.CustomQuantity4_NumericUpDown);
            this.QuickQuantitySelector_GroupBox.Controls.Add(this.CustomQuantity5_NumericUpDown);
            this.QuickQuantitySelector_GroupBox.Controls.Add(this.CustomQuantity2_Button);
            this.QuickQuantitySelector_GroupBox.Controls.Add(this.CustomQuantity2_NumericUpDown);
            this.QuickQuantitySelector_GroupBox.ForeColor = System.Drawing.Color.Snow;
            this.QuickQuantitySelector_GroupBox.Location = new System.Drawing.Point(12, 254);
            this.QuickQuantitySelector_GroupBox.Name = "QuickQuantitySelector_GroupBox";
            this.QuickQuantitySelector_GroupBox.Size = new System.Drawing.Size(230, 51);
            this.QuickQuantitySelector_GroupBox.TabIndex = 16;
            this.QuickQuantitySelector_GroupBox.TabStop = false;
            this.QuickQuantitySelector_GroupBox.Text = "Quick Quantity Selector";
            // 
            // CustomQuantity1_Button
            // 
            this.CustomQuantity1_Button.ForeColor = System.Drawing.SystemColors.ControlText;
            this.CustomQuantity1_Button.Location = new System.Drawing.Point(6, 19);
            this.CustomQuantity1_Button.Name = "CustomQuantity1_Button";
            this.CustomQuantity1_Button.Size = new System.Drawing.Size(39, 22);
            this.CustomQuantity1_Button.TabIndex = 17;
            this.CustomQuantity1_Button.Text = "1";
            this.CustomQuantity1_Button.UseVisualStyleBackColor = true;
            this.CustomQuantity1_Button.Click += new System.EventHandler(this.CustomQuantity1_Button_Click);
            this.CustomQuantity1_Button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CustomQuantity1_Button_MouseDown);
            // 
            // CustomQuantity3_Button
            // 
            this.CustomQuantity3_Button.ForeColor = System.Drawing.SystemColors.ControlText;
            this.CustomQuantity3_Button.Location = new System.Drawing.Point(95, 19);
            this.CustomQuantity3_Button.Name = "CustomQuantity3_Button";
            this.CustomQuantity3_Button.Size = new System.Drawing.Size(39, 22);
            this.CustomQuantity3_Button.TabIndex = 19;
            this.CustomQuantity3_Button.Text = "500";
            this.CustomQuantity3_Button.UseVisualStyleBackColor = true;
            this.CustomQuantity3_Button.Click += new System.EventHandler(this.CustomQuantity3_Button_Click);
            this.CustomQuantity3_Button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CustomQuantity3_Button_MouseDown);
            // 
            // CustomQuantity4_Button
            // 
            this.CustomQuantity4_Button.ForeColor = System.Drawing.SystemColors.ControlText;
            this.CustomQuantity4_Button.Location = new System.Drawing.Point(140, 19);
            this.CustomQuantity4_Button.Name = "CustomQuantity4_Button";
            this.CustomQuantity4_Button.Size = new System.Drawing.Size(39, 22);
            this.CustomQuantity4_Button.TabIndex = 20;
            this.CustomQuantity4_Button.Text = "5000";
            this.CustomQuantity4_Button.UseVisualStyleBackColor = true;
            this.CustomQuantity4_Button.Click += new System.EventHandler(this.CustomQuantity4_Button_Click);
            this.CustomQuantity4_Button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CustomQuantity4_Button_MouseDown);
            // 
            // CustomQuantity5_Button
            // 
            this.CustomQuantity5_Button.ForeColor = System.Drawing.SystemColors.ControlText;
            this.CustomQuantity5_Button.Location = new System.Drawing.Point(185, 19);
            this.CustomQuantity5_Button.Name = "CustomQuantity5_Button";
            this.CustomQuantity5_Button.Size = new System.Drawing.Size(39, 22);
            this.CustomQuantity5_Button.TabIndex = 21;
            this.CustomQuantity5_Button.Text = "9999";
            this.CustomQuantity5_Button.UseVisualStyleBackColor = true;
            this.CustomQuantity5_Button.Click += new System.EventHandler(this.CustomQuantity5_Button_Click);
            this.CustomQuantity5_Button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CustomQuantity5_Button_MouseDown);
            // 
            // CustomQuantity1_NumericUpDown
            // 
            this.CustomQuantity1_NumericUpDown.Location = new System.Drawing.Point(6, 20);
            this.CustomQuantity1_NumericUpDown.Maximum = new decimal(new int[] {
            2000000000,
            0,
            0,
            0});
            this.CustomQuantity1_NumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.CustomQuantity1_NumericUpDown.Name = "CustomQuantity1_NumericUpDown";
            this.CustomQuantity1_NumericUpDown.Size = new System.Drawing.Size(38, 20);
            this.CustomQuantity1_NumericUpDown.TabIndex = 22;
            this.CustomQuantity1_NumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.CustomQuantity1_NumericUpDown.Visible = false;
            this.CustomQuantity1_NumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CustomQuantity1_NumericUpDown_KeyDown);
            // 
            // CustomQuantity3_NumericUpDown
            // 
            this.CustomQuantity3_NumericUpDown.Location = new System.Drawing.Point(95, 20);
            this.CustomQuantity3_NumericUpDown.Maximum = new decimal(new int[] {
            2000000000,
            0,
            0,
            0});
            this.CustomQuantity3_NumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.CustomQuantity3_NumericUpDown.Name = "CustomQuantity3_NumericUpDown";
            this.CustomQuantity3_NumericUpDown.Size = new System.Drawing.Size(38, 20);
            this.CustomQuantity3_NumericUpDown.TabIndex = 23;
            this.CustomQuantity3_NumericUpDown.Value = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.CustomQuantity3_NumericUpDown.Visible = false;
            this.CustomQuantity3_NumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CustomQuantity3_NumericUpDown_KeyDown);
            // 
            // CustomQuantity4_NumericUpDown
            // 
            this.CustomQuantity4_NumericUpDown.Location = new System.Drawing.Point(140, 20);
            this.CustomQuantity4_NumericUpDown.Maximum = new decimal(new int[] {
            2000000000,
            0,
            0,
            0});
            this.CustomQuantity4_NumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.CustomQuantity4_NumericUpDown.Name = "CustomQuantity4_NumericUpDown";
            this.CustomQuantity4_NumericUpDown.Size = new System.Drawing.Size(38, 20);
            this.CustomQuantity4_NumericUpDown.TabIndex = 23;
            this.CustomQuantity4_NumericUpDown.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.CustomQuantity4_NumericUpDown.Visible = false;
            this.CustomQuantity4_NumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CustomQuantity4_NumericUpDown_KeyDown);
            // 
            // CustomQuantity5_NumericUpDown
            // 
            this.CustomQuantity5_NumericUpDown.Location = new System.Drawing.Point(185, 20);
            this.CustomQuantity5_NumericUpDown.Maximum = new decimal(new int[] {
            2000000000,
            0,
            0,
            0});
            this.CustomQuantity5_NumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.CustomQuantity5_NumericUpDown.Name = "CustomQuantity5_NumericUpDown";
            this.CustomQuantity5_NumericUpDown.Size = new System.Drawing.Size(38, 20);
            this.CustomQuantity5_NumericUpDown.TabIndex = 23;
            this.CustomQuantity5_NumericUpDown.Value = new decimal(new int[] {
            9999,
            0,
            0,
            0});
            this.CustomQuantity5_NumericUpDown.Visible = false;
            this.CustomQuantity5_NumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CustomQuantity5_NumericUpDown_KeyDown);
            // 
            // CustomQuantity2_Button
            // 
            this.CustomQuantity2_Button.ForeColor = System.Drawing.SystemColors.ControlText;
            this.CustomQuantity2_Button.Location = new System.Drawing.Point(50, 19);
            this.CustomQuantity2_Button.Name = "CustomQuantity2_Button";
            this.CustomQuantity2_Button.Size = new System.Drawing.Size(39, 22);
            this.CustomQuantity2_Button.TabIndex = 18;
            this.CustomQuantity2_Button.Text = "50";
            this.CustomQuantity2_Button.UseVisualStyleBackColor = true;
            this.CustomQuantity2_Button.Click += new System.EventHandler(this.Button8_Click);
            this.CustomQuantity2_Button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CustomQuantity2_Button_MouseDown);
            // 
            // CustomQuantity2_NumericUpDown
            // 
            this.CustomQuantity2_NumericUpDown.Location = new System.Drawing.Point(50, 20);
            this.CustomQuantity2_NumericUpDown.Maximum = new decimal(new int[] {
            2000000000,
            0,
            0,
            0});
            this.CustomQuantity2_NumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.CustomQuantity2_NumericUpDown.Name = "CustomQuantity2_NumericUpDown";
            this.CustomQuantity2_NumericUpDown.Size = new System.Drawing.Size(38, 20);
            this.CustomQuantity2_NumericUpDown.TabIndex = 23;
            this.CustomQuantity2_NumericUpDown.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.CustomQuantity2_NumericUpDown.Visible = false;
            this.CustomQuantity2_NumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CustomQuantity2_NumericUpDown_KeyDown);
            // 
            // Skillset_Button
            // 
            this.Skillset_Button.Enabled = false;
            this.Skillset_Button.Location = new System.Drawing.Point(12, 170);
            this.Skillset_Button.Name = "Skillset_Button";
            this.Skillset_Button.Size = new System.Drawing.Size(60, 22);
            this.Skillset_Button.TabIndex = 17;
            this.Skillset_Button.Text = "Skillset";
            this.Skillset_Button.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Skillset_Button.UseVisualStyleBackColor = true;
            // 
            // Skillset_NumericUpDown
            // 
            this.Skillset_NumericUpDown.Location = new System.Drawing.Point(72, 171);
            this.Skillset_NumericUpDown.Maximum = new decimal(new int[] {
            2000000000,
            0,
            0,
            0});
            this.Skillset_NumericUpDown.Name = "Skillset_NumericUpDown";
            this.Skillset_NumericUpDown.Size = new System.Drawing.Size(60, 20);
            this.Skillset_NumericUpDown.TabIndex = 18;
            this.Skillset_NumericUpDown.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Skillset_Button_KeyDown);
            // 
            // SkillsetHelp_Button
            // 
            this.SkillsetHelp_Button.Location = new System.Drawing.Point(55, 170);
            this.SkillsetHelp_Button.Name = "SkillsetHelp_Button";
            this.SkillsetHelp_Button.Size = new System.Drawing.Size(17, 22);
            this.SkillsetHelp_Button.TabIndex = 19;
            this.SkillsetHelp_Button.Text = "?";
            this.SkillsetHelp_Button.UseVisualStyleBackColor = true;
            this.SkillsetHelp_Button.Click += new System.EventHandler(this.SkillsetHelp_Button_Click);
            // 
            // ItemEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(254, 317);
            this.Controls.Add(this.SkillsetHelp_Button);
            this.Controls.Add(this.Skillset_Button);
            this.Controls.Add(this.Skillset_NumericUpDown);
            this.Controls.Add(this.QuickQuantitySelector_GroupBox);
            this.Controls.Add(this.Done_Button);
            this.Controls.Add(this.ChangeRarity_Button);
            this.Controls.Add(this.Item3_Label);
            this.Controls.Add(this.Item2_Label);
            this.Controls.Add(this.Item1_Label);
            this.Controls.Add(this.Slot3_PictureBox);
            this.Controls.Add(this.Slot2_PictureBox);
            this.Controls.Add(this.Slot1_PictureBox);
            this.Controls.Add(this.Variation_Label);
            this.Controls.Add(this.ItemID_Label);
            this.Controls.Add(this.CookedFoodList_Button);
            this.Controls.Add(this.RemoveItem_Button);
            this.Controls.Add(this.VariationAnd_Button);
            this.Controls.Add(this.Variation2_NumericUpDown);
            this.Controls.Add(this.Quantity_Button);
            this.Controls.Add(this.Quantity_NumericUpDown);
            this.Controls.Add(this.ItemID_NumericUpDown);
            this.Controls.Add(this.Variation1_NumericUpDown);
            this.Controls.Add(this.VariationNumerical_NumericUpDown);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ItemEditor";
            this.Text = "Quick Item Editor";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ItemEditor_FormClosing);
            this.Load += new System.EventHandler(this.ItemEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ItemID_NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.VariationNumerical_NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Quantity_NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Variation1_NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Variation2_NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Slot1_PictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Slot2_PictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Slot3_PictureBox)).EndInit();
            this.QuickQuantitySelector_GroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.CustomQuantity1_NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CustomQuantity3_NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CustomQuantity4_NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CustomQuantity5_NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CustomQuantity2_NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Skillset_NumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.NumericUpDown ItemID_NumericUpDown;
        private System.Windows.Forms.NumericUpDown VariationNumerical_NumericUpDown;
        private System.Windows.Forms.NumericUpDown Quantity_NumericUpDown;
        private System.Windows.Forms.Button Quantity_Button;
        private System.Windows.Forms.NumericUpDown Variation1_NumericUpDown;
        private System.Windows.Forms.Button VariationAnd_Button;
        private System.Windows.Forms.NumericUpDown Variation2_NumericUpDown;
        private System.Windows.Forms.Button RemoveItem_Button;
        private System.Windows.Forms.Button CookedFoodList_Button;
        private System.Windows.Forms.Label ItemID_Label;
        private System.Windows.Forms.Label Variation_Label;
        private System.Windows.Forms.PictureBox Slot1_PictureBox;
        private System.Windows.Forms.PictureBox Slot2_PictureBox;
        private System.Windows.Forms.PictureBox Slot3_PictureBox;
        private System.Windows.Forms.Label Item1_Label;
        private System.Windows.Forms.Label Item2_Label;
        private System.Windows.Forms.Label Item3_Label;
        private System.Windows.Forms.Button ChangeRarity_Button;
        private System.Windows.Forms.Button Done_Button;
        private System.Windows.Forms.GroupBox QuickQuantitySelector_GroupBox;
        private System.Windows.Forms.Button CustomQuantity5_Button;
        private System.Windows.Forms.Button CustomQuantity4_Button;
        private System.Windows.Forms.Button CustomQuantity3_Button;
        private System.Windows.Forms.Button CustomQuantity2_Button;
        private System.Windows.Forms.Button CustomQuantity1_Button;
        private System.Windows.Forms.NumericUpDown CustomQuantity1_NumericUpDown;
        private System.Windows.Forms.NumericUpDown CustomQuantity2_NumericUpDown;
        private System.Windows.Forms.NumericUpDown CustomQuantity3_NumericUpDown;
        private System.Windows.Forms.NumericUpDown CustomQuantity4_NumericUpDown;
        private System.Windows.Forms.NumericUpDown CustomQuantity5_NumericUpDown;
        private System.Windows.Forms.Button Skillset_Button;
        private System.Windows.Forms.NumericUpDown Skillset_NumericUpDown;
        private System.Windows.Forms.Button SkillsetHelp_Button;
    }
}
