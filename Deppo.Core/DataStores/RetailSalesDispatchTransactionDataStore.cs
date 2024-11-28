using Deppo.Core.DataResultModel;
using Deppo.Core.DTOs.PurchaseDispatchTransaction;
using Deppo.Core.DTOs.PurchaseReturnDispatchTransaction;
using Deppo.Core.DTOs.SalesDispatchTransaction;
using Deppo.Core.ResponseResultModels;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DataStores;

public class RetailSalesDispatchTransactionDataStore : IRetailSalesDispatchTransactionService
{
	string customPostUrl = "/gateway/customQuery/CustomQuery";

	public async Task<DataResult<dynamic>> Delete(HttpClient httpClient, int firmNumber, int periodNumber, int referenceId)
	{
		var content = new StringContent(JsonConvert.SerializeObject(DeleteLine(firmNumber, periodNumber, referenceId)), Encoding.UTF8, "application/json");

		HttpResponseMessage responseMessage = await httpClient.PostAsync(customPostUrl, content);
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

	public async Task<DataResult<ResponseModel>> InsertRetailSalesDispatchTransaction(HttpClient httpClient, int firmNumber, RetailSalesDispatchTransactionInsert dto)
    {
        var postUrl = $"/gateway/sales/RetailSalesDispatchTransaction/Tiger?firmNumber={firmNumber}";
        var result = new DataResult<ResponseModel>();
        try
        {
            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");


            var responseMessage = await httpClient.PostAsync($"{postUrl}", content);

            if (responseMessage.IsSuccessStatusCode)
            {
                var data = await responseMessage.Content.ReadAsStringAsync();
                var dtos = data.Trim('"').Replace("\\\"", "\"").Replace("\\", "");
                var dtos2 = JsonConvert.DeserializeObject<DataResult<ResponseModel>>(dtos);
                result.Message = dtos2.Message;
                result.IsSuccess = dtos2.IsSuccess;
                result.Data = dtos2.Data;
                return result;

            }
            else
            {

                var message = await responseMessage.Content.ReadAsStringAsync();
                result.Message = message;
                result.IsSuccess = false;
                return result;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            return new DataResult<ResponseModel>
            {
                IsSuccess = false,
                Message = $"An error occurred: {ex.Message}"
            };
        }
    }

	public async Task<DataResult<dynamic>> UpdateLine(HttpClient httpClient, int firmNumber, int periodNumber, int referenceId, double quantity)
	{
		
		var content = new StringContent(JsonConvert.SerializeObject(UpdateLineQuery(firmNumber, periodNumber, referenceId, quantity)), Encoding.UTF8, "application/json");

		HttpResponseMessage responseMessage = await httpClient.PostAsync(customPostUrl, content);
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

	public async Task<DataResult<dynamic>> UpdateFicheStatus(HttpClient httpClient, int firmNumber, int periodNumber, int referenceId, int transactionNumber,int isEDispatch)
	{

		var content = new StringContent(JsonConvert.SerializeObject(UpdateFicheQuery(firmNumber, periodNumber, referenceId,transactionNumber, isEDispatch)), Encoding.UTF8, "application/json");


		HttpResponseMessage responseMessage = await httpClient.PostAsync(customPostUrl, content);
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

	public async Task<DataResult<ResponseModel>> UpdateFiche(HttpClient httpClient, int firmNumber, RetailSalesDispatchUpdateDto dto)
	{

		var postUrl = $"/gateway/sales/RetailSalesDispatchTransaction/Tiger/Update/?firmNumber={firmNumber}";

		var json = JsonConvert.SerializeObject(dto);
		var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

		HttpResponseMessage responseMessage = await httpClient.PostAsync(postUrl, content);
		var result = new DataResult<ResponseModel>();
		if (responseMessage.IsSuccessStatusCode)
		{
			var data = await responseMessage.Content.ReadAsStringAsync();
			if (data != null)
			{
				if (!string.IsNullOrEmpty(data))
				{
					var dtos = data.Trim('"').Replace("\\\"", "\"").Replace("\\", "");
					var dtos2 = JsonConvert.DeserializeObject<DataResult<ResponseModel>>(dtos);

					result.Data = dtos2?.Data;
					result.IsSuccess = true;
					result.Message = "success";
					return result;
				}
				else
				{
					var message = await responseMessage.Content.ReadAsStringAsync();
					result.Message = message;
					result.IsSuccess = false;
					return result;
				}
			}
			else
			{
				var message = await responseMessage.Content.ReadAsStringAsync();
				result.Message = message;
				result.IsSuccess = false;
				return result;
			}
		}
		else
		{
			result.Data = null;
			result.IsSuccess = false;
			result.Message = await responseMessage.Content.ReadAsStringAsync();
			return result;
		}
	}

	private string UpdateLineQuery(int firmNumber, int periodNumber, int referenceId, double quantity)
	{
		var baseQuery = @$"UPDATE LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE 
		SET AMOUNT={quantity},
		TOTAL = PRICE * {quantity},
		VATAMNT =((PRICE * {quantity}) * VAT) /100,
		VATMATRAH = PRICE * {quantity},
		LINENET = (PRICE * {quantity}) + ((PRICE * {quantity}) * VAT) /100
		WHERE LOGICALREF = {referenceId}";

		return baseQuery;
	}
	private string UpdateFicheQuery(int firmNumber, int periodNumber, int referenceId,int transactionNumber,int isEDispatch)
	{
		var baseQuery = @$"UPDATE LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE SET TRCODE = {transactionNumber}, EDESPATCH = {isEDispatch} WHERE LOGICALREF = {referenceId}";

		return baseQuery;
	}

	private string DeleteLine(int firmNumber, int periodNumber, int referenceId)
	{
		var baseQuery = @$"DELETE FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE WHERE LOGICALREF = {referenceId}";

		return baseQuery;
	}

}
