using NTMiner.MinerClient;
using System;

namespace NTMiner.Vms {
    public class WorkerEventViewModel : ViewModelBase, IWorkerEvent {
        private readonly WorkerEventData _data;

        public WorkerEventViewModel(WorkerEventData data) {
            _data = data;
        }

        public int GetId() {
            return _data.Id;
        }

        public int Id {
            get {
                return _data.Id;
            }
        }

        public WorkerEventChannel Channel {
            get {
                return _data.Channel;
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
