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

        private static List<LocalIpData> GetLocalIps() {
            List<LocalIpData> list = new List<LocalIpData>();
            try {
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
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
                            dNSServer0 = dNSServerSearchOrder[0];
                        }
                        if (dNSServerSearchOrder.Length > 1) {
                            dNSServer1 = dNSServerSearchOrder[1];
                        }
                    }
                    string ipAddress = string.Empty;
                    if (mo["IPAddress"] != null) {
                        string[] items = (string[])mo["IPAddress"];
                        if (items.Length != 0) {
                            ipAddress = items[0];// 只去Ipv4
                        }
                    }
                    list.Add(new LocalIpData {
                        DefaultIPGateway = defaultIpGateways[0],
                        DHCPEnabled = (bool)mo["DHCPEnabled"],
                        DHCPServer = (string)mo["DHCPServer"],
                        SettingID = (string)mo["SettingID"],
                        DNSServer0 = dNSServer0,
                        DNSServer1 = dNSServer1,
                        IPAddress = ipAddress
                    });
                    FillNames(list);
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
