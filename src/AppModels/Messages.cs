using NTMiner.Core;
using NTMiner.Hub;
using NTMiner.MinerStudio.Vms;
using NTMiner.Vms;
using System;
using System.Windows;

namespace NTMiner {
    [MessageType(description: "升级")]
    public class UpgradeCommand : Cmd {
        public UpgradeCommand(string fileName, Action callback) {
            this.FileName = fileName;
            this.Callback = callback;
        }

        public string FileName { get; private set; }
        public Action Callback { get; private set; }
    }

    [MessageType(description: "启用windows远程桌面")]
    public class EnableRemoteDesktopCommand : Cmd {
        public EnableRemoteDesktopCommand() {
        }
    }

    [MessageType(description: "启用或禁用windows开机自动登录")]
    public class EnableOrDisableWindowsAutoLoginCommand : Cmd {
        public EnableOrDisableWindowsAutoLoginCommand() {
        }
    }

    [MessageType(description: "打开收集远程桌面登录名和密码的对话框")]
    public class ShowRemoteDesktopLoginDialogCommand : Cmd {
        public ShowRemoteDesktopLoginDialogCommand(RemoteDesktopLoginViewModel vm) {
            this.Vm = vm;
        }

        public RemoteDesktopLoginViewModel Vm { get; private set; }
    }

    [MessageType(description: "关闭主界面")]
    public class CloseMainWindowCommand : Cmd {
        public CloseMainWindowCommand(bool isAutoNoUi) {
            this.IsAutoNoUi = isAutoNoUi;
        }

        public bool IsAutoNoUi { get; private set; }
    }

    [MessageType(description: "打开内核列表窗口")]
    public class ShowKernelsWindowCommand : Cmd {
        public ShowKernelsWindowCommand() {
        }
    }

    [MessageType(description: "打开集线器页")]
    public class ShowMessagePathIdsCommand : Cmd {
        public ShowMessagePathIdsCommand() {
        }
    }

    [MessageType(description: "打开属性页")]
    public class ShowPropertyCommand : Cmd {
        public ShowPropertyCommand() {
        }
    }

    [MessageType(description: "打开通知样例页")]
    public class ShowNotificationSampleCommand : Cmd {
        public ShowNotificationSampleCommand() {
        }
    }

    [MessageType(description: "打开虚拟内存管理界面")]
    public class ShowVirtualMemoryCommand : Cmd {
        public ShowVirtualMemoryCommand() {
        }
    }

    [MessageType(description: "打开系统字典界面")]
    public class ShowSysDicPageCommand : Cmd {
        public ShowSysDicPageCommand() {
        }
    }

    [MessageType(description: "打开币组界面")]
    public class ShowCoinGroupsCommand : Cmd {
        public ShowCoinGroupsCommand() {
        }
    }

    [MessageType(description: "打开品牌打码页")]
    public class ShowTagBrandCommand : Cmd {
        public ShowTagBrandCommand() { }
    }

    [MessageType(description: "打开币种页面")]
    public class ShowCoinPageCommand : Cmd {
        public ShowCoinPageCommand(CoinViewModel currentCoin, string tabType) {
            this.CurrentCoin = currentCoin;
            this.TabType = tabType;
        }

        public CoinViewModel CurrentCoin { get; private set; }
        public string TabType { get; private set; }
    }

    [MessageType(description: "打开关于页面")]
    public class ShowAboutPageCommand : Cmd {
        public ShowAboutPageCommand() {
        }
    }

    [MessageType(description: "打开矿机雷达程序设置页面")]
    public class ShowMinerClientFinderConfigCommand : Cmd {
        public ShowMinerClientFinderConfigCommand() {
        }
    }

    [MessageType(description: "打开内核输入页面")]
    public class ShowKernelInputPageCommand : Cmd {
        public ShowKernelInputPageCommand() {
        }
    }

    [MessageType(description: "打开内核输出页面")]
    public class ShowKernelOutputPageCommand : Cmd {
        public ShowKernelOutputPageCommand(KernelOutputViewModel selectedKernelOutputVm) {
            this.SelectedKernelOutputVm = selectedKernelOutputVm;
        }

        public KernelOutputViewModel SelectedKernelOutputVm { get; private set; }
    }

    [MessageType(description: "打开环境变量编辑界面")]
    public class EditEnvironmentVariableCommand : Cmd {
        public EditEnvironmentVariableCommand(CoinKernelViewModel coinKernelVm, EnvironmentVariable environmentVariable) {
            this.CoinKernelVm = coinKernelVm;
            this.EnvironmentVariable = environmentVariable;
        }

        public CoinKernelViewModel CoinKernelVm { get; private set; }
        public EnvironmentVariable EnvironmentVariable { get; private set; }
    }

