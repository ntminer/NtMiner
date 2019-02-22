using System;
using System.Reflection;

namespace NTMiner.Hashrate {
    public class GpuRowData {
        public static readonly PropertyInfo[] GpuProperties = new PropertyInfo[12];

        static GpuRowData() {
            Type t = typeof(GpuRowData);
            for (int i = 0; i < 12; i++) {
                GpuProperties[i] = t.GetProperty("Gpu" + i);
            }
        }

        public string RowHeader { get; set; }
        public string Gpu0 { get; set; }
        public string Gpu1 { get; set; }
        public string Gpu2 { get; set; }
        public string Gpu3 { get; set; }
        public string Gpu4 { get; set; }
        public string Gpu5 { get; set; }
        public string Gpu6 { get; set; }
        public string Gpu7 { get; set; }
        public string Gpu8 { get; set; }
        public string Gpu9 { get; set; }
        public string Gpu10 { get; set; }
        public string Gpu11 { get; set; }
    }
}
