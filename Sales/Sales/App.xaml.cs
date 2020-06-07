﻿using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Sales
{
    using Newtonsoft.Json;
    using Sales.Common.Models;
    using Sales.Helpers;
    using Sales.ViewModels;
    using Views;
    public partial class App : Application
    {
        public static NavigationPage Navigator { get; internal set; }

        public App()
        {
            InitializeComponent();
            var mainViewModel = MainViewModel.GetInstance();

            if (Settings.IsRemembered)
            {
                if (!string.IsNullOrEmpty(Settings.UserASP))
                {
                    mainViewModel.UserASP = JsonConvert.DeserializeObject<MyUserASP>(Settings.UserASP);
                }

                mainViewModel.Products = new ProductsViewModel();
                this.MainPage = new MasterPage();
            }
            else
            {
                mainViewModel.Login = new LoginViewModel();
                this.MainPage = new NavigationPage(new LoginPage());
            }
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
