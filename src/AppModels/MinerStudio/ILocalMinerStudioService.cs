using System;
using System.Collections.Generic;

namespace NTMiner.MinerStudio {
    public interface ILocalMinerStudioService : IMinerStudioService {
        void AddClientsAsync(List<string> clientIps, Action<ResponseBase, Exception> callback);
    }
}
