using Deppo.Mobile.Core.Models.ShipAddressModels;
using Deppo.Mobile.Helpers.HttpClientHelpers;
using Deppo.Sys.Service.Services;
using System.Diagnostics;

namespace Deppo.Mobile.Helpers.TransactionAuditHelpers;

public class TransactionAuditHelperDataStore : ITransactionAuditHelperService
{
    private readonly ITransactionAuditService _transactionAuditService;
    private readonly IHttpClientSysService _httpClientSysService;

    public TransactionAuditHelperDataStore(ITransactionAuditService transactionAuditService, IHttpClientSysService httpClientSysService)
    {
        _transactionAuditService = transactionAuditService;
        _httpClientSysService = httpClientSysService;
    }

    public async Task InsertProducTransactionAuditAsync(int firmNumber, int periodNumber, int ioType, int transactionType, DateTime transactionDate, int transactionReferenceId, string transactionNumber, string documentNumber, int warehouseNumber, string warehouseName, int productReferenceCount)
    {
        try
        {
            var httpClient = _httpClientSysService.GetOrCreateHttpClient();
            await _transactionAuditService.CreateAsync(httpClient, new Sys.Service.DTOs.TransactionAuditDto
            {
                IOType = ioType,
                TransactionType = transactionType,
                ApplicationUserOid = _httpClientSysService.UserOid,
                TransactionDate = transactionDate,
                TransactionReferenceId = transactionReferenceId,
                TransactionNumber = transactionNumber,
                DocumentNumber = documentNumber,
                WarehouseNumber = warehouseNumber,
                WarehouseName = warehouseName,
                ProductReferenceCount = productReferenceCount,
                CreatedOn = DateTime.Now,
                FirmNumber = firmNumber,
                PeriodNumber = periodNumber,
            });
        }
        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine(ex);
#endif
        }
    }

    public async Task InsertPurchaseTransactionAuditAsync(int firmNumber, int periodNumber, int ioType, int transactionType, DateTime transactionDate, int transactionReferenceId, string transactionNumber, string documentNumber, int warehouseNumber, string warehouseName, int productReferenceCount, int currentReferenceId, string currentCode, string currentName, int shipAddressReferenceId = 0, string shipAddressCode = "", string shipAddressName = "")
    {
        try
        {
            var httpClient = _httpClientSysService.GetOrCreateHttpClient();
            await _transactionAuditService.CreateAsync(httpClient, new Sys.Service.DTOs.TransactionAuditDto
            {
                IOType = ioType,
                TransactionType = transactionType,
                ApplicationUserOid = _httpClientSysService.UserOid,
                TransactionDate = transactionDate,
                TransactionReferenceId = transactionReferenceId,
                TransactionNumber = transactionNumber,
                DocumentNumber = documentNumber,
                WarehouseNumber = warehouseNumber,
                WarehouseName = warehouseName,
                ProductReferenceCount = productReferenceCount,
                CreatedOn = DateTime.Now,
                FirmNumber = firmNumber,
                PeriodNumber = periodNumber,
                CurrentReferenceId = currentReferenceId,
                CurrentCode = currentCode,
                CurrentName = currentName,
                ShipAddressReferenceId = shipAddressReferenceId,
                ShipAddressCode = shipAddressCode,
                ShipAddressName = shipAddressName,
            });
        }
        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine(ex);
#endif
        }
    }

    public async Task InsertSalesTransactionAuditAsync(int firmNumber, int periodNumber, int ioType, int transactionType, DateTime transactionDate, int transactionReferenceId, string transactionNumber, string documentNumber, int warehouseNumber, string warehouseName, int productReferenceCount, int currentReferenceId, string currentCode, string currentName, int shipAddressReferenceId = 0, string shipAddressCode = "", string shipAddressName = "")
    {
        try
        {
            var httpClient = _httpClientSysService.GetOrCreateHttpClient();
            await _transactionAuditService.CreateAsync(httpClient, new Sys.Service.DTOs.TransactionAuditDto
            {
                IOType = ioType,
                TransactionType = transactionType,
                ApplicationUserOid = _httpClientSysService.UserOid,
                TransactionDate = transactionDate,
                TransactionReferenceId = transactionReferenceId,
                TransactionNumber = transactionNumber,
                DocumentNumber = documentNumber,
                WarehouseNumber = warehouseNumber,
                WarehouseName = warehouseName,
                ProductReferenceCount = productReferenceCount,
                CreatedOn = DateTime.Now,
                FirmNumber = firmNumber,
                PeriodNumber = periodNumber,
                CurrentReferenceId = currentReferenceId,
                CurrentCode = currentCode,
                CurrentName = currentName,
                ShipAddressReferenceId = shipAddressReferenceId,
                ShipAddressCode = shipAddressCode,
                ShipAddressName = shipAddressName,
            });
        }
        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine(ex);
#endif
        }
    }
}