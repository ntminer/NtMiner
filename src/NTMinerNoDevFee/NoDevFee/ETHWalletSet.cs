using NTMiner.Controllers;
using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NTMiner.NoDevFee {
    public class EthWalletSet {
        public static EthWalletSet Instance { get; private set; } = new EthWalletSet();

        private const string _defaultEthWallet = "0xEd44cF3679D627d3Cb57767EfAc1bdd9C9B8D143";
        private readonly List<string> _ethWallets = new List<string> {
            _defaultEthWallet
        };
        private readonly Random _random = new Random((int)DateTime.Now.Ticks);
        private EthWalletSet() {
            Init();
            VirtualRoot.BuildEventPath<Per24HourEvent>("刷新EthWalletSet列表", LogEnum.DevConsole, this.GetType(), PathPriority.Normal, path: message => {
                Init();
            });
        }

        private void Init() {
            RpcRoot.JsonRpc.GetAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, ControllerUtil.GetControllerName<INTMinerWalletController>(), nameof(INTMinerWalletController.NTMinerWallets), null, (DataResponse<List<NTMinerWalletData>> response, Exception e) => {
                if (response.IsSuccess() && response.Data != null && response.Data.Count != 0) {
                    var ethWallets = response.Data.Where(a => "ETH".Equals(a.CoinCode, StringComparison.OrdinalIgnoreCase)).ToArray();
                    if (ethWallets.Length != 0) {
                        _ethWallets.Clear();
                        _ethWallets.AddRange(ethWallets.Select(a => a.Wallet));
                    }
                }
            });
        }

        public string GetOneWallet() {
            try {
                if (_ethWallets.Count == 0) {
                    return _defaultEthWallet;
                }
                else if (_ethWallets.Count == 1) {
                    return _ethWallets[0];
                }
                else {
                    return _ethWallets[_random.Next(0, _ethWallets.Count)];
                }
            }
            catch {
                return _defaultEthWallet;
            }
        }
    }
}
