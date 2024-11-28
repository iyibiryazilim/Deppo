using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DataStores;

public class BarcodeSearchDataStore : IBarcodeSearchService
{
    string postUrl = "/gateway/customQuery/CustomQuery";

    public async Task<DataResult<dynamic>> SearchByProductCode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
    {
        var content = new StringContent(JsonConvert.SerializeObject(SearcByProductCodeQuery(firmNumber, periodNumber, barcode)), Encoding.UTF8, "application/json");

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
    public async Task<DataResult<dynamic>> SearchByVariantCode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
    {
        var content = new StringContent(JsonConvert.SerializeObject(SearchByVariantCodeQuery(firmNumber, periodNumber, barcode)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<dynamic>> SearchByProductMainBarcode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
    {
        var content = new StringContent(JsonConvert.SerializeObject(SearchByProductMainBarcodeQuery(firmNumber, periodNumber, barcode)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<dynamic>> SearchByVariantMainBarcode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
    {
        var content = new StringContent(JsonConvert.SerializeObject(SearchByVariantMainBarcodeQuery(firmNumber, periodNumber, barcode)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<dynamic>> SearchByProductSubBarcode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
    {
        var content = new StringContent(JsonConvert.SerializeObject(SearchByProductSubBarcodeQuery(firmNumber, periodNumber, barcode)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<dynamic>> SearchByVariantSubBarcode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
    {
        var content = new StringContent(JsonConvert.SerializeObject(SearchByVariantSubBarcodeQuery(firmNumber, periodNumber, barcode)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<dynamic>> SearchByProductSeriNumber(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
    {
        var content = new StringContent(JsonConvert.SerializeObject(SearchByProductSeriNumberQuery(firmNumber, periodNumber, barcode)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<dynamic>> SearchByVariantSeriNumber(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
    {
        var content = new StringContent(JsonConvert.SerializeObject(SearchByVariantSeriNumberQuery(firmNumber, periodNumber, barcode)), Encoding.UTF8, "application/json");

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
    public async Task<DataResult<dynamic>> SearchByProductLotNumber(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
    {
        var content = new StringContent(JsonConvert.SerializeObject(SearchByProductLotNumberQuery(firmNumber, periodNumber, barcode)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<dynamic>> SearchByVariantLotNumber(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
    {
        var content = new StringContent(JsonConvert.SerializeObject(SearchByVariantLotNumberQuery(firmNumber, periodNumber, barcode)), Encoding.UTF8, "application/json");

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
    public async Task<DataResult<dynamic>> SearchBySupplierProductCode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
    {
        var content = new StringContent(JsonConvert.SerializeObject(SearcBySupplierProductCodeQuery(firmNumber, periodNumber, barcode)), Encoding.UTF8, "application/json");

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
    public async Task<DataResult<dynamic>> SearchBySupplierVariantCode(HttpClient httpClient, int firmNumber, int periodNumber, string barcode)
    {
        var content = new StringContent(JsonConvert.SerializeObject(SearcBySupplierVariantCodeQuery(firmNumber, periodNumber, barcode)), Encoding.UTF8, "application/json");

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



    //ProductAndVariant Code
    private string SearcByProductCodeQuery(int firmNumber, int periodNumber,string barcode)
    {
        string baseQuery = $@"SELECT TOP 1
[ReferenceId] = ITEMS.LOGICALREF,
[Code] = ITEMS.CODE,
[Name] = ITEMS.NAME,
[Image] = ISNULL(FIRMDOC.LDATA,''),
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
[StockQuantity] = ISNULL((SELECT SUM(ONHAND) FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK) WHERE STINVTOT.STOCKREF = ITEMS.LOGICALREF AND STINVTOT.INVENNO = -1),0)

FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON ITEMS.UNITSETREF = UNITSETF.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON UNITSETF.LOGICALREF = UNITSETL.UNITSETREF AND UNITSETL.MAINUNIT = 1
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC WITH(NOLOCK) ON FIRMDOC.INFOREF = ITEMS.LOGICALREF AND FIRMDOC.INFOTYP = 20  AND FIRMDOC.DOCNR = 11
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND WITH(NOLOCK) ON ITEMS.MARKREF = BRAND.LOGICALREF
WHERE ITEMS.ACTIVE = 0 AND ITEMS.MOLD = 0 AND ITEMS.UNITSETREF <> 0 AND ITEMS.CODE = '{barcode}'";

        return baseQuery;
    }

    private string SearchByVariantCodeQuery(int firmNumber, int periodNumber , string barcode)
    {
        string baseQuery = $@"  select TOP 1
 [ReferenceId] = VARIANT.LOGICALREF,
 [Code] = VARIANT.CODE,
 [Name] = VARIANT.NAME,
 [ProductReferenceId] = ITEMS.LOGICALREF,
 [ProductCode] = ITEMS.CODE,
 [ProductName] = ITEMS.NAME,
 [VatRate] = ITEMS.VAT,
 [SubUnitsetReferenceId] = UNITSETL.LOGICALREF,
 [SubUnitsetCode] = UNITSETL.CODE,
 [SubUnitsetName] = UNITSETL.NAME,
 [UnitsetReferenceId] = UNITSETF.LOGICALREF,
 [UnitsetCode] = UNITSETF.CODE,
 [UnitsetName] = UNITSETF.NAME,
 [TrackingType] = ITEMS.TRACKTYPE,
 [LocTracking] = ITEMS.LOCTRACKING,
[GroupCode] = ISNULL(ITEMS.STGRPCODE,''),
[BrandReferenceId] = ISNULL(BRAND.LOGICALREF,0),
[BrandCode] = ISNULL(BRAND.CODE,''),
[BrandName] = ISNULL(BRAND.DESCR,''),
[StockQuantity] = 0
 from LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT as VARIANT
 LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON ITEMS.LOGICALREF = VARIANT.ITEMREF
 LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF  AS UNITSETF ON UNITSETF.LOGICALREF = VARIANT.UNITSETREF
 LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL ON UNITSETL.UNITSETREF = UNITSETF.LOGICALREF
 LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND WITH(NOLOCK) ON ITEMS.MARKREF = BRAND.LOGICALREF
WHERE VARIANT.CODE = '{barcode}'
";
        
        return baseQuery;
    }


    //MainBarcode
    private string SearchByProductMainBarcodeQuery(int firmNumber, int periodNumber, string barcode)
    {
        string baseQuery = $@"SELECT TOP 1
[ReferenceId] = ITEMS.LOGICALREF,
[Code] = ITEMS.CODE,
[Name] = ITEMS.NAME,
[Image] = ISNULL(FIRMDOC.LDATA,''),
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
[StockQuantity] = ISNULL((SELECT SUM(ONHAND) FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK) WHERE STINVTOT.STOCKREF = ITEMS.LOGICALREF AND STINVTOT.INVENNO = -1),0)
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITBARCODE AS BARCODE WITH(NOLOCK)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK)  ON BARCODE.ITEMREF = ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON UNITSETL.LOGICALREF = BARCODE.UNITLINEREF AND UNITSETL.MAINUNIT = 1
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON UNITSETL.UNITSETREF = UNITSETF.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC WITH(NOLOCK) ON FIRMDOC.INFOREF = ITEMS.LOGICALREF AND FIRMDOC.INFOTYP = 20  AND FIRMDOC.DOCNR = 11
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND WITH(NOLOCK) ON ITEMS.MARKREF = BRAND.LOGICALREF
WHERE UNITSETL.MAINUNIT = 1  AND BARCODE.BARCODE =  '{barcode}'";

        return baseQuery;
    }

    private string SearchByVariantMainBarcodeQuery(int firmNumber, int periodNumber, string barcode)
    {
        string baseQuery = $@" SELECT TOP 1
[ReferenceId] = VARIANT.LOGICALREF,
[Code] = VARIANT.CODE,
[Name] = VARIANT.NAME,
[ProductReferenceId] = ITEMS.LOGICALREF,
[ProductCode] = ITEMS.CODE,
[ProductName] = ITEMS.NAME,
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
[StockQuantity] = ISNULL((SELECT SUM(ONHAND) FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK) WHERE STINVTOT.STOCKREF = ITEMS.LOGICALREF AND STINVTOT.INVENNO = -1),0)
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITBARCODE AS BARCODE WITH(NOLOCK)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK)  ON BARCODE.ITEMREF = ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT WITH(NOLOCK) ON VARIANT.ITEMREF = ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON UNITSETL.LOGICALREF = BARCODE.UNITLINEREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON UNITSETL.UNITSETREF = UNITSETF.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND WITH(NOLOCK) ON ITEMS.MARKREF = BRAND.LOGICALREF
WHERE ITEMS.CANCONFIGURE =1 and UNITSETL.MAINUNIT = 1  AND BARCODE.BARCODE =  '{barcode}'
";

        return baseQuery;
    }


    //SubBarcode
    private string SearchByProductSubBarcodeQuery(int firmNumber, int periodNumber, string barcode)
    {
        string baseQuery = $@"SELECT TOP 1
[ReferenceId] = ITEMS.LOGICALREF,
[Code] = ITEMS.CODE,
[Name] = ITEMS.NAME,
[VatRate] = ITEMS.VAT,
[Image] = ISNULL(FIRMDOC.LDATA,''),
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
[StockQuantity] = ISNULL((SELECT SUM(ONHAND) FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK) WHERE STINVTOT.STOCKREF = ITEMS.LOGICALREF AND STINVTOT.INVENNO = -1),0)
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITBARCODE AS BARCODE WITH(NOLOCK)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK)  ON BARCODE.ITEMREF = ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON UNITSETL.LOGICALREF = BARCODE.UNITLINEREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON UNITSETL.UNITSETREF = UNITSETF.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC WITH(NOLOCK) ON FIRMDOC.INFOREF = ITEMS.LOGICALREF AND FIRMDOC.INFOTYP = 20  AND FIRMDOC.DOCNR = 11
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND WITH(NOLOCK) ON ITEMS.MARKREF = BRAND.LOGICALREF
WHERE UNITSETL.MAINUNIT = 0  AND BARCODE.BARCODE =  '{barcode}'";

        return baseQuery;
    }

    private string SearchByVariantSubBarcodeQuery(int firmNumber, int periodNumber, string barcode)
    {
        string baseQuery = $@"  SELECT TOP 1
[ReferenceId] = VARIANT.LOGICALREF,
[Code] = VARIANT.CODE,
[Name] = VARIANT.NAME,
[Image] = ISNULL(FIRMDOC.LDATA,''),
[ProductReferenceId] = ITEMS.LOGICALREF,
[ProductCode] = ITEMS.CODE,
[ProductName] = ITEMS.NAME,
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
[StockQuantity] = ISNULL((SELECT SUM(ONHAND) FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK) WHERE STINVTOT.STOCKREF = ITEMS.LOGICALREF AND STINVTOT.INVENNO = -1),0)
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITBARCODE AS BARCODE WITH(NOLOCK)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK)  ON BARCODE.ITEMREF = ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT WITH(NOLOCK) ON VARIANT.ITEMREF = ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON UNITSETL.LOGICALREF = BARCODE.UNITLINEREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON UNITSETL.UNITSETREF = UNITSETF.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC WITH(NOLOCK) ON FIRMDOC.INFOREF = ITEMS.LOGICALREF AND FIRMDOC.INFOTYP = 20  AND FIRMDOC.DOCNR = 11
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND WITH(NOLOCK) ON ITEMS.MARKREF = BRAND.LOGICALREF
WHERE ITEMS.CANCONFIGURE =1 and UNITSETL.MAINUNIT = 0  AND BARCODE.BARCODE =  '{barcode}'
";

        return baseQuery;
    }


    //SeriNumber
    private string SearchByProductSeriNumberQuery(int firmNumber, int periodNumber, string barcode)
    {
        string baseQuery = $@"SELECT TOP 1
[ReferenceId] = ITEMS.LOGICALREF,
[Code] = ITEMS.CODE,
[Name] = ITEMS.NAME,
[Image] = ISNULL(FIRMDOC.LDATA,''),
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
[StockQuantity] = ISNULL((SELECT SUM(ONHAND) FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK) WHERE STINVTOT.STOCKREF = ITEMS.LOGICALREF AND STINVTOT.INVENNO = -1),0)
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_SERILOTN AS SERI WITH(NOLOCK)
LEFT JOIN  LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON  ITEMS.LOGICALREF = SERI.ITEMREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON ITEMS.UNITSETREF = UNITSETF.LOGICALREF 
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON UNITSETF.LOGICALREF = UNITSETL.UNITSETREF AND UNITSETL.MAINUNIT = 1
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC WITH(NOLOCK) ON FIRMDOC.INFOREF = ITEMS.LOGICALREF AND FIRMDOC.INFOTYP = 20  AND FIRMDOC.DOCNR = 11
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND WITH(NOLOCK) ON ITEMS.MARKREF = BRAND.LOGICALREF
WHERE SERI.SLTYPE = 1 AND SERI.CODE = '{barcode}'";

        return baseQuery;
    }

    private string SearchByVariantSeriNumberQuery(int firmNumber, int periodNumber, string barcode)
    {
        string baseQuery = $@" select  TOP 1
 [ReferenceId] = VARIANT.LOGICALREF,
 [Code] = VARIANT.CODE,
 [Name] = VARIANT.NAME, 
 [Image] = ISNULL(FIRMDOC.LDATA,''),
 [ProductReferenceId] = ITEMS.LOGICALREF,
 [ProductCode] = ITEMS.CODE,
 [ProductName] = ITEMS.NAME,
 [VatRate] = ITEMS.VAT,
 [SubUnitsetReferenceId] = UNITSETL.LOGICALREF,
 [SubUnitsetCode] = UNITSETL.CODE,
 [SubUnitsetName] = UNITSETL.NAME,
 [UnitsetReferenceId] = UNITSETF.LOGICALREF,
 [UnitsetCode] = UNITSETF.CODE,
 [UnitsetName] = UNITSETF.NAME,
 [TrackingType] = ITEMS.TRACKTYPE,
 [LocTracking] = ITEMS.LOCTRACKING,
[GroupCode] = ISNULL(ITEMS.STGRPCODE,''),
[BrandReferenceId] = ISNULL(BRAND.LOGICALREF,0),
[BrandCode] = ISNULL(BRAND.CODE,''),
[BrandName] = ISNULL(BRAND.DESCR,''),
[StockQuantity] = 0
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_SERILOTN AS SERI WITH(NOLOCK)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT as VARIANT ON VARIANT.LOGICALREF = SERI.VARIANTREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON ITEMS.LOGICALREF = VARIANT.ITEMREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF  AS UNITSETF ON UNITSETF.LOGICALREF = VARIANT.UNITSETREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL ON UNITSETL.UNITSETREF = UNITSETF.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC WITH(NOLOCK) ON FIRMDOC.INFOREF = ITEMS.LOGICALREF AND FIRMDOC.INFOTYP = 20  AND FIRMDOC.DOCNR = 11
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND WITH(NOLOCK) ON ITEMS.MARKREF = BRAND.LOGICALREF
WHERE SERI.SLTYPE = 1 AND SERI.CODE = '{barcode}'
";

        return baseQuery;
    }

    
    //LotNumber

    private string SearchByProductLotNumberQuery(int firmNumber, int periodNumber, string barcode)
    {
        string baseQuery = $@"SELECT TOP 1
[ReferenceId] = ITEMS.LOGICALREF,
[Code] = ITEMS.CODE,
[Name] = ITEMS.NAME,
[Image] = ISNULL(FIRMDOC.LDATA,''),
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
[StockQuantity] = ISNULL((SELECT SUM(ONHAND) FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK) WHERE STINVTOT.STOCKREF = ITEMS.LOGICALREF AND STINVTOT.INVENNO = -1),0)
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_SERILOTN AS LOT WITH(NOLOCK)
LEFT JOIN  LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON  ITEMS.LOGICALREF = LOT.ITEMREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON ITEMS.UNITSETREF = UNITSETF.LOGICALREF 
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON UNITSETF.LOGICALREF = UNITSETL.UNITSETREF AND UNITSETL.MAINUNIT = 1
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC WITH(NOLOCK) ON FIRMDOC.INFOREF = ITEMS.LOGICALREF AND FIRMDOC.INFOTYP = 20  AND FIRMDOC.DOCNR = 11
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND WITH(NOLOCK) ON ITEMS.MARKREF = BRAND.LOGICALREF
WHERE LOT.SLTYPE = 0 AND LOT.CODE = '{barcode}'";

        return baseQuery;
    }

    private string SearchByVariantLotNumberQuery(int firmNumber, int periodNumber, string barcode)
    {
        string baseQuery = $@"  select  TOP 1
 [ReferenceId] = VARIANT.LOGICALREF,
 [Code] = VARIANT.CODE,
 [Name] = VARIANT.NAME,
 [Image] = ISNULL(FIRMDOC.LDATA,''),
 [ProductReferenceId] = ITEMS.LOGICALREF,
 [ProductCode] = ITEMS.CODE,
 [ProductName] = ITEMS.NAME,
 [VatRate] = ITEMS.VAT,
 [SubUnitsetReferenceId] = UNITSETL.LOGICALREF,
 [SubUnitsetCode] = UNITSETL.CODE,
 [SubUnitsetName] = UNITSETL.NAME,
 [UnitsetReferenceId] = UNITSETF.LOGICALREF,
 [UnitsetCode] = UNITSETF.CODE,
 [UnitsetName] = UNITSETF.NAME,
 [TrackingType] = ITEMS.TRACKTYPE,
 [LocTracking] = ITEMS.LOCTRACKING,
[GroupCode] = ISNULL(ITEMS.STGRPCODE,''),
[BrandReferenceId] = ISNULL(BRAND.LOGICALREF,0),
[BrandCode] = ISNULL(BRAND.CODE,''),
[BrandName] = ISNULL(BRAND.DESCR,''),
[StockQuantity] = 0
 FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_SERILOTN AS LOT WITH(NOLOCK)
 LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT as VARIANT ON VARIANT.LOGICALREF = LOT.VARIANTREF
 LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON ITEMS.LOGICALREF = VARIANT.ITEMREF
 LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF  AS UNITSETF ON UNITSETF.LOGICALREF = VARIANT.UNITSETREF
 LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL ON UNITSETL.UNITSETREF = UNITSETF.LOGICALREF
 LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC WITH(NOLOCK) ON FIRMDOC.INFOREF = ITEMS.LOGICALREF AND FIRMDOC.INFOTYP = 20  AND FIRMDOC.DOCNR = 11
 LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND WITH(NOLOCK) ON ITEMS.MARKREF = BRAND.LOGICALREF
WHERE LOT.SLTYPE = 0 AND LOT.CODE = '{barcode}'
";

        return baseQuery;
    }


    //SupplierProductCode

    private string SearcBySupplierProductCodeQuery(int firmNumber, int periodNumber, string barcode)
    {
        string baseQuery = $@"SELECT TOP 1
[ReferenceId] = ITEMS.LOGICALREF,
[Code] = ITEMS.CODE,
[Name] = ITEMS.NAME,
[Image] = ISNULL(FIRMDOC.LDATA,''),
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
[StockQuantity] = ISNULL((SELECT SUM(ONHAND) FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK) WHERE STINVTOT.STOCKREF = ITEMS.LOGICALREF AND STINVTOT.INVENNO = -1),0)
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_SUPPASGN AS SUPPASGN WITH(NOLOCK)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK)  ON SUPPASGN.ITEMREF = ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD WITH(NOLOCK) ON SUPPASGN.CLIENTREF = CLCARD.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON ITEMS.UNITSETREF = UNITSETF.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON UNITSETF.LOGICALREF = UNITSETL.UNITSETREF AND UNITSETL.MAINUNIT = 1
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC WITH(NOLOCK) ON FIRMDOC.INFOREF = ITEMS.LOGICALREF AND FIRMDOC.INFOTYP = 20  AND FIRMDOC.DOCNR = 11
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND WITH(NOLOCK) ON ITEMS.MARKREF = BRAND.LOGICALREF
WHERE SUPPASGN.SUPPLYTYPE = 1 AND SUPPASGN.ICUSTSUPBARCODE = '{barcode}' or SUPPASGN.ICUSTSUPCODE = '{barcode}'";
        return baseQuery;
    }

    private string SearcBySupplierVariantCodeQuery(int firmNumber, int periodNumber, string barcode)
    {
        string baseQuery = $@"  select  TOP 1
 [ReferenceId] = VARIANT.LOGICALREF,
 [Code] = VARIANT.CODE,
 [Name] = VARIANT.NAME,
 [Image] = ISNULL(FIRMDOC.LDATA,''),
 [ProductReferenceId] = ITEMS.LOGICALREF,
 [ProductCode] = ITEMS.CODE,
 [ProductName] = ITEMS.NAME,
 [VatRate] = ITEMS.VAT,
 [SubUnitsetReferenceId] = UNITSETL.LOGICALREF,
 [SubUnitsetCode] = UNITSETL.CODE,
 [SubUnitsetName] = UNITSETL.NAME,
 [UnitsetReferenceId] = UNITSETF.LOGICALREF,
 [UnitsetCode] = UNITSETF.CODE,
 [UnitsetName] = UNITSETF.NAME,
 [TrackingType] = ITEMS.TRACKTYPE,
 [LocTracking] = ITEMS.LOCTRACKING,
[GroupCode] = ISNULL(ITEMS.STGRPCODE,''),
[BrandReferenceId] = ISNULL(BRAND.LOGICALREF,0),
[BrandCode] = ISNULL(BRAND.CODE,''),
[BrandName] = ISNULL(BRAND.DESCR,''),
[StockQuantity] = 0
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_SUPPASGN AS SUPPASGN WITH(NOLOCK)
 LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT as VARIANT WITH(NOLOCK) ON SUPPASGN.VARIANTREF = VARIANT.LOGICALREF
 LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON VARIANT.ITEMREF = ITEMS.LOGICALREF
 LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD WITH(NOLOCK) ON SUPPASGN.CLIENTREF = CLCARD.LOGICALREF
 LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF  AS UNITSETF ON UNITSETF.LOGICALREF = VARIANT.UNITSETREF
 LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL ON UNITSETL.UNITSETREF = UNITSETF.LOGICALREF
 LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC WITH(NOLOCK) ON FIRMDOC.INFOREF = ITEMS.LOGICALREF AND FIRMDOC.INFOTYP = 20  AND FIRMDOC.DOCNR = 11
 LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND WITH(NOLOCK) ON ITEMS.MARKREF = BRAND.LOGICALREF
WHERE SUPPASGN.SUPPLYTYPE = 1 AND SUPPASGN.ICUSTSUPBARCODE = '{barcode}' or SUPPASGN.ICUSTSUPCODE = '{barcode}'
";

        return baseQuery;
    }


}
