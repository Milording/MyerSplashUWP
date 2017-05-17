using JP.Utils.UI;
using Microsoft.Graphics.Canvas.Effects;
using Windows.UI.Composition;
using Windows.UI.Xaml;

namespace MyerSplash.Common
{
    public static class CompositionBrushUtil
    {
        public static CompositionEffectBrush BuildHostBackdropBrush(Compositor compositor)
        {
            var effect = new GaussianBlurEffect()
            {
                BlurAmount = 2f,
                BorderMode = EffectBorderMode.Soft,
                Source = new ArithmeticCompositeEffect()
                {
                    MultiplyAmount = 0,
                    Source1Amount = 0.4f,
                    Source2Amount = 0.8f,
                    Source1 = new CompositionEffectSourceParameter("source"),
                    Source2 = new ColorSourceEffect()
                    {
                        Color = "#000000".ToColor()
                    }
                }
            };

            var effectFactory = compositor.CreateEffectFactory(effect);
            var backdropBrush = compositor.CreateHostBackdropBrush();
            var effectBrush = effectFactory.CreateBrush();
            effectBrush.SetSourceParameter("source", backdropBrush);

            return effectBrush;
        }

        public static CompositionEffectBrush BuildBackdropBrush(Compositor compositor)
        {
            var effect = new GaussianBlurEffect()
            {
                BlurAmount = 2f,
                BorderMode = EffectBorderMode.Soft,
                Source = new ArithmeticCompositeEffect()
                {
                    MultiplyAmount = 0,
                    Source1Amount = 0.2f,
                    Source2Amount = 0.4f,
                    Source1 = new CompositionEffectSourceParameter("source"),
                    Source2 = new ColorSourceEffect()
                    {
                        Color = "#000000".ToColor()
                    }
                }
            };

            var effectFactory = compositor.CreateEffectFactory(effect);
            var backdropBrush = compositor.CreateBackdropBrush();
            var effectBrush = effectFactory.CreateBrush();
            effectBrush.SetSourceParameter("source", backdropBrush);

            return effectBrush;
        }

        public static void AttachToRoot(UIElement element)
        {

        }
    }
}
