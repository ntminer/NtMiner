using System;
using System.Collections.Generic;

namespace NTMiner.Core.MinerStudio {
    public interface IColumnsShowSet {
        void AddOrUpdate(ColumnsShowData data);
        void Remove(Guid id);
        List<ColumnsShowData> GetAll();
    }
}
