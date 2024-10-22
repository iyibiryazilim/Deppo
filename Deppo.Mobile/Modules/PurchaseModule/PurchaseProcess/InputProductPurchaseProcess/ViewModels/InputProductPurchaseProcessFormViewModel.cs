using Android.OS;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.PurchaseDispatchTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.ShipAddressModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Helpers.TransactionAuditHelpers;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;
using Deppo.Sys.Service.Services;
using DevExpress.Maui.Controls;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(Items), queryId: nameof(Items))]
[QueryProperty(name: nameof(SupplierModel), queryId: nameof(SupplierModel))]
[QueryProperty(name: nameof(ShipAddressModel), queryId: nameof(ShipAddressModel))]
public partial class InputProductPurchaseProcessFormViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IPurchaseDispatchTransactionService _purchaseDispatchTransactionService;
    private readonly ITransactionAuditService _transactionAuditService;
    private readonly IHttpClientSysService _httpClientSysService;
    private readonly ITransactionAuditHelperService _transactionAuditHelperService;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    private SupplierModel supplierModel = null!;

    [ObservableProperty]
    private ShipAddressModel shipAddressModel = null!;

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
    private ObservableCollection<InputPurchaseBasketModel> items = null!;

    public InputProductPurchaseProcessFormViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IPurchaseDispatchTransactionService purchaseDispatchTransactionService, ITransactionAuditService transactionAuditService, IHttpClientSysService httpClientSysService, ITransactionAuditHelperService transactionAuditHelperService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _purchaseDispatchTransactionService = purchaseDispatchTransactionService;
        _transactionAuditService = transactionAuditService;
        _httpClientSysService = httpClientSysService;
        _transactionAuditHelperService = transactionAuditHelperService;
        Items = new();
        Title = "Mal Kabul İşlemi";

        LoadPageCommand = new Command(async () => await LoadPageAsync());
        ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
        SaveCommand = new Command(async () => await SaveAsync());
    }

    public Page CurrentPage { get; set; }
    public Command LoadPageCommand { get; }
    public Command BackCommand { get; }
    public Command SaveCommand { get; }
    public Command ShowBasketItemCommand { get; }

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

            var purchaseDispatchDto = new PurchaseDispatchTransactionInsert
            {
                SpeCode = SpecialCode,
                CurrentCode = SupplierModel.Code,
                Code = string.Empty,
                DocTrackingNumber = DocumentTrackingNumber,
                DoCode = DocumentNumber,
                TransactionDate = FicheDate,
                FirmNumber = _httpClientService.FirmNumber,
                WarehouseNumber = WarehouseModel.Number,
                Description = Description,
                IsEDispatch = (short?)((bool)SupplierModel?.IsEDispatch ? 1 : 0),
                DispatchType = (short?)((bool)SupplierModel?.IsEDispatch ? 1 : 0),
                DispatchStatus = 1,
                EDispatchProfileId = (short?)((bool)SupplierModel?.IsEDispatch ? 1 : 0),
            };

            foreach (var item in Items)
            {
                var purchaseDispatchTransactionLineDto = new PurchaseDispatchTransactionLineDto
                {
                    ProductCode = item.IsVariant ? item.MainItemCode : item.ItemCode,
                    VariantCode = item.IsVariant ? item.ItemCode : "",

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

                await _transactionAuditHelperService.InsertPurchaseTransactionAuditAsync(firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
                    ioType: 1,
                    transactionType: 1,
                    transactionDate: FicheDate,
                    transactionReferenceId: result.Data.ReferenceId,
                    transactionNumber: result.Data.Code,
                    documentNumber: DocumentNumber,
                    warehouseNumber: WarehouseModel.Number,
                    warehouseName: WarehouseModel.Name,
                    productReferenceCount: Items.Count,
                    currentReferenceId: SupplierModel?.ReferenceId ?? 0,  // Default değer veya kontrol
                    currentCode: SupplierModel?.Code ?? string.Empty,     // Default değer veya kontrol
                    currentName: SupplierModel?.Name ?? string.Empty,
                    shipAddressReferenceId: ShipAddressModel?.ReferenceId ?? 0,  // Default değer veya kontrol
                    shipAddressCode: ShipAddressModel?.Code ?? string.Empty,     // Default değer veya kontrol
                    shipAddressName: ShipAddressModel?.Name ?? string.Empty);

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
        }
    }
}