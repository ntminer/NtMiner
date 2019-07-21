using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Profiles.Impl {
    public class WalletSet {
        private readonly INTMinerRoot _root;
        private readonly Dictionary<Guid, WalletData> _dicById = new Dictionary<Guid, WalletData>();

        private MineWorkData _mineWorkData;
        public WalletSet(INTMinerRoot root, MineWorkData mineWorkData) {
            _root = root;
            _mineWorkData = mineWorkData;
            VirtualRoot.Window<AddWalletCommand>("添加钱包", LogEnum.DevConsole,
                action: message => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_root.CoinSet.Contains(message.Input.CoinId)) {
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

                    VirtualRoot.Happened(new WalletAddedEvent(entity));
                });
            VirtualRoot.Window<UpdateWalletCommand>("更新钱包", LogEnum.DevConsole,
                action: message => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_root.CoinSet.Contains(message.Input.CoinId)) {
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

                    VirtualRoot.Happened(new WalletUpdatedEvent(entity));
                });
            VirtualRoot.Window<RemoveWalletCommand>("移除钱包", LogEnum.DevConsole,
                action: (message) => {
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

                    VirtualRoot.Happened(new WalletRemovedEvent(entity));
                });
        }

        private void AddWallet(WalletData entity) {
            if (VirtualRoot.IsMinerStudio) {
                Server.ControlCenterService.AddOrUpdateWalletAsync(entity, null);
            }
            else {
                var repository = NTMinerRoot.CreateLocalRepository<WalletData>(isUseJson: false);
                repository.Add(entity);
            }
        }

        private void UpdateWallet(WalletData entity) {
            if (VirtualRoot.IsMinerStudio) {
                Server.ControlCenterService.AddOrUpdateWalletAsync(entity, null);
            }
            else {
                var repository = NTMinerRoot.CreateLocalRepository<WalletData>(isUseJson: false);
                repository.Update(entity);
            }
        }

        private void RemoveWallet(Guid id) {
            if (VirtualRoot.IsMinerStudio) {
                Server.ControlCenterService.RemoveWalletAsync(id, null);
            }
            else {
                var repository = NTMinerRoot.CreateLocalRepository<WalletData>(isUseJson: false);
                repository.Remove(id);
            }
        }

        public void Refresh(MineWorkData mineWorkData) {
            _mineWorkData = mineWorkData;
            _dicById.Clear();
            _isInited = false;
        }

        private bool _isInited = false;
        private object _locker = new object();

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
                            var response = Server.ControlCenterService.GetWallets();
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
                    bool isUseJson = _mineWorkData != null;
                    var repository = NTMinerRoot.CreateLocalRepository<WalletData>(isUseJson: isUseJson);
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
            WalletData wlt;
            bool r = _dicById.TryGetValue(walletId, out wlt);
            wallet = wlt;
            return r;
        }

        public IEnumerable<IWallet> GetWallets() {
            InitOnece();
            return _dicById.Values;
        }
    }
}
