using NTMiner.Core;
using NTMiner.Core.MinerServer;
using NTMiner.Gpus;
using NTMiner.User;
using System;
using System.Collections.Generic;

namespace NTMiner {
    public static class Enums {
        public static IEnumerable<EnumItem<LocalMessageChannel>> LocalMessageChannelEnumItems {
            get {
                return EnumItem<LocalMessageChannel>.GetEnumItems();
            }
        }

        public static IEnumerable<EnumItem<ConsoleColor>> ConsoleColorEnumItems {
            get {
                return EnumItem<ConsoleColor>.GetEnumItems();
            }
        }

        public static IEnumerable<EnumItem<SupportedGpu>> SupportedGpuEnumItems {
            get {
                return EnumItem<SupportedGpu>.GetEnumItems();
            }
        }

        public static IEnumerable<EnumItem<GpuType>> GpuTypeEnumItems {
            get {
                return EnumItem<GpuType>.GetEnumItems();
            }
        }

        public static IEnumerable<EnumItem<PublishStatus>> PublishStatusEnumItems {
            get {
                return EnumItem<PublishStatus>.GetEnumItems();
            }
        }

        public static IEnumerable<EnumItem<MineStatus>> MineStatusEnumItems {
            get {
                return EnumItem<MineStatus>.GetEnumItems();
            }
        }

        public static IEnumerable<EnumItem<UserStatus>> UserStatusEnumItems {
            get {
                return EnumItem<UserStatus>.GetEnumItems();
            }
        }

        public static IEnumerable<EnumItem<ServerMessageType>> ServerMessageTypeEnumItems {
            get {
                return EnumItem<ServerMessageType>.GetEnumItems();
            }
        }

        public static IEnumerable<EnumItem<LocalMessageType>> LocalMessageTypeEnumItems {
            get {
                return EnumItem<LocalMessageType>.GetEnumItems();
            }
        }
    }
}
