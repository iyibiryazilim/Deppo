using System;
using System.Text;
using Deppo.Core.DataResultModel;
using Deppo.Mobile.Core.Services;
using Newtonsoft.Json;

namespace Deppo.Mobile.Core.DataStores;

public class ProcurementByCustomerProductDataStore : IProcurementByCustomerProductService
{
    private string postUrl = "/gateway/customQuery/CustomQuery";

    public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int orderWarehouseNumber, int customerReferenceId, int shipAddressReferenceId = 0, string search = "", int skip = 0, int take = 20)
    {
        var content = new StringContent(JsonConvert.SerializeObject(ProcurementByCustomerProductListQueryV2(firmNumber, periodNumber, warehouseNumber, orderWarehouseNumber, customerReferenceId, shipAddressReferenceId,search, skip, take)), Encoding.UTF8, "application/json");

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

	private string ProcurementByCustomerProductListQueryV2(int firmNumber, int periodNumber, int warehouseNumber, int orderWarehouseNumber, int customerReferenceId, int shipAddressReferenceId = 0, string search = "", int skip = 0, int take = 20)
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
            [Image] = ISNULL((SELECT TOP 1 FIRMDOC.LDATA
                 FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC
                 WHERE FIRMDOC.INFOREF = ITEMS.LOGICALREF
                 AND FIRMDOC.INFOTYP = 20
                 AND FIRMDOC.DOCNR = 11), ''),
			[DispatchAmount] = ISNULL((SELECT SUM(AMOUNT) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
           LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE ON STLINE.STFICHEREF = STFICHE.LOGICALREF WHERE STLINE.ORDTRANSREF > 0 AND STFICHE.STATUS = 1 AND STFICHE.CLIENTREF = {customerReferenceId} AND STFICHE.TRCODE = 8 AND STFICHE.SHIPINFOREF = {shipAddressReferenceId} AND STLINE.STOCKREF = ITEMS.LOGICALREF),0),
            [ShippedQuantity] = SUM(ORFLINE.SHIPPEDAMOUNT),
            [StockQuantity] = ISNULL((SELECT SUM(ONHAND) FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK) WHERE STINVTOT.STOCKREF = ITEMS.LOGICALREF AND STINVTOT.INVENNO = {warehouseNumber}),0),
			[WaitingQuantity] = ISNULL(SUM(ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT), 0)
		FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE WITH(NOLOCK)
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFICHE AS ORFICHE WITH(NOLOCK) ON ORFLINE.ORDFICHEREF = ORFICHE.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD WITH(NOLOCK) ON ORFICHE.CLIENTREF = CLCARD.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON ORFLINE.STOCKREF = ITEMS.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET WITH(NOLOCK) ON ORFLINE.UOMREF = SUBUNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET WITH(NOLOCK) ON ORFLINE.USREF = UNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT WITH(NOLOCK) ON ORFLINE.VARIANTREF = VARIANT.LOGICALREF
		WHERE CLCARD.LOGICALREF = {customerReferenceId} AND ORFLINE.CLOSED = 0 AND ORFLINE.USREF <> 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 1 AND ORFLINE.SOURCEINDEX = {orderWarehouseNumber} AND  ORFICHE.SHIPINFOREF = {shipAddressReferenceId}
		";

		if (!string.IsNullOrEmpty(search))
			baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";

		baseQuery += @$" GROUP BY ITEMS.LOGICALREF, ITEMS.CODE, ITEMS.NAME, VARIANT.LOGICALREF, ORFLINE.VARIANTREF, VARIANT.CODE,
		VARIANT.NAME, UNITSET.LOGICALREF, UNITSET.CODE,UNITSET.NAME, SUBUNITSET.LOGICALREF, SUBUNITSET.CODE, SUBUNITSET.NAME, ITEMS.CANCONFIGURE, ITEMS.LOCTRACKING, ITEMS.TRACKTYPE ORDER BY ITEMS.CODE ASC  OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

		return baseQuery;
	}




	private string ProcurementByCustomerProductListQuery(int firmNumber, int periodNumber, int warehouseNumber, int orderWarehouseNumber, int customerReferenceId, string search = "", int skip = 0, int take = 20)
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
            [Image] = ISNULL((SELECT TOP 1 FIRMDOC.LDATA
                 FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC
                 WHERE FIRMDOC.INFOREF = ITEMS.LOGICALREF
                 AND FIRMDOC.INFOTYP = 20
                 AND FIRMDOC.DOCNR = 11), ''),

            [ShippedQuantity] = SUM(ORFLINE.SHIPPEDAMOUNT),
            [StockQuantity] = ISNULL((SELECT SUM(ONHAND) FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK) WHERE STINVTOT.STOCKREF = ITEMS.LOGICALREF AND STINVTOT.INVENNO = {warehouseNumber}),0),
			[WaitingQuantity] = ISNULL(SUM((ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT)), 0)
		FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE WITH(NOLOCK)
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFICHE AS ORFICHE WITH(NOLOCK) ON ORFLINE.ORDFICHEREF = ORFICHE.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD WITH(NOLOCK) ON ORFICHE.CLIENTREF = CLCARD.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON ORFLINE.STOCKREF = ITEMS.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET WITH(NOLOCK) ON ORFLINE.UOMREF = SUBUNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET WITH(NOLOCK) ON ORFLINE.USREF = UNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT WITH(NOLOCK) ON ORFLINE.VARIANTREF = VARIANT.LOGICALREF
		WHERE CLCARD.LOGICALREF = {customerReferenceId} AND ORFLINE.CLOSED = 0 AND ORFLINE.USREF <> 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 1 AND ORFLINE.SOURCEINDEX = {orderWarehouseNumber}
		";

        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";

        baseQuery += @$" GROUP BY ITEMS.LOGICALREF, ITEMS.CODE, ITEMS.NAME, VARIANT.LOGICALREF, ORFLINE.VARIANTREF, VARIANT.CODE,
		VARIANT.NAME, UNITSET.LOGICALREF, UNITSET.CODE,UNITSET.NAME, SUBUNITSET.LOGICALREF, SUBUNITSET.CODE, SUBUNITSET.NAME, ITEMS.CANCONFIGURE, ITEMS.LOCTRACKING, ITEMS.TRACKTYPE ORDER BY ITEMS.CODE ASC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }
}