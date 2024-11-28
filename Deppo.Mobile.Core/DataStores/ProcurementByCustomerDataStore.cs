using System;
using System.Text;
using Deppo.Core.DataResultModel;
using Deppo.Mobile.Core.Services;
using Newtonsoft.Json;

namespace Deppo.Mobile.Core.DataStores;

public class ProcurementByCustomerDataStore : IProcurementByCustomerService
{
    private string postUrl = "/gateway/customQuery/CustomQuery";

    public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, string search = "", int skip = 0, int take = 20)
    {
        var content = new StringContent(JsonConvert.SerializeObject(ProcurementByCustomerListQuery(firmNumber, periodNumber, warehouseNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

    private string ProcurementByCustomerListQuery(int firmNumber, int periodNumber, int warehouseNumber, string search = "", int skip = 0, int take = 20)
    {
        string baseQuery = $@"SELECT
    [ReferenceId] = ISNULL(CLCARD.LOGICALREF, 0),
    [Code] = ISNULL(CLCARD.CODE, ''),
    [Name] = ISNULL(CLCARD.DEFINITION_, ''),
    [ProductReferenceCount] = COUNT(DISTINCT ORFLINE.STOCKREF),
    [ShipAddressCount]=ISNULL((SELECT COUNT(SHIP.LOGICALREF) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_SHIPINFO AS SHIP WHERE CLIENTREF = CLCARD.LOGICALREF),0),
    [Country] = ISNULL(CLCARD.COUNTRY, ''),
    [City] = ISNULL(CLCARD.CITY, '')
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE 
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3,'0')}_CLCARD AS CLCARD WITH(NOLOCK) ON ORFLINE.CLIENTREF = CLCARD.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_SHIPINFO AS SHIPADRESS ON CLCARD.LOGICALREF = SHIPADRESS.CLIENTREF
WHERE ORFLINE.CLOSED = 0
    AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0
    AND ORFLINE.TRCODE = 1 AND ORFLINE.SOURCEINDEX = {warehouseNumber}";

        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" AND (CLCARD.CODE LIKE '{search}%' OR CLCARD.DEFINITION_ LIKE '%{search}%')";

        baseQuery += $@" GROUP BY CLCARD.LOGICALREF, CLCARD.CODE, CLCARD.DEFINITION_, CLCARD.COUNTRY, CLCARD.CITY
ORDER BY CLCARD.DEFINITION_ ASC
OFFSET {skip} ROWS
FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }
}
