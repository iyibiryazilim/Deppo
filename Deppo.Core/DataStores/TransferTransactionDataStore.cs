using Deppo.Core.DataResultModel;
using Deppo.Core.DTOs.TransferTransaction;
using Deppo.Core.ResponseResultModels;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace Deppo.Core.DataStores
{
    public class TransferTransactionDataStore : ITransferTransactionService
    {
		public async Task<DataResult<ResponseModel>> InsertTransferTransaction(HttpClient httpClient, TransferTransactionInsert dto, int firmNumber)
        {

            var postUrl = $"/gateway/product/TransferTransaction/Tiger?firmNumber={firmNumber}";


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
                    Debug.WriteLine($"Received non-successful HTTP status code: {responseMessage.StatusCode}");
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

		public async Task<DataResult<ResponseModel>> DeleteTransferTransaction(HttpClient httpClient, int referenceId, int firmNumber)
		{
			var postUrl = $"/gateway/product/TransferTransaction/Tiger/Delete?firmNumber={firmNumber}";
			DataResult<ResponseModel> dataResult = new DataResult<ResponseModel>();


			try
            {
                var jsonBody = new StringContent(
                    System.Text.Json.JsonSerializer.Serialize(new { referenceId, firmNumber }),
                    Encoding.UTF8,
                    "application/json"
                );

                var request = new HttpRequestMessage(HttpMethod.Post, postUrl)
                {
                    Content = jsonBody
				};

                var responseMessage = await httpClient.SendAsync(request);

                if (responseMessage.IsSuccessStatusCode)
                {
                    var data = await responseMessage.Content.ReadAsStringAsync();
                    var dtos = data.Trim('"').Replace("\\\"", "\"").Replace("\\", "");
                    var dtos2 = JsonConvert.DeserializeObject<DataResult<ResponseModel>>(dtos);
                    dataResult.Message = dtos2.Message;
                    dataResult.IsSuccess = true;
                    dataResult.Data = dtos2.Data;
                    return dataResult;
                }
                else
                {
                    var message = await responseMessage.Content.ReadAsStringAsync();
                    Debug.WriteLine($"Received non-successful HTTP status code: {responseMessage.StatusCode}");
                    dataResult.Message = message;
                    dataResult.IsSuccess = false;
                    return dataResult;
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

		public async Task<DataResult<dynamic>> UpdateDocumentTrackingNumber(HttpClient httpClient, int firmNumber, int periodNumber, string ficheNumber, int ficheReferenceId)
        {
            var postUrl = $"/gateway/customQuery/CustomQuery";
            var content = new StringContent(JsonConvert.SerializeObject(UpdateQuery(firmNumber, periodNumber, ficheNumber, ficheReferenceId)), Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = await httpClient.PostAsync(postUrl, content);
            DataResult<dynamic> dataResult = new DataResult<dynamic>();

            if (responseMessage.IsSuccessStatusCode)
            {
                var data = await responseMessage.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(data))
                {
                    var result = JsonConvert.DeserializeObject<DataResult<dynamic>>(data);
                    dataResult.Data = result?.Data;
                    dataResult.IsSuccess = true;
                    dataResult.Message = "success";
                }
                else
                {
                    var result = JsonConvert.DeserializeObject<DataResult<Dictionary<string, object>>>(data);
                    dataResult.Data = result?.Data;
                    dataResult.IsSuccess = true;
                    dataResult.Message = "empty";
                }
            }
            else
            {
                dataResult.Data = Enumerable.Empty<dynamic>();
                dataResult.IsSuccess = false;
                dataResult.Message = await responseMessage.Content.ReadAsStringAsync();
            }

            return dataResult;
        }

        private string UpdateQuery(int firmNumber, int periodNumber,string ficheNumber,int ficheReferenceId)
        {
            string baseQuery = $@"UPDATE LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE SET DOCTRACKINGNR = '{ficheNumber}' WHERE LOGICALREF = {ficheReferenceId}";

            return baseQuery;
        }
    }
}
