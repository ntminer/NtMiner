using System;
using System.Collections.Generic;

namespace NTMiner.Core.MinerStudio.Impl {
    public class MinerGroupSet : IMinerGroupSet {
        private readonly Dictionary<Guid, MinerGroupData> _dicById = new Dictionary<Guid, MinerGroupData>();

        public MinerGroupSet() {
            VirtualRoot.AddEventPath<MinerStudioServiceSwitchedEvent>("切换了群口后台服务类型后刷新内存", LogEnum.DevConsole, action: message => {
                _dicById.Clear();
                _isInited = false;
            }, this.GetType());
            VirtualRoot.AddCmdPath<AddMinerGroupCommand>(action: message => {
                InitOnece();
                if (!_dicById.ContainsKey(message.Input.Id)) {
                    var repository = VirtualRoot.CreateLocalRepository<MinerGroupData>();
                    var data = new MinerGroupData().Update(message.Input);
                    data.CreatedOn = DateTime.Now;
                    _dicById.Add(data.Id, data);
                    repository.Add(data);
                    VirtualRoot.RaiseEvent(new MinerGroupAddedEvent(message.MessageId, data));
                }
            }, this.GetType());
            VirtualRoot.AddCmdPath<UpdateMinerGroupCommand>(action: message => {
                InitOnece();
                if (_dicById.TryGetValue(message.Input.Id, out MinerGroupData data)) {
                    var repository = VirtualRoot.CreateLocalRepository<MinerGroupData>();
                    data.Update(message.Input);
                    data.ModifiedOn = DateTime.Now;
                    repository.Update(data);
                    VirtualRoot.RaiseEvent(new MinerGroupUpdatedEvent(message.MessageId, data));
                }
            }, this.GetType());
            VirtualRoot.AddCmdPath<RemoveMinerGroupCommand>(action: message => {
                InitOnece();
                if (_dicById.TryGetValue(message.EntityId, out MinerGroupData entity)) {
                    _dicById.Remove(message.EntityId);
                    var repository = VirtualRoot.CreateLocalRepository<MinerGroupData>();
                    repository.Remove(message.EntityId);
                    VirtualRoot.RaiseEvent(new MinerGroupRemovedEvent(message.MessageId, entity));
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
                        RpcRoot.OfficialServer.UserMinerGroupService.GetMinerGroupsAsync((response, e) => {
                            if (response.IsSuccess()) {
                                foreach (var item in response.Data) {
                                    _dicById.Add(item.Id, item);
                                }
                            }
                            _isInited = true;
                            VirtualRoot.RaiseEvent(new MinerGroupSetInitedEvent());
                        });
                    }
                    else {
                        var repository = VirtualRoot.CreateLocalRepository<MinerGroupData>();
                        foreach (var item in repository.GetAll()) {
                            _dicById.Add(item.Id, item);
                        }
                        _isInited = true;
                        VirtualRoot.RaiseEvent(new MinerGroupSetInitedEvent());
                    }
                }
            }
        }

        public bool Contains(Guid id) {
            InitOnece();
            return _dicById.ContainsKey(id);
        }

        public IEnumerable<MinerGroupData> AsEnumerable() {
            InitOnece();
            return _dicById.Values;
        }
    }
}
