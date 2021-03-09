using NTMiner.Core.MinerServer;
using NTMiner.Vms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.MinerStudio.Vms {
    public class CalcConfigViewModels : ViewModelBase {
        private List<CalcConfigViewModel> _calcConfigVms = new List<CalcConfigViewModel>();
        public readonly Guid Id = Guid.NewGuid();
        public ICommand Save { get; private set; }
        public ICommand Clear { get; private set; }

        public CalcConfigViewModels() {
            if (WpfUtil.IsInDesignMode) {
                return;
            }
            this.Save = new DelegateCommand(() => {
                NTMinerContext.Instance.CalcConfigSet.SaveCalcConfigs(this.CalcConfigVms.Select(a => CalcConfigData.Create(a)).ToList());
                VirtualRoot.Execute(new CloseWindowCommand(this.Id));
            });
            this.Clear = new DelegateCommand(() => {
                var coins = NTMinerContext.Instance.ServerContext.CoinSet.AsEnumerable();
                if (coins != null && coins.Any()) {
                    List<CalcConfigViewModel> toRemoves = new List<CalcConfigViewModel>();
                    foreach (var calConfigVm in _calcConfigVms) {
                        if (!coins.Any(a=>a.Code == calConfigVm.CoinCode)) {
                            toRemoves.Add(calConfigVm);
                        }
                    }
                    if (toRemoves.Count != 0) {
                        foreach (var item in toRemoves) {
                            _calcConfigVms.Remove(item);
                        }
                        this.Save.Execute(null);
                        VirtualRoot.Out.ShowSuccess("清理成功");
                    }
                    else {
                        VirtualRoot.Out.ShowSuccess("没有要清理的币种");
                    }
                }
            });
            Refresh();
        }

        public void Refresh() {
            var list = new List<CalcConfigViewModel>();
            foreach (var item in NTMinerContext.Instance.CalcConfigSet.AsEnumerable().ToArray()) {
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
