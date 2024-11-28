using System;
using System.Text;
using System.Text.Json;
using Deppo.Core.DataResultModel;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Core.SortModels;
using Newtonsoft.Json;

namespace Deppo.Core.DataStores;

public class WarehouseDataStore : IWarehouseService
{
    public string postUrl = $"/gateway/product/" + typeof(Warehouse).Name;
    public string customPostUrl = "/gateway/customQuery/CustomQuery";

    public async Task<DataResult<IEnumerable<Warehouse>>> GetObjects(HttpClient httpClient, string search, SortModel? orderBy, int page, int pageSize, int firmNumber)
    {
        HttpResponseMessage responseMessage = await httpClient.GetAsync(postUrl + $"?search={search}&orderBy={orderBy}&page={page}&pageSize={pageSize}&firmNumber={firmNumber}");
        DataResult<IEnumerable<Warehouse>> dataResult = new DataResult<IEnumerable<Warehouse>>();
        if (responseMessage.IsSuccessStatusCode)
        {
            var data = await responseMessage.Content.ReadAsStringAsync();
            if (data != null)
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var result = JsonConvert.DeserializeObject<DataResult<IEnumerable<Warehouse>>>(data);

                    dataResult.Data = result?.Data;
                    dataResult.IsSuccess = true;
                    dataResult.Message = "success";
                    return dataResult;

                }
                else
                {
                    var result = JsonConvert.DeserializeObject<DataResult<IEnumerable<Warehouse>>>(data);

                    dataResult.Data = result?.Data;
                    dataResult.IsSuccess = true;
                    dataResult.Message = "empty";
                    return dataResult;
                }

            }
            else
            {
                var result = JsonConvert.DeserializeObject<DataResult<IEnumerable<Warehouse>>>(data);

                dataResult.Data = Enumerable.Empty<Warehouse>();
                dataResult.IsSuccess = false;
                dataResult.Message = await responseMessage.Content.ReadAsStringAsync();

                return dataResult;
            }


        }
        else
        {
            dataResult.Data = Enumerable.Empty<Warehouse>();
            dataResult.IsSuccess = false;
            dataResult.Message = await responseMessage.Content.ReadAsStringAsync();
            return dataResult;
        }
    }

    public async Task<DataResult<IEnumerable<dynamic>>> GetObjectsAsync(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
    {

        var content = new StringContent(JsonConvert.SerializeObject(WarehouseListQuery(firmNumber, periodNumber, search, skip, take)), Encoding.UTF8, "application/json");

        HttpResponseMessage responseMessage = await httpClient.PostAsync(customPostUrl, content);
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

	public async Task<DataResult<dynamic>> GetObjectById(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber)
	{
		var content = new StringContent(JsonConvert.SerializeObject(WarehouseQueryByNumber(firmNumber, periodNumber, warehouseNumber)), Encoding.UTF8, "application/json");

		HttpResponseMessage responseMessage = await httpClient.PostAsync(postUrl, content);
		DataResult<dynamic> dataResult = new DataResult<dynamic>();
		if (responseMessage.IsSuccessStatusCode)
		{
			var data = await responseMessage.Content.ReadAsStringAsync();
			if (data != null)
			{
				if (!string.IsNullOrEmpty(data))
				{
					var result = JsonConvert.DeserializeObject<DataResult<dynamic>>(data);

					dataResult.Data = result?.Data;
					dataResult.IsSuccess = true;
					dataResult.Message = "success";
					return dataResult;
				}
				else
				{
					var result = JsonConvert.DeserializeObject<DataResult<Dictionary<string, object>>>(data);

					dataResult.Data = result?.Data;
					dataResult.IsSuccess = true;
					dataResult.Message = "empty";
					return dataResult;
				}
			}
			else
			{
				var result = JsonConvert.DeserializeObject<DataResult<Dictionary<string, object>>>(data);

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


	private string WarehouseListQuery(int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
    {
         string baseQuery = $@"SELECT 
[ReferenceId] = WHOUSE.LOGICALREF,
[Number] = WHOUSE.NR,
[Name] = WHOUSE.NAME,
[City] = WHOUSE.CITY,
[County] = WHOUSE.TOWN,
[Quantity] = 0,
[LocationCount] = (SELECT ISNULL(COUNT(*),0) FROM LG_{firmNumber.ToString().PadLeft(3,'0')}_LOCATION AS LOC WITH(NOLOCK) WHERE LOC.INVENNR = WHOUSE.NR)

FROM L_CAPIWHOUSE AS WHOUSE WITH(NOLOCK) 
WHERE WHOUSE.FIRMNR = {firmNumber}";

        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" AND (WHOUSE.NR LIKE '{search}%' OR WHOUSE.NAME LIKE '%{search}%')";

        baseQuery += $@" ORDER BY WHOUSE.NR ASC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }

	private string WarehouseQueryByNumber(int firmNumber, int periodNumber, int warehouseNumber)
	{
		string baseQuery = $@"SELECT 
[ReferenceId] = WHOUSE.LOGICALREF,
[Number] = WHOUSE.NR,
[Name] = WHOUSE.NAME,
[City] = WHOUSE.CITY,
[County] = WHOUSE.TOWN,
[Quantity] = 0,
[LocationCount] = (SELECT ISNULL(COUNT(*),0) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_LOCATION AS LOC WITH(NOLOCK) WHERE LOC.INVENNR = WHOUSE.NR)

FROM L_CAPIWHOUSE AS WHOUSE WITH(NOLOCK) 
WHERE WHOUSE.FIRMNR = {firmNumber} AND WHOUSE.NR = {warehouseNumber}";


		return baseQuery;
	}

}
