using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System.Text;

namespace Deppo.Core.DataStores
{
    public class VariantWarehouseTotalDataStore : IVariantWarehouseTotalService
    {
        private string postUrl = "/gateway/customQuery/CustomQuery";
        public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber,int productReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "")
        {

            var content = new StringContent(JsonConvert.SerializeObject(VariantWarehouseTotalQuery(firmNumber, periodNumber, warehouseNumber,productReferenceId, search, skip, take, externalDb)), Encoding.UTF8, "application/json");

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

        private string VariantWarehouseTotalQuery(int firmNumber, int periodNumber, int warehouseNumber,int productReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "")
        {
            string baseQuery = $@"SELECT 
[ReferenceId] = NEWID(),
[ProductReferenceId] =  ITEMS.LOGICALREF,
[StockQuantity] = ISNULL(SUM(VRNTINVTOT.ONHAND),0),
[ProductCode] = ITEMS.CODE,
[ProductName] = ITEMS.NAME,
[VariantReferenceId] = VARIANT.LOGICALREF,
[VariantCode] = VARIANT.CODE,
[VariantName] = VARIANT.NAME,
[UnitsetReferenceId] = UNITSETF.LOGICALREF,
[UnitsetCode] = UNITSETF.CODE,
[UnitsetName] = UNITSETF.NAME,
[SubUnitsetReferenceId] = UNITSETL.LOGICALREF,
[SubUnitsetCode] = UNITSETL.CODE,
[SubUnitsetName] = UNITSETL.NAME,
[IsVariant] = ITEMS.CANCONFIGURE,
[TrackingType] = ITEMS.TRACKTYPE,
[LocTracking] = ITEMS.LOCTRACKING


FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_VRNTINVTOT AS VRNTINVTOT WITH(NOLOCK)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT WITH(NOLOCK) ON VRNTINVTOT.VARIANTREF = VARIANT.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON VARIANT.ITEMREF= ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON VARIANT.UNITSETREF = UNITSETF.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON UNITSETL.UNITSETREF = UNITSETF.LOGICALREF AND UNITSETL.MAINUNIT = 1
LEFT JOIN {externalDb}L_CAPIWHOUSE AS WHOUSE WITH(NOLOCK) ON VRNTINVTOT.INVENNO = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_INVDEF AS INVDEF WITH(NOLOCK) ON VRNTINVTOT.INVENNO = INVDEF.INVENNO AND VRNTINVTOT.VARIANTREF = INVDEF.VARIANTREF
WHERE VRNTINVTOT.STOCKREF = {productReferenceId}
";

            if (!string.IsNullOrEmpty(search))
                baseQuery += $@" AND (VARIANT.CODE LIKE '{search}%' OR VARIANT.NAME LIKE '%{search}%')";

            baseQuery += $@" GROUP BY VRNTINVTOT.INVENNO,
ITEMS.LOGICALREF,
ITEMS.CODE,
ITEMS.NAME,
WHOUSE.LOGICALREF,
WHOUSE.NR,
WHOUSE.NAME,
UNITSETF.LOGICALREF,
UNITSETF.CODE,
UNITSETF.NAME,
UNITSETL.LOGICALREF,
UNITSETL.CODE,
UNITSETL.NAME, ITEMS.CANCONFIGURE, ITEMS.TRACKTYPE, ITEMS.LOCTRACKING,ITEMS.CARDTYPE,ITEMS.MOLD,VARIANT.LOGICALREF,VARIANT.CODE,VARIANT.NAME
 HAVING
    VRNTINVTOT.INVENNO = {warehouseNumber}
    AND VARIANT.CODE <> '�'
ORDER BY VARIANT.CODE DESC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";


            return baseQuery;
        }
    }
}
