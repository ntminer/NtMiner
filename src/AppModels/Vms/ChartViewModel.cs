using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using NTMiner.Core;
using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class ChartViewModel : ViewModelBase {
        private SeriesCollection _series;
        private AxesCollection _axisY;
        private AxesCollection _axisX;

        private static readonly SolidColorBrush SAxisForeground = new SolidColorBrush(Color.FromRgb(0x38, 0x52, 0x63));

        private readonly CoinViewModel _coinVm;

        public ICommand Hide { get; private set; }
        private readonly ChartValues<MeasureModel> _rejectValues;
        private readonly ChartValues<MeasureModel> _acceptValues;
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

            string DateTimeFormatter(double value) => new DateTime((long)value).ToString("HH:mm");
            string SpeedFormatter(double value) => value.ToUnitSpeedText();
            //AxisStep forces the distance between each separator in the X axis
            double axisStep = TimeSpan.FromMinutes(1).Ticks;
            //AxisUnit forces lets the axis know that we are plotting Minutes
            //this is not always necessary, but it can prevent wrong labeling
            double axisUnit = TimeSpan.TicksPerMinute;
            var axisYSpeed = new Axis() {
                LabelFormatter = SpeedFormatter,
                MinValue = 0,
                Separator = new Separator(),
                Foreground = SAxisForeground,
                FontSize = 13,
                Position = AxisPosition.RightTop
            };
            var axisYOnlineCount = new Axis() {
                LabelFormatter = value => Math.Round(value, 0) + "miner",
                Separator = new Separator(),
                Foreground = SAxisForeground,
                MinValue = 0,
                FontSize = 11
            };
            var axisYShareCount = new Axis() {
                LabelFormatter = value => Math.Round(value, 0) + "share",
                Separator = new Separator(),
                Foreground = SAxisForeground,
                MinValue = 0,
                FontSize = 11,
                Position = AxisPosition.RightTop
            };
            this._axisY = new AxesCollection {
                axisYOnlineCount, axisYSpeed, axisYShareCount
            };
            DateTime now = DateTime.Now;
            this._axisX = new AxesCollection() {
                new Axis() {
                    LabelFormatter = DateTimeFormatter,
                    MaxValue = now.Ticks,
                    MinValue = now.Ticks - TimeSpan.FromMinutes(NTMinerRoot.SpeedHistoryLengthByMinute).Ticks,
                    Unit=axisUnit,
                    Separator = new Separator() {
                        Step = axisStep
                    },
                    Foreground = SAxisForeground,
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
                Fill = WpfUtil.TransparentBrush,
                Stroke = OnlineColor,
                Values = new ChartValues<MeasureModel>()
            };
            LineSeries miningCountLs = new LineSeries {
                Title = "miningCount",
                DataLabels = false,
                PointGeometrySize = 0,
                StrokeThickness = 1,
                ScalesYAt = 0,
                Fill = WpfUtil.TransparentBrush,
                Stroke = MiningColor,
                Values = new ChartValues<MeasureModel>()
            };
            _rejectValues = new ChartValues<MeasureModel>();
            _acceptValues = new ChartValues<MeasureModel>();
            StackedColumnSeries rejectScs = new StackedColumnSeries {
                Title = "rejectShare",
                Values = _rejectValues,
                DataLabels = false,
                ScalesYAt = 2,
                MaxColumnWidth = 7
            };
            StackedColumnSeries acceptScs = new StackedColumnSeries {
                Title = "acceptShare",
                Values = _acceptValues,
                DataLabels = false,
                ScalesYAt = 2,
                MaxColumnWidth = 7
            };
            this._series = new SeriesCollection() {
                mainCoinSpeedLs, rejectScs, acceptScs, miningCountLs, onlineCountLs
            };
        }

        private bool _isFirst = true;
        private bool _isShow;
        public bool IsShow {
            get {
                if (!_isFirst) {
                    return _isShow;
                }
                _isFirst = false;
                string key = $"ChartVm.IsShow.{this.CoinVm.Code}";
                if (NTMinerRoot.Instance.ServerAppSettingSet.TryGetAppSetting(key, out IAppSetting _appSetting)) {
                    _isShow = (bool)_appSetting.Value;
                }
                else {
                    _isShow = false;
                }
                return _isShow;
            }
            set {
                _isFirst = false;
                _isShow = value;
                OnPropertyChanged(nameof(IsShow));
            }
        }

        private CoinSnapshotDataViewModel _snapshotDataVm;
        public CoinSnapshotDataViewModel SnapshotDataVm {
            get {
                if (_snapshotDataVm == null) {
                    AppContext.Instance.CoinSnapshotDataVms.TryGetSnapshotDataVm(CoinVm.Code, out _snapshotDataVm);
                }
                return _snapshotDataVm;
            }
        }

        public SolidColorBrush MiningColor {
            get {
                return WpfUtil.GreenBrush;
            }
        }

        public SolidColorBrush OnlineColor {
            get {
                return WpfUtil.BlackBrush;
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
            AxisX[0].MinValue = now.Ticks - TimeSpan.FromMinutes(NTMinerRoot.SpeedHistoryLengthByMinute).Ticks;
            double maxAcceptValue = 0;
            double maxRejectValue = 0;
            if (_acceptValues != null && _acceptValues.Count != 0) {
                maxAcceptValue = _acceptValues.Max(a => a.Value);
            }
            if (_rejectValues != null && _rejectValues.Count != 0) {
                maxRejectValue = _rejectValues.Max(a => a.Value);
            }
            double maxShareValue = Math.Max(maxRejectValue, maxAcceptValue);
            // 不能为0
            if (maxShareValue < 1) {
                maxShareValue = 1;
            }
            AxisY[2].MaxValue = maxShareValue * 3;
        }
    }
}
