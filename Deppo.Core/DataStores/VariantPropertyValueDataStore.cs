using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DataStores;

public class VariantPropertyValueDataStore : IVariantPropertyValueService
{
    private string postUrl = "/gateway/customQuery/CustomQuery";

    public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int variantPropertReferenceId, int varyantRef ,string search = "", int skip = 0, int take = 20)
    {

        var content = new StringContent(JsonConvert.SerializeObject(VariantPropertysQuery(firmNumber, periodNumber,variantPropertReferenceId, varyantRef, search, skip, take)), Encoding.UTF8, "application/json");

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

    private string VariantPropertysQuery(int firmNumber, int periodNumber, int variantPropertReferenceId, int varyantRef, string search = "", int skip = 0, int take = 20)
    {
        string baseQuery = $@"  select
 [ReferenceId] = CHARVAL.LOGICALREF , 
 [Code] = CHARVAL.CODE , 
 [Name] = CHARVAL.NAME ,
 [VariantPropertyReferenceId] = CHARVAL.CHARCODEREF,
 [ValNo] = CHARVAL.VALNO,
 VRNTC.VARIANTREF
 from LG_{firmNumber.ToString().PadLeft(3, '0')}_CHARVAL as CHARVAL
Left JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VRNTCHARASGN as VRNTC ON VRNTC.CHARVALREF = CHARVAL.LOGICALREF
 where CHARVAL.CHARCODEREF = {variantPropertReferenceId} AND VRNTC.VARIANTREF = {varyantRef} "
;

        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" AND (CHARCODE.CODE LIKE '{search}%' OR CHARCODE.NAME LIKE '%{search}%')";

        baseQuery += $@" ORDER BY CHARCODE.NAME DESC
OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";


        return baseQuery;
    }

}
