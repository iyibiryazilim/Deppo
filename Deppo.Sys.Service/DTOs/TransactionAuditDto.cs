namespace Deppo.Sys.Service.DTOs
{
    public class TransactionAuditDto
    {
        public Guid ApplicationUserOid { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public int TransactionReferenceId { get; set; } = default;
        public string TransactionNumber { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;

        public int CurrentReferenceId { get; set; } = default;
        public string CurrentName { get; set; } = string.Empty;
        public string CurrentCode { get; set; } = string.Empty;

        public int ShipAddressReferenceId { get; set; } = default;
        public string ShipAddressName { get; set; } = string.Empty;
        public string ShipAddressCode { get; set; } = string.Empty;

        public int WarehouseNumber { get; set; } = default;
        public string WarehouseName { get; set; } = string.Empty;
        public int ProductReferenceCount { get; set; } = default;

        public int FirmNumber { get; set; } = default;
        public int PeriodNumber { get; set; } = default;
        public int IOType { get; set; } = default;
        public int TransactionType { get; set; } = default;
    }
}