using LiteDB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Gpus.Impl.Nvidia {
    public class CudaVersionSet : IEnumerable<ICudaVersion> {
        private List<CudaVersion> _list = new List<CudaVersion>();

        private readonly INTMinerRoot _root;
        private readonly bool _isUseJson;

        public CudaVersionSet(INTMinerRoot root, bool isUseJson) {
            _isUseJson = isUseJson;
            _root = root;
            VirtualRoot.Window<AddCudaVersionCommand>("添加组", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || string.IsNullOrEmpty(message.Input.Version)) {
                        throw new ArgumentNullException();
                    }
                    if (_list.Any(a => a.Version == message.Input.Version)) {
                        return;
                    }
                    CudaVersion entity = new CudaVersion().Update(message.Input);
                    _list.Add(entity);
                    using (LiteDatabase db = new LiteDatabase($"filename={SpecialPath.ServerDbFileFullName};journal=false")) {
                        var col = db.GetCollection<CudaVersion>();
                        col.Insert(entity);
                    }

                    VirtualRoot.Happened(new CudaVersionAddedEvent(entity));
                }).AddToCollection(root.ContextHandlers);
            VirtualRoot.Window<UpdateCudaVersionCommand>("更新组", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Version)) {
                        throw new ValidationException("CudaVersion Version can't be null or empty");
                    }
                    CudaVersion entity = _list.FirstOrDefault(a => a.Version == message.Input.Version);
                    if (entity == null) {
                        return;
                    }
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
                    entity.Update(message.Input);
                    using (LiteDatabase db = new LiteDatabase($"filename={SpecialPath.ServerDbFileFullName};journal=false")) {
                        var col = db.GetCollection<CudaVersion>();
                        col.Update(entity);
                    }

                    VirtualRoot.Happened(new CudaVersionUpdatedEvent(entity));
                }).AddToCollection(root.ContextHandlers);
            VirtualRoot.Window<RemoveCudaVersionCommand>("移除组", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || string.IsNullOrEmpty(message.CudaVersion)) {
                        throw new ArgumentNullException();
                    }
                    CudaVersion entity = _list.FirstOrDefault(a => a.Version == message.CudaVersion);
                    if (entity == null) {
                        return;
                    }
                    _list.Remove(entity);
                    using (LiteDatabase db = new LiteDatabase($"filename={SpecialPath.ServerDbFileFullName};journal=false")) {
                        var col = db.GetCollection<CudaVersion>();
                        col.Delete(entity.Version);
                    }

                    VirtualRoot.Happened(new CudaVersionRemovedEvent(entity));
                }).AddToCollection(root.ContextHandlers);
        }

        private bool _isInited = false;
        private object _locker = new object();

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
            lock (_locker) {
                if (!_isInited) {
                    if (_isUseJson) {
                        _list = NTMinerRoot.GetCudaVersions().Select(a => new CudaVersion(a)).ToList();
                    }
                    else {
                        using (LiteDatabase db = new LiteDatabase($"filename={SpecialPath.ServerDbFileFullName};journal=false")) {
                            var col = db.GetCollection<CudaVersion>();
                            _list = col.FindAll().ToList();
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public IEnumerator<ICudaVersion> GetEnumerator() {
            InitOnece();
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _list.GetEnumerator();
        }
    }
}
