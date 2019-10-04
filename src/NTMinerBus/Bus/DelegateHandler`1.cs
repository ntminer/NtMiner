using System;
using System.ComponentModel;

namespace NTMiner.Bus {
    public class DelegateHandler<TMessage> : IHandlerId, INotifyPropertyChanged {
        private readonly Action<TMessage> _action;
        private bool _isEnabled;

        public event PropertyChangedEventHandler PropertyChanged;

        public DelegateHandler(Type location, string description, LogEnum logType, Action<TMessage> action) {
            this.IsEnabled = true;
            MessageType = typeof(TMessage);
            string path = $"{location.FullName}[{MessageType.FullName}]";
            Location = location;
            HandlerPath = path;
            Description = description;
            LogType = logType;
            _action = action;
        }

        public Type MessageType { get; private set; }
        public Type Location { get; private set; }
        public string HandlerPath { get; private set; }
        public LogEnum LogType { get; private set; }
        public string Description { get; private set; }
        public bool IsEnabled {
            get => _isEnabled;
            set {
                _isEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabled)));
            }
        }

        public void Handle(TMessage message) {
            try {
                _action?.Invoke(message);
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(HandlerPath + ":" + e.Message, e);
                throw;
            }
        }
    }
}
