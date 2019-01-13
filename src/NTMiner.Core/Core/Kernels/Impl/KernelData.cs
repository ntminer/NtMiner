using LiteDB;
using Newtonsoft.Json;
using System;

namespace NTMiner.Core.Kernels.Impl {
    public class KernelData : IKernel, IDbEntity<Guid> {
        public KernelData() {
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }

        public string Code { get; set; }

        public string Version { get; set; }

        [JsonIgnore]
        [BsonIgnore]
        public string FullName {
            get {
                return $"{this.Code}{this.Version}";
            }
        }
        public ulong PublishOn { get; set; }

        public string Package { get; set; }

        public string PackageHistory { get; set; }

        public string Sha1 { get; set; }

        public long Size { get; set; }

        public int SortNumber { get; set; }

        public PublishStatus PublishState { get; set; }

        public Guid DualCoinGroupId { get; set; }
        public string Args { get; set; }
        public bool IsSupportDualMine { get; set; }
        public double DualWeightMin { get; set; }
        public double DualWeightMax { get; set; }
        public bool IsAutoDualWeight { get; set; }
        public string DualWeightArg { get; set; }
        public string DualFullArgs { get; set; }


        public string HelpArg { get; set; }

        public string Notice { get; set; }

        public Guid KernelOutputId { get; set; }
    }
}
