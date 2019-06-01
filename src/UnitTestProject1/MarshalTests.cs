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

        [TestMethod]
        public void TestMethod1() {
            Console.WriteLine(Marshal.SizeOf(typeof(ADLODNPerformanceLevelsX2)));
            Console.WriteLine(Marshal.SizeOf(typeof(ADLODNPerformanceLevelX2)));
            Console.WriteLine(Marshal.SizeOf(typeof(ADLODNParameterRange)));
            Console.WriteLine(Marshal.SizeOf(typeof(ADLODNCapabilitiesX2)));
        }
    }
}
