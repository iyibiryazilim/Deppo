using Deppo.Core.DataResultModel;
using Deppo.Mobile.Core.Services;
using Newtonsoft.Json;
using System.Text;

namespace Deppo.Mobile.Core.DataStores;

public class SalesCustomerProductDataStore : ISalesCustomerProductService
{
    private string postUrl = "/gateway/customQuery/CustomQuery";

    public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int customerReferenceId, int warehouseNumber, string search = "", int skip = 0, int take = 20)
    {
        var content = new StringContent(JsonConvert.SerializeObject(SalesCustomerProductQuery(firmNumber, periodNumber, customerReferenceId, warehouseNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int customerReferenceId, int warehouseNumber, int shipInfoReferenceId, string search = "", int skip = 0, int take = 20)
    {
        var content = new StringContent(JsonConvert.SerializeObject(SalesCustomerProductWithShipInfoQuery(firmNumber, periodNumber, customerReferenceId, warehouseNumber, shipInfoReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

    private string SalesCustomerProductQuery(int firmNumber, int periodNumber, int customerReferenceId, int warehouseNumber, string search = "", int skip = 0, int take = 20)
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
            [LocTracking] = ITEMS.LOCTRACKING,
            [TrackingType] = ITEMS.TRACKTYPE,
            [Quantity] = SUM(ORFLINE.AMOUNT),
            [ShippedQuantity] = SUM(ORFLINE.SHIPPEDAMOUNT),
			[WaitingQuantity] = ISNULL(SUM((ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT)), 0)
		FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE WITH(NOLOCK)
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFICHE AS ORFICHE WITH(NOLOCK) ON ORFLINE.ORDFICHEREF = ORFICHE.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD WITH(NOLOCK) ON ORFICHE.CLIENTREF = CLCARD.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON ORFLINE.STOCKREF = ITEMS.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET WITH(NOLOCK) ON ORFLINE.UOMREF = SUBUNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET WITH(NOLOCK) ON ORFLINE.USREF = UNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT WITH(NOLOCK) ON ORFLINE.VARIANTREF = VARIANT.LOGICALREF
		WHERE CLCARD.LOGICALREF = {customerReferenceId} AND ORFLINE.CLOSED = 0 AND ORFLINE.USREF <> 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 1 AND ORFLINE.SOURCEINDEX = {warehouseNumber}
		";

        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";

        baseQuery += @$" GROUP BY ITEMS.LOGICALREF, ITEMS.CODE, ITEMS.NAME, VARIANT.LOGICALREF, ORFLINE.VARIANTREF, VARIANT.CODE,
		VARIANT.NAME, UNITSET.LOGICALREF, UNITSET.CODE,UNITSET.NAME, SUBUNITSET.LOGICALREF, SUBUNITSET.CODE, SUBUNITSET.NAME, ITEMS.CANCONFIGURE, ITEMS.LOCTRACKING, ITEMS.TRACKTYPE ORDER BY ITEMS.CODE ASC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }

    private string SalesCustomerProductWithShipInfoQuery(int firmNumber, int periodNumber, int customerReferenceId, int warehouseNumber, int shipInfoReferenceId = 0, string search = "", int skip = 0, int take = 20)
    {
        string baseQuery = $@" WITH BaseQuery AS (
  SELECT
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
            [LocTracking] = ITEMS.LOCTRACKING,
            [TrackingType] = ITEMS.TRACKTYPE,
            [Quantity] = SUM(ORFLINE.AMOUNT),
            [ShippedQuantity] = SUM(ORFLINE.SHIPPEDAMOUNT),
			[WaitingQuantity] = ISNULL(SUM((ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT)), 0)
		FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE WITH(NOLOCK)
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFICHE AS ORFICHE WITH(NOLOCK) ON ORFLINE.ORDFICHEREF = ORFICHE.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD WITH(NOLOCK) ON ORFICHE.CLIENTREF = CLCARD.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON ORFLINE.STOCKREF = ITEMS.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET WITH(NOLOCK) ON ORFLINE.UOMREF = SUBUNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET WITH(NOLOCK) ON ORFLINE.USREF = UNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT WITH(NOLOCK) ON ORFLINE.VARIANTREF = VARIANT.LOGICALREF
		WHERE CLCARD.LOGICALREF = {customerReferenceId} AND ORFLINE.CLOSED = 0 AND ORFLINE.USREF <> 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 1 AND ORFLINE.SOURCEINDEX = {warehouseNumber} AND ORFICHE.SHIPINFOREF = {shipInfoReferenceId}
		 GROUP BY ITEMS.LOGICALREF, ITEMS.CODE, ITEMS.NAME, VARIANT.LOGICALREF, ORFLINE.VARIANTREF, VARIANT.CODE,
		VARIANT.NAME, UNITSET.LOGICALREF, UNITSET.CODE,UNITSET.NAME, SUBUNITSET.LOGICALREF, SUBUNITSET.CODE, SUBUNITSET.NAME, ITEMS.CANCONFIGURE, ITEMS.LOCTRACKING, ITEMS.TRACKTYPE ORDER BY ITEMS.CODE ASC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY
)
SELECT
    BQ.ItemReferenceId,
	BQ.ItemCode,
	BQ.ItemName,
	BQ.MainItemReferenceId,
	BQ.MainItemCode,
	BQ.MainItemName,
	BQ.IsVariant,
	BQ.UnitsetReferenceId,
	BQ.UnitsetCode,
	BQ.UnitsetName,
	BQ.SubUnitsetReferenceId,
	BQ.SubUnitsetCode,
	BQ.SubUnitsetName,
	BQ.LocTracking,
	BQ.TrackingType,
	BQ.Quantity,
	BQ.ShippedQuantity,
	BQ.WaitingQuantity,
	FIRMDOC.LDATA AS Image
FROM
    BaseQuery AS BQ
    LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC ON FIRMDOC.INFOREF = BQ.ItemReferenceId AND FIRMDOC.INFOTYP = 20
 WHERE (BQ.ItemCode LIKE '{search}%' OR BQ.ItemName LIKE '%{search}%')";

        return baseQuery;
    }
}