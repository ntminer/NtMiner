namespace NTMiner.Core.Gpus {
    public class GpuName : IGpuName {
        public static bool IsValidTotalMemory(ulong value) {
            return value >= 4 * NTKeyword.ULongG;
        }

        public GpuName() { }

        public GpuType GpuType { get; set; }

        public string Name { get; set; }

        public ulong TotalMemory { get; set; }

        public bool IsValid() {
            return GpuType != GpuType.Empty && !string.IsNullOrEmpty(Name) && IsValidTotalMemory(TotalMemory);
        }

        public override bool Equals(object obj) {
            if (obj == null) {
                return false;
            }
            return this.ToString() == obj.ToString(); ;
        }

        public override int GetHashCode() {
            return this.ToString().GetHashCode();
        }

        /// <summary>
        /// 该ToString字符串会被作为redis key使用
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            ulong totalMemoryGb = (this.TotalMemory + NTKeyword.ULongG - 1) / NTKeyword.ULongG;
            // 通常显卡的名称上会带显存大小，比如1060分3G版和6G版所以NVIDIA命名显卡的时候
            // 已经带上了显存信息，但不能假定带了显存信息所以这里拼接上显存信息。
            return $"{this.GpuType.ToString()}///{this.Name}///{totalMemoryGb.ToString()}";
        }
    }
}
