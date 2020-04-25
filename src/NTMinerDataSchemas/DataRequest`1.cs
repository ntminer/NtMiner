using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NTMiner {
    public class DataRequest<T> : IRequest, ISignableData {
        public DataRequest() { }
        [ManualSign]
        public T Data { get; set; }

        public StringBuilder GetSignData() {
            StringBuilder sb = this.BuildSign();
            sb.Append(nameof(Data));
            AppendData(sb, Data);
            return sb;
        }

        private static void AppendData(StringBuilder sb, T data) {
            if (data == null) {
                return;
            }
            Type dataType = typeof(T);
            if (dataType.IsValueType) {
                sb.Append(dataType.ToString());
                return;
            }
            else if (dataType.TryGetAttribute(out DataSchemaIdAttribute schemaId)) {
                sb.Append(schemaId.Id);
                return;
            }
            else if (typeof(ISignableData).IsAssignableFrom(dataType)) {
                sb.Append(((ISignableData)data).GetSignData().ToString());
                return;
            }
            else if (data is IEnumerable<ISignableData> datas) {
                foreach (var item in datas) {
                    if (item != null) {
                        sb.Append(item.BuildSign().ToString());
                    }
                }
            }
            else if (data is IEnumerable items) {
                foreach (var item in items) {
                    if (item != null) {
                        sb.Append(item.ToString());
                    }
                }
            }

            sb.Append(data.ToString());
        }
    }
}
