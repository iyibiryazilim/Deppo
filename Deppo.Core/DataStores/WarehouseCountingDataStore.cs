using Deppo.Core.BaseModels;
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

        //Ambara Bağlı Menüsündeki Ambar Listesi
        public async Task<DataResult<IEnumerable<dynamic>>> GetWarehouses(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetWarehouses(firmNumber, periodNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<IEnumerable<dynamic>>> GetProductsByWarehouseAndLocation(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int locationReferenceId, string search = "", int skip = 0, int take = 20)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetProductsByWarehouseAndLocation(firmNumber, periodNumber, warehouseNumber, locationReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<IEnumerable<dynamic>>> GetVariantsByWarehouseAndLocation(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int locationReferenceId, string search = "", int skip = 0, int take = 20)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetVariantsByWarehouseAndLocation(firmNumber, periodNumber, warehouseNumber, locationReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<IEnumerable<dynamic>>> GetProductsByWarehouse(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, string search = "", int skip = 0, int take = 20)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetProductsByWarehouse(firmNumber, periodNumber, warehouseNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<IEnumerable<dynamic>>> GetVariantsByWarehouse(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, string search = "", int skip = 0, int take = 20)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetVariantsByWarehouse(firmNumber, periodNumber, warehouseNumber, search, skip, take)), Encoding.UTF8, "application/json");

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
    [Image] = ISNULL((SELECT TOP 1 FIRMDOC.LDATA
                 FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC
                 WHERE FIRMDOC.INFOREF = ITEMS.LOGICALREF
                 AND FIRMDOC.INFOTYP = 20
                 AND FIRMDOC.DOCNR = 11), ''),
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

        private string GetWarehouses(int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
        {
            var baseQuery = @$"SELECT
			[ReferenceId] = LGMAIN.LOGICALREF,
			[Number] = LGMAIN.NR,
			[Name] = LGMAIN.NAME,
			[DivisionReferenceId] = 0,
			[DivisionNumber] = LGMAIN.DIVISNR,
			[City] = LGMAIN.CITY,
			[Country] = LGMAIN.COUNTRY,
            [LocationCount] = (SELECT ISNULL(COUNT(*),0) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_LOCATION INVLOC WHERE INVLOC.INVENNR = LGMAIN.NR)
			FROM L_CAPIWHOUSE AS LGMAIN WITH (NOLOCK)

            WHERE LGMAIN.FIRMNR = {firmNumber}";

            if (!string.IsNullOrEmpty(search))
            {
                baseQuery += $" AND (LGMAIN.NAME LIKE '%{search}%' OR LGMAIN.NR LIKE '%{search}%')";
            }

            baseQuery += @$"
ORDER BY
    LGMAIN.NR ASC
OFFSET {skip} ROWS
FETCH NEXT {take} ROWS ONLY;";

            return baseQuery;
        }

        private string GetProductsByWarehouseAndLocation(int firmNumber, int periodNumber, int warehouseNumber, int locationReferenceId, string search = "", int skip = 0, int take = 20)
        {
            var baseQuery = @$"SELECT
    [MainItemReferenceId] = CASE
        WHEN VARIANT.LOGICALREF IS NOT NULL AND VARIANT.LOGICALREF <> 0 THEN ITEMS.LOGICALREF
        ELSE 0
    END,
    [MainItemCode] = CASE
        WHEN VARIANT.CODE IS NOT NULL AND VARIANT.CODE <> '' THEN ITEMS.CODE
        ELSE ''
    END,
    [MainItemName] = CASE
        WHEN VARIANT.NAME IS NOT NULL AND VARIANT.NAME <> '' THEN ITEMS.NAME
        ELSE ''
    END,
    [ItemReferenceId] = CASE
        WHEN VARIANT.LOGICALREF IS NULL OR VARIANT.LOGICALREF = 0 THEN ITEMS.LOGICALREF
        ELSE VARIANT.LOGICALREF
    END,
    [ItemCode] = CASE
        WHEN VARIANT.CODE IS NULL OR VARIANT.CODE = '' THEN ISNULL(ITEMS.CODE, '')
        ELSE VARIANT.CODE
    END,
    [ItemName] = CASE
        WHEN VARIANT.NAME IS NULL OR VARIANT.NAME = '' THEN ISNULL(ITEMS.NAME, '')
        ELSE VARIANT.NAME
    END,
	[UnitsetReferenceId] = ISNULL(UNITSETF.LOGICALREF,0),
    [UnitsetCode] = ISNULL(UNITSETF.CODE,''),
    [UnitsetName] = ISNULL(UNITSETF.NAME,''),
    [SubUnitsetReferenceId] =(SELECT LOGICALREF FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL WHERE UNITSETREF = UNITSETF.LOGICALREF AND MAINUNIT = 1 ),
	[SubUnitsetCode] = (SELECT CODE FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL WHERE UNITSETREF = UNITSETF.LOGICALREF AND MAINUNIT = 1 ),
	[SubUnitsetName] = (SELECT LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL.NAME FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL WHERE UNITSETREF = UNITSETF.LOGICALREF AND MAINUNIT = 1 ),
     [Image] = ISNULL((SELECT TOP 1 FIRMDOC.LDATA
                 FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC
                 WHERE FIRMDOC.INFOREF = ITEMS.LOGICALREF
                 AND FIRMDOC.INFOTYP = 20
                 AND FIRMDOC.DOCNR = 11), ''),
	[LocTracking] = ISNULL(ITEMS.LOCTRACKING,0),
    [StockQuantity] = ISNULL(SUM(LGMAIN.REMAMOUNT * (LGMAIN.UINFO2/LGMAIN.UINFO1)),0)
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_SLTRANS LGMAIN WITH(NOLOCK)
LEFT OUTER JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_LOCATION INVLOC WITH(NOLOCK) ON (LGMAIN.LOCREF = INVLOC.LOGICALREF)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON LGMAIN.ITEMREF = ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT ON LGMAIN.VARIANTREF = VARIANT.LOGICALREF
LEFT JOIN L_CAPIWHOUSE AS WHOUSE ON LGMAIN.INVENNO = WHOUSE.NR AND FIRMNR = {firmNumber}
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON ITEMS.UNITSETREF = UNITSETF.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC ON FIRMDOC.INFOREF = ITEMS.LOGICALREF AND FIRMDOC.INFOTYP = 20  AND FIRMDOC.DOCNR = 11
WHERE LGMAIN.CANCELLED = 0
  AND LGMAIN.LPRODSTAT = 0
  AND LGMAIN.EXIMFCTYPE IN (0, 4, 5, 3, 2, 7)
  AND LGMAIN.STATUS = 0
  AND LGMAIN.IOCODE IN (1, 2, 3, 4)
  AND INVLOC.LOGICALREF = {locationReferenceId}
  AND LGMAIN.INVENNO = {warehouseNumber}
  AND ITEMS.CANCONFIGURE = 0";

            if (!string.IsNullOrEmpty(search))
            {
                baseQuery += $" AND (ITEMS.NAME LIKE '%{search}%' OR ITEMS.CODE LIKE '{search}%')";
            }

            baseQuery += @$"
GROUP BY ITEMS.LOGICALREF,WHOUSE.NR,ITEMS.LOCTRACKING, ITEMS.CODE, ITEMS.NAME,UNITSETF.LOGICALREF, UNITSETF.CODE, UNITSETF.NAME,VARIANT.LOGICALREF,VARIANT.CODE,VARIANT.NAME
ORDER BY ITEMS.CODE
OFFSET {skip} ROWS
FETCH NEXT {take} ROWS ONLY;";

            return baseQuery;
        }

        private string GetVariantsByWarehouseAndLocation(int firmNumber, int periodNumber, int warehouseNumber, int locationReferenceId, string search = "", int skip = 0, int take = 20)
        {
            var baseQuery = @$"SELECT
    [MainItemReferenceId] = CASE
        WHEN VARIANT.LOGICALREF IS NOT NULL AND VARIANT.LOGICALREF <> 0 THEN ITEMS.LOGICALREF
        ELSE 0
    END,
    [MainItemCode] = CASE
        WHEN VARIANT.CODE IS NOT NULL AND VARIANT.CODE <> '' THEN ITEMS.CODE
        ELSE ''
    END,
    [MainItemName] = CASE
        WHEN VARIANT.NAME IS NOT NULL AND VARIANT.NAME <> '' THEN ITEMS.NAME
        ELSE ''
    END,
    [ItemReferenceId] = CASE
        WHEN VARIANT.LOGICALREF IS NULL OR VARIANT.LOGICALREF = 0 THEN ITEMS.LOGICALREF
        ELSE VARIANT.LOGICALREF
    END,
    [ItemCode] = CASE
        WHEN VARIANT.CODE IS NULL OR VARIANT.CODE = '' THEN ISNULL(ITEMS.CODE, '')
        ELSE VARIANT.CODE
    END,
    [ItemName] = CASE
        WHEN VARIANT.NAME IS NULL OR VARIANT.NAME = '' THEN ISNULL(ITEMS.NAME, '')
        ELSE VARIANT.NAME
    END,
	[UnitsetReferenceId] = ISNULL(UNITSETF.LOGICALREF,0),
    [UnitsetCode] = ISNULL(UNITSETF.CODE,''),
    [UnitsetName] = ISNULL(UNITSETF.NAME,''),
   [SubUnitsetReferenceId] =(SELECT LOGICALREF FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL WHERE UNITSETREF = UNITSETF.LOGICALREF AND MAINUNIT = 1 ),
	[SubUnitsetCode] = (SELECT CODE FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL WHERE UNITSETREF = UNITSETF.LOGICALREF AND MAINUNIT = 1 ),
	[SubUnitsetName] = (SELECT LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL.NAME FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL WHERE UNITSETREF = UNITSETF.LOGICALREF AND MAINUNIT = 1 ),
	[LocTracking] = ISNULL(ITEMS.LOCTRACKING,0),
    [StockQuantity] = ISNULL(SUM(LGMAIN.REMAMOUNT* (LGMAIN.UINFO2/LGMAIN.UINFO1)),0)
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_SLTRANS LGMAIN WITH(NOLOCK)
LEFT OUTER JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_LOCATION INVLOC WITH(NOLOCK) ON (LGMAIN.LOCREF = INVLOC.LOGICALREF)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON LGMAIN.ITEMREF = ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT ON LGMAIN.VARIANTREF = VARIANT.LOGICALREF
LEFT JOIN L_CAPIWHOUSE AS WHOUSE ON LGMAIN.INVENNO = WHOUSE.NR AND FIRMNR = {firmNumber}
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON ITEMS.UNITSETREF = UNITSETF.LOGICALREF
WHERE LGMAIN.CANCELLED = 0
  AND LGMAIN.LPRODSTAT = 0
  AND LGMAIN.EXIMFCTYPE IN (0, 4, 5, 3, 2, 7)
  AND LGMAIN.STATUS = 0
  AND LGMAIN.IOCODE IN (1, 2, 3, 4)
  AND INVLOC.LOGICALREF = {locationReferenceId}
  AND LGMAIN.INVENNO = {warehouseNumber}
  AND ITEMS.CANCONFIGURE = 1";

            if (!string.IsNullOrEmpty(search))
            {
                baseQuery += $" AND (ITEMS.NAME LIKE '%{search}%' OR ITEMS.CODE LIKE '{search}%')";
            }

            baseQuery += @$"
GROUP BY ITEMS.LOGICALREF,WHOUSE.NR,ITEMS.LOCTRACKING, ITEMS.CODE, ITEMS.NAME,UNITSETF.LOGICALREF, UNITSETF.CODE, UNITSETF.NAME,VARIANT.LOGICALREF,VARIANT.CODE,VARIANT.NAME
ORDER BY ITEMS.CODE
OFFSET {skip} ROWS
FETCH NEXT {take} ROWS ONLY;";

            return baseQuery;
        }

        private string GetProductsByWarehouse(int firmNumber, int periodNumber, int warehouseNumber, string search = "", int skip = 0, int take = 20)
        {
            var baseQuery = @$"SELECT
[ItemReferenceId] =  ITEMS.LOGICALREF,
[ItemCode] = ITEMS.CODE,
[ItemName] = ITEMS.NAME,
[UnitsetReferenceId] = UNITSETF.LOGICALREF,
[UnitsetCode] = UNITSETF.CODE,
[UnitsetName] = UNITSETF.NAME,
[SubUnitsetReferenceId] = UNITSETL.LOGICALREF,
[SubUnitsetCode] = UNITSETL.CODE,
[SubUnitsetName] = UNITSETL.NAME,
[LocTracking] = ITEMS.LOCTRACKING,
[StockQuantity] = ISNULL(SUM(STINVTOT.ONHAND),0),
[Image] = ISNULL((SELECT TOP 1 FIRMDOC.LDATA
                 FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC
                 WHERE FIRMDOC.INFOREF = ITEMS.LOGICALREF
                 AND FIRMDOC.INFOTYP = 20
                 AND FIRMDOC.DOCNR = 11), ''),

FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON STINVTOT.STOCKREF = ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON ITEMS.UNITSETREF = UNITSETF.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON UNITSETL.UNITSETREF = UNITSETF.LOGICALREF AND UNITSETL.MAINUNIT = 1
LEFT JOIN L_CAPIWHOUSE AS WHOUSE WITH(NOLOCK) ON STINVTOT.INVENNO = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}";

            if (!string.IsNullOrEmpty(search))
            {
                baseQuery += $" AND (ITEMS.NAME LIKE '%{search}%' OR ITEMS.CODE LIKE '{search}%')";
            }

            baseQuery += @$"
GROUP BY STINVTOT.INVENNO, ITEMS.LOGICALREF,ITEMS.CODE,ITEMS.NAME,UNITSETF.LOGICALREF,UNITSETF.CODE,UNITSETF.NAME,UNITSETL.LOGICALREF,UNITSETL.CODE,UNITSETL.NAME, ITEMS.LOCTRACKING,ITEMS.CARDTYPE,ITEMS.MOLD,ITEMS.CANCONFIGURE
 HAVING
    STINVTOT.INVENNO = {warehouseNumber}
    AND ITEMS.CODE <> '�'
    AND ITEMS.CARDTYPE <> 4 AND ITEMS.MOLD = 0
    AND ITEMS.CANCONFIGURE = 0
    AND ISNULL(SUM(STINVTOT.ONHAND), 0) <> 0
ORDER BY ITEMS.CODE DESC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

            return baseQuery;
        }

        private string GetVariantsByWarehouse(int firmNumber, int periodNumber, int warehouseNumber, string search = "", int skip = 0, int take = 20)
        {
            var baseQuery = @$"
SELECT
[ItemReferenceId] = VARIANT.LOGICALREF,
[ItemName] = VARIANT.NAME,
[ItemCode] = VARIANT.CODE,
[MainItemReferenceId] =  ITEMS.LOGICALREF,
[MainItemCode] = ITEMS.CODE,
[MainItemName] = ITEMS.NAME,
[UnitsetReferenceId] = UNITSETF.LOGICALREF,
[UnitsetCode] = UNITSETF.CODE,
[UnitsetName] = UNITSETF.NAME,
[SubUnitsetReferenceId] = UNITSETL.LOGICALREF,
[SubUnitsetCode] = UNITSETL.CODE,
[SubUnitsetName] = UNITSETL.NAME,
[LocTracking] = ITEMS.LOCTRACKING,
[StockQuantity] = ISNULL(SUM(VRNTINVTOT.ONHAND),0)
FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_VRNTINVTOT AS VRNTINVTOT WITH(NOLOCK)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT ON VRNTINVTOT.VARIANTREF = VARIANT.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON VARIANT.ITEMREF = ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON VARIANT.UNITSETREF = UNITSETF.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON UNITSETL.UNITSETREF = UNITSETF.LOGICALREF AND UNITSETL.MAINUNIT = 1
LEFT JOIN L_CAPIWHOUSE AS WHOUSE WITH(NOLOCK) ON VRNTINVTOT.INVENNO = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}";

            if (!string.IsNullOrEmpty(search))
            {
                baseQuery += $" AND (VARIANT.NAME LIKE '%{search}%' OR VARIANT.CODE LIKE '{search}%')";
            }

            baseQuery += @$"
GROUP BY VRNTINVTOT.INVENNO, ITEMS.LOGICALREF,ITEMS.CODE,ITEMS.NAME,UNITSETF.LOGICALREF,UNITSETF.CODE,UNITSETF.NAME,UNITSETL.LOGICALREF,UNITSETL.CODE,UNITSETL.NAME, ITEMS.LOCTRACKING,ITEMS.CARDTYPE,ITEMS.MOLD,VARIANT.LOGICALREF,VARIANT.NAME,VARIANT.CODE,ITEMS.CANCONFIGURE
 HAVING
    VRNTINVTOT.INVENNO = {warehouseNumber}
    AND ITEMS.CODE <> '�'
    AND ITEMS.CARDTYPE <> 4 AND ITEMS.MOLD = 0
    AND ITEMS.CANCONFIGURE = 1
    AND ISNULL(SUM(VRNTINVTOT.ONHAND), 0) <> 0
ORDER BY ITEMS.CODE DESC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

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
    LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON STINVTOT.STOCKREF = ITEMS.LOGICALREF
LEFT JOIN
    LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON ITEMS.UNITSETREF = UNITSETF.LOGICALREF
LEFT JOIN
    LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON UNITSETL.UNITSETREF = UNITSETF.LOGICALREF AND UNITSETL.MAINUNIT = 1
	LEFT JOIN L_CAPIWHOUSE AS WHOUSE ON STINVTOT.INVENNO = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
WHERE
    STINVTOT.INVENNO <> -1 GROUP BY
    STINVTOT.INVENNO, WHOUSE.NR, WHOUSE.NAME, WHOUSE.LOGICALREF,
    ITEMS.CODE, ITEMS.LOGICALREF,
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