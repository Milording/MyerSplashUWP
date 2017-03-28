﻿using ImageLib;
using ImageLib.Cache.Storage;
using ImageLib.Gif;
using MyerSplashCustomControl;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyerSplash.UC
{
    public sealed partial class TipsControl : UserControl
    {
        public TipsControl()
        {
            this.InitializeComponent();
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            PopupService.Instance.TryHide();
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            PopupService.Instance.TryHide();
            App.MainVM.ShowSettingsUC = true;
        }
    }
}
