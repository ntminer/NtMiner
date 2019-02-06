using System;
using System.Net.Http;

namespace NTMiner {
    public static partial class Server {
        public partial class ReportServiceFace {
            public static readonly ReportServiceFace Instance = new ReportServiceFace();
            private readonly string baseUrl = $"http://{MinerServerHost}:{MinerServerPort}/api/Report";

            private ReportServiceFace() { }

            public void LoginAsync(LoginData data) {
                try {
                    using (HttpClient client = new HttpClient()) {
                        client.PostAsJsonAsync($"{baseUrl}/Login", data);
                    }
                }
                catch {
                }
            }

            public void ReportSpeedAsync(SpeedData data) {
                try {
                    using (HttpClient client = new HttpClient()) {
                        client.PostAsJsonAsync($"{baseUrl}/ReportSpeed", data);
                    }
                }
                catch {
                }
            }

            public void ReportStateAsync(Guid clientId, bool isMining) {
                try {
                    using (HttpClient client = new HttpClient()) {
                        client.PostAsJsonAsync($"{baseUrl}/ReportState", new { clientId, isMining});
                    }
                }
                catch {
                }
            }
        }
    }
}