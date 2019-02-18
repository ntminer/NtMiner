using System;

namespace NTMiner {
    public class RequestBase {
        public RequestBase() {
            this.MessageId = Guid.NewGuid();
            this.Timestamp = DateTime.Now;
        }

        /// <summary>
        /// 有些消息需要异步持久跟踪所以需要标识
        /// </summary>
        public Guid MessageId { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
