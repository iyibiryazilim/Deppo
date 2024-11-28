using Deppo.Core.DataResultModel;
using Deppo.Core.Services;
using Newtonsoft.Json;
using System.Text;

namespace Deppo.Core.DataStores;

public class SupplierDetailAllFicheDataStore : ISupplierDetailAllFicheService
{
    private string postUrl = "/gateway/customQuery/CustomQuery";

    public async Task<DataResult<IEnumerable<dynamic>>> GetAllFichesBySupplier(HttpClient httpClient, int firmNumber, int periodNumber, int supplierReferenceId, string search = "", int skip = 0, int take = 20)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetAllFichesBySupplierQuery(firmNumber, periodNumber, supplierReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

    public async Task<DataResult<IEnumerable<dynamic>>> GetTransactionsByFiche(HttpClient httpClient, int firmNumber, int periodNumber, int ficheRefenceId)
    {
        var content = new StringContent(JsonConvert.SerializeObject(GetTransactionsByFicheQuery(firmNumber, periodNumber, ficheRefenceId)), Encoding.UTF8, "application/json");

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

    private string GetAllFichesBySupplierQuery(int firmNumber, int periodNumber, int supplierReferenceId, string search, int skip, int take)
    {
        string baseQuery = $@"SELECT
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
			From LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE
			left join LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON CLCARD.LOGICALREF = STFICHE.CLIENTREF
			LEFT JOIN L_CAPIWHOUSE AS CAPIWHOUSE on CAPIWHOUSE.NR = STFICHE.SOURCEINDEX AND CAPIWHOUSE.FIRMNR = {firmNumber}
			WHERE STFICHE.PRODSTAT = 0 AND CLCARD.LOGICALREF={supplierReferenceId}";

        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" AND (STFICHE.FICHENO LIKE '%{search}%' OR STFICHE.DOCODE LIKE '%{search}%')";

        baseQuery += $@" ORDER BY STFICHE.DATE_ DESC
                OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }

    private string GetTransactionsByFicheQuery(int firmNumber, int periodNumber, int ficheReferenceId)
    {
        string baseQuery = $@"SELECT
            [ReferenceId] = STLINE.LOGICALREF,
            [TransactionDate] = STLINE.DATE_,
            [TransactionTime] = dbo.LG_INTTOTIME(STLINE.FTIME),
			[SupplierReferenceId] = ISNULL(CLCARD.LOGICALREF,0),
			[SupplierCode] = ISNULL(CLCARD.CODE,''),
			[SupplierName] = ISNULL(CLCARD.DEFINITION_,''),
			[BaseTransactionReferenceId] = STFICHE.LOGICALREF,
			[BaseTransactionCode] = STFICHE.FICHENO,
			[TransactionType] = STLINE.TRCODE,
			[GroupCode] = STFICHE.GRPCODE,
			[IOType] = STLINE.IOCODE,
			[ProductReferenceId] =  ITEMS.LOGICALREF,
			[ProductCode] = ISNULL(ITEMS.CODE,''),
			[ProductName] = ISNULL(ITEMS.NAME,''),
			[UnitsetReferenceId] = ISNULL(UNITSET.LOGICALREF,0),
			[UnitsetCode] = ISNULL(UNITSET.CODE, ''),
			[UnitsetName] =  ISNULL(UNITSET.NAME , ''),
			[SubUnitsetReferenceId] = ISNULL(SUBUNITSET.LOGICALREF, 0),
			[SubUnitsetCode] = ISNULL(SUBUNITSET.CODE, ''),
			[SubUnitsetName] = ISNULL(SUBUNITSET.NAME, ''),
			[WarehouseName] = CAPIWHOUSE.NAME,
			[WarehouseNumber] = CAPIWHOUSE.NR,
			[Quantity] = ISNULL(STLINE.AMOUNT,0),
			[Length] =  ISNULL( STLINE.UINFO4 , 0),
			[Width] = ISNULL(STLINE.UINFO5, 0),
			[Height] = ISNULL(STLINE.UINFO6, 0),
			[Weight] = ISNULL(STLINE.UINFO7, 0),
			[Volume] = ISNULL(STLINE.UINFO8, 0),
			[Barcode] = '',
            [Image] = FIRMDOC.LDATA
FROM  LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE WITH(NOLOCK)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE WITH(NOLOCK) ON STLINE.STFICHEREF = STFICHE.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON STLINE.STOCKREF = ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD WITH(NOLOCK) ON STLINE.CLIENTREF = CLCARD.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC ON FIRMDOC.INFOREF = ITEMS.LOGICALREF AND FIRMDOC.INFOTYP = 20
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET WITH(NOLOCK) ON STLINE.UOMREF = SUBUNITSET.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET WITH(NOLOCK) ON STLINE.USREF = UNITSET.LOGICALREF
LEFT JOIN L_CAPIWHOUSE AS CAPIWHOUSE WITH(NOLOCK) ON STLINE.SOURCEINDEX = CAPIWHOUSE.NR AND CAPIWHOUSE.FIRMNR = {firmNumber}
WHERE STLINE.LINETYPE = 0 AND STFICHE.LOGICALREF = {ficheReferenceId}
ORDER BY STLINE.DATE_ DESC";

        return baseQuery;
    }
}