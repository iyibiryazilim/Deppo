using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.Views;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(SalesCustomer), queryId: nameof(SalesCustomer))]
public partial class OutputProductSalesProcessBasketListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly ILocationTransactionService _locationTransactionService;
	private readonly ISeriLotTransactionService _seriLotTransactionService;
	private readonly IUserDialogs _userDialogs;

	[ObservableProperty]
	WarehouseModel warehouseModel = null!;
	[ObservableProperty]
	SalesCustomer salesCustomer = null!;

	public OutputProductSalesProcessBasketListViewModel(IHttpClientService httpClientService, ILocationTransactionService locationTransactionService, ISeriLotTransactionService seriLotTransactionService, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_locationTransactionService = locationTransactionService;
		_seriLotTransactionService = seriLotTransactionService;
		_userDialogs = userDialogs;

		ShowProductViewCommand = new Command(async () => await ShowProductViewAsync());
	}

	public Page ContentPage { get; set; } = null!;

	public Command ShowProductViewCommand { get; }
	public Command NextViewCommand { get; }
	public Command BackCommand { get; }

	private async Task ShowProductViewAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(OutputProductSalesProcessProductListView)}", new Dictionary<string, object> 
			{
				[nameof(WarehouseModel)] = WarehouseModel,
				[nameof(SalesCustomer)] = SalesCustomer
			});
		}
		catch (Exception ex)
		{
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
			
		}
		finally
		{
			IsBusy = false;
		}
	}


}
