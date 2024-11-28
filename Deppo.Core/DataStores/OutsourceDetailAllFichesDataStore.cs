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
    public class OutsourceDetailAllFichesDataStore : IOutsourceDetailAllFichesService
    {
        private string postUrl = "/gateway/customQuery/CustomQuery";

        public async Task<DataResult<IEnumerable<dynamic>>> GetAllFiches(HttpClient httpClient, int firmNumber, int periodNumber, int outsourceReferenceId, string search = "", int skip = 0, int take = 20)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetAllFichesByOutsourceQuery(firmNumber, periodNumber, outsourceReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<IEnumerable<dynamic>>> GetTransactionsByFiche(HttpClient httpClient, int firmNumber, int periodNumber, int ficheReferenceId, int skip = 0, int take = 20)
        {
            var content = new StringContent(JsonConvert.SerializeObject(GetTransactionsByFicheQuery(firmNumber, periodNumber, ficheReferenceId, skip, take)), Encoding.UTF8, "application/json");

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

        private string GetAllFichesByOutsourceQuery(int firmNumber, int periodNumber, int outsourceReferenceId, string search, int skip, int take)
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
			WHERE STFICHE.TRCODE IN (25) AND STFICHE.PRODSTAT = 0 AND CLCARD.LOGICALREF={outsourceReferenceId}
			ORDER BY STFICHE.DATE_ DESC  OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

            return baseQuery;
        }

        private string GetTransactionsByFicheQuery(int firmNumber, int periodNumber, int ficheReferenceId, int skip = 0, int take = 20)
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
        [IOType] = STLINE.IOCODE,
        [WarehouseNumber] = CAPIWHOUSE.NR,
        [WarehouseName] = CAPIWHOUSE.NAME

        FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE ON STLINE.STFICHEREF = STFICHE.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON STLINE.STOCKREF = ITEMS.LOGICALREF
		LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON STLINE.CLIENTREF = CLCARD.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON STLINE.UOMREF = SUBUNITSET.LOGICALREF
        LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON STLINE.USREF = UNITSET.LOGICALREF
		LEFT JOIN L_CAPIWHOUSE AS CAPIWHOUSE ON STLINE.SOURCEINDEX = CAPIWHOUSE.NR AND CAPIWHOUSE.FIRMNR = {firmNumber}
		WHERE STFICHE.GRPCODE = 3 AND STFICHE.PRODSTAT = 0 AND STLINE.LPRODSTAT = 0 AND STFICHE.LOGICALREF = {ficheReferenceId}
		ORDER BY STLINE.DATE_ DESC OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";
            return baseQuery;
        }
    }
}