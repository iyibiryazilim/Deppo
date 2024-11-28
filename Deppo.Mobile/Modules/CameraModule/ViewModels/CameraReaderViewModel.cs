using AndroidX.Lifecycle;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BarcodeModels;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.CameraModels;
using Deppo.Mobile.Core.Models.CountingModels.BasketModels;
using Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.SalesModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.TransferModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.BarcodeHelper;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
using Deppo.Mobile.Modules.CountingModule.CountingProcess.WarehouseCountingProcess.ViewModels;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.InputOutsourceProcess.InputOutsourceTransfer.ViewModels;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcess.OutputOutsourceTransfer.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.DemandProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.InputProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.OutputProductProcess.ViewModels;
using Deppo.Mobile.Modules.ProductModule.ProductProcess.TransferProductProcess.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseOrderProcess.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.InputProductPurchaseProcess.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseDispatchProcess.ViewModels;
using Deppo.Mobile.Modules.PurchaseModule.PurchaseProcess.ReturnProductPurchaseProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesOrderProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.OutputProductSalesProcess.ViewModels;
using Deppo.Mobile.Modules.SalesModule.SalesProcess.ReturnProductSalesProcess.ViewModels;
using Newtonsoft.Json;
using ZXing.Net.Maui;
using ZXing.Net.Maui.Controls;
using ZXing.PDF417.Internal;

namespace Deppo.Mobile.Modules.CameraModule.ViewModels;

[QueryProperty(name: nameof(CameraScanModel), queryId: nameof(CameraScanModel))]
public partial class CameraReaderViewModel : BaseViewModel
{
	private readonly IUserDialogs _userDialogs;
	private readonly IHttpClientService _httpClientService;
	private readonly IServiceProvider _serviceProvider;
	private readonly IWaitingSalesOrderService _waitingSalesOrderService;
	private readonly IWaitingPurchaseOrderService _waitingPurchaseOrderService;
	private readonly IBarcodeSearchService _barcodeSearchService;
	private readonly IBarcodeSearchOutService _barcodeSearchOutService;
	private readonly IBarcodeSearchDemandService _barcodeSearchDemandService;
	private readonly IBarcodeSearchSalesService _barcodeSearchSalesService;
	private readonly IBarcodeSearchPurchaseService _barcodeSearchPurchaseService;

	[ObservableProperty]
	CameraScanModel cameraScanModel = null!;


