using NTMiner.MinerClient;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class WorkerMessageViewModel : ViewModelBase, IWorkerMessage {
        private static readonly StreamGeometry ErrorIcon = (StreamGeometry)Application.Current.Resources["Icon_Error"];
        private static readonly StreamGeometry WarnIcon = (StreamGeometry)Application.Current.Resources["Icon_Warn"];
        private static readonly StreamGeometry InfoIcon = (StreamGeometry)Application.Current.Resources["Icon_Info"];
        private static readonly SolidColorBrush InfoColor = (SolidColorBrush)Application.Current.Resources["InfoColor"];
        private static readonly SolidColorBrush WarnColor = (SolidColorBrush)Application.Current.Resources["WarnColor"];

        public static StreamGeometry GetIcon(WorkerMessageType messageType) {
            switch (messageType) {
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

        public static SolidColorBrush GetIconFill(WorkerMessageType messageType) {
            switch (messageType) {
                case WorkerMessageType.Info:
                    return InfoColor;
                case WorkerMessageType.Warn:
                    return WarnColor;
                case WorkerMessageType.Error:
                    return WpfUtil.RedBrush;
                default:
                    return WpfUtil.BlackBrush;
            }
        }

        private readonly IWorkerMessage _data;
        private readonly WorkerMessageChannel _channel;
        private readonly WorkerMessageType _messageTypeEnum;

        public ICommand ViewDetails { get; private set; }

        public WorkerMessageViewModel(IWorkerMessage data) {
            _data = data;
            _data.MessageType.TryParse(out _messageTypeEnum);
            _data.Channel.TryParse(out _channel);
            this.ViewDetails = new DelegateCommand(() => {

            });
        }

        public WorkerMessageChannel ChannelEnum {
            get { return _channel; }
        }

        public WorkerMessageType MessageTypeEnum {
            get { return _messageTypeEnum; }
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
                return _messageTypeEnum.GetDescription();
            }
        }

        public StreamGeometry MessageTypeIcon {
            get {
                return GetIcon(_messageTypeEnum);
            }
        }

        public SolidColorBrush IconFill {
            get {
                return GetIconFill(_messageTypeEnum);
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
                int offDay = (DateTime.Now.Date - Timestamp.Date).Days;
                switch (offDay) {
                    case 0:
                        return $"今天 {Timestamp.TimeOfDay.ToString("hh\\:mm\\:ss")}";
                    case 1:
                        return $"昨天 {Timestamp.TimeOfDay.ToString("hh\\:mm\\:ss")}";
                    case 2:
                        return $"前天 {Timestamp.TimeOfDay.ToString("hh\\:mm\\:ss")}";
                    default:
                        return Timestamp.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
        }
    }
}
