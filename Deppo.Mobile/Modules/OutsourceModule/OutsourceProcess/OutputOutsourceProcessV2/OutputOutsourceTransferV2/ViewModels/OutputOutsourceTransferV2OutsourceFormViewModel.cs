using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.OutsourceModels;
using Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;
using Deppo.Mobile.Core.Models.ShipAddressModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcessV2.OutputOutsourceTransferV2.ViewModels
{
  
    [QueryProperty(name: nameof(OutputOutsourceTransferV2BasketModel), queryId: nameof(OutputOutsourceTransferV2BasketModel))]

    public partial class OutputOutsourceTransferV2OutsourceFormViewModel :BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IUserDialogs _userDialogs;
        private readonly ICarrierService _carrierService;
        private readonly IDriverService _driverService;
        private readonly IServiceProvider _serviceProvider;


        [ObservableProperty]
        OutputOutsourceTransferV2BasketModel? outputOutsourceTransferV2BasketModel;

        [ObservableProperty]
        Carrier? selectedCarrier;

        [ObservableProperty]
        Driver? selectedDriver;

        [ObservableProperty]
        DateTime transactionDate = DateTime.Now;

        [ObservableProperty]
        string documentNumber = string.Empty;

        [ObservableProperty]
        string documentTrackingNumber = string.Empty;

        [ObservableProperty]
        string specialCode = string.Empty;

        [ObservableProperty]
        string description = string.Empty;

        [ObservableProperty]
        string cargoTrackingNumber = string.Empty;


     
        public ObservableCollection<Carrier> Carriers { get; } = new();
        public ObservableCollection<Driver> Drivers { get; } = new();


        public OutputOutsourceTransferV2OutsourceFormViewModel(IHttpClientService httpClientService, 
            IUserDialogs userDialogs,
            ICarrierService carrierService,
            IDriverService driverService,
            IServiceProvider serviceProvider)
        {

            _httpClientService = httpClientService;
            _userDialogs= userDialogs;
            _carrierService = carrierService;
            _driverService = driverService;
            _serviceProvider= serviceProvider;

            Title = "Fason Sevk Transfer İşlemi";

            LoadPageCommand = new Command(async () => await LoadPageAsync());
            SaveCommand = new Command(async () => await SaveAsync());
            ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
            BackCommand = new Command(async () => await BackAsync());

            
            LoadCarriersCommand = new Command(async () => await LoadCarriersAsync());
            LoadDriversCommand = new Command(async () => await LoadDriversAsync());
           

        }


        public Page CurrentPage { get; set; } = null!;

        public Command LoadPageCommand { get; }
        public Command BackCommand { get; }
        public Command SaveCommand { get; }
        public Command ShowBasketItemCommand { get; }
        public Command LoadCarriersCommand { get; }
        public Command LoadDriversCommand { get; }


        private async Task ShowBasketItemAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                CurrentPage.FindByName<BottomSheet>("basketItemBottomSheet").State = BottomSheetState.HalfExpanded;
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

        private async Task LoadPageAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                CurrentPage.FindByName<BottomSheet>("basketItemBottomSheet").State = BottomSheetState.HalfExpanded;

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

                var result = await _userDialogs.ConfirmAsync("Form verileri silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
                if (!result)
                    return;

                await ClearFormAsync();

                await Shell.Current.GoToAsync("..");
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

     
        

        private async Task LoadCarriersAsync()
        {
            if (IsBusy)
                return;
            try
            {
                Carriers.Clear();
                SelectedCarrier = null;
                IsBusy = true;

                var httpClient = _httpClientService.GetOrCreateHttpClient();

                var result = await _carrierService.GetObjects(
                        httpClient: httpClient,
                        firmNumber: _httpClientService.FirmNumber

                );

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                    {
                        Carriers.Add(Mapping.Mapper.Map<Carrier>(item));
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
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();
            }
        }

        private async Task LoadDriversAsync()
        {
            if (IsBusy)
                return;
            try
            {
                Drivers.Clear();
                SelectedDriver = null;
                IsBusy = true;

                var httpClient = _httpClientService.GetOrCreateHttpClient();

                var result = await _driverService.GetObjects(
                        httpClient: httpClient
                );

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                    {
                        Drivers.Add(Mapping.Mapper.Map<Driver>(item));
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
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();
            }
        }
      
        private async Task SaveAsync()
        {
            //if (IsBusy)
            //    return;
            //try
            //{
            //    IsBusy = true;
            //    _userDialogs.ShowLoading("İşlem Tamamlanıyor...");
            //    await Task.Delay(1000);

            //    var httpClient = _httpClientService.GetOrCreateHttpClient();

            //    var transferTransactionInsertDto = new TransferTransactionInsert
            //    {
            //        SpeCode = SpecialCode,
            //        CurrentCode = SelectedOutsource?.Code ?? string.Empty,
            //        CarrierCode = SelectedCarrier?.Code ?? string.Empty,
            //        DriverFirstName = SelectedDriver?.Name ?? string.Empty,
            //        DriverLastName = SelectedDriver?.Surname ?? string.Empty,
            //        Plaque = SelectedDriver?.PlateNumber ?? string.Empty,
            //        ShipInfoCode = SelectedShipAddress?.Code ?? string.Empty,
            //        IdentityNumber = SelectedDriver?.IdentityNumber ?? string.Empty,
            //        IsEDispatch = (bool)SelectedOutsource?.IsEDispatch ? 1 : 0,
            //        Code = string.Empty,
            //        DocTrackingNumber = DocumentTrackingNumber,
            //        DoCode = DocumentNumber,
            //        TransactionDate = TransactionDate,
            //        FirmNumber = _httpClientService.FirmNumber,
            //        WarehouseNumber = WarehouseModel.Number,
            //        DestinationWarehouseNumber = SelectedInWarehouse.Number,
            //        Description = Description,
            //    };

            //    foreach (var item in Items)
            //    {
            //        var tempItemQuantity = item.Quantity;
            //        var transferTransactionLineDto = new TransferTransactionLineDto
            //        {
            //            ProductCode = item.IsVariant ? item.MainItemCode : item.ItemCode,
            //            VariantCode = item.IsVariant ? item.ItemCode : "",
            //            WarehouseNumber = WarehouseModel.Number,
            //            DestinationWarehouseNumber = SelectedInWarehouse.Number,
            //            Quantity = item.Quantity,
            //            ConversionFactor = 1,
            //            OtherConversionFactor = 1,
            //            SubUnitsetCode = item.SubUnitsetCode,
            //        };

            //        foreach (var detail in item.Details)
            //        {
            //            var tempDetailQuantity = detail.Quantity;
            //            await LoadLocationTransaction(item, detail);
            //            LocationTransactions.OrderBy(x => x.TransactionDate).ToList();
            //            foreach (var locationTransaction in LocationTransactions)
            //            {
            //                var tempLocationRemainingQuantity = locationTransaction.RemainingQuantity;
            //                while (tempLocationRemainingQuantity > 0 && tempItemQuantity > 0 && tempDetailQuantity > 0)
            //                {
            //                    var serilotTransactionDto = new SeriLotTransactionDto
            //                    {
            //                        StockLocationCode = detail.LocationCode,
            //                        InProductTransactionLineReferenceId = locationTransaction.TransactionReferenceId,
            //                        OutProductTransactionLineReferenceId = locationTransaction.ReferenceId,
            //                        Quantity = tempDetailQuantity > tempLocationRemainingQuantity ? tempLocationRemainingQuantity : tempDetailQuantity,
            //                        SubUnitsetCode = item.SubUnitsetCode,
            //                        DestinationStockLocationCode = SelectedInlocationModel.Code,
            //                        ConversionFactor = 1,
            //                        OtherConversionFactor = 1,
            //                    };
            //                    tempLocationRemainingQuantity -= (double)serilotTransactionDto.Quantity;
            //                    tempDetailQuantity -= (double)serilotTransactionDto.Quantity;
            //                    transferTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);
            //                    tempItemQuantity -= (double)serilotTransactionDto.Quantity;
            //                }
            //            }
            //        }

            //        transferTransactionInsertDto.Lines.Add(transferTransactionLineDto);
            //    }

            //    var result = await _transferTransactionService.InsertTransferTransaction(httpClient, transferTransactionInsertDto, _httpClientService.FirmNumber);
            //    Console.WriteLine(result);
            //    ResultModel resultModel = new();

            //    if (result.IsSuccess)
            //    {
            //        resultModel.Message = "Başarılı";
            //        resultModel.Code = result.Data.Code;
            //        resultModel.PageTitle = "Fason Çıkış Transferi";
            //        resultModel.PageCountToBack = 4;

            //        var warehouseListViewModel = _serviceProvider.GetRequiredService<OutputOutsourceTransferWarehouseListViewModel>();
            //        var productListViewModel = _serviceProvider.GetRequiredService<OutputOutsourceTransferProductListViewModel>();
            //        var basketViewModel = _serviceProvider.GetRequiredService<OutputOutsourceTransferBasketListViewModel>();

            //        await Task.WhenAll(ClearFormAsync(), warehouseListViewModel?.ClearPageAsync(), productListViewModel?.ClearPageAsync(), basketViewModel?.ClearPageAsync());


            //        if (_userDialogs.IsHudShowing)
            //            _userDialogs.HideHud();

            //        await Shell.Current.GoToAsync($"{nameof(InsertSuccessPageView)}", new Dictionary<string, object>
            //        {
            //            [nameof(ResultModel)] = resultModel
            //        });
            //    }
            //    else
            //    {

            //        if (_userDialogs.IsHudShowing)
            //            _userDialogs.HideHud();

            //        resultModel.Message = "Başarısız";
            //        resultModel.PageTitle = "Fason Çıkış Transferi";
            //        resultModel.PageCountToBack = 1;
            //        resultModel.ErrorMessage = result.Message;

            //        await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
            //        {
            //            [nameof(ResultModel)] = resultModel
            //        });
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

        private async Task ClearFormAsync()
        {
            try
            {
                TransactionDate = DateTime.Now;
                CargoTrackingNumber = string.Empty;
                DocumentNumber = string.Empty;
                SpecialCode = string.Empty;
                DocumentTrackingNumber = string.Empty;
                Description = string.Empty;
                SelectedCarrier = null;
                SelectedDriver = null;
               

            }
            catch (Exception ex)
            {

                await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
            }
        }

        




    }
}
