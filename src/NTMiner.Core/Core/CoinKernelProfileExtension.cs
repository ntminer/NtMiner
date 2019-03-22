using NTMiner.Profile;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core {
    public static class CoinKernelProfileExtension {
        public static List<ICoin> GetDualCoins(this ICoinKernelProfile coinKernelProfile) {
            ICoinKernel coinKernel;
            if (!NTMinerRoot.Current.CoinKernelSet.TryGetCoinKernel(coinKernelProfile.CoinKernelId, out coinKernel)) {
                return new List<ICoin>();
            }
            IGroup group;
            if (!NTMinerRoot.Current.GroupSet.TryGetGroup(coinKernel.DualCoinGroupId, out group)) {
                return new List<ICoin>();
            }
            var coinGroups = NTMinerRoot.Current.CoinGroupSet.Where(a => a.GroupId == group.GetId()).OrderBy(a => a.SortNumber);
            List<ICoin> list = new List<ICoin>();
            foreach (var item in coinGroups) {
                ICoin coin;
                if (NTMinerRoot.Current.CoinSet.TryGetCoin(item.CoinId, out coin)) {
                    list.Add(coin);
                }
            }
            return list;
        }
    }
}
