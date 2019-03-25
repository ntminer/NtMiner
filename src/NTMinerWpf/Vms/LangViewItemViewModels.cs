using NTMiner.Language;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Vms {
    public class LangViewItemViewModels : ViewModelBase {
        public static readonly LangViewItemViewModels Current = new LangViewItemViewModels();

        private readonly Dictionary<Guid, LangViewItemViewModel> _dicById = new Dictionary<Guid, LangViewItemViewModel>();
        private readonly Dictionary<LangViewModel, Dictionary<string, List<LangViewItemViewModel>>> _dicByLangAndView = new Dictionary<LangViewModel, Dictionary<string, List<LangViewItemViewModel>>>();

        private LangViewItemViewModels() {
            VirtualRoot.On<LangViewItemAddedEvent>("添加了语言项后刷新VM内存", LogEnum.None,
                action: message => {
                    if (_dicById.ContainsKey(message.Source.GetId())) {
                        return;
                    }
                    LangViewModel langVm;
                    if (LangViewModels.Current.TryGetLangVm(message.Source.LangId, out langVm)) {
                        Dictionary<string, List<LangViewItemViewModel>> dic = null;
                        if (!_dicByLangAndView.ContainsKey(langVm)) {
                            dic = new Dictionary<string, List<LangViewItemViewModel>>();
                            _dicByLangAndView.Add(langVm, dic);
                        }
                        else {
                            dic = _dicByLangAndView[langVm];
                        }
                        if (!dic.ContainsKey(message.Source.ViewId)) {
                            dic.Add(message.Source.ViewId, new List<LangViewItemViewModel>());
                        }
                        var entity = new LangViewItemViewModel(message.Source);
                        dic[message.Source.ViewId].Add(entity);
                        _dicById.Add(entity.Id, entity);
                    }
                });
            VirtualRoot.On<LangViewItemUpdatedEvent>("更新了语言项后刷新VM内存", LogEnum.None,
                action: message => {
                    LangViewItemViewModel langItemVm;
                    if (_dicById.TryGetValue(message.Source.GetId(), out langItemVm)) {
                        langItemVm.Update(message.Source);
                    }
                });
            VirtualRoot.On<LangViewItemRemovedEvent>("删除了语言项后刷新VM内存", LogEnum.None,
                action: message => {
                    LangViewItemViewModel langItemVm;
                    if (_dicById.TryGetValue(message.Source.GetId(), out langItemVm)) {
                        _dicById.Remove(langItemVm.Id);
                        LangViewModel langVm;
                        if (LangViewModels.Current.TryGetLangVm(langItemVm.LangId, out langVm)) {
                            if (_dicByLangAndView.ContainsKey(langVm) && _dicByLangAndView[langVm].ContainsKey(langItemVm.ViewId)) {
                                _dicByLangAndView[langVm][langItemVm.ViewId].Remove(langItemVm);
                                if (_dicByLangAndView[langVm][langItemVm.ViewId].Count == 0) {
                                    _dicByLangAndView[langVm].Remove(langItemVm.ViewId);
                                    if (_dicByLangAndView[langVm].Count == 0) {
                                        _dicByLangAndView.Remove(langVm);
                                    }
                                }
                            }
                        }
                    }
                });
            Init();
        }

        private void Init() {
            _dicByLangAndView.Clear();
            _dicById.Clear();
            foreach (var lang in LangViewModels.Current.LangVms) {
                var dic = new Dictionary<string, List<LangViewItemViewModel>>();
                _dicByLangAndView.Add(lang, dic);
                foreach (var item in LangViewItemSet.Instance.GetLangItems(lang.Id)) {
                    if (!dic.ContainsKey(item.Key)) {
                        List<LangViewItemViewModel> list = item.Value.Select(a => new LangViewItemViewModel(a)).ToList();
                        dic.Add(item.Key, list);
                        foreach (var langViewItemVm in list) {
                            _dicById.Add(langViewItemVm.Id, langViewItemVm);
                        }
                    }
                }
            }
        }

        public bool Contains(Guid langViewItemId) {
            return _dicById.ContainsKey(langViewItemId);
        }

        public bool TryGetLangViewItemVm(Guid langViewItemId, out LangViewItemViewModel vm) {
            return _dicById.TryGetValue(langViewItemId, out vm);
        }

        public List<LangViewItemViewModel> GetLangItemVms(LangViewModel lang, string viewId) {
            if (!_dicByLangAndView.ContainsKey(lang)) {
                return new List<LangViewItemViewModel>();
            }
            var dic = _dicByLangAndView[lang];
            if (!dic.ContainsKey(viewId)) {
                return new List<LangViewItemViewModel>();
            }
            return dic[viewId];
        }
    }
}
