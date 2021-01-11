using NTMiner.Gpus.Adl;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Gpus {
    public class AdlHelper : IGpuHelper {
        private static IntPtr _context = IntPtr.Zero;
        private static readonly ADLAdapterInfo[] _adapterInfoes = new ADLAdapterInfo[0];

        static AdlHelper() {
            try {
                int numberOfAdapters = 0;
                var adlStatus = AdlNativeMethods.ADLMainControlCreate(out _context);
                if (adlStatus >= AdlStatus.ADL_OK) {
                    adlStatus = AdlNativeMethods.ADL_Adapter_NumberOfAdapters_Get(ref numberOfAdapters);
                    if (adlStatus < AdlStatus.ADL_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL_Adapter_NumberOfAdapters_Get)} {adlStatus.ToString()}");
                    }
                    NTMinerConsole.DevDebug(() => $"{nameof(numberOfAdapters)}={numberOfAdapters.ToString()}");
                }
                if (numberOfAdapters > 0) {
                    _adapterInfoes = new ADLAdapterInfo[numberOfAdapters];
                    adlStatus = AdlNativeMethods.ADLAdapterAdapterInfoGet(_adapterInfoes);
                    if (adlStatus >= AdlStatus.ADL_OK && _adapterInfoes.Length != 0) {
                        _adapterInfoes = _adapterInfoes.Where(adapterInfo => !string.IsNullOrEmpty(adapterInfo.UDID) && adapterInfo.VendorID == AdlConst.ATI_VENDOR_ID).ToArray();
                    }
                    foreach (var adapterInfo in _adapterInfoes) {
                        NTMinerConsole.DevDebug(() => adapterInfo.ToString());
                    }
                }
            }
            catch (Exception ex) {
                Logger.ErrorDebugLine(ex);
            }
        }

        public static bool IsHasATIGpu {
            get {
                return _adapterInfoes.Length > 0;
            }
        }

        public AdlHelper() {
            Init();
        }

        private List<ATIGPU> _atiGpus = new List<ATIGPU>();
        private bool Init() {
            try {
                if (IsHasATIGpu) {
                    foreach (var adapterInfo in _adapterInfoes) {
                        bool gpuInited = _atiGpus.Any(gpu => gpu.BusNumber == adapterInfo.BusNumber && gpu.DeviceNumber == adapterInfo.DeviceNumber);
                        if (!gpuInited) {
                            int adapterIndex = adapterInfo.AdapterIndex;
                            int overdriveVersion = 0;
                            int maxLevels = 0;
                            int gpuLevel = 0;
                            int memoryLevel = 0;
                            try {
                                if (AdlNativeMethods.ADL_Overdrive_Caps(adapterIndex, out _, out _, out overdriveVersion) != AdlStatus.ADL_OK) {
                                    overdriveVersion = -1;
                                }
                                NTMinerConsole.DevDebug(() => "overdriveVersion=" + overdriveVersion.ToString());
                            }
                            catch (Exception ex) {
                                Logger.ErrorDebugLine(ex);
                            }

                            #region 5700之前的卡，比如580、480、vega等
                            ADLODNCapabilitiesX2 aDLODNCapabilitiesX2 = new ADLODNCapabilitiesX2();
                            try {
                                var r = AdlNativeMethods.ADL2_OverdriveN_CapabilitiesX2_Get(_context, adapterIndex, ref aDLODNCapabilitiesX2);
                                if (r < AdlStatus.ADL_OK) {
                                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_CapabilitiesX2_Get)} {r.ToString()}");
                                }
                                else {
                                    NTMinerConsole.DevDebug(() => "aDLODNCapabilitiesX2=" + aDLODNCapabilitiesX2.ToString());
                                }
                            }
                            catch (Exception ex) {
                                Logger.ErrorDebugLine(ex);
                            }
                            maxLevels = aDLODNCapabilitiesX2.iMaximumNumberOfPerformanceLevels;
                            ADLODNPerformanceLevelsX2 systemClockX2 = ADLODNPerformanceLevelsX2.Create();
                            systemClockX2.iNumberOfPerformanceLevels = maxLevels;
                            try {
                                var r = AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get(_context, adapterIndex, ref systemClockX2);
                                if (r < AdlStatus.ADL_OK) {
                                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get)} {r.ToString()}");
                                }
                                else {
                                    NTMinerConsole.DevDebug(() => "systemClockX2=" + VirtualRoot.JsonSerializer.Serialize(systemClockX2));
                                }
                            }
                            catch (Exception ex) {
                                Logger.ErrorDebugLine(ex);
                            }
                            for (int j = 0; j < systemClockX2.aLevels.Length; j++) {
                                if (systemClockX2.aLevels[j].iEnabled != 0) {
                                    gpuLevel = j + 1;
                                }
                            }
                            ADLODNPerformanceLevelsX2 memoryClockX2 = ADLODNPerformanceLevelsX2.Create();
                            memoryClockX2.iNumberOfPerformanceLevels = maxLevels;
                            try {
                                var r = AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get(_context, adapterIndex, ref memoryClockX2);
                                if (r < AdlStatus.ADL_OK) {
                                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get)} {r.ToString()}");
                                }
                                else {
                                    NTMinerConsole.DevDebug(() => "memoryClockX2=" + VirtualRoot.JsonSerializer.Serialize(memoryClockX2));
                                }
                            }
                            catch (Exception ex) {
                                Logger.ErrorDebugLine(ex);
                            }
                            for (int j = 0; j < memoryClockX2.aLevels.Length; j++) {
                                if (memoryClockX2.aLevels[j].iEnabled != 0) {
                                    memoryLevel = j + 1;
                                }
                            }
                            #endregion

                            int powerMin = aDLODNCapabilitiesX2.power.iMin + 100;// -50
                            int powerMax = aDLODNCapabilitiesX2.power.iMax + 100;// 50
                            int powerDefault = aDLODNCapabilitiesX2.power.iDefault + 100;// 0
                            int voltMin = aDLODNCapabilitiesX2.svddcRange.iMin;
                            int voltMax = aDLODNCapabilitiesX2.svddcRange.iMax;
                            int voltDefault = aDLODNCapabilitiesX2.svddcRange.iDefault;
                            int tempLimitMin = aDLODNCapabilitiesX2.powerTuneTemperature.iMin;
                            int tempLimitMax = aDLODNCapabilitiesX2.powerTuneTemperature.iMax;
                            int tempLimitDefault = aDLODNCapabilitiesX2.powerTuneTemperature.iDefault;
                            int coreClockMin = aDLODNCapabilitiesX2.sEngineClockRange.iMin * 10;
                            int coreClockMax = aDLODNCapabilitiesX2.sEngineClockRange.iMax * 10;
                            int coreClockDefault = aDLODNCapabilitiesX2.sEngineClockRange.iDefault * 10;
                            int memoryClockMin = aDLODNCapabilitiesX2.sMemoryClockRange.iMin * 10;
                            int memoryClockMax = aDLODNCapabilitiesX2.sMemoryClockRange.iMax * 10;
                            int memoryClockDefault = aDLODNCapabilitiesX2.sMemoryClockRange.iDefault * 10;

                            ADLOD8InitSetting aDLOD8InitSetting = ADLOD8InitSetting.Create();
                            if ((gpuLevel <= 0 || memoryLevel <= 0) && overdriveVersion == 8) {
                                #region 5700和之后的卡，比如5700、6800等
                                try {
                                    if (GetOD8InitSetting(adapterIndex, out aDLOD8InitSetting)) {
                                        gpuLevel = 3;
                                        memoryLevel = 0;
                                        maxLevels = 3;

                                        powerMin = 0;
                                        powerMax = 0;
                                        powerDefault = 0;
                                        voltMin = 0;
                                        voltMax = 0;
                                        voltDefault = 0;
                                        tempLimitMin = 0;
                                        tempLimitMax = 0;
                                        tempLimitDefault = 0;
                                        coreClockMin = 0;
                                        coreClockMax = 0;
                                        coreClockDefault = 0;
                                        memoryClockMin = 0;
                                        memoryClockMax = 0;
                                        memoryClockDefault = 0;
                                        if ((aDLOD8InitSetting.overdrive8Capabilities & (int)ADLOD8FeatureControl.ADL_OD8_GFXCLK_LIMITS) == (int)ADLOD8FeatureControl.ADL_OD8_GFXCLK_LIMITS ||
                                            (aDLOD8InitSetting.overdrive8Capabilities & (int)ADLOD8FeatureControl.ADL_OD8_GFXCLK_CURVE) == (int)ADLOD8FeatureControl.ADL_OD8_GFXCLK_CURVE) {
                                            powerMin = 100 + aDLOD8InitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_POWER_PERCENTAGE].minValue;// -50
                                            powerMax = 100 + aDLOD8InitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_POWER_PERCENTAGE].maxValue;// 20
                                            powerDefault = 100 + aDLOD8InitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_POWER_PERCENTAGE].defaultValue;// 0
                                            voltMin = aDLOD8InitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_VOLTAGE3].minValue;// 800
                                            voltMax = aDLOD8InitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_VOLTAGE3].maxValue;// 1200
                                            voltDefault = aDLOD8InitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_VOLTAGE3].defaultValue;// 1055
                                            tempLimitMin = aDLOD8InitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_TEMPERATURE_5].minValue;// 20
                                            tempLimitMax = aDLOD8InitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_TEMPERATURE_5].maxValue;// 100
                                            tempLimitDefault = aDLOD8InitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_TEMPERATURE_5].defaultValue;// 90
                                            coreClockMin = aDLOD8InitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_FMAX].minValue * 1000;// 800
                                            coreClockMax = aDLOD8InitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_FMAX].maxValue * 1000;// 1850
                                            coreClockDefault = aDLOD8InitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_FMAX].defaultValue * 1000;// 1750
                                            memoryClockMin = aDLOD8InitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_UCLK_FMAX].minValue * 1000;// 1750
                                            memoryClockMax = aDLOD8InitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_UCLK_FMAX].maxValue * 1000;// 1860
                                            memoryClockDefault = aDLOD8InitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_UCLK_FMAX].defaultValue * 1000;// 1750
                                        }
                                    }
                                }
                                catch (Exception ex) {
                                    Logger.ErrorDebugLine(ex);
                                }
                                #endregion
                            }
                            if (powerMax <= 0) {
                                powerMax = 100;
                            }
                            _atiGpus.Add(new ATIGPU {
                                AdapterName = adapterInfo.AdapterName.Trim(),
                                AdapterIndex = adapterIndex,
                                BusNumber = adapterInfo.BusNumber,
                                DeviceNumber = adapterInfo.DeviceNumber,
                                OverdriveVersion = overdriveVersion,
                                MaxLevels = maxLevels,
                                GpuLevels = gpuLevel,
                                MemoryLevels = memoryLevel,
                                CoreClockMin = coreClockMin,
                                CoreClockMax = coreClockMax,
                                CoreClockDefault = coreClockDefault,
                                MemoryClockMin = memoryClockMin,
                                MemoryClockMax = memoryClockMax,
                                MemoryClockDefault = memoryClockDefault,
                                PowerMin = powerMin,
                                PowerMax = powerMax,
                                PowerDefault = powerDefault,
                                TempLimitMin = tempLimitMin,
                                TempLimitMax = tempLimitMax,
                                TempLimitDefault = tempLimitDefault,
                                VoltMin = voltMin,
                                VoltMax = voltMax,
                                VoltDefault = voltDefault,
                                FanSpeedMax = 100,
                                FanSpeedMin = 0,
                                ADLOD8InitSetting = aDLOD8InitSetting
                            });
                        }
                    }
                }
                _atiGpus = _atiGpus.OrderBy(a => a.BusNumber).ToList();
                NTMinerConsole.DevDebug(() => string.Join(",", _atiGpus.Select(a => a.AdapterIndex)));
            }
            catch (Exception ex) {
                Logger.ErrorDebugLine(ex);
                return false;
            }

            return true;
        }

        public void Close() {
            try {
                AdlNativeMethods.ADL_Main_Control_Destroy?.Invoke();
            }
            catch {
            }
            try {
                if (_context != IntPtr.Zero) {
                    AdlNativeMethods.ADL2_Main_Control_Destroy?.Invoke(_context);
                    _context = IntPtr.Zero;
                }
            }
            catch {
            }
        }

        public Version GetDriverVersion() {
            try {
                ADLVersionsInfoX2 info = new ADLVersionsInfoX2();
                var r = AdlNativeMethods.ADL2_Graphics_VersionsX2_Get(_context, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_Graphics_VersionsX2_Get)} {r.ToString()}");
                }
                if (string.IsNullOrEmpty(info.strCrimsonVersion) || !Version.TryParse(info.strCrimsonVersion, out Version v)) {
                    return new Version();
                }
                return v;
            }
            catch {
                return new Version();
            }
        }

        public List<ATIGPU> ATIGpus {
            get {
                return _atiGpus;
            }
        }

        #region IGpuHelper成员
        #region OverClock
        public void OverClock(
            IGpu gpu, 
            int coreClockMHz, 
            int coreClockVoltage, 
            int memoryClockMHz, 
            int memoryClockVoltage, 
            int powerLimit, 
            int tempLimit, 
            int fanSpeed) {
            int gpuIndex = gpu.GetOverClockId();
            // A卡的超频不会为负
            if (coreClockMHz < 0) {
                coreClockMHz = 0;
            }
            if (coreClockVoltage < 0) {
                coreClockVoltage = 0;
            }
            // A卡的超频不会为负
            if (memoryClockMHz < 0) {
                memoryClockMHz = 0;
            }
            if (memoryClockVoltage < 0) {
                memoryClockVoltage = 0;
            }
            try {
                if (!TryGetAtiGpu(gpuIndex, out ATIGPU atiGpu)) {
                    return;
                }
                bool isSetCoreClock = coreClockMHz == 0 || coreClockMHz != gpu.CoreClockDelta || coreClockVoltage != gpu.CoreVoltage;
                bool isSetMemoryClock = memoryClockMHz == 0 || memoryClockMHz != gpu.MemoryClockDelta || memoryClockVoltage != gpu.MemoryVoltage;
                bool isSetPowerLimit = powerLimit == 0 || powerLimit != gpu.PowerCapacity;
                bool isSetTempLimit = tempLimit == 0 || tempLimit != gpu.TempLimit;
                if (atiGpu.OverdriveVersion < 8) {
                    if (isSetCoreClock) {
                        SetCoreClockOld(atiGpu, coreClockMHz, coreClockVoltage);
                    }
                    if (isSetMemoryClock) {
                        SetMemoryClockOld(atiGpu, memoryClockMHz, memoryClockVoltage);
                    }
                    if (isSetPowerLimit) {
                        SetPowerLimitOld(atiGpu, powerLimit);
                    }
                    if (isSetTempLimit) {
                        SetTempLimitOld(atiGpu, tempLimit);
                    }
                    // fanSpeed == -1表示开源自动温控
                    if (fanSpeed >= 0) {
                        SetFanSpeedOld(atiGpu, fanSpeed);
                    }
                }
                else if (GetOD8CurrentSetting(atiGpu.AdapterIndex, out ADLOD8CurrentSetting aDLOD8CurrentSetting)) {
                    ADLOD8SetSetting odSetSetting = ADLOD8SetSetting.Create();
                    if (isSetCoreClock) {
                        SetCoreClockNew(atiGpu, coreClockMHz, coreClockVoltage, ref odSetSetting);
                    }
                    if (isSetMemoryClock) {
                        SetMemoryClockNew(atiGpu, memoryClockMHz, memoryClockVoltage, ref odSetSetting);
                    }
                    if (isSetPowerLimit) {
                        SetPowerLimitNew(powerLimit, ref odSetSetting);
                    }
                    if (isSetTempLimit) {
                        SetTempLimitNew(atiGpu, tempLimit, ref odSetSetting);
                    }
                    // fanSpeed == -1表示开源自动温控
                    if (fanSpeed >= 0) {
                        SetFanSpeedNew(atiGpu, fanSpeed, ref odSetSetting);
                    }
                    SetOD8Range(aDLOD8CurrentSetting, atiGpu.AdapterIndex, odSetSetting);
                }
                return;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return;
            }
        }
        #endregion

        #region SetFanSpeed
        public void SetFanSpeed(IGpu gpu, int value) {
            int gpuIndex = gpu.GetOverClockId();
            try {
                if (!TryGetAtiGpu(gpuIndex, out ATIGPU atiGpu)) {
                    return;
                }
                if (atiGpu.OverdriveVersion < 8) {
                    SetFanSpeedOld(atiGpu, value);
                }
                else {
                    if (GetOD8CurrentSetting(atiGpu.AdapterIndex, out ADLOD8CurrentSetting aDLOD8CurrentSetting)) {
                        ADLOD8SetSetting odSetSetting = ADLOD8SetSetting.Create();
                        SetFanSpeedNew(atiGpu, value, ref odSetSetting);
                        SetOD8Range(aDLOD8CurrentSetting, atiGpu.AdapterIndex, odSetSetting);
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return;
            }
        }
        #endregion

        #region GetClockRange
        public OverClockRange GetClockRange(IGpu gpu) {
            int gpuIndex = gpu.GetOverClockId();
            OverClockRange result = new OverClockRange(gpuIndex);
            try {
                if (!TryGetAtiGpu(gpuIndex, out ATIGPU atiGpu)) {
                    return result;
                }
                int powerLimit = 0;
                int tempLimit = 0;
                int memoryClock = 0;
                int memoryiVddc = 0;
                int coreClock = 0;
                int coreiVddc = 0;
                if (atiGpu.OverdriveVersion < 8) {
                    GetTempLimitAndPowerLimitOld(atiGpu, out powerLimit, out tempLimit);
                    GetClockAndVoltOld(atiGpu, out memoryClock, out memoryiVddc, out coreClock, out coreiVddc);
                }
                else if (GetOD8CurrentSetting(atiGpu.AdapterIndex, out ADLOD8CurrentSetting aDLOD8CurrentSetting)) {
                    GetTempLimitAndPowerLimitNew(out powerLimit, out tempLimit, aDLOD8CurrentSetting);
                    GetClockAndVoltNew(out memoryClock, out memoryiVddc, out coreClock, out coreiVddc, aDLOD8CurrentSetting);
                }
                result.PowerCurr = powerLimit;
                result.TempCurr = tempLimit;
                result.MemoryClockDelta = memoryClock;
                result.MemoryVoltage = memoryiVddc;
                result.CoreClockDelta = coreClock;
                result.CoreVoltage = coreiVddc;
                result.CoreClockMin = atiGpu.CoreClockMin;
                result.CoreClockMax = atiGpu.CoreClockMax;
                result.MemoryClockMin = atiGpu.MemoryClockMin;
                result.MemoryClockMax = atiGpu.MemoryClockMax;
                result.PowerMin = atiGpu.PowerMin;
                result.PowerMax = atiGpu.PowerMax;
                result.PowerDefault = atiGpu.PowerDefault;
                result.TempLimitMin = atiGpu.TempLimitMin;
                result.TempLimitMax = atiGpu.TempLimitMax;
                result.TempLimitDefault = atiGpu.TempLimitDefault;
                result.VoltMin = atiGpu.VoltMin;
                result.VoltMax = atiGpu.VoltMax;
                result.VoltDefault = atiGpu.VoltDefault;
                result.FanSpeedMin = atiGpu.FanSpeedMin;
                result.FanSpeedMax = atiGpu.FanSpeedMax;
#if DEBUG
                NTMinerConsole.DevDebug(() => $"GetClockRange {result.ToString()}");
#endif
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
            return result;
        }
        #endregion

        private static void SetCoreClockNew(
            ATIGPU atiGpu, 
            int coreClockMHz, 
            int coreClockVoltage, 
            ref ADLOD8SetSetting odSetSetting) {
            try {
                bool isResetCoreClock = coreClockMHz == 0;
                bool isResetCoreClockVoltage = coreClockVoltage == 0;
                odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_FMAX].requested = 1;
                odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_FMAX].reset = isResetCoreClock ? 1 : 0;
                odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_FREQ3].requested = 1;
                odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_FREQ3].reset = isResetCoreClock ? 1 : 0;
                odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_VOLTAGE3].requested = 1;
                odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_VOLTAGE3].reset = isResetCoreClockVoltage ? 1 : 0;
                if (isResetCoreClock) {
                    odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_FMAX].value = atiGpu.ADLOD8InitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_FMAX].defaultValue;
                    odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_FREQ3].value = atiGpu.ADLOD8InitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_FREQ3].defaultValue;
                }
                else {
                    odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_FMAX].value = coreClockMHz;
                    odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_FREQ3].value = coreClockMHz;
                }
                if (isResetCoreClockVoltage) {                    
                    odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_VOLTAGE3].value = atiGpu.ADLOD8InitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_VOLTAGE3].defaultValue;
                }
                else {
                    odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_VOLTAGE3].value = coreClockVoltage;                    
                }
                return;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return;
            }
        }

        private static void SetCoreClockOld(ATIGPU atiGpu, int value, int voltage) {
            try {
                bool isReset = value == 0 && voltage == 0;
                ADLODNPerformanceLevelsX2 info = ADLODNPerformanceLevelsX2.Create();
                var r = AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get(_context, atiGpu.AdapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get)} {r.ToString()}");
                    return;
                }
                info.iMode = AdlConst.ODNControlType_Default;
                r = AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Set(_context, atiGpu.AdapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Set)} {r.ToString()}");
                    return;
                }
                if (isReset) {
                    return;
                }
                r = AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get(_context, atiGpu.AdapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get)} {r.ToString()}");
                    return;
                }
                info.iMode = AdlConst.ODNControlType_Manual;
                int index = 0;
                for (int i = 0; i < info.aLevels.Length; i++) {
                    if (info.aLevels[i].iEnabled != 0) {
                        index = i;
                    }
                }
                NTMinerConsole.DevDebug(() => $"{nameof(SetCoreClockOld)} PState {index.ToString()} value={value.ToString()} voltage={voltage.ToString()}");
                if (value != 0) {
                    info.aLevels[index].iClock = value * 100;
                }
                if (voltage != 0) {
                    info.aLevels[index].iVddc = voltage;
                }
                r = AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Set(_context, atiGpu.AdapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Set)} {r.ToString()}");
                    return;
                }
                return;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return;
            }
        }

        private static void SetMemoryClockNew(
            ATIGPU atiGpu, 
            int memoryClockMHz, 
            int memoryClockVoltage, 
            ref ADLOD8SetSetting odSetSetting) {
            try {
                bool isResetMemoryClock = memoryClockMHz == 0;
                bool isResetMemoryClockVoltage = memoryClockVoltage == 0;
                odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_UCLK_FMAX].requested = 1;
                odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_UCLK_FMAX].reset = isResetMemoryClock ? 1 : 0;
                odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_VOLTAGE1].requested = 1;
                odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_VOLTAGE1].reset = isResetMemoryClockVoltage ? 1 : 0;
                odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_VOLTAGE2].requested = 1;
                odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_VOLTAGE2].reset = isResetMemoryClockVoltage ? 1 : 0;
                if (isResetMemoryClock) {
                    odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_UCLK_FMAX].value = atiGpu.ADLOD8InitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_UCLK_FMAX].defaultValue;
                }
                else {
                    odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_UCLK_FMAX].value = memoryClockMHz;
                }
                if (isResetMemoryClockVoltage) {
                    odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_VOLTAGE1].value = atiGpu.ADLOD8InitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_VOLTAGE1].defaultValue;
                    odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_VOLTAGE2].value = atiGpu.ADLOD8InitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_VOLTAGE2].defaultValue;
                }
                else {
                    odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_VOLTAGE1].value = memoryClockVoltage;
                    odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_VOLTAGE2].value = memoryClockVoltage;
                }
                return;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return;
            }
        }

        private static void SetMemoryClockOld(ATIGPU atiGpu, int value, int voltage) {
            try {
                bool isReset = value == 0 && voltage == 0;
                ADLODNPerformanceLevelsX2 info = ADLODNPerformanceLevelsX2.Create();
                var r = AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get(_context, atiGpu.AdapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get)} {r.ToString()}");
                    return;
                }
                info.iMode = AdlConst.ODNControlType_Default;
                r = AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Set(_context, atiGpu.AdapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Set)} {r.ToString()}");
                    return;
                }
                if (isReset) {
                    return;
                }
                r = AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get(_context, atiGpu.AdapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get)} {r.ToString()}");
                    return;
                }
                info.iMode = AdlConst.ODNControlType_Manual;
                int index = 0;
                for (int i = 0; i < info.aLevels.Length; i++) {
                    if (info.aLevels[i].iEnabled != 0) {
                        index = i;
                    }
                }
                NTMinerConsole.DevDebug(() => $"{nameof(SetMemoryClockOld)} PState {index.ToString()} value={value.ToString()} voltage={voltage.ToString()}");
                if (value != 0) {
                    info.aLevels[index].iClock = value * 100;
                }
                if (voltage != 0) {
                    info.aLevels[index].iVddc = voltage;
                }
                r = AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Set(_context, atiGpu.AdapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Set)} {r.ToString()}");
                    return;
                }
                return;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return;
            }
        }

        private static void SetFanSpeedOld(ATIGPU atiGpu, int value) {
            bool isAutoMode = value == 0;
            try {
                AdlStatus r;
                if (isAutoMode) {
                    try {
                        r = AdlNativeMethods.ADL2_Overdrive5_FanSpeedToDefault_Set(_context, atiGpu.AdapterIndex, 0);
                        if (r != AdlStatus.ADL_OK) {
                            NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_Overdrive5_FanSpeedToDefault_Set)} {r.ToString()}");
                        }
                    }
                    catch (Exception e) {
                        r = AdlStatus.ADL_ERR;
                        Logger.ErrorDebugLine(e);
                    }
                    if (r != AdlStatus.ADL_OK) {
                        try {
                            r = AdlNativeMethods.ADL_Overdrive5_FanSpeedToDefault_Set(atiGpu.AdapterIndex, 0);
                            if (r != AdlStatus.ADL_OK) {
                                NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL_Overdrive5_FanSpeedToDefault_Set)} {r.ToString()}");
                            }
                        }
                        catch (Exception e) {
                            Logger.ErrorDebugLine(e);
                        }
                    }
                    if (r != AdlStatus.ADL_OK) {
                        try {
                            r = AdlNativeMethods.ADL2_Overdrive6_FanSpeed_Reset(_context, atiGpu.AdapterIndex);
                            if (r != AdlStatus.ADL_OK) {
                                NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_Overdrive6_FanSpeed_Reset)} {r.ToString()}");
                            }
                        }
                        catch (Exception e) {
                            Logger.ErrorDebugLine(e);
                        }
                    }
                    return;
                }
                ADLFanSpeedValue info = new ADLFanSpeedValue {
                    SpeedType = AdlConst.ADL_DL_FANCTRL_SPEED_TYPE_PERCENT,
                    Flags = AdlConst.ADL_DL_FANCTRL_FLAG_USER_DEFINED_SPEED
                };
                r = AdlNativeMethods.ADL_Overdrive5_FanSpeed_Get(atiGpu.AdapterIndex, 0, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL_Overdrive5_FanSpeed_Get)} {r.ToString()}");
                    return;
                }
                info.FanSpeed = value;
                r = AdlNativeMethods.ADL_Overdrive5_FanSpeed_Set(atiGpu.AdapterIndex, 0, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL_Overdrive5_FanSpeed_Set)} {r.ToString()}");
                    return;
                }
                return;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return;
            }
        }

        private static void SetFanSpeedNew(ATIGPU atiGpu, int value, ref ADLOD8SetSetting odSetSetting) {
            bool isAutoMode = value == 0;
            try {
                odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_SPEED_1].requested = 1;
                odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_SPEED_2].requested = 1;
                odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_SPEED_3].requested = 1;
                odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_SPEED_4].requested = 1;
                odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_SPEED_5].requested = 1;
                int iReset = isAutoMode ? 1 : 0;
                odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_SPEED_1].reset = iReset;
                odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_SPEED_2].reset = iReset;
                odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_SPEED_3].reset = iReset;
                odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_SPEED_4].reset = iReset;
                odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_SPEED_5].reset = iReset;
                if (isAutoMode) {
                    odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_SPEED_1].value = atiGpu.ADLOD8InitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_SPEED_1].defaultValue;
                    odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_SPEED_2].value = atiGpu.ADLOD8InitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_SPEED_2].defaultValue;
                    odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_SPEED_3].value = atiGpu.ADLOD8InitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_SPEED_3].defaultValue;
                    odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_SPEED_4].value = atiGpu.ADLOD8InitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_SPEED_4].defaultValue;
                    odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_SPEED_5].value = atiGpu.ADLOD8InitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_SPEED_5].defaultValue;
                }
                else {
                    odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_SPEED_1].value = value;
                    odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_SPEED_2].value = value;
                    odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_SPEED_3].value = value;
                    odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_SPEED_4].value = value;
                    odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_SPEED_5].value = value;
                }
                return;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return;
            }
        }

        private static void SetPowerLimitOld(ATIGPU atiGpu, int value) {
            try {
                ADLODNPowerLimitSetting info = new ADLODNPowerLimitSetting();
                var r = AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get(_context, atiGpu.AdapterIndex, out info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get)} {r.ToString()}");
                    return;
                }
                info.iMode = AdlConst.ODNControlType_Manual;
                info.iTDPLimit = value - 100;
                r = AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Set(_context, atiGpu.AdapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Set)} {r.ToString()}");
                    return;
                }
                return;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return;
            }
        }

        private static void SetPowerLimitNew(int value, ref ADLOD8SetSetting odSetSetting) {
            try {
                if (value == 0) {
                    value = 100;
                }
                odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_POWER_PERCENTAGE].requested = 1;
                odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_POWER_PERCENTAGE].reset = value == 100 ? 1 : 0;
                odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_POWER_PERCENTAGE].value = value - 100;
                return;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return;
            }
        }

        private static void SetTempLimitOld(ATIGPU atiGpu, int value) {
            try {
                bool isAutoModel = value == 0;
                ADLODNPowerLimitSetting info = new ADLODNPowerLimitSetting();
                var r = AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get(_context, atiGpu.AdapterIndex, out info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get)} {r.ToString()}");
                    return;
                }
                info.iMode = isAutoModel ? AdlConst.ODNControlType_Auto : AdlConst.ODNControlType_Manual;
                info.iMaxOperatingTemperature = isAutoModel ? 0 : value;
                r = AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Set(_context, atiGpu.AdapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Set)} {r.ToString()}");
                    return;
                }
                return;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return;
            }
        }

        private static void SetTempLimitNew(ATIGPU atiGpu, int value, ref ADLOD8SetSetting odSetSetting) {
            try {
                bool isAutoModel = value == 0;
                odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_OPERATING_TEMP_MAX].requested = 1;
                bool isReset = value == 0;
                odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_OPERATING_TEMP_MAX].reset = isReset ? 1 : 0;
                if (isReset) {
                    odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_OPERATING_TEMP_MAX].value = atiGpu.ADLOD8InitSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_OPERATING_TEMP_MAX].defaultValue;
                }
                else {
                    odSetSetting.od8SettingTable[(int)ADLOD8SettingId.OD8_OPERATING_TEMP_MAX].value = value;
                }
                return;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return;
            }
        }
        #endregion

        public void GetPowerFanTemp(int gpuIndex, out uint power, out uint fanSpeed, out int temp) {
            power = 0;
            fanSpeed = 0;
            temp = 0;
            if (!TryGetAtiGpu(gpuIndex, out ATIGPU atiGpu)) {
                return;
            }
            try {
                if (atiGpu.OverdriveVersion < 8) {
                    temp = GetTemperatureOld(atiGpu);
                    power = GetPowerUsageOld(atiGpu);
                    fanSpeed = GetFanSpeedOld(atiGpu.AdapterIndex);
                }
                else {
                    ADLPMLogDataOutput logDataOutput = ADLPMLogDataOutput.Create();
                    var r = AdlNativeMethods.ADL2_New_QueryPMLogData_Get(_context, atiGpu.AdapterIndex, ref logDataOutput);
                    if (r < AdlStatus.ADL_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_New_QueryPMLogData_Get)} {r.ToString()}");
                    }
                    int i = (int)ADLSensorType.PMLOG_ASIC_POWER;
                    if (i < logDataOutput.Sensors.Length && logDataOutput.Sensors[i].Supported != 0) {
                        power = (uint)logDataOutput.Sensors[i].Value;
                    }
                    i = (int)ADLSensorType.PMLOG_FAN_PERCENTAGE;
                    if (i < logDataOutput.Sensors.Length && logDataOutput.Sensors[i].Supported != 0) {
                        fanSpeed = (uint)logDataOutput.Sensors[i].Value;
                    }
                    i = (int)ADLSensorType.PMLOG_TEMPERATURE_EDGE;
                    if (i < logDataOutput.Sensors.Length && logDataOutput.Sensors[i].Supported != 0) {
                        temp = logDataOutput.Sensors[i].Value;
                    }
                }
            }
            catch {
            }
        }

        public ulong GetTotalMemory(int gpuIndex) {
            try {
                if (!TryGpuAdapterIndex(gpuIndex, out int adapterIndex)) {
                    return 0;
                }
                ADLMemoryInfo info = new ADLMemoryInfo();
                var r = AdlNativeMethods.ADL_Adapter_MemoryInfo_Get(adapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL_Adapter_MemoryInfo_Get)} {r.ToString()}");
                    return 0;
                }
                return info.MemorySize;
            }
            catch {
                return 0;
            }
        }

        #region private methods
        // 将GPUIndex转换为AdapterIndex
        private bool TryGpuAdapterIndex(int gpuIndex, out int adapterIndex) {
            adapterIndex = 0;
            if (gpuIndex < 0 || gpuIndex >= _atiGpus.Count) {
                return false;
            }
            adapterIndex = _atiGpus[gpuIndex].AdapterIndex;
            return true;
        }

        private bool TryGetAtiGpu(int gpuIndex, out ATIGPU atiGpu) {
            atiGpu = ATIGPU.Empty;
            if (gpuIndex < 0 || gpuIndex >= _atiGpus.Count) {
                return false;
            }
            atiGpu = _atiGpus[gpuIndex];
            return true;
        }
        #endregion

        #region private static methods
        private static uint GetFanSpeedOld(int adapterIndex) {
            try {
                ADLFanSpeedValue info = new ADLFanSpeedValue {
                    SpeedType = AdlConst.ADL_DL_FANCTRL_SPEED_TYPE_PERCENT
                };
                var r = AdlNativeMethods.ADL_Overdrive5_FanSpeed_Get(adapterIndex, 0, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL_Overdrive5_FanSpeed_Get)} {r.ToString()}");
                    return 0;
                }
                return (uint)info.FanSpeed;
            }
            catch {
                return 0;
            }
        }

        private static int GetTemperatureOld(ATIGPU atiGpu) {
            try {
                if (atiGpu.OverdriveVersion >= 7) {
                    var r = AdlNativeMethods.ADL2_OverdriveN_Temperature_Get(_context, atiGpu.AdapterIndex, ADLODNTemperatureType.CORE, out int temperature);
                    if (r < AdlStatus.ADL_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_Temperature_Get)} {r.ToString()}");
                        return 0;
                    }
                    return (int)(0.001f * temperature);
                }
                else {
                    ADLTemperature info = new ADLTemperature();
                    var r = AdlNativeMethods.ADL_Overdrive5_Temperature_Get(atiGpu.AdapterIndex, 0, ref info);
                    if (r < AdlStatus.ADL_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL_Overdrive5_Temperature_Get)} {r.ToString()}");
                        return 0;
                    }
                    return (int)(0.001f * info.Temperature);
                }
            }
            catch {
            }
            return 0;
        }

        private static uint GetPowerUsageOld(ATIGPU atiGpu) {
            try {
                int power = 0;
                if (atiGpu.OverdriveVersion >= 6) {
                    var r = AdlNativeMethods.ADL2_Overdrive6_CurrentPower_Get(_context, atiGpu.AdapterIndex, ADLODNCurrentPowerType.TOTAL_POWER, out power);
                    if (r < AdlStatus.ADL_OK) {
                        NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_Overdrive6_CurrentPower_Get)} {r.ToString()}");
                        return 0;
                    }
                    return (uint)(power / 256.0);
                }
            }
            catch {
            }
            return 0;
        }

        private static bool GetOD8InitSetting(int adapterIndex, out ADLOD8InitSetting aDLOD8InitSetting) {
            aDLOD8InitSetting = ADLOD8InitSetting.Create();
            try {
                var r = AdlNativeMethods.ADL2_Overdrive8_Init_Setting_Get(_context, adapterIndex, ref aDLOD8InitSetting);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_Overdrive8_Init_Setting_Get)} {r.ToString()}");
                }
