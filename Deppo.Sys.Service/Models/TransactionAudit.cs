namespace Deppo.Sys.Service.Models;

public class TransactionAudit
{
    public Guid Oid { get; set; }
    public Guid? ApplicationUserOid { get; set; }
    public ApplicationUser? ApplicationUser { get; set; }
    public DateTime? CreatedOn { get; set; }
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
    public string TransactionTypeName
    {
        get
        {

            switch (TransactionType)
            {
                case 1:
                    return "Mal Alım İrsaliyesi";
                case 2:
                    return "Perakende Satış İade İrsaliyesi";
                case 3:
                    return "Toptan Satış İade İrsaliyesi";
                case 4:
                    return "Konsinye Çıkış İade İrsaliyesi";
                case 5:
                    return "Konsinye Giriş İade İrsaliyesi";
                case 6:
                    return "Satınalma İade İrsaliyesi";
                case 7:
                    return "Perakende Satış İrsaliyesi";
                case 8:
                    return "Toptan Satış İrsaliyesi";
                case 9:
                    return "Konsinye Çıkış İrsaliyesi";
                case 10:
                    return "Konsinye Giriş İade İrsaliyesi";
                case 13:
                    return "Üretimden Giriş Fişi";
                case 14:
                    return "Devir Fişi";
                case 12:
                    return "Sarf Fişi";
                case 11:
                    return "Fire Fişi";
                case 25:
                    return "Ambar Fişi";
                case 26:
                    return "Mustahsil İrsaliyesi";
                case 50:
                    return "Sayım Fazlası Fişi";
                case 51:
                    return "Sayım Eksiği Fişi";
                default:
                    return "Diğer";
            }
        }
    }
}