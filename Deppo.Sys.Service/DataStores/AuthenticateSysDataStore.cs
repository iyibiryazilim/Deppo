using Deppo.Sys.Service.Services;
using System.Text;
using System.Text.Json;

namespace Deppo.Sys.Service.DataStores
{
    public class AuthenticateSysDataStore : IAuthenticateSysService
    {
        private const string RequestUrl = "api/Authentication/Authenticate";

        public async Task<string> AuthenticateAsync(HttpClient httpClient, string username, string password)
        {
            string token;

            var serilazeData = JsonSerializer.Serialize(new { username, password });
            StringContent content = new StringContent(serilazeData, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await httpClient.PostAsync($"{RequestUrl}", content);
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

            return token;
        }
    }
}