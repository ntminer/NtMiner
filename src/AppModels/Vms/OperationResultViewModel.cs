using NTMiner.Core;
using System.Windows.Media;

namespace NTMiner.Vms {
    public class OperationResultViewModel : ViewModelBase, IOperationResult {
        private static readonly StreamGeometry OkIcon = AppUtil.GetResource<StreamGeometry>("Icon_Ok");
        private static readonly StreamGeometry FailIcon = AppUtil.GetResource<StreamGeometry>("Icon_Error");
        public OperationResultViewModel() { }

        public OperationResultViewModel(OperationResultData data) {
            this.Timestamp = data.Timestamp;
            this.StateCode = data.StateCode;
            this.ReasonPhrase = data.ReasonPhrase;
            this.Description = data.Description;
        }

        public long Timestamp { get; set; }

        public int StateCode { get; set; }

        public string ReasonPhrase { get; set; }

        public string Description { get; set; }

        public StreamGeometry StateCodeIcon {
            get {
                if (StateCode == 200) {
                    return OkIcon;
                }
                return FailIcon;
            }
        }

        public SolidColorBrush IconFill {
            get {
                if (StateCode == 200) {
                    return WpfUtil.GreenBrush;
                }
                return WpfUtil.RedBrush;
            }
        }

        public string TimestampText {
            get {
                return NTMiner.Timestamp.GetTimeSpanBeforeText(NTMiner.Timestamp.FromTimestamp(this.Timestamp));
            }
        }
    }
}
