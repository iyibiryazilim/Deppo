using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Models
{
    public class Notification
    {
        public Guid Oid { get; set; }
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}