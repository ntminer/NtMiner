using Aliyun.OSS;
using NTMiner.Data;
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
        IClientSet ClientSet { get; }
        ICoinSnapshotSet CoinSnapshotSet { get; }
        IMineWorkSet MineWorkSet { get; }
        IMinerGroupSet MinerGroupSet { get; }
        IPoolSet PoolSet { get; }
        IWalletSet WalletSet { get; }
        // ReSharper disable once InconsistentNaming
        INTMinerFileSet NTMinerFileSet { get; }
        IOverClockDataSet OverClockDataSet { get; }
    }
}
