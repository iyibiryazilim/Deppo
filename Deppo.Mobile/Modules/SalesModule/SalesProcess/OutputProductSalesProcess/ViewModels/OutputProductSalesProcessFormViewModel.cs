using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.SalesDispatchTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.ShipAddressModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Helpers.TransactionAuditHelpers;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;
using Deppo.Sys.Service.Services;
using DevExpress.Maui.Controls;
using System.Collections.ObjectModel;
using ICustomerService = Deppo.Core.Services.ICustomerService;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(Items), queryId: nameof(Items))]
public partial class OutputProductSalesProcessFormViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly ICustomerService _customerService;
    private readonly IShipAddressService _shipAddressService;
    private readonly IUserDialogs _userDialogs;
    private readonly ICarrierService _carrierService;
    private readonly IDriverService _driverService;
    private readonly IWholeSalesDispatchTransactionService _wholeSalesDispatchTransactionService;
    private readonly IRetailSalesDispatchTransactionService _retailSalesDispatchTransactionService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILocationTransactionService _locationTransactionService;
    private readonly ITransactionAuditService _transactionAuditService;
    private readonly IHttpClientSysService _httpClientSysService;
    private readonly ITransactionAuditHelperService _transactionAuditHelperService;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    private ObservableCollection<OutputSalesBasketModel> items = null!;

    public ObservableCollection<CustomerModel> Customers { get; } = new();
    public ObservableCollection<ShipAddressModel> ShipAddresses { get; } = new();
    public ObservableCollection<Carrier> Carriers { get; } = new();
    public ObservableCollection<Driver> Drivers { get; } = new();

    public ObservableCollection<LocationTransactionModel> LocationTransactions { get; } = new();

    [ObservableProperty]
    private CustomerModel? selectedCustomer;

    [ObservableProperty]
    private ShipAddressModel? selectedShipAddress;

    [ObservableProperty]
    private Carrier? selectedCarrier;

    [ObservableProperty]
    private Driver? selectedDriver;

    [ObservableProperty]
    private DateTime transactionDate = DateTime.Now;

    [ObservableProperty]
    private string documentNumber = string.Empty;

    [ObservableProperty]
    private string documentTrackingNumber = string.Empty;

    [ObservableProperty]
    private string specialCode = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string cargoTrackingNumber = string.Empty;

    public OutputProductSalesProcessFormViewModel(IHttpClientService httpClientService, IShipAddressService shipAddressService, IUserDialogs userDialogs, ICustomerService customerService, ICarrierService carrierService, IDriverService driverService, IWholeSalesDispatchTransactionService wholeSalesDispatchTransactionService, IRetailSalesDispatchTransactionService retailSalesDispatchTransactionService, IServiceProvider serviceProvider, ILocationTransactionService locationTransactionService, ITransactionAuditService transactionAuditService, IHttpClientSysService httpClientSysService, ITransactionAuditHelperService transactionAuditHelperService)
    {
        _httpClientService = httpClientService;
        _shipAddressService = shipAddressService;
        _userDialogs = userDialogs;
        _customerService = customerService;
        _carrierService = carrierService;
        _driverService = driverService;
        _wholeSalesDispatchTransactionService = wholeSalesDispatchTransactionService;
        _retailSalesDispatchTransactionService = retailSalesDispatchTransactionService;
        _serviceProvider = serviceProvider;
        _locationTransactionService = locationTransactionService;
        _transactionAuditService = transactionAuditService;
        _httpClientSysService = httpClientSysService;
        _transactionAuditHelperService = transactionAuditHelperService;

        Title = "Sevk İşlemi";

        LoadPageCommand = new Command(async () => await LoadPageAsync());
        SaveCommand = new Command(async () => await SaveAsync());
        ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
        BackCommand = new Command(async () => await BackAsync());

        LoadCustomersCommand = new Command(async () => await LoadCustomersAsync());
        LoadShipAddressesCommand = new Command<CustomerModel>(async (x) => await LoadShipAddressesAsync(x));
        LoadCarriersCommand = new Command(async () => await LoadCarriersAsync());
        LoadDriversCommand = new Command(async () => await LoadDriversAsync());

        SelectWholeCommand = new Command(async () => await SelectWholeAsync());
        SelectRetailCommand = new Command(async () => await SelectRetailAsync());
    }

    public Page CurrentPage { get; set; } = null!;

    public Command LoadPageCommand { get; }
    public Command BackCommand { get; }
    public Command SaveCommand { get; }
    public Command ShowBasketItemCommand { get; }
    public Command LoadCustomersCommand { get; }
    public Command<CustomerModel> LoadShipAddressesCommand { get; }

    public Command LoadCarriersCommand { get; }

    public Command LoadDriversCommand { get; }
    public Command SelectWholeCommand { get; }
    public Command SelectRetailCommand { get; }

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

            var result = await _customerService.GetObjects(
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
                    Customers.Add(Mapping.Mapper.Map<CustomerModel>(item));
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

    private async Task LoadShipAddressesAsync(CustomerModel salesCustomer)
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
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            if(SelectedCustomer == null)
            {
                _userDialogs.Alert("Müşteri alanı zorunlu", "Uyarı", "Tamam");
                return;
            }
            if(SelectedCustomer.IsEDispatch)
            {
                if(SelectedCarrier == null)
                {
                    _userDialogs.Alert("Taşıyıcı alanı zorunlu", "Uyarı", "Tamam");
                    return;
                }
                if (SelectedDriver == null)
                {
					_userDialogs.Alert("Şoför alanı zorunlu", "Uyarı", "Tamam");
					return;
				}
            }

            await OpenInsertOptionsAsync();
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

    private async Task SelectWholeAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var confirmInsert = await _userDialogs.ConfirmAsync("Toptan Satış İrsaliyesi oluşturulacaktır. Devam etmek istediğinize emin misiniz?", "Onay", "Evet", "Hayır");
            if (!confirmInsert)
            {
                await CloseInsertOptionsAsync();
                return;
            }
            await CloseInsertOptionsAsync();

            _userDialogs.Loading("İşlem tamamlanıyor...");
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            await WholeSalesDispatchTransactionInsertAsync(httpClient);

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
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

    private async Task SelectRetailAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;

            var confirmInsert = await _userDialogs.ConfirmAsync("Perakende Satış İrsaliyesi oluşturulacaktır. Devam etmek istediğinize emin misiniz?", "Onay", "Evet", "Hayır");
            if (!confirmInsert)
            {
                await CloseInsertOptionsAsync();
                return;
            }
            _userDialogs.Loading("İşlem tamamlanıyor...");
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            await RetailSalesDispatchTransactionInsertAsync(httpClient);

            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();
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

    public async Task LoadLocationTransaction(OutputSalesBasketModel outputSalesBasketModel, OutputSalesBasketDetailModel outputSalesBasketDetailModel)
    {
        try
        {
            LocationTransactions.Clear();

            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _locationTransactionService.GetInputObjectsAsync(
                httpClient: httpClient,
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                productReferenceId: outputSalesBasketModel.IsVariant ? outputSalesBasketModel.MainItemReferenceId : outputSalesBasketModel.ItemReferenceId,
                variantReferenceId: outputSalesBasketModel.IsVariant ? outputSalesBasketModel.ItemReferenceId : 0,
                warehouseNumber: WarehouseModel.Number,
                locationRef: outputSalesBasketDetailModel.LocationReferenceId,
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

    private async Task WholeSalesDispatchTransactionInsertAsync(HttpClient httpClient)
    {
        var dto = new WholeSalesDispatchTransactionInsert
        {
            Code = "",
            CurrentCode = SelectedCustomer != null ? SelectedCustomer.Code : "",
            DriverFirstName = SelectedDriver != null ? SelectedDriver.Name : "",
            DriverLastName = SelectedDriver != null ? SelectedDriver.Surname : "",
            CarrierCode = SelectedCarrier != null ? SelectedCarrier.Code : "",
            IdentityNumber = SelectedDriver != null ? SelectedDriver.IdentityNumber : "",
            ShipInfoCode = SelectedShipAddress != null ? SelectedShipAddress.Code : "",
            Plaque = SelectedDriver != null ? SelectedDriver.PlateNumber : "",
            IsEDispatch = (short?)((bool)SelectedCustomer?.IsEDispatch ? 1 : 0),
            DispatchType = (short?)((bool)SelectedCustomer?.IsEDispatch ? 1 : 0),
            DispatchStatus = 1,
            EDispatchProfileId = (short?)((bool)SelectedCustomer?.IsEDispatch ? 1 : 0),
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
			var wholeSalesDispatchTransactionLineDto = new WholeSalesDispatchTransactionLineInsert
            {
                ProductCode = item.IsVariant ? item.MainItemCode : item.ItemCode,
                VariantCode = item.IsVariant ? item.ItemCode : string.Empty,
                WarehouseNumber = (short?)WarehouseModel.Number,
                Quantity = item.Quantity,
                ConversionFactor = 1,
                OtherConversionFactor = 1,
                SubUnitsetCode = item.SubUnitsetCode,
            };

            foreach (var detail in item.Details)
            {
				var tempDetailQuantity = detail.Quantity;
				await LoadLocationTransaction(item, detail);
                LocationTransactions.OrderBy(x => x.TransactionDate).ToList();

                foreach (var locationTransaction in LocationTransactions)
                {
					var tempLocationRemaningQuantity = locationTransaction.RemainingQuantity;
					while (tempLocationRemaningQuantity > 0 && tempItemQuantity > 0 && tempDetailQuantity > 0)
                    {
                        var serilotTransactionDto = new SeriLotTransactionDto
                        {
                            StockLocationCode = detail.LocationCode,
                            InProductTransactionLineReferenceId = locationTransaction.TransactionReferenceId,
                            OutProductTransactionLineReferenceId = locationTransaction.ReferenceId,
                            Quantity = tempDetailQuantity > tempLocationRemaningQuantity ? tempLocationRemaningQuantity : tempDetailQuantity,
                            SubUnitsetCode = item.SubUnitsetCode,
                            DestinationStockLocationCode = string.Empty,
                            ConversionFactor = 1,
                            OtherConversionFactor = 1,
                        };
                        tempLocationRemaningQuantity -= (double)serilotTransactionDto.Quantity;
						wholeSalesDispatchTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);
						tempItemQuantity -= (double)serilotTransactionDto.Quantity;
						tempDetailQuantity -= (double)serilotTransactionDto.Quantity;
                    }
                }
            }
            dto.Lines.Add(wholeSalesDispatchTransactionLineDto);
        }

        var result = await _wholeSalesDispatchTransactionService.InsertWholeSalesDispatchTransaction(httpClient, _httpClientService.FirmNumber, dto);

        ResultModel resultModel = new();
        if (result.IsSuccess)
        {
            resultModel.Message = "Başarılı";
            resultModel.Code = result.Data.Code;
            resultModel.PageTitle = "Sevk İşlemi";
            resultModel.PageCountToBack = 5;

            try
            {
                await _transactionAuditHelperService.InsertSalesTransactionAuditAsync(
                firmNumber: _httpClientService.FirmNumber,
                periodNumber: _httpClientService.PeriodNumber,
                ioType: 3,
                transactionType: 8,
                transactionDate: TransactionDate,
                transactionReferenceId: result.Data.ReferenceId,
                transactionNumber: result.Data.Code,
                documentNumber: DocumentNumber,
                warehouseNumber: WarehouseModel.Number,
                warehouseName: WarehouseModel.Name,
                productReferenceCount: Items.Count,
                currentReferenceId: SelectedCustomer.ReferenceId,
                currentCode: SelectedCustomer.Code,
                currentName: SelectedCustomer.Name,
                shipAddressReferenceId: SelectedShipAddress.ReferenceId,
                shipAddressCode: SelectedShipAddress.Code,
                shipAddressName: SelectedShipAddress.Name

               );
            }
            catch (Exception ex)
            {
                await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
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

    private async Task RetailSalesDispatchTransactionInsertAsync(HttpClient httpClient)
    {
        var dto = new RetailSalesDispatchTransactionInsert
        {
            Code = "",
            CurrentCode = SelectedCustomer != null ? SelectedCustomer.Code : "",
            DriverFirstName = SelectedDriver != null ? SelectedDriver.Name : "",
            DriverLastName = SelectedDriver != null ? SelectedDriver.Surname : "",
            CarrierCode = SelectedCarrier != null ? SelectedCarrier.Code : "",
            IdentityNumber = SelectedDriver != null ? SelectedDriver.IdentityNumber : "",
            Plaque = SelectedDriver != null ? SelectedDriver.PlateNumber : "",
            ShipInfoCode = SelectedShipAddress != null ? SelectedShipAddress.Code : "",
            IsEDispatch = (short?)((bool)SelectedCustomer?.IsEDispatch ? 1 : 0),
            DispatchType = (short?)((bool)SelectedCustomer?.IsEDispatch ? 1 : 0),
            DispatchStatus = 1,
            EDispatchProfileId = (short?)((bool)SelectedCustomer?.IsEDispatch ? 1 : 0),
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
			var retailSalesDispatchTransactionLineDto = new RetailSalesDispatchTransactionLineInsert
            {
                ProductCode = item.IsVariant ? item.MainItemCode : item.ItemCode,
                VariantCode = item.IsVariant ? item.ItemCode : string.Empty,
                WarehouseNumber = (short?)WarehouseModel.Number,
                Quantity = item.Quantity,
                ConversionFactor = 1,
                OtherConversionFactor = 1,
                SubUnitsetCode = item.SubUnitsetCode,
            };

            foreach (var detail in item.Details)
            {
				var tempDetailQuantity = detail.Quantity;
				await LoadLocationTransaction(item, detail);
                LocationTransactions.OrderBy(x => x.TransactionDate).ToList();

                foreach (var locationTransaction in LocationTransactions)
                {
					var tempLocationRemaningQuantity = locationTransaction.RemainingQuantity;
					while (locationTransaction.RemainingQuantity > 0)
                    {
						var serilotTransactionDto = new SeriLotTransactionDto
                        {
                            StockLocationCode = detail.LocationCode,
                            InProductTransactionLineReferenceId = detail.TransactionReferenceId,
                            OutProductTransactionLineReferenceId = detail.ReferenceId,
                            Quantity = tempDetailQuantity > tempLocationRemaningQuantity ? tempLocationRemaningQuantity : tempDetailQuantity,
                            SubUnitsetCode = item.SubUnitsetCode,
                            DestinationStockLocationCode = string.Empty,
                            ConversionFactor = 1,
                            OtherConversionFactor = 1,
                        };

                        tempLocationRemaningQuantity -= (double)serilotTransactionDto.Quantity;
                        retailSalesDispatchTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);
                        tempItemQuantity -= (double)serilotTransactionDto.Quantity;
						tempDetailQuantity -= (double)serilotTransactionDto.Quantity;
                    }
                }
            }

            dto.Lines.Add(retailSalesDispatchTransactionLineDto);
        }
        
        var result = await _retailSalesDispatchTransactionService.InsertRetailSalesDispatchTransaction(httpClient, _httpClientService.FirmNumber, dto);

        ResultModel resultModel = new();
        if (result.IsSuccess)
        {
            resultModel.Message = "Başarılı";
            resultModel.Code = result.Data.Code;
            resultModel.PageTitle = "Sevk İşlemi";
            resultModel.PageCountToBack = 5;

			try
			{
				await _transactionAuditHelperService.InsertSalesTransactionAuditAsync(
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				ioType: 3,
				transactionType: 7,
				transactionDate: TransactionDate,
				transactionReferenceId: result.Data.ReferenceId,
				transactionNumber: result.Data.Code,
				documentNumber: DocumentNumber,
				warehouseNumber: WarehouseModel.Number,
				warehouseName: WarehouseModel.Name,
				productReferenceCount: Items.Count,
				currentReferenceId: SelectedCustomer.ReferenceId,
				currentCode: SelectedCustomer.Code,
				currentName: SelectedCustomer.Name,
				shipAddressReferenceId: SelectedShipAddress.ReferenceId,
				shipAddressCode: SelectedShipAddress.Code,
				shipAddressName: SelectedShipAddress.Name

			   );
			}
			catch (Exception ex)
			{
				await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
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

    private async Task CloseInsertOptionsAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            CurrentPage.FindByName<BottomSheet>("insertOptionsBottomSheet").State = BottomSheetState.Hidden;
        });
    }

    private async Task OpenInsertOptionsAsync()
    {
        await MainThread.InvokeOnMainThreadAsync(() =>
        {
            CurrentPage.FindByName<BottomSheet>("insertOptionsBottomSheet").State = BottomSheetState.HalfExpanded;
        });
    }

    private async Task ClearDataAsync()
    {
        try
        {
            var warehouseListViewModel = _serviceProvider.GetRequiredService<OutputProductSalesProcessWarehouseListViewModel>();
            var basketViewModel = _serviceProvider.GetRequiredService<OutputProductSalesProcessBasketListViewModel>();

            if(warehouseListViewModel is not null && warehouseListViewModel.SelectedWarehouseModel is not null)
            {
                warehouseListViewModel.SelectedWarehouseModel.IsSelected = false;
                warehouseListViewModel.SelectedWarehouseModel = null;
            }

            if(basketViewModel is not null)
            {
				foreach (var item in basketViewModel.Items)
				{
					item.Details.Clear();
				}
				basketViewModel.Items.Clear();
				basketViewModel.SelectedLocationTransactions.Clear();
				basketViewModel.SelectedSeriLotTransactions.Clear();
			}
        }
        catch (Exception ex)
        {
			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
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
            SelectedCustomer = null;
            SelectedShipAddress = null;
        }
        catch (Exception ex)
        {
            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
    }
}