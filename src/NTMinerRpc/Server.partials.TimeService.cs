using NTMiner.ServiceContracts;
using System;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Server {
        public class TimeServiceFace {
            public static readonly TimeServiceFace Instance = new TimeServiceFace();

            private TimeServiceFace() {
            }

            private ITimeService CreateService() {
                throw new NotImplementedException();
            }

            public void GetTimeAsync(Action<DateTime> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (var service = CreateService()) {
                            callback?.Invoke(service.GetTime());
                        }
                    }
                    catch (Exception e) {
                        callback?.Invoke(DateTime.Now);
                    }
                });
            }
        }
    }
}