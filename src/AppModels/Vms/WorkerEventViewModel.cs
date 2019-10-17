using NTMiner.MinerClient;
using System;

namespace NTMiner.Vms {
    public class WorkerEventViewModel : ViewModelBase, IWorkerEvent {
        private readonly WorkerEventData _data;

        public WorkerEventViewModel(WorkerEventData data) {
            _data = data;
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id {
            get {
                return _data.Id;
            }
        }

        public Guid EventTypeId {
            get {
                return _data.EventTypeId;
            }
        }

        public string Content {
            get {
                return _data.Content;
            }
        }

        public DateTime EventOn {
            get {
                return _data.EventOn;
            }
        }
    }
}
