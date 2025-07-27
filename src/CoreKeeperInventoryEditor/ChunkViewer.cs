using CoreKeepersWorkshop.Properties;
using CoreKeeperInventoryEditor;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Globalization;
using System.Numerics;
using System.Drawing;
using System.Timers;
using Memory;
using System;

namespace CoreKeepersWorkshop
{
    // Painting on a panel code referenced from: https://www.codeproject.com/Articles/198419/Painting-on-a-panel
    public partial class ChunkViewer : Form
    {
        // Form initialization.
        private CustomFormStyler _formThemeStyler;
        readonly Form callarForm;
        public ChunkViewer(Form parentForm)
        {
            InitializeComponent();
            Load += (_, __) => _formThemeStyler = this.ApplyTheme(); // Load the forms theme.

            // Set double buffering.
            Content_Panel.GetType().GetMethod("SetStyle", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(Content_Panel, new object[] { System.Windows.Forms.ControlStyles.UserPaint | System.Windows.Forms.ControlStyles.AllPaintingInWmPaint | System.Windows.Forms.ControlStyles.DoubleBuffer, true });

            // Define form hook.
            callarForm = parentForm;
        }

        #region Fields

        // Define some variables.
        public Mem MemLib = new Mem();
        readonly System.Timers.Timer timedEvents = new System.Timers.Timer();

        private const int _defaultXOffset = 65;
        private const int _defaultYOffset = 160;

        #endregion // End variables.

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

        // Convert vectors to points.
        public static Point Vector2ToPoint(Vector2 vector2)
        {
            Point pt = new Point(
                (int)(vector2.X + 0.5f), (int)(vector2.Y + 0.5f));

            return pt;
        }
        #endregion // End player math.

        #region Form Load And Closing Events

        // Do loading events for the form.
        private void ChunkViewer_Load(object sender, EventArgs e)
        {
            #region Set Custom Cusror

            // Set the applications cursor.
            Cursor = new Cursor(CoreKeepersWorkshop.Properties.Resources.UICursor.GetHicon());
            #endregion

            #region Set Form Opacity

            // Set form opacity based on trackbars value saved setting (1 to 100 -> 0.01 to 1.0).
            // NOTE: This form has a seperate setting for opacity.
            // this.Opacity = Settings.Default.FormOpacity / 100.0;
            this.Opacity = Settings.Default.ChunkViewerOpacity / 100.0;
            #endregion

            #region Set Form Controls

            // Set controls based on saved settings.
            ShowEnemySpawnChunks_CheckBox.Checked = Settings.Default.ChunkViewerMobs;
            Debug_CheckBox.Checked                = Settings.Default.ChunkViewerDebug;

            TranslateScale_NumericUpDown.Visible  = Settings.Default.ChunkViewerDebug;
            TranslateScale_NumericUpDown.Value    = Settings.Default.ChunkViewerDebugScale;

            Scale_TrackBar.Value                  = (int)(Settings.Default.ChunkViewerScale / 0.2);
            mapScale                              = Settings.Default.ChunkViewerScale;

            this.Size                             = new Size((int)Math.Round(64 * mapScale) + _defaultXOffset, (int)Math.Round(64 * mapScale) + _defaultYOffset); // Form size.
            #endregion

            #region Timed Events

            // Start the timed events.
            timedEvents.Interval = 100; // Custom intervals.
            timedEvents.Elapsed += new ElapsedEventHandler(TimedEvents);
            timedEvents.Start();
            #endregion

            #region Tooltips

            // Create a new tooltip.
            ToolTip toolTip = new ToolTip()
            {
                AutoPopDelay = 5000,
                InitialDelay = 750
            };

            // Set tool texts.
            toolTip.SetToolTip(HideMainForm_CheckBox,         "Hides the main form making usability much better.");
            toolTip.SetToolTip(ShowEnemySpawnChunks_CheckBox, "Shows the chunks the game uses to spawn enemies.");
            toolTip.SetToolTip(Debug_CheckBox,                "Hide or show debug tools.");
            toolTip.SetToolTip(TranslateScale_NumericUpDown,  "Adjust the display area for the grid box.");
            toolTip.SetToolTip(XAxisOffset_NumericUpDown,     "Adjust the forms x-axis size offset.");
            toolTip.SetToolTip(YAxisOffset_NumericUpDown,     "Adjust the forms y-axis size offset.");
            toolTip.SetToolTip(Opacity_TrackBar,              "Adjust the transparency of the form.");
            toolTip.SetToolTip(Scale_TrackBar,                "Adjust the scale of the grid.");
            #endregion

            #region Set Form Locations

            // Set the forms active location based on previous save.
            if (ActiveForm != null) this.Location = Settings.Default.ChunkViewerLocation;
            #endregion
        }

        #region Control Logic

        // Do form closing events.
        private void ChunkViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Ensure mainform is unhidden.
            callarForm.Show();

            // Close existing timers.
            timedEvents.Stop();

            // Check if the "X" button was pressed to close form.
            // if (!new StackTrace().GetFrames().Any(x => x.GetMethod().Name == "Close"))
            if (_formThemeStyler.CloseButtonPressed) // Now capture the custom titlebar.
            {
                // User pressed the "X" button cancel task.
                // this.Close();
            }

            // Ensure we catch all closing exceptions. // Fix v1.3.3.
            try
            {
                // Save some form settings.
                Settings.Default.ChunkViewerLocation = this.Location;
            }
            catch (Exception)
            { } // Do nothing.
        }

