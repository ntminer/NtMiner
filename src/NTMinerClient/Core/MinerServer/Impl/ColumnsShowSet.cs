using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.MinerServer.Impl {
    public class ColumnsShowSet : IColumnsShowSet {
        private readonly Dictionary<Guid, ColumnsShowData> _dicById = new Dictionary<Guid, ColumnsShowData>();

        public ColumnsShowSet() {
            VirtualRoot.AddCmdPath<AddColumnsShowCommand>(action: (message) => {
                InitOnece();
                if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty || message.Input.GetId() == ColumnsShowData.PleaseSelect.Id) {
                    throw new ArgumentNullException();
                }
                if (_dicById.ContainsKey(message.Input.GetId())) {
                    return;
                }
                ColumnsShowData entity = new ColumnsShowData().Update(message.Input);
                RpcRoot.Server.ColumnsShowService.AddOrUpdateColumnsShowAsync(entity, (response, exception) => {
                    if (response.IsSuccess()) {
                        _dicById.Add(entity.Id, entity);
                        VirtualRoot.RaiseEvent(new ColumnsShowAddedEvent(message.Id, entity));
                    }
                    else {
                        VirtualRoot.Out.ShowError(response.ReadMessage(exception), autoHideSeconds: 4);
                    }
                });
            }, location: this.GetType());
            VirtualRoot.AddCmdPath<UpdateColumnsShowCommand>(action: (message) => {
                InitOnece();
                if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                    throw new ArgumentNullException();
                }
                if (!_dicById.ContainsKey(message.Input.GetId())) {
                    return;
                }
                ColumnsShowData entity = _dicById[message.Input.GetId()];
                ColumnsShowData oldValue = new ColumnsShowData().Update(entity);
                entity.Update(message.Input);
                RpcRoot.Server.ColumnsShowService.AddOrUpdateColumnsShowAsync(entity, (response, exception) => {
                    if (!response.IsSuccess()) {
                        entity.Update(oldValue);
                        VirtualRoot.RaiseEvent(new ColumnsShowUpdatedEvent(message.Id, entity));
                        VirtualRoot.Out.ShowError(response.ReadMessage(exception), autoHideSeconds: 4);
                    }
                });
                VirtualRoot.RaiseEvent(new ColumnsShowUpdatedEvent(message.Id, entity));
            }, location: this.GetType());
            VirtualRoot.AddCmdPath<RemoveColumnsShowCommand>(action: (message) => {
                InitOnece();
                if (message == null || message.EntityId == Guid.Empty || message.EntityId == ColumnsShowData.PleaseSelect.Id) {
                    throw new ArgumentNullException();
                }
                if (!_dicById.ContainsKey(message.EntityId)) {
                    return;
                }
                ColumnsShowData entity = _dicById[message.EntityId];
                RpcRoot.Server.ColumnsShowService.RemoveColumnsShowAsync(entity.Id, (response, exception) => {
                    if (response.IsSuccess()) {
                        _dicById.Remove(entity.Id);
                        VirtualRoot.RaiseEvent(new ColumnsShowRemovedEvent(message.Id, entity));
                    }
                    else {
                        VirtualRoot.Out.ShowError(response.ReadMessage(exception), autoHideSeconds: 4);
                    }
                });
            }, location: this.GetType());
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
                lock (_locker) {
                    if (!_isInited) {
                        var result = RpcRoot.Server.ColumnsShowService.GetColumnsShows();
                        foreach (var item in result) {
                            if (!_dicById.ContainsKey(item.GetId())) {
                                _dicById.Add(item.GetId(), item);
                            }
                        }
                        if (!_dicById.ContainsKey(ColumnsShowData.PleaseSelect.Id)) {
                            _dicById.Add(ColumnsShowData.PleaseSelect.Id, ColumnsShowData.PleaseSelect);
                            RpcRoot.Server.ColumnsShowService.AddOrUpdateColumnsShowAsync(ColumnsShowData.PleaseSelect, (response, exception) => {
                                if (!response.IsSuccess()) {
                                    Logger.ErrorDebugLine("AddOrUpdateColumnsShowAsync " + response.ReadMessage(exception));
                                }
                            });
                        }
                        _isInited = true;
                    }
                }
            }
        }

        public bool Contains(Guid columnsShowId) {
            InitOnece();
            return _dicById.ContainsKey(columnsShowId);
        }

        public bool TryGetColumnsShow(Guid columnsShowId, out IColumnsShow columnsShow) {
            InitOnece();
            var r = _dicById.TryGetValue(columnsShowId, out ColumnsShowData g);
            columnsShow = g;
            return r;
        }

        public IEnumerable<IColumnsShow> AsEnumerable() {
            InitOnece();
            return _dicById.Values;
        }
    }
}
