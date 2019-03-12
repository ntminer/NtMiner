using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using NTMiner.MinerServer;
using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class ChartViewModel : ViewModelBase {
        private SeriesCollection _series;
        private AxesCollection _axisY;
        private AxesCollection _axisX;

        private static readonly SolidColorBrush s_transparent = new SolidColorBrush(Colors.Transparent);
        private static readonly SolidColorBrush s_black = new SolidColorBrush(Colors.Black);
        private static readonly SolidColorBrush s_green = new SolidColorBrush(Colors.Green);
        private static readonly SolidColorBrush s_AxisForeground = new SolidColorBrush(Color.FromRgb(0x38, 0x52, 0x63));

        private readonly CoinViewModel _coinVm;

        public ICommand Hide { get; private set; }
        private ChartValues<MeasureModel> _rejectValues;
        private ChartValues<MeasureModel> _acceptValues;
        public ChartViewModel(CoinViewModel coinVm) {
            this.Hide = new DelegateCommand(() => {
                this.IsShow = false;
            });
            _coinVm = coinVm;
            var mapper = Mappers.Xy<MeasureModel>()
                .X(model => model.DateTime.Ticks)   //use DateTime.Ticks as X
                .Y(model => model.Value);           //use the value property as Y

            //lets save the mapper globally.
            Charting.For<MeasureModel>(mapper);

            Func<double, string> dateTimeFormatter = value => new DateTime((long)value).ToString("HH:mm");
            Func<double, string> speedFormatter = value => value.ToUnitSpeedText();
            //AxisStep forces the distance between each separator in the X axis
            double axisStep = TimeSpan.FromMinutes(1).Ticks;
            //AxisUnit forces lets the axis know that we are plotting Minutes
            //this is not always necessary, but it can prevent wrong labeling
            double axisUnit = TimeSpan.TicksPerMinute;
            var axisYSpeed = new Axis() {
                LabelFormatter = speedFormatter,
                MinValue = 0,
                Separator = new Separator(),
                Foreground = s_AxisForeground,
                FontSize = 13,
                Position = AxisPosition.RightTop
            };
            var axisYOnlineCount = new Axis() {
                LabelFormatter = value => Math.Round(value, 0).ToString(),
                Separator = new Separator(),
                Foreground = s_AxisForeground,
                MinValue = 0,
                FontSize = 13
            };
            var axisYShareCount = new Axis() {
                LabelFormatter = value => Math.Round(value, 0).ToString(),
                Separator = new Separator(),
                Foreground = s_AxisForeground,
                MinValue = 0,
                FontSize = 13,
                Position = AxisPosition.RightTop
            };
            this._axisY = new AxesCollection {
                axisYOnlineCount, axisYSpeed, axisYShareCount
            };
            DateTime now = DateTime.Now;
            this._axisX = new AxesCollection() {
                new Axis() {
                    LabelFormatter = dateTimeFormatter,
                    MaxValue = now.Ticks,
                    MinValue = now.Ticks - TimeSpan.FromMinutes(NTMinerRoot.Current.SpeedHistoryLengthByMinute).Ticks,
                    Unit=axisUnit,
                    Separator = new Separator() {
                        Step = axisStep
                    },
                    Foreground = s_AxisForeground,
                    FontSize = 12,
                }
            };
            LineSeries mainCoinSpeedLs = new LineSeries {
                Title = "speed",
                DataLabels = false,
                PointGeometrySize = 0,
                StrokeThickness = 1,
                ScalesYAt = 1,
                Values = new ChartValues<MeasureModel>()
            };
            LineSeries onlineCountLs = new LineSeries {
                Title = "onlineCount",
                DataLabels = false,
                PointGeometrySize = 0,
                StrokeThickness = 1,
                ScalesYAt = 0,
                Fill = s_transparent,
                Stroke = OnlineColor,
                Values = new ChartValues<MeasureModel>()
            };
            LineSeries miningCountLs = new LineSeries {
                Title = "miningCount",
                DataLabels = false,
                PointGeometrySize = 0,
                StrokeThickness = 1,
                ScalesYAt = 0,
                Fill = s_transparent,
                Stroke = MiningColor,
                Values = new ChartValues<MeasureModel>()
            };
            _rejectValues = new ChartValues<MeasureModel>();
            _acceptValues = new ChartValues<MeasureModel>();
            Random r = new Random((int)DateTime.Now.Ticks);
            for (DateTime t = now.AddMinutes(-10); t < now; t = t.AddSeconds(10)) {
                _rejectValues.Add(new MeasureModel() {
                    DateTime = t,
                    Value = r.Next(2)
                });
                _acceptValues.Add(new MeasureModel() {
                    DateTime = t,
                    Value = 10 + r.Next(2)
                });
            }
            StackedColumnSeries rejectScs = new StackedColumnSeries {
                Values = _rejectValues,
                DataLabels = false,
                ScalesYAt = 2,
                MaxColumnWidth = 7
            };
            StackedColumnSeries acceptScs = new StackedColumnSeries {
                Values = _acceptValues,
                DataLabels = false,
                ScalesYAt = 2,
                MaxColumnWidth = 7
            };
            this._series = new SeriesCollection() {
                mainCoinSpeedLs, rejectScs, acceptScs, miningCountLs, onlineCountLs
            };
        }

        public bool IsShow {
            get {
                string key = $"ChartVm.IsShow.{this.CoinVm.Code}";
                AppSettingViewModel appSettingVm;
                if (AppSettingViewModels.Current.TryGetAppSettingVm(key, out appSettingVm)) {
                    return (bool)appSettingVm.Value;
                }
                else {
                    return true;
                }
            }
            set {
                string key = $"ChartVm.IsShow.{this.CoinVm.Code}";
                VirtualRoot.Execute(new ChangeAppSettingCommand(new AppSettingData {
                    Key = key,
                    Value = value
                }));
                OnPropertyChanged(nameof(IsShow));
            }
        }

        private CoinSnapshotDataViewModel _snapshotDataVm;
        public CoinSnapshotDataViewModel SnapshotDataVm {
            get {
                if (_snapshotDataVm == null) {
                    CoinSnapshotDataViewModels.Current.TryGetSnapshotDataVm(CoinVm.Code, out _snapshotDataVm);
                }
                return _snapshotDataVm;
            }
        }

        public SolidColorBrush MiningColor {
            get {
                return s_green;
            }
        }

        public SolidColorBrush OnlineColor {
            get {
                return s_black;
            }
        }

        public CoinViewModel CoinVm {
            get {
                return _coinVm;
            }
        }

        public SeriesCollection Series {
            get => _series;
            private set {
                if (_series != value) {
                    _series = value;
                    OnPropertyChanged(nameof(Series));
                }
            }
        }

        public AxesCollection AxisY {
            get => _axisY;
            private set {
                if (_axisY != value) {
                    _axisY = value;
                    OnPropertyChanged(nameof(AxisY));
                }
            }
        }

        public AxesCollection AxisX {
            get {
                return _axisX;
            }
            private set {
                if (_axisX != value) {
                    _axisX = value;
                    OnPropertyChanged(nameof(AxisX));
                }
            }
        }

        public void SetAxisLimits(DateTime now) {
            AxisX[0].MaxValue = now.Ticks;
            AxisX[0].MinValue = now.Ticks - TimeSpan.FromMinutes(NTMinerRoot.Current.SpeedHistoryLengthByMinute).Ticks;
            double maxAcceptValue = 0;
            double maxRejectValue = 0;
            if (_acceptValues != null && _acceptValues.Count != 0) {
                maxAcceptValue = _acceptValues.Max(a => a.Value);
            }
            if (_rejectValues != null && _rejectValues.Count != 0) {
                maxRejectValue = _rejectValues.Max(a => a.Value);
            }
            AxisY[2].MaxValue = Math.Max(maxRejectValue, maxAcceptValue) * 3;
        }
    }
}
