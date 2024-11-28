using System;
using System.Text;
using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;

namespace Deppo.Core.DataStores;

public class ProductPictureDataStore : IProductPictureService
{
    private string postUrl = "/gateway/customQuery/CustomQuery";

    public async Task<DataResult<IEnumerable<dynamic>>> InsertPictureAsync(HttpClient httpClient, int firmNumber, int productReferenceId, byte[] picture)
    {
        var content = new StringContent(JsonConvert.SerializeObject(InsertPictureQuery(firmNumber,productReferenceId,picture)), Encoding.UTF8, "application/json");

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

    private string InsertPictureQuery(int firmNumber,int productReferenceId, byte[] picture)
    {
        string baseQuery = $@"INSERT INTO LG_{firmNumber.ToString().PadLeft(3,'0')} (INFOTYP,INFOREF,DOCTYP,DOCNR,LDATA) VALUES (20,{productReferenceId},0,11,CAST('{picture.ToString()}' AS VARBINARY(MAX)))";

        return baseQuery;
    }
}
