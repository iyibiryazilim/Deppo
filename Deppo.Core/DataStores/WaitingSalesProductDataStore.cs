using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DataStores;

public class WaitingSalesProductDataStore : IWaitingSalesProductService
{
	private string postUrl = "gateway/customQuery/CustomQuery";

	public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
	{
		var content = new StringContent(JsonConvert.SerializeObject(GetObjectsQuery(firmNumber, periodNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

	public async Task<DataResult<IEnumerable<dynamic>>> GetOrderById(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId)
	{
		var content = new StringContent(JsonConvert.SerializeObject(GetOrderObjects(firmNumber, periodNumber, productReferenceId)), Encoding.UTF8, "application/json");

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

	private string GetObjectsQuery(int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
	{
		var baseQuery = @$"SELECT
            [ProductReferenceId] = ITEMS.LOGICALREF,
            [ProductCode] = ISNULL(ITEMS.CODE, ''),
            [ProductName] = ISNULL(ITEMS.NAME, ''),
            [UnitsetReferenceId] = UNITSET.LOGICALREF,
            [UnitsetCode] = UNITSET.CODE,
            [UnitsetName] = UNITSET.NAME,
            [SubUnitsetReferenceId] = SUBUNITSET.LOGICALREF,
            [SubUnitsetCode] = SUBUNITSET.CODE,
            [SubUnitsetName] = SUBUNITSET.NAME,
            [WaitingQuantity] = SUM(ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT),
            [IsVariant] = ITEMS.CANCONFIGURE,
            [VariantReferenceId] = ORFLINE.VARIANTREF,
            [VariantCode] = ISNULL(VARIANT.CODE, ''),
            [VariantName] = ISNULL(VARIANT.NAME, ''),
            [LocTracking] = ITEMS.LOCTRACKING,
            [TrackingType] = ITEMS.TRACKTYPE,
			[Image] = ISNULL((SELECT TOP 1 FIRMDOC.LDATA 
                 FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC
                 WHERE FIRMDOC.INFOREF = ITEMS.LOGICALREF 
                 AND FIRMDOC.INFOTYP = 20  
                 AND FIRMDOC.DOCNR = 11), '')
    FROM
       LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON ORFLINE.STOCKREF = ITEMS.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT ON ORFLINE.VARIANTREF = VARIANT.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON ORFLINE.UOMREF = SUBUNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON ORFLINE.USREF = UNITSET.LOGICALREF
    WHERE
    ORFLINE.CLOSED = 0
    AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0
    AND ORFLINE.TRCODE = 1
    AND ITEMS.UNITSETREF <> 0";

        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";

        baseQuery += @$" 
GROUP BY
    ITEMS.LOGICALREF,
    ITEMS.CODE,
    ITEMS.NAME,
    UNITSET.LOGICALREF,
    UNITSET.CODE,
    UNITSET.NAME,
    SUBUNITSET.LOGICALREF,
    SUBUNITSET.CODE,
    SUBUNITSET.NAME,
    ITEMS.CANCONFIGURE,
    ORFLINE.VARIANTREF,
    VARIANT.CODE,
    VARIANT.NAME,
    ITEMS.LOCTRACKING,
    ITEMS.TRACKTYPE
HAVING
    ITEMS.CODE <> 'ÿ'
ORDER BY 
    SUM(ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) DESC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }

	private string GetOrderObjects(int firmNumber, int periodNumber, int productReferenceId)
	{
		string baseQuery = @$"SELECT
			[ReferenceId] = ORFLINE.LOGICALREF,
            [OrderReferenceId] = ORFICHE.LOGICALREF,
            [SupplierReferenceId] = CLCARD.LOGICALREF,
            [OrderNumber] = ORFICHE.FICHENO,
            [SupplierCode] = CLCARD.CODE,
            [SupplierName] = CLCARD.DEFINITION_,
            [ProductReferenceId] = ORFLINE.STOCKREF,
            [ProductCode] = ISNULL(ITEMS.CODE, ''),
            [ProductName] = ISNULL(ITEMS.NAME, ''),
            [UnitsetReferenceId] = UNITSET.LOGICALREF,
            [UnitsetCode] = UNITSET.CODE,
            [UnitsetName] = UNITSET.NAME,
            [SubUnitsetReferenceId] = SUBUNITSET.LOGICALREF,
            [SubUnitsetCode] = SUBUNITSET.CODE,
            [SubUnitsetName] = SUBUNITSET.NAME,
            [Quantity] = ORFLINE.AMOUNT,
            [ShippedQuantity] = ORFLINE.SHIPPEDAMOUNT,
            [WaitingQuantity] = (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT),
            [IsVariant] = ITEMS.CANCONFIGURE,
            [VariantReferenceId] = ORFLINE.VARIANTREF,
            [VariantCode] = ISNULL(VARIANT.CODE, ''),
			[VariantName] = ISNULL(VARIANT.NAME, ''),
            [LocTracking] = ITEMS.LOCTRACKING,
			[TrackingType] = ITEMS.TRACKTYPE,
            [OrderDate] = ORFLINE.DATE_,
            [DueDate] = ISNULL(ORFLINE.DUEDATE, ORFLINE.DATE_)
		FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFICHE AS ORFICHE ON ORFLINE.ORDFICHEREF = ORFICHE.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON ORFLINE.STOCKREF = ITEMS.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON ORFICHE.CLIENTREF = CLCARD.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT ON ORFLINE.VARIANTREF = VARIANT.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON ORFLINE.UOMREF = SUBUNITSET.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON ORFLINE.USREF = UNITSET.LOGICALREF
        LEFT JOIN L_CAPIWHOUSE AS WHOUSE ON ORFLINE.SOURCEINDEX = WHOUSE.NR AND WHOUSE.FIRMNR ={firmNumber}
		WHERE ORFLINE.CLOSED = 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 1
		AND ITEMS.UNITSETREF <> 0 AND ITEMS.LOGICALREF = {productReferenceId}";

        return baseQuery;
	}
}
