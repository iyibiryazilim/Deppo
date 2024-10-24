using Deppo.Sys.Service.DTOs;
using Deppo.Sys.Service.Models;

namespace Deppo.Sys.Service.Services
{
	public interface IMediaDataObjectService
	{
		public Task<MediaDataObject> CreateAsync(HttpClient httpClient, MediaDataObjectDto dto);
	}
}
