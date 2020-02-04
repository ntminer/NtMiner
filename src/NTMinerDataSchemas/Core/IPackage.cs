using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IPackage : IEntity<Guid> {
        Guid Id { get; }
        /// <summary>
        /// 名称唯一
        /// </summary>
        string Name { get; }
        List<Guid> AlgoIds { get; }
    }
}
