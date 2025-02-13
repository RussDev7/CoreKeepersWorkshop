namespace CoreKeepersWorkshop
{
    partial class ChunkViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChunkViewer));
            this.Main_Panel = new System.Windows.Forms.Panel();
            this.YAxisOffset_NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.XAxisOffset_NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.DisplayArea_NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.HideMainForm_CheckBox = new System.Windows.Forms.CheckBox();
            this.ShowEnemySpawnChunks_CheckBox = new System.Windows.Forms.CheckBox();
            this.Opacity_TrackBar = new System.Windows.Forms.TrackBar();
            this.Opacity_Label = new System.Windows.Forms.Label();
            this.Debug_CheckBox = new System.Windows.Forms.CheckBox();
            this.Scale_TrackBar = new System.Windows.Forms.TrackBar();
            this.Scale_Label = new System.Windows.Forms.Label();
            this.Main_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.YAxisOffset_NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.XAxisOffset_NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DisplayArea_NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Opacity_TrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Scale_TrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // Main_Panel
            // 
            this.Main_Panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Main_Panel.BackColor = System.Drawing.Color.Black;
            this.Main_Panel.Controls.Add(this.YAxisOffset_NumericUpDown);
            this.Main_Panel.Controls.Add(this.XAxisOffset_NumericUpDown);
            this.Main_Panel.Controls.Add(this.DisplayArea_NumericUpDown);
            this.Main_Panel.Location = new System.Drawing.Point(-1, -1);
            this.Main_Panel.Name = "Main_Panel";
            this.Main_Panel.Size = new System.Drawing.Size(338, 338);
            this.Main_Panel.TabIndex = 0;
            this.Main_Panel.Paint += new System.Windows.Forms.PaintEventHandler(this.Panel1_Paint);
            // 
            // YAxisOffset_NumericUpDown
            // 
            this.YAxisOffset_NumericUpDown.Location = new System.Drawing.Point(69, 88);
            this.YAxisOffset_NumericUpDown.Maximum = new decimal(new int[] {
            700,
            0,
            0,
            0});
            this.YAxisOffset_NumericUpDown.Name = "YAxisOffset_NumericUpDown";
            this.YAxisOffset_NumericUpDown.Size = new System.Drawing.Size(120, 20);
            this.YAxisOffset_NumericUpDown.TabIndex = 7;
            this.YAxisOffset_NumericUpDown.Value = new decimal(new int[] {
            160,
            0,
            0,
            0});
            this.YAxisOffset_NumericUpDown.Visible = false;
            // 
            // XAxisOffset_NumericUpDown
            // 
            this.XAxisOffset_NumericUpDown.Location = new System.Drawing.Point(69, 62);
            this.XAxisOffset_NumericUpDown.Maximum = new decimal(new int[] {
            700,
            0,
            0,
            0});
            this.XAxisOffset_NumericUpDown.Name = "XAxisOffset_NumericUpDown";
            this.XAxisOffset_NumericUpDown.Size = new System.Drawing.Size(120, 20);
            this.XAxisOffset_NumericUpDown.TabIndex = 6;
            this.XAxisOffset_NumericUpDown.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            this.XAxisOffset_NumericUpDown.Visible = false;
            // 
            // DisplayArea_NumericUpDown
            // 
            this.DisplayArea_NumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DisplayArea_NumericUpDown.Location = new System.Drawing.Point(134, 318);
            this.DisplayArea_NumericUpDown.Maximum = new decimal(new int[] {
            700,
            0,
            0,
            0});
            this.DisplayArea_NumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.DisplayArea_NumericUpDown.Name = "DisplayArea_NumericUpDown";
            this.DisplayArea_NumericUpDown.Size = new System.Drawing.Size(62, 20);
            this.DisplayArea_NumericUpDown.TabIndex = 8;
            this.DisplayArea_NumericUpDown.Value = new decimal(new int[] {
            364,
            0,
            0,
            0});
            this.DisplayArea_NumericUpDown.Visible = false;
            this.DisplayArea_NumericUpDown.ValueChanged += new System.EventHandler(this.NumericUpDown1_ValueChanged);
            // 
            // HideMainForm_CheckBox
            // 
            this.HideMainForm_CheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.HideMainForm_CheckBox.AutoSize = true;
            this.HideMainForm_CheckBox.Location = new System.Drawing.Point(166, 366);
            this.HideMainForm_CheckBox.Name = "HideMainForm_CheckBox";
            this.HideMainForm_CheckBox.Size = new System.Drawing.Size(100, 17);
            this.HideMainForm_CheckBox.TabIndex = 4;
            this.HideMainForm_CheckBox.Text = "Hide Main Form";
            this.HideMainForm_CheckBox.UseVisualStyleBackColor = true;
            this.HideMainForm_CheckBox.CheckedChanged += new System.EventHandler(this.CheckBox1_CheckedChanged);
            // 
            // ShowEnemySpawnChunks_CheckBox
            // 
            this.ShowEnemySpawnChunks_CheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ShowEnemySpawnChunks_CheckBox.AutoSize = true;
            this.ShowEnemySpawnChunks_CheckBox.Location = new System.Drawing.Point(166, 343);
            this.ShowEnemySpawnChunks_CheckBox.Name = "ShowEnemySpawnChunks_CheckBox";
            this.ShowEnemySpawnChunks_CheckBox.Size = new System.Drawing.Size(163, 17);
            this.ShowEnemySpawnChunks_CheckBox.TabIndex = 3;
            this.ShowEnemySpawnChunks_CheckBox.Text = "Show Enemy Spawn Chunks";
            this.ShowEnemySpawnChunks_CheckBox.UseVisualStyleBackColor = true;
            this.ShowEnemySpawnChunks_CheckBox.CheckedChanged += new System.EventHandler(this.CheckBox2_CheckedChanged);
            // 
            // Opacity_TrackBar
            // 
            this.Opacity_TrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Opacity_TrackBar.Location = new System.Drawing.Point(1, 360);
            this.Opacity_TrackBar.Maximum = 100;
            this.Opacity_TrackBar.Minimum = 20;
            this.Opacity_TrackBar.Name = "Opacity_TrackBar";
            this.Opacity_TrackBar.Size = new System.Drawing.Size(73, 45);
            this.Opacity_TrackBar.TabIndex = 1;
            this.Opacity_TrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.Opacity_TrackBar.Value = 100;
            this.Opacity_TrackBar.ValueChanged += new System.EventHandler(this.TrackBar1_ValueChanged);
            // 
            // Opacity_Label
            // 
            this.Opacity_Label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Opacity_Label.AutoSize = true;
            this.Opacity_Label.Location = new System.Drawing.Point(15, 344);
            this.Opacity_Label.Name = "Opacity_Label";
            this.Opacity_Label.Size = new System.Drawing.Size(43, 13);
            this.Opacity_Label.TabIndex = 4;
            this.Opacity_Label.Text = "Opacity";
            // 
            // Debug_CheckBox
            // 
            this.Debug_CheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Debug_CheckBox.AutoSize = true;
            this.Debug_CheckBox.Location = new System.Drawing.Point(271, 366);
            this.Debug_CheckBox.Name = "Debug_CheckBox";
            this.Debug_CheckBox.Size = new System.Drawing.Size(58, 17);
            this.Debug_CheckBox.TabIndex = 5;
            this.Debug_CheckBox.Text = "Debug";
            this.Debug_CheckBox.UseVisualStyleBackColor = true;
            this.Debug_CheckBox.CheckedChanged += new System.EventHandler(this.CheckBox3_CheckedChanged);
            // 
            // Scale_TrackBar
            // 
            this.Scale_TrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Scale_TrackBar.LargeChange = 1;
            this.Scale_TrackBar.Location = new System.Drawing.Point(74, 360);
            this.Scale_TrackBar.Maximum = 100;
            this.Scale_TrackBar.Minimum = 1;
            this.Scale_TrackBar.Name = "Scale_TrackBar";
            this.Scale_TrackBar.Size = new System.Drawing.Size(73, 45);
            this.Scale_TrackBar.TabIndex = 2;
            this.Scale_TrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.Scale_TrackBar.Value = 21;
            this.Scale_TrackBar.ValueChanged += new System.EventHandler(this.TrackBar2_ValueChanged);
            // 
            // Scale_Label
            // 
            this.Scale_Label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Scale_Label.AutoSize = true;
            this.Scale_Label.Location = new System.Drawing.Point(91, 344);
            this.Scale_Label.Name = "Scale_Label";
            this.Scale_Label.Size = new System.Drawing.Size(34, 13);
            this.Scale_Label.TabIndex = 7;
            this.Scale_Label.Text = "Scale";
            // 
            // ChunkViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.ClientSize = new System.Drawing.Size(334, 388);
            this.Controls.Add(this.Scale_Label);
            this.Controls.Add(this.Debug_CheckBox);
            this.Controls.Add(this.Opacity_Label);
            this.Controls.Add(this.Opacity_TrackBar);
            this.Controls.Add(this.ShowEnemySpawnChunks_CheckBox);
            this.Controls.Add(this.HideMainForm_CheckBox);
            this.Controls.Add(this.Main_Panel);
            this.Controls.Add(this.Scale_TrackBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChunkViewer";
            this.Text = "Chunk Viewer --> Live Tracking";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChunkViewer_FormClosing);
            this.Load += new System.EventHandler(this.ChunkViewer_Load);
            this.Main_Panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.YAxisOffset_NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.XAxisOffset_NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DisplayArea_NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Opacity_TrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Scale_TrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel Main_Panel;
        private System.Windows.Forms.CheckBox HideMainForm_CheckBox;
        private System.Windows.Forms.CheckBox ShowEnemySpawnChunks_CheckBox;
        private System.Windows.Forms.TrackBar Opacity_TrackBar;
        private System.Windows.Forms.Label Opacity_Label;
        private System.Windows.Forms.NumericUpDown DisplayArea_NumericUpDown;
        private System.Windows.Forms.CheckBox Debug_CheckBox;
        private System.Windows.Forms.TrackBar Scale_TrackBar;
        private System.Windows.Forms.Label Scale_Label;
        private System.Windows.Forms.NumericUpDown YAxisOffset_NumericUpDown;
        private System.Windows.Forms.NumericUpDown XAxisOffset_NumericUpDown;
    }
}