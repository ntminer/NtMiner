using NTMiner.Profile;
using System;

namespace NTMiner.Core.Profiles {
    public interface IGpuProfileSet {
        IGpuProfile GetGpuProfile(Guid coinId, int index);
        bool IsOverClockEnabled(Guid coinId);
        bool IsOverClockGpuAll(Guid coinId);
        void Refresh();
        void SetIsOverClockEnabled(Guid coinId, bool value);
        void SetIsOverClockGpuAll(Guid coinId, bool value);
    }
}