using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System.Text;

namespace Deppo.Core.DataStores;

public class SeriLotTransactionDataStore : ISeriLotTransactionService
{
	string postUrl = "/gateway/customQuery/CustomQuery";

	public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, int warehouseNumber, int skip = 0, int take = 20, string search = "")
	{
		var content = new StringContent(JsonConvert.SerializeObject(SeriLotTransactionQuery(firmNumber, periodNumber, productReferenceId, warehouseNumber, skip, take, search)), Encoding.UTF8, "application/json");

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

	private string SeriLotTransactionQuery(int firmNumber, int periodNumber, int productReferenceId, int warehouseNumber, int skip = 0, int take = 20, string search = "")
	{
		var baseQuery = $@"SELECT
        [ReferenceId] = LGMAIN.LOGICALREF,
        [TransactionReferenceId] = LGMAIN.STTRANSREF,
        [TransactionFicheReferenceId] = LGMAIN.STFICHEREF,
		[SerilotReferenceId] = LGMAIN.SLREF,
        [InTransactionReferenceId] = LGMAIN.INTRANSREF,
        [InSerilotTransactionReferenceId] = LGMAIN.INSLTRANSREF,
        [SerilotCode] = ISNULL(SERILOT.CODE, ''),
        [SerilotName] = ISNULL(SERILOT.NAME, ''),
        [LocationReferenceId] = ISNULL(LGMAIN.LOCREF, 0),
        [LocationCode] = ISNULL(INVLOC.CODE, 0),
        [LocationName] = ISNULL(INVLOC.NAME, 0),
        [SubUnitsetReferenceId] =(SELECT LOGICALREF FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL WHERE UNITSETREF = UNITSET.LOGICALREF AND MAINUNIT = 1 ),
		[SubUnitsetCode] = (SELECT CODE FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL WHERE UNITSETREF = UNITSET.LOGICALREF AND MAINUNIT = 1 ),
		[SubUnitsetName] = (SELECT LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL.NAME FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL WHERE UNITSETREF = UNITSET.LOGICALREF AND MAINUNIT = 1 ),
        [UnitsetReferenceId] = UNITSET.LOGICALREF,
        [UnitsetCode] = UNITSET.CODE,
        [UnitsetName] = UNITSET.NAME,
        [ItemReferenceId] = ITEMS.LOGICALREF,
        [ItemCode] = ITEMS.CODE,
        [ItemName] = ITEMS.NAME,
        [Quantity] = LGMAIN.AMOUNT* (LGMAIN.UINFO2/LGMAIN.UINFO1),
        [RemainingQuantity] = LGMAIN.REMAMOUNT* (LGMAIN.UINFO2/LGMAIN.UINFO1),
        [RemainingUnitQuantity] = LGMAIN.REMLNUNITAMNT
        FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_SLTRANS LGMAIN WITH(NOLOCK)    
		LEFT OUTER JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS ITEMS WITH(NOLOCK) ON (LGMAIN.ITEMREF  =  ITEMS.LOGICALREF) 
		LEFT OUTER JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_LOCATION INVLOC WITH(NOLOCK) ON (LGMAIN.LOCREF  =  INVLOC.LOGICALREF)
        LEFT OUTER JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_SERILOTN SERILOT WITH(NOLOCK) ON (LGMAIN.SLREF = SERILOT.LOGICALREF)
		LEFT OUTER JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE STFIC WITH(NOLOCK) ON (LGMAIN.STFICHEREF  =  STFIC.LOGICALREF) 
		LEFT OUTER JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF UNITSET WITH(NOLOCK) ON (ITEMS.UNITSETREF  =  UNITSET.LOGICALREF)
		LEFT OUTER JOIN L_CAPIWHOUSE WHOUSE WITH(NOLOCK) ON (LGMAIN.INVENNO = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber})
        WHERE (LGMAIN.CANCELLED = 0) AND 
              (LGMAIN.LPRODSTAT = 0) AND 
              (LGMAIN.ITEMREF = {productReferenceId}) AND
              (LGMAIN.INVENNO = {warehouseNumber}) AND
		      (LGMAIN.EXIMFCTYPE IN ( 0 , 4 , 5 , 3 , 2 , 7 )) AND 
              (LGMAIN.STATUS = 0) AND 
              (LGMAIN.REMAMOUNT > 0) 	  
		";

		if (!string.IsNullOrEmpty(search))
		{
			baseQuery += $" AND ( SERILOT.CODE LIKE '{search}% OR SERILOT.NAME LIKE '%{search}%')";
		}

		baseQuery += $" ORDER BY SERILOT.CODE ASC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

		return baseQuery;
	}
}
