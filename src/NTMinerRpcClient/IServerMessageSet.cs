using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner {
    // 服务器消息集合为什么放在Rpc里？因为服务器消息集合是存储在
    // 服务器端的，管理员是通过Rpc调用管理服务器消息集合的。
    public interface IServerMessageSet {
        List<ServerMessageData> GetServerMessages(DateTime timeStamp);
        IEnumerable<IServerMessage> AsEnumerable();
    }
}
