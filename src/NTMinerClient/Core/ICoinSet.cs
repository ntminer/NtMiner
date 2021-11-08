using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface ICoinSet: ICountSet {
        bool Contains(string coinCode);
        bool Contains(Guid coinId);
        bool TryGetCoin(string coinCode, out ICoin coin);
        bool TryGetCoin(Guid coinId, out ICoin coin);
        IEnumerable<ICoin> AsEnumerable();
    }
}
