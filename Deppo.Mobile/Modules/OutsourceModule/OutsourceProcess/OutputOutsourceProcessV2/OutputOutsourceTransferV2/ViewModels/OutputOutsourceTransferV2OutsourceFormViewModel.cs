using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.DTOs.TransferTransaction;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.OutsourceModels;
using Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;
using Deppo.Mobile.Core.Models.ProcurementModels.ByCustomerModels;
using Deppo.Mobile.Core.Models.ShipAddressModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ResultModule;
using DevExpress.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Android.Provider.CallLog;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcessV2.OutputOutsourceTransferV2.ViewModels
{
  
    [QueryProperty(name: nameof(OutputOutsourceTransferV2BasketModel), queryId: nameof(OutputOutsourceTransferV2BasketModel))]
    [QueryProperty(name: nameof(OutputOutsourceTransferV2SubProductModel), queryId: nameof(OutputOutsourceTransferV2SubProductModel))]


    public partial class OutputOutsourceTransferV2OutsourceFormViewModel :BaseViewModel
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IUserDialogs _userDialogs;
        private readonly ICarrierService _carrierService;
        private readonly IDriverService _driverService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ITransferTransactionService _transferTransactionService;
        private readonly ILocationService _locationService;
        private readonly ILocationTransactionService _locationTransactionService;


        [ObservableProperty]
        OutputOutsourceTransferV2BasketModel? outputOutsourceTransferV2BasketModel;

        [ObservableProperty]
        OutputOutsourceTransferV2BasketModel? outputOutsourceTransferV2SubProductModel;

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


        public ObservableCollection<LocationTransactionModel> LocationTransactions { get; } = new();
        private ObservableCollection<LocationTransactionModel> DispatchLocationTransactions { get; } = new();

        public ObservableCollection<LocationModel> Locations { get; } = new();


        public OutputOutsourceTransferV2OutsourceFormViewModel(IHttpClientService httpClientService,
            IUserDialogs userDialogs,
            ICarrierService carrierService,
            IDriverService driverService,
            IServiceProvider serviceProvider,
            ITransferTransactionService transferTransactionService,
            ILocationService locationService,
            ILocationTransactionService locationTransactionService
            )
        {

            _httpClientService = httpClientService;
            _userDialogs= userDialogs;
            _carrierService = carrierService;
            _driverService = driverService;
            _serviceProvider= serviceProvider;
            _transferTransactionService= transferTransactionService;
            _locationService= locationService;
            _locationTransactionService = locationTransactionService;

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

        public async Task LoadLocationsAsync()
        {
            if (IsBusy)
                return;
            try
            {
                IsBusy = true;

                Locations.Clear();


                var httpClient = _httpClientService.GetOrCreateHttpClient();

                var result = await _locationService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferSubProducts.FirstOrDefault().InWarehouseNumber, "", 0, 9999);

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                    {
                        var obj = Mapping.Mapper.Map<LocationModel>(item);
                        Locations.Add(obj);
                    }
                }

            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task LoadLocationTransactionAsync(OutputOutsourceTransferV2SubProductModel product, LocationModel locationModel)
        {
            try
            {
                LocationTransactions.Clear();
                var httpClient = _httpClientService.GetOrCreateHttpClient();

                var result = await _locationTransactionService.GetInputObjectsAsync(
                    httpClient: httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    productReferenceId: product.ProductReferenceId,
                    warehouseNumber: product.OutWarehouseNumber,
                    locationRef: locationModel.ReferenceId,
                    skip: 0,
                    take: 99999,
                    search: string.Empty
                );

                if (result.IsSuccess)
                {
                    if (result.Data is null)
                        return;

                    foreach (var item in result.Data)
                    {
                        LocationTransactions.Add(Mapping.Mapper.Map<LocationTransactionModel>(item));
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

        private async Task SaveAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;

                var confirm = await _userDialogs.ConfirmAsync("İşlemi onaylıyor musunuz?", "Onay", "Evet", "Hayır");
                if (!confirm)
                    return;

                _userDialogs.ShowLoading("İşlem Tamamlanıyor...");
                await Task.Delay(1000);

                var httpClient = _httpClientService.GetOrCreateHttpClient();

                var transferTransactionInsertDto = new TransferTransactionInsert();

                transferTransactionInsertDto.Code = string.Empty;
                transferTransactionInsertDto.IsEDispatch = OutputOutsourceTransferV2BasketModel.OutsourceModel.IsEDispatch ? 1 : 0;
                transferTransactionInsertDto.SpeCode = SpecialCode;
                transferTransactionInsertDto.CurrentCode = OutputOutsourceTransferV2BasketModel.OutsourceModel.Code;
                transferTransactionInsertDto.DoCode = DocumentNumber;
                transferTransactionInsertDto.TransactionDate = TransactionDate.AddMinutes(-1);
                transferTransactionInsertDto.Description = Description;
                transferTransactionInsertDto.DestinationWarehouseNumber = OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferSubProducts.FirstOrDefault().OutWarehouseNumber;
                transferTransactionInsertDto.FirmNumber = _httpClientService.FirmNumber;
                transferTransactionInsertDto.ShipInfoCode = OutputOutsourceTransferV2BasketModel.OutsourceModel.ShipAddressCode ?? string.Empty;
                transferTransactionInsertDto.WarehouseNumber = OutputOutsourceTransferV2BasketModel.OutsourceWarehouseModel.Number;
                transferTransactionInsertDto.CarrierCode = SelectedCarrier?.Code ?? string.Empty;
                transferTransactionInsertDto.DriverFirstName = SelectedDriver?.Surname ?? string.Empty;
                transferTransactionInsertDto.DriverLastName = SelectedDriver?.Surname ?? string.Empty;
                transferTransactionInsertDto.Plaque = SelectedDriver?.PlateNumber ?? string.Empty;

                foreach (var item in OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferSubProducts)
                {
                    var tempProductQuantity = item.Quantity;

                    var lineDto = new TransferTransactionLineDto();
                    lineDto.ProductCode = item.ProductCode;
                    lineDto.WarehouseNumber = OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferSubProducts.FirstOrDefault().InWarehouseNumber;
                    lineDto.DestinationWarehouseNumber = OutputOutsourceTransferV2BasketModel.OutputOutsourceTransferSubProducts.FirstOrDefault().OutWarehouseNumber;
                    lineDto.Quantity = item.Quantity;
                    lineDto.ConversionFactor = lineDto.Quantity;
                    lineDto.OtherConversionFactor = lineDto.Quantity;
                    lineDto.SubUnitsetCode = item.SubUnitsetCode;

                    foreach (var location in item.Locations)
                    {
                        await LoadLocationTransactionAsync( item, location);
                        var tempLocationQuanity = location.InputQuantity;
                        var locationTransactionList = LocationTransactions.OrderBy(x => x.TransactionDate).ToList();

                        foreach (var locationTransaction in locationTransactionList)
                        {
                            var tempLocationTransactionQuantity = locationTransaction.RemainingQuantity;
                            while (tempLocationTransactionQuantity > 0 && tempProductQuantity > 0 && tempLocationQuanity > 0)
                            {
                                var serilotTransactionDto = new SeriLotTransactionDto
                                {
                                    StockLocationCode = locationTransaction.LocationCode,
                                    InProductTransactionLineReferenceId = locationTransaction.TransactionReferenceId,
                                    OutProductTransactionLineReferenceId = locationTransaction.ReferenceId,
                                    SubUnitsetCode = item.SubUnitsetCode,
                                    //DestinationStockLocationCode = item.DestinationLocationCode,
                                    ConversionFactor = tempLocationQuanity > tempLocationTransactionQuantity ? tempLocationTransactionQuantity : tempLocationQuanity,
                                    OtherConversionFactor = tempLocationQuanity > tempLocationTransactionQuantity ? tempLocationTransactionQuantity : tempLocationQuanity,
                                    Quantity = tempLocationQuanity > tempLocationTransactionQuantity ? tempLocationTransactionQuantity : tempLocationQuanity,
                                };

                                lineDto.SeriLotTransactions.Add(serilotTransactionDto);
                                tempLocationTransactionQuantity -= (double)serilotTransactionDto.Quantity;
                                tempProductQuantity -= (double)serilotTransactionDto.Quantity;
                                tempLocationQuanity -= (double)serilotTransactionDto.Quantity;
                            }
                        }

                    }

                    transferTransactionInsertDto.Lines.Add(lineDto);
                }


                var result = await _transferTransactionService.InsertTransferTransaction(httpClient, transferTransactionInsertDto, _httpClientService.FirmNumber);

                ResultModel resultModel = new();

                if (result.IsSuccess)
                {
                    resultModel.Message = "Başarılı";
                    resultModel.Code = result.Data.Code;
                    resultModel.PageTitle = "Fason Sevk İşlemi";



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
