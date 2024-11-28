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
    public class SalesAnalysisDataStore : ISalesAnalysisService
    {
        private string postUrl = "/gateway/customQuery/CustomQuery";

        //Termin Tarihi Geçen Müşteri Sayısı
        public async Task<DataResult<dynamic>> DueDatePassedCustomersCount(HttpClient httpClient, int firmNumber, int periodNumber)
        {
            var content = new StringContent(JsonConvert.SerializeObject(DueDatePassedCustomersCount(firmNumber, periodNumber)), Encoding.UTF8, "application/json");

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
        //Termin Tarihi Geçen Ürün Sayısı
        public async Task<DataResult<dynamic>> DueDatePassedProductsCount(HttpClient httpClient, int firmNumber, int periodNumber)
        {
            var content = new StringContent(JsonConvert.SerializeObject(DueDatePassedProductsCount(firmNumber, periodNumber)), Encoding.UTF8, "application/json");

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
        //İade Edilen Ürün Referans Sayısı
        public async Task<DataResult<dynamic>> ReturnProductReferenceCount(HttpClient httpClient, int firmNumber, int periodNumber)
        {
            var content = new StringContent(JsonConvert.SerializeObject(ReturnProductReferenceCount(firmNumber, periodNumber)), Encoding.UTF8, "application/json");

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
        //Satılan Malzeme Referans Sayısı
        public async Task<DataResult<dynamic>> SoldProductReferenceCount(HttpClient httpClient, int firmNumber, int periodNumber)
        {
            var content = new StringContent(JsonConvert.SerializeObject(SoldProductReferenceCount(firmNumber, periodNumber)), Encoding.UTF8, "application/json");

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

        //Satılan son 5  Müşteriler
        public async Task<DataResult<IEnumerable<dynamic>>> LastCustomers(HttpClient httpClient, int firmNumber, int periodNumber)
        {
            var content = new StringContent(JsonConvert.SerializeObject(LastCustomers(firmNumber, periodNumber)), Encoding.UTF8, "application/json");

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
        //Son 5 malzeme 
        public async Task<DataResult<IEnumerable<dynamic>>> LastProducts(HttpClient httpClient, int firmNumber, int periodNumber)
        {
            var content = new StringContent(JsonConvert.SerializeObject(LastProducts(firmNumber, periodNumber)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<IEnumerable<dynamic>>> SalesProductReferenceAnalysis(HttpClient httpClient, int firmNumber, int periodNumber, DateTime dateTime)
        {
            var content = new StringContent(JsonConvert.SerializeObject(SalesProductReferenceAnalysisQuery(firmNumber, periodNumber, dateTime)), Encoding.UTF8, "application/json");

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

        private string SalesProductReferenceAnalysisQuery(int firmNumber, int periodNumber, DateTime dateTime)
        {
            string baseQuery = $@"";
            DateTime xDate = dateTime;
            for (int i = 1; i < 7; i++)
            {
                if (i != 1)
                    xDate = xDate.AddMonths(-1);

if(i != 6)
                baseQuery += $@"
SELECT 
[Argument] = '{xDate.ToString("MMMM")}',
[ArgumentMonth] = {xDate.Month.ToString().PadLeft(2, '0')},
[SalesReferenceCount] = ISNULL((SELECT COUNT(DISTINCT STLINE.STOCKREF) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK) WHERE TRCODE IN(7,8) AND LINETYPE = 0 AND YEAR(STLINE.DATE_) = {xDate.Year} AND MONTH(STLINE.DATE_) = {xDate.Month}),0),
[ReturnReferenceCount] = ISNULL((SELECT COUNT(DISTINCT STLINE.STOCKREF) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK) WHERE TRCODE IN(2,3) AND LINETYPE = 0 AND YEAR(STLINE.DATE_) = {xDate.Year} AND MONTH(STLINE.DATE_) = {xDate.Month}),0)
UNION All ";
else{
                    baseQuery += $@"
SELECT 
[Argument] = '{xDate.ToString("MMMM")}',
[ArgumentMonth] = {xDate.Month.ToString().PadLeft(2, '0')},
[SalesReferenceCount] = ISNULL((SELECT COUNT(DISTINCT STLINE.STOCKREF) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK) WHERE TRCODE IN(7,8) AND LINETYPE = 0 AND YEAR(STLINE.DATE_) = {xDate.Year} AND MONTH(STLINE.DATE_) = {xDate.Month}),0),
[ReturnReferenceCount] = ISNULL((SELECT COUNT(DISTINCT STLINE.STOCKREF) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK) WHERE TRCODE IN(2,3) AND LINETYPE = 0 AND YEAR(STLINE.DATE_) = {xDate.Year} AND MONTH(STLINE.DATE_) = {xDate.Month}),0)";
}
            }

            return baseQuery;
        }

        private string DueDatePassedCustomersCount(int firmNumber, int periodNumber)
        {
            string baseQuery = $@"select ISNULL(COUNT(DISTINCT CLIENTREF),0) 
            AS DueDatePassedCustomersCount from LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE as ORFLINE
            WHERE ORFLINE.DUEDATE <= CAST(GETDATE() AS DATE) and  ORFLINE.TRCODE = 1 AND ORFLINE.CLOSED = 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.LINETYPE = 0;";

            return baseQuery;
        }
        private string DueDatePassedProductsCount(int firmNumber, int periodNumber)
        {
            string baseQuery = $@"select  ISNULL(COUNT(DISTINCT STOCKREF),0) 
            AS DueDatePassedProductsCount from LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE as ORFLINE
            WHERE ORFLINE.DUEDATE <= CAST(GETDATE() AS DATE) and  ORFLINE.TRCODE = 1 AND ORFLINE.CLOSED = 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.LINETYPE = 0;";

            return baseQuery;
        }
        private string ReturnProductReferenceCount(int firmNumber, int periodNumber)
        {
            string baseQuery = $@"SELECT ISNULL(COUNT(DISTINCT STOCKREF),0) AS ReturnProductReferenceCount
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE
WHERE TRCODE in (2,3) AND LINETYPE = 0;";

            return baseQuery;
        }
        private string SoldProductReferenceCount(int firmNumber, int periodNumber)
        {
            string baseQuery = $@"SELECT ISNULL(COUNT(DISTINCT STOCKREF),0) AS SoldProductReferenceCount
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE
WHERE TRCODE in (7 , 8) AND LINETYPE = 0;
";
            return baseQuery;
        }
        private string LastCustomers(int firmNumber, int periodNumber)
        {
            string baseQuery = $@"SELECT TOP(5)
    [ReferenceId] = CLCARD.LOGICALREF,
    [Code] = CLCARD.CODE,
    [Title] = ISNULL ( CLCARD.DEFINITION_ ,'') ,
    [IsPersonal] =
        CASE
            WHEN CLCARD.ISPERSCOMP = 0 THEN 0
            ELSE 1
        END,
    [Name] = ISNULL (CLCARD.DEFINITION_,''),
    [Email] = ISNULL(CLCARD.EMAILADDR,''),
	[Telephone] = ISNULL(CLCARD.TELNRS1, '') + ' ' + ISNULL(CLCARD.TELNRS2, ''),    
	[Address] = ISNULL ( CLCARD.ADDR1 , ''),
    [City] = ISNULL ( CLCARD.CITY, ''),
    [Country] = ISNULL ( CLCARD.COUNTRY , '' ),
    [PostalCode] = ISNULL (CLCARD.POSTCODE, ''),
    [TaxOffice] = ISNULL ( CLCARD.TAXOFFICE, ''),
    [TaxNumber] = ISNULL (CLCARD.TAXNR, ''),
    [IsActive] =
        CASE
            WHEN CLCARD.ACTIVE = 0 THEN 0
            ELSE 1
        END
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON STLINE.CLIENTREF = CLCARD.LOGICALREF
WHERE  CLCARD.CODE <> 'ÿ' 
  AND CLCARD.ACTIVE = 0
  AND STLINE.TRCODE in (7,8,2,3)
  
GROUP BY CLCARD.LOGICALREF, CLCARD.CODE, CLCARD.DEFINITION_, CLCARD.ISPERSCOMP, 
         CLCARD.EMAILADDR, CLCARD.TELNRS1, CLCARD.TELNRS2, CLCARD.ADDR1, 
         CLCARD.CITY, CLCARD.COUNTRY, CLCARD.POSTCODE, CLCARD.TAXOFFICE, 
         CLCARD.TAXNR, CLCARD.ACTIVE
ORDER BY MAX(STLINE.DATE_) DESC ;
";
            return baseQuery;
        }
        private string LastProducts(int firmNumber, int periodNumber)
        {
            string baseQuery = $@"SELECT TOP(5)
    [ReferenceId] = ITEMS.LOGICALREF,
    [Code] = ISNULL  (ITEMS.CODE,''),
    [Name] = ISNULL(ITEMS.NAME,''),
    [VatRate] = ISNULL (ITEMS.VAT, 0),
    [UnitsetReferenceId] = MAX(UNITSETF.LOGICALREF),
    [UnitsetCode] = MAX(UNITSETF.CODE),
    [UnitsetName] = MAX(UNITSETF.NAME),
    [SubUnitsetReferenceId] = MAX(UNITSETL.LOGICALREF),
    [SubUnitsetCode] = MAX(UNITSETL.CODE),
    [SubUnitsetName] = MAX(UNITSETL.NAME),
    [IsVariant] = ITEMS.CANCONFIGURE,
    [TrackingType] = ITEMS.TRACKTYPE,
    [LocTracking] = ITEMS.LOCTRACKING,
    [GroupCode] = ISNULL(MAX(ITEMS.STGRPCODE), ''),
    [BrandReferenceId] = ISNULL(MAX(BRAND.LOGICALREF), 0),
    [BrandCode] = ISNULL(MAX(BRAND.CODE), ''),
    [BrandName] = ISNULL(MAX(BRAND.DESCR), ''),
    [StockQuantity] = ISNULL(SUM(STINVTOT.ONHAND), 0)
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON STLINE.STOCKREF = ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON ITEMS.UNITSETREF = UNITSETF.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON UNITSETF.LOGICALREF = UNITSETL.UNITSETREF AND UNITSETL.MAINUNIT = 1
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_MARK AS BRAND WITH(NOLOCK) ON ITEMS.MARKREF = BRAND.LOGICALREF
LEFT JOIN LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK) ON STINVTOT.STOCKREF = ITEMS.LOGICALREF
WHERE ITEMS.ACTIVE = 0 
  AND ITEMS.MOLD = 0 
  AND TOOL = 0 
  AND ITEMS.CARDTYPE NOT IN (1, 4, 13) 
  AND ITEMS.UNITSETREF <> 0 
  AND (STLINE.TRCODE IN (7, 8, 2, 3))
GROUP BY 
    ITEMS.LOGICALREF, 
    ITEMS.CODE, 
    ITEMS.NAME, 
    ITEMS.VAT, 
    ITEMS.CANCONFIGURE, 
    ITEMS.TRACKTYPE, 
    ITEMS.LOCTRACKING, 
    ITEMS.STGRPCODE, 
    ITEMS.MARKREF
ORDER BY MAX(STLINE.DATE_) DESC;
";
            return baseQuery;
        }


    }

}
