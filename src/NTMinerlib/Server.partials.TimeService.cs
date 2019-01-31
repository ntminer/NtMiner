using NTMiner.ServiceContracts;
using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Server {
        public class TimeServiceFace {
            public static readonly TimeServiceFace Instance = new TimeServiceFace();

            private TimeServiceFace() {
            }

            private ITimeService CreateService() {
                return ChannelFactory.CreateChannel<ITimeService>(MinerServerHost, MinerServerPort);
            }

            public void GetTimeAsync(Action<DateTime> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (var service = CreateService()) {
                            callback?.Invoke(service.GetTime());
                        }
                    }
                    catch (CommunicationException e) {
                        Global.WriteDevLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(DateTime.Now);
                    }
                    catch (Exception e) {
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(DateTime.Now);
                    }
                });
            }
        }
    }
}