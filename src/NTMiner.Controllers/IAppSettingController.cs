using NTMiner.Core;
using NTMiner.Core.MinerServer;
using NTMiner.ServerNode;
using System;

namespace NTMiner.Controllers {
    public interface IAppSettingController {
        DateTime GetTime();
        /// <summary>
        /// post {'Key':"server2.0.0.json"}
        /// result "2020-06-10 15:46:38 683|2.8.2.0|1592227274|1591774936|0|Online"
        /// <see cref="ServerNode.ServerStateResponse.ToLine()"/>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        string GetJsonFileVersion(JsonFileVersionRequest request);
        ServerStateResponse GetServerState(JsonFileVersionRequest request);
        /// <summary>
        /// 需签名
        /// </summary>
        ResponseBase SetAppSetting(DataRequest<AppSettingData> request);
    }
}
