using System.Collections.Generic;

namespace NTMiner.Vms {
    public class LangViewModels : ViewModelBase {
        public static readonly LangViewModels Current = new LangViewModels();

        private readonly List<LangViewModel> _langVms = new List<LangViewModel>();

        private LangViewModels() {
            foreach (var lang in Language.LangSet.Instance) {
                _langVms.Add(new LangViewModel(lang));
            }
        }

        public List<LangViewModel> LangVms {
            get {
                return _langVms;
            }
        }
    }
}
