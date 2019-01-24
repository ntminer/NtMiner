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
                    NTMinerRegistry.SetAutoBoot("NTMinerDaemon", true);
                    bool isAutoBoot = NTMinerRegistry.GetIsAutoBoot();
                    if (isAutoBoot) {
                        string location = NTMinerRegistry.GetLocation();
                        if (!string.IsNullOrEmpty(location)) {
                            string arguments = NTMinerRegistry.GetArguments();
                            try {
                                Process.Start(location, arguments);
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
                DaemonServer.StartAsync();
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
