using System;
using System.IO;
using System.Text;

namespace NTMiner.Net {
    public static class Hosts {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="position">-2表示hosts文件不存在，-1表示对应的host记录不存在，非负数表示对应的host记录的字节位置</param>
        /// <param name="hostsPath">测试用</param>
        /// <returns></returns>
        public static string GetIp(string host, out long position, string hostsPath = null) {
            if (string.IsNullOrEmpty(hostsPath)) {
                hostsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers\\etc\\hosts");
            }
            if (!File.Exists(hostsPath)) {
                position = - 2;
                return string.Empty;
            }
            using (FileStream fs = new FileStream(hostsPath, FileMode.Open))
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8)) {
                while (true) {
                    long p = sr.BaseStream.Position;
                    string line = sr.ReadLine();
                    if (!string.IsNullOrWhiteSpace(line)) {
                        line = line.Trim();
                        if (line[0] == '#') {
                            continue;
                        }
                        string[] tuple = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (tuple.Length < 2) {
                            continue;
                        }
                        if (tuple[1] == host) {
                            position = p;
                            return tuple[0];
                        }
                    }
                    if (sr.EndOfStream) {
                        break;
                    }
                }
            }
            position = -1;
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="host"></param>
        /// <param name="ip">空值表示删除对应的host记录</param>
        /// <param name="hostsPath">测试用</param>
        public static void SetHost(string host, string ip, string hostsPath = null) {
            GetIp(host, out long position, hostsPath);
            if (position == -2) {
                File.WriteAllText(hostsPath, $"{ip} {host}");
                return;
            }
            if (string.IsNullOrEmpty(hostsPath)) {
                hostsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers\\etc\\hosts");
            }
            //通常情况下这个文件是只读的，所以写入之前要取消只读
            File.SetAttributes(hostsPath, File.GetAttributes(hostsPath) & (~FileAttributes.ReadOnly));
            byte[] buffer = new byte[]{ };
            using (MemoryStream ms = new MemoryStream())
            using (StreamWriter sw = new StreamWriter(ms))
            using (FileStream fs = new FileStream(hostsPath, FileMode.OpenOrCreate, FileAccess.Read))
            using (StreamReader sr = new StreamReader(fs)) {
                bool writed = false;
                while (true) {
                    long p = sr.BaseStream.Position;
                    string line = sr.ReadLine();
                    if (p == position) {
                        if (!string.IsNullOrEmpty(ip)) {
                            sw.WriteLine($"{ip} {host}");
                        }
                        writed = true;
                    }
                    else {
                        sw.WriteLine(line);
                    }
                    if (sr.EndOfStream) {
                        break;
                    }
                }
                if (!writed) {
                    sw.WriteLine($"{ip} {host}");
                }
                sw.Flush();
                buffer = ms.ToArray();
            }
            File.WriteAllBytes(hostsPath, buffer);
        }
    }
}
