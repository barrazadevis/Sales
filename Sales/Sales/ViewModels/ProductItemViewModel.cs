namespace Sales.ViewModels
{
    using System.Linq;
    using System.Windows.Input;
    using Common.Models;
    using GalaSoft.MvvmLight.Command;
    using Services;
    using Sales.Helpers;
    using Views;

    public class ProductItemViewModel : Product
    {
        #region Attributes

        private ApiService apiService;

        #endregion

        #region Constructors

        public ProductItemViewModel()
        {
            this.apiService = new ApiService();
        }

        #endregion



        #region Commands

        public ICommand EditProductCommand => new RelayCommand(EditProduct);

        private async void EditProduct()
        {
            MainViewModel.GetInstance().EditProduct = new EditProductViewModel(this);
            await App.Current.MainPage.Navigation.PushAsync(new EditProductPage());
        }

        public ICommand DeleteProductCommand => new RelayCommand(DeleteProduct);

        private async void DeleteProduct()
        {
            var answer = await App.Current.MainPage.DisplayAlert(
                Languages.Confirm, 
                Languages.DeleteConfirmation, 
                Languages.Yes, 
                Languages.No);
            if (!answer)
            {
                return;
            }

            var connection = await this.apiService.CheckConnection();
            if (!connection.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(Languages.Error, connection.Message, Languages.Accept);
                return;
            }


            var url = App.Current.Resources["UrlAPI"].ToString();
            var prefix = App.Current.Resources["UrlPrefix"].ToString();
            var controller = App.Current.Resources["UrlProductsController"].ToString();
            var response = await this.apiService.Delete<Product>(url, prefix, controller, this.ProductId);
            if (!response.IsSuccess)
            {
                await App.Current.MainPage.DisplayAlert(Languages.Error, response.Message, Languages.Accept);
                return;
            }


            var productsViewModel = ProductsViewModel.GetInstance();
            var deleteProduct = productsViewModel.MyProducts.Where(p => p.ProductId == this.ProductId).FirstOrDefault();
            if (deleteProduct != null)
            {
                productsViewModel.MyProducts.Remove(deleteProduct);
            }

            productsViewModel.RefreshList();

        }


        #endregion
    }
}
