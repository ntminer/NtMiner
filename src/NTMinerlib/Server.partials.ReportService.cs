using NTMiner.ServiceContracts;
using NTMiner.ServiceContracts.DataObjects;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Server {
        public partial class ReportServiceFace {
            public static readonly ReportServiceFace Instance = new ReportServiceFace();

            private ReportServiceFace() { }

            private IReportService CreateService() {
                return ChannelFactory.CreateChannel<IReportService>(
                        ChannelFactory.BasicHttpBinding,
                        Server.MinerServerHost,
                        Server.MinerServerPort);
            }

            public void Login(LoginData message) {
                Task.Factory.StartNew(() => {
                    try {
                        using (var service = CreateService()) {
                            service.Login(message);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.DebugLine(e.Message, ConsoleColor.Red);
                    }
                    catch (Exception e) {
                        Global.Logger.Error(e.Message, e);
                    }
                });
            }

            public void ReportSpeed(SpeedData message) {
                Task.Factory.StartNew(() => {
                    try {
                        using (var service = CreateService()) {
                            service.ReportSpeed(message);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.DebugLine(e.Message, ConsoleColor.Red);
                    }
                    catch (Exception e) {
                        Global.Logger.Error(e.Message, e);
                    }
                });
            }

            public void ReportState(Guid clientId, bool isMining) {
                Task.Factory.StartNew(() => {
                    try {
                        using (var service = CreateService()) {
                            service.ReportState(clientId, isMining);
                        }
                    }
                    catch (CommunicationException e) {
                        Global.DebugLine(e.Message, ConsoleColor.Red);
                    }
                    catch (Exception e) {
                        Global.Logger.Error(e.Message, e);
                    }
                });
            }
        }
    }
}