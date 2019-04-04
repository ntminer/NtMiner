using NTMiner.Core;
using System;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class KernelInputFragmentEditViewModel : ViewModelBase {
        private string _name;
        private string _fragment;

        public ICommand Save { get; private set; }
        public Action CloseWindow { get; set; }

        public KernelInputFragmentEditViewModel(KernelInputViewModel kernelInputVm, KernelInputFragment fragment) {
            _name = fragment.Name;
            _fragment = fragment.Fragment;
            this.Save = new DelegateCommand(() => {
                if (string.IsNullOrEmpty(this.Name)) {
                    throw new ValidationException("片段名不能为空");
                }
                fragment.Name = this.Name;
                fragment.Fragment = this.Fragment;
                if (!kernelInputVm.Fragments.Contains(fragment)) {
                    kernelInputVm.Fragments.Add(fragment);
                }
                kernelInputVm.Fragments = kernelInputVm.Fragments.ToList();
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
    }
}
