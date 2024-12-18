using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DataStores;

public class BarcodeSearchOutDataStore : IBarcodeSearchOutService
{
	string postUrl = "/gateway/customQuery/CustomQuery";

	public async Task<DataResult<dynamic>> SearchByProductCode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, string externalDb = "")
	{
		var content = new StringContent(JsonConvert.SerializeObject(SearchByProductCodeQuery(firmNumber, periodNumber, barcode, warehouseNumber, externalDb)), Encoding.UTF8, "application/json");

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

	public async Task<DataResult<dynamic>> SearchByProductLotNumber(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, string externalDb = "")
	{
		var content = new StringContent(JsonConvert.SerializeObject(SearchByProductLotNumberQuery(firmNumber, periodNumber, barcode, warehouseNumber, externalDb)), Encoding.UTF8, "application/json");

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

	public async Task<DataResult<dynamic>> SearchByProductMainBarcode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, string externalDb = "")
	{
		var content = new StringContent(JsonConvert.SerializeObject(SearchByProductMainBarcodeQuery(firmNumber, periodNumber, barcode, warehouseNumber, externalDb)), Encoding.UTF8, "application/json");

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

	public async Task<DataResult<dynamic>> SearchByProductSeriNumber(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, string externalDb = "")
	{
		var content = new StringContent(JsonConvert.SerializeObject(SearchByProductSeriNumberQuery(firmNumber, periodNumber, barcode, warehouseNumber, externalDb)), Encoding.UTF8, "application/json");

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

	public async Task<DataResult<dynamic>> SearchByProductSubBarcode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber, string externalDb = "")
	{
		var content = new StringContent(JsonConvert.SerializeObject(SearchByProductSubBarcodeQuery(firmNumber, periodNumber, barcode, warehouseNumber, externalDb)), Encoding.UTF8, "application/json");

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

	public Task<DataResult<dynamic>> SearchByVariantCode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber)
	{
		throw new NotImplementedException();
	}

	public Task<DataResult<dynamic>> SearchByVariantLotNumber(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber)
	{
		throw new NotImplementedException();
	}

	public Task<DataResult<dynamic>> SearchByVariantMainBarcode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber)
	{
		throw new NotImplementedException();
	}

	public Task<DataResult<dynamic>> SearchByVariantSeriNumber(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber)
	{
		throw new NotImplementedException();
	}

	public Task<DataResult<dynamic>> SearchByVariantSubBarcode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode, int warehouseNumber)
	{
		throw new NotImplementedException();
	}

	private string SearchByProductCodeQuery(int firmNumber, int periodNumber, string barcode, int warehouseNumber, string externalDb = "")
	{
		string baseQuery = $@"
			WITH BaseQuery AS (
SELECT
[ReferenceId] = NEWID(),
[ProductReferenceId] =  ITEMS.LOGICALREF,
[ProductCode] = ITEMS.CODE,
[ProductName] = ITEMS.NAME,
[WarehouseReferenceId] = WHOUSE.LOGICALREF,
[WarehouseNumber] = WHOUSE.NR,
[WarehouseName] = WHOUSE.NAME,
[UnitsetReferenceId] = UNITSETF.LOGICALREF,
[UnitsetCode] = UNITSETF.CODE,
[UnitsetName] = UNITSETF.NAME,
[SubUnitsetReferenceId] = UNITSETL.LOGICALREF,
[SubUnitsetCode] = UNITSETL.CODE,
[SubUnitsetName] = UNITSETL.NAME,
[IsVariant] = ITEMS.CANCONFIGURE,
[TrackingType] = ITEMS.TRACKTYPE,
[LocTracking] = ITEMS.LOCTRACKING,
[BrandReferenceId] = ISNULL(BRAND.LOGICALREF,0),
[BrandCode] = ISNULL(BRAND.CODE,''),
[BrandName] = ISNULL(BRAND.DESCR,''),
[StockQuantity] = ISNULL(SUM(STINVTOT.ONHAND),0)
FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON STINVTOT.STOCKREF = ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON ITEMS.UNITSETREF = UNITSETF.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON UNITSETL.UNITSETREF = UNITSETF.LOGICALREF AND UNITSETL.MAINUNIT = 1
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND WITH(NOLOCK) ON ITEMS.MARKREF = BRAND.LOGICALREF
LEFT JOIN {externalDb}L_CAPIWHOUSE AS WHOUSE WITH(NOLOCK) ON STINVTOT.INVENNO = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_INVDEF AS INVDEF WITH(NOLOCK) ON STINVTOT.INVENNO = INVDEF.INVENNO AND STINVTOT.STOCKREF = INVDEF.ITEMREF AND INVDEF.VARIANTREF = 0
WHERE INVDEF.OUTCTRL <> 2 AND  
	  STINVTOT.INVENNO = {warehouseNumber} AND 
      ITEMS.CODE = '{barcode}' AND 
      ITEMS.CARDTYPE <> 4 AND 
      ITEMS.MOLD = 0
 GROUP BY STINVTOT.INVENNO, ITEMS.LOGICALREF,ITEMS.CODE,ITEMS.NAME,WHOUSE.LOGICALREF,WHOUSE.NR,WHOUSE.NAME,UNITSETF.LOGICALREF,UNITSETF.CODE,UNITSETF.NAME,UNITSETL.LOGICALREF,UNITSETL.CODE,UNITSETL.NAME, ITEMS.CANCONFIGURE, ITEMS.TRACKTYPE, ITEMS.LOCTRACKING, BRAND.LOGICALREF, BRAND.CODE, BRAND.DESCR
 HAVING
    ISNULL(SUM(STINVTOT.ONHAND), 0) <> 0
)
SELECT TOP 1
    BQ.ReferenceId,
    BQ.ProductReferenceId,
    BQ.ProductCode,
    BQ.ProductName,
	BQ.WarehouseReferenceId,
    BQ.WarehouseNumber,
    BQ.WarehouseName,
    BQ.UnitsetReferenceId,
    BQ.UnitsetCode,
	BQ.UnitsetName,
    BQ.SubUnitsetReferenceId,
    BQ.SubUnitsetCode,
    BQ.SubUnitsetName,
	BQ.IsVariant,
	BQ.TrackingType,
	BQ.LocTracking,
	BQ.BrandReferenceId,
	BQ.BrandCode,
	BQ.BrandName,
    BQ.StockQuantity,
    FIRMDOC.LDATA AS Image
FROM
    BaseQuery AS BQ
    LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC ON FIRMDOC.INFOREF = BQ.ProductReferenceId AND FIRMDOC.INFOTYP = 20
		ORDER BY BQ.ProductCode DESC";

		return baseQuery;
	}

	private string SearchByProductMainBarcodeQuery(int firmNumber, int periodNumber, string barcode, int warehouseNumber, string externalDb = "")
	{
		string baseQuery = $@"
			WITH BaseQuery AS (
SELECT
[ReferenceId] = NEWID(),
[ProductReferenceId] =  ITEMS.LOGICALREF,
[ProductCode] = ITEMS.CODE,
[ProductName] = ITEMS.NAME,
[WarehouseReferenceId] = WHOUSE.LOGICALREF,
[WarehouseNumber] = WHOUSE.NR,
[WarehouseName] = WHOUSE.NAME,
[UnitsetReferenceId] = UNITSETF.LOGICALREF,
[UnitsetCode] = UNITSETF.CODE,
[UnitsetName] = UNITSETF.NAME,
[SubUnitsetReferenceId] = UNITSETL.LOGICALREF,
[SubUnitsetCode] = UNITSETL.CODE,
[SubUnitsetName] = UNITSETL.NAME,
[IsVariant] = ITEMS.CANCONFIGURE,
[TrackingType] = ITEMS.TRACKTYPE,
[LocTracking] = ITEMS.LOCTRACKING,
[BrandReferenceId] = ISNULL(BRAND.LOGICALREF,0),
[BrandCode] = ISNULL(BRAND.CODE,''),
[BrandName] = ISNULL(BRAND.DESCR,''),
[StockQuantity] = ISNULL(SUM(STINVTOT.ONHAND),0)
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITBARCODE AS BARCODE WITH(NOLOCK)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON BARCODE.ITEMREF = ITEMS.LOGICALREF
LEFT JOIN LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK) ON ITEMS.LOGICALREF = STINVTOT.STOCKREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON ITEMS.UNITSETREF = UNITSETF.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON UNITSETL.UNITSETREF = UNITSETF.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND WITH(NOLOCK) ON ITEMS.MARKREF = BRAND.LOGICALREF
LEFT JOIN {externalDb}L_CAPIWHOUSE AS WHOUSE WITH(NOLOCK) ON STINVTOT.INVENNO = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_INVDEF AS INVDEF WITH(NOLOCK) ON STINVTOT.INVENNO = INVDEF.INVENNO AND STINVTOT.STOCKREF = INVDEF.ITEMREF AND INVDEF.VARIANTREF = 0
WHERE INVDEF.OUTCTRL <> 2 AND  
      UNITSETL.MAINUNIT = 1 AND
	  STINVTOT.INVENNO = {warehouseNumber} AND 
      BARCODE.BARCODE = '{barcode}' AND 
      ITEMS.CARDTYPE <> 4 AND 
      ITEMS.MOLD = 0
 GROUP BY STINVTOT.INVENNO, ITEMS.LOGICALREF,ITEMS.CODE,ITEMS.NAME,WHOUSE.LOGICALREF,WHOUSE.NR,WHOUSE.NAME,UNITSETF.LOGICALREF,UNITSETF.CODE,UNITSETF.NAME,UNITSETL.LOGICALREF,UNITSETL.CODE,UNITSETL.NAME, ITEMS.CANCONFIGURE, ITEMS.TRACKTYPE, ITEMS.LOCTRACKING, BRAND.LOGICALREF, BRAND.CODE, BRAND.DESCR
 HAVING
    ISNULL(SUM(STINVTOT.ONHAND), 0) <> 0
)
SELECT TOP 1
    BQ.ReferenceId,
    BQ.ProductReferenceId,
    BQ.ProductCode,
    BQ.ProductName,
	BQ.WarehouseReferenceId,
    BQ.WarehouseNumber,
    BQ.WarehouseName,
    BQ.UnitsetReferenceId,
    BQ.UnitsetCode,
	BQ.UnitsetName,
    BQ.SubUnitsetReferenceId,
    BQ.SubUnitsetCode,
    BQ.SubUnitsetName,
	BQ.IsVariant,
	BQ.TrackingType,
	BQ.LocTracking,
	BQ.BrandReferenceId,
	BQ.BrandCode,
	BQ.BrandName,
    BQ.StockQuantity,
    FIRMDOC.LDATA AS Image
