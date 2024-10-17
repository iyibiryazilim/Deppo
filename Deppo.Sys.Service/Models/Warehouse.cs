using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Models;

public class Warehouse
{
    public Guid Oid { get; set; }
    public string WarehouseNumber { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
}