using LiveCharts;
using LiveCharts.Wpf;
using NTMiner.Bus;
using NTMiner.Core;
using NTMiner.Core.Gpus;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace NTMiner.Views.Ucs {
    public partial class SpeedCharts : UserControl {
        public static void ShowWindow(GpuSpeedViewModel gpuSpeedVm = null) {
            ContainerWindow.ShowWindow(new ContainerWindowViewModel() {
                IconName = "Icon_Speed",
                Width = 860,
                Height = 520,
                CloseVisible = Visibility.Visible,
                FooterVisible = Visibility.Collapsed
            }, ucFactory: (window) => {
                SpeedCharts uc = new SpeedCharts();
                return uc;
            }, beforeShow: (uc) => {
                if (gpuSpeedVm != null) {
                    SpeedChartsViewModel vm = (SpeedChartsViewModel)uc.DataContext;
                    SpeedChartViewModel item = vm.SpeedChartVms.FirstOrDefault(a => a.GpuSpeedVm == gpuSpeedVm);
                    if (item != null) {
                        vm.SetCurrentSpeedChartVm(item);
                    }
                }
            }, fixedSize: false);
        }

        private SpeedChartsViewModel Vm {
            get {
                return (SpeedChartsViewModel)this.DataContext;
            }
        }

        private readonly List<IDelegateHandler> _handlers = new List<IDelegateHandler>();
        private readonly Dictionary<SpeedChartViewModel, CartesianChart> _chartDic = new Dictionary<SpeedChartViewModel, CartesianChart>();
        public SpeedCharts() {
            InitializeComponent();
            ResourceDictionarySet.Instance.FillResourceDic(this, this.Resources);

            if (Design.IsInDesignMode) {
                return;
            }
            Guid mainCoinId = NTMinerRoot.Current.MinerProfile.CoinId;
            VirtualRoot.On<GpuSpeedChangedEvent>("显卡算力变更后刷新算力图界面", LogEnum.DevConsole,
                action: (message) => {
                    UIThread.Execute(() => {
                        if (mainCoinId != NTMinerRoot.Current.MinerProfile.CoinId) {
                            mainCoinId = NTMinerRoot.Current.MinerProfile.CoinId;
                            foreach (var speedChartVm in Vm.SpeedChartVms) {
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
                                if (((MeasureModel)chartValues[0]).DateTime.AddMinutes(NTMinerRoot.Current.SpeedHistoryLengthByMinute) < now) {
                                    chartValues.RemoveAt(0);
                                }
                                chartValues = seriesShadow[0].Values;
                                chartValues.Add(new MeasureModel() {
                                    DateTime = gpuSpeed.MainCoinSpeed.SpeedOn,
                                    Value = gpuSpeed.MainCoinSpeed.Value
                                });
                                if (((MeasureModel)chartValues[0]).DateTime.AddMinutes(NTMinerRoot.Current.SpeedHistoryLengthByMinute) < now) {
                                    chartValues.RemoveAt(0);
                                }
                            }
                            if (gpuSpeed.DualCoinSpeed != null && series.Count > 1) {
                                IChartValues chartValues = series[1].Values;
                                chartValues.Add(new MeasureModel() {
                                    DateTime = gpuSpeed.DualCoinSpeed.SpeedOn,
                                    Value = gpuSpeed.DualCoinSpeed.Value
                                });
                                if (((MeasureModel)chartValues[0]).DateTime.AddMinutes(NTMinerRoot.Current.SpeedHistoryLengthByMinute) < now) {
                                    chartValues.RemoveAt(0);
                                }
                                chartValues = seriesShadow[1].Values;
                                chartValues.Add(new MeasureModel() {
                                    DateTime = gpuSpeed.DualCoinSpeed.SpeedOn,
                                    Value = gpuSpeed.DualCoinSpeed.Value
                                });
                                if (((MeasureModel)chartValues[0]).DateTime.AddMinutes(NTMinerRoot.Current.SpeedHistoryLengthByMinute) < now) {
                                    chartValues.RemoveAt(0);
                                }
                            }

                            speedChartVm.SetAxisLimits(now);
                        }
                    });
                }).AddToCollection(_handlers);

            Vm.ItemsPanelColumns = 1;
            this.Unloaded += (object sender, RoutedEventArgs e) => {
                foreach (var handler in _handlers) {
                    VirtualRoot.UnPath(handler);
                }
                foreach (var item in Vm.SpeedChartVms) {
                    item.Series = null;
                    item.SeriesShadow = null;
                    item.AxisX = null;
                    item.AxisY = null;
                    item.AxisXShadow = null;
                    item.AxisYShadow = null;
                }
                _chartDic.Clear();
            };
            SolidColorBrush White = new SolidColorBrush(Colors.White);
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
                                Background = White,
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

            Vm.SetCurrentSpeedChartVm(Vm.SpeedChartVms.FirstOrDefault());

            if (MinerProfileViewModel.Current.CoinVm != null) {
                Guid coinId = MinerProfileViewModel.Current.CoinId;
                foreach (var item in NTMinerRoot.Current.GpuSet) {
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
                    if (values.Count > 0 && ((MeasureModel)values[0]).DateTime.AddMinutes(NTMinerRoot.Current.SpeedHistoryLengthByMinute) < now) {
                        series[0].Values.RemoveAt(0);
                    }
                    values = seriesShadow[0].Values;
                    if (values.Count > 0 && ((MeasureModel)values[0]).DateTime.AddMinutes(NTMinerRoot.Current.SpeedHistoryLengthByMinute) < now) {
                        seriesShadow[0].Values.RemoveAt(0);
                    }
                    speedChartVm.SetAxisLimits(now);
                }
            }
        }

        private void ScrollViewer_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            Wpf.Util.ScrollViewer_PreviewMouseDown(sender, e);
        }
    }
}