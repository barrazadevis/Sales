﻿namespace Sales.ViewModels
{
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Helpers;
    using Sales.Views;
    using Services;
    public class LoginViewModel : BaseViewModel
    {
        #region Atrributes
        private ApiService apiService;
        private bool isRunning;
        private bool isEnabled;

        #endregion

        #region Properties

        public string Email { get; set; }
        public string Password { get; set; }

        public bool IsRemembered { get; set; }

        public bool IsRunning
        {
            get { return this.isRunning; }

            set { this.SetValue(ref this.isRunning, value); }
        }
        public bool IsEnabled
        {
            get { return this.isEnabled; }

            set { this.SetValue(ref this.isEnabled, value); }
        }

        #endregion

        #region Constructors
        public LoginViewModel()
        {
            this.IsEnabled = true;
        }
        #endregion

        #region Commands

        public ICommand LoginCommand => new RelayCommand(Login);

        private async void Login()
        {
            if (string.IsNullOrEmpty(this.Email))
            {
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.EmailValidation,
                    Languages.Accept);
                return;
            }

            if (string.IsNullOrEmpty(this.Password))
            {
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PasswordValidation,
                    Languages.Accept);
                return;
            }

            this.IsRunning = true;
            this.IsEnabled = false;

            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(Languages.Error, connection.Message, Languages.Accept);
                return;
            }

            var url = App.Current.Resources["UrlAPI"].ToString();
            var token = await this.apiService.GetToken(url, this.Email, this.Password);

            if (token == null || string.IsNullOrEmpty(token.AccessToken))
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(Languages.Error, Languages.SomethingWrong, Languages.Accept);
                return;
            }

            Settings.TokenType = token.TokenType;
            Settings.AccessToken = token.AccessToken;
            Settings.IsRemembered = this.IsRemembered;

            MainViewModel.GetInstance().Products = new ProductsViewModel();
            App.Current.MainPage = new MasterPage();
            this.IsRunning = false;
            this.IsEnabled = true;

        }


        #endregion
    }
}