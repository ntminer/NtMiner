using NTMiner.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class CalcConfigViewModels : ViewModelBase {
        private List<CalcConfigViewModel> _calcConfigVms;
        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public CalcConfigViewModels() {
            if (Design.IsInDesignMode) {
                return;
            }
            this.Save = new DelegateCommand(() => {
                NTMinerRoot.Current.CalcConfigSet.SaveCalcConfigs(this.CalcConfigVms.Select(a => new CalcConfigData(a)).ToList());
                CloseWindow?.Invoke();
            });
            _calcConfigVms = new List<CalcConfigViewModel>();
            foreach (var item in NTMinerRoot.Current.CalcConfigSet) {
                _calcConfigVms.Add(new CalcConfigViewModel(item));
            }
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
