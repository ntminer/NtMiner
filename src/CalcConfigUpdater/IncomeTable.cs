using System.Collections.Generic;

namespace CalcConfigUpdater {
    public class IncomeTable {
        public IncomeTable() {
            this.IncomeItems = new List<IncomeItem>();
        }

        public double UsdCny { get; set; }
        public List<IncomeItem> IncomeItems { get; set; }
    }
}
