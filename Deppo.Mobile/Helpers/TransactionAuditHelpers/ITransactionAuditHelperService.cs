namespace Deppo.Mobile.Helpers.TransactionAuditHelpers;

public interface ITransactionAuditHelperService
{
    public Task InsertProducTransactionAuditAsync(int firmNumber, int periodNumber, int ioType, int transactionType, DateTime transactionDate, int transactionReferenceId, string transactionNumber, string documentNumber, int warehouseNumber, string warehouseName, int productReferenceCount);

    public Task InsertSalesTransactionAuditAsync(int firmNumber, int periodNumber, int ioType, int transactionType, DateTime transactionDate, int transactionReferenceId, string transactionNumber, string documentNumber, int warehouseNumber, string warehouseName, int productReferenceCount, int currentReferenceId, string currentCode, string currentName, int shipAddressReferenceId = default, string shipAddressCode = "", string shipAddressName = "");

    public Task InsertPurchaseTransactionAuditAsync(int firmNumber, int periodNumber, int ioType, int transactionType, DateTime transactionDate, int transactionReferenceId, string transactionNumber, string documentNumber, int warehouseNumber, string warehouseName, int productReferenceCount, int currentReferenceId, string currentCode, string currentName, int shipAddressReferenceId = default, string shipAddressCode = "", string shipAddressName = "");
}