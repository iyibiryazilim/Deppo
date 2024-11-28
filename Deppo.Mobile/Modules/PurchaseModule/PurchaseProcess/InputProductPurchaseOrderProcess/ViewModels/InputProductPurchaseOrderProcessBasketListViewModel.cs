using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BarcodeModels;
using Deppo.Mobile.Core.Models.CameraModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.BarcodeHelper;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CameraModule.Views;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(PurchaseSupplier), queryId: nameof(PurchaseSupplier))]
public partial class InputProductPurchaseOrderProcessBasketListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	private readonly ILocationService _locationService;
	private readonly ISeriLotService _seriLotService;
	private readonly IServiceProvider _serviceProvider;
	private readonly IBarcodeSearchPurchaseHelper _barcodeSearchPurchaseHelper;
	private readonly IWaitingPurchaseOrderService _waitingPurchaseOrderService;
	private readonly ISubUnitsetService _subUnitsetService;

	[ObservableProperty]
	private WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	private PurchaseSupplier purchaseSupplier = null!;

	[ObservableProperty]
	private InputPurchaseBasketModel? selectedInputPurchaseBasketModel;

	[ObservableProperty]
	public ObservableCollection<InputPurchaseBasketModel> items = new();

	[ObservableProperty]
	public Entry barcodeEntry;

	public InputProductPurchaseOrderProcessBasketListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, ILocationService locationService, ISeriLotService seriLotService, IServiceProvider serviceProvider, ISubUnitsetService subUnitsetService, IBarcodeSearchPurchaseHelper barcodeSearchPurchaseHelper, IWaitingPurchaseOrderService waitingPurchaseOrderService)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_locationService = locationService;
		_seriLotService = seriLotService;
		_serviceProvider = serviceProvider;
		_subUnitsetService = subUnitsetService;
		_barcodeSearchPurchaseHelper = barcodeSearchPurchaseHelper;
		_waitingPurchaseOrderService = waitingPurchaseOrderService;

		Title = "Satınalma Sepeti";

		PerformSearchCommand = new Command<Entry>(async (x) => await PerformSearchAsync(x));
		QuantityTappedCommand = new Command<InputPurchaseBasketModel>(async (x) => await QuantityTappedAsync(x));
		IncreaseCommand = new Command<InputPurchaseBasketModel>(async (x) => await IncreaseAsync(x));
		DecreaseCommand = new Command<InputPurchaseBasketModel>(async (x) => await DecreaseAsync(x));
		DeleteItemCommand = new Command<InputPurchaseBasketModel>(async (x) => await DeleteItemAsync(x));

		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());

		PlusTappedCommand = new Command(async () => await PlusTappedAsync());
		ProductOptionTappedCommand = new Command(async () => await ProductOptionTappedAsync());
		OrderOptionTappedCommand = new Command(async () => await OrderOptionTappedAsync());

		ShowOtherProductCommand = new Command(async () => await ShowOtherProductAsync());
		CameraTappedCommand = new Command(async () => await CameraTappedAsync());

		UnitActionTappedCommand = new Command<InputPurchaseBasketModel>(async (x) => await UnitActionTappedAsync(x));
		SubUnitsetTappedCommand = new Command<SubUnitset>(async (x) => await SubUnitsetTappedAsync(x));
		
	}

	public Page CurrentPage { get; set; }

	#region Commands

	public Command PerformSearchCommand { get; }
	public Command<InputPurchaseBasketModel> QuantityTappedCommand { get; }
	public Command<InputPurchaseBasketModel> DeleteItemCommand { get; }
	public Command<InputPurchaseBasketModel> IncreaseCommand { get; }
	public Command<InputPurchaseBasketModel> DecreaseCommand { get; }

	public Command NextViewCommand { get; }
	public Command BackCommand { get; }
	public Command PlusTappedCommand { get; }
	public Command ProductOptionTappedCommand { get; }
	public Command OrderOptionTappedCommand { get; }
	public Command CameraTappedCommand { get; }

	public Command ShowOtherProductCommand { get; }

	public Command UnitActionTappedCommand { get; }
	public Command SubUnitsetTappedCommand { get; }

	#endregion Commands

	private async Task UnitActionTappedAsync(InputPurchaseBasketModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			SelectedInputPurchaseBasketModel = item;
			await LoadSubUnitsetsAsync(item);
			CurrentPage.FindByName<BottomSheet>("subUnitsetBottomSheet").State = BottomSheetState.HalfExpanded;
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

	private async Task LoadSubUnitsetsAsync(InputPurchaseBasketModel item)
	{
		if (item is null)
			return;
		try
		{
			item.SubUnitsets.Clear();

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _subUnitsetService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: item.ItemReferenceId
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var subUnitset in result.Data)
				{
					item.SubUnitsets.Add(Mapping.Mapper.Map<SubUnitset>(subUnitset));
				}
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task SubUnitsetTappedAsync(SubUnitset subUnitset)
	{
		if (subUnitset is null)
			return;
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (SelectedInputPurchaseBasketModel is not null)
			{
				SelectedInputPurchaseBasketModel.SubUnitsetReferenceId = subUnitset.ReferenceId;
				SelectedInputPurchaseBasketModel.SubUnitsetName = subUnitset.Name;
				SelectedInputPurchaseBasketModel.SubUnitsetCode = subUnitset.Code;
				SelectedInputPurchaseBasketModel.ConversionFactor = subUnitset.ConversionValue;
				SelectedInputPurchaseBasketModel.OtherConversionFactor = subUnitset.OtherConversionValue;
			}

			CurrentPage.FindByName<BottomSheet>("subUnitsetBottomSheet").State = BottomSheetState.Hidden;
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

	private async Task ShowProductViewAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(InputProductProcessProductListView)}", new Dictionary<string, object>
			{
				{nameof(WarehouseModel), WarehouseModel}
			});
		}
		catch (Exception ex)
		{
			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task PerformSearchAsync(Entry barcodeEntry)
	{
		if (IsBusy)
			return;
		try
		{
			if (string.IsNullOrEmpty(barcodeEntry.Text))
				return;

			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _barcodeSearchPurchaseHelper.BarcodeDetectedAsync(
			  httpClient: httpClient,
			  firmNumber: _httpClientService.FirmNumber,
			  periodNumber: _httpClientService.PeriodNumber,
			  barcode: barcodeEntry.Text,
			  warehouseNumber: WarehouseModel.Number,
			  supplierReferenceId: PurchaseSupplier.ReferenceId,
			  shipInfoReferenceId: PurchaseSupplier.ShipAddressReferenceId
		   );

			if (result is not null)
			{
				Type resultType = result.GetType();
				if (resultType == typeof(BarcodePurchaseProductModel))
				{
					var existingItem = Items.FirstOrDefault(x => x.ItemReferenceId == result.ItemReferenceId);

					if (existingItem is not null)
					{
						// Orders'ları kontrol et ve eksik olanları ekle
						var existingItemFullOrders = await _waitingPurchaseOrderService.GetObjectsByProduct(
							httpClient: httpClient,
							firmNumber: _httpClientService.FirmNumber,
							periodNumber: _httpClientService.PeriodNumber,
							warehouseNumber: WarehouseModel.Number,
							productReferenceId: result.ItemReferenceId,
							supplierReferenceId: PurchaseSupplier.ReferenceId,
							skip: 0,
							take: 999999
						);

						if (existingItemFullOrders.IsSuccess && existingItemFullOrders.Data is not null)
						{
							foreach (var order in existingItemFullOrders.Data)
							{
								var obj = Mapping.Mapper.Map<InputPurchaseBasketOrderModel>(order);
								if (!existingItem.Orders.Any(x => x.ReferenceId == obj.ReferenceId))
								{
									existingItem.Orders.Add(new InputPurchaseBasketOrderModel
									{
										ReferenceId = obj.ReferenceId,
										OrderReferenceId = obj.OrderReferenceId,
										SupplierReferenceId = obj.SupplierReferenceId,
										SupplierCode = obj.SupplierCode,
										SupplierName = obj.SupplierName,
										ProductReferenceId = obj.ProductReferenceId,
										ProductCode = obj.ProductCode,
										ProductName = obj.ProductName,
										UnitsetReferenceId = obj.UnitsetReferenceId,
										UnitsetCode = obj.UnitsetCode,
										UnitsetName = obj.UnitsetName,
										SubUnitsetReferenceId = obj.SubUnitsetReferenceId,
										SubUnitsetCode = obj.SubUnitsetCode,
										SubUnitsetName = obj.SubUnitsetName,
										Quantity = obj.Quantity,
										ShippedQuantity = obj.ShippedQuantity,
										WaitingQuantity = obj.WaitingQuantity,
										OrderDate = obj.OrderDate,
										DueDate = obj.DueDate,
									});
								}
							}
						}
					}
					else
					{
						var inputPurchaseBasketModelItem = new InputPurchaseBasketModel
						{
							ItemReferenceId = result.ItemReferenceId,
							ItemCode = result.ItemCode,
							ItemName = result.ItemName,
							UnitsetReferenceId = result.UnitsetReferenceId,
							UnitsetCode = result.UnitsetCode,
							UnitsetName = result.UnitsetName,
							SubUnitsetReferenceId = result.SubUnitsetReferenceId,
							SubUnitsetCode = result.SubUnitsetCode,
							SubUnitsetName = result.SubUnitsetName,
							MainItemReferenceId = result.ItemReferenceId,
							MainItemCode = result.ItemCode,
							MainItemName = result.ItemName,
							StockQuantity = default,
							IsSelected = false,
							IsVariant = result.IsVariant,
							TrackingType = result.TrackingType,
							LocTracking = result.LocTracking,
							Image = result.ImageData,
							Quantity = result.WaitingQuantity,
							InputQuantity = result.LocTracking == 0 ? 1 : 0,
						};

						var itemOrders = await _waitingPurchaseOrderService.GetObjectsByProduct(
							httpClient: httpClient,
							firmNumber: _httpClientService.FirmNumber,
							periodNumber: _httpClientService.PeriodNumber,
							warehouseNumber: WarehouseModel.Number,
							productReferenceId: inputPurchaseBasketModelItem.ItemReferenceId,
							supplierReferenceId: PurchaseSupplier.ReferenceId,
							skip: 0,
							take: 999999
						);

						if (itemOrders.IsSuccess)
						{
							if (itemOrders.Data is not null)
							{
								foreach (var order in itemOrders.Data)
								{
									var obj = Mapping.Mapper.Map<InputPurchaseBasketOrderModel>(order);
									inputPurchaseBasketModelItem.Orders.Add(new InputPurchaseBasketOrderModel
									{
										ReferenceId = obj.ReferenceId,
										OrderReferenceId = obj.OrderReferenceId,
										SupplierReferenceId = obj.CustomerReferenceId,
										SupplierCode = obj.CustomerCode,
										SupplierName = obj.CustomerName,
										ProductReferenceId = obj.ProductReferenceId,
										ProductCode = obj.ProductCode,
										ProductName = obj.ProductName,
										UnitsetReferenceId = obj.UnitsetReferenceId,
										UnitsetCode = obj.UnitsetCode,
										UnitsetName = obj.UnitsetName,
										SubUnitsetReferenceId = obj.SubUnitsetReferenceId,
										SubUnitsetCode = obj.SubUnitsetCode,
										SubUnitsetName = obj.SubUnitsetName,
										Quantity = obj.Quantity,
										ShippedQuantity = obj.ShippedQuantity,
										WaitingQuantity = obj.WaitingQuantity,
										OrderDate = obj.OrderDate,
										DueDate = obj.DueDate,
									});

								}
							}
						}

						Items.Add(inputPurchaseBasketModelItem);
					}
				}
			}
			else
			{
				_userDialogs.ShowToast($"{barcodeEntry.Text} barkodunda herhangi bir ürün bulunamadı");
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
			BarcodeEntry.Text = string.Empty;
			barcodeEntry.Text = string.Empty;
			barcodeEntry.Focus();
			IsBusy = false;
		}
	}

	private async Task QuantityTappedAsync(InputPurchaseBasketModel inputPurchaseBasketModel)
	{
		if (IsBusy)
			return;
		if (inputPurchaseBasketModel is null)
			return;
		if (inputPurchaseBasketModel.LocTracking == 1)
			return;
		try
		{
			IsBusy = true;

			var result = await CurrentPage.DisplayPromptAsync(
				title: inputPurchaseBasketModel.ItemCode,
				message: "Miktarı giriniz",
				cancel: "Vazgeç",
				accept: "Tamam",
				placeholder: inputPurchaseBasketModel.InputQuantity.ToString(),
				keyboard: Keyboard.Numeric);

			if (string.IsNullOrEmpty(result))
				return;

			var quantity = Convert.ToDouble(result);

			if (quantity < 0)
			{
				await _userDialogs.AlertAsync("Miktar sıfırdan küçük olmamalıdır.", "Hata", "Tamam");
				return;
			}

			inputPurchaseBasketModel.InputQuantity = quantity;
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

	private async Task IncreaseAsync(InputPurchaseBasketModel inputPurchaseBasketModel)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			SelectedInputPurchaseBasketModel = inputPurchaseBasketModel;
			if (inputPurchaseBasketModel.LocTracking == 1)
			{
				var nextViewModel = _serviceProvider.GetRequiredService<InputProductPurchaseOrderProcessBasketLocationListViewModel>();

				nextViewModel.WarehouseModel = WarehouseModel;
				nextViewModel.InputPurchaseBasketModel = inputPurchaseBasketModel;

				await nextViewModel.LoadSelectedItemsAsync();

				await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseOrderProcessBasketLocationListView)}", new Dictionary<string, object>
				{
					{nameof(WarehouseModel), WarehouseModel},
					{nameof(InputPurchaseBasketModel), inputPurchaseBasketModel}
				});
			}

			// Sadece SeriLot takipli ise
			else if (inputPurchaseBasketModel.LocTracking == 0 && (inputPurchaseBasketModel.TrackingType == 1 || inputPurchaseBasketModel.TrackingType == 2))
			{
				await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseOrderProcessBasketSeriLotListView)}", new Dictionary<string, object>
				{
					 {nameof(WarehouseModel), WarehouseModel},
					{nameof(InputPurchaseBasketModel), inputPurchaseBasketModel}
				});
			}
			//stok yeri ve serilot takipli değilse
			else
			{
				if(inputPurchaseBasketModel.Quantity > inputPurchaseBasketModel.InputQuantity)
					inputPurchaseBasketModel.InputQuantity++;
			}
		}
		catch (Exception ex)
		{
			await _userDialogs.AlertAsync(ex.Message);
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task DecreaseAsync(InputPurchaseBasketModel inputPurchaseBasketModel)
	{

        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            SelectedInputPurchaseBasketModel = inputPurchaseBasketModel;
            if (inputPurchaseBasketModel.LocTracking == 1)
            {
                var nextViewModel = _serviceProvider.GetRequiredService<InputProductPurchaseOrderProcessBasketLocationListViewModel>();


				nextViewModel.WarehouseModel = WarehouseModel;
				nextViewModel.InputPurchaseBasketModel = inputPurchaseBasketModel;
				await nextViewModel.LoadSelectedItemsAsync();

				await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseOrderProcessBasketLocationListView)}", new Dictionary<string, object>
                {
                    {nameof(WarehouseModel), WarehouseModel},
                    {nameof(InputPurchaseBasketModel), inputPurchaseBasketModel}
                });
            }

            // Sadece SeriLot takipli ise
            else if (inputPurchaseBasketModel.LocTracking == 0 && (inputPurchaseBasketModel.TrackingType == 1 || inputPurchaseBasketModel.TrackingType == 2))
            {
                await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseOrderProcessBasketSeriLotListView)}", new Dictionary<string, object>
                {
                     {nameof(WarehouseModel), WarehouseModel},
                    {nameof(InputPurchaseBasketModel), inputPurchaseBasketModel}
                });
            }
            //stok yeri ve serilot takipli değilse
            else
            {
                if (inputPurchaseBasketModel.Quantity > inputPurchaseBasketModel.InputQuantity)
                    inputPurchaseBasketModel.InputQuantity--;
            }
        }
        catch (Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

	private async Task DeleteItemAsync(InputPurchaseBasketModel item)
	{
		if (item is null)
			return;
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var result = await _userDialogs.ConfirmAsync($"{item.ItemCode}\n{item.ItemName}\nİlgili ürün sepetinizden çıkarılacaktır. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
			if (!result)
				return;

			item.Details.Clear();

			Items.Remove(item);
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

			if (Items.Count == 0)
			{
				await _userDialogs.AlertAsync("Sepetinizde ürün bulunmamaktadır.", "Hata", "Tamam");
				return;
			}

			if (Items.Any(x => x.InputQuantity == 0))
			{
				await _userDialogs.AlertAsync("Sepetinizde miktarı sıfır olan ürünler bulunmakta. Lütfen yeniden düzenleme yapınız", "Uyarı", "Tamam");
				return;
			}

			await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseOrderProcessFormView)}", new Dictionary<string, object>
			{
				[nameof(WarehouseModel)] = WarehouseModel,
				[nameof(PurchaseSupplier)] = PurchaseSupplier,
				[nameof(Items)] = Items
			});
            CurrentPage.FindByName<Entry>("barcodeEntry").Text = string.Empty;

        }
        catch (Exception ex)
		{
			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task ShowOtherProductAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseOrderProcessOtherProductListView)}", new Dictionary<string, object>
			{
				{nameof(WarehouseModel), WarehouseModel},
				{nameof(PurchaseSupplier),PurchaseSupplier }
			});
		}
		catch (Exception ex)
		{
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
			if (Items.Count > 0)
			{
				var result = await _userDialogs.ConfirmAsync("Sepetinizdeki ürünler silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
				if (!result)
					return;

				foreach (var item in Items)
					item.Details.Clear();

				Items.Clear();

				await Shell.Current.GoToAsync("..");
			}
			else
				await Shell.Current.GoToAsync("..");

            CurrentPage.FindByName<Entry>("barcodeEntry").Text = string.Empty;

        }
        catch (Exception ex)
		{
			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task CameraTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			CameraScanModel cameraScanModel = new CameraScanModel
			{
				ComingPage = "InputProductPurchaseOrderProcessBasketListViewModel",
				WarehouseNumber = WarehouseModel.Number,
				CurrentReferenceId = PurchaseSupplier.ReferenceId,
				ShipInfoReferenceId = PurchaseSupplier.ShipAddressReferenceId
			};

			await Shell.Current.GoToAsync($"{nameof(CameraReaderView)}", new Dictionary<string, object>
			{
				[nameof(CameraScanModel)] = cameraScanModel
			});
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

	private async Task PlusTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			CurrentPage.FindByName<BottomSheet>("basketOptionsBottomSheet").State = BottomSheetState.HalfExpanded;
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

	private async Task ProductOptionTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			CurrentPage.FindByName<BottomSheet>("basketOptionsBottomSheet").State = BottomSheetState.Hidden;

			await Task.Delay(300);
			await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseOrderProcessProductListView)}", new Dictionary<string, object>
			{
				[nameof(WarehouseModel)] = WarehouseModel,
				[nameof(PurchaseSupplier)] = PurchaseSupplier
			});
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

	private async Task OrderOptionTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			CurrentPage.FindByName<BottomSheet>("basketOptionsBottomSheet").State = BottomSheetState.Hidden;
			await Task.Delay(300);

			await Shell.Current.GoToAsync($"{nameof(InputProductPurchaseOrderProcessOrderListView)}", new Dictionary<string, object>
			{
				[nameof(WarehouseModel)] = WarehouseModel,
				[nameof(PurchaseSupplier)] = PurchaseSupplier
			});
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
}