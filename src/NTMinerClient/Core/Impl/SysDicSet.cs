using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    internal class SysDicSet : ISysDicSet {
        private readonly Dictionary<string, SysDicData> _dicByCode = new Dictionary<string, SysDicData>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<Guid, SysDicData> _dicById = new Dictionary<Guid, SysDicData>();

        public SysDicSet(IServerContext context) {
            context.BuildCmdPath<AddSysDicCommand>("添加系统字典", LogEnum.DevConsole,
                action: message => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Code)) {
                        throw new ValidationException("dic code can't be null or empty");
                    }
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    if (_dicByCode.ContainsKey(message.Input.Code)) {
                        throw new ValidationException("编码重复");
                    }
                    SysDicData entity = new SysDicData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    _dicByCode.Add(entity.Code, entity);
                    var repository = NTMinerRoot.CreateServerRepository<SysDicData>();
                    repository.Add(entity);

                    VirtualRoot.RaiseEvent(new SysDicAddedEvent(entity));
                });
            context.BuildCmdPath<UpdateSysDicCommand>("更新系统字典", LogEnum.DevConsole,
                action: message => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Code)) {
                        throw new ValidationException("sysDic code can't be null or empty");
                    }
                    if (!_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    SysDicData entity = _dicById[message.Input.GetId()];
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
                    entity.Update(message.Input);
                    var repository = NTMinerRoot.CreateServerRepository<SysDicData>();
                    repository.Update(entity);

                    VirtualRoot.RaiseEvent(new SysDicUpdatedEvent(entity));
                });
            context.BuildCmdPath<RemoveSysDicCommand>("移除系统字典", LogEnum.DevConsole,
                action: message => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    SysDicData entity = _dicById[message.EntityId];
                    List<Guid> toRemoves = context.SysDicItemSet.GetSysDicItems(entity.Code).Select(a => a.GetId()).ToList();
                    foreach (var id in toRemoves) {
                        VirtualRoot.Execute(new RemoveSysDicItemCommand(id));
                    }
                    _dicById.Remove(entity.Id);
                    if (_dicByCode.ContainsKey(entity.Code)) {
                        _dicByCode.Remove(entity.Code);
                    }
                    var repository = NTMinerRoot.CreateServerRepository<SysDicData>();
                    repository.Remove(entity.Id);

                    VirtualRoot.RaiseEvent(new SysDicRemovedEvent(entity));
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
            lock (_locker) {
                if (!_isInited) {
                    var repository = NTMinerRoot.CreateServerRepository<SysDicData>();
                    foreach (var item in repository.GetAll()) {
                        if (!_dicById.ContainsKey(item.GetId())) {
                            _dicById.Add(item.GetId(), item);
                        }
                        if (!_dicByCode.ContainsKey(item.Code)) {
                            _dicByCode.Add(item.Code, item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public bool ContainsKey(Guid sysDicId) {
            InitOnece();
            return _dicById.ContainsKey(sysDicId);
        }

        public bool ContainsKey(string sysDicCode) {
            InitOnece();
            return _dicByCode.ContainsKey(sysDicCode);
        }

        public bool TryGetSysDic(string sysDicCode, out ISysDic sysDic) {
            InitOnece();
            var r = _dicByCode.TryGetValue(sysDicCode, out SysDicData d);
            sysDic = d;
            return r;
        }

        public bool TryGetSysDic(Guid sysDicId, out ISysDic sysDic) {
            InitOnece();
            var r = _dicById.TryGetValue(sysDicId, out SysDicData d);
            sysDic = d;
            return r;
        }

        public IEnumerable<ISysDic> AsEnumerable() {
            InitOnece();
            return _dicById.Values;
        }
    }
}
