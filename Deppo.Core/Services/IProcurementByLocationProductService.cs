using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services
{
	public interface IProcurementByLocationProductService
	{
		Task<DataResult<IEnumerable<dynamic>>> GetProducts(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, int locationReferenceId, string search = "", int skip = 0, int take = 20);
	}
}
