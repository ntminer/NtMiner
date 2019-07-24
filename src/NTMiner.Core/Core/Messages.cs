using NTMiner.Bus;
using NTMiner.Core.Gpus;
using NTMiner.MinerClient;
using NTMiner.MinerServer;
using System;

namespace NTMiner.Core {
    public class ServerJsonVersionChangedEvent : EventBase {
        public ServerJsonVersionChangedEvent(string oldVersion, string newVersion) {
            this.OldVersion = oldVersion;
            this.NewVersion = newVersion;
        }

        public string OldVersion { get; private set; }
        public string NewVersion { get; private set; }
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

    [MessageType(description: "收益计算器数据集初始化后")]
    public class CalcConfigSetInitedEvent : EventBase {
        public CalcConfigSetInitedEvent() {
        }
    }

    [MessageType(description: "从内核输出中提取了矿池延时")]
    public class PoolDelayPickedEvent : EventBase {
        public PoolDelayPickedEvent(string poolDelay, bool isDual) {
            this.PoolDelayText = poolDelay;
            this.IsDual = isDual;
        }

        public bool IsDual { get; private set; }
        /// <summary>
        /// 矿池延时只用于展示，没有其它用户所以是字符串
        /// </summary>
        public string PoolDelayText { get; private set; }
    }

    [MessageType(description: "禁用win10系统更新")]
    public class BlockWAUCommand : Cmd {
        public BlockWAUCommand() { }
    }

    [MessageType(description: "禁用win10系统更新后")]
    public class BlockWAUEvent : EventBase {
        public BlockWAUEvent(bool isSuccess, string message) {
            this.IsSuccess = isSuccess;
            this.Message = message;
        }

        public bool IsSuccess { get; private set; }
        public string Message { get; private set; }
    }

    [MessageType(description: "优化window10")]
    public class Win10OptimizeCommand : Cmd {
        public Win10OptimizeCommand() { }
    }

    [MessageType(description: "优化window10后")]
    public class Win10OptimizeEvent : EventBase {
        public Win10OptimizeEvent(bool isSuccess, string message) {
            this.IsSuccess = isSuccess;
            this.Message = message;
        }

        public bool IsSuccess { get; private set; }
        public string Message { get; private set; }
    }

    [MessageType(description: "开起A卡计算模式")]
    public class SwitchRadeonGpuCommand : Cmd {
        public SwitchRadeonGpuCommand() {
        }
    }

    [MessageType(description: "A卡驱动签名")]
    public class AtikmdagPatcherCommand : Cmd {
        public AtikmdagPatcherCommand() {
        }
    }

    [MessageType(description: "注册右键打开windindows命令行菜单")]
    public class RegCmdHereCommand : Cmd {
        public RegCmdHereCommand() { }
    }

    [MessageType(description: "注册右键打开windindows命令行菜单后")]
    public class RegCmdHereEvent : EventBase {
        public RegCmdHereEvent(bool isSuccess, string message) {
            this.IsSuccess = isSuccess;
            this.Message = message;
        }

        public bool IsSuccess { get; private set; }
        public string Message { get; private set; }
    }

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

    #region MineWork Messages
    [MessageType(description: "作业设置变更后")]
    public class MineWorkPropertyChangedEvent : EventBase {
        public MineWorkPropertyChangedEvent(string propertyName) {
            this.PropertyName = propertyName;
        }

        public string PropertyName { get; private set; }
    }

    [MessageType(description: "添加工作")]
    public class AddMineWorkCommand : AddEntityCommand<IMineWork> {
        public AddMineWorkCommand(IMineWork input) : base(input) {
        }
    }

    [MessageType(description: "更新工作")]
    public class UpdateMineWorkCommand : UpdateEntityCommand<IMineWork> {
        public UpdateMineWorkCommand(IMineWork input) : base(input) {
        }
    }

