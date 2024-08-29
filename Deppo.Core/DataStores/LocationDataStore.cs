using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System.Text;

namespace Deppo.Core.DataStores;

public class LocationDataStore : ILocationService
{
	string postUrl = "/gateway/customQuery/CustomQuery";
	public async Task<DataResult<IEnumerable<dynamic>>> GetObjectsAsync(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int skip = 0, int take = 20, string search = "")
	{
		var content = new StringContent(JsonConvert.SerializeObject(LocationQuery(firmNumber, periodNumber, warehouseNumber, skip, take, search)), Encoding.UTF8, "application/json");

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

	private string LocationQuery(int firmNumber, int periodNumber, int warehouseNumber, int skip = 0, int take = 20, string search = "")
	{
		var baseQuery = @$"SELECT 
			  [ReferenceId] = LOCATION.LOGICALREF,
			  [WarehouseNumber] =  LOCATION.INVENNR,
			  [LocationCode] = LOCATION.CODE,
			  [LocationName] = LOCATION.NAME
			FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_LOCATION AS LOCATION
            WHERE LOCATION.INVENNR = {warehouseNumber}";

		if (!string.IsNullOrEmpty(search))
		{
			baseQuery += $@" AND (LOCATION.CODE LIKE '{search}%' OR LOCATION.NAME LIKE '%{search}%')";
		}

		baseQuery += $@" ORDER BY LOCATION.CODE OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

		return baseQuery;
	}
}
