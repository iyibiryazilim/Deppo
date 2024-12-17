using AutoMapper.Execution;
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
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.SeriLotModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.BarcodeHelper;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CameraModule.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.Views;
using DevExpress.Data.Async.Helpers;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(SalesCustomer), queryId: nameof(SalesCustomer))]
public partial class OutputProductSalesOrderProcessBasketListViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly ILocationTransactionService _locationTransactionService;
    private readonly ISeriLotTransactionService _seriLotTransactionService;
    private readonly IBarcodeSearchSalesHelper _barcodeSearchSalesHelper;
    private readonly IWaitingSalesOrderService _waitingSalesOrderService;
  
    private readonly ISubUnitsetService _subUnitsetService;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    private SalesCustomer salesCustomer = null!;

    [ObservableProperty]
    public Entry barcodeEntry;

    [ObservableProperty]
    public SearchBar locationTransactionSearchText;

    public ObservableCollection<OutputSalesBasketModel> Items { get; } = new();

    [ObservableProperty]
    private OutputSalesBasketModel? selectedItem;

    [ObservableProperty]
    private GroupLocationTransactionModel? selectedLocationTransaction;

    [ObservableProperty]
    private SeriLotTransactionModel? selectedSeriLotTransaction;

    [ObservableProperty]
    public ObservableCollection<GroupLocationTransactionModel> selectedLocationTransactions = new();

    [ObservableProperty]
    public ObservableCollection<SeriLotTransactionModel> selectedSeriLotTransactions = new();

    public ObservableCollection<GroupLocationTransactionModel> LocationTransactions { get; } = new();

    //Arama yapıldığında kullanılan liste
    public ObservableCollection<GroupLocationTransactionModel> SelectedLocationTransactionItems { get; } = new();

    public ObservableCollection<SeriLotTransactionModel> SeriLotTransactions { get; } = new();

	public OutputProductSalesOrderProcessBasketListViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, ILocationTransactionService locationTransactionService, ISeriLotTransactionService seriLotTransactionService, ISubUnitsetService subUnitsetService, IBarcodeSearchSalesHelper barcodeSearchSalesHelper, IWaitingSalesOrderService waitingSalesOrderService)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_locationTransactionService = locationTransactionService;
		_seriLotTransactionService = seriLotTransactionService;
		_barcodeSearchSalesHelper = barcodeSearchSalesHelper;
		_subUnitsetService = subUnitsetService;
		_waitingSalesOrderService = waitingSalesOrderService;

		Title = "Satış Sepeti";

		LoadCommand = new Command(async () => await LoadAsync());
		PerformSearchCommand = new Command<Entry>(async (x) => await PerformSearchAsync(x));
		UnitActionTappedCommand = new Command<OutputSalesBasketModel>(async (item) => await UnitActionTappedAsync(item));
		SubUnitsetTappedCommand = new Command<SubUnitset>(async (subUnitset) => await SubUnitsetTappedAsync(subUnitset));
		QuantityTappedCommand = new Command<OutputSalesBasketModel>(async (item) => await QuantityTappedAsync(item));
		IncreaseCommand = new Command<OutputSalesBasketModel>(async (outputSalesBasketModel) => await IncreaseAsync(outputSalesBasketModel));
		DecreaseCommand = new Command<OutputSalesBasketModel>(async (outputSalesBasketModel) => await DecreaseAsync(outputSalesBasketModel));
		DeleteItemCommand = new Command<OutputSalesBasketModel>(async (outputSalesBasketModel) => await DeleteItemAsync(outputSalesBasketModel));
		BackCommand = new Command(async () => await BackAsync());
		PlusTappedCommand = new Command(async () => await PlusTappedAsync());
		ProductOptionTappedCommand = new Command(async () => await ProductOptionTappedAsync());
		OrderOptionTappedCommand = new Command(async () => await OrderOptionTappedAsync());
		CameraTappedCommand = new Command(async () => await CameraTappedAsync());
		NextViewCommand = new Command(async () => await NextViewAsync());

		LoadMoreLocationTransactionsCommand = new Command(async () => await LoadMoreWarehouseLocationTransactionsAsync());
		LocationTransactionQuantityTappedCommand = new Command<GroupLocationTransactionModel>(async (item) => await LocationTransactionQuantityTappedAsync(item));
		LocationTransactionIncreaseCommand = new Command<GroupLocationTransactionModel>(async (item) => await LocationTransactionIncreaseAsync(item));
		LocationTransactionDecreaseCommand = new Command<GroupLocationTransactionModel>(async (item) => await LocationTransactionDecreaseAsync(item));
		LocationTransactionConfirmCommand = new Command(ConfirmLocationTransactionAsync);
		LocationTransactionCloseCommand = new Command(async () => await LocationTransactionCloseAsync());
		LocationTransactionPerformSearchCommand = new Command(async () => await LocationTransactionPerformSearchAsync());
		LocationTransactionPerformEmptySearchCommand = new Command(async () => await LocationTransactionPerformEmptySearchAsync());

		LoadMoreSeriLotTransactionsCommand = new Command(async () => await LoadMoreSeriLotTransactionsAsync());
		SeriLotTransactionIncreaseCommand = new Command<SeriLotTransactionModel>(async (item) => SeriLotTransactionIncreaseAsync(item));
		SeriLotTransactionDecreaseCommand = new Command<SeriLotTransactionModel>(async (item) => SeriLotTransactionDecreaseAsync(item));
		SeriLotTransactionConfirmCommand = new Command(ConfirmSeriLotTransactionAsync);
		SeriLotTransactionCloseCommand = new Command(async () => await SeriLotTransactionCloseAsync());
	}

	public ContentPage CurrentPage { get; set; } = null!;

    #region Commands

    public Command LoadCommand { get; }
    public Command PerformSearchCommand { get; }
    public Command UnitActionTappedCommand { get; }
    public Command SubUnitsetTappedCommand { get; }

    public Command QuantityTappedCommand { get; }
    public Command<OutputSalesBasketModel> IncreaseCommand { get; }
    public Command<OutputSalesBasketModel> DecreaseCommand { get; }
    public Command<OutputSalesBasketModel> DeleteItemCommand { get; }
    public Command PlusTappedCommand { get; }
    public Command ProductOptionTappedCommand { get; }
    public Command OrderOptionTappedCommand { get; }
    public Command CameraTappedCommand { get; }

    #region LocationTransaction Command

    public Command LoadMoreLocationTransactionsCommand { get; }
    public Command LocationTransactionQuantityTappedCommand { get; }
    public Command LocationTransactionIncreaseCommand { get; }
    public Command LocationTransactionDecreaseCommand { get; }
    public Command LocationTransactionConfirmCommand { get; }
    public Command LocationTransactionCloseCommand { get; }
    public Command LocationTransactionPerformSearchCommand { get; }
    public Command LocationTransactionPerformEmptySearchCommand { get; }

    #endregion LocationTransaction Command

    #region SeriLotTransaction Command

    public Command LoadMoreSeriLotTransactionsCommand { get; }
    public Command SeriLotTransactionIncreaseCommand { get; }
    public Command SeriLotTransactionDecreaseCommand { get; }
    public Command SeriLotTransactionConfirmCommand { get; }
    public Command SeriLotTransactionCloseCommand { get; }

    #endregion SeriLotTransaction Command

    public Command BackCommand { get; }
    public Command NextViewCommand { get; }

    #endregion Commands

    private async Task LoadAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            var barcodeEntry = CurrentPage.FindByName<Entry>("barcodeEntry");
            if (barcodeEntry is not null)
            {
                // barcodeEntry.Focus();
                // barcodeEntry.Focused += async (s, e) =>
                // {
                // 	await barcodeEntry.HideKeyboardAsync(CancellationToken.None);
                // 	await barcodeEntry.HideSoftInputAsync(CancellationToken.None);
                // };
            }

            //_deviceDisplay.MainDisplayInfoChanged += (s, e) =>
            //{
            //	//to do
            //};
        }
        catch (System.Exception ex)
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

            var result = await _barcodeSearchSalesHelper.BarcodeDetectedAsync(
               httpClient: httpClient,
               firmNumber: _httpClientService.FirmNumber,
               periodNumber: _httpClientService.PeriodNumber,
               barcode: barcodeEntry.Text,
               warehouseNumber: WarehouseModel.Number,
               customerReferenceId: SalesCustomer.ReferenceId,
               shipInfoReferenceId: SalesCustomer.ShipAddressReferenceId
			);

            if (result is not null)
            {
                Type resultType = result.GetType();
                if (resultType == typeof(BarcodeSalesProductModel))
                {
                    var existingItem = Items.FirstOrDefault(x => x.ItemReferenceId == result.ItemReferenceId);

                    if(existingItem is not null)
                    {
						// Orders'ları kontrol et ve eksik olanları ekle
						var existingItemFullOrders = await _waitingSalesOrderService.GetObjectsByProduct(
							httpClient: httpClient,
							firmNumber: _httpClientService.FirmNumber,
							periodNumber: _httpClientService.PeriodNumber,
							warehouseNumber: WarehouseModel.Number,
							productReferenceId: result.ItemReferenceId,
							customerReferenceId: SalesCustomer.ReferenceId,
							shipInfoReferenceId: SalesCustomer.ShipAddressReferenceId,
							skip: 0,
							take: 999999
						);

						if (existingItemFullOrders.IsSuccess && existingItemFullOrders.Data is not null)
						{
							foreach (var order in existingItemFullOrders.Data)
							{
                                var obj = Mapping.Mapper.Map<OutputSalesBasketOrderModel>(order);
								if (!existingItem.Orders.Any(x => x.ReferenceId == obj.ReferenceId))
								{
									existingItem.Orders.Add(new OutputSalesBasketOrderModel
									{
										ReferenceId = obj.ReferenceId,
										OrderReferenceId = obj.OrderReferenceId,
										CustomerReferenceId = obj.CustomerReferenceId,
										CustomerCode = obj.CustomerCode,
										CustomerName = obj.CustomerName,
										ProductReferenceId = obj.ProductReferenceId,
										ProductCode = obj.ProductCode,
										ProductName = obj.ProductName,
										UnitsetReferenceId = obj.UnitsetReferenceId,
										UnitsetCode = obj.UnitsetCode,
										UnitsetName = obj.UnitsetName,
										SubUnitsetReferenceId = obj.SubUnitsetReferenceId,
										SubUnitsetCode = obj.SubUnitsetCode,
										SubUnitsetName = obj.SubUnitsetName,
										Quantity = obj.Quantity,
										ShippedQuantity = obj.ShippedQuantity,
										WaitingQuantity = obj.WaitingQuantity,
                                        Price = obj.Price,
                                        Vat = obj.Vat,
										OrderDate = obj.OrderDate,
										DueDate = obj.DueDate
									});
								}
							}
						}
					}
                    else
                    {
						var outputSalesBasketModelItem = new OutputSalesBasketModel
						{
							ItemReferenceId = result.ItemReferenceId,
							ItemCode = result.ItemCode,
							ItemName = result.ItemName,
							UnitsetReferenceId = result.UnitsetReferenceId,
							UnitsetCode = result.UnitsetCode,
							UnitsetName = result.UnitsetName,
							SubUnitsetReferenceId = result.SubUnitsetReferenceId,
							SubUnitsetCode = result.SubUnitsetCode,
							SubUnitsetName = result.SubUnitsetName,
							MainItemReferenceId = result.ItemReferenceId,
							MainItemCode = result.ItemCode,
							MainItemName = result.ItemName,
							StockQuantity = default,
							IsSelected = false,
							IsVariant = result.IsVariant,
							TrackingType = result.TrackingType,
							LocTracking = result.LocTracking,
							Image = result.ImageData,
							Quantity = result.WaitingQuantity,
							OutputQuantity = result.LocTracking == 0 ? 1 : 0,
						};

						var itemOrders = await _waitingSalesOrderService.GetObjectsByProduct(
							httpClient: httpClient,
							firmNumber: _httpClientService.FirmNumber,
							periodNumber: _httpClientService.PeriodNumber,
							warehouseNumber: WarehouseModel.Number,
							productReferenceId: outputSalesBasketModelItem.ItemReferenceId,
							customerReferenceId: SalesCustomer.ReferenceId,
							shipInfoReferenceId: SalesCustomer.ShipAddressReferenceId,
							skip: 0,
							take: 999999
						);

                        if(itemOrders.IsSuccess)
                        {
                            if(itemOrders.Data is not null)
                            {
                                foreach (var order in itemOrders.Data)
                                {
									var obj = Mapping.Mapper.Map<OutputSalesBasketOrderModel>(order);
									outputSalesBasketModelItem.Orders.Add(new OutputSalesBasketOrderModel
									{
										ReferenceId = obj.ReferenceId,
										OrderReferenceId = obj.OrderReferenceId,
										CustomerReferenceId = obj.CustomerReferenceId,
										CustomerCode = obj.CustomerCode,
										CustomerName = obj.CustomerName,
										ProductReferenceId = obj.ProductReferenceId,
										ProductCode = obj.ProductCode,
										ProductName = obj.ProductName,
										UnitsetReferenceId = obj.UnitsetReferenceId,
										UnitsetCode = obj.UnitsetCode,
										UnitsetName = obj.UnitsetName,
										SubUnitsetReferenceId = obj.SubUnitsetReferenceId,
										SubUnitsetCode = obj.SubUnitsetCode,
										SubUnitsetName = obj.SubUnitsetName,
										Quantity = obj.Quantity,
										ShippedQuantity = obj.ShippedQuantity,
										WaitingQuantity = obj.WaitingQuantity,
										OrderDate = obj.OrderDate,
										DueDate = obj.DueDate,
									});

								}
                            }
                        }

						Items.Add(outputSalesBasketModelItem);
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

    private async Task LoadSubUnitsetsAsync(OutputSalesBasketModel item)
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

    private async Task UnitActionTappedAsync(OutputSalesBasketModel item)
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

    private async Task QuantityTappedAsync(OutputSalesBasketModel item)
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

            if (quantity > item.Quantity)
            {
                await _userDialogs.AlertAsync("Girilen miktar, ürünün stok miktarını aşmamalıdır.", "Hata", "Tamam");
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

    private async Task IncreaseAsync(OutputSalesBasketModel outputSalesBasketModel)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            SelectedItem = outputSalesBasketModel;
            // Stok Yeri Takipli olma durumu
            if (outputSalesBasketModel.LocTracking == 1)
            {
                await LoadWarehouseLocationTransactionsAsync();
                CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.FullExpanded;
            }
            else if (outputSalesBasketModel.TrackingType == 1 || outputSalesBasketModel.TrackingType == 2)
            {
                CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.FullExpanded;
            }
            else
            {
                if (outputSalesBasketModel.OutputQuantity < outputSalesBasketModel.Orders.Sum(x=>x.WaitingQuantity))
                    outputSalesBasketModel.OutputQuantity++;
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

    private async Task DecreaseAsync(OutputSalesBasketModel outputSalesBasketModel)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            SelectedItem = outputSalesBasketModel;
            if (outputSalesBasketModel is not null)
            {
                if (outputSalesBasketModel.OutputQuantity > 1)
                {
                    // Stok Yeri Takipli olma durumu
                    if (outputSalesBasketModel.LocTracking == 1)
                    {
                        await LoadWarehouseLocationTransactionsAsync();
                        CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.FullExpanded;
                    }
                    // SeriLot takipli olma durumu
                    else if (outputSalesBasketModel.LocTracking == 0 && (outputSalesBasketModel.TrackingType == 1 || outputSalesBasketModel.TrackingType == 2))
                    {
                        await LoadSeriLotTransactionsAsync();
                        CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.FullExpanded;
                    }
                    else
                    {
                        outputSalesBasketModel.OutputQuantity--;
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

    private async Task DeleteItemAsync(OutputSalesBasketModel outputSalesBasketModel)
    {
        if (outputSalesBasketModel is null)
            return;
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var confirm = await _userDialogs.ConfirmAsync($"{outputSalesBasketModel.ItemCode}\n{outputSalesBasketModel.ItemName}\nİlgili ürün sepetinizden çıkarılacaktır. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
            if (!confirm)
                return;

            outputSalesBasketModel.Details.Clear();
            outputSalesBasketModel.Orders.Clear();
            Items.Remove(outputSalesBasketModel);
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

    private async Task LoadWarehouseLocationTransactionsAsync()
    {
        try
        {
            _userDialogs.ShowLoading("Yükleniyor...");
            LocationTransactions.Clear();
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationTransactionService.GetLocationTransactionsInputObjectsAsync(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, SelectedItem.ItemReferenceId, warehouseNumber: WarehouseModel.Number, 0, 20, LocationTransactionSearchText.Text);
            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<GroupLocationTransactionModel>(item);
                    var matchingItem = SelectedItem.Details.FirstOrDefault(x => x.LocationReferenceId == obj.LocationReferenceId);
                    if (matchingItem != null)
                        obj.OutputQuantity = matchingItem.Quantity;

                    LocationTransactions.Add(Mapping.Mapper.Map<GroupLocationTransactionModel>(item));
                }
            }

            _userDialogs.HideHud();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
    }

    private async Task LoadMoreWarehouseLocationTransactionsAsync()
    {
        if (IsBusy)
            return;
        if (LocationTransactions.Count < (18))  // 18 = Take (20) - Remaining ItemsThreshold (2)
            return;
        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationTransactionService.GetLocationTransactionsInputObjectsAsync(httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                productReferenceId: SelectedItem.ItemReferenceId,
                warehouseNumber: WarehouseModel.Number,
                skip: LocationTransactions.Count,
                take: 20,
                search: LocationTransactionSearchText.Text);

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    var obj = Mapping.Mapper.Map<GroupLocationTransactionModel>(item);
                    var matchingItem = SelectedItem.Details.FirstOrDefault(x => x.LocationReferenceId == obj.LocationReferenceId);
                    if (matchingItem != null)
                        obj.OutputQuantity = matchingItem.Quantity;

                    LocationTransactions.Add(Mapping.Mapper.Map<GroupLocationTransactionModel>(item));
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
            if (quantity > SelectedItem?.Quantity)
            {
                await _userDialogs.AlertAsync("Girilen miktar, ürünün miktarını aşmamalıdır.", "Hata", "Tamam");
                return;
            }

            var totalQuantity = LocationTransactions.Where(x=>x.LocationReferenceId != item.LocationReferenceId).Sum(x => x.OutputQuantity);
            if (SelectedItem?.Quantity >= totalQuantity + quantity)
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
                SelectedLocationTransaction = item;
                if (SelectedItem.TrackingType == 1 || SelectedItem.TrackingType == 2)
                {
                    await LoadSeriLotTransactionsAsync();
                    CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.FullExpanded;
                }
                else
                {
                    var totalQuantity = LocationTransactions.Sum(x => x.OutputQuantity);
                    if (SelectedItem.Quantity > totalQuantity)
                    {
                        if (item.OutputQuantity < item.RemainingQuantity && SelectedItem.Quantity > item.OutputQuantity)
                            item.OutputQuantity++;
                    }

                    if (item.OutputQuantity > 0 && !item.IsSelected)
                        item.IsSelected = true;
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

    private async Task LocationTransactionDecreaseAsync(GroupLocationTransactionModel item)
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            if (item.OutputQuantity > 0)
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

            if (LocationTransactions.Count > 0)
            {
				foreach (var item in LocationTransactions.Where(x => x.OutputQuantity <= 0))
				{
					SelectedItem.Details.Remove(SelectedItem.Details.FirstOrDefault(x => x.LocationReferenceId == item.LocationReferenceId));
				}
				SelectedLocationTransactions.Clear();
                foreach (var item in LocationTransactions.Where(x => x.OutputQuantity > 0))
                {
                    SelectedLocationTransactions.Add(item);
                }

                foreach (var item in SelectedLocationTransactions)
                {
                    var selectedLocationTransactionItem = SelectedItem.Details.FirstOrDefault(x => x.LocationReferenceId == item.LocationReferenceId);
                    if (selectedLocationTransactionItem is not null)
                    {
                        selectedLocationTransactionItem.Quantity = item.OutputQuantity;
                        selectedLocationTransactionItem.RemainingQuantity = item.OutputQuantity;
                    }

                    SelectedItem.Details.Add(new OutputSalesBasketDetailModel
                    {
                        //ReferenceId = item.ReferenceId,
                        LocationReferenceId = item.LocationReferenceId,
                        LocationCode = item.LocationCode,
                        LocationName = item.LocationName,
                        Quantity = item.OutputQuantity,
                        //TransactionReferenceId = item.TransactionReferenceId,
                        //TransactionFicheReferenceId = item.TransactionFicheReferenceId,
                        //InTransactionReferenceId = item.InTransactionReferenceId,
                        RemainingQuantity = item.RemainingQuantity,
                    });
                }

                var totalOutputQuantity = SelectedLocationTransactions.Sum(x => (double)x.OutputQuantity);
                SelectedItem.OutputQuantity = totalOutputQuantity;

                CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
            }
            else
            {
                CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.Hidden;
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
            LocationTransactionSearchText.Text = string.Empty;
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
                await LoadWarehouseLocationTransactionsAsync();
                LocationTransactionSearchText.Unfocus();
                return;
            }
            IsBusy = true;

            LocationTransactions.Clear();

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationTransactionService.GetInputObjectsAsync(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                productReferenceId: SelectedItem.ItemReferenceId,
                warehouseNumber: WarehouseModel.Number,
                skip: 0,
                take: 20,
                search: LocationTransactionSearchText.Text
            );

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;
                foreach (var item in result.Data)
                {
                    LocationTransactions.Add(Mapping.Mapper.Map<LocationTransactionModel>(item));
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

    private async Task LocationTransactionCloseAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
			LocationTransactionSearchText.Text = string.Empty;
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
            var result = await _seriLotTransactionService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, productReferenceId: SelectedItem.ItemReferenceId, warehouseNumber: WarehouseModel.Number, search: string.Empty);

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
        if (SeriLotTransactions.Count < 18)
            return;

        try
        {
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _seriLotTransactionService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, productReferenceId: SelectedItem.ItemReferenceId, warehouseNumber: WarehouseModel.Number, skip: SeriLotTransactions.Count, take: 20);

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
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
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
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

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
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

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
                    var totalOutputQuantity = SeriLotTransactions.Where(x => x.OutputQuantity > 0).Sum(x => (double)x.OutputQuantity);
                    SelectedItem.OutputQuantity = totalOutputQuantity;

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
            CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.Hidden;
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

            bool isQuantityValid = Items.All(x => x.OutputQuantity > 0);
            if (!isQuantityValid)
            {
                await _userDialogs.AlertAsync("Sepetinizde miktarı 0 olan ürünler bulunmaktadır.", "Uyarı", "Tamam");
                return;
            }

            await Shell.Current.GoToAsync($"{nameof(OutputProductSalesOrderProcessFormView)}", new Dictionary<string, object>
            {
                [nameof(WarehouseModel)] = WarehouseModel,
                [nameof(Items)] = Items,
                [nameof(SalesCustomer)] = SalesCustomer
            });

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
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task PlusTappedAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            CurrentPage.FindByName<BottomSheet>("basketOptionsBottomSheet").State = BottomSheetState.HalfExpanded;
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

    private async Task ProductOptionTappedAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            CurrentPage.FindByName<BottomSheet>("basketOptionsBottomSheet").State = BottomSheetState.Hidden;

            await Task.Delay(300);
            await Shell.Current.GoToAsync($"{nameof(OutputProductSalesOrderProcessProductListView)}", new Dictionary<string, object>
            {
                [nameof(WarehouseModel)] = WarehouseModel,
                [nameof(SalesCustomer)] = SalesCustomer
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

    private async Task OrderOptionTappedAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            CurrentPage.FindByName<BottomSheet>("basketOptionsBottomSheet").State = BottomSheetState.Hidden;
            await Task.Delay(300);

            await Shell.Current.GoToAsync($"{nameof(OutputProductSalesOrderProcessOrderListView)}", new Dictionary<string, object>
            {
                [nameof(WarehouseModel)] = WarehouseModel,
                [nameof(SalesCustomer)] = SalesCustomer
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

    private async Task CameraTappedAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            CameraScanModel cameraScanModel = new CameraScanModel
            {
                ComingPage = "OutputProductSalesOrderProcessBasketListViewModel",
                WarehouseNumber = WarehouseModel.Number,
                CurrentReferenceId = SalesCustomer.ReferenceId,
                ShipInfoReferenceId = SalesCustomer.ShipAddressReferenceId
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