using NTMiner.Data;
using NTMiner.MinerServer;
using NTMiner.Hashrate;
using System;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class ReportController : ApiController {
        [HttpPost]
        public void ReportSpeed([FromBody]SpeedData speedData) {
            try {
                if (speedData == null) {
                    return;
                }
                string minerIp = Request.GetWebClientIp();
                ClientData clientData = HostRoot.Current.ClientSet.LoadClient(speedData.ClientId);
                if (clientData == null) {
                    clientData = new ClientData() {
                        Id = speedData.ClientId,
                        IsAutoBoot = speedData.IsAutoBoot,
                        IsAutoStart = speedData.IsAutoStart,
                        IsAutoRestartKernel = speedData.IsAutoRestartKernel,
                        IsNoShareRestartKernel = speedData.IsNoShareRestartKernel,
                        NoShareRestartKernelMinutes = speedData.NoShareRestartKernelMinutes,
                        IsPeriodicRestartKernel = speedData.IsPeriodicRestartKernel,
                        PeriodicRestartKernelHours = speedData.PeriodicRestartKernelHours,
                        IsPeriodicRestartComputer = speedData.IsPeriodicRestartComputer,
                        PeriodicRestartComputerHours = speedData.PeriodicRestartComputerHours,
                        GpuDriver = speedData.GpuDriver,
                        GpuType = speedData.GpuType,
                        OSName = speedData.OSName,
                        OSVirtualMemoryMb = speedData.OSVirtualMemoryMb,
                        GpuInfo = speedData.GpuInfo,
                        WorkId = speedData.WorkId,
                        Version = speedData.Version,
                        IsMining = speedData.IsMining,
                        BootOn = speedData.BootOn,
                        MineStartedOn = speedData.MineStartedOn,
                        MinerIp = minerIp,
                        MinerName = speedData.MinerName,
                        CreatedOn = DateTime.Now,
                        ModifiedOn = DateTime.Now,
                        MainCoinCode = speedData.MainCoinCode,
                        MainCoinTotalShare = speedData.MainCoinTotalShare,
                        MainCoinRejectShare = speedData.MainCoinRejectShare,
                        MainCoinSpeed = speedData.MainCoinSpeed,
                        MainCoinPool = speedData.MainCoinPool,
                        MainCoinWallet = speedData.MainCoinWallet,
                        Kernel = speedData.Kernel,
                        IsDualCoinEnabled = speedData.IsDualCoinEnabled,
                        DualCoinPool = speedData.DualCoinPool,
                        DualCoinWallet = speedData.DualCoinWallet,
                        DualCoinCode = speedData.DualCoinCode,
                        DualCoinTotalShare = speedData.DualCoinTotalShare,
                        DualCoinRejectShare = speedData.DualCoinRejectShare,
                        DualCoinSpeed = speedData.DualCoinSpeed,
                        GpuTable = speedData.GpuTable
                    };
                    HostRoot.Current.ClientSet.Add(clientData);
                }
                else {
                    clientData.MinerIp = minerIp;
                    clientData.Update(speedData);
                }
                bool isMainCoin = !string.IsNullOrEmpty(speedData.MainCoinCode);
                // 认为双挖币不能和主挖币相同
                bool isDualCoin = !string.IsNullOrEmpty(speedData.DualCoinCode) && speedData.DualCoinCode != speedData.MainCoinCode;
                if (isMainCoin) {
                    HostRoot.Current.ClientCoinSnapshotSet.Add(new ClientCoinSnapshotData {
                        CoinCode = speedData.MainCoinCode,
                        ShareDelta = speedData.MainCoinShareDelta,
                        Speed = speedData.MainCoinSpeed,
                        Timestamp = DateTime.Now,
                        ClientId = speedData.ClientId
                    });
                }
                if (isDualCoin) {
                    HostRoot.Current.ClientCoinSnapshotSet.Add(new ClientCoinSnapshotData {
                        CoinCode = speedData.DualCoinCode,
                        ShareDelta = speedData.DualCoinShareDelta,
                        Speed = speedData.DualCoinSpeed,
                        Timestamp = DateTime.Now,
                        ClientId = speedData.ClientId
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
