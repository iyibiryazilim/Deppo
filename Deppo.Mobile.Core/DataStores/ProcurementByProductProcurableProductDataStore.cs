using Deppo.Core.DataResultModel;
using Deppo.Mobile.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.DataStores;

public class ProcurementByProductProcurableProductDataStore : IProcurementByProductProcurableProductService
{
	private string postUrl = "/gateway/customQuery/CustomQuery";
	public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, int warehouseNumber, string search = "", int skip = 0, int take = 20)
	{
		var content = new StringContent(JsonConvert.SerializeObject(ProcurableProductQuery(firmNumber, periodNumber, productReferenceId, warehouseNumber,search, skip, take)), Encoding.UTF8, "application/json");

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

	private string ProcurableProductQuery(int firmNumber, int periodNumber, int productReferenceId, int warehouseNumber, string search = "", int skip = 0, int take = 20)
	{
		string baseQuery = @$"
		SELECT
			[ItemReferenceId] = ITEMS.LOGICALREF,
			[ItemCode] = ITEMS.CODE,
			[ItemName] = ITEMS.NAME,
			[MainItemReferenceId] = ITEMS.LOGICALREF,
            [MainItemCode] = ITEMS.CODE,
            [MainItemName] = ITEMS.NAME,
            [Image] = '',
			[UnitsetReferenceId] = UNITSETF.LOGICALREF,
			[UnitsetCode] = UNITSETF.CODE,
			[UnitsetName] = UNITSETF.NAME,
			[SubUnitsetReferenceId] = UNITSETL.LOGICALREF,
			[SubUnitsetCode] = UNITSETL.CODE,
			[SubUnitsetName] = UNITSETL.NAME,
			[IsVariant] = ITEMS.CANCONFIGURE,
			[TrackingType] = ITEMS.TRACKTYPE,
			[LocTracking] = ITEMS.LOCTRACKING,
			[StockQuantity] = ISNULL(SUM(STINVTOT.ONHAND), 0),
            [Image] = ISNULL((SELECT TOP 1 FIRMDOC.LDATA
                 FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC
                 WHERE FIRMDOC.INFOREF = ITEMS.LOGICALREF
                 AND FIRMDOC.INFOTYP = 20
                 AND FIRMDOC.DOCNR = 11), '')
		FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK)
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON STINVTOT.STOCKREF = ITEMS.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON ITEMS.UNITSETREF = UNITSETF.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON UNITSETL.UNITSETREF = UNITSETF.LOGICALREF AND UNITSETL.MAINUNIT = 1
		WHERE STINVTOT.STOCKREF = {productReferenceId} AND STINVTOT.INVENNO <> -1 AND STINVTOT.INVENNO = {warehouseNumber} AND ITEMS.MOLD = 0 AND ITEMS.CODE <> '�' AND ITEMS.CARDTYPE <> 4";

		if (!string.IsNullOrEmpty(search))
		{
			baseQuery += $" AND (ITEMS.CODE LIKE '%{search}%' OR ITEMS.NAME LIKE '%{search}%')";
		}

		baseQuery += $@" GROUP BY ITEMS.LOGICALREF, ITEMS.CODE, ITEMS.NAME, UNITSETF.LOGICALREF, UNITSETF.CODE, UNITSETF.NAME, UNITSETL.LOGICALREF, UNITSETL.CODE, UNITSETL.NAME,
				ITEMS.CANCONFIGURE, ITEMS.TRACKTYPE, ITEMS.LOCTRACKING
		";

		baseQuery += $" ORDER BY ITEMS.CODE OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

		return baseQuery;
	}
}