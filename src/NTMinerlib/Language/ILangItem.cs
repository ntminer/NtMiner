using System;

namespace NTMiner.Language {
    public interface ILangItem : IDbEntity<Guid> {
        string LangCode { get; }
        string ViewId { get; }
        string Key { get; }
        string Value { get; }
    }
}
