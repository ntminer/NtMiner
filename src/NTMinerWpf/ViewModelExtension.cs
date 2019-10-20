using NTMiner.Views;
using NTMiner.Vms;
using System;

namespace NTMiner {
    public static class ViewModelExtension {
        public static void ShowDialog(this ViewModelBase vm, string icon = "Icon_Confirm",
            string title = null,
            string message = null,
            string helpUrl = null,
            Action onYes = null,
            Func<bool> onNo = null,
            string yesText = null,
            string noText = null) {
            DialogWindow.ShowDialog(new DialogWindowViewModel(
                icon: icon, 
                title: title, 
                message: message, 
                helpUrl: helpUrl, 
                onYes: onYes, 
                onNo: onNo, 
                yesText: yesText, 
                noText: noText));
        }
    }
}
