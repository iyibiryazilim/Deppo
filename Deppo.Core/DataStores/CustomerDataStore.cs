using Deppo.Core.DataResultModel;
using Deppo.Core.Models;
using Deppo.Core.Services;

namespace Deppo.Core.DataStores;

public class CustomerDataStore : ICustomerService
{
	public string postUrl = $"/gateway/sales/" + typeof(Customer).Name;

	public Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
	{
		throw new NotImplementedException();
	}

	public Task<DataResult<dynamic>> GetObjectById(HttpClient httpClient, int firmNumber, int periodNumber, int customerReferenceId)
	{
		throw new NotImplementedException();
	}

	//public async Task<DataResult<IEnumerable<Customer>>> GetObjects(HttpClient httpClient, string search, string groupCode, SortModel? orderBy, int page, int pageSize, int firmNumber)
	//{
	//    HttpResponseMessage responseMessage = await httpClient.GetAsync(postUrl + $"?search={search}&groupCode={groupCode}&orderBy={orderBy}&page={page}&pageSize={pageSize}&firmNumber={firmNumber}");
	//    DataResult<IEnumerable<Customer>> dataResult = new DataResult<IEnumerable<Customer>>();
	//    if (responseMessage.IsSuccessStatusCode)
	//    {
	//        var data = await responseMessage.Content.ReadAsStringAsync();
	//        if (data != null)
	//        {
	//            if (!string.IsNullOrEmpty(data))
	//            {
	//                var result = JsonSerializer.Deserialize<DataResult<IEnumerable<Customer>>>(data, new JsonSerializerOptions
	//                {
	//                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	//                });

	//                dataResult.Data = result?.Data;
	//                dataResult.IsSuccess = true;
	//                dataResult.Message = "success";
	//                return dataResult;

	//            }
	//            else
	//            {
	//                var result = JsonSerializer.Deserialize<DataResult<IEnumerable<Customer>>>(data, new JsonSerializerOptions
	//                {
	//                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	//                });

	//                dataResult.Data = result?.Data;
	//                dataResult.IsSuccess = true;
	//                dataResult.Message = "empty";
	//                return dataResult;
	//            }

	//        }
	//        else
	//        {
	//            var result = JsonSerializer.Deserialize<DataResult<IEnumerable<Customer>>>(data, new JsonSerializerOptions
	//            {
	//                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	//            });

	//            dataResult.Data = Enumerable.Empty<Customer>();
	//            dataResult.IsSuccess = false;
	//            dataResult.Message = await responseMessage.Content.ReadAsStringAsync();

	//            return dataResult;
	//        }

	//    }
	//    else
	//    {
	//        dataResult.Data = Enumerable.Empty<Customer>();
	//        dataResult.IsSuccess = false;
	//        dataResult.Message = await responseMessage.Content.ReadAsStringAsync();
	//        return dataResult;
	//    }
	//}
	//public async Task<DataResult<Customer>> GetObjectById(HttpClient httpClient, int ReferenceId, int firmNumber)
	//{
	//    HttpResponseMessage responseMessage = await httpClient.GetAsync($"{postUrl}/Id/{ReferenceId}?firmNumber={firmNumber}");
	//    DataResult<Customer> dataResult = new DataResult<Customer>();
	//    if (responseMessage.IsSuccessStatusCode)
	//    {
	//        var data = await responseMessage.Content.ReadAsStringAsync();
	//        if (data != null)
	//        {
	//            if (!string.IsNullOrEmpty(data))
	//            {
	//                var result = JsonSerializer.Deserialize<DataResult<Customer>>(data, new JsonSerializerOptions
	//                {
	//                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	//                });

	//                dataResult.Data = result?.Data;
	//                dataResult.IsSuccess = true;
	//                dataResult.Message = "success";
	//                return dataResult;

	//            }
	//            else
	//            {
	//                var result = JsonSerializer.Deserialize<DataResult<Customer>>(data, new JsonSerializerOptions
	//                {
	//                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	//                });

	//                dataResult.Data = result?.Data;
	//                dataResult.IsSuccess = true;
	//                dataResult.Message = "empty";
	//                return dataResult;
	//            }

	//        }
	//        else
	//        {
	//            var result = JsonSerializer.Deserialize<DataResult<Customer>>(data, new JsonSerializerOptions
	//            {
	//                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	//            });

	//            dataResult.Data = null;
	//            dataResult.IsSuccess = false;
	//            dataResult.Message = await responseMessage.Content.ReadAsStringAsync();

	//            return dataResult;
	//        }

	//    }
	//    else
	//    {
	//        dataResult.Data = null;
	//        dataResult.IsSuccess = false;
	//        dataResult.Message = await responseMessage.Content.ReadAsStringAsync();
	//        return dataResult;
	//    }
	//}
}