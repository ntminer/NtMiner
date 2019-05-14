using NTMiner.Core;
using System.Collections.Generic;

namespace NTMiner {
    public static class EnumSet {
        public static readonly IEnumerable<EnumItem<SupportedGpu>> SupportedGpuEnumItems = SupportedGpu.AMD.GetEnumItems();

        public static readonly IEnumerable<EnumItem<GpuType>> GpuTypeEnumItems = GpuType.AMD.GetEnumItems();

        public static readonly IEnumerable<EnumItem<PublishStatus>> PublishStatusEnumItems = PublishStatus.Published.GetEnumItems();

        public static readonly IEnumerable<EnumItem<MineStatus>> MineStatusEnumItems = MineStatus.All.GetEnumItems();
    }
}
