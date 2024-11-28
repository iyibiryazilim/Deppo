using Deppo.Core.DataResultModel;
using Deppo.Core.DTOs.ProductionTransaction;
using Deppo.Core.ResponseResultModels;
using Deppo.Core.Services;
using Newtonsoft.Json;  // Ensure Newtonsoft.Json is used for serialization
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DataStores
{
    public class ProductionTransactionDataStore : IProductionTransactionService
    {
       //private string postUrl = "/gateway/product/ProductionTransaction/Tiger";

        public async Task<DataResult<ResponseModel>> InsertProductionTransaction(HttpClient httpClient, ProductionTransactionInsert dto, int firmNumber)
        {
          
        var  postUrl = $"/gateway/product/ProductionTransaction/Tiger?firmNumer={firmNumber}";
            
            

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
    }
}
