using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Deppo.Mobile.Core.Models.BarcodeModels;
using Deppo.Mobile.Core.Models.ProductModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Helpers.BarcodeHelper;

public class BarcodeSearchDemandHelper : IBarcodeSearchDemandHelper
{
	IBarcodeSearchDemandService _barcodeSearchDemandService;

    public BarcodeSearchDemandHelper(IBarcodeSearchDemandService barcodeSearchDemandService)
    {
		_barcodeSearchDemandService = barcodeSearchDemandService;

		isFind = false;
	}

	bool isFind = false;

    public async Task<dynamic> BarcodeDetectedAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, string comingPage)
	{
		isFind = false;

		try
		{
			//var cts = new CancellationTokenSource();

			var codeTasks = new List<Task<dynamic>>
			{
				SearchByProductCodeAsync(httpClient, firmNumber, periodNumber, barcode, warehouseNumber),
				//SearchByVariantCodeAsync(httpClient, firmNumber, periodNumber, barcode, warehouseNumber),
			 };

			var barcodeTasks = new List<Task<dynamic>>
			{

				SearchByProductMainBarcodeAsync(httpClient, firmNumber, periodNumber, barcode, warehouseNumber),
				//SearchByVariantMainBarcodeAsync(httpClient, firmNumber, periodNumber, barcode, warehouseNumber),
				SearchByProductSubBarcodeAsync(httpClient, firmNumber, periodNumber, barcode, warehouseNumber),
				//SearchByVariantSubBarcodeAsync(httpClient, firmNumber, periodNumber, barcode, warehouseNumber),
			 };

			var seriTasks = new List<Task<dynamic>>
			{

				SearchByProductSeriNumberAsync(httpClient, firmNumber, periodNumber, barcode, warehouseNumber),
				//SearchByVariantSeriNumberAsync(httpClient, firmNumber, periodNumber, barcode, warehouseNumber),
			 };

			var lotTasks = new List<Task<dynamic>>
			{


				SearchByProductLotNumberAsync(httpClient, firmNumber, periodNumber, barcode, warehouseNumber),
				//SearchByVariantLotNumberAsync(httpClient, firmNumber, periodNumber, barcode, warehouseNumber),
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
					}
				}
			}

			return null;
		}
		catch (Exception)
		{

			throw;
		}
	}


	public async Task<dynamic> SearchByProductCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber)
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
						var barcodeDemandProductModelList = JsonConvert.DeserializeObject<List<BarcodeDemandProductModel>>(jsonString);
						var productModel = barcodeDemandProductModelList?.First();
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
	public async Task<dynamic> SearchByProductMainBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber)
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
						var barcodeDemandProductModelList = JsonConvert.DeserializeObject<List<BarcodeDemandProductModel>>(jsonString);
						var productModel = barcodeDemandProductModelList?.First();
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
	public async Task<dynamic> SearchByProductSubBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber)
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
						var barcodeDemandProductModelList = JsonConvert.DeserializeObject<List<BarcodeDemandProductModel>>(jsonString);
						var productModel = barcodeDemandProductModelList?.First();
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
	public async Task<dynamic> SearchByProductSeriNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber)
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
						var barcodeDemandProductModelList = JsonConvert.DeserializeObject<List<BarcodeDemandProductModel>>(jsonString);
						var productModel = barcodeDemandProductModelList?.First();
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
	public async Task<dynamic> SearchByProductLotNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber)
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
						var barcodeDemandProductModelList = JsonConvert.DeserializeObject<List<BarcodeDemandProductModel>>(jsonString);
						var productModel = barcodeDemandProductModelList?.First();
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

}
