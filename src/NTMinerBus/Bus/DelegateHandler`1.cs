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

        public DelegateHandler(Type messageType, Type location, string description, LogEnum logType, Action<TMessage> action) {
            this.IsEnabled = true;
            string path = $"{location.FullName}[{messageType.FullName}]";
            MessageType = messageType;
            Location = location;
            HandlerPath = path;
            Description = description;
            LogType = logType;
            _action = action;
        }

        public Type MessageType { get; set; }
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
