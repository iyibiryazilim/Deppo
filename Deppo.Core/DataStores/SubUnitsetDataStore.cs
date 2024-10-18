using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System.Text;

namespace Deppo.Core.DataStores;

public class SubUnitsetDataStore : ISubUnitsetService
{
	private string postUrl = "/gateway/customQuery/CustomQuery";

	public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId)
	{
		var content = new StringContent(JsonConvert.SerializeObject(SubUnitsetByProductQuery(firmNumber, periodNumber, productReferenceId)), Encoding.UTF8, "application/json");

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

	private string SubUnitsetByProductQuery(int firmNumber, int periodNumber, int productReferenceId)
	{
		string baseQuery = @$"SELECT
			[ReferenceId] = SUBUNITSET.LOGICALREF,
            [UnitsetReferenceId] = SUBUNITSET.UNITSETREF,
            [Code] = SUBUNITSET.CODE,
            [Name] = SUBUNITSET.NAME,
            [ConversionValue] = SUBUNITSET.CONVFACT1,
            [OtherConversionValue] = SUBUNITSET.CONVFACT2
		FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON SUBUNITSET.UNITSETREF = UNITSET.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON ITEMS.UNITSETREF = UNITSET.LOGICALREF
		WHERE ITEMS.LOGICALREF = {productReferenceId}
		";

		return baseQuery;
	}
}
