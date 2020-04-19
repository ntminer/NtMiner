using System;

namespace NTMiner {
    public static class AnonymousMessagePath {
        /// <summary>
        /// 路径通常在同一地点是不应重复的，但个别正常需求确实会在同一地点重复创建相同的路径，比如借助Per1SecondEvent实现的SecondsDelay方法。
        /// </summary>
        public static readonly Type Location = typeof(AnonymousMessagePath);
    }
}
