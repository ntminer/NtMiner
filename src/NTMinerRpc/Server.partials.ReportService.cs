using NTMiner.ServiceContracts;
using System;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Server {
        public partial class ReportServiceFace {
            public static readonly ReportServiceFace Instance = new ReportServiceFace();

            private ReportServiceFace() { }

            private IReportService CreateService() {
                throw new NotImplementedException();
            }

            public void LoginAsync(LoginData message) {
                Task.Factory.StartNew(() => {
                    try {
                        using (var service = CreateService()) {
                            service.Login(message);
                        }
                    }
                    catch (Exception e) {
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
                    catch (Exception e) {
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
                    catch (Exception e) {
                    }
                });
            }
        }
    }
}