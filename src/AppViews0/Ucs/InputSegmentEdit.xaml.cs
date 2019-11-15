using NTMiner.Core;
using NTMiner.Vms;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class InputSegmentEdit : UserControl {
        public static void ShowWindow(CoinKernelViewModel coinKernelVm, InputSegmentViewModel segment) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "片段",
                IsMaskTheParent = true,
                Width = 500,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) =>
            {
                InputSegmentEditViewModel vm = new InputSegmentEditViewModel(coinKernelVm, segment) {
                    CloseWindow = window.Close
                };
                return new InputSegmentEdit(vm);
            }, fixedSize: true);
        }

        private InputSegmentEditViewModel Vm {
            get {
                return (InputSegmentEditViewModel)this.DataContext;
            }
        }

        public InputSegmentEdit(InputSegmentEditViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
