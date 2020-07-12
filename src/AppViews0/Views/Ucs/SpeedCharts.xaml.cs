using LiveCharts;
using LiveCharts.Wpf;
using NTMiner.Gpus;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NTMiner.Views.Ucs {
    public partial class SpeedCharts : UserControl {
        public static void ShowWindow(GpuSpeedViewModel gpuSpeedVm = null) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel {
                Title = "算力图",
                IconName = "Icon_SpeedChart",
                Width = 760,
                Height = 460,
                CloseVisible = Visibility.Visible,
                FooterVisible = Visibility.Collapsed
            }, ucFactory: (window) => {
                SpeedCharts uc = new SpeedCharts();
                return uc;
            }, beforeShow: (window, uc) => {
                if (gpuSpeedVm != null) {
                    SpeedChartViewModel item = uc.Vm.SpeedChartVms.Items.FirstOrDefault(a => a.GpuSpeedVm == gpuSpeedVm);
                    if (item != null) {
                        uc.Vm.CurrentSpeedChartVm = item;
                    }
                }
            }, fixedSize: false);
        }

        public SpeedChartsViewModel Vm { get; private set; }

        private readonly Dictionary<SpeedChartViewModel, CartesianChart> _chartDic = new Dictionary<SpeedChartViewModel, CartesianChart>();
        public SpeedCharts() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Vm = new SpeedChartsViewModel();
            this.DataContext = this.Vm;
            InitializeComponent();
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            Guid mainCoinId = NTMinerContext.Instance.MinerProfile.CoinId;
            this.OnLoaded((window) => {
                window.AddEventPath<GpuSpeedChangedEvent>("显卡算力变更后刷新算力图界面", LogEnum.DevConsole,
                    action: (message) => {
                        UIThread.Execute(() => {
                            if (mainCoinId != NTMinerContext.Instance.MinerProfile.CoinId) {
                                mainCoinId = NTMinerContext.Instance.MinerProfile.CoinId;
                                foreach (var speedChartVm in Vm.SpeedChartVms.Items) {
                                    SeriesCollection series = speedChartVm.Series;
                                    SeriesCollection seriesShadow = speedChartVm.SeriesShadow;
                                    foreach (var item in series) {
                                        item.Values.Clear();
                                    }
                                    foreach (var item in seriesShadow) {
                                        item.Values.Clear();
                                    }
                                }
                            }
                            IGpuSpeed gpuSpeed = message.Source;
                            int index = gpuSpeed.Gpu.Index;
                            if (Vm.SpeedChartVms.ContainsKey(index)) {
                                SpeedChartViewModel speedChartVm = Vm.SpeedChartVms[index];
                                SeriesCollection series = speedChartVm.Series;
                                SeriesCollection seriesShadow = speedChartVm.SeriesShadow;
                                DateTime now = DateTime.Now;
                                if (gpuSpeed.MainCoinSpeed != null && series.Count > 0) {
                                    IChartValues chartValues = series[0].Values;
                                    chartValues.Add(new MeasureModel() {
                                        DateTime = gpuSpeed.MainCoinSpeed.SpeedOn,
                                        Value = gpuSpeed.MainCoinSpeed.Value
                                    });
                                    if (((MeasureModel)chartValues[0]).DateTime.AddMinutes(NTMinerContext.SpeedHistoryLengthByMinute) < now) {
                                        chartValues.RemoveAt(0);
                                    }
                                    chartValues = seriesShadow[0].Values;
                                    chartValues.Add(new MeasureModel() {
                                        DateTime = gpuSpeed.MainCoinSpeed.SpeedOn,
                                        Value = gpuSpeed.MainCoinSpeed.Value
                                    });
                                    if (((MeasureModel)chartValues[0]).DateTime.AddMinutes(NTMinerContext.SpeedHistoryLengthByMinute) < now) {
                                        chartValues.RemoveAt(0);
                                    }
                                }
                                if (gpuSpeed.DualCoinSpeed != null && series.Count > 1) {
                                    IChartValues chartValues = series[1].Values;
                                    chartValues.Add(new MeasureModel() {
                                        DateTime = gpuSpeed.DualCoinSpeed.SpeedOn,
                                        Value = gpuSpeed.DualCoinSpeed.Value
                                    });
                                    if (((MeasureModel)chartValues[0]).DateTime.AddMinutes(NTMinerContext.SpeedHistoryLengthByMinute) < now) {
                                        chartValues.RemoveAt(0);
                                    }
                                    chartValues = seriesShadow[1].Values;
                                    chartValues.Add(new MeasureModel() {
                                        DateTime = gpuSpeed.DualCoinSpeed.SpeedOn,
                                        Value = gpuSpeed.DualCoinSpeed.Value
                                    });
                                    if (((MeasureModel)chartValues[0]).DateTime.AddMinutes(NTMinerContext.SpeedHistoryLengthByMinute) < now) {
                                        chartValues.RemoveAt(0);
                                    }
                                }

                                speedChartVm.SetAxisLimits(now);
                            }
                        });
                    }, location: this.GetType());
            });

            Vm.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) => {
                if (e.PropertyName == nameof(Vm.CurrentSpeedChartVm)) {
                    SpeedChartViewModel currentItem = Vm.CurrentSpeedChartVm;
                    if (currentItem != null) {
                        foreach (var item in _chartDic.Values) {
                            item.Visibility = Visibility.Collapsed;
                        }
                        CartesianChart chart;
                        if (!_chartDic.ContainsKey(currentItem)) {
                            chart = new CartesianChart() {
                                DisableAnimations = true,
                                Hoverable = false,
                                DataTooltip = null,
                                Background = WpfUtil.WhiteBrush,
                                Padding = new Thickness(4, 0, 0, 0),
                                Visibility = Visibility.Visible
                            };
                            chart.Series = currentItem.SeriesShadow;
                            chart.AxisX = currentItem.AxisXShadow;
                            chart.AxisY = currentItem.AxisYShadow;
                            _chartDic.Add(currentItem, chart);
                            DetailsGrid.Children.Add(chart);
                        }
                        else {
                            chart = _chartDic[currentItem];
                            chart.Visibility = Visibility.Visible;
                        }
                    }
                }
            };

            Vm.CurrentSpeedChartVm = Vm.SpeedChartVms.Items.FirstOrDefault();

            if (AppRoot.MinerProfileVm.CoinVm != null) {
                Guid coinId = AppRoot.MinerProfileVm.CoinId;
                foreach (var item in NTMinerContext.Instance.GpuSet.AsEnumerable()) {
                    List<IGpuSpeed> gpuSpeedHistory = item.GetGpuSpeedHistory();
                    SpeedChartViewModel speedChartVm = Vm.SpeedChartVms[item.Index];
                    SeriesCollection series = speedChartVm.Series;
                    SeriesCollection seriesShadow = speedChartVm.SeriesShadow;
                    DateTime now = DateTime.Now;
                    foreach (var gpuSpeed in gpuSpeedHistory) {
                        if (gpuSpeed.MainCoinSpeed != null && series.Count > 0) {
                            series[0].Values.Add(new MeasureModel() {
                                DateTime = gpuSpeed.MainCoinSpeed.SpeedOn,
                                Value = gpuSpeed.MainCoinSpeed.Value
                            });
                            seriesShadow[0].Values.Add(new MeasureModel() {
                                DateTime = gpuSpeed.MainCoinSpeed.SpeedOn,
                                Value = gpuSpeed.MainCoinSpeed.Value
                            });
                        }
                        if (gpuSpeed.DualCoinSpeed != null && series.Count > 1) {
                            series[0].Values.Add(new MeasureModel() {
                                DateTime = gpuSpeed.DualCoinSpeed.SpeedOn,
                                Value = gpuSpeed.DualCoinSpeed.Value
                            });
                            seriesShadow[0].Values.Add(new MeasureModel() {
                                DateTime = gpuSpeed.DualCoinSpeed.SpeedOn,
                                Value = gpuSpeed.DualCoinSpeed.Value
                            });
                        }
                    }
                    IChartValues values = series[0].Values;
                    if (values.Count > 0 && ((MeasureModel)values[0]).DateTime.AddMinutes(NTMinerContext.SpeedHistoryLengthByMinute) < now) {
                        series[0].Values.RemoveAt(0);
                    }
                    values = seriesShadow[0].Values;
                    if (values.Count > 0 && ((MeasureModel)values[0]).DateTime.AddMinutes(NTMinerContext.SpeedHistoryLengthByMinute) < now) {
                        seriesShadow[0].Values.RemoveAt(0);
                    }
                    speedChartVm.SetAxisLimits(now);
                }
            }
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            WpfUtil.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }
}