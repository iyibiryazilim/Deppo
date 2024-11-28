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
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Helpers.TransactionAuditHelpers;
using Deppo.Sys.Service.Services;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.Views;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.ViewModels;

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
    private readonly ILocationTransactionService _locationTransactionService;
    private readonly ITransactionAuditService _transactionAuditService;
    private readonly IHttpClientSysService _httpClientSysService;
    private readonly ITransactionAuditHelperService _transactionAuditHelperService;

    [ObservableProperty]
    private OutputProductProcessType outputProductProcessType;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    private SupplierModel? selectedSupplier;

    [ObservableProperty]
    private ShipAddressModel? selectedShipAddress;

    [ObservableProperty]
    private Carrier? selectedCarrier;

    [ObservableProperty]
    private Driver? selectedDriver;

    public ObservableCollection<SupplierModel> Suppliers { get; } = new();
    public ObservableCollection<ShipAddressModel> ShipAddresses { get; } = new();
    public ObservableCollection<Carrier> Carriers { get; } = new();
    public ObservableCollection<Driver> Drivers { get; } = new();

    [ObservableProperty]
    private ObservableCollection<ReturnPurchaseBasketModel> items = null!;

    private ObservableCollection<LocationTransactionModel> LocationTransactions { get; } = new();

    [ObservableProperty]
    private string documentNumber = string.Empty;

    [ObservableProperty]
    private DateTime transactionDate = DateTime.Now;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string documentTrackingNumber = string.Empty;

    [ObservableProperty]
    private string specialCode = string.Empty;

    public ReturnPurchaseFormViewModel(IHttpClientService httpClientService, IPurchaseReturnDispatchTransactionService purchaseReturnDispatchTransactionService, IUserDialogs userDialogs, IPurchaseSupplierService purchaseSupplierService, IShipAddressService shipAddressService, ICarrierService carrierService, IDriverService driverService, ISupplierService supplierService, IServiceProvider serviceProvider, ILocationTransactionService locationTransactionService, ITransactionAuditService transactionAuditService, IHttpClientSysService httpClientSysService, ITransactionAuditHelperService transactionAuditHelperService)
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
        _locationTransactionService = locationTransactionService;
        _transactionAuditHelperService = transactionAuditHelperService;
        _httpClientSysService = httpClientSysService;
        _transactionAuditHelperService = transactionAuditHelperService;

        Items = new();

        Title = "Satınalma İade İrsaliyesi";

        SaveCommand = new Command(async () => await SaveAsync());
        LoadPageCommand = new Command(async () => await LoadPageAsync());
        LoadSuppliersCommand = new Command(async () => await LoadSuppliersAsync());
        LoadCarriersCommand = new Command(async () => await LoadCarriersAsync());
        LoadDriversCommand = new Command(async () => await LoadDriversAsync());
        BackCommand = new Command(async () => await BackAsync());
        LoadShipAddressesCommand = new Command<PurchaseSupplier>(async (purchaseSupplier) => await LoadShipAddressesAsync(purchaseSupplier));
    }

    public Page CurrentPage { get; set; }

    public Command SaveCommand { get; }
    public Command LoadPageCommand { get; }
    public Command LoadSuppliersCommand { get; }
    public Command LoadShipAddressesCommand { get; }
    public Command LoadCarriersCommand { get; }
    public Command LoadDriversCommand { get; }

    public Command BackCommand { get; }

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
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
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
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
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
            IsBusy = true;

            Drivers.Clear();
            SelectedDriver = null;

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

    public async Task LoadLocationTransaction(ReturnPurchaseBasketModel returnPurchaseBasketModel, ReturnPurchaseBasketDetailModel returnPurchaseBasketDetailModel)
    {
        try
        {
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            LocationTransactions.Clear();
            var result = await _locationTransactionService.GetInputObjectsAsync(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                productReferenceId: returnPurchaseBasketModel.IsVariant ? returnPurchaseBasketModel.MainItemReferenceId : returnPurchaseBasketModel.ItemReferenceId,
                variantReferenceId: returnPurchaseBasketModel.IsVariant ? returnPurchaseBasketModel.ItemReferenceId : 0,
                warehouseNumber: WarehouseModel.Number,
                locationRef: returnPurchaseBasketDetailModel.LocationReferenceId,
                skip: 0,
                take: 999999,
                search: ""
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

    private async Task SaveAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if(SelectedSupplier is null)
            {
				await _userDialogs.AlertAsync("Lütfen Tedarikçi seçiniz", "Uyarı", "Tamam");
				return;
			}

			if (SelectedDriver is null)
			{
				await _userDialogs.AlertAsync("Lütfen Sürücü seçiniz", "Uyarı", "Tamam");
				return;
			}
			if (SelectedCarrier is null)
			{
				await _userDialogs.AlertAsync("Lütfen Taşıyıcı seçiniz", "Uyarı", "Tamam");
				return;
			}

			var confirm = await _userDialogs.ConfirmAsync("Satınalma İade İrsaliyesi oluşturulacaktır. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
            if (!confirm)
                return;

            _userDialogs.ShowLoading("İşlem Tamamlanıyor...");
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var dto = new PurchaseReturnDispatchTransactionInsert
            {
                Code = "",
                CurrentCode = SelectedSupplier != null ?  SelectedSupplier.Code : "",
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
                var tempItemQuantity = item.Quantity;
				var purchaseReturnDispatchTransactionLineDto = new PurchaseReturnDispatchTransactionLineDto
                {
                    ProductCode = item.IsVariant ? item.MainItemCode : item.ItemCode,
                    VariantCode = item.IsVariant ? item.ItemCode : string.Empty,
                    WarehouseNumber = (short?)WarehouseModel.Number,
                    Quantity = item.Quantity,
                    ConversionFactor = item.Quantity * item.ConversionFactor,
                    OtherConversionFactor = item.Quantity * item.OtherConversionFactor,
                    SubUnitsetCode = item.SubUnitsetCode,
                };

                foreach (var detail in item.Details)
                {
                    var tempDetailQuantity = detail.Quantity;
					await LoadLocationTransaction(item, detail);
                    LocationTransactions.OrderBy(x => x.TransactionDate).ToList();

                    foreach (var locationTransaction in LocationTransactions)
                    {
                        var tempLocationTransactionQuantity = locationTransaction.RemainingQuantity;
						while (tempLocationTransactionQuantity > 0 && tempItemQuantity > 0 && tempDetailQuantity>0)
                        {
                            var serilotTransactionDto = new SeriLotTransactionDto
                            {
                                StockLocationCode = detail.LocationCode,
                                InProductTransactionLineReferenceId = locationTransaction.TransactionReferenceId,
                                OutProductTransactionLineReferenceId = locationTransaction.ReferenceId,
                                Quantity =tempDetailQuantity > tempLocationTransactionQuantity ? tempLocationTransactionQuantity : tempDetailQuantity,
                                SubUnitsetCode = item.SubUnitsetCode,
                                DestinationStockLocationCode = string.Empty,
                                ConversionFactor = item.ConversionFactor * (tempDetailQuantity > tempLocationTransactionQuantity ? tempLocationTransactionQuantity : tempDetailQuantity),
                                OtherConversionFactor = item.OtherConversionFactor * (tempDetailQuantity > tempLocationTransactionQuantity ? tempLocationTransactionQuantity : tempDetailQuantity),
                            };
                            purchaseReturnDispatchTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);
							tempLocationTransactionQuantity -= (double)serilotTransactionDto.Quantity;
                            tempItemQuantity -= (double)serilotTransactionDto.Quantity;
							tempDetailQuantity -= (double)serilotTransactionDto.Quantity;
						}
                    }
                }

                dto.Lines.Add(purchaseReturnDispatchTransactionLineDto);
            }
            Console.WriteLine(dto);

            var result = await _purchaseReturnDispatchTransactionService.InsertPurchaseReturnDispatchTransaction(httpClient, _httpClientService.FirmNumber, dto);

            ResultModel resultModel = new();
            if (result.IsSuccess)
            {
               

                resultModel.Message = "Başarılı";
                resultModel.Code = result.Data.Code;
                resultModel.PageTitle = Title;
                resultModel.PageCountToBack = 5;

                try
                {
                    await _transactionAuditHelperService.InsertPurchaseTransactionAuditAsync(firmNumber: _httpClientService.FirmNumber,
                        periodNumber: _httpClientService.PeriodNumber,
                        ioType: 3,
                        transactionType: 6,
                        transactionDate: TransactionDate,
                        transactionReferenceId: result.Data.ReferenceId,
                        transactionNumber: result.Data.Code,
                        documentNumber: DocumentNumber,
                        warehouseNumber: WarehouseModel.Number,
                        warehouseName: WarehouseModel.Name,
                        productReferenceCount: Items.Count,
                        currentReferenceId: SelectedSupplier is not null ? SelectedSupplier.ReferenceId : 0,
                        currentCode: SelectedSupplier is not null ? SelectedSupplier.Code : string.Empty,
                        currentName: SelectedSupplier is not null ? SelectedSupplier.Name : string.Empty,
                        shipAddressReferenceId: SelectedShipAddress is not null ?  SelectedShipAddress.ReferenceId : 0,
                        shipAddressCode: SelectedShipAddress is not null ?  SelectedShipAddress.Code : string.Empty,
                        shipAddressName: SelectedShipAddress is not null ?  SelectedShipAddress.Name : string.Empty
                        );
                }
                catch (Exception ex)
                {
                    _userDialogs.Alert(ex.Message, "Hata", "Tamam");
                }

                await ClearFormAsync();
				await ClearDataAsync();

				if (_userDialogs.IsHudShowing)
					_userDialogs.HideHud();

				await Shell.Current.GoToAsync($"{nameof(InsertSuccessPageView)}", new Dictionary<string, object>
                {
                    [nameof(ResultModel)] = resultModel
                });
            }
            else
            {
                resultModel.Message = "Başarısız";
                resultModel.PageTitle = Title;
                resultModel.PageCountToBack = 1;
                resultModel.ErrorMessage = result.Message;

				if (_userDialogs.IsHudShowing)
					_userDialogs.HideHud();

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
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
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

            DocumentNumber = string.Empty;
            SpecialCode = string.Empty;
            DocumentTrackingNumber = string.Empty;
            Description = string.Empty;
            SelectedCarrier = null;
            SelectedDriver = null;
            SelectedShipAddress = null;
            SelectedSupplier = null;

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
        finally
        {
            IsBusy = false;
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
        }
    }

    private async Task ClearFormAsync()
    {
        try
        {
            DocumentNumber = string.Empty;
            SpecialCode = string.Empty;
            DocumentTrackingNumber = string.Empty;
            Description = string.Empty;
            SelectedCarrier = null;
            SelectedDriver = null;
            SelectedSupplier = null;
            SelectedShipAddress = null;
        }
        catch (Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
    }
    private async Task ClearDataAsync()
    {
        try
        {
            var warehouseListViewModel = _serviceProvider.GetRequiredService<ReturnPurchaseWarehouseListViewModel>();
			var basketViewModel = _serviceProvider.GetRequiredService<ReturnPurchaseBasketViewModel>();
            var productListViewModel = _serviceProvider.GetRequiredService<ReturnPurchaseProductListViewModel>();

            if(warehouseListViewModel is not null)
            {
                if(warehouseListViewModel.SelectedWarehouseModel is not null)
                {
                    warehouseListViewModel.SelectedWarehouseModel.IsSelected = false;
					warehouseListViewModel.SelectedWarehouseModel = null;
				}
            }
            productListViewModel.SelectedItems.Clear();
            productListViewModel.SelectedProducts.Clear();

			foreach (var item in basketViewModel.Items)
            {
                item.Details.Clear();
            }
			basketViewModel.SelectedLocationTransactions.Clear();
			basketViewModel.SelectedSeriLotTransactions.Clear();
            basketViewModel.Items.Clear();

		}
        catch (Exception ex)
        {
			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
    }
}