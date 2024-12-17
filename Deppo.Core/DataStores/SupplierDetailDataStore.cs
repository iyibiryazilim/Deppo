using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DataStores;

public class SupplierDetailDataStore : ISupplierDetailService
{
    private string postUrl = "/gateway/customQuery/CustomQuery";

    public async Task<DataResult<IEnumerable<dynamic>>> GetLastFichesBySupplier(HttpClient httpClient, int firmNumber, int periodNumber, int supplierReferenceId, string externalDb = "")
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetLastFichesBySupplierQuery(firmNumber, periodNumber, supplierReferenceId, externalDb)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<IEnumerable<dynamic>>> GetTransactionsByFiche(HttpClient httpClient, int firmNumber, int periodNumber, int ficheRefenceId, string externalDb = "")
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetTransactionsByFicheQuery(firmNumber, periodNumber, ficheRefenceId, externalDb)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<dynamic>> GetInputQuantity(HttpClient httpClient, int firmNumber, int periodNumber, int supplierReferenceId)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetInputQuantityQuery(firmNumber, periodNumber, supplierReferenceId)), Encoding.UTF8, "application/json");

        HttpResponseMessage responseMessage = await httpClient.PostAsync(postUrl, content);
        DataResult<dynamic> dataResult = new DataResult<dynamic>();
        if (responseMessage.IsSuccessStatusCode)
        {
            var data = await responseMessage.Content.ReadAsStringAsync();
            if (data != null)
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var result = JsonConvert.DeserializeObject<DataResult<IEnumerable<Dictionary<string, object>>>>(data);

                    dataResult.Data = result?.Data.FirstOrDefault();
                    dataResult.IsSuccess = true;
                    dataResult.Message = "success";
                    return dataResult;
                }
                else
                {
                    var result = JsonConvert.DeserializeObject<DataResult<IEnumerable<Dictionary<string, object>>>>(data);

                    dataResult.Data = result?.Data.FirstOrDefault();
                    dataResult.IsSuccess = true;
                    dataResult.Message = "empty";
                    return dataResult;
                }
            }
            else
            {
                var result = JsonConvert.DeserializeObject<DataResult<IEnumerable<Dictionary<string, object>>>>(data);

                dataResult.Data = null;
                dataResult.IsSuccess = false;
                dataResult.Message = await responseMessage.Content.ReadAsStringAsync();

                return dataResult;
            }
        }
        else
        {
            dataResult.Data = null;
            dataResult.IsSuccess = false;
            dataResult.Message = await responseMessage.Content.ReadAsStringAsync();
            return dataResult;
        }
    }

    public async Task<DataResult<dynamic>> GetOutputQuantity(HttpClient httpClient, int firmNumber, int periodNumber, int supplierReferenceId)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetOutputQuantityQuery(firmNumber, periodNumber, supplierReferenceId)), Encoding.UTF8, "application/json");

        HttpResponseMessage responseMessage = await httpClient.PostAsync(postUrl, content);
        DataResult<dynamic> dataResult = new DataResult<dynamic>();
        if (responseMessage.IsSuccessStatusCode)
        {
            var data = await responseMessage.Content.ReadAsStringAsync();
            if (data != null)
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var result = JsonConvert.DeserializeObject<DataResult<IEnumerable<Dictionary<string, object>>>>(data);

                    dataResult.Data = result?.Data.FirstOrDefault();
                    dataResult.IsSuccess = true;
                    dataResult.Message = "success";
                    return dataResult;
                }
                else
                {
                    var result = JsonConvert.DeserializeObject<DataResult<IEnumerable<Dictionary<string, object>>>>(data);

                    dataResult.Data = result?.Data.FirstOrDefault();
                    dataResult.IsSuccess = true;
                    dataResult.Message = "empty";
                    return dataResult;
                }
            }
            else
            {
                var result = JsonConvert.DeserializeObject<DataResult<IEnumerable<Dictionary<string, object>>>>(data);

                dataResult.Data = null;
                dataResult.IsSuccess = false;
                dataResult.Message = await responseMessage.Content.ReadAsStringAsync();

                return dataResult;
            }
        }
        else
        {
            dataResult.Data = null;
            dataResult.IsSuccess = false;
            dataResult.Message = await responseMessage.Content.ReadAsStringAsync();
            return dataResult;
        }
    }

    public async Task<DataResult<dynamic>> GetWaitingProductReferenceCount(HttpClient httpClient, int firmNumber, int periodNumber, int supplierReferenceId)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetWaitingProductReferenceCountQuery(firmNumber, periodNumber, supplierReferenceId)), Encoding.UTF8, "application/json");

        HttpResponseMessage responseMessage = await httpClient.PostAsync(postUrl, content);
        DataResult<dynamic> dataResult = new DataResult<dynamic>();
        if (responseMessage.IsSuccessStatusCode)
        {
            var data = await responseMessage.Content.ReadAsStringAsync();
            if (data != null)
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var result = JsonConvert.DeserializeObject<DataResult<IEnumerable<Dictionary<string, object>>>>(data);

                    dataResult.Data = result?.Data.FirstOrDefault();
                    dataResult.IsSuccess = true;
                    dataResult.Message = "success";
                    return dataResult;
                }
                else
                {
                    var result = JsonConvert.DeserializeObject<DataResult<IEnumerable<Dictionary<string, object>>>>(data);

                    dataResult.Data = result?.Data.FirstOrDefault();
                    dataResult.IsSuccess = true;
                    dataResult.Message = "empty";
                    return dataResult;
                }
            }
            else
            {
                var result = JsonConvert.DeserializeObject<DataResult<IEnumerable<Dictionary<string, object>>>>(data);

                dataResult.Data = null;
                dataResult.IsSuccess = false;
                dataResult.Message = await responseMessage.Content.ReadAsStringAsync();

                return dataResult;
            }
        }
        else
        {
            dataResult.Data = null;
            dataResult.IsSuccess = false;
            dataResult.Message = await responseMessage.Content.ReadAsStringAsync();
            return dataResult;
        }
    }

    public async Task<DataResult<IEnumerable<dynamic>>> SupplierInputOutputQuantities(HttpClient httpClient, int firmNumber, int periodNumber, DateTime dateTime, int supplierReferenceId)
    {
        var content = new StringContent(JsonConvert.SerializeObject(SupplierInputOutputChartQuery(firmNumber, periodNumber, dateTime, supplierReferenceId)), Encoding.UTF8, "application/json");

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

    private string GetLastFichesBySupplierQuery(int firmNumber, int periodNumber, int supplierReferenceId, string externalDb = "")
    {
        string baseQuery = $@"SELECT TOP 5
            [ReferenceId] = STFICHE.LOGICALREF,
			[FicheType] = STFICHE.TRCODE,
			[FicheNumber] = STFICHE.FICHENO,
            [FicheDate] = STFICHE.DATE_,
            [FicheTime] = dbo.LG_INTTOTIME(STFICHE.FTIME),
			[DocumentNumber] =ISNULL (STFICHE.DOCODE , ''),
			[SpecialCode] = ISNULL  (STFICHE.SPECODE , ''),
			[CurrentReferenceId] = ISNULL (CLCARD.LOGICALREF, 0),
			[CurrentCode] = ISNULL (CLCARD.CODE , '' ),
			[CurrentName] = ISNULL ( CLCARD.DEFINITION_ ,''),
			[WarehouseName] =  ISNULL (CAPIWHOUSE.NAME , ''),
			[WarehouseNumber] = ISNULL( CAPIWHOUSE.NR, 0),
			[Description] =  ISNULL (STFICHE.GENEXP1, '')
			FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE WITH(NOLOCK)
			LEFT join LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD WITH(NOLOCK) ON CLCARD.LOGICALREF = STFICHE.CLIENTREF
			LEFT JOIN {externalDb}L_CAPIWHOUSE AS CAPIWHOUSE WITH(NOLOCK) ON CAPIWHOUSE.NR = STFICHE.SOURCEINDEX AND CAPIWHOUSE.FIRMNR = {firmNumber}
			WHERE STFICHE.PRODSTAT = 0 AND CLCARD.LOGICALREF={supplierReferenceId}
			ORDER BY STFICHE.DATE_ DESC";

        return baseQuery;
    }

    private string GetTransactionsByFicheQuery(int firmNumber, int periodNumber, int ficheReferenceId, string externalDb = "")
    {
        string baseQuery = $@"SELECT
            [ReferenceId] = STLINE.LOGICALREF,
            [TransactionDate] = STLINE.DATE_,
            [TransactionTime] = dbo.LG_INTTOTIME(STLINE.FTIME),
			[SupplierReferenceId] = ISNULL(CLCARD.LOGICALREF,0),
			[SupplierCode] = ISNULL(CLCARD.CODE,''),
			[SupplierName] = ISNULL(CLCARD.DEFINITION_,''),
			[BaseTransactionReferenceId] = STFICHE.LOGICALREF,
			[BaseTransactionCode] = STFICHE.FICHENO,
			[TransactionType] = STLINE.TRCODE,
			[GroupCode] = STFICHE.GRPCODE,
			[IOType] = STLINE.IOCODE,
			[ProductReferenceId] =  ITEMS.LOGICALREF,
			[ProductCode] = ISNULL(ITEMS.CODE,''),
			[ProductName] = ISNULL(ITEMS.NAME,''),
            [Image] = FIRMDOC.LDATA,
			[UnitsetReferenceId] = ISNULL(UNITSET.LOGICALREF,0),
			[UnitsetCode] = ISNULL(UNITSET.CODE, ''),
			[UnitsetName] =  ISNULL(UNITSET.NAME , ''),
			[SubUnitsetReferenceId] = ISNULL(SUBUNITSET.LOGICALREF, 0),
			[SubUnitsetCode] = ISNULL(SUBUNITSET.CODE, ''),
			[SubUnitsetName] = ISNULL(SUBUNITSET.NAME, ''),
			[WarehouseName] = CAPIWHOUSE.NAME,
			[WarehouseNumber] = CAPIWHOUSE.NR,
			[Quantity] = ISNULL(STLINE.AMOUNT,0),
			[Length] =  ISNULL( STLINE.UINFO4 , 0),
			[Width] = ISNULL(STLINE.UINFO5, 0),
			[Height] = ISNULL(STLINE.UINFO6, 0),
			[Weight] = ISNULL(STLINE.UINFO7, 0),
			[Volume] = ISNULL(STLINE.UINFO8, 0),
			[Barcode] = ''
FROM  LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE WITH(NOLOCK) ON STLINE.STFICHEREF = STFICHE.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON STLINE.STOCKREF = ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD WITH(NOLOCK) ON STLINE.CLIENTREF = CLCARD.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC ON FIRMDOC.INFOREF = ITEMS.LOGICALREF AND FIRMDOC.INFOTYP = 20
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET WITH(NOLOCK) ON STLINE.UOMREF = SUBUNITSET.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET WITH(NOLOCK) ON STLINE.USREF = UNITSET.LOGICALREF
LEFT JOIN {externalDb}L_CAPIWHOUSE AS CAPIWHOUSE WITH(NOLOCK) ON STLINE.SOURCEINDEX = CAPIWHOUSE.NR AND CAPIWHOUSE.FIRMNR = {firmNumber}
WHERE STLINE.LINETYPE = 0 AND STFICHE.LOGICALREF = {ficheReferenceId}
ORDER BY STLINE.DATE_ DESC";

        return baseQuery;
    }

    /// <summary>
    /// Satınalınan ve Satış İade Yapılan Malzeme Referans sayısı
    /// </summary>
    /// <param name="firmNumber"></param>
    /// <param name="periodNumber"></param>
    /// <param name="supplierReferenceId"></param>
    /// <returns></returns>
    private string GetInputQuantityQuery(int firmNumber, int periodNumber, int supplierReferenceId)
    {
        string baseQuery = $@"SELECT
    InputQuantity = ISNULL(COUNT(DISTINCT STLINE.STOCKREF), 0)
FROM
    LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK)
LEFT JOIN
    LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE WITH(NOLOCK) ON STLINE.STFICHEREF = STFICHE.LOGICALREF
WHERE
    STFICHE.TRCODE IN(1,2,3) AND
    STLINE.LINETYPE = 0 AND
	STLINE.CLIENTREF = {supplierReferenceId}";

        return baseQuery;
    }

    /// <summary>
    /// Satış Yapılan ve Satınalma İade Yapılan Malzeme Referans sayısı
    /// </summary>
    /// <param name="firmNumber"></param>
    /// <param name="periodNumber"></param>
    /// <param name="supplierReferenceId"></param>
    /// <returns></returns>
    private string GetOutputQuantityQuery(int firmNumber, int periodNumber, int supplierReferenceId)
    {
        string baseQuery = $@"SELECT
    OutputQuantity = ISNULL(COUNT(DISTINCT STLINE.STOCKREF), 0)
FROM
    LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK)
