using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System.Text;

namespace Deppo.Core.DataStores
{
    public class ProcurementSalesProductDataStore : IProcurementSalesProductService
    {
        private string postUrl = "/gateway/customQuery/CustomQuery";
        public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId, string search = "", int skip = 0, int take = 20)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetLinesQuery(firmNumber, periodNumber, ficheReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

        private string GetLinesQuery(int firmNumber, int periodNumber, int ficheReferenceId, string search = "", int skip = 0, int take = 20)
        {
            string baseQuery = $@"Select
     [ReferenceId] = STLINE.LOGICALREF,
     [TransactionDate] = STLINE.DATE_,
     [TransactionTime] = dbo.LG_INTTOTIME(STLINE.FTIME),
	 [BaseTransactionReferenceId] = STFICHE.LOGICALREF,
	 [BaseTransactionCode] = STFICHE.FICHENO,
	 [MainItemReferenceId] = CASE WHEN VARIANT.LOGICALREF IS NOT NULL AND VARIANT.LOGICALREF <> 0 THEN ITEMS.LOGICALREF ELSE 0 END,
	 [MainItemCode] = CASE WHEN VARIANT.CODE IS NOT NULL AND VARIANT.CODE <> '' THEN ITEMS.CODE ELSE '' END,
	 [MainItemName] = CASE WHEN VARIANT.NAME IS NOT NULL AND VARIANT.NAME <> '' THEN ITEMS.NAME ELSE '' END,
	 [ItemReferenceId] = CASE WHEN VARIANT.LOGICALREF IS NULL OR VARIANT.LOGICALREF = 0 THEN ITEMS.LOGICALREF ELSE VARIANT.LOGICALREF END,
	 [ItemCode] = CASE WHEN VARIANT.CODE IS NULL OR VARIANT.CODE = '' THEN ISNULL(ITEMS.CODE, '') ELSE VARIANT.CODE END,
	 [ItemName] = CASE WHEN VARIANT.NAME IS NULL OR VARIANT.NAME = '' THEN ISNULL(ITEMS.NAME, '') ELSE VARIANT.NAME END,
	 [UnitsetReferenceId] = ISNULL( unitset.LOGICALREF,0),
	 [UnitsetCode] = ISNULL( unitset.CODE , ''),
	 [UnitsetName] =  ISNULL (unitset.NAME , ''),
	 [SubUnitsetReferenceId] = ISNULL ( subunitset.LOGICALREF, 0),
	 [SubUnitsetCode] = ISNULL (subunitset.CODE,''),
	 [SubUnitsetName] = ISNULL (subunitset.NAME , ''),
	 [Quantity] = ISNULL (STLINE.AMOUNT,0)

from  LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE as STLINE
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE on STLINE.STFICHEREF = STFICHE.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS on STLINE.STOCKREF = ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_VARIANT AS VARIANT ON STLINE.VARIANTREF = VARIANT.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD as CLCARD ON STLINE.CLIENTREF = CLCARD.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS subunitset ON STLINE.UOMREF = subunitset.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS unitset ON STLINE.USREF = unitset.LOGICALREF
LEFT JOIN L_CAPIWHOUSE AS capiwhouse ON STLINE.SOURCEINDEX = capiwhouse.NR AND capiwhouse.FIRMNR = {firmNumber}
where STFICHE.LOGICALREF = {ficheReferenceId} AND STLINE.IOCODE = 2";

            if (!string.IsNullOrEmpty(search))
                baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%' OR VARIANT.CODE LIKE '{search}%' OR VARIANT.NAME LIKE '%{search}%')";

            baseQuery += $@" ORDER BY STLINE.DATE_ DESC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

            return baseQuery;
        }
    }
}
