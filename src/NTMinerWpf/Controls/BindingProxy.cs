using System.Windows;

namespace NTMiner.Controls {
    public class BindingProxy : Freezable {
        protected override Freezable CreateInstanceCore() {
            return new BindingProxy();
        }

        /// <summary>
        /// {Binding}是View的ViewModel
        /// </summary>
        public object Data {
            get { return (object)GetValue(DataProperty); }
            set { SetValue(DataProperty, value); }
        }

        public static readonly DependencyProperty DataProperty = DependencyProperty.Register(nameof(Data), typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));
    }
}
