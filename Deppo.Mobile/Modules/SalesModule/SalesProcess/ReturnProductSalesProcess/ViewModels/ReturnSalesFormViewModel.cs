using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.MVVMHelper;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using System.Collections.ObjectModel;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.SalesModels;
using DevExpress.Maui.Controls;
using System.Reflection;
using System.ComponentModel;
using Deppo.Core.DTOs.PurchaseDispatchTransaction;
using Deppo.Mobile.Modules.ResultModule.Views;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Mobile.Core.Models.ShipAddressModels;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Core.Services;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(Items), queryId: nameof(Items))]
//ToDo
public partial class ReturnSalesFormViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IPurchaseDispatchTransactionService _purchaseDispatchTransactionService;
    private readonly ISalesCustomerService _salesCustomerService;
    private readonly IShipAddressService _shipAddressService;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    public ObservableCollection<SalesCustomer> Customers { get; } = new();
    public ObservableCollection<ShipAddressModel> ShipAddresses { get; } = new();

    [ObservableProperty]
    SalesCustomer? selectedCustomer;
    [ObservableProperty]
    ShipAddressModel? selectedShipAddress;

    [ObservableProperty]
    private DateTime ficheDate = DateTime.Now;

    [ObservableProperty]
    private string documentNumber = string.Empty;

    [ObservableProperty]
    private string documentTrackingNumber = string.Empty;

    [ObservableProperty]
    private string specialCode = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private ObservableCollection<ReturnSalesBasketModel> items = null!;

    public ReturnSalesFormViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IPurchaseDispatchTransactionService purchaseDispatchTransactionService, ISalesCustomerService salesCustomerService, IShipAddressService shipAddressService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _purchaseDispatchTransactionService = purchaseDispatchTransactionService;
        _salesCustomerService = salesCustomerService;
        _shipAddressService = shipAddressService;
        Items = new();
        Title = "Satış İade Form İşlemi";

        LoadPageCommand = new Command(async () => await LoadPageAsync());
        ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
        LoadCustomersCommand = new Command(async () => await LoadCustomersAsync());
        LoadShipAddressesCommand = new Command<SalesCustomer>(async (x) => await LoadShipAddressesAsync(x));
        SaveCommand = new Command(async () => await SaveAsync());
    }

    public Page CurrentPage { get; set; }
    public Command LoadPageCommand { get; }
    public Command BackCommand { get; }
    public Command SaveCommand { get; }
    public Command ShowBasketItemCommand { get; }
    public Command LoadCustomersCommand { get; }
    public Command<SalesCustomer> LoadShipAddressesCommand { get; }

    private async Task ShowBasketItemAsync()
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

    private async Task LoadPageAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            Console.WriteLine(Items);

            CurrentPage.FindByName<BottomSheet>("basketItemBottomSheet").State = BottomSheetState.HalfExpanded;

            Console.WriteLine(Items);
        }
        catch (Exception ex)
        {
            _userDialogs.AlertAsync(ex.Message, "hata");
        }
        finally
        {
            IsBusy = false;
        }
    }

  

    private async Task SaveAsync()
    {/*
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("İşlem Tamamlanıyor...");
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();

           var purchaseDispatchDto = new PurchaseDispatchTransactionInsert
            {
                SpeCode = SpecialCode,
                CurrentCode = selectedCustomer.Code,
                Code = string.Empty,
                DocTrackingNumber = DocumentTrackingNumber,
                DoCode = DocumentNumber,
                TransactionDate = FicheDate,
                FirmNumber = _httpClientService.FirmNumber,
                WarehouseNumber = WarehouseModel.Number,
                Description = Description
            };

           foreach (var item in Items)
            {
                var purchaseDispatchTransactionLineDto = new PurchaseDispatchTransactionLineDto
                {
                    ProductCode = item.ItemCode,
                    WarehouseNumber = (short)WarehouseModel.Number,
                    Quantity = item.Quantity,
                    ConversionFactor = 1,
                    OtherConversionFactor = 1,
                    SubUnitsetCode = item.SubUnitsetCode,
                };

                foreach (var detail in item.Details)
                {
                    var seriLotTransactionDto = new SeriLotTransactionDto
                    {
                        StockLocationCode = detail.LocationCode,
                        SubUnitsetCode = item.SubUnitsetCode,
                        Quantity = detail.Quantity,
                        ConversionFactor = 1,
                        OtherConversionFactor = 1,
                        DestinationStockLocationCode = string.Empty,
                    };
                    purchaseDispatchTransactionLineDto.SeriLotTransactions.Add(seriLotTransactionDto);
                }

                purchaseDispatchDto.Lines.Add(purchaseDispatchTransactionLineDto);
            }

            var result = await _purchaseDispatchTransactionService.InsertPurchaseDispatchTransaction(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, purchaseDispatchDto);

            Console.WriteLine(result);
            ResultModel resultModel = new();
            if (result.IsSuccess)
            {
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
                resultModel.PageCountToBack = 4;
                await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
                {
                    [nameof(ResultModel)] = resultModel
                });
            }
        }
        catch (Exception ex)
        {
            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
        }*/
    }
    private async Task LoadShipAddressesAsync(SalesCustomer salesCustomer)
    {
        if (salesCustomer is null)
        {
            await _userDialogs.AlertAsync("Lütfen müşteri seçiniz.", "Uyarı", "Tamam");
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
                currentReferenceId: salesCustomer.ReferenceId,
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
                    Customers.Add(Mapping.Mapper.Map<ShipAddressModel>(item));
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
    private async Task LoadCustomersAsync()
    {
        if (IsBusy)
            return;
        try
        {
            Customers.Clear();
            SelectedCustomer = null;

            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var result = await _salesCustomerService.GetObjectsAsync(
                    httpClient: httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    warehouseNumber: WarehouseModel.Number,
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
                    Customers.Add(Mapping.Mapper.Map<SalesCustomer>(item));
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

}