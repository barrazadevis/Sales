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

		//public static string EmailMessage => Resource.EmailMessage;
	}

}
