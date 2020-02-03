using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.MinerServer.Impl {
    public class NTMinerWalletSet : INTMinerWalletSet {
        private readonly Dictionary<Guid, NTMinerWalletData> _dicById = new Dictionary<Guid, NTMinerWalletData>();

        public NTMinerWalletSet() {
            VirtualRoot.AddCmdPath<AddNTMinerWalletCommand>(action: (message) => {
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
                RpcRoot.OfficialServer.NTMinerWalletService.AddOrUpdateNTMinerWalletAsync(entity, (response, e) => {
                    if (response.IsSuccess()) {
                        _dicById.Add(entity.Id, entity);
                        VirtualRoot.RaiseEvent(new NTMinerWalletAddedEvent(message.Id, entity));
                    }
                    else {
                        VirtualRoot.Out.ShowError(response.ReadMessage(e), autoHideSeconds: 4);
                    }
                });
            }, location: this.GetType());
            VirtualRoot.AddCmdPath<UpdateNTMinerWalletCommand>(action: (message) => {
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
                RpcRoot.OfficialServer.NTMinerWalletService.AddOrUpdateNTMinerWalletAsync(entity, (response, e) => {
                    if (!response.IsSuccess()) {
                        entity.Update(oldValue);
                        VirtualRoot.RaiseEvent(new NTMinerWalletUpdatedEvent(message.Id, entity));
                        VirtualRoot.Out.ShowError(response.ReadMessage(e), autoHideSeconds: 4);
                    }
                });
                VirtualRoot.RaiseEvent(new NTMinerWalletUpdatedEvent(message.Id, entity));
            }, location: this.GetType());
            VirtualRoot.AddCmdPath<RemoveNTMinerWalletCommand>(action: (message) => {
                if (message == null || message.EntityId == Guid.Empty) {
                    throw new ArgumentNullException();
                }
                if (!_dicById.ContainsKey(message.EntityId)) {
                    return;
                }
                NTMinerWalletData entity = _dicById[message.EntityId];
                RpcRoot.OfficialServer.NTMinerWalletService.RemoveNTMinerWalletAsync(entity.Id, (response, e) => {
                    if (response.IsSuccess()) {
                        _dicById.Remove(entity.Id);
                        VirtualRoot.RaiseEvent(new NTMinerWalletRemovedEvent(message.Id, entity));
                    }
                    else {
                        VirtualRoot.Out.ShowError(response.ReadMessage(e), autoHideSeconds: 4);
                    }
                });
            }, location: this.GetType());
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
            RpcRoot.OfficialServer.NTMinerWalletService.GetNTMinerWalletsAsync((response, e) => {
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
