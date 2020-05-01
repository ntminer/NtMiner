using NTMiner.Core.MinerServer;
using NTMiner.MinerStudio.Vms;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.MinerStudio {
    public static partial class MinerStudioRoot {
        public class CoinSnapshotDataViewModels : ViewModelBase {
            public static readonly CoinSnapshotDataViewModels Instance = new CoinSnapshotDataViewModels();

            private readonly Dictionary<string, CoinSnapshotDataViewModel> _dicByCoinCode = new Dictionary<string, CoinSnapshotDataViewModel>(StringComparer.OrdinalIgnoreCase);

            private CoinSnapshotDataViewModels() {
                if (WpfUtil.IsInDesignMode) {
                    return;
                }
                foreach (var coinVm in AppRoot.CoinVms.AllCoins) {
                    _dicByCoinCode.Add(coinVm.Code, new CoinSnapshotDataViewModel(CoinSnapshotData.CreateEmpty(coinVm.Code)));
                }
            }

            public bool TryGetSnapshotDataVm(string coinCode, out CoinSnapshotDataViewModel vm) {
                return _dicByCoinCode.TryGetValue(coinCode, out vm);
            }

            public List<CoinSnapshotDataViewModel> CoinSnapshotDataVms {
                get {
                    return _dicByCoinCode.Values.ToList();
                }
            }
        }
    }
}
