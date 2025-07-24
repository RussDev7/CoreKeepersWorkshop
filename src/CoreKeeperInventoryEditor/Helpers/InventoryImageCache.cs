using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System;

namespace CoreKeepersWorkshop
{
    public class InventoryImageData
    {
        public Image Image { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }
        public string Variation { get; set; }
        public string SkillSet { get; set; } // Optional, can be null or empty if not present.
    }

    class InventoryImageCache
    {
        // Key: category index (int), Value: list of InventoryImageData objects.
        public static Dictionary<int, List<InventoryImageData>> CategoryImages = new();

        public static bool IsLoaded { get; private set; } = false;

        public static void LoadAllImages(IEnumerable<string> folderNames)
        {
            if (IsLoaded) return;
            CategoryImages.Clear();

            int i = 0;
            foreach (string folder in folderNames)
            {
                var imageDataList = new List<InventoryImageData>();
                string dirPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"assets\Inventory\", new DirectoryInfo(folder).Name);

                if (!Directory.Exists(dirPath)) { CategoryImages[i++] = imageDataList; continue; }

                foreach (var file in Directory.GetFiles(dirPath, "*.png"))
                {
                    var fi = new FileInfo(file);

                    // Expects format: "name,id,variation.png".
                    string[] filenameData = fi.Name.Split(',');

                    // Skip if not a valid image file.
                    if (filenameData[0] == "desktop.ini" || filenameData[0] == "Thumbs.db")
                        continue;

                    // Defensive: Ensure we have all fields.
                    if (filenameData.Length < 3)
                        continue;

                    try
                    {
                        // Remove ".png" extension from the last segment.
                        string lastPart = filenameData[filenameData.Length - 1];
                        string lastPartNoExt = lastPart.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                            ? lastPart.Substring(0, lastPart.Length - 4)
                            : lastPart;

                        // Determine which field is 'variation' and which (if any) is 'skillset'.
                        string variation;
                        string skillSet = null;

                        if (filenameData.Length == 3)
                        {
                            variation = lastPartNoExt;
                            // No skillSet
                        }
                        else if (filenameData.Length >= 4)
                        {
                            variation = filenameData[2];
                            skillSet = lastPartNoExt; // Last is always skillset if 4 segments.
                        }
                        else
                        {
                            variation = "";
                        }

                        imageDataList.Add(new InventoryImageData
                        {
                            Image = ImageFast.FromFile(file),         // Load image.
                            Name = filenameData[0].Replace("_", " "), // Display name (replace _'s with spaces).
                            Id = filenameData[1],                     // Item ID.
                            Variation = variation,                    // Variation (w/o extension).
                            SkillSet = skillSet                       // SkillSet (optional).
                        });
                    }
                    catch (Exception) { } // Swallow safely.
                }

                CategoryImages[i++] = imageDataList;
            }

            IsLoaded = true;
        }
    }
}
