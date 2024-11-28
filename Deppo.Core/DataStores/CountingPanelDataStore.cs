using System;
using System.Text;
using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;

namespace Deppo.Core.DataStores;

public class CountingPanelDataStore : ICountingPanelService
{
    private string postUrl = "/gateway/customQuery/CustomQuery";

    public async Task<DataResult<IEnumerable<dynamic>>> GetLastCountingFiches(HttpClient httpClient, int firmNumber, int periodNumber)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetLastCountingFichesQuery(firmNumber, periodNumber)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<IEnumerable<dynamic>>> GetLastProducts(HttpClient httpClient, int firmNumber, int periodNumber)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetLastProductsQuery(firmNumber, periodNumber)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<IEnumerable<dynamic>>> GetLastCountingTransactions(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetLastCountingTransactionsQuery(firmNumber, periodNumber, ficheReferenceId)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<dynamic>> GetCountingInProductCount(HttpClient httpClient, int firmNumber, int periodNumber)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetCountingInProductCountQuery(firmNumber, periodNumber)), Encoding.UTF8, "application/json");

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
                var result = JsonConvert.DeserializeObject<DataResult<Dictionary<string, object>>>(data);

                dataResult.Data = result.Data;
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

    public async Task<DataResult<dynamic>> GetCountingOutProductCount(HttpClient httpClient, int firmNumber, int periodNumber)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetCountingOutProductCountQuery(firmNumber, periodNumber)), Encoding.UTF8, "application/json");

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
                var result = JsonConvert.DeserializeObject<DataResult<Dictionary<string, object>>>(data);

                dataResult.Data = result.Data;
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

    public async Task<DataResult<dynamic>> GetCountingTotalProductCount(HttpClient httpClient, int firmNumber, int periodNumber)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetCountingTotalProductCountQuery(firmNumber, periodNumber)), Encoding.UTF8, "application/json");

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
                var result = JsonConvert.DeserializeObject<DataResult<Dictionary<string, object>>>(data);

                dataResult.Data = result.Data;
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

    public async Task<DataResult<IEnumerable<dynamic>>> GetInProducts(HttpClient httpClient, int firmNumber, int periodNumber, int skip = 0, int take = 20)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetInProductsQuery(firmNumber, periodNumber, skip, take)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<dynamic>> GetInCountingTransactions(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetInCountingTransactionsQuery(firmNumber, periodNumber, productReferenceId)), Encoding.UTF8, "application/json");

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
                var result = JsonConvert.DeserializeObject<DataResult<Dictionary<string, object>>>(data);

                dataResult.Data = result.Data;
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

    public async Task<DataResult<IEnumerable<dynamic>>> GetOutProducts(HttpClient httpClient, int firmNumber, int periodNumber, int skip = 0, int take = 20)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetOutProductsQuery(firmNumber, periodNumber, skip, take)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<dynamic>> GetOutCountingTransactions(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetOutCountingTransactionsQuery(firmNumber, periodNumber, productReferenceId)), Encoding.UTF8, "application/json");

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
                var result = JsonConvert.DeserializeObject<DataResult<Dictionary<string, object>>>(data);

                dataResult.Data = result.Data;
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

    public async Task<DataResult<IEnumerable<dynamic>>> GetAllCountingTransactions(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId, int skip = 0, int take = 20)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetAllCountingTransactionsQuery(firmNumber, periodNumber, ficheReferenceId)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<IEnumerable<dynamic>>> GetCountingFiches(HttpClient httpClient, int firmNumber, int periodNumber, int skip = 0, int take = 20)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetCountingFichesQuery(firmNumber, periodNumber)), Encoding.UTF8, "application/json");

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

    private string GetCountingTotalProductCountQuery(int firmNumber, int periodNumber)
    {
        string baseQuery = $@"SELECT
[TotalProductCount] = ISNULL(COUNT(DISTINCT STLINE.STOCKREF),0)
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK)
WHERE STLINE.TRCODE IN(50,51) AND STLINE.LPRODSTAT = 0";

        return baseQuery;
    }

    private string GetCountingOutProductCountQuery(int firmNumber, int periodNumber)
    {
        string baseQuery = $@"SELECT
        [OutProductCount] = ISNULL(COUNT(DISTINCT STLINE.STOCKREF),0)
        FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
        WHERE STLINE.TRCODE = 51 AND STLINE.LPRODSTAT = 0";

        return baseQuery;
    }

    private string GetCountingInProductCountQuery(int firmNumber, int periodNumber)
    {
        string baseQuery = $@"SELECT
        [InProductCount] = ISNULL(COUNT(DISTINCT STLINE.STOCKREF),0)
        FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
        WHERE STLINE.TRCODE = 50 AND STLINE.LPRODSTAT = 0";

        return baseQuery;
    }

    private string GetLastCountingFichesQuery(int firmNumber, int periodNumber)
    {
        string baseQuery = $@"Select TOP 5
            [ReferenceId] = STFICHE.LOGICALREF,
			[FicheType] = STFICHE.TRCODE,
			[FicheNumber] = STFICHE.FICHENO,
            [FicheDate] = STFICHE.DATE_,
            [FicheTime] = dbo.LG_INTTOTIME(STFICHE.FTIME),
			[DocumentNumber] =ISNULL (STFICHE.DOCODE , ''),
			[SpecialCode] = ISNULL  ( STFICHE.SPECODE , ''),
			[CurrentReferenceID] = ISNULL ( CLCARD.LOGICALREF, 0),
			[CurrentCode] = ISNULL (CLCARD.CODE , '' ),
			[CurrentName] = ISNULL ( CLCARD.DEFINITION_ ,''),
			[WarehouseName] =  ISNULL (CAPIWHOUSE.NAME , ''),
			[WarehouseNumber] = ISNULL( CAPIWHOUSE.NR, 0),
			[Description] =  ISNULL (STFICHE.GENEXP1, '')
			From LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE
			left join LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON CLCARD.LOGICALREF = STFICHE.CLIENTREF
			LEFT JOIN L_CAPIWHOUSE AS CAPIWHOUSE on CAPIWHOUSE.NR = STFICHE.SOURCEINDEX AND CAPIWHOUSE.FIRMNR = {firmNumber}
			WHERE STFICHE.TRCODE IN(50,51) AND STFICHE.PRODSTAT = 0
			ORDER BY STFICHE.DATE_ DESC;";

        return baseQuery;
    }

    private string GetLastProductsQuery(int firmNumber, int periodNumber)
    {
        string baseQuery = $@"SELECT TOP 5
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
[StockQuantity] = SUM(ISNULL(STLINE.AMOUNT, 0)),
[Image] = ISNULL((SELECT TOP 1 FIRMDOC.LDATA 
                 FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC
                 WHERE FIRMDOC.INFOREF = ITEMS.LOGICALREF 
                 AND FIRMDOC.INFOTYP = 20  
                 AND FIRMDOC.DOCNR = 11), '')
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE ON STFICHE.LOGICALREF = STLINE.STFICHEREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON STLINE.STOCKREF = ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT WITH(NOLOCK) ON STLINE.VARIANTREF = VARIANT.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON STLINE.UOMREF = UNITSETL.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON STLINE.USREF = UNITSETF.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND WITH(NOLOCK) ON ITEMS.MARKREF = BRAND.LOGICALREF
WHERE STFICHE.GRPCODE = 3 AND STFICHE.PRODSTAT = 0 AND STLINE.LPRODSTAT = 0 AND STLINE.TRCODE IN(50,51)
GROUP BY
    ITEMS.LOGICALREF,
    ITEMS.CODE,
    ITEMS.NAME,
    ITEMS.VAT,
    UNITSETF.LOGICALREF,
    UNITSETF.CODE,
    UNITSETF.NAME,
    UNITSETL.LOGICALREF,
    UNITSETL.CODE,
    UNITSETL.NAME,
    ITEMS.CANCONFIGURE,
    ITEMS.TRACKTYPE,
    ITEMS.LOCTRACKING,
    ISNULL(ITEMS.STGRPCODE, ''),
    ISNULL(BRAND.LOGICALREF, 0),
    ISNULL(BRAND.CODE, ''),
    ISNULL(BRAND.DESCR, '')
ORDER BY MAX(STLINE.DATE_) DESC;";

        return baseQuery;
    }

    private string GetLastCountingTransactionsQuery(int firmNumber, int periodNumber, int ficheReferenceId)
    {
        string baseQuery = $@"SELECT
        [ReferenceId] = STLINE.LOGICALREF,
        [TransactionDate] = STLINE.DATE_,
        [TransactionTime] = dbo.LG_INTTOTIME(STFICHE.FTIME),
		[BaseTransactionReferenceId] = STFICHE.LOGICALREF,
        [BaseTransactionCode] = STFICHE.FICHENO,
        [TransactionType] = STLINE.TRCODE,
        [ProductReferenceId] = STLINE.STOCKREF,
        [ProductCode] = ITEMS.CODE,
        [Image] = FIRMDOC.LDATA,
        [ProductName] = ITEMS.NAME,
        [SubUnitsetCode] = SUBUNITSET.CODE,
        [SubUnitsetName] = SUBUNITSET.NAME,
        [SubUnitsetReferenceId] = SUBUNITSET.LOGICALREF,
        [UnitsetCode] = UNITSET.CODE,
        [UnitsetReferenceId] = UNITSET.LOGICALREF,
		[UnitsetName] = UNITSET.NAME,
        [Quantity] = STLINE.AMOUNT,
        [Description] = STLINE.LINEEXP,
        [IOType] = STLINE.IOCODE,
        [WarehouseNumber] = CAPIWHOUSE.NR,
        [WarehouseName] = CAPIWHOUSE.NAME

        FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE ON STLINE.STFICHEREF = STFICHE.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON STLINE.STOCKREF = ITEMS.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON STLINE.CLIENTREF = CLCARD.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC ON FIRMDOC.INFOREF = ITEMS.LOGICALREF AND FIRMDOC.INFOTYP = 20
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON STLINE.UOMREF = SUBUNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON STLINE.USREF = UNITSET.LOGICALREF
		LEFT JOIN L_CAPIWHOUSE AS CAPIWHOUSE ON STLINE.SOURCEINDEX = CAPIWHOUSE.NR AND CAPIWHOUSE.FIRMNR = {firmNumber}
		WHERE STFICHE.TRCODE IN(50,51) AND STFICHE.LOGICALREF = {ficheReferenceId} AND STFICHE.PRODSTAT = 0 AND STLINE.LPRODSTAT = 0
		ORDER BY STLINE.DATE_ DESC";

        return baseQuery;
    }

    private string GetInProductsQuery(int firmNumber, int periodNumber, int skip = 0, int take = 20)
    {
        string baseQuery = $@"
    SELECT * FROM (
        SELECT
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
            [StockQuantity] = SUM(ISNULL(STLINE.AMOUNT, 0)),
            [Image] = ISNULL((SELECT TOP 1 FIRMDOC.LDATA 
                 FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC
                 WHERE FIRMDOC.INFOREF = ITEMS.LOGICALREF 
                 AND FIRMDOC.INFOTYP = 20  
                 AND FIRMDOC.DOCNR = 11), '')
        FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE ON STFICHE.LOGICALREF = STLINE.STFICHEREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON STLINE.STOCKREF = ITEMS.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT WITH(NOLOCK) ON STLINE.VARIANTREF = VARIANT.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON STLINE.UOMREF = UNITSETL.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON STLINE.USREF = UNITSETF.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND WITH(NOLOCK) ON ITEMS.MARKREF = BRAND.LOGICALREF
        WHERE STLINE.TRCODE = 50 AND STLINE.LPRODSTAT = 0
         GROUP BY
                ITEMS.LOGICALREF,
                ITEMS.CODE,
                ITEMS.NAME,
                ITEMS.VAT,
                UNITSETF.LOGICALREF,
                UNITSETF.CODE,
                UNITSETF.NAME,
                UNITSETL.LOGICALREF,
                UNITSETL.CODE,
                UNITSETL.NAME,
                ITEMS.CANCONFIGURE,
                ITEMS.TRACKTYPE,
                ITEMS.LOCTRACKING,
                ISNULL(ITEMS.STGRPCODE, ''),
                ISNULL(BRAND.LOGICALREF, 0),
                ISNULL(BRAND.CODE, ''),
                ISNULL(BRAND.DESCR, '')
        ) AS subQuery
        ORDER BY subQuery.StockQuantity DESC
        OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY;";
        return baseQuery;
    }

    private string GetInCountingTransactionsQuery(int firmNumber, int periodNumber, int itemReferenceId)
    {
        string baseQuery = $@"SELECT
        [ReferenceId] = STLINE.LOGICALREF,
        [TransactionDate] = STLINE.DATE_,
        [TransactionTime] = dbo.LG_INTTOTIME(STFICHE.FTIME),
		[BaseTransactionReferenceId] = STFICHE.LOGICALREF,
        [BaseTransactionCode] = STFICHE.FICHENO,
        [TransactionType] = STLINE.TRCODE,
        [ProductReferenceId] = STLINE.STOCKREF,
        [ProductCode] = ITEMS.CODE,
        [ProductName] = ITEMS.NAME,
        [SubUnitsetCode] = SUBUNITSET.CODE,
        [SubUnitsetName] = SUBUNITSET.NAME,
        [SubUnitsetReferenceId] = SUBUNITSET.LOGICALREF,
        [UnitsetCode] = UNITSET.CODE,
        [UnitsetReferenceId] = UNITSET.LOGICALREF,
		[UnitsetName] = UNITSET.NAME,
        [Quantity] = STLINE.AMOUNT,
        [Description] = STLINE.LINEEXP,
        [IOType] = STLINE.IOCODE,
        [WarehouseNumber] = CAPIWHOUSE.NR,
        [WarehouseName] = CAPIWHOUSE.NAME

        FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE ON STLINE.STFICHEREF = STFICHE.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON STLINE.STOCKREF = ITEMS.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON STLINE.CLIENTREF = CLCARD.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON STLINE.UOMREF = SUBUNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON STLINE.USREF = UNITSET.LOGICALREF
		LEFT JOIN L_CAPIWHOUSE AS CAPIWHOUSE ON STLINE.SOURCEINDEX = CAPIWHOUSE.NR AND CAPIWHOUSE.FIRMNR = {firmNumber}
		WHERE STFICHE.TRCODE = 50 AND ITEMS.LOGICALREF = {itemReferenceId} AND STFICHE.PRODSTAT = 0 AND STLINE.LPRODSTAT = 0
		ORDER BY STLINE.DATE_ DESC";

        return baseQuery;
    }

    private string GetOutProductsQuery(int firmNumber, int periodNumber, int skip = 0, int take = 20)
    {
        string baseQuery = $@"
        SELECT * FROM (
            SELECT
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
                [GroupCode] = ISNULL(ITEMS.STGRPCODE, ''),
                [BrandReferenceId] = ISNULL(BRAND.LOGICALREF, 0),
                [BrandCode] = ISNULL(BRAND.CODE, ''),
                [BrandName] = ISNULL(BRAND.DESCR, ''),
                [StockQuantity] = SUM(ISNULL(STLINE.AMOUNT, 0)) ,
                [Image] = ISNULL((SELECT TOP 1 FIRMDOC.LDATA 
                 FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC
                 WHERE FIRMDOC.INFOREF = ITEMS.LOGICALREF 
                 AND FIRMDOC.INFOTYP = 20  
                 AND FIRMDOC.DOCNR = 11), '')
            FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
            LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE
                ON STFICHE.LOGICALREF = STLINE.STFICHEREF
            LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK)
                ON STLINE.STOCKREF = ITEMS.LOGICALREF
            LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT WITH(NOLOCK)
                ON STLINE.VARIANTREF = VARIANT.LOGICALREF
            LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK)
                ON STLINE.UOMREF = UNITSETL.LOGICALREF
            LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK)
                ON STLINE.USREF = UNITSETF.LOGICALREF
            LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND WITH(NOLOCK)
                ON ITEMS.MARKREF = BRAND.LOGICALREF
            WHERE STLINE.TRCODE = 51
                AND STLINE.LPRODSTAT = 0
            GROUP BY
                ITEMS.LOGICALREF,
                ITEMS.CODE,
                ITEMS.NAME,
                ITEMS.VAT,
                UNITSETF.LOGICALREF,
                UNITSETF.CODE,
                UNITSETF.NAME,
                UNITSETL.LOGICALREF,
                UNITSETL.CODE,
                UNITSETL.NAME,
                ITEMS.CANCONFIGURE,
                ITEMS.TRACKTYPE,
                ITEMS.LOCTRACKING,
                ISNULL(ITEMS.STGRPCODE, ''),
                ISNULL(BRAND.LOGICALREF, 0),
                ISNULL(BRAND.CODE, ''),
                ISNULL(BRAND.DESCR, '')
            ORDER BY MAX(STLINE.DATE_) DESC
            OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY
        ) AS subQuery
        ORDER BY subQuery.StockQuantity DESC;";

        return baseQuery;
    }

    private string GetOutCountingTransactionsQuery(int firmNumber, int periodNumber, int itemReferenceId)
    {
        string baseQuery = $@"SELECT
        [ReferenceId] = STLINE.LOGICALREF,
        [TransactionDate] = STLINE.DATE_,
        [TransactionTime] = dbo.LG_INTTOTIME(STFICHE.FTIME),
		[BaseTransactionReferenceId] = STFICHE.LOGICALREF,
        [BaseTransactionCode] = STFICHE.FICHENO,
        [TransactionType] = STLINE.TRCODE,
        [ProductReferenceId] = STLINE.STOCKREF,
        [ProductCode] = ITEMS.CODE,
        [ProductName] = ITEMS.NAME,
        [SubUnitsetCode] = SUBUNITSET.CODE,
        [SubUnitsetName] = SUBUNITSET.NAME,
        [SubUnitsetReferenceId] = SUBUNITSET.LOGICALREF,
        [UnitsetCode] = UNITSET.CODE,
        [UnitsetReferenceId] = UNITSET.LOGICALREF,
		[UnitsetName] = UNITSET.NAME,
        [Quantity] = STLINE.AMOUNT,
        [Description] = STLINE.LINEEXP,
        [IOType] = STLINE.IOCODE,
        [WarehouseNumber] = CAPIWHOUSE.NR,
        [WarehouseName] = CAPIWHOUSE.NAME

        FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE ON STLINE.STFICHEREF = STFICHE.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON STLINE.STOCKREF = ITEMS.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON STLINE.CLIENTREF = CLCARD.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON STLINE.UOMREF = SUBUNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON STLINE.USREF = UNITSET.LOGICALREF
		LEFT JOIN L_CAPIWHOUSE AS CAPIWHOUSE ON STLINE.SOURCEINDEX = CAPIWHOUSE.NR AND CAPIWHOUSE.FIRMNR = {firmNumber}
		WHERE STFICHE.TRCODE = 51 AND ITEMS.LOGICALREF = {itemReferenceId} AND STFICHE.PRODSTAT = 0 AND STLINE.LPRODSTAT = 0
		ORDER BY STLINE.DATE_ DESC";

        return baseQuery;
    }

    private string GetAllCountingTransactionsQuery(int firmNumber, int periodNumber, int ficheReferenceId, int skip = 0, int take = 20)
    {
        string baseQuery = $@"SELECT
        [ReferenceId] = STLINE.LOGICALREF,
        [TransactionDate] = STLINE.DATE_,
        [TransactionTime] = dbo.LG_INTTOTIME(STFICHE.FTIME),
		[BaseTransactionReferenceId] = STFICHE.LOGICALREF,
        [BaseTransactionCode] = STFICHE.FICHENO,
        [TransactionType] = STLINE.TRCODE,
        [ProductReferenceId] = STLINE.STOCKREF,
        [ProductCode] = ITEMS.CODE,
        [ProductName] = ITEMS.NAME,
        [SubUnitsetCode] = SUBUNITSET.CODE,
        [SubUnitsetName] = SUBUNITSET.NAME,
        [SubUnitsetReferenceId] = SUBUNITSET.LOGICALREF,
        [UnitsetCode] = UNITSET.CODE,
        [UnitsetReferenceId] = UNITSET.LOGICALREF,
		[UnitsetName] = UNITSET.NAME,
        [Quantity] = STLINE.AMOUNT,
        [Description] = STLINE.LINEEXP,
        [IOType] = STLINE.IOCODE,
        [WarehouseNumber] = CAPIWHOUSE.NR,
        [WarehouseName] = CAPIWHOUSE.NAME,
        [Image] = ISNULL((SELECT TOP 1 FIRMDOC.LDATA 
                 FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC
                 WHERE FIRMDOC.INFOREF = ITEMS.LOGICALREF 
                 AND FIRMDOC.INFOTYP = 20  
                 AND FIRMDOC.DOCNR = 11), '')

        FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE ON STLINE.STFICHEREF = STFICHE.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON STLINE.STOCKREF = ITEMS.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON STLINE.CLIENTREF = CLCARD.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON STLINE.UOMREF = SUBUNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON STLINE.USREF = UNITSET.LOGICALREF
		LEFT JOIN L_CAPIWHOUSE AS CAPIWHOUSE ON STLINE.SOURCEINDEX = CAPIWHOUSE.NR AND CAPIWHOUSE.FIRMNR = {firmNumber}
		WHERE STFICHE.TRCODE IN(50,51) AND STFICHE.LOGICALREF = {ficheReferenceId} AND  STFICHE.PRODSTAT = 0 AND STLINE.LPRODSTAT = 0
		ORDER BY STLINE.DATE_ DESC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";
        return baseQuery;
    }

    private string GetCountingFichesQuery(int firmNumber, int periodNumber, int skip = 0, int take = 20)
    {
        string baseQuery = $@"Select
            [ReferenceId] = STFICHE.LOGICALREF,
			[FicheType] = STFICHE.TRCODE,
			[FicheNumber] = STFICHE.FICHENO,
            [FicheDate] = STFICHE.DATE_,
            [FicheTime] = dbo.LG_INTTOTIME(STFICHE.FTIME),
			[DocumentNumber] =ISNULL (STFICHE.DOCODE , ''),
			[SpecialCode] = ISNULL  ( STFICHE.SPECODE , ''),
			[CurrentReferenceID] = ISNULL ( CLCARD.LOGICALREF, 0),
			[CurrentCode] = ISNULL (CLCARD.CODE , '' ),
			[CurrentName] = ISNULL ( CLCARD.DEFINITION_ ,''),
			[WarehouseName] =  ISNULL (CAPIWHOUSE.NAME , ''),
			[WarehouseNumber] = ISNULL( CAPIWHOUSE.NR, 0),
			[Description] =  ISNULL (STFICHE.GENEXP1, '')
			From LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE
			left join LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON CLCARD.LOGICALREF = STFICHE.CLIENTREF
			LEFT JOIN L_CAPIWHOUSE AS CAPIWHOUSE on CAPIWHOUSE.NR = STFICHE.SOURCEINDEX AND CAPIWHOUSE.FIRMNR = {firmNumber}
			WHERE STFICHE.TRCODE IN(50,51) AND STFICHE.PRODSTAT = 0
			ORDER BY STFICHE.DATE_ DESC  OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY;";

        return baseQuery;
    }
}