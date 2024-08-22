using Deppo.Core.DataResultModel;
using Deppo.Core.Models;
using Deppo.Core.Services;
using Deppo.Core.SortModels;
using System.Text.Json;

namespace Deppo.Core.DataStores;

public class ProductTransactionLineDataStore : IProductTransactionLineService
{
	string postUrl = $"/gateway/product/ProductTransactionLine";
	public async Task<DataResult<IEnumerable<ProductTransaction>>> GetInputTransactionLineByProductId(HttpClient httpClient, int productId, string search, SortModel? orderBy, int page, int pageSize, int firmNumber)
	{
		HttpResponseMessage responseMessage = await httpClient.GetAsync(postUrl + $"/Product/Id/{productId}/Input?search={search}&orderBy={orderBy}&page={page}&pageSize={pageSize}&firmNumber={firmNumber}");
		DataResult<IEnumerable<ProductTransaction>> dataResult = new DataResult<IEnumerable<ProductTransaction>>();
		if (responseMessage.IsSuccessStatusCode)
		{
			var data = await responseMessage.Content.ReadAsStringAsync();
			if (data != null)
			{
				if (!string.IsNullOrEmpty(data))
				{
					var result = JsonSerializer.Deserialize<DataResult<IEnumerable<ProductTransaction>>>(data, new JsonSerializerOptions
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
					var result = JsonSerializer.Deserialize<DataResult<IEnumerable<ProductTransaction>>>(data, new JsonSerializerOptions
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
				var result = JsonSerializer.Deserialize<DataResult<IEnumerable<ProductTransaction>>>(data, new JsonSerializerOptions
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				});

				dataResult.Data = Enumerable.Empty<ProductTransaction>();
				dataResult.IsSuccess = false;
				dataResult.Message = await responseMessage.Content.ReadAsStringAsync();

				return dataResult;
			}
		}
		else
		{
			dataResult.Data = Enumerable.Empty<ProductTransaction>();
			dataResult.IsSuccess = false;
			dataResult.Message = await responseMessage.Content.ReadAsStringAsync();
			return dataResult;
		}
	}

	public async Task<DataResult<IEnumerable<ProductTransaction>>> GetOutputTransactionLineByProductId(HttpClient httpClient, int productId, string search, SortModel? orderBy, int page, int pageSize, int firmNumber)
	{
		HttpResponseMessage responseMessage = await httpClient.GetAsync(postUrl + $"/Product/Id/{productId}/Output?search={search}&orderBy={orderBy}&page={page}&pageSize={pageSize}&firmNumber={firmNumber}");
		DataResult<IEnumerable<ProductTransaction>> dataResult = new DataResult<IEnumerable<ProductTransaction>>();
		if (responseMessage.IsSuccessStatusCode)
		{
			var data = await responseMessage.Content.ReadAsStringAsync();
			if (data != null)
			{
				if (!string.IsNullOrEmpty(data))
				{
					var result = JsonSerializer.Deserialize<DataResult<IEnumerable<ProductTransaction>>>(data, new JsonSerializerOptions
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
					var result = JsonSerializer.Deserialize<DataResult<IEnumerable<ProductTransaction>>>(data, new JsonSerializerOptions
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
				var result = JsonSerializer.Deserialize<DataResult<IEnumerable<ProductTransaction>>>(data, new JsonSerializerOptions
				{
					PropertyNamingPolicy = JsonNamingPolicy.CamelCase
				});

				dataResult.Data = Enumerable.Empty<ProductTransaction>();
				dataResult.IsSuccess = false;
				dataResult.Message = await responseMessage.Content.ReadAsStringAsync();

				return dataResult;
			}


		}
		else
		{
			dataResult.Data = Enumerable.Empty<ProductTransaction>();
			dataResult.IsSuccess = false;
			dataResult.Message = await responseMessage.Content.ReadAsStringAsync();
			return dataResult;
		}
	}
}
