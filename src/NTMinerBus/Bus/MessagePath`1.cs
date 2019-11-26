using System;
using System.ComponentModel;

namespace NTMiner.Bus {
    public class MessagePath<TMessage> : IMessagePathId, INotifyPropertyChanged {
        private readonly Action<TMessage> _path;
        private bool _isEnabled;
        private int _viaLimit;

        public event PropertyChangedEventHandler PropertyChanged;

        public static MessagePath<TMessage> Build(IMessageDispatcher dispatcher, Type location, string description, LogEnum logType, Action<TMessage> action, int viaLimit = -1) {
            if (action == null) {
                throw new ArgumentNullException(nameof(action));
            }
            MessagePath<TMessage> handler = new MessagePath<TMessage>(location, description, logType, action, viaLimit);
            dispatcher.Connect(handler);
            return handler;
        }

        private MessagePath(Type location, string description, LogEnum logType, Action<TMessage> path, int viaLimit) {
            this.IsEnabled = true;
            MessageType = typeof(TMessage);
            Location = location;
            Path = $"{location.FullName}[{MessageType.FullName}]";
            Description = description;
            LogType = logType;
            _path = path;
            ViaLimit = viaLimit;
        }

        public int ViaLimit {
            get => _viaLimit;
            internal set {
                _viaLimit = value;
#if DEBUG
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ViaLimit)));
#endif
            }
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
#if DEBUG
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsEnabled)));
#endif
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
