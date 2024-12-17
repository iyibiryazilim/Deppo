using System;

namespace Deppo.Mobile.Helpers.HttpClientHelpers;


public interface IHttpClientService
{
    HttpClient GetOrCreateHttpClient();
    string BaseUri { get; set; }
    string Token { get; set; }
    int FirmNumber { get; set; }
    int PeriodNumber { get; set; }
    string ExternalDatabase { get; set; } 
}
