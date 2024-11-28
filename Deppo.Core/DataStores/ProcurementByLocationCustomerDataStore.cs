using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DataStores
{
	public class ProcurementByLocationCustomerDataStore : IProcurementByLocationCustomerService
	{
		private string postUrl = "/gateway/customQuery/CustomQuery";

		public async Task<DataResult<IEnumerable<dynamic>>> GetCustomers(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, string productReferenceIds, string search = "", int skip = 0, int take = 20)
		{
			var content = new StringContent(JsonConvert.SerializeObject(GetCustomersQuery(firmNumber, periodNumber, warehouseNumber, productReferenceIds, search, skip, take)), Encoding.UTF8, "application/json");

			HttpResponseMessage responseMessage = await httpClient.PostAsync(postUrl, content);
			DataResult<IEnumerable<dynamic>> dataResult = new DataResult<IEnumerable<dynamic>>();
			if (responseMessage.IsSuccessStatusCode)
			{
				var data = await responseMessage.Content.ReadAsStringAsync();
				if (data != null)
				{
					if (!string.IsNullOrEmpty(data))
					{
						var result = JsonConvert.DeserializeObject<DataResult<IEnumerable<dynamic>>>(data);

						dataResult.Data = result?.Data;
						dataResult.IsSuccess = true;
						dataResult.Message = "success";
						return dataResult;
					}
					else
					{
						var result = JsonConvert.DeserializeObject<DataResult<IEnumerable<Dictionary<string, object>>>>(data);

						dataResult.Data = result?.Data;
						dataResult.IsSuccess = true;
						dataResult.Message = "empty";
						return dataResult;
					}
				}
				else
				{
					var result = JsonConvert.DeserializeObject<DataResult<IEnumerable<Dictionary<string, object>>>>(data);

					dataResult.Data = Enumerable.Empty<dynamic>();
					dataResult.IsSuccess = false;
					dataResult.Message = await responseMessage.Content.ReadAsStringAsync();

					return dataResult;
				}
			}
			else
			{
				dataResult.Data = Enumerable.Empty<dynamic>();
				dataResult.IsSuccess = false;
				dataResult.Message = await responseMessage.Content.ReadAsStringAsync();
				return dataResult;
			}
		}

		private string GetCustomersQuery(int firmNumber, int periodNumber, int warehouseNumber, string productReferenceIds, string search = "", int skip = 0, int take = 20)
		{
			var baseQuery = @$"SELECT
            [CustomerReferenceId] = CLCARD.LOGICALREF,
            [CustomerCode] = CLCARD.CODE,
            [CustomerName] = CLCARD.DEFINITION_,
			[City]=CLCARD.CITY,
			[Country]=CLCARD.COUNTRY,
            [Quantity] = SUM(ORFLINE.AMOUNT),
            [ShippedQuantity] = SUM(ORFLINE.SHIPPEDAMOUNT),
            [WaitingQuantity] = SUM(ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT),
			[ProductReferenceId] = ORFLINE.STOCKREF,
			[ShipAddressCount]=ISNULL((SELECT COUNT(SHIP.LOGICALREF) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_SHIPINFO AS SHIP WHERE CLIENTREF = CLCARD.LOGICALREF),0)
        FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFICHE AS ORFICHE ON ORFLINE.ORDFICHEREF = ORFICHE.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON ORFLINE.STOCKREF = ITEMS.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON ORFICHE.CLIENTREF = CLCARD.LOGICALREF
        LEFT JOIN L_CAPIWHOUSE AS WHOUSE ON ORFLINE.SOURCEINDEX = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
		WHERE ORFLINE.CLOSED = 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 1
		AND ITEMS.UNITSETREF <> 0 AND WHOUSE.NR = {warehouseNumber} AND ITEMS.LOGICALREF IN({productReferenceIds})";

			if (!string.IsNullOrEmpty(search))
			{
				baseQuery += $" AND (CLCARD.DEFINITION_ LIKE '%{search}%' OR CLCARD.CODE LIKE '{search}%')";
			}

			baseQuery += @$"
 GROUP BY CLCARD.LOGICALREF,CLCARD.CODE,CLCARD.DEFINITION_,CLCARD.CITY,CLCARD.COUNTRY,ORFLINE.STOCKREF
ORDER BY SUM(ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT)
OFFSET {skip} ROWS 
FETCH NEXT {take} ROWS ONLY;";

			return baseQuery;
		}
	}
}
