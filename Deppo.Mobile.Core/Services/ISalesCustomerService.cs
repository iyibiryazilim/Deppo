using Deppo.Core.DataResultModel;

namespace Deppo.Mobile.Core.Services;

public interface ISalesCustomerService
{
	public Task<DataResult<IEnumerable<dynamic>>> GetObjectsAsync(HttpClient httpClient, int firmNumber, int periodNumber, int customerReferenceId, int skip = 0, int take = 20, string search = "");
}
