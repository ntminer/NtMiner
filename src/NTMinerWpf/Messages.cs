using NTMiner.Bus;
using NTMiner.Vms;

namespace NTMiner {
    [MessageType(description: "打开装饰器窗口")]
    public class ShowContainerWindowCommand : Cmd {
        public ShowContainerWindowCommand(ContainerWindowViewModel vm) {
            this.Vm = vm;
        }

        public ContainerWindowViewModel Vm { get; private set; }
    }
}
