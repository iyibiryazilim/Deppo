using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System.Text;

namespace Deppo.Core.DataStores
{
    public class ProductDetailDataStore : IProductDetailService
    {
        private string postUrl = "/gateway/customQuery/CustomQuery";

        public async Task<DataResult<IEnumerable<dynamic>>> GetLastFichesByProduct(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetLastFichesByProductQuery(firmNumber, periodNumber, productReferenceId)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<IEnumerable<dynamic>>> GetLastTransaction(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId, int productReferenceId)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetLastProductTransactionQuery(firmNumber, periodNumber, ficheReferenceId, productReferenceId)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<dynamic>> GetProductMeasure(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetProductMeasureQuery(firmNumber, periodNumber, productReferenceId)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<dynamic>> GetInputQuantity(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetInputQuantityQuery(firmNumber, periodNumber, productReferenceId)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<dynamic>> GetOutputQuantity(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetOutputQuery(firmNumber, periodNumber, productReferenceId)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<IEnumerable<dynamic>>> ProductInputOutputQuantities(HttpClient httpClient, int firmNumber, int periodNumber, DateTime dateTime, int productReferenceId)
        {
            var content = new StringContent(JsonConvert.SerializeObject(ProductInputOutputQuery(firmNumber, periodNumber, dateTime, productReferenceId)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<dynamic>> GetFirstStockQuantityAsync(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetFirstStockQuantityQuery(firmNumber, periodNumber, productReferenceId)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<dynamic>> GetLastStockQuantityAsync(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetLastStockQuantityQuery(firmNumber, periodNumber, productReferenceId)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<dynamic>> GetSalesQuantityAsync(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetSalesQuantityQuery(firmNumber, periodNumber, productReferenceId)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<dynamic>> GetPurchaseQuantityAsync(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, int month)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetPurchaseQuantityQuery(firmNumber, periodNumber, productReferenceId, month)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<dynamic>> UpdateProductMeasure(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, double width, double height, double length, double weight, double volume, int subunitsetId)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetUpdateProductMeasureQuery(firmNumber, periodNumber, productReferenceId, width, height, length, weight, volume, subunitsetId)), Encoding.UTF8, "application/json");

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

        private string GetLastFichesByProductQuery(int firmNumber, int periodNumber, int productReferenceId)
        {
            string baseQuery = $@"SELECT TOP 5
            [ReferenceId] = STFICHE.LOGICALREF,
			[FicheType] = STFICHE.TRCODE,
			[FicheNumber] = STFICHE.FICHENO,
            [FicheDate] = STFICHE.DATE_,
            [FicheTime] = dbo.LG_INTTOTIME(STFICHE.FTIME),
			[DocumentNumber] =ISNULL (STFICHE.DOCODE , ''),
			[SpecialCode] = ISNULL  (STFICHE.SPECODE , ''),
			[CurrentReferenceId] = ISNULL (CLCARD.LOGICALREF, 0),
			[CurrentCode] = ISNULL (CLCARD.CODE , '' ),
			[CurrentName] = ISNULL ( CLCARD.DEFINITION_ ,''),
			[WarehouseName] =  ISNULL (CAPIWHOUSE.NAME , ''),
			[WarehouseNumber] = ISNULL( CAPIWHOUSE.NR, 0),
			[Description] =  ISNULL (STFICHE.GENEXP1, '')
			From LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE WITH(NOLOCK) ON STLINE.STFICHEREF = STFICHE.LOGICALREF
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON CLCARD.LOGICALREF = STFICHE.CLIENTREF
			LEFT JOIN L_CAPIWHOUSE AS CAPIWHOUSE on CAPIWHOUSE.NR = STFICHE.SOURCEINDEX AND CAPIWHOUSE.FIRMNR = 1
			WHERE STLINE.STOCKREF = {productReferenceId} AND STFICHE.PRODSTAT = 0
			GROUP BY STFICHE.LOGICALREF,STFICHE.TRCODE,STFICHE.FICHENO,STFICHE.DATE_,dbo.LG_INTTOTIME(STFICHE.FTIME),STFICHE.DOCODE,STFICHE.SPECODE ,CLCARD.LOGICALREF,CLCARD.CODE ,
			CLCARD.DEFINITION_,CAPIWHOUSE.NAME ,CAPIWHOUSE.NR,STFICHE.GENEXP1
			ORDER BY STFICHE.DATE_ DESC";

            return baseQuery;
        }

        private string GetProductMeasureQuery(int firmNumber, int periodNumber, int productReferenceId)
        {
            string baseQuery = $@"SELECT
            [Width]=ITMUNITA.WIDTH,
            [Length]=ITMUNITA.LENGTH,
            [Height]=ITMUNITA.HEIGHT,
            [Weight]=ITMUNITA.WEIGHT,
            [Volume]=ITMUNITA.VOLUME_
            FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_ITMUNITA AS ITMUNITA
            LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON ITMUNITA.ITEMREF = ITEMS.LOGICALREF
            LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL ON ITMUNITA.UNITLINEREF= UNITSETL.LOGICALREF
            WHERE ITEMREF = {productReferenceId} AND UNITSETL.MAINUNIT = 1 ";

            return baseQuery;
        }

        private string GetLastProductTransactionQuery(int firmNumber, int periodNumber, int ficheReferenceId, int productReferenceId)
        {
            string baseQuery = $@"SELECT
        [ReferenceId] = STLINE.LOGICALREF,
        [TransactionDate] = STLINE.DATE_,
        [TransactionTime] = dbo.LG_INTTOTIME(STFICHE.FTIME),
		[BaseTransactionReferenceId] = STFICHE.LOGICALREF,
        [BaseTransactionCode] = STFICHE.FICHENO,
        [TransactionType] = STLINE.TRCODE,
        [ProductReferenceId] = STLINE.STOCKREF,
        [ProductCode] = ITEMS.CODE,
        [ProductName] = ITEMS.NAME,
        [SubUnitsetCode] = SUBUNITSET.CODE,
        [SubUnitsetName] = SUBUNITSET.NAME,
        [SubUnitsetReferenceId] = SUBUNITSET.LOGICALREF,
        [UnitsetCode] = UNITSET.CODE,
        [UnitsetReferenceId] = UNITSET.LOGICALREF,
		[UnitsetName] = UNITSET.NAME,
        [Quantity] = STLINE.AMOUNT,
        [Description] = STLINE.LINEEXP,
        [Image] = FIRMDOC.LDATA,
        [IOType] = STLINE.IOCODE,
        [WarehouseNumber] = CAPIWHOUSE.NR,
        [WarehouseName] = CAPIWHOUSE.NAME

        FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE ON STLINE.STFICHEREF = STFICHE.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON STLINE.STOCKREF = ITEMS.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON STLINE.CLIENTREF = CLCARD.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON STLINE.UOMREF = SUBUNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC ON FIRMDOC.INFOREF = ITEMS.LOGICALREF AND FIRMDOC.INFOTYP = 20
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON STLINE.USREF = UNITSET.LOGICALREF
		LEFT JOIN L_CAPIWHOUSE AS CAPIWHOUSE ON STLINE.SOURCEINDEX = CAPIWHOUSE.NR AND CAPIWHOUSE.FIRMNR = {firmNumber}
		WHERE  STFICHE.LOGICALREF = {ficheReferenceId} AND STLINE.STOCKREF={productReferenceId} AND  STFICHE.PRODSTAT = 0 AND STLINE.LPRODSTAT = 0
		ORDER BY STLINE.DATE_ DESC";

            return baseQuery;
        }

        private string ProductInputOutputQuery(int firmNumber, int periodNumber, DateTime dateTime, int productReferenceId)
        {
            string baseQuery = $@"";
            DateTime xDate = dateTime;
            for (int i = 1; i < 6; i++)
            {
                if (i != 1)
                    xDate = xDate.AddDays(-1);

                if (i != 5)
                {
                    baseQuery += $@"
SELECT
[Argument] = '{xDate.ToString("dddd")}',
[ArgumentDay] = {xDate.Day.ToString().PadLeft(2, '0')},
[InputQuantity] = ISNULL((SELECT SUM(STLINE.AMOUNT) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK) WHERE IOCODE IN(1,2) AND LINETYPE = 0 AND YEAR(STLINE.DATE_) = {xDate.Year} AND MONTH(STLINE.DATE_) = {xDate.Month} AND DAY(STLINE.DATE_) = {xDate.Day} AND STLINE.STOCKREF = {productReferenceId}),0),
[OutputQuantity] = ISNULL((SELECT SUM(STLINE.AMOUNT) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK) WHERE IOCODE IN(3,4) AND LINETYPE = 0 AND YEAR(STLINE.DATE_) = {xDate.Year} AND MONTH(STLINE.DATE_) = {xDate.Month} AND DAY(STLINE.DATE_) = {xDate.Day} AND STLINE.STOCKREF = {productReferenceId}),0)
UNION All ";
                }
                else
                {
                    baseQuery += $@"SELECT
[Argument] = '{xDate.ToString("dddd")}',
[ArgumentDay] = {xDate.Day.ToString().PadLeft(2, '0')},
[InputQuantity] = ISNULL((SELECT SUM(STLINE.AMOUNT) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK) WHERE IOCODE IN(1,2) AND LINETYPE = 0 AND YEAR(STLINE.DATE_) = {xDate.Year} AND MONTH(STLINE.DATE_) = {xDate.Month} AND DAY(STLINE.DATE_) = {xDate.Day} AND STLINE.STOCKREF = {productReferenceId}),0),
[OutputQuantity] = ISNULL((SELECT SUM(STLINE.AMOUNT) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK) WHERE IOCODE IN(3,4) AND LINETYPE = 0 AND YEAR(STLINE.DATE_) = {xDate.Year} AND MONTH(STLINE.DATE_) = {xDate.Month} AND DAY(STLINE.DATE_) = {xDate.Day} AND STLINE.STOCKREF = {productReferenceId}),0)";
                }
            }

            return baseQuery;
        }

        private string GetInputQuantityQuery(int firmNumber, int periodNumber, int productReferenceId)
        {
            string baseQuery = $@"SELECT
                    [InputQuantity] = (SELECT ISNULL(SUM(AMOUNT), 0) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE WHERE IOCODE IN(1, 2) AND STOCKREF = {productReferenceId})";

            return baseQuery;
        }

        private string GetOutputQuery(int firmNumber, int periodNumber, int productReferenceId)
        {
            string baseQuery = $@"SELECT
                    [OutputQuantity] = (SELECT ISNULL(SUM(AMOUNT), 0) FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE WHERE IOCODE IN(3, 4) AND STOCKREF = {productReferenceId})";

            return baseQuery;
        }

        private string GetFirstStockQuantityQuery(int firmNumber, int periodNumber, int productReferenceId)
        {
            string baseQuery = $@"SELECT ISNULL(SUM(ONHAND),0) FirstStockQuantity FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT WHERE STOCKREF = {productReferenceId} AND INVENNO = -1  AND DATE_ ='{DateTime.Now.Year}-01-01' ";

            return baseQuery;
        }

        private string GetLastStockQuantityQuery(int firmNumber, int periodNumber, int productReferenceId)
        {
            string baseQuery = $@"SELECT ISNULL(SUM(ONHAND),0) LastStockQuantity FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT WHERE STOCKREF = {productReferenceId} AND INVENNO = -1 AND YEAR(DATE_) = YEAR(GETDATE()) ";

            return baseQuery;
        }

        private string GetSalesQuantityQuery(int firmNumber, int periodNumber, int productReferenceId)
        {
            string baseQuery = $@"SELECT ISNULL(SUM(AMOUNT),0) SalesQuantity FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE
WHERE STOCKREF = {productReferenceId} AND YEAR(DATE_) = YEAR(GETDATE()) AND (
    TRCODE IN (7, 8, 11, 12, 51, 4)
    OR (TRCODE = 25 AND CLIENTREF > 0 AND IOCODE = 3)
)";

            return baseQuery;
        }

        private string GetPurchaseQuantityQuery(int firmNumber, int periodNumber, int productReferenceId, int month)
        {
            string baseQuery = $@"SELECT ISNULL(SUM(AMOUNT),0) PurchaseQuantity FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE
WHERE STOCKREF = {productReferenceId} AND YEAR(DATE_) = YEAR(GETDATE())
  AND MONTH(DATE_) = {month} AND TRCODE IN (1,5)";

            return baseQuery;
        }

        private string GetUpdateProductMeasureQuery(int firmNumber, int periodNumber, int productReferenceId, double width, double height, double length, double weight, double volume, int subunitsetId)
        {
            string baseQuery = $@"UPDATE  LG_{firmNumber.ToString().PadLeft(3, '0')}_ITMUNITA
            SET

            [WIDTH]={width},
            [HEIGHT]={height},
            [LENGTH]={length},
            [WEIGHT]={weight},
            [VOLUME_]={volume}
            FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_ITMUNITA AS ITMUNITA
            LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON ITMUNITA.ITEMREF = ITEMS.LOGICALREF
            LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL ON ITMUNITA.UNITLINEREF= UNITSETL.LOGICALREF
            WHERE ITEMREF = {productReferenceId} AND UNITSETL.MAINUNIT = 1 AND UNITLINEREF={subunitsetId} ";

            return baseQuery;
        }
    }
}