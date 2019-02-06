using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class ProfileController : ApiController {
        [HttpGet]
        public MineWorkData GetMineWork(Guid workId) {
            try {
                return HostRoot.Current.MineWorkSet.GetMineWork(workId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return null;
            }
        }

        [HttpGet]
        public List<MineWorkData> GetMineWorks() {
            try {
                return HostRoot.Current.MineWorkSet.GetMineWorks();
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return new List<MineWorkData>();
            }
        }

        [HttpGet]
        public MinerProfileData GetMinerProfile(Guid workId) {
            try {
                return HostRoot.Current.MineProfileManager.GetMinerProfile(workId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return null;
            }
        }

        [HttpGet]
        public CoinProfileData GetCoinProfile(Guid workId, Guid coinId) {
            try {
                return HostRoot.Current.MineProfileManager.GetCoinProfile(workId, coinId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return null;
            }
        }

        [HttpGet]
        public PoolProfileData GetPoolProfile(Guid workId, Guid poolId) {
            try {
                return HostRoot.Current.MineProfileManager.GetPoolProfile(workId, poolId);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
                return null;
            }
        }

        [HttpGet]
        public CoinKernelProfileData GetCoinKernelProfile(Guid workId, Guid coinKernelId) {
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
