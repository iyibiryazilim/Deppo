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
using Deppo.Mobile.Helpers.TransactionAuditHelpers;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;
using Deppo.Sys.Service.Services;
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
    private readonly ITransactionAuditService _transactionAuditService;
    private readonly IHttpClientSysService _httpClientSysService;
    private readonly ITransactionAuditHelperService _transactionAuditHelperService;

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
    private string cargoTrackingNumber = string.Empty;

    [ObservableProperty]
    private Carrier? selectedCarrier;

    [ObservableProperty]
    private Driver? selectedDriver;

    public InputProductPurchaseOrderProcessFormViewModel(IHttpClientService httpClientService, IUserDialogs userDialogs, IPurchaseDispatchTransactionService purchaseDispatchTransactionService, ICarrierService carrierService, IDriverService driverService, IServiceProvider serviceProvider, ITransactionAuditService transactionAuditService, IHttpClientSysService httpClientSysService, ITransactionAuditHelperService transactionAuditHelperService)
    {
        _httpClientService = httpClientService;
        _userDialogs = userDialogs;
        _purchaseDispatchTransactionService = purchaseDispatchTransactionService;
        _carrierService = carrierService;
        _driverService = driverService;
        _serviceProvider = serviceProvider;
        _transactionAuditService = transactionAuditService;
        _httpClientSysService = httpClientSysService;
        _transactionAuditHelperService = transactionAuditHelperService;

        Items = new();

        Title = "Siparişe Bağlı Mal Kabul İşlemi";

        LoadPageCommand = new Command(async () => await LoadPageAsync());
        ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
        SaveCommand = new Command(async () => await SaveAsync());
		BackCommand = new Command(async () => await BackAsync());

		LoadCarriersCommand = new Command(async () => await LoadCarriersAsync());
        LoadDriversCommand = new Command(async () => await LoadDriversAsync());
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
            if (_userDialogs.IsHudShowing)
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
				double remainingQuantity = item.InputQuantity;

				// Her sipariş döngüsünden önce detay miktarlarının başlangıç değerini tutmak için liste oluşturuyoruz.
				var remainingDetailQuantities = item.Details.ToDictionary(detail => detail, detail => detail.Quantity);

				foreach (var order in item.Orders.OrderBy(x => x.OrderDate))
				{
					if (remainingQuantity <= 0)
						break;

					double orderQuantity = order.WaitingQuantity;
					double allocatedQuantity = Math.Min(remainingQuantity, orderQuantity);

					var transactionLineDto = new PurchaseDispatchTransactionLineDto
					{
						ProductCode = item.ItemCode,
						OrderReferenceId = order.ReferenceId,
						WarehouseNumber = (short)WarehouseModel.Number,
						Quantity = allocatedQuantity,
						ConversionFactor = (remainingQuantity > orderQuantity ? orderQuantity : remainingQuantity) * item.ConversionFactor,
						OtherConversionFactor = (remainingQuantity > orderQuantity ? orderQuantity : remainingQuantity)  * item.OtherConversionFactor,
						SubUnitsetCode = item.SubUnitsetCode,
						SeriLotTransactions = new List<SeriLotTransactionDto>()
					};

					foreach (var detail in item.Details)
					{
						if (allocatedQuantity <= 0)
							break;

						// Detay için kalan miktarı sözlükten alıyoruz.
						double detailRemainingQuantity = remainingDetailQuantities[detail];

						while (detailRemainingQuantity > 0 && allocatedQuantity > 0)
						{
							var transactionDto = new SeriLotTransactionDto
							{
								StockLocationCode = detail.LocationCode,
								SubUnitsetCode = item.SubUnitsetCode,
								ConversionFactor = item.ConversionFactor * Math.Min(detailRemainingQuantity, allocatedQuantity),
								OtherConversionFactor = item.OtherConversionFactor * Math.Min(detailRemainingQuantity, allocatedQuantity),
								DestinationStockLocationCode = string.Empty,
								Quantity = Math.Min(detailRemainingQuantity, allocatedQuantity)
							};

							transactionLineDto.SeriLotTransactions.Add(transactionDto);

							detailRemainingQuantity -= (double)transactionDto.Quantity;
							allocatedQuantity -= (double)transactionDto.Quantity;
						}

						// İşlenen miktar kadar kalan miktarı güncelliyoruz.
						remainingDetailQuantities[detail] = detailRemainingQuantity;
					}

					purchaseDispatchDto.Lines.Add(transactionLineDto);

					remainingQuantity -= (double)transactionLineDto.Quantity;
				}
			}



			//if (remainingQuantity > 0)
			//{
			//    var purchaseDispatchTransactionLineDto = new PurchaseDispatchTransactionLineDto
			//    {
			//        ProductCode = item.ItemCode,
			//        OrderReferenceId = null,
			//        WarehouseNumber = (short)WarehouseModel.Number,
			//        Quantity = remainingQuantity,
			//        ConversionFactor = 1,
			//        OtherConversionFactor = 1,
			//        SubUnitsetCode = item.SubUnitsetCode,
			//        SpeCode = "Sipariş Fazlası",
			//        Description = string.Empty,
			//    };

			//    foreach (var detail in item.Details)
			//    {
			//        var seriLotTransactionDto = new SeriLotTransactionDto
			//        {
			//            StockLocationCode = detail.LocationCode,
			//            SubUnitsetCode = item.SubUnitsetCode,
			//            Quantity = remainingQuantity,
			//            ConversionFactor = 1,
			//            OtherConversionFactor = 1,
			//            DestinationStockLocationCode = string.Empty,
			//        };

			//        purchaseDispatchTransactionLineDto.SeriLotTransactions.Add(seriLotTransactionDto);
			//    }

			//    purchaseDispatchDto.Lines.Add(purchaseDispatchTransactionLineDto);
			//    remainingQuantity -= remainingQuantity;
			//}



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

                try
                {
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
                        currentReferenceId: PurchaseSupplier.ReferenceId,
                        currentCode: PurchaseSupplier.Code,
                        currentName: PurchaseSupplier.Name

                        );
                }
                catch (Exception ex)
                {
                    _userDialogs.Alert(ex.Message, "Hata", "Tamam");
                }

                await ClearDataAsync();
                await ClearFormAsync();

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
                resultModel.ErrorMessage = result.Message;
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
        finally
        {
            IsBusy = false;
        }
    }

	private async Task ClearDataAsync()
	{
		var warehouseListViewModel = _serviceProvider.GetRequiredService<InputProductPurchaseOrderProcessWarehouseListViewModel>();
		var supplierListViewModel = _serviceProvider.GetRequiredService<InputProductPurchaseOrderProcessSupplierListViewModel>();
		var basketViewModel = _serviceProvider.GetRequiredService<InputProductPurchaseOrderProcessBasketListViewModel>();
		var orderViewModel = _serviceProvider.GetRequiredService<InputProductPurchaseOrderProcessOrderListViewModel>();
		var productViewModel = _serviceProvider.GetRequiredService<InputProductPurchaseOrderProcessProductListViewModel>();
        var locationListViewModel = _serviceProvider.GetRequiredService<InputProductPurchaseOrderProcessBasketLocationListViewModel>();

		if (warehouseListViewModel != null)
		{
			warehouseListViewModel.SelectedWarehouseModel.IsSelected = false;
			warehouseListViewModel.SelectedWarehouseModel = null;

		}

        if(supplierListViewModel.SelectedShipAddressModel is not null)
        {
		    supplierListViewModel.SelectedShipAddressModel.IsSelected = false;
		    supplierListViewModel.SelectedShipAddressModel = null;
        }

        orderViewModel.SelectedItems.Clear();
        orderViewModel.BasketItems.Clear();

        productViewModel.SelectedItems.Clear();
        productViewModel.BasketItems.Clear();

        locationListViewModel.SelectedItems.Clear();

		foreach (var item in basketViewModel.Items)
		{
			item.Details.Clear();
			item.Orders.Clear();
		}
		basketViewModel.Items.Clear();
	}

	private async Task ClearFormAsync()
	{
		try
		{
			CargoTrackingNumber = string.Empty;
			DocumentNumber = string.Empty;
			SpecialCode = string.Empty;
			DocumentTrackingNumber = string.Empty;
			Description = string.Empty;
			CargoTrackingNumber = string.Empty;
			SelectedCarrier = null;
			SelectedDriver = null;
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}
}