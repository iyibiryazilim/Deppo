using Deppo.Core.DataResultModel;
using Deppo.Mobile.Core.Services;
using Newtonsoft.Json;
using System.Text;

namespace Deppo.Mobile.Core.DataStores;

public class SalesCustomerDataStore : ISalesCustomerService
{
    private string postUrl = "/gateway/customQuery/CustomQuery";

    public async Task<DataResult<IEnumerable<dynamic>>> GetObjectsAsync(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int skip = 0, int take = 20, string search = "")
    {
        var content = new StringContent(JsonConvert.SerializeObject(SalesCustomerQuery(firmNumber, periodNumber, warehouseNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<IEnumerable<dynamic>>> GetObjectsReturnAsync(HttpClient httpClient, int firmNumber, int periodNumber, int skip = 0, int take = 20, string search = "")
    {
        var content = new StringContent(JsonConvert.SerializeObject(SalesCustomerQueryReturn(firmNumber, periodNumber,/* warehouseNumber,*/ search, skip, take)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<IEnumerable<dynamic>>> SalesCustomerQueryFiche(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int skip = 0, int take = 20, string search = "")
    {
        var content = new StringContent(JsonConvert.SerializeObject(SalesCustomerQueryFiche(firmNumber, periodNumber, warehouseNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

    private string SalesCustomerQuery(int firmNumber, int periodNumber, int warehouseNumber, string search = "", int skip = 0, int take = 20)
    {
        string baseQuery = $@"SELECT
            [ReferenceId] = CLCARD.LOGICALREF,
			[Code] = CLCARD.CODE,
			[Name] = CLCARD.DEFINITION_,
			[ProductReferenceCount] = COUNT(DISTINCT ORFLINE.STOCKREF),
            [Country] = CLCARD.COUNTRY,
            [City] = CLCARD.CITY,
           CASE
        WHEN CLCARD.ACCEPTEDESP = 0 THEN 0
        ELSE 1
    END AS [IsEDispatch],
             [ShipAddressCount]=ISNULL((SELECT COUNT(SHIP.LOGICALREF) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_SHIPINFO AS SHIP WHERE CLIENTREF = CLCARD.LOGICALREF),0),
            [FicheType] =  CLCARD.ACCEPTEINV

        FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON CLCARD.LOGICALREF = ORFLINE.CLIENTREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_SHIPINFO AS SHIPADRESS ON CLCARD.LOGICALREF = SHIPADRESS.CLIENTREF
        WHERE ORFLINE.CLOSED = 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 1 AND ORFLINE.SOURCEINDEX = {warehouseNumber}
		";

        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" AND (CLCARD.CODE LIKE '{search}%' OR CLCARD.DEFINITION_ LIKE '%{search}%')";

        baseQuery += $@" GROUP BY CLCARD.LOGICALREF,CLCARD.ACCEPTEINV, CLCARD.CODE, CLCARD.DEFINITION_, CLCARD.COUNTRY, CLCARD.ACCEPTEDESP, CLCARD.CITY ORDER BY CLCARD.DEFINITION_ ASC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }

    private string SalesCustomerQueryReturn(int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
    {
        string baseQuery = $@"SELECT
            [ReferenceId] = CLCARD.LOGICALREF,
			[Code] = CLCARD.CODE,
			[Name] = CLCARD.DEFINITION_,
			[ProductReferenceCount] = COUNT(DISTINCT ORFLINE.STOCKREF),
            [Country] = CLCARD.COUNTRY,
            [City] = CLCARD.CITY,
            [ShipAddressCount] = COUNT(SHIPADRESS.CLIENTREF),
            [FicheType] =  CLCARD.ACCEPTEINV

        FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON CLCARD.LOGICALREF = ORFLINE.CLIENTREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_SHIPINFO AS SHIPADRESS ON CLCARD.LOGICALREF = SHIPADRESS.CLIENTREF
        WHERE ORFLINE.CLOSED = 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 1  AND CLCARD.CODE like '12%'
		";

        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" AND (CLCARD.CODE LIKE '{search}%' OR CLCARD.DEFINITION_ LIKE '%{search}%')";

        baseQuery += $@" GROUP BY CLCARD.LOGICALREF,CLCARD.ACCEPTEINV, CLCARD.CODE, CLCARD.DEFINITION_, CLCARD.COUNTRY, CLCARD.CITY ORDER BY CLCARD.DEFINITION_ desc OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }

    private string SalesCustomerQueryFiche(int firmNumber, int periodNumber, int warehouseNumber, string search = "", int skip = 0, int take = 20)
    {
        string baseQuery = $@"SELECT
    [ReferenceId] = CLCARD.LOGICALREF,
    [Code] = CLCARD.CODE,
    [Name] = CLCARD.DEFINITION_,
    [ProductReferenceCount] = COUNT(DISTINCT STLINE.STOCKREF),
    [Country] = CLCARD.COUNTRY,
CASE
       WHEN CLCARD.ACCEPTEDESP = 0 THEN 0
       ELSE 1
   END AS [IsEDispatch],
    [City] = CLCARD.CITY
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD
    ON STFICHE.CLIENTREF = CLCARD.LOGICALREF
left join LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE ON STLINE.STFICHEREF = STFICHE.LOGICALREF
WHERE
   STFICHE.TRCODE in  (7,8)
    AND  STLINE.SOURCEINDEX = {warehouseNumber}";

        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" AND (CLCARD.CODE LIKE '{search}%' OR CLCARD.DEFINITION_ LIKE '%{search}%')";

        baseQuery += $@" GROUP BY CLCARD.LOGICALREF, CLCARD.CODE, CLCARD.DEFINITION_, CLCARD.COUNTRY, CLCARD.CITY,CLCARD.ACCEPTEDESP
ORDER BY CLCARD.DEFINITION_ ASC
OFFSET {skip} ROWS
FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }
}