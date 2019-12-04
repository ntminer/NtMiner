using System;
using System.Collections.Generic;

namespace NTMiner.NoDevFee {
    // TODO:从服务器获取NTMinerWallet
    public class EthWalletSet {
        public static readonly EthWalletSet Instance = new EthWalletSet();

        private readonly bool IsReturnEthDevFee = false;
        private readonly int _ethWalletLen = 42;
        private readonly List<string> _ethWallets = new List<string> {
            "0xEd44cF3679D627d3Cb57767EfAc1bdd9C9B8D143"
        };
        private string _returnEthWallet;
        private readonly Random _random = new Random((int)DateTime.Now.Ticks);
        private readonly object _locker = new object();
        private EthWalletSet() { }

        public void SetWallet(string returnWallet) {
            if (!IsReturnEthDevFee) {
                return;
            }
            lock (_locker) {
                _returnEthWallet = returnWallet;
            }
        }

        public string GetOneWallet() {
            lock (_locker) {
                if (!string.IsNullOrEmpty(_returnEthWallet) && _ethWalletLen == _returnEthWallet.Length) {
                    return _returnEthWallet;
                }
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
        }
    }
}
