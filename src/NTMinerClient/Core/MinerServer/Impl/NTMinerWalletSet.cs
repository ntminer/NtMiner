using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.MinerServer.Impl {
    public class NTMinerWalletSet : INTMinerWalletSet {
        private readonly Dictionary<Guid, NTMinerWalletData> _dicById = new Dictionary<Guid, NTMinerWalletData>();

        public NTMinerWalletSet() {
            VirtualRoot.BuildCmdPath<AddNTMinerWalletCommand>(action: (message) => {
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
                        VirtualRoot.RaiseEvent(new NTMinerWalletAddedEvent(entity));
                    }
                    else {
                        Write.UserFail(response.ReadMessage(e));
                    }
                });
            });
            VirtualRoot.BuildCmdPath<UpdateNTMinerWalletCommand>(action: (message) => {
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
                        VirtualRoot.RaiseEvent(new NTMinerWalletUpdatedEvent(entity));
                        Write.UserFail(response.ReadMessage(e));
                    }
                });
                VirtualRoot.RaiseEvent(new NTMinerWalletUpdatedEvent(entity));
            });
            VirtualRoot.BuildCmdPath<RemoveNTMinerWalletCommand>(action: (message) => {
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
                        VirtualRoot.RaiseEvent(new NTMinerWalletRemovedEvent(entity));
                    }
                    else {
                        Write.UserFail(response.ReadMessage(e));
                    }
                });
            });
        }

        private bool _isInited = false;

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
                VirtualRoot.RaiseEvent(new NTMinerWalletSetInitedEvent());
            });
        }

        public bool TryGetNTMinerWallet(Guid id, out INTMinerWallet wallet) {
            InitOnece();
            var r = _dicById.TryGetValue(id, out NTMinerWalletData g);
            wallet = g;
            return r;
        }

        public IEnumerable<INTMinerWallet> AsEnumerable() {
            InitOnece();
            return _dicById.Values;
        }
    }
}
