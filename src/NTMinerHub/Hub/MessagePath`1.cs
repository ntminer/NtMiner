using System;
using System.Threading;
#if DEBUG
using System.ComponentModel;
#endif

namespace NTMiner.Hub {
#if DEBUG
    public class MessagePath<TMessage> : IMessagePathId, INotifyPropertyChanged {
#else
    public class MessagePath<TMessage> : IMessagePathId {
#endif
        private readonly Action<TMessage> _path;
        private bool _isEnabled;
        private int _viaTimesLimit;
        private int _lifeLimitSeconds;

#if DEBUG
        public event PropertyChangedEventHandler PropertyChanged;
#endif

        public static MessagePath<TMessage> AddMessagePath(IMessageHub hub, Type location, string description, LogEnum logType, Action<TMessage> action, Guid pathId, int viaTimesLimit = -1, int lifeLimitSeconds = -1) {
            if (action == null) {
                throw new ArgumentNullException(nameof(action));
            }
            MessagePath<TMessage> path = new MessagePath<TMessage>(location, description, logType, action, pathId, viaTimesLimit, lifeLimitSeconds);
            hub.AddMessagePath(path);
            return path;
        }

        private MessagePath(Type location, string description, LogEnum logType, Action<TMessage> action, Guid pathId, int viaTimesLimit, int lifeLimitSeconds) {
            if (viaTimesLimit == 0) {
                throw new InvalidProgramException("消息路径的viaTimesLimit不能为0，可以为负数表示不限制通过次数或为正数表示限定通过次数，但不能为0");
            }
            _isEnabled = true;
            MessageType = typeof(TMessage);
            Location = location;
            Path = $"{location.FullName}[{MessageType.FullName}]";
            Description = description;
            LogType = logType;
            _path = action;
            PathId = pathId;
            _viaTimesLimit = viaTimesLimit;
            _lifeLimitSeconds = lifeLimitSeconds;
            CreatedOn = DateTime.Now;
        }

        public int ViaTimesLimit {
            get => _viaTimesLimit;
            private set {
                _viaTimesLimit = value;
#if DEBUG
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ViaTimesLimit)));
#endif
            }
        }

        public int LifeLimitSeconds {
            get => _lifeLimitSeconds;
            private set {
                _lifeLimitSeconds = value;
#if DEBUG
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LifeLimitSeconds)));
#endif
            }
        }

        internal void DecreaseViaTimesLimit(Action<IMessagePathId> onDownToZero) {
            int newValue = Interlocked.Decrement(ref _viaTimesLimit);
            if (newValue == 0) {
                onDownToZero?.Invoke(this);
            }
        }

        internal void DecreaseLifeLimitSeconds(Action<IMessagePathId> onDownToZero) {
            int newValue = Interlocked.Decrement(ref _lifeLimitSeconds);
            if (newValue == 0) {
                onDownToZero?.Invoke(this);
            }
        }

        public Guid PathId { get; private set; }
        public DateTime CreatedOn { get; private set; }
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

        public void Go(TMessage message) {
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
