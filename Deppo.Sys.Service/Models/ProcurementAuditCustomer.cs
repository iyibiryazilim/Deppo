using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Models;

public class ProcurementAuditCustomer : ProcurementAudit
{
    public int CurrentReferenceId { get; set; }
    public string CurrentCode { get; set; } = string.Empty;
    public string CurrentName { get; set; } = string.Empty;
}