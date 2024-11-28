using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System.Text;

namespace Deppo.Core.DataStores
{
    public class ProductAnalysisDataStore : IProductAnalysisService
    {
        private string postUrl = "/gateway/customQuery/CustomQuery";

        public async Task<DataResult<dynamic>> GetInStockProductCountAsync(HttpClient httpClient, int firmNumber, int periodNumber)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetInStockProductCountQuery(firmNumber,periodNumber)), Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = await httpClient.PostAsync(postUrl, content);
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

        public async Task<DataResult<dynamic>> GetOutStockProductCountAsync(HttpClient httpClient, int firmNumber, int periodNumber)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetOutStockProductCountQuery(firmNumber,periodNumber)), Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = await httpClient.PostAsync(postUrl, content);
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

        public async Task<DataResult<dynamic>> GetTotalProductCountAsync(HttpClient httpClient, int firmNumber)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetTotalProductCountQuery(firmNumber)), Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = await httpClient.PostAsync(postUrl, content);
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

        public async Task<DataResult<dynamic>> GetNegativeStockProductsCountAsync(HttpClient httpClient, int firmNumber, int periodNumber)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetNegativeStockProductsCountQuery(firmNumber, periodNumber)), Encoding.UTF8, "application/json");

            HttpResponseMessage responseMessage = await httpClient.PostAsync(postUrl, content);
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

		public async Task<DataResult<IEnumerable<dynamic>>> InputOutputProductReferenceAnalysis(HttpClient httpClient, int firmNumber, int periodNumber, DateTime dateTime)
		{
			var content = new StringContent(JsonConvert.SerializeObject(InputOutputProductReferenceAnalysisQuery(firmNumber, periodNumber, dateTime)), Encoding.UTF8, "application/json");

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

		private string GetInStockProductCountQuery(int firmNumber,int periodNumber)
        {
            string baseQuery = $@"SELECT 
[InStockProductCount] = ISNULL(COUNT(DD.ProductReferenceId),0) 
FROM (select 
	[ProductReferenceId] = STINVTOT.STOCKREF
	from LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT
	LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON STINVTOT.STOCKREF = ITEMS.LOGICALREF
	WHERE STINVTOT.INVENNO = -1 AND ITEMS.ACTIVE = 0
	GROUP BY STINVTOT.STOCKREF,ITEMS.ACTIVE
	 HAVING SUM(STINVTOT.ONHAND) > 0) DD";

            return baseQuery;
        }

        private string GetOutStockProductCountQuery(int firmNumber,int periodNumber)
        {
            string baseQuery = $@"SELECT 
[OutStockProductCount] = ISNULL(COUNT(DD.ProductReferenceId),0) 
FROM (select 
	[ProductReferenceId] = STINVTOT.STOCKREF
	from LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT
	LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON STINVTOT.STOCKREF = ITEMS.LOGICALREF
	WHERE STINVTOT.INVENNO = -1 AND ITEMS.ACTIVE = 0
	GROUP BY STINVTOT.STOCKREF,ITEMS.ACTIVE
	 HAVING SUM(STINVTOT.ONHAND) <= 0) DD";

            return baseQuery;
        }

        private string GetTotalProductCountQuery(int firmNumber)
        {
            string baseQuery = $@"SELECT [TotalProductCount] = ISNULL(COUNT(LOGICALREF),0) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS WHERE ACTIVE = 0";

            return baseQuery;
        }

        private string GetNegativeStockProductsCountQuery(int firmNumber, int periodNumber)
        {
            string baseQuery = $@"SELECT
    ISNULL(COUNT(*),0) AS NegativeStockProductQuantity
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS
WHERE
    (SELECT ISNULL(SUM(ONHAND), 0) FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT WHERE STOCKREF = ITEMS.LOGICALREF AND INVENNO = -1) < 0";

            return baseQuery;
        }

		private string InputOutputProductReferenceAnalysisQuery(int firmNumber, int periodNumber, DateTime dateTime)
		{
			string baseQuery = $@"";
			DateTime xDate = dateTime;
			for (int i = 1; i < 7; i++)
			{
				if (i != 1)
					xDate = xDate.AddMonths(-1);

				if (i != 6)
					baseQuery += $@"
SELECT 
[Argument] = '{xDate.ToString("MMMM")}',
[ArgumentMonth] = {xDate.Month.ToString().PadLeft(2, '0')},
[InputReferenceCount] = ISNULL((SELECT COUNT(DISTINCT STLINE.STOCKREF) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK) WHERE IOCODE IN(1,2) AND LINETYPE = 0 AND YEAR(STLINE.DATE_) = {xDate.Year} AND MONTH(STLINE.DATE_) = {xDate.Month}),0),
[OutputReferenceCount] = ISNULL((SELECT COUNT(DISTINCT STLINE.STOCKREF) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK) WHERE IOCODE IN(3,4) AND LINETYPE = 0 AND YEAR(STLINE.DATE_) = {xDate.Year} AND MONTH(STLINE.DATE_) = {xDate.Month}),0)
UNION All ";
				else
				{
					baseQuery += $@"
SELECT 
[Argument] = '{xDate.ToString("MMMM")}',
[ArgumentMonth] = {xDate.Month.ToString().PadLeft(2, '0')},
[InputReferenceCount] = ISNULL((SELECT COUNT(DISTINCT STLINE.STOCKREF) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK) WHERE IOCODE IN(1,2) AND LINETYPE = 0 AND YEAR(STLINE.DATE_) = {xDate.Year} AND MONTH(STLINE.DATE_) = {xDate.Month}),0),
[OutputReferenceCount] = ISNULL((SELECT COUNT(DISTINCT STLINE.STOCKREF) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK) WHERE IOCODE IN(3,4) AND LINETYPE = 0 AND YEAR(STLINE.DATE_) = {xDate.Year} AND MONTH(STLINE.DATE_) = {xDate.Month}),0)";
				}
			}

			return baseQuery;
		}

	}
}
