using NTMiner.Core;
using System;
using System.Collections.Generic;

namespace NTMiner.Vms {
    public class ShareViewModels {
        public static readonly ShareViewModels Current = new ShareViewModels();

        private readonly Dictionary<Guid, ShareViewModel> _dicByCoinId = new Dictionary<Guid, ShareViewModel>();

        private ShareViewModels() {
            VirtualRoot.On<ShareChangedEvent>("收益变更后调整VM内存", LogEnum.DevConsole,
                action: message => {
                    ShareViewModel shareVm;
                    if (_dicByCoinId.TryGetValue(message.Source.CoinId, out shareVm)) {
                        shareVm.Update(message.Source);
                    }
                });
        }

        private readonly object _locker = new object();
        public ShareViewModel GetOrCreate(Guid coinId) {
            if (!NTMinerRoot.Current.CoinSet.Contains(coinId)) {
                return new ShareViewModel(coinId);
            }
            ShareViewModel shareVm;
            if (!_dicByCoinId.TryGetValue(coinId, out shareVm)) {
                lock (_locker) {
                    if (!_dicByCoinId.TryGetValue(coinId, out shareVm)) {
                        shareVm = new ShareViewModel(coinId);
                        _dicByCoinId.Add(coinId, shareVm);
                    }
                }
            }
            return shareVm;
        }
    }
}
