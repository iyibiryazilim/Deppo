using Deppo.Core.DataResultModel;
using Deppo.Mobile.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Core.DataStores;

public class PurchaseSupplierDataStore : IPurchaseSupplierService
{
    private string postUrl = "/gateway/customQuery/CustomQuery";

    private string PurchaseSupplierQuery(int firmNumber, int periodNumber, string search = "", int skip = 0, int take = 20)
    {
        string baseQuery = $@"SELECT
    [SupplierReferenceId] = CLCARD.LOGICALREF,
    [SupplierCode] = CLCARD.CODE,
    [SupplierName] = CLCARD.DEFINITION_,
    [TotalQuantity] = SUM(ORFLINE.AMOUNT),
    [TotalShippedQuantity] = SUM(ORFLINE.SHIPPEDAMOUNT),
    [TotalWaitingQuantity] = SUM(ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT)
FROM LG_001_02_ORFLINE AS ORFLINE
LEFT JOIN LG_001_02_ORFICHE AS ORFICHE
    ON ORFLINE.ORDFICHEREF = ORFICHE.LOGICALREF
LEFT JOIN LG_001_CLCARD AS CLCARD
    ON ORFICHE.CLIENTREF = CLCARD.LOGICALREF
LEFT JOIN LG_001_ITEMS AS ITEMS
    ON ORFLINE.STOCKREF = ITEMS.LOGICALREF
LEFT JOIN LG_001_UNITSETL AS SUBUNITSET
    ON ORFLINE.UOMREF = SUBUNITSET.LOGICALREF AND MAINUNIT = 1
LEFT JOIN LG_001_UNITSETF AS UNITSET
    ON ORFLINE.USREF = UNITSET.LOGICALREF
WHERE ORFLINE.CLOSED = 0
    AND (ORFLINE.AMOUNT - ORFLINE.SHIPPEDAMOUNT) > 0
    AND ORFLINE.TRCODE = 1
    AND ITEMS.UNITSETREF <> ";

        if (!string.IsNullOrEmpty(search))
            baseQuery += $@" AND (CLCARD.CODE LIKE '{search}%' OR CLCARD.DEFINITION_ LIKE '%{search}%')";

        baseQuery += $@" GROUP BY CLCARD.LOGICALREF, CLCARD.CODE, CLCARD.DEFINITION_
ORDER BY CLCARD.DEFINITION_ ASC
OFFSET {skip} ROWS
FETCH NEXT {take} ROWS ONLY";

        return baseQuery;
    }
}