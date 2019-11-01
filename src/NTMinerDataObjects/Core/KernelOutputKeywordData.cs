using System;

namespace NTMiner.Core {
    public class KernelOutputKeywordData : IKernelOutputKeyword, IDbEntity<Guid> {
        public static KernelOutputKeywordData Create(IKernelOutputKeyword data) {
            if (data == null) {
                return null;
            }
            if (data is KernelOutputKeywordData result) {
                return result;
            }
            return new KernelOutputKeywordData {
                Id = data.GetId(),
                KernelOutputId = data.KernelOutputId,
                MessageType = data.MessageType,
                Keyword = data.Keyword
            };
        }

        public KernelOutputKeywordData() { }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id { get; set; }
        public Guid KernelOutputId { get; set; }

        public string MessageType { get; set; }

        public string Keyword { get; set; }
    }
}
