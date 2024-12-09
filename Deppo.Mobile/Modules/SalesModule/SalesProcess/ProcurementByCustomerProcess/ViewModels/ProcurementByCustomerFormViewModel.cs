using AndroidX.Camera.Video;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.DTOs.SalesDispatchTransaction;
using Deppo.Core.DTOs.SeriLotTransactionDto;
using Deppo.Core.DTOs.TransferTransaction;
using Deppo.Core.Models;
using Deppo.Core.ResponseResultModels;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.LocationModels;
using Deppo.Mobile.Core.Models.ProcurementModels.ByCustomerModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.TransferModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.ResultModule;
using Deppo.Mobile.Modules.ResultModule.Views;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByProductProcess.ViewModels;
using Deppo.Sys.Service.DTOs;
using Deppo.Sys.Service.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DevExpress.Data.Filtering.Helpers.SubExprHelper.UiThreadRowStubSubExpressive;
using System.Net.Http;
using Android.Net.Wifi.Rtt;
using Microsoft.Maui.Controls.Shapes;
using Deppo.Core.DataResultModel;
using DevExpress.Maui.Controls;
using Xamarin.Google.ErrorProne.Annotations;

namespace Deppo.Mobile.Modules.SalesModule.SalesProcess.ProcurementByCustomerProcess.ViewModels;

[QueryProperty(name: nameof(ProcurementCustomerBasketModel), queryId: nameof(ProcurementCustomerBasketModel))]
[QueryProperty(name: nameof(Items), queryId: nameof(Items))]
[QueryProperty(name: nameof(OrderWarehouseModel), queryId: nameof(OrderWarehouseModel))]
[QueryProperty(name: nameof(ProcurementCustomerFormModel), queryId: nameof(ProcurementCustomerFormModel))]

public partial class ProcurementByCustomerFormViewModel : BaseViewModel
{
	private readonly IHttpClientService _httpClientService;
	private readonly IHttpClientSysService _httpClientSysService;
	private readonly ITransferTransactionService _transferTransactionService;
	private readonly ILocationTransactionService _locationTransactionService;
	private readonly ICarrierService _carrierService;
	private readonly IDriverService _driverService;
	private readonly IUserDialogs _userDialogs;
	private readonly IServiceProvider _serviceProvider;
	private readonly IProcurementAuditCustomerService _procurementAuditCustomerService;
	private readonly IWholeSalesDispatchTransactionService _wholeSalesDispatchTransactionService;
	private readonly IProcurementLocationTransactionService _procurementLocationTransactionService;
	private readonly ILocationService _locationService;
	private readonly IProcurementFicheService _procurementFicheService;
	private readonly Deppo.Sys.Service.Services.IWarehouseService _warehouseSysService;
	private readonly Deppo.Sys.Service.Services.ICustomerService _customerSysService;
	private readonly Deppo.Sys.Service.Services.IProductService _productSysService;
	private readonly Deppo.Sys.Service.Services.ISubunitsetService _subunitsetSysService;

	[ObservableProperty]
	WarehouseModel orderWarehouseModel;

	[ObservableProperty]
	ProcurementCustomerBasketModel procurementCustomerBasketModel = null!;

	[ObservableProperty]
	ProcurementCustomerFormModel procurementCustomerFormModel = null!;

	[ObservableProperty]
	List<ProcurementCustomerBasketModel> items;

	public ObservableCollection<LocationTransactionModel> LocationTransactions { get; } = new();
	private ObservableCollection<LocationTransactionModel> DispatchLocationTransactions { get; } = new();

	public ObservableCollection<LocationModel> Locations { get; } = new();

	[ObservableProperty]
	LocationModel selectedLocation;


	[ObservableProperty]
	private DateTime transactionDate = DateTime.Now;

	[ObservableProperty]
	private string documentNumber = string.Empty;


	[ObservableProperty]
	private string specialCode = string.Empty;

	[ObservableProperty]
	private string description = string.Empty;

