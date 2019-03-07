using System;

namespace NTMiner.Language.Impl {
    public class Lang : ILang, IDbEntity<Guid> {
        public static readonly ILang Empty = new Lang {
            Id = Guid.Empty,
            Name = string.Empty,
            Code = string.Empty
        };

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public int SortNumber { get; set; }
    }
}
