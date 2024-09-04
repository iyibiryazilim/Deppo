using Deppo.Core.DataResultModel;

namespace Deppo.Mobile.Core.Services;

public interface ISalesCustomerProductService
{
	public Task<DataResult<IEnumerable<dynamic>>> GetObjectsByCustomer(HttpClient httpClient, int firmNumber, int periodNumber, int customerReferenceId, int skip, int take);
}
