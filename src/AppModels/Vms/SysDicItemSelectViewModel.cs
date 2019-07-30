using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class SysDicItemSelectViewModel : ViewModelBase {
        private string _keyword;
        private SysDicItemViewModel _selectedResult;
        public readonly Action<SysDicItemViewModel> OnOk;
        private readonly IEnumerable<SysDicItemViewModel> _sysDicItemVms;

        public ICommand ClearKeyword { get; private set; }
        public ICommand HideView { get; set; }

        public SysDicItemSelectViewModel(IEnumerable<SysDicItemViewModel> sysDicItemVms, SysDicItemViewModel selected, Action<SysDicItemViewModel> onOk) {
            _sysDicItemVms = sysDicItemVms;
            _selectedResult = selected;
            OnOk = onOk;
            this.ClearKeyword = new DelegateCommand(() => {
                this.Keyword = string.Empty;
            });
        }

        public string Keyword {
            get => _keyword;
            set {
                if (_keyword != value) {
                    _keyword = value;
                    OnPropertyChanged(nameof(Keyword));
                    OnPropertyChanged(nameof(QueryResults));
                }
            }
        }

        public SysDicItemViewModel SelectedResult {
            get => _selectedResult;
            set {
                if (_selectedResult != value) {
                    _selectedResult = value;
                    OnPropertyChanged(nameof(SelectedResult));
                }
            }
        }

        public List<SysDicItemViewModel> QueryResults {
            get {
                if (!string.IsNullOrEmpty(Keyword)) {
                    return _sysDicItemVms.Where(a => 
                        (a.Code != null && a.Code.IgnoreCaseContains(Keyword)) || 
                        (a.Value != null && a.Value.IgnoreCaseContains(Keyword)) || 
                        (a.Description != null && a.Description.IgnoreCaseContains(Keyword))).OrderBy(a => a.SortNumber).ToList();
                }
                return _sysDicItemVms.OrderBy(a => a.SortNumber).ToList();
            }
        }
    }
}
