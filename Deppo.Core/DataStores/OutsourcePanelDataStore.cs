using System;
using System.Runtime.CompilerServices;
using System.Text;
using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;

namespace Deppo.Core.DataStores;

public class OutsourcePanelDataStore : IOutsourcePanelService
{
    private string postUrl = "/gateway/customQuery/CustomQuery";

    public async Task<DataResult<IEnumerable<dynamic>>> GetLastOutsourceFiches(HttpClient httpClient, int firmNumber, int periodNumber, string externalDb = "")
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetLastOutsourceFichesQuery(firmNumber, periodNumber, externalDb)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<IEnumerable<dynamic>>> GetLastOutsources(HttpClient httpClient, int firmNumber, int periodNumber)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetLastOutsourcesQuery(firmNumber, periodNumber)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<IEnumerable<dynamic>>> GetLastOutsourceTransactions(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId, int skip = 0, int take = 20, string externalDb = "")
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetLastOutsourceTransactionsQuery(firmNumber, periodNumber, ficheReferenceId, skip, take, externalDb)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<IEnumerable<dynamic>>> GetAllOutsourceFiches(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20, string externalDb = "")
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetAllOutsourceFichesQuery(firmNumber, periodNumber, search, skip, take, externalDb)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<dynamic>> GetOutsourceInProductCount(HttpClient httpClient, int firmNumber, int periodNumber)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetOutsourceInProductCountQuery(firmNumber, periodNumber)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<dynamic>> GetOutsourceOutProductCount(HttpClient httpClient, int firmNumber, int periodNumber)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetOutsourceOutProductCountQuery(firmNumber, periodNumber)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<dynamic>> GetOutsourceTotalProductCount(HttpClient httpClient, int firmNumber, int periodNumber)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetOutsourceTotalProductCountQuery(firmNumber, periodNumber)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<dynamic>> GetOutsourceInProductCountByProduct(HttpClient httpClient, int firmNumber, int periodNumber, int outsourceReferenceId)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetOutsourceInProductCountQueryByProduct(firmNumber, periodNumber, outsourceReferenceId)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<dynamic>> GetOutsourceOutProductCountByProduct(HttpClient httpClient, int firmNumber, int periodNumber, int outsourceReferenceId)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetOutsourceOutProductCountQueryByProduct(firmNumber, periodNumber, outsourceReferenceId)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<IEnumerable<dynamic>>> OutsourceInputOutputQuantities(HttpClient httpClient, int firmNumber, int periodNumber, DateTime dateTime, int outsourceReferenceId)
    {
        var content = new StringContent(JsonConvert.SerializeObject(OutsourceInputOutputChartQuery(firmNumber, periodNumber, dateTime, outsourceReferenceId)), Encoding.UTF8, "application/json");

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

    private string GetOutsourceTotalProductCountQuery(int firmNumber, int periodNumber)
    {
        string baseQuery = $@"SELECT
[TotalProductCount] = ISNULL(COUNT(DISTINCT STLINE.STOCKREF),0)
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS OUTSOURCE WITH(NOLOCK) ON STLINE.CLIENTREF = OUTSOURCE.LOGICALREF
WHERE STLINE.TRCODE = 25 AND STLINE.IOCODE IN(2,4) AND STLINE.LPRODSTAT = 0 AND OUTSOURCE.SUBCONT = 1";

        return baseQuery;
    }

    private string GetOutsourceOutProductCountQuery(int firmNumber, int periodNumber)
    {
        string baseQuery = $@"SELECT
        [OutProductCount] = ISNULL(COUNT(DISTINCT STLINE.STOCKREF),0)
        FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD WITH(NOLOCK) ON STLINE.CLIENTREF = CLCARD.LOGICALREF
        WHERE CLCARD.SUBCONT = 1 AND STLINE.TRCODE = 25 AND STLINE.IOCODE = 4 AND STLINE.LPRODSTAT = 0";

        return baseQuery;
    }

    private string GetOutsourceInProductCountQuery(int firmNumber, int periodNumber)
    {
        string baseQuery = $@"SELECT
        [InProductCount] = ISNULL(COUNT(DISTINCT STLINE.STOCKREF),0)
        FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD WITH(NOLOCK) ON STLINE.CLIENTREF = CLCARD.LOGICALREF
        WHERE CLCARD.SUBCONT = 1 AND STLINE.TRCODE = 25 AND STLINE.IOCODE = 2 AND STLINE.LPRODSTAT = 0";

        return baseQuery;
    }

    private string GetLastOutsourceFichesQuery(int firmNumber, int periodNumber, string externalDb = "")
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
			LEFT JOIN {externalDb}L_CAPIWHOUSE AS CAPIWHOUSE on CAPIWHOUSE.NR = STFICHE.SOURCEINDEX AND CAPIWHOUSE.FIRMNR = {firmNumber}
			WHERE STFICHE.TRCODE = 25 AND STFICHE.PRODSTAT = 0 AND STFICHE.CLIENTREF > 0 AND CLCARD.SUBCONT = 1
			ORDER BY STFICHE.DATE_ DESC;";

        return baseQuery;
    }

