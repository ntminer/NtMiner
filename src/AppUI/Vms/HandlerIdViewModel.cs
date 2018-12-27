using System;

namespace NTMiner.Vms {
    public class HandlerIdViewModel : ViewModelBase, IHandlerId {
        private Guid _id;
        private Type _messageType;
        private Type _location;
        private LogEnum _logType;
        private string _description;

        public HandlerIdViewModel(IHandlerId data) {
            _id = data.Id;
            _messageType = data.MessageType;
            _location = data.Location;
            _logType = data.LogType;
            _description = data.Description;
        }

        public void Update(IHandlerId data) {
            this.MessageType = data.MessageType;
            this.Description = data.Description;
            this.LogType = data.LogType;
            this.Location = data.Location;
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id {
            get => _id;
            set {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public Type MessageType {
            get { return _messageType; }
            set {
                _messageType = value;
                OnPropertyChanged(nameof(MessageType));
            }
        }

        public Type Location {
            get => _location;
            set {
                _location = value;
                OnPropertyChanged(nameof(Location));
            }
        }

        public LogEnum LogType {
            get => _logType;
            set {
                _logType = value;
                OnPropertyChanged(nameof(LogType));
            }
        }

        public EnumItem<LogEnum> LogTypeItem {
            get {
                return this.LogType.GetEnumItem();
            }
            set {
                this.LogType = value.Value;
            }
        }

        public string Description {
            get => _description;
            set {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
    }
}
