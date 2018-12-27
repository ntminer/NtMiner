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
                return ChannelFactory.CreateChannel<ITimeService>(
                        ChannelFactory.BasicHttpBinding,
                        Server.MinerServerHost,
                        Server.MinerServerPort);
            }

            public void GetTime(Action<DateTime> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (var service = CreateService()) {
                            callback?.Invoke(service.GetTime());
                        }
                    }
                    catch (CommunicationException e) {
                        Global.DebugLine(e.Message, ConsoleColor.Red);
                        callback?.Invoke(DateTime.Now);
                    }
                    catch (Exception e) {
                        Global.Logger.Error(e.Message, e);
                        callback?.Invoke(DateTime.Now);
                    }
                });
            }

            public string GetServerPubKey(string host) {
                using (var service = ChannelFactory.CreateChannel<ITimeService>(
                        ChannelFactory.BasicHttpBinding,
                        host,
                        Server.MinerServerPort)) {
                    return service.GetServerPubKey();
                }
            }
        }
    }
}