using Deppo.Sys.Service.Models;

namespace Deppo.Sys.Service.Services;

public interface IWarehouseProcessParameterService
{
    public Task<IEnumerable<WarehouseProcessParameter>> GetAllAsync(HttpClient httpClient);

    public Task<IEnumerable<WarehouseProcessParameter>> GetAllAsync(HttpClient httpClient, string filter);
}