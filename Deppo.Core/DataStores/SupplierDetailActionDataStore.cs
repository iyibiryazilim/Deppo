using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DataStores;

public class SupplierDetailActionDataStore : ISupplierDetailActionService
{
    private string postUrl = "/gateway/customQuery/CustomQuery";

    public async Task<DataResult<IEnumerable<dynamic>>> GetShipAddressesBySupplier(HttpClient httpClient, int firmNumber, int periodNumber, int supplierReferenceId, string search = "", int skip = 0, int take = 20)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetShipAddressBySupplierReferenceIdQuery(firmNumber, periodNumber, supplierReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<IEnumerable<dynamic>>> GetWaitingPurchaseOrdersBySupplier(HttpClient httpClient, int firmNumber, int periodNumber, int supplierReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "")
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetWaitingPurchaseOrderBySupplierReferenceIdQuery(firmNumber, periodNumber, supplierReferenceId, search, skip, take, externalDb)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<IEnumerable<dynamic>>> GetApprovedProductsBySupplier(HttpClient httpClient, int firmNumber, int periodNumber, int supplierReferenceId, string search = "", int skip = 0, int take = 20)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetApprovedProductsQuery(firmNumber, periodNumber, supplierReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

    private string GetShipAddressBySupplierReferenceIdQuery(int firmNumber, int periodNumber, int supplierReferenceId, string search = "", int skip = 0, int take = 20)
    {
        string baseQuery = @$"SELECT
			[ReferenceId] = SHIP.LOGICALREF,
            [Code] = SHIP.CODE,
            [Name] = SHIP.NAME,
            [Address] = SHIP.ADDR1,
            [City] = SHIP.CITY,
            [Country] = SHIP.COUNTRY
       FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_SHIPINFO AS SHIP WHERE CLIENTREF = {supplierReferenceId}
		";

        if (!string.IsNullOrEmpty(search))
        {
            baseQuery += @$" AND SHIP.CODE LIKE '{search}%' OR SHIP.NAME LIKE '%{search}%'";
        }

        baseQuery += @$" ORDER BY SHIP.CODE ASC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }

    private string GetWaitingPurchaseOrderBySupplierReferenceIdQuery(int firmNumber, int periodNumber, int supplierReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "")
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
    [DueDate] = ORFLINE.DUEDATE

FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFICHE AS ORFICHE ON ORFLINE.ORDFICHEREF = ORFICHE.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON ORFLINE.STOCKREF = ITEMS.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON ORFICHE.CLIENTREF = CLCARD.LOGICALREF
            LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT ON ORFLINE.VARIANTREF = VARIANT.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON ORFLINE.UOMREF = SUBUNITSET.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON ORFLINE.USREF = UNITSET.LOGICALREF
			LEFT JOIN {externalDb}L_CAPIWHOUSE AS WHOUSE ON ORFLINE.SOURCEINDEX = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
			WHERE ORFLINE.CLOSED = 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 2
 AND ITEMS.UNITSETREF <> 0 AND CLCARD.LOGICALREF = {supplierReferenceId}";

        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";

        baseQuery += $@" ORDER BY ITEMS.CODE ASC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }

    private string GetApprovedProductsQuery(int firmNumber, int periodNumber, int supplierReferenceId, string search = "", int skip = 0, int take = 20)
    {
        string baseQuery = @$" SELECT
    [ReferenceId] = ITEMS.LOGICALREF,
    [Code] = ITEMS.CODE,
    [Name] = ITEMS.NAME,
	[VatRate] = ITEMS.VAT,
	[UnitsetReferenceId] = UNITSETF.LOGICALREF,
	[UnitsetCode] = UNITSETF.CODE,
	[UnitsetName] = UNITSETF.NAME,
	[SubUnitsetReferenceId] = UNITSETL.LOGICALREF,
	[SubUnitsetCode] = UNITSETL.CODE,
	[SubUnitsetName] = UNITSETL.NAME,
	[IsVariant] = ITEMS.CANCONFIGURE,
	[TrackingType] = ITEMS.TRACKTYPE,
	[LocTracking] = ITEMS.LOCTRACKING,
	[GroupCode] = ISNULL(ITEMS.STGRPCODE,''),
	[BrandReferenceId] = ISNULL(BRAND.LOGICALREF,0),
	[BrandCode] = ISNULL(BRAND.CODE,''),
	[BrandName] = ISNULL(BRAND.DESCR,''),
    [Image] = ISNULL((SELECT TOP 1 FIRMDOC.LDATA
                 FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC
                 WHERE FIRMDOC.INFOREF = ITEMS.LOGICALREF
                 AND FIRMDOC.INFOTYP = 20
                 AND FIRMDOC.DOCNR = 11), '')

    FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_SUPPASGN AS SUPPASGN WITH(NOLOCK)
	LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON SUPPASGN.ITEMREF = ITEMS.LOGICALREF
	LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON ITEMS.UNITSETREF = UNITSETF.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON UNITSETF.LOGICALREF = UNITSETL.UNITSETREF AND UNITSETL.MAINUNIT = 1
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND WITH(NOLOCK) ON ITEMS.MARKREF = BRAND.LOGICALREF WHERE SUPPASGN.CLIENTREF = {supplierReferenceId}
		";

        if (!string.IsNullOrEmpty(search))
        {
            baseQuery += @$" AND ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%'";
        }

        baseQuery += @$" ORDER BY ITEMS.CODE ASC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }
}