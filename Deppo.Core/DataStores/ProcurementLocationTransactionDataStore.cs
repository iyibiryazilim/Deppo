﻿using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DataStores
{
	public class ProcurementLocationTransactionDataStore : IProcurementLocationTransactionService
	{
		string postUrl = "/gateway/customQuery/CustomQuery";
		public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, int warehouseNumber, int skip = 0, int take = 20, string search = "", int locationRef = 0, int ficheReferenceId = 0, int serilotRef = 0, int variantReferenceId = 0)
		{
			var content = new StringContent(JsonConvert.SerializeObject(LocationInputTransactionQuery(firmNumber, periodNumber, productReferenceId, warehouseNumber, skip, take, search, locationRef, ficheReferenceId, serilotRef, variantReferenceId)), Encoding.UTF8, "application/json");

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

		private string LocationInputTransactionQuery(int firmNumber, int periodNumber, int productReferenceId, int warehouseNumber, int skip = 0, int take = 20, string search = "", int locationRef = 0, int ficheReferenceId = 0, int serilotRef = 0, int variantReferenceId = 0)
		{
			var baseQuery = @$"SELECT
        [ReferenceId] = LGMAIN.LOGICALREF,
        [TransactionDate] = LGMAIN.DATE_,
        [TransactionReferenceId] = LGMAIN.STTRANSREF,
        [TransactionFicheReferenceId] = LGMAIN.STFICHEREF,
		[SerilotReferenceId] = LGMAIN.SLREF,
        [InTransactionReferenceId] = LGMAIN.INTRANSREF,
        [InSerilotTransactionReferenceId] = LGMAIN.INSLTRANSREF,
        [SerilotCode] = ISNULL(SERILOT.CODE, ''),
        [SerilotName] = ISNULL(SERILOT.NAME, ''),
        [LocationReferenceId] = ISNULL(LGMAIN.LOCREF, 0),
        [LocationCode] = ISNULL(INVLOC.CODE, ''),
        [LocationName] = ISNULL(INVLOC.NAME, ''),
        [SubUnitsetReferenceId] =(SELECT LOGICALREF FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL WHERE UNITSETREF = UNITSET.LOGICALREF AND MAINUNIT = 1 ),
		[SubUnitsetCode] = (SELECT CODE FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL WHERE UNITSETREF = UNITSET.LOGICALREF AND MAINUNIT = 1 ),
		[SubUnitsetName] = (SELECT LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL.NAME FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL WHERE UNITSETREF = UNITSET.LOGICALREF AND MAINUNIT = 1 ),
        [UnitsetReferenceId] = UNITSET.LOGICALREF,
		[UnitsetCode] = UNITSET.CODE,
		[UnitsetName] = UNITSET.NAME,
        [ItemReferenceId] = ITEMS.LOGICALREF,
		[ItemCode] = ITEMS.CODE,
		[ItemName] = ITEMS.NAME,
        [Quantity] = LGMAIN.AMOUNT * (LGMAIN.UINFO2/LGMAIN.UINFO1),
        [RemainingQuantity] = LGMAIN.REMAMOUNT * (LGMAIN.UINFO2/LGMAIN.UINFO1),
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
              (LGMAIN.STFICHEREF = {ficheReferenceId}) AND
              (LGMAIN.INVENNO = {warehouseNumber}) AND
			  (LGMAIN.VARIANTREF = {variantReferenceId}) AND
		      (LGMAIN.EXIMFCTYPE IN ( 0 , 4 , 5 , 3 , 2 , 7 )) AND 
              (LGMAIN.STATUS = 0) AND 
              (LGMAIN.REMAMOUNT > 0) AND
              (LGMAIN.IOCODE IN (1,2))
              ";

			if (!string.IsNullOrEmpty(search))
			{
				baseQuery += $@" AND (INVLOC.CODE LIKE '{search}%' OR INVLOC.NAME LIKE '%{search}%')";
			}

			if (locationRef != 0)
			{
				baseQuery += $@" AND INVLOC.LOGICALREF = {locationRef}";
			}

			baseQuery += $@" ORDER BY INVLOC.CODE OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

			return baseQuery;
		}
	}
}
