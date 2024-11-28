using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DataStores;

public class CustomerDetailDataStore : ICustomerDetailService
{
    private string postUrl = "/gateway/customQuery/CustomQuery";

    public async Task<DataResult<IEnumerable<dynamic>>> GetLastFichesByCustomer(HttpClient httpClient, int firmNumber, int periodNumber, int customerReferenceId)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetLastFichesByCustomer(firmNumber, periodNumber, customerReferenceId)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<IEnumerable<dynamic>>> GetLastTransaction(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId)
    {
        var content = new StringContent(JsonConvert.SerializeObject(CustomerTransaction(firmNumber, periodNumber, ficheReferenceId)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<dynamic>> GetInputQuantity(HttpClient httpClient, int firmNumber, int periodNumber, int customerReferenceId)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetInputQuantityQuery(firmNumber, periodNumber, customerReferenceId)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<dynamic>> GetOutputQuantity(HttpClient httpClient, int firmNumber, int periodNumber, int customerReferenceId)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetOutputQuantityQuery(firmNumber, periodNumber, customerReferenceId)), Encoding.UTF8, "application/json");

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


     public async Task<DataResult<dynamic>> GetWaitingProductReferenceCount(HttpClient httpClient, int firmNumber, int periodNumber, int customerReferenceId)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetWaitingProductReferenceCountQuery(firmNumber, periodNumber, customerReferenceId)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<IEnumerable<dynamic>>> CustomerInputOutputQuantities(HttpClient httpClient, int firmNumber, int periodNumber, DateTime dateTime, int customerReferenceId)
    {
        var content = new StringContent(JsonConvert.SerializeObject(CustomerInputOutputChartQuery(firmNumber, periodNumber, dateTime, customerReferenceId)), Encoding.UTF8, "application/json");

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

    private string GetLastFichesByCustomer(int firmNumber, int periodNumber, int customerReferenceId)
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
			From LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE
			left join LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON CLCARD.LOGICALREF = STFICHE.CLIENTREF
			LEFT JOIN L_CAPIWHOUSE AS CAPIWHOUSE on CAPIWHOUSE.NR = STFICHE.SOURCEINDEX AND CAPIWHOUSE.FIRMNR = {firmNumber}
			WHERE  STFICHE.PRODSTAT = 0 AND CLCARD.LOGICALREF={customerReferenceId}
			ORDER BY STFICHE.DATE_ DESC";

        return baseQuery;
    }

    private string CustomerTransaction(int firmNumber, int periodNumber, int ficheReferenceId)
    {
        string baseQuery = $@"Select
            [ReferenceId] = STLINE.LOGICALREF,
            [TransactionDate] = STLINE.DATE_,
            [TransactionTime] = dbo.LG_INTTOTIME(STLINE.FTIME),
     [CustomerReferenceId] = ISNULL(CLCARD.LOGICALREF,0),
	 [CustomerCode] = ISNULL(CLCARD.CODE,''),
     [CustomerName] = ISNULL(CLCARD.DEFINITION_,''),
	 [BaseTransactionReferenceId] = STFICHE.LOGICALREF,
	 [BaseTransactionCode] = STFICHE.FICHENO,
     [TransactionType] = STLINE.TRCODE,
	 [GroupCode] = STFICHE.GRPCODE,
	 [IOType] = STLINE.IOCODE,
	 [ProductReferenceId] =  ITEMS.LOGICALREF,
	 [ProductCode] = ISNULL(ITEMS.CODE,''),
	 [ProductName] = ISNULL(ITEMS.NAME,''),
     [Image] = ISNULL(FIRMDOC.LDATA, ''),
	 [UnitsetReferenceId] = ISNULL( unitset.LOGICALREF,0),
	 [UnitsetCode] = ISNULL( unitset.CODE , ''),
	 [UnitsetName] =  ISNULL (unitset.NAME , ''),
	 [SubUnitsetReferenceId] = ISNULL ( subunitset.LOGICALREF, 0),
	 [SubUnitsetCode] = ISNULL (subunitset.CODE,''),
	 [SubUnitsetName] = ISNULL (subunitset.NAME , ''),
	 [WarehouseName] = capiwhouse.NAME,
     [WarehouseNumber] = capiwhouse.NR,
	 [Quantity] = ISNULL ( STLINE.AMOUNT,0),
	 [Length] =  ISNULL ( STLINE.UINFO4 , 0),
	 [Width] = ISNULL  (STLINE.UINFO5,0),
	 [Height] = ISNULL ( STLINE.UINFO6,0),
	 [Weight] = ISNULL  ( STLINE.UINFO7 , 0),
	 [Volume] = ISNULL  (STLINE.UINFO8 , 0),
	 [Barcode] = ''
from  LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE as STLINE
Left Join LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE on STLINE.STFICHEREF = STFICHE.LOGICALREF
left join LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS on STLINE.STOCKREF = ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC ON FIRMDOC.INFOREF = ITEMS.LOGICALREF AND FIRMDOC.INFOTYP = 20
Left Join LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD as CLCARD ON STLINE.CLIENTREF = CLCARD.LOGICALREF
left join LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS subunitset ON STLINE.UOMREF = subunitset.LOGICALREF
left join LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS unitset ON STLINE.USREF = unitset.LOGICALREF
LEFT JOIN L_CAPIWHOUSE AS capiwhouse ON STLINE.SOURCEINDEX = capiwhouse.NR AND capiwhouse.FIRMNR = {firmNumber}
where STLINE.LINETYPE = 0 AND STFICHE.LOGICALREF = {ficheReferenceId}
order by STLINE.DATE_ desc;";

        return baseQuery;
    }

    /// <summary>
    /// Satınalınan ve Satış İade Yapılan Malzeme Referans sayısı
    /// </summary>
    /// <param name="firmNumber"></param>
    /// <param name="periodNumber"></param>
    /// <param name="supplierReferenceId"></param>
    /// <returns></returns>
    private string GetInputQuantityQuery(int firmNumber, int periodNumber, int customerReferenceId)
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
	STLINE.CLIENTREF = {customerReferenceId}";

        return baseQuery;
    }

