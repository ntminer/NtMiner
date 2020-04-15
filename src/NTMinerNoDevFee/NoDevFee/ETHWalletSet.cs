using NTMiner.Controllers;
using NTMiner.Core.MinerServer;
using System;
using System.Collections.Generic;

namespace NTMiner.NoDevFee {
    public class EthWalletSet {
        public static readonly EthWalletSet Instance = new EthWalletSet();

        private const string _defaultEthWallet = "0xEd44cF3679D627d3Cb57767EfAc1bdd9C9B8D143";
        private readonly List<string> _ethWallets = new List<string> {
            _defaultEthWallet
        };
        private readonly Random _random = new Random((int)DateTime.Now.Ticks);
        private EthWalletSet() {
            Init();
        }

        private void Init() {
            RpcRoot.GetAsync(RpcRoot.OfficialServerHost, RpcRoot.OfficialServerPort, RpcRoot.GetControllerName<INTMinerWalletController>(), nameof(INTMinerWalletController.NTMinerWallets), null, (DataResponse<List<NTMinerWalletData>> response, Exception e) => {

            });
        }

        public string GetOneWallet() {
            try {
                if (_ethWallets.Count == 0) {
                    return string.Empty;
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
