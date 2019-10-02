using System;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class CoinShareSet : ICoinShareSet {
        private readonly Dictionary<Guid, CoinShare> _dicByCoinId = new Dictionary<Guid, CoinShare>();
        private readonly object _locker = new object();
        private readonly INTMinerRoot _root;

        public CoinShareSet(INTMinerRoot root) {
            _root = root;
        }

        public ICoinShare GetOrCreate(Guid coinId) {
            if (!_root.CoinSet.Contains(coinId)) {
                return new CoinShare() {
                    CoinId = coinId,
                    RejectShareCount = 0,
                    RejectPercent = 0,
                    ShareOn = DateTime.Now,
                    AcceptShareCount = 0
                };
            }
            CoinShare share;
            if (!_dicByCoinId.TryGetValue(coinId, out share)) {
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
                VirtualRoot.Happened(new ShareChangedEvent(coinShare));
            }
        }
    }
}