    /// <summary>
    /// Satış Yapılan ve Satınalma İade Yapılan Malzeme Referans sayısı
    /// </summary>
    /// <param name="firmNumber"></param>
    /// <param name="periodNumber"></param>
    /// <param name="supplierReferenceId"></param>
    /// <returns></returns>
    private string GetOutputQuantityQuery(int firmNumber, int periodNumber, int customerReferenceId)
    {
        string baseQuery = $@"SELECT
    OutputQuantity = ISNULL(COUNT(DISTINCT STLINE.LOGICALREF), 0)
FROM
    LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK)
LEFT JOIN
    LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE WITH(NOLOCK) ON STLINE.STFICHEREF = STFICHE.LOGICALREF
WHERE
    STFICHE.TRCODE IN(6,7,8) AND
    STLINE.LINETYPE = 0 AND
	STLINE.CLIENTREF = {customerReferenceId}";

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

    private string CustomerInputOutputChartQuery(int firmNumber, int periodNumber, DateTime dateTime, int customerReferenceId)
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
				 STLINE.CLIENTREF = {customerReferenceId}
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
				 STLINE.CLIENTREF = {customerReferenceId}
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
				 STLINE.CLIENTREF = {customerReferenceId}
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
				 STLINE.CLIENTREF = {customerReferenceId}
			),
		0)";
            }
        }

        return baseQuery;
    }

    /// <summary>
    /// Satış Yapılan ve Satınalma İade Yapılan Malzeme Referans sayısı
    /// </summary>
    /// <param name="firmNumber"></param>
    /// <param name="periodNumber"></param>
    /// <param name="supplierReferenceId"></param>
    /// <returns></returns>
    private string GetWaitingProductReferenceCountQuery(int firmNumber, int periodNumber, int customerReferenceId)
    {
        string baseQuery = $@"SELECT

[WaitingProductReferenceCount] = ISNULL(COUNT(DISTINCT STOCKREF),0) 
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE 
WHERE CLIENTREF = {customerReferenceId} AND TRCODE = 1 AND (AMOUNT - SHIPPEDAMOUNT) > 0 AND CLOSED = 0";

        return baseQuery;
    }

}