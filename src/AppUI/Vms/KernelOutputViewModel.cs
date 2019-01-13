using NTMiner.Core;
using NTMiner.Core.Kernels;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class KernelOutputViewModel : ViewModelBase, IKernelOutput {
        private Guid _id;
        private string _name;
        private string _totalSpeedPattern;
        private string _totalSharePattern;
        private string _acceptSharePattern;
        private string _gpuSpeedPattern;
        private string _rejectSharePattern;
        private string _rejectPercentPattern;

        private string _dualTotalSpeedPattern;
        private string _dualTotalSharePattern;
        private string _dualAcceptSharePattern;
        private string _dualGpuSpeedPattern;
        private string _dualRejectSharePattern;
        private string _dualRejectPercentPattern;

        private string _translaterKeyword;

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Save { get; private set; }

        public ICommand AddKernelOutputFilter { get; private set; }

        public ICommand AddKernelOutputTranslater { get; private set; }

        public ICommand ClearTranslaterKeyword { get; private set; }

        public Action CloseWindow { get; set; }

        public KernelOutputViewModel() {
            if (!DevMode.IsDevMode) {
                throw new InvalidProgramException();
            }
        }

        public KernelOutputViewModel(IKernelOutput data) : this(data.GetId()) {
            _name = data.Name;
            _gpuSpeedPattern = data.GpuSpeedPattern;
            _rejectSharePattern = data.RejectSharePattern;
            _totalSharePattern = data.TotalSharePattern;
            _acceptSharePattern = data.AcceptSharePattern;
            _rejectPercentPattern = data.RejectPercentPattern;
            _totalSpeedPattern = data.TotalSpeedPattern;
            _dualGpuSpeedPattern = data.DualGpuSpeedPattern;
            _dualRejectSharePattern = data.DualRejectSharePattern;
            _dualAcceptSharePattern = data.DualAcceptSharePattern;
            _dualRejectPercentPattern = data.DualRejectPercentPattern;
            _dualTotalSharePattern = data.DualTotalSharePattern;
            _dualTotalSpeedPattern = data.DualTotalSpeedPattern;
        }

        public KernelOutputViewModel(Guid id) {
            this._id = id;
            this.Save = new DelegateCommand(() => {
                if (NTMinerRoot.Current.KernelOutputSet.Contains(this.Id)) {
                    Global.Execute(new UpdateKernelOutputCommand(this));
                }
                else {
                    Global.Execute(new AddKernelOutputCommand(this));
                }
                CloseWindow?.Invoke();
            });
            this.Edit = new DelegateCommand(() => {
                KernelOutputEdit.ShowEditWindow(this);
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                DialogWindow.ShowDialog(message: $"您确定删除{this.Name}内核输出组吗？", title: "确认", onYes: () => {
                    Global.Execute(new RemoveKernelOutputCommand(this.Id));
                }, icon: "Icon_Confirm");
            });
            this.AddKernelOutputFilter = new DelegateCommand(() => {
                new KernelOutputFilterViewModel(Guid.NewGuid()) {
                    KernelOutputId = this.Id
                }.Edit.Execute(null);
            });
            this.AddKernelOutputTranslater = new DelegateCommand(() => {
                int sortNumber = this.KernelOutputTranslaters.Count == 0 ? 1 : this.KernelOutputTranslaters.Count + 1;
                new KernelOutputTranslaterViewModel(Guid.NewGuid()) {
                    KernelOutputId = this.Id,
                    SortNumber = sortNumber
                }.Edit.Execute(null);
            });
            this.ClearTranslaterKeyword = new DelegateCommand(() => {
                this.TranslaterKeyword = string.Empty;
            });
        }

        public List<KernelOutputFilterViewModel> KernelOutputFilters {
            get {
                return KernelOutputFilterViewModels.Current.GetListByKernelId(this.Id).ToList();
            }
        }

        public string TranslaterKeyword {
            get { return _translaterKeyword; }
            set {
                _translaterKeyword = value;
                OnPropertyChanged(nameof(TranslaterKeyword));
                OnPropertyChanged(nameof(KernelOutputTranslaters));
            }
        }

        public List<KernelOutputTranslaterViewModel> KernelOutputTranslaters {
            get {
                var query = KernelOutputTranslaterViewModels.Current.GetListByKernelId(this.Id).AsQueryable();
                if (!string.IsNullOrEmpty(TranslaterKeyword)) {
                    query = query.Where(a => (a.RegexPattern != null && a.RegexPattern.Contains(TranslaterKeyword))
                        || (a.Replacement != null && a.Replacement.Contains(TranslaterKeyword)));
                }
                return query.OrderBy(a => a.SortNumber).ToList();
            }
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id {
            get => _id;
            private set {
                _id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public string Name {
            get { return _name; }
            set {
                _name = value;
                OnPropertyChanged(nameof(Name));
                if (string.IsNullOrEmpty(value)) {
                    throw new ValidationException("名称是必须的");
                }
            }
        }

        public string TotalSpeedPattern {
            get => _totalSpeedPattern;
            set {
                _totalSpeedPattern = value;
                OnPropertyChanged(nameof(TotalSpeedPattern));
            }
        }

        public string TotalSharePattern {
            get { return _totalSharePattern; }
            set {
                _totalSharePattern = value;
                OnPropertyChanged(nameof(TotalSharePattern));
            }
        }

        public string AcceptSharePattern {
            get { return _acceptSharePattern; }
            set {
                _acceptSharePattern = value;
                OnPropertyChanged(nameof(AcceptSharePattern));
            }
        }

        public string RejectSharePattern {
            get { return _rejectSharePattern; }
            set {
                _rejectSharePattern = value;
                OnPropertyChanged(nameof(RejectSharePattern));
            }
        }

        public string RejectPercentPattern {
            get { return _rejectPercentPattern; }
            set {
                _rejectPercentPattern = value;
                OnPropertyChanged(nameof(RejectPercentPattern));
            }
        }

        public string GpuSpeedPattern {
            get => _gpuSpeedPattern;
            set {
                _gpuSpeedPattern = value;
                OnPropertyChanged(nameof(GpuSpeedPattern));
            }
        }

        public string DualTotalSpeedPattern {
            get => _dualTotalSpeedPattern;
            set {
                _dualTotalSpeedPattern = value;
                OnPropertyChanged(nameof(DualTotalSpeedPattern));
            }
        }

        public string DualTotalSharePattern {
            get { return _dualTotalSharePattern; }
            set {
                _dualTotalSharePattern = value;
                OnPropertyChanged(nameof(DualTotalSharePattern));
            }
        }

        public string DualAcceptSharePattern {
            get { return _dualAcceptSharePattern; }
            set {
                _dualAcceptSharePattern = value;
                OnPropertyChanged(nameof(DualAcceptSharePattern));
            }
        }

        public string DualRejectSharePattern {
            get { return _dualRejectSharePattern; }
            set {
                _dualRejectSharePattern = value;
                OnPropertyChanged(nameof(DualRejectSharePattern));
            }
        }

        public string DualRejectPercentPattern {
            get { return _dualRejectPercentPattern; }
            set {
                _dualRejectPercentPattern = value;
                OnPropertyChanged(nameof(DualRejectPercentPattern));
            }
        }

        public string DualGpuSpeedPattern {
            get => _dualGpuSpeedPattern;
            set {
                _dualGpuSpeedPattern = value;
                OnPropertyChanged(nameof(DualGpuSpeedPattern));
            }
        }
    }
}
