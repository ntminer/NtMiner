using System;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace NTMiner.Windows {
    public static class WAU {
        public static void DisableWAU() {
            Task.Factory.StartNew(() => {
                ServiceController sc = new ServiceController("wuauserv");
                try {
                    if (sc != null && sc.Status == ServiceControllerStatus.Running) {
                        sc.Stop();
                        sc.WaitForStatus(ServiceControllerStatus.Stopped);
                        Global.DebugLine("Windows自动更新禁用成功", ConsoleColor.Green);
                    }
                    else {
                        Global.DebugLine("Windows自动更新已经处于禁用状态，无需再次禁用", ConsoleColor.Green);
                    }
                    sc.Close();
                }
                catch (Exception ex) {
                    Global.Logger.Error("Windows自动更新禁用失败，因为异常", ex);
                }
            });
        }
    }
}
