using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Linq;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class KernelCopySourceSelect : UserControl {
        public static void ShowWindow(KernelViewModel kernel, string tag) {
            KernelCopySourceSelect uc = null;
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "选择内核",
                IconName = "Icon_Kernel",
                IsDialogWindow = true,
                CloseVisible = System.Windows.Visibility.Visible
            }, ucFactory: (window) => {
                uc = new KernelCopySourceSelect(new KernelCopySourceSelectViewModel(kernel));
                return uc;
            }, fixedSize: true);
            if (uc != null) {
                if (uc.Vm.SelectedKernelVm != null) {
                    var sourceVm = uc.Vm.SelectedKernelVm;
                    DialogWindow.ShowDialog(message: $"您确定从{sourceVm.FullName}复制内容吗？", title: "确认", onYes: () => {
                        if (tag == "basic") {
                            kernel.HelpArg = sourceVm.HelpArg;
                            kernel.Args = sourceVm.Args;
                            kernel.TotalSpeedPattern = sourceVm.TotalSpeedPattern;
                            kernel.TotalSharePattern = sourceVm.TotalSharePattern;
                            kernel.AcceptSharePattern = sourceVm.AcceptSharePattern;
                            kernel.RejectSharePattern = sourceVm.RejectSharePattern;
                            kernel.RejectPercentPattern = sourceVm.RejectPercentPattern;
                            kernel.GpuSpeedPattern = sourceVm.GpuSpeedPattern;
                            kernel.IsSupportDualMine = sourceVm.IsSupportDualMine;
                            kernel.DualFullArgs = sourceVm.DualFullArgs;

                            kernel.DualTotalSpeedPattern = sourceVm.DualTotalSpeedPattern;
                            kernel.DualTotalSharePattern = sourceVm.DualTotalSharePattern;
                            kernel.DualAcceptSharePattern = sourceVm.DualAcceptSharePattern;
                            kernel.DualRejectSharePattern = sourceVm.DualRejectSharePattern;
                            kernel.DualRejectPercentPattern = sourceVm.DualRejectPercentPattern;
                            kernel.DualGpuSpeedPattern = sourceVm.DualGpuSpeedPattern;
                        }
                        else if (tag == "output") {
                            var source = NTMinerRoot.Current.KernelOutputTranslaterSet.GetKernelOutputTranslaters(sourceVm.Id);
                            foreach (var item in source) {
                                var list = NTMinerRoot.Current.KernelOutputTranslaterSet.GetKernelOutputTranslaters(kernel.Id);
                                var exist = list.FirstOrDefault(a => a.RegexPattern == item.RegexPattern && a.IsPre == item.IsPre);
                                if (exist != null) {
                                    Global.Execute(new UpdateKernelOutputTranslaterCommand(new KernelOutputTranslaterViewModel(item) {
                                        Id = exist.GetId(),
                                        KernelId = kernel.Id
                                    }));
                                }
                                else {
                                    Global.Execute(new AddKernelOutputTranslaterCommand(new KernelOutputTranslaterViewModel(item) {
                                        Id = Guid.NewGuid(),
                                        KernelId = kernel.Id
                                    }));
                                }
                            }
                            foreach (var item in NTMinerRoot.Current.KernelOutputFilterSet.GetKernelOutputFilters(sourceVm.Id)) {
                                var list = NTMinerRoot.Current.KernelOutputFilterSet.GetKernelOutputFilters(kernel.Id);
                                var exist = list.FirstOrDefault(a => a.RegexPattern == item.RegexPattern);
                                if (exist != null) {
                                    Global.Execute(new UpdateKernelOutputFilterCommand(new KernelOutputFilterViewModel(item) {
                                        Id = exist.GetId(),
                                        KernelId = kernel.Id
                                    }));
                                }
                                else {
                                    Global.Execute(new AddKernelOutputFilterCommand(new KernelOutputFilterViewModel(item) {
                                        Id = Guid.NewGuid(),
                                        KernelId = kernel.Id
                                    }));
                                }
                            }
                        }
                    }, icon: "Icon_Confirm");
                }
            }
        }

        public KernelCopySourceSelectViewModel Vm {
            get {
                return (KernelCopySourceSelectViewModel)this.DataContext;
            }
        }

        public KernelCopySourceSelect(KernelCopySourceSelectViewModel vm) {
            this.DataContext = vm;
            InitializeComponent();
        }
    }
}
