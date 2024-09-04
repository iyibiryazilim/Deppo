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
		IsProductListVisible = false;
		IsOrderListVisible = true;

		SwitchToProductListViewCommand = new Command(SwitchToProductListViewAsync);
		SwitchToOrderListViewCommand = new Command(SwitchToOrderListViewAsync);
	}

	#region Commands
	public Command SwitchToProductListViewCommand { get; }
	public Command SwitchToOrderListViewCommand { get; }
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

	[ObservableProperty]
	bool isProductListVisible = false;

	[ObservableProperty]
	bool isOrderListVisible = true;
	#endregion

	async void SwitchToProductListViewAsync()
	{
		IsProductListVisible = true;
		IsOrderListVisible = false;

		// Ürünler verilerini yükle
		await LoadItemsAsync();
	}

	async void SwitchToOrderListViewAsync()
	{
		IsProductListVisible = false;
		IsOrderListVisible = true;

		// Sipariş verilerini yükle
		await LoadOrdersAsync();
	}

	async Task LoadItemsAsync()
	{		
		try
		{
			IsBusy = true;

			Products.Clear();
			_userDialogs.Loading("Loading Items...");
			await Task.Delay(1000);
			

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

			Orders.Clear();
			_userDialogs.Loading("Loading Items...");
			await Task.Delay(1000);
			
            foreach (var item in SalesCustomer.Products)
            {
                if(item.Orders is not null)
				{
                    foreach (var order in item.Orders)
                    {
						Orders.Add(order);
                    }
                }
            }

            _userDialogs.Loading().Hide();
		}
		catch (Exception)
		{

			throw;
		}
		finally
		{
			IsBusy = false;
		}
	}
}
