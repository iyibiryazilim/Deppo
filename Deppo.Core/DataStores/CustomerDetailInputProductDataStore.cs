using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System.Text;

namespace Deppo.Core.DataStores;

public class CustomerDetailInputProductDataStore : ICustomerDetailInputProductService
{
    private string postUrl = "/gateway/customQuery/CustomQuery";

    public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int customerReferenceId, string search = "", int skip = 0, int take = 20)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetInputProductByCustomerQuery(firmNumber, periodNumber, customerReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

    private string GetInputProductByCustomerQuery(int firmNumber, int periodNumber, int customerReferenceId, string search, int skip, int take)
    {
        string baseQuery = $@"
    WITH BaseQuery AS (
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
            [TrackingType] = ITEMS.TRACKTYPE
    FROM
        LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE ON STLINE.STFICHEREF = STFICHE.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON STLINE.STOCKREF = ITEMS.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON STLINE.UOMREF = SUBUNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON STLINE.USREF = UNITSET.LOGICALREF
    WHERE
      STFICHE.TRCODE IN (1, 2, 3)
          AND STLINE.LINETYPE = 0 AND STLINE.CLIENTREF = {customerReferenceId}
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
    HAVING
        ITEMS.CODE <> 'ÿ'
)

SELECT
    BQ.ReferenceId,
    BQ.Code,
    BQ.Name,
    BQ.UnitsetReferenceId,
    BQ.UnitsetCode,
    BQ.UnitsetName,
    BQ.SubUnitsetReferenceId,
    BQ.SubUnitsetCode,
    BQ.SubUnitsetName,
    BQ.StockQuantity,
	BQ.IsVariant,
	BQ.LocTracking,
    BQ.TrackingType,
    FIRMDOC.LDATA AS Image
FROM
    BaseQuery AS BQ
    LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC ON FIRMDOC.INFOREF = BQ.ReferenceId AND FIRMDOC.INFOTYP = 20
ORDER BY BQ.StockQuantity DESC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY;";

        return baseQuery;
    }
}