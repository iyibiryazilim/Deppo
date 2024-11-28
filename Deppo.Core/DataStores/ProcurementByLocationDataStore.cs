using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System.Text;

namespace Deppo.Core.DataStores
{
	public class ProcurementByLocationDataStore : IProcurementByLocationService
	{
		private string postUrl = "/gateway/customQuery/CustomQuery";

		public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, string search = "", int skip = 0, int take = 20)
		{
			var content = new StringContent(JsonConvert.SerializeObject(GetLocationsQuery(firmNumber, periodNumber, warehouseNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

		private string GetLocationsQuery(int firmNumber, int periodNumber, int warehouseNumber, string search = "", int skip = 0, int take = 20)
		{
			var baseQuery = @$"SELECT

        [ReferenceId] = ISNULL(LGMAIN.LOCREF, 0),
        [Code] = ISNULL(INVLOC.CODE, ''),
        [Name] = ISNULL(INVLOC.NAME, ''),
		[InputQuantity] = COUNT(DISTINCT LGMAIN.ITEMREF)
        FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_SLTRANS LGMAIN WITH(NOLOCK)    
		LEFT OUTER JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_LOCATION INVLOC WITH(NOLOCK) ON (LGMAIN.LOCREF  =  INVLOC.LOGICALREF)
        LEFT OUTER JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_SERILOTN SERILOT WITH(NOLOCK) ON (LGMAIN.SLREF = SERILOT.LOGICALREF)
        WHERE (LGMAIN.CANCELLED = 0) AND 
              (LGMAIN.LPRODSTAT = 0) AND 
              (LGMAIN.INVENNO = {warehouseNumber}) AND
			 
		      (LGMAIN.EXIMFCTYPE IN ( 0 , 4 , 5 , 3 , 2 , 7 )) AND 
              (LGMAIN.STATUS = 0) AND 
              (LGMAIN.REMAMOUNT > 0) AND
              (LGMAIN.IOCODE IN (1,2,3,4))";

			if (!string.IsNullOrEmpty(search))
			{
				baseQuery += $" AND (INVLOC.CODE LIKE '%{search}%' OR INVLOC.NAME LIKE '%{search}%')";
			}

			baseQuery += @$"
 GROUP BY LGMAIN.LOCREF,INVLOC.CODE,INVLOC.NAME
			  ORDER BY InputQuantity DESC
OFFSET {skip} ROWS 
FETCH NEXT {take} ROWS ONLY;";

			return baseQuery;
		}
	}
}
