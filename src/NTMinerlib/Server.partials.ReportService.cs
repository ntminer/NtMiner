using NTMiner.ServiceContracts.ControlCenter;
using NTMiner.ServiceContracts.ControlCenter.DataObjects;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Server {
        public partial class ReportServiceFace {
            public static readonly ReportServiceFace Instance = new ReportServiceFace();

            private ReportServiceFace() { }

            private IReportService CreateService() {
                return ChannelFactory.CreateChannel<IReportService>(Server.MinerServerHost, Server.MinerServerPort);
            }

            public void LoginAsync(LoginData message) {
                Task.Factory.StartNew(() => {
                    try {
                        using (var service = CreateService()) {
                            service.Login(message);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.WriteDevLine(e.Message, ConsoleColor.Red);
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                    }
                });
            }

            public void ReportSpeedAsync(SpeedData message) {
                Task.Factory.StartNew(() => {
                    try {
                        using (var service = CreateService()) {
                            service.ReportSpeed(message);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.WriteDevLine(e.Message, ConsoleColor.Red);
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                    }
                });
            }

            public void ReportStateAsync(Guid clientId, bool isMining) {
                Task.Factory.StartNew(() => {
                    try {
                        using (var service = CreateService()) {
                            service.ReportState(clientId, isMining);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.WriteDevLine(e.Message, ConsoleColor.Red);
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                    }
                });
            }
        }
    }
}