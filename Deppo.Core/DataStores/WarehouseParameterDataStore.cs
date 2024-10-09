using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System.Text;

namespace Deppo.Core.DataStores
{
    public class WarehouseParameterDataStore : IWarehouseParameterService
    {
        private string postUrl = "/gateway/customQuery/CustomQuery";
        public async Task<DataResult<dynamic>> GetObjectsByProduct(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber,int productReferenceId, string search = "", int skip = 0, int take = 20)
        {

            var content = new StringContent(JsonConvert.SerializeObject(WarehouseParameterByProductQuery(firmNumber, periodNumber, warehouseNumber,productReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<dynamic>> GetObjectsByVariant(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int variantReferenceId, string search = "", int skip = 0, int take = 20)
        {

            var content = new StringContent(JsonConvert.SerializeObject(WarehouseParameterByVariantQuery(firmNumber, periodNumber, warehouseNumber,variantReferenceId ,search, skip, take)), Encoding.UTF8, "application/json");

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

        private string WarehouseParameterByProductQuery(int firmNumber, int periodNumber, int warehouseNumber,int productReferenceId, string search = "", int skip = 0, int take = 20)
        {
            string baseQuery = $@"SELECT 
[MinLevel] = MINLEVEL,
[SafeLevel] = SAFELEVEL,
[MaxLevel] = MAXLEVEL
 FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_INVDEF WHERE ITEMREF = {productReferenceId} AND INVENNO = {warehouseNumber}
";


            return baseQuery;
        }

        private string WarehouseParameterByVariantQuery(int firmNumber, int periodNumber, int warehouseNumber, int variantReferenceId, string search = "", int skip = 0, int take = 20)
        {
            string baseQuery = $@"SELECT 
[MinLevel] = MINLEVEL,
[SafeLevel] = SAFELEVEL,
[MaxLevel] = MAXLEVEL
 FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_INVDEF WHERE VARIANTREF = {variantReferenceId} AND INVENNO = {warehouseNumber}
";


            return baseQuery;
        }
    }
}
