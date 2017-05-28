﻿using JP.Utils.UI;
using Microsoft.Graphics.Canvas.Effects;
using MyerSplash.Common;
using MyerSplash.Model;
using MyerSplash.ViewModel;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace MyerSplash.View
{
    public sealed partial class MainPage : CustomizedTitleBarPage
    {
        private const float TITLE_GRID_HEIGHT = 70;
        private const float DRAWER_WIDTH = 275;

        private MainViewModel MainVM { get; set; }

        private Compositor _compositor;
        private Visual _drawerVisual;
        private Visual _drawerMaskVisual;
        private Visual _titleGridVisual;
        private Visual _refreshBtnVisual;
        private Visual _titleStackVisual;

        private double _lastVerticalOffset;
        private bool _isHideTitleGrid;
        private bool _restoreTitleStackStatus;

        private UnsplashImageBase _clickedImg;
        private FrameworkElement _clickedContainer;
        private bool _waitForToggleDetailAnimation;

        public bool IsLoading
        {
            get { return (bool)GetValue(IsLoadingProperty); }
            set { SetValue(IsLoadingProperty, value); }
        }

        public static readonly DependencyProperty IsLoadingProperty =
            DependencyProperty.Register("IsLoading", typeof(bool), typeof(MainViewModel),
                new PropertyMetadata(false, OnLoadingPropertyChanged));

        public static void OnLoadingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var page = d as MainPage;
            if (!(bool)e.NewValue)
            {
                page.HideLoading();
            }
            else page.ShowLoading();
        }

        public bool DrawerOpended
        {
            get { return (bool)GetValue(DrawerOpendedProperty); }
            set { SetValue(DrawerOpendedProperty, value); }
        }

        public static readonly DependencyProperty DrawerOpendedProperty =
            DependencyProperty.Register("DrawerOpended", typeof(bool), typeof(MainPage),
                new PropertyMetadata(false, OnDrawerOpenedPropertyChanged));

        public static void OnDrawerOpenedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var page = d as MainPage;
            page.ToggleDrawerAnimation((bool)e.NewValue);
            page.ToggleDrawerMaskAnimation((bool)e.NewValue);
        }

        public MainPage()
        {
            this.InitializeComponent();
            this.DataContext = MainVM = new MainViewModel();
            this.Loaded += MainPage_Loaded;
            InitComposition();
            InitBlur();
            InitBinding();
        }

        private void BlurBackground_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_blurVisual != null)
            {
                _blurVisual.Size = new Vector2((float)e.NewSize.Width, (float)e.NewSize.Height);
            }
        }

        private SpriteVisual _blurVisual;

        private CompositionEffectBrush BuildEffectBrush()
        {
            var effect = new GaussianBlurEffect()
            {
                BlurAmount = 4f,
                BorderMode = EffectBorderMode.Hard,
                Source = new ArithmeticCompositeEffect()
                {
                    MultiplyAmount = 0,
                    Source1Amount = 0.2f,
                    Source2Amount = 0.8f,
                    Source1 = new CompositionEffectSourceParameter("source"),
                    Source2 = new ColorSourceEffect()
                    {
                        Color = "#000000".ToColor()
                    }
                }
            };

            var effectFactory = _compositor.CreateEffectFactory(effect);
            var backdropBrush = _compositor.CreateHostBackdropBrush();
            var effectBrush = effectFactory.CreateBrush();
            effectBrush.SetSourceParameter("source", backdropBrush);

            return effectBrush;
        }

        private void InitBlur()
        {
            var stackVisual = BlurBackground.GetVisual();
            var effectBrush = BuildEffectBrush();

            _blurVisual = _compositor.CreateSpriteVisual();
            _blurVisual.Brush = effectBrush;
            _blurVisual.Size = new Vector2((float)BlurBackground.ActualWidth, (float)BlurBackground.ActualHeight);

            ElementCompositionPreview.SetElementChildVisual(BlurBackground, _blurVisual);
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            _drawerMaskVisual.Opacity = 0;
            _drawerVisual.SetTranslation(new Vector3(-DRAWER_WIDTH, 0f, 0f));

            DrawerMaskBorder.Visibility = Visibility.Collapsed;
        }

        private void InitBinding()
        {
            var b = new Binding()
            {
                Source = MainVM,
                Path = new PropertyPath("IsRefreshing"),
                Mode = BindingMode.TwoWay,
            };
            this.SetBinding(IsLoadingProperty, b);

            var b2 = new Binding()
            {
                Source = MainVM,
                Path = new PropertyPath("DrawerOpened"),
                Mode = BindingMode.TwoWay,
            };
            this.SetBinding(DrawerOpendedProperty, b2);
        }

        private void InitComposition()
        {
            _compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            _drawerVisual = DrawerControl.GetVisual();
            _drawerMaskVisual = DrawerMaskBorder.GetVisual();
            _titleGridVisual = TitleGrid.GetVisual();
            _refreshBtnVisual = RefreshBtn.GetVisual();
            _titleStackVisual = TitleStack.GetVisual();
        }

        #region Loading animation
        private void ShowLoading()
        {
            ListControl.Refreshing = true;
        }

        private void HideLoading()
        {
            ListControl.Refreshing = false;
        }
        #endregion

        #region Drawer animation
        private void ToggleDrawerAnimation(bool show)
        {
            var offsetAnim = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnim.InsertKeyFrame(1f, show ? 0f : -DRAWER_WIDTH);
            offsetAnim.Duration = TimeSpan.FromMilliseconds(300);

            _drawerVisual.StartAnimation(_drawerVisual.GetTranslationXPropertyName(), offsetAnim);
        }

        private void ToggleDrawerMaskAnimation(bool show)
        {
            if (show) DrawerMaskBorder.Visibility = Visibility.Visible;

            var fadeAnimation = _compositor.CreateScalarKeyFrameAnimation();
            fadeAnimation.InsertKeyFrame(1f, show ? 0.8f : 0f);
            fadeAnimation.Duration = TimeSpan.FromMilliseconds(500);

            var batch = _compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
            _drawerMaskVisual.StartAnimation("Opacity", fadeAnimation);
            batch.Completed += (sender, e) =>
              {
                  if (!show) DrawerMaskBorder.Visibility = Visibility.Collapsed;
              };
            batch.End();
        }
        #endregion

        private void StackPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ListControl.ScrollToTop();
        }

        private void ListControl_OnClickItemStarted(UnsplashImageBase img, FrameworkElement container)
        {
            _clickedContainer = container;
            _clickedImg = img;

            DetailControl.Visibility = Visibility.Visible;
            if (DetailControl.ActualHeight == 0)
            {
                _waitForToggleDetailAnimation = true;
            }
            else
            {
                _waitForToggleDetailAnimation = false;
                ToggleDetailControlAnimation();
            }
        }

        private void DetailControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_waitForToggleDetailAnimation)
            {
                _waitForToggleDetailAnimation = false;

                ToggleDetailControlAnimation();
            }
        }

        private void DetailControl_OnHideControl()
        {
            if (_restoreTitleStackStatus)
            {
                ToggleTitleStackAnimation(true);
                ToggleRefreshBtnAnimation(true);
                _restoreTitleStackStatus = false;
            }
            ListControl.HideItemDetailAnimation();
        }

        private void ToggleDetailControlAnimation()
        {
            var currentPos = _clickedContainer.TransformToVisual(ListControl).TransformPoint(new Point(0, 0));
            var targetPos = GetTargetPosition();
            var targetRatio = GetTargetSize().X / _clickedContainer.ActualWidth;
            var targetOffsetX = targetPos.X - currentPos.X;
            var targetOffsetY = targetPos.Y - currentPos.Y;

            if (_titleStackVisual.Offset.Y == 0)
            {
                _restoreTitleStackStatus = true;
            }
            ToggleTitleStackAnimation(false);
            ToggleRefreshBtnAnimation(false);

            ListControl.MoveItemAnimation(new Vector3((float)targetOffsetX, (float)targetOffsetY, 0f), (float)targetRatio);
            DetailControl.CurrentImage = _clickedImg;
            DetailControl.ToggleDetailGridAnimation(true);

            NavigationService.AddOperation(() =>
            {
                DetailControl.HideDetailControl();
                return true;
            });
        }

        private Vector2 GetTargetPosition()
        {
            var size = GetTargetSize();

            var x = 0f;
            var y = 0f;
            if (size.X != this.ActualWidth)
            {
                x = (float)(this.ActualWidth - size.X) / 2f;
            }
            y = (float)(this.ActualHeight - size.Y) / 2f;

            return new Vector2(x, y);
        }

        private Vector2 GetTargetSize()
        {
            var width = Math.Min(640, this.ActualWidth);
            var height = width / 1.5 + 100;

            return new Vector2((float)width, (float)height);
        }

        private void DetailControl_Loaded(object sender, RoutedEventArgs e)
        {
            DetailControl.Visibility = Visibility.Collapsed;
            DetailControl.SizeChanged -= DetailControl_SizeChanged;
            DetailControl.SizeChanged += DetailControl_SizeChanged;
        }

        #region Scrolling
        private void ToggleTitleBarAnimation(bool show)
        {
            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, show ? 0f : -100f);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(500);

            _titleGridVisual.StartAnimation(_titleGridVisual.GetTranslationYPropertyName(), offsetAnimation);
        }

        private void ToggleRefreshBtnAnimation(bool show)
        {
            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, show ? 1f : 0);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(500);

            _refreshBtnVisual.CenterPoint = new Vector3((float)RefreshBtn.ActualWidth / 2f, (float)RefreshBtn.ActualHeight / 2f, 0f);
            _refreshBtnVisual.StartAnimation("Scale.X", offsetAnimation);
            _refreshBtnVisual.StartAnimation("Scale.Y", offsetAnimation);
        }

        private void ToggleTitleStackAnimation(bool show)
        {
            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, show ? 0f : -100f);
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(500);

            _titleStackVisual.StartAnimation(_titleStackVisual.GetTranslationYPropertyName(), offsetAnimation);
        }

        private void ListControl_OnScrollViewerViewChanged(ScrollViewer scrollViewer)
        {
            if ((scrollViewer.VerticalOffset - _lastVerticalOffset) > 5 && !_isHideTitleGrid)
            {
                _isHideTitleGrid = true;
                ToggleRefreshBtnAnimation(false);
                ToggleTitleStackAnimation(false);
                //ToggleTitleBarAnimation(false);
            }
            else if (scrollViewer.VerticalOffset < _lastVerticalOffset && _isHideTitleGrid)
            {
                _isHideTitleGrid = false;
                ToggleRefreshBtnAnimation(true);
                ToggleTitleStackAnimation(true);
                //ToggleTitleBarAnimation(true);
            }
            _lastVerticalOffset = scrollViewer.VerticalOffset;
        }
        #endregion

        #region Drawer manipulation
        private void TouchGrid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (e.Cumulative.Translation.X >= 70)
            {
                if (!DrawerOpended)
                {
                    DrawerOpended = true;
                }
                else
                {
                    ToggleDrawerAnimation(true);
                    ToggleDrawerMaskAnimation(true);
                }

                NavigationService.AddOperation(() =>
                {
                    if (MainVM.DrawerOpened)
                    {
                        MainVM.DrawerOpened = false;
                        return true;
                    }
                    return false;
                });
            }
            else
            {
                if (DrawerOpended)
                {
                    DrawerOpended = false;
                }
                else
                {
                    ToggleDrawerAnimation(false);
                    ToggleDrawerMaskAnimation(false);
                }
            }
        }

        private void TouchGrid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (_drawerMaskVisual.Opacity < 1)
            {
                DrawerMaskBorder.Visibility = Visibility.Visible;
                _drawerMaskVisual.Opacity += 0.02f;
            }
            var targetOffsetX = _drawerVisual.Offset.X + e.Delta.Translation.X;
            _drawerVisual.Offset = new Vector3((float)(targetOffsetX > 1 ? 1 : targetOffsetX), 0f, 0f);
        }

        private void DrawerControl_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.Delta.Translation.X > 0) return;

            if (_drawerMaskVisual.Opacity > 0)
            {
                _drawerMaskVisual.Opacity -= 0.01f;
            }
            var targetOffsetX = _drawerVisual.Offset.X - Math.Abs(e.Delta.Translation.X);
            _drawerVisual.Offset = new Vector3((float)(targetOffsetX <= -300f ? -300f : targetOffsetX), 0f, 0f);
        }

        private void DrawerControl_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            //DrawerMaskBorder.Visibility = Visibility.Collapsed;

            if (e.Cumulative.Translation.X < 0)
            {
                DrawerOpended = false;
            }
            else
            {

            }
        }
        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            CustomTitleBar();
        }

        protected override void SetUpTitleBar()
        {
            TitleBarHelper.SetUpLightTitleBar();
        }

        private void OnShownChanged(object sender, ShownArgs e)
        {
            if (!e.Shown)
            {
                Window.Current.SetTitleBar(TitleGrid);
            }
        }

        private void TitleGrid_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ToggleTitleStackAnimation(true);
        }

        private void TitleGrid_PointerExited(object sender, PointerRoutedEventArgs e)
        {
        }
    }
}
