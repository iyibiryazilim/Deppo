using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Core.Models;
public class ProductProperty
{
    public int ReferenceId { get; set; }
    public string Code { get; set; }=string.Empty;
    public string Name { get; set; }=string.Empty;
    public int ProductReferenceId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set;} = string.Empty;
    public bool IsActive { get; set; } 

}
