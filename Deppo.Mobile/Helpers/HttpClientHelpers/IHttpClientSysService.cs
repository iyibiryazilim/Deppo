using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deppo.Mobile.Helpers.HttpClientHelpers;

public interface IHttpClientSysService
{
    HttpClient GetOrCreateHttpClient();

    public Guid UserOid { get; set; }
    string BaseUri { get; set; }
    string Token { get; set; }
    int FirmNumber { get; set; }
    int PeriodNumber { get; set; }
    string UserName { get; set; }
}