using Deppo.Core.DataResultModel;
using Deppo.Mobile.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.DataStores;

public class PurchaseSupplierDataStore : IPurchaseSupplierService
{
    private string postUrl = "/gateway/customQuery/CustomQuery";

    public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
    {
        var content = new StringContent(JsonConvert.SerializeObject(PurchaseSupplierQuery(firmNumber, periodNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

    private string PurchaseSupplierQuery(int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
    {
        string baseQuery = $@"SELECT
    [ReferenceId] = CLCARD.LOGICALREF,
    [Code] = CLCARD.CODE,
    [Name] = CLCARD.DEFINITION_,
    [ProductReferenceCount] = COUNT(DISTINCT ORFLINE.STOCKREF)
FROM LG_001_02_ORFLINE AS ORFLINE
LEFT JOIN LG_001_CLCARD AS CLCARD
    ON ORFLINE.CLIENTREF = CLCARD.LOGICALREF
WHERE ORFLINE.CLOSED = 0
    AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0
    AND ORFLINE.TRCODE = 1";

        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" AND (CLCARD.CODE LIKE '{search}%' OR CLCARD.DEFINITION_ LIKE '%{search}%')";

        baseQuery += $@" GROUP BY CLCARD.LOGICALREF, CLCARD.CODE, CLCARD.DEFINITION_
ORDER BY CLCARD.DEFINITION_ ASC
OFFSET {skip} ROWS
FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }
}