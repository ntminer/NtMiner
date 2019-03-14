using LiveCharts;
using MahApps.Metro.Controls;
using NTMiner.Bus;
using NTMiner.MinerServer;
using NTMiner.Vms;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using LiteDB;

namespace NTMiner.Views {
    public partial class ChartsWindow : MetroWindow, IMainWindow {
        private static ChartsWindow s_window = null;
        public static void ShowWindow() {
            if (s_window == null) {
                s_window = new ChartsWindow();
                Application.Current.MainWindow = s_window;
            }
            s_window.Show();
            if (s_window.WindowState == WindowState.Minimized) {
                s_window.WindowState = WindowState.Normal;
            }
            s_window.Activate();
            s_window.RefreshTotalSpeedChart(limit: 60);
        }

        public ChartsWindowViewModel Vm {
            get {
                return (ChartsWindowViewModel)this.DataContext;
            }
        }

        private ChartsWindow() {
            Width = SystemParameters.FullPrimaryScreenWidth * 0.95;
            Height = SystemParameters.FullPrimaryScreenHeight * 0.95;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
            #region 总算力
            DelegateHandler<Per10SecondEvent> refeshTotalSpeedChart = VirtualRoot.On<Per10SecondEvent>(
                "周期刷新总算力图",
                LogEnum.Console,
                action: message => {
                    RefreshTotalSpeedChart(limit: 1);
                });
            this.Unloaded += (object sender, RoutedEventArgs e) => {
                VirtualRoot.UnPath(refeshTotalSpeedChart);
            };
            #endregion
        }

        public void ShowThisWindow() {
            this.Show();
            if (WindowState == WindowState.Minimized) {
                this.WindowState = WindowState.Normal;
            }
            else {
                var oldState = WindowState;
                this.WindowState = WindowState.Minimized;
                this.WindowState = oldState;
            }
            this.Activate();
        }

        protected override void OnClosed(EventArgs e) {
            s_window = null;
            Application.Current.MainWindow = null;
            AppStatic.Managers.RemoveManager(Vm.Manager);
            base.OnClosed(e);
        }

        #region 刷新总算力图表
        private void RefreshTotalSpeedChart(int limit) {
            //NTMinerRoot.Current.DebugLine($"获取总算力数据，范围{leftTime} - {rightTime}");
            var coinCodes = NTMinerRoot.Current.CoinSet.Select(a => a.Code).ToList();
            Server.ControlCenterService.GetLatestSnapshotsAsync(
                limit,
                coinCodes,
                (response, exception) => {
                    if (response == null) {
                        return;
                    }

                    if (!response.IsSuccess()) {
                        Write.UserLine(response.Description, ConsoleColor.Red);
                        return;
                    }
                    if (exception != null) {
                        Write.DevLine(exception.Message);
                        return;
                    }
                    UIThread.Execute(() => {
                        bool isOnlyOne = limit == 1;
                        Vm.TotalMiningCount = response.TotalMiningCount;
                        Vm.TotalOnlineCount = response.TotalOnlineCount;
                        foreach (var chartVm in Vm.ChartVms) {
                            var list = response.Data.Where(a => a.CoinCode == chartVm.CoinVm.Code).ToList();
                            if (list.Count == 0) {
                                list.Add(new CoinSnapshotData {
                                    CoinCode = chartVm.CoinVm.Code,
                                    DualCoinOnlineCount = 0,
                                    DualCoinMiningCount = 0,
                                    MainCoinOnlineCount = 0,
                                    MainCoinMiningCount = 0,
                                    RejectShareDelta = 0,
                                    ShareDelta = 0,
                                    Speed = 0,
                                    Timestamp = DateTime.Now
                                });
                            }
                            CoinSnapshotData one = null;
                            if (isOnlyOne) {
                                one = list.Last();
                            }
                            else {
                                list = list.OrderBy(a => a.Timestamp).ToList();
                            }
                            CoinSnapshotData latestData = list.Last();
                            chartVm.SnapshotDataVm.Update(latestData);
                            foreach (var riser in chartVm.Series) {
                                if (riser.Title == "speed") {
                                    if (list.Count > 0) {
                                        if (isOnlyOne) {
                                            riser.Values.Add(new MeasureModel() {
                                                DateTime = one.Timestamp,
                                                Value = one.Speed
                                            });
                                        }
                                        else {
                                            foreach (var item in list) {
                                                riser.Values.Add(new MeasureModel() {
                                                    DateTime = item.Timestamp,
                                                    Value = item.Speed
                                                });
                                            }
                                        }
                                    }
                                }
                                else if (riser.Title == "onlineCount") {
                                    if (isOnlyOne) {
                                        riser.Values.Add(new MeasureModel() {
                                            DateTime = one.Timestamp,
                                            Value = one.MainCoinOnlineCount + one.DualCoinOnlineCount
                                        });
                                    }
                                    else {
                                        foreach (var item in list) {
                                            riser.Values.Add(new MeasureModel() {
                                                DateTime = item.Timestamp,
                                                Value = item.MainCoinOnlineCount + item.DualCoinOnlineCount
                                            });
                                        }
                                    }
                                }
                                else if (riser.Title == "miningCount") {
                                    if (isOnlyOne) {
                                        riser.Values.Add(new MeasureModel() {
                                            DateTime = one.Timestamp,
                                            Value = one.MainCoinMiningCount + one.DualCoinMiningCount
                                        });
                                    }
                                    else {
                                        foreach (var item in list) {
                                            riser.Values.Add(new MeasureModel() {
                                                DateTime = item.Timestamp,
                                                Value = item.MainCoinMiningCount + item.DualCoinMiningCount
                                            });
                                        }
                                    }
                                }
                                else if (riser.Title == "rejectShare") {
                                    if (isOnlyOne) {
                                        riser.Values.Add(new MeasureModel() {
                                            DateTime = one.Timestamp,
                                            Value = one.RejectShareDelta
                                        });
                                    }
                                    else {
                                        foreach (var item in list) {
                                            riser.Values.Add(new MeasureModel() {
                                                DateTime = item.Timestamp,
                                                Value = item.RejectShareDelta
                                            });
                                        }
                                    }
                                }
                                else if (riser.Title == "acceptShare") {
                                    if (isOnlyOne) {
                                        riser.Values.Add(new MeasureModel() {
                                            DateTime = one.Timestamp,
                                            Value = one.ShareDelta
                                        });
                                    }
                                    else {
                                        foreach (var item in list) {
                                            riser.Values.Add(new MeasureModel() {
                                                DateTime = item.Timestamp,
                                                Value = item.ShareDelta
                                            });
                                        }
                                    }
                                }
                            }
                            DateTime now = DateTime.Now;
                            foreach (var riser in chartVm.Series) {
                                IChartValues valuesTotal = riser.Values;
                                if (valuesTotal.Count > 0 && ((MeasureModel)valuesTotal[0]).DateTime.AddMinutes(NTMinerRoot.Current.SpeedHistoryLengthByMinute) < now) {
                                    valuesTotal.RemoveAt(0);
                                }
                            }
                            chartVm.SetAxisLimits(DateTime.Now);
                        }
                    });
                });
        }
        #endregion

        private void MetroWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            WpfUtil.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }
}
