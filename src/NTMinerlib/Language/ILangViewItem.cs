using NTMiner;
using System;

namespace NTMiner.Language {
    public interface ILangViewItem : IDbEntity<Guid> {
        Guid LangId { get; }
        string ViewId { get; }
        string Key { get; }
        string Value { get; }
    }
}
