using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.SysDics.Impl {
    internal class SysDicItemSet : ISysDicItemSet {

        private readonly INTMinerRoot _root;
        private readonly Dictionary<Guid, Dictionary<string, SysDicItemData>> _dicByDicId = new Dictionary<Guid, Dictionary<string, SysDicItemData>>();
        private readonly Dictionary<Guid, SysDicItemData> _dicById = new Dictionary<Guid, SysDicItemData>();

        private readonly bool _isUseJson;
        public SysDicItemSet(INTMinerRoot root, bool isUseJson) {
            _root = root;
            _isUseJson = isUseJson;
            VirtualRoot.Window<AddSysDicItemCommand>("添加系统字典项", LogEnum.DevConsole,
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
                        throw new DuplicateCodeException();
                    }
                    SysDicItemData entity = new SysDicItemData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    _dicByDicId[message.Input.DicId].Add(entity.Code, entity);
                    var repository = NTMinerRoot.CreateServerRepository<SysDicItemData>(isUseJson);
                    repository.Add(entity);

                    VirtualRoot.Happened(new SysDicItemAddedEvent(entity));
                }).AddToCollection(root.ContextHandlers);
            VirtualRoot.Window<UpdateSysDicItemCommand>("更新系统字典项", LogEnum.DevConsole,
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
                    entity.Update(message.Input);
                    var repository = NTMinerRoot.CreateServerRepository<SysDicItemData>(isUseJson);
                    repository.Update(entity);

                    VirtualRoot.Happened(new SysDicItemUpdatedEvent(entity));
                }).AddToCollection(root.ContextHandlers);
            VirtualRoot.Window<RemoveSysDicItemCommand>("移除系统字典项", LogEnum.DevConsole,
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
                    var repository = NTMinerRoot.CreateServerRepository<SysDicItemData>(isUseJson);
                    repository.Remove(entity.Id);

                    VirtualRoot.Happened(new SysDicItemRemovedEvent(entity));
                }).AddToCollection(root.ContextHandlers);
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
                    var repository = NTMinerRoot.CreateServerRepository<SysDicItemData>(_isUseJson);
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

        public IEnumerator<ISysDicItem> GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        public bool ContainsKey(Guid dicItemId) {
            InitOnece();
            return _dicById.ContainsKey(dicItemId);
        }

        public bool ContainsKey(Guid dicId, string dicItemCode) {
            InitOnece();
            Dictionary<string, SysDicItemData> items;
            if (!_dicByDicId.TryGetValue(dicId, out items)) {
                return false;
            }
            return _dicByDicId[dicId].ContainsKey(dicItemCode);
        }

        public bool ContainsKey(string dicCode, string dicItemCode) {
            InitOnece();
            ISysDic sysDic;
            if (!_root.SysDicSet.TryGetSysDic(dicCode, out sysDic)) {
                return false;
            }
            if (!_dicByDicId.ContainsKey(sysDic.GetId())) {
                return false;
            }
            return _dicByDicId[sysDic.GetId()].ContainsKey(dicItemCode);
        }

        public bool TryGetDicItem(Guid dicItemId, out ISysDicItem dicItem) {
            InitOnece();
            SysDicItemData di;
            var r = _dicById.TryGetValue(dicItemId, out di);
            dicItem = di;
            return r;
        }

        public bool TryGetDicItem(string dicCode, string dicItemCode, out ISysDicItem dicItem) {
            InitOnece();
            ISysDic sysDic;
            if (!_root.SysDicSet.TryGetSysDic(dicCode, out sysDic)) {
                dicItem = null;
                return false;
            }
            Dictionary<string, SysDicItemData> items;
            if (!_dicByDicId.TryGetValue(sysDic.GetId(), out items)) {
                dicItem = null;
                return false;
            }
            SysDicItemData di;
            var r = items.TryGetValue(dicItemCode, out di);
            dicItem = di;
            return r;
        }

        public bool TryGetDicItem(Guid dicId, string dicItemCode, out ISysDicItem dicItem) {
            InitOnece();
            Dictionary<string, SysDicItemData> items;
            if (!_dicByDicId.TryGetValue(dicId, out items)) {
                dicItem = null;
                return false;
            }
            SysDicItemData di;
            var r = items.TryGetValue(dicItemCode, out di);
            dicItem = di;
            return r;
        }

        public IEnumerable<ISysDicItem> GetSysDicItems(string dicCode) {
            InitOnece();
            ISysDic sysDic;
            if (!_root.SysDicSet.TryGetSysDic(dicCode, out sysDic)) {
                return new List<ISysDicItem>();
            }
            return _dicByDicId[sysDic.GetId()].Values.ToList();
        }
    }
}
