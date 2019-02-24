using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTMiner.Vms {
    public class AppSettingViewModels : ViewModelBase {
        public static readonly AppSettingViewModels Current = new AppSettingViewModels();
        private readonly Dictionary<string, AppSettingViewModel> _dicByKey = new Dictionary<string, AppSettingViewModel>();

        private AppSettingViewModels() {
            
        }

        public List<AppSettingViewModel> List {
            get {
                return _dicByKey.Values.ToList();
            }
        }
    }
}
