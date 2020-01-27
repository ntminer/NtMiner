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
                _dataLevel = data.GetDataLevel(),
                Description = data.Description
            };
        }

        public KernelOutputKeywordData() { }

        public Guid GetId() {
            return this.Id;
        }

        private DataLevel _dataLevel;
        public DataLevel GetDataLevel() {
            return _dataLevel;
        }

        public void SetDataLevel(DataLevel dataLevel) {
            this._dataLevel = dataLevel;
        }

        public Guid Id { get; set; }
        public Guid KernelOutputId { get; set; }

        public string MessageType { get; set; }

        public string Keyword { get; set; }

        public string Description { get; set; }
    }
}
