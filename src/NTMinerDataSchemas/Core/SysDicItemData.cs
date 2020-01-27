using System;

namespace NTMiner.Core {
    public class SysDicItemData : ISysDicItem, IDbEntity<Guid> {
        public SysDicItemData() { }

        public Guid GetId() {
            return this.Id;
        }

        private DataLevel _dataLevel;
        public DataLevel GetDataLevel() {
            return _dataLevel;
        }

        public void SetDataLevel(DataLevel dataLevel) {
            this._dataLevel = dataLevel;
        }

        public Guid Id { get; set; }

        public Guid DicId { get; set; }

        public string Code { get; set; }

        public string Value { get; set; }

        public string Description { get; set; }

        public int SortNumber { get; set; }
    }
}
