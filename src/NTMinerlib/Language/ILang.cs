using System;

namespace NTMiner.Language {
    public interface ILang : IEntity<Guid> {
        string Name { get; }
        string Code { get; }
        int SortNumber { get; }
    }
}