FROM
    BaseQuery AS BQ
    LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC ON FIRMDOC.INFOREF = BQ.ProductReferenceId AND FIRMDOC.INFOTYP = 20
		ORDER BY BQ.ProductCode DESC";

		return baseQuery;
	}

	private string SearchByProductSubBarcodeQuery(int firmNumber, int periodNumber, string barcode, int warehouseNumber, string externalDb = "")
	{
		string baseQuery = $@"
			WITH BaseQuery AS (
SELECT
[ReferenceId] = NEWID(),
[ProductReferenceId] =  ITEMS.LOGICALREF,
[ProductCode] = ITEMS.CODE,
[ProductName] = ITEMS.NAME,
[WarehouseReferenceId] = WHOUSE.LOGICALREF,
[WarehouseNumber] = WHOUSE.NR,
[WarehouseName] = WHOUSE.NAME,
[UnitsetReferenceId] = UNITSETF.LOGICALREF,
[UnitsetCode] = UNITSETF.CODE,
[UnitsetName] = UNITSETF.NAME,
[SubUnitsetReferenceId] = UNITSETL.LOGICALREF,
[SubUnitsetCode] = UNITSETL.CODE,
[SubUnitsetName] = UNITSETL.NAME,
[IsVariant] = ITEMS.CANCONFIGURE,
[TrackingType] = ITEMS.TRACKTYPE,
[LocTracking] = ITEMS.LOCTRACKING,
[BrandReferenceId] = ISNULL(BRAND.LOGICALREF,0),
[BrandCode] = ISNULL(BRAND.CODE,''),
[BrandName] = ISNULL(BRAND.DESCR,''),
[StockQuantity] = ISNULL(SUM(STINVTOT.ONHAND),0)
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITBARCODE AS BARCODE WITH(NOLOCK)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON BARCODE.ITEMREF = ITEMS.LOGICALREF
LEFT JOIN LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK) ON ITEMS.LOGICALREF = STINVTOT.STOCKREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON UNITSETL.LOGICALREF = BARCODE.UNITLINEREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON UNITSETL.UNITSETREF = UNITSETF.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND WITH(NOLOCK) ON ITEMS.MARKREF = BRAND.LOGICALREF
LEFT JOIN {externalDb}L_CAPIWHOUSE AS WHOUSE WITH(NOLOCK) ON STINVTOT.INVENNO = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_INVDEF AS INVDEF WITH(NOLOCK) ON STINVTOT.INVENNO = INVDEF.INVENNO AND STINVTOT.STOCKREF = INVDEF.ITEMREF AND INVDEF.VARIANTREF = 0
WHERE INVDEF.OUTCTRL <> 2 AND  
      UNITSETL.MAINUNIT = 0 AND
	  STINVTOT.INVENNO = {warehouseNumber} AND 
      BARCODE.BARCODE = '{barcode}' AND 
      ITEMS.CARDTYPE <> 4 AND 
      ITEMS.MOLD = 0
 GROUP BY STINVTOT.INVENNO, ITEMS.LOGICALREF,ITEMS.CODE,ITEMS.NAME,WHOUSE.LOGICALREF,WHOUSE.NR,WHOUSE.NAME,UNITSETF.LOGICALREF,UNITSETF.CODE,UNITSETF.NAME,UNITSETL.LOGICALREF,UNITSETL.CODE,UNITSETL.NAME, ITEMS.CANCONFIGURE, ITEMS.TRACKTYPE, ITEMS.LOCTRACKING, BRAND.LOGICALREF, BRAND.CODE, BRAND.DESCR
 HAVING
    ISNULL(SUM(STINVTOT.ONHAND), 0) <> 0
)
SELECT TOP 1
    BQ.ReferenceId,
    BQ.ProductReferenceId,
    BQ.ProductCode,
    BQ.ProductName,
	BQ.WarehouseReferenceId,
    BQ.WarehouseNumber,
    BQ.WarehouseName,
    BQ.UnitsetReferenceId,
    BQ.UnitsetCode,
	BQ.UnitsetName,
    BQ.SubUnitsetReferenceId,
    BQ.SubUnitsetCode,
    BQ.SubUnitsetName,
	BQ.IsVariant,
	BQ.TrackingType,
	BQ.LocTracking,
	BQ.BrandReferenceId,
	BQ.BrandCode,
	BQ.BrandName,
    BQ.StockQuantity,
    FIRMDOC.LDATA AS Image
