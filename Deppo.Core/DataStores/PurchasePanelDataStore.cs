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


        public async Task<DataResult<IEnumerable<dynamic>>> SupplierTransaction(HttpClient httpClient, int firmNumber, int periodNumber)
        {
            var content = new StringContent(JsonConvert.SerializeObject(SupplierTransaction(firmNumber, periodNumber)), Encoding.UTF8, "application/json");

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
	SUM(ORFLINE.SHIPPEDAMOUNT) AS ShippedQuantityTotal
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE
WHERE ORFLINE.TRCODE = 1";
            return baseQuery;
        }

        private string TotalOrderCount(int firmNumber, int periodNumber)
        {
            string baseQuery = $@"SELECT 
	SUM(ORFLINE.AMOUNT) AS AmountTotal
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE
WHERE ORFLINE.TRCODE = 1";
            return baseQuery;
        }
        private string SupplierTransaction(int firmNumber, int periodNumber)
        {
            string baseQuery = $@"Select TOP 5
     [SupplierReferenceId] = CLCARD.LOGICALREF,
	 [SupplierCode] = CLCARD.CODE,
     [SupplierName] = CLCARD.DEFINITION_,
	 [BaseTransactionReferenceId] = STFICHE.LOGICALREF,
	 [BaseTransactionCode] = STFICHE.FICHENO,
     [TransactionType] = STLINE.TRCODE,
	 [GroupCode] = ISNULL(ITEMS.STGRPCODE,''),
	 [IOType] = STLINE.IOCODE,
	 [ProductReferenceId] =  ITEMS.LOGICALREF,
	 [ProductCode] = ITEMS.CODE,
	 [ProductName] = ITEMS.NAME,
	 [UnitsetReferenceId] = unitset.LOGICALREF,
	 [UnitsetCode] = unitset.CODE,
	 [UnitsetName] = unitset.NAME,
	 [SubUnitsetReferenceId] = subunitset.LOGICALREF,
	 [SubUnitsetCode] = subunitset.CODE,
	 [SubUnitsetName] = subunitset.NAME,
	 [WarehouseName] = capiwhouse.NAME,
	 [Quantity] = STLINE.AMOUNT,
	 [Length] = ITMUNITA.LENGTH,
	 [Width] = ITMUNITA.WIDTH,
	 [Height] = ITMUNITA.HEIGHT,
	 [Weight] = ITMUNITA.WEIGHT,
	 [Volume] = ITMUNITA.VOLUME_,
	  [Barcode] = (SELECT TOP 1 ISNULL(ITMUNITA.BARCODE,'') FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITBARCODE
	  WHERE ITEMREF = ITMUNITA.ITEMREF AND ITMUNITAREF = ITMUNITA.LOGICALREF AND UNITLINEREF = ITMUNITA.UNITLINEREF)
from  LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE as STLINE
Left Join LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE on STLINE.STFICHEREF = STFICHE.LOGICALREF
left join LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS on STLINE.STOCKREF = ITEMS.LOGICALREF
Left Join LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD as CLCARD ON STLINE.CLIENTREF = CLCARD.LOGICALREF
left join LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS subunitset ON STLINE.UOMREF = subunitset.LOGICALREF
left join LG_{firmNumber.ToString().PadLeft(3, '0')}_ITMUNITA AS ITMUNITA on subunitset.LOGICALREF = ITMUNITA.UNITLINEREF
left join LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS unitset ON STLINE.USREF = unitset.LOGICALREF
LEFT JOIN L_CAPIWHOUSE AS capiwhouse ON STLINE.SOURCEINDEX = capiwhouse.NR
where CLCARD.Code like  '32%' AND STFICHE.GRPCODE = 1
order by STLINE.DATE_ desc;";

            return baseQuery;
        }


    }
}
