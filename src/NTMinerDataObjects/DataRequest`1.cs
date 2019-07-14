using System;
using System.Text;

namespace NTMiner {
    public class DataRequest<T> : RequestBase, IGetSignData {
        public DataRequest() { }
        public T Data { get; set; }

        private string GetData() {
            Type dataType = typeof(T);
            if (dataType.IsValueType) {
                return dataType.ToString();
            }

            if (Data == null) {
                return string.Empty;
            }
            if (typeof(IGetSignData).IsAssignableFrom(dataType)) {
                return ((IGetSignData)Data).GetSignData().ToString();
            }

            return Data.ToString();
        }

        public StringBuilder GetSignData() {
            StringBuilder sb = new StringBuilder();
            sb.Append(nameof(Data)).Append(GetData());
            return sb;
        }
    }
}
