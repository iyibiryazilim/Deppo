using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Deppo.Mobile.Helpers.HttpClientHelpers;

public class HttpClientSysService : IHttpClientSysService
{
    private readonly Lazy<HttpClient> _httpClient = new Lazy<HttpClient>(
   () =>
   {
       var httpClient = new HttpClient();
       //httpClient.BaseAddress = new Uri("http://172.16.1.25:1923");
       httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
	   httpClient.Timeout = TimeSpan.FromMinutes(5);

	   return httpClient;
   }

   , LazyThreadSafetyMode.None);

    public Guid UserOid { get; set; }
    public string Token { get; set; } = string.Empty;
    public string BaseUri { get; set; } = string.Empty;
    public int FirmNumber { get; set; }
    public int PeriodNumber { get; set; }
    public string UserName { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;

	public HttpClient GetOrCreateHttpClient()
    {
        var httpClient = _httpClient.Value;
        if (httpClient.BaseAddress == null)
            httpClient.BaseAddress = new Uri(BaseUri, true);

        if (!string.IsNullOrEmpty(BaseUri))
        {
            if (!string.IsNullOrEmpty(Token))
            {
                var token = Token.Trim('"');

				if (IsValid(token))
				{
					if (httpClient.DefaultRequestHeaders.Authorization == null)
						httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
				}
				else
				{
					var refreshToken = RefreshToken(httpClient, UserName, Password).Result;

					if (!string.IsNullOrEmpty(refreshToken))
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
                httpClient.DefaultRequestHeaders.Authorization = null;
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
		return Task.Run(async () =>
		{
			string token = string.Empty;
			try
			{
				var serilazeData = JsonSerializer.Serialize(new { username, password });
				StringContent content = new StringContent(serilazeData, Encoding.UTF8, "application/json");
				HttpResponseMessage response = await httpClient.PostAsync($"api/Authentication/Authenticate", content);
				if (response.IsSuccessStatusCode)
				{
					var responseContent = await response.Content.ReadAsStringAsync();
					if (response.StatusCode == System.Net.HttpStatusCode.OK)
					{
						token = responseContent;
					}
					else
						token = string.Empty;
				}
				else
					token = string.Empty;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}

			return token;
		});
	}
}