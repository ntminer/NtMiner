using NTMiner.Core;
using System.Windows;

namespace NTMiner.Vms {
    public class InputSegmentViewModel : ViewModelBase, IInputSegment {
        private SupportedGpu _targetGpu;
        private string _name;
        private string _segment;
        private string _description;
        private bool _isDefault;

        public InputSegmentViewModel() { }

        public InputSegmentViewModel(IInputSegment data) {
            _targetGpu = data.TargetGpu;
            _name = data.Name;
            _segment = data.Segment;
            _description = data.Description;
            _isDefault = data.IsDefault;
        }

        public SupportedGpu TargetGpu {
            get { return _targetGpu; }
            set {
                _targetGpu = value;
                OnPropertyChanged(nameof(TargetGpu));
            }
        }

        public string Name {
            get => _name;
            set {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public string Segment {
            get => _segment;
            set {
                _segment = value;
                OnPropertyChanged(nameof(Segment));
                OnPropertyChanged(nameof(IsNvidiaIconVisible));
                OnPropertyChanged(nameof(IsAmdIconVisible));
            }
        }

        public string Description {
            get => _description;
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

        public Visibility IsAmdIconVisible {
            get {
                if (TargetGpu == SupportedGpu.AMD || TargetGpu == SupportedGpu.Both) {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public bool IsChecked {
            get {
                if (this.IsDefault) {
                    CoinKernelProfileViewModel coinKernelProfileVm = AppRoot.MinerProfileVm.CoinVm?.CoinKernel?.CoinKernelProfile;
                    if (coinKernelProfileVm != null) {
                        if (string.IsNullOrEmpty(coinKernelProfileVm.TouchedArgs)) {
                            return true;
                        }
                        // 用户未触碰过则使用默认值
                        if (!coinKernelProfileVm.TouchedArgs.Contains(this.Segment)) {
                            return true;
                        }
                    }
                }
                if (AppRoot.MinerProfileVm.CoinVm?.CoinKernel?.CoinKernelProfile?.CustomArgs?.Contains(this.Segment) ?? false) {
                    return true;
                }
                return false;
            }
            set {
                CoinKernelProfileViewModel coinKernelProfileVm = AppRoot.MinerProfileVm.CoinVm?.CoinKernel?.CoinKernelProfile;
                string customArgs = (coinKernelProfileVm?.CustomArgs ?? string.Empty).Trim();
                if (coinKernelProfileVm != null) {
                    if (coinKernelProfileVm.TouchedArgs == null) {
                        coinKernelProfileVm.TouchedArgs = string.Empty;
                    }
                    if (!coinKernelProfileVm.TouchedArgs.Contains(this.Segment)) {
                        // 记录下用户是否触碰过该自定义参数，如果用户触碰过则IsDefault失效
                        if (coinKernelProfileVm.TouchedArgs.Length == 0) {
                            coinKernelProfileVm.TouchedArgs += this.Segment;
                        }
                        else {
                            coinKernelProfileVm.TouchedArgs += " " + this.Segment;
                        }
                    }
                }
                bool b = customArgs.Contains(this.Segment);
                if (value) {
                    if (b == false) {
                        if (string.IsNullOrEmpty(customArgs)) {
                            customArgs = this.Segment;
                        }
                        else {
                            customArgs += " " + this.Segment;
                        }
                    }
                }
                else {
                    if (b == true) {
                        string str = " " + this.Segment;
                        if (!customArgs.Contains(str)) {
                            str = this.Segment;
                        }
                        customArgs = customArgs.Replace(str, string.Empty);
                    }
                }
                if (coinKernelProfileVm != null) {
                    coinKernelProfileVm.CustomArgs = customArgs;
                }
                OnPropertyChanged(nameof(IsChecked));
            }
        }
    }
}
