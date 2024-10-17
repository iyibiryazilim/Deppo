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
	Task BarcodeDetectedAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, string comingPage);
	Task SendProductBasketPageAsync(ProductModel productModel, string comingPage);
	Task SendVariantBasketPageAsync(VariantModel variantModel, string comingPage);
}
