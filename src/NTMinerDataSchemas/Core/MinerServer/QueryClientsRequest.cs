using System;
using System.Text;

namespace NTMiner.Core.MinerServer {
    public class QueryClientsRequest : IRequest, ISignableData {
        public QueryClientsRequest() {
            this.SortField = ClientDataSortField.MinerName;
        }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? WorkId { get; set; }
        public string MinerIp { get; set; }
        public string MinerName { get; set; }
        public MineStatus MineState { get; set; }
        public string Coin { get; set; }
        public string Pool { get; set; }
        public string Wallet { get; set; }
        public string Version { get; set; }
        public string Kernel { get; set; }
        public GpuType GpuType { get; set; }
        public string GpuName { get; set; }
        public string GpuDriver { get; set; }
        public SortDirection SortDirection { get; set; }
        public ClientDataSortField SortField { get; set; }

        public StringBuilder GetSignData() {
            return this.GetActionIdSign("5BB1CE8C-995E-47D6-AF1A-A8303E53E296");
        }
    }
}
