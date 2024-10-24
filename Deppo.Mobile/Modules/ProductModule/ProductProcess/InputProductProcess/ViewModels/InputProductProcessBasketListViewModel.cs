using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.SeriLotModels;
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

	[ObservableProperty]
	public Entry barcodeEntry;

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

		ShowProductViewCommand = new Command(async () => await ShowProductViewAsync());
		PerformSearchCommand = new Command<Entry>(async (barcodeEntry) => await PerformSearchAsync(barcodeEntry));
		QuantityTappedCommand = new Command<InputProductBasketModel>(async (item) => await QuantityTappedAsync(item));

		UnitActionTappedCommand = new Command<InputProductBasketModel>(async (item) => await UnitActionTappedAsync(item));
		SubUnitsetTappedCommand = new Command<SubUnitset>(async (subUnitset) => await SubUnitsetTappedAsync(subUnitset));

		IncreaseCommand = new Command<InputProductBasketModel>(async (item) => await IncreaseAsync(item));
		DecreaseCommand = new Command<InputProductBasketModel>(async (item) => await DecreaseAsync(item));
		DeleteItemCommand = new Command<InputProductBasketModel>(async (item) => await DeleteItemAsync(item));

		LoadMoreLocationsCommand = new Command(async () => await LoadMoreWarehouseLocationsAsync());
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

			await _barcodeSearchHelper.BarcodeDetectedAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				barcode: barcodeEntry.Text,
				comingPage: "InputProductProcessBasketListViewModel"
			);
		}
		catch (Exception ex)
		{
			if(_userDialogs.IsHudShowing)
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

			var result = await CurrentPage.DisplayPromptAsync(
				title: inputProductBasketModel.ItemCode,
				message: "Miktarı giriniz",
				cancel: "Vazgeç",
				accept: "Tamam",
				initialValue: inputProductBasketModel.Quantity.ToString(),
				keyboard: Keyboard.Numeric);

			if (string.IsNullOrEmpty(result))
				return;

			var quantity = Convert.ToDouble(result);

			if (quantity <= 0)
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

				if(nextViewModel.InputProductBasketModel is null)
				{
					nextViewModel.InputProductBasketModel = inputProductBasketModel;
				}
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
						if (nextViewModel.InputProductBasketModel is null)
						{
							nextViewModel.InputProductBasketModel = inputProductBasketModel;
						}
						await nextViewModel.LoadSelectedItemsAsync();
						
                        await Shell.Current.GoToAsync($"{nameof(InputProductProcessBasketLocationListView)}", new Dictionary<string, object>
						{
							{nameof(WarehouseModel), WarehouseModel},
							{nameof(InputProductBasketModel), inputProductBasketModel}
						});

						
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

	[Obsolete("Not used")]
	private async Task LoadWarehouseLocationsAsync(InputProductBasketModel inputProductBasketModel)
	{
		try
		{
			_userDialogs.ShowLoading("Yükleniyor...");
			await Task.Delay(1000);
			Locations.Clear();

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, inputProductBasketModel.ItemReferenceId,0, string.Empty, 0, 20);
			if (result.IsSuccess)
			{
				if (result.Data is not null)
				{
					foreach (var item in result.Data)
						Locations.Add(Mapping.Mapper.Map<LocationModel>(item));
				}
			}

			_userDialogs.HideHud();
		}
		catch (System.Exception ex)
		{
			await _userDialogs.AlertAsync(ex.Message);
		}
	}

	[Obsolete("Not used")]
	private async Task LoadMoreWarehouseLocationsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, SelectedInputProductBasketModel.ItemReferenceId, search: string.Empty, skip: Locations.Count, take: 20);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
					Locations.Add(Mapping.Mapper.Map<LocationModel>(item));
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

	[Obsolete("Not used")]
	private async Task LocationCloseAsync()
	{
		await MainThread.InvokeOnMainThreadAsync(() =>
		{
			CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
		});
	}

	[Obsolete("Not used")]
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

	[Obsolete("Not used")]
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
				SelectedInputProductBasketModel.Quantity = totalInputQuantity;

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

	[Obsolete("Not used")]
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

	[Obsolete("Not used")]
	private async Task LoadSeriLotAsync(InputProductBasketModel inputProductBasketModel)
	{
		try
		{
			_userDialogs.ShowLoading("Yükleniyor...");
			await Task.Delay(1000);
			SeriLots.Clear();

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _seriLotService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, search: string.Empty, skip: 0, take: 20);
			if (result.IsSuccess)
			{
				if (result.Data is not null)
				{
					foreach (var item in result.Data)
						SeriLots.Add(Mapping.Mapper.Map<SeriLotModel>(item));
				}
			}

			_userDialogs.HideHud();
		}
		catch (System.Exception ex)
		{
			await _userDialogs.AlertAsync(ex.Message);
		}
	}

	[Obsolete("Not used")]
	private async Task LoadMoreSeriLotAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _seriLotService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, search: string.Empty, skip: SeriLots.Count, take: 20);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
					SeriLots.Add(Mapping.Mapper.Map<SeriLotModel>(item));
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

	[Obsolete("Not used")]
	private async Task SeriLotCloseAsync()
	{
		await MainThread.InvokeOnMainThreadAsync(() =>
		{
			CurrentPage.FindByName<BottomSheet>("serilotBottomSheet").State = BottomSheetState.Hidden;
		});
	}

	[Obsolete("Not used")]
	private void SeriLotIncrease(SeriLotModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			item.InputQuantity += 1;
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

	[Obsolete("Not used")]
	private void SeriLotDecrease(SeriLotModel item)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			if (item.InputQuantity > 0)
			{
				item.InputQuantity -= 1;
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

	[Obsolete("Not used")]
	private void SeriLotConfirm()
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			if (SeriLots.Count > 0)
			{
				double totalInputQuantity = 0;
				foreach (var seriLot in SeriLots)
				{
					if (seriLot.InputQuantity > 0)
						totalInputQuantity += seriLot.InputQuantity;
				}

				SelectedInputProductBasketModel.Quantity = totalInputQuantity;

				CurrentPage.FindByName<BottomSheet>("serilotBottomSheet").State = BottomSheetState.Hidden;
			}
			else
			{
				CurrentPage.FindByName<BottomSheet>("serilotBottomSheet").State = BottomSheetState.Hidden;
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

			await Shell.Current.GoToAsync($"{nameof(InputProductProcessFormView)}", new Dictionary<string, object>
                {
                    [nameof(WarehouseModel)] = WarehouseModel,
                    [nameof(InputProductProcessType)] = InputProductProcessType,
					[nameof(Items)] = Items
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

				Items.Clear();
				await Shell.Current.GoToAsync("..");
			}
			else
				await Shell.Current.GoToAsync("..");
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


			await Shell.Current.GoToAsync($"{nameof(CameraReaderView)}", new Dictionary<string, object>
			{
				["ComingPage"] = "InputProductProcessBasket"
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