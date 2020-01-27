using NTMiner.Core.MinerServer;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public partial class AppContext {
        public class CoinSnapshotDataViewModels : ViewModelBase {
            public static readonly CoinSnapshotDataViewModels Instance = new CoinSnapshotDataViewModels();

            private readonly Dictionary<string, CoinSnapshotDataViewModel> _dicByCoinCode = new Dictionary<string, CoinSnapshotDataViewModel>(StringComparer.OrdinalIgnoreCase);

            private CoinSnapshotDataViewModels() {
#if DEBUG
                NTStopwatch.Start();
#endif
                foreach (var coinVm in AppContext.Instance.CoinVms.AllCoins) {
                    _dicByCoinCode.Add(coinVm.Code, new CoinSnapshotDataViewModel(new CoinSnapshotData {
                        CoinCode = coinVm.Code,
                        MainCoinMiningCount = 0,
                        MainCoinOnlineCount = 0,
                        DualCoinMiningCount = 0,
                        DualCoinOnlineCount = 0,
                        ShareDelta = 0,
                        RejectShareDelta = 0,
                        Speed = 0,
                        Timestamp = DateTime.MinValue
                    }));
                }
#if DEBUG
                var elapsedMilliseconds = NTStopwatch.Stop();
                if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                    Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
                }
#endif
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
