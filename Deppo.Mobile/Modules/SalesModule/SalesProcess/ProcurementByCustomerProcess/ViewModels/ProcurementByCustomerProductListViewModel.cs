using System;
using System.Collections.ObjectModel;
using Android.Views.Accessibility;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProcurementModels.ByCustomerModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.Views;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(ProcurementCustomerModel), queryId: nameof(ProcurementCustomerModel))]
[QueryProperty(name: nameof(OrderWarehouseModel), queryId: nameof(OrderWarehouseModel))]
public partial class ProcurementByCustomerProductListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IProcurementByCustomerProductService _procurementByCustomerProductService;
	private readonly IUserDialogs _userDialogs;
	private readonly IWaitingSalesOrderService _waitingSalesOrderService;

	[ObservableProperty]
	WarehouseModel? warehouseModel;    // toplama ambarı

	[ObservableProperty]
	WarehouseModel? orderWarehouseModel;   // Sipariş ambarı

	[ObservableProperty]
	ProcurementCustomerModel procurementCustomerModel;

	[ObservableProperty]
	public SearchBar searchText;

	public ProcurementByCustomerProductListViewModel(IHttpClientService httpClientService, IProcurementByCustomerProductService procurementByCustomerProductService, IUserDialogs userDialogs, IWaitingSalesOrderService waitingSalesOrderService)
	{
		_httpClientService = httpClientService;
		_procurementByCustomerProductService = procurementByCustomerProductService;
		_userDialogs = userDialogs;
		_waitingSalesOrderService = waitingSalesOrderService;

		Title = "Toplanabilir Ürünler";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		ItemTappedCommand = new Command<ProcurementCustomerProductModel>(async (item) => await ItemTappedAsync(item));
		PerformSearchCommand = new Command(async () => await PerformSearchAsync());
		PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());
	}

	public Page CurrentPage { get; set; }

	public ObservableCollection<ProcurementCustomerProductModel> Items { get; } = new();
	public ObservableCollection<ProcurementCustomerProductModel> SearchItems { get; } = new();

	public Command LoadItemsCommand { get; }
	public Command<ProcurementCustomerProductModel> ItemTappedCommand { get; }
	public Command SelectAllCommand { get; }
	public Command DeselectAllCommand { get; }
	public Command PerformSearchCommand { get; }
	public Command PerformEmptySearchCommand { get; }
	public Command NextViewCommand { get; }
	public Command BackCommand { get; }

	public async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;
			_userDialogs.ShowLoading("Yükleniyor...");
			Items.Clear();
			SearchItems.Clear();
			await Task.Delay(500);

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _procurementByCustomerProductService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: WarehouseModel?.Number ?? 0,
				orderWarehouseNumber: OrderWarehouseModel?.Number ?? 0,
				customerReferenceId: ProcurementCustomerModel.ReferenceId,
				shipAddressReferenceId: ProcurementCustomerModel.ShipAddressReferenceId,
				search: string.Empty,
				skip: 0,
				take: 99999
			);

			if (result.IsSuccess)
			{
				if (result.Data is not null)
					foreach (var item in result.Data)
					{
						var customer = Mapping.Mapper.Map<ProcurementCustomerProductModel>(item);
						customer.DestinationLocationCode = OrderWarehouseModel.LocationCode;
						customer.WaitingQuantity = customer.Quantity - customer.DispatchAmount - customer.ShippedQuantity;
						if(customer.WaitingQuantity > 0)
						{
							Items.Add(customer);
							SearchItems.Add(customer);
						}
					}
			}

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
		}
		catch (System.Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert($"{ex.Message}", "Hata", "Tamam");

		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task PerformEmptySearchAsync()
	{
		if (string.IsNullOrWhiteSpace(SearchText.Text))
		{
			await PerformSearchAsync();
		}
	}

	private async Task PerformSearchAsync()
	{
		if (IsBusy)
			return;
		try
		{
			if(string.IsNullOrWhiteSpace(SearchText.Text))
			{
				await LoadItemsAsync();
			}

			IsBusy = true;

			_userDialogs.Loading("Searching...");
			SearchItems.Clear();

			foreach (var item in Items.Where(x => x.ItemCode.StartsWith(SearchText.Text, StringComparison.OrdinalIgnoreCase))) // case insensitive searching
			{
				SearchItems.Add(item);
            }

            if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task BackAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			SearchText.Text = string.Empty;

			await Shell.Current.GoToAsync("..");
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}


	private async Task ItemTappedAsync(ProcurementCustomerProductModel item)
	{
		// if (IsBusy)
		//     return;

		// try
		// {
		//     IsBusy = true;

		//     if (item is not null)
		//     {
		//         await Task.Run(() =>
		//         {
		//             if (SelectedItems.Contains(item))
		//             {
		//                 item.IsSelected = false;
		//                 SelectedItems.Remove(item);
		//             }
		//             else
		//             {
		//                 item.IsSelected = true;
		//                 SelectedItems.Add(item);
		//             }

		//         });

		//     }

		// }
		// catch (System.Exception ex)
		// {
		//     _userDialogs.Alert($"{ex.Message}", "Hata", "Tamam");
		// }
		// finally
		// {
		//     IsBusy = false;
		// }
	}

	public async Task NextViewAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			if (Items.Count > 0)
			{
				_userDialogs.Loading("Yükleniyor...");

				var param = await GetProcurementProductList(Items);
				if (param.ProcurementProductList.Count != 0 && param.ProcurementProductList.Any(x => x.StockQuantity > 0))
				{
					if(_userDialogs.IsHudShowing)
						_userDialogs.HideHud();

					await Shell.Current.GoToAsync($"{nameof(ProcurementByCustomerBasketView)}", new Dictionary<string, object>
					{
						[nameof(ProcurementCustomerBasketModel)] = param,
						["OrderWarehouseModel"] = OrderWarehouseModel
					});

					SearchText.Text = string.Empty;
				}
				else
				{
					if(_userDialogs.IsHudShowing)
						_userDialogs.HideHud();
					await _userDialogs.AlertAsync("Toplanacak ürün bulunamadı.", "Hata", "Tamam");

				}


			}
		}

		catch (System.Exception ex)
		{
			_userDialogs.Alert($"{ex.Message}", "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}

	}

	/// <summary>
	/// Kısmi veya tamamı toplanabilir ürünleri getirir.
	/// </summary>
	/// <param name="items"></param>
	/// <returns></returns>
	private async Task<ProcurementCustomerBasketModel> GetProcurementProductList(ObservableCollection<ProcurementCustomerProductModel> items)
	{
		var procurementProductList = Items.Where(x => x.StockQuantity >= x.WaitingQuantity || x.StockQuantity > 0).ToList();

		ProcurementCustomerBasketModel procurementCustomerBasketModel = new();
		procurementCustomerBasketModel.ProcurementProductList = procurementProductList;


		var httpClient = _httpClientService.GetOrCreateHttpClient();

		foreach (var item in procurementCustomerBasketModel.ProcurementProductList)
		{
			item.Orders = new List<WaitingSalesOrder>();
			var productOrdersResult = await _waitingSalesOrderService.GetObjectsByProduct(
							 httpClient: httpClient,
									 firmNumber: _httpClientService.FirmNumber,
									 periodNumber: _httpClientService.PeriodNumber,
									 warehouseNumber: OrderWarehouseModel.Number,
									 customerReferenceId: ProcurementCustomerModel.ReferenceId,
									 productReferenceId: item.ItemReferenceId,
									 shipInfoReferenceId: ProcurementCustomerModel.ShipAddressReferenceId,
									 skip: 0,
									 take: 99999999
						 );


			foreach (var order in productOrdersResult.Data)
			{
				item.Orders.Add(Mapping.Mapper.Map<WaitingSalesOrder>(order));
			}


		}

		procurementCustomerBasketModel.WarehouseNumber = WarehouseModel?.Number ?? 0;
		procurementCustomerBasketModel.WarehouseName = WarehouseModel?.Name ?? string.Empty;
		procurementCustomerBasketModel.CustomerReferenceId = ProcurementCustomerModel.ReferenceId;
		procurementCustomerBasketModel.CustomerCode = ProcurementCustomerModel.Code;
		procurementCustomerBasketModel.CustomerName = ProcurementCustomerModel.Name;
		procurementCustomerBasketModel.ShipAddressReferenceId = ProcurementCustomerModel.ShipAddressReferenceId;
		procurementCustomerBasketModel.ShipAddressCode = ProcurementCustomerModel.ShipAddressCode;
		procurementCustomerBasketModel.ShipAddressName = ProcurementCustomerModel.ShipAddressName;

		return procurementCustomerBasketModel;
	}

}
