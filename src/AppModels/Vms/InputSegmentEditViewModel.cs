using NTMiner.Core;
using System;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class InputSegmentEditViewModel : ViewModelBase, IInputSegment {
        private string _name;
        private string _segment;
        private string _description;

        public ICommand Save { get; private set; }
        public Action CloseWindow { get; set; }

        public InputSegmentEditViewModel() {
            if (!Design.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public InputSegmentEditViewModel(CoinKernelViewModel coinKernelVm, InputSegment segment) {
            _name = segment.Name;
            _segment = segment.Segment;
            _description = segment.Description;
            this.Save = new DelegateCommand(() => {
                if (string.IsNullOrEmpty(this.Name)) {
                    throw new ValidationException("片段名不能为空");
                }
                segment.Name = this.Name;
                segment.Segment = this.Segment;
                segment.Description = this.Description;
                if (!coinKernelVm.InputSegments.Contains(segment)) {
                    coinKernelVm.InputSegments.Add(segment);
                }
                coinKernelVm.InputSegments = coinKernelVm.InputSegments.ToList();
                CloseWindow?.Invoke();
            });
        }

        public string Name {
            get { return _name; }
            set {
                _name = value;
                OnPropertyChanged(nameof(Name));
                if (string.IsNullOrEmpty(value)) {
                    throw new ValidationException("片段名不能为空");
                }
            }
        }

        public string Segment {
            get { return _segment; }
            set {
                _segment = value;
                OnPropertyChanged(nameof(Segment));
            }
        }

        public string Description {
            get { return _description; }
            set {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }
    }
}
