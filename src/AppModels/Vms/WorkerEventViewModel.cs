using NTMiner.MinerClient;
using System;

namespace NTMiner.Vms {
    public class WorkerEventViewModel : ViewModelBase, IWorkerEvent {
        private readonly IWorkerEvent _data;

        public WorkerEventViewModel(IWorkerEvent data) {
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

        public WorkerEventType Level {
            get {
                return _data.Level;
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

        public string EventOnText {
            get {
                int offDay = (DateTime.Now.Date - _data.EventOn.Date).Days;
                switch (offDay) {
                    case 0:
                        return $"今天 {_data.EventOn.TimeOfDay.ToString("hh\\:mm\\:ss")}";
                    case 1:
                        return $"左天 {_data.EventOn.TimeOfDay.ToString("hh\\:mm\\:ss")}";
                    case 2:
                        return $"前天 {_data.EventOn.TimeOfDay.ToString("hh\\:mm\\:ss")}";
                    default:
                        return _data.EventOn.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
        }
    }
}
