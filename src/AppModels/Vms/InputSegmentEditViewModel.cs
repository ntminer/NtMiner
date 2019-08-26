using NTMiner.Core;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class InputSegmentEditViewModel : ViewModelBase, IInputSegment {
        private SupportedGpu _targetGpu;
        private string _name;
        private string _segment;
        private string _description;
        private bool _isDefault;

        public ICommand Save { get; private set; }
        public Action CloseWindow { get; set; }

        public InputSegmentEditViewModel() {
            if (!Design.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public InputSegmentEditViewModel(CoinKernelViewModel coinKernelVm, InputSegmentViewModel segment) {
            _targetGpu = segment.TargetGpu;
            _name = segment.Name;
            _segment = segment.Segment;
            _description = segment.Description;
            _isDefault = segment.IsDefault;
            this.Save = new DelegateCommand(() => {
                if (string.IsNullOrEmpty(this.Name)) {
                    throw new ValidationException("片段名不能为空");
                }
                segment.TargetGpu = this.TargetGpu;
                segment.Name = this.Name;
                segment.Segment = this.Segment;
                segment.Description = this.Description;
                segment.IsDefault = this.IsDefault;
                if (!coinKernelVm.InputSegmentVms.Contains(segment)) {
                    coinKernelVm.InputSegmentVms.Add(segment);
                }
                coinKernelVm.InputSegments = coinKernelVm.InputSegments.ToList();
                CloseWindow?.Invoke();
            });
        }

        public SupportedGpu TargetGpu {
            get { return _targetGpu; }
            set {
                _targetGpu = value;
                OnPropertyChanged(nameof(TargetGpu));
            }
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
                OnPropertyChanged(nameof(IsNvidiaIconVisible));
                OnPropertyChanged(nameof(IsAMDIconVisible));
                OnPropertyChanged(nameof(TargetGpuEnumItem));
            }
        }

        public string Description {
            get { return _description; }
            set {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public bool IsDefault {
            get { return _isDefault; }
            set {
                _isDefault = value;
                OnPropertyChanged(nameof(IsDefault));
            }
        }

        public Visibility IsNvidiaIconVisible {
            get {
                if (TargetGpu == SupportedGpu.NVIDIA || TargetGpu == SupportedGpu.Both) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public Visibility IsAMDIconVisible {
            get {
                if (TargetGpu == SupportedGpu.AMD || TargetGpu == SupportedGpu.Both) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public EnumItem<SupportedGpu> TargetGpuEnumItem {
            get {
                return EnumSet.SupportedGpuEnumItems.FirstOrDefault(a => a.Value == TargetGpu);
            }
            set {
                if (TargetGpu != value.Value) {
                    TargetGpu = value.Value;
                }
            }
        }
    }
}
