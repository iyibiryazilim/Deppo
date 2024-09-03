using System;
using System.Text;
using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;

namespace Deppo.Core.DataStores;

public class WaitingSalesOrderDataStore : IWaitingSalesOrderService
{
	private string postUrl = "/gateway/customQuery/CustomQuery";
	public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
	{
		var content = new StringContent(JsonConvert.SerializeObject(WaitingSalesOrderQuery(firmNumber, periodNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

	private string WaitingSalesOrderQuery(int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
	{
		string baseQuery = @$"SELECT
			[ReferenceId] = ORFLINE.LOGICALREF,
            [OrderReferenceId] = ORFICHE.LOGICALREF,
            [CustomerReferenceId] = CLCARD.LOGICALREF,
            [CustomerCode] = CLCARD.CODE,
            [CustomerName] = CLCARD.DEFINITION_,
            [ProductReferenceId] = ITEMS.LOGICALREF,
            [ProductCode] = ITEMS.CODE,
            [ProductName] = ITEMS.NAME,
            [UnitsetReferenceId] = UNITSET.LOGICALREF,
            [UnitsetCode] = UNITSET.CODE,
            [UnitsetName] = UNITSET.NAME,
            [SubUnitsetReferenceId] = SUBUNITSET.LOGICALREF,
            [SubUnitsetCode] = SUBUNITSET.CODE,
            [SubUnitsetName] = SUBUNITSET.NAME,
            [IsVariant] = ITEMS.CANCONFIGURE,
            [Quantity] = ORFLINE.AMOUNT,
            [ShippedQuantity] = ORFLINE.SHIPPEDAMOUNT,
            [WaitingQuantity] = (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT)
            
        FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFICHE AS ORFICHE ON ORFLINE.ORDFICHEREF = ORFICHE.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON ORFICHE.CLIENTREF = CLCARD.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON ORFLINE.STOCKREF = ITEMS.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON ORFLINE.UOMREF = SUBUNITSET.LOGICALREF AND MAINUNIT = 1
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON ORFLINE.USREF = UNITSET.LOGICALREF 
		WHERE ORFLINE.CLOSED = 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 1
		AND ITEMS.UNITSETREF <> 0
		";

		if (!string.IsNullOrEmpty(search))
		{
			baseQuery += $@" AND ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";
		}

		baseQuery += $@" ORDER BY ORFLINE.DATE_ DESC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

		return baseQuery;
	}
}
