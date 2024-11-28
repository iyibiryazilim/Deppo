using Deppo.Core.DataResultModel;

namespace Deppo.Core.Services
{
	public interface IProcurementByLocationCustomerService
	{
		Task<DataResult<IEnumerable<dynamic>>> GetCustomers(HttpClient httpClient, int firmNumber, int periodNumber, int warehouseNumber, string productReferenceIds, string search = "", int skip = 0, int take = 20);
	}
}
