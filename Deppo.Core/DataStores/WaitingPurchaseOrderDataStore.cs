using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System.Text;

namespace Deppo.Core.DataStores;

public class WaitingPurchaseOrderDataStore : IWaitingPurchaseOrderService
{
    private string postUrl = "/gateway/customQuery/CustomQuery";

    public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber,string search = "", int skip = 0, int take = 20)
    {
        var content = new StringContent(JsonConvert.SerializeObject(WaitingPurchaseOrderQueryByWarehouse(firmNumber, periodNumber,warehouseNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber,int supplierReferenceId, string search = "", int skip = 0, int take = 20)
    {
        var content = new StringContent(JsonConvert.SerializeObject(WaitingPurchaseOrderQueryBySupplierWarehouse(firmNumber, periodNumber, supplierReferenceId, warehouseNumber,search, skip, take)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
    {
        var content = new StringContent(JsonConvert.SerializeObject(WaitingPurchaseOrderQuery(firmNumber, periodNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<IEnumerable<dynamic>>> GetObjectsBySupplier(HttpClient httpClient, int firmNumber, int periodNumber, int supplierReferenceId, string search = "", int skip = 0, int take = 20)
    {
        var content = new StringContent(JsonConvert.SerializeObject(WaitingPurchaseOrderQueryBySupplier(firmNumber, periodNumber, supplierReferenceId,search, skip, take)), Encoding.UTF8, "application/json");

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

    private string WaitingPurchaseOrderQuery(int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
    {
        string baseQuery = $@"SELECT

    [ReferenceId] = ORFLINE.LOGICALREF,
    [OrderReferenceId] = ORFICHE.LOGICALREF,
    [SupplierReferenceId] = CLCARD.LOGICALREF,
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
    [IsVariant]=ITEMS.CANCONFIGURE,
    [VariantReferenceId]=ORFLINE.VARIANTREF,
    [VariantCode] = ISNULL(VARIANT.CODE, ''),
    [VariantName] = ISNULL(VARIANT.NAME, ''),
    [LocTracking] = ITEMS.LOCTRACKING,
    [TrackingType] = ITEMS.TRACKTYPE,
    [OrderDate] = ORFLINE.DATE_,
    [DueDate] = ORFLINE.DUEDATE

FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFICHE AS ORFICHE ON ORFLINE.ORDFICHEREF = ORFICHE.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON ORFLINE.STOCKREF = ITEMS.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON ORFICHE.CLIENTREF = CLCARD.LOGICALREF
            LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT ON ORFLINE.VARIANTREF = VARIANT.LOGICALREF 
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON ORFLINE.UOMREF = SUBUNITSET.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON ORFLINE.USREF = UNITSET.LOGICALREF
			LEFT JOIN L_CAPIWHOUSE AS WHOUSE ON ORFLINE.SOURCEINDEX = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
			WHERE ORFLINE.CLOSED = 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 2
 AND ITEMS.UNITSETREF <> 0";

        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";

        baseQuery += $@" ORDER BY ITEMS.CODE ASC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }

    private string WaitingPurchaseOrderQueryByWarehouse(int firmNumber, int periodNumber,int warehouseNumber, string search = "", int skip = 0, int take = 20)
    {
        string baseQuery = $@"SELECT

    [ReferenceId] = ORFLINE.LOGICALREF,
    [OrderReferenceId] = ORFICHE.LOGICALREF,
    [SupplierReferenceId] = CLCARD.LOGICALREF,
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
    [IsVariant]=ITEMS.CANCONFIGURE,
    [VariantReferenceId]=ORFLINE.VARIANTREF,
    [VariantCode] = ISNULL(VARIANT.CODE, ''),
    [VariantName] = ISNULL(VARIANT.NAME, ''),
    [LocTracking] = ITEMS.LOCTRACKING,
    [TrackingType] = ITEMS.TRACKTYPE,
    [OrderDate] = ORFLINE.DATE_,
    [DueDate] = ORFLINE.DUEDATE

FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFICHE AS ORFICHE ON ORFLINE.ORDFICHEREF = ORFICHE.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON ORFLINE.STOCKREF = ITEMS.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON ORFICHE.CLIENTREF = CLCARD.LOGICALREF
            LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT ON ORFLINE.VARIANTREF = VARIANT.LOGICALREF 
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON ORFLINE.UOMREF = SUBUNITSET.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON ORFLINE.USREF = UNITSET.LOGICALREF
			LEFT JOIN L_CAPIWHOUSE AS WHOUSE ON ORFLINE.SOURCEINDEX = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
			WHERE ORFLINE.CLOSED = 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 2 AND ORFLINE.SOURCEINDEX = {warehouseNumber}
 AND ITEMS.UNITSETREF <> 0";

        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";

        baseQuery += $@" ORDER BY ITEMS.CODE ASC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }

    private string WaitingPurchaseOrderQueryBySupplier(int firmNumber, int periodNumber, int supplierReferenceId, string search = "", int skip = 0, int take = 20)
    {
        string baseQuery = $@"SELECT

    [ReferenceId] = ORFLINE.LOGICALREF,
    [OrderReferenceId] = ORFICHE.LOGICALREF,
    [SupplierReferenceId] = CLCARD.LOGICALREF,
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
    [IsVariant]=ITEMS.CANCONFIGURE,
    [VariantReferenceId]=ORFLINE.VARIANTREF,
    [VariantCode] = ISNULL(VARIANT.CODE, ''),
    [VariantName] = ISNULL(VARIANT.NAME, ''),
    [LocTracking] = ITEMS.LOCTRACKING,
    [TrackingType] = ITEMS.TRACKTYPE,
    [OrderDate] = ORFLINE.DATE_,
    [DueDate] = ORFLINE.DUEDATE

FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFICHE AS ORFICHE ON ORFLINE.ORDFICHEREF = ORFICHE.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON ORFLINE.STOCKREF = ITEMS.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON ORFICHE.CLIENTREF = CLCARD.LOGICALREF
            LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT ON ORFLINE.VARIANTREF = VARIANT.LOGICALREF  
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON ORFLINE.UOMREF = SUBUNITSET.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON ORFLINE.USREF = UNITSET.LOGICALREF
			LEFT JOIN L_CAPIWHOUSE AS WHOUSE ON ORFLINE.SOURCEINDEX = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
			WHERE ORFLINE.CLOSED = 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 2
 AND ITEMS.UNITSETREF <> 0 AND CLCARD.LOGICALREF = {supplierReferenceId}";

        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";

        baseQuery += $@" ORDER BY ITEMS.CODE ASC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }

    private string WaitingPurchaseOrderQueryBySupplierWarehouse(int firmNumber, int periodNumber, int supplierReferenceId,int warehouseNumber, string search = "", int skip = 0, int take = 20)
    {
        string baseQuery = $@"SELECT

    [ReferenceId] = ORFLINE.LOGICALREF,
    [OrderReferenceId] = ORFICHE.LOGICALREF,
    [SupplierReferenceId] = CLCARD.LOGICALREF,
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
    [IsVariant]=ITEMS.CANCONFIGURE,
    [VariantReferenceId]=ORFLINE.VARIANTREF,
    [VariantCode] = ISNULL(VARIANT.CODE, ''),
    [VariantName] = ISNULL(VARIANT.NAME, ''),
    [LocTracking] = ITEMS.LOCTRACKING,
    [TrackingType] = ITEMS.TRACKTYPE,
    [OrderDate] = ORFLINE.DATE_,
    [DueDate] = ORFLINE.DUEDATE

FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFICHE AS ORFICHE ON ORFLINE.ORDFICHEREF = ORFICHE.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON ORFLINE.STOCKREF = ITEMS.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON ORFICHE.CLIENTREF = CLCARD.LOGICALREF
            LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT ON ORFLINE.VARIANTREF = VARIANT.LOGICALREF  
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON ORFLINE.UOMREF = SUBUNITSET.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON ORFLINE.USREF = UNITSET.LOGICALREF
			LEFT JOIN L_CAPIWHOUSE AS WHOUSE ON ORFLINE.SOURCEINDEX = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
			WHERE ORFLINE.CLOSED = 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 2
 AND ITEMS.UNITSETREF <> 0 AND CLCARD.LOGICALREF = {supplierReferenceId} AND ORFLINE.SOURCEINDEX = {warehouseNumber}";

        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";

        baseQuery += $@" ORDER BY ITEMS.CODE ASC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }
}