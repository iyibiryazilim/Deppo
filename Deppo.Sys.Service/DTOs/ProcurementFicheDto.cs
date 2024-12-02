using Deppo.Sys.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.DTOs
{
    public class ProcurementFicheDto
    {
        public int ReferenceId { get; set; }

        public DateTime CreatedOn { get; set; }

        public string FicheNumber { get; set; }=string.Empty;

        public  Guid Customer { get; set; }

        public ProcurementFicheDto()
        {
            Lines = new();
		}

        public List<ProcurementFicheTransactionDto> Lines { get; set; }

	}
}
