using Deppo.Core.DataResultModel;
using Deppo.Core.Models;
using Deppo.Core.SortModels;


namespace Deppo.Core.Services;

public interface IWarehouseService
{
    Task<DataResult<IEnumerable<Warehouse>>> GetObjects(HttpClient httpClient,string search, SortModel? orderBy, int page, int pageSize, int firmNumber);
    Task<DataResult<IEnumerable<dynamic>>> GetObjectsAsync(HttpClient httpClient,int firmNumber, int periodNumber,  string search = "", int skip = 0, int take = 20);
	Task<DataResult<dynamic>> GetObjectById(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber);


}
