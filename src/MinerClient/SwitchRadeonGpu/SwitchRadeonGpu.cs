using System;
using System.Reflection;
using System.Threading.Tasks;

/// <summary>
/// 注意不要挪动这里的命名空间也不要挪动该代码文件所处的程序集
/// 嵌入的资源的位置和命名空间有关契约关系
/// </summary>
namespace NTMiner.SwitchRadeonGpu {
    public static class SwitchRadeonGpu {
        public static Task Run(bool on) {
            return Task.Factory.StartNew(() => {
                ExtractManifestResource();
                Windows.Cmd.RunClose(App.SwitchRadeonGpuFileFullName, $"--compute={(on ? "on" : "off")} --admin --restart", waitForExit: true);
            });
        }

        private static void ExtractManifestResource() {
            Type type = typeof(SwitchRadeonGpu);
            Assembly assembly = type.Assembly;
            assembly.ExtractManifestResource(type, App.SwitchRadeonGpuResourceName, App.SwitchRadeonGpuFileFullName);
        }
    }
}