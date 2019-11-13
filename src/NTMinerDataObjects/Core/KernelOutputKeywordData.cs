using System;

namespace NTMiner.Core {
    public class KernelOutputKeywordData : IKernelOutputKeyword, ILevelEntity<Guid> {
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
                Keyword = data.Keyword,
                DataLevel = data.DataLevel,
                Description = data.Description
            };
        }

        public KernelOutputKeywordData() { }

        public Guid GetId() {
            return this.Id;
        }

        [LiteDB.BsonIgnore]
        public DataLevel DataLevel { get; set; }

        public void SetDataLevel(DataLevel dataLevel) {
            this.DataLevel = dataLevel;
        }

        public Guid Id { get; set; }
        public Guid KernelOutputId { get; set; }

        public string MessageType { get; set; }

        public string Keyword { get; set; }

        public string Description { get; set; }
    }
}