FROM
    BaseQuery AS BQ
    LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC ON FIRMDOC.INFOREF = BQ.ProductReferenceId AND FIRMDOC.INFOTYP = 20
		ORDER BY BQ.ProductCode DESC";

		return baseQuery;
	}

    private string SearchByProductSeriNumberQuery(int firmNumber, int periodNumber, string barcode, int warehouseNumber, string externalDb = "")
	{
		string baseQuery = $@"
			WITH BaseQuery AS (
SELECT
[ReferenceId] = NEWID(),
[ProductReferenceId] =  ITEMS.LOGICALREF,
[ProductCode] = ITEMS.CODE,
[ProductName] = ITEMS.NAME,
[WarehouseReferenceId] = WHOUSE.LOGICALREF,
[WarehouseNumber] = WHOUSE.NR,
[WarehouseName] = WHOUSE.NAME,
[UnitsetReferenceId] = UNITSETF.LOGICALREF,
[UnitsetCode] = UNITSETF.CODE,
[UnitsetName] = UNITSETF.NAME,
[SubUnitsetReferenceId] = UNITSETL.LOGICALREF,
[SubUnitsetCode] = UNITSETL.CODE,
[SubUnitsetName] = UNITSETL.NAME,
[IsVariant] = ITEMS.CANCONFIGURE,
[TrackingType] = ITEMS.TRACKTYPE,
[LocTracking] = ITEMS.LOCTRACKING,
[BrandReferenceId] = ISNULL(BRAND.LOGICALREF,0),
[BrandCode] = ISNULL(BRAND.CODE,''),
[BrandName] = ISNULL(BRAND.DESCR,''),
[StockQuantity] = ISNULL(SUM(STINVTOT.ONHAND),0)
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_SERILOTN AS SERI WITH(NOLOCK)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON SERI.ITEMREF = ITEMS.LOGICALREF
LEFT JOIN LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK) ON ITEMS.LOGICALREF = STINVTOT.STOCKREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON ITEMS.UNITSETREF = UNITSETF.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON UNITSETL.UNITSETREF = UNITSETF.LOGICALREF AND UNITSETL.MAINUNIT = 1
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND WITH(NOLOCK) ON ITEMS.MARKREF = BRAND.LOGICALREF
LEFT JOIN {externalDb}L_CAPIWHOUSE AS WHOUSE WITH(NOLOCK) ON STINVTOT.INVENNO = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_INVDEF AS INVDEF WITH(NOLOCK) ON STINVTOT.INVENNO = INVDEF.INVENNO AND STINVTOT.STOCKREF = INVDEF.ITEMREF AND INVDEF.VARIANTREF = 0
WHERE INVDEF.OUTCTRL <> 2 AND  
	  STINVTOT.INVENNO = {warehouseNumber} AND 
      SERI.CODE = '{barcode}' AND
      SERI.SLTYPE = 1 AND
      ITEMS.CARDTYPE <> 4 AND 
      ITEMS.MOLD = 0
 GROUP BY STINVTOT.INVENNO, ITEMS.LOGICALREF,ITEMS.CODE,ITEMS.NAME,WHOUSE.LOGICALREF,WHOUSE.NR,WHOUSE.NAME,UNITSETF.LOGICALREF,UNITSETF.CODE,UNITSETF.NAME,UNITSETL.LOGICALREF,UNITSETL.CODE,UNITSETL.NAME, ITEMS.CANCONFIGURE, ITEMS.TRACKTYPE, ITEMS.LOCTRACKING, BRAND.LOGICALREF, BRAND.CODE, BRAND.DESCR
 HAVING
    ISNULL(SUM(STINVTOT.ONHAND), 0) <> 0
)
SELECT TOP 1
    BQ.ReferenceId,
    BQ.ProductReferenceId,
    BQ.ProductCode,
    BQ.ProductName,
	BQ.WarehouseReferenceId,
    BQ.WarehouseNumber,
    BQ.WarehouseName,
    BQ.UnitsetReferenceId,
    BQ.UnitsetCode,
	BQ.UnitsetName,
    BQ.SubUnitsetReferenceId,
    BQ.SubUnitsetCode,
    BQ.SubUnitsetName,
	BQ.IsVariant,
	BQ.TrackingType,
	BQ.LocTracking,
	BQ.BrandReferenceId,
	BQ.BrandCode,
	BQ.BrandName,
    BQ.StockQuantity,
    FIRMDOC.LDATA AS Image
