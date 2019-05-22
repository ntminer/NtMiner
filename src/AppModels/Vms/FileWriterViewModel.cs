using NTMiner.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTMiner.Vms {
    public class FileWriterViewModel : ViewModelBase, IFileWriter {
        private string _fileUrl;
        private Guid _id;
        private string _body;

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

        public string FileUrl {
            get => _fileUrl;
            set => _fileUrl = value;
        }

        public string Body {
            get => _body;
            set => _body = value;
        }
    }
}
