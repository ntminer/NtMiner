using NTMiner.Core.Gpus;
using NTMiner.Core.Gpus.Impl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NTMiner.Core.Kernels.Impl {
    public class KernelOutputSet : IKernelOutputSet {
        private readonly Dictionary<Guid, KernelOutputData> _dicById = new Dictionary<Guid, KernelOutputData>();

        private readonly INTMinerRoot _root;

        public KernelOutputSet(INTMinerRoot root) {
            _root = root;
            Global.Access<RefreshKernelOutputSetCommand>(
                Guid.Parse("1BDEC360-3F1A-4CC2-9A1A-0E5805D6EBCD"),
                "处理刷新内核输出数据集命令",
                LogEnum.Console,
                action: message => {
                    _isInited = false;
                    Init(isReInit: true);
                });
            Global.Access<AddKernelOutputCommand>(
                Guid.Parse("142AE86A-C264-40B2-A617-D65E33C7FEE2"),
                "添加内核输出组",
                LogEnum.Console,
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
                    var repository = NTMinerRoot.CreateServerRepository<KernelOutputData>();
                    repository.Add(entity);

                    Global.Happened(new KernelOutputAddedEvent(entity));
                });
            Global.Access<UpdateKernelOutputCommand>(
                Guid.Parse("2A3CAE7E-D0E2-4E4B-B75B-357EB0BE1AA1"),
                "更新内核输出组",
                LogEnum.Console,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Name)) {
                        throw new ValidationException("KernelOutput name can't be null or empty");
                    }
                    if (!_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    KernelOutputData entity = _dicById[message.Input.GetId()];
                    entity.Update(message.Input);
                    var repository = NTMinerRoot.CreateServerRepository<KernelOutputData>();
                    repository.Update(entity);

                    Global.Happened(new KernelOutputUpdatedEvent(entity));
                });
            Global.Access<RemoveKernelOutputCommand>(
                Guid.Parse("43B565B4-1509-4DC6-9FA4-55D49C79C60A"),
                "移除内核输出组",
                LogEnum.Console,
                action: (message) => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    IKernel[] outputUsers = root.KernelSet.Where(a => a.KernelOutputId == message.EntityId).ToArray();
                    if (outputUsers.Length != 0) {
                        throw new ValidationException($"这些内核在使用该内核输出组，删除前请先解除使用：{string.Join(",", outputUsers.Select(a => a.FullName))}");
                    }
                    KernelOutputData entity = _dicById[message.EntityId];
                    List<Guid> kernelOutputFilterIds = root.KernelOutputFilterSet.Where(a => a.KernelOutputId == entity.Id).Select(a => a.GetId()).ToList();
                    List<Guid> kernelOutputTranslaterIds = root.KernelOutputTranslaterSet.Where(a => a.KernelOutputId == entity.Id).Select(a => a.GetId()).ToList();
                    foreach (var kernelOutputFilterId in kernelOutputFilterIds) {
                        Global.Execute(new RemoveKernelOutputFilterCommand(kernelOutputFilterId));
                    }
                    foreach (var kernelOutputTranslaterId in kernelOutputTranslaterIds) {
                        Global.Execute(new RemoveKernelOutputTranslaterCommand(kernelOutputTranslaterId));
                    }
                    _dicById.Remove(entity.GetId());
                    var repository = NTMinerRoot.CreateServerRepository<KernelOutputData>();
                    repository.Remove(message.EntityId);

                    Global.Happened(new KernelOutputRemovedEvent(entity));
                });
            Global.Logger.InfoDebugLine(this.GetType().FullName + "接入总线");
        }

        private bool _isInited = false;
        private object _locker = new object();

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init(bool isReInit = false) {
            lock (_locker) {
                if (!_isInited) {
                    var repository = NTMinerRoot.CreateServerRepository<KernelOutputData>();
                    foreach (var item in repository.GetAll()) {
                        if (!_dicById.ContainsKey(item.GetId())) {
                            _dicById.Add(item.GetId(), item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public bool Contains(Guid id) {
            InitOnece();
            return _dicById.ContainsKey(id);
        }

        public bool TryGetKernelOutput(Guid id, out IKernelOutput kernelOutput) {
            InitOnece();
            KernelOutputData data;
            var result = _dicById.TryGetValue(id, out data);
            kernelOutput = data;
            return result;
        }

        IEnumerator IEnumerable.GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        public IEnumerator<IKernelOutput> GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        public void Pick(Guid kernelId, ref string input, IMineContext mineContext) {
            try {
                InitOnece();
                if (!_dicById.ContainsKey(kernelId)) {
                    return;
                }
                if (string.IsNullOrEmpty(input)) {
                    return;
                }
                IKernelOutput kernelOutput = _dicById[kernelId];
                ICoin coin = mineContext.MainCoin;
                bool isDual = false;
                if ((mineContext is IDualMineContext dualMineContext) && !input.Contains(mineContext.MainCoin.Code)) {
                    isDual = true;
                    coin = dualMineContext.DualCoin;
                }
                PickTotalSpeed(_root, input, kernelOutput, coin, isDual);
                PickGpuSpeed(_root, input, kernelOutput, coin, isDual);
                PickTotalShare(_root, input, kernelOutput, coin, isDual);
                PickAcceptShare(_root, input, kernelOutput, coin, isDual);
                PickAcceptOneShare(_root, input, kernelOutput, coin, isDual);
                PickRejectPattern(_root, input, kernelOutput, coin, isDual);
                PickRejectOneShare(_root, input, kernelOutput, coin, isDual);
                PickRejectPercent(_root, input, kernelOutput, coin, isDual);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
            }
        }

        #region private methods
        private static void PickTotalSpeed(INTMinerRoot root, string input, IKernelOutput kernelOutput, ICoin coin, bool isDual) {
            string totalSpeedPattern = kernelOutput.TotalSpeedPattern;
            if (isDual) {
                totalSpeedPattern = kernelOutput.DualTotalSpeedPattern;
            }
            if (string.IsNullOrEmpty(totalSpeedPattern)) {
                return;
            }
            Match match = Regex.Match(input, totalSpeedPattern);
            if (match.Success) {
                string totalSpeedText = match.Groups["totalSpeed"].Value;
                string totalSpeedUnit = match.Groups["totalSpeedUnit"].Value;

                double totalSpeed;
                if (double.TryParse(totalSpeedText, out totalSpeed)) {
                    double totalSpeedL = totalSpeed.FromUnitSpeed(totalSpeedUnit);
                    var now = DateTime.Now;
                    GpusSpeed gpuSpeeds = (GpusSpeed)NTMinerRoot.Current.GpusSpeed;
                    IGpuSpeed totalGpuSpeed = gpuSpeeds.First(a => a.Gpu.Index == NTMinerRoot.GpuAllId);
                    gpuSpeeds.SetCurrentSpeed(totalGpuSpeed.Clone());
                    if (isDual) {
                        totalGpuSpeed.DualCoinSpeed.Value = totalSpeedL;
                        totalGpuSpeed.DualCoinSpeed.SpeedOn = now;
                    }
                    else {
                        totalGpuSpeed.MainCoinSpeed.Value = totalSpeedL;
                        totalGpuSpeed.MainCoinSpeed.SpeedOn = now;
                    }
                    Global.Happened(new GpuSpeedChangedEvent(isDualSpeed: isDual, gpuSpeed: totalGpuSpeed));
                    string gpuSpeedPattern = kernelOutput.GpuSpeedPattern;
                    if (isDual) {
                        gpuSpeedPattern = kernelOutput.DualGpuSpeedPattern;
                    }
                    if (string.IsNullOrEmpty(gpuSpeedPattern)) {
                        // 平分总算力
                        double gpuSpeedL = totalSpeedL / root.GpuSet.Count;
                        foreach (var item in gpuSpeeds) {
                            if (item.Gpu.Index != NTMinerRoot.GpuAllId) {
                                gpuSpeeds.SetCurrentSpeed(item.Clone());
                                if (isDual) {
                                    item.DualCoinSpeed.Value = gpuSpeedL;
                                    item.DualCoinSpeed.SpeedOn = now;
                                }
                                else {
                                    item.MainCoinSpeed.Value = gpuSpeedL;
                                    item.MainCoinSpeed.SpeedOn = now;
                                }
                                Global.Happened(new GpuSpeedChangedEvent(isDualSpeed: isDual, gpuSpeed: item));
                            }
                        }
                    }
                }
            }
        }

        private static void PickGpuSpeed(INTMinerRoot root, string input, IKernelOutput kernelOutput, ICoin coin, bool isDual) {
            string gpuSpeedPattern = kernelOutput.GpuSpeedPattern;
            if (isDual) {
                gpuSpeedPattern = kernelOutput.DualGpuSpeedPattern;
            }
            if (string.IsNullOrEmpty(gpuSpeedPattern)) {
                return;
            }
            var now = DateTime.Now;
            bool hasGpuId = gpuSpeedPattern.Contains("?<gpu>");
            Regex regex = new Regex(gpuSpeedPattern);
            MatchCollection matches = regex.Matches(input);
            if (matches.Count > 0) {
                GpusSpeed gpuSpeeds = (GpusSpeed)NTMinerRoot.Current.GpusSpeed;
                for (int i = 0; i < matches.Count; i++) {
                    Match match = matches[i];
                    string gpuSpeedText = match.Groups["gpuSpeed"].Value;
                    string gpuSpeedUnit = match.Groups["gpuSpeedUnit"].Value;

                    int gpu = i;
                    if (hasGpuId) {
                        string gpuText = match.Groups["gpu"].Value;
                        if (!int.TryParse(gpuText, out gpu)) {
                            gpu = i;
                        }
                    }
                    double gpuSpeed;
                    if (double.TryParse(gpuSpeedText, out gpuSpeed)) {
                        double gpuSpeedL = gpuSpeed.FromUnitSpeed(gpuSpeedUnit);
                        IGpuSpeed gpuSpeedItem = gpuSpeeds.First(a => a.Gpu.Index == gpu);
                        gpuSpeeds.SetCurrentSpeed(gpuSpeedItem.Clone());
                        if (isDual) {
                            gpuSpeedItem.DualCoinSpeed.Value = gpuSpeedL;
                            gpuSpeedItem.DualCoinSpeed.SpeedOn = now;
                        }
                        else {
                            gpuSpeedItem.MainCoinSpeed.Value = gpuSpeedL;
                            gpuSpeedItem.MainCoinSpeed.SpeedOn = now;
                        }
                        Global.Happened(new GpuSpeedChangedEvent(isDualSpeed: isDual, gpuSpeed: gpuSpeedItem));
                    }
                }
                string totalSpeedPattern = kernelOutput.DualTotalSpeedPattern;
                if (isDual) {
                    totalSpeedPattern = kernelOutput.DualTotalSpeedPattern;
                }
                if (string.IsNullOrEmpty(totalSpeedPattern)) {
                    // 求和分算力
                    IGpuSpeed totalGpuSpeed = gpuSpeeds.First(a => a.Gpu.Index == NTMinerRoot.GpuAllId);
                    gpuSpeeds.SetCurrentSpeed(totalGpuSpeed.Clone());
                    if (isDual) {
                        totalGpuSpeed.DualCoinSpeed.Value = gpuSpeeds.Where(a => a.Gpu.Index != NTMinerRoot.GpuAllId).Sum(a => a.DualCoinSpeed.Value);
                        totalGpuSpeed.DualCoinSpeed.SpeedOn = now;
                    }
                    else {
                        totalGpuSpeed.MainCoinSpeed.Value = gpuSpeeds.Where(a => a.Gpu.Index != NTMinerRoot.GpuAllId).Sum(a => a.MainCoinSpeed.Value); ;
                        totalGpuSpeed.MainCoinSpeed.SpeedOn = now;
                    }
                    Global.Happened(new GpuSpeedChangedEvent(isDualSpeed: isDual, gpuSpeed: totalGpuSpeed));
                }
            }
        }

        private static void PickTotalShare(INTMinerRoot root, string input, IKernelOutput kernelOutput, ICoin coin, bool isDual) {
            string totalSharePattern = kernelOutput.TotalSharePattern;
            if (isDual) {
                totalSharePattern = kernelOutput.DualTotalSharePattern;
            }
            if (string.IsNullOrEmpty(totalSharePattern)) {
                return;
            }
            var match = Regex.Match(input, totalSharePattern);
            if (match.Success) {
                string totalShareText = match.Groups["totalShare"].Value;
                int totalShare;
                if (int.TryParse(totalShareText, out totalShare)) {
                    ICoinShare share = root.CoinShareSet.GetOrCreate(coin.GetId());
                    share.AcceptShareCount = totalShare - share.RejectCount;
                    share.ShareOn = DateTime.Now;
                    Global.Happened(new ShareChangedEvent(share));
                }
            }
        }

        private static void PickAcceptShare(INTMinerRoot root, string input, IKernelOutput kernelOutput, ICoin coin, bool isDual) {
            string acceptSharePattern = kernelOutput.AcceptSharePattern;
            if (isDual) {
                acceptSharePattern = kernelOutput.DualAcceptSharePattern;
            }
            if (string.IsNullOrEmpty(acceptSharePattern)) {
                return;
            }
            var match = Regex.Match(input, acceptSharePattern);
            if (match.Success) {
                string acceptShareText = match.Groups["acceptShare"].Value;
                int acceptShare;
                if (int.TryParse(acceptShareText, out acceptShare)) {
                    ICoinShare share = root.CoinShareSet.GetOrCreate(coin.GetId());
                    share.AcceptShareCount = acceptShare;
                    share.ShareOn = DateTime.Now;
                    Global.Happened(new ShareChangedEvent(share));
                }
            }
        }

        private static void PickAcceptOneShare(INTMinerRoot root, string input, IKernelOutput kernelOutput, ICoin coin, bool isDual) {
            string acceptOneShare = kernelOutput.AcceptOneShare;
            if (isDual) {
                acceptOneShare = kernelOutput.DualAcceptOneShare;
            }
            if (string.IsNullOrEmpty(acceptOneShare)) {
                return;
            }
            var match = Regex.Match(input, acceptOneShare);
            if (match.Success) {
                ICoinShare share = root.CoinShareSet.GetOrCreate(coin.GetId());
                share.AcceptShareCount = share.AcceptShareCount + 1;
                share.ShareOn = DateTime.Now;
                Global.Happened(new ShareChangedEvent(share));
            }
        }

        private static void PickRejectPattern(INTMinerRoot root, string input, IKernelOutput kernelOutput, ICoin coin, bool isDual) {
            string rejectSharePattern = kernelOutput.RejectSharePattern;
            if (isDual) {
                rejectSharePattern = kernelOutput.DualRejectSharePattern;
            }
            if (string.IsNullOrEmpty(rejectSharePattern)) {
                return;
            }
            var match = Regex.Match(input, rejectSharePattern);
            if (match.Success) {
                string rejectShareText = match.Groups["rejectShare"].Value;

                int rejectShare;
                if (int.TryParse(rejectShareText, out rejectShare)) {
                    ICoinShare share = root.CoinShareSet.GetOrCreate(coin.GetId());
                    share.RejectCount = rejectShare;
                    share.ShareOn = DateTime.Now;
                    Global.Happened(new ShareChangedEvent(share));
                }
            }
        }

        private static void PickRejectOneShare(INTMinerRoot root, string input, IKernelOutput kernelOutput, ICoin coin, bool isDual) {
            string rejectOneShare = kernelOutput.RejectOneShare;
            if (isDual) {
                rejectOneShare = kernelOutput.DualRejectOneShare;
            }
            if (string.IsNullOrEmpty(rejectOneShare)) {
                return;
            }
            var match = Regex.Match(input, rejectOneShare);
            if (match.Success) {
                ICoinShare share = root.CoinShareSet.GetOrCreate(coin.GetId());
                share.RejectCount = share.RejectCount + 1;
                share.ShareOn = DateTime.Now;
                Global.Happened(new ShareChangedEvent(share));
            }
        }

        private static void PickRejectPercent(INTMinerRoot root, string input, IKernelOutput kernelOutput, ICoin coin, bool isDual) {
            string rejectPercentPattern = kernelOutput.RejectPercentPattern;
            if (isDual) {
                rejectPercentPattern = kernelOutput.DualRejectPercentPattern;
            }
            if (string.IsNullOrEmpty(rejectPercentPattern)) {
                return;
            }
            var match = Regex.Match(input, rejectPercentPattern);
            string rejectPercentText = match.Groups["rejectPercent"].Value;
            double rejectPercent;
            if (double.TryParse(rejectPercentText, out rejectPercent)) {
                ICoinShare share = root.CoinShareSet.GetOrCreate(coin.GetId());
                share.RejectCount = (int)(share.TotalShareCount * rejectPercent);
                share.ShareOn = DateTime.Now;
                Global.Happened(new ShareChangedEvent(share));
            }
        }
        #endregion
    }
}