LEFT JOIN
    LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE WITH(NOLOCK) ON STLINE.STFICHEREF = STFICHE.LOGICALREF
WHERE
   STFICHE.TRCODE IN(6,7,8) AND
    STLINE.LINETYPE = 0 AND
	STLINE.CLIENTREF = {supplierReferenceId}";

        return baseQuery;
    }

    /// <summary>
    /// Son 5 günün (Satınalınan ve Satış İade Yapılan) ve (Satış Yapılan ve Satınalma İade Yapılan) Malzeme Referansları
    /// </summary>
    /// <param name="firmNumber"></param>
    /// <param name="periodNumber"></param>
    /// <param name="dateTime"></param>
    /// <param name="supplierReferenceId"></param>
    /// <returns></returns>

    private string SupplierInputOutputChartQuery(int firmNumber, int periodNumber, DateTime dateTime, int supplierReferenceId)
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
		[PurchaseReferenceQuantity] = ISNULL(
			(SELECT COUNT(DISTINCT STLINE.STOCKREF)
			 FROM
				 LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK)
			 LEFT JOIN
				 LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE WITH(NOLOCK) ON STLINE.STFICHEREF = STFICHE.LOGICALREF
			 WHERE
				 STFICHE.TRCODE IN (1) AND
				 STLINE.LINETYPE = 0 AND
				 YEAR(STLINE.DATE_) = {xDate.Year} AND
				 MONTH(STLINE.DATE_) = {xDate.Month} AND
                 DAY(STLINE.DATE_) = {xDate.Day} AND
				 STLINE.CLIENTREF = {supplierReferenceId}
			),
		0),
		[ReturnReferenceQuantity] = ISNULL(
			(SELECT COUNT(DISTINCT STLINE.STOCKREF)
			 FROM
				 LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK)
			 LEFT JOIN
				 LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE WITH(NOLOCK) ON STLINE.STFICHEREF = STFICHE.LOGICALREF
			 WHERE
				 STFICHE.TRCODE IN (6) AND
				 YEAR(STLINE.DATE_) = {xDate.Year} AND
                 MONTH(STLINE.DATE_) = {xDate.Month} AND
                 DAY(STLINE.DATE_) = {xDate.Day} AND
				 STLINE.LINETYPE = 0 AND
				 STLINE.CLIENTREF = {supplierReferenceId}
			),
		0) UNION ALL";
            }
            else
            {
                baseQuery += $@"
	SELECT
		[Argument] = '{xDate.ToString("dddd")}',
		[ArgumentDay] = {xDate.Day.ToString().PadLeft(2, '0')},
		[PurchaseReferenceQuantity] = ISNULL(
			(SELECT COUNT(DISTINCT STLINE.STOCKREF)
			 FROM
				 LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK)
			 LEFT JOIN
				 LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE WITH(NOLOCK) ON STLINE.STFICHEREF = STFICHE.LOGICALREF
			 WHERE
				 STFICHE.TRCODE IN (1) AND
				 STLINE.LINETYPE = 0 AND
				 YEAR(STLINE.DATE_) = {xDate.Year} AND
				 MONTH(STLINE.DATE_) = {xDate.Month} AND
                 DAY(STLINE.DATE_) = {xDate.Day} AND
				 STLINE.CLIENTREF = {supplierReferenceId}
			),
		0),
		[ReturnReferenceQuantity] = ISNULL(
			(SELECT COUNT(DISTINCT STLINE.STOCKREF)
			 FROM
				 LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK)
			 LEFT JOIN
				 LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE WITH(NOLOCK) ON STLINE.STFICHEREF = STFICHE.LOGICALREF
			 WHERE
				 STFICHE.TRCODE IN (6) AND
				 YEAR(STLINE.DATE_) = {xDate.Year} AND
                 MONTH(STLINE.DATE_) = {xDate.Month} AND
                 DAY(STLINE.DATE_) = {xDate.Day} AND
				 STLINE.LINETYPE = 0 AND
				 STLINE.CLIENTREF = {supplierReferenceId}
			),
		0)";
            }
        }

        return baseQuery;
    }

    private string GetWaitingProductReferenceCountQuery(int firmNumber, int periodNumber, int supplierReferenceId)
    {
        string baseQuery = $@"SELECT

[WaitingProductReferenceCount] = ISNULL(COUNT(DISTINCT STOCKREF),0)
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE
WHERE CLIENTREF = {supplierReferenceId} AND TRCODE = 2 AND (AMOUNT - SHIPPEDAMOUNT) > 0 AND CLOSED = 0";

        return baseQuery;
    }
}