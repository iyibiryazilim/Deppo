using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.Models;

public class ApplicationUser
{
    public Guid Oid { get; set; }
    public string Code { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string Tckn { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public string Telephone { get; set; } = string.Empty;

    public string OtherTelephone { get; set; } = string.Empty;
    public string WebAddress { get; set; } = string.Empty;
    public string EMail { get; set; } = string.Empty;
    public string ClientMachineName { get; set; } = string.Empty;
    public string LogoUserName { get; set; } = string.Empty;
    public MediaDataObject Image { get; set; } = string.Empty;
}