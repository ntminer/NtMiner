using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.SysDics.Impl {
    internal class SysDicItemSet : ISysDicItemSet {

        private readonly INTMinerRoot _root;
        private readonly Dictionary<Guid, Dictionary<string, SysDicItemData>> _dicByDicId = new Dictionary<Guid, Dictionary<string, SysDicItemData>>();
        private readonly Dictionary<Guid, SysDicItemData> _dicById = new Dictionary<Guid, SysDicItemData>();

        public SysDicItemSet(INTMinerRoot root) {
            _root = root;
            Global.Access<RefreshSysDicItemSetCommand>(
                Guid.Parse("AE1E89D1-3282-4875-9AD3-3C50BB5DD1C6"),
                "处理刷新系统字典项数据集命令",
                LogEnum.Console,
                action: message => {
                    var repository = NTMinerRoot.CreateServerRepository<SysDicItemData>();
                    foreach (var item in repository.GetAll()) {
                        if (_dicById.ContainsKey(item.Id)) {
                            Global.Execute(new UpdateSysDicItemCommand(item));
                        }
                        else {
                            Global.Execute(new AddSysDicItemCommand(item));
                        }
                    }
                });
            Global.Access<AddSysDicItemCommand>(
                Guid.Parse("485407c5-ffe0-462d-b05f-a13418307be0"),
                "添加系统字典项",
                LogEnum.Console,
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
                    var repository = NTMinerRoot.CreateServerRepository<SysDicItemData>();
                    repository.Add(entity);

                    Global.Happened(new SysDicItemAddedEvent(entity));
                });
            Global.Access<UpdateSysDicItemCommand>(
                Guid.Parse("0379df7f-9f34-449a-91b2-4bd32e0c287f"),
                "更新系统字典项",
                LogEnum.Console,
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
                    entity.Update(message.Input);
                    var repository = NTMinerRoot.CreateServerRepository<SysDicItemData>();
                    repository.Update(entity);

                    Global.Happened(new SysDicItemUpdatedEvent(entity));
                });
            Global.Access<RemoveSysDicItemCommand>(
                Guid.Parse("d0b7b706-2a57-492c-842d-03a4281ecfdf"),
                "移除系统字典项",
                LogEnum.Console,
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
                    var repository = NTMinerRoot.CreateServerRepository<SysDicItemData>();
                    repository.Remove(entity.Id);

                    Global.Happened(new SysDicItemRemovedEvent(entity));
                });
            Global.Logger.InfoDebugLine(this.GetType().FullName + "接入总线");
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
                    var repository = NTMinerRoot.CreateServerRepository<SysDicItemData>();
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
