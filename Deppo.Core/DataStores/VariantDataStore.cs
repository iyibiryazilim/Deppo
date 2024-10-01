using System;
using System.Text;
using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;

namespace Deppo.Core.DataStores;

public class VariantDataStore : IVariantService
{
    private string postUrl = "/gateway/customQuery/CustomQuery";

    public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber,int productReferenceId, int warehouseNumber, string search = "", int skip = 0, int take = 20)
    {

        var content = new StringContent(JsonConvert.SerializeObject(VariantQuery(firmNumber, periodNumber,productReferenceId, warehouseNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<IEnumerable<dynamic>>> GetVariants(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20)
    {

        var content = new StringContent(JsonConvert.SerializeObject(GetVariantsQuery(firmNumber, periodNumber, productReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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




    public async Task<DataResult<IEnumerable<dynamic>>> GetProductVariants(HttpClient httpClient, int firmNumber, int periodNumber, int variantReferenceId, string search = "", int skip = 0, int take = 20)
    {

        var content = new StringContent(JsonConvert.SerializeObject(GetProductVariantsQuery(firmNumber, periodNumber, variantReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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




    private string VariantQuery(int firmNumber, int periodNumber,int productReferenceId, int warehouseNumber, string search = "", int skip = 0, int take = 20)
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
[StockQuantity] = ISNULL(SUM(STINVTOT.ONHAND),0)

FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON STINVTOT.STOCKREF = ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON ITEMS.UNITSETREF = UNITSETF.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON UNITSETL.UNITSETREF = UNITSETF.LOGICALREF AND UNITSETL.MAINUNIT = 1
LEFT JOIN L_CAPIWHOUSE AS WHOUSE WITH(NOLOCK) ON STINVTOT.INVENNO = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
WHERE STINVTOT.INVENNO = {warehouseNumber}";

        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";

        baseQuery += $@" GROUP BY ITEMS.LOGICALREF,ITEMS.CODE,ITEMS.NAME,WHOUSE.LOGICALREF,WHOUSE.NR,WHOUSE.NAME,UNITSETF.LOGICALREF,UNITSETF.CODE,UNITSETF.NAME,UNITSETL.LOGICALREF,UNITSETL.CODE,UNITSETL.NAME,STINVTOT.DATE_
ORDER BY STINVTOT.DATE_ DESC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";


        return baseQuery;
    }




    private string GetVariantsQuery(int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20)
    {
        string baseQuery = $@" select 
			   [ReferenceId] = ISNULL(Variant.LOGICALREF,0), 
			   [Name] = ISNULL(Variant.NAME,''), 
			   [Code] = ISNULL(Variant.CODE,''),
			   [ProductReferenceId] = ISNULL (Variant.ITEMREF,0)
			   from LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT as Variant 
			   where Variant.ITEMREF = {productReferenceId}" ;
        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" AND (Variant.CODE LIKE '{search}%' OR Variant.NAME LIKE '%{search}%')";
        baseQuery += $@" ORDER BY Variant.CODE DESC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";
        return baseQuery;
    }


    private string GetProductVariantsQuery(int firmNumber, int periodNumber, int variantReferenceId, string search = "", int skip = 0, int take = 20)
    {
        string baseQuery = $@" select
[CharCodeReferenceId] = CCODE.LOGICALREF,
[CharCodeName] = CCODE.NAME,
[CharValReferenceId] = CVAL.LOGICALREF,
[CharValName] = CVAL.NAME
from LG_{firmNumber.ToString().PadLeft(3, '0')}_VRNTCHARASGN as VRNTCHRASGN
left join LG_{firmNumber.ToString().PadLeft(3, '0')}_CHARCODE AS CCODE ON CCODE.LOGICALREF = VRNTCHRASGN.CHARCODEREF
left join  LG_{firmNumber.ToString().PadLeft(3, '0')}_CHARVAL as CVAL ON CVAL.CHARCODEREF = CCODE.LOGICALREF AND CVAL.LOGICALREF = VRNTCHRASGN.CHARVALREF

WHERE VRNTCHRASGN.VARIANTREF = {variantReferenceId}";
        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" AND (Variant.CODE LIKE '{search}%' OR Variant.NAME LIKE '%{search}%')";
        baseQuery += $@" ORDER BY CCODE.NAME DESC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";
        return baseQuery;
    }


}
