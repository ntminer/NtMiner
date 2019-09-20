using NTMiner.MinerClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Management;
using System.Net.NetworkInformation;

namespace NTMiner.Ip.Impl {
    public class LocalIpSet : ILocalIpSet {
        private List<LocalIpData> _localIps = new List<LocalIpData>();

        public LocalIpSet() { }

        private bool _isInited = false;
        private readonly object _locker = new object();

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
            lock (_locker) {
                if (!_isInited) {
                    _localIps = GetLocalIps();
                    _isInited = true;
                }
            }
        }

        public void SetIp(ILocalIp data, bool isAutoDNSServer) {
            ManagementObject mo = GetManagementObject(data.SettingID);
            if (mo != null) {
                if (data.DHCPEnabled) {
                    mo.InvokeMethod("EnableStatic", null);
                    mo.InvokeMethod("SetGateways", null);
                    mo.InvokeMethod("EnableDHCP", null);
                }
                else {
                    ManagementBaseObject inPar = mo.GetMethodParameters("EnableStatic");
                    inPar["IPAddress"] = new string[] { data.IPAddress };
                    inPar["SubnetMask"] = new string[] { data.IPSubnet };
                    mo.InvokeMethod("EnableStatic", inPar, null);
                    inPar = mo.GetMethodParameters("SetGateways");
                    inPar["DefaultIPGateway"] = new string[] { data.DefaultIPGateway };
                    mo.InvokeMethod("SetGateways", inPar, null);
                }

                if (isAutoDNSServer) {
                    mo.InvokeMethod("SetDNSServerSearchOrder", null);
                }
                else {
                    ManagementBaseObject inPar = mo.GetMethodParameters("SetDNSServerSearchOrder");
                    if (data.DNSServer0 == "0.0.0.0") {
                        VirtualRoot.Out.ShowErrorMessage("首选 DNS 服务器不能为空", delaySeconds: 4);
                        return;
                    }
                    if (data.DNSServer1 == "0.0.0.0") {
                        inPar["DNSServerSearchOrder"] = new string[] { data.DNSServer0 };
                    }
                    else {
                        inPar["DNSServerSearchOrder"] = new string[] { data.DNSServer0, data.DNSServer1 };
                    }
                    mo.InvokeMethod("SetDNSServerSearchOrder", inPar, null);
                }
            }
        }

        private static ManagementObject GetManagementObject(string settingId) {
            using (ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration")) {
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc) {
                    if ((string)mo["SettingID"] == settingId) {
                        return mo;
                    }
                }
            }
            return null;
        }

        private static List<LocalIpData> GetLocalIps() {
            List<LocalIpData> list = new List<LocalIpData>();
            try {
                using (ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration")) {
                    ManagementObjectCollection moc = mc.GetInstances();
                    foreach (ManagementObject mo in moc) {
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
                            IPAddress = ipAddress
                        });
                        FillNames(list);
                    }
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
            return list;
        }

        private static void FillNames(List<LocalIpData> list) {
            //获取网卡
            var items = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var item in list) {
                foreach (NetworkInterface ni in items) {
                    if (ni.Id == item.SettingID) {
                        item.Name = ni.Name;
                    }
                }
            }
        }

        public void Refresh() {
            _isInited = false;
            VirtualRoot.Happened(new LocalIpSetRefreshedEvent());
        }

        public IEnumerator<ILocalIp> GetEnumerator() {
            InitOnece();
            return _localIps.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _localIps.GetEnumerator();
        }
    }
}
