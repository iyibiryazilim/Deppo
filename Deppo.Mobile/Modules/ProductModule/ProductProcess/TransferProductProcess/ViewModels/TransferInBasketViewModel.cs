using Com.Google.Android.Exoplayer2.Transformer;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.SeriLotModels;
using Deppo.Mobile.Core.Models.TransferModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;
using System.Net.Http;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.ViewModels;

[QueryProperty(name: nameof(TransferBasketModel), queryId: nameof(TransferBasketModel))]
public partial class TransferInBasketViewModel : BaseViewModel
{

	private readonly IUserDialogs _userDialogs;
	private readonly IHttpClientService _httpClientService;
	private readonly ILocationService _locationService;
	private readonly ISeriLotService _seriLotService;
	private readonly IServiceProvider _serviceProvider;

	[ObservableProperty]
	TransferBasketModel transferBasketModel = null!;

	[ObservableProperty]
	InProductModel selectedInputProductModel = null!;

	[ObservableProperty]
	public SearchBar locationSearchText;

	public TransferInBasketViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, ILocationService locationService, ISeriLotService seriLotService, IServiceProvider serviceProvider)
	{
		_userDialogs = userDialogs;
		_httpClientService = httpClientService;
		_locationService = locationService;
		_seriLotService = seriLotService;
		_serviceProvider = serviceProvider;

		Title = "Transfer Sepeti";

		IncreaseCommand = new Command<InProductModel>(async (item) => await IncreaseAsync(item));
		DecreaseCommand = new Command<InProductModel>(async (item) => await DecreaseAsync(item));

		LoadMoreLocationsCommand = new Command(async () => await LoadMoreWarehouseLocationsAsync());
		LocationCloseCommand = new Command(async () => await LocationCloseAsync());
		LocationQuantityTappedCommand = new Command<LocationModel>(async (locationModel) => await LocationQuantityTappedAsync(locationModel));
		LocationConfirmCommand = new Command<LocationModel>(async (locationModel) => await LocationConfirmAsync(locationModel));
		LocationIncreaseCommand = new Command<LocationModel>(async (locationModel) => await LocationIncreaseAsync(locationModel));
		LocationDecreaseCommand = new Command<LocationModel>(async (LocationModel) => await LocationDecreaseAsync(LocationModel));

		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());
	}
	public Page CurrentPage { get; set; }

	//public Command<InProductModel> DeleteItemCommand { get; }
	public Command<InProductModel> IncreaseCommand { get; }
	public Command<InProductModel> DecreaseCommand { get; }

	public Command LoadMoreLocationsCommand { get; }
	public Command<LocationModel> LocationDecreaseCommand { get; }
	public Command<LocationModel> LocationIncreaseCommand { get; }
	public Command<LocationModel> LocationQuantityTappedCommand { get; }
	public Command LocationPerformSearchCommand { get; }
	public Command LocationPerformEmptySearchCommand { get; }

	public Command<LocationModel> LocationConfirmCommand { get; }
	public Command LocationCloseCommand { get; }

	public Command LoadMoreSeriLotsCommand { get; }
	public Command<SeriLotModel> SeriLotIncreaseCommand { get; }
	public Command<SeriLotModel> SeriLotDecreaseCommand { get; }
	public Command SeriLotConfirmCommand { get; }
	public Command SeriLotCloseCommand { get; }

	public Command NextViewCommand { get; }
	public Command BackCommand { get; }


	public ObservableCollection<LocationModel> Locations { get; } = new();
	public ObservableCollection<LocationModel> SearchLocations { get; } = new();
	public ObservableCollection<SeriLotModel> SeriLots { get; } = new();



	private async Task IncreaseAsync(InProductModel item)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;
			SelectedInputProductModel = item;

			await LoadWarehouseLocationsAsync(item);
			CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.FullExpanded;
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

	private async Task DecreaseAsync(InProductModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			SelectedInputProductModel = item;

			await LoadWarehouseLocationsAsync(item);
			CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.FullExpanded;

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


	private async Task LoadWarehouseLocationsAsync(InProductModel inProductModel)
	{
		try
		{
			_userDialogs.ShowLoading("Yükleniyor...");
			await Task.Delay(1000);
			Locations.Clear();

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, TransferBasketModel.InWarehouse.Number, productReferenceId:inProductModel.IsVariant ?  inProductModel.MainItemReferenceId : inProductModel.ItemReferenceId, 0, string.Empty, 0, 20);
			if (result.IsSuccess)
			{
				if (result.Data is not null)
				{
					foreach (var item in result.Data)
						Locations.Add(Mapping.Mapper.Map<LocationModel>(item));


                    foreach (var location in Locations)
                    {
                        var matchingItem = SelectedInputProductModel.Locations.FirstOrDefault(item => item.ReferenceId == location.ReferenceId);
                        if (matchingItem != null)
                        {
                            location.InputQuantity = matchingItem.InputQuantity;
                        }
                    }
                }
			}

			_userDialogs.HideHud();
		}
		catch (System.Exception ex)
		{
			await _userDialogs.AlertAsync(ex.Message);
		}
	}

	private async Task LoadMoreWarehouseLocationsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, TransferBasketModel.InWarehouse.Number, productReferenceId: SelectedInputProductModel.IsVariant ?  SelectedInputProductModel.MainItemReferenceId : SelectedInputProductModel.ItemReferenceId, search: string.Empty, skip: Locations.Count, take: 20);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
					Locations.Add(Mapping.Mapper.Map<LocationModel>(item));

                foreach (var location in Locations)
                {
                    var matchingItem = SelectedInputProductModel.Locations.FirstOrDefault(item => item.ReferenceId == location.ReferenceId);
                    if (matchingItem != null)
                    {
                        location.InputQuantity = matchingItem.InputQuantity;
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

	private async Task LocationCloseAsync()
	{
		await MainThread.InvokeOnMainThreadAsync(() =>
		{
			CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
		});
	}

	private async Task LocationPerformSearchAsync()
	{
		if(string.IsNullOrWhiteSpace(LocationSearchText.Text))
		{
			await LoadWarehouseLocationsAsync(SelectedInputProductModel);
		}
		IsBusy = true;

		var httpClient = _httpClientService.GetOrCreateHttpClient();

		var result = await _locationService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, TransferBasketModel.InWarehouse.Number, productReferenceId: SelectedInputProductModel.IsVariant ? SelectedInputProductModel.MainItemReferenceId : SelectedInputProductModel.ItemReferenceId, 0, LocationSearchText.Text, 0, 99999);

		if(result.IsSuccess)
		{
			if (result.Data is null)
				return;

            foreach (var item in result.Data)
            {
				var obj = Mapping.Mapper.Map<LocationModel>(item);

			}
        }
	}
	private async Task LocationPerformEmptySearchAsync()
	{
		if (string.IsNullOrWhiteSpace(LocationSearchText.Text))
		{
			await LocationPerformSearchAsync();
		}
	}
	private async Task LocationIncreaseAsync(LocationModel locationModel)
	{
		if (IsBusy) return;
		try
		{
			IsBusy = true;
			var totalLocationQuantity = Locations.Sum(x => x.InputQuantity);
			if (locationModel.InputQuantity > SelectedInputProductModel.OutputQuantity || totalLocationQuantity >= SelectedInputProductModel.OutputQuantity)
			{
				await _userDialogs.AlertAsync("Girilen miktar, seçilen ürün miktarından fazla olamaz.", "Hata", "Tamam");
				return;
			}
			else
			{
				locationModel.InputQuantity++;
			}

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

	private async Task LocationQuantityTappedAsync(LocationModel locationModel)
	{
		if (IsBusy)
			return;
		try
		{
			var result = await CurrentPage.DisplayPromptAsync(
			  title: locationModel.Code,
			  message: "Miktarı giriniz",
			  cancel: "Vazgeç",
			  accept: "Tamam",
			  placeholder: locationModel.InputQuantity.ToString(),
			  keyboard: Keyboard.Numeric
		  );

			if (string.IsNullOrEmpty(result))
				return;

			var quantity = Convert.ToDouble(result);
			if (quantity < 0)
			{
				await _userDialogs.AlertAsync("Girilen miktar 0'dan küçük olmamalıdır.", "Uyarı", "Tamam");
				return;
			}

			if(quantity > SelectedInputProductModel.OutputQuantity)
			{
				await _userDialogs.AlertAsync("Girilen miktar, seçilen ürün miktarından fazla olamaz.", "Hata", "Tamam");
				return;
			}

			var totalLocationQuantity = Locations.Where(x => x.Code != locationModel.Code).Sum(x => x.InputQuantity);

			if (totalLocationQuantity + quantity > SelectedInputProductModel.OutputQuantity)
			{
				await _userDialogs.AlertAsync("Girilen miktar, seçilen ürün miktarından fazla olamaz.", "Hata", "Tamam");
				return;
			}

			locationModel.InputQuantity = quantity;
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

	private async Task LocationConfirmAsync(LocationModel locationModel)
	{
		if (IsBusy) return;
		try
		{
			IsBusy = true;
			if (Locations.Count > 0)
			{
				double totalInputQuantity = 0;

				var totalLocationQuantity = Locations.Sum(x => x.InputQuantity);

				if (totalLocationQuantity != SelectedInputProductModel.OutputQuantity)
				{
					await _userDialogs.AlertAsync($"Toplam girilen miktar ({totalLocationQuantity}), seçilen ürünün {SelectedInputProductModel.OutputQuantity} miktarına eşit olmalıdır.", "Uyarı", "Tamam");
					return;
				}

				SelectedInputProductModel.Locations.Clear();

				foreach (var x in Locations.Where(X => X.InputQuantity > 0))
				{
					SelectedInputProductModel.Locations.Add(new LocationModel
					{
						ReferenceId = x.ReferenceId,
						Code = x.Code,
						StockQuantity = x.StockQuantity,
						WarehouseName = x.WarehouseName,
						WarehouseNumber = x.WarehouseNumber,
						WarehouseReferenceId = x.WarehouseReferenceId,
						Name = x.Name,
						InputQuantity = x.InputQuantity
					});
				}

				if(SelectedInputProductModel.OutputQuantity == SelectedInputProductModel.Locations.Sum(x => x.InputQuantity))
                {
                    SelectedInputProductModel.IsCompleted = true;
                }

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
	private async Task LoadSeriLotAsync(InProductModel inProductModel)
	{
		try
		{
			_userDialogs.ShowLoading("Yükleniyor...");
			await Task.Delay(1000);
			SeriLots.Clear();

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _seriLotService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, TransferBasketModel.InWarehouse.Number, search: string.Empty, skip: 0, take: 20);
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
			var result = await _seriLotService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, TransferBasketModel.InWarehouse.Number, search: string.Empty, skip: SeriLots.Count, take: 20);

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

				SelectedInputProductModel.OutputQuantity = totalInputQuantity;

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

            foreach (var item in TransferBasketModel.InProducts)
            {
                double totalInputQuantity = item.Locations.Sum(location => location.InputQuantity);
                item.IsCompleted = totalInputQuantity == item.OutputQuantity;
            }

            if (TransferBasketModel.InProducts.Any(x => x.IsCompleted == false))
            {
                await _userDialogs.AlertAsync("Lütfen tüm ürünlerin raflarını belirtiniz.", "Hata", "Tamam");
                return;
            }

            await Shell.Current.GoToAsync($"{nameof(TransferFormView)}", new Dictionary<string, object>
			{
				[nameof(TransferBasketModel)] = TransferBasketModel
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

			var result = await _userDialogs.ConfirmAsync("Sepetinizdeki ürünlerin raf miktarları silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
			if (!result)
				return;

			foreach (var item in TransferBasketModel.InProducts)
			{
				item.IsSelected = false;
                item.Locations.Clear();
			}

			TransferBasketModel.InProducts.Clear();



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
