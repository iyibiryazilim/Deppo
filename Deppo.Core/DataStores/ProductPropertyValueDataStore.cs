using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DataStores
{
    public class ProductPropertyValueDataStore : IProductPropertyValueService
    {
        private string postUrl = "gateway/customQuery/CustomQuery";

        public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int productPropertyReferenceId, string search = "", int skip = 0, int take = 20)
        {
            var content = new StringContent(JsonConvert.SerializeObject(ProductPropertyValueQuery(firmNumber, periodNumber,productPropertyReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

        private string ProductPropertyValueQuery(int firmNumber, int periodNumber,int productPropertyReferenceId, string search = "", int skip = 0, int take = 20)
        {
            string baseQuery = $@"
    SELECT 
    [ReferenceId] = CHARCODE.LOGICALREF,
    [Code] = CHARCODE.CODE,
    [Name] = CHARCODE.NAME,
    [ProductReferenceId] = ITEMS.LOGICALREF,
    [ProductName] = ITEMS.NAME,
    [ProductCode] = ITEMS.CODE,
    [IsActive] = CHARCODE.ACTIVE,
    [ProductPropertyReferenceId] = CHARVAL.LOGICALREF,
    [ProductPropertyCode] = CHARVAL.CODE,
	[ProductPropertyName]=CHARVAL.NAME,
	[ValueNumber]=CHARVAL.VALNO
FROM 
    LG_{firmNumber.ToString().PadLeft(3, '0')}_CHARASGN AS CHARASGN WITH(NOLOCK)
LEFT JOIN 
    LG_{firmNumber.ToString().PadLeft(3, '0')}_CHARCODE AS CHARCODE WITH(NOLOCK) ON CHARASGN.CHARCODEREF = CHARCODE.LOGICALREF
LEFT JOIN 
    LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON CHARASGN.ITEMREF = ITEMS.LOGICALREF
LEFT JOIN 
    LG_{firmNumber.ToString().PadLeft(3, '0')}_CHARVAL AS CHARVAL WITH(NOLOCK) ON CHARVAL.CHARCODEREF = CHARCODE.LOGICALREF
WHERE CHARVAL.LOGICALREF={productPropertyReferenceId};";

            if (!string.IsNullOrEmpty(search))
                baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";

            baseQuery += $@" ORDER BY ITEMS.CODE ASC
            OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

            return baseQuery;
        }
    }
}
