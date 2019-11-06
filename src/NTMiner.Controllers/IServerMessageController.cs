using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface IServerMessageController {
        ResponseBase AddOrUpdateServerMessage(DataRequest<ServerMessageData> request);
        ResponseBase RemoveServerMessage(DataRequest<Guid> request);
        DataResponse<List<ServerMessageData>> ServerMessages(ServerMessagesRequest request);
    }
}
