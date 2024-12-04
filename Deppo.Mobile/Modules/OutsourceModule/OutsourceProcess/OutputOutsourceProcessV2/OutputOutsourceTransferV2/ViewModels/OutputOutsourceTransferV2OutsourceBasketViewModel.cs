using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.OutsourceModels;
using Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;
using Deppo.Mobile.Core.Models.SeriLotModels;
using Deppo.Mobile.Core.Models.ShipAddressModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransferV2.Views;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcessV2.OutputOutsourceTransferV2.Views;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcessV2.OutputOutsourceTransferV2.ViewModels
{
    [QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
    [QueryProperty(name: nameof(OutsourceModel), queryId: nameof(OutsourceModel))]
    [QueryProperty(name: nameof(ShipAddressModel), queryId: nameof(ShipAddressModel))]
    [QueryProperty(name: nameof(OutputOutsourceTransferV2ProductModel), queryId: nameof(OutputOutsourceTransferV2ProductModel))]

    public partial class OutputOutsourceTransferV2OutsourceBasketViewModel :BaseViewModel    
    {

        private readonly IHttpClientService _httpClientService;
        private readonly IUserDialogs _userDialogs;
        private readonly ILocationTransactionService _locationTransactionService;
        private readonly ISeriLotService _seriLotService;

        [ObservableProperty]
        WarehouseModel? warehouseModel;

        [ObservableProperty]
        OutsourceModel? outsourceModel;

        [ObservableProperty]
        ShipAddressModel? shipAddressModel;

        [ObservableProperty]
        OutputOutsourceTransferV2ProductModel? outputOutsourceTransferV2ProductModel;

        [ObservableProperty]
        GroupLocationTransactionModel? selectedLocationTransaction;

        [ObservableProperty]
        OutputOutsourceTransferV2SubProductModel outputOutsourceTransferV2SubProductModel;

        [ObservableProperty]
        public ObservableCollection<GroupLocationTransactionModel> selectedLocationTransactions = new();
        public ObservableCollection<GroupLocationTransactionModel> LocationTransactions { get; } = new();
        [ObservableProperty]
        public ObservableCollection<SeriLotTransactionModel> selectedSeriLotTransactions = new();
        public ObservableCollection<SeriLotTransactionModel> SeriLotTransactions { get; } = new();

        public ObservableCollection<OutputOutsourceTransferV2SubProductModel> SubProducts { get; } = new();

        



        public OutputOutsourceTransferV2OutsourceBasketViewModel(IHttpClientService httpClientService,IUserDialogs userDialogs, ILocationTransactionService locationTransactionService, ISeriLotService _seriLotService)
        {
            _httpClientService = httpClientService;
            _userDialogs = userDialogs;
            _locationTransactionService = locationTransactionService;

            Title = "Ürün - Reçete";


                 //IncreaseCommand = new Command<OutputOutsourceTransferBasketModel>(async (item) => await IncreaseAsync(item));
		        //DecreaseCommand = new Command<OutputOutsourceTransferBasketModel>(async (item) => await DecreaseAsync(item));
                NextViewCommand = new Command(async () => await NextViewAsync());
                BackCommand = new Command(async () => await BackAsync());


            SubProducts = new ObservableCollection<OutputOutsourceTransferV2SubProductModel>
    {
        new OutputOutsourceTransferV2SubProductModel { ProductCode = "Code1", ProductName = "Name1" },
        new OutputOutsourceTransferV2SubProductModel { ProductCode = "Code2", ProductName = "Name2"},
        
    };

        }

        public Page CurrentPage { get; set; } = null!;
        public Command<OutputOutsourceTransferV2ProductModel> IncreaseCommand { get; }
        public Command<OutputOutsourceTransferV2ProductModel> DecreaseCommand { get; }

        public Command NextViewCommand { get; }
        public Command BackCommand { get; }


        private async Task LoadWarehouseLocationTransactionsAsync(OutputOutsourceTransferBasketModel outputOutsourceTransferBasketModel)
        {
            //try
            //{
            //    _userDialogs.ShowLoading("Load Location Transactions...");
            //    await Task.Delay(1000);

            //    LocationTransactions.Clear();

            //    var httpClient = _httpClientService.GetOrCreateHttpClient();
            //    var result = await _locationTransactionService.GetLocationTransactionsInputObjectsAsync(
            //        httpClient: httpClient,
            //        firmNumber: _httpClientService.FirmNumber,
            //        periodNumber: _httpClientService.PeriodNumber,
            //        productReferenceId: SelectedItem.IsVariant ? SelectedItem.MainItemReferenceId : SelectedItem.ItemReferenceId,
            //        variantReferenceId: SelectedItem.IsVariant ? SelectedItem.ItemReferenceId : 0,
            //        warehouseNumber: WarehouseModel.Number,
            //        skip: 0,
            //        take: 20,
            //        search: "");
            //    if (result.IsSuccess)
            //    {
            //        if (result.Data is null)
            //            return;

            //        foreach (var item in result.Data)
            //            LocationTransactions.Add(Mapping.Mapper.Map<GroupLocationTransactionModel>(item));

            //        foreach (var locationTransaction in LocationTransactions)
            //        {
            //            var matchingItem = SelectedItem.Details.FirstOrDefault(x => x.LocationReferenceId == locationTransaction.LocationReferenceId);
            //            if (matchingItem is not null)
            //            {
            //                locationTransaction.OutputQuantity = matchingItem.Quantity;
            //            }
            //        }
            //    }

            //    if (_userDialogs.IsHudShowing)
            //        _userDialogs.HideHud();
            //}
            //catch (Exception ex)
            //{
            //    if (_userDialogs.IsHudShowing)
            //        _userDialogs.HideHud();

            //    await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
            //}
        }
        private async Task LoadMoreWarehouseLocationTransactionsAsync()
        {
            //if (IsBusy)
            //    return;
            //try
            //{
            //    IsBusy = true;

            //    _userDialogs.ShowLoading("Load More Location Transactions...");

            //    var httpClient = _httpClientService.GetOrCreateHttpClient();
            //    var result = await _locationTransactionService.GetLocationTransactionsInputObjectsAsync(httpClient: httpClient,
            //        firmNumber: _httpClientService.FirmNumber,
            //        periodNumber: _httpClientService.PeriodNumber,
            //        productReferenceId: SelectedItem.IsVariant ? SelectedItem.MainItemReferenceId : SelectedItem.ItemReferenceId,
            //        variantReferenceId: SelectedItem.IsVariant ? SelectedItem.ItemReferenceId : 0,
            //        warehouseNumber: WarehouseModel.Number,
            //        skip: LocationTransactions.Count,
            //        take: 20);

            //    if (result.IsSuccess)
            //    {
            //        if (result.Data is null)
            //            return;

            //        foreach (var item in result.Data)
            //        {
            //            LocationTransactions.Add(Mapping.Mapper.Map<GroupLocationTransactionModel>(item));
            //        }

            //        foreach (var locationTransaction in LocationTransactions)
            //        {
            //            var matchingItem = SelectedItem.Details.FirstOrDefault(x => x.LocationReferenceId == locationTransaction.LocationReferenceId);
            //            if (matchingItem is not null)
            //            {
            //                locationTransaction.OutputQuantity = matchingItem.Quantity;
            //            }
            //        }
            //    }

            //    if (_userDialogs.IsHudShowing)
            //        _userDialogs.HideHud();
            //}
            //catch (Exception ex)
            //{
            //    if (_userDialogs.IsHudShowing)
            //        _userDialogs.HideHud();

            //    await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
            //}
            //finally
            //{
            //    IsBusy = false;
            //}
        }
        private async Task LoadSeriLotTransactionsAsync()
        {
            //try
            //{
            //    _userDialogs.ShowLoading("Load Serilot Items...");
            //    await Task.Delay(1000);
            //    SeriLotTransactions.Clear();
            //    var httpClient = _httpClientService.GetOrCreateHttpClient();
            //    var result = await _seriLotTransactionService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, productReferenceId: SelectedItem.ItemReferenceId, warehouseNumber: WarehouseModel.Number, search: string.Empty);

            //    if (result.IsSuccess)
            //    {
            //        if (result.Data is null)
            //            return;

            //        foreach (var item in result.Data)
            //        {
            //            SeriLotTransactions.Add(Mapping.Mapper.Map<SeriLotTransactionModel>(item));
            //        }
            //    }

            //    _userDialogs.Loading().Hide();
            //}
            //catch (Exception ex)
            //{
            //    if (_userDialogs.IsHudShowing)
            //        _userDialogs.HideHud();

            //    _userDialogs.Alert(ex.Message, "Hata", "Tamam");
            //}
            //finally
            //{
            //    IsBusy = false;
            //}
        }
        private async Task LoadMoreSeriLotTransactionsAsync()
        {
            //if (IsBusy)
            //    return;
            //if (SeriLotTransactions.Count < 18)
            //    return;

            //try
            //{
            //    IsBusy = true;

            //    var httpClient = _httpClientService.GetOrCreateHttpClient();
            //    var result = await _seriLotTransactionService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, productReferenceId: SelectedItem.ItemReferenceId, warehouseNumber: WarehouseModel.Number, skip: SeriLotTransactions.Count, take: 20);

            //    if (result.IsSuccess)
            //    {
            //        if (result.Data is null)
            //            return;

            //        foreach (var item in result.Data)
            //        {
            //            SeriLotTransactions.Add(Mapping.Mapper.Map<SeriLotTransactionModel>(item));
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    if (_userDialogs.IsHudShowing)
            //        _userDialogs.HideHud();

            //    _userDialogs.Alert(ex.Message, "Hata", "Tamam");
            //}
            //finally
            //{
            //    IsBusy = false;
            //}
        }


        private async Task IncreaseAsync(OutputOutsourceTransferBasketModel outputOutsourceTransferBasketModel)
        {
            if (outputOutsourceTransferBasketModel is null)
                return;
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

               // SelectedItem = outputOutsourceTransferBasketModel;

                if (outputOutsourceTransferBasketModel.LocTracking == 1 && outputOutsourceTransferBasketModel.TrackingType == 0)
                {
                    // Sadece Stok Yeri Takipli olma durumu
                    await LoadWarehouseLocationTransactionsAsync(outputOutsourceTransferBasketModel);
                    CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.FullExpanded;

                }
                else if (outputOutsourceTransferBasketModel.LocTracking == 1 && outputOutsourceTransferBasketModel.TrackingType == 1)
                {
                    // Stok Yeri ve Lot takipli olma durumu
                    await LoadSeriLotTransactionsAsync();
                    CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.FullExpanded;
                }
                else if (outputOutsourceTransferBasketModel.LocTracking == 1 && outputOutsourceTransferBasketModel.TrackingType == 2)
                {
                    //todo: Stok Yeri ve Seri takipli olma durumu
                }
                else if (outputOutsourceTransferBasketModel.LocTracking == 0 && outputOutsourceTransferBasketModel.TrackingType == 1)
                {
                    //todo: Sadece Lot takipli olma durumu
                }
                else if (outputOutsourceTransferBasketModel.LocTracking == 0 && outputOutsourceTransferBasketModel.TrackingType == 2)
                {
                    //todo:Sadece Seri takipli olma durumu
                }
                else
                {
                    if (outputOutsourceTransferBasketModel.Quantity < outputOutsourceTransferBasketModel.StockQuantity)
                        outputOutsourceTransferBasketModel.Quantity += 1;
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

        private async Task DecreaseAsync(OutputOutsourceTransferBasketModel outputOutsourceTransferBasketModel)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                if (outputOutsourceTransferBasketModel is not null)
                {
                    //SelectedItem = outputOutsourceTransferBasketModel;
                    if (outputOutsourceTransferBasketModel.Quantity > 1)
                    {
                        // Stok Yeri Takipli olma durumu
                        if (outputOutsourceTransferBasketModel.LocTracking == 1)
                        {
                            await LoadWarehouseLocationTransactionsAsync(outputOutsourceTransferBasketModel);
                            CurrentPage.FindByName<BottomSheet>("locationTransactionBottomSheet").State = BottomSheetState.FullExpanded;
                        }
                        // SeriLot takipli olma durumu
                        else if (outputOutsourceTransferBasketModel.LocTracking == 0 && (outputOutsourceTransferBasketModel.TrackingType == 1 || outputOutsourceTransferBasketModel.TrackingType == 2))
                        {
                            await LoadSeriLotTransactionsAsync();
                            CurrentPage.FindByName<BottomSheet>("serilotTransactionBottomSheet").State = BottomSheetState.FullExpanded;
                        }
                        else
                        {
                            outputOutsourceTransferBasketModel.Quantity -= 1;
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




        private async Task NextViewAsync()
        {
            if (IsBusy)
                return;
            
           

            if (OutputOutsourceTransferV2ProductModel is null && WarehouseModel is null && OutsourceModel is null)
                return;
            try
            {
                IsBusy = true;

                await Shell.Current.GoToAsync($"{nameof(OutputOutsourceTransferV2OutsourceFormView)}", new Dictionary<string, object>
                {
                    [nameof(WarehouseModel)] = WarehouseModel,
                    [nameof(OutsourceModel)] = OutsourceModel,
                    [nameof(ShipAddressModel)]= ShipAddressModel,
                    [nameof(InputOutsourceTransferProductModel)] = OutputOutsourceTransferV2ProductModel
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


                if (OutputOutsourceTransferV2ProductModel is not null)
                {
                    OutputOutsourceTransferV2ProductModel.IsSelected = false;
                    OutputOutsourceTransferV2ProductModel = null;
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



    }
}
