using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.DTOs;

public class ProcurementAuditCustomerDto : ProcurementAuditDto
{
    public int CurrentReferenceId { get; set; }
    public string CurrentCode { get; set; } = string.Empty;
    public string CurrentName { get; set; } = string.Empty;
}