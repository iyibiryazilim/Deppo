using Deppo.Mobile.Core.Models.ProductModels;
using Deppo.Mobile.Core.Models.VariantModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Helpers.BarcodeHelper;

public interface IBarcodeSearchHelper
{
	Task<dynamic> BarcodeDetectedAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage = "");
	Task<dynamic> SearchByProductCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode);
	Task<dynamic> SearchByVariantCodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode);
	Task<dynamic> SearchByProductMainBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode);
	Task<dynamic> SearchByVariantMainBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode);
	Task<dynamic> SearchByProductSubBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode);
	Task<dynamic> SearchByVariantSubBarcodeAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode);
	Task<dynamic> SearchByProductSeriNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode);
	Task<dynamic> SearchByVariantSeriNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode);
	Task<dynamic> SearchByProductLotNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode);
	Task<dynamic> SearchByVariantLotNumberAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode);
}
