using NTMiner.Core;
using System;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class KernelInputFragmentEditViewModel : ViewModelBase, IKernelInputFragment {
        private string _name;
        private string _fragment;
        private string _description;

        public ICommand Save { get; private set; }
        public Action CloseWindow { get; set; }

        public KernelInputFragmentEditViewModel(KernelInputViewModel kernelInputVm, KernelInputFragmentViewModel fragment) {
            _name = fragment.Name;
            _fragment = fragment.Fragment;
            _description = fragment.Description;
            this.Save = new DelegateCommand(() => {
                if (string.IsNullOrEmpty(this.Name)) {
                    throw new ValidationException("片段名不能为空");
                }
                fragment.Name = this.Name;
                fragment.Fragment = this.Fragment;
                fragment.Description = this.Description;
                if (!kernelInputVm.FragmentVms.Contains(fragment)) {
                    kernelInputVm.FragmentVms.Add(fragment);
                }
                kernelInputVm.FragmentVms = kernelInputVm.FragmentVms.ToList();
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

        public string Fragment {
            get { return _fragment; }
            set {
                _fragment = value;
                OnPropertyChanged(nameof(Fragment));
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
