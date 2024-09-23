using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System.Text;

namespace Deppo.Core.DataStores
{
    public class WarehouseCountingDataStore : IWarehouseCountingService
    {
        private string postUrl = "/gateway/customQuery/CustomQuery";

        public async Task<DataResult<IEnumerable<dynamic>>> GetNegativeProducts(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetNegativeProducts(firmNumber, periodNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<IEnumerable<dynamic>>> GetNegativeWarehousesByProductReferenceId(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetNegativeWarehouses(firmNumber, periodNumber, productReferenceId)), Encoding.UTF8, "application/json");

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

        private string GetNegativeProducts(int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
        {
            var baseQuery = @$"SELECT 
    [ReferenceId] = NEWID(),
    [ProductReferenceId] = ITEMS.LOGICALREF,
    [ProductCode] = ITEMS.CODE,
    [ProductName] = ITEMS.NAME,
    [UnitsetReferenceId] = UNITSETF.LOGICALREF,
    [UnitsetCode] = UNITSETF.CODE,
    [UnitsetName] = UNITSETF.NAME,
    [SubUnitsetReferenceId] = UNITSETL.LOGICALREF,
    [SubUnitsetCode] = UNITSETL.CODE,
    [SubUnitsetName] = UNITSETL.NAME,
    [IsVariant] = ITEMS.CANCONFIGURE,
    [TrackingType] = ITEMS.TRACKTYPE,
    [LocTracking] = ITEMS.LOCTRACKING,
    [StockQuantity] = ISNULL(SUM(CASE WHEN STINVTOT.ONHAND < 0 THEN STINVTOT.ONHAND ELSE 0 END), 0)
FROM 
    LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK)
LEFT JOIN 
    LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON STINVTOT.STOCKREF = ITEMS.LOGICALREF
LEFT JOIN
    LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON ITEMS.UNITSETREF = UNITSETF.LOGICALREF
LEFT JOIN 
    LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON UNITSETL.UNITSETREF = UNITSETF.LOGICALREF AND UNITSETL.MAINUNIT = 1
WHERE 
    STINVTOT.INVENNO <> -1 ";

            if (!string.IsNullOrEmpty(search))
            {
                baseQuery += $" AND (ITEMS.CODE LIKE '%{search}%' OR ITEMS.NAME LIKE '%{search}%')";
            }

            baseQuery += @$"GROUP BY 
    ITEMS.LOGICALREF, ITEMS.CODE, ITEMS.NAME, 
    UNITSETF.LOGICALREF, UNITSETF.CODE, UNITSETF.NAME, 
    UNITSETL.LOGICALREF, UNITSETL.CODE, UNITSETL.NAME, 
    ITEMS.CANCONFIGURE, ITEMS.TRACKTYPE, ITEMS.LOCTRACKING
HAVING 
    ITEMS.CODE <> 'ÿ'
    AND ISNULL(SUM(CASE WHEN STINVTOT.ONHAND < 0 THEN STINVTOT.ONHAND ELSE 0 END), 0) < 0
ORDER BY 
    ITEMS.CODE DESC
OFFSET {skip} ROWS 
FETCH NEXT {take} ROWS ONLY;";

            return baseQuery;
        }

        private string GetNegativeWarehouses(int firmNumber, int periodNumber, int productReferenceId)
        {
            var baseQuery = @$"SELECT 
    [ReferenceId] = NEWID(),
	[WarehouseName]  = ISNULL(WHOUSE.NAME,''),
    [WarehouseNumber] = ISNULL(WHOUSE.NR,0),
    [WarehouseReferenceId] = ISNULL(WHOUSE.LOGICALREF,0),
    [UnitsetReferenceId] = ISNULL(UNITSETF.LOGICALREF,0),
    [UnitsetCode] = ISNULL(UNITSETF.CODE,''),
    [UnitsetName] = ISNULL(UNITSETF.NAME,''),
    [SubUnitsetReferenceId] = ISNULL(UNITSETL.LOGICALREF,0),
    [SubUnitsetCode] = ISNULL(UNITSETL.CODE,''),
    [SubUnitsetName] = ISNULL(UNITSETL.NAME,''),
    [IsVariant] = ISNULL(ITEMS.CANCONFIGURE,0),
    [TrackingType] = ISNULL(ITEMS.TRACKTYPE,0),
    [LocTracking] = ISNULL(ITEMS.LOCTRACKING,0),
    [StockQuantity] = ISNULL(SUM(CASE WHEN STINVTOT.ONHAND < 0 THEN STINVTOT.ONHAND ELSE 0 END), 0)
FROM 
    LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK)
LEFT JOIN 
    LG_{firmNumber.ToString().PadLeft(3, '0')}ITEMS AS ITEMS WITH(NOLOCK) ON STINVTOT.STOCKREF = ITEMS.LOGICALREF
LEFT JOIN 
    LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON ITEMS.UNITSETREF = UNITSETF.LOGICALREF
LEFT JOIN 
    LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON UNITSETL.UNITSETREF = UNITSETF.LOGICALREF AND UNITSETL.MAINUNIT = 1
	LEFT JOIN L_CAPIWHOUSE AS WHOUSE ON STINVTOT.INVENNO = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
WHERE 
    STINVTOT.INVENNO <> -1 GROUP BY 
    STINVTOT.INVENNO, ITEMS.LOGICALREF, ITEMS.CODE, ITEMS.NAME, 
    UNITSETF.LOGICALREF, UNITSETF.CODE, UNITSETF.NAME, 
    UNITSETL.LOGICALREF, UNITSETL.CODE, UNITSETL.NAME, 
    ITEMS.CANCONFIGURE, ITEMS.TRACKTYPE, ITEMS.LOCTRACKING
HAVING
    STINVTOT.INVENNO <> -1 
    AND ITEMS.CODE <> 'ÿ'
	AND ITEMS.LOGICALREF = {productReferenceId}
    AND ISNULL(SUM(CASE WHEN STINVTOT.ONHAND < 0 THEN STINVTOT.ONHAND ELSE 0 END), 0) < 0
ORDER BY 
    WHOUSE.NR
";

            return baseQuery;
        }
    }
}
