using System;

namespace NTMiner.Language {
    public interface ILangView : IDbEntity<Guid> {
        Guid LangId { get; }
        string Code { get; }
        int SortNumber { get; }
    }
}
