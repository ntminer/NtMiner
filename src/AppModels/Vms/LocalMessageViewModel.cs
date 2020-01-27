using NTMiner.Core.MinerClient;
using System;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class LocalMessageViewModel : ViewModelBase, ILocalMessage {
        private static readonly StreamGeometry ErrorIcon = AppUtil.GetResource<StreamGeometry>("Icon_Error");
        private static readonly StreamGeometry WarnIcon = AppUtil.GetResource<StreamGeometry>("Icon_Warn");
        private static readonly StreamGeometry InfoIcon = AppUtil.GetResource<StreamGeometry>("Icon_Message");
        private static readonly SolidColorBrush InfoColor = AppUtil.GetResource<SolidColorBrush>("InfoColor");
        private static readonly SolidColorBrush WarnColor = AppUtil.GetResource<SolidColorBrush>("WarnColor");

        public static StreamGeometry GetIcon(LocalMessageType messageType) {
            switch (messageType) {
                case LocalMessageType.Info:
                    return InfoIcon;
                case LocalMessageType.Warn:
                    return WarnIcon;
                case LocalMessageType.Error:
                    return ErrorIcon;
                default:
                    return null;
            }
        }

        public static SolidColorBrush GetIconFill(LocalMessageType messageType) {
            switch (messageType) {
                case LocalMessageType.Info:
                    return InfoColor;
                case LocalMessageType.Warn:
                    return WarnColor;
                case LocalMessageType.Error:
                    return WpfUtil.RedBrush;
                default:
                    return WpfUtil.BlackBrush;
            }
        }

        private readonly ILocalMessage _data;
        private readonly LocalMessageChannel _channel;
        private readonly LocalMessageType _messageTypeEnum;

        public LocalMessageViewModel(ILocalMessage data) {
            _data = data;
            data.MessageType.TryParse(out _messageTypeEnum);
            data.Channel.TryParse(out _channel);
        }

        public LocalMessageChannel ChannelEnum {
            get { return _channel; }
        }

        public LocalMessageType MessageTypeEnum {
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
                if (_channel != LocalMessageChannel.Unspecified) {
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
