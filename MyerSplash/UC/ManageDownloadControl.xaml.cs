using MyerSplash.Common;
using MyerSplash.ViewModel;
using Windows.UI.Xaml;

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

        public ManageDownloadControl()
        {
            this.InitializeComponent();
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
