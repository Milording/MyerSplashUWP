using MyerSplash.Interface;
using System;
using System.Numerics;
using Windows.ApplicationModel;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyerSplash.Common
{
    public class ShownArgs
    {
        public bool Shown { get; set; }
    }

    public class NavigableUserControl : UserControl, INavigableUserControl
    {
        private bool IsWide
        {
            get
            {
                var ratio = Window.Current.Bounds.Width / Window.Current.Bounds.Height;
                return ratio > 1;
            }
        }

        public bool Shown
        {
            get { return (bool)GetValue(ShownProperty); }
            set { SetValue(ShownProperty, value); }
        }

        public static readonly DependencyProperty ShownProperty =
            DependencyProperty.Register("Shown", typeof(bool), typeof(NavigableUserControl),
                new PropertyMetadata(false, OnShownPropertyChanged));

        public event EventHandler<ShownArgs> OnShownChanged;

        private static void OnShownPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as INavigableUserControl;
            if ((bool)e.NewValue) control.OnShow();
            else control.OnHide();
            control.ToggleAnimation();
        }

        private Compositor _compositor;
        private Visual _rootVisual;

        public NavigableUserControl()
        {
            if (!DesignMode.DesignModeEnabled)
            {
                InitComposition();
                this.SizeChanged += UserControlBase_SizeChanged;
            }
        }

        private void UserControlBase_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResetOffset();
        }

        private void InitComposition()
        {
            _compositor = this.GetVisual().Compositor;
            _rootVisual = this.GetVisual();
            ResetOffset();
        }

        private void ResetOffset()
        {
            if (!Shown)
            {
                if (IsWide)
                {
                    _rootVisual.SetTranslation(new Vector3(0f, (float)this.ActualHeight, 0f));
                }
                else
                {
                    _rootVisual.SetTranslation(new Vector3((float)this.ActualWidth, 0f, 0f));
                }
            }
        }

        public virtual void OnHide()
        {
            OnShownChanged?.Invoke(this, new ShownArgs() { Shown = false });
        }

        public virtual void OnShow()
        {
            OnShownChanged?.Invoke(this, new ShownArgs() { Shown = true });
        }

        public void ToggleAnimation()
        {
            var offsetAnimation = _compositor.CreateScalarKeyFrameAnimation();
            offsetAnimation.InsertKeyFrame(1f, Shown ? 0f :
                (IsWide ? (float)this.ActualHeight : (float)this.ActualWidth));
            offsetAnimation.Duration = TimeSpan.FromMilliseconds(800);

            _rootVisual.StartAnimation(IsWide ? "Translation.Y" : "Translation.X", offsetAnimation);
        }
    }
}
