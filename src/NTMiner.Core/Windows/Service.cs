using System;
using System.ServiceProcess;

namespace NTMiner.Windows {
    public static class Service {
        public static void StopService(string serviceName, Action<bool> callback = null) {
            if (string.IsNullOrEmpty(serviceName)) {
                throw new InvalidProgramException();
            }
            using (ServiceController serv = new ServiceController(serviceName)) {
                try {
                    if (serv.CanStop) {
                        serv.Stop();
                        if (callback != null) {
                            serv.WaitForStatus(ServiceControllerStatus.Stopped);
                            callback.Invoke(true);
                        }
                    }
                }
                catch (Exception e) {
                    Global.Logger.ErrorDebugLine(e.Message, e);
                    if (callback != null) {
                        callback.Invoke(false);
                    }
                }
            }
        }
    }
}
