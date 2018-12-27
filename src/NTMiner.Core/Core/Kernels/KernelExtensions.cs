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
            if (kernel == null) {
                return string.Empty;
            }
            string args = kernel.Args;
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
                commandName = args.Substring(0, args.IndexOf(' '));
            }
            return commandName;
        }

        public static bool IsSupported(this IKernel kernel) {
            foreach (var item in NTMinerRoot.Current.CoinKernelSet.Where(a => a.KernelId == kernel.GetId())) {
                if (item.SupportedGpu == Gpus.SupportedGpu.Both) {
                    return true;
                }
                if (item.SupportedGpu == Gpus.SupportedGpu.NVIDIA && NTMinerRoot.Current.GpuSet.GpuType == Gpus.GpuType.NVIDIA) {
                    return true;
                }
                if (item.SupportedGpu == Gpus.SupportedGpu.AMD && NTMinerRoot.Current.GpuSet.GpuType == Gpus.GpuType.AMD) {
                    return true;
                }
            }
            return false;
        }

        public static string GetKernelDirFullName(this IKernel kernel) {
            return Path.Combine(SpecialPath.KernelsDirFullName, Path.GetFileNameWithoutExtension(kernel.Package));
        }

        public static string GetPackageFileFullName(this IKernel kernel) {
            return Path.Combine(SpecialPath.PackagesDirFullName, kernel.Package);
        }

        public static string GetDownloadFileFullName(this IKernel kernel) {
            return Path.Combine(SpecialPath.DownloadDirFullName, kernel.Package);
        }

        public static List<string> GetPackageHistoryFileFullNames(this IKernel kernel) {
            return GetPackageHistories(kernel).Select(a => Path.Combine(SpecialPath.PackagesDirFullName, a)).ToList();
        }

        public static List<string> GetPackageHistories(this IKernel kernel) {
            string packageHistory = kernel.PackageHistory;
            if (string.IsNullOrEmpty(packageHistory)) {
                return new List<string>();
            }
            return packageHistory.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public static void ExtractPackage(this IKernel kernel) {
            string kernelDir = GetKernelDirFullName(kernel);
            if (!Directory.Exists(kernelDir)) {
                Directory.CreateDirectory(kernelDir);
            }
            string packageZipFileFullName = GetPackageFileFullName(kernel);
            ZipUtil.DecompressZipFile(packageZipFileFullName, kernelDir);
        }
    }
}
