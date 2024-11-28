using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BarcodeModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Helpers.BarcodeHelper;

public class BarcodeSearchPurchaseHelper : IBarcodeSearchPurchaseHelper
{
	private readonly IBarcodeSearchPurchaseService _barcodeSearchPurchaseService;

	public BarcodeSearchPurchaseHelper(IBarcodeSearchPurchaseService barcodeSearchPurchaseService)
	{
		_barcodeSearchPurchaseService = barcodeSearchPurchaseService;
		isFind = false;
	}
	bool isFind = false;

	public async Task<dynamic> BarcodeDetectedAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId, string comingPage = "")
	{
		isFind = false;
		try
		{
			var cts = new CancellationTokenSource();

			var codeTasks = new List<Task<dynamic>>
			{
				SearchByProductCodeAsync(httpClient, firmNumber, periodNumber, barcode, warehouseNumber, supplierReferenceId, shipInfoReferenceId, comingPage),
				//SearchByVariantCodeAsync(httpClient, firmNumber, periodNumber, barcode, warehouseNumber, supplierReferenceId, shipInfoReferenceId, comingPage),
			 };

			var barcodeTasks = new List<Task<dynamic>>
			{

				SearchByProductMainBarcodeAsync(httpClient, firmNumber, periodNumber, barcode, warehouseNumber, supplierReferenceId, shipInfoReferenceId, comingPage),
				//SearchByVariantMainBarcodeAsync(httpClient, firmNumber, periodNumber, barcode, warehouseNumber, supplierReferenceId, shipInfoReferenceId, comingPage),
				SearchByProductSubBarcodeAsync(httpClient, firmNumber, periodNumber, barcode, warehouseNumber, supplierReferenceId, shipInfoReferenceId, comingPage),
				//SearchByVariantSubBarcodeAsync(httpClient, firmNumber, periodNumber, barcode, warehouseNumber,supplierReferenceId, shipInfoReferenceId, comingPage),

			 };

			var seriTasks = new List<Task<dynamic>>
			{

				SearchByProductSeriNumberAsync(httpClient, firmNumber, periodNumber, barcode, warehouseNumber, supplierReferenceId, shipInfoReferenceId, comingPage),
				//SearchByVariantSeriNumberAsync(httpClient, firmNumber, periodNumber, barcode, warehouseNumber, supplierReferenceId, shipInfoReferenceId, comingPage),


			 };

			var lotTasks = new List<Task<dynamic>>
			{


				SearchByProductLotNumberAsync(httpClient, firmNumber, periodNumber, barcode, warehouseNumber, supplierReferenceId, shipInfoReferenceId, comingPage),
				//SearchByVariantLotNumberAsync(httpClient, firmNumber, periodNumber, barcode, warehouseNumber, supplierReferenceId, shipInfoReferenceId, comingPage),

			 };

			var supplierTasks = new List<Task<dynamic>>
			{
				SearchBySupplierProductCodeAsync(httpClient, firmNumber, periodNumber, barcode, warehouseNumber, supplierReferenceId, shipInfoReferenceId, comingPage),
				//SearchBySupplierVariantCodeAsync(httpClient, firmNumber, periodNumber, barcode, warehouseNumber, supplierReferenceId, shipInfoReferenceId, comingPage),

			 };


			var codeTaskResults = await Task.WhenAll(
				codeTasks
			);

			if (codeTaskResults.Any())
			{
				foreach (var result in codeTaskResults)
				{
					if (result != null) return result;
				}
			}
			else
			{
				var barcodeTaskResults = await Task.WhenAll(barcodeTasks);

				if (barcodeTaskResults.Any())
				{
					foreach (var result in barcodeTaskResults)
					{
						if (result != null) return result;
					}
				}
				else
				{
					var seriTaskResults = await Task.WhenAll(seriTasks);

					if (seriTaskResults.Any())
					{
						foreach (var result in seriTaskResults)
						{
							if (result != null) return result;
						}
					}
					else
					{
						var lotTaskResult = await Task.WhenAll(lotTasks);

						if (lotTaskResult.Any())
						{
							foreach (var result in lotTaskResult)
							{
								if (result != null) return result;
							}
						}
						else
						{
							var supplierTaskResult = await Task.WhenAll(supplierTasks);

							if (supplierTaskResult.Any())
							{
								foreach (var result in supplierTaskResult)
								{
									if (result != null) return result;
								}
							}
						}
					}
				}
			}

			return null;
		}
		catch (Exception ex)
		{
			throw;
		}
		finally
		{

		}
	}

	private async Task<dynamic> SearchByProductCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId, string comingPage)
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
						var productModel = productModelList?.First();
						isFind = true;
						return productModel;
					}
				}
			}
			return null;
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task<dynamic> SearchByProductMainBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId, string comingPage)
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
						return productModel;
					}
				}
			}
			return null;
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task<dynamic> SearchByProductSubBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId, string comingPage)
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
						return productModel;
					}
				}
			}
			return null;
		}
		catch (System.Exception)
		{

			throw;
		}
	}

	private async Task<dynamic> SearchByProductSeriNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId, string comingPage)
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
						return productModel;
					}
				}
			}
			return null;
		}
		catch (Exception ex)
		{

			throw;
		}
	}

	private async Task<dynamic> SearchByProductLotNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId, string comingPage)
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
						return productModel;
					}
				}
			}
			return null;

		}
		catch (System.Exception)
		{

			throw;
		}
	}

	private async Task<dynamic> SearchBySupplierProductCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId, string comingPage)
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
						return productModel;
					}
				}
			}
			return null;

		}
		catch (System.Exception)
		{

			throw;
		}
	}
}
