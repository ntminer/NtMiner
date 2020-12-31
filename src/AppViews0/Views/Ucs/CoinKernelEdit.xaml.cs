using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace NTMiner.Views.Ucs {
    public partial class CoinKernelEdit : UserControl {
        public static void ShowWindow(FormType formType, CoinKernelViewModel source) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "币种级参数",
                FormType = formType,
                Width = 700,
                IsMaskTheParent = true,
                IconName = "Icon_Kernel",
                CloseVisible = Visibility.Visible,
                FooterVisible = Visibility.Collapsed
            }, ucFactory: (window) =>
            {
                CoinKernelViewModel vm = new CoinKernelViewModel(source);
                window.BuildCloseWindowOnecePath(vm.Id);
                return new CoinKernelEdit(vm);
            }, fixedSize: true);
        }

        public CoinKernelViewModel Vm { get; private set; }

        public CoinKernelEdit(CoinKernelViewModel vm) {
            this.Vm = vm;
            this.DataContext = vm;
            InitializeComponent();
        }

        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            WpfUtil.DataGrid_MouseDoubleClick<EnvironmentVariable>(sender, e, rowVm => {
                Vm.EditEnvironmentVariable.Execute(rowVm);
            });
        }

        private void DataGridSegments_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            WpfUtil.DataGrid_MouseDoubleClick<InputSegmentViewModel>(sender, e, rowVm => {
                Vm.EditSegment.Execute(rowVm);
            });
        }

        private void ButtonAddFileWriter_Click(object sender, RoutedEventArgs e) {
            PopupFileWriter.Child = new FileWriterSelect(
                new FileWriterSelectViewModel( onOk: selectedResult => {
                    if (selectedResult != null) {
                        var fileWriterIds = new List<Guid>(this.Vm.FileWriterIds) {
                            selectedResult.Id
                        };
                        this.Vm.FileWriterIds = fileWriterIds;
                        PopupFileWriter.IsOpen = false;
                    }
                }) {
                    HideView = new DelegateCommand(() => {
                        PopupFileWriter.IsOpen = false;
                    })
                });
            PopupFileWriter.IsOpen = true;
        }

        private void ButtonAddFragmentWriter_Click(object sender, RoutedEventArgs e) {
            PopupFragmentWriter.Child = new FragmentWriterSelect(
                new FragmentWriterSelectViewModel(onOk: selectedResult => {
                    if (selectedResult != null) {
                        var fileWriterIds = new List<Guid>(this.Vm.FragmentWriterIds) {
                            selectedResult.Id
                        };
                        this.Vm.FragmentWriterIds = fileWriterIds;
                        PopupFragmentWriter.IsOpen = false;
                    }
                }) {
                    HideView = new DelegateCommand(() => {
                        PopupFragmentWriter.IsOpen = false;
                    })
                });
            PopupFragmentWriter.IsOpen = true;
        }
    }
}
