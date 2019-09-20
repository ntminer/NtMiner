using NTMiner.MinerClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Management;

namespace NTMiner.Ip.Impl {
    public class LocalIpSet : ILocalIpSet {
        private List<ILocalIp> _localIps = new List<ILocalIp>();

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

        private static List<ILocalIp> GetLocalIps() {
            List<ILocalIp> list = new List<ILocalIp>();
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
                        SettingID = new Guid(mo["SettingID"].ToString()),
                        DNSServer0 = dNSServer0,
                        DNSServer1 = dNSServer1,
                        IPAddress = ipAddress
                    });
                }
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
            return list;
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
