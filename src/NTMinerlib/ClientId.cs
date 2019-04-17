using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace NTMiner {
    public static class ClientId {
        public static readonly string AppFileFullName = Process.GetCurrentProcess().MainModule.FileName;
        public static Guid Id { get; private set; }

        static ClientId() {
            if (!Directory.Exists(VirtualRoot.GlobalDirFullName)) {
                Directory.CreateDirectory(VirtualRoot.GlobalDirFullName);
            }
            Id = NTMinerRegistry.GetClientId();
        }

        private static Guid? kernelBrandId = null;
        public static Guid KernelBrandId {
            get {
                if (!kernelBrandId.HasValue) {
                    kernelBrandId = GetKernelBrandId(AppFileFullName);
                }
                return kernelBrandId.Value;
            }
        }

        public static void TagKernelBrandId(Guid kernelBrandId, string inputFileFullName, string outFileFullName) {
            string brand = $"KernelBrandId{kernelBrandId}KernelBrandId";
            string rawBrand = $"KernelBrandId{KernelBrandId}KernelBrandId";
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

        public static Guid GetKernelBrandId(string fileFullName) {
            int LEN = "KernelBrandId".Length;
            string rawBrand = $"KernelBrandId{Guid.Empty}KernelBrandId";
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
            Guid guid;
            Guid.TryParse(guidString, out guid);
            return guid;
        }
    }
}
