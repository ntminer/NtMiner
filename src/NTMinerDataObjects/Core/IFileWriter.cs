using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NTMiner.Core {
    public interface IFileWriter {
        /// <summary>
        /// 相对路径。
        /// </summary>
        string FileUrl { get; }

        /// <summary>
        /// 该字符串里面可能具有变量，比如{pool1}、{wallet1}、{userName1}等
        /// </summary>
        string Boday { get; }

        /// <summary>
        /// 形式参数。只有形式参数匹配时该文件写作者才会执行。
        /// </summary>
        string[] Parameters { get; }
    }
}
