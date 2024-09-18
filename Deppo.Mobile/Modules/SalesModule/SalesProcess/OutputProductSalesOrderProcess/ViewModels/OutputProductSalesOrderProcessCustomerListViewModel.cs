using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.Views;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class OutputProductSalesOrderProcessCustomerListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly ISalesCustomerService _salesCustomerService;
	private readonly ISalesCustomerProductService _salesCustomerProductService;
	private readonly IUserDialogs _userDialogs;

	[ObservableProperty]
	WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	SalesCustomer salesCustomer = null!;

	[ObservableProperty]
	SalesCustomer? swipedSalesCustomer;  // Sipariş listesini göstermek için swipe edilen müşteriyi tutar.

	#region Collections
	public ObservableCollection<SalesCustomer> Items { get; } = new();
	#endregion

	public OutputProductSalesOrderProcessCustomerListViewModel(IHttpClientService httpClientService,
	ISalesCustomerService salesCustomerService,
	IUserDialogs userDialogs,
	ISalesCustomerProductService salesCustomerProductService)
	{
		_httpClientService = httpClientService;
		_salesCustomerService = salesCustomerService;
		_userDialogs = userDialogs;
		_salesCustomerProductService = salesCustomerProductService;

		Title = "Müşteriler";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ItemTappedCommand = new Command<SalesCustomer>(async (customer) => await ItemTappedAsync(customer));
		NextViewCommand = new Command(async () => await NextViewAsync());
		ShowOrdersCommand = new Command<SalesCustomer>(async (customer) => await ShowOrdersAsync(customer));
	}

	#region Commands
	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command ItemTappedCommand { get; }
	public Command ShowOrdersCommand { get; }
	public Command NextViewCommand { get; }
	#endregion

	private async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Loading Items...");
			Items.Clear();
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _salesCustomerService.GetObjectsAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, warehouseNumber: WarehouseModel.Number, skip: 0, take: 20, search: "");
			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
					Items.Add(Mapping.Mapper.Map<SalesCustomer>(item));
			}

			_userDialogs.HideHud();
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message);
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task LoadMoreItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _salesCustomerService.GetObjectsAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, warehouseNumber: WarehouseModel.Number, skip: Items.Count, take: 20, search: "");
			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
					Items.Add(Mapping.Mapper.Map<SalesCustomer>(item));
			}

			_userDialogs.HideHud();
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message);
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task ShowOrdersAsync(SalesCustomer selectedItem)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			SwipedSalesCustomer = selectedItem;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var customerOrders = await _salesCustomerProductService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				customerReferenceId: selectedItem.ReferenceId,
				warehouseNumber: WarehouseModel.Number,
				skip: 0,
				take: 9999999
			);

			if (customerOrders.IsSuccess)
			{
				if (customerOrders.Data is null)
					return;

				foreach (var item in customerOrders.Data)
				{
					var customer = Items.FirstOrDefault(x => x.ReferenceId == selectedItem.ReferenceId);
					Console.WriteLine(customer);
					if(customer is not null)
					{
						customer.Products.Add(Mapping.Mapper.Map<SalesCustomerProduct>(item));

						//customer.Products.Add(new SalesCustomerProduct
						//{
						//	ItemReferenceId = item.ItemReferenceId,
						//	ItemCode = item.ItemCode,
						//	ItemName = item.ItemName,
						//	MainItemReferenceId = item.MainItemReferenceId,
						//	MainItemCode = item.MainItemCode,
						//	MainItemName = item.MainItemName,
						//	IsVariant = item.IsVariant,
						//	UnitsetReferenceId = item.UnitsetReferenceId,
						//	UnitsetCode = item.UnitsetCode,
						//	UnitsetName = item.UnitsetName,
						//	SubUnitsetReferenceId = item.SubUnitsetReferenceId,
						//	SubUnitsetCode = item.SubUnitsetCode,
						//	SubUnitsetName = item.SubUnitsetName,
						//	LocTracking = item.LocTracking,
						//	TrackingType = item.TrackingType,
						//	Quantity = item.Quantity,
						//	ShippedQuantity = item.ShippedQuantity,
						//	WaitingQuantity = item.WaitingQuantity,
						//});
					}
						

				}
			}
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

	private async Task ItemTappedAsync(SalesCustomer item)
	{
		try
		{
			IsBusy = true;

			if (SalesCustomer == item)
			{
				SalesCustomer.IsSelected = false;
				SalesCustomer = null;
			}
			else
			{
				if (SalesCustomer is not null)
				{
					SalesCustomer.IsSelected = false;
				}
				SalesCustomer = item;
				SalesCustomer.IsSelected = true;
			}
		}
		catch (Exception ex)
		{
			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task NextViewAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (SalesCustomer is not null)
			{
				await Shell.Current.GoToAsync($"{nameof(OutputProductSalesOrderProcessProductListView)}", new Dictionary<string, object>
				{
					[nameof(SalesCustomer)] = SalesCustomer,
					[nameof(WarehouseModel)] = WarehouseModel,
				});
			}
			else
			{
				_userDialogs.Alert("Lütfen bir müşteri seçiniz.", "Hata", "Tamam");
			}



		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}
}