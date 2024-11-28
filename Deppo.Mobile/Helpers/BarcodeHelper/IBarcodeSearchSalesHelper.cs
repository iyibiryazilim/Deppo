using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Helpers.BarcodeHelper;

public interface IBarcodeSearchSalesHelper
{
	Task<dynamic> BarcodeDetectedAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int customerReferenceId, int shipInfoReferenceId, string comingPage = "");
}
