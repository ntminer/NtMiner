using NTMiner.Bus;
using System;
using System.ComponentModel;

namespace NTMiner {
    public partial class HandlerId : IHandlerId, INotifyPropertyChanged {
        private bool _isEnabled;

        public event PropertyChangedEventHandler PropertyChanged;

        public static IHandlerId Create(Type messageType, Type location, string description, LogEnum logType) {
            string path = $"{location.FullName}[{messageType.FullName}]";
            var item = new HandlerId {
                MessageType = messageType,
                Location = location,
                HandlerPath = path,
                Description = description,
                LogType = logType
            };
            return item;
        }

        private HandlerId() {
            this.IsEnabled = true;
        }

        public Type MessageType { get; set; }
        [LiteDB.BsonIgnore]
        public Type Location { get; set; }
        public string HandlerPath { get; set; }
        public LogEnum LogType { get; set; }
        public string Description { get; set; }
        public bool IsEnabled {
            get => _isEnabled;
            set {
                _isEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabled)));
            }
        }
    }
}
