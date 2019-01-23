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
                return ChannelFactory.CreateChannel<ITimeService>(Server.MinerServerHost, Server.MinerServerPort);
            }

            public void GetTimeAsync(Action<DateTime> callback) {
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
                        Global.Logger.ErrorDebugLine(e.Message, e);
                        callback?.Invoke(DateTime.Now);
                    }
                });
            }

            /// <summary>
            /// 同步方法
            /// </summary>
            /// <param name="host"></param>
            /// <returns></returns>
            public string GetServerPubKey(string host) {
                using (var service = ChannelFactory.CreateChannel<ITimeService>(host, Server.MinerServerPort)) {
                    return service.GetServerPubKey();
                }
            }
        }
    }
}