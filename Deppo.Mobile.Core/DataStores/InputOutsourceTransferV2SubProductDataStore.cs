using Deppo.Core.DataResultModel;
using Deppo.Mobile.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.DataStores;

public class InputOutsourceTransferV2SubProductDataStore : IInputOutsourceTransferV2SubProductService
{
	string postUrl = "gateway/customQuery/CustomQuery";
	public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int mainProductReferenceId)
	{
		var content = new StringContent(JsonConvert.SerializeObject(GetObjectsQuery(firmNumber, periodNumber, mainProductReferenceId)), Encoding.UTF8, "application/json");

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

	string GetObjectsQuery(int firmNumber, int periodNumber, int mainProductReferenceId)
	{
		string baseQuery = @$"
		SELECT 
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
		[LocTracking] = ITEMS.LOCTRACKING,
		[TrackingType] = ITEMS.TRACKTYPE,
		[InWarehouseNumber] = CLCARD.OUTINVENNR,
		[WarehouseNumber] = WHOUSE.NR,
        [WarehouseName] = WHOUSE.NAME,
		[BOMQuantity] = BOMLINE.AMOUNT,
		[StockQuantity] = ISNULL((SELECT SUM(ONHAND) FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK) WHERE STINVTOT.STOCKREF = ITEMS.LOGICALREF AND STINVTOT.INVENNO = CLCARD.OUTINVENNR), 0),
		[Image] = ISNULL((SELECT TOP 1 FIRMDOC.LDATA
					 FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC
					 WHERE FIRMDOC.INFOREF = ITEMS.LOGICALREF
					 AND FIRMDOC.INFOTYP = 20
					 AND FIRMDOC.DOCNR = 11), '')
		FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_BOMLINE AS BOMLINE WITH(NOLOCK)
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_BOMASTER AS BOMASTER WITH(NOLOCK) ON BOMLINE.BOMMASTERREF = BOMASTER.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON BOMLINE.ITEMREF = ITEMS.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON BOMLINE.UOMREF = UNITSETL.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON BOMLINE.USREF = UNITSETF.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD WITH(NOLOCK) ON BOMASTER.CLIENTREF = CLCARD.LOGICALREF
		LEFT JOIN L_CAPIWHOUSE AS WHOUSE WITH(NOLOCK) ON CLCARD.OUTINVENNR = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
		WHERE BOMASTER.MAINPRODREF = {mainProductReferenceId} AND BOMLINE.BOMTYPE = 2 AND BOMLINE.LINETYPE <> 4
		";

		return baseQuery;
	}

}
