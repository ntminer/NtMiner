using System;
using System.IO;
using System.Reflection;

namespace NTMiner.Core.Gpus.Impl.DeviceDetection {
    public static class DeviceDetectionUtil {
        public static void ExtractResource() {
            try {
                if (File.Exists(SpecialPath.DeviceDetectionPrinterFileFullName)) {
                    return;
                }
                Type type = typeof(DeviceDetectionUtil);
                Assembly assembly = type.Assembly;
                string[] names = new string[] {
                    Path.GetFileName(SpecialPath.DeviceDetectionPrinterFileFullName),
                    "cuda_device_detection.dll",
                    "opencl_device_detection.dll"
                };
                foreach (var name in names) {
                    using (var stream = assembly.GetManifestResourceStream(type, name)) {
                        byte[] data = new byte[stream.Length];
                        stream.Read(data, 0, data.Length);
                        File.WriteAllBytes(Path.Combine(SpecialPath.TempDirFullName, name), data);
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }
    }
}
