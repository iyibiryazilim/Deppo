using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System.Text;

namespace Deppo.Core.DataStores
{
    public class ProductCountingDataStore : IProductCountingService
    {
        private string postUrl = "/gateway/customQuery/CustomQuery";
        public async Task<DataResult<IEnumerable<dynamic>>> GetWarehouses(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "")
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetWarehouses(firmNumber, periodNumber, productReferenceId, search, skip, take, externalDb)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<IEnumerable<dynamic>>> GetWarehousesByVariant(HttpClient httpClient, int firmNumber, int periodNumber, int variantReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "")
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetWarehousesByVariant(firmNumber, periodNumber, variantReferenceId, search, skip, take, externalDb)), Encoding.UTF8, "application/json");

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

        private string GetWarehouses(int firmNumber, int periodNumber,int productReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "")
        {
            var baseQuery = @$"SELECT
			[ReferenceId] = LGMAIN.LOGICALREF,
			[Number] = LGMAIN.NR,
			[Name] = LGMAIN.NAME,
			[DivisionReferenceId] = 0,
			[DivisionNumber] = LGMAIN.DIVISNR,
			[City] = LGMAIN.CITY,
			[Country] = LGMAIN.COUNTRY,
            [LocationCount] = (SELECT ISNULL(COUNT(*),0) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_LOCATION INVLOC WHERE INVLOC.INVENNR = LGMAIN.NR),
			[Quantity] = ISNULL((SELECT SUM(ONHAND) FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK) WHERE STINVTOT.STOCKREF = {productReferenceId} AND STINVTOT.INVENNO = LGMAIN.NR),0)
			FROM {externalDb}L_CAPIWHOUSE AS LGMAIN WITH (NOLOCK) 

            WHERE LGMAIN.FIRMNR = {firmNumber}";

            if (!string.IsNullOrEmpty(search))
            {
                baseQuery += $" AND (LGMAIN.NAME LIKE '%{search}%' OR LGMAIN.NR LIKE '%{search}%')";
            }

            baseQuery += @$"
ORDER BY 
    LGMAIN.NR
OFFSET {skip} ROWS 
FETCH NEXT {take} ROWS ONLY;";

            return baseQuery;
        }

        private string GetWarehousesByVariant(int firmNumber, int periodNumber, int variantReferenceId, string search = "", int skip = 0, int take = 20, string externalDb = "")
        {
            var baseQuery = @$"SELECT
			[ReferenceId] = LGMAIN.LOGICALREF,
			[Number] = LGMAIN.NR,
			[Name] = LGMAIN.NAME,
			[DivisionReferenceId] = 0,
			[DivisionNumber] = LGMAIN.DIVISNR,
			[City] = LGMAIN.CITY,
			[Country] = LGMAIN.COUNTRY,
            [LocationCount] = (SELECT ISNULL(COUNT(*),0) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_LOCATION INVLOC WHERE INVLOC.INVENNR = LGMAIN.NR),
			[Quantity] = ISNULL((SELECT SUM(ONHAND) FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_VRNTINVTOT AS VRNTINVTOT WITH(NOLOCK) WHERE VRNTINVTOT.VARIANTREF = {variantReferenceId} AND VRNTINVTOT.INVENNO = LGMAIN.NR),0)
			FROM {externalDb}L_CAPIWHOUSE AS LGMAIN WITH (NOLOCK) 

            WHERE LGMAIN.FIRMNR = {firmNumber}";

            if (!string.IsNullOrEmpty(search))
            {
                baseQuery += $" AND (LGMAIN.NAME LIKE '%{search}%' OR LGMAIN.NR LIKE '%{search}%')";
            }

            baseQuery += @$"
ORDER BY 
    LGMAIN.NR
OFFSET {skip} ROWS 
FETCH NEXT {take} ROWS ONLY;";

            return baseQuery;
        }
    }
}
