namespace NTMiner.Vms {
    public class ServerHostItem {
        public ServerHostItem(string ipOrHost) {
            if (ipOrHost == null) {
                throw new System.ArgumentNullException(nameof(ipOrHost));
            }
            this.IpOrHost = ipOrHost;
            this.IsInnerIp = Net.Util.IsInnerIp(ipOrHost);
            this.IsLocalHost = Net.Util.IsLocalHost(ipOrHost);
        }

        public string IpOrHost { get; set; }
        public bool IsInnerIp { get; set; }
        public bool IsLocalHost { get; set; }
    }
}
