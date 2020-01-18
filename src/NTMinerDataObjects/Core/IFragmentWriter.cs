using System;

namespace NTMiner.Core {
    /// <summary>
    /// Fragment是片段，里面有变量
    /// </summary>
    public interface IFragmentWriter : IEntity<Guid> {
        Guid Id { get; }
        string Name { get; }

        /// <summary>
        /// 该字符串里面往往具有变量，比如{pool}、{host}、{port}、{wallet}、{userName}、{pool1}、{wallet1}、{userName1}等。
        /// 运行时会识别出里面的形参，并且只有当Body中所有的形参都被填充了时才会执行。
        /// </summary>
        string Body { get; }
    }
}
