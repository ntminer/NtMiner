using System;

namespace NTMiner.Language {
    public interface ILangViewItem : IEntity<Guid> {
        Guid LangId { get; }
        string ViewId { get; }
        string Key { get; }
        string Value { get; }
    }
}
