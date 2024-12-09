using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.OutsourceModels;
using Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.Views;
using DevExpress.Data.Async.Helpers;
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
	private readonly IInputOutsourceTransferV2SubProductService _inputOutsourceTransferV2SubProductService;
	private readonly ILocationService _locationService;

	[ObservableProperty]
	InputOutsourceTransferV2BasketModel? inputOutsourceTransferV2BasketModel;

	//public ObservableCollection<GroupLocationTransactionModel> LocationTransactions { get; } = new();
	public ObservableCollection<LocationModel> Locations { get; } = new();

	[ObservableProperty]
	InputOutsourceTransferSubProductModel? selectedSubProductModel;

	public InputOutsourceTransferV2BasketViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IServiceProvider serviceProvider, ILocationTransactionService locationTransactionService, IInputOutsourceTransferV2SubProductService inputOutsourceTransferV2SubProductService, ILocationService locationService)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_serviceProvider = serviceProvider;
		_locationTransactionService = locationTransactionService;
		_inputOutsourceTransferV2SubProductService = inputOutsourceTransferV2SubProductService;
		_locationService = locationService;

		Title = "Fason Kabul Sepeti";

		LoadPageCommand = new Command(async () => await LoadPageAsync());
		IncreaseCommand = new Command(async () => await IncreaseAsync());
		DecreaseCommand = new Command(async () => await DecreaseAsync());
		QuantityTappedCommand = new Command(async () => await QuantityTappedAsync());

		SubProductTappedCommand = new Command<InputOutsourceTransferSubProductModel>(async (item) => await SubProductTappedAsync(item));
		SubProductIncreaseCommand = new Command<InputOutsourceTransferSubProductModel>(async (item) => await SubProductIncreaseAsync(item));
		SubProductDecreaseCommand = new Command<InputOutsourceTransferSubProductModel>(async (item) => await SubProductDecreaseAsync(item));
		SubProductQuantityTappedCommand = new Command<InputOutsourceTransferSubProductModel>(async (item) => await SubProductQuantityTappedAsync(item));

		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());

		LoadMoreLocationsCommand = new Command(async () => await LoadMoreLocationsAsync());
		LocationIncreaseCommand = new Command<LocationModel>(async (item) => await LocationIncreaseAsync(item));
		LocationDecreaseCommand = new Command<LocationModel>(async (item) => await LocationDecreaseAsync(item));
		LocationQuantityTappedCommand = new Command<LocationModel>(async (item) => await LocationQuantityTappedAsync(item));
		LocationConfirmCommand = new Command(async () => await LocationConfirmAsync());
		LocationCloseCommand = new Command(async () => await LocationCloseAsync());
	}
	public Page CurrentPage { get; set; } = null!;
	public Command LoadPageCommand { get; }
	public Command IncreaseCommand { get; }
	public Command DecreaseCommand { get; }
	public Command QuantityTappedCommand { get; }

	public Command SubProductTappedCommand { get; }
	public Command SubProductIncreaseCommand { get; }
	public Command SubProductDecreaseCommand { get; }
	public Command SubProductQuantityTappedCommand { get; }

	public Command NextViewCommand { get; }
	public Command BackCommand { get; }

	public Command LoadMoreLocationsCommand { get; }
	public Command LocationIncreaseCommand { get; }
	public Command LocationDecreaseCommand { get; }
	public Command LocationQuantityTappedCommand { get; }
	public Command LocationConfirmCommand { get; }
	public Command LocationCloseCommand { get; }


	private async Task LoadPageAsync()
	{
		if (InputOutsourceTransferV2BasketModel is null || InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel is null)
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
			var result = await _inputOutsourceTransferV2SubProductService.GetObjects(
				httpClient: httpClient, 
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				mainProductReferenceId: InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.ProductReferenceId
			);


			if(result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<InputOutsourceTransferSubProductModel>(item);
					obj.TotalBOMQuantity = InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.LocTracking == 1 ? obj.BOMQuantity : (obj.BOMQuantity * InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity);
					obj.OutputQuantity = InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity * obj.BOMQuantity;
					obj.Details = new List<InputOutsourceTransferSubProductDetailModel>();

					InputOutsourceTransferV2BasketModel.InputOutsourceTransferSubProducts.Add(obj);
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
				if(InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity >= InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.PlanningQuantity)
				{
					_userDialogs.ShowToast($"Girilen miktar ({InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity}), ürünün planlanan miktarını ({InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.PlanningQuantity}) geçemez.");
					return;

				}

				InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity += 1;

                foreach (var subProduct in InputOutsourceTransferV2BasketModel.InputOutsourceTransferSubProducts)
                {
					subProduct.TotalBOMQuantity = subProduct.BOMQuantity * InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity;
					subProduct.OutputQuantity = subProduct.TotalBOMQuantity;
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

			if (InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.LocTracking == 0 && InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity == 0)
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
				InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity -= 1;

				foreach (var subProduct in InputOutsourceTransferV2BasketModel.InputOutsourceTransferSubProducts)
				{
					subProduct.TotalBOMQuantity = InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity == 0 ? subProduct.BOMQuantity :  subProduct.BOMQuantity * InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity;
					subProduct.OutputQuantity = InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity == 0 ? 0 : subProduct.TotalBOMQuantity;
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

			if (quantity <= 0)
			{
				await _userDialogs.AlertAsync("Girilen miktar 0'dan büyük olmalıdır.", "Hata", "Tamam");
				return;
			}

			//if (InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.LocTracking == 0 && quantity < 1)
			//{
			//	await _userDialogs.AlertAsync("Girilen miktar 1'den küçük olmamalıdır.", "Hata", "Tamam");
			//	return;
			//}

			if (quantity > InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.PlanningQuantity)
			{
				_userDialogs.ShowToast($"Girilen miktar ({quantity}), ürünün planlanan miktarını ({InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.PlanningQuantity}) geçemez.");
				return;
			}

			InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity = quantity;

            foreach (var subProduct in InputOutsourceTransferV2BasketModel.InputOutsourceTransferSubProducts)
            {
				subProduct.TotalBOMQuantity = subProduct.BOMQuantity * InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity;
				subProduct.OutputQuantity = subProduct.TotalBOMQuantity;
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

	private async Task SubProductTappedAsync(InputOutsourceTransferSubProductModel item)
	{
		if (IsBusy)
			return;
		if (item.LocTracking == 0)
			return;
		try
		{
			IsBusy = true;

			SelectedSubProductModel = item;

			await LoadLocationsAsync();
			CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.FullExpanded;
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
				await LoadLocationsAsync();
				CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.FullExpanded;
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

				await LoadLocationsAsync();
				CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.FullExpanded;
			}

			if(item.LocTracking == 0)
			{
				if (item.OutputQuantity == 0)
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
		if (item.LocTracking == 1)
			return;
		try
		{
			IsBusy = true;

			SelectedSubProductModel = item;

			var result = await CurrentPage.DisplayPromptAsync(
				title: item.ProductCode,
				message: "Miktarı giriniz",
				cancel: "Vazgeç",
				accept: "Tamam",
				placeholder: item.OutputQuantity.ToString(),
				keyboard: Keyboard.Numeric
			);

			if (string.IsNullOrEmpty(result))
				return;

			var quantity = Convert.ToDouble(result);

			if (quantity < 0)
			{
				_userDialogs.ShowToast("Girilen miktar 0'dan küçük olmamalıdır.");
				return;
			}

			if (quantity > item.StockQuantity)
			{
				_userDialogs.ShowToast($"Girilen miktar ({quantity}), ürünün ({item.ProductCode}) stok miktarını ({item.StockQuantity}) geçemez.");
				return;
			}

			item.OutputQuantity = quantity;
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

	private async Task LoadLocationsAsync()
	{
		if (SelectedSubProductModel is null)
			return;

		try
		{
			_userDialogs.Loading("Loading...");
			Locations.Clear();
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationService.GetLocationsWithStock(
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
                    var obj = Mapping.Mapper.Map<LocationModel>(item);
					var matchingItem = SelectedSubProductModel.Details.FirstOrDefault(x => x.LocationReferenceId == obj.ReferenceId);
					obj.InputQuantity = matchingItem?.Quantity ?? 0;
					Locations.Add(obj);
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

	private async Task LoadMoreLocationsAsync()
	{
		if (IsBusy)
			return;

		if (Locations.Count < 18)
			return;

		if (SelectedSubProductModel is null)
			return;

		try
		{
			IsBusy = true;

			_userDialogs.Loading("Loading...");
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationService.GetLocationsWithStock(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: SelectedSubProductModel.IsVariant ? SelectedSubProductModel.ProductReferenceId : SelectedSubProductModel.ProductReferenceId,
				variantReferenceId: SelectedSubProductModel.IsVariant ? SelectedSubProductModel.ProductReferenceId : 0,
				warehouseNumber: SelectedSubProductModel.WarehouseNumber,
				skip: Locations.Count,
				take: 20,
				search: ""
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<LocationModel>(item);
					var matchingItem = SelectedSubProductModel.Details.FirstOrDefault(x => x.LocationReferenceId == obj.ReferenceId);
					obj.InputQuantity = matchingItem?.Quantity ?? 0;
					Locations.Add(obj);

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

	private async Task LocationIncreaseAsync(LocationModel item)
	{
		if (IsBusy)
			return;
		if (item is null)
			return;
		try
		{
			IsBusy = true;

			if (item.StockQuantity <= item.InputQuantity)
				return;

			item.InputQuantity += 1;
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

	private async Task LocationDecreaseAsync(LocationModel item)
	{
		if (IsBusy)
			return;
		if (item is null)
			return;
		try
		{
			IsBusy = true;

			if (item.InputQuantity == 0)
				return;

			item.InputQuantity -= 1;
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

	private async Task LocationQuantityTappedAsync(LocationModel item)
	{
		if (IsBusy)
			return;
		if (item is null)
			return;
		try
		{
			IsBusy = true;

			var result = await CurrentPage.DisplayPromptAsync(
				title: item.Code,
				message: "Miktarı giriniz",
				cancel: "Vazgeç",
				accept: "Tamam",
				placeholder: item.InputQuantity.ToString(),
				keyboard: Keyboard.Numeric
			);

			if (string.IsNullOrEmpty(result))
				return;

			var quantity = Convert.ToDouble(result);

			if (quantity < 0)
			{
				_userDialogs.ShowToast("Girilen miktar 0'dan küçük olmamalıdır.");
				return;
			}

			if (quantity > item.StockQuantity)
			{
				_userDialogs.ShowToast($"Girilen miktar, rafın stok miktarını ({item.StockQuantity}) geçemez.");
				return;
			}

			if (quantity > SelectedSubProductModel?.StockQuantity)
			{
				_userDialogs.ShowToast($"Girilen miktar, ilgili ürünün ({SelectedSubProductModel.ProductCode}) stok miktarını {SelectedSubProductModel.StockQuantity} aşmamalıdır.");
				return;
			}

			var totalQuantity = Locations.Where(x => x.Code != item.Code).Sum(x => x.InputQuantity);
			if (totalQuantity + quantity > SelectedSubProductModel?.StockQuantity)
			{
				_userDialogs.ShowToast($"Toplam girilen miktar, ilgili ürünün ({SelectedSubProductModel.ProductCode}) stok miktarını ({SelectedSubProductModel.StockQuantity}) aşmamalıdır.");
				return;
			}

			item.InputQuantity = quantity;

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

	private async Task LocationConfirmAsync()
	{
		if (IsBusy)
			return;

		if (SelectedSubProductModel is null)
			return;

		try
		{
			IsBusy = true;

			if(!Locations.Any())
			{
				CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
				return;
			}

			foreach(var item in Locations.Where(x => x.InputQuantity <= 0))
			{
				SelectedSubProductModel.Details.Remove(SelectedSubProductModel.Details.FirstOrDefault(x => x.LocationReferenceId == item.ReferenceId));
			}

			foreach (var item in Locations.Where(x => x.InputQuantity > 0))
			{
				var selectedLocationTransaction = SelectedSubProductModel.Details.FirstOrDefault(x => x.LocationReferenceId == item.ReferenceId);
				if (selectedLocationTransaction is not null)
				{
					selectedLocationTransaction.Quantity = item.InputQuantity;
				}
				else
				{
					SelectedSubProductModel.Details.Add(new InputOutsourceTransferSubProductDetailModel
					{
						LocationReferenceId = item.ReferenceId,
						LocationCode = item.Code,
						LocationName = item.Name,
						Quantity = item.InputQuantity,
					});
				}
			}

			//var totalInputQuantity = Locations.Where(x => x.InputQuantity > 0).Sum(x => x.InputQuantity);
			//SelectedSubProductModel.OutputQuantity = totalInputQuantity;

			CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
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

	private async Task LocationCloseAsync()
	{
		await MainThread.InvokeOnMainThreadAsync(() =>
		{
			CurrentPage.FindByName<BottomSheet>("locationBottomSheet").State = BottomSheetState.Hidden;
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

			if(InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.InputQuantity == 0)
			{
				_userDialogs.ShowToast($"Ana ürünün ({InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.ProductCode}) miktarı 0 olamaz");
				return;
			}

			if(!InputOutsourceTransferV2BasketModel.InputOutsourceTransferSubProducts.Any())
			{
				_userDialogs.ShowToast($"Herhangi bir sarf malzemesi bulunamadı, işleme devam edemezsiniz!");
				return;
			}

            foreach (var item in InputOutsourceTransferV2BasketModel.InputOutsourceTransferSubProducts)
            {
				var itemDetailTotalQuantity = item.Details.Sum(x => x.Quantity);
				if (item.TotalBOMQuantity > item.StockQuantity)
				{
					_userDialogs.ShowToast($"Toplam katsayı miktarı ({item.TotalBOMQuantity}), ilgili sarf malzemenin stok miktarını ({item.StockQuantity}) geçemez!");
					return;
				}
				else if(item.OutputQuantity > item.StockQuantity)
				{
					_userDialogs.ShowToast($"Girilen miktar ({item.OutputQuantity}), ilgili sarf malzemenin ({item.ProductCode}) stok miktarını ({item.StockQuantity}) geçemez!");
					return;
				}
				else if(item.OutputQuantity > item.TotalBOMQuantity)
				{
					_userDialogs.ShowToast($"Girilen miktar ({item.OutputQuantity}), ilgili sarf malzemenin ({item.ProductCode}) toplam katsayı miktarını ({item.TotalBOMQuantity}) geçemez!");
					return;
				}
				else if(item.OutputQuantity != item.TotalBOMQuantity)
				{
					_userDialogs.ShowToast($"Girilen miktar ({item.OutputQuantity}), ilgili sarf malzemenin ({item.ProductCode}) toplam katsayı miktarına ({item.TotalBOMQuantity}) eşit olmak zorunda!");
					return;
				}
				else if(item.LocTracking == 1 && item.OutputQuantity != itemDetailTotalQuantity)
				{
					_userDialogs.ShowToast($"Ürünün ({item.ProductCode})  miktarı ({item.OutputQuantity}) alt ürünün girilen raf miktarına ({itemDetailTotalQuantity}) eşit olmalıdır!");
					return;
				}
            }

			var confirm = await _userDialogs.ConfirmAsync("Devam etmek istediğinize emin misiniz?", "Onay", "Evet", "Hayır");
			if (!confirm)
				return;

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
		if (InputOutsourceTransferV2BasketModel is null)
			return;
		try
		{
			IsBusy = true;

			var confirm = await _userDialogs.ConfirmAsync("İşlemi iptal etmek istediğinize emin misiniz?", "Onay", "Evet", "Hayır");
			if (!confirm)
				return;

			if(InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.Details is not null)
			{
				InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel.Details.Clear();
			}
			
			InputOutsourceTransferV2BasketModel.InputOutsourceTransferMainProductModel = null;

			foreach (var item in InputOutsourceTransferV2BasketModel.InputOutsourceTransferSubProducts)
			{
				item.Details.Clear();
			}

			InputOutsourceTransferV2BasketModel.InputOutsourceTransferSubProducts.Clear();

			SelectedSubProductModel = null;
			Locations.Clear();

			await Shell.Current.GoToAsync("..");

			var previousViewModel = _serviceProvider.GetRequiredService<InputOutsourceTransferV2ProductListViewModel>();
			previousViewModel.WarehouseModel = InputOutsourceTransferV2BasketModel.OutsourceWarehouseModel;
			previousViewModel.OutsourceModel = InputOutsourceTransferV2BasketModel.OutsourceModel;
			IsBusy = false;
			await previousViewModel.LoadItemsAsync();

			InputOutsourceTransferV2BasketModel = null;
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
