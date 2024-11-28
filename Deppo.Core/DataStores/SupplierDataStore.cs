using System;
using System.Text.Json;
using Deppo.Core.DataResultModel;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Core.SortModels;

namespace Deppo.Core.DataStores;

public class SupplierDataStore : ISupplierService
{
    public string postUrl = $"/gateway/purchase/" + typeof(Supplier).Name;

    public Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
    {
        throw new NotImplementedException();
    }

	public Task<DataResult<dynamic>> GetObjectById(HttpClient httpClient, int firmNumber, int periodNumber, int supplierReferenceId)
	{
		throw new NotImplementedException();
	}

	//public async Task<DataResult<IEnumerable<Supplier>>> GetObjects(HttpClient httpClient, string search, string groupCode, SortModel? orderBy, int page, int pageSize, int firmNumber)
	//{
	//    HttpResponseMessage responseMessage = await httpClient.GetAsync(postUrl + $"?search={search}&groupCode={groupCode}&orderBy={orderBy}&page={page}&pageSize={pageSize}&firmNumber={firmNumber}");
	//    DataResult<IEnumerable<Supplier>> dataResult = new DataResult<IEnumerable<Supplier>>();
	//    if (responseMessage.IsSuccessStatusCode)
	//    {
	//        var data = await responseMessage.Content.ReadAsStringAsync();
	//        if (data != null)
	//        {
	//            if (!string.IsNullOrEmpty(data))
	//            {
	//                var result = JsonSerializer.Deserialize<DataResult<IEnumerable<Supplier>>>(data, new JsonSerializerOptions
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
	//                var result = JsonSerializer.Deserialize<DataResult<IEnumerable<Supplier>>>(data, new JsonSerializerOptions
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
	//            var result = JsonSerializer.Deserialize<DataResult<IEnumerable<Supplier>>>(data, new JsonSerializerOptions
	//            {
	//                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
	//            });

	//            dataResult.Data = Enumerable.Empty<Supplier>();
	//            dataResult.IsSuccess = false;
	//            dataResult.Message = await responseMessage.Content.ReadAsStringAsync();

	//            return dataResult;
	//        }

	//    }
	//    else
	//    {
	//        dataResult.Data = Enumerable.Empty<Supplier>();
	//        dataResult.IsSuccess = false;
	//        dataResult.Message = await responseMessage.Content.ReadAsStringAsync();
	//        return dataResult;
	//    }
	//}
	//public async Task<DataResult<Supplier>> GetObjectById(HttpClient httpClient, int ReferenceId, int firmNumber)
	//{
	//    HttpResponseMessage responseMessage = await httpClient.GetAsync($"{postUrl}/Id/{ReferenceId}?firmNumber={firmNumber}");
	//    DataResult<Supplier> dataResult = new DataResult<Supplier>();
	//    if (responseMessage.IsSuccessStatusCode)
	//    {
	//        var data = await responseMessage.Content.ReadAsStringAsync();
	//        if (data != null)
	//        {
	//            if (!string.IsNullOrEmpty(data))
	//            {
	//                var result = JsonSerializer.Deserialize<DataResult<Supplier>>(data, new JsonSerializerOptions
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
	//                var result = JsonSerializer.Deserialize<DataResult<Supplier>>(data, new JsonSerializerOptions
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
	//            var result = JsonSerializer.Deserialize<DataResult<Supplier>>(data, new JsonSerializerOptions
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