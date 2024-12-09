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
    public class OutputOutsourceTransferV2DataStore : IOutputOutsourceTransferV2Service
    {
        private string postUrl = "/gateway/customQuery/CustomQuery";

        public async Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int currentReferenceId, string search = "", int skip = 0, int take = 20)
        {
            var content = new StringContent(JsonConvert.SerializeObject(OutsourceProductQuery(firmNumber, periodNumber, warehouseNumber, currentReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

        public async Task<DataResult<IEnumerable<dynamic>>> GetObjectSubProducts(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20)
        {
            var content = new StringContent(JsonConvert.SerializeObject(OutsourceSubProductQuery(firmNumber, periodNumber,productReferenceId, search, skip, take)), Encoding.UTF8, "application/json");

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

        private string OutsourceProductQuery(int firmNumber, int periodNumber, int warehouseNumber,int currentReferenceId, string search = "", int skip = 0, int take = 20)
        {
            string baseQuery = $@"SELECT 
[ReferenceId]  =DISPLINE.LOGICALREF,
[ProductionReferenceId] = PRODORD.LOGICALREF,
[ProductReferenceId] = ITEMS.LOGICALREF,
[ProductCode] = ITEMS.CODE,
[ProductName] = ITEMS.NAME,
[UnitsetReferenceId] = UNITSETF.LOGICALREF,
[UnitsetCode] = UNITSETF.CODE,
[UnitsetName] = UNITSETF.NAME,
[SubUnitsetReferenceId] = UNITSETL.LOGICALREF,
[SubUnitsetCode] = UNITSETL.CODE,
[SubUnitsetName] = UNITSETL.NAME,
[OutsourceReferenceId] = CLCARD.LOGICALREF,
[OutsourceCode] = CLCARD.CODE,
[OutsourceName] = CLCARD.DEFINITION_,
[OperationReferenceId] = OPR.LOGICALREF,
[OperationCode] = OPR.CODE,
[OperationName] = OPR.NAME,
[PlanningQantity] = PRODORD.PLNAMOUNT,
[ActualQuantity] = PRODORD.ACTAMOUNT,
[WarehouseNumber] = POLINE.INVENNO,
[WarehouseName] = '',
[StockQuantity] = ISNULL((SELECT SUM(ONHAND) FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK) WHERE STINVTOT.STOCKREF = ITEMS.LOGICALREF AND STINVTOT.INVENNO = {warehouseNumber}),0),
[IsVariant] = ITEMS.CANCONFIGURE,
[TrackingType] = ITEMS.TRACKTYPE,
[LocTracking] = ITEMS.LOCTRACKING,
[Image] = ISNULL((SELECT TOP 1 FIRMDOC.LDATA
                 FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC
                 WHERE FIRMDOC.INFOREF = ITEMS.LOGICALREF
                 AND FIRMDOC.INFOTYP = 20
                 AND FIRMDOC.DOCNR = 11), '')
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_DISPLINE AS DISPLINE WITH(NOLOCK)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_POLINE AS POLINE WITH(NOLOCK) ON POLINE.DISPLINEREF = DISPLINE.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_PRODORD AS PRODORD WITH(NOLOCK) ON DISPLINE.PRODORDREF = PRODORD.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON DISPLINE.ITEMREF = ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON POLINE.UOMREF = UNITSETL.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON POLINE.USREF = UNITSETF.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD WITH(NOLOCK) ON DISPLINE.CLIENTREF = CLCARD.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_OPERTION AS OPR WITH(NOLOCK) ON DISPLINE.OPERATIONREF = OPR.LOGICALREF
LEFT JOIN L_CAPIWHOUSE AS WAREHOUSE WITH(NOLOCK) ON POLINE.INVENNO = WAREHOUSE.NR AND WAREHOUSE.FIRMNR = {firmNumber}
WHERE DISPLINE.PRODORDTYP = 2 AND DISPLINE.LINESTATUS = 0 AND POLINE.INVENNO={warehouseNumber} AND CLCARD.LOGICALREF={currentReferenceId}"  ;

            if (!string.IsNullOrEmpty(search))
                baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%'
                        OR ITEMS.NAME LIKE '%{search}%')";

            baseQuery += $@" ORDER BY ITEMS.CODE DESC
                    OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

            return baseQuery;
        }

        private string OutsourceSubProductQuery(int firmNumber, int periodNumber, int productReferenceId, string search = "", int skip = 0, int take = 20)
        {
            string baseQuery = $@"SELECT 
[ReferenceId] = ITEMS.LOGICALREF,
[ProductCode] = ITEMS.CODE,
[ProductName] = ITEMS.NAME,
[OutWarehouseNumber] = OUTWHOUSE.NR,
[OutWarehouseName] = OUTWHOUSE.NAME,
[InWarehouseNumber] = INWHOUSE.NR ,
[InWarehouseName] = INWHOUSE.NAME,
[OutsourceReferenceId] = CLCARD.LOGICALREF,
[OutsourceCode] = CLCARD.CODE,
[OutsourceName] = CLCARD.DEFINITION_,
[BOMQuantity] = BOMLINE.AMOUNT,
[UnitsetReferenceId]=UNITSETF.LOGICALREF,
[UnitsetName]=UNITSETF.NAME,
[UnitsetCode]=UNITSETF.CODE,
[SubUnitsetReferenceId]=UNITSETL.LOGICALREF,
[SubUnitseName]=UNITSETL.NAME,
[SubUnitsetCode]=UNITSETL.CODE,
[StokQuantity] = ISNULL((SELECT SUM(ONHAND) FROM LV_{firmNumber.ToString().PadLeft(3, '0')}_{periodNumber.ToString().PadLeft(2, '0')}_STINVTOT AS STINVTOT WITH(NOLOCK) WHERE STINVTOT.STOCKREF = ITEMS.LOGICALREF AND STINVTOT.INVENNO = CLCARD.ININVENNR),0),
[IsVariant] = ITEMS.CANCONFIGURE,
[TrackingType] = ITEMS.TRACKTYPE,
[LocTracking] = ITEMS.LOCTRACKING,
[Image] = ISNULL((SELECT TOP 1 FIRMDOC.LDATA
                 FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_FIRMDOC AS FIRMDOC
                 WHERE FIRMDOC.INFOREF = ITEMS.LOGICALREF
                 AND FIRMDOC.INFOTYP = 20
                 AND FIRMDOC.DOCNR = 11), '')
FROM LG_{firmNumber.ToString().PadLeft(3, '0')}_BOMLINE AS BOMLINE WITH(NOLOCK)
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_BOMASTER AS BOMASTER WITH(NOLOCK) ON BOMLINE.BOMMASTERREF = BOMASTER.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS WITH(NOLOCK) ON BOMLINE.ITEMREF = ITEMS.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS UNITSETL WITH(NOLOCK) ON BOMLINE.UOMREF = UNITSETL.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSETF WITH(NOLOCK) ON BOMLINE.USREF = UNITSETF.LOGICALREF
LEFT JOIN LG_{firmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD WITH(NOLOCK) ON BOMASTER.CLIENTREF = CLCARD.LOGICALREF
LEFT JOIN L_CAPIWHOUSE AS OUTWHOUSE WITH(NOLOCK) ON CLCARD.ININVENNR = OUTWHOUSE.NR AND OUTWHOUSE.FIRMNR = {firmNumber}
LEFT JOIN L_CAPIWHOUSE AS INWHOUSE WITH(NOLOCK) ON CLCARD.OUTINVENNR = INWHOUSE.NR AND INWHOUSE.FIRMNR = {firmNumber}
WHERE BOMLINE.BOMTYPE = 2 AND BOMLINE.LINETYPE <> 4 AND BOMASTER.MAINPRODREF = {productReferenceId}";

            if (!string.IsNullOrEmpty(search))
                baseQuery += $@" AND (ITEMS.CODE LIKE '{search}%'
                        OR ITEMS.NAME LIKE '%{search}%')";

            baseQuery += $@" ORDER BY ITEMS.CODE DESC
                    OFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";

            return baseQuery;
        }


    }
}
