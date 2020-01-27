using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class CalcConfigViewModels : ViewModelBase {
        private List<CalcConfigViewModel> _calcConfigVms = new List<CalcConfigViewModel>();
        public readonly Guid Id = Guid.NewGuid();
        public ICommand Save { get; private set; }

        public CalcConfigViewModels() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Save = new DelegateCommand(() => {
                NTMinerRoot.Instance.CalcConfigSet.SaveCalcConfigs(this.CalcConfigVms.Select(a => CalcConfigData.Create(a)).ToList());
                VirtualRoot.Execute(new CloseWindowCommand(this.Id));
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
