using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.Views;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(PurchaseSupplier), queryId: nameof(PurchaseSupplier))]
public partial class InputProductPurchaseOrderProcessProductListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	private readonly IServiceProvider _serviceProvider;
	private readonly IWaitingPurchaseOrderService _waitingPurchaseOrderService;
	private readonly IPurchaseSupplierProductService _purchaseSupplierProductService;

	[ObservableProperty]
	private WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	private PurchaseSupplier purchaseSupplier = null!;

	public ObservableCollection<PurchaseSupplierProduct> Items { get; } = new();

	public ObservableCollection<PurchaseSupplierProduct> SelectedItems { get; } = new();

	[ObservableProperty]
	public ObservableCollection<InputPurchaseBasketModel> basketItems = new();

	[ObservableProperty]
	public SearchBar searchText;

	public InputProductPurchaseOrderProcessProductListViewModel(
		IHttpClientService httpClientService, IUserDialogs userDialogs,
		IServiceProvider serviceProvider,
		IWaitingPurchaseOrderService waitingPurchaseOrderService,
		IPurchaseSupplierProductService purchaseSupplierProductService)
	{
		_httpClientService = httpClientService;

		_userDialogs = userDialogs;
		_serviceProvider = serviceProvider;
		_waitingPurchaseOrderService = waitingPurchaseOrderService;
		_purchaseSupplierProductService = purchaseSupplierProductService;

		Title = "Ürün Listesi";

		LoadItemsCommand = new Command(async () => await LoadItemsAsync());
		LoadMoreItemsCommand = new Command(async () => await LoadMoreItemsAsync());
		ItemTappedCommand = new Command<PurchaseSupplierProduct>(async (x) => await ItemTappedAsync(x));

		PerformSearchCommand = new Command(async () => await PerformSearchAsync());
		PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());

		ConfirmCommand = new Command(async () => await ConfirmAsync());
		CloseCommand = new Command(async () => await CloseAsync());
	}

	public Page CurrentPage { get; set; }

	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command<PurchaseSupplierProduct> ItemTappedCommand { get; }
	public Command PerformSearchCommand { get; }
	public Command PerformEmptySearchCommand { get; }

	public Command ConfirmCommand { get; }
	public Command CloseCommand { get; }

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
			var result = await _purchaseSupplierProductService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, PurchaseSupplier.ReferenceId, WarehouseModel.Number, SearchText.Text, 0, 20);
			if (result.IsSuccess)
			{
				if (result.Data is not null)
				{
					foreach (var item in result.Data)
					{
						var product = Mapping.Mapper.Map<PurchaseSupplierProduct>(item);
						var matchedItem = SelectedItems.FirstOrDefault(x => x.ItemReferenceId == product.ItemReferenceId);
						if (matchedItem is not null)
							product.IsSelected = matchedItem.IsSelected;
						else
							product.IsSelected = false;

						Items.Add(product);
					}
				}
			}

			_userDialogs.HideHud();
		}
		catch (System.Exception ex)
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

	private async Task LoadMoreItemsAsync()
	{
		if (IsBusy)
			return;

		if (Items.Count < 18)
			return;
		try
		{
			IsBusy = true;
			_userDialogs.ShowLoading("Loading More Items...");

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _purchaseSupplierProductService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, PurchaseSupplier.ReferenceId, WarehouseModel.Number, SearchText.Text, Items.Count, 20);
			if (result.IsSuccess)
			{
				if (result.Data is not null)
				{
					foreach (var item in result.Data)
					{
						var product = Mapping.Mapper.Map<PurchaseSupplierProduct>(item);
						var matchedItem = SelectedItems.FirstOrDefault(x => x.ItemReferenceId == product.ItemReferenceId);
						if (matchedItem is not null)
							product.IsSelected = matchedItem.IsSelected;
						else
							product.IsSelected = false;

						Items.Add(product);
					}

					_userDialogs.HideHud();
				}
			}
		}
		catch (System.Exception ex)
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

	private async Task ItemTappedAsync(PurchaseSupplierProduct purchaseSupplierProduct)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			var selectedItem = Items.FirstOrDefault(x => x.ItemReferenceId == purchaseSupplierProduct.ItemReferenceId && x.SubUnitsetCode == purchaseSupplierProduct.SubUnitsetCode);
			if (selectedItem is not null)
			{
				if (selectedItem.IsSelected)
				{
					Items.FirstOrDefault(x => x.ItemReferenceId == purchaseSupplierProduct.ItemReferenceId && x.SubUnitsetCode == purchaseSupplierProduct.SubUnitsetCode).IsSelected = false;
					SelectedItems.Remove(SelectedItems.FirstOrDefault(x => x.ItemReferenceId == selectedItem.ItemReferenceId && x.SubUnitsetCode == selectedItem.SubUnitsetCode));
					BasketItems.Remove(BasketItems.FirstOrDefault(x => x.ItemReferenceId == selectedItem.ItemReferenceId && x.SubUnitsetCode == selectedItem.SubUnitsetCode));
				}
				else
				{
					Items.FirstOrDefault(x => x.ItemReferenceId == purchaseSupplierProduct.ItemReferenceId && x.SubUnitsetCode == purchaseSupplierProduct.SubUnitsetCode).IsSelected = true;
					SelectedItems.Add(selectedItem);

					var inputPurchaseBasketModelItem = new InputPurchaseBasketModel
					{
						ItemReferenceId = selectedItem.ItemReferenceId,
						ItemCode = selectedItem.ItemCode,
						ItemName = selectedItem.ItemName,
						IsVariant = selectedItem.IsVariant,
						UnitsetReferenceId = selectedItem.UnitsetReferenceId,
						UnitsetCode = selectedItem.UnitsetCode,
						UnitsetName = selectedItem.UnitsetName,
						SubUnitsetReferenceId = selectedItem.SubUnitsetReferenceId,
						SubUnitsetCode = selectedItem.SubUnitsetCode,
						SubUnitsetName = selectedItem.SubUnitsetName,
						MainItemReferenceId = selectedItem.MainItemReferenceId,
						MainItemCode = selectedItem.MainItemCode,
						MainItemName = selectedItem.MainItemName,
						StockQuantity = default,
						IsSelected = false,
						TrackingType = selectedItem.TrackingType,
						LocTracking = selectedItem.LocTracking,
						Image = selectedItem.ImageData,
						Quantity = selectedItem.WaitingQuantity,
					};

					if (selectedItem.LocTracking == 1 || selectedItem.TrackingType == 1)
						inputPurchaseBasketModelItem.InputQuantity = 0;
					else
						inputPurchaseBasketModelItem.InputQuantity = 1;

					BasketItems.Add(inputPurchaseBasketModelItem);
				}
			}
		}
		catch (System.Exception ex)
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

	private async Task ConfirmAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var previousViewModel = _serviceProvider.GetRequiredService<InputProductPurchaseOrderProcessBasketListViewModel>();

			if (BasketItems.Count > 0)
			{
				_userDialogs.Loading("Ürünler sepete ekleniyor...");

				if (previousViewModel is not null)
				{
					foreach (var basketItem in BasketItems)
					{
						var productOrdersResult = await _waitingPurchaseOrderService.GetObjectsByProduct(
							httpClient: httpClient,
									firmNumber: _httpClientService.FirmNumber,
									periodNumber: _httpClientService.PeriodNumber,
									warehouseNumber: WarehouseModel.Number,
									supplierReferenceId: PurchaseSupplier.ReferenceId,
									productReferenceId: basketItem.ItemReferenceId,
									skip: 0,
									take: 99999999
						);

						if (productOrdersResult.IsSuccess)
						{
							if (productOrdersResult.Data is not null)
							{
								basketItem.Orders.Clear();
								foreach (var productOrder in productOrdersResult.Data)
								{
									basketItem.Orders.Add(new InputPurchaseBasketOrderModel
									{
										ReferenceId = productOrder.ReferenceId,
										OrderReferenceId = productOrder.OrderReferenceId,
										SupplierReferenceId = productOrder.SupplierReferenceId,
										SupplierCode = productOrder.SupplierReferenceId,
										SupplierName = productOrder.SupplierReferenceIds,
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
										IsVariant = productOrder.IsVariant,
										LocTracking = productOrder.LocTracking,
										TrackingType = productOrder.TrackingType,
										OrderDate = productOrder.OrderDate,
										DueDate = productOrder.DueDate,
									});

								}
							}
						}

						if (!previousViewModel.Items.Any(x => x.ItemReferenceId == basketItem.ItemReferenceId && x.SubUnitsetCode == basketItem.SubUnitsetCode))
						{
							previousViewModel.Items.Add(basketItem);
						}
						else
						{
							foreach (var order in basketItem.Orders)
							{
								var existingItem = previousViewModel.Items.FirstOrDefault(x => x.ItemReferenceId == basketItem.ItemReferenceId && x.SubUnitsetCode == basketItem.SubUnitsetCode);
								if (!existingItem.Orders.Any(x => x.ReferenceId == order.ReferenceId))
								{
									existingItem.Orders.Add(order);
								}

							}

						}
					}
					SelectedItems.Clear();
					BasketItems.Clear();

					await Shell.Current.GoToAsync("..");
				}
			}
			else
			{
				await Shell.Current.GoToAsync("..");
			}

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

	private async Task CloseAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (SelectedItems.Count > 0)
			{
				var confirm = await _userDialogs.ConfirmAsync("Seçmiş olduğunuz ürünler silinecektir. Devam etmek istiyor musunuz?", "Hata", "Evet", "Hayır");

				if (!confirm)
					return;

				SelectedItems.Clear();
				BasketItems.Clear();
				await Shell.Current.GoToAsync("..");
			}
			else
			{
				await Shell.Current.GoToAsync("..");
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

	//private async Task<ObservableCollection<InputPurchaseBasketModel>> ConvertOrderItems()
	//{
	//    return await Task.Run(() =>
	//    {
	//        ObservableCollection<InputPurchaseBasketModel> basketItems = new();
	//        foreach (var item in SelectedOrders)
	//        {
	//            var basketItem = new InputPurchaseBasketModel
	//            {
	//                ItemReferenceId = item.ProductReferenceId,
	//                ItemCode = item.ProductCode,
	//                ItemName = item.ProductName,
	//                IsVariant = item.IsVariant,
	//                UnitsetReferenceId = item.UnitsetReferenceId,
	//                UnitsetCode = item.UnitsetCode,
	//                UnitsetName = item.UnitsetName,
	//                SubUnitsetReferenceId = item.SubUnitsetReferenceId,
	//                SubUnitsetCode = item.SubUnitsetCode,
	//                SubUnitsetName = item.SubUnitsetName,
	//                MainItemReferenceId = item.ProductReferenceId,
	//                MainItemCode = item.ProductCode,
	//                MainItemName = item.ProductName,
	//                StockQuantity = default,
	//                IsSelected = false,
	//                TrackingType = item.TrackingType,
	//                LocTracking = item.LocTracking,
	//                Image = string.Empty,
	//                Quantity = item.WaitingQuantity,
	//            };

	//            if (item.LocTracking == 1 || item.TrackingType == 1)
	//                basketItem.InputQuantity = 0;
	//            else
	//                basketItem.InputQuantity = 1;

	//            basketItems.Add(basketItem);
	//        }

	//        return basketItems;
	//    });
	//}

	//private async Task<ObservableCollection<InputPurchaseBasketModel>> ConvertProductItems()
	//{
	//    return await Task.Run(async () =>
	//    {
	//        ObservableCollection<InputPurchaseBasketModel> basketItems = new();
	//        foreach (var item in SelectedProducts)
	//        {
	//            var basketItem = new InputPurchaseBasketModel
	//            {
	//                ItemReferenceId = item.ItemReferenceId,
	//                ItemCode = item.ItemCode,
	//                ItemName = item.ItemName,
	//                IsVariant = item.IsVariant,
	//                UnitsetReferenceId = item.UnitsetReferenceId,
	//                UnitsetCode = item.UnitsetCode,
	//                UnitsetName = item.UnitsetName,
	//                SubUnitsetReferenceId = item.SubUnitsetReferenceId,
	//                SubUnitsetCode = item.SubUnitsetCode,
	//                SubUnitsetName = item.SubUnitsetName,
	//                MainItemReferenceId = item.MainItemReferenceId,
	//                MainItemCode = item.MainItemCode,
	//                MainItemName = item.MainItemName,
	//                StockQuantity = default,
	//                IsSelected = false,
	//                TrackingType = item.TrackingType,
	//                LocTracking = item.LocTracking,
	//                Image = string.Empty,
	//                Quantity = item.WaitingQuantity,
	//            };

	//            if (item.LocTracking == 1 || item.TrackingType == 1)
	//                basketItem.InputQuantity = 0;
	//            else
	//                basketItem.InputQuantity = 1;

	//            var httpClient = _httpClientService.GetOrCreateHttpClient();
	//            var result = await _waitingPurchaseOrderService.GetObjectsByProduct(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, warehouseNumber: WarehouseModel.Number, supplierReferenceId: PurchaseSupplier.ReferenceId, productReferenceId: item.ItemReferenceId, string.Empty, 0, 999999999);
	//            if (result.IsSuccess)
	//            {
	//                if (result.Data is not null)
	//                {
	//                    foreach (var purchaseOrder in result.Data)
	//                    {
	//                        basketItem.Orders.Add(new InputPurchaseBasketOrderModel
	//                        {
	//                            OrderReferenceId = purchaseOrder.ReferenceId,
	//                            SupplierReferenceId = purchaseOrder.SupplierReferenceId,
	//                            SupplierCode = purchaseOrder.SupplierCode,
	//                            SupplierName = purchaseOrder.SupplierName,
	//                            ProductReferenceId = purchaseOrder.ProductReferenceId,
	//                            ProductCode = purchaseOrder.ProductCode,
	//                            ProductName = purchaseOrder.ProductName,
	//                            UnitsetReferenceId = purchaseOrder.UnitsetReferenceId,
	//                            UnitsetCode = purchaseOrder.UnitsetCode,
	//                            UnitsetName = purchaseOrder.UnitsetName,
	//                            SubUnitsetName = purchaseOrder.SubUnitsetName,
	//                            SubUnitsetCode = purchaseOrder.SubUnitsetCode,
	//                            SubUnitsetReferenceId = purchaseOrder.UnitsetReferenceId,
	//                            Quantity = purchaseOrder.Quantity,
	//                            ShippedQuantity = purchaseOrder.ShippedQuantity,
	//                            WaitingQuantity = purchaseOrder.WaitingQuantity,
	//                            OrderDate = purchaseOrder.OrderDate,
	//                            DueDate = purchaseOrder.DueDate,
	//                        });
	//                    }
	//                }
	//            }

	//            basketItems.Add(basketItem);
	//        }

	//        return basketItems;
	//    });
	//}

	private async Task PerformSearchAsync()
	{
		if (IsBusy)
			return;

		try
		{
			if (string.IsNullOrWhiteSpace(SearchText.Text))
			{
				await LoadItemsAsync();
				SearchText.Unfocus();
				return;
			}
			IsBusy = true;
			Items.Clear();

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _purchaseSupplierProductService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, PurchaseSupplier.ReferenceId, WarehouseModel.Number, SearchText.Text, 0, 20);
			if (result.IsSuccess)
			{
				if (result.Data is not null)
				{
					foreach (var item in result.Data)
					{
						var product = Mapping.Mapper.Map<PurchaseSupplierProduct>(item);
						var matchedItem = SelectedItems.FirstOrDefault(x => x.ItemReferenceId == product.ItemReferenceId);
						if (matchedItem is not null)
							product.IsSelected = matchedItem.IsSelected;
						else
							product.IsSelected = false;

						Items.Add(product);
					}
				}
			}
		}
		catch (System.Exception ex)
		{
			_userDialogs.Alert(ex.Message, "Hata");
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
}