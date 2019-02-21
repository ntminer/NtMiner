using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.Vms {
    public class ShareViewModels {
        public static readonly ShareViewModels Current = new ShareViewModels();

        private readonly Dictionary<Guid, ShareViewModel> _dicByCoinId = new Dictionary<Guid, ShareViewModel>();

        private ShareViewModels() {
            VirtualRoot.On<ShareChangedEvent>(
                "收益变更后调整VM内存",
                LogEnum.Console,
                action: message => {
                    ShareViewModel shareVm;
                    if (_dicByCoinId.TryGetValue(message.Source.CoinId, out shareVm)) {
                        shareVm.AcceptShareCount = message.Source.AcceptShareCount;
                        shareVm.RejectShareCount = message.Source.RejectShareCount;
                        shareVm.ShareOn = message.Source.ShareOn;
                    }
                });
        }

        public ShareViewModel GetOrCreate(Guid coinId) {
            if (!NTMinerRoot.Current.CoinSet.Contains(coinId)) {
                return new ShareViewModel(coinId);
            }
            if (_dicByCoinId.ContainsKey(coinId)) {
                return _dicByCoinId[coinId];
            }
            ShareViewModel item = new ShareViewModel(coinId);
            _dicByCoinId.Add(coinId, item);
            return item;
        }
    }
}
