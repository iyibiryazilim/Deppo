using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DataStores;

public class BarcodeSearchPurchaseDataStore : IBarcodeSearchPurchaseService
{
	string postUrl = "/gateway/customQuery/CustomQuery";
	public async Task<DataResult<dynamic>> SearchByProductCode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId)
	{
		var content = new StringContent(JsonConvert.SerializeObject(SearchByProductCodeQuery(firmNumber, periodNumber, barcode, warehouseNumber, supplierReferenceId, shipInfoReferenceId)), Encoding.UTF8, "application/json");

		HttpResponseMessage responseMessage = await httpClient.PostAsync(postUrl, content);
		DataResult<dynamic> dataResult = new DataResult<dynamic>();
		if (responseMessage.IsSuccessStatusCode)
		{
			var data = await responseMessage.Content.ReadAsStringAsync();
			if (data != null)
			{
				if (!string.IsNullOrEmpty(data))
				{
					var result = JsonConvert.DeserializeObject<DataResult<dynamic>>(data);

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

	public async Task<DataResult<dynamic>> SearchByProductLotNumber(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId)
	{
		var content = new StringContent(JsonConvert.SerializeObject(SearchByProductLotNumberQuery(firmNumber, periodNumber, barcode, warehouseNumber, supplierReferenceId, shipInfoReferenceId)), Encoding.UTF8, "application/json");

		HttpResponseMessage responseMessage = await httpClient.PostAsync(postUrl, content);
		DataResult<dynamic> dataResult = new DataResult<dynamic>();
		if (responseMessage.IsSuccessStatusCode)
		{
			var data = await responseMessage.Content.ReadAsStringAsync();
			if (data != null)
			{
				if (!string.IsNullOrEmpty(data))
				{
					var result = JsonConvert.DeserializeObject<DataResult<dynamic>>(data);

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

	public async Task<DataResult<dynamic>> SearchByProductMainBarcode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId)
	{
		var content = new StringContent(JsonConvert.SerializeObject(SearchByProductCodeQuery(firmNumber, periodNumber, barcode, warehouseNumber, supplierReferenceId, shipInfoReferenceId)), Encoding.UTF8, "application/json");

		HttpResponseMessage responseMessage = await httpClient.PostAsync(postUrl, content);
		DataResult<dynamic> dataResult = new DataResult<dynamic>();
		if (responseMessage.IsSuccessStatusCode)
		{
			var data = await responseMessage.Content.ReadAsStringAsync();
			if (data != null)
			{
				if (!string.IsNullOrEmpty(data))
				{
					var result = JsonConvert.DeserializeObject<DataResult<dynamic>>(data);

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

	public async Task<DataResult<dynamic>> SearchByProductSeriNumber(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId)
	{
		var content = new StringContent(JsonConvert.SerializeObject(SearchByProductSeriNumberQuery(firmNumber, periodNumber, barcode, warehouseNumber, supplierReferenceId, shipInfoReferenceId)), Encoding.UTF8, "application/json");

		HttpResponseMessage responseMessage = await httpClient.PostAsync(postUrl, content);
		DataResult<dynamic> dataResult = new DataResult<dynamic>();
		if (responseMessage.IsSuccessStatusCode)
		{
			var data = await responseMessage.Content.ReadAsStringAsync();
			if (data != null)
			{
				if (!string.IsNullOrEmpty(data))
				{
					var result = JsonConvert.DeserializeObject<DataResult<dynamic>>(data);

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

	public async Task<DataResult<dynamic>> SearchByProductSubBarcode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId)
	{
		var content = new StringContent(JsonConvert.SerializeObject(SearchByProductSubBarcodeQuery(firmNumber, periodNumber, barcode, warehouseNumber, supplierReferenceId, shipInfoReferenceId)), Encoding.UTF8, "application/json");

		HttpResponseMessage responseMessage = await httpClient.PostAsync(postUrl, content);
		DataResult<dynamic> dataResult = new DataResult<dynamic>();
		if (responseMessage.IsSuccessStatusCode)
		{
			var data = await responseMessage.Content.ReadAsStringAsync();
			if (data != null)
			{
				if (!string.IsNullOrEmpty(data))
				{
					var result = JsonConvert.DeserializeObject<DataResult<dynamic>>(data);

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

	public Task<DataResult<dynamic>> SearchByVariantCode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId)
	{
		throw new NotImplementedException();
	}

	public Task<DataResult<dynamic>> SearchByVariantLotNumber(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId)
	{
		throw new NotImplementedException();
	}

	public Task<DataResult<dynamic>> SearchByVariantMainBarcode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId)
	{
		throw new NotImplementedException();
	}

	public Task<DataResult<dynamic>> SearchByVariantSeriNumber(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId)
	{
		throw new NotImplementedException();
	}

	public Task<DataResult<dynamic>> SearchByVariantSubBarcode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId)
	{
		throw new NotImplementedException();
	}

	public async Task<DataResult<dynamic>> SearchBySupplierProductCode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId)
	{
		var content = new StringContent(JsonConvert.SerializeObject(SearchBySupplierProductCodeQuery(firmNumber, periodNumber, barcode, warehouseNumber, supplierReferenceId, shipInfoReferenceId)), Encoding.UTF8, "application/json");

		HttpResponseMessage responseMessage = await httpClient.PostAsync(postUrl, content);
		DataResult<dynamic> dataResult = new DataResult<dynamic>();
		if (responseMessage.IsSuccessStatusCode)
		{
			var data = await responseMessage.Content.ReadAsStringAsync();
			if (data != null)
			{
				if (!string.IsNullOrEmpty(data))
				{
					var result = JsonConvert.DeserializeObject<DataResult<dynamic>>(data);

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

	public Task<DataResult<dynamic>> SearchBySupplierVariantCode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId = 0)
	{
		throw new NotImplementedException();
	}

	private string SearchByProductCodeQuery(int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId = 0)
	{
		string baseQuery = $@"WITH BaseQuery AS (
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
		WHERE CLCARD.LOGICALREF = {supplierReferenceId} AND 
		        ITEMS.CODE = '{barcode}' AND
				ORFLINE.CLOSED = 0 AND
				ORFLINE.USREF <> 0 AND 
				(ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND 
				ORFLINE.TRCODE = 2 AND 
				ORFLINE.SOURCEINDEX = {warehouseNumber} AND 
				ORFICHE.SHIPINFOREF = {shipInfoReferenceId} OR ORFICHE.SHIPINFOREF IS NULL
		 GROUP BY ITEMS.LOGICALREF, ITEMS.CODE, ITEMS.NAME, VARIANT.LOGICALREF, ORFLINE.VARIANTREF, VARIANT.CODE,
		VARIANT.NAME, UNITSET.LOGICALREF, UNITSET.CODE,UNITSET.NAME, SUBUNITSET.LOGICALREF, SUBUNITSET.CODE, SUBUNITSET.NAME, ITEMS.CANCONFIGURE, ITEMS.LOCTRACKING, ITEMS.TRACKTYPE
)
SELECT TOP 1
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
    LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC ON FIRMDOC.INFOREF = BQ.ItemReferenceId AND FIRMDOC.INFOTYP = 20";


		return baseQuery;
	}

	private string SearchByProductMainBarcodeQuery(int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId)
	{
		string baseQuery = $@"
WITH BaseQuery AS (
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
		FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITBARCODE AS BARCODE WITH(NOLOCK)
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON BARCODE.ITEMREF = ITEMS.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE WITH(NOLOCK) ON ORFLINE.STOCKREF = ITEMS.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFICHE AS ORFICHE WITH(NOLOCK) ON ORFLINE.ORDFICHEREF = ORFICHE.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD WITH(NOLOCK) ON ORFICHE.CLIENTREF = CLCARD.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET WITH(NOLOCK) ON ORFLINE.UOMREF = SUBUNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET WITH(NOLOCK) ON ORFLINE.USREF = UNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT WITH(NOLOCK) ON ORFLINE.VARIANTREF = VARIANT.LOGICALREF
		WHERE CLCARD.LOGICALREF = {supplierReferenceId} AND 
		        BARCODE.BARCODE = '{barcode}' AND
				ORFLINE.CLOSED = 0 AND
				ORFLINE.USREF <> 0 AND 
				(ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND 
				ORFLINE.TRCODE = 2 AND 
				ORFLINE.SOURCEINDEX = {warehouseNumber} AND 
				ORFICHE.SHIPINFOREF = {shipInfoReferenceId} OR ORFICHE.SHIPINFOREF IS NULL
		 GROUP BY ITEMS.LOGICALREF, ITEMS.CODE, ITEMS.NAME, VARIANT.LOGICALREF, ORFLINE.VARIANTREF, VARIANT.CODE,
		VARIANT.NAME, UNITSET.LOGICALREF, UNITSET.CODE,UNITSET.NAME, SUBUNITSET.LOGICALREF, SUBUNITSET.CODE, SUBUNITSET.NAME, ITEMS.CANCONFIGURE, ITEMS.LOCTRACKING, ITEMS.TRACKTYPE
)
SELECT TOP 1
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
		";

		return baseQuery;
	}

	private string SearchByProductSubBarcodeQuery(int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId)
	{
		string baseQuery = @$"

WITH BaseQuery AS (
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
		FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITBARCODE AS BARCODE WITH(NOLOCK)
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON BARCODE.ITEMREF = ITEMS.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE WITH(NOLOCK) ON ORFLINE.STOCKREF = ITEMS.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFICHE AS ORFICHE WITH(NOLOCK) ON ORFLINE.ORDFICHEREF = ORFICHE.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD WITH(NOLOCK) ON ORFICHE.CLIENTREF = CLCARD.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET WITH(NOLOCK) ON BARCODE.UNITLINEREF = SUBUNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET WITH(NOLOCK) ON UNITSET.LOGICALREF = SUBUNITSET.UNITSETREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT WITH(NOLOCK) ON ORFLINE.VARIANTREF = VARIANT.LOGICALREF
		WHERE CLCARD.LOGICALREF = {supplierReferenceId} AND 
		        BARCODE.BARCODE = '{barcode}' AND
				SUBUNITSET.MAINUNIT = 0 AND
				ORFLINE.CLOSED = 0 AND
				ORFLINE.USREF <> 0 AND 
				(ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND 
				ORFLINE.TRCODE = 2 AND 
				ORFLINE.SOURCEINDEX = {warehouseNumber} AND 
				ORFICHE.SHIPINFOREF = {shipInfoReferenceId} OR ORFUCHE.SHIPINFOREF IS NULL
		 GROUP BY ITEMS.LOGICALREF, ITEMS.CODE, ITEMS.NAME, VARIANT.LOGICALREF, ORFLINE.VARIANTREF, VARIANT.CODE,
		VARIANT.NAME, UNITSET.LOGICALREF, UNITSET.CODE,UNITSET.NAME, SUBUNITSET.LOGICALREF, SUBUNITSET.CODE, SUBUNITSET.NAME, ITEMS.CANCONFIGURE, ITEMS.LOCTRACKING, ITEMS.TRACKTYPE
)
SELECT TOP 1
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
    LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC ON FIRMDOC.INFOREF = BQ.ItemReferenceId AND FIRMDOC.INFOTYP = 20";

		return baseQuery;
	}

	private string SearchByProductSeriNumberQuery(int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId)
	{
		string baseQuery = $@"
			WITH BaseQuery AS (
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
		FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_SERILOTN AS SERI WITH(NOLOCK)
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON ITEMS.LOGICALREF = SERI.ITEMREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE WITH(NOLOCK) ON ORFLINE.STOCKREF = ITEMS.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFICHE AS ORFICHE WITH(NOLOCK) ON ORFLINE.ORDFICHEREF = ORFICHE.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD WITH(NOLOCK) ON ORFICHE.CLIENTREF = CLCARD.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET WITH(NOLOCK) ON ORFLINE.UOMREF = SUBUNITSET.LOGICALREF AND SUBUNITSET.MAINUNIT = 1
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET WITH(NOLOCK) ON ORFLINE.USREF = UNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT WITH(NOLOCK) ON ORFLINE.VARIANTREF = VARIANT.LOGICALREF
		WHERE CLCARD.LOGICALREF = {shipInfoReferenceId} AND 
		        SERI.SLTYPE = 1 AND
				SERI.CODE = '{barcode}' AND
				ORFLINE.CLOSED = 0 AND
				ORFLINE.USREF <> 0 AND 
				(ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND 
				ORFLINE.TRCODE = 2 AND 
				ORFLINE.SOURCEINDEX = {warehouseNumber} AND 
				ORFICHE.SHIPINFOREF = {shipInfoReferenceId} OR ORFICHE.SHIPINFOREF IS NULL
		 GROUP BY ITEMS.LOGICALREF, ITEMS.CODE, ITEMS.NAME, VARIANT.LOGICALREF, ORFLINE.VARIANTREF, VARIANT.CODE,
		VARIANT.NAME, UNITSET.LOGICALREF, UNITSET.CODE,UNITSET.NAME, SUBUNITSET.LOGICALREF, SUBUNITSET.CODE, SUBUNITSET.NAME, ITEMS.CANCONFIGURE, ITEMS.LOCTRACKING, ITEMS.TRACKTYPE
)
SELECT TOP 1
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
		";

		return baseQuery;
	}

	private string SearchByProductLotNumberQuery(int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId)
	{
		string baseQuery = @$"
			WITH BaseQuery AS (
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
		FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_SERILOTN AS LOT WITH(NOLOCK)
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON ITEMS.LOGICALREF = LOT.ITEMREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE WITH(NOLOCK) ON ORFLINE.STOCKREF = ITEMS.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFICHE AS ORFICHE WITH(NOLOCK) ON ORFLINE.ORDFICHEREF = ORFICHE.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD WITH(NOLOCK) ON ORFICHE.CLIENTREF = CLCARD.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET WITH(NOLOCK) ON ORFLINE.USREF = UNITSET.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET WITH(NOLOCK) ON ORFLINE.UOMREF = SUBUNITSET.LOGICALREF AND SUBUNITSET.MAINUNIT = 1
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT WITH(NOLOCK) ON ORFLINE.VARIANTREF = VARIANT.LOGICALREF
		WHERE CLCARD.LOGICALREF = {supplierReferenceId} AND 
		        LOT.SLTYPE = 0 AND
				LOT.CODE = '{barcode}' AND
				ORFLINE.CLOSED = 0 AND
				ORFLINE.USREF <> 0 AND 
				(ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND 
				ORFLINE.TRCODE = 2 AND 
				ORFLINE.SOURCEINDEX = {warehouseNumber} AND 
				ORFICHE.SHIPINFOREF = {shipInfoReferenceId} OR ORFICHE.SHIPINFOREF IS NULL
		 GROUP BY ITEMS.LOGICALREF, ITEMS.CODE, ITEMS.NAME, VARIANT.LOGICALREF, ORFLINE.VARIANTREF, VARIANT.CODE,
		VARIANT.NAME, UNITSET.LOGICALREF, UNITSET.CODE,UNITSET.NAME, SUBUNITSET.LOGICALREF, SUBUNITSET.CODE, SUBUNITSET.NAME, ITEMS.CANCONFIGURE, ITEMS.LOCTRACKING, ITEMS.TRACKTYPE
)
SELECT TOP 1
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
		";

		return baseQuery;
	}

	private string SearchBySupplierProductCodeQuery(int firmNumber, int periodNumber, string barcode, int warehouseNumber, int supplierReferenceId, int shipInfoReferenceId)
	{
		string baseQuery = @$"WITH BaseQuery AS (
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
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_02_ORFICHE AS ORFICHE WITH(NOLOCK) ON ORFLINE.ORDFICHEREF = ORFICHE.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD WITH(NOLOCK) ON ORFICHE.CLIENTREF = CLCARD.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON ORFLINE.STOCKREF = ITEMS.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_SUPPASGN AS SUPPASGN WITH(NOLOCK) ON ITEMS.LOGICALREF = SUPPASGN.ITEMREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET WITH(NOLOCK) ON ORFLINE.UOMREF = SUBUNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET WITH(NOLOCK) ON ORFLINE.USREF = UNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT WITH(NOLOCK) ON ORFLINE.VARIANTREF = VARIANT.LOGICALREF
		WHERE CLCARD.LOGICALREF = {supplierReferenceId} AND 
				ORFLINE.CLOSED = 0 AND
				ORFLINE.USREF <> 0 AND 
				SUPPASGN.SUPPLYTYPE = 1 AND
				SUPPASGN.ICUSTSUPBARCODE = '' or SUPPASGN.ICUSTSUPCODE = ''AND
				(ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND 
				ORFLINE.TRCODE = 2 AND 
				ORFLINE.SOURCEINDEX = {warehouseNumber} AND 
				ORFICHE.SHIPINFOREF = {shipInfoReferenceId} PR ORFICHE.SHIPINFOREF IS NULL
		 GROUP BY ITEMS.LOGICALREF, ITEMS.CODE, ITEMS.NAME, VARIANT.LOGICALREF, ORFLINE.VARIANTREF, VARIANT.CODE,
		VARIANT.NAME, UNITSET.LOGICALREF, UNITSET.CODE,UNITSET.NAME, SUBUNITSET.LOGICALREF, SUBUNITSET.CODE, SUBUNITSET.NAME, ITEMS.CANCONFIGURE, ITEMS.LOCTRACKING, ITEMS.TRACKTYPE
)
SELECT TOP 1
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
    LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC ON FIRMDOC.INFOREF = BQ.ItemReferenceId AND FIRMDOC.INFOTYP = 20";

		return baseQuery;
	}
}
