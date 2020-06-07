﻿using GalaSoft.MvvmLight.Command;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Sales.Common.Models;
using Sales.Helpers;
using Sales.Services;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Sales.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        #region Attributes

        private MediaFile file;

        private ImageSource imageSource;

        private ApiService apiService;

        private bool isRunning;

        private bool isEnabled;


        #endregion

        #region Properties

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }
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

        public ImageSource ImageSource {
            get { return this.imageSource; }
            set { this.SetValue(ref this.imageSource, value); }
        }


        #endregion

        #region Constructors
        public RegisterViewModel()
        {
            this.apiService = new ApiService();
            this.IsEnabled = true;
            this.ImageSource = "nouser";
        }
        #endregion

        #region Methods

        #endregion

        #region Commands

        public ICommand SaveCommand => new RelayCommand(Save);

        private async void Save()
        {
            if (string.IsNullOrEmpty(this.FirstName))
            {
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.FirstNameError,
                    Languages.Accept);
                return;
            }

            if (string.IsNullOrEmpty(this.LastName))
            {
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.LastNameError,
                    Languages.Accept);
                return;
            }

            if (string.IsNullOrEmpty(this.Email))
            {
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.EMailError,
                    Languages.Accept);
                return;
            }

            if (!RegexHelper.IsValidEmailAddress(this.Email))
            {
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.EMailError,
                    Languages.Accept);
                return;
            }

            if (string.IsNullOrEmpty(this.Phone))
            {
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PhoneError,
                    Languages.Accept);
                return;
            }

            if (string.IsNullOrEmpty(this.Password))
            {
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PasswordError,
                    Languages.Accept);
                return;
            }

            if (this.Password.Length < 6)
            {
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PasswordError,
                    Languages.Accept);
                return;
            }

            if (string.IsNullOrEmpty(this.PasswordConfirm))
            {
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PasswordConfirmError,
                    Languages.Accept);
                return;
            }

            if (this.Password.Equals(this.PasswordConfirm))
            {
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PasswordsNoMatch,
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
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    connection.Message,
                    Languages.Accept);
                return;
            }

            byte[] imageArray = null;
            if (this.file != null)
            {
                imageArray = FilesHelper.ReadyFully(this.file.GetStream());
            }

            var userRequest = new UserRequest
            {
                Address = this.Address,
                Email = this.Email,
                FirstName = this.FirstName,
                ImageArray = imageArray,
                LastName = this.LastName,
                Password = this.Password,
            };

            var url = App.Current.Resources["UrlAPI"].ToString();
            var prefix = App.Current.Resources["UrlPrefix"].ToString();
            var controller = App.Current.Resources["UrlUsersController"].ToString();
            var response = await this.apiService.Post(url, prefix, controller, userRequest);

            if (!response.IsSuccess)
            {
                this.IsRunning = false;
                this.IsEnabled = true;
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    response.Message,
                    Languages.Accept);
                return;
            }

            this.IsRunning = false;
            this.IsEnabled = true;

            await App.Current.MainPage.DisplayAlert(
                Languages.Confirm,
                Languages.RegisterConfirmation,
                Languages.Accept);

            await App.Current.MainPage.Navigation.PopAsync();



        }

        public ICommand ChangeImageCommand => new RelayCommand(ChangeImage);

        private async void ChangeImage()
        {
            await CrossMedia.Current.Initialize();

            var source = await Application.Current.MainPage.DisplayActionSheet(
                Languages.ImageSource,
                Languages.Cancel,
                null,
                Languages.FromGallery,
                Languages.NewPicture);

            if (source == Languages.Cancel)
            {
                this.file = null;
                return;
            }

            if (source == Languages.NewPicture)
            {
                this.file = await CrossMedia.Current.TakePhotoAsync(
                    new StoreCameraMediaOptions
                    {
                        Directory = "Sample",
                        Name = "test.jpg",
                        PhotoSize = PhotoSize.Small,
                    }
                );
            }
            else
            {
                this.file = await CrossMedia.Current.PickPhotoAsync();
            }

            if (this.file != null)
            {
                this.ImageSource = ImageSource.FromStream(() =>
                {
                    var stream = this.file.GetStream();
                    return stream;
                });
            }
        }
        #endregion
    }
}
