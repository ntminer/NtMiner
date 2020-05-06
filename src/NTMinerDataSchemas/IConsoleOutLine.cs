namespace NTMiner {
    public interface IConsoleOutLine {
        /// <summary>
        /// 使用long类型比DateTime类型消减一些序列化体积
        /// </summary>
        long Timestamp { get; }
        string Line { get; }
    }
}
