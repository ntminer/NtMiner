namespace NTMiner.Core.Gpus {
    /// <summary>
    /// GpuName是管理员人脑基于<see cref="GpuNameCount"/>集提取的特征名。
    /// </summary>
    public class GpuName : IGpuName {
        public static bool IsValidTotalMemory(ulong value) {
            return value >= 4 * NTKeyword.ULongG;
        }

        public static string Format(GpuType gpuType, string gpuName, ulong totalMemory) {
            ulong totalMemoryGb = (totalMemory + NTKeyword.ULongG - 1) / NTKeyword.ULongG;
            // 通常显卡的名称上会带显存大小，比如1060分3G版和6G版所以NVIDIA命名显卡的时候
            // 已经带上了显存信息，但不能假定带了显存信息所以这里拼接上显存信息。
            return $"{gpuType.GetName()}///{gpuName}///{totalMemoryGb.ToString()}";
        }

        public GpuName() { }

        public GpuType GpuType { get; set; }

        /// <summary>
        /// 注意：匹配名称的时候注意按照名称长短的顺序由长到短运算，就是说先用5700关键字匹配再用570关键字匹配。
        /// </summary>
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
            return Format(this.GpuType, this.Name, this.TotalMemory);
        }
    }
}
