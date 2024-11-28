using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.ReworkModels;
using Deppo.Mobile.Core.Models.ReworkModels.BasketModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.QuicklyProductionModule.QuicklyProductionProcess.ManuelRemorkProcess.ViewModels;


[QueryProperty(name: nameof(ReworkBasketModel), queryId: nameof(ReworkBasketModel))]
public partial class ManuelReworkProcessBasketViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly ILocationTransactionService _locationTransactionService;
	private readonly IUserDialogs _userDialogs;
	private readonly IServiceProvider _serviceProvider;

	[ObservableProperty]
	ReworkBasketModel reworkBasketModel = null!;

	[ObservableProperty]
	ReworkInProductModel? selectedReworkInProductModel;

	public ObservableCollection<GroupLocationTransactionModel> LocationTransactions { get; } = new();

	public ManuelReworkProcessBasketViewModel(IHttpClientService httpClientService, ILocationTransactionService locationTransactionService, IUserDialogs userDialogs, IServiceProvider serviceProvider)
	{
		_httpClientService = httpClientService;
		_locationTransactionService = locationTransactionService;
		_userDialogs = userDialogs;
		_serviceProvider = serviceProvider;

		Title = "Sepet";

		IncreaseCommand = new Command(async () => await IncreaseAsync());
		DecreaseCommand = new Command(async () => await DecreaseAsync());
		AddProductTappedCommand = new Command(async () => await AddProductTappedAsync());
		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());


		InProductDecreaseCommand = new Command<ReworkInProductModel>(async (x) => await InProductDecreaseAsync(x));
		InProductIncreaseCommand = new Command<ReworkInProductModel>(async (x) => await InProductIncreaseAsync(x));
		InProductDeleteCommand = new Command<ReworkInProductModel>(async (x) => await InProductDeleteAsync(x));

		LoadMoreLocationTransactionsCommand = new Command(async () => await LoadMoreLocationTransactionsAsync());
		LocationTransactionQuantityTappedCommand = new Command<GroupLocationTransactionModel>(async (x) => await LocationTransactionQuantityTappedAsync(x));
		LocationTransactionIncreaseCommand = new Command<GroupLocationTransactionModel>(async (x) => await LocationTransactionIncreaseAsync(x));
		LocationTransactionDecreaseCommand = new Command<GroupLocationTransactionModel>(async (x) => await LocationTransactionDecreaseAsync(x));
		LocationTransactionConfirmCommand = new Command(async () => await LocationTransactionConfirmAsync());
		LocationTransactionCloseCommand = new Command(async () => await LocationTransactionCloseAsync());
	}
	public Page CurrentPage { get; set; } = null!;
	public Command IncreaseCommand { get; }
	public Command DecreaseCommand { get; }
	public Command AddProductTappedCommand { get; }
	public Command BackCommand { get; }
	public Command NextViewCommand { get; }

	public Command InProductIncreaseCommand { get; }
	public Command InProductDecreaseCommand { get; }
	public Command InProductDeleteCommand { get; }

	public Command LoadMoreLocationTransactionsCommand { get; }
	public Command<GroupLocationTransactionModel> LocationTransactionQuantityTappedCommand { get; }
	public Command<GroupLocationTransactionModel> LocationTransactionIncreaseCommand { get; }
	public Command<GroupLocationTransactionModel> LocationTransactionDecreaseCommand { get; }
	public Command LocationTransactionConfirmCommand { get; }
	public Command LocationTransactionCloseCommand { get; }

	private async Task IncreaseAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if(ReworkBasketModel.ReworkOutProductModel.StockQuantity > ReworkBasketModel.ReworkOutProductModel.OutputQuantity)
			{
				if(ReworkBasketModel.ReworkOutProductModel.LocTracking == 1 && ReworkBasketModel.ReworkOutProductModel.TrackingType == 0)
				{
					await LoadLocationTransactionsAsync();
					CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.FullExpanded;
				}
				else if (ReworkBasketModel.ReworkOutProductModel.LocTracking == 0 && ReworkBasketModel.ReworkOutProductModel.TrackingType == 0)
				{
					ReworkBasketModel.ReworkOutProductModel.OutputQuantity += 1;
				}
			}
				
		}
		catch (Exception ex)
		{
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}
	private async Task QuantityTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{

		}
		catch (Exception ex)
		{
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task DecreaseAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (ReworkBasketModel.ReworkOutProductModel.OutputQuantity > 0)
			{
				if (ReworkBasketModel.ReworkOutProductModel.LocTracking == 1 && ReworkBasketModel.ReworkOutProductModel.TrackingType == 0)
				{
					await LoadLocationTransactionsAsync();
					CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.FullExpanded;
				}
				else if (ReworkBasketModel.ReworkOutProductModel.LocTracking == 0 && ReworkBasketModel.ReworkOutProductModel.TrackingType == 0)
				{
					ReworkBasketModel.ReworkOutProductModel.OutputQuantity -= 1;
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

			if(ReworkBasketModel.ReworkInProducts.Count <= 0)
			{
				if (_userDialogs.IsHudShowing)
					_userDialogs.HideHud();

				await _userDialogs.AlertAsync($"Devam etmek için alt ürün seçmelisiniz!", "Uyarı", "Tamam");
				return;
			}

			if(ReworkBasketModel?.ReworkOutProductModel?.OutputQuantity <= 0)
			{
				if (_userDialogs.IsHudShowing)
					_userDialogs.HideHud();

				await _userDialogs.AlertAsync($"Ana ürünün çıkış miktarı 0'dan büyük olmalıdır", "Uyarı", "Tamam");
				return;
			}

			var confirm = await _userDialogs.ConfirmAsync($"Devam etmek istediğinize emin misiniz?", "Bilgilendirme", "Evet", "Hayır");
			if (!confirm)
				return;


            foreach (var inProduct in ReworkBasketModel?.ReworkInProducts.Where(x => x.InputQuantity <= 0))
            {
				inProduct.Details.Clear();
				ReworkBasketModel.ReworkInProducts.Remove(inProduct);
            }


            await Shell.Current.GoToAsync($"{nameof(ManuelReworkProcessFormView)}", new Dictionary<string, object>
			{
				[nameof(ReworkBasketModel)] = ReworkBasketModel
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
	private async Task BackAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var confirm = await _userDialogs.ConfirmAsync("Sepetinizdeki ürünler silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
			if (!confirm)
				return;

			if (ReworkBasketModel.ReworkInProducts.Any())
			{
				foreach (var item in ReworkBasketModel.ReworkInProducts)
				{
					if (item.Details.Any())
						item.Details.Clear();
				}

				ReworkBasketModel.ReworkInProducts.Clear();
			}

			await Shell.Current.GoToAsync("..");
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

	private async Task InProductIncreaseAsync(ReworkInProductModel item)
	{
		if (item is null)
			return;
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			SelectedReworkInProductModel = item;

			var locationViewModel = _serviceProvider.GetRequiredService<ManuelReworkProcessBasketLocationListViewModel>();

			if(SelectedReworkInProductModel.LocTracking == 1 && SelectedReworkInProductModel.TrackingType == 0)
			{
				await Shell.Current.GoToAsync($"{nameof(ManuelReworkProcessBasketLocationListView)}", new Dictionary<string, object>
				{
					["SelectedReworkInProductModel"] = SelectedReworkInProductModel
				});

				await locationViewModel.LoadSelectedItemsAsync();
			}
			else if(SelectedReworkInProductModel.LocTracking == 0 && SelectedReworkInProductModel.TrackingType == 0)
			{
				SelectedReworkInProductModel.InputQuantity += 1;
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

	private async Task InProductDecreaseAsync(ReworkInProductModel item)
	{
		if (item is null)
			return;
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if(item.InputQuantity > 0)
			{
				SelectedReworkInProductModel = item;

				if (SelectedReworkInProductModel.LocTracking == 1 && SelectedReworkInProductModel.TrackingType == 0)
				{
					await Shell.Current.GoToAsync($"{nameof(ManuelReworkProcessBasketLocationListView)}", new Dictionary<string, object>
					{
						["SelectedReworkInProductModel"] = SelectedReworkInProductModel
					});
				}
				else if (SelectedReworkInProductModel.LocTracking == 0 && SelectedReworkInProductModel.TrackingType == 0)
				{
					SelectedReworkInProductModel.InputQuantity -= 1;
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

	private async Task InProductDeleteAsync(ReworkInProductModel item)
	{
		if (item is null)
			return;
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			SelectedReworkInProductModel = item;

			var confirm = await _userDialogs.ConfirmAsync("Ürünü silmek istediğinize emin misiniz?", "Sil", "Evet", "Hayır");
			if(!confirm)
				return;

			ReworkBasketModel.ReworkInProducts.Remove(SelectedReworkInProductModel);

			if (SelectedReworkInProductModel.Details.Any())
			{
				SelectedReworkInProductModel.Details.Clear();
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
	private async Task LoadLocationTransactionsAsync()
	{
		if (ReworkBasketModel is null)
			return;
		if (ReworkBasketModel.ReworkOutProductModel is null)
			return;
		try
		{
			_userDialogs.ShowLoading("Load LocationTransaction Items...");
			LocationTransactions.Clear();

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationTransactionService.GetLocationTransactionsInputObjectsAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: ReworkBasketModel.ReworkOutProductModel.IsVariant ? ReworkBasketModel.ReworkOutProductModel.MainItemReferenceId : ReworkBasketModel.ReworkOutProductModel.ReferenceId,
				variantReferenceId: ReworkBasketModel.ReworkOutProductModel.IsVariant ? ReworkBasketModel.ReworkOutProductModel.ReferenceId : 0,
				warehouseNumber: ReworkBasketModel.ReworkOutProductModel.WarehouseNumber,
				skip: 0,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					LocationTransactions.Add(Mapping.Mapper.Map<GroupLocationTransactionModel>(item));
				}

				foreach (var locationTransaction in LocationTransactions)
				{
					var matchingItem = ReworkBasketModel.ReworkOutProductModel.Details.FirstOrDefault(x => x.LocationReferenceId == locationTransaction.LocationReferenceId);
					if (matchingItem != null)
					{
						locationTransaction.OutputQuantity = matchingItem.Quantity;
					}
				}
			}

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task LoadMoreLocationTransactionsAsync()
	{
		if (IsBusy)
			return;
		if (LocationTransactions.Count < 18)
			return;
		if (ReworkBasketModel is null)
			return;
		if (ReworkBasketModel.ReworkOutProductModel is null)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.ShowLoading("Load LocationTransaction Items...");
			LocationTransactions.Clear();

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationTransactionService.GetLocationTransactionsInputObjectsAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: ReworkBasketModel.ReworkOutProductModel.IsVariant ? ReworkBasketModel.ReworkOutProductModel.MainItemReferenceId : ReworkBasketModel.ReworkOutProductModel.ReferenceId,
				variantReferenceId: ReworkBasketModel.ReworkOutProductModel.IsVariant ? ReworkBasketModel.ReworkOutProductModel.ReferenceId : 0,
				warehouseNumber: ReworkBasketModel.ReworkOutProductModel.WarehouseNumber,
				skip: LocationTransactions.Count,
				take: 20
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					LocationTransactions.Add(Mapping.Mapper.Map<GroupLocationTransactionModel>(item));
				}

				foreach (var locationTransaction in LocationTransactions)
				{
					var matchingItem = ReworkBasketModel.ReworkOutProductModel.Details.FirstOrDefault(x => x.LocationReferenceId == locationTransaction.LocationReferenceId);
					if (matchingItem != null)
					{
						locationTransaction.OutputQuantity = matchingItem.Quantity;
					}
				}
			}

			if (_userDialogs.IsHudShowing)
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

	private async Task LocationTransactionQuantityTappedAsync(GroupLocationTransactionModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var result = await CurrentPage.DisplayPromptAsync(
				title: item.ItemCode,
				message: "Miktarı giriniz",
				cancel: "Vazgeç",
				accept: "Tamam",
				placeholder: item.OutputQuantity.ToString(),
				keyboard: Keyboard.Numeric);

			if (string.IsNullOrEmpty(result))
				return;

			var quantity = Convert.ToDouble(result);

			if (quantity < 0)
			{
				await _userDialogs.AlertAsync("Girilen miktar 0'dan küçük olmamalıdır.", "Hata", "Tamam");
				return;
			}

			if (quantity > item.RemainingQuantity)
			{
				await _userDialogs.AlertAsync($"Girilen miktar, kalan miktarı ({item.RemainingQuantity}) aşmamalıdır.", "Hata", "Tamam");
				return;
			}

			if (quantity > ReworkBasketModel.ReworkOutProductModel.StockQuantity)
			{
				await _userDialogs.AlertAsync($"Girilen miktar, Ana ürünün stok miktarını ({ReworkBasketModel.ReworkOutProductModel.StockQuantity}) aşmamalıdır.", "Hata", "Tamam");
				return;
			}

			var totalQuantity = LocationTransactions.Sum(x => x.OutputQuantity);
			if (ReworkBasketModel.ReworkOutProductModel.StockQuantity >= totalQuantity + quantity)
			{
				item.OutputQuantity = quantity;
			}
			else
			{
				await _userDialogs.AlertAsync($"Toplam girilen miktar, Ana ürünün stok miktarını ({ReworkBasketModel.ReworkOutProductModel.StockQuantity}) aşmamalıdır.", "Hata", "Tamam");
				return;
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

	private async Task LocationTransactionIncreaseAsync(GroupLocationTransactionModel item)
	{
		if (IsBusy)
			return;
		if (item is null)
			return;
		try
		{
			IsBusy = true;

			var totalOutputQuantity = LocationTransactions.Sum(x => x.OutputQuantity);

			if (ReworkBasketModel.ReworkOutProductModel.StockQuantity <= totalOutputQuantity)
				return;

			if (item.OutputQuantity >= item.RemainingQuantity)
				return;

			item.OutputQuantity += 1;

		}
		catch (Exception ex)
		{
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task LocationTransactionDecreaseAsync(GroupLocationTransactionModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (item.OutputQuantity <= 0)
				return;

			item.OutputQuantity -= 1;

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

	private async Task LocationTransactionConfirmAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (LocationTransactions.Count <= 0)
			{
				CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
				return;
			}

			var confirm = await _userDialogs.ConfirmAsync("Onaylamak istediğinize emin misiniz?", "Onay", "Evet", "Hayır");
			if (!confirm)
				return;

			var locationTransacionTotalOutputQuantity = LocationTransactions.Where(x => x.OutputQuantity > 0).Sum(x => x.OutputQuantity);

			if (locationTransacionTotalOutputQuantity > ReworkBasketModel.ReworkOutProductModel.StockQuantity)
			{
				await _userDialogs.AlertAsync($"Ana ürünün stok miktarından {ReworkBasketModel.ReworkOutProductModel.StockQuantity} fazla hareket çıkışı yapamazsınız.", "Hata", "Tamam");
				return;
			}

			foreach (var locationTransaction in LocationTransactions.Where(x => x.OutputQuantity <= 0))
			{
				if (ReworkBasketModel.ReworkOutProductModel.Details.Any(x => x.LocationCode == locationTransaction.LocationCode))
				{
					ReworkBasketModel.ReworkOutProductModel?.Details?.Remove(ReworkBasketModel.ReworkOutProductModel?.Details.FirstOrDefault(x => x.LocationCode == locationTransaction.LocationCode));
				}
			}

			foreach (var locationTransaction in LocationTransactions.Where(x => x.OutputQuantity > 0))
			{
				if (ReworkBasketModel.ReworkOutProductModel.Details.Any(x => x.LocationCode == locationTransaction.LocationCode))
				{
					ReworkBasketModel.ReworkOutProductModel.Details.FirstOrDefault(x => x.LocationCode == locationTransaction.LocationCode).Quantity = locationTransaction.OutputQuantity;
				}
				else
				{
					ReworkBasketModel.ReworkOutProductModel.Details.Add(new ReworkOutProductDetailModel
					{
						LocationCode = locationTransaction.LocationCode,
						LocationReferenceId = locationTransaction.LocationReferenceId,
						Quantity = locationTransaction.OutputQuantity,
						RemainingQuantity = locationTransaction.RemainingQuantity
					});
				}
			}

			ReworkBasketModel.ReworkOutProductModel.OutputQuantity = locationTransacionTotalOutputQuantity;

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
		}
		catch (Exception ex)
		{
			if(_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

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


	private async Task AddProductTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(ManuelReworkProcessInWarehouseListView)}");
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

	public async Task ClearPageAsync()
	{
		try
		{
			await Task.Run(() =>
			{
				if(SelectedReworkInProductModel is not null)
				{
					SelectedReworkInProductModel.Details.Clear();
					SelectedReworkInProductModel = null;
				}
				if(ReworkBasketModel is not null)
				{
					ReworkBasketModel = null;
				}
			});
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}
}
