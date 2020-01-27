using LiveCharts;
using NTMiner.Core;
using NTMiner.Core.MinerServer;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Views {
    public partial class ChartsWindow : BlankWindow {
        private static ChartsWindow _sWindow = null;
        public static void ShowWindow() {
            if (_sWindow == null) {
                _sWindow = new ChartsWindow();
            }
            _sWindow.Show();
            if (_sWindow.WindowState == WindowState.Minimized) {
                _sWindow.WindowState = WindowState.Normal;
            }
            _sWindow.Activate();
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
            this.TbUcName.Text = nameof(ChartsWindow);
            NotiCenterWindow.Instance.Bind(this);
            #region 总算力
            this.AddEventPath<Per10SecondEvent>("周期刷新总算力图", LogEnum.DevConsole,
                action: message => {
                    RefreshTotalSpeedChart(limit: 1);
                }, location: this.GetType());
            RefreshTotalSpeedChart(limit: 60);
            #endregion
        }

        protected override void OnClosing(CancelEventArgs e) {
            List<AppSettingData> list = new List<AppSettingData>();
            foreach (var item in Vm.ChartVms) {
                string key = $"ChartVm.IsShow.{item.CoinVm.Code}";
                list.Add(new AppSettingData {
                    Key = key,
                    Value = item.IsShow
                });
            }
            VirtualRoot.Execute(new SetServerAppSettingsCommand(list));
            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e) {
            _sWindow = null;
            base.OnClosed(e);
        }

        #region 刷新总算力图表
        private void RefreshTotalSpeedChart(int limit) {
            //NTMinerRoot.Current.DebugLine($"获取总算力数据，范围{leftTime} - {rightTime}");
            RpcRoot.Server.ControlCenterService.GetLatestSnapshotsAsync(
                limit,
                (response, exception) => {
                    if (response == null) {
                        return;
                    }

                    if (!response.IsSuccess()) {
                        Write.UserFail(response.ReadMessage(exception));
                        return;
                    }
                    UIThread.Execute(() => () => {
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
                                    Timestamp = DateTime.Now.AddSeconds(-5)
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
                            DateTime now = DateTime.Now.AddSeconds(10);
                            foreach (var riser in chartVm.Series) {
                                IChartValues valuesTotal = riser.Values;
                                if (valuesTotal.Count > 0 && ((MeasureModel)valuesTotal[0]).DateTime.AddMinutes(NTMinerRoot.SpeedHistoryLengthByMinute) < now) {
                                    valuesTotal.RemoveAt(0);
                                }
                            }
                            chartVm.SetAxisLimits(now);
                        }
                    });
                });
        }
        #endregion
        
        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            WpfUtil.ScrollViewer_PreviewMouseDown(sender, e);
        }

        private void IsShowCheckBox_Click(object sender, RoutedEventArgs e) {
            Vm.OnPropertyChanged(nameof(Vm.IsShowAll));
        }
    }
}
