using Memory;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Timers;
using System.Windows.Forms;

namespace CoreKeepersWorkshop
{
    // Painting on a pannel code referenced from: https://www.codeproject.com/Articles/198419/Painting-on-a-panel
    public partial class ChunkViewer : Form
    {
        readonly Form callarForm;
        public ChunkViewer(Form parentForm)
        {
            InitializeComponent();

            // Set double buffering.
            panel1.GetType().GetMethod("SetStyle", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(panel1, new object[] { System.Windows.Forms.ControlStyles.UserPaint | System.Windows.Forms.ControlStyles.AllPaintingInWmPaint | System.Windows.Forms.ControlStyles.DoubleBuffer, true });

            // Define form hook.
            callarForm = parentForm;
        }

        #region Variables

        // Define some varibles.
        public Mem MemLib = new Mem();
        readonly System.Timers.Timer timedEvents = new System.Timers.Timer();

        #endregion // End variables.

        #region Chunk Math Functions

        // Get the nearest XY position of a chunk based on position.
        public Vector2 GetChunk(Vector2 Position)
        {
            return new Vector2(IRoundTo((int)((Vector2)Position).X, 64), IRoundTo((int)((Vector2)Position).Y, 64));
        }

        // Algorithm for compairing two intagers.
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

            #region Set Form Locations

            // Set the forms active location based on previous save.
            this.Location = CoreKeepersWorkshop.Properties.Settings.Default.ChunkViewerLocation;
            #endregion

            #region Set Form Controls

            // Set controls based on saved settings.
            checkBox2.Checked = CoreKeepersWorkshop.Properties.Settings.Default.ChunkViewerMobs;
            checkBox3.Checked = CoreKeepersWorkshop.Properties.Settings.Default.ChunkViewerDebug;

            numericUpDown1.Visible = CoreKeepersWorkshop.Properties.Settings.Default.ChunkViewerDebug;
            numericUpDown1.Value = CoreKeepersWorkshop.Properties.Settings.Default.ChunkViewerDebugScale;

            // ActiveForm.Opacity = CoreKeepersWorkshop.Properties.Settings.Default.ChunkViewerOpacity;
            trackBar2.Value = (int)(CoreKeepersWorkshop.Properties.Settings.Default.ChunkViewerScale / 0.2);
            mapScale = CoreKeepersWorkshop.Properties.Settings.Default.ChunkViewerScale;

            this.Size = new Size((int)Math.Round(64 * mapScale) + 80, (int)Math.Round(64 * mapScale) + 160); // Form size.
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
            toolTip.SetToolTip(checkBox1, "Hides the main form making usability much better.");
            toolTip.SetToolTip(checkBox2, "Shows the chunks the game uses to spawn enemies.");
            toolTip.SetToolTip(checkBox3, "Hide or show debug tools.");
            toolTip.SetToolTip(numericUpDown1, "Adjust the display area for the grid box.");
            toolTip.SetToolTip(numericUpDown2, "Adjust the forms x-axis size offset.");
            toolTip.SetToolTip(numericUpDown3, "Adjust the forms y-axis size offset.");
            toolTip.SetToolTip(trackBar1, "Adjust the transparency of the form.");
            toolTip.SetToolTip(trackBar2, "Adjust the scale of the grid.");
            #endregion
        }

        // Do form closing events.
        private void ChunkViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Ensure mainform is unhidden.
            callarForm.Show();

