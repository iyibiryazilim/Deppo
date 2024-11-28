using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System.Text;

namespace Deppo.Core.DataStores;

public class SupplierTransactionDataStore : ISupplierTransactionService
{
	private string postUrl = "/gateway/customQuery/CustomQuery";
	public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int supplierReferenceId, string search = "", int skip = 0, int take = 20)
	{
		var content = new StringContent(JsonConvert.SerializeObject(SupplierTransactionQuery(firmNumber, periodNumber, supplierReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

	private string SupplierTransactionQuery(int firmNumber, int periodNumber, int supplierReferenceId, string search = "", int skip = 0, int take = 20)
	{
		var baseQuery = @$" SELECT 
            [ReferenceId] = STLINE.LOGICALREF,
            [BaseTransactionReferenceId] = STFICHE.LOGICALREF,
            [BaseTransactionCode] = STFICHE.FICHENO,
			[TransactionDate] = STLINE.DATE_,
            [TransactionTime] = dbo.LG_INTTOTIME(STFICHE.FTIME),
            [TransactionType] = STLINE.TRCODE,
            [IOType] = STLINE.IOCODE,
            [ProductReferenceId] = ITEMS.LOGICALREF,
            [ProductCode] = ITEMS.CODE,
            [ProductName] = ITEMS.NAME,
            [UnitsetReferenceId] = UNITSET.LOGICALREF,
            [UnitsetCode] = UNITSET.CODE,
            [UnitsetName] = UNITSET.NAME,
            [SubUnitsetReferenceId] = SUBUNITSET.LOGICALREF,
            [SubUnitsetCode] = SUBUNITSET.CODE,
            [SubUnitsetName] = SUBUNITSET.NAME,
	        [SupplierReferenceId] = CLCARD.LOGICALREF,
			[SupplierCode] = CLCARD.CODE,
			[SupplierName] = CLCARD.DEFINITION_,
            [Quantity] = STLINE.AMOUNT,
            [WarehouseNumber] = CAPIWHOUSE.NR
		FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE ON STLINE.STFICHEREF = STFICHE.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON STLINE.STOCKREF = ITEMS.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON STLINE.CLIENTREF = CLCARD.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON STLINE.UOMREF = SUBUNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON STLINE.USREF = UNITSET.LOGICALREF
        LEFT JOIN L_CAPIWHOUSE AS CAPIWHOUSE ON STLINE.SOURCEINDEX = CAPIWHOUSE.NR AND CAPIWHOUSE.FIRMNR = {firmNumber}
        WHERE STLINE.IOCODE IN (1,2,3,4) AND STFICHE.TRCODE IN (1,2,3,7,6,8) AND STLINE.STFICHEREF <> 0 AND STLINE.USREF <> 0 AND STLINE.UOMREF <> 0 AND CLCARD.LOGICALREF = {supplierReferenceId}
		";

		if (!string.IsNullOrEmpty(search))
		{
			baseQuery += $" AND (ITEMS.CODE LIKE '%{search}%' OR ITEMS.NAME LIKE '%{search}%')";
		}

		baseQuery += $" ORDER BY STLINE.DATE_ DESC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

		return baseQuery;
	}
}
