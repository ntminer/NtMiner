using System;
using System.Windows.Media.Imaging;

namespace NTMiner {
    public static class IconConst {
        public static readonly BitmapImage BigLogoImageSource = new BitmapImage(new Uri((VirtualRoot.IsMinerStudio ? "/NTMinerWpf;component/Styles/Images/cc128.png" : "/NTMinerWpf;component/Styles/Images/logo128.png"), UriKind.RelativeOrAbsolute));
        public const string IconConfirm = "Icon_Confirm";
    }
}
