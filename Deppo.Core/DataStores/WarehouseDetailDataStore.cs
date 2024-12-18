using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DataStores
{
    public class WarehouseDetailDataStore : IWarehouseDetailService
    {
        private string postUrl = "/gateway/customQuery/CustomQuery";

        public async Task<DataResult<dynamic>> GetInputQuantity(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, string externalDb = "")
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetInputQuantityQuery(firmNumber, periodNumber, warehouseNumber, externalDb)), Encoding.UTF8, "application/json");

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
                        var result = JsonConvert.DeserializeObject<DataResult<dynamic>>(data);

                        dataResult.Data = result?.Data;
                        dataResult.IsSuccess = true;
                        dataResult.Message = "empty";
                        return dataResult;
                    }
                }
                else
                {
                    var result = JsonConvert.DeserializeObject<DataResult<dynamic>>(data);

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

        public async Task<DataResult<dynamic>> GetOutputQuantity(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, string externalDb = "")
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetOutputQuantityQuery(firmNumber, periodNumber, warehouseNumber, externalDb)), Encoding.UTF8, "application/json");

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
                        var result = JsonConvert.DeserializeObject<DataResult<dynamic>>(data);

                        dataResult.Data = result?.Data;
                        dataResult.IsSuccess = true;
                        dataResult.Message = "empty";
                        return dataResult;
                    }
                }
                else
                {
                    var result = JsonConvert.DeserializeObject<DataResult<dynamic>>(data);

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

        public async Task<DataResult<IEnumerable<dynamic>>> GetLastFiches(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, string externalDb = "")
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetLastFichesQuery(firmNumber, periodNumber, warehouseNumber, externalDb)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<IEnumerable<dynamic>>> GetLastTransaction(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferendeId, int warehouseNumber, string externalDb = "")
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetLastTransactionQuery(firmNumber, periodNumber, ficheReferendeId, warehouseNumber, externalDb)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<IEnumerable<dynamic>>> ProductInputOutputReferences(HttpClient httpClient, int firmNumber, int periodNumber, DateTime dateTime, int warehouseNumber)
        {
            var content = new StringContent(JsonConvert.SerializeObject(ProductInputOutputReferencesQuery(firmNumber, periodNumber, dateTime, warehouseNumber)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<dynamic>> WarehouseReferenceCount(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, string externalDb = "")
        {
            var content = new StringContent(JsonConvert.SerializeObject(WarehouseReferenceCountQuery(firmNumber, periodNumber, warehouseNumber, externalDb)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<dynamic>> WarehouseLastTransactionDate(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, string externalDb = "")
        {
            var content = new StringContent(JsonConvert.SerializeObject(WarehouseLastTransactionDateQuery(firmNumber, periodNumber, warehouseNumber, externalDb)), Encoding.UTF8, "application/json");

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


        private string GetLastFichesQuery(int firmNumber, int periodNumber, int warehouseNumber, string externalDb = "")
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
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON CLCARD.LOGICALREF = STFICHE.CLIENTREF
			LEFT JOIN {externalDb}L_CAPIWHOUSE AS CAPIWHOUSE ON STFICHE.SOURCEINDEX = CAPIWHOUSE.NR AND CAPIWHOUSE.FIRMNR = {firmNumber}
			WHERE STFICHE.SOURCEINDEX={warehouseNumber}  AND STFICHE.PRODSTAT = 0
			ORDER BY STFICHE.DATE_ DESC ";

            return baseQuery;
        }

        private string GetLastTransactionQuery(int firmNumber, int periodNumber, int ficheReferenceId, int warehouseNumber, string externalDb = "")
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
        [Image] = FIRMDOC.LDATA,
        [WarehouseNumber] = CAPIWHOUSE.NR,
        [WarehouseName] = CAPIWHOUSE.NAME

        FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE ON STLINE.STFICHEREF = STFICHE.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON STLINE.STOCKREF = ITEMS.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON STLINE.CLIENTREF = CLCARD.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON STLINE.UOMREF = SUBUNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC ON FIRMDOC.INFOREF = ITEMS.LOGICALREF AND FIRMDOC.INFOTYP = 20
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON STLINE.USREF = UNITSET.LOGICALREF
		LEFT JOIN {externalDb}L_CAPIWHOUSE AS CAPIWHOUSE ON STLINE.SOURCEINDEX = CAPIWHOUSE.NR AND CAPIWHOUSE.FIRMNR = {firmNumber}
		WHERE  STFICHE.LOGICALREF = {ficheReferenceId} AND  STLINE.SOURCEINDEX={warehouseNumber} AND  STFICHE.PRODSTAT = 0 AND STLINE.LPRODSTAT = 0
		ORDER BY STLINE.DATE_ DESC";

            return baseQuery;
        }

        private string GetInputQuantityQuery(int firmNumber, int periodNumber, int warehouseNumber, string externalDb = "")
        {
            var baseQuery = @$"SELECT
[InputQuantity] = ISNULL(COUNT(DISTINCT STLINE.STOCKREF),0)

FROM
    LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
LEFT JOIN
    {externalDb}L_CAPIWHOUSE AS CAPIWHOUSE
    ON STLINE.SOURCEINDEX = CAPIWHOUSE.NR
    AND CAPIWHOUSE.FIRMNR = {firmNumber}
WHERE STLINE.SOURCEINDEX = {warehouseNumber} AND STLINE.IOCODE IN (1, 2);";

            return baseQuery;
        }

        private string GetOutputQuantityQuery(int firmNumber, int periodNumber, int warehouseNumber, string externalDb = "")
        {
            var baseQuery = @$"SELECT
[OutputQuantity] = ISNULL(COUNT(DISTINCT STLINE.STOCKREF),0)

FROM
    LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
LEFT JOIN
    {externalDb}L_CAPIWHOUSE AS CAPIWHOUSE
    ON STLINE.SOURCEINDEX = CAPIWHOUSE.NR
    AND CAPIWHOUSE.FIRMNR ={firmNumber}
WHERE STLINE.SOURCEINDEX ={warehouseNumber} AND STLINE.IOCODE IN (3, 4)";

            return baseQuery;
        }

        private string ProductInputOutputReferencesQuery(int firmNumber, int periodNumber, DateTime dateTime, int warehouseNumber)
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
[InputQuantity] = ISNULL((SELECT COUNT(DISTINCT STLINE.STOCKREF) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK) WHERE IOCODE IN(1,2) AND LINETYPE = 0 AND YEAR(STLINE.DATE_) = {xDate.Year} AND MONTH(STLINE.DATE_) = {xDate.Month} AND DAY(STLINE.DATE_) = {xDate.Day} AND  STLINE.SOURCEINDEX={warehouseNumber}),0),
[OutputQuantity] = ISNULL((SELECT COUNT(DISTINCT STLINE.STOCKREF) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK) WHERE TRCODE IN(3,4) AND LINETYPE = 0 AND YEAR(STLINE.DATE_) = {xDate.Year} AND DAY(STLINE.DATE_) = {xDate.Month} AND DAY(STLINE.DATE_) = {xDate.Day} AND  STLINE.SOURCEINDEX={warehouseNumber}),0)
UNION All ";
                }
                else
                {
                    baseQuery += $@"SELECT
[Argument] = '{xDate.ToString("dddd")}',
[ArgumentDay] = {xDate.Day.ToString().PadLeft(2, '0')},
[InputQuantity] = ISNULL((SELECT SUM(STLINE.AMOUNT) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK) WHERE IOCODE IN(1,2) AND LINETYPE = 0 AND YEAR(STLINE.DATE_) = {xDate.Year} AND MONTH(STLINE.DATE_) = {xDate.Month} AND DAY(STLINE.DATE_) = {xDate.Day} AND  STLINE.SOURCEINDEX={warehouseNumber}),0),
[OutputQuantity] = ISNULL((SELECT SUM(STLINE.AMOUNT) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK) WHERE TRCODE IN(3,4) AND LINETYPE = 0 AND YEAR(STLINE.DATE_) = {xDate.Year} AND DAY(STLINE.DATE_) = {xDate.Month} AND DAY(STLINE.DATE_) = {xDate.Day} AND  STLINE.SOURCEINDEX={warehouseNumber}),0)";
                }
            }

            return baseQuery;
        }

        private string WarehouseReferenceCountQuery(int firmNumber, int periodNumber, int warehouseNumber, string externalDb = "")
        {
            string baseQuery = $@"SELECT
    [WarehouseReferenceCount] = ISNULL(COUNT(DISTINCT STINVTOT.STOCKREF), 0)

FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON STINVTOT.STOCKREF = ITEMS.LOGICALREF
LEFT JOIN {externalDb}L_CAPIWHOUSE AS WHOUSE WITH(NOLOCK) ON STINVTOT.INVENNO = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_INVDEF AS INVDEF WITH(NOLOCK) ON STINVTOT.INVENNO = INVDEF.INVENNO AND STINVTOT.STOCKREF = INVDEF.ITEMREF AND INVDEF.VARIANTREF = 0
WHERE INVDEF.OUTCTRL <> 2 AND STINVTOT.INVENNO ={warehouseNumber}";

            return baseQuery;
        }
        private string WarehouseLastTransactionDateQuery(int firmNumber, int periodNumber, int warehouseNumber, string externalDb = "")
        {
            string baseQuery = $@"SELECT TOP 1
    STLINE.LOGICALREF AS [ReferenceId],
    STLINE.DATE_ AS [LastTransactionDate],
    dbo.LG_INTTOTIME(STFICHE.FTIME) AS [LastTransactionTime],
    STFICHE.LOGICALREF AS [BaseTransactionReferenceId],
    STFICHE.FICHENO AS [BaseTransactionCode],
    CAPIWHOUSE.NR AS [WarehouseNumber],
    CAPIWHOUSE.NAME AS [WarehouseName]
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE ON STLINE.STFICHEREF = STFICHE.LOGICALREF
LEFT JOIN {externalDb}L_CAPIWHOUSE AS CAPIWHOUSE ON STLINE.SOURCEINDEX = CAPIWHOUSE.NR AND CAPIWHOUSE.FIRMNR = {firmNumber}
WHERE STLINE.SOURCEINDEX ={warehouseNumber}
    AND STFICHE.PRODSTAT = 0 
    AND STLINE.LPRODSTAT = 0
ORDER BY STLINE.DATE_ DESC;";

            return baseQuery;
        }

    }
}