        // Toggle debug mode.
        private void Debug_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Hide or unhide controls.
            if (Debug_CheckBox.Checked)
            {
                // Enable.
                TranslateScale_NumericUpDown.Visible = true;
                XAxisOffset_NumericUpDown.Visible = true;
                YAxisOffset_NumericUpDown.Visible = true;
                XAxisOffset_Label.Visible = true;
                YAxisOffset_Label.Visible = true;
                TranslateScale_Label.Visible = true;

                // Save some form settings.
                Settings.Default.ChunkViewerDebug = true;
            }
            else
            {
                // Disable.
                TranslateScale_NumericUpDown.Visible = false;
                XAxisOffset_NumericUpDown.Visible = false;
                YAxisOffset_NumericUpDown.Visible = false;
                XAxisOffset_Label.Visible = false;
                YAxisOffset_Label.Visible = false;
                TranslateScale_Label.Visible = false;

                // Save some form settings.
                Settings.Default.ChunkViewerDebug = false;
            }
        }

        // Toggle show mob grid.
        private void ShowEnemySpawnChunks_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Save controls based on selection.
            if (ShowEnemySpawnChunks_CheckBox.Checked)
            {
                // Save some form settings.
                Settings.Default.ChunkViewerMobs = true;
            }
            else
            {
                // Save some form settings.
                Settings.Default.ChunkViewerMobs = false;
            }
        }

        // Save scale settings.
        private void DisplayArea_NumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            // Save settings.
            Settings.Default.ChunkViewerDebugScale = TranslateScale_NumericUpDown.Value;
        }

        // Hide main form.
        private void HideMainForm_CheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Check if we are to hide or unhide the mainform.
            if (HideMainForm_CheckBox.Checked)
            {
                // Hide.
                callarForm.Hide();
            }
            else
            {
                // Unhide.
                callarForm.Show();
            }
        }

        // Change the opacity of the form.
        private void Opacity_TrackBar_ValueChanged(object sender, EventArgs e)
        {
            // Get the new value.
            int    rawOpacity = Opacity_TrackBar.Value;
            double newOpacity = (double)rawOpacity / 100.0;
            ActiveForm.Opacity = newOpacity;

            // Save the opacity.
            Settings.Default.ChunkViewerOpacity = rawOpacity;
        }

        // Adjust the grid scale.
        private void Scale_TrackBar_ValueChanged(object sender, EventArgs e)
        {
            // Get the scale to multiply by to get a larger range.
            float sliderScale = 0.2f;

            // Set the map scale value.
            mapScale = sliderScale * Scale_TrackBar.Value;

            // Save the new scale.
            Settings.Default.ChunkViewerScale = mapScale;

            // Define the offsets for the form size.
            int xOffset = _defaultXOffset;
            int yOffset = _defaultYOffset;
            if (Debug_CheckBox.Checked)
            {
                xOffset = (int)XAxisOffset_NumericUpDown.Value;
                yOffset = (int)YAxisOffset_NumericUpDown.Value;
            }

            // Set the form size.
            this.Size = new Size((int)Math.Round(64 * mapScale) + xOffset, (int)Math.Round(64 * mapScale) + yOffset);
        }
        #endregion

        #endregion

        #region Do Painting

        // Get the players position timer.
        float mapScale = 4.2f;
        Vector2 playersPosition = new Vector2(0, 0);
        private void TimedEvents(Object source, ElapsedEventArgs e)
        {
            // Change the forms background opacity.

            // Define string from host form.
            string playerToolAddress = Settings.Default.ChunkViewerAddress;

            if (MemLib.OpenProcess("CoreKeeper") && playerToolAddress != null)
            {
                // Get the addresses.
                // Updated offsets 04Oct23.
                // Updated offsets 07Feb25.
                string positionXAddress = BigInteger.Add(BigInteger.Parse(playerToolAddress, NumberStyles.HexNumber), BigInteger.Parse("0", NumberStyles.Integer)).ToString("X");
                string positionYAddress = BigInteger.Add(BigInteger.Parse(playerToolAddress, NumberStyles.HexNumber), BigInteger.Parse("4", NumberStyles.Integer)).ToString("X");

                float playerX = MemLib.ReadFloat(positionXAddress);
                float playerY = MemLib.ReadFloat(positionYAddress);

                // Chunk location.
                Vector2 currentChunk = new Vector2(0, 0);
                try
                {
                    currentChunk = GetChunk(new Vector2(playerX, playerY));
                }
                catch (Exception)
                { } // Do nothing.

                // Change text based on minimized window.
                this.Text = "Chunk Viewer --> Pos [X: " + playerX + " Y: " + playerY + "]";

                // Define the point to draw.
                playersPosition = new Vector2((float)(playerX * mapScale), (float)(playerY * mapScale));

                // Refresh the panel to draw.
                Content_Panel.Refresh();
            }
            else
            {
                // Process or addresses is not valid, kill form.
                timedEvents.Stop();
                callarForm.Show(); // Ensure mainform is unhidden.
                this.Close();
            }
        }

        // Return whether a double is negative.
        bool IsNegative(float n)
        {
            string nStr = n.ToString();
            if (nStr.IndexOf('-', 0, 1) != 0)
                return false;
            else
                return true;
        }

        // Do painting events for the panel.
        private void Content_Panel_Paint(object sender, PaintEventArgs e)
        {
            // Get location variables for this chunk.
            Vector2 currentChunk = GetChunk(new Vector2(playersPosition.X / mapScale, playersPosition.Y / mapScale)); // Chunk location.
            Vector2 corner1 = new Vector2(currentChunk.X, currentChunk.Y);                                                                                                                      // A: 0,0
            Vector2 corner2 = new Vector2(currentChunk.X, IsNegative(currentChunk.Y) ? currentChunk.Y - 63 : currentChunk.Y + 63);                                                              // B: 0,63
            Vector2 corner3 = new Vector2(IsNegative(currentChunk.X) ? currentChunk.X - 63 : currentChunk.X + 63, IsNegative(currentChunk.Y) ? currentChunk.Y - 63 : currentChunk.Y + 63);      // C: 63,63
            Vector2 corner4 = new Vector2(IsNegative(currentChunk.X) ? currentChunk.X - 63 : currentChunk.X + 63, currentChunk.Y);                                                              // D: 63,0

            // Check if values are -64 to 0, adjust offsets. // Fix for negative chunks.
            if (IsNegative(playersPosition.X) && (currentChunk.X / 64) - 1 == -1) // Adjust X axis.
            {
                corner3 = new Vector2((IsNegative(currentChunk.X) ? currentChunk.X - 63 : currentChunk.X + 63) * -1, IsNegative(currentChunk.Y) ? currentChunk.Y - 63 : currentChunk.Y + 63);   // C: 63,63
                corner4 = new Vector2((IsNegative(currentChunk.X) ? currentChunk.X - 63 : currentChunk.X + 63) * -1, currentChunk.Y);                                                           // D: 63,0
                
                // corner1 = new Vector2(currentChunk.X * -1, currentChunk.Y);                                                                                                                  // A: 0,0
                // corner2 = new Vector2(currentChunk.X * -1, IsNegative(currentChunk.Y) ? currentChunk.Y - 63 : currentChunk.Y + 63);                                                          // B: 0,63
            }
            if (IsNegative(playersPosition.Y) && (currentChunk.Y / 64) - 1 == -1) // Adjust Y axis.
            {
                corner2 = new Vector2(currentChunk.X, (IsNegative(currentChunk.Y) ? currentChunk.Y - 63 : currentChunk.Y + 63) * -1);                                                           // B: 0,63
                corner3 = new Vector2(corner3.X, (IsNegative(currentChunk.Y) ? currentChunk.Y - 63 : currentChunk.Y + 63) * -1);                                                                // C: 63,63

                // corner1 = new Vector2(currentChunk.X, currentChunk.Y * -1);                                                                                                                  // A: 0,0
                // corner4 = new Vector2(IsNegative(currentChunk.X) ? currentChunk.X - 63 : currentChunk.X + 63, currentChunk.Y * -1);                                                          // D: 63,0
            }

            // Draw coordinates.
            e.Graphics.DrawString((Debug_CheckBox.Checked ? "A " : "") + "(" + (IsNegative(playersPosition.X) ? corner4.X : corner1.X) + ", " + (IsNegative(playersPosition.Y) ? corner2.Y : corner1.Y) + ")", new Font("Arial", 10, FontStyle.Italic), Brushes.Red, new Point(32 - 2, (int)Math.Round(64 * mapScale) + 40));                                    // A: 0,0.
            e.Graphics.DrawString((Debug_CheckBox.Checked ? "B " : "") + "(" + (IsNegative(playersPosition.X) ? corner3.X : corner2.X) + ", " + (IsNegative(playersPosition.Y) ? corner1.Y : corner2.Y) + ")", new Font("Arial", 10, FontStyle.Italic), Brushes.Red, new Point(32 - 2, 16));                                                                     // B: 0,63.
            e.Graphics.DrawString((Debug_CheckBox.Checked ? "C " : "") + "(" + (IsNegative(playersPosition.X) ? corner2.X : corner3.X) + ", " + (IsNegative(playersPosition.Y) ? corner4.Y : corner3.Y) + ")", new Font("Arial", 10, FontStyle.Italic), Brushes.Red, new Point((int)Math.Round(64 * mapScale) - 32, 16));                                        // C: 63,63.
            e.Graphics.DrawString((Debug_CheckBox.Checked ? "D " : "") + "(" + (IsNegative(playersPosition.X) ? corner1.X : corner4.X) + ", " + (IsNegative(playersPosition.Y) ? corner3.Y : corner4.Y) + ")", new Font("Arial", 10, FontStyle.Italic), Brushes.Red, new Point((int)Math.Round(64 * mapScale) - 32, (int)Math.Round(64 * mapScale) + 40));       // D: 63,0.
            e.Graphics.DrawString("CHUNK: [" + (IsNegative(playersPosition.X) ? (currentChunk.X / 64) - 1 : (currentChunk.X / 64)) + ", " + (IsNegative(playersPosition.Y) ? (currentChunk.Y / 64) - 1 : (currentChunk.Y / 64)) + "]", new Font("Arial", 10, FontStyle.Italic), Brushes.Lime, new Point((((int)Math.Round(64 * mapScale) + 32) / 3) + 10, 16));  // Area.

            // Begin graphics container
            GraphicsContainer containerState = e.Graphics.BeginContainer();

            // Flip the Y-Axis
            e.Graphics.ScaleTransform(1.0F, -1.0F);

            // Translate the drawing area accordingly
            var translateScale = (Debug_CheckBox.Checked) ? TranslateScale_NumericUpDown.Value : (int)Math.Round(64 * mapScale) + 108;
            e.Graphics.TranslateTransform(0.0F, -(float)translateScale);

            //Apply a smoothing mode to smooth out the line.
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            // Draw a border around the chunk.
            e.Graphics.DrawRectangle(new Pen(Color.Lime, 3f), 32, 72, (int)Math.Round(64 * mapScale), (int)Math.Round(64 * mapScale));

            // Draw mob grid.
            if (ShowEnemySpawnChunks_CheckBox.Checked)
            {
                int numOfCells = 4;
                int cellSize = 16;
                for (int y = 0; y < numOfCells; ++y)
                {
                    if (y != 0)
                        e.Graphics.DrawLine(new Pen(Color.Gray, 1f), (0) + 32, (y * cellSize * mapScale) + 72, (numOfCells * cellSize * mapScale) + 32, (y * cellSize * mapScale) + 72);
                }

                for (int x = 0; x < numOfCells; ++x)
                {
                    if (x != 0)
                        e.Graphics.DrawLine(new Pen(Color.Gray, 1f), (x * cellSize * mapScale) + 32, (0) + 72, (x * cellSize * mapScale) + 32, (numOfCells * cellSize * mapScale) + 72);
                }
            }

            // Whatever you draw now (using this graphics context) will appear as
            // though (0,0) were at the bottom left corner

            // Translate xy to only the first chunk.
            int firstChunkX = (playersPosition.X >= 0) ? (int)(playersPosition.X - (64 * (currentChunk.X / 64) * mapScale)) : (int)(playersPosition.X - (64 * (currentChunk.X / 64) * mapScale) + (64 * mapScale));
            int firstChunkY = (playersPosition.Y >= 0) ? (int)(playersPosition.Y - (64 * (currentChunk.Y / 64) * mapScale)) : (int)(playersPosition.Y - (64 * (currentChunk.Y / 64) * mapScale) + (64 * mapScale));

            // Draw circle.
            float CurrentWidth = 10;
            e.Graphics.DrawEllipse(new Pen(Color.White, 0.5f), (firstChunkX + 32) - (CurrentWidth / 2), (firstChunkY + 72) - (CurrentWidth / 2), CurrentWidth, CurrentWidth);

            // End graphics container
            e.Graphics.EndContainer(containerState);
        }
        #endregion // End painting.
    }
}
