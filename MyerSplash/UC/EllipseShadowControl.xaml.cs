using JP.Utils.UI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyerSplash.UC
{
    public sealed partial class EllipseShadowControl : UserControl
    {
        public int Radius
        {
            get { return (int)GetValue(RadiusProperty); }
            set { SetValue(RadiusProperty, value); }
        }

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register("Radius", typeof(int), typeof(EllipseShadowControl),
                new PropertyMetadata(0, (s, e) =>
                 {
                     var control = s as EllipseShadowControl;
                     control.CanvasControl.Invalidate();
                 }));

        public EllipseShadowControl()
        {
            this.InitializeComponent();
        }

        private void CanvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var radius = Radius;
            var center = new Vector2((float)sender.Size.Width / 2f, (float)sender.Size.Height / 2f);

            using (var renderTarget = new CanvasRenderTarget(sender, sender.Size))
            {
                using (var ds = renderTarget.CreateDrawingSession())
                {
                    ds.FillEllipse(center, radius, radius, new CanvasSolidColorBrush(sender, Colors.Black));
                }
                using (var effect = new ShadowEffect())
                {
                    effect.Source = renderTarget;
                    effect.ShadowColor = "#5C000000".ToColor();
                    effect.BlurAmount = 2f;

                    using (args.DrawingSession)
                    {
                        args.DrawingSession.DrawImage(effect, 1, 1);
                    }
                }
            }
        }
    }
}
