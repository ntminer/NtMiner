using System;
using System.Collections.Generic;

namespace NTMiner.Core.Impl {
    public class FragmentWriterSet : SetBase, IFragmentWriterSet {
        private readonly Dictionary<Guid, FragmentWriterData> _dicById = new Dictionary<Guid, FragmentWriterData>();

        private readonly IServerContext _context;
        public FragmentWriterSet(IServerContext context) {
            _context = context;
            context.AddCmdPath<AddFragmentWriterCommand>(LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    if (string.IsNullOrEmpty(message.Input.Body)) {
                        throw new ValidationException("FragmentWriter body can't be null or empty");
                    }
                    FragmentWriterData entity = new FragmentWriterData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    var repository = context.CreateServerRepository<FragmentWriterData>();
                    repository.Add(entity);

                    VirtualRoot.RaiseEvent(new FragmentWriterAddedEvent(message.MessageId, entity));
                }, location: this.GetType());
            context.AddCmdPath<UpdateFragmentWriterCommand>(LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Body)) {
                        throw new ValidationException("FragmentWriter body can't be null or empty");
                    }
                    if (!_dicById.TryGetValue(message.Input.GetId(), out FragmentWriterData entity)) {
                        return;
                    }
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
                    entity.Update(message.Input);
                    var repository = context.CreateServerRepository<FragmentWriterData>();
                    repository.Update(entity);

                    VirtualRoot.RaiseEvent(new FragmentWriterUpdatedEvent(message.MessageId, entity));
                }, location: this.GetType());
            context.AddCmdPath<RemoveFragmentWriterCommand>(LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    FragmentWriterData entity = _dicById[message.EntityId];
                    _dicById.Remove(entity.GetId());
                    var repository = context.CreateServerRepository<FragmentWriterData>();
                    repository.Remove(message.EntityId);

                    VirtualRoot.RaiseEvent(new FragmentWriterRemovedEvent(message.MessageId, entity));
                }, location: this.GetType());
        }

        protected override void Init() {
            var repository = _context.CreateServerRepository<FragmentWriterData>();
            foreach (var item in repository.GetAll()) {
                if (!_dicById.ContainsKey(item.GetId())) {
                    _dicById.Add(item.GetId(), item);
                }
            }
        }

        public bool TryGetFragmentWriter(Guid writerId, out IFragmentWriter writer) {
            InitOnece();
            bool r = _dicById.TryGetValue(writerId, out FragmentWriterData g);
            writer = g;
            return r;
        }

        public IEnumerable<IFragmentWriter> AsEnumerable() {
            InitOnece();
            return _dicById.Values;
        }
    }
}
