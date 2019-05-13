using NTMiner.Core;
using NTMiner.Vms;

namespace NTMiner {
    public partial class AppContext {
        private static void Link() {
            VirtualRoot.Window<EnvironmentVariableEditCommand>(LogEnum.DevConsole,
                action: message => {
                    UIThread.Execute(() => {
                        Views.Ucs.EnvironmentVariableEdit.ShowWindow(message.CoinKernelVm, message.EnvironmentVariable);
                    });
                });
        }

        public static class ShowWindow {
            public static void InputSegmentEdit(CoinKernelViewModel coinKernelVm, InputSegment segment) {
                Views.Ucs.InputSegmentEdit.ShowWindow(coinKernelVm, segment);
            }

            public static void CoinKernelEdit(FormType formType, CoinKernelViewModel source) {
                Views.Ucs.CoinKernelEdit.ShowWindow(formType, source);
            }

            public static void CoinEdit(FormType formType, CoinViewModel source) {
                Views.Ucs.CoinEdit.ShowWindow(formType, source);
            }

            public static void ColumnsShowEdit(FormType formType, ColumnsShowViewModel source) {
                Views.Ucs.ColumnsShowEdit.ShowWindow(formType, source);
            }

            public static void ContainerWindow(ContainerWindowViewModel vm) {
                Views.ContainerWindow window = Views.ContainerWindow.GetWindow(vm);
                window?.ShowWindow();
            }

            public static void SpeedCharts(GpuSpeedViewModel gpuSpeedVm = null) {
                Views.Ucs.SpeedCharts.ShowWindow(gpuSpeedVm);
            }

            public static void GroupEdit(FormType formType, GroupViewModel source) {
                Views.Ucs.GroupEdit.ShowWindow(formType, source);
            }

            public static void KernelInputEdit(FormType formType, KernelInputViewModel source) {
                Views.Ucs.KernelInputEdit.ShowWindow(formType, source);
            }

            public static void KernelOutputFilterEdit(FormType formType, KernelOutputFilterViewModel source) {
                Views.Ucs.KernelOutputFilterEdit.ShowWindow(formType, source);
            }

            public static void KernelOutputTranslaterEdit(FormType formType, KernelOutputTranslaterViewModel source) {
                Views.Ucs.KernelOutputTranslaterEdit.ShowWindow(formType, source);
            }

            public static void KernelOutputEdit(FormType formType, KernelOutputViewModel source) {
                Views.Ucs.KernelOutputEdit.ShowWindow(formType, source);
            }

            public static void PackagesWindow() {
                Views.PackagesWindow.ShowWindow();
            }

            public static void KernelEdit(FormType formType, KernelViewModel source) {
                Views.Ucs.KernelEdit.ShowWindow(formType, source);
            }

            public static void LogColor() {
                Views.Ucs.LogColor.ShowWindow();
            }

            public static void MinerClientSetting(MinerClientSettingViewModel vm) {
                Views.Ucs.MinerClientSetting.ShowWindow(vm);
            }

            public static void MinerNamesSeter(MinerNamesSeterViewModel vm) {
                Views.Ucs.MinerNamesSeter.ShowWindow(vm);
            }

            public static void GpuProfilesPage(MinerClientsWindowViewModel minerClientsWindowVm) {
                Views.Ucs.GpuProfilesPage.ShowWindow(minerClientsWindowVm);
            }

            public static void MinerClientAdd() {
                Views.Ucs.MinerClientAdd.ShowWindow();
            }

            public static void MinerGroupEdit(FormType formType, MinerGroupViewModel source) {
                Views.Ucs.MinerGroupEdit.ShowWindow(formType, source);
            }

            public static void MineWorkEdit(FormType formType, MineWorkViewModel source) {
                Views.Ucs.MineWorkEdit.ShowWindow(formType, source);
            }

            public static void OverClockDataEdit(FormType formType, OverClockDataViewModel source) {
                Views.Ucs.OverClockDataEdit.ShowWindow(formType, source);
            }

            public static void PackageEdit(FormType formType, PackageViewModel source) {
                Views.Ucs.PackageEdit.ShowWindow(formType, source);
            }

            public static void PoolKernelEdit(FormType formType, PoolKernelViewModel source) {
                Views.Ucs.PoolKernelEdit.ShowWindow(formType, source);
            }

            public static void PoolEdit(FormType formType, PoolViewModel source) {
                Views.Ucs.PoolEdit.ShowWindow(formType, source);
            }

            public static void ControlCenterHostConfig() {
                Views.Ucs.ControlCenterHostConfig.ShowWindow();
            }

            public static void SysDicItemEdit(FormType formType, SysDicItemViewModel source) {
                Views.Ucs.SysDicItemEdit.ShowWindow(formType, source);
            }

            public static void SysDicEdit(FormType formType, SysDicViewModel source) {
                Views.Ucs.SysDicEdit.ShowWindow(formType, source);
            }

            public static void UserEdit(FormType formType, UserViewModel source) {
                Views.Ucs.UserEdit.ShowWindow(formType, source);
            }

            public static void WalletEdit(FormType formType, WalletViewModel source) {
                Views.Ucs.WalletEdit.ShowWindow(formType, source);
            }
        }
    }
}
