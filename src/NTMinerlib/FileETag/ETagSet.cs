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

        public void AddOrUpdateETag(string key, string value) {
            InitOnece();
            IRepository<ETag> repository = Repository.CreateETagRepository<ETag>();
            ETag etag;
            if (_dicByKey.TryGetValue(key, out etag)) {
                string oldValue = etag.Value;
                etag.Value = value;
                if (oldValue != value) {
                    _dicByValue.Remove(oldValue);
                    _dicByValue.Add(value, etag);
                }
                etag.TimeStamp = DateTime.Now;
                repository.Update(etag);
            }
            else {
                etag = new ETag() {
                    Id = Guid.NewGuid(),
                    Key = key,
                    Value = value,
                    TimeStamp = DateTime.Now
                };
                _dicById.Add(etag.Id, etag);
                _dicByKey.Add(etag.Key, etag);
                _dicByValue.Add(etag.Value, etag);
                repository.Add(etag);
            }
        }

        public bool TryGetETagByKey(string key, out IETag etag) {
            InitOnece();
            ETag entity;
            bool result = _dicByKey.TryGetValue(key, out entity);
            etag = entity;
            return result;
        }

        public bool TryGetETagByValue(string value, out IETag etag) {
            InitOnece();
            ETag entity;
            bool result = _dicByValue.TryGetValue(value, out entity);
            etag = entity;
            return result;
        }
    }
}
