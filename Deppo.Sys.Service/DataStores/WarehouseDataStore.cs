using Deppo.Sys.Service.Models;
using Deppo.Sys.Service.Services;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Deppo.Sys.Service.DataStores;

public class WarehouseDataStore : IWarehouseService
{
    public string RequestUrl = "/api/odata/" + typeof(Warehouse).Name;

    public async Task<IEnumerable<Warehouse>> GetAllAsync(HttpClient httpClient)
    {
        var response = await httpClient.GetAsync(RequestUrl);
        if (response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonNode.Parse(content)["value"].Deserialize<IEnumerable<Warehouse>>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return Enumerable.Empty<Warehouse>();
            }
            else
            {
                return Enumerable.Empty<Warehouse>();
            }
        }
        else
        {
            return Enumerable.Empty<Warehouse>();
        }
    }

    public async Task<IEnumerable<Warehouse>> GetAllAsync(HttpClient httpClient, string filter)
    {
        var response = await httpClient.GetAsync($"{RequestUrl}?{filter}");
        if (response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonNode.Parse(content)["value"].Deserialize<IEnumerable<Warehouse>>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return Enumerable.Empty<Warehouse>();
            }
            else
            {
                return Enumerable.Empty<Warehouse>();
            }
        }
        else
        {
            return Enumerable.Empty<Warehouse>();
        }
    }
}