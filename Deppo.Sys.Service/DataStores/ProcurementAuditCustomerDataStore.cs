using Deppo.Sys.Service.DTOs;
using Deppo.Sys.Service.Models;
using Deppo.Sys.Service.Services;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Deppo.Sys.Service.DataStores
{
    public class ProcurementAuditCustomerDataStore : IProcurementAuditCustomerService
    {
        public string RequestUrl = "/api/odata/" + typeof(ProcurementAuditCustomer).Name;

        public async Task<ProcurementAuditCustomer> CreateAsync(HttpClient httpClient, ProcurementAuditCustomerDto dto)
        {
            var json = JsonSerializer.Serialize(dto);

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(RequestUrl, content);
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Created)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ProcurementAuditCustomer>(responseContent);
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

        public async Task<IEnumerable<ProcurementAuditCustomer>> GetAllAsync(HttpClient httpClient)
        {
            var response = await httpClient.GetAsync(RequestUrl);
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonNode.Parse(content)["value"].Deserialize<IEnumerable<ProcurementAuditCustomer>>();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return Enumerable.Empty<ProcurementAuditCustomer>();
                }
                else
                {
                    return Enumerable.Empty<ProcurementAuditCustomer>();
                }
            }
            else
            {
                return Enumerable.Empty<ProcurementAuditCustomer>();
            }
        }

        public async Task<IEnumerable<ProcurementAuditCustomer>> GetAllAsync(HttpClient httpClient, string filter)
        {
            var response = await httpClient.GetAsync($"{RequestUrl}?{filter}");
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonNode.Parse(content)["value"].Deserialize<IEnumerable<ProcurementAuditCustomer>>();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return Enumerable.Empty<ProcurementAuditCustomer>();
                }
                else
                {
                    return Enumerable.Empty<ProcurementAuditCustomer>();
                }
            }
            else
            {
                return Enumerable.Empty<ProcurementAuditCustomer>();
            }
        }
    }
}