using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System.Text;

namespace Deppo.Core.DataStores
{
	public class ProcurementByLocationProductDataStore : IProcurementByLocationProductService
	{
		private string postUrl = "/gateway/customQuery/CustomQuery";

		public async Task<DataResult<IEnumerable<dynamic>>> GetProducts(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int locationReferenceId, string search = "", int skip = 0, int take = 20)
		{
			var content = new StringContent(JsonConvert.SerializeObject(GetProductsQuery(firmNumber, periodNumber, warehouseNumber, locationReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

		private string GetProductsQuery(int firmNumber, int periodNumber, int warehouseNumber, int locationReferenceId, string search = "", int skip = 0, int take = 20)
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
WHERE LGMAIN.CANCELLED = 0 
  AND LGMAIN.LPRODSTAT = 0 
  AND LGMAIN.EXIMFCTYPE IN (0, 4, 5, 3, 2, 7) 
  AND LGMAIN.STATUS = 0 
  AND LGMAIN.IOCODE IN (1, 2, 3, 4)
  AND INVLOC.LOGICALREF = {locationReferenceId} 
  AND LGMAIN.INVENNO = {warehouseNumber}        
  AND LGMAIN.REMAMOUNT > 0
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
	}
}
