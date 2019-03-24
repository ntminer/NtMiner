using System;
using System.Collections.Generic;
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

        public static string GetCommandName(this IKernel kernel, bool fromHelpArg = false) {
            try {
                if (kernel == null) {
                    return string.Empty;
                }
                IKernelInput kernelInput;
                if (kernel.KernelInputId == Guid.Empty || !NTMinerRoot.Current.KernelInputSet.TryGetKernelInput(kernel.KernelInputId, out kernelInput)) {
                    return string.Empty;
                }
                string args = kernelInput.Args;
                if (fromHelpArg) {
                    args = kernel.HelpArg;
                }
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
                Logger.ErrorDebugLine(e.Message, e);
                return string.Empty;
            }
        }

        public static bool IsSupported(this IKernel kernel) {
            if (VirtualRoot.IsMinerStudio) {
                return true;
            }
            foreach (var item in NTMinerRoot.Current.CoinKernelSet.Where(a => a.KernelId == kernel.GetId())) {
                if (item.SupportedGpu == SupportedGpu.Both) {
                    return true;
                }
                if (item.SupportedGpu == SupportedGpu.NVIDIA && NTMinerRoot.Current.GpuSet.GpuType == GpuType.NVIDIA) {
                    return true;
                }
                if (item.SupportedGpu == SupportedGpu.AMD && NTMinerRoot.Current.GpuSet.GpuType == GpuType.AMD) {
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

        public static void ExtractPackage(this IKernel kernel) {
            try {
                string kernelDir = GetKernelDirFullName(kernel);
                if (string.IsNullOrEmpty(kernelDir)) {
                    return;
                }
                if (!Directory.Exists(kernelDir)) {
                    Directory.CreateDirectory(kernelDir);
                }
                string packageZipFileFullName = GetPackageFileFullName(kernel);
                if (string.IsNullOrEmpty(packageZipFileFullName)) {
                    return;
                }
                if (File.Exists(packageZipFileFullName)) {
                    ZipUtil.DecompressZipFile(packageZipFileFullName, kernelDir);
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }
    }
}
