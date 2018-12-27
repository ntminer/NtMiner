using LiteDB;
using LiveCharts;
using LiveCharts.Configurations;
using LiveCharts.Wpf;
using NTMiner.ServiceContracts.DataObjects;
using System;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class ControlCenterChartViewModel : ViewModelBase {
        private SeriesCollection _series;
        private AxesCollection _axisY;
        private AxesCollection _axisX;
        private static readonly SolidColorBrush transparent = new SolidColorBrush(Colors.Transparent);
        private static readonly SolidColorBrush black = new SolidColorBrush(Colors.Black);
        private static readonly SolidColorBrush green = new SolidColorBrush(Colors.Green);

        private readonly CoinViewModel _coinVm;

        public ControlCenterChartViewModel(CoinViewModel coinVm) {
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
                Foreground = black,
                FontSize = 13,
                Position = AxisPosition.RightTop
            };
            var axisYOnlineCount = new Axis() {
                LabelFormatter = value => Math.Round(value, 0).ToString(),
                Separator = new Separator(),
                Foreground = black,
                MinValue = 0,
                FontSize = 13
            };
            this._axisY = new AxesCollection {
                axisYOnlineCount, axisYSpeed
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
                    Foreground = black,
                    FontSize = 13,
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
                Fill = transparent,
                Stroke = OnlineColor,
                Values = new ChartValues<MeasureModel>()
            };
            LineSeries miningCountLs = new LineSeries {
                Title = "miningCount",
                DataLabels = false,
                PointGeometrySize = 0,
                StrokeThickness = 1,
                ScalesYAt = 0,
                Fill = transparent,
                Stroke = MiningColor,
                Values = new ChartValues<MeasureModel>()
            };
            this._series = new SeriesCollection() {
                mainCoinSpeedLs, onlineCountLs, miningCountLs
            };
        }

        private CoinSnapshotDataViewModel _snapshotDataVm;
        public CoinSnapshotDataViewModel SnapshotDataVm {
            get {
                if (_snapshotDataVm == null) {
                    _snapshotDataVm = new CoinSnapshotDataViewModel(new CoinSnapshotData {
                        Id = ObjectId.Empty,
                        CoinCode = CoinVm.Code,
                        MainCoinMiningCount = 0,
                        MainCoinOnlineCount = 0,
                        DualCoinMiningCount = 0,
                        DualCoinOnlineCount = 0,
                        ShareDelta = 0,
                        Speed = 0,
                        Timestamp = DateTime.MinValue
                    });
                }
                return _snapshotDataVm;
            }
        }

        public SolidColorBrush MiningColor {
            get {
                return green;
            }
        }

        public SolidColorBrush OnlineColor {
            get {
                return black;
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
                _series = value;
                OnPropertyChanged(nameof(Series));
            }
        }

        public AxesCollection AxisY {
            get => _axisY;
            private set {
                _axisY = value;
                OnPropertyChanged(nameof(AxisY));
            }
        }

        public AxesCollection AxisX {
            get {
                return _axisX;
            }
            private set {
                _axisX = value;
                OnPropertyChanged(nameof(AxisX));
            }
        }

        public void SetAxisLimits(DateTime now) {
            AxisX[0].MaxValue = now.Ticks;
            AxisX[0].MinValue = now.Ticks - TimeSpan.FromMinutes(NTMinerRoot.Current.SpeedHistoryLengthByMinute).Ticks;
        }
    }
}
