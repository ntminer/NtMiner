using NTMiner.Core;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class WorkerMessageViewModel : ViewModelBase, IWorkerMessage {
        private static readonly StreamGeometry ErrorIcon = (StreamGeometry)Application.Current.Resources["Icon_Error"];
        private static readonly StreamGeometry WarnIcon = (StreamGeometry)Application.Current.Resources["Icon_Waring"];
        private static readonly StreamGeometry InfoIcon = (StreamGeometry)Application.Current.Resources["Icon_Info"];
        private static readonly SolidColorBrush IconFillColor = (SolidColorBrush)Application.Current.Resources["IconFillColor"];
        private static readonly SolidColorBrush WarnColor = (SolidColorBrush)Application.Current.Resources["WarnColor"];

        private readonly IWorkerMessage _data;
        private readonly WorkerMessageChannel _channel;
        private readonly WorkerMessageType _messageType;

        public ICommand ViewDetails { get; private set; }

        public WorkerMessageViewModel(IWorkerMessage data) {
            _data = data;
            _data.MessageType.TryParse(out _messageType);
            _data.Channel.TryParse(out _channel);
            this.ViewDetails = new DelegateCommand(() => {

            });
        }

        public WorkerMessageChannel ChannelEnum {
            get { return _channel; }
        }

        public WorkerMessageType MessageTypeEnum {
            get { return _messageType; }
        }

        public Guid GetId() {
            return _data.Id;
        }

        public Guid Id {
            get {
                return _data.Id;
            }
        }

        public string Channel {
            get {
                return _data.Channel;
            }
        }

        public string Provider {
            get {
                return _data.Provider;
            }
        }

        public string ChannelText {
            get {
                if (_channel != WorkerMessageChannel.Unspecified) {
                    return _channel.GetDescription();
                }
                return "未知";
            }
        }

        public string MessageType {
            get {
                return _data.MessageType;
            }
        }

        public string MessageTypeText {
            get {
                if (_messageType != WorkerMessageType.Undefined) {
                    return _messageType.GetDescription();
                }
                return "未知";
            }
        }

        public StreamGeometry MessageTypeIcon {
            get {
                switch (_messageType) {
                    case WorkerMessageType.Undefined:
                        return null;
                    case WorkerMessageType.Info:
                        return InfoIcon;
                    case WorkerMessageType.Warn:
                        return WarnIcon;
                    case WorkerMessageType.Error:
                        return ErrorIcon;
                    default:
                        return null;
                }
            }
        }

        public SolidColorBrush IconFill {
            get {
                switch (_messageType) {
                    case WorkerMessageType.Undefined:
                        return WpfUtil.BlackBrush;
                    case WorkerMessageType.Info:
                        return IconFillColor;
                    case WorkerMessageType.Warn:
                        return WarnColor;
                    case WorkerMessageType.Error:
                        return WpfUtil.RedBrush;
                    default:
                        return WpfUtil.BlackBrush;
                }
            }
        }

        public SolidColorBrush Foreground {
            get {
                switch (_messageType) {
                    case WorkerMessageType.Undefined:
                        return WpfUtil.BlackBrush;
                    case WorkerMessageType.Info:
                        return WpfUtil.BlackBrush;
                    case WorkerMessageType.Warn:
                        return WarnColor;
                    case WorkerMessageType.Error:
                        return WpfUtil.RedBrush;
                    default:
                        return WpfUtil.BlackBrush;
                }
            }
        }

        public string Content {
            get {
                return _data.Content;
            }
        }

        public DateTime Timestamp {
            get {
                return _data.Timestamp;
            }
        }

        public string TimestampText {
            get {
                int offDay = (DateTime.Now.Date - _data.Timestamp.Date).Days;
                switch (offDay) {
                    case 0:
                        return $"今天 {_data.Timestamp.TimeOfDay.ToString("hh\\:mm\\:ss")}";
                    case 1:
                        return $"昨天 {_data.Timestamp.TimeOfDay.ToString("hh\\:mm\\:ss")}";
                    case 2:
                        return $"前天 {_data.Timestamp.TimeOfDay.ToString("hh\\:mm\\:ss")}";
                    default:
                        return _data.Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
        }
    }
}
