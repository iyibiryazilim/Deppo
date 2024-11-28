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
    public class SalesDispatchTransactionDataStore : ISalesDispatchTransactionService
    {
        public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int customerReferenceId, string search = "", int skip = 0, int take = 20)
        {
            var postUrl = $"/gateway/customQuery/CustomQuery";
            var content = new StringContent(JsonConvert.SerializeObject(GetSalesDispatchQuery(firmNumber, periodNumber, warehouseNumber, customerReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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
        public async Task<DataResult<IEnumerable<dynamic>>> GetTransactionsByFicheReferenceId(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId, string search = "", int skip = 0, int take = 20)
        {
            var postUrl = $"/gateway/customQuery/CustomQuery";
            var content = new StringContent(JsonConvert.SerializeObject(GetTrancationsByFicheReferenceIdQuery(firmNumber, periodNumber, ficheReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

        private string GetTrancationsByFicheReferenceIdQuery(int firmNumber, int periodNumber, int ficheReferenceId, string search = "", int skip = 0, int take = 20)
        {
            string baseQuery = $@"Select
            [ReferenceId] = STLINE.LOGICALREF,
            [TransactionDate] = STLINE.DATE_,
            [TransactionTime] = dbo.LG_INTTOTIME(STLINE.FTIME),
     [CustomerReferenceId] = CLCARD.LOGICALREF,
	 [CustomerCode] = CLCARD.CODE,
     [CustomerName] = CLCARD.DEFINITION_,
	 [BaseTransactionReferenceId] = STFICHE.LOGICALREF,
	 [BaseTransactionCode] = STFICHE.FICHENO,
     [TransactionType] = STLINE.TRCODE,
	 [GroupCode] = STFICHE.GRPCODE,
	 [IOType] = STLINE.IOCODE,
	 [ProductReferenceId] =  ITEMS.LOGICALREF,
	 [ProductCode] = ISNULL(ITEMS.CODE,''),
	 [ProductName] = ISNULL(ITEMS.NAME,''),
     [Image] = ISNULL(FIRMDOC.LDATA,''),
     [IsVariant] = ITEMS.CANCONFIGURE,
     [TrackingType] = ITEMS.TRACKTYPE,
     [LocTracking] = ITEMS.LOCTRACKING,
	 [UnitsetReferenceId] = ISNULL( unitset.LOGICALREF,0),
	 [UnitsetCode] = ISNULL( unitset.CODE , ''),
	 [UnitsetName] =  ISNULL (unitset.NAME , ''),
	 [SubUnitsetReferenceId] = ISNULL ( subunitset.LOGICALREF, 0),
	 [SubUnitsetCode] = ISNULL (subunitset.CODE,''),
	 [SubUnitsetName] = ISNULL (subunitset.NAME , ''),
	 [WarehouseName] = capiwhouse.NAME,
     [WarehouseNumber] = capiwhouse.NR,
	 [Quantity] = ISNULL ( STLINE.AMOUNT,0),
	 [Length] =  ISNULL ( STLINE.UINFO4 , 0),
	 [Width] = ISNULL  (STLINE.UINFO5,0),
	 [Height] = ISNULL ( STLINE.UINFO6,0),
	 [Weight] = ISNULL  ( STLINE.UINFO7 , 0),
	 [Volume] = ISNULL  (STLINE.UINFO8 , 0),
	 [Barcode] = ''
from  LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE as STLINE
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE on STLINE.STFICHEREF = STFICHE.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS on STLINE.STOCKREF = ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC ON FIRMDOC.INFOREF = ITEMS.LOGICALREF AND FIRMDOC.INFOTYP = 20  AND FIRMDOC.DOCNR = 11
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD as CLCARD ON STLINE.CLIENTREF = CLCARD.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS subunitset ON STLINE.UOMREF = subunitset.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS unitset ON STLINE.USREF = unitset.LOGICALREF
LEFT JOIN L_CAPIWHOUSE AS capiwhouse ON STLINE.SOURCEINDEX = capiwhouse.NR AND capiwhouse.FIRMNR = {firmNumber}
where STFICHE.TRCODE IN(7,8) AND STLINE.LINETYPE = 0 AND STFICHE.LOGICALREF = {ficheReferenceId}
";

            if (!string.IsNullOrEmpty(search))
                baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%' OR ITEMS.NAME LIKE '%{search}%')";

            baseQuery += $@" ORDER BY ITEMS.LOGICALREF DESC 
OFFSET {skip} ROWS
FETCH NEXT {take} ROWS ONLY";

            return baseQuery;
        }
    



    private string GetSalesDispatchQuery(int firmNumber, int periodNumber, int warehouseNumber, int customerReferenceId, string search, int skip, int take)
        {
            string baseQuery = $@"Select
            [ReferenceId] = STFICHE.LOGICALREF,
			[FicheType] = STFICHE.TRCODE,
			[FicheNumber] = STFICHE.FICHENO,
            [FicheDate] = STFICHE.DATE_,
            [FicheTime] = dbo.LG_INTTOTIME(STFICHE.FTIME),
			[DocumentNumber] =ISNULL (STFICHE.DOCODE , ''),
			[DocumentDate] =  STFICHE.DOCDATE,
			[SpecialCode] = ISNULL  ( STFICHE.SPECODE , ''),
			[CurrentReferenceID] = ISNULL ( CLCARD.LOGICALREF, 0),
			[CurrentCode] = ISNULL (CLCARD.CODE , '' ),
			[CurrentName] = ISNULL ( CLCARD.DEFINITION_ ,''),
			[WarehouseName] =  ISNULL (CAPIWHOUSE.NAME , ''),
			[WarehouseNumber] = ISNULL( CAPIWHOUSE.NR, 0),
			[Description] =  ISNULL (STFICHE.GENEXP1, ''),
            [ShipAddressReferenceId] = SHIPINFO.LOGICALREF,
			[ShipAddressCode] = SHIPINFO.CODE,
			[ShipAddressName] = SHIPINFO.NAME
			From LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON CLCARD.LOGICALREF = STFICHE.CLIENTREF
			LEFT JOIN L_CAPIWHOUSE AS CAPIWHOUSE ON CAPIWHOUSE.NR = STFICHE.SOURCEINDEX AND CAPIWHOUSE.FIRMNR = {firmNumber}
			LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_SHIPINFO AS SHIPINFO ON STFICHE.SHIPINFOREF = SHIPINFO.LOGICALREF
			WHERE STFICHE.TRCODE IN (7,8) AND STFICHE.SOURCEINDEX = {warehouseNumber} AND STFICHE.CLIENTREF = {customerReferenceId} ";

            if (!string.IsNullOrEmpty(search))
                baseQuery += $@" AND (STFICHE.FICHENO LIKE '%{search}%' OR STFICHE.GENEXP1 LIKE '%{search}%')";

            baseQuery += $@"
ORDER BY STFICHE.DATE_ DESC
OFFSET {skip} ROWS
FETCH NEXT {take} ROWS ONLY";

            return baseQuery;
        }
    }
}

