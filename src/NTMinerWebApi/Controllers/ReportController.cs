using NTMiner.Data;
using System;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class ReportController : ApiController, IReportService {
        [HttpPost]
        public void Login([FromBody]LoginData message) {
            try {
                if (message == null) {
                    return;
                }
                string minerIp = System.Web.HttpContext.Current.GetWebClientIp();
                ClientData clientData = HostRoot.Current.ClientSet.LoadClient(message.ClientId);
                if (clientData == null) {
                    clientData = new ClientData() {
                        Id = message.ClientId,
                        WorkId = message.WorkId,
                        Version = message.Version,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        MinerIp = minerIp,
                        MinerName = message.MinerName,
                        GpuInfo = message.GpuInfo
                    };
                    HostRoot.Current.ClientSet.Add(clientData);
                }
                else {
                    clientData.WorkId = message.WorkId;
                    clientData.Version = message.Version;
                    clientData.ModifiedOn = DateTime.Now;
                    clientData.MinerIp = minerIp;
                    clientData.MinerName = message.MinerName;
                    clientData.GpuInfo = message.GpuInfo;
                }
                Global.Logger.InfoDebugLine($"{message.ClientId} {minerIp} 登录");
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
            }
        }

        [HttpPost]
        public void ReportSpeed([FromBody]SpeedData message) {
            try {
                if (message == null) {
                    return;
                }
                string minerIp = System.Web.HttpContext.Current.GetWebClientIp();
                ClientData clientData = HostRoot.Current.ClientSet.LoadClient(message.ClientId);
                if (clientData == null) {
                    clientData = new ClientData() {
                        Id = message.ClientId,
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
                        IsMining = message.IsMining,
                        MinerIp = minerIp
                    };
                    HostRoot.Current.ClientSet.Add(clientData);
                }
                else {
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
                    clientData.MinerIp = minerIp;
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
                Global.Logger.ErrorDebugLine(e.Message, e);
            }
        }

        [HttpPost]
        public void ReportState(Guid clientId, bool isMining) {
            try {
                string minerIp = System.Web.HttpContext.Current.GetWebClientIp();
                ClientData clientData = HostRoot.Current.ClientSet.LoadClient(clientId);
                if (clientData == null) {
                    clientData = new ClientData {
                        Id = clientId,
                        IsMining = isMining,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        MinerIp = minerIp
                    };
                    HostRoot.Current.ClientSet.Add(clientData);
                }
                else {
                    clientData.IsMining = isMining;
                    clientData.ModifiedOn = DateTime.Now;
                    clientData.MinerIp = minerIp;
                }
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
            }
        }
    }
}
