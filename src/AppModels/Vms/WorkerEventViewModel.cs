using NTMiner.MinerClient;
using System;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class WorkerEventViewModel : ViewModelBase, IWorkerEvent {
        private readonly IWorkerEvent _data;

        public WorkerEventViewModel(IWorkerEvent data) {
            _data = data;
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
                if (_data.EventType.TryParse(out WorkerEventType eventType)) {
                    return eventType.GetDescription();
                }
                return "未知";
            }
        }

        public SolidColorBrush Foreground {
            get {
                if (_data.EventType.TryParse(out WorkerEventType eventType)) {
                    switch (eventType) {
                        case WorkerEventType.Undefined:
                            return Wpf.Util.BlackBrush;
                        case WorkerEventType.Info:
                            return Wpf.Util.BlackBrush;
                        case WorkerEventType.Warn:
                            return Wpf.Util.WarnBrush;
                        case WorkerEventType.Error:
                            return Wpf.Util.RedBrush;
                        default:
                            return Wpf.Util.BlackBrush;
                    }
                }
                return Wpf.Util.BlackBrush;
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
