using NTMiner.Core;
using NTMiner.Core.MinerClient;
using NTMiner.Core.MinerServer;
using NTMiner.Core.MinerStudio;
using NTMiner.Core.Profile;
using NTMiner.Gpus;
using NTMiner.Hub;
using NTMiner.Mine;
using NTMiner.User;
using NTMiner.Ws;
using System;

namespace NTMiner {
    [MessageType(description: "释放并执行挖矿端嵌入的工具")]
    public class MinerClientActionCommand : Cmd {
        public MinerClientActionCommand(MinerClientActionType actionType) {
            this.ActionType = actionType;
        }

        public MinerClientActionType ActionType { get; private set; }
    }

    [MessageType(description: "停止挖矿")]
    public class StopMineCommand : Cmd {
        public StopMineCommand() { }
    }

    [MessageType(description: "打开windows重启倒计时界面")]
    public class ShowRestartWindowsCommand : Cmd {
        public ShowRestartWindowsCommand(int countDownSeconds) {
            this.CountDownSeconds = countDownSeconds;
        }

        public int CountDownSeconds { get; private set; }
    }

    [MessageType(description: "打开内核下载界面")]
    public class ShowKernelDownloaderCommand : Cmd {
        public ShowKernelDownloaderCommand(Guid kernelId, Action<bool, string> downloadComplete) {
            this.KernelId = kernelId;
            this.DownloadComplete = downloadComplete;
        }

        public Guid KernelId { get; private set; }
        public Action<bool, string> DownloadComplete { get; private set; }
    }

    [MessageType(description: "设置开机自动挖矿")]
    public class SetAutoStartCommand : Cmd {
        public SetAutoStartCommand(bool isAutoBoot, bool isAutoStart) {
            this.IsAutoBoot = isAutoBoot;
            this.IsAutoStart = isAutoStart;
        }

        public bool IsAutoBoot { get; private set; }
        public bool IsAutoStart { get; private set; }
    }

    [MessageType(description: "ServerContext重新初始化后")]
    public class ServerContextReInitedEvent : EventBase {
        public ServerContextReInitedEvent() { }
    }

    [MessageType(description: "LocalContext重新初始化后")]
    public class LocalContextReInitedEvent : EventBase {
        public LocalContextReInitedEvent() { }
    }

    [MessageType(description: "处理了LocalContext重新初始化事件后")]
    public class LocalContextReInitedEventHandledEvent : EventBase {
        public LocalContextReInitedEventHandledEvent() { }
    }

    [MessageType(description: "发现了NTMiner或MinerStudio新版本")]
    public class AppVersionChangedEvent : EventBase {
        /// <summary>
        /// 如果给定的服务器版本比本地版本高则发布AppVersionChangedEvent事件
        /// </summary>
        /// <param name="serverVersion"></param>
        public static void PublishIfNewVersion(string serverVersion) {
            if (!string.IsNullOrEmpty(serverVersion) && serverVersion != EntryAssemblyInfo.CurrentVersionStr) {
                if (Version.TryParse(serverVersion, out Version v)) {
                    if (v > EntryAssemblyInfo.CurrentVersion) {
                        NTMinerContext.SetServerVersion(v);
                        VirtualRoot.RaiseEvent(new AppVersionChangedEvent());
                    }
                }
            }
        }

        public AppVersionChangedEvent() { }
    }

    [MessageType(description: "内核自我重启")]
    public class KernelSelfRestartedEvent : EventBase {
        public KernelSelfRestartedEvent() { }
    }

    [MessageType(description: "开始挖矿")]
    public class StartingMineEvent : EventBase {
        public StartingMineEvent() {
        }
    }

    [MessageType(description: "开始挖矿不成功")]
    public class StartingMineFailedEvent : EventBase {
        public StartingMineFailedEvent(string message) {
            this.Message = message;
        }

        public string Message { get; private set; }
    }

    [MessageType(description: "挖矿开始事件")]
    public class MineStartedEvent : EventBase {
        public MineStartedEvent(IMineContext mineContext) {
            this.MineContext = mineContext;
        }
        public IMineContext MineContext { get; private set; }
    }

    [MessageType(description: "挖矿上下文发生了变更")]
    public class CurrentMineContextChangedEvent : EventBase {
        public CurrentMineContextChangedEvent() { }
    }

    [MessageType(description: "挖矿停止事件")]
    public class MineStopedEvent : EventBase {
        public MineStopedEvent(IMineContext mineContext, StopMineReason stopReason) {
            this.MineContext = mineContext;
            this.StopReason = stopReason;
        }
        public IMineContext MineContext { get; private set; }
        public StopMineReason StopReason { get; private set; }
    }

    [MessageType(description: "关闭NTMiner客户端")]
    // ReSharper disable once InconsistentNaming
    public class CloseNTMinerCommand : Cmd {
        public CloseNTMinerCommand(string reason) {
            this.Reason = reason;
        }

        public string Reason { get; private set; }
    }

    [MessageType(description: "刷新开机启动和自动挖矿")]
    public class RefreshAutoBootStartCommand : Cmd {
        public RefreshAutoBootStartCommand() { }
    }

    [MessageType(description: "开机启动和自动挖矿刷新后")]
    public class AutoBootStartRefreshedEvent : EventBase {
        public AutoBootStartRefreshedEvent() { }
    }

