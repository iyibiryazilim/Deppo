using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.DTOs.TransferTransaction;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.OutsourceModels;
using Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;
using Deppo.Mobile.Core.Models.ShipAddressModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;
using DevExpress.Maui.Controls;

namespace Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcess.OutputOutsourceTransfer.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(Items), queryId: nameof(Items))]
public partial class OutputOutsourceTransferFormViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IOutsourceService _outsourceService;
    private readonly IShipAddressService _shipAddressService;
    private readonly IWarehouseService _warehouseService;
    private readonly ILocationService _locationService;
    private readonly IUserDialogs _userDialogs;
    private readonly ICarrierService _carrierService;
    private readonly IDriverService _driverService;
    private readonly ITransferTransactionService _transferTransactionService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILocationTransactionService _locationTransactionService;

    [ObservableProperty]
    WarehouseModel warehouseModel = null!;

    [ObservableProperty]
    ObservableCollection<OutputOutsourceTransferBasketModel> items = null!;

	ObservableCollection<LocationTransactionModel> LocationTransactions { get; } = new();

	public ObservableCollection<OutsourceModel> Outsources { get; } = new();
    public ObservableCollection<ShipAddressModel> ShipAddresses { get; } = new();
    public ObservableCollection<WarehouseModel> InWarehouses { get; } = new();
    public ObservableCollection<LocationModel> InLocations { get; } = new();
    public ObservableCollection<Carrier> Carriers { get; } = new();
    public ObservableCollection<Driver> Drivers { get; } = new();

    [ObservableProperty]
    OutsourceModel? selectedOutsource;
    [ObservableProperty]
    ShipAddressModel? selectedShipAddress;

    [ObservableProperty]
    Carrier? selectedCarrier;

    [ObservableProperty]
    Driver? selectedDriver;

    [ObservableProperty]
    WarehouseModel selectedInWarehouse = null!;

    [ObservableProperty]
    LocationModel selectedInlocationModel = null!;

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

    [ObservableProperty]
    WarehouseModel inWarehouse = null!;

    public OutputOutsourceTransferFormViewModel(IHttpClientService httpClientService,
        IShipAddressService shipAddressService,
        IWarehouseService warehouseService,
        ILocationService locationService,
        IUserDialogs userDialogs,
        IOutsourceService outsourceService,
        ICarrierService carrierService,
        IDriverService driverService,
        ITransferTransactionService transferTransactionService,
        IServiceProvider serviceProvider,
        ILocationTransactionService locationTransactionService)
    {
        _httpClientService = httpClientService;
        _shipAddressService = shipAddressService;
        _warehouseService = warehouseService;
        _locationService = locationService;
        _userDialogs = userDialogs;
        _outsourceService = outsourceService;
        _carrierService = carrierService;
        _driverService = driverService;
        _transferTransactionService = transferTransactionService;
        _serviceProvider = serviceProvider;
        _locationTransactionService = locationTransactionService;

        Title = "Fason Çıkış Transfer İşlemi";

        LoadPageCommand = new Command(async () => await LoadPageAsync());
        SaveCommand = new Command(async () => await SaveAsync());
        ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
        BackCommand = new Command(async () => await BackAsync());

        LoadOutsourcesCommand = new Command(async () => await LoadOutsourcesAsync());
        LoadShipAddressesCommand = new Command<OutsourceModel>(async (x) => await LoadShipAddressesAsync(x));
        LoadCarriersCommand = new Command(async () => await LoadCarriersAsync());
        LoadDriversCommand = new Command(async () => await LoadDriversAsync());
        LoadInWarehousesCommand = new Command(async () => await LoadInWarehousesAsync());
        LoadInLocationsCommand = new Command(async () => await LoadInLocationsAsync());
    }

    public Page CurrentPage { get; set; } = null!;

    public Command LoadPageCommand { get; }
    public Command BackCommand { get; }
    public Command SaveCommand { get; }
    public Command ShowBasketItemCommand { get; }
    public Command LoadOutsourcesCommand { get; }
    public Command<OutsourceModel> LoadShipAddressesCommand { get; }
    public Command LoadCarriersCommand { get; }
    public Command LoadDriversCommand { get; }
    public Command LoadInWarehousesCommand { get; }
    public Command LoadInLocationsCommand { get; }

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

    private async Task LoadOutsourcesAsync()
    {
        if (IsBusy)
            return;
        try
        {
            Outsources.Clear();
            SelectedOutsource = null;

            IsBusy = true;

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var result = await _outsourceService.GetObjects(
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
                    Outsources.Add(Mapping.Mapper.Map<OutsourceModel>(item));
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

    private async Task LoadShipAddressesAsync(OutsourceModel salesCustomer)
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
	public async Task LoadLocationTransaction(OutputOutsourceTransferBasketModel outputOutsourceTransferBasketModel, OutputOutsourceTransferBasketDetailModel outputOutsourceTransferBasketDetailModel)
	{
		try
		{

            LocationTransactions.Clear();
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var result = await _locationTransactionService.GetInputObjectsAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: outputOutsourceTransferBasketModel.IsVariant ? outputOutsourceTransferBasketModel.MainItemReferenceId : outputOutsourceTransferBasketModel.ItemReferenceId,
				variantReferenceId: outputOutsourceTransferBasketModel.IsVariant ? outputOutsourceTransferBasketModel.ItemReferenceId : 0,
				warehouseNumber: WarehouseModel.Number,
				locationRef: outputOutsourceTransferBasketDetailModel.LocationReferenceId,
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
            _userDialogs.ShowLoading("İşlem Tamamlanıyor...");
            await Task.Delay(1000);

            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var transferTransactionInsertDto = new TransferTransactionInsert
            {
                SpeCode = SpecialCode,
                CurrentCode = SelectedOutsource?.Code ?? string.Empty,
                CarrierCode = SelectedCarrier?.Code ?? string.Empty,
                DriverFirstName = SelectedDriver?.Name ?? string.Empty,
                DriverLastName = SelectedDriver?.Surname ?? string.Empty,
                Plaque = SelectedDriver?.PlateNumber ?? string.Empty,
                ShipInfoCode = SelectedShipAddress?.Code ?? string.Empty,
                IdentityNumber = SelectedDriver?.IdentityNumber ?? string.Empty,
                IsEDispatch = (bool)SelectedOutsource?.IsEDispatch ? 1 : 0,
                Code = string.Empty,
                DocTrackingNumber = DocumentTrackingNumber,
                DoCode = DocumentNumber,
                TransactionDate = TransactionDate,
                FirmNumber = _httpClientService.FirmNumber,
                WarehouseNumber = WarehouseModel.Number,
                DestinationWarehouseNumber = SelectedInWarehouse.Number,
                Description = Description,
            };

            foreach (var item in Items)
            {
                var tempItemQuantity = item.Quantity;
                var transferTransactionLineDto = new TransferTransactionLineDto
                {
					ProductCode = item.IsVariant ? item.MainItemCode : item.ItemCode,
					VariantCode = item.IsVariant ? item.ItemCode : "",
					WarehouseNumber = WarehouseModel.Number,
                    DestinationWarehouseNumber = SelectedInWarehouse.Number,
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
                        var tempLocationRemainingQuantity = locationTransaction.RemainingQuantity;
						while (tempLocationRemainingQuantity > 0 && tempItemQuantity > 0 && tempDetailQuantity > 0) {
							var serilotTransactionDto = new SeriLotTransactionDto
							{
								StockLocationCode = detail.LocationCode,
								InProductTransactionLineReferenceId = locationTransaction.TransactionReferenceId,
								OutProductTransactionLineReferenceId = locationTransaction.ReferenceId,
								Quantity = tempDetailQuantity > tempLocationRemainingQuantity ? tempLocationRemainingQuantity : tempDetailQuantity,
								SubUnitsetCode = item.SubUnitsetCode,
								DestinationStockLocationCode = SelectedInlocationModel.Code,
								ConversionFactor = 1,
								OtherConversionFactor = 1,
							};
                            tempLocationRemainingQuantity -= (double)serilotTransactionDto.Quantity;
							tempDetailQuantity -= (double)serilotTransactionDto.Quantity;
							transferTransactionLineDto.SeriLotTransactions.Add(serilotTransactionDto);
                            tempItemQuantity -= (double)serilotTransactionDto.Quantity;
						}
					}
                }

                transferTransactionInsertDto.Lines.Add(transferTransactionLineDto);
            }

            var result = await _transferTransactionService.InsertTransferTransaction(httpClient, transferTransactionInsertDto, _httpClientService.FirmNumber);
            Console.WriteLine(result);
            ResultModel resultModel = new();

            if (result.IsSuccess)
            {
                resultModel.Message = "Başarılı";
                resultModel.Code = result.Data.Code;
                resultModel.PageTitle = "Fason Çıkış Transferi";
                resultModel.PageCountToBack = 4;

                var warehouseListViewModel = _serviceProvider.GetRequiredService<OutputOutsourceTransferWarehouseListViewModel>();
                var productListViewModel = _serviceProvider.GetRequiredService<OutputOutsourceTransferProductListViewModel>();
                var basketViewModel = _serviceProvider.GetRequiredService<OutputOutsourceTransferBasketListViewModel>();

                await Task.WhenAll(ClearFormAsync(), warehouseListViewModel?.ClearPageAsync(), productListViewModel?.ClearPageAsync(), basketViewModel?.ClearPageAsync());
              

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
                resultModel.PageTitle = "Fason Çıkış Transferi";
                resultModel.PageCountToBack = 1;
                resultModel.ErrorMessage = result.Message;

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
            SelectedOutsource = null;
            SelectedShipAddress = null;
            SelectedInWarehouse = null;
            SelectedInlocationModel = null;

        }
        catch (Exception ex)
        {

            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
        }
    }

    private async Task LoadInWarehousesAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;

            _userDialogs.ShowLoading("Loading...");
            InWarehouses.Clear();
            SelectedInWarehouse = null!;

            await Task.Delay(500);
            var httpClient = _httpClientService.GetOrCreateHttpClient();
            var result = await _warehouseService.GetObjects(httpClient, string.Empty, null, 0, 20, _httpClientService.FirmNumber);
            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                        InWarehouses.Add(new WarehouseModel
                        {
                            ReferenceId = item.ReferenceId,
                            Name = item.Name,
                            Number = item.Number,
                            City = item.City,
                            Country = item.Country,
                            IsSelected = false
                        });
                }
            }

            _userDialogs.HideHud();
        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadInLocationsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            _userDialogs.ShowLoading("Loading...");
            InLocations.Clear();
            SelectedInlocationModel = null!;

            await Task.Delay(500);
            var httpClient = _httpClientService.GetOrCreateHttpClient();

            var result = await _locationService.GetObjects(httpClient,
                            firmNumber: _httpClientService.FirmNumber,
                            periodNumber: _httpClientService.PeriodNumber,
                            warehouseNumber: SelectedInWarehouse.Number,
                            productReferenceId: 0,
                            search: string.Empty,
                            skip: 0,
                            take: 20);

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    foreach (var item in result.Data)
                        InLocations.Add(new LocationModel
                        {
                            ReferenceId = item.ReferenceId,
                            Name = item.Name,
                            Code = item.Code,
                            WarehouseReferenceId = item.WarehouseReferenceId,
                            WarehouseNumber = item.WarehouseNumber,
                            WarehouseName = item.WarehouseName,
                            IsSelected = false
                        });
                }
            }

            _userDialogs.HideHud();

        }
        catch (Exception ex)
        {
            if (_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            _userDialogs.Alert(ex.Message);
        }
        finally
        {
            IsBusy = false;
        }

    }

}
