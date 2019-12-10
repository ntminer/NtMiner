using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public partial class AppContext {
        public class ShareViewModels {
            public static readonly ShareViewModels Instance = new ShareViewModels();
            private readonly Dictionary<Guid, ShareViewModel> _dicByCoinId = new Dictionary<Guid, ShareViewModel>();

            private ShareViewModels() {
#if DEBUG
                NTStopwatch.Start();
#endif
                AddEventPath<ShareChangedEvent>("收益变更后调整VM内存", LogEnum.DevConsole,
                    action: message => {
                        if (_dicByCoinId.TryGetValue(message.Target.CoinId, out ShareViewModel shareVm)) {
                            shareVm.Update(message.Target);
                        }
                    }, location: this.GetType());
#if DEBUG
                var elapsedMilliseconds = NTStopwatch.Stop();
                if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                    Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.ctor");
                }
#endif
            }

            private readonly object _locker = new object();
            public ShareViewModel GetOrCreate(Guid coinId) {
                if (!NTMinerRoot.Instance.ServerContext.CoinSet.Contains(coinId)) {
                    return new ShareViewModel(coinId);
                }
                if (!_dicByCoinId.TryGetValue(coinId, out ShareViewModel shareVm)) {
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
}
