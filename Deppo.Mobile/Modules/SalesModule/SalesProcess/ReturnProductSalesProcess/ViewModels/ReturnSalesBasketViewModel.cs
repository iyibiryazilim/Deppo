using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BarcodeModels;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.CameraModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.BarcodeHelper;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CameraModule.Views;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.Views;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class ReturnSalesBasketViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	private readonly ILocationService _locationService;
	private readonly ISeriLotService _seriLotService;
	private readonly IServiceProvider _serviceProvider;
	private readonly IBarcodeSearchHelper _barcodeSearchHelper;
	private readonly ISubUnitsetService _subUnitsetService;

	[ObservableProperty]
	private WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	private ReturnSalesBasketModel? selectedReturnSalesBasketModel;

	public ObservableCollection<ReturnSalesBasketModel> Items { get; } = new();
	public ObservableCollection<LocationModel> Locations { get; }

	[ObservableProperty]
	public Entry barcodeEntry;

	public ReturnSalesBasketViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IHttpClientService httpClientService2, ILocationService locationService, ISeriLotService seriLotService, IServiceProvider serviceProvider, IBarcodeSearchHelper barcodeSearchHelper, ISubUnitsetService subUnitsetService)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_locationService = locationService;
		_seriLotService = seriLotService;
		_serviceProvider = serviceProvider;
		_barcodeSearchHelper = barcodeSearchHelper;
		_subUnitsetService = subUnitsetService;

		Title = "Sepet Listesi";

		ShowProductViewCommand = new Command(async () => await ShowProductViewAsync());
		PerformSearchCommand = new Command<Entry>(async (barcodeEntry) => await PerformSearchAsync(barcodeEntry));
		UnitActionTappedCommand = new Command<ReturnSalesBasketModel>(async (item) => await UnitActionTappedAsync(item));
		SubUnitsetTappedCommand = new Command<SubUnitset>(async (subUnitset) => await SubUnitsetTappedAsync(subUnitset));

		QuantityTappedCommand = new Command<ReturnSalesBasketModel>(async (item) => await QuantityTappedAsync(item));
		DeleteItemCommand = new Command<ReturnSalesBasketModel>(async (item) => await DeleteItemAsync(item));
		IncreaseCommand = new Command<ReturnSalesBasketModel>(async (item) => await IncreaseAsync(item));
		DecreaseCommand = new Command<ReturnSalesBasketModel>(async (item) => await DecreaseAsync(item));

		LocationCloseCommand = new Command(async () => await LocationCloseAsync());
		LocationConfirmCommand = new Command<LocationModel>(async (locationModel) => await LocationConfirmAsync(locationModel));
		LocationIncreaseCommand = new Command<LocationModel>(async (locationModel) => await LocationIncreaseAsync(locationModel));
		LocationDecreaseCommand = new Command<LocationModel>(async (LocationModel) => await LocationDecreaseAsync(LocationModel));

		LocationTransactionCloseCommand = new Command(async () => await LocationTransactionCloseAsync());

		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());
		CameraTappedCommand = new Command(async () => await CameraTappedAsync());

		Items.Clear();
	}

	public Page CurrentPage { get; set; } = null!;

	public Command ShowProductViewCommand { get; }
	public Command PerformSearchCommand { get; }
	public Command UnitActionTappedCommand { get; }
	public Command SubUnitsetTappedCommand { get; }

	public Command<ReturnSalesBasketModel> QuantityTappedCommand { get; }
	public Command<ReturnSalesBasketModel> DeleteItemCommand { get; }
	public Command<ReturnSalesBasketModel> IncreaseCommand { get; }
	public Command<ReturnSalesBasketModel> DecreaseCommand { get; }

	public Command<LocationModel> LocationDecreaseCommand { get; }
	public Command<LocationModel> LocationIncreaseCommand { get; }
	public Command<LocationModel> LocationConfirmCommand { get; }
	public Command LocationCloseCommand { get; }

	public Command NextViewCommand { get; }
	public Command BackCommand { get; }
	public Command CameraTappedCommand { get; }

	public Command LocationTransactionCloseCommand { get; }


	private async Task ShowProductViewAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;
			var viewModel = _serviceProvider.GetRequiredService<ReturnSalesProductListViewModel>();
			await viewModel.LoadPageAsync();
			await Shell.Current.GoToAsync($"{nameof(ReturnSalesProductListView)}", new Dictionary<string, object>
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

			var result = await _barcodeSearchHelper.BarcodeDetectedAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				barcode: barcodeEntry.Text,
				comingPage: "ReturnSalesBasketViewModel"
			);

			if (result is not null)
			{
				Type resultType = result.GetType();
				if (resultType == typeof(BarcodeInProductModel))
				{
					if (Items.Any(x => x.ItemCode == result.Code))
					{
						_userDialogs.ShowToast($"{barcodeEntry.Text} barkodlu ürün sepetinizde zaten bulunmaktadır.");
						return;
					}
					else
					{
						var basketItem = new ReturnSalesBasketModel
						{
							ReferenceId = Guid.NewGuid(),
							ItemReferenceId = result.ReferenceId,
							ItemCode = result.Code,
							ItemName = result.Name,
							Image = result.ImageData,
							UnitsetReferenceId = result.UnitsetReferenceId,
							UnitsetCode = result.UnitsetCode,
							UnitsetName = result.UnitsetName,
							SubUnitsetReferenceId = result.SubUnitsetReferenceId,
							SubUnitsetCode = result.SubUnitsetCode,
							SubUnitsetName = result.SubUnitsetName,
							IsSelected = false,
							MainItemCode = string.Empty,
							MainItemName = string.Empty,
							MainItemReferenceId = default,
							StockQuantity = result.StockQuantity,
							Quantity = result.LocTracking == 0 ? 1 : 0,
							InputQuantity = result.LocTracking == 0 ? 1 : 0,
							LocTracking = result.LocTracking,
							TrackingType = result.TrackingType,
							IsVariant = result.IsVariant,
							VariantIcon = result.VariantIcon,
							LocTrackingIcon = result.LocTrackingIcon,
							TrackingTypeIcon = result.TrackingTypeIcon
						};

						Items.Add(basketItem);
					}
				}
				else if (resultType == typeof(VariantModel))
				{
					if (Items.Any(x => x.ItemCode == result.Code))
					{
						_userDialogs.ShowToast($"{barcodeEntry.Text} barkodlu ürün sepetinizde zaten bulunmaktadır.");
						return;
					}
					else
					{
						var basketItem = new ReturnSalesBasketModel
						{
							ReferenceId = Guid.NewGuid(),
							ItemReferenceId = result.ReferenceId,
							ItemCode = result.Code,
							ItemName = result.Name,
							Image = result.ImageData,
							UnitsetReferenceId = result.UnitsetReferenceId,
							UnitsetCode = result.UnitsetCode,
							UnitsetName = result.UnitsetName,
							SubUnitsetReferenceId = result.SubUnitsetReferenceId,
							SubUnitsetCode = result.SubUnitsetCode,
							SubUnitsetName = result.SubUnitsetName,
							IsSelected = false,
							MainItemCode = result.ProductCode,
							MainItemName = result.ProductName,
							MainItemReferenceId = result.ProductReferenceId,
							StockQuantity = result.StockQuantity,
							TrackingType = result.TrackingType,
							LocTracking = result.LocTracking,
							IsVariant = true,
							InputQuantity = result.LocTracking == 0 ? 1 : 0,
							Quantity = result.LocTracking == 0 ? 1 : 0,
						};

						Items.Add(basketItem);
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

	private async Task LoadSubUnitsetsAsync(ReturnSalesBasketModel item)
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

			if (SelectedReturnSalesBasketModel is not null)
			{
				SelectedReturnSalesBasketModel.SubUnitsetReferenceId = subUnitset.ReferenceId;
				SelectedReturnSalesBasketModel.SubUnitsetName = subUnitset.Name;
				SelectedReturnSalesBasketModel.SubUnitsetCode = subUnitset.Code;
				SelectedReturnSalesBasketModel.ConversionFactor = subUnitset.ConversionValue;
				SelectedReturnSalesBasketModel.OtherConversionFactor = subUnitset.OtherConversionValue;
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

	private async Task UnitActionTappedAsync(ReturnSalesBasketModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			SelectedReturnSalesBasketModel = item;
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

	private async Task QuantityTappedAsync(ReturnSalesBasketModel returnSalesBasketModel)
	{
		if (IsBusy)
			return;
		if (returnSalesBasketModel is null)
			return;
		if (returnSalesBasketModel.LocTracking == 1)
			return;
		try
		{
			IsBusy = true;

			var result = await CurrentPage.DisplayPromptAsync(
				title: returnSalesBasketModel.ItemCode,
				message: "Miktarı giriniz",
				cancel: "Vazgeç",
				accept: "Tamam",
				placeholder: returnSalesBasketModel.Quantity.ToString(),
				keyboard: Keyboard.Numeric);

			if (string.IsNullOrEmpty(result))
				return;

			var quantity = Convert.ToDouble(result);

			if (quantity < 0)
			{
				await _userDialogs.AlertAsync("Miktar sıfırdan küçük olmamalıdır.", "Hata", "Tamam");
				return;
			}

			returnSalesBasketModel.Quantity = quantity;
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
	private async Task DeleteItemAsync(ReturnSalesBasketModel item)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			var result = await _userDialogs.ConfirmAsync($"{item.ItemCode}\n{item.ItemName}\nİlgili ürün sepetinizden çıkarılacaktır. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
			if (!result)
				return;

			Items.Remove(item);
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

	private async Task IncreaseAsync(ReturnSalesBasketModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			SelectedReturnSalesBasketModel = item;

			if (item.LocTracking == 1)
			{
				var nextViewModel = _serviceProvider.GetRequiredService<ReturnSalesBasketLocationListViewModel>();

				nextViewModel.ReturnSalesBasketModel = item;

				await nextViewModel.LoadSelectedItemsAsync();

				await Shell.Current.GoToAsync($"{nameof(ReturnSalesBasketLocationListView)}", new Dictionary<string, object>
				{
					{nameof(WarehouseModel), WarehouseModel},
					{nameof(ReturnSalesBasketModel), item}
				});
			}
			else
				item.Quantity++;
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

	private async Task DecreaseAsync(ReturnSalesBasketModel item)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			if (item is not null)
			{
				if (item.Quantity > 1)
				{
					// Stok Yeri takipli ise locationTransactionBottomSheet aç
					if (item.LocTracking == 1)
					{
						var nextViewModel = _serviceProvider.GetRequiredService<ReturnSalesBasketLocationListViewModel>();

						await Shell.Current.GoToAsync($"{nameof(ReturnSalesBasketLocationListView)}", new Dictionary<string, object>
					{
						{nameof(WarehouseModel), WarehouseModel},
						{nameof(ReturnSalesBasketModel), item}
						});

						await nextViewModel.LoadSelectedItemsAsync();
					}
					// Sadece SeriLot takipli ise serilotTransactionBottomSheet aç
					else if (item.LocTracking == 0 && (item.TrackingType == 1 || item.TrackingType == 2))
					{
						await Shell.Current.GoToAsync($"{nameof(ReturnSalesBasketSeriLotListView)}", new Dictionary<string, object>
					{
						{nameof(WarehouseModel), WarehouseModel},
						{nameof(ReturnSalesBasketModel), item}
					});
					}
					// Stok yeri ve SeriLot takipli değilse
					else
					{
						item.Quantity--;
					}
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

	private async Task LoadLocationItemsAsync(ReturnSalesBasketModel item)
	{
		try
		{
			_userDialogs.ShowLoading("Yükleniyor...");
			await Task.Delay(1000);
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, item.ItemReferenceId, 0, string.Empty, 0, 20);
			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var location in result.Data)
				{
					Locations.Add(Mapping.Mapper.Map<LocationModel>(location));
				}
			}
			_userDialogs.HideHud();
		}
		catch (Exception ex)
		{
			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task LocationCloseAsync()
	{
		await MainThread.InvokeOnMainThreadAsync(() =>
		{
			CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
		});
	}

	private async Task LocationIncreaseAsync(LocationModel locationModel)
	{
		if (IsBusy) return;

		try
		{
			IsBusy = true;
			locationModel.InputQuantity++;
		}
		catch (Exception ex)
		{
			await _userDialogs.AlertAsync($"{ex.Message}");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task LocationConfirmAsync(LocationModel locationModel)
	{
		if (IsBusy) return;
		try
		{
			IsBusy = true;
			if (Locations.Count > 0)
			{
				double totalInputQuantity = 0;
				foreach (var location in Locations)
				{
					if (location.InputQuantity > 0)
					{
						totalInputQuantity += location.InputQuantity;
					}
				}
				SelectedReturnSalesBasketModel.Quantity = totalInputQuantity;

				CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
			}
			else
			{
				CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
			}
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

	private async Task LocationDecreaseAsync(LocationModel locationModel)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			if (locationModel.InputQuantity > 0)
			{
				locationModel.InputQuantity -= 1;
			}
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

	private async Task LocationTransactionCloseAsync()
	{
		await MainThread.InvokeOnMainThreadAsync(() =>
		{
			CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
		});
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

			await Shell.Current.GoToAsync($"{nameof(ReturnSalesFormView)}", new Dictionary<string, object>
			{
				[nameof(WarehouseModel)] = WarehouseModel,
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
				{
					item.Details.Clear();
				}

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
	public async Task LoadPageAsync()
	{
		try
		{


			if (Items?.Count > 0)
				Items.Clear();
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
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
				ComingPage = "ReturnSalesBasketViewModel"
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
}