    [MessageType(description: "刷新主界面状态栏的windows远程桌面图标颜色")]
    public class RefreshIsRemoteDesktopEnabledCommand : Cmd {
        public RefreshIsRemoteDesktopEnabledCommand() {
        }
    }

    [MessageType(description: "更新挖矿端界面上显示的外网群控状态")]
    public class RefreshWsStateCommand : Cmd {
        public RefreshWsStateCommand(WsClientState wsClientState) {
            this.WsClientState = wsClientState;
        }

        public WsClientState WsClientState { get; private set; }
    }

    [MessageType(description: "发现服务端server.json文件版本变更后")]
    public class ServerJsonVersionChangedEvent : EventBase {
        public ServerJsonVersionChangedEvent(string oldVersion, string newVersion) {
            this.OldVersion = oldVersion;
            this.NewVersion = newVersion;
        }

        public string OldVersion { get; private set; }
        public string NewVersion { get; private set; }
        public bool IsChagned {
            get {
                return OldVersion != NewVersion;
            }
        }
    }

    [MessageType(description: "电价变更后")]
    public class EPriceChangedEvent : EventBase {
        public EPriceChangedEvent() {
        }
    }

    [MessageType(description: "功耗补偿变更后")]
    public class PowerAppendChangedEvent : EventBase {
        public PowerAppendChangedEvent() { }
    }

    [MessageType(description: "高温红色阈值变更后")]
    public class MaxTempChangedEvent : EventBase {
        public MaxTempChangedEvent() { }
    }

    [MessageType(description: "从内核输出中提取了矿池延时")]
    public class PoolDelayPickedEvent : EventBase {
        public PoolDelayPickedEvent(Guid poolId, bool isDual, string poolDelay) {
            this.PoolDelayText = poolDelay;
            this.PoolId = poolId;
            this.IsDual = isDual;
        }

        public Guid PoolId { get; private set; }
        public bool IsDual { get; private set; }
        /// <summary>
        /// 矿池延时只用于展示，没有其它用户所以是字符串
        /// </summary>
        public string PoolDelayText { get; private set; }
    }

    [MessageType(description: "CPU包的状态发生了变更")]
    public class CpuPackageStateChangedEvent : EventBase {
        public CpuPackageStateChangedEvent() {
        }
    }

    #region toolbox
    [MessageType(description: "禁用win10系统更新")]
    public class BlockWAUCommand : Cmd {
        public BlockWAUCommand() { }
    }

    [MessageType(description: "优化window10")]
    public class Win10OptimizeCommand : Cmd {
        public Win10OptimizeCommand() { }
    }

    [MessageType(description: "开起A卡计算模式")]
    public class SwitchRadeonGpuCommand : Cmd {
        public SwitchRadeonGpuCommand(bool on) {
            this.On = on;
        }

        public bool On { get; private set; }
    }

    /// <summary>
    /// 挖矿端主界面下面有一层原版控制台窗口，当点击主界面上的menu下拉列表或者popup时下面
    /// 的层可能跑到上面来，配合使用这对TopMostCommand和UnTopMostCommand可以解决这个问题。
    /// </summary>
    [MessageType(description: "设置主界面的Topmost")]
    public class TopmostCommand : Cmd {
        public TopmostCommand() {
        }
    }

    [MessageType(description: "取消主界面的Topmost")]
    public class UnTopmostCommand : Cmd {
        public UnTopmostCommand() {
        }
    }

    [MessageType(description: "A卡驱动签名")]
    public class AtikmdagPatcherCommand : Cmd {
        public AtikmdagPatcherCommand() {
        }
    }

    [MessageType(description: "注册右键打开windows命令行菜单")]
    public class RegCmdHereCommand : Cmd {
        public RegCmdHereCommand() { }
    }

    [MessageType(description: "移除右键打开windows命令行菜单")]
    public class UnRegCmdHereCommand : Cmd {
        public UnRegCmdHereCommand() { }
    }
    #endregion

    #region profile Messages
    [MessageType(description: "MinerProfile设置变更后")]
    public class MinerProfilePropertyChangedEvent : EventBase {
        public MinerProfilePropertyChangedEvent(string propertyName) {
            this.PropertyName = propertyName;
        }

        public string PropertyName { get; private set; }
    }

    [MessageType(description: "挖矿币种级设置变更后")]
    public class CoinProfilePropertyChangedEvent : EventBase {
        public CoinProfilePropertyChangedEvent(Guid coinId, string propertyName) {
            this.CoinId = coinId;
            this.PropertyName = propertyName;
        }

        public Guid CoinId { get; }

        public string PropertyName { get; private set; }
    }

    [MessageType(description: "矿池级设置变更后")]
    public class PoolProfilePropertyChangedEvent : EventBase {
        public PoolProfilePropertyChangedEvent(Guid coinId, string propertyName) {
            this.PoolId = coinId;
            this.PropertyName = propertyName;
        }

        public Guid PoolId { get; }

        public string PropertyName { get; private set; }
    }

    [MessageType(description: "挖矿币种内核级设置变更后")]
    public class CoinKernelProfilePropertyChangedEvent : EventBase {
        public CoinKernelProfilePropertyChangedEvent(Guid coinKernelId, string propertyName) {
            this.CoinKernelId = coinKernelId;
            this.PropertyName = propertyName;
        }

        public Guid CoinKernelId { get; }

