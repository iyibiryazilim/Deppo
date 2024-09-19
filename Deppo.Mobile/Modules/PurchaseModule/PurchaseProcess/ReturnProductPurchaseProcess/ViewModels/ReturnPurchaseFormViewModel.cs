using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.PurchaseReturnDispatchTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ResultModule.Views;
using Deppo.Mobile.Modules.ResultModule;
using static Android.Util.EventLogTags;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using DevExpress.Maui.Controls;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.ShipAddressModels;
using Deppo.Core.Models;
using Deppo.Mobile.Core.Services;
using Deppo.Mobile.Helpers.MappingHelper;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(Items), queryId: nameof(Items))]

public partial class ReturnPurchaseFormViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IPurchaseReturnDispatchTransactionService _purchaseReturnDispatchTransactionService;
    private readonly ISupplierService _supplierService;
    private readonly IPurchaseSupplierService _purchaseSupplierService;
    private readonly IUserDialogs _userDialogs;
    private readonly IShipAddressService _shipAddressService;
    private readonly ICarrierService _carrierService;
    private readonly IDriverService _driverService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    OutputProductProcessType outputProductProcessType;

    [ObservableProperty]
    WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    SupplierModel? selectedSupplier;
    [ObservableProperty]
    ShipAddressModel? selectedShipAddress;
    [ObservableProperty]
    Carrier? selectedCarrier;
    [ObservableProperty]
    Driver? selectedDriver;

    public ObservableCollection<SupplierModel> Suppliers { get; } = new();
    public ObservableCollection<ShipAddressModel> ShipAddresses { get; } = new();
    public ObservableCollection<Carrier> Carriers { get; } = new();
    public ObservableCollection<Driver> Drivers { get; } = new();

    [ObservableProperty]
    ObservableCollection<ReturnPurchaseBasketModel> items = null!;


    [ObservableProperty]
    string documentNumber = string.Empty;

    [ObservableProperty]
    DateTime transactionDate = DateTime.Now;

    [ObservableProperty]
    string description = string.Empty;

    [ObservableProperty]
    string documentTrackingNumber = string.Empty;

    [ObservableProperty]
    string specialCode = string.Empty;


    public ReturnPurchaseFormViewModel(IHttpClientService httpClientService, IPurchaseReturnDispatchTransactionService purchaseReturnDispatchTransactionService, IUserDialogs userDialogs, IPurchaseSupplierService purchaseSupplierService, IShipAddressService shipAddressService, ICarrierService carrierService, IDriverService driverService, ISupplierService supplierService, IServiceProvider serviceProvider)
    {
        _httpClientService = httpClientService;
        _purchaseReturnDispatchTransactionService = purchaseReturnDispatchTransactionService;
        _userDialogs = userDialogs;
        _purchaseSupplierService = purchaseSupplierService;
        _shipAddressService = shipAddressService;
        _carrierService = carrierService;
        _driverService = driverService;
        _supplierService = supplierService;
        _serviceProvider = serviceProvider;
        Items = new();

        Title = "Satınalma İade İrsaliyesi";

        SaveCommand = new Command(async () => await SaveAsync());
        LoadPageCommand = new Command(async () => await LoadPageAsync());
        LoadSuppliersCommand = new Command(async () => await LoadSuppliersAsync());
        LoadCarriersCommand = new Command(async () => await LoadCarriersAsync());
        LoadDriversCommand = new Command(async () => await LoadDriversAsync());
        LoadShipAddressesCommand = new Command<PurchaseSupplier>(async (purchaseSupplier) => await LoadShipAddressesAsync(purchaseSupplier));
        
    }
    public Page CurrentPage { get; set; }

    public Command SaveCommand { get; }
    public Command LoadPageCommand { get; }
    public Command LoadSuppliersCommand { get; }
    public Command LoadShipAddressesCommand { get; }
    public Command LoadCarriersCommand { get; }
    public Command LoadDriversCommand { get; }


    private async Task LoadPageAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            CurrentPage.FindByName<BottomSheet>("basketItemBottomSheet").State = BottomSheetState.HalfExpanded;


        }
        catch (System.Exception)
        {

            throw;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public static string GetEnumDescription(Enum value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());

        DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

        if (attributes != null && attributes.Any())
        {
            return attributes.First().Description;
        }

        return value.ToString();
    }

    private async Task LoadSuppliersAsync()
    {
        if (IsBusy)
            return;
        try
        {
            Suppliers.Clear();
            SelectedSupplier = null;
            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var result = await _supplierService.GetObjects(
                    httpClient: httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    skip: 0,
                    take: 9999999,
                    search: ""
            );

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    Suppliers.Add(Mapping.Mapper.Map<SupplierModel>(item));
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

    private async Task LoadShipAddressesAsync(PurchaseSupplier purchaseSupplier)
    {
        if (purchaseSupplier is null)
        {
            await _userDialogs.AlertAsync("Lütfen tedarikçi seçiniz.", "Uyarı", "Tamam");
            return;
        }
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            ShipAddresses.Clear();
            SelectedShipAddress = null;

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var result = await _shipAddressService.GetObjects(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                currentReferenceId: purchaseSupplier.ReferenceId,
                skip: 0,
                take: 9999999,
                search: ""
            );

            if (result.IsSuccess)
            {
                if (result.Data is null)
                    return;

                foreach (var item in result.Data)
                {
                    ShipAddresses.Add(Mapping.Mapper.Map<ShipAddressModel>(item));
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
        }
    }

    private async Task SaveAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("İşlem Tamamlanıyor...");
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var dto = new PurchaseReturnDispatchTransactionInsert
            {
                Code = "",
                CurrentCode = SelectedSupplier.Code,
                CarrierCode = SelectedCarrier != null ? SelectedCarrier.Code : "",
                DriverFirstName = SelectedDriver != null ? SelectedDriver.Name : "",
                DriverLastName = SelectedDriver != null ? SelectedDriver.Surname : "",
                Plaque = SelectedDriver != null ? SelectedDriver.PlateNumber : "",
                IdentityNumber = SelectedDriver != null ? SelectedDriver.IdentityNumber : "",
                ShipInfoCode = SelectedShipAddress != null ? SelectedShipAddress.Code : "",
                Description = Description,
                DoCode = DocumentNumber,
                DocTrackingNumber = DocumentTrackingNumber,
                TransactionDate = TransactionDate,
                FirmNumber = _httpClientService.FirmNumber,
                SpeCode = SpecialCode,
                WarehouseNumber = WarehouseModel.Number,

            };

            foreach (var item in Items)
            {
                var consumableTransactionLineDto = new PurchaseReturnDispatchTransactionLineDto
                {
                    ProductCode = item.ItemCode,
                    WarehouseNumber = (short?)WarehouseModel.Number,
                    Quantity = item.Quantity,
                    ConversionFactor = 1,
                    OtherConversionFactor = 1,
                    SubUnitsetCode = item.SubUnitsetCode,
                };

                foreach (var detail in item.Details)
                {
                    var serilotTransactionDto = new SeriLotTransactionDto
                    {
                        StockLocationCode = detail.LocationCode,
                        InProductTransactionLineReferenceId = detail.TransactionReferenceId,
                        OutProductTransactionLineReferenceId = detail.ReferenceId,
                        Quantity = detail.RemainingQuantity,
                        SubUnitsetCode = item.SubUnitsetCode,
                        DestinationStockLocationCode = string.Empty,
                        ConversionFactor = 1,
                        OtherConversionFactor = 1,
                    };

                    consumableTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);
                }

                dto.Lines.Add(consumableTransactionLineDto);
            }
            Console.WriteLine(dto);

            var result = await _purchaseReturnDispatchTransactionService.InsertPurchaseReturnDispatchTransaction(httpClient, _httpClientService.FirmNumber, dto);

            ResultModel resultModel = new();
            if (result.IsSuccess)
            {
                var basketViewModel = _serviceProvider.GetRequiredService<ReturnPurchaseBasketViewModel>();
                basketViewModel.Items.Clear();
                basketViewModel.SelectedLocationTransactions.Clear();
                basketViewModel.SelectedSeriLotTransactions.Clear();

                resultModel.Message = "Başarılı";
                resultModel.Code = result.Data.Code;
                resultModel.PageTitle = Title;
                resultModel.PageCountToBack = 5;

                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                await Shell.Current.GoToAsync($"{nameof(InsertSuccessPageView)}", new Dictionary<string, object>
                {
                    [nameof(ResultModel)] = resultModel
                });
            }
            else
            {
                if (_userDialogs.IsHudShowing)
                    _userDialogs.HideHud();

                resultModel.Message = "Başarısız";
                resultModel.PageTitle = Title;
                resultModel.PageCountToBack = 1;
                await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
                {
                    [nameof(ResultModel)] = resultModel
                });
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
}
