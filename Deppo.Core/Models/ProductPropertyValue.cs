using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Models;

public class ProductPropertyValue
{
    public int ReferenceId { get; set; }
    public int ProductReferenceId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int ProductPropertyReferenceId { get; set; }
    public string ProductPropertyCode { get; set; } = string.Empty;
    public string ProductPropertyName { get; set; } = string.Empty;
    public int ValueNumber { get; set; } = default;

}
