using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NTMiner.Language.Impl {
    public class LangJson {
        public static readonly LangJson Instance;

        static LangJson() {
            string langJsonFileFullName = Path.Combine(Global.GlobalDirFullName, "lang.json");
            if (!File.Exists(langJsonFileFullName)) {
                Instance = new LangJson();
            }
            else {
                Instance = Global.JsonSerializer.Deserialize<LangJson>(File.ReadAllText(langJsonFileFullName));
            }
        }

        public static void Export() {
            LangJson data = new LangJson() {
                Langs = LangSet.Instance.Cast<Lang>().ToArray(),
                LangViewItems = LangViewItemSet.Instance.Cast<LangViewItem>().ToArray()
            };
            string json = Global.JsonSerializer.Serialize(data);
            File.WriteAllText(Global.ServerLangJsonFileFullName, json);
        }

        public LangJson() {
            this.Langs = new Lang[0];
            this.LangViewItems = new LangViewItem[0];
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