    [MessageType(description: "删除工作")]
    public class RemoveMineWorkCommand : RemoveEntityCommand {
        public RemoveMineWorkCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "添加工作后")]
    public class MineWorkAddedEvent : DomainEvent<IMineWork> {
        public MineWorkAddedEvent(IMineWork source) : base(source) {
        }
    }

    [MessageType(description: "更新工作后")]
    public class MineWorkUpdatedEvent : DomainEvent<IMineWork> {
        public MineWorkUpdatedEvent(IMineWork source) : base(source) {
        }
    }

    [MessageType(description: "删除工作后")]
    public class MineWorkRemovedEvent : DomainEvent<IMineWork> {
        public MineWorkRemovedEvent(IMineWork source) : base(source) {
        }
    }
    #endregion

    #region MinerGroup Messages
    [MessageType(description: "添加矿机分组")]
    public class AddMinerGroupCommand : AddEntityCommand<IMinerGroup> {
        public AddMinerGroupCommand(IMinerGroup input) : base(input) {
        }
    }

    [MessageType(description: "更新矿机分组")]
    public class UpdateMinerGroupCommand : UpdateEntityCommand<IMinerGroup> {
        public UpdateMinerGroupCommand(IMinerGroup input) : base(input) {
        }
    }

    [MessageType(description: "删除矿机分组")]
    public class RemoveMinerGroupCommand : RemoveEntityCommand {
        public RemoveMinerGroupCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "添加矿机分组后")]
    public class MinerGroupAddedEvent : DomainEvent<IMinerGroup> {
        public MinerGroupAddedEvent(IMinerGroup source) : base(source) {
        }
    }

    [MessageType(description: "更新矿机分组后")]
    public class MinerGroupUpdatedEvent : DomainEvent<IMinerGroup> {
        public MinerGroupUpdatedEvent(IMinerGroup source) : base(source) {
        }
    }

    [MessageType(description: "删除矿机分组后")]
    public class MinerGroupRemovedEvent : DomainEvent<IMinerGroup> {
        public MinerGroupRemovedEvent(IMinerGroup source) : base(source) {
        }
    }
    #endregion

    #region OverClockData Messages
    [MessageType(description: "添加超频建议")]
    public class AddOverClockDataCommand : AddEntityCommand<IOverClockData> {
        public AddOverClockDataCommand(IOverClockData input) : base(input) {
        }
    }

    [MessageType(description: "更新超频建议")]
    public class UpdateOverClockDataCommand : UpdateEntityCommand<IOverClockData> {
        public UpdateOverClockDataCommand(IOverClockData input) : base(input) {
        }
    }

    [MessageType(description: "删除超频建议")]
    public class RemoveOverClockDataCommand : RemoveEntityCommand {
        public RemoveOverClockDataCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "添加超频建议后")]
    public class OverClockDataAddedEvent : DomainEvent<IOverClockData> {
        public OverClockDataAddedEvent(IOverClockData source) : base(source) {
        }
    }

    [MessageType(description: "更新超频建议后")]
    public class OverClockDataUpdatedEvent : DomainEvent<IOverClockData> {
        public OverClockDataUpdatedEvent(IOverClockData source) : base(source) {
        }
    }

    [MessageType(description: "删除超频建议后")]
    public class OverClockDataRemovedEvent : DomainEvent<IOverClockData> {
        public OverClockDataRemovedEvent(IOverClockData source) : base(source) {
        }
    }

    [MessageType(description: "超频建议集初始化后")]
    public class OverClockDataSetInitedEvent : EventBase {
        public OverClockDataSetInitedEvent() {
        }
    }
    #endregion

    #region ColumnsShow Messages
    [MessageType(description: "添加矿机分组")]
    public class AddColumnsShowCommand : AddEntityCommand<IColumnsShow> {
        public AddColumnsShowCommand(IColumnsShow input) : base(input) {
        }
    }

    [MessageType(description: "更新列显")]
    public class UpdateColumnsShowCommand : UpdateEntityCommand<IColumnsShow> {
        public UpdateColumnsShowCommand(IColumnsShow input) : base(input) {
        }
    }

    [MessageType(description: "删除列显")]
    public class RemoveColumnsShowCommand : RemoveEntityCommand {
        public RemoveColumnsShowCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "添加列显后")]
    public class ColumnsShowAddedEvent : DomainEvent<IColumnsShow> {
        public ColumnsShowAddedEvent(IColumnsShow source) : base(source) {
        }
    }

    [MessageType(description: "更新列显后")]
    public class ColumnsShowUpdatedEvent : DomainEvent<IColumnsShow> {
        public ColumnsShowUpdatedEvent(IColumnsShow source) : base(source) {
        }
    }

    [MessageType(description: "删除列显后")]
    public class ColumnsShowRemovedEvent : DomainEvent<IColumnsShow> {
        public ColumnsShowRemovedEvent(IColumnsShow source) : base(source) {
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

    [MessageType(description: "Gpu超频数据添加或更新后")]
    public class GpuProfileAddedOrUpdatedEvent : DomainEvent<IGpuProfile> {
        public GpuProfileAddedOrUpdatedEvent(IGpuProfile source) : base(source) {
        }
    }
    #endregion

    #region speed and share
    [MessageType(description: "显卡算力变更事件")]
    public class GpuSpeedChangedEvent : DomainEvent<IGpuSpeed> {
        public GpuSpeedChangedEvent(bool isDualSpeed, IGpuSpeed gpuSpeed) : base(gpuSpeed) {
            this.IsDualSpeed = isDualSpeed;
        }

        public bool IsDualSpeed { get; private set; }
    }

    [MessageType(description: "收益变更事件")]
    public class ShareChangedEvent : DomainEvent<ICoinShare> {
        public ShareChangedEvent(ICoinShare share) : base(share) {
        }
    }
    #endregion

    #region Gpu Messages
    [MessageType(description: "显卡状态变更事件", isCanNoHandler: true)]
    public class GpuStateChangedEvent : DomainEvent<IGpu> {
        public GpuStateChangedEvent(IGpu source) : base(source) {
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
    public class SysDicAddedEvent : DomainEvent<ISysDic> {
        public SysDicAddedEvent(ISysDic source) : base(source) {
        }
    }

    [MessageType(description: "更新系统字典后")]
    public class SysDicUpdatedEvent : DomainEvent<ISysDic> {
        public SysDicUpdatedEvent(ISysDic source) : base(source) {
        }
    }

    [MessageType(description: "删除系统字典后")]
    public class SysDicRemovedEvent : DomainEvent<ISysDic> {
        public SysDicRemovedEvent(ISysDic source) : base(source) {
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
    public class SysDicItemAddedEvent : DomainEvent<ISysDicItem> {
        public SysDicItemAddedEvent(ISysDicItem source) : base(source) {
        }
    }

    [MessageType(description: "更新了系统字典项后")]
    public class SysDicItemUpdatedEvent : DomainEvent<ISysDicItem> {
        public SysDicItemUpdatedEvent(ISysDicItem source) : base(source) {
        }
    }

    [MessageType(description: "删除了系统字典项后")]
    public class SysDicItemRemovedEvent : DomainEvent<ISysDicItem> {
        public SysDicItemRemovedEvent(ISysDicItem source) : base(source) {
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
    public class CoinAddedEvent : DomainEvent<ICoin> {
        public CoinAddedEvent(ICoin source) : base(source) {
        }
    }

    [MessageType(description: "更新了币种后")]
    public class CoinUpdatedEvent : DomainEvent<ICoin> {
        public CoinUpdatedEvent(ICoin source) : base(source) {
        }
    }

    [MessageType(description: "移除了币种后")]
    public class CoinRemovedEvent : DomainEvent<ICoin> {
        public CoinRemovedEvent(ICoin source) : base(source) {
        }
    }

    [MessageType(description: "下载了币种图标后")]
    public class CoinIconDownloadedEvent : DomainEvent<ICoin> {
        public CoinIconDownloadedEvent(ICoin source) : base(source) {
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
    public class GroupAddedEvent : DomainEvent<IGroup> {
        public GroupAddedEvent(IGroup source) : base(source) {
        }
    }

    [MessageType(description: "更新了组后")]
    public class GroupUpdatedEvent : DomainEvent<IGroup> {
        public GroupUpdatedEvent(IGroup source) : base(source) {
        }
    }

    [MessageType(description: "移除了组后")]
    public class GroupRemovedEvent : DomainEvent<IGroup> {
        public GroupRemovedEvent(IGroup source) : base(source) {
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
    public class CoinGroupAddedEvent : DomainEvent<ICoinGroup> {
        public CoinGroupAddedEvent(ICoinGroup source) : base(source) {
        }
    }

    [MessageType(description: "更新了币组后")]
    public class CoinGroupUpdatedEvent : DomainEvent<ICoinGroup> {
        public CoinGroupUpdatedEvent(ICoinGroup source) : base(source) {
        }
    }

    [MessageType(description: "移除了币组后")]
    public class CoinGroupRemovedEvent : DomainEvent<ICoinGroup> {
        public CoinGroupRemovedEvent(ICoinGroup source) : base(source) {
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
    public class FileWriterAddedEvent : DomainEvent<IFileWriter> {
        public FileWriterAddedEvent(IFileWriter source) : base(source) {
        }
    }

    [MessageType(description: "更新了文件书写器后")]
    public class FileWriterUpdatedEvent : DomainEvent<IFileWriter> {
        public FileWriterUpdatedEvent(IFileWriter source) : base(source) {
        }
    }

    [MessageType(description: "移除了文件书写器后")]
    public class FileWriterRemovedEvent : DomainEvent<IFileWriter> {
        public FileWriterRemovedEvent(IFileWriter source) : base(source) {
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
    public class FragmentWriterAddedEvent : DomainEvent<IFragmentWriter> {
        public FragmentWriterAddedEvent(IFragmentWriter source) : base(source) {
        }
    }

    [MessageType(description: "更新了命令行片段书写器后")]
    public class FragmentWriterUpdatedEvent : DomainEvent<IFragmentWriter> {
        public FragmentWriterUpdatedEvent(IFragmentWriter source) : base(source) {
        }
    }

    [MessageType(description: "移除了命令行片段书写器后")]
    public class FragmentWriterRemovedEvent : DomainEvent<IFragmentWriter> {
        public FragmentWriterRemovedEvent(IFragmentWriter source) : base(source) {
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
    public class WalletAddedEvent : DomainEvent<IWallet> {
        public WalletAddedEvent(IWallet source) : base(source) {
        }
    }

    [MessageType(description: "更新了钱包后")]
    public class WalletUpdatedEvent : DomainEvent<IWallet> {
        public WalletUpdatedEvent(IWallet source) : base(source) {
        }
    }

    [MessageType(description: "移除了钱包后")]
    public class WalletRemovedEvent : DomainEvent<IWallet> {
        public WalletRemovedEvent(IWallet source) : base(source) {
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
    public class PoolAddedEvent : DomainEvent<IPool> {
        public PoolAddedEvent(IPool source) : base(source) {
        }
    }

    [MessageType(description: "更新了矿池后")]
    public class PoolUpdatedEvent : DomainEvent<IPool> {
        public PoolUpdatedEvent(IPool source) : base(source) {
        }
    }

    [MessageType(description: "移除了矿池后")]
    public class PoolRemovedEvent : DomainEvent<IPool> {
        public PoolRemovedEvent(IPool source) : base(source) {
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
    public class CoinKernelAddedEvent : DomainEvent<ICoinKernel> {
        public CoinKernelAddedEvent(ICoinKernel source) : base(source) {
        }
    }

    [MessageType(description: "更新了币种级内核后")]
    public class CoinKernelUpdatedEvent : DomainEvent<ICoinKernel> {
        public CoinKernelUpdatedEvent(ICoinKernel source) : base(source) {
        }
    }

    [MessageType(description: "移除了币种级内核后")]
    public class CoinKernelRemovedEvent : DomainEvent<ICoinKernel> {
        public CoinKernelRemovedEvent(ICoinKernel source) : base(source) {
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
    public class PoolKernelAddedEvent : DomainEvent<IPoolKernel> {
        public PoolKernelAddedEvent(IPoolKernel source) : base(source) {
        }
    }

    [MessageType(description: "更新了矿池级内核后")]
    public class PoolKernelUpdatedEvent : DomainEvent<IPoolKernel> {
        public PoolKernelUpdatedEvent(IPoolKernel source) : base(source) {
        }
    }

    [MessageType(description: "移除了矿池级内核后")]
    public class PoolKernelRemovedEvent : DomainEvent<IPoolKernel> {
        public PoolKernelRemovedEvent(IPoolKernel source) : base(source) {
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
    public class PackageAddedEvent : DomainEvent<IPackage> {
        public PackageAddedEvent(IPackage source) : base(source) {
        }
    }

    [MessageType(description: "更新了包后")]
    public class PackageUpdatedEvent : DomainEvent<IPackage> {
        public PackageUpdatedEvent(IPackage source) : base(source) {
        }
    }

    [MessageType(description: "删除了包后")]
    public class PackageRemovedEvent : DomainEvent<IPackage> {
        public PackageRemovedEvent(IPackage source) : base(source) {
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
    public class KernelAddedEvent : DomainEvent<IKernel> {
        public KernelAddedEvent(IKernel source) : base(source) {
        }
    }

    [MessageType(description: "更新了内核后")]
    public class KernelUpdatedEvent : DomainEvent<IKernel> {
        public KernelUpdatedEvent(IKernel source) : base(source) {
        }
    }

    [MessageType(description: "删除了内核后")]
    public class KernelRemovedEvent : DomainEvent<IKernel> {
        public KernelRemovedEvent(IKernel source) : base(source) {
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
    public class KernelInputAddedEvent : DomainEvent<IKernelInput> {
        public KernelInputAddedEvent(IKernelInput source) : base(source) {
        }
    }

    [MessageType(description: "更新了内核输入组后")]
    public class KernelInputUpdatedEvent : DomainEvent<IKernelInput> {
        public KernelInputUpdatedEvent(IKernelInput source) : base(source) {
        }
    }

    [MessageType(description: "移除了内核输入组后")]
    public class KernelInputRemovedEvent : DomainEvent<IKernelInput> {
        public KernelInputRemovedEvent(IKernelInput source) : base(source) {
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
    public class KernelOutputAddedEvent : DomainEvent<IKernelOutput> {
        public KernelOutputAddedEvent(IKernelOutput source) : base(source) {
        }
    }

    [MessageType(description: "更新了内核输出组后")]
    public class KernelOutputUpdatedEvent : DomainEvent<IKernelOutput> {
        public KernelOutputUpdatedEvent(IKernelOutput source) : base(source) {
        }
    }

    [MessageType(description: "移除了内核输出组后")]
    public class KernelOutputRemovedEvent : DomainEvent<IKernelOutput> {
        public KernelOutputRemovedEvent(IKernelOutput source) : base(source) {
        }
    }
    #endregion

    #region KernelOutputFilter Messages
    [MessageType(description: "添加内核输出过滤器")]
    public class AddKernelOutputFilterCommand : AddEntityCommand<IKernelOutputFilter> {
        public AddKernelOutputFilterCommand(IKernelOutputFilter input) : base(input) {
        }
    }

    [MessageType(description: "更新内核输出过滤器")]
    public class UpdateKernelOutputFilterCommand : UpdateEntityCommand<IKernelOutputFilter> {
        public UpdateKernelOutputFilterCommand(IKernelOutputFilter input) : base(input) {
        }
    }

    [MessageType(description: "移除内核输出过滤器")]
    public class RemoveKernelOutputFilterCommand : RemoveEntityCommand {
        public RemoveKernelOutputFilterCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "添加了内核输出过滤器后")]
    public class KernelOutputFilterAddedEvent : DomainEvent<IKernelOutputFilter> {
        public KernelOutputFilterAddedEvent(IKernelOutputFilter source) : base(source) {
        }
    }

    [MessageType(description: "更新了内核输出过滤器后")]
    public class KernelOutputFilterUpdatedEvent : DomainEvent<IKernelOutputFilter> {
        public KernelOutputFilterUpdatedEvent(IKernelOutputFilter source) : base(source) {
        }
    }

    [MessageType(description: "移除了内核输出过滤器后")]
    public class KernelOutputFilterRemovedEvent : DomainEvent<IKernelOutputFilter> {
        public KernelOutputFilterRemovedEvent(IKernelOutputFilter source) : base(source) {
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
    public class KernelOutputTranslaterAddedEvent : DomainEvent<IKernelOutputTranslater> {
        public KernelOutputTranslaterAddedEvent(IKernelOutputTranslater source) : base(source) {
        }
    }

    [MessageType(description: "更新了内核输出翻译器后")]
    public class KernelOutputTranslaterUpdatedEvent : DomainEvent<IKernelOutputTranslater> {
        public KernelOutputTranslaterUpdatedEvent(IKernelOutputTranslater source) : base(source) {
        }
    }

    [MessageType(description: "移除了内核输出翻译器后")]
    public class KernelOutputTranslaterRemovedEvent : DomainEvent<IKernelOutputTranslater> {
        public KernelOutputTranslaterRemovedEvent(IKernelOutputTranslater source) : base(source) {
        }
    }
    #endregion

    #region WorkerEvent Messages
    [MessageType(description: "发生了矿机事件")]
    public class WorkerEventOccurredEvent : DomainEvent<IWorkerEvent> {
        public WorkerEventOccurredEvent(IWorkerEvent source) : base(source) {
        }
    }
    #endregion

    #region KernelOutputKeyword Messages
    [MessageType(description: "添加内核输出关键字")]
    public class AddKernelOutputKeywordCommand : AddEntityCommand<IKernelOutputKeyword> {
        public AddKernelOutputKeywordCommand(IKernelOutputKeyword input) : base(input) {
        }
    }

    [MessageType(description: "更新内核输出关键字")]
    public class UpdateKernelOutputKeywordCommand : UpdateEntityCommand<IKernelOutputKeyword> {
        public UpdateKernelOutputKeywordCommand(IKernelOutputKeyword input) : base(input) {
        }
    }

    [MessageType(description: "移除内核输出关键字")]
    public class RemoveKernelOutputKeywordCommand : RemoveEntityCommand {
        public RemoveKernelOutputKeywordCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(description: "添加了内核输出关键字后")]
    public class KernelOutputKeywordAddedEvent : DomainEvent<IKernelOutputKeyword> {
        public KernelOutputKeywordAddedEvent(IKernelOutputKeyword source) : base(source) {
        }
    }

    [MessageType(description: "更新了内核输出关键字后")]
    public class KernelOutputKeywordUpdatedEvent : DomainEvent<IKernelOutputKeyword> {
        public KernelOutputKeywordUpdatedEvent(IKernelOutputKeyword source) : base(source) {
        }
    }

    [MessageType(description: "移除了内核输出关键字后")]
    public class KernelOutputKeywordRemovedEvent : DomainEvent<IKernelOutputKeyword> {
        public KernelOutputKeywordRemovedEvent(IKernelOutputKeyword source) : base(source) {
        }
    }
    #endregion
}
