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
                string directoryName = destDir;

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
                            FileStream streamWriter = File.Create(path);
                            const int bufferSize = 1024 * 30;
                            byte[] data = new byte[bufferSize];
                            StreamUtils.Copy(zipInputStream, streamWriter, data);
                            streamWriter.Close();
                        }
                        catch {
                            if (File.Exists(path)) {
                                string sha1 = HashUtil.Sha1(File.ReadAllBytes(path));
                                zipInputStream.Position = 0;
                                byte[] data = new byte[zipInputStream.Length];
                                zipInputStream.Read(data, 0, data.Length);
                                string sha2 = HashUtil.Sha1(data);
                                if (sha1 != sha2) {
                                    // 个别文件可能会由于正在被使用导致解压失败，此时判断一下目标文件是否和源文件sha1相同，
                                    // 如果相同视为无需解压视为解压成功无需记录异常
                                    throw;
                                }
                            }
                            else {
                                throw;
                            }
                        }
                    }
                }
            }
        }
    }
}
