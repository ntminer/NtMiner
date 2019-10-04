using System;
using System.ComponentModel;

namespace NTMiner.Bus {
    /// <summary>
    /// 委托处理器，该处理器将处理逻辑委托给构造时传入的委托。
    /// </summary>
    /// <typeparam name="TMessage"></typeparam>
    public class DelegateHandler<TMessage> : IHandlerId, INotifyPropertyChanged {
        private readonly Action<TMessage> _action;
        private bool _isEnabled;

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="location">该处理器在第0级空间的位置</param>
        /// <param name="description">处理器描述</param>
        /// <param name="logType">观察日志</param>
        /// <param name="action">委托</param>
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
