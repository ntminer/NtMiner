using NTMiner.MinerServer;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    internal class WalletSet : IWalletSet {
        private readonly INTMinerRoot _root;
        private readonly Dictionary<Guid, WalletData> _dicById = new Dictionary<Guid, WalletData>();

        private bool UseRemoteWalletList {
            get {
                return CommandLineArgs.IsWorker || CommandLineArgs.IsControlCenter;
            }
        }

        private void AddWallet(WalletData entity) {
            if (UseRemoteWalletList) {
                Server.ControlCenterService.AddOrUpdateWalletAsync(entity, null);
            }
            else {
                var repository = NTMinerRoot.CreateLocalRepository<WalletData>();
                repository.Add(entity);
            }
        }

        private void UpdateWallet(WalletData entity) {
            if (UseRemoteWalletList) {
                Server.ControlCenterService.AddOrUpdateWalletAsync(entity, null);
            }
            else {
                var repository = NTMinerRoot.CreateLocalRepository<WalletData>();
                repository.Update(entity);
            }
        }

        private void RemoveWallet(Guid id) {
            if (UseRemoteWalletList) {
                Server.ControlCenterService.RemoveWalletAsync(id, null);
            }
            else {
                var repository = NTMinerRoot.CreateLocalRepository<WalletData>();
                repository.Remove(id);
            }
        }

        public WalletSet(INTMinerRoot root) {
            _root = root;
            Global.Access<AddWalletCommand>(
                Guid.Parse("d050de9d-7356-471b-b9c7-19d685aa770a"),
                "添加钱包",
                LogEnum.Console,
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

                    Global.Happened(new WalletAddedEvent(entity));
                });
            Global.Access<UpdateWalletCommand>(
                Guid.Parse("658f0e61-8c86-493f-a147-d66da2ed194d"),
                "更新钱包",
                LogEnum.Console,
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

                    Global.Happened(new WalletUpdatedEvent(entity));
                });
            Global.Access<RemoveWalletCommand>(
                Guid.Parse("bd70fe34-7575-43d0-a8e5-d8e9566d8d56"),
                "移除钱包",
                LogEnum.Console,
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

                    Global.Happened(new WalletRemovedEvent(entity));
                });
        }

        private bool _isInited = false;
        private object _locker = new object();

        public int Count {
            get {
                InitOnece();
                return _dicById.Count;
            }
        }

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
            if (!_isInited) {
                if (UseRemoteWalletList) {
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

        public bool Contains(Guid walletId) {
            InitOnece();
            return _dicById.ContainsKey(walletId);
        }

        public bool TryGetWallet(Guid walletId, out IWallet wallet) {
            InitOnece();
            WalletData wlt;
            bool r = _dicById.TryGetValue(walletId, out wlt);
            wallet = wlt;
            return r;
        }

        public IEnumerator<IWallet> GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }
    }
}
