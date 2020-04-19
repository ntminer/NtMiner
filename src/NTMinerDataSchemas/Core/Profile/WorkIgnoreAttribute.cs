using System;

namespace NTMiner.Core.Profile {
    /// <summary>
    /// 在MinerProfileData类型的属性上标记该属性，标记了该属性的意思是被标记的属性的值不能被作业覆写。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class WorkIgnoreAttribute : Attribute {
    }
}
