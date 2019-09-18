using IWshRuntimeLibrary;
using System;
using System.IO;

namespace NTMiner {
    public static class WindowsShortcut {
        public static void CreateShortcut(
            string directory, string shortcutName, string targetPath,
            string description = null, string iconLocation = null) {

            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }

            string shortcutPath = Path.Combine(directory, string.Format("{0}.lnk", shortcutName));
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = targetPath;
            shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
            shortcut.WindowStyle = 1;
            shortcut.Description = description;
            shortcut.IconLocation = string.IsNullOrWhiteSpace(iconLocation) ? targetPath : iconLocation;
            shortcut.Save();
        }

        public static void CreateShortcutOnDesktop(
            string shortcutName, string targetPath,
            string description = null, string iconLocation = null) {

            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            CreateShortcut(desktop, shortcutName, targetPath, description, iconLocation);
        }
    }
}
