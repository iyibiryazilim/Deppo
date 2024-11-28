using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Models
{
    public class ProcurementFiche
    {
        public int ReferenceId { get; set; }

        public DateTime CreatedOn { get; set; }

        public string  FicheNumber { get; set; }=string.Empty; 

        public Customer? Customer { get; set; }



    }
}
