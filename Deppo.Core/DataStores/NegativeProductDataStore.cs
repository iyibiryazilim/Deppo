using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System.Text;

namespace Deppo.Core.DataStores
{
	public class NegativeProductDataStore : INegativeProductService
	{
		private string postUrl = "gateway/customQuery/CustomQuery";
		public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
		{
			var content = new StringContent(JsonConvert.SerializeObject(NegativeProductQuery(firmNumber, periodNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

		private string NegativeProductQuery(int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
		{
			string baseQuery = $@"SELECT *
FROM (
    SELECT
        [ReferenceId] = ITEMS.LOGICALREF,
        [Code] = ITEMS.CODE,
        [Name] = ITEMS.NAME,
        [VatRate] = ITEMS.VAT,
        [UnitsetReferenceId] = UNITSETF.LOGICALREF,
        [UnitsetCode] = UNITSETF.CODE,
        [UnitsetName] = UNITSETF.NAME,
        [SubUnitsetReferenceId] = UNITSETL.LOGICALREF,
        [SubUnitsetCode] = UNITSETL.CODE,
        [SubUnitsetName] = UNITSETL.NAME,
        [IsVariant] = ITEMS.CANCONFIGURE,
        [TrackingType] = ITEMS.TRACKTYPE,
        [LocTracking] = ITEMS.LOCTRACKING,
        [GroupCode] = ISNULL(ITEMS.STGRPCODE, ''),
        [BrandReferenceId] = ISNULL(BRAND.LOGICALREF, 0),
        [BrandCode] = ISNULL(BRAND.CODE, ''),
        [BrandName] = ISNULL(BRAND.DESCR, ''),
        [Image] = ISNULL(FIRMDOC.LDATA, ''),
        [IsPurchased] = ISNULL(ITMFACTP.PROCURECLASS, 0),
             [StockQuantity] = ROUND(ISNULL((SELECT SUM(ONHAND) FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK) WHERE STINVTOT.STOCKREF = ITEMS.LOGICALREF AND STINVTOT.INVENNO = -1), 0), 2)
    FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK)
    LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON ITEMS.UNITSETREF = UNITSETF.LOGICALREF
    LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON UNITSETF.LOGICALREF = UNITSETL.UNITSETREF AND UNITSETL.MAINUNIT = 1
    LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC ON FIRMDOC.INFOREF = ITEMS.LOGICALREF AND FIRMDOC.INFOTYP = 20 AND FIRMDOC.DOCNR = 11
    LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND WITH(NOLOCK) ON ITEMS.MARKREF = BRAND.LOGICALREF
    LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITMFACTP AS ITMFACTP WITH(NOLOCK) ON ITMFACTP.ITEMREF = ITEMS.LOGICALREF AND VARIANTREF = 0
    WHERE ITEMS.ACTIVE = 0 
      AND ITEMS.MOLD = 0 
      AND TOOL = 0 
      AND ITEMS.CARDTYPE NOT IN (1, 4, 13) 
      AND ITEMS.UNITSETREF <> 0
) AS dd
WHERE dd.StockQuantity < 0";

			if (!string.IsNullOrEmpty(search))
				baseQuery += $@" AND (dd.Code LIKE '{search}%' OR dd.Name LIKE '%{search}%')";

			baseQuery += $@" ORDER BY dd.Code ASC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY;";

			return baseQuery;
		}

	}
}
