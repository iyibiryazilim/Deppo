using Deppo.Sys.Service.Models;
using Deppo.Sys.Service.Services;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Deppo.Sys.Service.DataStores
{
	public class ProcurementAuditDataStore : IProcurementAuditService
    {
        public string RequestUrl = "/api/odata/" + typeof(ProcurementAudit).Name;

        public async Task<IEnumerable<ProcurementAudit>> GetAllAsync(HttpClient httpClient)
        {
            var response = await httpClient.GetAsync(RequestUrl);
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonNode.Parse(content)["value"].Deserialize<IEnumerable<ProcurementAudit>>();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return Enumerable.Empty<ProcurementAudit>();
                }
                else
                {
                    return Enumerable.Empty<ProcurementAudit>();
                }
            }
            else
            {
                return Enumerable.Empty<ProcurementAudit>();
            }
        }

        public async Task<IEnumerable<ProcurementAudit>> GetAllAsync(HttpClient httpClient, string filter)
        {
            var response = await httpClient.GetAsync($"{RequestUrl}?{filter}");
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonNode.Parse(content)["value"].Deserialize<IEnumerable<ProcurementAudit>>();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return Enumerable.Empty<ProcurementAudit>();
                }
                else
                {
                    return Enumerable.Empty<ProcurementAudit>();
                }
            }
            else
            {
                return Enumerable.Empty<ProcurementAudit>();
            }
        }
    }
}