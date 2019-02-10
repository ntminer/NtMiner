using NTMiner.Data;
using NTMiner.MinerServer;
using System;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class ReportController : ApiController {
        [HttpPost]
        public void ReportSpeed([FromBody]SpeedData message) {
            try {
                if (message == null) {
                    return;
                }
                string minerIp = Request.GetWebClientIp();
                ClientData clientData = HostRoot.Current.ClientSet.LoadClient(message.ClientId);
                if (clientData == null) {
                    clientData = new ClientData() {
                        Id = message.ClientId,
                        WorkId = message.WorkId,
                        Version = message.Version,
                        MinerIp = minerIp,
                        GpuInfo = message.GpuInfo,
                        MinerName = message.MinerName,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        MainCoinCode = message.MainCoinCode,
                        MainCoinPool = message.MainCoinPool,
                        MainCoinWallet = message.MainCoinWallet,
                        Kernel = message.Kernel,
                        IsDualCoinEnabled = message.IsDualCoinEnabled,
                        DualCoinPool = message.DualCoinPool,
                        DualCoinWallet = message.DualCoinWallet,
                        DualCoinCode = message.DualCoinCode,
                        IsMining = message.IsMining
                    };
                    HostRoot.Current.ClientSet.Add(clientData);
                }
                else {
                    clientData.WorkId = message.WorkId;
                    clientData.Version = message.Version;
                    clientData.MinerIp = minerIp;
                    clientData.GpuInfo = message.GpuInfo;
                    clientData.MinerName = message.MinerName;
                    clientData.ModifiedOn = DateTime.Now;
                    clientData.MainCoinCode = message.MainCoinCode;
                    clientData.MainCoinPool = message.MainCoinPool;
                    clientData.MainCoinWallet = message.MainCoinWallet;
                    clientData.Kernel = message.Kernel;
                    clientData.IsDualCoinEnabled = message.IsDualCoinEnabled;
                    clientData.DualCoinPool = message.DualCoinPool;
                    clientData.DualCoinWallet = message.DualCoinWallet;
                    clientData.DualCoinCode = message.DualCoinCode;
                    clientData.IsMining = message.IsMining;
                }
                bool isMainCoin = !string.IsNullOrEmpty(message.MainCoinCode);
                // 认为双挖币不能和主挖币相同
                bool isDualCoin = !string.IsNullOrEmpty(message.DualCoinCode) && message.DualCoinCode != message.MainCoinCode;
                if (isMainCoin) {
                    HostRoot.Current.ClientCoinSnapshotSet.Add(new ClientCoinSnapshotData {
                        CoinCode = message.MainCoinCode,
                        ShareDelta = message.MainCoinShareDelta,
                        Speed = message.MainCoinSpeed,
                        Timestamp = DateTime.Now,
                        ClientId = message.ClientId
                    });
                }
                if (isDualCoin) {
                    HostRoot.Current.ClientCoinSnapshotSet.Add(new ClientCoinSnapshotData {
                        CoinCode = message.DualCoinCode,
                        ShareDelta = message.DualCoinShareDelta,
                        Speed = message.DualCoinSpeed,
                        Timestamp = DateTime.Now,
                        ClientId = message.ClientId
                    });
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }

        [HttpPost]
        public void ReportState([FromBody]ReportStateRequest request) {
            try {
                string minerIp = Request.GetWebClientIp();
                ClientData clientData = HostRoot.Current.ClientSet.LoadClient(request.ClientId);
                if (clientData == null) {
                    clientData = new ClientData {
                        Id = request.ClientId,
                        IsMining = request.IsMining,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        MinerIp = minerIp
                    };
                    HostRoot.Current.ClientSet.Add(clientData);
                }
                else {
                    clientData.IsMining = request.IsMining;
                    clientData.ModifiedOn = DateTime.Now;
                    clientData.MinerIp = minerIp;
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
            }
        }
    }
}
