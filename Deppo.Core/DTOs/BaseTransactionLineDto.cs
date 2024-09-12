using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DTOs;

public class BaseTransactionLineDto
{
    
        public BaseTransactionLineDto()
    {
        SeriLotTransactions = new List<SeriLotTransactionDto>();
    }

    public IList<SeriLotTransactionDto> SeriLotTransactions { get; set; }
    public string? ProductCode { get; set; } = string.Empty;
    public double? Quantity { get; set; }
    public string? SubUnitsetCode { get; set; } = string.Empty;
    public int? WarehouseNumber { get; set; }
    public string? Description { get; set; } = string.Empty;
    public string? SpeCode { get; set; } = string.Empty;
    public double? ConversionFactor { get; set; }
    public double? OtherConversionFactor { get; set; }
    public double? UnitPrice { get; set; } = default;
    public double? VatRate { get; set; } = default;
}

