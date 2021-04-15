using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class ReadOnlyNTMinerFileSet : SetBase, IReadOnlyNTMinerFileSet {
        private readonly Dictionary<Guid, NTMinerFileData> _dicById = new Dictionary<Guid, NTMinerFileData>();
        private DateTime _timestamp = DateTime.MinValue;

        public ReadOnlyNTMinerFileSet() {
            VirtualRoot.BuildCmdPath<RefreshNTMinerFileSetCommand>(this.GetType(), LogEnum.DevConsole, path: message => {
                Refresh();
            });
        }

        protected override void Init() {
            RpcRoot.OfficialServer.NTMinerFileService.GetNTMinerFilesAsync(_timestamp, (response, e) => {
                if (response.IsSuccess()) {
                    if (response.Data.Count > 0) {
                        _dicById.Clear();
                        _timestamp = response.Timestamp;
                        foreach (var item in response.Data) {
                            _dicById.Add(item.Id, item);
                        }
                        VirtualRoot.RaiseEvent(new NTMinerFileSetInitedEvent());
                    }
                }
                else {
                    Logger.ErrorDebugLine(e.GetInnerMessage(), e);
                }
            });
        }

        private void Refresh() {
            base.DeferReInit();
            InitOnece();
        }

        public IEnumerable<NTMinerFileData> AsEnumerable() {
            Refresh();
            return _dicById.Values;
        }
    }
}