#if DEBUG
                Logger.Debug($"{nameof(aDLOD8InitSetting)}={aDLOD8InitSetting.ToString()}");
#endif
                return r == AdlStatus.ADL_OK;
            }
            catch (Exception ex) {
                Logger.ErrorDebugLine(ex);
                return false;
            }
        }

        private static bool GetOD8CurrentSetting(int adapterIndex, out ADLOD8CurrentSetting aDLOD8CurrentSetting) {
            aDLOD8CurrentSetting = ADLOD8CurrentSetting.Create();
            try {
                var r = AdlNativeMethods.ADL2_Overdrive8_Current_Setting_Get(_context, adapterIndex, ref aDLOD8CurrentSetting);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_Overdrive8_Current_Setting_Get)} {r.ToString()}");
                }
#if DEBUG
                Logger.Debug($"{nameof(adapterIndex)}={adapterIndex.ToString()}, {nameof(aDLOD8CurrentSetting)}={aDLOD8CurrentSetting.ToString()}");
#endif
                return r == AdlStatus.ADL_OK;
            }
            catch (Exception ex) {
                Logger.ErrorDebugLine(ex);
                return false;
            }
        }

        private static void GetTempLimitAndPowerLimitOld(ATIGPU atiGpu, out int powerLimit, out int tempLimit) {
            powerLimit = 0;
            tempLimit = 0;
            ADLODNPowerLimitSetting info = new ADLODNPowerLimitSetting();
            try {
                var r = AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get(_context, atiGpu.AdapterIndex, out info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_PowerLimit_Get)} {r.ToString()}");
                }
                else {
                    powerLimit = 100 + info.iTDPLimit;
                    tempLimit = info.iMaxOperatingTemperature;
                }
            }
            catch {
            }
        }

        private static void GetTempLimitAndPowerLimitNew(
            out int powerLimit, 
            out int tempLimit, 
            ADLOD8CurrentSetting aDLOD8CurrentSetting) {
            powerLimit = 0;
            tempLimit = 0;
            try {
                powerLimit = 100 + aDLOD8CurrentSetting.Od8SettingTable[(int)ADLOD8SettingId.OD8_POWER_PERCENTAGE];
                tempLimit = aDLOD8CurrentSetting.Od8SettingTable[(int)ADLOD8SettingId.OD8_FAN_CURVE_TEMPERATURE_5];
            }
            catch (Exception ex) {
                Logger.ErrorDebugLine(ex);
            }
        }

        private static void GetClockAndVoltOld(
            ATIGPU atiGpu, 
            out int memoryClock, 
            out int memoryiVddc, 
            out int coreClock, 
            out int coreiVddc) {
            memoryClock = 0;
            memoryiVddc = 0;
            coreClock = 0;
            coreiVddc = 0;
            try {
                ADLODNPerformanceLevelsX2 info = ADLODNPerformanceLevelsX2.Create();
                var r = AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get(_context, atiGpu.AdapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_SystemClocksX2_Get)} {r.ToString()}");
                }
                var index = atiGpu.GpuLevels - 1;
                if (index >= 0 && index < info.aLevels.Length) {
                    coreClock = info.aLevels[index].iClock * 10;
                    coreiVddc = info.aLevels[index].iVddc;
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
            try {
                ADLODNPerformanceLevelsX2 info = ADLODNPerformanceLevelsX2.Create();
                var r = AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get(_context, atiGpu.AdapterIndex, ref info);
                if (r < AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_OverdriveN_MemoryClocksX2_Get)} {r.ToString()}");
                }
                int index = atiGpu.MemoryLevels - 1;
                if (index >= 0 && index < info.aLevels.Length) {
                    memoryClock = info.aLevels[index].iClock * 10;
                    memoryiVddc = info.aLevels[index].iVddc;
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        private static void GetClockAndVoltNew(
            out int memoryClock, 
            out int memoryiVddc, 
            out int coreClock, 
            out int coreiVddc, 
            ADLOD8CurrentSetting aDLOD8CurrentSetting) {
            memoryClock = 0;
            memoryiVddc = 0;
            coreClock = 0;
            coreiVddc = 0;
            try {
                coreClock = aDLOD8CurrentSetting.Od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_FMAX] * 1000;
                coreiVddc = aDLOD8CurrentSetting.Od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_VOLTAGE3];
                memoryClock = aDLOD8CurrentSetting.Od8SettingTable[(int)ADLOD8SettingId.OD8_UCLK_FMAX] * 1000;
                memoryiVddc = aDLOD8CurrentSetting.Od8SettingTable[(int)ADLOD8SettingId.OD8_GFXCLK_VOLTAGE2];
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        private static bool SetOD8Range(
            ADLOD8CurrentSetting aDLOD8CurrentSetting, 
            int adapterIndex, 
            ADLOD8SetSetting odSetSetting) {
            try {
                var r = AdlNativeMethods.ADL2_Overdrive8_Setting_Set(_context, adapterIndex, ref odSetSetting, ref aDLOD8CurrentSetting);
                if (r != AdlStatus.ADL_OK) {
                    NTMinerConsole.DevError(() => $"{nameof(AdlNativeMethods.ADL2_Overdrive8_Setting_Set)} {r.ToString()}");
                }
                return r == AdlStatus.ADL_OK;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
                return false;
            }
        }
        #endregion
    }
}
