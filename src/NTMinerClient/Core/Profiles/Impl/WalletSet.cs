using System;
using System.Collections.Generic;

namespace NTMiner.Core.Profiles.Impl {
    public class WalletSet {
        private readonly INTMinerRoot _root;
        private readonly Dictionary<Guid, WalletData> _dicById = new Dictionary<Guid, WalletData>();

        public WalletSet(INTMinerRoot root) {
            _root = root;
            VirtualRoot.AddCmdPath<AddWalletCommand>(action: message => {
                InitOnece();
                if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                    throw new ArgumentNullException();
                }
                if (!_root.ServerContext.CoinSet.Contains(message.Input.CoinId)) {
                    throw new ValidationException("there is not coin with id " + message.Input.CoinId);
                }
                if (string.IsNullOrEmpty(message.Input.Address)) {
                    throw new ValidationException("wallet code and Address can't be null or empty");
                }
                if (_dicById.ContainsKey(message.Input.GetId())) {
                    return;
                }
                WalletData entity = new WalletData().Update(message.Input);
                _dicById.Add(entity.Id, entity);
                AddWallet(entity);

                VirtualRoot.RaiseEvent(new WalletAddedEvent(message.MessageId, entity));
            }, location: this.GetType());
            VirtualRoot.AddCmdPath<UpdateWalletCommand>(action: message => {
                InitOnece();
                if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                    throw new ArgumentNullException();
                }
                if (!_root.ServerContext.CoinSet.Contains(message.Input.CoinId)) {
                    throw new ValidationException("there is not coin with id " + message.Input.CoinId);
                }
                if (string.IsNullOrEmpty(message.Input.Address)) {
                    throw new ValidationException("wallet Address can't be null or empty");
                }
                if (string.IsNullOrEmpty(message.Input.Name)) {
                    throw new ValidationException("wallet name can't be null or empty");
                }
                if (!_dicById.ContainsKey(message.Input.GetId())) {
                    return;
                }
                WalletData entity = _dicById[message.Input.GetId()];
                entity.Update(message.Input);
                UpdateWallet(entity);

                VirtualRoot.RaiseEvent(new WalletUpdatedEvent(message.MessageId, entity));
            }, location: this.GetType());
            VirtualRoot.AddCmdPath<RemoveWalletCommand>(action: (message) => {
                InitOnece();
                if (message == null || message.EntityId == Guid.Empty) {
                    throw new ArgumentNullException();
                }
                if (!_dicById.ContainsKey(message.EntityId)) {
                    return;
                }
                WalletData entity = _dicById[message.EntityId];
                _dicById.Remove(entity.GetId());
                RemoveWallet(entity.Id);

                VirtualRoot.RaiseEvent(new WalletRemovedEvent(message.MessageId, entity));
            }, location: this.GetType());
        }

        private void AddWallet(WalletData entity) {
            if (VirtualRoot.IsMinerStudio) {
                RpcRoot.Server.WalletService.AddOrUpdateWalletAsync(entity, (response, e) => {
                    if (!response.IsSuccess()) {
                        VirtualRoot.Out.ShowError(response.ReadMessage(e), autoHideSeconds: 4);
                    }
                });
            }
            else {
                var repository = NTMinerRoot.CreateLocalRepository<WalletData>();
                repository.Add(entity);
            }
        }

        private void UpdateWallet(WalletData entity) {
            if (VirtualRoot.IsMinerStudio) {
                RpcRoot.Server.WalletService.AddOrUpdateWalletAsync(entity, (response, e) => {
                    if (!response.IsSuccess()) {
                        VirtualRoot.Out.ShowError(response.ReadMessage(e), autoHideSeconds: 4);
                    }
                });
            }
            else {
                var repository = NTMinerRoot.CreateLocalRepository<WalletData>();
                repository.Update(entity);
            }
        }

        private void RemoveWallet(Guid id) {
            if (VirtualRoot.IsMinerStudio) {
                RpcRoot.Server.WalletService.RemoveWalletAsync(id, (response, e) => {
                    if (!response.IsSuccess()) {
                        VirtualRoot.Out.ShowError(response.ReadMessage(e), autoHideSeconds: 4);
                    }
                });
            }
            else {
                var repository = NTMinerRoot.CreateLocalRepository<WalletData>();
                repository.Remove(id);
            }
        }

        public void Refresh() {
            _dicById.Clear();
            _isInited = false;
        }

        private bool _isInited = false;
        private readonly object _locker = new object();

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
            if (!_isInited) {
                if (VirtualRoot.IsMinerStudio) {
                    lock (_locker) {
                        if (!_isInited) {
                            var response = RpcRoot.Server.WalletService.GetWallets();
                            if (response != null) {
                                foreach (var item in response.Data) {
                                    if (!_dicById.ContainsKey(item.Id)) {
                                        _dicById.Add(item.Id, item);
                                    }
                                }
                            }
                            _isInited = true;
                        }
                    }
                }
                else {
                    var repository = NTMinerRoot.CreateLocalRepository<WalletData>();
                    lock (_locker) {
                        if (!_isInited) {
                            foreach (var item in repository.GetAll()) {
                                if (!_dicById.ContainsKey(item.Id)) {
                                    _dicById.Add(item.Id, item);
                                }
                            }
                            _isInited = true;
                        }
                    }
                }
            }
        }

        public bool TryGetWallet(Guid walletId, out IWallet wallet) {
            InitOnece();
            bool r = _dicById.TryGetValue(walletId, out WalletData wlt);
            wallet = wlt;
            return r;
        }

        public IEnumerable<IWallet> GetWallets() {
            InitOnece();
            return _dicById.Values;
        }
    }
}
