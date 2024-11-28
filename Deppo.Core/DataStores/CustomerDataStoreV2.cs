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
    public class CustomerDataStoreV2 : ICustomerService
    {
        private string postUrl = "/gateway/customQuery/CustomQuery";

        public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
        {
            var content = new StringContent(JsonConvert.SerializeObject(CustomerQuery(firmNumber, periodNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

		public async Task<DataResult<dynamic>> GetObjectById(HttpClient httpClient, int firmNumber, int periodNumber, int customerReferenceId)
		{
			var content = new StringContent(JsonConvert.SerializeObject(CustomerQueryById(firmNumber, periodNumber, customerReferenceId)), Encoding.UTF8, "application/json");

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
						var result = JsonConvert.DeserializeObject<DataResult<Dictionary<string, object>>>(data);

						dataResult.Data = result?.Data;
						dataResult.IsSuccess = true;
						dataResult.Message = "empty";
						return dataResult;
					}
				}
				else
				{
					var result = JsonConvert.DeserializeObject<DataResult<Dictionary<string, object>>>(data);

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

		private string CustomerQuery(int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
        {
            string baseQuery = $@"SELECT
[ReferenceId]=CUSTOMER.LOGICALREF,
[Code]=CUSTOMER.CODE,
[Title]=CUSTOMER.DEFINITION_,
[IsPersonal] =
        CASE
            WHEN CUSTOMER.ISPERSCOMP= 0 THEN 0
            ELSE 1
        END,
[Name]=CUSTOMER.DEFINITION_,
[Email]=CUSTOMER.EMAILADDR,
[Telephone]=CUSTOMER.TELNRS1+' '+ CUSTOMER.TELNRS2,
[Address]=CUSTOMER.ADDR1,
[City]=CUSTOMER.CITY,
[Country]=CUSTOMER.COUNTRY,
[PostalCode]=CUSTOMER.POSTCODE,
[TaxOffice]=CUSTOMER.TAXOFFICE,
[TaxNumber]=CUSTOMER.TAXNR,
 CASE
        WHEN CUSTOMER.ACCEPTEDESP = 0 THEN 0
        ELSE 1
    END AS [IsEDispatch],
[OrderReferenceCount] = ISNULL((SELECT COUNT(DISTINCT STOCKREF) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE WHERE CLIENTREF = CUSTOMER.LOGICALREF AND (AMOUNT-SHIPPEDAMOUNT) > 0 AND CLOSED = 0  AND LINETYPE = 0 AND TRCODE = 1 ),0),
[IsActive]=
       CASE
	      WHEN CUSTOMER.ACTIVE=0 THEN 0
		  ELSE 1
END
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CUSTOMER
WHERE CUSTOMER.CODE LIKE '12%' AND CUSTOMER.CODE <> 'ÿ' AND CUSTOMER.ACTIVE = 0";

            if (!string.IsNullOrEmpty(search))
                baseQuery += $@" AND (CUSTOMER.CODE LIKE '{search}%' OR CUSTOMER.DEFINITION_ LIKE '%{search}%')";

            baseQuery += $@" ORDER BY CUSTOMER.CODE DESC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

            return baseQuery;
        }

		private string CustomerQueryById(int firmNumber, int periodNumber,int customerReferenceId)
		{
			string baseQuery = $@"SELECT
[ReferenceId]=CUSTOMER.LOGICALREF,
[Code]=CUSTOMER.CODE,
[Title]=CUSTOMER.DEFINITION_,
[IsPersonal] =
        CASE
            WHEN CUSTOMER.ISPERSCOMP= 0 THEN 0
            ELSE 1
        END,
[Name]=CUSTOMER.DEFINITION_,
[Email]=CUSTOMER.EMAILADDR,
[Telephone]=CUSTOMER.TELNRS1+' '+ CUSTOMER.TELNRS2,
[Address]=CUSTOMER.ADDR1,
[City]=CUSTOMER.CITY,
[Country]=CUSTOMER.COUNTRY,
[PostalCode]=CUSTOMER.POSTCODE,
[TaxOffice]=CUSTOMER.TAXOFFICE,
[TaxNumber]=CUSTOMER.TAXNR,
 CASE
        WHEN CUSTOMER.ACCEPTEDESP = 0 THEN 0
        ELSE 1
    END AS [IsEDispatch],
[OrderReferenceCount] = ISNULL((SELECT COUNT(DISTINCT STOCKREF) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE WHERE CLIENTREF = CUSTOMER.LOGICALREF AND (AMOUNT-SHIPPEDAMOUNT) > 0 AND CLOSED = 0  AND LINETYPE = 0 AND TRCODE = 1 ),0),
[IsActive]=
       CASE
	      WHEN CUSTOMER.ACTIVE=0 THEN 0
		  ELSE 1
END
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CUSTOMER
WHERE CUSTOMER.CODE LIKE '12%' AND CUSTOMER.CODE <> 'ÿ' AND CUSTOMER.ACTIVE = 0 AND CUSTOMER.LOGICALREF = {customerReferenceId}";

			

			return baseQuery;
		}
	}
}