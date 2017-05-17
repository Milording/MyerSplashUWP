using MyerSplash.ViewModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.ViewManagement;
using System;
using Windows.UI.Composition;
using MyerSplash.Common;
using System.Numerics;
using Windows.UI.Xaml.Hosting;
using MyerSplash.Common.Brush;

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

        public DrawerControl()
        {
            this.InitializeComponent();

            FullscreenBtn.Visibility = Visibility.Visible;
            Window.Current.SizeChanged += Current_SizeChanged;
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
