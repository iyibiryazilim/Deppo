using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Models;

public class ReasonsForRejection
{
    public Guid Oid { get; set; }
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}