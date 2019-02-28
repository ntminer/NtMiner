using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class OverClockDataViewModels : ViewModelBase {
        public static readonly OverClockDataViewModels Current = new OverClockDataViewModels();

        private readonly Dictionary<Guid, OverClockDataViewModel> _dicById = new Dictionary<Guid, OverClockDataViewModel>();

        public ICommand Add { get; private set; }

        private OverClockDataViewModels() {
            if (Design.IsInDesignMode) {
                return;
            }
            foreach (var item in NTMinerRoot.Current.OverClockDataSet) {
                _dicById.Add(item.GetId(), new OverClockDataViewModel(item));
            }
            this.Add = new DelegateCommand(() => {
                new OverClockDataViewModel(Guid.NewGuid()).Edit.Execute(null);
            });
        }
    }
}
