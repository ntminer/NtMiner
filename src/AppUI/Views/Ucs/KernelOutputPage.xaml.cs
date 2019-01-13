using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelOutputPage : UserControl {
        public static void ShowWindow() {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                IconName = "Icon_KernelOutput",
                CloseVisible = System.Windows.Visibility.Visible,
                FooterVisible = System.Windows.Visibility.Collapsed,
                Width = 860,
                Height = 520
            }, 
            ucFactory: (window) => new KernelOutputPage());
        }

        private KernelOutputPageViewModel Vm {
            get {
                return (KernelOutputPageViewModel)this.DataContext;
            }
        }

        public KernelOutputPage() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
            //HashSet<string> codes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            //foreach (var kernel in NTMinerRoot.Current.KernelSet) {
            //    if (codes.Contains(kernel.Code)) {
            //        continue;
            //    }
            //    codes.Add(kernel.Code);
            //    Guid kernelOutputId = Guid.NewGuid();
            //    Global.Execute(new AddKernelOutputCommand(new KernelOutputViewModel(kernelOutputId) {
            //        Name = kernel.FullName,
            //        AcceptSharePattern = kernel.AcceptSharePattern,
            //        DualAcceptSharePattern = kernel.DualAcceptSharePattern,
            //        DualGpuSpeedPattern = kernel.DualGpuSpeedPattern,
            //        DualRejectPercentPattern = kernel.DualRejectPercentPattern,
            //        DualRejectSharePattern = kernel.DualRejectSharePattern,
            //        DualTotalSharePattern = kernel.DualTotalSharePattern,
            //        DualTotalSpeedPattern = kernel.DualTotalSpeedPattern,
            //        GpuSpeedPattern = kernel.GpuSpeedPattern,
            //        RejectPercentPattern = kernel.RejectPercentPattern,
            //        RejectSharePattern = kernel.RejectSharePattern,
            //        TotalSharePattern = kernel.TotalSharePattern,
            //        TotalSpeedPattern = kernel.TotalSpeedPattern
            //    }));
            //    var translaters = NTMinerRoot.Current.KernelOutputTranslaterSet.GetKernelOutputTranslaters(kernel.GetId()).ToList();
            //    foreach (var translater in translaters) {
            //        Global.Execute(new UpdateKernelOutputTranslaterCommand(new KernelOutputTranslaterViewModel(translater) {
            //            KernelOutputId = kernelOutputId
            //        }));
            //    }
            //    var filters = NTMinerRoot.Current.KernelOutputFilterSet.GetKernelOutputFilters(kernel.GetId()).ToList();
            //    foreach (var filter in filters) {
            //        Global.Execute(new UpdateKernelOutputFilterCommand(new KernelOutputFilterViewModel(filter) {
            //            KernelOutputId = kernelOutputId
            //        }));
            //    }
            //}
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            DataGrid dg = (DataGrid)sender;
            if (Vm.CurrentKernelOutputVm != null) {
                Vm.CurrentKernelOutputVm.Edit.Execute(null);
            }
        }
    }
}
