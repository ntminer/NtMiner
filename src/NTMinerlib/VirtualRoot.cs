using NTMiner.Bus;
using NTMiner.Bus.DirectBus;
using NTMiner.Serialization;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace NTMiner {
    public static partial class VirtualRoot {
        public static readonly Stopwatch Stopwatch = new Stopwatch();

        public static readonly string AppFileFullName = Process.GetCurrentProcess().MainModule.FileName;
        public static Guid Id { get; private set; }

        public static bool IsMinerClient { get; private set; }

        public static void SetIsMinerClient(bool value) {
            IsMinerClient = value;
        }

        public static bool IsMinerStudio { get; private set; }

        public static void SetIsMinerStudio(bool value) {
            IsMinerStudio = value;
        }

        public static IObjectSerializer JsonSerializer { get; private set; }

        public static readonly IMessageDispatcher SMessageDispatcher;
        private static readonly ICmdBus SCommandBus;
        private static readonly IEventBus SEventBus;

        static VirtualRoot() {
            Id = NTMinerRegistry.GetClientId();
            JsonSerializer = new ObjectJsonSerializer();
            SMessageDispatcher = new MessageDispatcher();
            SCommandBus = new DirectCommandBus(SMessageDispatcher);
            SEventBus = new DirectEventBus(SMessageDispatcher);
        }

        public static Guid ConvertToGuid(object obj) {
            if (obj == null) {
                return Guid.Empty;
            }
            if (obj is Guid guid1) {
                return guid1;
            }
            if (obj is string s) {
                if (Guid.TryParse(s, out Guid guid)) {
                    return guid;
                }
            }
            return Guid.Empty;
        }

        public static void TagBrandId(string brandKeyword, Guid brandId, string inputFileFullName, string outFileFullName) {
            string brand = $"{brandKeyword}{brandId}{brandKeyword}";
            string rawBrand = $"{brandKeyword}{GetBrandId(inputFileFullName, brandKeyword)}{brandKeyword}";
            byte[] data = Encoding.UTF8.GetBytes(brand);
            byte[] rawData = Encoding.UTF8.GetBytes(rawBrand);
            if (data.Length != rawData.Length) {
                throw new InvalidProgramException();
            }
            byte[] source = File.ReadAllBytes(inputFileFullName);
            int index = 0;
            for (int i = 0; i < source.Length - rawData.Length; i++) {
                int j = 0;
                for (; j < rawData.Length; j++) {
                    if (source[i + j] != rawData[j]) {
                        break;
                    }
                }
                if (j == rawData.Length) {
                    index = i;
                    break;
                }
            }
            for (int i = index; i < index + data.Length; i++) {
                source[i] = data[i - index];
            }
            File.WriteAllBytes(outFileFullName, source);
        }

        public static Guid GetBrandId(string fileFullName, string keyword) {
#if DEBUG
            Stopwatch.Restart();
#endif
            Guid guid = Guid.Empty;
            int LEN = keyword.Length;
            if (fileFullName == AppFileFullName) {
                Assembly assembly = Assembly.GetEntryAssembly();
                string name = $"NTMiner.Brand.{keyword}";
                using (var stream = assembly.GetManifestResourceStream(name)) {
                    if (stream == null) {
                        return guid;
                    }
                    byte[] data = new byte[stream.Length];
                    stream.Read(data, 0, data.Length);
                    string rawBrand = Encoding.UTF8.GetString(data);
                    string guidString = rawBrand.Substring(LEN, rawBrand.Length - 2 * LEN);
                    Guid.TryParse(guidString, out guid);
                }
            }
            else {
                string rawBrand = $"{keyword}{Guid.Empty}{keyword}";
                byte[] rawData = Encoding.UTF8.GetBytes(rawBrand);
                int len = rawData.Length;
                byte[] source = File.ReadAllBytes(fileFullName);
                int index = 0;
                for (int i = 0; i < source.Length - len; i++) {
                    int j = 0;
                    for (; j < len; j++) {
                        if ((j < LEN || j > len - LEN) && source[i + j] != rawData[j]) {
                            break;
                        }
                    }
                    if (j == rawData.Length) {
                        index = i;
                        break;
                    }
                }
                string guidString = Encoding.UTF8.GetString(source, index + LEN, len - 2 * LEN);
                Guid.TryParse(guidString, out guid);
            }
#if DEBUG
            Write.DevWarn($"耗时{Stopwatch.ElapsedMilliseconds}毫秒 {typeof(VirtualRoot).Name}.GetBrandId");
#endif
            return guid;
        }
    }
}
