using NTMiner.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Controllers {
    public interface IServerMessageController {
        /// <summary>
        /// 注意：更新时如果没有更新Timestamp服务器自动将Timestamp更新为当前时间
        /// </summary>
        ResponseBase AddOrUpdateServerMessage(DataRequest<ServerMessageData> request);
        /// <summary>
        /// 注意：此为标记删除，效果是将IsDeleted更新为true并更新Timestamp为当前时间。
        /// </summary>
        ResponseBase MarkDeleteServerMessage(DataRequest<Guid> request);
        DataResponse<List<ServerMessageData>> ServerMessages(ServerMessagesRequest request);
    }
}
