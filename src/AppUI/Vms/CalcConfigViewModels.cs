using System.Collections.Generic;

namespace NTMiner.Vms {
    public class CalcConfigViewModels : ViewModelBase {
        private List<CalcConfigViewModel> _calcConfigVms;

        public CalcConfigViewModels() {
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
