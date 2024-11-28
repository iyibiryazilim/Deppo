using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.SeriLotModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseProcess.Views;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.ViewModels;

[QueryProperty(name: nameof(Items), queryId: nameof(Items))]
[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(PurchaseSupplier), queryId: nameof(PurchaseSupplier))]
[QueryProperty(name: nameof(SelectedPurchaseTransactions), queryId: nameof(SelectedPurchaseTransactions))]
[QueryProperty(name: nameof(PurchaseFicheModel), queryId: nameof(PurchaseFicheModel))]
public partial class ReturnPurchaseDispatchBasketViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ISeriLotTransactionService _serilotTransactionService;
    private readonly ILocationTransactionService _locationTransactionService;
    private readonly IUserDialogs _userDialogs;
    private readonly ISubUnitsetService _subUnitsetService;

    [ObservableProperty]
    WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    PurchaseSupplier purchaseSupplier = null!;

    [ObservableProperty]
    PurchaseFicheModel purchaseFicheModel = null!;

    [ObservableProperty]
    public ObservableCollection<ReturnPurchaseBasketModel> items;

    [ObservableProperty]
    public ObservableCollection<PurchaseTransactionModel> selectedPurchaseTransactions;

    [ObservableProperty]
    ReturnPurchaseBasketModel? selectedItem;

    [ObservableProperty]
    SeriLotTransactionModel? selectedSeriLotTransaction;

    [ObservableProperty]
    GroupLocationTransactionModel? selectedLocationTransaction;

    [ObservableProperty]
    public ObservableCollection<SeriLotTransactionModel> selectedSeriLotTransactions = new();

    [ObservableProperty]
    public ObservableCollection<GroupLocationTransactionModel> selectedLocationTransactions = new();

    #region Collections
    public ObservableCollection<SeriLotTransactionModel> SeriLotTransactions { get; } = new();
    public ObservableCollection<GroupLocationTransactionModel> LocationTransactions { get; } = new();
	#endregion

	public ReturnPurchaseDispatchBasketViewModel(IHttpClientService httpClientService, ISeriLotTransactionService serilotTransactionService, ILocationTransactionService locationTransactionService, IUserDialogs userDialogs, ISubUnitsetService subUnitsetService)
	{
		_httpClientService = httpClientService;
		_serilotTransactionService = serilotTransactionService;
		_locationTransactionService = locationTransactionService;
		_userDialogs = userDialogs;
		_subUnitsetService = subUnitsetService;

		Title = "Sepet Listesi";

        UnitActionTappedCommand = new Command<ReturnPurchaseBasketModel>(async (item) => await UnitActionTappedAsync(item));
        SubUnitsetTappedCommand = new Command<SubUnitset>(async (item) => await SubUnitsetTappedAsync(item));

        QuantityTappedCommand = new Command<ReturnPurchaseBasketModel>(async (item) => await QuantityTappedAsync(item));
		IncreaseCommand = new Command<ReturnPurchaseBasketModel>(async (item) => await IncreaseAsync(item));
		DecreaseCommand = new Command<ReturnPurchaseBasketModel>(async (item) => await DecreaseAsync(item));
		DeleteItemCommand = new Command<ReturnPurchaseBasketModel>(async (item) => await DeleteItemAsync(item));
		NextViewCommand = new Command(async () => await NextViewAsync());
		BackCommand = new Command(async () => await BackAsync());


		LoadMoreSeriLotTransactionsCommand = new Command(async () => await LoadMoreSeriLotTransactionsAsync());
		SeriLotTransactionIncreaseCommand = new Command<SeriLotTransactionModel>(item => SeriLotTransactionIncreaseAsync(item));
		SeriLotTransactionDecreaseCommand = new Command<SeriLotTransactionModel>(item => SeriLotTransactionDecreaseAsync(item));
		ConfirmSeriLotTransactionCommand = new Command(ConfirmSeriLotTransactionAsync);
		SeriLotTransactionCloseCommand = new Command(async () => await SeriLotTransactionCloseAsync());

		LoadMoreLocationTransactionsCommand = new Command(async () => await LoadMoreLocationTransactionsAsync());
        LocationTransactionQuantityTappedCommand = new Command<GroupLocationTransactionModel>(async (item) => await LocationTransactionQuantityTappedAsync(item));
		LocationTransactionIncreaseCommand = new Command<GroupLocationTransactionModel>(async (item) => await LocationTransactionIncreaseAsync(item));
		LocationTransactionDecreaseCommand = new Command<GroupLocationTransactionModel>(async (item) => await LocationTransactionDecreaseAsync(item));
		ConfirmLocationTransactionCommand = new Command(ConfirmLocationTransactionAsync);
		LocationTransactionCloseCommand = new Command(async () => await LocationTransactionCloseAsync());
	}

	#region Properties
	public ContentPage CurrentPage { get; set; } = null!;

    #endregion

    #region Commands
    public Command UnitActionTappedCommand { get; }
    public Command SubUnitsetTappedCommand { get; }

    public Command QuantityTappedCommand { get; }
	public Command IncreaseCommand { get; }
    public Command DecreaseCommand { get; }
    public Command DeleteItemCommand { get; }
    public Command NextViewCommand { get; }
    public Command BackCommand { get; }


    #region Location Transaction Command
    public Command LoadMoreLocationTransactionsCommand { get; }
    public Command LocationTransactionQuantityTappedCommand { get; }
    public Command LocationTransactionIncreaseCommand { get; }
    public Command LocationTransactionDecreaseCommand { get; }
    public Command ConfirmLocationTransactionCommand { get; }
    public Command LocationTransactionCloseCommand { get; }
    #endregion

    #region SeriLotTransaction Command
    public Command LoadMoreSeriLotTransactionsCommand { get; }
    public Command SeriLotTransactionIncreaseCommand { get; }
    public Command SeriLotTransactionDecreaseCommand { get; }
    public Command ConfirmSeriLotTransactionCommand { get; }
    public Command SeriLotTransactionCloseCommand { get; }
	#endregion

	#endregion

	private async Task UnitActionTappedAsync(ReturnPurchaseBasketModel item)
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

	private async Task LoadSubUnitsetsAsync(ReturnPurchaseBasketModel item)
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

	private async Task QuantityTappedAsync(ReturnPurchaseBasketModel item)
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

	private async Task IncreaseAsync(ReturnPurchaseBasketModel item)
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
                else if (item.LocTracking == 0 && (item.TrackingType == 1 || item.TrackingType == 2))
                {
                    await LoadSeriLotTransactionsAsync();
                    CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.FullExpanded;
                }
                // Stok yeri ve SeriLot takipli değilse
                else
                {
                    if (item.Quantity < item.StockQuantity)
                        item.Quantity++;
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

    private async Task DecreaseAsync(ReturnPurchaseBasketModel item)
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
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task DeleteItemAsync(ReturnPurchaseBasketModel item)
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

            item.Details.Clear();
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

    private async Task LoadLocationTransactionsAsync()
    {
        try
        {
            LocationTransactions.Clear();
            _userDialogs.ShowLoading("Load Location Items...");
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationTransactionService.GetLocationTransactionsInputObjectsAsync(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                productReferenceId: SelectedItem.ItemReferenceId,
                warehouseNumber: WarehouseModel.Number
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

            _userDialogs.Loading().Hide();
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
        if (LocationTransactions.Count < (18))  // 18 = Take (20) - Remaining ItemsThreshold (2)
            return;
        try
        {
            IsBusy = true;

            _userDialogs.Loading("Load Location Items...");
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationTransactionService.GetLocationTransactionsInputObjectsAsync(httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                productReferenceId: SelectedItem.ItemReferenceId,
                warehouseNumber: WarehouseModel.Number,
                skip: LocationTransactions.Count,
                take: 20);

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
			if (quantity > SelectedItem?.StockQuantity)
			{
				await _userDialogs.AlertAsync("Girilen miktar, stok miktarını aşmamalıdır.", "Hata", "Tamam");
				return;
			}

			var totalQuantity = LocationTransactions.Where(x => x.LocationReferenceId != item.LocationReferenceId).Sum(x => x.OutputQuantity);
			if (SelectedItem?.StockQuantity >= totalQuantity + quantity)
			{
				item.OutputQuantity = quantity;
			}
			else
			{
				await _userDialogs.AlertAsync($"Girilen miktar, Ürünün stok miktarını ({SelectedItem?.StockQuantity}) aşmamalıdır.", "Hata", "Tamam");
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
                if (SelectedItem.TrackingType == 1 || SelectedItem.TrackingType == 2)
                {
                    await LoadSeriLotTransactionsAsync();
                    CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.FullExpanded;
                }
                else
                {
                    var totalQuantity = LocationTransactions.Sum(x => x.OutputQuantity);
                    if (SelectedItem.StockQuantity > totalQuantity)
                    {
                        if (item.OutputQuantity < item.RemainingQuantity && SelectedItem.StockQuantity > item.OutputQuantity)
                            item.OutputQuantity++;
                    }

                }

                if (item.OutputQuantity > 0 && !item.IsSelected)
                    item.IsSelected = true;

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
                if (SelectedItem.TrackingType == 1 || SelectedItem.TrackingType == 2)
                {
                    await LoadSeriLotTransactionsAsync();
                    CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.FullExpanded;
                }
                // SeriLot takipli değilse
                else
                {
                    if (item.OutputQuantity > 0)
                        item.OutputQuantity--;

                    if (item.OutputQuantity == 0)
                        item.IsSelected = false;
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

    private void ConfirmLocationTransactionAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if(LocationTransactions.Count == 0)
			{
				CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
				return;
			}

            foreach (var item in LocationTransactions.Where(x => x.OutputQuantity <= 0))
            {
                if(SelectedItem.Details.Any(x => x.LocationReferenceId == item.LocationReferenceId))
                {
                    SelectedItem.Details.Remove(SelectedItem.Details.FirstOrDefault(x => x.LocationReferenceId == item.LocationReferenceId));
                }
            }

            SelectedLocationTransactions.Clear();
            foreach (var locationTransaction in LocationTransactions.Where(x => x.OutputQuantity > 0))
            {
                SelectedLocationTransactions.Add(locationTransaction);
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
                    SelectedItem.Details.Add(new ReturnPurchaseBasketDetailModel
                    {
                        //ReferenceId = item.ReferenceId,
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
            var result = await _serilotTransactionService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, productReferenceId: SelectedItem.ItemReferenceId, warehouseNumber: WarehouseModel.Number, search: string.Empty);

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
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
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
            var result = await _serilotTransactionService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, productReferenceId: SelectedItem.ItemReferenceId, warehouseNumber: WarehouseModel.Number, skip: SeriLotTransactions.Count, take: 20);

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
        catch (Exception ex)
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
                if (item.OutputQuantity == 0)
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
                SelectedSeriLotTransactions.ToList().AddRange(SeriLotTransactions.Where(x => x.OutputQuantity > 0));

                // Stok yeri takipli değilse
                if (SelectedItem.LocTracking == 0)
                {

                    foreach (var item in SelectedSeriLotTransactions)
                    {
                        SelectedItem.Details.Add(new ReturnPurchaseBasketDetailModel
                        {
                            SeriLotReferenceId = item.ReferenceId,
                            SeriLotCode = item.SerilotCode,
                            SeriLotName = item.SerilotName,
                            TransactionFicheReferenceId = item.TransactionFicheReferenceId,
                            TransactionReferenceId = item.TransactionReferenceId,
                            InTransactionReferenceId = item.InTransactionReferenceId,
                            Quantity = item.OutputQuantity,
                            InSerilotTransactionReferenceId = item.InSerilotTransactionReferenceId,
                            RemainingQuantity = item.Quantity - item.OutputQuantity
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
            if(!isQuantityValid)
            {
                await _userDialogs.AlertAsync("Sepetinizde miktarı 0 olan ürünler bulunmaktadır.", "Uyarı", "Tamam");
                return;
            }
                

            await Shell.Current.GoToAsync($"{nameof(ReturnPurchaseDispatchFormView)}", new Dictionary<string, object>
            {
                [nameof(WarehouseModel)] = WarehouseModel,
                [nameof(PurchaseSupplier)] = PurchaseSupplier,
                [nameof(PurchaseFicheModel)] = PurchaseFicheModel,
                [nameof(Items)] = Items,
                [nameof(SelectedPurchaseTransactions)] = SelectedPurchaseTransactions
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

            var result = await _userDialogs.ConfirmAsync("Sepetinizdeki ürünler silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
            if (!result)
                return;

            foreach (var item in Items)
            {
                item.IsSelected = false;
            }
            foreach (var item in SelectedPurchaseTransactions)
            {
                item.IsSelected = false;
            }
            SelectedLocationTransactions.Clear();
            SelectedSeriLotTransactions.Clear();
            SelectedPurchaseTransactions.Clear();
            Items.Clear();
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

    public async Task ClearBasketItemAsync()
    {
        try
        {
            if (Items?.Count > 0)
                Items.Clear();
        }
        catch (Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
    }
}
