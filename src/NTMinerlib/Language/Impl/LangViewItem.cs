using System;

namespace NTMiner.Language.Impl {
    public class LangViewItem : ILangViewItem, IDbEntity<Guid> {
        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public Guid LangId { get; set; }

        public string ViewId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }
}
