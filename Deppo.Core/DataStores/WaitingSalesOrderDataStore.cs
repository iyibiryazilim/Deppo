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

	public async Task<DataResult<IEnumerable<dynamic>>> GetObjectsByCustomerAndShipInfo(HttpClient httpClient, int firmNumber, int periodNumber, int customerReferenceId, int shipInfoReferenceId = 0, string search = "", int skip = 0, int take = 0)
	{
		var content = new StringContent(JsonConvert.SerializeObject(WaitingSalesOrderQueryByCustomerAndShipInfo(firmNumber, periodNumber, customerReferenceId, shipInfoReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

	public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, string search = "", int skip = 0, int take = 20)
	{
		var content = new StringContent(JsonConvert.SerializeObject(WaitingSalesOrderQueryByWarehouse(firmNumber, periodNumber,warehouseNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

	public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int customerReferenceId, string search = "", int skip = 0, int take = 20)
	{
		var content = new StringContent(JsonConvert.SerializeObject(WaitingSalesOrderQueryByCustomerAndWarehouse(firmNumber, periodNumber, warehouseNumber, customerReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

	public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int customerReferenceId, int shipInfoReferenceId = 0, string search = "", int skip = 0, int take = 20)
	{
		var content = new StringContent(JsonConvert.SerializeObject(WaitingSalesOrderQueryByCustomerAndWarehouseAndAShipInfo(firmNumber, periodNumber, warehouseNumber, customerReferenceId, shipInfoReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

	public async Task<DataResult<IEnumerable<dynamic>>> GetObjectsByCustomer(HttpClient httpClient, int firmNumber, int periodNumber, int customerReferenceId, string search = "", int skip = 0, int take = 20)
	{
		var content = new StringContent(JsonConvert.SerializeObject(WaitingSalesOrderQueryByCustomer(firmNumber, periodNumber, customerReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

	public async Task<DataResult<IEnumerable<dynamic>>> GetObjectsByProduct(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int customerReferenceId, int productReferenceId, int shipInfoReferenceId = 0 ,string search = "", int skip = 0, int take = 20)
	{
		var content = new StringContent(JsonConvert.SerializeObject(WaitingSalesOrderQueryByCustomerAndWarehouseAndProduct(firmNumber, periodNumber, warehouseNumber, customerReferenceId, productReferenceId, shipInfoReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

	public async Task<DataResult<IEnumerable<dynamic>>> GetCustomers(HttpClient httpClient,int firmNumber,int periodNumber,string search="",int skip=0,int take = 0)
	{
        var content = new StringContent(JsonConvert.SerializeObject(GetCustomersQuery(firmNumber, periodNumber, search, skip, take)), Encoding.UTF8, "application/json");

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
			[OrderNumber] = ORFICHE.FICHENO,
            [CustomerReferenceId] = CLCARD.LOGICALREF,
            [CustomerCode] = CLCARD.CODE,
            [CustomerName] = CLCARD.DEFINITION_,
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
        LEFT JOIN L_CAPIWHOUSE AS WHOUSE ON ORFLINE.SOURCEINDEX = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
		WHERE ORFLINE.CLOSED = 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 1
		AND ITEMS.UNITSETREF <> 0
		";

		if (!string.IsNullOrEmpty(search))
		{
			baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%' OR CLCARD.CODE LIKE '{search}%' OR CLCARD.DEFINITION_ LIKE '%{search}%')";
		}

		baseQuery += $@" ORDER BY ITEMS.CODE ASC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

		return baseQuery;
	}

	private string WaitingSalesOrderQueryByWarehouse(int firmNumber, int periodNumber, int warehouseNumber, string search = "", int skip = 0, int take = 20)
	{
		string baseQuery = @$"SELECT
			[ReferenceId] = ORFLINE.LOGICALREF,
            [OrderNumber] = ORFICHE.FICHENO,
            [OrderReferenceId] = ORFICHE.LOGICALREF,
            [CustomerReferenceId] = CLCARD.LOGICALREF,
            [CustomerCode] = CLCARD.CODE,
            [CustomerName] = CLCARD.DEFINITION_,
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
        LEFT JOIN L_CAPIWHOUSE AS WHOUSE ON ORFLINE.SOURCEINDEX = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
		WHERE ORFLINE.CLOSED = 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 1 AND ORFLINE.SOURCEINDEX = {warehouseNumber}
		AND ITEMS.UNITSETREF <> 0
		";

		if (!string.IsNullOrEmpty(search))
		{
			baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";
		}

		baseQuery += $@" ORDER BY ITEMS.CODE ASC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

		return baseQuery;
	}

	private string WaitingSalesOrderQueryByCustomer(int firmNumber, int periodNumber, int customerReferenceId, string search = "", int skip = 0, int take = 20)
	{
		string baseQuery = @$"SELECT
			[ReferenceId] = ORFLINE.LOGICALREF,
            [OrderReferenceId] = ORFICHE.LOGICALREF,
			[OrderNumber] = ORFICHE.FICHENO,
            [CustomerReferenceId] = CLCARD.LOGICALREF,
            [CustomerCode] = CLCARD.CODE,
            [CustomerName] = CLCARD.DEFINITION_,
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
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_SHIPINFO AS SHIP ON ORFICHE.SHIPINFOREF = SHIP.LOGICALREF
        LEFT JOIN L_CAPIWHOUSE AS WHOUSE ON ORFLINE.SOURCEINDEX = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
		WHERE ORFLINE.CLOSED = 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 1
		AND ITEMS.UNITSETREF <> 0 AND CLCARD.LOGICALREF = {customerReferenceId}
		";

		if (!string.IsNullOrEmpty(search))
		{
			baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";
		}

		baseQuery += $@" ORDER BY ITEMS.CODE ASC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

		return baseQuery;
	}

	private string WaitingSalesOrderQueryByCustomerAndWarehouse(int firmNumber, int periodNumber, int warehouseNumber, int customerReferenceId, string search = "", int skip = 0, int take = 20)
	{
		string baseQuery = @$"SELECT
			[ReferenceId] = ORFLINE.LOGICALREF,
            [OrderReferenceId] = ORFICHE.LOGICALREF,
            [OrderNumber] = ORFICHE.FICHENO,
            [CustomerReferenceId] = CLCARD.LOGICALREF,
            [CustomerCode] = CLCARD.CODE,
            [CustomerName] = CLCARD.DEFINITION_,
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
        LEFT JOIN L_CAPIWHOUSE AS WHOUSE ON ORFLINE.SOURCEINDEX = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
		WHERE ORFLINE.CLOSED = 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 1
		AND ITEMS.UNITSETREF <> 0 AND CLCARD.LOGICALREF = {customerReferenceId} AND ORFLINE.SOURCEINDEX = {warehouseNumber}
		";

		if (!string.IsNullOrEmpty(search))
		{
			baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";
		}

		baseQuery += $@" ORDER BY ITEMS.CODE ASC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

		return baseQuery;
	}

	private string WaitingSalesOrderQueryByCustomerAndWarehouseAndProduct(int firmNumber, int periodNumber, int warehouseNumber, int customerReferenceId, int productReferenceId, int shipInfoReferenceId = 0, string search = "", int skip = 0, int take = 20)
	{
		string baseQuery = @$"SELECT
			[ReferenceId] = ORFLINE.LOGICALREF,
            [OrderReferenceId] = ORFICHE.LOGICALREF,
            [OrderNumber] = ORFICHE.FICHENO,
            [CustomerReferenceId] = CLCARD.LOGICALREF,
            [CustomerCode] = CLCARD.CODE,
            [CustomerName] = CLCARD.DEFINITION_,
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
            [Price] = ORFLINE.PRICE,
            [Vat] = ORFLINE.VAT,
            [OrderDate] = ORFLINE.DATE_,
            [DueDate] = ISNULL(ORFLINE.DUEDATE, ORFLINE.DATE_)
		FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFICHE AS ORFICHE ON ORFLINE.ORDFICHEREF = ORFICHE.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON ORFLINE.STOCKREF = ITEMS.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON ORFICHE.CLIENTREF = CLCARD.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT ON ORFLINE.VARIANTREF = VARIANT.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON ORFLINE.UOMREF = SUBUNITSET.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON ORFLINE.USREF = UNITSET.LOGICALREF 
        LEFT JOIN L_CAPIWHOUSE AS WHOUSE ON ORFLINE.SOURCEINDEX = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
		WHERE ORFLINE.CLOSED = 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 1
		AND ITEMS.UNITSETREF <> 0 AND CLCARD.LOGICALREF = {customerReferenceId} AND ORFLINE.SOURCEINDEX = {warehouseNumber} AND ORFLINE.STOCKREF = {productReferenceId} AND ORFICHE.SHIPINFOREF = {shipInfoReferenceId}
		";

		if (!string.IsNullOrEmpty(search))
		{
			baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";
		}

		baseQuery += $@" ORDER BY ITEMS.CODE ASC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

		return baseQuery;
	}

	private string WaitingSalesOrderQueryByCustomerAndWarehouseAndAShipInfo(int firmNumber, int periodNumber, int warehouseNumber, int customerReferenceId, int shipInfoReferenceId, string search = "", int skip = 0, int take = 20) {
		string baseQuery = @$"SELECT
			[ReferenceId] = ORFLINE.LOGICALREF,
            [OrderReferenceId] = ORFICHE.LOGICALREF,
			[OrderNumber] = ORFICHE.FICHENO,
            [CustomerReferenceId] = CLCARD.LOGICALREF,
            [CustomerCode] = CLCARD.CODE,
            [CustomerName] = CLCARD.DEFINITION_,
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
			[Price] = ORFLINE.PRICE,
            [Vat] = ORFLINE.VAT,
            [DueDate] = ISNULL(ORFLINE.DUEDATE, ORFLINE.DATE_),
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
		WHERE ORFLINE.CLOSED = 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 1
		AND ITEMS.UNITSETREF <> 0 AND CLCARD.LOGICALREF = {customerReferenceId} AND ORFLINE.SOURCEINDEX = {warehouseNumber} AND ORFICHE.SHIPINFOREF = {shipInfoReferenceId}
		";

		if (!string.IsNullOrEmpty(search))
		{
			baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";
		}

		baseQuery += $@" ORDER BY ITEMS.CODE ASC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

		return baseQuery;

	}

    private string GetCustomersQuery(int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
    {
        string baseQuery = @$"SELECT * FROM (SELECT
[ReferenceId]=CUSTOMER.LOGICALREF,
[Code]=CUSTOMER.CODE,
[Title]=CUSTOMER.DEFINITION_,
[IsPersonal] =
        CASE
            WHEN CUSTOMER.ISPERSCOMP= 0 THEN 0
            ELSE 1
        END,
[Name]=CUSTOMER.DEFINITION_,
[Email]=CUSTOMER.EMAILADDR,
[Telephone]=CUSTOMER.TELNRS1+' '+ CUSTOMER.TELNRS2,
[Address]=CUSTOMER.ADDR1,
[City]=CUSTOMER.CITY,
[Country]=CUSTOMER.COUNTRY,
[PostalCode]=CUSTOMER.POSTCODE,
[TaxOffice]=CUSTOMER.TAXOFFICE,
[TaxNumber]=CUSTOMER.TAXNR,
[OrderReferenceCount] = ISNULL((SELECT COUNT(DISTINCT STOCKREF) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE WHERE CLIENTREF = CUSTOMER.LOGICALREF AND (AMOUNT-SHIPPEDAMOUNT) > 0 AND CLOSED = 0  AND LINETYPE = 0 AND TRCODE = 1 ),0),
[IsActive]=
       CASE
	      WHEN CUSTOMER.ACTIVE=0 THEN 0
		  ELSE 1
END
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CUSTOMER
WHERE CUSTOMER.CODE LIKE '12%' AND CUSTOMER.CODE <> 'ÿ' AND CUSTOMER.ACTIVE = 0) DD
WHERE DD.OrderReferenceCount > 0
		";

        if (!string.IsNullOrEmpty(search))
        {
            baseQuery += $@" AND (DD.Code LIKE '%{search}%' OR DD.Name LIKE '%{search}%')";
        }

        baseQuery += $@" ORDER BY DD.OrderReferenceCount DESC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }

	private string WaitingSalesOrderQueryByCustomerAndShipInfo(int firmNumber, int periodNumber, int customerReferenceId, int shipInfoReferenceId = 0, string search = "", int skip = 0, int take = 20)
	{
		string baseQuery = @$"SELECT
			[ReferenceId] = ORFLINE.LOGICALREF,
            [OrderNumber] = ISNULL(ORFICHE.FICHENO, ''),
            [OrderReferenceId] = ISNULL(ORFICHE.LOGICALREF, 0),
            [CustomerReferenceId] = ISNULL(CLCARD.LOGICALREF, 0),
            [CustomerCode] = ISNULL(CLCARD.CODE, ''),
            [CustomerName] = ISNULL(CLCARD.DEFINITION_, ''),
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
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_SHIPINFO AS SHIP ON ORFICHE.SHIPINFOREF = SHIP.LOGICALREF
        LEFT JOIN L_CAPIWHOUSE AS WHOUSE ON ORFLINE.SOURCEINDEX = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
		WHERE ORFLINE.CLOSED = 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 1
		AND ITEMS.UNITSETREF <> 0 AND CLCARD.LOGICALREF = {customerReferenceId} AND ORFICHE.SHIPINFOREF = {shipInfoReferenceId} OR ORFICHE.SHIPINFOREF IS NULL
		";

		if (!string.IsNullOrEmpty(search))
		{
			baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";
		}

		baseQuery += $@" ORDER BY ITEMS.CODE ASC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

		return baseQuery;
	}

	
}
