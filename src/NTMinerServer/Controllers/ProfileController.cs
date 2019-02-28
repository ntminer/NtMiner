using NTMiner.MinerServer;
using NTMiner.Profile;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace NTMiner.Controllers {
    public class ProfileController : ApiController {
        [HttpPost]
        public MineWorkData MineWork([FromBody]MineWorkRequest request) {
            try {
                return HostRoot.Current.MineWorkSet.GetMineWork(request.WorkId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return null;
            }
        }

        [HttpPost]
        public List<MineWorkData> MineWorks() {
            try {
                return HostRoot.Current.MineWorkSet.GetAll();
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return new List<MineWorkData>();
            }
        }

        [HttpPost]
        public MinerProfileData MinerProfile([FromBody]MinerProfileRequest request) {
            try {
                return HostRoot.Current.MineProfileManager.GetMinerProfile(request.WorkId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return null;
            }
        }

        [HttpPost]
        public CoinProfileData CoinProfile([FromBody]CoinProfileRequest request) {
            try {
                return HostRoot.Current.MineProfileManager.GetCoinProfile(request.WorkId, request.CoinId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return null;
            }
        }

        [HttpPost]
        public PoolProfileData PoolProfile([FromBody]PoolProfileRequest request) {
            try {
                return HostRoot.Current.MineProfileManager.GetPoolProfile(request.WorkId, request.PoolId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return null;
            }
        }

        [HttpPost]
        public CoinKernelProfileData CoinKernelProfile([FromBody]CoinKernelProfileRequest request) {
            try {
                return HostRoot.Current.MineProfileManager.GetCoinKernelProfile(request.WorkId, request.CoinKernelId);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return null;
            }
        }
    }
}
