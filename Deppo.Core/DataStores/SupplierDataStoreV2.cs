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
    public class SupplierDataStoreV2 : ISupplierService
    {
        private string postUrl = "/gateway/customQuery/CustomQuery";

        public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
        {
            var content = new StringContent(JsonConvert.SerializeObject(SupplierQuery(firmNumber, periodNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

        private string SupplierQuery(int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
        {
            string baseQuery = $@"SELECT
[ReferenceId]=SUPPLIER.LOGICALREF,
[Code]=SUPPLIER.CODE,
[Title]=SUPPLIER.DEFINITION_,
[IsPersonal] =
        CASE
            WHEN SUPPLIER.ISPERSCOMP= 0 THEN 0
            ELSE 1
        END,
[Name]=SUPPLIER.DEFINITION_,
[Email]=SUPPLIER.EMAILADDR,
[Telephone]=SUPPLIER.TELNRS1+' '+ SUPPLIER.TELNRS2,
[Address]=SUPPLIER.ADDR1,
[City]=SUPPLIER.CITY,
[Country]=SUPPLIER.COUNTRY,
[PostalCode]=SUPPLIER.POSTCODE,
[TaxOffice]=SUPPLIER.TAXOFFICE,
[TaxNumber]=SUPPLIER.TAXNR,
[IsActive]=
       CASE
	      WHEN SUPPLIER.ACTIVE=0 THEN 0
		  ELSE 1
END
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS SUPPLIER
WHERE SUPPLIER.CODE LIKE '32%' AND SUPPLIER.CODE <> 'ÿ' AND SUPPLIER.ACTIVE = 0
";

            if (!string.IsNullOrEmpty(search))
                baseQuery += $@" AND (SUPPLIER.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";

            baseQuery += $@" ORDER BY SUPPLIER.CODE DESC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

            return baseQuery;
        }
    }
}