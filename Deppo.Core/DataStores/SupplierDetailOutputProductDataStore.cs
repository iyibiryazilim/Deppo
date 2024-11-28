using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DataStores;

public class SupplierDetailOutputProductDataStore : ISupplierDetailOutputProductService
{
    private string postUrl = "/gateway/customQuery/CustomQuery";

    public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int supplierReferenceId, string search = "", int skip = 0, int take = 20)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetOutputProductBySupplierQuery(firmNumber, periodNumber, supplierReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

    private string GetOutputProductBySupplierQuery(int firmNumber, int periodNumber, int supplierReferenceId, string search, int skip, int take)
    {
        string baseQuery = $@"
    SELECT * FROM (
        SELECT
            [ReferenceId] = ITEMS.LOGICALREF,
            [Code] = ISNULL(ITEMS.CODE, ''),
            [Name] = ISNULL(ITEMS.NAME, ''),
            [UnitsetReferenceId] = ISNULL(UNITSET.LOGICALREF, 0),
            [UnitsetCode] = ISNULL(UNITSET.CODE, ''),
            [UnitsetName] = ISNULL(UNITSET.NAME, ''),
            [SubUnitsetReferenceId] = ISNULL(SUBUNITSET.LOGICALREF, 0),
            [SubUnitsetCode] = ISNULL(SUBUNITSET.CODE, ''),
            [SubUnitsetName] = ISNULL(SUBUNITSET.NAME, ''),
            [StockQuantity] = ISNULL(SUM(STLINE.AMOUNT), 0),
            [IsVariant] = ITEMS.CANCONFIGURE,
            [LocTracking] = ITEMS.LOCTRACKING,
            [TrackingType] = ITEMS.TRACKTYPE,
            [Image] = ISNULL((SELECT TOP 1 FIRMDOC.LDATA
                 FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC
                 WHERE FIRMDOC.INFOREF = ITEMS.LOGICALREF
                 AND FIRMDOC.INFOTYP = 20
                 AND FIRMDOC.DOCNR = 11), '')
        FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE ON STLINE.STFICHEREF = STFICHE.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON STLINE.STOCKREF = ITEMS.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON STLINE.UOMREF = SUBUNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON STLINE.USREF = UNITSET.LOGICALREF
        WHERE STFICHE.TRCODE IN (6,7,8)
          AND STLINE.LINETYPE = 0 AND STLINE.CLIENTREF = {supplierReferenceId}";

        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";

        baseQuery += $@"
        GROUP BY
            ITEMS.LOGICALREF,
            ITEMS.CODE,
            ITEMS.NAME,
            UNITSET.LOGICALREF,
            UNITSET.CODE,
            UNITSET.NAME,
            SUBUNITSET.LOGICALREF,
            SUBUNITSET.CODE,
            ITEMS.CANCONFIGURE,
            ITEMS.LOCTRACKING,
            ITEMS.TRACKTYPE,
            SUBUNITSET.NAME
        ORDER BY ITEMS.CODE DESC
        OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY
    ) AS subQuery
    ORDER BY subQuery.StockQuantity DESC;";

        return baseQuery;
    }
}