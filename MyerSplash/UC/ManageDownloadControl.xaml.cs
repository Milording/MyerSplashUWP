using MyerSplash.Common;
using MyerSplash.ViewModel;
using System.Numerics;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Hosting;

namespace MyerSplash.UC
{
    public sealed partial class ManageDownloadControl : NavigableUserControl
    {
        public DownloadsViewModel DownloadsVM
        {
            get
            {
                return App.VMLocator.DownloadsVM;
            }
        }

        private Compositor _compositor;
        private SpriteVisual _blurVisual;

        public ManageDownloadControl()
        {
            this.InitializeComponent();
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

        public void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            App.MainVM.ShowDownloadsUC = false;
        }

        public override void OnShow()
        {
            base.OnShow();
            Window.Current.SetTitleBar(TitleBar);
        }
    }
}
