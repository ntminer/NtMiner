using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class ColumnsShowViewModels : ViewModelBase {
        public static readonly ColumnsShowViewModels Current = new ColumnsShowViewModels();
        private readonly Dictionary<Guid, ColumnsShowViewModel> _dicById = new Dictionary<Guid, ColumnsShowViewModel>();

        private ColumnsShowViewModels() {
            foreach (var item in NTMinerRoot.Current.ColumnsShowSet) {
                _dicById.Add(item.GetId(), new ColumnsShowViewModel(item));
            }
            if (!_dicById.ContainsKey(ColumnsShowViewModel.PleaseSelect.Id)) {
                _dicById.Add(ColumnsShowViewModel.PleaseSelect.Id, ColumnsShowViewModel.PleaseSelect);
            }
        }

        public List<ColumnsShowViewModel> ColumnsShowVms {
            get {
                return _dicById.Values.ToList();
            }
        }
    }
}
