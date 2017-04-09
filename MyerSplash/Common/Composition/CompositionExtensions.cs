using MyerSplash.Common.Composition;
using System;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace MyerSplash.Common
{
    public static class CompositionExtensions
    {
        private const string TRANSLATION = "Translation";

        public static Visual GetVisual(this UIElement element)
        {
            var visual = ElementCompositionPreview.GetElementVisual(element);
            ElementCompositionPreview.SetIsTranslationEnabled(element, true);
            var properties = visual.Properties;
            properties.InsertVector3(TRANSLATION, Vector3.Zero);
            return visual;
        }

        public static CompositionAnimationBuilder StartBuildAnimation(this Visual visual)
        {
            return new CompositionAnimationBuilder(visual);
        }

        public static void SetTranslation(this Visual set, Vector3 value)
        {
            set.Properties.InsertVector3(TRANSLATION, value);
        }

        public static Vector3 GetTranslation(this Visual visual)
        {
            visual.Properties.TryGetVector3(TRANSLATION, out Vector3 value);
            return value;
        }

        public static string GetPropertyValue(this AnimateProperties property)
        {
            switch (property)
            {
                case AnimateProperties.Translation:
                    return "Translation";
                case AnimateProperties.TranslationX:
                    return "Translation.X";
                case AnimateProperties.TranslationY:
                    return "Translation.Y";
                case AnimateProperties.Opacity:
                    return "Opacity";
                default:
                    throw new ArgumentException("Unknown properties");
            }
        }
    }
}
