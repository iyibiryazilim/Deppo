using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System.Text;

namespace Deppo.Core.DataStores;

public class ShipAddressDataStore : IShipAddressService
{
    private string postUrl = "/gateway/customQuery/CustomQuery";

    public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int currentReferenceId, string search = "", int skip = 0, int take = 20)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetObjectsByCurrentId(firmNumber, periodNumber, currentReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<IEnumerable<dynamic>>> GetObjectsByOrder(HttpClient httpClient, int firmNumber, int periodNumber, int currentReferenceId, string search = "", int skip = 0, int take = 20)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetObjectsByOrderQuery(firmNumber, periodNumber, currentReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

    private string GetObjectsByCurrentId(int firmNumber, int periodNumber, int currentReferenceId, string search = "", int skip = 0, int take = 20)
    {
        string baseQuery = @$"SELECT
			[ReferenceId] = SHIP.LOGICALREF,
            [Code] = SHIP.CODE,
            [Name] = SHIP.NAME,
            [Address] = SHIP.ADDR1,
            [City] = SHIP.CITY,
            [Country] = SHIP.COUNTRY
       FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_SHIPINFO AS SHIP WHERE CLIENTREF = {currentReferenceId}
		";

        if (!string.IsNullOrEmpty(search))
        {
            baseQuery += @$" AND SHIP.CODE LIKE '{search}%' OR SHIP.NAME LIKE '%{search}%'";
        }

        baseQuery += @$" ORDER BY SHIP.CODE ASC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }

    private string GetObjectsByOrderQuery(int firmNumber, int periodNumber, int currentReferenceId, string search = "", int skip = 0, int take = 20)
    {
        string baseQuery = @$"SELECT
[ReferenceId] = ISNULL(SHIP.LOGICALREF, 0),
[Code] = ISNULL(SHIP.CODE, ''),
[Name] = ISNULL(SHIP.NAME, ''),
[Address] = ISNULL(SHIP.ADDR1, ''),
[City] = ISNULL(SHIP.CITY, ''),
[Country] = ISNULL(SHIP.COUNTRY, '')
        FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFLINE AS ORFLINE
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_ORFICHE AS ORFICHE ON ORFLINE.ORDFICHEREF = ORFICHE.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON ORFICHE.CLIENTREF = CLCARD.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_SHIPINFO AS SHIP ON ORFICHE.SHIPINFOREF=SHIP.LOGICALREF
        LEFT JOIN L_CAPIWHOUSE AS WHOUSE ON ORFLINE.SOURCEINDEX = WHOUSE.NR AND WHOUSE.FIRMNR = {firmNumber}
		WHERE ORFLINE.CLOSED = 0 AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0 AND ORFLINE.TRCODE = 1 AND
		 CLCARD.LOGICALREF = {currentReferenceId}  GROUP BY SHIP.LOGICALREF,SHIP.CODE,SHIP.NAME,SHIP.ADDR1,SHIP.CITY,SHIP.COUNTRY
		";

        if (!string.IsNullOrEmpty(search))
        {
            baseQuery += @$" AND SHIP.CODE LIKE '{search}%' OR SHIP.NAME LIKE '%{search}%'";
        }

        baseQuery += @$" ORDER BY SHIP.CODE ASC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }
}