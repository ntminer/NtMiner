using System;

namespace NTMiner.Language {
    public interface ILang : IDbEntity<Guid> {
        string Name { get; }
        string Code { get; }
    }
}
