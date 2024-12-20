using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BarcodeModels;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.CameraModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.SeriLotModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.BarcodeHelper;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CameraModule.Views;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.ViewModels;

[QueryProperty(name: nameof(OutputProductProcessType), queryId: nameof(OutputProductProcessType))]
[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
public partial class OutputProductProcessBasketListViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly ISeriLotTransactionService _serilotTransactionService;
	private readonly ILocationTransactionService _locationTransactionService;
	private readonly IUserDialogs _userDialogs;
	private readonly IServiceProvider _serviceProvider;
	private readonly IBarcodeSearchOutHelper _barcodeSearchOutHelper;
	private readonly ISubUnitsetService _subUnitsetService;

    [ObservableProperty]
	OutputProductProcessType outputProductProcessType;

	[ObservableProperty]
	WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	OutputProductBasketModel? selectedItem;

	[ObservableProperty]
	SeriLotTransactionModel? selectedSeriLotTransaction;

	[ObservableProperty]
	GroupLocationTransactionModel? selectedLocationTransaction;

	[ObservableProperty]
	public ObservableCollection<SeriLotTransactionModel> selectedSeriLotTransactions = new();

	[ObservableProperty]
	public ObservableCollection<GroupLocationTransactionModel> selectedLocationTransactions = new();

	[ObservableProperty]
	public Entry barcodeEntry;

	[ObservableProperty]
	public SearchBar locationTransactionSearchText;

	#region Collections
	public ObservableCollection<OutputProductBasketModel> Items { get; } = new();
	public ObservableCollection<OutputProductBasketModel> SelectedItems { get; } = new();

	public ObservableCollection<SeriLotTransactionModel> SeriLotTransactions { get; } = new();
	public ObservableCollection<GroupLocationTransactionModel> LocationTransactions { get; } = new();
	#endregion

	public OutputProductProcessBasketListViewModel(IHttpClientService httpClientService, ISeriLotTransactionService serilotTransactionService, ILocationTransactionService locationTransactionService, IUserDialogs userDialogs, IServiceProvider serviceProvider, ISubUnitsetService subUnitsetService, IBarcodeSearchOutHelper barcodeSearchOutHelper)
	{
		_httpClientService = httpClientService;
		_serilotTransactionService = serilotTransactionService;
		_locationTransactionService = locationTransactionService;
		_userDialogs = userDialogs;
		_serviceProvider = serviceProvider;
		_subUnitsetService = subUnitsetService;
		_barcodeSearchOutHelper = barcodeSearchOutHelper;


		Title = "Sepet Listesi";

		ShowProductViewCommand = new Command(async () => await ShowProductViewAsync());
		PerformSearchCommand = new Command<Entry>(async (x) => await PerformSearchAsync(x));
		UnitActionTappedCommand = new Command<OutputProductBasketModel>(async (item) => await UnitActionTappedAsync(item));
		SubUnitsetTappedCommand = new Command<SubUnitset>(async (item) => await SubUnitsetTappedAsync(item));

		QuantityTappedCommand = new Command<OutputProductBasketModel>(async (item) => await QuantityTappedAsync(item));
		IncreaseCommand = new Command<OutputProductBasketModel>(async (item) => await IncreaseAsync(item));
		DecreaseCommand = new Command<OutputProductBasketModel>(async (item) => await DecreaseAsync(item));
		DeleteItemCommand = new Command<OutputProductBasketModel>(async (item) => await DeleteItemAsync(item));


		LoadMoreSeriLotTransactionsCommand = new Command(async () => await LoadMoreSeriLotTransactionsAsync());
		SeriLotTransactionIncreaseCommand = new Command<SeriLotTransactionModel>(item => SeriLotTransactionIncreaseAsync(item));
		SeriLotTransactionDecreaseCommand = new Command<SeriLotTransactionModel>(item => SeriLotTransactionDecreaseAsync(item));
		ConfirmSeriLotTransactionCommand = new Command(ConfirmSeriLotTransactionAsync);
		SeriLotTransactionCloseCommand = new Command(async () => await SeriLotTransactionCloseAsync());


		LoadMoreLocationTransactionsCommand = new Command(async () => await LoadMoreLocationTransactionsAsync());
		LocationTransactionIncreaseCommand = new Command<GroupLocationTransactionModel>(async (item) => await LocationTransactionIncreaseAsync(item));
		LocationTransactionDecreaseCommand = new Command<GroupLocationTransactionModel>(async (item) => await LocationTransactionDecreaseAsync(item));
		LocationTransactionQuantityTappedCommand = new Command<GroupLocationTransactionModel>(async (item) => await LocationTransactionQuantityTappedAsync(item));
		ConfirmLocationTransactionCommand = new Command(ConfirmLocationTransactionAsync);
		LocationTransactionCloseCommand = new Command(async () => await LocationTransactionCloseAsync());

		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());
		CameraTappedCommand = new Command(async () => await CameraTappedAsync());
		LocationTransactionPerformSearchCommand = new Command(async () => await LocationTransactionPerformSearchAsync());
		LocationTransactionPerformEmptySearchCommand = new Command(async () => await LocationTransactionPerformEmptySearchAsync());
		
	}

	#region Properties
	public ContentPage CurrentPage { get; set; } = null!;

	#endregion

	#region Commands
	public Command ShowProductViewCommand { get; }
	public Command PerformSearchCommand { get; }
	public Command UnitActionTappedCommand { get; }
	public Command SubUnitsetTappedCommand { get; }
	public Command QuantityTappedCommand { get; }
	public Command IncreaseCommand { get; }
	public Command DecreaseCommand { get; }
	public Command DeleteItemCommand { get; }

	#region Location Transaction Command
	public Command LoadMoreLocationTransactionsCommand { get; }
	public Command LocationTransactionQuantityTappedCommand { get; }
	public Command LocationTransactionIncreaseCommand { get; }
	public Command LocationTransactionDecreaseCommand { get; }
	public Command ConfirmLocationTransactionCommand { get; }
	public Command LocationTransactionCloseCommand { get; }
  public Command LocationTransactionPerformSearchCommand { get; }
  public Command LocationTransactionPerformEmptySearchCommand { get; }
	#endregion

	#region SeriLotTransaction Command
	public Command LoadMoreSeriLotTransactionsCommand { get; }
	public Command SeriLotTransactionIncreaseCommand { get; }
	public Command SeriLotTransactionDecreaseCommand { get; }
	public Command ConfirmSeriLotTransactionCommand { get; }
	public Command SeriLotTransactionCloseCommand { get; }
	#endregion
	public Command NextViewCommand { get; }
	public Command BackCommand { get; }
	public Command CameraTappedCommand { get; }
	#endregion

   
    private async Task ShowProductViewAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await Shell.Current.GoToAsync($"{nameof(OutputProductProcessProductListView)}", new Dictionary<string, object>
			{
				[nameof(WarehouseModel)] = WarehouseModel
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

			var result = await _barcodeSearchOutHelper.BarcodeDetectedAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				barcode: barcodeEntry.Text,
				warehouseNumber: WarehouseModel.Number
			);

			if (result is not null)
			{
				Type resultType = result.GetType();
				if (resultType == typeof(BarcodeOutProductModel))
				{
					if (Items.Any(x => x.ItemCode == result.ProductCode))
					{
						_userDialogs.ShowToast($"{barcodeEntry.Text} barkodlu ürün sepetinizde zaten bulunmaktadır.");
						return;
					}
					else
					{
						var basketItem = new OutputProductBasketModel
						{
							ReferenceId = Guid.NewGuid(),
							ItemReferenceId = result.ProductReferenceId,
							ItemCode = result.ProductCode,
							ItemName = result.ProductName,
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
						};

						Items.Add(basketItem);
					}
				}
				//else if (resultType == typeof(VariantModel))
				//{
				//	if (Items.Any(x => x.ItemCode == result.Code))
				//	{
				//		_userDialogs.ShowToast($"{barcodeEntry.Text} barkodlu ürün sepetinizde zaten bulunmaktadır.");
				//		return;
				//	}
				//	else
				//	{
				//		var basketItem = new OutputProductBasketModel
				//		{
				//			ReferenceId = Guid.NewGuid(),
				//			ItemReferenceId = result.ReferenceId,
				//			ItemCode = result.Code,
				//			ItemName = result.Name,
				//			//Image = result.ImageData,
				//			UnitsetReferenceId = result.UnitsetReferenceId,
				//			UnitsetCode = result.UnitsetCode,
				//			UnitsetName = result.UnitsetName,
				//			SubUnitsetReferenceId = result.SubUnitsetReferenceId,
				//			SubUnitsetCode = result.SubUnitsetCode,
				//			SubUnitsetName = result.SubUnitsetName,
				//			IsSelected = false,
				//			MainItemCode = result.ProductCode,
				//			MainItemName = result.ProductName,
				//			MainItemReferenceId = result.ProductReferenceId,
				//			StockQuantity = result.StockQuantity,
				//			Quantity = result.LocTracking == 0 ? 1 : 0,
				//			LocTracking = result.LocTracking,
				//			TrackingType = result.TrackingType,
				//			IsVariant = true,
				//		};

				//		Items.Add(basketItem);
				//	}
				//}
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
			IsBusy = false;
			barcodeEntry.Focus();
		}
	}

	private async Task LoadSubUnitsetsAsync(OutputProductBasketModel item)
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

			if (SelectedItem is not null)
			{
				SelectedItem.SubUnitsetReferenceId = subUnitset.ReferenceId;
				SelectedItem.SubUnitsetName = subUnitset.Name;
				SelectedItem.SubUnitsetCode = subUnitset.Code;
				SelectedItem.ConversionFactor = subUnitset.ConversionValue;
				SelectedItem.OtherConversionFactor = subUnitset.OtherConversionValue;
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

	private async Task UnitActionTappedAsync(OutputProductBasketModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			SelectedItem = item;
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

	private async Task DeleteItemAsync(OutputProductBasketModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;
			var result = await _userDialogs.ConfirmAsync($"{item.ItemCode}\n{item.ItemName}\nİlgili ürün sepetinizden çıkarılacaktır. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");

			if (!result)
				return;

			if (SelectedItem == item)
			{
				SelectedItem.IsSelected = false;
				SelectedItem = null;
			}

			Items.Remove(item);
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

	private async Task IncreaseAsync(OutputProductBasketModel item)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			if (item is not null)
			{
				SelectedItem = item;

				// Stok Yeri takipli ise locationTransactionBottomSheet aç
				if (item.LocTracking == 1)
				{
					await LoadLocationTransactionsAsync();
					CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.FullExpanded;
				}
				// Sadece SeriLot takipli ise serilotTransactionBottomSheet aç
				else if(item.LocTracking == 0 && (item.TrackingType == 1 || item.TrackingType == 2))
				{
					await LoadSeriLotTransactionsAsync();
					CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.FullExpanded;
				}
				// Stok yeri ve SeriLot takipli değilse
				else
				{
					if(item.Quantity < item.StockQuantity)
						item.Quantity++;
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

	private async Task DecreaseAsync(OutputProductBasketModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if(item is not null)
			{
				SelectedItem = item;
				if (item.Quantity > 1)
				{
					// Stok Yeri takipli ise locationTransactionBottomSheet aç
					if(item.LocTracking == 1)
					{
						await LoadLocationTransactionsAsync();
						CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.FullExpanded;
					}
					// Sadece SeriLot takipli ise serilotTransactionBottomSheet aç
					else if (item.LocTracking == 0 && (item.TrackingType == 1 || item.TrackingType == 2))
					{
						await LoadSeriLotTransactionsAsync();
						CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.FullExpanded;
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
	private async Task QuantityTappedAsync(OutputProductBasketModel item)
	{
		if (IsBusy)
			return;
		if (item is null)
			return;
		if (item.LocTracking == 1)
			return;
		try
		{
			IsBusy = true;

            var result = await CurrentPage.DisplayPromptAsync(
				title: item.ItemCode,
				message: "Miktarı giriniz",
				cancel: "Vazgeç",
				accept: "Tamam",
				placeholder: item.Quantity.ToString(),
				keyboard: Keyboard.Numeric);

			if (string.IsNullOrEmpty(result))
				return;

			var quantity = Convert.ToDouble(result);
			if (quantity < 0)
			{
				await _userDialogs.AlertAsync("Girilen miktar 0'dan küçük olmamalıdır.", "Hata", "Tamam");
				return;
			}

			if (quantity > item.StockQuantity)
			{
				await _userDialogs.AlertAsync("Girilen miktar, stok miktarını aşmamalıdır.", "Hata", "Tamam");
				return;
			}

			item.Quantity = quantity;
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
		try
		{
			_userDialogs.ShowLoading("Load Location Items...");
			LocationTransactions.Clear();
			await Task.Delay(1000);

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationTransactionService.GetLocationTransactionsInputObjectsAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
                productReferenceId: SelectedItem.IsVariant ? SelectedItem.MainItemReferenceId : SelectedItem.ItemReferenceId,
                variantReferenceId: SelectedItem.IsVariant ? SelectedItem.ItemReferenceId : 0,
                warehouseNumber: WarehouseModel.Number,
				skip: 0,
				take: 20,
				search:LocationTransactionSearchText.Text,
				externalDb: _httpClientService.ExternalDatabase
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
                    var matchingItem = SelectedItem.Details.FirstOrDefault(item => item.LocationReferenceId == locationTransaction.LocationReferenceId);
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
			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task LoadMoreLocationTransactionsAsync()
	{
		if (IsBusy)
			return;
		if (LocationTransactions.Count < 18)
			return;
		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationTransactionService.GetLocationTransactionsInputObjectsAsync(httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId:SelectedItem.IsVariant ?  SelectedItem.MainItemReferenceId : SelectedItem.ItemReferenceId,
				variantReferenceId:SelectedItem.IsVariant ? SelectedItem.ItemReferenceId : 0,
                warehouseNumber: WarehouseModel.Number,
				skip: LocationTransactions.Count,
				take: 20,
                search:LocationTransactionSearchText.Text,
				externalDb: _httpClientService.ExternalDatabase);

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
                    var matchingItem = SelectedItem.Details.FirstOrDefault(item => item.LocationReferenceId == locationTransaction.LocationReferenceId);
                    if (matchingItem != null)
                    {
                        locationTransaction.OutputQuantity = matchingItem.Quantity;
                    }
                }

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

    private async Task LocationTransactionPerformSearchAsync()
    {
        if (IsBusy)
            return;

        try
        {
            if (string.IsNullOrWhiteSpace(LocationTransactionSearchText.Text))
            {
                await LoadLocationTransactionsAsync();
                LocationTransactionSearchText.Unfocus();
                return;
            }
            IsBusy = true;

            LocationTransactions.Clear();

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationTransactionService.GetLocationTransactionsInputObjectsAsync(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                productReferenceId: SelectedItem.ItemReferenceId,
                warehouseNumber: WarehouseModel.Number,
                skip: 0,
                take: 20,
                search: LocationTransactionSearchText.Text,
				externalDb: _httpClientService.ExternalDatabase
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
                    var matchingItem = SelectedItem.Details.FirstOrDefault(item => item.LocationReferenceId == locationTransaction.LocationReferenceId);
                    if (matchingItem != null)
                    {
                        locationTransaction.OutputQuantity = matchingItem.Quantity;
                    }
                }


            }
        }
        catch (System.Exception ex)
        {
            _userDialogs.Alert(ex.Message, "Hata");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LocationTransactionPerformEmptySearchAsync()
    {
        if (string.IsNullOrWhiteSpace(LocationTransactionSearchText.Text))
        {
            await LocationTransactionPerformSearchAsync();
        }
    }

	private async Task LocationTransactionQuantityTappedAsync(GroupLocationTransactionModel item)
	{
		if (item is null)
			return;
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
				await _userDialogs.AlertAsync("Girilen miktar, kalan miktarı aşmamalıdır.", "Hata", "Tamam");
				return;
			}
			if(quantity > SelectedItem?.StockQuantity)
			{
				await _userDialogs.AlertAsync("Girilen miktar, stok miktarını aşmamalıdır.", "Hata", "Tamam");
				return;
			}

			var totalQuantity = LocationTransactions.Where(x => x.LocationReferenceId != item.LocationReferenceId).Sum(x => x.OutputQuantity);
			if(SelectedItem?.StockQuantity >= totalQuantity + quantity)
			{
				item.OutputQuantity = quantity;
			}
			else
			{
				await _userDialogs.AlertAsync($"Girilen miktar, Ürünün stok miktarını ({SelectedItem?.StockQuantity}) aşmamalıdır.", "Hata", "Tamam");
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
		try
		{
			IsBusy = true;

			if (item is not null)
			{
				//SelectedLocationTransaction = item;
				// SeriLot takipli ise serilotTransactionBottomSheet aç
				if (SelectedItem.TrackingType == 1 || SelectedItem.TrackingType == 2) {
					await LoadSeriLotTransactionsAsync();
					CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.FullExpanded;
				}
				else
				{
					var totalQuantity = LocationTransactions.Sum(x => x.OutputQuantity);

					if(SelectedItem.StockQuantity > totalQuantity)
					{
						if (item.OutputQuantity < item.RemainingQuantity && SelectedItem.StockQuantity > item.OutputQuantity)
							item.OutputQuantity++;
					}
				}

				if (item.OutputQuantity > 0 && !item.IsSelected)
					item.IsSelected = true;

			}
		}
		catch(Exception ex)
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

	private async Task LocationTransactionDecreaseAsync(GroupLocationTransactionModel item)
	{
		if (IsBusy)
			return;

		try
		{
			IsBusy = true;

			if (item is not null)
			{
				// SeriLot takipli ise serilotTransactionBottomSheet aç
				if(SelectedItem.TrackingType == 1 || SelectedItem.TrackingType == 2)
				{
					await LoadSeriLotTransactionsAsync();
					CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.FullExpanded;
				}
				// SeriLot takipli değilse
				else
				{
					if(item.OutputQuantity > 0)
						item.OutputQuantity--;

					if(item.OutputQuantity == 0)
						item.IsSelected = false;
				}
			}
		}
		catch(Exception ex)
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

	private void ConfirmLocationTransactionAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (LocationTransactions.Count > 0)
			{
				foreach (var item in LocationTransactions.Where(x=>x.OutputQuantity <= 0))
				{
					SelectedItem.Details.Remove(SelectedItem.Details.FirstOrDefault(x => x.LocationReferenceId == item.LocationReferenceId));
				}
				SelectedLocationTransactions.Clear();
                foreach (var x in LocationTransactions.Where(x => x.OutputQuantity > 0))
                {
					SelectedLocationTransactions.Add(x);
                }

                foreach (var item in SelectedLocationTransactions)
				{
					var selectedLocationTransactionItem = SelectedItem.Details.FirstOrDefault(x => x.LocationReferenceId == item.LocationReferenceId);
					if (selectedLocationTransactionItem is not null)
					{
						selectedLocationTransactionItem.Quantity = item.OutputQuantity;
					}
					else
					{
                        SelectedItem.Details.Add(new OutputProductBasketDetailModel
                        {
                            ReferenceId = item.ReferenceId,
                            LocationReferenceId = item.LocationReferenceId,
                            LocationCode = item.LocationCode,
                            LocationName = item.LocationName,
                            //TransactionReferenceId = item.TransactionReferenceId,
                            //InSerilotTransactionReferenceId = item.InSerilotTransactionReferenceId,
                            //TransactionFicheReferenceId = item.TransactionFicheReferenceId,
                            //InTransactionReferenceId = item.InTransactionReferenceId,
                            Quantity = item.OutputQuantity,
                            RemainingQuantity = item.RemainingQuantity,
                        });
                    }


					
				}

				var totalOutputQuantity = LocationTransactions.Where(x => x.OutputQuantity > 0).Sum(x => (double)x.OutputQuantity);
				SelectedItem.Quantity = totalOutputQuantity;


				CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
			}
			else
			{
				CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
			}
		}
		catch (Exception ex)
		{
			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
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


	private async Task LoadSeriLotTransactionsAsync()
	{

		try
		{
			_userDialogs.ShowLoading("Load Serilot Items...");
			await Task.Delay(1000);
			SeriLotTransactions.Clear();
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _serilotTransactionService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, productReferenceId: SelectedItem.ItemReferenceId, warehouseNumber: WarehouseModel.Number, search: string.Empty, externalDb: _httpClientService.ExternalDatabase);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					SeriLotTransactions.Add(Mapping.Mapper.Map<SeriLotTransactionModel>(item));
				}
			}

			_userDialogs.Loading().Hide();
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

	private async Task LoadMoreSeriLotTransactionsAsync()
	{
		if (IsBusy)
			return;
		if (SeriLotTransactions.Count < (18))  // 18 = Take (20) - Remaining ItemsThreshold (2)
			return;
		try
		{
			IsBusy = true;

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _serilotTransactionService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, productReferenceId: SelectedItem.ItemReferenceId, warehouseNumber: WarehouseModel.Number, skip: SeriLotTransactions.Count, take: 20, externalDb: _httpClientService.ExternalDatabase);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					SeriLotTransactions.Add(Mapping.Mapper.Map<SeriLotTransactionModel>(item));
				}
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

	private void SeriLotTransactionIncreaseAsync(SeriLotTransactionModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (item is not null)
			{
				SelectedSeriLotTransaction = item;
				if (item.OutputQuantity < item.Quantity)
				{
					item.OutputQuantity++;

					if (item.OutputQuantity > 0 && !item.IsSelected)
						item.IsSelected = true;
				}
			}
		}
		catch(Exception ex)
		{
			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private void SeriLotTransactionDecreaseAsync(SeriLotTransactionModel item)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (item is not null)
			{
				SelectedSeriLotTransaction = item;
				if(item.OutputQuantity == 0)
					item.IsSelected = false;
				if (item.OutputQuantity > 0)
				{
					item.OutputQuantity--;
				}
			}
		}
		catch (Exception ex)
		{
			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private void ConfirmSeriLotTransactionAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (SeriLotTransactions.Count > 0)
			{
				SelectedSeriLotTransactions.Clear();
                foreach (var item in SeriLotTransactions.Where(x => x.OutputQuantity > 0))
                {
					SelectedSeriLotTransactions.Add(item);
				}

				// Stok yeri takipli değilse
				if (SelectedItem.LocTracking == 0) {

					foreach(var item in SelectedSeriLotTransactions)
					{
						SelectedItem.Details.Add(new OutputProductBasketDetailModel
						{
							SeriLotReferenceId = item.SerilotReferenceId,
							SeriLotCode = item.SerilotCode,
							SeriLotName = item.SerilotName,
							//ReferenceId = item.ReferenceId,
							TransactionFicheReferenceId = item.TransactionFicheReferenceId,
							TransactionReferenceId = item.TransactionReferenceId,
							InTransactionReferenceId = item.InTransactionReferenceId,
							Quantity = item.OutputQuantity,
							InSerilotTransactionReferenceId = item.InSerilotTransactionReferenceId,
							RemainingQuantity = item.OutputQuantity
						});
					}


					var totalOutputQuantity = SeriLotTransactions.Where(x => x.OutputQuantity > 0).Sum(x => (double)x.OutputQuantity);
					SelectedItem.Quantity = totalOutputQuantity;

					CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.Hidden;
				} 
				// Stok yeri takipli ise
				else
				{
					var totalOutputQuantity = SeriLotTransactions.Where(x => x.OutputQuantity > 0).Sum(x => (double)x.OutputQuantity);
					SelectedLocationTransaction.OutputQuantity = totalOutputQuantity;
					CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.Hidden;
				}
			}
			else
			{
				CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.Hidden;
			}
		}
		catch (Exception ex)
		{
			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}

	private async Task SeriLotTransactionCloseAsync()
	{
		await MainThread.InvokeOnMainThreadAsync(() =>
		{
			CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.Hidden;
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

			bool isQuantityValid = Items.All(x => x.Quantity > 0);
			if (!isQuantityValid)
			{
				await _userDialogs.AlertAsync("Sepetinizde miktarı 0 olan ürünler bulunmaktadır.", "Uyarı", "Tamam");
				return;
			}

			SelectedItems.Clear();
			foreach (var item in Items)
			{
				SelectedItems.Add(item);
			}

            await Shell.Current.GoToAsync($"{nameof(OutputProductProcessFormView)}", new Dictionary<string, object>
			{
				[nameof(WarehouseModel)] = WarehouseModel,
				[nameof(OutputProductProcessType)] = OutputProductProcessType,
				[nameof(Items)] = SelectedItems
			});

            CurrentPage.FindByName<Entry>("barcodeEntry").Text = string.Empty;

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

				SelectedLocationTransactions.Clear();
				SelectedSeriLotTransactions.Clear();
				Items.Clear();

				var productListViewModel = _serviceProvider.GetRequiredService<OutputProductProcessProductListViewModel>();

				foreach (var item in productListViewModel.Items)
					item.IsSelected = false;
				

				productListViewModel.Items.Clear();
				productListViewModel.SelectedProducts.Clear();

                await Shell.Current.GoToAsync("..");
			}
			else
			{
				await Shell.Current.GoToAsync("..");
			}

            CurrentPage.FindByName<Entry>("barcodeEntry").Text = string.Empty;

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

	private async Task CameraTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			CameraScanModel cameraScanModel = new CameraScanModel
			{
				ComingPage = "OutputProductProcessBasketListViewModel",
				WarehouseNumber = WarehouseModel.Number,
				CurrentReferenceId = 0,
				ShipInfoReferenceId = 0,
			};

			await Shell.Current.GoToAsync($"{nameof(CameraReaderView)}", new Dictionary<string, object>
			{
				[nameof(CameraScanModel)] = cameraScanModel
			});
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
}
