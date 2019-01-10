using NTMiner.Core.Gpus;
using NTMiner.Core.Gpus.Impl;
using NTMiner.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NTMiner.Core.Kernels.Impl {
    internal class KernelSet : IKernelSet {
        private readonly INTMinerRoot _root;
        private readonly Dictionary<Guid, KernelData> _dicById = new Dictionary<Guid, KernelData>();

        public KernelSet(INTMinerRoot root) {
            _root = root;
            Global.Access<AddKernelCommand>(
                Guid.Parse("331be370-2d4f-488f-9dd8-3709e3ff63af"),
                "添加内核",
                LogEnum.Log,
                action: message => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Code)) {
                        throw new ValidationException("package code can't be null or empty");
                    }
                    if (_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    KernelData entity = new KernelData().Update(message.Input);
                    _dicById.Add(entity.Id, entity);
                    IRepository<KernelData> repository = NTMinerRoot.CreateServerRepository<KernelData>();
                    repository.Add(entity);

                    Global.Happened(new KernelAddedEvent(entity));
                });
            Global.Access<UpdateKernelCommand>(
                Guid.Parse("f23c801a-afbe-4e59-93c2-3eaecf3c7d8e"),
                "更新内核",
                LogEnum.Log,
                action: message => {
                    InitOnece();
                    if (message == null || message.Input == null || message.Input.GetId() == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (string.IsNullOrEmpty(message.Input.Code)) {
                        throw new ValidationException("package code can't be null or empty");
                    }
                    if (!_dicById.ContainsKey(message.Input.GetId())) {
                        return;
                    }
                    KernelData entity = _dicById[message.Input.GetId()];
                    entity.Update(message.Input);
                    IRepository<KernelData> repository = NTMinerRoot.CreateServerRepository<KernelData>();
                    repository.Update(entity);

                    Global.Happened(new KernelUpdatedEvent(entity));
                });
            Global.Access<RemoveKernelCommand>(
                Guid.Parse("b90d68ba-2af2-48db-8bf3-5b2795667e8c"),
                "移除内核",
                LogEnum.Log,
                action: message => {
                    InitOnece();
                    if (message == null || message.EntityId == Guid.Empty) {
                        throw new ArgumentNullException();
                    }
                    if (!_dicById.ContainsKey(message.EntityId)) {
                        return;
                    }
                    KernelData entity = _dicById[message.EntityId];
                    List<Guid> coinKernelIds = root.CoinKernelSet.Where(a => a.KernelId == entity.Id).Select(a => a.GetId()).ToList();
                    List<Guid> kernelOutputFilterIds = root.KernelOutputFilterSet.Where(a => a.KernelId == entity.Id).Select(a => a.GetId()).ToList();
                    List<Guid> kernelOutputTranslaterIds = root.KernelOutputTranslaterSet.Where(a => a.KernelId == entity.Id).Select(a => a.GetId()).ToList();
                    foreach (var coinKernelId in coinKernelIds) {
                        Global.Execute(new RemoveCoinKernelCommand(coinKernelId));
                    }
                    foreach (var kernelOutputFilterId in kernelOutputFilterIds) {
                        Global.Execute(new RemoveKernelOutputFilterCommand(kernelOutputFilterId));
                    }
                    foreach (var kernelOutputTranslaterId in kernelOutputTranslaterIds) {
                        Global.Execute(new RemoveKernelOutputTranslaterCommand(kernelOutputTranslaterId));
                    }
                    _dicById.Remove(entity.Id);
                    IRepository<KernelData> repository = NTMinerRoot.CreateServerRepository<KernelData>();
                    repository.Remove(entity.Id);

                    Global.Happened(new KernelRemovedEvent(entity));
                });
            Global.Logger.Debug(this.GetType().FullName + "接入总线");
        }

        private bool _isInited = false;
        private object _locker = new object();

        public int Count {
            get {
                InitOnece();
                return _dicById.Count;
            }
        }

        private void InitOnece() {
            if (_isInited) {
                return;
            }
            Init();
        }

        private void Init() {
            lock (_locker) {
                if (!_isInited) {
                    IRepository<KernelData> repository = NTMinerRoot.CreateServerRepository<KernelData>();
                    foreach (var item in repository.GetAll()) {
                        if (!_dicById.ContainsKey(item.GetId())) {
                            _dicById.Add(item.GetId(), item);
                        }
                    }
                    _isInited = true;
                }
            }
        }

        public bool Contains(Guid packageId) {
            InitOnece();
            return _dicById.ContainsKey(packageId);
        }

        public bool TryGetKernel(Guid packageId, out IKernel package) {
            InitOnece();
            KernelData pkg;
            var r = _dicById.TryGetValue(packageId, out pkg);
            package = pkg;
            return r;
        }

        public IEnumerator<IKernel> GetEnumerator() {
            InitOnece();
            return _dicById.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
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
                IKernel kernel = _dicById[kernelId];
                if ("Claymore".Equals(kernel.Code, StringComparison.OrdinalIgnoreCase)) {
                    if (mineContext.MainCoin.Code != "ETH" && input.Contains("ETH")) {
                        input = input.Replace("ETH", mineContext.MainCoin.Code);
                    }
                }
                ICoin coin = mineContext.MainCoin;
                bool isDual = false;
                if ((mineContext is IDualMineContext dualMineContext) && !input.Contains(mineContext.MainCoin.Code)) {
                    isDual = true;
                    coin = dualMineContext.DualCoin;
                }
                PickTotalSpeed(_root, input, kernel, coin, isDual);
                PickGpuSpeed(_root, input, kernel, coin, isDual);
                PickTotalShare(_root, input, kernel, coin, isDual);
                PickAcceptShare(_root, input, kernel, coin, isDual);
                PickRejectPattern(_root, input, kernel, coin, isDual);
                PickRejectPercent(_root, input, kernel, coin, isDual);
            }
            catch (Exception e) {
                Global.Logger.ErrorDebugLine(e.Message, e);
            }
        }

        #region private methods
        private static void PickTotalSpeed(INTMinerRoot root, string input, IKernel kernel, ICoin coin, bool isDual) {
            string totalSpeedPattern = kernel.TotalSpeedPattern;
            if (isDual) {
                totalSpeedPattern = kernel.DualTotalSpeedPattern;
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
                    long totalSpeedL = totalSpeed.ToLong(totalSpeedUnit);
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
                    string gpuSpeedPattern = kernel.GpuSpeedPattern;
                    if (isDual) {
                        gpuSpeedPattern = kernel.DualGpuSpeedPattern;
                    }
                    if (string.IsNullOrEmpty(gpuSpeedPattern)) {
                        // 平分总算力
                        long gpuSpeedL = totalSpeedL / root.GpuSet.Count;
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

        private static void PickGpuSpeed(INTMinerRoot root, string input, IKernel kernel, ICoin coin, bool isDual) {
            string gpuSpeedPattern = kernel.GpuSpeedPattern;
            if (isDual) {
                gpuSpeedPattern = kernel.DualGpuSpeedPattern;
            }
            if (string.IsNullOrEmpty(gpuSpeedPattern)) {
                return;
            }
            var now = DateTime.Now;
            Regex regex = new Regex(gpuSpeedPattern);
            MatchCollection matches = regex.Matches(input);
            if (matches.Count > 0) {
                GpusSpeed gpuSpeeds = (GpusSpeed)NTMinerRoot.Current.GpusSpeed;

                foreach (Match match in matches) {
                    string gpuText = match.Groups["gpu"].Value;
                    string gpuSpeedText = match.Groups["gpuSpeed"].Value;
                    string gpuSpeedUnit = match.Groups["gpuSpeedUnit"].Value;

                    int gpu;
                    double gpuSpeed;
                    if (int.TryParse(gpuText, out gpu) && double.TryParse(gpuSpeedText, out gpuSpeed)) {
                        long gpuSpeedL = gpuSpeed.ToLong(gpuSpeedUnit);
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
                string totalSpeedPattern = kernel.DualTotalSpeedPattern;
                if (isDual) {
                    totalSpeedPattern = kernel.DualTotalSpeedPattern;
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

        private static void PickTotalShare(INTMinerRoot root, string input, IKernel kernel, ICoin coin, bool isDual) {
            string totalSharePattern = kernel.TotalSharePattern;
            if (isDual) {
                totalSharePattern = kernel.DualTotalSharePattern;
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

        private static void PickAcceptShare(INTMinerRoot root, string input, IKernel kernel, ICoin coin, bool isDual) {
            string acceptSharePattern = kernel.AcceptSharePattern;
            if (isDual) {
                acceptSharePattern = kernel.DualAcceptSharePattern;
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

        private static void PickRejectPattern(INTMinerRoot root, string input, IKernel kernel, ICoin coin, bool isDual) {
            string rejectSharePattern = kernel.RejectSharePattern;
            if (isDual) {
                rejectSharePattern = kernel.DualRejectSharePattern;
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

        private static void PickRejectPercent(INTMinerRoot root, string input, IKernel kernel, ICoin coin, bool isDual) {
            string rejectPercentPattern = kernel.RejectPercentPattern;
            if (isDual) {
                rejectPercentPattern = kernel.DualRejectPercentPattern;
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
