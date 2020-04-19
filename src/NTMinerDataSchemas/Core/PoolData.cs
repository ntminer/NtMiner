using System;
using System.Text;

namespace NTMiner.Core {
    public class PoolData : IPool, IDbEntity<Guid>, ISignableData {
        public PoolData() {
        }

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

        public Guid BrandId { get; set; }

        public Guid CoinId { get; set; }

        public string Name { get; set; }

        public string Server { get; set; }

        public string Url { get; set; }

        public string Website { get; set; }

        public int SortNumber { get; set; }

        public bool IsUserMode { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Notice { get; set; }

        public string TutorialUrl { get; set; }

        public bool NoPool1 { get; set; }

        public bool NotPool1 { get; set; }

        public string MinerNamePrefix { get; set; }

        public string MinerNamePostfix { get; set; }

        public StringBuilder GetSignData() {
            return this.BuildSign();
        }
    }
}
