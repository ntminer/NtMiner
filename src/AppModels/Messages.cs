using NTMiner.Hub;
using NTMiner.Core;
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
    public class EnableWindowsRemoteDesktopCommand : Cmd {
        public EnableWindowsRemoteDesktopCommand() {
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

    [MessageType(description: "打开用户列表页")]
    public class ShowUserPageCommand : Cmd {
        public ShowUserPageCommand() {
        }
    }

    [MessageType(description: "打开超频菜谱列表页")]
    public class ShowOverClockDataPageCommand : Cmd {
        public ShowOverClockDataPageCommand() {
        }
    }

    [MessageType(description: "打开NTMiner钱包列表页")]
    public class ShowNTMinerWalletPageCommand : Cmd {
        public ShowNTMinerWalletPageCommand() {
        }
    }

    [MessageType(description: "打开群控算力图表窗口")]
    public class ShowChartsWindowCommand : Cmd {
        public ShowChartsWindowCommand() {
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

    [MessageType(description: "打开组界面")]
    public class ShowGroupPageCommand : Cmd {
        public ShowGroupPageCommand() {
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

    [MessageType(description: "打开列显页面")]
    public class ShowColumnsShowPageCommand : Cmd {
        public ShowColumnsShowPageCommand() {
        }
    }

    [MessageType(description: "打开关于页面")]
    public class ShowAboutPageCommand : Cmd {
        public ShowAboutPageCommand() {
        }
    }

    [MessageType(description: "打开升级器设置页面")]
    public class ShowNTMinerUpdaterConfigCommand : Cmd {
        public ShowNTMinerUpdaterConfigCommand() {
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
    public class EnvironmentVariableEditCommand : Cmd {
        public EnvironmentVariableEditCommand(CoinKernelViewModel coinKernelVm, EnvironmentVariable environmentVariable) {
            this.CoinKernelVm = coinKernelVm;
            this.EnvironmentVariable = environmentVariable;
        }

        public CoinKernelViewModel CoinKernelVm { get; private set; }
        public EnvironmentVariable EnvironmentVariable { get; private set; }
    }

    [MessageType(description: "打开内核输入片段编辑界面")]
    public class InputSegmentEditCommand : Cmd {
        public InputSegmentEditCommand(CoinKernelViewModel coinKernelVm, InputSegmentViewModel segment) {
            this.CoinKernelVm = coinKernelVm;
            this.Segment = segment;
        }

        public CoinKernelViewModel CoinKernelVm { get; private set; }
        public InputSegmentViewModel Segment { get; private set; }
    }

    [MessageType(description: "打开币种级内核编辑界面")]
    public class CoinKernelEditCommand : Cmd {
        public CoinKernelEditCommand(FormType formType, CoinKernelViewModel source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public CoinKernelViewModel Source { get; private set; }
    }

    [MessageType(description: "打开币种编辑界面")]
    public class CoinEditCommand : Cmd {
        public CoinEditCommand(FormType formType, CoinViewModel source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public CoinViewModel Source { get; private set; }
    }

    [MessageType(description: "打开列显编辑界面")]
    public class ColumnsShowEditCommand : Cmd {
        public ColumnsShowEditCommand(FormType formType, ColumnsShowViewModel source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public ColumnsShowViewModel Source { get; private set; }
    }

    [MessageType(description: "打开算力图界面")]
    public class ShowSpeedChartsCommand : Cmd {
        public ShowSpeedChartsCommand(GpuSpeedViewModel gpuSpeedVm = null) {
            this.GpuSpeedVm = gpuSpeedVm;
        }

        public GpuSpeedViewModel GpuSpeedVm { get; private set; }
    }

    [MessageType(description: "打开币组编辑界面")]
    public class GroupEditCommand : Cmd {
        public GroupEditCommand(FormType formType, GroupViewModel source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public GroupViewModel Source { get; private set; }
    }

    [MessageType(description: "打开文件书写器列表页")]
    public class ShowFileWriterPageCommand : Cmd {
        public ShowFileWriterPageCommand() {
        }
    }

    [MessageType(description: "打开文件书写器编辑界面")]
    public class FileWriterEditCommand : Cmd {
        public FileWriterEditCommand(FormType formType, FileWriterViewModel source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public FileWriterViewModel Source { get; private set; }
    }

    [MessageType(description: "打开命令行片段书写器列表页")]
    public class ShowFragmentWriterPageCommand : Cmd {
        public ShowFragmentWriterPageCommand() {
        }
    }

    [MessageType(description: "打开命令行片段书写器编辑界面")]
    public class FragmentWriterEditCommand : Cmd {
        public FragmentWriterEditCommand(FormType formType, FragmentWriterViewModel source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public FragmentWriterViewModel Source { get; private set; }
    }

    [MessageType(description: "打开服务器消息编辑界面")]
    public class ServerMessageEditCommand : Cmd {
        public ServerMessageEditCommand(FormType formType, ServerMessageViewModel source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public ServerMessageViewModel Source { get; private set; }
    }

    [MessageType(description: "打开内核输入编辑界面")]
    public class KernelInputEditCommand : Cmd {
        public KernelInputEditCommand(FormType formType, KernelInputViewModel source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public KernelInputViewModel Source { get; private set; }
    }

    [MessageType(description: "打开内核输出关键字编辑界面")]
    public class KernelOutputKeywordEditCommand : Cmd {
        public KernelOutputKeywordEditCommand(FormType formType, KernelOutputKeywordViewModel source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public KernelOutputKeywordViewModel Source { get; private set; }
    }

    [MessageType(description: "打开内核输出翻译器编辑界面")]
    public class KernelOutputTranslaterEditCommand : Cmd {
        public KernelOutputTranslaterEditCommand(FormType formType, KernelOutputTranslaterViewModel source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public KernelOutputTranslaterViewModel Source { get; private set; }
    }

    [MessageType(description: "打开内核输出编辑界面")]
    public class KernelOutputEditCommand : Cmd {
        public KernelOutputEditCommand(FormType formType, KernelOutputViewModel source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public KernelOutputViewModel Source { get; private set; }
    }

    [MessageType(description: "打开内核包窗口")]
    public class ShowPackagesWindowCommand : Cmd {
        public ShowPackagesWindowCommand() {
        }
    }

    [MessageType(description: "打开内核编辑界面")]
    public class KernelEditCommand : Cmd {
        public KernelEditCommand(FormType formType, KernelViewModel source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public KernelViewModel Source { get; private set; }
    }

    [MessageType(description: "打开挖矿端远程设置界面")]
    public class ShowMinerClientSettingCommand : Cmd {
        public ShowMinerClientSettingCommand(MinerClientSettingViewModel vm) {
            this.Vm = vm;
        }

        public MinerClientSettingViewModel Vm { get; private set; }
    }

    [MessageType(description: "打开群控矿机列表页")]
    public class ShowMinerClientsWindowCommand : Cmd {
        public ShowMinerClientsWindowCommand() {
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

    [MessageType(description: "打开ETH反抽水配置页")]
    public class ShowEthNoDevFeeCommand : Cmd {
        public ShowEthNoDevFeeCommand() { }
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

    [MessageType(description: "打开作群控名设置界面")]
    public class ShowMinerNamesSeterCommand : Cmd {
        public ShowMinerNamesSeterCommand(MinerNamesSeterViewModel vm) {
            this.Vm = vm;
        }

        public MinerNamesSeterViewModel Vm { get; private set; }
    }

    [MessageType(description: "打开群控超频界面")]
    public class ShowGpuProfilesPageCommand : Cmd {
        public ShowGpuProfilesPageCommand(MinerClientsWindowViewModel minerClientsWindowVm) {
            this.MinerClientsWindowVm = minerClientsWindowVm;
        }

        public MinerClientsWindowViewModel MinerClientsWindowVm { get; private set; }
    }

    [MessageType(description: "打开添加矿机界面")]
    public class ShowMinerClientAddCommand : Cmd {
        public ShowMinerClientAddCommand() {
        }
    }

    [MessageType(description: "打开矿工组编辑界面")]
    public class MinerGroupEditCommand : Cmd {
        public MinerGroupEditCommand(FormType formType, MinerGroupViewModel source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public MinerGroupViewModel Source { get; private set; }
    }

    [MessageType(description: "打开NTMiner钱包编辑界面")]
    public class NTMinerWalletEditCommand : Cmd {
        public NTMinerWalletEditCommand(FormType formType, NTMinerWalletViewModel source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public NTMinerWalletViewModel Source { get; private set; }
    }

    [MessageType(description: "打开作业编辑界面")]
    public class MineWorkEditCommand : Cmd {
        public MineWorkEditCommand(FormType formType, MineWorkViewModel source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public MineWorkViewModel Source { get; private set; }
    }

    [MessageType(description: "打开超频菜谱编辑界面")]
    public class OverClockDataEditCommand : Cmd {
        public OverClockDataEditCommand(FormType formType, OverClockDataViewModel source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public OverClockDataViewModel Source { get; private set; }
    }

    [MessageType(description: "打开内核包编辑界面")]
    public class PackageEditCommand : Cmd {
        public PackageEditCommand(FormType formType, PackageViewModel source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public PackageViewModel Source { get; private set; }
    }

    [MessageType(description: "打开矿池级内核编辑界面")]
    public class PoolKernelEditCommand : Cmd {
        public PoolKernelEditCommand(FormType formType, PoolKernelViewModel source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public PoolKernelViewModel Source { get; private set; }
    }

    [MessageType(description: "打开矿池编辑界面")]
    public class PoolEditCommand : Cmd {
        public PoolEditCommand(FormType formType, PoolViewModel source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public PoolViewModel Source { get; private set; }
    }

    [MessageType(description: "打开字典项编辑界面")]
    public class SysDicItemEditCommand : Cmd {
        public SysDicItemEditCommand(FormType formType, SysDicItemViewModel source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public SysDicItemViewModel Source { get; private set; }
    }

    [MessageType(description: "打开字典编辑界面")]
    public class SysDicEditCommand : Cmd {
        public SysDicEditCommand(FormType formType, SysDicViewModel source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public SysDicViewModel Source { get; private set; }
    }

    [MessageType(description: "打开内核输出关键字列表页")]
    public class ShowKernelOutputKeywordsCommand : Cmd {
        public ShowKernelOutputKeywordsCommand() {
        }
    }

    [MessageType(description: "打开用户编辑界面")]
    public class UserEditCommand : Cmd {
        public UserEditCommand(FormType formType, UserViewModel source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public UserViewModel Source { get; private set; }
    }

    [MessageType(description: "打开钱包地址编辑界面")]
    public class WalletEditCommand : Cmd {
        public WalletEditCommand(FormType formType, WalletViewModel source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public WalletViewModel Source { get; private set; }
    }
}
