using System;
using System.IO;
using System.Linq;

namespace NTMiner.Core.Kernels {
    public static class KernelExtensions {
        public static string GetProcessName(this IKernel kernel) {
            string commandName = GetCommandName(kernel);
            if (string.IsNullOrEmpty(commandName)) {
                return string.Empty;
            }
            return Path.GetFileNameWithoutExtension(commandName);
        }

        public static string GetCommandName(this IKernel kernel) {
            try {
                if (kernel == null) {
                    return string.Empty;
                }
                IKernelInput kernelInput;
                if (kernel.KernelInputId == Guid.Empty || !NTMinerRoot.Instance.KernelInputSet.TryGetKernelInput(kernel.KernelInputId, out kernelInput)) {
                    return string.Empty;
                }
                string args = kernelInput.Args;
                if (!string.IsNullOrEmpty(args)) {
                    args = args.Trim();
                }
                else {
                    return string.Empty;
                }
                string commandName;
                if (args[0] == '"') {
                    int index = args.IndexOf('"', 1);
                    commandName = args.Substring(1, index - 1);
                }
                else {
                    int firstSpaceIndex = args.IndexOf(' ');
                    if (firstSpaceIndex != -1) {
                        commandName = args.Substring(0, args.IndexOf(' '));
                    }
                    else {
                        commandName = args;
                    }
                }
                return commandName;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return string.Empty;
            }
        }

        public static bool IsSupported(this IKernel kernel, ICoin coin) {
            // 群控客户端和无显卡的电脑的GpuSet类型都是空
            if (NTMinerRoot.Instance.GpuSet.GpuType == GpuType.Empty) {
                return true;
            }
            foreach (var item in NTMinerRoot.Instance.CoinKernelSet.Where(a => a.CoinId == coin.GetId() && a.KernelId == kernel.GetId())) {
                if (item.SupportedGpu == SupportedGpu.Both) {
                    return true;
                }
                if (item.SupportedGpu == SupportedGpu.NVIDIA && NTMinerRoot.Instance.GpuSet.GpuType == GpuType.NVIDIA) {
                    return true;
                }
                if (item.SupportedGpu == SupportedGpu.AMD && NTMinerRoot.Instance.GpuSet.GpuType == GpuType.AMD) {
                    return true;
                }
            }
            return false;
        }

        public static string GetKernelDirFullName(this IKernel kernel) {
            if (kernel == null || string.IsNullOrEmpty(kernel.Package)) {
                return string.Empty;
            }
            return Path.Combine(SpecialPath.KernelsDirFullName, Path.GetFileNameWithoutExtension(kernel.Package));
        }

        public static string GetPackageFileFullName(this IKernel kernel) {
            if (kernel == null || string.IsNullOrEmpty(kernel.Package)) {
                return string.Empty;
            }
            return Path.Combine(SpecialPath.PackagesDirFullName, kernel.Package);
        }

        public static bool IsPackageFileExist(this IKernel kernel) {
            if (kernel == null || string.IsNullOrEmpty(kernel.Package)) {
                return false;
            }
            string fileFullName = GetPackageFileFullName(kernel);
            return File.Exists(fileFullName);
        }

        public static string GetDownloadFileFullName(this IKernel kernel) {
            if (kernel == null || string.IsNullOrEmpty(kernel.Package)) {
                return string.Empty;
            }
            return Path.Combine(SpecialPath.DownloadDirFullName, kernel.Package);
        }

        public static bool ExtractPackage(this IKernel kernel) {
            try {
                string kernelDir = GetKernelDirFullName(kernel);
                if (string.IsNullOrEmpty(kernelDir)) {
                    return false;
                }
                if (!Directory.Exists(kernelDir)) {
                    Directory.CreateDirectory(kernelDir);
                }
                string packageZipFileFullName = GetPackageFileFullName(kernel);
                if (string.IsNullOrEmpty(packageZipFileFullName)) {
                    return false;
                }
                if (!File.Exists(packageZipFileFullName)) {
                    Write.DevDebug($"试图解压的{packageZipFileFullName}文件不存在");
                    return false;
                }
                ZipUtil.DecompressZipFile(packageZipFileFullName, kernelDir);
                return true;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return false;
            }
        }
    }
}
