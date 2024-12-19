using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;

namespace Deppo.Mobile.Helpers.HttpClientHelpers;

public class HttpClientService : IHttpClientService
{
    private readonly Lazy<HttpClient> _httpClient = new Lazy<HttpClient>(
    () =>
    {
        var httpClient = new HttpClient();
        //httpClient.BaseAddress = new Uri("http://172.16.1.25:52789");
        httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        httpClient.Timeout = TimeSpan.FromMinutes(5);

		return httpClient;
    }

    , LazyThreadSafetyMode.None);

    public string Token { get; set; } = string.Empty;
    public string BaseUri { get; set; } = string.Empty;
    public int FirmNumber { get; set; }
    public int PeriodNumber { get; set; }
    public string UserName { get; set; } = string.Empty;
	public string ExternalDatabase { get; set; } = string.Empty;
	public HttpClient GetOrCreateHttpClient()
    {
        var httpClient = _httpClient.Value;
        if (httpClient.BaseAddress == null)
            httpClient.BaseAddress = new Uri(BaseUri,true);

        if (!string.IsNullOrEmpty(BaseUri))
        {
            if (!string.IsNullOrEmpty(Token))
            {
                var token = Token.Trim('"');

                if(IsValid(token))
                {
					if (httpClient.DefaultRequestHeaders.Authorization == null)
						httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
				}
                else
                {
                    var refreshToken = RefreshToken(httpClient, "Admin", "").Result;
                    if(!string.IsNullOrEmpty(refreshToken))
                    {
						if (httpClient.DefaultRequestHeaders.Authorization == null)
                        {
							Token = refreshToken.Trim('"');
							httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Token);

                        }
                        else
                        {
							Token = refreshToken.Trim('"');
						}
					}
                    else
                    {
					    httpClient.DefaultRequestHeaders.Authorization = null;
                    }
				}   
            }
            else
            {
				httpClient.DefaultRequestHeaders.Authorization = null;
			}
                
        }

        return httpClient;
    }

    private bool IsValid(string token)
    {
        JwtSecurityToken jwtSecurityToken;
        try
        {
            jwtSecurityToken = new(token);
		}
        catch (Exception)
        {
            return false;
        }

        return jwtSecurityToken.ValidTo > DateTime.UtcNow;

	}

	public Task<string> RefreshToken(HttpClient httpClient, string username, string password)
	{
      return  Task.Run(async () =>
        {
            string token = string.Empty;
            try
            {
                var responseMessage = await httpClient.PostAsync($"gateway/identity/Authentication/Authenticate",
                    new StringContent(JsonSerializer.Serialize(new { username, password }), Encoding.UTF8, "application/json"));

                if (responseMessage.IsSuccessStatusCode)
                {
                    if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var data = await responseMessage.Content.ReadAsStringAsync();
                        token = data.Trim('"');
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return token;
        });
	}

}
