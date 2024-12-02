using Deppo.Sys.Service.DTOs;
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

public class CustomerDataStore : ICustomerService
{
    public string RequestUrl = "/api/odata/" + typeof(Customer).Name;
    public async Task<Customer> CreateAsync(HttpClient httpClient, CustomerDto dto)
    {
        var json = JsonSerializer.Serialize(dto);

        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync(RequestUrl, content);
        if (response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<Customer>(responseContent);
            }
            else
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return null!;
            }
        }
        else
        {
            return null!;
        }
    }

    public async Task<IEnumerable<Customer>> GetAllAsync(HttpClient httpClient)
    {
        var response = await httpClient.GetAsync(RequestUrl);
        if (response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonNode.Parse(content)["value"].Deserialize<IEnumerable<Customer>>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return Enumerable.Empty<Customer>();
            }
            else
            {
                return Enumerable.Empty<Customer>();
            }
        }
        else
        {
            return Enumerable.Empty<Customer>();
        }
    }

    public async Task<IEnumerable<Customer>> GetAllAsync(HttpClient httpClient, string filter)
    {
        var response = await httpClient.GetAsync($"{RequestUrl}?{filter}");
        if (response.IsSuccessStatusCode)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonNode.Parse(content)["value"].Deserialize<IEnumerable<Customer>>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return Enumerable.Empty<Customer>();
            }
            else
            {
                return Enumerable.Empty<Customer>();
            }
        }
        else
        {
            return Enumerable.Empty<Customer>();
        }
    }
}
