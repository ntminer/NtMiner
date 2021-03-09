using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Runtime.InteropServices;

namespace NTMiner {
    [TestClass]
    public class MarshalTests {
        [TestMethod]
        public void IntPtrTest() {
            Assert.AreEqual(IntPtr.Zero, default);
        }

        [TestMethod]
        public void TestMethod1() {
            Console.WriteLine(Marshal.SizeOf(typeof(ADLODNPerformanceLevelsX2)));
            Console.WriteLine(Marshal.SizeOf(typeof(ADLODNPerformanceLevelX2)));
            Console.WriteLine(Marshal.SizeOf(typeof(ADLODNParameterRange)));
            Console.WriteLine(Marshal.SizeOf(typeof(ADLODNCapabilitiesX2)));
        }

        [TestMethod]
        public void Test() {
            Assert.AreEqual(sizeof(int), Marshal.SizeOf(typeof(int)));
        }

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
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public ADLODNPerformanceLevelX2[] aLevels;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct ADLODNParameterRange {
            public int iMode;
            public int iMin;
            public int iMax;
            public int iStep;
            public int iDefault;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct ADLODNCapabilitiesX2 {
            public int iMaximumNumberOfPerformanceLevels;
            public int iFlags;
            public ADLODNParameterRange sEngineClockRange;
            public ADLODNParameterRange sMemoryClockRange;
            public ADLODNParameterRange svddcRange;
            public ADLODNParameterRange power;
            public ADLODNParameterRange powerTuneTemperature;
            public ADLODNParameterRange fanTemperature;
            public ADLODNParameterRange fanSpeed;
            public ADLODNParameterRange minimumPerformanceClock;
            public ADLODNParameterRange throttleNotificaion;
            public ADLODNParameterRange autoSystemClock;
        }
    }
}
