using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Microsoft.VisualBasic;
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

	public async Task<DataResult<IEnumerable<dynamic>>> GetObjectsByProduct(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int supplierReferenceId, int productReferenceId, string search = "", int skip = 0, int take = 20)
	{
		var content = new StringContent(JsonConvert.SerializeObject(WaitingPurchaseOrderQueryBySupplierAndWarehouseAndProduct(firmNumber, periodNumber, warehouseNumber, supplierReferenceId, productReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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
    public async Task<DataResult<IEnumerable<dynamic>>> GetSuppliers(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetSuppliersQuery(firmNumber, periodNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

	public async Task<DataResult<IEnumerable<dynamic>>> GetObjectsBySupplierAndShipInfo(HttpClient httpClient, int firmNumber, int periodNumber, int supplierReferenceId, int shipInfoReferenceId = 0, string search = "", int skip = 0, int take = 20)
	{
		var content = new StringContent(JsonConvert.SerializeObject(WaitingPurchaseOrderQueryBySupplierAndShipInfoQuery(firmNumber, periodNumber, supplierReferenceId, shipInfoReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

    [ReferenceId] = ISNULL(ORFLINE.LOGICALREF, 0),
    [OrderReferenceId] = ISNULL(ORFICHE.LOGICALREF, 0),
    [OrderNumber] = ISNULL(ORFICHE.FICHENO,''),
    [SupplierReferenceId] = ISNULL(CLCARD.LOGICALREF, 0),
    [SupplierCode] = ISNULL(CLCARD.CODE, ''),
    [SupplierName] = ISNULL(CLCARD.DEFINITION_, ''),
    [ProductReferenceId] = ISNULL(ORFLINE.STOCKREF, 0),
    [ProductCode] = ISNULL(ITEMS.CODE, ''),
    [ProductName] = ISNULL(ITEMS.NAME, ''),
    [UnitsetReferenceId] = ISNULL(UNITSET.LOGICALREF, 0),
    [UnitsetCode] = ISNULL(UNITSET.CODE, ''),
    [UnitsetName] = ISNULL(UNITSET.NAME, ''),
    [SubUnitsetReferenceId] = ISNULL(SUBUNITSET.LOGICALREF, 0),
    [SubUnitsetCode] = ISNULL(SUBUNITSET.CODE, ''),
    [SubUnitsetName] = ISNULL(SUBUNITSET.NAME, ''),
    [Quantity] = ISNULL(ORFLINE.AMOUNT, 0),
    [ShippedQuantity] = ISNULL(ORFLINE.SHIPPEDAMOUNT, 0),
    [WaitingQuantity] = ISNULL((ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT), 0),
    [IsVariant]=ISNULL(ITEMS.CANCONFIGURE, 0),
    [VariantReferenceId]=ISNULL(ORFLINE.VARIANTREF, 0),
    [VariantCode] = ISNULL(VARIANT.CODE, ''),
    [VariantName] = ISNULL(VARIANT.NAME, ''),
    [LocTracking] = ISNULL(ITEMS.LOCTRACKING, 0),
    [TrackingType] = ISNULL(ITEMS.TRACKTYPE, 0),
    [OrderDate] = ISNULL(ORFLINE.DATE_, ''),
    [DueDate] = ISNULL(ORFLINE.DUEDATE, ''),
    [Image] = ISNULL((SELECT TOP 1 FIRMDOC.LDATA
                 FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC
                 WHERE FIRMDOC.INFOREF = ITEMS.LOGICALREF
                 AND FIRMDOC.INFOTYP = 20
                 AND FIRMDOC.DOCNR = 11), '')

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
    [OrderNumber] = ORFICHE.FICHENO,
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
    [DueDate] = ORFLINE.DUEDATE,
    [Image] = ISNULL((SELECT TOP 1 FIRMDOC.LDATA
                 FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC
                 WHERE FIRMDOC.INFOREF = ITEMS.LOGICALREF
                 AND FIRMDOC.INFOTYP = 20
                 AND FIRMDOC.DOCNR = 11), '')

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
    [OrderNumber] = ORFICHE.FICHENO,
    [SupplierReferenceId] = CLCARD.LOGICALREF,
    [SupplierCode] = CLCARD.CODE,
    [SupplierName] = CLCARD.DEFINITION_,
    [ShipInfoCode] = ISNULL(SHIP.CODE, ''),
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
    [DueDate] = ORFLINE.DUEDATE,
    [Image] = ISNULL((SELECT TOP 1 FIRMDOC.LDATA
                 FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC
                 WHERE FIRMDOC.INFOREF = ITEMS.LOGICALREF
                 AND FIRMDOC.INFOTYP = 20
                 AND FIRMDOC.DOCNR = 11), '')

FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFICHE AS ORFICHE ON ORFLINE.ORDFICHEREF = ORFICHE.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON ORFLINE.STOCKREF = ITEMS.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON ORFICHE.CLIENTREF = CLCARD.LOGICALREF
            LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT ON ORFLINE.VARIANTREF = VARIANT.LOGICALREF  
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON ORFLINE.UOMREF = SUBUNITSET.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON ORFLINE.USREF = UNITSET.LOGICALREF
            LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_SHIPINFO AS SHIP on ORFICHE.SHIPINFOREF = SHIP.LOGICALREF
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
    [OrderNumber] = ORFICHE.FICHENO,
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
    [DueDate] = ORFLINE.DUEDATE,
    [Image] = ISNULL((SELECT TOP 1 FIRMDOC.LDATA 
                 FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC
                 WHERE FIRMDOC.INFOREF = ITEMS.LOGICALREF 
                 AND FIRMDOC.INFOTYP = 20  
                 AND FIRMDOC.DOCNR = 11), '')

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

    private string WaitingPurchaseOrderQueryBySupplierAndWarehouseAndProduct(int firmNumber, int periodNumber, int warehouseNumber, int supplierReferenceId, int productReferenceId, string search = "", int skip = 0, int take = 20)
    {
		string baseQuery = $@"SELECT

    [ReferenceId] = ORFLINE.LOGICALREF,
    [OrderReferenceId] = ORFICHE.LOGICALREF,
    [OrderNumber] = ORFICHE.FICHENO,
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
    [DueDate] = ORFLINE.DUEDATE,
    [Image] = ISNULL((SELECT TOP 1 FIRMDOC.LDATA
                 FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC
                 WHERE FIRMDOC.INFOREF = ITEMS.LOGICALREF
                 AND FIRMDOC.INFOTYP = 20
                 AND FIRMDOC.DOCNR = 11), '')

FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFICHE AS ORFICHE ON ORFLINE.ORDFICHEREF = ORFICHE.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON ORFLINE.STOCKREF = ITEMS.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON ORFICHE.CLIENTREF = CLCARD.LOGICALREF
            LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT ON ORFLINE.VARIANTREF = VARIANT.LOGICALREF  
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON ORFLINE.UOMREF = SUBUNITSET.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON ORFLINE.USREF = UNITSET.LOGICALREF
			LEFT JOIN L_CAPIWHOUSE AS WHOUSE ON ORFLINE.SOURCEINDEX = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
			WHERE ORFLINE.CLOSED = 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 2
 AND ITEMS.UNITSETREF <> 0 AND CLCARD.LOGICALREF = {supplierReferenceId} AND ORFLINE.SOURCEINDEX = {warehouseNumber} AND ORFLINE.STOCKREF = {productReferenceId}";

		if (!string.IsNullOrEmpty(search))
			baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";

		baseQuery += $@" ORDER BY ITEMS.CODE ASC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

		return baseQuery;
	}

    private string GetSuppliersQuery(int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
    {
        string baseQuery = @$"SELECT * FROM (SELECT
[ReferenceId]=SUPPLIER.LOGICALREF,
[Code]=SUPPLIER.CODE,
[Title]=SUPPLIER.DEFINITION_,
[IsPersonal] =
        CASE
            WHEN SUPPLIER.ISPERSCOMP= 0 THEN 0
            ELSE 1
        END,
[Name]=SUPPLIER.DEFINITION_,
[Email]=SUPPLIER.EMAILADDR,
[Telephone]=SUPPLIER.TELNRS1+' '+ SUPPLIER.TELNRS2,
[Address]=SUPPLIER.ADDR1,
[City]=SUPPLIER.CITY,
[Country]=SUPPLIER.COUNTRY,
[PostalCode]=SUPPLIER.POSTCODE,
[TaxOffice]=SUPPLIER.TAXOFFICE,
[TaxNumber]=SUPPLIER.TAXNR,
[OrderReferenceCount] = ISNULL((SELECT COUNT(DISTINCT STOCKREF) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE WHERE CLIENTREF = SUPPLIER.LOGICALREF AND (AMOUNT-SHIPPEDAMOUNT) > 0 AND CLOSED = 0  AND LINETYPE = 0 AND TRCODE = 2 ),0),
[IsActive]=
       CASE
	      WHEN SUPPLIER.ACTIVE=0 THEN 0
		  ELSE 1
END
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS SUPPLIER
WHERE SUPPLIER.CODE LIKE '32%' AND SUPPLIER.CODE <> 'ÿ' AND SUPPLIER.ACTIVE = 0) DD
WHERE DD.OrderReferenceCount > 0
		";

        if (!string.IsNullOrEmpty(search))
        {
            baseQuery += $@" AND (DD.Code LIKE '%{search}%' OR DD.Name LIKE '%{search}%')";
        }

        baseQuery += $@" ORDER BY DD.OrderReferenceCount DESC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }

	private string WaitingPurchaseOrderQueryBySupplierAndShipInfoQuery(int firmNumber, int periodNumber, int supplierReferenceId, int shipInfoReferenceId, string search = "", int skip = 0, int take = 20)
	{
		string baseQuery = $@"SELECT

    [ReferenceId] = ORFLINE.LOGICALREF,
    [OrderReferenceId] = ISNULL(ORFICHE.LOGICALREF, 0),
    [OrderNumber] = ISNULL(ORFICHE.FICHENO, ''),
    [SupplierReferenceId] = ISNULL(CLCARD.LOGICALREF, 0),
    [SupplierCode] = ISNULL(CLCARD.CODE, ''),
    [SupplierName] = ISNULL(CLCARD.DEFINITION_, ''),
    [ShipInfoCode] = ISNULL(SHIP.CODE, ''),
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
    [DueDate] = ORFLINE.DUEDATE,
    [Image] = ISNULL((SELECT TOP 1 FIRMDOC.LDATA
                 FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC
                 WHERE FIRMDOC.INFOREF = ITEMS.LOGICALREF
                 AND FIRMDOC.INFOTYP = 20
                 AND FIRMDOC.DOCNR = 11), '')

FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFICHE AS ORFICHE ON ORFLINE.ORDFICHEREF = ORFICHE.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON ORFLINE.STOCKREF = ITEMS.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON ORFICHE.CLIENTREF = CLCARD.LOGICALREF
            LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT ON ORFLINE.VARIANTREF = VARIANT.LOGICALREF  
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON ORFLINE.UOMREF = SUBUNITSET.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON ORFLINE.USREF = UNITSET.LOGICALREF
            LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_SHIPINFO AS SHIP on ORFICHE.SHIPINFOREF = SHIP.LOGICALREF
			LEFT JOIN L_CAPIWHOUSE AS WHOUSE ON ORFLINE.SOURCEINDEX = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
			WHERE ORFLINE.CLOSED = 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 2
 AND ITEMS.UNITSETREF <> 0 AND CLCARD.LOGICALREF = {supplierReferenceId} AND ORFICHE.SHIPINFOREF = {shipInfoReferenceId} OR ORFICHE.SHIPINFOREF IS NULL";

		if (!string.IsNullOrEmpty(search))
			baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";

		baseQuery += $@" ORDER BY ITEMS.CODE ASC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

		return baseQuery;
	}


}