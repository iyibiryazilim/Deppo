using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BarcodeModels;
using Deppo.Mobile.Core.Models.CameraModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.OutsourceModels;
using Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.BarcodeHelper;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CameraModule.Views;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransfer.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransfer.ViewModels;

[QueryProperty(name: nameof(OutsourceModel), queryId: nameof(OutsourceModel))]
[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class InputOutsourceTransferOutsourceBasketListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	private readonly ILocationService _locationService;
	private readonly ISeriLotService _seriLotService;
	private readonly IServiceProvider _serviceProvider;
	private readonly ISubUnitsetService _subUnitsetService;
	private readonly IBarcodeSearchHelper _barcodeSearchHelper;

	[ObservableProperty]
	private WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	private OutsourceModel outsourceModel = null!;

	[ObservableProperty]
	private InputOutsourceTransferBasketModel? selectedInputOutsourceTransferBasketModel;
	public ObservableCollection<InputOutsourceTransferBasketModel> Items { get; } = new();
	public ObservableCollection<LocationModel> Locations { get; }

	public InputOutsourceTransferOutsourceBasketListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IHttpClientService httpClientService2, ILocationService locationService, ISeriLotService seriLotService, IServiceProvider serviceProvider, ISubUnitsetService subUnitsetService, IBarcodeSearchHelper barcodeSearchHelper)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_locationService = locationService;
		_seriLotService = seriLotService;
		_serviceProvider = serviceProvider;
		_subUnitsetService = subUnitsetService;
		_barcodeSearchHelper = barcodeSearchHelper;

		Title = "Sepet Listesi";

		ShowProductViewCommand = new Command(async () => await ShowProductViewAsync());
		PerformSearchCommand = new Command<Entry>(async (barcodeEntry) => await PerformSearchAsync(barcodeEntry));
		UnitActionTappedCommand = new Command<InputOutsourceTransferBasketModel>(async (item) => await UnitActionTappedAsync(item));
		SubUnitsetTappedCommand = new Command<SubUnitset>(async (subUnitset) => await SubUnitsetTappedAsync(subUnitset));

		QuantityTappedCommand = new Command<InputOutsourceTransferBasketModel>(async (item) => await QuantityTappedAsync(item));
		DeleteItemCommand = new Command<InputOutsourceTransferBasketModel>(async (item) => await DeleteItemAsync(item));
		IncreaseCommand = new Command<InputOutsourceTransferBasketModel>(async (item) => await IncreaseAsync(item));
		DecreaseCommand = new Command<InputOutsourceTransferBasketModel>(async (item) => await DecreaseAsync(item));

		LocationCloseCommand = new Command(async () => await LocationCloseAsync());
		LocationConfirmCommand = new Command<LocationModel>(async (locationModel) => await LocationConfirmAsync(locationModel));
		LocationIncreaseCommand = new Command<LocationModel>(async (locationModel) => await LocationIncreaseAsync(locationModel));
		LocationDecreaseCommand = new Command<LocationModel>(async (LocationModel) => await LocationDecreaseAsync(LocationModel));

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

	public Command<InputOutsourceTransferBasketModel> QuantityTappedCommand { get; }
	public Command<InputOutsourceTransferBasketModel> DeleteItemCommand { get; }
	public Command<InputOutsourceTransferBasketModel> IncreaseCommand { get; }
	public Command<InputOutsourceTransferBasketModel> DecreaseCommand { get; }

	public Command<LocationModel> LocationDecreaseCommand { get; }
	public Command<LocationModel> LocationIncreaseCommand { get; }
	public Command<LocationModel> LocationConfirmCommand { get; }
	public Command LocationCloseCommand { get; }

	public Command NextViewCommand { get; }
	public Command BackCommand { get; }
	public Command CameraTappedCommand { get; }

	private async Task ShowProductViewAsync()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(InputOutsourceTransferOutsourceProductListView)}", new Dictionary<string, object>
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
				barcode: barcodeEntry.Text
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
						var basketItem = new InputOutsourceTransferBasketModel
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
							LocTracking = result.LocTracking,
							TrackingType = result.TrackingType,
							IsVariant = result.IsVariant,
							Quantity = result.LocTracking == 0 ? 1 : 0
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
						var basketItem = new InputOutsourceTransferBasketModel
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
							LocTracking = result.LocTracking,
							TrackingType = result.TrackingType,
							IsVariant = true,
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
			barcodeEntry.Text = string.Empty;
			barcodeEntry.Focus();
			IsBusy = false;
		}
	}

	private async Task LoadSubUnitsetsAsync(InputOutsourceTransferBasketModel item)
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

			if (SelectedInputOutsourceTransferBasketModel is not null)
			{
				SelectedInputOutsourceTransferBasketModel.SubUnitsetReferenceId = subUnitset.ReferenceId;
				SelectedInputOutsourceTransferBasketModel.SubUnitsetName = subUnitset.Name;
				SelectedInputOutsourceTransferBasketModel.SubUnitsetCode = subUnitset.Code;
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

	private async Task UnitActionTappedAsync(InputOutsourceTransferBasketModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			SelectedInputOutsourceTransferBasketModel = item;
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

	private async Task QuantityTappedAsync(InputOutsourceTransferBasketModel inputOutsourceTransferBasketModel)
	{
		if (IsBusy)
			return;
		if (inputOutsourceTransferBasketModel is null)
			return;
		if (inputOutsourceTransferBasketModel.LocTracking == 1)
			return;
		try
		{
			IsBusy = true;

			var result = await CurrentPage.DisplayPromptAsync(
				title: inputOutsourceTransferBasketModel.ItemCode,
				message: "Miktarı giriniz",
				cancel: "Vazgeç",
				accept: "Tamam",
				placeholder: inputOutsourceTransferBasketModel.Quantity.ToString(),
				keyboard: Keyboard.Numeric);

			if (string.IsNullOrEmpty(result))
				return;

			var quantity = Convert.ToDouble(result);

			if (quantity < 0)
			{
				await _userDialogs.AlertAsync("Miktar sıfırdan küçük olmamalıdır.", "Hata", "Tamam");
				return;
			}

			inputOutsourceTransferBasketModel.Quantity = quantity;
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

	private async Task DeleteItemAsync(InputOutsourceTransferBasketModel item)
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

	private async Task IncreaseAsync(InputOutsourceTransferBasketModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (item.LocTracking == 1)
			{
				var nextViewModel = _serviceProvider.GetRequiredService<InputOutsourceTransferBasketLocationListViewModel>();

				await Shell.Current.GoToAsync($"{nameof(InputOutsourceTransferBasketLocationListView)}", new Dictionary<string, object>
				{
					[nameof(WarehouseModel)] = WarehouseModel,
					[nameof(InputOutsourceTransferBasketModel)] = item
				});
				await nextViewModel.LoadSelectedItemsAsync();
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

	private async Task DecreaseAsync(InputOutsourceTransferBasketModel item)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			if (item.Quantity > 1)
				item.Quantity--;
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

	private async Task LoadLocationItemsAsync(InputOutsourceTransferBasketModel item)
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
				SelectedInputOutsourceTransferBasketModel.Quantity = totalInputQuantity;

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

			await Shell.Current.GoToAsync($"{nameof(InputOutsourceTransferOutsourceFormView)}", new Dictionary<string, object>
			{
				[nameof(WarehouseModel)] = WarehouseModel,
				[nameof(Items)] = Items,
				[nameof(OutsourceModel)] = OutsourceModel,
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
				ComingPage = "InputOutsourceTransferOutsourceBasketListViewModel"
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

			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}
}