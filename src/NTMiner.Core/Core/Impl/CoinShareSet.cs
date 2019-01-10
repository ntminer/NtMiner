using System;
using System.Collections.Generic;

namespace NTMiner.Core.Gpus {
    public class CoinShareSet : ICoinShareSet {
        private readonly Dictionary<Guid, CoinShare> _dicByCoinId = new Dictionary<Guid, CoinShare>();
        private readonly INTMinerRoot _root;

        public CoinShareSet(INTMinerRoot root) {
            _root = root;
            Global.Logger.Debug(this.GetType().FullName + "接入总线");
        }

        public ICoinShare GetOrCreate(Guid coinId) {
            if (!_root.CoinSet.Contains(coinId)) {
                return new CoinShare() {
                    CoinId = coinId,
                    RejectCount = 0,
                    RejectPercent = 0,
                    ShareOn = DateTime.Now,
                    AcceptShareCount = 0
                };
            }
            if (_dicByCoinId.ContainsKey(coinId)) {
                return _dicByCoinId[coinId];
            }
            CoinShare share = new CoinShare {
                CoinId = coinId,
                RejectCount = 0,
                RejectPercent = 0,
                ShareOn = DateTime.Now,
                AcceptShareCount = 0
            };
            _dicByCoinId.Add(coinId, share);
            return share;
        }
    }
}
