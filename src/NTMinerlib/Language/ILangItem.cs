using System;

namespace NTMiner.Language {
    public interface ILangItem : IDbEntity<Guid> {
        Guid LangId { get; }
        string ViewId { get; }
        string Key { get; }
        string Value { get; }
    }
}
