using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services;

public interface ISeriLotTransactionService
{
	Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, int warehouseNumber, int skip = 0, int take = 20, string search = "", string externalDb = "");
}
