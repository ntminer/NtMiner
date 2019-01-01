using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Language.Impl {
    public class LangJson {
        public LangJson() {
            this.Langs = new List<Lang>();
            this.LangItems = new List<LangViewItem>();
        }

        public bool Exists<T>(Guid key) where T : IDbEntity<Guid> {
            return GetAll<T>().Any(a => a.GetId() == key);
        }

        public T GetByKey<T>(Guid key) where T : IDbEntity<Guid> {
            return GetAll<T>().FirstOrDefault(a => a.GetId() == key);
        }

        public IList<T> GetAll<T>() where T : IDbEntity<Guid> {
            string typeName = typeof(T).Name;
            switch (typeName) {
                case nameof(Lang):
                    return (IList<T>)this.Langs;
                case nameof(LangViewItem):
                    return (IList<T>)this.LangItems;
                default:
                    return new List<T>();
            }
        }

        public ulong TimeStamp { get; set; }

        public List<Lang> Langs { get; set; }
        public List<LangViewItem> LangItems { get; set; }
    }
}
