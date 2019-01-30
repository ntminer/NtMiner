using LiteDB;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class KeySet : IKeySet {
        private readonly Dictionary<string, KeyData> _dicByPublicKey = new Dictionary<string, KeyData>();

        private readonly INTMinerRoot _root;
        public KeySet(INTMinerRoot root) {
            _root = root;
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
                    using (LiteDatabase db = new LiteDatabase($"filename={ClientId.LangDbFileFullName};journal=false")) {
                        var col = db.GetCollection<KeyData>();
                        foreach (var item in col.FindAll()) {
                            if (!_dicByPublicKey.ContainsKey(item.PublicKey)) {
                                _dicByPublicKey.Add(item.PublicKey, item);
                            }
                        }
                        _isInited = true;
                    }
                }
            }
        }

        public bool Contains(string publicKey) {
            InitOnece();
            return _dicByPublicKey.ContainsKey(publicKey);
        }
    }
}
