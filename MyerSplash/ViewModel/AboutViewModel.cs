﻿using GalaSoft.MvvmLight.Command;
using JP.Utils.Debug;
using JP.Utils.Helper;
using MyerSplash.Common;
using MyerSplash.UC;
using MyerSplashCustomControl;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Email;
using Windows.System;

namespace MyerSplash.ViewModel
{
    public class AboutViewModel
    {
        private RelayCommand _visitGitHubCommand;
        public RelayCommand VisitGitHubCommand
        {
            get
            {
                if (_visitGitHubCommand != null) return _visitGitHubCommand;
                return _visitGitHubCommand = new RelayCommand(async () =>
                  {
                      await Launcher.LaunchUriAsync(new Uri("https://github.com/JuniperPhoton/MyerSplashUWP"));
                  });
            }
        }

        private RelayCommand _donateCommand;
        public RelayCommand DonateCommand
        {
            get
            {
                if (_donateCommand != null) return _donateCommand;
                return _donateCommand = new RelayCommand(async () =>
                  {
                      var uc = new DonateDialogControl();
                      await PopupService.Instance.ShowAsync(uc);
                  });
            }
        }

        private RelayCommand _feedbackCommand;
        public RelayCommand FeedbackCommand
        {
            get
            {
                if (_feedbackCommand != null) return _feedbackCommand;
                return _feedbackCommand = new RelayCommand(async () =>
                  {
                      EmailRecipient rec = new EmailRecipient("dengweichao@hotmail.com");
                      EmailMessage mes = new EmailMessage();
                      mes.To.Add(rec);
                      var attach = await Logger.GetLogFileAttachementAsync();
                      if (attach != null)
                      {
                          mes.Attachments.Add(attach);
                      }
                      var platform = DeviceHelper.IsDesktop ? "PC" : "Mobile";

                      mes.Subject = $"MyerSplash for Windows 10 {platform}, {ResourcesHelper.GetDicString("AppVersion")} feedback, {DeviceHelper.OSVersion}";
                      await EmailManager.ShowComposeNewEmailAsync(mes);
                  });
            }
        }

        private RelayCommand _rateCommand;
        public RelayCommand RateCommand
        {
            get
            {
                if (_rateCommand != null) return _rateCommand;
                return _rateCommand = new RelayCommand(async () =>
                  {
                      await Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?PFN=" + Package.Current.Id.FamilyName));
                  });
            }
        }

        public AboutViewModel()
        {

        }
    }
}
