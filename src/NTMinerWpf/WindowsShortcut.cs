using IWshRuntimeLibrary;
using System.IO;

namespace NTMiner {
    public static class WindowsShortcut {
        public static void CreateShortcut(
            string linkFileFullName, string targetPath,
            string description = null, string iconLocation = null) {

            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(linkFileFullName);
            shortcut.TargetPath = targetPath;
            shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);
            shortcut.WindowStyle = 1;
            shortcut.Description = description;
            shortcut.IconLocation = string.IsNullOrWhiteSpace(iconLocation) ? targetPath : iconLocation;
            shortcut.Save();
        }

        public static string GetTargetPath(string linkFileFullName) {
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(linkFileFullName);
            return shortcut.TargetPath;
        }
    }
}
