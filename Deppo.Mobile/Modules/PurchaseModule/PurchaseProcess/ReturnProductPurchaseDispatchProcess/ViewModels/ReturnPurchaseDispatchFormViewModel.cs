using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.PurchaseReturnDispatchTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Helpers.TransactionAuditHelpers;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;
using Deppo.Sys.Service.Services;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(PurchaseSupplier), queryId: nameof(PurchaseSupplier))]
[QueryProperty(name: nameof(PurchaseFicheModel), queryId: nameof(PurchaseFicheModel))]
[QueryProperty(name: nameof(SelectedPurchaseTransactions), queryId: nameof(SelectedPurchaseTransactions))]
[QueryProperty(name: nameof(Items), queryId: nameof(Items))]
public partial class ReturnPurchaseDispatchFormViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IPurchaseReturnDispatchTransactionService _purchaseReturnDispatchTransactionService;
    private readonly IUserDialogs _userDialogs;
    private readonly ICarrierService _carrierService;
    private readonly IDriverService _driverService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILocationTransactionService _locationTransactionService;
    private readonly ITransactionAuditService _transactionAuditService;
    private readonly IHttpClientSysService _httpClientSysService;
    private readonly ITransactionAuditHelperService _transactionAuditHelperService;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    private PurchaseSupplier purchaseSupplier = null!;

    [ObservableProperty]
    private PurchaseFicheModel purchaseFicheModel = null!;

    [ObservableProperty]
    public ObservableCollection<PurchaseTransactionModel> selectedPurchaseTransactions;

    [ObservableProperty]
    public ObservableCollection<ReturnPurchaseBasketModel> items = null!;

    public ObservableCollection<LocationTransactionModel> LocationTransactions { get; } = new();

    [ObservableProperty]
    private Carrier? selectedCarrier;

    [ObservableProperty]
    private Driver? selectedDriver;

    public ObservableCollection<Carrier> Carriers { get; } = new();
    public ObservableCollection<Driver> Drivers { get; } = new();

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

    public ReturnPurchaseDispatchFormViewModel(IHttpClientService httpClientService, IPurchaseReturnDispatchTransactionService purchaseReturnDispatchTransactionService, IUserDialogs userDialogs, ICarrierService carrierService, IDriverService driverService, IServiceProvider serviceProvider, ILocationTransactionService locationTransactionService)
    {
        _httpClientService = httpClientService;
        _purchaseReturnDispatchTransactionService = purchaseReturnDispatchTransactionService;
        _userDialogs = userDialogs;
        _locationTransactionService = locationTransactionService;
		_carrierService = carrierService;
		_driverService = driverService;
		_serviceProvider = serviceProvider;
		Items = new();

        Title = "Satınalma İade İrsaliyesi";

        SaveCommand = new Command(async () => await SaveAsync());
        LoadPageCommand = new Command(async () => await LoadPageAsync());
        LoadCarriersCommand = new Command(async () => await LoadCarriersAsync());
        LoadDriversCommand = new Command(async () => await LoadDriversAsync());
        BackCommand = new Command(async () => await BackAsync());
    }

    public Page CurrentPage { get; set; }

    public Command SaveCommand { get; }
    public Command LoadPageCommand { get; }
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

            if(SelectedDriver is null)
            {
                await _userDialogs.AlertAsync("Lütfen Sürücü seçiniz", "Uyarı", "Tamam");
                return;
            }
            if(SelectedCarrier is null)
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
                CurrentCode = PurchaseSupplier.Code,
                DriverFirstName = SelectedDriver != null ? SelectedDriver.Name : "",
                DriverLastName = SelectedDriver != null ? SelectedDriver.Surname : "",
                CarrierCode = SelectedCarrier != null ? SelectedCarrier.Code : "",
                IdentityNumber = SelectedDriver != null ? SelectedDriver.IdentityNumber : "",
                Plaque = SelectedDriver != null ? SelectedDriver.PlateNumber : "",
                IsEDispatch = (short?)((bool)PurchaseSupplier?.IsEDispatch ? 1 : 0),
                DispatchType = (short?)((bool)PurchaseSupplier?.IsEDispatch ? 1 : 0),
                DispatchStatus = 1,
                EDispatchProfileId = (short?)((bool)PurchaseSupplier?.IsEDispatch ? 1 : 0),
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
                    DispatchReferenceId = item.DispatchReferenceId,
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
						while (tempLocationTransactionQuantity > 0 && tempItemQuantity > 0 && tempDetailQuantity >0)
                        {
                            var serilotTransactionDto = new SeriLotTransactionDto
                            {
                                StockLocationCode = detail.LocationCode,
                                InProductTransactionLineReferenceId = locationTransaction.TransactionReferenceId,
                                OutProductTransactionLineReferenceId = locationTransaction.ReferenceId,
                                Quantity = tempDetailQuantity > tempLocationTransactionQuantity ? tempLocationTransactionQuantity : tempDetailQuantity,
                                SubUnitsetCode = item.SubUnitsetCode,
                                DestinationStockLocationCode = string.Empty,
                                ConversionFactor = item.ConversionFactor * (tempDetailQuantity > tempLocationTransactionQuantity ? tempLocationTransactionQuantity : tempDetailQuantity),
                                OtherConversionFactor = item.OtherConversionFactor * (tempDetailQuantity > tempLocationTransactionQuantity ? tempLocationTransactionQuantity : tempDetailQuantity),
							};
                            purchaseReturnDispatchTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);
							tempLocationTransactionQuantity -= (double)serilotTransactionDto.Quantity;
							tempDetailQuantity -= (double)serilotTransactionDto.Quantity;
							tempItemQuantity -= (double)serilotTransactionDto.Quantity;
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
                resultModel.PageCountToBack = 7;

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
                        currentReferenceId: PurchaseSupplier.ReferenceId,
                        currentCode: PurchaseSupplier.Code,
                        currentName: PurchaseSupplier.Name,
                        shipAddressReferenceId: PurchaseSupplier.ShipAddressReferenceId,
                        shipAddressCode: PurchaseSupplier.ShipAddressCode,
                        shipAddressName: PurchaseSupplier.ShipAddressName

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
                resultModel.ErrorMessage = result.Message;
                resultModel.PageCountToBack = 1;

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
			var basketViewModel = _serviceProvider.GetRequiredService<ReturnPurchaseDispatchBasketViewModel>();
            var supplierListViewModel = _serviceProvider.GetRequiredService<ReturnPurchaseDispatchSupplierListViewModel>();
            var productListViewModel = _serviceProvider.GetRequiredService<ReturnPurchaseDispatchProductListViewModel>();

            if(supplierListViewModel.PurchaseSupplier is not null)
            {
                supplierListViewModel.PurchaseSupplier.IsSelected = false;
                supplierListViewModel.PurchaseSupplier = null;
            }

			productListViewModel.SelectedProducts.Clear();
            productListViewModel.SelectedItems.Clear();

			foreach (var item in basketViewModel.Items)
            {
                item.Details.Clear();
            }
            basketViewModel.Items.Clear();
			basketViewModel.SelectedPurchaseTransactions.Clear();
			basketViewModel.SelectedLocationTransactions.Clear();
			basketViewModel.SelectedSeriLotTransactions.Clear();
		}
        catch (Exception ex)
        {
			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
    }
}