using System;
using System.Text.Json;
using Deppo.Core.BaseModels;
using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Deppo.Core.SortModels;
using Newtonsoft.Json;

namespace Deppo.Core.DataStores;

public class ProductDataStore 
{
    public string postUrl = $"/gateway/product/" + typeof(Product).Name;

    public async Task<DataResult<IEnumerable<Product>>> GetObjects(HttpClient httpClient, string search, string groupCode, SortModel? orderBy, int page, int pageSize, int firmNumber)
    {
        HttpResponseMessage responseMessage = await httpClient.GetAsync(postUrl + $"?search={search}&groupCode={groupCode}&orderBy={orderBy}&page={page}&pageSize={pageSize}&firmNumber={firmNumber}");
        DataResult<IEnumerable<Product>> dataResult = new DataResult<IEnumerable<Product>>();
        if (responseMessage.IsSuccessStatusCode)
        {
            var data = await responseMessage.Content.ReadAsStringAsync();
            if (data != null)
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var result = JsonConvert.DeserializeObject<DataResult<IEnumerable<Product>>>(data);

                    dataResult.Data = result?.Data;
                    dataResult.IsSuccess = true;
                    dataResult.Message = "success";
                    return dataResult;

                }
                else
                {
                    var result = JsonConvert.DeserializeObject<DataResult<IEnumerable<Product>>>(data);

                    dataResult.Data = result?.Data;
                    dataResult.IsSuccess = true;
                    dataResult.Message = "empty";
                    return dataResult;
                }

            }
            else
            {
                var result = JsonConvert.DeserializeObject<DataResult<IEnumerable<Product>>>(data);

                dataResult.Data = Enumerable.Empty<Product>();
                dataResult.IsSuccess = false;
                dataResult.Message = await responseMessage.Content.ReadAsStringAsync();

                return dataResult;
            }


        }
        else
        {
            dataResult.Data = Enumerable.Empty<Product>();
            dataResult.IsSuccess = false;
            dataResult.Message = await responseMessage.Content.ReadAsStringAsync();
            return dataResult;
        }
    }
    public async Task<DataResult<Product>> GetObjectById(HttpClient httpClient, int ReferenceId, int firmNumber)
    {
        HttpResponseMessage responseMessage = await httpClient.GetAsync($"{postUrl}/Id/{ReferenceId}?firmNumber={firmNumber}");
        DataResult<Product> dataResult = new DataResult<Product>();
        if (responseMessage.IsSuccessStatusCode)
        {
            var data = await responseMessage.Content.ReadAsStringAsync();
            if (data != null)
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var result = JsonConvert.DeserializeObject<DataResult<Product>>(data);

                    dataResult.Data = result?.Data;
                    dataResult.IsSuccess = true;
                    dataResult.Message = "success";
                    return dataResult;

                }
                else
                {
                    var result = JsonConvert.DeserializeObject<DataResult<Product>>(data);

                    dataResult.Data = result?.Data;
                    dataResult.IsSuccess = true;
                    dataResult.Message = "empty";
                    return dataResult;
                }

            }
            else
            {
                var result = JsonConvert.DeserializeObject<DataResult<Product>>(data);

                dataResult.Data = null;
                dataResult.IsSuccess = false;
                dataResult.Message = await responseMessage.Content.ReadAsStringAsync();

                return dataResult;
            }


        }
        else
        {
            dataResult.Data = null;
            dataResult.IsSuccess = false;
            dataResult.Message = await responseMessage.Content.ReadAsStringAsync();
            return dataResult;
        }
    }
    public async Task<DataResult<Product>> GetObjectByCode(HttpClient httpClient, string Code, int firmNumber)
    {
        HttpResponseMessage responseMessage = await httpClient.GetAsync($"{postUrl}/Code/{Code}?firmNumber={firmNumber}");
        DataResult<Product> dataResult = new DataResult<Product>();
        if (responseMessage.IsSuccessStatusCode)
        {
            var data = await responseMessage.Content.ReadAsStringAsync();
            if (data != null)
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var result = JsonConvert.DeserializeObject<DataResult<Product>>(data);

                    dataResult.Data = result?.Data;
                    dataResult.IsSuccess = true;
                    dataResult.Message = "success";
                    return dataResult;

                }
                else
                {
                    var result = JsonConvert.DeserializeObject<DataResult<Product>>(data);

                    dataResult.Data = result?.Data;
                    dataResult.IsSuccess = true;
                    dataResult.Message = "empty";
                    return dataResult;
                }

            }
            else
            {
                var result = JsonConvert.DeserializeObject<DataResult<Product>>(data);

                dataResult.Data = null;
                dataResult.IsSuccess = false;
                dataResult.Message = await responseMessage.Content.ReadAsStringAsync();

                return dataResult;
            }


        }
        else
        {
            dataResult.Data = null;
            dataResult.IsSuccess = false;
            dataResult.Message = await responseMessage.Content.ReadAsStringAsync();
            return dataResult;
        }
    }


}