FROM
    BaseQuery AS BQ
    LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC ON FIRMDOC.INFOREF = BQ.ProductReferenceId AND FIRMDOC.INFOTYP = 20
		ORDER BY BQ.ProductCode DESC";

		return baseQuery;
	}

	private string SearchByProductLotNumberQuery(int firmNumber, int periodNumber, string barcode, int warehouseNumber, string externalDb = "")
	{
		string baseQuery = $@"
			WITH BaseQuery AS (
SELECT
[ReferenceId] = NEWID(),
[ProductReferenceId] =  ITEMS.LOGICALREF,
[ProductCode] = ITEMS.CODE,
[ProductName] = ITEMS.NAME,
[WarehouseReferenceId] = WHOUSE.LOGICALREF,
[WarehouseNumber] = WHOUSE.NR,
[WarehouseName] = WHOUSE.NAME,
[UnitsetReferenceId] = UNITSETF.LOGICALREF,
[UnitsetCode] = UNITSETF.CODE,
[UnitsetName] = UNITSETF.NAME,
[SubUnitsetReferenceId] = UNITSETL.LOGICALREF,
[SubUnitsetCode] = UNITSETL.CODE,
[SubUnitsetName] = UNITSETL.NAME,
[IsVariant] = ITEMS.CANCONFIGURE,
[TrackingType] = ITEMS.TRACKTYPE,
[LocTracking] = ITEMS.LOCTRACKING,
[BrandReferenceId] = ISNULL(BRAND.LOGICALREF,0),
[BrandCode] = ISNULL(BRAND.CODE,''),
[BrandName] = ISNULL(BRAND.DESCR,''),
[StockQuantity] = ISNULL(SUM(STINVTOT.ONHAND),0)
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_SERILOTN AS LOT WITH(NOLOCK)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON LOT.ITEMREF = ITEMS.LOGICALREF
LEFT JOIN LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK) ON ITEMS.LOGICALREF = STINVTOT.STOCKREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON ITEMS.UNITSETREF = UNITSETF.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON UNITSETL.UNITSETREF = UNITSETF.LOGICALREF AND UNITSETL.MAINUNIT = 1
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND WITH(NOLOCK) ON ITEMS.MARKREF = BRAND.LOGICALREF
LEFT JOIN {externalDb}L_CAPIWHOUSE AS WHOUSE WITH(NOLOCK) ON STINVTOT.INVENNO = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_INVDEF AS INVDEF WITH(NOLOCK) ON STINVTOT.INVENNO = INVDEF.INVENNO AND STINVTOT.STOCKREF = INVDEF.ITEMREF AND INVDEF.VARIANTREF = 0
WHERE INVDEF.OUTCTRL <> 2 AND  
	  STINVTOT.INVENNO = {warehouseNumber} AND 
      LOT.CODE = '{barcode}' AND
      LOT.SLTYPE = 0 AND
      ITEMS.CARDTYPE <> 4 AND 
      ITEMS.MOLD = 0
 GROUP BY STINVTOT.INVENNO, ITEMS.LOGICALREF,ITEMS.CODE,ITEMS.NAME,WHOUSE.LOGICALREF,WHOUSE.NR,WHOUSE.NAME,UNITSETF.LOGICALREF,UNITSETF.CODE,UNITSETF.NAME,UNITSETL.LOGICALREF,UNITSETL.CODE,UNITSETL.NAME, ITEMS.CANCONFIGURE, ITEMS.TRACKTYPE, ITEMS.LOCTRACKING, BRAND.LOGICALREF, BRAND.CODE, BRAND.DESCR
 HAVING
    ISNULL(SUM(STINVTOT.ONHAND), 0) <> 0
)
SELECT TOP 1
    BQ.ReferenceId,
    BQ.ProductReferenceId,
    BQ.ProductCode,
    BQ.ProductName,
	BQ.WarehouseReferenceId,
    BQ.WarehouseNumber,
    BQ.WarehouseName,
    BQ.UnitsetReferenceId,
    BQ.UnitsetCode,
	BQ.UnitsetName,
    BQ.SubUnitsetReferenceId,
    BQ.SubUnitsetCode,
    BQ.SubUnitsetName,
	BQ.IsVariant,
	BQ.TrackingType,
	BQ.LocTracking,
	BQ.BrandReferenceId,
	BQ.BrandCode,
	BQ.BrandName,
    BQ.StockQuantity,
    FIRMDOC.LDATA AS Image
FROM
    BaseQuery AS BQ
    LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC ON FIRMDOC.INFOREF = BQ.ProductReferenceId AND FIRMDOC.INFOTYP = 20
		ORDER BY BQ.ProductCode DESC";

		return baseQuery;
	}
}
