using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.OutsourceModels;
using Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.ViewModels;


[QueryProperty(name: nameof(InputOutsourceTransferV2BasketModel), queryId: nameof(InputOutsourceTransferV2BasketModel))]
public partial class InputOutsourceTransferV2BasketViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IUserDialogs _userDialogs;
	private readonly IServiceProvider _serviceProvider;
	private readonly ILocationTransactionService _locationTransactionService;

	[ObservableProperty]
	InputOutsourceTransferV2BasketModel? inputOutsourceTransferV2BasketModel;

	public ObservableCollection<GroupLocationTransactionModel> LocationTransactions { get; } = new();

	[ObservableProperty]
	InputOutsourceTransferSubProductModel? selectedSubProductModel;

	public InputOutsourceTransferV2BasketViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IServiceProvider serviceProvider, ILocationTransactionService locationTransactionService)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_serviceProvider = serviceProvider;
		_locationTransactionService = locationTransactionService;

		Title = "Fason Kabul Sepeti";

		LoadPageCommand = new Command(async () => await LoadPageAsync());
		IncreaseCommand = new Command(async () => await IncreaseAsync());
		DecreaseCommand = new Command(async () => await DecreaseAsync());
		QuantityTappedCommand = new Command(async () => await QuantityTappedAsync());

		SubProductIncreaseCommand = new Command<InputOutsourceTransferSubProductModel>(async (item) => await SubProductIncreaseAsync(item));
		SubProductDecreaseCommand = new Command<InputOutsourceTransferSubProductModel>(async (item) => await SubProductDecreaseAsync(item));
		SubProductQuantityTappedCommand = new Command<InputOutsourceTransferSubProductModel>(async (item) => await SubProductQuantityTappedAsync(item));

		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());

		LoadMoreLocationTransactionsCommand = new Command(async () => await LoadMoreLocationTransactionsAsync());
		LocationTransactionIncreaseCommand = new Command<GroupLocationTransactionModel>(async (item) => await LocationTransactionIncreaseAsync(item));
		LocationTransactionDecreaseCommand = new Command<GroupLocationTransactionModel>(async (item) => await LocationTransactionDecreaseAsync(item));
		LocationTransactionQuantityTappedCommand = new Command<GroupLocationTransactionModel>(async (item) => await LocationTransactionQuantityTappedAsync(item));
		LocationTransactionConfirmCommand = new Command(async () => await LocationTransactionConfirmAsync());
		LocationTransactionCloseCommand = new Command(async () => await LocationTransactionCloseAsync());
	}
	public Page CurrentPage { get; set; } = null!;
	public Command LoadPageCommand { get; }
	public Command IncreaseCommand { get; }
	public Command DecreaseCommand { get; }
	public Command QuantityTappedCommand { get; }

	public Command SubProductIncreaseCommand { get; }
	public Command SubProductDecreaseCommand { get; }
	public Command SubProductQuantityTappedCommand { get; }

	public Command NextViewCommand { get; }
	public Command BackCommand { get; }

	public Command LoadMoreLocationTransactionsCommand { get; }
	public Command LocationTransactionIncreaseCommand { get; }
	public Command LocationTransactionDecreaseCommand { get; }
	public Command LocationTransactionQuantityTappedCommand { get; }
	public Command LocationTransactionConfirmCommand { get; }
	public Command LocationTransactionCloseCommand { get; }


	private async Task LoadPageAsync()
	{
		if (InputOutsourceTransferV2BasketModel is null)
			return;
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			_userDialogs.Loading("Yükleniyor...");
			InputOutsourceTransferV2BasketModel?.InputOutsourceTransferSubProducts?.Clear();
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			// TODO: Alt ürünlerin yüklenmesi 

			ObservableCollection<InputOutsourceTransferSubProductModel> subProducts = new();
			subProducts.Add(new InputOutsourceTransferSubProductModel
			{
				Image = "",
				ProductCode = "Y.1245",
				ProductName = "Takoz",
				IsVariant = false,
				LocTracking = 0,
				StockQuantity = 50,
				WarehouseNumber = 2,
				WarehouseName = "Sevkiyat",
				OutputQuantity = 1,
				QuotientQuantity = 2,
				TotalQuantityWithQuotient = 2,
				Details = new()
			});

			subProducts.Add(new InputOutsourceTransferSubProductModel
			{
				Image = "",
				ProductCode = "Y.1246",
				ProductName = "Amortisör",
				IsVariant = false,
				LocTracking = 1,
				StockQuantity = 67,
				WarehouseNumber = 2,
				WarehouseName = "Sevkiyat",
				OutputQuantity = 0,
				QuotientQuantity = 3,
				TotalQuantityWithQuotient = 3,
				Details = new()
			});

			subProducts.Add(new InputOutsourceTransferSubProductModel
			{
				Image = "",
				ProductCode = "Y.1248",
				ProductName = "Direksiyon",
				IsVariant = false,
				LocTracking = 0,
				StockQuantity = 50,
				WarehouseNumber = 2,
				WarehouseName = "Sevkiyat",
				OutputQuantity = 0,
				QuotientQuantity = 1,
				TotalQuantityWithQuotient = 1,
				Details = new()
			});


			foreach (var item in subProducts)
            {
				InputOutsourceTransferV2BasketModel.InputOutsourceTransferSubProducts.Add(item);

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

	private async Task IncreaseAsync()
	{
		if (IsBusy)
			return;
		if (InputOutsourceTransferV2BasketModel is null || InputOutsourceTransferV2BasketModel?.InputOutsourceTransferMainProductModel is null)
			return;

		try
		{
			IsBusy = true;

			if (InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.LocTracking == 1)
			{
				var locationListViewModel = _serviceProvider.GetRequiredService<InputOutsourceTransferV2MainProductLocationListViewModel>();
				locationListViewModel.InputOutsourceTransferV2BasketModel = InputOutsourceTransferV2BasketModel;
				await locationListViewModel.LoadSelectedItemsAsync();

				await Shell.Current.GoToAsync($"{nameof(InputOutsourceTransferV2MainProductLocationListView)}", new Dictionary<string, object>
				{
					[nameof(InputOutsourceTransferV2BasketModel)] = InputOutsourceTransferV2BasketModel
				});
			}
			else
			{
				InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity += 1;

                // TODO: Sarf Malzemeler çarpan kadar miktarı arttırılacak
                foreach (var subProduct in InputOutsourceTransferV2BasketModel.InputOutsourceTransferSubProducts)
                {
					subProduct.TotalQuantityWithQuotient = subProduct.QuotientQuantity * InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity;
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

	private async Task DecreaseAsync()
	{
		if (IsBusy)
			return;

		if (InputOutsourceTransferV2BasketModel is null || InputOutsourceTransferV2BasketModel?.InputOutsourceTransferMainProductModel is null)
			return;
		try
		{
			IsBusy = true;

			if (InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.LocTracking == 1 && InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity == 0)
				return;

			if (InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.LocTracking == 0 && InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity == 1)
				return;

			if (InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.LocTracking == 1)
			{
				var locationListViewModel = _serviceProvider.GetRequiredService<InputOutsourceTransferV2MainProductLocationListViewModel>();
				locationListViewModel.InputOutsourceTransferV2BasketModel = InputOutsourceTransferV2BasketModel;
				await locationListViewModel.LoadSelectedItemsAsync();

				await Shell.Current.GoToAsync($"{nameof(InputOutsourceTransferV2MainProductLocationListView)}", new Dictionary<string, object>
				{
					[nameof(InputOutsourceTransferV2BasketModel)] = InputOutsourceTransferV2BasketModel
				});
			}
			else
			{
				// TODO: Sarf Malzemeler çarpan kadar miktarı kadar azaltılacak

				InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity -= 1;

				foreach (var subProduct in InputOutsourceTransferV2BasketModel.InputOutsourceTransferSubProducts)
				{
					subProduct.TotalQuantityWithQuotient = subProduct.QuotientQuantity * InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity;
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

	private async Task QuantityTappedAsync()
	{
		if (IsBusy)
			return;
		if (InputOutsourceTransferV2BasketModel is null || InputOutsourceTransferV2BasketModel?.InputOutsourceTransferMainProductModel is null)
			return;

		if (InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.LocTracking == 1) // Malzeme raf takipli ise bu fonksiyon çalışmamalı
			return;

		try
		{
			IsBusy = true;

			var result = await CurrentPage.DisplayPromptAsync(
				title: InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.ProductCode,
				message: "Miktarı giriniz",
				cancel: "Vazgeç",
				accept: "Tamam",
				placeholder: InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity.ToString(),
				keyboard: Keyboard.Numeric
			);

			if (string.IsNullOrEmpty(result))
				return;

			var quantity = Convert.ToDouble(result);

			if (quantity < 0)
			{
				await _userDialogs.AlertAsync("Girilen miktar 0'dan küçük olmamalıdır.", "Hata", "Tamam");
				return;
			}

			if (InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.LocTracking == 0 && quantity < 1)
			{
				await _userDialogs.AlertAsync("Girilen miktar 1'den küçük olmamalıdır.", "Hata", "Tamam");
				return;
			}


			InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity = quantity;
			// TODO: Sarf Malzemelerin quantityleri buradaki sayı kadar çarpılacak 

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

	private async Task SubProductIncreaseAsync(InputOutsourceTransferSubProductModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			SelectedSubProductModel = item;

			if(item.LocTracking == 1)
			{
				await LoadLocationTransactionsAsync();
				CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.FullExpanded;
			}
			
			if(item.LocTracking == 0)
			{
				if (item.StockQuantity <= item.OutputQuantity)
					return;

				item.OutputQuantity += 1;
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

	private async Task SubProductDecreaseAsync(InputOutsourceTransferSubProductModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			SelectedSubProductModel = item;

			if(item.LocTracking == 1)
			{
				if (item.OutputQuantity == 0)
					return;

				await LoadLocationTransactionsAsync();
				CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.FullExpanded;
			}

			if(item.LocTracking == 0)
			{
				if (item.OutputQuantity == 1)
					return;

				item.OutputQuantity -= 1;
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

	private async Task SubProductQuantityTappedAsync(InputOutsourceTransferSubProductModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;
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
		if (SelectedSubProductModel is null)
			return;

		try
		{
			_userDialogs.Loading("Loading Location Transaction Items...");
			LocationTransactions.Clear();
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationTransactionService.GetLocationTransactionsInputObjectsAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: SelectedSubProductModel.IsVariant ? SelectedSubProductModel.ProductReferenceId : SelectedSubProductModel.ProductReferenceId,
				variantReferenceId: SelectedSubProductModel.IsVariant ? SelectedSubProductModel.ProductReferenceId : 0,
				warehouseNumber: SelectedSubProductModel.WarehouseNumber,
				skip: 0,
				take: 20,
				search: ""
			);

			if(result.IsSuccess)
			{
				if (result.Data is null)
					return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<GroupLocationTransactionModel>(item);
					var matchingItem = SelectedSubProductModel.Details.FirstOrDefault(x => x.LocationReferenceId == obj.LocationReferenceId);
					obj.OutputQuantity = matchingItem?.Quantity ?? 0;

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

		if (SelectedSubProductModel is null)
			return;

		try
		{
			IsBusy = true;

			_userDialogs.Loading("Loading more Location Transaction Items...");
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationTransactionService.GetLocationTransactionsInputObjectsAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: SelectedSubProductModel.IsVariant ? SelectedSubProductModel.ProductReferenceId : SelectedSubProductModel.ProductReferenceId,
				variantReferenceId: SelectedSubProductModel.IsVariant ? SelectedSubProductModel.ProductReferenceId : 0,
				warehouseNumber: SelectedSubProductModel.WarehouseNumber,
				skip: 0,
				take: LocationTransactions.Count,
				search: ""
			);

			if(result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<GroupLocationTransactionModel>(item);
					var matchingItem = SelectedSubProductModel.Details.FirstOrDefault(x => x.LocationReferenceId == obj.LocationReferenceId);
					obj.OutputQuantity = matchingItem?.Quantity ?? 0;

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

	private async Task LocationTransactionIncreaseAsync(GroupLocationTransactionModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (item.RemainingQuantity <= item.OutputQuantity)
				return;

			item.OutputQuantity += 1;
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

	private async Task LocationTransactionDecreaseAsync(GroupLocationTransactionModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (item.OutputQuantity == 0)
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

	private async Task LocationTransactionQuantityTappedAsync(GroupLocationTransactionModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;
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

		if (SelectedSubProductModel is null)
			return;

		try
		{
			IsBusy = true;

			if(!LocationTransactions.Any())
			{
				CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
				return;
			}

			foreach(var item in LocationTransactions.Where(x => x.OutputQuantity <= 0))
			{
				SelectedSubProductModel.Details.Remove(SelectedSubProductModel.Details.FirstOrDefault(x => x.LocationReferenceId == item.LocationReferenceId));
			}

			foreach (var item in LocationTransactions.Where(x => x.OutputQuantity > 0))
			{
				var selectedLocationTransaction = SelectedSubProductModel.Details.FirstOrDefault(x => x.LocationReferenceId == item.LocationReferenceId);
				if (selectedLocationTransaction is not null)
				{
					selectedLocationTransaction.Quantity = item.OutputQuantity;
				}
				else
				{
					SelectedSubProductModel.Details.Add(new InputOutsourceTransferSubProductDetailModel
					{
						LocationReferenceId = item.LocationReferenceId,
						LocationCode = item.LocationCode,
						LocationName = item.LocationName,
						Quantity = item.OutputQuantity,
						RemainingQuantity = item.RemainingQuantity,
					});
				}
			}

			var totalOutputQuantity = LocationTransactions.Where(x => x.OutputQuantity > 0).Sum(x => x.OutputQuantity);
			SelectedSubProductModel.OutputQuantity = totalOutputQuantity;

			CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
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

	private async Task LocationTransactionCloseAsync()
	{
		await MainThread.InvokeOnMainThreadAsync(() =>
		{
			CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
		});
	}


	private async Task NextViewAsync()
	{
		if (InputOutsourceTransferV2BasketModel is null || InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel is null)
			return;

		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			Console.WriteLine(InputOutsourceTransferV2BasketModel);

			if(InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.LocTracking == 1 && InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity == 0)
			{
				_userDialogs.ShowToast($"Ana ürünün miktarı 0 olamaz");
				return;
			}

			if(!InputOutsourceTransferV2BasketModel.InputOutsourceTransferSubProducts.Any())
			{
				_userDialogs.ShowToast($"Herhangi bir sarf malzemesi bulunamadı, işleme devam edemezsiniz!");
				return;
			}

            foreach (var item in InputOutsourceTransferV2BasketModel.InputOutsourceTransferSubProducts)
            {
				if(item.TotalQuantityWithQuotient > item.StockQuantity)
				{
					_userDialogs.ShowToast($"Toplam katsayı miktarı, ilgili sarf malzemenin stok miktarını geçemez!");
					return;
				}
				else if(item.OutputQuantity > item.StockQuantity)
				{
					_userDialogs.ShowToast($"Girilen miktar, ilgili sarf malzemenin stok miktarını geçemez!");
					return;
				}
				else if(item.OutputQuantity > item.TotalQuantityWithQuotient)
				{
					_userDialogs.ShowToast($"Girilen miktar, ilgili sarf malzemenin toplam katsayı miktarını geçemez!");
					return;
				}
				else if(item.OutputQuantity != item.TotalQuantityWithQuotient)
				{
					_userDialogs.ShowToast($"Girilen miktar, ilgili sarf malzemenin toplam katsayı miktarına eşit olmak zorunda!");
					return;
				}
            }

            await Shell.Current.GoToAsync($"{nameof(InputOutsourceTransferV2FormView)}", new Dictionary<string, object>
			{
				[nameof(InputOutsourceTransferV2BasketModel)] = InputOutsourceTransferV2BasketModel
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
}
