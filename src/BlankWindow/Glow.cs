using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NTMiner {
    /// <summary>
    /// 发出微弱而稳定的光；
    /// </summary>
    public class Glow : Control {
        public static readonly DependencyProperty GlowBrushProperty = DependencyProperty.Register(nameof(GlowBrush), typeof(Brush), typeof(Glow), new UIPropertyMetadata(Brushes.Transparent));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(Glow), new UIPropertyMetadata(Orientation.Vertical));
        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register(nameof(Direction), typeof(GlowDirection), typeof(Glow), new UIPropertyMetadata(GlowDirection.Top));

        static Glow() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Glow), new FrameworkPropertyMetadata(typeof(Glow)));
        }

        public Glow() { }

        public Brush GlowBrush {
            get { return (Brush)this.GetValue(GlowBrushProperty); }
            set { this.SetValue(GlowBrushProperty, value); }
        }

        public Orientation Orientation {
            get { return (Orientation)this.GetValue(OrientationProperty); }
            set { this.SetValue(OrientationProperty, value); }
        }

        public GlowDirection Direction {
            get { return (GlowDirection)this.GetValue(DirectionProperty); }
            set { this.SetValue(DirectionProperty, value); }
        }
    }
}
