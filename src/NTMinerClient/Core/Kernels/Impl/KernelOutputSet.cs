using NTMiner.Gpus;
using NTMiner.Mine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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

        private string _preline;
        public void Pick(ref string line, IMineContext mineContext) {
            try {
                InitOnece();
                if (string.IsNullOrEmpty(line)) {
                    return;
                }
                // 注意：硬编码逻辑。使用Claymore挖非ETH币种时它也打印ETH，所以这里需要纠正它
                if ("Claymore".Equals(mineContext.Kernel.Code, StringComparison.OrdinalIgnoreCase)) {
                    if (mineContext.MainCoin.Code != "ETH" && line.Contains("ETH")) {
                        line = line.Replace("ETH", mineContext.MainCoin.Code);
                    }
                }
                ICoin coin = mineContext.MainCoin;
                bool isDual = false;
                Guid poolId = mineContext.MainCoinPool.GetId();
                // 如果是双挖上下文且当前输入行中没有主币关键字则视为双挖币
                if ((mineContext is IDualMineContext dualMineContext) && !line.Contains(mineContext.MainCoin.Code)) {
                    isDual = true;
                    coin = dualMineContext.DualCoin;
                    poolId = dualMineContext.DualCoinPool.GetId();
                }
                INTMinerContext context = NTMinerContext.Instance;
                // 这些方法输出的是事件消息
                #region 总
                PickTotalSpeed(context, line, mineContext.KernelOutput, isDual);
                PickTotalShare(context, line, mineContext.KernelOutput, coin, isDual);
                PickAcceptShare(context, line, mineContext.KernelOutput, coin, isDual);
                PickRejectShare(context, line, mineContext.KernelOutput, coin, isDual);
                PickRejectPercent(context, line, mineContext.KernelOutput, coin, isDual);
                #endregion

                #region 一个
                if (!isDual) {
                    // 决定不支持双挖的单卡份额统计
                    PicFoundOneShare(context, mineContext, line, _preline, mineContext.KernelOutput);
                    PicGotOneIncorrectShare(context, mineContext, line, _preline, mineContext.KernelOutput);
                }
                PickAcceptOneShare(context, mineContext, line, _preline, mineContext.KernelOutput, coin, isDual);
                PickRejectOneShare(context, mineContext, line, _preline, mineContext.KernelOutput, coin, isDual);
                #endregion

                #region 单卡
                PickGpuSpeed(context, mineContext, line, mineContext.KernelOutput, isDual);
                PicGpuAcceptShare(context, mineContext, line, mineContext.KernelOutput, isDual);
                PicGpuRejectShare(context, mineContext, line, mineContext.KernelOutput, isDual);
                PicGpuIncorrectShare(context, mineContext, line, mineContext.KernelOutput, isDual);
                #endregion
                PickPoolDelay(line, mineContext.KernelOutput, isDual, poolId);
                // 如果是像BMiner那样的主币和双挖币的输出在同一行那样的模式则一行输出既要视为主币又要视为双挖币
                if (isDual && mineContext.KernelOutput.IsDualInSameLine) {
                    coin = mineContext.MainCoin;
                    isDual = false;
                    #region 总
                    PickTotalSpeed(context, line, mineContext.KernelOutput, isDual);
                    PickTotalShare(context, line, mineContext.KernelOutput, coin, isDual);
                    PickAcceptShare(context, line, mineContext.KernelOutput, coin, isDual);
                    PickRejectShare(context, line, mineContext.KernelOutput, coin, isDual);
                    PickRejectPercent(context, line, mineContext.KernelOutput, coin, isDual);
                    #endregion

                    #region 一个
                    PickAcceptOneShare(context, mineContext, line, _preline, mineContext.KernelOutput, coin, isDual);
                    PickRejectOneShare(context, mineContext, line, _preline, mineContext.KernelOutput, coin, isDual);
                    #endregion

                    #region 单卡
                    PickGpuSpeed(context, mineContext, line, mineContext.KernelOutput, isDual);
                    PicGpuAcceptShare(context, mineContext, line, mineContext.KernelOutput, isDual);
                    PicGpuRejectShare(context, mineContext, line, mineContext.KernelOutput, isDual);
                    PicGpuIncorrectShare(context, mineContext, line, mineContext.KernelOutput, isDual);
                    #endregion
                    PickPoolDelay(line, mineContext.KernelOutput, isDual, poolId);
                }
                _preline = line;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e);
            }
        }

        #region private static methods
        #region 总
        #region PickTotalSpeed
        private static void PickTotalSpeed(INTMinerContext context, string line, IKernelOutput kernelOutput, bool isDual) {
            string totalSpeedPattern = kernelOutput.TotalSpeedPattern;
            if (isDual) {
                totalSpeedPattern = kernelOutput.DualTotalSpeedPattern;
            }
            if (string.IsNullOrEmpty(totalSpeedPattern)) {
                return;
            }
            Regex regex = VirtualRoot.GetRegex(totalSpeedPattern);
            Match match = regex.Match(line);
            if (match.Success) {
                string totalSpeedText = match.Groups[NTKeyword.TotalSpeedGroupName].Value;
                string totalSpeedUnit = match.Groups[NTKeyword.TotalSpeedUnitGroupName].Value;
                if (string.IsNullOrEmpty(totalSpeedUnit)) {
                    if (isDual) {
                        totalSpeedUnit = kernelOutput.DualSpeedUnit;
                    }
                    else {
                        totalSpeedUnit = kernelOutput.SpeedUnit;
                    }
                }
                if (double.TryParse(totalSpeedText, out double totalSpeed)) {
                    totalSpeed = totalSpeed.FromUnitSpeed(totalSpeedUnit);
                    var now = DateTime.Now;
                    IGpusSpeed gpuSpeeds = NTMinerContext.Instance.GpusSpeed;
                    gpuSpeeds.SetCurrentSpeed(NTMinerContext.GpuAllId, totalSpeed, isDual, now);
                    string gpuSpeedPattern = kernelOutput.GpuSpeedPattern;
                    if (isDual) {
                        gpuSpeedPattern = kernelOutput.DualGpuSpeedPattern;
                    }
                    // 如果没有单卡算力正则则平分总算力作为单卡算力正则
                    if ((string.IsNullOrEmpty(gpuSpeedPattern) || context.GpuSet.Count == 1) && context.GpuSet.Count != 0) {
                        double gpuSpeed = totalSpeed / context.GpuSet.Count;
                        foreach (var item in gpuSpeeds.AsEnumerable()) {
                            if (item.Gpu.Index != NTMinerContext.GpuAllId) {
                                gpuSpeeds.SetCurrentSpeed(item.Gpu.Index, gpuSpeed, isDual, now);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region PickTotalShare
        private static void PickTotalShare(INTMinerContext context, string line, IKernelOutput kernelOutput, ICoin coin, bool isDual) {
            string totalSharePattern = kernelOutput.TotalSharePattern;
            if (isDual) {
                totalSharePattern = kernelOutput.DualTotalSharePattern;
            }
            if (string.IsNullOrEmpty(totalSharePattern)) {
                return;
            }
            Regex regex = VirtualRoot.GetRegex(totalSharePattern);
            var match = regex.Match(line);
            if (match.Success) {
                string totalShareText = match.Groups[NTKeyword.TotalShareGroupName].Value;
                if (int.TryParse(totalShareText, out int totalShare)) {
                    ICoinShare share = context.CoinShareSet.GetOrCreate(coin.GetId());
                    context.CoinShareSet.UpdateShare(coin.GetId(), acceptShareCount: totalShare - share.RejectShareCount, rejectShareCount: null, now: DateTime.Now);
                }
            }
        }
        #endregion

        #region PickAcceptShare
        private static void PickAcceptShare(INTMinerContext context, string line, IKernelOutput kernelOutput, ICoin coin, bool isDual) {
            string acceptSharePattern = kernelOutput.AcceptSharePattern;
            if (isDual) {
                acceptSharePattern = kernelOutput.DualAcceptSharePattern;
            }
            if (string.IsNullOrEmpty(acceptSharePattern)) {
                return;
            }
            Regex regex = VirtualRoot.GetRegex(acceptSharePattern);
            var match = regex.Match(line);
            if (match.Success) {
                string acceptShareText = match.Groups[NTKeyword.AcceptShareGroupName].Value;
                if (int.TryParse(acceptShareText, out int acceptShare)) {
                    context.CoinShareSet.UpdateShare(coin.GetId(), acceptShareCount: acceptShare, rejectShareCount: null, now: DateTime.Now);
                }
            }
        }
        #endregion

        #region PickRejectShare
        private static void PickRejectShare(INTMinerContext context, string line, IKernelOutput kernelOutput, ICoin coin, bool isDual) {
            string rejectSharePattern = kernelOutput.RejectSharePattern;
            if (isDual) {
                rejectSharePattern = kernelOutput.DualRejectSharePattern;
            }
            if (string.IsNullOrEmpty(rejectSharePattern)) {
                return;
            }
            Regex regex = VirtualRoot.GetRegex(rejectSharePattern);
            var match = regex.Match(line);
            if (match.Success) {
                string rejectShareText = match.Groups[NTKeyword.RejectShareGroupName].Value;

                if (int.TryParse(rejectShareText, out int rejectShare)) {
                    context.CoinShareSet.UpdateShare(coin.GetId(), acceptShareCount: null, rejectShareCount: rejectShare, now: DateTime.Now);
                }
            }
        }
        #endregion

        #region PickRejectPercent
        private static void PickRejectPercent(INTMinerContext context, string line, IKernelOutput kernelOutput, ICoin coin, bool isDual) {
            string rejectPercentPattern = kernelOutput.RejectPercentPattern;
            if (isDual) {
                rejectPercentPattern = kernelOutput.DualRejectPercentPattern;
            }
            if (string.IsNullOrEmpty(rejectPercentPattern)) {
                return;
            }
            Regex regex = VirtualRoot.GetRegex(rejectPercentPattern);
            var match = regex.Match(line);
            string rejectPercentText = match.Groups[NTKeyword.RejectPercentGroupName].Value;
            if (double.TryParse(rejectPercentText, out double rejectPercent)) {
                ICoinShare share = context.CoinShareSet.GetOrCreate(coin.GetId());
                context.CoinShareSet.UpdateShare(coin.GetId(), acceptShareCount: null, rejectShareCount: (int)(share.TotalShareCount * rejectPercent), now: DateTime.Now);
            }
        }
        #endregion
        #endregion

        #region 一个
        #region PicFoundOneShare
        private static void PicFoundOneShare(INTMinerContext context, IMineContext mineContext, string line, string preline, IKernelOutput kernelOutput) {
            string foundOneShare = kernelOutput.FoundOneShare;
            if (string.IsNullOrEmpty(foundOneShare)) {
                return;
            }
            if (foundOneShare.Contains("\n")) {
                line = preline + "\n" + line;
            }
            Regex regex = VirtualRoot.GetRegex(foundOneShare);
            var match = regex.Match(line);
            if (match.Success) {
                string gpuText = match.Groups[NTKeyword.GpuIndexGroupName].Value;
                if (!string.IsNullOrEmpty(gpuText)) {
                    if (int.TryParse(gpuText, out int gpuIndex)) {
                        if (IsMapGpuIndex(context, mineContext, kernelOutput) && gpuIndex < mineContext.UseDevices.Length) {
                            gpuIndex = mineContext.UseDevices[gpuIndex];
                        }
                        context.GpusSpeed.IncreaseFoundShare(gpuIndex);
                    }
                }
            }
        }
        #endregion

        #region PicGotOneIncorrectShare
        private static void PicGotOneIncorrectShare(INTMinerContext context, IMineContext mineContext, string line, string preline, IKernelOutput kernelOutput) {
            string pattern = kernelOutput.GpuGotOneIncorrectShare;
            if (string.IsNullOrEmpty(pattern)) {
                return;
            }
            if (pattern.Contains("\n")) {
                line = preline + "\n" + line;
            }
            Regex regex = VirtualRoot.GetRegex(pattern);
            var match = regex.Match(line);
            if (match.Success) {
                string gpuText = match.Groups[NTKeyword.GpuIndexGroupName].Value;
                if (!string.IsNullOrEmpty(gpuText)) {
                    if (int.TryParse(gpuText, out int gpuIndex)) {
                        if (IsMapGpuIndex(context, mineContext, kernelOutput) && gpuIndex < mineContext.UseDevices.Length) {
                            gpuIndex = mineContext.UseDevices[gpuIndex];
                        }
                        context.GpusSpeed.IncreaseIncorrectShare(gpuIndex);
                    }
                }
            }
        }
        #endregion

        #region PickAcceptOneShare
        private static void PickAcceptOneShare(INTMinerContext context, IMineContext mineContext, string line, string preline, IKernelOutput kernelOutput, ICoin coin, bool isDual) {
            string acceptOneShare = kernelOutput.AcceptOneShare;
            if (isDual) {
                acceptOneShare = kernelOutput.DualAcceptOneShare;
            }
            if (string.IsNullOrEmpty(acceptOneShare)) {
                return;
            }
            if (acceptOneShare.Contains("\n")) {
                line = preline + "\n" + line;
            }
            Regex regex = VirtualRoot.GetRegex(acceptOneShare);
            var match = regex.Match(line);
            if (match.Success) {
                if (!isDual) {
                    // 决定不支持双挖的单卡份额统计
                    string gpuText = match.Groups[NTKeyword.GpuIndexGroupName].Value;
                    if (!string.IsNullOrEmpty(gpuText)) {
                        if (int.TryParse(gpuText, out int gpuIndex)) {
                            if (IsMapGpuIndex(context, mineContext, kernelOutput) && gpuIndex < mineContext.UseDevices.Length) {
                                gpuIndex = mineContext.UseDevices[gpuIndex];
                            }
                            if (string.IsNullOrEmpty(kernelOutput.FoundOneShare)) {
                                context.GpusSpeed.IncreaseFoundShare(gpuIndex);
                            }
                            context.GpusSpeed.IncreaseAcceptShare(gpuIndex);
                        }
                    }
                }
                ICoinShare share = context.CoinShareSet.GetOrCreate(coin.GetId());
                context.CoinShareSet.UpdateShare(coin.GetId(), acceptShareCount: share.AcceptShareCount + 1, rejectShareCount: null, now: DateTime.Now);
            }
        }
        #endregion

        #region PickRejectOneShare
        private static void PickRejectOneShare(INTMinerContext context, IMineContext mineContext, string line, string preline, IKernelOutput kernelOutput, ICoin coin, bool isDual) {
            string rejectOneShare = kernelOutput.RejectOneShare;
            if (isDual) {
                rejectOneShare = kernelOutput.DualRejectOneShare;
            }
            if (string.IsNullOrEmpty(rejectOneShare)) {
                return;
            }
            if (rejectOneShare.Contains("\n")) {
                line = preline + "\n" + line;
            }
            Regex regex = VirtualRoot.GetRegex(rejectOneShare);
            var match = regex.Match(line);
            if (match.Success) {
                if (!isDual) {
                    // 决定不支持双挖的单卡份额统计
                    string gpuText = match.Groups[NTKeyword.GpuIndexGroupName].Value;
                    if (!string.IsNullOrEmpty(gpuText)) {
                        if (int.TryParse(gpuText, out int gpuIndex)) {
                            if (IsMapGpuIndex(context, mineContext, kernelOutput) && gpuIndex < mineContext.UseDevices.Length) {
                                gpuIndex = mineContext.UseDevices[gpuIndex];
                            }
                            if (string.IsNullOrEmpty(kernelOutput.FoundOneShare)) {
                                context.GpusSpeed.IncreaseFoundShare(gpuIndex);
                            }
                            context.GpusSpeed.IncreaseRejectShare(gpuIndex);
                        }
                    }
                    else if (!string.IsNullOrEmpty(kernelOutput.FoundOneShare)) {
                        // 哪个GPU最近找到了一个share就是那个GPU拒绝了一个share
                        var gpuSpeeds = context.GpusSpeed.AsEnumerable();
                        IGpuSpeed gpuSpeed = null;
                        foreach (var item in gpuSpeeds) {
                            if (gpuSpeed == null) {
                                gpuSpeed = item;
                            }
                            else if (item.FoundShareOn > gpuSpeed.FoundShareOn) {
                                gpuSpeed = item;
                            }
                        }
                        if (gpuSpeed != null) {
                            var gpuIndex = gpuSpeed.Gpu.Index;
                            context.GpusSpeed.IncreaseRejectShare(gpuIndex);
                        }
                    }
                }
                ICoinShare share = context.CoinShareSet.GetOrCreate(coin.GetId());
                context.CoinShareSet.UpdateShare(coin.GetId(), null, share.RejectShareCount + 1, DateTime.Now);
            }
        }
        #endregion
        #endregion

        #region 单卡
        #region PickGpuSpeed
        private static void PickGpuSpeed(INTMinerContext context, IMineContext mineContext, string line, IKernelOutput kernelOutput, bool isDual) {
            string gpuSpeedPattern = kernelOutput.GpuSpeedPattern;
            if (isDual) {
                gpuSpeedPattern = kernelOutput.DualGpuSpeedPattern;
            }
            if (string.IsNullOrEmpty(gpuSpeedPattern)) {
                return;
            }
            var now = DateTime.Now;
            bool hasGpuId = gpuSpeedPattern.Contains($"?<{NTKeyword.GpuIndexGroupName}>");
            Regex regex = VirtualRoot.GetRegex(gpuSpeedPattern);
            MatchCollection matches = regex.Matches(line);
            if (matches.Count > 0) {
                IGpusSpeed gpuSpeeds = NTMinerContext.Instance.GpusSpeed;
                for (int i = 0; i < matches.Count; i++) {
                    Match match = matches[i];
                    string gpuSpeedText = match.Groups[NTKeyword.GpuSpeedGroupName].Value;
                    string gpuSpeedUnit = match.Groups[NTKeyword.GpuSpeedUnitGroupName].Value;
                    if (string.IsNullOrEmpty(gpuSpeedUnit)) {
                        if (isDual) {
                            gpuSpeedUnit = kernelOutput.DualSpeedUnit;
                        }
                        else {
                            gpuSpeedUnit = kernelOutput.SpeedUnit;
                        }
                    }
                    int gpuIndex = i;
                    if (hasGpuId) {
                        string gpuText = match.Groups[NTKeyword.GpuIndexGroupName].Value;
                        if (!int.TryParse(gpuText, out gpuIndex)) {
                            gpuIndex = i;
                        }
                        else {
                            gpuIndex -= kernelOutput.GpuBaseIndex;
                            if (gpuIndex < 0) {
                                continue;
                            }
                        }
                    }
                    if (IsMapGpuIndex(context, mineContext, kernelOutput) && gpuIndex < mineContext.UseDevices.Length) {
                        gpuIndex = mineContext.UseDevices[gpuIndex];
                    }
                    if (double.TryParse(gpuSpeedText, out double gpuSpeed)) {
                        double gpuSpeedL = gpuSpeed.FromUnitSpeed(gpuSpeedUnit);
                        gpuSpeeds.SetCurrentSpeed(gpuIndex, gpuSpeedL, isDual, now);
                    }
                }
                string totalSpeedPattern = kernelOutput.TotalSpeedPattern;
                if (isDual) {
                    totalSpeedPattern = kernelOutput.DualTotalSpeedPattern;
                }
                // 求和分算力，因为有些内核在只有一张卡时不输出总算力
                double speed = isDual ? gpuSpeeds.AsEnumerable().Where(a => a.Gpu.Index != NTMinerContext.GpuAllId).Sum(a => a.DualCoinSpeed.Value)
                                     : gpuSpeeds.AsEnumerable().Where(a => a.Gpu.Index != NTMinerContext.GpuAllId).Sum(a => a.MainCoinSpeed.Value);
                if (speed > (isDual ? gpuSpeeds.CurrentSpeed(NTMinerContext.GpuAllId).DualCoinSpeed : gpuSpeeds.CurrentSpeed(NTMinerContext.GpuAllId).MainCoinSpeed).Value) {
                    gpuSpeeds.SetCurrentSpeed(NTMinerContext.GpuAllId, speed, isDual, now);
                }
            }
        }
        #endregion

        #region PicGpuAcceptShare
        private static void PicGpuAcceptShare(INTMinerContext context, IMineContext mineContext, string line, IKernelOutput kernelOutput, bool isDual) {
            string gpuAcceptSharePattern = kernelOutput.GpuAcceptShare;
            if (isDual) {
                return;
            }
            if (string.IsNullOrEmpty(gpuAcceptSharePattern)) {
                return;
            }
            Regex regex = VirtualRoot.GetRegex(gpuAcceptSharePattern);
            Match match = regex.Match(line);
            if (match.Success) {
                string gpuText = match.Groups[NTKeyword.GpuIndexGroupName].Value;
                string acceptShareText = match.Groups[NTKeyword.AcceptShareGroupName].Value;
                if (!string.IsNullOrEmpty(gpuText)) {
                    if (int.TryParse(gpuText, out int gpuIndex)) {
                        if (IsMapGpuIndex(context, mineContext, kernelOutput) && gpuIndex < mineContext.UseDevices.Length) {
                            gpuIndex = mineContext.UseDevices[gpuIndex];
                        }
                        if (!string.IsNullOrEmpty(acceptShareText)) {
                            if (int.TryParse(acceptShareText, out int acceptShare)) {
                                context.GpusSpeed.SetAcceptShare(gpuIndex, acceptShare);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region PicGpuRejectShare
        private static void PicGpuRejectShare(INTMinerContext context, IMineContext mineContext, string line, IKernelOutput kernelOutput, bool isDual) {
            string gpuRejectSharePattern = kernelOutput.GpuRejectShare;
            if (isDual) {
                return;
            }
            if (string.IsNullOrEmpty(gpuRejectSharePattern)) {
                return;
            }
            Regex regex = VirtualRoot.GetRegex(gpuRejectSharePattern);
            Match match = regex.Match(line);
            if (match.Success) {
                string gpuText = match.Groups[NTKeyword.GpuIndexGroupName].Value;
                string rejectShareText = match.Groups[NTKeyword.RejectShareGroupName].Value;
                if (!string.IsNullOrEmpty(gpuText)) {
                    if (int.TryParse(gpuText, out int gpuIndex)) {
                        if (IsMapGpuIndex(context, mineContext, kernelOutput) && gpuIndex < mineContext.UseDevices.Length) {
                            gpuIndex = mineContext.UseDevices[gpuIndex];
                        }
                        if (!string.IsNullOrEmpty(rejectShareText)) {
                            if (int.TryParse(rejectShareText, out int rejectShare)) {
                                context.GpusSpeed.SetRejectShare(gpuIndex, rejectShare);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region PicGpuIncorrectShare
        private static void PicGpuIncorrectShare(INTMinerContext context, IMineContext mineContext, string line, IKernelOutput kernelOutput, bool isDual) {
            string gpuIncorrectSharePattern = kernelOutput.GpuIncorrectShare;
            if (isDual) {
                return;
            }
            if (string.IsNullOrEmpty(gpuIncorrectSharePattern)) {
                return;
            }
            Regex regex = VirtualRoot.GetRegex(gpuIncorrectSharePattern);
            Match match = regex.Match(line);
            if (match.Success) {
                string gpuText = match.Groups[NTKeyword.GpuIndexGroupName].Value;
                string incorrectShareText = match.Groups[NTKeyword.IncorrectShareGroupName].Value;
                if (!string.IsNullOrEmpty(gpuText)) {
                    if (int.TryParse(gpuText, out int gpuIndex)) {
                        if (IsMapGpuIndex(context, mineContext, kernelOutput) && gpuIndex < mineContext.UseDevices.Length) {
                            gpuIndex = mineContext.UseDevices[gpuIndex];
                        }
                        if (!string.IsNullOrEmpty(incorrectShareText)) {
                            if (int.TryParse(incorrectShareText, out int incorrectShare)) {
                                context.GpusSpeed.SetIncorrectShare(gpuIndex, incorrectShare);
                            }
                        }
                    }
                }
            }
        }
        #endregion
        #endregion

        #region PickPoolDelay
        private static void PickPoolDelay(string line, IKernelOutput kernelOutput, bool isDual, Guid poolId) {
            string poolDelayPattern = kernelOutput.PoolDelayPattern;
            if (isDual) {
                poolDelayPattern = kernelOutput.DualPoolDelayPattern;
            }
            if (string.IsNullOrEmpty(poolDelayPattern)) {
                return;
            }
            Regex regex = VirtualRoot.GetRegex(poolDelayPattern);
            Match match = regex.Match(line);
            if (match.Success) {
                string poolDelayText = match.Groups[NTKeyword.PoolDelayGroupName].Value;
                VirtualRoot.RaiseEvent(new PoolDelayPickedEvent(poolId, isDual, poolDelayText));
            }
        }
        #endregion

        private static bool IsMapGpuIndex(INTMinerContext context, IMineContext mineContext, IKernelOutput kernelOutput) {
            return kernelOutput.IsMapGpuIndex && !string.IsNullOrWhiteSpace(mineContext.KernelInput.DevicesArg)
                && mineContext.UseDevices.Length != 0 && mineContext.UseDevices.Length != context.GpuSet.Count;
        }
        #endregion
    }
}
