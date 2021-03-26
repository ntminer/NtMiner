using NTMiner.ServerNode;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public static class ActionCountRoot {
        private static readonly Dictionary<string, ActionCountData> _actionCounts = new Dictionary<string, ActionCountData>();
        public static IEnumerable<ActionCountData> ActionCounts {
            get { return _actionCounts.Values; }
        }

        public static void Count(string actionName) {
            if (_actionCounts.TryGetValue(actionName, out ActionCountData count)) {
                if (count.Count == long.MaxValue) {
                    count.Count = 0;
                }
                count.Count += 1;
            }
            else {
                _actionCounts[actionName] = new ActionCountData {
                    ActionName = actionName,
                    Count = 1
                };
            }
        }

        public static List<ActionCountData> QueryActionCounts(QueryActionCountsRequest query, out int total) {
            List<ActionCountData> list = new List<ActionCountData>();
            bool isFilterByKeyword = !string.IsNullOrEmpty(query.Keyword);
            if (isFilterByKeyword) {
                foreach (var item in _actionCounts) {
                    if (item.Key.Contains(query.Keyword)) {
                        list.Add(item.Value);
                    }
                }
            }
            else {
                list.AddRange(_actionCounts.Values);
            }
            total = list.Count;
            return list.OrderBy(a => a.ActionName).Take(paging: query).ToList();
        }
    }
}
