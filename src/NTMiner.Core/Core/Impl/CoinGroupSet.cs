using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    public class CoinGroupSet : ICoinGroupSet {
        private readonly Dictionary<Guid, CoinGroupData> _dicById = new Dictionary<Guid, CoinGroupData>();

        private readonly INTMinerRoot _root;
        private readonly bool _isUseJson;
        public CoinGroupSet(INTMinerRoot root, bool isUseJson) {
            _root = root;
            _isUseJson = isUseJson;
            _root.ServerContextWindow<AddCoinGroupCommand>("添加币组", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    CoinGroupData entity = new CoinGroupData().Update(message.Input);
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
                    _dicById.Add(entity.Id, entity);
                    var repository = NTMinerRoot.CreateServerRepository<CoinGroupData>(isUseJson);
                    repository.Add(entity);

                    VirtualRoot.Happened(new CoinGroupAddedEvent(entity));
                });
            _root.ServerContextWindow<RemoveCoinGroupCommand>("移除币组", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    CoinGroupData entity = _dicById[message.EntityId];
                    _dicById.Remove(entity.GetId());
                    var repository = NTMinerRoot.CreateServerRepository<CoinGroupData>(isUseJson);
                    repository.Remove(message.EntityId);

                    VirtualRoot.Happened(new CoinGroupRemovedEvent(entity));
                });
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
            lock (_locker) {
                if (!_isInited) {
                    var repository = NTMinerRoot.CreateServerRepository<CoinGroupData>(_isUseJson);
                    foreach (var item in repository.GetAll()) {
                        if (!_dicById.ContainsKey(item.GetId())) {
                            _dicById.Add(item.GetId(), item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public List<Guid> GetGroupCoinIds(Guid groupId) {
            InitOnece();
            return _dicById.Values.Where(a => a.GroupId == groupId).Select(a => a.CoinId).ToList();
        }

        public IEnumerator<ICoinGroup> GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }
    }
}
