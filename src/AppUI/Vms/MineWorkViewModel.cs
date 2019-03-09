using NTMiner.Core;
using NTMiner.Core.Impl;
using NTMiner.MinerServer;
using NTMiner.Profile;
using NTMiner.Views;
using NTMiner.Views.Ucs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace NTMiner.Vms {
    public class MineWorkViewModel : ViewModelBase, IMineWork, IEditableViewModel {
        public static readonly MineWorkViewModel PleaseSelect = new MineWorkViewModel(Guid.Empty) {
            _name = "请选择"
        };
        public static readonly MineWorkViewModel FreeMineWork = new MineWorkViewModel(Guid.Empty) {
            _name = "自由作业"
        };

        private Guid _id;
        private string _name;
        private string _description;

        public string Sha1 { get; private set; }

        public ICommand Remove { get; private set; }
        public ICommand Edit { get; private set; }
        public ICommand Save { get; private set; }

        public Action CloseWindow { get; set; }

        public MineWorkViewModel() {
            if (!Design.IsInDesignMode) {
                throw new InvalidProgramException();
            }
        }

        public MineWorkViewModel(IMineWork mineWork) : this(mineWork.GetId()) {
            _name = mineWork.Name;
            _description = mineWork.Description;
        }

        public MineWorkViewModel(MineWorkViewModel vm) : this((IMineWork)vm) {
            Sha1 = vm.Sha1;
        }

        public MineWorkViewModel(Guid id) {
            _id = id;
            this.Save = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                if (string.IsNullOrEmpty(this.Name)) {
                    throw new ValidationException("作业名称是必须的");
                }
                bool isMinerProfileChanged = false;
                IMineWork entity;
                if (NTMinerRoot.Current.MineWorkSet.TryGetMineWork(this.Id, out entity)) {
                    string sha1 = NTMinerRoot.Current.MinerProfile.GetSha1();
                    if (this.Sha1 != sha1) {
                        isMinerProfileChanged = true;
                    }
                    if (entity.Name != this.Name || entity.Description != this.Description) {
                        VirtualRoot.Execute(new UpdateMineWorkCommand(this));
                    }
                    CloseWindow?.Invoke();
                }
                else {
                    isMinerProfileChanged = true;
                    VirtualRoot.Execute(new AddMineWorkCommand(this));
                    CloseWindow?.Invoke();
                    this.Edit.Execute(FormType.Edit);
                }
                if (isMinerProfileChanged) {
                    Write.DevLine("检测到MinerProfile状态变更");
                    string localJson = ExportMineWork();
                }
            });
            this.Edit = new DelegateCommand<FormType?>((formType) => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                MineWorkViewModel mineWorkVm;
                if (!MineWorkViewModels.Current.TryGetMineWorkVm(this.Id, out mineWorkVm)) {
                    MineWorkAdd.ShowWindow(formType ?? FormType.Edit, new MineWorkViewModel(this));
                }
                else {
                    // 编辑作业前切换上下文
                    VirtualRoot.Execute(new SwichMinerProfileCommand(this.Id));
                    this.Sha1 = NTMinerRoot.Current.MinerProfile.GetSha1();
                    MineWorkEdit.ShowWindow(formType ?? FormType.Edit, new MineWorkViewModel(this));
                }
            });
            this.Remove = new DelegateCommand(() => {
                if (this.Id == Guid.Empty) {
                    return;
                }
                DialogWindow.ShowDialog(message: $"您确定删除吗？", title: "确认", onYes: () => {
                    VirtualRoot.Execute(new RemoveMineWorkCommand(this.Id));
                }, icon: "Icon_Confirm");
            });
        }

        private string ExportMineWork() {
            try {
                LocalJson localJsonObj = LocalJson.NewInstance();
                var minerProfile = NTMinerRoot.Current.MinerProfile;
                localJsonObj.MinerProfile = new MinerProfileData(minerProfile);
                localJsonObj.MineWork = new MineWorkData(this);
                CoinProfileData mainCoinProfile = new CoinProfileData(minerProfile.GetCoinProfile(localJsonObj.MinerProfile.CoinId));
                List<CoinProfileData> coinProfiles = new List<CoinProfileData> { mainCoinProfile };
                List<PoolProfileData> poolProfiles = new List<PoolProfileData>();
                CoinKernelProfileData coinKernelProfile = new CoinKernelProfileData(minerProfile.GetCoinKernelProfile(mainCoinProfile.CoinKernelId));
                PoolProfileData mainCoinPoolProfile = new PoolProfileData(minerProfile.GetPoolProfile(mainCoinProfile.PoolId));
                poolProfiles.Add(mainCoinPoolProfile);
                if (coinKernelProfile.IsDualCoinEnabled) {
                    CoinProfileData dualCoinProfile = new CoinProfileData(minerProfile.GetCoinProfile(coinKernelProfile.DualCoinId));
                    coinProfiles.Add(dualCoinProfile);
                    PoolProfileData dualCoinPoolProfile = new PoolProfileData(minerProfile.GetPoolProfile(dualCoinProfile.PoolId));
                    poolProfiles.Add(dualCoinPoolProfile);
                }
                localJsonObj.CoinProfiles = coinProfiles.ToArray();
                localJsonObj.CoinKernelProfiles = new CoinKernelProfileData[] { coinKernelProfile };
                localJsonObj.PoolProfiles = poolProfiles.ToArray();
                localJsonObj.GpuProfiles = new GpuProfileData[] { new GpuProfileData(minerProfile.GetGpuProfile(localJsonObj.MinerProfile.CoinId, NTMinerRoot.GpuAllId)) };
                localJsonObj.TimeStamp = Timestamp.GetTimestamp();
                localJsonObj.Pools = NTMinerRoot.Current.PoolSet.Where(a => poolProfiles.Any(b => b.PoolId == a.GetId())).Select(a=>new PoolData(a)).ToArray();
                localJsonObj.Users = minerProfile.GetUsers().Select(a => new UserData(a)).ToArray();
                localJsonObj.Wallets = minerProfile.GetWallets().Select(a => new WalletData(a)).ToArray();
                foreach (var user in localJsonObj.Users) {
                    user.Password = HashUtil.Sha1(user.Password);
                }
                string json = VirtualRoot.JsonSerializer.Serialize(localJsonObj);

                return json;
            }
            catch (Exception e) {
                Logger.ErrorDebugLine(e.Message, e);
                return string.Empty;
            }
        }

        public Guid GetId() {
            return this.Id;
        }

        public Guid Id {
            get => _id;
            set {
                if (_id != value) {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public string Name {
            get { return _name; }
            set {
                if (_name != value) {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                    if (this == PleaseSelect) {
                        return;
                    }
                    if (this == FreeMineWork) {
                        return;
                    }
                    if (string.IsNullOrEmpty(value)) {
                        throw new ValidationException("名称是必须的");
                    }
                    if (MineWorkViewModels.Current.List.Any(a=>a.Name == value && a.Id != this.Id)) {
                        throw new ValidationException("名称重复");
                    }
                }
            }
        }

        public MinerProfileViewModel MinerProfile {
            get {
                return MinerProfileViewModel.Current;
            }
        }

        public string Description {
            get => _description;
            set {
                if (_description != value) {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }
    }
}
