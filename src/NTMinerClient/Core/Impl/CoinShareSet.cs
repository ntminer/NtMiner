using System;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class CoinShareSet : ICoinShareSet {
        private readonly Dictionary<Guid, CoinShare> _dicByCoinId = new Dictionary<Guid, CoinShare>();
        private readonly object _locker = new object();
        private readonly INTMinerContext _ntminerContext;

        public CoinShareSet(INTMinerContext ntminerContext) {
            _ntminerContext = ntminerContext;
        }

        public ICoinShare GetOrCreate(Guid coinId) {
            if (!_ntminerContext.ServerContext.CoinSet.Contains(coinId)) {
                return new CoinShare() {
                    CoinId = coinId,
                    RejectShareCount = 0,
                    RejectPercent = 0,
                    ShareOn = DateTime.Now,
                    AcceptShareCount = 0
                };
            }
            if (!_dicByCoinId.TryGetValue(coinId, out CoinShare share)) {
                lock (_locker) {
                    if (!_dicByCoinId.TryGetValue(coinId, out share)) {
                        share = new CoinShare {
                            CoinId = coinId,
                            RejectShareCount = 0,
                            RejectPercent = 0,
                            ShareOn = DateTime.Now,
                            AcceptShareCount = 0
                        };
                        _dicByCoinId.Add(coinId, share);
                    }
                }
            }
            return share;
        }

        public void UpdateShare(Guid coinId, int? acceptShareCount, int? rejectShareCount, DateTime now) {
            CoinShare coinShare = (CoinShare)GetOrCreate(coinId);
            bool isChanged = false;
            if (acceptShareCount.HasValue) {
                if (coinShare.AcceptShareCount != acceptShareCount.Value) {
                    coinShare.AcceptShareCount = acceptShareCount.Value;
                    isChanged = true;
                }
            }
            if (rejectShareCount.HasValue) {
                if (coinShare.RejectShareCount != rejectShareCount.Value) {
                    coinShare.RejectShareCount = rejectShareCount.Value;
                    isChanged = true;
                }
            }
            coinShare.ShareOn = now;
            if (isChanged) {
                VirtualRoot.RaiseEvent(new ShareChangedEvent(Guid.Empty, coinShare));
            }
        }
    }
}
