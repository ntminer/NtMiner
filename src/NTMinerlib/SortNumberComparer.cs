using System;
using System.Collections.Generic;

namespace NTMiner {
    public class SortNumberComparer : IComparer<ISortable> {
        public int Compare(ISortable x, ISortable y) {
            if (x == null || y == null) {
                throw new ArgumentNullException();
            }
            return x.SortNumber - y.SortNumber;
        }
    }
}
