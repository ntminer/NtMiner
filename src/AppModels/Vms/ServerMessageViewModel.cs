using NTMiner.MinerServer;
using System;

namespace NTMiner.Vms {
    public class ServerMessageViewModel : ViewModelBase, IServerMessage {
        private Guid _id;
        private string _provider;
        private string _messageType;
        private string _content;
        private DateTime _timestamp;

        public ServerMessageViewModel(IServerMessage data) {
            _id = data.Id;
            _provider = data.Provider;
            _messageType = data.MessageType;
            _content = data.Content;
            _timestamp = data.Timestamp;
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

        public string Provider {
            get => _provider;
            set {
                _provider = value;
                OnPropertyChanged(nameof(Provider));
            }
        }

        public string MessageType {
            get => _messageType;
            set {
                _messageType = value;
                OnPropertyChanged(nameof(MessageType));
            }
        }

        public string Content {
            get => _content;
            set {
                _content = value;
                OnPropertyChanged(nameof(Content));
            }
        }

        public DateTime Timestamp {
            get => _timestamp;
            set {
                _timestamp = value;
                OnPropertyChanged(nameof(Timestamp));
            }
        }
    }
}
