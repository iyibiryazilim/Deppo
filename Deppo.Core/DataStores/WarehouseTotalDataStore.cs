using System;
using System.Text;
using Deppo.Core.DataResultModel;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Core.SortModels;
using Newtonsoft.Json;

namespace Deppo.Core.DataStores;

public class WarehouseTotalDataStore : IWarehouseTotalService
{
    private string postUrl = "/gateway/customQuery/CustomQuery";

    public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, string search = "", int skip = 0, int take = 20)
    {
        var content = new StringContent(JsonConvert.SerializeObject(WarehouseTotalQuery(firmNumber, periodNumber, warehouseNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<IEnumerable<dynamic>>> GetObjectsByProduct(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20)
    {
        var content = new StringContent(JsonConvert.SerializeObject(WarehouseTotalByProductQuery(firmNumber, periodNumber, productReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<IEnumerable<dynamic>>> TotalQuery(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
    {
        var content = new StringContent(JsonConvert.SerializeObject(TotalQuery(firmNumber, periodNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

    private string WarehouseTotalQuery(int firmNumber, int periodNumber, int warehouseNumber, string search = "", int skip = 0, int take = 20)
    {
        string baseQuery = $@"WITH BaseQuery AS (
   SELECT
[ReferenceId] = NEWID(),
[ProductReferenceId] =  ITEMS.LOGICALREF,
[ProductCode] = ITEMS.CODE,
[ProductName] = ITEMS.NAME,
[WarehouseReferenceId] = WHOUSE.LOGICALREF,
[WarehouseNumber] = WHOUSE.NR,
[WarehouseName] = WHOUSE.NAME,
[UnitsetReferenceId] = UNITSETF.LOGICALREF,
[UnitsetCode] = UNITSETF.CODE,
[UnitsetName] = UNITSETF.NAME,
[SubUnitsetReferenceId] = UNITSETL.LOGICALREF,
[SubUnitsetCode] = UNITSETL.CODE,
[SubUnitsetName] = UNITSETL.NAME,
[IsVariant] = ITEMS.CANCONFIGURE,
[TrackingType] = ITEMS.TRACKTYPE,
[LocTracking] = ITEMS.LOCTRACKING,
[BrandReferenceId] = ISNULL(BRAND.LOGICALREF,0),
[BrandCode] = ISNULL(BRAND.CODE,''),
[BrandName] = ISNULL(BRAND.DESCR,''),
[StockQuantity] = ISNULL(SUM(STINVTOT.ONHAND),0)

FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON STINVTOT.STOCKREF = ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON ITEMS.UNITSETREF = UNITSETF.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON UNITSETL.UNITSETREF = UNITSETF.LOGICALREF AND UNITSETL.MAINUNIT = 1
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND WITH(NOLOCK) ON ITEMS.MARKREF = BRAND.LOGICALREF
LEFT JOIN L_CAPIWHOUSE AS WHOUSE WITH(NOLOCK) ON STINVTOT.INVENNO = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_INVDEF AS INVDEF WITH(NOLOCK) ON STINVTOT.INVENNO = INVDEF.INVENNO AND STINVTOT.STOCKREF = INVDEF.ITEMREF AND INVDEF.VARIANTREF = 0

WHERE INVDEF.OUTCTRL <> 2";

		if (!string.IsNullOrEmpty(search))
			baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";

		baseQuery += $@" GROUP BY STINVTOT.INVENNO, ITEMS.LOGICALREF,ITEMS.CODE,ITEMS.NAME,WHOUSE.LOGICALREF,WHOUSE.NR,WHOUSE.NAME,UNITSETF.LOGICALREF,UNITSETF.CODE,UNITSETF.NAME,UNITSETL.LOGICALREF,UNITSETL.CODE,UNITSETL.NAME, ITEMS.CANCONFIGURE, ITEMS.TRACKTYPE, ITEMS.LOCTRACKING,ITEMS.CARDTYPE,ITEMS.MOLD,BRAND.LOGICALREF,BRAND.CODE,BRAND.DESCR
 HAVING
    STINVTOT.INVENNO = {warehouseNumber}
    AND ITEMS.CODE <> '�'
    AND ITEMS.CARDTYPE <> 4 AND ITEMS.MOLD = 0
    AND ISNULL(SUM(STINVTOT.ONHAND), 0) <> 0
ORDER BY ITEMS.CODE DESC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY
)
SELECT
    BQ.ReferenceId,
    BQ.ProductReferenceId,
    BQ.ProductCode,
    BQ.ProductName,
    BQ.WarehouseReferenceId,
    BQ.WarehouseNumber,
    BQ.WarehouseName,
    BQ.UnitsetReferenceId,
    BQ.UnitsetCode,
	BQ.UnitsetName,
    BQ.SubUnitsetReferenceId,
    BQ.SubUnitsetCode,
    BQ.SubUnitsetName,
	BQ.IsVariant,
	BQ.TrackingType,
	BQ.LocTracking,
	BQ.BrandReferenceId,
	BQ.BrandCode,
	BQ.BrandName,
    BQ.StockQuantity,
    FIRMDOC.LDATA AS Image
FROM
    BaseQuery AS BQ
    LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC ON FIRMDOC.INFOREF = BQ.ProductReferenceId AND FIRMDOC.INFOTYP = 20
";

        return baseQuery;
    }

    private string TotalQuery(int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
    {
        string baseQuery = $@"SELECT
[ReferenceId] = NEWID(),
[ProductReferenceId] =  ITEMS.LOGICALREF,
[ProductCode] = ITEMS.CODE,
[ProductName] = ITEMS.NAME,
[WarehouseReferenceId] = WHOUSE.LOGICALREF,
[WarehouseNumber] = WHOUSE.NR,
[WarehouseName] = WHOUSE.NAME,
[UnitsetReferenceId] = UNITSETF.LOGICALREF,
[UnitsetCode] = UNITSETF.CODE,
[UnitsetName] = UNITSETF.NAME,
[SubUnitsetReferenceId] = UNITSETL.LOGICALREF,
[SubUnitsetCode] = UNITSETL.CODE,
[SubUnitsetName] = UNITSETL.NAME,
[IsVariant] = ITEMS.CANCONFIGURE,
[TrackingType] = ITEMS.TRACKTYPE,
[LocTracking] = ITEMS.LOCTRACKING,
[BrandReferenceId] = ISNULL(BRAND.LOGICALREF,0),
[BrandCode] = ISNULL(BRAND.CODE,''),
[BrandName] = ISNULL(BRAND.DESCR,''),
[StockQuantity] = ISNULL(SUM(STINVTOT.ONHAND),0)

FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON STINVTOT.STOCKREF = ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON ITEMS.UNITSETREF = UNITSETF.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON UNITSETL.UNITSETREF = UNITSETF.LOGICALREF AND UNITSETL.MAINUNIT = 1
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND WITH(NOLOCK) ON ITEMS.MARKREF = BRAND.LOGICALREF
LEFT JOIN L_CAPIWHOUSE AS WHOUSE WITH(NOLOCK) ON STINVTOT.INVENNO = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
";

        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" WHERE (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";

        baseQuery += $@" GROUP BY STINVTOT.INVENNO, ITEMS.LOGICALREF,ITEMS.CODE,ITEMS.NAME,WHOUSE.LOGICALREF,WHOUSE.NR,WHOUSE.NAME,UNITSETF.LOGICALREF,UNITSETF.CODE,UNITSETF.NAME,UNITSETL.LOGICALREF,UNITSETL.CODE,UNITSETL.NAME, ITEMS.CANCONFIGURE, ITEMS.TRACKTYPE, ITEMS.LOCTRACKING,BRAND.LOGICALREF,BRAND.CODE,BRAND.DESCR
 HAVING
     ITEMS.CODE <> '�'
ORDER BY ITEMS.CODE DESC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }

    private string WarehouseTotalByProductQuery(int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20)
    {
        string baseQuery = $@"SELECT
[ReferenceId] = NEWID(),
[ProductReferenceId] =  ITEMS.LOGICALREF,
[ProductCode] = ITEMS.CODE,
[ProductName] = ITEMS.NAME,
[WarehouseReferenceId] = WHOUSE.LOGICALREF,
[WarehouseNumber] = WHOUSE.NR,
[WarehouseName] = WHOUSE.NAME,
[UnitsetReferenceId] = UNITSETF.LOGICALREF,
[UnitsetCode] = UNITSETF.CODE,
[UnitsetName] = UNITSETF.NAME,
[SubUnitsetReferenceId] = UNITSETL.LOGICALREF,
[SubUnitsetCode] = UNITSETL.CODE,
[SubUnitsetName] = UNITSETL.NAME,
[IsVariant] = ITEMS.CANCONFIGURE,
[TrackingType] = ITEMS.TRACKTYPE,
[LocTracking] = ITEMS.LOCTRACKING,
[BrandReferenceId] = ISNULL(BRAND.LOGICALREF,0),
[BrandCode] = ISNULL(BRAND.CODE,''),
[BrandName] = ISNULL(BRAND.DESCR,''),
[StockQuantity] = ISNULL(SUM(STINVTOT.ONHAND),0),
[LocationCount] = (SELECT ISNULL(COUNT(*),0) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_LOCATION INVLOC WHERE INVLOC.INVENNR = STINVTOT.INVENNO)

FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON STINVTOT.STOCKREF = ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON ITEMS.UNITSETREF = UNITSETF.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON UNITSETL.UNITSETREF = UNITSETF.LOGICALREF AND UNITSETL.MAINUNIT = 1
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND WITH(NOLOCK) ON ITEMS.MARKREF = BRAND.LOGICALREF
LEFT JOIN L_CAPIWHOUSE AS WHOUSE WITH(NOLOCK) ON STINVTOT.INVENNO = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
";

        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" WHERE (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";

        baseQuery += $@" GROUP BY STINVTOT.INVENNO, ITEMS.LOGICALREF,ITEMS.CODE,ITEMS.NAME,WHOUSE.LOGICALREF,WHOUSE.NR,WHOUSE.NAME,UNITSETF.LOGICALREF,UNITSETF.CODE,UNITSETF.NAME,UNITSETL.LOGICALREF,UNITSETL.CODE,UNITSETL.NAME, ITEMS.CANCONFIGURE, ITEMS.TRACKTYPE, ITEMS.LOCTRACKING,ITEMS.CARDTYPE,ITEMS.MOLD,BRAND.LOGICALREF,BRAND.CODE,BRAND.DESCR,STINVTOT.STOCKREF
 HAVING
    STINVTOT.STOCKREF = {productReferenceId}
    AND ITEMS.CODE <> '�'
    AND STINVTOT.INVENNO <> -1
    AND ITEMS.CARDTYPE <> 4 AND ITEMS.MOLD = 0
ORDER BY ITEMS.CODE DESC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }
}