	public ProcurementByCustomerFormViewModel(IHttpClientService httpClientService, IHttpClientSysService httpClientSysService, ITransferTransactionService transferTransactionService, IUserDialogs userDialogs, IServiceProvider serviceProvider, ILocationTransactionService locationTransactionService, ICarrierService carrierService, IDriverService driverService, IProcurementAuditCustomerService procurementAuditCustomerService, IWholeSalesDispatchTransactionService wholeSalesDispatchTransactionService, IProcurementLocationTransactionService procurementLocationTransactionService, ILocationService locationService, IProcurementFicheService procurementFicheService, Sys.Service.Services.IWarehouseService warehouseSysService, Sys.Service.Services.ICustomerService customerSysService, Sys.Service.Services.IProductService productSysService, ISubunitsetService subunitsetSysService)
	{
		_httpClientService = httpClientService;
		_httpClientSysService = httpClientSysService;
		_transferTransactionService = transferTransactionService;
		_userDialogs = userDialogs;
		_serviceProvider = serviceProvider;
		_locationTransactionService = locationTransactionService;
		_carrierService = carrierService;
		_driverService = driverService;
		_procurementAuditCustomerService = procurementAuditCustomerService;
		_wholeSalesDispatchTransactionService = wholeSalesDispatchTransactionService;
		_procurementLocationTransactionService = procurementLocationTransactionService;
		_locationService = locationService;
		_procurementFicheService = procurementFicheService;
		_warehouseSysService = warehouseSysService;
		_customerSysService = customerSysService;
		_productSysService = productSysService;
		_subunitsetSysService = subunitsetSysService;

		Title = "Ürün Toplama Formu";

		SaveCommand = new Command(async () => await SaveAsync());
		BackCommand = new Command(async () => await BackAsync());
		LoadLocationsCommand = new Command(async () => await LoadLocationsAsync());
		BasketTappedCommand = new Command(async () => await BasketTappedAsync());
		LoadPageCommand = new Command(async () => await LoadPageAsync());
	}

	public Page CurrentPage { get; set; } = null!;

	public Command LoadPageCommand { get; }
	public Command ShowBasketItemCommand { get; }

	public Command LoadCarriersCommand { get; }
	public Command LoadDriversCommand { get; }
	public Command SaveCommand { get; }
	public Command BackCommand { get; }
	public Command LoadLocationsCommand { get; }
	public Command BasketTappedCommand { get; }

	private async Task BasketTappedAsync()
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

