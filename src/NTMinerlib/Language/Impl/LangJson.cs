using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NTMiner.Language.Impl {
    public class LangJson {
        public static readonly LangJson Instance = new LangJson();

        public static string Export() {
            LangJson data = new LangJson() {
                Langs = LangSet.Instance.Cast<Lang>().ToArray(),
                LangViewItems = LangViewItemSet.Instance.Cast<LangViewItem>().ToArray()
            };
            string json = VirtualRoot.JsonSerializer.Serialize(data);
            File.WriteAllText(AssemblyInfo.LangVersionJsonFileFullName, json);
            return Path.GetFileName(AssemblyInfo.LangVersionJsonFileFullName);
        }

        // 私有构造函数不影响序列化反序列化
        private LangJson() {
            this.Langs = new Lang[0];
            this.LangViewItems = new LangViewItem[0];
        }

        private readonly object _locker = new object();
        private bool _inited = false;
        public void Init(string rawJson) {
            if (!_inited) {
                lock (_locker) {
                    if (!_inited) {
                        if (!string.IsNullOrEmpty(rawJson)) {
                            try {
                                LangJson data = VirtualRoot.JsonSerializer.Deserialize<LangJson>(rawJson);
                                this.Langs = data.Langs ?? new Lang[0];
                                this.LangViewItems = data.LangViewItems ?? new LangViewItem[0];
                                ClientId.WriteLocalLangJsonFile(rawJson);
                            }
                            catch (Exception e) {
                                Logger.ErrorDebugLine(e.Message, e);
                            }
                        }
                        _inited = true;
                    }
                }
            }
        }

        public void ReInit(string rawJson) {
            _inited = false;
            Init(rawJson);
        }

        public bool Exists<T>(Guid key) where T : IDbEntity<Guid> {
            return GetAll<T>().Any(a => a.GetId() == key);
        }

        public T GetByKey<T>(Guid key) where T : IDbEntity<Guid> {
            return GetAll<T>().FirstOrDefault(a => a.GetId() == key);
        }

        public IEnumerable<T> GetAll<T>() where T : IDbEntity<Guid> {
            string typeName = typeof(T).Name;
            switch (typeName) {
                case nameof(Lang):
                    return this.Langs.Cast<T>();
                case nameof(LangViewItem):
                    return this.LangViewItems.Cast<T>();
                default:
                    return new List<T>();
            }
        }

        public ulong TimeStamp { get; set; }

        public Lang[] Langs { get; set; }
        public LangViewItem[] LangViewItems { get; set; }
    }
}
