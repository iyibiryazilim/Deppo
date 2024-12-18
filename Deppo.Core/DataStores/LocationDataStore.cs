using System;
using System.Text;
using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;

namespace Deppo.Core.DataStores;

public class LocationDataStore : ILocationService
{
    private string postUrl = "/gateway/customQuery/CustomQuery";

    public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int productReferenceId,int variantReferenceId = 0, string search = "", int skip = 0, int take = 20, string externalDb = "")
    {
        var content = new StringContent(JsonConvert.SerializeObject(LocationQuery(firmNumber, periodNumber, warehouseNumber, productReferenceId,variantReferenceId,
            search, skip, take, externalDb)), Encoding.UTF8, "application/json");

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

	public async Task<DataResult<IEnumerable<dynamic>>> GetLocationsWithStock(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int productReferenceId, int variantReferenceId = 0, string search = "", int skip = 0, int take = 20, string externalDb = "")
	{
		var content = new StringContent(JsonConvert.SerializeObject(LocationWithStockQuery(firmNumber, periodNumber, warehouseNumber, productReferenceId, variantReferenceId,
			search, skip, take, externalDb)), Encoding.UTF8, "application/json");

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

	public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, string search = "", int skip = 0, int take = 20, string externalDb = "")
    {
        var content = new StringContent(JsonConvert.SerializeObject(LocationQuery(firmNumber, periodNumber, warehouseNumber, search, skip, take, externalDb)), Encoding.UTF8, "application/json");

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

    private string LocationQuery(int firmNumber, int periodNumber, int warehouseNumber, int productReferenceId, int variantReferenceId = 0, string search = "", int skip = 0, int take = 20, string externalDb = "")
    {
        string baseQuery = $@"SELECT 
[ReferenceId] = LOC.LOGICALREF,
[Code] = LOC.CODE,
[Name] = LOC.NAME,
[WarehouseReferenceId] = WHOUSE.LOGICALREF,
[WarehouseNumber] = WHOUSE.NR,
[WarehouseName] = WHOUSE.NAME,
[StockQuantity] = ISNULL((SELECT SUM(REMAMOUNT) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_SLTRANS AS SLTRANS WITH(NOLOCK) WHERE SLTRANS.INVENNO = WHOUSE.NR AND SLTRANS.LOCREF = LOC.LOGICALREF AND SLTRANS.ITEMREF = {productReferenceId} AND SLTRANS.VARIANTREF = {variantReferenceId}),0)

FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_LOCATION AS LOC WITH(NOLOCK)
LEFT JOIN {externalDb}L_CAPIWHOUSE AS WHOUSE WITH(NOLOCK) ON LOC.INVENNR = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
WHERE WHOUSE.NR = {warehouseNumber}";

        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" AND (LOC.CODE LIKE '{search}%' OR LOC.NAME LIKE '%{search}%')";

        baseQuery += $@" ORDER BY LOC.CODE ASC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }

	/// <summary>
	/// Stoðu 0'dan büyük olan raflarý getirir
	/// </summary>

	private string LocationWithStockQuery(int firmNumber, int periodNumber, int warehouseNumber, int productReferenceId, int variantReferenceId = 0, string search = "", int skip = 0, int take = 20, string externalDb = "")
	{
		string baseQuery = $@"SELECT 
[ReferenceId] = LOC.LOGICALREF,
[Code] = LOC.CODE,
[Name] = LOC.NAME,
[WarehouseReferenceId] = WHOUSE.LOGICALREF,
[WarehouseNumber] = WHOUSE.NR,
[WarehouseName] = WHOUSE.NAME,
[StockQuantity] = ISNULL((SELECT SUM(REMAMOUNT) 
                          FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_SLTRANS AS SLTRANS WITH(NOLOCK) 
                          WHERE SLTRANS.INVENNO = WHOUSE.NR AND SLTRANS.LOCREF = LOC.LOGICALREF AND SLTRANS.ITEMREF = {productReferenceId} AND SLTRANS.VARIANTREF = {variantReferenceId}),0)

FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_LOCATION AS LOC WITH(NOLOCK)
LEFT JOIN {externalDb}L_CAPIWHOUSE AS WHOUSE WITH(NOLOCK) ON LOC.INVENNR = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
WHERE WHOUSE.NR = {warehouseNumber}
AND ISNULL((SELECT SUM(REMAMOUNT) 
            FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_SLTRANS AS SLTRANS WITH(NOLOCK) 
            WHERE SLTRANS.INVENNO = WHOUSE.NR AND SLTRANS.LOCREF = LOC.LOGICALREF AND SLTRANS.ITEMREF = {productReferenceId} AND SLTRANS.VARIANTREF = {variantReferenceId}),0) > 0";

		if (!string.IsNullOrEmpty(search))
			baseQuery += $@" AND (LOC.CODE LIKE '{search}%' OR LOC.NAME LIKE '%{search}%')";

		baseQuery += $@" ORDER BY LOC.CODE ASC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

		return baseQuery;
	}



	private string LocationQuery(int firmNumber, int periodNumber, int warehouseNumber, string search = "", int skip = 0, int take = 20, string externalDb = "")
    {
        string baseQuery = $@"SELECT 
[ReferenceId] = LOC.LOGICALREF,
[Code] = LOC.CODE,
[Name] = LOC.NAME,
[WarehouseReferenceId] = WHOUSE.LOGICALREF,
[WarehouseNumber] = WHOUSE.NR,
[WarehouseName] = WHOUSE.NAME,
[StockQuantity] = ISNULL((SELECT COUNT(DISTINCT ITEMREF) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_SLTRANS AS SLTRANS WITH(NOLOCK) WHERE SLTRANS.INVENNO = WHOUSE.NR AND SLTRANS.LOCREF = LOC.LOGICALREF ),0)

FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_LOCATION AS LOC WITH(NOLOCK)
LEFT JOIN {externalDb}L_CAPIWHOUSE AS WHOUSE WITH(NOLOCK) ON LOC.INVENNR = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
WHERE WHOUSE.NR = {warehouseNumber}";

        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" AND (LOC.CODE LIKE '{search}%' OR LOC.NAME LIKE '%{search}%')";

        baseQuery += $@" ORDER BY LOC.CODE ASC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }
}
