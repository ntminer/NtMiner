using System;
using System.ComponentModel;

namespace NTMiner.Bus {
    public class MessagePath<TMessage> : IMessagePathId, INotifyPropertyChanged {
        private readonly Action<TMessage> _path;
        private bool _isEnabled;

        public event PropertyChangedEventHandler PropertyChanged;

        public MessagePath(Type location, string description, LogEnum logType, Action<TMessage> path) {
            this.IsEnabled = true;
            MessageType = typeof(TMessage);
            Location = location;
            Path = $"{location.FullName}[{MessageType.FullName}]";
            Description = description;
            LogType = logType;
            _path = path;
        }

        public Type MessageType { get; private set; }
        public Type Location { get; private set; }
        public string Path { get; private set; }
        public LogEnum LogType { get; private set; }
        public string Description { get; private set; }
        public bool IsEnabled {
            get => _isEnabled;
            set {
                _isEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabled)));
            }
        }

        public void Run(TMessage message) {
            try {
                _path?.Invoke(message);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(Path + ":" + e.Message, e);
                throw;
            }
        }
    }
}
