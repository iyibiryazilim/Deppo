using System;
using System.Text;
using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;

namespace Deppo.Core.DataStores;

public class OutsourceDataStore : IOutsourceService
{
    private string postUrl = "/gateway/customQuery/CustomQuery";

    public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
    {
        var content = new StringContent(JsonConvert.SerializeObject(OutsourceQuery(firmNumber, periodNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

	public async Task<DataResult<dynamic>> GetObjectById(HttpClient httpClient, int firmNumber, int periodNumber, int outsourceReferenceId)
	{
		var content = new StringContent(JsonConvert.SerializeObject(OutsourceQueryById(firmNumber, periodNumber, outsourceReferenceId)), Encoding.UTF8, "application/json");

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

	public async Task<DataResult<IEnumerable<dynamic>>> GetOutsourceWarehousesAsync(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
    {
        var content = new StringContent(JsonConvert.SerializeObject(OutsourceWarehouseQuery(firmNumber, periodNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

    private string OutsourceWarehouseQuery(int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
    {
        string baseQuery = $@"SELECT
[ReferenceId] = WAREHOUSE.LOGICALREF,
[Number] = WAREHOUSE.NR,
[Name] = WAREHOUSE.NAME,
[City] = WAREHOUSE.CITY,
[County] = WAREHOUSE.TOWN,
[Quantity] = 0
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS OUTSOURCE WITH(NOLOCK)
LEFT JOIN L_CAPIWHOUSE AS WAREHOUSE WITH(NOLOCK) ON (OUTSOURCE.OUTINVENNR = WAREHOUSE.NR) AND WAREHOUSE.FIRMNR = {firmNumber}
WHERE SUBCONT = 1
GROUP BY WAREHOUSE.LOGICALREF,WAREHOUSE.NR,WAREHOUSE.NAME,WAREHOUSE.CITY,WAREHOUSE.TOWN";

        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" AND (WAREHOUSE.NR LIKE '{search}%' OR WAREHOUSE.NAME LIKE '%{search}%')";

        baseQuery += $@" ORDER BY WAREHOUSE.NR DESC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }

    private string OutsourceQuery(int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
    {
        string baseQuery = $@"SELECT
[ReferenceId] = CUSTOMER.LOGICALREF,
[Code] = CUSTOMER.CODE,
[Title] = CUSTOMER.DEFINITION_,
[IsPersonal] =
        CASE
            WHEN CUSTOMER.ISPERSCOMP = 0 THEN 0
            ELSE 1
        END,
[Name] = CUSTOMER.DEFINITION_,
[Email] = CUSTOMER.EMAILADDR,
[Telephone] = CUSTOMER.TELNRS1 + ' ' + CUSTOMER.TELNRS2,
[Address] = CUSTOMER.ADDR1,
[City] = CUSTOMER.CITY,
[Country] = CUSTOMER.COUNTRY,
[PostalCode] = CUSTOMER.POSTCODE,
[TaxOffice] = CUSTOMER.TAXOFFICE,
[TaxNumber] = CUSTOMER.TAXNR,
[OrderReferenceCount] = 0,
[ShipAddressCount] = ISNULL((SELECT COUNT(LOGICALREF)
                            FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_SHIPINFO
                            WHERE CLIENTREF = CUSTOMER.LOGICALREF), 0),
[IsActive] =
       CASE
	      WHEN CUSTOMER.ACTIVE = 0 THEN 0
		  ELSE 1
        END,
[IsEDispatch] = CASE
        WHEN CUSTOMER.ACCEPTEDESP = 0 THEN 0
        ELSE 1
    END
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CUSTOMER
WHERE CUSTOMER.SUBCONT = 1
  AND CUSTOMER.CODE <> 'ÿ'
  AND CUSTOMER.ACTIVE = 0";

        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" AND (CUSTOMER.CODE LIKE '{search}%'
                        OR CUSTOMER.DEFINITION_ LIKE '%{search}%')";

        baseQuery += $@" ORDER BY CUSTOMER.CODE DESC
                    OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }

	private string OutsourceQueryById(int firmNumber, int periodNumber, int outsourceReferenceId)
	{
		string baseQuery = $@"SELECT
[ReferenceId] = CUSTOMER.LOGICALREF,
[Code] = CUSTOMER.CODE,
[Title] = CUSTOMER.DEFINITION_,
[IsPersonal] =
        CASE
            WHEN CUSTOMER.ISPERSCOMP = 0 THEN 0
            ELSE 1
        END,
[Name] = CUSTOMER.DEFINITION_,
[Email] = CUSTOMER.EMAILADDR,
[Telephone] = CUSTOMER.TELNRS1 + ' ' + CUSTOMER.TELNRS2,
[Address] = CUSTOMER.ADDR1,
[City] = CUSTOMER.CITY,
[Country] = CUSTOMER.COUNTRY,
[PostalCode] = CUSTOMER.POSTCODE,
[TaxOffice] = CUSTOMER.TAXOFFICE,
[TaxNumber] = CUSTOMER.TAXNR,
[OrderReferenceCount] = 0,
[ShipAddressCount] = ISNULL((SELECT COUNT(LOGICALREF)
                            FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_SHIPINFO
                            WHERE CLIENTREF = CUSTOMER.LOGICALREF), 0),
[IsActive] =
       CASE
	      WHEN CUSTOMER.ACTIVE = 0 THEN 0
		  ELSE 1
        END,
[IsEDispatch] = CASE
        WHEN CUSTOMER.ACCEPTEDESP = 0 THEN 0
        ELSE 1
    END
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CUSTOMER
WHERE CUSTOMER.SUBCONT = 1
  AND CUSTOMER.CODE <> 'ÿ'
  AND CUSTOMER.ACTIVE = 0 AND CUSTOMER.LOGICALREF = {outsourceReferenceId}";


		return baseQuery;
	}
}