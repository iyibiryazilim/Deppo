using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using Deppo.Sys.Service.Models;
using Deppo.Sys.Service.Services;

namespace Deppo.Sys.Service.DataStores;

public class ApplicationUserDataStore : IApplicationUserService
{
    public string RequestUrl = "/api/odata/" + typeof(ApplicationUser).Name;

    public async Task<IEnumerable<ApplicationUser>> GetAllAsync(HttpClient httpClient)
    {
        var response = await httpClient.GetAsync(RequestUrl);
        if (response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonNode.Parse(content)["value"].Deserialize<IEnumerable<ApplicationUser>>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return Enumerable.Empty<ApplicationUser>();
            }
            else
            {
                return Enumerable.Empty<ApplicationUser>();
            }
        }
        else
        {
            return Enumerable.Empty<ApplicationUser>();
        }
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllAsync(HttpClient httpClient, string filter)
    {
        var response = await httpClient.GetAsync($"{RequestUrl}?{filter}");
        if (response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonNode.Parse(content)["value"].Deserialize<IEnumerable<ApplicationUser>>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return Enumerable.Empty<ApplicationUser>();
            }
            else
            {
                return Enumerable.Empty<ApplicationUser>();
            }
        }
        else
        {
            return Enumerable.Empty<ApplicationUser>();
        }
    }
}
