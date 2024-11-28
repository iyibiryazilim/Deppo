using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.DTOs;

public class ReasonsForRejectionDto
{
    public string Code { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}