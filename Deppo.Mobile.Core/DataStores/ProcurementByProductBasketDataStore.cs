using Deppo.Core.DataResultModel;
using Deppo.Mobile.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.DataStores;

public class ProcurementByProductBasketDataStore : IProcurementByProductBasketService
{
	private string postUrl = "/gateway/customQuery/CustomQuery";

	public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int[] itemsReferenceId, string search = "", int skip = 0, int take = 20)
	{
		var content = new StringContent(JsonConvert.SerializeObject(ProcurementByProductBasketQuery(firmNumber, periodNumber, warehouseNumber, itemsReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

	private string ProcurementByProductBasketQuery(int firmNumber, int periodNumber, int warehouseNumber, int[] itemsReferenceId, string search = "", int skip = 0, int take = 20)
	{
		string referenceIds = itemsReferenceId.Select(referenceId => referenceId.ToString()).Aggregate((current, next) => current + "," + next);

		string baseQuery = $@"SELECT  

[ItemReferenceId] = ITEMS.LOGICALREF,
[ItemCode] = ITEMS.CODE,
[ItemName] = ITEMS.NAME,
[VatRate] = ITEMS.VAT,
[UnitsetReferenceId] = UNITSETF.LOGICALREF,
[UnitsetCode] = UNITSETF.CODE,
[UnitsetName] = UNITSETF.NAME,
[SubUnitsetReferenceId] =(SELECT LOGICALREF FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL WHERE UNITSETREF = UNITSETF.LOGICALREF AND MAINUNIT = 1 ),
[SubUnitsetCode] = (SELECT CODE FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL WHERE UNITSETREF = UNITSETF.LOGICALREF AND MAINUNIT = 1 ),
[SubUnitsetName] = (SELECT LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL.NAME FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL WHERE UNITSETREF = UNITSETF.LOGICALREF AND MAINUNIT = 1 ),
[IsVariant] = ITEMS.CANCONFIGURE,
[TrackingType] = ITEMS.TRACKTYPE,
[LocTracking] = ITEMS.LOCTRACKING,
[GroupCode] = ISNULL(ITEMS.STGRPCODE,''),
[BrandReferenceId] = ISNULL(BRAND.LOGICALREF,0),
[BrandCode] = ISNULL(BRAND.CODE,''),
[BrandName] = ISNULL(BRAND.DESCR,''),
[LocationReferenceId] = LOC.LOGICALREF,
[LocationCode] = LOC.CODE,
[LocationName] = LOC.NAME,
[ProcurementQuantity] = ISNULL(SUM(LGMAIN.REMAMOUNT),0),
[Image] = ISNULL((SELECT TOP 1 FIRMDOC.LDATA
                 FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC
                 WHERE FIRMDOC.INFOREF = ITEMS.LOGICALREF
                 AND FIRMDOC.INFOTYP = 20
                 AND FIRMDOC.DOCNR = 11), '')
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_SLTRANS AS LGMAIN WITH(NOLOCK)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON LGMAIN.ITEMREF = ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON ITEMS.UNITSETREF = UNITSETF.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND WITH(NOLOCK) ON ITEMS.MARKREF = BRAND.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_LOCATION AS LOC WITH(NOLOCK) ON LGMAIN.LOCREF = LOC.LOGICALREF AND LOC.INVENNR = {warehouseNumber}
WHERE (LGMAIN.CANCELLED = 0) AND 
              (LGMAIN.LPRODSTAT = 0) AND 
              (LGMAIN.ITEMREF IN({referenceIds})) AND
              (LGMAIN.INVENNO = {warehouseNumber}) AND
		      (LGMAIN.EXIMFCTYPE IN ( 0 , 4 , 5 , 3 , 2 , 7 , 8 , 9)) AND 
              (LGMAIN.STATUS = 0) AND 
              (LGMAIN.REMAMOUNT > 0) AND
              (LGMAIN.IOCODE IN (1,2,3,4))";

		if (!string.IsNullOrEmpty(search))
			baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";

		baseQuery += @$" 

GROUP BY ITEMS.LOGICALREF,ITEMS.CODE,ITEMS.NAME,ITEMS.VAT,UNITSETF.LOGICALREF,UNITSETF.CODE,UNITSETF.NAME,ITEMS.CANCONFIGURE,ITEMS.TRACKTYPE,ITEMS.LOCTRACKING,ITEMS.STGRPCODE,BRAND.LOGICALREF,BRAND.CODE,BRAND.DESCR,LOC.LOGICALREF,LOC.CODE,LOC.NAME ORDER BY ITEMS.CODE ASC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

		return baseQuery;
	}
}
