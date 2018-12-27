using NTMiner.ServiceContracts;
using NTMiner.ServiceContracts.DataObjects;
using System;
using System.Collections.Generic;

namespace NTMiner.Services {
    public class ProfileServiceImpl : IProfileService {
        public MineWorkData GetMineWork(Guid workId) {
            try {
                return HostRoot.Current.MineWorkSet.GetMineWork(workId);
            }
            catch (Exception e) {
                Global.Logger.Error(e.Message, e);
                return null;
            }
        }

        public List<MineWorkData> GetMineWorks() {
            try {
                return HostRoot.Current.MineWorkSet.GetMineWorks();
            }
            catch (Exception e) {
                Global.Logger.Error(e.Message, e);
                return new List<MineWorkData>();
            }
        }

        public MinerProfileData GetMinerProfile(Guid workId) {
            try {
                return HostRoot.Current.MineProfileManager.GetMinerProfile(workId);
            }
            catch (Exception e) {
                Global.Logger.Error(e.Message, e);
                return null;
            }
        }

        public CoinProfileData GetCoinProfile(Guid workId, Guid coinId) {
            try {
                return HostRoot.Current.MineProfileManager.GetCoinProfile(workId, coinId);
            }
            catch (Exception e) {
                Global.Logger.Error(e.Message, e);
                return null;
            }
        }

        public CoinKernelProfileData GetCoinKernelProfile(Guid workId, Guid coinKernelId) {
            try {
                return HostRoot.Current.MineProfileManager.GetCoinKernelProfile(workId, coinKernelId);
            }
            catch (Exception e) {
                Global.Logger.Error(e.Message, e);
                return null;
            }
        }

        public void Dispose() {
            // nothing need todo
        }
    }
}
