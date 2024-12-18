using System;

namespace Deppo.Mobile.Helpers.QueryHelper;

public static class CustomerQuery
{
    /// <summary>
    /// Müşteri Giriş Hareketlerini Listeler
    /// </summary>
    /// <param name="FirmNumber">Firma Numarası</param>
    /// <param name="PeriodNumber">Dönem Numarası</param>
    /// <param name="CustomerReferenceId">Müşteri Referans Id</param>
    /// <param name="Sorting">Sıralama ASC / DESC</param>
    /// <param name="CurrentIndex">Sayfa Başlangıcı</param>
    /// <param name="PageSize">Sayfa Sayısı</param>
    /// <returns></returns>
    public static string InputTransactionListQuery(int FirmNumber, int PeriodNumber, int CustomerReferenceId, string Sorting = "DESC", int CurrentIndex = 0, int PageSize = 20, string ExternalDb = "")
    {
        string baseQuery = @$"SELECT
        [ReferenceId] = STLINE.LOGICALREF,
        [TransactionDate] = STLINE.DATE_,
        [TransactionTime] = dbo.LG_INTTOTIME(STFICHE.FTIME),
		[BaseTransactionReferenceId] = STFICHE.LOGICALREF,
        [BaseTransactionCode] = STFICHE.FICHENO,
        [TransactionType] = STLINE.TRCODE,
        [SubUnitsetCode] = ISNULL(SUBUNITSET.CODE,''),
        [SubUnitsetReferenceId] = ISNULL(SUBUNITSET.LOGICALREF,0),
        [UnitsetCode] = UNITSET.CODE,
        [UnitsetReferenceId] = UNITSET.LOGICALREF,
        [Quantity] = STLINE.AMOUNT,
        [IOType] = STLINE.IOCODE,
        [WarehouseName] = CAPIWHOUSE.NAME,
		[CustomerReferenceId] = CLCARD.LOGICALREF,
		[CustomerCode] = CLCARD.CODE,
		[CustomerName] = CLCARD.DEFINITION_,
        [ProductName]=ITEMS.NAME,
        [ProductCode]=ITEMS.CODE
        FROM LG_{FirmNumber.ToString().PadLeft(3, '0')}_{PeriodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
        LEFT JOIN LG_{FirmNumber.ToString().PadLeft(3, '0')}_{PeriodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE ON STLINE.STFICHEREF = STFICHE.LOGICALREF
        LEFT JOIN LG_{FirmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON STLINE.STOCKREF = ITEMS.LOGICALREF
		LEFT JOIN LG_{FirmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON STLINE.CLIENTREF = CLCARD.LOGICALREF
        LEFT JOIN LG_{FirmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON STLINE.UOMREF = SUBUNITSET.LOGICALREF
        LEFT JOIN LG_{FirmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON STLINE.USREF = UNITSET.LOGICALREF
		LEFT JOIN {ExternalDb}L_CAPIWHOUSE AS CAPIWHOUSE ON STLINE.SOURCEINDEX = CAPIWHOUSE.NR AND CAPIWHOUSE.FIRMNR = {FirmNumber}
		WHERE STLINE.IOCODE IN (1,2) AND CLCARD.LOGICALREF = {CustomerReferenceId}";

        if (!string.IsNullOrEmpty(Sorting))
            baseQuery += $" ORDER BY STLINE.DATE_ {Sorting}";

        return baseQuery += $"\nOFFSET {CurrentIndex} ROWS FETCH NEXT {PageSize} ROWS ONLY";
    }

    /// <summary>
    /// Müşteri Çıkış Hareketlerini Listeler
    /// </summary>
    /// <param name="FirmNumber">Firma Numarası</param>
    /// <param name="PeriodNumber">Dönem Numarası</param>
    /// <param name="CustomerReferenceId">Müşteri Referans Id</param>
    /// <param name="Sorting">Sıralama ASC / DESC</param>
    /// <param name="Skip">Sayfa Başlangıcı</param>
    /// <param name="Take">Sayfa Sayısı</param>
    /// <returns></returns>
    public static string OutputTransactionListQuery(int FirmNumber, int PeriodNumber, int CustomerReferenceId, string Sorting = "DESC", int Skip = 0, int Take = 20, string ExternalDb = "")
    {
        string baseQuery = @$"SELECT
        [ReferenceId] = STLINE.LOGICALREF,
        [TransactionDate] = STLINE.DATE_,
        [TransactionTime] = dbo.LG_INTTOTIME(STFICHE.FTIME),
		[BaseTransactionReferenceId] = STFICHE.LOGICALREF,
        [BaseTransactionCode] = STFICHE.FICHENO,
        [TransactionType] = STLINE.TRCODE,
        [SubUnitsetCode] = ISNULL(SUBUNITSET.CODE,''),
        [SubUnitsetName] = ISNULL(SUBUNITSET.NAME,''),
        [SubUnitsetReferenceId] = ISNULL(SUBUNITSET.LOGICALREF,0),
        [UnitsetReferenceId] = ISNULL(UNITSET.LOGICALREF, 0),
        [UnitsetCode] = ISNULL(UNITSET.CODE, ''),
        [UnitsetName] = ISNULL(UNITSET.NAME, ''),
        [Quantity] = STLINE.AMOUNT,
        [IOType] = STLINE.IOCODE,
        [WarehouseName] = CAPIWHOUSE.NAME,
		[CustomerReferenceId] = CLCARD.LOGICALREF,
		[CustomerCode] = CLCARD.CODE,
		[CustomerName] = CLCARD.DEFINITION_,
        [ProductName]=ITEMS.NAME,
        [ProductCode]=ITEMS.CODE
        FROM LG_{FirmNumber.ToString().PadLeft(3, '0')}_{PeriodNumber.ToString().PadLeft(2, '0')}_STLINE AS STLINE
        LEFT JOIN LG_{FirmNumber.ToString().PadLeft(3, '0')}_{PeriodNumber.ToString().PadLeft(2, '0')}_STFICHE AS STFICHE ON STLINE.STFICHEREF = STFICHE.LOGICALREF
        LEFT JOIN LG_{FirmNumber.ToString().PadLeft(3, '0')}_ITEMS AS ITEMS ON STLINE.STOCKREF = ITEMS.LOGICALREF
		LEFT JOIN LG_{FirmNumber.ToString().PadLeft(3, '0')}_CLCARD AS CLCARD ON STLINE.CLIENTREF = CLCARD.LOGICALREF
        LEFT JOIN LG_{FirmNumber.ToString().PadLeft(3, '0')}_UNITSETL AS SUBUNITSET ON STLINE.UOMREF = SUBUNITSET.LOGICALREF
        LEFT JOIN LG_{FirmNumber.ToString().PadLeft(3, '0')}_UNITSETF AS UNITSET ON STLINE.USREF = UNITSET.LOGICALREF
		LEFT JOIN {ExternalDb}L_CAPIWHOUSE AS CAPIWHOUSE ON STLINE.SOURCEINDEX = CAPIWHOUSE.NR AND CAPIWHOUSE.FIRMNR = {FirmNumber}
		WHERE STLINE.IOCODE IN (3,4) AND CLCARD.LOGICALREF = {CustomerReferenceId}";

        if (!string.IsNullOrEmpty(Sorting))
            baseQuery += $" ORDER BY STLINE.DATE_ {Sorting}";

        return baseQuery += $"\nOFFSET {Skip} ROWS FETCH NEXT {Take} ROWS ONLY";
    }
}