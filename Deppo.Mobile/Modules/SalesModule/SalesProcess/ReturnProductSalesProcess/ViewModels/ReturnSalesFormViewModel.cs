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
using static Deppo.Mobile.Core.Helpers.DeppoEnums;
using Deppo.Core.DTOs.SalesDispatchTransaction;
using Deppo.Core.DataResultModel;
using Deppo.Core.ResponseResultModels;
using Deppo.Core.DTOs.RetailSalesReturnDispatchTransaction;
using Deppo.Core.DTOs.WholeSalesReturnDispatchTransaction;
using System.Windows.Input;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(Items), queryId: nameof(Items))]
public partial class ReturnSalesFormViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IUserDialogs _userDialogs;
    private readonly IRetailSalesReturnDispatchTransactionService _retailService;
    private readonly IWholeSalesReturnDispatchTransactionService _wholeService;
    private readonly ISalesCustomerService _salesCustomerService;
    private readonly IShipAddressService _shipAddressService;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

    public ObservableCollection<SalesCustomer> Customers { get; } = new();
    public ObservableCollection<ShipAddressModel> ShipAddresses { get; } = new();

    [ObservableProperty]
    private SalesCustomer? selectedCustomer;

    [ObservableProperty]
    private ShipAddressModel? selectedShipAddress;

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
    private SalesReturnEnumType salesReturnEnumType = SalesReturnEnumType.Retail;
    [ObservableProperty]
    private string ficheNo  = string.Empty;

    [ObservableProperty]
    private ObservableCollection<ReturnSalesBasketModel> items = null!;

    public ReturnSalesFormViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, ISalesCustomerService salesCustomerService, IShipAddressService shipAddressService, IRetailSalesReturnDispatchTransactionService retailService, IWholeSalesReturnDispatchTransactionService wholeService, IServiceProvider serviceProvider)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _salesCustomerService = salesCustomerService;
        _shipAddressService = shipAddressService;
        _serviceProvider = serviceProvider;
		_retailService = retailService;
		_wholeService = wholeService;

		Items = new();
        Title = "Satış İade Form İşlemi";

        LoadPageCommand = new Command(async () => await LoadPageAsync());
        ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
        LoadCustomersCommand = new Command(async () => await LoadCustomersAsync());
        LoadShipAddressesCommand = new Command<SalesCustomer>(async (x) => await LoadShipAddressesAsync(x));
        SelectWholeCommand = new Command(async (x) => await SelectTransactionTypeAsync(SalesReturnEnumType.Whole));
        SelectRetailCommand = new Command(async (x) => await SelectTransactionTypeAsync(SalesReturnEnumType.Retail));
        SaveCommand = new Command(OpenBottomSheetAsync);
		BackCommand = new Command(async () => await BackAsync());
	}

    public Page CurrentPage { get; set; }
    public Command LoadPageCommand { get; }
    public Command BackCommand { get; }
    public Command SaveCommand { get; }
    public Command ShowBasketItemCommand { get; }
    public Command LoadCustomersCommand { get; }
    public Command<SalesCustomer> LoadShipAddressesCommand { get; }

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

    private void OpenBottomSheetAsync()
    {
        // BottomSheet'i aç
        CurrentPage.FindByName<BottomSheet>("selectConfirmBottomSheet").State = BottomSheetState.HalfExpanded;
    }

    private async Task SelectTransactionTypeAsync(SalesReturnEnumType selectedType)
    {
        SalesReturnEnumType = selectedType;

        // BottomSheet'i kapat
        CurrentPage.FindByName<BottomSheet>("selectConfirmBottomSheet").State = BottomSheetState.Hidden;

        // SaveCommand'i tetikle
        await SaveAsync();
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

            SelectedCustomer.ShipAddressCode = SelectedShipAddress.Code;
            SelectedCustomer.ShipAddressReferenceId = SelectedShipAddress.ReferenceId;
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            DataResult<ResponseModel> result = new();
            if (SalesReturnEnumType == SalesReturnEnumType.Retail)
            {
                RetailSalesReturnDispatchTransactionInsert dto = new()
                {
                    
                    SpeCode = SpecialCode,
                    CurrentCode = SelectedCustomer.Code,
                    DispatchType = (short)SelectedCustomer.FicheType,
                    IsEDispatch = (short)SelectedCustomer.FicheType,
                    Code = FicheNo,
                    DocTrackingNumber = DocumentTrackingNumber,
                    DoCode = DocumentNumber,
                    TransactionDate = FicheDate,
                    FirmNumber = _httpClientService.FirmNumber,
                    WarehouseNumber = WarehouseModel.Number,
                    Description = Description,
                    ShipInfoCode = SelectedShipAddress.Code ?? string.Empty,
                };
                foreach (var item in Items)
                {
                    var line = new RetailSalesReturnDispatchTransactionLineInsert
                    {
                        VariantCode = item.IsVariant ? item.ItemCode : string.Empty,
                        ProductCode = item.IsVariant ? item.MainItemCode : item.ItemCode,
                        WarehouseNumber = (short)WarehouseModel.Number,
                        Quantity = item.Quantity,
                        ConversionFactor = item.Quantity * item.ConversionFactor,
                        OtherConversionFactor = item.Quantity * item.OtherConversionFactor,
                        SubUnitsetCode = item.SubUnitsetCode,

                        //DispatchReferenceId =0
                    };
                    dto.Lines.Add(line);
                    foreach (var detail in item.Details)
                    {
                        var seriLotTransactionDto = new SeriLotTransactionDto
                        {
                            StockLocationCode = detail.LocationCode,
                            SubUnitsetCode = item.SubUnitsetCode,
                            Quantity = detail.Quantity,
							ConversionFactor = detail.Quantity * item.ConversionFactor,
							OtherConversionFactor = detail.Quantity * item.OtherConversionFactor,
							DestinationStockLocationCode = string.Empty,
                        };
                        line.SeriLotTransactions.Add(seriLotTransactionDto);
                    }
                }
                result = await _retailService.InsertRetailSalesReturnDispatchTransaction(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, dto);
            }
            else
            {
                WholeSalesReturnDispatchTransactionInsert dto = new()
                {
                    SpeCode = SpecialCode,
                    CurrentCode = SelectedCustomer.Code,
                    DispatchType = (short)SelectedCustomer.FicheType,
                    IsEDispatch = (short)SelectedCustomer.FicheType,
                    Code = FicheNo,
                    DocTrackingNumber = DocumentTrackingNumber,
                    DoCode = DocumentNumber,
                    TransactionDate = FicheDate,
                    FirmNumber = _httpClientService.FirmNumber,
                    WarehouseNumber = WarehouseModel.Number,
                    Description = Description,
                    ShipInfoCode = SelectedShipAddress.Code ?? string.Empty,
                };
                foreach (var item in Items)
                {
                    var wholeSalesDispatchTransactionLineInsert = new WholeSalesReturnTransactionLineInsert
                    {
						VariantCode = item.IsVariant ? item.ItemCode : string.Empty,
						ProductCode = item.IsVariant ? item.MainItemCode : item.ItemCode,
						WarehouseNumber = (short)WarehouseModel.Number,
                        Quantity = item.Quantity,
						ConversionFactor = item.Quantity * item.ConversionFactor,
						OtherConversionFactor = item.Quantity * item.OtherConversionFactor,
						SubUnitsetCode = item.SubUnitsetCode,

                        //DispatchReferenceId =0
                    };
                    dto.Lines.Add(wholeSalesDispatchTransactionLineInsert);
                    foreach (var detail in item.Details)
                    {
                        var seriLotTransactionDto = new SeriLotTransactionDto
                        {
                            StockLocationCode = detail.LocationCode,
                            SubUnitsetCode = item.SubUnitsetCode,
                            Quantity = detail.Quantity,
							ConversionFactor = detail.Quantity * item.ConversionFactor,
							OtherConversionFactor = detail.Quantity * item.OtherConversionFactor,
							DestinationStockLocationCode = string.Empty,
                        };
                        wholeSalesDispatchTransactionLineInsert.SeriLotTransactions.Add(seriLotTransactionDto);
                    }
                }
                result = await _wholeService.InsertWholeSalesReturnDispatchTransaction(httpClient: httpClient, firmNumber: _httpClientService.FirmNumber, dto);
            }

            Console.WriteLine(result);
            ResultModel resultModel = new();
            if (result.IsSuccess)
            {
                resultModel.Message = "Başarılı";
                resultModel.Code = result.Data.Code;
                resultModel.PageTitle = Title;
                resultModel.PageCountToBack = 5;

				DocumentNumber = string.Empty;
				DocumentTrackingNumber = string.Empty;
				SpecialCode = string.Empty;
				Description = string.Empty;
				FicheNo = string.Empty;
				SelectedShipAddress = null;
				SelectedCustomer = null;


				var basketViewModel = _serviceProvider.GetRequiredService<ReturnSalesBasketViewModel>();

				foreach (var item in basketViewModel.Items)
				{
					item.Details.Clear();
				}
				basketViewModel.Items.Clear();

				var locationListViewModel = _serviceProvider.GetRequiredService<ReturnSalesBasketLocationListViewModel>();
				locationListViewModel.SelectedItems.Clear();
				locationListViewModel.Items.Clear();
				foreach (var item in Items)
				{
					item.Details.Clear();
					item.Dispatches.Clear();
				}

				Items.Clear();

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
                resultModel.ErrorMessage = result.Message;
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

            var result = await _salesCustomerService.GetObjectsReturnAsync(
                    httpClient: httpClient,
                    firmNumber: _httpClientService.FirmNumber,
                    periodNumber: _httpClientService.PeriodNumber,
              //      warehouseNumber: WarehouseModel.Number,
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

	private async Task ClearFormAsync()
	{
		try
		{
			DocumentNumber = string.Empty;
			SpecialCode = string.Empty;
			DocumentTrackingNumber = string.Empty;
			Description = string.Empty;
			SelectedShipAddress = null;
		}
		catch (Exception ex)
		{
			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}

}