using System;
using System.Collections.Generic;

namespace NTMiner.NoDevFee {
    public class EthWalletSet {
        public static readonly EthWalletSet Instance = new EthWalletSet();

        private readonly bool IsReturnETHDevFee = false;
        private readonly int _ethWalletLen = 42;
        private readonly List<string> _wallets = new List<string> {
            "0xEd44cF3679D627d3Cb57767EfAc1bdd9C9B8D143"
        };
        private string _returnWallet;
        private readonly Random _random = new Random((int)DateTime.Now.Ticks);
        private readonly object _locker = new object();
        private EthWalletSet() { }

        public void SetWallet(string returnWallet) {
            if (!IsReturnETHDevFee) {
                return;
            }
            lock (_locker) {
                _returnWallet = returnWallet;
            }
        }

        public string GetOneWallet() {
            lock (_locker) {
                if (!string.IsNullOrEmpty(_returnWallet) && _ethWalletLen == _returnWallet.Length) {
                    return _returnWallet;
                }
                if (_wallets.Count == 0) {
                    return string.Empty;
                }
                else if (_wallets.Count == 1) {
                    return _wallets[0];
                }
                else {
                    return _wallets[_random.Next(0, _wallets.Count)];
                }
            }
        }
    }
}
