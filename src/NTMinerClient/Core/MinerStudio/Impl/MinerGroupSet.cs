using System;
using System.Collections.Generic;

namespace NTMiner.Core.MinerStudio.Impl {
    public class MinerGroupSet : SetBase, IMinerGroupSet {
        private readonly Dictionary<Guid, MinerGroupData> _dicById = new Dictionary<Guid, MinerGroupData>();

        public MinerGroupSet() {
            VirtualRoot.BuildEventPath<MinerStudioServiceSwitchedEvent>("切换了群口后台服务类型后刷新内存", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, path: message => {
                _dicById.Clear();
                base.DeferReInit();
                // 初始化以触发MinerGroupSetInitedEvent事件
                InitOnece();
            });
            VirtualRoot.BuildCmdPath<AddMinerGroupCommand>(this.GetType(), LogEnum.DevConsole, path: message => {
                InitOnece();
                if (!_dicById.ContainsKey(message.Input.Id)) {
                    var repository = VirtualRoot.CreateLocalRepository<MinerGroupData>();
                    var data = new MinerGroupData().Update(message.Input);
                    data.CreatedOn = DateTime.Now;
                    _dicById.Add(data.Id, data);
                    repository.Add(data);
                    VirtualRoot.RaiseEvent(new MinerGroupAddedEvent(message.MessageId, data));
                }
            });
            VirtualRoot.BuildCmdPath<UpdateMinerGroupCommand>(this.GetType(), LogEnum.DevConsole, path: message => {
                InitOnece();
                if (_dicById.TryGetValue(message.Input.Id, out MinerGroupData data)) {
                    var repository = VirtualRoot.CreateLocalRepository<MinerGroupData>();
                    data.Update(message.Input);
                    data.ModifiedOn = DateTime.Now;
                    repository.Update(data);
                    VirtualRoot.RaiseEvent(new MinerGroupUpdatedEvent(message.MessageId, data));
                }
            });
            VirtualRoot.BuildCmdPath<RemoveMinerGroupCommand>(this.GetType(), LogEnum.DevConsole, path: message => {
                InitOnece();
                if (_dicById.TryGetValue(message.EntityId, out MinerGroupData entity)) {
                    _dicById.Remove(message.EntityId);
                    var repository = VirtualRoot.CreateLocalRepository<MinerGroupData>();
                    repository.Remove(message.EntityId);
                    VirtualRoot.RaiseEvent(new MinerGroupRemovedEvent(message.MessageId, entity));
                }
            });
        }

        protected override void Init() {
            if (RpcRoot.IsOuterNet) {
                RpcRoot.OfficialServer.UserMinerGroupService.GetMinerGroupsAsync((response, e) => {
                    if (response.IsSuccess()) {
                        foreach (var item in response.Data) {
                            _dicById.Add(item.Id, item);
                        }
                    }
                    VirtualRoot.RaiseEvent(new MinerGroupSetInitedEvent());
                });
            }
            else {
                var repository = VirtualRoot.CreateLocalRepository<MinerGroupData>();
                foreach (var item in repository.GetAll()) {
                    _dicById.Add(item.Id, item);
                }
                VirtualRoot.RaiseEvent(new MinerGroupSetInitedEvent());
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