    [MessageType(description: "打开内核输入片段编辑界面")]
    public class EditInputSegmentCommand : Cmd {
        public EditInputSegmentCommand(CoinKernelViewModel coinKernelVm, InputSegmentViewModel segment) {
            this.CoinKernelVm = coinKernelVm;
            this.Segment = segment;
        }

        public CoinKernelViewModel CoinKernelVm { get; private set; }
        public InputSegmentViewModel Segment { get; private set; }
    }

    public abstract class EditCommand<T> : Cmd {
        public EditCommand(FormType formType, T source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public T Source { get; private set; }
    }

    [MessageType(description: "打开币种级内核编辑界面")]
    public class EditCoinKernelCommand : EditCommand<CoinKernelViewModel> {
        public EditCoinKernelCommand(FormType formType, CoinKernelViewModel source) : base(formType, source) {
        }
    }

    [MessageType(description: "打开币种编辑界面")]
    public class EditCoinCommand : EditCommand<CoinViewModel> {
        public EditCoinCommand(FormType formType, CoinViewModel source) : base(formType, source) {
        }
    }

    [MessageType(description: "打开算力图界面")]
    public class ShowSpeedChartsCommand : Cmd {
        public ShowSpeedChartsCommand(GpuSpeedViewModel gpuSpeedVm = null) {
            this.GpuSpeedVm = gpuSpeedVm;
        }

        public GpuSpeedViewModel GpuSpeedVm { get; private set; }
    }

    [MessageType(description: "打开币组编辑界面")]
    public class EditGroupCommand : EditCommand<GroupViewModel> {
        public EditGroupCommand(FormType formType, GroupViewModel source) : base(formType, source) {
        }
    }

    [MessageType(description: "打开文件书写器列表页")]
    public class ShowFileWriterPageCommand : Cmd {
        public ShowFileWriterPageCommand() {
        }
    }

    [MessageType(description: "打开文件书写器编辑界面")]
    public class EditFileWriterCommand : EditCommand<FileWriterViewModel> {
        public EditFileWriterCommand(FormType formType, FileWriterViewModel source) : base(formType, source) {
        }
    }

    [MessageType(description: "打开命令行片段书写器列表页")]
    public class ShowFragmentWriterPageCommand : Cmd {
        public ShowFragmentWriterPageCommand() {
        }
    }

    [MessageType(description: "打开命令行片段书写器编辑界面")]
    public class EditFragmentWriterCommand : EditCommand<FragmentWriterViewModel> {
        public EditFragmentWriterCommand(FormType formType, FragmentWriterViewModel source) : base(formType, source) {
        }
    }

    [MessageType(description: "打开服务器消息编辑界面")]
    public class EditServerMessageCommand : EditCommand<ServerMessageViewModel> {
        public EditServerMessageCommand(FormType formType, ServerMessageViewModel source) : base(formType, source) {
        }
    }

    [MessageType(description: "打开内核输入编辑界面")]
    public class EditKernelInputCommand : EditCommand<KernelInputViewModel> {
        public EditKernelInputCommand(FormType formType, KernelInputViewModel source) : base(formType, source) {
        }
    }

    [MessageType(description: "打开内核输出关键字编辑界面")]
    public class EditKernelOutputKeywordCommand : EditCommand<KernelOutputKeywordViewModel> {
        public EditKernelOutputKeywordCommand(FormType formType, KernelOutputKeywordViewModel source) : base(formType, source) {
        }
    }

    [MessageType(description: "打开内核输出翻译器编辑界面")]
    public class EditKernelOutputTranslaterCommand : EditCommand<KernelOutputTranslaterViewModel> {
        public EditKernelOutputTranslaterCommand(FormType formType, KernelOutputTranslaterViewModel source) : base(formType, source) {
        }
    }

    [MessageType(description: "打开内核输出编辑界面")]
    public class EditKernelOutputCommand : EditCommand<KernelOutputViewModel> {
        public EditKernelOutputCommand(FormType formType, KernelOutputViewModel source) : base(formType, source) {
        }
    }

    [MessageType(description: "打开内核包窗口")]
    public class ShowPackagesWindowCommand : Cmd {
        public ShowPackagesWindowCommand() {
        }
    }

    [MessageType(description: "打开内核编辑界面")]
    public class EditKernelCommand : EditCommand<KernelViewModel> {
        public EditKernelCommand(FormType formType, KernelViewModel source) : base(formType, source) {
        }
    }

    public class ShowFileDownloaderCommand : Cmd {
        public ShowFileDownloaderCommand(
            string downloadFileUrl,
            string fileTitle,
            // window, isSuccess, message, saveFileFullName, etagValue
            Action<Window, bool, string, string> downloadComplete) {
            this.DownloadFileUrl = downloadFileUrl;
            this.FileTitle = fileTitle;
            this.DownloadComplete = downloadComplete;
        }

