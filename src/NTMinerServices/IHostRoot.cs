using Aliyun.OSS;
using NTMiner.AppSetting;
using NTMiner.Data;
using NTMiner.KernelOutputKeyword;
using NTMiner.ServerMessage;
using NTMiner.User;
using System;

namespace NTMiner {
    public interface IHostRoot {
        DateTime StartedOn { get; }

        IHostConfig HostConfig { get; }

        OssClient OssClient { get; }

        IUserSet UserSet { get; }
        IAppSettingSet AppSettingSet { get; }
        ICalcConfigSet CalcConfigSet { get; }
        IColumnsShowSet ColumnsShowSet { get; }
        INTMinerWalletSet NTMinerWalletSet { get; }
        IClientSet ClientSet { get; }
        ICoinSnapshotSet CoinSnapshotSet { get; }
        IMineWorkSet MineWorkSet { get; }
        IMinerGroupSet MinerGroupSet { get; }
        IPoolSet PoolSet { get; }
        IWalletSet WalletSet { get; }
        INTMinerFileSet NTMinerFileSet { get; }
        IOverClockDataSet OverClockDataSet { get; }
        IKernelOutputKeywordSet KernelOutputKeywordSet { get; }
        IServerMessageSet ServerMessageSet { get; }
        DateTime ServerMessageTimestamp { get; }
        DateTime KernelOutputKeywordTimestamp { get; }
        void UpdateServerMessageTimestamp();
    }
}
