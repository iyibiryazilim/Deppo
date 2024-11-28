using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
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
	private WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	private SalesCustomer salesCustomer = null!;

	[ObservableProperty]
	public SearchBar productSearchText;

	public ObservableCollection<SalesCustomerProduct> Items { get; } = new();
	public ObservableCollection<SalesCustomerProduct> SelectedItems { get; } = new();

	[ObservableProperty]
	public ObservableCollection<OutputSalesBasketModel> basketItems = new();

	//Arama İşleminde seçim yapılan ürünler
	public ObservableCollection<SalesCustomerProduct> SelectedSearchItems { get; } = new();

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
		CloseCommand = new Command(async () => await CloseAsync());
		ConfirmCommand = new Command(async () => await ConfirmAsync());

		PerformSearchCommand = new Command(async () => await PerformSearchAsync());
		PerformEmptySearchCommand = new Command(async () => await PerformEmptySearchAsync());
	}

	public Page CurrentPage { get; set; } = null!;

	#region Commands

	public Command LoadItemsCommand { get; }
	public Command LoadMoreItemsCommand { get; }
	public Command<SalesCustomerProduct> ItemTappedCommand { get; }
	public Command PerformSearchCommand { get; }
	public Command PerformEmptySearchCommand { get; }

	public Command ConfirmCommand { get; }
	public Command CloseCommand { get; }

	#endregion Commands

	private async Task LoadItemsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Yükleniyor...");
			Items.Clear();
			SelectedItems.Clear();
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _salesCustomerProductService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SalesCustomer.ReferenceId, WarehouseModel.Number, shipInfoReferenceId: SalesCustomer.ShipAddressReferenceId, ProductSearchText.Text, 0, 20);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<SalesCustomerProduct>(item);
					var matchingItem = SelectedItems.FirstOrDefault(x => x.ItemReferenceId == obj.ItemReferenceId);
					if (matchingItem is not null)
						obj.IsSelected = matchingItem.IsSelected;

					Items.Add(obj);
				}
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
			var result = await _salesCustomerProductService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SalesCustomer.ReferenceId, WarehouseModel.Number, shipInfoReferenceId: SalesCustomer.ShipAddressReferenceId, ProductSearchText.Text, Items.Count, 20);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<SalesCustomerProduct>(item);
					var matchingItem = SelectedItems.FirstOrDefault(x => x.ItemReferenceId == obj.ItemReferenceId);
					if (matchingItem is not null)
						obj.IsSelected = matchingItem.IsSelected;

					Items.Add(obj);
				}
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

	private async Task PerformSearchAsync()
	{
		if (IsBusy)
			return;

		try
		{
			if (string.IsNullOrWhiteSpace(ProductSearchText.Text))
			{
				await LoadItemsAsync();
				ProductSearchText.Unfocus();
				return;
			}
			IsBusy = true;
			Items.Clear();
			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _salesCustomerProductService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SalesCustomer.ReferenceId, WarehouseModel.Number, shipInfoReferenceId: SalesCustomer.ShipAddressReferenceId, ProductSearchText.Text, Items.Count, 20);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<SalesCustomerProduct>(item);
					var matchingItem = SelectedItems.FirstOrDefault(x => x.ItemReferenceId == obj.ItemReferenceId);
					if (matchingItem is not null)
						obj.IsSelected = matchingItem.IsSelected;

					Items.Add(obj);
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
		if (string.IsNullOrWhiteSpace(ProductSearchText.Text))
		{
			await PerformSearchAsync();
		}
	}

	private async Task ItemTappedAsync(SalesCustomerProduct salesCustomerProduct)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = IsBusy = true;

			var selectedItem = Items.FirstOrDefault(x => x.ItemReferenceId == salesCustomerProduct.ItemReferenceId);

			if (selectedItem is not null)
			{
				if (selectedItem.IsSelected)
				{
					Items.FirstOrDefault(x => x.ItemReferenceId == salesCustomerProduct.ItemReferenceId).IsSelected = false;
					SelectedItems.Remove(SelectedItems.FirstOrDefault(x => x.ItemReferenceId == selectedItem.ItemReferenceId));
					BasketItems.Remove(BasketItems.FirstOrDefault(x => x.ItemReferenceId == selectedItem.ItemReferenceId));
				}
				else
				{
					Items.FirstOrDefault(x => x.ItemReferenceId == salesCustomerProduct.ItemReferenceId).IsSelected = true;
					SelectedItems.Add(selectedItem);

					var outputSalesBasketModelItem = new OutputSalesBasketModel
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
						MainItemReferenceId = selectedItem.ItemReferenceId,
						MainItemCode = selectedItem.ItemCode,
						MainItemName = selectedItem.ItemName,
						StockQuantity = default,
						IsSelected = false,
						TrackingType = selectedItem.TrackingType,
						LocTracking = selectedItem.LocTracking,
						Image = selectedItem.ImageData,
						Quantity = selectedItem.WaitingQuantity,
					};
					if (selectedItem.LocTracking == 1 || selectedItem.TrackingType == 1)
						outputSalesBasketModelItem.OutputQuantity = 0;
					else
						outputSalesBasketModelItem.OutputQuantity = 1;

					BasketItems.Add(outputSalesBasketModelItem);
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

	private async Task ConfirmAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var previousViewModel = _serviceProvider.GetRequiredService<OutputProductSalesOrderProcessBasketListViewModel>();
			if (BasketItems.Count > 0)
			{
				_userDialogs.Loading("Ürünler sepete ekleniyor...");
				if (previousViewModel is not null)
				{
					foreach (var basketItem in BasketItems)
					{
						var productOrdersResult = await _waitingSalesOrderService.GetObjectsByProduct(
							httpClient: httpClient,
									firmNumber: _httpClientService.FirmNumber,
									periodNumber: _httpClientService.PeriodNumber,
									warehouseNumber: WarehouseModel.Number,
									customerReferenceId: SalesCustomer.ReferenceId,
									productReferenceId: basketItem.ItemReferenceId,
									shipInfoReferenceId: SalesCustomer.ShipAddressReferenceId,
									skip: 0,
									take: 99999999
						);

						if (productOrdersResult.IsSuccess)
						{
							basketItem.Orders.Clear();
							if (productOrdersResult.Data is not null)
							{
								foreach (var productOrder in productOrdersResult.Data)
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
										Price = productOrder.Price,
										Vat = productOrder.Vat
									});

								}
							}
						}

						if (!previousViewModel.Items.Any(x => x.ItemReferenceId == basketItem.ItemReferenceId))
						{
							previousViewModel.Items.Add(basketItem);
						}
						else
						{
							foreach (var order in basketItem.Orders)
							{
								var existingItem = previousViewModel.Items.FirstOrDefault(x => x.ItemReferenceId == basketItem.ItemReferenceId);
								if (!existingItem.Orders.Any(x => x.ReferenceId == order.ReferenceId))
								{
									existingItem.Orders.Add(order);
								}

							}

						}
					}
					BasketItems.Clear();
					SelectedItems.Clear();

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

	//   private async Task ItemTappedAsync(SalesCustomerProduct salesCustomerProduct)
	//{
	//	if (IsBusy)
	//		return;

	//	try
	//	{
	//		IsBusy = true;

	//		var selectedItem = Items.FirstOrDefault(x => x.ItemReferenceId == salesCustomerProduct.ItemReferenceId);

	//		if (selectedItem is not null)
	//		{
	//			if (selectedItem.IsSelected)
	//			{
	//				Items.FirstOrDefault(x => x.ItemReferenceId == salesCustomerProduct.ItemReferenceId).IsSelected = false;
	//				SelectedProducts.Remove(SelectedProducts.FirstOrDefault(x => x.ItemReferenceId == selectedItem.ItemReferenceId));
	//				SelectedItems.Remove(SelectedItems.FirstOrDefault(x => x.ItemReferenceId == selectedItem.ItemReferenceId));
	//               }
	//			else
	//			{
	//				Items.FirstOrDefault(x => x.ItemReferenceId == salesCustomerProduct.ItemReferenceId).IsSelected = true;

	//				//var basketItem = new OutputSalesBasketModel
	//				//{
	//				//	ItemReferenceId = selectedItem.ItemReferenceId,
	//				//	ItemCode = selectedItem.ItemCode,
	//				//	ItemName = selectedItem.ItemName,
	//				//	IsVariant = selectedItem.IsVariant,
	//				//	UnitsetReferenceId = selectedItem.UnitsetReferenceId,
	//				//	UnitsetCode = selectedItem.UnitsetCode,
	//				//	UnitsetName = selectedItem.UnitsetName,
	//				//	SubUnitsetReferenceId = selectedItem.SubUnitsetReferenceId,
	//				//	SubUnitsetCode = selectedItem.SubUnitsetCode,
	//				//	SubUnitsetName = selectedItem.SubUnitsetName,
	//				//	MainItemReferenceId = selectedItem.MainItemReferenceId,
	//				//	MainItemCode = selectedItem.MainItemCode,
	//				//	MainItemName = selectedItem.MainItemName,
	//				//	StockQuantity = default,
	//				//	IsSelected = false,
	//				//	TrackingType = selectedItem.TrackingType,
	//				//	LocTracking = selectedItem.LocTracking,
	//				//	Image = string.Empty,
	//				//	Quantity = selectedItem.WaitingQuantity,
	//				//};

	//				//if (selectedItem.LocTracking == 1 || selectedItem.TrackingType == 1)
	//				//	basketItem.OutputQuantity = 0;
	//				//else
	//				//	basketItem.OutputQuantity = 1;

	//				SelectedProducts.Add(selectedItem);
	//				SelectedItems.Add(selectedItem);
	//               }
	//		}
	//	}
	//	catch (Exception ex)
	//	{
	//		if (_userDialogs.IsHudShowing)
	//			_userDialogs.HideHud();

	//		await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
	//	}
	//	finally
	//	{
	//		IsBusy = false;
	//	}
	//}

	//private async Task<ObservableCollection<OutputSalesBasketModel>> ConvertProductItems()
	//{
	//	return await Task.Run(async () =>
	//	{
	//		ObservableCollection<OutputSalesBasketModel> basketItems = new();
	//		foreach (var item in SelectedProducts)
	//		{
	//			var basketItem = new OutputSalesBasketModel
	//			{
	//				ItemReferenceId = item.ItemReferenceId,
	//				ItemCode = item.ItemCode,
	//				ItemName = item.ItemName,
	//				IsVariant = item.IsVariant,
	//				UnitsetReferenceId = item.UnitsetReferenceId,
	//				UnitsetCode = item.UnitsetCode,
	//				UnitsetName = item.UnitsetName,
	//				SubUnitsetReferenceId = item.SubUnitsetReferenceId,
	//				SubUnitsetCode = item.SubUnitsetCode,
	//				SubUnitsetName = item.SubUnitsetName,
	//				MainItemReferenceId = item.MainItemReferenceId,
	//				MainItemCode = item.MainItemCode,
	//				MainItemName = item.MainItemName,
	//				StockQuantity = default,
	//				IsSelected = false,
	//				TrackingType = item.TrackingType,
	//				LocTracking = item.LocTracking,
	//				Image = string.Empty,
	//				Quantity = item.WaitingQuantity,
	//				LocTrackingIcon = item.LocTrackingIcon,
	//				TrackingTypeIcon = item.TrackingTypeIcon,
	//				VariantIcon = item.VariantIcon,
	//			};

	//			if (item.LocTracking == 1 || item.TrackingType == 1)
	//				basketItem.OutputQuantity = 0;
	//			else
	//				basketItem.OutputQuantity = 1;

	//			var httpClient = _httpClientService.GetOrCreateHttpClient();
	//			var productOrders = await _waitingSalesOrderService.GetObjectsByProduct(
	//								httpClient: httpClient,
	//								firmNumber: _httpClientService.FirmNumber,
	//								periodNumber: _httpClientService.PeriodNumber,
	//								warehouseNumber: WarehouseModel.Number,
	//								customerReferenceId: SalesCustomer.ReferenceId,
	//								productReferenceId: basketItem.ItemReferenceId,
	//								skip: 0,
	//								take: 99999999
	//			);

	//			if (productOrders.IsSuccess)
	//			{
	//				if (productOrders.Data is not null)
	//				{
	//					foreach (var productOrder in productOrders.Data)
	//					{
	//						basketItem.Orders.Add(new OutputSalesBasketOrderModel
	//						{
	//							ReferenceId = productOrder.ReferenceId,
	//							OrderReferenceId = productOrder.OrderReferenceId,
	//							CustomerReferenceId = productOrder.CustomerReferenceId,
	//							CustomerCode = productOrder.CustomerCode,
	//							CustomerName = productOrder.CustomerName,
	//							ProductReferenceId = productOrder.ProductReferenceId,
	//							ProductCode = productOrder.ProductCode,
	//							ProductName = productOrder.ProductName,
	//							UnitsetReferenceId = productOrder.UnitsetReferenceId,
	//							UnitsetCode = productOrder.UnitsetCode,
	//							UnitsetName = productOrder.UnitsetName,
	//							SubUnitsetReferenceId = productOrder.SubUnitsetReferenceId,
	//							SubUnitsetCode = productOrder.SubUnitsetCode,
	//							SubUnitsetName = productOrder.SubUnitsetName,
	//							Quantity = productOrder.Quantity,
	//							ShippedQuantity = productOrder.ShippedQuantity,
	//							WaitingQuantity = productOrder.WaitingQuantity,
	//							OrderDate = productOrder.OrderDate,
	//							DueDate = productOrder.DueDate,
	//						});
	//					}
	//				}
	//			}

	//			basketItems.Add(basketItem);
	//		}

	//		return basketItems;
	//	});
	//}
}