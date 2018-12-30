using System;

namespace NTMiner.Language.Impl {
    public class Lang : ILang {
        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }
    }
}
