using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace NTMiner {
    public static partial class Client {
        public partial class MinerStudioServiceFace {
            public static readonly MinerStudioServiceFace Instance = new MinerStudioServiceFace();

            private MinerStudioServiceFace() {
            }

            public void CloseMinerStudio() {
                bool isClosed = false;
                try {
                    using (HttpClient client = new HttpClient()) {
                        Task<HttpResponseMessage> message = client.PostAsJsonAsync($"http://localhost:{WebApiConst.MinerStudioPort}/api/MinerStudio/CloseMinerStudio", new SignatureRequest {});
                        ResponseBase response = message.Result.Content.ReadAsAsync<ResponseBase>().Result;
                        isClosed = response.IsSuccess();
                    }
                }
                catch (Exception e) {
                    Logger.ErrorDebugLine(e.Message, e);
                }
                if (!isClosed) {
                    try {
                        string location = NTMinerRegistry.GetLocation();
                        if (!string.IsNullOrEmpty(location) && File.Exists(location)) {
                            string processName = Path.GetFileNameWithoutExtension(location);
                            Windows.TaskKill.Kill(processName);
                        }
                    }
                    catch (Exception e) {
                        Logger.ErrorDebugLine(e.Message, e);
                    }
                }
            }
        }
    }
}
