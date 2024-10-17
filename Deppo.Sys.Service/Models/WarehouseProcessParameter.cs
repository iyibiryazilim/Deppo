namespace Deppo.Sys.Service.Models;

public class WarehouseProcessParameter
{
    public Guid Oid { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public string OutProcessType { get; set; } = string.Empty;
}