        public string DownloadFileUrl { get; private set; }
        public string FileTitle { get; private set; }
        public Action<Window, bool, string, string> DownloadComplete { get; private set; }
    }

    [MessageType(description: "打开收益计算器设置页")]
    public class ShowCalcConfigCommand : Cmd {
        public ShowCalcConfigCommand() {
        }
    }

    [MessageType(description: "打开收益计算器")]
    public class ShowCalcCommand : Cmd {
        public ShowCalcCommand(CoinViewModel coinVm) {
            this.CoinVm = coinVm;
        }

        public CoinViewModel CoinVm { get; private set; }
    }

    [MessageType(description: "打开QQ群二维码")]
    public class ShowQQGroupQrCodeCommand : Cmd {
        public ShowQQGroupQrCodeCommand() {
        }
    }

    [MessageType(description: "打开对话界面")]
    public class ShowDialogWindowCommand : Cmd {
        public ShowDialogWindowCommand(string icon = null,
            string title = null,
            string message = null,
            Action onYes = null,
            Action onNo = null) {
            this.Icon = icon;
            this.Title = title;
            this.Message = message;
            this.OnYes = onYes;
            this.OnNo = onNo;
        }
        public string Icon { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public Action OnYes { get; private set; }
        public Action OnNo { get; private set; }
    }

    [MessageType(description: "打开内核包编辑界面")]
    public class EditPackageCommand : EditCommand<PackageViewModel> {
        public EditPackageCommand(FormType formType, PackageViewModel source) : base(formType, source) {
        }
    }

    [MessageType(description: "打开矿池级内核编辑界面")]
    public class EditPoolKernelCommand : EditCommand<PoolKernelViewModel> {
        public EditPoolKernelCommand(FormType formType, PoolKernelViewModel source) : base(formType, source) {
        }
    }

    [MessageType(description: "打开矿池编辑界面")]
    public class EditPoolCommand : EditCommand<PoolViewModel> {
        public EditPoolCommand(FormType formType, PoolViewModel source) : base(formType, source) {
        }
    }

    [MessageType(description: "打开字典项编辑界面")]
    public class EditSysDicItemCommand : EditCommand<SysDicItemViewModel> {
        public EditSysDicItemCommand(FormType formType, SysDicItemViewModel source) : base(formType, source) {
        }
    }

    [MessageType(description: "打开字典编辑界面")]
    public class EditSysDicCommand : EditCommand<SysDicViewModel> {
        public EditSysDicCommand(FormType formType, SysDicViewModel source) : base(formType, source) {
        }
    }

    [MessageType(description: "打开内核输出关键字列表页")]
    public class ShowKernelOutputKeywordsCommand : Cmd {
        public ShowKernelOutputKeywordsCommand() {
        }
    }

    [MessageType(description: "打开用户注册页")]
    public class ShowSignUpPageCommand : Cmd {
        public ShowSignUpPageCommand() { }
    }

    [MessageType(description: "打开钱包地址编辑界面")]
    public class EditWalletCommand : EditCommand<WalletViewModel> {
        public EditWalletCommand(FormType formType, WalletViewModel source) : base(formType, source) {
        }
    }

    [MessageType(description: "添加了币种后")]
    public class CoinVmAddedEvent : VmEventBase<CoinAddedEvent> {
        public CoinVmAddedEvent(CoinAddedEvent evt) : base(evt) {
        }
    }

    [MessageType(description: "移除了币种后")]
    public class CoinVmRemovedEvent : VmEventBase<CoinRemovedEvent> {
        public CoinVmRemovedEvent(CoinRemovedEvent evt) : base(evt) {
        }
    }

    [MessageType(description: "添加了钱包后")]
    public class WalletVmAddedEvent : VmEventBase<WalletAddedEvent> {
        public WalletVmAddedEvent(WalletAddedEvent evt) : base(evt) {
        }
    }

    [MessageType(description: "移除了钱包后")]
    public class WalletVmRemovedEvent : VmEventBase<WalletRemovedEvent> {
        public WalletVmRemovedEvent(WalletRemovedEvent evt) : base(evt) {
        }
    }

    [MessageType(description: "添加了币种级内核后")]
    public class CoinKernelVmAddedEvent : VmEventBase<CoinKernelAddedEvent> {
        public CoinKernelVmAddedEvent(CoinKernelAddedEvent evt) : base(evt) {
        }
    }

    [MessageType(description: "移除了币种级内核后")]
    public class CoinKernelVmRemovedEvent : VmEventBase<CoinKernelRemovedEvent> {
        public CoinKernelVmRemovedEvent(CoinKernelRemovedEvent evt) : base(evt) {
        }
    }
}
