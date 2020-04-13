using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner {
    public static class SortableExtension {
        public static T GetNextOne<T>(this IEnumerable<T> items, int sortNumber, Func<T, bool> predicate = null) where T : ISortable {
            if (predicate != null) {
                items = items.Where(a => predicate(a));
            }
            return items.OrderBy(a => a.SortNumber).FirstOrDefault(a => a.SortNumber > sortNumber);
        }

        public static T GetUpOne<T>(this IEnumerable<T> items, int sortNumber, Func<T, bool> predicate = null) where T : ISortable {
            if (predicate != null) {
                items = items.Where(a => predicate(a));
            }
            return items.OrderByDescending(a => a.SortNumber).FirstOrDefault(a => a.SortNumber < sortNumber);
        }
    }
}
