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
            this.Content_Panel = new System.Windows.Forms.Panel();
            this.TranslateScale_Label = new System.Windows.Forms.Label();
            this.YAxisOffset_Label = new System.Windows.Forms.Label();
            this.XAxisOffset_Label = new System.Windows.Forms.Label();
            this.YAxisOffset_NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.XAxisOffset_NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.TranslateScale_NumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.HideMainForm_CheckBox = new System.Windows.Forms.CheckBox();
            this.ShowEnemySpawnChunks_CheckBox = new System.Windows.Forms.CheckBox();
            this.Opacity_TrackBar = new System.Windows.Forms.TrackBar();
            this.Opacity_Label = new System.Windows.Forms.Label();
            this.Debug_CheckBox = new System.Windows.Forms.CheckBox();
            this.Scale_TrackBar = new System.Windows.Forms.TrackBar();
            this.Scale_Label = new System.Windows.Forms.Label();
            this.MainContentPanel2_Panel = new System.Windows.Forms.Panel();
            this.MainContentPanel1_Panel = new System.Windows.Forms.Panel();
            this.Content_Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.YAxisOffset_NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.XAxisOffset_NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.TranslateScale_NumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Opacity_TrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Scale_TrackBar)).BeginInit();
            this.MainContentPanel2_Panel.SuspendLayout();
            this.MainContentPanel1_Panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // Content_Panel
            // 
            this.Content_Panel.BackColor = System.Drawing.Color.Black;
            this.Content_Panel.Controls.Add(this.TranslateScale_Label);
            this.Content_Panel.Controls.Add(this.YAxisOffset_Label);
            this.Content_Panel.Controls.Add(this.XAxisOffset_Label);
            this.Content_Panel.Controls.Add(this.YAxisOffset_NumericUpDown);
            this.Content_Panel.Controls.Add(this.XAxisOffset_NumericUpDown);
            this.Content_Panel.Controls.Add(this.TranslateScale_NumericUpDown);
            this.Content_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Content_Panel.Location = new System.Drawing.Point(0, 0);
            this.Content_Panel.Name = "Content_Panel";
            this.Content_Panel.Size = new System.Drawing.Size(334, 334);
            this.Content_Panel.TabIndex = 0;
            this.Content_Panel.Paint += new System.Windows.Forms.PaintEventHandler(this.Content_Panel_Paint);
            // 
            // TranslateScale_Label
            // 
            this.TranslateScale_Label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TranslateScale_Label.AutoSize = true;
            this.TranslateScale_Label.ForeColor = System.Drawing.Color.Snow;
            this.TranslateScale_Label.Location = new System.Drawing.Point(128, 319);
            this.TranslateScale_Label.Name = "TranslateScale_Label";
            this.TranslateScale_Label.Size = new System.Drawing.Size(17, 13);
            this.TranslateScale_Label.TabIndex = 11;
            this.TranslateScale_Label.Text = "T:";
            this.TranslateScale_Label.Visible = false;
            // 
            // YAxisOffset_Label
            // 
            this.YAxisOffset_Label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.YAxisOffset_Label.AutoSize = true;
            this.YAxisOffset_Label.ForeColor = System.Drawing.Color.Snow;
            this.YAxisOffset_Label.Location = new System.Drawing.Point(257, 319);
            this.YAxisOffset_Label.Name = "YAxisOffset_Label";
            this.YAxisOffset_Label.Size = new System.Drawing.Size(17, 13);
            this.YAxisOffset_Label.TabIndex = 10;
            this.YAxisOffset_Label.Text = "Y:";
            this.YAxisOffset_Label.Visible = false;
            // 
            // XAxisOffset_Label
            // 
            this.XAxisOffset_Label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.XAxisOffset_Label.AutoSize = true;
            this.XAxisOffset_Label.ForeColor = System.Drawing.Color.Snow;
            this.XAxisOffset_Label.Location = new System.Drawing.Point(3, 319);
            this.XAxisOffset_Label.Name = "XAxisOffset_Label";
            this.XAxisOffset_Label.Size = new System.Drawing.Size(17, 13);
            this.XAxisOffset_Label.TabIndex = 9;
            this.XAxisOffset_Label.Text = "X:";
            this.XAxisOffset_Label.Visible = false;
            // 
            // YAxisOffset_NumericUpDown
            // 
            this.YAxisOffset_NumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.YAxisOffset_NumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.YAxisOffset_NumericUpDown.Location = new System.Drawing.Point(276, 316);
            this.YAxisOffset_NumericUpDown.Maximum = new decimal(new int[] {
            700,
            0,
            0,
            0});
            this.YAxisOffset_NumericUpDown.Name = "YAxisOffset_NumericUpDown";
            this.YAxisOffset_NumericUpDown.Size = new System.Drawing.Size(58, 18);
            this.YAxisOffset_NumericUpDown.TabIndex = 7;
            this.YAxisOffset_NumericUpDown.Value = new decimal(new int[] {
            160,
            0,
            0,
            0});
            this.YAxisOffset_NumericUpDown.Visible = false;
            this.YAxisOffset_NumericUpDown.ValueChanged += new System.EventHandler(this.Scale_TrackBar_ValueChanged);
            // 
            // XAxisOffset_NumericUpDown
            // 
            this.XAxisOffset_NumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.XAxisOffset_NumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.XAxisOffset_NumericUpDown.Location = new System.Drawing.Point(22, 316);
            this.XAxisOffset_NumericUpDown.Maximum = new decimal(new int[] {
            700,
            0,
            0,
            0});
            this.XAxisOffset_NumericUpDown.Name = "XAxisOffset_NumericUpDown";
            this.XAxisOffset_NumericUpDown.Size = new System.Drawing.Size(58, 18);
            this.XAxisOffset_NumericUpDown.TabIndex = 6;
            this.XAxisOffset_NumericUpDown.Value = new decimal(new int[] {
            65,
            0,
            0,
            0});
            this.XAxisOffset_NumericUpDown.Visible = false;
            this.XAxisOffset_NumericUpDown.ValueChanged += new System.EventHandler(this.Scale_TrackBar_ValueChanged);
            // 
            // TranslateScale_NumericUpDown
            // 
            this.TranslateScale_NumericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TranslateScale_NumericUpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TranslateScale_NumericUpDown.Location = new System.Drawing.Point(148, 316);
            this.TranslateScale_NumericUpDown.Maximum = new decimal(new int[] {
            700,
            0,
            0,
            0});
            this.TranslateScale_NumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.TranslateScale_NumericUpDown.Name = "TranslateScale_NumericUpDown";
            this.TranslateScale_NumericUpDown.Size = new System.Drawing.Size(58, 18);
            this.TranslateScale_NumericUpDown.TabIndex = 8;
            this.TranslateScale_NumericUpDown.Value = new decimal(new int[] {
            364,
            0,
            0,
            0});
            this.TranslateScale_NumericUpDown.Visible = false;
            this.TranslateScale_NumericUpDown.ValueChanged += new System.EventHandler(this.DisplayArea_NumericUpDown_ValueChanged);
            // 
            // HideMainForm_CheckBox
            // 
            this.HideMainForm_CheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.HideMainForm_CheckBox.AutoSize = true;
            this.HideMainForm_CheckBox.Location = new System.Drawing.Point(167, 30);
            this.HideMainForm_CheckBox.Name = "HideMainForm_CheckBox";
            this.HideMainForm_CheckBox.Size = new System.Drawing.Size(100, 17);
            this.HideMainForm_CheckBox.TabIndex = 4;
            this.HideMainForm_CheckBox.Text = "Hide Main Form";
            this.HideMainForm_CheckBox.UseVisualStyleBackColor = true;
            this.HideMainForm_CheckBox.CheckedChanged += new System.EventHandler(this.HideMainForm_CheckBox_CheckedChanged);
            // 
            // ShowEnemySpawnChunks_CheckBox
            // 
            this.ShowEnemySpawnChunks_CheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ShowEnemySpawnChunks_CheckBox.AutoSize = true;
            this.ShowEnemySpawnChunks_CheckBox.Location = new System.Drawing.Point(167, 7);
            this.ShowEnemySpawnChunks_CheckBox.Name = "ShowEnemySpawnChunks_CheckBox";
            this.ShowEnemySpawnChunks_CheckBox.Size = new System.Drawing.Size(163, 17);
            this.ShowEnemySpawnChunks_CheckBox.TabIndex = 3;
            this.ShowEnemySpawnChunks_CheckBox.Text = "Show Enemy Spawn Chunks";
            this.ShowEnemySpawnChunks_CheckBox.UseVisualStyleBackColor = true;
            this.ShowEnemySpawnChunks_CheckBox.CheckedChanged += new System.EventHandler(this.ShowEnemySpawnChunks_CheckBox_CheckedChanged);
            // 
            // Opacity_TrackBar
            // 
            this.Opacity_TrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Opacity_TrackBar.Location = new System.Drawing.Point(2, 24);
            this.Opacity_TrackBar.Maximum = 100;
            this.Opacity_TrackBar.Minimum = 20;
            this.Opacity_TrackBar.Name = "Opacity_TrackBar";
            this.Opacity_TrackBar.Size = new System.Drawing.Size(73, 45);
            this.Opacity_TrackBar.TabIndex = 1;
            this.Opacity_TrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.Opacity_TrackBar.Value = 100;
            this.Opacity_TrackBar.ValueChanged += new System.EventHandler(this.Opacity_TrackBar_ValueChanged);
            // 
            // Opacity_Label
            // 
            this.Opacity_Label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Opacity_Label.AutoSize = true;
            this.Opacity_Label.Location = new System.Drawing.Point(16, 8);
            this.Opacity_Label.Name = "Opacity_Label";
            this.Opacity_Label.Size = new System.Drawing.Size(43, 13);
            this.Opacity_Label.TabIndex = 4;
            this.Opacity_Label.Text = "Opacity";
            // 
            // Debug_CheckBox
            // 
            this.Debug_CheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Debug_CheckBox.AutoSize = true;
            this.Debug_CheckBox.Location = new System.Drawing.Point(272, 30);
            this.Debug_CheckBox.Name = "Debug_CheckBox";
            this.Debug_CheckBox.Size = new System.Drawing.Size(58, 17);
            this.Debug_CheckBox.TabIndex = 5;
            this.Debug_CheckBox.Text = "Debug";
            this.Debug_CheckBox.UseVisualStyleBackColor = true;
            this.Debug_CheckBox.CheckedChanged += new System.EventHandler(this.Debug_CheckBox_CheckedChanged);
            // 
            // Scale_TrackBar
            // 
            this.Scale_TrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Scale_TrackBar.LargeChange = 1;
            this.Scale_TrackBar.Location = new System.Drawing.Point(75, 24);
            this.Scale_TrackBar.Maximum = 100;
            this.Scale_TrackBar.Minimum = 1;
            this.Scale_TrackBar.Name = "Scale_TrackBar";
            this.Scale_TrackBar.Size = new System.Drawing.Size(73, 45);
            this.Scale_TrackBar.TabIndex = 2;
            this.Scale_TrackBar.TickStyle = System.Windows.Forms.TickStyle.None;
            this.Scale_TrackBar.Value = 21;
            this.Scale_TrackBar.ValueChanged += new System.EventHandler(this.Scale_TrackBar_ValueChanged);
            // 
            // Scale_Label
            // 
            this.Scale_Label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.Scale_Label.AutoSize = true;
            this.Scale_Label.Location = new System.Drawing.Point(92, 8);
            this.Scale_Label.Name = "Scale_Label";
            this.Scale_Label.Size = new System.Drawing.Size(34, 13);
            this.Scale_Label.TabIndex = 7;
            this.Scale_Label.Text = "Scale";
            // 
            // MainContentPanel2_Panel
            // 
            this.MainContentPanel2_Panel.Controls.Add(this.Scale_Label);
            this.MainContentPanel2_Panel.Controls.Add(this.Opacity_Label);
            this.MainContentPanel2_Panel.Controls.Add(this.HideMainForm_CheckBox);
            this.MainContentPanel2_Panel.Controls.Add(this.Debug_CheckBox);
            this.MainContentPanel2_Panel.Controls.Add(this.Scale_TrackBar);
            this.MainContentPanel2_Panel.Controls.Add(this.ShowEnemySpawnChunks_CheckBox);
            this.MainContentPanel2_Panel.Controls.Add(this.Opacity_TrackBar);
            this.MainContentPanel2_Panel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.MainContentPanel2_Panel.Location = new System.Drawing.Point(0, 334);
            this.MainContentPanel2_Panel.Name = "MainContentPanel2_Panel";
            this.MainContentPanel2_Panel.Size = new System.Drawing.Size(334, 54);
            this.MainContentPanel2_Panel.TabIndex = 1;
            // 
            // MainContentPanel1_Panel
            // 
            this.MainContentPanel1_Panel.Controls.Add(this.Content_Panel);
            this.MainContentPanel1_Panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainContentPanel1_Panel.Location = new System.Drawing.Point(0, 0);
            this.MainContentPanel1_Panel.Name = "MainContentPanel1_Panel";
            this.MainContentPanel1_Panel.Size = new System.Drawing.Size(334, 334);
            this.MainContentPanel1_Panel.TabIndex = 2;
            // 
            // ChunkViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.ClientSize = new System.Drawing.Size(334, 388);
            this.Controls.Add(this.MainContentPanel1_Panel);
            this.Controls.Add(this.MainContentPanel2_Panel);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(12)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ChunkViewer";
            this.Text = "Chunk Viewer --> Live Tracking";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChunkViewer_FormClosing);
            this.Load += new System.EventHandler(this.ChunkViewer_Load);
            this.Content_Panel.ResumeLayout(false);
            this.Content_Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.YAxisOffset_NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.XAxisOffset_NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.TranslateScale_NumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Opacity_TrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Scale_TrackBar)).EndInit();
            this.MainContentPanel2_Panel.ResumeLayout(false);
            this.MainContentPanel2_Panel.PerformLayout();
            this.MainContentPanel1_Panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel Content_Panel;
        private System.Windows.Forms.CheckBox HideMainForm_CheckBox;
        private System.Windows.Forms.CheckBox ShowEnemySpawnChunks_CheckBox;
        private System.Windows.Forms.TrackBar Opacity_TrackBar;
        private System.Windows.Forms.Label Opacity_Label;
        private System.Windows.Forms.NumericUpDown TranslateScale_NumericUpDown;
        private System.Windows.Forms.CheckBox Debug_CheckBox;
        private System.Windows.Forms.TrackBar Scale_TrackBar;
        private System.Windows.Forms.Label Scale_Label;
        private System.Windows.Forms.NumericUpDown YAxisOffset_NumericUpDown;
        private System.Windows.Forms.NumericUpDown XAxisOffset_NumericUpDown;
        private System.Windows.Forms.Panel MainContentPanel2_Panel;
        private System.Windows.Forms.Panel MainContentPanel1_Panel;
        private System.Windows.Forms.Label YAxisOffset_Label;
        private System.Windows.Forms.Label XAxisOffset_Label;
        private System.Windows.Forms.Label TranslateScale_Label;
    }
}