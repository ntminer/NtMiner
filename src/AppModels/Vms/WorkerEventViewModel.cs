using NTMiner.MinerClient;
using System;
using System.Windows;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class WorkerEventViewModel : ViewModelBase, IWorkerEvent {
        private static readonly StreamGeometry ErrorIcon = (StreamGeometry)Application.Current.Resources["Icon_Error"];
        private static readonly StreamGeometry WarnIcon = (StreamGeometry)Application.Current.Resources["Icon_Waring"];
        private static readonly StreamGeometry InfoIcon = (StreamGeometry)Application.Current.Resources["Icon_Info"];
        private static readonly SolidColorBrush IconFillColor = (SolidColorBrush)Application.Current.Resources["IconFillColor"];
        private static readonly SolidColorBrush Warn = (SolidColorBrush)Application.Current.Resources["Warn"];

        private readonly IWorkerEvent _data;
        private readonly WorkerEventType _eventType;

        public WorkerEventViewModel(IWorkerEvent data) {
            _data = data;
            _data.EventType.TryParse(out _eventType);
        }

        public int GetId() {
            return _data.Id;
        }

        public int Id {
            get {
                return _data.Id;
            }
        }

        public string Channel {
            get {
                return _data.Channel;
            }
        }

        public string ChannelText {
            get {
                if (_data.Channel.TryParse(out WorkerEventChannel channel)) {
                    return channel.GetDescription();
                }
                return "未知";
            }
        }

        public string EventType {
            get {
                return _data.EventType;
            }
        }

        public string EventTypeText {
            get {
                if (_eventType != WorkerEventType.Undefined) {
                    return _eventType.GetDescription();
                }
                return "未知";
            }
        }

        public StreamGeometry EventTypeIcon {
            get {
                switch (_eventType) {
                    case WorkerEventType.Undefined:
                        return null;
                    case WorkerEventType.Info:
                        return InfoIcon;
                    case WorkerEventType.Warn:
                        return WarnIcon;
                    case WorkerEventType.Error:
                        return ErrorIcon;
                    default:
                        return null;
                }
            }
        }

        public SolidColorBrush IconFill {
            get {
                switch (_eventType) {
                    case WorkerEventType.Undefined:
                        return Wpf.Util.BlackBrush;
                    case WorkerEventType.Info:
                        return IconFillColor;
                    case WorkerEventType.Warn:
                        return Warn;
                    case WorkerEventType.Error:
                        return Wpf.Util.RedBrush;
                    default:
                        return Wpf.Util.BlackBrush;
                }
            }
        }

        public SolidColorBrush Foreground {
            get {
                switch (_eventType) {
                    case WorkerEventType.Undefined:
                        return Wpf.Util.BlackBrush;
                    case WorkerEventType.Info:
                        return Wpf.Util.BlackBrush;
                    case WorkerEventType.Warn:
                        return Warn;
                    case WorkerEventType.Error:
                        return Wpf.Util.RedBrush;
                    default:
                        return Wpf.Util.BlackBrush;
                }
            }
        }

        public string Content {
            get {
                return _data.Content;
            }
        }

        public DateTime EventOn {
            get {
                return _data.EventOn;
            }
        }

        public string EventOnText {
            get {
                int offDay = (DateTime.Now.Date - _data.EventOn.Date).Days;
                switch (offDay) {
                    case 0:
                        return $"今天 {_data.EventOn.TimeOfDay.ToString("hh\\:mm\\:ss")}";
                    case 1:
                        return $"左天 {_data.EventOn.TimeOfDay.ToString("hh\\:mm\\:ss")}";
                    case 2:
                        return $"前天 {_data.EventOn.TimeOfDay.ToString("hh\\:mm\\:ss")}";
                    default:
                        return _data.EventOn.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
        }
    }
}
