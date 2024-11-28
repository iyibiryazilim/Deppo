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
    public class PurchasePanelDataStore : IPurchasePanelService
    {
        private string postUrl = "/gateway/customQuery/CustomQuery";

        public async Task<DataResult<IEnumerable<dynamic>>> SupplierTransaction(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId)
        {
            var content = new StringContent(JsonConvert.SerializeObject(SupplierTransaction(firmNumber, periodNumber, ficheReferenceId)), Encoding.UTF8, "application/json");

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

        //Yapıldı
        public async Task<DataResult<IEnumerable<dynamic>>> GetLastTransactionBySupplier(HttpClient httpClient, int firmNumber, int periodNumber)
        {
            var content = new StringContent(JsonConvert.SerializeObject(QueryBySupplier(firmNumber, periodNumber)), Encoding.UTF8, "application/json");

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

        //Sevk Edilen Toplamı
        public async Task<DataResult<dynamic>> ShippedOrderCount(HttpClient httpClient, int firmNumber, int periodNumber)
        {
            var content = new StringContent(JsonConvert.SerializeObject(ShippedOrderCount(firmNumber, periodNumber)), Encoding.UTF8, "application/json");

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

        //Toplamların Toplamı
        public async Task<DataResult<dynamic>> TotalOrderCount(HttpClient httpClient, int firmNumber, int periodNumber)
        {
            var content = new StringContent(JsonConvert.SerializeObject(TotalOrderCount(firmNumber, periodNumber)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<IEnumerable<dynamic>>> GetLastFiche(HttpClient httpClient, int firmNumber, int periodNumber)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetLastFiche(firmNumber, periodNumber)), Encoding.UTF8, "application/json");

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



        public async Task<DataResult<IEnumerable<dynamic>>> GetWaitingProducts(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetWaitingProducts(firmNumber, periodNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<IEnumerable<dynamic>>> GetWaitingOrders(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetWaitingOrders(firmNumber, periodNumber, productReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<IEnumerable<dynamic>>> GetReceivedProducts(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetReceivedProducts(firmNumber, periodNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<IEnumerable<dynamic>>> GetReceivedOrders(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetReceivedOrders(firmNumber, periodNumber, productReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<IEnumerable<dynamic>>> GetAllFiche(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetAllFiche(firmNumber, periodNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<IEnumerable<dynamic>>> GetFicheTransactions(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId, string search = "", int skip = 0, int take = 20)
        {
            var content = new StringContent(JsonConvert.SerializeObject(FicheTransactions(firmNumber, periodNumber, ficheReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<dynamic>> WaitingProductCount(HttpClient httpClient, int firmNumber, int periodNumber)
        {
            var content = new StringContent(JsonConvert.SerializeObject(WaitingProductQuery(firmNumber, periodNumber)), Encoding.UTF8, "application/json");

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


        private string GetLastFiche(int firmNumber, int periodNumber)
        {
            string baseQuery = $@"Select TOP 5
            [ReferenceId] = STFICHE.LOGICALREF,
			[FicheType] = STFICHE.TRCODE,
			[FicheNumber] = STFICHE.FICHENO,
            [FicheDate] = STFICHE.DATE_,
            [FicheTime] = dbo.LG_INTTOTIME(STFICHE.FTIME),
			[DocumentNumber] =ISNULL (STFICHE.DOCODE , ''),
			[DocumentDate] =  STFICHE.DOCDATE,
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
			WHERE STFICHE.TRCODE in (1,5,6)
			ORDER BY STFICHE.DATE_ DESC;";

            return baseQuery;
        }

        private string QueryBySupplier(int firmNumber, int periodNumber)
        {
            string baseQuery = $@"SELECT TOP 5
    [ReferenceId] = CLCARD.LOGICALREF,
    [Code] = CLCARD.CODE,
    [Title] = CLCARD.DEFINITION_,
    [IsPersonal] =
        CASE
            WHEN CLCARD.ISPERSCOMP = 0 THEN 0
            ELSE 1
        END,
    [Name] = CLCARD.DEFINITION_,
    [Email] = CLCARD.EMAILADDR,
    [Telephone] = CLCARD.TELNRS1 + ' ' + CLCARD.TELNRS2,
    [Address] = CLCARD.ADDR1,
    [City] = CLCARD.CITY,
    [Country] = CLCARD.COUNTRY,
    [PostalCode] = CLCARD.POSTCODE,
    [TaxOffice] = CLCARD.TAXOFFICE,
    [TaxNumber] = CLCARD.TAXNR,
    [IsActive] =
        CASE
            WHEN CLCARD.ACTIVE = 0 THEN 0
            ELSE 1
        END
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
Left Join LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE on STLINE.STFICHEREF = STFICHE.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON STLINE.CLIENTREF = CLCARD.LOGICALREF
WHERE CLCARD.CODE LIKE '32%'
  AND CLCARD.CODE <> 'ÿ'
  AND CLCARD.ACTIVE = 0
  AND STFICHE.GRPCODE = 1
GROUP BY CLCARD.LOGICALREF, CLCARD.CODE, CLCARD.DEFINITION_, CLCARD.ISPERSCOMP,
         CLCARD.EMAILADDR, CLCARD.TELNRS1, CLCARD.TELNRS2, CLCARD.ADDR1,
         CLCARD.CITY, CLCARD.COUNTRY, CLCARD.POSTCODE, CLCARD.TAXOFFICE,
         CLCARD.TAXNR, CLCARD.ACTIVE
ORDER BY MAX(STLINE.DATE_) DESC;";

            return baseQuery;
        }

        private string ShippedOrderCount(int firmNumber, int periodNumber)
        {
            string baseQuery = $@"SELECT
	ISNULL(COUNT(Distinct STOCKREF),0) AS ShippedQuantityTotal
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE 
WHERE TRCODE IN(1)";
            return baseQuery;
        }

        private string TotalOrderCount(int firmNumber, int periodNumber)
        {
            string baseQuery = $@"SELECT
	ISNULL(COUNT(Distinct ORFLINE.STOCKREF),0) AS AmountTotal
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE
WHERE ORFLINE.TRCODE = 2";
            return baseQuery;
        }

        private string SupplierTransaction(int firmNumber, int periodNumber, int ficheReferenceId)
        {
            string baseQuery = $@"Select
            [ReferenceId] = STLINE.LOGICALREF,
            [TransactionDate] = STLINE.DATE_,
            [TransactionTime] = dbo.LG_INTTOTIME(STLINE.FTIME),
     [SupplierReferenceId] = CLCARD.LOGICALREF,
	 [SupplierCode] = CLCARD.CODE,
     [SupplierName] = CLCARD.DEFINITION_,
	 [BaseTransactionReferenceId] = STFICHE.LOGICALREF,
	 [BaseTransactionCode] = STFICHE.FICHENO,
     [TransactionType] = STLINE.TRCODE,
	 [GroupCode] = STFICHE.GRPCODE,
	 [IOType] = STLINE.IOCODE,
	 [ProductReferenceId] =  ITEMS.LOGICALREF,
	 [ProductCode] = ISNULL(ITEMS.CODE,''),
	 [ProductName] = ISNULL(ITEMS.NAME,''),
	 [UnitsetReferenceId] = ISNULL( unitset.LOGICALREF,0),
	 [UnitsetCode] = ISNULL( unitset.CODE , ''),
	 [UnitsetName] =  ISNULL (unitset.NAME , ''),
	 [SubUnitsetReferenceId] = ISNULL ( subunitset.LOGICALREF, 0),
	 [SubUnitsetCode] = ISNULL (subunitset.CODE,''),
	 [SubUnitsetName] = ISNULL (subunitset.NAME , ''),
     [Image] = FIRMDOC.LDATA,
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
where STFICHE.TRCODE IN(1,5, 6) AND STLINE.LINETYPE = 0 AND STLINE.STFICHEREF = {ficheReferenceId}
order by STLINE.DATE_ desc;";

            return baseQuery;
        }

        private string GetWaitingProducts(int firmNumber, int periodNumber, string search, int skip, int take)
        {
            string baseQuery = $@"WITH BaseQuery AS (
    SELECT
            [ProductReferenceId] = ORFLINE.STOCKREF,
            [ProductCode] = ISNULL(ITEMS.CODE, ''),
            [ProductName] = ISNULL(ITEMS.NAME, ''),
            [UnitsetReferenceId] = UNITSET.LOGICALREF,
            [UnitsetCode] = UNITSET.CODE,
            [UnitsetName] = UNITSET.NAME,
            [SubUnitsetReferenceId] = SUBUNITSET.LOGICALREF,
            [SubUnitsetCode] = SUBUNITSET.CODE,
            [SubUnitsetName] = SUBUNITSET.NAME,
            [WaitingQuantity] = SUM(ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT),
            [IsVariant] = ITEMS.CANCONFIGURE,
            [VariantReferenceId] = ORFLINE.VARIANTREF,
            [VariantCode] = ISNULL(VARIANT.CODE, ''),
            [VariantName] = ISNULL(VARIANT.NAME, ''),
            [LocTracking] = ITEMS.LOCTRACKING,
            [TrackingType] = ITEMS.TRACKTYPE

    FROM
      LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON ORFLINE.STOCKREF = ITEMS.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT ON ORFLINE.VARIANTREF = VARIANT.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON ORFLINE.UOMREF = SUBUNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON ORFLINE.USREF = UNITSET.LOGICALREF

   WHERE
        ORFLINE.CLOSED = 0
  AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0
  AND ORFLINE.TRCODE = 2
  AND ITEMS.UNITSETREF <> 0
    GROUP BY
        ORFLINE.STOCKREF,
            ITEMS.CODE,
            ITEMS.NAME,
            UNITSET.LOGICALREF,
            UNITSET.CODE,
            UNITSET.NAME,
            SUBUNITSET.LOGICALREF,
            SUBUNITSET.CODE,
            SUBUNITSET.NAME,
            ITEMS.CANCONFIGURE,
            ORFLINE.VARIANTREF,
            VARIANT.CODE,
            VARIANT.NAME,
            ITEMS.LOCTRACKING,
            ITEMS.TRACKTYPE
    HAVING
       ITEMS.CODE <> 'ÿ'

)
SELECT
    BQ.ProductReferenceId,
    BQ.ProductCode,
    BQ.ProductName,
	BQ.UnitsetReferenceId,
	BQ.UnitsetCode,
	BQ.UnitsetName,
	BQ.SubUnitsetReferenceId,
    BQ.SubUnitsetCode,
    BQ.SubUnitsetName,
	BQ.WaitingQuantity,
	BQ.IsVariant,
	BQ.VariantReferenceId,
	BQ.VariantCode,
	BQ.VariantName,
	BQ.TrackingType,
    BQ.TrackingType,
    FIRMDOC.LDATA AS Image
FROM
       BaseQuery AS BQ
    LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC ON FIRMDOC.INFOREF = BQ.ProductReferenceId AND FIRMDOC.INFOTYP = 20
ORDER BY BQ.WaitingQuantity DESC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

            return baseQuery;
        }

        private string GetWaitingOrders(int firmNumber, int periodNumber, int productReferenceId, string search, int skip, int take)
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
        LEFT JOIN L_CAPIWHOUSE AS WHOUSE ON ORFLINE.SOURCEINDEX = WHOUSE.NR AND WHOUSE.FIRMNR ={firmNumber}
		WHERE ORFLINE.CLOSED = 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 2
		AND ITEMS.UNITSETREF <> 0 AND ITEMS.LOGICALREF = {productReferenceId}
";
            if (!string.IsNullOrEmpty(search))
                baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";

            baseQuery += $@" ORDER BY ITEMS.CODE DESC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

            return baseQuery;
        }

        private string GetReceivedProducts(int firmNumber, int periodNumber, string search, int skip, int take)
        {
            string baseQuery = $@"
    WITH BaseQuery AS (
    SELECT
       [ProductReferenceId] = ITEMS.LOGICALREF,
            [ProductCode] = ISNULL(ITEMS.CODE, ''),
            [ProductName] = ISNULL(ITEMS.NAME, ''),
            [UnitsetReferenceId] = ISNULL(UNITSET.LOGICALREF, 0),
            [UnitsetCode] = ISNULL(UNITSET.CODE, ''),
            [UnitsetName] = ISNULL(UNITSET.NAME, ''),
            [SubUnitsetReferenceId] = ISNULL(SUBUNITSET.LOGICALREF, 0),
            [SubUnitsetCode] = ISNULL(SUBUNITSET.CODE, ''),
            [SubUnitsetName] = ISNULL(SUBUNITSET.NAME, ''),
            [Quantity] = ISNULL(SUM(STLINE.AMOUNT), 0),
            [IsVariant] = ITEMS.CANCONFIGURE,
            [LocTracking] = ITEMS.LOCTRACKING,
            [TrackingType] = ITEMS.TRACKTYPE

    FROM
      LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON STLINE.STOCKREF = ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON STLINE.UOMREF = SUBUNITSET.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON STLINE.USREF = UNITSET.LOGICALREF
    WHERE
     STLINE.TRCODE IN (1)
  AND STLINE.LINETYPE = 0

    GROUP BY
       ITEMS.LOGICALREF,
            ITEMS.CODE,
            ITEMS.NAME,
            UNITSET.LOGICALREF,
            UNITSET.CODE,
            UNITSET.NAME,
            SUBUNITSET.LOGICALREF,
            SUBUNITSET.CODE,
            ITEMS.CANCONFIGURE,
            ITEMS.LOCTRACKING,
            ITEMS.TRACKTYPE,
            SUBUNITSET.NAME
      HAVING
       ITEMS.CODE <> 'ÿ'

)
SELECT
    BQ.ProductReferenceId,
    BQ.ProductCode,
    BQ.ProductName,
	BQ.UnitsetReferenceId,
	BQ.UnitsetCode,
	BQ.UnitsetName,
	BQ.SubUnitsetReferenceId,
    BQ.SubUnitsetCode,
    BQ.SubUnitsetName,
	BQ.Quantity,
	BQ.IsVariant,
	BQ.LocTracking,
    BQ.TrackingType,
    FIRMDOC.LDATA AS Image
FROM
       BaseQuery AS BQ
    LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC ON FIRMDOC.INFOREF = BQ.ProductReferenceId AND FIRMDOC.INFOTYP = 20
ORDER BY BQ.Quantity DESC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

            return baseQuery;
        }

        private string GetReceivedOrders(int firmNumber, int periodNumber, int productReferenceId, string search, int skip, int take)
        {
            string baseQuery = $@"Select
            [ReferenceId] = STLINE.LOGICALREF,
            [TransactionDate] = STLINE.DATE_,
            [TransactionTime] = dbo.LG_INTTOTIME(STLINE.FTIME),
     [CustomerReferenceId] = CLCARD.LOGICALREF,
	 [CustomerCode] = CLCARD.CODE,
     [CustomerName] = CLCARD.DEFINITION_,
	 [BaseTransactionReferenceId] = STFICHE.LOGICALREF,
	 [BaseTransactionCode] = STFICHE.FICHENO,
     [TransactionType] = STLINE.TRCODE,
	 [GroupCode] = STFICHE.GRPCODE,
	 [IOType] = STLINE.IOCODE,
	 [ProductReferenceId] =  ITEMS.LOGICALREF,
	 [ProductCode] = ISNULL(ITEMS.CODE,''),
	 [ProductName] = ISNULL(ITEMS.NAME,''),
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
Left Join LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD as CLCARD ON STLINE.CLIENTREF = CLCARD.LOGICALREF
left join LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS subunitset ON STLINE.UOMREF = subunitset.LOGICALREF
left join LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS unitset ON STLINE.USREF = unitset.LOGICALREF
LEFT JOIN L_CAPIWHOUSE AS capiwhouse ON STLINE.SOURCEINDEX = capiwhouse.NR AND capiwhouse.FIRMNR = {firmNumber}
where STFICHE.TRCODE IN(1) AND STLINE.LINETYPE = 0 AND ITEMS.LOGICALREF = {productReferenceId}
";
            if (!string.IsNullOrEmpty(search))
                baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";

            baseQuery += $@" ORDER BY ITEMS.CODE DESC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

            return baseQuery;
        }

        private string GetAllFiche(int firmNumber, int periodNumber, string search, int skip, int take)
        {
            string baseQuery = $@"Select
            [ReferenceId] = STFICHE.LOGICALREF,
			[FicheType] = STFICHE.TRCODE,
			[FicheNumber] = STFICHE.FICHENO,
            [FicheDate] = STFICHE.DATE_,
            [FicheTime] = dbo.LG_INTTOTIME(STFICHE.FTIME),
			[DocumentNumber] =ISNULL (STFICHE.DOCODE , ''),
			[DocumentDate] =  STFICHE.DOCDATE,
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
			WHERE STFICHE.TRCODE in (1,5,6)
			ORDER BY STFICHE.DATE_ DESC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

            return baseQuery;
        }

        private string FicheTransactions(int firmNumber, int periodNumber, int ficheReferenceId, string search, int skip, int take)
        {
            string baseQuery = $@"Select
            [ReferenceId] = STLINE.LOGICALREF,
            [TransactionDate] = STLINE.DATE_,
            [TransactionTime] = dbo.LG_INTTOTIME(STLINE.FTIME),
     [CustomerReferenceId] = CLCARD.LOGICALREF,
	 [CustomerCode] = CLCARD.CODE,
     [CustomerName] = CLCARD.DEFINITION_,
	 [BaseTransactionReferenceId] = STFICHE.LOGICALREF,
	 [BaseTransactionCode] = STFICHE.FICHENO,
     [TransactionType] = STLINE.TRCODE,
	 [GroupCode] = STFICHE.GRPCODE,
	 [IOType] = STLINE.IOCODE,
	 [ProductReferenceId] =  ITEMS.LOGICALREF,
	 [ProductCode] = ISNULL(ITEMS.CODE,''),
	 [ProductName] = ISNULL(ITEMS.NAME,''),
     [Image] = FIRMDOC.LDATA,
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
where STFICHE.TRCODE IN(1) AND STLINE.LINETYPE = 0 AND STFICHE.LOGICALREF = {ficheReferenceId}
order by STLINE.DATE_ desc OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

            return baseQuery;
        }

        private string WaitingProductQuery(int firmNumber, int periodNumber)
        {
            string baseQuery = $@"
SELECT COUNT(DISTINCT ORFLINE.STOCKREF) AS WaitingOrderCount
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON ORFLINE.STOCKREF = ITEMS.LOGICALREF
WHERE ORFLINE.CLOSED = 0
    AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0
    AND ORFLINE.TRCODE = 2
    AND ITEMS.UNITSETREF <> 0
    ";
            return baseQuery;
        }


    }
}