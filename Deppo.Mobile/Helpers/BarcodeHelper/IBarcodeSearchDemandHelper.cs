using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Helpers.BarcodeHelper;

public interface IBarcodeSearchDemandHelper
{
	Task<dynamic> BarcodeDetectedAsync(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, string comingPage = "");
}
