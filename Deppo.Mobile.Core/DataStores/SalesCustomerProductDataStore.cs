using Deppo.Core.DataResultModel;
using Deppo.Mobile.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.DataStores;

public class SalesCustomerProductDataStore : ISalesCustomerProductService
{
	private string postUrl = "/gateway/customQuery/CustomQuery";
	public async Task<DataResult<IEnumerable<dynamic>>> GetObjectsByCustomer(HttpClient httpClient, int firmNumber, int periodNumber, int customerReferenceId, int skip, int take)
	{
		var content = new StringContent(JsonConvert.SerializeObject(SalesCustomerProductQuery(firmNumber, periodNumber, customerReferenceId, skip, take)), Encoding.UTF8, "application/json");

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

	private string SalesCustomerProductQuery(int firmNumber, int periodNumber, int customerReferenceId, int skip, int take)
	{
		string baseQuery = $@"SELECT
			[ItemReferenceId] = CASE WHEN ORFLINE.VARIANTREF <> 0 THEN VARIANT.LOGICALREF ELSE ITEMS.LOGICALREF END,
            [ItemCode] = CASE WHEN ORFLINE.VARIANTREF <> 0 THEN VARIANT.CODE ELSE ITEMS.CODE END,
            [ItemName] = CASE WHEN ORFLINE.VARIANTREF <> 0 THEN VARIANT.NAME ELSE ITEMS.NAME END,
            [MainItemReferenceId] = ITEMS.LOGICALREF,
            [MainItemCode] = ITEMS.CODE,
            [MainItemName] = ITEMS.NAME,
			[IsVariant] = ITEMS.CANCONFIGURE,
            [UnitsetReferenceId] = UNITSET.LOGICALREF,
            [UnitsetCode] = UNITSET.CODE,
            [UnitsetName] = UNITSET.NAME,
            [SubUnitsetReferenceId] = SUBUNITSET.LOGICALREF,
            [SubUnitsetCode] = SUBUNITSET.CODE,
            [SubUnitsetName] = SUBUNITSET.NAME,
            [Quantity] = SUM(ORFLINE.AMOUNT),
            [ShippedQuantity] = SUM(ORFLINE.SHIPPEDAMOUNT),
			[WaitingQuantity] = ISNULL(SUM((ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT)), 0)
		FROM LG_{firmNumber.ToString().PadLeft(3,'0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFICHE AS ORFICHE ON ORFLINE.ORDFICHEREF = ORFICHE.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON ORFLINE.STOCKREF = ITEMS.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON ORFLINE.UOMREF = SUBUNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON ORFLINE.USREF = UNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT ON ORFLINE.VARIANTREF = VARIANT.LOGICALREF
		WHERE ORFLINE.CLOSED = 0 AND ORFLINE.CLIENTREF = {customerReferenceId} AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 2 AND ITEMS.UNITSETREF <> 0
        GROUP BY ITEMS.LOGICALREF, ITEMS.CODE, ITEMS.NAME, ORFLINE.VARIANTREF, ITEMS.CANCONFIGURE, UNITSET.LOGICALREF, UNITSET.CODE, UNITSET.NAME, SUBUNITSET.LOGICALREF, SUBUNITSET.CODE, SUBUNITSET.NAME, VARIANT.CODE, VARIANT.NAME,ORFLINE.VARIANTREF, VARIANT.LOGICALREF
		";

		baseQuery += $" ORDER BY ITEMS.CODE OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";
		
		return baseQuery;
	}
}