	public async Task LoadLocationsAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			Locations.Clear();
			SelectedLocation = null;

			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _locationService.GetObjects(httpClient, _httpClientService.FirmNumber, _httpClientService.PeriodNumber, OrderWarehouseModel.Number, "", 0, 9999);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					var obj = Mapping.Mapper.Map<LocationModel>(item);
					Locations.Add(obj);
				}
			}

		}
		catch (Exception ex)
		{

			throw;
		}
		finally
		{
			IsBusy = false;
		}
	}

	public async Task LoadLocationTransactionAsync(ProcurementCustomerBasketProductModel product, LocationModel locationModel)
	{
		try
		{
			LocationTransactions.Clear();
			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _locationTransactionService.GetInputObjectsAsync(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: product.ItemReferenceId,
				warehouseNumber: Items.FirstOrDefault().WarehouseNumber,
				locationRef: locationModel.ReferenceId,
				skip: 0,
				take: 99999,
				search: string.Empty
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
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}

	public async Task DispatchLoadLocationTransactionAsync(ProcurementCustomerBasketProductModel product, int ficheReferenceId)
	{
		try
		{
			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var result = await _procurementLocationTransactionService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				productReferenceId: product.ItemReferenceId,
				warehouseNumber: OrderWarehouseModel.Number,
				locationRef: product.DestinationLocationReferenceId,
				ficheReferenceId: ficheReferenceId,
				skip: 0,
				take: 99999,
				search: string.Empty
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
					return;

				foreach (var item in result.Data)
				{
					DispatchLocationTransactions.Add(Mapping.Mapper.Map<LocationTransactionModel>(item));
				}
			}

		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}


	private async Task SaveAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var confirm = await _userDialogs.ConfirmAsync("İşlemi onaylıyor musunuz?", "Onay", "Evet", "Hayır");
			if (!confirm)
				return;


			_userDialogs.ShowLoading("İşlem Tamamlanıyor...");
			await Task.Delay(500);
			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var transferTransactionInsertDto = new TransferTransactionInsert();
			transferTransactionInsertDto.Code = string.Empty;
			transferTransactionInsertDto.IsEDispatch = 0;
			transferTransactionInsertDto.SpeCode = SpecialCode;
			transferTransactionInsertDto.CurrentCode = ProcurementCustomerBasketModel.CustomerCode ?? string.Empty;
			transferTransactionInsertDto.DoCode = DocumentNumber;
			transferTransactionInsertDto.TransactionDate = TransactionDate.AddMinutes(-1);
			transferTransactionInsertDto.Description = Description;
			transferTransactionInsertDto.DestinationWarehouseNumber = OrderWarehouseModel.Number;
			transferTransactionInsertDto.FirmNumber = _httpClientService.FirmNumber;
			transferTransactionInsertDto.ShipInfoCode = ProcurementCustomerBasketModel.ShipAddressCode ?? string.Empty;
			transferTransactionInsertDto.WarehouseNumber = Items.FirstOrDefault().WarehouseNumber;


			foreach (var item in ProcurementCustomerFormModel.Products)
			{
				var tempProductQuantity = item.Quantity;

				var lineDto = new TransferTransactionLineDto();
				lineDto.ProductCode = item.ItemCode;
				lineDto.WarehouseNumber = Items.FirstOrDefault().WarehouseNumber;
				lineDto.DestinationWarehouseNumber = OrderWarehouseModel.Number;
				lineDto.Quantity = item.Quantity;
				lineDto.ConversionFactor = lineDto.Quantity;
				lineDto.OtherConversionFactor = lineDto.Quantity;
				lineDto.SubUnitsetCode = item.SubUnitsetCode;

				foreach (var location in item.Locations)
				{
					await LoadLocationTransactionAsync(item, location);
					var tempLocationQuanity = location.InputQuantity;
					var locationTransactionList = LocationTransactions.OrderBy(x => x.TransactionDate).ToList();

					foreach (var locationTransaction in locationTransactionList)
					{
						var tempLocationTransactionQuantity = locationTransaction.RemainingQuantity;
						while (tempLocationTransactionQuantity > 0 && tempProductQuantity > 0 && tempLocationQuanity > 0)
						{
							var serilotTransactionDto = new SeriLotTransactionDto
							{
								StockLocationCode = locationTransaction.LocationCode,
								InProductTransactionLineReferenceId = locationTransaction.TransactionReferenceId,
								OutProductTransactionLineReferenceId = locationTransaction.ReferenceId,
								SubUnitsetCode = item.SubUnitsetCode,
								DestinationStockLocationCode = item.DestinationLocationCode,
								ConversionFactor = tempLocationQuanity > tempLocationTransactionQuantity ? tempLocationTransactionQuantity : tempLocationQuanity,
								OtherConversionFactor = tempLocationQuanity > tempLocationTransactionQuantity ? tempLocationTransactionQuantity : tempLocationQuanity,
								Quantity = tempLocationQuanity > tempLocationTransactionQuantity ? tempLocationTransactionQuantity : tempLocationQuanity,
							};

							lineDto.SeriLotTransactions.Add(serilotTransactionDto);
							tempLocationTransactionQuantity -= (double)serilotTransactionDto.Quantity;
							tempProductQuantity -= (double)serilotTransactionDto.Quantity;
							tempLocationQuanity -= (double)serilotTransactionDto.Quantity;
						}
					}

				}

				transferTransactionInsertDto.Lines.Add(lineDto);
			}


			var result = await _transferTransactionService.InsertTransferTransaction(httpClient, transferTransactionInsertDto, _httpClientService.FirmNumber);

			ResultModel resultModel = new();

			if (result.IsSuccess)
			{
				resultModel.Message = "Başarılı";
				resultModel.Code = result.Data.Code;
				resultModel.PageTitle = "Ürün Toplama İşlemi";
				resultModel.PageCountToBack = 7;

				foreach (var item in Items)
				{
					foreach (var item1 in item.Products.Where(x => x.RejectionCode != string.Empty))
					{
						var httpSysClient = _httpClientSysService.GetOrCreateHttpClient();

						ProcurementAuditCustomerDto procurementAuditCustomerDto = new ProcurementAuditCustomerDto
						{
							ApplicationUser = _httpClientSysService.UserOid,
							ReasonsForRejectionProcurement = item1.RejectionOid,
							CurrentCode = ProcurementCustomerBasketModel.CustomerCode ?? string.Empty,
							CurrentName = ProcurementCustomerBasketModel.CustomerName ?? string.Empty,
							CurrentReferenceId = ProcurementCustomerBasketModel.CustomerReferenceId,
							IsVariant = item1.IsVariant,
							Quantity = item1.Quantity,
							ProcurementQuantity = item1.ProcurementQuantity,
							ProductName = item1.IsVariant ? item1.MainItemName : item1.ItemName,
							ProductReferenceId = item1.IsVariant ? item1.MainItemReferenceId : item1.ItemReferenceId,
							CreatedOn = DateTime.Now,
							LocationCode = item.LocationCode,
							LocationReferenceId = item.LocationReferenceId,
							LocationName = item.LocationName,
							WarehouseName = ProcurementCustomerBasketModel.WarehouseName,
							WarehouseNumber = ProcurementCustomerBasketModel.WarehouseNumber,
						};

						await _procurementAuditCustomerService.CreateAsync(
							httpClient: httpSysClient,
							dto: procurementAuditCustomerDto
						);
					}
				}



				try
				{
					var wholeResult = await WholeSalesDispatchInsert(httpClient, result.Data.ReferenceId, result.Data.Code);
					if(!wholeResult.IsSuccess)
					{
						resultModel.Message = "Başarısız";
						resultModel.PageTitle = "Toptan Satış İrsaliyesi";
						resultModel.PageCountToBack = 7;
						resultModel.ErrorMessage = wholeResult.Message;

						await ClearFormAsync();
						await ClearDataAsync();

						if (_userDialogs.IsHudShowing)
							_userDialogs.HideHud();

						await Shell.Current.GoToAsync($"{nameof(InsertFailurePageView)}", new Dictionary<string, object>
						{
							[nameof(ResultModel)] = resultModel
						});

						return;
					}
				}
				catch (Exception ex)
				{
					await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
				}

				//#region ProcurementFiche Insert
				//try
				//{
				//	var httpSysClient = _httpClientSysService.GetOrCreateHttpClient();
				//	var customerResult = await _customerSysService.GetAllAsync(httpSysClient, $"$filter= Code eq '{ProcurementCustomerBasketModel.CustomerCode}'");
				//	var warehouseResult = await _warehouseSysService.GetAllAsync(httpSysClient, $"$filter= WarehouseNumber eq {OrderWarehouseModel.Number}");

				//	ProcurementFicheDto procurementFicheDto = new ProcurementFicheDto
				//	{
				//		CreatedOn = DateTime.Now,
				//		Customer = customerResult.FirstOrDefault().Oid,
				//		ReferenceId = result.Data.ReferenceId,
				//		FicheNumber = result.Data.Code,
				//	};

				//	foreach (var item in Items)
				//	{
				//		foreach (var product in item.Products)
				//		{
				//			ProcurementFicheTransactionDto procurementFicheTransactionDto = new();
				//			var productResult = await _productSysService.GetAllAsync(httpSysClient, $"$filter= Code eq '{product.ItemCode}'");
				//			var subUnitsetResult = await _subunitsetSysService.GetAllAsync(httpSysClient, $"filter= Code eq '{product.SubUnitsetCode}'");

				//			foreach (var order in product.Orders)
				//			{
				//				procurementFicheTransactionDto.Product = productResult.FirstOrDefault().Oid;
				//				procurementFicheTransactionDto.SubUnitset = subUnitsetResult.FirstOrDefault().Oid;
				//				procurementFicheTransactionDto.Quantity = product.Quantity;
				//				procurementFicheTransactionDto.Warehouse = warehouseResult.FirstOrDefault().Oid;
				//				procurementFicheTransactionDto.OrderNumber = order.OrderNumber;
				//				procurementFicheTransactionDto.OrderReferenceId = order.OrderReferenceId;
				//			}
				//			procurementFicheDto.Lines.Add(procurementFicheTransactionDto);
				//		}
				//	}
				//	await _procurementFicheService.CreateAsync(httpSysClient, procurementFicheDto);
				//}
				//catch (Exception ex)
				//{
				//	throw;
				//}
				//#endregion


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
				resultModel.PageTitle = "Ürün Toplama";
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

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
		finally
		{
			IsBusy = false;
		}
	}


	private async Task<DataResult<ResponseModel>> WholeSalesDispatchInsert(HttpClient httpClient, int ficheReferenceId, string code)
	{
		var dto = await CreateDto(ficheReferenceId, code);


		var result = await _wholeSalesDispatchTransactionService.InsertWholeSalesDispatchTransaction(httpClient, _httpClientService.FirmNumber, dto);


		return result;

	}

	private async Task<WholeSalesDispatchTransactionInsert> CreateDto(int ficheReferenceId, string code)
	{
		var dto = new WholeSalesDispatchTransactionInsert
		{
			Code = "",
			CurrentCode = ProcurementCustomerBasketModel.CustomerCode ?? string.Empty,
			ShipInfoCode = ProcurementCustomerBasketModel.ShipAddressCode ?? string.Empty,
			IsEDispatch = 0,
			DispatchType = 0,
			Status = 1,
			DispatchStatus = 0,
			EDispatchProfileId = 0,
			Description = Description,
			DoCode = DocumentNumber,
			TransactionDate = TransactionDate,
			FirmNumber = _httpClientService.FirmNumber,
			DocTrackingNumber = code,
			SpeCode = SpecialCode,
			WarehouseNumber = OrderWarehouseModel.Number,
		};

		dto.Lines = await GetLines(ficheReferenceId);

		return dto;
	}

	private async Task<List<WholeSalesDispatchTransactionLineInsert>> GetLines(int ficheReferenceId)
	{
		List<WholeSalesDispatchTransactionLineInsert> wholeSalesDispatchTransactionLineInserts = new();


		foreach (var item in ProcurementCustomerFormModel.Products)
		{
			await DispatchLoadLocationTransactionAsync(item, ficheReferenceId);
			var itemQuantity = item.Quantity;

			foreach (var order in item.Orders.OrderBy(x => x.OrderDate))
			{
				if (itemQuantity == 0)
					break;

				var lineDto = new WholeSalesDispatchTransactionLineInsert
				{
					Quantity = itemQuantity > order.WaitingQuantity ? order.WaitingQuantity : itemQuantity,
					ProductCode = item.ItemCode,
					WarehouseNumber = (short?)OrderWarehouseModel.Number,
					OrderReferenceId = order.ReferenceId,
					UnitPrice = order.Price,
					VatRate = order.Vat,
					ConversionFactor = itemQuantity > order.WaitingQuantity ? order.WaitingQuantity : itemQuantity,
					OtherConversionFactor = itemQuantity > order.WaitingQuantity ? order.WaitingQuantity : itemQuantity,
					SubUnitsetCode = item.SubUnitsetCode,
				};

				lineDto.SeriLotTransactions = await GetSerilotTransactionAsync(lineDto, item);
				wholeSalesDispatchTransactionLineInserts.Add(lineDto);
				itemQuantity -= (double)lineDto.Quantity;
			}

			DispatchLocationTransactions.Clear();
		}

		return wholeSalesDispatchTransactionLineInserts;
	}



	private async Task<List<SeriLotTransactionDto>> GetSerilotTransactionAsync(WholeSalesDispatchTransactionLineInsert line,
	ProcurementCustomerBasketProductModel item)
	{
		List<SeriLotTransactionDto> wholeSalesDispatchTransactionserilots = new();

		double detailQuantity = (double)line.Quantity;

		foreach (var locationTransaction in DispatchLocationTransactions.OrderBy(x => x.TransactionDate).Where(x => x.RemainingQuantity > 0))
		{
			var tempLocationQuantity = locationTransaction.RemainingQuantity;
			while (tempLocationQuantity > 0 && detailQuantity > 0)
			{

				var serilotTransactionDto = new SeriLotTransactionDto
				{
					StockLocationCode = item.DestinationLocationCode,
					InProductTransactionLineReferenceId = locationTransaction.TransactionReferenceId,
					OutProductTransactionLineReferenceId = locationTransaction.ReferenceId,
					Quantity = tempLocationQuantity > detailQuantity ? detailQuantity : tempLocationQuantity,
					SubUnitsetCode = line.SubUnitsetCode,
					DestinationStockLocationCode = string.Empty,
					ConversionFactor = tempLocationQuantity > detailQuantity ? detailQuantity : tempLocationQuantity,
					OtherConversionFactor = tempLocationQuantity > detailQuantity ? detailQuantity : tempLocationQuantity
				};

				detailQuantity -= (double)serilotTransactionDto.Quantity;
				tempLocationQuantity -= (double)serilotTransactionDto.Quantity;
				wholeSalesDispatchTransactionserilots.Add(serilotTransactionDto);
			}
		}

		return wholeSalesDispatchTransactionserilots;
	}


	private async Task BackAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			var confirm = await _userDialogs.ConfirmAsync("Form verileriniz silinecektir. Devam etmek istiyor musunuz?", "Uyarı", "Evet", "Hayır");
			if (!confirm)
				return;

			DocumentNumber = string.Empty;
			SpecialCode = string.Empty;
			Description = string.Empty;
			TransactionDate = DateTime.Now;

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

	private async Task ClearFormAsync()
	{
		DocumentNumber = string.Empty;
		SpecialCode = string.Empty;
		Description = string.Empty;
		TransactionDate = DateTime.Now;
	}

	private async Task ClearDataAsync()
	{
		try
		{
			var warehouseListViewModel = _serviceProvider.GetRequiredService<ProcurementByCustomerWarehouseListViewModel>();
			var customerListViewModel = _serviceProvider.GetRequiredService<ProcurementByCustomerListViewModel>();
			var productListViewModel = _serviceProvider.GetRequiredService<ProcurementByProductListViewModel>();
			var basketViewModel = _serviceProvider.GetRequiredService<ProcurementByCustomerBasketViewModel>();
			var procurementWarehouseListViewModel = _serviceProvider.GetRequiredService<ProcurementByCustomerProcurementWarehouseListViewModel>();

			if (warehouseListViewModel.SelectedWarehouseModel is not null)
			{
				warehouseListViewModel.SelectedWarehouseModel.IsSelected = false;
				warehouseListViewModel.SelectedWarehouseModel = null;
			}

			if (procurementWarehouseListViewModel.SelectedWarehouseModel is not null)
			{
				procurementWarehouseListViewModel.SelectedWarehouseModel.IsSelected = false;
				procurementWarehouseListViewModel.SelectedWarehouseModel = null;
			}

			if (customerListViewModel.SelectedCustomerModel is not null)
			{
				customerListViewModel.SelectedCustomerModel.IsSelected = false;
				customerListViewModel.SelectedCustomerModel = null;
			}

			if (customerListViewModel.SelectedShipAddressModel is not null)
			{
				customerListViewModel.SelectedShipAddressModel.IsSelected = false;
				customerListViewModel.SelectedShipAddressModel = null;
			}

			if (productListViewModel.SelectedProductOrderModel is not null)
			{
				productListViewModel.SelectedProductOrderModel.IsSelected = false;
				productListViewModel.SelectedProductOrderModel = null;
			}

			foreach (var item in basketViewModel.Items)
			{
				item.ProcurementProductList.Clear();
				item.Products.Clear();
			}

			basketViewModel.ProcurementCustomerBasketModel.Products.Clear();
			basketViewModel.ProcurementCustomerFormModel.Products.Clear();
			basketViewModel.ProcurementCustomerBasketModel.ProcurementProductList.Clear();

			//basketViewModel.ProcurementCustomerFormModel = null;
			//basketViewModel.ProcurementCustomerBasketModel = null;

			basketViewModel.Items.Clear();
			basketViewModel.DataItems.Clear();

			LocationTransactions.Clear();
			DispatchLocationTransactions.Clear();

		}
		catch (Exception ex)
		{
			_userDialogs.Alert(ex.Message, "Hata", "Tamam");
		}
	}
}
