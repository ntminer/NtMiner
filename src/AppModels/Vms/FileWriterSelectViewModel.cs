using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class FileWriterSelectViewModel : ViewModelBase {
        private FileWriterViewModel _selectedResult;
        public readonly Action<FileWriterViewModel> OnOk;

        public ICommand HideView { get; set; }

        [Obsolete("这是供WPF设计时使用的构造，不应在业务代码中被调用")]
        public FileWriterSelectViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public FileWriterSelectViewModel(Action<FileWriterViewModel> onOk) {
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
