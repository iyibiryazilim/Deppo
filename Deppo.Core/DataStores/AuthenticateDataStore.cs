using System;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Deppo.Core.Services;

namespace Deppo.Core.DataStores;

public class AuthenticateDataStore : IAuthenticationService
{
public async Task<string> Authenticate(HttpClient httpClient, string username, string password)
	{
		string token = string.Empty;
		try
		{
			var responseMessage = await httpClient.PostAsync($"gateway/identity/Authentication/Authenticate",
				new StringContent(JsonSerializer.Serialize(new {username, password}), Encoding.UTF8, "application/json")); 

			if(responseMessage.IsSuccessStatusCode)
			{
				if(responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
				{
					var data = await responseMessage.Content.ReadAsStringAsync();
					token = data.Trim('"');
				}
			}
        }
		catch(Exception ex)
		{
			throw new Exception(ex.Message);
		}

		return token;
	}
}
