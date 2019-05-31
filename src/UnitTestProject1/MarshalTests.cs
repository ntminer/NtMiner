using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1 {
    [TestClass]
    public class MarshalTests {
        [StructLayout(LayoutKind.Sequential)]
        struct ADLODNPerformanceLevelX2 {
            public int iClock;
            public int iVddc;
            public int iEnabled;
            public int iControl;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct ADLODNPerformanceLevelsX2 {
            public int iSize;
            public int iMode;
            public int iNumberOfPerformanceLevels;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 7)]
            public ADLODNPerformanceLevelX2[] aLevels;
        }

        [TestMethod]
        public void TestMethod1() {
            Console.WriteLine(Marshal.SizeOf(typeof(ADLODNPerformanceLevelsX2)));
            Console.WriteLine(Marshal.SizeOf(typeof(ADLODNPerformanceLevelX2)));
        }
    }
}
