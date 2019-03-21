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

    #region profile Messages
    [MessageType(messageType: typeof(MinerProfilePropertyChangedEvent), description: "MinerProfile设置变更后")]
    public class MinerProfilePropertyChangedEvent : EventBase {
        public MinerProfilePropertyChangedEvent(string propertyName) {
            this.PropertyName = propertyName;
        }

        public string PropertyName { get; private set; }
    }

    [MessageType(messageType: typeof(CoinProfilePropertyChangedEvent), description: "挖矿币种级设置变更后")]
    public class CoinProfilePropertyChangedEvent : EventBase {
        public CoinProfilePropertyChangedEvent(Guid coinId, string propertyName) {
            this.CoinId = coinId;
            this.PropertyName = propertyName;
        }

        public Guid CoinId { get; }

        public string PropertyName { get; private set; }
    }

    [MessageType(messageType: typeof(PoolProfilePropertyChangedEvent), description: "矿池级设置变更后")]
    public class PoolProfilePropertyChangedEvent : EventBase {
        public PoolProfilePropertyChangedEvent(Guid coinId, string propertyName) {
            this.PoolId = coinId;
            this.PropertyName = propertyName;
        }

        public Guid PoolId { get; }

        public string PropertyName { get; private set; }
    }

    [MessageType(messageType: typeof(CoinKernelProfilePropertyChangedEvent), description: "挖矿币种内核级设置变更后")]
    public class CoinKernelProfilePropertyChangedEvent : EventBase {
        public CoinKernelProfilePropertyChangedEvent(Guid coinKernelId, string propertyName) {
            this.CoinKernelId = coinKernelId;
            this.PropertyName = propertyName;
        }

        public Guid CoinKernelId { get; }

        public string PropertyName { get; private set; }
    }

    [MessageType(messageType: typeof(GpuProfileSetRefreshedEvent), description: "Gpu超频集合刷新后")]
    public class GpuProfileSetRefreshedEvent : EventBase {
        public GpuProfileSetRefreshedEvent() { }
    }
    #endregion

    #region MineWork Messages
    [MessageType(messageType: typeof(MineWorkPropertyChangedEvent), description: "作业设置变更后")]
    public class MineWorkPropertyChangedEvent : EventBase {
        public MineWorkPropertyChangedEvent(string propertyName) {
            this.PropertyName = propertyName;
        }

        public string PropertyName { get; private set; }
    }

    [MessageType(messageType: typeof(AddMineWorkCommand), description: "添加工作")]
    public class AddMineWorkCommand : AddEntityCommand<IMineWork> {
        public AddMineWorkCommand(IMineWork input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(UpdateMineWorkCommand), description: "更新工作")]
    public class UpdateMineWorkCommand : UpdateEntityCommand<IMineWork> {
        public UpdateMineWorkCommand(IMineWork input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(RemoveMineWorkCommand), description: "删除工作")]
    public class RemoveMineWorkCommand : RemoveEntityCommand {
        public RemoveMineWorkCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(messageType: typeof(MineWorkAddedEvent), description: "添加工作后")]
    public class MineWorkAddedEvent : DomainEvent<IMineWork> {
        public MineWorkAddedEvent(IMineWork source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(MineWorkUpdatedEvent), description: "更新工作后")]
    public class MineWorkUpdatedEvent : DomainEvent<IMineWork> {
        public MineWorkUpdatedEvent(IMineWork source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(MineWorkRemovedEvent), description: "删除工作后")]
    public class MineWorkRemovedEvent : DomainEvent<IMineWork> {
        public MineWorkRemovedEvent(IMineWork source) : base(source) {
        }
    }
    #endregion

    #region MinerGroup Messages
    [MessageType(messageType: typeof(AddMinerGroupCommand), description: "添加矿机分组")]
    public class AddMinerGroupCommand : AddEntityCommand<IMinerGroup> {
        public AddMinerGroupCommand(IMinerGroup input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(UpdateMinerGroupCommand), description: "更新矿机分组")]
    public class UpdateMinerGroupCommand : UpdateEntityCommand<IMinerGroup> {
        public UpdateMinerGroupCommand(IMinerGroup input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(RemoveMinerGroupCommand), description: "删除矿机分组")]
    public class RemoveMinerGroupCommand : RemoveEntityCommand {
        public RemoveMinerGroupCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(messageType: typeof(MinerGroupAddedEvent), description: "添加矿机分组后")]
    public class MinerGroupAddedEvent : DomainEvent<IMinerGroup> {
        public MinerGroupAddedEvent(IMinerGroup source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(MinerGroupUpdatedEvent), description: "更新矿机分组后")]
    public class MinerGroupUpdatedEvent : DomainEvent<IMinerGroup> {
        public MinerGroupUpdatedEvent(IMinerGroup source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(MinerGroupRemovedEvent), description: "删除矿机分组后")]
    public class MinerGroupRemovedEvent : DomainEvent<IMinerGroup> {
        public MinerGroupRemovedEvent(IMinerGroup source) : base(source) {
        }
    }
    #endregion

    #region OverClockData Messages
    [MessageType(messageType: typeof(AddOverClockDataCommand), description: "添加超频建议")]
    public class AddOverClockDataCommand : AddEntityCommand<IOverClockData> {
        public AddOverClockDataCommand(IOverClockData input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(UpdateOverClockDataCommand), description: "更新超频建议")]
    public class UpdateOverClockDataCommand : UpdateEntityCommand<IOverClockData> {
        public UpdateOverClockDataCommand(IOverClockData input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(RemoveOverClockDataCommand), description: "删除超频建议")]
    public class RemoveOverClockDataCommand : RemoveEntityCommand {
        public RemoveOverClockDataCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(messageType: typeof(OverClockDataAddedEvent), description: "添加超频建议后")]
    public class OverClockDataAddedEvent : DomainEvent<IOverClockData> {
        public OverClockDataAddedEvent(IOverClockData source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(OverClockDataUpdatedEvent), description: "更新超频建议后")]
    public class OverClockDataUpdatedEvent : DomainEvent<IOverClockData> {
        public OverClockDataUpdatedEvent(IOverClockData source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(OverClockDataRemovedEvent), description: "删除超频建议后")]
    public class OverClockDataRemovedEvent : DomainEvent<IOverClockData> {
        public OverClockDataRemovedEvent(IOverClockData source) : base(source) {
        }
    }
    #endregion

    #region ColumnsShow Messages
    [MessageType(messageType: typeof(AddColumnsShowCommand), description: "添加矿机分组")]
    public class AddColumnsShowCommand : AddEntityCommand<IColumnsShow> {
        public AddColumnsShowCommand(IColumnsShow input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(UpdateColumnsShowCommand), description: "更新列显")]
    public class UpdateColumnsShowCommand : UpdateEntityCommand<IColumnsShow> {
        public UpdateColumnsShowCommand(IColumnsShow input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(RemoveColumnsShowCommand), description: "删除列显")]
    public class RemoveColumnsShowCommand : RemoveEntityCommand {
        public RemoveColumnsShowCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(messageType: typeof(ColumnsShowAddedEvent), description: "添加列显后")]
    public class ColumnsShowAddedEvent : DomainEvent<IColumnsShow> {
        public ColumnsShowAddedEvent(IColumnsShow source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(ColumnsShowUpdatedEvent), description: "更新列显后")]
    public class ColumnsShowUpdatedEvent : DomainEvent<IColumnsShow> {
        public ColumnsShowUpdatedEvent(IColumnsShow source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(ColumnsShowRemovedEvent), description: "删除列显后")]
    public class ColumnsShowRemovedEvent : DomainEvent<IColumnsShow> {
        public ColumnsShowRemovedEvent(IColumnsShow source) : base(source) {
        }
    }
    #endregion

    #region gpu overclock
    [MessageType(messageType: typeof(AddOrUpdateGpuProfileCommand), description: "添加或更新Gpu超频数据")]
    public class AddOrUpdateGpuProfileCommand : Cmd {
        public AddOrUpdateGpuProfileCommand(IGpuProfile input) {
            this.Input = input;
        }

        public IGpuProfile Input { get; private set; }
    }

    [MessageType(messageType: typeof(OverClockCommand), description: "超频")]
    public class OverClockCommand : Cmd {
        public OverClockCommand(IGpuProfile input) {
            this.Input = input;
        }

        public IGpuProfile Input { get; private set; }
    }

    [MessageType(messageType: typeof(CoinOverClockCommand), description: "币种超频")]
    public class CoinOverClockCommand : Cmd {
        public CoinOverClockCommand(Guid coinId) {
            this.CoinId = coinId;
        }

        public Guid CoinId { get; private set; }
    }

    [MessageType(messageType: typeof(AddOrUpdateGpuProfileCommand), description: "Gpu超频数据添加或更新后")]
    public class GpuProfileAddedOrUpdatedEvent : DomainEvent<IGpuProfile> {
        public GpuProfileAddedOrUpdatedEvent(IGpuProfile source) : base(source) {
        }
    }
    #endregion

    #region speed and share
    [MessageType(messageType: typeof(GpuSpeedChangedEvent), description: "显卡算力变更事件")]
    public class GpuSpeedChangedEvent : DomainEvent<IGpuSpeed> {
        public GpuSpeedChangedEvent(bool isDualSpeed, IGpuSpeed gpuSpeed) : base(gpuSpeed) {
            this.IsDualSpeed = isDualSpeed;
        }

        public bool IsDualSpeed { get; private set; }
    }

    [MessageType(messageType: typeof(ShareChangedEvent), description: "收益变更事件")]
    public class ShareChangedEvent : DomainEvent<ICoinShare> {
        public ShareChangedEvent(ICoinShare share) : base(share) {
        }
    }
    #endregion

    #region Gpu Messages
    [MessageType(messageType: typeof(GpuStateChangedEvent), description: "显卡状态变更事件", isCanNoHandler: true)]
    public class GpuStateChangedEvent : DomainEvent<IGpu> {
        public GpuStateChangedEvent(IGpu source) : base(source) {
        }
    }
    #endregion

    #region SysDic Messages
    [MessageType(messageType: typeof(AddSysDicCommand), description: "添加系统字典")]
    public class AddSysDicCommand : AddEntityCommand<ISysDic> {
        public AddSysDicCommand(ISysDic input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(UpdateSysDicCommand), description: "更新系统字典")]
    public class UpdateSysDicCommand : UpdateEntityCommand<ISysDic> {
        public UpdateSysDicCommand(ISysDic input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(RemoveSysDicCommand), description: "删除系统字典")]
    public class RemoveSysDicCommand : RemoveEntityCommand {
        public RemoveSysDicCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(messageType: typeof(SysDicAddedEvent), description: "添加系统字典后")]
    public class SysDicAddedEvent : DomainEvent<ISysDic> {
        public SysDicAddedEvent(ISysDic source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(SysDicUpdatedEvent), description: "更新系统字典后")]
    public class SysDicUpdatedEvent : DomainEvent<ISysDic> {
        public SysDicUpdatedEvent(ISysDic source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(SysDicRemovedEvent), description: "删除系统字典后")]
    public class SysDicRemovedEvent : DomainEvent<ISysDic> {
        public SysDicRemovedEvent(ISysDic source) : base(source) {
        }
    }
    #endregion

    #region SysDicItem Messages
    [MessageType(messageType: typeof(AddSysDicItemCommand), description: "添加系统字典项")]
    public class AddSysDicItemCommand : AddEntityCommand<ISysDicItem> {
        public AddSysDicItemCommand(ISysDicItem input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(UpdateSysDicItemCommand), description: "更新系统字典项")]
    public class UpdateSysDicItemCommand : UpdateEntityCommand<ISysDicItem> {
        public UpdateSysDicItemCommand(ISysDicItem input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(RemoveSysDicItemCommand), description: "移除系统字典项")]
    public class RemoveSysDicItemCommand : RemoveEntityCommand {
        public RemoveSysDicItemCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(messageType: typeof(SysDicItemAddedEvent), description: "添加了系统字典项后")]
    public class SysDicItemAddedEvent : DomainEvent<ISysDicItem> {
        public SysDicItemAddedEvent(ISysDicItem source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(SysDicItemUpdatedEvent), description: "更新了系统字典项后")]
    public class SysDicItemUpdatedEvent : DomainEvent<ISysDicItem> {
        public SysDicItemUpdatedEvent(ISysDicItem source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(SysDicItemRemovedEvent), description: "删除了系统字典项后")]
    public class SysDicItemRemovedEvent : DomainEvent<ISysDicItem> {
        public SysDicItemRemovedEvent(ISysDicItem source) : base(source) {
        }
    }
    #endregion

    #region Coin Messages
    [MessageType(messageType: typeof(AddCoinCommand), description: "添加币种")]
    public class AddCoinCommand : AddEntityCommand<ICoin> {
        public AddCoinCommand(ICoin input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(UpdateCoinCommand), description: "更新币种")]
    public class UpdateCoinCommand : UpdateEntityCommand<ICoin> {
        public UpdateCoinCommand(ICoin input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(RemoveCoinCommand), description: "移除币种")]
    public class RemoveCoinCommand : RemoveEntityCommand {
        public RemoveCoinCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(messageType: typeof(CoinAddedEvent), description: "添加了币种后")]
    public class CoinAddedEvent : DomainEvent<ICoin> {
        public CoinAddedEvent(ICoin source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(CoinUpdatedEvent), description: "更新了币种后")]
    public class CoinUpdatedEvent : DomainEvent<ICoin> {
        public CoinUpdatedEvent(ICoin source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(CoinRemovedEvent), description: "移除了币种后")]
    public class CoinRemovedEvent : DomainEvent<ICoin> {
        public CoinRemovedEvent(ICoin source) : base(source) {
        }
    }
    #endregion

    #region Group Messages
    [MessageType(messageType: typeof(AddGroupCommand), description: "添加组")]
    public class AddGroupCommand : AddEntityCommand<IGroup> {
        public AddGroupCommand(IGroup input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(UpdateGroupCommand), description: "更新组")]
    public class UpdateGroupCommand : UpdateEntityCommand<IGroup> {
        public UpdateGroupCommand(IGroup input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(RemoveGroupCommand), description: "移除组")]
    public class RemoveGroupCommand : RemoveEntityCommand {
        public RemoveGroupCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(messageType: typeof(GroupAddedEvent), description: "添加了组后")]
    public class GroupAddedEvent : DomainEvent<IGroup> {
        public GroupAddedEvent(IGroup source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(GroupUpdatedEvent), description: "更新了组后")]
    public class GroupUpdatedEvent : DomainEvent<IGroup> {
        public GroupUpdatedEvent(IGroup source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(GroupRemovedEvent), description: "移除了组后")]
    public class GroupRemovedEvent : DomainEvent<IGroup> {
        public GroupRemovedEvent(IGroup source) : base(source) {
        }
    }
    #endregion

    #region CoinGroup Messages
    [MessageType(messageType: typeof(AddCoinGroupCommand), description: "添加币组")]
    public class AddCoinGroupCommand : AddEntityCommand<ICoinGroup> {
        public AddCoinGroupCommand(ICoinGroup input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(UpdateCoinGroupCommand), description: "修改币组")]
    public class UpdateCoinGroupCommand : UpdateEntityCommand<ICoinGroup> {
        public UpdateCoinGroupCommand(ICoinGroup input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(RemoveCoinGroupCommand), description: "移除币组")]
    public class RemoveCoinGroupCommand : RemoveEntityCommand {
        public RemoveCoinGroupCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(messageType: typeof(CoinGroupAddedEvent), description: "添加了币组后")]
    public class CoinGroupAddedEvent : DomainEvent<ICoinGroup> {
        public CoinGroupAddedEvent(ICoinGroup source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(CoinGroupUpdatedEvent), description: "更新了币组后")]
    public class CoinGroupUpdatedEvent : DomainEvent<ICoinGroup> {
        public CoinGroupUpdatedEvent(ICoinGroup source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(CoinGroupRemovedEvent), description: "移除了币组后")]
    public class CoinGroupRemovedEvent : DomainEvent<ICoinGroup> {
        public CoinGroupRemovedEvent(ICoinGroup source) : base(source) {
        }
    }
    #endregion

    #region Wallet Messages
    [MessageType(messageType: typeof(AddWalletCommand), description: "添加钱包")]
    public class AddWalletCommand : AddEntityCommand<IWallet> {
        public AddWalletCommand(IWallet input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(UpdateWalletCommand), description: "更新钱包")]
    public class UpdateWalletCommand : UpdateEntityCommand<IWallet> {
        public UpdateWalletCommand(IWallet input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(RemoveWalletCommand), description: "移除钱包")]
    public class RemoveWalletCommand : RemoveEntityCommand {
        public RemoveWalletCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(messageType: typeof(WalletAddedEvent), description: "添加了钱包后")]
    public class WalletAddedEvent : DomainEvent<IWallet> {
        public WalletAddedEvent(IWallet source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(WalletUpdatedEvent), description: "更新了钱包后")]
    public class WalletUpdatedEvent : DomainEvent<IWallet> {
        public WalletUpdatedEvent(IWallet source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(WalletRemovedEvent), description: "移除了钱包后")]
    public class WalletRemovedEvent : DomainEvent<IWallet> {
        public WalletRemovedEvent(IWallet source) : base(source) {
        }
    }
    #endregion

    #region Pool Messages
    [MessageType(messageType: typeof(AddPoolCommand), description: "添加矿池")]
    public class AddPoolCommand : AddEntityCommand<IPool> {
        public AddPoolCommand(IPool input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(UpdatePoolCommand), description: "更新矿池")]
    public class UpdatePoolCommand : UpdateEntityCommand<IPool> {
        public UpdatePoolCommand(IPool input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(RemovePoolCommand), description: "移除矿池")]
    public class RemovePoolCommand : RemoveEntityCommand {
        public RemovePoolCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(messageType: typeof(PoolAddedEvent), description: "添加了矿池后")]
    public class PoolAddedEvent : DomainEvent<IPool> {
        public PoolAddedEvent(IPool source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(PoolUpdatedEvent), description: "更新了矿池后")]
    public class PoolUpdatedEvent : DomainEvent<IPool> {
        public PoolUpdatedEvent(IPool source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(PoolRemovedEvent), description: "移除了矿池后")]
    public class PoolRemovedEvent : DomainEvent<IPool> {
        public PoolRemovedEvent(IPool source) : base(source) {
        }
    }
    #endregion

    #region CoinKernel Messages
    [MessageType(messageType: typeof(AddCoinKernelCommand), description: "添加币种级内核")]
    public class AddCoinKernelCommand : AddEntityCommand<ICoinKernel> {
        public AddCoinKernelCommand(ICoinKernel input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(UpdateCoinKernelCommand), description: "更新币种级内核")]
    public class UpdateCoinKernelCommand : UpdateEntityCommand<ICoinKernel> {
        public UpdateCoinKernelCommand(ICoinKernel input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(RemoveCoinKernelCommand), description: "移除币种级内核")]
    public class RemoveCoinKernelCommand : RemoveEntityCommand {
        public RemoveCoinKernelCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(messageType: typeof(CoinKernelAddedEvent), description: "添加了币种级内核后")]
    public class CoinKernelAddedEvent : DomainEvent<ICoinKernel> {
        public CoinKernelAddedEvent(ICoinKernel source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(CoinKernelUpdatedEvent), description: "更新了币种级内核后")]
    public class CoinKernelUpdatedEvent : DomainEvent<ICoinKernel> {
        public CoinKernelUpdatedEvent(ICoinKernel source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(CoinKernelRemovedEvent), description: "移除了币种级内核后")]
    public class CoinKernelRemovedEvent : DomainEvent<ICoinKernel> {
        public CoinKernelRemovedEvent(ICoinKernel source) : base(source) {
        }
    }
    #endregion

    #region PoolKernel Messages
    [MessageType(messageType: typeof(AddPoolKernelCommand), description: "添加矿池级内核")]
    public class AddPoolKernelCommand : UpdateEntityCommand<IPoolKernel> {
        public AddPoolKernelCommand(IPoolKernel input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(UpdatePoolKernelCommand), description: "更新矿池级内核")]
    public class UpdatePoolKernelCommand : UpdateEntityCommand<IPoolKernel> {
        public UpdatePoolKernelCommand(IPoolKernel input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(RemovePoolKernelCommand), description: "移除矿池级内核")]
    public class RemovePoolKernelCommand : RemoveEntityCommand {
        public RemovePoolKernelCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(messageType: typeof(PoolKernelAddedEvent), description: "添加了矿池级内核后")]
    public class PoolKernelAddedEvent : DomainEvent<IPoolKernel> {
        public PoolKernelAddedEvent(IPoolKernel source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(PoolKernelUpdatedEvent), description: "更新了矿池级内核后")]
    public class PoolKernelUpdatedEvent : DomainEvent<IPoolKernel> {
        public PoolKernelUpdatedEvent(IPoolKernel source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(PoolKernelRemovedEvent), description: "移除了矿池级内核后")]
    public class PoolKernelRemovedEvent : DomainEvent<IPoolKernel> {
        public PoolKernelRemovedEvent(IPoolKernel source) : base(source) {
        }
    }
    #endregion

    #region Kernel Messages
    [MessageType(messageType: typeof(AddKernelCommand), description: "添加内核")]
    public class AddKernelCommand : AddEntityCommand<IKernel> {
        public AddKernelCommand(IKernel input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(UpdateKernelCommand), description: "更新内核")]
    public class UpdateKernelCommand : UpdateEntityCommand<IKernel> {
        public UpdateKernelCommand(IKernel input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(RemoveKernelCommand), description: "删除内核")]
    public class RemoveKernelCommand : RemoveEntityCommand {
        public RemoveKernelCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(messageType: typeof(KernelAddedEvent), description: "添加了内核后")]
    public class KernelAddedEvent : DomainEvent<IKernel> {
        public KernelAddedEvent(IKernel source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(KernelUpdatedEvent), description: "更新了内核后")]
    public class KernelUpdatedEvent : DomainEvent<IKernel> {
        public KernelUpdatedEvent(IKernel source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(KernelRemovedEvent), description: "删除了内核后")]
    public class KernelRemovedEvent : DomainEvent<IKernel> {
        public KernelRemovedEvent(IKernel source) : base(source) {
        }
    }
    #endregion

    #region KernelInput Messages
    [MessageType(messageType: typeof(AddKernelInputCommand), description: "添加内核输入组")]
    public class AddKernelInputCommand : AddEntityCommand<IKernelInput> {
        public AddKernelInputCommand(IKernelInput input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(UpdateKernelInputCommand), description: "更新内核输入组")]
    public class UpdateKernelInputCommand : UpdateEntityCommand<IKernelInput> {
        public UpdateKernelInputCommand(IKernelInput input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(RemoveKernelInputCommand), description: "移除内核输入组")]
    public class RemoveKernelInputCommand : RemoveEntityCommand {
        public RemoveKernelInputCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(messageType: typeof(KernelInputAddedEvent), description: "添加了内核输入组后")]
    public class KernelInputAddedEvent : DomainEvent<IKernelInput> {
        public KernelInputAddedEvent(IKernelInput source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(KernelInputUpdatedEvent), description: "更新了内核输入组后")]
    public class KernelInputUpdatedEvent : DomainEvent<IKernelInput> {
        public KernelInputUpdatedEvent(IKernelInput source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(KernelInputRemovedEvent), description: "移除了内核输入组后")]
    public class KernelInputRemovedEvent : DomainEvent<IKernelInput> {
        public KernelInputRemovedEvent(IKernelInput source) : base(source) {
        }
    }
    #endregion

    #region KernelOutput Messages
    [MessageType(messageType: typeof(AddKernelOutputCommand), description: "添加内核输出组")]
    public class AddKernelOutputCommand : AddEntityCommand<IKernelOutput> {
        public AddKernelOutputCommand(IKernelOutput input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(UpdateKernelOutputCommand), description: "更新内核输出组")]
    public class UpdateKernelOutputCommand : UpdateEntityCommand<IKernelOutput> {
        public UpdateKernelOutputCommand(IKernelOutput input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(RemoveKernelOutputCommand), description: "移除内核输出组")]
    public class RemoveKernelOutputCommand : RemoveEntityCommand {
        public RemoveKernelOutputCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(messageType: typeof(KernelOutputAddedEvent), description: "添加了内核输出组后")]
    public class KernelOutputAddedEvent : DomainEvent<IKernelOutput> {
        public KernelOutputAddedEvent(IKernelOutput source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(KernelOutputUpdatedEvent), description: "更新了内核输出组后")]
    public class KernelOutputUpdatedEvent : DomainEvent<IKernelOutput> {
        public KernelOutputUpdatedEvent(IKernelOutput source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(KernelOutputRemovedEvent), description: "移除了内核输出组后")]
    public class KernelOutputRemovedEvent : DomainEvent<IKernelOutput> {
        public KernelOutputRemovedEvent(IKernelOutput source) : base(source) {
        }
    }
    #endregion

    #region KernelOutputFilter Messages
    [MessageType(messageType: typeof(AddKernelOutputFilterCommand), description: "添加内核输出过滤器")]
    public class AddKernelOutputFilterCommand : AddEntityCommand<IKernelOutputFilter> {
        public AddKernelOutputFilterCommand(IKernelOutputFilter input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(UpdateKernelOutputFilterCommand), description: "更新内核输出过滤器")]
    public class UpdateKernelOutputFilterCommand : UpdateEntityCommand<IKernelOutputFilter> {
        public UpdateKernelOutputFilterCommand(IKernelOutputFilter input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(RemoveKernelOutputFilterCommand), description: "移除内核输出过滤器")]
    public class RemoveKernelOutputFilterCommand : RemoveEntityCommand {
        public RemoveKernelOutputFilterCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(messageType: typeof(KernelOutputFilterAddedEvent), description: "添加了内核输出过滤器后")]
    public class KernelOutputFilterAddedEvent : DomainEvent<IKernelOutputFilter> {
        public KernelOutputFilterAddedEvent(IKernelOutputFilter source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(KernelOutputFilterUpdatedEvent), description: "更新了内核输出过滤器后")]
    public class KernelOutputFilterUpdatedEvent : DomainEvent<IKernelOutputFilter> {
        public KernelOutputFilterUpdatedEvent(IKernelOutputFilter source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(KernelOutputFilterRemovedEvent), description: "移除了内核输出过滤器后")]
    public class KernelOutputFilterRemovedEvent : DomainEvent<IKernelOutputFilter> {
        public KernelOutputFilterRemovedEvent(IKernelOutputFilter source) : base(source) {
        }
    }
    #endregion

    #region KernelOutputTranslater Messages
    [MessageType(messageType: typeof(AddKernelOutputTranslaterCommand), description: "添加内核输出翻译器")]
    public class AddKernelOutputTranslaterCommand : AddEntityCommand<IKernelOutputTranslater> {
        public AddKernelOutputTranslaterCommand(IKernelOutputTranslater input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(UpdateKernelOutputTranslaterCommand), description: "更新内核输出翻译器")]
    public class UpdateKernelOutputTranslaterCommand : UpdateEntityCommand<IKernelOutputTranslater> {
        public UpdateKernelOutputTranslaterCommand(IKernelOutputTranslater input) : base(input) {
        }
    }

    [MessageType(messageType: typeof(RemoveKernelOutputTranslaterCommand), description: "移除内核输出翻译器")]
    public class RemoveKernelOutputTranslaterCommand : RemoveEntityCommand {
        public RemoveKernelOutputTranslaterCommand(Guid entityId) : base(entityId) {
        }
    }

    [MessageType(messageType: typeof(KernelOutputTranslaterAddedEvent), description: "添加了内核输出翻译器后")]
    public class KernelOutputTranslaterAddedEvent : DomainEvent<IKernelOutputTranslater> {
        public KernelOutputTranslaterAddedEvent(IKernelOutputTranslater source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(KernelOutputTranslaterUpdatedEvent), description: "更新了内核输出翻译器后")]
    public class KernelOutputTranslaterUpdatedEvent : DomainEvent<IKernelOutputTranslater> {
        public KernelOutputTranslaterUpdatedEvent(IKernelOutputTranslater source) : base(source) {
        }
    }

    [MessageType(messageType: typeof(KernelOutputTranslaterRemovedEvent), description: "移除了内核输出翻译器后")]
    public class KernelOutputTranslaterRemovedEvent : DomainEvent<IKernelOutputTranslater> {
        public KernelOutputTranslaterRemovedEvent(IKernelOutputTranslater source) : base(source) {
        }
    }
    #endregion
}
