using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.DTOs.SeriLotTransactionDto
{
    public class SeriLotTransactionDto
    {
        public string? StockLocationCode { get; set; } = string.Empty;

        public string? DestinationStockLocationCode { get; set; } = string.Empty;

        public int? InProductTransactionLineReferenceId { get; set; } = 0;

        public int? OutProductTransactionLineReferenceId { get; set; } = 0;

        public short? SerilotType { get; set; } = default;

        public double? Quantity { get; set; } = default;

        public string? SubUnitsetCode { get; set; } = string.Empty;

        public double? ConversionFactor { get; set; } = default;

        public double? OtherConversionFactor { get; set; } = default;
    }
}
