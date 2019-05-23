using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class FileWriterSelectViewModel : ViewModelBase {
        private FileWriterViewModel _selectedResult;
        public readonly Action<FileWriterViewModel> OnOk;

        public ICommand HideView { get; set; }

        public FileWriterSelectViewModel(FileWriterViewModel selected, Action<FileWriterViewModel> onOk) {
            _selectedResult = selected;
            OnOk = onOk;
        }

        public FileWriterViewModel SelectedResult {
            get => _selectedResult;
            set {
                if (_selectedResult != value) {
                    _selectedResult = value;
                    OnPropertyChanged(nameof(SelectedResult));
                }
            }
        }

        public List<FileWriterViewModel> FileWriterVms {
            get {
                return AppContext.Instance.FileWriterVms.List;
            }
        }
    }
}
