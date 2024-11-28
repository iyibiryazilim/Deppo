using System;
using System.Text;
using System.Text.Json;
using Deppo.Core.DataResultModel;
using Deppo.Core.Models;
using Deppo.Core.Services;

namespace Deppo.Core.DataStores;

public class CompanyDataStore : ICompanyService
{
    private string postUrl = "/gateway/customQuery/CustomQuery";
    public async Task<DataResult<IEnumerable<Company>>> GetObjectsAsync(HttpClient httpClient, string query)
    {
        DataResult<IEnumerable<Company>> dataResult = new();

        var content = new StringContent(JsonSerializer.Serialize(query), Encoding.UTF8, "application/json");
        HttpResponseMessage responseMessage = await httpClient.PostAsync(postUrl, content);

        if (responseMessage.IsSuccessStatusCode)
        {
            var data = await responseMessage.Content.ReadAsStringAsync();
            if (data != null)
            {
                if (!string.IsNullOrEmpty(data))
                {
                    var result = JsonSerializer.Deserialize<DataResult<IEnumerable<Company>>>(data);

                    dataResult.Data = result?.Data;
                    dataResult.IsSuccess = true;
                    dataResult.Message = "success";
                }
                else
                {
                    var result = JsonSerializer.Deserialize<DataResult<IEnumerable<Company>>>(data);

                    dataResult.Data = result?.Data;
                    dataResult.IsSuccess = true;
                    dataResult.Message = "empty";
                }

            }
            else
            {
                var result = JsonSerializer.Deserialize<DataResult<IEnumerable<Company>>>(data);

                dataResult.Data = null;
                dataResult.IsSuccess = false;
                dataResult.Message = await responseMessage.Content.ReadAsStringAsync();
            }


        }
        else
        {
            dataResult.Data = null;
            dataResult.IsSuccess = false;
            dataResult.Message = await responseMessage.Content.ReadAsStringAsync();
        }

        return dataResult;
    }
}
