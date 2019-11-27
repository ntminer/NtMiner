using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Impl {
    internal class SysDicItemSet : ISysDicItemSet {
        private readonly IServerContext _context;
        private readonly Dictionary<Guid, Dictionary<string, SysDicItemData>> _dicByDicId = new Dictionary<Guid, Dictionary<string, SysDicItemData>>();
        private readonly Dictionary<Guid, SysDicItemData> _dicById = new Dictionary<Guid, SysDicItemData>();

        public SysDicItemSet(IServerContext context) {
            _context = context;
            _context.BuildCmdPath<AddSysDicItemCommand>("添加系统字典项", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Code)) {
                        throw new ValidationException("dicitem code can't be null or empty");
                    }
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    if (!_dicByDicId.ContainsKey(message.Input.DicId)) {
                        _dicByDicId.Add(message.Input.DicId, new Dictionary<string, SysDicItemData>(StringComparer.OrdinalIgnoreCase));
                    }
                    if (_dicByDicId[message.Input.DicId].ContainsKey(message.Input.Code)) {
                        throw new ValidationException("编码重复");
                    }
                    SysDicItemData entity = new SysDicItemData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    _dicByDicId[message.Input.DicId].Add(entity.Code, entity);
                    var repository = NTMinerRoot.CreateCompositeRepository<SysDicItemData>();
                    repository.Add(entity);

                    VirtualRoot.RaiseEvent(new SysDicItemAddedEvent(message.Id, entity));
                });
            _context.BuildCmdPath<UpdateSysDicItemCommand>("更新系统字典项", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Code)) {
                        throw new ValidationException("sysDicItem code can't be null or empty");
                    }
                    if (!_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    SysDicItemData entity = _dicById[message.Input.GetId()];
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
                    string oldCode = entity.Code;
                    entity.Update(message.Input);
                    // 如果编码变更了 
                    if (oldCode != entity.Code) {
                        _dicByDicId[entity.DicId].Remove(oldCode);
                        _dicByDicId[entity.DicId].Add(entity.Code, entity);
                    }
                    var repository = NTMinerRoot.CreateCompositeRepository<SysDicItemData>();
                    repository.Update(entity);

                    VirtualRoot.RaiseEvent(new SysDicItemUpdatedEvent(message.Id, entity));
                });
            _context.BuildCmdPath<RemoveSysDicItemCommand>("移除系统字典项", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    SysDicItemData entity = _dicById[message.EntityId];
                    _dicById.Remove(entity.Id);
                    if (_dicByDicId.ContainsKey(entity.DicId)) {
                        if (_dicByDicId[entity.DicId].ContainsKey(entity.Code)) {
                            _dicByDicId[entity.DicId].Remove(entity.Code);
                        }
                    }
                    var repository = NTMinerRoot.CreateCompositeRepository<SysDicItemData>();
                    repository.Remove(entity.Id);

                    VirtualRoot.RaiseEvent(new SysDicItemRemovedEvent(message.Id, entity));
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
                    var repository = NTMinerRoot.CreateCompositeRepository<SysDicItemData>();
                    foreach (var item in repository.GetAll()) {
                        if (!_dicById.ContainsKey(item.GetId())) {
                            _dicById.Add(item.GetId(), item);
                        }
                        if (!_dicByDicId.ContainsKey(item.DicId)) {
                            _dicByDicId.Add(item.DicId, new Dictionary<string, SysDicItemData>(StringComparer.OrdinalIgnoreCase));
                        }
                        if (!_dicByDicId[item.DicId].ContainsKey(item.Code)) {
                            _dicByDicId[item.DicId].Add(item.Code, item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public IEnumerable<ISysDicItem> AsEnumerable() {
            InitOnece();
            return _dicById.Values;
        }

        public bool ContainsKey(Guid dicItemId) {
            InitOnece();
            return _dicById.ContainsKey(dicItemId);
        }

        public bool ContainsKey(Guid dicId, string dicItemCode) {
            InitOnece();
            if (!_dicByDicId.TryGetValue(dicId, out _)) {
                return false;
            }
            return _dicByDicId[dicId].ContainsKey(dicItemCode);
        }

        public bool ContainsKey(string dicCode, string dicItemCode) {
            InitOnece();
            if (!_context.SysDicSet.TryGetSysDic(dicCode, out ISysDic sysDic)) {
                return false;
            }
            if (!_dicByDicId.ContainsKey(sysDic.GetId())) {
                return false;
            }
            return _dicByDicId[sysDic.GetId()].ContainsKey(dicItemCode);
        }

        public bool TryGetDicItem(Guid dicItemId, out ISysDicItem dicItem) {
            InitOnece();
            var r = _dicById.TryGetValue(dicItemId, out SysDicItemData di);
            dicItem = di;
            return r;
        }

        public bool TryGetDicItem(string dicCode, string dicItemCode, out ISysDicItem dicItem) {
            InitOnece();
            if (!_context.SysDicSet.TryGetSysDic(dicCode, out ISysDic sysDic)) {
                dicItem = null;
                return false;
            }
            if (!_dicByDicId.TryGetValue(sysDic.GetId(), out Dictionary<string, SysDicItemData> items)) {
                dicItem = null;
                return false;
            }
            var r = items.TryGetValue(dicItemCode, out SysDicItemData di);
            dicItem = di;
            return r;
        }

        public bool TryGetDicItem(Guid dicId, string dicItemCode, out ISysDicItem dicItem) {
            InitOnece();
            if (!_dicByDicId.TryGetValue(dicId, out Dictionary<string, SysDicItemData> items)) {
                dicItem = null;
                return false;
            }
            var r = items.TryGetValue(dicItemCode, out SysDicItemData di);
            dicItem = di;
            return r;
        }

        public IEnumerable<ISysDicItem> GetSysDicItems(string dicCode) {
            InitOnece();
            if (!_context.SysDicSet.TryGetSysDic(dicCode, out ISysDic sysDic)) {
                return new List<ISysDicItem>();
            }
            return _dicByDicId[sysDic.GetId()].Values.ToList();
        }
    }
}
