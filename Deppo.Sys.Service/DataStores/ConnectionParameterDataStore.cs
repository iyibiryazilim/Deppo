using Deppo.Sys.Service.Models;
using Deppo.Sys.Service.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Deppo.Sys.Service.DataStores
{
    public class ConnectionParameterDataStore : IConnectionParameterService
    {
        public string RequestUrl = "/api/odata/" + typeof(ConnectionParameter).Name;

        public async Task<IEnumerable<ConnectionParameter>> GetAllAsync(HttpClient httpClient)
        {
            var response = await httpClient.GetAsync(RequestUrl);
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonNode.Parse(content)["value"].Deserialize<IEnumerable<ConnectionParameter>>();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return Enumerable.Empty<ConnectionParameter>();
                }
                else
                {
                    return Enumerable.Empty<ConnectionParameter>();
                }
            }
            else
            {
                return Enumerable.Empty<ConnectionParameter>();
            }
        }

        public async Task<IEnumerable<ConnectionParameter>> GetAllAsync(HttpClient httpClient, string filter)
        {
            var response = await httpClient.GetAsync($"{RequestUrl}?{filter}");
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonNode.Parse(content)["value"].Deserialize<IEnumerable<ConnectionParameter>>();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return Enumerable.Empty<ConnectionParameter>();
                }
                else
                {
                    return Enumerable.Empty<ConnectionParameter>();
                }
            }
            else
            {
                return Enumerable.Empty<ConnectionParameter>();
            }
        }
    }
}