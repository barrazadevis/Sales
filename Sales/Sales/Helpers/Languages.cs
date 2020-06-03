namespace Sales.Helpers
{
	using Interfaces;
	using Resources;
	using Xamarin.Forms;

	public static class Languages
	{
		static Languages()
		{
			var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
			Resource.Culture = ci;
			DependencyService.Get<ILocalize>().SetLocale(ci);
		}

		public static string Accept => Resource.Accept;

		public static string Error => Resource.Error;

		public static string NoInternet => Resource.NoInternet;

		public static string Products => Resource.Products;

		public static string TurnOnInternet => Resource.TurnOnInternet;

		public static string AddProduct => Resource.AddProduct;

		public static string Description => Resource.Description;

		public static string DescriptionPlaceHolder => Resource.DescriptionPlaceHolder;

		public static string Price => Resource.Price;

		public static string PricePlaceHolder => Resource.PricePlaceHolder;

		public static string Remarks => Resource.Remarks;

		public static string Save => Resource.Save;

		public static string ChangeImage => Resource.ChangeImage;
		public static string DescriptionError => Resource.DescriptionError;
		public static string PriceError => Resource.PriceError;
		public static string ImageSource => Resource.ImageSource;
		public static string FromGallery => Resource.FromGallery;
		public static string NewPicture => Resource.NewPicture;
		public static string Cancel => Resource.Cancel;

		public static string Edit => Resource.Edit;
		public static string Delete => Resource.Delete;
		public static string DeleteConfirmation => Resource.DeleteConfirmation;
		public static string Yes => Resource.Yes;
		public static string No => Resource.No;
		public static string Confirm => Resource.Confirm;
		public static string EditProduct => Resource.EditProduct;
		public static string IsAvailable => Resource.IsAvailable;



		//public static string EmailMessage => Resource.EmailMessage;
	}

}
