using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.PurchaseDispatchTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.PurchaseModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;
using DevExpress.Data.Async.Helpers;
using DevExpress.Maui.Controls;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Deppo.Mobile.Core.Helpers.DeppoEnums;

namespace Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(PurchaseSupplier), queryId: nameof(PurchaseSupplier))]
[QueryProperty(name: nameof(Items), queryId: nameof(Items))]
public partial class InputProductPurchaseOrderProcessFormViewModel : BaseViewModel
{
    private readonly IHttpClientService _httpClientService;
    private readonly IPurchaseDispatchTransactionService _purchaseDispatchTransactionService;
    private readonly ICarrierService _carrierService;
    private readonly IDriverService _driverService;
    private readonly IUserDialogs _userDialogs;
    private readonly IServiceProvider _serviceProvider;

    [ObservableProperty]
    private WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	private PurchaseSupplier purchaseSupplier;

	[ObservableProperty]
	private ObservableCollection<InputPurchaseBasketModel> items = null!;

	public ObservableCollection<Carrier> Carriers { get; } = new();
	public ObservableCollection<Driver> Drivers { get; } = new();


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
	string cargoTrackingNumber = string.Empty;

	

	[ObservableProperty]
	Carrier? selectedCarrier;
	[ObservableProperty]
	Driver? selectedDriver;

	public InputProductPurchaseOrderProcessFormViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IPurchaseDispatchTransactionService purchaseDispatchTransactionService, ICarrierService carrierService, IDriverService driverService, IServiceProvider serviceProvider)
	{
		_httpClientService = httpClientService;
		_userDialogs = userDialogs;
		_purchaseDispatchTransactionService = purchaseDispatchTransactionService;
		_carrierService = carrierService;
		_driverService = driverService;
		Items = new();

		Title = "Siparişe Bağlı Mal Kabul İşlemi";

		LoadPageCommand = new Command(async () => await LoadPageAsync());
		ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
		SaveCommand = new Command(async () => await SaveAsync());

		LoadCarriersCommand = new Command(async () => await LoadCarriersAsync());
		LoadDriversCommand = new Command(async () => await LoadDriversAsync());
		_serviceProvider = serviceProvider;
	}

	public Page CurrentPage { get; set; }

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
            if(_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
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
            IsBusy = true;

            Carriers.Clear();
            SelectedCarrier = null;

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
            if(_userDialogs.IsHudShowing)
                _userDialogs.HideHud();

            await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
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
                Code = string.Empty,
                CurrentCode = PurchaseSupplier?.Code,
                FirmNumber = _httpClientService.FirmNumber,
                WarehouseNumber = WarehouseModel.Number,
                CarrierCode = SelectedCarrier != null ? SelectedCarrier.Code : "",
                DriverFirstName = SelectedDriver != null ? SelectedDriver.Name : "",
                DriverLastName = SelectedDriver != null ? SelectedDriver.Surname : "",
                Plaque = SelectedDriver != null ? SelectedDriver.PlateNumber : "",
                ShipInfoCode = PurchaseSupplier?.ShipAddressReferenceId != 0 ? PurchaseSupplier?.ShipAddressCode : "",
                SpeCode = SpecialCode,
                DocTrackingNumber = DocumentTrackingNumber,
                DoCode = DocumentNumber,
                TransactionDate = FicheDate,
                Description = Description,
                IsEDispatch = (short?)((bool)PurchaseSupplier?.IsEDispatch ? 1 : 0),
                DispatchType = (short?)((bool)PurchaseSupplier?.IsEDispatch ? 1 : 0),
                DispatchStatus = 1,
                EDispatchProfileId = (short?)((bool)PurchaseSupplier?.IsEDispatch ? 1 : 0),
            };

            foreach (var item in Items.Where(x => x.Orders.Count > 0))
            {
                var remainingQuantity = item.InputQuantity;
                foreach (var orders in item.Orders.OrderBy(x => x.OrderDate))
                {
                    if (remainingQuantity > 0)
                    {
                        var purchaseDispatchTransactionLineDto = new PurchaseDispatchTransactionLineDto
                        {
                            ProductCode = item.ItemCode,
                            OrderReferenceId = orders.OrderReferenceId,
                            WarehouseNumber = (short)WarehouseModel.Number,
                            Quantity = remainingQuantity > orders.WaitingQuantity ? orders.WaitingQuantity : remainingQuantity,
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
                                Quantity = Convert.ToDouble(remainingQuantity > orders.WaitingQuantity ? orders.WaitingQuantity : remainingQuantity),
                                ConversionFactor = 1,
                                OtherConversionFactor = 1,
                                DestinationStockLocationCode = string.Empty,
                            };

                            purchaseDispatchTransactionLineDto.SeriLotTransactions.Add(seriLotTransactionDto);
                        }

                        purchaseDispatchDto.Lines.Add(purchaseDispatchTransactionLineDto);

                        remainingQuantity -= Convert.ToDouble(remainingQuantity > orders.WaitingQuantity ? orders.WaitingQuantity : remainingQuantity);
                    }
                    else
                    {
                        break;
                    }
                }

                if (remainingQuantity > 0)
                {
                    var purchaseDispatchTransactionLineDto = new PurchaseDispatchTransactionLineDto
                    {
                        ProductCode = item.ItemCode,
                        OrderReferenceId = null,
                        WarehouseNumber = (short)WarehouseModel.Number,
                        Quantity = remainingQuantity,
                        ConversionFactor = 1,
                        OtherConversionFactor = 1,
                        SubUnitsetCode = item.SubUnitsetCode,
                        SpeCode = "Sipariş Fazlası",
                        Description = string.Empty,
                    };

                    foreach (var detail in item.Details)
                    {
                        var seriLotTransactionDto = new SeriLotTransactionDto
                        {
                            StockLocationCode = detail.LocationCode,
                            SubUnitsetCode = item.SubUnitsetCode,
                            Quantity = remainingQuantity,
                            ConversionFactor = 1,
                            OtherConversionFactor = 1,
                            DestinationStockLocationCode = string.Empty,
                        };

                        purchaseDispatchTransactionLineDto.SeriLotTransactions.Add(seriLotTransactionDto);
                    }

                    purchaseDispatchDto.Lines.Add(purchaseDispatchTransactionLineDto);
                    remainingQuantity -= remainingQuantity;
                }
            }

            foreach (var item in Items.Where(x => x.Orders.Count == 0))
            {
                var purchaseDispatchTransactionLineDto = new PurchaseDispatchTransactionLineDto
                {
                    ProductCode = item.ItemCode,
                    OrderReferenceId = item.Orders[0].OrderReferenceId,
                    WarehouseNumber = (short)WarehouseModel.Number,
                    Quantity = item.InputQuantity,
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
                        Quantity = item.InputQuantity,
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

				var basketViewModel = _serviceProvider.GetRequiredService<InputProductPurchaseOrderProcessBasketListViewModel>();
                foreach (var item in basketViewModel?.Items)
				{
					item.Details.Clear();
				}
				basketViewModel.Items.Clear();

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
            _userDialogs.Alert(ex.Message, "Hata", "Tamam");
        }
        finally { 
            IsBusy = false; 
        }
    }
}