        public string PropertyName { get; private set; }
    }

    [MessageType(description: "Gpu超频集合刷新后")]
    public class GpuProfileSetRefreshedEvent : EventBase {
        public GpuProfileSetRefreshedEvent() { }
    }
    #endregion

    #region NTMinerWallet Messages
    [MessageType(description: "添加NTMiner钱包")]
    public class AddNTMinerWalletCommand : AddEntityCommand<INTMinerWallet> {
        public AddNTMinerWalletCommand(INTMinerWallet input) : base(input) {
        }
    }

    [MessageType(description: "更新NTMiner钱包")]
    public class UpdateNTMinerWalletCommand : UpdateEntityCommand<INTMinerWallet> {
        public UpdateNTMinerWalletCommand(INTMinerWallet input) : base(input) {
        }
    }

    [MessageType(description: "删除NTMiner钱包")]
    public class RemoveNTMinerWalletCommand : RemoveEntityCommand {
        public RemoveNTMinerWalletCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "添加NTMiner钱包后")]
    public class NTMinerWalletAddedEvent : SourcedEvent<INTMinerWallet> {
        public NTMinerWalletAddedEvent(PathId targetPathId, INTMinerWallet source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "更新NTMiner钱包后")]
    public class NTMinerWalletUpdatedEvent : SourcedEvent<INTMinerWallet> {
        public NTMinerWalletUpdatedEvent(PathId targetPathId, INTMinerWallet source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "删除NTMiner钱包后")]
    public class NTMinerWalletRemovedEvent : SourcedEvent<INTMinerWallet> {
        public NTMinerWalletRemovedEvent(PathId targetPathId, INTMinerWallet source) : base(targetPathId, source) {
        }
    }
    #endregion

    #region OverClockData Messages
    [MessageType(description: "添加超频菜谱")]
    public class AddOverClockDataCommand : AddEntityCommand<IOverClockData> {
        public AddOverClockDataCommand(IOverClockData input) : base(input) {
        }
    }

    [MessageType(description: "更新超频菜谱")]
    public class UpdateOverClockDataCommand : UpdateEntityCommand<IOverClockData> {
        public UpdateOverClockDataCommand(IOverClockData input) : base(input) {
        }
    }

    [MessageType(description: "删除超频菜谱")]
    public class RemoveOverClockDataCommand : RemoveEntityCommand {
        public RemoveOverClockDataCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "添加超频菜谱后")]
    public class OverClockDataAddedEvent : SourcedEvent<IOverClockData> {
        public OverClockDataAddedEvent(PathId targetPathId, IOverClockData source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "更新超频菜谱后")]
    public class OverClockDataUpdatedEvent : SourcedEvent<IOverClockData> {
        public OverClockDataUpdatedEvent(PathId targetPathId, IOverClockData source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "删除超频菜谱后")]
    public class OverClockDataRemovedEvent : SourcedEvent<IOverClockData> {
        public OverClockDataRemovedEvent(PathId targetPathId, IOverClockData source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "超频菜谱集初始化后")]
    public class OverClockDataSetInitedEvent : EventBase {
        public OverClockDataSetInitedEvent() {
        }
    }

    [MessageType(description: "NTMiner钱包集初始化后")]
    public class NTMinerWalletSetInitedEvent : EventBase {
        public NTMinerWalletSetInitedEvent() {
        }
    }
    #endregion

    #region gpu overclock
    [MessageType(description: "添加或更新Gpu超频数据")]
    public class AddOrUpdateGpuProfileCommand : Cmd {
        public AddOrUpdateGpuProfileCommand(IGpuProfile input) {
            this.Input = input;
        }

        public IGpuProfile Input { get; private set; }
    }

    [MessageType(description: "币种超频")]
    public class CoinOverClockCommand : Cmd {
        public CoinOverClockCommand(Guid coinId) {
            this.CoinId = coinId;
        }

        public Guid CoinId { get; private set; }
    }

    [MessageType(description: "币种超频完成后")]
    public class CoinOverClockDoneEvent : EventBase {
        public CoinOverClockDoneEvent(PathId targetPathId) : base(targetPathId) {
        }
    }

    [MessageType(description: "Gpu超频数据添加或更新后")]
    public class GpuProfileAddedOrUpdatedEvent : SourcedEvent<IGpuProfile> {
        public GpuProfileAddedOrUpdatedEvent(PathId targetPathId, IGpuProfile source) : base(targetPathId, source) {
        }
    }
    #endregion

    #region speed and share
    [MessageType(description: "显卡算力变更事件")]
    public class GpuSpeedChangedEvent : SourcedEvent<IGpuSpeed> {
        public GpuSpeedChangedEvent(bool isDual, PathId targetPathId, IGpuSpeed gpuSpeed) : base(targetPathId, gpuSpeed) {
            this.IsDual = isDual;
        }

        public bool IsDual { get; private set; }
    }

    [MessageType(description: "显卡份额变更事件")]
    public class GpuShareChangedEvent : SourcedEvent<IGpuSpeed> {
        public GpuShareChangedEvent(PathId targetPathId, IGpuSpeed gpuSpeed) : base(targetPathId, gpuSpeed) {
        }
    }

    [MessageType(description: "显卡找到了一个份额")]
    public class FoundShareIncreasedEvent : SourcedEvent<IGpuSpeed> {
        public FoundShareIncreasedEvent(PathId targetPathId, IGpuSpeed gpuSpeed) : base(targetPathId, gpuSpeed) {
        }
    }

    [MessageType(description: "显卡接受了一个份额")]
    public class AcceptShareIncreasedEvent : SourcedEvent<IGpuSpeed> {
        public AcceptShareIncreasedEvent(PathId targetPathId, IGpuSpeed gpuSpeed) : base(targetPathId, gpuSpeed) {
        }
    }

    [MessageType(description: "设置了显卡接受份额")]
    public class AcceptShareSetedEvent : SourcedEvent<IGpuSpeed> {
        public AcceptShareSetedEvent(PathId targetPathId, IGpuSpeed gpuSpeed) : base(targetPathId, gpuSpeed) {
        }
    }

    [MessageType(description: "设置了显卡拒绝份额")]
    public class RejectShareSetedEvent : SourcedEvent<IGpuSpeed> {
        public RejectShareSetedEvent(PathId targetPathId, IGpuSpeed gpuSpeed) : base(targetPathId, gpuSpeed) {
        }
    }

    [MessageType(description: "设置了显卡计算错误份额")]
    public class IncorrectShareSetedEvent : SourcedEvent<IGpuSpeed> {
        public IncorrectShareSetedEvent(PathId targetPathId, IGpuSpeed gpuSpeed) : base(targetPathId, gpuSpeed) {
        }
    }

    [MessageType(description: "显卡拒绝了一个份额")]
    public class RejectShareIncreasedEvent : SourcedEvent<IGpuSpeed> {
        public RejectShareIncreasedEvent(PathId targetPathId, IGpuSpeed gpuSpeed) : base(targetPathId, gpuSpeed) {
        }
    }

    [MessageType(description: "显卡算错了一个份额")]
    public class IncorrectShareIncreasedEvent : SourcedEvent<IGpuSpeed> {
        public IncorrectShareIncreasedEvent(PathId targetPathId, IGpuSpeed gpuSpeed) : base(targetPathId, gpuSpeed) {
        }
    }

    [MessageType(description: "收益变更事件")]
    public class ShareChangedEvent : SourcedEvent<ICoinShare> {
        public ShareChangedEvent(PathId targetPathId, ICoinShare share) : base(targetPathId, share) {
        }
    }
    #endregion

    #region Gpu Messages
    [MessageType(description: "显卡状态变更事件", isCanNoPath: true)]
    public class GpuStateChangedEvent : SourcedEvent<IGpu> {
        public GpuStateChangedEvent(PathId targetPathId, IGpu source) : base(targetPathId, source) {
        }
    }
    #endregion

    #region SysDic Messages
    [MessageType(description: "添加系统字典")]
    public class AddSysDicCommand : AddEntityCommand<ISysDic> {
        public AddSysDicCommand(ISysDic input) : base(input) {
        }
    }

    [MessageType(description: "更新系统字典")]
    public class UpdateSysDicCommand : UpdateEntityCommand<ISysDic> {
        public UpdateSysDicCommand(ISysDic input) : base(input) {
        }
    }

    [MessageType(description: "删除系统字典")]
    public class RemoveSysDicCommand : RemoveEntityCommand {
        public RemoveSysDicCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "添加系统字典后")]
    public class SysDicAddedEvent : SourcedEvent<ISysDic> {
        public SysDicAddedEvent(PathId targetPathId, ISysDic source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "更新系统字典后")]
    public class SysDicUpdatedEvent : SourcedEvent<ISysDic> {
        public SysDicUpdatedEvent(PathId targetPathId, ISysDic source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "删除系统字典后")]
    public class SysDicRemovedEvent : SourcedEvent<ISysDic> {
        public SysDicRemovedEvent(PathId targetPathId, ISysDic source) : base(targetPathId, source) {
        }
    }
    #endregion

    #region SysDicItem Messages
    [MessageType(description: "添加系统字典项")]
    public class AddSysDicItemCommand : AddEntityCommand<ISysDicItem> {
        public AddSysDicItemCommand(ISysDicItem input) : base(input) {
        }
    }

    [MessageType(description: "更新系统字典项")]
    public class UpdateSysDicItemCommand : UpdateEntityCommand<ISysDicItem> {
        public UpdateSysDicItemCommand(ISysDicItem input) : base(input) {
        }
    }

    [MessageType(description: "移除系统字典项")]
    public class RemoveSysDicItemCommand : RemoveEntityCommand {
        public RemoveSysDicItemCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "添加了系统字典项后")]
    public class SysDicItemAddedEvent : SourcedEvent<ISysDicItem> {
        public SysDicItemAddedEvent(PathId targetPathId, ISysDicItem source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "更新了系统字典项后")]
    public class SysDicItemUpdatedEvent : SourcedEvent<ISysDicItem> {
        public SysDicItemUpdatedEvent(PathId targetPathId, ISysDicItem source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "删除了系统字典项后")]
    public class SysDicItemRemovedEvent : SourcedEvent<ISysDicItem> {
        public SysDicItemRemovedEvent(PathId targetPathId, ISysDicItem source) : base(targetPathId, source) {
        }
    }
    #endregion

    #region Coin Messages
    [MessageType(description: "添加币种")]
    public class AddCoinCommand : AddEntityCommand<ICoin> {
        public AddCoinCommand(ICoin input) : base(input) {
        }
    }

    [MessageType(description: "更新币种")]
    public class UpdateCoinCommand : UpdateEntityCommand<ICoin> {
        public UpdateCoinCommand(ICoin input) : base(input) {
        }
    }

    [MessageType(description: "移除币种")]
    public class RemoveCoinCommand : RemoveEntityCommand {
        public RemoveCoinCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "添加了币种后")]
    public class CoinAddedEvent : SourcedEvent<ICoin> {
        public CoinAddedEvent(PathId targetPathId, ICoin source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "更新了币种后")]
    public class CoinUpdatedEvent : SourcedEvent<ICoin> {
        public CoinUpdatedEvent(PathId targetPathId, ICoin source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "移除了币种后")]
    public class CoinRemovedEvent : SourcedEvent<ICoin> {
        public CoinRemovedEvent(PathId targetPathId, ICoin source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "下载了币种图标后")]
    public class CoinIconDownloadedEvent : SourcedEvent<ICoin> {
        public CoinIconDownloadedEvent(PathId targetPathId, ICoin source) : base(targetPathId, source) {
        }
    }
    #endregion

    #region Group Messages
    [MessageType(description: "添加组")]
    public class AddGroupCommand : AddEntityCommand<IGroup> {
        public AddGroupCommand(IGroup input) : base(input) {
        }
    }

    [MessageType(description: "更新组")]
    public class UpdateGroupCommand : UpdateEntityCommand<IGroup> {
        public UpdateGroupCommand(IGroup input) : base(input) {
        }
    }

    [MessageType(description: "移除组")]
    public class RemoveGroupCommand : RemoveEntityCommand {
        public RemoveGroupCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "添加了组后")]
    public class GroupAddedEvent : SourcedEvent<IGroup> {
        public GroupAddedEvent(PathId targetPathId, IGroup source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "更新了组后")]
    public class GroupUpdatedEvent : SourcedEvent<IGroup> {
        public GroupUpdatedEvent(PathId targetPathId, IGroup source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "移除了组后")]
    public class GroupRemovedEvent : SourcedEvent<IGroup> {
        public GroupRemovedEvent(PathId targetPathId, IGroup source) : base(targetPathId, source) {
        }
    }
    #endregion

    #region CoinGroup Messages
    [MessageType(description: "添加币组")]
    public class AddCoinGroupCommand : AddEntityCommand<ICoinGroup> {
        public AddCoinGroupCommand(ICoinGroup input) : base(input) {
        }
    }

    [MessageType(description: "修改币组")]
    public class UpdateCoinGroupCommand : UpdateEntityCommand<ICoinGroup> {
        public UpdateCoinGroupCommand(ICoinGroup input) : base(input) {
        }
    }

    [MessageType(description: "移除币组")]
    public class RemoveCoinGroupCommand : RemoveEntityCommand {
        public RemoveCoinGroupCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "添加了币组后")]
    public class CoinGroupAddedEvent : SourcedEvent<ICoinGroup> {
        public CoinGroupAddedEvent(PathId targetPathId, ICoinGroup source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "移除了币组后")]
    public class CoinGroupRemovedEvent : SourcedEvent<ICoinGroup> {
        public CoinGroupRemovedEvent(PathId targetPathId, ICoinGroup source) : base(targetPathId, source) {
        }
    }
    #endregion

    #region FileWriter Messages
    [MessageType(description: "添加文件书写器")]
    public class AddFileWriterCommand : AddEntityCommand<IFileWriter> {
        public AddFileWriterCommand(IFileWriter input) : base(input) {
        }
    }

    [MessageType(description: "更新文件书写器")]
    public class UpdateFileWriterCommand : UpdateEntityCommand<IFileWriter> {
        public UpdateFileWriterCommand(IFileWriter input) : base(input) {
        }
    }

    [MessageType(description: "移除文件书写器")]
    public class RemoveFileWriterCommand : RemoveEntityCommand {
        public RemoveFileWriterCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "添加了文件书写器后")]
    public class FileWriterAddedEvent : SourcedEvent<IFileWriter> {
        public FileWriterAddedEvent(PathId targetPathId, IFileWriter source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "更新了文件书写器后")]
    public class FileWriterUpdatedEvent : SourcedEvent<IFileWriter> {
        public FileWriterUpdatedEvent(PathId targetPathId, IFileWriter source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "移除了文件书写器后")]
    public class FileWriterRemovedEvent : SourcedEvent<IFileWriter> {
        public FileWriterRemovedEvent(PathId targetPathId, IFileWriter source) : base(targetPathId, source) {
        }
    }
    #endregion

    #region FragmentWriter Messages
    [MessageType(description: "添加命令行片段书写器")]
    public class AddFragmentWriterCommand : AddEntityCommand<IFragmentWriter> {
        public AddFragmentWriterCommand(IFragmentWriter input) : base(input) {
        }
    }

    [MessageType(description: "更新命令行片段书写器")]
    public class UpdateFragmentWriterCommand : UpdateEntityCommand<IFragmentWriter> {
        public UpdateFragmentWriterCommand(IFragmentWriter input) : base(input) {
        }
    }

    [MessageType(description: "移除命令行片段书写器")]
    public class RemoveFragmentWriterCommand : RemoveEntityCommand {
        public RemoveFragmentWriterCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "添加了命令行片段书写器后")]
    public class FragmentWriterAddedEvent : SourcedEvent<IFragmentWriter> {
        public FragmentWriterAddedEvent(PathId targetPathId, IFragmentWriter source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "更新了命令行片段书写器后")]
    public class FragmentWriterUpdatedEvent : SourcedEvent<IFragmentWriter> {
        public FragmentWriterUpdatedEvent(PathId targetPathId, IFragmentWriter source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "移除了命令行片段书写器后")]
    public class FragmentWriterRemovedEvent : SourcedEvent<IFragmentWriter> {
        public FragmentWriterRemovedEvent(PathId targetPathId, IFragmentWriter source) : base(targetPathId, source) {
        }
    }
    #endregion

    #region Wallet Messages
    [MessageType(description: "添加钱包")]
    public class AddWalletCommand : AddEntityCommand<IWallet> {
        public AddWalletCommand(IWallet input) : base(input) {
        }
    }

    [MessageType(description: "更新钱包")]
    public class UpdateWalletCommand : UpdateEntityCommand<IWallet> {
        public UpdateWalletCommand(IWallet input) : base(input) {
        }
    }

    [MessageType(description: "移除钱包")]
    public class RemoveWalletCommand : RemoveEntityCommand {
        public RemoveWalletCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "添加了钱包后")]
    public class WalletAddedEvent : SourcedEvent<IWallet> {
        public WalletAddedEvent(PathId targetPathId, IWallet source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "更新了钱包后")]
    public class WalletUpdatedEvent : SourcedEvent<IWallet> {
        public WalletUpdatedEvent(PathId targetPathId, IWallet source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "移除了钱包后")]
    public class WalletRemovedEvent : SourcedEvent<IWallet> {
        public WalletRemovedEvent(PathId targetPathId, IWallet source) : base(targetPathId, source) {
        }
    }
    #endregion

    #region Pool Messages
    [MessageType(description: "添加矿池")]
    public class AddPoolCommand : AddEntityCommand<IPool> {
        public AddPoolCommand(IPool input) : base(input) {
        }
    }

    [MessageType(description: "更新矿池")]
    public class UpdatePoolCommand : UpdateEntityCommand<IPool> {
        public UpdatePoolCommand(IPool input) : base(input) {
        }
    }

    [MessageType(description: "移除矿池")]
    public class RemovePoolCommand : RemoveEntityCommand {
        public RemovePoolCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "添加了矿池后")]
    public class PoolAddedEvent : SourcedEvent<IPool> {
        public PoolAddedEvent(PathId targetPathId, IPool source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "更新了矿池后")]
    public class PoolUpdatedEvent : SourcedEvent<IPool> {
        public PoolUpdatedEvent(PathId targetPathId, IPool source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "移除了矿池后")]
    public class PoolRemovedEvent : SourcedEvent<IPool> {
        public PoolRemovedEvent(PathId targetPathId, IPool source) : base(targetPathId, source) {
        }
    }
    #endregion

    #region CoinKernel Messages
    [MessageType(description: "添加币种级内核")]
    public class AddCoinKernelCommand : AddEntityCommand<ICoinKernel> {
        public AddCoinKernelCommand(ICoinKernel input) : base(input) {
        }
    }

    [MessageType(description: "更新币种级内核")]
    public class UpdateCoinKernelCommand : UpdateEntityCommand<ICoinKernel> {
        public UpdateCoinKernelCommand(ICoinKernel input) : base(input) {
        }
    }

    [MessageType(description: "移除币种级内核")]
    public class RemoveCoinKernelCommand : RemoveEntityCommand {
        public RemoveCoinKernelCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "添加了币种级内核后")]
    public class CoinKernelAddedEvent : SourcedEvent<ICoinKernel> {
        public CoinKernelAddedEvent(PathId targetPathId, ICoinKernel source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "更新了币种级内核后")]
    public class CoinKernelUpdatedEvent : SourcedEvent<ICoinKernel> {
        public CoinKernelUpdatedEvent(PathId targetPathId, ICoinKernel source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "移除了币种级内核后")]
    public class CoinKernelRemovedEvent : SourcedEvent<ICoinKernel> {
        public CoinKernelRemovedEvent(PathId targetPathId, ICoinKernel source) : base(targetPathId, source) {
        }
    }
    #endregion

    #region PoolKernel Messages
    [MessageType(description: "添加矿池级内核")]
    public class AddPoolKernelCommand : UpdateEntityCommand<IPoolKernel> {
        public AddPoolKernelCommand(IPoolKernel input) : base(input) {
        }
    }

    [MessageType(description: "更新矿池级内核")]
    public class UpdatePoolKernelCommand : UpdateEntityCommand<IPoolKernel> {
        public UpdatePoolKernelCommand(IPoolKernel input) : base(input) {
        }
    }

    [MessageType(description: "移除矿池级内核")]
    public class RemovePoolKernelCommand : RemoveEntityCommand {
        public RemovePoolKernelCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "添加了矿池级内核后")]
    public class PoolKernelAddedEvent : SourcedEvent<IPoolKernel> {
        public PoolKernelAddedEvent(PathId targetPathId, IPoolKernel source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "更新了矿池级内核后")]
    public class PoolKernelUpdatedEvent : SourcedEvent<IPoolKernel> {
        public PoolKernelUpdatedEvent(PathId targetPathId, IPoolKernel source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "移除了矿池级内核后")]
    public class PoolKernelRemovedEvent : SourcedEvent<IPoolKernel> {
        public PoolKernelRemovedEvent(PathId targetPathId, IPoolKernel source) : base(targetPathId, source) {
        }
    }
    #endregion

    #region Package Messages
    [MessageType(description: "添加包")]
    public class AddPackageCommand : AddEntityCommand<IPackage> {
        public AddPackageCommand(IPackage input) : base(input) {
        }
    }

    [MessageType(description: "更新包")]
    public class UpdatePackageCommand : UpdateEntityCommand<IPackage> {
        public UpdatePackageCommand(IPackage input) : base(input) {
        }
    }

    [MessageType(description: "删除包")]
    public class RemovePackageCommand : RemoveEntityCommand {
        public RemovePackageCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "添加了包后")]
    public class PackageAddedEvent : SourcedEvent<IPackage> {
        public PackageAddedEvent(PathId targetPathId, IPackage source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "更新了包后")]
    public class PackageUpdatedEvent : SourcedEvent<IPackage> {
        public PackageUpdatedEvent(PathId targetPathId, IPackage source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "删除了包后")]
    public class PackageRemovedEvent : SourcedEvent<IPackage> {
        public PackageRemovedEvent(PathId targetPathId, IPackage source) : base(targetPathId, source) {
        }
    }
    #endregion

    #region Kernel Messages
    [MessageType(description: "添加内核")]
    public class AddKernelCommand : AddEntityCommand<IKernel> {
        public AddKernelCommand(IKernel input) : base(input) {
        }
    }

    [MessageType(description: "更新内核")]
    public class UpdateKernelCommand : UpdateEntityCommand<IKernel> {
        public UpdateKernelCommand(IKernel input) : base(input) {
        }
    }

    [MessageType(description: "删除内核")]
    public class RemoveKernelCommand : RemoveEntityCommand {
        public RemoveKernelCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "添加了内核后")]
    public class KernelAddedEvent : SourcedEvent<IKernel> {
        public KernelAddedEvent(PathId targetPathId, IKernel source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "更新了内核后")]
    public class KernelUpdatedEvent : SourcedEvent<IKernel> {
        public KernelUpdatedEvent(PathId targetPathId, IKernel source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "删除了内核后")]
    public class KernelRemovedEvent : SourcedEvent<IKernel> {
        public KernelRemovedEvent(PathId targetPathId, IKernel source) : base(targetPathId, source) {
        }
    }
    #endregion

    #region KernelInput Messages
    [MessageType(description: "添加内核输入组")]
    public class AddKernelInputCommand : AddEntityCommand<IKernelInput> {
        public AddKernelInputCommand(IKernelInput input) : base(input) {
        }
    }

    [MessageType(description: "更新内核输入组")]
    public class UpdateKernelInputCommand : UpdateEntityCommand<IKernelInput> {
        public UpdateKernelInputCommand(IKernelInput input) : base(input) {
        }
    }

    [MessageType(description: "移除内核输入组")]
    public class RemoveKernelInputCommand : RemoveEntityCommand {
        public RemoveKernelInputCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "添加了内核输入组后")]
    public class KernelInputAddedEvent : SourcedEvent<IKernelInput> {
        public KernelInputAddedEvent(PathId targetPathId, IKernelInput source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "更新了内核输入组后")]
    public class KernelInputUpdatedEvent : SourcedEvent<IKernelInput> {
        public KernelInputUpdatedEvent(PathId targetPathId, IKernelInput source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "移除了内核输入组后")]
    public class KernelInputRemovedEvent : SourcedEvent<IKernelInput> {
        public KernelInputRemovedEvent(PathId targetPathId, IKernelInput source) : base(targetPathId, source) {
        }
    }
    #endregion

    #region KernelOutput Messages
    [MessageType(description: "添加内核输出组")]
    public class AddKernelOutputCommand : AddEntityCommand<IKernelOutput> {
        public AddKernelOutputCommand(IKernelOutput input) : base(input) {
        }
    }

    [MessageType(description: "更新内核输出组")]
    public class UpdateKernelOutputCommand : UpdateEntityCommand<IKernelOutput> {
        public UpdateKernelOutputCommand(IKernelOutput input) : base(input) {
        }
    }

    [MessageType(description: "移除内核输出组")]
    public class RemoveKernelOutputCommand : RemoveEntityCommand {
        public RemoveKernelOutputCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "添加了内核输出组后")]
    public class KernelOutputAddedEvent : SourcedEvent<IKernelOutput> {
        public KernelOutputAddedEvent(PathId targetPathId, IKernelOutput source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "更新了内核输出组后")]
    public class KernelOutputUpdatedEvent : SourcedEvent<IKernelOutput> {
        public KernelOutputUpdatedEvent(PathId targetPathId, IKernelOutput source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "移除了内核输出组后")]
    public class KernelOutputRemovedEvent : SourcedEvent<IKernelOutput> {
        public KernelOutputRemovedEvent(PathId targetPathId, IKernelOutput source) : base(targetPathId, source) {
        }
    }
    #endregion

    #region KernelOutputTranslater Messages
    [MessageType(description: "添加内核输出翻译器")]
    public class AddKernelOutputTranslaterCommand : AddEntityCommand<IKernelOutputTranslater> {
        public AddKernelOutputTranslaterCommand(IKernelOutputTranslater input) : base(input) {
        }
    }

    [MessageType(description: "更新内核输出翻译器")]
    public class UpdateKernelOutputTranslaterCommand : UpdateEntityCommand<IKernelOutputTranslater> {
        public UpdateKernelOutputTranslaterCommand(IKernelOutputTranslater input) : base(input) {
        }
    }

    [MessageType(description: "移除内核输出翻译器")]
    public class RemoveKernelOutputTranslaterCommand : RemoveEntityCommand {
        public RemoveKernelOutputTranslaterCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "添加了内核输出翻译器后")]
    public class KernelOutputTranslaterAddedEvent : SourcedEvent<IKernelOutputTranslater> {
        public KernelOutputTranslaterAddedEvent(PathId targetPathId, IKernelOutputTranslater source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "更新了内核输出翻译器后")]
    public class KernelOutputTranslaterUpdatedEvent : SourcedEvent<IKernelOutputTranslater> {
        public KernelOutputTranslaterUpdatedEvent(PathId targetPathId, IKernelOutputTranslater source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "移除了内核输出翻译器后")]
    public class KernelOutputTranslaterRemovedEvent : SourcedEvent<IKernelOutputTranslater> {
        public KernelOutputTranslaterRemovedEvent(PathId targetPathId, IKernelOutputTranslater source) : base(targetPathId, source) {
        }
    }
    #endregion

    #region MineWork Messages
    [MessageType(description: "添加作业")]
    public class AddMineWorkCommand : AddEntityCommand<IMineWork> {
        public AddMineWorkCommand(IMineWork input) : base(input) {
        }
    }

    [MessageType(description: "更新作业")]
    public class UpdateMineWorkCommand : UpdateEntityCommand<IMineWork> {
        public UpdateMineWorkCommand(IMineWork input) : base(input) {
        }
    }

    [MessageType(description: "移除作业")]
    public class RemoveMineWorkCommand : RemoveEntityCommand {
        public RemoveMineWorkCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "添加了作业后")]
    public class MineWorkAddedEvent : SourcedEvent<IMineWork> {
        public MineWorkAddedEvent(PathId targetPathId, IMineWork source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "更新了作业后")]
    public class MineWorkUpdatedEvent : SourcedEvent<IMineWork> {
        public MineWorkUpdatedEvent(PathId targetPathId, IMineWork source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "移除了作业后")]
    public class MineWorkRemovedEvent : SourcedEvent<IMineWork> {
        public MineWorkRemovedEvent(PathId targetPathId, IMineWork source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "外网群控作业集初始化后")]
    public class MineWorkSetInitedEvent : EventBase {
        public MineWorkSetInitedEvent() { }
    }
    #endregion

    #region MineGroup Messages
    [MessageType(description: "添加矿机组")]
    public class AddMinerGroupCommand : AddEntityCommand<IMinerGroup> {
        public AddMinerGroupCommand(IMinerGroup input) : base(input) {
        }
    }

    [MessageType(description: "更新矿机组")]
    public class UpdateMinerGroupCommand : UpdateEntityCommand<IMinerGroup> {
        public UpdateMinerGroupCommand(IMinerGroup input) : base(input) {
        }
    }

    [MessageType(description: "移除矿机组")]
    public class RemoveMinerGroupCommand : RemoveEntityCommand {
        public RemoveMinerGroupCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "添加了矿机组后")]
    public class MinerGroupAddedEvent : SourcedEvent<IMinerGroup> {
        public MinerGroupAddedEvent(PathId targetPathId, IMinerGroup source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "更新了矿机组后")]
    public class MinerGroupUpdatedEvent : SourcedEvent<IMinerGroup> {
        public MinerGroupUpdatedEvent(PathId targetPathId, IMinerGroup source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "移除了矿机组后")]
    public class MinerGroupRemovedEvent : SourcedEvent<IMinerGroup> {
        public MinerGroupRemovedEvent(PathId targetPathId, IMinerGroup source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "外网群控矿工组集初始化后")]
    public class MinerGroupSetInitedEvent : EventBase {
        public MinerGroupSetInitedEvent() { }
    }
    #endregion

    #region ColumnsShow Messages
    [MessageType(description: "添加或修改了列分组后")]
    public class ColumnsShowAddedOrUpdatedEvent : SourcedEvent<IColumnsShow> {
        public ColumnsShowAddedOrUpdatedEvent(PathId targetPathId, IColumnsShow source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "删除了列分组后")]
    public class ColumnsRemovedEvent : SourcedEvent<IColumnsShow> {
        public ColumnsRemovedEvent(PathId targetPathId, IColumnsShow source) : base(targetPathId, source) { }
    }
    #endregion

    #region User Messages
    [MessageType(description: "启用了用户后")]
    public class UserEnabledEvent : SourcedEvent<IUser> {
        public UserEnabledEvent(PathId targetPathId, IUser source) : base(targetPathId, source) {
        }
    }

    [MessageType(description: "禁用了用户后")]
    public class UserDisabledEvent : SourcedEvent<IUser> {
        public UserDisabledEvent(PathId targetPathId, IUser source) : base(targetPathId, source) {
        }
    }
    #endregion

    [MessageType(description: "用户设置数据集变更后")]
    public class UserAppSettingSetInitedEvent : EventBase {
        public UserAppSettingSetInitedEvent() { }
    }
}
