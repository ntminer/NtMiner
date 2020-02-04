using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NTMiner {
    public class DataRequest<T> : RequestBase, IGetSignData {
        public DataRequest() { }
        [ManualSign]
        public T Data { get; set; }

        private void Append(StringBuilder sb) {
            Type dataType = typeof(T);
            if (dataType.IsValueType) {
                sb.Append(dataType.ToString());
                return;
            }

            if (Data == null) {
                return;
            }
            if (typeof(IGetSignData).IsAssignableFrom(dataType)) {
                sb.Append(((IGetSignData)Data).GetSignData().ToString());
                return;
            }
            else if (this.Data is IEnumerable<IGetSignData> datas) {
                foreach (var item in datas) {
                    if (item != null) {
                        sb.Append(item.BuildSign().ToString());
                    }
                }
            }
            else if (this.Data is IEnumerable items) {
                foreach (var item in items) {
                    if (item != null) {
                        sb.Append(item.ToString());
                    }
                }
            }

            sb.Append(Data.ToString());
        }

        public StringBuilder GetSignData() {
            StringBuilder sb = this.BuildSign();
            sb.Append(nameof(Data));
            Append(sb);
            return sb;
        }
    }
}
