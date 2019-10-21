using NTMiner.MinerServer;
using System;
using System.Collections;
using System.Collections.Generic;

namespace NTMiner.Core.MinerServer.Impl {
    public class NTMinerWalletSet : INTMinerWalletSet {
        private readonly Dictionary<Guid, NTMinerWalletData> _dicById = new Dictionary<Guid, NTMinerWalletData>();
        private readonly INTMinerRoot _root;

        public NTMinerWalletSet(INTMinerRoot root) {
            _root = root;
            VirtualRoot.CreateCmdPath<AddNTMinerWalletCommand>(action: (message) => {
                if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                    throw new ArgumentNullException();
                }
                if (string.IsNullOrEmpty(message.Input.Wallet)) {
                    throw new ValidationException("NTMinerWallet Wallet can't be null or empty");
                }
                if (_dicById.ContainsKey(message.Input.GetId())) {
                    return;
                }
                NTMinerWalletData entity = new NTMinerWalletData().Update(message.Input);
                OfficialServer.NTMinerWalletService.AddOrUpdateNTMinerWalletAsync(entity, (response, e) => {
                    if (response.IsSuccess()) {
                        _dicById.Add(entity.Id, entity);
                        VirtualRoot.Happened(new NTMinerWalletAddedEvent(entity));
                    }
                    else {
                        Write.UserFail(response.ReadMessage(e));
                    }
                });
            });
            VirtualRoot.CreateCmdPath<UpdateNTMinerWalletCommand>(action: (message) => {
                if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                    throw new ArgumentNullException();
                }
                if (string.IsNullOrEmpty(message.Input.Wallet)) {
                    throw new ValidationException("minerGroup Wallet can't be null or empty");
                }
                if (!_dicById.ContainsKey(message.Input.GetId())) {
                    return;
                }
                NTMinerWalletData entity = _dicById[message.Input.GetId()];
                NTMinerWalletData oldValue = new NTMinerWalletData().Update(entity);
                entity.Update(message.Input);
                OfficialServer.NTMinerWalletService.AddOrUpdateNTMinerWalletAsync(entity, (response, e) => {
                    if (!response.IsSuccess()) {
                        entity.Update(oldValue);
                        VirtualRoot.Happened(new NTMinerWalletUpdatedEvent(entity));
                        Write.UserFail(response.ReadMessage(e));
                    }
                });
                VirtualRoot.Happened(new NTMinerWalletUpdatedEvent(entity));
            });
            VirtualRoot.CreateCmdPath<RemoveNTMinerWalletCommand>(action: (message) => {
                if (message == null || message.EntityId == Guid.Empty) {
                    throw new ArgumentNullException();
                }
                if (!_dicById.ContainsKey(message.EntityId)) {
                    return;
                }
                NTMinerWalletData entity = _dicById[message.EntityId];
                OfficialServer.NTMinerWalletService.RemoveNTMinerWalletAsync(entity.Id, (response, e) => {
                    if (response.IsSuccess()) {
                        _dicById.Remove(entity.Id);
                        VirtualRoot.Happened(new NTMinerWalletRemovedEvent(entity));
                    }
                    else {
                        Write.UserFail(response.ReadMessage(e));
                    }
                });
            });
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
            if (_isInited) {
                return;
            }
            _isInited = true;
            OfficialServer.NTMinerWalletService.GetNTMinerWalletsAsync((response, e) => {
                if (response.IsSuccess()) {
                    foreach (var item in response.Data) {
                        if (!_dicById.ContainsKey(item.GetId())) {
                            _dicById.Add(item.GetId(), item);
                        }
                    }
                }
                VirtualRoot.Happened(new NTMinerWalletSetInitedEvent());
            });
        }

        public bool TryGetNTMinerWallet(Guid id, out INTMinerWallet wallet) {
            InitOnece();
            var r = _dicById.TryGetValue(id, out NTMinerWalletData g);
            wallet = g;
            return r;
        }

        public IEnumerator<INTMinerWallet> GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }
    }
}
