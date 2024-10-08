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
    SUPPLIER.LOGICALREF AS [ReferenceId],
    SUPPLIER.CODE AS [Code],
    SUPPLIER.DEFINITION_ AS [Title],
    CASE
        WHEN SUPPLIER.ISPERSCOMP = 0 THEN 0
        ELSE 1
    END AS [IsPersonal],
    SUPPLIER.DEFINITION_ AS [Name],
    SUPPLIER.EMAILADDR AS [Email],
    SUPPLIER.TELNRS1 + ' ' + SUPPLIER.TELNRS2 AS [Telephone],
    SUPPLIER.ADDR1 AS [Address],
    SUPPLIER.CITY AS [City],
    SUPPLIER.COUNTRY AS [Country],
    SUPPLIER.POSTCODE AS [PostalCode],
    SUPPLIER.TAXOFFICE AS [TaxOffice],
    SUPPLIER.TAXNR AS [TaxNumber],
    CASE
        WHEN SUPPLIER.ACTIVE = 0 THEN 0
        ELSE 1
    END AS [IsActive],
	[OrderReferenceCount] = ISNULL( (SELECT COUNT(DISTINCT ORFLINE.STOCKREF)
        FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE
        WHERE ORFLINE.CLIENTREF = SUPPLIER.LOGICALREF
        AND ORFLINE.TRCODE = 2
        AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0
        AND ORFLINE.CLOSED = 0),0)

FROM
    LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS SUPPLIER
WHERE
    SUPPLIER.CODE LIKE '32%'
    AND SUPPLIER.CODE <> 'ÿ'";

            if (!string.IsNullOrEmpty(search))
                baseQuery += $@" AND (SUPPLIER.CODE LIKE '{search}%' OR SUPPLIER.DEFINITION_ LIKE '%{search}%')";

            baseQuery += $@" ORDER BY SUPPLIER.CODE DESC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

            return baseQuery;
        }
    }
}