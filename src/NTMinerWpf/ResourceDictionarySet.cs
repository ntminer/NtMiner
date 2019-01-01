using NTMiner.Language;
using System.Collections.Generic;
using System.Windows;

namespace NTMiner {
    public class ResourceDictionarySet {
        public static readonly ResourceDictionarySet Instance = new ResourceDictionarySet();

        private readonly Dictionary<string, ResourceDictionary> _dicByViewId = new Dictionary<string, ResourceDictionary>();

        public bool TryGetResourceDic(string viewId, out ResourceDictionary resourceDictionary) {
            resourceDictionary = null;
            if (!_dicByViewId.ContainsKey(viewId)) {
                return false;
            }
            return _dicByViewId.TryGetValue(viewId, out resourceDictionary);
        }

        public void FillResourceDic(string viewId, ResourceDictionary resourceDictionary) {
            if (!_dicByViewId.ContainsKey(viewId)) {
                _dicByViewId.Add(viewId, resourceDictionary);
            }
            IList<ILangViewItem> langItems = LangViewItemSet.Instance.GetLangItems(Global.CurrentLang, viewId);
            foreach (var item in langItems) {
                resourceDictionary[item.Key] = item.Value;
            }
        }
    }
}
