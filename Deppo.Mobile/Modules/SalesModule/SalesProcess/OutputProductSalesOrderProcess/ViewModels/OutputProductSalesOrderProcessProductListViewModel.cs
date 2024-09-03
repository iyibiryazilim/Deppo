using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(SalesCustomer), queryId: nameof(SalesCustomer))]
public partial class OutputProductSalesOrderProcessProductListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IWaitingSalesOrderService _waitingSalesOrderService;
	private readonly IServiceProvider _serviceProvider;
	private readonly IUserDialogs _userDialogs;

	public OutputProductSalesOrderProcessProductListViewModel(IHttpClientService httpClientService, IWaitingSalesOrderService waitingSalesOrderService, IServiceProvider serviceProvider, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_waitingSalesOrderService = waitingSalesOrderService;
		_serviceProvider = serviceProvider;
		_userDialogs = userDialogs;

		Title = "Sipariş ve Ürünler";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
	}

	#region Commands
	public Command LoadItemsCommand { get; }
	public Command ItemTappedCommand { get; }
	public Command NextViewCommand { get; }
	#endregion

	#region Collections
	public ObservableCollection<WaitingSalesOrder> Orders { get; } = new();
	public ObservableCollection<SalesCustomerProduct> Products { get; } = new();
	#endregion

	#region Properties
	[ObservableProperty]
	WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	SalesCustomer salesCustomer = null!;
	#endregion

	async Task LoadItemsAsync()
	{
		
		try
		{
			IsBusy = true;

			_userDialogs.Loading("Loading Items...");
			await Task.Delay(1000);
			Products.Clear();

            foreach (var item in SalesCustomer.Products)
            {
				Products.Add(item);
            }

			_userDialogs.Loading().Hide();
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

	async Task LoadOrdersAsync()
	{
		try
		{
			IsBusy = true;

			_userDialogs.Loading("Loading Items...");
			await Task.Delay(1000);
			Orders.Clear();

			_userDialogs.Loading().Hide();
		}
		catch (Exception)
		{

			throw;
		}
	}
}
