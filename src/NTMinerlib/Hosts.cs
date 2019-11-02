using System;
using System.IO;
using System.Text;

namespace NTMiner {
    public static class Hosts {
        public static long GetHost(string host, string hostsPath = null) {
            if (string.IsNullOrEmpty(hostsPath)) {
                hostsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers\\etc\\hosts");
            }
            if (!File.Exists(hostsPath)) {
                return -2;
            }
            using (FileStream fs = new FileStream(hostsPath, FileMode.Open))
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8)) {
                while (!sr.EndOfStream) {
                    long position = sr.BaseStream.Position;
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
                            return position;
                        }
                    }
                }
            }
            return -1;
        }

        public static void SetHost(string host, string ip, string hostsPath = null) {
            long position = GetHost(host);
            if (position == -2) {
                File.WriteAllText(hostsPath, $"{ip} {host}");
                return;
            }
            if (string.IsNullOrEmpty(hostsPath)) {
                hostsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers\\etc\\hosts");
            }
            byte[] buffer = new byte[]{ };
            using (MemoryStream ms = new MemoryStream())
            using (StreamWriter sw = new StreamWriter(ms))
            using (FileStream fs = new FileStream(hostsPath, FileMode.Open))
            using (StreamReader sr = new StreamReader(fs)) {
                if (sr.EndOfStream) {
                    if (!string.IsNullOrEmpty(ip)) {
                        sw.WriteLine($"{ip} {host}");
                    }
                }
                else {
                    while (!sr.EndOfStream) {
                        if (sr.BaseStream.Position == position) {
                            if (!string.IsNullOrEmpty(ip)) {
                                sw.WriteLine($"{ip} {host}");
                            }
                        }
                        else {
                            sw.WriteLine(sr.ReadLine());
                        }
                    }
                }
                sw.BaseStream.Position = 0;
                buffer = new byte[sw.BaseStream.Length];
                sw.BaseStream.Read(buffer, 0, (int)sw.BaseStream.Length);
            }
            File.WriteAllBytes(hostsPath, buffer);
        }
    }
}
