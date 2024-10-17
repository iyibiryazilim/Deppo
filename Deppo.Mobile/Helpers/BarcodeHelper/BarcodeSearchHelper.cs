using Controls.UserDialogs.Maui;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.OutsourceModels.BasketModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Mobile.Modules.OutsourceModule.OutsourceProcess.OutputOutsourceProcess.OutputOutsourceTransfer.ViewModels;
using Newtonsoft.Json;
using ZXing.Net.Maui;

namespace Deppo.Mobile.Helpers.BarcodeHelper;

public class BarcodeSearchHelper : IBarcodeSearchHelper
{
	private readonly IBarcodeSearchService _barcodeSearchService;
	private readonly IServiceProvider _serviceProvider;
	private readonly IUserDialogs _userDialogs;

	public BarcodeSearchHelper(IBarcodeSearchService barcodeSearchService, IServiceProvider serviceProvider, IUserDialogs userDialogs)
	{
		_barcodeSearchService = barcodeSearchService;
		_serviceProvider = serviceProvider;
		_userDialogs = userDialogs;

		isFind = false;
	}

	bool isFind = false;

	public async Task BarcodeDetectedAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage)
	{
		try
		{
			Task searchByProductCodeTask = SearchByProductCodeAsync(httpClient, firmNumber, periodNumber, barcode, comingPage);
			Task searchByVariantCodeTask = SearchByVariantCodeAsync(httpClient, firmNumber, periodNumber, barcode, comingPage);
			Task searchByProductMainBarcodeTask = SearchByProductMainBarcodeAsync(httpClient, firmNumber, periodNumber, barcode, comingPage);
			Task searchByVariantMainBarcodeTask = SearchByVariantMainBarcodeAsync(httpClient, firmNumber, periodNumber, barcode, comingPage);
			Task searchByProductSubBarcodeTask = SearchByProductSubBarcodeAsync(httpClient, firmNumber, periodNumber, barcode, comingPage);
			Task searchByVariantSubBarcodeTask = SearchByVariantSubBarcodeAsync(httpClient, firmNumber, periodNumber, barcode, comingPage);
			Task searchByProductSeriNumberTask = SearchByProductSeriNumberAsync(httpClient, firmNumber, periodNumber, barcode, comingPage);
			Task searchByVariantSeriNumberTask = SearchByVariantSeriNumberAsync(httpClient, firmNumber, periodNumber, barcode, comingPage);
			Task searchByProductLotNumberTask = SearchByProductLotNumberAsync(httpClient, firmNumber, periodNumber, barcode, comingPage);
			Task searchByVariantLotNumberTask = SearchByVariantLotNumberAsync(httpClient, firmNumber, periodNumber, barcode, comingPage);
			Task searchBySupplierProductCodeTask = SearchBySupplierProductCodeAsync(httpClient, firmNumber, periodNumber, barcode, comingPage);
			Task searchBySupplierVariantCodeTask = SearchBySupplierVariantCodeAsync(httpClient, firmNumber, periodNumber, barcode, comingPage);

			await Task.WhenAll(
				searchByProductCodeTask
				//searchByVariantCodeTask,
				//searchByProductMainBarcodeTask,
				//searchByVariantMainBarcodeTask,
				//searchByProductSubBarcodeTask,
				//searchByVariantSubBarcodeTask,
				//searchByProductSeriNumberTask,
				//searchByVariantSeriNumberTask,
				//searchByProductLotNumberTask,
				//searchByVariantLotNumberTask,
				//searchBySupplierProductCodeTask,
				//searchBySupplierVariantCodeTask
			);
		}
		catch (Exception ex)
		{
			throw;
		}
		finally
		{
			
		}
	}


	public async Task SendProductBasketPageAsync(ProductModel productModel, string comingPage)
	{
		try
		{
			switch (comingPage)
			{
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
						await _userDialogs.AlertAsync($"Ürün Sepete Eklendi");		
					}
					break;
				default:
					await _userDialogs.AlertAsync("Sayfa bulunamadı");
					break;
			}

			isFind = false;
		}
		catch (Exception)
		{

			throw;
		}
	}

	public async Task SendVariantBasketPageAsync(VariantModel variantModel, string comingPage)
	{
		try
		{

		}
		catch (Exception)
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
					//LocTrackingIcon = productModel.LocTrackingIcon,
					//VariantIcon = productModel.VariantIcon,
					//TrackingTypeIcon = productModel.TrackingTypeIcon,
				};

				return basketItem;
			});
		}
		catch (Exception ex)
		{
			throw;
		}
	}




	private async Task SearchByProductCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage)
	{
		try
		{
			if(!isFind) {
				var result = await _barcodeSearchService.SearchByProductCode(httpClient, firmNumber, periodNumber, barcode);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<ProductModel>>(jsonString);
						var productModel = productModelList?.First();
						isFind = true;
						await SendProductBasketPageAsync(productModel, comingPage);
					}
				}
			}
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task SearchByVariantCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage)
	{
		try
		{
			if(!isFind)
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
						//await SendVariantBasketPageAsync(variantModel);
					}
				}
			}
		}
		catch (Exception ex)
		{

			throw;
		}
	}
	private async Task SearchByProductMainBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage)
	{
		try
		{
			if(!isFind)
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
						//await SendProductBasketPageAsync(productModel);
					}
				}
			}
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task SearchByVariantMainBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage)
	{
		try
		{
			if(!isFind)
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
						//await SendVariantBasketPageAsync(variantModel);
					}
				}
			}
		}
		catch (System.Exception)
		{

			throw;
		}
	}

	private async Task SearchByProductSubBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage)
	{
		try
		{
			if(!isFind)
			{
				var result = await _barcodeSearchService.SearchByProductSubBarcode(httpClient, firmNumber, periodNumber, barcode);

				if (result.IsSuccess)
				{
					if (result.Data is not null && result.Data.Count != 0)
					{
						string jsonString = result.Data.ToString();
						var productModelList = JsonConvert.DeserializeObject<List<ProductModel>>(jsonString);

						var productModel = productModelList.First();

						//isFind = true;
						//await SendProductBasketPageAsync(productModel);
					}
				}
			}
		}
		catch (System.Exception)
		{

			throw;
		}
	}

	private async Task SearchByVariantSubBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage)
	{
		try
		{
			if(!isFind)
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
						//await SendVariantBasketPageAsync(variantModel);
					}
				}
			}
		}
		catch (System.Exception)
		{

			throw;
		}
	}

	private async Task SearchByProductSeriNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage)
	{
		try
		{
			if(!isFind)
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
						//await SendProductBasketPageAsync(productModel);
					}
				}
			}
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task SearchByVariantSeriNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage)
	{
		try
		{
			if(!isFind)
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
						//await SendVariantBasketPageAsync(variantModel);
					}
				}
			}
		}
		catch (Exception)
		{

			throw;
		}
	}

	private async Task SearchByProductLotNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage)
	{
		try
		{
			if(!isFind)
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
						//await SendProductBasketPageAsync(productModel);
					}
				}
			}
			
		}
		catch (System.Exception)
		{

			throw;
		}
	}

	private async Task SearchByVariantLotNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage)
	{
		try
		{
			if(!isFind)
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
						//await SendVariantBasketPageAsync(variantModel);
					}
				}
			}
			
		}
		catch (System.Exception)
		{

			throw;
		}
	}

	private async Task SearchBySupplierProductCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage)
	{
		try
		{
			if(!isFind)
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
						//await SendProductBasketPageAsync(productModel);
					}
				}
			}
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task SearchBySupplierVariantCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage)
	{
		try
		{
			if(!isFind)
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
						//await SendVariantBasketPageAsync(variantModel);
					}
				}
			}
			

		}
		catch (System.Exception)
		{

			throw;
		}
	}
}
