using NTMiner.Core.MinerClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;

namespace NTMiner.Net {
    public class LocalIpSet : ILocalIpSet {
        public static IEnumerable<ManagementObject> GetNetCardInfo() {
            using (ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
            using (ManagementObjectCollection moc = mc.GetInstances()) {
                foreach (ManagementObject mo in moc) {
                    if (!(bool)mo["IPEnabled"] || mo["DefaultIPGateway"] == null) {
                        continue;
                    }
                    string[] defaultIpGateways = (string[])mo["DefaultIPGateway"];
                    if (defaultIpGateways.Length == 0) {
                        continue;
                    }
                    yield return mo;
                }
            }
        }

        #region private static method GetLocalIps
        private static LocalIpData[] GetLocalIps() {
            List<LocalIpData> list = new List<LocalIpData>();
            try {
                foreach (ManagementObject mo in GetNetCardInfo()) {
                    if (!(bool)mo["IPEnabled"] || mo["DefaultIPGateway"] == null) {
                        continue;
                    }
                    string[] defaultIpGateways = (string[])mo["DefaultIPGateway"];
                    if (defaultIpGateways.Length == 0) {
                        continue;
                    }
                    string dNSServer0 = string.Empty;
                    string dNSServer1 = string.Empty;
                    if (mo["DNSServerSearchOrder"] != null) {
                        string[] dNSServerSearchOrder = (string[])mo["DNSServerSearchOrder"];
                        if (dNSServerSearchOrder.Length > 0) {
                            if (dNSServerSearchOrder[0] != defaultIpGateways[0]) {
                                dNSServer0 = dNSServerSearchOrder[0];
                            }
                        }
                        if (dNSServerSearchOrder.Length > 1) {
                            dNSServer1 = dNSServerSearchOrder[1];
                        }
                    }
                    string ipAddress = string.Empty;
                    if (mo["IPAddress"] != null) {
                        string[] items = (string[])mo["IPAddress"];
                        if (items.Length != 0) {
                            ipAddress = items[0];// 只取Ipv4
                        }
                    }
                    string ipSubnet = string.Empty;
                    if (mo["IPSubnet"] != null) {
                        string[] items = (string[])mo["IPSubnet"];
                        if (items.Length != 0) {
                            ipSubnet = items[0];// 只取Ipv4
                        }
                    }
                    list.Add(new LocalIpData {
                        DefaultIPGateway = defaultIpGateways[0],
                        DHCPEnabled = (bool)mo["DHCPEnabled"],
                        SettingID = (string)mo["SettingID"],
                        IPSubnet = ipSubnet,
                        DNSServer0 = dNSServer0,
                        DNSServer1 = dNSServer1,
                        IPAddress = ipAddress,
                        MACAddress = (string)mo["MACAddress"]
                    });
                    FillNames(list);
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
            return list.ToArray();
        }
        private static void FillNames(List<LocalIpData> list) {
            //获取网卡
            var items = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var item in list) {
                foreach (NetworkInterface ni in items) {
                    if (ni.Id == item.SettingID) {
                        item.Name = ni.Name;
                        break;
                    }
                }
            }
        }
        #endregion

        private LocalIpData[] _localIps = new LocalIpData[0];
        public LocalIpSet() {
            NetworkChange.NetworkAddressChanged += (object sender, EventArgs e) => {
                // 延迟获取网络信息以防止立即获取时获取不到
                1.SecondsDelay().ContinueWith(t => {
                    var old = _localIps;
                    _isInited = false;
                    InitOnece();
                    var localIps = _localIps;
                    if (localIps.Length == 0) {
                        VirtualRoot.ThisLocalWarn(nameof(LocalIpSet), "网络连接已断开", toConsole: true);
                    }
                    else {
                        if (old.Length == 0) {
                            VirtualRoot.ThisLocalInfo(nameof(LocalIpSet), "网络连接已连接", toConsole: true);
                        }
                        else {
                            bool isIpChanged = false;
                            if (old.Length != localIps.Length) {
                                isIpChanged = true;
                            }
                            else {
                                foreach (var item in localIps) {
                                    var oldItem = old.FirstOrDefault(a => a.SettingID == item.SettingID);
                                    if (item != oldItem) {
                                        isIpChanged = true;
                                        break;
                                    }
                                }
                            }
                            VirtualRoot.ThisLocalWarn(nameof(LocalIpSet), $"网络接口的 IP 地址发生了 {(isIpChanged ? "变更" : "刷新")}", toConsole: true);
                        }
                    }
                });
            };
            NetworkChange.NetworkAvailabilityChanged += (object sender, NetworkAvailabilityEventArgs e) => {
                if (e.IsAvailable) {
                    VirtualRoot.ThisLocalInfo(nameof(LocalIpSet), $"网络可用", toConsole: true);
                }
                else {
                    VirtualRoot.ThisLocalWarn(nameof(LocalIpSet), $"网络不可用", toConsole: true);
                }
            };
            VirtualRoot.AddCmdPath<SetLocalIpCommand>(action: message => {
                ManagementObject mo = null; 
                using (ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration"))
                using (ManagementObjectCollection moc = mc.GetInstances()) {
                    foreach (ManagementObject item in moc) {
                        if ((string)item["SettingID"] == message.Input.SettingID) {
                            mo = item;
                            break;
                        }
                    }
                }
                if (mo != null) {
                    if (message.Input.DHCPEnabled) {
                        mo.InvokeMethod("EnableStatic", null);
                        mo.InvokeMethod("SetGateways", null);
                        mo.InvokeMethod("EnableDHCP", null);
                        1.SecondsDelay().ContinueWith(t => {
                            _isInited = false;
                            InitOnece();
                        });
                    }
                    else {
                        ManagementBaseObject inPar = mo.GetMethodParameters("EnableStatic");
                        inPar["IPAddress"] = new string[] { message.Input.IPAddress };
                        inPar["SubnetMask"] = new string[] { message.Input.IPSubnet };
                        mo.InvokeMethod("EnableStatic", inPar, null);
                        inPar = mo.GetMethodParameters("SetGateways");
                        inPar["DefaultIPGateway"] = new string[] { message.Input.DefaultIPGateway };
                        mo.InvokeMethod("SetGateways", inPar, null);
                    }

                    if (message.IsAutoDNSServer) {
                        mo.InvokeMethod("SetDNSServerSearchOrder", null);
                    }
                    else {
                        ManagementBaseObject inPar = mo.GetMethodParameters("SetDNSServerSearchOrder");
                        inPar["DNSServerSearchOrder"] = new string[] { message.Input.DNSServer0, message.Input.DNSServer1 };
                        mo.InvokeMethod("SetDNSServerSearchOrder", inPar, null);
                    }
                }
            }, location: this.GetType());
        }

        private bool _isInited = false;
        private readonly object _locker = new object();

        public void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
#if DEBUG
            NTStopwatch.Start();
#endif
            lock (_locker) {
                if (!_isInited) {
                    _isInited = true;
                    _localIps = GetLocalIps();
                    VirtualRoot.RaiseEvent(new LocalIpSetInitedEvent());
                }
            }
#if DEBUG
            // 将近300毫秒
            var elapsedMilliseconds = NTStopwatch.Stop();
            if (elapsedMilliseconds.ElapsedMilliseconds > NTStopwatch.ElapsedMilliseconds) {
                Write.DevTimeSpan($"耗时{elapsedMilliseconds} {this.GetType().Name}.{nameof(Init)}");
            }
#endif
        }

        public IEnumerable<ILocalIp> AsEnumerable() {
            InitOnece();
            return _localIps;
        }

        public List<string> GetAllSubnetIps() {
            InitOnece();
            var localIp = _localIps.FirstOrDefault();
            if (localIp != null) {
                var fromIp = localIp.DefaultIPGateway;
                if (!string.IsNullOrEmpty(fromIp)) {
                    fromIp = IpUtil.ConvertToIpString(IpUtil.ConvertToIpNum(fromIp) + 1);
                    string[] parts = fromIp.Split('.');
                    parts[parts.Length - 1] = "255";
                    var toIp = string.Join(".", parts);
                    return IpUtil.CreateIpRange(fromIp, toIp);
                }
            }

            return new List<string>();
        }
    }
}
