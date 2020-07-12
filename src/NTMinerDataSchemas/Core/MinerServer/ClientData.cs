using LiteDB;
using Newtonsoft.Json;
using NTMiner.Gpus;
using NTMiner.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NTMiner.Core.MinerServer {
    public class ClientData : SpeedDto, IClientData {
        private static readonly Dictionary<string, PropertyInfo> _reflectionUpdateProperties = new Dictionary<string, PropertyInfo>();

        public static bool TryGetReflectionUpdateProperty(string propertyName, out PropertyInfo propertyInfo) {
            return _reflectionUpdateProperties.TryGetValue(propertyName, out propertyInfo);
        }

        static ClientData() {
            Type type = typeof(ClientData);
            // 这算是一个安全措施，因为propertyName是来自客户端传入的，所以需要白名单。
            HashSet<string> propertyNames = new HashSet<string> {
                nameof(WorkerName),
                nameof(GroupId),
                nameof(WorkId),
                nameof(WindowsLoginName),
                nameof(WindowsPassword)
            };
            foreach (var propertyName in propertyNames) {
                _reflectionUpdateProperties.Add(propertyName, type.GetProperty(propertyName));
            }
        }

        public ClientData() : base() {
        }

        public static ClientData Create(IMinerData data) {
            return new ClientData() {
                #region
                Id = data.Id,
                MineContextId = Guid.Empty,
                CpuId = data.CpuId,
                DiskSpaceMb = 0,
                ClientId = data.ClientId,
                MACAddress = data.MACAddress,
                LocalIp = data.LocalIp,
                MinerIp = data.MinerIp,
                MinerName = data.MinerName,
                WorkerName = data.WorkerName,
                CreatedOn = data.CreatedOn,
                GroupId = data.GroupId,
                WorkId = data.WorkId,
                WindowsLoginName = data.WindowsLoginName,
                WindowsPassword = data.WindowsPassword,
                MineWorkId = Guid.Empty,
                MineWorkName = string.Empty,
                IsAutoBoot = false,
                IsAutoStart = false,
                AutoStartDelaySeconds = 15,
                IsAutoRestartKernel = false,
                AutoRestartKernelTimes = 10,
                IsNoShareRestartKernel = false,
                NoShareRestartKernelMinutes = 0,
                IsNoShareRestartComputer = false,
                NoShareRestartComputerMinutes = 0,
                IsPeriodicRestartKernel = false,
                PeriodicRestartKernelHours = 0,
                IsPeriodicRestartComputer = false,
                PeriodicRestartComputerHours = 0,
                PeriodicRestartKernelMinutes = 10,
                PeriodicRestartComputerMinutes = 10,
                IsAutoStartByCpu = false,
                IsAutoStopByCpu = false,
                CpuGETemperatureSeconds = 60,
                CpuLETemperatureSeconds = 60,
                CpuStartTemperature = 40,
                CpuStopTemperature = 65,
                CpuPerformance = 0,
                CpuTemperature = 0,
                IsRaiseHighCpuEvent = false,
                HighCpuPercent = 80,
                HighCpuSeconds = 10,
                GpuDriver = string.Empty,
                GpuType = GpuType.Empty,
                OSName = string.Empty,
                OSVirtualMemoryMb = 0,
                GpuInfo = string.Empty,
                Version = string.Empty,
                IsMining = false,
                BootOn = DateTime.MinValue,
                MineStartedOn = DateTime.MinValue,
                MinerActiveOn = DateTime.MinValue,
                MainCoinCode = string.Empty,
                MainCoinTotalShare = 0,
                MainCoinRejectShare = 0,
                MainCoinSpeed = 0,
                MainCoinPool = string.Empty,
                MainCoinWallet = string.Empty,
                Kernel = string.Empty,
                IsDualCoinEnabled = false,
                DualCoinPool = string.Empty,
                DualCoinWallet = string.Empty,
                DualCoinCode = string.Empty,
                DualCoinTotalShare = 0,
                DualCoinRejectShare = 0,
                DualCoinSpeed = 0,
                KernelCommandLine = string.Empty,
                MainCoinPoolDelay = string.Empty,
                DualCoinPoolDelay = string.Empty,
                DiskSpace = string.Empty,
                IsFoundOneGpuShare = false,
                IsRejectOneGpuShare = false,
                IsGotOneIncorrectGpuShare = false,
                KernelSelfRestartCount = 0,
                LoginName = data.LoginName,
                IsOuterUserEnabled = data.IsOuterUserEnabled,
                OuterUserId = data.OuterUserId,
                ReportOuterUserId = data.OuterUserId,
                TotalPhysicalMemoryMb = 0,
                LocalServerMessageTimestamp = Timestamp.UnixBaseTime,
                NetActiveOn = DateTime.MinValue,
                IsOnline = false,
                AESPassword = string.Empty,
                AESPasswordOn = DateTime.MinValue,
                IsAutoDisableWindowsFirewall = true,
                IsDisableAntiSpyware = true,
                IsDisableUAC = true,
                IsDisableWAU = true,
                MainCoinSpeedOn = DateTime.MinValue,
                DualCoinSpeedOn = DateTime.MinValue,
                GpuTable = new GpuSpeedData[0],
                DualCoinPoolDelayNumber = 0,
                MainCoinPoolDelayNumber = 0,
                MainCoinRejectPercent = 0,
                DualCoinRejectPercent = 0
                #endregion
            };
        }

        public static ClientData Clone(ClientData data) {
            return new ClientData() {
                #region
                Id = data.Id,
                CpuId = data.CpuId,
                DiskSpaceMb = data.DiskSpaceMb,
                MineContextId = data.MineContextId,
                MinerName = data.MinerName,
                MinerIp = data.MinerIp,
                CreatedOn = data.CreatedOn,
                MinerActiveOn = data.MinerActiveOn,
                GroupId = data.GroupId,
                WorkId = data.WorkId,
                WindowsLoginName = data.WindowsLoginName,
                WindowsPassword = data.WindowsPassword,
                MACAddress = data.MACAddress,
                LocalIp = data.LocalIp,
                ClientId = data.ClientId,
                IsAutoBoot = data.IsAutoBoot,
                IsAutoStart = data.IsAutoStart,
                AutoStartDelaySeconds = data.AutoStartDelaySeconds,
                IsAutoRestartKernel = data.IsAutoRestartKernel,
                AutoRestartKernelTimes = data.AutoRestartKernelTimes,
                IsNoShareRestartKernel = data.IsNoShareRestartKernel,
                NoShareRestartKernelMinutes = data.NoShareRestartKernelMinutes,
                IsNoShareRestartComputer = data.IsNoShareRestartComputer,
                NoShareRestartComputerMinutes = data.NoShareRestartComputerMinutes,
                IsPeriodicRestartKernel = data.IsPeriodicRestartKernel,
                PeriodicRestartKernelHours = data.PeriodicRestartKernelHours,
                IsPeriodicRestartComputer = data.IsPeriodicRestartComputer,
                PeriodicRestartComputerHours = data.PeriodicRestartComputerHours,
                PeriodicRestartComputerMinutes = data.PeriodicRestartComputerMinutes,
                PeriodicRestartKernelMinutes = data.PeriodicRestartKernelMinutes,
                IsAutoStopByCpu = data.IsAutoStopByCpu,
                IsAutoStartByCpu = data.IsAutoStartByCpu,
                CpuStopTemperature = data.CpuStopTemperature,
                CpuStartTemperature = data.CpuStartTemperature,
                CpuLETemperatureSeconds = data.CpuLETemperatureSeconds,
                CpuGETemperatureSeconds = data.CpuGETemperatureSeconds,
                CpuTemperature = data.CpuTemperature,
                CpuPerformance = data.CpuPerformance,
                IsRaiseHighCpuEvent = data.IsRaiseHighCpuEvent,
                HighCpuPercent = data.HighCpuPercent,
                HighCpuSeconds = data.HighCpuSeconds,
                GpuDriver = data.GpuDriver,
                GpuType = data.GpuType,
                OSName = data.OSName,
                OSVirtualMemoryMb = data.OSVirtualMemoryMb,
                TotalPhysicalMemoryMb = data.TotalPhysicalMemoryMb,
                GpuInfo = data.GpuInfo,
                Version = data.Version,
                IsMining = data.IsMining,
                BootOn = data.BootOn,
                MineStartedOn = data.MineStartedOn,
                MainCoinCode = data.MainCoinCode,
                MainCoinTotalShare = data.MainCoinTotalShare,
                MainCoinRejectShare = data.MainCoinRejectShare,
                MainCoinSpeed = data.MainCoinSpeed,
                MainCoinPool = data.MainCoinPool,
                MainCoinWallet = data.MainCoinWallet,
                Kernel = data.Kernel,
                IsDualCoinEnabled = data.IsDualCoinEnabled,
                DualCoinPool = data.DualCoinPool,
                DualCoinWallet = data.DualCoinWallet,
                DualCoinCode = data.DualCoinCode,
                DualCoinTotalShare = data.DualCoinTotalShare,
                DualCoinRejectShare = data.DualCoinRejectShare,
                DualCoinSpeed = data.DualCoinSpeed,
                KernelCommandLine = data.KernelCommandLine,
                GpuTable = data.GpuTable,
                MineWorkId = data.MineWorkId,
                MineWorkName = data.MineWorkName,
                WorkerName = data.WorkerName,
                DiskSpace = data.DiskSpace,
                MainCoinPoolDelay = data.MainCoinPoolDelay,
                DualCoinPoolDelay = data.DualCoinPoolDelay,
                IsFoundOneGpuShare = data.IsFoundOneGpuShare,
                IsRejectOneGpuShare = data.IsRejectOneGpuShare,
                IsGotOneIncorrectGpuShare = data.IsGotOneIncorrectGpuShare,
                KernelSelfRestartCount = data.KernelSelfRestartCount,
                LocalServerMessageTimestamp = data.LocalServerMessageTimestamp,
                LoginName = data.LoginName,
                IsOuterUserEnabled = data.IsOuterUserEnabled,
                OuterUserId = data.OuterUserId,
                ReportOuterUserId = data.ReportOuterUserId,
                NetActiveOn = data.NetActiveOn,
                IsOnline = data.IsOnline,
                IsDisableWAU = data.IsDisableWAU,
                IsDisableUAC = data.IsDisableUAC,
                AESPassword = data.AESPassword,
                AESPasswordOn = data.AESPasswordOn,
                IsDisableAntiSpyware = data.IsDisableAntiSpyware,
                IsAutoDisableWindowsFirewall = data.IsAutoDisableWindowsFirewall,
                MainCoinSpeedOn = data.MainCoinSpeedOn,
                DualCoinSpeedOn = data.DualCoinSpeedOn,
                DualCoinRejectPercent = data.DualCoinRejectPercent,
                MainCoinRejectPercent = data.MainCoinRejectPercent,
                MainCoinPoolDelayNumber = data.MainCoinPoolDelayNumber,
                DualCoinPoolDelayNumber = data.DualCoinPoolDelayNumber
                #endregion
            };
        }

        public static ClientData Create(ReportState state, string minerIp) {
            return new ClientData {
                Id = ObjectId.NewObjectId().ToString(),
                ClientId = state.ClientId,
                IsMining = state.IsMining,
                CreatedOn = DateTime.Now,
                MinerActiveOn = DateTime.Now,
                MinerIp = minerIp
            };
        }

        /// <summary>
        /// 从给定的speedData中提取出主币矿池延时，辅币矿池延时，主币拒绝率，辅币拒绝率。
        /// </summary>
        private static void Extract(
            ISpeedDto speedDto, 
            out int mainCoinPoolDelayNumber, 
            out int dualCoinPoolDelayNumber, 
            out double mainCoinRejectPercent, 
            out double dualCoinRejectPercent,
            out int diskSpaceMb) {
            #region
            mainCoinPoolDelayNumber = 0;
            dualCoinPoolDelayNumber = 0;
            mainCoinRejectPercent = 0.0;
            dualCoinRejectPercent = 0.0;
            if (!string.IsNullOrEmpty(speedDto.MainCoinPoolDelay)) {
                string text = speedDto.MainCoinPoolDelay.Trim();
                int count = 0;
                for (int i = 0; i < text.Length; i++) {
                    if (!char.IsNumber(text[i])) {
                        count = i;
                        break;
                    }
                }
                if (count != 0) {
                    mainCoinPoolDelayNumber = int.Parse(text.Substring(0, count));
                }
            }
            if (!string.IsNullOrEmpty(speedDto.DualCoinPoolDelay)) {
                string text = speedDto.DualCoinPoolDelay.Trim();
                int count = 0;
                for (int i = 0; i < text.Length; i++) {
                    if (!char.IsNumber(text[i])) {
                        count = i;
                        break;
                    }
                }
                if (count != 0) {
                    dualCoinPoolDelayNumber = int.Parse(text.Substring(0, count));
                }
            }
            if (speedDto.MainCoinTotalShare != 0) {
                mainCoinRejectPercent = (speedDto.MainCoinRejectShare * 100.0) / speedDto.MainCoinTotalShare;
            }
            if (speedDto.DualCoinTotalShare != 0) {
                dualCoinRejectPercent = (speedDto.DualCoinRejectShare * 100.0) / speedDto.DualCoinTotalShare;
            }
            diskSpaceMb = GetMinDiskSpaceMb(speedDto.DiskSpace);
            #endregion
        }

        private static int GetMinDiskSpaceMb(string diskSpace) {
            // C:\21.4 Gb;D:\9.2 Gb;E:\27.1 Gb
            if (string.IsNullOrEmpty(diskSpace)) {
                return 0;
            }
            string[] parts = diskSpace.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            List<int> list = new List<int>();
            foreach (string part in parts) {
                if (char.IsDigit(part[0])) {
                    if (double.TryParse(part.Substring(0, part.IndexOf(' ')), out double value)) {
                        list.Add((int)(value * 1024));
                    }
                }
            }
            if (list.Count == 0) {
                return 0;
            }
            return list.Min();
        }

        public static ClientData Create(ISpeedDto speedDto, string minerIp) {
            Extract(
                speedDto, 
                out int mainCoinPoolDelayNumber, 
                out int dualCoinPoolDelayNumber, 
                out double mainCoinRejectPercent, 
                out double dualCoinRejectPercent,
                out int diskSpaceMb);
            return new ClientData() {
                #region
                Id = ObjectId.NewObjectId().ToString(),
                MineContextId = speedDto.MineContextId,
                MinerName = speedDto.MinerName,
                CpuId = speedDto.CpuId,
                MinerIp = minerIp,
                CreatedOn = DateTime.Now,
                MinerActiveOn = DateTime.Now,
                GroupId = Guid.Empty,
                WorkId = Guid.Empty,// 这是服务端指定的作业，不受客户端的影响
                WindowsLoginName = string.Empty,
                WindowsPassword = string.Empty,
                MACAddress = speedDto.MACAddress,
                LocalIp = speedDto.LocalIp,
                ClientId = speedDto.ClientId,
                IsAutoBoot = speedDto.IsAutoBoot,
                IsAutoStart = speedDto.IsAutoStart,
                AutoStartDelaySeconds = speedDto.AutoStartDelaySeconds,
                IsAutoRestartKernel = speedDto.IsAutoRestartKernel,
                AutoRestartKernelTimes = speedDto.AutoRestartKernelTimes,
                IsNoShareRestartKernel = speedDto.IsNoShareRestartKernel,
                NoShareRestartKernelMinutes = speedDto.NoShareRestartKernelMinutes,
                IsNoShareRestartComputer = speedDto.IsNoShareRestartComputer,
                NoShareRestartComputerMinutes = speedDto.NoShareRestartComputerMinutes,
                IsPeriodicRestartKernel = speedDto.IsPeriodicRestartKernel,
                PeriodicRestartKernelHours = speedDto.PeriodicRestartKernelHours,
                IsPeriodicRestartComputer = speedDto.IsPeriodicRestartComputer,
                PeriodicRestartComputerHours = speedDto.PeriodicRestartComputerHours,
                PeriodicRestartComputerMinutes = speedDto.PeriodicRestartComputerMinutes,
                PeriodicRestartKernelMinutes = speedDto.PeriodicRestartKernelMinutes,
                IsAutoStopByCpu = speedDto.IsAutoStopByCpu,
                IsAutoStartByCpu = speedDto.IsAutoStartByCpu,
                CpuStopTemperature = speedDto.CpuStopTemperature,
                CpuStartTemperature = speedDto.CpuStartTemperature,
                CpuLETemperatureSeconds = speedDto.CpuLETemperatureSeconds,
                CpuGETemperatureSeconds = speedDto.CpuGETemperatureSeconds,
                CpuTemperature = speedDto.CpuTemperature,
                CpuPerformance = speedDto.CpuPerformance,
                IsRaiseHighCpuEvent = speedDto.IsRaiseHighCpuEvent,
                HighCpuPercent = speedDto.HighCpuPercent,
                HighCpuSeconds = speedDto.HighCpuSeconds,
                GpuDriver = speedDto.GpuDriver,
                GpuType = speedDto.GpuType,
                OSName = speedDto.OSName,
                OSVirtualMemoryMb = speedDto.OSVirtualMemoryMb,
                TotalPhysicalMemoryMb = speedDto.TotalPhysicalMemoryMb,
                GpuInfo = speedDto.GpuInfo,
                Version = speedDto.Version,
                IsMining = speedDto.IsMining,
                BootOn = speedDto.BootOn,
                MineStartedOn = speedDto.MineStartedOn,
                MainCoinCode = speedDto.MainCoinCode,
                MainCoinTotalShare = speedDto.MainCoinTotalShare,
                MainCoinRejectShare = speedDto.MainCoinRejectShare,
                MainCoinSpeed = speedDto.MainCoinSpeed,
                MainCoinPool = speedDto.MainCoinPool,
                MainCoinWallet = speedDto.MainCoinWallet,
                Kernel = speedDto.Kernel,
                IsDualCoinEnabled = speedDto.IsDualCoinEnabled,
                DualCoinPool = speedDto.DualCoinPool,
                DualCoinWallet = speedDto.DualCoinWallet,
                DualCoinCode = speedDto.DualCoinCode,
                DualCoinTotalShare = speedDto.DualCoinTotalShare,
                DualCoinRejectShare = speedDto.DualCoinRejectShare,
                DualCoinSpeed = speedDto.DualCoinSpeed,
                KernelCommandLine = speedDto.KernelCommandLine,
                MainCoinSpeedOn = speedDto.MainCoinSpeedOn,
                DualCoinSpeedOn = speedDto.DualCoinSpeedOn,
                GpuTable = speedDto.GpuTable,
                MineWorkId = speedDto.MineWorkId,
                MineWorkName = speedDto.MineWorkName,
                DiskSpace = speedDto.DiskSpace,
                MainCoinPoolDelay = speedDto.MainCoinPoolDelay,
                DualCoinPoolDelay = speedDto.DualCoinPoolDelay,
                IsFoundOneGpuShare = speedDto.IsFoundOneGpuShare,
                IsRejectOneGpuShare = speedDto.IsRejectOneGpuShare,
                IsGotOneIncorrectGpuShare = speedDto.IsGotOneIncorrectGpuShare,
                KernelSelfRestartCount = speedDto.KernelSelfRestartCount - 1,// 需要减1
                LocalServerMessageTimestamp = speedDto.LocalServerMessageTimestamp,
                AESPassword = string.Empty,
                AESPasswordOn = DateTime.MinValue,
                IsAutoDisableWindowsFirewall = speedDto.IsAutoDisableWindowsFirewall,
                IsDisableAntiSpyware = speedDto.IsDisableAntiSpyware,
                IsDisableUAC = speedDto.IsDisableUAC,
                IsDisableWAU = speedDto.IsDisableWAU,
                IsOnline = false,
                NetActiveOn = DateTime.MinValue,
                LoginName = string.Empty,
                IsOuterUserEnabled = speedDto.IsOuterUserEnabled,
                OuterUserId = string.Empty,
                ReportOuterUserId = speedDto.ReportOuterUserId,
                WorkerName = string.Empty,
                DualCoinPoolDelayNumber = dualCoinPoolDelayNumber,
                MainCoinPoolDelayNumber = mainCoinPoolDelayNumber,
                MainCoinRejectPercent = mainCoinRejectPercent,
                DualCoinRejectPercent = dualCoinRejectPercent,
                DiskSpaceMb = diskSpaceMb
                #endregion
            };
        }

        public void Update(MinerSign minerSign, out bool isChanged) {
            #region
            this.LoginName = minerSign.LoginName;
            isChanged = false;
            if (!isChanged) {
                isChanged = this.ClientId != minerSign.ClientId;
            }
            this.ClientId = minerSign.ClientId;
            if (!isChanged) {
                isChanged = this.OuterUserId != minerSign.OuterUserId;
            }
            this.OuterUserId = minerSign.OuterUserId;
            if (!isChanged) {
                isChanged = this.AESPassword != minerSign.AESPassword;
            }
            this.AESPassword = minerSign.AESPassword;
            if (!isChanged) {
                isChanged = this.AESPasswordOn != minerSign.AESPasswordOn;
            }
            this.AESPasswordOn = minerSign.AESPasswordOn;
            #endregion
        }

        public SpeedData ToSpeedData() {
            return new SpeedData {
                #region
                SpeedOn = this.MinerActiveOn,
                AutoRestartKernelTimes = this.AutoRestartKernelTimes,
                AutoStartDelaySeconds = this.AutoStartDelaySeconds,
                BootOn = this.BootOn,
                CpuId = this.CpuId,
                ClientId = this.ClientId,
                CpuGETemperatureSeconds = this.CpuGETemperatureSeconds,
                CpuLETemperatureSeconds = this.CpuLETemperatureSeconds,
                CpuPerformance = this.CpuPerformance,
                CpuStartTemperature = this.CpuStartTemperature,
                CpuStopTemperature = this.CpuStopTemperature,
                CpuTemperature = this.CpuTemperature,
                DiskSpace = this.DiskSpace,
                DualCoinCode = this.DualCoinCode,
                DualCoinPool = this.DualCoinPool,
                DualCoinPoolDelay = this.DualCoinPoolDelay,
                DualCoinRejectShare = this.DualCoinRejectShare,
                DualCoinSpeed = this.DualCoinSpeed,
                DualCoinSpeedOn = this.DualCoinSpeedOn,
                DualCoinTotalShare = this.DualCoinTotalShare,
                DualCoinWallet = this.DualCoinWallet,
                GpuDriver = this.GpuDriver,
                GpuInfo = this.GpuInfo,
                GpuTable = this.GpuTable,
                GpuType = this.GpuType,
                HighCpuPercent = this.HighCpuPercent,
                HighCpuSeconds = this.HighCpuSeconds,
                IsAutoBoot = this.IsAutoBoot,
                IsAutoDisableWindowsFirewall = this.IsAutoDisableWindowsFirewall,
                IsAutoRestartKernel = this.IsAutoRestartKernel,
                IsAutoStart = this.IsAutoStart,
                IsAutoStartByCpu = this.IsAutoStartByCpu,
                IsAutoStopByCpu = this.IsAutoStopByCpu,
                IsDisableAntiSpyware = this.IsDisableAntiSpyware,
                IsDisableUAC = this.IsDisableUAC,
                IsDisableWAU = this.IsDisableWAU,
                IsDualCoinEnabled = this.IsDualCoinEnabled,
                IsFoundOneGpuShare = this.IsFoundOneGpuShare,
                IsGotOneIncorrectGpuShare = this.IsGotOneIncorrectGpuShare,
                IsMining = this.IsMining,
                IsNoShareRestartComputer = this.IsNoShareRestartComputer,
                IsNoShareRestartKernel = this.IsNoShareRestartKernel,
                IsOuterUserEnabled = this.IsOuterUserEnabled,
                ReportOuterUserId = this.ReportOuterUserId,
                IsPeriodicRestartComputer = this.IsPeriodicRestartComputer,
                IsPeriodicRestartKernel = this.IsPeriodicRestartKernel,
                IsRaiseHighCpuEvent = this.IsRaiseHighCpuEvent,
                IsRejectOneGpuShare = this.IsRejectOneGpuShare,
                Kernel = this.Kernel,
                KernelCommandLine = this.KernelCommandLine,
                KernelSelfRestartCount = this.KernelSelfRestartCount,
                LocalIp = this.LocalIp,
                LocalServerMessageTimestamp = this.LocalServerMessageTimestamp,
                MACAddress = this.MACAddress,
                MainCoinCode = this.MainCoinCode,
                MainCoinPool = this.MainCoinPool,
                MainCoinPoolDelay = this.MainCoinPoolDelay,
                MainCoinRejectShare = this.MainCoinRejectShare,
                MainCoinSpeed = this.MainCoinSpeed,
                MainCoinSpeedOn = this.MainCoinSpeedOn,
                MainCoinTotalShare = this.MainCoinTotalShare,
                MainCoinWallet = this.MainCoinWallet,
                MineContextId = this.MineContextId,
                MinerIp = this.MinerIp,
                MinerName = this.MinerName,
                MineStartedOn = this.MineStartedOn,
                MineWorkId = this.MineWorkId,
                MineWorkName = this.MineWorkName,
                NoShareRestartComputerMinutes = this.NoShareRestartComputerMinutes,
                NoShareRestartKernelMinutes = this.NoShareRestartKernelMinutes,
                OSName = this.OSName,
                OSVirtualMemoryMb = this.OSVirtualMemoryMb,
                PeriodicRestartComputerHours = this.PeriodicRestartComputerHours,
                PeriodicRestartComputerMinutes = this.PeriodicRestartComputerMinutes,
                PeriodicRestartKernelHours = this.PeriodicRestartKernelHours,
                PeriodicRestartKernelMinutes = this.PeriodicRestartKernelMinutes,
                TotalPhysicalMemoryMb = this.TotalPhysicalMemoryMb,
                Version = this.Version
                #endregion
            };
        }

        /// <summary>
        /// 上报算力时。
        /// 因为只有MinerData具有的成员发生了变化时才需要持久化所以该非法输出isMinerDataChanged参数以表示MinerData的成员是否发生了变化。
        /// </summary>
        /// <param name="speedDto"></param>
        /// <param name="minerIp"></param>
        /// <param name="isMinerDataChanged"></param>
        public void Update(ISpeedDto speedDto, string minerIp, out bool isMinerDataChanged) {
            Update(speedDto, out isMinerDataChanged);
            if (!isMinerDataChanged && minerIp != this.MinerIp) {
                isMinerDataChanged = true;
            }
            this.MinerIp = minerIp;
        }

        /// <summary>
        /// 因为只有MinerData具有的成员发生了变化时才需要持久化所以该非法输出isMinerDataChanged参数以表示MinerData的成员是否发生了变化。
        /// </summary>
        /// <param name="state"></param>
        /// <param name="minerIp"></param>
        /// <param name="isMinerDataChanged"></param>
        public void Update(ReportState state, string minerIp, out bool isMinerDataChanged) {
            isMinerDataChanged = false;
            this.IsMining = state.IsMining;
            this.MinerActiveOn = DateTime.Now;
            if (!isMinerDataChanged && minerIp != this.MinerIp) {
                isMinerDataChanged = true;
            }
            this.MinerIp = minerIp;
        }

        private DateTime _preUpdateOn = DateTime.Now;
        private int _preMainCoinShare = 0;
        private int _preDualCoinShare = 0;
        private int _preMainCoinRejectShare = 0;
        private int _preDualCoinRejectShare = 0;
        private string _preMainCoin;
        private string _preDualCoin;
        /// <summary>
        /// 上报算力时和拉取算力时。
        /// 因为只有MinerData具有的成员发生了变化时才需要持久化所以该非法输出isMinerDataChanged参数以表示MinerData的成员是否发生了变化。
        /// </summary>
        /// <param name="speedDto"></param>
        /// <param name="isMinerDataChanged"></param>
        public void Update(ISpeedDto speedDto, out bool isMinerDataChanged) {
            #region
            isMinerDataChanged = false;
            if (speedDto == null) {
                return;
            }
            _preUpdateOn = DateTime.Now;
            if (_preMainCoin != this.MainCoinCode) {
                _preMainCoinShare = 0;
                _preMainCoinRejectShare = 0;
            }
            else {
                _preMainCoinShare = this.MainCoinTotalShare;
                _preMainCoinRejectShare = this.MainCoinRejectShare;
            }
            _preMainCoin = this.MainCoinCode;
            if (_preDualCoin != this.DualCoinCode) {
                _preDualCoinShare = 0;
                _preDualCoinRejectShare = 0;
            }
            else {
                _preDualCoinShare = this.DualCoinTotalShare;
                _preDualCoinRejectShare = this.DualCoinRejectShare;
            }
            _preDualCoin = this.DualCoinCode;
            #region MinerData
            if (!isMinerDataChanged) {
                isMinerDataChanged = this.ClientId != speedDto.ClientId;
            }
            this.ClientId = speedDto.ClientId;
            if (!isMinerDataChanged) {
                isMinerDataChanged = this.MACAddress != speedDto.MACAddress;
            }
            this.MACAddress = speedDto.MACAddress;
            if (!isMinerDataChanged) {
                isMinerDataChanged = this.LocalIp != speedDto.LocalIp;
            }
            this.LocalIp = speedDto.LocalIp;
            if (!isMinerDataChanged) {
                isMinerDataChanged = this.MinerName != speedDto.MinerName;
            }
            this.MinerName = speedDto.MinerName;
            if (!isMinerDataChanged) {
                isMinerDataChanged = this.IsOuterUserEnabled != speedDto.IsOuterUserEnabled;
            }
            this.IsOuterUserEnabled = speedDto.IsOuterUserEnabled;
            this.ReportOuterUserId = speedDto.ReportOuterUserId;
            #endregion
            this.MineContextId = speedDto.MineContextId;
            this.IsAutoBoot = speedDto.IsAutoBoot;
            this.IsAutoStart = speedDto.IsAutoStart;
            this.AutoStartDelaySeconds = speedDto.AutoStartDelaySeconds;
            this.IsAutoRestartKernel = speedDto.IsAutoRestartKernel;
            this.AutoRestartKernelTimes = speedDto.AutoRestartKernelTimes;
            this.IsNoShareRestartKernel = speedDto.IsNoShareRestartKernel;
            this.NoShareRestartKernelMinutes = speedDto.NoShareRestartKernelMinutes;
            this.IsNoShareRestartComputer = speedDto.IsNoShareRestartComputer;
            this.NoShareRestartComputerMinutes = speedDto.NoShareRestartComputerMinutes;
            this.IsPeriodicRestartKernel = speedDto.IsPeriodicRestartKernel;
            this.PeriodicRestartKernelHours = speedDto.PeriodicRestartKernelHours;
            this.IsPeriodicRestartComputer = speedDto.IsPeriodicRestartComputer;
            this.PeriodicRestartComputerHours = speedDto.PeriodicRestartComputerHours;
            this.PeriodicRestartComputerMinutes = speedDto.PeriodicRestartComputerMinutes;
            this.PeriodicRestartKernelMinutes = speedDto.PeriodicRestartKernelMinutes;
            this.IsAutoStopByCpu = speedDto.IsAutoStopByCpu;
            this.IsAutoStartByCpu = speedDto.IsAutoStartByCpu;
            this.CpuStopTemperature = speedDto.CpuStopTemperature;
            this.CpuStartTemperature = speedDto.CpuStartTemperature;
            this.CpuLETemperatureSeconds = speedDto.CpuLETemperatureSeconds;
            this.CpuGETemperatureSeconds = speedDto.CpuGETemperatureSeconds;
            this.CpuPerformance = speedDto.CpuPerformance;
            this.CpuTemperature = speedDto.CpuTemperature;
            this.IsRaiseHighCpuEvent = speedDto.IsRaiseHighCpuEvent;
            this.HighCpuPercent = speedDto.HighCpuPercent;
            this.HighCpuSeconds = speedDto.HighCpuSeconds;
            this.GpuDriver = speedDto.GpuDriver;
            this.GpuType = speedDto.GpuType;
            this.OSName = speedDto.OSName;
            this.OSVirtualMemoryMb = speedDto.OSVirtualMemoryMb;
            this.GpuInfo = speedDto.GpuInfo;
            this.CpuId = speedDto.CpuId;
            this.Version = speedDto.Version;
            this.IsMining = speedDto.IsMining;
            this.BootOn = speedDto.BootOn;
            this.MineStartedOn = speedDto.MineStartedOn;
            this.DiskSpace = speedDto.DiskSpace;
            this.MainCoinCode = speedDto.MainCoinCode;
            this.MainCoinTotalShare = speedDto.MainCoinTotalShare;
            this.MainCoinRejectShare = speedDto.MainCoinRejectShare;
            this.MainCoinSpeed = speedDto.MainCoinSpeed;
            this.MainCoinPool = speedDto.MainCoinPool;
            this.MainCoinWallet = speedDto.MainCoinWallet;
            this.Kernel = speedDto.Kernel;
            this.IsDualCoinEnabled = speedDto.IsDualCoinEnabled;
            this.DualCoinPool = speedDto.DualCoinPool;
            this.DualCoinWallet = speedDto.DualCoinWallet;
            this.DualCoinCode = speedDto.DualCoinCode;
            this.DualCoinTotalShare = speedDto.DualCoinTotalShare;
            this.DualCoinRejectShare = speedDto.DualCoinRejectShare;
            this.DualCoinSpeed = speedDto.DualCoinSpeed;
            this.KernelCommandLine = speedDto.KernelCommandLine;
            this.MainCoinSpeedOn = speedDto.MainCoinSpeedOn;
            this.DualCoinSpeedOn = speedDto.DualCoinSpeedOn;
            this.GpuTable = speedDto.GpuTable;
            this.MainCoinPoolDelay = speedDto.MainCoinPoolDelay;
            this.DualCoinPoolDelay = speedDto.DualCoinPoolDelay;
            this.IsFoundOneGpuShare = speedDto.IsFoundOneGpuShare;
            this.IsRejectOneGpuShare = speedDto.IsRejectOneGpuShare;
            this.IsGotOneIncorrectGpuShare = speedDto.IsGotOneIncorrectGpuShare;
            this.MineWorkId = speedDto.MineWorkId;
            this.MineWorkName = speedDto.MineWorkName;
            this.KernelSelfRestartCount = speedDto.KernelSelfRestartCount - 1;// 需要减1
            this.LocalServerMessageTimestamp = speedDto.LocalServerMessageTimestamp;
            this.TotalPhysicalMemoryMb = speedDto.TotalPhysicalMemoryMb;
            this.MinerActiveOn = DateTime.Now;// 现在时间
            this.IsAutoDisableWindowsFirewall = speedDto.IsAutoDisableWindowsFirewall;
            this.IsDisableAntiSpyware = speedDto.IsDisableAntiSpyware;
            this.IsDisableUAC = speedDto.IsDisableUAC;
            this.IsDisableWAU = speedDto.IsDisableWAU;
            Extract(
                speedDto, 
                out int mainCoinPoolDelayNumber, 
                out int dualCoinPoolDelayNumber, 
                out double mainCoinRejectPercent, 
                out double dualCoinRejectPercent,
                out int diskSpaceMb);
            this.MainCoinPoolDelayNumber = mainCoinPoolDelayNumber;
            this.DualCoinPoolDelayNumber = dualCoinPoolDelayNumber;
            this.MainCoinRejectPercent = mainCoinRejectPercent;
            this.DualCoinRejectPercent = dualCoinRejectPercent;
            this.DiskSpaceMb = diskSpaceMb;
            #endregion
        }

        #region GetMainCoinShareDelta
        public int GetMainCoinShareDelta(bool isPull) {
            if (_preMainCoinShare == 0) {
                return 0;
            }
            if (this.IsMining == false || string.IsNullOrEmpty(this.MainCoinCode)) {
                return 0;
            }
            if (isPull) {
                if (this._preUpdateOn.AddSeconds(5) > DateTime.Now) {
                    return 0;
                }
            }
            else if (this._preUpdateOn.AddSeconds(115) > DateTime.Now) {
                return 0;
            }

            int delta = this.MainCoinTotalShare - _preMainCoinShare;
            if (delta < 0) {
                delta = 0;
            }
            return delta;
        }
        #endregion

        #region GetDualCoinShareDelta
        public int GetDualCoinShareDelta(bool isPull) {
            if (_preDualCoinShare == 0) {
                return 0;
            }
            if (this.IsMining == false || string.IsNullOrEmpty(this.DualCoinCode)) {
                return 0;
            }
            if (isPull) {
                if (this._preUpdateOn.AddSeconds(5) > DateTime.Now) {
                    return 0;
                }
            }
            else if (this._preUpdateOn.AddSeconds(115) > DateTime.Now) {
                return 0;
            }

            int delta = this.DualCoinTotalShare - _preDualCoinShare;
            if (delta < 0) {
                delta = 0;
            }
            return delta;
        }
        #endregion

        #region GetMainCoinRejectShareDelta
        public int GetMainCoinRejectShareDelta(bool isPull) {
            if (_preMainCoinRejectShare == 0) {
                return 0;
            }
            if (this.IsMining == false || string.IsNullOrEmpty(this.MainCoinCode)) {
                return 0;
            }
            if (isPull && this._preUpdateOn.AddSeconds(5) > DateTime.Now) {
                return 0;
            }
            else if (this._preUpdateOn.AddSeconds(115) > DateTime.Now) {
                return 0;
            }

            int delta = this.MainCoinRejectShare - _preMainCoinRejectShare;
            if (delta < 0) {
                delta = 0;
            }
            return delta;
        }
        #endregion

        #region GetDualCoinRejectShareDelta
        public int GetDualCoinRejectShareDelta(bool isPull) {
            if (_preDualCoinRejectShare == 0) {
                return 0;
            }
            if (this.IsMining == false || string.IsNullOrEmpty(this.DualCoinCode)) {
                return 0;
            }
            if (isPull) {
                if (this._preUpdateOn.AddSeconds(5) > DateTime.Now) {
                    return 0;
                }
            }
            else if (this._preUpdateOn.AddSeconds(115) > DateTime.Now) {
                return 0;
            }

            int delta = this.DualCoinRejectShare - _preDualCoinRejectShare;
            if (delta < 0) {
                delta = 0;
            }
            return delta;
        }
        #endregion

        public string Id { get; set; }

        /// <summary>
        /// 服务的指定的作业
        /// </summary>
        public Guid WorkId { get; set; }

        public string WorkerName { get; set; }

        public string WindowsLoginName { get; set; }

        private string _windowsPassword;
        public string WindowsPassword {
            get {
                return _windowsPassword;
            }
            set {
                if (!Base64Util.IsBase64OrEmpty(value)) {
                    value = string.Empty;
                }
                _windowsPassword = value;
            }
        }

        public DateTime CreatedOn { get; set; }

        public DateTime MinerActiveOn { get; set; }

        public Guid GroupId { get; set; }

        public DateTime NetActiveOn { get; set; }

        public bool IsOnline { get; set; }

        public bool GetIsOnline(bool isOuterNet) {
            if (!IsOnline) {
                return false;
            }
            if (isOuterNet) {
                if (this.IsOuterUserEnabled) {
                    if (NetActiveOn.AddSeconds(60) < DateTime.Now) {
                        return false;
                    }
                }
                else if (NetActiveOn.AddSeconds(180) < DateTime.Now) {
                    return false;
                }
                return true;
            }
            if (NetActiveOn.AddSeconds(20) < DateTime.Now) {
                return false;
            }
            return true;
        }

        public string LoginName { get; set; }

        public string OuterUserId { get; set; }

        // 不会传到客户端
        [JsonIgnore]
        public string AESPassword { get; set; }

        public DateTime AESPasswordOn { get; set; }

        // 因为服务端需要根据如下几个字段排序所以会有这几个字段
        public int MainCoinPoolDelayNumber { get; set; }
        public int DualCoinPoolDelayNumber { get; set; }
        public double MainCoinRejectPercent { get; set; }
        public double DualCoinRejectPercent { get; set; }
        public int DiskSpaceMb { get; set; }
    }
}
