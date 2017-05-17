using MyerSplash.ViewModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.ViewManagement;
using System;
using Windows.UI.Composition;
using MyerSplash.Common;
using System.Numerics;
using Windows.UI.Xaml.Hosting;

namespace MyerSplash.UC
{
    public sealed partial class DrawerControl : UserControl
    {
        private MainViewModel MainVM
        {
            get
            {
                return this.DataContext as MainViewModel;
            }
        }

        private Compositor _compositor;
        private SpriteVisual _blurVisual;

        public DrawerControl()
        {
            this.InitializeComponent();

            FullscreenBtn.Visibility = Visibility.Visible;
            Window.Current.SizeChanged += Current_SizeChanged;
            _compositor = this.GetVisual().Compositor;

            InitBlur();
        }

        private void BlurBackground_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_blurVisual != null)
            {
                _blurVisual.Size = new Vector2((float)e.NewSize.Width, (float)e.NewSize.Height);
            }
        }

        private void InitBlur()
        {
            var effectBrush = CompositionBrushUtil.BuildBackdropBrush(_compositor);

            _blurVisual = _compositor.CreateSpriteVisual();
            _blurVisual.Brush = effectBrush;
            _blurVisual.Size = new Vector2((float)BlurBackground.ActualWidth, (float)BlurBackground.ActualHeight);

            ElementCompositionPreview.SetElementChildVisual(BlurBackground, _blurVisual);
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            if (!ApplicationView.GetForCurrentView().IsFullScreenMode)
            {
                FullscreenIcon.Symbol = Symbol.FullScreen;
            }
            else
            {
                FullscreenIcon.Symbol = Symbol.BackToWindow;
            }
        }
    }
}
