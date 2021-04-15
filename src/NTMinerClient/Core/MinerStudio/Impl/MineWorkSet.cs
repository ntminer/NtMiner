using System;
using System.Collections.Generic;

namespace NTMiner.Core.MinerStudio.Impl {
    public class MineWorkSet : SetBase, IMineWorkSet {
        private readonly Dictionary<Guid, MineWorkData> _dicById = new Dictionary<Guid, MineWorkData>();

        public MineWorkSet() {
            VirtualRoot.BuildEventPath<MinerStudioServiceSwitchedEvent>("切换了群口后台服务类型后刷新内存", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, path: message => {
                _dicById.Clear();
                base.DeferReInit();
                // 初始化以触发MineWorkSetInitedEvent事件
                InitOnece();
            });
            VirtualRoot.BuildCmdPath<AddMineWorkCommand>(this.GetType(), LogEnum.DevConsole, path: message => {
                InitOnece();
                if (!_dicById.ContainsKey(message.Input.Id)) {
                    var repository = VirtualRoot.CreateLocalRepository<MineWorkData>();
                    var data = new MineWorkData().Update(message.Input);
                    data.CreatedOn = DateTime.Now;
                    _dicById.Add(data.Id, data);
                    repository.Add(data);
                    VirtualRoot.RaiseEvent(new MineWorkAddedEvent(message.MessageId, data));
                }
            });
            VirtualRoot.BuildCmdPath<UpdateMineWorkCommand>(this.GetType(), LogEnum.DevConsole, path: message => {
                InitOnece();
                if (_dicById.TryGetValue(message.Input.Id, out MineWorkData data)) {
                    var repository = VirtualRoot.CreateLocalRepository<MineWorkData>();
                    data.Update(message.Input);
                    data.ModifiedOn = DateTime.Now;
                    repository.Update(data);
                    VirtualRoot.RaiseEvent(new MineWorkUpdatedEvent(message.MessageId, data));
                }
            });
            VirtualRoot.BuildCmdPath<RemoveMineWorkCommand>(this.GetType(), LogEnum.DevConsole, path: message => {
                InitOnece();
                if (_dicById.TryGetValue(message.EntityId, out MineWorkData entity)) {
                    _dicById.Remove(message.EntityId);
                    var repository = VirtualRoot.CreateLocalRepository<MineWorkData>();
                    repository.Remove(message.EntityId);
                    MinerStudioPath.DeleteMineWorkFiles(message.EntityId);
                    VirtualRoot.RaiseEvent(new MineWorkRemovedEvent(message.MessageId, entity));
                }
            });
        }

        protected override void Init() {
            if (RpcRoot.IsOuterNet) {
                RpcRoot.OfficialServer.UserMineWorkService.GetMineWorksAsync((response, e) => {
                    if (response.IsSuccess()) {
                        foreach (var item in response.Data) {
                            _dicById.Add(item.Id, item);
                        }
                    }
                    VirtualRoot.RaiseEvent(new MineWorkSetInitedEvent());
                });
            }
            else {
                var repository = VirtualRoot.CreateLocalRepository<MineWorkData>();
                foreach (var item in repository.GetAll()) {
                    _dicById.Add(item.Id, item);
                }
                VirtualRoot.RaiseEvent(new MineWorkSetInitedEvent());
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
