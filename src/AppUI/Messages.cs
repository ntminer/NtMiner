using NTMiner.Bus;
using NTMiner.Core;
using NTMiner.Vms;

namespace NTMiner {
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
        public InputSegmentEditCommand(CoinKernelViewModel coinKernelVm, InputSegment segment) {
            this.CoinKernelVm = coinKernelVm;
            this.Segment = segment;
        }

        public CoinKernelViewModel CoinKernelVm { get; private set; }
        public InputSegment Segment { get; private set; }
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

    [MessageType(description: "打开装饰器窗口")]
    public class ShowContainerWindowCommand : Cmd {
        public ShowContainerWindowCommand(ContainerWindowViewModel vm) {
            this.Vm = vm;
        }

        public ContainerWindowViewModel Vm { get; private set; }
    }

    [MessageType(description: "打开算力图界面")]
    public class SpeedChartsCommand : Cmd {
        public SpeedChartsCommand(GpuSpeedViewModel gpuSpeedVm = null) {
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

    [MessageType(description: "打开内核输入编辑界面")]
    public class KernelInputEditCommand : Cmd {
        public KernelInputEditCommand(FormType formType, KernelInputViewModel source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public KernelInputViewModel Source { get; private set; }
    }

    [MessageType(description: "打开内核输出过滤器编辑界面")]
    public class KernelOutputFilterEditCommand : Cmd {
        public KernelOutputFilterEditCommand(FormType formType, KernelOutputFilterViewModel source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public KernelOutputFilterViewModel Source { get; private set; }
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

    [MessageType(description: "打开内核日志颜色配置器界面")]
    public class ShowLogColorCommand : Cmd {
        public ShowLogColorCommand() {
        }
    }

    [MessageType(description: "打开挖矿端远程设置界面")]
    public class ShowMinerClientSettingCommand : Cmd {
        public ShowMinerClientSettingCommand(MinerClientSettingViewModel vm) {
            this.Vm = vm;
        }

        public MinerClientSettingViewModel Vm { get; private set; }
    }

    [MessageType(description: "打开作业矿机名设置界面")]
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

    [MessageType(description: "打开群控服务地址设置界面")]
    public class ShowControlCenterHostConfigCommand : Cmd {
        public ShowControlCenterHostConfigCommand() {
        }
    }

    [MessageType(description: "打开字典项编辑界面")]
    public class SysDicItemEditCommand : Cmd {
        public SysDicItemEditCommand(FormType formType, SysDicItemEditCommand source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public SysDicItemEditCommand Source { get; private set; }
    }

    [MessageType(description: "打开字典编辑界面")]
    public class SysDicEditCommand : Cmd {
        public SysDicEditCommand(FormType formType, SysDicEditCommand source) {
            this.FormType = formType;
            this.Source = source;
        }

        public FormType FormType { get; private set; }
        public SysDicEditCommand Source { get; private set; }
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
