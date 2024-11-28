namespace Deppo.Sys.Module.Helpers.HttpClientHelpers;

public interface IHttpClientService
{
    HttpClient GetOrCreateHttpClient();
    string BaseUri { get; set; }
    string Token { get; set; }
    int FirmNumber { get; set; }
    int PeriodNumber { get; set; }
}
