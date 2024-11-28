using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DataStores
{
    public class WarehouseInputTransactionDataStore : IWarehouseInputTransactionService
    {
        private string postUrl = "/gateway/customQuery/CustomQuery";

        public async Task<DataResult<IEnumerable<dynamic>>> GetWarehouseInputTransactions(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetWarehouseInputTransactionsQuery(firmNumber, periodNumber, productReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<IEnumerable<dynamic>>> GetWarehouseInputProducts(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, string search = "", int skip = 0, int take = 20)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetWarehouseInputProductsQuery(firmNumber, periodNumber, warehouseNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

        private string GetWarehouseInputProductsQuery(int firmNumber, int periodNumber, int warehouseNumber, string search, int skip, int take)
        {
            string baseQuery = $@"
    WITH BaseQuery AS (
    SELECT
        [ProductReferenceId] = ITEMS.LOGICALREF,
        [ProductCode] = ISNULL(ITEMS.CODE, ''),
        [ProductName] = ISNULL(ITEMS.NAME, ''),
        [UnitsetReferenceId] = ISNULL(UNITSET.LOGICALREF, 0),
        [UnitsetCode] = ISNULL(UNITSET.CODE, ''),
        [UnitsetName] = ISNULL(UNITSET.NAME, ''),
        [SubUnitsetReferenceId] = ISNULL(SUBUNITSET.LOGICALREF, 0),
        [SubUnitsetCode] = ISNULL(SUBUNITSET.CODE, ''),
        [SubUnitsetName] = ISNULL(SUBUNITSET.NAME, ''),
        [Quantity] = ISNULL(SUM(STLINE.AMOUNT), 0),
        [IsVariant] = ITEMS.CANCONFIGURE,
        [LocTracking] = ITEMS.LOCTRACKING,
        [TrackingType] = ITEMS.TRACKTYPE
    FROM
        LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK)
    LEFT JOIN
        LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON STLINE.STOCKREF = ITEMS.LOGICALREF
    LEFT JOIN
        LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET WITH(NOLOCK) ON STLINE.UOMREF = SUBUNITSET.LOGICALREF
    LEFT JOIN
        LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET WITH(NOLOCK) ON STLINE.USREF = UNITSET.LOGICALREF
    WHERE
        (STLINE.SOURCEINDEX = {warehouseNumber}) AND
        STLINE.IOCODE IN (1, 2) AND
        STLINE.LINETYPE = 0 AND
        STLINE.LPRODSTAT = 0
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
),
OrderedBaseQuery AS (
    SELECT *
    FROM BaseQuery
    ORDER BY ProductCode DESC
    OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY
)
SELECT
    BQ.ProductReferenceId,
    BQ.ProductCode,
    BQ.ProductName,
    BQ.UnitsetReferenceId,
    BQ.UnitsetCode,
    BQ.UnitsetName,
    BQ.SubUnitsetReferenceId,
    BQ.SubUnitsetCode,
    BQ.SubUnitsetName,
    BQ.Quantity,
    BQ.IsVariant,
    BQ.LocTracking,
    BQ.TrackingType,
    FIRMDOC.LDATA AS Image
FROM
    OrderedBaseQuery AS BQ
LEFT JOIN
    LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC ON FIRMDOC.INFOREF = BQ.ProductReferenceId AND FIRMDOC.INFOTYP = 20
ORDER BY
    BQ.Quantity DESC;";

            return baseQuery;
        }

        private string GetWarehouseInputTransactionsQuery(int firmNumber, int periodNumber, int productReferenceId, string search, int skip, int take)
        {
            string baseQuery = $@"Select
            [ReferenceId] = STLINE.LOGICALREF,
            [TransactionDate] = STLINE.DATE_,
            [TransactionTime] = dbo.LG_INTTOTIME(STLINE.FTIME),
     [CurrentReferenceId] = ISNULL(CLCARD.LOGICALREF,0),
	 [CurrentCode] = ISNULL(CLCARD.CODE,''),
     [CurrentName] = ISNULL(CLCARD.DEFINITION_,''),
	 [BaseTransactionReferenceId] = STFICHE.LOGICALREF,
	 [BaseTransactionCode] = STFICHE.FICHENO,
     [TransactionType] = STLINE.TRCODE,
	 [GroupCode] = STFICHE.GRPCODE,
	 [IOType] = STLINE.IOCODE,
	 [ProductReferenceId] =  ITEMS.LOGICALREF,
	 [ProductCode] = ISNULL(ITEMS.CODE,''),
	 [ProductName] = ISNULL(ITEMS.NAME,''),
	 [UnitsetReferenceId] = ISNULL( unitset.LOGICALREF,0),
	 [UnitsetCode] = ISNULL( unitset.CODE , ''),
	 [UnitsetName] =  ISNULL (unitset.NAME , ''),
	 [SubUnitsetReferenceId] = ISNULL ( subunitset.LOGICALREF, 0),
	 [SubUnitsetCode] = ISNULL (subunitset.CODE,''),
	 [SubUnitsetName] = ISNULL (subunitset.NAME , ''),
	 [WarehouseName] = capiwhouse.NAME,
     [WarehouseNumber] = capiwhouse.NR,
	 [Quantity] = ISNULL ( STLINE.AMOUNT,0),
	 [Length] =  ISNULL ( STLINE.UINFO4 , 0),
	 [Width] = ISNULL  (STLINE.UINFO5,0),
	 [Height] = ISNULL ( STLINE.UINFO6,0),
	 [Weight] = ISNULL  ( STLINE.UINFO7 , 0),
	 [Volume] = ISNULL  (STLINE.UINFO8 , 0),
	 [Barcode] = ''
from  LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE as STLINE
Left Join LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE on STLINE.STFICHEREF = STFICHE.LOGICALREF
left join LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS on STLINE.STOCKREF = ITEMS.LOGICALREF
Left Join LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD as CLCARD ON STLINE.CLIENTREF = CLCARD.LOGICALREF
left join LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS subunitset ON STLINE.UOMREF = subunitset.LOGICALREF
left join LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS unitset ON STLINE.USREF = unitset.LOGICALREF
LEFT JOIN L_CAPIWHOUSE AS capiwhouse ON STLINE.SOURCEINDEX = capiwhouse.NR AND capiwhouse.FIRMNR = {firmNumber}
WHERE STLINE.IOCODE IN (1,2) AND STLINE.LINETYPE = 0 AND ITEMS.LOGICALREF = {productReferenceId}
";
            if (!string.IsNullOrEmpty(search))
                baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";

            baseQuery += $@" ORDER BY ITEMS.CODE DESC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

            return baseQuery;
        }
    }
}