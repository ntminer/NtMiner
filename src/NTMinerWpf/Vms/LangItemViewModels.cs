using NTMiner.Language;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class LangItemViewModels : ViewModelBase {
        public static readonly LangItemViewModels Current = new LangItemViewModels();

        private readonly Dictionary<ILang, Dictionary<string, List<LangItemViewModel>>> _dicByLangAndView = new Dictionary<ILang, Dictionary<string, List<LangItemViewModel>>>();

        private LangItemViewModels() {
            foreach (var lang in LangSet.Instance) {
                var dic = new Dictionary<string, List<LangItemViewModel>>();
                _dicByLangAndView.Add(lang, dic);
                foreach (var item in LangItemSet.Instance.GetLangItems(lang)) {
                    if (!dic.ContainsKey(item.Key)) {
                        dic.Add(item.Key, item.Value.Select(a => new LangItemViewModel(a)).ToList());
                    }
                }
            }
        }

        public List<LangItemViewModel> GetLangItemVms(ILang lang, string viewId) {
            if (!_dicByLangAndView.ContainsKey(lang)) {
                return new List<LangItemViewModel>();
            }
            var dic = _dicByLangAndView[lang];
            if (!dic.ContainsKey(viewId)) {
                return new List<LangItemViewModel>();
            }
            return dic[viewId];
        }
    }
}
