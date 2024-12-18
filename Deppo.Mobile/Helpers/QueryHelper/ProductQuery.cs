using Deppo.Core.BaseModels;
using System;

namespace Deppo.Mobile.Helpers.QueryHelper;

public static class ProductQuery
{

    /// <summary>
    /// Malzeme Giriþ Hareketlerini Listeler
    /// </summary>
    /// <param name="FirmNumber">Firma Numarasý</param>
    /// <param name="PeriodNumber">Dönem Numarasý</param>
    /// <param name="ProductReferenceId">Malzeme referans numarasý</param>
    /// <param name="Sorting">Sýralama ASC / DESC</param>
    /// <param name="Skip">Sayfa Baþlangýcý</param>
    /// <param name="Take">Sayfa Sayýsý</param>
    /// <returns></returns>
    public static string InputTransactionListQuery(int FirmNumber, int PeriodNumber, int ProductReferenceId, string Sorting = "DESC", int Skip = 0, int Take = 20, string ExternalDb = "")
    {
        string basequery = $@"SELECT
		[ReferenceId] = STLINE.LOGICALREF,
        [TransactionDate] = STLINE.DATE_,
        [TransactionTime] = dbo.LG_INTTOTIME(STFICHE.FTIME),
		[BaseTransactionReferenceId] = STFICHE.LOGICALREF,
        [BaseTransactionCode] = STFICHE.FICHENO,
        [TransactionType] = STLINE.TRCODE,
        [ProductReferenceId] = STLINE.STOCKREF,
        [ProductCode] = ITEMS.CODE,
        [ProductName] = ITEMS.NAME,
        [SubUnitsetCode] = ISNULL(SUBUNITSET.CODE, ''),
        [SubUnitsetReferenceId] = ISNULL(SUBUNITSET.LOGICALREF, 0),
        [UnitsetCode] = ISNULL(UNITSET.CODE, ''),
        [UnitsetReferenceId] = ISNULL(UNITSET.LOGICALREF, 0),
        [Quantity] = STLINE.AMOUNT,
        [IOType] = STLINE.IOCODE,
        [WarehouseNumber] = CAPIWHOUSE.NR,
        [WarehouseName] = CAPIWHOUSE.NAME
        FROM LG_{FirmNumber.ToString().PadLeft(3, '0')}_{PeriodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
        LEFT JOIN LG_{FirmNumber.ToString().PadLeft(3, '0')}_{PeriodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE ON STLINE.STFICHEREF = STFICHE.LOGICALREF
        LEFT JOIN LG_{FirmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON STLINE.STOCKREF = ITEMS.LOGICALREF
		LEFT JOIN LG_{FirmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON STLINE.CLIENTREF = CLCARD.LOGICALREF
        LEFT JOIN LG_{FirmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON STLINE.UOMREF = SUBUNITSET.LOGICALREF
        LEFT JOIN LG_{FirmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON STLINE.USREF = UNITSET.LOGICALREF
		LEFT JOIN {ExternalDb}L_CAPIWHOUSE AS CAPIWHOUSE ON STLINE.SOURCEINDEX = CAPIWHOUSE.NR AND CAPIWHOUSE.FIRMNR = {FirmNumber}
		WHERE STLINE.IOCODE IN (1,2) AND ITEMS.LOGICALREF = {ProductReferenceId}";

        if (!string.IsNullOrEmpty(Sorting))
            basequery += $" ORDER BY STLINE.DATE_ {Sorting}";

        return basequery += $"\nOFFSET {Skip} ROWS FETCH NEXT {Take} ROWS ONLY";
    }

	/// <summary>
	/// Malzeme Çýkýþ Hareketlerini Listeler
	/// </summary>
	/// <param name="FirmNumber">Firma Numarasý</param>
	/// <param name="PeriodNumber">Dönem Numarasý</param>
	/// <param name="ProductReferenceId">Malzeme referans numarasý</param>
	/// <param name="Sorting">Sýralama ASC / DESC</param>
	/// <param name="Skip">Sayfa Baþlangýcý</param>
	/// <param name="Take">Sayfa Sayýsý</param>
	/// <returns></returns>
	public static string OutputTransactionListQuery(int FirmNumber, int PeriodNumber, int ProductReferenceId, string Sorting = "DESC", int Skip = 0, int Take = 20, string ExternalDb = "")
	{
		string basequery = $@"SELECT
		[ReferenceId] = STLINE.LOGICALREF,
        [TransactionDate] = STLINE.DATE_,
        [TransactionTime] = dbo.LG_INTTOTIME(STFICHE.FTIME),
		[BaseTransactionReferenceId] = STFICHE.LOGICALREF,
        [BaseTransactionCode] = STFICHE.FICHENO,
        [TransactionType] = STLINE.TRCODE,
        [ProductReferenceId] = STLINE.STOCKREF,
        [ProductCode] = ITEMS.CODE,
        [ProductName] = ITEMS.NAME,
        [SubUnitsetCode] = ISNULL(SUBUNITSET.CODE, ''),
        [SubUnitsetReferenceId] = ISNULL(SUBUNITSET.LOGICALREF, 0),
        [UnitsetCode] = UNITSET.CODE,
        [UnitsetReferenceId] = UNITSET.LOGICALREF,
        [Quantity] = STLINE.AMOUNT,
        [IOType] = STLINE.IOCODE,
        [WarehouseNumber] = CAPIWHOUSE.NR,
        [WarehouseName] = CAPIWHOUSE.NAME
        FROM LG_{FirmNumber.ToString().PadLeft(3, '0')}_{PeriodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
        LEFT JOIN LG_{FirmNumber.ToString().PadLeft(3, '0')}_{PeriodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE ON STLINE.STFICHEREF = STFICHE.LOGICALREF
        LEFT JOIN LG_{FirmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON STLINE.STOCKREF = ITEMS.LOGICALREF
		LEFT JOIN LG_{FirmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON STLINE.CLIENTREF = CLCARD.LOGICALREF
        LEFT JOIN LG_{FirmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON STLINE.UOMREF = SUBUNITSET.LOGICALREF
        LEFT JOIN LG_{FirmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON STLINE.USREF = UNITSET.LOGICALREF
		LEFT JOIN {ExternalDb}L_CAPIWHOUSE AS CAPIWHOUSE ON STLINE.SOURCEINDEX = CAPIWHOUSE.NR AND CAPIWHOUSE.FIRMNR = {FirmNumber}
		WHERE STLINE.IOCODE IN (3,4) AND ITEMS.LOGICALREF = {ProductReferenceId}";

		if (!string.IsNullOrEmpty(Sorting))
			basequery += $" ORDER BY STLINE.DATE_ {Sorting}";

		return basequery += $"\nOFFSET {Skip} ROWS FETCH NEXT {Take} ROWS ONLY";
	}
}
