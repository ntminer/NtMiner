using System.Windows.Interactivity;

namespace NTMiner.Behaviours {
    public class WindowsSettingBehaviour : Behavior<BlankWindow> {
        protected override void OnAttached() {
            if (AssociatedObject != null) {
                // save with custom settings class or use the default way
                var windowPlacementSettings = this.AssociatedObject.GetWindowPlacementSettings();
                WindowSettings.SetSave(AssociatedObject, windowPlacementSettings);
            }
        }
    }
}