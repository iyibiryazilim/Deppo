using Deppo.Sys.Service.Models;
using Deppo.Sys.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.DataStores;

public class WarehouseProcessParameterDataStore : IWarehouseProcessParameterService
{
    public string RequestUrl = "/api/odata/" + typeof(WarehouseProcessParameter).Name;

    public async Task<IEnumerable<WarehouseProcessParameter>> GetAllAsync(HttpClient httpClient)
    {
        var response = await httpClient.GetAsync(RequestUrl);
        if (response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonNode.Parse(content)["value"].Deserialize<IEnumerable<WarehouseProcessParameter>>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return Enumerable.Empty<WarehouseProcessParameter>();
            }
            else
            {
                return Enumerable.Empty<WarehouseProcessParameter>();
            }
        }
        else
        {
            return Enumerable.Empty<WarehouseProcessParameter>();
        }
    }

    public async Task<IEnumerable<WarehouseProcessParameter>> GetAllAsync(HttpClient httpClient, string filter)
    {
        var response = await httpClient.GetAsync($"{RequestUrl}?{filter}");
        if (response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonNode.Parse(content)["value"].Deserialize<IEnumerable<WarehouseProcessParameter>>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return Enumerable.Empty<WarehouseProcessParameter>();
            }
            else
            {
                return Enumerable.Empty<WarehouseProcessParameter>();
            }
        }
        else
        {
            return Enumerable.Empty<WarehouseProcessParameter>();
        }
    }
}