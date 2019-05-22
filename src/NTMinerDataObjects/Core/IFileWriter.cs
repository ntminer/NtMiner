using System;

namespace NTMiner.Core {
    /// <summary>
    /// 运行时根据上下文和该对象往指定的位置写文件
    /// </summary>
    public interface IFileWriter : IEntity<Guid> {
        /// <summary>
        /// 相对路径，路径的根由上下文确定。
        /// </summary>
        string FileUrl { get; }

        /// <summary>
        /// 该字符串里面往往具有变量，比如{pool}、{host}、{port}、{wallet}、{userName}、{pool1}、{wallet1}、{userName1}等。
        /// 运行时会识别出里面的形参，并且只有当Body中所有的形参都被填充了时才会执行。
        /// </summary>
        string Body { get; }
    }
}
