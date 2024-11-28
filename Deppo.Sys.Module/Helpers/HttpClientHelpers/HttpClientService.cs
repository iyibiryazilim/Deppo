namespace Deppo.Sys.Module.Helpers.HttpClientHelpers;

public class HttpClientService : IHttpClientService
{
    private readonly Lazy<HttpClient> _httpClient = new Lazy<HttpClient>(
    () =>
    {
        var httpClient = new HttpClient();
        //httpClient.BaseAddress = new Uri("http://172.16.1.25:52789");
        httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

        return httpClient;
    }

    , LazyThreadSafetyMode.None);

    public string Token { get; set; } = string.Empty;
    public string BaseUri { get; set; } = string.Empty;
    public int FirmNumber { get; set; }
    public int PeriodNumber { get; set; }
    public string UserName { get; set; } = string.Empty;
    public HttpClient GetOrCreateHttpClient()
    {
        var httpClient = _httpClient.Value;
        if (httpClient.BaseAddress == null)
            httpClient.BaseAddress = new Uri(BaseUri);

        if (!string.IsNullOrEmpty(BaseUri))
        {
            if (!string.IsNullOrEmpty(Token))
            {
                var token = Token.Trim('"');
                if (httpClient.DefaultRequestHeaders.Authorization == null)
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
            }
            else
                httpClient.DefaultRequestHeaders.Authorization = null;
        }

        return httpClient;
    }
}
