using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Data {
    public interface IColumnsShowSet {
        bool Contains(Guid id);
        void AddOrUpdate(ColumnsShowData data);
        void Remove(Guid id);
        ColumnsShowData GetColumnsShow(Guid id);
        List<ColumnsShowData> GetAll();
    }
}
