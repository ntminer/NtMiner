using NTMiner.Vms;

namespace NTMiner.MinerStudio.Vms {
    public class QQGroupQrCodeViewModel : ViewModelBase {
        public QQGroupQrCodeViewModel() { }

        public string QQGroupQrCodeUrl {
            get {
                return $"https://ntwebsite.{NTKeyword.CloudFileDomain}/img/NTMiner_QQGroupQrCode.png";
            }
        }
    }
}
