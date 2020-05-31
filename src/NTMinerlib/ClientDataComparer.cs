using NTMiner.Core.MinerServer;
using System.Collections.Generic;

namespace NTMiner {
    public class ClientDataComparer : IComparer<IClientData> {
        private readonly SortDirection _sortDirection;
        private readonly ClientDataSortField _sortField;
        public ClientDataComparer(SortDirection sortDirection, ClientDataSortField sortField) {
            _sortDirection = sortDirection;
            _sortField = sortField;
        }

        public int Compare(IClientData x, IClientData y) {
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
            switch (_sortField) {
                case ClientDataSortField.MinerName: {
                        bool xMinerNameIsNullOrEmpty = string.IsNullOrEmpty(((IMinerData)x).MinerName);
                        bool yMinerNameIsNullOrEmpty = string.IsNullOrEmpty(((IMinerData)y).MinerName);
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
                        int r = ((IMinerData)x).MinerName.CompareTo(((IMinerData)y).MinerName);
                        switch (_sortDirection) {
                            case SortDirection.Ascending:
                                return r;
                            case SortDirection.Descending:
                                return -r;
                            default:
                                return 0;
                        }
                    }
                case ClientDataSortField.MainCoinRejectPercent: {
                        int r = x.MainCoinRejectPercent.CompareTo(y.MainCoinRejectPercent);
                        switch (_sortDirection) {
                            case SortDirection.Ascending:
                                return r;
                            case SortDirection.Descending:
                                return -r;
                            default:
                                return 0;
                        }
                    }
                case ClientDataSortField.DualCoinRejectPercent: {
                        int r = x.DualCoinRejectPercent.CompareTo(y.DualCoinRejectPercent);
                        switch (_sortDirection) {
                            case SortDirection.Ascending:
                                return r;
                            case SortDirection.Descending:
                                return -r;
                            default:
                                return 0;
                        }
                    }
                case ClientDataSortField.MainCoinPoolDelay: {
                        int r = x.MainCoinPoolDelayNumber.CompareTo(y.MainCoinPoolDelayNumber);
                        switch (_sortDirection) {
                            case SortDirection.Ascending:
                                return r;
                            case SortDirection.Descending:
                                return -r;
                            default:
                                return 0;
                        }
                    }
                case ClientDataSortField.DualCoinPoolDelay: {
                        int r = x.DualCoinPoolDelayNumber.CompareTo(y.DualCoinPoolDelayNumber);
                        switch (_sortDirection) {
                            case SortDirection.Ascending:
                                return r;
                            case SortDirection.Descending:
                                return -r;
                            default:
                                return 0;
                        }
                    }
                case ClientDataSortField.CpuTemperature: {
                        int r = x.CpuTemperature.CompareTo(y.CpuTemperature);
                        switch (_sortDirection) {
                            case SortDirection.Ascending:
                                return r;
                            case SortDirection.Descending:
                                return -r;
                            default:
                                return 0;
                        }
                    }
                case ClientDataSortField.KernelSelfRestartCount: {
                        int r = x.KernelSelfRestartCount.CompareTo(y.KernelSelfRestartCount);
                        switch (_sortDirection) {
                            case SortDirection.Ascending:
                                return r;
                            case SortDirection.Descending:
                                return -r;
                            default:
                                return 0;
                        }
                    }
                case ClientDataSortField.MainCoinSpeed: {
                        int r = x.MainCoinSpeed.CompareTo(y.MainCoinSpeed);
                        switch (_sortDirection) {
                            case SortDirection.Ascending:
                                return r;
                            case SortDirection.Descending:
                                return -r;
                            default:
                                return 0;
                        }
                    }
                case ClientDataSortField.DiskSpace: {
                        int r = x.DiskSpaceMb.CompareTo(y.DiskSpaceMb);
                        switch (_sortDirection) {
                            case SortDirection.Ascending:
                                return r;
                            case SortDirection.Descending:
                                return -r;
                            default:
                                return 0;
                        }
                    }
                default:
                    return 0;
            }
        }
    }
}
