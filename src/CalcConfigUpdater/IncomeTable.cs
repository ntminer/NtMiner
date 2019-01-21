using System.Collections.Generic;

namespace NTMiner {
    public class IncomeTable {
        public IncomeTable() {
            this.IncomeItems = new List<IncomeItem>();
        }

        public double UsdCny { get; set; }
        public List<IncomeItem> IncomeItems { get; set; }
    }
}
