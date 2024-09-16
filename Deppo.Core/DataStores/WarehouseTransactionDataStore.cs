using Deppo.Core.DataResultModel;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Core.SortModels;
using System.Text.Json;

namespace Deppo.Core.DataStores;

public class WarehouseTransactionDataStore : IWarehouseTransactionService
{
	private string request = "/gateway/customQuery/CustomQuery";

	string postUrl = $"/gateway/product/" + typeof(WarehouseTransaction).Name;
	public async Task<DataResult<IEnumerable<WarehouseTransaction>>> GetInputTransactionByWarehouseNumberAsync(HttpClient httpClient,int number, string search, SortModel? orderBy, int page, int pageSize, int firmNumber)
	{
		HttpResponseMessage responseMessage = await httpClient.GetAsync(postUrl + $"/Warehouse/Number/{number}/Input?search={search}&orderBy={orderBy}&page={page}&pageSize={pageSize}&firmNumber={firmNumber}");
		DataResult<IEnumerable<WarehouseTransaction>> dataResult = new DataResult<IEnumerable<WarehouseTransaction>>();
		if (responseMessage.IsSuccessStatusCode)
		{
			var data = await responseMessage.Content.ReadAsStringAsync();
			if (data != null)
			{
				if (!string.IsNullOrEmpty(data))
				{
					var result = JsonSerializer.Deserialize<DataResult<IEnumerable<WarehouseTransaction>>>(data, new JsonSerializerOptions
					{
						PropertyNamingPolicy = JsonNamingPolicy.CamelCase
					});

					dataResult.Data = result?.Data;
					dataResult.IsSuccess = true;
					dataResult.Message = "success";
					return dataResult;

				}
				else
				{
					var result = JsonSerializer.Deserialize<DataResult<IEnumerable<WarehouseTransaction>>>(data, new JsonSerializerOptions
					{
						PropertyNamingPolicy = JsonNamingPolicy.CamelCase
					});

					dataResult.Data = result?.Data;
					dataResult.IsSuccess = true;
					dataResult.Message = "empty";
					return dataResult;
				}

			}
			else
			{
				var result = JsonSerializer.Deserialize<DataResult<IEnumerable<WarehouseTransaction>>>(data, new JsonSerializerOptions
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				});

				dataResult.Data = Enumerable.Empty<WarehouseTransaction>();
				dataResult.IsSuccess = false;
				dataResult.Message = await responseMessage.Content.ReadAsStringAsync();

				return dataResult;
			}


		}
		else
		{
			dataResult.Data = Enumerable.Empty<WarehouseTransaction>();
			dataResult.IsSuccess = false;
			dataResult.Message = await responseMessage.Content.ReadAsStringAsync();
			return dataResult;
		}
	}

	public async Task<DataResult<IEnumerable<WarehouseTransaction>>> GetOutputTransactionByWarehouseNumberAsync(HttpClient httpClient, int number,string search, SortModel? orderBy, int page, int pageSize, int firmNumber)
	{
		HttpResponseMessage responseMessage = await httpClient.GetAsync(postUrl + $"/Warehouse/Number/{number}/Output?search={search}&orderBy={orderBy}&page={page}&pageSize={pageSize}&firmNumber={firmNumber}");
		DataResult<IEnumerable<WarehouseTransaction>> dataResult = new DataResult<IEnumerable<WarehouseTransaction>>();
		if (responseMessage.IsSuccessStatusCode)
		{
			var data = await responseMessage.Content.ReadAsStringAsync();
			if (data != null)
			{
				if (!string.IsNullOrEmpty(data))
				{
					var result = JsonSerializer.Deserialize<DataResult<IEnumerable<WarehouseTransaction>>>(data, new JsonSerializerOptions
					{
						PropertyNamingPolicy = JsonNamingPolicy.CamelCase
					});

					dataResult.Data = result?.Data;
					dataResult.IsSuccess = true;
					dataResult.Message = "success";
					return dataResult;

				}
				else
				{
					var result = JsonSerializer.Deserialize<DataResult<IEnumerable<WarehouseTransaction>>>(data, new JsonSerializerOptions
					{
						PropertyNamingPolicy = JsonNamingPolicy.CamelCase
					});

					dataResult.Data = result?.Data;
					dataResult.IsSuccess = true;
					dataResult.Message = "empty";
					return dataResult;
				}

			}
			else
			{
				var result = JsonSerializer.Deserialize<DataResult<IEnumerable<WarehouseTransaction>>>(data, new JsonSerializerOptions
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				});

				dataResult.Data = Enumerable.Empty<WarehouseTransaction>();
				dataResult.IsSuccess = false;
				dataResult.Message = await responseMessage.Content.ReadAsStringAsync();

				return dataResult;
			}


		}
		else
		{
			dataResult.Data = Enumerable.Empty<WarehouseTransaction>();
			dataResult.IsSuccess = false;
			dataResult.Message = await responseMessage.Content.ReadAsStringAsync();
			return dataResult;
		}
	}

	private string GetInputTransactionByWarehouseNumberQuery(int firmNumber, int periodNumber, int warehouseNumber, string search = "", int skip = 0, int take = 20)
	{
		var baseQuery = "";

		return baseQuery;
	}
}
