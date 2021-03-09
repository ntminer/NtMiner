using System;

namespace NTMiner {
    /// <summary>
    /// 忽略内容，只签名一个固定的字符串
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DataSchemaIdAttribute : Attribute {
        public DataSchemaIdAttribute(string id) {
            this.Id = id;
        }

        public string Id { get; private set; }
    }
}
