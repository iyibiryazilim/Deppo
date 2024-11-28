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
    public class TaskListDataStore : ITaskListService
    {
        public string RequestUrl = "/api/odata/" + typeof(TaskList).Name;

        public async Task<IEnumerable<TaskList>> GetAllAsync(HttpClient httpClient)
        {
            var response = await httpClient.GetAsync(RequestUrl);
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonNode.Parse(content)["value"].Deserialize<IEnumerable<TaskList>>();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return Enumerable.Empty<TaskList>();
                }
                else
                {
                    return Enumerable.Empty<TaskList>();
                }
            }
            else
            {
                return Enumerable.Empty<TaskList>();
            }
        }

        public async Task<IEnumerable<TaskList>> GetAllAsync(HttpClient httpClient, string filter)
        {
            var response = await httpClient.GetAsync($"{RequestUrl}?{filter}");
            if (response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonNode.Parse(content)["value"].Deserialize<IEnumerable<TaskList>>();
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    return Enumerable.Empty<TaskList>();
                }
                else
                {
                    return Enumerable.Empty<TaskList>();
                }
            }
            else
            {
                return Enumerable.Empty<TaskList>();
            }
        }

        public async Task<TaskList> PatchObjectAsync(HttpClient httpClient, TaskList taskList, Guid oid)
        {
            var response = await httpClient.PatchAsync($"{RequestUrl}/({oid.ToString()})", new StringContent(JsonSerializer.Serialize(taskList), Encoding.UTF8, "application/json"));

			if (response.IsSuccessStatusCode)
			{
				if (response.StatusCode == System.Net.HttpStatusCode.OK)
				{
					var content = await response.Content.ReadAsStringAsync();
					return JsonNode.Parse(content).Deserialize<TaskList>();
				}
				else if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
				{
					return null;
				}
				else
				{
					return null;
				}
			}
			else
			{
				return null;
			}
		}
    }
}