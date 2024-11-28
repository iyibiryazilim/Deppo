using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services
{
	public interface IProcurementLocationTransactionService
	{
		Task<DataResult<IEnumerable<dynamic>>> GetObjects(HttpClient httpClient, int firmNumber, int periodNumber, int productReferenceId, int warehouseNumber, int skip = 0, int take = 20, string search = "", int locationRef = 0, int ficheReferenceId = 0, int serilotRef = 0, int variantReferenceId = 0);
	}
}
