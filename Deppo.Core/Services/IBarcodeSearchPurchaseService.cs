using Deppo.Core.DataResultModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Services;

public interface IBarcodeSearchPurchaseService
{
	Task<DataResult<dynamic>> SearchByProductCode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId);
	Task<DataResult<dynamic>> SearchByVariantCode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId);

	Task<DataResult<dynamic>> SearchByProductMainBarcode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId);

	Task<DataResult<dynamic>> SearchByVariantMainBarcode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId);

	Task<DataResult<dynamic>> SearchByProductSubBarcode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId);

	Task<DataResult<dynamic>> SearchByVariantSubBarcode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId);

	Task<DataResult<dynamic>> SearchByProductSeriNumber(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId);

	Task<DataResult<dynamic>> SearchByVariantSeriNumber(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId);

	Task<DataResult<dynamic>> SearchByProductLotNumber(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId);

	Task<DataResult<dynamic>> SearchByVariantLotNumber(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId);

	Task<DataResult<dynamic>> SearchBySupplierProductCode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId);

	Task<DataResult<dynamic>> SearchBySupplierVariantCode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId);
}
