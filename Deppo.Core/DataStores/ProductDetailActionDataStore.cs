using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace Deppo.Core.DataStores
{
    public class ProductDetailActionDataStore : IProductDetailActionService
    {
        private string postUrl = "/gateway/customQuery/CustomQuery";

        public async Task<DataResult<IEnumerable<dynamic>>> GetWarehouses(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "")
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetWarehousesQuery(firmNumber, periodNumber, productReferenceId, search, skip, take, externalDb)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<IEnumerable<dynamic>>> GetWaitingSalesOrders(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "")
        {
            var content = new StringContent(JsonConvert.SerializeObject(WaitingSalesOrderQuery(firmNumber, periodNumber, productReferenceId, search, skip, take, externalDb)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<IEnumerable<dynamic>>> GetWaitingPurchaseOrders(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "")
        {
            var content = new StringContent(JsonConvert.SerializeObject(WaitingPurchaseOrderQuery(firmNumber, periodNumber, productReferenceId, search, skip, take, externalDb)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<IEnumerable<dynamic>>> GetVariantTotals(HttpClient httpClient, int firmNumber, int periodNumber, int variantReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "")
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetVariantTotalsQuery(firmNumber, periodNumber, variantReferenceId, search, skip, take, externalDb)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<IEnumerable<dynamic>>> GetLocationTransactions(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, int skip = 0, int take = 20, string search = "", string externalDb = "")
        {
            var content = new StringContent(JsonConvert.SerializeObject(LocationTransactionQuery(firmNumber, periodNumber, productReferenceId, skip, take, search, externalDb)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<IEnumerable<dynamic>>> GetApprovedSuppliers(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetApprovedSuppliersQuery(firmNumber, periodNumber, productReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

		public async Task<DataResult<IEnumerable<dynamic>>> GetAlternativeProducts(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20)
		{
			var content = new StringContent(JsonConvert.SerializeObject(GetAlternativeProductsQuery(firmNumber, periodNumber, productReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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


		private string GetWarehousesQuery(int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "")
        {
            var baseQuery = @$"SELECT
			[ReferenceId] = LGMAIN.LOGICALREF,
			[Number] = LGMAIN.NR,
			[Name] = LGMAIN.NAME,
			[DivisionReferenceId] = 0,
			[DivisionNumber] = LGMAIN.DIVISNR,
			[City] = LGMAIN.CITY,
			[Country] = LGMAIN.COUNTRY,
			[Quantity] = ISNULL((SELECT SUM(ONHAND) FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK) WHERE STINVTOT.STOCKREF = {productReferenceId} AND STINVTOT.INVENNO = LGMAIN.NR),0)
			FROM {externalDb}L_CAPIWHOUSE AS LGMAIN WITH (NOLOCK)

            WHERE LGMAIN.FIRMNR = {firmNumber}";

            if (!string.IsNullOrEmpty(search))
            {
                baseQuery += $" AND (LGMAIN.NAME LIKE '%{search}%' OR LGMAIN.NR LIKE '%{search}%')";
            }

            baseQuery += @$"
ORDER BY
    LGMAIN.NR
OFFSET {skip} ROWS
FETCH NEXT {take} ROWS ONLY;";

            return baseQuery;
        }

        private string WaitingSalesOrderQuery(int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "")
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
        LEFT JOIN {externalDb}L_CAPIWHOUSE AS WHOUSE ON ORFLINE.SOURCEINDEX = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
		WHERE ORFLINE.CLOSED = 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 1
		AND ITEMS.UNITSETREF <> 0 AND ITEMS.LOGICALREF = {productReferenceId}
		";

            if (!string.IsNullOrEmpty(search))
            {
                baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";
            }

            baseQuery += $@" ORDER BY (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) DESC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

            return baseQuery;
        }

        private string WaitingPurchaseOrderQuery(int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "")
        {
            string baseQuery = @$"SELECT
			[ReferenceId] = ORFLINE.LOGICALREF,
            [OrderReferenceId] = ISNULL(ORFICHE.LOGICALREF,0),
            [OrderNumber] = ISNULL(ORFICHE.FICHENO,''),
            [SupplierReferenceId] = ISNULL(CLCARD.LOGICALREF,0),
            [SupplierCode] = ISNULL(CLCARD.CODE,''),
            [SupplierName] = ISNULL(CLCARD.DEFINITION_,''),
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
        LEFT JOIN {externalDb}L_CAPIWHOUSE AS WHOUSE ON ORFLINE.SOURCEINDEX = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
		WHERE ORFLINE.CLOSED = 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 2
		AND ITEMS.UNITSETREF <> 0 AND ITEMS.LOGICALREF = {productReferenceId}
		";

            if (!string.IsNullOrEmpty(search))
            {
                baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";
            }

            baseQuery += $@" ORDER BY (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) DESC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

            return baseQuery;
        }

        private string LocationTransactionQuery(int firmNumber, int periodNumber, int productReferenceId, int skip = 0, int take = 20, string search = "", string externalDb = "")
        {
            var baseQuery = @$"SELECT
    [LocationReferenceId] = ISNULL(LGMAIN.LOCREF, 0),
    [LocationCode] = ISNULL(INVLOC.CODE, ''),
    [LocationName] = ISNULL(INVLOC.NAME, ''),
    [SubUnitsetReferenceId] =(SELECT LOGICALREF FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL WHERE UNITSETREF = UNITSET.LOGICALREF AND MAINUNIT = 1 ),
	[SubUnitsetCode] = (SELECT CODE FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL WHERE UNITSETREF = UNITSET.LOGICALREF AND MAINUNIT = 1 ),
	[SubUnitsetName] = (SELECT LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL.NAME FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL WHERE UNITSETREF = UNITSET.LOGICALREF AND MAINUNIT = 1 ),
    [UnitsetReferenceId] = UNITSET.LOGICALREF,
    [UnitsetCode] = UNITSET.CODE,
    [UnitsetName] = UNITSET.NAME,
    [RemainingQuantity] = ISNULL(SUM(LGMAIN.REMAMOUNT), 0),
	[WarehouseReferenceId] = WHOUSE.LOGICALREF,
	[WarehouseName] = WHOUSE.NAME,
	[WarehouseNumber] = WHOUSE.NR
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_SLTRANS LGMAIN WITH(NOLOCK)
LEFT OUTER JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS ITEMS WITH(NOLOCK) ON (LGMAIN.ITEMREF  =  ITEMS.LOGICALREF)
LEFT OUTER JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_LOCATION INVLOC WITH(NOLOCK) ON (LGMAIN.LOCREF  =  INVLOC.LOGICALREF)
LEFT OUTER JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF UNITSET WITH(NOLOCK) ON (ITEMS.UNITSETREF  =  UNITSET.LOGICALREF)
LEFT OUTER JOIN {externalDb}L_CAPIWHOUSE WHOUSE WITH(NOLOCK) ON (LGMAIN.INVENNO = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber})
WHERE
    LGMAIN.CANCELLED = 0 AND
    LGMAIN.LPRODSTAT = 0 AND
    LGMAIN.ITEMREF = {productReferenceId} AND
    LGMAIN.EXIMFCTYPE IN (0, 2, 3, 4, 5, 7) AND
    LGMAIN.STATUS = 0 AND
    LGMAIN.IOCODE IN (1, 2, 3, 4)
              ";

            if (!string.IsNullOrEmpty(search))
            {
                baseQuery += $@" AND (INVLOC.CODE LIKE '{search}%' OR INVLOC.NAME LIKE '%{search}%')";
            }

            baseQuery += $@" GROUP BY
    LGMAIN.LOCREF,
    INVLOC.CODE,
    INVLOC.NAME,
    UNITSET.LOGICALREF,
    UNITSET.CODE,
    UNITSET.NAME,
	WHOUSE.LOGICALREF,
	WHOUSE.NAME,
	WHOUSE.NR
ORDER BY
    INVLOC.CODE
OFFSET {skip} ROWS
FETCH NEXT {take} ROWS ONLY;";

            return baseQuery;
        }

        private string GetVariantTotalsQuery(int firmNumber, int periodNumber, int variantReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "")
        {
            string baseQuery = @$"SELECT
			[ReferenceId] = LGMAIN.LOGICALREF,
			[Number] = LGMAIN.NR,
			[Name] = LGMAIN.NAME,
			[DivisionReferenceId] = 0,
			[DivisionNumber] = LGMAIN.DIVISNR,
			[City] = LGMAIN.CITY,
			[Country] = LGMAIN.COUNTRY,
			[Quantity] = ISNULL((SELECT SUM(ONHAND) FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_VRNTINVTOT AS VRNTINVTOT WITH(NOLOCK) WHERE VRNTINVTOT.VARIANTREF = {variantReferenceId} AND VRNTINVTOT.INVENNO = LGMAIN.NR),0)
			FROM {externalDb}L_CAPIWHOUSE AS LGMAIN WITH (NOLOCK)

            WHERE LGMAIN.FIRMNR = {firmNumber}";

            if (!string.IsNullOrEmpty(search))
            {
                baseQuery += $" AND (LGMAIN.NAME LIKE '%{search}%' OR LGMAIN.NR LIKE '%{search}%')";
            }

            baseQuery += @$"
ORDER BY
    LGMAIN.NR
OFFSET {skip} ROWS
FETCH NEXT {take} ROWS ONLY;";

            return baseQuery;
        }

        private string GetApprovedSuppliersQuery(int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20)
        {
            string baseQuery = @$"SELECT
SUPPASGN.CLCARDTYPE,
    SUPPASGN.ITEMREF,
    CLCARD.LOGICALREF AS [ReferenceId],
    CLCARD.CODE AS [Code],
    CLCARD.DEFINITION_ AS [Title],
    CASE
        WHEN CLCARD.ISPERSCOMP = 0 THEN 0
        ELSE 1
    END AS [IsPersonal],
    CLCARD.DEFINITION_ AS [Name],
    CLCARD.EMAILADDR AS [Email],
    CLCARD.TELNRS1 + ' ' + CLCARD.TELNRS2 AS [Telephone],
    CLCARD.ADDR1 AS [Address],
    CLCARD.CITY AS [City],
    CLCARD.COUNTRY AS [Country],
    CLCARD.POSTCODE AS [PostalCode],
    CLCARD.TAXOFFICE AS [TaxOffice],
    CLCARD.TAXNR AS [TaxNumber] FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_SUPPASGN AS SUPPASGN
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON SUPPASGN.CLIENTREF = CLCARD.LOGICALREF
WHERE SUPPASGN.ITEMREF = {productReferenceId}";

            if (!string.IsNullOrEmpty(search))
            {
                baseQuery += $" AND (CLCARD.CODE LIKE '%{search}%' OR CLCARD.DEFINITION_ LIKE '%{search}%')";
            }

            baseQuery += @$"
ORDER BY
    CLCARD.CODE
OFFSET {skip} ROWS
FETCH NEXT {take} ROWS ONLY;";

            return baseQuery;
        }

		private string GetAlternativeProductsQuery(int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20)
		{
			string baseQuery = @$"SELECT
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
[Image] = ISNULL(FIRMDOC.LDATA,''),
            [StockQuantity] = ISNULL((SELECT SUM(ONHAND) FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT WHERE STOCKREF = ITEMS.LOGICALREF AND INVENNO = -1),0)
            FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMSUBS AS ITEMSUBS
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON ITEMSUBS.SUBITEMREF = ITEMS.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC ON FIRMDOC.INFOREF = ITEMS.LOGICALREF AND FIRMDOC.INFOTYP = 20
            LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF ON ITEMS.UNITSETREF = UNITSETF.LOGICALREF
		    LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND ON ITEMS.MARKREF =BRAND.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL ON UNITSETF.LOGICALREF = UNITSETL.UNITSETREF
            WHERE ITEMS.CODE <> 'ÿ' AND ITEMS.MOLD = 0 AND UNITSETL.MAINUNIT=1 AND ITEMSUBS.MAINITEMREF ={productReferenceId}";

			if (!string.IsNullOrEmpty(search))
			{
				baseQuery += $" AND (ITEMS.CODE LIKE '%{search}%' OR ITEMS.NAME LIKE '%{search}%')";
			}

			baseQuery += @$"
ORDER BY
    ITEMS.CODE
OFFSET {skip} ROWS
FETCH NEXT {take} ROWS ONLY;";

			return baseQuery;
		}
	}
}