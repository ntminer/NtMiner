using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.Core.Kernels.Impl {
    public class KernelOutputSet : SetBase, IKernelOutputSet {
        private readonly Dictionary<Guid, KernelOutputData> _dicById = new Dictionary<Guid, KernelOutputData>();

        private readonly IServerContext _context;
        public KernelOutputSet(IServerContext context) {
            _context = context;
            #region 接线
            context.AddCmdPath<AddKernelOutputCommand>(LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    KernelOutputData entity = new KernelOutputData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    var repository = context.CreateServerRepository<KernelOutputData>();
                    repository.Add(entity);

                    VirtualRoot.RaiseEvent(new KernelOutputAddedEvent(message.MessageId, entity));
                }, location: this.GetType());
            context.AddCmdPath<UpdateKernelOutputCommand>(LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Name)) {
                        throw new ValidationException($"{nameof(message.Input.Name)} can't be null or empty");
                    }
                    if (!_dicById.TryGetValue(message.Input.GetId(), out KernelOutputData entity)) {
                        return;
                    }
                    if (ReferenceEquals(entity, message.Input)) {
                        return;
                    }
                    entity.Update(message.Input);
                    var repository = context.CreateServerRepository<KernelOutputData>();
                    repository.Update(entity);

                    VirtualRoot.RaiseEvent(new KernelOutputUpdatedEvent(message.MessageId, entity));
                }, location: this.GetType());
            context.AddCmdPath<RemoveKernelOutputCommand>(LogEnum.DevConsole,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    IKernel[] outputUses = context.KernelSet.AsEnumerable().Where(a => a.KernelOutputId == message.EntityId).ToArray();
                    if (outputUses.Length != 0) {
                        throw new ValidationException($"这些内核在使用该内核输出组，删除前请先解除使用：{string.Join(",", outputUses.Select(a => a.GetFullName()))}");
                    }
                    KernelOutputData entity = _dicById[message.EntityId];
                    List<Guid> kernelOutputTranslaterIds = context.KernelOutputTranslaterSet.AsEnumerable().Where(a => a.KernelOutputId == entity.Id).Select(a => a.GetId()).ToList();
                    foreach (var kernelOutputTranslaterId in kernelOutputTranslaterIds) {
                        VirtualRoot.Execute(new RemoveKernelOutputTranslaterCommand(kernelOutputTranslaterId));
                    }
                    _dicById.Remove(entity.GetId());
                    var repository = context.CreateServerRepository<KernelOutputData>();
                    repository.Remove(message.EntityId);

                    VirtualRoot.RaiseEvent(new KernelOutputRemovedEvent(message.MessageId, entity));
                }, location: this.GetType());
            #endregion
        }

        // 填充空间
        protected override void Init() {
            var repository = _context.CreateServerRepository<KernelOutputData>();
            foreach (var item in repository.GetAll()) {
                if (!_dicById.ContainsKey(item.GetId())) {
                    _dicById.Add(item.GetId(), item);
                }
            }
        }

        public bool Contains(Guid id) {
            InitOnece();
            return _dicById.ContainsKey(id);
        }

        public bool TryGetKernelOutput(Guid id, out IKernelOutput kernelOutput) {
            InitOnece();
            var result = _dicById.TryGetValue(id, out KernelOutputData data);
            kernelOutput = data;
            return result;
        }

        public IEnumerable<IKernelOutput> AsEnumerable() {
            InitOnece();
            return _dicById.Values;
        }
    }
}
