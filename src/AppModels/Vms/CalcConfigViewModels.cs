using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class CalcConfigViewModels : ViewModelBase {
        private List<CalcConfigViewModel> _calcConfigVms = new List<CalcConfigViewModel>();
        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public CalcConfigViewModels() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Save = new DelegateCommand(() => {
                NTMinerRoot.Instance.CalcConfigSet.SaveCalcConfigs(this.CalcConfigVms.Select(a => CalcConfigData.Create(a)).ToList());
                CloseWindow?.Invoke();
            });
            Refresh();
        }

        public void Refresh() {
            var list = new List<CalcConfigViewModel>();
            foreach (var item in NTMinerRoot.Instance.CalcConfigSet.AsEnumerable()) {
                list.Add(new CalcConfigViewModel(item));
            }
            CalcConfigVms = list;
        }

        public List<CalcConfigViewModel> CalcConfigVms {
            get => _calcConfigVms;
            set {
                _calcConfigVms = value;
                OnPropertyChanged(nameof(CalcConfigVms));
            }
        }
    }
}