    private string GetLastOutsourcesQuery(int firmNumber, int periodNumber)
    {
        string baseQuery = $@"Select TOP 5
            [ReferenceId]=OUTSOURCE.LOGICALREF,
[Code]=OUTSOURCE.CODE,
[Title]=OUTSOURCE.DEFINITION_,
[IsPersonal] =
        CASE
            WHEN OUTSOURCE.ISPERSCOMP= 0 THEN 0
            ELSE 1
        END,
[Name]=OUTSOURCE.DEFINITION_,
[Email]=OUTSOURCE.EMAILADDR,
[Telephone]=OUTSOURCE.TELNRS1+' '+ OUTSOURCE.TELNRS2,
[Address]=OUTSOURCE.ADDR1,
[City]=OUTSOURCE.CITY,
[Country]=OUTSOURCE.COUNTRY,
[PostalCode]=OUTSOURCE.POSTCODE,
[TaxOffice]=OUTSOURCE.TAXOFFICE,
[TaxNumber]=OUTSOURCE.TAXNR,
[OrderReferenceCount] = 0,
[IsActive]=
       CASE
	      WHEN OUTSOURCE.ACTIVE=0 THEN 0
		  ELSE 1
END
			From LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE
			left join LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS OUTSOURCE ON OUTSOURCE.LOGICALREF = STFICHE.CLIENTREF
			WHERE STFICHE.TRCODE = 25 AND STFICHE.PRODSTAT = 0 AND STFICHE.CLIENTREF > 0 AND OUTSOURCE.SUBCONT = 1
			ORDER BY STFICHE.DATE_ DESC;";

        return baseQuery;
    }

