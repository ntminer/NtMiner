using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Server {
        public class TimeServiceFace {
            public static readonly TimeServiceFace Instance = new TimeServiceFace();
            private readonly string baseUrl = $"http://{MinerServerHost}:{MinerServerPort}/api/Time";

            private TimeServiceFace() {
            }

            public void GetTimeAsync(Action<DateTime> callback) {
                Task.Factory.StartNew(() => {
                    try {
                        using (HttpClient client = new HttpClient()) {
                            Task<HttpResponseMessage> message = client.GetAsync($"{baseUrl}/GetTime");
                            DateTime response = message.Result.Content.ReadAsAsync<DateTime>().Result;
                            callback?.Invoke(response);
                        }
                    }
                    catch {
                        callback?.Invoke(DateTime.Now);
                    }
                });
            }
        }
    }
}