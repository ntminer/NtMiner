using System;

namespace NTMiner.Core {
    // 开源矿工最初版本时界面上的内核输出是通过读取内核日志转述到界面上的，
    // 转述时进行翻译是有意义的。现在不再转述，界面上的内核输出是原版控制
    // 台窗口，所以这个翻译器一半的功能已经无用了，前译在用。
    // 前译：通常用于将不标准的算力单位替换为标准算力单位，比如将sol/s替换为H/s
    public interface IKernelOutputTranslater : IEntity<Guid> {
        Guid KernelOutputId { get; }
        string RegexPattern { get; }
        string Replacement { get; }
        int SortNumber { get; }
        bool IsPre { get; }
    }
}
