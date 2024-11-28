using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Models;

public class TaskList
{
    public Guid Oid { get; set; }
    public string Subject { get; set; } = string.Empty;
    public DateTime? CreatedOn { get; set; }

    public ApplicationUser? User { get; set; }

    public DateTime? StartOn { get; set; }

    public string Priority { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;
}