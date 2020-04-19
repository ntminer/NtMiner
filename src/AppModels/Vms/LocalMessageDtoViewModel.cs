using NTMiner.Core;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class LocalMessageDtoViewModel : ViewModelBase, ILocalMessageDto {
        public LocalMessageDtoViewModel(LocalMessageDto dto) {
            this.Channel = dto.Channel;
            this.Content = dto.Content;
            this.MessageType = dto.MessageType;
            this.Provider = dto.Provider;
            this.Timestamp = dto.Timestamp;
        }

        public long Timestamp { get; set; }

        public LocalMessageChannel Channel { get; set; }

        public LocalMessageType MessageType { get; set; }

        public string Provider { get; set; }

        public string Content { get; set; }

        public string ChannelText {
            get {
                if (this.Channel == LocalMessageChannel.Unspecified) {
                    return "未知";
                }
                return this.Channel.GetDescription();
            }
        }

        public string MessageTypeText {
            get {
                return this.MessageType.GetDescription();
            }
        }

        public StreamGeometry MessageTypeIcon {
            get {
                return LocalMessageViewModel.GetIcon(this.MessageType);
            }
        }

        public SolidColorBrush IconFill {
            get {
                return LocalMessageViewModel.GetIconFill(this.MessageType);
            }
        }

        public string TimestampText {
            get {
                return NTMiner.Timestamp.GetTimeSpanText(NTMiner.Timestamp.FromTimestamp(this.Timestamp));
            }
        }
    }
}