	bool isFind = false;
	public CameraReaderViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, IServiceProvider serviceProvider, IBarcodeSearchService barcodeSearchService, IBarcodeSearchOutService barcodeSearchOutService, IBarcodeSearchDemandService barcodeSearchDemandService, IBarcodeSearchSalesService barcodeSearchSalesService, IBarcodeSearchPurchaseService barcodeSearchPurchaseService, IWaitingSalesOrderService waitingSalesOrderService, IWaitingPurchaseOrderService waitingPurchaseOrderService)
	{
		_userDialogs = userDialogs;
		_httpClientService = httpClientService;
		_serviceProvider = serviceProvider;
		_waitingSalesOrderService = waitingSalesOrderService;
		_waitingPurchaseOrderService = waitingPurchaseOrderService;
		_barcodeSearchService = barcodeSearchService;
		_barcodeSearchOutService = barcodeSearchOutService;
		_barcodeSearchDemandService = barcodeSearchDemandService;
		_barcodeSearchSalesService = barcodeSearchSalesService;
		_barcodeSearchPurchaseService = barcodeSearchPurchaseService;

		isFind = false;

		BackCommand = new Command(async () => await BackAsync());
		CameraDetectedCommand = new Command<BarcodeDetectionEventArgs>(async (e) => await CameraDetectedAsync(e));
		SwitchCameraTappedCommand = new Command(async () => await SwitchCameraTappedAsync());
		FlashlightTappedCommand = new Command(async () => await FlashlightTappedAsync());
		
	}

	public CameraBarcodeReaderView BarcodeReader { get; set; } = null!;

	public Command BackCommand { get; }
	public Command CameraDetectedCommand { get; }
	public Command SwitchCameraTappedCommand { get; }
	public Command FlashlightTappedCommand { get; }

	private IReadOnlyList<string> InPages = new List<string>
	{
		"InputProductProcessBasketListViewModel",
		"InputProductPurchaseProcessBasketListViewModel",
		"InputOutsourceTransferOutsourceBasketListViewModel",
		"ReturnSalesBasketViewModel",
		"WarehouseCountingBasketViewModel"
	};

	private IReadOnlyList<string> OutPages = new List<string>
	{
		"OutputProductProcessBasketListViewModel",
		"OutputProductSalesProcessBasketListViewModel",
		"ReturnPurchaseBasketViewModel",
		"TransferOutBasketViewModel",
		"OutputOutsourceTransferBasketListViewModel"
	};

	private IReadOnlyList<string> DemandPages = new List<string>
	{
		"DemandProcessBasketListViewModel",

	};

	private IReadOnlyList<string> SalesPages = new List<string>
	{
		"OutputProductSalesOrderProcessBasketListViewModel",

	};

	private IReadOnlyList<string> PurchasePages = new List<string>
	{
		"InputProductPurchaseOrderProcessBasketListViewModel",
	};


	private async Task SearchBarcodeAsync(BarcodeResult readBarcode)
	{
		try
		{
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var firmNumber = _httpClientService.FirmNumber;
			var periodNumber = _httpClientService.PeriodNumber;

			isFind = false;


			var first = readBarcode;

			if (first is null)
			{
				await _userDialogs.AlertAsync("Barkod Bulunamadı", "Hata", "Tamam");
				return;
			}

			if(InPages.Contains(CameraScanModel.ComingPage))
			{
				Task inSearchByProductCodeTask = InSearchByProductCodeAsync(httpClient, firmNumber, periodNumber, first.Value);
				Task inSearchByVariantCodeTask = InSearchByVariantCodeAsync(httpClient, firmNumber, periodNumber, first.Value);
				Task inSearchByProductMainBarcodeTask = InSearchByProductMainBarcodeAsync(httpClient, firmNumber, periodNumber, first.Value);
				Task inSearchByVariantMainBarcodeTask = InSearchByVariantMainBarcodeAsync(httpClient, firmNumber, periodNumber, first.Value);
				Task inSearchByProductSubBarcodeTask = InSearchByProductSubBarcodeAsync(httpClient, firmNumber, periodNumber, first.Value);
				Task inSearchByVariantSubBarcodeTask = InSearchByVariantSubBarcodeAsync(httpClient, firmNumber, periodNumber, first.Value);
				Task inSearchByProductSeriNumberTask = InSearchByProductSeriNumberAsync(httpClient, firmNumber, periodNumber, first.Value);
				Task inSearchByVariantSeriNumberTask = InSearchByVariantSeriNumberAsync(httpClient, firmNumber, periodNumber, first.Value);
				Task inSearchByProductLotNumberTask = InSearchByProductLotNumberAsync(httpClient, firmNumber, periodNumber, first.Value);
				Task inSearchByVariantLotNumberTask = InSearchByVariantLotNumberAsync(httpClient, firmNumber, periodNumber, first.Value);

				// Code tasks
				await Task.WhenAll(inSearchByProductCodeTask, inSearchByVariantCodeTask);

				if (isFind)
					return;


				//Barcode tasks
				await Task.WhenAll(inSearchByProductMainBarcodeTask, inSearchByVariantMainBarcodeTask, inSearchByProductSubBarcodeTask, inSearchByVariantSubBarcodeTask);

				if (isFind)
					return;

				// Seri tasks
				await Task.WhenAll(inSearchByProductSeriNumberTask, inSearchByVariantSeriNumberTask);

				if (isFind)
					return;

				// Lot tasks
				await Task.WhenAll(inSearchByProductLotNumberTask, inSearchByVariantLotNumberTask);

				if (isFind)
					return;
			}
			else if(OutPages.Contains(CameraScanModel.ComingPage))
			{
				Task outSearchByProductCodeTask = OutSearchByProductCodeAsync(httpClient, firmNumber, periodNumber, first.Value, CameraScanModel.WarehouseNumber);
				Task outSearchByProductMainBarcodeTask = OutSearchByProductMainBarcodeAsync(httpClient, firmNumber, periodNumber, first.Value, CameraScanModel.WarehouseNumber);
				Task outSearchByProductSubBarcodeTask = OutSearchByProductSubBarcodeAsync(httpClient, firmNumber, periodNumber, first.Value, CameraScanModel.WarehouseNumber);
				Task outSearchByProductSeriNumberTask = OutSearchByProductSeriNumberAsync(httpClient, firmNumber, periodNumber, first.Value, CameraScanModel.WarehouseNumber);
				Task outSearchByProductLotNumberTask = OutSearchByProductLotNumberAsync(httpClient, firmNumber, periodNumber, first.Value, CameraScanModel.WarehouseNumber);

				// Code tasks
				await Task.WhenAll(outSearchByProductCodeTask);

				if (isFind)
					return;


				//Barcode tasks
				await Task.WhenAll(outSearchByProductMainBarcodeTask, outSearchByProductSubBarcodeTask);

				if (isFind)
					return;

				// Seri tasks
				await Task.WhenAll(outSearchByProductSeriNumberTask);

				if (isFind)
					return;

				// Lot tasks
				await Task.WhenAll(outSearchByProductLotNumberTask);

				if (isFind)
					return;
			}
			else if (DemandPages.Contains(CameraScanModel.ComingPage))
			{
				Task demandSearchByProductCodeTask = DemandSearchByProductCodeAsync(httpClient, firmNumber, periodNumber, first.Value, CameraScanModel.WarehouseNumber);
				Task demandSearchByProductMainBarcodeTask = DemandSearchByProductMainBarcodeAsync(httpClient, firmNumber, periodNumber, first.Value, CameraScanModel.WarehouseNumber);
				Task demandSearchByProductSubBarcodeTask = DemandSearchByProductSubBarcodeAsync(httpClient, firmNumber, periodNumber, first.Value, CameraScanModel.WarehouseNumber);
				Task demandSearchByProductSeriNumberTask = DemandSearchByProductSeriNumberAsync(httpClient, firmNumber, periodNumber, first.Value, CameraScanModel.WarehouseNumber);
				Task demandSearchByProductLotNumberTask = DemandSearchByProductLotNumberAsync(httpClient, firmNumber, periodNumber, first.Value, CameraScanModel.WarehouseNumber);

				// Code tasks
				await Task.WhenAll(demandSearchByProductCodeTask);

				if (isFind)
					return;


				//Barcode tasks
				await Task.WhenAll(demandSearchByProductMainBarcodeTask, demandSearchByProductSubBarcodeTask);

				if (isFind)
					return;

				// Seri tasks
				await Task.WhenAll(demandSearchByProductSeriNumberTask);

				if (isFind)
					return;

				// Lot tasks
				await Task.WhenAll(demandSearchByProductLotNumberTask);

				if (isFind)
					return;
			}
			else if (SalesPages.Contains(CameraScanModel.ComingPage))
			{
				Task salesSearchByProductCodeTask = SalesSearchByProductCodeAsync(httpClient, firmNumber, periodNumber, first.Value, CameraScanModel.WarehouseNumber, CameraScanModel.CurrentReferenceId, CameraScanModel.ShipInfoReferenceId);
				Task salesSearchByProductMainBarcodeTask = SalesSearchByProductMainBarcodeAsync(httpClient, firmNumber, periodNumber, first.Value, CameraScanModel.WarehouseNumber, CameraScanModel.CurrentReferenceId, CameraScanModel.ShipInfoReferenceId);
				Task salesSearchByProductSubBarcodeTask = SalesSearchByProductSubBarcodeAsync(httpClient, firmNumber, periodNumber, first.Value, CameraScanModel.WarehouseNumber, CameraScanModel.CurrentReferenceId, CameraScanModel.ShipInfoReferenceId);
				Task salesSearchByProductSeriNumberTask = SalesSearchByProductSeriNumberAsync(httpClient, firmNumber, periodNumber, first.Value, CameraScanModel.WarehouseNumber, CameraScanModel.CurrentReferenceId, CameraScanModel.ShipInfoReferenceId);
				Task salesSearchByProductLotNumberTask = SalesSearchByProductLotNumberAsync(httpClient, firmNumber, periodNumber, first.Value, CameraScanModel.WarehouseNumber, CameraScanModel.CurrentReferenceId, CameraScanModel.ShipInfoReferenceId);

				// Code tasks
				await Task.WhenAll(salesSearchByProductCodeTask);

				if (isFind)
					return;


				//Barcode tasks
				await Task.WhenAll(salesSearchByProductMainBarcodeTask, salesSearchByProductSubBarcodeTask);

				if (isFind)
					return;

				// Seri tasks
				await Task.WhenAll(salesSearchByProductSeriNumberTask);

				if (isFind)
					return;

				// Lot tasks
				await Task.WhenAll(salesSearchByProductLotNumberTask);

				if (isFind)
					return;
			}
			else if (PurchasePages.Contains(CameraScanModel.ComingPage))
			{
				Task purchaseSearchByProductCodeTask = PurchaseSearchByProductCodeAsync(httpClient, firmNumber, periodNumber, first.Value, CameraScanModel.WarehouseNumber, CameraScanModel.CurrentReferenceId, CameraScanModel.ShipInfoReferenceId);
				Task purchaseSearchByProductMainBarcodeTask = PurchaseSearchByProductMainBarcodeAsync(httpClient, firmNumber, periodNumber, first.Value, CameraScanModel.WarehouseNumber, CameraScanModel.CurrentReferenceId, CameraScanModel.ShipInfoReferenceId);
				Task purchaseSearchByProductSubBarcodeTask = PurchaseSearchByProductSubBarcodeAsync(httpClient, firmNumber, periodNumber, first.Value, CameraScanModel.WarehouseNumber, CameraScanModel.CurrentReferenceId, CameraScanModel.ShipInfoReferenceId);
				Task purchaseSearchByProductSeriNumberTask = PurchaseSearchByProductSeriNumberAsync(httpClient, firmNumber, periodNumber, first.Value, CameraScanModel.WarehouseNumber, CameraScanModel.CurrentReferenceId, CameraScanModel.ShipInfoReferenceId);
				Task purchaseSearchByProductLotNumberTask = PurchaseSearchByProductLotNumberAsync(httpClient, firmNumber, periodNumber, first.Value, CameraScanModel.WarehouseNumber, CameraScanModel.CurrentReferenceId, CameraScanModel.ShipInfoReferenceId);
				Task purchaseSearchBySupplierProductCodeTask = PurchaseSearchBySupplierProductCodeAsync(httpClient, firmNumber, periodNumber, first.Value, CameraScanModel.WarehouseNumber, CameraScanModel.CurrentReferenceId, CameraScanModel.ShipInfoReferenceId);

				// Code tasks
				await Task.WhenAll(purchaseSearchByProductCodeTask);

				if (isFind)
					return;


				//Barcode tasks
				await Task.WhenAll(purchaseSearchByProductMainBarcodeTask, purchaseSearchByProductSubBarcodeTask);

				if (isFind)
					return;

				// Seri tasks
				await Task.WhenAll(purchaseSearchByProductSeriNumberTask);

				if (isFind)
					return;

				// Lot tasks
				await Task.WhenAll(purchaseSearchByProductLotNumberTask);

				if (isFind)
					return;

				// Supplier tasks
				await Task.WhenAll(purchaseSearchBySupplierProductCodeTask);
				if (isFind)
					return;
			}





			_userDialogs.ShowToast($"{readBarcode.Value} barkodunda herhangi bir ürün bulunamadı");

		}
		catch (System.Exception)
		{

			throw;
		}
	}

	private async Task CameraDetectedAsync(BarcodeDetectionEventArgs e)
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			await SearchBarcodeAsync(e.Results.FirstOrDefault());
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

			isFind = false;
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

	private async Task SwitchCameraTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			BarcodeReader.CameraLocation = BarcodeReader.CameraLocation == CameraLocation.Rear ? CameraLocation.Front : CameraLocation.Rear;
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

	private async Task FlashlightTappedAsync()
	{
		if (IsBusy)
			return;
		try
		{
			IsBusy = true;

			BarcodeReader.IsTorchOn = !BarcodeReader.IsTorchOn;
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


	#region InPages Functions
	private async Task InSearchByProductCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
	{
		try
		{

			if (!isFind)
			{
				var result = await _barcodeSearchService.SearchByProductCode(httpClient, firmNumber, periodNumber, barcode);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<BarcodeInProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendProductBasketPageAsync(productModel);
					}
				}
			}
		}
		catch (System.Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}
	}

	private async Task InSearchByVariantCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchService.SearchByVariantCode(httpClient, firmNumber, periodNumber, barcode);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var variantModelList = JsonConvert.DeserializeObject<List<VariantModel>>(jsonString);

						var variantModel = variantModelList.First();
						isFind = true;
						await SendVariantBasketPageAsync(variantModel);
					}
				}
			}
		}
		catch (Exception)
		{

			throw;
		}
	}

	private async Task InSearchByProductMainBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchService.SearchByProductMainBarcode(httpClient, firmNumber, periodNumber, barcode);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<BarcodeInProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendProductBasketPageAsync(productModel);
					}
				}
			}
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task InSearchByVariantMainBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchService.SearchByVariantMainBarcode(httpClient, firmNumber, periodNumber, barcode);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var variantModelList = JsonConvert.DeserializeObject<List<VariantModel>>(jsonString);

						var variantModel = variantModelList.First();

						isFind = true;
						await SendVariantBasketPageAsync(variantModel);
					}
				}
			}
		}
		catch (System.Exception)
		{

			throw;
		}
	}

	private async Task InSearchByProductSubBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchService.SearchByProductSubBarcode(httpClient, firmNumber, periodNumber, barcode);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<BarcodeInProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendProductBasketPageAsync(productModel);
					}
				}
			}
		}
		catch (System.Exception)
		{

			throw;
		}
	}

	private async Task InSearchByVariantSubBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchService.SearchByVariantSubBarcode(httpClient, firmNumber, periodNumber, barcode);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var variantModelList = JsonConvert.DeserializeObject<List<VariantModel>>(jsonString);

						var variantModel = variantModelList.First();

						isFind = true;
						await SendVariantBasketPageAsync(variantModel);
					}
				}
			}
		}
		catch (System.Exception)
		{

			throw;
		}
	}

	private async Task InSearchByProductSeriNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchService.SearchByProductSeriNumber(httpClient, firmNumber, periodNumber, barcode);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<BarcodeInProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendProductBasketPageAsync(productModel);
					}
				}
			}
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task InSearchByVariantSeriNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchService.SearchByVariantSeriNumber(httpClient, firmNumber, periodNumber, barcode);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var variantModelList = JsonConvert.DeserializeObject<List<VariantModel>>(jsonString);

						var variantModel = variantModelList.First();

						isFind = true;
						await SendVariantBasketPageAsync(variantModel);
					}
				}
			}
		}
		catch (Exception)
		{

			throw;
		}
	}

	private async Task InSearchByProductLotNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchService.SearchByProductLotNumber(httpClient, firmNumber, periodNumber, barcode);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<BarcodeInProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendProductBasketPageAsync(productModel);

					}


				}
			}
		}
		catch (System.Exception)
		{

			throw;
		}
	}

	private async Task InSearchByVariantLotNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchService.SearchByVariantLotNumber(httpClient, firmNumber, periodNumber, barcode);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var variantModelList = JsonConvert.DeserializeObject<List<VariantModel>>(jsonString);

						var variantModel = variantModelList.First();

						isFind = true;
						await SendVariantBasketPageAsync(variantModel);
					}
				}
			}
		}
		catch (System.Exception)
		{

			throw;
		}
	}
	#endregion


	#region OutPages Functions
	private async Task OutSearchByProductCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber)
	{
		try
		{

			if (!isFind)
			{
				var result = await _barcodeSearchOutService.SearchByProductCode(httpClient, firmNumber, periodNumber, barcode, warehouseNumber);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<BarcodeOutProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendOutProductBasketPageAsync(productModel);
					}
				}
			}
		}
		catch (System.Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}
	}

	//private async Task OutSearchByVariantCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber)
	//{
	//	try
	//	{
	//		if (!isFind)
	//		{
	//			var result = await _barcodeSearchOutService.SearchByVariantCode(httpClient, firmNumber, periodNumber, barcode, warehouseNumber);

	//			if (result.IsSuccess)
	//			{
	//				if (result.Data is not null && result.Data.Count != 0)
	//				{
	//					string jsonString = result.Data.ToString();
	//					var variantModelList = JsonConvert.DeserializeObject<List<VariantModel>>(jsonString);

	//					var variantModel = variantModelList.First();

	//					isFind = true;
	//					await SendVariantBasketPageAsync(variantModel);
	//				}
	//			}
	//		}
	//	}
	//	catch (Exception)
	//	{

	//		throw;
	//	}
	//}

	private async Task OutSearchByProductMainBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchOutService.SearchByProductMainBarcode(httpClient, firmNumber, periodNumber, barcode, warehouseNumber);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<BarcodeOutProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendOutProductBasketPageAsync(productModel);
					}
				}
			}
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	//private async Task OutSearchByVariantMainBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
	//{
	//	try
	//	{
	//		if (!isFind)
	//		{
	//			var result = await _barcodeSearchService.SearchByVariantMainBarcode(httpClient, firmNumber, periodNumber, barcode);

	//			if (result.IsSuccess)
	//			{
	//				if (result.Data is not null && result.Data.Count != 0)
	//				{
	//					string jsonString = result.Data.ToString();
	//					var variantModelList = JsonConvert.DeserializeObject<List<VariantModel>>(jsonString);

	//					var variantModel = variantModelList.First();

	//					isFind = true;
	//					await SendVariantBasketPageAsync(variantModel);
	//				}
	//			}
	//		}
	//	}
	//	catch (System.Exception)
	//	{

	//		throw;
	//	}
	//}

	private async Task OutSearchByProductSubBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchOutService.SearchByProductSubBarcode(httpClient, firmNumber, periodNumber, barcode, warehouseNumber);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<BarcodeOutProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendOutProductBasketPageAsync(productModel);
					}
				}
			}
		}
		catch (System.Exception)
		{

			throw;
		}
	}

	//private async Task OutSearchByVariantSubBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
	//{
	//	try
	//	{
	//		if (!isFind)
	//		{
	//			var result = await _barcodeSearchService.SearchByVariantSubBarcode(httpClient, firmNumber, periodNumber, barcode);

	//			if (result.IsSuccess)
	//			{
	//				if (result.Data is not null && result.Data.Count != 0)
	//				{
	//					string jsonString = result.Data.ToString();
	//					var variantModelList = JsonConvert.DeserializeObject<List<VariantModel>>(jsonString);

	//					var variantModel = variantModelList.First();

	//					isFind = true;
	//					await SendVariantBasketPageAsync(variantModel);
	//				}
	//			}
	//		}
	//	}
	//	catch (System.Exception)
	//	{

	//		throw;
	//	}
	//}

	private async Task OutSearchByProductSeriNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchOutService.SearchByProductSeriNumber(httpClient, firmNumber, periodNumber, barcode, warehouseNumber);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<BarcodeOutProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendOutProductBasketPageAsync(productModel);
					}
				}
			}
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	//private async Task OutSearchByVariantSeriNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
	//{
	//	try
	//	{
	//		if (!isFind)
	//		{
	//			var result = await _barcodeSearchService.SearchByVariantSeriNumber(httpClient, firmNumber, periodNumber, barcode);

	//			if (result.IsSuccess)
	//			{
	//				if (result.Data is not null && result.Data.Count != 0)
	//				{
	//					string jsonString = result.Data.ToString();
	//					var variantModelList = JsonConvert.DeserializeObject<List<VariantModel>>(jsonString);

	//					var variantModel = variantModelList.First();

	//					isFind = true;
	//					await SendVariantBasketPageAsync(variantModel);
	//				}
	//			}
	//		}
	//	}
	//	catch (Exception)
	//	{

	//		throw;
	//	}
	//}

	private async Task OutSearchByProductLotNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchOutService.SearchByProductLotNumber(httpClient, firmNumber, periodNumber, barcode, warehouseNumber);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<BarcodeOutProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendOutProductBasketPageAsync(productModel);

					}


				}
			}
		}
		catch (System.Exception)
		{

			throw;
		}
	}

	//private async Task OutSearchByVariantLotNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
	//{
	//	try
	//	{
	//		if (!isFind)
	//		{
	//			var result = await _barcodeSearchService.SearchByVariantLotNumber(httpClient, firmNumber, periodNumber, barcode);

	//			if (result.IsSuccess)
	//			{
	//				if (result.Data is not null && result.Data.Count != 0)
	//				{
	//					string jsonString = result.Data.ToString();
	//					var variantModelList = JsonConvert.DeserializeObject<List<VariantModel>>(jsonString);

	//					var variantModel = variantModelList.First();

	//					isFind = true;
	//					await SendVariantBasketPageAsync(variantModel);
	//				}
	//			}
	//		}
	//	}
	//	catch (System.Exception)
	//	{

	//		throw;
	//	}
	//}
	#endregion

	#region SalesPages Functions
	private async Task SalesSearchByProductCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int customerReferenceId, int shipInfoReferenceId)
	{
		try
		{

			if (!isFind)
			{
				var result = await _barcodeSearchSalesService.SearchByProductCode(httpClient, firmNumber, periodNumber, barcode, warehouseNumber, customerReferenceId, shipInfoReferenceId);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<BarcodeSalesProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendSalesProductBasketPageAsync(productModel);
					}
				}
			}
		}
		catch (System.Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}
	}
	private async Task SalesSearchByProductMainBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int customerReferenceId, int shipInfoReferenceId)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchSalesService.SearchByProductMainBarcode(httpClient, firmNumber, periodNumber, barcode, warehouseNumber, customerReferenceId, shipInfoReferenceId);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<BarcodeSalesProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendSalesProductBasketPageAsync(productModel);
					}
				}
			}
		}
		catch (Exception ex)
		{

			throw;
		}
	}
	private async Task SalesSearchByProductSubBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int customerReferenceId, int shipInfoReferenceId)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchSalesService.SearchByProductSubBarcode(httpClient, firmNumber, periodNumber, barcode, warehouseNumber, customerReferenceId, shipInfoReferenceId);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<BarcodeSalesProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendSalesProductBasketPageAsync(productModel);
					}
				}
			}
		}
		catch (System.Exception)
		{

			throw;
		}
	}
	private async Task SalesSearchByProductSeriNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int customerReferenceId, int shipInfoReferenceId)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchSalesService.SearchByProductSeriNumber(httpClient, firmNumber, periodNumber, barcode, warehouseNumber, customerReferenceId, shipInfoReferenceId);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<BarcodeSalesProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendSalesProductBasketPageAsync(productModel);
					}
				}
			}
		}
		catch (Exception ex)
		{

			throw;
		}
	}
	private async Task SalesSearchByProductLotNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int customerReferenceId, int shipInfoReferenceId)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchSalesService.SearchByProductLotNumber(httpClient, firmNumber, periodNumber, barcode, warehouseNumber, customerReferenceId, shipInfoReferenceId);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<BarcodeSalesProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendSalesProductBasketPageAsync(productModel);

					}
				}
			}
		}
		catch (System.Exception)
		{

			throw;
		}
	}
	#endregion

	#region PurchasePages Functions
	private async Task PurchaseSearchByProductCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId)
	{
		try
		{

			if (!isFind)
			{
				var result = await _barcodeSearchPurchaseService.SearchByProductCode(httpClient, firmNumber, periodNumber, barcode, warehouseNumber, supplierReferenceId, shipInfoReferenceId);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<BarcodePurchaseProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendPurchaseProductBasketPageAsync(productModel);
					}
				}
			}
		}
		catch (System.Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}
	}
	private async Task PurchaseSearchByProductMainBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchPurchaseService.SearchByProductMainBarcode(httpClient, firmNumber, periodNumber, barcode, warehouseNumber, supplierReferenceId, shipInfoReferenceId);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<BarcodePurchaseProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendPurchaseProductBasketPageAsync(productModel);
					}
				}
			}
		}
		catch (Exception ex)
		{

			throw;
		}
	}
	private async Task PurchaseSearchByProductSubBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchPurchaseService.SearchByProductSubBarcode(httpClient, firmNumber, periodNumber, barcode, warehouseNumber, supplierReferenceId, shipInfoReferenceId);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<BarcodePurchaseProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendPurchaseProductBasketPageAsync(productModel);
					}
				}
			}
		}
		catch (System.Exception)
		{

			throw;
		}
	}
	private async Task PurchaseSearchByProductSeriNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchPurchaseService.SearchByProductSeriNumber(httpClient, firmNumber, periodNumber, barcode, warehouseNumber, supplierReferenceId, shipInfoReferenceId);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<BarcodePurchaseProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendPurchaseProductBasketPageAsync(productModel);
					}
				}
			}
		}
		catch (Exception ex)
		{

			throw;
		}
	}
	private async Task PurchaseSearchByProductLotNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchPurchaseService.SearchByProductLotNumber(httpClient, firmNumber, periodNumber, barcode, warehouseNumber, supplierReferenceId, shipInfoReferenceId);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<BarcodePurchaseProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendPurchaseProductBasketPageAsync(productModel);

					}


				}
			}
		}
		catch (System.Exception)
		{

			throw;
		}
	}

	private async Task PurchaseSearchBySupplierProductCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchPurchaseService.SearchBySupplierProductCode(httpClient, firmNumber, periodNumber, barcode, warehouseNumber, supplierReferenceId, shipInfoReferenceId);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<BarcodePurchaseProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendPurchaseProductBasketPageAsync(productModel);

					}


				}
			}
		}
		catch (System.Exception)
		{

			throw;
		}
	}
	#endregion

	#region DemandPages Functions
	private async Task DemandSearchByProductCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber)
	{
		try
		{

			if (!isFind)
			{
				var result = await _barcodeSearchDemandService.SearchByProductCode(httpClient, firmNumber, periodNumber, barcode, warehouseNumber);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<BarcodeDemandProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendDemandProductBasketPageAsync(productModel);
					}
				}
			}
		}
		catch (System.Exception ex)
		{
			Console.WriteLine(ex);
			throw;
		}
	}
	private async Task DemandSearchByProductMainBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchDemandService.SearchByProductMainBarcode(httpClient, firmNumber, periodNumber, barcode, warehouseNumber);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<BarcodeDemandProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendDemandProductBasketPageAsync(productModel);
					}
				}
			}
		}
		catch (Exception ex)
		{

			throw;
		}
	}
	private async Task DemandSearchByProductSubBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchDemandService.SearchByProductSubBarcode(httpClient, firmNumber, periodNumber, barcode, warehouseNumber);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<BarcodeDemandProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendDemandProductBasketPageAsync(productModel);
					}
				}
			}
		}
		catch (System.Exception)
		{

			throw;
		}
	}
	private async Task DemandSearchByProductSeriNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchDemandService.SearchByProductSeriNumber(httpClient, firmNumber, periodNumber, barcode, warehouseNumber);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<BarcodeDemandProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendDemandProductBasketPageAsync(productModel);
					}
				}
			}
		}
		catch (Exception ex)
		{

			throw;
		}
	}
	private async Task DemandSearchByProductLotNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchDemandService.SearchByProductLotNumber(httpClient, firmNumber, periodNumber, barcode, warehouseNumber);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<BarcodeDemandProductModel>>(jsonString);

						var productModel = productModelList.First();

						isFind = true;
						await SendDemandProductBasketPageAsync(productModel);

					}


				}
			}
		}
		catch (System.Exception)
		{

			throw;
		}
	}
	#endregion




	#region BarcodeIn Send Basket Functions
	private async Task SendProductBasketPageAsync(BarcodeInProductModel productModel)
	{
		try
		{
			switch (CameraScanModel.ComingPage)
			{
				case "InputProductProcessBasketListViewModel":
					var inputProductProcessBasketListViewModel = _serviceProvider.GetRequiredService<InputProductProcessBasketListViewModel>();
					var inputProductBasketItem = await ConvertInputProductBasketAsync(productModel);
					if (inputProductProcessBasketListViewModel.Items.Any(x => x.ItemCode == inputProductBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						inputProductProcessBasketListViewModel.Items.Add(inputProductBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "InputProductPurchaseProcessBasketListViewModel":
					var inputProductPurchaseProcessBasketListViewModel = _serviceProvider.GetRequiredService<InputProductPurchaseProcessBasketListViewModel>();
					var inputPurchaseBasketItem = await ConvertInputPurchaseBasketAsync(productModel);

					if (inputProductPurchaseProcessBasketListViewModel.Items.Any(x => x.ItemCode == inputPurchaseBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						inputProductPurchaseProcessBasketListViewModel.Items.Add(inputPurchaseBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "ReturnSalesBasketViewModel":
					var returnSalesBasketViewModel = _serviceProvider.GetRequiredService<ReturnSalesBasketViewModel>();
					var returnSalesBasketItem = await ConvertReturnSalesBasketAsync(productModel);
					if (returnSalesBasketViewModel.Items.Any(x => x.ItemCode == returnSalesBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						returnSalesBasketViewModel.Items.Add(returnSalesBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "InputOutsourceTransferOutsourceBasketListViewModel":
					var InputOutsourceTransferOutsourceBasketListViewModel = _serviceProvider.GetRequiredService<InputOutsourceTransferOutsourceBasketListViewModel>();
					var inputOutsourceTransferBasketItem = await ConvertInputOutsourceTransferBasketAsync(productModel);

					if (InputOutsourceTransferOutsourceBasketListViewModel.Items.Any(x => x.ItemCode == inputOutsourceTransferBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						InputOutsourceTransferOutsourceBasketListViewModel.Items.Add(inputOutsourceTransferBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "WarehouseCountingBasketViewModel":
					var warehouseCountingBasketViewModel = _serviceProvider.GetRequiredService<WarehouseCountingBasketViewModel>();
					var warehouseCountingBasketItem = await ConvertWarehouseCountingBasketAsync(productModel);
					if (warehouseCountingBasketViewModel.Items.Any(x => x.ItemCode == warehouseCountingBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						warehouseCountingBasketViewModel.Items.Add(warehouseCountingBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;

			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}

	// Yeniden düzenlenecek
	private async Task SendVariantBasketPageAsync(VariantModel variantModel)
	{
		try
		{
			switch (CameraScanModel.ComingPage)
			{
				case "InputProductProcessBasket":
					var inputProductProcessBasketListViewModel = _serviceProvider.GetRequiredService<InputProductProcessBasketListViewModel>();
					var inputProductBasketItem = await ConvertInputProductBasketAsync(variantModel);
					if (inputProductProcessBasketListViewModel.Items.Any(x => x.ItemCode == inputProductBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						inputProductProcessBasketListViewModel.Items.Add(inputProductBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "OutputProductProcessBasket":
					var outputProductProcessBasketListViewModel = _serviceProvider.GetRequiredService<OutputProductProcessBasketListViewModel>();
					var outputProductBasketItem = await ConvertOutputProductBasketAsync(variantModel);

					if (outputProductProcessBasketListViewModel.Items.Any(x => x.ItemCode == outputProductBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						outputProductProcessBasketListViewModel.Items.Add(outputProductBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "TransferOutBasket":
					var transferOutBasketViewModel = _serviceProvider.GetRequiredService<TransferOutBasketViewModel>();
					var outProductItem = await ConvertOutProductAsync(variantModel);
					if (transferOutBasketViewModel.TransferBasketModel.OutProducts.Any(x => x.ItemCode == outProductItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						transferOutBasketViewModel.TransferBasketModel.OutProducts.Add(outProductItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "OutputProductSalesProcessBasket":
					var outputProductSalesProcessBasketListViewModel = _serviceProvider.GetRequiredService<OutputProductSalesProcessBasketListViewModel>();
					var outputSalesBasketItem = await ConvertOutputSalesBasketAsync(variantModel);
					if (outputProductSalesProcessBasketListViewModel.Items.Any(x => x.ItemCode == outputSalesBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						outputProductSalesProcessBasketListViewModel.Items.Add(outputSalesBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "InputProductPurchaseProcessBasket":
					var inputProductPurchaseProcessBasketListViewModel = _serviceProvider.GetRequiredService<InputProductPurchaseProcessBasketListViewModel>();
					var inputPurchaseBasketItem = await ConvertInputPurchaseBasketAsync(variantModel);

					if (inputProductPurchaseProcessBasketListViewModel.Items.Any(x => x.ItemCode == inputPurchaseBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						inputProductPurchaseProcessBasketListViewModel.Items.Add(inputPurchaseBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "ReturnPurchaseBasket":
					var returnPurchaseBasketViewModel = _serviceProvider.GetRequiredService<ReturnPurchaseBasketViewModel>();
					var returnPurchaseBasketItem = await ConvertReturnPurchaseBasketAsync(variantModel);

					if (returnPurchaseBasketViewModel.Items.Any(x => x.ItemCode == returnPurchaseBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						returnPurchaseBasketViewModel.Items.Add(returnPurchaseBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "ReturnSalesBasket":
					var returnSalesBasketViewModel = _serviceProvider.GetRequiredService<ReturnSalesBasketViewModel>();
					var returnSalesBasketItem = await ConvertReturnSalesBasketAsync(variantModel);
					if (returnSalesBasketViewModel.Items.Any(x => x.ItemCode == returnSalesBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						returnSalesBasketViewModel.Items.Add(returnSalesBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "InputProductPurchaseOrderBasket":
					var inputProductPurchaseOrderBasketListViewModel = _serviceProvider.GetRequiredService<InputProductPurchaseOrderProcessBasketListViewModel>();
					var inputPurchaseOrderBasketItem = await ConvertInputPurchaseBasketAsync(variantModel);
					if (inputProductPurchaseOrderBasketListViewModel.Items.Any(x => x.ItemCode == inputPurchaseOrderBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						inputProductPurchaseOrderBasketListViewModel.Items.Add(inputPurchaseOrderBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "OutputProductSalesOrderBasket":
					var outputProductSalesOrderProcessBasketListViewModel = _serviceProvider.GetRequiredService<OutputProductSalesOrderProcessBasketListViewModel>();
					var outputSalesOrderBasketItem = await ConvertOutputSalesBasketAsync(variantModel);
					if (outputProductSalesOrderProcessBasketListViewModel.Items.Any(x => x.ItemCode == outputSalesOrderBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						outputProductSalesOrderProcessBasketListViewModel.Items.Add(outputSalesOrderBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "DemandProcessBasket":
					var demandProcessBasketListViewModel = _serviceProvider.GetRequiredService<DemandProcessBasketListViewModel>();
					var demandProcessBasketItem = await ConvertDemandProcessBasketAsync(variantModel);
					if (demandProcessBasketListViewModel.Items.Any(x => x.ItemCode == demandProcessBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						demandProcessBasketListViewModel.Items.Add(demandProcessBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "OutputOutsourceTransferBasket":
					var outputOutsourceTransferBasketViewModel = _serviceProvider.GetRequiredService<OutputOutsourceTransferBasketListViewModel>();
					var outputOutsourceTransferBasketItem = await ConvertOutputOutsourceTransferBasketAsync(variantModel);
					if (outputOutsourceTransferBasketViewModel.Items.Any(x => x.ItemCode == outputOutsourceTransferBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						outputOutsourceTransferBasketViewModel.Items.Add(outputOutsourceTransferBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
			}
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}
	#endregion

	#region BarcodeOut Send Basket Functions
	private async Task SendOutProductBasketPageAsync(BarcodeOutProductModel productModel)
	{
		try
		{
			switch (CameraScanModel.ComingPage)
			{
				case "OutputProductProcessBasketListViewModel":
					var outputProductProcessBasketListViewModel = _serviceProvider.GetRequiredService<OutputProductProcessBasketListViewModel>();
					var outputProductBasketItem = await ConvertOutputProductBasketAsync(productModel);
					if (outputProductProcessBasketListViewModel.Items.Any(x => x.ItemCode == outputProductBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						outputProductProcessBasketListViewModel.Items.Add(outputProductBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "OutputProductSalesProcessBasketListViewModel":
					var outputProductSalesProcessBasketListViewModel = _serviceProvider.GetRequiredService<OutputProductSalesProcessBasketListViewModel>();
					var outputSalesBasketItem = await ConvertOutputSalesBasketAsync(productModel);

					if (outputProductSalesProcessBasketListViewModel.Items.Any(x => x.ItemCode == outputSalesBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						outputProductSalesProcessBasketListViewModel.Items.Add(outputSalesBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "TransferOutBasketViewModel":
					var transferOutBasketViewModel = _serviceProvider.GetRequiredService<TransferOutBasketViewModel>();
					var outProductItem = await ConvertOutProductAsync(productModel);
					if (transferOutBasketViewModel.TransferBasketModel.OutProducts.Any(x => x.ItemCode == outProductItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						transferOutBasketViewModel.TransferBasketModel.OutProducts.Add(outProductItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "OutputOutsourceTransferBasketListViewModel":
					var outputOutsourceTransferBasketListViewModel = _serviceProvider.GetRequiredService<OutputOutsourceTransferBasketListViewModel>();
					var outputOutsourceTransferBasketItem = await ConvertOutputOutsourceTransferBasketAsync(productModel);
					if (outputOutsourceTransferBasketListViewModel.Items.Any(x => x.ItemCode == outputOutsourceTransferBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						outputOutsourceTransferBasketListViewModel.Items.Add(outputOutsourceTransferBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "ReturnPurchaseBasketViewModel":
					var returnPurchaseBasketViewModel = _serviceProvider.GetRequiredService<ReturnPurchaseBasketViewModel>();
					var returnPurchaseBasketItem = await ConvertReturnPurchaseBasketAsync(productModel);

					if (returnPurchaseBasketViewModel.Items.Any(x => x.ItemCode == returnPurchaseBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						returnPurchaseBasketViewModel.Items.Add(returnPurchaseBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
			}
		}
		catch (Exception ex)
		{

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}
	#endregion

	#region BarcodeDemand Send Basket Functions
	private async Task SendDemandProductBasketPageAsync(BarcodeDemandProductModel productModel)
	{
		try
		{
			switch (CameraScanModel.ComingPage)
			{
				case "DemandProcessBasketListViewModel":
					var demandProcessBasketListViewModel = _serviceProvider.GetRequiredService<DemandProcessBasketListViewModel>();
					var demandProcessBasketItem = await ConvertDemandProcessBasketAsync(productModel);
					if (demandProcessBasketListViewModel.Items.Any(x => x.ItemCode == demandProcessBasketItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						demandProcessBasketListViewModel.Items.Add(demandProcessBasketItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
			}
		}
		catch (Exception ex)
		{

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}
	#endregion

	#region BarcodeSales Send Basket Functions
	private async Task SendSalesProductBasketPageAsync(BarcodeSalesProductModel productModel)
	{
		try
		{
			switch(CameraScanModel.ComingPage)
			{
				case "OutputProductSalesOrderProcessBasketListViewModel":
					var httpClient = _httpClientService.GetOrCreateHttpClient();
					var outputProductSalesOrderProcessBasketListViewModel = _serviceProvider.GetRequiredService<OutputProductSalesOrderProcessBasketListViewModel>();
					var existingItem = outputProductSalesOrderProcessBasketListViewModel.Items.FirstOrDefault(x => x.ItemCode == productModel.ItemCode);
					if(existingItem is not null)
					{
						var existingItemFullOrders = await _waitingSalesOrderService.GetObjectsByProduct(
							httpClient: httpClient,
							firmNumber: _httpClientService.FirmNumber,
							periodNumber: _httpClientService.PeriodNumber,
							warehouseNumber: CameraScanModel.WarehouseNumber,
							customerReferenceId: CameraScanModel.CurrentReferenceId,
							productReferenceId: existingItem.ItemReferenceId,
							shipInfoReferenceId: CameraScanModel.ShipInfoReferenceId,
							skip: 0,
							take: 999999,
							search: ""
						);

						if(existingItemFullOrders.IsSuccess && existingItemFullOrders.Data is not null)
						{
                            foreach (var order in existingItemFullOrders.Data)
                            {
								var obj = Mapping.Mapper.Map<OutputSalesBasketOrderModel>(order);
								if(!existingItem.Orders.Any(x => x.ReferenceId == obj.ReferenceId))
								{
									existingItem.Orders.Add(new OutputSalesBasketOrderModel
									{
										ReferenceId = obj.ReferenceId,
										OrderReferenceId = obj.OrderReferenceId,
										CustomerReferenceId = obj.CustomerReferenceId,
										CustomerCode = obj.CustomerCode,
										CustomerName = obj.CustomerName,
										ProductReferenceId = obj.ProductReferenceId,
										ProductCode = obj.ProductCode,
										ProductName = obj.ProductName,
										UnitsetReferenceId = obj.UnitsetReferenceId,
										UnitsetCode = obj.UnitsetCode,
										UnitsetName = obj.UnitsetName,
										SubUnitsetReferenceId = obj.SubUnitsetReferenceId,
										SubUnitsetCode = obj.SubUnitsetCode,
										SubUnitsetName = obj.SubUnitsetName,
										Quantity = obj.Quantity,
										ShippedQuantity = obj.ShippedQuantity,
										WaitingQuantity = obj.WaitingQuantity,
										OrderDate = obj.OrderDate,
										Price = obj.Price,
										Vat = obj.Vat,
										DueDate = obj.DueDate
									});
								}
                            }
                        }
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						var outputSalesBasketModelItem = new OutputSalesBasketModel
						{
							ItemReferenceId = productModel.ItemReferenceId,
							ItemCode = productModel.ItemCode,
							ItemName = productModel.ItemName,
							UnitsetReferenceId = productModel.UnitsetReferenceId,
							UnitsetCode = productModel.UnitsetCode,
							UnitsetName = productModel.UnitsetName,
							SubUnitsetReferenceId = productModel.SubUnitsetReferenceId,
							SubUnitsetCode = productModel.SubUnitsetCode,
							SubUnitsetName = productModel.SubUnitsetName,
							MainItemReferenceId = productModel.ItemReferenceId,
							MainItemCode = productModel.ItemCode,
							MainItemName = productModel.ItemName,
							StockQuantity = default,
							IsSelected = false,
							IsVariant = productModel.IsVariant,
							TrackingType = productModel.TrackingType,
							LocTracking = productModel.LocTracking,
							Image = productModel.ImageData,
							Quantity = productModel.WaitingQuantity,
							OutputQuantity = productModel.LocTracking == 0 ? 1 : 0,
						};

						var itemOrders = await _waitingSalesOrderService.GetObjectsByProduct(
							httpClient: httpClient,
							firmNumber: _httpClientService.FirmNumber,
							periodNumber: _httpClientService.PeriodNumber,
							warehouseNumber: CameraScanModel.WarehouseNumber,
							productReferenceId: outputSalesBasketModelItem.ItemReferenceId,
							customerReferenceId: CameraScanModel.CurrentReferenceId,
							shipInfoReferenceId: CameraScanModel.ShipInfoReferenceId,
							skip: 0,
							take: 999999
						);

						if (itemOrders.IsSuccess)
						{
							if (itemOrders.Data is not null)
							{
								foreach (var order in itemOrders.Data)
								{
									var obj = Mapping.Mapper.Map<OutputSalesBasketOrderModel>(order);
									outputSalesBasketModelItem.Orders.Add(new OutputSalesBasketOrderModel
									{
										ReferenceId = obj.ReferenceId,
										OrderReferenceId = obj.OrderReferenceId,
										CustomerReferenceId = obj.CustomerReferenceId,
										CustomerCode = obj.CustomerCode,
										CustomerName = obj.CustomerName,
										ProductReferenceId = obj.ProductReferenceId,
										ProductCode = obj.ProductCode,
										ProductName = obj.ProductName,
										UnitsetReferenceId = obj.UnitsetReferenceId,
										UnitsetCode = obj.UnitsetCode,
										UnitsetName = obj.UnitsetName,
										SubUnitsetReferenceId = obj.SubUnitsetReferenceId,
										SubUnitsetCode = obj.SubUnitsetCode,
										SubUnitsetName = obj.SubUnitsetName,
										Quantity = obj.Quantity,
										ShippedQuantity = obj.ShippedQuantity,
										WaitingQuantity = obj.WaitingQuantity,
										OrderDate = obj.OrderDate,
										DueDate = obj.DueDate,
										Price = obj.Price,
										Vat = obj.Vat,
									});

								}
							}
						}
						outputProductSalesOrderProcessBasketListViewModel.Items.Add(outputSalesBasketModelItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}

					break;
			}
		}
		catch (Exception ex)
		{

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}
	#endregion

	#region BarcodePurchase Send Basket Functions
	private async Task SendPurchaseProductBasketPageAsync(BarcodePurchaseProductModel productModel)
	{
		try
		{
			switch (CameraScanModel.ComingPage)
			{
				case "InputProductPurchaseOrderProcessBasketListViewModel":
					var httpClient = _httpClientService.GetOrCreateHttpClient();
					var inputProductPurchaseOrderProcessBasketListViewModel = _serviceProvider.GetRequiredService<InputProductPurchaseOrderProcessBasketListViewModel>();
					var existingItem = inputProductPurchaseOrderProcessBasketListViewModel.Items.FirstOrDefault(x => x.ItemCode == productModel.ItemCode);
					if (existingItem is not null)
					{
						var existingItemFullOrders = await _waitingPurchaseOrderService.GetObjectsByProduct(
							httpClient: httpClient,
							firmNumber: _httpClientService.FirmNumber,
							periodNumber: _httpClientService.PeriodNumber,
							warehouseNumber: CameraScanModel.WarehouseNumber,
							supplierReferenceId: CameraScanModel.CurrentReferenceId,
							productReferenceId: existingItem.ItemReferenceId,
							skip: 0,
							take: 999999,
							search: ""
						);

						if (existingItemFullOrders.IsSuccess && existingItemFullOrders.Data is not null)
						{
							foreach (var order in existingItemFullOrders.Data)
							{
								var obj = Mapping.Mapper.Map<InputPurchaseBasketOrderModel>(order);
								if (!existingItem.Orders.Any(x => x.ReferenceId == obj.ReferenceId))
								{
									existingItem.Orders.Add(new InputPurchaseBasketOrderModel
									{
										ReferenceId = obj.ReferenceId,
										OrderReferenceId = obj.OrderReferenceId,
										SupplierReferenceId = obj.SupplierReferenceId,
										SupplierCode = obj.SupplierCode,
										SupplierName = obj.SupplierName,
										ProductReferenceId = obj.ProductReferenceId,
										ProductCode = obj.ProductCode,
										ProductName = obj.ProductName,
										UnitsetReferenceId = obj.UnitsetReferenceId,
										UnitsetCode = obj.UnitsetCode,
										UnitsetName = obj.UnitsetName,
										SubUnitsetReferenceId = obj.SubUnitsetReferenceId,
										SubUnitsetCode = obj.SubUnitsetCode,
										SubUnitsetName = obj.SubUnitsetName,
										Quantity = obj.Quantity,
										ShippedQuantity = obj.ShippedQuantity,
										WaitingQuantity = obj.WaitingQuantity,
										OrderDate = obj.OrderDate,
										DueDate = obj.DueDate
									});
								}
							}
						}
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					}
					else
					{
						var inputPurchaseBasketModelItem = new InputPurchaseBasketModel
						{
							ItemReferenceId = productModel.ItemReferenceId,
							ItemCode = productModel.ItemCode,
							ItemName = productModel.ItemName,
							UnitsetReferenceId = productModel.UnitsetReferenceId,
							UnitsetCode = productModel.UnitsetCode,
							UnitsetName = productModel.UnitsetName,
							SubUnitsetReferenceId = productModel.SubUnitsetReferenceId,
							SubUnitsetCode = productModel.SubUnitsetCode,
							SubUnitsetName = productModel.SubUnitsetName,
							MainItemReferenceId = productModel.ItemReferenceId,
							MainItemCode = productModel.ItemCode,
							MainItemName = productModel.ItemName,
							StockQuantity = default,
							IsSelected = false,
							IsVariant = productModel.IsVariant,
							TrackingType = productModel.TrackingType,
							LocTracking = productModel.LocTracking,
							Image = productModel.ImageData,
							Quantity = productModel.WaitingQuantity,
							InputQuantity = productModel.LocTracking == 0 ? 1 : 0,
						};

						var itemOrders = await _waitingPurchaseOrderService.GetObjectsByProduct(
							httpClient: httpClient,
							firmNumber: _httpClientService.FirmNumber,
							periodNumber: _httpClientService.PeriodNumber,
							warehouseNumber: CameraScanModel.WarehouseNumber,
							productReferenceId: inputPurchaseBasketModelItem.ItemReferenceId,
							supplierReferenceId: CameraScanModel.CurrentReferenceId,
							skip: 0,
							take: 999999
						);

						if (itemOrders.IsSuccess)
						{
							if (itemOrders.Data is not null)
							{
								foreach (var order in itemOrders.Data)
								{
									var obj = Mapping.Mapper.Map<InputPurchaseBasketOrderModel>(order);
									inputPurchaseBasketModelItem.Orders.Add(new InputPurchaseBasketOrderModel
									{
										ReferenceId = obj.ReferenceId,
										OrderReferenceId = obj.OrderReferenceId,
										SupplierReferenceId = obj.SupplierReferenceId,
										SupplierCode = obj.SupplierCode,
										SupplierName = obj.SupplierName,
										ProductReferenceId = obj.ProductReferenceId,
										ProductCode = obj.ProductCode,
										ProductName = obj.ProductName,
										UnitsetReferenceId = obj.UnitsetReferenceId,
										UnitsetCode = obj.UnitsetCode,
										UnitsetName = obj.UnitsetName,
										SubUnitsetReferenceId = obj.SubUnitsetReferenceId,
										SubUnitsetCode = obj.SubUnitsetCode,
										SubUnitsetName = obj.SubUnitsetName,
										Quantity = obj.Quantity,
										ShippedQuantity = obj.ShippedQuantity,
										WaitingQuantity = obj.WaitingQuantity,
										OrderDate = obj.OrderDate,
										DueDate = obj.DueDate,
									});

								}
							}
						}
						inputProductPurchaseOrderProcessBasketListViewModel.Items.Add(inputPurchaseBasketModelItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}

					break;
			}
		}
		catch (Exception ex)
		{

			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}
	#endregion



	private async Task<InputProductBasketModel> ConvertInputProductBasketAsync(BarcodeInProductModel productModel)
	{

		try
		{
			return await Task.Run(() =>
			{

				var basketItem = new InputProductBasketModel
				{
					ItemReferenceId = productModel.ReferenceId,
					ItemCode = productModel.Code,
					ItemName = productModel.Name,
					Image = productModel.ImageData,
					UnitsetReferenceId = productModel.UnitsetReferenceId,
					UnitsetCode = productModel.UnitsetCode,
					UnitsetName = productModel.UnitsetName,
					SubUnitsetReferenceId = productModel.SubUnitsetReferenceId,
					SubUnitsetCode = productModel.SubUnitsetCode,
					SubUnitsetName = productModel.SubUnitsetName,
					IsSelected = false,
					MainItemCode = string.Empty,
					MainItemName = string.Empty,
					MainItemReferenceId = default,
					StockQuantity = productModel.StockQuantity,
					Quantity = productModel.LocTracking == 0 ? 1 : 0,
					LocTracking = productModel.LocTracking,
					TrackingType = productModel.TrackingType,
					IsVariant = productModel.IsVariant,
					ConversionFactor = 1,
					OtherConversionFactor = 1,
					VariantIcon = productModel.VariantIcon,
					LocTrackingIcon = productModel.LocTrackingIcon,
					TrackingTypeIcon = productModel.TrackingTypeIcon
				};

				return basketItem;
			});
		}
		catch (System.Exception)
		{

			throw;
		}

	}

	private async Task<OutputProductBasketModel> ConvertOutputProductBasketAsync(BarcodeOutProductModel productModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new OutputProductBasketModel
				{
					ItemReferenceId = productModel.ProductReferenceId,
					ItemCode = productModel.ProductCode,
					ItemName = productModel.ProductName,
					Image = productModel.ImageData,
					UnitsetReferenceId = productModel.UnitsetReferenceId,
					UnitsetCode = productModel.UnitsetCode,
					UnitsetName = productModel.UnitsetName,
					SubUnitsetReferenceId = productModel.SubUnitsetReferenceId,
					SubUnitsetCode = productModel.SubUnitsetCode,
					SubUnitsetName = productModel.SubUnitsetName,
					IsSelected = false,
					MainItemCode = string.Empty,
					MainItemName = string.Empty,
					MainItemReferenceId = default,
					StockQuantity = productModel.StockQuantity,
					Quantity = productModel.LocTracking == 0 ? 1 : 0,
					LocTracking = productModel.LocTracking,
					TrackingType = productModel.TrackingType,
					IsVariant = productModel.IsVariant,
					VariantIcon = productModel.VariantIcon,
					LocTrackingIcon = productModel.LocTrackingIcon,
					TrackingTypeIcon = productModel.TrackingTypeIcon,
					ConversionFactor = 1,
					OtherConversionFactor = 1,
				};

				return basketItem;
			});
		}
		catch (System.Exception ex)
		{
			throw;
		}

	}

	private async Task<OutProductModel> ConvertOutProductAsync(BarcodeOutProductModel productModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new OutProductModel
				{
					ReferenceId = Guid.NewGuid(),
					ItemReferenceId = productModel.ProductReferenceId,
					ItemCode = productModel.ProductCode,
					ItemName = productModel.ProductName,
					Image = productModel.Image,
					UnitsetReferenceId = productModel.UnitsetReferenceId,
					UnitsetCode = productModel.UnitsetCode,
					UnitsetName = productModel.UnitsetName,
					SubUnitsetReferenceId = productModel.SubUnitsetReferenceId,
					SubUnitsetCode = productModel.SubUnitsetCode,
					SubUnitsetName = productModel.SubUnitsetName,
					StockQuantity = productModel.StockQuantity,
					IsVariant = productModel.IsVariant,
					LocTracking = productModel.LocTracking,
					TrackingType = productModel.TrackingType,
					LocTrackingIcon = productModel.LocTrackingIcon,
					VariantIcon = productModel.VariantIcon,
					TrackingTypeIcon = productModel.TrackingTypeIcon,
					OutputQuantity = productModel.LocTracking == 0 ? 1 : 0,
					IsSelected = false,
					MainItemCode = string.Empty,
					MainItemName = string.Empty,
					MainItemReferenceId = default,
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{
			throw;
		}
	}

	private async Task<OutputSalesBasketModel> ConvertOutputSalesBasketAsync(BarcodeOutProductModel productModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new OutputSalesBasketModel
				{
					ReferenceId = Guid.NewGuid(),
					ItemReferenceId = productModel.ProductReferenceId,
					ItemCode = productModel.ProductCode,
					ItemName = productModel.ProductName,
					Image = productModel.ImageData,
					UnitsetReferenceId = productModel.UnitsetReferenceId,
					UnitsetCode = productModel.UnitsetCode,
					UnitsetName = productModel.UnitsetName,
					SubUnitsetReferenceId = productModel.SubUnitsetReferenceId,
					SubUnitsetCode = productModel.SubUnitsetCode,
					SubUnitsetName = productModel.SubUnitsetName,
					MainItemReferenceId = default,  //
					MainItemCode = string.Empty,    //
					MainItemName = string.Empty,    //
					StockQuantity = productModel.StockQuantity,
					IsSelected = false,   //
					IsVariant = productModel.IsVariant,
					LocTracking = productModel.LocTracking,
					TrackingType = productModel.TrackingType,
					Quantity = productModel.LocTracking == 0 ? 1 : 0,
					OutputQuantity = productModel.LocTracking == 0 ? 1 : 0,
					LocTrackingIcon = productModel.LocTrackingIcon,
					VariantIcon = productModel.VariantIcon,
					TrackingTypeIcon = productModel.TrackingTypeIcon,
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task<InputPurchaseBasketModel> ConvertInputPurchaseBasketAsync(BarcodeInProductModel productModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new InputPurchaseBasketModel
				{
					ReferenceId = Guid.NewGuid(),
					ItemReferenceId = productModel.ReferenceId,
					ItemCode = productModel.Code,
					ItemName = productModel.Name,
					Image = productModel.ImageData,
					UnitsetReferenceId = productModel.UnitsetReferenceId,
					UnitsetCode = productModel.UnitsetCode,
					UnitsetName = productModel.UnitsetName,
					SubUnitsetReferenceId = productModel.SubUnitsetReferenceId,
					SubUnitsetCode = productModel.SubUnitsetCode,
					SubUnitsetName = productModel.SubUnitsetName,
					IsSelected = false,
					MainItemCode = string.Empty,
					MainItemName = string.Empty,
					MainItemReferenceId = default,
					StockQuantity = productModel.StockQuantity,
					Quantity = productModel.LocTracking == 0 ? 1 : 0,
					InputQuantity = productModel.LocTracking == 0 ? 1 : 0,
					LocTracking = productModel.LocTracking,
					TrackingType = productModel.TrackingType,
					IsVariant = productModel.IsVariant,
					VariantIcon = productModel.VariantIcon,
					LocTrackingIcon = productModel.LocTrackingIcon,
					TrackingTypeIcon = productModel.TrackingTypeIcon,
					ConversionFactor = 1,
					OtherConversionFactor = 1,
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task<ReturnPurchaseBasketModel> ConvertReturnPurchaseBasketAsync(BarcodeOutProductModel productModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new ReturnPurchaseBasketModel
				{
					ReferenceId = Guid.NewGuid(),
					ItemReferenceId = productModel.ProductReferenceId,
					ItemCode = productModel.ProductCode,
					ItemName = productModel.ProductName,
					Image = productModel.ImageData,
					UnitsetReferenceId = productModel.UnitsetReferenceId,
					UnitsetCode = productModel.UnitsetCode,
					UnitsetName = productModel.UnitsetName,
					SubUnitsetReferenceId = productModel.SubUnitsetReferenceId,
					SubUnitsetCode = productModel.SubUnitsetCode,
					SubUnitsetName = productModel.SubUnitsetName,
					MainItemReferenceId = default,  //
					MainItemCode = string.Empty,    //
					MainItemName = string.Empty,    //
					StockQuantity = productModel.StockQuantity,
					IsSelected = false,   //
					IsVariant = productModel.IsVariant,
					LocTracking = productModel.LocTracking,
					TrackingType = productModel.TrackingType,
					Quantity = productModel.LocTracking == 0 ? 1 : 0,
					LocTrackingIcon = productModel.LocTrackingIcon,
					VariantIcon = productModel.VariantIcon,
					TrackingTypeIcon = productModel.TrackingTypeIcon,
					ConversionFactor = 1,
					OtherConversionFactor = 1
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task<ReturnSalesBasketModel> ConvertReturnSalesBasketAsync(BarcodeInProductModel productModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new ReturnSalesBasketModel
				{
					ReferenceId = Guid.NewGuid(),
					ItemReferenceId = productModel.ReferenceId,
					ItemCode = productModel.Code,
					ItemName = productModel.Name,
					Image = productModel.ImageData,
					UnitsetReferenceId = productModel.UnitsetReferenceId,
					UnitsetCode = productModel.UnitsetCode,
					UnitsetName = productModel.UnitsetName,
					SubUnitsetReferenceId = productModel.SubUnitsetReferenceId,
					SubUnitsetCode = productModel.SubUnitsetCode,
					SubUnitsetName = productModel.SubUnitsetName,
					IsSelected = false,
					MainItemCode = string.Empty,
					MainItemName = string.Empty,
					MainItemReferenceId = default,
					StockQuantity = productModel.StockQuantity,
					Quantity = productModel.LocTracking == 0 ? 1 : 0,
					InputQuantity = productModel.LocTracking == 0 ? 1 : 0,
					LocTracking = productModel.LocTracking,
					TrackingType = productModel.TrackingType,
					IsVariant = productModel.IsVariant,
					LocTrackingIcon = productModel.LocTrackingIcon,
					VariantIcon = productModel.VariantIcon,
					TrackingTypeIcon = productModel.TrackingTypeIcon,
					OtherConversionFactor = 1,
					ConversionFactor = 1,
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{

			throw;
		}
	}
	private async Task<DemandProcessBasketModel> ConvertDemandProcessBasketAsync(BarcodeDemandProductModel productModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new DemandProcessBasketModel
				{
					ReferenceId = Guid.NewGuid(),
					ItemReferenceId = productModel.ProductReferenceId,
					ItemCode = productModel.ProductCode,
					ItemName = productModel.ProductName,
					Image = productModel.ImageData,
					UnitsetReferenceId = productModel.UnitsetReferenceId,
					UnitsetCode = productModel.UnitsetCode,
					UnitsetName = productModel.UnitsetName,
					SubUnitsetReferenceId = productModel.SubUnitsetReferenceId,
					SubUnitsetCode = productModel.SubUnitsetCode,
					SubUnitsetName = productModel.SubUnitsetName,
					MainItemReferenceId = default,  //
					MainItemCode = string.Empty,    //
					MainItemName = string.Empty,    //
					StockQuantity = productModel.StockQuantity,
					IsSelected = false,   //
					IsVariant = productModel.IsVariant,
					LocTracking = productModel.LocTracking,
					TrackingType = productModel.TrackingType,
					SafeLevel = productModel.SafeLevel,
					Quantity = productModel.SafeLevel - productModel.StockQuantity,
					LocTrackingIcon = productModel.LocTrackingIcon,
					VariantIcon = productModel.VariantIcon,
					TrackingTypeIcon = productModel.TrackingTypeIcon,
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{

			throw;
		}
	}
	private async Task<OutputOutsourceTransferBasketModel> ConvertOutputOutsourceTransferBasketAsync(BarcodeOutProductModel productModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new OutputOutsourceTransferBasketModel
				{
					ReferenceId = Guid.NewGuid(),
					ItemReferenceId = productModel.ProductReferenceId,
					ItemCode = productModel.ProductCode,
					ItemName = productModel.ProductName,
					Image = productModel.ImageData,
					UnitsetReferenceId = productModel.UnitsetReferenceId,
					UnitsetCode = productModel.UnitsetCode,
					UnitsetName = productModel.UnitsetName,
					SubUnitsetReferenceId = productModel.SubUnitsetReferenceId,
					SubUnitsetCode = productModel.SubUnitsetCode,
					SubUnitsetName = productModel.SubUnitsetName,
					MainItemReferenceId = default,  //
					MainItemCode = string.Empty,    //
					MainItemName = string.Empty,    //
					StockQuantity = productModel.StockQuantity,
					IsSelected = false,   //
					IsVariant = productModel.IsVariant,
					LocTracking = productModel.LocTracking,
					TrackingType = productModel.TrackingType,
					Quantity = productModel.LocTracking == 0 ? 1 : 0,
					LocTrackingIcon = productModel.LocTrackingIcon,
					VariantIcon = productModel.VariantIcon,
					TrackingTypeIcon = productModel.TrackingTypeIcon,
					
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{
			throw;
		}
	}
	private async Task<InputOutsourceTransferBasketModel> ConvertInputOutsourceTransferBasketAsync(BarcodeInProductModel productModel)
	{

		try
		{
			return await Task.Run(() =>
			{

				var basketItem = new InputOutsourceTransferBasketModel
				{
					ReferenceId = Guid.NewGuid(),
					ItemReferenceId = productModel.ReferenceId,
					ItemCode = productModel.Code,
					ItemName = productModel.Name,
					Image = productModel.ImageData,
					UnitsetReferenceId = productModel.UnitsetReferenceId,
					UnitsetCode = productModel.UnitsetCode,
					UnitsetName = productModel.UnitsetName,
					SubUnitsetReferenceId = productModel.SubUnitsetReferenceId,
					SubUnitsetCode = productModel.SubUnitsetCode,
					SubUnitsetName = productModel.SubUnitsetName,
					IsSelected = false,
					MainItemCode = string.Empty,
					MainItemName = string.Empty,
					MainItemReferenceId = default,
					StockQuantity = productModel.StockQuantity,
					Quantity = productModel.LocTracking == 0 ? 1 : 0,
					LocTracking = productModel.LocTracking,
					TrackingType = productModel.TrackingType,
					IsVariant = productModel.IsVariant,
					VariantIcon = productModel.VariantIcon,
					LocTrackingIcon = productModel.LocTrackingIcon,
					TrackingTypeIcon = productModel.TrackingTypeIcon,
				};

				return basketItem;
			});
		}
		catch (System.Exception)
		{

			throw;
		}

	}
	private async Task<WarehouseCountingBasketModel> ConvertWarehouseCountingBasketAsync(BarcodeInProductModel productModel)
	{
		try
		{
			return await Task.Run(() =>
			{

				var basketItem = new WarehouseCountingBasketModel
				{
					ItemReferenceId = productModel.ReferenceId,
					ItemCode = productModel.Code,
					ItemName = productModel.Name,
					MainItemReferenceId = 0,
					MainItemCode = "",
					MainItemName = "",
					IsVariant = productModel.IsVariant,
					LocTracking = productModel.LocTracking,
					TrackingType = productModel.TrackingType,
					SubUnitsetCode = productModel.SubUnitsetCode,
					SubUnitsetReferenceId = productModel.SubUnitsetReferenceId,
					SubUnitsetName = productModel.SubUnitsetName,
					UnitsetReferenceId = productModel.UnitsetReferenceId,
					UnitsetCode = productModel.UnitsetCode,
					UnitsetName = productModel.UnitsetName,
					ConversionFactor = 1,
					OtherConversionFactor = 1,
					StockQuantity = productModel.StockQuantity,
					OutputQuantity = productModel.StockQuantity,
					Image = productModel.Image,
				};

				return basketItem;
			});
		}
		catch (System.Exception)
		{

			throw;
		}
	}


	private async Task<InputProductBasketModel> ConvertInputProductBasketAsync(VariantModel variantModel)
	{

		try
		{
			return await Task.Run(() =>
			{

				var basketItem = new InputProductBasketModel
				{
					ItemReferenceId = variantModel.ProductReferenceId,
					ItemCode = variantModel.ProductCode,
					ItemName = variantModel.ProductName,
					Image = variantModel.ImageData,
					UnitsetReferenceId = variantModel.UnitsetReferenceId,
					UnitsetCode = variantModel.UnitsetCode,
					UnitsetName = variantModel.UnitsetName,
					SubUnitsetReferenceId = variantModel.SubUnitsetReferenceId,
					SubUnitsetCode = variantModel.SubUnitsetCode,
					SubUnitsetName = variantModel.SubUnitsetName,
					IsSelected = false,
					MainItemCode = variantModel.Code,
					MainItemName = variantModel.Name,
					MainItemReferenceId = variantModel.ReferenceId,
					StockQuantity = variantModel.StockQuantity,
					Quantity = variantModel.LocTracking == 0 ? 1 : 0,
					LocTracking = variantModel.LocTracking,
					TrackingType = variantModel.TrackingType,
					IsVariant = true,
					//VariantIcon = variantModel.VariantIcon,
					//LocTrackingIcon = variantModel.LocTrackingIcon,
					//TrackingTypeIcon = variantModel.TrackingTypeIcon
				};

				return basketItem;
			});



		}
		catch (System.Exception)
		{

			throw;
		}

	}
	
	private async Task<OutputProductBasketModel> ConvertOutputProductBasketAsync(VariantModel variantModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new OutputProductBasketModel
				{
					ItemReferenceId = variantModel.ProductReferenceId,
					ItemCode = variantModel.ProductCode,
					ItemName = variantModel.ProductName,
					//Image = variantModel.ImageData,
					UnitsetReferenceId = variantModel.UnitsetReferenceId,
					UnitsetCode = variantModel.UnitsetCode,
					UnitsetName = variantModel.UnitsetName,
					SubUnitsetReferenceId = variantModel.SubUnitsetReferenceId,
					SubUnitsetCode = variantModel.SubUnitsetCode,
					SubUnitsetName = variantModel.SubUnitsetName,
					IsSelected = false,
					MainItemCode = variantModel.Code,
					MainItemName = variantModel.Name,
					MainItemReferenceId = variantModel.ReferenceId,
					StockQuantity = variantModel.StockQuantity,
					Quantity = variantModel.LocTracking == 0 ? 1 : 0,
					LocTracking = variantModel.LocTracking,
					TrackingType = variantModel.TrackingType,
					IsVariant = true,
					//VariantIcon = variantModel.VariantIcon,
					//LocTrackingIcon = variantModel.LocTrackingIcon,
					//TrackingTypeIcon = variantModel.TrackingTypeIcon
				};

				return basketItem;
			});
		}
		catch (System.Exception ex)
		{
			throw;
		}

	}
	
	private async Task<OutProductModel> ConvertOutProductAsync(VariantModel variantModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new OutProductModel
				{
					ItemReferenceId = variantModel.ReferenceId,
					ItemCode = variantModel.Code,
					ItemName = variantModel.Name,
					MainItemReferenceId = variantModel.ProductReferenceId,
					MainItemCode = variantModel.ProductCode,
					MainItemName = variantModel.ProductName,
                   // Image = variantModel.ImageData,
					//VatRate = variantModel.VatRate,
					UnitsetReferenceId = variantModel.UnitsetReferenceId,
					UnitsetCode = variantModel.UnitsetCode,
					UnitsetName = variantModel.UnitsetName,
					SubUnitsetReferenceId = variantModel.SubUnitsetReferenceId,
					SubUnitsetCode = variantModel.SubUnitsetCode,
					SubUnitsetName = variantModel.SubUnitsetName,
					StockQuantity = variantModel.StockQuantity,
					IsVariant = true,
					LocTracking = variantModel.LocTracking,
					TrackingType = variantModel.TrackingType,
					//LocTrackingIcon = variantModel.LocTrackingIcon,
					//VariantIcon = variantModel.VariantIcon,
					//TrackingTypeIcon = variantModel.TrackingTypeIcon,
					OutputQuantity = variantModel.LocTracking == 0 ? 1 : 0,
					IsSelected = false,
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{
			throw;
		}
	}
	
	private async Task<OutputSalesBasketModel> ConvertOutputSalesBasketAsync(VariantModel variantModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new OutputSalesBasketModel
				{
					ItemReferenceId = variantModel.ProductReferenceId,
					ItemCode = variantModel.ProductCode,
					ItemName = variantModel.ProductName,
					//Image = variantModel.ImageData,
					UnitsetReferenceId = variantModel.UnitsetReferenceId,
					UnitsetCode = variantModel.UnitsetCode,
					UnitsetName = variantModel.UnitsetName,
					SubUnitsetReferenceId = variantModel.SubUnitsetReferenceId,
					SubUnitsetCode = variantModel.SubUnitsetCode,
					SubUnitsetName = variantModel.SubUnitsetName,
					MainItemReferenceId = variantModel.ReferenceId,  
					MainItemCode = variantModel.Code,    
					MainItemName = variantModel.Name,    
					StockQuantity = variantModel.StockQuantity,
					IsSelected = false,   
					IsVariant = true,
					LocTracking = variantModel.LocTracking,
					TrackingType = variantModel.TrackingType,
					Quantity = variantModel.LocTracking == 0 ? 1 : 0,
					OutputQuantity = variantModel.LocTracking == 0 ? 1 : 0,
					//LocTrackingIcon = variantModel.LocTrackingIcon,
					//VariantIcon = variantModel.VariantIcon,
					//TrackingTypeIcon = variantModel.TrackingTypeIcon,
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task<InputPurchaseBasketModel> ConvertInputPurchaseBasketAsync(VariantModel variantModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new InputPurchaseBasketModel
				{
					ItemReferenceId = variantModel.ProductReferenceId,
					ItemCode = variantModel.ProductCode,
					ItemName = variantModel.ProductName,
					Image = variantModel.ImageData,
					UnitsetReferenceId = variantModel.UnitsetReferenceId,
					UnitsetCode = variantModel.UnitsetCode,
					UnitsetName = variantModel.UnitsetName,
					SubUnitsetReferenceId = variantModel.SubUnitsetReferenceId,
					SubUnitsetCode = variantModel.SubUnitsetCode,
					SubUnitsetName = variantModel.SubUnitsetName,
					IsSelected = false,
					MainItemCode = variantModel.Code,
					MainItemName = variantModel.Name,
					MainItemReferenceId = variantModel.ReferenceId,
					StockQuantity = variantModel.StockQuantity,
					Quantity = variantModel.LocTracking == 0 ? 1 : 0,
					InputQuantity = variantModel.LocTracking == 0 ? 1 : 0,
					LocTracking = variantModel.LocTracking,
					TrackingType = variantModel.TrackingType,
					IsVariant = true,
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{

			throw;
		}
	}
	
	private async Task<ReturnPurchaseBasketModel> ConvertReturnPurchaseBasketAsync(VariantModel variantModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new ReturnPurchaseBasketModel
				{
					ItemReferenceId = variantModel.ProductReferenceId,
					ItemCode = variantModel.ProductCode,
					ItemName = variantModel.ProductName,
					Image = variantModel.ImageData,
					UnitsetReferenceId = variantModel.UnitsetReferenceId,
					UnitsetCode = variantModel.UnitsetCode,
					UnitsetName = variantModel.UnitsetName,
					SubUnitsetReferenceId = variantModel.SubUnitsetReferenceId,
					SubUnitsetCode = variantModel.SubUnitsetCode,
					SubUnitsetName = variantModel.SubUnitsetName,
					MainItemReferenceId = variantModel.ReferenceId,  
					MainItemCode = variantModel.Code,    
					MainItemName = variantModel.Name,    
					StockQuantity = variantModel.StockQuantity,
					IsSelected = false,   
					IsVariant = true,
					//Image = string.Empty,
					LocTracking = variantModel.LocTracking,
					TrackingType = variantModel.TrackingType,
					Quantity = variantModel.LocTracking == 0 ? 1 : 0,
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task<ReturnSalesBasketModel> ConvertReturnSalesBasketAsync(VariantModel variantModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new ReturnSalesBasketModel
				{
					ItemReferenceId = variantModel.ProductReferenceId,
					ItemCode = variantModel.ProductCode,
					ItemName = variantModel.ProductName,
					Image = variantModel.ImageData,
					UnitsetReferenceId = variantModel.UnitsetReferenceId,
					UnitsetCode = variantModel.UnitsetCode,
					UnitsetName = variantModel.UnitsetName,
					SubUnitsetReferenceId = variantModel.SubUnitsetReferenceId,
					SubUnitsetCode = variantModel.SubUnitsetCode,
					SubUnitsetName = variantModel.SubUnitsetName,
					IsSelected = false,
					MainItemCode = variantModel.Code,
					MainItemName = variantModel.Name,
					MainItemReferenceId = variantModel.ReferenceId,
					StockQuantity = variantModel.StockQuantity,
					Quantity = variantModel.LocTracking == 0 ? 1 : 0,
					InputQuantity = variantModel.LocTracking == 0 ? 1 : 0,
					LocTracking = variantModel.LocTracking,
					TrackingType = variantModel.TrackingType,
					IsVariant = true,
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{

			throw;
		}
	}
	private async Task<DemandProcessBasketModel> ConvertDemandProcessBasketAsync(VariantModel variantModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new DemandProcessBasketModel
				{
					ItemReferenceId = variantModel.ReferenceId,
					ItemCode = variantModel.Code,
					ItemName = variantModel.Name,
					//Image = variantModel.ImageData,
					UnitsetReferenceId = variantModel.UnitsetReferenceId,
					UnitsetCode = variantModel.UnitsetCode,
					UnitsetName = variantModel.UnitsetName,
					SubUnitsetReferenceId = variantModel.SubUnitsetReferenceId,
					SubUnitsetCode = variantModel.SubUnitsetCode,
					SubUnitsetName = variantModel.SubUnitsetName,
					MainItemReferenceId = variantModel.ProductReferenceId,
					MainItemCode = variantModel.ProductCode,    
					MainItemName = variantModel.ProductName,
					StockQuantity = variantModel.StockQuantity,
					IsSelected = false,   //
					IsVariant = true,
					LocTracking = variantModel.LocTracking,
					TrackingType = variantModel.TrackingType,
					//SafeLevel = item.SafeLevel,
					//Quantity = item.SafeLevel - productModel.StockQuantity,
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{

			throw;
		}
	}
	private async Task<OutputOutsourceTransferBasketModel> ConvertOutputOutsourceTransferBasketAsync(VariantModel variantModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new OutputOutsourceTransferBasketModel
				{
					ItemReferenceId = variantModel.ReferenceId,
					ItemCode = variantModel.Code,
					ItemName = variantModel.Name,
					//Image = variantModel.ImageData,
					UnitsetReferenceId = variantModel.UnitsetReferenceId,
					UnitsetCode = variantModel.UnitsetCode,
					UnitsetName = variantModel.UnitsetName,
					SubUnitsetReferenceId = variantModel.SubUnitsetReferenceId,
					SubUnitsetCode = variantModel.SubUnitsetCode,
					SubUnitsetName = variantModel.SubUnitsetName,
					MainItemReferenceId = variantModel.ProductReferenceId, 
					MainItemCode = variantModel.ProductCode,   
					MainItemName = variantModel.ProductName,    
					StockQuantity = variantModel.StockQuantity,
					IsSelected = false,   //
					IsVariant = true,
					LocTracking = variantModel.LocTracking,
					//Image = variantModel.Image,
					TrackingType = variantModel.TrackingType,
					Quantity = variantModel.LocTracking == 0 ? 1 : 0,
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{
			throw;
		}
	}
}
