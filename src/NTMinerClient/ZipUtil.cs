using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;

namespace NTMiner {
    public static class ZipUtil {
        public static void DecompressZipFile(string zipFileFullName, string destDir) {
            if (string.IsNullOrEmpty(zipFileFullName)) {
                throw new ArgumentNullException(nameof(zipFileFullName));
            }
            if (!Directory.Exists(destDir)) {
                Directory.CreateDirectory(destDir);
            }
            FileInfo fileInfo = new FileInfo(zipFileFullName);
            if (!fileInfo.Exists) {
                throw new FileNotFoundException("file not found", zipFileFullName);
            }
            using (FileStream fs = fileInfo.OpenRead()) {
                Decompress(fs, destDir);
            }
        }

        private static void Decompress(Stream stream, string destDir) {
            if (stream == null) {
                throw new ArgumentNullException(nameof(stream));
            }
            if (string.IsNullOrEmpty(destDir)) {
                throw new ArgumentNullException(nameof(destDir));
            }
            using (ZipInputStream zipInputStream = new ZipInputStream(stream)) {
                ZipEntry theEntry;

                while ((theEntry = zipInputStream.GetNextEntry()) != null) {
                    string path = Path.Combine(destDir, theEntry.Name);
                    if (theEntry.IsDirectory) {
                        DirectoryInfo dir = new DirectoryInfo(path);
                        if (!dir.Exists) {
                            dir.Create();
                        }
                    }
                    else if (theEntry.IsFile) {
                        // 放进try catch保证一个文件的失败不影响另一个
                        try {
                            using (FileStream streamWriter = File.Create(path)) {
                                const int bufferSize = NTKeyword.IntK * 30;
                                byte[] data = new byte[bufferSize];
                                StreamUtils.Copy(zipInputStream, streamWriter, data);
                            }
                        }
                        catch(Exception e) {
                            Logger.ErrorDebugLine(e);
                        }
                    }
                }
            }
        }
    }
}
