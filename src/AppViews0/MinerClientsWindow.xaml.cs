using NTMiner.Core;
using NTMiner.Vms;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class MinerClientsWindow : BlankWindow {
        private static MinerClientsWindow _instance = null;
        public static MinerClientsWindow ShowWindow() {
            if (_instance == null) {
                _instance = new MinerClientsWindow();
            }
            _instance.Show();
            if (_instance.WindowState == WindowState.Minimized) {
                _instance.WindowState = WindowState.Normal;
            }
            _instance.Activate();
            return _instance;
        }

        public MinerClientsWindowViewModel Vm {
            get {
                return AppContext.Instance.MinerClientsWindowVm;
            }
        }

        private MinerClientsWindow() {
            Width = SystemParameters.FullPrimaryScreenWidth * 0.95;
            Height = SystemParameters.FullPrimaryScreenHeight * 0.95;
            this.DataContext = Vm;
            this.DataContext = AppContext.Instance.MinerClientsWindowVm;
            InitializeComponent();
            this.TbUcName.Text = nameof(MinerClientsWindow);
            DateTime lastGetServerMessageOn = DateTime.MinValue;
            this.ServerMessagesUc.IsVisibleChanged += (sender, e) => {
                VirtualRoot.SetIsServerMessagesVisible(this.ServerMessagesUc.IsVisible);
                if (this.ServerMessagesUc.IsVisible) {
                    if (lastGetServerMessageOn.AddSeconds(10) < DateTime.Now) {
                        lastGetServerMessageOn = DateTime.Now;
                        VirtualRoot.Execute(new LoadNewServerMessageCommand());
                    }
                }
            };
            this.AddEventPath<Per1SecondEvent>("刷新倒计时秒表", LogEnum.None,
                action: message => {
                    var minerClients = Vm.MinerClients.ToArray();
                    if (Vm.CountDown > 0) {
                        Vm.CountDown = Vm.CountDown - 1;
                        foreach (var item in minerClients) {
                            item.OnPropertyChanged(nameof(item.LastActivedOnText));
                        }
                    }
                }, location: this.GetType());
            this.AddEventPath<Per10SecondEvent>("周期刷新在线客户端列表", LogEnum.DevConsole,
                action: message => {
                    AppContext.Instance.MinerClientsWindowVm.QueryMinerClients();
                }, location: this.GetType());
            NotiCenterWindow.Instance.Bind(this);
            AppContext.Instance.MinerClientsWindowVm.QueryMinerClients();
        }

        protected override void OnClosing(CancelEventArgs e) {
            VirtualRoot.Execute(new SetServerAppSettingsCommand(
                new AppSettingData[]{
                        new AppSettingData {
                            Key = NTKeyword.FrozenColumnCountAppSettingKey,
                            Value = Vm.FrozenColumnCount
                        },new AppSettingData {
                            Key = NTKeyword.MaxTempAppSettingKey,
                            Value = Vm.MaxTemp
                        },new AppSettingData {
                            Key = NTKeyword.MinTempAppSettingKey,
                            Value = Vm.MinTemp
                        },new AppSettingData {
                            Key = NTKeyword.RejectPercentAppSettingKey,
                            Value = Vm.RejectPercent
                        }
            }));
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e) {
            _instance = null;
            base.OnClosed(e);
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            WpfUtil.ScrollViewer_PreviewMouseDown(sender, e);
        }

        private void MinerClientUcScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            WpfUtil.ScrollViewer_PreviewMouseDown(sender, e, isLeftBar: true);
        }

        private void ButtonLeftCoin_Click(object sender, RoutedEventArgs e) {
            double offset = ColumnsShowScrollView.ContentHorizontalOffset - ColumnsShowScrollView.ViewportWidth;
            ColumnsShowScrollView.ScrollToHorizontalOffset(offset);
            ButtonLeft.IsEnabled = offset > 0;
            ButtonRight.IsEnabled = offset < ColumnsShowScrollView.ScrollableWidth;
        }

        private void ButtonRightCoin_Click(object sender, RoutedEventArgs e) {
            double offset = ColumnsShowScrollView.ContentHorizontalOffset + ColumnsShowScrollView.ViewportWidth;
            ColumnsShowScrollView.ScrollToHorizontalOffset(offset);
            ButtonLeft.IsEnabled = offset > 0;
            ButtonRight.IsEnabled = offset < ColumnsShowScrollView.ScrollableWidth;
        }
    }
}
