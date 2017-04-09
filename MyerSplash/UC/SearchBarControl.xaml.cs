using MyerSplash.ViewModel;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Composition;
using System.Numerics;
using Windows.UI.Xaml.Data;
using Windows.ApplicationModel;
using MyerSplashShared.Utils;
using MyerSplash.Common;

namespace MyerSplash.UC
{
    public sealed partial class SearchBarControl : UserControl
    {
        private MainViewModel MainVM
        {
            get
            {
                return this.DataContext as MainViewModel;
            }
        }

        public bool Shown
        {
            get { return (bool)GetValue(ShownProperty); }
            set { SetValue(ShownProperty, value); }
        }

        public static readonly DependencyProperty ShownProperty =
            DependencyProperty.Register("Shown", typeof(bool), typeof(SearchBarControl),
                new PropertyMetadata(false, OnShownPropertyChanged));

        private static void OnShownPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as SearchBarControl;
            control.ToggleAnimation();
        }

        private Compositor _compositor;
        private Visual _maskVisual;
        private Visual _contentVisual;

        public SearchBarControl()
        {
            this.InitializeComponent();
            if (!DesignMode.DesignModeEnabled)
            {
                InitComposition();
                InitBinding();
            }
        }

        private void InitBinding()
        {
            var b = new Binding()
            {
                Source = MainVM,
                Path = new PropertyPath("ShowSearchBar"),
                Mode = BindingMode.TwoWay,
            };
            SetBinding(ShownProperty, b);
        }

        private void InitComposition()
        {
            this.Visibility = Visibility.Collapsed;

            _compositor = this.GetVisual().Compositor;
            _maskVisual = MaskBorder.GetVisual();
            _contentVisual = ContentGrid.GetVisual();

            _contentVisual.Opacity = _maskVisual.Opacity = 0f;
        }

        private async void ToggleAnimation()
        {
            this.Visibility = Visibility.Visible;

            if (Shown)
            {
                InputTB.Focus(FocusState.Programmatic);
            }

            await ContentGrid.WaitForNonZeroSizeAsync();
            _contentVisual.CenterPoint = new Vector3((float)(ContentGrid.ActualWidth / 2f), (float)(ContentGrid.ActualHeight / 2f), 1f);

            var maskAnimation = _compositor.CreateScalarKeyFrameAnimation();
            maskAnimation.InsertKeyFrame(1f, Shown ? 1f : 0f);
            maskAnimation.Duration = TimeSpan.FromMilliseconds(400);

            var contentAnimation = _compositor.CreateVector3KeyFrameAnimation();
            contentAnimation.InsertKeyFrame(0f, Shown ? new Vector3(1.2f, 1.2f, 1f) : new Vector3(1f, 1f, 1f));
            contentAnimation.InsertKeyFrame(1f, Shown ? new Vector3(1f, 1f, 1f) : new Vector3(1.2f, 1.2f, 1f));
            contentAnimation.Duration = TimeSpan.FromMilliseconds(400);

            var batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            _maskVisual.StartAnimation("Opacity", maskAnimation);
            _contentVisual.StartAnimation("Opacity", maskAnimation);
            _contentVisual.StartAnimation("Scale", contentAnimation);
            batch.Completed += (sender, e) =>
            {
                if (!Shown)
                {
                    this.Visibility = Visibility.Collapsed;
                }
            };
            batch.End();
        }
    }
}
