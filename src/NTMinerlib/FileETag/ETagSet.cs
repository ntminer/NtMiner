using NTMiner.FileETag.Impl;
using NTMiner.Repositories;
using System;
using System.Collections.Generic;

namespace NTMiner.FileETag {
    public class ETagSet {
        public static readonly ETagSet Instance = new ETagSet();

        // 注意ETag的key是区分大小写的，因为云服务器往往是linux，linux的文件名区分大小写，而ETag的key是文件名
        private readonly Dictionary<string, ETag> _dicByKey = new Dictionary<string, ETag>();
        private readonly Dictionary<string, ETag> _dicByValue = new Dictionary<string, ETag>();
        private readonly Dictionary<Guid, ETag> _dicById = new Dictionary<Guid, ETag>();

        private ETagSet() {
            Global.Access<AddETagCommand>(
                Guid.Parse("BAE23025-199B-4E5D-A90E-C6C33F4C4075"),
                "处理添加ETag命令",
                LogEnum.None,
                action: message => {
                    var entity = new ETag().Update(message.Input);
                    if (!string.IsNullOrEmpty(message.Input.Key) && !_dicByKey.ContainsKey(message.Input.Key)) {
                        _dicByKey.Add(message.Input.Key, entity);
                    }
                    if (!string.IsNullOrEmpty(message.Input.Value) && !_dicByValue.ContainsKey(message.Input.Value)) {
                        _dicByValue.Add(message.Input.Value, entity);
                    }
                    IRepository<ETag> repository = Repository.CreateETagRepository<ETag>();
                    if (!_dicById.ContainsKey(entity.Id)) {
                        _dicById.Add(entity.Id, entity);
                    }
                    repository.Add(entity);
                    Global.Happened(new ETagAddedEvent(entity));
                });
            Global.Access<UpdateETagCommand>(
                Guid.Parse("CC801DF8-6B53-4900-A2CB-86B0DFE6C489"),
                "处理修改ETag命令",
                LogEnum.None,
                action: message => {
                    ETag entity;
                    if (_dicById.TryGetValue(message.Input.GetId(), out entity)) {
                        string value = entity.Value;
                        entity.Update(message.Input);
                        if (value != entity.Value) {
                            _dicByValue.Remove(value);
                            _dicByValue.Add(entity.Value, entity);
                        }
                        IRepository<ETag> repository = Repository.CreateETagRepository<ETag>();
                        repository.Update(entity);
                        Global.Happened(new ETagUpdatedEvent(entity));
                    }
                });
            Global.Access<RemoveETagCommand>(
                Guid.Parse("75DDFBB8-B6C9-4D45-9ABA-90850CF66F73"),
                "处理删除ETag命令",
                LogEnum.None,
                action: message => {
                    ETag entity;
                    if (message.EntityId != Guid.Empty && _dicById.TryGetValue(message.EntityId, out entity)) {
                        _dicById.Remove(entity.Id);
                        _dicByKey.Remove(entity.Key);
                        _dicByValue.Remove(entity.Value);
                    }
                    else if(!string.IsNullOrEmpty(message.Key) && _dicByKey.TryGetValue(message.Key, out entity)){
                        _dicById.Remove(entity.Id);
                        _dicByKey.Remove(entity.Key);
                        _dicByValue.Remove(entity.Value);
                    }
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
                    IRepository<ETag> repository = Repository.CreateETagRepository<ETag>();
                    foreach (var item in repository.GetAll()) {
                        if (!_dicByKey.ContainsKey(item.Key)) {
                            _dicByKey.Add(item.Key, item);
                        }
                        if (!_dicByValue.ContainsKey(item.Value)) {
                            _dicByValue.Add(item.Value, item);
                        }
                        if (!_dicById.ContainsKey(item.Id)) {
                            _dicById.Add(item.Id, item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public bool TryGetETagByKey(string key, out ETag etag) {
            InitOnece();
            return _dicByKey.TryGetValue(key, out etag);
        }

        public bool TryGetETagByValue(string value, out ETag etag) {
            InitOnece();
            return _dicByValue.TryGetValue(value, out etag);
        }
    }
}
