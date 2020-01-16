namespace NTMiner.Core {
    /// <summary>
    /// 从内核进程获取输出信息的方式分两种：1，内核如果支持写日志文件则通过读取日志文件获取；2，通过管道将内核的输出输送到日志文件然后读取；
    /// </summary>
    public enum KernelProcessType {
        Logfile = 0,
        Pip = 1
    }
}
