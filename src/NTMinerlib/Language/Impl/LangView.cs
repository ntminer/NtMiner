using System;

namespace NTMiner.Language.Impl {
    public class LangView : ILangView {
        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public Guid LangId { get; set; }

        public string Code { get; set; }

        public int SortNumber { get; set; }
    }
}
