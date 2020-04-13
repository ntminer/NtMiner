using NTMiner.Core;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class InputSegmentEditViewModel : ViewModelBase, IInputSegment {
        public readonly Guid Id = Guid.NewGuid();
        private SupportedGpu _targetGpu;
        private string _name;
        private string _segment;
        private string _description;
        private bool _isDefault;

        public ICommand Save { get; private set; }

        [Obsolete(message: NTKeyword.WpfDesignOnly, error: true)]
        public InputSegmentEditViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(NTKeyword.WpfDesignOnly);
            }
        }

        private readonly InputSegmentViewModel _old;
        public InputSegmentEditViewModel(CoinKernelViewModel coinKernelVm, InputSegmentViewModel segment) {
            _old = segment;
            segment = new InputSegmentViewModel(segment);
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
                bool isUpdate = !string.IsNullOrEmpty(_old.Name);
                if (isUpdate) {
                    var existItem = coinKernelVm.InputSegments.FirstOrDefault(a => a.Name == _old.Name && a.Segment == _old.Segment);
                    if (existItem != null) {
                        existItem.Update(segment);
                    }
                }
                else {
                    coinKernelVm.InputSegments.Add(new InputSegment(segment));
                }
                coinKernelVm.InputSegments = coinKernelVm.InputSegments.ToList();
                VirtualRoot.Execute(new CloseWindowCommand(this.Id));
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
                return NTMinerContext.SupportedGpuEnumItems.FirstOrDefault(a => a.Value == TargetGpu);
            }
            set {
                if (TargetGpu != value.Value) {
                    TargetGpu = value.Value;
                }
            }
        }
    }
}
