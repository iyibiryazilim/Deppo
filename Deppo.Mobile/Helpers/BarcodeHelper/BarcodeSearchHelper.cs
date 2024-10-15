using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.VariantModels;
using Newtonsoft.Json;

namespace Deppo.Mobile.Helpers.BarcodeHelper;

public class BarcodeSearchHelper
{
	private readonly IBarcodeSearchService _barcodeSearchService;

	public BarcodeSearchHelper(IBarcodeSearchService barcodeSearchService)
	{
		_barcodeSearchService = barcodeSearchService;
	}

	private async Task SearchByProductCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
	{
		try
		{
			var result = await _barcodeSearchService.SearchByProductCode(httpClient, firmNumber, periodNumber, barcode);

			if (result.IsSuccess)
			{
				if (result.Data is not null && result.Data.Count != 0)
				{
					string jsonString = result.Data.ToString();
					var productModelList = JsonConvert.DeserializeObject<List<ProductModel>>(jsonString);
					var productModel = productModelList?.First();
					// isFind = true
					//await SendProductBasketPageAsync(productModel)
				}
			}
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task SearchByVariantCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
	{
		try
		{

			var result = await _barcodeSearchService.SearchByVariantCode(httpClient, firmNumber, periodNumber, barcode);

			if (result.IsSuccess)
			{
				if (result.Data is not null && result.Data.Count != 0)
				{
					string jsonString = result.Data.ToString();
					var variantModelList = JsonConvert.DeserializeObject<List<VariantModel>>(jsonString);

					var variantModel = variantModelList.First();

					//isFind = true;
					//await SendVariantBasketPageAsync(variantModel);
				}
			}

		}
		catch (Exception ex)
		{

			throw;
		}
	}
	private async Task SearchByProductMainBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
	{
		try
		{
			var result = await _barcodeSearchService.SearchByProductMainBarcode(httpClient, firmNumber, periodNumber, barcode);

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
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task SearchByVariantMainBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
	{
		try
		{

			var result = await _barcodeSearchService.SearchByVariantMainBarcode(httpClient, firmNumber, periodNumber, barcode);

			if (result.IsSuccess)
			{
				if (result.Data is not null && result.Data.Count != 0)
				{
					string jsonString = result.Data.ToString();
					var variantModelList = JsonConvert.DeserializeObject<List<VariantModel>>(jsonString);

					var variantModel = variantModelList.First();

					//isFind = true;
					//await SendVariantBasketPageAsync(variantModel);
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
		catch (System.Exception)
		{

			throw;
		}
	}

	private async Task SearchByVariantSubBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
	{
		try
		{

			var result = await _barcodeSearchService.SearchByVariantSubBarcode(httpClient, firmNumber, periodNumber, barcode);

			if (result.IsSuccess)
			{
				if (result.Data is not null && result.Data.Count != 0)
				{
					string jsonString = result.Data.ToString();
					var variantModelList = JsonConvert.DeserializeObject<List<VariantModel>>(jsonString);

					var variantModel = variantModelList.First();

					//isFind = true;
					//await SendVariantBasketPageAsync(variantModel);
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

			var result = await _barcodeSearchService.SearchByProductSeriNumber(httpClient, firmNumber, periodNumber, barcode);

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
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task SearchByVariantSeriNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
	{
		try
		{
			var result = await _barcodeSearchService.SearchByVariantSeriNumber(httpClient, firmNumber, periodNumber, barcode);

			if (result.IsSuccess)
			{
				if (result.Data is not null && result.Data.Count != 0)
				{
					string jsonString = result.Data.ToString();
					var variantModelList = JsonConvert.DeserializeObject<List<VariantModel>>(jsonString);

					var variantModel = variantModelList.First();

					//isFind = true;
					//await SendVariantBasketPageAsync(variantModel);
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
			var result = await _barcodeSearchService.SearchByProductLotNumber(httpClient, firmNumber, periodNumber, barcode);

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
		catch (System.Exception)
		{

			throw;
		}
	}

	private async Task SearchByVariantLotNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
	{
		try
		{

			var result = await _barcodeSearchService.SearchByVariantLotNumber(httpClient, firmNumber, periodNumber, barcode);

			if (result.IsSuccess)
			{
				if (result.Data is not null && result.Data.Count != 0)
				{
					string jsonString = result.Data.ToString();
					var variantModelList = JsonConvert.DeserializeObject<List<VariantModel>>(jsonString);

					var variantModel = variantModelList.First();

					//isFind = true;
					//await SendVariantBasketPageAsync(variantModel);
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

			var result = await _barcodeSearchService.SearchBySupplierProductCode(httpClient, firmNumber, periodNumber, barcode);

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
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task SearchBySupplierVariantCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
	{
		try
		{

			var result = await _barcodeSearchService.SearchBySupplierVariantCode(httpClient, firmNumber, periodNumber, barcode);

			if (result.IsSuccess)
			{
				if (result.Data is not null && result.Data.Count != 0)
				{
					string jsonString = result.Data.ToString();
					var variantModelList = JsonConvert.DeserializeObject<List<VariantModel>>(jsonString);

					var variantModel = variantModelList.First();

					//isFind = true;
					//await SendVariantBasketPageAsync(variantModel);
				}
			}

		}
		catch (System.Exception)
		{

			throw;
		}
	}
}