            // Close existing timers.
            timedEvents.Stop();

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
                CoreKeepersWorkshop.Properties.Settings.Default.ChunkViewerLocation = this.Location;
            }
            catch (Exception)
            { } // Do nothing.
        }

        // Toggle debug mode.
        private void CheckBox3_CheckedChanged(object sender, EventArgs e)
        {
            // Hide or unhide controls.
            if (checkBox3.Checked)
            {
                // Disable
                numericUpDown1.Visible = true; // Show numericupdown.

                // Save some form settings.
                CoreKeepersWorkshop.Properties.Settings.Default.ChunkViewerDebug = true;
            }
            else
            {
                // Enable
                numericUpDown1.Visible = false; // Hide numericupdown.

                // Save some form settings.
                CoreKeepersWorkshop.Properties.Settings.Default.ChunkViewerDebug = false;
            }
        }

        // Toggle show mob grid.
        private void CheckBox2_CheckedChanged(object sender, EventArgs e)
        {
            // Save controls based on selection.
            if (checkBox2.Checked)
            {
                // Save some form settings.
                CoreKeepersWorkshop.Properties.Settings.Default.ChunkViewerMobs = true;
            }
            else
            {
                // Save some form settings.
                CoreKeepersWorkshop.Properties.Settings.Default.ChunkViewerMobs = false;
            }
        }

        // Save scale settings.
        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            // Save settings.
            CoreKeepersWorkshop.Properties.Settings.Default.ChunkViewerDebugScale = numericUpDown1.Value;
        }

        // Hide main form.
        private void CheckBox1_CheckedChanged(object sender, EventArgs e)
        {
            // Check if we are to hide or unhide the mainform.
            if (checkBox1.Checked)
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
        private void TrackBar1_ValueChanged(object sender, EventArgs e)
        {
            // Get the new value.
            double newOpacity = ((double)trackBar1.Value) / 100.0;
            ActiveForm.Opacity = newOpacity;

            // Save the opacity.
            // CoreKeepersWorkshop.Properties.Settings.Default.ChunkViewerOpacity = newOpacity;
        }

        // Adjust the grid scale.
        private void TrackBar2_ValueChanged(object sender, EventArgs e)
        {
            // Get the scale to multiply by to get a larger range.
            float sliderScale = 0.2f;

            // Set the map scale value.
            mapScale = sliderScale * trackBar2.Value;

            // Save the new scale.
            CoreKeepersWorkshop.Properties.Settings.Default.ChunkViewerScale = mapScale;

            // Define the offsets for the form size.
            int xOffset = 80;
            int yOffset = 160;
            if (checkBox3.Checked)
            {
                xOffset = (int)numericUpDown2.Value;
                yOffset = (int)numericUpDown3.Value;
            }

            // Set the form size.
            this.Size = new Size((int)Math.Round(64 * mapScale) + xOffset, (int)Math.Round(64 * mapScale) + yOffset);
        }
        #endregion

        #region Do Painting

        // Get the players position timer.
        float mapScale = 4.2f;
        Vector2 playersPosition = new Vector2(0, 0);
        private void TimedEvents(Object source, ElapsedEventArgs e)
        {
            // Change the forms background opacity.

            // Define string from host form.
            string playerToolAddress = CoreKeepersWorkshop.Properties.Settings.Default.ChunkViewerAddress;

            if (MemLib.OpenProcess("CoreKeeper") && playerToolAddress != null)
            {
                // Get the addresses.
                // Updated offsets 04Oct23.
                string positionXAddress = BigInteger.Add(BigInteger.Parse(playerToolAddress, NumberStyles.HexNumber), BigInteger.Parse("56", NumberStyles.Integer)).ToString("X");
                string positionYAddress = BigInteger.Add(BigInteger.Parse(playerToolAddress, NumberStyles.HexNumber), BigInteger.Parse("64", NumberStyles.Integer)).ToString("X");

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

                // Refresh the pannel to draw.
                panel1.Refresh();
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
        private void Panel1_Paint(object sender, PaintEventArgs e)
        {
            // Get location varibles for this chunk.
            Vector2 currentChunk = GetChunk(new Vector2(playersPosition.X / mapScale, playersPosition.Y / mapScale)); // Chunk location.
            Vector2 corner1 = new Vector2(currentChunk.X, currentChunk.Y);                                                                                                                   // A: 0,0
            Vector2 corner2 = new Vector2(currentChunk.X, IsNegative(currentChunk.Y) ? currentChunk.Y - 63 : currentChunk.Y + 63);                                                           // B: 0,63
            Vector2 corner3 = new Vector2(IsNegative(currentChunk.X) ? currentChunk.X - 63 : currentChunk.X + 63, IsNegative(currentChunk.Y) ? currentChunk.Y - 63 : currentChunk.Y + 63);   // C: 63,63
            Vector2 corner4 = new Vector2(IsNegative(currentChunk.X) ? currentChunk.X - 63 : currentChunk.X + 63, currentChunk.Y);                                                           // D: 63,0

            // Check if values are -64 to 0, adjust offsets. // Fix for negitive chunks.
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
                corner3 = new Vector2(corner3.X, (IsNegative(currentChunk.Y) ? currentChunk.Y - 63 : currentChunk.Y + 63) * -1);   // C: 63,63

                // corner1 = new Vector2(currentChunk.X, currentChunk.Y * -1);                                                                                                                  // A: 0,0
                // corner4 = new Vector2(IsNegative(currentChunk.X) ? currentChunk.X - 63 : currentChunk.X + 63, currentChunk.Y * -1);                                                          // D: 63,0
            }

            // Draw coordinates.
            e.Graphics.DrawString((checkBox3.Checked ? "A" : "") + "(" + (IsNegative(playersPosition.X) ? corner4.X : corner1.X) + ", " + (IsNegative(playersPosition.Y) ? corner2.Y : corner1.Y) + ")", new Font("Arial", 10, FontStyle.Italic), Brushes.Red, new Point(32 - 2, (int)Math.Round(64 * mapScale) + 40));                              // A: 0,0
            e.Graphics.DrawString((checkBox3.Checked ? "B" : "") + "(" + (IsNegative(playersPosition.X) ? corner3.X : corner2.X) + ", " + (IsNegative(playersPosition.Y) ? corner1.Y : corner2.Y) + ")", new Font("Arial", 10, FontStyle.Italic), Brushes.Red, new Point(32 - 2, 16));                                                               // B: 0,63
            e.Graphics.DrawString((checkBox3.Checked ? "C" : "") + "(" + (IsNegative(playersPosition.X) ? corner2.X : corner3.X) + ", " + (IsNegative(playersPosition.Y) ? corner4.Y : corner3.Y) + ")", new Font("Arial", 10, FontStyle.Italic), Brushes.Red, new Point((int)Math.Round(64 * mapScale) - 32, 16));                                  // C: 63,63
            e.Graphics.DrawString((checkBox3.Checked ? "D" : "") + "(" + (IsNegative(playersPosition.X) ? corner1.X : corner4.X) + ", " + (IsNegative(playersPosition.Y) ? corner3.Y : corner4.Y) + ")", new Font("Arial", 10, FontStyle.Italic), Brushes.Red, new Point((int)Math.Round(64 * mapScale) - 32, (int)Math.Round(64 * mapScale) + 40)); // D: 63,0
            e.Graphics.DrawString("CHUNK: [" + (IsNegative(playersPosition.X) ? (currentChunk.X / 64) - 1 : (currentChunk.X / 64)) + ", " + (IsNegative(playersPosition.Y) ? (currentChunk.Y / 64) - 1 : (currentChunk.Y / 64)) + "]", new Font("Arial", 10, FontStyle.Italic), Brushes.Lime, new Point((((int)Math.Round(64 * mapScale) + 32) / 3) + 10, 16)); // Area

            // Begin graphics container
            GraphicsContainer containerState = e.Graphics.BeginContainer();

            // Flip the Y-Axis
            e.Graphics.ScaleTransform(1.0F, -1.0F);

            // Translate the drawing area accordingly
            var translateScale = (checkBox3.Checked) ? numericUpDown1.Value : (int)Math.Round(64 * mapScale) + 108;
            e.Graphics.TranslateTransform(0.0F, -(float)translateScale);

            //Apply a smoothing mode to smooth out the line.
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            // Draw a border around the chunk.
            e.Graphics.DrawRectangle(new Pen(Color.Lime, 3f), 32, 72, (int)Math.Round(64 * mapScale), (int)Math.Round(64 * mapScale));

            // Draw mob grid.
            if (checkBox2.Checked)
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
