using Deppo.Core.DataResultModel;
using Deppo.Core.Models;
using Deppo.Core.SortModels;

namespace Deppo.Core.Services;

public interface IProductTransactionLineService
{
	public Task<DataResult<IEnumerable<ProductTransaction>>> GetInputTransactionLineByProductId(HttpClient httpClient, int productId, string search, SortModel? orderBy, int page, int pageSize, int firmNumber);

	public Task<DataResult<IEnumerable<ProductTransaction>>> GetOutputTransactionLineByProductId(HttpClient httpClient, int productId, string search, SortModel? orderBy, int page, int pageSize, int firmNumber);
}
