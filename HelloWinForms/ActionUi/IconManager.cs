using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace HelloWinForms.ActionUi
{
    public class IconManager
    {
        private Dictionary<string, Dictionary<string, string>> iconPaths;

        public IconManager()
        {
            iconPaths = new Dictionary<string, Dictionary<string, string>>();
        }

        public void LoadIconsFromDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                string[] catalogDirectories = Directory.GetDirectories(directoryPath);

                foreach (string catalogDirectory in catalogDirectories)
                {
                    string catalog = Path.GetFileName(catalogDirectory);
                    LoadIconsForCatalog(catalogDirectory, catalog);
                }
            }
            else
            {
                Console.WriteLine($"Directory not found: {directoryPath}");
            }
        }

        private void LoadIconsForCatalog(string catalogDirectory, string catalog)
        {
            string[] iconFiles = Directory.GetFiles(catalogDirectory, "*.png"); // Assuming icons are PNG files

            foreach (string iconFile in iconFiles)
            {
                string name = Path.GetFileNameWithoutExtension(iconFile);

                RegisterIcon(catalog, name, iconFile);
            }
        }

        private void RegisterIcon(string catalog, string actionName, string iconPath)
        {
            if (!iconPaths.ContainsKey(catalog))
            {
                iconPaths[catalog] = new Dictionary<string, string>();
            }

            if (!iconPaths[catalog].ContainsKey(actionName))
            {
                iconPaths[catalog][actionName] = iconPath;
            }
            else
            {
                Console.WriteLine($"Icon for action with catalog '{catalog}' and name '{actionName}' already registered.");
            }
        }

        public Image GetIcon(string catalog, string actionName)
        {
            if (iconPaths.ContainsKey(catalog) && iconPaths[catalog].ContainsKey(actionName))
            {
                string iconPath = iconPaths[catalog][actionName];
                if (File.Exists(iconPath))
                {
                    return Image.FromFile(iconPath);
                }
                else
                {
                    Console.WriteLine($"Icon file not found for action with catalog '{catalog}' and name '{actionName}'.");
                }
            }
            else
            {
                Console.WriteLine($"Icon not registered for action with catalog '{catalog}' and name '{actionName}'.");
            }

            // Return a default icon or null based on your requirements
            return null;
        }

        public void DisplayIconOnButton(Button button, string catalog, string actionName)
        {
            Image icon = GetIcon(catalog, actionName);

            if (icon != null)
            {
                button.Image = icon;
            }
            else
            {
                // Optionally, set a default icon or handle the absence of an icon
                Console.WriteLine("Unable to display icon on the button.");
            }
        }
    }
}
