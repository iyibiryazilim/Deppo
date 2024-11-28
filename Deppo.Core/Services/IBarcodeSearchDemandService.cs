using Deppo.Core.DataResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services;

public interface IBarcodeSearchDemandService
{
	Task<DataResult<dynamic>> SearchByProductCode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber);
	Task<DataResult<dynamic>> SearchByVariantCode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber);

	Task<DataResult<dynamic>> SearchByProductMainBarcode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber);

	Task<DataResult<dynamic>> SearchByVariantMainBarcode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber);

	Task<DataResult<dynamic>> SearchByProductSubBarcode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber);

	Task<DataResult<dynamic>> SearchByVariantSubBarcode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber);

	Task<DataResult<dynamic>> SearchByProductSeriNumber(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber);

	Task<DataResult<dynamic>> SearchByVariantSeriNumber(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber);

	Task<DataResult<dynamic>> SearchByProductLotNumber(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber);

	Task<DataResult<dynamic>> SearchByVariantLotNumber(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber);
}
