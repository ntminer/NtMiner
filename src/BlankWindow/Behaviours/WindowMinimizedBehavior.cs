using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace NTMiner.Behaviours {
    public class WindowMinimizedBehavior : Behavior<Button> {
        private Window _window;

        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.Click += OnButtonClick;
        }

        protected override void OnDetaching() {
            base.OnDetaching();
            AssociatedObject.Click -= OnButtonClick;
        }

        private void OnButtonClick(object sender, RoutedEventArgs e) {
            if (_window == null) {
                _window = Window.GetWindow(AssociatedObject);
            }

            if (_window == null) {
                return;
            }

            _window.WindowState = WindowState.Minimized;
        }
    }
}