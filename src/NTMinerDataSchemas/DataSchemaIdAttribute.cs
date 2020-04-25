using System;

namespace NTMiner {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DataSchemaIdAttribute : Attribute {
        public DataSchemaIdAttribute(string id) {
            this.Id = id;
        }

        public string Id { get; private set; }
    }
}
