using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace NTMiner {
    public static class CosturaUtil {
        /// <summary>
        /// 解压缩出来入口程序集中的被Fody Costura压缩的dll保存在磁盘上以供阅读，保存在家目录的costura文件夹下。
        /// </summary>
        public static void ExtractCosturaCompressedDlls() {
            Assembly assembly = Assembly.GetEntryAssembly();
            string[] resourceNames = assembly.GetManifestResourceNames();
            string costuraDirFullName = Path.Combine(HomePath.HomeDirFullName, "costura");
            if (!Directory.Exists(costuraDirFullName)) {
                Directory.CreateDirectory(costuraDirFullName);
            }
            foreach (var resourceName in resourceNames) {
                if (resourceName.StartsWith("costura.") && resourceName.EndsWith(".compressed")) {
                    string destinationFileName = resourceName.Substring("costura.".Length, resourceName.Length - "costura..compressed".Length);
                    string destinationFileFullName = Path.Combine(costuraDirFullName, destinationFileName);
                    var sourceStream = assembly.GetManifestResourceStream(resourceName);
                    using (var destinationFileStream = File.Create(destinationFileFullName))
                    using (var decompressionStream = new DeflateStream(sourceStream, CompressionMode.Decompress)) {
                        decompressionStream.CopyTo(destinationFileStream);
                    }
                }
            }
            Process.Start(costuraDirFullName);
        }
    }
}
