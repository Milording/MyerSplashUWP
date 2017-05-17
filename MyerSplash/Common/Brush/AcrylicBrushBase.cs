using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace MyerSplash.Common.Brush
{
    public abstract class AcrylicBrushBase : XamlCompositionBrushBase
    {
        protected Compositor _compositor;
        protected CompositionEffectBrush _brush;

        public Color TintColor
        {
            get { return (Color)GetValue(TintColorProperty); }
            set { SetValue(TintColorProperty, value); }
        }

        public static readonly DependencyProperty TintColorProperty =
            DependencyProperty.Register("TintColor", typeof(Color), typeof(AcrylicBrushBase),
                new PropertyMetadata(null));

        public float BackdropFactor
        {
            get { return (float)GetValue(BackdropFactorProperty); }
            set { SetValue(BackdropFactorProperty, value); }
        }

        public static readonly DependencyProperty BackdropFactorProperty =
            DependencyProperty.Register("BackdropFactor", typeof(float), typeof(AcrylicBrushBase), new PropertyMetadata(0.5f));

        public float TintColorFactor
        {
            get { return (float)GetValue(TintColorFactorProperty); }
            set { SetValue(TintColorFactorProperty, value); }
        }

        public static readonly DependencyProperty TintColorFactorProperty =
            DependencyProperty.Register("TintColorFactor", typeof(float), typeof(AcrylicBrushBase), new PropertyMetadata(0.5f));

        public float BlurAmount
        {
            get { return (float)GetValue(BlurAmountProperty); }
            set { SetValue(BlurAmountProperty, value); }
        }

        public static readonly DependencyProperty BlurAmountProperty =
            DependencyProperty.Register("BlurAmount", typeof(float), typeof(AcrylicBrushBase), new PropertyMetadata(2f));

        public AcrylicBrushBase()
        {
        }

        protected abstract BackdropBrushType GetBrushType();

        protected override void OnConnected()
        {
            base.OnConnected();
            _compositor = Window.Current.Content.GetVisual().Compositor;
            _brush = new CompositionBrushBuilder(GetBrushType()).SetTintColor(TintColor)
                .SetBackdropFactor(BackdropFactor)
                .SetTintColorFactor(TintColorFactor)
                .SetBlurAmount(BlurAmount)
                .Build(_compositor);
            CompositionBrush = _brush;
        }

        protected override void OnDisconnected()
        {
            base.OnDisconnected();
            _brush.Dispose();
            _compositor.Dispose();
        }
    }
}
