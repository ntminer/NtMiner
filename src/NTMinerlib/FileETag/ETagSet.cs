using LiteDB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NTMiner.FileETag {
    public class ETagSet {
        public static readonly ETagSet Instance = new ETagSet();

        // 注意ETag的key是区分大小写的，因为云服务器往往是linux，linux的文件名区分大小写，而ETag的key是文件名
        private readonly Dictionary<string, ETag> _dicByKey = new Dictionary<string, ETag>();
        private readonly Dictionary<string, ETag> _dicByValue = new Dictionary<string, ETag>();

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
                    foreach (var item in GetAllETags()) {
                        if (!_dicByKey.ContainsKey(item.Key)) {
                            _dicByKey.Add(item.Key, item);
                        }
                        if (!_dicByValue.ContainsKey(item.Value)) {
                            _dicByValue.Add(item.Value, item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public void AddOrUpdateETag(string key, string value) {
            InitOnece();
            ETag etag;
            if (_dicByKey.TryGetValue(key, out etag)) {
                string oldValue = etag.Value;
                etag.Value = value;
                if (oldValue != value) {
                    _dicByValue.Remove(oldValue);
                    _dicByValue.Add(value, etag);
                }
                etag.TimeStamp = DateTime.Now;
                UpdateETag(etag);
            }
            else {
                etag = new ETag() {
                    Key = key,
                    Value = value,
                    TimeStamp = DateTime.Now
                };
                _dicByKey.Add(etag.Key, etag);
                _dicByValue.Add(etag.Value, etag);
                AddETag(etag);
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

        private readonly string _localDbFileFullName = Path.Combine(ClientId.GlobalDirFullName, "local.litedb");

        private List<ETag> GetAllETags() {
            using (LiteDatabase db = new LiteDatabase($"filename={_localDbFileFullName};journal=false")) {
                return db.GetCollection<ETag>().FindAll().ToList();
            }
        }

        private void AddETag(ETag etag) {
            using (LiteDatabase db = new LiteDatabase($"filename={_localDbFileFullName};journal=false")) {
                var col = db.GetCollection<ETag>();
                col.Insert(etag);
            }
        }

        private void UpdateETag(ETag etag) {
            using (LiteDatabase db = new LiteDatabase($"filename={_localDbFileFullName};journal=false")) {
                var col = db.GetCollection<ETag>();
                col.Update(etag);
            }
        }
    }
}
