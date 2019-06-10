using NTMiner.MinerClient;
using System;
using System.Collections.Generic;

namespace NTMiner.Core {
    public interface IKernelOutputPickerSet : IEnumerable<IKernelOutputPicker> {
        bool Contains(Guid id);
        bool TryGetKernelOutputPicker(Guid id, out IKernelOutputPicker eventType);
    }
}
