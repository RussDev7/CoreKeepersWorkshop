using System;
using System.Linq;
using System.Text;

using static CoreKeeperInventoryEditor.MainForm;

namespace CoreKeepersWorkshop
{
    /// <summary>
    /// Provides paging & pretty‑printing for console or in‑game chat commands.
    /// </summary>
    public class CommandReader
    {
        // Keeps formatting logic private; unit tests can target this directly.
        private static string FormatCommand(string command, string description, int maxLength)
        {
            int spacesToAdd = maxLength - command.Length;

            int offset = (spacesToAdd != 0) ? -1 : 0; // Apply a -1 offset if the spaces to add is not zero.
            return command + new string(' ', spacesToAdd + offset) + "\t - " + description;
        }

        public static string ReadCommands(int maxLines, int page, bool ingameText = false)
        {
            int totalPages = (int)Math.Ceiling((double)commands.Length / maxLines);
            if (page < 1 || page > totalPages)
            {
                return "Invalid page number. Total pages: " + totalPages;
            }

            int maxCommandLength = commands.Max(cmd => cmd.command.Length);

            StringBuilder sb = new();

            // Header block.
            if (ingameText)
                sb.AppendLine("=============================================");
            else
                sb.AppendLine("========================================================");
            sb.AppendLine(    "[] ---- Required Argument");
            sb.AppendLine(    "() ---- Optional Argument");
            sb.AppendLine(    "==============CONSOLE COMMANDS [PAGE " + page.ToString("D2") + "]==============");
            sb.AppendLine(    "");

            // Body block.
            int startIndex = (page - 1) * maxLines;
            int endIndex = Math.Min(startIndex + maxLines, commands.Length);

            for (int i = startIndex; i < endIndex; i++)
            {
                // Get the format based on where to display.
                if (!ingameText)
                {
                    // Format for console.

                    // Get string for richtextbox.
                    sb.AppendLine(FormatCommand(commands[i].command, commands[i].description, maxCommandLength));
                }
                else
                {
                    // Format for ingame.
                    // In‑game: No fancy spacing because proportional fonts vary width.

                    // Get string for richtextbox.
                    sb.AppendLine(commands[i].command + "\t - " + commands[i].description);
                }
            }

            // Footer block - Finish the ingame builder.
            if (ingameText)
            {
                sb.AppendLine("");
                sb.AppendLine("Use '/help " + (page + 1) + "' for more commands.");
            }

            return sb.ToString();
        }
    }
}
