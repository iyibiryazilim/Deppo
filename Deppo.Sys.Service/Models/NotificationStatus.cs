using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Models
{
    public class NotificationStatus
    {
        public Notification? Notification { get; set; }

        public ApplicationUser? ApplicationUser { get; set; }

        public Boolean IsRead { get; set; }
    }
}