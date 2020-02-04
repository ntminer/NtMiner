using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class CoinSelectViewModel : ViewModelBase {
        private string _keyword;
        private CoinViewModel _selectedResult;
        private CoinViewModel _selectedHotCoin;
        public readonly Action<CoinViewModel> OnOk;
        private readonly IEnumerable<CoinViewModel> _coins;

        public ICommand ClearKeyword { get; private set; }
        public ICommand HideView { get; set; }

        [Obsolete(message: NTKeyword.WpfDesignOnly, error: true)]
        public CoinSelectViewModel() {
            if (!WpfUtil.IsInDesignMode) {
                throw new InvalidProgramException(NTKeyword.WpfDesignOnly);
            }
        }

        public CoinSelectViewModel(
            IEnumerable<CoinViewModel> coins,
            CoinViewModel selected,
            Action<CoinViewModel> onOk,
            bool isPromoteHotCoin = false) {
            _coins = coins;
            _selectedResult = selected;
            if (selected != null && selected.IsHot) {
                _selectedHotCoin = selected;
            }
            OnOk = onOk;
            IsPromoteHotCoin = isPromoteHotCoin;
            this.ClearKeyword = new DelegateCommand(() => {
                this.Keyword = string.Empty;
            });
        }

        public bool IsPromoteHotCoin {
            get; set;
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

        public CoinViewModel SelectedResult {
            get => _selectedResult;
            set {
                if (_selectedResult != value) {
                    _selectedResult = value;
                    if (value != null && value.IsHot) {
                        _selectedHotCoin = value;
                    }
                    else {
                        _selectedHotCoin = null;
                    }
                    OnPropertyChanged(nameof(SelectedResult));
                    OnPropertyChanged(nameof(SelectedHotCoin));
                }
            }
        }

        public CoinViewModel SelectedHotCoin {
            get => _selectedHotCoin;
            set {
                SelectedResult = value;
            }
        }

        public List<CoinViewModel> QueryResults {
            get {
                if (!string.IsNullOrEmpty(Keyword)) {
                    return _coins.Where(a =>
                        (a.Code != null && a.Code.IgnoreCaseContains(Keyword)) ||
                        (a.CnName != null && a.CnName.IgnoreCaseContains(Keyword)) ||
                        (a.EnName != null && a.EnName.IgnoreCaseContains(Keyword))).OrderBy(a => a.Code).ToList();
                }
                return _coins.OrderBy(a => a.Code).ToList();
            }
        }

        public List<CoinViewModel> HotCoins {
            get {
                if (!IsPromoteHotCoin) {
                    return new List<CoinViewModel>();
                }
                return _coins.Where(a => a.IsHot).OrderBy(a => a.Code).ToList();
            }
        }
    }
}
