using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Deppo.Sys.Service.Models
{
    public class Current
    {
        public string Code { get; set; }=string.Empty;  
        public string Title { get; set; }=string.Empty;

        public string TaxOffice { get; set; } = string.Empty;

        public string TaxNumber { get; set; } = string.Empty;

        //public City MyProperty { get; set; }
        //public County MyProperty1 { get; set; }
        public string PostCode { get; set; } = string.Empty;
        //public Currency Currency { get; set; }
        public bool EInvoice { get; set; } = false;

        public string Telephone { get; set; } = string.Empty;
        public string OtherTelephone { get; set;} = string.Empty;
        public string Fax { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string WebAddress { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public decimal CreditLimit { get; set; } = default;

        public bool CreditHold { get; set; }

        public int ReferenceId { get; set; }

        public bool IsActive { get; set; }

        public bool IsPersonal { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public double CustomerDiscountRate { get; set; } = default;

        public string Tckn { get; set; } = string.Empty;

        public bool IsForeign { get; set; }

        public CustomerType CustomerType { get; set; }
        public int FirmNumber { get; set; }






    }
}
