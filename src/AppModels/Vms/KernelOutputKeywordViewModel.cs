using NTMiner.Core;
using System;

namespace NTMiner.Vms {
    public class KernelOutputKeywordViewModel : ViewModelBase, IKernelOutputKeyword {
        private Guid _id;
        private Guid _kernelOutputId;
        private string _messageType;
        private string _keyword;

        public KernelOutputKeywordViewModel() {

        }

        public Guid GetId() {
            throw new NotImplementedException();
        }

        public Guid Id {
            get => _id;
            set {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public Guid KernelOutputId {
            get => _kernelOutputId;
            set {
                _kernelOutputId = value;
                OnPropertyChanged(nameof(KernelOutputId));
            }
        }

        public string MessageType {
            get => _messageType;
            set {
                _messageType = value;
                OnPropertyChanged(nameof(MessageType));
            }
        }

        public string Keyword {
            get => _keyword;
            set {
                _keyword = value;
                OnPropertyChanged(nameof(Keyword));
            }
        }
    }
}
