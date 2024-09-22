using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.Views;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(SalesCustomer), queryId: nameof(SalesCustomer))]
public partial class OutputProductSalesOrderProcessProductListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IWaitingSalesOrderService _waitingSalesOrderService;
	private readonly ISalesCustomerProductService _salesCustomerProductService;
	private readonly IServiceProvider _serviceProvider;
	private readonly IUserDialogs _userDialogs;

	[ObservableProperty]
	WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	SalesCustomer salesCustomer = null!;

	[ObservableProperty]
	ObservableCollection<SalesCustomerProduct> selectedProducts = new();

	[ObservableProperty]
	ObservableCollection<WaitingSalesOrderModel> selectedOrders = new();

	[ObservableProperty]
	int targetViewType = default;

	[ObservableProperty]
	bool isProductVisible = true;

	[ObservableProperty]
	bool isOrderVisible = false;

	public Page CurrentPage { get; set; }

	public OutputProductSalesOrderProcessProductListViewModel(IHttpClientService httpClientService, IWaitingSalesOrderService waitingSalesOrderService, ISalesCustomerProductService salesCustomerProductService, IServiceProvider serviceProvider, IUserDialogs userDialogs)
	{
		_httpClientService = httpClientService;
		_waitingSalesOrderService = waitingSalesOrderService;
		_salesCustomerProductService = salesCustomerProductService;
		_serviceProvider = serviceProvider;
		_userDialogs = userDialogs;

		Title = "Ürün Listesi";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ItemTappedCommand = new Command<SalesCustomerProduct>(async (item) => await ItemTappedAsync(item));

		LoadOrdersCommand = new Command(async () => await LoadOrdersAsync());
		LoadMoreOrdersCommand = new Command(async () => await LoadMoreOrdersAsync());
		OrderTappedCommand = new Command<WaitingSalesOrderModel>(async (item) => await OrderTappedAsync(item));

		SwitchViewCommand = new Command(async () => await SwitchViewAsync());
		NextViewCommand = new Command(async () => await NextViewAsync());
	}

	#region Commands
	public Command SwitchViewCommand { get; }
	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command<SalesCustomerProduct> ItemTappedCommand { get; }

	public Command LoadOrdersCommand { get; }
	public Command LoadMoreOrdersCommand { get; }
	public Command<WaitingSalesOrderModel> OrderTappedCommand { get; }

	public Command NextViewCommand { get; }
	#endregion

	#region Collections
	public ObservableCollection<WaitingSalesOrderModel> Orders { get; } = new();
	public ObservableCollection<SalesCustomerProduct> Items { get; } = new();
	#endregion

	private async Task SwitchViewAsync()
	{
		if (IsBusy)
			return;

		try
		{
			switch (TargetViewType)
			{
				case 0:
					await SwitchToOrderListViewAsync();
					break;
				case 1:
					await SwitchToProductListViewAsync();
					break;
				default:
					await SwitchToProductListViewAsync();
					break;
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

	//TargetViewType = 0
	private async Task SwitchToProductListViewAsync()
	{
		try
		{
			TargetViewType = 0;
			IsProductVisible = true;
			IsOrderVisible = false;
			CurrentPage.FindByName<Border>("productView").IsVisible = true;
			CurrentPage.FindByName<Border>("orderView").IsVisible = false;
			Orders.Clear();
			Title = "Ürün Listesi";
			SelectedOrders.Clear();
			await LoadItemsAsync();

		}
		catch (Exception ex)
		{
			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
	}

	//TargetViewType = 1
	private async Task SwitchToOrderListViewAsync()
	{
		try
		{
			TargetViewType = 1;
			IsProductVisible = false;
			IsOrderVisible = true;
			CurrentPage.FindByName<Border>("productView").IsVisible = false;
			CurrentPage.FindByName<Border>("orderView").IsVisible = true;
			SelectedProducts.Clear();
			Items.Clear();
			Title = "Sipariş Listesi";
			await LoadOrdersAsync();

		}
		catch (Exception ex)
		{
			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Yükleniyor...");
			Items.Clear();
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _salesCustomerProductService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SalesCustomer.ReferenceId, WarehouseModel.Number, string.Empty, 0, 20);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
					Items.Add(Mapping.Mapper.Map<SalesCustomerProduct>(item));
			}

			_userDialogs.HideHud();
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

	private async Task LoadMoreItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _salesCustomerProductService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SalesCustomer.ReferenceId, WarehouseModel.Number, string.Empty, Items.Count, 20);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
					Items.Add(Mapping.Mapper.Map<SalesCustomerProduct>(item));
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

	private async Task ItemTappedAsync(SalesCustomerProduct salesCustomerProduct)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			var selectedItem = Items.FirstOrDefault(x => x.ItemReferenceId == salesCustomerProduct.ItemReferenceId);

			if (selectedItem is not null)
			{
				if (selectedItem.IsSelected)
				{
					Items.FirstOrDefault(x => x.ItemReferenceId == salesCustomerProduct.ItemReferenceId).IsSelected = false;
					SelectedProducts.Remove(SelectedProducts.FirstOrDefault(x => x.ItemReferenceId == selectedItem.ItemReferenceId));
				}
				else
				{
					Items.FirstOrDefault(x => x.ItemReferenceId == salesCustomerProduct.ItemReferenceId).IsSelected = true;

					//var basketItem = new OutputSalesBasketModel
					//{
					//	ItemReferenceId = selectedItem.ItemReferenceId,
					//	ItemCode = selectedItem.ItemCode,
					//	ItemName = selectedItem.ItemName,
					//	IsVariant = selectedItem.IsVariant,
					//	UnitsetReferenceId = selectedItem.UnitsetReferenceId,
					//	UnitsetCode = selectedItem.UnitsetCode,
					//	UnitsetName = selectedItem.UnitsetName,
					//	SubUnitsetReferenceId = selectedItem.SubUnitsetReferenceId,
					//	SubUnitsetCode = selectedItem.SubUnitsetCode,
					//	SubUnitsetName = selectedItem.SubUnitsetName,
					//	MainItemReferenceId = selectedItem.MainItemReferenceId,
					//	MainItemCode = selectedItem.MainItemCode,
					//	MainItemName = selectedItem.MainItemName,
					//	StockQuantity = default,
					//	IsSelected = false,
					//	TrackingType = selectedItem.TrackingType,
					//	LocTracking = selectedItem.LocTracking,
					//	Image = string.Empty,
					//	Quantity = selectedItem.WaitingQuantity,
					//};

					//if (selectedItem.LocTracking == 1 || selectedItem.TrackingType == 1)
					//	basketItem.OutputQuantity = 0;
					//else
					//	basketItem.OutputQuantity = 1;

					SelectedProducts.Add(selectedItem);
				}
			}
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

	private async Task LoadOrdersAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Loading Orders...");
			Orders.Clear();
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _waitingSalesOrderService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, warehouseNumber: WarehouseModel.Number, customerReferenceId: SalesCustomer.ReferenceId, string.Empty, 0, 20);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
					Orders.Add(Mapping.Mapper.Map<WaitingSalesOrderModel>(item));
			}

			_userDialogs.HideHud();
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

	private async Task LoadMoreOrdersAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _waitingSalesOrderService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, warehouseNumber: WarehouseModel.Number, customerReferenceId: SalesCustomer.ReferenceId, string.Empty, Orders.Count, 20);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
					Orders.Add(Mapping.Mapper.Map<WaitingSalesOrderModel>(item));
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

	private async Task OrderTappedAsync(WaitingSalesOrderModel waitingSalesOrderModel)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			var selectedItem = Orders.FirstOrDefault(x => x.ReferenceId == waitingSalesOrderModel.ReferenceId);
			if (selectedItem is not null)
			{
				if (selectedItem.IsSelected)
				{
					Orders.FirstOrDefault(x => x.ReferenceId == waitingSalesOrderModel.ReferenceId).IsSelected = false;
					SelectedOrders.Remove(SelectedOrders.FirstOrDefault(x => x.ReferenceId == selectedItem.ReferenceId));
				}
				else
				{
					Orders.FirstOrDefault(x => x.ReferenceId == waitingSalesOrderModel.ReferenceId).IsSelected = true;
					SelectedOrders.Add(selectedItem);

					//if (SelectedProducts.ToList().Exists(x => x.ItemReferenceId == selectedItem.ProductReferenceId))
					//{
					//	SelectedProducts.FirstOrDefault(x => x.ItemReferenceId == selectedItem.ProductReferenceId).Quantity += selectedItem.WaitingQuantity;
					//}
					//else
					//{
					//	var basketItem = new OutputSalesBasketModel
					//	{
					//		ItemReferenceId = selectedItem.IsVariant ? selectedItem.VariantReferenceId : selectedItem.ProductReferenceId,
					//		ItemCode = selectedItem.IsVariant ? selectedItem.VariantCode : selectedItem.ProductCode,
					//		ItemName = selectedItem.IsVariant ? selectedItem.VariantName : selectedItem.VariantName,
					//		IsVariant = selectedItem.IsVariant,
					//		UnitsetReferenceId = selectedItem.UnitsetReferenceId,
					//		UnitsetCode = selectedItem.UnitsetCode,
					//		UnitsetName = selectedItem.UnitsetName,
					//		SubUnitsetReferenceId = selectedItem.SubUnitsetReferenceId,
					//		SubUnitsetCode = selectedItem.SubUnitsetCode,
					//		SubUnitsetName = selectedItem.SubUnitsetName,
					//		MainItemReferenceId = selectedItem.ProductReferenceId,
					//		MainItemCode = selectedItem.ProductCode,
					//		MainItemName = selectedItem.VariantName,
					//		StockQuantity = default,
					//		IsSelected = false,
					//		TrackingType = selectedItem.TrackingType,
					//		LocTracking = selectedItem.LocTracking,
					//		Image = string.Empty,
					//		Quantity = selectedItem.WaitingQuantity,
					//	};

					//	if (selectedItem.LocTracking == 1 || selectedItem.TrackingType == 1)
					//		basketItem.OutputQuantity = 0;
					//	else
					//		basketItem.OutputQuantity = 1;


					//}
				}
			}
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

	private async Task NextViewAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (SelectedOrders.Count == 0 && SelectedProducts.Count == 0)
			{
				await _userDialogs.AlertAsync("Lütfen işlem yapılabilecek bir ürün veya sipariş seçiniz.", "Uyarı", "Tamam");
				return;
			}

			CancellationTokenSource cts = new();
			ObservableCollection<OutputSalesBasketModel> basketItems = new();
			switch (TargetViewType)
			{
				case 0:
					basketItems = await ConvertProductItems().WaitAsync(cts.Token);
					break;
				case 1:
					basketItems = await ConvertOrderItems().WaitAsync(cts.Token);
					break;
				default:
					break;

			}

			await Shell.Current.GoToAsync($"{nameof(OutputProductSalesOrderProcessBasketListView)}", new Dictionary<string, object>
			{
				[nameof(WarehouseModel)] = WarehouseModel,
				[nameof(SalesCustomer)] = SalesCustomer,
				["Items"] = basketItems
			});
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


	private async Task<ObservableCollection<OutputSalesBasketModel>> ConvertOrderItems()
	{
		return await Task.Run(() =>
		{
			ObservableCollection<OutputSalesBasketModel> basketItems = new();
			foreach (var item in SelectedOrders)
			{
				var basketItem = new OutputSalesBasketModel
				{
					ItemReferenceId = item.ProductReferenceId,
					ItemCode = item.ProductCode,
					ItemName = item.ProductName,
					IsVariant = item.IsVariant,
					UnitsetReferenceId = item.UnitsetReferenceId,
					UnitsetCode = item.UnitsetCode,
					UnitsetName = item.UnitsetName,
					SubUnitsetReferenceId = item.SubUnitsetReferenceId,
					SubUnitsetCode = item.SubUnitsetCode,
					SubUnitsetName = item.SubUnitsetName,
					MainItemReferenceId = item.ProductReferenceId,
					MainItemCode = item.ProductCode,
					MainItemName = item.ProductName,
					StockQuantity = default,
					IsSelected = false,
					TrackingType = item.TrackingType,
					LocTracking = item.LocTracking,
					Image = string.Empty,
					Quantity = item.WaitingQuantity,
					LocTrackingIcon = item.LocTrackingIcon,
					TrackingTypeIcon = item.TrackingTypeIcon,
					VariantIcon = item.VariantIcon,
				};

				if (item.LocTracking == 1 || item.TrackingType == 1)
					basketItem.OutputQuantity = 0;
				else
					basketItem.OutputQuantity = 1;

				basketItems.Add(basketItem);
			}

			return basketItems;

		});
	}

	private async Task<ObservableCollection<OutputSalesBasketModel>> ConvertProductItems()
	{
		return await Task.Run(async () =>
		{
			ObservableCollection<OutputSalesBasketModel> basketItems = new();
			foreach (var item in SelectedProducts)
			{
				var basketItem = new OutputSalesBasketModel
				{
					ItemReferenceId = item.ItemReferenceId,
					ItemCode = item.ItemCode,
					ItemName = item.ItemName,
					IsVariant = item.IsVariant,
					UnitsetReferenceId = item.UnitsetReferenceId,
					UnitsetCode = item.UnitsetCode,
					UnitsetName = item.UnitsetName,
					SubUnitsetReferenceId = item.SubUnitsetReferenceId,
					SubUnitsetCode = item.SubUnitsetCode,
					SubUnitsetName = item.SubUnitsetName,
					MainItemReferenceId = item.MainItemReferenceId,
					MainItemCode = item.MainItemCode,
					MainItemName = item.MainItemName,
					StockQuantity = default,
					IsSelected = false,
					TrackingType = item.TrackingType,
					LocTracking = item.LocTracking,
					Image = string.Empty,
					Quantity = item.WaitingQuantity,
					LocTrackingIcon = item.LocTrackingIcon,
					TrackingTypeIcon = item.TrackingTypeIcon,
					VariantIcon = item.VariantIcon,
				};

				if (item.LocTracking == 1 || item.TrackingType == 1)
					basketItem.OutputQuantity = 0;
				else
					basketItem.OutputQuantity = 1;

				var httpClient = _httpClientService.GetOrCreateHttpClient();
				var productOrders = await _waitingSalesOrderService.GetObjectsByProduct(
									httpClient: httpClient,
									firmNumber: _httpClientService.FirmNumber,
									periodNumber: _httpClientService.PeriodNumber,
									warehouseNumber: WarehouseModel.Number,
									customerReferenceId: SalesCustomer.ReferenceId,
									productReferenceId: basketItem.ItemReferenceId,
									skip: 0,
									take: 99999999
				);

				if (productOrders.IsSuccess)
				{
					if (productOrders.Data is not null)
					{
						foreach (var productOrder in productOrders.Data)
						{
							basketItem.Orders.Add(new OutputSalesBasketOrderModel
							{

								ReferenceId = productOrder.ReferenceId,
								OrderReferenceId = productOrder.OrderReferenceId,
								CustomerReferenceId = productOrder.CustomerReferenceId,
								CustomerCode = productOrder.CustomerCode,
								CustomerName = productOrder.CustomerName,
								ProductReferenceId = productOrder.ProductReferenceId,
								ProductCode = productOrder.ProductCode,
								ProductName = productOrder.ProductName,
								UnitsetReferenceId = productOrder.UnitsetReferenceId,
								UnitsetCode = productOrder.UnitsetCode,
								UnitsetName = productOrder.UnitsetName,
								SubUnitsetReferenceId = productOrder.SubUnitsetReferenceId,
								SubUnitsetCode = productOrder.SubUnitsetCode,
								SubUnitsetName = productOrder.SubUnitsetName,
								Quantity = productOrder.Quantity,
								ShippedQuantity = productOrder.ShippedQuantity,
								WaitingQuantity = productOrder.WaitingQuantity,
								OrderDate = productOrder.OrderDate,
								DueDate = productOrder.DueDate,
							});
						}
					}
				}

				basketItems.Add(basketItem);
			}

			return basketItems;
		});
	}
}
