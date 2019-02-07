using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class ProfileController : ApiController {
        [HttpPost]
        public MineWorkData MineWork(Guid workId) {
            try {
                return HostRoot.Current.MineWorkSet.GetMineWork(workId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return null;
            }
        }

        [HttpPost]
        public List<MineWorkData> MineWorks() {
            try {
                return HostRoot.Current.MineWorkSet.GetMineWorks();
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return new List<MineWorkData>();
            }
        }

        [HttpPost]
        public MinerProfileData MinerProfile(Guid workId) {
            try {
                return HostRoot.Current.MineProfileManager.GetMinerProfile(workId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return null;
            }
        }

        [HttpPost]
        public CoinProfileData CoinProfile(Guid workId, Guid coinId) {
            try {
                return HostRoot.Current.MineProfileManager.GetCoinProfile(workId, coinId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return null;
            }
        }

        [HttpPost]
        public PoolProfileData PoolProfile(Guid workId, Guid poolId) {
            try {
                return HostRoot.Current.MineProfileManager.GetPoolProfile(workId, poolId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return null;
            }
        }

        [HttpPost]
        public CoinKernelProfileData CoinKernelProfile(Guid workId, Guid coinKernelId) {
            try {
                return HostRoot.Current.MineProfileManager.GetCoinKernelProfile(workId, coinKernelId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return null;
            }
        }
    }
}
