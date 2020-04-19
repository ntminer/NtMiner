using System;
using System.Collections.Generic;

namespace NTMiner.Core.MinerStudio.Impl {
    public class MineWorkSet : IMineWorkSet {
        private readonly Dictionary<Guid, MineWorkData> _dicById = new Dictionary<Guid, MineWorkData>();

        public MineWorkSet() {
            VirtualRoot.AddCmdPath<AddMineWorkCommand>(action: message => {
                InitOnece();
                if (!_dicById.ContainsKey(message.Input.Id)) {
                    var repository = VirtualRoot.CreateLocalRepository<MineWorkData>();
                    var data = new MineWorkData().Update(message.Input);
                    data.CreatedOn = DateTime.Now;
                    _dicById.Add(data.Id, data);
                    repository.Add(data);
                    VirtualRoot.RaiseEvent(new MineWorkAddedEvent(message.MessageId, data));
                }
            }, this.GetType());
            VirtualRoot.AddCmdPath<UpdateMineWorkCommand>(action: message => {
                InitOnece();
                if (_dicById.TryGetValue(message.Input.Id, out MineWorkData data)) {
                    var repository = VirtualRoot.CreateLocalRepository<MineWorkData>();
                    data.Update(message.Input);
                    data.ModifiedOn = DateTime.Now;
                    repository.Update(data);
                    VirtualRoot.RaiseEvent(new MineWorkUpdatedEvent(message.MessageId, data));
                }
            }, this.GetType());
            VirtualRoot.AddCmdPath<RemoveMineWorkCommand>(action: message => {
                InitOnece();
                if (_dicById.TryGetValue(message.EntityId, out MineWorkData entity)) {
                    _dicById.Remove(message.EntityId);
                    var repository = VirtualRoot.CreateLocalRepository<MineWorkData>();
                    repository.Remove(message.EntityId);
                    MinerStudioPath.DeleteMineWorkFiles(message.EntityId);
                    VirtualRoot.RaiseEvent(new MineWorkRemovedEvent(message.MessageId, entity));
                }
            }, this.GetType());
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
            lock (_locker) {
                if (!_isInited) {
                    if (RpcRoot.IsOuterNet) {
                        RpcRoot.OfficialServer.UserMineWorkService.GetMineWorksAsync((response, e) => {
                            if (response.IsSuccess()) {
                                foreach (var item in response.Data) {
                                    _dicById.Add(item.Id, item);
                                }
                                VirtualRoot.RaiseEvent(new MineWorkSetInitedEvent());
                            }
                        });
                    }
                    else {
                        var repository = VirtualRoot.CreateLocalRepository<MineWorkData>();
                        foreach (var item in repository.GetAll()) {
                            _dicById.Add(item.Id, item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public bool Contains(Guid mineWorkId) {
            InitOnece();
            return _dicById.ContainsKey(mineWorkId);
        }

        public IEnumerable<MineWorkData> AsEnumerable() {
            InitOnece();
            return _dicById.Values;
        }
    }
}
