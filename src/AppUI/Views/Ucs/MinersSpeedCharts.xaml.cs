using LiveCharts;
using NTMiner.Bus;
using NTMiner.ServiceContracts.DataObjects;
using NTMiner.Vms;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class MinersSpeedCharts : UserControl {
        public MinersSpeedChartsViewModel Vm {
            get {
                return (MinersSpeedChartsViewModel)this.DataContext;
            }
        }

        private readonly Window _window;
        public MinersSpeedCharts(Window window) {
            _window = window;
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);

            if (Design.IsInDesignMode) {
                return;
            }
            #region 总算力
            RefreshTotalSpeedChart(limit: 60);
            DelegateHandler<Per10SecondEvent> refeshTotalSpeedChart = Global.Access<Per10SecondEvent>(
                Guid.Parse("CCF4833F-7957-49B2-9642-3EFAFCFE9C9E"),
                "周期刷新总算力图",
                LogEnum.None,
                action: message => {
                    RefreshTotalSpeedChart(limit: 1);
                    Execute.OnUIThread(() => {
                        foreach (var item in Vm.TotalVms) {
                            item.SetAxisLimits(message.Timestamp);
                        }
                    });
                });
            this.Unloaded += (object sender, RoutedEventArgs e) => {
                Global.UnAccess(refeshTotalSpeedChart);
            };
            #endregion
        }

        #region 刷新总算力图表
        private void RefreshTotalSpeedChart(int limit) {
            //NTMinerRoot.Current.DebugLine($"获取总算力数据，范围{leftTime} - {rightTime}");
            var coinCodes = NTMinerRoot.Current.CoinSet.Select(a => a.Code).ToList();
            Server.ControlCenterService.GetLatestSnapshotsAsync(
                limit,
                coinCodes,
                (response)=> {
                    if (response == null) {
                        return;
                    }
                    Execute.OnUIThread(() => {
                        bool isOnlyOne = limit == 1;
                        Vm.TotalMiningCount = response.TotalMiningCount;
                        Vm.TotalOnlineCount = response.TotalOnlineCount;
                        //NTMinerRoot.Current.DebugLine($"获取了{data.Count}条");
                        foreach (var chartVm in Vm.TotalVms) {
                            var list = response.Data.Where(a => a.CoinCode == chartVm.CoinVm.Code).ToList();
                            if (list.Count != 0) {
                                list = list.OrderBy(a => a.Timestamp).ToList();
                                CoinSnapshotData latestData = list.Last();
                                chartVm.SnapshotDataVm.Update(latestData);
                                foreach (var seriy in chartVm.Series) {
                                    if (seriy.Title == "speed") {
                                        if (list.Count > 0) {
                                            if (isOnlyOne) {
                                                var item = list.Last();
                                                seriy.Values.Add(new MeasureModel() {
                                                    DateTime = item.Timestamp,
                                                    Value = item.Speed
                                                });
                                            }
                                            else {
                                                foreach (var item in list) {
                                                    seriy.Values.Add(new MeasureModel() {
                                                        DateTime = item.Timestamp,
                                                        Value = item.Speed
                                                    });
                                                }
                                            }
                                        }
                                    }
                                    else if (seriy.Title == "onlineCount") {
                                        if (isOnlyOne) {
                                            var item = list.Last();
                                            seriy.Values.Add(new MeasureModel() {
                                                DateTime = item.Timestamp,
                                                Value = item.MainCoinOnlineCount + item.DualCoinOnlineCount
                                            });
                                        }
                                        else {
                                            foreach (var item in list) {
                                                seriy.Values.Add(new MeasureModel() {
                                                    DateTime = item.Timestamp,
                                                    Value = item.MainCoinOnlineCount + item.DualCoinOnlineCount
                                                });
                                            }
                                        }
                                    }
                                    else if (seriy.Title == "miningCount") {
                                        if (isOnlyOne) {
                                            var item = list.Last();
                                            seriy.Values.Add(new MeasureModel() {
                                                DateTime = item.Timestamp,
                                                Value = item.MainCoinMiningCount + item.DualCoinMiningCount
                                            });
                                        }
                                        else {
                                            foreach (var item in list) {
                                                seriy.Values.Add(new MeasureModel() {
                                                    DateTime = item.Timestamp,
                                                    Value = item.MainCoinMiningCount + item.DualCoinMiningCount
                                                });
                                            }
                                        }
                                    }
                                }
                            }
                            DateTime now = DateTime.Now;
                            foreach (var seriy in chartVm.Series) {
                                IChartValues valuesTotal = seriy.Values;
                                if (valuesTotal.Count > 0 && ((MeasureModel)valuesTotal[0]).DateTime.AddMinutes(NTMinerRoot.Current.SpeedHistoryLengthByMinute) < now) {
                                    valuesTotal.RemoveAt(0);
                                }
                            }
                        }
                    });
                });
        }
        #endregion

        private void ItemsControl_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                _window?.DragMove();
            }
        }
    }
}
