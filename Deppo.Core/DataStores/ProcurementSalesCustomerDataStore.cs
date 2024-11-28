using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System.Text;

namespace Deppo.Core.DataStores
{
    public class ProcurementSalesCustomerDataStore : IProcurementSalesCustomerService
    {
        private string postUrl = "/gateway/customQuery/CustomQuery";
        public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, string search = "", int skip = 0, int take = 20)
        {
            var content = new StringContent(JsonConvert.SerializeObject(CustomerQuery(firmNumber, periodNumber,warehouseNumber, search, skip, take)), Encoding.UTF8, "application/json");

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

        private string CustomerQuery(int firmNumber, int periodNumber, int warehouseNumber,string search = "", int skip = 0, int take = 20)
        {
            string baseQuery = $@"SELECT
    [ReferenceId] = STFICHE.LOGICALREF,
    [FicheNumber] = STFICHE.FICHENO,
    [FicheDate] = STFICHE.DATE_,
    [FicheTime] = dbo.LG_INTTOTIME(STFICHE.FTIME),
    [DocumentNumber] = STFICHE.DOCODE,
    [DocumentTrackingNumber] = STFICHE.DOCTRACKINGNR,
    [SpeCode] = STFICHE.SPECODE,
    [Description] = STFICHE.GENEXP1,
    [CustomerReferenceId] = CLCARD.LOGICALREF,
    [CustomerCode] = CLCARD.CODE,
    [CustomerName] = CLCARD.DEFINITION_,
    CASE
        WHEN CLCARD.ACCEPTEDESP = 0 THEN 0
        ELSE 1
    END AS [IsEDispatch],
    [ProductReferenceCount] = COUNT(DISTINCT STLINE.STOCKREF),
    [Country] = CLCARD.COUNTRY,
    [City] = CLCARD.CITY,
	[ShipAddressReferenceId] = ISNULL(STFICHE.SHIPINFOREF,0),
	[ShipAddressCode] = ISNULL(SHIPADRESS.CODE,''),
    [ShipAddressName] = ISNULL(SHIPADRESS.NAME,'')
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON STFICHE.CLIENTREF = CLCARD.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE ON STLINE.STFICHEREF = STFICHE.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_SHIPINFO AS SHIPADRESS ON STFICHE.SHIPINFOREF = SHIPADRESS.LOGICALREF
WHERE
   STFICHE.TRCODE = 8 AND STFICHE.SOURCEINDEX = {warehouseNumber} AND 
   CLCARD.SUBCONT = 0 AND CLCARD.LOGICALREF IS NOT NULL AND STFICHE.STATUS = 1 AND STFICHE.EDESPATCH = 0";

            if (!string.IsNullOrEmpty(search))
                baseQuery += $@" AND (CLCARD.CODE LIKE '{search}%' OR CLCARD.DEFINITION_ LIKE '%{search}%')";

            baseQuery += $@" GROUP BY CLCARD.LOGICALREF, CLCARD.CODE, CLCARD.DEFINITION_, CLCARD.COUNTRY, CLCARD.CITY,CLCARD.ACCEPTEDESP,STFICHE.SHIPINFOREF, SHIPADRESS.CODE,SHIPADRESS.NAME, STFICHE.LOGICALREF,STFICHE.DOCODE,STFICHE.DOCTRACKINGNR,STFICHE.FICHENO,STFICHE.DATE_,STFICHE.FTIME,STFICHE.SPECODE,STFICHE.GENEXP1,SHIPADRESS.ADDR1,SHIPADRESS.CITY,SHIPADRESS.COUNTRY
ORDER BY CLCARD.DEFINITION_ ASC
OFFSET {skip} ROWS
FETCH NEXT {take} ROWS ONLY";

            return baseQuery;
        }
    }
}
