using NTMiner.Profile;
using NTMiner.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NTMiner.Core.Profiles {
    internal class MinerProfile : IMinerProfile {
        private readonly INTMinerRoot _root;

        private MinerProfileData _data;
        private readonly Guid _workId;
        private MinerProfileData GetMinerProfileData() {
            if (_workId != Guid.Empty) {
                return Server.ProfileService.GetMinerProfile(_workId);
            }
            else {
                IRepository<MinerProfileData> repository = NTMinerRoot.CreateLocalRepository<MinerProfileData>();
                var result = repository.GetAll().FirstOrDefault();
                if (result == null) {
                    result = MinerProfileData.CreateDefaultData();
                }
                else if (result.IsAutoThisPCName) {
                    result.MinerName = GetThisPcName();
                }
                return result;
            }
        }

        public MinerProfile(INTMinerRoot root, Guid workId) {
            _root = root;
            _workId = workId;
            _data = GetMinerProfileData();
            if (_data == null) {
                throw new ValidationException("未获取到MinerProfileData数据，请重试");
            }
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id {
            get { return _data.Id; }
            private set {
                _data.Id = value;
            }
        }

        public string MinerName {
            get => _data.MinerName;
            private set {
                if (string.IsNullOrEmpty(value)) {
                    value = GetThisPcName();
                }
                value = new string(value.ToCharArray().Where(a => !invalidChars.Contains(a)).ToArray());
                if (_data.MinerName != value) {
                    _data.MinerName = value;
                    VirtualRoot.Execute(new RefreshArgsAssemblyCommand());
                }
            }
        }

        private static readonly char[] invalidChars = { '.', ' ', '-', '_' };
        public string GetThisPcName() {
            string value = Environment.MachineName.ToLower();
            value = new string(value.ToCharArray().Where(a => !invalidChars.Contains(a)).ToArray());
            return value;
        }

        public bool IsAutoThisPCName {
            get { return _data.IsAutoThisPCName; }
            private set {
                if (_data.IsAutoThisPCName != value) {
                    _data.IsAutoThisPCName = value;
                    if (value) {
                        this.MinerName = string.Empty;
                    }
                }
            }
        }

        public bool IsShowInTaskbar {
            get { return _data.IsShowInTaskbar; }
            private set {
                if (_data.IsShowInTaskbar != value) {
                    _data.IsShowInTaskbar = value;
                }
            }
        }

        public bool IsAutoBoot {
            get => _data.IsAutoBoot;
            private set {
                if (_data.IsAutoBoot != value) {
                    _data.IsAutoBoot = value;
                    NTMinerRegistry.SetIsAutoBoot(value);
                }
            }
        }

        public bool IsNoShareRestartKernel {
            get => _data.IsNoShareRestartKernel;
            private set {
                if (_data.IsNoShareRestartKernel != value) {
                    _data.IsNoShareRestartKernel = value;
                }
            }
        }
        public int NoShareRestartKernelMinutes {
            get => _data.NoShareRestartKernelMinutes;
            private set {
                if (_data.NoShareRestartKernelMinutes != value) {
                    _data.NoShareRestartKernelMinutes = value;
                }
            }
        }
        public bool IsPeriodicRestartKernel {
            get => _data.IsPeriodicRestartKernel;
            private set {
                if (_data.IsPeriodicRestartKernel != value) {
                    _data.IsPeriodicRestartKernel = value;
                }
            }
        }
        public int PeriodicRestartKernelHours {
            get => _data.PeriodicRestartKernelHours;
            private set {
                if (_data.PeriodicRestartKernelHours != value) {
                    _data.PeriodicRestartKernelHours = value;
                }
            }
        }
        public bool IsPeriodicRestartComputer {
            get => _data.IsPeriodicRestartComputer;
            private set {
                if (_data.IsPeriodicRestartComputer != value) {
                    _data.IsPeriodicRestartComputer = value;
                }
            }
        }
        public int PeriodicRestartComputerHours {
            get => _data.PeriodicRestartComputerHours;
            private set {
                if (_data.PeriodicRestartComputerHours != value) {
                    _data.PeriodicRestartComputerHours = value;
                }
            }
        }

        public bool IsAutoStart {
            get => _data.IsAutoStart;
            private set {
                if (_data.IsAutoStart != value) {
                    _data.IsAutoStart = value;
                }
            }
        }

        public bool IsAutoRestartKernel {
            get {
                return _data.IsAutoRestartKernel;
            }
            private set {
                if (_data.IsAutoRestartKernel != value) {
                    _data.IsAutoRestartKernel = value;
                }
            }
        }

        public bool IsShowCommandLine {
            get { return _data.IsShowCommandLine; }
            set {
                _data.IsShowCommandLine = value;
            }
        }

        public Guid CoinId {
            get => _data.CoinId;
            private set {
                if (_data.CoinId != value) {
                    _data.CoinId = value;
                }
            }
        }

        private static Dictionary<string, PropertyInfo> _properties;
        private static Dictionary<string, PropertyInfo> Properties {
            get {
                if (_properties == null) {
                    _properties = new Dictionary<string, PropertyInfo>();
                    foreach (var item in typeof(MinerProfile).GetProperties()) {
                        _properties.Add(item.Name, item);
                    }
                }
                return _properties;
            }
        }

        public void SetValue(string propertyName, object value) {
            if (Properties.TryGetValue(propertyName, out PropertyInfo propertyInfo)) {
                if (propertyInfo.CanWrite) {
                    if (propertyInfo.PropertyType == typeof(Guid)) {
                        value = DictionaryExtensions.ConvertToGuid(value);
                    }
                    propertyInfo.SetValue(this, value, null);
                    if (_workId != Guid.Empty) {
                        if (CommandLineArgs.IsControlCenter) {
                            Server.ControlCenterService.SetMinerProfilePropertyAsync(_workId, propertyName, value, isSuccess => {
                                VirtualRoot.Happened(new MinerProfilePropertyChangedEvent(propertyName));
                            });
                        }
                    }
                    else {
                        IRepository<MinerProfileData> repository = NTMinerRoot.CreateLocalRepository<MinerProfileData>();
                        repository.Update(_data);
                        VirtualRoot.Happened(new MinerProfilePropertyChangedEvent(propertyName));
                    }
                }
            }
        }

        public object GetValue(string propertyName) {
            if (Properties.TryGetValue(propertyName, out PropertyInfo propertyInfo)) {
                if (propertyInfo.CanRead) {
                    return propertyInfo.GetValue(this, null);
                }
            }
            return null;
        }
    }
}
