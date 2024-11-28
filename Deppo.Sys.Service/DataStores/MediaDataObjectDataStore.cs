using Deppo.Sys.Service.DTOs;
using Deppo.Sys.Service.Models;
using Deppo.Sys.Service.Services;
using System.Text.Json;
using System.Text;

namespace Deppo.Sys.Service.DataStores
{
	public class MediaDataObjectDataStore : IMediaDataObjectService
	{
		public string RequestUrl = "/api/odata/" + typeof(MediaDataObject).Name;
		public async Task<MediaDataObject> CreateAsync(HttpClient httpClient, MediaDataObjectDto dto)
		{
			var json = JsonSerializer.Serialize(dto);

			var content = new StringContent(json, Encoding.UTF8, "application/json");
			var response = await httpClient.PostAsync(RequestUrl, content);
			if (response.IsSuccessStatusCode)
			{
				if (response.StatusCode == System.Net.HttpStatusCode.Created)
				{
					var responseContent = await response.Content.ReadAsStringAsync();
					return JsonSerializer.Deserialize<MediaDataObject>(responseContent);
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
	}
}
