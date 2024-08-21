using Deppo.Core.DataResultModel;
using Deppo.Core.Models;
using Deppo.Core.SortModels;

namespace Deppo.Core.Services;

public interface IWarehouseTransactionService
{
	Task<DataResult<IEnumerable<WarehouseTransaction>>> GetInputTransactionByWarehouseNumberAsync(HttpClient httpClient, int number, string search, SortModel? orderBy, int page, int pageSize, int firmNumber);

	Task<DataResult<IEnumerable<WarehouseTransaction>>> GetOutputTransactionByWarehouseNumberAsync(HttpClient httpClient, int number, string search, SortModel? orderBy, int page, int pageSize, int firmNumber);
}
