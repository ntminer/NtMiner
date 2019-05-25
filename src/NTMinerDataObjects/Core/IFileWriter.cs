namespace NTMiner.Core {
    /// <summary>
    /// 运行时根据上下文和该对象往指定的位置写文件
    /// </summary>
    public interface IFileWriter : IFragmentWriter {
        /// <summary>
        /// 相对路径，路径的根由上下文确定。
        /// </summary>
        string FileUrl { get; }
    }
}
