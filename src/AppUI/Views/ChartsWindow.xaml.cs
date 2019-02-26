using LiveCharts;
using MahApps.Metro.Controls;
using NTMiner.Bus;
using NTMiner.MinerServer;
using NTMiner.Vms;
using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace NTMiner.Views {
    public partial class ChartsWindow : MetroWindow, IMainWindow {
        public ChartsWindowViewModel Vm {
            get {
                return (ChartsWindowViewModel)this.DataContext;
            }
        }

        public ChartsWindow() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);
            Write.WriteUserLineMethod = (text, foreground)=> {
                WriteLine(this.RichTextBox, this.ConsoleParagraph, text, foreground);
            };
            Write.WriteDevLineMethod = (text, foreground) => {
                WriteLine(this.RichTextBoxDebug, this.ConsoleParagraphDebug, text, foreground);
            };
            #region 总算力
            RefreshTotalSpeedChart(limit: 60);
            DelegateHandler<Per10SecondEvent> refeshTotalSpeedChart = VirtualRoot.On<Per10SecondEvent>(
                "周期刷新总算力图",
                LogEnum.Console,
                action: message => {
                    RefreshTotalSpeedChart(limit: 1);
                    UIThread.Execute(() => {
                        foreach (var item in Vm.ChartVms) {
                            item.SetAxisLimits(message.Timestamp);
                        }
                    });
                });
            this.Unloaded += (object sender, RoutedEventArgs e) => {
                VirtualRoot.UnPath(refeshTotalSpeedChart);
            };
            #endregion
        }

        public void ShowThisWindow() {
            this.Show();
            this.WindowState = WindowState.Normal;
            this.Activate();
        }

        #region 刷新总算力图表
        private void RefreshTotalSpeedChart(int limit) {
            //NTMinerRoot.Current.DebugLine($"获取总算力数据，范围{leftTime} - {rightTime}");
            var coinCodes = NTMinerRoot.Current.CoinSet.Select(a => a.Code).ToList();
            Server.ControlCenterService.GetLatestSnapshotsAsync(
                limit,
                coinCodes,
                (response) => {
                    if (response == null) {
                        return;
                    }
                    UIThread.Execute(() => {
                        bool isOnlyOne = limit == 1;
                        Vm.TotalMiningCount = response.TotalMiningCount;
                        Vm.TotalOnlineCount = response.TotalOnlineCount;
                        //NTMinerRoot.Current.DebugLine($"获取了{data.Count}条");
                        foreach (var chartVm in Vm.ChartVms) {
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

        private void MetroWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed) {
                this.DragMove();
            }
        }

        private void InnerWrite(RichTextBox rtb, Paragraph p, string text, ConsoleColor foreground) {
            InlineCollection list = p.Inlines;
            // 满1000行删除500行
            if (list.Count > 1000) {
                int delLines = 500;
                while (delLines-- > 0) {
                    ((IList)list).RemoveAt(0);
                }
            }
            Run run = new Run(text) {
                Foreground = new SolidColorBrush(foreground.ToMediaColor())
            };
            list.Add(run);

            if (ChkbIsConsoleAutoScrollToEnd.IsChecked.HasValue && ChkbIsConsoleAutoScrollToEnd.IsChecked.Value) {
                rtb.ScrollToEnd();
            }
        }

        public void WriteLine(RichTextBox rtb, Paragraph p, string text, ConsoleColor foreground) {
            Dispatcher.Invoke((Action)(() => {
                if (p.Inlines.Count > 0) {
                    text = "\n" + text;
                }
                InnerWrite(rtb, p, text, foreground);
            }));
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            if (e.LeftButton == MouseButtonState.Pressed && e.Source.GetType() == typeof(ScrollViewer)) {
                Window.GetWindow(this).DragMove();
                e.Handled = true;
            }
        }
    }
}
