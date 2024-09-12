using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DTOs.BaseTransaction;

public class BaseTransactionDto
{
    public string? Code { get; set; }
    public int FirmNumber { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.Now;
    public int WarehouseNumber { get; set; } = 0;
    public string Description { get; set; } = string.Empty;
    public string SpeCode { get; set; } = string.Empty;
    public string DoCode { get; set; } = string.Empty;
    public string DocTrackingNumber { get; set; } = string.Empty;
    public string CurrentCode { get; set; } = string.Empty;
}
