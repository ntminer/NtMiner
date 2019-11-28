using System;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class FileWriterSet : IFileWriterSet {
        private readonly Dictionary<Guid, FileWriterData> _dicById = new Dictionary<Guid, FileWriterData>();

        public FileWriterSet(IServerContext context) {
            context.AddCmdPath<AddFileWriterCommand>("添加文件书写器", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    if (string.IsNullOrEmpty(message.Input.FileUrl) || string.IsNullOrEmpty(message.Input.Body)) {
                        throw new ValidationException("FileWriter name and body can't be null or empty");
                    }
                    FileWriterData entity = new FileWriterData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    var repository = NTMinerRoot.CreateServerRepository<FileWriterData>();
                    repository.Add(entity);

                    VirtualRoot.RaiseEvent(new FileWriterAddedEvent(message.Id, entity));
                });
            context.AddCmdPath<UpdateFileWriterCommand>("更新文件书写器", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.FileUrl) || string.IsNullOrEmpty(message.Input.Body)) {
                        throw new ValidationException("FileWriter name and body can't be null or empty");
                    }
                    if (!_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    FileWriterData entity = _dicById[message.Input.GetId()];
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
                    entity.Update(message.Input);
                    var repository = NTMinerRoot.CreateServerRepository<FileWriterData>();
                    repository.Update(entity);

                    VirtualRoot.RaiseEvent(new FileWriterUpdatedEvent(message.Id, entity));
                });
            context.AddCmdPath<RemoveFileWriterCommand>("移除文件书写器", LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    FileWriterData entity = _dicById[message.EntityId];
                    _dicById.Remove(entity.GetId());
                    var repository = NTMinerRoot.CreateServerRepository<FileWriterData>();
                    repository.Remove(message.EntityId);

                    VirtualRoot.RaiseEvent(new FileWriterRemovedEvent(message.Id, entity));
                });
        }

        private bool _isInited = false;
        private readonly object _locker = new object();

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
            lock (_locker) {
                if (!_isInited) {
                    var repository = NTMinerRoot.CreateServerRepository<FileWriterData>();
                    foreach (var item in repository.GetAll()) {
                        if (!_dicById.ContainsKey(item.GetId())) {
                            _dicById.Add(item.GetId(), item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public bool TryGetFileWriter(Guid writerId, out IFileWriter writer) {
            InitOnece();
            FileWriterData g;
            bool r = _dicById.TryGetValue(writerId, out g);
            writer = g;
            return r;
        }

        public IEnumerable<IFileWriter> AsEnumerable() {
            InitOnece();
            return _dicById.Values;
        }
    }
}
