using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.SeriLotModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
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

	[ObservableProperty]
	private WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	private InputProductBasketModel? selectedInputProductBasketModel;

	[ObservableProperty]
	private InputProductProcessType inputProductProcessType;

	public InputProductProcessBasketListViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, ILocationService locationService, ISeriLotService seriLotService, IServiceProvider serviceProvider)
	{
		_userDialogs = userDialogs;
		_httpClientService = httpClientService;
		_locationService = locationService;
		_seriLotService = seriLotService;
		_serviceProvider = serviceProvider;

		Title = "Sepet Listesi";

		ShowProductViewCommand = new Command(async () => await ShowProductViewAsync());
		IncreaseCommand = new Command<InputProductBasketModel>(async (item) => await IncreaseAsync(item));
		DecreaseCommand = new Command<InputProductBasketModel>(async (item) => await DecreaseAsync(item));

		LoadMoreLocationsCommand = new Command(async () => await LoadMoreWarehouseLocationsAsync());
		LocationCloseCommand = new Command(async () => await LocationCloseAsync());
		LocationConfirmCommand = new Command<LocationModel>(async (locationModel) => await LocationConfirmAsync(locationModel));
		LocationIncreaseCommand = new Command<LocationModel>(async (locationModel) => await LocationIncreaseAsync(locationModel));
		LocationDecreaseCommand = new Command<LocationModel>(async (LocationModel) => await LocationDecreaseAsync(LocationModel));

		DeleteItemCommand = new Command<InputProductBasketModel>(async (item) => await DeleteItemAsync(item));
		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());

		Items.Clear();
	}

	public Command ShowProductViewCommand { get; }

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

	public Page CurrentPage { get; set; } = null!;

	public ObservableCollection<InputProductBasketModel> Items { get; } = new();
	public ObservableCollection<LocationModel> Locations { get; } = new();
	public ObservableCollection<SeriLotModel> SeriLots { get; } = new();

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
				

				await Shell.Current.GoToAsync($"{nameof(InputProductProcessBasketLocationListView)}", new Dictionary<string, object>
				{
					{nameof(WarehouseModel), WarehouseModel},
					{nameof(InputProductBasketModel), inputProductBasketModel}
				});
				await nextViewModel.LoadSelectedItemsAsync();

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
			var result = await _locationService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, WarehouseModel.Number, inputProductBasketModel.ItemReferenceId, string.Empty, 0, 20);
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
}