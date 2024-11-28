using CommunityToolkit.Maui.Core.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BarcodeModels;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.CameraModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.SeriLotModels;
using Deppo.Mobile.Core.Models.TransferModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.BarcodeHelper;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CameraModule.Views;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(InputProductProcessType), queryId: nameof(InputProductProcessType))]
public partial class InputProductProcessBasketListViewModel : BaseViewModel
{
	private readonly IUserDialogs _userDialogs;
	private readonly IHttpClientService _httpClientService;
	private readonly ILocationService _locationService;
	private readonly ISeriLotService _seriLotService;
	private readonly IServiceProvider _serviceProvider;
	private readonly IBarcodeSearchHelper _barcodeSearchHelper;
	private readonly ISubUnitsetService _subUnitsetService;

	[ObservableProperty]
	private WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	private InputProductBasketModel? selectedInputProductBasketModel;

	[ObservableProperty]
	private InputProductProcessType inputProductProcessType;

	public ObservableCollection<InputProductBasketModel> Items { get; } = new();
	public ObservableCollection<LocationModel> Locations { get; } = new();
	public ObservableCollection<SeriLotModel> SeriLots { get; } = new();

	//[ObservableProperty]
	//public Entry barcodeEntry;

	public InputProductProcessBasketListViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, ILocationService locationService, ISeriLotService seriLotService, IServiceProvider serviceProvider, IBarcodeSearchHelper barcodeSearchHelper, ISubUnitsetService subUnitsetService)
	{
		_userDialogs = userDialogs;
		_httpClientService = httpClientService;
		_locationService = locationService;
		_seriLotService = seriLotService;
		_serviceProvider = serviceProvider;
		_barcodeSearchHelper = barcodeSearchHelper;
		_subUnitsetService = subUnitsetService;

		Title = "Sepet Listesi";

		LoadPageCommand = new Command(async () => await LoadPageAsync());
		ShowProductViewCommand = new Command(async () => await ShowProductViewAsync());
		PerformSearchCommand = new Command<Entry>(async (barcodeEntry) => await PerformSearchAsync(barcodeEntry));
		QuantityTappedCommand = new Command<InputProductBasketModel>(async (item) => await QuantityTappedAsync(item));

		UnitActionTappedCommand = new Command<InputProductBasketModel>(async (item) => await UnitActionTappedAsync(item));
		SubUnitsetTappedCommand = new Command<SubUnitset>(async (subUnitset) => await SubUnitsetTappedAsync(subUnitset));

		IncreaseCommand = new Command<InputProductBasketModel>(async (item) => await IncreaseAsync(item));
		DecreaseCommand = new Command<InputProductBasketModel>(async (item) => await DecreaseAsync(item));
		DeleteItemCommand = new Command<InputProductBasketModel>(async (item) => await DeleteItemAsync(item));


		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());
		CameraTappedCommand = new Command(async () => await CameraTappedAsync());

		Items.Clear();
	}

	public ContentPage CurrentPage { get; set; } = null!;

	public Command LoadPageCommand { get; }
	public Command ShowProductViewCommand { get; }
	public Command PerformSearchCommand { get; }

	public Command SubUnitsetTappedCommand { get; }
	public Command UnitActionTappedCommand { get; }

	public Command<InputProductBasketModel> QuantityTappedCommand { get; }
	public Command<InputProductBasketModel> DeleteItemCommand { get; }
	public Command<InputProductBasketModel> IncreaseCommand { get; }
	public Command<InputProductBasketModel> DecreaseCommand { get; }

	public Command LoadMoreLocationsCommand { get; }
	public Command<LocationModel> LocationDecreaseCommand { get; }
	public Command<LocationModel> LocationIncreaseCommand { get; }
	public Command<LocationModel> LocationConfirmCommand { get; }
	public Command LocationCloseCommand { get; }

	public Command LoadMoreSeriLotsCommand { get; }
	public Command<SeriLotModel> SeriLotIncreaseCommand { get; }
	public Command<SeriLotModel> SeriLotDecreaseCommand { get; }
	public Command SeriLotConfirmCommand { get; }
	public Command SeriLotCloseCommand { get; }

	public Command NextViewCommand { get; }
	public Command BackCommand { get; }
	public Command CameraTappedCommand { get; }
	public Command EntryFocusedCommand { get; }

	public async Task LoadPageAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var barcodeEntry = CurrentPage.FindByName<Entry>("barcodeEntry");
			if (barcodeEntry is not null)
			{
				barcodeEntry.Focus();
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
						var basketItem = new InputProductBasketModel
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
						var basketItem = new InputProductBasketModel
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

	private async Task LoadSubUnitsetsAsync(InputProductBasketModel item)
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

			if (SelectedInputProductBasketModel is not null)
			{
				SelectedInputProductBasketModel.SubUnitsetReferenceId = subUnitset.ReferenceId;
				SelectedInputProductBasketModel.SubUnitsetName = subUnitset.Name;
				SelectedInputProductBasketModel.SubUnitsetCode = subUnitset.Code;
				SelectedInputProductBasketModel.ConversionFactor = subUnitset.ConversionValue;
				SelectedInputProductBasketModel.OtherConversionFactor = subUnitset.OtherConversionValue;
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

	private async Task UnitActionTappedAsync(InputProductBasketModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			SelectedInputProductBasketModel = item;
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

	private async Task QuantityTappedAsync(InputProductBasketModel inputProductBasketModel)
	{
		if (inputProductBasketModel is null)
			return;
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (inputProductBasketModel.LocTracking == 1)
				return;

			var result = await CurrentPage.DisplayPromptAsync(
				title: inputProductBasketModel.ItemCode,
				message: "Miktarı giriniz",
				cancel: "Vazgeç",
				accept: "Tamam",
				placeholder: inputProductBasketModel.Quantity.ToString(),
				keyboard: Keyboard.Numeric);

			if (string.IsNullOrEmpty(result))
				return;

			var quantity = Convert.ToDouble(result);

			if (quantity < 0)
			{
				await _userDialogs.AlertAsync("Miktar sıfırdan küçük olmamalıdır.", "Hata", "Tamam");
				return;
			}

			inputProductBasketModel.Quantity = quantity;
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

	private async Task IncreaseAsync(InputProductBasketModel inputProductBasketModel)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			SelectedInputProductBasketModel = inputProductBasketModel;
			if (inputProductBasketModel.LocTracking == 1)
			{
				var nextViewModel = _serviceProvider.GetRequiredService<InputProductProcessBasketLocationListViewModel>();

				nextViewModel.InputProductBasketModel = inputProductBasketModel;

				await nextViewModel.LoadSelectedItemsAsync();

				await Shell.Current.GoToAsync($"{nameof(InputProductProcessBasketLocationListView)}", new Dictionary<string, object>
				{
					{nameof(WarehouseModel), WarehouseModel},
					{nameof(InputProductBasketModel), inputProductBasketModel}
				});
			}

			// Sadece SeriLot takipli ise
			else if (inputProductBasketModel.LocTracking == 0 && (inputProductBasketModel.TrackingType == 1 || inputProductBasketModel.TrackingType == 2))
			{
				await Shell.Current.GoToAsync($"{nameof(InputProductProcessBasketSeriLotListView)}", new Dictionary<string, object>
				{
					 {nameof(WarehouseModel), WarehouseModel},
					{nameof(InputProductBasketModel), inputProductBasketModel}
				});
			}
			//stok yeri ve serilot takipli değilse
			else
			{
				inputProductBasketModel.Quantity++;
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

	private async Task DecreaseAsync(InputProductBasketModel inputProductBasketModel)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (inputProductBasketModel is not null)
			{
				if (inputProductBasketModel.Quantity > 1)
				{
					// Stok Yeri takipli ise locationTransactionBottomSheet aç
					if (inputProductBasketModel.LocTracking == 1)
					{
						var nextViewModel = _serviceProvider.GetRequiredService<InputProductProcessBasketLocationListViewModel>();

						nextViewModel.InputProductBasketModel = inputProductBasketModel;

						await Shell.Current.GoToAsync($"{nameof(InputProductProcessBasketLocationListView)}", new Dictionary<string, object>
						{
							{nameof(WarehouseModel), WarehouseModel},
							{nameof(InputProductBasketModel), inputProductBasketModel}
						});
						await nextViewModel.LoadSelectedItemsAsync();

					}
					// Sadece SeriLot takipli ise serilotTransactionBottomSheet aç
					else if (inputProductBasketModel.LocTracking == 0 && (inputProductBasketModel.TrackingType == 1 || inputProductBasketModel.TrackingType == 2))
					{
						await Shell.Current.GoToAsync($"{nameof(InputProductProcessBasketSeriLotListView)}", new Dictionary<string, object>
						{
							{nameof(WarehouseModel), WarehouseModel},
							{nameof(InputProductBasketModel), inputProductBasketModel}
						});
					}
					// Stok yeri ve SeriLot takipli değilse
					else
					{
						inputProductBasketModel.Quantity--;
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

	private async Task DeleteItemAsync(InputProductBasketModel item)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			var result = await _userDialogs.ConfirmAsync($"{item.ItemCode}\n{item.ItemName}\nİlgili ürün sepetinizden çıkarılacaktır. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
			if (!result)
				return;

			Items.Remove(Items.FirstOrDefault(x => x.ItemCode == item.ItemCode));
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
			if (Items.Any(item => item.Quantity == 0))
			{
				await _userDialogs.AlertAsync("Sepetinizde 0 miktara sahip ürün bulunmaktadır. Miktar bilgilerini düzenleyiniz.", "Hata", "Tamam");
				return;
			}

			await Shell.Current.GoToAsync($"{nameof(InputProductProcessFormView)}", new Dictionary<string, object>
			{
				[nameof(WarehouseModel)] = WarehouseModel,
				[nameof(InputProductProcessType)] = InputProductProcessType,
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
				ComingPage = "InputProductProcessBasketListViewModel",
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