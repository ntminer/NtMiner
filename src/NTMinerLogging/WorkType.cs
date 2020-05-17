namespace NTMiner {
    /// <summary>
    /// 作业类型。主要是因为HomePath这个类型对WorkType有依赖所以这个类型出现在这里。
    /// </summary>
    public enum WorkType {
        /// <summary>
        /// 未使用作业
        /// </summary>
        None,
        /// <summary>
        /// 单机作业
        /// </summary>
        SelfWork,
        /// <summary>
        /// 统一作业
        /// </summary>
        MineWork
    }
}
