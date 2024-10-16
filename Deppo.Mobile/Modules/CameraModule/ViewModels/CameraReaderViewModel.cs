using AndroidX.Lifecycle;
using CommunityToolkit.Mvvm.ComponentModel;
using Controls.UserDialogs.Maui;
using Deppo.Core.BaseModels;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BasketModels;
using Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.PurchaseModels.BasketModels;
using Deppo.Mobile.Core.Models.SalesModels.BasketModels;
using Deppo.Mobile.Core.Models.TransferModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Core.Models.WarehouseModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Helpers.MappingHelper;
using Deppo.Mobile.Helpers.MVVMHelper;
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


[QueryProperty(name: nameof(ComingPage), queryId: nameof(ComingPage))]
public partial class CameraReaderViewModel : BaseViewModel
{
	private readonly IUserDialogs _userDialogs;
	private readonly IHttpClientService _httpClientService;
	private readonly IWarehouseTotalService _warehouseTotalService;
	private readonly IProductService _productService;
	private readonly IServiceProvider _serviceProvider;
	private readonly IBarcodeSearchService _barcodeSearchService;

	[ObservableProperty]
	string comingPage = null!;

	bool isFind = false;
	public CameraReaderViewModel(IUserDialogs userDialogs, IHttpClientService httpClientService, IWarehouseTotalService warehouseTotalService, IProductService productService, IServiceProvider serviceProvider, IBarcodeSearchService barcodeSearchService)
	{
		_userDialogs = userDialogs;
		_httpClientService = httpClientService;
		_warehouseTotalService = warehouseTotalService;
		_productService = productService;
		_serviceProvider = serviceProvider;
		_barcodeSearchService = barcodeSearchService;

		BackCommand = new Command(async () => await BackAsync());
		CameraDetectedCommand = new Command<BarcodeDetectionEventArgs>(async (e) => await CameraDetectedAsync(e));
		SwitchCameraTappedCommand = new Command(async () => await SwitchCameraTappedAsync());
		FlashlightTappedCommand = new Command(async () => await FlashlightTappedAsync());

		isFind = false;

		_userDialogs = userDialogs;
		_httpClientService = httpClientService;
		_warehouseTotalService = warehouseTotalService;
		_productService = productService;
		_serviceProvider = serviceProvider;

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

	private async Task SearchBarcodeAsync(BarcodeResult[] readBarcodes)
	{
		try
		{
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var firmNumber = _httpClientService.FirmNumber;
			var periodNumber = _httpClientService.PeriodNumber;

			for (int i = 0; i < readBarcodes.Length; i++)
			{
				var first = readBarcodes[i];

				if (first is null)
				{
					await _userDialogs.AlertAsync("Barkod Bulunamadı", "Hata", "Tamam");
					return;
				}
				Task searchByProductCodeTask = SearchByProductCodeAsync(httpClient, firmNumber, periodNumber, first.Value);
				Task searchByVariantCodeTask = SearchByVariantCodeAsync(httpClient, firmNumber, periodNumber, first.Value);
				Task searchByProductMainBarcodeTask = SearchByProductMainBarcodeAsync(httpClient, firmNumber, periodNumber, first.Value);
				Task searchByVariantMainBarcodeTask = SearchByVariantMainBarcodeAsync(httpClient, firmNumber, periodNumber, first.Value);
				Task searchByProductSubBarcodeTask = SearchByProductSubBarcodeAsync(httpClient, firmNumber, periodNumber, first.Value);
				Task searchByVariantSubBarcodeTask = SearchByVariantSubBarcodeAsync(httpClient, firmNumber, periodNumber, first.Value);
				Task searchByProductSeriNumberTask = SearchByProductSeriNumberAsync(httpClient, firmNumber, periodNumber, first.Value);
				Task searchByVariantSeriNumberTask = SearchByVariantSeriNumberAsync(httpClient, firmNumber, periodNumber, first.Value);
				Task searchByProductLotNumberTask = SearchByProductLotNumberAsync(httpClient, firmNumber, periodNumber, first.Value);
				Task searchByVariantLotNumberTask = SearchByVariantLotNumberAsync(httpClient, firmNumber, periodNumber, first.Value);
				Task searchBySupplierProductCodeTask = SearchBySupplierProductCodeAsync(httpClient, firmNumber, periodNumber, first.Value);
				Task searchBySupplierVariantCodeTask = SearchBySupplierVariantCodeAsync(httpClient, firmNumber, periodNumber, first.Value);


				await Task.WhenAll(
					searchByProductCodeTask,
					searchByProductMainBarcodeTask,
					searchByProductSubBarcodeTask,
					searchByProductLotNumberTask
				);



			}
		}
		catch (System.Exception)
		{

			throw;
		}
	}

	[Obsolete("Not used")]
	private async Task ReadBarcodeAsync(BarcodeResult[] readBarcodes)
	{
		try
		{
			for (int i = 0; i < readBarcodes.Length; i++)
			{
				var first = readBarcodes[i];

				if (first is null)
				{
					await _userDialogs.AlertAsync("Barkod Bulunamadı", "Hata", "Tamam");
					return;
				}
				switch (ComingPage)
				{
					case "InputProductProcessBasket": // Üretimden Giriş, Sayım Fazlası
						await FindProductInputProductProcessBasketListAsync(first.Value);
						break;
					case "OutputProductProcessBasket": // Sarf, Sayım Eksiği, Fire Fişi
						await FindProductOutputProductProcessBasketListAsync(first.Value);
						break;
					case "TransferOutBasket": // Ambar Transferi
						await FindProductTransferOutBasketListAsync(first.Value);
						break;
					case "OutputProductSalesProcessBasket": // Sevk İşlemi
						await FindProductOutputProductSalesProcessBasketListAsync(first.Value);
						break;
					case "InputProductPurchaseProcessBasket": // Mal Kabul İşlemi
						await FindProductInputProductPurchaseProcessBasketListAsync(first.Value);
						break;
					case "ReturnPurchaseBasket": // Satınalma İade İşlemi
						await FindProductReturnPurchaseBasketListAsync(first.Value);
						break;
					case "ReturnSalesBasket": // Satış İade İşlemi
						await FindProductReturnSalesBasketListAsync(first.Value);
						break;
				}
			}
		}
		catch (Exception)
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

			await SearchBarcodeAsync(e.Results);
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



	private async Task SearchByProductCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
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
						var productModelList = JsonConvert.DeserializeObject<List<ProductModel>>(jsonString);

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

	private async Task SearchByVariantCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
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

	private async Task SearchByProductMainBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
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
						var productModelList = JsonConvert.DeserializeObject<List<ProductModel>>(jsonString);

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

	private async Task SearchByVariantMainBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
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

	private async Task SearchByProductSubBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
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
						var productModelList = JsonConvert.DeserializeObject<List<ProductModel>>(jsonString);

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

	private async Task SearchByVariantSubBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
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

	private async Task SearchByProductSeriNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
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
						var productModelList = JsonConvert.DeserializeObject<List<ProductModel>>(jsonString);

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

	private async Task SearchByVariantSeriNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
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

	private async Task SearchByProductLotNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
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
						var productModelList = JsonConvert.DeserializeObject<List<ProductModel>>(jsonString);

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

	private async Task SearchByVariantLotNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
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

	private async Task SearchBySupplierProductCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchService.SearchBySupplierProductCode(httpClient, firmNumber, periodNumber, barcode);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<ProductModel>>(jsonString);

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

	private async Task SearchBySupplierVariantCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
	{
		try
		{
			if (!isFind)
			{
				var result = await _barcodeSearchService.SearchBySupplierVariantCode(httpClient, firmNumber, periodNumber, barcode);

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




	private async Task SendProductBasketPageAsync(ProductModel productModel)
	{
		try
		{
			switch (ComingPage)
			{
				case "InputProductProcessBasket":
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
				case "OutputProductProcessBasket":
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
				case "TransferOutBasket":
					var transferOutBasketViewModel = _serviceProvider.GetRequiredService<TransferOutBasketViewModel>();
					var outProductItem = await ConvertOutProductAsync(productModel);
					if(transferOutBasketViewModel.TransferBasketModel.OutProducts.Any(x => x.ItemCode == outProductItem.ItemCode))
					{
						_userDialogs.ShowToast($"Ürün Sepette Zaten Var");
					} else
					{
						transferOutBasketViewModel.TransferBasketModel.OutProducts.Add(outProductItem);
						_userDialogs.ShowToast($"Ürün Sepete Eklendi");
					}
					break;
				case "OutputProductSalesProcessBasket":
					var outputProductSalesProcessBasketListViewModel = _serviceProvider.GetRequiredService<OutputProductSalesProcessBasketListViewModel>();
					var outputSalesBasketItem = await ConvertOutputSalesBasketAsync(productModel);
					if(outputProductSalesProcessBasketListViewModel.Items.Any(x => x.ItemCode == outputSalesBasketItem.ItemCode))
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
				case "ReturnPurchaseBasket":
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
				case "ReturnSalesBasket":
					var returnSalesBasketViewModel = _serviceProvider.GetRequiredService<ReturnSalesBasketViewModel>();
					var returnSalesBasketItem = await ConvertReturnSalesBasketAsync(productModel);
					if(returnSalesBasketViewModel.Items.Any(x => x.ItemCode == returnSalesBasketItem.ItemCode))
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
					var inputPurchaseOrderBasketItem = await ConvertInputPurchaseBasketAsync(productModel);
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
					var outputSalesOrderBasketItem = await ConvertOutputSalesBasketAsync(productModel);
					if(outputProductSalesOrderProcessBasketListViewModel.Items.Any(x => x.ItemCode == outputSalesOrderBasketItem.ItemCode))
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
				case "OutputOutsourceTransferBasket":
					var outputOutsourceTransferBasketViewModel = _serviceProvider.GetRequiredService<OutputOutsourceTransferBasketListViewModel>();
					var outputOutsourceTransferBasketItem = await ConvertOutputOutsourceTransferBasketAsync(productModel);
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

			isFind = false;
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}

	private async Task SendVariantBasketPageAsync(VariantModel variantModel)
	{
		try
		{
			switch (ComingPage)
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

			isFind = false;
		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
				_userDialogs.HideHud();

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}



	private async Task<InputProductBasketModel> ConvertInputProductBasketAsync(ProductModel productModel)
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

	private async Task<OutputProductBasketModel> ConvertOutputProductBasketAsync(ProductModel productModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new OutputProductBasketModel
				{
					ItemReferenceId = productModel.ReferenceId,
					ItemCode = productModel.Code,
					ItemName = productModel.Name,
					//Image = productModel.ImageData,
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
					TrackingTypeIcon = productModel.TrackingTypeIcon
				};

				return basketItem;
			});
		}
		catch (System.Exception ex)
		{
			throw;
		}

	}

	private async Task<OutProductModel> ConvertOutProductAsync(ProductModel productModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new OutProductModel
				{
					//ReferenceId = productModel.ReferenceId,
					ItemReferenceId = productModel.ReferenceId,
					ItemCode = productModel.Code,
					ItemName = productModel.Name,
					//Image = productModel.ImageData,
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
					IsSelected = productModel.IsSelected,
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{
			throw;
		}
	}

	private async Task<OutputSalesBasketModel> ConvertOutputSalesBasketAsync(ProductModel productModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new OutputSalesBasketModel
				{
					ItemReferenceId = productModel.ReferenceId,
					ItemCode = productModel.Code,
					ItemName = productModel.Name,
					//Image = productModel.ImageData,
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

	private async Task<InputPurchaseBasketModel> ConvertInputPurchaseBasketAsync(ProductModel productModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new InputPurchaseBasketModel
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
					InputQuantity = productModel.LocTracking == 0 ? 1 : 0,
					LocTracking = productModel.LocTracking,
					TrackingType = productModel.TrackingType,
					IsVariant = productModel.IsVariant,
					VariantIcon = productModel.VariantIcon,
					LocTrackingIcon = productModel.LocTrackingIcon,
					TrackingTypeIcon = productModel.TrackingTypeIcon
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task<ReturnPurchaseBasketModel> ConvertReturnPurchaseBasketAsync(ProductModel productModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new ReturnPurchaseBasketModel
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

	private async Task<ReturnSalesBasketModel> ConvertReturnSalesBasketAsync(ProductModel productModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new ReturnSalesBasketModel
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
					InputQuantity = productModel.LocTracking == 0 ? 1 : 0,
					LocTracking = productModel.LocTracking,
					TrackingType = productModel.TrackingType,
					IsVariant = productModel.IsVariant,
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
	private async Task<DemandProcessBasketModel> ConvertDemandProcessBasketAsync(ProductModel productModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new DemandProcessBasketModel
				{
					ItemReferenceId = productModel.ReferenceId,
					ItemCode = productModel.Code,
					ItemName = productModel.Name,
					//Image = productModel.ImageData,
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
					//SafeLevel = item.SafeLevel,
					//Quantity = item.SafeLevel - productModel.StockQuantity,
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
	private async Task<OutputOutsourceTransferBasketModel> ConvertOutputOutsourceTransferBasketAsync(ProductModel productModel)
	{
		try
		{
			return await Task.Run(() =>
			{
				var basketItem = new OutputOutsourceTransferBasketModel
				{
					ItemReferenceId = productModel.ReferenceId,
					ItemCode = productModel.Code,
					ItemName = productModel.Name,
					//Image = productModel.ImageData,
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


	private async Task FindProductInputProductProcessBasketListAsync(string barcodeValue)
	{
		try
		{
			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var viewModel = _serviceProvider.GetRequiredService<InputProductProcessBasketListViewModel>();

			var result = await _productService.GetObjects(
				httpClient,
				_httpClientService.FirmNumber,
				_httpClientService.PeriodNumber,
				barcodeValue,
				0,
				1
			);

			if (result.IsSuccess)
			{
				if (result.Data is null)
				{
					await _userDialogs.AlertAsync("Bir hata oluştu", "Hata", "Tamam");
					return;
				}
				if (!(result.Data.Count() > 0))
				{
					_userDialogs.ShowToast($"{barcodeValue} kodlu ürün bulunamadı!");
					return;
				}

				foreach (var product in result.Data)
				{
					var item = Mapping.Mapper.Map<Product>(product);

					ProductModel productModel = new ProductModel
					{
						ReferenceId = item.ReferenceId,
						Code = item.Code,
						Name = item.Name,
						UnitsetReferenceId = item.UnitsetReferenceId,
						UnitsetCode = item.UnitsetCode,
						UnitsetName = item.UnitsetName,
						SubUnitsetReferenceId = item.SubUnitsetReferenceId,
						SubUnitsetCode = item.SubUnitsetCode,
						SubUnitsetName = item.SubUnitsetName,
						StockQuantity = item.StockQuantity,
						TrackingType = item.TrackingType,
						LocTracking = item.LocTracking,
						GroupCode = item.GroupCode,
						BrandReferenceId = item.BrandReferenceId,
						BrandCode = item.BrandCode,
						BrandName = item.BrandName,
						VatRate = item.VatRate,
						Image = item.Image,
						IsVariant = item.IsVariant,
						IsSelected = false
					};

					var basketItem = new InputProductBasketModel
					{
						ItemReferenceId = productModel.ReferenceId,
						ItemCode = productModel.Code,
						ItemName = productModel.Name,
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
						TrackingTypeIcon = productModel.TrackingTypeIcon
					};

					if (viewModel.Items.Any(x => x.ItemCode == basketItem.ItemCode))
					{
						_userDialogs.ShowToast($"{barcodeValue} kodlu Ürün Sepette Zaten Var");
					}
					else
					{
						viewModel.Items.Add(basketItem);
						_userDialogs.ShowToast($"{barcodeValue} kodlu Ürün Sepete Eklendi");
					}
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

	private async Task FindProductOutputProductProcessBasketListAsync(string barcodeValue)
	{
		try
		{
			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var viewModel = _serviceProvider.GetRequiredService<OutputProductProcessBasketListViewModel>();

			var result = await _warehouseTotalService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: viewModel.WarehouseModel.Number,
				search: barcodeValue,
				skip: 0,
				take: 1);

			if (result.IsSuccess)
			{
				if (result.Data is not null)
				{
					if (!(result.Data.Count() > 0))
					{
						_userDialogs.ShowToast($"{barcodeValue} kodlu ürün bulunamadı!");
						return;
					}

					foreach (var product in result.Data)
					{
						var item = Mapping.Mapper.Map<WarehouseTotal>(product);
						WarehouseTotalModel warehouseTotalModel = new WarehouseTotalModel
						{
							ProductReferenceId = item.ProductReferenceId,
							ProductCode = item.ProductCode,
							ProductName = item.ProductName,
							UnitsetReferenceId = item.UnitsetReferenceId,
							UnitsetCode = item.UnitsetCode,
							UnitsetName = item.UnitsetName,
							SubUnitsetReferenceId = item.SubUnitsetReferenceId,
							SubUnitsetCode = item.SubUnitsetCode,
							SubUnitsetName = item.SubUnitsetName,
							StockQuantity = item.StockQuantity,
							WarehouseReferenceId = item.WarehouseReferenceId,
							WarehouseName = item.WarehouseName,
							WarehouseNumber = item.WarehouseNumber,
							LocTracking = item.LocTracking,
							IsVariant = item.IsVariant,
							TrackingType = item.TrackingType,
							IsSelected = false,
						};

						var basketItem = new OutputProductBasketModel()
						{
							ItemReferenceId = warehouseTotalModel.ProductReferenceId,
							ItemCode = warehouseTotalModel.ProductCode,
							ItemName = warehouseTotalModel.ProductName,
							UnitsetReferenceId = warehouseTotalModel.UnitsetReferenceId,
							UnitsetCode = warehouseTotalModel.UnitsetCode,
							UnitsetName = warehouseTotalModel.UnitsetName,
							SubUnitsetReferenceId = warehouseTotalModel.SubUnitsetReferenceId,
							SubUnitsetCode = warehouseTotalModel.SubUnitsetCode,
							SubUnitsetName = warehouseTotalModel.SubUnitsetName,
							MainItemReferenceId = default,  //
							MainItemCode = string.Empty,    //
							MainItemName = string.Empty,    //
							StockQuantity = warehouseTotalModel.StockQuantity,
							IsSelected = false,   //
							IsVariant = warehouseTotalModel.IsVariant,
							LocTracking = warehouseTotalModel.LocTracking,
							TrackingType = warehouseTotalModel.TrackingType,
							Quantity = warehouseTotalModel.LocTracking == 0 ? 1 : 0
						};

						if (viewModel.Items.Any(x => x.ItemCode == basketItem.ItemCode))
						{
							_userDialogs.ShowToast($"{barcodeValue} kodlu Ürün Sepette Zaten Var");
						}
						else
						{
							viewModel.Items.Add(basketItem);
							_userDialogs.ShowToast($"{barcodeValue} kodlu Ürün Sepete Eklendi");
						}


					}
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

	private async Task FindProductTransferOutBasketListAsync(string barcodeValue)
	{
		try
		{
			var httpClient = _httpClientService.GetOrCreateHttpClient();

			var viewModel = _serviceProvider.GetRequiredService<TransferOutBasketViewModel>();

			var result = await _warehouseTotalService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: viewModel.TransferBasketModel.OutWarehouse.Number,
				search: barcodeValue,
				skip: 0,
				take: 1);

			if (result.IsSuccess)
			{
				if (result.Data is not null)
				{
					if (!(result.Data.Count() > 0))
					{
						_userDialogs.ShowToast($"{barcodeValue} kodlu ürün bulunamadı!");
						return;
					}

					foreach (var product in result.Data)
					{
						var item = Mapping.Mapper.Map<WarehouseTotalModel>(product);


						var basketItem = new OutProductModel()
						{
							ReferenceId = item.ReferenceId,
							ItemCode = item.ProductCode,
							ItemName = item.ProductName,
							ItemReferenceId = item.ProductReferenceId,
							UnitsetReferenceId = item.UnitsetReferenceId,
							UnitsetCode = item.UnitsetCode,
                            MainItemCode = string.Empty,
							MainItemReferenceId = default,
							MainItemName = string.Empty,
                            UnitsetName = item.UnitsetName,
							SubUnitsetReferenceId = item.SubUnitsetReferenceId,
							SubUnitsetCode = item.SubUnitsetCode,
							SubUnitsetName = item.SubUnitsetName,
							StockQuantity = item.StockQuantity,
							IsVariant = item.IsVariant,
							LocTracking = item.LocTracking,
							TrackingType = item.TrackingType,
							LocTrackingIcon = item.LocTrackingIcon,
							VariantIcon = item.VariantIcon,
							TrackingTypeIcon = item.TrackingTypeIcon,
							OutputQuantity = item.LocTracking == 0 ? 1 : 0,
							IsSelected = item.IsSelected,
						};

						if (viewModel.TransferBasketModel.OutProducts.Any(x => x.ItemCode == basketItem.ItemCode))
						{
							_userDialogs.ShowToast($"{barcodeValue} kodlu Ürün Sepette Zaten Var");
						}
						else
						{
							viewModel.TransferBasketModel.OutProducts.Add(basketItem);
							_userDialogs.ShowToast($"{barcodeValue} kodlu Ürün Sepete Eklendi");
						}
					}
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

	private async Task FindProductOutputProductSalesProcessBasketListAsync(string barcodeValue)
	{
		try
		{
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var viewModel = _serviceProvider.GetRequiredService<OutputProductSalesProcessBasketListViewModel>();

			var result = await _warehouseTotalService.GetObjects(
				httpClient,
				_httpClientService.FirmNumber,
				_httpClientService.PeriodNumber,
				warehouseNumber: viewModel.WarehouseModel.Number,
				search: barcodeValue,
				skip: 0,
				take: 1
			);

			if (result.IsSuccess)
			{
				if (result.Data is not null)
				{
					if (!(result.Data.Count() > 0))
					{
						_userDialogs.ShowToast($"{barcodeValue} kodlu ürün bulunamadı!");
						return;
					}

					foreach (var product in result.Data)
					{
						var item = new WarehouseTotalModel
						{
							ProductReferenceId = product.ProductReferenceId,
							ProductCode = product.ProductCode,
							ProductName = product.ProductName,
							UnitsetReferenceId = product.UnitsetReferenceId,
							UnitsetCode = product.UnitsetCode,
							UnitsetName = product.UnitsetName,
							SubUnitsetReferenceId = product.SubUnitsetReferenceId,
							SubUnitsetCode = product.SubUnitsetCode,
							SubUnitsetName = product.SubUnitsetName,
							StockQuantity = product.StockQuantity,
							WarehouseReferenceId = product.WarehouseReferenceId,
							WarehouseName = product.WarehouseName,
							WarehouseNumber = product.WarehouseNumber,
							LocTracking = product.LocTracking,
							IsVariant = product.IsVariant,
							TrackingType = product.TrackingType,
							IsSelected = false,
							LocTrackingIcon = product.LocTrackingIcon,
							VariantIcon = product.VariantIcon,
							TrackingTypeIcon = product.TrackingTypeIcon,
						};

						var basketItem = new OutputSalesBasketModel
						{
							ItemReferenceId = item.ProductReferenceId,
							ItemCode = item.ProductCode,
							ItemName = item.ProductName,
							UnitsetReferenceId = item.UnitsetReferenceId,
							UnitsetCode = item.UnitsetCode,
							UnitsetName = item.UnitsetName,
							SubUnitsetReferenceId = item.SubUnitsetReferenceId,
							SubUnitsetCode = item.SubUnitsetCode,
							SubUnitsetName = item.SubUnitsetName,
							MainItemReferenceId = default,  //
							MainItemCode = string.Empty,    //
							MainItemName = string.Empty,    //
							StockQuantity = item.StockQuantity,
							IsSelected = false,   //
							IsVariant = item.IsVariant,
							LocTracking = item.LocTracking,
							TrackingType = item.TrackingType,
							Quantity = item.LocTracking == 0 ? 1 : 0,
							LocTrackingIcon = item.LocTrackingIcon,
							VariantIcon = item.VariantIcon,
							TrackingTypeIcon = item.TrackingTypeIcon,
						};

						if (viewModel.Items.Any(x => x.ItemCode == basketItem.ItemCode))
						{
							_userDialogs.ShowToast($"{barcodeValue} kodlu Ürün Sepette Zaten Var");
						}
						else
						{
							viewModel.Items.Add(basketItem);
							_userDialogs.ShowToast($"{barcodeValue} kodlu Ürün Sepete Eklendi");
						}
					}
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

	private async Task FindProductInputProductPurchaseProcessBasketListAsync(string barcodeValue)
	{
		try
		{
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var viewModel = _serviceProvider.GetRequiredService<InputProductPurchaseProcessBasketListViewModel>();

			var result = await _productService.GetObjectsPurchaseProduct(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				search: barcodeValue,
				skip: 0,
				take: 1
			);

			if (result.IsSuccess)
			{
				if (result.Data is not null)
				{
					if (!(result.Data.Count() > 0))
					{
						_userDialogs.ShowToast($"{barcodeValue} kodlu ürün bulunamadı!");
						return;
					}

					foreach (var product in result.Data)
					{
						var item = Mapping.Mapper.Map<Product>(product);

						ProductModel productModel = new ProductModel
						{
							ReferenceId = item.ReferenceId,
							Code = item.Code,
							Name = item.Name,
							UnitsetReferenceId = item.UnitsetReferenceId,
							UnitsetCode = item.UnitsetCode,
							UnitsetName = item.UnitsetName,
							SubUnitsetReferenceId = item.SubUnitsetReferenceId,
							SubUnitsetCode = item.SubUnitsetCode,
							SubUnitsetName = item.SubUnitsetName,
							StockQuantity = item.StockQuantity,
							TrackingType = item.TrackingType,
							LocTracking = item.LocTracking,
							IsVariant = item.IsVariant,
							IsSelected = false
						};

						var basketItem = new InputPurchaseBasketModel
						{

							ItemReferenceId = productModel.ReferenceId,
							ItemCode = productModel.Code,
							ItemName = productModel.Name,
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
							IsVariant = productModel.IsVariant
						};

						if (viewModel.Items.Any(x => x.ItemCode == basketItem.ItemCode))
						{
							_userDialogs.ShowToast($"{barcodeValue} kodlu Ürün Sepette Zaten Var");
						}
						else
						{
							viewModel.Items.Add(basketItem);
							_userDialogs.ShowToast($"{barcodeValue} kodlu Ürün Sepete Eklendi");
						}

					}

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

	private async Task FindProductReturnPurchaseBasketListAsync(string barcodeValue)
	{
		try
		{
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var viewModel = _serviceProvider.GetRequiredService<ReturnPurchaseBasketViewModel>();

			var result = await _warehouseTotalService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				warehouseNumber: viewModel.WarehouseModel.Number,
				search: barcodeValue,
				skip: 0,
				take: 1
			);

			if (result.IsSuccess)
			{
				if (result.Data is not null)
				{
					if (!(result.Data.Count() > 0))
					{
						_userDialogs.ShowToast($"{barcodeValue} kodlu ürün bulunamadı!");
						return;
					}

					foreach (var product in result.Data)
					{
						var item = Mapping.Mapper.Map<WarehouseTotal>(product);

						WarehouseTotalModel warehouseTotalModel = new WarehouseTotalModel
						{
							ProductReferenceId = item.ProductReferenceId,
							ProductCode = item.ProductCode,
							ProductName = item.ProductName,
							UnitsetReferenceId = item.UnitsetReferenceId,
							UnitsetCode = item.UnitsetCode,
							UnitsetName = item.UnitsetName,
							SubUnitsetReferenceId = item.SubUnitsetReferenceId,
							SubUnitsetCode = item.SubUnitsetCode,
							SubUnitsetName = item.SubUnitsetName,
							StockQuantity = item.StockQuantity,
							WarehouseReferenceId = item.WarehouseReferenceId,
							WarehouseName = item.WarehouseName,
							WarehouseNumber = item.WarehouseNumber,
							LocTracking = item.LocTracking,
							IsVariant = item.IsVariant,
							TrackingType = item.TrackingType,
							IsSelected = false,
							LocTrackingIcon = product.LocTrackingIcon,
							VariantIcon = product.VariantIcon,
							TrackingTypeIcon = product.TrackingTypeIcon,
						};

						var basketItem = new ReturnPurchaseBasketModel
						{
							ItemReferenceId = warehouseTotalModel.ProductReferenceId,
							ItemCode = warehouseTotalModel.ProductCode,
							ItemName = warehouseTotalModel.ProductName,
							UnitsetReferenceId = warehouseTotalModel.UnitsetReferenceId,
							UnitsetCode = warehouseTotalModel.UnitsetCode,
							UnitsetName = warehouseTotalModel.UnitsetName,
							SubUnitsetReferenceId = warehouseTotalModel.SubUnitsetReferenceId,
							SubUnitsetCode = warehouseTotalModel.SubUnitsetCode,
							SubUnitsetName = warehouseTotalModel.SubUnitsetName,
							MainItemReferenceId = default,  //
							MainItemCode = string.Empty,    //
							MainItemName = string.Empty,    //
							StockQuantity = warehouseTotalModel.StockQuantity,
							IsSelected = false,   //
							IsVariant = warehouseTotalModel.IsVariant,
							LocTracking = warehouseTotalModel.LocTracking,
							TrackingType = warehouseTotalModel.TrackingType,
							Quantity = warehouseTotalModel.LocTracking == 0 ? 1 : 0,
							LocTrackingIcon = warehouseTotalModel.LocTrackingIcon,
							VariantIcon = warehouseTotalModel.VariantIcon,
							TrackingTypeIcon = warehouseTotalModel.TrackingTypeIcon,
						};

						if (viewModel.Items.Any(x => x.ItemCode == basketItem.ItemCode))
						{
							_userDialogs.ShowToast($"{barcodeValue} kodlu Ürün Sepette Zaten Var");
						}
						else
						{
							viewModel.Items.Add(basketItem);
							_userDialogs.ShowToast($"{barcodeValue} kodlu Ürün Sepete Eklendi");
						}
					}
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
	private async Task FindProductReturnSalesBasketListAsync(string barcodeValue)
	{
		try
		{
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var viewModel = _serviceProvider.GetRequiredService<ReturnSalesBasketViewModel>();

			var result = await _productService.GetObjects(
				httpClient: httpClient,
				firmNumber: _httpClientService.FirmNumber,
				periodNumber: _httpClientService.PeriodNumber,
				search: barcodeValue,
				skip: 0,
				take: 1
			);

			if (result.IsSuccess)
			{
				if (result.Data is not null)
				{
					if (!(result.Data.Count() > 0))
					{
						_userDialogs.ShowToast($"{barcodeValue} kodlu ürün bulunamadı!");
						return;
					}

					foreach (var item in result.Data)
					{
						ProductModel productModel = new ProductModel
						{
							IsSelected = false,
							BrandCode = item.BrandCode,
							BrandName = item.BrandName,
							BrandReferenceId = item.BrandReferenceId,
							Code = item.Code,
							Image = item.Image,
							IsVariant = item.IsVariant,
							LocTracking = item.LocTracking,
							Name = item.Name,
							ReferenceId = item.ReferenceId,
							StockQuantity = item.StockQuantity,
							SubUnitsetCode = item.SubUnitsetCode,
							SubUnitsetName = item.SubUnitsetName,
							SubUnitsetReferenceId = item.SubUnitsetReferenceId,
							TrackingType = item.TrackingType,
							UnitsetCode = item.UnitsetCode,
							UnitsetName = item.UnitsetName,
							UnitsetReferenceId = item.UnitsetReferenceId,
							VatRate = item.VatRate,
							GroupCode = item.GroupCode,
							LocTrackingIcon = item.LocTrackingIcon,
							VariantIcon = item.VariantIcon,
							TrackingTypeIcon = item.TrackingTypeIcon,
						};

						ReturnSalesBasketModel basketItem = new ReturnSalesBasketModel
						{
							ItemReferenceId = productModel.ReferenceId,
							ItemCode = productModel.Code,
							ItemName = productModel.Name,
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
							//Image = productModel.Image,
						};

						if (viewModel.Items.Any(x => x.ItemCode == basketItem.ItemCode))
						{
							_userDialogs.ShowToast($"{barcodeValue} kodlu Ürün Sepette Zaten Var");
						}
						else
						{
							viewModel.Items.Add(basketItem);
							_userDialogs.ShowToast($"{barcodeValue} kodlu Ürün Sepete Eklendi");
						}
					}

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

	private async Task FindProductReturnPurchaseDispatchBasketListAsync(string barcodeValue)
	{
		try
		{
			var httpClient = _httpClientService.GetOrCreateHttpClient();
			var viewModel = _serviceProvider.GetRequiredService<ReturnPurchaseDispatchBasketViewModel>();

		}
		catch (Exception ex)
		{
			if (_userDialogs.IsHudShowing)
			{
				_userDialogs.HideHud();
			}

			await _userDialogs.AlertAsync(ex.Message, "Hata", "Tamam");
		}
	}
}
