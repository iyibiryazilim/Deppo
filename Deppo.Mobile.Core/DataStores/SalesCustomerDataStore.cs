using Deppo.Core.DataResultModel;
using Deppo.Mobile.Core.Services;
using Newtonsoft.Json;
using System.Text;

namespace Deppo.Mobile.Core.DataStores;

public class SalesCustomerDataStore : ISalesCustomerService
{
	private string postUrl = "/gateway/customQuery/CustomQuery";
	public async Task<DataResult<IEnumerable<dynamic>>> GetObjectsAsync(HttpClient httpClient, int firmNumber, int periodNumber, int customerReferenceId, int skip = 0, int take = 20, string search = "")
	{
		var content = new StringContent(JsonConvert.SerializeObject(SalesCustomerQuery(firmNumber, periodNumber, skip, take, search)), Encoding.UTF8, "application/json");

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

	private string SalesCustomerQuery(int firmNumber, int periodNumber, int skip = 0, int take = 20, string search = "")
	{
		string baseQuery = $@"SELECT
            [ReferenceId] = CLCARD.LOGICALREF,
			[Code] = CLCARD.CODE,
			[Name] = CLCARD.NAME,
			[ProductReferenceCount] = COUNT(DISTINCT ORFLINE.STOCKREF)
        FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2,'0')}_ORFLINE AS ORFLINE
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON CLCARD.LOGICALREF = ORFLINE.CLIENTREF
        WHERE ORFLINE.CLOSED = 0 AND (ORFLINE.AMOUN - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 1
		";

		if(!string.IsNullOrEmpty(search))
			baseQuery += $@" AND (CUSTOMER.CODE LIKE '{search}%' OR CUSTOMER.NAME LIKE '%{search}%')";

		baseQuery += $@" GROUP BY CLCARD.LOGICALREF, CLCARD.CODE, CLCARD.NAME ORDER BY CUSTOMER.CODE OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

		return baseQuery;
	}

}
