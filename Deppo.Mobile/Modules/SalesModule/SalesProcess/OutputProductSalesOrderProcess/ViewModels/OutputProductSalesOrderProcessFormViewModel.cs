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
using Deppo.Sys.Service.Services;
using DevExpress.Maui.Controls;
using DevExpress.Maui.Core.Internal;
using System.Collections.ObjectModel;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;

[QueryProperty(name: nameof(WarehouseModel), queryId: nameof(WarehouseModel))]
[QueryProperty(name: nameof(SalesCustomer), queryId: nameof(SalesCustomer))]
[QueryProperty(name: nameof(Items), queryId: nameof(Items))]
public partial class OutputProductSalesOrderProcessFormViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly ICarrierService _carrierService;
	private readonly IDriverService _driverService;
	private readonly IRetailSalesDispatchTransactionService _retailSalesDispatchTransactionService;
	private readonly IWholeSalesDispatchTransactionService _wholeSalesDispatchTransactionService;
	private readonly IServiceProvider _serviceProvider;
	private readonly IUserDialogs _userDialogs;
	private readonly ILocationTransactionService _locationTransactionService;
	private readonly ITransactionAuditService _transactionAuditService;
	private readonly IHttpClientSysService _httpClientSysService;
	private readonly ITransactionAuditHelperService _transactionAuditHelperService;

	[ObservableProperty]
	private WarehouseModel warehouseModel = null!;

	[ObservableProperty]
	private SalesCustomer salesCustomer = null!;

	[ObservableProperty]
	private ObservableCollection<OutputSalesBasketModel> items = null!;

	[ObservableProperty]
	private ObservableCollection<OutputSalesBasketModel> cacheItems = new();


	private ObservableCollection<LocationTransactionModel> LocationTransactions { get; } = new();

	public ObservableCollection<Carrier> Carriers { get; } = new();
	public ObservableCollection<Driver> Drivers { get; } = new();

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

	public OutputProductSalesOrderProcessFormViewModel(IHttpClientService httpClientService, ICarrierService carrierService, IDriverService driverService, IUserDialogs userDialogs, IRetailSalesDispatchTransactionService retailSalesDispatchTransactionService, IWholeSalesDispatchTransactionService wholeSalesDispatchTransactionService, IServiceProvider serviceProvider, ILocationTransactionService locationTransactionService, ITransactionAuditService transactionAuditService, IHttpClientSysService httpClientSysService, ITransactionAuditHelperService transactionAuditHelperService)
	{
		_httpClientService = httpClientService;

		_carrierService = carrierService;
		_driverService = driverService;
		_userDialogs = userDialogs;
		_retailSalesDispatchTransactionService = retailSalesDispatchTransactionService;
		_wholeSalesDispatchTransactionService = wholeSalesDispatchTransactionService;
		_serviceProvider = serviceProvider;
		_locationTransactionService = locationTransactionService;
		_transactionAuditHelperService = transactionAuditHelperService;
		_httpClientSysService = httpClientSysService;
		_transactionAuditService = transactionAuditService;

		Title = "Sevk İşlemi";

		LoadPageCommand = new Command(async () => await LoadPageAsync());
		ShowBasketItemCommand = new Command(async () => await ShowBasketItemAsync());
		SaveCommand = new Command(async () => await SaveAsync());
		SelectWholeCommand = new Command(async () => await SelectWholeAsync());
		SelectRetailCommand = new Command(async () => await SelectRetailAsync());
		BackCommand = new Command(async () => await BackAsync());

		LoadCarriersCommand = new Command(async () => await LoadCarriersAsync());
		LoadDriversCommand = new Command(async () => await LoadDriversAsync());
	}

	public Page CurrentPage { get; set; } = null!;

	public Command LoadPageCommand { get; }
	public Command BackCommand { get; }
	public Command SaveCommand { get; }
	public Command ShowBasketItemCommand { get; }

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

	private async Task SaveAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			if (SalesCustomer == null)
			{
				_userDialogs.Alert("Müşteri alanı zorunlu", "Uyarı", "Tamam");
				return;
			}
			if (SalesCustomer.IsEDispatch)
			{
				if (SelectedCarrier == null)
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

			var confirmInsert = await _userDialogs.ConfirmAsync("Siparişe Bağlı Toptan Satış İrsaliyesi oluşturulacaktır. Devam etmek istediğinize emin misiniz?", "Onay", "Evet", "Hayır");
			if (!confirmInsert)
			{
				await CloseInsertOptionsAsync();
				return;
			}
			await CloseInsertOptionsAsync();

			var httpClient = _httpClientService.GetOrCreateHttpClient();
			await WholeSalesDispatchTransactionInsertAsync(httpClient);
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

			var confirmInsert = await _userDialogs.ConfirmAsync("Siparişe Bağlı Perakende Satış İrsaliyesi oluşturulacaktır. Devam etmek istediğinize emin misiniz?", "Onay", "Evet", "Hayır");
			if (!confirmInsert)
			{
				await CloseInsertOptionsAsync();
				return;
			}

			await CloseInsertOptionsAsync();

			var httpClient = _httpClientService.GetOrCreateHttpClient();

			await RetailSalesDispatchTransactionInsertAsync(httpClient);

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

	private async Task RetailSalesDispatchTransactionInsertAsync(HttpClient httpClient)
	{
		_userDialogs.Loading("İşlem tamamlanıyor...");

		CacheItems = Items;

		var dto = await CreateRetailDto();

		var result = await _retailSalesDispatchTransactionService.InsertRetailSalesDispatchTransaction(httpClient, _httpClientService.FirmNumber, dto);

		ResultModel resultModel = new();
		if (result.IsSuccess)
		{
			resultModel.Message = "Başarılı";
			resultModel.Code = result.Data.Code;
			resultModel.PageTitle = Title;
			resultModel.PageCountToBack = 6;

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
				currentReferenceId: SalesCustomer.ReferenceId,
				currentCode: SalesCustomer.Code,
				currentName: SalesCustomer.Name,
				shipAddressReferenceId: SalesCustomer.ShipAddressReferenceId,
				shipAddressCode: SalesCustomer.ShipAddressCode,
				shipAddressName: SalesCustomer.ShipAddressName

			   );
			}
			catch (Exception ex)
			{
				if (_userDialogs.IsHudShowing)
					_userDialogs.HideHud();

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

			LocationTransactions.Clear();
		}


	}
	private async Task<RetailSalesDispatchTransactionInsert> CreateRetailDto()
	{
		var dto = new RetailSalesDispatchTransactionInsert
		{
			Code = "",
			CurrentCode = SalesCustomer != null ? SalesCustomer.Code : "",
			DriverFirstName = SelectedDriver != null ? SelectedDriver.Name : "",
			ShipInfoCode = SalesCustomer?.ShipAddressReferenceId != 0 ? SalesCustomer?.ShipAddressCode : "",
			DriverLastName = SelectedDriver != null ? SelectedDriver.Surname : "",
			CarrierCode = SelectedCarrier != null ? SelectedCarrier.Code : "",
			IdentityNumber = SelectedDriver != null ? SelectedDriver.IdentityNumber : "",
			Plaque = SelectedDriver != null ? SelectedDriver.PlateNumber : "",
			IsEDispatch = (short?)((bool)SalesCustomer?.IsEDispatch ? 1 : 0),
			DispatchType = (short?)((bool)SalesCustomer?.IsEDispatch ? 1 : 0),
			DispatchStatus = 1,
			EDispatchProfileId = (short?)((bool)SalesCustomer?.IsEDispatch ? 1 : 0),
			Description = Description,
			DoCode = DocumentNumber,
			DocTrackingNumber = DocumentTrackingNumber,
			TransactionDate = TransactionDate,
			FirmNumber = _httpClientService.FirmNumber,
			SpeCode = SpecialCode,
			WarehouseNumber = WarehouseModel.Number,
		};

		dto.Lines = await GetRetailLines();

		return dto;
	}



	private async Task<List<RetailSalesDispatchTransactionLineInsert>> GetRetailLines()
	{
		List<RetailSalesDispatchTransactionLineInsert> retailSalesDispatchTransactionLineInserts = new();

		foreach (var item in CacheItems)
		{
			await GetLocationTransaction(item);
			var itemQuantity = item.OutputQuantity;

			foreach (var order in item.Orders.OrderBy(x => x.OrderDate))
			{

				if (itemQuantity == 0)
					break;

				var lineDto = new RetailSalesDispatchTransactionLineInsert
				{
					Quantity = itemQuantity > order.WaitingQuantity ? order.WaitingQuantity : itemQuantity,
					ProductCode = item.ItemCode,
					WarehouseNumber = (short?)WarehouseModel.Number,
					OrderReferenceId = order.ReferenceId,
					UnitPrice = order.Price,
					VatRate = order.Vat,
					ConversionFactor = itemQuantity > order.WaitingQuantity ? order.WaitingQuantity : itemQuantity,
					OtherConversionFactor = itemQuantity > order.WaitingQuantity ? order.WaitingQuantity : itemQuantity,
					SubUnitsetCode = item.SubUnitsetCode,
				};

				lineDto.SeriLotTransactions = await GetRetailSeriLotTransaction(lineDto,item);
				retailSalesDispatchTransactionLineInserts.Add(lineDto);
				itemQuantity -= (double)lineDto.Quantity;
			}


			LocationTransactions.Clear();
		}

		return retailSalesDispatchTransactionLineInserts;
	}

	private async Task<List<SeriLotTransactionDto>> GetRetailSeriLotTransaction(RetailSalesDispatchTransactionLineInsert line,OutputSalesBasketModel item)
	{
		List<SeriLotTransactionDto> retailSalesDispatchTransactionSerilots = new();

		// Orijinal detay miktarlarını korumak için kopya oluştur
		var originalQuantities = item.Details.ToDictionary(x => x.LocationReferenceId, x => x.Quantity);
		var tempSerilotQuantities = new Dictionary<int, double>(originalQuantities);

		double detailQuantity = (double)line.Quantity;

		foreach (var locationTransaction in LocationTransactions.OrderBy(x => x.TransactionDate).Where(x => x.RemainingQuantity > 0))
		{
			var serilot = item.Details.FirstOrDefault(x => x.LocationReferenceId == locationTransaction.LocationReferenceId);

			if (serilot == null || !tempSerilotQuantities.ContainsKey(locationTransaction.LocationReferenceId) || tempSerilotQuantities[locationTransaction.LocationReferenceId] <= 0)
				continue;

			var serilotQuantity = tempSerilotQuantities[locationTransaction.LocationReferenceId];
			double locationRemaningQuantity = locationTransaction.RemainingQuantity;

			while (locationRemaningQuantity > 0 && detailQuantity > 0 && serilotQuantity > 0)
			{
				var transactionQuantity = Math.Min(Math.Min(locationRemaningQuantity, detailQuantity), serilotQuantity);

				var serilotTransactionDto = new SeriLotTransactionDto
				{
					StockLocationCode = locationTransaction.LocationCode,
					InProductTransactionLineReferenceId = locationTransaction.TransactionReferenceId,
					OutProductTransactionLineReferenceId = locationTransaction.ReferenceId,
					Quantity = transactionQuantity,
					SubUnitsetCode = line.SubUnitsetCode,
					DestinationStockLocationCode = string.Empty,
					ConversionFactor = transactionQuantity,
					OtherConversionFactor = transactionQuantity,
				};

				locationRemaningQuantity -= transactionQuantity;
				detailQuantity -= transactionQuantity;
				serilotQuantity -= transactionQuantity;
				locationTransaction.RemainingQuantity -= transactionQuantity;
				tempSerilotQuantities[locationTransaction.LocationReferenceId] = serilotQuantity;
				retailSalesDispatchTransactionSerilots.Add(serilotTransactionDto);
			}
		}

		// Orijinal miktarları değiştirmiyoruz
		return retailSalesDispatchTransactionSerilots;


	}
	private async Task WholeSalesDispatchTransactionInsertAsync(HttpClient httpClient)
	{
		_userDialogs.Loading("İşlem tamamlanıyor...");

		CacheItems = Items;

		var dto = await CreateDto();

		var result = await _wholeSalesDispatchTransactionService.InsertWholeSalesDispatchTransaction(httpClient, _httpClientService.FirmNumber, dto);

		ResultModel resultModel = new();
		if (result.IsSuccess)
		{
			resultModel.Message = "Başarılı";
			resultModel.Code = result.Data.Code;
			resultModel.PageTitle = Title;
			resultModel.PageCountToBack = 6;

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
				currentReferenceId: SalesCustomer.ReferenceId,
				currentCode: SalesCustomer.Code,
				currentName: SalesCustomer.Name,
				shipAddressReferenceId: SalesCustomer.ShipAddressReferenceId,
				shipAddressCode: SalesCustomer.ShipAddressCode,
				shipAddressName: SalesCustomer.ShipAddressName

			   );
			}
			catch (Exception ex)
			{
				if (_userDialogs.IsHudShowing)
					_userDialogs.HideHud();

				await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
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

			LocationTransactions.Clear();
		}


	}

	private async Task<WholeSalesDispatchTransactionInsert> CreateDto()
	{
		var dto = new WholeSalesDispatchTransactionInsert
		{
			Code = "",
			CurrentCode = SalesCustomer != null ? SalesCustomer.Code : "",
			DriverFirstName = SelectedDriver != null ? SelectedDriver.Name : "",
			ShipInfoCode = SalesCustomer?.ShipAddressReferenceId != 0 ? SalesCustomer?.ShipAddressCode : "",
			DriverLastName = SelectedDriver != null ? SelectedDriver.Surname : "",
			CarrierCode = SelectedCarrier != null ? SelectedCarrier.Code : "",
			IdentityNumber = SelectedDriver != null ? SelectedDriver.IdentityNumber : "",
			Plaque = SelectedDriver != null ? SelectedDriver.PlateNumber : "",
			IsEDispatch = (short?)((bool)SalesCustomer?.IsEDispatch ? 1 : 0),
			DispatchType = (short?)((bool)SalesCustomer?.IsEDispatch ? 1 : 0),
			DispatchStatus = 1,
			EDispatchProfileId = (short?)((bool)SalesCustomer?.IsEDispatch ? 1 : 0),
			Description = Description,
			DoCode = DocumentNumber,
			DocTrackingNumber = DocumentTrackingNumber,
			TransactionDate = TransactionDate,
			FirmNumber = _httpClientService.FirmNumber,
			SpeCode = SpecialCode,
			WarehouseNumber = WarehouseModel.Number,
		};

		dto.Lines = await GetLines();

		return dto;
	}

	private async Task<List<WholeSalesDispatchTransactionLineInsert>> GetLines()
	{
		List<WholeSalesDispatchTransactionLineInsert> wholeSalesDispatchTransactionLineInserts = new();

		foreach (var item in CacheItems)
		{
			await GetLocationTransaction(item);
			var itemQuantity = item.OutputQuantity;

			foreach (var order in item.Orders.OrderBy(x => x.OrderDate))
			{

				if (itemQuantity == 0)
					break;

				var lineDto = new WholeSalesDispatchTransactionLineInsert
				{
					Quantity = itemQuantity > order.WaitingQuantity ? order.WaitingQuantity : itemQuantity,
					ProductCode = item.ItemCode,
					WarehouseNumber = (short?)WarehouseModel.Number,
					OrderReferenceId = order.ReferenceId,
					UnitPrice = order.Price,
					VatRate = order.Vat,
					ConversionFactor = itemQuantity > order.WaitingQuantity ? order.WaitingQuantity : itemQuantity,
					OtherConversionFactor = itemQuantity > order.WaitingQuantity ? order.WaitingQuantity : itemQuantity,
					SubUnitsetCode = item.SubUnitsetCode,
				};

				lineDto.SeriLotTransactions = await GetSeriLotTransaction(lineDto, item);
				wholeSalesDispatchTransactionLineInserts.Add(lineDto);
				itemQuantity -= (double)lineDto.Quantity;
			}


			LocationTransactions.Clear();
		}

		return wholeSalesDispatchTransactionLineInserts;
	}

	private async Task<List<SeriLotTransactionDto>> GetSeriLotTransaction(
	WholeSalesDispatchTransactionLineInsert line,
	OutputSalesBasketModel item)
	{
		List<SeriLotTransactionDto> wholeSalesDispatchTransactionSerilots = new();

		// Orijinal detay miktarlarını korumak için kopya oluştur
		var originalQuantities = item.Details.ToDictionary(x => x.LocationReferenceId, x => x.Quantity);
		var tempSerilotQuantities = new Dictionary<int, double>(originalQuantities);

		double detailQuantity = (double)line.Quantity;

		foreach (var locationTransaction in LocationTransactions.OrderBy(x => x.TransactionDate).Where(x => x.RemainingQuantity > 0))
		{
			var serilot = item.Details.FirstOrDefault(x => x.LocationReferenceId == locationTransaction.LocationReferenceId);

			if (serilot == null || !tempSerilotQuantities.ContainsKey(locationTransaction.LocationReferenceId) || tempSerilotQuantities[locationTransaction.LocationReferenceId] <= 0)
				continue;

			var serilotQuantity = tempSerilotQuantities[locationTransaction.LocationReferenceId];
			double locationRemaningQuantity = locationTransaction.RemainingQuantity;

			while (locationRemaningQuantity > 0 && detailQuantity > 0 && serilotQuantity > 0)
			{
				var transactionQuantity = Math.Min(Math.Min(locationRemaningQuantity, detailQuantity), serilotQuantity);

				var serilotTransactionDto = new SeriLotTransactionDto
				{
					StockLocationCode = locationTransaction.LocationCode,
					InProductTransactionLineReferenceId = locationTransaction.TransactionReferenceId,
					OutProductTransactionLineReferenceId = locationTransaction.ReferenceId,
					Quantity = transactionQuantity,
					SubUnitsetCode = line.SubUnitsetCode,
					DestinationStockLocationCode = string.Empty,
					ConversionFactor = transactionQuantity,
					OtherConversionFactor = transactionQuantity,
				};

				locationRemaningQuantity -= transactionQuantity;
				detailQuantity -= transactionQuantity;
				serilotQuantity -= transactionQuantity;
				locationTransaction.RemainingQuantity -= transactionQuantity;
				tempSerilotQuantities[locationTransaction.LocationReferenceId] = serilotQuantity;
				wholeSalesDispatchTransactionSerilots.Add(serilotTransactionDto);
			}
		}

		// Orijinal miktarları değiştirmiyoruz
		return wholeSalesDispatchTransactionSerilots;
	}




	private async Task GetLocationTransaction(OutputSalesBasketModel item)
	{

		foreach (var detail in item.Details)
		{
			await LoadLocationTransaction(item, detail);
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

	private async Task ClearDataAsync()
	{
		var warehouseListViewModel = _serviceProvider.GetRequiredService<OutputProductSalesOrderProcessWarehouseListViewModel>();
		var customerListViewModel = _serviceProvider.GetRequiredService<OutputProductSalesOrderProcessCustomerListViewModel>();
		var basketViewModel = _serviceProvider.GetRequiredService<OutputProductSalesOrderProcessBasketListViewModel>();
		var orderViewModel = _serviceProvider.GetRequiredService<OutputProductSalesOrderProcessOrderListViewModel>();
		var productViewModel = _serviceProvider.GetRequiredService<OutputProductSalesOrderProcessProductListViewModel>();


		if (warehouseListViewModel.SelectedWarehouseModel != null)
		{
			warehouseListViewModel.SelectedWarehouseModel.IsSelected = false;
			warehouseListViewModel.SelectedWarehouseModel = null;

		}
		if(customerListViewModel.SelectedShipAddressModel != null)
		{
			customerListViewModel.SelectedShipAddressModel.IsSelected = false;
			customerListViewModel.SelectedShipAddressModel = null;
		}

		foreach (var item in basketViewModel.Items)
		{
			item.Details.Clear();
			item.Orders.Clear();
		}
		basketViewModel.Items.Clear();

		basketViewModel.SelectedLocationTransactionItems.Clear();
		basketViewModel.SelectedLocationTransactions.Clear();
		basketViewModel.SelectedSeriLotTransactions.Clear();
		orderViewModel.BasketItems.Clear();
		productViewModel.BasketItems.Clear();
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
			LocationTransactions.Clear();
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
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
}