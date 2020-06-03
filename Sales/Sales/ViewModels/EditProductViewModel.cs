namespace Sales.ViewModels
{
    using Plugin.Media.Abstractions;
    using Common.Models;
    using Services;
    using Xamarin.Forms;
    using System.Windows.Input;
    using GalaSoft.MvvmLight.Command;
    using Plugin.Media;
    using Sales.Helpers;
    using System.Linq;

    public class EditProductViewModel : BaseViewModel
    {
        #region Attributes
        private Product product;
        private MediaFile file;
        private ImageSource imageSource;
        private ApiService apiService;
        private bool isRunning;
        private bool isEnabled;
        #endregion

        #region Properties

        public ImageSource ImageSource
        {
            get { return this.imageSource; }

            set { this.SetValue(ref this.imageSource, value); }
        }

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

        public Product Product
        {
            get { return this.product; }
            set { this.SetValue(ref this.product, value); }
        }

        #endregion

        #region Constructors
        public EditProductViewModel(Product product)
        {
            this.product = product;
            this.apiService = new ApiService();
            this.isEnabled = true;
            this.ImageSource = product.ImageFullPath;
        }
        #endregion


        #region Commands

        public ICommand ChangeImageCommand
        {
            get
            {
                return new RelayCommand(ChangeImage);
            }
        }

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


        public ICommand SaveCommand
        {
            get
            {
                return new RelayCommand(Save);
            }
        }

        private async void Save()
        {
            if (string.IsNullOrEmpty(this.Product.Description))
            {
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.DescriptionError,
                    Languages.Accept);
                return;
            }

            if (this.Product.Price < 0)
            {
                await App.Current.MainPage.DisplayAlert(
                    Languages.Error,
                    Languages.PriceError,
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
                this.Product.ImageArray = imageArray;
            }

            var url = App.Current.Resources["UrlAPI"].ToString();
            var prefix = App.Current.Resources["UrlPrefix"].ToString();
            var controller = App.Current.Resources["UrlProductsController"].ToString();
            var response = await this.apiService.Put<Product>(url, prefix, controller, this.Product, this.Product.ProductId);

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

            var newProduct = (Product)response.Result;
            var productsViewModel = ProductsViewModel.GetInstance();

            var oldProduct = productsViewModel.MyProducts.Where(p => p.ProductId == this.Product.ProductId).FirstOrDefault();
            if (oldProduct != null)
            {
                productsViewModel.MyProducts.Remove(oldProduct);
            }

            productsViewModel.MyProducts.Add(newProduct);
            productsViewModel.RefreshList();

            this.IsRunning = false;
            this.IsEnabled = true;
            await App.Current.MainPage.Navigation.PopAsync();

        }

        #endregion
    }
}