    private string GetLastOutsourceTransactionsQuery(int firmNumber, int periodNumber, int ficheReferenceId, int skip, int take, string externalDb = "")
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
        [Image] = ISNULL(FIRMDOC.LDATA, ''),
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
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC ON FIRMDOC.INFOREF = ITEMS.LOGICALREF AND FIRMDOC.INFOTYP = 20  AND FIRMDOC.DOCNR = 11
		LEFT JOIN {externalDb}L_CAPIWHOUSE AS CAPIWHOUSE ON STLINE.SOURCEINDEX = CAPIWHOUSE.NR AND CAPIWHOUSE.FIRMNR = {firmNumber}
		WHERE STFICHE.TRCODE = 25 AND STLINE.IOCODE = 2 AND STFICHE.LOGICALREF = {ficheReferenceId} AND STFICHE.PRODSTAT = 0 AND STLINE.LPRODSTAT = 0
		ORDER BY STLINE.DATE_ DESC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }

    private string GetAllOutsourceFichesQuery(int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20, string externalDb = "")
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
			LEFT JOIN {externalDb}L_CAPIWHOUSE AS CAPIWHOUSE on CAPIWHOUSE.NR = STFICHE.SOURCEINDEX AND CAPIWHOUSE.FIRMNR = {firmNumber}
			WHERE STFICHE.TRCODE = 25 AND STFICHE.PRODSTAT = 0 AND STFICHE.CLIENTREF > 0 AND CLCARD.SUBCONT = 1
			";
        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";

        baseQuery += $@" ORDER BY STFICHE.DATE_ DESC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }

    private string GetOutsourceOutProductCountQueryByProduct(int firmNumber, int periodNumber, int outsourceReferenceId)
    {
        string baseQuery = $@"SELECT
        [OutputQuantity] = ISNULL(COUNT(DISTINCT STLINE.STOCKREF),0)
        FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD WITH(NOLOCK) ON STLINE.CLIENTREF = CLCARD.LOGICALREF
        WHERE CLCARD.LOGICALREF={outsourceReferenceId} AND CLCARD.SUBCONT = 1 AND STLINE.TRCODE = 25 AND STLINE.IOCODE = 4 AND STLINE.LPRODSTAT = 0";

        return baseQuery;
    }

    private string GetOutsourceInProductCountQueryByProduct(int firmNumber, int periodNumber, int outsourceReferenceId)
    {
        string baseQuery = $@"SELECT
        [InputQuantity] = ISNULL(COUNT(DISTINCT STLINE.STOCKREF),0)
        FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD WITH(NOLOCK) ON STLINE.CLIENTREF = CLCARD.LOGICALREF
        WHERE CLCARD.LOGICALREF={outsourceReferenceId} AND CLCARD.SUBCONT = 1 AND STLINE.TRCODE = 25 AND STLINE.IOCODE = 2 AND STLINE.LPRODSTAT = 0";

        return baseQuery;
    }

    private string OutsourceInputOutputChartQuery(int firmNumber, int periodNumber, DateTime dateTime, int outsourceReferenceId)
    {
        string baseQuery = $@"";
        DateTime xDate = dateTime;
        for (int i = 1; i < 6; i++)
        {
            if (i != 1)
                xDate = xDate.AddDays(-1);

            if (i != 5)
            {
                baseQuery += $@"
	SELECT
		[Argument] = '{xDate.ToString("dddd")}',
		[ArgumentDay] = {xDate.Day.ToString().PadLeft(2, '0')},
		[SalesReferenceQuantity] = ISNULL(
			(SELECT COUNT(DISTINCT STLINE.STOCKREF)
			 FROM
				 LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK)
			 LEFT JOIN
				 LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE WITH(NOLOCK) ON STLINE.STFICHEREF = STFICHE.LOGICALREF
			 WHERE
				 STFICHE.TRCODE IN (7,8) AND
				 STLINE.LINETYPE = 0 AND
				 YEAR(STLINE.DATE_) = {xDate.Year} AND
				 MONTH(STLINE.DATE_) = {xDate.Month} AND
                 DAY(STLINE.DATE_) = {xDate.Day} AND
				 STLINE.CLIENTREF = {outsourceReferenceId}
			),
		0),
		[ReturnReferenceQuantity] = ISNULL(
			(SELECT COUNT(DISTINCT STLINE.STOCKREF)
			 FROM
				 LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK)
			 LEFT JOIN
				 LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE WITH(NOLOCK) ON STLINE.STFICHEREF = STFICHE.LOGICALREF
			 WHERE
				 STFICHE.TRCODE IN (2,3) AND
				 YEAR(STLINE.DATE_) = {xDate.Year} AND
                 MONTH(STLINE.DATE_) = {xDate.Month} AND
                 DAY(STLINE.DATE_) = {xDate.Day} AND
				 STLINE.LINETYPE = 0 AND
				 STLINE.CLIENTREF = {outsourceReferenceId}
			),
		0) UNION ALL";
            }
            else
            {
                baseQuery += $@"
	SELECT
		[Argument] = '{xDate.ToString("dddd")}',
		[ArgumentDay] = {xDate.Day.ToString().PadLeft(2, '0')},
		[SalesReferenceQuantity] = ISNULL(
			(SELECT COUNT(DISTINCT STLINE.STOCKREF)
			 FROM
				 LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK)
			 LEFT JOIN
				 LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE WITH(NOLOCK) ON STLINE.STFICHEREF = STFICHE.LOGICALREF
			 WHERE
				 STFICHE.TRCODE IN (7,8) AND
				 STLINE.LINETYPE = 0 AND
				 YEAR(STLINE.DATE_) = {xDate.Year} AND
				 MONTH(STLINE.DATE_) = {xDate.Month} AND
                 DAY(STLINE.DATE_) = {xDate.Day} AND
				 STLINE.CLIENTREF = {outsourceReferenceId}
			),
		0),
		[ReturnReferenceQuantity] = ISNULL(
			(SELECT COUNT(DISTINCT STLINE.STOCKREF)
			 FROM
				 LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK)
			 LEFT JOIN
				 LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE WITH(NOLOCK) ON STLINE.STFICHEREF = STFICHE.LOGICALREF
			 WHERE
				 STFICHE.TRCODE IN (2,3) AND
				 YEAR(STLINE.DATE_) = {xDate.Year} AND
                 MONTH(STLINE.DATE_) = {xDate.Month} AND
                 DAY(STLINE.DATE_) = {xDate.Day} AND
				 STLINE.LINETYPE = 0 AND
				 STLINE.CLIENTREF = {outsourceReferenceId}
			),
		0)";
            }
        }

        return baseQuery;
    }
}