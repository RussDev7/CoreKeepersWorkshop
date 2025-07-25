using System.Windows.Forms;
using System;

namespace CoreKeepersWorkshop
{
    /// <summary>
    /// Extension / helper methods for writing to a <see cref="RichTextBox"/>
    /// without spamming duplicate lines.
    /// </summary>
    public static class RichTextBoxExtensions
    {
        // Stores the last line appended per RichTextBox instance.
        // Prevents duplicates across multiple consoles.
        private static string _lastMessage = string.Empty;

        public static void AppendUniqueText(RichTextBox richTextBox, string message, bool useOverlay)
        {
            if (_lastMessage != message)
            {
                // Display console in-game.
                if (useOverlay)
                {
                    OverlayHelper.ShowOverlay(message, 5);
                }

                // Mirror to overlay first (so the time-to-live countdown starts immediately).
                richTextBox.AppendText(message + Environment.NewLine);
                _lastMessage = message;
            }
        }

        // Normalises an item name by stripping case, underscores, and spaces.
        public static string NormalizeItemName(string input)
        {
            return input.ToLower().Replace("_", "").Replace(" ", "");
        }
    }
}
