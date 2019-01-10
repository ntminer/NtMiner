using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace NTMiner {
    class Program {
        private static Mutex mutexApp;

        static void Main(string[] args) {
            try {
                bool mutexCreated;
                try {
                    mutexApp = new Mutex(true, "NTMinerDaemonAppMutex", out mutexCreated);
                }
                catch {
                    mutexCreated = false;
                }
                if (mutexCreated) {
                    Windows.App.SetAutoBoot("NTMinerDaemon", true);
                    object isAutoBootValue = Windows.Registry.GetValue(Registry.Users, ClientId.NTMinerRegistrySubKey, "IsAutoBoot");
                    if (isAutoBootValue != null && isAutoBootValue.ToString() == "True") {
                        object locationValue = Windows.Registry.GetValue(Registry.Users, ClientId.NTMinerRegistrySubKey, "Location");
                        if (locationValue != null) {
                            string arguments = string.Empty;
                            object argumentsValue = Windows.Registry.GetValue(Registry.Users, ClientId.NTMinerRegistrySubKey, "Arguments");
                            if (argumentsValue != null) {
                                arguments = (string)argumentsValue;
                            }
                            try {
                                Process.Start((string)locationValue, arguments);
                            }
                            catch (Exception e) {
                                Global.Logger.ErrorDebugLine(e.Message, e);
                            }
                        }
                    }
                    Run();
                }
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
            }
        }
        private static void Run() {
            try {
                Task.Factory.StartNew(DaemonServer.Start);
                while (true) {
                    Thread.Sleep(1000);
                }
            }
            catch (Exception e) {
                DaemonServer.Stop();
                Global.Logger.ErrorDebugLine(e.Message, e);
            }
        }
    }
}
