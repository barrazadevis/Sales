﻿using GalaSoft.MvvmLight.Command;
using Sales.Helpers;
using Sales.ViewModels;
using Sales.Views;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Sales.ViewsModels
{
    public class MenuItemViewModel
    {
        #region Properties
        public string Icon { get; set; }
        public string Title { get; set; }
        public string PageName { get; set; }
        #endregion

        #region Commands

        public ICommand GoToCommand => new RelayCommand(GoTo);

        private void GoTo()
        {
            if (this.PageName == "LoginPage")
            {
                Settings.AccessToken = string.Empty;
                Settings.TokenType = string.Empty;
                Settings.IsRemembered = false;
                MainViewModel.GetInstance().Login = new LoginViewModel();
                App.Current.MainPage = new NavigationPage(new LoginPage());
            }
        }

        #endregion
    }
}
