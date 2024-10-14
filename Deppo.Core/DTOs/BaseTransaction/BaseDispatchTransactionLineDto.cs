using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DTOs.BaseTransaction;

public class BaseDispatchTransactionLineDto
{
    public BaseDispatchTransactionLineDto()
    {
        SeriLotTransactions = new List<Deppo.Core.DTOs.SeriLotTransactionDto.SeriLotTransactionDto>();
    }
    public string? ProductCode { get; set; }
	public string? VariantCode { get; set; }
	public string? SubUnitsetCode { get; set; }
    public double? Quantity { get; set; }
    public double? UnitPrice { get; set; }
    public double? VatRate { get; set; }
    public short? WarehouseNumber { get; set; }
    public int? OrderReferenceId { get; set; }
    public string? Description { get; set; }
    public string? SpeCode { get; set; }
    public double? ConversionFactor { get; set; }
    public double? OtherConversionFactor { get; set; }

    public IList<Deppo.Core.DTOs.SeriLotTransactionDto.SeriLotTransactionDto> SeriLotTransactions { get; set; }
}
