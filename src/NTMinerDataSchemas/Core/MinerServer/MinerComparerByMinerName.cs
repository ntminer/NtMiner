using System.Collections.Generic;

namespace NTMiner.Core.MinerServer {
    public class MinerComparerByMinerName : IComparer<IMinerData> {
        private readonly SortDirection _sortDirection;
        public MinerComparerByMinerName(SortDirection sortDirection) {
            _sortDirection = sortDirection;
        }

        public int Compare(IMinerData x, IMinerData y) {
            bool xIsNull = x == null;
            bool yIsNull = y == null;
            if (xIsNull && yIsNull) {
                return 0;
            }
            if (xIsNull) {
                switch (_sortDirection) {
                    case SortDirection.Ascending:
                        return -1;
                    case SortDirection.Descending:
                        return 1;
                    default:
                        return 0;
                }
            }
            if (yIsNull) {
                switch (_sortDirection) {
                    case SortDirection.Ascending:
                        return 1;
                    case SortDirection.Descending:
                        return -1;
                    default:
                        return 0;
                }
            }
            bool xMinerNameIsNullOrEmpty = string.IsNullOrEmpty(x.MinerName);
            bool yMinerNameIsNullOrEmpty = string.IsNullOrEmpty(y.MinerName);
            if (xMinerNameIsNullOrEmpty && yMinerNameIsNullOrEmpty) {
                return 0;
            }
            if (xMinerNameIsNullOrEmpty) {
                switch (_sortDirection) {
                    case SortDirection.Ascending:
                        return -1;
                    case SortDirection.Descending:
                        return 1;
                    default:
                        return 0;
                }
            }
            if (yMinerNameIsNullOrEmpty) {
                switch (_sortDirection) {
                    case SortDirection.Ascending:
                        return 1;
                    case SortDirection.Descending:
                        return -1;
                    default:
                        return 0;
                }
            }
            int r = x.MinerName.CompareTo(y.MinerName);
            switch (_sortDirection) {
                case SortDirection.Ascending:
                    return r;
                case SortDirection.Descending:
                    return -r;
                default:
                    return 0;
            }
        }
    }
}
