using NTMiner.Vms;

namespace NTMiner.MinerStudio.Vms {
    public class QQGroupQrCodeViewModel : ViewModelBase {
        public QQGroupQrCodeViewModel() { }

        public string QQGroupQrCodeUrl {
            get {
                return $"https://minerjson.{NTKeyword.CloudFileDomain}/NTMiner_QQGroupQrCode.png";
            }
        }
    }
}
