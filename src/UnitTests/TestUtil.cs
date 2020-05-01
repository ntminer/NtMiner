using System;
using System.IO;

namespace NTMiner {
    public static class TestUtil {
        public static readonly string DataDirFullName = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "data");
    }
}
